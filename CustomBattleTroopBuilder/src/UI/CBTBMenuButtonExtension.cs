using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace CustomBattleTroopBuilder.UI
{
    // v0.3.7: button 从 CenterControls 内 (inline 跟 Randomize/Start/Back 并排)
    // 挪到 CenterControls 的父 Widget Children (兄弟位置) · 用 MarginBottom 浮在
    // Start 按钮正上方 · 不再挤歪 vanilla CenterControls layout
    [PrefabExtension("CustomBattleScreen", "descendant::ListPanel[@Id='CenterControls']/../Children")]
    public class CBTBMenuButtonExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => (InsertType)3;

        [PrefabExtensionFileName(true)]
        public string GetPrefabExtension => "CBTBMenuButton.xml";
    }
}
