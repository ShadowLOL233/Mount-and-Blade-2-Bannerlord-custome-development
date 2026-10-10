# 护甲数据总表 · 头盔（HeadArmor）

**生成日期**：2026-10-10
**数据源**：
- `Vanilla_Reference/data/vanilla_items.csv`（Native 原始快照）
- `RBM_Reference/data/rbm_items.csv`（RBM v4.5.0 全量 override）
- `OSA_Reference/data/osa_items.csv`（OSA v2.0.0 + OSW v2.0.1 全量）

**合并规则**（Bannerlord `<Item>` 同 id 后加载者**整节替换**前者）：
Vanilla → RBM 覆盖 → OSA 覆盖 · 本表每行是"玩家实际加载到的"版本。

**生成工具**：
- `src/extract_armor_tables.ps1` · 合并 3 CSV + 算 tier + 分槽输出 JSON
- `src/build_armor_md.ps1` · JSON → ASCII markdown tables
- `src/compose_armor_docs.ps1` · 拼接 Chinese 前言 + ASCII tables

---

## 字段说明

| 字段 | 含义 |
|---|---|
| **StringId** | 游戏内部物品 ID（XML `id`）|
| **Display Name** | 游戏内物品栏显示名（已剥 `{=xxx}` 定位 token）|
| **Mat** | 材质分类（Armor.material_type）· `Cloth` / `Leather` / `Chainmail` / `Plate`（空 `-`）|
| **Tier** | 武器等级 · **公式计算**见下 |
| **Price** | 物品价格（XML `value`）· `auto` 表示由 vanilla 公式自动计算 |
| **H / B / A / L** | head_armor / body_armor / arm_armor / leg_armor 四围防护 |
| **Wt** | 重量 kg |
| **Src** | 数据来源：`Vanilla`（RBM/OSA 都没碰）/ `RBM`（RBM 修过的 vanilla）/ `OSA`（OSA 新增或覆盖）|

### Tier 公式（vanilla `DefaultItemValueModel.CalculateArmorTier` · BALANCE_V2_LOG §Tier）

```
raw    = 1.2·head + body + leg + arm
scaled = raw × typeMultiplier     // HeadArmor 1.2 / BodyArmor 1.0 / Cape 1.8 / LegArmor 1.6 / HandArmor 1.7
tier   = clamp(round(scaled × 0.1 − 0.4), 0, 6) − 1
```

**tier 取值范围** -1 到 5：
- **T-1 / T0**：轻布帽、兜帽、无防护装饰品
- **T1-T2**：轻装/中装常见档位
- **T3-T4**：精锐步兵、骑兵常规档
- **T5**：顶级重装、贵族装、Cataphract 级

---

## 全槽统计

本文档只覆盖 HeadArmor（头盔）· 其他 4 槽见同目录下 `ARMOR_DATA_BODY.md` / `ARMOR_DATA_SHOULDER.md` / `ARMOR_DATA_ARM.md` / `ARMOR_DATA_LEG.md`。

### 按文化排序

文化排序：**empire → vlandia → sturgia → battania → khuzait → aserai → nord → looters → neutral_culture → (none)**。

每文化内：**tier desc → rawArmor desc → stringId asc**。

---

## 数据表

