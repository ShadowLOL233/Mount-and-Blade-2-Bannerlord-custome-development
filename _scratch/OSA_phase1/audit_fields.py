"""
Phase 1 · field completeness + MaterialType audit.
Input:  _scratch/OSA_phase1/decisions_flat.json (1760 decisions)
Output: _scratch/OSA_phase1/audit_report.md
        + audit_field_gaps.json / audit_materialtype_mismatches.json

Rules (RBM hitbox formulas, see reference-rbm-mechanics / journal §影响 2):
  HeadArmor  : head_armor (主) · body_armor (Face 唯一防护 · 必填非 0)
  BodyArmor  : body_armor (主) · arm_armor (Shoulder+LowerShoulder) · leg_armor (腿部 50%)
  Cape       : body_armor (Shoulder 源1) · arm_armor (Shoulder 源2 · §48 Cape 铁律)
  HorseHarness: 全 4 字段 (body/head/leg/arm) · §49 修订后全参与计算
  LegArmor   : leg_armor (主 · 腿部另 50%)
  HandArmor  : arm_armor (主)

MaterialType cross-check: modifier_group plate↔Plate, chain↔Chainmail, leather↔Leather, cloth↔Cloth.
"""
import json, os, sys
from collections import defaultdict, Counter
if hasattr(sys.stdout, 'reconfigure'): sys.stdout.reconfigure(encoding='utf-8')

HERE = os.path.dirname(os.path.abspath(__file__))
with open(os.path.join(HERE, 'decisions_flat.json'), 'r', encoding='utf-8') as f:
    DECISIONS = json.load(f)

def norm_type(t): return 'HeadArmor' if t.lower() == 'headarmor' else t

def iv(s):
    try: return int(s or 0)
    except: return 0

# ---------- Field completeness rules ----------
REQUIRED_FIELDS = {
    'HeadArmor':    ['head_armor', 'body_armor'],      # head=主, body=Face
    'BodyArmor':    ['body_armor', 'arm_armor', 'leg_armor'],
    'Cape':         ['body_armor', 'arm_armor'],
    'HorseHarness': ['body_armor', 'head_armor', 'leg_armor', 'arm_armor'],
    'LegArmor':     ['leg_armor'],
    'HandArmor':    ['arm_armor'],
}

# MaterialType ↔ modifier_group canonical mapping
MG_TO_MT = {
    'plate':   'Plate',
    'chain':   'Chainmail',
    'leather': 'Leather',
    'cloth':   'Cloth',
}

field_gaps = []       # {id, type, missing_fields, zero_fields, name}
mat_mismatches = []   # {id, type, modifier_group, material_type, expected, name}

for d in DECISIONS:
    t = norm_type(d['Type'])
    req = REQUIRED_FIELDS.get(t, [])
    missing = [f for f in req if d.get(f, '') == '']
    zero    = [f for f in req if d.get(f, '') != '' and iv(d.get(f, '')) == 0]
    if missing or zero:
        field_gaps.append({
            'id': d['id'], 'Type': t, 'culture': d.get('culture',''),
            'missing_fields': missing, 'zero_fields': zero,
            'name': d.get('name','')[:70],
            'has_values': {f: d.get(f,'') for f in req},
        })

    mg = (d.get('modifier_group') or '').lower()
    mt = d.get('material_type') or ''
    expected = MG_TO_MT.get(mg)
    # HorseHarness has 'chain' with Chainmail OR 'plate' with Plate; same canonical rules.
    # Skip if modifier_group empty (common on Cape — Cape has no material curve to audit).
    if expected and mt and mt != expected:
        mat_mismatches.append({
            'id': d['id'], 'Type': t, 'culture': d.get('culture',''),
            'modifier_group': mg, 'material_type': mt, 'expected': expected,
            'name': d.get('name','')[:70],
        })

# Save detail JSONs
with open(os.path.join(HERE, 'audit_field_gaps.json'), 'w', encoding='utf-8') as f:
    json.dump(field_gaps, f, ensure_ascii=False, indent=1)
with open(os.path.join(HERE, 'audit_materialtype_mismatches.json'), 'w', encoding='utf-8') as f:
    json.dump(mat_mismatches, f, ensure_ascii=False, indent=1)

# ---------- Report ----------
R = []
def P(s=''):
    R.append(s); print(s)

P('# Phase 1 Field Completeness + MaterialType Audit Report')
P(f'\n**Input**: _scratch/OSA_phase1/decisions_flat.json · {len(DECISIONS)} decisions')
P()
# Type dist
type_dist = Counter(norm_type(d['Type']) for d in DECISIONS)
P('## Type distribution (headArmor merged into HeadArmor)')
for t, c in type_dist.most_common():
    P(f'- {t:16s}: {c}')

# Field gaps grouped by Type
P('\n## Field Completeness Gaps')
P(f'Total items with missing or zero required field: **{len(field_gaps)} / {len(DECISIONS)} ({100*len(field_gaps)/len(DECISIONS):.1f}%)**\n')
by_type = defaultdict(list)
for g in field_gaps: by_type[g['Type']].append(g)
for t in sorted(by_type.keys()):
    gaps = by_type[t]
    total_t = type_dist[t]
    P(f'### {t} ({len(gaps)}/{total_t} items with gaps · {100*len(gaps)/total_t:.1f}%)')
    # Which specific fields are most missing?
    missing_counter = Counter()
    zero_counter = Counter()
    for g in gaps:
        for f in g['missing_fields']: missing_counter[f] += 1
        for f in g['zero_fields']: zero_counter[f] += 1
    if missing_counter:
        P('- **missing attribute** (field absent from <Armor> tag):')
        for f, c in missing_counter.most_common():
            P(f'  - `{f}`: {c} items')
    if zero_counter:
        P('- **zero value** (field present but = 0):')
        for f, c in zero_counter.most_common():
            P(f'  - `{f}`: {c} items')
    P()

# MaterialType mismatches
P('## MaterialType ↔ modifier_group Mismatches')
P(f'Total: **{len(mat_mismatches)}** items with modifier_group not matching material_type\n')
if mat_mismatches:
    # Group by (modifier_group, material_type)
    pair_counter = Counter((m['modifier_group'], m['material_type']) for m in mat_mismatches)
    P('### Mismatch patterns')
    P('| modifier_group | material_type | expected | count |')
    P('|---|---|---|---|')
    for (mg, mt), c in pair_counter.most_common():
        exp = MG_TO_MT.get(mg, '?')
        P(f'| `{mg}` | `{mt}` | `{exp}` | {c} |')
    P()
    # First 15 samples
    P('### Samples (first 15)')
    for m in mat_mismatches[:15]:
        P(f"- `{m['id']}` [{m['Type']}] mg=`{m['modifier_group']}` mt=`{m['material_type']}` (expected `{m['expected']}`) · {m['name']}")

# Severity ranking
P('\n## Severity Summary (what breaks what)')
P('''
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
''')

with open(os.path.join(HERE, 'audit_report.md'), 'w', encoding='utf-8') as f:
    f.write('\n'.join(R))
print(f'\n[written] {HERE}\\audit_report.md')
