using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace PlayerFiefRecruitBoost
{
    // Loaded once at SubModule startup from ModuleData/config.xml.
    // Fields default to safe values if file missing / malformed / partial.
    internal static class Config
    {
        // Multiplier applied to GetDailyVolunteerProductionProbability for
        // notables in player-clan-owned fiefs (and their bound villages).
        // Vanilla = 1.0. Higher = faster slot regen.
        public static float SlotRegenMultiplier = 3.0f;

        // If true, MaximumIndexHeroCanRecruitFromHero for player recruiting
        // in own fief returns max (6) regardless of relation-to-notable.
        public static bool UnlockMaxTierInOwnFiefs = true;

        // If true, village bound to a player-clan-owned town/castle counts
        // as a player fief for the purposes of both patches.
        public static bool IncludeBoundVillages = true;

        // Emit "[PFRB] configuration loaded" line at startup, so you can
        // confirm the mod is live.
        public static bool LogOnLoad = true;

        public static void Load()
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                var dllPath = asm.Location;
                // <ModRoot>/bin/Win64_Shipping_Client/PlayerFiefRecruitBoost.dll
                // → <ModRoot>/ModuleData/config.xml
                var modRoot = Directory.GetParent(Path.GetDirectoryName(dllPath))?.Parent?.FullName;
                if (modRoot == null) return;
                var cfgPath = Path.Combine(modRoot, "ModuleData", "config.xml");
                if (!File.Exists(cfgPath)) return;

                var doc = new XmlDocument();
                doc.Load(cfgPath);
                var root = doc.DocumentElement;
                if (root == null) return;

                TryReadFloat(root, "SlotRegenMultiplier", ref SlotRegenMultiplier);
                TryReadBool (root, "UnlockMaxTierInOwnFiefs", ref UnlockMaxTierInOwnFiefs);
                TryReadBool (root, "IncludeBoundVillages",   ref IncludeBoundVillages);
                TryReadBool (root, "LogOnLoad",              ref LogOnLoad);

                // Clamp to sane range so a typo like 100 doesn't blow up.
                if (SlotRegenMultiplier < 0.1f) SlotRegenMultiplier = 0.1f;
                if (SlotRegenMultiplier > 20f)  SlotRegenMultiplier = 20f;
            }
            catch
            {
                // Silent — defaults already in place.
            }
        }

        private static void TryReadFloat(XmlElement root, string tag, ref float target)
        {
            var node = root[tag];
            if (node == null) return;
            if (float.TryParse(node.InnerText.Trim(), System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var v))
            {
                target = v;
            }
        }

        private static void TryReadBool(XmlElement root, string tag, ref bool target)
        {
            var node = root[tag];
            if (node == null) return;
            if (bool.TryParse(node.InnerText.Trim(), out var v))
            {
                target = v;
            }
        }
    }
}
