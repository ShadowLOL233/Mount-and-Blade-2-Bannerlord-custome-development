# Phase 0.B Strict Reclassifier Report

Input: 1648 OSA items (all_items.json)

## Tier A (high confidence · auto-accept to production)
- POM: 187  (dist: {'Culture.aserai': 49, 'Culture.empire': 39, 'Culture.vlandia': 28, 'Culture.sturgia': 26, 'Culture.battania': 24, 'Culture.khuzait': 21})
- PBS: 21  (dist: {'Culture.battania': 13, 'Culture.empire': 8})
- CFM: 80  (dist: {'Culture.vlandia': 35, 'Culture.sturgia': 14, 'Culture.khuzait': 11, 'Culture.empire': 10, 'Culture.aserai': 10})

## Tier B (medium · needs manual review)
- POM: 0  (empty — scale/lamellar covers everything; placeholder)
- PBS: 1  (BodyArmor Plate without naming cue but body_armor>=35)
- CFM: 62  (closed-face cue but no explicit mail cue — may still be CFM if aventail is implicit)

## Rejected (not in any whitelist)
- Plate BodyArmor rejected: 10  (low body_armor + no cue → likely cosmetic/light armor mis-marked as Plate)
- HeadArmor rejected: 701  (no closed-face cue → open/half-face helmets)

## Tier A production counts (ready to inject)
| Whitelist | Vanilla | Tier A | Combined |
|---|---|---|---|
| POM | 20 | 187 | 207 |
| PBS |  9 | 21 | 30 |
| CFM | 16 | 80 | 96 |

## Tier B samples (first 20 each · to help manual review)
### PBS Tier B (plate body_armor>=35, no cue)
- `AR_khuzait_armor_a` body=45 arm=16 leg=17 cult=Culture.khuzait name={=osa_ar_khuzait_armor_a}Eastern Plated Armor

### CFM Tier B (closed-face cue, no mail cue)
- `ao_sturgian_feathered_helmet_closed` head=53 cult=Culture.sturgia name={=osa_ao_sturgian_feathered_helmet_closed}Northern Feathered
- `saracen_helmet_c` head=23 cult=Culture.aserai name={=osa_saracen_helmet_c}Closed Southern Spiked Helmet
- `southern_light_helmet_a` head=22 cult=Culture.aserai name={=osa_southern_light_helmet_a}Southern Closed Pointed Skullc
- `AR_aserai_archer_helmet_b` head=21 cult=Culture.aserai name={=osa_ar_aserai_archer_helmet_b}Southern Closed Archer Helme
- `AR_aserai_archer_helmet_f` head=21 cult=Culture.aserai name={=osa_ar_aserai_archer_helmet_f}Southern Closed Domed Archer
- `AR_aserai_helmet_v` head=42 cult=Culture.aserai name={=osa_ar_aserai_helmet_v}Southern Iron Closed Helmet
- `AR_aserai_helmet_v2` head=43 cult=Culture.aserai name={=osa_ar_aserai_helmet_v2}Southern Iron Closed Helmet with T
- `AR_aserai_lord_helmet_f` head=52 cult=Culture.aserai name={=osa_ar_aserai_lord_helmet_f}Darshi Closed Noble Cavalry He
- `AR_battania_helmet_j` head=30 cult=Culture.battania name={=osa_ar_battania_helmet_j}Highland Steel Helmet With Facegu
- `AR_battania_helmet_k` head=30 cult=Culture.battania name={=osa_ar_battania_helmet_k}Highland Bronze Helmet With Faceg
- `AR_battania_helmet_l` head=20 cult=Culture.battania name={=osa_ar_battania_helmet_l}Highland Open Steel Helmet With F
- `AR_battania_helmet_m` head=20 cult=Culture.battania name={=osa_ar_battania_helmet_m}Highland Open Bronze Helmet With 
- `AR_battania_helmet_n` head=30 cult=Culture.battania name={=osa_ar_battania_helmet_n}Plumed Highland Steel Helmet With
- `AR_battania_helmet_o` head=30 cult=Culture.battania name={=osa_ar_battania_helmet_o}Plumed Highland Bronze Helmet Wit
- `AR_battania_helmet_v` head=30 cult=Culture.battania name={=osa_ar_battania_helmet_v}Highland Steel Tall Helmet With F
- `AR_battania_helmet_v2` head=30 cult=Culture.battania name={=osa_ar_battania_helmet_v2}Highland Bronze Tall Helmet With
- `AR_empire_crested_helm_a` head=36 cult=Culture.empire name={=osa_ar_empire_crested_helm_a}Imperial Crested Helmet With 
- `AR_empire_helmet_h` head=51 cult=Culture.empire name={=osa_ar_empire_helmet_h}Imperial Gilded Closed Helmet With 
- `AR_empire_lord_helmet_a` head=51 cult=Culture.empire name={=osa_ar_empire_lord_helmet_a}Imperial Lord Helmet With Face
- `AR_roman_helmet_a` head=20 cult=Culture.empire name={=osa_ar_roman_helmet_a}Imperial Open Helmet With Faceguard