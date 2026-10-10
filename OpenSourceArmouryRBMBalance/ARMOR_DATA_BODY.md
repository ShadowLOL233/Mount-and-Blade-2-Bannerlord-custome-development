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


<!-- SLOT:BodyArmor COUNT:855 -->

<!-- SOURCE_DIST -->
| Source | Count |
|---|---:|
| OSA | 388 |
| RBM | 274 |
| Vanilla | 193 |

<!-- TIER_DIST -->
| Tier | Count |
|---|---:|
| T5 | 305 |
| T4 | 111 |
| T3 | 82 |
| T2 | 90 |
| T1 | 119 |
| T0 | 83 |
| T-1 | 65 |

<!-- CULTURE:empire COUNT:129 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `imperial_scale_armor` | Heavy Scale Armor over Double Mail | Plate | T5 | auto | 0 | 135 | 67 | 122 | 35.5 | RBM |
| `lamellar_with_scale_skirt` | Heavy Lamellar over Mail with Scale Skirt | Plate | T5 | auto | 0 | 118 | 45 | 122 | 34 | RBM |
| `imperial_lamellar` | Light Lamellar over Mail Armor | Plate | T5 | auto | 0 | 79 | 45 | 78 | 28.5 | RBM |
| `imperial_lamellar_over_leather` | Luxury Lamellar Vest over Leather | Plate | T5 | auto | 0 | 88 | 35 | 30 | 15 | RBM |
| `legionary_mail` | Legionary Mail | Chainmail | T5 | auto | 0 | 55 | 44 | 49 | 10.5 | RBM |
| `empire_legion_b` | Ornate Legionary Scale Mail | Plate | T5 | auto | 0 | 85 | 20 | 30 | 28 | RBM |
| `empire_horseman_armor` | Cavalryman Mail Shirt | Chainmail | T5 | auto | 0 | 55 | 38 | 27 | 8.3 | RBM |
| `imperial_mail_vest` | Infantryman Mail Vest | Chainmail | T5 | auto | 0 | 45 | 39 | 34 | 9.8 | RBM |
| `empire_legion_a` | Decorated Legionary Mail | Chainmail | T5 | auto | 0 | 55 | 18 | 44 | 22 | RBM |
| `imperial_mail_over_stripped_leather` | Infantryman Mail over Striped Leather | Chainmail | T5 | auto | 0 | 46 | 30 | 27 | 10.2 | RBM |
| `imperial_mail_over_leather` | Infantryman Mail over Leather | Chainmail | T5 | auto | 0 | 47 | 33 | 22 | 8.6 | RBM |
| `TV_empire_armor_d` | Imperial Decorated Brass Heavy Scale Armour | Plate | T5 | auto | 0 | 52 | 16 | 23 | 24.1 | OSA |
| `woven_leather_coat` | Boarskin Leather Coat | Leather | T5 | auto | 0 | 30 | 30 | 30 | 5.1 | RBM |
| `ao_imperial_cataphracts_lamellar` | Imperial White Brass Lamellar Over Scale | Plate | T5 | auto | 0 | 55 | 16 | 18 | 23 | OSA |
| `ao_imperial_cataphracts_lamellar_b` | Imperial Brass Lamellar Over Scale | Plate | T5 | auto | 0 | 55 | 16 | 18 | 23 | OSA |
| `TV_empire_armor_m` | Imperial Long Scale Cuirass Over Mail | Plate | T5 | auto | 0 | 52 | 10 | 23 | 34 | OSA |
| `ao_imperial_southern_scale_armor_over_mail` | Imperial Heavy Scale Armor Over Mail | Plate | T5 | auto | 0 | 47 | 14 | 23 | 20 | OSA |
| `ao_imperial_southern_scale_armor_over_mail_b` | Imperial Heavy Brass Scale Armor Over Mail | Plate | T5 | auto | 0 | 47 | 14 | 23 | 20 | OSA |
| `eastern_studded_leather` | Cured Studded Leather Armor | Leather | T5 | auto | 0 | 32 | 27 | 24 | 3 | RBM |
| `AR_empire_scale_armor_c` | Imperial Long Steel Scale Cuirass Over Mail | Plate | T5 | auto | 0 | 52 | 10 | 20 | 20.1 | OSA |
| `AR_empire_scale_armor_d` | Imperial Long Brass Scale Cuirass Over Mail | Plate | T5 | auto | 0 | 52 | 10 | 20 | 20.1 | OSA |
| `AR_empire_armor_i` | Imperial Lamellar Cuirass Over Mailed Leather | Plate | T5 | auto | 0 | 52 | 10 | 18 | 20.1 | OSA |
| `empire_plate_vest_armor` | Ornate Scale Armor | Plate | T5 | auto | 0 | 37 | 12 | 30 | 16 | RBM |
| `basic_imperial_leather_armor` | Leather Armor | Leather | T5 | auto | 0 | 28 | 25 | 22 | 3.1 | RBM |
| `AR_empire_horseman_armor_a` | Imperial Breastplate Over Steel Scale | Plate | T5 | auto | 0 | 42 | 12 | 20 | 14 | OSA |
| `AR_empire_horseman_armor_a2` | Imperial Gilded Breastplate Over Brass Scale | Plate | T5 | auto | 0 | 42 | 12 | 20 | 14 | OSA |
| `AR_empire_scale_armor_f` | Imperial Decorated Scale Cuirass Over Stripped Cloth | Plate | T5 | auto | 0 | 50 | 10 | 12 | 15.1 | OSA |
| `AR_empire_scale_armor_g` | Imperial Decorated Brass Scale Cuirass Over Stripped Cloth | Plate | T5 | auto | 0 | 50 | 10 | 12 | 15.1 | OSA |
| `AR_empire_scale_armor_h` | Imperial Decorated Alternating Scale Cuirass Over Stripped Cloth | Plate | T5 | auto | 0 | 50 | 10 | 12 | 15.1 | OSA |
| `DZ_empire_armor_d` | Imperial Steel Scale Armor | Plate | T5 | auto | 0 | 47 | 12 | 12 | 20 | OSA |
| `DZ_empire_armor_d2` | Imperial Brass Scale Armor | Plate | T5 | auto | 0 | 47 | 12 | 12 | 20 | OSA |
| `empire_warrior_padded_armor_b` | Infantryman Gambeson over Leather Jacket | Cloth | T5 | auto | 0 | 28 | 16 | 27 | 1.8 | RBM |
| `ao_imperial_lamellar_vest_over_mail` | Imperial White Brass Lamellar Over Mail | Chainmail | T5 | auto | 0 | 46 | 12 | 12 | 14 | OSA |
| `ao_imperial_lamellar_vest_over_mail_b` | Imperial Brass Lamellar Over Mail | Chainmail | T5 | auto | 0 | 46 | 12 | 12 | 14 | OSA |
| `AR_empire_armor_h` | Imperial Lamellar Cuirass Over Stripped Cloth | Plate | T5 | auto | 0 | 48 | 10 | 12 | 15.1 | OSA |
| `empire_warrior_padded_armor_g` | Auxiliary Armor with Straps | Cloth | T5 | auto | 0 | 26 | 19 | 25 | 2 | RBM |
| `AR_empire_armor_j` | Imperial Legionary Mail With Strips | Chainmail | T5 | auto | 0 | 35 | 15 | 18 | 19 | OSA |
| `empire_warrior_padded_armor_f` | Auxiliary Armor | Cloth | T5 | auto | 0 | 25 | 18 | 23 | 1.9 | RBM |
| `mp_empire_legion_b` | Ornate Legionary Scale Mail | Plate | T5 | 1 | 0 | 36 | 8 | 18 | 28 | Vanilla |
| `TV_empire_armor_j` | Imperial Cavalry Lamellar Armor | Plate | T5 | auto | 0 | 40 | 10 | 11 | 8.3 | OSA |
| `AR_empire_brass_lamellar_a` | Imperial Brass Lamellar Over Leather | Plate | T5 | auto | 0 | 38 | 10 | 12 | 12 | OSA |
| `AR_empire_brass_lamellar_c` | Imperial White Brass Lamellar Over Leather | Plate | T5 | auto | 0 | 38 | 10 | 12 | 12 | OSA |
| `padded_cloth_with_strips` | Padded Cloth With Strips | Cloth | T5 | auto | 0 | 22 | 13 | 25 | 2.2 | RBM |
| `AR_empire_armor_r` | Imperial Explorator's Mail | Plate | T4 | auto | 0 | 26 | 16 | 16 | 9.5 | OSA |
| `AR_empire_armor_s` | Imperial Decorated Explorator's Mail | Plate | T4 | auto | 0 | 26 | 16 | 16 | 9.5 | OSA |
| `empire_warrior_padded_armor_c` | Infantryman Gambeson with Skirts | Cloth | T4 | auto | 0 | 22 | 13 | 23 | 2 | RBM |
| `AR_empire_scale_armor_a` | Imperial Brass Scale Cuirass Over Stripped Leather | Plate | T4 | auto | 0 | 36 | 8 | 12 | 15.1 | OSA |
| `AR_empire_scale_armor_b` | Imperial Steel Scale Cuirass Over Stripped Leather | Plate | T4 | auto | 0 | 36 | 8 | 12 | 15.1 | OSA |
| `leather_tunic` | Leather Tunic | Leather | T4 | auto | 0 | 18 | 15 | 23 | 2.1 | RBM |
| `mp_empire_legion_a` | Decorated Legionary Mail | Chainmail | T4 | 1 | 0 | 30 | 8 | 18 | 22 | Vanilla |
| `AR_empire_armor_c` | Imperial Decorated Mail Over Stripped Cloth | Chainmail | T4 | auto | 0 | 33 | 10 | 12 | 10.2 | OSA |
| `AR_empire_horseman_armor` | Imperial Breastplate Over Mail | Plate | T4 | auto | 0 | 34 | 8 | 12 | 9.5 | OSA |
| `AR_empire_horseman_armor_b` | Imperial Gilded Breastplate Over Mail | Plate | T4 | auto | 0 | 34 | 8 | 12 | 9.5 | OSA |
| `khuzait_leather_stitched` | Thick Brigandine Vest | Leather | T4 | auto | 0 | 22 | 10 | 22 | 2.5 | RBM |
| `empire_warrior_padded_armor_d` | Infantryman Gambeson with Straps | Cloth | T4 | auto | 0 | 22 | 11 | 19 | 1.6 | RBM |
| `tv_empire_armor_a` | Imperial Lamellar Vest Over Stripped Mail | Plate | T4 | auto | 0 | 36 | 6 | 10 | 15 | OSA |
| `TV_empire_armor_p` | Imperial Studded Leather Over Mail Shirt | Chainmail | T4 | auto | 0 | 30 | 10 | 12 | 3 | OSA |
| `empire_warrior_padded_armor_e` | Infantryman Long Gambeson | Cloth | T4 | auto | 0 | 18 | 15 | 18 | 1.5 | RBM |
| `empire_warrior_padded_armor_a` | Infantryman Gambeson | Cloth | T4 | auto | 0 | 18 | 14 | 18 | 2.1 | RBM |
| `imperial_padded_cloth` | Infantryman Rough Gambeson | Cloth | T3 | auto | 0 | 16 | 15 | 16 | 2.8 | RBM |
| `tv_empire_armor_b` | Imperial Gilded Lamellar Over Boiled Leather | Plate | T3 | auto | 0 | 33 | 6 | 7 | 15 | OSA |
| `TV_empire_armor_f` | Imperial Alternating Scale Cuirass Over Red Stripped Cloth | Plate | T3 | auto | 0 | 34 | 6 | 6 | 16.5 | OSA |
| `TV_empire_armor_g` | Imperial Brass Scale Cuirass Over White Stripped Cloth | Plate | T3 | auto | 0 | 34 | 6 | 6 | 16.5 | OSA |
| `TV_empire_armor_h` | Imperial Steel Scale Cuirass Over Stripped Leather | Plate | T3 | auto | 0 | 34 | 6 | 6 | 16.5 | OSA |
| `TV_empire_armor_i` | Imperial Auxiliary Lamellar Armor With Straps | Plate | T3 | auto | 0 | 33 | 4 | 8 | 2 | OSA |
| `AR_empire_scout_armor_b` | Imperial Breastplate Over Padded Cloth | Plate | T3 | auto | 0 | 30 | 6 | 8 | 6.2 | OSA |
| `ao_imperial_lamellar_vest` | Imperial Steel Lamellar Over Padded Cloth | Plate | T3 | auto | 0 | 28 | 6 | 9 | 7 | OSA |
| `ao_imperial_lamellar_vest_b` | Imperial Brass Lamellar Over Padded Cloth | Plate | T3 | auto | 0 | 28 | 6 | 9 | 7 | OSA |
| `AR_empire_horseman_armor_d` | Imperial Breastplate Over Leather | Plate | T3 | auto | 0 | 30 | 6 | 6 | 8.3 | OSA |
| `patched_gambeson` | Patched Gambeson | Cloth | T3 | auto | 0 | 14 | 12 | 14 | 2.4 | RBM |
| `TV_empire_armor_l` | Imperial Steel Lamellar Over Padded Coat | Plate | T3 | auto | 0 | 28 | 4 | 8 | 7 | OSA |
| `TV_empire_armor_l2` | Imperial Brass Lamellar Over Padded Coat | Plate | T3 | auto | 0 | 28 | 4 | 8 | 7 | OSA |
| `AR_empire_horseman_armor_c` | Imperial Cavalry Leather Armor | Chainmail | T3 | auto | 0 | 27 | 6 | 6 | 8.3 | OSA |
| `AR_empire_brass_lamellar_b` | Southern Brass Lamellar Over Padded Cloth | Plate | T2 | auto | 0 | 24 | 6 | 8 | 6.2 | OSA |
| `AR_empire_brass_lamellar_d` | Southern White Brass Lamellar Over Padded Cloth | Plate | T2 | auto | 0 | 24 | 6 | 8 | 6.2 | OSA |
| `AR_empire_armor_a` | Imperial Leather Subarmalis Over Tunic | Leather | T2 | auto | 0 | 22 | 6 | 6 | 3.1 | OSA |
| `AR_empire_armor_d` | Imperial Decorated Cloth Subarmalis | Cloth | T2 | auto | 0 | 16 | 6 | 12 | 2.1 | OSA |
| `AR_empire_scout_armor_a` | Imperial Cavalry Padded Cloth | Leather | T2 | auto | 0 | 20 | 6 | 8 | 6.2 | OSA |
| `AR_empire_scout_armor_d` | Imperial Breastplate Over Tunic | Plate | T2 | auto | 0 | 28 | 3 | 3 | 1.9 | OSA |
| `AR_empire_armor_e` | Imperial Cloth Subarmalis Over Tunic | Cloth | T2 | auto | 0 | 15 | 6 | 12 | 2.1 | OSA |
| `AR_huntress_armor_b` | Huntress' Iron Breastplate With Skirt | Plate | T2 | auto | 0 | 28 | 3 | 2 | 1.9 | OSA |
| `AR_aserai_leather_a` | Southern Leather Cuirass Over Padded Cloth | Plate | T2 | auto | 0 | 18 | 6 | 8 | 6.2 | OSA |
| `ao_imperial_light_leather` | Imperial Rugged Leather Armor | Leather | T2 | auto | 0 | 18 | 6 | 5 | 3.1 | OSA |
| `tunic_with_rolled_cloth` | Tunic with Rolled Cloth | Cloth | T2 | auto | 0 | 11 | 7 | 11 | 0.7 | RBM |
| `AR_empire_armor_o` | Imperial Leather Cuirass Over Tunic With Pteruges | Leather | T1 | auto | 0 | 18 | 3 | 4 | 0.6 | OSA |
| `mp_imperial_mail_vest` | Infantryman Mail Vest | Chainmail | T1 | 1 | 0 | 23 | 1 | 1 | 7.8 | Vanilla |
| `footmans_tunic` | Military Tunic | Cloth | T1 | auto | 0 | 8 | 8 | 8 | 0.6 | RBM |
| `mp_padded_cloth_with_strips` | Padded Cloth with Strips | Cloth | T1 | 1 | 0 | 10 | 2 | 11 | 2.2 | Vanilla |
| `imperial_robes` | Toga | Cloth | T1 | auto | 0 | 8 | 6 | 8 | 0.9 | RBM |
| `AR_huntress_armor_c` | Huntress' Open Leather Vest With Skirt | Leather | T1 | auto | 0 | 16 | 3 | 2 | 1.9 | OSA |
| `mp_leather_tunic` | Leather Tunic | Leather | T1 | 1 | 0 | 12 | 2 | 7 | 2.1 | Vanilla |
| `tied_cloth_tunic` | Tied Cloth Tunic | Cloth | T1 | auto | 0 | 8 | 5 | 8 | 0.4 | RBM |
| `tunic_with_shoulder_pads` | Tunic with Shoulder Pads | Cloth | T1 | auto | 0 | 7 | 7 | 7 | 0.6 | RBM |
| `AR_empire_dress_armor_a` | Imperial Dress With Leather Vest | Leather | T1 | auto | 0 | 16 | 2 | 2 | 1.9 | OSA |
| `AR_empire_scout_armor_c` | Imperial Cavalry Tunic | Cloth | T1 | auto | 0 | 14 | 3 | 3 | 1.9 | OSA |
| `empire_dress_b` | Red Dress | Cloth | T1 | auto | 0 | 7 | 6 | 7 | 0.6 | RBM |
| `AR_huntress_armor_a` | Huntress' Leather Breastplate With Skirt | Cloth | T1 | auto | 0 | 14 | 3 | 2 | 1.9 | OSA |
| `mp_empire_warrior_padded_armor_f` | Auxiliary Armor | Cloth | T1 | 1 | 0 | 11 | 2 | 6 | 1.9 | Vanilla |
| `peasant_costume` | Commoner Clothes | Cloth | T1 | auto | 0 | 7 | 5 | 7 | 0.5 | RBM |
| `vlandian_dress` | Rich Dress | Cloth | T1 | auto | 0 | 7 | 5 | 7 | 0.5 | RBM |
| `empire_dress` | Ladies Dress | Cloth | T0 | auto | 0 | 6 | 6 | 5 | 0.7 | RBM |
| `mp_empire_warrior_padded_armor_g` | Auxiliary Armor with Straps | Cloth | T0 | 1 | 0 | 11 | 1 | 5 | 2 | Vanilla |
| `fine_town_tunic` | Rich Tunic | Cloth | T0 | auto | 0 | 6 | 5 | 5 | 0.4 | RBM |
| `mp_khuzait_leather_stitched` | Steppe Stitched Leather | Leather | T0 | 1 | 0 | 12 | 2 | 2 | 2.5 | Vanilla |
| `AR_empire_armor_b` | Imperial Toga With Tunic | Cloth | T0 | auto | 0 | 8 | 4 | 3 | 1.9 | OSA |
| `AR_empire_armor_m` | Imperial Robes | Cloth | T0 | auto | 0 | 8 | 4 | 3 | 0.9 | OSA |
| `AR_empire_armor_n` | Imperial White Robes | Cloth | T0 | auto | 0 | 8 | 4 | 3 | 0.9 | OSA |
| `AR_empire_armor_t` | Imperial Noble Robes | Cloth | T0 | auto | 0 | 8 | 4 | 3 | 0.9 | OSA |
| `AR_merchants_coat_a` | Merchant's Coat | Cloth | T0 | auto | 0 | 8 | 4 | 3 | 0.9 | OSA |
| `hemp_tunic` | Hemp Tunic | Cloth | T0 | auto | 0 | 5 | 5 | 5 | 0.4 | RBM |
| `mp_empire_warrior_padded_armor_e` | Infantryman Long Gambeson | Cloth | T0 | 1 | 0 | 9 | 1 | 5 | 1.8 | Vanilla |
| `mp_imperial_robes` | Toga | Cloth | T0 | 1 | 0 | 8 | 4 | 3 | 0.9 | Vanilla |
| `AR_empire_armor_p` | Palaic Chiton | Cloth | T0 | auto | 0 | 4 | 0 | 9 | 0.7 | OSA |
| `AR_empire_armor_k` | Tunic With Shoulder Pads And Pteruges | Cloth | T0 | auto | 0 | 6 | 2 | 3 | 0.6 | OSA |
| `empire_short_dress` | Short Tunic | Cloth | T0 | auto | 0 | 8 | 0 | 3 | 1 | RBM |
| `mp_patched_gambeson` | Patched Gambeson | Cloth | T0 | 1 | 0 | 8 | 1 | 1 | 3.6 | Vanilla |
| `AR_empire_armor_q` | Imperial Decorated Explorator's Tunic | Cloth | T-1 | auto | 0 | 6 | 1 | 1 | 0.4 | OSA |
| `AO_imperial_scouts_tunic` | Imperial Explorator's Tunic | Cloth | T-1 | auto | 0 | 5 | 1 | 1 | 0.4 | OSA |
| `AR_vlandia_armor_g` | Western Tunic With Clavi | Cloth | T-1 | auto | 0 | 4 | 1 | 2 | 0.4 | OSA |
| `plain_hemp_tunic` | Plain Hemp Tunic | Cloth | T-1 | auto | 0 | 4 | 1 | 2 | 0.4 | OSA |
| `mp_empire_short_dress` | Short Tunic | Cloth | T-1 | 1 | 0 | 6 | 0 | 0 | 1 | Vanilla |
| `TV_empire_armor_k` | Imperial Decorated Short Tunic | Cloth | T-1 | 120 | 0 | 6 | 0 | 0 | 1 | OSA |
| `AR_noble_dress_a` | Decorated Rich Dress | Cloth | T-1 | auto | 0 | 3 | 1 | 1 | 0.5 | OSA |
| `AR_noble_dress_a2` | Patterned Rich Dress | Cloth | T-1 | auto | 0 | 3 | 1 | 1 | 0.5 | OSA |
| `AR_empire_armor_f` | Imperial Skirted Trousers | Cloth | T-1 | auto | 0 | 1 | 0 | 3 | 0.4 | OSA |
| `mp_fine_town_tunic` | Rich Tunic | Cloth | T-1 | 1 | 0 | 2 | 1 | 1 | 0.4 | Vanilla |
| `AR_empire_armor_g` | Imperial Belted Skirt | Cloth | T-1 | auto | 0 | 1 | 0 | 2 | 0.4 | OSA |
| `mp_hemp_tunic` | Hemp Tunic | Cloth | T-1 | 1 | 0 | 1 | 1 | 1 | 0.4 | Vanilla |
| `mp_tied_cloth_tunic` | Tied Cloth Tunic | Cloth | T-1 | 1 | 0 | 1 | 1 | 1 | 0.4 | Vanilla |

<!-- CULTURE:vlandia COUNT:130 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `sturgian_fortified_armor` | Brigandine over Hauberk | Plate | T5 | auto | 0 | 100 | 100 | 95 | 26 | RBM |
| `plated_leather_coat` | Rough Brigandine | Plate | T5 | auto | 0 | 75 | 60 | 56 | 12.7 | RBM |
| `coat_of_plates_over_mail` | Brigandine over Mail | Plate | T5 | auto | 0 | 82 | 49 | 38 | 20.11 | RBM |
| `hauberk` | Double Mail Hauberk | Chainmail | T5 | auto | 0 | 77 | 44 | 44 | 12.3 | RBM |
| `red_coat_over_mail` | Red Tabard over Mail Hauberk | Chainmail | T5 | auto | 0 | 45 | 42 | 38 | 8.6 | RBM |
| `white_coat_over_mail` | White Tabard over Mail Hauberk | Chainmail | T5 | auto | 0 | 45 | 42 | 38 | 9.6 | RBM |
| `vlandia_chainmail` | Heavy Mail Hauberk | Chainmail | T5 | auto | 0 | 40 | 40 | 44 | 10 | RBM |
| `mail_shirt` | Mail Shirt | Chainmail | T5 | auto | 0 | 41 | 36 | 33 | 7.5 | RBM |
| `banded_leather_over_mail` | Banded Leather Over Mail | Chainmail | T5 | auto | 0 | 52 | 27 | 27 | 8.6 | RBM |
| `AR_vlandia_armor_b` | Western Heavy Padded Coat Over Mail And Lamellar | Plate | T5 | auto | 0 | 55 | 20 | 25 | 28.6 | OSA |
| `TV_vlandia_armor_t` | Western Long Steel Scale Coat Over Hauberk | Plate | T5 | auto | 0 | 52 | 20 | 25 | 26 | OSA |
| `TV_vlandia_armor_t2` | Western Long Bronze Scale Coat Over Hauberk | Plate | T5 | auto | 0 | 52 | 20 | 25 | 26 | OSA |
| `AR_vlandia_armor_u` | Western Reticulated Plate Over Scale Coat | Plate | T5 | auto | 0 | 57 | 16 | 20 | 21.4 | OSA |
| `AR_vlandia_scale_a` | Western Long Scale Vest Over Mail Hauberk | Plate | T5 | 47950 | 0 | 52 | 16 | 24 | 36.9 | OSA |
| `AR_vlandia_armor_a` | Western Heavy Padded Coat Over Mail | Plate | T5 | auto | 0 | 49 | 20 | 22 | 28.6 | OSA |
| `TV_vlandia_armor_r` | Western Heavy Padded Coat Over Hauberk | Chainmail | T5 | auto | 0 | 49 | 20 | 22 | 28.6 | OSA |
| `TV_empire_armor_n` | Western Scale Cuirass Over Mail With Scale Skirt | Plate | T5 | auto | 0 | 52 | 16 | 21 | 34 | OSA |
| `AR_vlandia_scale_b` | Western Rough Scale Vest Over Mail | Plate | T5 | auto | 0 | 50 | 16 | 22 | 15.1 | OSA |
| `padded_coat` | Padded Footman Coat | Cloth | T5 | auto | 0 | 28 | 26 | 32 | 2.4 | RBM |
| `veteran_mercenary_armor` | Fortified Mercenary Armor | Chainmail | T5 | auto | 0 | 37 | 13 | 33 | 8.2 | RBM |
| `AR_empire_scale_armor_e3` | Western Scale Cuirass Over Stripped Scale | Plate | T5 | auto | 0 | 52 | 10 | 20 | 20.1 | OSA |
| `AR_vlandia_armor_s` | Western Padded Vest Over Mailed Steel Scale | Plate | T5 | auto | 0 | 52 | 10 | 20 | 20.1 | OSA |
| `AR_vlandia_armor_s2` | Western Padded Vest Over Mailed Brass Scale | Plate | T5 | auto | 0 | 52 | 10 | 20 | 20.1 | OSA |
| `AR_vlandia_armor_h` | Western Reticulated Plate Over Long Hauberk | Plate | T5 | auto | 0 | 52 | 14 | 14 | 12 | OSA |
| `TV_vlandia_armor_g` | Western Mailled Brigandine With Decorated Tabard | Chainmail | T5 | auto | 0 | 50 | 14 | 16 | 23.2 | OSA |
| `TV_vlandia_armor_p` | Western Decorated Long Bronze Scale Cuirass | Plate | T5 | auto | 0 | 50 | 10 | 20 | 20.1 | OSA |
| `TV_vlandia_armor_q` | Western Decorated Long Steel Scale Cuirass | Plate | T5 | auto | 0 | 50 | 10 | 20 | 20.1 | OSA |
| `TV_vlandia_armor_s` | Western Long Steel Scale Coat | Plate | T5 | auto | 0 | 50 | 10 | 20 | 20.1 | OSA |
| `TV_vlandia_armor_s2` | Western Long Bronze Scale Coat | Plate | T5 | auto | 0 | 50 | 10 | 20 | 20.1 | OSA |
| `TV_empire_armor_o` | Western Scale Cuirass With Scale Skirt | Plate | T5 | auto | 0 | 52 | 4 | 20 | 34 | OSA |
| `TV_vlandia_armor_d` | Western Steel Scale Cuirass Over Chainmail | Chainmail | T5 | auto | 0 | 47 | 14 | 14 | 10 | OSA |
| `TV_vlandia_armor_d2` | Western Brass Scale Cuirass Over Chainmail | Chainmail | T5 | auto | 0 | 47 | 14 | 14 | 10 | OSA |
| `AR_empire_scale_armor_e` | Western Scale Cuirass Over Stripped Mail | Plate | T5 | auto | 0 | 52 | 10 | 12 | 21.5 | OSA |
| `AR_vlandia_lamellar_a` | Western Steel Lamellar Over Heavy Mail Hauberk | Plate | T5 | auto | 0 | 46 | 14 | 14 | 22 | OSA |
| `AR_vlandia_lamellar_a2` | Western Brass Lamellar Over Heavy Mail Hauberk | Plate | T5 | auto | 0 | 46 | 14 | 14 | 22 | OSA |
| `AR_vlandia_leather_a` | Western Leather Vest Over Mail Hauberk | Chainmail | T5 | auto | 0 | 40 | 12 | 22 | 21 | OSA |
| `TV_empire_armor_e` | Western Scale Over Mail Shirt | Plate | T5 | auto | 0 | 47 | 14 | 13 | 7.5 | OSA |
| `TV_empire_armor_e2` | Western Brass Scale Over Mail Shirt | Plate | T5 | auto | 0 | 47 | 14 | 13 | 7.5 | OSA |
| `TV_vlandia_armor_k` | Western Decorated Tunic Over Long Hauberk | Chainmail | T5 | auto | 0 | 40 | 12 | 22 | 21 | OSA |
| `TV_vlandia_armor_m` | Western Heavy Padded Mail Hauberk | Chainmail | T5 | auto | 0 | 40 | 12 | 22 | 21 | OSA |
| `TV_vlandia_armor_n` | Western Decorated Aketon Over Mail | Chainmail | T5 | auto | 0 | 40 | 12 | 22 | 21 | OSA |
| `AR_vlandia_armor_e` | Western Plated Mail Shirt | Chainmail | T5 | auto | 0 | 40 | 15 | 14 | 7.5 | OSA |
| `TV_vlandia_armor_f` | Western Padded Chainmail | Chainmail | T5 | auto | 0 | 38 | 14 | 16 | 12 | OSA |
| `leather_scale_armor` | Leather Scale Armor | Leather | T5 | auto | 0 | 24 | 18 | 22 | 8.6 | RBM |
| `AR_vlandia_lamellar_b` | Western Leather Lamellar Over Heavy Mail Hauberk | Plate | T5 | auto | 0 | 35 | 14 | 14 | 16 | OSA |
| `AR_monk_robe_c` | Western Mailed Colored Robe | Chainmail | T5 | auto | 0 | 30 | 16 | 16 | 12 | OSA |
| `AR_monk_robe_c2` | Western Mailed Plain Robe | Chainmail | T5 | auto | 0 | 30 | 16 | 16 | 12 | OSA |
| `AR_monk_robe_d` | Western Mailed Black Robe | Chainmail | T5 | auto | 0 | 30 | 16 | 16 | 12 | OSA |
| `TV_monk_robe_b` | Western Mailled Patterned Robe | Chainmail | T5 | auto | 0 | 30 | 16 | 16 | 12 | OSA |
| `TV_monk_robe_d` | Western Mailled Patterned Belted Robe | Chainmail | T5 | auto | 0 | 30 | 16 | 16 | 12 | OSA |
| `TV_vlandia_armor_h` | Western Padded Double Mail Hauberk | Chainmail | T5 | auto | 0 | 32 | 14 | 16 | 9.5 | OSA |
| `TV_vlandia_armor_o` | Western Heavy Plated Leather Over Mail | Chainmail | T5 | auto | 0 | 34 | 12 | 16 | 8.6 | OSA |
| `AR_vlandia_armor_q` | Western Leather Cuirass Over Long Hauberk | Plate | T5 | auto | 0 | 33 | 14 | 14 | 12 | OSA |
| `AR_vlandia_armor_r2` | Western Leather Cuirass Over Stripped Mail | Plate | T5 | auto | 0 | 33 | 14 | 14 | 12 | OSA |
| `padded_short_coat` | Padded Short Coat | Cloth | T5 | auto | 0 | 23 | 18 | 19 | 2.2 | RBM |
| `ao_eorling_plates_over_gambeson` | Western Iron Scale Over Gambeson | Plate | T5 | auto | 0 | 32 | 15 | 12 | 14 | OSA |
| `LE_vlandia_armor_a` | Patterned Tabard Over Sloven Mail | Chainmail | T5 | auto | 0 | 35 | 12 | 12 | 15 | OSA |
| `LE_vlandia_armor_b` | Red Tabard Over Sloven Mail | Chainmail | T5 | auto | 0 | 35 | 12 | 12 | 15 | OSA |
| `ao_crude_mail_with_tunic` | Crude Mail Over Tunic | Plate | T4 | auto | 0 | 26 | 16 | 16 | 9.5 | OSA |
| `AR_empire_scale_armor_e2` | Western Scale Cuirass Over Stripped Gambeson | Plate | T4 | auto | 0 | 48 | 5 | 5 | 12 | OSA |
| `AR_vlandia_armor_c` | Western Tabard Over Mail Hauberk | Chainmail | T4 | auto | 0 | 34 | 12 | 12 | 15 | OSA |
| `AR_vlandia_armor_f` | Western Long Hauberk | Chainmail | T4 | auto | 0 | 30 | 14 | 14 | 12 | OSA |
| `AR_vlandia_armor_h2` | Western Reticulated Plate Over Aketon | Plate | T4 | auto | 0 | 48 | 5 | 5 | 12 | OSA |
| `sleeveless_padded_coat` | Sleeveless Padded Coat | Cloth | T4 | auto | 0 | 23 | 10 | 25 | 2.2 | RBM |
| `TV_vlandia_armor_a` | Western Leather Coat Over Mail | Chainmail | T4 | auto | 0 | 34 | 12 | 12 | 28.6 | OSA |
| `TV_vlandia_armor_c` | Western Patterned Tabard Over Mail Hauberk | Chainmail | T4 | auto | 0 | 34 | 12 | 12 | 9.6 | OSA |
| `TV_vlandia_armor_i` | Western Patterned Coat Over Mail | Chainmail | T4 | auto | 0 | 34 | 12 | 12 | 28.6 | OSA |
| `TV_vlandia_armor_i2` | Western Decorated Coat Over Mail | Chainmail | T4 | auto | 0 | 34 | 12 | 12 | 28.6 | OSA |
| `aketon` | Heavy Aketon | Cloth | T4 | auto | 0 | 19 | 19 | 19 | 2.4 | RBM |
| `AP_leather_tabard_a` | Western Leather Coat Over Hauberk | Chainmail | T4 | auto | 0 | 31 | 12 | 14 | 28.6 | OSA |
| `TV_vlandia_armor_b` | Western Padded Coat Over Hauberk | Chainmail | T4 | auto | 0 | 31 | 12 | 14 | 28.6 | OSA |
| `AR_vlandia_armor_p` | Western Padded Vest Over Steel Scale | Plate | T4 | auto | 0 | 36 | 8 | 12 | 15.1 | OSA |
| `AR_vlandia_armor_p2` | Western Padded Vest Over Brass Scale | Plate | T4 | auto | 0 | 36 | 8 | 12 | 15.1 | OSA |
| `AR_vlandia_armor_o` | Western Padded Vest Over Mail | Chainmail | T4 | auto | 0 | 33 | 10 | 12 | 10.2 | OSA |
| `leather_coat` | Leather Tunic | Leather | T4 | auto | 0 | 20 | 15 | 20 | 2.3 | RBM |
| `mercenary_mail_armor` | Western Mercenary Mail Armor | Chainmail | T4 | auto | 0 | 29 | 14 | 12 | 10.3 | OSA |
| `TV_vlandia_armor_l` | Western Mail Armor | Chainmail | T4 | auto | 0 | 29 | 14 | 12 | 10.3 | OSA |
| `gambeson_b` | Aketon | Cloth | T4 | auto | 0 | 18 | 18 | 18 | 2 | RBM |
| `sleeveless_padded_short_coat` | Sleeveless Padded Short Coat | Cloth | T4 | auto | 0 | 23 | 10 | 20 | 2 | RBM |
| `TV_vlandia_armor_h2` | Western Double Mail Hauberk | Chainmail | T4 | auto | 0 | 27 | 12 | 14 | 9.5 | OSA |
| `woven_leather_vest` | Woven Leather Vest | Leather | T4 | auto | 0 | 22 | 12 | 18 | 4.7 | RBM |
| `AP_leather_tabard_c` | Western Leather Short Coat Over Hauberk | Chainmail | T4 | auto | 0 | 31 | 12 | 8 | 28.6 | OSA |
| `AR_empire_armor_l` | Western Mail Shirt With Rolled Cloth | Chainmail | T4 | auto | 0 | 27 | 11 | 13 | 7.5 | OSA |
| `TV_vlandia_armor_b3` | Western Padded Short Coat Over Hauberk | Chainmail | T4 | auto | 0 | 31 | 12 | 8 | 28.6 | OSA |
| `leather_tabard_over_mail` | Western Sleeveless Leather Coat Over Hauberk | Chainmail | T4 | auto | 0 | 28 | 8 | 14 | 28.6 | OSA |
| `TV_vlandia_armor_b2` | Western Sleeveless Padded Coat Over Hauberk | Chainmail | T4 | auto | 0 | 28 | 8 | 14 | 28.6 | OSA |
| `AR_vlandia_armor_d` | Western Mail Shirt | Chainmail | T4 | auto | 0 | 25 | 11 | 13 | 7.5 | OSA |
| `AR_vlandia_armor_d2` | Western Chainmail Shirt | Chainmail | T4 | auto | 0 | 25 | 11 | 13 | 7.5 | OSA |
| `mp_veteran_mercenary_armor` | Fortified Mercenary Armor | Chainmail | T4 | 1 | 0 | 24 | 12 | 13 | 8.2 | Vanilla |
| `mp_red_coat_over_mail` | Red Coat over Mail | Chainmail | T3 | 1 | 0 | 24 | 11 | 13 | 8.2 | Vanilla |
| `TV_vlandia_armor_j` | Western Plated Leather Over Mail | Chainmail | T3 | auto | 0 | 29 | 8 | 10 | 8.6 | OSA |
| `AP_leather_tabard_d` | Western Sleeveless Leather Short Coat Over Hauberk | Chainmail | T3 | auto | 0 | 28 | 8 | 8 | 28.6 | OSA |
| `TV_vlandia_armor_b4` | Western Sleeveless Padded Short Coat Over Hauberk | Chainmail | T3 | auto | 0 | 28 | 8 | 8 | 28.6 | OSA |
| `leather_coat_over_cloth` | Leather Coat Over Cloth | Cloth | T3 | auto | 0 | 18 | 7 | 18 | 0.7 | RBM |
| `padded_leather_shirt` | Padded Leather Shirt | Leather | T3 | auto | 0 | 16 | 11 | 16 | 1.8 | RBM |
| `AR_vlandia_armor_n` | Western Padded Vest Over Leather | Leather | T2 | auto | 0 | 22 | 6 | 6 | 3.1 | OSA |
| `AR_vlandia_armor_q2` | Western Leather Cuirass Over Aketon | Plate | T2 | auto | 0 | 24 | 5 | 5 | 12 | OSA |
| `AR_vlandia_armor_r` | Western Leather Cuirass Over Stripped Gambeson | Plate | T2 | auto | 0 | 24 | 5 | 5 | 12 | OSA |
| `mercenary_gambeson` | Western Mercenary Gambeson | Cloth | T2 | auto | 0 | 16 | 6 | 8 | 8.3 | OSA |
| `AO_vlandia_armor_c` | Western Leather Tabard Over Aketon | Leather | T1 | auto | 0 | 20 | 3 | 3 | 3.4 | OSA |
| `AR_vlandia_armor_k` | Western Sleeveless Padded Gambeson | Cloth | T1 | auto | 0 | 14 | 2 | 9 | 2.2 | OSA |
| `AR_vlandia_armor_a2` | Western Heavy Padded Coat | Cloth | T1 | auto | 0 | 14 | 5 | 5 | 2.4 | OSA |
| `AR_vlandia_armor_j` | Western Padded Gambeson | Cloth | T1 | auto | 0 | 14 | 5 | 5 | 2.4 | OSA |
| `ao_vlandian_tabard_over_aketon` | Western Tabard Over Aketon | Cloth | T1 | auto | 0 | 16 | 3 | 3 | 3.4 | OSA |
| `ao_vlandian_tabard_over_aketon_b` | Western Plain Tabard Over Aketon | Cloth | T1 | auto | 0 | 16 | 3 | 3 | 3.4 | OSA |
| `long_hemp_tunic` | Long Hemp Tunic | Cloth | T1 | auto | 0 | 8 | 6 | 8 | 0.6 | RBM |
| `TV_vlandia_armor_e` | Western Patterned Tabard Over Aketon | Cloth | T1 | auto | 0 | 16 | 3 | 3 | 3.4 | OSA |
| `AR_vlandia_armor_l` | Western Short Padded Gambeson | Cloth | T1 | auto | 0 | 14 | 5 | 2 | 2.2 | OSA |
| `AR_vlandia_armor_m` | Western Sleeveless Short Padded Gambeson | Cloth | T1 | auto | 0 | 14 | 2 | 5 | 2 | OSA |
| `mp_padded_coat` | Padded Footman Coat | Cloth | T1 | 1 | 0 | 12 | 2 | 7 | 2.4 | Vanilla |
| `AR_vlandia_armor_t` | Western Cavalry Tunic | Cloth | T1 | auto | 0 | 14 | 3 | 3 | 1.9 | OSA |
| `mp_sleeveless_padded_coat` | Sleeveless Padded Coat | Cloth | T1 | 1 | 0 | 11 | 2 | 7 | 2.2 | Vanilla |
| `mp_leather_coat_over_cloth` | Leather Coat over Cloth | Cloth | T1 | 1 | 0 | 17 | 1 | 1 | 0.7 | Vanilla |
| `monk_robe` | Scholar Robe | Cloth | T0 | auto | 0 | 6 | 5 | 6 | 0.5 | RBM |
| `vlandian_corset_dress` | Corseted Dress | Cloth | T0 | auto | 0 | 6 | 5 | 6 | 0.5 | RBM |
| `vlandian_woman_dress` | Aproned Dress | Cloth | T0 | auto | 0 | 6 | 5 | 6 | 0.7 | RBM |
| `cloth_tunic` | Simple Cloth Tunic | Cloth | T0 | auto | 0 | 6 | 5 | 5 | 0.4 | RBM |
| `long_woolen_tunic` | Long Woolen Tunic | Cloth | T0 | auto | 0 | 6 | 5 | 5 | 0.4 | RBM |
| `sackcloth_tunic` | Sackcloth Tunic | Cloth | T0 | auto | 0 | 6 | 5 | 5 | 0.6 | RBM |
| `vlandian_noble_woman_dress` | Silken Noble Dress | Cloth | T0 | auto | 0 | 6 | 5 | 5 | 0.6 | RBM |
| `DZ_vlandia_armor_a` | Western Noble Robes | Cloth | T0 | auto | 0 | 6 | 3 | 3 | 2.7 | OSA |
| `AR_monk_robe_a` | Western Colored Robe | Cloth | T-1 | auto | 0 | 5 | 1 | 2 | 0.5 | OSA |
| `AR_monk_robe_a2` | Western Plain Robe | Cloth | T-1 | auto | 0 | 5 | 1 | 2 | 0.5 | OSA |
| `AR_monk_robe_b` | Western Brown Robe | Cloth | T-1 | auto | 0 | 5 | 1 | 2 | 0.5 | OSA |
| `TV_monk_robe_a` | Western Patterned Robe | Cloth | T-1 | auto | 0 | 5 | 1 | 2 | 0.5 | OSA |
| `TV_monk_robe_c` | Western Patterned Belted Robe | Cloth | T-1 | auto | 0 | 5 | 1 | 2 | 0.5 | OSA |
| `AR_vlandia_armor_i` | Western Long Woolen Tunic | Cloth | T-1 | auto | 0 | 2 | 1 | 1 | 0.4 | OSA |
| `mp_long_woolen_tunic` | Long Woolen Tunic | Cloth | T-1 | 1 | 0 | 2 | 1 | 1 | 0.4 | Vanilla |
| `TV_vlandia_armor_u` | Western Rich Tunic | Cloth | T-1 | auto | 0 | 2 | 1 | 1 | 0.4 | OSA |
| `mp_cloth_tunic` | Simple Cloth Tunic | Cloth | T-1 | 1 | 0 | 1 | 1 | 1 | 0.4 | Vanilla |

<!-- CULTURE:sturgia COUNT:96 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `sturgian_lamellar_fortified` | Heavy Lamellar over Hauberk | Plate | T5 | auto | 0 | 105 | 45 | 75 | 26 | RBM |
| `northern_brass_lamellar_over_mail` | Heavy Lamellar over Mail | Plate | T5 | auto | 0 | 90 | 45 | 50 | 23.6 | RBM |
| `sturgian_chainmale_longsleeve` | Decorated Double Hauberk | Chainmail | T5 | auto | 0 | 77 | 44 | 44 | 12.3 | RBM |
| `sturgian_lamellar_base` | Light Lamellar over Mail | Plate | T5 | auto | 0 | 79 | 40 | 38 | 21.5 | RBM |
| `northern_coat_of_plates` | Coat Of Plates | Plate | T5 | auto | 0 | 90 | 30 | 22 | 20 | RBM |
| `sturgian_lamellar_gambeson_heavy` | Heavy Lamellar Vest | Plate | T5 | auto | 0 | 73 | 25 | 40 | 8.3 | RBM |
| `nordic_sloven_over_mail` | Leather Tabard over Mail | Chainmail | T5 | auto | 0 | 47 | 41 | 22 | 8.4 | RBM |
| `decorated_nordic_hauberk` | Huscarl Decorated Mail Shirt | Chainmail | T5 | auto | 0 | 44 | 38 | 22 | 10.5 | RBM |
| `nordic_hauberk` | Huscarl Mail Shirt | Chainmail | T5 | auto | 0 | 44 | 38 | 22 | 8.3 | RBM |
| `TV_sturgia_armor_b` | Northern Alternating Scale Over Hauberk | Plate | T5 | auto | 0 | 52 | 25 | 25 | 26 | OSA |
| `TV_sturgia_armor_c` | Northern Brass Scale Over Hauberk | Plate | T5 | auto | 0 | 52 | 25 | 25 | 26 | OSA |
| `TV_sturgia_armor_d` | Northern Steel Scale Over Hauberk | Plate | T5 | auto | 0 | 52 | 25 | 25 | 26 | OSA |
| `sturgian_chainmale_shortsleeve` | Huscarl Hauberk | Chainmail | T5 | auto | 0 | 35 | 29 | 33 | 8.3 | RBM |
| `ao_eorling_bronze_scale_over_mail` | Northern Bronze Scale Coat Over Mail | Plate | T5 | auto | 0 | 45 | 27 | 22 | 16 | OSA |
| `ao_eorling_iron_scale_over_mail` | Northern Iron Scale Coat Over Mail | Plate | T5 | auto | 0 | 45 | 27 | 22 | 16 | OSA |
| `mp_sturgian_fortified_armor` | Brigandine over Hauberk | Plate | T5 | 1 | 0 | 49 | 20 | 25 | 28.6 | Vanilla |
| `nordic_lamellar_armor` | Bronze Scale Vest over Gambeson | Plate | T5 | auto | 0 | 55 | 15 | 24 | 15 | RBM |
| `TV_sturgia_armor_e` | Northern Heavy Lamellar Over Hauberk | Plate | T5 | auto | 0 | 50 | 15 | 26 | 28 | OSA |
| `TV_sturgia_armor_e2` | Northern Heavy Decorated Lamellar Over Hauberk | Plate | T5 | auto | 0 | 50 | 15 | 26 | 28 | OSA |
| `TV_sturgia_armor_e3` | Northern Heavy Gilded Lamellar Over Hauberk | Plate | T5 | auto | 0 | 50 | 15 | 26 | 28 | OSA |
| `mp_sturgian_lamellar_fortified` | Heavy Lamellar over Hauberk | Plate | T5 | 1 | 0 | 50 | 15 | 25 | 28 | Vanilla |
| `ao_sturgia_armor_d` | Northern Lamellar Coat Over Hauberk | Plate | T5 | auto | 0 | 50 | 14 | 25 | 16.5 | OSA |
| `ao_sturgia_armor_d2` | Northern Alternating Lamellar Coat Over Hauberk | Plate | T5 | auto | 0 | 50 | 14 | 25 | 16.5 | OSA |
| `leather_and_iron_plate_armor` | Leather Armor with Iron Plates | Plate | T5 | auto | 0 | 32 | 32 | 25 | 9.6 | RBM |
| `AR_sturgia_armor_f` | Northern Lamellar Coat Over Mail | Plate | T5 | auto | 0 | 50 | 7 | 21 | 24 | OSA |
| `AR_sturgia_armor_f2` | Northern Lamellar Coat Over Mail | Plate | T5 | auto | 0 | 50 | 7 | 21 | 24 | OSA |
| `mp_sturgian_lamellar_base` | Heavy Lamellar over Mail Armor | Plate | T5 | 1 | 0 | 50 | 7 | 21 | 24 | Vanilla |
| `TV_sturgia_armor_f` | Northern Lamellar Over Hauberk | Plate | T5 | auto | 0 | 50 | 7 | 20 | 21.5 | OSA |
| `TV_sturgia_armor_f2` | Northern Decorated Lamellar Over Hauberk | Plate | T5 | auto | 0 | 50 | 7 | 20 | 21.5 | OSA |
| `TV_sturgia_armor_f3` | Northern Gilded Lamellar Over Hauberk | Plate | T5 | auto | 0 | 50 | 7 | 20 | 21.5 | OSA |
| `AO_eorling_armor_d` | Northern Bronze Scale Vest Over Mail | Plate | T5 | auto | 0 | 42 | 15 | 18 | 16 | OSA |
| `AO_eorling_armor_d2` | Northern Iron Scale Vest Over Mail | Plate | T5 | auto | 0 | 42 | 15 | 18 | 16 | OSA |
| `nordic_sloven_leather` | Sloven Leather | Leather | T5 | auto | 0 | 25 | 25 | 25 | 2.9 | RBM |
| `AR_sturgia_armor_c` | Northern Leather Tabard Over Mail | Chainmail | T5 | auto | 0 | 40 | 12 | 22 | 21 | OSA |
| `nordic_sloven` | Leather Armor over Heavy Cloth | Leather | T5 | auto | 0 | 25 | 22 | 25 | 1.8 | RBM |
| `DZ_sturgia_armor_b` | Northern Lamellar Vest Over Mailled Gambeson | Plate | T5 | auto | 0 | 48 | 10 | 12 | 15.1 | OSA |
| `DZ_sturgia_armor_b2` | Northern Decorated Lamellar Vest Over Mailled Gambeson | Plate | T5 | auto | 0 | 48 | 10 | 12 | 15.1 | OSA |
| `DZ_sturgia_armor_b3` | Northern Gilded Lamellar Vest Over Mailled Gambeson | Plate | T5 | auto | 0 | 48 | 10 | 12 | 15.1 | OSA |
| `sturgian_lamellar_gambeson` | Short Lamellar Vest | Plate | T5 | auto | 0 | 40 | 15 | 15 | 6.2 | RBM |
| `ao_sturgian_lamellar_vest` | Northern Lamellar Vest Over Hauberk | Plate | T5 | auto | 0 | 42 | 12 | 14 | 16.5 | OSA |
| `ao_sturgian_lamellar_vest_b` | Northern Alternating Lamellar Vest Over Hauberk | Plate | T5 | auto | 0 | 42 | 12 | 14 | 16.5 | OSA |
| `northern_lamellar_armor` | Light Lamellar Shirt | Plate | T5 | auto | 0 | 36 | 12 | 20 | 1.9 | RBM |
| `ao_gen_gambeson_over_mail` | Northern Padded Coat Over Mail | Plate | T5 | auto | 0 | 32 | 16 | 16 | 10 | OSA |
| `mp_northern_brass_lamellar_over_mail` | Heavy Bronze Lamellar over Mail | Plate | T5 | 1 | 0 | 38 | 13 | 13 | 11.4 | Vanilla |
| `nordic_lamellar_vest` | Bronze Scale Vest | Plate | T5 | auto | 0 | 37 | 12 | 14 | 12 | RBM |
| `mp_sturgian_chainmale_longsleeve` | Decorated Hauberk | Chainmail | T5 | 1 | 0 | 32 | 15 | 15 | 14.5 | Vanilla |
| `TV_sturgia_armor_a` | Northern Double Hauberk | Chainmail | T5 | auto | 0 | 32 | 15 | 15 | 12.3 | OSA |
| `TV_sturgia_armor_a2` | Northern Decorated Double Hauberk | Chainmail | T5 | auto | 0 | 32 | 15 | 15 | 12.3 | OSA |
| `northern_leather_tabard` | Leather Tabard | Leather | T5 | auto | 0 | 26 | 12 | 22 | 1.7 | RBM |
| `AR_sturgia_armor_a` | Northern Padded Leather Vest Over Mail | Chainmail | T4 | auto | 0 | 33 | 10 | 12 | 10.2 | OSA |
| `layered_leather_tunic` | Layered Leather Tunic | Leather | T4 | auto | 0 | 20 | 14 | 21 | 1.2 | RBM |
| `mp_sturgian_lamellar_gambeson_heavy` | Heavy Lamellar Vest | Plate | T4 | 1 | 0 | 35 | 2 | 18 | 8.3 | Vanilla |
| `ao_sturgian_leather_over_mail` | Northern Leather Coat Over Hauberk | Chainmail | T4 | auto | 0 | 29 | 11 | 13 | 8.3 | OSA |
| `stitched_leather_vest` | Stitched Leather Vest | Leather | T4 | auto | 0 | 18 | 16 | 18 | 1.5 | RBM |
| `AR_sturgia_armor_e` | Northern Leather Coat Over Mail | Chainmail | T3 | auto | 0 | 26 | 10 | 12 | 8.3 | OSA |
| `AR_sturgia_armor_e2` | Northern Leather Coat Over Mail | Chainmail | T3 | auto | 0 | 26 | 10 | 12 | 8.3 | OSA |
| `DZ_sturgia_armor_a` | Northern Lamellar Vest Over Gambeson | Plate | T3 | auto | 0 | 37 | 2 | 8 | 9.5 | OSA |
| `DZ_sturgia_armor_a2` | Northern Decorated Lamellar Vest Over Gambeson | Plate | T3 | auto | 0 | 37 | 2 | 8 | 9.5 | OSA |
| `DZ_sturgia_armor_a3` | Northern Gilded Lamellar Vest Over Gambeson | Plate | T3 | auto | 0 | 37 | 2 | 8 | 9.5 | OSA |
| `mp_sturgian_chainmale_shortsleeve` | Huscarls Hauberk | Chainmail | T3 | 1 | 0 | 25 | 10 | 12 | 8.3 | Vanilla |
| `northern_padded_gambeson` | Short Gambeson over Tunic | Cloth | T3 | auto | 0 | 16 | 13 | 16 | 1.5 | RBM |
| `nordic_padded_cloth` | Reinforced Thick Tunic | Cloth | T3 | auto | 0 | 14 | 14 | 14 | 1.1 | RBM |
| `northern_padded_cloth` | Light Padded Armor | Cloth | T3 | auto | 0 | 14 | 14 | 14 | 1.6 | RBM |
| `AR_sturgia_armor_b` | Northern Padded Leather Vest | Leather | T2 | auto | 0 | 22 | 6 | 6 | 3.1 | OSA |
| `AR_sturgia_armor_d` | Northern Leather Coat | Leather | T2 | auto | 0 | 22 | 6 | 6 | 3.1 | OSA |
| `AR_sturgia_armor_d2` | Northern Leather Coat | Leather | T2 | auto | 0 | 22 | 6 | 6 | 3.1 | OSA |
| `ao_sturgian_leather_over_cloth` | Northern Leather Coat | Leather | T2 | auto | 0 | 18 | 6 | 9 | 4.4 | OSA |
| `light_tunic` | Light Tunic | Cloth | T2 | auto | 0 | 13 | 6 | 13 | 0.4 | RBM |
| `mp_sturgian_lamellar_gambeson` | Heavy Lamellar Vest | Plate | T2 | 1 | 0 | 28 | 2 | 2 | 6.2 | Vanilla |
| `northern_leather_vest` | Tunic with Leather Vest | Leather | T2 | auto | 0 | 13 | 7 | 11 | 0.9 | RBM |
| `mp_northern_lamellar_armor` | Cured Leather Lamellar Shirt | Cloth | T2 | 1 | 0 | 25 | 1 | 3 | 1.9 | Vanilla |
| `heavy_nordic_coat` | Sleeved Heavy Tunic | Cloth | T1 | auto | 0 | 11 | 5 | 11 | 0.9 | RBM |
| `heavy_nordic_tunic` | Heavy Tunic | Cloth | T1 | auto | 0 | 11 | 5 | 11 | 1 | RBM |
| `roughcloth_shortcoat` | Roughcloth Shortcoat | Cloth | T1 | auto | 0 | 9 | 5 | 9 | 0.4 | RBM |
| `decorated_short_tunic` | Decorated Thick Tunic | Cloth | T1 | auto | 0 | 8 | 6 | 8 | 0.6 | RBM |
| `fur_trimmed_tunic` | Fur Trimmed Tunic | Cloth | T1 | auto | 0 | 8 | 6 | 8 | 0.8 | RBM |
| `tundra_tunic` | Belted Tunic | Cloth | T1 | auto | 0 | 8 | 6 | 8 | 0.3 | RBM |
| `nordic_tunic` | Luxury Tunic | Cloth | T1 | auto | 0 | 7 | 5 | 7 | 0.6 | RBM |
| `northern_dress` | Cold Weather Dress | Cloth | T0 | auto | 0 | 6 | 5 | 6 | 0.7 | RBM |
| `dress_with_overall` | Dress | Cloth | T0 | auto | 0 | 6 | 5 | 5 | 0.5 | RBM |
| `fur_trimmed_short_tunic` | Fur Trimmed Short Tunic | Cloth | T0 | auto | 0 | 6 | 5 | 5 | 0.4 | RBM |
| `laced_dress` | Laced Dress | Cloth | T0 | auto | 0 | 6 | 5 | 5 | 0.6 | RBM |
| `mp_nordic_sloven_leather` | Sloven Leather | Leather | T0 | 1 | 0 | 12 | 2 | 2 | 2.9 | Vanilla |
| `northern_tunic` | Varyag Tunic | Cloth | T0 | auto | 0 | 6 | 5 | 5 | 0.4 | RBM |
| `plain_dress` | Plain Dress | Cloth | T0 | auto | 0 | 6 | 5 | 5 | 0.5 | RBM |
| `sturgia_light_tunic` | Long Tunic | Cloth | T0 | auto | 0 | 6 | 5 | 5 | 0.4 | RBM |
| `cut_dress_with_pouch` | Cut Dress with Pouch | Cloth | T0 | auto | 0 | 7 | 0 | 7 | 0.5 | RBM |
| `cut_dress` | Frilled Dress | Cloth | T0 | auto | 0 | 6 | 0 | 6 | 0.6 | RBM |
| `mp_sturgia_light_armor_c` | Fur Trimmed Tunic | Cloth | T0 | 1 | 0 | 6 | 3 | 3 | 0.8 | Vanilla |
| `mp_sturgia_light_armor_e` | Decorated Thick Tunic | Cloth | T0 | 1 | 0 | 6 | 3 | 3 | 0.6 | Vanilla |
| `mp_tundra_tunic` | Northlander Belted Tunic | Cloth | T0 | 1 | 0 | 9 | 1 | 1 | 0.3 | Vanilla |
| `kaftan` | Northern Kaftan | Cloth | T-1 | auto | 0 | 2 | 1 | 1 | 8.3 | OSA |
| `mp_northern_tunic` | Varyag Tunic | Cloth | T-1 | 1 | 0 | 2 | 1 | 1 | 0.4 | Vanilla |
| `mp_sturgia_light_armor_a` | Light Tunic | Cloth | T-1 | 1 | 0 | 2 | 1 | 1 | 0.4 | Vanilla |
| `mp_sturgia_light_armor_b` | Long Tunic | Cloth | T-1 | 1 | 0 | 2 | 1 | 1 | 0.4 | Vanilla |
| `mp_sturgia_light_armor_d` | Fur Trimmed Short Tunic | Cloth | T-1 | 1 | 0 | 2 | 1 | 1 | 0.4 | Vanilla |

<!-- CULTURE:battania COUNT:106 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `battania_warlord_armor` | Scale Warlord Armor | Plate | T5 | auto | 0 | 96 | 40 | 50 | 25 | RBM |
| `battanian_scale_armor_a` | Highland Scale Armor over Mail Armor | Plate | T5 | auto | 0 | 79 | 25 | 35 | 26 | RBM |
| `battania_noble_armor` | Highborn Mail Armor | Chainmail | T5 | auto | 0 | 66 | 33 | 33 | 13.6 | RBM |
| `western_scale_mail` | Rough Scale Mail | Plate | T5 | auto | 0 | 50 | 38 | 38 | 14 | RBM |
| `battania_mercenary_armor` | Luxury Brigandine | Chainmail | T5 | auto | 0 | 55 | 33 | 27 | 10.6 | RBM |
| `battania_brass_plate_armor` | Bronze Breastplate | Plate | T5 | auto | 0 | 70 | 20 | 20 | 10 | RBM |
| `battania_woodland_chainmail` | Heavy Mail Shirt with Wide Belt | Chainmail | T5 | auto | 0 | 45 | 38 | 27 | 10.3 | RBM |
| `kilt_over_plated_leather` | Kilt Over Plated Leather | Leather | T5 | auto | 0 | 48 | 12 | 48 | 10.8 | RBM |
| `battanian_scale_armor_b` | Highland Scale Armor Over Cloth | Plate | T5 | auto | 0 | 60 | 15 | 30 | 14.5 | RBM |
| `battanian_chainmail_armor_d` | Highland Reinforced Mail | Chainmail | T5 | auto | 0 | 51 | 16 | 22 | 16.6 | RBM |
| `battanian_chainmail_armor_c` | Highland Reinforced Mail Over Tartan | Chainmail | T5 | auto | 0 | 49 | 16 | 22 | 14.7 | RBM |
| `ao_eorling_plates_over_mail` | Western Iron Scale Over Mail | Plate | T5 | auto | 0 | 40 | 25 | 20 | 18 | OSA |
| `tv_battania_armor_c` | Highland Kilt Over Plated Ranger Mail | Chainmail | T5 | auto | 0 | 44 | 14 | 26 | 12.2 | OSA |
| `battanian_chainmail_armor_a` | Heavy Mail Vest | Chainmail | T5 | auto | 0 | 40 | 16 | 25 | 10.7 | RBM |
| `ranger_mail` | Ranger Mail | Chainmail | T5 | auto | 0 | 47 | 18 | 15 | 12 | RBM |
| `AR_battania_armor_b` | Highland Decorated Savage Scale Over Mail | Plate | T5 | auto | 0 | 50 | 12 | 14 | 20.1 | OSA |
| `ao_battanian_decorated_scale_armor` | Highland Decorated Bronze Scale Over Kilted Mail | Plate | T5 | auto | 0 | 44 | 16 | 15 | 22.4 | OSA |
| `ao_battanian_decorated_scale_armor_b` | Highland Decorated Iron Scale Over Kilted Mail | Plate | T5 | auto | 0 | 44 | 16 | 15 | 22.4 | OSA |
| `ao_battanian_decorated_scale_armor_c` | Highland Decorated Alternating Scale Over Kilted Mail | Plate | T5 | auto | 0 | 44 | 16 | 15 | 22.4 | OSA |
| `AR_battania_armor_b2` | Highland Savage Scale Over Mail | Plate | T5 | auto | 0 | 48 | 12 | 14 | 20.1 | OSA |
| `battanian_chainmail_armor_b` | Highland Mail Shirt | Chainmail | T5 | auto | 0 | 36 | 16 | 22 | 10.7 | RBM |
| `AR_battania_armor_n` | Highland Mailed Brass Scale Shirt With Rolled Cloth | Plate | T5 | auto | 0 | 42 | 16 | 14 | 22.4 | OSA |
| `AR_battania_armor_n2` | Highland Mailed Steel Scale Shirt With Rolled Cloth | Plate | T5 | auto | 0 | 42 | 16 | 14 | 22.4 | OSA |
| `tv_battania_warlord_armor_a` | Highland Bronze Warlord Armor | Plate | T5 | auto | 0 | 38 | 16 | 18 | 19 | OSA |
| `tv_battania_warlord_armor_b2` | Highland Heavy Steel Warlord Cuirass | Plate | T5 | auto | 0 | 38 | 16 | 18 | 25 | OSA |
| `tv_battania_warlord_armor_c2` | Highland Steel Warlord Cuirass | Plate | T5 | auto | 0 | 38 | 16 | 18 | 25 | OSA |
| `mp_battanian_scale_armor_a` | Highland Scale Armor over Mail Armor | Plate | T5 | 1 | 0 | 42 | 12 | 15 | 22.4 | Vanilla |
| `AR_battania_armor_a2` | Highland Savage Scale Over Mail Shirt | Plate | T5 | auto | 0 | 48 | 12 | 8 | 20.1 | OSA |
| `tv_battania_warlord_armor_b` | Highland Heavy Brass Warlord Cuirass | Plate | T5 | auto | 0 | 36 | 14 | 14 | 25 | OSA |
| `tv_battania_warlord_armor_c` | Highland Brass Warlord Cuirass | Plate | T5 | auto | 0 | 36 | 14 | 14 | 25 | OSA |
| `tv_battania_armor_b` | Highland Decorated Ranger Mail | Chainmail | T5 | auto | 0 | 36 | 10 | 15 | 12 | OSA |
| `ao_battanian_hauberk_with_kilt` | Highland Hauberk With Kilt | Chainmail | T4 | auto | 0 | 29 | 12 | 16 | 9.5 | OSA |
| `AR_battania_armor_m` | Highland Brass Scale Shirt With Rolled Cloth | Plate | T4 | auto | 0 | 36 | 8 | 12 | 15.1 | OSA |
| `AR_battania_armor_m2` | Highland Steel Scale Shirt With Rolled Cloth | Plate | T4 | auto | 0 | 36 | 8 | 12 | 15.1 | OSA |
| `battania_woodland_outfit` | Woodland Garments | Leather | T4 | auto | 0 | 20 | 14 | 22 | 1.2 | RBM |
| `AR_battania_armor_o` | Highland Padded Steel Breastplate | Plate | T4 | auto | 0 | 42 | 5 | 8 | 10 | OSA |
| `AR_battania_armor_o2` | Highland Padded Decorated Steel Breastplate | Plate | T4 | auto | 0 | 42 | 5 | 8 | 10 | OSA |
| `AR_battania_armor_q` | Highland Brass Breastplate Over Mail | Plate | T4 | auto | 0 | 34 | 8 | 12 | 9.5 | OSA |
| `AR_battania_armor_q2` | Highland Steel Breastplate Over Mail | Plate | T4 | auto | 0 | 34 | 8 | 12 | 9.5 | OSA |
| `AR_battania_armor_q3` | Highland Decorated Steel Breastplate Over Mail | Plate | T4 | auto | 0 | 34 | 8 | 12 | 9.5 | OSA |
| `scale_armor` | Rugged Scale Armor | Plate | T4 | auto | 0 | 34 | 0 | 20 | 10.7 | RBM |
| `ao_battanian_mail_shirt_with_kilt` | Highland Mail Shirt With Kilt | Chainmail | T4 | auto | 0 | 27 | 11 | 15 | 7.5 | OSA |
| `AR_battania_armor_f` | Highland Rough Mail Hauberk With Fur | Chainmail | T4 | auto | 0 | 27 | 12 | 14 | 9.5 | OSA |
| `battania_light_armor_a` | Highland Tartan with Chest Plate | Cloth | T4 | auto | 0 | 25 | 10 | 17 | 6.5 | RBM |
| `fur_armor` | Fur Armor | Leather | T4 | auto | 0 | 19 | 11 | 19 | 13.6 | RBM |
| `ao_battanian_alt_scale_over_tunic_with_kilt` | Highland Alternating Scale Armor With Kilt | Plate | T3 | auto | 0 | 35 | 3 | 10 | 11.4 | OSA |
| `ao_battanian_bronze_scale_over_tunic_with_kilt` | Highland Bronze Scale Armor With Kilt | Plate | T3 | auto | 0 | 35 | 3 | 10 | 11.4 | OSA |
| `ao_battanian_iron_scale_over_tunic_with_kilt` | Highland Iron Scale Armor With Kilt | Plate | T3 | auto | 0 | 35 | 3 | 10 | 11.4 | OSA |
| `AR_battania_armor_l` | Highland Mail Shirt With Rolled Cloth | Chainmail | T3 | auto | 0 | 27 | 8 | 13 | 7.5 | OSA |
| `mp_kilt_over_plated_leather` | Kilt over Plated Leather | Leather | T3 | 1 | 0 | 30 | 2 | 16 | 7.8 | Vanilla |
| `battania_light_armor_b` | Highland Tartan | Cloth | T3 | auto | 0 | 20 | 10 | 17 | 3.2 | RBM |
| `mp_battanian_chainmail_armor_d` | Highland Reinforced Fian Mail | Chainmail | T3 | 1 | 0 | 34 | 4 | 8 | 16.6 | Vanilla |
| `AR_battania_armor_a` | Highland Savage Scale Cuirass | Plate | T3 | auto | 0 | 35 | 2 | 8 | 15 | OSA |
| `mp_battanian_chainmail_armor_c` | Highland Reinforced Mail over Tartan | Chainmail | T3 | 1 | 0 | 32 | 4 | 8 | 14.7 | Vanilla |
| `AR_battania_armor_p` | Highland Brass Breastplate Over Tunic | Plate | T3 | auto | 0 | 32 | 5 | 6 | 10 | OSA |
| `AR_battania_armor_p2` | Highland Steel Breastplate Over Tunic | Plate | T3 | auto | 0 | 32 | 5 | 6 | 10 | OSA |
| `AR_battania_armor_p3` | Highland Decorated Steel Breastplate Over Tunic | Plate | T3 | auto | 0 | 32 | 5 | 6 | 10 | OSA |
| `long_fur_coat` | Long Fur Coat | Leather | T3 | auto | 0 | 16 | 10 | 16 | 2.6 | RBM |
| `mp_battanian_scale_armor_b` | Highland Scale Armor over Cloth | Plate | T3 | 1 | 0 | 31 | 3 | 6 | 11.4 | Vanilla |
| `ao_battanian_scale_armor` | Highland Rugged Scale Armor Over Tunic | Plate | T3 | auto | 0 | 24 | 6 | 9 | 10.7 | OSA |
| `tv_battania_armor_f` | Highland Plated Scale Vest | Plate | T3 | auto | 0 | 32 | 0 | 7 | 10.7 | OSA |
| `battanian_savage_armor` | Wildling Armor | Cloth | T2 | auto | 0 | 4 | 2 | 32 | 0.8 | RBM |
| `mp_battanian_chainmail_armor_a` | Heavy Mail Vest | Chainmail | T2 | 1 | 0 | 28 | 2 | 8 | 10.7 | Vanilla |
| `AR_battania_armor_j` | Highland Plated Mail Vest | Chainmail | T2 | auto | 0 | 26 | 0 | 10 | 1 | OSA |
| `highland_cloth` | Woodland Cloth Armor | Cloth | T2 | auto | 0 | 14 | 8 | 14 | 9 | RBM |
| `tv_battania_armor_a` | Highland Rugged Scale Vest | Plate | T2 | auto | 0 | 26 | 2 | 8 | 10.7 | OSA |
| `AR_battania_armor_i` | Highland Rugged Mail Vest | Chainmail | T2 | auto | 0 | 25 | 2 | 8 | 1 | OSA |
| `fur_armor_with_strap` | Fur Armor With Strap | Leather | T2 | auto | 0 | 14 | 9 | 12 | 1.6 | RBM |
| `ao_battanian_gambeson_with_kilt` | Highland Gambeson Over Tunic With Kilt | Cloth | T2 | auto | 0 | 19 | 5 | 8 | 1.5 | OSA |
| `battania_civil_b` | Villager Leather Tunic | Leather | T2 | auto | 0 | 13 | 7 | 11 | 0.4 | RBM |
| `mp_battanian_chainmail_armor_b` | Highland Mail Shirt | Chainmail | T2 | 1 | 0 | 23 | 4 | 4 | 10.7 | Vanilla |
| `battania_light_armor_c` | Highland Long Tartan | Chainmail | T2 | auto | 0 | 15 | 0 | 15 | 2.4 | RBM |
| `tv_battania_armor_e` | Highland Plated Leather Vest | Leather | T2 | auto | 0 | 22 | 0 | 7 | 0.8 | OSA |
| `armored_baggy_trunks` | Armored Baggy Trunks | Leather | T1 | auto | 0 | 2 | 1 | 24 | 0.3 | RBM |
| `AR_battania_armor_h` | Highland Rugged Leather Vest | Cloth | T1 | auto | 0 | 16 | 2 | 8 | 0.8 | OSA |
| `ao_battanian_leather_tunic_with_kilt` | Highland Leather Tunic With Kilt | Leather | T1 | auto | 0 | 16 | 2 | 7 | 2.1 | OSA |
| `battania_civil_c` | Townsman Luxury Coat | Leather | T1 | auto | 0 | 9 | 7 | 9 | 0.6 | RBM |
| `battania_light_armor_e` | Tartan Toga | Cloth | T1 | auto | 0 | 10 | 0 | 15 | 0.7 | RBM |
| `AR_battania_armor_g` | Highland Sleeveless Leather Tunic | Cloth | T1 | auto | 0 | 15 | 2 | 7 | 2 | OSA |
| `baggy_trunks` | Baggy Trunks | Leather | T1 | auto | 0 | 0 | 0 | 22 | 0.2 | RBM |
| `battania_dress_a` | Plaid Dress | Cloth | T1 | auto | 0 | 8 | 6 | 8 | 0.9 | RBM |
| `battania_dress_b` | Woodland Commoner Dress | Cloth | T1 | auto | 0 | 8 | 6 | 8 | 0.8 | RBM |
| `battania_dress_c` | Tartan Long Dress | Cloth | T1 | auto | 0 | 8 | 6 | 8 | 0.7 | RBM |
| `battania_female_civil_a` | Tartan Dress | Cloth | T1 | auto | 0 | 8 | 6 | 8 | 0.7 | RBM |
| `battania_female_civil_b` | Villager Thick Dress | Cloth | T1 | auto | 0 | 8 | 6 | 8 | 0.7 | RBM |
| `mp_scale_armor` | Rugged Scale Armor | Plate | T1 | 1 | 0 | 19 | 0 | 1 | 8.7 | Vanilla |
| `sleeveless_studded_fur_armor` | Sleeveless Studded Fur Armor | Leather | T1 | auto | 0 | 10 | 0 | 10 | 0.4 | RBM |
| `battania_civil_a` | Tartan Tunic | Cloth | T1 | auto | 0 | 7 | 5 | 7 | 0.6 | RBM |
| `burlap_waistcoat` | Burlap Waistcoat | Cloth | T1 | auto | 0 | 7 | 5 | 7 | 0.4 | RBM |
| `mp_battanian_savage_armor` | Savage Armor | Cloth | T1 | 1 | 0 | 10 | 3 | 6 | 0.8 | Vanilla |
| `mp_long_fur_coat` | Long Fur Coat | Leather | T0 | 1 | 0 | 8 | 2 | 6 | 2.6 | Vanilla |
| `battania_light_armor_d` | Highland Kilt over Tartan Trousers | Chainmail | T0 | auto | 0 | 0 | 0 | 15 | 0.8 | RBM |
| `mp_battania_light_armor_e` | Tartan Toga | Cloth | T0 | 1 | 0 | 4 | 0 | 9 | 0.7 | Vanilla |
| `mp_fur_armor_with_strap` | Fur Armor with Strap | Leather | T0 | 1 | 0 | 8 | 2 | 2 | 2.1 | Vanilla |
| `AR_battania_armor_k` | Highland Tartan Tunic with Rolled Cloth | Cloth | T0 | auto | 0 | 6 | 2 | 3 | 0.7 | OSA |
| `mp_battania_light_armor_c` | Highland Long Tartan | Chainmail | T0 | 1 | 0 | 7 | 0 | 3 | 2.4 | Vanilla |
| `ao_battanian_cloth_tunic_with_kilt` | Highland Cloth Tunic With Kilt | Cloth | T-1 | auto | 0 | 6 | 1 | 1 | 0.4 | OSA |
| `ao_battanian_woolen_tunic_with_kilt` | Highland Woolen Tunic With Kilt | Cloth | T-1 | auto | 0 | 6 | 1 | 1 | 0.4 | OSA |
| `mp_sleeveless_studded_fur_armor` | Sleeveless Studded Fur Armor | Leather | T-1 | 1 | 0 | 3 | 0 | 3 | 0.4 | Vanilla |
| `AR_battania_armor_c` | Highland Cloth Trousers | Cloth | T-1 | auto | 0 | 1 | 0 | 3 | 0.4 | OSA |
| `AR_battania_armor_d` | Highland Plaid Cloth Trousers | Cloth | T-1 | auto | 0 | 1 | 0 | 3 | 0.4 | OSA |
| `AR_battania_armor_e` | Highland Striped Cloth Trousers | Cloth | T-1 | auto | 0 | 1 | 0 | 3 | 0.4 | OSA |
| `mp_battania_civil_b` | Villager Leather Tunic | Cloth | T-1 | 1 | 0 | 2 | 1 | 1 | 0.4 | Vanilla |
| `mp_battania_civil_c` | Fur Coat over Tunic | Cloth | T-1 | 1 | 0 | 2 | 1 | 1 | 0.4 | Vanilla |
| `mp_battania_light_armor_d` | Highland Kilt over Tartan Trousers | Chainmail | T-1 | 1 | 0 | 0 | 0 | 4 | 0.8 | Vanilla |
| `mp_battania_civil_a` | Tartan Tunic | Cloth | T-1 | 1 | 0 | 1 | 1 | 1 | 0.6 | Vanilla |

<!-- CULTURE:khuzait COUNT:71 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `khuzait_heavy_armor` | Khan's Coat of Plates | Plate | T5 | auto | 0 | 100 | 80 | 55 | 26.5 | RBM |
| `brass_lamellar_over_mail` | Bronze Lamellar over Mail | Plate | T5 | auto | 0 | 100 | 44 | 44 | 23 | RBM |
| `eastern_lamellar_armor` | Lancer Lamellar Armor | Plate | T5 | auto | 0 | 75 | 25 | 50 | 16 | RBM |
| `eastern_plated_leather` | Mirrored Brigandine Armor | Plate | T5 | auto | 0 | 60 | 35 | 40 | 18 | RBM |
| `eastern_stitched_leather_coat` | Stitched Cured Leather Coat | Leather | T5 | auto | 0 | 32 | 25 | 38 | 13.9 | RBM |
| `studded_leather_over_aketon` | Studded Leather Over Aketon | Leather | T5 | auto | 0 | 32 | 27 | 32 | 3.8 | RBM |
| `AR_khuzait_armor_b` | Eastern Lamellar Over Mail | Plate | T5 | auto | 0 | 50 | 15 | 25 | 28 | OSA |
| `AR_aserai_armor_z` | Eastern Heavy Alternating Lamellar Armor | Plate | T5 | auto | 0 | 54 | 16 | 18 | 25.3 | OSA |
| `eastern_heavy_lamellar_armor` | Eastern Heavy Lamellar Armor | Plate | T5 | auto | 0 | 54 | 16 | 18 | 25.3 | OSA |
| `khuzait_lamellar_strapped` | Reinforced Leather Armor | Plate | T5 | auto | 0 | 40 | 15 | 32 | 15 | RBM |
| `ao_durkhan_lamellar_with_mail` | Eastern Heavy Lamellar With Mail | Plate | T5 | auto | 0 | 48 | 14 | 24 | 22.1 | OSA |
| `TV_khuzait_armor_k` | Eastern Stitched Lamellar Coat Over Mail | Plate | T5 | auto | 0 | 52 | 10 | 23 | 20 | OSA |
| `AR_khuzait_armor_a` | Eastern Plated Armor | Plate | T5 | auto | 0 | 45 | 16 | 17 | 18.6 | OSA |
| `TV_khuzait_armor_j` | Eastern Alternating Lamellar Coat Over Mail | Plate | T5 | auto | 0 | 46 | 10 | 20 | 20 | OSA |
| `TV_khuzait_armor_o` | Eastern Stitched Lamellar Vest Over Mail | Plate | T5 | auto | 0 | 52 | 10 | 12 | 20 | OSA |
| `ao_durkhan_nobles_lamellar` | Eastern Noble Lamellar | Plate | T5 | auto | 0 | 49 | 2 | 18 | 14 | OSA |
| `TV_khuzait_armor_n` | Eastern Alternating Lamellar Vest Over Mail | Plate | T5 | auto | 0 | 46 | 10 | 12 | 20 | OSA |
| `TV_khuzait_armor_u` | Eastern Blackened Leather Lamellar Over Mail | Chainmail | T5 | auto | 0 | 38 | 14 | 16 | 28 | OSA |
| `ao_durkhan_lamellar_thighguard` | Eastern Lamellar With Thighguard | Plate | T5 | auto | 0 | 40 | 2 | 25 | 10 | OSA |
| `leather_lamellar_armor` | Cured Leather Lamellar Armor | Leather | T5 | auto | 0 | 28 | 17 | 22 | 2.6 | RBM |
| `ao_durkhan_heavy_lamellar` | Eastern Heavy Lamellar | Plate | T5 | auto | 0 | 45 | 2 | 18 | 12 | OSA |
| `eastern_plated_leather_vest` | Brigandine Vest | Plate | T4 | auto | 0 | 34 | 12 | 12 | 10 | RBM |
| `reinforced_suede_armor` | Reinforced Suede Armor | Leather | T4 | auto | 0 | 24 | 10 | 23 | 2.3 | RBM |
| `khuzait_fortified_armor` | Reinforced Mirror Armor | Leather | T4 | auto | 0 | 22 | 13 | 21 | 2.4 | RBM |
| `TV_khuzait_armor_m` | Eastern Stitched Lamellar Coat | Plate | T4 | auto | 0 | 32 | 6 | 18 | 12.5 | OSA |
| `belted_leather_cuirass` | Belted Leather Cuirass | Leather | T4 | auto | 0 | 20 | 12 | 20 | 1.9 | RBM |
| `TV_khuzait_armor_e` | Eastern Stitched Leather Coat Over Mail | Chainmail | T4 | auto | 0 | 30 | 10 | 12 | 13.9 | OSA |
| `TV_khuzait_armor_f` | Eastern Stitched Leather Vest Over Mail | Chainmail | T4 | auto | 0 | 30 | 10 | 12 | 13.9 | OSA |
| `TV_khuzait_armor_s` | Eastern Studded Leather Coat Over Mail | Chainmail | T4 | auto | 0 | 30 | 10 | 12 | 3 | OSA |
| `TV_khuzait_armor_g` | Eastern Light Lamellar Armor | Plate | T4 | auto | 0 | 39 | 6 | 6 | 16 | OSA |
| `khuzait_sturdy_armor` | Mirrored Leather Armor | Leather | T4 | auto | 0 | 21 | 10 | 18 | 2.4 | RBM |
| `studded_steppe_leather` | Studded Leather Armor | Leather | T4 | auto | 0 | 18 | 12 | 19 | 2.4 | RBM |
| `crude_leather_armor` | Crude Hide Armor | Leather | T3 | auto | 0 | 16 | 16 | 16 | 2.4 | RBM |
| `TV_khuzait_armor_l` | Eastern Alternating Lamellar Coat | Plate | T3 | auto | 0 | 28 | 6 | 14 | 12.5 | OSA |
| `TV_khuzait_armor_q` | Eastern Stitched Lamellar Vest | Plate | T3 | auto | 0 | 32 | 6 | 6 | 12.5 | OSA |
| `TV_khuzait_armor_t` | Eastern Blackened Leather Lamellar Armor | Leather | T3 | auto | 0 | 24 | 6 | 12 | 14 | OSA |
| `TV_khuzait_armor_p` | Eastern Alternating Lamellar Vest | Plate | T3 | auto | 0 | 28 | 6 | 6 | 12.5 | OSA |
| `khuzait_belt_leather` | Coat with Mirrored Belt | Leather | T3 | auto | 0 | 15 | 12 | 12 | 2.4 | RBM |
| `khuzait_civil_coat_b` | Luxury Thick Coat | Leather | T3 | auto | 0 | 12 | 12 | 15 | 2.7 | RBM |
| `TV_khuzait_armor_h` | Eastern Blackened Leather Lamellar Vest | Leather | T2 | auto | 0 | 24 | 6 | 6 | 2.6 | OSA |
| `studded_leather_waistcoat` | Studded Leather Waistcoat | Leather | T2 | auto | 0 | 14 | 6 | 14 | 0.7 | RBM |
| `TV_khuzait_armor_b` | Eastern Strapped Iron Lamellar Armor | Plate | T2 | auto | 0 | 24 | 2 | 8 | 15 | OSA |
| `TV_khuzait_armor_r` | Eastern Studded Leather Coat | Leather | T2 | auto | 0 | 18 | 6 | 10 | 3 | OSA |
| `ao_durkhan_lamellar` | Eastern Lamellar | Plate | T2 | auto | 0 | 28 | 2 | 2 | 8 | OSA |
| `TV_khuzait_armor_i` | Eastern Stitched Leather Vest | Leather | T2 | auto | 0 | 20 | 6 | 6 | 8.4 | OSA |
| `khuzait_dress_b` | Dress with Fur | Cloth | T2 | auto | 0 | 13 | 5 | 13 | 0.9 | RBM |
| `khuzait_civil_coat` | Luxury Coat | Leather | T2 | auto | 0 | 12 | 6 | 12 | 2.7 | RBM |
| `TV_khuzait_armor_a` | Eastern Strapped Leather Armor | Plate | T2 | auto | 0 | 22 | 2 | 6 | 15 | OSA |
| `TV_khuzait_armor_d` | Eastern Light Strapped Iron Lamellar Armor | Plate | T2 | auto | 0 | 24 | 2 | 4 | 15 | OSA |
| `TV_khuzait_armor_d2` | Eastern Light Strapped Lamellar Armor | Plate | T2 | auto | 0 | 24 | 2 | 4 | 15 | OSA |
| `ao_durkhan_light_lamellar` | Eastern Light Lamellar | Plate | T1 | auto | 0 | 24 | 2 | 2 | 6.5 | OSA |
| `mp_khuzait_lamellar_strapped` | Reinforced Leather Armor | Plate | T1 | 1 | 0 | 24 | 2 | 2 | 7.3 | Vanilla |
| `TV_khuzait_armor_c` | Eastern Light Strapped Leather Armor | Plate | T1 | auto | 0 | 22 | 2 | 4 | 15 | OSA |
| `steppe_armor` | Striped Coat | Cloth | T1 | auto | 0 | 11 | 5 | 11 | 0.6 | RBM |
| `ao_khuzait_leather_vest` | Eastern Leather Vest Over Tunic | Leather | T1 | auto | 0 | 19 | 3 | 4 | 1.9 | OSA |
| `ao_durkhan_leather_lamellar` | Eastern Leather Lamellar | Leather | T1 | auto | 0 | 18 | 2 | 2 | 5.5 | OSA |
| `khuzait_dress` | Aproned Feminine Coat | Cloth | T1 | auto | 0 | 8 | 6 | 8 | 0.8 | RBM |
| `mp_khuzait_belt_leather` | Coat with Mirrored Belt | Leather | T1 | 1 | 0 | 12 | 2 | 7 | 2.4 | Vanilla |
| `mp_khuzait_fortified_armor` | Reinforced Mirror Armor | Leather | T1 | 1 | 0 | 12 | 2 | 7 | 2.4 | Vanilla |
| `mp_khuzait_sturdy_armor` | Sturdy Steppe Armor | Leather | T1 | 1 | 0 | 12 | 2 | 7 | 2.4 | Vanilla |
| `ao_durkhan_sleeveless_leather_lamellar` | Eastern Sleeveless Leather Lamellar | Leather | T1 | auto | 0 | 17 | 1 | 2 | 5.3 | OSA |
| `ao_durkhan_chestplate` | Eastern Leather Chestplate | Leather | T0 | auto | 0 | 15 | 1 | 2 | 4 | OSA |
| `ao_durkhan_topless_leather_lamellar` | Eastern Open Leather Lamellar | Leather | T0 | auto | 0 | 16 | 0 | 2 | 5 | OSA |
| `ao_durkhan_hide_poncho` | Eastern Hide Vest | Leather | T0 | auto | 0 | 10 | 1 | 5 | 1 | OSA |
| `steppe_dress` | Envelope Dress | Cloth | T0 | auto | 0 | 6 | 5 | 5 | 0.6 | RBM |
| `steppe_robe` | Steppe Robe | Cloth | T0 | auto | 0 | 6 | 5 | 5 | 0.4 | RBM |
| `DZ_khuzait_armor_b` | Eastern Decorated Coat With Belt | Leather | T0 | auto | 0 | 8 | 3 | 3 | 2.4 | OSA |
| `DZ_khuzait_armor_a` | Eastern Luxury Silk Robes | Leather | T0 | auto | 0 | 6 | 3 | 3 | 2.7 | OSA |
| `mp_khuzait_civil_coat_b` | Luxury Thick Coat | Leather | T-1 | 1 | 0 | 6 | 0 | 1 | 2.7 | Vanilla |
| `ao_durkhan_trousers` | Eastern Cloth Trousers | Cloth | T-1 | auto | 0 | 0 | 0 | 3 | 0.4 | OSA |
| `ao_durkhan_tunic` | Eastern Ragged Tunic | Cloth | T-1 | auto | 0 | 2 | 0 | 1 | 0.5 | OSA |

<!-- CULTURE:aserai COUNT:143 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `desert_scale_armor` | Scale Armor over Mail | Plate | T5 | auto | 0 | 95 | 45 | 68 | 25 | RBM |
| `aserai_full_scale_armor_on_chain` | Mastercrafted Southern Scale over Chain Hauberk | Plate | T5 | auto | 0 | 90 | 35 | 65 | 28 | RBM |
| `desert_lamellar` | Lamellar Armor over Mail | Plate | T5 | auto | 0 | 105 | 45 | 35 | 23 | RBM |
| `sturgia_cavalry_armor` | Cavalry Lamellar Armor | Plate | T5 | auto | 0 | 85 | 45 | 43 | 19.4 | RBM |
| `aserai_scale_armor_on_chain` | Mastercrafted Southern Scale over Chain Mail | Chainmail | T5 | auto | 0 | 93 | 38 | 33 | 22 | RBM |
| `aserai_archer_armor` | Gambeson over Mail Armor | Chainmail | T5 | auto | 0 | 60 | 55 | 44 | 9.3 | RBM |
| `aserai_chain_plate_armor_a` | Southern Reinforced Mail Armor | Chainmail | T5 | auto | 0 | 66 | 44 | 38 | 22 | RBM |
| `desert_robe_over_mail` | Robe Over Mail Hauberk | Chainmail | T5 | auto | 0 | 49 | 49 | 49 | 8.9 | RBM |
| `stitched_leather_over_mail` | Stitched Leather Over Mail | Chainmail | T5 | auto | 0 | 47 | 44 | 55 | 11.1 | RBM |
| `aserai_armor_02_b` | Southern Decorated Chain Mail | Chainmail | T5 | auto | 0 | 55 | 44 | 44 | 18 | RBM |
| `aserai_armor_02` | Southern Chain Mail | Chainmail | T5 | auto | 0 | 49 | 49 | 44 | 17 | RBM |
| `aserai_brass_plate_a` | Southern Mail Shirt with Brass Plates | Chainmail | T5 | auto | 0 | 55 | 46 | 37 | 18 | RBM |
| `aserai_robe_c_chain` | Robe over Mail Shirt | Chainmail | T5 | auto | 0 | 50 | 44 | 38 | 8.2 | RBM |
| `aserai_chain_plate_armor_d` | Southern Reinforced Short Mail Armor | Chainmail | T5 | auto | 0 | 66 | 44 | 13 | 18 | RBM |
| `aserai_chain_plate_armor_b` | Southern Reinforced Sleeveless Mail Armor | Chainmail | T5 | auto | 0 | 66 | 16 | 38 | 19 | RBM |
| `aserai_brass_plate_b` | Southern Sleeveless Mail Shirt with Brass Plates | Chainmail | T5 | auto | 0 | 55 | 13 | 37 | 15 | RBM |
| `aserai_scale_armor_on_cloth` | Mastercrafted Southern Scale Mail | Plate | T5 | auto | 0 | 70 | 15 | 20 | 18 | RBM |
| `ringed_desert_armor` | Heavy Ring Mail | Chainmail | T5 | auto | 0 | 38 | 33 | 31 | 7.5 | RBM |
| `AR_aserai_scale_armor_c` | Mastercrafted Southern Steel Scale over Chain Hauberk | Plate | T5 | auto | 0 | 51 | 16 | 25 | 28 | OSA |
| `aserai_chain_plate_armor_c` | Southern Reinforced Mail Vest | Chainmail | T5 | auto | 0 | 66 | 13 | 13 | 15 | RBM |
| `AR_aserai_armor_u` | Darshi Lamellar Over Stripped Mail | Plate | T5 | auto | 0 | 50 | 20 | 20 | 7 | OSA |
| `AR_aserai_armor_u2` | Darshi Alternating Lamellar Over Stripped Mail | Plate | T5 | auto | 0 | 50 | 20 | 20 | 7 | OSA |
| `southern_lamellar_armor` | Bronze Lamellar Vest over Gambeson | Plate | T5 | auto | 0 | 50 | 15 | 25 | 11 | RBM |
| `tv_empire_armor_s` | Darshi Stripped Lamellar With Mail | Plate | T5 | auto | 0 | 50 | 20 | 20 | 7 | OSA |
| `tv_empire_armor_s2` | Darshi Stripped Alternating Lamellar With Mail | Plate | T5 | auto | 0 | 50 | 20 | 20 | 7 | OSA |
| `tv_aserai_lamellar_e` | Southern Heavy Steel Lamellar Over Mail | Plate | T5 | auto | 0 | 46 | 14 | 25 | 23 | OSA |
| `tv_aserai_lamellar_e2` | Southern Heavy Brass Lamellar Over Mail | Plate | T5 | auto | 0 | 46 | 14 | 25 | 23 | OSA |
| `AR_aserai_armor_c` | Southern Brass Scale Armor | Plate | T5 | auto | 0 | 47 | 14 | 23 | 20 | OSA |
| `AR_aserai_scale_armor_b` | Mastercrafted Southern Steel Scale over Chain Mail | Chainmail | T5 | auto | 0 | 51 | 16 | 16 | 22 | OSA |
| `AR_aserai_armor_y` | Eastern Alternating Lamellar Over Heavy Mail | Plate | T5 | auto | 0 | 50 | 14 | 16 | 18 | OSA |
| `tv_aserai_lamellar_a` | Southern Brass Lamellar Vest Over Mailed Silk Kaftan | Plate | T5 | auto | 0 | 48 | 16 | 14 | 6.2 | OSA |
| `tv_aserai_lamellar_d` | Southern Steel Lamellar Vest Over Mailed Silk Kaftan | Plate | T5 | auto | 0 | 48 | 16 | 14 | 6.2 | OSA |
| `ao_imperial_brass_scale_armor` | Brass Scale Coat Over Padded Mail | Plate | T5 | auto | 0 | 50 | 12 | 15 | 24 | OSA |
| `ao_imperial_brass_scale_armor_b` | Iron Scale Coat Over Padded Mail | Plate | T5 | auto | 0 | 50 | 12 | 15 | 24 | OSA |
| `AR_aserai_armor_f` | Southern Steel Scale Over Mail | Plate | T5 | auto | 0 | 48 | 14 | 14 | 22.1 | OSA |
| `AR_aserai_armor_f2` | Southern Brass Scale Over Mail | Plate | T5 | auto | 0 | 48 | 14 | 14 | 22.1 | OSA |
| `tv_aserai_lamellar_b` | Southern Brass Lamellar Vest Over Mail | Plate | T5 | auto | 0 | 46 | 14 | 14 | 22 | OSA |
| `tv_aserai_lamellar_b2` | Southern Steel Lamellar Vest Over Mail | Plate | T5 | auto | 0 | 46 | 14 | 14 | 22 | OSA |
| `tv_aserai_lamellar_f` | Southern Steel Lamellar Over Mail | Plate | T5 | auto | 0 | 46 | 14 | 12 | 23 | OSA |
| `tv_aserai_lamellar_f2` | Southern Brass Lamellar Over Mail | Plate | T5 | auto | 0 | 46 | 14 | 12 | 23 | OSA |
| `AR_aserai_armor_v` | Darshi Mailed Lamellar Vest Over Kaftan | Plate | T5 | auto | 0 | 50 | 10 | 10 | 8.6 | OSA |
| `AR_aserai_armor_v2` | Darshi Mailed Alternating Lamellar Vest Over Kaftan | Plate | T5 | auto | 0 | 50 | 10 | 10 | 8.6 | OSA |
| `AR_aserai_lamellar_a` | Southern Brass Lamellar Armor Over Padded Mail | Plate | T5 | auto | 0 | 48 | 10 | 12 | 15.1 | OSA |
| `AR_aserai_lamellar_b` | Southern Iron Lamellar Armor Over Padded Mail | Plate | T5 | auto | 0 | 48 | 10 | 12 | 15.1 | OSA |
| `AO_aserai_armor_a` | Brass Scale Armor Over Long Tunic | Plate | T5 | auto | 0 | 38 | 12 | 17 | 12.5 | OSA |
| `AO_aserai_armor_b` | Iron Scale Armor Over Long Tunic | Plate | T5 | auto | 0 | 38 | 12 | 17 | 12.5 | OSA |
| `AR_aserai_armor_t` | Darshi Lamellar Over Stripped Cloth | Plate | T5 | auto | 0 | 48 | 8 | 8 | 2.1 | OSA |
| `AR_aserai_armor_t2` | Darshi Alternating Lamellar Over Stripped Cloth | Plate | T5 | auto | 0 | 48 | 8 | 8 | 2.1 | OSA |
| `leather_strips_over_padded_robe` | Leather Strips Over Padded Robe | Cloth | T5 | auto | 0 | 25 | 15 | 24 | 4.1 | RBM |
| `tv_empire_armor_r` | Darshi Stripped Lamellar With Gambeson | Plate | T5 | auto | 0 | 48 | 8 | 8 | 7 | OSA |
| `tv_empire_armor_r2` | Darshi Stripped Alternating Lamellar With Gambeson | Plate | T5 | auto | 0 | 48 | 8 | 8 | 7 | OSA |
| `mirror_armor` | Southern Mirrored Plated Mail Shirt | Chainmail | T5 | auto | 0 | 35 | 13 | 14 | 7.5 | OSA |
| `AR_aserai_armor_d` | Southern Scale Armor Over Leather | Plate | T5 | auto | 0 | 32 | 12 | 17 | 12.5 | OSA |
| `AR_aserai_armor_d2` | Southern Brass Scale Armor Over Leather | Plate | T5 | auto | 0 | 32 | 12 | 17 | 12.5 | OSA |
| `AR_aserai_armor_g` | Aserai Decorated Scale Cuirass With Chain Mail | Chainmail | T5 | auto | 0 | 32 | 15 | 14 | 18 | OSA |
| `AR_aserai_armor_g2` | Aserai Decorated Brass Scale Cuirass With Chain Mail | Chainmail | T5 | auto | 0 | 32 | 15 | 14 | 18 | OSA |
| `mp_aserai_armor_02_b` | Aserai Decorated Chain Mail | Chainmail | T5 | 1 | 0 | 32 | 15 | 14 | 18 | Vanilla |
| `AO_aserai_armor_c` | Brass Scale Vest Over Long Tunic | Plate | T5 | auto | 0 | 38 | 12 | 10 | 12.5 | OSA |
| `AO_aserai_armor_d` | Iron Scale Vest Over Long Tunic | Plate | T5 | auto | 0 | 38 | 12 | 10 | 12.5 | OSA |
| `AR_aserai_armor_x` | Eastern Alternating Lamellar Over Mail Armor | Plate | T5 | auto | 0 | 46 | 14 | 0 | 16.5 | OSA |
| `mp_aserai_armor_02` | Aserai Chain Mail | Chainmail | T5 | 1 | 0 | 31 | 15 | 14 | 17 | Vanilla |
| `tv_aserai_lamellar_a2` | Southern Brass Lamellar Vest Over Padded Silk Kaftan | Plate | T4 | auto | 0 | 28 | 16 | 14 | 6.2 | OSA |
| `tv_aserai_lamellar_d2` | Southern Steel Lamellar Vest Over Padded Silk Kaftan | Plate | T4 | auto | 0 | 28 | 16 | 14 | 6.2 | OSA |
| `tv_aserai_lamellar_h` | Southern Bronze Lamellar Over Mailed Kaftan | Plate | T4 | auto | 0 | 42 | 10 | 6 | 6.2 | OSA |
| `tv_aserai_lamellar_h2` | Southern Steel Lamellar Over Mailed Kaftan | Plate | T4 | auto | 0 | 42 | 10 | 6 | 6.2 | OSA |
| `TV_aserai_robe_a` | Southern Decorated Robe With Double Mail | Chainmail | T4 | auto | 0 | 28 | 14 | 14 | 8.2 | OSA |
| `AR_aserai_armor_j` | Darshi Decorated Kaftan Over Mail | Chainmail | T4 | auto | 0 | 37 | 6 | 12 | 15 | OSA |
| `mp_desert_scale_armor` | Luxury Scale Armor | Plate | T4 | 1 | 0 | 32 | 11 | 11 | 14.4 | Vanilla |
| `studded_leather_coat` | Studded Leather Coat | Leather | T4 | auto | 0 | 21 | 13 | 20 | 1.6 | RBM |
| `AR_aserai_armor_b` | Southern Double Mail Hauberk | Chainmail | T4 | auto | 0 | 27 | 12 | 14 | 9.5 | OSA |
| `AR_aserai_armor_i` | Darshi Kaftan Over Mail | Chainmail | T4 | auto | 0 | 35 | 6 | 12 | 15 | OSA |
| `desert_padded_cloth` | Light Long Padded Armor | Cloth | T4 | auto | 0 | 18 | 16 | 18 | 1.3 | RBM |
| `long_padded_robe` | Long Padded Robe | Cloth | T4 | auto | 0 | 20 | 10 | 20 | 22.3 | RBM |
| `AR_aserai_armor_e` | Southern Scale Armor Over Cloth | Plate | T4 | auto | 0 | 25 | 10 | 14 | 12.5 | OSA |
| `AR_aserai_armor_e2` | Southern Brass Scale Armor Over Cloth | Plate | T4 | auto | 0 | 25 | 10 | 14 | 12.5 | OSA |
| `AR_aserai_scale_armor_a` | Mastercrafted Southern Steel Scale Mail | Plate | T3 | auto | 0 | 35 | 7 | 6 | 18 | OSA |
| `aserai_horseman_armor` | Ring Mail | Leather | T3 | auto | 0 | 26 | 12 | 10 | 4.4 | RBM |
| `mp_aserai_scale_armor_on_cloth` | Mastercrafted Southern Scale Mail | Plate | T3 | 1 | 0 | 35 | 7 | 6 | 18 | Vanilla |
| `tv_empire_armor_c` | Darshi Mail Over Stripped Leather With Gambeson | Chainmail | T3 | auto | 0 | 32 | 8 | 8 | 10.2 | OSA |
| `AR_aserai_armor_m` | Darshi Mailed Heavy Leather Vest Over Kaftan | Chainmail | T3 | auto | 0 | 31 | 8 | 8 | 8.6 | OSA |
| `short_padded_robe` | Short Padded Robe | Cloth | T3 | auto | 0 | 18 | 11 | 18 | 1 | RBM |
| `AR_aserai_armor_w` | Eastern Light Lamellar | Plate | T3 | auto | 0 | 36 | 8 | 0 | 16.5 | OSA |
| `AR_aserai_armor_w2` | Eastern Light Alternating Lamellar | Plate | T3 | auto | 0 | 36 | 8 | 0 | 16.5 | OSA |
| `mp_leather_strips_over_padded_robe` | Leather Strips over Padded Robe | Cloth | T3 | 1 | 0 | 34 | 1 | 9 | 4.1 | Vanilla |
| `AR_aserai_armor_r` | Darshi Mail Over Stripped Cloth | Chainmail | T3 | auto | 0 | 31 | 6 | 6 | 10.2 | OSA |
| `tv_empire_armor_q` | Darshi Stripped Leather With Gambeson | Leather | T3 | auto | 0 | 23 | 8 | 8 | 10.2 | OSA |
| `tv_empire_armor_t` | Darshi Stripped Cloth With Gambeson | Leather | T3 | auto | 0 | 23 | 8 | 8 | 10.2 | OSA |
| `ao_mabili_hide_armor_c` | Southern Iron Lamellar Over Hide Skirt | Leather | T2 | auto | 0 | 34 | 1 | 3 | 5 | OSA |
| `ao_mabili_hide_armor_c2` | Southern Iron Lamellar Over Leopard Pelt Skirt | Leather | T2 | auto | 0 | 34 | 1 | 3 | 5 | OSA |
| `ao_mabili_hide_armor_c3` | Southern Iron Lamellar Over Zebra Pelt Skirt | Leather | T2 | auto | 0 | 34 | 1 | 3 | 5 | OSA |
| `mp_ringed_desert_armor` | Heavy Ring Mail | Chainmail | T2 | 1 | 0 | 25 | 1 | 11 | 10.5 | Vanilla |
| `tv_aserai_lamellar_g` | Southern Brass Lamellar Over Padded Cloth | Plate | T2 | auto | 0 | 28 | 2 | 7 | 7 | OSA |
| `tv_aserai_lamellar_g2` | Southern Steel Lamellar Over Padded Cloth | Plate | T2 | auto | 0 | 28 | 2 | 7 | 7 | OSA |
| `AR_aserai_armor_a` | Southern Leather Coat | Leather | T2 | auto | 0 | 20 | 6 | 10 | 8.4 | OSA |
| `AR_aserai_armor_p` | Southern Gambeson | Cloth | T2 | auto | 0 | 18 | 9 | 9 | 2 | OSA |
| `tv_aserai_lamellar_c` | Southern Bronze Lamellar Over Long Kaftan | Plate | T2 | auto | 0 | 28 | 4 | 4 | 6.2 | OSA |
| `tv_aserai_lamellar_c2` | Southern Steel Lamellar Over Long Kaftan | Plate | T2 | auto | 0 | 28 | 4 | 4 | 6.2 | OSA |
| `AR_aserai_armor_l` | Darshi Heavy Leather Vest Over Kaftan | Leather | T2 | auto | 0 | 23 | 6 | 6 | 3.1 | OSA |
| `AR_aserai_armor_q` | Darshi Leather Subarmalis Over Tunic | Leather | T2 | auto | 0 | 22 | 6 | 6 | 3.1 | OSA |
| `AR_aserai_armor_zb` | Darshi Kaftan Over Leather | Leather | T2 | auto | 0 | 22 | 6 | 6 | 3.1 | OSA |
| `layered_robe` | Layered Robe | Cloth | T2 | auto | 0 | 13 | 6 | 13 | 14 | RBM |
| `AR_aserai_armor_k` | Darshi Leather Vest Over Kaftan | Leather | T2 | auto | 0 | 22 | 4 | 4 | 3.1 | OSA |
| `mp_southern_lamellar_armor` | Bronze Lamellar Vest over Gambeson | Plate | T2 | 1 | 0 | 25 | 1 | 4 | 8.8 | Vanilla |
| `AR_aserai_armor_s` | Darshi Cloth Subarmalis Over Tunic | Cloth | T1 | auto | 0 | 15 | 6 | 6 | 2.1 | OSA |
| `AR_aserai_armor_za` | Darshi Kaftan Over Cloth | Cloth | T1 | auto | 0 | 15 | 6 | 6 | 2.1 | OSA |
| `aserai_civil_a` | Luxury Kaftan with Waistband | Cloth | T1 | auto | 0 | 11 | 5 | 11 | 0.8 | RBM |
| `tv_aserai_lamellar_i` | Southern Leather Cuirass Over Padded Cloth | Plate | T1 | auto | 0 | 18 | 2 | 7 | 7 | OSA |
| `tv_aserai_lamellar_j` | Southern Leather Lamellar Cuirass Over Padded Cloth | Plate | T1 | auto | 0 | 18 | 2 | 7 | 7 | OSA |
| `aserai_female_civil_a` | Feminine Kaftan | Cloth | T1 | auto | 0 | 9 | 7 | 9 | 0.9 | RBM |
| `aserai_civil_b` | Kaftan with Leather Belt | Cloth | T1 | auto | 0 | 8 | 6 | 8 | 1.1 | RBM |
| `thick_sleeved_robe` | Thick Sleeved Robe | Cloth | T1 | auto | 0 | 8 | 5 | 8 | 1 | RBM |
| `ao_mabili_hide_armor_a` | Southern Leather Lamellar Over Hide Skirt | Leather | T1 | auto | 0 | 16 | 1 | 3 | 5 | OSA |
| `ao_mabili_hide_armor_a2` | Southern Leather Lamellar Over Leopard Pelt Skirt | Leather | T1 | auto | 0 | 16 | 1 | 3 | 5 | OSA |
| `ao_mabili_hide_armor_a3` | Southern Leather Lamellar Over Zebra Pelt Skirt | Leather | T1 | auto | 0 | 16 | 1 | 3 | 5 | OSA |
| `ao_mabili_hide_armor_b` | Southern Leather Vest Over Hide Skirt | Leather | T1 | auto | 0 | 16 | 1 | 3 | 1.9 | OSA |
| `ao_mabili_hide_armor_b2` | Southern Leather Vest Over Leopard Pelt Skirt | Leather | T1 | auto | 0 | 16 | 1 | 3 | 1.9 | OSA |
| `ao_mabili_hide_armor_b3` | Southern Leather Vest Over Zebra Pelt Skirt | Leather | T1 | auto | 0 | 16 | 1 | 3 | 1.9 | OSA |
| `AR_aserai_armor_h` | Southern Aketon | Cloth | T1 | auto | 0 | 14 | 3 | 3 | 3.4 | OSA |
| `aserai_civil_d` | Colored Bisht | Cloth | T1 | auto | 0 | 7 | 6 | 7 | 1 | RBM |
| `aserai_civil_e` | Thawb | Cloth | T1 | auto | 0 | 7 | 6 | 7 | 1.1 | RBM |
| `aserai_civil_f` | Simple Peasant Robe | Cloth | T1 | auto | 0 | 7 | 6 | 7 | 1.1 | RBM |
| `eastern_silk_clothing` | Silk Kaftan | Cloth | T1 | auto | 0 | 7 | 6 | 7 | 0.4 | RBM |
| `fine_southern_dress` | Tailored Dress | Cloth | T1 | auto | 0 | 7 | 6 | 7 | 0.7 | RBM |
| `tassled_southern_robes` | Tassled Robes | Cloth | T1 | auto | 0 | 7 | 6 | 7 | 0.9 | RBM |
| `AR_aserai_armor_o` | Southern Fur Coat Over Tunic | Leather | T1 | auto | 0 | 12 | 2 | 5 | 2.1 | OSA |
| `aserai_civil_c` | Colored Kaftan | Cloth | T1 | auto | 0 | 7 | 5 | 7 | 0.9 | RBM |
| `aserai_female_civil_b` | Kaftan with Waistband | Cloth | T1 | auto | 0 | 7 | 5 | 7 | 0.8 | RBM |
| `aserai_tunic_waistcoat` | Luxury Kaftan | Cloth | T1 | auto | 0 | 7 | 5 | 7 | 0.8 | RBM |
| `desert_robe` | Simple Kaftan | Cloth | T1 | auto | 0 | 7 | 5 | 7 | 1.8 | RBM |
| `aserai_villager_female_dress` | Villager Short Dress | Cloth | T0 | auto | 0 | 6 | 5 | 5 | 0.5 | RBM |
| `long_desert_robe` | Long Simple Robe | Cloth | T0 | auto | 0 | 6 | 5 | 5 | 0.6 | RBM |
| `AR_aserai_armor_zc` | Darshi Silk Noble Robes | Cloth | T0 | auto | 0 | 8 | 4 | 3 | 0.9 | OSA |
| `AR_aserai_armor_zd` | Southern Noble Robes | Cloth | T0 | auto | 0 | 8 | 4 | 3 | 0.9 | OSA |
| `AR_aserai_armor_zd2` | Southern Decorated Noble Robes | Cloth | T0 | auto | 0 | 8 | 4 | 3 | 0.9 | OSA |
| `mp_aserai_civil_d` | Colored Bisht | Cloth | T0 | 1 | 0 | 5 | 2 | 2 | 1 | Vanilla |
| `AR_aserai_armor_n` | Southern Wrapped Tunic | Cloth | T-1 | auto | 0 | 4 | 2 | 2 | 0.3 | OSA |
| `mp_aserai_civil_b` | Kaftan with Leather Belt | Cloth | T-1 | 1 | 0 | 4 | 2 | 2 | 1.1 | Vanilla |
| `mp_aserai_civil_c` | Colored Kaftan | Cloth | T-1 | 1 | 0 | 4 | 2 | 2 | 0.9 | Vanilla |
| `mp_aserai_civil_e` | Thawb | Cloth | T-1 | 1 | 0 | 4 | 2 | 1 | 1.1 | Vanilla |
| `mp_tassled_southern_robes` | Tassled Robes | Cloth | T-1 | 1 | 0 | 3 | 2 | 2 | 0.9 | Vanilla |
| `ao_mabili_hide_skirt` | Southern Hide Skirt | Leather | T-1 | auto | 0 | 1 | 0 | 3 | 0.1 | OSA |
| `ao_mabili_leopard_skirt` | Southern Leopard Pelt Skirt | Leather | T-1 | auto | 0 | 1 | 0 | 3 | 0.1 | OSA |
| `ao_mabili_zebra_skirt` | Southern Zebra Pelt Skirt | Leather | T-1 | auto | 0 | 1 | 0 | 3 | 0.1 | OSA |

<!-- CULTURE:nord COUNT:24 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `nordic_chainmail_c` | Reinforced Mail Coat | Chainmail | T5 | auto | 0 | 75 | 55 | 60 | 18 | RBM |
| `nord_king_armor_b` | Gilded Noble Armor | Plate | T5 | auto | 0 | 62 | 62 | 57 | 21 | RBM |
| `nord_king_armor_c` | Gilded Shield-Companion Armor | Plate | T5 | auto | 0 | 62 | 62 | 57 | 21 | RBM |
| `nord_king_armor` | Mastercrafted Northern Armor | Plate | T5 | auto | 0 | 60 | 60 | 55 | 25 | RBM |
| `nord_king_armor_d` | Mastercrafted Northern Armor | Plate | T5 | auto | 0 | 60 | 60 | 55 | 25 | RBM |
| `nordic_chainmail_a` | Nordic Long Heavy Mail | Chainmail | T5 | auto | 0 | 60 | 50 | 50 | 18 | RBM |
| `RBM_master_assassin_armor` | Master Assassin Armor | Chainmail | T5 | auto | 0 | 53 | 45 | 42 | 5 | RBM |
| `nord_chainmail_armor` | Nordic Heavy Mail Armor | Chainmail | T5 | auto | 0 | 45 | 48 | 45 | 17 | RBM |
| `northman_raider_armor` | Northern Raider Armor | Plate | T5 | auto | 0 | 60 | 40 | 15 | 18 | RBM |
| `pirate_tier_3_armor` | Sky Chain Armor | Chainmail | T5 | auto | 0 | 62 | 38 | 13 | 14 | RBM |
| `heavy_gambeson_armor` | Mariner Leather Gambeson | Leather | T5 | auto | 0 | 35 | 40 | 35 | 5.5 | RBM |
| `improved_assassin_armor_q5` | Blackened Armor | Leather | T5 | auto | 0 | 38 | 35 | 32 | 5 | RBM |
| `norse_chainmail_pleatedtrousers` | Norse Chainmail | Chainmail | T5 | auto | 0 | 38 | 38 | 23 | 16 | RBM |
| `heavy_gambeson_armor_plain` | Mariner Gambeson | Leather | T5 | auto | 0 | 27 | 30 | 27 | 5 | RBM |
| `naval_strappy_tunic` | Nordic Luxury Tunic | Cloth | T5 | auto | 0 | 25 | 27 | 25 | 5 | RBM |
| `assassin_armor` | Blackened Armor | Leather | T5 | auto | 0 | 28 | 25 | 22 | 5 | RBM |
| `double_belted_leather_armor` | Long Leather Vest | Leather | T5 | auto | 0 | 28 | 18 | 28 | 6 | RBM |
| `pirate_tier_2_body` | Pirate Leather Armor | Leather | T3 | auto | 0 | 26 | 11 | 11 | 5 | RBM |
| `nord_casual_tunic` | Nordic Woolen Tunic | Leather | T3 | auto | 0 | 13 | 13 | 13 | 3 | RBM |
| `pirate_a` | Pirate Cloth Armor | Cloth | T2 | auto | 0 | 11 | 11 | 11 | 3 | RBM |
| `dress_norse_lady` | Northern Knotwork Tunic | Cloth | T2 | auto | 0 | 12 | 7 | 12 | 2 | RBM |
| `nord_poor_civil` | Old Linen Tunic | Leather | T1 | auto | 0 | 11 | 8 | 8 | 3 | RBM |
| `nordic_civilian_dress` | Nordic Dress | Cloth | T1 | auto | 0 | 8 | 5 | 10 | 4 | RBM |
| `nordic_civilian_dress_plaid` | Nordic Plaid Dress | Cloth | T1 | auto | 0 | 8 | 5 | 10 | 4 | RBM |

<!-- CULTURE:looters COUNT:28 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `dummy_armor_max` | Dummy Max Armor | Plate | T5 | auto | 55 | 55 | 55 | 55 | 40 | RBM |
| `dummy_armor_higher_average` | Dummy Higher Average | Chainmail | T5 | auto | 40 | 47 | 40 | 40 | 30 | RBM |
| `rough_fur_over_chain` | Rough Fur over Mail | Chainmail | T5 | auto | 0 | 46 | 38 | 38 | 14 | RBM |
| `buckled_wildsman_armor` | Buckled Mail Armor | Chainmail | T5 | auto | 0 | 40 | 28 | 12 | 12.3 | RBM |
| `padded_leather_overcoat` | Padded Leather Overcoat | Leather | T5 | auto | 0 | 22 | 17 | 20 | 3.2 | RBM |
| `dummy_armor_lower_average` | Dummy Lower Average | Leather | T4 | auto | 12 | 12 | 18 | 12 | 20 | RBM |
| `rough_fur_armor` | Rough Fur Armor | Leather | T4 | auto | 0 | 20 | 13 | 21 | 1.3 | RBM |
| `AR_bandit_armor_a` | Studded Fur Armor With Mail | Chainmail | T4 | auto | 0 | 32 | 14 | 7 | 1.2 | OSA |
| `studded_fur_armor` | Studded Fur Armor | Leather | T3 | auto | 0 | 16 | 12 | 19 | 0.6 | RBM |
| `vlandia_bandit_a` | Cheap Paddings | Leather | T2 | auto | 0 | 15 | 11 | 11 | 2.9 | RBM |
| `fur_coat_over_tunic` | Fur Coat Over Tunic | Leather | T2 | auto | 0 | 12 | 8 | 12 | 2.1 | RBM |
| `fur_waistcoat_over_tunic` | Fur Waistcoat Over Tunic | Leather | T2 | auto | 0 | 12 | 8 | 12 | 2.3 | RBM |
| `sleeveless_fur_coat` | Sleeveless Fur Coat | Leather | T2 | auto | 0 | 12 | 6 | 12 | 1.8 | RBM |
| `fur_skirt` | Fur Skirt | Leather | T2 | auto | 0 | 8 | 0 | 21 | 0.6 | RBM |
| `bandit_gambeson` | Rugged Gambeson | Cloth | T1 | auto | 0 | 11 | 5 | 11 | 0.9 | RBM |
| `vlandia_bandit_b` | Cheap Colored Clothes | Leather | T1 | auto | 0 | 10 | 7 | 10 | 1.8 | RBM |
| `fur_skirt_with_tunic` | Fur Skirt With Tunic | Leather | T1 | auto | 0 | 10 | 6 | 10 | 0.9 | RBM |
| `bandit_envelope_dress_v1` | Commoner Tunic | Cloth | T1 | auto | 0 | 8 | 6 | 8 | 0.6 | RBM |
| `bandit_fur_a` | Old Fur Armor | Cloth | T1 | auto | 0 | 11 | 0 | 11 | 0.9 | RBM |
| `bandit_leather_water_flask` | Commoner Shirt | Cloth | T1 | auto | 0 | 7 | 5 | 7 | 0.5 | RBM |
| `ragged_robes` | Ragged Robes | Cloth | T1 | auto | 0 | 7 | 5 | 7 | 0.9 | RBM |
| `bandit_envelope_dress_v2` | Sleeveless Enveloped Padded Garments | Cloth | T0 | auto | 0 | 6 | 5 | 5 | 0.5 | RBM |
| `bandit_fur_b` | Old Fur Coat | Cloth | T0 | auto | 0 | 11 | 0 | 5 | 1 | RBM |
| `torn_bandit_clothes` | Torn Padded Garments | Cloth | T0 | auto | 0 | 6 | 5 | 5 | 0.5 | RBM |
| `vlandia_bandit_c` | Torn Robe | Leather | T0 | auto | 0 | 8 | 0 | 8 | 0.4 | RBM |
| `burlap_sack_dress` | Burlap Dress | Cloth | T0 | auto | 0 | 5 | 5 | 5 | 0.5 | RBM |
| `tattered_rags` | Tattered Rags | Cloth | T0 | auto | 0 | 7 | 0 | 7 | 0.3 | RBM |
| `dummy_armor_min` | Dummy Min Armor | Cloth | T-1 | auto | 0 | 0 | 0 | 0 | 10 | RBM |

<!-- CULTURE:neutral_culture COUNT:128 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `mp_dummy_armor_max` | Dummy Max Armor | Plate | T5 | 1 | 55 | 55 | 55 | 55 | 40 | Vanilla |
| `mp_heavy_skirt` | Imperial Lamellar Armor | Plate | T5 | 1 | 54 | 54 | 54 | 54 | 22.7 | Vanilla |
| `tacky_bandit` | Gilded Raider Armor | Chainmail | T5 | auto | 0 | 60 | 55 | 55 | 18 | RBM |
| `mp_dummy_armor_higher_average` | Dummy Higher Average | Chainmail | T5 | 1 | 37 | 37 | 37 | 37 | 30 | Vanilla |
| `mp_medium_skirt` | Thick Brigandine Vest | Leather | T5 | 1 | 33 | 33 | 33 | 33 | 10.6 | Vanilla |
| `mp_lamellar_with_scale_skirt` | Lamellar with Scale Skirt | Plate | T5 | 1 | 0 | 45 | 11 | 20 | 26 | Vanilla |
| `mp_light_armor` | mp_light_armor | Cloth | T5 | 1 | 18 | 18 | 18 | 18 | 1.9 | Vanilla |
| `mp_coat_of_plates_over_mail` | Brigandine over Mail | Chainmail | T5 | 1 | 0 | 45 | 11 | 19 | 25.1 | Vanilla |
| `mp_brass_lamellar_over_mail` | Bronze Lamellar over Mail | Chainmail | T5 | 1 | 0 | 44 | 11 | 19 | 22.5 | Vanilla |
| `mp_imperial_scale_armor` | Heavy Scale Armor over Mail Hauberk | Plate | T5 | 1 | 0 | 44 | 11 | 18 | 27.2 | Vanilla |
| `mp_white_coat_over_mail` | White Tabard over Mail Hauberk | Chainmail | T5 | 1 | 0 | 43 | 11 | 18 | 23.5 | Vanilla |
| `mp_imperial_lamellar` | Cataphract Lamellar Armor | Plate | T5 | 1 | 0 | 43 | 11 | 17 | 22.1 | Vanilla |
| `mp_stitched_leather_over_mail` | Stitched Leather over Mail | Chainmail | T5 | 1 | 0 | 43 | 7 | 20 | 23.8 | Vanilla |
| `mp_battania_noble_armor` | Highborn Mail Armor | Chainmail | T5 | 1 | 0 | 43 | 4 | 18 | 25 | Vanilla |
| `mp_western_scale_mail` | Scale Mail | Plate | T5 | 1 | 0 | 43 | 2 | 19 | 27.7 | Vanilla |
| `mp_northern_coat_of_plates` | Coat Of Plates | Plate | T5 | 1 | 0 | 45 | 3 | 15 | 21.5 | Vanilla |
| `mp_aserai_archer_armor` | Gambeson over Mail Armor | Cloth | T5 | 1 | 0 | 36 | 11 | 14 | 18.6 | Vanilla |
| `mp_battania_warlord_armor` | Scale Warlord Armor | Plate | T5 | 1 | 0 | 41 | 4 | 16 | 20.9 | Vanilla |
| `mp_banded_leather_over_mail` | Banded Leather over Mail | Chainmail | T5 | 1 | 0 | 39 | 3 | 17 | 24.7 | Vanilla |
| `mp_ranger_mail` | Ranger Mail | Chainmail | T5 | 1 | 0 | 37 | 2 | 20 | 25 | Vanilla |
| `mp_battania_mercenary_armor` | Luxury Brigandine | Chainmail | T4 | 1 | 0 | 38 | 4 | 16 | 18.6 | Vanilla |
| `mp_imperial_lamellar_over_leather` | Luxury Lamellar Vest over Leather | Plate | T4 | 1 | 0 | 41 | 3 | 14 | 24 | Vanilla |
| `mp_nordic_lamellar_vest` | Heavy Lamellar Vest | Cloth | T4 | 1 | 0 | 44 | 2 | 12 | 16.7 | Vanilla |
| `mp_eastern_lamellar_armor` | Lancer Lamellar Armor | Plate | T4 | 1 | 0 | 37 | 4 | 16 | 20.3 | Vanilla |
| `mp_nordic_lamellar_armor` | Sturgian Lamellar Armor | Plate | T4 | 1 | 0 | 42 | 2 | 13 | 22.2 | Vanilla |
| `mp_eastern_plated_leather` | Mirrored Brigandine Armor | Plate | T4 | 1 | 0 | 39 | 2 | 15 | 21.5 | Vanilla |
| `mp_battania_woodland_chainmail` | Heavy Mail Shirt with Wide Belt | Chainmail | T4 | 1 | 0 | 36 | 4 | 15 | 23 | Vanilla |
| `mp_desert_lamellar` | Aserai Lamellar Armor | Plate | T4 | 1 | 0 | 40 | 4 | 10 | 25.8 | Vanilla |
| `mp_imperial_mail_over_stripped_leather` | Infantryman Mail over Stripped Leather | Chainmail | T4 | 1 | 0 | 37 | 3 | 14 | 18.6 | Vanilla |
| `mp_vlandia_chainmail` | Vlandia Chainmail | Chainmail | T4 | 1 | 0 | 37 | 4 | 12 | 23 | Vanilla |
| `mp_desert_robe_over_mail` | Robe over Mail Hauberk | Chainmail | T4 | 1 | 0 | 33 | 4 | 15 | 18.9 | Vanilla |
| `mp_nordic_sloven_over_mail` | Leather Tabard over Mail | Chainmail | T4 | 1 | 0 | 37 | 2 | 13 | 21.5 | Vanilla |
| `mp_rough_fur_over_chain` | Rough Fur over Chain | Chainmail | T4 | 1 | 0 | 30 | 11 | 11 | 12 | Vanilla |
| `mp_decorated_nordic_hauberk` | Huscarl Decorated Mail Shirt | Chainmail | T4 | 1 | 0 | 34 | 2 | 15 | 20.5 | Vanilla |
| `southern_pirate_armor` | Corsair Cloth Armor | Cloth | T4 | auto | 0 | 17 | 17 | 17 | 4 | RBM |
| `mp_dummy_armor_lower_average` | Dummy Lower Average | Leather | T4 | 1 | 12 | 12 | 12 | 12 | 20 | Vanilla |
| `mp_hauberk` | Hauberk | Chainmail | T4 | 1 | 0 | 34 | 3 | 12 | 23.7 | Vanilla |
| `mp_nordic_hauberk` | Huscarl Mail Shirt | Chainmail | T3 | 1 | 0 | 33 | 3 | 12 | 19.7 | Vanilla |
| `bandit_hybrid_armor_b` | Raider Iron Clothing | Cloth | T3 | auto | 0 | 23 | 12 | 12 | 4 | RBM |
| `mp_eastern_plated_leather_vest` | Brigandine Vest | Plate | T3 | 1 | 0 | 36 | 2 | 8 | 25.5 | Vanilla |
| `mp_mail_shirt` | Mail Shirt | Chainmail | T3 | 1 | 0 | 35 | 3 | 8 | 22 | Vanilla |
| `mp_studded_leather_over_aketon` | Studded Leather over Aketon | Leather | T3 | 1 | 0 | 29 | 6 | 11 | 13.7 | Vanilla |
| `mp_eastern_stitched_leather_coat` | Stitched Cured Leather Coat | Leather | T3 | 1 | 0 | 30 | 3 | 12 | 13.9 | Vanilla |
| `bandit_hybrid_armor_a` | Raider Bronze Clothing | Cloth | T3 | auto | 0 | 20 | 12 | 12 | 4 | RBM |
| `mp_woven_leather_coat` | Boarskin Leather Coat | Leather | T3 | 1 | 0 | 31 | 2 | 11 | 12.2 | Vanilla |
| `mp_leather_lamellar_armor` | Cured Leather Lamellar Armor | Leather | T3 | 1 | 0 | 31 | 4 | 8 | 14 | Vanilla |
| `mp_basic_imperial_leather_armor` | Imperial Leather Armor | Leather | T3 | 1 | 0 | 29 | 4 | 9 | 9.5 | Vanilla |
| `mp_empire_warrior_padded_armor_a` | Infantryman Gambeson | Leather | T3 | 1 | 0 | 29 | 4 | 9 | 13.4 | Vanilla |
| `mp_empire_warrior_padded_armor_b` | Infantryman Gambeson over Leather Jacket | Leather | T3 | 1 | 0 | 29 | 4 | 9 | 15.1 | Vanilla |
| `mp_empire_warrior_padded_armor_c` | Infantryman Gambeson with Skirts | Leather | T3 | 1 | 0 | 29 | 4 | 9 | 12.3 | Vanilla |
| `mp_empire_warrior_padded_armor_d` | Infantryman Gambeson with Straps | Leather | T3 | 1 | 0 | 29 | 4 | 9 | 12.3 | Vanilla |
| `mp_plated_leather_coat` | Rough Brigandine | Plate | T3 | 1 | 0 | 37 | 1 | 1 | 10.7 | Vanilla |
| `mp_aserai_horseman_armor` | Ring Mail | Leather | T2 | 1 | 0 | 30 | 0 | 8 | 9 | Vanilla |
| `mp_eastern_studded_leather` | Steppe Studded Leather | Leather | T2 | 1 | 0 | 27 | 3 | 8 | 14 | Vanilla |
| `mp_legionary_mail` | Legionary Mail | Chainmail | T2 | 1 | 0 | 29 | 2 | 7 | 9.5 | Vanilla |
| `mp_padded_leather_shirt` | Padded Leather Shirt | Leather | T2 | 1 | 0 | 28 | 2 | 8 | 14.9 | Vanilla |
| `mp_imperial_mail_over_leather` | Infantryman Mail over Leather | Chainmail | T2 | 1 | 0 | 28 | 2 | 7 | 8.6 | Vanilla |
| `mp_leather_coat` | Leather Tunic | Leather | T2 | 1 | 0 | 26 | 2 | 9 | 13.9 | Vanilla |
| `bandit_hybrid_armor_c` | Raider Clothes | Cloth | T2 | auto | 0 | 12 | 12 | 12 | 3 | RBM |
| `mp_padded_short_coat` | Padded Short Coat | Cloth | T2 | 1 | 0 | 27 | 0 | 9 | 11.6 | Vanilla |
| `mp_woven_leather_vest` | Woven Leather Vest | Leather | T2 | 1 | 0 | 25 | 2 | 9 | 13.1 | Vanilla |
| `mp_belted_leather_cuirass` | Belted Leather Cuirass | Leather | T2 | 1 | 0 | 22 | 5 | 8 | 11.8 | Vanilla |
| `mp_northern_padded_cloth` | Light Padded Armor | Cloth | T2 | 1 | 0 | 22 | 5 | 7 | 9 | Vanilla |
| `mp_stitched_leather_vest` | Stitched Leather Vest | Leather | T2 | 1 | 0 | 25 | 2 | 7 | 10.8 | Vanilla |
| `mp_buckled_wildsman_armor` | Buckled Brigandine Armor | Chainmail | T2 | 1 | 0 | 29 | 2 | 2 | 12.3 | Vanilla |
| `mp_vlandia_bandit_c` | Torn Robe | Leather | T2 | 1 | 0 | 23 | 0 | 10 | 7 | Vanilla |
| `mp_aserai_robe_c_chain` | Layered Desert Robe | Cloth | T2 | 1 | 0 | 22 | 3 | 7 | 24.3 | Vanilla |
| `mp_long_padded_robe` | Long Padded Robe | Cloth | T2 | 1 | 0 | 22 | 3 | 7 | 22.3 | Vanilla |
| `merchants_fur_coat` | Merchant Fur Coat | Leather | T2 | auto | 0 | 11 | 8 | 11 | 2.9 | RBM |
| `mp_crude_leather_armor` | Crude Hide Armor | Leather | T2 | 1 | 0 | 20 | 5 | 5 | 12.3 | Vanilla |
| `mp_gambeson_b` | Heavy Aketon | Cloth | T2 | 1 | 0 | 19 | 4 | 7 | 10.8 | Vanilla |
| `mp_leather_scale_armor` | Leather Scale Armor | Leather | T2 | 1 | 0 | 18 | 7 | 5 | 8.6 | Vanilla |
| `mp_studded_steppe_leather` | Studded Leather Armor | Leather | T2 | 1 | 0 | 21 | 3 | 6 | 14 | Vanilla |
| `mp_desert_padded_cloth` | Light Long Padded Armor | Cloth | T1 | 1 | 0 | 18 | 3 | 7 | 7.6 | Vanilla |
| `mp_layered_leather_tunic` | Layered Leather Tunic | Leather | T1 | 1 | 0 | 18 | 4 | 6 | 11.4 | Vanilla |
| `mp_nordic_padded_cloth` | Reinforced Thick Tunic | Cloth | T1 | 1 | 0 | 21 | 2 | 5 | 11 | Vanilla |
| `mp_northern_padded_gambeson` | Short Gambeson over Tunic | Cloth | T1 | 1 | 0 | 19 | 3 | 6 | 11 | Vanilla |
| `mp_reinforced_suede_armor` | Reinforced Suede Armor | Cloth | T1 | 1 | 0 | 19 | 2 | 7 | 7.9 | Vanilla |
| `mp_studded_leather_coat` | Studded Leather Coat | Leather | T1 | 1 | 0 | 19 | 2 | 7 | 10.6 | Vanilla |
| `leather_apron` | Leather Apron | Leather | T1 | auto | 0 | 10 | 7 | 10 | 2.5 | RBM |
| `mp_highland_cloth` | Woodland Cloth Armor | Cloth | T1 | 1 | 0 | 18 | 3 | 6 | 9 | Vanilla |
| `mp_northern_leather_tabard` | Leather Tabard | Leather | T1 | 1 | 0 | 20 | 2 | 5 | 20.6 | Vanilla |
| `mp_sleeveless_padded_short_coat` | Sleeveless Padded Short Coat | Cloth | T1 | 1 | 0 | 19 | 2 | 6 | 11.5 | Vanilla |
| `mp_short_padded_robe` | Short Padded Robe | Cloth | T1 | 1 | 0 | 18 | 2 | 5 | 21.3 | Vanilla |
| `mp_bandit_gambeson` | Rugged Gambeson | Cloth | T1 | 1 | 0 | 14 | 3 | 7 | 2.2 | Vanilla |
| `mp_padded_leather_overcoat` | Padded Leather Overcoat | Leather | T1 | 1 | 0 | 13 | 3 | 7 | 3.2 | Vanilla |
| `mp_aketon` | Aketon | Cloth | T1 | 1 | 0 | 10 | 6 | 6 | 12.4 | Vanilla |
| `mp_northern_leather_vest` | Tunic with Leather Vest | Leather | T1 | 1 | 0 | 18 | 2 | 0 | 8.1 | Vanilla |
| `mp_tunic_with_rolled_cloth` | Tunic with Rolled Cloth | Cloth | T1 | 1 | 0 | 15 | 2 | 3 | 8.7 | Vanilla |
| `mp_heavy_nordic_coat` | Heavy Coat | Cloth | T1 | 1 | 0 | 14 | 2 | 3 | 11.3 | Vanilla |
| `mp_tunic_with_shoulder_pads` | Tunic with Shoulder Pads | Cloth | T1 | 1 | 0 | 13 | 2 | 4 | 8 | Vanilla |
| `mp_fur_armor` | Fur Armor | Leather | T0 | 1 | 0 | 15 | 1 | 2 | 13.6 | Vanilla |
| `mp_long_hemp_tunic` | Long Hemp Tunic | Cloth | T0 | 1 | 0 | 12 | 2 | 4 | 11 | Vanilla |
| `mp_rough_fur_armor` | Rough Fur Armor | Leather | T0 | 1 | 0 | 11 | 2 | 4 | 3.3 | Vanilla |
| `mp_footmans_tunic` | Military Tunic | Cloth | T0 | 1 | 0 | 11 | 2 | 3 | 8 | Vanilla |
| `mp_eastern_silk_clothing` | Silk Kaftan | Cloth | T0 | 1 | 0 | 14 | 1 | 0 | 11.3 | Vanilla |
| `mp_steppe_robe` | Steppe Robe | Cloth | T0 | 1 | 0 | 9 | 2 | 4 | 14 | Vanilla |
| `cloth_apron` | Cloth Apron | Cloth | T0 | auto | 0 | 7 | 0 | 7 | 0.8 | RBM |
| `half_apron` | Half Apron | Cloth | T0 | auto | 0 | 7 | 0 | 7 | 0.7 | RBM |
| `mp_heavy_nordic_tunic` | Heavy Tunic | Cloth | T0 | 1 | 0 | 12 | 2 | 0 | 7.2 | Vanilla |
| `mp_layered_robe` | Layered Robe | Cloth | T0 | 1 | 0 | 8 | 2 | 3 | 14 | Vanilla |
| `mp_vlandia_bandit_a` | Cheap Paddings | Leather | T0 | 1 | 0 | 10 | 1 | 2 | 2.9 | Vanilla |
| `mp_burlap_waistcoat` | Burlap Waistcoat | Cloth | T0 | 1 | 0 | 8 | 2 | 2 | 12 | Vanilla |
| `mp_fur_waistcoat_over_tunic` | Fur Waistcoat over Tunic | Leather | T0 | 1 | 0 | 8 | 2 | 2 | 2.3 | Vanilla |
| `mp_steppe_armor` | Striped Coat | Cloth | T0 | 1 | 0 | 8 | 2 | 2 | 14 | Vanilla |
| `mp_studded_fur_armor` | Studded Fur Armor | Leather | T0 | 1 | 0 | 7 | 2 | 3 | 0.6 | Vanilla |
| `mp_nordic_tunic` | Luxury Tunic | Cloth | T0 | 1 | 0 | 9 | 2 | 0 | 12 | Vanilla |
| `mp_roughcloth_shortcoat` | Roughcloth Shortcoat | Cloth | T0 | 1 | 0 | 7 | 1 | 3 | 9 | Vanilla |
| `mp_thick_sleeved_robe` | Thick Sleeved Robe | Cloth | T0 | 1 | 0 | 8 | 2 | 1 | 6.8 | Vanilla |
| `mp_long_desert_robe` | Long Desert Robe | Cloth | T0 | 1 | 0 | 7 | 2 | 1 | 10 | Vanilla |
| `mp_vlandia_bandit_b` | Cheap Colored Clothes | Leather | T0 | 1 | 0 | 6 | 2 | 2 | 1.8 | Vanilla |
| `mp_desert_robe` | Long Simple Robe | Cloth | T0 | 1 | 0 | 7 | 1 | 1 | 9 | Vanilla |
| `mp_bandit_fur_a` | Loathsome Fur Armor | Cloth | T-1 | 1 | 0 | 7 | 0 | 1 | 0.9 | Vanilla |
| `mp_battania_woodland_outfit` | Woodland Garments | Leather | T-1 | 1 | 0 | 4 | 2 | 2 | 1.2 | Vanilla |
| `mp_ragged_robes` | Ragged Robes | Cloth | T-1 | 1 | 0 | 4 | 2 | 2 | 5 | Vanilla |
| `mp_fur_skirt_with_tunic` | Fur Skirt with Tunic | Leather | T-1 | 1 | 0 | 3 | 1 | 3 | 0.9 | Vanilla |
| `mp_sleeveless_fur_coat` | Sleeveless Fur Coat | Leather | T-1 | 1 | 0 | 6 | 0 | 1 | 1.8 | Vanilla |
| `mp_armored_baggy_trunks` | Armored Baggy Trousers | Leather | T-1 | 1 | 0 | 2 | 0 | 4 | 1.5 | Vanilla |
| `mp_baggy_trunks` | Baggy Trousers | Leather | T-1 | 1 | 0 | 2 | 0 | 4 | 1.4 | Vanilla |
| `mp_bandit_envelope_dress_v1` | Commoner Tunic | Cloth | T-1 | 1 | 0 | 4 | 1 | 1 | 5 | Vanilla |
| `mp_bandit_fur_b` | Fur Coat | Cloth | T-1 | 1 | 0 | 6 | 0 | 0 | 1 | Vanilla |
| `mp_bandit_leather_water_flask` | Commoner Shirt | Cloth | T-1 | 1 | 0 | 4 | 1 | 1 | 7 | Vanilla |
| `mp_aserai_civil_a` | Luxury Kaftan with Waistband | Cloth | T-1 | 1 | 0 | 3 | 1 | 1 | 0.8 | Vanilla |
| `mp_bandit_envelope_dress_v2` | Sleeveless Enveloped Padded Garments | Cloth | T-1 | 1 | 0 | 4 | 0 | 1 | 6.5 | Vanilla |
| `mp_fur_skirt` | Fur Skirt | Leather | T-1 | 1 | 0 | 2 | 0 | 3 | 5 | Vanilla |
| `mp_sackcloth_tunic` | Sackcloth Tunic | Cloth | T-1 | 1 | 0 | 2 | 1 | 1 | 0.6 | Vanilla |
| `mp_burlap_sack_dress` | Burlap Dress | Cloth | T-1 | 1 | 0 | 1 | 0 | 1 | 0.5 | Vanilla |
| `mp_dummy_armor_min` | Dummy Min Armor | Cloth | T-1 | 1 | 0 | 0 | 0 | 0 | 10 | Vanilla |



