using System;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace CheatsGuard.ReplacementPatches
{
    // Shows a hotkey tip whenever the player opens the Gauntlet inventory screen.
    // Mirrors Cheats' EnableHotkeyTipsScreens pattern (patch on ScreenManager.PushScreen)
    // but scoped to just the money hotkeys and pulls amounts from Config so the tip
    // always matches the currently configured Small/Large values.
    //
    // Cheats' own EnableHotkeyTipsScreens stays skipped (would print stale 1k/100k text).

    [HarmonyPatch(typeof(ScreenManager), "PushScreen")]
    internal static class MoneyHotkeyTipsPatch
    {
        [HarmonyPostfix]
        static void Postfix(ScreenBase screen)
        {
            if (!Config.EnableMoneyHotkey || !Config.ShowMoneyHotkeyTip) return;
            if (screen == null) return;

            try
            {
                if (screen.GetType().Name != "GauntletInventoryScreen") return;

                InformationManager.DisplayMessage(new InformationMessage(
                    "[CheatsGuard] Money hotkeys:", Colors.Yellow));
                InformationManager.DisplayMessage(new InformationMessage(
                    $"  Ctrl+X       → +{FormatAmount(Config.CheatsMoneySmall)} denars", Colors.Yellow));
                InformationManager.DisplayMessage(new InformationMessage(
                    $"  Ctrl+Shift+X → +{FormatAmount(Config.CheatsMoneyLarge)} denars", Colors.Yellow));
            }
            catch (Exception ex)
            {
                LogFile.Write($"MoneyHotkeyTipsPatch error: {ex.GetType().Name}: {ex.Message}");
            }
        }

        static string FormatAmount(long amount)
        {
            if (amount >= 1_000_000_000L) return $"{amount / 1_000_000_000.0:0.##}B";
            if (amount >= 1_000_000L)     return $"{amount / 1_000_000.0:0.##}M";
            if (amount >= 1_000L)         return $"{amount / 1_000.0:0.##}k";
            return amount.ToString("N0");
        }
    }
}
