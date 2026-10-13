# Phase 1.1 (重做) · OSA Plate helm structure-bucket fill proposal

**MUST-FILL items**: 143 OSA Plate helmets (body_armor=0, head>0)
**Anchor source**: RBM own 172 plate helmets dissected into 10 structure buckets

## Bucket distribution of 143 OSA items
| bucket | OSA n | RBM anchor n | confidence | ratio used | proposed body avg | sample head |
|---|---|---|---|---|---|---|
| **over_mail** | 11 | 32 | **HIGH** | 0.165 | 14.0 | avg 84.8 |
| **closed_face** | 3 | 13 | **HIGH** | 0.638 | 31.9 | avg 50.0 |
| **spangenhelm** | 18 | 5 | **MEDIUM** | 0.113 | 7.7 | avg 68.3 |
| **lamellar_helm** | 5 | 1 | **LOW** | 0.000 | 0.0 | avg 55.8 |
| **nasal** | 11 | 20 | **HIGH** | 0.234 | 14.6 | avg 62.4 |
| **skullcap** | 10 | 15 | **HIGH** | 0.067 | 3.6 | avg 54.4 |
| **pointed** | 13 | 0 | **NONE** | 0.234 (fb:nasal) | 15.6 | avg 66.8 |
| **kettle** | 5 | 17 | **HIGH** | 0.020 | 1.4 | avg 69.4 |
| **crested** | 29 | 12 | **HIGH** | 0.153 | 10.8 | avg 70.5 |
| **other** | 38 | 51 | **HIGH** | 0.146 | 6.8 | avg 46.5 |

## Risk zones

🟡 **lamellar_helm**: RBM has only 1 item (ratio 0.000 · body=0 even in RBM own).
   If OSA Plate helm is `_lamellar_helmet` named, RBM anchor says body stays 0.
   This is a signal: lamellar helmets historically are open-face · face relies on
   underlying mail coif (which OSA may not model).

🟡 **crested/plumed**: RBM 6 items ratio 0.172 but variance is high — plumed_nomad
   is b=12 while empire_battle_crown_north is b=12 at h=104 (ratio 0.115) · it's
   mostly a decorator cue. Final value may need user override.

🟡 **other (catch-all)**: OSA items whose id/name has no clear cue · bucket avg
   ratio 0.145 is a rough fallback · better to spot-check.

## Per-item fill table (first 50 by head_armor desc)
| id | culture | head | bucket | proposed body | proposed arm (if 0) | name |
|---|---|---|---|---|---|---|
| `AR_battania_helmet_zf` | battania | 95 | over_mail | **16** | (kept) | Highland Heavy Nasalhelm Over Mail |
| `TV_battania_helmet_j` | battania | 95 | nasal | **22** | (kept) | Highland Feathered Heavy Bronze Nasalhel |
| `TV_battania_helmet_k` | battania | 95 | nasal | **22** | (kept) | Highland Heavy Bronze Nasalhelm |
| `TV_battania_helmet_n` | battania | 95 | crested | **15** | 7 | Highland Horned Steel Helmet With Plume |
| `TV_battania_helmet_o` | battania | 95 | crested | **15** | 7 | Highland Horned Bronze Helmet With Plume |
| `tv_vlandia_helmet_k3` | vlandia | 92 | over_mail | **15** | (kept) | Western Peaked Helmet Over Mail |
| `bearhelmet` | battania | 92 | crested | **14** | 7 | Bearskin Over Highland Helmet |
| `TV_pict_helmet_j` | battania | 90 | spangenhelm | **10** | 23 | Highland Tall Silvered Nasal Spangenhelm |
| `TV_pict_helmet_k` | battania | 90 | spangenhelm | **10** | 23 | Highland Plumed Tall Silvered Nasal Span |
| `AR_battania_lord_helmet_l` | battania | 90 | crested | **14** | (kept) | Highland Bronze Ridge Helmet With Feathe |
| `AR_battania_lord_helmet_m` | battania | 90 | crested | **14** | (kept) | Highland Ridge Helmet With Feather Crest |
| `wolfhelmet` | battania | 90 | crested | **14** | 7 | Wolfskin Over Highland Helmet |
| `TV_battania_lord_helmet_c` | battania | 89 | skullcap | **6** | 6 | Crested Decorated Steel Cap With Cheek G |
| `TV_pict_helmet_c` | battania | 88 | over_mail | **15** | (kept) | Highland Plumed Heavy Banded Nasal Helme |
| `TV_pict_helmet_f` | battania | 88 | over_mail | **15** | (kept) | Highland Domed Nasal Spangenhelm Over Ma |
| `TV_pict_helmet_i` | battania | 88 | over_mail | **15** | (kept) | Highland Tall Nasal Spangenhelm Over Mai |
| `TV_pict_helmet_l` | battania | 88 | over_mail | **15** | (kept) | Highland Plumed Tall Silvered Nasal Span |
| `TV_pict_helmet_d` | battania | 88 | spangenhelm | **10** | 23 | Highland Domed Nasal Spangenhelm |
| `TV_pict_helmet_g` | battania | 88 | spangenhelm | **10** | 23 | Highland Tall Nasal Spangenhelm |
| `TV_battania_helmet_h` | battania | 88 | spangenhelm | **10** | 23 | Highland Plumed Heavy Bronze Spangenhelm |
| `TV_battania_helmet_i` | battania | 88 | spangenhelm | **10** | 23 | Highland Feathered Heavy Bronze Spangenh |
| `wolfskin_helmet_guard` | battania | 88 | crested | **13** | 7 | Wolfskin Helmet |
| `AR_goth_helmet_a` | battania | 88 | other | **13** | (kept) | Iron Roughscale Helmet |
| `AR_battania_helmet_i` | battania | 85 | over_mail | **14** | (kept) | Highland Plumed Spangenhelm Over Mail |
| `TV_battania_helmet_g` | battania | 85 | spangenhelm | **10** | 22 | Highland Heavy Bronze Spangenhelm |
| `AR_battania_helmet_r` | battania | 85 | crested | **13** | 7 | Highland Steel Ridged Helmet |
| `tv_vlandia_helmet_o2` | vlandia | 84 | nasal | **20** | (kept) | Western Banded Nasal Cevelliere Over Pad |
| `AR_battania_helmet_s` | battania | 84 | crested | **13** | 6 | Highland Bronze Ridged Helmet |
| `AR_sturgia_helmet_n` | vlandia | 84 | other | **12** | (kept) | Cervelliere Over Open Mail |
| `celtic_mail_helm` | battania | 82 | over_mail | **14** | (kept) | Tall Nasalhelm With Mail |
| `AR_phrygian_helmet_b` | battania | 82 | pointed | **19** | 15 | Dryatic Steel Decorated Crested Bent Con |
| `AR_phrygian_helmet_b2` | battania | 82 | pointed | **19** | 15 | Dryatic Brass Decorated Crested Bent Con |
| `TV_pict_helmet_o` | battania | 82 | crested | **13** | 6 | Highland Crested Decorated Banded Helmet |
| `TV_battania_helmet_a` | battania | 79 | pointed | **18** | 15 | Highland Pointed Steel Kettle With Fring |
| `TV_battania_helmet_b` | battania | 79 | pointed | **18** | 15 | Highland Pointed Bronze Kettle With Frin |
| `TV_battania_helmet_c` | battania | 79 | crested | **12** | 6 | Highland Fringed Decorated Steel Helmet |
| `TV_khuzait_helmet_z3` | khuzait | 77 | over_mail | **13** | (kept) | Steppe Lamellar Helmet Over Mail |
| `TV_vlandia_helmet_h` | vlandia | 77 | kettle | **2** | 12 | Roundkettle Helmet Over Studded Leather |
| `AR_battania_helmet_b2` | battania | 75 | over_mail | **12** | (kept) | Highland Plumed Tall Helmet Over Mail |
| `ao_eorling_riveted_nasal_helmet` | battania | 75 | over_mail | **12** | (kept) | Highland Riveted Nasal Helmet With Mail  |
| `TV_pict_helmet_n` | battania | 75 | crested | **11** | 6 | Highland Decorated Banded Helmet |
| `TV_battania_helmet_e` | battania | 74 | crested | **11** | 6 | Highland Decorated Steel Helmet |
| `TV_battania_helmet_f` | battania | 74 | crested | **11** | 6 | Highland Decorated Bronze Helmet |
| `AR_phrygian_helmet_a` | battania | 73 | pointed | **17** | 14 | Dryatic Steel Decorated Bent Conical Hel |
| `AR_phrygian_helmet_a2` | battania | 73 | pointed | **17** | 14 | Dryatic Brass Decorated Bent Conical Hel |
| `TV_battania_helmet_d` | battania | 73 | crested | **11** | 6 | Highland Fringed Decorated Bronze Helmet |
| `TV_battania_helmet_s` | battania | 71 | pointed | **17** | 13 | Highland Pointed Steel Helmet |
| `TV_battania_helmet_s2` | battania | 71 | pointed | **17** | 13 | Highland Pointed Bronze Helmet |
| `AR_battania_helmet_d` | battania | 70 | kettle | **1** | (kept) | Highland Plumed Kettle Helmet |
| `hmj_imperial_cheek_pteurges_plume` | battania | 70 | kettle | **1** | (kept) | Highland Plumed Kettle Helmet With Strip |

...and 93 more items (see osa_phase1_1_proposal.json)