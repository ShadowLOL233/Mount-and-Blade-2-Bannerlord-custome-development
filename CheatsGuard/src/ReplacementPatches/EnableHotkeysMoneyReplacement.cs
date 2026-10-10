using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace CheatsGuard.ReplacementPatches
{
    // Drop-in replacement for BannerlordCheats.Patches.General.EnableHotkeysMoney.
    //
    // Cheats hardcodes +1,000 (Ctrl+X) and +100,000 (Ctrl+Shift+X). User wants those raised
    // to +10M / +100M. Rather than IL-patch the workshop DLL (Steam would overwrite on update),
    // we skip the original class via SkipClasses and provide our own OnTick postfix here.
    //
    // Amounts are pulled from Config so they can be tuned in ModuleData/config.xml without rebuild.
    //
    // Behaviour is identical to Cheats:
    //   - Only fires when the topmost screen is the Gauntlet inventory screen.
    //   - Ctrl+X (KeyPressed edge-triggered) with Shift NOT held → small amount.
    //   - Ctrl+Shift+X (KeyPressed edge-triggered) → large amount.
    //
    // We resolve GauntletInventoryScreen by type name string so we don't have to add a
    // reference to SandBox.GauntletUI.dll.

    [HarmonyPatch(typeof(GameManagerBase), "OnTick")]
    internal static class EnableHotkeysMoneyReplacement
    {
        [HarmonyPostfix]
        static void Postfix(float dt)
        {
            if (!Config.EnableMoneyHotkey) return;

            try
            {
                var top = ScreenManager.TopScreen;
                if (top == null) return;
                if (top.GetType().Name != "GauntletInventoryScreen") return;

                if (!Input.IsKeyPressed(InputKey.X)) return;

                bool ctrl  = Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl);
                if (!ctrl) return;

                bool shift = Input.IsKeyDown(InputKey.LeftShift)   || Input.IsKeyDown(InputKey.RightShift);

                var hero = Hero.MainHero;
                if (hero == null) return;

                long amount = shift ? Config.CheatsMoneyLarge : Config.CheatsMoneySmall;
                if (amount <= 0L) return;
                int add = amount > int.MaxValue ? int.MaxValue : (int)amount;

                hero.ChangeHeroGold(add);
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[CheatsGuard] +{add:N0} denars",
                    Colors.Yellow));
            }
            catch (Exception ex)
            {
                LogFile.Write($"EnableHotkeysMoneyReplacement runtime error: {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
