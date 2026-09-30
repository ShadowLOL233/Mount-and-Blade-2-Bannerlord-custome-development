using System.Collections.Generic;

namespace UnifiedTroopManager.Data
{
    // Per DESIGN §3.1 · a troop's assignment to one or more formations.
    // Mode == SingleFormation → look at SingleFormationIndex (0..7).
    // Mode == Split           → look at Splits, weights normalized by SplitAllocator (Phase 3).
    public enum FormationPlanMode
    {
        SingleFormation = 0,
        Split = 1
    }

    public sealed class SplitEntry
    {
        public int FormationIndex { get; set; }
        public int Weight { get; set; }
    }

    public sealed class TroopFormationPlan
    {
        public FormationPlanMode Mode { get; set; } = FormationPlanMode.SingleFormation;
        public int? SingleFormationIndex { get; set; }
        public List<SplitEntry> Splits { get; set; }

        public static TroopFormationPlan Single(int formationIndex)
        {
            return new TroopFormationPlan
            {
                Mode = FormationPlanMode.SingleFormation,
                SingleFormationIndex = formationIndex
            };
        }
    }
}
