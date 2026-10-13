"""
Dissect RBM's 172 plate helmets into structure buckets based on craft/layer cues
in id/name, then compute per-bucket body/head and arm/head ratios.

This is the real anchor for OSA Plate helmet body_armor fill.
Previous Phase 1.1 iteration used flat 0.427 (全类别 avg) — wrong.
Correct: per-structure-bucket ratio from RBM plate helmet itself.
"""
import json, re, os, sys
from collections import defaultdict, Counter
if hasattr(sys.stdout, 'reconfigure'): sys.stdout.reconfigure(encoding='utf-8')

HERE = os.path.dirname(os.path.abspath(__file__))
with open(os.path.join(HERE, 'rbm_armor_reference.json'), 'r', encoding='utf-8') as f:
    RBM = json.load(f)

def iv(s):
    try: return int(s or 0)
    except: return 0

PLATE_HELMS = [i for i in RBM if i['Type']=='HeadArmor' and i['modifier_group']=='plate' and iv(i['head_armor'])>0]
print(f'RBM plate helmets (head>0): {len(PLATE_HELMS)}')

# Structure cue hierarchy (highest-specific first — first match wins)
# Each cue has structural meaning:
#   full_helm / great_helm / closed_full : 全罩式 · 覆盖整个 Face + top
#   over_mail / with_mail / mail_coif : 盔下带 mail aventail (layered)
#   closed / goggled / faceguard / visored : 闭面但未必有 mail
#   bascinet / spangenhelm / lamellar_helmet : 中档 open-face 带可选面罩
#   nasal / cone / pointed / skullcap / cap : 低档 open-face
#   kettle / wide-brim : 阔沿盔 (open face · body=0 even in plate)
STRUCTURE_CUES = [
    ('full_helm',        r'full[_\s-]?helm|great[_\s-]?helm'),
    ('over_mail_full',   r'full[_\s-]?helm.*over.*mail|over.*mail.*full[_\s-]?helm'),
    ('over_mail',        r'over[_\s-]?mail|with[_\s-]?mail|over[_\s-]?full[_\s-]?mail|mail_hauberk|over[_\s-]?coif|with[_\s-]?coif'),
    ('closed_face',      r'closed|_close(_|$)|goggled|face[_\s-]?guard|faceguard|bascinet|visor|bevor'),
    ('spangenhelm',      r'spangen|segmentata'),
    ('lamellar_helm',    r'lamellar'),
    ('nasal',            r'nasal'),
    ('skullcap',         r'skullcap|skull[_\s-]?cap|cap[_\s-]|_cap$|^cap'),
    ('pointed',          r'pointed|conical|_cone|cone[_\s-]'),
    ('kettle',           r'kettle|wide[_\s-]?brim'),
    ('crested',          r'crest|plume|feather|\bhorn|ridge|ridged|decorated|fringed|banded|wolfskin|bearskin'),
    ('other',            r'.'),  # catch-all
]

def classify(item):
    tok = (item['id'] + ' ' + item.get('name','')).lower()
    for bucket, pat in STRUCTURE_CUES:
        if re.search(pat, tok):
            return bucket
    return 'other'

# Run
by_bucket = defaultdict(list)
for i in PLATE_HELMS:
    by_bucket[classify(i)].append(i)

print('\n### RBM plate helmet (172 items) · structure bucket ratios')
print('| bucket | n | head avg | head p50 | body avg | body/head | arm avg | arm/head | arm/body |')
print('|---|---|---|---|---|---|---|---|---|')
def pct(vals, p):
    s = sorted(vals)
    if not s: return 0
    k = int(len(s) * p / 100)
    return s[min(k, len(s)-1)]

bucket_stats = {}
for name, _ in STRUCTURE_CUES:
    group = by_bucket.get(name, [])
    if not group: continue
    heads = [iv(i['head_armor']) for i in group]
    bodies = [iv(i['body_armor']) for i in group]
    arms = [iv(i['arm_armor']) for i in group]
    head_avg = sum(heads)/len(heads)
    body_avg = sum(bodies)/len(bodies)
    arm_avg = sum(arms)/len(arms)
    bh = body_avg/head_avg if head_avg else 0
    ah = arm_avg/head_avg if head_avg else 0
    ab = arm_avg/body_avg if body_avg else 0
    bucket_stats[name] = dict(n=len(group), head_avg=head_avg, body_avg=body_avg,
                              arm_avg=arm_avg, body_head=bh, arm_head=ah,
                              arm_body=ab, head_p50=pct(heads,50))
    print(f'| {name} | {len(group)} | {head_avg:.1f} | {pct(heads,50)} | {body_avg:.1f} | **{bh:.3f}** | {arm_avg:.1f} | **{ah:.3f}** | {ab:.2f} |')

# Save bucket stats
with open(os.path.join(HERE, 'rbm_plate_bucket_stats.json'), 'w', encoding='utf-8') as f:
    json.dump(bucket_stats, f, ensure_ascii=False, indent=1)

# Show samples per bucket
print('\n### Samples per bucket (first 5 each)')
for name, _ in STRUCTURE_CUES:
    group = by_bucket.get(name, [])
    if not group: continue
    print(f'\n#### `{name}` ({len(group)} items)')
    for i in group[:5]:
        print(f"- `{i['id']}` h={i['head_armor']} b={i['body_armor']} a={i['arm_armor']} · {i.get('name','')[:60]}")
