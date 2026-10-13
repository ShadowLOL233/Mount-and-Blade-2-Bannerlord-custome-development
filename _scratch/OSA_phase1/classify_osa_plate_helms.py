"""
Classify 143 OSA MUST-FILL Plate helmets into RBM structure buckets.
For each item, compute proposed body_armor = round(head_armor * bucket.body_head)
and proposed arm_armor if arm=0 and bucket has arm_head.

Output: _scratch/OSA_phase1/osa_phase1_1_proposal.{json,md}

User decision point per item: accept bucket classification, override, or defer.
"""
import json, re, os, sys
from collections import defaultdict, Counter
if hasattr(sys.stdout, 'reconfigure'): sys.stdout.reconfigure(encoding='utf-8')

HERE = os.path.dirname(os.path.abspath(__file__))

# Load OSA 1760 decisions
with open(os.path.join(HERE, 'decisions_flat.json'), 'r', encoding='utf-8') as f:
    DEC = json.load(f)

# Load RBM bucket stats
with open(os.path.join(HERE, 'rbm_plate_bucket_stats.json'), 'r', encoding='utf-8') as f:
    RBM_BUCKETS = json.load(f)

def norm_type(t): return 'HeadArmor' if t.lower() == 'headarmor' else t
def iv(s):
    try: return int(s or 0)
    except: return 0

# Same cue hierarchy as RBM analysis — first match wins
STRUCTURE_CUES = [
    ('full_helm',        r'full[_\s-]?helm|great[_\s-]?helm'),
    ('over_mail',        r'over[_\s-]?mail|with[_\s-]?mail|over[_\s-]?full[_\s-]?mail|mail_hauberk|over[_\s-]?coif|with[_\s-]?coif'),
    ('closed_face',      r'closed|_close(_|$)|goggled|face[_\s-]?guard|faceguard|bascinet|visor|bevor'),
    ('spangenhelm',      r'spangen|segmentata'),
    ('lamellar_helm',    r'lamellar'),
    ('nasal',            r'nasal'),
    ('skullcap',         r'skullcap|skull[_\s-]?cap|cap[_\s-]|_cap$|^cap'),
    ('pointed',          r'pointed|conical|_cone|cone[_\s-]'),
    ('kettle',           r'kettle|wide[_\s-]?brim'),
    ('crested',          r'crest|plume|feather|\bhorn|ridge|ridged|decorated|fringed|banded|wolfskin|bearskin'),
    ('other',            r'.'),
]

def classify(item):
    tok = (item['id'] + ' ' + item.get('name','')).lower()
    for bucket, pat in STRUCTURE_CUES:
        if re.search(pat, tok):
            return bucket
    return 'other'

# 143 MUST-FILL = HeadArmor + Plate + body=0 + head>0
MUST = [d for d in DEC if norm_type(d['Type'])=='HeadArmor'
        and d.get('material_type','')=='Plate'
        and iv(d.get('body_armor',''))==0
        and iv(d.get('head_armor',''))>0]
print(f'OSA MUST-FILL Plate helms: {len(MUST)}')

# Bucket OSA ones
by_bucket = defaultdict(list)
for i in MUST:
    by_bucket[classify(i)].append(i)

# Produce proposals
proposals = []
for bucket_name, _ in STRUCTURE_CUES:
    group = by_bucket.get(bucket_name, [])
    if not group: continue
    stats = RBM_BUCKETS.get(bucket_name, {})
    bh = stats.get('body_head', 0)
    ah = stats.get('arm_head', 0)
    rbm_n = stats.get('n', 0)
    # Confidence tier based on RBM anchor sample size
    if rbm_n >= 10:   conf = 'HIGH'
    elif rbm_n >= 5:  conf = 'MEDIUM'
    elif rbm_n >= 1:  conf = 'LOW'
    else:             conf = 'NONE'
    # Fallback for buckets with no RBM anchor (currently just 'pointed')
    fallback_bh = fallback_ah = None
    if conf == 'NONE':
        # pointed/conical open-face top helm → fallback to nasal bucket (both are
        # open-face with similar structure).
        nasal_stats = RBM_BUCKETS.get('nasal', {})
        fallback_bh = nasal_stats.get('body_head', 0)
        fallback_ah = nasal_stats.get('arm_head', 0)
    for i in group:
        h = iv(i['head_armor'])
        use_bh = fallback_bh if fallback_bh is not None else bh
        use_ah = fallback_ah if fallback_ah is not None else ah
        proposed_body = round(h * use_bh)
        existing_arm = iv(i.get('arm_armor','') or 0)
        existing_body = iv(i.get('body_armor','') or 0)
        proposals.append({
            'id': i['id'],
            'name': i.get('name',''),
            'culture': i.get('culture','').replace('Culture.',''),
            'head_armor': h,
            'current_body_armor': existing_body,
            'current_arm_armor': existing_arm,
            'bucket': bucket_name,
            'confidence': conf,
            'rbm_bucket_n': rbm_n,
            'rbm_body_head_ratio': use_bh,
            'rbm_arm_head_ratio': use_ah,
            'fallback_bucket': 'nasal' if fallback_bh is not None else None,
            'proposed_body_armor': proposed_body,
            'proposed_arm_armor_if_zero': round(h * use_ah) if existing_arm == 0 else None,
        })

# Save
with open(os.path.join(HERE, 'osa_phase1_1_proposal.json'), 'w', encoding='utf-8') as f:
    json.dump(proposals, f, ensure_ascii=False, indent=1)

# Report
R = []
def P(s=''): R.append(s); print(s)

P('# Phase 1.1 (重做) · OSA Plate helm structure-bucket fill proposal')
P(f'\n**MUST-FILL items**: {len(MUST)} OSA Plate helmets (body_armor=0, head>0)')
P('**Anchor source**: RBM own 172 plate helmets dissected into 10 structure buckets')
P()
P('## Bucket distribution of 143 OSA items')
dist = Counter(p['bucket'] for p in proposals)
P('| bucket | OSA n | RBM anchor n | confidence | ratio used | proposed body avg | sample head |')
P('|---|---|---|---|---|---|---|')
# Pick representative from proposals for ratio/confidence consistency
for name, _ in STRUCTURE_CUES:
    n = dist.get(name, 0)
    if n == 0: continue
    bucket_proposals = [p for p in proposals if p['bucket']==name]
    rep = bucket_proposals[0]
    conf = rep['confidence']
    bh_used = rep['rbm_body_head_ratio']
    fb = rep.get('fallback_bucket')
    heads = [p['head_armor'] for p in bucket_proposals]
    head_avg = sum(heads)/len(heads) if heads else 0
    pb_avg = head_avg * bh_used
    ratio_cell = f'{bh_used:.3f}' + (f' (fb:{fb})' if fb else '')
    P(f'| **{name}** | {n} | {rep["rbm_bucket_n"]} | **{conf}** | {ratio_cell} | {pb_avg:.1f} | avg {head_avg:.1f} |')
P()

P('## Risk zones')
P('''
🟡 **lamellar_helm**: RBM has only 1 item (ratio 0.000 · body=0 even in RBM own).
   If OSA Plate helm is `_lamellar_helmet` named, RBM anchor says body stays 0.
   This is a signal: lamellar helmets historically are open-face · face relies on
   underlying mail coif (which OSA may not model).

🟡 **crested/plumed**: RBM 6 items ratio 0.172 but variance is high — plumed_nomad
   is b=12 while empire_battle_crown_north is b=12 at h=104 (ratio 0.115) · it's
   mostly a decorator cue. Final value may need user override.

🟡 **other (catch-all)**: OSA items whose id/name has no clear cue · bucket avg
   ratio 0.145 is a rough fallback · better to spot-check.
''')

P('## Per-item fill table (first 50 by head_armor desc)')
P('| id | culture | head | bucket | proposed body | proposed arm (if 0) | name |')
P('|---|---|---|---|---|---|---|')
sorted_p = sorted(proposals, key=lambda x: -x['head_armor'])
for p in sorted_p[:50]:
    arm_cell = p['proposed_arm_armor_if_zero'] if p['proposed_arm_armor_if_zero'] is not None else '(kept)'
    P(f"| `{p['id']}` | {p['culture']} | {p['head_armor']} | {p['bucket']} | **{p['proposed_body_armor']}** | {arm_cell} | {p['name'][:40]} |")
P()
P(f'...and {len(proposals)-50} more items (see osa_phase1_1_proposal.json)')

with open(os.path.join(HERE, 'osa_phase1_1_proposal.md'), 'w', encoding='utf-8') as f:
    f.write('\n'.join(R))
print(f'\n[written] osa_phase1_1_proposal.{{json,md}}')
