# 护甲数据总表 · 身甲（BodyArmor）

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
| **H / B / A / L** | head / body / arm / leg 四围防护 |
| **Wt** | 重量 kg |
| **Src** | 数据来源（Vanilla / RBM / OSA）|

### Tier 公式（本槽 typeMultiplier = **1.0**）

```
raw    = 1.2·head + body + leg + arm
scaled = raw × 1.0
tier   = clamp(round(scaled × 0.1 − 0.4), 0, 6) − 1
```

身甲是 5 槽里 typeMultiplier 最低的 · 同 raw 下 tier 比其他 4 槽低 1-2 档（意味着身甲"数值密度"要求高）。

---

## 数据表

本文档只覆盖 BodyArmor（身甲）· 其他 4 槽见同目录。
文化排序：**empire → vlandia → sturgia → battania → khuzait → aserai → nord → looters → neutral_culture → (none)**。
每文化内：**tier desc → rawArmor desc → stringId asc**。

