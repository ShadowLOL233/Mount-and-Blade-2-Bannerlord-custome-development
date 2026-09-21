using System;
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
    // v1.0.2 architecture — deferred manual patching:
    //   RBM's SubModule.OnSubModuleLoad does NOT load RBMAI.dll (verified via dnSpy decompile
    //   2026-09-21: RBM.SubModule.OnSubModuleLoad only touches RBMConfig / prefab patches /
    //   AddInitialStateOption — no RBMAI.* type reference). RBMAI is loaded later via
    //   RBM.SubModule.ApplyHarmonyPatches → RBMAiPatcher.FirstPatch.
    //
    //   Consequence for us: if we do harmony.PatchAll in OnSubModuleLoad with a string-based
    //   [HarmonyPatch("RBMAI.Stance", ...)] attribute, Harmony resolves the type via
    //   AccessTools.TypeByName during PatchAll → returns null (RBMAI not loaded yet) → Harmony
    //   throws → uncaught → native crash before main menu. This is what killed v1.0.1.
    //
    //   Fix: OnSubModuleLoad only constructs the Harmony instance. First Mission start
    //   (OnMissionBehaviorInitialize) triggers TryPatch, which does the reflection + manual
    //   patch. By first mission RBM.ApplyHarmonyPatches has run and RBMAI is loaded.
    //
    //   Failure modes are all soft: TypeByName returns null → set _patchFailed, buff silently
    //   disables, no crash.
    public class SubModule : MBSubModuleBase
    {
        private const string HarmonyId = "RBMPlayerStaminaPoiseBuff";
        private const float StaminaMultiplier = 6f;
        private const float PostureMultiplier = 2f;

        private static Harmony _harmony;
        private static bool _patched;
        private static bool _patchFailed;
        private static FieldInfo _agentStancesValuesField;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            _harmony = new Harmony(HarmonyId);
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            TryPatch();
        }

        private static void TryPatch()
        {
            if (_patched || _patchFailed) return;

            var stanceType = AccessTools.TypeByName("RBMAI.Stance");
            if (stanceType == null)
            {
                _patchFailed = true;
                InformationManager.DisplayMessage(new InformationMessage(
                    "RBM Player Stamina & Poise Buff: RBMAI.Stance not resolvable, buff disabled."));
                return;
            }

            var staminaMethod = AccessTools.Method(stanceType, "tickStaminaRegen");
            var postureMethod = AccessTools.Method(stanceType, "tickPostureRegen");
            if (staminaMethod == null || postureMethod == null)
            {
                _patchFailed = true;
                InformationManager.DisplayMessage(new InformationMessage(
                    "RBM Player Stamina & Poise Buff: tick regen methods not found, buff disabled."));
                return;
            }

            var staminaPrefix = new HarmonyMethod(
                AccessTools.Method(typeof(SubModule), nameof(StaminaPrefix)));
            var posturePrefix = new HarmonyMethod(
                AccessTools.Method(typeof(SubModule), nameof(PosturePrefix)));

            try
            {
                _harmony.Patch(staminaMethod, prefix: staminaPrefix);
                _harmony.Patch(postureMethod, prefix: posturePrefix);
                _patched = true;
                InformationManager.DisplayMessage(new InformationMessage(
                    "RBM Player Stamina & Poise Buff v1.0.2 loaded. Player-only regen: stamina x"
                    + StaminaMultiplier + ", posture x" + PostureMultiplier + ". AI unchanged."));
            }
            catch (Exception ex)
            {
                _patchFailed = true;
                InformationManager.DisplayMessage(new InformationMessage(
                    "RBM Player Stamina & Poise Buff: patch failed: " + ex.Message));
            }
        }

        public static void StaminaPrefix(object __instance, ref float multiplier)
        {
            if (IsPlayerStance(__instance)) multiplier *= StaminaMultiplier;
        }

        public static void PosturePrefix(object __instance, ref float multiplier)
        {
            if (IsPlayerStance(__instance)) multiplier *= PostureMultiplier;
        }

        // Whether this Stance instance belongs to Agent.Main. Reflected access to
        // RBMAI.AgentStances.values (verified via dnSpy: public static
        // ConcurrentDictionary<Agent, Stance>) keeps our assembly free of RBMAI references.
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
            return ReferenceEquals(dict[main], stanceInstance);
        }
    }
}
