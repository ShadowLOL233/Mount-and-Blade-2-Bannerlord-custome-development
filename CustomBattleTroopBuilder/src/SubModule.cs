using System;
using System.IO;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

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
            // Phase 1 hotkey: Ctrl+Shift+B emits an info message proving mod is live.
            // Phase 2 will replace this with proper ButterLib hotkey registration +
            // CustomBattleMenu button + full Gauntlet editor screen.
            try
            {
                if (Input.IsKeyDown(InputKey.LeftControl) && Input.IsKeyDown(InputKey.LeftShift) && Input.IsKeyPressed(InputKey.B))
                {
                    InformationManager.DisplayMessage(new InformationMessage(
                        "[CBTB] Hotkey Ctrl+Shift+B pressed · scaffold works · Phase 2 editor coming",
                        Colors.Green));
                    Log("[CBTB] Hotkey Ctrl+Shift+B pressed");
                }
            }
            catch { }
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
