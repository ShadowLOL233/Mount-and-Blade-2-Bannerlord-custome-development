# 护甲数据总表 · 肩甲 / 披风（Cape）

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

### Tier 公式（本槽 typeMultiplier = **1.8** · 最高）

```
raw    = 1.2·head + body + leg + arm
scaled = raw × 1.8
tier   = clamp(round(scaled × 0.1 − 0.4), 0, 6) − 1
```

肩甲槽 typeMultiplier 为 5 槽最高 · 同 raw 下 tier 偏高 · 游戏里披风 /肩甲档位分化鲜明。

---

## 数据表

本文档只覆盖 Cape（肩甲 / 披风 / 肩披）· 其他 4 槽见同目录。
文化排序：**empire → vlandia → sturgia → battania → khuzait → aserai → nord → looters → neutral_culture → (none)**。
每文化内：**tier desc → rawArmor desc → stringId asc**。


<!-- SLOT:Cape COUNT:474 -->

<!-- SOURCE_DIST -->
| Source | Count |
|---|---:|
| OSA | 310 |
| RBM | 111 |
| Vanilla | 53 |

<!-- TIER_DIST -->
| Tier | Count |
|---|---:|
| T5 | 29 |
| T4 | 25 |
| T3 | 143 |
| T2 | 41 |
| T1 | 92 |
| T0 | 112 |
| T-1 | 32 |

<!-- CULTURE:empire COUNT:111 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `imperial_lamellar_shoulders` | Heavy Lamellar Pauldrons | Plate | T5 | auto | 0 | 55 | 0 | 0 | 3.5 | RBM |
| `varangian_bra_scale` | Decorated Leather Harness over Scale | Plate | T5 | auto | 0 | 40 | 0 | 0 | 4 | RBM |
| `varangian_bra_mail` | Decorated Leather Harness over Mail | Chainmail | T5 | auto | 0 | 35 | 0 | 0 | 3.2 | RBM |
| `imperial_studded_strip_shoulders` | Legionary Reinforced Studded Harness | Plate | T5 | auto | 0 | 34 | 0 | 0 | 4.1 | RBM |
| `AR_imperial_shoulders_y` | Imperial Decorated Leather Harness Over Scale With Cape | Plate | T4 | auto | 0 | 19 | 12 | 0 | 14 | OSA |
| `mp_varangian_bra_scale` | Decorated Leather Harness over Scale | Plate | T4 | 1 | 0 | 19 | 12 | 0 | 14 | Vanilla |
| `studded_imperial_neckguard` | Neckguard with Bronze Plate Pauldrons | Plate | T4 | auto | 0 | 31 | 0 | 0 | 3.6 | RBM |
| `AR_lion_head_b` | Plated Lion Head Cloak | Leather | T4 | auto | 7 | 18 | 4 | 0 | 7.4 | OSA |
| `pauldron_cape_a` | Legionary Cape | Plate | T4 | auto | 0 | 30 | 0 | 0 | 3.5 | RBM |
| `AR_lion_head_a` | Decorated Lion Head Cloak | Leather | T4 | auto | 7 | 16 | 4 | 0 | 6 | OSA |
| `AR_lion_head_d` | Lion Head Cloak | Leather | T4 | auto | 7 | 16 | 4 | 0 | 6 | OSA |
| `AR_wolf_shoulder_a` | Imperial Lamellar Shoulders With Wolf Pelt | Plate | T4 | auto | 0 | 18 | 10 | 0 | 3.8 | OSA |
| `AR_wolf_shoulder_b` | Imperial Lamellar Shoulders With Brown Wolf Pelt | Plate | T4 | auto | 0 | 18 | 10 | 0 | 3.6 | OSA |
| `AR_gladiator_shoulder_a` | Single Lamellar Pauldron Over Leather | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.5 | OSA |
| `AR_imperial_lamellar_cape_a` | Imperial Lamellar With Long Cape | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.9 | OSA |
| `AR_imperial_lamellar_cape_a2` | Imperial Gilded Lamellar With Long Cape | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.9 | OSA |
| `AR_imperial_lamellar_cape_c` | Imperial Lamellar With Fur Cape | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.9 | OSA |
| `AR_imperial_shoulders_a` | Imperial Lamellar Shoulders Over Leather | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.8 | OSA |
| `AR_imperial_shoulders_a2` | Imperial Gilded Lamellar Shoulders Over Leather | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.5 | OSA |
| `AR_imperial_shoulders_d` | Imperial Scale Shoulders With Lamellar | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.8 | OSA |
| `AR_imperial_shoulders_e` | Imperial Lamellar Shoulders With Leopard Pelt | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.8 | OSA |
| `AR_imperial_shoulders_e2` | Imperial Gilded Lamellar Shoulders With Leopard Pelt | Plate | T3 | auto | 0 | 18 | 9 | 0 | 4.8 | OSA |
| `AR_imperial_shoulders_e3` | Imperial Lamellar Shoulders With Snow Leopard Pelt | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.8 | OSA |
| `AR_imperial_shoulders_e4` | Imperial Gilded Lamellar Shoulders With Snow Leopard Pelt | Plate | T3 | auto | 0 | 18 | 9 | 0 | 4.8 | OSA |
| `AR_imperial_shoulders_q` | Imperial Lamellar Shoulders With Lion Pelt | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.8 | OSA |
| `AR_imperial_shoulders_q2` | Imperial Gilded Lamellar Shoulders With Lion Pelt | Plate | T3 | auto | 0 | 18 | 9 | 0 | 4.8 | OSA |
| `AR_noble_collar_b` | Imperial Noble Lamellar With Long Cape | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.9 | OSA |
| `lamellar_scarf` | Imperial Lamellar Shoulders with Scarf | Plate | T3 | auto | 0 | 18 | 9 | 0 | 4 | OSA |
| `varangian_bra_padded` | Decorated Leather Harness with Padding | Leather | T3 | auto | 0 | 27 | 0 | 0 | 3.2 | RBM |
| `AR_imperial_lamellar_cape_b` | Imperial Lamellar With Cape | Plate | T3 | auto | 0 | 17 | 9 | 0 | 3.9 | OSA |
| `AR_imperial_lamellar_cape_b2` | Imperial Gilded Lamellar With Cape | Plate | T3 | auto | 0 | 17 | 9 | 0 | 3.9 | OSA |
| `AR_imperial_lamellar_shoulders_a` | Imperial Gilded Lamellar Shoulders | Plate | T3 | auto | 0 | 17 | 9 | 0 | 3.5 | OSA |
| `AR_imperial_shoulders_f` | Imperial Lamellar Pauldrons With Leopard Pelt | Plate | T3 | auto | 0 | 16 | 10 | 0 | 3.6 | OSA |
| `AR_imperial_shoulders_f2` | Imperial Lamellar Pauldrons With Snow Leopard Pelt | Plate | T3 | auto | 0 | 16 | 10 | 0 | 3.6 | OSA |
| `AR_imperial_shoulders_h` | Imperial Lamellar Pauldrons With Scarf | Plate | T3 | auto | 0 | 16 | 10 | 0 | 3.6 | OSA |
| `AR_imperial_shoulders_r` | Imperial Lamellar Pauldrons With Lion Pelt | Plate | T3 | auto | 0 | 16 | 10 | 0 | 3.6 | OSA |
| `AR_gladiator_shoulder_b` | Single Lamellar Pauldron Over Plated Leather | Plate | T3 | auto | 0 | 16 | 8 | 0 | 4.1 | OSA |
| `AR_imperial_lamellar_cape_e` | Imperial Lamellar With Long Plaid Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.9 | OSA |
| `AR_imperial_lamellar_cape_f` | Imperial Lamellar With Long Striped Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.9 | OSA |
| `AR_imperial_lamellar_cape_g` | Imperial Lamellar With Bear Pelt | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.9 | OSA |
| `AR_imperial_shoulders_b` | Imperial Lamellar Pauldrons Over Leather | Plate | T3 | auto | 0 | 16 | 8 | 0 | 2.7 | OSA |
| `AR_imperial_shoulders_c` | Imperial Scale Shoulders | Plate | T3 | auto | 0 | 16 | 8 | 0 | 2.7 | OSA |
| `AR_imperial_shoulders_j` | Studded Brass Imperial Neckguard With Leopard Pelt | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.8 | OSA |
| `AR_imperial_shoulders_j2` | Studded Brass Imperial Neckguard With Snow Leopard Pelt | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.8 | OSA |
| `AR_imperial_shoulders_k` | Studded Iron Imperial Neckguard With Leopard Pelt | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.8 | OSA |
| `AR_imperial_shoulders_k2` | Studded Iron Imperial Neckguard With Snow Leopard Pelt | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.8 | OSA |
| `AR_imperial_shoulders_l` | Imperial Brass Scale Shoulders | Plate | T3 | auto | 0 | 16 | 8 | 0 | 2.7 | OSA |
| `AR_noble_collar_a` | Imperial Noble Shoulders With Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 1.4 | OSA |
| `AR_wolf_shoulder_c` | Imperial Lamellar Pauldrons With Wolf Pelt | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.8 | OSA |
| `AR_wolf_shoulder_d` | Imperial Lamellar Pauldrons With Brown Wolf Pelt | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.6 | OSA |
| `AR_wolf_shoulder_g` | Imperial Brass Scale Shoulders With Wolf Pelt | Plate | T3 | auto | 0 | 16 | 8 | 0 | 2.7 | OSA |
| `AR_wolf_shoulder_g2` | Imperial Brass Scale Shoulders With Brown Wolf Pelt | Plate | T3 | auto | 0 | 16 | 8 | 0 | 2.7 | OSA |
| `BA_empire_shoulders_a` | Imperial Studded Brass Pauldrons With Mail | Chainmail | T3 | auto | 0 | 16 | 8 | 0 | 2.7 | OSA |
| `BA_empire_shoulders_b` | Imperial Studded Iron Pauldrons With Mail | Chainmail | T3 | auto | 0 | 16 | 8 | 0 | 2.7 | OSA |
| `DZ_empire_shoulder_a` | Imperial Plumed Lamellar Pauldrons Over Leather | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.9 | OSA |
| `DZ_empire_shoulder_b` | Imperial Plumed Lamellar Pauldrons Over Leather With Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.9 | OSA |
| `empire_warrior_padded_armor_shoulder` | Legionary Padded Straps | Leather | T3 | auto | 0 | 24 | 0 | 0 | 1.6 | RBM |
| `AR_imperial_shoulders_za` | Imperial Studded Brass Neckguard With Long Cape | Plate | T3 | auto | 0 | 15 | 8 | 0 | 3.6 | OSA |
| `AR_imperial_shoulders_zb` | Imperial Studded Iron Neckguard With Long Cape | Plate | T3 | auto | 0 | 15 | 8 | 0 | 3.6 | OSA |
| `a_empire_plated_shoulder_b` | Lamellar Pauldrons | Plate | T3 | auto | 0 | 22 | 0 | 0 | 4.1 | RBM |
| `AR_imperial_lamellar_cape_d` | Imperial Strip Shoulders With Long Cape | Plate | T3 | auto | 0 | 16 | 6 | 0 | 3.9 | OSA |
| `AR_wolf_shoulder_f` | Imperial Mail Shoulders With Wolf Pelt | Chainmail | T3 | auto | 0 | 14 | 8 | 0 | 2.7 | OSA |
| `AR_wolf_shoulder_f2` | Imperial Mail Shoulders With Brown Wolf Pelt | Chainmail | T3 | auto | 0 | 14 | 8 | 0 | 2.7 | OSA |
| `varangian_bra_royal` | Caped Leather Harness | Leather | T3 | auto | 0 | 22 | 0 | 0 | 2.7 | RBM |
| `AR_imperial_shoulders_z` | Imperial Studded Iron Neckguard | Plate | T2 | auto | 0 | 14 | 7 | 0 | 3.6 | OSA |
| `empire_plate_armor_shoulder_b` | Iron Plate Pauldrons | Plate | T2 | auto | 0 | 21 | 0 | 0 | 3.6 | RBM |
| `a_pauldron_cape_b` | Bronze Pauldrons with Neck Guard | Plate | T2 | auto | 0 | 20 | 0 | 0 | 3.5 | RBM |
| `BA_empire_shoulders_c` | Imperial Brass Pauldrons With Mail | Chainmail | T2 | auto | 0 | 14 | 6 | 0 | 2.7 | OSA |
| `BA_empire_shoulders_d` | Imperial Iron Pauldrons With Mail | Chainmail | T2 | auto | 0 | 14 | 6 | 0 | 2.7 | OSA |
| `AR_imperial_shoulders_g` | Imperial Bronze Pauldrons With Scarf | Plate | T2 | auto | 0 | 18 | 0 | 0 | 3.5 | OSA |
| `AR_wolf_shoulder_e` | Imperial Leather Shoulders With Wolf Pelt | Leather | T2 | auto | 0 | 10 | 8 | 0 | 2.1 | OSA |
| `AR_wolf_shoulder_e2` | Imperial Leather Shoulders With Brown Wolf Pelt | Leather | T2 | auto | 0 | 10 | 8 | 0 | 2.1 | OSA |
| `mp_varangian_bra_padded` | Decorated Leather Harness with Padding | Leather | T2 | 1 | 0 | 10 | 8 | 0 | 3.2 | Vanilla |
| `varangian_bra_basic` | Decorated Leather Harness | Leather | T2 | auto | 0 | 18 | 0 | 0 | 2.5 | RBM |
| `empire_plate_armor_shoulder_a` | Bronze Plate Pauldrons | Plate | T2 | auto | 0 | 17 | 0 | 0 | 3.6 | RBM |
| `AR_empire_shoulders_b` | Imperial Padded Shoulders With Scarf | Leather | T1 | auto | 0 | 12 | 4 | 0 | 1.6 | OSA |
| `AR_imperial_shoulders_i` | Studded Imperial Neckguard With Scarf | Plate | T1 | auto | 0 | 16 | 0 | 0 | 3.6 | OSA |
| `AR_imperial_shoulders_n` | Imperial Mail Shoulders | Chainmail | T1 | auto | 0 | 12 | 4 | 0 | 2.7 | OSA |
| `AR_imperial_shoulders_u` | Imperial Alternating Scale Pauldrons | Plate | T1 | auto | 0 | 16 | 0 | 0 | 4.5 | OSA |
| `AR_imperial_shoulders_v` | Imperial Alternating Scale Shoulders | Plate | T1 | auto | 0 | 16 | 0 | 0 | 4.1 | OSA |
| `AR_imperial_shoulders_w` | Imperial Steel Scale Pauldrons | Plate | T1 | auto | 0 | 16 | 0 | 0 | 4.5 | OSA |
| `AR_imperial_shoulders_x` | Imperial Steel Scale Shoulders | Plate | T1 | auto | 0 | 16 | 0 | 0 | 4.1 | OSA |
| `woven_leather_shoulders` | Woven Leather Shoulders | Leather | T1 | auto | 0 | 16 | 0 | 0 | 1.6 | RBM |
| `a_pauldron_cape_c` | Bronze Pauldrons | Plate | T1 | auto | 0 | 15 | 0 | 0 | 3.5 | RBM |
| `AR_imperial_shoulders_o` | Imperial Studded Strip Shoulders With Lamellar Pauldrons And Focale | Plate | T1 | auto | 0 | 7 | 6 | 0 | 3.9 | OSA |
| `AR_imperial_shoulders_o2` | Imperial Studded Strip Shoulders With Lamellar Pauldrons And Cape | Plate | T1 | auto | 0 | 7 | 6 | 0 | 3.9 | OSA |
| `TV_empire_shoulders_b` | Imperial Leather Shoulders With Lamellar Pauldrons | Plate | T1 | auto | 0 | 7 | 6 | 0 | 3.9 | OSA |
| `a_empire_plated_shoulder_a` | Legionary Studded Harness | Leather | T1 | auto | 0 | 12 | 0 | 0 | 4.1 | RBM |
| `AR_imperial_leather_cape_a` | Imperial Leather Shoulders With Cape | Leather | T1 | auto | 0 | 12 | 0 | 0 | 4 | OSA |
| `AR_imperial_leather_cape_b` | Imperial Leather Shoulders With Striped Cloak | Leather | T1 | auto | 0 | 12 | 0 | 0 | 4 | OSA |
| `AR_imperial_leather_cape_b2` | Imperial Leather Shoulders With Cloak | Leather | T1 | auto | 0 | 12 | 0 | 0 | 4 | OSA |
| `mp_varangian_bra_royal` | Caped Leather Harness | Leather | T1 | 1 | 0 | 5 | 7 | 0 | 2.7 | Vanilla |
| `TV_empire_shoulders_a` | Imperial Studded Leather Shoulders With Cape | Plate | T1 | auto | 0 | 8 | 4 | 0 | 3.6 | OSA |
| `tv_imperial_leather_cape_a` | Imperial Leather Shoulders With Plaid Cape | Leather | T1 | auto | 0 | 12 | 0 | 0 | 4 | OSA |
| `tv_imperial_leather_cape_b` | Imperial Leather Shoulders With Striped Cape | Leather | T1 | auto | 0 | 12 | 0 | 0 | 4 | OSA |
| `AR_imperial_shoulders_p` | Imperial Studded Strip Shoulders With Focale | Leather | T1 | auto | 0 | 7 | 4 | 0 | 3.6 | OSA |
| `mp_varangian_bra_basic` | Decorated Leather Harness | Leather | T1 | 1 | 0 | 4 | 7 | 0 | 2.5 | Vanilla |
| `AR_imperial_shoulders_m` | Imperial Leather Shoulders | Leather | T0 | auto | 0 | 8 | 2 | 0 | 2.1 | OSA |
| `AR_imperial_shoulders_s` | Imperial Studded Leather Shoulders With Focale | Leather | T0 | auto | 0 | 8 | 2 | 0 | 2.1 | OSA |
| `AR_imperial_shoulders_t` | Imperial Trimmed Leather Shoulders | Leather | T0 | auto | 0 | 8 | 2 | 0 | 2.1 | OSA |
| `AR_imperial_shoulders_p2` | Imperial Studded Shoulders With Focale | Leather | T0 | auto | 0 | 6 | 0 | 0 | 3.6 | OSA |
| `mp_woven_leather_shoulders` | Woven Leather Shoulders | Leather | T0 | 1 | 0 | 6 | 0 | 0 | 1.6 | Vanilla |
| `TV_battania_cloak_o2` | Plaid Cloth Cape | Cloth | T0 | auto | 0 | 6 | 0 | 0 | 4.5 | OSA |
| `TV_battania_cloak_o3` | Striped Cloth Cape | Cloth | T0 | auto | 0 | 6 | 0 | 0 | 4.5 | OSA |
| `ao_imperial_noblemans_sash` | Imperial Noble Sash | Cloth | T0 | auto | 0 | 3 | 2 | 0 | 0.5 | OSA |
| `TV_battania_cloak_m` | Long Cape | Cloth | T0 | auto | 0 | 3 | 2 | 0 | 0.5 | OSA |
| `TV_battania_cloak_m2` | Long Plaid Cape | Cloth | T0 | auto | 0 | 3 | 2 | 0 | 0.5 | OSA |
| `TV_battania_cloak_m3` | Long Striped Cape | Cloth | T0 | auto | 0 | 3 | 2 | 0 | 0.5 | OSA |
| `AR_leopard_pelt_a` | Snow Leopard Pelt | Leather | T-1 | auto | 0 | 4 | 0 | 0 | 2 | OSA |
| `TV_battania_cloak_o` | Cloth Cape | Cloth | T-1 | auto | 0 | 2 | 0 | 0 | 0.2 | OSA |
| `ao_focale` | Imperial Focale | Cloth | T-1 | auto | 0 | 1 | 0 | 0 | 0.1 | OSA |

<!-- CULTURE:vlandia COUNT:48 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `noble_pauldron_with_scarf` | Reinforced Ornate Pauldrons | Plate | T5 | auto | 0 | 88 | 0 | 0 | 3.2 | RBM |
| `noble_pauldron_with_cape` | Ornate Pauldrons with Cape | Plate | T5 | auto | 0 | 68 | 0 | 0 | 1.8 | RBM |
| `noble_pauldron` | Ornate Pauldrons | Plate | T5 | auto | 0 | 60 | 0 | 0 | 1.2 | RBM |
| `pauldron_over_scale_armor` | Reinforced Ornate Pauldrons over Scale | Plate | T5 | auto | 0 | 50 | 0 | 0 | 3.4 | RBM |
| `pauldron_with_cape` | Pauldrons with Cape | Plate | T5 | auto | 0 | 33 | 0 | 0 | 1.3 | RBM |
| `TV_vlandia_shoulders_s` | Western Heavy Trimmed Mail And Steel Scale Shoulders | Plate | T4 | auto | 0 | 19 | 12 | 0 | 4 | OSA |
| `TV_vlandia_shoulders_t` | Western Heavy Trimmed Mail And Steel Scale Shoulders With Cloak | Plate | T4 | auto | 0 | 19 | 12 | 0 | 4 | OSA |
| `TV_vlandia_shoulders_f` | Western Lamellar Pauldrons With Long Cape | Plate | T4 | auto | 0 | 19 | 9 | 0 | 3.9 | OSA |
| `AR_vlandia_shoulders_f` | Western Reinforced Ornate Pauldrons With Cape | Plate | T3 | auto | 0 | 15 | 12 | 0 | 11.4 | OSA |
| `TV_vlandia_shoulders_e` | Western Lamellar Pauldrons Over Leather | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.8 | OSA |
| `AR_imperial_lamellar_cape_h` | Lamellar With Long Western Cape | Plate | T3 | auto | 0 | 18 | 8 | 0 | 3.9 | OSA |
| `AR_imperial_lamellar_cape_j` | Lamellar With Western Fur Cape | Plate | T3 | auto | 0 | 18 | 8 | 0 | 3.9 | OSA |
| `TV_vlandia_shoulders_d` | Western Lamellar Pauldrons | Plate | T3 | auto | 0 | 17 | 9 | 0 | 3.5 | OSA |
| `scale_shoulder_armor` | Scale Shoulderguards | Plate | T3 | auto | 0 | 25 | 0 | 0 | 1.7 | RBM |
| `AR_vlandia_shoulders_d` | Western Lamellar Pauldrons Over Leather | Plate | T3 | auto | 0 | 16 | 8 | 0 | 2.7 | OSA |
| `TV_vlandia_shoulders_r` | Western Heavy Trimmed Mail Shoulders With Shoulder Plates | Chainmail | T3 | auto | 0 | 16 | 8 | 0 | 2.8 | OSA |
| `AR_imperial_lamellar_cape_k` | Strip Shoulders With Western Cape | Plate | T3 | auto | 0 | 17 | 6 | 0 | 3.9 | OSA |
| `AR_imperial_lamellar_cape_i` | Lamellar With Western Cape | Plate | T3 | auto | 0 | 14 | 8 | 0 | 3.9 | OSA |
| `padded_leather_shoulders` | Padded Leather Shoulders | Cloth | T3 | auto | 0 | 22 | 0 | 0 | 1.4 | RBM |
| `TV_vlandia_shoulders_q` | Western Heavy Trimmed Mail Shoulders | Chainmail | T3 | auto | 0 | 14 | 8 | 0 | 2.8 | OSA |
| `chainmail_shoulder_armor` | Reinforced Mail Shoulders | Chainmail | T2 | auto | 0 | 20 | 0 | 0 | 1.4 | RBM |
| `AR_vlandia_shoulders_a` | Western Reinforced Leather Lamellar Shoulders | Plate | T2 | auto | 0 | 10 | 8 | 0 | 4.2 | OSA |
| `AR_vlandia_shoulders_e` | Western Leather Shoulders | Leather | T1 | auto | 0 | 12 | 4 | 0 | 2.7 | OSA |
| `TV_vlandia_shoulders_n` | Western Trimmed Mailled Leather Shoulders | Chainmail | T1 | auto | 0 | 12 | 4 | 0 | 2.7 | OSA |
| `TV_vlandia_shoulders_o` | Western Trimmed Mailled Leather Shoulders With Cape | Chainmail | T1 | auto | 0 | 12 | 4 | 0 | 2.7 | OSA |
| `TV_vlandia_shoulders_p` | Western Trimmed Mailled Leather Shoulders With Cloak | Chainmail | T1 | auto | 0 | 12 | 4 | 0 | 2.7 | OSA |
| `DZ_vlandia_shoulders_a` | Western Rough Chainmail Shoulders | Chainmail | T1 | auto | 0 | 14 | 0 | 0 | 2.8 | OSA |
| `DZ_vlandia_shoulders_b` | Western Rough Mail Shoulders | Chainmail | T1 | auto | 0 | 14 | 0 | 0 | 2.8 | OSA |
| `leather_shoulder_b` | Western Plated Leather Shoulders | Plate | T1 | auto | 0 | 14 | 0 | 0 | 3.4 | OSA |
| `mercenary_padding_cape` | Western Padded Mail Shoulders | Chainmail | T1 | auto | 0 | 14 | 0 | 0 | 2.8 | OSA |
| `TV_vlandia_shoulders_b` | Western Mail Hood | Chainmail | T1 | auto | 0 | 14 | 0 | 0 | 2.8 | OSA |
| `TV_vlandia_shoulders_b2` | Western Chainmail Hood | Chainmail | T1 | auto | 0 | 14 | 0 | 0 | 2.8 | OSA |
| `TV_vlandia_shoulders_c` | Western Heavy Mail Shoulders | Chainmail | T1 | auto | 0 | 14 | 0 | 0 | 2.8 | OSA |
| `tv_battania_cloak_k` | Long Western Cape | Plate | T1 | auto | 0 | 12 | 0 | 0 | 4 | OSA |
| `TV_vlandia_shoulders_m` | Western Trimmed Leather Shoulders | Leather | T1 | auto | 0 | 10 | 2 | 0 | 2.1 | OSA |
| `leather_shoulder_a` | Western Strapped Leather Shoulders | Leather | T1 | auto | 0 | 11 | 0 | 0 | 2.6 | OSA |
| `TV_vlandia_shoulders_a` | Western Mail Shoulders | Chainmail | T1 | auto | 0 | 11 | 0 | 0 | 2.7 | OSA |
| `AR_vlandia_shoulders_b` | Western Leather Lamellar Shoulders | Plate | T0 | auto | 0 | 5 | 5 | 0 | 4 | OSA |
| `hood` | Hood | Cloth | T0 | auto | 0 | 9 | 0 | 0 | 0.4 | RBM |
| `AR_fur_cape_e` | Furred Long Western Cape | Cloth | T0 | auto | 0 | 8 | 0 | 0 | 4 | OSA |
| `TV_vlandia_shoulders_g` | Western Padded Leather Hood | Leather | T0 | auto | 0 | 8 | 0 | 0 | 1.4 | OSA |
| `TV_vlandia_shoulders_k` | Western Furred Cape | Cloth | T0 | auto | 0 | 8 | 0 | 0 | 4 | OSA |
| `TV_vlandia_shoulders_l` | Western Patterned Furred Cape | Cloth | T0 | auto | 0 | 8 | 0 | 0 | 4 | OSA |
| `green_hood` | Green Hood | Cloth | T0 | auto | 0 | 7 | 0 | 0 | 0.5 | RBM |
| `TV_vlandia_shoulders_i` | Western Patterned Cape | Cloth | T0 | auto | 0 | 6 | 0 | 0 | 4.5 | OSA |
| `TV_vlandia_shoulders_j` | Western Hooded Cloak | Cloth | T0 | auto | 0 | 6 | 0 | 0 | 4.5 | OSA |
| `TV_vlandia_shoulders_h` | Western Long Cape | Cloth | T0 | auto | 0 | 3 | 2 | 0 | 0.5 | OSA |
| `AR_vlandia_shoulders_c` | Western Leather Lamellar Pauldrons | Plate | T-1 | auto | 0 | 2 | 2 | 0 | 2 | OSA |

<!-- CULTURE:sturgia COUNT:43 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `brass_lamellar_shoulder_white` | Lamellar Pauldrons | Plate | T5 | auto | 0 | 40 | 0 | 0 | 5 | RBM |
| `berserkr_fur_c` | Rough Bear Pelt | Leather | T4 | auto | 7 | 16 | 8 | 0 | 5 | OSA |
| `berserkr_fur_a` | Plated Bear Fur | Leather | T4 | auto | 7 | 18 | 4 | 0 | 7.4 | OSA |
| `mail_shoulders` | Mail Shoulder Guards | Chainmail | T4 | auto | 0 | 30 | 0 | 0 | 2.7 | RBM |
| `berserkr_fur_b` | Bear Pelt | Leather | T4 | auto | 7 | 16 | 4 | 0 | 6 | OSA |
| `TV_battania_shoulders_c` | Brown Bear Pelt | Leather | T4 | auto | 7 | 16 | 4 | 0 | 6 | OSA |
| `AR_sturgia_shoulders_c` | Northern Brass Scale Shoulders With Cape | Plate | T4 | auto | 0 | 20 | 8 | 0 | 3.9 | OSA |
| `AR_sturgia_shoulders_d` | Northern Iron Scale Shoulders With Cape | Plate | T4 | auto | 0 | 20 | 8 | 0 | 3.9 | OSA |
| `a_brass_lamellar_shoulder_white_b` | Reinforced Lamellar Shoulder Guards | Plate | T3 | auto | 0 | 25 | 0 | 0 | 5 | RBM |
| `brass_scale_shoulders` | Decorated Scale Shoulder Guards | Plate | T3 | auto | 0 | 25 | 0 | 0 | 2.2 | RBM |
| `AR_sturgia_shoulders_a2` | Northern Brass Lamellar Shoulders | Plate | T3 | auto | 0 | 16 | 8 | 0 | 1.4 | OSA |
| `AR_sturgia_shoulders_a3` | Northern Iron Lamellar Shoulders | Plate | T3 | auto | 0 | 16 | 8 | 0 | 1.4 | OSA |
| `AR_sturgia_shoulders_a4` | Northern Brass Lamellar Shoulders With Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 1.4 | OSA |
| `AR_sturgia_shoulders_a5` | Northern Iron Lamellar Shoulders With Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 1.4 | OSA |
| `lamellar_shoulder` | Northern Lamellar Shoulders | Plate | T3 | auto | 0 | 20 | 4 | 0 | 3.5 | OSA |
| `lamellar_shoulder_scarf` | Northern Lamellar Shoulders With Scarf | Plate | T3 | auto | 0 | 20 | 4 | 0 | 3.5 | OSA |
| `AR_sturgia_shoulders_b` | Northern Iron Scale Shoulders | Plate | T3 | auto | 0 | 18 | 4 | 0 | 2.2 | OSA |
| `mp_brass_lamellar_shoulder_white` | Northlander Lamellar Shoulders | Plate | T3 | 1 | 0 | 16 | 6 | 0 | 5 | Vanilla |
| `TV_sturgia_shoulders_a` | Nordic Large Lamellar Shoulders | Plate | T3 | auto | 0 | 16 | 6 | 0 | 3.5 | OSA |
| `TV_sturgia_shoulders_d` | Large Steel Lamellar Shoulders | Plate | T3 | auto | 0 | 16 | 6 | 0 | 3.5 | OSA |
| `TV_sturgia_shoulders_i` | Large Brass Lamellar Shoulders | Plate | T3 | auto | 0 | 16 | 6 | 0 | 3.5 | OSA |
| `TV_sturgia_shoulders_l` | Nordic Lamellar Cape | Plate | T3 | auto | 0 | 16 | 6 | 0 | 5 | OSA |
| `TV_sturgia_shoulders_m` | Steel Lamellar Cape | Plate | T3 | auto | 0 | 16 | 6 | 0 | 5 | OSA |
| `TV_sturgia_shoulders_n` | Brass Lamellar Cape | Plate | T3 | auto | 0 | 16 | 6 | 0 | 5 | OSA |
| `pauldron_cape_z` | Northern Long Cape | Plate | T2 | auto | 0 | 16 | 4 | 0 | 3.5 | OSA |
| `stitched_leather_shoulders` | Stitched Leather Shoulders | Leather | T1 | auto | 0 | 14 | 0 | 0 | 1.5 | RBM |
| `AR_sturgia_shoulders_a` | Northern Leather Scale Shoulders | Leather | T1 | auto | 0 | 8 | 4 | 0 | 1.4 | OSA |
| `AR_sturgia_shoulders_e2` | Northern Chainmail Shoulders | Chainmail | T1 | auto | 0 | 12 | 0 | 0 | 1.4 | OSA |
| `mp_brass_scale_shoulders` | Bronze Scale Shoulders | Plate | T1 | 1 | 0 | 12 | 0 | 0 | 2.2 | Vanilla |
| `TV_sturgia_shoulders_g` | Steel Lamellar Pauldrons | Plate | T1 | auto | 0 | 0 | 12 | 0 | 2.2 | OSA |
| `TV_sturgia_shoulders_h` | Brass Lamellar Pauldrons | Plate | T1 | auto | 0 | 12 | 0 | 0 | 4.1 | OSA |
| `mp_mail_shoulders` | Mail Shoulders | Chainmail | T1 | 1 | 0 | 11 | 0 | 0 | 2.7 | Vanilla |
| `a_brass_lamellar_shoulder_white_a` | Lamellar Shoulder Guards | Plate | T0 | auto | 0 | 10 | 0 | 0 | 1 | RBM |
| `AR_sturgia_shoulders_e` | Northern Leather Shoulders | Leather | T0 | auto | 0 | 8 | 0 | 0 | 1.4 | OSA |
| `DZ_sturgia_shoulder_b` | Northern Brass Lamellar Pauldrons | Plate | T0 | auto | 0 | 8 | 0 | 0 | 4.1 | OSA |
| `DZ_sturgia_shoulder_c` | Northern Iron Lamellar Pauldrons | Plate | T0 | auto | 0 | 8 | 0 | 0 | 4.1 | OSA |
| `tv_sturgia_cloak_a` | Northern Long Cape | Cloth | T0 | auto | 0 | 8 | 0 | 0 | 4 | OSA |
| `TV_sturgia_shoulders_b` | Nordic Lamellar Shoulders | Plate | T0 | auto | 0 | 0 | 8 | 0 | 3 | OSA |
| `TV_sturgia_shoulders_e` | Steel Lamellar Shoulders | Plate | T0 | auto | 0 | 0 | 8 | 0 | 3 | OSA |
| `TV_sturgia_shoulders_j` | Brass Lamellar Shoulders | Plate | T0 | auto | 0 | 0 | 8 | 0 | 3 | OSA |
| `TV_sturgia_shoulders_c` | Nordic Small Lamellar Shoulders | Plate | T0 | auto | 0 | 0 | 5 | 0 | 1.6 | OSA |
| `TV_sturgia_shoulders_f` | Small Steel Lamellar Shoulders | Plate | T0 | auto | 0 | 0 | 5 | 0 | 1.6 | OSA |
| `TV_sturgia_shoulders_k` | Small Brass Lamellar Shoulders | Plate | T0 | auto | 0 | 0 | 5 | 0 | 1.6 | OSA |

<!-- CULTURE:battania COUNT:95 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `battania_warlord_pauldrons` | Heavy Warlord Pauldrons | Plate | T5 | auto | 0 | 45 | 0 | 0 | 4 | RBM |
| `AR_wolf_shoulder_h` | Highland Warlord Pauldrons With Wolf Pelt | Plate | T5 | auto | 0 | 21 | 12 | 0 | 4 | OSA |
| `AR_wolf_shoulder_h2` | Highland Warlord Pauldrons With Brown Wolf Pelt | Plate | T5 | auto | 0 | 21 | 12 | 0 | 4 | OSA |
| `AR_wolf_shoulder_i` | Highland Bronze Warlord Pauldrons With Wolf Pelt | Plate | T5 | auto | 0 | 21 | 12 | 0 | 4 | OSA |
| `AR_wolf_shoulder_i2` | Highland Bronze Warlord Pauldrons With Brown Wolf Pelt | Plate | T5 | auto | 0 | 21 | 12 | 0 | 4 | OSA |
| `battanian_chainmail_shoulder_b` | Mail Shoulder Reinforcements | Chainmail | T4 | auto | 0 | 32 | 0 | 0 | 4.5 | RBM |
| `TV_battania_warlord_pauldrons_b` | Highland Brass Warlord Pauldrons | Plate | T4 | auto | 0 | 20 | 10 | 0 | 4 | OSA |
| `TV_battania_warlord_pauldrons_c` | Highland Steel Warlord Pauldrons | Plate | T4 | auto | 0 | 20 | 10 | 0 | 4 | OSA |
| `battanian_chainmail_shoulder_a` | Mail Shoulder Pieces | Chainmail | T3 | auto | 0 | 27 | 0 | 0 | 4.5 | RBM |
| `bearskin` | Bear Pelt | Leather | T3 | auto | 0 | 26 | 0 | 0 | 2.3 | RBM |
| `mp_battania_civil_cloak` | Highland Cloak | Cloth | T3 | 1 | 0 | 13 | 13 | 0 | 4 | Vanilla |
| `rough_bearskin` | Rough Bearskin | Leather | T3 | auto | 0 | 26 | 0 | 0 | 2.3 | RBM |
| `fur_cloak_c` | Large Fur Cloak With Round Plate | Leather | T3 | auto | 0 | 25 | 0 | 0 | 8.3 | RBM |
| `armored_bearskin` | Armored Bearskin | Leather | T3 | auto | 0 | 24 | 0 | 0 | 7 | RBM |
| `tv_battania_cloak_c` | Highland Scale Shoulders With Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.9 | OSA |
| `tv_battania_cloak_d` | Highland Scale Shoulders With Plaid Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.9 | OSA |
| `tv_battania_cloak_e` | Highland Scale Shoulders With Striped Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.9 | OSA |
| `tv_battania_cloak_r` | Highland Alternating Scale Shoulders With Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.9 | OSA |
| `tv_battania_cloak_r2` | Highland Alternating Scale Shoulders With Plaid Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.9 | OSA |
| `tv_battania_cloak_r3` | Highland Alternating Scale Shoulders With Striped Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.9 | OSA |
| `tv_battania_cloak_s` | Highland Steel Scale Shoulders With Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.9 | OSA |
| `tv_battania_cloak_s2` | Highland Steel Scale Shoulders With Plaid Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.9 | OSA |
| `tv_battania_cloak_s3` | Highland Steel Scale Shoulders With Striped Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.9 | OSA |
| `battania_cloak_furr` | Chained Fur Cloak | Leather | T3 | auto | 0 | 22 | 0 | 0 | 1.5 | RBM |
| `wolf_shoulder` | Wolf Pelt Cape | Leather | T3 | auto | 0 | 22 | 0 | 0 | 1.8 | RBM |
| `fur_cloak_a` | Fur Cloak With Round Plate | Leather | T2 | auto | 0 | 20 | 0 | 0 | 7.6 | RBM |
| `AR_wolf_head_cloak_a` | Wolfskin Cloak | Leather | T2 | auto | 6 | 8 | 2 | 0 | 3.6 | OSA |
| `AR_wolf_head_cloak_b` | Brown Wolfskin Cloak | Leather | T2 | auto | 6 | 8 | 2 | 0 | 3.6 | OSA |
| `TV_wolf_pelt_h` | Eastern Wolfskin Cloak | Leather | T2 | auto | 6 | 8 | 2 | 0 | 3.6 | OSA |
| `AR_battanian_shoulder_a` | Highland Scale Shoulders | Plate | T1 | auto | 0 | 16 | 0 | 0 | 4.5 | OSA |
| `battania_shoulder_strap_cloak` | Tartan Shoulder Harness | Leather | T1 | auto | 0 | 16 | 0 | 0 | 1.3 | RBM |
| `battania_shoulder_furr` | Reinforced Harness with Cape | Leather | T1 | auto | 0 | 15 | 0 | 0 | 1.6 | RBM |
| `battania_woodland_cloak` | Woodland Cape | Leather | T1 | auto | 0 | 15 | 0 | 0 | 1.4 | RBM |
| `battania_cloak_b` | Tartan Cape | Cloth | T1 | auto | 0 | 14 | 0 | 0 | 3.2 | RBM |
| `battania_shoulder_strap` | Shoulder Harness | Leather | T1 | auto | 0 | 14 | 0 | 0 | 1 | RBM |
| `TV_battania_shoulders_d` | Brown Bear Shoulders | Leather | T1 | auto | 0 | 14 | 0 | 0 | 2.3 | OSA |
| `TV_wolf_pelt_f` | Eastern Heavy Wolf Shoulders | Leather | T1 | auto | 0 | 10 | 4 | 0 | 2.6 | OSA |
| `battania_scarf_a` | Heavy Tartan Scarf | Cloth | T1 | auto | 0 | 13 | 0 | 0 | 0.55 | RBM |
| `AR_battanian_shoulder_h` | Highland Heavy Shoulder Straps With Tartan Cape | Leather | T1 | auto | 0 | 12 | 0 | 0 | 1 | OSA |
| `battanian_leather_shoulder_a` | Cured Leather Shoulder Pieces | Leather | T1 | auto | 0 | 12 | 0 | 0 | 1.2 | RBM |
| `hmj_battanian_shoulders_e` | Highland Mail Shoulders With Fur | Chainmail | T1 | auto | 0 | 12 | 0 | 0 | 4.5 | OSA |
| `AR_battanian_shoulder_g` | Highland Shoulder Straps With Tartan Cape | Leather | T0 | auto | 0 | 10 | 0 | 0 | 1 | OSA |
| `battanian_mail_scarf` | Highland Mail Shoulders With Cloak | Chainmail | T0 | auto | 0 | 10 | 0 | 0 | 4.5 | OSA |
| `hmj_battanian_shoulders_c` | Highland Mail Shoulders With Bearskin | Chainmail | T0 | auto | 0 | 10 | 0 | 0 | 4.5 | OSA |
| `hmj_battanian_shoulders_d` | Highland Mail Shoulders With Rough Bearskin | Chainmail | T0 | auto | 0 | 10 | 0 | 0 | 4.5 | OSA |
| `tv_battania_cloak_h` | Highland Chainmail Shoulders With Cape | Plate | T0 | auto | 0 | 8 | 2 | 0 | 3.9 | OSA |
| `tv_battania_cloak_i` | Highland Chainmail Shoulders With Plaid Cape | Plate | T0 | auto | 0 | 8 | 2 | 0 | 3.9 | OSA |
| `tv_battania_cloak_j` | Highland Chainmail Shoulders With Striped Cape | Plate | T0 | auto | 0 | 8 | 2 | 0 | 3.9 | OSA |
| `hmj_battanian_shoulders_a` | Highland Mail Shoulders With Furred Cloak | Chainmail | T0 | auto | 0 | 9 | 0 | 0 | 4.5 | OSA |
| `AR_battanian_shoulder_b` | Highland Leather Shoulders | Leather | T0 | auto | 0 | 8 | 0 | 0 | 1.4 | OSA |
| `AR_battanian_shoulder_c` | Highland Cured Leather Shoulders With Striped Cape | Leather | T0 | auto | 0 | 8 | 0 | 0 | 1.2 | OSA |
| `AR_battanian_shoulder_d` | Highland Cured Leather Shoulders With Cape | Leather | T0 | auto | 0 | 8 | 0 | 0 | 1.2 | OSA |
| `AR_fur_cape_a` | Furred Long Cape | Cloth | T0 | auto | 0 | 8 | 0 | 0 | 4 | OSA |
| `AR_fur_cape_b` | Furred Long Plaid Cape | Cloth | T0 | auto | 0 | 8 | 0 | 0 | 4 | OSA |
| `AR_fur_cape_c` | Furred Long Striped Cape | Cloth | T0 | auto | 0 | 8 | 0 | 0 | 4 | OSA |
| `AR_fur_cape_d` | Furred Hide Cape | Cloth | T0 | auto | 0 | 8 | 0 | 0 | 4 | OSA |
| `HMJ_battanian_leather_scarf` | Highland Leather Shoulders With Cloak | Leather | T0 | auto | 0 | 8 | 0 | 0 | 1.4 | OSA |
| `tv_battania_cloak_a` | Long Cape | Cloth | T0 | auto | 0 | 8 | 0 | 0 | 4 | OSA |
| `tv_battania_cloak_b` | Long Plaid Cape | Cloth | T0 | auto | 0 | 8 | 0 | 0 | 4 | OSA |
| `tv_battania_cloak_f` | Long Striped Cape | Cloth | T0 | auto | 0 | 8 | 0 | 0 | 4 | OSA |
| `tv_battania_cloak_g` | Long Hide Cape | Cloth | T0 | auto | 0 | 8 | 0 | 0 | 4 | OSA |
| `TV_wolf_pelt_d` | Brown Wolf Shoulders | Leather | T0 | auto | 0 | 6 | 2 | 0 | 1.8 | OSA |
| `TV_wolf_pelt_g` | Eastern Wolf Shoulders | Leather | T0 | auto | 0 | 6 | 2 | 0 | 1.8 | OSA |
| `ao_battanian_mail_shoulders` | Highland Trimmed Mail Shoulders | Cloth | T0 | auto | 0 | 7 | 0 | 0 | 4.5 | OSA |
| `AR_empire_shoulders_a` | Highland Cured Leather Shoulders With Scarf | Cloth | T0 | auto | 0 | 7 | 0 | 0 | 1.2 | OSA |
| `battania_civil_cape` | Woodland Fur Cloak | Cloth | T0 | auto | 0 | 7 | 0 | 0 | 1.5 | RBM |
| `fur_cloak_b` | Fur Cloak | Leather | T0 | auto | 0 | 7 | 0 | 0 | 2.7 | RBM |
| `mp_battanian_chainmail_shoulder_b` | Mail Shoulder Reinforcements | Cloth | T0 | 1 | 0 | 7 | 0 | 0 | 4.5 | Vanilla |
| `mp_rough_bearskin` | Rough Bearskin | Leather | T0 | 1 | 0 | 7 | 0 | 0 | 2.3 | Vanilla |
| `TV_wolf_pelt_a` | Brown Wolf Pelt | Leather | T0 | auto | 0 | 7 | 0 | 0 | 2 | OSA |
| `TV_wolf_pelt_b` | Wolf Pelt | Leather | T0 | auto | 0 | 7 | 0 | 0 | 2 | OSA |
| `a_battania_cloak_a` | Simple Cape | Cloth | T0 | auto | 0 | 6 | 0 | 0 | 0.5 | RBM |
| `AR_battanian_shoulder_f` | Highland Simple Shoulder Straps With Tartan Cape | Leather | T0 | auto | 0 | 6 | 0 | 0 | 1 | OSA |
| `battania_civil_cloak` | Fringed Cape | Cloth | T0 | auto | 0 | 6 | 0 | 0 | 1 | RBM |
| `battania_cloak` | Tartan Cloak | Cloth | T0 | auto | 0 | 6 | 0 | 0 | 0.5 | RBM |
| `mp_battanian_chainmail_shoulder_a` | Mail Shoulder Pieces | Cloth | T0 | 1 | 0 | 6 | 0 | 0 | 4.5 | Vanilla |
| `TV_battania_cloak_p` | Long Cloak | Cloth | T0 | auto | 0 | 6 | 0 | 0 | 4.5 | OSA |
| `TV_battania_cloak_p2` | Long Plaid Cloak | Cloth | T0 | auto | 0 | 6 | 0 | 0 | 4.5 | OSA |
| `TV_battania_cloak_p3` | Long Striped Cloak | Cloth | T0 | auto | 0 | 6 | 0 | 0 | 4.5 | OSA |
| `TV_battania_cloak_q` | Plain Woolen Cape | Cloth | T0 | auto | 0 | 6 | 0 | 0 | 4.5 | OSA |
| `TV_battania_cloak_q2` | Plaid Woolen Cape | Cloth | T0 | auto | 0 | 6 | 0 | 0 | 4.5 | OSA |
| `TV_battania_cloak_q3` | Striped Woolen Cape | Cloth | T0 | auto | 0 | 6 | 0 | 0 | 4.5 | OSA |
| `ao_light_fur_shoulder` | Highland Shoulder Cloak | Cloth | T-1 | auto | 0 | 4 | 0 | 0 | 0.8 | OSA |
| `AR_battanian_shoulder_e` | Highland Simple Shoulder Straps | Leather | T-1 | auto | 0 | 4 | 0 | 0 | 1 | OSA |
| `mp_battania_cloak_b` | Tartan Cape | Cloth | T-1 | 1 | 0 | 4 | 0 | 0 | 0.8 | Vanilla |
| `mp_battania_scarf_a` | Heavy Tartan Cape | Cloth | T-1 | 1 | 0 | 3 | 1 | 0 | 0.55 | Vanilla |
| `mp_battania_shoulder_strap` | Shoulder Harness | Leather | T-1 | 1 | 0 | 4 | 0 | 0 | 1 | Vanilla |
| `TV_battania_cloak_l` | Highland Plaid Cloak | Cloth | T-1 | auto | 0 | 2 | 2 | 0 | 0.25 | OSA |
| `TV_battania_cloak_l2` | Highland Brown Plaid Cloak | Cloth | T-1 | auto | 0 | 2 | 2 | 0 | 0.25 | OSA |
| `TV_battania_cloak_l3` | Highland Green Cloak | Cloth | T-1 | auto | 0 | 2 | 2 | 0 | 0.25 | OSA |
| `ao_generic_short_cape` | Plain Short Cape | Cloth | T-1 | auto | 0 | 2 | 0 | 0 | 0.2 | OSA |
| `hmj_battanian_shoulders_b` | Highland Shoulder Cape | Cloth | T-1 | auto | 0 | 2 | 0 | 0 | 0.2 | OSA |
| `mp_a_battania_cloak_a` | Simple Cape | Cloth | T-1 | 1 | 0 | 1 | 1 | 0 | 0.5 | Vanilla |
| `ao_battanian_medallion` | Highland Medallion | Cloth | T-1 | auto | 0 | 1 | 0 | 0 | 0.2 | OSA |
| `ao_caladogs_medallion` | Highland Noble Medallion | Cloth | T-1 | auto | 0 | 1 | 0 | 0 | 0.2 | OSA |

<!-- CULTURE:khuzait COUNT:52 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `lamellar_shoulders` | Lamellar Shoulder Pieces | Plate | T5 | auto | 0 | 40 | 0 | 0 | 5 | RBM |
| `brass_lamellar_shoulder` | Bronze Lamellar Shoulder Pieces | Plate | T4 | auto | 0 | 32 | 0 | 0 | 5 | RBM |
| `eastern_studded_shoulders` | Reinforced Studded Shoulder Guards | Leather | T3 | auto | 0 | 27 | 0 | 0 | 2.3 | RBM |
| `TV_khuzait_shoulder_h` | Eastern Lamellar Shoulders Over Leather | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.8 | OSA |
| `TV_khuzait_shoulder_i` | Eastern Lamellar Shoulders With Cape | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.8 | OSA |
| `TV_khuzait_shoulder_o` | Eastern Decorated Steel Lamellar Shoulder Pieces Over Leather | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.8 | OSA |
| `TV_khuzait_shoulder_p` | Eastern Decorated Steel Lamellar Shoulder Pieces With Cape | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.8 | OSA |
| `AO_aserai_shoulders_d` | Brass Scale Shoulders With Cloak | Plate | T3 | auto | 0 | 18 | 8 | 0 | 4.2 | OSA |
| `AO_aserai_shoulders_d2` | Iron Scale Shoulders With Cloak | Plate | T3 | auto | 0 | 18 | 8 | 0 | 4.2 | OSA |
| `TV_khuzait_shoulder_g` | Eastern Lamellar Pauldrons | Plate | T3 | auto | 0 | 17 | 9 | 0 | 3.5 | OSA |
| `TV_khuzait_shoulder_m` | Eastern Steel Lamellar Shoulder Pieces Over Leather | Plate | T3 | auto | 0 | 17 | 9 | 0 | 3.8 | OSA |
| `TV_khuzait_shoulder_n` | Eastern Steel Lamellar Shoulder Pieces With Cape | Plate | T3 | auto | 0 | 17 | 9 | 0 | 3.8 | OSA |
| `a_metal_lamellar_armor_shoulder_a` | Decorated Steel Lamellar Shoulder Pieces | Plate | T3 | auto | 0 | 25 | 0 | 0 | 5 | RBM |
| `AR_aserai_shoulder_w` | Southern Heavy Lamellar Shoulders With Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 4.5 | OSA |
| `AR_aserai_shoulder_w2` | Eastern Heavy Lamellar Shoulders With Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 4.5 | OSA |
| `eastern_heavy_lamellar_shoulders` | Eastern Heavy Lamellar Shoulders | Plate | T3 | auto | 0 | 16 | 8 | 0 | 4.5 | OSA |
| `leather_lamellar_shoulders` | Leather Lamellar Shoulder Guards | Leather | T3 | auto | 0 | 24 | 0 | 0 | 2.3 | RBM |
| `AR_khuzait_cape_a` | Eastern Lamellar With Cape | Plate | T3 | auto | 0 | 16 | 6 | 0 | 3.9 | OSA |
| `AR_khuzait_cape_b` | Eastern Steel Lamellar With Woolen Cape | Plate | T3 | auto | 0 | 16 | 6 | 0 | 3.9 | OSA |
| `AR_khuzait_cape_c` | Eastern Steel Lamellar With Linen Cape | Plate | T3 | auto | 0 | 16 | 6 | 0 | 3.9 | OSA |
| `brass_lamellar_cape` | Brass Lamellar Cape | Plate | T3 | auto | 0 | 16 | 6 | 0 | 5 | OSA |
| `brass_lamellar_cape_z` | Northern Lamellar Cape | Plate | T3 | auto | 0 | 16 | 6 | 0 | 5 | OSA |
| `khuzait_leather_pauldron` | Reinforced Leather Pauldrons | Leather | T3 | auto | 0 | 22 | 0 | 0 | 1.5 | RBM |
| `TV_khuzait_shoulder_f` | Eastern Heavy Steel Lamellar Shoulders | Plate | T3 | auto | 0 | 18 | 4 | 0 | 5 | OSA |
| `steppe_leather_shoulders` | Cured Leather Pauldrons | Leather | T2 | auto | 0 | 21 | 0 | 0 | 0.7 | RBM |
| `TV_khuzait_shoulder_s` | Eastern Blackened Leather Lamellar Shoulder Pieces Over Leather | Plate | T2 | auto | 0 | 16 | 5 | 0 | 3.8 | OSA |
| `TV_khuzait_shoulder_t` | Eastern Blackened Leather Lamellar Shoulder Pieces With Cape | Plate | T2 | auto | 0 | 16 | 5 | 0 | 3.8 | OSA |
| `a_brass_lamellar_shoulder_b` | Reinforced Bronze Lamellar Shoulder Guards | Plate | T2 | auto | 0 | 20 | 0 | 0 | 5 | RBM |
| `TV_khuzait_shoulder_r` | Eastern Heavy Blackened Leather Lamellar Shoulders | Leather | T2 | auto | 0 | 16 | 4 | 0 | 5 | OSA |
| `TV_khuzait_shoulder_k` | Eastern Leather Lamellar Shoulders | Leather | T2 | auto | 0 | 15 | 4 | 0 | 2.3 | OSA |
| `TV_khuzait_shoulder_l` | Eastern Leather Lamellar Shoulders With Cape | Leather | T2 | auto | 0 | 15 | 4 | 0 | 2.3 | OSA |
| `a_leather_lamellar_armor_shoulder_a` | Decorated Lamellar Leather Shoulder Guards | Leather | T2 | auto | 0 | 18 | 0 | 0 | 2.3 | RBM |
| `AR_khuzait_cape_f` | Eastern Leather Lamellar With Hide Cape | Leather | T2 | auto | 0 | 14 | 3 | 0 | 3.9 | OSA |
| `AR_khuzait_cape_g` | Eastern Leather Lamellar With Woolen Cape | Leather | T2 | auto | 0 | 14 | 3 | 0 | 3.9 | OSA |
| `AR_khuzait_cape_h` | Eastern Leather Lamellar With Linen Cape | Leather | T2 | auto | 0 | 14 | 3 | 0 | 3.9 | OSA |
| `TV_khuzait_shoulder_a` | Eastern Plated Bronze Shoulder Straps | Plate | T1 | auto | 0 | 16 | 0 | 0 | 3.5 | OSA |
| `TV_khuzait_shoulder_b` | Eastern Plated Brass Shoulder Straps | Plate | T1 | auto | 0 | 16 | 0 | 0 | 3.5 | OSA |
| `a_khuzait_leather_shoulder_b_a` | Cured Studded Leather Shoulders | Leather | T1 | auto | 0 | 15 | 0 | 0 | 2.3 | RBM |
| `TV_khuzait_shoulder_j` | Eastern Leather Lamellar Pauldrons | Leather | T1 | auto | 0 | 10 | 5 | 0 | 2.5 | OSA |
| `a_khuzait_leather_shoulder_b_b` | Reinforced Studded Neckguard | Leather | T1 | auto | 0 | 14 | 0 | 0 | 2.3 | RBM |
| `bolted_leather_strips` | Bolted Leather Strips | Leather | T1 | auto | 0 | 14 | 0 | 0 | 0.5 | RBM |
| `reinforced_suede_shoulders` | Reinforced Suede Shoulder Guards | Leather | T1 | auto | 0 | 13 | 0 | 0 | 0.9 | RBM |
| `TV_khuzait_shoulder_d` | Eastern Plated Leather Pauldrons | Leather | T1 | auto | 0 | 8 | 3 | 0 | 1.5 | OSA |
| `TV_khuzait_shoulder_c` | Eastern Leather Pauldrons | Leather | T0 | auto | 0 | 8 | 2 | 0 | 1.5 | OSA |
| `TV_khuzait_shoulder_e` | Eastern Steel Lamellar Shoulder Pieces | Plate | T0 | auto | 0 | 0 | 10 | 0 | 2.8 | OSA |
| `TV_khuzait_shoulder_q` | Eastern Blackened Leather Lamellar Shoulder Pieces | Leather | T0 | auto | 0 | 0 | 10 | 0 | 2.8 | OSA |
| `a_brass_lamellar_shoulder_a` | Bronze Lamellar Shoulder Guards | Plate | T0 | auto | 0 | 8 | 0 | 0 | 1 | RBM |
| `AR_khuzait_cape_d` | Eastern Woolen Cape | Cloth | T0 | auto | 0 | 5 | 3 | 0 | 1 | OSA |
| `AR_khuzait_cape_e` | Eastern Linen Cape | Cloth | T0 | auto | 0 | 5 | 3 | 0 | 1 | OSA |
| `DZ_khuzait_shoulder_a` | Eastern Heavy Leather Pauldrons | Leather | T0 | auto | 0 | 8 | 0 | 0 | 4.1 | OSA |
| `DZ_sturgia_shoulder_a` | Eastern Lamellar Pauldrons | Plate | T0 | auto | 0 | 8 | 0 | 0 | 4.1 | OSA |
| `ao_durkhan_tassled_necklace` | Eastern Tassled Necklace | Cloth | T-1 | auto | 0 | 1 | 0 | 0 | 0.2 | OSA |

<!-- CULTURE:aserai COUNT:62 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `desert_scale_shoulders` | Reinforced Scale Pauldrons | Plate | T5 | auto | 0 | 45 | 0 | 0 | 4.4 | RBM |
| `aserai_scale_shoulder_b` | Long Sleeved Bronze Scale Shoulder Guards | Plate | T5 | auto | 0 | 43 | 0 | 0 | 2.2 | RBM |
| `aserai_scale_shoulder_e` | Decorated Long Sleeve Bronze Scale Pauldrons | Plate | T5 | auto | 0 | 39 | 0 | 0 | 2.8 | RBM |
| `aserai_scale_shoulder_a` | Bronze Scale Shoulder Guards | Plate | T5 | auto | 0 | 37 | 0 | 0 | 1.8 | RBM |
| `a_aserai_scale_b_shoulder_e` | Reinforced Bronze Scale Shoulder Guards | Plate | T5 | auto | 0 | 36 | 0 | 0 | 4.8 | RBM |
| `a_aserai_scale_b_shoulder_a` | Scale Shoulder Guards | Plate | T4 | auto | 0 | 30 | 0 | 0 | 4 | RBM |
| `AR_aserai_lamellar_cape_a` | Southern Gilded Lamellar With Long Cape | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.9 | OSA |
| `AR_aserai_lamellar_cape_b` | Southern Gilded Lamellar With Cape | Plate | T3 | auto | 0 | 18 | 9 | 0 | 3.9 | OSA |
| `aserai_scale_shoulder_c` | Bronze Scale Pauldrons | Plate | T3 | auto | 0 | 27 | 0 | 0 | 1.9 | RBM |
| `AO_aserai_shoulders_a` | Brass Scale Shoulders | Plate | T3 | auto | 0 | 18 | 8 | 0 | 4.2 | OSA |
| `AO_aserai_shoulders_a2` | Iron Scale Shoulders | Plate | T3 | auto | 0 | 18 | 8 | 0 | 4.2 | OSA |
| `mp_desert_scale_shoulders` | Reinforced Scale Pauldrons | Plate | T3 | 1 | 0 | 18 | 8 | 0 | 4.2 | Vanilla |
| `a_aserai_scale_b_shoulder_c` | Bronze Scale Shoulder Guards | Plate | T3 | auto | 0 | 24 | 0 | 0 | 4.4 | RBM |
| `AR_aserai_scale_cape_a` | Desert Scale Shoulders With Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.9 | OSA |
| `AR_aserai_scale_cape_a2` | Desert Brass Scale Shoulders With Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 3.9 | OSA |
| `AR_aserai_scale_cape_b` | Long Sleeved Brass Scale Shoulder Guards With Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 2.2 | OSA |
| `AR_aserai_scale_cape_b2` | Long Sleeved Iron Scale Shoulder Guards With Cape | Plate | T3 | auto | 0 | 16 | 8 | 0 | 2.2 | OSA |
| `AR_aserai_scale_shoulder_b` | Long Sleeved Iron Scale Shoulder Guards | Plate | T3 | auto | 0 | 16 | 8 | 0 | 2.2 | OSA |
| `AR_aserai_shoulder_a` | Desert Scale Shoulders With Leopard Pelt | Plate | T3 | auto | 0 | 16 | 8 | 0 | 4.3 | OSA |
| `AR_aserai_shoulder_a2` | Desert Brass Scale Shoulders With Leopard Pelt | Plate | T3 | auto | 0 | 16 | 8 | 0 | 4.3 | OSA |
| `AR_aserai_shoulder_o` | Desert Scale Shoulders With Zebra Skin | Plate | T3 | auto | 0 | 16 | 8 | 0 | 4.3 | OSA |
| `AR_aserai_shoulder_o2` | Desert Brass Scale Shoulders With Zebra Skin | Plate | T3 | auto | 0 | 16 | 8 | 0 | 4.3 | OSA |
| `AR_aserai_shoulder_t` | Desert Scale Shoulders With Lion Pelt | Plate | T3 | auto | 0 | 16 | 8 | 0 | 4.3 | OSA |
| `AR_aserai_shoulder_t2` | Desert Brass Scale Shoulders With Lion Pelt | Plate | T3 | auto | 0 | 16 | 8 | 0 | 4.3 | OSA |
| `AR_aserai_shoulder_y` | Southern Mail Shoulders With Leopard Pelt | Chainmail | T2 | auto | 0 | 14 | 7 | 0 | 4 | OSA |
| `AR_aserai_shoulder_y2` | Southern Mail Shoulders With Zebra Skin | Chainmail | T2 | auto | 0 | 14 | 7 | 0 | 4 | OSA |
| `AR_aserai_shoulder_y3` | Southern Mail Shoulders With Lion Pelt | Chainmail | T2 | auto | 0 | 14 | 7 | 0 | 4 | OSA |
| `AR_aserai_scale_shoulder_a` | Iron Scale Shoulder Guards | Plate | T2 | auto | 0 | 16 | 4 | 0 | 1.8 | OSA |
| `AR_aserai_scale_shoulder_c` | Iron Scale Pauldrons | Plate | T2 | auto | 0 | 8 | 10 | 0 | 1.9 | OSA |
| `AR_aserai_shoulder_e` | Southern Reinforced Leather Shoulders | Plate | T2 | auto | 0 | 10 | 8 | 0 | 4.2 | OSA |
| `AR_aserai_shoulder_u` | Southern Reinforced Leather Shoulders With Long Cape | Plate | T2 | auto | 0 | 10 | 8 | 0 | 4.2 | OSA |
| `AR_aserai_shoulder_u2` | Desert Reinforced Leather Shoulders With Cape | Plate | T2 | auto | 0 | 10 | 8 | 0 | 4.2 | OSA |
| `tv_aserai_cloak_b` | Southern Plated Brass Shoulder Straps With Cape | Plate | T2 | auto | 0 | 17 | 0 | 0 | 3.5 | OSA |
| `tv_aserai_cloak_c` | Southern Plated Brass Shoulder Straps With Tassels | Plate | T2 | auto | 0 | 17 | 0 | 0 | 3.5 | OSA |
| `AR_aserai_scale_cape_c` | Decorated Long Sleeve Brass Scale Pauldrons With Cape | Plate | T1 | auto | 0 | 6 | 10 | 0 | 2.8 | OSA |
| `AR_aserai_scale_cape_c2` | Decorated Long Sleeve Iron Scale Pauldrons With Cape | Plate | T1 | auto | 0 | 6 | 10 | 0 | 2.8 | OSA |
| `AR_aserai_scale_shoulder_e` | Decorated Long Sleeve Iron Scale Pauldrons | Plate | T1 | auto | 0 | 6 | 10 | 0 | 2.8 | OSA |
| `AR_aserai_shoulder_l` | Studded Brass Neckguard With Zebra Skin | Plate | T1 | auto | 0 | 16 | 0 | 0 | 3.8 | OSA |
| `AR_aserai_shoulder_m` | Studded Iron Neckguard With Zebra Skin | Plate | T1 | auto | 0 | 16 | 0 | 0 | 3.8 | OSA |
| `AR_aserai_shoulder_r` | Studded Brass Neckguard With Lion Pelt | Plate | T1 | auto | 0 | 16 | 0 | 0 | 3.8 | OSA |
| `AR_aserai_shoulder_s` | Studded Iron Neckguard With Lion Pelt | Plate | T1 | auto | 0 | 16 | 0 | 0 | 3.8 | OSA |
| `aserai_scale_shoulder_d` | Decorated Bronze Scale Pauldrons | Plate | T1 | auto | 0 | 16 | 0 | 0 | 2.4 | RBM |
| `a_aserai_scale_b_shoulder_b` | Scale Pauldrons | Plate | T1 | auto | 0 | 15 | 0 | 0 | 2 | RBM |
| `aserai_horseman_shoulder` | Ringmail Shoulder Guard | Leather | T1 | auto | 0 | 15 | 0 | 0 | 2.5 | RBM |
| `AR_aserai_scale_shoulder_d` | Decorated Iron Scale Pauldrons | Plate | T1 | auto | 0 | 6 | 8 | 0 | 2.4 | OSA |
| `leopard_pelt` | Leopard Pelt | Leather | T1 | auto | 0 | 13 | 0 | 0 | 2 | RBM |
| `a_aserai_scale_b_shoulder_d` | Bronze Scale Pauldrons | Plate | T1 | auto | 0 | 12 | 0 | 0 | 2.2 | RBM |
| `AR_aserai_shoulder_f` | Southern Leather Shoulders | Plate | T1 | auto | 0 | 5 | 7 | 0 | 4 | OSA |
| `AR_aserai_shoulder_g` | Southern Leather Pauldrons | Plate | T0 | auto | 0 | 2 | 8 | 0 | 2 | OSA |
| `AR_aserai_shoulder_v` | Southern Patterned Cloth Pauldrons | Plate | T0 | auto | 0 | 2 | 8 | 0 | 2 | OSA |
| `AR_aserai_shoulder_v2` | Southern Cloth Pauldrons | Plate | T0 | auto | 0 | 2 | 8 | 0 | 2 | OSA |
| `mp_a_aserai_scale_b_shoulder_d` | Brass Scale Pauldrons | Plate | T0 | 1 | 0 | 2 | 7 | 0 | 2.2 | Vanilla |
| `mp_desert_fabric_shoulderpad` | Desert Fabric Shoulderpad | Cloth | T0 | 1 | 0 | 8 | 0 | 0 | 1.8 | Vanilla |
| `tv_aserai_cloak_a` | Southern Long Cape | Cloth | T0 | auto | 0 | 8 | 0 | 0 | 4 | OSA |
| `AR_aserai_shoulder_i` | Southern Nomad Sash With Leopard Pelt | Leather | T0 | auto | 0 | 5 | 0 | 0 | 0.5 | OSA |
| `AR_aserai_shoulder_j` | Southern Nomad Sash With Zebra Skin | Leather | T0 | auto | 0 | 5 | 0 | 0 | 0.5 | OSA |
| `AR_aserai_shoulder_p` | Southern Nomad Sash With Lion Pelt | Leather | T0 | auto | 0 | 5 | 0 | 0 | 0.5 | OSA |
| `AR_aserai_shoulder_x` | Stitched Cloth Shoulders | Leather | T0 | auto | 0 | 5 | 0 | 0 | 1.5 | OSA |
| `AR_aserai_shoulder_h` | Southern Nomad Sash | Cloth | T-1 | auto | 0 | 4 | 0 | 0 | 0.5 | OSA |
| `AR_aserai_shoulder_k` | Zebra Skin | Leather | T-1 | auto | 0 | 4 | 0 | 0 | 2 | OSA |
| `AR_aserai_shoulder_q` | Lion Pelt | Leather | T-1 | auto | 0 | 4 | 0 | 0 | 2 | OSA |
| `AR_aserai_scarf_a` | Southern Cloth Cape | Cloth | T-1 | auto | 0 | 2 | 0 | 0 | 0.5 | OSA |

<!-- CULTURE:nord COUNT:16 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `nord_king_shoulder_armor_b` | Gilded Noble Shoulder | Plate | T5 | auto | 0 | 52 | 0 | 0 | 1 | RBM |
| `nord_king_shoulder_armor` | Northern Majestic Shoulder | Plate | T5 | auto | 0 | 50 | 0 | 0 | 1 | RBM |
| `RBM_master_assassin_shoulder` | Master Assassin Cape | Chainmail | T5 | auto | 0 | 43 | 0 | 0 | 0.85 | RBM |
| `northman_raider_cape` | Northmen Raider Heavy Fur Cape | Leather | T5 | auto | 0 | 42 | 0 | 0 | 2.3 | RBM |
| `fur_cape_b` | Nordic Furry Cape | Leather | T5 | auto | 0 | 40 | 0 | 0 | 1.2 | RBM |
| `improved_assassin_shoulder_q5` | Blackened Cape | Leather | T5 | auto | 0 | 33 | 0 | 0 | 0.85 | RBM |
| `chainmail_shoulder_armor_b` | Chainmail Shoulder Guards | Plate | T3 | auto | 0 | 27 | 0 | 0 | 3 | RBM |
| `berserker_cloak` | Berserker Cloak | Leather | T3 | auto | 0 | 25 | 0 | 0 | 3.8 | RBM |
| `fur_cape_a` | Nordic Furry Mail Cape | Leather | T3 | auto | 0 | 17 | 8 | 0 | 1.5 | RBM |
| `nord_shoulder_a` | Reinforced Lamellar Pauldrons | Plate | T3 | auto | 0 | 25 | 0 | 0 | 2.5 | RBM |
| `assassin_shoulder` | Blackened Cape | Leather | T3 | auto | 0 | 23 | 0 | 0 | 0.85 | RBM |
| `northman_raider_shoulder` | Nordic Armored Shoulder Pads | Plate | T2 | auto | 0 | 21 | 0 | 0 | 1.6 | RBM |
| `nord_shoulder_b` | Northern Lamellar Pauldrons | Plate | T2 | auto | 0 | 20 | 0 | 0 | 2.7 | RBM |
| `nord_fur_shoulder` | Long Linen Fur Cape | Leather | T1 | auto | 0 | 15 | 0 | 0 | 3 | RBM |
| `cape_norse_lady` | Northern Fur Cape | Cloth | T1 | auto | 0 | 13 | 0 | 0 | 1 | RBM |
| `nord_casual_cloak` | Long Woolen Cloak | Cloth | T0 | auto | 0 | 8 | 0 | 0 | 0.5 | RBM |

<!-- CULTURE:neutral_culture COUNT:46 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `bandit_hybrid_armor_shoulder_b` | Raider Reinforced Shoulderguard | Plate | T5 | auto | 0 | 45 | 0 | 0 | 1.8 | RBM |
| `tacky_bandit_shoulder` | Gilded Raider Shoulder | Plate | T5 | auto | 0 | 40 | 0 | 0 | 3 | RBM |
| `tacky_bandit_shoulder_b` | Plumed Serpenthide Shoulder | Leather | T4 | auto | 0 | 28 | 0 | 0 | 2 | RBM |
| `mp_battania_warlord_pauldrons` | Highland Warlord Pauldrons | Plate | T3 | 1 | 0 | 13 | 13 | 0 | 4 | Vanilla |
| `mp_armored_bearskin` | Armored Bearskin | Cloth | T1 | 1 | 0 | 15 | 0 | 0 | 7 | Vanilla |
| `mp_battania_civil_cape` | Highland Cape | Cloth | T1 | 1 | 0 | 15 | 0 | 0 | 4 | Vanilla |
| `mp_lamellar_shoulders` | Lamellar Shoulders | Plate | T1 | 1 | 0 | 15 | 0 | 0 | 5 | Vanilla |
| `mp_brass_lamellar_shoulder` | Bronze Lamellar Shoulders | Plate | T1 | 1 | 0 | 14 | 0 | 0 | 5 | Vanilla |
| `mp_imperial_studded_strip_shoulders` | Studded Strip Shoulders | Plate | T1 | 1 | 0 | 14 | 0 | 0 | 4.1 | Vanilla |
| `mp_leopard_pelt` | Leopard Pelt | Leather | T1 | 1 | 0 | 9 | 5 | 0 | 2 | Vanilla |
| `desert_leather_shoulderpad` | Leather Shoulder Guard | Leather | T1 | auto | 0 | 13 | 0 | 0 | 1.8 | RBM |
| `mp_imperial_lamellar_shoulders` | Lamellar Shoulders | Plate | T1 | 1 | 0 | 13 | 0 | 0 | 3.5 | Vanilla |
| `mp_studded_imperial_neckguard` | Studded Neckguard | Plate | T1 | 1 | 0 | 13 | 0 | 0 | 3.6 | Vanilla |
| `empire_cape_a` | Heavy Cloak | Cloth | T1 | auto | 0 | 12 | 0 | 0 | 0.5 | RBM |
| `mp_pauldron_cape_a` | Empire Pauldron Cape | Plate | T1 | 1 | 0 | 11 | 0 | 0 | 5.5 | Vanilla |
| `mp_pauldron_cape_b` | Vlandian Pauldron Cape | Plate | T1 | 1 | 0 | 11 | 0 | 0 | 5.5 | Vanilla |
| `bandit_hybrid_armor_shoulder_a` | Raider Fur Shoulderguard | Leather | T0 | auto | 0 | 10 | 0 | 0 | 0.85 | RBM |
| `mp_bearskin` | Bearskin | Leather | T0 | 1 | 0 | 6 | 4 | 0 | 2.3 | Vanilla |
| `desert_fabric_shoulderpad` | Woven Shoulder Pad | Cloth | T0 | auto | 0 | 9 | 0 | 0 | 1.8 | RBM |
| `female_scarf` | Thick Scarf | Cloth | T0 | auto | 0 | 8 | 0 | 0 | 0.6 | RBM |
| `mp_leather_lamellar_shoulders` | Leather Lamellar Shoulders | Leather | T0 | 1 | 0 | 8 | 0 | 0 | 8 | Vanilla |
| `mp_stitched_leather_shoulders` | Stitched Leather Shoulders | Leather | T0 | 1 | 0 | 8 | 0 | 0 | 3 | Vanilla |
| `mp_wolf_shoulder` | Wolf Shoulder | Leather | T0 | 1 | 0 | 8 | 0 | 0 | 1.8 | Vanilla |
| `mp_aserai_horseman_shoulder` | Desert Ringmail Shoulder | Leather | T0 | 1 | 0 | 7 | 0 | 0 | 2.5 | Vanilla |
| `female_hood` | Cotton Hood | Cloth | T0 | auto | 0 | 6 | 0 | 0 | 0.5 | RBM |
| `mp_battania_shoulder_furr` | Highland Shoulder Furr | Leather | T0 | 1 | 0 | 6 | 0 | 0 | 3.6 | Vanilla |
| `mp_empire_warrior_padded_armor_shoulder` | Padded Warrior Shoulders | Leather | T0 | 1 | 0 | 6 | 0 | 0 | 1.6 | Vanilla |
| `scarf` | Scarf | Cloth | T0 | auto | 0 | 6 | 0 | 0 | 0.5 | RBM |
| `southern_shawl` | Short Cloak | Cloth | T0 | auto | 0 | 6 | 0 | 0 | 0.4 | RBM |
| `wrapped_scarf` | Shawl | Cloth | T0 | auto | 0 | 6 | 0 | 0 | 0.5 | RBM |
| `mp_battania_cloak_furr` | Fur Cloak | Leather | T0 | 1 | 0 | 5 | 0 | 0 | 1.5 | Vanilla |
| `mp_battania_woodland_cloak` | Highland Woodland Cloak | Leather | T0 | 1 | 0 | 5 | 0 | 0 | 3 | Vanilla |
| `mp_battanian_leather_shoulder_a` | Cured Leather Shoulder Pieces | Cloth | T0 | 1 | 0 | 5 | 0 | 0 | 1.2 | Vanilla |
| `mp_desert_leather_shoulderpad` | Desert Leather Shoulderpad | Leather | T0 | 1 | 0 | 5 | 0 | 0 | 1.8 | Vanilla |
| `mp_eastern_studded_shoulders` | Steppe Studded Shoulders | Leather | T0 | 1 | 0 | 5 | 0 | 0 | 2.3 | Vanilla |
| `mp_padded_leather_shoulders` | Padded Leather Shoulders | Cloth | T0 | 1 | 0 | 5 | 0 | 0 | 1.4 | Vanilla |
| `mp_battania_shoulder_strap_cloak` | Battania Shoulder Strap Cloak | Leather | T-1 | 1 | 0 | 4 | 0 | 0 | 1.3 | Vanilla |
| `mp_khuzait_leather_pauldron` | Steppe Leather Pauldron | Leather | T-1 | 1 | 0 | 4 | 0 | 0 | 1.5 | Vanilla |
| `mp_green_hood` | Green Hood | Cloth | T-1 | 1 | 0 | 3 | 0 | 0 | 4 | Vanilla |
| `mp_reinforced_suede_shoulders` | Reinforced Suede Shoulders | Plate | T-1 | 1 | 0 | 3 | 0 | 0 | 3 | Vanilla |
| `mp_battania_cloak` | Highland Cloak | Cloth | T-1 | 1 | 0 | 2 | 0 | 0 | 0.5 | Vanilla |
| `mp_hood` | Hood | Cloth | T-1 | 1 | 0 | 2 | 0 | 0 | 0.4 | Vanilla |
| `mp_peasant_hood` | Hood | Cloth | T-1 | 1 | 0 | 2 | 0 | 0 | 0.4 | Vanilla |
| `mp_scarf` | Scarf | Cloth | T-1 | 1 | 0 | 2 | 0 | 0 | 0.5 | Vanilla |
| `mp_steppe_leather_shoulders` | Steppe Leather Shoulders | Leather | T-1 | 1 | 0 | 2 | 0 | 0 | 0.7 | Vanilla |
| `mp_wrapped_scarf` | Wrapped Scarf | Cloth | T-1 | 1 | 0 | 2 | 0 | 0 | 0.5 | Vanilla |

<!-- CULTURE:(none) COUNT:1 -->
| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| `wolf_shoulder_ulfhedinn` | Wolf Pelt Cape | Leather | T3 | auto | 0 | 23 | 0 | 0 | 3 | RBM |



