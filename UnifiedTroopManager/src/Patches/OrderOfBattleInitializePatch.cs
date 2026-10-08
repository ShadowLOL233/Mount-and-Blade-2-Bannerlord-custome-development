using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;
using UnifiedTroopManager.Data;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // B6-C Step 1 · OoB card class override via RefreshFormation.
    //
    // The previous attempts to steer vanilla by setting agent.Formation at
    // SpawnTroop/OnBattleSideDeployed time ran into a terminal problem:
    // vanilla has an additional class-based cleanup after both of our hooks
    // that pushes every agent back to the Formation matching its
    // CharacterObject.DefaultFormationClass. The agent writes "worked" (log
    // confirmed reassigned=690) but got overwritten before battle camera.
    //
    // The real fix is upstream: make vanilla's own class pool for each OoB
    // card match the UTM plan. If the Infantry pool and the Ranged pool for
    // Formation #0 are both "allowed" because UTM routes Legionary+Crossbowman
    // there, vanilla won't push them out — because its cleanup is driven by
    // the card's declared classes.
    //
    // OrderOfBattleFormationItemVM.RefreshFormation(formation, overriddenClass,
    // mustExist) does exactly this: it declares which DeploymentFormationClass
    // the card accepts. CYT doesn't touch this; FM uses it + LockManagedSliders
    // (that lock is the bug source). We use just RefreshFormation — no lock,
    // no forced slider weights, player keeps full OoB control.
    //
    // DeploymentFormationClass values (from FM decompile):
    //   Unset = 0, Infantry = 1, Ranged = 2, Cavalry = 3, HorseArcher = 4,
    //   InfantryAndRanged = 5, CavalryAndHorseArcher = 6
    //
    // Per-slot logic:
    //   1. Walk every PartyPlan entry; collect each troop's effective
    //      DeploymentFormationClass (merging HeavyInfantry→Infantry,
    //      LightCavalry/HeavyCavalry→Cavalry, Skirmisher→Ranged).
    //   2. If the slot has both Infantry and Ranged → InfantryAndRanged.
    //      If it has Cavalry and HorseArcher → CavalryAndHorseArcher.
    //      Else single class.
    //   3. RefreshFormation(formation, computed, mustExist: true).
    //
    // Falls back to leaving a slot untouched if no troops are planned for it,
    // so empty UTM slots don't blow away vanilla's auto-deploy for them.
    [HarmonyPatch(typeof(OrderOfBattleVM), nameof(OrderOfBattleVM.Initialize))]
    internal static class OrderOfBattleInitializePatch
    {
        [HarmonyPostfix]
        private static void Postfix(OrderOfBattleVM __instance)
        {
            // Re-enabled 2026-10-07 (second time) after confirming the earlier
            // crash was fixed by UTMInitialSpawnPatch (InitialSpawnNumber
            // alignment), not by anything this patch did. User asked for OoB
            // slot icons to match the UTM plan (slot assigned Infantry troops
            // should show the Infantry sword icon, etc.) — that's exactly what
            // this patch does via RefreshFormation.
            try
            {
                var s = UTMSettings.Instance;
                if (s == null || !s.MasterEnabled || !s.ApplyOoBWeights) return;
                if (UTMBattleState.IsHideoutBattle()) return;

                var plan = PartyPlanRuntime.Current;
                if (plan == null || plan.IsEmpty)
                {
                    UTMLog.Info("OoB.Initialize.Postfix · no PartyPlan · skip");
                    return;
                }

                if (__instance == null) return;
                var items = (__instance.FormationsFirstHalf == null
                        ? Enumerable.Empty<OrderOfBattleFormationItemVM>()
                        : __instance.FormationsFirstHalf)
                    .Concat(__instance.FormationsSecondHalf == null
                        ? Enumerable.Empty<OrderOfBattleFormationItemVM>()
                        : __instance.FormationsSecondHalf)
                    .Where(f => f != null && f.Formation != null)
                    .ToList();

                int applied = 0, skipped = 0;
                foreach (var item in items)
                {
                    int slot = (int)item.Formation.FormationIndex;
                    var dc = ComputePlannedClass(plan, slot);
                    if (dc == DeploymentFormationClass.Unset)
                    {
                        skipped++;
                        continue;
                    }
                    try
                    {
                        item.RefreshFormation(item.Formation, dc, mustExist: true);
                        applied++;
                        UTMLog.Info("OoB · Formation " + slot + " · DeploymentFormationClass=" + dc);
                    }
                    catch (Exception innerEx)
                    {
                        UTMLog.Exception("RefreshFormation(" + slot + ", " + dc + ")", innerEx);
                    }
                }
                UTMLog.Info("OoB.Initialize.Postfix · applied RefreshFormation to " + applied
                    + " formations · skipped " + skipped + " (no plan)");
            }
            catch (Exception ex)
            {
                UTMLog.Exception("OrderOfBattleInitializePatch.Postfix", ex);
            }
        }

        // Walk the plan and find every troop routed to this slot. Collapse
        // their effective deployment class into one DeploymentFormationClass.
        private static DeploymentFormationClass ComputePlannedClass(BattlePlan plan, int slotIndex)
        {
            var dcSet = new HashSet<DeploymentFormationClass>();
            foreach (var kv in plan.Formations)
            {
                if (kv.Value == null || !kv.Value.Contains(slotIndex)) continue;
                var ch = Game.Current.ObjectManager.GetObject<CharacterObject>(kv.Key);
                if (ch == null) continue;
                var dc = ClassToDeployment(ch.DefaultFormationClass);
                if (dc != DeploymentFormationClass.Unset) dcSet.Add(dc);
            }
            if (dcSet.Count == 0) return DeploymentFormationClass.Unset;

            bool hasInf = dcSet.Contains(DeploymentFormationClass.Infantry);
            bool hasRng = dcSet.Contains(DeploymentFormationClass.Ranged);
            if (hasInf && hasRng) return DeploymentFormationClass.InfantryAndRanged;
            bool hasCav = dcSet.Contains(DeploymentFormationClass.Cavalry);
            bool hasHA = dcSet.Contains(DeploymentFormationClass.HorseArcher);
            if (hasCav && hasHA) return DeploymentFormationClass.CavalryAndHorseArcher;
            // Single-category plan · pick deterministically.
            return dcSet.First();
        }

        private static DeploymentFormationClass ClassToDeployment(FormationClass fc)
        {
            switch (fc)
            {
                case FormationClass.Infantry:
                case FormationClass.HeavyInfantry:
                    return DeploymentFormationClass.Infantry;
                case FormationClass.Ranged:
                case FormationClass.Skirmisher:
                    return DeploymentFormationClass.Ranged;
                case FormationClass.Cavalry:
                case FormationClass.LightCavalry:
                case FormationClass.HeavyCavalry:
                    return DeploymentFormationClass.Cavalry;
                case FormationClass.HorseArcher:
                    return DeploymentFormationClass.HorseArcher;
                default:
                    return DeploymentFormationClass.Unset;
            }
        }
    }
}
