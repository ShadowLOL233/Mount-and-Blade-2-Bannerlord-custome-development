using System;
using System.IO;
using Bannerlord.UIExtenderEx;
using CustomBattleTroopBuilder.UI;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;

namespace CustomBattleTroopBuilder
{
    public sealed class CBTBSubModule : MBSubModuleBase
    {
        public const string ModuleId = "CustomBattleTroopBuilder";
        public const string HarmonyId = "com.pslr.cbtb";
        public const string LogFile = "CustomBattleTroopBuilder.log";

        private static Harmony? _harmony;
        private static UIExtender? _extender;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                _harmony = new Harmony(HarmonyId);
                // v0.4.3: Patch Apply 延迟到 OnBeforeInitialModuleScreenSetAsRoot (CustomBattle.dll ready 后)
                // · 遵守 [[bannerlord-cross-dll-typeof]] 铁律 · 跨 DLL class 必须 lazy load
                _extender = UIExtender.Create(ModuleId);
                _extender.Register(typeof(CBTBSubModule).Assembly);
                _extender.Enable();
                Log($"[CBTB] OnSubModuleLoad OK · v0.5.2 · Harmony id={HarmonyId} · UIExtender enabled · ROLLBACK v0.5.1 Prefab (5th bootstrap crash)");
            }
            catch (Exception e)
            {
                Log($"[CBTB] OnSubModuleLoad FAILED: {e}");
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            try
            {
                // v0.5.0 架构转向：Phase 3b-2b 彻底弃
                // ArmyCompositionGroupVMPatch.Apply + CBTBClonesRegistry.Restore 调用全删
                // 真因：试图让 clone 出现在 vanilla Custom Battle 菜单的 culture slot picker
                // 跟 Retinues 模式其实不匹配 (Retinues 也不改 culture slot picker)
                // 4 次 bootstrap crash + 多轮 timing/filter 迭代证明是技术死胡同
                //
                // Patch class 文件 (ArmyCompositionGroupVMPatch.cs) 保留 code 作调研参考
                // CBTBClonesRegistry 保留 (Phase 6 JSON save 要用 blueprint 持久化)
                // CBTBCloneHelper 保留 (Clone 流程仍需要)
                //
                // 新方向：editor 内独立实现 Retinues 模式
                //   Phase 3c: editor 加 "Test Battle" button · 自己构造 Mission 绕过 vanilla
                //   Phase 4: Equipment 10-slot picker (ItemObject.All 配装)
                //   Phase 5: Skill editor (20+ skills slider)
                //   Phase 6: JSON save/load preset (跨 Bannerlord process 持久)

                Log("[CBTB] OnBeforeInitialModuleScreenSetAsRoot · v0.5.0 Phase 3b-2b 弃 · Track A (Editor functions) + Track B (Test Battle launcher) ready to start");
                InformationManager.DisplayMessage(new InformationMessage(
                    "[CBTB] v0.5.2 loaded. 5th bootstrap crash emergency rollback · Prefab reverted to v0.5.0 Preview layout · VM Tab code dormant · next session incremental Prefab experiments.", Colors.Yellow));
            }
            catch (Exception e)
            {
                Log($"[CBTB] OnBeforeInitialModuleScreenSetAsRoot FAILED: {e}");
            }
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);
            try
            {
                if (Input.IsKeyDown(InputKey.LeftControl) && Input.IsKeyDown(InputKey.LeftShift) && Input.IsKeyPressed(InputKey.B))
                {
                    OpenEditor();
                }
            }
            catch (Exception e)
            {
                Log($"[CBTB] hotkey handler failed: {e}");
            }
        }

        /// <summary>
        /// Shared entry point for both the Ctrl+Shift+B hotkey and the Custom Battle menu button.
        /// Pushes <see cref="CBTBEditorScreen"/>; the screen itself warns when the troop pool is empty.
        /// </summary>
        public static void OpenEditor()
        {
            Log("[CBTB] OpenEditor invoked · pushing CBTBEditorScreen");
            try
            {
                int poolCount = 0;
                try
                {
                    var all = MBObjectManager.Instance?.GetObjectTypeList<BasicCharacterObject>();
                    if (all != null) poolCount = all.Count;
                }
                catch { }

                if (poolCount == 0)
                {
                    Log("[CBTB] OpenEditor: troop pool empty · showing inquiry · aborting push");
                    InformationManager.ShowInquiry(new InquiryData(
                        titleText: "Custom Battle Troop Builder",
                        text: "Troop pool is empty.\n\nMBObjectManager hasn't loaded any BasicCharacterObject yet. Open the editor from the Custom Battle menu instead — the hotkey only works once Custom Battle has initialised the troop pool.",
                        isAffirmativeOptionShown: true,
                        isNegativeOptionShown: false,
                        affirmativeText: "OK",
                        negativeText: null,
                        affirmativeAction: () => { },
                        negativeAction: null));
                    return;
                }

                ScreenManager.PushScreen(new CBTBEditorScreen());
            }
            catch (Exception e)
            {
                Log($"[CBTB] OpenEditor failed: {e}");
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[CBTB] Editor open failed: {e.Message}", Colors.Red));
            }
        }

        internal static void Log(string msg)
        {
            try
            {
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "Mount and Blade II Bannerlord", "logs");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var path = Path.Combine(dir, LogFile);
                File.AppendAllText(path, $"[{DateTime.Now:HH:mm:ss.fff}] {msg}{Environment.NewLine}");
            }
            catch { }
        }
    }
}
