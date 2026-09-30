using System.Collections.Generic;

namespace UnifiedTroopManager.Data
{
    // Per DESIGN §3.4 · in-memory only, one active selection at a time.
    // Populated when the user confirms in the encounter menu UI;
    // reset by PlayerEncounterFinishPatch when the encounter ends.
    public sealed class RosterSelection
    {
        // troopStringId → how many the player agreed to bring this battle.
        public Dictionary<string, int> Counts { get; } = new Dictionary<string, int>();

        public bool IsEmpty => Counts.Count == 0;

        public static RosterSelection Current { get; private set; }

        public static void SetCurrent(RosterSelection selection) { Current = selection; }
        public static void Clear() { Current = null; }
    }
}
