# UnifiedTroopManager · 设计文档（Phase 0 · Draft v0.1）

**作者**：Claude 起草 · 2026-09-28
**状态**：🟡 待用户 review + 修改 · 尚未开工
**目标 Bannerlord 版本**：v1.4.7（build 117484）—— 与主力机版本对齐
**目标 .NET Framework**：net472（同 CYT/FM）
**目标 mod Id**：`UnifiedTroopManager`（待定 · 见 Open Question O-1）

---

## 1. 项目动机

### 1.1 触发事件
2026-09-28 用户 in-game 观察：装 SSYF（FormationManager）+ CYT（ChooseYourTroops）后 · 在 CYT UI 选定"混合兵种上场"→ 进战斗只加载**一种**兵种（用户最近在 SSYF UI 里编辑过的那个）。

### 1.2 反编译定位的 bug（详见 § 附录 A）
FormationManager 的 `OrderOfBattleVMInitializePatch.Postfix` 在 OoB 屏初始化时对每个 formation slot 强制 `Classes[0].Class = <planned FormationClass>` 且 `Classes[1].Class = NumberOfAllFormations`（清空副类）· `OobWeightDistributor` 分权重后 `LockManagedSliders` 锁死。

**后果**：没被用户在 SSYF 里配 plan 的兵种在 OoB 里没有可站的 slot class → 被挤出 spawn。CYT 的 `MapEventSide.AllocateTroops` filter 在 `_selectedTroops` 层级放行了这些兵，但 OoB 权重预处理阶段已经把它们排除。

### 1.3 为什么选择"整合成新 mod"而不是"给 SSYF 打 patch"
- 两个 mod 的功能语义**天然互补但代码上互不知晓**（CYT 只管 roster · FM 只管 formation · 没有 API 让对方感知）
- SSYF 里的 bug 不是"实现漏洞" · 是 SSYF **不知道 CYT 存在**导致的语义假设错误 · 打 patch 治标不治本
- 整合后：单一权威数据模型（roster + plan 一体化） · 无跨 mod 通信 · 语义一致

---

## 2. 项目范围（用户 2026-09-28 拍板 · 首版 29 项功能）

### 2.1 首版范围 · MUST（16 项 · Phase 1-4 必做）

**A. 战前 Roster 选择**（7 项）
| ID | 功能 | 来源 |
|---|---|---|
| A1 | 按兵种选具体数量上场（每兵种一个滑块 · 0 到 party 现有量） | CYT |
| A2 | Battle size 显示与协调 | CYT |
| A3 | Auto-fill 剩余空位（vanilla / tier / plan 三选一 · 见 O-5） | 新 |
| A4 | 重置到 vanilla（全带） | 新 |
| A5 | 战斗结束自动清空选择 | CYT |
| A6 | 进攻/防御方分别处理 | CYT |
| A7 | 波次总量控制（选中的兵优先第一波） | CYT |

**B. 兵种 → Formation 分配**（5 项）
| ID | 功能 | 来源 |
|---|---|---|
| B1 | 每兵种指定单个 formation（I-VIII） | FM |
| B2 | 每兵种 Split 到多个 formation（Weight 模式） | FM 简化 |
| B3 | 无 plan 走 vanilla `DefaultFormationClass` | 新 |
| B4 | Plan 持久化到 per-hero JSON | FM |
| B5 | 战场生成时 `Mission.SpawnTroop` Postfix apply plan | FM |

**C. Bug Fix**（4 项）
| ID | 功能 | 说明 |
|---|---|---|
| C1 | OoB 屏不锁 formation slot 的 Classes[] | 消除现有 SSYF bug |
| C2 | OoB 屏不锁 slider（无 LockManagedSliders） | 同上 |
| C3 | Plan 应用前先与 Roster 求交 | 未选上场的兵种忽略 plan |
| C4 | 独立于 OoB 也能生效 | Field battle 无 OoB 时走 SpawnTroop 层 |

### 2.2 首版范围 · SHOULD（13 项 · Phase 1-4 一并做）

**D. UX / QoL**（8 项）
| ID | 功能 | 说明 |
|---|---|---|
| D1 | Encounter menu 加"Manage Troops"按钮 | 战前触发 UI |
| D2 | 记住"上一场选择"以便重试 | 战败重试免重选 |
| D3 | UI 显示每 formation 的预览人数 | "Formation I: 15 people" |
| D4 | Formation 支持自定义 rename（如"前排肉盾"） | 存 per-hero JSON |
| D5 | 每兵种显示 tier + FormationClass 图标 | 辅助 plan 决策 |
| D6 | MCM 总开关 + 每功能子开关 | 出问题一键关 |
| D7 | 独立 log 文件 `Configs\UnifiedTroopManager\log.txt` | 排查用 |
| D8 | Reflection 失败降级不 crash | 单点故障不阻塞其它 patch |

**E. 情境适配**（5 项）
| ID | 功能 | 说明 |
|---|---|---|
| E1 | Field battle（遭遇战平地/山地） | 首版主场景 |
| E2 | Siege（攻/守双方） | 攻城战沿用同 UI |
| E3 | Hideout（山贼窝） | 小规模战斗 |
| E4 | Lord's Hall 战斗兼容（`LordsHallFightMissionController` reset） | 特殊 mission 类型 |
| E5 | Tournament / Arena / Practice fight 不干涉 | 判定 mission 类型直接 skip |

### 2.3 Backlog · COULD（10 项 · 首版留档 · 后续 Phase 5+）

以下 10 项**首版不做** · 记录在此供未来启动 · 若首版实测发现某项刚需可优先提上来。

**F. 高级 Formation 行为**
- F1 · 远程兵缺弹自动撤到后线
- F2 · 骑马/下马 formation 自动切换
- F3 · Role Plans（角色映射 · 多兵种共用 plan）

**G. Roster 高级**
- G1 · 多套 Preset 切换（步兵 vs 骑兵 profile 一键切）
- G2 · 按 tier 一键选（"只带 T5+" / "T3 以下不带"）
- G3 · 招募类型全局黑名单（永不带农民 recruit 等）

**H. 盟友管理**
- H1 · 管理盟友部队的 formation
- H2 · 盟友分类型开关（步/骑/远程 独立）

**I. 分析 / debug**
- I1 · Battle summary 显示"本场 formation 分配情况"
- I2 · Mission overlay debug 信息

### 2.4 明确不做（Explicit Non-Goals · 永久 DROP）

- ❌ **Archived / Paused plans**（用 G1 Preset 替代 · G1 也是 COULD）
- ❌ **Custom Split 三种模式**（只保留 Weight · 减认知负担）
- ❌ **PartyCharacterVM mixin**（Party screen 侵入 · 用独立 Gauntlet UI）
- ❌ **Party screen 内 formation 徽章**（与 Gauntlet UI 重复）
- ❌ **强制 OoB slot class / slider lock**（就是 bug 源头）
- ❌ **复用 CYT/FM 源代码**（clean-room reimpl · 见 § 8）
- ❌ **兵种数值 / 装备调整**（属 OSA/RBM/Retinues 领域）
- ❌ **兵种升级 / 招募控制**（Campaign 层不介入）
- ❌ **AI 智能推荐 formation**（只执行 · 不推荐）
- ❌ **Custom Battle 支持**（首版只做 Campaign）

---

## 3. 数据模型

### 3.1 核心概念图

```
Hero (MainHero / 玩家主角)
  │
  └─ PartyPlan (JSON persistence · per-hero)
      │
      ├─ RosterSelection (per-encounter · in-memory, not persisted)
      │     · Dictionary<troopStringId, int>  ← 用户"这场带多少个"
      │
      └─ FormationPlans (persisted · cross-battle)
           · Dictionary<troopStringId, TroopFormationPlan>

TroopFormationPlan {
  Mode: Enum { SingleFormation, Split }
  SingleFormationIndex: int?  // 0-7 · SingleFormation 时用
  Splits: List<SplitEntry>?   // Split 时用（长度 2-8）
}

SplitEntry {
  FormationIndex: int          // 0-7
  Weight: int                  // 相对权重 (>0)
  // 内部由 SplitAllocator 算实际数量 · UI 只暴露 weight
}
```

### 3.2 与现有 FM 数据模型的对比

| FM 概念 | 新 mod 是否保留 | 说明 |
|---|---|---|
| `Assignments: Dictionary<string, int>` | ✅ 简化后保留 | 即 `SingleFormation` 模式 |
| `SecondaryAssignments` | ❌ 移除 | 可用 `Split` 表达 · 降低认知负担 |
| `DeploymentPlans: Dictionary<string, TroopDeploymentPlan>` | ✅ 保留（模型简化） | Custom Split 数据源 |
| `ArchivedDeploymentPlans` / `PausedDeploymentPlans` | ❌ 移除 | 用户体验复杂度过高（除非有明确需求） |
| `RoleAssignments` | ⚠ Phase 5 才引入 | 首版不做 · 简化 UI |
| `ArchivedRoleAssignments` | ❌ 移除 | 同上 |

> **Open Question O-2**：是否需要 archived / paused plans 这类"临时切换套装"功能？如果需要 UI 会复杂 30%+。

### 3.3 JSON 持久化格式

```json
{
  "SchemaVersion": 1,
  "HeroStringId": "main_hero",
  "FormationPlans": {
    "imperial_sergeant_crossbowman": {
      "Mode": "SingleFormation",
      "SingleFormationIndex": 4
    },
    "imperial_legionary": {
      "Mode": "Split",
      "Splits": [
        { "FormationIndex": 0, "Weight": 3 },
        { "FormationIndex": 1, "Weight": 1 }
      ]
    }
  }
}
```

- **文件路径**：`<Documents>\Mount and Blade II Bannerlord\Configs\UnifiedTroopManager\<HeroStringId>.json`
- **兼容策略**：**首版不迁移 FM 旧 JSON**（Open Question O-3：是否要写迁移工具？工作量 ~0.5 session · 用户需要吗？）
- **Schema versioning**：加 `SchemaVersion` 字段 · 未来变更时可迁移

### 3.4 RosterSelection（战前每场重设）

不持久化到磁盘 · 只在从 game menu 打开 UI 到进入 Mission 之间的窗口内活跃。生命周期：

```
PlayerEncounter 发起
   ↓
用户从 game menu 点 "Manage Troops"
   ↓
打开 UI · 默认预填 = 全 party × Number（都上）
   ↓
用户调整每兵种数量
   ↓
Confirm → RosterSelection 存到 Behavior 单例
   ↓
Mission 初始化时 · MapEventSide.AllocateTroops Prefix 读 RosterSelection · 过滤
   ↓
PlayerEncounter.FinishEncounterInternal → 重置 RosterSelection
```

---

## 4. 架构与合并算法（**关键决策**）

### 4.1 合并算法伪代码（**消除现有 bug 的核心**）

```csharp
// 触发点：OrderOfBattleVM.Initialize Postfix (与 FM 同一点)
void OnOoBInitialized(OrderOfBattleVM oobVm, Mission mission) {
    var roster = RosterSelection.Current;     // CYT 层选中的兵
    var plans = PartyPlanStore.Current.FormationPlans;  // FM 层保存的 plan

    // 关键 fix #1: 只考虑被 CYT 选中的兵种
    var selectedTroopIds = roster.Keys.ToHashSet();

    // 关键 fix #2: 不 override formation slot 的 Classes[]
    //   —— 让 vanilla / OoB UI 保持所有 class 可选
    //   （原 FM 的 Classes[0/1] 强制赋值那段 · 不复制）

    // 关键 fix #3: 计算每 formation 的兵种分配 (基于 roster ∩ plans)
    var perFormationTroops = new Dictionary<int, List<(string troopId, int count)>>();
    foreach (var (troopId, selectedCount) in roster) {
        if (!plans.TryGetValue(troopId, out var plan)) {
            // 未配 plan · 用 vanilla 默认 (character.DefaultFormationClass → formation)
            var defaultIdx = (int) MBObjectManager.Instance
                .GetObject<CharacterObject>(troopId)?.DefaultFormationClass;
            perFormationTroops.AddToList(defaultIdx, (troopId, selectedCount));
            continue;
        }
        // 有 plan · 按 plan 分配
        var alloc = SplitAllocator.Allocate(plan, selectedCount);
        foreach (var (formIdx, cnt) in alloc)
            perFormationTroops.AddToList(formIdx, (troopId, cnt));
    }

    // 关键 fix #4: 只调 slider WEIGHT · 不 lock
    //   用户仍可拖 slider 手动覆盖
    OobWeightHint.SoftDistribute(oobVm, perFormationTroops);
}

// 触发点：Mission.SpawnTroop Postfix (与 FM 同一点)
void OnAgentSpawned(Agent agent) {
    if (agent == null || agent.IsMainAgent) return;
    if (!agent.Team.IsPlayerTeam) return;

    var troopId = agent.Character.StringId;
    var roster = RosterSelection.Current;
    if (roster != null && !roster.ContainsKey(troopId))
        return;  // 该兵不该在场（防御性 check · CYT filter 应已挡住）

    if (!plans.TryGetValue(troopId, out var plan))
        return;  // 无 plan · 走 vanilla

    var targetFormationIdx = SplitAllocator.PickForAgent(plan, agent);
    var formation = agent.Team.GetFormation((FormationClass) targetFormationIdx);
    if (formation != null && agent.Formation != formation)
        agent.Formation = formation;
}
```

**核心与原 FM 的差异**：
1. **不 override `item.Classes[i].Class`** — 让 OoB 屏所有 class 保持 vanilla · CYT roster 里的 Infantry/Ranged/etc 都能进
2. **不 `LockManagedSliders`** — slider 由用户完全控制 · 我们只给"推荐权重"作为默认位置
3. **plan 应用前先与 roster 求交** — 未被 CYT 选中的兵种即使有 plan 也不处理
4. **未配 plan 的兵走 vanilla `DefaultFormationClass`** — 而不是被挤出

### 4.2 生命周期时序图

```
用户在地图点遇敌
   │
   ↓
PlayerEncounter 打开 · Encounter game menu
   │
   ├─ [new menu option] "Manage Troops for this Battle"  ← 我们注入
   │       │
   │       ↓
   │    Gauntlet UI 打开
   │       │
   │       ├─ Tab 1: Roster (数量选择)
   │       └─ Tab 2: Formation Plan (per-troop split editor)
   │       │
   │       ↓ Confirm
   │    RosterSelection.Current 填充
   │    PartyPlanStore 存盘
   │
   ↓
用户点 "Battle"
   │
   ↓
Mission 构造中 · DefaultBattleMissionAgentSpawnLogic.Init
   │
   ├─ Postfix: 用 RosterSelection 改 InitialSpawnNumber / RemainingSpawnNumber
   │
   ↓
OrderOfBattleVM.Initialize (如果启用了 OoB)
   │
   ├─ Postfix: 用 roster+plan 计算每 formation 权重 · 只调 slider · 不 lock
   │
   ↓
玩家在 OoB 屏调整 (可选) · 点 Deploy
   │
   ↓
MapEventSide.AllocateTroops
   │
   ├─ Prefix: 注入 customAllocationConditions · 只允许 RosterSelection 里的兵
   │
   ↓
Mission.SpawnTroop (每个 troop spawn 一次)
   │
   ├─ Postfix: 按 plan 分配 formation
   │
   ↓
战斗进行
   │
PlayerEncounter.FinishEncounterInternal
   │
   ├─ Prefix: 重置 RosterSelection.Current
```

---

## 5. UI 架构

### 5.1 入口点

**唯一入口**：Encounter game menu（用户与敌方遭遇后 · 战斗前显示的菜单）新增按钮 `Manage Troops`。

- 触发方式：`campaignGameStarter.AddGameMenuOption("encounter", "utm_manage_troops", ...)`
- 位置：在原生 "Battle" 按钮**之前**
- 显示条件：当前 encounter 有真实战斗（非模拟）
- 图标：无（首版复用 native menu 样式）

### 5.2 UI 方案选型

> **Open Question O-4**：Gauntlet full-screen vs UIExtenderEx overlay？

| 方案 | 优点 | 缺点 | 参考 |
|---|---|---|---|
| **A · Gauntlet full-screen** | 视觉 native · 空间充足 | 需要写 `.xml` GUI 文件 · Bannerlord Gauntlet 反射较深 | CYT 用 `CYTGauntletMenuTroopSelectionView` |
| **B · UIExtenderEx overlay 混合** | 复杂度低 · UIExtenderEx 已装 | 视觉不如 A · 屏幕小时拥挤 | FM 用 `PartyCharacterVMMixin` overlay 到 Party Screen |
| **C · Party Screen 内 tab 扩展** | 最省事 · 已有 mixin 前例 | 从 encounter menu 打开 Party Screen 跳转多 · UX 断层 | 混合方案 |

**推荐 A · Gauntlet full-screen**：
- 用户 in-game 体验最佳 · 与 CYT 原有习惯一致
- 首次 Gauntlet 开发有学习曲线 · 但一次投入长期受益
- 参考代码：CYT 的 `CYTGauntletMenuTroopSelectionView.cs`（反编译可读 · 不复制 · 学结构）

### 5.3 UI 布局草图（Gauntlet · 待用户确认）

```
┌────────────────────────────────────────────────────────────┐
│  Manage Troops · Battle Size: 150/500  Enemy: 320          │
├────────────────────────────────────────────────────────────┤
│  [ Roster ]  [ Formations ]                       [Reset] │
├────────────────────────────────────────────────────────────┤
│                                                            │
│  [ Roster tab ]                                            │
│  ┌───────────────────────────────────────────────────┐    │
│  │ ☑ Imperial Sergeant Crossbowman  T4  Ranged       │    │
│  │   In party: 25 · Bring: [====■═════] 15 / 25       │    │
│  ├───────────────────────────────────────────────────┤    │
│  │ ☑ Imperial Legionary            T5  Infantry      │    │
│  │   In party: 40 · Bring: [======■═══] 30 / 40       │    │
│  ├───────────────────────────────────────────────────┤    │
│  │ ☐ Recruit                       T1  Infantry      │    │
│  │   In party: 50 · Bring: [═════════] 0 / 50         │    │
│  └───────────────────────────────────────────────────┘    │
│                                                            │
│  Total selected: 45 / 150 slots · Auto-fill remaining [☐] │
├────────────────────────────────────────────────────────────┤
│                              [Cancel]  [Save & Continue]   │
└────────────────────────────────────────────────────────────┘
```

```
[ Formations tab ]  (per-troop 展开)
┌───────────────────────────────────────────────────┐
│ Imperial Sergeant Crossbowman                     │
│   Mode: (●) Single formation   ( ) Split          │
│   → Formation V (Ranged)                          │
├───────────────────────────────────────────────────┤
│ Imperial Legionary                                │
│   Mode: ( ) Single formation   (●) Split          │
│   Formation I (Infantry):  [==■══] 3              │
│   Formation II (Shield):   [■════] 1              │
│   (weights · actual counts computed by allocator) │
├───────────────────────────────────────────────────┤
│ Recruit         [No plan · uses default]          │
└───────────────────────────────────────────────────┘
```

> **Open Question O-5**：`Auto-fill remaining` 按钮的语义？
>   - 选项 α：自动按剩余 slot 数补齐 party 里剩余的兵（vanilla behavior）
>   - 选项 β：按 tier 从高到低填
>   - 选项 γ：按 formation plan 优先度填

### 5.4 UIExtenderEx 使用点

- 不 mixin `PartyCharacterVM`（避免与其他 mod 潜在冲突 · 我们的 UI 独立）
- 只 Register 一个 `GauntletView` 类和对应 xml
- 无 `<XmlNode>` 用于游戏内 ItemData（本 mod 不改物品）

---

## 6. Harmony Patch 清单

### 6.1 核心 patches（首版必需）

| Target | Type | 作用 | 对应现有 mod |
|---|---|---|---|
| `MapEventSide.AllocateTroops` | Prefix | 注入 customAllocationConditions filter | CYT |
| `DefaultBattleMissionAgentSpawnLogic.AfterStart` | Postfix | 设置 `_setupSide` 标志（内部状态） | CYT |
| `DefaultBattleMissionAgentSpawnLogic.Init` | Postfix | 改 `_battleSideSpawnContexts` 和 `_phases` 波次数字 | CYT |
| `Mission.OnBattleSideDeployed` | Postfix | 清理 side 状态计数器 | CYT |
| `LordsHallFightMissionController.OnCreated` | Prefix | 特殊 mission 类型重置 | CYT |
| `PlayerEncounter.FinishEncounterInternal` | Prefix | 战斗结束重置 RosterSelection | CYT |
| `OrderOfBattleVM.Initialize` | Postfix | 应用 plan 的 weight（**不 lock class · 不 lock slider**） | FM 修订版 |
| `Mission.SpawnTroop` | Postfix | 按 plan 分配 formation | FM |

**共 8 处 patch** · 都有明确反编译参照。

### 6.2 可选 patches（Phase 5-6）

| Target | 功能 | Phase |
|---|---|---|
| `PartyCharacterVM.ExecuteSetSelected` | Party screen 联动（可能省略） | P5 |
| `MissionBehavior` (custom) tick | Backline auto-reassignment | P5 |
| `SandBoxBattleMissionSpawnHandler.AfterStart` | Battle size fine-tuning | P6 |
| `SandBoxSiegeMissionSpawnHandler.AfterStart` | 攻城战 battle size fine-tuning | P6 |

### 6.3 不 patch 的（相对 FM 原版）

| FM 原有 patch | 为什么不做 |
|---|---|
| `MissionConstructorPatch` | FM 原版 Postfix 是空 · 无用 |
| `PartyTroopTupleCustomSplitEditorPatch` etc | UI 走 Gauntlet · 不 mixin Party Screen |
| `SetInitialHeroFormationsPatch` | Hero 由玩家手动 override · 首版不介入 |
| `RefreshFormationPatch` | 属于 OoB 复杂交互 · 首版跳过 · 观察是否需要 |

---

## 7. MCM 设置项

### 7.1 主选项卡

```
General
  ☑ Enable UnifiedTroopManager (master toggle)
  ☑ Show "Manage Troops" in encounter menu
  ☑ Auto-open Manage UI before every battle (else optional)

Roster
  ☑ Remember last-battle roster selection (for retreat + retry)
  ☑ Support for Maximum Troops (CYT compat mode)
  Default fill mode: [ Vanilla / Tier high-first / Plan priority ]

Formation
  ☑ Apply plans at battle start
  ☑ Reassign on mount/dismount
  ☑ Reassign backline when out of ammo (Phase 5)

Advanced
  Log level: [ None / Info / Debug ]
  ☐ Show debug overlay in mission
```

### 7.2 与 FM 现有设置的映射

| FM 设置 | 新 mod 保留？ | 备注 |
|---|---|---|
| `ModEnabled` | ✅ | `Enable UnifiedTroopManager` |
| `AutoReassignmentEnabled` | ✅ | `Apply plans at battle start` |
| `BacklineReassignmentEnabled` | ⚠ Phase 5 | 首版无 |
| `LockManagedOobSliders` | ❌ **删除** | 这是 bug 源头 · 永久 false |
| `ShowAdvancedFormationEditor` | ❌ 删除 | Advanced Split 首版不做 |
| `ManageAlliedRanged/Cavalry/HorseArchers/Infantry` | ⚠ Phase 6 | 首版只管 MainParty |
| `InfantryFormation` / `ArcherFormation` / `BacklineFormation` etc | ⚠ Phase 5 | 首版无默认全局 formation |
| `MountedBacklineFormation` | ⚠ Phase 5 | 同上 |

---

## 8. License 与 Clean-Room 策略

### 8.1 现状
- CYT (Steam Workshop ID 2957211804) · **无 LICENSE 文件**
- FormationManager (Nexus 11869) · **无 LICENSE 文件**
- TroopClassifier (Nexus 12104) · **无 LICENSE 文件**

无 license 文件在美国版权法下默认视为 "All Rights Reserved" · 未经作者授权的复制 / 修改 / 分发均属侵权。

### 8.2 Clean-Room 策略（强烈推荐）

1. **反编译只用来理解语义**（哪些 Bannerlord 内部 API · 哪些 field · 什么触发时机）
2. **不复制任何反编译的代码块** —— 包括变量命名 / 方法命名 / 注释
3. **文档化每个"知识来源"**：如"通过反编译 FM 得知 `OrderOfBattleVM.Initialize` 是 OoB 生效点" —— 事实性知识不受版权保护
4. **原创实现**：数据模型、算法、UI 布局都用不同的组织方式
5. **保留独立命名空间** `UnifiedTroopManager.*` · 不用 `FormationManager.*` 或 `ChooseYourTroops.*` 命名模式
6. **README 明确 attribution**：致谢原 mod 作者的功能启发 · 声明本 mod 是 clean-room 实现

### 8.3 兜底：联系作者

- CYT 作者：Steam Workshop 页面（Bannerlord Workshop 2957211804）留言 / 问是否 MIT license
- FM 作者：Nexus 11869 页面 · Terms of Use tab
- 如获授权可直接 fork · 但即使有授权 · clean-room 依然是更好的路径（代码质量可控）

---

## 9. 分 Phase 计划与验收标准

### Phase 0 · 设计（**当前**）
- 交付：本 `DESIGN.md` · 用户 review + 定稿
- 验收：用户签字确认 · 所有 Open Question 有明确决策

### Phase 1 · Scaffold（0.5 session）
- 交付：
  - `UnifiedTroopManager/` 完整目录结构（csproj · SubModule.xml · GUI/ · ModuleData/）
  - MBSubModuleBase 骨架 · MCM 设置类骨架
  - Harmony bootstrap · 空 patch 类模板
- 验收：Bannerlord 启动 · mod list 出现 · 无 crash · MCM 里出现 UTM 设置页

### Phase 2 · Roster Picker（1.5 session）
- 交付：
  - Gauntlet UI · Roster tab 可用
  - MapEventSide.AllocateTroops 过滤生效
  - RosterSelection 生命周期管理
- 验收：
  - encounter menu 出现 "Manage Troops" 按钮 · 点开有 UI
  - 选 5 个 Sergeant Crossbowman + 10 个 Legionary · 战斗中只有这 15 个上场（不 spawn Recruits）
  - 战斗结束后重新遭遇 · RosterSelection 已重置

### Phase 3 · Formation Assignment（1.5 session）
- 交付：
  - Gauntlet UI · Formations tab 可用
  - PartyPlanStore JSON 持久化
  - Mission.SpawnTroop Postfix 分配 formation
- 验收：
  - 给 Sergeant Crossbowman 配 Single Formation V · 关游戏重启 · plan 仍在
  - 战斗中确认所有 Crossbowman 在 Formation V
  - 给 Legionary 配 Split (I 权重 3 · II 权重 1) · 确认 spawn 时按比例分入两个 formation

### Phase 4 · OoB Integration + Bug Fix（1 session）
- 交付：
  - OrderOfBattleVM.Initialize Postfix · soft weight distribute
  - 无 class lock · 无 slider lock
- 验收（**关键 · 复现 bug 场景验证 fix**）：
  - 配 Sergeant Crossbowman 到 Formation V
  - 不配 Legionary
  - Roster 选 5 Sergeant + 10 Legionary
  - 战斗中：Sergeant 在 V · Legionary 在其默认 formation I (Infantry) · **两者都上场**
  - OoB 屏上所有 Classes[] 保持 vanilla · slider 未锁死

### Phase 5 · Role Plans + Backline（可选 · 1 session）
- 见 § 2.2

### Phase 6 · Ally 管理 + 边角 case（可选 · 1 session）
- 见 § 2.2

### Phase 7 · 全场景测试（1 session）
- 场景：Field battle (平地 / 山地) · Siege (defender / attacker) · Hideout · Lord's Hall · Tournament (确认不影响)
- 验收：无 crash · 无 log 报错 · 每场景 roster+plan 都符合预期

---

## 10. 依赖清单

### 10.1 直接依赖

| Mod | 版本要求 | 用途 |
|---|---|---|
| Bannerlord.Harmony | ≥ v2.4.2 | Harmony patching |
| Bannerlord.ButterLib | ≥ v2.12 | Utility + GlobalSettings |
| Bannerlord.UIExtenderEx | ≥ v2.13 | Gauntlet UI extension |
| Bannerlord.MBOptionScreen (MCM) | ≥ v5.12 | Settings UI |
| Native / SandBoxCore / Sandbox / StoryMode / CustomBattle | v1.4.7 | Base game |

### 10.2 是否吸收 TroopClassifier？

**决策**：**不依赖 TroopClassifier · 内联所需 API**

原因：
- TC 只提供 `TroopRole` enum + `Classify(CharacterObject) → TroopRole` 静态方法
- 首版不做 Role Plan · 所以只需要基础 FormationClass · vanilla 自带
- 减少 mod 依赖链

Phase 5 引入 Role Plans 时：clean-room 重实现分类逻辑 · 或引入 TC 依赖（届时再决定）

---

## 11. 风险与缓解

| 风险 | 概率 | 影响 | 缓解 |
|---|---|---|---|
| Gauntlet UI 学习曲线 | 高 | Phase 2 拖长到 2-3 session | 参考 EquipmentSpawnerMod 已有 Gauntlet 经验（如有） · 或先做 UIExtenderEx overlay MVP · 后续升级 |
| 反射依赖 Bannerlord 内部 field | 中 | 版本升级易断 | 每反射点 try/catch + log · 反射失败降级到 vanilla · README 记录反射清单 |
| CYT × FM 状态时序错乱 | 中 | 某些 mission 类型 filter 不生效 | Phase 7 全场景测试 · 每场景确认 lifecycle 触发点 |
| License 争议 | 低 | 分发时被 takedown | Clean-room + attribution · 极端情况仅私用不分发 |
| 与 Retinues 冲突 | 中 | Retinues 的 PartyCharacterVMMixin 或 SpawnAgent Prefix 干扰 | 反编译 Retinues 确认无 overlap（memory 2026-09-19 已初步核实：Retinues UI 在 Clan Screen · 与 UTM 独立） |
| 与 RBM 冲突 | 低 | RBM 数值 patch 不干涉我们 | 无 mitigation 需要 |

---

## 12. Open Questions · ✅ 全部已决（2026-09-28 用户拍板）

| ID | 问题 | 决议 |
|---|---|---|
| **O-1** | mod Id/Name | ✅ **`UnifiedTroopManager`** · 显示名 `Unified Troop Manager` |
| **O-2** | Archived / paused plans | ✅ **DROP** · 与 § 2.4 一致 |
| **O-3** | FM 旧 JSON 迁移工具 | ✅ **写简版** · 只拿 `Assignments` 字段（troop → single formation）· 不处理 Split/Role 复杂字段 · Phase 2 附加 +0.3 session |
| **O-4** | UI 框架 | ✅ **Gauntlet full-screen** · Phase 2 = 1.5 session |
| **O-5** | Auto-fill 语义 | ✅ **选项 α · vanilla 顺序补齐** · 与 Bannerlord 默认发兵一致 |
| **O-6** | 多 Preset profile | ✅ DROP（进 COULD backlog G1） |
| **O-7** | Battle summary formation 分配显示 | ✅ DROP（进 COULD backlog I1） |
| **O-8** | Fallback 策略 | ✅ **单点故障细粒度降级** · 每 patch 独立 try/catch · 失败仅该 patch 退化 vanilla · 其它继续工作 · log 警告 |

**所有 Phase 1 blocking Opens 已清空 · 可进入 Phase 1 scaffold**。

---

## 13. 附录 A · 现有 Bug 详细路径（供开发时避坑）

### A.1 FM 的 `OrderOfBattleVMInitializePatch.Postfix` 关键片段

```csharp
// 反编译节选 · 版权归原作者 · 仅用于诊断参考
foreach (OrderOfBattleFormationItemVM item in formationsList) {
    if (item.Formation == null) continue;
    int index = item.Formation.Index;
    DeploymentFormationClass[] array = dictionary[index];
    if (array.Length != 0) {
        item.RefreshFormation(item.Formation, ToNativeCardClass(array), mustExist: true);
        if (item.Classes != null && item.Classes.Count > 0 && array.Length == 1) {
            item.Classes[0].Class = MapToNativeClass(array[0]);       // ← 这里
            if (item.Classes.Count > 1) {
                item.Classes[1].Class = FormationClass.NumberOfAllFormations;  // ← 这里
            }
        }
    }
}
OobWeightDistributor.DistributeWeights(__instance, layout);            // ← 这里权重被硬分
// ...
OobWeightDistributor.LockManagedSliders(__instance, layout);           // ← 这里 lock 死
```

新 mod 的对应部分：**只做 weight hint · 不写 Classes[] · 不 call LockManagedSliders**。

### A.2 CYT 的 `MapEventSidePatches.Prefix` 关键片段

```csharp
customAllocationConditions = delegate(UniqueTroopDescriptor descriptor, MapEventParty party) {
    try {
        CharacterObject troop = party.Troops[descriptor].Troop;
        if (troop == null) return false;
        return _selectedTroops.Contains(troop.StringId);
    } catch { return false; }
};
```

新 mod 保留同类过滤 · 但 `_selectedTroops` 换成 `RosterSelection.Current.Keys`。

---

## 14. 附录 B · 参考文件位置

| 用途 | 路径 |
|---|---|
| FM DLL（反编译源） | `E:\SteamLibrary\...\Modules\FormationManager\bin\Win64_Shipping_Client\FormationManager.dll` |
| CYT DLL | `E:\SteamLibrary\...\workshop\content\261550\2957211804\bin\Win64_Shipping_Client\ChooseYourTroops.dll` |
| TC DLL | `E:\SteamLibrary\...\Modules\TroopClassifier\bin\Win64_Shipping_Client\TroopClassifier.dll` |
| ilspycmd | `C:\Users\situj\.dotnet\tools\ilspycmd.exe` |
| Bannerlord bin | `E:\SteamLibrary\...\bin\Win64_Shipping_Client\` |
| 已有自研 mod 参照 | `EquipmentSpawnerMod\` (工程结构参考) |

---

## 15. 变更记录

| 日期 | 版本 | 内容 |
|---|---|---|
| 2026-09-28 | v0.1 draft | Claude 起草 · 待用户 review |
| 2026-09-28 | v0.2 | 用户拍板 scope · MUST 16 + SHOULD 13 = 29 项首版 · COULD 10 项 backlog 留档 |
| 2026-09-28 | v0.3 | 用户拍板全部 Open Questions · O-1 UnifiedTroopManager · O-3 简版迁移工具 · O-4 Gauntlet · O-5 vanilla 顺序 α · O-8 单点降级 · **Phase 1 可启动** |
| 2026-09-29 | v0.3.1 | Phase 1 Scaffold 完成 · dotnet build 0 warn / 0 err · 部署到 game modules |
| 2026-09-29 | v0.4 | 用户否决 v0.3 § 5.2 "2-tab full-screen" · 改为 **方案 A · Gauntlet 单屏 · 左列 roster + 右侧编组面板** · roster 和编组在同一心智决策 · 拒绝"CYT+FM 简单合并"叙事 · 详见下方 § 16 v0.4 Patch Note |

---

## 16. v0.4 Patch Note · 单屏 UI 方向调整（2026-09-29）

### 16.1 背景

Phase 1 Scaffold 交付后 · 用户拒绝 v0.3 里的 "**Gauntlet full-screen · 2 tab (Roster + Formations)**" 方案 · 两点理由：

1. **Roster 选择和编组分配是同一心智决策** —— 你选什么兵种取决于你想让他们在哪个编组 · 反之亦然。分两个 tab 割裂决策链
2. **拒绝 "CYT 移植 + FM 移植 + 简单合并" 的叙事** —— 两个 mod 是不同设计思路 · 只作参考。UTM 应有独立的 UX 组织方式

### 16.2 § 5.2 UI 选型 · O-4 结论变更

| Open Question | v0.3 决议 | v0.4 决议 |
|---|---|---|
| O-4 · UI 框架 | Gauntlet full-screen · 2 tab（Roster · Formations） | **Gauntlet 单屏 · 左列 roster 列表 + 右侧编组编辑面板** |

### 16.3 § 5.3 布局草图 · 覆盖

```
┌──────────────────────────────────────────────────────────────┐
│ Manage Troops · Battle Size 150/500 · Enemy 320    [Reset]   │
├──────────────────────────────────────────────────────────────┤
│ Party Troops                        Formation Assignment     │
│ ┌─────────────────────────────┐   ┌────────────────────────┐│
│ │ ☑ Sergeant Crossbowman T4   │   │ Selected troop:        ││
│ │   In party: 25   Bring: 15  │◄──┤ Sergeant Crossbowman   ││
│ │   ▶ Formation V (Ranged)    │   │                        ││
│ ├─────────────────────────────┤   │ Mode: ⚫Single ⚪Split ││
│ │ ☑ Legionary          T5     │   │                        ││
│ │   In party: 40   Bring: 30  │   │ Formation: [V ▼]       ││
│ │   ▶ Split I:3 / II:1        │   │  (I / II / III / ...)  ││
│ ├─────────────────────────────┤   │                        ││
│ │ ☐ Recruit            T1     │   │ [Save as default plan] ││
│ │   In party: 50   Bring: 0   │   │                        ││
│ └─────────────────────────────┘   └────────────────────────┘│
│ Total: 45 / 150 · ☑ Auto-fill vanilla order                  │
├──────────────────────────────────────────────────────────────┤
│                            [Cancel]  [Save & Start Battle]   │
└──────────────────────────────────────────────────────────────┘
```

**关键交互**：

- 左侧列表：每兵种一行 · 选中/取消 checkbox · 数量滑块 · 摘要行 "▶ Formation V" 一眼看到当前编组
- 右侧面板：跟随左列表选中项 · Single/Split 单选 · Single 时下拉选 I-VIII · Split 时权重编辑器 · "Save as default plan" 勾选才写 PartyPlan JSON
- 底部：Total 计数 · Auto-fill 复选（vanilla 顺序 · § 12 O-5 决议）· Cancel/Save 按钮

### 16.4 § 5.1 入口点 · 不变

- encounter game menu 加 "Manage Troops" 按钮 · 位置在 vanilla "Battle" 之前
- 点击打开本单屏 · 无 tab 切换 · 全流程在一屏内完成

### 16.5 § 8 · Clean-room 边界补充

除原 § 8.2 六条外，追加：

7. **UI 布局虽然思路上接近 CYT 的 Select Troops 屏 · 但**：
   - 不复制 CYT Gauntlet XML 结构（widget 层级 / 命名 / 属性绑定名）
   - 不复用 CYT ViewModel 字段名 / 命令名 / 事件名
   - 单屏内嵌编组编辑面板是 UTM 独有 · CYT 无此设计
   - **右侧编组面板的语义**思路上接近 FM 的 plan · 但**不 mixin PartyCharacterVM** · 独立 VM · 独立命令

### 16.6 § 2 · 首版 scope 调整

- **A2 · Battle size 显示**：布局顶栏承担 · 无独立控件 · 沿用 v0.3
- **B2 · Split 到多个 formation (Weight 模式)**：**首版 UI shell 阶段先展位不接后端** · 用户实机看观感后决定是否 Phase 3 完整实装 vs 简化为只 Single
- **D3 · 每 formation 预览人数**：布局中未体现 · 暂时降级为 Backlog · 用户可要求补回

### 16.7 分 Phase 重排（v0.4 生效）

Phase 2 拆成 2A / 2B：

| Phase | 内容 | 交付 |
|---|---|---|
| **2A · UI shell** | Gauntlet screen + XML + VM · mock 数据 · 单屏布局落地 · encounter menu 按钮打开 | 用户实机看观感 · 拍板方向 |
| **2B · Roster 实装** | MapEventSide filter · RosterSelection 生命周期 · Party 真实数据绑定 · FM 迁移工具 | Phase 2 完整验收 |

Phase 2B 触发条件：用户看完 2A 观感确认方向 · 说"启动 2B" 或 "确认 A · 继续"。

### 16.8 v0.4 未变更内容

- § 3 数据模型：`PartyPlan` + `TroopFormationPlan` + `RosterSelection` 结构不动
- § 4 合并算法：不动
- § 6 Harmony patch 清单：8 处 target 不动 · 与 UI 无关
- § 7 MCM：不动
- § 9 Phase 3-7 计划：不动 · 仅 Phase 2 拆为 2A/2B
- § 12 其它 Open Questions：不动

---

## ✏️ 用户 Review Checklist

在决定进入 Phase 1 之前请确认：

- [ ] § 2 范围界定合理？有无遗漏想要的功能？
- [ ] § 3 数据模型能表达您的所有 use case？
- [ ] § 4 合并算法是否符合直觉？有无想要的额外行为？
- [ ] § 5 UI 布局草图 OK 吗？tabs 组织好用吗？
- [ ] § 8 clean-room 策略同意吗？
- [ ] § 9 分 phase 计划顺序 OK 吗？某 phase 想提前 / 延后？
- [ ] § 12 Open Questions 全部回答
- [ ] 有无补充的 requirements 未在文档中？
