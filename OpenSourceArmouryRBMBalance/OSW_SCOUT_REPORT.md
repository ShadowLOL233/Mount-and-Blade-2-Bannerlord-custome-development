# OSW v2 侦查报告（2026-09-25）

**目的**：OSA armor v2 已 2026-09-24 收官（1835 决议 + 323 XML deploy）。OSW（Open Source Weaponry, workshop id `3010984416`）从未纳入 v2 手工审。本报告扫全 OSW 数据 + 对比 RBM/RBM_WS baseline，判定项目实际规模。

**数据源**：
- OSW：`OSA_Reference/data/osa_items.csv` (source_mod=OSW) + `osa_crafting_pieces.csv` (source_mod=OSW)
- RBM：`RBM_Reference/data/rbm_items.csv` (source_mod=RBM) + `rbm_crafting_pieces.csv`
- RBM_WS：`RBM_Reference/data/rbm_items.csv` (source_mod=RBM_WS)

---

## 1. OSW 数据规模复核

| Type | 件数 | 备注 |
|---|---:|---|
| Shield | **114** | 全部 `weapon_class=LargeShield` |
| Thrown | 7 | 全部 `weapon_class=Stone`（罗马 plumbata / martiobarbuli / dart / naptha pot） |
| Crossbow | 1 | AR_cheirosiphon_a（Byzantine 火焰喷射器，非常规） |
| Bolts | 1 | cheirosiphon 弹药 |
| **Items 小计** | **123** | 比上次记录的 124 少 1（可能记忆偏差） |
| CraftingPiece Blade | 18 | v1 已处理 damage_factor |
| CraftingPiece Pommel | 13 | v1 未处理 |
| CraftingPiece Handle | 11 | v1 未处理 |
| CraftingPiece Guard | 8 | v1 未处理 |
| **CraftingPieces 小计** | **50** | 32 件（Pommel/Handle/Guard）v1 完全未审 |
| **总计** | **173** | 与 P0 立项记录 174 基本一致 |

**Culture 分布**（Items）：
- Empire 35 · Aserai 15 · Battania 13 · Khuzait 12 · Sturgia 8 · Vlandia 2 · **无 culture 38**
- Nord = **0**（→ Nord NavalDLC 不在 OSW 覆盖内，铁律不受影响）

**Culture 分布**（CraftingPieces）：31/50 无 culture 标签（跨文化通用件）。

---

## 2. RBM_WS baseline 定性（关键更正）

**上一对话立项时的假设**：RBM_WS 可能"充分覆盖 OSW 盾牌"，若如此则项目缩为 41 件。

**实测反证**：**RBM_WS 不是通用武器 baseline** —— 它是 Nord/NavalDLC 内容包（113 armor + 28 weapons，culture=nord 83 件）。构成：

| Type | 件数 |
|---|---:|
| HeadArmor | 34 |
| BodyArmor | 28 |
| Cape | 20 |
| Thrown | 16 |
| Shield | **9** |
| LegArmor | 9 |
| HandArmor | 9 |
| Arrows | 4 |
| Bolts | 2 |
| Bow | 1 |

**id 交集**：
- OSW ∩ RBM_WS = **0**
- OSW ∩ RBM (main) = **0**

OSW 用 `AR_*` / `AD_*` 全新 id（未占用 vanilla/RBM 命名空间）→ **无自动继承任何 RBM 平衡**，全部裸原厂数值上桌。

---

## 3. imbalance 严重程度（stat 分布对比）

### 3.1 Shield（114 件 vs RBM 60 LargeShield + RBM_WS 9 Nord）

| 指标 | OSW 114 | RBM main LargeShield 60 | RBM_WS Nord 9 |
|---|---:|---:|---:|
| hit_points min | 220 | 345 | 490 |
| hit_points max | **520** | 800 | 840 |
| hit_points mean | **346** | 572 | 679 |
| body_armor min | 1 | 1 | 4 |
| body_armor max | 7 | 9 | 8 |
| body_armor mean | **2.7** | 4.6 | 5.9 |
| speed_rating range | 82–89（近乎压平） | — | 82–88 |

**读数**：
- OSW 顶配盾 520 hp ≈ RBM tier 2（490 hp）—— **OSW 的"最好"约等于 RBM 的"垃圾"**
- OSW 均值 346 hp = RBM 均值 60%、RBM_WS 均值 51%
- 25 件 OSW 盾（21%）的 body_armor = 1 —— RBM 战斗下几乎不挡箭
- 5 件最弱：`AR_shield_infantry_l/l2/zg/zh/zi`（220–240 hp，body_armor 1–4）—— 明显 T1 素材，但等距于 vanilla T1

**结论**：**系统性欠强**。玩家用 OSW 顶配盾对上 RBM 装备的 AI，盾在数击内就碎。

### 3.2 CraftingPiece Blade（18 件 vs RBM 256 Blade）

| 指标 | OSW 18 Blade（13 有 swing） | RBM 256 Blade |
|---|---:|---:|
| swing damage_factor min | 1.6 | 0.2 |
| swing damage_factor max | 3.9 | 1.7 |
| swing damage_factor mean | **2.80** | **0.91** |

**读数**：**OSW blades 比 RBM blades 强约 3 倍**。v1 已处理但强度仍远超 RBM curve —— 说明 v1 的处理不够激进（或没往 RBM 方向对齐）。

**这一段与 3.1 组合起来的画面**：OSW 的**盾偏弱、刀偏强**，正好呼应用户对当前装备生态"重甲飘刀"的隐忧 —— 不仅是武器整体欠强，而是**刀过强 + 盾过弱**的双向失衡。

### 3.3 CraftingPiece Guard/Handle/Pommel（32 件）

v1 完全未审。这些字段影响 crafted weapon 的 speed/weight/handling/length 平衡：
- Pommel 13：影响 weight 与 handle_offset
- Handle 11：影响 grip 长度与 weight
- Guard 8：影响 hilt weight 与 blade attachment

**未有 RBM baseline 对比**（需单独扫 RBM_Reference 的 CraftingPiece 字段），但因为**下游 crafted weapon 依赖它们**，若不校准这 32 件、只调 Blade damage 会顾此失彼。

### 3.4 边缘武器（9 件）

- **Thrown 7**：全部 `weapon_class=Stone`（技术上是"投石"类，会跳过 RBM 投掷 patch 的部分逻辑）。damage 50–60 Pierce（napa pot 200 Blunt）。这些是罗马 plumbata / martiobarbuli / war dart / naptha pot —— **`Stone` 分类可能是原作者的偷懒或有意规避**，需 case-by-case 判定
- **Crossbow 1**：cheirosiphon（Byzantine 希腊火喷射器）—— 完全非常规，独立平衡
- **Bolts 1**：cheirosiphon 专用弹药

**优先级低** —— 9 件都是异形武器，不影响主战斗循环。

---

## 4. 项目规模判定（P0 立项要回答的问题）

**先决判断**（立项文档原文）：
> 如果 RBM_WS 已经充分覆盖 OSW 盾牌 · 那本项目可能只需处理 CraftingPieces 32 件深化 + 9 件边缘武器（工作量大幅缩小）；反之则走全 174 件。

**答案**：走 **全 173 件**。理由：
1. RBM_WS id 交集 = 0，**零覆盖**
2. Shield 分布证明 OSW 盾整体欠强，非个别校准问题
3. Blade damage_factor 呈现 3× 过强，方向与 Shield 相反 —— **是不可分割的两侧平衡问题**
4. 若只调 Guard/Handle/Pommel + 边缘武器（41 件），最大痛点（盾脆 + 刀猛）**完全没解决**

---

## 5. 推荐 6 阶段推进顺序

按 impact 排序（重要性递减）：

| 阶段 | 内容 | 件数 | 预计 session |
|---|---|---:|---:|
| **P1** | Shield 全审（分档 T1/T2/T3/T4/T5/T6，参考 RBM LargeShield 曲线 345→800 hp） | 114 | 2 |
| **P2** | Blade 重审（v1 → v2，往 RBM 曲线拉齐，重点降 damage_factor 上限） | 18 | 0.5 |
| **P3** | Guard/Handle/Pommel 审（补齐 v1 缺口，配合 Blade 定型 crafted weapon 平衡） | 32 | 1 |
| **P4** | Thrown/Crossbow/Bolts 边缘武器（case-by-case） | 9 | 0.5 |
| **P5** | 世界观精工线联动（Khuzait 华夏 / Aserai 波斯 / —— OSW 无 Nord，Nord 精英维京武器留给未来 mod） | — | 0.5 |
| **P6** | Deploy（`manual_override.ps1` pipeline 加武器决议） | — | 0.5 |

**总估**：4.5–5 session（略高于立项 3–5 session 估计的中位）。

**Pipeline 复用**（armor v2 遗产）：
- `manual_override.ps1` $decisions hashtable：追加 `id → 数值 map` 即可
- BALANCE_V2_LOG.md：追加 `## OSW Weapon Balance` 一级章节
- 铁律不变：vanilla + RBM 唯一权威，用户特批 override
- 无 Nord NavalDLC 陷阱（OSW 未覆盖 nord）

---

## 6. 关键新发现（建议入日志）

1. **RBM_WS ≠ 通用武器 baseline**，是 Nord/NavalDLC 内容包 —— 修正上一对话立项时的假设
2. **OSW 盾系统性欠强 + 刀系统性过强**，双向失衡而非单侧
3. **OSW 与 vanilla/RBM 命名空间零交集**，`AR_*`/`AD_*` 前缀新建，故不影响 vanilla override 机制
4. **无 Nord 冲突**：OSW 0 件 nord，铁律"Nord NavalDLC 陷阱"在 OSW 项目下不生效

---

## 7. 下一步（等用户确认后开工）

**需要用户决策**：
- [ ] 确认走 **全 173 件全量平衡**（推荐）
- [ ] 若同意，第一 session 从 **P1 Shield 分档**开始（先给 6 tier 曲线草案 + 分档标准，用户批准后 batch 生成决议）

**若用户暂不开工**：本报告归档于 `OpenSourceArmouryRBMBalance/OSW_SCOUT_REPORT.md`，随时可回。
