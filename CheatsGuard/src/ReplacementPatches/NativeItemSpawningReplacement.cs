using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace CheatsGuard.ReplacementPatches
{
    // Drop-in replacement for BannerlordCheats.Patches.Inventory.NativeItemSpawning.
    //
    // Cheats v3.0.3.0 hard-codes a 10-param signature for InventoryLogic.Initialize
    // that no longer exists in 1.4.7 — Harmony throws ArgumentException at patch time
    // (the enum was moved from top-level `InventoryCategoryType` into nested
    // `InventoryScreenHelper.InventoryCategoryType`, plus two new params inserted).
    // Cheats' original Patch is skipped via config.xml SkipList; this replacement
    // targets whichever 12+ param overload of Initialize exists in the current build
    // via TargetMethod() (robust against future signature drift).
    //
    // OS compatibility: reads all items via ObjectManager.GetObjectTypeList<ItemObject>()
    // which returns every registered ItemObject — OSA, OSW, Saddlery, Naval, everything.

    [HarmonyPatch]
    internal static class NativeItemSpawningReplacement
    {
        static MethodBase TargetMethod()
        {
            var candidates = typeof(InventoryLogic)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name == "Initialize")
                .ToArray();

            // Pick the overload whose 2nd parameter is MobileParty (as opposed to
            // the overload that takes ItemRoster+TroopRoster directly). This matches
            // Cheats' original intent (patch the player-facing "open inventory" call).
            foreach (var m in candidates)
            {
                var ps = m.GetParameters();
                if (ps.Length >= 2 && ps[1].ParameterType == typeof(MobileParty))
                {
                    LogFile.Write($"NativeItemSpawningReplacement will patch {m}");
                    return m;
                }
            }
            LogFile.Write("NativeItemSpawningReplacement: no matching InventoryLogic.Initialize overload found — patch inactive");
            return null;
        }

        [HarmonyPostfix]
        static void Postfix(ItemRoster leftItemRoster, MobileParty party, bool isTrading)
        {
            if (!Config.EnableNativeItemSpawning) return;
            if (leftItemRoster == null || party == null) return;

            try
            {
                if (party != MobileParty.MainParty) return;
                if (isTrading) return;
                if (Game.Current == null || Game.Current.CheatMode) return;

                var items = Game.Current.ObjectManager.GetObjectTypeList<ItemObject>();
                if (items == null) return;

                int count = Config.NativeItemSpawnCount;
                int added = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (item == null) continue;
                    leftItemRoster.AddToCounts(item, count);
                    added++;
                }
                LogFile.Write($"NativeItemSpawningReplacement injected {added} item types × {count} each");
            }
            catch (Exception ex)
            {
                LogFile.Write($"NativeItemSpawningReplacement runtime error: {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
