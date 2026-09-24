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

---

## Empire · BodyArmor · 39 件（2026-09-23 归档）

**关键设计观察**：vanilla+RBM 帝国 BodyArmor 全线走**材质档次 + 结构层叠**双维度：材质决定基础档（Cloth < Leather < Chainmail < Plate），结构描述（Over Padded / Over Mail / Over Leather / Scale Skirt / Double Mail）决定同材质内部细分。分档**极其规整**：Cloth 5-28、Leather 18-32、Chainmail 45-55、Plate 37-135。**顶点** `imperial_scale_armor` 135/122/67 ⭐

### 👕 Cloth 民用（16 件 · body 5-14 · 无 subarmalis）

| id | wt | b/l/a | 名称 |
|---|---:|---|---|
| `hemp_tunic` | 0.4 | 5/5/5 | Hemp Tunic |
| `empire_dress` | 0.7 | 6/5/6 | Ladies Dress |
| `fine_town_tunic` | 0.4 | 6/5/5 | Rich Tunic |
| `empire_dress_b` | 0.6 | 7/7/6 | Red Dress |
| `peasant_costume` | 0.5 | 7/7/5 | Commoner Clothes |
| `tunic_with_shoulder_pads` | 0.6 | 7/7/7 | Tunic with Shoulder Pads |
| `vlandian_dress` | 0.5 | 7/7/5 | Rich Dress |
| `empire_short_dress` | 1.0 | 8/3/0 | Short Tunic |
| `footmans_tunic` | 0.6 | 8/8/8 | Military Tunic |
| `imperial_robes` | 0.9 | 8/8/6 | Toga |
| `tied_cloth_tunic` | 0.4 | 8/8/5 | Tied Cloth Tunic |
| `tunic_with_rolled_cloth` | 0.7 | 11/11/7 | Tunic with Rolled Cloth |

### 🧥 Padded Gambeson/Cloth（8 件 · body 14-28 · Cloth mat + gambeson 结构）

| id | wt | b/l/a | 名称 |
|---|---:|---|---|
| `patched_gambeson` | 2.4 | 14/14/12 | Patched Gambeson |
| `imperial_padded_cloth` | 2.8 | 16/16/15 | Infantryman Rough Gambeson |
| `empire_warrior_padded_armor_a` | 2.1 | 18/18/14 | Infantryman Gambeson |
| `empire_warrior_padded_armor_e` | 1.5 | 18/18/15 | Infantryman Long Gambeson |
| `empire_warrior_padded_armor_c` | 2.0 | 22/23/13 | Infantryman Gambeson with Skirts |
| `empire_warrior_padded_armor_d` | 1.6 | 22/19/11 | Infantryman Gambeson with Straps |
| `padded_cloth_with_strips` | 2.2 | 22/25/13 | Padded Cloth With Strips |
| `empire_warrior_padded_armor_f` | 1.9 | 25/23/18 | Auxiliary Armor |
| `empire_warrior_padded_armor_g` | 2.0 | 26/25/19 | Auxiliary Armor with Straps |
| `empire_warrior_padded_armor_b` | 1.8 | 28/27/16 | Infantryman Gambeson over Leather Jacket ⭐ Padded 顶 |

### 🦌 Leather（5 件 · body 18-32 · Leather mat）

| id | wt | b/l/a | 名称 |
|---|---:|---|---|
| `leather_tunic` | 2.1 | 18/23/15 | Leather Tunic |
| `khuzait_leather_stitched` | 2.5 | 22/22/10 | Thick Brigandine Vest（帝国可用）|
| `basic_imperial_leather_armor` | 3.1 | 28/22/25 | Leather Armor |
| `woven_leather_coat` | 5.1 | 30/30/30 | Boarskin Leather Coat |
| `eastern_studded_leather` | 3.0 | 32/24/27 | Cured Studded Leather Armor ⭐ Leather 顶 |

### 🛡 Plate 轻档 · Scale Vest（1 件 · body 37）

| id | wt | b/l/a | 名称 |
|---|---:|---|---|
| `empire_plate_vest_armor` | 16 | 37/30/12 | Ornate Scale Armor |

### 🔗 Chainmail Vest / Over Leather（6 件 · body 45-55 · Chainmail mat）

| id | wt | b/l/a | 名称 |
|---|---:|---|---|
| `imperial_mail_vest` | 9.8 | 45/34/39 | Infantryman Mail Vest |
| `imperial_mail_over_stripped_leather` | 10.2 | 46/27/30 | Infantryman Mail over Striped Leather |
| `imperial_mail_over_leather` | 8.6 | 47/22/33 | Infantryman Mail over Leather |
| `empire_horseman_armor` | 8.3 | 55/27/38 | Cavalryman Mail Shirt |
| `empire_legion_a` | 22 | 55/44/18 | Decorated Legionary Mail |
| `legionary_mail` | 10.5 | 55/49/44 | Legionary Mail ⭐ Chainmail 顶 |

### 🛡 Plate 顶档 · Lamellar/Scale over Mail（5 件 · body 79-135）

| id | wt | b/l/a | 名称 |
|---|---:|---|---|
| `imperial_lamellar` | 28.5 | 79/78/45 | Light Lamellar over Mail Armor |
| `empire_legion_b` | 28 | 85/30/20 | Ornate Legionary Scale Mail |
| `imperial_lamellar_over_leather` | 15 | 88/30/35 | Luxury Lamellar Vest over Leather |
| `lamellar_with_scale_skirt` | 34 | 118/122/45 | Heavy Lamellar over Mail with Scale Skirt |
| `imperial_scale_armor` | 35.5 | **135/122/67** | Heavy Scale Armor over Double Mail ⭐ **顶点** |

### BodyArmor 家族梯度 · Empire

```
Cloth 民用（tunic/dress/toga）                     body 5-11
Padded Gambeson                                    body 14-28 ⭐ Padded 顶
Leather Tunic/Coat/Studded                         body 18-32 ⭐ Leather 顶
Plate 轻档（Scale Vest）                            body 37（arm 12 唯一）
Chainmail Vest/Over Leather                        body 45-47
Chainmail Legion（Cavalryman/Legionary）            body 55 ⭐ Chainmail 顶
Plate 中高档（Lamellar over Mail）                  body 79-88
Plate 顶点（Heavy Lamellar/Scale over Mail）        body 118-135 ⭐ 顶点
```

### BodyArmor 家族关键约束

- **vanilla Empire BodyArmor 顶点**：`imperial_scale_armor` **135/122/67**（Heavy Scale over Double Mail）· raw scaled with BodyArmor mult 1.0
- **OSA Empire BodyArmor 需在此顶点内**：body ≤ 135
- **序**：`body_armor ≥ leg_armor > arm_armor`（vanilla 全 39 件遵守，仅 `lamellar_with_scale_skirt` 118/122 leg 略高——因 Scale Skirt 结构强调腿部）
- **arm 上限**：Chainmail 顶档 44（`legionary_mail`），Plate 顶档 67（`imperial_scale_armor`）
- **leg 上限**：Plate 顶档 122（`lamellar_with_scale_skirt` / `imperial_scale_armor`）
- **material_type 分档硬约束**：Cloth ≤ 28（Padded 顶）· Leather ≤ 32 · Chainmail 45-55 · Plate 37-135
- **结构描述子分档**：`Over Padded/Leather` 中档 · `Over Mail` 高档 · `Over Scale/Double Mail` 顶档 · `Scale Skirt` leg 强化
- **Cataphract 定义**：body ≥ 79（Lamellar over Mail 起）+ leg ≥ 45 + arm ≥ 20 = 三档全上 → Empire Cataphract 顶级重装

---

## Empire · HandArmor · 7 件（2026-09-23 归档）

**关键设计观察**：vanilla+RBM 帝国 HandArmor 全 7 件 body_armor 均为 0（HandArmor 只有 arm_armor 字段有效——这类物品覆盖手部，护甲值全走 arm）。分档极简：**Leather 26 → Cloth Padded 31-37 → Plate 42-63**。顶点 `lamellar_plate_gauntlets` 63 ⭐

### 🦌 Leather（1 件 · arm 26）

| id | wt | arm | 名称 |
|---|---:|---:|---|
| `woven_leather_bracers` | 0.8 | 26 | Woven Leather Bracers |

### 🧥 Cloth Padded Mittens（3 件 · arm 31-37）

| id | wt | arm | 名称 |
|---|---:|---:|---|
| `padded_mitten` | 0.6 | 31 | Padded Mittens |
| `lordly_padded_mitten` | 1.3 | 32 | Lordly Padded Mittens |
| `reinforced_padded_mitten` | 0.9 | 37 | Reinforced Padded Mittens |

### 🛡 Plate（3 件 · arm 42-63）

| id | wt | arm | 名称 |
|---|---:|---:|---|
| `plated_strip_gauntlets` | 1.4 | 42 | Plated Striped Vambraces |
| `decorated_imperial_gauntlets` | 1.5 | 50 | Decorated Imperial Gauntlets |
| `lamellar_plate_gauntlets` | 1.8 | **63** | Lamellar Plate Gauntlets ⭐ 顶点 |

### HandArmor 家族梯度 · Empire

```
Leather Bracers                                arm 26
Cloth Padded Mittens (Padded/Lordly)           arm 31-32
Cloth Padded Mittens (Reinforced)              arm 37
Plate Vambraces (Plated Strip)                 arm 42
Plate Imperial Gauntlets (Decorated)           arm 50
Plate Lamellar Gauntlets                       arm 63 ⭐ 顶点
```

### HandArmor 家族关键约束

- **body_armor 恒为 0**：HandArmor 类 body_armor 字段无效——所有护甲值集中在 arm_armor
- **arm 上限 63**（`lamellar_plate_gauntlets`）· OSA arm ≤ 63
- **wt 分档**：Cloth 轻档 0.4-0.9 · Leather 0.8 · Plate 中档 1.4-1.8
- **材质分档**：Cloth Padded ≤ 37 · Leather ≤ 26 · Plate 42-63
- **命名子结构**：Vambraces（前臂）< Gauntlets（全手）· Bracers 轻档 · Mittens 布软档 · Splint/Mail 复合结构档

---

## Empire · LegArmor · 7 件（2026-09-23 归档）

**关键设计观察**：vanilla+RBM 帝国 LegArmor 全 7 件 body_armor 均为 0（LegArmor 只有 leg_armor 字段）。分档梯度**极其规整**：Leather 20-28 → Cloth 30 → Plate 42-62。顶点 `lamellar_plate_boots` 62 ⭐

### 🦌 Leather（3 件 · leg 20-28）

| id | wt | leg | 名称 |
|---|---:|---:|---|
| `fine_town_boots` | 0.5 | 20 | Fine Town Boots |
| `folded_town_boots` | 1.0 | 24 | Folded Town Boots |
| `empire_horseman_boots` | 0.8 | 28 | Horseman Boots |

### 👕 Cloth Strapped（1 件 · leg 30）

| id | wt | leg | 名称 |
|---|---:|---:|---|
| `strapped_leather_boots` | 0.9 | 30 | Strapped Leather Boots |

### 🛡 Plate（3 件 · leg 42-62）

| id | wt | leg | 名称 |
|---|---:|---:|---|
| `plated_strip_boots` | 2.7 | 42 | Splint Boots |
| `decorated_imperial_boots` | 2.3 | 44 | Decorated Plate Boots |
| `lamellar_plate_boots` | 3.5 | **62** | Lamellar Plate Boots ⭐ 顶点 |

### LegArmor 家族梯度 · Empire

```
Leather Boots (Fine Town / Folded / Horseman)         leg 20-28
Cloth Strapped Leather Boots                          leg 30
Plate Splint Boots (Plated Strip)                     leg 42
Plate Decorated Boots                                 leg 44
Plate Lamellar Boots                                  leg 62 ⭐ 顶点
```

### LegArmor 家族关键约束

- **body_armor 恒为 0**：LegArmor 类 body_armor 字段无效——所有护甲值集中在 leg_armor
- **leg 上限 62**（`lamellar_plate_boots`）· OSA leg ≤ 62
- **wt 分档**：Leather 轻档 0.5-1.0 · Cloth 0.9 · Plate 中档 2.3-3.5
- **材质分档**：Cloth Slippers ≤ 30 · Leather ≤ 28 · Plate 42-62
- **命名子结构**：Slippers/Shoes（极轻）< Boots（标准）< Boots With Greaves（+ 金属护胫）< Lamellar Plate Boots（顶档全 lamellar 覆盖）

---

## Empire · HorseHarness · 4 件（2026-09-23 归档 · 全 4 字段修订）

**关键设计观察**：vanilla+RBM 帝国 HorseHarness 仅 **4 件**——但**每件都用全 4 armor 字段**（head/body/arm/leg），OSA v1 只用了 body 一字段是漏洞。分档结构：民用 Leather 5-10 → Cataphract 顶档（Chainmail Scale）h=90/b=50/l=50/a=60。**顶点实际是 head 90 而非 body 50**。

### HorseHarness 4 字段对应马部位

| 字段 | 马部位 | vanilla 顶点 | 说明 |
|---|---|---:|---|
| **head_armor** | 马头（chamfron 面甲）| **90** ⭐ | 顶档 Cataphract 硬约束 · 民用 5-10 |
| **body_armor** | 马身/胸腹（main barding） | 50 | 民用 5-8 |
| **arm_armor** | 马颈/前腿（crinet + peytral）| 60 | 民用 5-10 · 顶档比 body 高 |
| **leg_armor** | 马后腿/臀（crupper）| 50 | Half 只 5 · Full 50 · Half vs Full 差异关键点 |

### 🐴 全 4 件详细数据

| id | mat | wt | h/b/l/a | 名称 |
|---|---|---:|---|---|
| `stripped_leather_harness` | Leather | 8 | **10/5/5/10** | Striped Leather Harness（民用）|
| `imperial_riding_harness` | Leather | 6 | **5/8/3/5** | Imperial Riding Harness（民用）|
| `half_scale_barding` | Chainmail | 17 | **90/50/5/60** | Cataphract Half Scale Barding（半覆盖 · **leg 只 5**）|
| `imperial_scale_barding` | Chainmail | 30 | **90/50/50/60** ⭐ | Cataphract Scale Barding（全覆盖顶点）|

### HorseHarness 家族梯度 · Empire

```
民用 Leather Harness                              h=5-10  b=5-8   l=3-5  a=5-10   wt=6-8
──────────── 跨档断裂 ────────────
Cataphract Half Coverage (Chainmail)              h=90    b=50    l=5    a=60     wt=17
Cataphract Full Coverage (Chainmail) ⭐            h=90    b=50    l=50   a=60     wt=30
```

### HorseHarness 家族关键约束

- **无中间档**：vanilla+RBM 帝国 HorseHarness 从民用直接跳到顶档 Cataphract——中间无中档（OSA 侧填补 4 层中间过渡）
- **顶点硬约束**：h ≤ 90 · b ≤ 50 · l ≤ 50 · a ≤ 60 · wt ≤ 30
- **arm > body 反常合理**：马颈+前胸+前腿表面积其实比躯干还大，护甲值 arm 60 > body 50 合理
- **Half vs Full 差异**：在 **leg** 字段（Half=5，Full=50）——"半覆盖"= 只覆盖马前半段（头/胸/前腿），不含后腿臀部
- **head 主导**：顶档 head=90（第一大字段）> arm=60 > body=50 = leg=50 · 反映 chamfron（面甲）是马甲最重要部分
- **wt 分档**：民用 6-8 · Half 覆盖 17 · Full 覆盖 30 · OSA wt 不应超 30
- **wt-body 比**：0.6-1.7 · OSA 应遵守避免"马甲比马重"荒谬
- **material_type**：仅两档 · **Leather**（民用）· **Chainmail**（Cataphract 顶）· Plate/Lamellar 全部划归 Chainmail mat
- **命名子结构**：
  - **Harness**（马鞍/马饰）= 极轻档 · Leather mat
  - **Half Barding**（半覆盖甲）= leg 特别低（5）· 其余接近顶
  - **Full Barding / Heavy Barding**（全覆盖甲）= 顶档全字段 · leg 50
- **⚠ OSA v1 系统性错误**：**只用 body 字段**、head/arm/leg 全 0 · v1 body 最高 75 超 vanilla 50 表面 50%，但实际总护甲量（h+b+l+a）远低于 vanilla 顶（250）· v2 需**补齐 4 字段**且各字段严格 ≤ vanilla 顶点

### 🔍 引擎机制核实（2026-09-23 dnSpy 反编译）

**核心发现**：**vanilla native 引擎 + RBM 补丁对马的 armor 计算硬 code 只用 body_armor 字段**——head/arm/leg 三字段引擎完全忽略。

**证据链**：
1. `SandBox.GameComponents.SandboxAgentStatCalculateModel.UpdateHorseStats`（L1041-1057）——马只累加 `spawnEquipment[i].GetModifiedMountBodyArmor()` → `agentDrivenProperties.ArmorTorso`（一字段）
2. `TaleWorlds.MountAndBlade.Agent.GetBaseArmorEffectivenessForBodyPart`（L3385）——对非人类硬 code `return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorTorso)`（无论哪个部位）
3. RBM `ArmorRework.ApplyDrivenArmorBonus`（L679-736）——`if (!agent.IsHuman) { driven = props.ArmorTorso; vanillaBase += GetModifiedMountBodyArmor(); }` 明确按 IsHuman 分支，非人类只用 body_armor

**RBM 为什么还在 XML 加 head/arm/leg？**（推测，无源码注释）：
- XML 结构一致性（与 human armor 保持字段 pattern 统一）
- 未来兼容性（预留字段，等 TW 升级引擎支持马部位差异化）
- 文档表意（告诉 modder Cataphract 马甲设计上覆盖头/颈/胸/腿的意图）
- 可能被 UI mod 消费（PlayerArmorStatus 等 armor status mod）

**对 OSA balance 的影响**：
- 补齐全 4 字段符合"数值与 RBM 参照相近"的 user policy · RBM 风格一致
- **真正生效的仅 body_armor**——真正决定马甲防护力的是这一字段
- 保留全 4 字段无害，且未来引擎若真支持马部位差异化，OSA 已经 ready

**用户 quote 记录**："RBM模组修改后的马甲同时有头甲、马甲、臂甲和腿甲，但我却没有在你的马甲平衡中看到四个数据" → 引发本节反编译核实

---

# Vlandia 文化

## Vlandia · HeadArmor · 70 件（2026-09-23 归档）

**关键设计观察**：Vlandia 头盔梯度**极其丰富**——70 件覆盖民用 6-11 → Padded/Cloth 16-22 → Cervelliere 48-53 → Nasal Helmet 56-95 → Peaked/Kettle 65-105 → Full Helm 108-140 顶点。**Vlandia 特色 Full Helm 全罩式** body_armor 顶 116（覆盖颊+颈甲），远超 Empire aventail 系（body 30-40）。

### 🧣 Cloth Headwrap/Scarf 民用（7 件 · h 6-11）

| id | mat | wt | h/b/a | 名称 |
|---|---|---:|---|---|
| `womens_headwrap_c` | Cloth | 0.1 | 6/0/0 | Rural Head Wrap |
| `head_wrapped` | Cloth | 0.3 | 9/0/0 | Female Rural Headwrap |
| `head_wrapping` | Cloth | 0.05 | 9/0/0 | Peasant Head Wrapping |
| `headscarf_c` | Cloth | 0.3 | 9/0/0 | Luxurious Head Scarf |
| `head_piece` | Cloth | 0.2 | 10/0/0 | Female Rural Scarf |
| `headscarf` | Cloth | 0.5 | 10/0/0 | Loose Head Scarf |
| `cloth_headwrap` | Cloth | 0.4 | 11/7/11 | Cloth Headwrap |

### 🧢 Padded Cap/Coif（6 件 · h 16-22 · 底层软垫头饰）

| id | mat | wt | h/b/a | 名称 |
|---|---|---:|---|---|
| `padded_cap` | Cloth | 0.2 | 16/0/0 | Padded Cap |
| `padded_coif` | Cloth | 0.5 | 16/5/0 | Padded Half Coif |
| `arming_cap` | Cloth | 0.2 | 17/7/17 | Padded Coif |
| `laced_coif` | Cloth | 0.4 | 19/0/19 | Laced Coif |
| `open_padded_coif` | Cloth | 0.3 | 21/0/0 | Open Padded Coif |
| `arming_coif` | Cloth | 0.3 | 22/0/0 | Leather Coif |

### 🎩 Padded Leather Cape（1 件 · h 31）

| id | mat | wt | h/b/a | 名称 |
|---|---|---:|---|---|
| `padded_leather_cape` | Leather | 1.0 | 31/0/0 | Padded Leather Cape |

### ⛑ Cervelliere 家族（4 件 · h 48-53 · 板甲小盔）

| id | wt | h/b/a | 名称 · aventail 类型 |
|---|---:|---|---|
| `cervelliere_over_laced_coif` | 1.1 | 48/0/15 | Cervelliere · Laced Coif |
| `cervelliere_over_cloth_headwrap` | 1.0 | 50/6/10 | Cervelliere · Cloth Headwrap |
| `cervelliere_over_padded_cap` | 1.0 | 51/0/0 | Cervelliere · Padded Cap |
| `cervelliere_over_arming_coif` | 0.9 | 53/0/0 | Cervelliere · Arming Coif |

### ⛑ Segmented Cervelliere 家族（4 件 · h 53-86 · 分片小盔 + aventail 等级）

| id | wt | h/b/a | 名称 · aventail |
|---|---:|---|---|
| `segmented_cervelliere_over_laced_coif` | 1.3 | 53/0/12 | Segmented · Laced Coif |
| `segmented_cervelliere_over_padded_cloth` | 1.2 | 56/12/7 | Segmented · Padded Cloth |
| `segmented_cervelliere_over_mail` | 3.3 | 84/0/20 | Segmented · Mail |
| `segmented_cervelliere_over_mail_coif` | 3.2 | 86/12/40 | Segmented · Mail Coif |

### ⛑ Nasal Cervelliere（4 件 · h 54-81 · 带鼻梁 Cervelliere）

| id | wt | h/b/a | 名称 · aventail |
|---|---:|---|---|
| `nasal_cervelliere_over_padded_cap` | 1.0 | 54/12/0 | Nasal Cerv · Padded Cap |
| `nasal_cervelliere_over_laced_coif` | 1.1 | 59/12/13 | Nasal Cerv · Laced Coif |
| `nasal_cervelliere_over_padded_coif` | 1.2 | 61/15/25 | Nasal Cerv · Padded Coif |
| `nasal_cervelliere_over_mail_coif` | 2.6 | 81/24/40 | Nasal Cerv · Mail Coif |

### ⛑ Nasal Helmet 家族（6 件 · h 56-89 · 鼻梁盔 + aventail 6 档）

| id | wt | h/b/a | 名称 · aventail |
|---|---:|---|---|
| `nasal_helmet_over_cloth_headwrap` | 1.1 | 56/16/10 | Nasal · Cloth Headwrap |
| `nasal_helmet_over_padded_cloth` | 1.2 | 57/12/8 | Nasal · Padded Cloth |
| `nasal_helmet_over_padded_coif` | 1.4 | 58/15/25 | Nasal · Padded Coif |
| `nasal_helmet_over_laced_coif` | 1.3 | 60/12/18 | Nasal · Laced Coif |
| `nasal_helmet_over_mail` | 3.1 | 86/12/20 | Nasal · Mail |
| `nasal_helmet_over_mail_coif` | 2.7 | 89/24/40 | Nasal · Mail Coif |

### ⛑ Peaked Helmet 家族（4 件 · h 65-95 · 尖顶盔）

| id | wt | h/b/a | 名称 · aventail |
|---|---:|---|---|
| `peaked_helmet_over_laced_coif` | 1.1 | 65/0/12 | Peaked · Laced Coif |
| `peaked_helmet_over_padded_cloth` | 1.0 | 68/0/12 | Peaked · Padded Cloth |
| `peaked_helmet_over_mail` | 2.6 | 92/0/20 | Peaked · Mail |
| `peaked_helmet_over_mail_coif` | 2.7 | 95/12/40 | Peaked · Mail Coif |

### ⛑ Segmented Skullcap 家族（4 件 · h 73-92 · 分片头盔 + aventail 4 档）

| id | wt | h/b/a | 名称 · aventail |
|---|---:|---|---|
| `segmented_skullcap_over_laced_coif` | 1.2 | 73/12/20 | Segmented Skull · Laced Coif |
| `segmented_skullcap_over_padded_coif` | 1.3 | 75/12/20 | Segmented Skull · Padded Coif |
| `segmented_skullcap_over_padded_cloth` | 1.1 | 77/8/10 | Segmented Skull · Padded Cloth |
| `segmented_skullcap_over_mail_coif` | 3.5 | 92/25/40 | Segmented Skull · Mail Coif |

### 🪖 Kettle Helmet/Hat 家族（10 件 · h 77-107 · Vlandia 标志性水壶盔）

| id | wt | h/b/a | 名称 · aventail |
|---|---:|---|---|
| `kettle_helmet_over_padded_cap` | 1.2 | 77/0/0 | Kettle · Padded Cap |
| `kettle_helmet_over_laced_coif` | 1.2 | 80/0/20 | Kettle · Laced Coif |
| `kettle_helmet_over_padded_cloth` | 1.1 | 83/0/10 | Kettle · Padded Cloth |
| `kettle_hat_over_padded_coif` | 1.4 | 84/5/22 | Kettle Hat · Padded Coif |
| `kettle_hat_over_padded_cloth` | 1.2 | 85/0/0 | Kettle Hat · Padded Cloth |
| `kettle_helmet_over_padded_coif` | 1.3 | 85/0/25 | Kettle · Padded Coif |
| `kettle_helmet_over_arming_coif` | 1.0 | 87/0/0 | Kettle · Arming Coif |
| `kettle_helmet_with_leather` | 1.0 | 88/0/0 | Kettle with Leather |
| `kettle_helmet_with_mail` | 2.7 | 92/5/33 | Kettle with Mail |
| `kettle_hat_over_mail_coif` | 2.9 | 105/0/40 | Kettle Hat · Mail Coif |
| `kettle_helmet_over_mail` | 3.1 | 107/12/20 | Kettle · Mail |

### 🪖 Guards Kettle（3 件 · h 83-109 · 护卫水壶盔）

| id | wt | h/b/a | 名称 · aventail |
|---|---:|---|---|
| `guards_kettle_over_laced_coif` | 1.4 | 83/0/20 | Guards Kettle · Laced Coif |
| `guards_kettle_over_padded_coif` | 1.5 | 89/5/25 | Guards Kettle · Padded Coif |
| `guards_kettle_over_mail_coif` | 3.2 | 109/12/40 | Guards Kettle · Mail Coif |

### 🎭 Visored Helmet（4 件 · h 83-109 · 面甲盔 · body 顶 75）

| id | wt | h/b/a | 名称 · aventail |
|---|---:|---|---|
| `visored_helmet_over_padded_cloth` | 2.3 | 83/**75**/12 | Visored · Padded Cloth |
| `visored_helmet_over_padded_coif` | 2.5 | 89/**75**/25 | Visored · Padded Coif |
| `visored_helmet_over_laced_coif` | 2.4 | 90/**75**/20 | Visored · Laced Coif |
| `visored_helmet_over_mail_coif` | 3.6 | 109/**75**/40 | Visored · Mail Coif |

### 🎭 Faceguard Knight Helmet + Lord Helmet（3 件 · h 90 · Vlandia Knight 顶档 · body 70-90）

| id | wt | h/b/a | 名称 |
|---|---:|---|---|
| `vlandia_lord_helmet_b2` | 4.1 | 90/0/0 | Plated Helmet (Lord) |
| `vlandian_faceguard_helmet_a` | 2.2 | 90/**90**/0 | Knight Helmet with Steel Faceguard |
| `vlandian_faceguard_helmet_b` | 2.3 | 90/**70**/0 | Knight Helmet with Bronze Faceguard |

### 👑 Western Plated/Crowned Helmet（3 件 · h 98-105）

| id | wt | h/b/a | 名称 |
|---|---:|---|---|
| `western_plated_helmet` | 4.1 | 98/15/0 | Plated Helmet with Sideguards |
| `western_crowned_helmet` | 3.7 | 100/10/0 | Crowned Helmet |
| `western_crowned_plated_helmet` | 3.9 | 105/15/20 | Crowned Plate Helmet |

### 🎭 Full Helm 家族（7 件 · h 108-140 · **Vlandia 顶点** · body 116 全罩）

| id | wt | h/b/a | 名称 · aventail |
|---|---:|---|---|
| `full_helm_over_padded_cap` | 3.1 | 108/**116**/0 | Full Helm · Padded Cap |
| `full_helm_over_arming_coif` | 3.0 | 109/**116**/0 | Full Helm · Arming Coif |
| `full_helm_over_laced_coif` | 3.5 | 111/**113**/20 | Full Helm · Laced Coif |
| `full_helm_over_cloth_headwrap` | 3.2 | 113/**116**/12 | Full Helm · Cloth Headwrap |
| `full_helm_over_padded_cloth` | 3.1 | 116/**116**/12 | Full Helm · Padded Cloth |
| `full_helm_over_mail_coif` | 4.5 | **140/116/40** ⭐ | Full Helm · Mail Coif ⭐ Vlandia 顶点 |

### Vlandia HeadArmor 家族梯度

```
Cloth Headwrap/Scarf 民用                             h 6-11
Padded Cap/Coif/Arming Coif 底层软垫                   h 16-22
Padded Leather Cape                                   h 31
Cervelliere 板甲小盔                                   h 48-53
Segmented Cervelliere / Nasal Cervelliere              h 53-86
Nasal Helmet 系                                       h 56-89
Peaked Helmet 尖顶盔                                   h 65-95
Segmented Skullcap 分片头盔                            h 73-92
Kettle Helmet / Kettle Hat / Guards Kettle              h 77-109
Visored Helmet 面甲盔（body 75 特色）                  h 83-109
Vlandian Faceguard Knight Helmet（body 70-90）        h 90
Plated Helmet / Crowned Helmet                        h 98-105
Full Helm 全罩式（body 116 顶）                        h 108-140 ⭐ 顶点
```

### Vlandia HeadArmor 家族关键约束

- **顶点**：`full_helm_over_mail_coif` **h=140 / b=116 / a=40 / wt=4.5** ⭐
- **body 顶 116**（Full Helm 全罩式 · 覆盖颊甲+颈甲，非 aventail）· vanilla Full Helm 5 件 body=113-116
- **arm 顶 40**（Mail Coif aventail · 各基型 + Mail Coif 都是 arm 40）· Kettle/Guards Kettle/Visored/Full Helm 顶档均达 40
- **aventail 6 档**（从下到上）：无 → Cloth Headwrap / Padded Cap → Arming Coif → Padded Cloth → Padded Coif / Laced Coif → Mail → Mail Coif
- **子结构分档**（同 base_type + aventail 差异）：
  - 无 aventail → 只 head 值
  - Cloth/Padded Cap → head + 少量 body
  - Laced/Padded Coif → head + body 12 + arm 12-25
  - Mail → head + arm 20 (body 变)
  - Mail Coif → head + body 12-25 + arm 40 (顶档 aventail)
- **Vlandia 特色 base_type**：
  - **Full Helm**（全罩式）：body 116 是 Vlandia 独有的"整个颊+颈甲"设计
  - **Kettle Helmet/Hat**：西方水壶盔（宽檐防日+防箭），Vlandia 标志性
  - **Visored Helmet**：面甲盔（可开合），body 75 反映面甲结构
  - **Cervelliere / Segmented Cervelliere / Nasal Cervelliere**：Vlandia 三段板甲小盔梯度
  - **Peaked Helmet**：尖顶盔（骑士早期）
  - **Segmented Skullcap**：分片头盔（Vlandia + Sturgia 共有）
- **无 Cataphract 类**：Vlandia 是 Knight 板甲骑士，非 Cataphract 复合甲——命名"Cataphract"应视为 OSA 借用 Empire 概念的翻译，实际 vanilla Vlandia 无 Cataphract 头盔

---

## Vlandia · Cape · 10 件（2026-09-23 归档）

**关键设计观察**：vanilla+RBM Vlandia Cape 10 件 arm_armor 全 0（RBM 设计意图同 Empire · Cape 是纯 body_armor / mantle · 无肩甲延伸）。**Vlandia 顶点 `noble_pauldron_with_scarf` 88** —— **比 Empire 顶 55 高 1.6×**，反映 Vlandia Knight 板甲传统。

### 🧣 Cloth Hood 民用（2 件 · body 7-9）

| id | mat | wt | body | 名称 |
|---|---|---:|---:|---|
| `green_hood` | Cloth | 0.5 | 7 | Green Hood |
| `hood` | Cloth | 0.4 | 9 | Hood |

### 🔗 Chainmail Shoulders（1 件 · body 20）

| id | mat | wt | body | 名称 |
|---|---|---:|---:|---|
| `chainmail_shoulder_armor` | Chainmail | 1.4 | 20 | Reinforced Mail Shoulders |

### 🦌 Padded Leather Shoulders（1 件 · body 22）

| id | mat | wt | body | 名称 |
|---|---|---:|---:|---|
| `padded_leather_shoulders` | Cloth | 1.4 | 22 | Padded Leather Shoulders |

### 🛡 Plate Pauldrons 家族（6 件 · body 25-88 · 板甲护肩递进）

| id | wt | body | 名称 |
|---|---:|---:|---|
| `scale_shoulder_armor` | 1.7 | 25 | Scale Shoulderguards |
| `pauldron_with_cape` | 1.3 | 33 | Pauldrons with Cape |
| `pauldron_over_scale_armor` | 3.4 | 50 | Reinforced Ornate Pauldrons over Scale |
| `noble_pauldron` | 1.2 | 60 | Ornate Pauldrons |
| `noble_pauldron_with_cape` | 1.8 | 68 | Ornate Pauldrons with Cape |
| `noble_pauldron_with_scarf` | 3.2 | **88** | Reinforced Ornate Pauldrons ⭐ 顶点 |

### Vlandia Cape 家族梯度

```
Cloth Hood                                            body 7-9
Chainmail Reinforced Mail Shoulders                   body 20
Padded Leather Shoulders                              body 22
Plate Scale Shoulderguards                            body 25
Plate Pauldrons with Cape (Standard)                  body 33
Plate Pauldrons over Scale (Reinforced Ornate)        body 50
Plate Ornate Pauldrons (Noble)                        body 60
Plate Ornate Pauldrons with Cape (Noble)              body 68
Plate Reinforced Ornate Pauldrons (顶) ⭐              body 88
```

### Vlandia Cape 家族关键约束

- **vanilla 顶点**：`noble_pauldron_with_scarf` **88/0/3.2**（Reinforced Ornate Pauldrons · Vlandia Knight 顶档）
- **OSA Vlandia Cape 需在此顶点内**：raw body ≤ 88（比 Empire 顶 55 高，反映 Vlandia Plate 传统）
- **Vlandia 顶 88 vs Empire 顶 55 差异**：Empire Lamellar 复合结构 vs Vlandia Reinforced Plate · Vlandia Plate 材质更重更硬
- **命名子结构分档**（同 Empire 三部分律）：
  - Rule 1A（有 shoulder/pauldron 命名）：允许 body + arm，body > arm
  - Rule 1B（无 shoulder/pauldron 命名，如 Cape/Cloak/Hood）：arm 必须 = 0
  - Rule 2 arm mesh-tiered 分档律：Elite Heavy 25 · Standard Shoulders 20 · Standard Pauldrons 12 · Chainmail 10-12 · Leather 4-8
  - Rule 3 视觉判断优先律：命名允许 ≠ 数值强制
- **arm 上限**：Vlandia OSA arm ≤ 25（Elite Heavy · 同 Empire）

---

## Vlandia · BodyArmor · 29 件（2026-09-23 归档）

**关键设计观察**：Vlandia BodyArmor 顶 100（`sturgian_fortified_armor` Brigandine over Hauberk）· **比 Empire 135 低 26%**（Empire Cataphract 复合甲更重）· 但 Vlandia arm 顶 100 > Empire 67——Vlandia Knight 强调**臂部覆盖**（Full Sleeve Hauberk 传统），Empire 强调 body 覆盖。

### 👕 Cloth 民用（8 件 · body 6-8）

| id | wt | b/l/a | 名称 |
|---|---:|---|---|
| `cloth_tunic` | 0.4 | 6/5/5 | Simple Cloth Tunic |
| `long_woolen_tunic` | 0.4 | 6/5/5 | Long Woolen Tunic |
| `monk_robe` | 0.5 | 6/6/5 | Scholar Robe |
| `sackcloth_tunic` | 0.6 | 6/5/5 | Sackcloth Tunic |
| `vlandian_corset_dress` | 0.5 | 6/6/5 | Corseted Dress |
| `vlandian_noble_woman_dress` | 0.6 | 6/5/5 | Silken Noble Dress |
| `vlandian_woman_dress` | 0.7 | 6/6/5 | Aproned Dress |
| `long_hemp_tunic` | 0.6 | 8/8/6 | Long Hemp Tunic |

### 🧥 Padded Gambeson / Aketon（4 件 · body 16-19）

| id | mat | wt | b/l/a | 名称 |
|---|---|---:|---|---|
| `padded_leather_shirt` | Leather | 1.8 | 16/16/11 | Padded Leather Shirt |
| `gambeson_b` | Cloth | 2.0 | 18/18/18 | Aketon |
| `leather_coat_over_cloth` | Cloth | 0.7 | 18/18/7 | Leather Coat Over Cloth |
| `aketon` | Cloth | 2.4 | 19/19/19 | Heavy Aketon |

### 🦌 Leather Vest/Coat（4 件 · body 20-24）

| id | mat | wt | b/l/a | 名称 |
|---|---|---:|---|---|
| `leather_coat` | Leather | 2.3 | 20/20/15 | Leather Tunic |
| `woven_leather_vest` | Leather | 4.7 | 22/18/12 | Woven Leather Vest |
| `padded_short_coat` | Cloth | 2.2 | 23/19/18 | Padded Short Coat |
| `sleeveless_padded_coat` | Cloth | 2.2 | 23/25/10 | Sleeveless Padded Coat |
| `sleeveless_padded_short_coat` | Cloth | 2.0 | 23/20/10 | Sleeveless Padded Short Coat |
| `leather_scale_armor` | Leather | 8.6 | 24/22/18 | Leather Scale Armor |
| `padded_coat` | Cloth | 2.4 | 28/32/26 | Padded Footman Coat |

### 🔗 Chainmail Mail Shirt/Hauberk（5 件 · body 37-45）

| id | wt | b/l/a | 名称 |
|---|---:|---|---|
| `veteran_mercenary_armor` | 8.2 | 37/33/13 | Fortified Mercenary Armor |
| `vlandia_chainmail` | 10 | 40/44/40 | Heavy Mail Hauberk |
| `mail_shirt` | 7.5 | 41/33/36 | Mail Shirt |
| `red_coat_over_mail` | 8.6 | 45/38/42 | Red Tabard over Mail Hauberk |
| `white_coat_over_mail` | 9.6 | 45/38/42 | White Tabard over Mail Hauberk |

### 🔗 Chainmail 高档（2 件 · body 52-77）

| id | wt | b/l/a | 名称 |
|---|---:|---|---|
| `banded_leather_over_mail` | 8.6 | 52/27/27 | Banded Leather Over Mail |
| `hauberk` | 12.3 | 77/44/44 | Double Mail Hauberk |

### 🛡 Plate 顶档（3 件 · body 75-100）

| id | wt | b/l/a | 名称 |
|---|---:|---|---|
| `plated_leather_coat` | 12.7 | 75/56/60 | Rough Brigandine |
| `coat_of_plates_over_mail` | 20.11 | 82/38/49 | Brigandine over Mail |
| `sturgian_fortified_armor` | 26 | **100/95/100** ⭐ | Brigandine over Hauberk ⭐ 顶点 |

### Vlandia BodyArmor 家族梯度

```
Cloth 民用 (tunic/dress/robe)                          body 6-8
Padded Leather Shirt / Aketon (Padded/Cloth)           body 16-19
Leather Coat/Vest / Padded Short Coat                  body 20-24
Padded Footman Coat (Cloth 顶)                          body 28
Chainmail Mercenary Armor                              body 37
Chainmail Mail Shirt/Hauberk                           body 40-45
Chainmail Banded Leather Over Mail                     body 52
Chainmail Double Mail Hauberk                          body 77
Plate Rough Brigandine                                 body 75
Plate Brigandine over Mail                             body 82
Plate Brigandine over Hauberk (顶) ⭐                  body 100
```

### Vlandia BodyArmor 家族关键约束

- **顶点**：`sturgian_fortified_armor` **100/95/100/26**（Brigandine over Hauberk）· body ≤ 100 · arm ≤ 100
- **Vlandia vs Empire 差异**：
  - Vlandia 顶 100 < Empire 顶 135（Vlandia Knight 板甲更轻）
  - Vlandia arm 顶 100 > Empire arm 67（Vlandia Full Sleeve Hauberk 传统 · 全臂锁子甲）
  - Vlandia leg 顶 95 ≈ Empire leg 122（Vlandia 无 Scale Skirt 强化）
- **序**：`body_armor ≥ leg_armor > arm_armor`（vanilla 全 29 件遵守，唯 `sturgian_fortified_armor` arm=100=body 齐）
- **材质分档硬约束**：Cloth ≤ 28（Padded Footman Coat 顶）· Leather ≤ 24 · Chainmail 37-77 · Plate 75-100
- **结构描述子分档**：Tunic 民用 · Padded Gambeson/Aketon 中档 · Coat Over Mail 中高档 · Brigandine 顶档
- **Vlandia 特色 base_type**：Aketon 命名（Cloth Gambeson 变体）· Tabard Over Mail Hauberk（骑士外袍）· Brigandine（板衣 · Plate 顶档）

---

## Vlandia · HandArmor · 5 件（2026-09-23 归档）

**关键设计观察**：Vlandia HandArmor 顶 `lordly_mail_mitten` **60** · 比 Empire 顶 63 略低 · 全为 Mail/Plate 系（无 Cloth Padded Mitten）

| id | mat | wt | arm | 名称 |
|---|---|---:|---:|---|
| `khuzait_heavy_armor_bracer` | Plate | 2.2 | 38 | Khan's Plated Bracers |
| `reinforced_leather_vambraces` | Plate | 1.0 | 40 | Splint Vambraces |
| `mail_mitten` | Chainmail | 1.4 | 45 | Mail Mittens |
| `reinforced_mail_mitten` | Chainmail | 1.6 | 48 | Reinforced Mail Mittens |
| `lordly_mail_mitten` | Chainmail | 1.7 | **60** | Heavy Mail Mittens ⭐ 顶点 |

### Vlandia HandArmor 关键约束

- **顶点**：`lordly_mail_mitten` 60/1.7 · arm ≤ 60（比 Empire 63 略低）
- **命名子结构**：Bracers/Vambraces（前臂）< Mittens（Mail 全手）
- **Vlandia 特色**：全线 Mail Mittens 系为主（无 Padded Mitten）· 反映 Vlandia Knight Mail 传统

---

## Vlandia · LegArmor · 2 件（2026-09-23 归档）

**关键设计观察**：Vlandia LegArmor 极简 · 顶 `mail_cavalier_boots` **43** · 无 Plate Boots 顶档（Empire Lamellar Plate Boots 62）

| id | mat | wt | leg | 名称 |
|---|---|---:|---:|---|
| `leather_cavalier_boots` | Leather | 0.9 | 32 | Leather Cavalier Boots |
| `mail_cavalier_boots` | Chainmail | 1.8 | **43** | Mail Cavalier Boots ⭐ 顶点 |

### Vlandia LegArmor 关键约束

- **顶点**：`mail_cavalier_boots` 43/1.8 · leg ≤ 43（比 Empire 62 低 30%）
- **Vlandia 特色**：Mail Boots + Leather Boots 二档 · 无 Plate 顶档

---

## Vlandia · HorseHarness · 2 件（2026-09-23 归档 · 全 4 字段）

**关键设计观察**：Vlandia HorseHarness 极简 · 只 2 件 · 全 Chainmail 材质 · **顶点 `chain_barding` h=90 b=40 l=40 a=50** · 比 Empire `imperial_scale_barding` 90/50/50/60 稍弱（body -10, arm -10）

**⚠ 引擎机制回忆**：HorseHarness 引擎硬 code 只用 body_armor · head/arm/leg 三字段是 RBM 装饰性填充（见 Empire HorseHarness 引擎机制核实）

| id | wt | h/b/l/a | 名称 |
|---|---:|---|---|
| `halfchain_barding` | 16 | **60/40/25/50** | Half Mail Barding |
| `chain_barding` | 26 | **90/40/40/50** ⭐ | Reinforced Chainmail Barding ⭐ 顶点 |

### Vlandia HorseHarness 关键约束

- **顶点**：`chain_barding` **h=90/b=40/l=40/a=50/wt=26** · 全字段 ≤ Empire 顶点
- **无民用 Harness**（Vlandia 无 stripped_leather_harness / imperial_riding_harness 等值）· 只有 Barding 中高档
- **Half vs Full 差异**：Half `halfchain_barding` **l=25** vs Full `chain_barding` **l=40** · Vlandia 半覆盖 leg 25（比 Empire Half 5 高很多）
- **Vlandia 特色**：全 Chainmail 材质 · 无 Plate/Scale 顶档 · 反映 Vlandia Knight Mail 传统

---

# Battania 文化

## Battania · HeadArmor · 27 件（2026-09-23 归档）

**关键设计观察**：Battania 顶点 **121**（`battanian_crowned_helmet`）· 低于 Empire/Vlandia 但仍高档 · 无 Full Helm/Cataphract 顶级 aventail · 主流是 **Cheek Guards Cap**（护颊帽 · Battania 标志）+ Ridge Helmet + Bear/Wolf Head 部落装饰

### 🧣 Cloth 民用（2 件 · h 9-11）

| id | mat | wt | h | 名称 |
|---|---|---:|---:|---|
| `wrapped_headcloth` | Cloth | 0.2 | 9 | Wrapped Headcloth |
| `battania_civil_hood` | Cloth | 0.5 | 11 | Cloth Hood |

### 👑 Highland Ceremonial Crown（1 件 · h 15）

| id | mat | wt | h | 名称 |
|---|---|---:|---:|---|
| `battania_crown` | Plate | 0.5 | 15 | Highland Ceremonial Crown |

### 🦌 Leather Coif / Studded Helm（5 件 · h 18-27）

| id | mat | wt | h | 名称 |
|---|---|---:|---:|---|
| `thinhide_coif` | Leather | 0.5 | 18 | Thinhide Coif |
| `leather_studdedhelm` | Leather | 0.5 | 23 | Leather Studded Helm |
| `leather_studdedhelm_over_headcloth` | Leather | 1.1 | 24 | Leather Studded Helm over Headcloth |
| `leather_studdedhelm_over_thinhide` | Leather | 1.2 | 25 | Leather Studded Helm over Thinhide |
| `leather_studdedhelm_over_roughscale` | Leather | 3.6 | 27 | Leather Studded Helm over Roughscale |

### 🐺 Wolf/Bear Head + Fur（3 件 · h 25-34 · 部落装饰）

| id | mat | wt | h | 名称 |
|---|---|---:|---:|---|
| `wolfhead` | Leather | 1.3 | 25 | Wolf Head |
| `battania_fur_helmet` | Leather | 1.3 | 34 | Cap with Fur Coif |
| `bearhead` | Leather | 1.4 | 34 | Bear Head |

### ⛑ Bronze/Steel Cap with Cheek Guards（Battania 标志性 · 8 件 · h 61-89）

| id | wt | h | 名称 |
|---|---:|---:|---|
| `battania_earmuff_helmet_b_brnz` | 3.0 | 61 | Bronze Cap With Cheek Guards |
| `battania_earmuff_helmet_c_cpr` | 3.3 | 63 | Decorated Bronze with Cheek Guards |
| `battania_earmuff_helmet_a_brnz` | 3.2 | 65 | Fringed Bronze Cap With Cheek Guards |
| `battania_earmuff_helmet_d_brnz` | 3.3 | 71 | Ridged Decorated Bronze Cap |
| `battania_earmuff_helmet_c_brnz` | 3.3 | 73 | Fringed Decorated Bronze Cap |
| `battania_earmuff_helmet_b` | 2.9 | 74 | Steel Cap With Cheek Guards |
| `battania_earmuff_helmet_a` | 3.0 | 79 | Fringed Steel Cap |
| `battania_earmuff_helmet_c` | 3.1 | 89 | Fringed Decorated Steel Cap |

### 🏛 Old Guard / Battle Crown（2 件 · h 63-66）

| id | wt | h | 名称 |
|---|---:|---:|---|
| `celtic_helmet` | 2.5 | 63 | Old Guard Helmet |
| `battania_battle_crown` | 2.3 | 66 | Two Pronged Bronze Crown |

### ⛑ Roughscale Helmet（1 件 · h 88 · body 0 · **arm 20** 首个带 arm）

| id | wt | h/b/a | 名称 |
|---|---:|---|---|
| `roughscale_helmet` | 3.2 | 88/0/20 | Roughscale Helmet |

### ⛑ Ridged Decorated Steel Cap / Ridged Helm（2 件 · h 92-94 · body 12）

| id | wt | h/b/a | 名称 |
|---|---:|---|---|
| `battania_earmuff_helmet_d` | 3.2 | 92/12/0 | Ridged Decorated Steel Cap |
| `ridged_northernhelm` | 3.4 | 94/12/0 | Ridged Helm |

### 👑 Warlord Helmet + Highland Crowned Helmet 顶档（3 件 · h 95-121）

| id | wt | h/b/a | 名称 |
|---|---:|---|---|
| `battanian_plated_noble_helmet` | 2.9 | 95/16/15 | Warlord Helmet with Feathers |
| `battanian_noble_helmet_with_feather` | 3.1 | 102/12/25 | Warlord Helmet |
| `battanian_crowned_helmet` | 3.2 | **121/16/15** ⭐ | Highland Crowned Helmet ⭐ 顶点 |

### Battania HeadArmor 家族梯度

```
Cloth Headcloth / Hood                             h 9-11
Highland Ceremonial Crown                          h 15
Thinhide Coif / Leather Studded Helm               h 18-27
Fur Helmet / Wolf/Bear Head 部落装饰                h 25-34
Bronze/Steel Cap with Cheek Guards（标志性）        h 61-89
Old Guard / Battle Crown                           h 63-66
Roughscale Helmet（首个 arm 20）                    h 88
Ridged Decorated / Ridged Helm                     h 92-94
Warlord Helmet / Highland Crowned 顶 ⭐             h 95-121
```

### Battania HeadArmor 关键约束

- **顶点**：`battanian_crowned_helmet` **121/16/15/3.2** ⭐ · h ≤ 121（比 Empire 155+ 低 · 比 Vlandia 140 低）
- **无 Full Helm / Cataphract 顶级 aventail 顶档**——Battania 无金属全罩式 · 顶档是"高冠+羽饰"的 Warlord Helmet
- **body/arm 顶极低**：body 顶 16 · arm 顶 25 · 反映 Battania 无 aventail 传统
- **Battania 特色 base_type**：
  - **Cap with Cheek Guards**（护颊帽）· Battania 标志性主流兵种装备
  - **Ridge Helmet**（凸脊盔）· Highland 装饰性
  - **Wolf/Bear Head + Fur Coif**（部落图腾装饰）
  - **Warlord Helmet with Feathers**（羽饰战王盔 · 顶档）
  - **Highland Crowned Helmet**（王冠盔 · 绝对顶点）
- **材质分档**：Cloth ≤ 11 · Leather ≤ 34（部落装饰含 Fur/Bear/Wolf）· Plate 15-121（跨极大范围 · Ceremonial Crown 15 最低 · Highland Crowned 121 最高）

---

## Battania · Cape · 22 件（2026-09-23 归档）

**关键设计观察**：Battania Cape 顶 **45** · **比 Empire 55/Vlandia 88 都低**——Battania Cape 主流是 Leather/Fur/Bear/Wolf 部落装饰，非重装 Plate 板甲 · **arm 全 0**（vanilla 全 22 件同 Empire/Vlandia）

### 🧣 Cloth 民用（4 件 · body 6-7）

| id | mat | wt | body | 名称 |
|---|---|---:|---:|---|
| `a_battania_cloak_a` | Cloth | 0.5 | 6 | Simple Cape |
| `battania_civil_cloak` | Cloth | 1.0 | 6 | Fringed Cape |
| `battania_cloak` | Cloth | 0.5 | 6 | Tartan Cloak |
| `battania_civil_cape` | Cloth | 1.5 | 7 | Woodland Fur Cloak |

### 🦌 Leather Cape / Shoulder（8 件 · body 7-16）

| id | mat | wt | body | 名称 |
|---|---|---:|---:|---|
| `fur_cloak_b` | Leather | 2.7 | 7 | Fur Cloak |
| `battanian_leather_shoulder_a` | Leather | 1.2 | 12 | Cured Leather Shoulder Pieces |
| `battania_scarf_a` | Cloth | 0.55 | 13 | Heavy Tartan Scarf |
| `battania_cloak_b` | Cloth | 3.2 | 14 | Tartan Cape |
| `battania_shoulder_strap` | Leather | 1.0 | 14 | Shoulder Harness |
| `battania_shoulder_furr` | Leather | 1.6 | 15 | Reinforced Harness with Cape |
| `battania_woodland_cloak` | Leather | 1.4 | 15 | Woodland Cape |
| `battania_shoulder_strap_cloak` | Leather | 1.3 | 16 | Tartan Shoulder Harness |

### 🐺 Fur/Wolf/Bear Cape 部落（6 件 · body 20-26）

| id | mat | wt | body | 名称 |
|---|---|---:|---:|---|
| `fur_cloak_a` | Leather | 7.6 | 20 | Fur Cloak With Round Plate |
| `battania_cloak_furr` | Leather | 1.5 | 22 | Chained Fur Cloak |
| `wolf_shoulder` | Leather | 1.8 | 22 | Wolf Pelt Cape |
| `armored_bearskin` | Leather | 7.0 | 24 | Armored Bearskin |
| `fur_cloak_c` | Leather | 8.3 | 25 | Large Fur Cloak With Round Plate |
| `bearskin` | Leather | 2.3 | 26 | Bear Pelt |
| `rough_bearskin` | Leather | 2.3 | 26 | Rough Bearskin |

### 🔗 Chainmail Mail Shoulders（2 件 · body 27-32）

| id | mat | wt | body | 名称 |
|---|---|---:|---:|---|
| `battanian_chainmail_shoulder_a` | Chainmail | 4.5 | 27 | Mail Shoulder Pieces |
| `battanian_chainmail_shoulder_b` | Chainmail | 4.5 | 32 | Mail Shoulder Reinforcements |

### 🛡 Plate Warlord Pauldrons（1 件 · body 45 ⭐）

| id | mat | wt | body | 名称 |
|---|---|---:|---:|---|
| `battania_warlord_pauldrons` | Plate | 4.0 | **45** ⭐ | Heavy Warlord Pauldrons ⭐ 顶点 |

### Battania Cape 家族关键约束

- **顶点**：`battania_warlord_pauldrons` **45/0/4.0** ⭐（Heavy Warlord Pauldrons · 唯一 Plate Cape）
- **Battania Cape 顶 45**（对比 Empire 55 / Vlandia 88）——**Battania 无重装 Plate Cape 传统**
- **主流 Leather/Fur 部落装饰**：Wolf Pelt Cape / Bear Pelt / Chained Fur Cloak 主导 body 22-26 范围
- **Chainmail Shoulder 有限**：仅 2 件（27, 32）· Chainmail 材质稀少
- **三部分律沿用**：命名二分律 + arm mesh-tiered + 视觉判断律 · Battania OSA arm ≤ 20（Warlord Pauldrons Elite 档 · sub Empire 25）

---

## Battania · BodyArmor · 38 件（2026-09-23 归档）

**关键设计观察**：Battania BodyArmor 顶 **96**（`battania_warlord_armor` Scale Warlord · Plate）· 与 Vlandia 100 接近 · **arm 顶 40**（vs Empire 67 / Vlandia 100）· Battania 无 Full Sleeve 传统 · **主流 Chainmail Hauberk**（顶 55-66）· Plate 稀少（仅 5 件 60-96）

### 👕 Cloth 民用（8 件 · body 4-10）

| id | wt | b/l/a | 名称 |
|---|---:|---|---|
| `battanian_savage_armor` | 0.8 | 4/32/2 | Wildling Armor（leg 32 高） |
| `battania_civil_a` | 0.6 | 7/7/5 | Tartan Tunic |
| `burlap_waistcoat` | 0.4 | 7/7/5 | Burlap Waistcoat |
| `battania_dress_a/b/c` `battania_female_civil_a/b` | 0.7-0.9 | 8/8/6 | Various Dresses |
| `battania_light_armor_e` | 0.7 | 10/15/0 | Tartan Toga |

### 🦌 Leather Vest/Coat（8 件 · body 9-20）

| id | wt | b/l/a | 名称 |
|---|---:|---|---|
| `battania_civil_c` | 0.6 | 9/9/7 | Townsman Luxury Coat |
| `sleeveless_studded_fur_armor` | 0.4 | 10/10/0 | Sleeveless Studded Fur Armor |
| `battania_civil_b` | 0.4 | 13/11/7 | Villager Leather Tunic |
| `fur_armor_with_strap` | 1.6 | 14/12/9 | Fur Armor With Strap |
| `long_fur_coat` | 2.6 | 16/16/10 | Long Fur Coat |
| `fur_armor` | 13.6 | 19/19/11 | Fur Armor |
| `battania_woodland_outfit` | 1.2 | 20/22/14 | Woodland Garments |

### 🧥 Chainmail Tartan/Kilt（3 件 · body 15-25 · Chainmail mat 但民用 Kilt/Tartan）

| id | wt | b/l/a | 名称 |
|---|---:|---|---|
| `battania_light_armor_d` | 0.8 | 0/15/0 | Highland Kilt over Tartan Trousers |
| `battania_light_armor_c` | 2.4 | 15/15/0 | Highland Long Tartan |
| `battania_light_armor_b` | 3.2 | 20/17/10 | Highland Tartan |
| `battania_light_armor_a` | 6.5 | 25/17/10 | Highland Tartan with Chest Plate |

### 🔗 Chainmail Mail Shirt/Hauberk（7 件 · body 36-66）

| id | wt | b/l/a | 名称 |
|---|---:|---|---|
| `battanian_chainmail_armor_b` | 10.7 | 36/22/16 | Highland Mail Shirt |
| `battanian_chainmail_armor_a` | 10.7 | 40/25/16 | Heavy Mail Vest |
| `battania_woodland_chainmail` | 10.3 | 45/27/38 | Heavy Mail Shirt with Wide Belt |
| `ranger_mail` | 12 | 47/15/18 | Ranger Mail |
| `battanian_chainmail_armor_c` | 14.7 | 49/22/16 | Highland Reinforced Mail Over Tartan |
| `battanian_chainmail_armor_d` | 16.6 | 51/22/16 | Highland Reinforced Mail |
| `battania_mercenary_armor` | 10.6 | 55/27/33 | Luxury Brigandine |
| `battania_noble_armor` | 13.6 | 66/33/33 | Highborn Mail Armor |

### 🛡 Plate 顶档（5 件 · body 34-96）

| id | wt | b/l/a | 名称 |
|---|---:|---|---|
| `scale_armor` | 10.7 | 34/20/0 | Rugged Scale Armor |
| `kilt_over_plated_leather` | 10.8 | 48/48/12 | Kilt Over Plated Leather |
| `western_scale_mail` | 14 | 50/38/38 | Rough Scale Mail |
| `battanian_scale_armor_b` | 14.5 | 60/30/15 | Highland Scale Armor Over Cloth |
| `battania_brass_plate_armor` | 10 | 70/20/20 | Bronze Breastplate |
| `battanian_scale_armor_a` | 26 | 79/35/25 | Highland Scale Armor over Mail |
| `battania_warlord_armor` | 25 | **96/50/40** ⭐ | Scale Warlord Armor ⭐ 顶点 |

### Battania BodyArmor 关键约束

- **顶点**：`battania_warlord_armor` **96/50/40/25**（Scale Warlord Armor · Plate 顶）· body ≤ 96
- **arm 顶 40**（vs Empire 67 · Vlandia 100）——Battania 无 Full Sleeve 传统
- **主流 Chainmail Hauberk**（顶 55-66 · 高频档）· Plate 稀少（仅 5 件）
- **Battania 特色**：Fur Armor / Woodland Garments 部落装扮 · Kilt over Trousers / Highland Tartan / Wildling Armor（Kilt 高档 leg 32）

---

## Battania · HandArmor · 10 件（2026-09-23 归档）

| id | mat | wt | arm | 名称 |
|---|---|---:|---:|---|
| `rough_tied_bracers` | Cloth | 0.6 | 21 | Rough Tied Bracers |
| `armwraps` | Cloth | 0.2 | 22 | Armwraps |
| `ragged_armwraps` | Cloth | 0.4 | 22 | Ragged Armwraps |
| `buttoned_leather_bracers` | Leather | 0.5 | 24 | Buttoned Leather Bracers |
| `roughtied_leather_bracers` | Leather | 0.5 | 24 | Roughtied Leather Bracers |
| `guarded_armwraps` | Cloth | 0.4 | 25 | Guarded Armwraps |
| `strapped_leather_bracers` | Leather | 0.5 | 25 | Strapped Leather Bracers |
| `highland_gloves` | Leather | 0.8 | 30 | Fur Rimmed Leather Gloves |
| `battania_noble_bracers` | Leather | 0.6 | 39 | Fian Bracers |
| `battania_warlord_bracers` | Plate | 1.5 | **50** ⭐ | Scale Warlord Bracers ⭐ 顶点 |

**Battania HandArmor 特色**：Bracers/Armwraps 部落主流 · Fian Bracers 39（弓手贵族档）· Warlord Bracers 50 顶（vs Empire 63 / Vlandia 60 略低）

---

## Battania · LegArmor · 10 件（2026-09-23 归档）

| id | mat | wt | leg | 名称 |
|---|---|---:|---:|---|
| `turndown_leather_boots` | Leather | 0.9 | 25 | Folded Leather Boots |
| `highland_boots` | Leather | 1.0 | 26 | Forester Boots |
| `battania_woodland_boots` | Leather | 1.0 | 28 | Woodland Boots |
| `belted_leather_boots` | Leather | 1.1 | 29 | Belted Leather Boots |
| `highland_leg_wrappings` | Cloth | 1.0 | 30 | Woodland Leg Wrappings |
| `rough_tied_boots` | Leather | 0.9 | 30 | Rough Tied Boots |
| `battania_leather_boots` | Leather | 1.0 | 32 | Highborn Leather Boots |
| `wrapped_leather_boots` | Leather | 1.0 | 33 | Wrapped Leather Boots |
| `battania_fur_boots` | Leather | 1.5 | 35 | Fur Boots |
| `battania_warlord_boots` | Plate | 2.6 | **46** ⭐ | Scaled Boots ⭐ 顶点 |

**Battania LegArmor 特色**：Leather Boots 主流 · Fur Boots 35 部落顶级 Leather · Warlord Boots 46 Plate 顶（vs Empire 62 / Vlandia 43 · 介于两者）

---

## Battania · HorseHarness · 4 件（2026-09-23 归档 · 全 4 字段）

| id | mat | wt | h/b/l/a | 名称 |
|---|---|---:|---|---|
| `bandit_saddle_highland` | Cloth | 8 | 5/5/0/5 | Rugged Fur Saddle |
| `battania_horse_harness` | Leather | 10.3 | 10/15/10/10 | Battania Horse Harness |
| `battania_horse_harness_halfscaled` | Chainmail | 15 | 50/35/20/45 | Half Scaled Barding |
| `battania_horse_harness_scaled` | Chainmail | 20.6 | **50/35/30/45** ⭐ | Scale Barding ⭐ 顶点 |

**Battania HorseHarness 特色**：极简 4 档 · 顶点 `battania_horse_harness_scaled` h=50/b=35/l=30/a=45 · **比 Empire/Vlandia 顶低**（Empire 90/50/50/60）· Battania 无重装骑兵传统 · **⚠ 引擎只读 body_armor**（回忆 HorseHarness 引擎机制）
