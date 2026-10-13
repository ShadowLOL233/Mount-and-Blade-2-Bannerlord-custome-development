"""
Phase 1 · parse OSABalance_manual_override.xml into a flat index.

Each item in manual_override.xml is one of the 1835 hand-audited balance decisions.
Each has:
  - id, name, Type, culture, weight (identifying)
  - head/body/arm/leg armor fields (hitbox-contributing)
  - material_type, modifier_group (RBM material curve inputs)

Output:
  _scratch/OSA_phase1/decisions_flat.json  — 1835 flat records
"""
import json, re, os, sys
if hasattr(sys.stdout, 'reconfigure'): sys.stdout.reconfigure(encoding='utf-8')

HERE = os.path.dirname(os.path.abspath(__file__))
XML = os.path.normpath(os.path.join(HERE, '..', '..',
    'OpenSourceArmouryRBMBalance', 'ModuleData', 'OSABalance_manual_override.xml'))
OUT = os.path.join(HERE, 'decisions_flat.json')

with open(XML, 'r', encoding='utf-8-sig') as f:
    txt = f.read()

items = []
for m in re.finditer(r'<Item\b([^>]*)>([\s\S]*?)</Item>', txt):
    hdr, inner = m.group(1), m.group(2)
    hdr_attrs = dict(re.findall(r'(\w+)=\"([^\"]*)\"', hdr))
    arm = re.search(r'<Armor\b([^/>]*)/?>', inner)
    a = dict(re.findall(r'(\w+)=\"([^\"]*)\"', arm.group(1))) if arm else {}
    hh = re.search(r'<HorseHarness\b([^/>]*)/?>', inner)
    h = dict(re.findall(r'(\w+)=\"([^\"]*)\"', hh.group(1))) if hh else {}
    # Items with HorseHarness instead of Armor (saddle mods)
    rec = {
        'id': hdr_attrs.get('id',''),
        'name': hdr_attrs.get('name','').split('}')[-1] if '}' in hdr_attrs.get('name','') else hdr_attrs.get('name',''),
        'Type': hdr_attrs.get('Type',''),
        'subtype': hdr_attrs.get('subtype',''),
        'culture': hdr_attrs.get('culture',''),
        'weight': hdr_attrs.get('weight',''),
        'has_armor_component': bool(arm),
        'has_horse_harness_component': bool(hh),
    }
    if arm:
        for k in ('head_armor','body_armor','arm_armor','leg_armor',
                  'material_type','modifier_group','covers_body','covers_head','covers_legs','covers_hands'):
            rec[k] = a.get(k,'')
    elif hh:
        # HorseHarness uses body/head/leg/arm fields natively
        for k in ('body_armor','head_armor','leg_armor','arm_armor','modifier_group'):
            rec[k] = h.get(k,'')
    items.append(rec)

with open(OUT, 'w', encoding='utf-8') as f:
    json.dump(items, f, ensure_ascii=False, indent=1)

# Summary
from collections import Counter
print(f'Total decisions: {len(items)}')
type_dist = Counter(i['Type'] for i in items)
for t,c in type_dist.most_common():
    print(f'  {t:16s}: {c}')
print(f'HorseHarness items (special): {sum(1 for i in items if i["has_horse_harness_component"])}')
print(f'Written: {OUT}')
