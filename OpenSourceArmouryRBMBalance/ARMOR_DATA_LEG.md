# 护甲数据总表 · 腿甲（LegArmor）

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
| **H / B / A / L** | head / body / arm / leg 四围防护（本槽主要看 **L**）|
| **Wt** | 重量 kg |
| **Src** | 数据来源（Vanilla / RBM / OSA）|

### Tier 公式（本槽 typeMultiplier = **1.6**）

```
raw    = 1.2·head + body + leg + arm
scaled = raw × 1.6
tier   = clamp(round(scaled × 0.1 − 0.4), 0, 6) − 1
```

---

## 数据表

本文档只覆盖 LegArmor（腿甲 / 靴 / 腿裙）· 其他 4 槽见同目录。
文化排序：**empire → vlandia → sturgia → battania → khuzait → aserai → nord → looters → neutral_culture → (none)**。
每文化内：**tier desc → rawArmor desc → stringId asc**。


<!-- SLOT:LegArmor COUNT:139 -->

<!-- SOURCE_DIST -->
| Source | Count |
|---|---:|
| OSA | 61 |
| RBM | 48 |
| Vanilla | 30 |

<!-- TIER_DIST -->
| Tier | Count |
|---|---:|
| T5 | 13 |
| T4 | 6 |
| T3 | 22 |
| T2 | 33 |
| T1 | 24 |
| T0 | 17 |
| T-1 | 24 |

<!-- CULTURE:empire COUNT:19 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `lamellar_plate_boots` | Lamellar Plate Boots | Plate | T5 | auto | 0 | 0 | 0 | 62 | 3.5 | RBM |
| `decorated_imperial_boots` | Decorated Plate Boots | Plate | T5 | auto | 0 | 0 | 0 | 44 | 2.3 | RBM |
| `plated_strip_boots` | Splint Boots | Plate | T5 | auto | 0 | 0 | 0 | 42 | 2.7 | RBM |
| `strapped_leather_boots` | Strapped Leather Boots | Cloth | T3 | auto | 0 | 0 | 0 | 30 | 0.9 | RBM |
| `empire_horseman_boots` | Horseman Boots | Leather | T3 | auto | 0 | 0 | 0 | 28 | 0.8 | RBM |
| `AR_empire_boots_c` | Gilded Lamellar Plate Boots | Plate | T2 | auto | 0 | 0 | 0 | 24 | 3.5 | OSA |
| `folded_town_boots` | Folded Town Boots | Leather | T2 | auto | 0 | 0 | 0 | 24 | 1 | RBM |
| `TV_empire_boots_a` | Imperial Boots With Gilded Greaves | Plate | T2 | auto | 0 | 0 | 0 | 24 | 2.7 | OSA |
| `TV_empire_boots_b` | Imperial Boots With Iron Greaves | Plate | T2 | auto | 0 | 0 | 0 | 24 | 2.7 | OSA |
| `tv_empire_plate_a` | Imperial Mail Boots With Decorated Greaves | Plate | T2 | auto | 0 | 0 | 0 | 24 | 2.3 | OSA |
| `tv_empire_plate_c` | Suede Splint Boots | Plate | T2 | auto | 0 | 0 | 0 | 22 | 2.7 | OSA |
| `fine_town_boots` | Fine Town Boots | Leather | T2 | auto | 0 | 0 | 0 | 20 | 0.5 | RBM |
| `AR_empire_boots_a` | Light Lamellar Plate Boots | Plate | T1 | auto | 0 | 0 | 0 | 18 | 3.5 | OSA |
| `AR_empire_boots_b` | Light Gilded Lamellar Plate Boots | Plate | T1 | auto | 0 | 0 | 0 | 18 | 3.5 | OSA |
| `TV_empire_boots_c` | Imperial Boots With Leather Greaves | Leather | T1 | auto | 0 | 0 | 0 | 14 | 2.7 | OSA |
| `mp_folded_town_boots` | Folded Town Boots | Leather | T0 | 1 | 0 | 0 | 0 | 7 | 1 | Vanilla |
| `DZ_empire_boots_a` | Wrapped Leather Boots | Cloth | T-1 | auto | 0 | 0 | 0 | 5 | 0.9 | OSA |
| `DZ_empire_boots_b` | Tied Leather Shoes | Leather | T-1 | auto | 0 | 0 | 0 | 3 | 0.7 | OSA |
| `TV_moccasins_a` | Leather Slippers | Cloth | T-1 | auto | 0 | 0 | 0 | 1 | 0.6 | OSA |

<!-- CULTURE:vlandia COUNT:9 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `mail_cavalier_boots` | Mail Cavalier Boots | Chainmail | T5 | auto | 0 | 0 | 0 | 43 | 1.8 | RBM |
| `leather_cavalier_boots` | Leather Cavalier Boots | Leather | T4 | auto | 0 | 0 | 0 | 32 | 0.9 | RBM |
| `TV_vlandia_boots_c` | Mailled Splint Boots | Plate | T2 | auto | 0 | 0 | 0 | 24 | 3.5 | OSA |
| `TV_vlandia_boots_a` | Mailled Boots With Leather Greaves | Chainmail | T2 | auto | 0 | 0 | 0 | 23 | 3 | OSA |
| `AR_vlandia_boots_b` | Western Boots With Iron Greaves | Plate | T2 | auto | 0 | 0 | 0 | 22 | 2.7 | OSA |
| `AR_vlandia_boots_c` | Western Boots With Gilded Greaves | Plate | T2 | auto | 0 | 0 | 0 | 22 | 2.7 | OSA |
| `DZ_vlandia_boots_a` | Mail Boots | Chainmail | T2 | auto | 0 | 0 | 0 | 20 | 1.8 | OSA |
| `TV_vlandia_boots_b` | Mailled Shoes | Chainmail | T2 | auto | 0 | 0 | 0 | 20 | 1.8 | OSA |
| `AR_vlandia_boots_a` | Western Boots With Leather Greaves | Leather | T1 | auto | 0 | 0 | 0 | 14 | 0.9 | OSA |

<!-- CULTURE:sturgia COUNT:11 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `northern_plated_boots` | Huscarl Plated Boots | Plate | T5 | auto | 0 | 0 | 0 | 60 | 2.9 | RBM |
| `strapped_mail_chausses` | Strapped Mail Chausses | Chainmail | T5 | auto | 0 | 0 | 0 | 43 | 3 | RBM |
| `mail_chausses` | Mail Chausses | Chainmail | T5 | auto | 0 | 0 | 0 | 42 | 2.9 | RBM |
| `sturgia_boots_b` | Tied Leather Boots | Leather | T3 | auto | 0 | 0 | 0 | 29 | 1.8 | RBM |
| `sturgia_boots_a` | Simple Leather Boots | Leather | T3 | auto | 0 | 0 | 0 | 27 | 1.55 | RBM |
| `TV_sturgia_boots_a` | Northern Boots With Steel Greaves | Plate | T2 | auto | 0 | 0 | 0 | 24 | 2.7 | OSA |
| `TV_sturgia_boots_b` | Northern Boots With Gilded Greaves | Plate | T2 | auto | 0 | 0 | 0 | 24 | 2.7 | OSA |
| `sturgia_boots_c` | Tied Boots | Leather | T2 | auto | 0 | 0 | 0 | 23 | 1.2 | RBM |
| `sturgia_boots_d` | Wrapped Boots | Leather | T2 | auto | 0 | 0 | 0 | 20 | 1 | RBM |
| `TV_aserai_boots_h` | Leather Boots | Leather | T-1 | auto | 0 | 0 | 0 | 3 | 1 | OSA |
| `TV_aserai_boots_i` | Blackened Leather Boots | Leather | T-1 | auto | 0 | 0 | 0 | 3 | 1 | OSA |

<!-- CULTURE:battania COUNT:40 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `battania_warlord_boots` | Scaled Boots | Plate | T5 | auto | 0 | 0 | 0 | 46 | 2.6 | RBM |
| `battania_fur_boots` | Fur Boots | Leather | T4 | auto | 0 | 0 | 0 | 35 | 1.5 | RBM |
| `wrapped_leather_boots` | Wrapped Leather Boots | Leather | T4 | auto | 0 | 0 | 0 | 33 | 1 | RBM |
| `battania_leather_boots` | Highborn Leather Boots | Leather | T4 | auto | 0 | 0 | 0 | 32 | 1 | RBM |
| `highland_leg_wrappings` | Woodland Leg Wrappings | Cloth | T3 | auto | 0 | 0 | 0 | 30 | 1 | RBM |
| `rough_tied_boots` | Rough Tied Boots | Leather | T3 | auto | 0 | 0 | 0 | 30 | 0.9 | RBM |
| `belted_leather_boots` | Belted Leather Boots | Leather | T3 | auto | 0 | 0 | 0 | 29 | 1.1 | RBM |
| `battania_woodland_boots` | Woodland Boots | Leather | T3 | auto | 0 | 0 | 0 | 28 | 1 | RBM |
| `highland_boots` | Forester Boots | Leather | T3 | auto | 0 | 0 | 0 | 26 | 1 | RBM |
| `tv_battania_warlord_boots` | Highland Bronze Warlord Boots | Plate | T3 | auto | 0 | 0 | 0 | 26 | 2.6 | OSA |
| `tv_battania_warlord_boots_b` | Highland Brass Warlord Boots | Plate | T3 | auto | 0 | 0 | 0 | 26 | 2.6 | OSA |
| `tv_battania_warlord_boots_c` | Highland Steel Warlord Boots | Plate | T3 | auto | 0 | 0 | 0 | 26 | 2.6 | OSA |
| `turndown_leather_boots` | Folded Leather Boots | Leather | T3 | auto | 0 | 0 | 0 | 25 | 0.9 | RBM |
| `DZ_battania_boots_a` | Chainmail Boots | Chainmail | T2 | auto | 0 | 0 | 0 | 20 | 1.8 | OSA |
| `TV_battania_boots_k` | Highland Brass Greave Boots With Green Plaid Legwraps | Plate | T2 | auto | 0 | 0 | 0 | 20 | 1.8 | OSA |
| `TV_battania_boots_l` | Highland Brass Greave Boots With Lime Plaid Legwraps | Plate | T2 | auto | 0 | 0 | 0 | 20 | 1.8 | OSA |
| `TV_battania_boots_m` | Highland Brass Greave Boots With Orange Plaid Legwraps | Plate | T2 | auto | 0 | 0 | 0 | 20 | 1.8 | OSA |
| `TV_battania_boots_n` | Highland Brass Greave Boots With Brown Legwraps | Plate | T2 | auto | 0 | 0 | 0 | 20 | 1.8 | OSA |
| `TV_battania_boots_o` | Highland Iron Greave Boots With Green Plaid Legwraps | Plate | T2 | auto | 0 | 0 | 0 | 20 | 1.8 | OSA |
| `TV_battania_boots_p` | Highland Iron Greave Boots With Lime Plaid Legwraps | Plate | T2 | auto | 0 | 0 | 0 | 20 | 1.8 | OSA |
| `TV_battania_boots_q` | Highland Iron Greave Boots With Orange Plaid Legwraps | Plate | T2 | auto | 0 | 0 | 0 | 20 | 1.8 | OSA |
| `TV_battania_boots_r` | Highland Iron Greave Boots With Brown Legwraps | Plate | T2 | auto | 0 | 0 | 0 | 20 | 1.8 | OSA |
| `TV_battania_boots_g` | Highland Leather Greave Boots With Green Plaid Legwraps | Leather | T1 | auto | 0 | 0 | 0 | 14 | 1.8 | OSA |
| `TV_battania_boots_h` | Highland Leather Greave Boots With Lime Plaid Legwraps | Leather | T1 | auto | 0 | 0 | 0 | 14 | 1.8 | OSA |
| `TV_battania_boots_i` | Highland Leather Greave Boots With Orange Plaid Legwraps | Leather | T1 | auto | 0 | 0 | 0 | 14 | 1.8 | OSA |
| `TV_battania_boots_j` | Highland Leather Greave Boots With Brown Legwraps | Leather | T1 | auto | 0 | 0 | 0 | 14 | 1.8 | OSA |
| `mp_battania_leather_boots` | Highland Leather Boots | Leather | T0 | 1 | 0 | 0 | 0 | 11 | 1 | Vanilla |
| `mp_rough_tied_boots` | Rough Tied Boots | Leather | T0 | 1 | 0 | 0 | 0 | 9 | 0.9 | Vanilla |
| `mp_ragged_boots` | Ragged Boots | Cloth | T0 | 1 | 0 | 0 | 0 | 8 | 0.8 | Vanilla |
| `TV_battania_boots_a` | Highland Shoes With Green Plaid Legwraps | Cloth | T-1 | auto | 0 | 0 | 0 | 4 | 0.8 | OSA |
| `TV_battania_boots_b` | Highland Shoes With Lime Plaid Legwraps | Cloth | T-1 | auto | 0 | 0 | 0 | 4 | 0.8 | OSA |
| `TV_battania_boots_c` | Highland Shoes With Orange Plaid Legwraps | Cloth | T-1 | auto | 0 | 0 | 0 | 4 | 0.8 | OSA |
| `TV_battania_boots_d` | Highland Shoes With Brown Legwraps | Cloth | T-1 | auto | 0 | 0 | 0 | 4 | 0.8 | OSA |
| `TV_battania_boots_s` | Highland Boots With Lime Plaid Legwraps | Cloth | T-1 | auto | 0 | 0 | 0 | 4 | 0.8 | OSA |
| `TV_battania_boots_t` | Highland Boots With Green Plaid Legwraps | Cloth | T-1 | auto | 0 | 0 | 0 | 4 | 0.8 | OSA |
| `TV_battania_boots_u` | Highland Boots With Orange Plaid Legwraps | Cloth | T-1 | auto | 0 | 0 | 0 | 4 | 0.8 | OSA |
| `TV_battania_boots_v` | Highland Boots With Brown Legwraps | Cloth | T-1 | auto | 0 | 0 | 0 | 4 | 0.8 | OSA |
| `ao_leather_shoes` | Blackened Leather Shoes | Cloth | T-1 | auto | 0 | 0 | 0 | 2 | 0.8 | OSA |
| `simple_shoes` | Simple Shoes | Cloth | T-1 | auto | 0 | 0 | 0 | 2 | 0.2 | OSA |
| `TV_battania_boots_w` | Highland Simple Boots | Cloth | T-1 | auto | 0 | 0 | 0 | 2 | 0.2 | OSA |

<!-- CULTURE:khuzait COUNT:12 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `reinforced_suede_boots` | Reinforced Suede Boots | Leather | T4 | auto | 0 | 0 | 0 | 36 | 1 | RBM |
| `leather_boots` | Fur Rimmed Curved Boots | Leather | T3 | auto | 0 | 0 | 0 | 30 | 0.8 | RBM |
| `woven_leather_boots` | Woven Leather Boots | Leather | T3 | auto | 0 | 0 | 0 | 30 | 0.7 | RBM |
| `eastern_leather_boots` | Leather Riding Boots | Leather | T3 | auto | 0 | 0 | 0 | 29 | 1.1 | RBM |
| `steppe_leather_boots` | Sturdy Leather Boots | Leather | T3 | auto | 0 | 0 | 0 | 29 | 0.8 | RBM |
| `khuzait_curved_boots` | Curved Boots | Leather | T3 | auto | 0 | 0 | 0 | 28 | 0.9 | RBM |
| `AO_durkhan_boots_c` | Eastern Tassled Splint Boots | Plate | T2 | auto | 0 | 0 | 0 | 22 | 2.7 | OSA |
| `eastern_splint_boots` | Eastern Splint Boots | Plate | T2 | auto | 0 | 0 | 0 | 22 | 2.7 | OSA |
| `AO_durkhan_boots_b` | Eastern Tassled Boots With Studded Leather | Leather | T1 | auto | 0 | 0 | 0 | 14 | 0.9 | OSA |
| `hmj_eastern_leather_boots` | Eastern Studded Boots | Leather | T1 | auto | 0 | 0 | 0 | 14 | 0.9 | OSA |
| `ao_durkhan_tassled_boots` | Eastern Tassled Boots | Leather | T-1 | auto | 0 | 0 | 0 | 5 | 0.8 | OSA |
| `TV_khuzait_boots_b` | Eastern Suede Curved Boots | Leather | T-1 | auto | 0 | 0 | 0 | 5 | 0.9 | OSA |

<!-- CULTURE:aserai COUNT:8 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `wrapped_shoes` | Wrapped Shoes | Cloth | T4 | auto | 0 | 0 | 0 | 35 | 0.8 | RBM |
| `TV_aserai_boots_c` | Southern Boots With Gilded Greaves | Plate | T2 | auto | 0 | 0 | 0 | 24 | 2.7 | OSA |
| `TV_aserai_boots_d` | Southern Blackened Boots With Iron Greaves | Plate | T2 | auto | 0 | 0 | 0 | 24 | 2.7 | OSA |
| `southern_moccasins` | Moccasins | Cloth | T1 | auto | 0 | 0 | 0 | 16 | 0.5 | RBM |
| `TV_aserai_boots_e` | Southern Boots With Leather Greaves | Leather | T1 | auto | 0 | 0 | 0 | 14 | 2.7 | OSA |
| `TV_aserai_boots_e2` | Southern Blackened Boots With Leather Greaves | Leather | T1 | auto | 0 | 0 | 0 | 14 | 2.7 | OSA |
| `TV_moccasins_b` | Southern Decorated Moccasins | Cloth | T-1 | auto | 0 | 0 | 0 | 2 | 0.5 | OSA |
| `TV_moccasins_c` | Southern Noble Moccasins | Cloth | T-1 | auto | 0 | 0 | 0 | 2 | 0.5 | OSA |

<!-- CULTURE:nord COUNT:8 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `hybrid_armor_boots_b` | Majectic Iron Boots | Plate | T5 | auto | 0 | 0 | 0 | 59 | 2 | RBM |
| `RBM_master_assassin_boot` | Master Assassin Boots | Chainmail | T5 | auto | 0 | 0 | 0 | 48 | 1 | RBM |
| `improved_assassin_boot_q5` | Blackened Boots | Leather | T5 | auto | 0 | 0 | 0 | 38 | 1 | RBM |
| `northman_raider_boot` | Northern Raider Boots | Leather | T3 | auto | 0 | 0 | 0 | 30 | 0.4 | RBM |
| `assassin_boot` | Blackened Boots | Leather | T3 | auto | 0 | 0 | 0 | 28 | 1 | RBM |
| `strapped_jackboots` | Strapped Jackboots | Leather | T3 | auto | 0 | 0 | 0 | 28 | 0.7 | RBM |
| `nord_casual_boots` | Short Flap Boots | Leather | T1 | auto | 0 | 0 | 0 | 18 | 0.5 | RBM |
| `nord_poor_boot` | Worn Leather Boots | Leather | T1 | auto | 0 | 0 | 0 | 17 | 0.5 | RBM |

<!-- CULTURE:neutral_culture COUNT:32 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `tacky_bandit_boot` | Gilded Raider Boots | Plate | T5 | auto | 0 | 0 | 0 | 50 | 2.6 | RBM |
| `bandit_hybrid_armor_boots_a` | Raider Bronze Boots | Plate | T5 | auto | 0 | 0 | 0 | 40 | 2.4 | RBM |
| `ragged_boots` | Ragged Boots | Cloth | T3 | auto | 0 | 0 | 0 | 26 | 0.8 | RBM |
| `strapped_shoes` | Strapped Shoes | Cloth | T2 | auto | 0 | 0 | 0 | 22 | 0.7 | RBM |
| `mp_lamellar_plate_boots` | Lamellar Plate Boots | Plate | T2 | 1 | 0 | 0 | 0 | 20 | 3.5 | Vanilla |
| `leather_shoes` | Leather Shoes | Leather | T2 | auto | 0 | 0 | 0 | 19 | 0.7 | RBM |
| `mp_northern_plated_boots` | Plated Boots | Plate | T1 | 1 | 0 | 0 | 0 | 17 | 2.9 | Vanilla |
| `mp_battania_warlord_boots` | Highland Warlord Boots | Plate | T1 | 1 | 0 | 0 | 0 | 16 | 2.6 | Vanilla |
| `mp_decorated_imperial_boots` | Decorated Imperial Boots | Plate | T1 | 1 | 0 | 0 | 0 | 16 | 2.3 | Vanilla |
| `mp_mail_cavalier_boots` | Mail Cavalier Boots | Chainmail | T1 | 1 | 0 | 0 | 0 | 15 | 1.8 | Vanilla |
| `mp_strapped_mail_chausses` | Strapped Mail Chausses | Chainmail | T1 | 1 | 0 | 0 | 0 | 15 | 3 | Vanilla |
| `ladys_shoe` | Ladies Shoes | Cloth | T1 | auto | 0 | 0 | 0 | 14 | 0.6 | RBM |
| `mp_plated_strip_boots` | Splint Boots | Plate | T1 | 1 | 0 | 0 | 0 | 14 | 2.7 | Vanilla |
| `mp_mail_chausses` | Mail Chausses | Chainmail | T1 | 1 | 0 | 0 | 0 | 13 | 2.9 | Vanilla |
| `mp_battania_fur_boots` | Highland Fur Boots | Leather | T1 | 1 | 0 | 0 | 0 | 12 | 1.5 | Vanilla |
| `mp_eastern_leather_boots` | Curved Boots | Leather | T0 | 1 | 0 | 0 | 0 | 11 | 1.1 | Vanilla |
| `mp_battania_woodland_boots` | Highland Woodland Boots | Leather | T0 | 1 | 0 | 0 | 0 | 10 | 1 | Vanilla |
| `mp_khuzait_curved_boots` | Stapped Curved Boots | Leather | T0 | 1 | 0 | 0 | 0 | 10 | 0.1 | Vanilla |
| `mp_leather_cavalier_boots` | Leather Cavalier Boots | Leather | T0 | 1 | 0 | 0 | 0 | 10 | 0.9 | Vanilla |
| `mp_empire_horseman_boots` | Courser Boots | Leather | T0 | 1 | 0 | 0 | 0 | 9 | 1.4 | Vanilla |
| `mp_highland_boots` | Highland Boots | Leather | T0 | 1 | 0 | 0 | 0 | 9 | 1 | Vanilla |
| `mp_reinforced_suede_boots` | Reinforced Suede Boots | Leather | T0 | 1 | 0 | 0 | 0 | 9 | 4 | Vanilla |
| `mp_wrapped_leather_boots` | Wrapped Leather Boots | Leather | T0 | 1 | 0 | 0 | 0 | 9 | 1 | Vanilla |
| `mp_woven_leather_boots` | Woven Leather Boots | Leather | T0 | 1 | 0 | 0 | 0 | 8 | 0.3 | Vanilla |
| `mp_fine_town_boots` | Fine Town Boots | Leather | T0 | 1 | 0 | 0 | 0 | 7 | 0.5 | Vanilla |
| `mp_leather_boots` | Leather Boots | Leather | T0 | 1 | 0 | 0 | 0 | 7 | 0.8 | Vanilla |
| `mp_steppe_leather_boots` | Steppe Leather Boots | Leather | T0 | 1 | 0 | 0 | 0 | 7 | 0.8 | Vanilla |
| `mp_highland_leg_wrappings` | Highland Leg Wrappings | Cloth | T0 | 1 | 0 | 0 | 0 | 6 | 1 | Vanilla |
| `mp_strapped_leather_boots` | Strapped Leather Boots | Cloth | T-1 | 1 | 0 | 0 | 0 | 5 | 0.9 | Vanilla |
| `mp_strapped_shoes` | Strapped Shoes | Cloth | T-1 | 1 | 0 | 0 | 0 | 4 | 0.7 | Vanilla |
| `mp_wrapped_shoes` | Wrapped Shoes | Cloth | T-1 | 1 | 0 | 0 | 0 | 4 | 2 | Vanilla |
| `mp_leather_shoes` | Leather Shoes | Leather | T-1 | 1 | 0 | 0 | 0 | 3 | 0.7 | Vanilla |



