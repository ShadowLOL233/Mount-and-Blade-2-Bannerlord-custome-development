using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Encounters;
using UnifiedTroopManager.Data;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // Phase 2 target · reset RosterSelection.Current when the encounter ends so the
    // next encounter starts with a fresh selection. DESIGN §6.1 row 6 + §3.4 lifecycle.
    [HarmonyPatch(typeof(PlayerEncounter), "FinishEncounterInternal")]
    internal static class PlayerEncounterFinishPatch
    {
        [HarmonyPrefix]
        private static void Prefix()
        {
            try
            {
                var s = UTMSettings.Instance;
                if (s == null || !s.MasterEnabled) return;
                if (s.RememberLastRoster) return; // D2 · keep selection across retreat/retry.
                RosterSelection.Clear();
            }
            catch (Exception ex)
            {
                UTMLog.Exception("PlayerEncounterFinishPatch.Prefix", ex);
            }
        }
    }
}
