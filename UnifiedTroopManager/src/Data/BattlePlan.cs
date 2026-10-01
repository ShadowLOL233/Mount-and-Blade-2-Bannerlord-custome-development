using System.Collections.Generic;

namespace UnifiedTroopManager.Data
{
    // In-memory per-battle Formation plan snapshot. Written by the UI when the
    // player hits Save Selection; read by the OrderOfBattleInitializePatch
    // (to open all Formation class slots) and MissionSpawnTroopPatch (to
    // reassign each spawned agent to its planned Formation).
    //
    // Scoped to a single vanilla battle lifecycle — cleared by
    // PlayerEncounterFinishPatch alongside RosterSelection and
    // UTMBattleState.
    public sealed class BattlePlan
    {
        // CharacterObject.StringId → list of planned Formation indices (0..7).
        // Multi-entry list = split plan · agents are distributed across slots
        // round-robin by their spawn order within the troop type.
        public Dictionary<string, List<int>> Formations { get; }
            = new Dictionary<string, List<int>>();

        public bool IsEmpty => Formations.Count == 0;
    }

    public static class PartyPlanRuntime
    {
        public static BattlePlan Current { get; private set; }

        public static void SetCurrent(BattlePlan plan) { Current = plan; }
        public static void Clear() { Current = null; }
    }
}
