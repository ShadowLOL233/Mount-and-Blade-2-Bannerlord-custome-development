using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using UnifiedTroopManager.Data;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // Per-battle cleanup on encounter finish.
    //
    // Previous B6-A had a snapshot/restore block for MainParty.MemberRoster
    // mutations · removed in R1 along with the roster-filter patches that
    // created the snapshot.  UTM no longer touches MainParty; roster quota
    // enforcement moved to UTMQuotaEnforcerPatch via a Prefix filter on
    // MapEventSide.AllocateTroops.
    //
    // What this Prefix still does:
    //   · Reset per-battle UTMBattleState (SetupActive, PlayerSide)
    //   · Reset MissionSpawnTroopPatch round-robin counters
    //   · Clear RosterSelection + PartyPlanRuntime UNLESS RememberLastRoster
    //     (D2 · battle retry without re-picking roster)
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

                UTMBattleState.ResetAll();
                MissionSpawnTroopPatch.ResetRoundRobin();

                if (s.RememberLastRoster) return; // D2 · keep selection across retreat/retry.
                RosterSelection.Clear();
                PartyPlanRuntime.Clear();
            }
            catch (Exception ex)
            {
                UTMLog.Exception("PlayerEncounterFinishPatch.Prefix", ex);
            }
        }
    }
}
