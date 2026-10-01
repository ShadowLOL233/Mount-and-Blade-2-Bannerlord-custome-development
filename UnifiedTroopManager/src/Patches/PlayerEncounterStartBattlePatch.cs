using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using UnifiedTroopManager.Data;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // B6-A v2 · Roster filter via temporary roster modification.
    //
    // v1 attempted the CYT pattern of injecting a customAllocationConditions
    // delegate from the AllocateTroops Prefix. That requires the v1.4.8+
    // signature where customAllocationConditions is `ref`; v1.4.7 passes it by
    // value, and the ABI mismatch crashes natively the moment vanilla invokes
    // the method.
    //
    // v2 strategy — snapshot + modify MobileParty.MainParty.MemberRoster right
    // before vanilla builds the MapEventSide roster, then restore in Finish.
    //
    //   For each non-hero troop in the main party:
    //     target  = RosterSelection.Counts[stringId] (0 if not in selection)
    //     available = Number - WoundedNumber
    //     remove  = max(0, available - target)
    //     if remove > 0:
    //         AddToCounts(character, -remove)   // vanilla removes healthy first
    //         Snapshot[character] = remove       // remember to add back later
    //
    // vanilla's StartBattleInternal then copies the (filtered) roster into
    // MapEventSide. Battle plays out normally — kills come out of the reduced
    // Number, so vanilla's post-battle casualty accounting is still correct for
    // the units that participated. On FinishEncounterInternal we AddToCounts
    // the snapshot entries back so the removed-but-untouched troops reappear.
    //
    // Guards:
    //   · RosterModified latch prevents re-applying on retry / join-battle
    //   · snapshot is cleared on reset
    [HarmonyPatch]
    internal static class PlayerEncounterStartBattlePatch
    {
        // Explicit TargetMethod via AccessTools because the vanilla method is
        // private — the attribute-based `[HarmonyPatch(typeof(...), "name")]`
        // path can silently fail to resolve non-public methods under some
        // Harmony versions, leaving the Prefix unregistered with no error.
        // AccessTools.Method is the robust lookup path CYT and friends use.
        private static System.Reflection.MethodBase TargetMethod()
        {
            var m = AccessTools.Method(typeof(PlayerEncounter), "StartBattleInternal");
            if (m == null)
                UTMLog.Error("TargetMethod · PlayerEncounter.StartBattleInternal NOT FOUND");
            else
                UTMLog.Info("TargetMethod · PlayerEncounter.StartBattleInternal resolved OK");
            return m;
        }

        [HarmonyPrefix]
        private static void Prefix()
        {
            UTMLog.Info("StartBattleInternal.Prefix · entered");
            try
            {
                var s = UTMSettings.Instance;
                if (s == null || !s.MasterEnabled || !s.RosterFilterEnabled)
                {
                    UTMLog.Info("StartBattleInternal.Prefix · skipped · master=" + (s?.MasterEnabled)
                        + " · rosterFilter=" + (s?.RosterFilterEnabled));
                    return;
                }
                if (UTMBattleState.RosterModified)
                {
                    UTMLog.Info("StartBattleInternal.Prefix · skipped · already modified");
                    return;
                }

                var selection = RosterSelection.Current;
                if (selection == null || selection.IsEmpty)
                {
                    UTMLog.Info("StartBattleInternal.Prefix · skipped · no selection");
                    return;
                }

                var mainParty = MobileParty.MainParty;
                if (mainParty == null || mainParty.MemberRoster == null) return;

                var pending = new List<(CharacterObject ch, int remove)>();
                foreach (var el in mainParty.MemberRoster.GetTroopRoster())
                {
                    if (el.Character == null) continue;
                    if (el.Character.IsHero) continue;
                    int available = el.Number - el.WoundedNumber;
                    if (available <= 0) continue;

                    int target = selection.Counts.TryGetValue(el.Character.StringId, out var v) ? v : 0;
                    int remove = available - target;
                    if (remove <= 0) continue;
                    pending.Add((el.Character, remove));
                }

                foreach (var p in pending)
                {
                    mainParty.MemberRoster.AddToCounts(p.ch, -p.remove);
                    UTMBattleState.RosterSnapshot[p.ch] = p.remove;
                }
                UTMBattleState.RosterModified = pending.Count > 0;

                UTMLog.Info("StartBattleInternal · UTM filter applied · removed " + pending.Count
                    + " entries · total " + SumRemoved(pending) + " troops · selection=" + selection.Counts.Count + " types");
            }
            catch (Exception ex)
            {
                UTMLog.Exception("PlayerEncounterStartBattlePatch.Prefix", ex);
            }
        }

        private static int SumRemoved(List<(CharacterObject, int)> list)
        {
            int sum = 0;
            foreach (var p in list) sum += p.Item2;
            return sum;
        }
    }
}
