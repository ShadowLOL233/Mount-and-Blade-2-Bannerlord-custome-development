using System.Collections.Generic;

namespace UnifiedTroopManager.Data
{
    // Persisted per-hero plan (DESIGN §3.1 + §3.3).
    // SchemaVersion lets us migrate the JSON blob when fields change.
    public sealed class PartyPlan
    {
        public int SchemaVersion { get; set; } = 1;
        public string HeroStringId { get; set; }
        public Dictionary<string, TroopFormationPlan> FormationPlans { get; set; }
            = new Dictionary<string, TroopFormationPlan>();
    }
}
