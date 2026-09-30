using System;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // Phase 2 target · reset our per-side counters when a side finishes deploying.
    // DESIGN §6.1 row 4.
    [HarmonyPatch(typeof(Mission), "OnBattleSideDeployed")]
    internal static class MissionOnBattleSideDeployedPatch
    {
        [HarmonyPostfix]
        private static void Postfix(Mission __instance)
        {
            try
            {
                var s = UTMSettings.Instance;
                if (s == null || !s.MasterEnabled) return;
                // Phase 2 · clean state per side here.
            }
            catch (Exception ex)
            {
                UTMLog.Exception("MissionOnBattleSideDeployedPatch.Postfix", ex);
            }
        }
    }
}
