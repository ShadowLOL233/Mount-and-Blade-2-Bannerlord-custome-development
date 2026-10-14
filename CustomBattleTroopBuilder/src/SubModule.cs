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
                Log($"[CBTB] OnSubModuleLoad OK · v0.4.5 · Harmony id={HarmonyId} · UIExtender enabled · Restore moved to CtorPostfix (MBObjectManager ready) · !IsHero filter removed");
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
                // v0.4.3: Apply Harmony patch 延迟到此时机 · CustomBattle.dll 已完全 load
                if (_harmony != null)
                {
                    try
                    {
                        CustomBattleTroopBuilder.Patches.ArmyCompositionGroupVMPatch.Apply(_harmony);
                    }
                    catch (Exception px)
                    {
                        Log($"[CBTB] ArmyCompositionGroupVMPatch.Apply failed (non-fatal): {px}");
                    }
                }

                // v0.4.4: Restore clones · OnBeforeInitialModuleScreenSetAsRoot fire 多次
                // (游戏启动 + 每次 Back-to-main-menu 都 fire · Game re-init 清空 MBObjectManager pool)
                // Registry 保存 blueprint 跨 Game re-init · 这里重 clone 回 pool
                try
                {
                    CustomBattleTroopBuilder.CBTBClonesRegistry.Restore();
                }
                catch (Exception rx)
                {
                    Log($"[CBTB] CBTBClonesRegistry.Restore failed (non-fatal): {rx}");
                }

                Log($"[CBTB] OnBeforeInitialModuleScreenSetAsRoot · Phase 3b-2b v0.4.4 ready · registry blueprints={CustomBattleTroopBuilder.CBTBClonesRegistry.Count}");
                InformationManager.DisplayMessage(new InformationMessage(
                    "[CBTB] v0.4.5 loaded. Phase 3b-2b: Restore in CtorPostfix (fix OnBefore-null) · filter aligned with vanilla (restore Retinues placeholders).", Colors.Yellow));
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
