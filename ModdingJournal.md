# Bannerlord 骑马与砍杀2 模组开发日志

**最后更新**：2026-09-18

## 目录
- [环境与路径](#环境与路径)
- [模组清单（最终状态）](#模组清单最终状态)
- [已做的定制修改](#已做的定制修改)
- [Bug 历史与修复](#bug-历史与修复)
- [模组间交互与已知风险](#模组间交互与已知风险)
- [备份与回滚](#备份与回滚)
- [待办 / 开放问题](#待办--开放问题)
- [在另一台设备上复刻本套配置](#在另一台设备上复刻本套配置)
- [调试参考](#调试参考如何找信息)

**相关文档**：`TroopDesignReference.md`（同目录，T1-T7 兵种技能模板与装备指南）

---

## 环境与路径

| 项 | 值 |
|---|---|
| 游戏本体版本 | Native v1.4.7 (build 117484) |
| Steam beta 分支 | `v1.4.7`（`appmanifest_261550.acf` 里 `BetaKey = "v1.4.7"`），用户主动锁版本 |
| Steam 上的下一档 | v1.5.3（跳过 v1.4.8；v1.4.8 只是模组作者用过的内部 build）|
| 游戏安装目录 | `E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\` |
| 游戏本体 DLL | `E:\SteamLibrary\...\bin\Win64_Shipping_Client\`（未装 BLSE，原生启动器）|
| Workshop 模组根 | `E:\SteamLibrary\steamapps\workshop\content\261550\<id>\` |
| 用户数据（junction）| `C:\Users\situj\OneDrive\Documents\Mount and Blade II Bannerlord\` → `E:\Bannerlord-UserData\Mount and Blade II Bannerlord\` |
| Junction reparse tag | `0xA0000003`（IO_REPARSE_TAG_MOUNT_POINT，OneDrive 会跳过）|
| Junction 原目录 | `OneDrive\Documents\Mount and Blade II Bannerlord.OneDriveBackup`（回滚用，可删）|
| 游戏语言 | English（`BannerlordConfig.txt` → `Language=English`）|
| Steam Cloud 状态 | 建议关闭（Steam → Bannerlord → Properties → General → 取消勾选 Keep game saves in Steam Cloud），junction 后 Steam Cloud 元数据混乱曾导致存档消失 |

---

## 模组清单（最终状态）

### 已启用（20 个）

| 类别 | Mod Id | 版本 | Workshop ID |
|---|---|---|---|
| 核心库 | Bannerlord.Harmony | v2.4.2.248 | 2859188632 |
| 核心库 | Bannerlord.ButterLib | v2.12.0 | 2859232415 |
| 核心库 | Bannerlord.UIExtenderEx | v2.13.3 | 2859222409 |
| 核心库 | Bannerlord.MBOptionScreen (MCM) | v5.12.3 | 2859238197 |
| Native | Native / SandBoxCore / Sandbox / StoryMode / CustomBattle | v1.4.7 | (base) |
| Native | NavalDLC | v1.2.7 | (base) |
| Combat | **RBM** | v4.5.0 | 2859251492 |
| Combat | **RBM_WS** | v4.5.0 | 3635788184 |
| Combat | RTSCamera | v5.4.16 | 3747725551 |
| Combat | RTSCamera.CommandSystem | v5.4.16 | 3747771970 |
| Combat | DismembermentPlus | v2.0.8.8 | 2875093027 |
| Campaign | ImprovedGarrisons | v4.2.0.7 | 2859265386 |
| Campaign | Retinues | v1.4.14.31 | 3599557394 |
| Campaign | PartySizeReunited (PSR) | v2.2.0 | 3372837208 |
| Campaign | ChooseYourTroops | v1.8.3 | 2957211804 |
| Campaign | PlayerSettlement | v7.5.0 | 3720376888 |
| Campaign | GarrisonDrills | v1.1.0 | 3735834360 |

### 已订阅但禁用
BirthAndDeath / FastMode / Cheats / BahamutArmory / swadian armoury / XorberaxLegacy / CustomClanPartySize / AutoParry

### 待下载 / 计划安装（2026-09-18 评估）

**目标**：实现"严格按兵种编队"（全面战争式，8 编队上限内），并配套放大战场规模。

| Mod | 版本 | 状态 | 用途 |
|---|---|---|---|
| **Stop Shuffling You Fools – Formation Manager** | 0.5.2 (Nexus 11869) | 已下载 `Downloads/` | 队伍界面把兵种钉到编队 I–VIII，持久化 + 防增援洗牌 |
| **TroopClassifier** | v0.2.0 (Nexus 12104) | 已下载 `Downloads/` | 兵种分类库，FormationManager 硬依赖 |
| **BattleSizeResized** | 2.0.4 for 1.4.x (Nexus 8177) | 已下载 `Downloads/` | 改战场/攻城/海战兵力上限 + 增援波阈值 + 大战 LOD 优化 |

（当前已装 20 mod 见上方"已启用"表；本节是在其之上计划新增。）

#### 兼容性评估（基于 SubModule.xml + DLL 字符串扫描；dnSpy/E: 本次不在，未做反编译对比）

**BattleSizeResized 2.0.4 —— 判定：低风险，可装**
- 依赖 Harmony/ButterLib/UIExtenderEx/MCM（**全已装**），无版本硬约束；目标 1.4.x，匹配 v1.4.7。
- 补丁全是 **Postfix**（`MaxBattleSizePostfix` / `MaxNumberOfAgentsForMissionPostfix` / `GetRealBattleSize*`），非破坏性、只改返回值 → 冲突面极小。
- 自带海战/攻城/sally-out 分别的兵力设定（配合 NavalDLC）+ 增援波阈值 + 大战 LOD 优化。
- ⚠ **唯一注意**：与 PSR（大部队）+ RBM（战斗计算）+ DismembermentPlus（断肢视效）叠加，超高兵力压性能/稳定性（参见 Bug #3 大战硬崩）。**兵力先调中档**，用它自带 LOD/增援阈值收敛。

**TroopClassifier v0.2.0 —— 判定：低风险，可装（版本满足）**
- 极轻量纯分类库，**只依赖 Native/SandBoxCore**（无 Harmony/UIExtender），冲突面几乎为零；.NET 4.7.2；目标 1.4.6，匹配 v1.4.7。
- **版本核对**：FormationManager 0.5.2 要求 `TroopClassifier v0.1.1`，你下的是 **v0.2.0**（> 0.1.1，满足）。交叉扫描确认 FormationManager 实际绑定的符号 `TroopClassifier` / `TroopRoleClassifier` / `Classify` **在 v0.2.0 里都存在** → API 表面兼容。
- ⚠ 残留风险：0.1.1→0.2.0 若某方法**签名**变了（同名不同参/返回），FormationManager 加载时会抛 `MissingMethodException` → **首启看 `Configs\ModLogs\trace<日期>.txt` / `butterlib*.txt`**；若真报错，退回 FormationManager Nexus "Requirements" 栏拿精确的 v0.1.1。
- 加载顺序：**TroopClassifier 必须在 FormationManager 之前**（FormationManager 的 SubModule 已声明 LoadBeforeThis）。

**Stop Shuffling You Fools 0.5.2 —— 判定：前置已齐 + 需实测**
- ✅ **硬依赖 `TroopClassifier`（Optional=false）已下载 v0.2.0**，版本满足（见上）。
- 其余依赖 Harmony 2.4.2 / UIExtenderEx 2.13.2 / MCM 5.11.4 —— 你的 2.4.2.248 / 2.13.3 / 5.12.3 **均满足或更新**。
- 目标 Native v1.4.6，游戏 v1.4.7 —— 差一档；按 Bug #3 结论 `DependentVersion` 只是 built-against 标记，原生启动器黄字警告但可加载。
- **补丁面（DLL 扫描）**：`MissionAgentSpawnPatch`（兵按指定编队生成、含增援=防洗牌核心）+ `MissionConstructorPatch` + `OrderOfBattleVMInitializePatch` + 队伍界面 UI（`PartyTroopTupleFormationBadge/CustomSplitEditor/RolePlanEditor`、`PartyCharacterVMMixin`）。
- **冲突面（2026-09-19 反编译核对后更正）**：早前基于 DLL 字符串扫描的假设有误，反编译 FM + CYT + Retinues 后重新评估：
  1. ~~**队伍界面**：CYT + Retinues + FM 三家争 `PartyCharacterVM`~~ ❌ **误判**。实际情况：
     - **CYT 不 mixin PartyCharacterVM**——它用独立全屏 `CYTGauntletMenuTroopSelectionView`，从 game menu 触发（`AddGamesMenu`）
     - **Retinues 不 mixin PartyCharacterVM**——它的 UI 全在 Clan Screen 的 Troops 标签（`ClanScreen_TroopsPanel` mixin）
     - **只有 FM 一家改造 PartyCharacterVM**（`PartyCharacterVMMixin.cs`）→ 无三家冲突
  2. **Mission spawn 三家 patch 三个不同方法，是流水线协作而非竞争**：
     - **CYT**：`SandBoxBattleMissionSpawnHandlerPatch` / `SandBoxSiegeMissionSpawnHandlerPatch`（上游 roster 筛选）
     - **FM**：`Mission.SpawnTroop` Postfix（每 troop 类型分配到 formation）
     - **Retinues**：`Mission.SpawnAgent` Prefix（每 agent 装备 set 随机）
     - **RBM**：其他 Mission/Agent 数值 patch（不与上三家撞 method）
     - 执行链路：CYT 筛 roster → SpawnTroop 分 formation（FM）→ SpawnAgent 换装（Retinues）→ RBM 数值套用。**顺序稳定，无竞争**。
  3. **Retinues.FormationQuerySystemPatch vs FM**：概念正交。Retinues patch 的是 `FormationQuerySystem.get_MainClass`（查询 "这 formation **算作** 哪一类"），FM 是 `agent.Formation = X` 的赋值。且 Retinues 该 patch 只在 `AllowFormationOverrides=true` 时生效——用户当前 `false`，Retinues 侧休眠。
- **真正需要实测的两件事**：
  1. **OoB 屏 vs RTSCamera.CommandSystem**：FM 的 `OrderOfBattleVMInitializePatch` 与 RTSCamera 的战场指挥扩展在 OoB 屏可能触及；风险低但值得进战斗前打开 OoB 屏看渲染
  2. **TroopClassifier v0.2.0 vs v0.1.1 签名漂移**：FM 是针对 v0.1.1 编译的，若 API 签名有变会启动报 `MissingMethodException`。首启看 `Configs\ModLogs\trace<日期>.txt` / `butterlib*.txt`
- **建议**：补 TroopClassifier → 新存档小规模试 → 验证 OoB 屏渲染 + 启动日志无 MissingMethod + 战场编队指定生效且增援不洗牌。**队伍界面渲染不再是风险点**（只 FM 一家 mixin）。

#### 建议加载顺序（在现有基础上插入）
`Harmony → ButterLib → UIExtenderEx → MCM → TroopClassifier → [其余现有 mod] → BattleSizeResized / FormationManager`（库全在前；FormationManager 放 CYT/Retinues 之后再试）

---

## 已做的定制修改

### 1. 用户数据 Junction 化（2026-09-16 上午）
OneDrive Files-On-Demand 阻塞 IO；把用户目录从 OneDrive 挪到 E: 本地，用 mount-point junction 桥接。

### 2. PSR `psr_bonus_scope` 2 → 0（2026-09-16 下午）
文件：`Configs\ModSettings\PartySizeReunited\PartySizeReunited.json`
效果：只有玩家部队扩容，其他 lord 回到 vanilla；结算不再卡死。
备份：`PartySizeReunited.json.bak-20260916`

### 3. Garrison Drills DLL 补丁（2026-09-16 晚）
文件：`workshop\261550\3735834360\bin\Win64_Shipping_Client\GarrisonDrills.dll`

三处字节修改（IL `ldc.i4.s` 操作数，同长度替换）：

| 偏移 | 原 | 新 | 语义 |
|---|---|---|---|
| 2332 | 0x0A (10) | 0x14 (20) | Basic XP/兵/6h |
| 2348 | 0x1E (30) | 0x3C (60) | Advanced XP/兵/6h |
| 2365 | 0x32 (50) | 0x64 (100) | Masterful XP/兵/6h |

同步改的语言 XML：`std_GarrisonDrills_strings.xml`（英）+ `CNs\std_GarrisonDrills_strings_cns.xml`（简中），文案 +10/+30/+50 → +20/+60/+100。

备份：`GarrisonDrills.dll.orig-20260916`
风险：Workshop 若自动更新，补丁被覆盖，需重跑。

### 4. LauncherData.xml 演化
- 上午：禁用 RBM / RBM_WS / RTSCamera / RTSCamera.CommandSystem（排查战斗崩溃）
- 中午：用户手动重开 RTSCamera / RTSCamera.CommandSystem（实测无问题）
- 晚上：启用 ImprovedGarrisons（食物采集方案）
- 深夜：重新启用 RBM + RBM_WS（确认崩溃是老存档不兼容，非 API）

备份：`LauncherData.xml.bak-pre-rbm-disable-20260916`

### 5. 食物经济堆叠（2026-09-17）

**症状**：RBM Campaign 打开后 fief 食物变化 -800 到 -900/天，库存长期为 0。

**为什么不走"精准降驻军食物"路**：RBM 的 `TownFoodStocksChangePatch` 对 `DefaultSettlementFoodModel.CalculateTownFoodStocksChange` Prefix return false，用 `RBMTownFoodSupply._measuredFoodChange` 缓存字典给结果。这是**整包账单**——作坊材料、囚犯粮、繁荣消耗、驻军、民兵混在一起，单调某一个参数无法精确控制结果。

**决策：三层叠加**

| 层 | 文件 | 改动 | 作用面 |
|---|---|---|---|
| 1. 村庄产出恢复 | `Configs\RBM\config.xml` | `<VillageProductionMultiplier>` 0.5 → **0.75** | 全世界 |
| 2. 每兵消耗降到近 vanilla | `Configs\RBM\config.xml` | `<TroopFoodWageFraction>` 0.5 → **0.1** | 全世界 |
| 3. 玩家 fief 兜底 | 5 个 `Configs\ImprovedGarrisons\Saves\IGConfiguration_*_TdthS0Oa3xcE.xml` | `<LoadFoodGatheringModule>true</...>` + `<EnablePlayerFoodBonus>true</...>` + `<DailyFoodGatheringAmount>` 10 → **1000** | **仅玩家 fief** |

- 1+2 让 RBM 全球经济不再窒息（NPC 领主一起松一口气）
- 3 是玩家侧兜底，作者代码字面标 `[IG-Cheats]`
- `EnabeNPCFoodbonus` 保持 false → RBM 经济压力在 NPC 侧保留

**关键代码**（`ImprovedGarrisons.dll` · `GarrisonFoodModel.CalculateTownFoodStocksChange`）：

```csharp
ExplainedNumber en = base.CalculateTownFoodStocksChange(...);  // 走 RBM Prefix
if (loadFoodGatheringModule && ((npcFief && npcBonus) || (playerFief && playerBonus))) {
    en.Add(DailyFoodGatheringAmount, "[IG-Cheats] Garrison Food Bonus");   // ← +1000
}
```

**关键澄清**：`DailyFoodGatheringAmount` **是 fief 级别的 flat 值，不是每兵**——最初误以为按驻军人数乘算，反编译后纠正。

**未来收敛/放大的旋钮**：
- 若食物永远爆仓觉得没意思：先降 `DailyFoodGatheringAmount` 从 1000 到 100-200
- 若想让 NPC 世界再更活：`TroopSettlementFoodDays=20` 可尝试降到 6-8（未验证具体路径进 `_measuredFoodChange`，先观察）
- **不建议**用 `DisableGarrisonNeedsFood`——它只砍驻军消耗，不动作坊/囚犯/民兵，效果反不如 +1000 flat

**备份**：
- `RBM\config.xml.bak-20260917`（`VillageProductionMultiplier` 改前）
- `RBM\config.xml.bak-TroopFoodWage05-20260917`（`TroopFoodWageFraction` 改前）
- 5 × `IGConfiguration_*_TdthS0Oa3xcE.xml.bak-20260917`（IG 开关改前）
- 5 × `IGConfiguration_*_TdthS0Oa3xcE.xml.bak-DailyFood10-20260917`（DailyFood 从 10 改前）

### 6. Retinues `MaxTroopTier` 8 → 10（2026-09-18）

**文件**：`Configs\ModSettings\Retinues\Retinues.Settings.xml`

**改动**：`<MaxTroopTier>` 从 `8` 拉到 `10`（schema 里 `SkillTotalTierN` / `SkillCapTierN` 已经定义到 tier 10，作者留了坑）。

**效果**：House Champion / House Guard 可以升到 tier 9 / tier 10，属性配置池扩到 `SkillTotalTier10=2350` + `SkillCapTier7Plus=360`。**新 tier 不自动应用到已有兵种**——玩家要手动进 Clan → Troops 花 `RankUpCostPerTier=1000` 金 + `RenownRequiredPerTier=10` renown 升上去。

**装备侧不变**：引擎硬顶在 `ItemTiers.Tier6`（见 Retinues 机制备忘 · 装备 tier 天花板）；抬 troop tier 只放宽属性，不解锁新装备。

**备份**：`Retinues.Settings.xml.bak-MaxTroopTier8-20260918`

### 7. Retinues XP 经济完全归零（2026-09-18）

**文件**：`Configs\ModSettings\Retinues\Retinues.Settings.xml`

**改动**：三个键一起改

```xml
<BaseSkillXpCost>100</BaseSkillXpCost>       → 0
<SkillXpCostPerPoint>1</SkillXpCostPerPoint> → 0
<SharedXpPool>false</SharedXpPool>           → true
<ForceXpRefunds>false</ForceXpRefunds>       → true
```

**效果**：
- 加技能 0 XP、降技能 100% 退款——**自定义兵种技能任意调整无成本**
- `SharedXpPool=true` 后 House Champion / House Guard / 所有自定义兵**共用同一 XP 池**（`TroopXpBehavior.PoolKey` 返回 `"_shared"`）——即使还在攒 XP 场景，Guard 打的 XP 也能给 Champion 用
- UI 里"降点扣 XP"警告变成 no-op（可能仍弹窗但零损失）

**为什么这样改**：目的是让自定义兵种**设计**完全自由，不用被 XP 经济卡住迭代节奏。反编译发现 XP 系统所有关键常量（`XpPerTier=10`、`AutoResolveXpPerTier=2.5f`、`TrainingXpMultiplier=0.2f`）都是硬编码，改配置是最干净的绕过路径。

**备份**：`Retinues.Settings.xml.bak-XpEconomy-20260918`

**替代路径**（未采用，作参考）：
- 启用 Cheats mod 后用 console `retinues.troop_xp_add <troop_stringid> <amount>` 官方后门直给 XP
- Retinues UI 切换到 Studio Mode（Culture/Kingdom 编辑）→ `SkillPointXpCost` 直接 return 0

### 8. Retinues Doctrine 系统限制完全解除（2026-09-18）

**文件**：`Configs\ModSettings\Retinues\Retinues.Settings.xml`

**改动**：三键改零

```xml
<EnableFeatRequirements>true</EnableFeatRequirements>            → false
<DoctrineGoldCostMultiplier>1</DoctrineGoldCostMultiplier>       → 0
<DoctrineInfluenceCostMultiplier>1</DoctrineInfluenceCostMultiplier> → 0
```

**效果**：
- Feat 进度要求 bypass（`GetDoctrineStatus` 里 `if (!Config.EnableFeatRequirements) return DoctrineStatus.InProgress`——feat 检查直接跳过，doctrine 立即可购买）
- Gold cost 归零（`num × 0 = 0`）
- Influence cost 归零（同上）
- **唯一保留门槛**：前置 doctrine 必须先解锁——UI 里按 Column × Row 0→3 顺序点即可，全部免费即时

**Doctrine 系统架构**（`DoctrineServiceBehavior.GetDoctrineStatus` + `TryAcquireDoctrine`）：

- 20 个 doctrine，5 Column × 4 Row 布局
- 每 doctrine 有：Column/Row（决定 gold/influence 基础 cost）+ Feats（前置进度）+ Prerequisite（前置 doctrine）+ IsDisabled 自检
- Row 0/1/2/3 基础 cost = 1000/5000/25000/100000 gold + 50/100/200/500 influence
- **`IsDisabled` 自检机制**：当 config 已经赐予该 doctrine 的效果时，doctrine 自动 disabled——避免重复
- Status 枚举：`Locked`（前置未满足）→ `Unlockable`（前置满足但 feat 未完成）→ `InProgress`（feat 完成，可购买）→ `Unlocked`

**当前 config 已自动灰化的 8 个 doctrine**（不需要解锁，功能已由 config 赐予）：

| Doctrine | 灰化原因 |
|---|---|
| LionsShare / BattlefieldTithes / PragmaticScavengers (C0R0-R2) | `UnlockItemsFromKills=true` |
| AncestralHeritage (C0R3) | `AllCultureEquipmentUnlocked=true` |
| ArmedPeasantry / StalwartMilitia / RoadWardens (C2R0-R2) | `NoDoctrineRequirements=true` |
| AdaptiveTraining (C3R3) | `ForceXpRefunds=true` + XP cost=0 |

**剩余 12 个可解锁 doctrine 及效果**：

| 位置 | Doctrine | 效果 |
|---|---|---|
| C1R0 | CulturalPride | 20% clan 文化装备返现 |
| C1R1 | ClanicTraditions | 兵种可装备打造武器 |
| C1R2 | RoyalPatronage | 20% kingdom 文化装备返现 |
| C1R3 | **Ironclad** | 无 tier 限制装备（突破 AllowedTierDifference）|
| C2R3 | Captains | 解锁 Captains |
| C3R0 | **IronDiscipline** | +5 技能上限（每 skill cap +5）|
| C3R1 | **SteadfastSoldiers** | +10 技能点（总预算 +10）|
| C3R2 | MastersAtArms | +1 elite 升级分叉数（2→3）|
| C4R0 | Indomitable | +5 HP 给 retinue |
| C4R1 | BoundByHonor | +20% retinue morale |
| C4R2 | **Vanguard** | +15% retinue 上限（比例卡放宽）|
| C4R3 | Immortals | +20% retinue 存活率 |

**备份**：`Retinues.Settings.xml.bak-Doctrines-20260918`

**替代路径**（未采用，作参考）：
- 保持 `EnableFeatRequirements=true`，用 console `retinues.feat_unlock_all` 一键完成所有 feat 进度（需 Cheats mod）
- 或单独 `retinues.feat_unlock <FeatKey>` 精准解锁；`retinues.feat_list` 查全表

---

## Retinues 机制备忘（2026-09-18 反编译结论）

反编译 `E:\SteamLibrary\...\workshop\content\261550\3599557394\bin\Win64_Shipping_Client\Retinues.dll` + `TaleWorlds.Core.dll` 得到的确切机制，避免以后再问：

### House 单位是什么

- `ret_retinue_house_champion` = Clan 的 `RetinueElite`
- `ret_retinue_house_guard` = Clan 的 `RetinueBasic`
- 玩家成为国王后名字变成 King's/Queen's Champion + Royal Guard（`TroopBuilder.MakeRetinueName`）

### 招募流程（100% vanilla + 轻量 patch）

- 招募本身走 vanilla `RecruitmentCampaignBehavior`——从村庄 notable 处雇兵
- Retinues 的干预：进入 settlement 时 patch `notable.Hero.VolunteerTypes[]`，用 custom troop 替换 vanilla 兵种；离开或存档前 `RestoreSnapshot` 还原（避免污染存档）
- 关键类：`Retinues.Features.Volunteers.Patches.VolunteerSwapForPlayer` / `WNotable.SwapVolunteers` / `WSettlement.SwapVolunteers`
- `RestrictToOwnedSettlements=true` → 只有自家 Clan 的 fief 才做替换；别人的村照常给 vanilla 兵
- `CustomVolunteerProportion=1` → 自家村里 100% volunteer 都是 custom
- **招募没有名额上限**——`MaxBasicRetinueRatio=0.2` / `MaxEliteRetinueRatio=0.1` **只影响"用普通兵转换成 retinue"**（`RetinueManager.RetinueCapFor`），不限制 volunteer

### 装备语义：多套 set + 每 agent 独立随机

- 每个 CharacterObject 的 `MBEquipmentRoster.AllEquipments` 是一个 `List<Equipment>`——**同一兵种可以有多套 battle set 和多套 civilian set**
- 战场 spawn 规则（`Retinues.Features.Agents.Patches.Mission_SpawnAgent_Prefix.Prefix`）：
  - 若 agent 是 civilian → 从 civilian sets 均匀随机
  - 否则若 `ForceMainBattleSetInCombat=true` → 强制第一套 battle set
  - 否则（**当前设置 = false**）→ 遍历所有 battle set 按 `CombatAgentBehavior.IsEnabled` 过滤后均匀随机
- **同一支部队里不同 agent 穿不同 set 是正常现象**，不是 bug
- 想统一装扮：Retinues UI 里删多余 set，或把 `ForceMainBattleSetInCombat` 改成 true

### 装备刷新语义：引用不是快照

- TroopRoster 只存 `CharacterObject` 引用 + 计数，无装备快照
- `WLoadout.SetEquipments` 直接写共享的 `_equipmentRoster`
- 保存 UI 改动的瞬间，**世界上所有该兵种（队里的、别的 lord 的、村里 volunteer 的）下一场战斗都读新装备**
- **不需要遣散再招**——那只是浪费 renown/兵源，没有刷新意义

### 装备 tier 天花板：引擎硬顶 Tier6

`TaleWorlds.Core.ItemObject+ItemTiers` 枚举只有：

```csharp
public enum ItemTiers { Tier1, Tier2, Tier3, Tier4, Tier5, Tier6, NumTiers }
```

**Bannerlord 引擎里所有装备的 tier 只到 6**，没有 tier 7-10 的物品。抬 `MaxTroopTier` 或 `AllowedTierDifference` 都不能创造新装备，只能让**属性槽位**变大。想要更多装备选择：装 BahamutArmory / swadian armoury / XorberaxLegacy 等加装备的 mod（仍然 tier 6 上限，但**数量**更多）。

### 相关设置速查（当前生效值）

| 设置 | 值 | 作用 |
|---|---|---|
| `MaxTroopTier` | 10 | Troop tier 上限（作者预留到 10）|
| `AllEquipmentUnlocked` | true | 跳过击杀解锁，所有 tier 1-6 装备直接可选 |
| `AllCultureEquipmentUnlocked` | true | 跨文化装备可选 |
| `AllowedTierDifference` | 3 | `item.tier ≤ troop.tier + 3` 才可穿；因物品 tier 上限 6，troop tier ≥ 3 后不再起限制 |
| `ForceMainBattleSetInCombat` | false | 每 agent 独立随机 battle set |
| `CopyAllSetsOnUnlock` | true | 解锁物品时复制到所有 set |
| `MaxEliteRetinueRatio` | 0.1 | Retinue 转换（非招募）上限 |
| `MaxBasicRetinueRatio` | 0.2 | 同上 |
| `CustomVolunteerProportion` | 1 | 自家村 volunteer 100% custom |
| `RestrictToOwnedSettlements` | true | 只在自家 fief 做 volunteer 替换 |
| `RankUpCostPerTier` | 1000 gold | 每 tier 升级金币消耗 |
| `RenownRequiredPerTier` | 10 | 每 tier 升级 renown 门槛 |

---

## 俘虏 / 灭国收编 / 地牢机制（游戏机制笔记）

> **验证状态**：以下为 vanilla 机制层面结论（general knowledge），**尚未反编译核对**——E: 盘（游戏 DLL + live journal 权威源）本次会话未挂载。确切触发条件与逃跑概率公式待 E: 挂载后扒 DLL 补齐，见文末 decompile TODO。当前 mod 清单里没有改写俘虏/灭国收编系统的项，故 vanilla 机制应照常生效（ImprovedGarrisons 只间接经"驻军规模→地牢逃跑率"相关）。

### 敌国覆灭后，被俘贵族会自动加入我吗？

**不会自动加入。"被俘" ≠ "效忠"。**

- **自由身的敌国氏族**（没被你关押）：王国失去最后领地被摧毁（`DestroyKingdomAction`）后，其氏族变成"无王国"，由氏族 AI 自行决定去向（投靠现存王国当封臣/雇佣兵/游荡），基于关系、实力、文化、领地机会评分——**不强制归你**。
- **被你关押的敌国贵族**：这才是收编敌将的正路 → 见下。

### 收编俘虏贵族（劝降 / defection）

- 你可对手里的俘虏贵族发起**招募/劝降**（persuasion 说服小游戏），成功后其**整个氏族并入你的王国，成为你的封臣（vassal clan）**。
- **祖国被灭、无家可归（homeless）的贵族劝降成功率大幅提升**——失去领地与君主后最愿改旗易帜。
- 实战正路：国战期间**尽量俘虏并关押敌方所有 lord**（别急着放/换）→ 打到对方只剩残地/刚灭国时他们变 homeless → 逐个劝降收编。
- **关键定性**：收编来的敌将成为**独立封臣**（自带氏族、自领一支部队、AI 驱动），**不是加入你氏族的 companion**。这条直接决定了下面 perk/trait 的可控性。

### 地牢 / 俘虏系统

- **存放**：战斗胜利后俘虏进入**队伍俘虏栏**（容量有限，受 Roguery/Steward perk 影响）；进自己据点后可"关进地牢"腾队伍容量，据点地牢容量远大于行军队伍。
- **逃跑（每日判定）**：
  - 关在**行军队伍**里 → 逃跑率**高**（贵族尤甚）
  - 关在**据点地牢** → 逃跑率**低**；**驻军越多/据点越安全越低**
  - 据点被攻陷或叛乱（rebellion）→ 俘虏被放出/解救
- **处置三条路**：① 收编（贵族→劝降入你王国；普通兵→随时间有概率自愿入队）② 赎金/释放（经 ransom broker 卖钱或对方赎回）③ 关着当战略资源（削弱敌国野战实力，加速其崩盘）。
- **想稳住敌国 lord**：别带在身上，塞进一座**驻军充足**的城堡地牢。

### 收编贵族的 perk / trait 可控性

- **Perk**：vanilla 里**只有主角（player character）能手动选 perk**；companion、氏族成员、封臣都是随技能到里程碑**自动选 perk**，玩家不能手动分配。所以"像 companion 那样调 perk"这个前提本身对 vanilla 不成立——**companion 也不能手动调**（社区常年吐槽点，需 Improved Companions / Character Reload 之类 mod 才解锁；当前清单没装）。收编来的敌将是**独立封臣**，比 companion 更不可控（纯 AI 英雄）。
- **Trait**（Honor / Valor / Mercy / Generosity / Calculating）：性格值。主角的 trait 随行为/抉择缓慢漂移；AI 英雄的 trait 生成时基本定死，只随其自身行为轻微漂移，**玩家无法直接编辑他人 trait**，无对应 UI。收编来的敌将保留自己的 trait，改不了。
- **想要"技能/perk/装备全由我掌控"的英雄** → 只能走**酒馆 wanderer companion** 路线加入你氏族；即便如此 vanilla 也只能控装备/队伍角色/总督任命，perk 仍自动、技能靠使用成长。
- ⚠ 以上 perk/trait 结论有 v1.4.7 版本差异风险，需反编译 perk 选择 UI 门禁（是否 gate 在 `Hero.MainHero`）核实。

### decompile TODO（E: 挂载后核对）

- [ ] `DestroyKingdomAction`：灭国触发条件 + 氏族选新王国的评分逻辑
- [ ] 招募俘虏贵族的对话条件函数（`LordConversationsCampaignBehavior` 或相关）：确切 gate（是否要求祖国被灭 / 关系阈值 / 玩家是否为国王 / 氏族实力）
- [ ] 逃跑概率模型（`EscapePrisoner...` behavior/model）：公式 + 驻军/安全度权重
- [ ] perk 选择 UI 门禁：确认 v1.4.7 是否只允许 `Hero.MainHero` 选 perk；trait 是否存在任何 setter 路径

---

## 治国：中央集权 vs 分封 / 执政官机制（游戏机制笔记）

> **验证状态**：vanilla 机制层面结论 + 策略分析，未反编译。当前 mod 不改写封地/执政官/氏族经济（RBM 只加重驻军军饷+口粮成本，反而放大集权代价）。

### 执政官（Governor）≠ 封地拥有者

- 执政官 = 把自己氏族成员/companion 派驻到你**已拥有**的据点，提供加成（同文化→忠诚、安全、繁荣、民兵、Stewardship perk 加速建设），但据点仍属于你。
- **一个英雄要么坐镇当执政官、要么在外带兵，不能兼得**（机会成本）。

### 纯中央集权的三道硬天花板

1. **英雄池小**：氏族成员数（家族 + companion）被 clan tier 卡死，凑不齐几十个据点的执政官。
2. **执政官抽走军事人手**：撒去当执政官 = 自废野战军；可带部队数（clan party limit）也被 clan tier 卡。
3. **钱粮扛不动**：自有每块地的驻军军饷 + 口粮全你付，还得亲自守。**RBM 下驻军口粮成本很实**（对照 fief 食物赤字 -800~-900/天，修改 #5）——集权 = 把全部据点口粮/军饷堆到一个氏族。

### 分封买到的东西（移到"资产负债表之外"）

| 维度 | 自有(集权) | 分封 |
|---|---|---|
| 招兵/军饷 | 你出 | 封臣自费 |
| 驻军口粮 | 你出(RBM 痛) | 封臣承担 |
| 边境防守 | 你亲跑 | 封臣自守 |
| 大会战兵力 | 只你几支队 | 花影响力召封臣组军 |
| 英雄占用 | 吃你英雄池 | 封臣带自己英雄 |

分封 = 用"部分控制权"换"远超单氏族上限的军事投射与守土"。

### 推荐：集权核心 + 分封边疆（混合制）

1. 核心心脏地带（本文化城、经济枢纽、要地）→ 自留 + 自己人执政 = 直辖集权核心。
2. 边境/异文化/次要据点 → 分给忠诚封臣自费守边、供军。
3. 少数核心据点用氏族成员执政，其余留着带兵维持野战拳头。

### 政治现实

一块地都不分 → 封臣关系暴跌 → 叛离（且连原封地一起带走）。哪怕最集权也得留部分封地喂忠诚。**分地 = 守地盘 vs 喂封臣的持续取舍，无法单方取消。**

---

## 氏族(Clan) vs 家族(血脉 Family) / 成员添加机制（游戏机制笔记）

> **验证状态**：vanilla 机制层面结论，未反编译。

### Clan ≠ Family

- **Clan（氏族）** = 整个组织。companion **属于氏族**（出现在 Members 列表，可带队/当执政官/总督），但**不属于家族血脉**——**不能继承氏族**（继承人只从血脉里选）。
- **Family（家族血脉）** = 血缘 + 姻亲核心：族长、配偶、子女、兄弟姐妹及其配偶子女。
- companion 可**联姻并入血脉** → 从"氏族成员"升级为"家族成员"，这是唯一通道。

### 添加成员的路径

| 方式 | 加入身份 | 机制 |
|---|---|---|
| 雇 wanderer | 氏族成员(非家族) | 酒馆找流浪英雄(或百科定位)，花钱招募；数量受 clan tier 上限 |
| 联姻(娶外族贵族) | 家族(血脉) | 配偶并入你氏族当家族成员 |
| 联姻(companion × 家族成员) | companion 升级为家族 | 配对后 companion 并入血脉 |
| 生育 | 家族(血脉) | 已婚夫妻生子，随游戏年份成年后成为可带队家族成员 |

- 子女是**原版原生机制**（base game 自带生育/年龄/死亡）；禁用的 `BirthAndDeath` mod 在 v1.4.7 基本冗余（具体待扒）。
- companion 上限 / 可带部队数(clan party limit) / 家族容量都靠 **clan tier（renown）** 抬高。
- **招募敌国封臣 ≠ 加入你氏族**（那是独立氏族进你王国当封臣）。

### 与中央集权计划的接口

执政官池 = **家族成员 + companion 都算**。扩充直辖治理靠：多雇 companion（最快，受 tier 上限）/ 联姻拉贵族 / 生育养子女 / 刷 renown 抬 clan tier。**companion 是填满执政官岗位的主力兵源，只是不能继承王位。**

---

## 战斗编队 / 部署机制 · 为何不能按兵种细分（游戏机制笔记）

> **验证状态**：vanilla 机制层面，未反编译（E: 未挂载）。含 decompile TODO。

### Custom Battle vs Campaign：兵种为何一个能挑一个不能

- **Custom Battle** = UI 选择器从零构建军队，兵种构成是**输入参数**（打完即弃）。
- **Campaign/Sandbox** = 战斗兵力 = 真实队伍花名册(TroopRoster)，构成在上游（招募/升级/遣散）就定死；战斗时给的是**部署权**不是**重组权**。设计一致性：战役军队 = 队伍本身。

### FormationClass：引擎 8 槽

- 枚举 8 个常规编队：`Infantry(0) Ranged(1) Cavalry(2) HorseArcher(3) Skirmisher(4) HeavyInfantry(5) LightCavalry(6) HeavyCavalry(7)`。
- 每兵种有**计算出的** `DefaultFormationClass` / `GetFormationClass()`（按装备/技能：有马+远程=骑射，有马=骑兵，有远程=远程，否则步兵）。
- **默认自动分组只用前 4 类**（步/远/骑/骑射），F1–F8 指挥。这就是"按属性分组"。

### 原版其实支持"按兵种编排"——在部署 / Order of Battle 阶段

- 大型野战、你当指挥时有部署阶段（小规模/伏击/非指挥官会跳过）。
- 兵以"兵种卡"呈现，可**拖进 8 个编队槽任意一个** + 指派 captain/阵型 → **同 class 不同兵种可拆到不同编队**。
- **局限**：只在部署阶段、每场手动重来（无预案保存）、部分战斗跳过、**中途难按兵种重组**、**增援波按 class 自动归队可能打乱手动编排**。

### Choose Your Troops 的层级

- CYT 只作用在**花名册层**（决定带哪些部队进场），**不碰编队层**。与"进战斗后按兵种编组"是两个不同层，天生管不着。

### 已有解法：RTSCamera.CommandSystem

- 提供 RTS 俯视 + 框选/点选单位 + 扩展编队控制，是最接近"战斗中按实际单位重组指挥"的东西。优先榨干它再考虑新 mod。具体能力待反编译坐实。

### decompile TODO（E: 挂载后核对）

- [ ] `FormationClass` 枚举 + `GetFormationClass`/`DefaultFormationClass` 判定逻辑
- [ ] Order of Battle 兵种分配代码（`OrderOfBattleVM`/`MissionOrderVM`/部署 controller）：v1.4.7 能拖到哪几个槽
- [ ] **RTSCamera.CommandSystem** patch 了什么、能否按兵种类型建独立编队、中途能否重组（优先级最高）
- [ ] CYT 花名册层 hook（确认不碰编队）
- [ ] 增援波(reinforcement)如何按 class 分配到编队

---

## Bug 历史与修复

### Bug #1 · 大规模会战结算界面卡死
- 症状：220v240 大战后结算界面画面正常但按钮无反应；`safely_exited=1`；无 crash 报告
- 根因：PSR `psr_bonus_scope=2` 让所有 lord 都扩容，多个 party 同时触发 `DefaultPartySizeLimitModel` patch 死锁
- 修复：`psr_bonus_scope` 改 0

### Bug #2 · Junction 化后 Steam Cloud 覆盖存档
- 症状：`save001.sav` 从 E: 路径消失
- 修复：从 OneDriveBackup 复原，并备份到 `SaveBackups\`
- 预防：手动关 Bannerlord 的 Steam Cloud

### Bug #3 · 加装 RBM 后进入战斗即崩
- 症状：进战斗 Mission StartUp 后进程消失（硬崩），无 crash 日志
- 错误诊断走弯路：以为是 `Native@v1.4.8` 依赖不匹配；用户实测 RTSCamera 5.4.16 同样声明 v1.4.8 但单独可用，证明 `DependentVersion` 只是 built-against 标记
- 真根因：RBM 4.4.9 引入 Campaign Economy，`OnGameStart` 加 14 个 CampaignBehavior + 完全替换 `WorkshopModel`；老 vanilla 存档没这些数据，Mission StartUp 时 NPE 穿到 native access violation
- 修复：开新战役（当前 save003），弃用 save001
- 逃生舱：`Configs\RBM\config.xml` 里 `<RBMCampaign><Enabled>0</Enabled>` 可跳过全部 14 个 behavior，只留 Combat/AI/Tournament

RBM.SubModule.OnGameStart 关键代码（Token 0x06000007）：
```csharp
if (rbmCampaignEnabled && game.GameType is Campaign) {
    starter.AddBehavior(new RBMSpoilsCampaignBehavior());
    starter.AddBehavior(new RBMTroopUpkeepCampaignBehavior());
    starter.AddBehavior(new RBMSimulationCampaignBehavior());  // mission startup 打快照
    starter.AddBehavior(new RBMSpectateCampaignBehavior());
    starter.AddBehavior(new RBMEconomyCampaignBehavior());
    starter.AddBehavior(new RBMSettlementWealthCampaignBehavior());
    starter.AddBehavior(new RBMCaravanBehavior());
    starter.AddBehavior(new RBMVillageLedgerCampaignBehavior());
    starter.AddBehavior(new RBMTownLedgerCampaignBehavior());
    starter.AddBehavior(new RBMGarrisonRefillBehavior());
    starter.AddBehavior(new RBMRecruitBiasBehavior());
    starter.AddBehavior(new RBMSettlementDefenseBehavior());
    starter.AddBehavior(new RBMDeserterRaiderBehavior());
    starter.AddModel<WorkshopModel>(new RBMWorkshopModel());   // 完全替换
}
```

### Bug #4 · IG 设置在 MCM 里找不到
- 原因：IG 不走 MCM，有自己的 config UI（ribbon → ConfigScreen prefab）
- 正规路径：进城/堡 → 城镇菜单上有 IG ribbon → 点 Configuration
- 备用路径：关游戏后改 `Configs\ImprovedGarrisons\Saves\IGConfiguration_<save>_<hash>.xml`

IG 默认配置（Bug #4 发现当时的状态，未开启食物采集）：

```xml
<LoadFoodGatheringModule>false</LoadFoodGatheringModule>
<EnablePlayerFoodBonus>false</EnablePlayerFoodBonus>
<EnabeNPCFoodbonus>false</EnabeNPCFoodbonus>
<DailyFoodGatheringAmount>10</DailyFoodGatheringAmount>
<CustomGarrisonSizeMultiplier>1</CustomGarrisonSizeMultiplier>
```

当前生效值见"已做的定制修改 #5 食物经济堆叠"。

### Bug #5 · Fief 食物赤字 -800 到 -900

- **症状**：RBM Campaign 开启后 fief 食物变化 -800 到 -900/天，`FoodStocks` 长期 0
- **用户初始认知偏差**：以为是 PSR 扩容驻军导致。实际 PSR `psr_bonus_scope=0` 只影响玩家部队上限，未直接扩容驻军。真正抬高每兵消耗的是 RBM `TroopFoodWageFraction=0.5` + `TroopSettlementFoodDays=20`；驻军变大是因为玩家 PSR 扩容后往驻军转兵
- **诊断走弯路**：
  1. 首次修：只开 IG `LoadFoodGatheringModule=true` + `EnablePlayerFoodBonus=true`（`DailyFoodGatheringAmount=10`）→ 无感
  2. 反编译 `GarrisonFoodModel.CalculateTownFoodStocksChange` 发现 `DailyFoodGatheringAmount` **是 fief 级 flat 值**（不是我最初说的 per-troop）
  3. 反编译 `TownFoodStocksChangePatch` 发现 RBM 用 `_measuredFoodChange` 整包账单，单调不可控
- **修复**：见"已做的定制修改 #5 食物经济堆叠"

---

## 模组间交互与已知风险

### 有意协同
- **食物经济堆叠**（见修改 #5）：`VillageProductionMultiplier=0.75` + `TroopFoodWageFraction=0.1` + IG `DailyFoodGatheringAmount=1000`——NPC 世界经济压力保留，玩家 fief 兜底满仓
- RBM + Retinues：域不重叠（RBM 改战斗计算，Retinues 加新兵种模板）
- RBM Combat + DismembermentPlus：域不重叠（RBM 改数值，DP 加视效）

### 观察中的风险
- RBM `WorkshopModel` 替换 + PlayerSettlement 建作坊：PS 自建作坊走 RBM 新配方，边缘 case 需观察
- CYT + RBM `RBMRecruitBiasBehavior`：都影响招兵，CYT 手选覆盖 RBM bias（只作用 AI）
- IG `NPCSpawnGuards` + RBM `RBMGarrisonRefillBehavior`：都改 NPC 驻军，IG 默认 NPCSpawnGuards=false 无冲突
- RTSCamera 声明 v1.4.8：实测 v1.4.7 不炸，但战斗中 native 层若有隐性调用未覆盖仍可能崩

---

## 备份与回滚

| 备份 | 内容 |
|---|---|
| `E:\Bannerlord-UserData\SaveBackups\save001.pre-v1.4.8-upgrade.<时间戳>.sav` | 旧 vanilla 存档 |
| `C:\Users\situj\OneDrive\Documents\Mount and Blade II Bannerlord.OneDriveBackup\` | Junction 化前的 OneDrive 版本 |
| `Configs\LauncherData.xml.bak-pre-rbm-disable-20260916` | 一次禁用 RBM 前的启用列表 |
| `Configs\ModSettings\PartySizeReunited\PartySizeReunited.json.bak-20260916` | PSR 改 scope 前的 config |
| `workshop\...\3735834360\...\GarrisonDrills.dll.orig-20260916` | 未改 DLL |
| `Configs\RBM\config.xml.bak-20260917` | RBM 食物调整前（VillageProductionMultiplier 改前）|
| `Configs\RBM\config.xml.bak-TroopFoodWage05-20260917` | RBM `TroopFoodWageFraction` 改前 |
| 5 × `IGConfiguration_*_TdthS0Oa3xcE.xml.bak-20260917` | IG 食物采集开关改前 |
| 5 × `IGConfiguration_*_TdthS0Oa3xcE.xml.bak-DailyFood10-20260917` | IG `DailyFoodGatheringAmount` 从 10 改前 |
| `Configs\ModSettings\Retinues\Retinues.Settings.xml.bak-MaxTroopTier8-20260918` | Retinues `MaxTroopTier` 从 8 改 10 前 |
| `Configs\ModSettings\Retinues\Retinues.Settings.xml.bak-XpEconomy-20260918` | Retinues XP 经济四键改零/开启前 |
| `Configs\ModSettings\Retinues\Retinues.Settings.xml.bak-Doctrines-20260918` | Retinues Doctrine 三键改零前 |

---

## 待办 / 开放问题

- [ ] 手动在 Steam 里关闭 Bannerlord 的 Steam Cloud sync（Properties → General）
- [x] ~~进游戏在 town/castle 菜单里找 IG ribbon，打开 Food Gathering~~ ← 已通过直接改 XML 完成（修改 #5）
- [ ] 观察新战役里 RBM Campaign 是否正确工作（看 `RBM\logs\garrison\`、消息栏 spoils/ledger）
- [ ] 验证 Garrison Drills 训练效果翻倍（进城 → Train troops → 看 UI +20/+60/+100 XP）
- [x] ~~验证食物经济堆叠实测效果（进 fief → 食物变化条应有 `[IG-Cheats] Garrison Food Bonus: +1000` 且总变化转正）~~ ← 2026-09-18 确认修复
- [ ] Workshop 若自动更新 GarrisonDrills，DLL 补丁被覆盖，需重跑
- [x] ~~食物调优（若观察后需要）：太富裕/NPC 穷/玩家仍赤字三档旋钮~~ ← 2026-09-18 食物经济已稳定，无需再调
- [ ] AutoParry 是否要启用（当前 false）

### 调查与开发项目（backlog）

- [x] ~~**Retinues · House 单位 tier 上限**~~ ← **2026-09-18 结案**：作者早已在 MCM 里预留 `MaxTroopTier` 到 10，改配置即可；改后需玩家手动 rank up 已有兵种。详见"Retinues 机制备忘"小节 + 修改 #6
- [ ] **Retinues · Clan Traditions 跳过**：Clan Traditions 系统（族群传统）是否有内置开关能整个禁用/跳过？如果没有，找它绑定的 CampaignBehavior 名称，评估直接不加载该 behavior 的可行性
- [x] ~~**RBM · Bot 武器优先度**~~ ← **2026-09-18 结案**：RBM AI **不重写** vanilla 武器选择评分，只做辅助（posture 掉武器、盾墙方向、骑射队分配）；skill 通过 handling/speed 间接影响 AI 评分。完整 combo 表、"废装备"警告、骑马武器长度限制、Cataphract Lance 副武器陷阱见 `TroopDesignReference.md`
- [ ] **PlayerSettlement · 村庄绑定机制**：扒 `PlayerSettlement.dll` 找 `MaxBoundVillages` / `AttachVillage` / `BindVillage` 类 API。目标：自建 town 时能否指定绑定多个食物特化村（wheat/cattle/sheep/swine/fisherman）来打造食物爆棚 fief。附带查：绑定村庄数量是否有硬上限、能否**重新绑定 vanilla 村庄**（把邻近 wheat 村从别人 fief "转"到自己 fief）

### 食物经济 · 已解决（2026-09-18）

- [x] ~~每日食物变化在 +/- 之间震荡，无法稳定积累~~ ← 食物经济堆叠（修改 #5）实测生效，问题关闭

---

## 在另一台设备上复刻本套配置

**目标**：在一台全新安装 Bannerlord + 相同 mod 的设备上复现本 repo 记录的所有配置改动。

### 前置条件

1. Steam 上安装 **Mount & Blade II Bannerlord**，Beta 分支切到 **v1.4.7**（`Steam → 右键游戏 → Properties → Betas → 选 v1.4.7`）
2. 订阅并启用（Workshop）以下 20 个 mod（详见"模组清单"章节）：
   - 核心库：Harmony 2.4.2、ButterLib 2.12.0、UIExtenderEx 2.13.3、MCM 5.12.3
   - Combat：**RBM 4.5.0**、RBM_WS 4.5.0、RTSCamera 5.4.16、RTSCamera.CommandSystem 5.4.16、DismembermentPlus 2.0.8.8
   - Campaign：ImprovedGarrisons 4.2.0.7、Retinues 1.4.14.31、PartySizeReunited 2.2.0、ChooseYourTroops 1.8.3、PlayerSettlement 7.5.0、GarrisonDrills 1.1.0
3. **游戏至少启动一次**——让 mod 生成默认 `Configs` 目录
4. Steam Cloud **关掉**（Properties → General → 取消 "Keep game saves in Steam Cloud"）—— 避免 junction + Cloud 混乱丢档
5. **用户数据 junction 化**（推荐但可选）：
   ```powershell
   $src = "$env:USERPROFILE\OneDrive\Documents\Mount and Blade II Bannerlord"
   $dst = "E:\Bannerlord-UserData\Mount and Blade II Bannerlord"
   # 先把 $src 里的内容复制到 $dst
   Move-Item $src "$src.OneDriveBackup"
   New-Item -ItemType Junction -Path $src -Target $dst
   ```

### 复刻步骤

**Step 1**：Clone 本 repo 到新设备（用来查改动的目标值 + 备份 baseline）：

```powershell
cd $env:USERPROFILE\git
git clone https://github.com/ShadowLOL233/Mount-and-Blade-2-Bannerlord-custome-development.git
```

**Step 2**：定位 Configs 根目录（**关闭 Bannerlord 后操作**）：

```
C:\Users\<你>\OneDrive\Documents\Mount and Blade II Bannerlord\Configs\
或（junction 化后）
E:\Bannerlord-UserData\Mount and Blade II Bannerlord\Configs\
```

**Step 3**：按以下清单在对应文件里改值。**每个文件都先备份**（`Copy-Item file file.bak-复刻日期`）。

| # | 文件 | 键 | 改前 | 改后 |
|---|---|---|---|---|
| 1 | `ModSettings\PartySizeReunited\PartySizeReunited.json` | `psr_bonus_scope` | 2 | **0** |
| 2 | `RBM\config.xml` | `<VillageProductionMultiplier>` | 0.5 | **0.75** |
| 2 | `RBM\config.xml` | `<TroopFoodWageFraction>` | 0.5 | **0.1** |
| 3 | `ImprovedGarrisons\Saves\IGConfiguration_<save>_TdthS0Oa3xcE.xml` × 5 | `<LoadFoodGatheringModule>` | false | **true** |
| 3 | 同上 | `<EnablePlayerFoodBonus>` | false | **true** |
| 3 | 同上 | `<DailyFoodGatheringAmount>` | 10 | **1000** |
| 4 | `ModSettings\Retinues\Retinues.Settings.xml` | `<MaxTroopTier>` | 8 | **10** |
| 5 | `ModSettings\Retinues\Retinues.Settings.xml` | `<BaseSkillXpCost>` | 100 | **0** |
| 5 | 同上 | `<SkillXpCostPerPoint>` | 1 | **0** |
| 5 | 同上 | `<SharedXpPool>` | false | **true** |
| 5 | 同上 | `<ForceXpRefunds>` | false | **true** |
| 6 | 同上 | `<EnableFeatRequirements>` | true | **false** |
| 6 | 同上 | `<DoctrineGoldCostMultiplier>` | 1 | **0** |
| 6 | 同上 | `<DoctrineInfluenceCostMultiplier>` | 1 | **0** |

**Step 4**：DLL 补丁（GarrisonDrills 训练效果翻倍）——**这个需要重跑**，Workshop 每次更新覆盖后重做：

文件：`workshop\261550\3735834360\bin\Win64_Shipping_Client\GarrisonDrills.dll`

三处字节修改（PowerShell 脚本）：

```powershell
$dll = 'E:\SteamLibrary\steamapps\workshop\content\261550\3735834360\bin\Win64_Shipping_Client\GarrisonDrills.dll'
Copy-Item $dll "$dll.orig-复刻日期"
$bytes = [IO.File]::ReadAllBytes($dll)
$bytes[2332] = 0x14   # Basic XP  10 → 20
$bytes[2348] = 0x3C   # Advanced  30 → 60
$bytes[2365] = 0x64   # Masterful 50 → 100
[IO.File]::WriteAllBytes($dll, $bytes)
```

同步改文案（可选，仅影响 UI 显示）：`workshop\261550\3735834360\ModuleData\Languages\std_GarrisonDrills_strings.xml` 和 `CNs\std_GarrisonDrills_strings_cns.xml`，把 +10/+30/+50 改成 +20/+60/+100。

**Step 5**：LauncherData.xml——参考本 repo 的 `LauncherData.xml.bak-pre-rbm-disable-20260916` 或直接手动在启动器 UI 里勾选那 20 个 mod。

**Step 6**：进游戏。**必须开新战役**（老 vanilla 存档 + RBM 4.4.9+ Campaign 会硬崩，见 Bug #3）。

### 复刻后进入游戏做的事（一次性）

1. Escape → Options → 确认 Bannerlord 语言 = English
2. 首次进战役后，Retinues UI → 每个自定义兵种按 `TroopDesignReference.md` 分配技能（现在 XP 成本 0，随便调）
3. House Champion 装备按 `TroopDesignReference.md` 第 5.1 节配（Cataphract Lance + Heavy Shield + 空 + 空）
4. 战术：Cavalry 按 F1+F3 单独 Charge

### 快速验证清单（复刻完打一场速验）

- [ ] Retinues UI 里加技能显示 **Cost: 0 XP**
- [ ] 220v240 大战结算界面**不卡死**（PSR 修复生效）
- [ ] Fief 食物变化条含 `[IG-Cheats] Garrison Food Bonus: +1000` 且总变化转正
- [ ] Garrison Drills 训练面板显示 **+20 / +60 / +100 XP**
- [ ] House Champion tier 能升到 **10**（`MaxTroopTier` 生效）
- [ ] 大战一场后 House Champion / Guard 的 XP 池是**同一个数字**（`SharedXpPool` 生效）
- [ ] Retinues UI → Doctrine 页面：可解锁的 12 个 doctrine 显示为 `In Progress`（feat 已 bypass），点击后 gold/influence cost 显示 **0**，可立即解锁

如果某项不符，回查本 journal 对应 modification 小节。

---

## 调试参考：如何找信息

崩溃/卡死时优先看：
1. `Configs\ModLogs\butterlib<日期>.txt` — ButterLib 托管代码异常。有 exception = 托管 bug
2. `Configs\ModLogs\default<日期>.log` — MCM/ButterLib/Retinues 通用日志
3. `Configs\ModLogs\trace<日期>.txt` — Harmony/AccessTools 找不到 method 的告警，第一信号
4. `workshop\261550\3599557394\debug.log`（Retinues）— `OnMissionStarted`/`OnEndMission` 最细粒度探针
5. `Configs\RBM\logs\garrison\` — RBM 每次运行的 garrison 独立日志
6. `CrashHandler\player_settlement_fixes.log` — PlayerSettlement 独立 patch 日志
7. `Configs\ImprovedGarrisons\Saves\` — IG 每存档配置 + binary state

**没有 crash 报告 = native access violation**（进程直接死，托管层 exception handler 抓不到），几乎都是版本不匹配或存档兼容性问题。

反编译 mod DLL 工具：`C:\Users\situj\Desktop\dnSpy-net-win64\`
- GUI 编辑：`dnSpy.exe`
- CLI 反编译：`dnSpy.Console.exe -t <TypeName> <path\to\mod.dll>`
- 内嵌 dnlib：`bin\dnlib.dll`（程序化 patch 时可加载）

DLL 常量字节修改：
- 同长度改（`ldc.i4.s N` 换 -128..127）：`dnSpy.Console` 找目标 → PowerShell 扫 `1F <hex>` pattern 确认唯一 → `[IO.File]::ReadAllBytes` 改 `WriteAllBytes`。**必须先关 Bannerlord 启动器**（`TaleWorlds.MountAndBlade.Launcher` 会锁 mod DLL）
- 长度变化改（如 "50" → "100"）：走 language XML 覆盖，DLL fallback 字符串不动

---

*本文档由 Claude Code 协助生成，2026-09-16 起记录一路的模组诊断与配置。后续新 bug 或修改请在对应小节增补。*
