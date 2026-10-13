# Phase 1 Field Completeness + MaterialType Audit Report

**Input**: _scratch/OSA_phase1/decisions_flat.json · 1760 decisions

## Type distribution (headArmor merged into HeadArmor)
- HeadArmor       : 808
- BodyArmor       : 398
- Cape            : 324
- HorseHarness    : 110
- LegArmor        : 67
- HandArmor       : 53

## Field Completeness Gaps
Total items with missing or zero required field: **292 / 1760 (16.6%)**

### BodyArmor (6/398 items with gaps · 1.5%)
- **zero value** (field present but = 0):
  - `arm_armor`: 6 items
  - `body_armor`: 4 items

### Cape (108/324 items with gaps · 33.3%)
- **zero value** (field present but = 0):
  - `arm_armor`: 106 items
  - `body_armor`: 2 items

### HeadArmor (176/808 items with gaps · 21.8%)
- **zero value** (field present but = 0):
  - `body_armor`: 176 items

### HorseHarness (2/110 items with gaps · 1.8%)
- **zero value** (field present but = 0):
  - `body_armor`: 2 items
  - `leg_armor`: 2 items

## MaterialType ↔ modifier_group Mismatches
Total: **0** items with modifier_group not matching material_type


## Severity Summary (what breaks what)

Impact classes (per RBM hitbox formulas):

| Gap | RBM downstream | severity |
|---|---|---|
| HeadArmor `body_armor` missing/0 | Face hit calculated with 0 protection (even with Plate helmet) | 🔴 critical — face dies to any pierce |
| BodyArmor `arm_armor` missing/0 | Shoulder/LowerShoulder uses main body only · no shoulder-specific curve | 🟡 moderate — ~50% shoulder protection miss |
| BodyArmor `leg_armor` missing/0 | Leg hit lacks the chest-contributed 50% leg protection | 🟡 moderate — ~50% upper-leg protection miss |
| Cape `body_armor` or `arm_armor` missing/0 | Removes 1 of 3 Shoulder sources | 🟢 minor — ~33% shoulder contribution lost |
| HorseHarness any field missing/0 | Corresponding horse body region lacks horse-level protection (§49 修订后全 4 字段算) | 🟡 moderate — scoped to mount only |
| LegArmor `leg_armor` missing/0 | Boot's half of leg protection lost | 🟡 moderate — half of foot/lower-leg bare |
| HandArmor `arm_armor` missing/0 | Hand hitbox bare | 🟢 minor — rare hit zone |
| MaterialType enum wrong | Wrong RBM damage-type curve applied silently (暗坑) | 🟡 moderate — kind of damage felt wrong |
