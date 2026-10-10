# 护甲数据总表 · 臂甲 / 手甲（HandArmor）

**生成日期**：2026-10-10
**数据源**：Vanilla（Native）→ RBM v4.5.0 override → OSA v2.0.0 override · 整节替换语义合并。
**生成工具**：`src/extract_armor_tables.ps1` + `src/build_armor_md.ps1` + `src/compose_armor_docs.ps1`

---

## 字段说明

| 字段 | 含义 |
|---|---|
| **StringId** | 游戏内部物品 ID |
| **Display Name** | 游戏内物品栏显示名（已剥 `{=xxx}`）|
| **Mat** | 材质分类（Cloth / Leather / Chainmail / Plate）|
| **Tier** | 等级 · 公式见下 |
| **Price** | XML `value` · `auto` = 由公式自动算 |
| **H / B / A / L** | head / body / arm / leg 四围防护（本槽主要看 **A**）|
| **Wt** | 重量 kg |
| **Src** | 数据来源（Vanilla / RBM / OSA）|

### Tier 公式（本槽 typeMultiplier = **1.7**）

```
raw    = 1.2·head + body + leg + arm
scaled = raw × 1.7
tier   = clamp(round(scaled × 0.1 − 0.4), 0, 6) − 1
```

臂甲 typeMultiplier 仅次于 Cape · 同 arm_armor 下 tier 比 head 等高一档。

---

## 数据表

本文档只覆盖 HandArmor（臂甲 / 手甲 / 护腕）· 其他 4 槽见同目录。
文化排序：**empire → vlandia → sturgia → battania → khuzait → aserai → nord → looters → neutral_culture → (none)**。
每文化内：**tier desc → rawArmor desc → stringId asc**。

