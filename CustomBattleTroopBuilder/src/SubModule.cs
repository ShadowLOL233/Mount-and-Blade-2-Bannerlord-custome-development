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
                // v0.4.2 ROLLBACK: v0.4.1 的 ArmyCompositionGroupVMPatch.Apply 导致 bootstrap crash
                // (CBTB log 没 OnSubModuleLoad 条目 · 说明 assembly load 阶段挂 · 可能 typeof(ArmyCompositionGroupVM)
                // JIT 时 CustomBattle.dll 还没完全 ready · 或 class 名/namespace 跟 Explore agent 推断不同)
                // 下 session 深度诊断 · 可能方案：lazy reflection AccessTools.TypeByName · 延迟到 OnBeforeInitialModuleScreenSetAsRoot
                // CustomBattleTroopBuilder.Patches.ArmyCompositionGroupVMPatch.Apply(_harmony);
                _extender = UIExtender.Create(ModuleId);
                _extender.Register(typeof(CBTBSubModule).Assembly);
                _extender.Enable();
                Log($"[CBTB] OnSubModuleLoad OK · v0.4.2 · Harmony id={HarmonyId} · UIExtender enabled · ArmyCompositionGroupVM patch DISABLED (v0.4.1 crash rollback)");
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
                Log("[CBTB] OnBeforeInitialModuleScreenSetAsRoot · Phase 2b-2 ready");
                InformationManager.DisplayMessage(new InformationMessage(
                    "[CBTB] v0.4.2 loaded. Emergency rollback from v0.4.1 Patch crash · Clone 核心可用 · Custom Battle 菜单 see-clone fix TODO next session.", Colors.Yellow));
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
