using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace CustomBattleTroopBuilder.UI
{
    // v0.3.8: XPath 从 '/..' (UIExt v3 不支持 parent axis · v0.3.7 实测 "node not found") →
    // 用 attribute predicate 直接定位 CenterControls 父 Widget (VisualDefinition='CenterMenu')
    // button 作为 CenterMenu Widget 的 Children · 跟 CenterControls 平级 · 用 MarginBottom=110
    // 浮在 Start 按钮正上方 · 不碰 vanilla CenterControls 内 3 button 排列
    //
    // CenterMenu Widget 定位: Modules\SandBoxCore\GUI\Prefabs\CustomBattle\CustomBattleScreen.xml:68
    // <Widget VisualDefinition="CenterMenu" ... PositionYOffset="-505" ... >
    [PrefabExtension("CustomBattleScreen", "descendant::Widget[@VisualDefinition='CenterMenu']/Children")]
    public class CBTBMenuButtonExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => (InsertType)3;

        [PrefabExtensionFileName(true)]
        public string GetPrefabExtension => "CBTBMenuButton.xml";
    }
}
