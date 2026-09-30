using System.Collections.Generic;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Data
{
    // Facade over the on-disk PartyPlan store · JSON I/O lands in Phase 3.
    // Phase 1 keeps an in-memory dictionary so Phase 2/3 code has a stable API
    // to call without touching the filesystem yet.
    public static class PartyPlanStore
    {
        private static readonly Dictionary<string, PartyPlan> _byHero
            = new Dictionary<string, PartyPlan>();

        public static PartyPlan GetOrCreate(string heroStringId)
        {
            if (string.IsNullOrEmpty(heroStringId)) return null;
            if (_byHero.TryGetValue(heroStringId, out var plan)) return plan;
            plan = new PartyPlan { HeroStringId = heroStringId };
            _byHero[heroStringId] = plan;
            UTMLog.Debug("PartyPlanStore: created empty plan for " + heroStringId);
            return plan;
        }

        public static PartyPlan Current
        {
            get
            {
                // Real Campaign.Current?.MainParty?.LeaderHero lookup lands in Phase 2.
                // For Phase 1 scaffold we just hand out a singleton keyed by a stub id.
                return GetOrCreate("main_hero");
            }
        }

        public static void Save(PartyPlan plan)
        {
            // Phase 3 · write JSON to Configs/UnifiedTroopManager/<hero>.json.
            // Phase 1 no-op keeps the API surface stable for callers.
            if (plan == null || string.IsNullOrEmpty(plan.HeroStringId)) return;
            _byHero[plan.HeroStringId] = plan;
        }
    }
}
