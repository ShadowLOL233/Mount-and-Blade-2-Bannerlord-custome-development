using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace RetinuesCultureFilter.PrefabExtensions
{
    // Injects CultureButtonRow.xml into the ClanScreen prefab, as sibling AFTER the
    // ListPanel that holds the equipment-list Filter Row (search text box).
    //
    // Movie name is "ClanScreen" — the vanilla screen Retinues extends. UIExtenderEx applies
    // patches per-movie in mod-load order, so this patch runs AFTER Retinues has inserted its
    // ClanScreen_TroopsPanel_BL14 subtree (our SubModule.xml declares LoadBeforeThis on Retinues),
    // meaning the XPath below can navigate into Retinues' injected content.
    //
    // XPath disambiguation: Retinues has TWO ListPanel Id="SortButtons" DataSource="{EquipmentList}"
    // — a Sort Row (line 2984, four SortButtonWidgets) and a Filter Row (line 3027, with the
    // FilterText search box). Also a Troop-list Filter Row (line 2402) with DataSource="{TroopList}".
    // The predicate below matches only the Equipment Filter Row.
    [PrefabExtension(
        "ClanScreen",
        "descendant::ListPanel[@Id='SortButtons' and @DataSource='{EquipmentList}' and .//EditableTextWidget[@Text='@FilterText']]")]
    public sealed class CultureButtonRowInsert : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Append;

        [PrefabExtensionFileName(false)]
        public string FileName => "CultureButtonRow";
    }
}
