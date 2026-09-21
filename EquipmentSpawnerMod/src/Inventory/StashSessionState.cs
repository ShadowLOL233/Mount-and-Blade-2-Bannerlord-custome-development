using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem.Roster;

namespace EquipmentSpawnerMod.Inventory
{
    // Tracks the ItemRoster passed to the most recent InventoryScreenHelper.OpenScreenAsStash call.
    // Captured via Harmony Prefix so the inject button can target whichever stash the player just
    // opened — our personal stash OR vanilla Settlement.Stash (both go through the same API).
    // No cleanup on close: overwritten on next open. If no stash was ever opened this session,
    // CurrentStash is null and the inject button becomes a no-op with a helpful message.
    public static class StashSessionState
    {
        public static ItemRoster CurrentStash;
    }

    [HarmonyPatch(typeof(InventoryScreenHelper), nameof(InventoryScreenHelper.OpenScreenAsStash))]
    public static class CaptureOpenScreenAsStashPatch
    {
        public static void Prefix(ItemRoster stash)
        {
            StashSessionState.CurrentStash = stash;
        }
    }
}
