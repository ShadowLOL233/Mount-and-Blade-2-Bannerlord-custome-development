using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace CheatsGuard
{
    internal static class Config
    {
        public static bool VerboseLog = true;
        public static readonly HashSet<string> SkipSet = new HashSet<string>(StringComparer.Ordinal);
        public static string ModRoot { get; private set; }

        // Nuclear option: when true, skip ALL BannerlordCheats.Patches.* classes
        // (regardless of skip-list). For bisecting "installed patch crashes at runtime" vs
        // "install phase itself crashes". Use to confirm root cause quickly.
        public static bool SkipAllCheatsPatches = false;

        // Replacement patch: NativeItemSpawning (replaces Cheats' broken 10-param version)
        // When true, opening the player inventory (outside trading + outside vanilla cheat mode)
        // injects NativeItemSpawnCount of every registered ItemObject (auto-includes OSA/OSW/Saddlery).
        public static bool EnableNativeItemSpawning = false;
        public static int  NativeItemSpawnCount = 1;

        // Replacement patch: EnableHotkeysMoney (raises Cheats' hardcoded 1k/100k to 10M/100M).
        // Ctrl+X in inventory UI → +CheatsMoneySmall · Ctrl+Shift+X → +CheatsMoneyLarge.
        // Cheats' original EnableHotkeysMoney is skipped via config.xml SkipList so amounts don't double up.
        public static bool EnableMoneyHotkey = true;
        public static long CheatsMoneySmall = 10_000_000L;
        public static long CheatsMoneyLarge = 100_000_000L;
        // Whether to flash a hotkey tip whenever the player opens the inventory screen.
        public static bool ShowMoneyHotkeyTip = true;

        // RBM Troop Maintenance mirrors. These are applied to RBMConfig statics on load
        // (and again whenever MCM values change) via RBMBridge. Values here act as
        // config.xml-driven fallback defaults when MCM hasn't overridden yet.
        public static float RBMTroopMaintenanceFraction = 0.005f;
        public static int   RBMRecruitMaintenanceDays   = 20;
        public static float RBMIndependentPurseFraction = 1.0f;

        // Passive daily XP for player + companions in the main party.
        // Amount is applied per-skill (i.e. every skill on every eligible hero gets +Amount every daily tick).
        public static bool EnablePassiveDailyXp = true;
        public static int  PassiveDailyXpAmount = 1000;
        public static bool PassiveDailyXpIncludeCompanions = true;

        public static void Load()
        {
            var asmPath = Assembly.GetExecutingAssembly().Location;
            // <ModRoot>/bin/Win64_Shipping_Client/CheatsGuard.dll → <ModRoot>
            ModRoot = Directory.GetParent(Path.GetDirectoryName(asmPath))?.Parent?.FullName;
            if (ModRoot == null) return;

            var cfgPath = Path.Combine(ModRoot, "ModuleData", "config.xml");
            if (!File.Exists(cfgPath)) return;

            try
            {
                var doc = new XmlDocument();
                doc.Load(cfgPath);
                var root = doc.DocumentElement;
                if (root == null) return;

                var verboseNode = root["VerboseLog"];
                if (verboseNode != null && bool.TryParse(verboseNode.InnerText.Trim(), out var v))
                    VerboseLog = v;

                var skipAllNode = root["SkipAllCheatsPatches"];
                if (skipAllNode != null && bool.TryParse(skipAllNode.InnerText.Trim(), out var sa))
                    SkipAllCheatsPatches = sa;

                var enableNode = root["EnableNativeItemSpawning"];
                if (enableNode != null && bool.TryParse(enableNode.InnerText.Trim(), out var e))
                    EnableNativeItemSpawning = e;

                var countNode = root["NativeItemSpawnCount"];
                if (countNode != null && int.TryParse(countNode.InnerText.Trim(), out var c))
                {
                    if (c < 1) c = 1;
                    if (c > 99) c = 99;
                    NativeItemSpawnCount = c;
                }

                var moneyHotkeyNode = root["EnableMoneyHotkey"];
                if (moneyHotkeyNode != null && bool.TryParse(moneyHotkeyNode.InnerText.Trim(), out var mh))
                    EnableMoneyHotkey = mh;

                var moneySmallNode = root["CheatsMoneySmall"];
                if (moneySmallNode != null && long.TryParse(moneySmallNode.InnerText.Trim(), out var ms))
                {
                    if (ms < 0L) ms = 0L;
                    CheatsMoneySmall = ms;
                }

                var moneyLargeNode = root["CheatsMoneyLarge"];
                if (moneyLargeNode != null && long.TryParse(moneyLargeNode.InnerText.Trim(), out var ml))
                {
                    if (ml < 0L) ml = 0L;
                    CheatsMoneyLarge = ml;
                }

                var showTipNode = root["ShowMoneyHotkeyTip"];
                if (showTipNode != null && bool.TryParse(showTipNode.InnerText.Trim(), out var st))
                    ShowMoneyHotkeyTip = st;

                var rbmFracNode = root["RBMTroopMaintenanceFraction"];
                if (rbmFracNode != null && float.TryParse(rbmFracNode.InnerText.Trim(),
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out var rf))
                {
                    if (rf < 0f) rf = 0f;
                    RBMTroopMaintenanceFraction = rf;
                }

                var rbmRecruitNode = root["RBMRecruitMaintenanceDays"];
                if (rbmRecruitNode != null && int.TryParse(rbmRecruitNode.InnerText.Trim(), out var rd))
                {
                    if (rd < 0) rd = 0;
                    RBMRecruitMaintenanceDays = rd;
                }

                var rbmPurseNode = root["RBMIndependentPurseFraction"];
                if (rbmPurseNode != null && float.TryParse(rbmPurseNode.InnerText.Trim(),
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out var rp))
                {
                    if (rp < 0f) rp = 0f;
                    RBMIndependentPurseFraction = rp;
                }

                var pxpEnableNode = root["EnablePassiveDailyXp"];
                if (pxpEnableNode != null && bool.TryParse(pxpEnableNode.InnerText.Trim(), out var pxe))
                    EnablePassiveDailyXp = pxe;

                var pxpAmountNode = root["PassiveDailyXpAmount"];
                if (pxpAmountNode != null && int.TryParse(pxpAmountNode.InnerText.Trim(), out var pxa))
                {
                    if (pxa < 0) pxa = 0;
                    PassiveDailyXpAmount = pxa;
                }

                var pxpCompNode = root["PassiveDailyXpIncludeCompanions"];
                if (pxpCompNode != null && bool.TryParse(pxpCompNode.InnerText.Trim(), out var pxc))
                    PassiveDailyXpIncludeCompanions = pxc;

                var skipRoot = root["SkipClasses"];
                if (skipRoot != null)
                {
                    foreach (XmlNode child in skipRoot.ChildNodes)
                    {
                        if (child.NodeType != XmlNodeType.Element) continue;
                        if (child.LocalName != "SkipClass") continue;
                        var fqn = child.InnerText.Trim();
                        if (!string.IsNullOrEmpty(fqn)) SkipSet.Add(fqn);
                    }
                }
            }
            catch
            {
                // Silent — defaults already in place.
            }
        }

        public static bool IsSkipped(string typeFullName)
        {
            return typeFullName != null && SkipSet.Contains(typeFullName);
        }
    }
}
