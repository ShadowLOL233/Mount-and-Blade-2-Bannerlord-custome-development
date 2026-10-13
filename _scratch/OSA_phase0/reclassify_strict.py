"""
Phase 0.B strict reclassifier (v5).
Input:  _scratch/OSA_phase0/all_items.json (1648 flat items)
Output: _scratch/OSA_phase0/strict_{pom,pbs,cfm}.{tierA,tierB}.json
        + strict_report.md summarising counts, culture distribution, edge cases.

v6 changes (over v5):
  - CFM open anti-cue reorder: mail cue now takes priority over 'Open' — names like
    'Over Open Mail Coif' or 'Vendel Goggled Over Open Mail' have an explicit mail
    presence and should be Tier A even though they contain the word 'Open'. The old
    rule demoted them to Tier B. 'Open' only triggers rejection when NO mail cue hits.
  - CFM Tier B promotion threshold lowered to head_armor >= 50 (no elite cue required):
    OSA convention for head_armor >= 50 is near-universal mail aventail attachment.
  - Secutor/gladiator anti-cue: integrated-faceplate helmets have no mail backing;
    exclude from auto-promotion to keep head>=50 rule safe.

v5 changes:
  - PBS: add '\\bplated\\b' word-boundary cue to catch names like 'Eastern Plated Armor'
  - CFM Tier B promotion for head_armor >= 45 AND elite cue
  - CFM reject: head_armor <= 24

v2-v4 history retained for context (see git log).

Semantics derived from vanilla RBMCombat whitelists (see _scratch/RBM_decomp/RBMCombat.decompiled.cs):
  POM : BodyArmor whose SHOULDER region is visually mail (RBM demotes shoulder material
        Plate->Chainmail). Vanilla cue coverage: 'scale' OR 'lamellar' hits 19/20;
        1 exception (empire_legion_b) is a lamellar+mail segmentata.
  PBS : BodyArmor whose below-shoulder region is full plate with no chain transition
        (RBM keeps below-shoulder on Plate curve). Vanilla cues: plated_leather, fortified,
        heavy_armor, raider_armor + 1 overlap (western_scale_mail — scale shoulders, plate below).
  CFM : HeadArmor with closed face AND mail aventail (RBM gives face hit mail protection).
        Vanilla: strict cue requires BOTH 'closed/goggled/faceguard' AND 'with_mail/over_mail'.
        5 vanilla entries (nordic, eastern_vendel, lendman, vlandian_faceguard, decorated_goggled)
        satisfy the closed-face half but their mail aventail is implicit — these go to tier B.
"""
import json, re, os, sys
if hasattr(sys.stdout, 'reconfigure'):
    sys.stdout.reconfigure(encoding='utf-8')

HERE = os.path.dirname(os.path.abspath(__file__))

with open(os.path.join(HERE, 'all_items.json'), 'r', encoding='utf-8') as f:
    ITEMS = json.load(f)

# ----- POM rules -----
# Primary cue: scale OR lamellar OR "over mail" / "over chain" / "on mail" / "on cloth".
# OSA often puts "Over Mail" in the name rather than id, so we must match name text too.
POM_PRIMARY = re.compile(
    r'(scale|lamellar|over[_\s-]?mail|over[_\s-]?chain|on[_\s-]?mail|on[_\s-]?cloth|'
    r'coat[_\s-]?of[_\s-]?plates|plate[_\s-]?over|segmentata|reticulated[_\s-]?plate)',
    re.I)
POM_OVERMAIL_CUE = re.compile(r'(over_mail|over.?mail|with_mail|on_cloth|on.?cloth|with.?chain|mail_hauberk)', re.I)
# Reject POM if clearly a NON-chest piece we accidentally grabbed (shouldn't happen because
# we already filter Type=BodyArmor, but belt+Supplementary).

# ----- PBS rules -----
# Vanilla cues: plated_leather, fortified, heavy_armor, raider_armor.
# OSA naming extends to: warlord, breastplate, plate_cuirass, full_plate, reinforced_plate,
# plate_body, platebody, lorica (lorica_segmentata etc.), coat_of_plates (when NOT 'over_mail').
PBS_PRIMARY = re.compile(
    r'(plated_leather|plated.?leather|\bplated\b|fortified|heavy_armor|heavy.?cuirass|'
    r'raider_armor|plate_cuirass|full_plate|segmented_cuirass|cuirass_heavy|warlord|'
    r'breastplate|plate_body|platebody|reinforced_plate|lorica_segmentata|coat_of_plates)',
    re.I)
# PBS tier B threshold: no cue, but body_armor >= 35 (vanilla PBS floor roughly here)
PBS_TIERB_BODY_MIN = 35

# ----- CFM rules -----
# Closed cue: 'closed' / 'close' word, goggled, faceguard, great_helm, bascinet, bevor,
# full_helm, visor. Reject if name explicitly says 'Open'.
CFM_CLOSED_CUE = re.compile(
    r'(closed|_close(_|$)|goggled|faceguard|face[_\s-]?guard|great[_\s-]?helm|greathelm|'
    r'bascinet|bevor|full[_\s-]?helm|visor)', re.I)
# Mail-equivalent cue: word-boundary 'mail' / 'aventail' / 'coif' (chainmail hood) /
# 'lamellar' (lamellar face/aventail is functionally equivalent to mail backing) /
# 'vendel' (vendel-style helmet always has mail aventail in Bannerlord/OSA mesh set).
CFM_MAIL_CUE = re.compile(
    r'(\bmail\b|aventail|\bcoif\b|\blamellar\b|\bvendel\b|mail_coif|mail_backing|closed_mail)',
    re.I)
# Open-face anti-cue: if name says 'Open ... Helmet' it is NOT closed-face even if a
# faceguard is attached, so we move to Tier B instead of Tier A.
CFM_OPEN_ANTI = re.compile(r'\bopen\b', re.I)
# Elite high-tier helmet cue (v5): Lord/Noble/Cataphract/Guard/Cavalry → OSA convention
# gives mail aventail even if name doesn't say so.
CFM_ELITE_CUE = re.compile(r'(\blord\b|\bnoble\b|cataphract|\bguard\b|\bcavalry\b|\broyal\b|\belite\b|\bimperial\b)', re.I)
# v6 anti-cue: integrated-faceplate helms (Secutor/gladiator) have NO mail backing even
# though head_armor can be high. Exclude from head>=50 auto-promotion.
CFM_INTEGRATED_FACEPLATE = re.compile(r'(secutor|gladiator)', re.I)
# Head-armor thresholds for Tier B → promotion / reject
CFM_PROMOTE_HA_MIN       = 45   # v5 elite-cue path (unchanged)
CFM_PROMOTE_HA_MIN_V6    = 50   # v6 head-only path (no elite cue required)
CFM_REJECT_HA_MAX        = 24

def item_text(it):
    return (it['id'] + ' ' + it.get('name','')).lower()

def is_body_plate_or_chain(it):
    """Vanilla POM uses MaterialType=Plate but RBM demotes Shoulder->Chainmail. Our
    Phase 0 target is MaterialType=Plate items only (RBM's whitelist check is gated
    on MaterialType==Plate). Chainmail bodies are already treated as chain by RBM,
    no injection needed."""
    return it['Type'] == 'BodyArmor' and it['material_type'] == 'Plate'

def is_head(it):
    return it['Type'].lower() == 'headarmor'

# ----- Classify -----
pom_A, pom_B = [], []
pbs_A, pbs_B = [], []
cfm_A, cfm_B = [], []
plate_body_rejected = []
head_rejected = []

for it in ITEMS:
    tok = item_text(it)
    if is_body_plate_or_chain(it):
        pom_cue = POM_PRIMARY.search(tok)
        pbs_cue = PBS_PRIMARY.search(tok)
        if pom_cue and not pbs_cue:
            pom_A.append(it)
        elif pbs_cue and not pom_cue:
            pbs_A.append(it)
        elif pom_cue and pbs_cue:
            # Overlap (like vanilla western_scale_mail which is in BOTH lists).
            # Put in both at tier A.
            pom_A.append(it)
            pbs_A.append(it)
        else:
            # No primary cue. Tier B heuristic: high body_armor + plate → PBS candidate.
            try:
                ba = int(it.get('body_armor','0') or '0')
            except: ba = 0
            if ba >= PBS_TIERB_BODY_MIN:
                pbs_B.append(it)
            else:
                plate_body_rejected.append(it)
    elif is_head(it):
        closed    = CFM_CLOSED_CUE.search(tok)
        mail      = CFM_MAIL_CUE.search(tok)
        open_face = CFM_OPEN_ANTI.search(tok)
        integrated = CFM_INTEGRATED_FACEPLATE.search(tok)
        try: ha = int(it.get('head_armor','0') or '0')
        except: ha = 0
        if closed and mail:
            # v6: mail cue takes priority over 'Open' — 'Over Open Mail Coif' / 'Vendel
            # Goggled Over Open Mail' have explicit mail presence despite 'Open' word.
            cfm_A.append(it)
        elif closed:
            # No explicit mail cue. Apply promotion paths:
            elite = CFM_ELITE_CUE.search(tok)
            if integrated:
                # Secutor/gladiator → integrated faceplate, no mail backing.
                head_rejected.append(it)
            elif ha >= CFM_PROMOTE_HA_MIN_V6 and not open_face:
                # v6: head >= 50 → OSA convention attaches mail aventail silently.
                cfm_A.append(it)
            elif ha >= CFM_PROMOTE_HA_MIN and elite and not open_face:
                # v5: elite high-tier closed helm (Lord/Noble/Cataphract/etc.)
                cfm_A.append(it)
            elif ha <= CFM_REJECT_HA_MAX:
                head_rejected.append(it)
            else:
                cfm_B.append(it)
        else:
            head_rejected.append(it)

def save(name, data):
    with open(os.path.join(HERE, f'strict_{name}.json'), 'w', encoding='utf-8') as f:
        json.dump([i['id'] for i in data], f, ensure_ascii=False, indent=1)

save('pom.tierA', pom_A)
save('pom.tierB', pom_B)  # empty by design — kept for symmetry
save('pbs.tierA', pbs_A)
save('pbs.tierB', pbs_B)
save('cfm.tierA', cfm_A)
save('cfm.tierB', cfm_B)

# ----- Report -----
from collections import Counter
def dist(lst, key='culture'):
    return dict(Counter(i[key] for i in lst).most_common())

report_lines = []
def P(s=''):
    report_lines.append(s)
    print(s)

P("# Phase 0.B Strict Reclassifier Report\n")
P(f"Input: 1648 OSA items (all_items.json)\n")
P("## Tier A (high confidence · auto-accept to production)")
P(f"- POM: {len(pom_A)}  (dist: {dist(pom_A)})")
P(f"- PBS: {len(pbs_A)}  (dist: {dist(pbs_A)})")
P(f"- CFM: {len(cfm_A)}  (dist: {dist(cfm_A)})")
P()
P("## Tier B (medium · needs manual review)")
P(f"- POM: {len(pom_B)}  (empty — scale/lamellar covers everything; placeholder)")
P(f"- PBS: {len(pbs_B)}  (BodyArmor Plate without naming cue but body_armor>={PBS_TIERB_BODY_MIN})")
P(f"- CFM: {len(cfm_B)}  (closed-face cue but no explicit mail cue — may still be CFM if aventail is implicit)")
P()
P("## Rejected (not in any whitelist)")
P(f"- Plate BodyArmor rejected: {len(plate_body_rejected)}  (low body_armor + no cue → likely cosmetic/light armor mis-marked as Plate)")
P(f"- HeadArmor rejected: {len(head_rejected)}  (no closed-face cue → open/half-face helmets)")
P()
P("## Tier A production counts (ready to inject)")
P(f"| Whitelist | Vanilla | Tier A | Combined |")
P(f"|---|---|---|---|")
P(f"| POM | 20 | {len(pom_A)} | {20+len(pom_A)} |")
P(f"| PBS |  9 | {len(pbs_A)} | {9+len(pbs_A)} |")
P(f"| CFM | 16 | {len(cfm_A)} | {16+len(cfm_A)} |")
P()
P("## Tier B samples (first 20 each · to help manual review)")
P("### PBS Tier B (plate body_armor>=35, no cue)")
for i in pbs_B[:20]:
    P(f"- `{i['id']}` body={i['body_armor']} arm={i['arm_armor']} leg={i['leg_armor']} cult={i['culture']} name={i.get('name','')[:60]}")
P()
P("### CFM Tier B (closed-face cue, no mail cue)")
for i in cfm_B[:20]:
    P(f"- `{i['id']}` head={i['head_armor']} cult={i['culture']} name={i.get('name','')[:60]}")

with open(os.path.join(HERE, 'strict_report.md'), 'w', encoding='utf-8') as f:
    f.write('\n'.join(report_lines))
