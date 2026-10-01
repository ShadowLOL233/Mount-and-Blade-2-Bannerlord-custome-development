using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using UnifiedTroopManager.Data;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // B6-A v2 · restore the roster snapshot that PlayerEncounterStartBattlePatch
    // applied before the battle, then reset per-battle state.
    //
    // RosterSelection.Clear is still gated on RememberLastRoster so the player
    // can retry a battle without re-picking the roster. The UTMBattleState
    // reset (incl. snapshot) always runs because snapshot is scoped to the
    // vanilla battle lifecycle · the next battle will re-apply from
    // RosterSelection.Current if it's still set.
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

                // Restore the snapshot BEFORE resetting state so a mid-flow
                // exception doesn't leave the roster permanently reduced.
                if (UTMBattleState.RosterModified && UTMBattleState.RosterSnapshot.Count > 0)
                {
                    var mainParty = MobileParty.MainParty;
                    if (mainParty != null && mainParty.MemberRoster != null)
                    {
                        int restoredTypes = 0, restoredCount = 0;
                        foreach (var kv in UTMBattleState.RosterSnapshot)
                        {
                            try
                            {
                                mainParty.MemberRoster.AddToCounts(kv.Key, kv.Value);
                                restoredTypes++;
                                restoredCount += kv.Value;
                            }
                            catch (Exception innerEx)
                            {
                                UTMLog.Exception("Restore AddToCounts(" + kv.Key?.StringId + ")", innerEx);
                            }
                        }
                        UTMLog.Info("FinishEncounter · UTM restored · " + restoredTypes + " types · " + restoredCount + " troops");
                    }
                    else
                    {
                        UTMLog.Warn("FinishEncounter · snapshot present but MainParty unavailable · roster entries lost");
                    }
                }

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
