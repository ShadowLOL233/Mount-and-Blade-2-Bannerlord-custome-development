using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace CustomBattleTroopBuilder.UI
{
    [PrefabExtension("CustomBattleScreen", "descendant::ListPanel[@Id='CenterControls']/Children")]
    public class CBTBMenuButtonExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => (InsertType)3;

        [PrefabExtensionFileName(true)]
        public string GetPrefabExtension => "CBTBMenuButton.xml";
    }
}
