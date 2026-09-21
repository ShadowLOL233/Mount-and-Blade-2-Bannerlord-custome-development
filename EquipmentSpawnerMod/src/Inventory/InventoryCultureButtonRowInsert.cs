using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace EquipmentSpawnerMod.Inventory
{
    // Injects our 6-culture button row into vanilla's Inventory prefab (Modules/SandBox/GUI/
    // Prefabs/Inventory/Inventory.xml). Movie name = "Inventory". XPath targets the
    // NavigatableListPanel Id="CenterItems" — the horizontal strip of 6 category filter
    // buttons (All / Weapons / ShieldsAndRanged / Armors / Mounts / Misc) at the top center
    // of the screen. We Append as sibling so our row sits directly below.
    //
    // The category row and our culture row combine as a two-dimensional filter (AND semantics
    // enforced by our Harmony postfix on UpdateFilteredStatusOfItem OR-ing the culture check
    // into IsFiltered).
    [PrefabExtension(
        "Inventory",
        "descendant::NavigatableListPanel[@Id='CenterItems']")]
    public sealed class InventoryCultureButtonRowInsert : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Append;

        [PrefabExtensionFileName(false)]
        public string FileName => "InventoryCultureButtonRow";
    }
}
