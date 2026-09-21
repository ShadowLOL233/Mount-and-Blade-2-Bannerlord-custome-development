using System.Reflection;
using HarmonyLib;
using RBMAI;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RBMPlayerStaminaPoiseBuff
{
    // Boosts RBM stamina/posture regen rate for the player only, leaving AI at RBM's default
    // values so combat difficulty vs AI stays unchanged. Multipliers hardcoded per user
    // preference (2026-09-21):
    //   Stamina regen  x6
    //   Posture regen  x2
    //
    // No changes to max pool, damage/reduction, or any AI behavior. Vanilla mechanic: RBM's
    // Stance.tickStaminaRegen/tickPostureRegen apply a `multiplier` parameter to the tick's
    // regen amount; we Prefix-patch it and, if the Stance instance is the player's, multiply
    // the multiplier by our factor before the original method runs.
    //
    // Player detection uses AgentStances.values[Agent.Main] — RBM's per-agent Stance lookup
    // (ConcurrentDictionary). O(1) hash lookup per regen tick.
    public class SubModule : MBSubModuleBase
    {
        private const string HarmonyId = "RBMPlayerStaminaPoiseBuff";
        private const float StaminaMultiplier = 6f;
        private const float PostureMultiplier = 2f;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            var harmony = new Harmony(HarmonyId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            // Game.GameType type name check avoids taking a CampaignSystem.dll reference just
            // for a one-line diagnostic. Announcement is harmless on any game type.
            var gtName = game.GameType?.GetType().Name ?? "";
            if (gtName == "Campaign")
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "RBM Player Stamina & Poise Buff v1.0 loaded. Player-only regen: stamina x"
                    + StaminaMultiplier + ", posture x" + PostureMultiplier
                    + ". AI unchanged."));
            }
        }

        internal static float GetStaminaMultiplier() => StaminaMultiplier;
        internal static float GetPostureMultiplier() => PostureMultiplier;

        internal static bool IsPlayerStance(Stance stance)
        {
            var main = Agent.Main;
            if (main == null || stance == null) return false;
            return AgentStances.values.TryGetValue(main, out var playerStance)
                && ReferenceEquals(playerStance, stance);
        }
    }

    [HarmonyPatch(typeof(Stance), nameof(Stance.tickStaminaRegen))]
    public static class StaminaRegenPatch
    {
        // Prefix mutates the value-type `multiplier` parameter via `ref`. Harmony supports this
        // on prefix — the original method sees the boosted value and applies it through its
        // normal rubber-band + tickCount pipeline. Preserves all RBM's regen math.
        public static void Prefix(Stance __instance, ref float multiplier)
        {
            if (SubModule.IsPlayerStance(__instance))
            {
                multiplier *= SubModule.GetStaminaMultiplier();
            }
        }
    }

    [HarmonyPatch(typeof(Stance), nameof(Stance.tickPostureRegen))]
    public static class PostureRegenPatch
    {
        public static void Prefix(Stance __instance, ref float multiplier)
        {
            if (SubModule.IsPlayerStance(__instance))
            {
                multiplier *= SubModule.GetPostureMultiplier();
            }
        }
    }
}
