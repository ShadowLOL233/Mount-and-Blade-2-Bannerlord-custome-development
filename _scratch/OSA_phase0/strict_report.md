# Phase 0.B Strict Reclassifier Report

Input: 1648 OSA items (all_items.json)

## Tier A (high confidence · auto-accept to production)
- POM: 187  (dist: {'Culture.aserai': 49, 'Culture.empire': 39, 'Culture.vlandia': 28, 'Culture.sturgia': 26, 'Culture.battania': 24, 'Culture.khuzait': 21})
- PBS: 23  (dist: {'Culture.battania': 14, 'Culture.empire': 8, 'Culture.khuzait': 1})
- CFM: 97  (dist: {'Culture.vlandia': 36, 'Culture.sturgia': 22, 'Culture.empire': 15, 'Culture.aserai': 12, 'Culture.khuzait': 12})

## Tier B (medium · needs manual review)
- POM: 0  (empty — scale/lamellar covers everything; placeholder)
- PBS: 0  (BodyArmor Plate without naming cue but body_armor>=35)
- CFM: 31  (closed-face cue but no explicit mail cue — may still be CFM if aventail is implicit)

## Rejected (not in any whitelist)
- Plate BodyArmor rejected: 10  (low body_armor + no cue → likely cosmetic/light armor mis-marked as Plate)
- HeadArmor rejected: 715  (no closed-face cue → open/half-face helmets)

## Tier A production counts (ready to inject)
| Whitelist | Vanilla | Tier A | Combined |
|---|---|---|---|
| POM | 20 | 187 | 207 |
| PBS |  9 | 23 | 32 |
| CFM | 16 | 97 | 113 |

## Tier B samples (first 20 each · to help manual review)
### PBS Tier B (plate body_armor>=35, no cue)

### CFM Tier B (closed-face cue, no mail cue)
- `AR_aserai_helmet_v` head=42 cult=Culture.aserai name={=osa_ar_aserai_helmet_v}Southern Iron Closed Helmet
- `AR_aserai_helmet_v2` head=43 cult=Culture.aserai name={=osa_ar_aserai_helmet_v2}Southern Iron Closed Helmet with T
- `AR_battania_helmet_j` head=30 cult=Culture.battania name={=osa_ar_battania_helmet_j}Highland Steel Helmet With Facegu
- `AR_battania_helmet_k` head=30 cult=Culture.battania name={=osa_ar_battania_helmet_k}Highland Bronze Helmet With Faceg
- `AR_battania_helmet_n` head=30 cult=Culture.battania name={=osa_ar_battania_helmet_n}Plumed Highland Steel Helmet With
- `AR_battania_helmet_o` head=30 cult=Culture.battania name={=osa_ar_battania_helmet_o}Plumed Highland Bronze Helmet Wit
- `AR_battania_helmet_v` head=30 cult=Culture.battania name={=osa_ar_battania_helmet_v}Highland Steel Tall Helmet With F
- `AR_battania_helmet_v2` head=30 cult=Culture.battania name={=osa_ar_battania_helmet_v2}Highland Bronze Tall Helmet With
- `AR_empire_crested_helm_a` head=36 cult=Culture.empire name={=osa_ar_empire_crested_helm_a}Imperial Crested Helmet With 
- `AR_roman_helmet_b_fur_a` head=32 cult=Culture.empire name={=osa_ar_roman_helmet_b_fur_a}Imperial Scout's Helmet With F
- `AR_roman_helmet_b_plumed` head=30 cult=Culture.empire name={=osa_ar_roman_helmet_b_plumed}Plumed Helmet With Faceguard
- `AR_roman_helmet_b_plumed_b` head=30 cult=Culture.empire name={=osa_ar_roman_helmet_b_plumed_b}Plumed Feathered Helmet Wit
- `AR_goth_helmet_b` head=38 cult=Culture.battania name={=osa_ar_goth_helmet_b}Iron Roughscale Helmet With Faceguard
- `AR_goth_helmet_c` head=46 cult=Culture.battania name={=osa_ar_goth_helmet_c}Plumed Iron Roughscale Helmet With Fa
- `AR_khuzait_helmet_j` head=46 cult=Culture.khuzait name={=osa_ar_khuzait_helmet_j}Eastern Closed Plumed Helmet
- `AR_khuzait_helmet_u` head=34 cult=Culture.khuzait name={=osa_ar_khuzait_helmet_u}Eastern Nasalhelm With Suede Faceg
- `AR_khuzait_helmet_v` head=40 cult=Culture.khuzait name={=osa_ar_khuzait_helmet_v}Eastern Nasalhelm With Studded Fac
- `AR_vlandia_helmet_x` head=30 cult=Culture.vlandia name={=osa_ar_vlandia_helmet_x}Western Banded Nasalhelm with Face
- `AR_vlandia_helmet_z` head=37 cult=Culture.vlandia name={=osa_ar_vlandia_helmet_z}Western Visored Ridge Helmet Over 
- `AR_vlandia_lord_helmet_c` head=44 cult=Culture.vlandia name={=osa_ar_vlandia_lord_helmet_c}Western Noble Goggled Bandedh