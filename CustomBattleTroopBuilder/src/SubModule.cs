using System;
using System.IO;
using System.Linq;
using CustomBattleTroopBuilder.UI;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;

namespace CustomBattleTroopBuilder
{
    public sealed class CBTBSubModule : MBSubModuleBase
    {
        public const string HarmonyId = "com.pslr.cbtb";
        public const string LogFile = "CustomBattleTroopBuilder.log";
        private static Harmony? _harmony;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                _harmony = new Harmony(HarmonyId);
                // Phase 1 scaffold: no PatchAll yet — Harmony instance ready for Phase 2 UI patches.
                Log($"[CBTB] OnSubModuleLoad OK · v0.1.0-scaffold · Harmony id={HarmonyId}");
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
                Log("[CBTB] OnBeforeInitialModuleScreenSetAsRoot · Phase 1 scaffold ready");
                InformationManager.DisplayMessage(new InformationMessage(
                    "[CBTB] v0.1.0-scaffold loaded. Phase 2 UI coming.", Colors.Yellow));
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
                    OpenEditorPlaceholder();
                }
            }
            catch (Exception e)
            {
                Log($"[CBTB] hotkey handler failed: {e}");
            }
        }

        /// <summary>
        /// Phase 2a placeholder editor · uses vanilla InquiryData instead of full Gauntlet screen.
        /// Phase 2b will replace with real ScreenBase + Gauntlet editor + CustomBattleMenu button.
        /// </summary>
        private static void OpenEditorPlaceholder()
        {
            Log("[CBTB] OpenEditorPlaceholder invoked · pushing CBTBEditorScreen");
            try
            {
                ScreenManager.PushScreen(new CBTBEditorScreen());
            }
            catch (Exception e)
            {
                Log($"[CBTB] PushScreen failed: {e}");
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
