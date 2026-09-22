using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace PSCacheWarmup
{
    // Warms up vanilla's lazy MapDistanceModel cache after PlayerSettlement finishes
    // building a settlement, so subsequent gameplay doesn't hit random A* pathfind
    // stalls when systems (village trade binding, AI daily tick, quest generation)
    // first query distances involving the new settlement.
    //
    // See ModdingJournal.md section 31 for root-cause decompile summary.
    //
    // v0.1.1: adds file logging to Configs\ModLogs\PSCacheWarmup.log so we can
    // diagnose whether subscribe / callback / drain actually ran, without
    // depending on in-game message bar (which gets flooded by other mods'
    // notifications and is impossible to scroll back through).
    public class SubModule : MBSubModuleBase
    {
        private const int PairsPerTick = 3;
        private const int ProgressUpdateEveryPairs = 25;

        private static readonly Queue<Settlement> _warmupQueue = new Queue<Settlement>();
        private static Settlement _currentTarget;
        private static List<Settlement> _pairsForCurrent;
        private static int _pairsIndex;
        private static int _pairsDone;
        private static int _pairsTotal;

        private static bool _subscribeAttempted;
        private static bool _subscribeSucceeded;

        // File log so diagnosis doesn't rely on watching in-game message bar.
        private static readonly string LogPath =
            Path.Combine(BasePath.Name, "Configs", "ModLogs", "PSCacheWarmup.log");
        private static readonly object LogLock = new object();

        private static void Log(string msg)
        {
            try
            {
                lock (LogLock)
                {
                    var line = string.Format("[{0:yyyy-MM-ddTHH:mm:ss.fff}] {1}\n",
                        DateTime.Now, msg);
                    Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
                    File.AppendAllText(LogPath, line);
                }
            }
            catch
            {
                // never break the mod for logging failure
            }
        }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Log("========== OnSubModuleLoad called (mod DLL loaded successfully) ==========");
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            var gtName = game.GameType?.GetType().Name ?? "<null>";
            Log("OnGameInitializationFinished called, gtName='" + gtName + "'");
            if (gtName != "Campaign")
            {
                Log("  gtName != 'Campaign' -> skipping subscribe (no-op for non-campaign games)");
                return;
            }
            TrySubscribeToPS();
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

            var model = campaign.Models.MapDistanceModel;
            for (int i = 0; i < PairsPerTick && _pairsIndex < _pairsForCurrent.Count; i++)
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
                Log("  progress " + _pairsDone + " / " + _pairsTotal);
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

                var eventProp = AccessTools.Property(psBehType, "SettlementBuildCompleteEvent");
                if (eventProp == null)
                {
                    Log("  eventProp NULL - property 'SettlementBuildCompleteEvent' not found on " + psBehType.FullName);
                    return;
                }
                Log("  eventProp resolved: " + eventProp.PropertyType.Name);

                var evt = eventProp.GetValue(null);
                if (evt == null)
                {
                    Log("  evt NULL - PS.Instance not yet initialized, cannot subscribe. Will retry on next campaign init.");
                    _subscribeAttempted = false;   // allow retry next time
                    return;
                }
                Log("  evt resolved: type=" + evt.GetType().FullName);

                var addListener = AccessTools.Method(evt.GetType(), "AddNonSerializedListener");
                if (addListener == null)
                {
                    Log("  addListener NULL - method 'AddNonSerializedListener' not found on " + evt.GetType().FullName);
                    return;
                }
                Log("  addListener resolved: " + addListener);

                Action<Settlement> handler = OnPSSettlementComplete;
                addListener.Invoke(evt, new object[] { new object(), handler });
                _subscribeSucceeded = true;
                Log("  SUBSCRIBE SUCCEEDED. Waiting for PS SettlementBuildCompleteEvent to fire...");

                InformationManager.DisplayMessage(new InformationMessage(
                    "[PS Cache Warmup] v0.1.1 subscribed to PlayerSettlement build events. "
                    + "Log at Configs\\ModLogs\\PSCacheWarmup.log"));
            }
            catch (Exception ex)
            {
                Log("  SUBSCRIBE FAILED with exception: " + ex.GetType().Name + " " + ex.Message + "\n" + ex.StackTrace);
                InformationManager.DisplayMessage(new InformationMessage(
                    "[PS Cache Warmup] subscribe failed: " + ex.Message));
            }
        }

        private static void OnPSSettlementComplete(Settlement s)
        {
            if (s == null)
            {
                Log("OnPSSettlementComplete: received NULL settlement, ignoring");
                return;
            }
            _warmupQueue.Enqueue(s);
            var name = s.Name != null ? s.Name.ToString() : s.StringId;
            Log("OnPSSettlementComplete: PS event FIRED for '" + name + "' (StringId=" + s.StringId + "), queueSize=" + _warmupQueue.Count);
            InformationManager.DisplayMessage(new InformationMessage(
                string.Format("[PS Cache Warmup] queued '{0}' for warmup (queue size {1}).",
                    name, _warmupQueue.Count)));
        }
    }
}
