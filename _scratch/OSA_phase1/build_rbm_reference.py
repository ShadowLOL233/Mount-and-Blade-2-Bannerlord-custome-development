"""
Phase 1.1 (重做) · Build RBM armor reference database.

Source: D:\\...\\workshop\\content\\261550\\2859251492\\ModuleData\\RBMCombat_*_armors.xml
RBM's own overhauls (not OSA): the authoritative baseline for RBM-aware balance.

Output: _scratch/OSA_phase1/rbm_armor_reference.json  — flat index of all RBM
        armor items with normalized fields.
Also:   _scratch/OSA_phase1/rbm_vocab.md               — modifier_group / material_type
                                                        vocabulary + sample names per group.

Why: Phase 1.1 previous iteration used OSA's own fill as anchor — mistake, because
     OSA was designed under vanilla game assumptions, not RBM. Correct anchor is
     RBM's own 255 helmets + 246 bodies + 91 shoulders overhaul data.
"""
import json, re, os, sys
from collections import defaultdict, Counter
if hasattr(sys.stdout, 'reconfigure'): sys.stdout.reconfigure(encoding='utf-8')

HERE = os.path.dirname(os.path.abspath(__file__))
RBM = r'D:\SteamLibrary\steamapps\workshop\content\261550\2859251492\ModuleData'

TARGET_XMLS = [
    'RBMCombat_head_armors.xml',
    'RBMCombat_body_armors.xml',
    'RBMCombat_shoulder_armors.xml',
    'RBMCombat_arm_armors.xml',
    'RBMCombat_leg_armors.xml',
    'RBMCombat_horse_armors.xml',
]

items = []
for fn in TARGET_XMLS:
    path = os.path.join(RBM, fn)
    if not os.path.exists(path):
        print(f'[skip] {fn} missing')
        continue
    with open(path, 'r', encoding='utf-8-sig') as f:
        txt = f.read()
    file_count = 0
    for m in re.finditer(r'<Item\b([^>]*)>([\s\S]*?)</Item>', txt):
        hdr, inner = m.group(1), m.group(2)
        h = dict(re.findall(r'(\w+)=\"([^\"]*)\"', hdr))
        arm = re.search(r'<Armor\b([^/>]*)/?>', inner)
        a = dict(re.findall(r'(\w+)=\"([^\"]*)\"', arm.group(1))) if arm else {}
        rec = {
            '_source_xml': fn,
            'id': h.get('id',''),
            'name': h.get('name','').split('}')[-1] if '}' in h.get('name','') else h.get('name',''),
            'Type': h.get('Type',''),
            'subtype': h.get('subtype',''),
            'culture': h.get('culture',''),
            'weight': h.get('weight',''),
        }
        for k in ('head_armor','body_armor','arm_armor','leg_armor',
                  'material_type','modifier_group','covers_body','covers_head',
                  'covers_legs','covers_hands','family_type'):
            rec[k] = a.get(k,'')
        items.append(rec)
        file_count += 1
    print(f'[{fn}] {file_count} items')

print(f'Total: {len(items)}')
with open(os.path.join(HERE, 'rbm_armor_reference.json'), 'w', encoding='utf-8') as f:
    json.dump(items, f, ensure_ascii=False, indent=1)

# ============================
# Vocabulary analysis
# ============================
def iv(s):
    try: return int(s or 0)
    except: return 0

vocab_lines = ['# RBM Armor Vocabulary (modifier_group × material_type × Type)\n']
vocab_lines.append(f'Total: **{len(items)}** items across {len(TARGET_XMLS)} XMLs\n')

# modifier_group distribution per Type
by_type = defaultdict(list)
for i in items: by_type[i['Type']].append(i)
for t in sorted(by_type.keys()):
    tl = by_type[t]
    vocab_lines.append(f'\n## Type `{t}` · {len(tl)} items\n')
    # modifier_group distribution
    mg_dist = Counter(i['modifier_group'] for i in tl)
    mt_dist = Counter(i['material_type'] for i in tl)
    vocab_lines.append('### modifier_group distribution')
    for mg, c in mg_dist.most_common():
        vocab_lines.append(f'- `{mg or "(empty)"}`: {c}')
    vocab_lines.append('### material_type distribution')
    for mt, c in mt_dist.most_common():
        vocab_lines.append(f'- `{mt or "(empty)"}`: {c}')
    # Per modifier_group · body/head/arm ratios (only for HeadArmor + non-zero head)
    if t == 'HeadArmor':
        vocab_lines.append('### body/head and arm/head ratios per modifier_group (head>0)')
        vocab_lines.append('| modifier_group | n | head avg | body avg | body/head | arm/head |')
        vocab_lines.append('|---|---|---|---|---|---|')
        for mg, c in mg_dist.most_common():
            group = [i for i in tl if i['modifier_group']==mg and iv(i['head_armor'])>0]
            if not group: continue
            head_avg = sum(iv(i['head_armor']) for i in group)/len(group)
            body_avg = sum(iv(i['body_armor']) for i in group)/len(group)
            arm_avg  = sum(iv(i['arm_armor']) for i in group)/len(group)
            bh = body_avg / head_avg if head_avg else 0
            ah = arm_avg / head_avg if head_avg else 0
            vocab_lines.append(f'| `{mg or "(empty)"}` | {len(group)} | {head_avg:.1f} | {body_avg:.1f} | **{bh:.3f}** | **{ah:.3f}** |')

with open(os.path.join(HERE, 'rbm_vocab.md'), 'w', encoding='utf-8') as f:
    f.write('\n'.join(vocab_lines))
print(f'[written] rbm_armor_reference.json + rbm_vocab.md')
