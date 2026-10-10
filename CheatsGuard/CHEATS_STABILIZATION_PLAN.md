# Cheats Stabilization Plan · Session-Ready 二分定位工作手册

**当前状态**（2026-09-26 session 结束）：
- Cheats v3.0.3.0 已装 · CheatsGuard v1.3 已装 · 二者 `IsSelected=true`
- `config.xml` 里 `SkipAllCheatsPatches=true` · Cheats patch 全短路 · **Campaign 可正常起**
- **待做**：二分定位 131 个"安装成功"的 patch 中崩游戏的元凶

**下 session 第一件事**：读本文档 · 编辑 config.xml → 启动 Round 1 → 反馈结果 → 我推进后续 round。

---

## 0. 数据 · 163 patch 完整清单

来源：`ilspycmd + reflection` 对 workshop `2882183897\bin\...\BannerlordCheats.dll`（详见 §53）。

### 32 个"必须 skip"（已确认签名 FAIL · Finalizer 已 catch · 加不加都不生效）

**分 5 组**：

**Workshops (6)** —— Workshop API 全变了
- `EveryoneBuysWorkshopsCost`
- `EveryoneBuysWorkshopsSelectOwner`
- `WorkshopBuyingCostPercentage`
- `WorkshopDailyExpensePercentage`
- `WorkshopSellingCostMultiplier`
- `WorkshopUpgradeCostPercentage`

**Smithing FAIL (1)**
- `CraftedWeaponModifierBonus` —— `OverrideData` 类型消失

**Settlements FAIL (2)** —— int → ExplainedNumber
- `DailyGarrisonBonus`
- `FreeTroopRecruitment`

**Party FAIL (1)**
- `FreeTroopUpgrades` —— int → ExplainedNumber

**Inventory FAIL (1)** —— 类型嵌套改
- `NativeItemSpawning`（CheatsGuard 已提供替代 `NativeItemSpawningReplacement`）

**Experience FAIL (4)** —— 大部分是 int → ExplainedNumber
- `CompanionLearningRateMultiplier`
- `LearningLimitMultiplier`
- `LearningRateMultiplier`
- `TroopExperienceMultiplier`

**Combat FAIL (17)** —— Ambiguous match / int → ExplainedNumber / method 消失
- `AlwaysWinBattleSimulation`（Ambiguous）
- `BanditHideoutTroopLimit`
- `DamageMultiplier_Sandbox`
- `DamageTakenPercentage_Sandbox`
- `EnemyDamagePercentage_Sandbox`
- `InfluenceRewardMultiplier`
- `NoFriendlyFire_Sandbox`
- `NoTroopSacrificeBreakIn`
- `NoTroopSacrificeBreakOut`
- `NoTroopSacrificeRunaway`
- `OneHitKill_Sandbox`
- `PartyDamageMultiplier_Sandbox`
- `PartyDamageTakenPercentage_Default`
- `PartyDamageTakenPercentage_Sandbox`
- `PartyOneHitKill_Sandbox`
- `RenownRewardMultiplierBattle`
- `SliceThroughEveryoneWeapon`

### 131 个"待测"（安装 OK · 其中若干在运行时崩）

**Army (3)**
- ArmyCohesionLossPercentage · ArmyFoodConsumptionPercentage · FactionArmyCohesionLossPercentage

**Characters (10)**
- AdjustPregnancyDuration · AllowSameSexMarriage · BarterOfferAlwaysAccepted · ConversationAlwaysSuccessful · NeverDieOfOldAge · NoBarterCooldown · PerfectAttraction · PerfectRelationships · PregnancyChanceMultiplier · RelationGainAfterBattleMultiplier

**Clan (3)**
- ExtraClanPartyLimit · ExtraClanPartySize · ExtraCompanionLimit

**Combat (50)** —— 67 总 − 17 FAIL
- AlwaysCrushThroughShields · AlwaysCrushThroughShields_Sandbox
- AlwaysKnockDown · AlwaysKnockDown_Sandbox
- CombatZoomMultiplier
- CompanionsKnockoutOrKilled · CompanionsKnockoutOrKilled_Default · CompanionsKnockoutOrKilled_Sandbox · CompanionsKnockoutOrKilled_StoryMode
- DamageMultiplier · DamageTakenPercentage
- EnemiesNoRunningAway · EnemyDamagePercentage
- EnemyLordsKnockoutOrKilled · EnemyLordsKnockoutOrKilled_Default · EnemyLordsKnockoutOrKilled_Sandbox · EnemyLordsKnockoutOrKilled_StoryMode
- EnemyTroopsKnockoutOrKilled · EnemyTroopsKnockoutOrKilled_Default · EnemyTroopsKnockoutOrKilled_Sandbox · EnemyTroopsKnockoutOrKilled_StoryMode
- FriendlyLordsKnockoutOrKilled · FriendlyLordsKnockoutOrKilled_Default · FriendlyLordsKnockoutOrKilled_Sandbox · FriendlyLordsKnockoutOrKilled_StoryMode
- HealthRegeneration · InfiniteAmmo · InstantCrossbowReload · Invincible
- NeverKnockedBackByAttacks · NeverKnockedBackByAttacks_Sandbox
- NoFriendlyFire · NoRunningAway · NoStuckArrows
- OneHitKill
- PartyDamageMultiplier · PartyDamageTakenPercentage
- PartyHealthRegeneration · PartyHeroesInvincible · PartyInfiniteAmmo · PartyInvincible
- PartyKnockoutOrKilled · PartyKnockoutOrKilled_Default · PartyKnockoutOrKilled_Sandbox · PartyKnockoutOrKilled_StoryMode
- PartyOneHitKill · PlayerHorseInvincible
- RenownRewardMultiplierTournament
- SliceThroughEveryonePassive · SliceThroughEveryonePassive_Sandbox

**Experience (4)** —— 8 总 − 4 FAIL
- ClanExperienceMultiplier · CompanionExperienceMultiplier · ExperienceMultiplier · FreeFocusPointAssignment

**General (13)** —— **HIGH SUSPECT ⚠**（native input hook）
- EnableHotkeysAddEncyclopediaTroops · EnableHotkeysAddItems · EnableHotkeysChangePlayerCharacter · EnableHotkeysCharacterAttributes · EnableHotkeysCharacterPoints · EnableHotkeysInfluence · EnableHotkeysKillCharacter · EnableHotkeysMoney · EnableHotkeysTransferSettlement · EnableHotkeysTroopCount · EnableHotkeysTroopExperience · EnableHotkeyTipsEncyclopedia · EnableHotkeyTipsScreens

**Inventory (1)** —— 2 总 − 1 FAIL
- ExtraInventoryCapacity

**Kingdom (6)**
- DecisionOverrideInfluenceCostPercentage · KingdomDecisionWeightMultiplier · NoCrimeRatingForCrimes · NoRelationshipLossOnDecisionArmyDisband · NoRelationshipLossOnDecisionExpellingClan · NoRelationshipLossOnDecisionKingdomDecision

**Map (5)**
- CaravansInvisibleOnMap · MapSpeedMultiplier · MapVisibilityMultiplier · NpcMapSpeedPercentage · PartyInvisibleOnMap

**Party (11)** —— 12 总 − 1 FAIL
- ExtraPartyMemberSize · ExtraPartyMorale · ExtraPartyPrisonerSize · FoodConsumptionPercentage · FreeCompanionHiring · InstantEscape · InstantPrisonerRecruitment · NoPrisonerEscape · PartyHealingMultiplierHeroes · PartyHealingMultiplierTroops · TroopWagesPercentage

**Settlements (17)** —— 19 总 − 2 FAIL
- ConstructionPowerMultiplier · DailyFoodBonus · DailyHearthsBonus · DailyLoyaltyBonus · DailyMilitiaBonusTown · DailyMilitiaBonusVillage · DailyProsperityBonus · DailySecurityBonus · DisguiseAlwaysWorks · GarrisonWagesPercentage · ItemTradingCostPercentage · NeverRequireCivilianEquipment · NoBribeToEnterKeep · RebellionChancePercentage · SellingPriceMultiplier · TournamentMaximumBetMultiplier · VillagesNeverRaided

**Sieges (2)**
- EnemySiegeBuildingSpeedPercentage · SiegeBuildingSpeedMultiplier

**Smithing (6)** —— 7 总 − 1 FAIL
- SmithingCostPercentage · SmithingDifficultyPercentage · SmithingEnergyCostPercentageRefining · SmithingEnergyCostPercentageSmelting · SmithingEnergyCostPercentageSmithing · UnlockAllParts

**总计 131 待测**：Army 3 + Characters 10 + Clan 3 + Combat 50 + Experience 4 + **General 13** + Inventory 1 + Kingdom 6 + Map 5 + Party 11 + Settlements 17 + Sieges 2 + Smithing 6 = **131** ✓

---

## 1. 二分策略 · CATEGORY BINARY CHOP

**思路**：按类别切 · 一次启用一批 · 稳则加白名单 · 崩则细分该批。

**每轮改动**：编辑 `Modules/CheatsGuard/ModuleData/config.xml` · 改 `SkipAllCheatsPatches` = false · 用 `<SkipClasses>` 列出要 skip 的（未启用的批次 + 32 个 always-skip）。

**决策依据**：Campaign 起 = 稳 · Campaign 崩 = 该批含元凶。

### 收敛路径

```
Round 1 (安全批)：启用 30 patches (Sieges + Map + Kingdom + Smithing + Inventory + Army + Clan + Experience)
  ↓ 稳
Round 2 (加中风险)：+38 patches (Characters + Party + Settlements)
  ↓ 稳
Round 3 (加 General)：+13 patches (General.EnableHotkey*)  ← 高嫌疑
  ↓ 稳/崩
    崩 → General 是元凶 · 二分 13 个 Hotkey (log_2 = 4 rounds)
    稳 → 转 Round 4
Round 4 (加 Combat)：+50 patches (全部 Combat non-FAIL)
  ↓ 稳/崩
    崩 → Combat 二分（log_2(50) = 6 rounds）
    稳 → 完成！全 131 patch 都安全（不太可能 · Round 4 必崩）
```

**预计**：Round 1-3 稳（~6 min gameplay 测试）+ Round 4 崩 · Combat 二分收敛（6 rounds · ~12 min）· **总 ~18 min gameplay** + config 编辑时间。

---

## 2. Round 1 · 安全批（copy-paste 就能用）

**替换 `Modules/CheatsGuard/ModuleData/config.xml` 的 `<SkipClasses>` 块为下方**（并把 `<SkipAllCheatsPatches>` 改回 `false`）：

```xml
<CheatsGuard>
  <VerboseLog>true</VerboseLog>
  <SkipAllCheatsPatches>false</SkipAllCheatsPatches>  <!-- ← 从 true 改回 false -->
  <EnableNativeItemSpawning>false</EnableNativeItemSpawning>
  <NativeItemSpawnCount>1</NativeItemSpawnCount>

  <SkipClasses>
    <!-- ============ 32 个 always-skip (已确认 FAIL) ============ -->
    <SkipClass>BannerlordCheats.Patches.Workshops.EveryoneBuysWorkshopsCost</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Workshops.EveryoneBuysWorkshopsSelectOwner</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Workshops.WorkshopBuyingCostPercentage</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Workshops.WorkshopDailyExpensePercentage</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Workshops.WorkshopSellingCostMultiplier</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Workshops.WorkshopUpgradeCostPercentage</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Smithing.CraftedWeaponModifierBonus</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.DailyGarrisonBonus</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.FreeTroopRecruitment</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Party.FreeTroopUpgrades</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Inventory.NativeItemSpawning</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Experience.CompanionLearningRateMultiplier</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Experience.LearningLimitMultiplier</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Experience.LearningRateMultiplier</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Experience.TroopExperienceMultiplier</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.AlwaysWinBattleSimulation</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.BanditHideoutTroopLimit</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.DamageMultiplier_Sandbox</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.DamageTakenPercentage_Sandbox</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.EnemyDamagePercentage_Sandbox</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.InfluenceRewardMultiplier</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.NoFriendlyFire_Sandbox</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.NoTroopSacrificeBreakIn</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.NoTroopSacrificeBreakOut</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.NoTroopSacrificeRunaway</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.OneHitKill_Sandbox</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.PartyDamageMultiplier_Sandbox</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.PartyDamageTakenPercentage_Default</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.PartyDamageTakenPercentage_Sandbox</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.PartyOneHitKill_Sandbox</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.RenownRewardMultiplierBattle</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.SliceThroughEveryoneWeapon</SkipClass>

    <!-- ============ Round 1: skip 101 待测（暂不启用） ============ -->
    <!-- Characters (10) - Round 2 才加 -->
    <SkipClass>BannerlordCheats.Patches.Characters.AdjustPregnancyDuration</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Characters.AllowSameSexMarriage</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Characters.BarterOfferAlwaysAccepted</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Characters.ConversationAlwaysSuccessful</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Characters.NeverDieOfOldAge</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Characters.NoBarterCooldown</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Characters.PerfectAttraction</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Characters.PerfectRelationships</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Characters.PregnancyChanceMultiplier</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Characters.RelationGainAfterBattleMultiplier</SkipClass>
    <!-- Party (11) - Round 2 -->
    <SkipClass>BannerlordCheats.Patches.Party.ExtraPartyMemberSize</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Party.ExtraPartyMorale</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Party.ExtraPartyPrisonerSize</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Party.FoodConsumptionPercentage</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Party.FreeCompanionHiring</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Party.InstantEscape</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Party.InstantPrisonerRecruitment</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Party.NoPrisonerEscape</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Party.PartyHealingMultiplierHeroes</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Party.PartyHealingMultiplierTroops</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Party.TroopWagesPercentage</SkipClass>
    <!-- Settlements (17) - Round 2 -->
    <SkipClass>BannerlordCheats.Patches.Settlements.ConstructionPowerMultiplier</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.DailyFoodBonus</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.DailyHearthsBonus</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.DailyLoyaltyBonus</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.DailyMilitiaBonusTown</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.DailyMilitiaBonusVillage</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.DailyProsperityBonus</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.DailySecurityBonus</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.DisguiseAlwaysWorks</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.GarrisonWagesPercentage</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.ItemTradingCostPercentage</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.NeverRequireCivilianEquipment</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.NoBribeToEnterKeep</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.RebellionChancePercentage</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.SellingPriceMultiplier</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.TournamentMaximumBetMultiplier</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Settlements.VillagesNeverRaided</SkipClass>
    <!-- General (13) - Round 3 -->
    <SkipClass>BannerlordCheats.Patches.General.EnableHotkeysAddEncyclopediaTroops</SkipClass>
    <SkipClass>BannerlordCheats.Patches.General.EnableHotkeysAddItems</SkipClass>
    <SkipClass>BannerlordCheats.Patches.General.EnableHotkeysChangePlayerCharacter</SkipClass>
    <SkipClass>BannerlordCheats.Patches.General.EnableHotkeysCharacterAttributes</SkipClass>
    <SkipClass>BannerlordCheats.Patches.General.EnableHotkeysCharacterPoints</SkipClass>
    <SkipClass>BannerlordCheats.Patches.General.EnableHotkeysInfluence</SkipClass>
    <SkipClass>BannerlordCheats.Patches.General.EnableHotkeysKillCharacter</SkipClass>
    <SkipClass>BannerlordCheats.Patches.General.EnableHotkeysMoney</SkipClass>
    <SkipClass>BannerlordCheats.Patches.General.EnableHotkeysTransferSettlement</SkipClass>
    <SkipClass>BannerlordCheats.Patches.General.EnableHotkeysTroopCount</SkipClass>
    <SkipClass>BannerlordCheats.Patches.General.EnableHotkeysTroopExperience</SkipClass>
    <SkipClass>BannerlordCheats.Patches.General.EnableHotkeyTipsEncyclopedia</SkipClass>
    <SkipClass>BannerlordCheats.Patches.General.EnableHotkeyTipsScreens</SkipClass>
    <!-- Combat non-FAIL (50) - Round 4 -->
    <SkipClass>BannerlordCheats.Patches.Combat.AlwaysCrushThroughShields</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.AlwaysCrushThroughShields_Sandbox</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.AlwaysKnockDown</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.AlwaysKnockDown_Sandbox</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.CombatZoomMultiplier</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.CompanionsKnockoutOrKilled</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.CompanionsKnockoutOrKilled_Default</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.CompanionsKnockoutOrKilled_Sandbox</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.CompanionsKnockoutOrKilled_StoryMode</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.DamageMultiplier</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.DamageTakenPercentage</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.EnemiesNoRunningAway</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.EnemyDamagePercentage</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.EnemyLordsKnockoutOrKilled</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.EnemyLordsKnockoutOrKilled_Default</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.EnemyLordsKnockoutOrKilled_Sandbox</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.EnemyLordsKnockoutOrKilled_StoryMode</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.EnemyTroopsKnockoutOrKilled</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.EnemyTroopsKnockoutOrKilled_Default</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.EnemyTroopsKnockoutOrKilled_Sandbox</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.EnemyTroopsKnockoutOrKilled_StoryMode</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.FriendlyLordsKnockoutOrKilled</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.FriendlyLordsKnockoutOrKilled_Default</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.FriendlyLordsKnockoutOrKilled_Sandbox</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.FriendlyLordsKnockoutOrKilled_StoryMode</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.HealthRegeneration</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.InfiniteAmmo</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.InstantCrossbowReload</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.Invincible</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.NeverKnockedBackByAttacks</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.NeverKnockedBackByAttacks_Sandbox</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.NoFriendlyFire</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.NoRunningAway</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.NoStuckArrows</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.OneHitKill</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.PartyDamageMultiplier</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.PartyDamageTakenPercentage</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.PartyHealthRegeneration</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.PartyHeroesInvincible</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.PartyInfiniteAmmo</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.PartyInvincible</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.PartyKnockoutOrKilled</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.PartyKnockoutOrKilled_Default</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.PartyKnockoutOrKilled_Sandbox</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.PartyKnockoutOrKilled_StoryMode</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.PartyOneHitKill</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.PlayerHorseInvincible</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.RenownRewardMultiplierTournament</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.SliceThroughEveryonePassive</SkipClass>
    <SkipClass>BannerlordCheats.Patches.Combat.SliceThroughEveryonePassive_Sandbox</SkipClass>
  </SkipClasses>
</CheatsGuard>
```

**启用中的 30 个（Round 1 白名单）**：
- Sieges (2)：EnemySiegeBuildingSpeedPercentage · SiegeBuildingSpeedMultiplier
- Map (5)：CaravansInvisibleOnMap · MapSpeedMultiplier · MapVisibilityMultiplier · NpcMapSpeedPercentage · PartyInvisibleOnMap
- Kingdom (6)：DecisionOverrideInfluenceCostPercentage · KingdomDecisionWeightMultiplier · NoCrimeRatingForCrimes · NoRelationshipLoss × 3
- Smithing (6)：SmithingCostPercentage · SmithingDifficultyPercentage · SmithingEnergyCost × 3 · UnlockAllParts
- Inventory (1)：ExtraInventoryCapacity
- Army (3)：ArmyCohesionLossPercentage · ArmyFoodConsumptionPercentage · FactionArmyCohesionLossPercentage
- Clan (3)：ExtraClanPartyLimit · ExtraClanPartySize · ExtraCompanionLimit
- Experience non-FAIL (4)：ClanExperienceMultiplier · CompanionExperienceMultiplier · ExperienceMultiplier · FreeFocusPointAssignment

**测试**：启动游戏 → 新 Campaign 或 load save → 玩 30 秒 · 若稳 → Round 1 通过 → Round 2；若崩 → 元凶在这 30 个 · 二分。

---

## 3. 后续 rounds 简述

**Round 2**（稳则加中风险 38 patches）：从 Round 1 的 SkipClasses 里**删除** Characters + Party + Settlements 三批（共 38 条）。启用总数变 68。

**Round 3**（稳则加 General 13）：再删 General 13 条。启用总数 81。这批是 HIGH SUSPECT。

**Round 4**（稳则加 Combat 50）：删 Combat non-FAIL 50 条。启用总数 131（全部）· 必崩概率大。

**若某 Round 崩**：把该批切一半 · 再测。递归到具体 patch。

**收敛后**：确定元凶列表 · 归档到 `CheatsGuard/README.md` "Final Skip List" 章节 · config.xml 里 SkipClasses 就是最终列表。

---

## 4. 决策树 · 每 Round 后做什么

```
Round X 结果
├─ Campaign 起 → 该批全安全 → 白名单已扩大 → 转 Round X+1（加下一批）
└─ Campaign 崩 → 该批含元凶 → 二分该批：
    ├─ 该批切上下半 · 上半启用 · 下半 skip · 再测
    │   ├─ 稳 → 元凶在下半 · 切下半 · 再测
    │   └─ 崩 → 元凶在上半 · 切上半 · 再测
    └─ 递归到单个 patch → 加入永久 skip list
       → 该批剩余 patch 都启用 · 转下一 batch
```

**Debug 辅助**：每次崩后读 `Modules/CheatsGuard/Logs/cheats_guard.log`：
- 最后一条 `OK X` 表明 X 及之前都装成功了
- 若 log 有 `FAIL X` · X 是安装失败（Finalizer 抓住）· 不会是运行时崩因
- 崩因 = 已 OK 且不在当前 SkipList 的某个（几个）patch

---

## 5. 完成后的归档步骤

1. **最终 config.xml**：SkipClasses 里包含 32 always-skip + N 运行时崩患
2. **`CheatsGuard/README.md` 加"Confirmed Bad Patches" 章节**：记录发现的元凶 + 每个的失败模式（若已知）
3. **`ModdingJournal.md` §53 补充结论**：从"待二分"改"已收官"· 列最终 skip list · 记录耗时
4. **测试可用 cheats**：进游戏 MCM 面板 · 逐类别试各 cheat · 确认没被 skip 的功能都能用
5. **`CheatsGuard/CHEATS_STABILIZATION_PLAN.md` 归档**：保留供未来 Cheats 更新后回归测试

---

## 6. 中止条件 · 何时放弃 Cheats

若二分过程中发现：
- **超过 30% patch 都崩** → Cheats 本体与 1.4.7 兼容性太差 · 考虑放弃 · 转 Nexus "Cheats for Bannerlord" (mod 12878 · 明确 1.4.7 支持)
- **崩因是 Cheats 与其他 mod (RBM/Retinues) 冲突** → 二分只能确定"启用哪些 cheats 兼容当前 stack" · 不能修 Cheats 本身
- **8 rounds 后还没收敛** → 说明多个 patch 崩 · 组合爆炸 · 考虑放弃

**放弃时的替代路径**：
- PFRB 已解你最痛的招兵瓶颈
- Nexus Cheats for Bannerlord (12878) · Ultimate Cheat Menu (12829) 等替代
- 或者接受"Cheats 仅装 30 安全 patch"的最小可用状态

---

## 7. 下 session 首 30 秒操作

1. **打开** `Modules/CheatsGuard/ModuleData/config.xml`
2. **`SkipAllCheatsPatches` 改 `false`**
3. **`<SkipClasses>` 替换为 §2 里的 Round 1 完整块**（copy-paste）
4. **保存 · 启动游戏 · 进 Campaign · 玩 30 秒**
5. **反馈崩没崩** · 我推进 Round 2 / 二分

**若稳** → 恭喜，Round 1 通过 · 走 Round 2  
**若崩** → 元凶在 30 个"安全"里（意外）· 我发 Round 1a / 1b 二分  

---

**版本**：v1.0 · 2026-09-26 首版 · 基于 CheatsGuard v1.3 能力设计
