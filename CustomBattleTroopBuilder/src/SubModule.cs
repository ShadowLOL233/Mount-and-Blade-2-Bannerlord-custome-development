using System;
using System.IO;
using System.Linq;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

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
            Log("[CBTB] OpenEditorPlaceholder invoked");
            int troopCount = 0;
            string sampleTroops = "";
            try
            {
                var all = MBObjectManager.Instance?.GetObjectTypeList<BasicCharacterObject>();
                if (all != null)
                {
                    troopCount = all.Count;
                    var sample = all.Where(c => c != null && !string.IsNullOrEmpty(c.StringId))
                                    .OrderBy(c => c.StringId)
                                    .Take(5)
                                    .Select(c => $"  - {c.StringId}  ({c.Culture?.StringId ?? "?"}, level {c.Level})");
                    sampleTroops = string.Join("\n", sample);
                }
            }
            catch (Exception e)
            {
                Log($"[CBTB] troop enum failed: {e}");
            }

            string body =
                $"CBTB v0.1.0 · Phase 2a placeholder dialog.\n\n" +
                $"Troop pool enumeration test:\n" +
                $"  Total BasicCharacterObject count: {troopCount}\n" +
                $"  First 5 (alphabetical):\n{sampleTroops}\n\n" +
                "Phase 2b (next session): Custom Battle menu button + real Gauntlet editor screen.\n" +
                "Phase 3+: base troop picker + equipment/skill editor + JSON persistence.";

            InformationManager.ShowInquiry(new InquiryData(
                titleText: "CBTB Editor (Phase 2a)",
                text: body,
                isAffirmativeOptionShown: true,
                isNegativeOptionShown: false,
                affirmativeText: "OK",
                negativeText: "",
                affirmativeAction: () =>
                {
                    InformationManager.DisplayMessage(new InformationMessage(
                        $"[CBTB] Dialog OK · {troopCount} troops enumerable · Phase 2b incoming", Colors.Green));
                    Log($"[CBTB] Dialog OK · troopCount={troopCount}");
                },
                negativeAction: () => { },
                soundEventPath: ""));
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
