using System;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // Phase 2 target · mark _setupSide so subsequent spawn logic sees our roster.
    // Corresponds to DESIGN §6.1 row 2.
    [HarmonyPatch(typeof(DefaultBattleMissionAgentSpawnLogic), "AfterStart")]
    internal static class SpawnLogicAfterStartPatch
    {
        [HarmonyPostfix]
        private static void Postfix(DefaultBattleMissionAgentSpawnLogic __instance)
        {
            try
            {
                var s = UTMSettings.Instance;
                if (s == null || !s.MasterEnabled || !s.RosterFilterEnabled) return;
                // Phase 2 · flip our _setupSide-analog internal state here.
            }
            catch (Exception ex)
            {
                UTMLog.Exception("SpawnLogicAfterStartPatch.Postfix", ex);
            }
        }
    }
}
