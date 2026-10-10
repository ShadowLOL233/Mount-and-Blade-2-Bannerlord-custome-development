# OSW v2 平衡方案（v0.2 · 2026-09-25 · 分叉 α "只抬盾"）

**取代**：`OSW_BALANCE_PROPOSAL.md` v0.1（基于错误 baseline）
**基于**：`OSW_BASELINE_SCAN.md`（完整 vanilla + RBM baseline · 2026-09-25）
**用户决策**：α · 只抬盾 · blade/CraftingPiece/CraftedItem/edge 一件不动

---

## 0. 本方案范围

**做**：114 OSW 盾牌 hp / body_armor / speed / weight 平衡
**不做**：
- ❌ 18 Blade（保 OSW 原设计 · 对齐 vanilla-unpatched 段）
- ❌ 32 Guard / Handle / Pommel（vanilla 已 patch 或 vanilla-scale · OSW 未大幅出格）
- ❌ 9 边缘武器（Thrown/Crossbow/Bolts · exotic · 微调价值低）
- ❌ 140 CraftedItem（数值由 Piece 传导 · 不动 Piece 就不动 CraftedItem）

**依据**：blade 现有数值精准对齐 vanilla 未 patch 段 · nerf 会破坏 OSW 原设计意图 · 抬盾单向修复系统性欠强就够。

---

## 1. Shield 平衡逻辑

### 1.1 SP-effective baseline 目标曲线（OSW 需匹配）

| Baseline 分位 | HP | ba | 说明 |
|---|---:|---:|---|
| p10 | 380 | 3 | 底端（初级 fief 兵） |
| p25 | 480 | 4 | 常见步兵 |
| p50 | 570 | 5 | 中位（主流精兵） |
| p75 | 700 | 6 | 精英 |
| p90 | 750 | 7 | 稀有 |
| p95+ | 800 | 7-8 | 顶配 |

**基准 tier 曲线**（贴合上表 · **推荐**）：

| Tier | HP | 对应分位 |
|:---:|---:|:---:|
| T1 | 380 | p10 |
| T2 | 480 | p25 |
| T3 | 570 | p50 |
| T4 | 700 | p75 |
| T5 | 750 | p90 |
| T6 | 800 | p95+ |

**说明**：不再刻意让 OSW 顶配（880）超 vanilla 上限（840）· T6 定在 800 = "OSW 顶配 ≈ vanilla 精英"，不越权。

### 1.2 双正交轴 · 保留 OSW 原设计意图

**轴 A · 重量决定尺寸/形态**：

| Class | Weight range | 件数 | HP × | ba 修正 | speed 修正 |
|---|---:|---:|---:|---:|---:|
| Buckler | ≤ 2.9 kg | 7 | ×0.85 | ba **硬顶 2** | +2 |
| Round Light | 2.9–4.2 kg | 37 | ×0.95 | 0 | +1 |
| Round Standard | 4.2–5.5 kg | 49 | ×1.00 | 0 | 0 |
| Heavy Scutum | > 5.5 kg | 21 | ×1.10 | +1 | −1 |

**轴 B · 现有 ba 决定材质**（保 OSW 木/加固设计）：

| Current ba | 分类 | 件数 | 新 ba 曲线 |
|:---:|---|---:|---|
| ≤ 2 | Wood | 80 | T1=1 · T2=2 · T3=3 · T4=3 · T5=4 · T6=4 |
| ≥ 3 | Reinforced | 34 | T1=3 · T2=4 · T3=5 · T4=6 · T5=7 · T6=8 |

**wood/reinforced 平衡性质**：
- Wood 全 tier 低于 baseline median ba(5) → "木盾易被箭穿"设计味道
- Reinforced 全 tier 贴近或超过 baseline median → "金属加固挡箭好"
- OSW 玩家可按需选 —— 装重甲用 wood 盾（快回旋）· 轻甲用 reinforced 盾（补挡箭）

### 1.3 hp bucket → tier 自动分派

按 OSW 现有 hp 分档（保留 OSW 相对序）：

| Current hp | → Tier | 件数 |
|---|:---:|---:|
| ≤ 260 | T1 | 17 |
| 280–320 | T2 | 29 |
| 340–380 | T3 | 36 |
| 400–440 | T4 | 21 |
| 460–500 | T5 | 8 |
| 520 | T6 | 3 |

**分布形状**：金字塔（T3 波峰 · T6 稀缺）—— 符合装备生态直觉。

### 1.4 speed_rating

**baseline vanilla LargeShield speed 分布**：mean ~84 · range 82-90
**OSW 现状**：全部 82-89 · 近乎压平

**新规则**：`speed = 84 + weight_speed_offset`（不区分 tier · 只按 weight 修正）
- Buckler → 86
- Round Light → 85
- Round Standard → 84
- Heavy Scutum → 83

### 1.5 weight

**不改** —— OSW 原 weight 值已合理（分布 2.5–7.3kg 均落在 baseline 分布内）· 改 weight 会同步影响 mesh 视觉/物理 · 高风险低收益。

---

## 2. 完整分组预览（114 → 25 非空组）

按 (Tier × WeightClass × Material) 三键交叉分组：

| 组名 | 件数 | 新 HP | 新 ba | 新 spd |
|---|---:|---:|---:|---:|
| T1 · HeavyScutum · Reinforced | 3 | 418 | 4 | 83 |
| T1 · HeavyScutum · Wood | 4 | 418 | 1 | 83 |
| T1 · RoundLight · Wood | 1 | 361 | 1 | 85 |
| T1 · RoundStd · Wood | 9 | 380 | 1 | 84 |
| T2 · Buckler · Wood | 2 | 408 | 1 | 86 |
| T2 · HeavyScutum · Wood | 5 | 528 | 2 | 83 |
| T2 · RoundLight · Reinforced | 2 | 456 | 4 | 85 |
| T2 · RoundLight · Wood | 8 | 456 | 2 | 85 |
| T2 · RoundStd · Reinforced | 1 | 480 | 4 | 84 |
| T2 · RoundStd · Wood | 11 | 480 | 2 | 84 |
| T3 · Buckler · Wood | 5 | 485 | 2 | 86 |
| T3 · HeavyScutum · Reinforced | 2 | 627 | 6 | 83 |
| T3 · HeavyScutum · Wood | 2 | 627 | 3 | 83 |
| T3 · RoundLight · Reinforced | 4 | 542 | 5 | 85 |
| T3 · RoundLight · Wood | 1 | 542 | 3 | 85 |
| T3 · RoundStd · Reinforced | 8 | 570 | 5 | 84 |
| T3 · RoundStd · Wood | 14 | 570 | 3 | 84 |
| T4 · HeavyScutum · Reinforced | 2 | 770 | 7 | 83 |
| T4 · HeavyScutum · Wood | 3 | 770 | 3 | 83 |
| T4 · RoundLight · Reinforced | 6 | 665 | 6 | 85 |
| T4 · RoundLight · Wood | 5 | 665 | 3 | 85 |
| T4 · RoundStd · Reinforced | 5 | 700 | 6 | 84 |
| T5 · RoundLight · Wood | 7 | 713 | 4 | 85 |
| T5 · RoundStd · Reinforced | 1 | 750 | 7 | 84 |
| T6 · RoundLight · Wood | 3 | 760 | 4 | 85 |

**验证点**（用户批准后我用实数复算）：
- T3 · RoundStd · Wood（14 件 · 最大组）: 570 hp / ba=3 / spd=84 · 全组同数值 → 检查合理
- T4 · HeavyScutum · Reinforced（2 件）: 770 hp / ba=7 / spd=83 → 顶级重型加固塔盾 · 略优于 baseline p90
- T6 · RoundLight · Wood（3 件 · 顶轻盾）: 760 hp / ba=4 / spd=85 → 顶级木质轻盾 · 中位 ba · 保 "轻但挡箭差"味道

---

## 3. 3 件示例（before → after）

| ID | Before | After | 分类 |
|---|---|---|---|
| `AR_shield_infantry_a` | 340hp/ba=1/wt=4.7/spd=82 | **570 hp / ba=3 / spd=84** | T3 · RoundStd · Wood |
| `AR_shield_infantry_zc` | 520hp/ba=1/wt=4.0/spd=82 | **760 hp / ba=4 / spd=85** | T6 · RoundLight · Wood |
| `AR_shield_infantry_zg` | 220hp/ba=4/wt=7.3/spd=82 | **418 hp / ba=4 / spd=83** | T1 · HeavyScutum · Reinforced |

---

## 4. Pipeline

```powershell
# manual_override.ps1 追加
$decisions_shield = @{
    'AR_shield_infantry_a'  = @{ hit_points=570; body_armor=3; speed_rating=84 }
    'AR_shield_infantry_a2' = @{ hit_points=570; body_armor=3; speed_rating=84 }
    # ... 114 rows 自动生成
}
```

生成 XML override → 落 `OpenSourceArmouryRBMBalance/ModuleData/` → 用户 `deploy.ps1` 到主力机。

---

## 5. 待用户签的 4 项决策（比 v1 精简）

- [ ] **D1**：Tier hp 曲线 380 / 480 / 570 / 700 / 750 / 800（贴 SP-effective 百分位）· ✅走 or 改？
- [ ] **D2**：Weight 4 分类（Buckler ≤2.9 / RoundLight 2.9-4.2 / RoundStd 4.2-5.5 / HeavyScutum >5.5）+ hp×0.85/0.95/1.0/1.1 · ✅走 or 改边界？
- [ ] **D3**：Wood/Reinforced ba 双曲线（wood: 1/2/3/3/4/4 · reinforced: 3/4/5/6/7/8）· ✅走 or 调？
- [ ] **D4**：Speed 简化为 84 + weight 修正（不区分 tier）· ✅走 or 保留 tier 递增？

**副项（非阻塞）**：
- [ ] **D5**：本 session 一起归档到 `BALANCE_V2_LOG.md` 新章节 `## OSW Shield Balance` · ✅
- [ ] **D6**：世界观联动（Khuzait 华夏 / Aserai 波斯精工盾特批 T5/T6）· 本 session 一起做 or 后补？

---

## 6. 用户签完后我做的事

1. 生成完整 `$decisions_shield` hashtable（114 行）
2. 追加到 `scripts/manual_override.ps1`
3. 跑 pipeline · XML override 落 `ModuleData/`
4. 写 `BALANCE_V2_LOG.md > ## OSW Shield Balance` 章节 · 归档所有分类决策
5. 更新 `ModdingJournal.md` P0 章节 · 标记 P1 阶段完成
6. 汇报给用户 · 等 deploy 到主力机实测反馈
