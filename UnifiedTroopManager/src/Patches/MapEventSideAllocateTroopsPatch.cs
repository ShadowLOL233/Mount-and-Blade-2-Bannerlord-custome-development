using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem.MapEvents;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // Phase 2 target · inject customAllocationConditions so only RosterSelection ids spawn.
    // Corresponds to DESIGN §6.1 row 1 (CYT-layer roster filter).
    [HarmonyPatch(typeof(MapEventSide), nameof(MapEventSide.AllocateTroops))]
    internal static class MapEventSideAllocateTroopsPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(MapEventSide __instance)
        {
            try
            {
                var s = UTMSettings.Instance;
                if (s == null || !s.MasterEnabled || !s.RosterFilterEnabled) return true;
                // Phase 2 · plug customAllocationConditions here.
                return true;
            }
            catch (Exception ex)
            {
                UTMLog.Exception("MapEventSideAllocateTroopsPatch.Prefix", ex);
                return true; // O-8 fallback: never block vanilla path.
            }
        }
    }
}
