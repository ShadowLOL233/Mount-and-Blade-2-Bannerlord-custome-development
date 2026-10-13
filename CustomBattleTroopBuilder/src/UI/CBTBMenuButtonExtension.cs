using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace CustomBattleTroopBuilder.UI
{
    // v0.3.9: 紧急回滚到 v0.3.6 的 inline XPath
    //
    // 失败路径记录（避免未来重复踩坑）:
    //   v0.3.7: XPath '/../Children' (parent axis) → soft-fail · 红字 "node not found"
    //   v0.3.8: XPath 'descendant::Widget[@VisualDefinition=...]/Children' → HARD CRASH
    //           bootstrap 阶段挂 · CBTB OnSubModuleLoad 没触发 · rgl log 卡 shader warn
    //           疑因 UIExt v3 对 Bannerlord 自定义 VisualDefinition attribute 不支持
    //
    // 当前状态：button 在 CenterControls 内 inline · 跟 Randomize/Start/Back 并排
    //           副作用：CenterControls CoverChildren 展开后 vanilla 3 button 从居中偏左挤
    //
    // 下 session TODO：用 Harmony patch CustomBattleScreen.OnInitialize 代码层加
    //                   button · 完全放弃 PrefabExtension · 作为 CenterControls 兄弟
    //                   浮在 Start 上方
    [PrefabExtension("CustomBattleScreen", "descendant::ListPanel[@Id='CenterControls']/Children")]
    public class CBTBMenuButtonExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => (InsertType)3;

        [PrefabExtensionFileName(true)]
        public string GetPrefabExtension => "CBTBMenuButton.xml";
    }
}
