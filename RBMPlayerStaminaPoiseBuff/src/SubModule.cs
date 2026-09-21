using System.Collections;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RBMPlayerStaminaPoiseBuff
{
    // Boosts RBM stamina/posture regen for the player only. Multipliers hardcoded per user
    // preference (2026-09-21): stamina x6, posture x2. AI stays on RBM defaults.
    //
    // Why string-based Harmony patches instead of typeof(RBMAI.Stance): RBM's SubModule.xml only
    // registers RBM.dll as its SubModule DLL — RBMAI.dll is loaded dynamically by RBM.dll at
    // runtime and is NOT in the launcher's static-scan search path. If we use `typeof(Stance)`
    // in a [HarmonyPatch] attribute, the launcher's DLL verifier tries to resolve that type
    // during its static scan (before any game code runs), fails, and flags our DLL as
    // IsDangerous=true — which then either auto-disables the mod or causes a native crash when
    // the launcher force-loads a "dangerous" DLL alongside a partly-initialised managed context.
    //
    // String-based [HarmonyPatch("Namespace.Type", "method")] defers type resolution to runtime
    // via AccessTools.TypeByName, when RBM.dll has already caused RBMAI.dll to load. Passes the
    // launcher's static scan cleanly.
    //
    // Same story for our AgentStances.values lookup — we do that via reflection instead of a
    // direct `AgentStances.values` reference, keeping our assembly's static type table free of
    // RBMAI references altogether.
    public class SubModule : MBSubModuleBase
    {
        private const string HarmonyId = "RBMPlayerStaminaPoiseBuff";
        private const float StaminaMultiplier = 6f;
        private const float PostureMultiplier = 2f;

        // Cached at first successful lookup, then reused. Null when RBMAI.dll not yet loaded
        // OR when reflection paths fail — in either case the buff silently no-ops that tick.
        private static FieldInfo _agentStancesValuesField;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            var harmony = new Harmony(HarmonyId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            var gtName = game.GameType?.GetType().Name ?? "";
            if (gtName == "Campaign")
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "RBM Player Stamina & Poise Buff v1.0.1 loaded. Player-only regen: stamina x"
                    + StaminaMultiplier + ", posture x" + PostureMultiplier
                    + ". AI unchanged."));
            }
        }

        internal static float GetStaminaMultiplier() => StaminaMultiplier;
        internal static float GetPostureMultiplier() => PostureMultiplier;

        // Whether this Stance instance belongs to Agent.Main. Everything is reflected so the
        // assembly never statically references RBMAI types — required to keep the launcher's
        // DLL scan happy (see class-level comment).
        internal static bool IsPlayerStance(object stanceInstance)
        {
            if (stanceInstance == null) return false;
            var main = Agent.Main;
            if (main == null) return false;

            if (_agentStancesValuesField == null)
            {
                var t = AccessTools.TypeByName("RBMAI.AgentStances");
                if (t == null) return false;
                _agentStancesValuesField = AccessTools.Field(t, "values");
                if (_agentStancesValuesField == null) return false;
            }

            var dict = _agentStancesValuesField.GetValue(null) as IDictionary;
            if (dict == null) return false;
            if (!dict.Contains(main)) return false;
            var playerStance = dict[main];
            return ReferenceEquals(playerStance, stanceInstance);
        }
    }

    // String-based patch target — no typeof(RBMAI.Stance) → launcher static scan passes.
    // AccessTools.TypeByName runs at PatchAll time when RBMAI.dll is loaded.
    [HarmonyPatch("RBMAI.Stance", "tickStaminaRegen")]
    public static class StaminaRegenPatch
    {
        // __instance is boxed as object because we deliberately avoid `Stance` type in the
        // signature. Harmony still resolves it correctly by name matching.
        public static void Prefix(object __instance, ref float multiplier)
        {
            if (SubModule.IsPlayerStance(__instance))
            {
                multiplier *= SubModule.GetStaminaMultiplier();
            }
        }
    }

    [HarmonyPatch("RBMAI.Stance", "tickPostureRegen")]
    public static class PostureRegenPatch
    {
        public static void Prefix(object __instance, ref float multiplier)
        {
            if (SubModule.IsPlayerStance(__instance))
            {
                multiplier *= SubModule.GetPostureMultiplier();
            }
        }
    }
}
