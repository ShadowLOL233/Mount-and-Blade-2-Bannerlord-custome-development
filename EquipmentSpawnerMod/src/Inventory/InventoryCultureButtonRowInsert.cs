using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace EquipmentSpawnerMod.Inventory
{
    // Injects our 6-culture button row into the LEFT ("Other") inventory panel's outer wrapper,
    // at the TOP as first child. Visually: [Owner name + gold header] -> [6 culture buttons] ->
    // [Type/Name/Wt/#/Value column headers + item list].
    //
    // DataSource inheritance: OtherInventoryListWidgetParent has no explicit DataSource, so it
    // inherits from LeftPanel/InventoryScreenWidget which use the movie's root VM = SPInventoryVM.
    // Command.Click on our SortButtonWidgets therefore resolves against SPInventoryVM, where our
    // mixin's [DataSourceMethod] ExecuteEqsm* methods live. Same reason Execute*Culture click
    // was silent when we injected inside the CenterItems block — that widget's DataSource is
    // implicit via the horizontal filter chain but the click resolver picked up its category
    // filter DataSource, not our mixin. Attaching to the LeftPanel container avoids the
    // resolution mismatch entirely.
    // XPath ends in "/Children": Bannerlord XAML wraps every widget's actual children in a
    // <Children> element. Targeting the ListPanel itself and using InsertType.Child would insert
    // as a sibling of <Children>, which is not a valid widget position — nothing renders.
    // Targeting /Children directly + InsertType.Child inserts INTO the widget's actual children.
    // This is the same pattern Retinues uses for its main ClanScreen injection.
    [PrefabExtension(
        "Inventory",
        "descendant::ListPanel[@Id='OtherInventoryListWidgetParent']/Children")]
    public sealed class InventoryCultureButtonRowInsert : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Child;
        public override int Index => 0;   // Prepend as first child inside <Children>

        [PrefabExtensionFileName(false)]
        public string FileName => "InventoryCultureButtonRow";
    }
}
