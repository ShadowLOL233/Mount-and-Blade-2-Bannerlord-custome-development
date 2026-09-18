# Bannerlord 骑马与砍杀2 模组开发日志

**最后更新**：2026-09-17

## 目录
- [环境与路径](#环境与路径)
- [模组清单（最终状态）](#模组清单最终状态)
- [已做的定制修改](#已做的定制修改)
- [Bug 历史与修复](#bug-历史与修复)
- [模组间交互与已知风险](#模组间交互与已知风险)
- [备份与回滚](#备份与回滚)
- [待办 / 开放问题](#待办--开放问题)
- [调试参考](#调试参考如何找信息)

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

---

## 待办 / 开放问题

- [ ] 手动在 Steam 里关闭 Bannerlord 的 Steam Cloud sync（Properties → General）
- [x] ~~进游戏在 town/castle 菜单里找 IG ribbon，打开 Food Gathering~~ ← 已通过直接改 XML 完成（修改 #5）
- [ ] 观察新战役里 RBM Campaign 是否正确工作（看 `RBM\logs\garrison\`、消息栏 spoils/ledger）
- [ ] 验证 Garrison Drills 训练效果翻倍（进城 → Train troops → 看 UI +20/+60/+100 XP）
- [ ] 验证食物经济堆叠实测效果（进 fief → 食物变化条应有 `[IG-Cheats] Garrison Food Bonus: +1000` 且总变化转正）
- [ ] Workshop 若自动更新 GarrisonDrills，DLL 补丁被覆盖，需重跑
- [ ] 食物调优（若观察后需要）：
  - 太富裕 → `DailyFoodGatheringAmount` 从 1000 降到 100-200
  - NPC 世界还是穷 → `TroopSettlementFoodDays` 20 → 6-8
  - 玩家 fief 仍赤字 → `VillageProductionMultiplier` 0.75 → 1.0
- [ ] AutoParry 是否要启用（当前 false）

### 调查与开发项目（backlog）

- [ ] **Retinues · House 单位 tier 上限**：能否把 Retinues 里"House"级别的自定义部队直接调到最高 tier（6/7）？需扒 `Retinues.dll` 里的 troop 定义/upgrade 路径限制，找 tier cap 相关字段
- [ ] **Retinues · Clan Traditions 跳过**：Clan Traditions 系统（族群传统）是否有内置开关能整个禁用/跳过？如果没有，找它绑定的 CampaignBehavior 名称，评估直接不加载该 behavior 的可行性
- [ ] **RBM · Bot 武器优先度**：RBM 的 AI 是否有"给 bot 挑武器"的优先度设定（比如偏好长杆 vs 双手）？看 `RBMAI.dll` 或 `RBMCombat.dll` 里 `Formation` / `WeaponPreference` / `EquipmentSelection` 相关字段，判断能否 config 调节

### 食物经济 · 观察中的问题

- [ ] **每日食物变化在 +/- 之间震荡，无法稳定积累**：根因是 RBM `_measuredFoodChange` 每日重算，输入项（村庄产出、繁荣、驻军、作坊、民兵）各自浮动，均值接近 0。当前仅有 IG default 10 的加成不足以撑住。**待打 getter 补丁**（`get_DailyFoodGatheringAmount` 常返 1000）后重测；若仍震荡，考虑把 `VillageProductionMultiplier` 从 0.75 拉到 1.0

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
