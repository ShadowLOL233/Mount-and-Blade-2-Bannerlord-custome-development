# Bannerlord 骑马与砍杀2 模组开发日志

**最后更新**：2026-09-20

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

## 第二设备：反编译 / 构建工具链（2026-09-19 搭建）

> 本节针对**第二台设备**（游戏装在 **D:**，非主力机的 E:）。记录在此机复刻反编译 + 自研 mod 构建所需的环境。游戏版本与主力机一致（v1.4.7），故 DLL/构建产物通用。

### 环境差异 vs 主力机

| 项 | 主力机 | 第二设备（本机） |
|---|---|---|
| 游戏本体 | `E:\SteamLibrary\...\` | `D:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\` |
| 游戏 DLL | `E:\SteamLibrary\...\bin\Win64_Shipping_Client\` | `D:\SteamLibrary\...\bin\Win64_Shipping_Client\` |
| 游戏版本 | v1.4.7 | v1.4.7（一致） |
| 反编译器 | dnSpyEx（Desktop GUI） | **ilspycmd 8.2.0.7535**（CLI，dotnet global tool） |

### 已装工具（winget / dotnet）

- **.NET SDK 8.0.425** — `winget install Microsoft.DotNet.SDK.8`；`C:\Program Files\dotnet\dotnet.exe`
- **.NET Framework 4.8.1 Developer Pack** — `winget install Microsoft.DotNet.Framework.DeveloperPack_4`（编译 net472 目标必需）
- **ilspycmd 8.2.0.7535** — `dotnet tool install -g ilspycmd --version 8.2.0.7535`（最新版需 .NET 9，故 pin 8.x）；`C:\Users\situj\.dotnet\tools\ilspycmd.exe`

### 反编译用法（ilspycmd）

- 列举类型：`ilspycmd <asm.dll> -l e -r <binDir>`（`e`=enum，另有 c/i/s/d）
- 反编译单类型：`ilspycmd <asm.dll> -t <Full.Type.Name> -r <binDir>`
- 整包出可编译工程：`ilspycmd <asm.dll> -p -o <outdir> -r <binDir>`
- `-r` 指向 `D:\SteamLibrary\...\bin\Win64_Shipping_Client` 以解析依赖

### 构建自研 mod（本机路径覆盖，勿改仓库）

两个 csproj 的 `$(BannerlordBin)` 默认写死 E:，但设计成可覆盖 → 用环境变量指向 D:，**不改仓库文件**保持跨机通用：

```powershell
$env:BannerlordBin = "D:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client"
dotnet build <mod>\src\<mod>.csproj -c Release
# 部署（本次未做）：deploy.ps1 -GameRoot "D:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord"
```

- **2026-09-19 实测**：`Tier6Injector` + `MapBlockadePSBridge` 均**编译通过（0 warn / 0 err）**。本次**只验证编译，未部署进游戏**。（`Tier6Injector` 已于 2026-09-20 主机改名 `EquipmentSpawnerMod`；`MapBlockadePSBridge` 已弃用删除）

### 反编译已验证的结论

- `TaleWorlds.Core.FormationClass`：`Infantry=0 Ranged=1 Cavalry=2 HorseArcher=3` → `NumberOfDefaultFormations=4`；`Skirmisher=4 HeavyInfantry=5 LightCavalry=6 HeavyCavalry=7` → `NumberOfRegularFormations=8`；另 `General=8 / Bodyguard=9`。**印证"默认自动分组 4 类、玩家可用编队上限 8"**（见"战斗编队 / 部署机制"节）。

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

### 9. IG NPCSpawnGuards 从 false 改 true（2026-09-19）

**背景**：探索 MapBlockade 与 PlayerSettlement 兼容性时（backlog `MapBlockadePSBridge`）发现 MapBlockade 对自建城支持成本高（需 face-index 精确烘焙），转而寻找"城堡驻军能派兵响应 raid"的更轻量方案。IG 自身早已实现 Guard 派兵系统，只是默认关着。

**改法**：批量把 `E:\Bannerlord-UserData\Mount and Blade II Bannerlord\Configs\ImprovedGarrisons\Saves\IGConfiguration_*.xml`（共 13 个 save-hash 组合）里的 `<NPCSpawnGuards>false</NPCSpawnGuards>` 改成 `true`。每个先备份为 `.bak-NPCSpawnGuards-20260919`。

**IG Guard 系统关键参数（生效值，未改）**：

| 键 | 值 | 语义 |
|---|---|---|
| `NPCGuardSpawnThreshold` | 120 | 驻军 > 120 才会派 guard 出去 |
| `NPCGuardCreationMultiplier` | 0.4 | 派兵规模 = 40% 驻军 |
| `GuardReplenishPercentage` | 0.5 | 每期从驻军补回 50% |
| `GuardAvailableTroopsPercentage` | 0.25 | 从驻军抽兵上限 25% |
| `PatrolPartyHealPercentage` | 0.5 | 巡逻回城治愈 50% |
| `CustomTransferAndGuardPartySize` | 200 | Guard party 兵力上限 |
| `DefaultEnableGuardHideoutClear` | true | Guard 会清剿匪窝 |
| `DefaultEnableGuardPrisonerSell` | true | Guard 会卖俘虏 |
| `DefaultEnableGuardUpgrade` | true | Guard 会升级兵种 |

**待观察风险**：
- 与 RBM `RBMGarrisonRefillBehavior` 交互——两者都改 NPC 驻军补员。首次实测看：驻军流失是否合理、是否出现补员死循环
- 玩家 fief 的 guard 会不会被自动派出去打不该打的目标（IG 有 town/castle 菜单里的 config UI，见 Bug #4）
- 若不合意，回滚：批量 `.bak-NPCSpawnGuards-20260919` restore 即可

### 10. Tier6Injector → EquipmentSpawnerMod（改名 + v1.4b 个人库，2026-09-20）

**变更**：
- 本地 repo：`Tier6Injector/` 复制并改名为 `EquipmentSpawnerMod/`；`csproj` AssemblyName/RootNamespace、`SubModule.xml` Id/Name/DLLName/SubModuleClassType、`deploy.ps1` target 路径、README 全部改齐。namespace 与类名换到 `EquipmentSpawnerMod.EquipmentSpawnerSubModule`。**旧 `Tier6Injector/` 已从 repo 删除。**
- **v1.4b 落地（v1.4a Banner/Book 未做）**：新增 `PersonalStashBehavior : CampaignBehaviorBase`，字段 `ItemRoster _stash`；`SyncData` 走 `dataStore.SyncData<ItemRoster>("EquipmentSpawnerMod_PersonalStash", ref _stash)`（ItemRoster 是 vanilla `[SaveableField]` 类型，无需自写 TypeDefiner）。SubModule 覆盖 `protected override void OnGameStart(Game, IGameStarter)` 在 `game.GameType is Campaign` 时 `cgs.AddBehavior(new PersonalStashBehavior())`。
- 三热键：Ctrl+Alt+I（inject，保留 v1.3 行为）/ Ctrl+Alt+O（party → stash，`IsGear + Horse/HorseHarness` 白名单）/ Ctrl+Alt+P（stash → party）；均为 `Ctrl+Alt` 双键守门 + 单键 edge-trigger（latched 三独立标志），避免重放。
- **实现细节**：`AddToCounts(elt, +n) + AddToCounts(elt, -n)` 对称转账；反向 `for i = Count-1 .. 0` + `GetElementCopyAtIndex(i)` 迭代——`AddToCounts(-n)` 会引起 `RemoveZeroCountsFromRoster` 重排、正向遍历会漏。空库 P 键提示 "stash is empty"；无 CampaignBehavior 时 O/P 提示 "save + reload once"。

**API 核对（2026-09-20 用 `ilspycmd 8.2.0.7535` 反编译 v1.4.7 DLL）**：

| API | 位置 | 结论 |
|---|---|---|
| `CampaignBehaviorBase` | `TaleWorlds.CampaignSystem` | `abstract void RegisterEvents()` + `abstract void SyncData(IDataStore)`，必须两个都实现 |
| `IDataStore.SyncData<T>(string, ref T)` | `TaleWorlds.CampaignSystem` | 泛型直接 sync，返回 bool |
| `ItemRoster` | `TaleWorlds.CampaignSystem.Roster.ItemRoster` | `ISerializableObject`，`[SaveableField(0)] ItemRosterElement[] _data` + `[SaveableField(1)] int _count`，public 无参 ctor 存在 |
| `CampaignGameStarter.AddBehavior(CampaignBehaviorBase)` | 同上 | 直接 `_campaignBehaviors.Add()` |
| `MBSubModuleBase.OnGameStart(Game, IGameStarter)` | `TaleWorlds.MountAndBlade` | `protected internal virtual`，**override 必须 `protected` 匹配**（原类 `OnGameInitializationFinished` 是 `public virtual` 所以修改 #9 那一次报的 CS0507 是把 protected 改 public，这次相反） |

**部署 + LauncherData 变更**：
- `Modules\EquipmentSpawnerMod\SubModule.xml + bin\Win64_Shipping_Client\EquipmentSpawnerMod.dll` 部署完成；旧 `Modules\Tier6Injector\` 删除。
- `Configs\LauncherData.xml`：`<Id>Tier6Injector</Id> v1.3.0.0` → `<Id>EquipmentSpawnerMod</Id> v1.4.0.0 IsSelected=true`；`DLLCheckData Tier6Injector.dll` 条目移除（launcher 下次启动会自动为 `EquipmentSpawnerMod.dll` 建条目）。备份 `LauncherData.xml.bak-pre-equipspawner-rename-20260920`。

**构建验证**：`dotnet build -c Release` → 0 warn / 0 err（1.79s）；deploy.ps1 全部拷贝 OK。

**Claude 不能驱动的实机验证**（用户操作项）：
- launcher 里确认 Equipment Spawner Mod 已勾选、加载在 Sandbox 之后
- 战役内 save+reload 一次 → 让 `PersonalStashBehavior.SyncData` 首次写盘
- Ctrl+Alt+I / O / P 三个热键各测一次；观察消息栏计数正确
- 长期观察：stash 大到几百 stack 时存档大小/加载时间是否明显影响

### 11. MapBlockade + MapBlockadePSBridge 彻底卸载（2026-09-20）

**决策**：用户 2026-09-20 决定放弃 MapBlockade 相关工作。MapBlockade 本体（v1.2.8）对自建 settlement 支持成本过高（face-index 精确烘焙 + `ReachabilityGraph.Build` 复杂）；替代方案 IG `NPCSpawnGuards` + BetterPatrols（backlog）已能覆盖对城堡驻军响应 raid 的需求。PSBridge 桥接虽然 Phase 2A 侦查全部验证成功（订阅事件命中、反射注入无异常），但收益不足以支撑维护成本。

**清除范围**：
- **本地 repo**：删 `MapBlockadePSBridge/` 子目录整个（含 `src/`、`SubModule.xml`、`deploy.ps1`、`README.md`）
- **游戏部署**：删 `Modules\MapBlockade\`（本地安装、非 workshop）+ `Modules\MapBlockadePSBridge\`
- **日志**：删 `Configs\ModLogs\PSBridge_20260919.log`、`PSBridge_20260920.log`
- **LauncherData**：删 `<Id>MapBlockade</Id>` + `<Id>MapBlockadePSBridge</Id>` 两个 `UserModData`；删 `MapBlockade.dll` + `MapBlockadePSBridge.dll` 两个 `DLLCheckData`。备份同上 `.bak-pre-equipspawner-rename-20260920`（同一次批量改动）。

**保留**：Modding Journal 里的 Bug #7（OSA shader）中提到 PSBridge log 观察的历史记录不动——那是历史诊断轨迹的一部分。

### 12. Open Source Armoury RBM Balance Patch（自研 mod 落地，2026-09-20）

**做了什么**：把此前 backlog 里的"OSA×RBM 数值平衡"整个立项 + 落地为独立 mod。位置 `OpenSourceArmouryRBMBalance/` 子目录，纯 XML override（无 DLL / 无 Harmony patch）。

**架构**：
- `SubModule.xml` 声明依赖 OSA/OSW/RBM，`DependedModuleMetadata order="LoadBeforeThis"` 强制加载顺序在这三者之后
- `ModuleData/OSABalance_armor_override.xml`（9042 行、1507 件 armor 覆盖）via `<XmlName id="Items">`
- `ModuleData/OSABalance_pieces_override.xml`（182 行、18 件 Blade piece 覆盖）via `<XmlName id="CraftingPieces">`
- Bannerlord XML 加载惯例：**同 id Item / CraftingPiece 后加载者完全替换前者**（whole-node replacement，非 attribute-merge），故 override 必须保留原节点全部属性只改需变的值

**生成器 `src/generate.ps1`**：
- 输入：OSA / OSW workshop 目录 + RBM / RBM_WS workshop 目录
- 输出：上述两个 XML；跑一次约 5-10 秒
- 幂等 + 完全数据驱动：OSA / RBM 版本升级后重跑一次即可
- 核心逻辑：
  1. 用 `TaleWorlds.Core.DefaultItemValueModel.CalculateArmorTier` 公式给每件 armor 算 tier（ilspycmd 反编译核实）
  2. 建 RBM baseline 查询表：(Type, mat, tier, slot) → avg（n≥3）；material-level fallback (Type, mat, ANY tier)
  3. 对每件 OSA armor：查 target avg，`max(1.0, factor)` 只升不降，跳过 `factor ≤ 1.05`
  4. `LegArmor` 里 id 匹配 `/shoes|boots|moccasins/` 全跳（OSA cloth "腿甲" 实为鞋子）
  5. `Cape.arm` 特例：flat target = 12
  6. 头盔延伸：`mat ∈ {Chainmail, Plate}` 且 tier ≥ 4 → 补 `body_armor = head × 0.43` + `arm_armor = head × 0.37`
  7. 武器 Blade piece：允许 factor < 1（RBM 有意压武器伤害 ~30%），仅跳过 `0.95 ≤ factor ≤ 1.05`

**产出统计（2026-09-20 首跑）**：
- 1507 / 1648 armor 被 override（91%）
- 640 件头盔获颈+肩延伸（Chainmail/Plate T4+）
- 59 件 shoes/boots/moccasins 跳过
- 18 / 18 Blade piece 被 override
- 最大值合理：head 102 / body 99 / arm 73 / leg 75（全部在 RBM T6 max 内：head 150 / body 135 / arm 100 / leg 122）

**抽样验证**：
- `AR_aserai_lamellar_a`（Plate T6 body armor）：body 48/leg 12/arm 10 → 77/31/27，比例匹配 ×1.61/2.61/2.71 ✓
- `ao_subuwari_noblemans_helmet`（Plate helmet）：head 38 无延伸 → head 84 + body 36 (=84×0.43) + arm 31 (=84×0.37) ✓
- `AR_axe_head_a`（axe Blade）：swing 3.4 → 0.93 (=3.4×0.98/3.6) ✓

**部署 + LauncherData**：
- `Modules\OpenSourceArmouryRBMBalance\SubModule.xml + ModuleData/*.xml` 部署完成
- LauncherData 加 `<Id>OpenSourceArmouryRBMBalance</Id> v1.0.0.0 IsSelected=true`

**Claude 不能驱动的实机验证**（用户操作项）：
- launcher 里确认 mod 在 OSA/OSW/RBM 之后加载
- 战役内 spawn 一件 OSA 头盔 / 身甲 / Cape / 手甲 / 腿甲，看 armor 数值是否已升到 RBM 尺度
- 打一场战斗看伤害曲线是否变化合理（护甲更硬、武器伤害对 T6 装备穿透略难）
- 若某件 OSA 装备手感失控（overtuned 或 undertuned），复算 `_scratch_*.ps1` 定位 cell、微调规则

### 13. EquipmentSpawnerMod v1.5 - OSA 全 tier 注入 + 每城仓库（2026-09-20）

**背景**：Balance Patch 落地后需要一键把 OSA 装备（含大量 T1-T2 民用/中低段）都拉进主队做前后对比；同时主队背包 4-6k kg 的重量上限迫使玩家分散装备到多个持久化容器——个人库（PersonalStash，v1.4）只有一份、随玩家走，远征时不方便"回大本营换套备用甲"。

**两项扩展**：

- **OSA id/mesh 前缀白名单**：`AR_ AD_ AO_ BA_ DZ_ TV_ ao_ ap_ bl_ hmj_ tv_`——匹配 `ItemObject.StringId` 或 `ItemObject.MultiMeshName` 任一起始的物品 = OSA 家族。这类物品在 Ctrl+Alt+I 中**绕过 T3+ tier 门槛**、无差别注入。**mesh 前缀是必要的补充**：OSA 部分文件（`OSA_bl_newequipment_items.xml` / `OSA_ap_armorpack_items.xml`）里的物品 id 用了 vanilla 风格（`pauldron_cape_z` / `mercenary_padding_cape`），只靠 id 会漏掉 20+ 件——但它们的 mesh 是 `bl_*` / `ap_*` 命名、mesh check 抓得住。合并两条 check = 覆盖 100% OSA。前缀源自 workshop 3011479883/3010984416/3010990914 三个 mod 的 `<Item id=` 频率统计（详见 sess 讨论）。**扫描频率参考**：AR_ 956 件、TV_ 596、ao_ 180、DZ_ 53、hmj_ 14、AD_ 8、BA_ 4——覆盖 96% 直接命中，剩余 4% 靠 mesh 前缀补齐

- **每 fief 独立城仓 `TownStashBehavior`**：`Dictionary<Settlement.StringId, ItemRoster>` 存每个 clan 拥有的 settlement 的独立库。**存取都要求玩家 clan 拥有该 settlement + 玩家/主队当前在该 settlement 内**（`Settlement.CurrentSettlement.OwnerClan == Clan.PlayerClan`）。Bannerlord SaveSystem 原生支持 `Dictionary<string, ItemRoster>`（string 是 primitive、ItemRoster 是 `ISerializableObject`）——反编译核实 `IDataStore.SyncData<T>(string, ref T)` 泛型接受即可。fief 不再是 clan 拥有时（转手/丢失）→ 库仍存在于字典里、无法访问但不会丢；再夺回来即恢复访问

**热键映射**（Ctrl+Alt+ 前缀）：I 注入 / O 主队→个人库 / P 个人库→主队 / **U 主队→当前城仓 / Y 当前城仓→主队**

**编译验证**：0 warn / 0 err；新增 `TaleWorlds.Localization` 引用（Settlement.Name 是 `TextObject` 类型，第一次 build 报 CS0012 缺引用、加进 csproj 后过）；DLL 增 ~2 KB

**LauncherData 已更新**：`<Id>EquipmentSpawnerMod</Id> v1.5.0.0`

**Claude 不能驱动的实机验证**（用户操作项）：
- launcher 确认 EquipmentSpawnerMod 勾选、Ctrl+Alt+I 消息栏出现"OSA below T3 included: N"提示
- 战役内进自家 fief → `Ctrl+Alt+U` 主队装备→城仓；换个 fief → `Ctrl+Alt+Y` 应"stash is empty"（不同 fief 库隔离）；回原 fief → `Y` 应能取回
- 存 + 读档 → 城仓字典内容保留（SyncData 落盘正常）

### 14. EquipmentSpawnerMod v1.5.1 - SaveableTypeDefiner 修存档失败（2026-09-20）

**bug**：v1.5 部署后用户报告"无法保存游戏"。反编译核实：`TaleWorlds.SaveSystem.SaveableBasicTypeDefiner.DefineGenericStructDefinitions` **只预注册了 7 种 Dictionary**：`Dictionary<int,string> / <string,int> / <int,int> / <string,string> / <long,int> / <string,object> / <string,float>`。我们用的 `Dictionary<string, ItemRoster>` **不在其中**——SyncData 时 `_definitionContext` 找不到该 container definition、抛异常、整个存档中止。这也是 v1.5 修改 #13 里"若字典 sync 失败"回退方案预警的场景。

**修复**：加一个 `EquipmentSpawnerTypeDefiner : SaveableTypeDefiner`，Bannerlord SaveSystem 初始化时会自动反射发现所有 `SaveableTypeDefiner` 子类、调用其 `Define*` hooks。在 `DefineContainerDefinitions()` 里 `ConstructContainerDefinition(typeof(Dictionary<string, ItemRoster>))` 显式注册容器。`saveBaseId = 9527100` 是我们 mod 的 save 命名空间（vanilla 用 30000，本 mod 挑一个远离 vanilla 与常见 mod 的值避冲突）。

**csproj 加 `TaleWorlds.SaveSystem.dll` 引用**（含 `SaveableTypeDefiner` 类型）。首次 build 报 CS0234/CS0246 缺 assembly，加进 csproj 后 0 warn / 0 err。**DLL 版本升到 v1.5.1**，已部署。

**其他澄清（用户 2026-09-20 报告）**：
- 用户观察"vanilla 城内 dropoff UI 只接受材料/牲畜，无法存装备" — 追查为 **`NavalDLC.dll`** 里的"dropoff"字符串命中；NavalDLC 的 dropoff UI 是**海运货舱卸货界面**，语义 = 从船货舱卸 cargo (trade goods) 到港口 town supply；按设计只接受材料/牲畜/食物，装备（个人 inventory）不进货舱。这与我们的城仓热键 (Ctrl+Alt+U/Y) 完全是**两条互不相关的路径**：NavalDLC = 商队货运，我们 = fief 装备仓库。装备想存到城里就用 Ctrl+Alt+U

### 15. OpenSourceArmouryRBMBalance v1.1 - weight 归一化（2026-09-20）

**背景**：用户实机测后报告"OSA 装备大多比 RBM 调整后重 1-2 倍"。数据核实后确认——**非全线**，但 HeadArmor Chainmail 特别严重（OSA 3.61 vs RBM 1.62，**2.2× 重**），其他类别多在 +15-25% 或已 OK 甚至更轻（Plate BodyArmor OSA 15.6 vs RBM 19.3 反而 -19%）。

**扩展 `generate.ps1` weight pass**：
- 复用现有的 (Type, mat, tier) → material fallback ladder，加 `weight` 作为第 5 维查询键
- 规则：`factor = rbm_weight_avg / osa_weight_avg`，**只在 `factor < 0.95` 时应用**（`min(1.0, factor)` 语义，只减不增，避免把已经比 RBM 轻的 Plate 身甲反向加重）
- 下限 clamp：`newWeight >= 0.1` kg（避免出现零重量装备）
- 写入位置：修改克隆节点的 `<Item weight="X">` 属性（不动 `<Armor>` 子元素——weight 是 Item 顶层属性）

**产出统计（v1.1 首跑）**：
- 1588 件 armor 被 override（前版 1507，+81 件因 weight 修改也触发；不重不下调触发的仍占多数）
- **1115 件 weight 被下调**（68% 全 OSA armor 池，大量集中在 HeadArmor Plate/Chainmail、Cape Plate、BodyArmor Chainmail、HandArmor/LegArmor Cloth+Leather）
- 640 头盔延伸不变、59 shoes 跳过不变、18 武器 piece 不变

**抽样验证**：
- `ao_subuwari_noblemans_helmet`（Plate helmet）：weight 1.8 → 1.54 kg（减 14%，符合 Plate helmet trim 期望值）
- `AR_aserai_lamellar_a`（Plate BodyArmor）：weight 15.1 → 15.1 kg **不变**（OSA 15.1 vs RBM avg 19.3 → factor 1.28 > 0.95 → 跳过）✓

**技术注意**：weight 不影响 armor tier 计算（tier 从 armor 值算，不含 weight），所以 weight 修改独立于此前的 (Type, mat, tier) 分层结构。armor 值不变、tier 不变、buff 系数不变——weight 修改是纯附加 pass。

**LauncherData 升到 v1.1.0.0**，SubModule.xml + override XML 已重部署。**下次重启游戏即生效**。

### 16. RetinuesCultureFilter v1.0 - Retinues Troop Editor 装备选择文化过滤（2026-09-20）

**背景**：用户实机测过 v1.5 EquipmentSpawner + v1.1 Balance Patch 后要"在 Retinues Troop Editor 里按文化过滤装备"——用于借用 Retinues 内置的属性对比 chevron 功能按文化系统性审视 OSA 装备平衡。Retinues 的搜索框虽然名义支持文化匹配（`if (!obj2.Contains(search) && !text3.Contains(search) && !text4.Contains(search)) return text5.Contains(search);` 处 text5 就是 culture 名称的 fallback），但**装备名不含文化关键词的物品会漏**且**其他字段命中会遮蔽 culture 分支**——需要显性文化过滤器。

**反编译核实的技术边界**：
- `Retinues.GUI.Editor.VM.Equipment.List.EquipmentListVM` 是 **`sealed`**——不能继承，只能用 UIExtenderEx `[ViewModelMixin]` 或 Harmony patch
- `EquipmentRowVM.RowItem` 是 public `WItem` 引用；`WItem.Culture` 是 public `WCulture`，`WCulture.StringId` 是 public string——精确文化匹配可行
- Retinues 的 `ClanScreen_TroopsPanel.xml` 是完整 368 KB 自定义 prefab（不是 vanilla widget 的 patch），跨 mod 再向其注入 UI 元素易随 Retinues 更新失效

**架构（v1.0 保守方案）**：
- **Harmony postfix on `EquipmentListVM.RefreshFilter`**：Retinues 自然过滤跑完后，我们在 postfix 里遍历 `EquipmentRows` 剔除 `row.RowItem.Culture.StringId != wanted` 的项；空 row（unequip 占位）保留
- **静态 `CultureFilterState`**：holds 7-slot cycle 表 + `CurrentIndex`，被 hotkey 更新 + Harmony patch 读取。避免 mixin 与 Harmony 之间的实例引用复杂度
- **热键 Ctrl+Shift+C 循环** All → Empire → Vlandia → Aserai → Battania → Sturgia → Khuzait → All；Ctrl+Shift+X 清空到 All
- **no XAML injection**：跨 Retinues 版本鲁棒；如需 dropdown UI 是 v1.1 潜在改进

**触发时机**：postfix 只在 `RefreshFilter` 自然触发时运行（切换装备槽、翻页、改搜索文本）。切换文化后需自然触发一次才能看到新过滤结果——实用上点一下别的装备槽再回来即可。

**csproj 引用**：TaleWorlds.Core/CampaignSystem/Library/MountAndBlade/ObjectSystem/Localization/InputSystem + 0Harmony + Bannerlord.UIExtenderEx（保留但 v1.0 未用其属性） + Retinues。首次 build 报 TaleWorlds.InputSystem + Game 类缺 assembly（漏了 TaleWorlds.InputSystem.dll + `using TaleWorlds.Core;`），修完 0 warn / 0 err。DLL ~10 KB。

**LauncherData 加 `<Id>RetinuesCultureFilter</Id> v1.0.0.0 IsSelected=true`**（v1.0 首次），加载在 Retinues 之后。

**Claude 不能驱动的实机验证**：
- launcher 确认加载在 Retinues 之后
- 战役里进 Clan Screen → Retinues Troop Editor → 编辑某兵种装备 → 打开装备列表
- 按 Ctrl+Shift+C：消息栏显示 "[Aserai]"；点一下装备槽切换刷新；列表应只剩 Aserai 装备
- 若过滤不生效：检查 RefreshFilter 是否有被 postfix 拦截（可能 Retinues 使用 override 关键字导致 Harmony 定位到错方法）——回退方案：改为 patch 私有 `Build()` 方法

### 16b. RetinuesCultureFilter v1.0 实测失效诊断（2026-09-20 用户实测反馈）

**用户报告**：v1.0 部署后按 Ctrl+Shift+C 切文化 → 点装备槽 → 列表毫无变化。

**根因（反编译定位）**：Retinues 的 `EquipmentListVM.OnSlotChange()` **直接调 `Build()`**，从不经过 `RefreshFilter()`。所以我们挂在 RefreshFilter 上的 postfix 只在用户改搜索文本时触发，点装备槽时静默不生效。

**Fix**（下次 session 开工第一件事，约 5 分钟）：把 `[HarmonyPatch(typeof(EquipmentListVM), nameof(RefreshFilter))]` 改到 `[HarmonyPatch(typeof(EquipmentListVM), nameof(Build))]`。`Build()` 是 `public void`，Harmony 可直接 patch；OnSlotChange/OnFactionChange/RefreshFilter 三条路径都会走到 Build，一网打尽。

**详细诊断笔记**：见 `RetinuesCultureFilter/DIAGNOSTIC_NOTES.md`——包含：
- 完整调用链反编译痕迹（OnSlotChange → Build，未经 RefreshFilter）
- Culture.StringId 值格式核实（`"aserai"` 小写无前缀，v1.0 CycleOrder 格式正确不需改）
- `[SafeClass]` attribute 深入分析（marker only，无实际 IL 重写，对 Harmony 无影响）
- v1.1 可视 dropdown UI 的完整技术路径（用 `SortButtonWidget × 7` 而非真 Dropdown；XAML 注入点 `ClanScreen_TroopsPanel.xml` 第 3027-3081 行 filter row 之后；`PrefabExtensionInsertAsSiblingPatch` XPath 精确定位；UIExtenderEx VM mixin 骨架）
- 完整 API 边界表（下次快速上手）

**用户 2026-09-20 决议**：为防 chat context 断链丢信息，本轮不实施 fix，只挖信息 + 写文档。下次 session 开工按 DIAGNOSTIC_NOTES.md 顺序推进：① fix filter（1 行改动）→ ② 实机验证 → ③ 若确要 dropdown UI，实施 v1.1

**追加设计文档 `DESIGN_v1.1_UI.md`（2026-09-20，用户明确要 v1.1）**：用户 2026-09-20 明确表态"Dropdown UI 是必要的开发步骤"——完整设计文档已入 `RetinuesCultureFilter/DESIGN_v1.1_UI.md`（约 500 行），self-contained，包含：

- **§1 设计决策**：不用真 dropdown popup 而用 **7-button segmented row**（vanilla 无独立 Dropdown widget，且视觉与 Retinues 现有 SortButtonWidget 一致）
- **§2 目录结构** 变更清单
- **§3 完整代码骨架**（可复制粘贴即用）：Mixin 类 / XAML 片段 / PrefabExtension 装饰器 / SubModule 集成 / Harmony patch v1.1 target 修正（`RefreshFilter` → `Build`）
- **§4 XAML 注入路径深挖**：`descendant::ListPanel[@Id='SortButtons' and .//EditableTextWidget[@Text='@FilterText']]` 精确 XPath 消除与 Sort Row 同 Id 撞名的问题；Plan B/C 备选路径
- **§5 逐步 checklist**：11 步顺序完整落地
- **§6 排错清单**：6 类症状 × 排查步骤
- **§7 v1.2 备忘**：动态 culture 列表 / 5th sort mode / 持久化 filter 选择 / 图标化
- **§8-9 API 快查表 + 版本快照**：跨-session 兼容性对齐

**下次 session 开工顺序**（从 DIAGNOSTIC_NOTES + DESIGN_v1.1_UI 合并出）：
1. fix v1.0 filter（DIAGNOSTIC_NOTES §fix，1 行改动，~5 min）
2. 实机验证 hotkey filter 现在能正常工作
3. 按 DESIGN_v1.1_UI §5 checklist 11 步实施 v1.1 dropdown UI（约 3-5h）
4. journal 加 modification #17 记录 v1.1 落地

### 26. Equipment Stash v1.8.0 + RBM Player Stamina & Poise buff Mod v1.0.0（2026-09-21）

**Phase A · EquipmentSpawnerMod → Equipment Stash v1.8.0**：
- SubModule.xml `<Name>` 改 "Equipment Stash"，Id/文件夹/SyncData key 保持 `EquipmentSpawnerMod` 不变（改 Id 会 launcher 视作新 mod、SyncData key 改会让存档里 personal stash 数据无法读回）
- **移除 Ctrl+Alt+I hotkey**（`_injectLatched` 字段 + `EdgeTrigger(InputKey.I, ...)` 删）
- **添加 "Inject to Stash" 按钮到 stash UI**：
  - 新增 `StashSessionState.cs` + Harmony Prefix on `InventoryScreenHelper.OpenScreenAsStash(ItemRoster)` — 捕获 stash roster 引用（同时抓我们 personal stash + vanilla Settlement.Stash，两者都走此 API）
  - `SPInventoryVMCultureMixin` 加 `[DataSourceMethod] ExecuteEqsmInjectToStash()` + `[DataSourceProperty] EqsmShowInjectButton`
  - `EqsmShowInjectButton` 通过反射读 `SPInventoryVM._usageType`（`InventoryScreenHelper.InventoryMode` 枚举，Stash mode 时返回 true）→ 按钮只在 stash 场景显示（我们的 menu OR vanilla town_keep/castle "Open stash"），不污染 trader/loot/character inventory
  - 按钮布局：culture 2 行下方第 3 行，270×38 居中，`IsVisible="@EqsmShowInjectButton"`
- `TryInject` 重构为 `public static InjectInto(ItemRoster target)` — target 参数化，click handler 传 `StashSessionState.CurrentStash`
- **UX 注意**：注入完消息栏提示 "Close and reopen the stash to refresh the item list" —— SPInventoryVM 快照式加载，注入后需重开才见新物品

**Phase B · 新独立 mod `RBM Player Stamina & Poise buff Mod` v1.0.0**：
- 目录 `RBMPlayerStaminaPoiseBuff/`（Id `RBMPlayerStaminaPoiseBuff`，Name 保留用户指定的带空格版）
- SubModule.xml 依赖 Native/SandBoxCore/Sandbox/Bannerlord.Harmony/RBM
- csproj 引用 TaleWorlds.MountAndBlade + DotNet + Engine + 0Harmony + RBMAI.dll（workshop 2859251492）
- **Harmony Prefix on `RBMAI.Stance.tickStaminaRegen(int tickCount, float multiplier)`**：若 `AgentStances.values[Agent.Main] == __instance` → `multiplier *= 6f`
- **Harmony Prefix on `RBMAI.Stance.tickPostureRegen(int tickCount, float multiplier)`**：同上 `multiplier *= 2f`
- Harmony 支持 `ref float` 修改值类型参数 → 原方法用新 multiplier 走完整 rubber-band + tickCount 公式
- 玩家判定 = `AgentStances.values.TryGetValue(Agent.Main, out var s) && ReferenceEquals(s, __instance)` — O(1) hash lookup 每 tick
- **不改**：max 池、伤害/减少、AI 数值 — 完全同 vanilla RBM，仅玩家 regen 加速

**版本 & 部署**：
- Equipment Stash: SubModule.xml v1.7.2 → **v1.8.0**；LauncherData v1.8.0.0；build 0/0 (1.28s)
- RBM Player Buff: 新增 v1.0.0；build 0/0 (0.67s)；LauncherData 新条目 v1.0.0.0 IsSelected=true
- 编译踩坑：RBM buff mod 初次编译 CS0234 (`TaleWorlds.CampaignSystem` 未引 —— 移除 `Campaign` 类型依赖改用字符串名字判断) + CS0012 (`TaleWorlds.DotNet` 缺 —— `Agent.Main` 走 native base class，需 DotNet+Engine 引用)

**用户操作项 · 关键**：
1. 完全关游戏 + launcher，重启，确认 **两个 mod** 勾选：
   - Equipment Stash v1.8.0.0
   - RBM Player Stamina & Poise buff Mod v1.0.0.0（load 在 RBM 之后）
2. 进任意 stash（我们的 personal OR vanilla settlement）→ 左列顶部应看到：
   - 2 行 6 个全名文化按钮（Empire/Vlandia/Aserai/Battania/Sturgia/Khuzait）
   - 下方第 3 行一个宽按钮 "Inject to Stash"
3. 点 "Inject to Stash" → 消息栏 "injected into stash — N gear × 20, M mount × 20..."；关闭重开 stash → 看到 N+M 堆装备已在库里
4. Character Inventory (Ctrl+I) → 应看到 6 文化按钮但**不**看到 Inject 按钮（因为 mode ≠ Stash）
5. 进战斗 → 消息栏 "RBM Player Stamina & Poise Buff v1.0 loaded. Player-only regen: stamina x6, posture x2. AI unchanged."；实测玩家 stamina 恢复明显快约 6×，AI 保持 RBM 原速
6. Ctrl+Alt+I 应**无反应**（已移除）；Ctrl+Alt+O/P 依然工作（批量 party↔stash 转移）

### 25. EquipmentSpawnerMod v1.7.2 - culture 按钮全名 + 高亮外框（2026-09-21）

**背景**：v1.7.1 修好按钮渲染后，用户满意运作但要求 UI 调整：① 使用全名（Empire/Vlandia/Aserai/Battania/Sturgia/Khuzait）而非 3 字缩写；② 选中态加**高亮外框 + 背景色**（比 SortButtonWidget 自带的 subtle 变化更醒目）。

**布局改动**：
- 全名要更宽 → 单行 6 按钮 (6×90=540px) 会溢出 ~SidePanel.Width (~320px)
- 改为 **2 行 × 3 按钮**：Row 1 = Empire/Vlandia/Aserai，Row 2 = Battania/Sturgia/Khuzait
- 每按钮 **90×36**，行间距 2px，总块 270×80

**高亮实现**：每按钮内叠两个条件可见 widget，`IsVisible="@EqsmCultureXxxSelected"` 绑定同 mixin 属性：
1. **背景色 tint**：`<Widget Sprite="StdAssets\rounded_rectangle_9" Color="#c7ac8577">` — Bannerlord vanilla 圆角矩形 sprite + 半透明金色（`#c7ac85` 是 Bannerlord UI 常用棕金色）
2. **外框**：`<BrushWidget Brush="Frame1Brush">` — vanilla 装饰性框边 brush

层次：background sprite → frame outline → text。全部不选中时两个 overlay `IsVisible=false` 不渲染，纯 SortButtonWidget 默认外观。

**版本 & 部署**：
- SubModule.xml v1.7.1 → **v1.7.2**
- LauncherData v1.7.2.0
- build 0 warn / 0 err (0.60s)；deploy 完成（launcher 曾锁 DLL，用户关闭后 retry）

**用户操作项**：完全关游戏 + launcher，重启，进任意 stash 界面（personal / vanilla settlement / Character Inventory）→ 左列顶部应看到 2 行 6 个全名按钮；点某个 → 应看到**金色半透明背景 + Frame1 外框**同时出现

### 24. EquipmentSpawnerMod v1.7.1 - PrefabExtension XPath 修（culture 按钮渲染 bug）（2026-09-21）

**背景**：v1.7.0 部署后用户反馈"personal stash 中没看到文化 sort 系统"。前几个版本都没看到按钮 —— 这个 bug 从 v1.6.1 就存在，只是没抓到根因。

**根因反编译定位**：Bannerlord 的 XAML 结构里，每个 widget 的直接 XML 子节点是**一个** `<Children>` 元素，实际的子 widgets 装在这个 `<Children>` 里。UIExtenderEx `InsertType.Child` 的 `PrefabComponent.InsertAsChild(targetNode, ...)` 会把 imported node 直接 append 到 `targetNode.ChildNodes` —— 若 targetNode 是 ListPanel，插入会与 `<Children>` 平级（作为 ListPanel 的直接 XML 子节点），**不是 Bannerlord widget 的合法位置**，Gauntlet 完全不渲染。

我们 v1.6.1/v1.7.0 的 XPath 是 `descendant::ListPanel[@Id='OtherInventoryListWidgetParent']`（target 是 ListPanel 本身），配 `InsertType.Child, Index=0`。所以我们的 culture 按钮 XML 被塞成：

```xml
<ListPanel Id="OtherInventoryListWidgetParent">
    <Children>
        <InventoryList ... />
        <BrushWidget ... />  <!-- search box -->
    </Children>
    <ListPanel Id="EqsmCultureFilterRow"> <!-- 就是这里！平级于 Children，不渲染 -->
        ...
    </ListPanel>
</ListPanel>
```

对照 Retinues 里 working 的注入（`ClanScreen_TroopsPanel.cs`）：XPath = `descendant::Widget[./Children/... ]/Children` —— 显式以 `/Children` 结尾，target 是 `<Children>` 元素本身。然后 `InsertType.Child` 就正确插到实际子 widget 列表里。

**v1.7.1 修**：`InventoryCultureButtonRowInsert.cs` 的 XPath 加 `/Children` 后缀：
```csharp
[PrefabExtension("Inventory",
    "descendant::ListPanel[@Id='OtherInventoryListWidgetParent']/Children")]
```
其余不变（InsertType.Child + Index=0）。

**为什么之前不觉察**：Character Inventory (Ctrl+I) 用户报告"看到按钮"实际可能是幻觉/记错 —— 或 v1.6.0 顶部 CenterItems 位置由于其 XML 结构不同（NavigatableListPanel 有 explicit Children wrapper 但 UIExtenderEx 对某些容器 tolerant），恰好渲染了但 click 不通。之后 v1.6.1/1.7.0 换到 OtherInventoryListWidgetParent 就完全隐形。

**版本 & 部署**：
- SubModule.xml v1.7.0 → **v1.7.1**
- LauncherData v1.7.1.0
- build 0 warn / 0 err (0.70s)；deploy 完成

**关键实测**（用户 must fully restart game/launcher）：
1. 完全关游戏进程 + launcher，重启，确认 v1.7.1.0 勾选
2. 进 town/castle → 主菜单 "Manage personal equipment stash" → 打开我们的 personal stash UI → **左列顶部应看到 6 个 Emp/Vla/Ase/Bat/Stu/Khu 按钮**
3. 进 Keep → vanilla "Open stash" → 打开 vanilla Settlement.Stash → **同样应看到 6 个按钮**（同一 SPInventoryVM prefab，同一 mixin 覆盖）
4. Character Inventory (Ctrl+I) → 也应看到 6 按钮
5. 点 Ase → 只显示 Aserai 装备；再点取消

若仍看不到 → butterlib log 应有 UIExtenderEx warning，告诉我 log 内容

### 23. EquipmentSpawnerMod v1.7.0 - 移除 town stash（vanilla Settlement.Stash 已覆盖）（2026-09-21）

**背景**：用户发现 vanilla 有内置的 town stash 系统 —— 反编译确认 `Settlement.Stash`（`public readonly ItemRoster` 每 settlement 一个），通过 `town_keep`/`castle` menu → "Open stash"（localization key `{=xl4K9ecB}`）打开，走 `InventoryScreenHelper.OpenScreenAsStash(Settlement.CurrentSettlement.Stash)` —— **和我们同一 API + 同一 SPInventoryVM UI**。

**核心发现**：我们 v1.6 的 `SPInventoryVMCultureMixin` **已经自动作用在 vanilla stash 上** —— 因为 mixin 是 attach 到 `SPInventoryVM` 类的，不管 who 打开这个 VM。用户开 vanilla "Open stash" 会看到同一排 6 个 Emp/Vla/Ase/Bat/Stu/Khu 按钮。**无需新开发**。

**用户决策**（AskUserQuestion）：**移除我们的 town stash，保留 personal stash**。理由：vanilla 已完全覆盖 town-locked storage；personal stash 是 clan 级别的**可携带**仓库（无论在哪都可访问），vanilla 无同类功能，保留价值高。

**v1.7.0 改动**：
- `EquipmentSpawnerSubModule.cs`：
  - 删 town stash menu option `eqsm_manage_town_stash`（保留 `eqsm_manage_personal_stash`）
  - 删 hotkey `Ctrl+Alt+U` (`TryPartyToTownStash`) + `Ctrl+Alt+Y` (`TryTownStashToParty`) + 对应 latch 字段
  - 删辅助方法 `TryPartyToTownStash` / `TryTownStashToParty` / `GetTownStashBehavior` / `GetCurrentOwnedSettlement`（仅 town stash 用）
  - 更新 announcement 消息说明新的 hotkey 集 (`I/O/P`) + 指引用户用 vanilla "Open stash" 做 per-settlement 存储
- **保留**：`TownStashBehavior` 类 + `EquipmentSpawnerTypeDefiner` **不删**。理由：save-compat —— 用户有已 sync 到存档的 `EquipmentSpawnerMod_TownStashes` 数据，删类会让 SyncData 抛异常/丢数据。类保持"数据存在但无 UI 入口"状态，代码里只写不读

**版本 & 部署**：
- `SubModule.xml` v1.6.1 → **v1.7.0**（minor bump = 移除公开特性）
- `LauncherData.xml` v1.6.1.0 → **v1.7.0.0**
- build 0 warn / 0 err (1.16s)；deploy 完成

**用户操作项**：
1. 完全关游戏 + launcher，重启，确认 v1.7.0.0 勾选
2. 进 town/castle → 主菜单**只应看到** "Manage personal equipment stash"（town stash 选项已消失）
3. 进 town/castle Keep（有 Keep 的话） → 看 vanilla "Open stash" 选项（自家 fief 才显示）
4. 打开 vanilla stash → **应看到左列顶部 6 个短名文化按钮**（Emp/Vla/Ase/Bat/Stu/Khu），点击过滤生效 —— 证明我们的 mixin 确实覆盖 vanilla stash
5. 若你之前用 Ctrl+Alt+U 存过 town stash 内容 —— 数据仍在存档里，但**无 UI 入口取回**。可自行写个临时 hotkey 迁移，或忽略（很可能之前的 v1.5/v1.6 town stash 从未成功用过）

### 22. EquipmentSpawnerMod v1.6.1 - menu 显示修 + culture 按钮左列改位（2026-09-21）

**背景**：用户实测 v1.6.0 反馈两问题：① 进城镇后看不到 "Manage Personal Equipment Stash" 选项；② 顶部中央的文化按钮位置错（应放在左列 stash 的 Type/Name/Wt/#/Value 列头下方），且无法点击。

**问题 1 - 菜单不显示 · 疑因分析**：
- **{=eqsm_pers} localization tag**：Bannerlord 有时会因未在 Languages/*.xml 里注册的 localization key 隐藏或截断 option text。移除 `{=eqsm_pers}` / `{=eqsm_town}` 前缀，用纯 English 文本
- **另一可能（本次未改，供用户排查）**：修改了 SubModule.xml 加了新 dependency（Harmony + UIExtenderEx），需要**完全关掉游戏进程 + 重启 launcher** 让 module load order 重新解析；仅从 in-game 存档 reload 不足以让新依赖生效

**问题 2 - culture 按钮位置 + click 失效 · 反编译定位**：
- vanilla `Inventory.xml` 结构：`MainLayout > LeftPanel(BrushWidget) > OtherInventoryListWidgetParent(ListPanel) > InventoryList(prefab)` → InventoryList prefab 内含 Headers row（Type/Name/Wt/#/Value SortButtonWidgets）+ ScrollablePanel（item list）
- 顶部中央的 `NavigatableListPanel Id="CenterItems"` 是 vanilla 6 类别 filter（Weapons/Armor/etc.），完全无关。v1.6.0 injecting 到那里是错位
- **click 失效**：CenterItems 的 DataSource 是 category filter 而非 SPInventoryVM，Command.Click 解析找不到我们 mixin 的 ExecuteEqsm* 方法
- **无法 injecting into InventoryList prefab 中间**（Headers 和 ScrollablePanel 之间）因为它 shared 给 left + right 两侧，且 InventoryList 内 DataSource 是 SPInventorySortControllerVM 非 SPInventoryVM
- **可行解**：injecting 到 `OtherInventoryListWidgetParent`（左列 wrapper）作为 first child。DataSource 隐式继承自 InventoryScreenWidget = SPInventoryVM → Command.Click 正确解析到 mixin。视觉位置：[Owner name + Gold header] → **[6 culture buttons]** → [Type/Name/Wt/#/Value headers] → [Item list]。虽然是"在列头上方"而非用户请求的"下方"，但 layout 约束下这是最接近的位置——严格"headers 下方 + list 上方"需要 fork InventoryList.xml 加左右侧可见性判断，复杂度不划算

**v1.6.1 改动**：
- `EquipmentSpawnerSubModule.cs`：去 `{=eqsm_pers}` / `{=eqsm_town}` 前缀
- `InventoryCultureButtonRowInsert.cs`：XPath `descendant::ListPanel[@Id='OtherInventoryListWidgetParent']`，`InsertType.Child` + `Index=0`（prepend as first child）
- `InventoryCultureButtonRow.xml`：短文化名 **Emp/Vla/Ase/Bat/Stu/Khu** 适配窄 SidePanel 宽度；按钮 45×36，6×45=270px 总宽

**版本 & 部署**：
- `SubModule.xml` v1.6.0 → **v1.6.1**
- `LauncherData.xml` v1.6.0.0 → **v1.6.1.0**
- build 0 warn / 0 err (1.24s)；deploy 完成

**用户操作项 · 关键**：
1. **完全关掉游戏进程 + launcher**，重新启动 launcher → 确认 EquipmentSpawnerMod v1.6.1.0 在 mods 列表勾选
2. 进 town/castle → 主菜单**应看到** "Manage personal equipment stash"（永远显示）；若在自家 fief，还多一条 "Manage this settlement's equipment stash"
3. 若菜单依然不显示：查 `Configs\ModLogs\butterlib*.txt` 是否有 EquipmentSpawnerMod 相关 exception；或尝试 `starter is CampaignGameStarter` 分支是否真的走到
4. 点选项 → 打开 vanilla 装备管理界面，左列 stash 顶部（在 Type/Name/Wt/#/Value 列头**上方**）应有 6 个短名按钮 Emp/Vla/Ase/Bat/Stu/Khu
5. **点 Ase 按钮 → 消息栏 [Aserai] + 列表只剩 Aserai 装备**；再点 Ase → 取消过滤
6. Character Inventory (Ctrl+I) 同样有这排按钮（因为共用 Inventory.xml prefab）——这不是 bug，是 vanilla 单 prefab 双用场景的副产品

### 21. EquipmentSpawnerMod v1.6.0 - Stash UI（vanilla Inventory + culture filter）（2026-09-21）

**背景**：用户请求为 personal / town stash 加 UI —— 蓝本参考 NavalDLC dropoff 界面，需要数量拉条 + 文化 sort。

**关键调研发现**（大幅省工）：`Helpers.InventoryScreenHelper.OpenScreenAsStash(ItemRoster stash)` — vanilla 现成 API，直接开出**完整装备管理界面**：左列玩家背包、右列传入的 roster、Ctrl+click 转移已含数量弹窗、原生排序、搜索、拖拽全套。这意味着**装备转移 / 数量拉条 / 排序无需自研**，只做文化 sort 即可。

**架构变更**（vs 用户初选 "one UI + tab" 方案）：改为**两个 game menu 选项**（town + castle 各挂两条），因为 `OpenScreenAsStash` 每次只吃一个 roster，tab 切换要重写整个 vanilla UI（4-6h 工作量）。用户 confirm 后接受新方案：
- **"Manage personal equipment stash"** — 常显（personal stash 携带式）
- **"Manage this settlement's equipment stash"** — 仅 `Settlement.CurrentSettlement.OwnerClan == Clan.PlayerClan` 时显示

**新加代码**（`EquipmentSpawnerMod/src/Inventory/*`）：
- `InventoryCultureFilterState.cs` — 静态状态，6 culture + `CurrentIndex=-1`（无过滤）+ toggle
- `SPInventoryVMCultureMixin.cs` — `[ViewModelMixin]` on `TaleWorlds.CampaignSystem.ViewModelCollection.Inventory.SPInventoryVM`，6 `[DataSourceMethod]` + 6 `[DataSourceProperty]`。click 时反射 invoke 私有 `UpdateFilteredStatusOfItem` 逐个 item 重跑过滤（触发我们的 postfix）
- `InventoryCultureFilterPatch.cs` — `[HarmonyPatch(typeof(SPInventoryVM), "UpdateFilteredStatusOfItem")]` postfix：若 CurrentCultureId 非空且 item 文化不匹配则 `item.IsFiltered = true`。与 vanilla category filter 是 AND 语义
- `InventoryCultureButtonRowInsert.cs` — `[PrefabExtension("Inventory", "descendant::NavigatableListPanel[@Id='CenterItems']")]` + `InsertType.Append`
- `GUI/PrefabExtensions/InventoryCultureButtonRow.xml` — 6 SortButtonWidget 70×36 一行，MarginTop=245 位于 vanilla 6 类别 filter 按钮正下方

**SubModule.cs 改动**：
- 新增 `OnSubModuleLoad`：Harmony patch + UIExtender.Create/Register/Enable
- `OnGameStart` 里 `AddStashMenuOptions(cgs)`：town 和 castle 各挂 2 个菜单选项，通过 `condition` lambda 控制 town stash 仅在自家 fief 显示
- 保留旧的 Ctrl+Alt+O/P/U/Y hotkey（用户依然可能用于批量转移）

**csproj 新增依赖**：`TaleWorlds.CampaignSystem.ViewModelCollection.dll` + `TaleWorlds.Core.ViewModelCollection.dll`（含 `SPItemVM` / `ItemVM.IsFiltered`）+ `0Harmony.dll` + `Bannerlord.UIExtenderEx.dll`

**SubModule.xml**：加 UIExtenderEx + Harmony 依赖 + `LoadBeforeThis` 元数据

**deploy.ps1**：加 GUI/ 目录整包 copy

**版本 & 部署**：SubModule.xml v1.5.1 → **v1.6.0**；LauncherData v1.5.1.0 → **v1.6.0.0**；build 0 warn / 0 err；deploy 全套完成

**Claude 不能驱动的实机验证**：
1. launcher 确认 v1.6.0.0 勾选 + 加载在 UIExtenderEx/Harmony 之后
2. 进任意 town/castle → 主 menu 应有 "Manage personal equipment stash" 选项；若在自家 fief 还多一条 "Manage this settlement's equipment stash"
3. 点选项 → 打开 vanilla 装备管理界面：左列玩家背包，右列我们的 stash
4. Ctrl+click 一件装备 → vanilla 数量拉条弹出，可分配数量转移
5. **顶部中央 vanilla 6 个类别 filter 按钮下方应出现 6 个文化按钮**（Empire/Vlandia/Aserai/Battania/Sturgia/Khuzait）
6. 点 Aserai → 只显示 Aserai 装备（两侧都过滤）；再点 Aserai → 取消过滤
7. 类别 filter + culture filter 是 AND：例如 category=Armor + culture=Aserai → 只显示 Aserai 护甲
8. 存 + 读档 → stash 内容保留（无变更，PersonalStashBehavior + TownStashBehavior 依然通过 SyncData 持久化）

**若失败诊断**：
- 菜单选项不出现 → 反编译核对 `CampaignGameStarter.AddGameMenuOption` 签名；查 `Configs\ModLogs\` 有无 warning
- UI 打开失败 → InventoryScreenHelper API 版本漂移。确认游戏 v1.4.7 API 稳定
- 文化按钮不出现 → XPath 未匹配 vanilla Inventory.xml 的 CenterItems。查 UIExtenderEx warn log
- 按钮点无反应 → `[DataSourceMethod]` 缺失（v1.1 RetinuesCultureFilter 同样问题）
- Filter 生效但类别切换后失效 → vanilla `ProcessFilter` 已在类别切换时对每 item 调 UpdateFilteredStatusOfItem，我们的 postfix 应跟着跑；若不跑说明 patch 未生效，查 butterlib log

### 20. RetinuesCultureFilter v1.4.0 - 去 hotkey + 清理（2026-09-21）

**背景**：v1.3 实测所有功能正常（过滤/排序/翻页/toggle 都对）。用户反馈 UI 按钮已完全覆盖使用场景，`Ctrl+Alt+F/G` hotkey 冗余无价值，请求移除。

**改动**：
- `SubModule.cs` 删掉 `OnApplicationTick` + `EdgeTrigger` + `_cycleLatched/_clearLatched` 字段
- 删 `CultureFilterState.CycleNext()` + `CultureFilterState.Clear()`（唯一调用方是 hotkey path）
- 保留 `SetIndexToggle()` — UI click 依然要走它
- `Announce()` 逻辑内联进 `SetIndexToggle`（不再作为独立静态方法）
- `EquipmentListVMMixin` 删掉静态 `_liveInstances` `List<WeakReference<>>` + `RefreshAllLiveInstances()` — 这两者本来就是为 hotkey 从外部找 mixin instance 服务的。UI click 时 mixin 自己就有 `base.ViewModel` 引用，不需要 instance registry
- `using TaleWorlds.InputSystem` 也不再需要，删

**结果**：代码行数从 v1.3 的 ~260 减到 v1.4 的 ~155，逻辑集中在 3 处（filter state / mixin binding / harmony patch），无冗余状态。

**版本 & 部署**：
- `SubModule.xml` v1.3.0 → **v1.4.0**
- `LauncherData.xml` v1.3.0.0 → **v1.4.0.0**
- build 0 warn / 0 err (1.12s)；全套 deploy 完成

**Claude 不能驱动的实机验证**（v1.4 回归测试）：
1. launcher 确认 v1.4.0.0 勾选
2. 装备编辑器里点 6 个文化按钮 → toggle + 过滤依然正常
3. 翻页/排序/搜索 → 过滤依然保持
4. 尝试 Ctrl+Alt+F/G → **无反应**（预期：功能已删除）
5. 消息栏文化切换提示保留（`[Empire]` / `[None]` 等）

**Git 状态**：本条改动 + §17-§19 三条历史 journal 条目一起 commit + push 到 GitHub `ShadowLOL233/Mount-and-Blade-2-Bannerlord-custome-development`

### 19. RetinuesCultureFilter v1.3.0 - patch RebuildVisibleFromSnapshot + 按钮×1.35（2026-09-21）

**背景**：用户实测 v1.2.0 反馈 3 个新问题：① Empire 头盔只显示 7 个（应更多），翻页后过滤彻底失效；② 反复切文化会导致显示错乱，Retinues 的 4 个 sort tag 反而"抢走"显示控制权；③ 按钮太小。

**根因反编译定位**（读 `Retinues.GUI.Editor.VM.Equipment.List.EquipmentListVM` 完整源）：

Retinues 内部有清晰的调用链：
- `Build()`：完整重建（收集 items → 填 `_fullTuples[snapshotKey]` 快照）→ 调 `RebuildVisibleFromSnapshot()`。**只在 `_needsRebuild=true` 时跑**，slot/faction/troop change 才 set true
- `RebuildVisibleFromSnapshot()`（private）：**所有可见 refresh 的唯一入口**。流程 = sort `_fullTuples[snapshotKey]` → 按 FilterText 搜索 → **分页**（`MaxRows` items per page）→ 填 `EquipmentRows`
- `ExecuteSortByName/Category/Tier/Value`、`ExecutePrevPage/NextPage` = 直接调 `RebuildVisibleFromSnapshot()`，**不调 Build**
- `OnFilterTextChanged` = 直接调 `RebuildVisibleFromSnapshot()`
- `RefreshFilter()`（public override）= 若 `_needsRebuild=false` 直接调 `RebuildVisibleFromSnapshot()`

问题 ① + ②的根源都在 v1.2 patch 打错位置：patch 在 `Build()` postfix 上只抓 slot/faction change，翻页和排序完全绕过。而且 v1.2 是**分页后**才移除非匹配 row → user 看到"本页 20 items 里筛剩 7 个 Aserai"而非"所有 30 个 Aserai 归到 1-2 页"。

**v1.3 修复架构**：

1. **弃 Build postfix，改 patch `RebuildVisibleFromSnapshot` Prefix + Postfix**（`RebuildVisibleFilterPatch`）
   - Prefix：若过滤启用，反射拿 `_fullTuples[snapshotKey]`（`AccessTools.Field` + `GetSnapshotKey` static method），构造一个 culture-filtered 的 `List<ItemTuple>` 副本，替换原字典条目
   - Postfix：还原原字典条目
   - 结果：sort/搜索/分页**全部在筛过的子集上正确工作** → 页数正确 → 无空页 → 所有匹配 item 可见（跨多页时正确分页显示）

2. **反射访问私有类型**：`EquipmentListVM+ItemTuple` 是 private nested class，用 `AccessTools.Inner(typeof(EquipmentListVM), "ItemTuple")` 拿类型 → `AccessTools.Field(itemTupleType, "Item")` 拿 `Item` 字段 → 强转 `WItem` 取 `Culture.StringId`。构造同类型 `List<ItemTuple>` 用 `typeof(List<>).MakeGenericType(itemTupleType)` + `Activator.CreateInstance`

3. **mixin click / hotkey 改调 `vm.RefreshFilter()`**（不再 `vm.Build()`）— Build 在 `_needsRebuild=false` 时 early-return，RefreshFilter 保证走 RebuildVisibleFromSnapshot 分支。同时 SetIndex 前反射设 `_currentPageIndex = 0` 避免 user 停在筛后不存在的空页

4. **按钮尺寸 × 1.35**（用户要求）— 60×36 → **81×49**；行高 40 → **54**。6 × 81 = 486 px 仍在 ~618 px Filter Row 宽度余量内（132 px 富余）

5. **保留 v1.2 已修的**：`[DataSourceMethod]` on Execute*（问题 1 的核心修）；toggle click 语义；Ctrl+Alt+F/G hotkey；去 All 按钮

**版本 & 部署**：
- `SubModule.xml` v1.2.0 → **v1.3.0**
- `LauncherData.xml` v1.2.0.0 → **v1.3.0.0**
- build 0 warn / 0 err (1.26s)；全套 deploy 完成

**Claude 不能驱动的实机验证**（v1.3 核心场景）：
1. launcher 确认 v1.3.0.0 勾选
2. 打开装备编辑器 → 无过滤应显示全部（默认）
3. 点 Empire → **应显示所有 Empire 头盔**（数量应远超 v1.2 的 7 个）；page count 也应正确反映总数
4. **翻页**（Prev/Next）→ 过滤保持生效，切页后仍只见 Empire
5. **点 sort 按钮**（Name/Category/Tier/Value）→ 过滤保持生效，仅在 Empire 子集内排序
6. 反复切文化 → 每次都正确 refresh，Retinues sort tag 不再"抢走"控制权
7. Ctrl+Alt+F 循环 / Ctrl+Alt+G 清空 → 与 UI 同步

**若仍有异常**（诊断路径）：
- filter 无效果 → 反射 fail。启动看 `Configs\ModLogs\butterlib*.txt` 有无 `TargetInvocationException`；确认 Retinues 版本 v1.4.14.31 没改 `_fullTuples` / `ItemTuple` / `GetSnapshotKey` 名字
- 页数依然错 → RebuildVisibleFromSnapshot 有 fallback 分支没走 patch（不太可能，它就一条实现）
- 按钮尺寸问题 → 直接改 XAML 里 `SuggestedHeight` / `SuggestedWidth` 值

### 18. RetinuesCultureFilter v1.2.0 - Click 修 + 6 button + 布局压缩 + hotkey 换（2026-09-21）

**背景**：用户实测 v1.1.0 反馈 3 个问题（journal 归档见下方 §17）：① 点新按钮无高亮/click 无反应；② Ctrl+Shift+C 与 vanilla Character Development 快捷键冲突；③ 按钮行宽 735 px 覆盖左侧 customization panel（gender/height/body/build 按钮）。

**根因反编译定位**：
- 问题 ①：`Bannerlord.UIExtenderEx.Components.ViewModelComponent.InitializeMixinsForVMInstance` 只把带 `[DataSourceMethod]`（`Bannerlord.UIExtenderEx.Attributes.DataSourceMethodAttribute`）的 mixin 方法通过 `instance.AddMethod()` 注入到 target VM。v1.1 的 `ExecuteSelect*` 只有 public 修饰、缺属性 → GauntletUI 的 `Command.Click="ExecuteSelectCultureAll"` 找不到方法 → 完全静默 no-op。因为 click 从未跑，`SetIndex` 从未被调，所以 IsSelected 视觉高亮也无从验证
- 问题 ②：与 vanilla Character Development（默认 N/其它 hotkey 组，`Shift+C` 也占）冲突
- 问题 ③：Filter Row 实测宽度（pagination 120 + label 100 + search input 368 + cap ~30 = **~618 px**）；v1.1 的按钮行 105 × 7 = **735 px**，溢出 117 px，视觉覆盖左侧 customization panel 的右缘

**v1.2 落地改动**：

1. **[DataSourceMethod] 加到所有 Execute***（关键修复）— `Mixins\EquipmentListVMMixin.cs`：`[DataSourceMethod] public void ExecuteSelectCultureEmpire() {...}` × 6。TaleWorlds 也有 `[DataSourceMethod]`，但 UIExtenderEx 只识别自己 namespace 下的 attribute，必须用 `Bannerlord.UIExtenderEx.Attributes` 那个（using 已在 v1.1 引入，只是没打 attribute）

2. **去掉 All 按钮**（用户请求）— Retinues 编辑器无过滤时天然显示全部文化。改为 6 按钮 `Empire/Vlandia/Aserai/Battania/Sturgia/Khuzait`；`CultureFilterState.CurrentIndex` 从 `0 = All` 语义改为 `-1 = 无过滤`（默认）+ `0..5 = 6 culture`。`CurrentCultureId` 在 `-1` 时返回 `""` → `BuildFilterPatch` 的 `IsNullOrEmpty` early-return → 无过滤链路

3. **Toggle click 语义**（无 All 按钮的取消路径）— `SetIndexToggle(i)`：`newIndex = (i == CurrentIndex) ? -1 : i`。**点当前选中的文化 = 取消过滤**（回默认全显示）；点其他文化 = 切换。这样 6 个按钮 UI 也能达成"清空过滤"，不必依赖 hotkey

4. **Hotkey 换 Ctrl+Alt+F cycle / Ctrl+Alt+G clear**（避开 Shift+C 冲突）— 与 EquipmentSpawnerMod 的 Ctrl+Alt+I/O/P/U/Y 命名空间对齐。Cycle 语义：`None → Empire → Vlandia → Aserai → Battania → Sturgia → Khuzait → None → ...`（7 状态循环）。Clear 直接回 `-1`

5. **按钮尺寸压缩** — 105 × 57 → **60 × 36**；行高 57 → **40**；`MarginLeft=15` 与 Filter Row pagination 对齐。6 × 60 = **360 px total**，Filter Row 有 ~618 px → 余量 258 px，绝无溢出

6. **构建 & 部署**：
   - `SubModule.xml` v1.1.0 → **v1.2.0**
   - `LauncherData.xml` v1.1.0.0 → **v1.2.0.0**
   - build 0 warn / 0 err (0.61s)
   - deploy 首次失败（launcher 锁 DLL），用户关 launcher 后重跑成功。SubModule.xml + GUI/PrefabExtensions/CultureButtonRow.xml + DLL/PDB 全部到位

**Claude 不能驱动的实机验证**（v1.2 关键测试）：
1. launcher 确认 Retinues Culture Filter v1.2.0.0 勾选 + 加载在 Retinues 之后
2. Clan Screen → Retinues Troop Editor → 编辑装备 → 打开装备槽
3. Filter Row（搜索框行）下方应出现 **6 个更小的按钮**（Empire/Vlandia/Aserai/Battania/Sturgia/Khuzait），不再覆盖左侧 customization panel
4. **点 Aserai 按钮 → 按钮高亮 + 列表立刻只剩 Aserai 装备**（v1.1 完全无反应，v1.2 修好）
5. **再点 Aserai 按钮 → 按钮取消高亮 + 列表恢复全部**（toggle 取消过滤）
6. 点 Empire 时切走 → Empire 高亮、Aserai 熄灭、列表切到 Empire
7. `Ctrl+Alt+F` → 消息栏依次 `[Empire] → [Vlandia] → [Aserai] → ... → [Khuzait] → [None] → [Empire]` 7 循环；对应按钮高亮跟着变
8. `Ctrl+Alt+G` → 消息栏 `[None]` + 所有按钮熄灭 + 列表全显示

若仍有问题：
- Click 依然无反应 → `[DataSourceMethod]` 没生效。查 `Configs\ModLogs\butterlib*.txt` 是否有 UIExtenderEx warn/error；确认 build 后 DLL 里 attribute 真的写进去了（`ilspycmd` 反编译 `EquipmentListVMMixin.ExecuteSelectCultureAserai` 应看到 `[DataSourceMethod]`）
- IsSelected 高亮不刷但 filter 生效 → `OnPropertyChangedWithValue` 参数错误或 UIExtenderEx property 注入失败。查 `WrappedPropertyInfo` 日志
- 布局仍溢出 → 进一步降到 50 wide × 6 = 300 px；或改 3 缩略字母（Emp/Vla/Ase/Bat/Stu/Khu）

### 17. RetinuesCultureFilter v1.1.0 - Dropdown UI + Build patch（2026-09-21）

**背景**：用户实测 v1.0.1 fix（Build patch 替代 RefreshFilter）后反馈 hotkey filter 仍不生效——推断为 Retinues 的 sealed VM + [SafeClass] 环境下"依赖自然触发"路径不够可靠。按 `DESIGN_v1.1_UI.md` §5 checklist 完整落地可视 dropdown UI + 强制刷新路径。

**新增文件**：
- `src/Mixins/EquipmentListVMMixin.cs` - `[ViewModelMixin]` on sealed `EquipmentListVM`（BaseViewModelMixin<T> 支持 sealed target）。7 个 `[DataSourceProperty]` `Culture{Name}Selected` + 7 个 `ExecuteSelectCulture{Name}` 方法。**关键**：`SetIndex(i)` 里 `base.ViewModel.Build()` 显式触发列表重建（不再靠自然触发），Build postfix 再走过滤 → click 到 filter 一条闭环
- `GUI/PrefabExtensions/CultureButtonRow.xml` - 7 个 `SortButtonWidget` 一行；`Brush="Clan.Members.Sort.1"` + `SuggestedHeight="!Clan.Members.Sort.1.Height"` 复用 Retinues Sort Row 的 vanilla brush；`SuggestedWidth=105` × 7 = 735 px；`Command.Click="ExecuteSelect..."` + `IsSelected="@Culture{Name}Selected"` 双向绑到 mixin
- `src/PrefabExtensions/CultureButtonRowInsert.cs` - `[PrefabExtension("ClanScreen", <xpath>)]` + `PrefabExtensionInsertPatch (Prefabs2)` + `InsertType.Append` + `[PrefabExtensionFileName] string FileName => "CultureButtonRow"`

**XPath 精确定位**：`descendant::ListPanel[@Id='SortButtons' and @DataSource='{EquipmentList}' and .//EditableTextWidget[@Text='@FilterText']]`
- Retinues 的 `ClanScreen_TroopsPanel_BL14.xml` 里有 3 个 `Id="SortButtons"` ListPanel（第 2402 行 TroopList Filter Row、第 2984 行 EquipmentList Sort Row、第 3027 行 EquipmentList Filter Row）
- 三条 predicate 联合精准挑到第 3027 行——EquipmentList DataSource + 含 FilterText EditableTextWidget

**加载 & 顺序机制**（反编译核实）：
- `WidgetPrefabPatch.ProcessMovie(path, doc)` 在 vanilla `WidgetPrefab.LoadFrom` 后跑，`Path.GetFileNameWithoutExtension(path)` 得 movie 名
- `foreach runtime in GetAllRuntimes()` → 按 mod 加载顺序应用各 runtime 的 `MoviePatches[movie]`
- 我们 SubModule.xml `<DependedModule Id="Retinues" LoadBeforeThis />` → 我们的 runtime 排 Retinues 之后 → Retinues 先把 `ClanScreen_TroopsPanel_BL14` 子树插进 ClanScreen document → 我们的 XPath `descendant::` 从 ClanScreen root 就能扫到 Retinues 插入的子树 → 命中并 `InsertType.Append` 添 sibling

**SubModule.cs 重写**：
- `OnSubModuleLoad`：`UIExtender.Create(id).Register(Assembly).Enable()`（比旧 `new UIExtender()` 无 obsolete）
- `CultureFilterState.SetIndex(i)`：**唯一状态入口**，hotkey/UI click 都走它；写 CurrentIndex → Announce → `Mixins.EquipmentListVMMixin.RefreshAllLiveInstances()`
- `RefreshAllLiveInstances`：mixin 静态 `List<WeakReference>` 追踪活 instance，`RemoveAll` 清死引用 → 逐个 `RefreshCultureButtonStates` + `vm.Build()`（**这是 hotkey 触发过滤的关键路径**）
- Build patch 保持 v1.0.1 的 `[HarmonyPatch(EquipmentListVM, nameof(Build))]` postfix

**双路径闭环**：
1. **UI click**：`ExecuteSelectCultureAserai` → `SetIndex(3)` → `RefreshAllLiveInstances` 及 mixin 内部 `base.ViewModel.Build()` → BuildFilterPatch 跑 → 过滤生效
2. **Hotkey Ctrl+Shift+C**：SubModule.OnApplicationTick → `CultureFilterState.CycleNext` → `SetIndex((idx+1)%7)` → 同上路径

**版本 & 部署**：
- `SubModule.xml` v1.0.1 → **v1.1.0**
- `LauncherData.xml` `<Id>RetinuesCultureFilter</Id>` LastKnownVersion v1.0.1.0 → **v1.1.0.0**
- `deploy.ps1` 加 GUI/ 目录整包 copy（UIExtenderEx 通过 `Modules\<mod>\GUI\**\*.xml` 递归匹配 `PrefabExtensionFileName` 返回值）
- 构建：0 warn / 0 err，1.41s；DLL/PDB/SubModule.xml/GUI/PrefabExtensions/CultureButtonRow.xml 全部部署到 `Modules\RetinuesCultureFilter\`

**已知潜在风险 & 排错**（若 v1.1 依然不生效）：
- 按钮完全不出现 → XPath 未匹配。检查 `Configs\ModLogs\` 里 UIExtenderEx warn；备选 XPath 见 DESIGN_v1.1_UI §4.3 Plan B（直接 target `EditableTextWidget[@Text='@FilterText']` sibling insert）
- 按钮出现但点击无反应 → mixin 没注册。检查 SubModule.cs 里 `extender.Enable()` 已调，mixin 类是 `public sealed` + `[ViewModelMixin]`
- click 生效但 IsSelected 视觉不刷 → `OnPropertyChangedWithValue` 参数错误（typo）
- click filter 生效但 hotkey 依然不动 → WeakReference `_liveInstances` 被 GC 清空。改成 strong reference `List<EquipmentListVMMixin>` 加显式 unregister（或直接省 WeakReference，只 append 不清理，接受轻微泄漏因编辑器关-开次数少）
- 编译成功但游戏首启 warn "Failed to apply extension to ClanScreen" → XPath predicate `@DataSource='{EquipmentList}'` 语法可能需要转义，改用 `contains(@DataSource,'EquipmentList')`

**Claude 不能驱动的实机验证**（用户操作项）：
1. launcher 确认 Retinues Culture Filter v1.1.0.0 勾选，加载**在 Retinues 之后**
2. 进战役 → Clan Screen → Retinues Troop Editor → 编辑某兵种装备 → 打开装备槽（如 Weapon Slot 1）
3. Filter Row（搜索框那行）下方应出现 7 个按钮：`All | Empire | Vlandia | Aserai | Battania | Sturgia | Khuzait`
4. 点 Aserai 按钮 → Aserai 按钮 IsSelected 视觉高亮 + 列表立刻只剩 Aserai 装备（不需要再切槽）
5. 点 All → 列表恢复全部
6. Ctrl+Shift+C 循环 → 消息栏 `[Aserai]` + 对应按钮高亮变化 + 列表跟着切
7. Ctrl+Shift+X 清空 → All 按钮高亮 + 列表恢复
8. 若某步失败 → 记具体现象（按钮不出现 / click 无反应 / IsSelected 不刷 / hotkey 不响应），对照上方"已知潜在风险 & 排错"逐项排查

### 16c. v1.0.1 fix 已应用（2026-09-21）

**改动**：`SubModule.cs` 里 `[HarmonyPatch(typeof(EquipmentListVM), nameof(RefreshFilter))]` → `nameof(Build)`；`RefreshFilterPatch` 类改名为 `BuildFilterPatch`，patch 主体不变。

**版本**：`SubModule.xml` v1.0.0 → **v1.0.1**；LauncherData 里 `<Id>RetinuesCultureFilter</Id>` 的 `LastKnownVersion` 同步升 v1.0.1.0。

**构建**：`deploy.ps1` → 0 warn / 0 err（1.29s），部署到 `Modules\RetinuesCultureFilter\bin\Win64_Shipping_Client\`。

**Claude 不能驱动的实机验证**（用户操作项）：launcher 确认 Retinues Culture Filter 勾选 → Clan Screen → Retinues Troop Editor → 编辑某兵种装备 → 打开装备列表 → 按 Ctrl+Shift+C 消息栏出现 "[Aserai]" → **点一下装备槽**触发 Build → 列表应只剩 Aserai 装备。DIAGNOSTIC_NOTES.md §fix 里"OnSlotChange/OnFactionChange 直接调 Build"的推论若正确，则不再需要"改搜索文本"作为触发条件。

**下一步**：待用户实测反馈；若 filter 正常工作，按 DESIGN_v1.1_UI.md §5 checklist 推进 v1.1 dropdown UI。

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

## OSA 三件套 × RBM 兼容核对（2026-09-19 本机 D: 实测）

> 用本机 ilspycmd/脚本对 workshop 文件实测得出（游戏 + mod 都在 D:）。**已替换会崩的散装 armory**：`swadian armoury(2875090166)` + `BahamutArmory(2875496157)` 本会话期间已从 workshop 移除，改用 OSA 三件套。

### 三件套构成（均 workshop，纯 XML 无 DLL）
- Open Source Armory (OSA) `3011479883` v2.0.0 — 护甲/头盔
- Open Source Weaponry (OSW) `3010984416` v2.0.1 — 武器 + 锻造件 + XSLT（weapon_descriptions/crafting_templates）
- Open Source Saddlery `3010990914` v2.0.0 — 马具

### 为什么安全（vs 崩溃的散装 armory）
- **纯 XML，无任何 DLL** → 无版本敏感 Harmony 补丁（老 armory 最大崩因不存在）
- 依赖仅 Native/SandBox 核心；无 `DependentVersion`（版本无关，v1.4.7 正常）
- SubModule.xml 内建 `<DependedModuleMetadata id="RBM" order="LoadBeforeThis" optional="true"/>` → 自声明排在 RBM 之后，顺序自动正确
- shaders 文件夹存在但 **0 文件** → 网传"shaders 致崩（1.4.8）"不适用

### 与 RBM 的物品 ID 交集（脚本求交，硬结论）
- OSA/OSW/Saddlery 定义 **1950** 个 item+piece ID；RBM 覆盖 **1343** 个原版 ID
- **交集仅 20 个**：几乎全是原版锻造钝头件（`spear_blade_*_blunt` / `axe_craft_*_head_blunt` / 各文化 `*_blade_*_blunt`）+ 1 件 `empire_battle_crown_west`
- 其余 **1930 个是全新 ID**（`OSA_*` / `AD_shield_*` 等），与 RBM 不撞
- OSW 排 RBM 之后 → 这 20 个 **OSW 定义盖过 RBM**（丢 RBM 调校；影响极小：训练钝头件 + 冠冕）

### 结论与残留风险
- **可用，崩溃风险远低于 Bahamut/Swadian**。
- 残留：① 1930 新装备非 RBM 数值口径——实测 RBM 把原版护甲**上调约 1.7–1.8×**（见下"数值平衡方案"），故 OSA 护甲在 RBM 下**偏弱（更易被砍穿），非偏强**（更正上一版记录）；② OSW 用 XSLT，别的 mod 畸形 XML 可能触发 `"XmlReader state should be Interactive"`，首启看 trace/butterlib。
- **已自动生效的一半平衡**：OSA 物品引用标准 `modifier_group`（leather/chain/plate）→ RBM 的 `RBMCombat_item_modifiers.xml`（104 条）对修饰词分级的重平衡**这些新装备自动继承**；未调的只是**基础护甲值本身**。

### OSA×RBM 数值平衡方案（待办，2026-09-19 探讨）

**问题**：RBM 为配合更狠伤害模型把原版护甲整体上调（本机实测 246 件原版身甲，RBM 全覆盖）：

| 材质 | RBM/原版 身甲均值倍率 | n | 离散 |
|---|---|---|---|
| Chainmail | ×1.70 | 44 | 1.04–2.85 |
| Cloth | ×1.78 | 107 | 0.69–6.50 |
| Leather | ×1.65 | 55 | 0.00–8.00 |
| Plate | ×1.80 | 38 | 0.95–2.67 |

上表 = RBM 相对**原版**的抬升（离散度大 = RBM 逐件手调，非统一公式）。

**⚠ 修正（2026-09-19 实测 OSA 388 件身甲，之前"OSA ≈ 原版 / 均匀低 RBM 1.75×"过度简化）**：OSA 与 RBM 的差距**材质相关、不均匀**：

| 材质 | 原版 | RBM | OSA | OSA/RBM |
|---|---|---|---|---|
| Chainmail | 30.0 | 49.8 | 33.1 | 0.66× |
| Cloth | 7.4 | 10.7 | 8.4 | 0.78× |
| Leather | 13.0 | 18.3 | **18.2** | **≈1.00×** |
| Plate | 40.1 | 73.3 | 40.9 | 0.56× |
| max 值 | 57 | **135** | **57** | — |

- Plate/Chainmail 明显低于 RBM；Cloth 略低；**Leather 已 ≈ RBM（无 gap；OSA 皮甲不是原版尺度而是被抬到了 RBM 水平）**。
- **OSA 顶配身甲 57 = 原版天花板；RBM 把原生重甲推到 135** → OSA 在 RBM 下偏弱的最直观来源。
- 统计陷阱：OSA 身甲**总均值(31.8) 反略高于 RBM(29.2)**，因 OSA 目录 **53%(206/388) 是 plate** 拉高总均值；必须**按材质逐一比**才是真相。

**机制（不动 OSA 本体）**：建**个人纯 XML 覆盖 mod**，加载排 OSA 之后，用相同 OSA 物品 ID 重定义护甲到 RBM 尺度 → Workshop 更新安全、无再分发、零崩溃面；本机工具链可脚本生成。

**取值（修正后）**：**不能一律 ×1.75**（会把 Leather 严重超模）。应**材质相关**：Plate ×~1.8、Chainmail ×~1.5、Cloth ×~1.3、**Leather ≈跳过（×1.0）**；或方案 B 就近映射 RBM 原生件拷护甲档（最准）。**头/腿/手甲未量**，可能有类似材质差异，做 mod 前需逐类跑全。武器见下。

### OSW 武器 · RBM 方向与护甲相反（2026-09-19 实测）

- **OSA 确加武器**（经 OSW）：完整武器 Item 少量（17，多为投掷/石头/远程）；**主体是锻造部件**（`OSA_crafting_pieces.xml` 刀刃/斧/枪头等——可锻造武器伤害来自部件）。OSW 的 4 个 XSLT 只改 mesh/name + 把新部件加进 crafting_templates/weapon_descriptions 的可用件列表，**不重算伤害**。
- **实测 blade 部件 damage_factor**：

| | Swing df | Thrust df |
|---|---|---|
| 原版 | 3.23 | 2.40 |
| RBM | **0.96** | **0.84** |
| OSW | **3.00（≈原版）** | 1.91 |

- **方向与护甲相反**：RBM 把武器伤害系数**砍到原版 ~30%**（3.23→0.96），OSW 停在原版 → **OSW 武器 ≈ RBM 的 3× 伤害系数 → RBM 下大概率超模/过强**（护甲则是 RBM 抬高、OSA 偏弱）。一句话：同样"按原版设计"，护甲=太弱、武器=太强。
- **平衡须双向**：护甲 ×~1.5–1.8（升）；**武器部件 damage_factor ×~0.3（降）向 RBM 靠**。社区 RBM-OSA 补丁只提"改 armor values"，**武器是否覆盖存疑**——若不覆盖则超模武器问题仍在。
- 确定度：部件系数实测（blade 原版/RBM/OSW = 255/157/28 件）+ XSLT 无重算已确认；"超模"是基于 ~3× 原始差距的强推断，最终伤害仍过 RBM 公式，100% 坐实需反编译 RBM 伤害链。**只测了 blade**（斧/锤/枪未测）。

### 加载顺序
`... RBM / RBM WS → OSA / OSW / Saddlery → Retinues / CYT / ...`

---

## IG 城堡防御 / 守卫（Guards）配置深挖（2026-09-19 本机反编译）

> 目标：让城堡"派多支带减耗 buff 的驻军去猎杀半径内敌军 + 护村"。结论：**IG 本身几乎全实现，无需 MapBlockade / 新 mod**，主要是开关+调参。反编译 `ImprovedGarrisons.SaveSystem.Configuration.Config`（默认值）+ `Behaviours.GarrisonPartyBehavior` + `AI.Orders.PartyOrder.{OrderPatrol,OrderDefense}` + `AI.AITypes.MobileGarrison`。

### 关键区分：你的据点 vs AI 据点
- **你自己的 fief**：守卫在**游戏内 IG Guards 子菜单**逐据点下令（Patrol/Defense/Escort/MergeGarrison，来自 `AI.Orders.PartyOrder.*`）——**不是 XML 开关**。
- **`NPCSpawnGuards`（默认 false）**：只决定 **AI 据点**是否也自动派守卫。只想强化自己城堡 → **保持 false**，用游戏内 Guards UI 给自家 fief 下令即可（开它 = 全图 AI 派队，性能/存档/钱粮代价大）。

### 守卫/巡逻配置键（反编译默认值）
| 键 | 默认 | 作用 |
|---|---|---|
| `NPCSpawnGuards` | false | AI 据点也自动派守卫（全世界）|
| `NPCGuardSpawnThreshold` | 120 | AI 据点驻军 ≥ 此值才派（仅 NPC）|
| `NPCGuardCreationMultiplier` | 0.4 | AI 抽驻军比例（仅 NPC）|
| `AmountOfUnitsToSpawn` | 5 | 每批派出单位数 |
| `GuardAvailableTroopsPercentage` | 0.25 | 守卫最多抽走驻军 25%（**护城底线，勿拉到 1.0**）|
| `DefaultGuardsEnableReplenish` / `GuardReplenishPercentage` | true / 0.5 | 守卫从驻军补员至 50% |
| `PatrolPartyHealPercentage` | 0.5 | 掉到 50% 撤回治疗 |
| `CustomTransferAndGuardPartySize` | 200 | 守卫/转运队规模上限 |
| `CustomGuardAndTransferPartySpeed` | 4.8 | 守卫队速度（⚠ 需 `LoadCustomPartySpeedModel=true`，默认 false）|
| `SpawnOnlyNobleTroops` | false | 只用贵族线兵组队 |
| `DefaultEnableGuardHideoutClear` / `...Upgrade` / `...PrisonerSell` | true/true/true | 守卫清匪窝/自动升级/卖俘 |
| `DefaultEnableGuardBuyHorses` / `...PrisonerRecruitment` | false/false | 守卫买马/抓俘 |

### 减食 / 减耗 buff（你要的）
| 键 | 默认 | 作用 |
|---|---|---|
| `DisableGarrisonNeedsFood` | false | **true → 驻军（含出击队）不吃食物** |
| `GarrisonGuardsWageMultiplier` | 1.0 | **<1 → 守卫队更省工资** |
| `GarrisonWageMultiplier` | 1.0 | 整体驻军工资 |

### 需求覆盖与硬缺口
- 多支出击 ✅（`AmountOfUnitsToSpawn`/`GuardAvailableTroopsPercentage`/`CustomTransferAndGuardPartySize`）
- 护村 ✅（`OrderDefense` + `DefendVillageIfNeeded`，raid 触发，无独立开关）
- 减耗 ✅（`DisableGarrisonNeedsFood` + `GarrisonGuardsWageMultiplier<1`）
- **自定义兵种** ⚠ 无"守卫选兵器"，守卫从**驻军花名册**抽兵（受 `SpawnOnlyNobleTroops`）→ 只能**把 Retinues 自定义兵编进驻军**
- **radius 内攻击** ⚠ 半径由 `CalculatePatrolRadius` **代码算，无配置键** → 想扩大需 **DLL patch**（类似 GarrisonDrills 修改 #3）
- **增强村庄驻军** ⚠ IG 是**派守卫队护村**，非静态村庄驻军；无"村庄驻军规模"键

### 配置位置
游戏内 IG ribbon → Configuration；或关游戏改 `Configs\ImprovedGarrisons\Saves\IGConfiguration_<save>_<hash>.xml`（键名同上）。

---

## 新兵 / 志愿兵（Volunteer）生成机制 + RBM/Retinues 覆盖（2026-09-19 反编译）

### Vanilla `DefaultVolunteerModel`（三维度）
- **线（精英 vs 基础）· `GetBasicVolunteer(sellerHero)`**：`IsRuralNotable && Village.Bound.IsCastle` → `Culture.EliteBasicTroop`；否则 `Culture.BasicTroop`。即**绑定城堡的村庄出精英线，绑定城镇的出基础线**——硬规则，不是"更高比例"。
- **tier（能招多高）· `MaximumIndexHeroCanRecruitFromHero`**：主看**你与 notable 的关系**（≥5/10/20/40/60/80/100 → index 1..7），叠同阵营/非主角/战争/perk，`Min(6,…)`，`MaxVolunteerTier=4` 封顶。
- **每日补充概率 · `GetDailyVolunteerProductionProbability`**：阵营 fief 数/繁荣、slot index（越高越稀）、Cantons 政策、骑兵 perk。
- 文化决定用哪棵树。

### ⚠ RBM 覆盖（关键发现）——`RBMCombat.CampaignChanges.DefaultVolunteerModelPatch`
RBM 用 Harmony **Prefix `return false` 完全替换** `GetBasicVolunteer`：
```csharp
if (MBRandom.RandomFloat < 0.15f) __result = EliteBasicTroop; else __result = BasicTroop; return false;
```
→ **全局 15% 精英 / 85% 基础，无视城堡/城镇绑定**。vanilla"城堡村→精英"被废。**这就是"逛村招不到多少精锐"的根因**（不是 bug，是 RBM 设计）。`0.15f` 是**硬编码字面量，无 config 开关**。

### PlayerSettlement 覆盖？——**覆盖**
PS 建真正的 Village（`SetBound`/`GetPotentialVillageBoundOwners`）+ 生成 notable（`AddInitialNotables`/`CreateNotable`）+ 有 culture（`ForcePlayerCulture`/`SelectedCultureOnly`）→ vanilla 模型原样生效。**PS 村庄绑城堡 → 精英线（但仍被上面 RBM 15% patch 拦截）**。

### Retinues 覆盖（你自有 fief）
Retinues 在**自有据点**把志愿兵 100% 换成自定义兵（`VolunteerSwapForPlayer`，`RestrictToOwnedSettlements=true`，`CustomVolunteerProportion=1`）→ 自有 fief 里 vanilla/RBM 的线判定被 Retinues 盖掉。

### "只影响玩家阵营 + 兼容 Retinues"的精英修复 · 可行性判定
- 关键约束：`GetBasicVolunteer` **只有卖家无买家**，且 notable 的 `VolunteerTypes[]` **全阵营共享** → 生成层无法区分"谁来招"。
- **只对自有据点生效**：可写（据点归属检查 + `[HarmonyBefore(RBM)]`），但**和 Retinues 100% swap 重叠 → 基本空转**（除非 Retinues proportion<1）。
- **只玩家受益、AI 不变**：❌ 不干净（池共享、买家未知）。
- **推荐替代**：① 逛别人村想刷精英 → 做"恢复 vanilla 城堡村→精英"的**全局** Harmony mod（简单，正统玩法）；② 自家 fief 要精英 → 直接在 **Retinues 设计精英自定义兵**（零代码）。

---

## Retinues 自创兵种跨档保存（Export/Import，2026-09-19 反编译）

**结论：能跨档保存，不必每开一档重做。** `Retinues.Troops.TroopImportExport` 提供 XML 导出/导入。

- **导出** `ExportUnified(fileName, includeCustom, includeCultures)` → XML（`XmlSerializer` of `RetinuesTroopsPackage`）
  - `includeCustom` = 你的**氏族 + 王国自定义兵**（`FactionSaveData(Player.Clan)` + `Player.Kingdom`）
  - `includeCultures` = 连所有文化/氏族兵种树一起打包
- **导入** `ImportUnified(fileName, scope)`，`ImportScope` = `CustomOnly` / `CulturesOnly` / `Both` → `clanData.Apply(Player.Clan)` 套用到当前存档
- **游戏内入口**：Retinues 管理 UI 的 **Export All / Import** 按钮（`ExecuteExportAll` / `PickAndImportUnified`，导入弹文件选择、最新在前）
- **文件位置**：`<Retinues 模块>\Exports\<前缀>_yyyy_MM_dd_HH_mm.xml`；workshop 版 = `D:\SteamLibrary\steamapps\workshop\content\261550\3599557394\Exports\`（首次导出自动建）
- ⚠ **在 workshop 目录 → Steam 更新/校验可能清空**，导出后**务必另存**一份到 workshop 之外（E: 盘 / 本 repo）
- 向下兼容旧导出格式（`LegacyTroopImporter`）

**流程**：当前档设计好 → Export All（勾 includeCustom）→ **备份 XML** → 新档进游戏后 Import（scope=CustomOnly）→ 整套兵种套回。

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

### Bug #6 · CalradianPatrolsV2 v4.0.2 加载失败（2026-09-19）

- **症状**：LauncherData 里 CP2 `IsSelected` 自动变 `false`，DLLCheckData 里 `IsDangerous=true`；butterlib/default/trace 日志无 CP2 记录（根本没跑到）
- **根因**：v1.2.8 → v1.4.7 base class 演化，CP2 有 3 个 Custom Model 的 abstract override 签名对不上，launcher 静态检查直接拦下
- **具体不兼容**：
  - `CustomWageModel : PartyWageModel`：`MaxWage`（应 `MaxWagePaymentLimit`）、`GetTotalWage(MobileParty, bool)`（应带 `TroopRoster`）、`int GetTroopRecruitmentCost(...)`（应返回 `ExplainedNumber`）
  - `CustomBanditDensityModel : BanditDensityModel`：缺 6 个新 abstract（`NumberOfMinimumBanditPartiesInAHideoutToInfestIt` / `NumberOfMinimumBanditTroopsInHideoutMission` / `NumberOfMaximumTroopCountForFirstFightInHideout` / `NumberOfMaximumTroopCountForBossFightInHideout` / `SpawnPercentageForFirstFightInHideoutMission` / `GetMinimumTroopCountForHideoutMission`），多余 `NumberOfMaximumLooterParties`
  - `CustomSettlementSecurityModel : SettlementSecurityModel`：缺 6 个新 abstract（`ThresholdForTaxCorruption` 等）
- **教训**：`DependentVersion` 只是 built-against 标记不代表兼容——**跨主版本（v1.2→v1.4）base class 演化时**，abstract override 匹配失败会被 launcher 静态拒收，比 Bug #3 那种"存档不兼容"更早且更彻底
- **处理**：弃用，走 IG `NPCSpawnGuards` + BetterPatrols 替代

### Bug #7 · OSA 三件套 Shader 缓存导致装备不显示 + 退出崩溃（2026-09-20）

- **症状**：装完 OSA v2.0.0 + OSW v2.0.1 + Saddlery v2.0.0 首跑：
  1. Retinues Troop Editor 里看不到 `AR_*` 前缀的 OSA 物品
  2. 退出游戏时崩溃
- **诊断过程**：
  1. Retinues `debug.log` 03:25:46 `SaveBehaviorData: 96 unlocked` 全是 vanilla ID（无 `AR_*`）→ OSA 物品未进 `MBObjectManager`
  2. PSBridge log 显示 `MapBlockade already tracks 0 settlements` + 134 candidates 全部 "no faces within radius 5" → MapBlockade 侧数据 pipeline 也异常
  3. 46 个 OSA XML 语法自查全 parse OK（不是语法错）
  4. 检查 workshop 目录：**每个 OSA mod 都有 3 个 Shaders/D3D11 缓存文件**（OSA 15.19 MB + Saddlery 2.54 + OSW 1.45 = 共 19.18 MB）——**与 journal 之前 D: 机器"shaders 文件夹 0 文件"结论矛盾**（那时 D: 机器上还没编译过 shader）
  5. `shader_compile_report.log` head 无错误提示；具体版本兼容性无法通过日志判断
- **假设**：Shader 缓存来自旧 game engine 版本或跨机器复制，v1.4.7 加载时与运行时冲突；退出时 D3D11 资源清理路径遇到 dangling 引用 → 崩
- **修复（尝试中）**：删除三个 mod 的 `Shaders/D3D11/` 目录，Bannerlord 下次启动会重编译（预计 10-15 min 首启等待）
- **待验证**：Shader 重编后 OSA 物品是否进 UI + 退出是否不再崩。若仍有问题走 bisect（禁 OSA/OSW/Saddlery 逐个隔离）

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

- [x] ~~手动在 Steam 里关闭 Bannerlord 的 Steam Cloud sync（Properties → General）~~ ← 2026-09-20 用户确认已关
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
- [x] ~~**Tier6Injector · 自研模组（2026-09-19 立项 → v1.3 已发）**~~ ← **2026-09-20 改名 `EquipmentSpawnerMod` v1.4，加入个人库**（详见下方 v1.4 条 + 2026-09-20 日志）。位置：本 repo `EquipmentSpawnerMod/` 子目录（旧 `Tier6Injector/` 已删）、详见该子目录 `README.md`。热键：Ctrl+Alt+I 注入 Tier 3-6 装备 ×20 + 战马 ×20；Ctrl+Alt+O 主队→个人库；Ctrl+Alt+P 个人库→主队。个人库通过 `PersonalStashBehavior : CampaignBehaviorBase` + `dataStore.SyncData<ItemRoster>()` 持久化到存档
- [x] ~~**OSA×RBM 数值平衡（2026-09-19 立项）**~~ ← **2026-09-20 结案**：作为 `OpenSourceArmouryRBMBalance` mod 落地（v1.0.0，纯 XML override，1507 件 armor + 18 件 Blade piece override + 640 件头盔颈/肩延伸）。详见修改 #12。武器系数 + 护甲跨槽结构同时处理完毕。**待用户实机验证 + 若手感失控则微调**

- [x] ~~**OSA 护甲 vs RBM 跨槽覆盖分析（2026-09-20 实测 + 二次修订）**~~ ← **2026-09-20 已落地为 `OpenSourceArmouryRBMBalance` mod**（见修改 #12）：用户观察到 RBM 装备常给多个 body slot 加护甲。**全量扫描**（RBM 664 件 + OSA 1648 件，全部 OSA_*.xml 都覆盖，非仅 ar_reforms 系列）：

  **RBM 跨槽覆盖模式（RBM 沉浸感与真实感的关键结构）**：

  | RBM ItemType | n | head槽 | body槽 | arm槽 | leg槽 | 结构说明 |
  |---|---:|---:|---:|---:|---:|---|
  | HeadArmor（头盔） | 256 | **100%** avg 63.9 | **54%** avg 27.3 | **50%** avg 23.4 | 0% | 头盔延伸颈甲+肩甲（"aventail 模型"） |
  | BodyArmor（身甲） | 246 | 1% | **99%** avg 29.6 | **93%** avg 18.4 | **100%** avg 21.4 | 身甲覆盖上臂+大腿 |
  | Cape（披风/斗篷） | 91 | 0% | **100%** avg 23.1 | **0%** | 0% | **纯 body_armor，绝无 arm** |
  | HandArmor（手甲） | 32 | 0% | 0% | **100%** avg 35.2 | 0% | 单槽 |
  | LegArmor（腿甲） | 39 | 0% | 0% | 0% | **100%** avg 31.4 | 单槽 |

  **OSA 现状全量（14 个 XML 文件、1648 件）**：

  | OSA ItemType | n | head槽 | body槽 | arm槽 | leg槽 | vs RBM |
  |---|---:|---:|---:|---:|---:|---|
  | HeadArmor  | 843 | **100%** avg 36.4 | **0%** | **0%** | 0% | head 57%；无颈/肩延伸 |
  | BodyArmor  | 388 | 0% | **100%** avg **31.9** | **96%** avg 9.1 | **99%** avg 11.3 | **body 已 107%（不 buff）**、arm 49%、leg 53% |
  | **Cape**   | 310 | 3% | **97%** avg 12.5 | **69%** avg 6.9 | 0% | body 54%；**arm 69% 覆盖是 OSA 特有设计**（RBM 侧 0%） |
  | HandArmor  | 46  | 0% | 0% | **100%** avg 17.6 | 0% | arm 50% |
  | LegArmor   | 61  | 0% | 0% | 0% | **100%** avg 14.6 | leg 47% |

  **关键设计差异（用户 2026-09-20 观察 + 决议）**：
  - OSA 有 213 件 Cape 带 arm_armor（69% 覆盖率）——命名清一色 `AR_*_lamellar_cape`/`_scale_cape`/`_shoulder_a-z`。**这是 OSA 系统性肩甲（pauldron/spaulder/mantle）设计约定**，RBM 侧完全没有对应实例。
  - **RBM 把"shoulder arm 覆盖"归到头盔的 arm_armor 字段（aventail 模型）**；**OSA 把它归到 Cape 的 arm_armor 字段（pauldron 模型）**。两种设计等价、只是槽位分工不同。
  - 用户偏好 RBM 环境下 arm 保护充实——决议：**尊重 OSA Cape 肩甲约定 + 有节制地并入 RBM 头盔 aventail 模型**（两条路径都启用，Cape 侧 buff 到中档而非全档，避免暴堆）。

  **落地系数（2026-09-20 用户决议）**：

  | slot 修改 | 现况 avg | 目标 avg | ×倍率 | 备注 |
  |---|---:|---:|---:|---|
  | HeadArmor.head_armor         | 34.7 | 63.9 | **×1.84** | vs RBM 全量 |
  | HeadArmor.body_armor（新增） | 0    | 27.3 | 补一个 | 仅 Chainmail/Plate 头盔；Cloth/Leather 跳过 |
  | HeadArmor.arm_armor（新增）  | 0    | 23.4 | 补一个 | 同上 |
  | BodyArmor.body_armor         | 31.9 | 29.6 | **不动**（OSA 已略强） | 保持 |
  | BodyArmor.arm_armor          | 9.1  | 18.4 | **×2.02** | |
  | BodyArmor.leg_armor          | 11.3 | 21.4 | **×1.89** | |
  | Cape.body_armor              | 12.5 | 23.1 | **×1.85** | |
  | Cape.arm_armor（保留 OSA 约定） | 6.9 | 18.4 | **×2.66**（不 ×3.4） | 中档：向 RBM.BodyArmor.arm avg 看齐、非 HeadArmor.arm，避免与头盔 arm 延伸堆叠爆炸 |
  | HandArmor.arm_armor          | 17.6 | 35.2 | **×2.00** | |
  | LegArmor.leg_armor           | 14.6 | 31.4 | **×2.15** | |

  **OSA 头盔 material_type 分布**（Chainmail/Plate 白名单实际会命中多少）：Plate 772 + Chainmail 20 = **792 件（94%）**；Leather 27 + Cloth 24 = 51 件（6%）跳过。OSA 对轻头饰标 Plate 也很常见（stylized 分类偏宽），实际接近"除布/皮 hood 外的所有头盔"都 buff——这是可接受的默认。

  **落地建议 · 三次修订（2026-09-20 tier 分层分析后）**：flat × 与 material-only 都被验证不够精细，采用**分层 + fallback + 排除**三条规则。

  **前置事实（ilspycmd 反编译 `TaleWorlds.Core.DefaultItemValueModel.CalculateArmorTier`）**：
  - Armor tier = `clamp(round(raw × 0.1 - 0.4), 0, 6) - 1`，其中 `raw = 1.2·head + body + leg + arm`，再按 ItemType 乘 (LegArmor 1.6 / HandArmor 1.7 / HeadArmor 1.2 / Cape 1.8 / BodyArmor 1.0)
  - **无 XML 覆盖字段可用**：RBM 只在 ranged 15 处用 `tier_override`；OSA 全线不用；vanilla armor 全线不用。所以 tier 100% 由公式算
  - 有个坑：**tier 是从 armor 值反推的**——所以 "OSA T6 vs RBM T6" 的比较里，两侧 T6 都是"高值 → 高 tier"，但同 tier 的绝对值仍可差很多（RBM T6 Plate head_armor avg **84.8** vs OSA T6 Plate head_armor avg **48**，×1.77 buff 是真实结构差距）

  **(Type, mat, tier) 分层扫描的关键发现**：

  | 已确认要 buff（cell 密度足够）| RBM avg | OSA avg | × |
  |---|---:|---:|---:|
  | HeadArmor Plate T6 head | 84.8 | 48.0 | ×1.77 |
  | BodyArmor Plate T6 body | 75.4 | 46.8 | ×1.61 |
  | BodyArmor Chainmail T6 body | 51.7 | 38.0 | ×1.36 |
  | BodyArmor Plate T6 arm | 36.0 | 13.3 | ×2.71 |
  | BodyArmor Chainmail T6 arm | 34.0 | 14.0 | ×2.43 |
  | BodyArmor Plate T6 leg | 45.2 | 17.3 | ×2.61 |
  | BodyArmor Chainmail T6 leg | 32.6 | 16.5 | ×1.98 |

  | 已确认**不要** buff（OSA 已 ≥ RBM）| RBM avg | OSA avg | × |
  |---|---:|---:|---:|
  | **HeadArmor Chainmail T6 head** | 39.5 (n=2) | 48.4 (n=15) | ×0.82 — flat 会毁掉 |
  | **HeadArmor Plate T2 head** | 15.0 | 19.0 | ×0.79 — OSA 已略强 |
  | **BodyArmor Cloth T2 body** | 8.1 | 14.7 | ×0.55 |
  | **BodyArmor Leather T2 body** | 8.5 | 16.5 | ×0.52 — OSA leather 反而超 RBM |
  | **BodyArmor Leather T3 body** | 12.4 | 23.0 | ×0.54 |
  | **BodyArmor Cloth T3 arm** | 5.7 | 6.4 | ×0.89 |

  | 排除类（语义不匹配、绝不 buff）|
  |---|
  | **OSA Cloth LegArmor 全 15 件** = `simple_shoes / TV_battania_boots_* / TV_moccasins_* / DZ_empire_boots_a / ao_leather_shoes / leather_shoes` — **全是鞋/靴**（Tier 1，leg_armor 1-5），语义等同 vanilla 民用鞋子。RBM Cloth LegArmor 那 7 件是布料腿甲/裹腿。**不 buff、不当同类比较**。落地脚本要按 id keyword 排除 `shoes|boots|moccasins` |

  | Cape.arm slot（RBM 完全无对应数据）| 只 OSA 有 |
  |---|---|
  | Chainmail T2-T4: OSA 4-8 range | 保持 OSA 原值，或**统一目标 arm avg ≈ 10-12**（比 RBM.BodyArmor.arm 略低，避免叠加过强）|
  | Plate T4 106 件 avg 8 | 同上 |

  **落地脚本规则 · 用户 2026-09-20 拍板版**：
  ```
  for each OSA item:
    if Type == LegArmor and id matches /shoes|boots|moccasins/: skip
    compute current tier via formula (raw × 0.1 - 0.4 → clamp+round → -1)
    for each of item's non-zero slot values (head/body/arm/leg):
      key = (Type, mat, tier, slot)
      target = lookup_avg(key, fallback ladder below)
      current = item.slot_value
      if target is null: no buff
      else:
        factor = target / (avg of OSA at same key)
        if factor <= 1.05: SKIP (no downscale, no near-equal churn) ← "max(1.0, factor)" rule
        else: item.slot_value = round(current × factor)

  fallback ladder for target:
    1. RBM avg at (Type, mat, tier)     if n_rbm >= 3
    2. RBM avg at (Type, mat, any tier) if n_rbm >= 3
    3. null → no buff

  Cape.arm slot special case: target = 12.0 flat  ← 用户定 (×1.74 系数、"pauldron 辅助"定位)
                              only for OSA items where arm_armor > 0

  for each OSA HeadArmor with mat in {Chainmail, Plate}:
    if computed_tier < 4: skip (light headwear, no aventail)
    add body_armor = round(head_armor × 0.43)
    add arm_armor  = round(head_armor × 0.37)
  ```

  **三处审慎点已定案（2026-09-20 用户拍板）**：
  1. **Chainmail T6 头盔（OSA 48.4 > RBM 39.5）** → **不动** — `factor ≤ 1.05` 规则自然跳过
  2. **Leather T2/T3 身甲（OSA 已 1.8-1.9× 强于 RBM）** → **不动** — 同上规则跳过；OSA "精工皮甲"定位保留
  3. **Cape.arm slot（RBM 无基线）** → **buff × 1.74**（目标 avg ≈ 12）— 保持"肩甲辅助 arm"定位，不与头盔 aventail 全量叠加

  **等效保护 vs RBM**（buff 后预估）：
  - 全副装备（mail 头盔 + pauldron cape + Plate 身甲 + gauntlet + greaves）：head cover ≈ RBM；shoulder cover 略 > RBM（cape 提供 12 + 头盔延伸 20 ≈ 32；RBM 只有头盔 23）；body / leg / hand cover ≈ RBM
  - 单穿 mail 头盔（无 cape）→ head + neck + shoulder cover 完全等同 RBM
  - 单穿 pauldron cape（无 mail 头盔）→ body + arm 12（OSA 特色，RBM 没有）
  - **Arm slot 总量**：轻装 ≈ 6-10，中装 ≈ 20-30，重装 ≈ 45-55（RBM 相似档位是 ≈ 5、20-25、40-50），OSA 略高但 RBM armor damage 衰减模型自然容纳

  **脚本**：`_scratch_tier_strat.ps1` + `_scratch_final_matrix.ps1` + `_scratch_material_strat.ps1`（repo 根，可复算完整矩阵），落地为 XML override mod 后可删
- [ ] **精英志愿兵修复（2026-09-19 探讨，未做）**：RBM `DefaultVolunteerModelPatch` 把精英志愿兵砍成全局 15%、废掉 vanilla"城堡村→精英"。可选自研 Harmony mod **恢复 vanilla 城堡村→精英线**（`[HarmonyBefore(RBM)]` + `Priority.First`，全局生效）。"只影响玩家阵营"版因 notable 池共享 + Retinues 自有据点 100% swap → 判定为**空转不划算**（详见"新兵/志愿兵生成机制"节）。自家 fief 要精英优先走 Retinues 设计
- [x] ~~**CalradianPatrolsV2 v4.0.2 安装（2026-09-19）**~~ ← **2026-09-19 结案：不可用，launcher 自动禁用**。反编译核实是 v1.2.8 → v1.4.7 API 断裂：3 个 Custom model 类跟 v1.4.7 abstract 签名对不上——`CustomWageModel` 用 `MaxWage`/`GetTotalWage(MobileParty, bool)`/`int GetTroopRecruitmentCost(...)`，v1.4.7 要 `MaxWagePaymentLimit`/`GetTotalWage(MobileParty, TroopRoster, bool)`/`ExplainedNumber GetTroopRecruitmentCost(...)`；`CustomBanditDensityModel` 缺 6 个新 abstract（`NumberOfMinimumBanditPartiesInAHideoutToInfestIt` 等），有个多余 `NumberOfMaximumLooterParties`；`CustomSettlementSecurityModel` 缺 6 个新 abstract（`ThresholdForTaxCorruption` 等）。**结果**：launcher 静态检查发现 abstract 不匹配→标 `IsDangerous=true`+auto-disable，butterlib 日志无 CP2 记录（因为根本没跑起来）。**已弃用**（LauncherData.xml `IsSelected=false`）。IG `NPCSpawnGuards` + BetterPatrols 已覆盖需求
- [ ] **RBM Poise/Stamina 系统调查结论（2026-09-19，未改）**：`Configs\RBM\config.xml` 里两个总开关 `<PostureEnabled>` + `<StaminaEnabled>`（默认均 1）。玩家侧调节靠 `<PlayerPostureMultiplier>`——**注意 RBMConfig.cs line 213-229 的解析是三档预设选择器，不是浮点乘数**：`"0"` → 1.0x（跟 AI 一样，**当前状态**）、`"1"` → 1.5x、`"2"` → 2.0x。反编译 `RBMAI\Stance.cs` 确认这个 multiplier **同时**乘 `maxPosture/postureRegenPerTick/maxStamina/staminaRegenPerTick`（池 + 回复绑定）。**要动的话**：`PostureEnabled=0` + `StaminaEnabled=0` 完全关整套（所有 agent 回归 vanilla）；或 `PlayerPostureMultiplier=1/2` 让玩家 1.5x/2x。**要独立控制回复速度**或**给玩家 0x 完全豁免**都需 DLL byte-patch（类似 GarrisonDrills 修改 #3）
- [x] ~~**MapBlockadePSBridge · 自研桥接 mod（2026-09-19 立项，Phase 2A v0.2 已编译）**~~ ← **2026-09-20 用户决定放弃**：本 repo `MapBlockadePSBridge/` 子目录、部署 `Modules\MapBlockadePSBridge\`、`Configs\ModLogs\PSBridge_*.log`、LauncherData `UserModData`+`DLLCheckData` 全部清理；同时 **MapBlockade 本体也已卸载**（`Modules\MapBlockade\` 删除、LauncherData 条目移除）。历史归档：订阅 `PlayerSettlementBehaviour.SettlementBuildCompleteEvent` → 反射注入 `MapBlockade.BlockadeReachabilityCache._cities` → 调 `RebuildAll(string)` 重算，Phase 2A 骨架 + 反射注入 + `RebuildAll` 触发已跑通；Phase 2A 侦查（is Campaign 命中 / 反射类找到 / 订阅无异常）2026-09-19 全过。放弃原因＝ IG NPCSpawnGuards + BetterPatrols 已覆盖对城堡防御需求，PSBridge 收益不足以支撑维护成本
- [ ] **Village Defense (Xiangyong) v1.1.11 + BetterPatrols v1.0.0 安装（2026-09-20）**：两个都适配 v1.4.7（无 `DependentVersion` 版本锁，用现代 API/AccessTools 运行时探测）。**跟 IG 互补不替代**：IG=城堡驻军派 Guard（40% 抽兵、清匪、卖俘虏），VD=村庄被 raid 时按 hearth 阈值刷民兵（60/80/100/150），BP=buff vanilla castle/town patrol（Guard House 分级 25/50/100/150）+ 自建 village defender + NavalDLC 双巡逻。**加载排 IG 之后、EquipmentSpawnerMod 之前**。**已知重叠**：BP `EnableVillageDefenders=true` + VD 都做村庄防御——若嫌拥挤在 BP MCM 里关这个开关，让 VD 独占村庄侧
- [ ] **OSA 三件套装备 UI 不显示 + 退出崩溃调查（2026-09-20）**：装完 OSA/OSW/Saddlery 首跑，两症状：Retinues Troop Editor 里看不到 `AR_*` 物品；游戏退出时崩溃。诊断：Retinues `debug.log` 显示 `SaveBehaviorData: 96 unlocked` 全是 vanilla ID（无 `AR_*`），OSA XML 语法自查 46 个文件全 parse OK 但物品未进 `MBObjectManager`。**发现每个 OSA mod 都自带 3 个 Shaders 文件**（本次 E: 机器实测 OSA=15.19 MB + Saddlery=2.54 MB + OSW=1.45 MB，与之前 D: 机器 journal 记录的"0 文件"矛盾）。**已删所有三个 Shaders 目录**（共释放 19.18 MB）——Bannerlord 下次启动会重编译（首次约 10-15 min）。**待用户实测**：Shader 清完是否两症状都好；若仍有问题走 bisect（禁 OSA/OSW/Saddlery 逐个隔离）
- [ ] **CYT × FM 兼容性核实（2026-09-20）**：反编译坐实两者**流水线协作无竞争**——CYT patch `MapEventSide.AllocateTroops` **Prefix**（注入 `customAllocationConditions` 白名单，按 StringId 过滤 troopsList）；FM patch `Mission.SpawnTroop` **Postfix**（查 `FormationAssignmentResolver.ResolveFormationIndex` 表设 `agent.Formation`）。**不同方法 + 不同 patch 类型**。fallback 干净：FM 若无该兵映射 → `GetDefaultFormationIndex` → `GetVanillaFormationIndex(character.DefaultFormationClass)`；FM 若从未配任何映射 → `HasCustomDefaults=false` → patch early-return，vanilla 全权分配。**CYT 未选的兵不进战场**，FM 对应 Formation 空着不产生 bug
- [x] ~~**OSA 武器 damage_factor 完整对照表（2026-09-20 实测）**~~ ← **2026-09-20 已落地为 `OpenSourceArmouryRBMBalance` mod**（见修改 #12）。以下为原始数据版（此前的日志条被 2026-09-20 二轮重算覆盖）。

  **✅ Guard/Handle/Pommel 已核实无 damage_factor**（用 PowerShell `[xml]` 遍历全 50 个 `<CraftingPiece>`）：Guard 只带 `armor_bonus + length + weight`；Handle 只带 `length + weight (+ excluded_item_usage_features)`；Pommel 只带 `length + weight (+ appearance)`。**全部武器伤害走 Blade `<BladeData><Swing/Thrust damage_factor=…/>` 一条路径。** 因此 OSA×RBM 平衡 mod **只需 override Blade piece damage_factor**，Guard/Handle/Pommel 不必碰。

  **Blade piece 真实计数（之前 grep 39 的错觉修正）**：`OSA_crafting_pieces.xml` 里第 342–569 行是一整块 XML 注释（`<!--CraftingPiece … </CraftingPiece-->`）包住 20 件 `_blunt` 训练件（tournament 用、`is_hidden="true"`），实际参战 Blade piece **仅 18 件**（journal 早前数字正确）。

  **OSA 侧数据（2026-09-20 重扫，18 件 Blade）**：

  | wclass | n | swing_avg | swing 范围 | thrust_avg | thrust 范围 |
  |---|---:|---:|---:|---:|---:|
  | axe        | 2 | 3.60 | 3.4–3.8   | —    | —          |
  | mace       | 8 | 2.56 | 1.86–3.30 | 1.00 | 1.0（3 件） |
  | spear      | 6 | 1.60 | 1.6（1 件·billhook） | 2.32 | 1.5–2.8 |
  | sword_1h   | 2 | 3.55 | 3.2–3.9   | 0.60 | 0.4–0.8    |

  **RBM 侧基线（RBM + RBM_WS 合并、按 wclass 聚合）**：

  | wclass | swing_avg (pooled) | thrust_avg (pooled) | 数据源计数 |
  |---|---:|---:|---|
  | axe        | 0.98 | 0.80 (couched 1 件)   | RBM.axe 41 + RBM_WS.axe 20 + RBM.couched.axe 1 |
  | mace       | 0.74 | 0.88                  | RBM.mace 37 + RBM_WS.mace 2 + RBM.couched.mace 1 |
  | spear      | 0.75 (couched+WS)   | 0.93                  | RBM.couched.spear 51+ + RBM_WS.spear 16 |
  | sword_1h   | 0.99 | 0.85                  | RBM.sword.sword_1h 134 + RBM_WS.sword_1h 17 + RBM.couched.sword_1h 2 |

  **最终 override 系数（`RBM_pooled ÷ OSA_avg`，2026-09-20 精算）**：

  | 项目 | 方向 | ×倍率 | 备注 |
  |---|---|---:|---|
  | **axe swing**          | ↓ | **0.27** | OSA 太重 |
  | **mace swing**         | ↓ | **0.29** | OSA 太重 |
  | **sword_1h swing**     | ↓ | **0.28** | OSA 太重 |
  | **spear/polearm swing**| ↓ | **0.47** | 仅 billhook 1 件有 swing |
  | **sword_1h thrust**    | ↑ | **1.42** | OSA 弱、需 buff |
  | **mace thrust**        | ↓ | **0.88** | 微降；journal 之前的 "≈OK" 结论精确到 12% 偏强 |
  | **spear thrust**       | ↓ | **0.40** | OSA 太强 |

  **相对 journal 首版（axe/polearm swing×0.25、mace×0.29、sword swing×0.33、sword thrust×1.42、spear thrust×0.43）差异**：sword swing 0.33 → 0.28（更低）；spear thrust 0.43 → 0.40（略更低）；其余相同；新加 mace thrust×0.88（此前记为 "≈OK 不动"）。

  **落地建议**：
  - 建 XML override mod（个人 modmod）：不改 OSA 源文件，用 `<CraftingPiece>` 覆盖同名 id 或走 `[Overrides="MyMod"]` 机制。7 个 override 系数 × 18 件 Blade = ~18 行改动可搞定 swing/thrust 两个数字重写
  - **未处理的类别**：OSA 无 sword_2h、无 dagger、无 pure spear-swing（除 billhook）—— 这些类别若之后 OSA 补 piece，需重跑本脚本再校
  - **护甲侧仍待做**：本轮只处理武器 damage_factor。OSA 头盔/身甲/披风/腿甲/手甲 4 大类共 1648 件的 `head_armor/body_armor/leg_armor/arm_armor` 数值 vs RBM 尺度比对，是 OSA×RBM 平衡的另一半，未展开
  - 脚本：`_scratch_extract_pieces.ps1`（可复算，见 repo 根；本 mod 落地后可删）
- [ ] **Companion 量产 mod 设计（2026-09-20 探讨，未做）· `CompanionFoundry`**：核心 API `HeroCreator.CreateSpecialHero(template, homeSettlement, faction, culture, age)` —— 运行时无中生有创建 Hero。热键（如 Ctrl+Alt+C）从 vanilla `spnpccharacters` wanderer 池挑模板 → 随机生成 → 放到最近 town 酒馆；玩家走 vanilla 招募流程雇佣（或加"直接入 clan"选项）。**Captain 是战场职位不是角色类型**——任何 Hero 站在编队里都是那编队 Captain，量产 Captain = 量产 Companion。工作量 ~1 天，跟 EquipmentSpawnerMod 一个量级
- [~] **EquipmentSpawnerMod v1.4 扩展（前身 Tier6Injector v1.4 设计，2026-09-20 部分落地）**：**a) Banner/Book 加入白名单**——未做（用户 2026-09-20 只选 b 落地）；**b) 个人库 + 三热键**——✅ **2026-09-20 落地**，`PersonalStashBehavior : CampaignBehaviorBase` 挂 `ItemRoster _stash` + `dataStore.SyncData<ItemRoster>("EquipmentSpawnerMod_PersonalStash", ref _stash)`，三热键 Ctrl+Alt+I/O/P 已实现（O/P 用 `IsGear + Horse/HorseHarness` 白名单，不搬食物贸易品；反向 for + `GetElementCopyAtIndex` 避免 `RemoveZeroCountsFromRoster` 重排漏）。编译 0 warn / 0 err，已 deploy 到 `Modules\EquipmentSpawnerMod\`；LauncherData 已更新 `<Id>EquipmentSpawnerMod</Id> v1.4.0.0 IsSelected=true`。**待用户操作**：① launcher UI 里确认 Equipment Spawner Mod 勾选；② 战役内首次 save+reload 让 SyncData 落盘、再按 Ctrl+Alt+O/P 试用；③ 观察 stash 大到几百 stack 时存档大小/加载时间是否明显变化

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
