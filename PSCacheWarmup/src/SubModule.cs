using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace PSCacheWarmup
{
    // Warms up vanilla's lazy MapDistanceModel cache when PlayerSettlement PLACES
    // (v0.2 hook 1) or completes (v0.1 hook, kept as fallback) a settlement, so
    // downstream systems (village trade binding, AI daily tick, quest generation)
    // don't trip a random A* pathfind stall on first distance query.
    //
    // See ModdingJournal.md sections 31 (v0.1 root-cause), 32 (v0.1.1 file logging),
    // 42 (v0.2 spec + Phase 1 agent findings).
    //
    // v0.2 additions:
    //   1. Subscribe to SettlementCreatedEvent (placement-time) alongside the
    //      existing SettlementBuildCompleteEvent (completion-time). Placement fires
    //      15-30 in-game days before completion; warming from placement means the
    //      construction timer silently drains the queue and the player never sees a
    //      stall on the completion toast.
    //   2. Persist the warmed navigation cache to disk (Configs/PSCacheWarmup/nav_cache.bin
    //      + sidecar .meta.json with sha256 of Settlement.StringId set). On next
    //      OnGameInitializationFinished, if the sidecar matches, Deserialize replaces
    //      the freshly-loaded vanilla+MNR cache with our warmed version — skips 5-8
    //      minutes of re-drain on repeat loads of the same campaign.
    //   3. Adaptive PairsPerTick: 3 when the sim is running, 6 when
    //      TimeControlMode == Stop (PS forces Stop during NotifyComplete). Uses more
    //      CPU when the player is already staring at a "wait" screen, less when they
    //      might notice a frame drop.
    //
    // Not implemented from the Phase 1 spec:
    //   - NavigationType.All warmup — DefaultMapDistanceModel holds a single
    //     _navigationCache field (see its RegisterDistanceCache) and ignores the
    //     navigationCapability parameter in GetDistance overloads. Warming with
    //     NavigationType.All would call the same GetDistance path as Default, so
    //     it's a no-op duplicate. Documented in journal §42.
    public class SubModule : MBSubModuleBase
    {
        private const int PairsPerTickNormal = 3;
        private const int PairsPerTickPaused = 6;
        private const int ProgressUpdateEveryPairs = 25;

        private const string CacheFileName = "nav_cache.bin";
        private const string MetaFileName  = "nav_cache.meta.json";

        private static string CacheDir  { get { return Path.Combine(BasePath.Name, "Configs", "PSCacheWarmup"); } }
        private static string CachePath { get { return Path.Combine(CacheDir, CacheFileName); } }
        private static string MetaPath  { get { return Path.Combine(CacheDir, MetaFileName); } }

        private static readonly Queue<Settlement> _warmupQueue = new Queue<Settlement>();
        private static readonly HashSet<string> _alreadyWarmed = new HashSet<string>();

        private static Settlement _currentTarget;
        private static List<Settlement> _pairsForCurrent;
        private static int _pairsIndex;
        private static int _pairsDone;
        private static int _pairsTotal;

        private static bool _subscribeAttempted;
        private static bool _subscribeSucceeded;
        private static bool _persistLoadAttempted;

        private static readonly string LogPath =
            Path.Combine(BasePath.Name, "Configs", "ModLogs", "PSCacheWarmup.log");
        private static readonly object LogLock = new object();

        private static void Log(string msg)
        {
            try
            {
                lock (LogLock)
                {
                    var line = string.Format("[{0:yyyy-MM-ddTHH:mm:ss.fff}] {1}\n", DateTime.Now, msg);
                    Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
                    File.AppendAllText(LogPath, line);
                }
            }
            catch { /* never break the mod for logging failure */ }
        }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Log("========== OnSubModuleLoad called (v0.2 DLL loaded) ==========");
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            var gtName = game.GameType?.GetType().Name ?? "<null>";
            Log("OnGameInitializationFinished, gtName='" + gtName + "'");
            // v0.2.1: accept both Campaign (Sandbox mode) and CampaignStoryMode
            // (Story mode). Both use identical campaign systems + PlayerSettlement
            // works in both. v0.2 only checked "Campaign" and silently skipped
            // subscribe for Story mode users, leaving MNR+PS lag unresolved.
            if (gtName != "Campaign" && gtName != "CampaignStoryMode")
            {
                Log("  gtName not in {Campaign, CampaignStoryMode} -> skipping subscribe + persist load (non-campaign game)");
                return;
            }
            TrySubscribeToPS();
            TryLoadPersistedCache();
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);

            var campaign = Campaign.Current;
            if (campaign == null || campaign.Models?.MapDistanceModel == null) return;

            if (_currentTarget == null)
            {
                if (_warmupQueue.Count == 0) return;
                BeginNextTarget();
                if (_currentTarget == null) return;
            }

            var pairsPerTick = (campaign.TimeControlMode == CampaignTimeControlMode.Stop)
                ? PairsPerTickPaused
                : PairsPerTickNormal;

            var model = campaign.Models.MapDistanceModel;
            for (int i = 0; i < pairsPerTick && _pairsIndex < _pairsForCurrent.Count; i++)
            {
                var other = _pairsForCurrent[_pairsIndex++];
                if (other == null || other == _currentTarget) continue;
                try
                {
                    model.GetDistance(_currentTarget, other, false, false, MobileParty.NavigationType.Default);
                }
                catch (Exception ex)
                {
                    Log("  A* threw on pair (" + _currentTarget.StringId + " -> " + other.StringId + "): " + ex.GetType().Name + " " + ex.Message);
                }
                _pairsDone++;
            }

            if (_pairsDone > 0 && (_pairsDone % ProgressUpdateEveryPairs == 0))
            {
                var msg = string.Format("[PS Cache Warmup] {0} / {1} pairs...", _pairsDone, _pairsTotal);
                InformationManager.DisplayMessage(new InformationMessage(msg));
                Log("  progress " + _pairsDone + " / " + _pairsTotal + " (rate=" + pairsPerTick + "/tick)");
            }

            if (_pairsIndex >= _pairsForCurrent.Count)
            {
                var name = _currentTarget.Name != null ? _currentTarget.Name.ToString() : _currentTarget.StringId;
                var doneMsg = string.Format("[PS Cache Warmup] done for '{0}' ({1} pairs).", name, _pairsTotal);
                InformationManager.DisplayMessage(new InformationMessage(doneMsg));
                Log("done for '" + name + "' (" + _pairsTotal + " pairs). queueRemaining=" + _warmupQueue.Count);

                _currentTarget = null;
                _pairsForCurrent = null;
                _pairsIndex = 0;
                _pairsTotal = 0;
                _pairsDone = 0;

                // Persist after final drain (when queue is empty). Skips disk I/O
                // mid-multi-settlement session, groups all pairs into one save.
                if (_warmupQueue.Count == 0)
                {
                    TrySavePersistedCache();
                }
            }
        }

        private static void BeginNextTarget()
        {
            _currentTarget = _warmupQueue.Dequeue();
            if (_currentTarget == null)
            {
                Log("BeginNextTarget: dequeued NULL settlement, skipping");
                return;
            }

            _pairsForCurrent = new List<Settlement>(Settlement.All);
            _pairsIndex = 0;
            _pairsDone = 0;
            _pairsTotal = Math.Max(0, _pairsForCurrent.Count - 1);

            var name = _currentTarget.Name != null ? _currentTarget.Name.ToString() : _currentTarget.StringId;
            Log("BeginNextTarget: target='" + name + "' (StringId=" + _currentTarget.StringId + "), pairs=" + _pairsTotal);
            var startMsg = string.Format("[PS Cache Warmup] warming distance cache for '{0}' ({1} pairs). "
                + "Expect brief loading -- settlements can be added to travel plans safely.",
                name, _pairsTotal);
            InformationManager.DisplayMessage(new InformationMessage(startMsg));
        }

        private static void TrySubscribeToPS()
        {
            if (_subscribeAttempted)
            {
                Log("TrySubscribeToPS: already attempted (succeeded=" + _subscribeSucceeded + "), skipping");
                return;
            }
            _subscribeAttempted = true;
            Log("TrySubscribeToPS: start");

            try
            {
                var psBehType = AccessTools.TypeByName(
                    "BannerlordPlayerSettlement.Behaviours.PlayerSettlementBehaviour");
                if (psBehType == null)
                {
                    Log("  psBehType NULL - PlayerSettlement mod not loaded (or namespace changed). Silent no-op.");
                    return;
                }
                Log("  psBehType resolved: " + psBehType.AssemblyQualifiedName);

                // v0.1 hook: completion-time (safety net for old saves where placement already happened).
                bool completeOk = TrySubscribeOneEvent(psBehType, "SettlementBuildCompleteEvent",
                    new Action<Settlement>(OnPSSettlementComplete));

                // v0.2 hook 1: placement-time (main warmup path - fires day 0, drains
                // during build timer, player never sees stall on completion).
                bool createOk = TrySubscribeOneEvent(psBehType, "SettlementCreatedEvent",
                    new Action<Settlement>(OnPSSettlementCreated));

                if (completeOk || createOk)
                {
                    _subscribeSucceeded = true;
                    InformationManager.DisplayMessage(new InformationMessage(
                        "[PS Cache Warmup] v0.2 subscribed to PlayerSettlement (Created="
                        + (createOk ? "yes" : "NO") + ", BuildComplete=" + (completeOk ? "yes" : "NO")
                        + "). Log at Configs\\ModLogs\\PSCacheWarmup.log"));
                }
                else
                {
                    _subscribeAttempted = false; // retry next campaign init
                }
            }
            catch (Exception ex)
            {
                Log("  SUBSCRIBE FAILED with exception: " + ex.GetType().Name + " " + ex.Message + "\n" + ex.StackTrace);
                InformationManager.DisplayMessage(new InformationMessage(
                    "[PS Cache Warmup] subscribe failed: " + ex.Message));
            }
        }

        private static bool TrySubscribeOneEvent(Type psBehType, string propertyName, Action<Settlement> handler)
        {
            var eventProp = AccessTools.Property(psBehType, propertyName);
            if (eventProp == null)
            {
                Log("  " + propertyName + " NULL - property not found on " + psBehType.FullName);
                return false;
            }
            var evt = eventProp.GetValue(null);
            if (evt == null)
            {
                Log("  " + propertyName + " evt NULL - PS.Instance not ready. Skipping this event (will retry on next init if all failed).");
                return false;
            }
            var addListener = AccessTools.Method(evt.GetType(), "AddNonSerializedListener");
            if (addListener == null)
            {
                Log("  " + propertyName + " AddNonSerializedListener NULL on " + evt.GetType().FullName);
                return false;
            }
            addListener.Invoke(evt, new object[] { new object(), handler });
            Log("  " + propertyName + " SUBSCRIBE SUCCEEDED (handler=" + handler.Method.Name + ")");
            return true;
        }

        private static void OnPSSettlementCreated(Settlement s)
        {
            if (s == null)
            {
                Log("OnPSSettlementCreated: received NULL, ignoring");
                return;
            }
            if (_alreadyWarmed.Contains(s.StringId))
            {
                Log("OnPSSettlementCreated: '" + s.StringId + "' already warmed, dedup");
                return;
            }
            _warmupQueue.Enqueue(s);
            _alreadyWarmed.Add(s.StringId);
            var name = s.Name != null ? s.Name.ToString() : s.StringId;
            Log("OnPSSettlementCreated: PLACEMENT event for '" + name + "' (StringId=" + s.StringId + "), queueSize=" + _warmupQueue.Count);
            InformationManager.DisplayMessage(new InformationMessage(
                string.Format("[PS Cache Warmup] queued '{0}' at placement — silent warmup during build timer.", name)));
        }

        private static void OnPSSettlementComplete(Settlement s)
        {
            if (s == null)
            {
                Log("OnPSSettlementComplete: received NULL, ignoring");
                return;
            }
            if (_alreadyWarmed.Contains(s.StringId))
            {
                Log("OnPSSettlementComplete: '" + s.StringId + "' already warmed (likely queued at placement), skipping");
                return;
            }
            _warmupQueue.Enqueue(s);
            _alreadyWarmed.Add(s.StringId);
            var name = s.Name != null ? s.Name.ToString() : s.StringId;
            Log("OnPSSettlementComplete: PS event FIRED for '" + name + "' (StringId=" + s.StringId + "), queueSize=" + _warmupQueue.Count);
            InformationManager.DisplayMessage(new InformationMessage(
                string.Format("[PS Cache Warmup] queued '{0}' at completion (placement hook was missed).", name)));
        }

        // v0.2 hook 2 — persistence
        //
        // Serialize the NavigationCache<Settlement> after a full drain, so the next
        // load of the same campaign can Deserialize instead of re-A*-drain 989 pairs
        // (5-8 min real wall time). Sidecar .meta.json holds sha256 of the sorted
        // Settlement.StringId set — mismatch means the world composition changed
        // (different playthrough, different mods) so we skip the load.

        private static string ComputeSettlementHash()
        {
            var ids = Settlement.All.Select(s => s.StringId).OrderBy(s => s, StringComparer.Ordinal).ToArray();
            var joined = string.Join("|", ids);
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(joined));
                var sb = new StringBuilder(bytes.Length * 2);
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString().Substring(0, 32);
            }
        }

        private static void TryLoadPersistedCache()
        {
            if (_persistLoadAttempted) return;
            _persistLoadAttempted = true;

            if (!File.Exists(CachePath) || !File.Exists(MetaPath))
            {
                Log("TryLoadPersistedCache: no persisted cache at " + CachePath);
                return;
            }

            try
            {
                var metaJson = File.ReadAllText(MetaPath);
                var currentHash = ComputeSettlementHash();
                if (metaJson.IndexOf("\"settlement_hash\": \"" + currentHash + "\"", StringComparison.Ordinal) < 0)
                {
                    Log("TryLoadPersistedCache: sidecar hash mismatch. Current world hash=" + currentHash + ". Meta:\n" + metaJson);
                    return;
                }

                var mapDistModel = Campaign.Current?.Models?.MapDistanceModel;
                if (mapDistModel == null)
                {
                    Log("TryLoadPersistedCache: MapDistanceModel not ready");
                    return;
                }
                var navCacheField = AccessTools.Field(mapDistModel.GetType(), "_navigationCache");
                if (navCacheField == null)
                {
                    Log("TryLoadPersistedCache: _navigationCache field not found on " + mapDistModel.GetType().FullName);
                    return;
                }
                var navCache = navCacheField.GetValue(mapDistModel);
                if (navCache == null)
                {
                    Log("TryLoadPersistedCache: _navigationCache instance is null");
                    return;
                }

                var deserialize = AccessTools.Method(navCache.GetType(), "Deserialize", new[] { typeof(string) });
                if (deserialize == null)
                {
                    Log("TryLoadPersistedCache: Deserialize method not found on " + navCache.GetType().FullName);
                    return;
                }

                deserialize.Invoke(navCache, new object[] { CachePath });
                Log("TryLoadPersistedCache: SUCCESS — restored " + CachePath + " (hash " + currentHash + ")");
                InformationManager.DisplayMessage(new InformationMessage(
                    "[PS Cache Warmup] restored persisted cache from previous session — no warmup needed this load."));
            }
            catch (Exception ex)
            {
                Log("TryLoadPersistedCache FAILED: " + ex.GetType().Name + " " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private static void TrySavePersistedCache()
        {
            try
            {
                var mapDistModel = Campaign.Current?.Models?.MapDistanceModel;
                if (mapDistModel == null) { Log("TrySavePersistedCache: no MapDistanceModel"); return; }
                var navCacheField = AccessTools.Field(mapDistModel.GetType(), "_navigationCache");
                var navCache = navCacheField?.GetValue(mapDistModel);
                if (navCache == null) { Log("TrySavePersistedCache: _navigationCache null"); return; }

                var serialize = AccessTools.Method(navCache.GetType(), "Serialize", new[] { typeof(string) });
                if (serialize == null) { Log("TrySavePersistedCache: Serialize method not found on " + navCache.GetType().FullName); return; }

                Directory.CreateDirectory(CacheDir);
                serialize.Invoke(navCache, new object[] { CachePath });

                var hash = ComputeSettlementHash();
                var meta = "{\n"
                    + "  \"saved_at\": \"" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "\",\n"
                    + "  \"settlement_count\": " + Settlement.All.Count + ",\n"
                    + "  \"settlement_hash\": \"" + hash + "\",\n"
                    + "  \"version\": \"v0.2\"\n"
                    + "}\n";
                File.WriteAllText(MetaPath, meta);
                Log("TrySavePersistedCache: SUCCESS — cache=" + CachePath + " hash=" + hash + " count=" + Settlement.All.Count);
                InformationManager.DisplayMessage(new InformationMessage(
                    "[PS Cache Warmup] cache persisted. Next load of this save will skip warmup."));
            }
            catch (Exception ex)
            {
                Log("TrySavePersistedCache FAILED: " + ex.GetType().Name + " " + ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
