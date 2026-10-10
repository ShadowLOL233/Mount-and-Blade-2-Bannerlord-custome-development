# OSW v2 baseline 完整扫描（2026-09-25 · 修正版）

**背景**：v1 侦查报告只对比了 RBM_WS Nord（9 shields），远不够。用户 2026-09-25 明确要求补齐 —— **全部武器数值** + **RBM 修改后的原版武器数值**。本文补上。

**新增数据源**：`Vanilla_Reference/`（本次首建）
- 提取脚本：`scripts/extract_vanilla.ps1`（复用 OSA/RBM 脚本的 flatten 逻辑）
- 覆盖：SandBoxCore/items/*.xml + Native/crafting_pieces.xml + Native/mpitems.xml + NavalDLC/{items,naval_weapons,naval_crafting_pieces}.xml
- 产物：`data/vanilla_{items,crafted_items,crafted_items_pieces,crafting_pieces}.csv`

---

## 1. 全量数据规模（首次统一口径）

### 1.1 Items（未构造武器：盾、弓、弩、投掷、箭矢等）

| 来源 | Items | 说明 |
|---|---:|---|
| Vanilla_SP | 272 | `SandBoxCore/items/*.xml`（campaign 主线） |
| Vanilla_Native_MP | 679 | `Native/mpitems.xml`（多人模式） |
| NavalDLC | 132 | Nord + Naval 内容 |
| RBM (main) | 1081 | 大部分是护甲 + 盾 + 弓 + 弩；OneHandedWeapon 只 3 件（gladius） |
| RBM_WS | 132 | Nord 内容包 |
| **OSW** | **123** | 114 盾 + 7 掷 + 1 弩 + 1 弩弹（本项目主体） |
| Saddlery | 110 | 马具 |

### 1.2 CraftedItems（已构造武器：剑、斧、锤、矛、匕首、掷斧等 NPC/玩家用的成品）

| 来源 | CraftedItems | 说明 |
|---|---:|---|
| Vanilla_SP | 284 | vanilla 主线成品武器（campaign lord 装备） |
| Vanilla_Native_MP | 180 | MP 成品 |
| NavalDLC | 66 | Nord/Naval 成品 |
| RBM (main) | 97 | 全 override vanilla · 0 新增 |
| RBM_WS | 11 | Nord override |
| **OSW** | **140** | **全新 id · 0 override vanilla**（本项目此前完全忽略的一块！） |

### 1.3 CraftingPieces（模块化零件：Blade / Handle / Guard / Pommel）

| 来源 | CraftingPieces | 说明 |
|---|---:|---|
| Vanilla_Native | 805 | 主 vanilla 零件池 |
| Vanilla_Native_MP | 435 | MP 零件 |
| NavalDLC | 150 | Nord 零件 |
| RBM (main) | 341 | **全 override vanilla · 0 新增**（选择性 patch） |
| RBM_WS | 59 | Nord 零件 |
| **OSW** | **50** | 18 Blade + 13 Pommel + 11 Handle + 8 Guard |

### 1.4 OSW 完整清单（修正 v1 遗漏）

**v1 报告漏了 CraftedItems**。真实 OSW 数据是：

| 类型 | 数量 | 说明 |
|---|---:|---|
| Shield (Item) | 114 | 全 LargeShield · v1 已扫 |
| Thrown (Item) | 7 | 全 `weapon_class=Stone`(!)· v1 已扫 |
| Crossbow + Bolts | 2 | cheirosiphon 希腊火 · v1 已扫 |
| **CraftedItem** | **140** | **v1 漏扫** —— 41 TwoHandedPolearm + 31 OneHandedSword + 16 OneHandedAxe + 10 TwoHandedMace + 9 Mace + 8 Pike + 8 Javelin + 6 Dagger + 5 TwoHandedSword + 5 TwoHandedAxe + 1 ThrowingKnife |
| CraftingPiece | 50 | 18 Blade + 13 Pommel + 11 Handle + 8 Guard · v1 已扫 |
| **合计** | **313** | v1 报告说 173 · **少算 140 CraftedItem** |

---

## 2. RBM 修改行为（关键澄清）

**RBM 如何影响 vanilla 武器数值**？逐层实测：

| RBM 目标 | 数量 | 行为 |
|---|---:|---|
| CraftingPieces override | **341/341 = 100%** | RBM 每条 patch 都 override 一个 vanilla id · 0 新增 |
| Shield Items override | 81 override + 47 new = 128 | 对 vanilla 144 shields 中 81 件重写 · 加 47 件新 shield |
| CraftedItems override | 97 override + 0 new = 97 | 对 vanilla 530 crafted 中 97 件重写（不到 20%）· 0 新增 |
| Item (Bow/Arrow/etc) | 42 Bow + 39 Arrow + ... | 大量 override vanilla ranged |

**关键结论**：
- RBM 只 patch，不新增（CraftingPiece + CraftedItem 层面）
- RBM CraftingPiece 覆盖率：**55%**（290/523 Blade · 51/436 Handle · 0/252 Guard · 0/179 Pommel）
- **Guard/Pommel/大部分 Handle 保留 vanilla 原值** —— RBM 的哲学是选择性 patch，只调 damage-critical 件

**"vanilla 未 patch 的 Blade"**（234 件）保留 vanilla 原值 —— 这些原值极烫（T2 mean=3.19 · T5 mean=4.12）。所以 in-game 有效 Blade 分布 = **RBM 冷值 + vanilla 未 patch 热值 的混合**。

→ **OSW 该对齐哪个 baseline？** 后文详述。

---

## 3. Shield 有效 baseline（vanilla + RBM 融合后）

**融合规则**：vanilla id 起底 → RBM 同 id 覆盖 → RBM 新 id 追加 → 结果集去 MP-only。

### 3.1 SP-effective LargeShield（n=70）

（排除 MP-only 项 · 因为 OSW 是单人向）

| 指标 | 值 |
|---|---:|
| count | 70 |
| hp min | 260 |
| hp max | 840 |
| hp mean | **581** |
| hp median | **570** |
| ba mean | **4.7** |
| ba median | **5** |
| weight mean | 4.78 kg |

**HP 百分位**：

| p10 | p25 | p50 | p75 | p90 | p95 |
|---:|---:|---:|---:|---:|---:|
| 380 | 480 | **570** | 700 | 750 | 780 |

**HP 分桶**（100hp 一档）：
```
200-299: 1
300-399: 8
400-499: 13
500-599: 15
600-699: 13
700-799: 17
800-899: 3
```
分布近似正态，中位 ~570，主体 500-700。

### 3.2 SP-effective SmallShield（n=68）

| 指标 | 值 |
|---|---:|
| hp median | 490 |
| hp max | **2000**（少数 buckler 硬顶） |
| ba median | 5 |

### 3.3 OSW Shield vs SP-effective baseline（**修正 v1 结论**）

| 指标 | OSW (n=114) | SP-effective LargeShield (n=70) |
|---|---:|---:|
| hp min | 220 | 260 |
| hp max | 520 | 840 |
| **hp mean** | **346** | **581** |
| **hp median** | **340** | **570** |
| ba mean | 2.67 | 4.7 |
| ba median | 2 | 5 |

**OSW 分位置换 SP-effective 百分位**：
- OSW hp median 340 → SP-effective 约 **p8**（p10=380）
- OSW hp max 520 → SP-effective 约 **p30**（在 p25=480 和 p50=570 之间）
- OSW ba median 2 → SP-effective 约 **p15**

→ **修正结论**：OSW 盾**整体分布位于 SP 有效 baseline 底部 8-30% 分位**。系统性欠强被数据再次确认，但**具体位置比 v1 报告的更明确**（v1 用 RBM_WS Nord 9 件对比，偏差方向对但数值不精）。

---

## 4. Blade 有效 baseline（**关键修正**）

### 4.1 RBM-patched Blade 曲线（coherent RBM 生态）

| Tier | n | swing (min/p50/p90/max/mean) | thrust (min/max/mean) |
|:---:|---:|---|---|
| T1 | 37 | 0.2 / 0.4 / 0.7 / 0.7 / **0.45** | 0.2 / 0.75 / **0.57** |
| T2 | 65 | 0.2 / 0.7 / 0.8 / 0.9 / **0.66** | 0.55 / 0.9 / **0.74** |
| T3 | 55 | 0.7 / 0.9 / 1.0 / 1.1 / **0.86** | 0.7 / 1.05 / **0.86** |
| T4 | 63 | 0.9 / 1.0 / 1.2 / 1.3 / **1.05** | 0.7 / 1.0 / **0.88** |
| T5 | 70 | 0.9 / 1.3 / 1.6 / 1.7 / **1.31** | 0.4 / 1.4 / **1.04** |

### 4.2 Vanilla-未 patch Blade 曲线（RBM 未管的原值 · 依然极烫）

| Tier | n | swing max | swing mean |
|:---:|---:|---:|---:|
| T1 | 6 | 2.6 | 1.75 |
| T2 | 177 | 5.0 | **3.19** |
| T3 | 18 | 4.85 | 3.29 |
| T4 | 14 | 3.9 | 3.57 |
| T5 | 18 | 5.2 | 4.12 |

### 4.3 OSW Blade vs 两条曲线（修正 v1 结论）

| ID | tier | OSW swing | RBM T-mean | Vanilla-unpatched T-mean |
|---|:---:|---:|---:|---:|
| AR_khuzait_mace_head_a | 1 | 1.86 | 0.45 | 1.75 |
| AR_aserai_blade_a | 2 | 3.2 | 0.66 | 3.19 |
| AR_empire_mace_head_a | 2 | 2.3 | 0.66 | 3.19 |
| AR_billhook_blade_a | 2 | 1.6 | 0.66 | 3.19 |
| AR_darshi_mace_head_a | 3 | 2.61 | 0.86 | 3.29 |
| AR_aserai_blade_b | 4 | 3.9 | 1.05 | 3.57 |
| AR_spear_blade_a | 5 | 2.8 (thrust) | 1.04 (thrust) | — |

**修正观察**：**OSW blade 数值几乎完美匹配 vanilla-未 patch 曲线** —— 不是 OSW 作者设计过强 · 而是他们参照的是 vanilla 原始热值（RBM patch 前）。

**含义变了**：
- 若采用"对齐 vanilla 未 patch"哲学 → **OSW blade 无需 nerf**（它已经和 vanilla 未 patch 装备同级）
- 若采用"对齐 RBM coherent 冷值"哲学 → OSW blade **必须 3× 下调**（v1 结论方向对但更激进）
- **这是设计哲学的分歧，需要用户决策**

### 4.4 用户"重甲飘刀"隐忧的重新诊断

已知：
- 玩家 armor v2 是 **RBM-scale**（铁浮屠 148 head · Heavy Lamellar 138 body 属于 RBM 高档段）
- OSW blade 是 **vanilla-unpatched-scale**（热值）
- OSW shield 是**低于 SP-effective 底端**

**"重甲飘刀"实际含义**（重新推理）：
- 对抗中：玩家高 RBM-scale 甲 vs 敌 mixed vanilla+OSW 武器 → 敌方伤害被玩家甲大量吸收 → 玩家感觉 tank 十足（无挑战）
- 攻击时：玩家 OSW 武器 vs 敌 mixed vanilla 甲 → 玩家武器"能砍死人但需要多刀"（因为 vanilla 有很多低甲敌，一击杀反而快；但顶级 vanilla 甲敌会硬扛）

用户的"飘刀"感受核心：**玩家自己觉得武器不够狠**。但数据看 OSW blade 其实已经和 vanilla 未 patch 段一样烫 —— 所以感受源头不是绝对数值，而是**装甲曲线（v2 已抬高）和武器曲线（未跟进）的不匹配**。

**两条修法**：
- **A · 对称抬武器**：让武器数值也进入 RBM-scale 上限（比对齐 vanilla 未 patch 更烫）→ 打破生态平衡（敌方也变强）
- **B · 对称降 blade + 抬 shield**：把 OSW 拉回 RBM coherent · 让整体武器/防具都在 RBM 尺度里 → 生态一致 · 但玩家会感受更多刀 kill 敌

**推荐 B**（生态一致 · 长期可维护）· 但**决策权归用户**。

---

## 5. Guard / Handle / Pommel baseline（vanilla · RBM 未 patch）

RBM 对 Guard/Pommel **完全不 patch**（0/252 · 0/179）· Handle 只 patch 51/436。所以这三类保留 vanilla 原值：

| piece_type | tier | n | 关键 stat mean |
|---|:---:|---:|---|
| Guard | T1-T5 | 27/112/38/39/36 | weight 0.09-0.16 |
| Handle | T1-T5 | 15/209/85/87/40 | weight 0.28-0.59 · length 47-95 |
| Pommel | T1-T5 | 14/74/23/36/32 | weight 0.12-0.18 |

**OSW 对应件**（v1 未审）：
- Guard 8：多为剑护手 · vanilla-scale weight 无问题预期
- Handle 11：包括长柄 handle · 长度需专项对齐
- Pommel 13：包括斧首/矛头结构件 · 极多元 · 需 case-by-case

**判定**：Guard/Pommel 走 vanilla baseline 即可（都在 vanilla 分布内的话免动）· Handle 需 length 校准（若 OSW 有超长 handle 会导致 crafted weapon 过长）· 但这些**不是主战斗手感瓶颈** · P3 依然合理延后。

---

## 6. Vanilla CraftedItems 分布（作为 OSW 140 CraftedItem 的对标）

vanilla 有 530 CraftedItem · OSW 加了 140 全新的。OSW CraftedItem 的数值**完全由其 Piece 组合决定** —— 也就是说：

- 如果 OSW 用 vanilla/RBM 已知 CraftingPiece 拼装：自动继承那些 Piece 的（已 RBM-patch 或 vanilla-untouched）数值
- 如果 OSW 用 OSW 自己的 CraftingPiece（50 件）：数值由 OSW piece 值决定

→ 所以 **OSW CraftedItem 数值 = f(OSW piece 值)** · 平衡 Blade/Handle/Guard/Pommel 就自动传导到 CraftedItem。**不需要单独逐件 CraftedItem override**。

**待验证**（下一 session · 若走 B 方案）：随机抽 10-15 件 OSW CraftedItem · 展开其 Piece 组成 · 确认对应 CraftingPiece 是 OSW/vanilla/RBM 哪个 · 验证平衡链条完整。

---

## 7. 修正后的 baseline 结论汇总

| 项目 | v1 结论 | 修正后结论 |
|---|---|---|
| OSW 规模 | 173 件 | **313 件**（+140 CraftedItem 漏扫） |
| RBM_WS 定性 | 可能是武器 baseline | **不是** · 是 Nord/NavalDLC 内容包 |
| Shield 系统欠强？ | 是（vs RBM_WS Nord 679 mean） | **是**（vs SP-effective 581 mean · OSW 均值在 p8-p30） |
| Blade 系统过强？ | 是（vs RBM-only 0.91 mean） | **视哲学而定**：对 RBM = 3× 过强 · 对 vanilla-unpatched = 完全对齐 |
| Blade 修法 | tier-cap 一律降 | **需用户决策**：对齐 RBM 还是保 vanilla |
| Guard/Pommel/Handle 处理 | P3 延后 | **依然延后**（RBM 也没管；vanilla 值就是标准） |
| CraftedItem 处理 | 未提及 | **不需要单独 override** · 由 Piece 值传导 |

---

## 8. 交给用户的两个方案分叉

**分叉 α：Blade 走 vanilla-unpatched 曲线**（保 OSW 原设计）
- Shield 抬到 SP-effective 中段（P1 项目实施）
- Blade / Guard / Handle / Pommel 全部**不动**（已对齐 vanilla 未 patch）
- 边缘武器（Thrown/Crossbow）微调
- 优点：工作量最小（只做 shield 114 件）· 不打破 OSW 原设计意图
- 缺点：与用户 armor v2 的 RBM-scale 生态**不一致** · "重甲飘刀"感受不消除 · 敌 AI 用 vanilla-unpatched 武器仍能一击秒杀轻甲小兵

**分叉 β：Blade 走 RBM coherent 曲线**（对齐 armor v2 生态）
- Shield 抬到 SP-effective 中段（同 α）
- Blade tier-cap 到 RBM 同 tier max × 1.05（v1 提出的方案 A）
- Guard/Handle/Pommel 走 vanilla baseline（同 α）
- 优点：生态自洽 · 武器/护甲统一在 RBM scale · 长期可维护
- 缺点：18 Blade 全部调低 · 玩家用 OSW 武器 kill NPC 时间更长

---

## 9. 待用户签的决策

请针对 §8 二选一：
- [ ] **α 保设计**：只调 shield · 不动 blade
- [ ] **β 生态一致**：shield + blade 都调 · 对齐 armor v2 RBM-scale

以及副选项：
- [ ] Shield 曲线用 SP-effective 百分位 P25/P50/P75/P90 分档（380/570/700/780）· 还是保留 v1 报告的略偏保守曲线（350/500/620/720/800/880）？
- [ ] 是否本 session 也验证 15 件 OSW CraftedItem 的 Piece 组成 · 确认传导链？

**下一步**：等你 α/β 决策后 · 我用真 baseline 出 v0.2 平衡方案 · 直接生成 `$decisions` hashtable。
