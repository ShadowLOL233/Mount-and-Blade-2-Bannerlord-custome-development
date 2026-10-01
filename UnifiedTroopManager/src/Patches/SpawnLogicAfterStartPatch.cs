using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using UnifiedTroopManager.Data;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // B6-A · DefaultBattleMissionAgentSpawnLogic.AfterStart carries BOTH hooks:
    //
    //   Prefix  — snapshot MainParty roster + remove unselected troops (same
    //             mechanics as PlayerEncounterStartBattlePatch) · placed here
    //             because testing showed PlayerEncounter.StartBattleInternal is
    //             NOT called on the real battle entry path · AfterStart is the
    //             next earliest confirmed hook (SpawnLogic log fires before
    //             MapEventSide.AllocateTroops). If MapEventSide has already
    //             copied the roster by this point the filter still catches it
    //             because AllocateTroops reads descriptors back through
    //             MapEventParty.Troops which traces to MainParty.MemberRoster.
    //
    //   Postfix — flip UTMBattleState.SetupActive and cache PlayerSide for the
    //             AllocateTroops Prefix log (diagnostic only on v1.4.7 since
    //             the filter can't be injected via ref).
    [HarmonyPatch(typeof(DefaultBattleMissionAgentSpawnLogic), "AfterStart")]
    internal static class SpawnLogicAfterStartPatch
    {
        [HarmonyPrefix]
        private static void Prefix(DefaultBattleMissionAgentSpawnLogic __instance)
        {
            UTMLog.Info("SpawnLogic.AfterStart.Prefix · entered");
            try
            {
                var s = UTMSettings.Instance;
                if (s == null || !s.MasterEnabled || !s.RosterFilterEnabled)
                {
                    UTMLog.Info("SpawnLogic.AfterStart.Prefix · skipped · master=" + (s?.MasterEnabled)
                        + " · rosterFilter=" + (s?.RosterFilterEnabled));
                    return;
                }
                if (UTMBattleState.RosterModified)
                {
                    UTMLog.Info("SpawnLogic.AfterStart.Prefix · skipped · already modified");
                    return;
                }

                var selection = RosterSelection.Current;
                if (selection == null || selection.IsEmpty)
                {
                    UTMLog.Info("SpawnLogic.AfterStart.Prefix · skipped · no selection");
                    return;
                }

                var mainParty = MobileParty.MainParty;
                if (mainParty == null || mainParty.MemberRoster == null)
                {
                    UTMLog.Warn("SpawnLogic.AfterStart.Prefix · MainParty.MemberRoster null");
                    return;
                }

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

                UTMLog.Info("SpawnLogic.AfterStart.Prefix · roster filter applied · removed "
                    + pending.Count + " entries · total " + SumRemoved(pending)
                    + " troops · selection=" + selection.Counts.Count + " types");
            }
            catch (Exception ex)
            {
                UTMLog.Exception("SpawnLogicAfterStartPatch.Prefix", ex);
            }
        }

        [HarmonyPostfix]
        private static void Postfix(DefaultBattleMissionAgentSpawnLogic __instance)
        {
            try
            {
                var s = UTMSettings.Instance;
                if (s == null || !s.MasterEnabled || !s.RosterFilterEnabled) return;

                var selection = RosterSelection.Current;
                if (selection == null || selection.IsEmpty)
                {
                    UTMBattleState.ResetAll();
                    return;
                }

                var mapEvent = TaleWorlds.CampaignSystem.MapEvents.MapEvent.PlayerMapEvent;
                UTMBattleState.PlayerSide = mapEvent != null ? mapEvent.PlayerSide : BattleSideEnum.None;
                UTMBattleState.SetupActive = true;

                UTMLog.Info("SpawnLogic.AfterStart.Postfix · UTM setup active · playerSide=" + UTMBattleState.PlayerSide
                    + " · selection=" + selection.Counts.Count + " types"
                    + " · modified=" + UTMBattleState.RosterModified);
            }
            catch (Exception ex)
            {
                UTMLog.Exception("SpawnLogicAfterStartPatch.Postfix", ex);
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
