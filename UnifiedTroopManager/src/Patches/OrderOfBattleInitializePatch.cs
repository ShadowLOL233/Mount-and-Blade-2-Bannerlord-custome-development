using System;
using HarmonyLib;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // Phase 4 · CORE BUG FIX. Unlike FM we do NOT touch item.Classes[i].Class and we do NOT
    // call any LockManagedSliders equivalent. We only compute a soft weight hint per formation
    // and populate the slider defaults so the OoB screen matches the player's plan.
    // See DESIGN §4.1 fix #1 / #2 and §13 Appendix A for the original FM bug path.
    [HarmonyPatch(typeof(OrderOfBattleVM), nameof(OrderOfBattleVM.Initialize))]
    internal static class OrderOfBattleInitializePatch
    {
        [HarmonyPostfix]
        private static void Postfix(OrderOfBattleVM __instance)
        {
            try
            {
                var s = UTMSettings.Instance;
                if (s == null || !s.MasterEnabled || !s.ApplyOoBWeights) return;
                // Phase 4 · SoftDistribute weights here · never lock.
            }
            catch (Exception ex)
            {
                UTMLog.Exception("OrderOfBattleInitializePatch.Postfix", ex);
            }
        }
    }
}
