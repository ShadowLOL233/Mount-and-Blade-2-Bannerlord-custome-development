using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using UnifiedTroopManager.Data;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // B6-B · Formation assignment via Mission.SpawnTroop Postfix.
    //
    // After vanilla spawns an agent (for either side) we check:
    //   1. Is it the player side?
    //   2. Does the player's PartyPlan have a Formation preference for this
    //      troop's StringId?
    //   3. If yes, reassign agent.Formation to the planned slot (0-7 →
    //      FormationClass enum maps directly).
    //
    // Split plans (one troop type → multiple Formations) are honored via
    // round-robin: a static per-StringId counter distributes agents evenly
    // across the listed slots in encounter order. Counter is reset by
    // PlayerEncounterFinishPatch so each battle starts fresh.
    //
    // Must be used TOGETHER with OrderOfBattleInitializePatch which opens
    // every Formation's Classes[0].Class to NumberOfAllFormations · otherwise
    // vanilla's class-based filtering can refuse the reassignment.
    //
    // Signature of vanilla Mission.SpawnTroop (v1.4.7):
    //   public Agent SpawnTroop(
    //       IAgentOriginBase troopOrigin, bool isPlayerSide, bool hasFormation,
    //       bool spawnWithHorse, bool isReinforcement, int formationTroopCount,
    //       int formationTroopIndex, bool isAlarmed, bool wieldInitialWeapons,
    //       Vec3? initialPosition, Vec2? initialDirection,
    //       string specialActionSetSuffix = null, ItemObject bannerItem = null,
    //       FormationClass formationIndex = FormationClass.NumberOfAllFormations,
    //       bool useTroopClassForSpawn = false)
    //
    // Harmony picks the overload by name match since this is the only public
    // SpawnTroop. We take only the two parameters we need to avoid signature
    // churn across minor game versions.
    [HarmonyPatch(typeof(Mission), nameof(Mission.SpawnTroop))]
    internal static class MissionSpawnTroopPatch
    {
        // Round-robin counter per StringId, reset per battle.
        private static readonly Dictionary<string, int> _rrCounters
            = new Dictionary<string, int>();

        // Diagnostic counters · the first _diagLogLimit calls always log a full
        // trace (regardless of DebugLogging) so we can see exactly what the
        // Postfix is doing on each spawn without flooding the log in big battles.
        private static int _diagLogCount;
        private const int _diagLogLimit = 25;
        private static int _enteredCount, _skippedByIsPlayerSide, _skippedByNullResult,
            _skippedByNoPlan, _skippedByNullTroop, _skippedByHero,
            _skippedByNotInPlan, _skippedByEmptySlots, _skippedByOutOfRange,
            _skippedByNullTeam, _skippedByNullTargetFormation, _reassigned;

        public static void ResetRoundRobin()
        {
            _rrCounters.Clear();
            _diagLogCount = 0;
            // Dump the summary from the previous battle before resetting · lets
            // us see the breakdown of why the Postfix did/didn't do its job.
            if (_enteredCount > 0)
            {
                UTMLog.Info("SpawnTroop summary · entered=" + _enteredCount
                    + " · reassigned=" + _reassigned
                    + " · skip(notPlayerSide=" + _skippedByIsPlayerSide
                    + ", nullResult=" + _skippedByNullResult
                    + ", noPlan=" + _skippedByNoPlan
                    + ", nullTroop=" + _skippedByNullTroop
                    + ", hero=" + _skippedByHero
                    + ", notInPlan=" + _skippedByNotInPlan
                    + ", emptySlots=" + _skippedByEmptySlots
                    + ", outOfRange=" + _skippedByOutOfRange
                    + ", nullTeam=" + _skippedByNullTeam
                    + ", nullTargetFormation=" + _skippedByNullTargetFormation + ")");
            }
            _enteredCount = _skippedByIsPlayerSide = _skippedByNullResult =
                _skippedByNoPlan = _skippedByNullTroop = _skippedByHero =
                _skippedByNotInPlan = _skippedByEmptySlots = _skippedByOutOfRange =
                _skippedByNullTeam = _skippedByNullTargetFormation = _reassigned = 0;
        }

        [HarmonyPostfix]
        private static void Postfix(Agent __result, IAgentOriginBase troopOrigin, bool isPlayerSide)
        {
            try
            {
                _enteredCount++;
                var s = UTMSettings.Instance;
                if (s == null || !s.MasterEnabled || !s.ApplyFormationPlans) return;

                bool diagLog = _diagLogCount < _diagLogLimit;
                var troopId = (troopOrigin?.Troop)?.StringId ?? "<null>";

                if (!isPlayerSide)
                {
                    _skippedByIsPlayerSide++;
                    if (diagLog) UTMLog.Info("SpawnTroop#" + _diagLogCount++ + " · " + troopId + " · skip notPlayerSide");
                    return;
                }
                if (__result == null)
                {
                    _skippedByNullResult++;
                    if (diagLog) UTMLog.Info("SpawnTroop#" + _diagLogCount++ + " · " + troopId + " · skip nullResult");
                    return;
                }

                var plan = PartyPlanRuntime.Current;
                if (plan == null || plan.IsEmpty)
                {
                    _skippedByNoPlan++;
                    if (diagLog) UTMLog.Info("SpawnTroop#" + _diagLogCount++ + " · " + troopId + " · skip noPlan");
                    return;
                }

                var troop = troopOrigin?.Troop as CharacterObject;
                if (troop == null)
                {
                    _skippedByNullTroop++;
                    if (diagLog) UTMLog.Info("SpawnTroop#" + _diagLogCount++ + " · " + troopId + " · skip nullTroop");
                    return;
                }
                if (troop.IsHero)
                {
                    _skippedByHero++;
                    if (diagLog) UTMLog.Info("SpawnTroop#" + _diagLogCount++ + " · " + troopId + " · skip hero");
                    return;
                }

                if (!plan.Formations.TryGetValue(troop.StringId, out var slots))
                {
                    _skippedByNotInPlan++;
                    if (diagLog) UTMLog.Info("SpawnTroop#" + _diagLogCount++ + " · " + troopId + " · skip notInPlan");
                    return;
                }
                if (slots == null || slots.Count == 0)
                {
                    _skippedByEmptySlots++;
                    if (diagLog) UTMLog.Info("SpawnTroop#" + _diagLogCount++ + " · " + troopId + " · skip emptySlots");
                    return;
                }

                int formationIndex = PickSlotRoundRobin(troop.StringId, slots);
                if (formationIndex < 0 || formationIndex > 7)
                {
                    _skippedByOutOfRange++;
                    if (diagLog) UTMLog.Info("SpawnTroop#" + _diagLogCount++ + " · " + troopId + " · skip slot=" + formationIndex + " outOfRange");
                    return;
                }

                var team = __result.Team;
                if (team == null)
                {
                    _skippedByNullTeam++;
                    if (diagLog) UTMLog.Info("SpawnTroop#" + _diagLogCount++ + " · " + troopId + " · skip nullTeam · slot=" + formationIndex);
                    return;
                }

                var targetFormation = team.GetFormation((FormationClass)formationIndex);
                var beforeFormation = __result.Formation?.FormationIndex.ToString() ?? "<null>";
                if (targetFormation == null)
                {
                    _skippedByNullTargetFormation++;
                    if (diagLog) UTMLog.Info("SpawnTroop#" + _diagLogCount++ + " · " + troopId + " · slot=" + formationIndex + " · target=NULL · before=" + beforeFormation);
                    return;
                }

                __result.Formation = targetFormation;
                _reassigned++;
                if (diagLog) UTMLog.Info("SpawnTroop#" + _diagLogCount++ + " · " + troopId + " · reassigned " + beforeFormation + " → " + targetFormation.FormationIndex + " (slot=" + formationIndex + ")");
            }
            catch (Exception ex)
            {
                UTMLog.Exception("MissionSpawnTroopPatch.Postfix", ex);
            }
        }

        private static int PickSlotRoundRobin(string stringId, List<int> slots)
        {
            if (slots.Count == 1) return slots[0];
            _rrCounters.TryGetValue(stringId, out int n);
            int slot = slots[n % slots.Count];
            _rrCounters[stringId] = n + 1;
            return slot;
        }
    }
}
