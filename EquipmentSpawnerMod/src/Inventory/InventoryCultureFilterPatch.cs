using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;

namespace EquipmentSpawnerMod.Inventory
{
    // Postfix on the private UpdateFilteredStatusOfItem — the single per-item filter callback
    // vanilla uses. It runs for every item every time a category filter changes, search text
    // changes, or when we explicitly re-invoke it from the mixin. Vanilla sets
    // item.IsFiltered = flag || flag2 based on category + search text; we OR in a culture check.
    // No-op when no culture is selected (CurrentCultureId is empty).
    [HarmonyPatch(typeof(SPInventoryVM), "UpdateFilteredStatusOfItem")]
    public static class UpdateFilteredStatusOfItemPatch
    {
        public static void Postfix(SPItemVM item)
        {
            if (item == null) return;
            string wanted = InventoryCultureFilterState.CurrentCultureId;
            if (string.IsNullOrEmpty(wanted)) return;

            var equipmentItem = item.ItemRosterElement.EquipmentElement.Item;
            if (equipmentItem == null) return;

            var culture = equipmentItem.Culture;
            string cid = culture?.StringId?.ToLowerInvariant() ?? "";
            if (cid != wanted)
            {
                item.IsFiltered = true;
            }
        }
    }
}
