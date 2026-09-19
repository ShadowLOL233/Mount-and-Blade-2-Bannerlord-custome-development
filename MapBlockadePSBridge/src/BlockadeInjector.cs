using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace MapBlockadePSBridge
{
    // Design note: all diagnostics go through BridgeLog so we get file + message-feed
    // output. Message-feed alone was lost when the user missed the pop-up.
    internal static class BlockadeInjector
    {
        // Radius (campaign map units) inside which map faces become blocked around the
        // player's settlement. Vanilla settlement outlines in the XML span roughly 10-30
        // units in diameter; 5.0 is a conservative first guess. Tunable in Phase 2B.
        private const float BlockedRadius = 5.0f;

        // Cached reflection handles resolved once at first subscription attempt.
        private static Type _cacheType;
        private static Type _blockadeCityDataType;
        private static Type _wallGateDataType;
        private static Type _reachabilityGraphType;
        private static FieldInfo _citiesField;
        private static FieldInfo _faceToCityField;
        private static FieldInfo _gateFaceToCityField;
        private static FieldInfo _wallGateByFaceField;
        private static MethodInfo _rebuildAllMethod;
        private static MethodInfo _tryGetFaceCenterMethod;
        private static FieldInfo _maxFaceIndexField;
        private static bool _resolved;
        private static string _resolveError;

        internal static string TrySubscribe(object anchor)
        {
            BridgeLog.WriteFile("TrySubscribe start");
            if (!TryResolveReflection())
                return "not subscribed - MapBlockade reflection failed: " + (_resolveError ?? "unknown");
            BridgeLog.WriteFile("MapBlockade reflection resolved OK");

            var psBehaviourType = FindType("BannerlordPlayerSettlement.Behaviours.PlayerSettlementBehaviour");
            if (psBehaviourType == null)
                return "not subscribed - PlayerSettlementBehaviour type not found (is PlayerSettlement enabled?)";
            BridgeLog.WriteFile("PlayerSettlementBehaviour type found: " + psBehaviourType.AssemblyQualifiedName);

            var eventProp = psBehaviourType.GetProperty("SettlementBuildCompleteEvent",
                BindingFlags.Public | BindingFlags.Static);
            if (eventProp == null)
                return "not subscribed - SettlementBuildCompleteEvent property not found";

            var eventObj = eventProp.GetValue(null);
            if (eventObj == null)
                return "not subscribed - event object null (PSB.Instance not initialised yet? try loading a save first)";
            BridgeLog.WriteFile("event object type: " + eventObj.GetType().FullName);

            var addMethod = eventObj.GetType().GetMethod("AddNonSerializedListener");
            if (addMethod == null)
                return "not subscribed - AddNonSerializedListener not found on " + eventObj.GetType().Name;

            Action<Settlement> callback = OnPlayerSettlementBuilt;
            addMethod.Invoke(eventObj, new object[] { anchor, callback });

            return "subscribed to SettlementBuildCompleteEvent (radius=" + BlockedRadius + ")";
        }

        // Retroactively inject any town/castle that already exists in Settlement.All but
        // MapBlockade doesn't know about. Handles the "player built the city in a previous
        // session before this mod existed / was loaded" case, where SettlementBuildCompleteEvent
        // already fired and won't replay.
        internal static int InjectExistingPlayerSettlements()
        {
            if (!_resolved)
            {
                BridgeLog.WriteFile("retro scan aborted: reflection not resolved");
                return 0;
            }

            // Force MapBlockade to run its XML-load initialisation. Public API like
            // TryGetFaceCenter fires EnsureInitialized() internally.
            try
            {
                var forceArgs = new object[] { 0, Vec2.Zero };
                _tryGetFaceCenterMethod.Invoke(null, forceArgs);
            }
            catch (Exception ex)
            {
                BridgeLog.WriteFile("retro scan: force-init call threw " + ex.GetType().Name + ": " + ex.Message);
            }

            var cities = (IList)_citiesField.GetValue(null);
            var settlementField = _blockadeCityDataType.GetField("Settlement",
                BindingFlags.Public | BindingFlags.Instance);
            var tracked = new HashSet<Settlement>();
            foreach (var cityObj in cities)
            {
                if (cityObj == null) continue;
                var s = settlementField.GetValue(cityObj) as Settlement;
                if (s != null) tracked.Add(s);
            }
            BridgeLog.WriteFile("retro scan: MapBlockade already tracks " + tracked.Count + " settlements");

            int seen = 0, candidates = 0, injected = 0, skipped = 0;
            foreach (var s in Settlement.All)
            {
                seen++;
                if (s == null) continue;
                if (!s.IsTown && !s.IsCastle) continue;
                if (tracked.Contains(s)) { skipped++; continue; }
                candidates++;
                BridgeLog.WriteFile("retro candidate: " + s.StringId + " '" + (s.Name != null ? s.Name.ToString() : "?") + "' town=" + s.IsTown + " castle=" + s.IsCastle);
                try
                {
                    InjectBlockade(s);
                    injected++;
                }
                catch (Exception ex)
                {
                    BridgeLog.WriteFile("retro inject failed for " + s.StringId + ": " + ex.GetType().Name + " " + ex.Message);
                }
            }
            BridgeLog.WriteFile("retro scan done: seen=" + seen + " tracked=" + tracked.Count
                + " candidates=" + candidates + " injected=" + injected + " already_tracked_skipped=" + skipped);
            return injected;
        }

        private static bool TryResolveReflection()
        {
            if (_resolved) return true;

            _cacheType = FindType("MapBlockade.BlockadeReachabilityCache");
            if (_cacheType == null)
            {
                _resolveError = "BlockadeReachabilityCache type not found (MapBlockade not loaded?)";
                BridgeLog.WriteFile("reflection: " + _resolveError);
                // Diagnostic: dump loaded assemblies containing 'MapBlockade' in the name
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (asm.FullName != null && asm.FullName.IndexOf("MapBlockade", StringComparison.OrdinalIgnoreCase) >= 0)
                        BridgeLog.WriteFile("  seen asm: " + asm.FullName);
                }
                return false;
            }
            BridgeLog.WriteFile("reflection: found " + _cacheType.AssemblyQualifiedName);

            var bfNested = BindingFlags.NonPublic | BindingFlags.Public;
            _blockadeCityDataType = _cacheType.GetNestedType("BlockadeCityData", bfNested);
            _wallGateDataType = _cacheType.GetNestedType("WallGateData", bfNested);

            _reachabilityGraphType = FindType("MapBlockade.Algorithms.ReachabilityGraph");

            var bfPrivStatic = BindingFlags.NonPublic | BindingFlags.Static;
            var bfPubStatic = BindingFlags.Public | BindingFlags.Static;
            var bfAllStatic = bfPrivStatic | bfPubStatic;

            _citiesField = _cacheType.GetField("_cities", bfPrivStatic);
            _faceToCityField = _cacheType.GetField("_faceToCity", bfPrivStatic);
            _gateFaceToCityField = _cacheType.GetField("_gateFaceToCity", bfPrivStatic);
            _wallGateByFaceField = _cacheType.GetField("_wallGateByFace", bfPrivStatic);
            _rebuildAllMethod = _cacheType.GetMethod("RebuildAll", bfAllStatic, null, new[] { typeof(string) }, null);
            _tryGetFaceCenterMethod = _cacheType.GetMethod("TryGetFaceCenter", bfPubStatic);
            _maxFaceIndexField = _reachabilityGraphType?.GetField("MaxFaceIndex", bfAllStatic);

            var missing = new List<string>();
            if (_blockadeCityDataType == null) missing.Add("BlockadeCityData");
            if (_wallGateDataType == null) missing.Add("WallGateData");
            if (_reachabilityGraphType == null) missing.Add("ReachabilityGraph");
            if (_citiesField == null) missing.Add("_cities");
            if (_faceToCityField == null) missing.Add("_faceToCity");
            if (_gateFaceToCityField == null) missing.Add("_gateFaceToCity");
            if (_wallGateByFaceField == null) missing.Add("_wallGateByFace");
            if (_rebuildAllMethod == null) missing.Add("RebuildAll(string)");
            if (_tryGetFaceCenterMethod == null) missing.Add("TryGetFaceCenter");
            if (_maxFaceIndexField == null) missing.Add("ReachabilityGraph.MaxFaceIndex");

            if (missing.Count > 0)
            {
                _resolveError = "missing: " + string.Join(", ", missing.ToArray());
                return false;
            }

            _resolved = true;
            return true;
        }

        private static Type FindType(string fullName)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var t = asm.GetType(fullName);
                    if (t != null) return t;
                }
                catch { /* skip broken assemblies */ }
            }
            return null;
        }

        private static void OnPlayerSettlementBuilt(Settlement settlement)
        {
            if (settlement == null) return;
            try
            {
                if (!settlement.IsTown && !settlement.IsCastle)
                {
                    Log("skipped " + settlement.StringId + ": not town/castle");
                    return;
                }
                InjectBlockade(settlement);
            }
            catch (Exception ex)
            {
                Log("inject failed for " + settlement.StringId + ": " + ex.GetType().Name + " " + ex.Message);
            }
        }

        private static void InjectBlockade(Settlement settlement)
        {
            // Dedup: if MapBlockade already tracks this Settlement (either from its own
            // XML load or from a previous injection this session), skip. Two paths can
            // race for the same settlement: the event handler and the retro scan.
            var citiesForDedup = (IList)_citiesField.GetValue(null);
            var settlementField = _blockadeCityDataType.GetField("Settlement",
                BindingFlags.Public | BindingFlags.Instance);
            foreach (var cityObj in citiesForDedup)
            {
                if (cityObj == null) continue;
                if ((Settlement)settlementField.GetValue(cityObj) == settlement)
                {
                    Log("skip inject: " + settlement.StringId + " already tracked");
                    return;
                }
            }

            var pos = settlement.Position.ToVec2();

            int maxFi = (int)_maxFaceIndexField.GetValue(null);
            var blockedFis = new HashSet<int>();
            int gateFi = -1;
            float gateDistSq = float.MaxValue;
            float radiusSq = BlockedRadius * BlockedRadius;

            var args = new object[2];
            for (int fi = 0; fi <= maxFi; fi++)
            {
                args[0] = fi;
                args[1] = Vec2.Zero;
                var ok = (bool)_tryGetFaceCenterMethod.Invoke(null, args);
                if (!ok) continue;
                var center = (Vec2)args[1];
                var dx = center.x - pos.x;
                var dy = center.y - pos.y;
                var dsq = dx * dx + dy * dy;
                if (dsq > radiusSq) continue;
                blockedFis.Add(fi);
                if (dsq < gateDistSq) { gateDistSq = dsq; gateFi = fi; }
            }

            if (blockedFis.Count == 0 || gateFi < 0)
            {
                Log(settlement.StringId + " @ (" + pos.x.ToString("F1") + "," + pos.y.ToString("F1")
                    + "): no faces within radius " + BlockedRadius);
                return;
            }

            var cities = (IList)_citiesField.GetValue(null);
            int newIdx = cities.Count;

            var data = Activator.CreateInstance(_blockadeCityDataType);
            SetField(data, "Settlement", settlement);
            SetField(data, "CityIndex", newIdx);
            SetField(data, "BlockedFis", blockedFis);
            SetField(data, "CurrentKingdom", GetKingdom(settlement));
            SetField(data, "LastKingdom", GetKingdom(settlement));
            // GraceUntil keeps its field-initialiser default (CampaignTime.Never)

            cities.Add(data);

            var faceToCity = (IDictionary)_faceToCityField.GetValue(null);
            foreach (int fi in blockedFis) faceToCity[fi] = newIdx;

            var gateFaceToCity = (IDictionary)_gateFaceToCityField.GetValue(null);
            gateFaceToCity[gateFi] = newIdx;

            var wallGateByFace = (IDictionary)_wallGateByFaceField.GetValue(null);
            var gate = Activator.CreateInstance(_wallGateDataType);
            SetField(gate, "WallId", 0);
            SetField(gate, "CityIndex", newIdx);
            SetField(gate, "OutlineIndex", 0);
            SetField(gate, "GatePosition", pos);
            SetField(gate, "GateFaceIndex", gateFi);
            wallGateByFace[gateFi] = gate;

            _rebuildAllMethod.Invoke(null, new object[] { "PSBridge:" + settlement.StringId });

            Log("injected " + settlement.StringId + " @ (" + pos.x.ToString("F1") + "," + pos.y.ToString("F1")
                + ") idx=" + newIdx + " faces=" + blockedFis.Count + " gate=" + gateFi);
        }

        private static Kingdom GetKingdom(Settlement s)
        {
            var clan = s.OwnerClan;
            return clan != null ? clan.Kingdom : null;
        }

        private static void SetField(object target, string name, object value)
        {
            var t = target.GetType();
            var f = t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (f == null) throw new InvalidOperationException("field '" + name + "' not found on " + t.FullName);
            f.SetValue(target, value);
        }

        private static void Log(string msg)
        {
            BridgeLog.Both(msg);
        }
    }
}
