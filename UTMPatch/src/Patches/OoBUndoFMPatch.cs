using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;
using UTMPatch.Util;

namespace UTMPatch.Patches
{
    // P1 · Reverse-undo of FM's OrderOfBattleVMInitializePatch.Postfix.
    //
    // FM (Stop Shuffling You Fools) writes two things on OoB init that squeeze
    // out any troop class without an active FM plan — including the troops CYT
    // just picked for the battle:
    //
    //   1. For each formation slot, Classes[0].Class is set to the FM-planned
    //      native FormationClass (e.g. Infantry).  Classes[1].Class is set to
    //      NumberOfAllFormations.
    //   2. OobWeightDistributor.LockManagedSliders() locks the slot's sliders
    //      so the player can't adjust the distribution.
    //
    // FM's actual Formation assignment at spawn time is independent of these
    // writes — it happens in MissionAgentSpawnPatch.Postfix using
    // FormationAssignmentResolver.  So we can safely undo the OoB-layer writes
    // without breaking FM's core feature.
    //
    // Strategy — Postfix on vanilla OrderOfBattleVM.Initialize at priority
    // Last, declared HarmonyAfter("FormationManager"), so we run AFTER FM's
    // Postfix.  For every Classes entry we reset .Class to NumberOfAllFormations
    // (the vanilla "accepts any class" marker) and unlock the sliders.
    //
    // Guards — if FormationManager is not loaded, UTMPatchSubModule skips
    // Harmony.PatchAll entirely, so this type is dead code in that case.
    [HarmonyPatch]
    internal static class OoBUndoFMPatch
    {
        private static MethodBase TargetMethod()
        {
            var m = AccessTools.Method(typeof(OrderOfBattleVM), nameof(OrderOfBattleVM.Initialize));
            if (m == null)
                UTMPatchLog.Error("TargetMethod · OrderOfBattleVM.Initialize NOT FOUND");
            return m;
        }

        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        [HarmonyAfter("com.formationmanager")]
        private static void Postfix(OrderOfBattleVM __instance)
        {
            try
            {
                if (__instance == null) return;

                var items = (__instance.FormationsFirstHalf
                                ?? Enumerable.Empty<OrderOfBattleFormationItemVM>())
                    .Concat(__instance.FormationsSecondHalf
                                ?? Enumerable.Empty<OrderOfBattleFormationItemVM>())
                    .Where(f => f != null)
                    .ToList();

                int slotsTouched = 0;
                int classesReset = 0;
                int slidersUnlocked = 0;

                foreach (var item in items)
                {
                    try
                    {
                        if (item.Classes == null) continue;
                        bool touched = false;
                        foreach (var c in item.Classes)
                        {
                            if (c == null) continue;
                            if (c.Class != FormationClass.NumberOfAllFormations)
                            {
                                c.Class = FormationClass.NumberOfAllFormations;
                                classesReset++;
                                touched = true;
                            }
                            if (c.IsLocked)
                            {
                                c.SetWeightAdjustmentLock(isLocked: false);
                                c.IsLocked = false;
                                slidersUnlocked++;
                                touched = true;
                            }
                        }
                        if (touched) slotsTouched++;
                    }
                    catch (Exception perSlot)
                    {
                        UTMPatchLog.Exception("OoBUndoFM · per-slot", perSlot);
                    }
                }

                UTMPatchLog.Info("OoBUndoFM · slotsTouched=" + slotsTouched
                    + " · classesReset=" + classesReset
                    + " · slidersUnlocked=" + slidersUnlocked
                    + " · itemsSeen=" + items.Count);
            }
            catch (Exception ex)
            {
                UTMPatchLog.Exception("OoBUndoFMPatch.Postfix", ex);
            }
        }
    }
}
