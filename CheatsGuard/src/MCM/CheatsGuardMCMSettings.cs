using System.ComponentModel;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace CheatsGuard.MCM
{
    // MCM v5 attribute-based settings for CheatsGuard.
    //
    // Groups:
    //   0. Money Hotkeys — Ctrl+X / Ctrl+Shift+X amounts + tip toggle
    //   1. RBM Troop Maintenance (player only) — 3 fields, scoped via
    //      MaintenancePlayerScopePatch, NOT written to global RBMConfig
    //   2. RBM: Combat & Battle AI     ] every property carrying [RbmField(name)]
    //   3. RBM: Troop Economy          ]  is auto-synced to the matching
    //   4. RBM: Kingdom & Settlements  ]  RBMConfig.RBMConfig public static field
    //   5. RBM: Auto-Battle Simulation ]  each time MCM raises PropertyChanged,
    //   6. RBM: Debug & Global Toggles ]  via reflection in SubModule.SyncFromMcm.
    //
    // Adding a new mirror: add one property here + [RbmField("rbmFieldName")]. Nothing
    // else to wire up. Boolean master toggles typically need a game restart because
    // RBM's Harmony patches are installed at module load; numeric fields apply at the
    // next tick.

    public sealed class CheatsGuardMCMSettings : AttributeGlobalSettings<CheatsGuardMCMSettings>
    {
        public override string Id => "CheatsGuard_v1";
        public override string DisplayName => "CheatsGuard";
        public override string FolderName => "CheatsGuard";
        public override string FormatType => "json2";

        // ---- Money Hotkeys ----------------------------------------------------
        [SettingPropertyGroup("Money Hotkeys", GroupOrder = 0)]
        [SettingPropertyBool("Enable Money Hotkey", Order = 0, HintText = "Master switch for Ctrl+X / Ctrl+Shift+X money hotkeys (inventory UI only).")]
        public bool EnableMoneyHotkey { get; set; } = true;

        [SettingPropertyGroup("Money Hotkeys", GroupOrder = 0)]
        [SettingPropertyBool("Show tip on inventory open", Order = 1, HintText = "Flash a 3-line yellow reminder every time the inventory screen opens.")]
        public bool ShowMoneyHotkeyTip { get; set; } = true;

        [SettingPropertyGroup("Money Hotkeys", GroupOrder = 0)]
        [SettingPropertyInteger("Ctrl+X amount", 0, 1_000_000_000, "N0", Order = 2, HintText = "Denars added when pressing Ctrl+X in the inventory screen.")]
        public int MoneySmall { get; set; } = 10_000_000;

        [SettingPropertyGroup("Money Hotkeys", GroupOrder = 0)]
        [SettingPropertyInteger("Ctrl+Shift+X amount", 0, 2_000_000_000, "N0", Order = 3, HintText = "Denars added when pressing Ctrl+Shift+X in the inventory screen.")]
        public int MoneyLarge { get; set; } = 100_000_000;

        // ---- Passive Daily XP (folded into Money group for proximity to other player buffs) ----
        [SettingPropertyGroup("Money Hotkeys", GroupOrder = 0)]
        [SettingPropertyBool("Passive XP: enable", Order = 4, HintText = "Every day, grant XP to eligible heroes in the player party (per skill).")]
        public bool EnablePassiveDailyXp { get; set; } = true;

        [SettingPropertyGroup("Money Hotkeys", GroupOrder = 0)]
        [SettingPropertyInteger("Passive XP: amount per skill per day", 0, 100_000, "N0", Order = 5, HintText = "Applied once per skill per eligible hero, per daily tick. Vanilla has 18 skills, so 1000 here = 18000 total per hero per day.")]
        public int PassiveDailyXpAmount { get; set; } = 1000;

        [SettingPropertyGroup("Money Hotkeys", GroupOrder = 0)]
        [SettingPropertyBool("Passive XP: include companions in main party", Order = 6, HintText = "When on, every hero riding with you also gets the daily XP. When off, only the player character.")]
        public bool PassiveDailyXpIncludeCompanions { get; set; } = true;

        // ---- RBM Troop Maintenance (player clan only) ------------------------
        // These sliders only affect the player clan's maintenance charge. Every other
        // clan keeps whatever RBM loaded from its own config.xml. See
        // ReplacementPatches/MaintenancePlayerScopePatch.cs for the swap logic.
        [SettingPropertyGroup("RBM Troop Maintenance (player only)", GroupOrder = 1)]
        [SettingPropertyFloatingInteger("Troop maintenance fraction", 0.0f, 0.05f, "0.0000", Order = 0, HintText = "Player-only daily gold cost per troop wage. RBM default 0.005 (0.5%). Set 0 to disable for the player.")]
        public float TroopMaintenanceFraction { get; set; } = 0.005f;

        [SettingPropertyGroup("RBM Troop Maintenance (player only)", GroupOrder = 1)]
        [SettingPropertyInteger("Recruit free days", 0, 365, "0", Order = 1, HintText = "Player-only: new recruits pay no maintenance for this many days. RBM default 20.")]
        public int RecruitMaintenanceDays { get; set; } = 20;

        [SettingPropertyGroup("RBM Troop Maintenance (player only)", GroupOrder = 1)]
        [SettingPropertyFloatingInteger("Independent purse fraction", 0.0f, 2.0f, "0.00", Order = 2, HintText = "Player-only: fraction of maintenance paid from independent party purses. RBM default 1.0.")]
        public float IndependentPurseFraction { get; set; } = 1.0f;

        // ---- RBM mirror properties (generated) --------------------------
        // Auto-synced to RBMConfig.RBMConfig static fields via reflection every time
        // PropertyChanged fires (see SubModule.SyncFromMcm). MCM label shown in-panel.

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyBool("Hit stop", Order = 0, HintText = "Vanilla-like on-hit pause. Toggle if you want a fluid vs punchy feel.")]
        [RbmField("hitStopEnabled")]
        public bool HitStopEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyBool("Posture system", Order = 1, HintText = "Melee posture (stagger on high load). Disable for classic feel.")]
        [RbmField("postureEnabled")]
        public bool PostureEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyBool("Stamina system", Order = 2, HintText = "Stamina tracking on melee agents. May need restart.")]
        [RbmField("staminaEnabled")]
        public bool StaminaEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyFloatingInteger("Player posture multiplier", 0f, 5f, "0.00", Order = 3, HintText = "Scales the posture pool of the player only. 1 = default.")]
        [RbmField("playerPostureMultiplier")]
        public float PlayerPostureMultiplier { get; set; } = 1.0f;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyBool("Posture GUI", Order = 4, HintText = "Show posture bar HUD.")]
        [RbmField("postureGUIEnabled")]
        public bool PostureGUIEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyBool("Use vanilla combat AI", Order = 5, HintText = "Bypass RBM AI overhaul and use TW's. Restart required.")]
        [RbmField("vanillaCombatAi")]
        public bool VanillaCombatAi { get; set; } = false;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyBool("Keep-battle after auto-resolve", Order = 6, HintText = "Continue watching after auto-resolve triggers.")]
        [RbmField("keepBattleEnabled")]
        public bool KeepBattleEnabled { get; set; } = false;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyBool("Frontline AI", Order = 7, HintText = "Enable RBM's frontline formation AI. Restart may be required.")]
        [RbmField("frontlineEnabled")]
        public bool FrontlineEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyInteger("Frontline min formation size", 0, 1000, "0", Order = 8, HintText = "Formations smaller than this skip frontline logic.")]
        [RbmField("frontlineMinFormationSize")]
        public int FrontlineMinFormationSize { get; set; } = 25;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyFloatingInteger("Frontline decision timer (s)", 0f, 60f, "0.00", Order = 9, HintText = "Seconds between AI frontline decisions.")]
        [RbmField("frontlineDecisionTimerMax")]
        public float FrontlineDecisionTimerMax { get; set; } = 2.0f;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyFloatingInteger("Frontline attack weight", 0f, 5f, "0.00", Order = 10, HintText = "Weight for attack decisions in frontline AI.")]
        [RbmField("frontlineAttackWeight")]
        public float FrontlineAttackWeight { get; set; } = 1.0f;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyFloatingInteger("Frontline back-step weight", 0f, 5f, "0.00", Order = 11, HintText = "Weight for back-step decisions.")]
        [RbmField("frontlineBackStepWeight")]
        public float FrontlineBackStepWeight { get; set; } = 1.0f;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyFloatingInteger("Frontline find-ally weight", 0f, 5f, "0.00", Order = 12, HintText = "Weight for allying-up decisions.")]
        [RbmField("frontlineFindAllyWeight")]
        public float FrontlineFindAllyWeight { get; set; } = 1.0f;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyFloatingInteger("Frontline flank weight", 0f, 5f, "0.00", Order = 13, HintText = "Weight for flanking decisions.")]
        [RbmField("frontlineFlankWeight")]
        public float FrontlineFlankWeight { get; set; } = 1.0f;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyBool("Realistic arrow arc", Order = 14, HintText = "Steeper physics-based arrow arcs. Slightly harder to hit.")]
        [RbmField("realisticArrowArc")]
        public bool RealisticArrowArc { get; set; } = false;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyBool("Armor status UI", Order = 15, HintText = "Show live armor rating in HUD.")]
        [RbmField("armorStatusUIEnabled")]
        public bool ArmorStatusUIEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyFloatingInteger("Armor multiplier", 0f, 5f, "0.00", Order = 16, HintText = "Global multiplier on armor values. RBM default 2.0 (double vanilla).")]
        [RbmField("armorMultiplier")]
        public float ArmorMultiplier { get; set; } = 2.0f;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyBool("Armor penetration messages", Order = 17, HintText = "Print AP hit messages to combat log.")]
        [RbmField("armorPenetrationMessage")]
        public bool ArmorPenetrationMessage { get; set; } = false;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyBool("Better arrow visuals", Order = 18, HintText = "Improved arrow rendering.")]
        [RbmField("betterArrowVisuals")]
        public bool BetterArrowVisuals { get; set; } = true;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyBool("Passive shoulder shields", Order = 19, HintText = "Shields on the back partially block hits.")]
        [RbmField("passiveShoulderShields")]
        public bool PassiveShoulderShields { get; set; } = false;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyFloatingInteger("Mace blunt modifier", 0f, 5f, "0.00", Order = 20, HintText = "Extra blunt damage multiplier for mace-class weapons.")]
        [RbmField("maceBluntModifier")]
        public float MaceBluntModifier { get; set; } = 1.0f;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyFloatingInteger("Armor threshold modifier", 0f, 5f, "0.00", Order = 21, HintText = "Adjusts the armor value at which damage begins to fall off.")]
        [RbmField("armorThresholdModifier")]
        public float ArmorThresholdModifier { get; set; } = 1.0f;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyFloatingInteger("Blunt trauma bonus", 0f, 5f, "0.00", Order = 22, HintText = "Additive bonus to blunt trauma checks.")]
        [RbmField("bluntTraumaBonus")]
        public float BluntTraumaBonus { get; set; } = 0.0f;

        [SettingPropertyGroup("RBM: Combat & Battle AI", GroupOrder = 2)]
        [SettingPropertyBool("Sneak-attack insta-kill", Order = 23, HintText = "Backstabs on unaware targets are lethal.")]
        [RbmField("sneakAttackInstaKill")]
        public bool SneakAttackInstaKill { get; set; } = false;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyBool("Troop overhaul active", Order = 0, HintText = "RBM's replacement troop tree hooks. Restart required.")]
        [RbmField("troopOverhaulActive")]
        public bool TroopOverhaulActive { get; set; } = true;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyFloatingInteger("Troop upgrade cost multiplier", 0f, 5f, "0.00", Order = 1, HintText = "Scales gold cost of every troop upgrade.")]
        [RbmField("troopUpgradeCostMultiplier")]
        public float TroopUpgradeCostMultiplier { get; set; } = 1.0f;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyFloatingInteger("Upgrade spoils loot multiplier", 0f, 5f, "0.00", Order = 2, HintText = "Scales spoils drawn from party pool on upgrade.")]
        [RbmField("troopUpgradeSpoilsLootMultiplier")]
        public float TroopUpgradeSpoilsLootMultiplier { get; set; } = 1.0f;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyBool("Upgrade requires supply town", Order = 3, HintText = "Must be near a supply town to upgrade.")]
        [RbmField("troopUpgradeRequireSupplyTown")]
        public bool TroopUpgradeRequireSupplyTown { get; set; } = true;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyBool("Recruits draw from settlement stock", Order = 4, HintText = "Recruits reduce settlement stockpile.")]
        [RbmField("recruitDrawsFromSettlementStock")]
        public bool RecruitDrawsFromSettlementStock { get; set; } = true;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyFloatingInteger("Upgrade supply radius (map units)", 0f, 200f, "0.00", Order = 5, HintText = "Distance to nearest supply town that still counts.")]
        [RbmField("troopUpgradeSupplyRadius")]
        public float TroopUpgradeSupplyRadius { get; set; } = 30.0f;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyBool("Charge mount value on upgrade", Order = 6, HintText = "Include mount cost when upgrading mounted units.")]
        [RbmField("troopUpgradeChargeMountValue")]
        public bool TroopUpgradeChargeMountValue { get; set; } = true;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyInteger("Loot pieces per man", 0, 50, "0", Order = 7, HintText = "Number of loot rolls generated per fallen enemy.")]
        [RbmField("troopLootPiecesPerMan")]
        public int TroopLootPiecesPerMan { get; set; } = 3;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyFloatingInteger("Loot overlook chance per tier", 0f, 2f, "0.00", Order = 8, HintText = "Chance to miss loot, scaled per troop tier.")]
        [RbmField("troopLootOverlookChancePerTier")]
        public float TroopLootOverlookChancePerTier { get; set; } = 0.5f;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyInteger("Settlement food days", 0, 365, "0", Order = 9, HintText = "How long settlement stock feeds a garrison.")]
        [RbmField("troopSettlementFoodDays")]
        public int TroopSettlementFoodDays { get; set; } = 20;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyFloatingInteger("Food wage fraction", 0f, 2f, "0.00", Order = 10, HintText = "Fraction of wage that goes to food.")]
        [RbmField("troopFoodWageFraction")]
        public float TroopFoodWageFraction { get; set; } = 0.5f;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyFloatingInteger("Settlement fun wage fraction", 0f, 2f, "0.00", Order = 11, HintText = "Fraction of wage troops spend on downtime luxuries.")]
        [RbmField("troopSettlementFunWageFraction")]
        public float TroopSettlementFunWageFraction { get; set; } = 0.25f;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyFloatingInteger("Raid spoils multiplier", 0f, 5f, "0.00", Order = 12, HintText = "Scales spoils gained from village raids.")]
        [RbmField("troopRaidSpoilsMultiplier")]
        public float TroopRaidSpoilsMultiplier { get; set; } = 0.25f;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyFloatingInteger("Leader spoils cut fraction", 0f, 2f, "0.00", Order = 13, HintText = "Fraction of spoils skimmed off the top for the leader.")]
        [RbmField("troopLeaderSpoilsCutFraction")]
        public float TroopLeaderSpoilsCutFraction { get; set; } = 0.05f;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyInteger("Spoils cap days", 0, 365, "0", Order = 14, HintText = "Max days of accumulated spoils per troop.")]
        [RbmField("troopSpoilsCapDays")]
        public int TroopSpoilsCapDays { get; set; } = 20;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyInteger("Spoils heal gold per tier", 0, 1000, "0", Order = 15, HintText = "Gold spent per tier when healing wounded via spoils.")]
        [RbmField("troopSpoilsHealGoldPerTier")]
        public int TroopSpoilsHealGoldPerTier { get; set; } = 10;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyFloatingInteger("Spoils heal fraction per hour", 0f, 2f, "0.00", Order = 16, HintText = "How fast wounded recover via spoils each hour.")]
        [RbmField("troopSpoilsHealFractionPerHour")]
        public float TroopSpoilsHealFractionPerHour { get; set; } = 0.05f;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyFloatingInteger("Fallen spoils capture fraction", 0f, 2f, "0.00", Order = 17, HintText = "How much of a slain troop's spoils the victor captures.")]
        [RbmField("troopFallenSpoilsCaptureFraction")]
        public float TroopFallenSpoilsCaptureFraction { get; set; } = 0.75f;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyInteger("Luxury cooldown days", 0, 365, "0", Order = 18, HintText = "Days between luxury spend attempts per troop.")]
        [RbmField("troopLuxuryCooldownDays")]
        public int TroopLuxuryCooldownDays { get; set; } = 20;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyFloatingInteger("Luxury spend chance", 0f, 2f, "0.00", Order = 19, HintText = "Per-hour chance a troop spends on luxuries.")]
        [RbmField("troopLuxurySpendChance")]
        public float TroopLuxurySpendChance { get; set; } = 0.02f;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyFloatingInteger("Steward XP per spoils", 0f, 10f, "0.00", Order = 20, HintText = "XP dripped to Steward skill per spoils transaction.")]
        [RbmField("stewardProvisioningXpPerSpoils")]
        public float StewardProvisioningXpPerSpoils { get; set; } = 0.05f;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyFloatingInteger("Steward XP per food reserve tier", 0f, 10f, "0.00", Order = 21, HintText = "XP granted per tier of food reserve maintained.")]
        [RbmField("stewardFoodReserveXpPerTier")]
        public float StewardFoodReserveXpPerTier { get; set; } = 3.0f;

        [SettingPropertyGroup("RBM: Troop Economy", GroupOrder = 3)]
        [SettingPropertyFloatingInteger("Steward XP per horse", 0f, 10f, "0.00", Order = 22, HintText = "XP for stabling / maintaining a horse.")]
        [RbmField("stewardMountedFootmanXpPerHorse")]
        public float StewardMountedFootmanXpPerHorse { get; set; } = 0.25f;

        [SettingPropertyGroup("RBM: Kingdom & Settlements", GroupOrder = 4)]
        [SettingPropertyInteger("Building cost multiplier", 0, 2000, "0", Order = 0, HintText = "Scales building costs in settlements (unit: percent-like scalar).")]
        [RbmField("buildingCostMultiplier")]
        public int BuildingCostMultiplier { get; set; } = 250;

        [SettingPropertyGroup("RBM: Kingdom & Settlements", GroupOrder = 4)]
        [SettingPropertyFloatingInteger("Construction budget share", 0f, 2f, "0.00", Order = 1, HintText = "Share of settlement gold allocated to construction each day.")]
        [RbmField("constructionBudgetShare")]
        public float ConstructionBudgetShare { get; set; } = 0.01f;

        [SettingPropertyGroup("RBM: Kingdom & Settlements", GroupOrder = 4)]
        [SettingPropertyInteger("Campaign starting gold", 0, 1000000, "0", Order = 2, HintText = "Starting gold on new campaign. Set high for cheat-start.")]
        [RbmField("campaignStartingGold")]
        public int CampaignStartingGold { get; set; } = 5000;

        [SettingPropertyGroup("RBM: Kingdom & Settlements", GroupOrder = 4)]
        [SettingPropertyBool("Days-of-supply AI signal", Order = 3, HintText = "RBM tells AI parties to worry about supply days.")]
        [RbmField("rbmDaysOfSupplyAiSignal")]
        public bool RbmDaysOfSupplyAiSignal { get; set; } = true;

        [SettingPropertyGroup("RBM: Kingdom & Settlements", GroupOrder = 4)]
        [SettingPropertyBool("Workshop headroom gate", Order = 4, HintText = "Restrict workshop opening by clan tier headroom.")]
        [RbmField("workshopHeadroomGateEnabled")]
        public bool WorkshopHeadroomGateEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Kingdom & Settlements", GroupOrder = 4)]
        [SettingPropertyFloatingInteger("Village production multiplier", 0f, 5f, "0.00", Order = 5, HintText = "Scales village production output.")]
        [RbmField("villageProductionMultiplier")]
        public float VillageProductionMultiplier { get; set; } = 0.5f;

        [SettingPropertyGroup("RBM: Kingdom & Settlements", GroupOrder = 4)]
        [SettingPropertyFloatingInteger("Workshop production multiplier", 0f, 5f, "0.00", Order = 6, HintText = "Scales workshop production output.")]
        [RbmField("workshopProductionMultiplier")]
        public float WorkshopProductionMultiplier { get; set; } = 1.0f;

        [SettingPropertyGroup("RBM: Kingdom & Settlements", GroupOrder = 4)]
        [SettingPropertyBool("Kingdom caravans", Order = 7, HintText = "Enable RBM's kingdom caravan behaviour.")]
        [RbmField("kingdomCaravansEnabled")]
        public bool KingdomCaravansEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Kingdom & Settlements", GroupOrder = 4)]
        [SettingPropertyBool("Caravan investment", Order = 8, HintText = "Enable player-side caravan investment mechanic.")]
        [RbmField("caravanInvestmentEnabled")]
        public bool CaravanInvestmentEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Kingdom & Settlements", GroupOrder = 4)]
        [SettingPropertyBool("Deserter raiders", Order = 9, HintText = "Enable escaped-deserter bandit spawning.")]
        [RbmField("deserterRaidersEnabled")]
        public bool DeserterRaidersEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyBool("Simulation: equipment", Order = 0, HintText = "Use equipment in auto-battle sim. Restart required.")]
        [RbmField("simulationEquipmentEnabled")]
        public bool SimulationEquipmentEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyFloatingInteger("Simulation: equipment power weight", 0f, 5f, "0.00", Order = 1, HintText = "How much equipment power factors into sim.")]
        [RbmField("simulationEquipmentPowerWeight")]
        public float SimulationEquipmentPowerWeight { get; set; } = 1.0f;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyBool("Strategic power", Order = 2, HintText = "Enable RBM's strategic power layer.")]
        [RbmField("strategicPowerEnabled")]
        public bool StrategicPowerEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyBool("Absolute-damage mode", Order = 3, HintText = "Use RBM's absolute damage model in sim.")]
        [RbmField("simulationAbsoluteDamage")]
        public bool SimulationAbsoluteDamage { get; set; } = true;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyFloatingInteger("Absolute damage scale", 0f, 5f, "0.00", Order = 4, HintText = "Scale factor when absolute-damage mode is on.")]
        [RbmField("simulationAbsoluteScale")]
        public float SimulationAbsoluteScale { get; set; } = 1.0f;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyFloatingInteger("Absolute-damage blow cap", 0f, 10f, "0.00", Order = 5, HintText = "Max per-blow damage cap under absolute mode.")]
        [RbmField("simulationAbsoluteBlowCap")]
        public float SimulationAbsoluteBlowCap { get; set; } = 1.5f;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyFloatingInteger("Shield block chance", 0f, 2f, "0.00", Order = 6, HintText = "Chance a shield blocks a sim hit.")]
        [RbmField("simulationShieldBlockChance")]
        public float SimulationShieldBlockChance { get; set; } = 0.4f;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyBool("Defense system", Order = 7, HintText = "Enable defensive rolls in sim.")]
        [RbmField("simulationDefenseSystem")]
        public bool SimulationDefenseSystem { get; set; } = true;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyBool("Arm targeting", Order = 8, HintText = "Enable arm-hit resolution in sim.")]
        [RbmField("simulationArmTargeting")]
        public bool SimulationArmTargeting { get; set; } = true;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyBool("Ranged miss", Order = 9, HintText = "Enable ranged-miss checks in sim.")]
        [RbmField("simulationRangedMissEnabled")]
        public bool SimulationRangedMissEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyFloatingInteger("Ranged miss chance", 0f, 2f, "0.00", Order = 10, HintText = "Base ranged miss chance.")]
        [RbmField("simulationRangedMissChance")]
        public float SimulationRangedMissChance { get; set; } = 0.35f;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyFloatingInteger("Round minutes", 0f, 60f, "0.00", Order = 11, HintText = "Minutes of game-time per sim round.")]
        [RbmField("simulationRoundMinutes")]
        public float SimulationRoundMinutes { get; set; } = 10.0f;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyBool("Siege defender advantage", Order = 12, HintText = "Give siege defenders extra bonus.")]
        [RbmField("simulationSiegeDefenderEnabled")]
        public bool SimulationSiegeDefenderEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyFloatingInteger("Siege defender bonus", 0f, 5f, "0.00", Order = 13, HintText = "Multiplier for defender advantage in sieges.")]
        [RbmField("simulationSiegeDefenderDefenseBonus")]
        public float SimulationSiegeDefenderDefenseBonus { get; set; } = 1.3f;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyFloatingInteger("Siege ranged miss skew", 0f, 5f, "0.00", Order = 14, HintText = "Skew multiplier on ranged miss in siege sim.")]
        [RbmField("simulationSiegeRangedMissSkew")]
        public float SimulationSiegeRangedMissSkew { get; set; } = 1.4f;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyBool("Rout mechanic", Order = 15, HintText = "Enable morale rout in sim.")]
        [RbmField("simulationRoutEnabled")]
        public bool SimulationRoutEnabled { get; set; } = false;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyBool("Perk system", Order = 16, HintText = "Include perks in sim resolution.")]
        [RbmField("simulationPerkSystem")]
        public bool SimulationPerkSystem { get; set; } = true;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyBool("Spectate battles", Order = 17, HintText = "Allow spectating auto-resolved battles.")]
        [RbmField("spectateBattlesEnabled")]
        public bool SpectateBattlesEnabled { get; set; } = false;

        [SettingPropertyGroup("RBM: Auto-Battle Simulation", GroupOrder = 5)]
        [SettingPropertyInteger("Spectate min troops per side", 0, 1000, "0", Order = 18, HintText = "Skip spectate view under this size.")]
        [RbmField("spectateMinTroopsPerSide")]
        public int SpectateMinTroopsPerSide { get; set; } = 100;

        [SettingPropertyGroup("RBM: Debug & Global Toggles", GroupOrder = 6)]
        [SettingPropertyBool("RBM Tournament module", Order = 0, HintText = "Master toggle for RBMTournament. Restart required.")]
        [RbmField("rbmTournamentEnabled")]
        public bool RbmTournamentEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Debug & Global Toggles", GroupOrder = 6)]
        [SettingPropertyBool("RBM AI module", Order = 1, HintText = "Master toggle for RBMAI. Restart required.")]
        [RbmField("rbmAiEnabled")]
        public bool RbmAiEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Debug & Global Toggles", GroupOrder = 6)]
        [SettingPropertyBool("RBM Combat module", Order = 2, HintText = "Master toggle for RBMCombat. Restart required.")]
        [RbmField("rbmCombatEnabled")]
        public bool RbmCombatEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Debug & Global Toggles", GroupOrder = 6)]
        [SettingPropertyBool("RBM Campaign module", Order = 3, HintText = "Master toggle for RBMCampaign. Restart required.")]
        [RbmField("rbmCampaignEnabled")]
        public bool RbmCampaignEnabled { get; set; } = true;

        [SettingPropertyGroup("RBM: Debug & Global Toggles", GroupOrder = 6)]
        [SettingPropertyBool("Developer mode", Order = 4, HintText = "Extra dev-only options in RBM.")]
        [RbmField("developerMode")]
        public bool DeveloperMode { get; set; } = false;

        [SettingPropertyGroup("RBM: Debug & Global Toggles", GroupOrder = 6)]
        [SettingPropertyBool("Show inventory item weight", Order = 5, HintText = "Display item weight in inventory tooltips.")]
        [RbmField("showInventoryItemWeight")]
        public bool ShowInventoryItemWeight { get; set; } = true;

        [SettingPropertyGroup("RBM: Debug & Global Toggles", GroupOrder = 6)]
        [SettingPropertyBool("Log: AI behaviour", Order = 6, HintText = "Verbose AI behaviour logging.")]
        [RbmField("aiBehaviorLogEnabled")]
        public bool AiBehaviorLogEnabled { get; set; } = false;

        [SettingPropertyGroup("RBM: Debug & Global Toggles", GroupOrder = 6)]
        [SettingPropertyBool("Log: spoils", Order = 7, HintText = "Log spoils transactions.")]
        [RbmField("spoilsLoggingEnabled")]
        public bool SpoilsLoggingEnabled { get; set; } = false;

        [SettingPropertyGroup("RBM: Debug & Global Toggles", GroupOrder = 6)]
        [SettingPropertyBool("Log: spoils (verbose)", Order = 8, HintText = "Extra-verbose spoils logging.")]
        [RbmField("spoilsVerboseLoggingEnabled")]
        public bool SpoilsVerboseLoggingEnabled { get; set; } = false;

        [SettingPropertyGroup("RBM: Debug & Global Toggles", GroupOrder = 6)]
        [SettingPropertyBool("Log: economy", Order = 9, HintText = "Log economy events.")]
        [RbmField("economyLoggingEnabled")]
        public bool EconomyLoggingEnabled { get; set; } = false;

        [SettingPropertyGroup("RBM: Debug & Global Toggles", GroupOrder = 6)]
        [SettingPropertyBool("Log: caravans", Order = 10, HintText = "Log caravan behaviour.")]
        [RbmField("caravanLoggingEnabled")]
        public bool CaravanLoggingEnabled { get; set; } = false;

        [SettingPropertyGroup("RBM: Debug & Global Toggles", GroupOrder = 6)]
        [SettingPropertyBool("Log: strategic power", Order = 11, HintText = "Log strategic power computations.")]
        [RbmField("strategicPowerLoggingEnabled")]
        public bool StrategicPowerLoggingEnabled { get; set; } = false;

        [SettingPropertyGroup("RBM: Debug & Global Toggles", GroupOrder = 6)]
        [SettingPropertyBool("Log: simulation", Order = 12, HintText = "Log auto-battle sim.")]
        [RbmField("simulationLoggingEnabled")]
        public bool SimulationLoggingEnabled { get; set; } = false;

        [SettingPropertyGroup("RBM: Debug & Global Toggles", GroupOrder = 6)]
        [SettingPropertyBool("Log: sim hits", Order = 13, HintText = "Log each hit in auto-battle sim.")]
        [RbmField("simulationLogHits")]
        public bool SimulationLogHits { get; set; } = false;

        [SettingPropertyGroup("RBM: Debug & Global Toggles", GroupOrder = 6)]
        [SettingPropertyBool("Log: battle hits", Order = 14, HintText = "Log live battle hit resolution.")]
        [RbmField("battleHitLoggingEnabled")]
        public bool BattleHitLoggingEnabled { get; set; } = false;
    }
}
