using System;
using HarmonyLib;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // Phase 4/7 target · Lord's Hall fight is a distinct mission controller that
    // creates its own party context. Reset roster state so our filters kick in fresh.
    // DESIGN §6.1 row 5 + §2.2 E4.
    [HarmonyPatch(typeof(LordsHallFightMissionController), "OnCreated")]
    internal static class LordsHallFightOnCreatedPatch
    {
        [HarmonyPrefix]
        private static void Prefix()
        {
            try
            {
                var s = UTMSettings.Instance;
                if (s == null || !s.MasterEnabled) return;
                // Phase 4/7 · Lord's Hall specific reset here.
            }
            catch (Exception ex)
            {
                UTMLog.Exception("LordsHallFightOnCreatedPatch.Prefix", ex);
            }
        }
    }
}
