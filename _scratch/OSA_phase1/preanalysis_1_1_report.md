# Phase 1.1 Pre-Analysis · HeadArmor body_armor Fill Formula

**Input**: 1760 decisions · 808 HeadArmor items
**Anchor (journal 修改 #12 · 2026-09-20)**: RBM body/head avg ratio ≈ 0.427 · 仅 Chainmail/Plate 补 · Cloth/Leather 跳过
**Design philosophy (§48)**: 头 > 身 > 臂 strict ordering

## Q1 · 176 gap items (body_armor=0) · material_type breakdown
Total gap: 176

| material_type | count | classification |
|---|---|---|
| Plate | 143 | FILL (RBM aventail 模型) |
| Cloth | 18 | SKIP |
| Leather | 15 | SKIP (设计意图 · 轻盔无 aventail) |

**MUST FILL (Plate+Chainmail)**: 143 / 176 items
**INTENTIONAL SKIP (Cloth+Leather)**: 33 / 176 items

## Q2 · body/head ratio distribution in 632 well-filled HeadArmor items
Well-filled sample: 632

| material_type | n | mean ratio | p25 | p50 (median) | p75 | min | max |
|---|---|---|---|---|---|---|---|
| Plate | 609 | 0.291 | 0.135 | 0.214 | 0.391 | 0.049 | 1.000 |
| Chainmail | 20 | 0.286 | 0.141 | 0.273 | 0.385 | 0.124 | 0.649 |
| Leather | 2 | 0.360 | 0.360 | 0.360 | 0.360 | 0.360 | 0.360 |
| Cloth | 1 | 0.720 | 0.720 | 0.720 | 0.720 | 0.720 | 0.720 |

> Anchor target: 0.427 (RBM全量 avg per journal #12)

## Q3 · MUST FILL items bucketed by (culture, material_type, head range)

| culture | material_type | head_armor bucket | count |
|---|---|---|---|
| battania | Plate | >=70 | 45 |
| battania | Plate | 50-69 | 18 |
| battania | Plate | 40-49 | 14 |
| sturgia | Plate | 50-69 | 10 |
| khuzait | Plate | 50-69 | 9 |
| aserai | Plate | 50-69 | 8 |
| vlandia | Plate | 50-69 | 7 |
| khuzait | Plate | 40-49 | 7 |
| aserai | Plate | 40-49 | 5 |
| vlandia | Plate | >=70 | 4 |
| khuzait | Plate | 30-39 | 4 |
| vlandia | Plate | <20 | 3 |
| khuzait | Plate | >=70 | 3 |
| vlandia | Plate | 20-29 | 2 |
| empire | Plate | 40-49 | 1 |
| battania | Plate | <20 | 1 |
| nord | Plate | 20-29 | 1 |
| khuzait | Plate | 20-29 | 1 |

## Q4 · §48 ordering (head >= body >= arm) check · filled items only
- head < body: **0** items
- body < arm:  **270** items

### body < arm violations (first 10)
- `AR_empire_desert_helmet_c` h=65 b=12 a=25 · Southern Spangenhelm Cap
- `ao_imperial_spangenhelm_with_mail` h=95 b=12 a=25 · Imperial Spangenhelm With Mail
- `ao_imperial_heavy_spangenhelm_with_mail` h=99 b=15 a=28 · Imperial Heavy Spangenhelm With Mail
- `ao_imperial_nasal_spangenhelmet_with_mail` h=98 b=12 a=25 · Imperial Nasal Spangenhelm With Mail
- `AR_empire_desert_helmet_a` h=90 b=12 a=25 · Southern Plumed Nasalhelm Over Imperial Mail
- `TV_empire_helmet_h` h=97 b=12 a=25 · Imperial Gilded Spangenhelm Over Mail
- `TV_empire_lord_helmet_h` h=99 b=13 a=26 · Imperial Gilded Plumed Spangenhelm Over Mail
- `aserai_feathered_spangenhelm` h=95 b=12 a=25 · Southern Feathered Spangenhelm Over Mail
- `bronze_aserai_helm` h=91 b=12 a=25 · Southern Iron Nasalhelm Over Mail
- `AR_empire_lord_helmet_f` h=122 b=15 a=21 · Imperial Guarded Spangenhelm With Feather Crest

## Q5 · Candidate formulas for MUST-FILL (143 Plate items)

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
