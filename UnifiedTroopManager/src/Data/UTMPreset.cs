using System;
using System.Collections.Generic;

namespace UnifiedTroopManager.Data
{
    // Full snapshot of what the player set up in the Manage Troops UI · persisted
    // to disk so the same roster + formation layout can be reused in future battles
    // without redoing the whole picker. Stored under
    // Modules/UnifiedTroopManager/Presets/*.xml so it is global (shared across
    // save files) · player-named for disambiguation.
    public sealed class UTMPreset
    {
        public string Name { get; set; }
        public DateTime Created { get; set; }
        // CharacterObject.StringId → Bring count. Troops in the current party not
        // listed here load as Bring=0; listed troops not in the party are skipped
        // during Apply with an info log.
        public Dictionary<string, int> Roster { get; set; } = new Dictionary<string, int>();
        // CharacterObject.StringId → list of planned Formation slot indices (0..7).
        // Empty list = no plan (vanilla DefaultFormationClass takes over).
        public Dictionary<string, List<int>> Formations { get; set; } = new Dictionary<string, List<int>>();
    }
}
