# Vanilla + RBM 装备参考底表

**用途**：OSA 物品平衡 v2 的**权威参照数据源**。所有 OSA 物品优先按此表 (base_type + aventail_type) 匹配对应 vanilla 物品直接抄数值；无 vanilla 对应的 OSA-only 物品用家族均值 fallback。

**数据源**：`RBM_Reference/data/rbm_items.csv`（RBM 团队已完成的 vanilla 数值调整）。

**提取日期**：2026-09-22。

**RBM 命名规则**（用于分类）：
- **前缀**：`leatherlame_` (皮革边缘/软构造) / `ironlame_` (铁片边缘/硬构造) / `heavy_` (最重构造) / 无前缀 (标准) / `imperial_` (帝式变体) / `spiked_` (尖顶)
- **base_type**：Cloth Coif / Padded Coif / Leather Coif / Mail Coif / Roundkettle / Spiked Kettle / Spangenhelm / Nasalhelm / Tall Helmet / Faceguard / Goggled / Lord Helmet / Guarded Lord / Crown
- **后缀 aventail**：无 / `_over_imperial_cloth` / `_over_laced_cloth` / `_over_imperial_padding` / `_over_imperial_leather` / `_over_imperial_mail` / `_over_imperial_coif`

---

## Empire · HeadArmor · 54 件

### 📖 Coif 类（无金属帽壳，仅软/链甲头罩）

| id | mat | wt | h/b/a | 名称 |
|---|---|---:|---|---|
| `laced_cloth_coif` | Cloth | 0.4 | 14/0/0 | Laced Cloth Coif |
| `imperial_cloth_coif` | Cloth | 0.2 | 17/0/0 | Cloth Coif |
| `imperial_padded_coif` | Cloth | 0.5 | 22/7/22 | Lightweight Padded Coif |
| `leather_cap` | Leather | 0.4 | 14/0/0 | Leather Cap |
| `imperial_leather_coif` | Leather | 0.3 | 23/0/0 | Leather Coif |
| `open_mail_coif` | Chainmail | 1.6 | 37/0/0 | Open Mail Coif |
| `imperial_open_mail_coif` | Chainmail | 1.5 | 39/0/0 | Open Mail Coif |
| `mail_coif` | Chainmail | 1.7 | 38/12/38 | Mail Coif with Helmet Padding |
| `imperial_mail_coif` | Chainmail | 1.7 | 41/0/0 | Mail Coif |

### 🪖 Roundkettle 家族（圆锅盔）

| id | wt | h/b/a | 名称 |
|---|---:|---|---|
| `leatherlame_roundkettle` | 1.8 | 47/0/0 | Roundkettle Helmet (bare) |
| `roundkettle_over_imperial_cloth` | 1.2 | 64/0/6 | Roundkettle over Cloth |
| `roundkettle_over_laced_cloth` | 1.3 | 65/0/10 | Roundkettle Over Laced Cloth |
| `roundkettle_over_imperial_padding` | 1.4 | 67/0/16 | Roundkettle over Padding |
| `leatherlame_roundkettle_over_imperial_leather` | 1.4 | 58/12/7 | Roundkettle over Leather (leatherlame) |
| `roundkettle_over_imperial_leather` | 1.3 | 74/0/0 | Light Roundkettle over Leather |
| `ironlame_roundkettle_over_imperial_leather` | 3.1 | 56/0/15 | Iron Roundkettle over Leather |
| `leatherlame_roundkettle_over_imperial_mail` | 3.4 | 74/0/25 | Roundkettle over Mail (leatherlame) |
| `roundkettle_over_imperial_mail` | 3.3 | 92/0/20 | Roundkettle over Mail |
| `ironlame_roundkettle_over_imperial_mail` | 3.6 | 78/0/25 | Iron Roundkettle over Mail |

### 🔺 Spiked Kettle（尖顶锅盔）

| id | wt | h/b/a | 名称 |
|---|---:|---|---|
| `spiked_kettle_over_imperial_padding` | 1.5 | 60/3/17 | Spiked Kettle over Padding |
| `spiked_kettle_over_imperial_mail` | 3.6 | 90/4/20 | Spiked Kettle over Mail |
| `ironlame_spiked_kettle_over_mail` | 3.8 | 73/0/25 | Iron Spiked Kettle over Mail |

### ⭐ Spangenhelm 家族（拼片盔）· 6 件

| id | wt | h/b/a | 名称 |
|---|---:|---|---|
| `ironlame_feathered_spangenhelm` | 3.2 | 65/12/25 | Feathered Spangenhelm (bare-ish) |
| `leatherlame_feathered_spangenhelm_over_leather` | **1.7** | **76/12/15** | **Spangenhelm over Leather** ← 用户 anchor A |
| `ironlame_feathered_spangenhelm_over_leather` | 2.8 | 77/12/15 | Feathered Spangenhelm over Leather |
| `ironlame_feathered_spangenhelm_over_mail` | **3.3** | **95/12/25** | **Spangenhelm over Mail** ← 用户 anchor B |
| `leatherlame_feathered_spangenhelm_over_mail` | 3.4 | 97/22/45 | Feathered Spangenhelm over Mail (leatherlame) |
| `feathered_spangenhelm_over_imperial_coif` | 3.1 | 88/22/40 | Feathered Spangenhelm over Coif |

### 🎯 Nasalhelm 家族（鼻护盔）· 11 件

| id | wt | h/b/a | 名称 |
|---|---:|---|---|
| `ironlame_nasalhelm_over_imperial_cloth` | 3.2 | 55/12/6 | Iron Legionary Helm over Cloth |
| `ironlame_nasalhelm_over_imperial_padding` | 3.4 | 57/15/25 | Iron Nasalhelm over Padding |
| `leatherlame_nasalhelm_over_imperial_leather` | 1.9 | 61/12/17 | Iron Nasalhelm over Leather |
| `ironlame_nasalhelm_over_imperial_coif` | 4.1 | 85/22/45 | Iron Nasalhelm over Coif |
| `leatherlame_nasalhelm_over_imperial_mail` | 3.7 | 87/12/25 | Bronze Nasalhelm over Mail |
| `ironlame_nasalhelm_over_imperial_mail` | 3.9 | 90/12/25 | Iron Nasalhelm over Mail |
| `imperial_nasal_helm` | 2.2 | 97/12/25 | Legionary Helm |
| `heavy_nasalhelm_over_laced_cloth` | 3.1 | 97/36/12 | Heavy Nasalhelm over Laced Cloth |
| `heavy_nasalhelm_over_imperial_padding` | 3.2 | 100/41/25 | Heavy Nasalhelm over Padding |
| `heavy_nasalhelm_over_imperial_leather` | 3.0 | 105/36/12 | Heavy Nasalhelm over Leather |
| `heavy_nasalhelm_over_imperial_mail` | 3.6 | **124/36/20** | Heavy Nasalhelm over Mail |

### 👑 Lord / Cataphract / 特殊（高端）· 9 件

| id | wt | h/b/a | 名称 |
|---|---:|---|---|
| `tall_helmet` | 1.8 | 84/0/0 | Tall Helmet |
| `helmet_with_faceguard` | 3.5 | 94/12/0 | Helmet with Faceguard |
| `plumed_helmet` | 2.9 | 104/24/0 | Plumed Helmet |
| `empire_battle_crown_north` | 2.1 | 104/12/0 | Jeweled Plumed Battle Crown |
| `empire_battle_crown_west` | 3.1 | 109/12/0 | Imperial Plumed Helmet |
| `empire_helmet_with_metal_strips` | 3.8 | 120/14/20 | Lord Helmet with Metal Strips |
| `empire_lord_helmet` | 3.5 | 123/10/0 | Noble Guard Helmet |
| `empire_jewelled_helmet` | 3.8 | 125/14/20 | Imperial Jeweled Helmet |
| `empire_guarded_lord_helmet` | 3.7 | **130/30/0** | Royal Cataphract Helmet |
| `imperial_goggled_helmet` | 4.2 | **144/82/45** | **Goggled Closed Cataphract Helmet** (顶配) |

### 💍 Crown 类（冠饰，非战斗头盔）· 5 件

| id | wt | h/b/a | 名称 |
|---|---:|---|---|
| `empire_crown_north` | 0.3 | 15/0/0 | Golden Laurel Crown |
| `empire_crown_v2` | 0.85 | 15/0/0 | Imperial Jeweled Band |
| `empire_crown_west` | 0.5 | 15/0/0 | Imperial Jeweled Band |
| `vlandia_crown` | 0.35 | 15/0/0 | Austere Crown |
| `empire_crown` | 3.1 | 46/0/0 | Jeweled Crown |

---

## Aventail Suffix 查表（用于 OSA-only 物品 auto-derive）

按 aventail 类别看 vanilla 的典型 b/a 范围：

| Aventail | 典型 body | 典型 arm | wt 增量 | 备注 |
|---|---:|---:|---:|---|
| 无 (bare) | 0 | 0 | — | 无护颈 |
| `over_imperial_cloth` | 12 | 6 | +0.2 | 布垫细颈 |
| `over_laced_cloth` | 0-36 | 10-12 | +0.2 | 编织布 |
| `over_imperial_padding` | 15-41 | 17-25 | +0.4 | 加厚布 (varies wildly) |
| `over_imperial_leather` | 12 | 15-17 | +0.5 | 皮革颈甲 |
| `over_imperial_mail` | 12-22 | 20-25 | +1.5 | 链甲颈甲 (标准) |
| `over_imperial_coif` | 22 | 40-45 | +1.6 | 完整头罩链甲 |

## Prefix 修饰词（构造质量）

| 前缀 | 典型效果 vs 无前缀 |
|---|---|
| `leatherlame_` | 皮革边缘，wt -0.5 ~ -1.5，b/a 大致相同或略高 |
| `ironlame_` | 铁片边缘，wt +0.3 ~ +0.5，head +5 ~ +10 |
| `heavy_` | 最重构造，vs `ironlame_`: **head +25 ~ +35**, **body +20 ~ +25**, arm -5 |
| 无前缀 (baseline) | 标准 |
| `imperial_` | 帝式外观变体，数值同 baseline |

**关键观察**：`heavy_` 前缀是一个**独立的 tier 跳跃**，vs 我 v1 方案里的 +5/+2/+2 delta，vanilla 实际 delta 大 5-6 倍。这是重新平衡 OSA 时最关键的修正点之一。

---

## 材质品质字典

**已迁移到独立文档**：[`MATERIAL_QUALITY_DICT.md`](./MATERIAL_QUALITY_DICT.md)（2026-09-22 抽离）

该文档维护所有品质修饰词（Lord/Gilded/Iron/Bronze/Face Plate/Metal Stripes/Ridge/Silvered 等）的 delta 定义、叠加规则、优先级规则和修订历史。任何新词入库先在 [`BALANCE_V2_LOG.md`](./BALANCE_V2_LOG.md) 立项讨论。

---

## OSA-only 物品的 Auto-derive 规则

按用户 2026-09-22 决议，无 vanilla 对应的 OSA 物品自动取家族均值：

1. **解析 OSA 物品名** → (base_type, aventail_type, construction_prefix, decorative_modifiers)
2. **查 vanilla 底表**：
   - 若有 (base_type, aventail_type) 完全匹配 → 直接抄数值
   - 若只有 base_type 匹配 → 用该 base_type 家族均值，b/a 按 aventail suffix 查表调整
   - 若都不匹配 → fallback 到 aventail suffix 表的典型值 + 相似 base_type 家族的 head 均值
3. **decorative_modifiers 一律不影响护甲**（Plumed / Crested / Crowned / Feathered / Gilded / Jeweled / Southern / Eastern / Bronze / Iron / Noble / Decorated / Redcrest）
4. **construction_prefix**（heavy/cataphract/guarded）应用 vanilla delta：
   - `Heavy X` → 找 `heavy_X_*` vanilla 参照
   - `Cataphract` → 找 `empire_lord_helmet` / `empire_guarded_lord_helmet` / `imperial_goggled_helmet` 参照
   - `Guarded` → 找 `empire_guarded_lord_helmet` 参照

---

## Empire · Cape · 17 件（2026-09-23 归档）

**关键设计观察**：vanilla+RBM 全 17 件 Empire Cape 的 **arm_armor = 0**（RBM 设计意图：Cape 是纯 body_armor / mantle · 无肩甲延伸）。OSA 在 Cape 上加了 arm（pauldron/spaulder 结构）是 **OSA 系统性设计特色**（2026-09-20 旧日志已确认）。

### 📖 Leather 类（6 件 · body 12-27 · 皮革护肩/背带）

| id | wt | b/a | 名称 |
|---|---:|---|---|
| `a_empire_plated_shoulder_a` | 4.1 | 12/0 | Legionary Studded Harness |
| `woven_leather_shoulders` | 1.6 | 16/0 | Woven Leather Shoulders |
| `varangian_bra_basic` | 2.5 | 18/0 | Decorated Leather Harness |
| `varangian_bra_royal` | 2.7 | 22/0 | Caped Leather Harness |
| `empire_warrior_padded_armor_shoulder` | 1.6 | 24/0 | Legionary Padded Straps |
| `varangian_bra_padded` | 3.2 | 27/0 | Decorated Leather Harness with Padding |

### 🔗 Chainmail 类（1 件 · body 35）

| id | wt | b/a | 名称 |
|---|---:|---|---|
| `varangian_bra_mail` | 3.2 | 35/0 | Decorated Leather Harness over Mail |

### 🛡 Plate 类（10 件 · body 15-55 · Pauldron/Lamellar/Scale）

| id | wt | b/a | 名称 |
|---|---:|---|---|
| `a_pauldron_cape_c` | 3.5 | 15/0 | Bronze Pauldrons |
| `empire_plate_armor_shoulder_a` | 3.6 | 17/0 | Bronze Plate Pauldrons |
| `a_pauldron_cape_b` | 3.5 | 20/0 | Bronze Pauldrons with Neck Guard |
| `empire_plate_armor_shoulder_b` | 3.6 | 21/0 | Iron Plate Pauldrons |
| `a_empire_plated_shoulder_b` | 4.1 | 22/0 | Lamellar Pauldrons |
| `pauldron_cape_a` | 3.5 | 30/0 | Legionary Cape |
| `studded_imperial_neckguard` | 3.6 | 31/0 | Neckguard with Bronze Plate Pauldrons |
| `imperial_studded_strip_shoulders` | 4.1 | 34/0 | Legionary Reinforced Studded Harness |
| `varangian_bra_scale` | 4.0 | 40/0 | Decorated Leather Harness over Scale |
| **`imperial_lamellar_shoulders`** | 3.5 | **55/0** | **Heavy Lamellar Pauldrons** ⭐ 顶点 |

### Cape 家族梯度 · Empire

```
Cloth / 无 Cape                                    body 0-5
Leather 护肩 (Legionary Studded/Padded/Woven)      body 12-27
Chainmail 护肩 (varangian_bra_mail)                body 35
Plate Pauldron (Bronze/Iron/Neckguard)             body 15-31
Plate Heavy Lamellar (imperial_lamellar_shoulders) body 55 ⭐ 顶点
```

### Cape 家族关键约束

- **vanilla Empire Cape 顶点**：`imperial_lamellar_shoulders` **55/0/3.5**（raw = 55 · scaled with Cape mult 1.8 = 99）
- **OSA Empire Cape 需在此顶点内**：raw body ≤ 55
- **OSA arm 特色可保留**：OSA 明确添加 arm_armor（0-12）反映 pauldron mesh 的物理外形，vanilla 侧无对应设计
- **头 > 身 > 臂原则不适用**（Cape 无 head_armor 字段）
- **拟议 Cape 铁律**（待用户确认）：`body_armor ≥ arm_armor`（斗篷主体 > 肩甲延伸）
