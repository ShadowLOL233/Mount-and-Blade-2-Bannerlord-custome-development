"""
Phase 1.1 pre-analysis · formula anchors for HeadArmor body_armor fill.

Reference anchor (journal 修改 #12 · 2026-09-20):
  RBM全量 256 head items:
    head_armor avg 63.9 (100% coverage)
    body_armor avg 27.3 (54% coverage) → body/head ratio ≈ 0.427
    arm_armor  avg 23.4 (50% coverage) → arm/head  ratio ≈ 0.366
  User rule: 仅 Chainmail/Plate 头盔补 body/arm · Cloth/Leather 跳过。
  Design philosophy (§48): 头 > 身 > 臂 strict ordering per HeadArmor.

Questions this script answers:
  Q1: Of the 176 body_armor=0 gap items, how many are Cloth/Leather (expected 0)
      vs Chainmail/Plate (真遗漏 · must fill)?
  Q2: In the 632 already-filled HeadArmor items, what's the body/head ratio
      distribution per material_type? Does it match the 0.427 anchor?
  Q3: For the "must fill" subset, bucket by (culture, material_type, head_armor
      range) to see where fills are most concentrated.
  Q4: Verify §48 ordering head >= body >= arm in the already-filled 632.
"""
import json, os, sys
from collections import defaultdict, Counter
if hasattr(sys.stdout, 'reconfigure'): sys.stdout.reconfigure(encoding='utf-8')

HERE = os.path.dirname(os.path.abspath(__file__))
with open(os.path.join(HERE, 'decisions_flat.json'), 'r', encoding='utf-8') as f:
    DEC = json.load(f)

def norm_type(t): return 'HeadArmor' if t.lower() == 'headarmor' else t
def iv(s):
    try: return int(s or 0)
    except: return 0

HEADS = [d for d in DEC if norm_type(d['Type']) == 'HeadArmor']
R = []
def P(s=''):
    R.append(s); print(s)

P('# Phase 1.1 Pre-Analysis · HeadArmor body_armor Fill Formula')
P(f'\n**Input**: 1760 decisions · {len(HEADS)} HeadArmor items')
P('**Anchor (journal 修改 #12 · 2026-09-20)**: RBM body/head avg ratio ≈ 0.427 · 仅 Chainmail/Plate 补 · Cloth/Leather 跳过')
P('**Design philosophy (§48)**: 头 > 身 > 臂 strict ordering')
P()

# =========================================================================
# Q1 · 176 gap items material_type breakdown
# =========================================================================
P('## Q1 · 176 gap items (body_armor=0) · material_type breakdown')
gaps = [d for d in HEADS if iv(d.get('body_armor','')) == 0]
P(f'Total gap: {len(gaps)}')
mt_dist = Counter(d.get('material_type','') for d in gaps)
P('\n| material_type | count | classification |')
P('|---|---|---|')
MT_FILL_RULE = {
    'Plate':     'FILL (RBM aventail 模型)',
    'Chainmail': 'FILL',
    'Leather':   'SKIP (设计意图 · 轻盔无 aventail)',
    'Cloth':     'SKIP',
    '':          'inspect (empty material_type)',
}
must_fill = []
intentional_skip = []
for mt, c in mt_dist.most_common():
    rule = MT_FILL_RULE.get(mt, 'inspect (unknown)')
    P(f'| {mt or "(empty)"} | {c} | {rule} |')
for d in gaps:
    mt = d.get('material_type','')
    if mt in ('Plate', 'Chainmail'):
        must_fill.append(d)
    else:
        intentional_skip.append(d)
P(f'\n**MUST FILL (Plate+Chainmail)**: {len(must_fill)} / 176 items')
P(f'**INTENTIONAL SKIP (Cloth+Leather)**: {len(intentional_skip)} / 176 items')
P()

# =========================================================================
# Q2 · 632 filled items body/head ratio distribution
# =========================================================================
P('## Q2 · body/head ratio distribution in 632 well-filled HeadArmor items')
filled = [d for d in HEADS if iv(d.get('body_armor','')) > 0 and iv(d.get('head_armor','')) > 0]
P(f'Well-filled sample: {len(filled)}')

by_mt = defaultdict(list)  # mt -> list of ratios
for d in filled:
    r = iv(d['body_armor']) / iv(d['head_armor'])
    by_mt[d.get('material_type','')].append((r, d))

def pct(vals, p):
    s = sorted(vals)
    if not s: return 0
    k = int(len(s) * p / 100)
    return s[min(k, len(s)-1)]

P('\n| material_type | n | mean ratio | p25 | p50 (median) | p75 | min | max |')
P('|---|---|---|---|---|---|---|---|')
for mt in ('Plate','Chainmail','Leather','Cloth',''):
    if mt not in by_mt: continue
    ratios = [r for r,_ in by_mt[mt]]
    mean = sum(ratios)/len(ratios)
    P(f'| {mt or "(empty)"} | {len(ratios)} | {mean:.3f} | {pct(ratios,25):.3f} | {pct(ratios,50):.3f} | {pct(ratios,75):.3f} | {min(ratios):.3f} | {max(ratios):.3f} |')
P('\n> Anchor target: 0.427 (RBM全量 avg per journal #12)')
P()

# =========================================================================
# Q3 · must-fill 176 subset · bucket (culture, material_type, head_armor range)
# =========================================================================
P('## Q3 · MUST FILL items bucketed by (culture, material_type, head range)')
def ha_bucket(ha):
    if ha < 20: return '<20'
    if ha < 30: return '20-29'
    if ha < 40: return '30-39'
    if ha < 50: return '40-49'
    if ha < 70: return '50-69'
    return '>=70'

bucket = Counter()
for d in must_fill:
    k = (d.get('culture','').replace('Culture.',''), d.get('material_type',''), ha_bucket(iv(d.get('head_armor',''))))
    bucket[k] += 1

P('\n| culture | material_type | head_armor bucket | count |')
P('|---|---|---|---|')
for (cult, mt, hb), c in sorted(bucket.items(), key=lambda x: -x[1]):
    P(f'| {cult or "(none)"} | {mt} | {hb} | {c} |')
P()

# =========================================================================
# Q4 · §48 head > body > arm ordering check (filled items only)
# =========================================================================
P('## Q4 · §48 ordering (head >= body >= arm) check · filled items only')
violations_hb = []
violations_ba = []
for d in filled:
    h, b, a = iv(d['head_armor']), iv(d['body_armor']), iv(d.get('arm_armor','') or 0)
    if h < b: violations_hb.append(d)
    if b < a: violations_ba.append(d)
P(f'- head < body: **{len(violations_hb)}** items')
P(f'- body < arm:  **{len(violations_ba)}** items')
if violations_hb:
    P('\n### head < body violations (first 10)')
    for d in violations_hb[:10]:
        P(f"- `{d['id']}` h={d['head_armor']} b={d['body_armor']} a={d.get('arm_armor','')} · {d.get('name','')[:60]}")
if violations_ba:
    P('\n### body < arm violations (first 10)')
    for d in violations_ba[:10]:
        P(f"- `{d['id']}` h={d['head_armor']} b={d['body_armor']} a={d.get('arm_armor','')} · {d.get('name','')[:60]}")
P()

# =========================================================================
# Q5 · Candidate formulas
# =========================================================================
P('## Q5 · Candidate formulas for MUST-FILL (143 Plate items)')
P('''
**🔴 核心分歧** · 两个权威数据不一致:

| 来源 | body/head ratio |
|---|---|
| Journal 修改 #12 anchor (RBM 全量 256 件 avg) | **0.427** |
| OSA 已填 Plate 609 件 p50 (实测中位数) | **0.214** |

差 2 倍。**选哪个？决定了 143 件新填值相差 1 倍**。

### Candidate A · 跟 OSA 既有一致 (ratio 0.21)
  `body_armor = round(head_armor × 0.21)`
  - **Pros**: 跟 OSA 作者/§48 既有 609 件 Plate 决议对账完美 · 补齐后风格不冲突 · 最保守
  - **Cons**: Face 保护弱 (大 helm head=100 → body=21) · 跟 RBM anchor 0.427 偏离
  - 例: battania Plate >=70 bucket 45 件 → head avg~90 · body 补 ~19

### Candidate B · 跟 RBM anchor 对齐 (ratio 0.42)
  `body_armor = round(head_armor × 0.42)`
  - **Pros**: Face 保护达 RBM 全量 avg 水平 · 修复 §影响 2 "单字段思维" 更彻底
  - **Cons**: 143 件新填值平均比 OSA 已填 609 件高 2× · 风格断裂 · 用户 2026-09-20 决议是"仅补 Chainmail/Plate" 当时没明确 anchor 值 · 可能当时也不是走 0.427
  - 例: battania Plate >=70 bucket 45 件 → head avg~90 · body 补 ~38

### Candidate C · 分阶梯 (低档 × 0.21 · 高档 × 0.35)
  ```
  head < 40:  body = round(head × 0.21)
  40-69:      body = round(head × 0.28)
  >= 70:      body = round(head × 0.35)
  ```
  - **Pros**: 高档 helm 走 vanilla full_helm_over_mail (ratio 0.83) 方向 · 低档保守
  - **Cons**: 阶梯边界 arbitrary · 143 件分布 bucket 不均 (143 件里 52 件 >=70 全升档)

### Candidate D · 兜底 flat small
  `body_armor = round(head_armor × 0.25)`
  - 最简单的"中间值" · 介于 A/B 之间 · 143 件均值自洽

### §48 ordering 检查 (Q4):
  已填 632 件里 **270 件 body < arm 违反 §48** · 说明 §48 "头>身>臂" 不是 OSA 作者严格执行的 · OSA 实际做法是 "aventail 肩袖 (arm) > 面罩 (body)" · 跟 §48 名义相反。所以**补 body 时不受 §48 约束** · 自由选 A/B/C/D。

### 建议路径
**从 A 起步** · 跟 OSA 既有 609 件对账最平滑 · 143 件补齐后 avg body 保持 0.21。
后续若实游戏测试发现"重甲飘刀"仍严重 (面部命中吃刀) · 再升级到 C 或 B。

A 可直接自动化 · 本机能生成 patched XML · 不需用户 case-by-case 判决。
B/C 需要用户追加确认 anchor 偏移。
''')

with open(os.path.join(HERE, 'preanalysis_1_1_report.md'), 'w', encoding='utf-8') as f:
    f.write('\n'.join(R))
print(f'\n[written] {HERE}\\preanalysis_1_1_report.md')
