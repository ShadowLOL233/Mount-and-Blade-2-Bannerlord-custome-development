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


<!-- SLOT:HeadArmor COUNT:1328 -->

<!-- SOURCE_DIST -->
| Source | Count |
|---|---:|
| OSA | 843 |
| RBM | 282 |
| Vanilla | 203 |

<!-- TIER_DIST -->
| Tier | Count |
|---|---:|
| T5 | 593 |
| T4 | 179 |
| T3 | 175 |
| T2 | 106 |
| T1 | 157 |
| T0 | 55 |
| T-1 | 63 |

<!-- CULTURE:empire COUNT:245 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `imperial_goggled_helmet` | Goggled Closed Cataphract Helmet | Plate | T5 | auto | 144 | 82 | 45 | 0 | 4.2 | RBM |
| `heavy_nasalhelm_over_imperial_mail` | Heavy Nasalhelm over Mail | Plate | T5 | auto | 124 | 36 | 20 | 0 | 3.6 | RBM |
| `empire_guarded_lord_helmet` | Royal Cataphract Helmet | Plate | T5 | auto | 130 | 30 | 0 | 0 | 3.7 | RBM |
| `heavy_nasalhelm_over_imperial_padding` | Heavy Nasalhelm over Padding | Plate | T5 | auto | 100 | 41 | 25 | 0 | 3.2 | RBM |
| `empire_jewelled_helmet` | Imperial Jeweled Helmet | Plate | T5 | auto | 125 | 14 | 20 | 0 | 3.8 | RBM |
| `leatherlame_feathered_spangenhelm_over_mail` | Feathered Spangenhelm over Mail | Plate | T5 | auto | 97 | 22 | 45 | 0 | 3.4 | RBM |
| `empire_helmet_with_metal_strips` | Lord Helmet with Metal Strips | Plate | T5 | auto | 120 | 14 | 20 | 0 | 3.8 | RBM |
| `heavy_nasalhelm_over_imperial_leather` | Heavy Nasalhelm over Leather | Plate | T5 | auto | 105 | 36 | 12 | 0 | 3 | RBM |
| `ironlame_nasalhelm_over_imperial_coif` | Iron Nasalhelm over Coif | Plate | T5 | auto | 85 | 22 | 45 | 0 | 4.1 | RBM |
| `feathered_spangenhelm_over_imperial_coif` | Feathered Spangenhelm over Coif | Plate | T5 | auto | 88 | 22 | 40 | 0 | 3.1 | RBM |
| `heavy_nasalhelm_over_laced_cloth` | Heavy Nasalhelm over Laced Cloth | Plate | T5 | auto | 97 | 36 | 12 | 0 | 3.1 | RBM |
| `empire_lord_helmet` | Noble Guard Helmet | Plate | T5 | auto | 123 | 10 | 0 | 0 | 3.5 | RBM |
| `imperial_nasal_helm` | Legionary Helm | Plate | T5 | auto | 97 | 12 | 25 | 0 | 2.2 | RBM |
| `ironlame_feathered_spangenhelm_over_mail` | Spangenhelm over Mail | Plate | T5 | auto | 95 | 12 | 25 | 0 | 3.3 | RBM |
| `plumed_helmet` | Plumed Helmet | Plate | T5 | auto | 104 | 24 | 0 | 0 | 2.9 | RBM |
| `ironlame_nasalhelm_over_imperial_mail` | Iron Nasalhelm over Mail | Plate | T5 | auto | 90 | 12 | 25 | 0 | 3.9 | RBM |
| `leatherlame_nasalhelm_over_imperial_mail` | Bronze Nasalhelm over Mail | Plate | T5 | auto | 87 | 12 | 25 | 0 | 3.7 | RBM |
| `empire_battle_crown_north` | Jeweled Plumed Battle Crown | Plate | T5 | auto | 104 | 12 | 0 | 0 | 2.1 | RBM |
| `spiked_kettle_over_imperial_mail` | Spiked Kettle over Mail | Plate | T5 | auto | 90 | 4 | 20 | 0 | 3.6 | RBM |
| `roundkettle_over_imperial_mail` | Roundkettle over Mail | Plate | T5 | auto | 92 | 0 | 20 | 0 | 3.3 | RBM |
| `helmet_with_faceguard` | Helmet with Faceguard | Plate | T5 | auto | 94 | 12 | 0 | 0 | 3.5 | RBM |
| `ironlame_feathered_spangenhelm_over_leather` | Feathered Spangenhelm over Leather | Plate | T5 | auto | 77 | 12 | 15 | 0 | 2.8 | RBM |
| `ironlame_roundkettle_over_imperial_mail` | Iron Roundkettle over Mail | Plate | T5 | auto | 78 | 0 | 25 | 0 | 3.6 | RBM |
| `leatherlame_feathered_spangenhelm_over_leather` | Spangenhelm over Leather | Plate | T5 | auto | 76 | 12 | 15 | 0 | 1.7 | RBM |
| `ironlame_feathered_spangenhelm` | Feathered Spangenhelm | Plate | T5 | auto | 65 | 12 | 25 | 0 | 3.2 | RBM |
| `leatherlame_roundkettle_over_imperial_mail` | Roundkettle over Mail | Plate | T5 | auto | 74 | 0 | 25 | 0 | 3.4 | RBM |
| `ironlame_spiked_kettle_over_mail` | Iron Spiked Kettle over Mail | Plate | T5 | auto | 73 | 0 | 25 | 0 | 3.8 | RBM |
| `ironlame_nasalhelm_over_imperial_padding` | Iron Nasalhelm over Padding | Plate | T5 | auto | 57 | 15 | 25 | 0 | 3.4 | RBM |
| `leatherlame_nasalhelm_over_imperial_leather` | Iron Nasalhelm over Leather | Plate | T5 | auto | 61 | 12 | 17 | 0 | 1.9 | RBM |
| `tall_helmet` | Tall Helmet | Plate | T5 | auto | 84 | 0 | 0 | 0 | 1.8 | RBM |
| `roundkettle_over_imperial_padding` | Roundkettle over Padding | Plate | T5 | auto | 67 | 0 | 16 | 0 | 1.4 | RBM |
| `mail_coif` | Mail Coif with Helmet Padding | Chainmail | T5 | auto | 38 | 12 | 38 | 0 | 1.7 | RBM |
| `spiked_kettle_over_imperial_padding` | Spiked Kettle over Padding | Plate | T5 | auto | 60 | 3 | 17 | 0 | 1.5 | RBM |
| `roundkettle_over_imperial_leather` | Light Roundkettle over Leather | Plate | T5 | auto | 74 | 0 | 0 | 0 | 1.3 | RBM |
| `leatherlame_roundkettle_over_imperial_leather` | Roundkettle over Leather | Plate | T5 | auto | 58 | 12 | 7 | 0 | 1.4 | RBM |
| `roundkettle_over_laced_cloth` | Roundkettle Over Laced Cloth | Plate | T5 | auto | 65 | 0 | 10 | 0 | 1.3 | RBM |
| `ironlame_nasalhelm_over_imperial_cloth` | Iron Legionary Helm over Cloth | Plate | T5 | auto | 55 | 12 | 6 | 0 | 3.2 | RBM |
| `roundkettle_over_imperial_cloth` | Roundkettle over Cloth | Plate | T5 | auto | 64 | 0 | 6 | 0 | 1.2 | RBM |
| `ironlame_roundkettle_over_imperial_leather` | Iron Roundkettle over Leather | Plate | T5 | auto | 56 | 0 | 15 | 0 | 3.1 | RBM |
| `ao_imperial_cataphracts_flamboyant_helmet` | Imperial Ornate Cataphract's Helmet | Plate | T5 | auto | 55 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_Empire_Lord_Guarded_Face_Helmet` | Imperial Guarded Lord Helmet with Faceplate | Plate | T5 | auto | 54 | 0 | 0 | 0 | 3.9 | OSA |
| `varangian_guard_helmet_a` | Imperial Guard's Helmet With Closed Plated Mail | Chainmail | T5 | auto | 54 | 0 | 0 | 0 | 5.8 | OSA |
| `varangian_guard_helmet_c` | Imperial Guard's Helmet With Stripped Cataphract Mail | Chainmail | T5 | auto | 53 | 0 | 0 | 0 | 5.4 | OSA |
| `AR_empire_lord_helmet_b` | Imperial Lord Helmet With Faceplate | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.7 | OSA |
| `DZ_empire_helmet_c` | Imperial Plumed Cataphract's Domed Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 4 | OSA |
| `DZ_empire_helmet_d` | Imperial Cataphract's Bent Conical Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.2 | OSA |
| `DZ_empire_helmet_i` | Imperial Cataphract's Feathered Pointed Kettle | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.2 | OSA |
| `imperial_lord_helm_mail` | Imperial Closed Guarded Lord Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.7 | OSA |
| `AO_empire_lord_helmet_k` | Imperial Plumed Heavy Palatine Helmet | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.6 | OSA |
| `ao_imperial_aristocrats_helmet` | Imperial Guarded Noble Helmet With Feather Crest | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.6 | OSA |
| `AR_empire_helmet_h` | Imperial Gilded Closed Helmet With Metal Strips | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_empire_lord_helmet_a` | Imperial Lord Helmet With Faceguard | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.6 | OSA |
| `AR_empire_lord_helmet_d` | Gilded Imperial Guarded Lord Helmet | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_empire_lord_helmet_f` | Imperial Guarded Spangenhelm With Feather Crest | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.7 | OSA |
| `DZ_empire_helmet_a` | Imperial Mailled Kettle Helmet | Plate | T5 | auto | 51 | 0 | 0 | 0 | 4 | OSA |
| `DZ_empire_helmet_b` | Imperial Cataphract's Domed Helmet | Plate | T5 | auto | 51 | 0 | 0 | 0 | 4 | OSA |
| `echerian_elite_helm` | Imperial Closed Guarded Lord Helmet | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.7 | OSA |
| `ao_imperial_guarded_conical_nasal_helmet` | Imperial Guarded Cataphract's Conical Helmet | Plate | T5 | auto | 50 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_imperial_guarded_decorated_nasal_helmet` | Imperial Guarded Decorated Nasal Helmet | Plate | T5 | auto | 50 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_closed_kettle_helmet_a` | Imperial Plumed Kettle Over Closed Mail | Plate | T5 | auto | 50 | 0 | 0 | 0 | 4 | OSA |
| `ar_empire_cataphracts_helmet_a` | Imperial Plumed Cataphract's Helmet | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.9 | OSA |
| `AR_empire_desert_helmet_e` | Southern Helmet with Metal Strips | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_empire_helmet_a` | Imperial Noble Jeweled Heavy Nasalhelm | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.6 | OSA |
| `AR_empire_helmet_g` | Imperial Gilded Helmet With Metal Strips | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_empire_lord_helmet_c_a` | Imperial Plumed Helmet with Metal Strips | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.9 | OSA |
| `AR_empire_lord_helmet_e` | Imperial Feathered Jeweled Helmet | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_gladiator_helmet_b` | Gilded Visored Secutor Helmet With Feathers | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3 | OSA |
| `AR_gladiator_helmet_d` | Visored Secutor Helmet With Feathers | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3 | OSA |
| `AR_Imperial_Crowned_Helmet_b` | Imperial Crowned Helmet With Faceplate | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.9 | OSA |
| `AR_roman_helmet_d_plumed_c` | Gilded Crested Helmet with Faceplate | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.9 | OSA |
| `DZ_empire_helmet_j` | Imperial Cataphract's Plumed Pointed Kettle | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.2 | OSA |
| `ao_imperial_closed_mail_nasal_spangenhelmet` | Imperial Feathered Spangenhelm With Closed Mail | Plate | T5 | auto | 49 | 0 | 0 | 0 | 1.8 | OSA |
| `TV_empire_lord_helmet_j` | Imperial Cataphract's Closed Fluted Helmet | Plate | T5 | auto | 49 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_imperial_cataphracts_closed_mail_helmet` | Imperial Cataphract's Closed Mail Helmet | Plate | T5 | auto | 48 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_imperial_cataphracts_plumed_closed_mail_helmet` | Imperial Cataphract's Plumed Spangenhelm Over Closed Mail | Plate | T5 | auto | 48 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_empire_helmet_b` | Imperial Noble Gilded Jeweled Heavy Nasalhelm | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.6 | OSA |
| `AR_empire_lord_helmet_g` | Imperial Silvered Ridge Helmet | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.1 | OSA |
| `AR_Imperial_Crowned_Helmet_a` | Imperial Crowned Helmet | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.9 | OSA |
| `AR_lion_head_c` | Ridge Helmet With Lion Head | Plate | T5 | auto | 48 | 0 | 0 | 0 | 4.8 | OSA |
| `DZ_empire_helmet_h` | Imperial Feathered Pointed Kettle Over Mail | Plate | T5 | auto | 48 | 0 | 0 | 0 | 2.85 | OSA |
| `empire_battle_crown_west` | Imperial Plumed Helmet | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.1 | OSA |
| `mp_ironlame_nasalhelm_over_imperial_mail` | Iron Nasalhelm over Mail | Plate | T5 | 1 | 48 | 0 | 0 | 0 | 3.3 | Vanilla |
| `solar_helmet` | Imperial Solar Helmet With Gilded Faceplate | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.5 | OSA |
| `solar_helmet_b` | Imperial Solar Helmet With Silvered Faceplate | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.5 | OSA |
| `TV_empire_lord_helmet_a` | Imperial Fluted Helmet With Metal Strips | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.8 | OSA |
| `TV_empire_lord_helmet_b` | Imperial Crested Decorated Silvered Ridge Helmet | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.1 | OSA |
| `TV_empire_lord_helmet_c` | Imperial Crested Decorated Gilded Ridge Helmet | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.1 | OSA |
| `TV_empire_lord_helmet_d` | Imperial Decorated Silvered Ridge Helmet | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.1 | OSA |
| `TV_empire_lord_helmet_e` | Imperial Decorated Gilded Ridge Helmet | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.1 | OSA |
| `leatherlame_roundkettle` | Roundkettle Helmet | Plate | T5 | auto | 47 | 0 | 0 | 0 | 1.8 | RBM |
| `TV_empire_lord_helmet_f` | Imperial Feather Crested Decorated Silvered Ridge Helmet | Plate | T5 | auto | 47 | 0 | 0 | 0 | 3.1 | OSA |
| `imperial_padded_coif` | Lightweight Padded Coif | Cloth | T5 | auto | 22 | 7 | 22 | 0 | 0.5 | RBM |
| `ao_imperial_evocati_helmet` | Imperial Evocatus' Helmet | Plate | T5 | auto | 46 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_imperial_plumed_cone_helmet_with_closed_mail` | Imperial Plumed Conical Helmet With Closed Mail | Plate | T5 | auto | 46 | 0 | 0 | 0 | 1.8 | OSA |
| `DZ_empire_helmet_g` | Imperial Pointed Kettle Over Mail | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3.1 | OSA |
| `empire_crown` | Jeweled Crown | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3.1 | RBM |
| `AR_empire_lord_helmet_h` | Imperial Jeweled Gilded Ridge Helmet | Plate | T5 | auto | 45 | 0 | 0 | 0 | 2.1 | OSA |
| `AR_empire_lord_helmet_j` | Imperial Jeweled Ridge Helmet | Plate | T5 | auto | 45 | 0 | 0 | 0 | 2.1 | OSA |
| `visored_helmet_x` | Visored Cap Helmet | Plate | T5 | auto | 45 | 0 | 0 | 0 | 4.2 | OSA |
| `ao_imperial_palatine_guard_helmet` | Imperial Palatine Helmet Over Mail | Plate | T5 | auto | 44 | 0 | 0 | 0 | 1.8 | OSA |
| `khuzait_spiked_kettle` | Imperial Closed Spiked Kettle | Chainmail | T5 | auto | 44 | 0 | 0 | 0 | 5 | OSA |
| `khuzait_ironlame_kettle` | Imperial Closed Iron Kettle | Chainmail | T5 | auto | 43 | 0 | 0 | 0 | 4 | OSA |
| `AR_empire_helmet_n` | Imperial Plumed Banded Helmet With Metal Strips | Plate | T5 | auto | 42 | 0 | 0 | 0 | 2.2 | OSA |
| `AR_empire_lord_helmet_i` | Imperial Plumed Decorated Banded Helmet With Metal Strips | Plate | T5 | auto | 42 | 0 | 0 | 0 | 2.2 | OSA |
| `DZ_empire_helmet_f` | Imperial Pointed Kettle Over Leather | Plate | T5 | auto | 42 | 0 | 0 | 0 | 2.5 | OSA |
| `imperial_mail_coif` | Mail Coif | Chainmail | T5 | auto | 41 | 0 | 0 | 0 | 1.7 | RBM |
| `ao_imperial_redcrest_bronze_spangenhelm_over_mail_coif` | Imperial Plumed Bronze Spangenhelm Over Mail Coif | Plate | T4 | auto | 40 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_gladiator_helmet_g` | Gilded Provocator Helmet With Faceplate | Plate | T4 | auto | 40 | 0 | 0 | 0 | 3.5 | OSA |
| `AR_gladiator_helmet_h` | Decorated Provocator Helmet With Faceplate | Plate | T4 | auto | 40 | 0 | 0 | 0 | 3.5 | OSA |
| `aserai_feathered_spangenhelm` | Southern Feathered Spangenhelm Over Mail | Plate | T4 | auto | 40 | 0 | 0 | 0 | 3.4 | OSA |
| `bronze_aserai_helm` | Southern Iron Nasalhelm Over Mail | Plate | T4 | auto | 40 | 0 | 0 | 0 | 3.4 | OSA |
| `AR_empire_desert_helmet_a` | Southern Plumed Nasalhelm Over Imperial Mail | Plate | T4 | auto | 39 | 0 | 0 | 0 | 3.2 | OSA |
| `AR_roman_helmet_d_strips_mail` | Plumed Tall Helmet Over Stripped Mail | Plate | T4 | auto | 39 | 0 | 0 | 0 | 3.6 | OSA |
| `AR_roman_helmet_e_strips_mail` | Tall Helmet Over Stripped Mail | Plate | T4 | auto | 39 | 0 | 0 | 0 | 3.6 | OSA |
| `DZ_empire_helmet_e` | Imperial Pointed Kettle Over Padding | Plate | T4 | auto | 39 | 0 | 0 | 0 | 2.5 | OSA |
| `imperial_open_mail_coif` | Open Mail Coif | Chainmail | T4 | auto | 39 | 0 | 0 | 0 | 1.5 | RBM |
| `TV_empire_lord_helmet_g` | Imperial Pointed Ridge Helmet With Faceguard Over Mail | Plate | T4 | auto | 39 | 0 | 0 | 0 | 3.6 | OSA |
| `TV_empire_lord_helmet_h` | Imperial Gilded Plumed Spangenhelm Over Mail | Plate | T4 | auto | 39 | 0 | 0 | 0 | 3.6 | OSA |
| `ao_imperial_noblemans_cone_helmet` | Imperial Plumed Conical Helmet Over Mail Coif | Plate | T4 | auto | 38 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_empire_helmet_m` | Imperial Banded Helmet With Leather Strips | Plate | T4 | auto | 38 | 0 | 0 | 0 | 2.2 | OSA |
| `AR_empire_legatus_helm_a` | Imperial Decorated Crested Lord Helmet | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_empire_legatus_helm_b` | Imperial Gilded Crested Lord Helmet | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_gladiator_helmet_a2` | Gilded Secutor Helmet With Feathers | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3 | OSA |
| `AR_gladiator_helmet_c2` | Secutor Helmet With Feathers | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3 | OSA |
| `mp_empire_guarded_lord_helmet` | Guarded Lord Helmet | Plate | T4 | 1 | 38 | 0 | 0 | 0 | 3.7 | Vanilla |
| `mp_empire_helmet_with_metal_strips` | Guarded Lord Helmet | Plate | T4 | 1 | 38 | 0 | 0 | 0 | 3.7 | Vanilla |
| `TV_empire_helmet_a` | Imperial Feathered Decorated Steel Cap Over Stripped Mail | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3.9 | OSA |
| `TV_empire_helmet_d` | Imperial Decorated Banded Helmet With Faceguard Over Mail Coif | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3.5 | OSA |
| `TV_empire_helmet_f` | Imperial Spiked Kettle Helmet Over Closed Mail Coif | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3.7 | OSA |
| `ao_imperial_crowned_nasal_helmet` | Imperial Crowned Bronze Spangenhelm With Cloth | Plate | T4 | auto | 37 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_imperial_guarded_nasal_helmet` | Imperial Guarded Nasal Helmet | Plate | T4 | auto | 37 | 0 | 0 | 0 | 1.8 | OSA |
| `mp_empire_lord_helmet` | Lordly Helmet | Plate | T4 | 1 | 37 | 0 | 0 | 0 | 3.5 | Vanilla |
| `open_mail_coif` | Open Mail Coif | Chainmail | T4 | auto | 37 | 0 | 0 | 0 | 1.6 | RBM |
| `AR_empire_crested_helm_a` | Imperial Crested Helmet With Faceguard | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_empire_helmet_l` | Imperial Banded Helmet | Plate | T4 | auto | 36 | 0 | 0 | 0 | 2.2 | OSA |
| `AR_roman_helmet_b_fur_c` | Imperial Scout's Plumed Helmet | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_roman_helmet_d_plumed_b` | Gilded Plumed Helmet | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.9 | OSA |
| `mp_ironlame_roundkettle_over_imperial_mail` | Iron Roundkettle over Mail | Plate | T4 | 1 | 36 | 0 | 0 | 0 | 2.85 | Vanilla |
| `TV_empire_crested_helm_a` | Imperial Gilded Crested Helmet With Faceguard | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.7 | OSA |
| `TV_empire_helmet_e` | Imperial Roundkettle Over Closed Mail Coif | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.7 | OSA |
| `AO_empire_helmet_za` | Imperial Palatine Helmet Over Leather | Plate | T4 | auto | 35 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_imperial_cone_helmet_with_lamellar_strips` | Imperial Conical Helmet With Mail And Lamellar Strips | Plate | T4 | auto | 35 | 0 | 0 | 0 | 1.8 | OSA |
| `mp_leatherlame_nasalhelm_over_imperial_mail` | Bronze Nasalhelm over Mail | Plate | T4 | 1 | 35 | 0 | 0 | 0 | 3.7 | Vanilla |
| `roman_helmet_z` | Tall Helmet Over Stripped Cloth | Plate | T4 | auto | 35 | 0 | 0 | 0 | 3.5 | OSA |
| `ao_imperial_heavy_nasal_spangenhelm_over_mail_coif` | Imperial Heavy Spangenhelm Over Mail Coif | Plate | T3 | auto | 34 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_empire_archer_helmet_d` | Imperial Sagittarius Helmet Over Scale | Plate | T3 | auto | 34 | 0 | 0 | 0 | 3.2 | OSA |
| `AR_empire_archer_helmet_d2` | Imperial Gilded Sagittarius Helmet Over Scale | Plate | T3 | auto | 34 | 0 | 0 | 0 | 3.2 | OSA |
| `AR_empire_desert_helmet_d` | Southern Nasalhelm Over Leather | Plate | T3 | auto | 34 | 0 | 0 | 0 | 1.9 | OSA |
| `AR_intercisa_helmet_d` | Imperial Feathered Decorated Ridged Helmet With Metal Crest | Plate | T3 | auto | 34 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_intercisa_helmet_e` | Imperial Feathered Gilded Ridged Helmet With Metal Crest | Plate | T3 | auto | 34 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_roman_helmet_d_plumed_a` | Decorated Plumed Helmet | Plate | T3 | auto | 34 | 0 | 0 | 0 | 3.9 | OSA |
| `mp_heavy_nasalhelm_over_imperial_mail` | Heavy Nasalhelm over Mail | Plate | T3 | 1 | 34 | 0 | 0 | 0 | 3.6 | Vanilla |
| `TV_empire_helmet_c` | Imperial Decorated Banded Helmet Over Mail Coif | Plate | T3 | auto | 34 | 0 | 0 | 0 | 3.5 | OSA |
| `TV_empire_helmet_h` | Imperial Gilded Spangenhelm Over Mail | Plate | T3 | auto | 34 | 0 | 0 | 0 | 2.7 | OSA |
| `ao_imperial_pointed_helmet_with_mail_coif` | Pointed Helmet With Lamellar Strips | Plate | T3 | auto | 33 | 0 | 0 | 0 | 1.8 | OSA |
| `mp_ironlame_spiked_kettle_over_mail` | Iron Spiked Kettle over Mail | Plate | T3 | 1 | 33 | 0 | 0 | 0 | 3.8 | Vanilla |
| `AO_empire_helmet_zb` | Imperial Palatine Helmet Over Cloth | Plate | T3 | auto | 32 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_imperial_cone_helmet_with_leather` | Imperial Conical Helmet With Leather | Plate | T3 | auto | 32 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_empire_helmet_r` | Imperial Spangenhelm Over Stripped Cloth | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.2 | OSA |
| `AR_empire_legatus_helm_e` | Imperial Plumed Reinforced Cavalry Helmet | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.5 | OSA |
| `AR_intercisa_helmet_c` | Imperial Feathered Ridged Helmet With Metal Crest | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_roman_helmet_b_fur_a` | Imperial Scout's Helmet With Faceguard | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.7 | OSA |
| `TV_empire_helmet_b` | Imperial Spiked Roundkettle Over Mail | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.8 | OSA |
| `ao_imperial_heavy_spangenhelm_over_mail_coif` | Imperial Heavy Spangenhelm Over Mail Coif | Plate | T3 | auto | 31 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_empire_legatus_helm_d` | Imperial Feathered Reinforced Cavalry Helmet | Plate | T3 | auto | 31 | 0 | 0 | 0 | 3.5 | OSA |
| `ao_imperial_kettlehelm_over_mail_coif` | Imperial Roundkettle Over Mail Coif | Plate | T3 | auto | 30 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_empire_archer_helmet_c` | Imperial Sagittarius Helmet Over Mail | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.6 | OSA |
| `AR_empire_archer_helmet_c2` | Imperial Gilded Sagittarius Helmet Over Mail | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.6 | OSA |
| `AR_empire_helmet_q` | Imperial Spangenhelm Over Cloth | Plate | T3 | auto | 30 | 0 | 0 | 0 | 2.7 | OSA |
| `AR_empire_helmet_s` | Eastern Spangenhelm Over Cloth | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.2 | OSA |
| `AR_empire_legatus_helm_c` | Imperial Reinforced Cavalry Helmet | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.5 | OSA |
| `AR_intercisa_helmet_a` | Imperial Ridged Helmet With Metal Crest | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_roman_helmet_b_fur_b` | Imperial Scout's Tall Helmet | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_roman_helmet_b_plumed` | Plumed Helmet With Faceguard | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_roman_helmet_b_plumed_b` | Plumed Feathered Helmet With Faceguard | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_roman_helmet_d_strips` | Plumed Tall Helmet Over Stripped Cloth | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.5 | OSA |
| `mp_heavy_nasalhelm_over_imperial_leather` | Heavy Nasalhelm over Leather | Plate | T3 | 1 | 30 | 0 | 0 | 0 | 3 | Vanilla |
| `ao_imperial_nasal_helmet` | Imperial Nasal Helmet | Plate | T3 | auto | 29 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_imperial_pointed_helmet_with_lamellar_strips` | Pointed Helmet With Lamellar Strips | Plate | T3 | auto | 29 | 0 | 0 | 0 | 1.8 | OSA |
| `mp_leatherlame_feathered_spangenhelm_over_leather` | Bronze Feathered Spangenhelm over Leather | Plate | T3 | 1 | 29 | 0 | 0 | 0 | 1.7 | Vanilla |
| `TV_empire_helmet_g` | Imperial Gilded Spangenhelm Over Padded Cloth | Plate | T3 | auto | 28 | 0 | 0 | 0 | 2.7 | OSA |
| `ao_imperial_kettlehelm_with_mail` | Imperial Roundkettle With Mail | Plate | T2 | auto | 27 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_imperial_nasal_spangenhelmet_with_mail` | Imperial Nasal Spangenhelm With Mail | Plate | T2 | auto | 27 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_empire_desert_helmet_b` | Southern Nasalhelm Over Imperial Cloth | Plate | T2 | auto | 26 | 0 | 0 | 0 | 3.2 | OSA |
| `AR_empire_helmet_o` | Imperial Brass Scale Coif | Plate | T2 | auto | 26 | 0 | 0 | 0 | 1.7 | OSA |
| `AR_empire_helmet_o2` | Imperial Alternating Scale Coif | Plate | T2 | auto | 26 | 0 | 0 | 0 | 1.7 | OSA |
| `AR_empire_helmet_o3` | Imperial Steel Scale Coif | Plate | T2 | auto | 26 | 0 | 0 | 0 | 1.7 | OSA |
| `AR_gladiator_helmet_a` | Gilded Secutor Helmet | Plate | T2 | auto | 26 | 0 | 0 | 0 | 3.2 | OSA |
| `AR_gladiator_helmet_c` | Secutor Helmet | Plate | T2 | auto | 26 | 0 | 0 | 0 | 3.2 | OSA |
| `ao_imperial_heavy_spangenhelm_with_mail` | Imperial Heavy Spangenhelm With Mail | Plate | T2 | auto | 25 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_imperial_nasal_spangenhelm_with_leather_strips` | Imperial Nasal Spangenhelm With Leather Strips | Plate | T2 | auto | 25 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_imperial_spangenhelm_with_mail` | Imperial Spangenhelm With Mail | Plate | T2 | auto | 25 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_empire_archer_helmet_a` | Imperial Sagittarius Helmet Over Leather | Plate | T2 | auto | 25 | 0 | 0 | 0 | 1.3 | OSA |
| `AR_empire_archer_helmet_a2` | Imperial Gilded Sagittarius Helmet Over Leather | Plate | T2 | auto | 25 | 0 | 0 | 0 | 1.3 | OSA |
| `imperial_leather_coif` | Leather Coif | Leather | T2 | auto | 23 | 0 | 0 | 0 | 0.3 | RBM |
| `ao_imperial_heavy_spangenhelm_with_leather` | Imperial Heavy Spangenhelm With Leather | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_empire_desert_helmet_c` | Southern Spangenhelm Cap | Plate | T2 | auto | 22 | 0 | 0 | 0 | 3.4 | OSA |
| `ao_imperial_pointed_helmet_with_leather` | Pointed Helmet With Leather | Plate | T2 | auto | 21 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_imperial_spangenhelm_over_padded_coif` | Imperial Spangenhelm Over Padded Coif | Plate | T2 | auto | 21 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_intercisa_helmet_b` | Imperial Open Ridged Helmet With Metal Crest | Plate | T2 | auto | 21 | 0 | 0 | 0 | 3.5 | OSA |
| `ao_imperial_kettlehelm_with_leather` | Imperial Roundkettle With Leather | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_empire_archer_helmet_b` | Imperial Sagittarius Helmet Over Cloth | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.3 | OSA |
| `AR_empire_archer_helmet_b2` | Imperial Gilded Sagittarius Helmet Over Cloth | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.3 | OSA |
| `AR_gladiator_helmet_e` | Gilded Provocator Helmet | Plate | T1 | auto | 20 | 0 | 0 | 0 | 3.5 | OSA |
| `AR_gladiator_helmet_f` | Decorated Provocator Helmet | Plate | T1 | auto | 20 | 0 | 0 | 0 | 3.5 | OSA |
| `AR_roman_helmet_a` | Imperial Open Helmet With Faceguard | Plate | T1 | auto | 20 | 0 | 0 | 0 | 3.4 | OSA |
| `AR_roman_helmet_a_fur_a` | Imperial Scout's Ridged Cap | Plate | T1 | auto | 20 | 0 | 0 | 0 | 3.4 | OSA |
| `AR_roman_helmet_a_fur_b` | Imperial Scout's Banded Cap | Plate | T1 | auto | 20 | 0 | 0 | 0 | 3.4 | OSA |
| `mail_coif_pilgrim_hood` | Pilgrim's Hood over Mail Coif | Cloth | T1 | auto | 20 | 0 | 0 | 0 | 1 | OSA |
| `mp_spiked_kettle_over_imperial_padding` | Spiked Kettle over Padding | Plate | T1 | 1 | 20 | 0 | 0 | 0 | 1.5 | Vanilla |
| `ao_imperial_pointed_helmet_with_cloth_wrap` | Pointed Helmet With Cloth Wrap | Plate | T1 | auto | 19 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_imperial_spangenhelm_with_leather` | Imperial Spangenhelm With Leather | Plate | T1 | auto | 19 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_roman_helmet_cap` | Imperial Cap Helmet | Plate | T1 | auto | 18 | 0 | 0 | 0 | 3.2 | OSA |
| `mp_mail_coif` | Mail Coif with Helmet Padding | Chainmail | T1 | 1 | 18 | 0 | 0 | 0 | 1.7 | Vanilla |
| `mp_open_mail_coif` | Open Mail Coif | Chainmail | T1 | 1 | 18 | 0 | 0 | 0 | 1.6 | Vanilla |
| `imperial_cloth_coif` | Cloth Coif | Cloth | T1 | auto | 17 | 0 | 0 | 0 | 0.2 | RBM |
| `AR_empire_helmet_p` | Imperial Padded Cloth Coif | Cloth | T1 | auto | 16 | 0 | 0 | 0 | 0.5 | OSA |
| `empire_crown_north` | Golden Laurel Crown | Plate | T1 | auto | 15 | 0 | 0 | 0 | 0.3 | RBM |
| `empire_crown_v2` | Imperial Jeweled Band | Plate | T1 | auto | 15 | 0 | 0 | 0 | 0.85 | RBM |
| `empire_crown_west` | Imperial Jeweled Band | Plate | T1 | auto | 15 | 0 | 0 | 0 | 0.5 | RBM |
| `tiara_x` | Imperial Tiara | Plate | T1 | auto | 15 | 0 | 0 | 0 | 3.1 | OSA |
| `vlandia_crown` | Austere Crown | Plate | T1 | auto | 15 | 0 | 0 | 0 | 0.35 | RBM |
| `laced_cloth_coif` | Laced Cloth Coif | Cloth | T1 | auto | 14 | 0 | 0 | 0 | 0.4 | RBM |
| `leather_cap` | Leather Cap | Leather | T1 | auto | 14 | 0 | 0 | 0 | 0.4 | RBM |
| `mp_heavy_nasalhelm_over_laced_cloth` | Heavy Nasalhelm over Laced Cloth | Plate | T0 | 1 | 13 | 0 | 0 | 0 | 3.1 | Vanilla |
| `pilgrim_hood` | Pilgrim's Hood | Cloth | T0 | auto | 7 | 5 | 0 | 0 | 0.2 | RBM |
| `AR_hat_a` | Straw Hat | Cloth | T-1 | auto | 6 | 0 | 0 | 0 | 1.2 | OSA |
| `AR_hat_b` | Straw Hat With Feather | Cloth | T-1 | auto | 6 | 0 | 0 | 0 | 1.2 | OSA |
| `AR_empire_laurel_a` | Laurel Headband | Leather | T-1 | auto | 4 | 0 | 0 | 0 | 1 | OSA |
| `AR_empire_laurel_a2` | Vine Headband | Leather | T-1 | auto | 4 | 0 | 0 | 0 | 1 | OSA |
| `AR_headband_a` | Wrapped Headband | Leather | T-1 | auto | 4 | 0 | 0 | 0 | 1 | OSA |
| `AR_phrygian_cap_a` | Banded Bent Conical Hat | Cloth | T-1 | auto | 4 | 0 | 0 | 0 | 1.2 | OSA |
| `AR_roman_hat_b` | Imperial Fur Hat | Leather | T-1 | auto | 4 | 0 | 0 | 0 | 1.4 | OSA |
| `AR_roman_hat_b2` | Imperial Conical Fur Hat | Leather | T-1 | auto | 4 | 0 | 0 | 0 | 1.4 | OSA |
| `mp_laced_cloth_coif` | Laced Cloth Coif | Cloth | T-1 | 1 | 4 | 0 | 0 | 0 | 0.4 | Vanilla |
| `TV_phrygian_cap_a` | Bent Conical Hat | Cloth | T-1 | auto | 4 | 0 | 0 | 0 | 1.2 | OSA |
| `TV_phrygian_cap_b` | Plain Bent Conical Hat | Cloth | T-1 | auto | 4 | 0 | 0 | 0 | 1.2 | OSA |
| `AR_bandit_hood_a` | Open Pilgrim's Hood | Cloth | T-1 | auto | 3 | 0 | 0 | 0 | 0.2 | OSA |
| `AR_roman_hat_a` | Imperial Decorated Hat | Leather | T-1 | auto | 2 | 0 | 0 | 0 | 1.4 | OSA |
| `AR_roman_hat_a2` | Imperial Feathered Decorated Hat | Leather | T-1 | auto | 2 | 0 | 0 | 0 | 1.4 | OSA |
| `AR_roman_hat_c` | Imperial Decorated Leather Hat | Leather | T-1 | auto | 2 | 0 | 0 | 0 | 1.4 | OSA |
| `AR_roman_hat_d` | Conical Cloth Hat | Leather | T-1 | auto | 2 | 0 | 0 | 0 | 1.4 | OSA |
| `AR_roman_hat_e` | Plain Conical Cloth Hat | Leather | T-1 | auto | 2 | 0 | 0 | 0 | 1.4 | OSA |
| `AR_roman_hat_f` | Conical Leather Hat | Leather | T-1 | auto | 2 | 0 | 0 | 0 | 1.4 | OSA |
| `AR_satan_hat` | Festive Hat | Leather | T-1 | auto | 2 | 0 | 0 | 0 | 1.4 | OSA |

<!-- CULTURE:vlandia COUNT:231 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `full_helm_over_mail_coif` | Full Helm Over Mail Coif | Plate | T5 | auto | 140 | 116 | 40 | 0 | 4.5 | RBM |
| `full_helm_over_padded_cloth` | Full Helm Over Padded Cloth | Plate | T5 | auto | 116 | 116 | 12 | 0 | 3.1 | RBM |
| `full_helm_over_laced_coif` | Full Helm Over Laced Coif | Plate | T5 | auto | 111 | 113 | 20 | 0 | 3.5 | RBM |
| `full_helm_over_cloth_headwrap` | Full Helm Over Cloth Headwrap | Plate | T5 | auto | 113 | 116 | 12 | 0 | 3.2 | RBM |
| `full_helm_over_arming_coif` | Full Helm Over Arming Coif | Plate | T5 | auto | 109 | 116 | 0 | 0 | 3 | RBM |
| `visored_helmet_over_mail_coif` | Visored Helmet Over Mail Coif | Plate | T5 | auto | 109 | 75 | 40 | 0 | 3.6 | RBM |
| `full_helm_over_padded_cap` | Full Helm Over Padded Cap | Plate | T5 | auto | 108 | 116 | 0 | 0 | 3.1 | RBM |
| `visored_helmet_over_padded_coif` | Visored Helmet Over Padded Coif | Plate | T5 | auto | 89 | 75 | 25 | 0 | 2.5 | RBM |
| `visored_helmet_over_laced_coif` | Visored Helmet over Laced Coif | Plate | T5 | auto | 90 | 75 | 20 | 0 | 2.4 | RBM |
| `vlandian_faceguard_helmet_a` | Knight Helmet with Steel Faceguard | Plate | T5 | auto | 90 | 90 | 0 | 0 | 2.2 | RBM |
| `visored_helmet_over_padded_cloth` | Visored Helmet Over Padded Cloth | Plate | T5 | auto | 83 | 75 | 12 | 0 | 2.3 | RBM |
| `guards_kettle_over_mail_coif` | Kettle over Mail Coif | Plate | T5 | auto | 109 | 12 | 40 | 0 | 3.2 | RBM |
| `vlandian_faceguard_helmet_b` | Knight Helmet with Bronze Faceguard | Plate | T5 | auto | 90 | 70 | 0 | 0 | 2.3 | RBM |
| `segmented_skullcap_over_mail_coif` | Segmented Skullcap over Mail Coif | Plate | T5 | auto | 92 | 25 | 40 | 0 | 3.5 | RBM |
| `nasal_helmet_over_mail_coif` | Nasal Helmet over Mail Coif | Plate | T5 | auto | 89 | 24 | 40 | 0 | 2.7 | RBM |
| `kettle_hat_over_mail_coif` | Kettle Hat Over Mail Coif | Plate | T5 | auto | 105 | 0 | 40 | 0 | 2.9 | RBM |
| `peaked_helmet_over_mail_coif` | Peaked Helmet over Mail Coif | Plate | T5 | auto | 95 | 12 | 40 | 0 | 2.7 | RBM |
| `nasal_cervelliere_over_mail_coif` | Nasal Cervelliere over Mail Coif | Plate | T5 | auto | 81 | 24 | 40 | 0 | 2.6 | RBM |
| `western_crowned_plated_helmet` | Crowned Plate Helmet | Plate | T5 | auto | 105 | 15 | 20 | 0 | 3.9 | RBM |
| `kettle_helmet_over_mail` | Kettle Helmet over Mail | Plate | T5 | auto | 107 | 12 | 20 | 0 | 3.1 | RBM |
| `segmented_cervelliere_over_mail_coif` | Segmented Cervelliere over Mail Coif | Plate | T5 | auto | 86 | 12 | 40 | 0 | 3.2 | RBM |
| `kettle_helmet_with_mail` | Kettle Helmet with Mail | Plate | T5 | auto | 92 | 5 | 33 | 0 | 2.7 | RBM |
| `guards_kettle_over_padded_coif` | Kettle over Padded Coif | Plate | T5 | auto | 89 | 5 | 25 | 0 | 1.5 | RBM |
| `nasal_helmet_over_mail` | Nasal Helmet over Mail | Plate | T5 | auto | 86 | 12 | 20 | 0 | 3.1 | RBM |
| `western_plated_helmet` | Plated Helmet with Sideguards | Plate | T5 | auto | 98 | 15 | 0 | 0 | 4.1 | RBM |
| `peaked_helmet_over_mail` | Peaked Helmet over Mail | Plate | T5 | auto | 92 | 0 | 20 | 0 | 2.6 | RBM |
| `western_crowned_helmet` | Crowned Helmet | Plate | T5 | auto | 100 | 10 | 0 | 0 | 3.7 | RBM |
| `kettle_hat_over_padded_coif` | Kettle Hat Over Padded Coif | Plate | T5 | auto | 84 | 5 | 22 | 0 | 1.4 | RBM |
| `kettle_helmet_over_padded_coif` | Kettle Helmet over Padded Coif | Plate | T5 | auto | 85 | 0 | 25 | 0 | 1.3 | RBM |
| `segmented_skullcap_over_padded_coif` | Segmented Skullcap over Padded Coif | Plate | T5 | auto | 75 | 12 | 20 | 0 | 1.3 | RBM |
| `segmented_cervelliere_over_mail` | Segmented Cervelliere over Mail | Plate | T5 | auto | 84 | 0 | 20 | 0 | 3.3 | RBM |
| `guards_kettle_over_laced_coif` | Kettle over Laced Coif | Plate | T5 | auto | 83 | 0 | 20 | 0 | 1.4 | RBM |
| `segmented_skullcap_over_laced_coif` | Segmented Skullcap Over Laced Coif | Plate | T5 | auto | 73 | 12 | 20 | 0 | 1.2 | RBM |
| `kettle_helmet_over_laced_coif` | Kettle Helmet over Laced Coif | Plate | T5 | auto | 80 | 0 | 20 | 0 | 1.2 | RBM |
| `nasal_cervelliere_over_padded_coif` | Nasal Cervelliere over Padded Coif | Plate | T5 | auto | 61 | 15 | 25 | 0 | 1.2 | RBM |
| `segmented_skullcap_over_padded_cloth` | Segmented Skullcap over Padded Cloth | Plate | T5 | auto | 77 | 8 | 10 | 0 | 1.1 | RBM |
| `kettle_helmet_over_padded_cloth` | Kettle Helmet Over Padded Cloth | Plate | T5 | auto | 83 | 0 | 10 | 0 | 1.1 | RBM |
| `nasal_helmet_over_padded_coif` | Nasal Helmet over Padded Coif | Plate | T5 | auto | 58 | 15 | 25 | 0 | 1.4 | RBM |
| `vlandia_lord_helmet_b2` | Plated Helmet | Plate | T5 | auto | 90 | 0 | 0 | 0 | 4.1 | RBM |
| `kettle_helmet_with_leather` | Kettle Helmet with Leather | Plate | T5 | auto | 88 | 0 | 0 | 0 | 1 | RBM |
| `kettle_helmet_over_arming_coif` | Kettle Helmet over Arming Coif | Plate | T5 | auto | 87 | 0 | 0 | 0 | 1 | RBM |
| `kettle_hat_over_padded_cloth` | Kettle Hat over Padded Cloth | Plate | T5 | auto | 85 | 0 | 0 | 0 | 1.2 | RBM |
| `nasal_helmet_over_laced_coif` | Nasal Helmet Over Laced Coif | Plate | T5 | auto | 60 | 12 | 18 | 0 | 1.3 | RBM |
| `nasal_cervelliere_over_laced_coif` | Nasal Cervelliere Over Laced Coif | Plate | T5 | auto | 59 | 12 | 13 | 0 | 1.1 | RBM |
| `peaked_helmet_over_padded_cloth` | Peaked Helmet over Padded Cloth | Plate | T5 | auto | 68 | 0 | 12 | 0 | 1 | RBM |
| `nasal_helmet_over_cloth_headwrap` | Nasal Helmet Over Cloth Headwrap | Plate | T5 | auto | 56 | 16 | 10 | 0 | 1.1 | RBM |
| `kettle_helmet_over_padded_cap` | Kettle Helmet over Padded Cap | Plate | T5 | auto | 77 | 0 | 0 | 0 | 1.2 | RBM |
| `peaked_helmet_over_laced_coif` | Peaked Helmet over Laced Coif | Plate | T5 | auto | 65 | 0 | 12 | 0 | 1.1 | RBM |
| `nasal_helmet_over_padded_cloth` | Nasal Helmet Over Padded Cloth | Plate | T5 | auto | 57 | 12 | 8 | 0 | 1.2 | RBM |
| `segmented_cervelliere_over_padded_cloth` | Segmented Cervelliere over Padded Cloth | Plate | T5 | auto | 56 | 12 | 7 | 0 | 1.2 | RBM |
| `nasal_cervelliere_over_padded_cap` | Nasal Cervelliere Over Padded Cap | Plate | T5 | auto | 54 | 12 | 0 | 0 | 1 | RBM |
| `cervelliere_over_cloth_headwrap` | Cervelliere over Cloth Headwrap | Plate | T5 | auto | 50 | 6 | 10 | 0 | 1 | RBM |
| `segmented_cervelliere_over_laced_coif` | Segmented Cervelliere over Laced Coif | Plate | T5 | auto | 53 | 0 | 12 | 0 | 1.3 | RBM |
| `cervelliere_over_laced_coif` | Cervelliere Over Laced Coif | Plate | T5 | auto | 48 | 0 | 15 | 0 | 1.1 | RBM |
| `tv_vlandia_lord_helmet_a` | Western Fluted Helmet With Steel Visor Over Mail | Plate | T5 | auto | 58 | 0 | 0 | 0 | 4.8 | OSA |
| `tv_vlandia_lord_helmet_a2` | Western Fluted Helmet With Gilded Visor Over Mail | Plate | T5 | auto | 58 | 0 | 0 | 0 | 4.8 | OSA |
| `tv_vlandia_lord_helmet_b` | Western Fluted Helmet With Steel Visor Over Mail | Plate | T5 | auto | 58 | 0 | 0 | 0 | 4.8 | OSA |
| `tv_vlandia_lord_helmet_b2` | Western Fluted Helmet With Gilded Visor Over Mail | Plate | T5 | auto | 58 | 0 | 0 | 0 | 4.8 | OSA |
| `full_helm_over_mail_coif_z` | Western Crowned Plate Helmet With Visor | Plate | T5 | auto | 55 | 0 | 0 | 0 | 5 | OSA |
| `cervelliere_over_arming_coif` | Cervelliere over Arming Coif | Plate | T5 | auto | 53 | 0 | 0 | 0 | 0.9 | RBM |
| `tv_vlandia_lord_helmet_c` | Western Noble Peaked Lancer's Helmet | Plate | T5 | auto | 53 | 0 | 0 | 0 | 3.2 | OSA |
| `tv_vlandia_lord_helmet_d` | Western Crested Noble Peaked Lancer's Helmet | Plate | T5 | auto | 53 | 0 | 0 | 0 | 3.2 | OSA |
| `ao_battanian_guarded_aristocrats_spangenhelmet_crest` | Western Crested Guarded Spangenhelm | Plate | T5 | auto | 52 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_vlandia_helmet_t` | Western Cataphract's Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_vlandia_lord_helmet_d` | Western Noble Ridge Helmet With Faceplate | Plate | T5 | auto | 52 | 0 | 0 | 0 | 2.2 | OSA |
| `AR_vlandia_royal_helmet_a` | Western Royal Cataphract's Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_vlandia_royal_helmet_b` | Western Royal Cataphract's Shrouded Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.7 | OSA |
| `tv_vlandia_lord_helmet_f` | Western Bent Conical Helmet With Steel Visor Over Mail | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.6 | OSA |
| `tv_vlandia_lord_helmet_f2` | Western Bent Conical Helmet With Gilded Visor Over Mail | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.6 | OSA |
| `tv_vlandia_lord_helmet_j` | Knightly Helmet With Gilded Faceguard Over Mail | Plate | T5 | auto | 52 | 0 | 0 | 0 | 2.3 | OSA |
| `tv_vlandia_lord_helmet_j2` | Knightly Helmet With Steel Faceguard Over Mail | Plate | T5 | auto | 52 | 0 | 0 | 0 | 2.3 | OSA |
| `tv_vlandia_lord_helmet_l` | Western Bent Conical Helmet With Visor Over Mail Coif | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.6 | OSA |
| `tv_vlandia_lord_helmet_l2` | Western Gilded Bent Conical Helmet With Visor Over Mail Coif | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.6 | OSA |
| `AR_vlandia_lord_helmet_a` | Western Noble Nasalhelm With Feather Crest | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.7 | OSA |
| `cervelliere_over_padded_cap` | Cervelliere over Padded Cap | Plate | T5 | auto | 51 | 0 | 0 | 0 | 1 | RBM |
| `tv_vlandia_lord_helmet_h` | Western Segmented Cevelliere With Steel Visor Over Mail | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.6 | OSA |
| `tv_vlandia_lord_helmet_h2` | Western Segmented Cevelliere With Gilded Visor Over Mail | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.6 | OSA |
| `tv_vlandia_lord_helmet_i` | Western Noble Domed Helmet Over Scale | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.1 | OSA |
| `tv_vlandia_lord_helmet_k` | Western Segmented Cevelliere With Visor Over Mail Coif | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.6 | OSA |
| `tv_vlandia_lord_helmet_k2` | Western Gilded Segmented Cevelliere With Visor Over Mail Coif | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.6 | OSA |
| `AR_vlandia_helmet_r` | Bronze Plumed Spangenhelm Over Plated Mail Coif | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.9 | OSA |
| `AR_vlandia_helmet_r2` | Bronze Crested Spangenhelm Over Plated Mail Coif | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.9 | OSA |
| `AR_vlandia_helmet_s` | Western Peaked Cataphract's Helmet | Plate | T5 | auto | 50 | 0 | 0 | 0 | 4.4 | OSA |
| `AR_vlandian_crown_mask_b` | Western Crowned Mask Helmet | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.7 | OSA |
| `full_helm_over_mail_coif_x` | Western Crowned Helmet With Visor | Plate | T5 | auto | 50 | 0 | 0 | 0 | 5 | OSA |
| `tv_vlandia_helmet_k4` | Western Peaked Helmet Over Iron Scale | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3 | OSA |
| `tv_vlandia_helmet_k5` | Western Peaked Helmet Over Steel Scale | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3 | OSA |
| `TV_vlandia_royal_helmet_c` | Western Crowned Fluted Helmet Over Mail | Plate | T5 | auto | 50 | 0 | 0 | 0 | 4.1 | OSA |
| `mp_full_helm_over_padded_cloth` | Full Helm over Padded Cloth | Plate | T5 | 1 | 48 | 0 | 0 | 0 | 3.6 | Vanilla |
| `tv_vlandia_helmet_a5` | Western Painted Nasalhelm Over Closed Mail Coif | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.2 | OSA |
| `tv_vlandia_helmet_b5` | Western Nasal Helmet Over Closed Mail Coif | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.2 | OSA |
| `tv_vlandia_helmet_c5` | Western Conical Nasalhelm Over Closed Mail Coif | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.2 | OSA |
| `tv_vlandia_helmet_i4` | Western Bent Conical Helmet Over Closed Mail Coif | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.2 | OSA |
| `tv_vlandia_helmet_j2` | Western Heavy Nasalhelm Over Closed Mail Coif | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.2 | OSA |
| `tv_vlandia_helmet_k6` | Western Peaked Helmet Over Closed Mail Coif | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.2 | OSA |
| `tv_vlandia_helmet_o4` | Western Banded Nasal Cevelliere Over Closed Mail Coif | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.2 | OSA |
| `TV_vlandia_helmet_p2` | Western Pointed Skullcap Over Closed Mail Coif | Plate | T5 | auto | 48 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_vlandia_helmet_q2` | Western Nasal Helmet Over Closed Mail Coif | Plate | T5 | auto | 48 | 0 | 0 | 0 | 2.8 | OSA |
| `tv_vlandia_helmet_r4` | Western Banded Nasalhelm Over Closed Mail Coif | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.2 | OSA |
| `tv_vlandia_helmet_t4` | Western Bent Conical Nasalhelm Over Closed Mail Coif | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.2 | OSA |
| `tv_vlandia_helmet_d` | Western Fluted Helmet Over Mail | Plate | T5 | auto | 47 | 0 | 0 | 0 | 4.1 | OSA |
| `mp_visored_helmet_over_laced_coif` | Visored Helmet over Laced Coif | Plate | T5 | 1 | 46 | 0 | 0 | 0 | 3.2 | Vanilla |
| `mp_visored_helmet_over_padded_cloth` | Visored Helmet over Padded Cloth | Plate | T5 | 1 | 46 | 0 | 0 | 0 | 3.3 | Vanilla |
| `tv_vlandia_helmet_u3` | Western Banded Helmet With Faceguard Over Open Mail Coif | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3.2 | OSA |
| `ao_battanian_guarded_aristocrats_spangenhelmet` | Western Guarded Spangenhelm | Plate | T5 | auto | 45 | 0 | 0 | 0 | 1.8 | OSA |
| `mp_vlandian_faceguard_helmet_a` | Closed Knight Helmet | Plate | T5 | 1 | 45 | 0 | 0 | 0 | 2.2 | Vanilla |
| `tv_vlandia_lord_helmet_g` | Western Simple Plated Helmet | Plate | T5 | auto | 45 | 0 | 0 | 0 | 3.6 | OSA |
| `AR_vlandia_lord_helmet_c` | Western Noble Goggled Bandedhelm | Plate | T5 | auto | 44 | 0 | 0 | 0 | 2.2 | OSA |
| `AR_vlandian_knights_helmet_c` | Western Guarded Knight's Helmet | Plate | T5 | auto | 44 | 0 | 0 | 0 | 3.8 | OSA |
| `mp_vlandian_faceguard_helmet_b` | Knight Helmet with Bronze Faceguard | Plate | T5 | 1 | 44 | 0 | 0 | 0 | 2.3 | Vanilla |
| `TV_vlandia_helmet_p` | Western Pointed Skullcap With Closed Mail | Plate | T5 | auto | 44 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_vlandia_helmet_q` | Western Nasal Helmet With Closed Mail | Plate | T5 | auto | 44 | 0 | 0 | 0 | 2.8 | OSA |
| `tv_vlandia_lord_helmet_e` | Western Crested Noble Domed Cevelliere | Plate | T5 | auto | 44 | 0 | 0 | 0 | 3.8 | OSA |
| `TV_vlandia_royal_helmet_b` | Western Crowned Heavy Nasalhelm Over Mail Coif | Plate | T5 | auto | 44 | 0 | 0 | 0 | 3.2 | OSA |
| `AR_vlandia_helmet_z2` | Western Visored Ridge Helmet Over Mail | Plate | T5 | auto | 42 | 0 | 0 | 0 | 2.2 | OSA |
| `AR_vlandian_knights_helmet_a` | Western Knight's Helmet | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_vlandian_knights_helmet_b` | Western Feathered Knight's Helmet | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.7 | OSA |
| `mp_nasal_helmet_over_mail` | Nasal Helmet over Mail | Plate | T5 | 1 | 42 | 0 | 0 | 0 | 3.1 | Vanilla |
| `tv_vlandia_helmet_a4` | Western Painted Nasalhelm Over Open Mail Coif | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.2 | OSA |
| `tv_vlandia_helmet_b4` | Western Nasal Helmet Over Open Mail Coif | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.2 | OSA |
| `tv_vlandia_helmet_c4` | Western Conical Nasalhelm Over Open Mail Coif | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.2 | OSA |
| `tv_vlandia_helmet_i3` | Western Bent Conical Helmet Over Open Mail Coif | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.2 | OSA |
| `tv_vlandia_helmet_j` | Western Heavy Nasalhelm Over Open Mail Coif | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.2 | OSA |
| `tv_vlandia_helmet_k3` | Western Peaked Helmet Over Mail | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3 | OSA |
| `tv_vlandia_helmet_o3` | Western Banded Nasal Cevelliere Over Open Mail Coif | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.2 | OSA |
| `tv_vlandia_helmet_r3` | Western Banded Nasalhelm Over Open Mail Coif | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.2 | OSA |
| `tv_vlandia_helmet_t3` | Western Bent Conical Nasalhelm Over Mail | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3 | OSA |
| `AR_vlandia_helmet_y` | Western Plumed Goggled Bandedhelm Over Mail | Plate | T5 | auto | 41 | 0 | 0 | 0 | 2.2 | OSA |
| `RW_vlandia_lord_helmet_a` | Western Visored Helmet Over Mail | Plate | T5 | auto | 41 | 0 | 0 | 0 | 2.2 | OSA |
| `AR_vlandia_helmet_j` | Western Heavy Cevelliere Over Mail | Plate | T4 | auto | 40 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_vlandia_helmet_n` | Western Heavy Spangenhelm Over Mail | Plate | T4 | auto | 40 | 0 | 0 | 0 | 3.8 | OSA |
| `mp_peaked_helmet_over_mail` | Peaked Helmet over Mail | Plate | T4 | 1 | 40 | 0 | 0 | 0 | 2.8 | Vanilla |
| `tv_vlandia_helmet_u2` | Western Banded Helmet With Faceguard Over Padded Mail | Plate | T4 | auto | 40 | 0 | 0 | 0 | 3.1 | OSA |
| `AR_vlandia_helmet_i` | Nasal Helmet With Faceguard Over Mail | Plate | T4 | auto | 39 | 0 | 0 | 0 | 3.6 | OSA |
| `AR_vlandia_helmet_m` | Western Spangenhelm With Faceguard Over Mail | Plate | T4 | auto | 39 | 0 | 0 | 0 | 3.6 | OSA |
| `AR_vlandia_helmet_q` | Bronze Plumed Spangenhelm Over Mail Coif | Plate | T4 | auto | 39 | 0 | 0 | 0 | 3.6 | OSA |
| `AR_vlandia_helmet_q2` | Bronze Crested Spangenhelm Over Mail Coif | Plate | T4 | auto | 39 | 0 | 0 | 0 | 3.6 | OSA |
| `AR_vlandian_crown_mask_a` | Western Crowned Mask | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3.7 | OSA |
| `mp_guards_kettle_over_mail_coif` | Kettle over Mail Coif | Plate | T4 | 1 | 38 | 0 | 0 | 0 | 3.6 | Vanilla |
| `RW_vlandia_helmet_b` | Western Visored Helmet Over Stripped Padding | Plate | T4 | auto | 38 | 0 | 0 | 0 | 1.8 | OSA |
| `TV_vlandia_helmet_n` | Western Peaked Lancer's Helmet Over Mail | Plate | T4 | auto | 38 | 0 | 0 | 0 | 2.8 | OSA |
| `AR_vlandia_helmet_z` | Western Visored Ridge Helmet Over Cloth | Plate | T4 | auto | 37 | 0 | 0 | 0 | 1.8 | OSA |
| `arming_cap` | Padded Coif | Cloth | T4 | auto | 17 | 7 | 17 | 0 | 0.2 | RBM |
| `mp_peaked_helmet_over_laced_coif` | Peaked Helmet over Laced Coif | Plate | T4 | 1 | 37 | 0 | 0 | 0 | 2.8 | Vanilla |
| `tv_vlandia_helmet_k2` | Western Peaked Helmet Over Leather | Plate | T4 | auto | 37 | 0 | 0 | 0 | 2.8 | OSA |
| `tv_vlandia_helmet_t2` | Western Bent Conical Nasalhelm Over Leather | Plate | T4 | auto | 37 | 0 | 0 | 0 | 2.8 | OSA |
| `mp_kettle_hat_over_padded_coif` | Kettle Hat over Padded Coif | Plate | T4 | 1 | 36 | 0 | 0 | 0 | 2.6 | Vanilla |
| `mp_nasal_helmet_over_padded_coif` | Nasal Helmet over Padded Coif | Plate | T4 | 1 | 36 | 0 | 0 | 0 | 2.4 | Vanilla |
| `tv_vlandia_helmet_a3` | Western Painted Nasalhelm Over Mail | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.1 | OSA |
| `tv_vlandia_helmet_b3` | Western Nasal Helmet Over Mail | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.1 | OSA |
| `tv_vlandia_helmet_c3` | Western Conical Nasalhelm Over Mail | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.1 | OSA |
| `TV_vlandia_helmet_g` | Roundkettle Helmet Over Mail | Plate | T4 | auto | 36 | 0 | 0 | 0 | 2.9 | OSA |
| `tv_vlandia_helmet_i2` | Western Bent Conical Helmet Over Padded Mail | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.1 | OSA |
| `tv_vlandia_helmet_o2` | Western Banded Nasal Cevelliere Over Padded Mail | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.1 | OSA |
| `tv_vlandia_helmet_r2` | Western Banded Nasalhelm Over Padded Mail | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.1 | OSA |
| `DZ_vlandia_helmet_a` | Western Pointed Helmet With Leather | Plate | T4 | auto | 35 | 0 | 0 | 0 | 1.2 | OSA |
| `RW_vlandia_helmet_a` | Western Visored Helmet Over Stripped Cap | Plate | T4 | auto | 35 | 0 | 0 | 0 | 1.8 | OSA |
| `TV_vlandia_helmet_m` | Western Peaked Lancer's Helmet Over Leather | Plate | T4 | auto | 35 | 0 | 0 | 0 | 2.5 | OSA |
| `laced_coif` | Laced Coif | Cloth | T4 | auto | 19 | 0 | 19 | 0 | 0.4 | RBM |
| `AR_vlandia_helmet_e` | Decorated Heavy Guarded Skullcap Over Mail Coif | Plate | T3 | auto | 34 | 0 | 0 | 0 | 3.5 | OSA |
| `TV_vlandia_helmet_h` | Roundkettle Helmet Over Studded Leather | Plate | T3 | auto | 33 | 0 | 0 | 0 | 2.4 | OSA |
| `TV_vlandia_helmet_l` | Western Peaked Lancer's Helmet Over Cloth | Plate | T3 | auto | 33 | 0 | 0 | 0 | 2.8 | OSA |
| `AR_sturgia_helmet_n` | Cervelliere Over Open Mail | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_vlandia_helmet_d` | Decorated Heavy Skullcap Over Mail Coif | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.5 | OSA |
| `tv_vlandia_helmet_u` | Western Banded Helmet With Faceguard Over Padded Cloth | Plate | T3 | auto | 32 | 0 | 0 | 0 | 2.6 | OSA |
| `padded_leather_cape` | Padded Leather Cape | Leather | T3 | auto | 31 | 0 | 0 | 0 | 1 | RBM |
| `ao_vlandian_nasal_helmet_with_mail_coif` | Nasal Helmet With Mail Coif | Plate | T3 | auto | 30 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_vlandia_helmet_x` | Western Banded Nasalhelm with Faceguard | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.7 | OSA |
| `TV_vlandia_helmet_s` | Western Peaked Lancer's Helmet Over Cloth Cap | Plate | T3 | auto | 30 | 0 | 0 | 0 | 2.8 | OSA |
| `mp_kettle_helmet_over_mail` | Kettle Helmet over Mail | Plate | T3 | 1 | 29 | 0 | 0 | 0 | 3.1 | Vanilla |
| `mp_nasal_cervelliere_over_laced_coif` | Nasal Cervelliere over Laced Coif | Plate | T3 | 1 | 28 | 0 | 0 | 0 | 2.1 | Vanilla |
| `tv_vlandia_helmet_a2` | Western Painted Nasalhelm Over Padded Cloth | Plate | T3 | auto | 28 | 0 | 0 | 0 | 2.6 | OSA |
| `tv_vlandia_helmet_b2` | Western Nasal Helmet Over Padded Leather | Plate | T3 | auto | 28 | 0 | 0 | 0 | 2.6 | OSA |
| `tv_vlandia_helmet_c2` | Western Conical Nasalhelm Over Padded Cloth | Plate | T3 | auto | 28 | 0 | 0 | 0 | 2.6 | OSA |
| `tv_vlandia_helmet_i` | Western Bent Conical Helmet Over Padded Cloth | Plate | T3 | auto | 28 | 0 | 0 | 0 | 2.6 | OSA |
| `tv_vlandia_helmet_o` | Western Banded Nasal Cevelliere Over Padded Cloth | Plate | T3 | auto | 28 | 0 | 0 | 0 | 2.6 | OSA |
| `tv_vlandia_helmet_r` | Western Banded Nasalhelm Over Padded Cloth | Plate | T3 | auto | 28 | 0 | 0 | 0 | 2.6 | OSA |
| `ao_mailed_angevin_crown` | Western Ornate Spiked Crown Over Mail Coif | Chainmail | T2 | auto | 27 | 0 | 0 | 0 | 1.7 | OSA |
| `AR_vlandia_helmet_c2` | Feathered Heavy Skullcap Over Padded Cloth | Plate | T2 | auto | 27 | 0 | 0 | 0 | 1.1 | OSA |
| `AR_vlandia_helmet_c` | Decorated Heavy Skullcap Over Padded Cloth | Plate | T2 | auto | 26 | 0 | 0 | 0 | 1.1 | OSA |
| `cloth_headwrap` | Cloth Headwrap | Cloth | T2 | auto | 11 | 7 | 11 | 0 | 0.4 | RBM |
| `mp_segmented_cervelliere_over_mail` | Segmented Cervelliere over Mail | Plate | T2 | 1 | 26 | 0 | 0 | 0 | 2.85 | Vanilla |
| `vlandia_helmet_s_iii` | Western Flat Topped Helmet Over Mail | Plate | T2 | auto | 26 | 0 | 0 | 0 | 1.2 | OSA |
| `mp_segmented_cervelliere_over_laced_coif` | Segmented Cevelliere over Laced Coif | Plate | T2 | 1 | 24 | 0 | 0 | 0 | 2.3 | Vanilla |
| `vlandia_helmet_s_ii` | Western Flat Topped Helmet Over Leather | Plate | T2 | auto | 23 | 0 | 0 | 0 | 0.8 | OSA |
| `AR_vlandia_helmet_p` | Bronze Spangenhelm Over Padded Coif | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1.2 | OSA |
| `arming_coif` | Leather Coif | Cloth | T2 | auto | 22 | 0 | 0 | 0 | 0.3 | RBM |
| `mp_guards_kettle_over_padded_coif` | Kettle over Padded Coif | Plate | T2 | 1 | 22 | 0 | 0 | 0 | 1.5 | Vanilla |
| `mp_kettle_helmet_over_padded_coif` | Kettle Helmet over Padded Coif | Plate | T2 | 1 | 22 | 0 | 0 | 0 | 1.3 | Vanilla |
| `mp_nasal_cervelliere_over_padded_coif` | Nasal Cervelliere over Padded Coif | Plate | T2 | 1 | 22 | 0 | 0 | 0 | 1.2 | Vanilla |
| `AR_vlandia_helmet_w` | Western Open Banded Helmet With Faceguard | Plate | T2 | auto | 21 | 0 | 0 | 0 | 3.4 | OSA |
| `mp_nasal_cervelliere_over_mail_coif` | Nasal Cervelliere over Mail Coif | Plate | T2 | 1 | 21 | 0 | 0 | 0 | 2.6 | Vanilla |
| `open_padded_coif` | Open Padded Coif | Cloth | T2 | auto | 21 | 0 | 0 | 0 | 0.3 | RBM |
| `padded_coif` | Padded Half Coif | Cloth | T2 | auto | 16 | 5 | 0 | 0 | 0.5 | RBM |
| `AR_vlandia_helmet_f` | Decorated Heavy Skullcap Over Laced Coif | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.2 | OSA |
| `AR_vlandia_helmet_k` | Western Spangenhelm Over Laced Coif | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.2 | OSA |
| `AR_vlandia_helmet_o` | Bronze Spangenhelm Over Cloth Coif | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.1 | OSA |
| `mp_cervelliere_over_padded_cap` | Cervelliere over Padded Cap | Plate | T1 | 1 | 20 | 0 | 0 | 0 | 2.1 | Vanilla |
| `mp_kettle_helmet_over_padded_cap` | Kettle Helmet over Padded Cap | Plate | T1 | 1 | 20 | 0 | 0 | 0 | 1.2 | Vanilla |
| `mp_nasal_cervelliere_over_padded_cap` | Nasal Cervelliere over Padded Cap | Plate | T1 | 1 | 20 | 0 | 0 | 0 | 1 | Vanilla |
| `mp_nasal_helmet_over_cloth_headwrap` | Nasal Helmet over Cloth Headwrap | Plate | T1 | 1 | 20 | 0 | 0 | 0 | 1.1 | Vanilla |
| `mp_segmented_cervelliere_over_padded_cloth` | Segmented Cervelliere over Padded Cloth | Plate | T1 | 1 | 20 | 0 | 0 | 0 | 1.2 | Vanilla |
| `tv_vlandia_helmet_a` | Western Painted Nasalhelm | Plate | T1 | auto | 20 | 0 | 0 | 0 | 2.1 | OSA |
| `tv_vlandia_helmet_b` | Western Nasal Helmet | Plate | T1 | auto | 20 | 0 | 0 | 0 | 2.1 | OSA |
| `tv_vlandia_helmet_c` | Western Conical Nasalhelm | Plate | T1 | auto | 20 | 0 | 0 | 0 | 2.1 | OSA |
| `tv_vlandia_helmet_k` | Western Peaked Helmet | Plate | T1 | auto | 20 | 0 | 0 | 0 | 2.1 | OSA |
| `tv_vlandia_helmet_t` | Western Bent Conical Nasalhelm | Plate | T1 | auto | 20 | 0 | 0 | 0 | 2.1 | OSA |
| `TV_vlandia_royal_helmet_a` | Western Crown Over Mail Coif | Chainmail | T1 | auto | 20 | 0 | 0 | 0 | 1.7 | OSA |
| `mp_kettle_helmet_over_padded_cloth` | Kettle Helmet over Padded Cloth | Plate | T1 | 1 | 19 | 0 | 0 | 0 | 1.1 | Vanilla |
| `ao_angevin_crown` | Western Ornate Spiked Crown | Plate | T1 | auto | 18 | 0 | 0 | 0 | 3.1 | OSA |
| `AR_vlandia_helmet_h` | Heavy Skullcap With Faceguard | Plate | T1 | auto | 18 | 0 | 0 | 0 | 1.1 | OSA |
| `AR_vlandia_helmet_g` | Heavy Skullcap Over Leather | Plate | T1 | auto | 17 | 0 | 0 | 0 | 1.1 | OSA |
| `AR_vlandia_helmet_l` | Western Spangenhelm Over Leather | Plate | T1 | auto | 17 | 0 | 0 | 0 | 1.1 | OSA |
| `mp_cervelliere_over_arming_coif` | Cervelliere over Arming Coif | Plate | T1 | 1 | 16 | 0 | 0 | 0 | 0.9 | Vanilla |
| `padded_cap` | Padded Cap | Cloth | T1 | auto | 16 | 0 | 0 | 0 | 0.2 | RBM |
| `ao_crown_a` | Pointed Crown | Plate | T1 | auto | 15 | 0 | 0 | 0 | 3.1 | OSA |
| `crown_x` | Gilded Circlet With Tail | Plate | T1 | 32650 | 15 | 0 | 0 | 0 | 3.1 | OSA |
| `crown_z` | Studded Spiked Crown | Plate | T1 | auto | 15 | 0 | 0 | 0 | 3.1 | OSA |
| `mp_padded_leather_cape` | Padded Leather Cap | Plate | T1 | 1 | 15 | 0 | 0 | 0 | 1 | Vanilla |
| `mp_open_padded_coif` | Open Padded Coif | Cloth | T1 | 1 | 14 | 0 | 0 | 0 | 0.6 | Vanilla |
| `mp_padded_coif` | Padded Half Coif | Cloth | T1 | 1 | 14 | 0 | 0 | 0 | 0.7 | Vanilla |
| `mp_arming_coif` | Leather Coif | Cloth | T0 | 1 | 11 | 0 | 0 | 0 | 0.3 | Vanilla |
| `ao_gold_circlet_a` | Simple Gilded Circlet | Plate | T0 | 8650 | 10 | 0 | 0 | 0 | 3.1 | OSA |
| `head_piece` | Female Rural Scarf | Cloth | T0 | auto | 10 | 0 | 0 | 0 | 0.2 | RBM |
| `headscarf` | Loose Head Scarf | Cloth | T0 | auto | 10 | 0 | 0 | 0 | 0.5 | RBM |
| `head_wrapped` | Female Rural Headwrap | Cloth | T0 | auto | 9 | 0 | 0 | 0 | 0.3 | RBM |
| `head_wrapping` | Peasant Head Wrapping | Cloth | T0 | auto | 9 | 0 | 0 | 0 | 0.05 | RBM |
| `headscarf_c` | Luxurious Head Scarf | Cloth | T0 | auto | 9 | 0 | 0 | 0 | 0.3 | RBM |
| `green_hat` | Cloth Hat | Cloth | T-1 | auto | 6 | 0 | 0 | 0 | 0.2 | OSA |
| `mp_cloth_headwrap` | Cloth Headwrap | Cloth | T-1 | 1 | 6 | 0 | 0 | 0 | 0.4 | Vanilla |
| `womens_headwrap_c` | Rural Head Wrap | Cloth | T-1 | auto | 6 | 0 | 0 | 0 | 0.1 | RBM |

<!-- CULTURE:sturgia COUNT:142 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `sturgian_lord_helmet_c` | Plated Warlord Helmet | Plate | T5 | auto | 150 | 95 | 50 | 0 | 4.4 | RBM |
| `decorated_goggled_helmet` | Reinforced Closed Goggled Helmet | Plate | T5 | auto | 133 | 82 | 45 | 0 | 4.2 | RBM |
| `lendman_helmet_over_full_mail` | Closed Warlord Helmet over Full Mail | Plate | T5 | auto | 127 | 84 | 45 | 0 | 4.3 | RBM |
| `sturgian_helmet_closed` | Closed Warlord Helmet | Plate | T5 | auto | 118 | 82 | 30 | 0 | 1.8 | RBM |
| `sturgian_helmet_b_close` | Closed Decorated Goggled Helmet | Plate | T5 | auto | 107 | 82 | 30 | 0 | 3 | RBM |
| `sturgian_lord_helmet_b` | Warlord Helmet with Nose Guard | Plate | T5 | auto | 115 | 36 | 20 | 0 | 3.4 | RBM |
| `northern_warlord_helmet` | Warlord Helmet with Cheek Guards | Plate | T5 | auto | 113 | 36 | 12 | 0 | 3.6 | RBM |
| `sturgian_lord_helmet_a` | Ridged Warlord Helmet | Plate | T5 | auto | 116 | 24 | 20 | 0 | 3.6 | RBM |
| `sturgia_heavy_cavalary_helmet` | Heavy Cavalry Helmet | Plate | T5 | auto | 120 | 12 | 20 | 0 | 2.8 | RBM |
| `sturgian_battle_crown` | Battle Crown | Plate | T5 | auto | 125 | 12 | 0 | 0 | 2.2 | RBM |
| `sturgian_helmet_b_open` | Plumed Decorated Mailed Nasal Helmet | Plate | T5 | auto | 98 | 14 | 0 | 0 | 1.8 | RBM |
| `sturgian_helmet_open` | Mailed Nasal Helmet | Plate | T5 | auto | 85 | 12 | 17 | 0 | 1.8 | RBM |
| `goggled_helmet_over_leather` | Goggled Helmet over Leather | Plate | T5 | auto | 87 | 16 | 0 | 0 | 1.4 | RBM |
| `nasalhelm_over_mail` | Nasalhelm over Mail | Plate | T5 | auto | 75 | 12 | 17 | 0 | 1.8 | RBM |
| `nasal_helmet_with_leather` | Nasal Helmet with Leather | Plate | T5 | auto | 63 | 12 | 25 | 0 | 1.2 | RBM |
| `sturgian_helmet_base` | Steel Nasal Cap | Plate | T5 | auto | 83 | 12 | 0 | 0 | 1.8 | RBM |
| `nasal_helmet_with_mail` | Nasal Helmet with Mail | Plate | T5 | auto | 65 | 12 | 20 | 0 | 1.5 | RBM |
| `spangenhelm_with_leather` | Spangenhelm With Leather | Plate | T5 | auto | 67 | 0 | 20 | 0 | 1.2 | RBM |
| `spangenhelm_with_padded_cloth` | Spangenhelm with Padded Cloth | Plate | T5 | auto | 61 | 3 | 15 | 0 | 1.3 | RBM |
| `nasalhelm_over_leather` | Nasalhelm over Leather | Plate | T5 | auto | 65 | 12 | 0 | 0 | 1.4 | RBM |
| `mp_sturgian_helmet_b_close` | Closed Decorated Goggled Helmet | Plate | T5 | 1 | 54 | 0 | 0 | 0 | 3.8 | Vanilla |
| `TV_sturgia_helmet_a` | Northern Closed Spangenhelm | Plate | T5 | auto | 54 | 0 | 0 | 0 | 3.8 | OSA |
| `TV_sturgia_helmet_b` | Northern Tailed Closed Spangenhelm | Plate | T5 | auto | 54 | 0 | 0 | 0 | 3.8 | OSA |
| `ao_sturgian_feathered_helmet_closed` | Northern Feathered Closed Nasal Helmet | Plate | T5 | auto | 53 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_sturgian_feathered_helmet_with_closed_mail` | Northern Feathered Nasal Helmet With Closed Mail | Plate | T5 | auto | 53 | 0 | 0 | 0 | 1.8 | OSA |
| `mp_sturgian_helmet_closed` | Closed Warlord Helmet | Plate | T5 | 1 | 53 | 0 | 0 | 0 | 3.8 | Vanilla |
| `TV_sturgia_helmet_d` | Northern Blackened Steel Closed Helmet | Plate | T5 | auto | 53 | 0 | 0 | 0 | 3.8 | OSA |
| `TV_sturgia_helmet_g2` | Northern Gilded Steel Closed Nasal Helmet | Plate | T5 | auto | 53 | 0 | 0 | 0 | 3.8 | OSA |
| `DZ_sturgia_helmet_d` | Northern Feathered Domed Helmet Over Mail | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.2 | OSA |
| `AR_vaegir_helmet_g` | Northern Spiked Helmet With Decorated Facemask | Plate | T5 | auto | 51 | 0 | 0 | 0 | 4.5 | OSA |
| `TV_nord_helmet_d` | Nordic Vendel Cap Over Closed Mail | Chainmail | T5 | auto | 51 | 0 | 0 | 0 | 4.4 | OSA |
| `TV_nord_helmet_h` | Nordic Vendel Goggled Helm Over Closed Mail | Chainmail | T5 | auto | 51 | 0 | 0 | 0 | 4.4 | OSA |
| `TV_nord_helmet_p` | Northern Lendman's Cap Over Closed Mail | Chainmail | T5 | auto | 51 | 0 | 0 | 0 | 4.4 | OSA |
| `TV_sturgia_helmet_f` | Northern Closed Pointed Helmet | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.8 | OSA |
| `tv_vlandia_helmet_f` | Northern Nasal Helmet With Closed Mail | Plate | T5 | auto | 51 | 0 | 0 | 0 | 4 | OSA |
| `sturgian_crown` | Jeweled Fur Rimmed Helmet | Plate | T5 | auto | 50 | 0 | 0 | 0 | 2.4 | RBM |
| `TV_nord_helmet_c` | Nordic Vendel Cap Over Open Mail | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.4 | OSA |
| `TV_nord_helmet_g` | Nordic Vendel Goggled Helm Over Open Mail | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.4 | OSA |
| `TV_nord_helmet_o` | Northern Lendman's Cap Over Open Mail | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.4 | OSA |
| `ao_sturgian_feathered_helmet` | Northern Feathered Nasal Helmet | Plate | T5 | auto | 49 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_sturgian_feathered_helmet_with_mail` | Northern Feathered Nasal Helmet With Full Mail | Plate | T5 | auto | 49 | 0 | 0 | 0 | 1.8 | OSA |
| `mp_sturgian_helmet_b_open` | Plumed Decorated Mailed Nasal Helmet | Plate | T5 | 1 | 49 | 0 | 0 | 0 | 3.2 | Vanilla |
| `mp_sturgian_helmet_open` | Mailed Nasal Helmet | Plate | T5 | 1 | 49 | 0 | 0 | 0 | 3.15 | Vanilla |
| `TV_sturgia_helmet_c` | Northern Plumed Open Helmet | Plate | T5 | auto | 49 | 0 | 0 | 0 | 3.2 | OSA |
| `TV_sturgia_helmet_e` | Northern Blackened Steel Open Helmet | Plate | T5 | auto | 49 | 0 | 0 | 0 | 3.15 | OSA |
| `TV_sturgia_helmet_g` | Northern Gilded Steel Nasal Helmet | Plate | T5 | auto | 49 | 0 | 0 | 0 | 3.15 | OSA |
| `TV_sturgia_helmet_h` | Northern Gilded Steel Helmet | Plate | T5 | auto | 49 | 0 | 0 | 0 | 3.15 | OSA |
| `AR_sturgia_helmet_c` | Northern Plumed Closed Cavalry Helmet | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.4 | OSA |
| `berserkr_helm_a` | Berserker's Helm | Plate | T5 | auto | 48 | 0 | 0 | 0 | 4.8 | OSA |
| `TV_nord_helmet_s` | Nordic Goggled Nasal Helmet With Faceguard Over Mail | Plate | T5 | auto | 48 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_sturgia_lord_helmet_b2` | Northern Noble Mailed Steel Nasal Helmet | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.15 | OSA |
| `TV_sturgia_lord_helmet_c2` | Northern Noble Mailed Gilded Nasal Helmet | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.15 | OSA |
| `ulfhednar_helm` | Ulfhednar's Goggled Helmet | Plate | T5 | auto | 48 | 0 | 0 | 0 | 4.8 | OSA |
| `TV_nord_helmet_k` | Nordic Vendel Nasalhelm With Faceguard | Plate | T5 | auto | 47 | 0 | 0 | 0 | 3.6 | OSA |
| `TV_nord_helmet_l` | Nordic Vendel Nasalhelm With Goggled Faceguard | Plate | T5 | auto | 47 | 0 | 0 | 0 | 3.6 | OSA |
| `TV_sturgia_lord_helmet_a` | Northern Blackened Steel Battle Crown | Plate | T5 | auto | 47 | 0 | 0 | 0 | 2.2 | OSA |
| `TV_sturgia_lord_helmet_d` | Northern Mailed Gilded Spangenhelm With Feathers | Plate | T5 | auto | 47 | 0 | 0 | 0 | 3.15 | OSA |
| `TV_sturgia_lord_helmet_e` | Northern Mailed Gilded Spangenhelm With Plume | Plate | T5 | auto | 47 | 0 | 0 | 0 | 3.15 | OSA |
| `DZ_sturgia_helmet_c` | Northern Domed Helmet Over Padded Mail | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3.5 | OSA |
| `TV_nord_helmet_b` | Nordic Vendel Cap Over Leather | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3.1 | OSA |
| `TV_nord_helmet_f` | Nordic Vendel Goggled Helm Over Leather | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3.1 | OSA |
| `TV_nord_helmet_n` | Northern Lendman's Cap Over Leather | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3.1 | OSA |
| `ao_eorling_guarded_nasal_helmet_with_closed_mail` | Northern Plumed Guarded Nasal Helmet With Closed Mail | Plate | T5 | auto | 45 | 0 | 0 | 0 | 3.4 | OSA |
| `TV_sturgia_lord_helmet_b` | Northern Noble Mailed Steel Helmet | Plate | T5 | auto | 45 | 0 | 0 | 0 | 3.15 | OSA |
| `TV_sturgia_lord_helmet_c` | Northern Noble Mailed Gilded Helmet | Plate | T5 | auto | 45 | 0 | 0 | 0 | 3.15 | OSA |
| `TV_sturgia_helmet_k2` | Northern Mailed Gilded Spangenhelm | Plate | T5 | auto | 44 | 0 | 0 | 0 | 2.9 | OSA |
| `AR_vaegir_helmet_e` | Northern Spiked Helmet Over Leather | Plate | T5 | auto | 42 | 0 | 0 | 0 | 4 | OSA |
| `AR_vaegir_helmet_f` | Northern Fur Spiked Helmet Over Leather | Plate | T5 | auto | 42 | 0 | 0 | 0 | 4 | OSA |
| `TV_nord_helmet_r` | Nordic Nasal Helmet With Faceguard Over Mail | Plate | T5 | auto | 42 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_nord_helmet_u3` | Nordic Pointed Nasalhelm With Faceguard Over Mail | Plate | T5 | auto | 42 | 0 | 0 | 0 | 2.9 | OSA |
| `vaegir_helmet_closed` | Varyag Closed Helmet | Plate | T5 | auto | 42 | 0 | 0 | 0 | 2.2 | OSA |
| `battanian_decorated_spangenhelm` | Decorated Nasalhelm With Mail | Plate | T5 | auto | 41 | 0 | 0 | 0 | 3.6 | OSA |
| `mailed_cavalry_helm` | Northern Closed Cavalry Helmet | Plate | T4 | auto | 40 | 0 | 0 | 0 | 3 | OSA |
| `TV_nord_helmet_a` | Nordic Vendel Cap | Plate | T4 | auto | 40 | 0 | 0 | 0 | 3.1 | OSA |
| `TV_nord_helmet_e` | Nordic Vendel Goggled Helm | Plate | T4 | auto | 40 | 0 | 0 | 0 | 3.1 | OSA |
| `TV_nord_helmet_m` | Northern Lendman's Cap | Plate | T4 | auto | 40 | 0 | 0 | 0 | 3.1 | OSA |
| `ao_sturgian_spiked_nasal_helmet` | Northern Simple Nasal Helmet With Full Mail | Plate | T4 | auto | 38 | 0 | 0 | 0 | 1.8 | OSA |
| `DZ_sturgia_helmet_a` | Northern Plumed Visored Cap Over Closed Mail | Plate | T4 | auto | 38 | 0 | 0 | 0 | 4.1 | OSA |
| `TV_nord_helmet_v3` | Nordic Boar Nasal Helmet With Mail | Plate | T4 | auto | 38 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_nord_helmet_w3` | Nordic Heavy Guarded Nasal Helmet With Mail | Plate | T4 | auto | 38 | 0 | 0 | 0 | 2.9 | OSA |
| `mp_sturgian_lord_helmet_a` | Northlander Lord Helmet | Plate | T4 | 1 | 37 | 0 | 0 | 0 | 3.6 | Vanilla |
| `vaegir_helmet_open` | Varyag Open Helmet | Plate | T4 | auto | 37 | 0 | 0 | 0 | 2 | OSA |
| `ao_eorling_guarded_nasal_helmet_with_mail` | Northern Guarded Nasal Helmet With Mail | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.4 | OSA |
| `DZ_sturgia_helmet_b` | Northern Domed Helmet Over Padded Cloth | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.2 | OSA |
| `TV_nord_helmet_q` | Nordic Nasal Helmet With Faceguard Over Leather | Plate | T4 | auto | 36 | 0 | 0 | 0 | 2.9 | OSA |
| `mp_sturgian_helmet_base` | Steel Nasal Cap | Plate | T4 | 1 | 35 | 0 | 0 | 0 | 2.4 | Vanilla |
| `TV_nord_helmet_t3` | Nordic Nasal Ridged Cap Over Mail | Plate | T4 | auto | 35 | 0 | 0 | 0 | 2.6 | OSA |
| `TV_nord_helmet_u2` | Nordic Pointed Nasalhelm Over Mail | Plate | T4 | auto | 35 | 0 | 0 | 0 | 2.6 | OSA |
| `TV_nord_helmet_v2` | Nordic Boar Nasal Helmet With Leather | Plate | T4 | auto | 35 | 0 | 0 | 0 | 2.6 | OSA |
| `TV_nord_helmet_w2` | Nordic Heavy Guarded Nasal Helmet With Padding | Plate | T4 | auto | 35 | 0 | 0 | 0 | 2.6 | OSA |
| `AR_sturgia_helmet_m` | Northern Pointed Helmet With Feather | Plate | T3 | auto | 34 | 0 | 0 | 0 | 4.5 | OSA |
| `TV_sturgia_helmet_k` | Northern Gilded Spangenhelm | Plate | T3 | auto | 34 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_nord_helmet_j` | Nordic Vendel Nasalhelm Over Open Mail | Plate | T3 | auto | 33 | 0 | 0 | 0 | 3.4 | OSA |
| `AR_sturgia_helmet_e` | Spangenhelm Over Mail | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_sturgia_helmet_j` | Furred Spangenhelm Over Mail | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_sturgia_helmet_j2` | Brown Furred Spangenhelm Over Mail | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_sturgia_helmet_l2` | Northern Helmet With Mail | Plate | T3 | auto | 32 | 0 | 0 | 0 | 1.8 | OSA |
| `mp_northern_warlord_helmet` | Sturgian Warlord Helmet | Plate | T3 | 1 | 32 | 0 | 0 | 0 | 3.6 | Vanilla |
| `ulfhednar_helm_b` | Ulfhednar's Helmet | Plate | T3 | auto | 32 | 0 | 0 | 0 | 4.8 | OSA |
| `hmj_forpolkka_sturgia_gopniki_helm` | Northern Helmet With Fur Hat | Plate | T3 | auto | 31 | 0 | 0 | 0 | 1.9 | OSA |
| `TV_nord_helmet_t2` | Nordic Ridged Cap Over Mail | Plate | T3 | auto | 31 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_nord_helmet_u` | Nordic Pointed Nasalhelm Over Leather | Plate | T3 | auto | 31 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_nord_helmet_v` | Nordic Boar Nasal Helmet | Plate | T3 | auto | 31 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_nord_helmet_w` | Nordic Heavy Guarded Nasal Helmet | Plate | T3 | auto | 31 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_sturgia_helmet_i` | Northern Blackened Steel Helmet | Plate | T3 | auto | 31 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_sturgia_helmet_j` | Northern Steel Helmet | Plate | T3 | auto | 31 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_sturgia_helmet_j2` | Northern Gilded Helmet | Plate | T3 | auto | 31 | 0 | 0 | 0 | 2.9 | OSA |
| `ao_eorling_guarded_nasal_helmet_with_leather` | Northern Guarded Nasal Helmet With Leather | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.4 | OSA |
| `ao_sturgian_helmet_with_leather` | Northern Nasal Helmet With Leather | Plate | T3 | auto | 30 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_sturgia_helmet_l` | Northern Helmet With Leather | Plate | T3 | auto | 30 | 0 | 0 | 0 | 1.8 | OSA |
| `mp_sturgia_heavy_cavalry_helmet` | Sturgia Heavy Cavalry Helmet | Plate | T3 | 1 | 30 | 0 | 0 | 0 | 1.8 | Vanilla |
| `mp_nasalhelm_over_mail` | Nasalhelm over Mail | Plate | T3 | 1 | 29 | 0 | 0 | 0 | 1.8 | Vanilla |
| `ao_sturgian_spangenhelm_with_leather` | Northern Spangenhelm With Leather | Plate | T2 | auto | 27 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_sturgia_helmet_b` | Northern Plumed Heavy Cavalry Helmet | Plate | T2 | auto | 27 | 0 | 0 | 0 | 2.9 | OSA |
| `AR_sturgia_helmet_d` | Northern Feathered Nasal Helmet | Plate | T2 | auto | 27 | 0 | 0 | 0 | 2.9 | OSA |
| `AR_sturgia_helmet_a` | Northern Nasalhelm Over Cloth | Plate | T2 | auto | 26 | 0 | 0 | 0 | 1.4 | OSA |
| `AR_sturgia_helmet_k` | Northern Cap Helmet Over Leather | Plate | T2 | auto | 23 | 0 | 0 | 0 | 1.9 | OSA |
| `TV_nord_helmet_i` | Nordic Vendel Nasalhelm Over Leather | Plate | T2 | auto | 23 | 0 | 0 | 0 | 3.1 | OSA |
| `AR_sturgia_helmet_base` | Northern Cavalry Helmet | Plate | T2 | auto | 22 | 0 | 0 | 0 | 2.6 | OSA |
| `AR_sturgia_helmet_i` | Furred Spangenhelm With Leather | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1.2 | OSA |
| `AR_sturgia_helmet_i2` | Brown Furred Spangenhelm With Leather | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1.2 | OSA |
| `sloven_helmet` | Eastern Conical Helmet | Plate | T2 | auto | 22 | 0 | 0 | 0 | 0.4 | OSA |
| `AR_sturgia_helmet_h` | Furred Spangenhelm with Padded Cloth | Plate | T2 | auto | 21 | 0 | 0 | 0 | 1.9 | OSA |
| `AR_sturgia_helmet_h2` | Brown Furred Spangenhelm with Padded Cloth | Plate | T2 | auto | 21 | 0 | 0 | 0 | 1.9 | OSA |
| `roughhide_cap` | Roughhide Cap | Cloth | T2 | auto | 21 | 0 | 0 | 0 | 0.5 | RBM |
| `AR_sturgia_helmet_base_b` | Northern Light Cavalry Helmet | Plate | T1 | auto | 20 | 0 | 0 | 0 | 2.6 | OSA |
| `AR_vaegir_helmet_b` | Varyag Nasalhelm With Fur | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_vaegir_helmet_b2` | Northern Nasalhelm With Fur | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.8 | OSA |
| `mp_spangenhelm_with_padded_cloth` | Spangenhelm with Padded Cloth | Plate | T1 | 1 | 20 | 0 | 0 | 0 | 1.9 | Vanilla |
| `sturgian_nasal_helmet_fur` | Northern Nasalhelm with Hat | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.8 | OSA |
| `TV_nord_helmet_t` | Nordic Ridged Cap Over Cloth | Plate | T1 | auto | 20 | 0 | 0 | 0 | 2.2 | OSA |
| `AR_sturgia_helmet_g` | Furred Spangenhelm Cap | Plate | T1 | auto | 19 | 0 | 0 | 0 | 3.2 | OSA |
| `AR_sturgia_helmet_g2` | Brown Furred Spangenhelm Cap | Plate | T1 | auto | 19 | 0 | 0 | 0 | 3.2 | OSA |
| `northern_fur_cap` | Fur Cap | Leather | T1 | auto | 19 | 0 | 0 | 0 | 0.5 | RBM |
| `AR_sturgia_helmet_f` | Spangenhelm Cap | Plate | T1 | auto | 18 | 0 | 0 | 0 | 3.2 | OSA |
| `nordic_fur_cap` | Fur Rimmed Cap | Leather | T1 | auto | 18 | 0 | 0 | 0 | 0.3 | RBM |
| `nordic_leather_cap` | Fur Cap with Ear Flaps | Leather | T1 | auto | 14 | 0 | 0 | 0 | 0.4 | RBM |
| `mp_nordic_fur_cap` | Fur Rimmed Cap | Leather | T0 | 1 | 10 | 0 | 0 | 0 | 0.3 | Vanilla |
| `sloven_hat` | Sloven Hat | Cloth | T0 | auto | 8 | 0 | 0 | 0 | 0.4 | OSA |
| `sloven_hat_a` | Rich Sloven Hat | Cloth | T0 | auto | 8 | 0 | 0 | 0 | 0.5 | OSA |
| `AR_sturgia_womens_hat_a` | Northern Women's Hat | Leather | T-1 | auto | 6 | 0 | 0 | 0 | 0.4 | OSA |
| `womens_headwrap_b` | Head Scarf | Cloth | T-1 | auto | 6 | 0 | 0 | 0 | 0.2 | RBM |

<!-- CULTURE:battania COUNT:194 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `battanian_crowned_helmet` | Highland Crowned Helmet | Plate | T5 | auto | 121 | 16 | 15 | 0 | 3.2 | RBM |
| `battanian_noble_helmet_with_feather` | Warlord Helmet | Plate | T5 | auto | 102 | 12 | 25 | 0 | 3.1 | RBM |
| `battanian_plated_noble_helmet` | Warlord Helmet with Feathers | Plate | T5 | auto | 95 | 16 | 15 | 0 | 2.9 | RBM |
| `roughscale_helmet` | Roughscale Helmet | Plate | T5 | auto | 88 | 0 | 20 | 0 | 3.2 | RBM |
| `ridged_northernhelm` | Ridged Helm | Plate | T5 | auto | 94 | 12 | 0 | 0 | 3.4 | RBM |
| `battania_earmuff_helmet_d` | Ridged Decorated Steel Cap With Cheek Guards | Plate | T5 | auto | 92 | 12 | 0 | 0 | 3.2 | RBM |
| `battania_earmuff_helmet_c` | Fringed Decorated Steel Cap With Cheek Guards | Plate | T5 | auto | 89 | 0 | 0 | 0 | 3.1 | RBM |
| `battania_earmuff_helmet_a` | Fringed Steel Cap With Cheek Guards | Plate | T5 | auto | 79 | 0 | 0 | 0 | 3 | RBM |
| `battania_earmuff_helmet_b` | Steel Cap With Cheek Guards | Plate | T5 | auto | 74 | 0 | 0 | 0 | 2.9 | RBM |
| `battania_earmuff_helmet_c_brnz` | Fringed Decorated Bronze Cap With Cheek Guards | Plate | T5 | auto | 73 | 0 | 0 | 0 | 3.3 | RBM |
| `battania_earmuff_helmet_d_brnz` | Ridged Decorated Bronze Cap With Cheek Guards | Plate | T5 | auto | 71 | 0 | 0 | 0 | 3.3 | RBM |
| `battania_battle_crown` | Two Pronged Bronze Crown | Plate | T5 | auto | 66 | 0 | 0 | 0 | 2.3 | RBM |
| `battania_earmuff_helmet_a_brnz` | Fringed Bronze Cap With Cheek Guards | Plate | T5 | auto | 65 | 0 | 0 | 0 | 3.2 | RBM |
| `battania_earmuff_helmet_c_cpr` | Decorated Bronze Helmet with Cheek Guards | Plate | T5 | auto | 63 | 0 | 0 | 0 | 3.3 | RBM |
| `celtic_helmet` | Old Guard Helmet | Plate | T5 | auto | 63 | 0 | 0 | 0 | 2.5 | RBM |
| `battania_earmuff_helmet_b_brnz` | Bronze Cap With Cheek Guards | Plate | T5 | auto | 61 | 0 | 0 | 0 | 3 | RBM |
| `TV_battania_helmet_j` | Highland Feathered Heavy Bronze Nasalhelm | Plate | T5 | auto | 54 | 0 | 0 | 0 | 3.6 | OSA |
| `TV_battania_helmet_k` | Highland Heavy Bronze Nasalhelm | Plate | T5 | auto | 54 | 0 | 0 | 0 | 3.6 | OSA |
| `AR_battania_lord_helmet_a` | Plumed Highland Crowned Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.3 | OSA |
| `AR_battania_lord_helmet_b` | Gilded Plumed Highland Crowned Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.4 | OSA |
| `AR_battania_lord_helmet_f` | Highland Noble Steel Ridged Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.3 | OSA |
| `AR_battania_lord_helmet_g` | Highland Noble Bronze Ridged Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.4 | OSA |
| `AR_battania_lord_helmet_h` | Highland Noble Steel Tall Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.3 | OSA |
| `AR_battania_lord_helmet_i` | Highland Noble Bronze Tall Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.4 | OSA |
| `AR_battania_lord_helmet_p` | Highland Gilded Noble Heavy Nasalhelm Over Scale | Plate | T5 | auto | 52 | 0 | 0 | 0 | 4.2 | OSA |
| `AR_battania_lord_helmet_q` | Highland Noble Heavy Nasalhelm Over Scale | Plate | T5 | auto | 52 | 0 | 0 | 0 | 4.2 | OSA |
| `TV_battania_helmet_h` | Highland Plumed Heavy Bronze Spangenhelm | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.6 | OSA |
| `TV_battania_helmet_i` | Highland Feathered Heavy Bronze Spangenhelm | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.6 | OSA |
| `AR_battania_helmet_zf` | Highland Heavy Nasalhelm Over Mail | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.6 | OSA |
| `AR_battania_lord_helmet_j` | Highland Noble Helmet With Feather Crest | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.1 | OSA |
| `AR_battania_lord_helmet_l` | Highland Bronze Ridge Helmet With Feather Crest | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.1 | OSA |
| `AR_battania_lord_helmet_m` | Highland Ridge Helmet With Feather Crest | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.1 | OSA |
| `AR_battania_lord_helmet_n` | Highland Warlord's Iron Crested Ridge Helmet | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.3 | OSA |
| `AR_battania_lord_helmet_o` | Highland Warlord's Gilded Crested Ridge Helmet | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.3 | OSA |
| `AR_battania_helmet_i` | Highland Plumed Spangenhelm Over Mail | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_battania_lord_helmet_c` | Plumed Highland Noble Helmet | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.1 | OSA |
| `AR_battania_lord_helmet_d` | Highland Noble Steel Decorated Helmet With Plume | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.1 | OSA |
| `AR_battania_lord_helmet_e` | Highland Noble Bronze Decorated Helmet With Plume | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.1 | OSA |
| `AR_battania_lord_helmet_k` | Highland Noble Steel Decorated Helmet With Bird | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.1 | OSA |
| `TV_battania_helmet_g` | Highland Heavy Bronze Spangenhelm | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.5 | OSA |
| `AR_battania_helmet_ze` | Highland Heavy Nasalhelm over Leather | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.4 | OSA |
| `AR_phrygian_helmet_b` | Dryatic Steel Decorated Crested Bent Conical Helmet | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.1 | OSA |
| `AR_phrygian_helmet_b2` | Dryatic Brass Decorated Crested Bent Conical Helmet | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.1 | OSA |
| `bearhelmet` | Bearskin Over Highland Helmet | Plate | T5 | auto | 48 | 0 | 0 | 0 | 4.5 | OSA |
| `TV_pict_helmet_o` | Highland Crested Decorated Banded Helmet | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.1 | OSA |
| `ao_eorling_guardian_helmet` | Highland Heavy Guarded Helmet | Plate | T5 | auto | 47 | 0 | 0 | 0 | 3.4 | OSA |
| `AR_battania_helmet_zd` | Highland Heavy Nasalhelm Over Cloth | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3.1 | OSA |
| `AR_goth_helmet_c` | Plumed Iron Roughscale Helmet With Faceguard | Plate | T5 | auto | 46 | 0 | 0 | 0 | 2.2 | OSA |
| `ao_battanian_crowned_helmet` | Highland Crowned Plated Noble Nasalhelm With Plume | Plate | T5 | auto | 45 | 0 | 0 | 0 | 1.8 | OSA |
| `wolfhelmet` | Wolfskin Over Highland Helmet | Plate | T5 | auto | 45 | 0 | 0 | 0 | 2 | OSA |
| `ao_battanian_guarded_aristocrats_helmet` | Highland Plated Noble Nasalhelm | Plate | T5 | auto | 42 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_eorling_heavy_guarded_noblemans_helmet_b` | Highland Decorated Plumed Guarded Nobleman's Helmet | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.4 | OSA |
| `ao_eorling_heavy_guarded_noblemans_helmet_c` | Highland Decorated Guarded Nobleman's Helmet | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.4 | OSA |
| `AR_battania_helmet_h` | Highland Feathered Spangenhelm Over Suede | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.5 | OSA |
| `AR_phrygian_helmet_a` | Dryatic Steel Decorated Bent Conical Helmet | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.1 | OSA |
| `AR_phrygian_helmet_a2` | Dryatic Brass Decorated Bent Conical Helmet | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.1 | OSA |
| `mp_battanian_crowned_helmet` | Highland Crowned Helmet | Plate | T5 | 1 | 42 | 0 | 0 | 0 | 3.2 | Vanilla |
| `TV_battania_helmet_n` | Highland Horned Steel Helmet With Plume | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.5 | OSA |
| `TV_battania_helmet_o` | Highland Horned Bronze Helmet With Plume | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.5 | OSA |
| `TV_pict_helmet_n` | Highland Decorated Banded Helmet | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.1 | OSA |
| `AR_battania_helmet_z` | Highland Banded Helmet Over Mail | Plate | T5 | auto | 41 | 0 | 0 | 0 | 2.2 | OSA |
| `mp_battania_earmuff_helmet_d` | Ridged Decorated Steel Cap with Cheek Guards | Plate | T5 | 1 | 41 | 0 | 0 | 0 | 3.2 | Vanilla |
| `TV_battania_helmet_p` | Highland Ridged Decorated Steel Nasalhelm | Plate | T5 | auto | 41 | 0 | 0 | 0 | 3.2 | OSA |
| `TV_battania_helmet_q` | Highland Ridged Decorated Bronze Nasalhelm | Plate | T5 | auto | 41 | 0 | 0 | 0 | 3.2 | OSA |
| `TV_battania_helmet_u` | Highland Ridged Decorated Steel Helmet | Plate | T5 | auto | 41 | 0 | 0 | 0 | 3.2 | OSA |
| `TV_battania_helmet_u2` | Highland Ridged Decorated Bronze Helmet | Plate | T5 | auto | 41 | 0 | 0 | 0 | 3.2 | OSA |
| `ao_eorling_heavy_guarded_noblemans_helmet` | Highland Heavy Plumed Guarded Nobleman's Helmet | Plate | T4 | auto | 40 | 0 | 0 | 0 | 3.4 | OSA |
| `ao_eorling_heavy_guarded_noblemans_helmet_a` | Highland Heavy Guarded Nobleman's Helmet | Plate | T4 | auto | 40 | 0 | 0 | 0 | 3.4 | OSA |
| `mp_battania_earmuff_helmet_d_brnz` | Ridged Decorated Bronze Cap with Cheek Guards | Plate | T4 | 1 | 40 | 0 | 0 | 0 | 3.3 | Vanilla |
| `TV_battania_helmet_a` | Highland Pointed Steel Kettle With Fringes | Plate | T4 | auto | 40 | 0 | 0 | 0 | 3.5 | OSA |
| `TV_battania_helmet_b` | Highland Pointed Bronze Kettle With Fringes | Plate | T4 | auto | 40 | 0 | 0 | 0 | 3.5 | OSA |
| `TV_battania_lord_helmet_a` | Iron Highland Crown | Plate | T4 | auto | 40 | 0 | 0 | 0 | 2.3 | OSA |
| `TV_battania_lord_helmet_b` | Two Pronged Gold Crown | Plate | T4 | auto | 40 | 0 | 0 | 0 | 2.3 | OSA |
| `wolfskin_helmet_guard` | Wolfskin Helmet | Plate | T4 | auto | 40 | 0 | 0 | 0 | 4 | OSA |
| `AR_battania_helmet_b2` | Highland Plumed Tall Helmet Over Mail | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_battania_helmet_f` | Highland Plumed Decorated Helmet | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_goth_helmet_b` | Iron Roughscale Helmet With Faceguard | Plate | T4 | auto | 38 | 0 | 0 | 0 | 2.2 | OSA |
| `mp_battania_earmuff_helmet_c` | Fringed Decorated Steel Cap with Cheek Guards | Plate | T4 | 1 | 38 | 0 | 0 | 0 | 3.1 | Vanilla |
| `TV_battania_helmet_c` | Highland Fringed Decorated Steel Helmet | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3.1 | OSA |
| `TV_battania_helmet_d` | Highland Fringed Decorated Bronze Helmet | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3.1 | OSA |
| `TV_battania_helmet_e` | Highland Decorated Steel Helmet | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3.1 | OSA |
| `TV_battania_helmet_f` | Highland Decorated Bronze Helmet | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3.1 | OSA |
| `TV_battania_helmet_s` | Highland Pointed Steel Helmet | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3.2 | OSA |
| `TV_battania_helmet_s2` | Highland Pointed Bronze Helmet | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3.2 | OSA |
| `TV_battania_lord_helmet_c` | Crested Decorated Steel Cap With Cheek Guards | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3.1 | OSA |
| `TV_pict_helmet_c` | Highland Plumed Heavy Banded Nasal Helmet Over Mail | Plate | T4 | auto | 38 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_pict_helmet_f` | Highland Domed Nasal Spangenhelm Over Mail | Plate | T4 | auto | 38 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_pict_helmet_i` | Highland Tall Nasal Spangenhelm Over Mail | Plate | T4 | auto | 38 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_pict_helmet_l` | Highland Plumed Tall Silvered Nasal Spangenhelm Over Mail | Plate | T4 | auto | 38 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_pict_helmet_r` | Highland Heavy Guarded Nasal Helmet With Mail | Plate | T4 | auto | 38 | 0 | 0 | 0 | 2.9 | OSA |
| `mp_battania_earmuff_helmet_c_brnz` | Fringed Decorated Bronze Cap with Cheek Guards | Plate | T4 | 1 | 37 | 0 | 0 | 0 | 3.3 | Vanilla |
| `ao_eorling_guarded_noblemans_helmet` | Highland Plumed Guarded Nobleman's Helmet | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.4 | OSA |
| `ao_eorling_guarded_noblemans_helmet_b` | Highland Guarded Nobleman's Helmet | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.4 | OSA |
| `AR_battania_helmet_f_decorated` | Highland Decorated Helmet | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_battania_helmet_g` | Highland Spangenhelm | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.4 | OSA |
| `mp_battania_earmuff_helmet_a` | Fringed Steel Cap with Cheek Guards | Plate | T4 | 1 | 36 | 0 | 0 | 0 | 3 | Vanilla |
| `AR_phrygian_helmet_c` | Dryatic Steel Bent Conical Helmet | Plate | T4 | auto | 35 | 0 | 0 | 0 | 3.1 | OSA |
| `AR_phrygian_helmet_c2` | Dryatic Brass Bent Conical Helmet | Plate | T4 | auto | 35 | 0 | 0 | 0 | 3.1 | OSA |
| `mp_battania_earmuff_helmet_a_brnz` | Fringed Bronze Cap with Cheek Guards | Plate | T4 | 1 | 35 | 0 | 0 | 0 | 3.2 | Vanilla |
| `mp_battania_earmuff_helmet_c_cpr` | Decorated Bronze Helmet with Cheek Guards | Plate | T4 | 1 | 35 | 0 | 0 | 0 | 3.3 | Vanilla |
| `TV_pict_helmet_b` | Highland Heavy Banded Nasal Helmet Over Hide | Plate | T4 | auto | 35 | 0 | 0 | 0 | 2.6 | OSA |
| `TV_pict_helmet_e` | Highland Domed Nasal Spangenhelm Over Padding | Plate | T4 | auto | 35 | 0 | 0 | 0 | 2.6 | OSA |
| `TV_pict_helmet_h` | Highland Tall Nasal Spangenhelm Over Cloth | Plate | T4 | auto | 35 | 0 | 0 | 0 | 2.6 | OSA |
| `TV_pict_helmet_k` | Highland Plumed Tall Silvered Nasal Spangenhelm Over Leather | Plate | T4 | auto | 35 | 0 | 0 | 0 | 2.6 | OSA |
| `TV_pict_helmet_m` | Highland Banded Helmet | Plate | T4 | auto | 35 | 0 | 0 | 0 | 3.1 | OSA |
| `TV_pict_helmet_q` | Highland Heavy Guarded Nasal Helmet With Leather | Plate | T4 | auto | 35 | 0 | 0 | 0 | 2.6 | OSA |
| `AR_battania_helmet_y` | Highland Banded Helmet Over Leather | Plate | T3 | auto | 34 | 0 | 0 | 0 | 3.5 | OSA |
| `battania_fur_helmet` | Cap with Fur Coif | Leather | T3 | auto | 34 | 0 | 0 | 0 | 1.3 | RBM |
| `bearhead` | Bear Head | Leather | T3 | auto | 34 | 0 | 0 | 0 | 1.4 | RBM |
| `AR_battania_helmet_t` | Highland Steel Ridged Helmet Over Mail | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_battania_helmet_u` | Highland Bronze Ridged Helmet Over Mail | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_battania_helmet_x` | Highland Steel Tall Helmet Over Mail | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_battania_helmet_x2` | Highland Bronze Tall Helmet Over Mail | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_battania_helmet_y2` | Highland Banded Helmet Over Cloth | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.5 | OSA |
| `AR_battania_helmet_zc` | Highland Iron Ridge Helmet Over Stripped Mail | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.7 | OSA |
| `mp_battania_earmuff_helmet_b` | Steel Cap with Cheek Guards | Plate | T3 | 1 | 32 | 0 | 0 | 0 | 2.9 | Vanilla |
| `TV_battania_helmet_l` | Highland Feathered Pointed Steel Cap Over Cloth | Plate | T3 | auto | 32 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_battania_helmet_m` | Highland Feathered Pointed Bronze Cap Over Cloth | Plate | T3 | auto | 32 | 0 | 0 | 0 | 2.9 | OSA |
| `ao_battanian_noblemans_plumed_ridge_helmet` | Highland Plumed Ridge Helmet With Mail | Plate | T3 | auto | 31 | 0 | 0 | 0 | 1.8 | OSA |
| `mp_battania_earmuff_helmet_b_brnz` | Bronze Cap with Cheek Guards | Plate | T3 | 1 | 31 | 0 | 0 | 0 | 3 | Vanilla |
| `TV_pict_helmet_a` | Highland Heavy Banded Nasal Helmet | Plate | T3 | auto | 31 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_pict_helmet_d` | Highland Domed Nasal Spangenhelm | Plate | T3 | auto | 31 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_pict_helmet_g` | Highland Tall Nasal Spangenhelm | Plate | T3 | auto | 31 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_pict_helmet_j` | Highland Tall Silvered Nasal Spangenhelm | Plate | T3 | auto | 31 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_pict_helmet_p` | Highland Heavy Guarded Nasal Helmet | Plate | T3 | auto | 31 | 0 | 0 | 0 | 2.9 | OSA |
| `AR_battania_helmet_b` | Highland Plumed Tall Helmet | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_battania_helmet_j` | Highland Steel Helmet With Faceguard | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.5 | OSA |
| `AR_battania_helmet_k` | Highland Bronze Helmet With Faceguard | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.5 | OSA |
| `AR_battania_helmet_n` | Plumed Highland Steel Helmet With Faceguard | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_battania_helmet_o` | Plumed Highland Bronze Helmet With Faceguard | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_battania_helmet_r` | Highland Steel Ridged Helmet | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.5 | OSA |
| `AR_battania_helmet_s` | Highland Bronze Ridged Helmet | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.5 | OSA |
| `AR_battania_helmet_v` | Highland Steel Tall Helmet With Faceguard | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.5 | OSA |
| `AR_battania_helmet_v2` | Highland Bronze Tall Helmet With Faceguard | Plate | T3 | auto | 30 | 0 | 0 | 0 | 3.5 | OSA |
| `AR_bone_helmet_c` | Shaman's Horned Bone Lamellar Helmet | Plate | T3 | auto | 30 | 0 | 0 | 0 | 2.3 | OSA |
| `AR_goth_helmet_a` | Iron Roughscale Helmet | Plate | T3 | auto | 30 | 0 | 0 | 0 | 2.2 | OSA |
| `celtic_mail_helm` | Tall Nasalhelm With Mail | Plate | T3 | auto | 30 | 0 | 0 | 0 | 2.9 | OSA |
| `ao_battanian_noblemans_ridge_helmet` | Highland Ridge Helmet With Mail | Plate | T3 | auto | 29 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_battania_helmet_a2` | Highland Tall Helmet With Neckguard | Plate | T3 | auto | 28 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_battania_helmet_d` | Highland Plumed Kettle Helmet | Plate | T3 | auto | 28 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_battania_helmet_zb` | Highland Iron Ridge Helmet With Leather | Plate | T3 | auto | 28 | 0 | 0 | 0 | 3.5 | OSA |
| `hmj_imperial_cheek_pteurges_plume` | Highland Plumed Kettle Helmet With Strips | Plate | T3 | auto | 28 | 0 | 0 | 0 | 3.7 | OSA |
| `leather_studdedhelm_over_roughscale` | Leather Studded Helm over Roughscale | Leather | T2 | auto | 27 | 0 | 0 | 0 | 3.6 | RBM |
| `AR_battania_helmet_a` | Highland Tall Helmet | Plate | T2 | auto | 26 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_bone_helmet_b` | Shaman's Feathered Bone Lamellar Helmet | Plate | T2 | auto | 26 | 0 | 0 | 0 | 2.1 | OSA |
| `simple_helmet_with_scale` | Simple Helmet With Scale | Plate | T2 | auto | 26 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_eorling_riveted_nasal_helmet` | Highland Riveted Nasal Helmet With Mail Aventail | Plate | T2 | auto | 25 | 0 | 0 | 0 | 1.8 | OSA |
| `leather_studdedhelm_over_thinhide` | Leather Studded Helm over Thinhide | Leather | T2 | auto | 25 | 0 | 0 | 0 | 1.2 | RBM |
| `wolfhead` | Wolf Head | Leather | T2 | auto | 25 | 0 | 0 | 0 | 1.3 | RBM |
| `AR_battania_helmet_za` | Highland Iron Ridge Helmet Over Cloth | Plate | T2 | auto | 24 | 0 | 0 | 0 | 3.2 | OSA |
| `battanian_crown` | Highland Crown | Plate | T2 | auto | 24 | 0 | 0 | 0 | 3.2 | OSA |
| `hmj_simple_nasal_helm_with_scale` | Highland Nasal Cap Over Scale | Plate | T2 | auto | 24 | 0 | 0 | 0 | 1.1 | OSA |
| `leather_studdedhelm_over_headcloth` | Leather Studded Helm over Headcloth | Leather | T2 | auto | 24 | 0 | 0 | 0 | 1.1 | RBM |
| `TV_battania_shoulders_a` | Brown Bear Head | Leather | T2 | auto | 24 | 0 | 0 | 0 | 1.4 | OSA |
| `leather_studdedhelm` | Leather Studded Helm | Leather | T2 | auto | 23 | 0 | 0 | 0 | 0.5 | RBM |
| `ao_battanian_ridge_helmet` | Highland Ridge Helmet With Leather | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_battania_helmet_c` | Highland Kettle Helmet | Plate | T2 | auto | 22 | 0 | 0 | 0 | 3.7 | OSA |
| `hmj_simple_nasal_helm_hood` | Highland Nasal Cap Over Leather | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1.1 | OSA |
| `simple_helmet_headcloth` | Simple Helmet Over Headwrap | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1.4 | OSA |
| `TV_battania_lord_helmet_d` | Highland Plumed Kettle Helmet | Plate | T2 | auto | 22 | 0 | 0 | 0 | 3.7 | OSA |
| `TV_pict_helmet_j2` | Highland Tall Silvered Spangenhelm Cap | Plate | T2 | auto | 22 | 0 | 0 | 0 | 2.9 | OSA |
| `simple_helmet_scarf` | Simple Helmet With Scarf | Plate | T2 | auto | 21 | 0 | 0 | 0 | 1.5 | OSA |
| `ao_battanian_nasal_helmet` | Highland Nasal Helmet | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_battania_helmet_l` | Highland Open Steel Helmet With Faceguard | Plate | T1 | auto | 20 | 0 | 0 | 0 | 2.3 | OSA |
| `AR_battania_helmet_m` | Highland Open Bronze Helmet With Faceguard | Plate | T1 | auto | 20 | 0 | 0 | 0 | 2.3 | OSA |
| `AR_battania_helmet_p` | Highland Open Steel Ridged Helmet | Plate | T1 | auto | 20 | 0 | 0 | 0 | 2.3 | OSA |
| `AR_battania_helmet_q` | Highland Open Bronze Ridged Helmet | Plate | T1 | auto | 20 | 0 | 0 | 0 | 2.3 | OSA |
| `AR_bone_helmet_a` | Shaman's Bone Lamellar Helmet | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.9 | OSA |
| `hmj_simple_nasal_helm` | Highland Nasal Cap | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.1 | OSA |
| `simple_helmet` | Simple Helmet | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.5 | OSA |
| `ao_battanian_nasal_spangenhelm` | Highland Nasal Spangenhelmet | Plate | T1 | auto | 19 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_battanian_simple_ridge_helmet` | Highland Ridge Helmet With Cloth | Plate | T1 | auto | 19 | 0 | 0 | 0 | 1.8 | OSA |
| `battania_simple_spangenhelm` | Highland Cap Helmet | Plate | T1 | auto | 19 | 0 | 0 | 0 | 1.1 | OSA |
| `battania_skull_cap` | Highland Skull Cap | Plate | T1 | auto | 19 | 0 | 0 | 0 | 1.1 | OSA |
| `AR_battania_helmet_w` | Highland Open Steel Tall Helmet | Plate | T1 | auto | 18 | 0 | 0 | 0 | 3.2 | OSA |
| `AR_battania_helmet_w2` | Highland Open Bronze Tall Helmet | Plate | T1 | auto | 18 | 0 | 0 | 0 | 3.2 | OSA |
| `battania_fur_cap` | Highland Fur Cap | Leather | T1 | auto | 18 | 0 | 0 | 0 | 0.8 | OSA |
| `thinhide_coif` | Thinhide Coif | Leather | T1 | auto | 18 | 0 | 0 | 0 | 0.5 | RBM |
| `ao_battanian_segmented_helmet` | Highland Segmented Helmet | Plate | T1 | auto | 17 | 0 | 0 | 0 | 1.8 | OSA |
| `mp_bearhead` | Bear Head | Leather | T1 | 1 | 16 | 0 | 0 | 0 | 1.4 | Vanilla |
| `TV_wolf_pelt_c` | Brown Wolf Head | Leather | T1 | auto | 16 | 0 | 0 | 0 | 1.3 | OSA |
| `TV_wolf_pelt_e` | Eastern Wolf Head | Leather | T1 | auto | 16 | 0 | 0 | 0 | 1.3 | OSA |
| `battania_crown` | Highland Ceremonial Crown | Plate | T1 | auto | 15 | 0 | 0 | 0 | 0.5 | RBM |
| `AR_hat_e` | Rough Padded Cloth Cap | Cloth | T0 | auto | 11 | 0 | 0 | 0 | 0.2 | OSA |
| `AR_hat_f` | Padded Cloth Cap | Cloth | T0 | auto | 11 | 0 | 0 | 0 | 0.2 | OSA |
| `AR_hat_g` | Plain Padded Cloth Cap | Cloth | T0 | auto | 11 | 0 | 0 | 0 | 0.2 | OSA |
| `battania_civil_hood` | Cloth Hood | Cloth | T0 | auto | 11 | 0 | 0 | 0 | 0.5 | RBM |
| `TV_battania_hood_a` | Plaid Cloth Hood | Cloth | T0 | 300 | 11 | 0 | 0 | 0 | 0.5 | OSA |
| `TV_battania_hood_a2` | Brown Plaid Cloth Hood | Cloth | T0 | 300 | 11 | 0 | 0 | 0 | 0.5 | OSA |
| `TV_battania_hood_a3` | Green Cloth Hood | Cloth | T0 | 300 | 11 | 0 | 0 | 0 | 0.5 | OSA |
| `mp_battania_civil_hood` | Cloth Hood | Cloth | T0 | 1 | 9 | 0 | 0 | 0 | 0.5 | Vanilla |
| `wrapped_headcloth` | Wrapped Headcloth | Cloth | T0 | auto | 9 | 0 | 0 | 0 | 0.2 | RBM |
| `AR_phrygian_cap_b` | Tall Banded Bent Conical Hat | Cloth | T-1 | auto | 5 | 0 | 0 | 0 | 1.2 | OSA |
| `highland_headscarf` | Highland Head Scarf | Cloth | T-1 | auto | 4 | 0 | 0 | 0 | 0.3 | OSA |

<!-- CULTURE:khuzait COUNT:156 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `spiked_helmet_with_facemask` | Spiked Helmet With Facemask | Plate | T5 | auto | 119 | 90 | 40 | 0 | 4.5 | RBM |
| `eastern_vendel_helmet` | Heavy Closed Lancer Helmet | Plate | T5 | auto | 106 | 65 | 40 | 0 | 4 | RBM |
| `khuzait_noble_helmet_with_fur` | Furred Noble Lancer Helmet | Plate | T5 | auto | 141 | 26 | 20 | 0 | 3.9 | RBM |
| `khuzait_noble_helmet_with_feathers` | Noble Lancer Helmet | Plate | T5 | auto | 105 | 14 | 25 | 0 | 3.4 | RBM |
| `khuzait_noble_helmet_with_neckguard` | Plumed Noble Lancer Helmet with Neckguard | Plate | T5 | auto | 102 | 30 | 0 | 0 | 4 | RBM |
| `spiked_helmet` | Spiked Helmet | Plate | T5 | auto | 94 | 0 | 37 | 0 | 4.5 | RBM |
| `khuzait_battle_crown` | Khuzait Battle Crown | Plate | T5 | auto | 90 | 12 | 0 | 0 | 2.7 | RBM |
| `plumed_lamellar_helmet` | Plumed Lamellar Helmet | Plate | T5 | auto | 77 | 0 | 25 | 0 | 3.3 | RBM |
| `plumed_nomad_helmet` | Plumed Nomad Helmet | Plate | T5 | auto | 60 | 12 | 0 | 0 | 1.1 | RBM |
| `steppe_helmet` | Infantry Steel Helmet | Plate | T5 | auto | 65 | 0 | 0 | 0 | 1.2 | RBM |
| `nomad_helmet` | Nomad Helmet | Plate | T5 | auto | 56 | 0 | 0 | 0 | 1.1 | RBM |
| `ao_durkhan_cataphract_helmet_a` | Eastern Mailed Cataphract Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.8 | OSA |
| `ao_durkhan_cataphract_helmet_b` | Eastern Lamellar Cataphract Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.8 | OSA |
| `ao_durkhan_heavy_fur_trimmed_helmet` | Eastern Heavy Fur Trimmed Lamellar Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_goth_helmet_f` | Eastern Plumed Lamellar Helmet Over Mail | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.2 | OSA |
| `AR_goth_helmet_g` | Eastern Closed Plumed Lamellar Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 4.2 | OSA |
| `AR_goth_helmet_o` | Eastern Plumed Banded Helmet Over Mail | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.2 | OSA |
| `AR_khuzait_lord_helmet_b` | Eastern Cataphract Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.9 | OSA |
| `AR_khuzait_lord_helmet_f` | Eastern Plumed Heavy Helmet With Mask | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.9 | OSA |
| `TV_khuzait_lord_helmet_n` | Eastern Noble Fluted Helmet With Sideguards | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.9 | OSA |
| `AR_khuzait_lord_helmet_d` | Eastern Plumed Gilded Helmet With Closed Mail | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_khuzait_lord_helmet_d2` | Eastern Plumed Iron Helmet With Closed Mail | Plate | T5 | auto | 51 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_vaegir_helmet_d` | Northern Fur Vendel Helmet | Plate | T5 | auto | 51 | 0 | 0 | 0 | 4 | OSA |
| `DZ_khuzait_helmet_c` | Eastern Closed Plumed Banded Helmet | Plate | T5 | auto | 51 | 0 | 0 | 0 | 4 | OSA |
| `TV_khuzait_helmet_k` | Eastern Pointed Steel Helmet With Plated Mail | Plate | T5 | auto | 51 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_lord_helmet_e` | Decorated Spiked Helmet With Facemask | Plate | T5 | auto | 51 | 0 | 0 | 0 | 4.5 | OSA |
| `TV_khuzait_lord_helmet_l` | Steppe Spiked Iron Helmet With Facemask | Plate | T5 | auto | 51 | 0 | 0 | 0 | 4.5 | OSA |
| `AR_khuzait_helmet_o` | Eastern Feathered Cataphract's Helmet | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.9 | OSA |
| `AR_khuzait_helmet_t` | Eastern Lamellar Spangenhelm With Mask | Plate | T5 | auto | 50 | 0 | 0 | 0 | 1.7 | OSA |
| `AR_khuzait_lord_helmet_a` | Eastern Plumed Noble Helmet With Faceplate | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_khuzait_lord_helmet_c` | Eastern Gilded Helmet With Mail | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_khuzait_lord_helmet_c2` | Eastern Iron Helmet With Mail | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_khuzait_lord_helmet_g` | Eastern Heavy Vendel Helmet With Leather | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.9 | OSA |
| `TV_khuzait_helmet_j` | Eastern Pointed Steel Helmet With Faceplate | Plate | T5 | auto | 50 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_helmet_q` | Eastern Steel Cap With Closed Lamellar | Plate | T5 | auto | 50 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_helmet_r` | Steppe Plumed Steel Helmet With Closed Lamellar | Plate | T5 | auto | 50 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_helmet_s` | Steppe Feathered Steel Helmet With Closed Lamellar | Plate | T5 | auto | 50 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_lord_helmet_m` | Eastern Plumed Noble Helmet With Plates | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.4 | OSA |
| `AR_khuzait_helmet_w` | Eastern Nasalhelm With Lamellar Faceguard | Plate | T5 | auto | 49 | 0 | 0 | 0 | 2.8 | OSA |
| `AR_khuzait_lord_helmet_i` | Eastern Lordly Nasalhelm With Lamellar Faceguard | Plate | T5 | auto | 49 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_helmet_b` | Eastern Helmet Over Closed Lamellar Veil | Plate | T5 | auto | 49 | 0 | 0 | 0 | 4.8 | OSA |
| `TV_khuzait_helmet_u` | Eastern Steel Cap With Lamellar Faceguard | Plate | T5 | auto | 49 | 0 | 0 | 0 | 2.8 | OSA |
| `AR_goth_helmet_k` | Steppe Plumed Lamellar Cap Over Mail | Plate | T5 | auto | 48 | 0 | 0 | 0 | 1.7 | OSA |
| `AR_goth_helmet_l` | Steppe Decorated Plumed Lamellar Cap Over Mail | Plate | T5 | auto | 48 | 0 | 0 | 0 | 1.7 | OSA |
| `DZ_khuzait_helmet_d` | Eastern Plumed Banded Helmet With Facemask | Plate | T5 | auto | 48 | 0 | 0 | 0 | 4.5 | OSA |
| `DZ_khuzait_helmet_e` | Eastern Decorated Noble Helmet With Facemask | Plate | T5 | auto | 48 | 0 | 0 | 0 | 4.5 | OSA |
| `mp_khuzait_noble_helmet_with_feathers` | Noble Lancer Helmet | Plate | T5 | 1 | 48 | 0 | 0 | 0 | 3.4 | Vanilla |
| `TV_khuzait_helmet_h` | Eastern Pointed Steel Helmet Over Closed Mail | Plate | T5 | auto | 48 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_helmet_n` | Steppe Plumed Steel Helmet With Lamellar | Plate | T5 | auto | 48 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_helmet_o` | Steppe Furred Steel Helmet With Lamellar | Plate | T5 | auto | 48 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_helmet_p` | Steppe Feathered Steel Nasalhelm With Lamellar | Plate | T5 | auto | 48 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_helmet_z3` | Steppe Lamellar Helmet Over Mail | Plate | T5 | auto | 48 | 0 | 0 | 0 | 3.6 | OSA |
| `TV_khuzait_lord_helmet_b` | Eastern Furred Noble Helmet With Feathers | Plate | T5 | auto | 47 | 0 | 0 | 0 | 4.8 | OSA |
| `AR_empire_helmet_e` | Steppe Plumed Spangenhelm Over Mail | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3.3 | OSA |
| `AR_goth_helmet_e` | Eastern Lamellar Helmet Over Leather | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3.5 | OSA |
| `AR_goth_helmet_n` | Eastern Banded Helmet Over Mail | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3.5 | OSA |
| `AR_khuzait_helmet_j` | Eastern Closed Plumed Helmet | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3.9 | OSA |
| `AR_khuzait_lord_helmet_e` | Eastern Heavy Plumed Noble Helmet | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3.7 | OSA |
| `DZ_khuzait_helmet_b` | Eastern Open Plumed Banded Helmet | Plate | T5 | auto | 46 | 0 | 0 | 0 | 4.5 | OSA |
| `TV_khuzait_helmet_c` | Eastern Plated Steel Cap | Plate | T5 | auto | 46 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_helmet_f` | Eastern Feathered Plated Steel Cap | Plate | T5 | auto | 46 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_helmet_i` | Eastern Pointed Steel Helmet Over Plates | Plate | T5 | auto | 46 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_helmet_l` | Steppe Steel Helmet With Lamellar | Plate | T5 | auto | 46 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_helmet_m` | Steppe Feathered Steel Helmet With Lamellar | Plate | T5 | auto | 46 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_helmet_z` | Steppe Lamellar Helmet Over Cloth | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3.2 | OSA |
| `TV_khuzait_lord_helmet_k` | Eastern Gilded Noble Helmet With Plates | Plate | T5 | auto | 46 | 0 | 0 | 0 | 4.6 | OSA |
| `ao_durkhan_brass_lamellar_helmet` | Iltanlar Brass Helmet Helmet With Mail | Plate | T5 | auto | 45 | 0 | 0 | 0 | 3.8 | OSA |
| `ao_durkhan_iron_lamellar_helmet` | Iltanlar Iron Helmet Helmet With Mail | Plate | T5 | auto | 45 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_khuzait_helmet_s` | Eastern Lamellar Spangenhelm Over Mail | Plate | T5 | auto | 45 | 0 | 0 | 0 | 1.7 | OSA |
| `khuzait_crown` | Ornate Steel Cap | Plate | T5 | auto | 45 | 0 | 0 | 0 | 1.4 | RBM |
| `plumed_fur_lined_helmet` | Plumed Fur Lined Helmet | Plate | T5 | auto | 45 | 0 | 0 | 0 | 1.1 | RBM |
| `TV_khuzait_helmet_x` | Eastern Plumed Lamellar Helmet Over Mail | Plate | T5 | auto | 45 | 0 | 0 | 0 | 1.7 | OSA |
| `TV_khuzait_lord_helmet_a` | Eastern Furred Noble Helmet With Plume | Plate | T5 | auto | 45 | 0 | 0 | 0 | 4.6 | OSA |
| `TV_khuzait_lord_helmet_g` | Eastern Lordly Plumed Lamellar Helmet | Plate | T5 | auto | 45 | 0 | 0 | 0 | 4.6 | OSA |
| `TV_khuzait_lord_helmet_h` | Eastern Silvered Battle Crown With Lamellar | Plate | T5 | auto | 45 | 0 | 0 | 0 | 4.6 | OSA |
| `TV_khuzait_lord_helmet_h2` | Eastern Gilded Battle Crown With Leather Lamellar | Plate | T5 | auto | 45 | 0 | 0 | 0 | 4.6 | OSA |
| `AR_empire_helmet_c` | Steppe Feathered Spangenhelm Over Mail Coif | Plate | T5 | auto | 44 | 0 | 0 | 0 | 3.1 | OSA |
| `AR_khuzait_helmet_m` | Eastern Plumed Furred Spangenhelm Over Lamellar | Plate | T5 | auto | 44 | 0 | 0 | 0 | 3.9 | OSA |
| `TV_khuzait_helmet_a` | Eastern Helmet Over Open Lamellar | Plate | T5 | auto | 44 | 0 | 0 | 0 | 3.3 | OSA |
| `TV_khuzait_helmet_d` | Eastern Crested Plated Steel Cap | Plate | T5 | auto | 44 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_helmet_e` | Eastern Plumed Plated Steel Cap | Plate | T5 | auto | 44 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_helmet_t` | Steppe Mailed Steel Nasalhelm | Plate | T5 | auto | 44 | 0 | 0 | 0 | 3.15 | OSA |
| `TV_khuzait_helmet_y4` | Eastern Lordly Plumed Decorated Helmet | Plate | T5 | auto | 44 | 0 | 0 | 0 | 4.6 | OSA |
| `TV_khuzait_helmet_z2` | Steppe Lamellar Helmet Over Leather | Plate | T5 | auto | 43 | 0 | 0 | 0 | 3.5 | OSA |
| `ao_durkhan_heavy_lamellar_helmet` | Eastern Heavy Lamellar Helmet | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_goth_helmet_j` | Steppe Plumed Lamellar Cap Over Leather | Plate | T5 | auto | 42 | 0 | 0 | 0 | 1.7 | OSA |
| `AR_khuzait_helmet_g` | Eastern Spiked Helmet With Leather | Plate | T5 | auto | 42 | 0 | 0 | 0 | 4 | OSA |
| `TV_khuzait_lord_helmet_f` | Steppe Plumed Lamellar Helmet | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.4 | OSA |
| `TV_khuzait_lord_helmet_i` | Eastern Silvered Noble Helmet With Lamellar | Plate | T5 | auto | 42 | 0 | 0 | 0 | 2.7 | OSA |
| `AR_khuzait_helmet_v` | Eastern Nasalhelm With Studded Faceguard | Plate | T4 | auto | 40 | 0 | 0 | 0 | 1.7 | OSA |
| `mp_khuzait_noble_helmet_with_neckguard` | Noble Helmet with Neckguard | Plate | T4 | 1 | 39 | 0 | 0 | 0 | 4 | Vanilla |
| `AR_khuzait_helmet_c` | Eastern Lamellar Helmet With Fur | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3.3 | OSA |
| `AR_khuzait_helmet_c_fur` | Eastern Lamellar Helmet With Upturned Cap | Plate | T4 | auto | 38 | 0 | 0 | 0 | 3.3 | OSA |
| `AR_khuzait_helmet_k` | Eastern Furred Spangenhelm With Feather | Plate | T4 | auto | 38 | 0 | 0 | 0 | 1.7 | OSA |
| `AR_khuzait_helmet_n` | Eastern Spangenhelm With Fur Cap | Plate | T4 | auto | 38 | 0 | 0 | 0 | 1.7 | OSA |
| `AR_khuzait_helmet_r` | Eastern Lamellar Spangenhelm Over Leather | Plate | T4 | auto | 38 | 0 | 0 | 0 | 1.7 | OSA |
| `mp_khuzait_noble_helmet_with_fur` | Noble Helmet with Fur | Plate | T4 | 1 | 38 | 0 | 0 | 0 | 3.9 | Vanilla |
| `TV_khuzait_helmet_w` | Eastern Lamellar Helmet Over Leather | Plate | T4 | auto | 38 | 0 | 0 | 0 | 1.7 | OSA |
| `TV_khuzait_helmet_y3` | Eastern Plumed Decorated Helmet | Plate | T4 | auto | 38 | 0 | 0 | 0 | 2.1 | OSA |
| `mp_spiked_helmet` | Spiked Helmet | Plate | T4 | 1 | 37 | 0 | 0 | 0 | 4.5 | Vanilla |
| `AR_goth_helmet_d` | Eastern Lamellar Helmet Over Cloth | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.2 | OSA |
| `AR_goth_helmet_m` | Eastern Banded Helmet Over Leather | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.2 | OSA |
| `AR_khuzait_lord_helmet_h` | Eastern Silk Wrapped Decorated Noble Cap | Plate | T4 | auto | 36 | 0 | 0 | 0 | 2.7 | OSA |
| `DZ_khuzait_helmet_a` | Eastern Silvered Lamellar Helmet | Plate | T4 | auto | 36 | 0 | 0 | 0 | 3.3 | OSA |
| `TV_khuzait_helmet_g` | Eastern Pointed Steel Helmet Over Leather | Plate | T4 | auto | 36 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_khuzait_lord_helmet_c` | Eastern Decorated Battle Crown | Plate | T4 | auto | 36 | 0 | 0 | 0 | 2.7 | OSA |
| `TV_khuzait_lord_helmet_j` | Eastern Gilded Noble Helmet With Leather Lamellar | Plate | T4 | auto | 36 | 0 | 0 | 0 | 2.7 | OSA |
| `ao_durkhan_brass_horsehair_helmet` | Iltanlar Brass Helmet With Leather | Plate | T4 | auto | 35 | 0 | 0 | 0 | 3.8 | OSA |
| `ao_durkhan_fur_trimmed_helmet` | Eastern Guarded Fur Trimmed Helmet | Plate | T4 | auto | 35 | 0 | 0 | 0 | 3.1 | OSA |
| `ao_durkhan_iron_horsehair_helmet` | Iltanlar Iron Helmet With Leather | Plate | T4 | auto | 35 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_empire_helmet_d` | Steppe Feathered Spangenhelm Over Leather | Plate | T3 | auto | 34 | 0 | 0 | 0 | 1.7 | OSA |
| `AR_goth_helmet_i` | Steppe Lamellar Cap With Cheekguards | Plate | T3 | auto | 34 | 0 | 0 | 0 | 1.7 | OSA |
| `AR_khuzait_helmet_i` | Eastern Iron Helmet Over Leather | Plate | T3 | auto | 34 | 0 | 0 | 0 | 1.4 | OSA |
| `AR_khuzait_helmet_i2` | Eastern Brass Helmet Over Leather | Plate | T3 | auto | 34 | 0 | 0 | 0 | 1.4 | OSA |
| `AR_khuzait_helmet_l` | Eastern Furred Spangenhelm | Plate | T3 | auto | 34 | 0 | 0 | 0 | 1.7 | OSA |
| `AR_khuzait_helmet_q` | Eastern Lamellar Spangenhelm | Plate | T3 | auto | 34 | 0 | 0 | 0 | 1.7 | OSA |
| `AR_khuzait_helmet_u` | Eastern Nasalhelm With Suede Faceguard | Plate | T3 | auto | 34 | 0 | 0 | 0 | 1.7 | OSA |
| `AR_vaegir_helmet_a` | Northern Spiked Helmet With Mail | Plate | T3 | auto | 34 | 0 | 0 | 0 | 4.5 | OSA |
| `AR_vaegir_helmet_c` | Northern Fur Spiked Helmet With Mail | Plate | T3 | auto | 34 | 0 | 0 | 0 | 4.6 | OSA |
| `TV_khuzait_helmet_v` | Eastern Lamellar Helmet | Plate | T3 | auto | 34 | 0 | 0 | 0 | 1.7 | OSA |
| `AR_khuzait_helmet_c3` | Eastern Helmet With Upturned Cap | Plate | T3 | auto | 33 | 0 | 0 | 0 | 2.2 | OSA |
| `TV_khuzait_helmet_a2` | Eastern Helmet Over Leather | Plate | T3 | auto | 33 | 0 | 0 | 0 | 3.3 | OSA |
| `TV_khuzait_helmet_c2` | Eastern Steel Cap | Plate | T3 | auto | 33 | 0 | 0 | 0 | 2.8 | OSA |
| `ao_durkhan_guarded_iron_helmet` | Eastern Guarded Iron Helmet | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_empire_helmet_f` | Steppe Plumed Spangenhelm | Plate | T3 | auto | 32 | 0 | 0 | 0 | 3.2 | OSA |
| `TV_khuzait_helmet_y2` | Eastern Decorated Helmet | Plate | T3 | auto | 32 | 0 | 0 | 0 | 2.1 | OSA |
| `TV_khuzait_helmet_za` | Eastern Silvered Helmet | Plate | T3 | auto | 32 | 0 | 0 | 0 | 2.1 | OSA |
| `TV_khuzait_helmet_zb` | Eastern Gilded Helmet | Plate | T3 | auto | 32 | 0 | 0 | 0 | 2.1 | OSA |
| `ao_durkhan_brass_helmet_with_leather` | Iltanlar Brass Cap | Plate | T3 | auto | 28 | 0 | 0 | 0 | 3.8 | OSA |
| `ao_durkhan_iron_helmet_with_leather` | Iltanlar Iron Cap | Plate | T3 | auto | 28 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_khuzait_helmet_h` | Eastern Iron Helmet Over Rough Fur | Plate | T2 | auto | 26 | 0 | 0 | 0 | 1.4 | OSA |
| `AR_khuzait_helmet_h2` | Eastern Brass Helmet Over Rough Fur | Plate | T2 | auto | 26 | 0 | 0 | 0 | 1.4 | OSA |
| `AR_goth_helmet_h` | Steppe Lamellar Cap | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1.4 | OSA |
| `AR_khuzait_helmet_b` | Steppe Plumed Helmet With Fur | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1.4 | OSA |
| `AR_khuzait_helmet_p` | Eastern Spangenhelm Cap With Fur | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1.4 | OSA |
| `TV_khuzait_helmet_y` | Eastern Decorated Cap | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1.4 | OSA |
| `TV_khuzait_lord_helmet_d` | Eastern Ornate Cap | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1.4 | OSA |
| `AR_khuzait_helmet_b2` | Steppe Plumed Leather Helmet With Fur | Leather | T1 | auto | 20 | 0 | 0 | 0 | 1.4 | OSA |
| `nomad_padded_hood` | Nomad Padded Hood | Cloth | T1 | auto | 20 | 0 | 0 | 0 | 0.4 | RBM |
| `AR_khuzait_helmet_d` | Steppe Leather Cap | Leather | T1 | auto | 18 | 0 | 0 | 0 | 1.1 | OSA |
| `AR_khuzait_helmet_d2` | Steppe Furred Leather Cap | Leather | T1 | auto | 18 | 0 | 0 | 0 | 1.1 | OSA |
| `AR_khuzait_helmet_e` | Steppe Leather Helmet | Leather | T1 | auto | 18 | 0 | 0 | 0 | 1.1 | OSA |
| `AR_khuzait_helmet_f` | Steppe Leather Helmet With Fur | Leather | T1 | auto | 18 | 0 | 0 | 0 | 1.1 | OSA |
| `decorated_nomad_cap` | Decorated Nomad Cap | Cloth | T1 | auto | 15 | 0 | 0 | 0 | 0.4 | RBM |
| `eastern_cap` | Cap with Neck Flap | Cloth | T1 | auto | 14 | 0 | 0 | 0 | 0.3 | RBM |
| `nomad_cap` | Nomad Cap | Cloth | T1 | auto | 14 | 0 | 0 | 0 | 0.3 | RBM |
| `peaked_fur_hood` | Peaked Fur Hood | Leather | T1 | auto | 14 | 0 | 0 | 0 | 0.7 | RBM |
| `upturned_fur_cap` | Upturned Fur Cap | Leather | T1 | auto | 14 | 0 | 0 | 0 | 0.6 | RBM |
| `AR_khuzait_hood_b` | Eastern Peaked Leather Hood | Leather | T0 | auto | 13 | 0 | 0 | 0 | 0.7 | OSA |
| `AR_khuzait_hood_a` | Eastern Leather Hood | Leather | T0 | auto | 12 | 0 | 0 | 0 | 0.6 | OSA |
| `fur_hood` | Thick Fur Hood | Leather | T0 | auto | 12 | 0 | 0 | 0 | 0.6 | RBM |
| `fur_hat` | Nomad Fur Cap | Leather | T0 | auto | 7 | 0 | 0 | 0 | 0.5 | RBM |
| `mp_decorated_nomad_cap` | Decorated Nomad Cap | Cloth | T-1 | 1 | 6 | 0 | 0 | 0 | 0.4 | Vanilla |
| `ON_cone_hat_a` | Eastern Conical Straw Hat | Cloth | T-1 | auto | 6 | 0 | 0 | 0 | 1.2 | OSA |
| `AR_khuzait_helmet_a` | Eastern Fur Cap | Leather | T-1 | auto | 4 | 0 | 0 | 0 | 0.6 | OSA |
| `mp_nomad_padded_hood` | Nomad Padded Hood | Cloth | T-1 | 1 | 4 | 0 | 0 | 0 | 0.4 | Vanilla |

<!-- CULTURE:aserai COUNT:217 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `brass_aserai_helmet_closed` | Brass Southern Closed Helmet | Plate | T5 | auto | 88 | 64 | 40 | 0 | 2.5 | RBM |
| `brass_aserai_helmet_b_closed` | Brass Southern Closed Helmet with Turban | Plate | T5 | auto | 90 | 70 | 30 | 0 | 2.55 | RBM |
| `pointed_skullcap_over_mail_coif` | Pointed Skullcap over Mail Coif | Plate | T5 | auto | 88 | 24 | 40 | 0 | 3 | RBM |
| `emirs_helmet` | Emir Helmet | Plate | T5 | auto | 94 | 10 | 40 | 0 | 3 | RBM |
| `desert_mail_coif` | Wrapped Helmet with Mail Coif | Plate | T5 | auto | 90 | 10 | 40 | 0 | 3 | RBM |
| `southern_noble_helmet` | Noble Steel Cap | Plate | T5 | auto | 97 | 12 | 25 | 0 | 3.5 | RBM |
| `closed_desert_helmet_with_mail` | Closed Steel Cap | Plate | T5 | auto | 67 | 35 | 35 | 0 | 3.2 | RBM |
| `aserai_lord_helmet_a` | Noble Crowned Lord Helmet | Plate | T5 | auto | 91 | 12 | 25 | 0 | 3.7 | RBM |
| `southern_lord_helmet` | Lordly Steel Cap | Plate | T5 | auto | 100 | 22 | 0 | 0 | 3.9 | RBM |
| `pointed_skullcap_over_mail` | Pointed Skullcap over Gambeson | Plate | T5 | auto | 85 | 12 | 25 | 0 | 1.3 | RBM |
| `brass_aserai_helmet_b_open` | Brass Southern Helmet with Turban | Plate | T5 | auto | 85 | 14 | 20 | 0 | 2.3 | RBM |
| `brass_aserai_helmet_open` | Brass Southern Helmet | Plate | T5 | auto | 83 | 14 | 20 | 0 | 2.2 | RBM |
| `aserai_battle_crown` | Ornate Desert Battle Crown | Plate | T5 | auto | 90 | 22 | 0 | 0 | 2.8 | RBM |
| `pointed_skullcap_with_mail` | Pointed Skullcap with Mail | Plate | T5 | auto | 73 | 12 | 20 | 0 | 2.8 | RBM |
| `pointed_skullcap_over_cloth_headwrap` | Pointed Skullcap over Cloth Headwrap | Plate | T5 | auto | 62 | 16 | 10 | 0 | 1.1 | RBM |
| `brass_aserai_helmet_b_leather` | Brass Turbaned Helmet with Leather Flaps | Plate | T5 | auto | 77 | 4 | 0 | 0 | 1.9 | RBM |
| `pointed_skullcap_over_laced_coif` | Pointed Skullcap over Laced Coif | Plate | T5 | auto | 57 | 12 | 15 | 0 | 1.2 | RBM |
| `brass_aserai_helmet_leather` | Brass Southern Helmet with Leather Flaps | Plate | T5 | auto | 75 | 4 | 0 | 0 | 1.8 | RBM |
| `trailed_desert_helmet` | Helmet with Turban and Coif | Plate | T5 | auto | 57 | 0 | 10 | 0 | 1.1 | RBM |
| `desert_helmet_with_mail` | Mamluke Helmet with Mail | Plate | T5 | auto | 65 | 0 | 0 | 0 | 1.9 | RBM |
| `aserai_crown` | Luxury Turban Over Helmet | Plate | T5 | auto | 60 | 3 | 0 | 0 | 2.8 | RBM |
| `closed_desert_helmet` | Spiked Helmet with Closed Turban | Plate | T5 | auto | 47 | 6 | 6 | 0 | 1.4 | RBM |
| `desert_helmet` | Steel Cap | Plate | T5 | auto | 46 | 12 | 0 | 0 | 1.2 | RBM |
| `open_desert_helmet` | Spiked Helmet with Open Turban | Plate | T5 | auto | 45 | 5 | 6 | 0 | 1.3 | RBM |
| `AR_aserai_helmet_r` | Darshi Immortal's Plumed Ridge Helmet | Plate | T5 | auto | 54 | 0 | 0 | 0 | 4.2 | OSA |
| `AR_aserai_helmet_z` | Darshi Immortal's Plumed Lamellar Helmet | Plate | T5 | auto | 54 | 0 | 0 | 0 | 4.2 | OSA |
| `AR_aserai_helmet_zd` | Darshi Immortal's Plumed Spangenhelm | Plate | T5 | auto | 54 | 0 | 0 | 0 | 4.2 | OSA |
| `AR_aserai_lord_helmet_h` | Southern Noble Lamellar Helmet With Faceplate | Plate | T5 | auto | 54 | 0 | 0 | 0 | 4.2 | OSA |
| `loose_wrapped_desert_helmet` | Helmet with Open Turban | Plate | T5 | auto | 44 | 0 | 12 | 0 | 1.2 | RBM |
| `AR_aserai_cataphract_helmet_a` | Southern Cataphract's Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.7 | OSA |
| `AR_aserai_cataphract_helmet_b` | Desert Cataphract's Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.4 | OSA |
| `AR_aserai_cataphract_helmet_c` | Southern Cataphract's Gilded Tall Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 4.2 | OSA |
| `AR_aserai_cataphract_helmet_d` | Southern Cataphract's Tall Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 4.2 | OSA |
| `AR_aserai_helmet_g` | Darshi Tall Helmet Over Mail | Plate | T5 | auto | 52 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_aserai_helmet_h` | Darshi Tall Helmet Over Closed Mail | Plate | T5 | auto | 52 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_aserai_helmet_q` | Darshi Plumed Ridge Helmet Over Mail | Plate | T5 | auto | 52 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_aserai_helmet_y` | Darshi Plumed Lamellar Helmet Over Mail | Plate | T5 | auto | 52 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_aserai_helmet_zc` | Darshi Plumed Spangenhelm Over Mail | Plate | T5 | auto | 52 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_aserai_lord_helmet_e` | Southern Immortal's Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 4.2 | OSA |
| `AR_aserai_lord_helmet_f` | Darshi Closed Noble Cavalry Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 4.2 | OSA |
| `AR_aserai_lord_helmet_g` | Southern Noble Pointed Iron Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 4.2 | OSA |
| `AR_aserai_lord_helmet_g2` | Southern Noble Pointed Bronze Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 4.2 | OSA |
| `AR_aserai_lord_helmet_i` | Darshi Crowned Noble Lamellar Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 4.2 | OSA |
| `TV_aserai_helmet_i` | Darshi Decorated Spangenhelm With Closed Mail | Chainmail | T5 | auto | 52 | 0 | 0 | 0 | 3.2 | OSA |
| `TV_aserai_helmet_l` | Darshi Plumed Decorated Spangenhelm With Closed Mail | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.2 | OSA |
| `tv_aserai_lord_helmet_h` | Southern Gilded Cataphract's Helmet | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.7 | OSA |
| `tv_aserai_lord_helmet_i` | Southern Cataphract's Spangenhelm | Plate | T5 | auto | 52 | 0 | 0 | 0 | 3.7 | OSA |
| `DZ_aserai_helmet_d` | Desert Decorated Closed Cap Helmet | Plate | T5 | auto | 51 | 0 | 0 | 0 | 4 | OSA |
| `southern_vendel_helmet` | Southern Vendel Helmet | Chainmail | T5 | auto | 51 | 0 | 0 | 0 | 4.2 | OSA |
| `AR_aserai_lord_helmet_c` | Southern Gilded Tall Helmet With Clothed Mail | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.9 | OSA |
| `AR_aserai_lord_helmet_d` | Southern Tall Helmet With Clothed Mail | Plate | T5 | auto | 50 | 0 | 0 | 0 | 3.9 | OSA |
| `AR_aserai_lord_helmet_f2` | Darshi Open Noble Cavalry Helmet | Plate | T5 | auto | 50 | 0 | 0 | 0 | 4.2 | OSA |
| `TV_aserai_helmet_y` | Darshi Decorated Spangenhelm With Iron Scale | Plate | T5 | auto | 50 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_aserai_helmet_y2` | Darshi Decorated Spangenhelm With Brass Scale | Plate | T5 | auto | 50 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_aserai_guard_helmet_f2` | Southern Fluted Steel Helmet Over Closed Wrapped Mail | Plate | T5 | auto | 48 | 0 | 0 | 0 | 2.55 | OSA |
| `TV_aserai_guard_helmet_f3` | Southern Fluted Steel Helmet Over Closed Veiled Mail | Plate | T5 | auto | 48 | 0 | 0 | 0 | 2.55 | OSA |
| `AR_aserai_lord_helmet_j` | Darshi Tall Helmet With Feathers | Plate | T5 | auto | 47 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_aserai_guard_helmet_f` | Southern Fluted Steel Helmet Over Closed Mail | Plate | T5 | auto | 47 | 0 | 0 | 0 | 2.5 | OSA |
| `AR_aserai_archer_helmet_c` | Southern Archer Helmet Over Mail | Chainmail | T5 | auto | 46 | 0 | 0 | 0 | 3 | OSA |
| `AR_aserai_archer_helmet_g` | Southern Domed Archer Helmet Over Mail | Chainmail | T5 | auto | 46 | 0 | 0 | 0 | 3 | OSA |
| `AR_aserai_helmet_e` | Southern Wrapped Spangenhelm Over Mail | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3 | OSA |
| `AR_aserai_helmet_f` | Southern Tailed Spangenhelm Over Mail | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3 | OSA |
| `AR_aserai_helmet_n` | Southern Pointed Iron Helmet With Mail | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3.9 | OSA |
| `AR_aserai_helmet_n2` | Southern Pointed Bronze Helmet With Mail | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3.9 | OSA |
| `AR_aserai_helmet_p` | Darshi Ridge Helmet Over Mail | Plate | T5 | auto | 46 | 0 | 0 | 0 | 2.9 | OSA |
| `AR_aserai_helmet_x` | Darshi Lamellar Helmet Over Mail | Plate | T5 | auto | 46 | 0 | 0 | 0 | 2.9 | OSA |
| `AR_aserai_helmet_zb` | Darshi Spangenhelm Over Mail | Plate | T5 | auto | 46 | 0 | 0 | 0 | 2.9 | OSA |
| `AR_aserai_lord_helmet_b` | Desert Lord Helmet With Faceplate | Plate | T5 | auto | 46 | 0 | 0 | 0 | 3.7 | OSA |
| `aserai_mubarizun_helmet` | Southern Champion's Helmet | Chainmail | T5 | auto | 46 | 0 | 0 | 0 | 3.5 | OSA |
| `TV_aserai_helmet_q` | Southern Spangenhelm With Heavy Mail | Chainmail | T5 | auto | 46 | 0 | 0 | 0 | 3 | OSA |
| `TV_aserai_helmet_s` | Southern Cap Helmet With Heavy Mail | Chainmail | T5 | auto | 46 | 0 | 0 | 0 | 3 | OSA |
| `TV_aserai_helmet_u` | Southern Gilded Helmet With Heavy Mail | Chainmail | T5 | auto | 46 | 0 | 0 | 0 | 3 | OSA |
| `tv_aserai_lord_helmet_g` | Southern Lord's Spangenhelm | Plate | T5 | auto | 46 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_aserai_guard_helmet_c` | Southern Fluted Steel Cap Over Mail | Plate | T5 | auto | 45 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_aserai_guard_helmet_c2` | Southern Fluted Steel Cap Over Wrapped Mail | Plate | T5 | auto | 45 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_aserai_guard_helmet_c3` | Southern Fluted Steel Nasal Cap Over Mail | Plate | T5 | auto | 45 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_aserai_guard_helmet_c4` | Southern Fluted Steel Nasal Cap Over Wrapped Mail | Plate | T5 | auto | 45 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_aserai_guard_helmet_c5` | Southern Fluted Steel Cap Over Open Mail | Plate | T5 | auto | 45 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_aserai_helmet_p` | Southern Spangenhelm With Mail | Plate | T5 | auto | 45 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_aserai_helmet_r` | Southern Cap Helmet With Mail | Plate | T5 | auto | 45 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_aserai_helmet_t` | Southern Gilded Helmet With Mail | Plate | T5 | auto | 45 | 0 | 0 | 0 | 2.9 | OSA |
| `TV_aserai_helmet_x` | Darshi Decorated Spangenhelm With Mail | Plate | T5 | auto | 45 | 0 | 0 | 0 | 2.9 | OSA |
| `AR_aserai_lord_helmet_a` | Desert Lord Helmet | Plate | T5 | auto | 44 | 0 | 0 | 0 | 3.7 | OSA |
| `DZ_aserai_helmet_f` | Desert Noble Pointed Helmet | Plate | T5 | auto | 44 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_aserai_lord_helmet_b` | Ornate Southern Silvered Battle Crown | Plate | T5 | auto | 44 | 0 | 0 | 0 | 2.8 | OSA |
| `TV_aserai_lord_helmet_c` | Ornate Southern Battle Crown | Plate | T5 | auto | 44 | 0 | 0 | 0 | 2.8 | OSA |
| `AR_aserai_helmet_v2` | Southern Iron Closed Helmet with Turban | Plate | T5 | auto | 43 | 0 | 0 | 0 | 2.55 | OSA |
| `mp_aserai_lord_helmet_a` | Aserai Lord Helmet | Plate | T5 | 1 | 43 | 0 | 0 | 0 | 3.7 | Vanilla |
| `mp_southern_lord_helmet` | Aserai Lord Helmet | Plate | T5 | 1 | 43 | 0 | 0 | 0 | 3.7 | Vanilla |
| `mp_southern_noble_helmet` | Aserai Noble Helmet | Plate | T5 | 1 | 43 | 0 | 0 | 0 | 3.7 | Vanilla |
| `TV_aserai_guard_helmet_e2` | Southern Fluted Steel Cap Over Closed Wrapped Mail | Plate | T5 | auto | 43 | 0 | 0 | 0 | 2.55 | OSA |
| `TV_aserai_guard_helmet_e3` | Southern Fluted Steel Cap Over Closed Veiled Mail | Plate | T5 | auto | 43 | 0 | 0 | 0 | 2.55 | OSA |
| `AR_aserai_archer_helmet_d` | Southern Noble Archer Helmet | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.9 | OSA |
| `AR_aserai_archer_helmet_h` | Southern Noble Domed Archer Helmet | Plate | T5 | auto | 42 | 0 | 0 | 0 | 3.9 | OSA |
| `AR_aserai_helmet_v` | Southern Iron Closed Helmet | Plate | T5 | auto | 42 | 0 | 0 | 0 | 2.5 | OSA |
| `TV_aserai_guard_helmet_e` | Southern Fluted Steel Cap Over Closed Mail | Plate | T5 | auto | 42 | 0 | 0 | 0 | 2.5 | OSA |
| `TV_aserai_lord_helmet_d` | Decorated Southern Cap Helmet Over Turban | Plate | T5 | auto | 42 | 0 | 0 | 0 | 2.8 | OSA |
| `pirate_tier_3_head` | Pirate Helmet with Turban | Plate | T5 | auto | 40 | 2 | 0 | 0 | 2.3 | RBM |
| `southern_lord_helmet_a` | Southern Spiked Lord Helmet Over Guarded Mail | Plate | T4 | auto | 40 | 0 | 0 | 0 | 1.5 | OSA |
| `tv_aserai_lord_helmet_e` | Southern Lord's Pointed Cap Helmet | Plate | T4 | auto | 40 | 0 | 0 | 0 | 3.8 | OSA |
| `tv_aserai_lord_helmet_f` | Southern Lord's Domed Cevelliere | Plate | T4 | auto | 40 | 0 | 0 | 0 | 3.8 | OSA |
| `AR_aserai_helmet_u2` | Southern Iron Helmet with Turban | Plate | T4 | auto | 39 | 0 | 0 | 0 | 2.3 | OSA |
| `hmj_aserai_helmet_a` | Southern Wrapped Pointed Skullcap Over Mail Coif | Plate | T4 | auto | 39 | 0 | 0 | 0 | 2.8 | OSA |
| `ao_subuwari_noblemans_helmet` | Southern Bronze Cone Helmet Over Mail | Plate | T4 | auto | 38 | 0 | 0 | 0 | 1.8 | OSA |
| `ao_subuwari_noblemans_helmet_b` | Southern Iron Cone Helmet Over Mail | Plate | T4 | auto | 38 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_aserai_helmet_u` | Southern Iron Helmet | Plate | T4 | auto | 38 | 0 | 0 | 0 | 2.2 | OSA |
| `southern_lord_helmet_b` | Southern Spiked Lord Helmet Over Mail | Plate | T4 | auto | 38 | 0 | 0 | 0 | 1.5 | OSA |
| `AR_aserai_helmet_m` | Southern Open Pointed Iron Helmet | Plate | T4 | auto | 37 | 0 | 0 | 0 | 2.2 | OSA |
| `AR_aserai_helmet_m2` | Southern Open Pointed Bronze Helmet | Plate | T4 | auto | 37 | 0 | 0 | 0 | 2.2 | OSA |
| `AR_aserai_helmet_t2` | Southern Iron Turbaned Helmet with Leather Flaps | Plate | T4 | auto | 37 | 0 | 0 | 0 | 1.9 | OSA |
| `AR_aserai_helmet_t` | Southern Iron Helmet with Leather Flaps | Plate | T4 | auto | 36 | 0 | 0 | 0 | 1.8 | OSA |
| `TV_aserai_helmet_a` | Southern Mailed Nasal Helmet With Headscarf | Plate | T4 | auto | 36 | 0 | 0 | 0 | 2.1 | OSA |
| `AR_aserai_helmet_l` | Southern Wrapped Pointed Iron Helmet | Plate | T4 | auto | 35 | 0 | 0 | 0 | 1.9 | OSA |
| `AR_aserai_helmet_l2` | Southern Wrapped Pointed Bronze Helmet | Plate | T4 | auto | 35 | 0 | 0 | 0 | 1.9 | OSA |
| `AR_aserai_helmet_c2` | Southern Spangenhelm Over Leather | Plate | T3 | auto | 34 | 0 | 0 | 0 | 1.1 | OSA |
| `AR_aserai_helmet_k` | Southern Tailed Pointed Iron Helmet | Plate | T3 | auto | 34 | 0 | 0 | 0 | 1.8 | OSA |
| `AR_aserai_helmet_k2` | Southern Tailed Pointed Bronze Helmet | Plate | T3 | auto | 34 | 0 | 0 | 0 | 1.8 | OSA |
| `southern_heavy_helmet_with_feather` | Southern Feathered Lord Helmet Over Mail | Plate | T3 | auto | 34 | 0 | 0 | 0 | 1.8 | OSA |
| `TV_aserai_helmet_w` | Darshi Decorated Spangenhelm With Leather | Plate | T3 | auto | 34 | 0 | 0 | 0 | 2.9 | OSA |
| `saracen_helmet_d` | Southern Vendel Helmet With Tuareg | Plate | T3 | auto | 33 | 0 | 0 | 0 | 1.6 | OSA |
| `southern_heavy_helmet` | Southern Heavy Helmet | Plate | T3 | auto | 33 | 0 | 0 | 0 | 1.2 | OSA |
| `AR_aserai_helmet_i` | Darshi Open Tall Helmet | Plate | T3 | auto | 32 | 0 | 0 | 0 | 1.8 | OSA |
| `mp_desert_mail_coif` | Desert Mail Coif | Chainmail | T3 | 1 | 32 | 0 | 0 | 0 | 3 | Vanilla |
| `southern_heavy_tuareg` | Southern Pointed Skullcap With Tuareg | Plate | T3 | auto | 32 | 0 | 0 | 0 | 1.3 | OSA |
| `TV_aserai_helmet_k` | Southern Cap Helmet With Tuareg | Plate | T3 | auto | 32 | 0 | 0 | 0 | 1.3 | OSA |
| `AR_aserai_helmet_j` | Darshi Tailed Tall Helmet | Plate | T3 | auto | 30 | 0 | 0 | 0 | 1.8 | OSA |
| `aserai_nasalhelm_mail_veil` | Southern Nasalhelm With Closed Mail | Chainmail | T3 | auto | 30 | 0 | 0 | 0 | 3.2 | OSA |
| `southern_mail_helm_band` | Southern Banded Nasalhelm Over Mail | Chainmail | T3 | auto | 30 | 0 | 0 | 0 | 3.2 | OSA |
| `aserai_nasalhelm_mailcoif` | Southern Nasalhelm With Mail Coif | Chainmail | T3 | auto | 28 | 0 | 0 | 0 | 3.1 | OSA |
| `southern_helmet_with_band` | Southern Helmet With Band | Plate | T3 | auto | 28 | 0 | 0 | 0 | 1 | OSA |
| `TV_aserai_helmet_f` | Loose Wrapped Southern Nasalhelm | Plate | T2 | auto | 27 | 0 | 0 | 0 | 2.1 | OSA |
| `closed_head_scarf` | Closed Head Scarf | Cloth | T2 | auto | 14 | 14 | 0 | 0 | 0.6 | RBM |
| `tight_head_scarf` | Tight Head Scarf | Cloth | T2 | auto | 14 | 14 | 0 | 0 | 0.7 | RBM |
| `TV_aserai_helmet_e` | Open Southern Cap Helmet | Plate | T2 | auto | 24 | 0 | 0 | 0 | 1.3 | OSA |
| `southern_pirate_head` | Corsair Helmet | Cloth | T2 | auto | 15 | 10 | 0 | 0 | 1.3 | RBM |
| `saracen_helmet_c` | Closed Southern Spiked Helmet | Plate | T2 | auto | 23 | 0 | 0 | 0 | 1.3 | OSA |
| `southern_cone_helmet` | Southern Open Pointed Skullcap | Plate | T2 | auto | 23 | 0 | 0 | 0 | 1 | OSA |
| `TV_aserai_helmet_c` | Gilded Closed Southern Helmet | Plate | T2 | auto | 23 | 0 | 0 | 0 | 2.4 | OSA |
| `TV_aserai_helmet_d` | Closed Southern Cap Helmet | Plate | T2 | auto | 23 | 0 | 0 | 0 | 2.4 | OSA |
| `TV_aserai_helmet_j` | Tailed Southern Cap Helmet | Plate | T2 | auto | 23 | 0 | 0 | 0 | 1.3 | OSA |
| `TV_aserai_helmet_n` | Southern Closed Spangenhelm | Plate | T2 | auto | 23 | 0 | 0 | 0 | 2.4 | OSA |
| `AR_aserai_crown_a` | Darshi Jeweled Crown | Plate | T2 | auto | 22 | 0 | 0 | 0 | 3.1 | OSA |
| `mp_desert_helmet_with_mail` | Desert Helmet with Mail | Plate | T2 | 1 | 22 | 0 | 0 | 0 | 1.9 | Vanilla |
| `mp_pointed_skullcap_over_laced_coif` | Pointed Skullcap over Laced Coif | Plate | T2 | 1 | 22 | 0 | 0 | 0 | 1.2 | Vanilla |
| `saracen_helmet_d_ii` | Southern Wrapped Pointed Skullcap | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1 | OSA |
| `southern_helmet_IIII` | Southern Tailed Pointed Skullcap | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1 | OSA |
| `southern_light_helmet_a` | Southern Closed Pointed Skullcap | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1 | OSA |
| `TV_aserai_helmet_b` | Gilded Open Southern Helmet | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1.3 | OSA |
| `TV_aserai_helmet_g` | Wrapped Southern Cap Helmet | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1.3 | OSA |
| `TV_aserai_helmet_m` | Southern Open Spangenhelm | Plate | T2 | auto | 22 | 0 | 0 | 0 | 1.3 | OSA |
| `TV_aserai_helmet_o` | Southern Loose Wrapped Spangenhelm | Plate | T2 | auto | 22 | 0 | 0 | 0 | 2.1 | OSA |
| `AR_aserai_archer_helmet_b` | Southern Closed Archer Helmet | Plate | T2 | auto | 21 | 0 | 0 | 0 | 1.4 | OSA |
| `AR_aserai_archer_helmet_f` | Southern Closed Domed Archer Helmet | Plate | T2 | auto | 21 | 0 | 0 | 0 | 1.4 | OSA |
| `AR_aserai_helmet_d` | Southern Open Spangenhelm | Plate | T2 | auto | 21 | 0 | 0 | 0 | 1.3 | OSA |
| `TV_aserai_guard_helmet_d2` | Southern Fluted Steel Helmet Over Open Cloth | Plate | T2 | auto | 21 | 0 | 0 | 0 | 1.3 | OSA |
| `TV_aserai_guard_helmet_d3` | Southern Fluted Steel Helmet Over Wrapped Cloth | Plate | T2 | auto | 21 | 0 | 0 | 0 | 1.3 | OSA |
| `ao_subuwari_helmet_a` | Southern Bronze Cone Helmet | Plate | T1 | auto | 20 | 0 | 0 | 0 | 0.5 | OSA |
| `ao_subuwari_helmet_a2` | Southern Iron Cone Helmet | Plate | T1 | auto | 20 | 0 | 0 | 0 | 0.5 | OSA |
| `AR_aserai_archer_helmet_a` | Southern Open Archer Helmet | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.3 | OSA |
| `AR_aserai_archer_helmet_e` | Southern Open Domed Archer Helmet | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.3 | OSA |
| `AR_aserai_helmet_c` | Southern Spangenhelm Over Trailed Cloth | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.1 | OSA |
| `AR_aserai_helmet_o` | Darshi Ridged Helmet Over Cloth | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.1 | OSA |
| `AR_aserai_helmet_w` | Darshi Lamellar Helmet Over Cloth | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.1 | OSA |
| `AR_aserai_helmet_za` | Darshi Spangenhelm Over Cloth | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.1 | OSA |
| `TV_aserai_guard_helmet_b2` | Southern Fluted Steel Cap Over Open Cloth | Plate | T1 | auto | 20 | 0 | 0 | 0 | 1.3 | OSA |
| `TV_aserai_helmet_v` | Darshi Decorated Spangenhelm Over Cloth | Plate | T1 | auto | 20 | 0 | 0 | 0 | 2.2 | OSA |
| `AR_aserai_helmet_b` | Southern Cap Helmet With Headscarf | Plate | T1 | auto | 19 | 0 | 0 | 0 | 1.2 | OSA |
| `hmj_southern_headband_helm` | Southern Nasal Helmet With Headscarf | Plate | T1 | auto | 19 | 0 | 0 | 0 | 1.2 | OSA |
| `mp_pointed_skullcap_over_mail` | Pointed Skullcap over Mail | Leather | T1 | 1 | 19 | 0 | 0 | 0 | 1.3 | Vanilla |
| `TV_aserai_guard_helmet_b` | Southern Fluted Steel Cap Over Cloth | Plate | T1 | auto | 19 | 0 | 0 | 0 | 2.2 | OSA |
| `TV_aserai_guard_helmet_d` | Southern Fluted Steel Helmet | Plate | T1 | auto | 19 | 0 | 0 | 0 | 1.3 | OSA |
| `AR_aserai_helmet_s` | Southern Wrapped Pointed Iron Cap Helmet | Plate | T1 | auto | 18 | 0 | 0 | 0 | 1.2 | OSA |
| `AR_aserai_helmet_s2` | Southern Wrapped Pointed Bronze Cap Helmet | Plate | T1 | auto | 18 | 0 | 0 | 0 | 1.2 | OSA |
| `aserai_nasalhelm_cloth` | Southern Loose Wrapped Nasal Helmet | Plate | T1 | auto | 18 | 0 | 0 | 0 | 1.4 | OSA |
| `TV_aserai_helmet_v2` | Darshi Decorated Spangenhelm | Plate | T1 | auto | 18 | 0 | 0 | 0 | 2.2 | OSA |
| `AR_aserai_helmet_a` | Southern Wrapped Helmet | Plate | T1 | auto | 17 | 0 | 0 | 0 | 1.2 | OSA |
| `mp_desert_helmet` | Steel Cap | Plate | T1 | 1 | 17 | 0 | 0 | 0 | 1.2 | Vanilla |
| `TV_aserai_guard_helmet_a` | Southern Fluted Steel Cap | Plate | T1 | auto | 17 | 0 | 0 | 0 | 1.2 | OSA |
| `TV_aserai_guard_helmet_a2` | Southern Fluted Steel Cap With Feathers | Plate | T1 | auto | 17 | 0 | 0 | 0 | 1.2 | OSA |
| `mp_pointed_skullcap_over_cloth_headwrap` | Pointed Skullcap over Cloth Headwrap | Plate | T1 | 1 | 16 | 0 | 0 | 0 | 1.1 | Vanilla |
| `open_head_scarf` | Open Head Scarf | Cloth | T1 | auto | 10 | 5 | 0 | 0 | 0.6 | RBM |
| `desert_cap` | Wrapped Cap | Cloth | T1 | auto | 14 | 0 | 0 | 0 | 0.4 | RBM |
| `mp_trailed_desert_helmet` | Helmet with Turban and Coif | Plate | T1 | 1 | 14 | 0 | 0 | 0 | 1.1 | Vanilla |
| `tied_head_wrapping` | Tied Head Wrapping | Cloth | T1 | auto | 14 | 0 | 0 | 0 | 0.5 | RBM |
| `pirate_a_head` | Pirate Turban | Cloth | T0 | auto | 12 | 0 | 0 | 0 | 0.5 | RBM |
| `AR_hat_i` | Bedouin Wrapped Straw Cap | Cloth | T0 | auto | 11 | 0 | 0 | 0 | 1.2 | OSA |
| `pirate_tier_2_head` | Pirate Turban with Stone | Cloth | T0 | auto | 11 | 0 | 0 | 0 | 0.6 | RBM |
| `tuareg` | Tribal Turban | Cloth | T0 | auto | 11 | 0 | 0 | 0 | 0.6 | RBM |
| `woven_turban` | Woven Turban | Cloth | T0 | auto | 11 | 0 | 0 | 0 | 0.8 | RBM |
| `wrapped_desert_cap` | Wrapped Leather Cap | Cloth | T0 | auto | 11 | 0 | 0 | 0 | 0.5 | RBM |
| `aserai_civil_f_cape` | Bandit Leather Hood | Leather | T0 | auto | 8 | 3 | 0 | 0 | 0.1 | RBM |
| `aserai_female_civil_hscarf` | Desert Headscarf | Cloth | T0 | auto | 10 | 0 | 0 | 0 | 0.4 | RBM |
| `aserai_female_civil_hscarf_b` | Headscarf with Fez | Cloth | T0 | auto | 10 | 0 | 0 | 0 | 0.4 | RBM |
| `female_head_wrap` | Female Headwrap | Cloth | T0 | auto | 10 | 0 | 0 | 0 | 0.4 | RBM |
| `aserai_civil_d_hscarf` | Colored Turban | Cloth | T0 | auto | 9 | 0 | 0 | 0 | 0.1 | RBM |
| `desert_headdress` | Desert Headdress | Cloth | T0 | auto | 9 | 0 | 0 | 0 | 0.2 | RBM |
| `southern_pirate_turban` | Corsair Feathered Helmet | Cloth | T0 | auto | 9 | 0 | 0 | 0 | 1 | RBM |
| `turban` | Turban | Cloth | T0 | auto | 9 | 0 | 0 | 0 | 0.5 | RBM |
| `mp_woven_turban` | Woven Turban | Cloth | T0 | 1 | 8 | 0 | 0 | 0 | 0.8 | Vanilla |
| `aserai_villager_female_headdress` | Rural Headdress | Cloth | T0 | auto | 7 | 0 | 0 | 0 | 0.2 | RBM |
| `headscarf_d` | Hijab | Cloth | T0 | auto | 7 | 0 | 0 | 0 | 0.5 | RBM |
| `AR_hat_c` | Southern Rough Cloth Hat | Cloth | T-1 | auto | 6 | 0 | 0 | 0 | 1.2 | OSA |
| `AR_hat_d` | Southern Cloth Hat | Cloth | T-1 | auto | 6 | 0 | 0 | 0 | 1.2 | OSA |
| `AR_hat_h` | Bedouin Straw Cap | Cloth | T-1 | auto | 6 | 0 | 0 | 0 | 1.2 | OSA |
| `aserai_civil_c_head` | Keffiyeh with Silken Band | Cloth | T-1 | auto | 6 | 0 | 0 | 0 | 0.1 | RBM |
| `aserai_civil_e_hscarf` | Colored Keffiyeh | Cloth | T-1 | auto | 6 | 0 | 0 | 0 | 0.1 | RBM |
| `aserai_civil_hscarf_a` | Keffiyeh with Brown Band | Cloth | T-1 | auto | 6 | 0 | 0 | 0 | 0.1 | RBM |
| `aserai_civil_hscarf_b` | Roughspun Keffiyeh | Cloth | T-1 | auto | 6 | 0 | 0 | 0 | 0.1 | RBM |
| `head_scarf` | Colored Hijab | Cloth | T-1 | auto | 6 | 0 | 0 | 0 | 0.1 | RBM |
| `mp_wrapped_desert_cap` | Wrapped Leather Cap | Cloth | T-1 | 1 | 6 | 0 | 0 | 0 | 0.5 | Vanilla |
| `mp_aserai_civil_d_hscarf` | Colored Turban | Cloth | T-1 | 1 | 4 | 0 | 0 | 0 | 0.1 | Vanilla |
| `ao_mabili_feathered_headband` | Southern Feathered Headband | Leather | T-1 | auto | 3 | 0 | 0 | 0 | 1 | OSA |
| `ao_mabili_leather_headband` | Southern Leather Headband | Leather | T-1 | auto | 3 | 0 | 0 | 0 | 1 | OSA |
| `mp_aserai_civil_c_head` | Keffiyeh with Silken Band | Cloth | T-1 | 1 | 2 | 0 | 0 | 0 | 0.1 | Vanilla |
| `mp_aserai_civil_e_hscarf` | Wrapped Keffiyeh | Cloth | T-1 | 1 | 2 | 0 | 0 | 0 | 0.1 | Vanilla |
| `mp_aserai_civil_hscarf_a` | Keffiyeh with Cloth Band | Cloth | T-1 | 1 | 2 | 0 | 0 | 0 | 0.1 | Vanilla |
| `mp_aserai_civil_hscarf_b` | Roughspun Keffiyeh | Cloth | T-1 | 1 | 2 | 0 | 0 | 0 | 0.1 | Vanilla |

<!-- CULTURE:nord COUNT:23 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `goggled_helmet_over_full_mail` | Closed Huscarl Helmet | Plate | T5 | auto | 114 | 74 | 40 | 0 | 4.4 | RBM |
| `closed_goggled_helmet` | Northern Closed Warlord Helmet | Plate | T5 | auto | 104 | 64 | 40 | 0 | 4.1 | RBM |
| `lendman_helmet_over_mail` | Warlord Helmet over Mail | Plate | T5 | auto | 119 | 24 | 20 | 0 | 1.9 | RBM |
| `nordic_helmet` | Closed Mail Goggled Helmet | Plate | T5 | auto | 86 | 40 | 40 | 0 | 4.1 | RBM |
| `nord_helmet_a` | Ornate Vendel Helmet | Plate | T5 | auto | 105 | 32 | 20 | 0 | 3.3 | RBM |
| `nord_helmet_b` | Vendel Noble Helmet | Plate | T5 | auto | 105 | 28 | 20 | 0 | 3.2 | RBM |
| `goggled_helmet_over_mail` | Goggled Helmet over Mail | Plate | T5 | auto | 106 | 24 | 20 | 0 | 1.8 | RBM |
| `nord_helmet_c` | Brass Vendel Helmet | Plate | T5 | auto | 90 | 30 | 20 | 0 | 3.2 | RBM |
| `northern_goggled_helmet` | Sea Raider Goggled Helmet | Plate | T5 | auto | 89 | 24 | 20 | 0 | 1.8 | RBM |
| `nord_helmet_d` | Nordic Spectacle Helmet | Plate | T5 | auto | 85 | 26 | 20 | 0 | 2.8 | RBM |
| `nord_helmet_e` | Nordic Nasal Helm over Mail | Plate | T5 | auto | 87 | 12 | 20 | 0 | 2.3 | RBM |
| `nord_helmet_da` | Nordic Spectacle Helm over Padded | Plate | T5 | auto | 73 | 26 | 8 | 0 | 2.5 | RBM |
| `improved_assassin_hood_q5` | Blackened Hood | Leather | T5 | auto | 40 | 28 | 40 | 0 | 0.4 | RBM |
| `nord_helmet_f` | Nordic Iron Helm over Mail | Plate | T5 | auto | 68 | 12 | 17 | 0 | 2.1 | RBM |
| `nord_helmet_ea` | Nordic Nasal Helmet | Plate | T5 | auto | 75 | 12 | 7 | 0 | 2.2 | RBM |
| `berserker_helmet_reinforced` | Berserker Reinforced Cloak | Plate | T5 | auto | 65 | 8 | 10 | 0 | 3 | RBM |
| `nord_helmet_fa` | Nordic Iron Helmet | Plate | T5 | auto | 55 | 12 | 7 | 0 | 2 | RBM |
| `assassin_hood` | Blackened Hood | Leather | T5 | auto | 30 | 18 | 30 | 0 | 0.4 | RBM |
| `nasal_helmet` | Steel Cap with Nose Guard | Plate | T5 | auto | 50 | 12 | 0 | 0 | 1.8 | RBM |
| `berserker_helmet` | Berserker Helmet | Leather | T5 | auto | 35 | 0 | 10 | 0 | 2 | RBM |
| `nordic_civilian_hat` | Nordic Civilian Hat | Cloth | T0 | auto | 7 | 0 | 0 | 0 | 0.5 | RBM |
| `nordic_civilian_hat_fur_brim` | Nordic Civilian Hat with Fur Brim | Cloth | T0 | auto | 7 | 0 | 0 | 0 | 0.6 | RBM |
| `nord_king_crown` | Northern Majestic Crown | Plate | T-1 | auto | 4 | 0 | 0 | 0 | 0.3 | RBM |

<!-- CULTURE:looters COUNT:2 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `vlandia_bandit_cape_b` | Hide Cap | Leather | T3 | auto | 32 | 0 | 0 | 0 | 2.5 | RBM |
| `vlandia_bandit_cape_a` | Rough Padded Cap | Cloth | T2 | auto | 26 | 0 | 0 | 0 | 2.5 | RBM |

<!-- CULTURE:neutral_culture COUNT:116 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `bandit_hybrid_armor_helmet_b` | Raider Reinforced Fur Helmet | Plate | T5 | auto | 65 | 12 | 12 | 0 | 2.1 | RBM |
| `tacky_bandit_helmet_c` | Serpenthide Plume Helmet | Plate | T5 | auto | 55 | 10 | 0 | 0 | 2 | RBM |
| `tacky_bandit_helmet_b` | Plumed Bandit Helmet | Plate | T5 | auto | 53 | 10 | 0 | 0 | 3 | RBM |
| `tacky_bandit_helmet` | Feathered Helmet | Plate | T5 | auto | 45 | 8 | 0 | 0 | 2.5 | RBM |
| `mp_goggled_helmet_over_full_mail` | Closed Huscarl Helmet | Chainmail | T5 | 1 | 51 | 0 | 0 | 0 | 4.4 | Vanilla |
| `mp_empire_battle_crown_west` | Imperial Plumed Helmet | Plate | T5 | 1 | 48 | 0 | 0 | 0 | 3.1 | Vanilla |
| `mp_full_helm_over_padded_cap` | Full Helm over Padded Cap | Plate | T5 | 1 | 48 | 0 | 0 | 0 | 4.8 | Vanilla |
| `bandit_hybrid_armor_helmet_a` | Raider Fur Helmet | Leather | T5 | auto | 25 | 12 | 12 | 0 | 0.8 | RBM |
| `mp_empire_battle_crown_north` | Jeweled Plumed Battle Crown | Plate | T5 | 1 | 45 | 0 | 0 | 0 | 2.1 | Vanilla |
| `mp_aserai_battle_crown` | Ornate Desert Battle Crown | Plate | T5 | 1 | 44 | 0 | 0 | 0 | 2.8 | Vanilla |
| `mp_aserai_crown` | Luxury Turban Over Helmet | Plate | T5 | 1 | 42 | 0 | 0 | 0 | 2.8 | Vanilla |
| `mp_closed_goggled_helmet` | Closed Goggled Helmet | Chainmail | T5 | 1 | 41 | 0 | 0 | 0 | 4 | Vanilla |
| `mp_battania_battle_crown` | Two Pronged Bronze Crown | Plate | T4 | 1 | 40 | 0 | 0 | 0 | 2.3 | Vanilla |
| `mp_full_helm_over_mail_coif` | Full Helm over Mail Coif | Plate | T4 | 1 | 40 | 0 | 0 | 0 | 4.7 | Vanilla |
| `mp_spiked_helmet_with_facemask` | Spiked Helmet with Facemask | Plate | T4 | 1 | 40 | 0 | 0 | 0 | 7 | Vanilla |
| `mp_sturgian_lord_helmet_c` | Decorated Helmet | Plate | T4 | 1 | 40 | 0 | 0 | 0 | 4.4 | Vanilla |
| `mp_lendman_helmet_over_full_mail` | Lendman Helmet over Full Mail | Chainmail | T4 | 1 | 39 | 0 | 0 | 0 | 4.3 | Vanilla |
| `mp_western_crowned_plated_helmet` | Crowned Plated Helmet | Plate | T4 | 1 | 39 | 0 | 0 | 0 | 3.9 | Vanilla |
| `mp_eastern_vendel_helmet` | Steppe Raider Helmet | Chainmail | T4 | 1 | 38 | 0 | 0 | 0 | 6.7 | Vanilla |
| `mp_imperial_goggled_helmet` | Imperial Goggled Helmet | Chainmail | T4 | 1 | 38 | 0 | 0 | 0 | 4.2 | Vanilla |
| `mp_decorated_goggled_helmet` | Decorated Goggled Helmet | Chainmail | T4 | 1 | 37 | 0 | 0 | 0 | 4.2 | Vanilla |
| `mp_nordic_helmet` | Northlander Helmet | Chainmail | T4 | 1 | 37 | 0 | 0 | 0 | 4.1 | Vanilla |
| `mp_khuzait_battle_crown` | Khuzait Battle Crown | Plate | T4 | 1 | 36 | 0 | 0 | 0 | 2.7 | Vanilla |
| `mp_leatherlame_feathered_spangenhelm_over_mail` | Feathered Spangenhelm over Mail | Plate | T4 | 1 | 36 | 0 | 0 | 0 | 3.4 | Vanilla |
| `mp_visored_helmet_over_mail_coif` | Visored Helmet over Mail Coif | Plate | T4 | 1 | 36 | 0 | 0 | 0 | 1.3 | Vanilla |
| `mp_battanian_noble_helmet_with_feather` | Highland Noble Helmet with Feather | Plate | T4 | 1 | 35 | 0 | 0 | 0 | 3.1 | Vanilla |
| `mp_ironlame_feathered_spangenhelm_over_mail` | Iron Feathered Spangenhelm over Mail | Plate | T4 | 1 | 35 | 0 | 0 | 0 | 3.3 | Vanilla |
| `mp_sturgian_lord_helmet_b` | Warlord Helmet with Nose Guard | Plate | T4 | 1 | 35 | 0 | 0 | 0 | 3.4 | Vanilla |
| `mp_closed_desert_helmet_with_mail` | Closed Desert Helmet with Mail | Chainmail | T3 | 1 | 34 | 0 | 0 | 0 | 3.2 | Vanilla |
| `mp_ironlame_feathered_spangenhelm` | Ironlame Feathered Spangenhelm | Plate | T3 | 1 | 34 | 0 | 0 | 0 | 3.2 | Vanilla |
| `mp_leather_studdedhelm_over_roughscale` | Leather Studded Helm over Roughscale | Leather | T3 | 1 | 34 | 0 | 0 | 0 | 4 | Vanilla |
| `mp_segmented_skullcap_over_mail_coif` | Segmented Skullcap over Mail Coif | Plate | T3 | 1 | 34 | 0 | 0 | 0 | 3.5 | Vanilla |
| `mp_emirs_helmet` | Emir Helmet | Chainmail | T3 | 1 | 33 | 0 | 0 | 0 | 3 | Vanilla |
| `mp_feathered_spangenhelm_over_imperial_coif` | Feathered Spangenhelm over Coif | Plate | T3 | 1 | 33 | 0 | 0 | 0 | 6 | Vanilla |
| `mp_helmet_with_faceguard` | Helmet with Faceguard | Plate | T3 | 1 | 33 | 0 | 0 | 0 | 5 | Vanilla |
| `mp_plumed_lamellar_helmet` | Plumed Lamellar Helmet | Plate | T3 | 1 | 33 | 0 | 0 | 0 | 3.3 | Vanilla |
| `mp_battanian_plated_noble_helmet` | Highland Plated Noble Helmet | Plate | T3 | 1 | 32 | 0 | 0 | 0 | 2.9 | Vanilla |
| `mp_kettle_hat_over_mail_coif` | Kettle Hat over Mail Coif | Chainmail | T3 | 1 | 32 | 0 | 0 | 0 | 2.9 | Vanilla |
| `mp_lendman_helmet_over_mail` | Raider Helmet over Mail | Plate | T3 | 1 | 32 | 0 | 0 | 0 | 9.1 | Vanilla |
| `mp_ridged_northernhelm` | Ridged Helm | Plate | T3 | 1 | 32 | 0 | 0 | 0 | 3.4 | Vanilla |
| `mp_segmented_cervelliere_over_mail_coif` | Segmented Cervelliere over Mail Coif | Plate | T3 | 1 | 32 | 0 | 0 | 0 | 3.2 | Vanilla |
| `mp_ironlame_feathered_spangenhelm_over_leather` | Ironlame Feathered Spangenhelm over Leather | Plate | T3 | 1 | 31 | 0 | 0 | 0 | 2.8 | Vanilla |
| `mp_northern_goggled_helmet` | Goggled Helmet | Plate | T3 | 1 | 31 | 0 | 0 | 0 | 1.8 | Vanilla |
| `mp_plumed_helmet` | Plumed Helmet | Plate | T3 | 1 | 31 | 0 | 0 | 0 | 4 | Vanilla |
| `mp_roughscale_helmet` | Roughscale Helmet | Plate | T3 | 1 | 31 | 0 | 0 | 0 | 3.2 | Vanilla |
| `mp_full_helm_over_laced_coif` | Full Helm over Laced Coif | Plate | T3 | 1 | 30 | 0 | 0 | 0 | 6.5 | Vanilla |
| `mp_goggled_helmet_over_mail` | Goggled Helmet over Mail | Plate | T3 | 1 | 30 | 0 | 0 | 0 | 1.8 | Vanilla |
| `mp_nasal_helmet_over_mail_coif` | Nasal Helmet over Mail Coif | Plate | T3 | 1 | 30 | 0 | 0 | 0 | 2.7 | Vanilla |
| `mp_peaked_helmet_over_mail_coif` | Peaked Helmet over Mail Coif | Plate | T3 | 1 | 30 | 0 | 0 | 0 | 2.7 | Vanilla |
| `mp_roundkettle_over_imperial_mail` | Roundkettle over Mail | Plate | T3 | 1 | 30 | 0 | 0 | 0 | 3.3 | Vanilla |
| `mp_pointed_skullcap_with_mail` | Pointed Skullcap with Mail | Plate | T3 | 1 | 29 | 0 | 0 | 0 | 2.8 | Vanilla |
| `mp_full_helm_over_cloth_headwrap` | Full Helm over Cloth Headwrap | Plate | T3 | 1 | 28 | 0 | 0 | 0 | 5 | Vanilla |
| `mp_kettle_helmet_over_arming_coif` | Kettle Helmet over Arming Coif | Plate | T3 | 1 | 28 | 0 | 0 | 0 | 1 | Vanilla |
| `mp_visored_helmet_over_padded_coif` | Visored Helmet over Padded Coif | Plate | T2 | 1 | 27 | 0 | 0 | 0 | 3.9 | Vanilla |
| `mp_full_helm_over_arming_coif` | Full Helm over Arming Coif | Plate | T2 | 1 | 26 | 0 | 0 | 0 | 6 | Vanilla |
| `mp_imperial_nasal_helm` | Legionary Helm | Plate | T2 | 1 | 21 | 0 | 0 | 0 | 1.2 | Vanilla |
| `mp_leatherlame_nasalhelm_over_imperial_leather` | Nasalhelm over Leather | Plate | T1 | 1 | 20 | 0 | 0 | 0 | 1.9 | Vanilla |
| `mp_closed_desert_helmet` | Closed Desert Helmet | Plate | T1 | 1 | 19 | 0 | 0 | 0 | 1.4 | Vanilla |
| `mp_goggled_helmet_over_leather` | Goggled Helmet over Leather | Plate | T1 | 1 | 19 | 0 | 0 | 0 | 1.4 | Vanilla |
| `mp_segmented_skullcap_over_padded_coif` | Segmented Skullcap over Padded Coif | Plate | T1 | 1 | 19 | 0 | 0 | 0 | 1.3 | Vanilla |
| `mp_tall_helmet` | Tall Helmet | Plate | T1 | 1 | 19 | 0 | 0 | 0 | 1.2 | Vanilla |
| `mp_battania_fur_helmet` | Highland Hide Helmet | Leather | T1 | 1 | 18 | 0 | 0 | 0 | 1.3 | Vanilla |
| `mp_open_desert_helmet` | Open Desert Helmet | Plate | T1 | 1 | 18 | 0 | 0 | 0 | 1.3 | Vanilla |
| `mp_plumed_nomad_helmet` | Plumed Nomad Helmet | Plate | T1 | 1 | 18 | 0 | 0 | 0 | 1.1 | Vanilla |
| `mp_segmented_skullcap_over_laced_coif` | Segmented Skullcap over Laced Coif | Plate | T1 | 1 | 18 | 0 | 0 | 0 | 1.2 | Vanilla |
| `mp_steppe_helmet` | Steppe Helmet | Plate | T1 | 1 | 18 | 0 | 0 | 0 | 5 | Vanilla |
| `mp_kettle_helmet_over_laced_coif` | Kettle Helmet over Laced Coif | Plate | T1 | 1 | 17 | 0 | 0 | 0 | 1.2 | Vanilla |
| `mp_nomad_helmet` | Nomad Helmet | Plate | T1 | 1 | 17 | 0 | 0 | 0 | 2.1 | Vanilla |
| `mp_segmented_skullcap_over_padded_cloth` | Segmented Skullcap over Padded Cloth | Plate | T1 | 1 | 17 | 0 | 0 | 0 | 0.5 | Vanilla |
| `mp_kettle_hat_over_padded_cloth` | Kettle Hat over Padded Cloth | Plate | T1 | 1 | 16 | 0 | 0 | 0 | 0.2 | Vanilla |
| `mp_kettle_helmet_with_leather` | Kettle Helmet with Leather | Plate | T1 | 1 | 16 | 0 | 0 | 0 | 1 | Vanilla |
| `mp_nasal_helmet_over_laced_coif` | Nasal Helmet over Laced Coif | Plate | T1 | 1 | 16 | 0 | 0 | 0 | 6 | Vanilla |
| `mp_nasal_helmet_with_leather` | Nasal Helmet with Leather | Plate | T1 | 1 | 16 | 0 | 0 | 0 | 1.2 | Vanilla |
| `mp_nasalhelm_over_leather` | Nasalhelm over Leather | Plate | T1 | 1 | 16 | 0 | 0 | 0 | 1.4 | Vanilla |
| `mp_peaked_helmet_over_padded_cloth` | Peaked Helmet over Padded Cloth | Plate | T1 | 1 | 16 | 0 | 0 | 0 | 1.2 | Vanilla |
| `mp_vlandia_bandit_cape_a` | Rough Padded Cap | Plate | T1 | 1 | 16 | 0 | 0 | 0 | 2.5 | Vanilla |
| `mp_vlandia_bandit_cape_b` | Hide Cap | Plate | T1 | 1 | 16 | 0 | 0 | 0 | 2.5 | Vanilla |
| `mp_guards_kettle_over_laced_coif` | Kettle over Laced Coif | Plate | T1 | 1 | 15 | 0 | 0 | 0 | 1.4 | Vanilla |
| `mp_leather_studdedhelm_over_thinhide` | Leather Studded Helm over Thinhide | Leather | T1 | 1 | 15 | 0 | 0 | 0 | 1.2 | Vanilla |
| `mp_loose_wrapped_desert_helmet` | Loose Wrapped Desert Helmet | Plate | T1 | 1 | 15 | 0 | 0 | 0 | 1.2 | Vanilla |
| `mp_nasal_helmet_over_padded_cloth` | Nasal Helmet over Padded Cloth | Plate | T1 | 1 | 15 | 0 | 0 | 0 | 1.2 | Vanilla |
| `mp_plumed_fur_lined_helmet` | Plumed Fur Lined Helmet | Plate | T1 | 1 | 15 | 0 | 0 | 0 | 1.1 | Vanilla |
| `mp_roundkettle_over_imperial_leather` | Roundkettle over Leather | Plate | T1 | 1 | 15 | 0 | 0 | 0 | 5 | Vanilla |
| `mp_roundkettle_over_laced_cloth` | Roundkettle over Laced Cloth | Plate | T1 | 1 | 15 | 0 | 0 | 0 | 1.3 | Vanilla |
| `mp_spangenhelm_with_leather` | Spangenhelm with Leather | Plate | T1 | 1 | 15 | 0 | 0 | 0 | 1.2 | Vanilla |
| `mp_wolfhead` | Wolfhead | Leather | T1 | 1 | 15 | 0 | 0 | 0 | 1.3 | Vanilla |
| `mp_leather_studdedhelm_over_headcloth` | Leather Studded Helm over Headcloth | Leather | T1 | 1 | 14 | 0 | 0 | 0 | 1.1 | Vanilla |
| `mp_nasal_helmet` | Nasal Helmet | Plate | T1 | 1 | 14 | 0 | 0 | 0 | 1.1 | Vanilla |
| `mp_roundkettle_over_imperial_cloth` | Roundkettle over Cloth | Plate | T1 | 1 | 14 | 0 | 0 | 0 | 5 | Vanilla |
| `mp_peaked_fur_hood` | Peaked Fur Hood | Leather | T0 | 1 | 10 | 0 | 0 | 0 | 0.7 | Vanilla |
| `mp_closed_head_scarf` | Closed Head Scarf | Cloth | T0 | 1 | 9 | 0 | 0 | 0 | 0.6 | Vanilla |
| `mp_fur_hood` | Fur Hood | Leather | T0 | 1 | 9 | 0 | 0 | 0 | 0.6 | Vanilla |
| `mp_laced_coif` | Laced Coif | Cloth | T0 | 1 | 9 | 0 | 0 | 0 | 0.4 | Vanilla |
| `mp_leather_studdedhelm` | Leather Studded Helm | Leather | T0 | 1 | 9 | 0 | 0 | 0 | 0.5 | Vanilla |
| `mp_tight_head_scarf` | Tight Head Scarf | Cloth | T0 | 1 | 9 | 0 | 0 | 0 | 0.7 | Vanilla |
| `mp_leather_cap` | Leather Cap | Leather | T0 | 1 | 8 | 0 | 0 | 0 | 2 | Vanilla |
| `mp_open_head_scarf` | Open Head Scarf | Cloth | T0 | 1 | 8 | 0 | 0 | 0 | 0.6 | Vanilla |
| `mp_tied_head_wrapping` | Tied Head Wrapping | Cloth | T0 | 1 | 8 | 0 | 0 | 0 | 0.5 | Vanilla |
| `mp_upturned_fur_cap` | Upturned Fur Cap | Leather | T0 | 1 | 8 | 0 | 0 | 0 | 0.6 | Vanilla |
| `mp_turban` | Turban | Cloth | T0 | 1 | 7 | 0 | 0 | 0 | 0.5 | Vanilla |
| `merchants_hat` | Merchant Hat | Leather | T-1 | auto | 6 | 0 | 0 | 0 | 0.1 | RBM |
| `mp_eastern_cap` | Cap with Neck Flap | Cloth | T-1 | 1 | 5 | 0 | 0 | 0 | 4 | Vanilla |
| `mp_nordic_leather_cap` | Fur Cap with Ear Flaps | Leather | T-1 | 1 | 5 | 0 | 0 | 0 | 0.4 | Vanilla |
| `mp_padded_cap` | Padded Cap | Cloth | T-1 | 1 | 5 | 0 | 0 | 0 | 1.8 | Vanilla |
| `mp_roughhide_cap` | Roughhide Cap | Cloth | T-1 | 1 | 5 | 0 | 0 | 0 | 0.5 | Vanilla |
| `mp_thinhide_coif` | Thinhide Coif | Leather | T-1 | 1 | 5 | 0 | 0 | 0 | 0.5 | Vanilla |
| `mp_tuareg` | Tuareg | Cloth | T-1 | 1 | 5 | 0 | 0 | 0 | 0.6 | Vanilla |
| `mp_desert_cap` | Desert Cap | Cloth | T-1 | 1 | 4 | 0 | 0 | 0 | 2 | Vanilla |
| `mp_fur_hat` | Fur Hat | Leather | T-1 | 1 | 4 | 0 | 0 | 0 | 3 | Vanilla |
| `mp_northern_fur_cap` | Fur Cap | Leather | T-1 | 1 | 4 | 0 | 0 | 0 | 0.5 | Vanilla |
| `mp_desert_headdress` | Desert Headdress | Cloth | T-1 | 1 | 3 | 0 | 0 | 0 | 0.2 | Vanilla |
| `mp_imperial_cloth_coif` | Cloth Coif | Cloth | T-1 | 1 | 3 | 0 | 0 | 0 | 3.2 | Vanilla |
| `mp_imperial_leather_coif` | Leather Coif | Leather | T-1 | 1 | 3 | 0 | 0 | 0 | 0.3 | Vanilla |
| `mp_wrapped_headcloth` | Wrapped Headcloth | Cloth | T-1 | 1 | 3 | 0 | 0 | 0 | 0.2 | Vanilla |
| `mp_merchants_hat` | Merchant Hat | Leather | T-1 | 1 | 2 | 0 | 0 | 0 | 0.1 | Vanilla |
| `mp_pilgrim_hood` | Pilgrim Hood | Cloth | T-1 | 1 | 2 | 0 | 0 | 0 | 1 | Vanilla |

<!-- CULTURE:(none) COUNT:2 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `wolfhead_ulfhedinn` | Reinforced Wolf Head | Leather | T5 | auto | 62 | 7 | 9 | 0 | 3 | RBM |
| `mp_nasal_helmet_with_mail` | Nasal Helmet with Mail | Plate | T1 | 1 | 20 | 0 | 0 | 0 | 1.5 | Vanilla |



