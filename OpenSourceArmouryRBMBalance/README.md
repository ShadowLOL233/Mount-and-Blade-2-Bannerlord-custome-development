# Open Source Armoury RBM Balance Patch

自研 Bannerlord v1.4.7 纯 XML 覆盖 mod，把 OSA / OSW 的装备与武器 piece 数值校准到 RBM 尺度。

## 做了什么

1. **护甲 buff**：按 (Type, material, tier, slot) 分层查 RBM 平均值，`max(1.0, factor)` 只升不降；跳过 `factor ≤ 1.05` 的近相等 cell；skip `LegArmor` 里所有 `shoes|boots|moccasins` 命名的鞋子；`Cape.arm` slot 特例目标 12（RBM 无基线）
2. **头盔颈/肩延伸**：`material_type ∈ {Chainmail, Plate}` 且 tier ≥ 4 的头盔自动补 `body_armor = head × 0.43` + `arm_armor = head × 0.37`——模仿 RBM 头盔覆盖颈甲 + 肩甲的 aventail 结构
3. **武器 Blade piece damage_factor**：按 wclass (axe/mace/sword_1h/spear/polearm) 拉到 RBM 池均值——RBM 有意将武器伤害压缩 ~30%，本 mod 补上这个下调；spear/mace thrust 保持 buff 方向

**Guard / Handle / Pommel piece 无 damage_factor 字段**（Bannerlord piece 数据结构限制），不动。

## 详细规则

见 repo 根 `ModdingJournal.md` 中 "OSA 护甲 vs RBM 跨槽覆盖分析" + "OSA 武器 damage_factor 完整对照表" 两条 backlog（三次修订版）。tier 公式来源 `TaleWorlds.Core.DefaultItemValueModel.CalculateArmorTier`（ilspycmd 反编译核实）。

## 目录结构

```
OpenSourceArmouryRBMBalance/
├── SubModule.xml                        # 依赖 + XML 加载声明
├── ModuleData/
│   ├── OSABalance_armor_override.xml    # 1507 件 armor override（Items）
│   └── OSABalance_pieces_override.xml   # 18 件 weapon Blade piece override
├── src/
│   └── generate.ps1                     # 生成器，输入 OSA/OSW/RBM XML，产出上面两个 override
├── deploy.ps1                           # 拷贝到 Modules
└── README.md
```

## 前置

- 装 **Open Source Armory** (Workshop 3011479883)
- 装 **Open Source Weaponry** (Workshop 3010984416)
- 装 **RBM** (Workshop 2859251492) + **RBM_WS** (Workshop 3635788184)
- 加载顺序：**Native → SandBoxCore → Sandbox → StoryMode → CustomBattle → RBM → OSA → OSW → 本 mod**（SubModule.xml 的 DependedModuleMetadata 已强制此顺序）

## 部署

```powershell
# 关掉 launcher（虽然 XML 不锁，但下次启动才生效）
cd C:\Users\situj\git\Mount-and-Blade-2-Bannerlord-custome-development\OpenSourceArmouryRBMBalance
.\deploy.ps1
```

启动器里勾选 **Open Source Armoury RBM Balance Patch**，拖到 OSA/OSW/RBM 之后。

## 重新生成 override（OSA/OSW/RBM 版本升级后）

```powershell
cd C:\Users\situj\git\Mount-and-Blade-2-Bannerlord-custome-development\OpenSourceArmouryRBMBalance
.\src\generate.ps1
.\deploy.ps1
```

生成器完全 idempotent、自动读取 OSA/RBM workshop 当前版本。

## 卸载

启动器取消勾选、或删 `Modules\OpenSourceArmouryRBMBalance\`。Item id 不再被本 mod 覆盖后立刻回到 OSA 原值，存档兼容（无 SaveableField，无副作用）。

## 采用的关键决策（用户 2026-09-20 拍板）

| 决议点 | 决议 |
|---|---|
| Chainmail T6 头盔（OSA 已超 RBM）| 不动（`max(1.0, factor)` 规则自然跳过）|
| Leather T2/T3 身甲（OSA 已超 RBM）| 不动（同上）|
| Cape.arm slot（RBM 无基线）| flat target = 12 avg（×1.74 系数）|
| OSA cloth leg 15 件（其实全是鞋）| 全部跳过（id keyword 排除）|
| 武器 damage_factor | 允许下调（RBM 有意 ~30% 削弱武器伤害）|

## 兼容性

- 无 Harmony patch，无 DLL，无 CampaignBehavior，无 SaveableField
- 其他 mod 若也覆盖同 id 的 Item / CraftingPiece，取加载顺序更靠后的
- 与 Retinues / CYT / FormationManager 无冲突（本 mod 只碰 `<Item>` 数值和 `<CraftingPiece>` damage_factor）

## 版本

- v1.0.0（2026-09-20）
- 数据源：OSA v2.0.0 + OSW v2.0.0 + RBM v4.5.0 + RBM_WS v4.5.0
- 游戏：Bannerlord Native v1.4.7 (build 117484)
