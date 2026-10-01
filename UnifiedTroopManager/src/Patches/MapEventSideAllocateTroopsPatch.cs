using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem.MapEvents;
using UnifiedTroopManager.Data;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // B6-A step 1 · DISABLED FILTER · log only.
    //
    // Vanilla v1.4.7's signature is:
    //   public void AllocateTroops(
    //       ref List<UniqueTroopDescriptor> troopsList,
    //       int numberToAllocate,
    //       Func<UniqueTroopDescriptor, MapEventParty, bool> customAllocationConditions = null)
    //
    // CYT's clean filter uses `ref Func<...> customAllocationConditions` to inject
    // a filter from the Prefix. That `ref` was added in v1.4.8+ — in v1.4.7 the
    // parameter is pass-by-value, so a Prefix cannot propagate the new delegate
    // back to vanilla. Writing the Prefix with `ref` on a non-ref vanilla param
    // produces a native-side calling-convention mismatch and crashes the game
    // the moment vanilla invokes AllocateTroops (no managed exception).
    //
    // Until the roster-filter is reimplemented via a v1.4.7-safe path (likely
    // temporary modification of MobileParty.MainParty.MemberRoster around the
    // battle, with restore on PlayerEncounter.Finish), this Prefix only logs so
    // we can confirm the hook timing without affecting gameplay.
    [HarmonyPatch(typeof(MapEventSide), nameof(MapEventSide.AllocateTroops))]
    internal static class MapEventSideAllocateTroopsPatch
    {
        [HarmonyPrefix]
        private static void Prefix(MapEventSide __instance, int numberToAllocate)
        {
            try
            {
                var s = UTMSettings.Instance;
                if (s == null || !s.MasterEnabled || !s.RosterFilterEnabled) return;
                if (!UTMBattleState.SetupActive) return;
                if (__instance == null || __instance.MapEvent == null) return;
                if (__instance.MapEvent.IsPlayerSimulation) return;
                if (__instance.MissionSide != UTMBattleState.PlayerSide) return;

                var selection = RosterSelection.Current;
                if (selection == null || selection.IsEmpty) return;

                UTMLog.Info("AllocateTroops HIT · player side · numberToAllocate=" + numberToAllocate
                    + " · selection=" + selection.Counts.Count + " types (filter not applied in v1.4.7)");
            }
            catch (Exception ex)
            {
                UTMLog.Exception("MapEventSideAllocateTroopsPatch.Prefix", ex);
            }
        }
    }
}
