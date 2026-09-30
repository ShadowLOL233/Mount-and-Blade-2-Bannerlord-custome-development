using System;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // Phase 2 target · adjust _battleSideSpawnContexts + _phases waves so our
    // roster-selected troops go into the first wave. DESIGN §6.1 row 3.
    [HarmonyPatch(typeof(DefaultBattleMissionAgentSpawnLogic), "Init")]
    internal static class SpawnLogicInitPatch
    {
        [HarmonyPostfix]
        private static void Postfix(DefaultBattleMissionAgentSpawnLogic __instance)
        {
            try
            {
                var s = UTMSettings.Instance;
                if (s == null || !s.MasterEnabled || !s.RosterFilterEnabled) return;
                // Phase 2 · rewrite wave counts here.
            }
            catch (Exception ex)
            {
                UTMLog.Exception("SpawnLogicInitPatch.Postfix", ex);
            }
        }
    }
}
