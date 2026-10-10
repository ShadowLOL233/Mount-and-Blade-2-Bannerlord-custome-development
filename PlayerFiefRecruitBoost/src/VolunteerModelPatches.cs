using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;

namespace PlayerFiefRecruitBoost
{
    // ---- Helpers ------------------------------------------------------------

    internal static class PlayerFiefCheck
    {
        // A settlement counts as "player fief" if the player's clan owns it,
        // or (opt-in) if it's a village bound to a player-owned town/castle.
        public static bool IsPlayerFief(Settlement s)
        {
            if (s == null) return false;
            var playerClan = Clan.PlayerClan;
            if (playerClan == null) return false;

            if (s.OwnerClan == playerClan) return true;

            if (Config.IncludeBoundVillages && s.IsVillage)
            {
                var bound = s.Village?.Bound;
                if (bound != null && bound.OwnerClan == playerClan) return true;
            }
            return false;
        }
    }

    // ---- Patch A: faster volunteer slot regen in player fiefs --------------
    //
    // Vanilla GetDailyVolunteerProductionProbability returns a per-slot per-day
    // fill probability (0..1). Boosting it makes notables' 6 slots fill faster,
    // giving the player more recruits per village visit.
    //
    // Signature confirmed by ilspycmd against TaleWorlds.CampaignSystem.dll
    // (v1.4.7): DefaultVolunteerModel.GetDailyVolunteerProductionProbability(
    //   Hero hero, int index, Settlement settlement)  →  float

    [HarmonyPatch(typeof(DefaultVolunteerModel), nameof(DefaultVolunteerModel.GetDailyVolunteerProductionProbability))]
    [HarmonyAfter("RBM", "com.rbmcombat.harmony")]  // safe no-op if RBM absent
    internal static class Patch_GetDailyVolunteerProductionProbability
    {
        static void Postfix(Hero hero, int index, Settlement settlement, ref float __result)
        {
            if (settlement == null) return;
            if (!PlayerFiefCheck.IsPlayerFief(settlement)) return;

            var boosted = __result * Config.SlotRegenMultiplier;
            // Clamp: probability must not exceed 1.0 (avoids RNG weirdness).
            if (boosted > 1f) boosted = 1f;
            __result = boosted;
        }
    }

    // ---- Patch B: unlock max tier for player in own fiefs ------------------
    //
    // Vanilla MaximumIndexHeroCanRecruitFromHero returns a tier index [0..6],
    // gated primarily by relation-to-notable (relation ≥ 100 needed for full 6).
    // In player-clan-owned fiefs we treat the player as always having full
    // access, since it's your own settlement — this cuts the "grind relation
    // before you can hire top-tier recruits from your own villages" pain.
    //
    // Signature confirmed: MaximumIndexHeroCanRecruitFromHero(Hero buyerHero,
    //   Hero sellerHero, int useValueAsRelation = -101) → int
    //
    // We only override when the buyer is the player AND the seller notable
    // sits in a player-clan-owned fief. AI recruitment path is untouched.

    [HarmonyPatch(typeof(DefaultVolunteerModel), nameof(DefaultVolunteerModel.MaximumIndexHeroCanRecruitFromHero))]
    [HarmonyAfter("RBM", "com.rbmcombat.harmony")]
    internal static class Patch_MaximumIndexHeroCanRecruitFromHero
    {
        static void Postfix(Hero buyerHero, Hero sellerHero, ref int __result)
        {
            if (!Config.UnlockMaxTierInOwnFiefs) return;
            if (buyerHero == null || sellerHero == null) return;
            if (buyerHero != Hero.MainHero) return;

            var s = sellerHero.CurrentSettlement;
            if (s == null) return;
            if (!PlayerFiefCheck.IsPlayerFief(s)) return;

            // Vanilla max is 6 (MathF.Min(6, ...)).
            if (__result < 6) __result = 6;
        }
    }
}
