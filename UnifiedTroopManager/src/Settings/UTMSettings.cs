using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace UnifiedTroopManager.Settings
{
    // Phase 1 skeleton per DESIGN §7.1 · master toggle + per-feature switches + log level.
    // Phases 2-4 will add: default fill mode enum, backline reassignment, etc.
    // All fields default to the "safe / expected" value so a fresh install just works.
    public sealed class UTMSettings : AttributeGlobalSettings<UTMSettings>
    {
        public override string Id => "UnifiedTroopManager_v1";
        public override string DisplayName => "Unified Troop Manager";
        public override string FolderName => "UnifiedTroopManager";
        public override string FormatType => "json2";

        // ---- General ------------------------------------------------------------
        [SettingPropertyGroup("General", GroupOrder = 0)]
        [SettingPropertyBool("Enable Unified Troop Manager", Order = 0, HintText = "Master switch. When off no patches run · vanilla behaviour restored.")]
        public bool MasterEnabled { get; set; } = true;

        [SettingPropertyGroup("General", GroupOrder = 0)]
        [SettingPropertyBool("Show \"Manage Troops\" in encounter menu", Order = 1, HintText = "Add the button that opens the roster + formation UI before each battle.")]
        public bool ShowEncounterMenuButton { get; set; } = true;

        [SettingPropertyGroup("General", GroupOrder = 0)]
        [SettingPropertyBool("Remember last-battle roster on retreat/retry", Order = 2, HintText = "Skip re-selection when you retreat and re-engage the same enemy.")]
        public bool RememberLastRoster { get; set; } = true;

        // ---- Roster -------------------------------------------------------------
        [SettingPropertyGroup("Roster", GroupOrder = 1)]
        [SettingPropertyBool("Filter troops via MapEventSide.AllocateTroops", Order = 0, HintText = "Core CYT-style roster filter. Turn off only if you suspect a conflict.")]
        public bool RosterFilterEnabled { get; set; } = true;

        [SettingPropertyGroup("Roster", GroupOrder = 1)]
        [SettingPropertyBool("Auto-fill remaining battle slots (vanilla order)", Order = 1, HintText = "When your selection is smaller than battle size · auto-fill the rest in vanilla party order (Open Question O-5 · option α).")]
        public bool AutoFillRemaining { get; set; } = true;

        // ---- Formation ----------------------------------------------------------
        [SettingPropertyGroup("Formation", GroupOrder = 2)]
        [SettingPropertyBool("Apply formation plans at battle start", Order = 0, HintText = "Reassign each troop to its planned formation via Mission.SpawnTroop.")]
        public bool ApplyFormationPlans { get; set; } = true;

        [SettingPropertyGroup("Formation", GroupOrder = 2)]
        [SettingPropertyBool("Apply plans at OoB screen (soft weights only)", Order = 1, HintText = "Populate Order-of-Battle sliders as hints · never locks classes or sliders. Turn off to see pure vanilla OoB.")]
        public bool ApplyOoBWeights { get; set; } = true;

        // ---- Advanced -----------------------------------------------------------
        [SettingPropertyGroup("Advanced", GroupOrder = 9)]
        [SettingPropertyBool("Verbose debug logging", Order = 0, HintText = "Write extra detail to Configs/UnifiedTroopManager/log.txt. Slow · leave off unless debugging.")]
        public bool DebugLogging { get; set; } = false;
    }
}
