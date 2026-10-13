# Phase 0.B 完整报告 · OSAIds 清单精炼

**日期**: 2026-10-13（主力机 CBTB 攻坚中 · 本机独立推进）
**目标**: 把 Phase 0.A smoke-test 的 15 件 safe subset 扩展到 production list
**产物**: `OpenSourceArmouryRBMBalance/src/OSAIds.cs` v2（POM 187 / PBS 21 / CFM 80 Tier A OSA items）
**build 验证**: `dotnet build -c Release` 0 warning / 0 error · DLL 从 8192 → 22016 bytes

---

## 1 · 工作流

### 1.1 · 从 vanilla ID 反推真语义

```
POM (20):  'scale' 10 次 · 'lamellar' 8 次 · 共覆盖 19/20  → 本质 = scale OR lamellar
PBS  (9):  'plated_leather' 4 · 'fortified' 2 · 'heavy_armor' 1 · 'raider_armor' 1
            + 1 件 western_scale_mail 跟 POM 重叠（scale 肩 · plate 身下）
CFM (16):  'closed' 7 · 'goggled' 4 · 显式 'mail/aventail' 词只 3 件 → vanilla CFM 的
            mail 保护常是 implicit · 5 件单一 cue (nordic/vendel/lendman/faceguard/decorated_goggled)
```

### 1.2 · 严格 reclassifier（v1 → v4 迭代）

迭代历史（每轮解决一个盲点）:

| 版本 | 关键改动 | POM / PBS / CFM Tier A |
|---|---|---|
| **v1** | 启发式 cue + 单层分桶 | 148 / 11 / 142（Phase 0 的 PHASE0_RECON.md）|
| **v2** | 严格 cue + Tier A/B 分级 + encode utf-8 | 172 / 0 / 8 |
| **v3** | POM 加 "over mail" / "reticulated plate" + CFM `\bmail\b` word-boundary + 'Open' 反向排除 | 187 / 21 / 68 |
| **v4** | CFM 加 `\blamellar\b` / `\bvendel\b` / `\bcoif\b`（lamellar faceguard 等同 mail aventail）| 187 / 21 / **80** |

### 1.3 · 工具链

```
_scratch/OSA_phase0/
├── reclassify_strict.py       v4 分类器 (POM/PBS/CFM Tier A + Tier B)
├── generate_osaids_cs.py      Tier A → OSAIds.cs 生成器
├── all_items.json             1648 件 OSA Item flat index (Phase 0 侦查产物)
├── strict_report.md           v4 run 输出报告
├── strict_pom.tierA.json      187 件 POM production
├── strict_pbs.tierA.json      21 件 PBS production
├── strict_cfm.tierA.json      80 件 CFM production
├── strict_pbs.tierB.json      1 件待审 (AR_khuzait_armor_a)
└── strict_cfm.tierB.json      62 件待审
```

---

## 2 · 产出清单（Tier A production · 已入 OSAIds.cs）

### 2.1 · 计数总览

| 白名单 | Vanilla | Tier A (OSA) | 合计（RBM 看到的）| 增幅 |
|---|---|---|---|---|
| **PlateOverMail (POM)** | 20 | **187** | 207 | +935% |
| **PlateBelowShoulder (PBS)** | 9 | **21** | 30 | +233% |
| **ClosedFaceMail (CFM)** | 16 | **80** | 96 | +500% |
| **总计** | 45 | **288** | 333 | +640% |

### 2.2 · POM 分布（文化）

```
Culture.aserai:    49 件  (Southern 系列 · 多板鳞 + 板链)
Culture.empire:    39 件  (Imperial 系列 · 多片甲 + lamellar vest + 罗马 segmentata)
Culture.vlandia:   28 件  (Western 系列 · reticulated plate over mail)
Culture.sturgia:   26 件  (Northern 系列 · scale over mail + plates over gambeson)
Culture.battania:  24 件  (Highland 系列 · scale over tunic with kilt)
Culture.khuzait:   21 件  (Eastern 系列 · lamellar + durkhan)
```

### 2.3 · PBS 分布

```
Culture.battania:  13 件  (warlord_armor_b/b2/c + 其他 heavy plate chest)
Culture.empire:     8 件  (罗马 lorica 系列 · 加 1 件 khuzait Plated Armor)
```

**偏科原因**: PBS cue 匹配比较窄 · 多数 OSA Plate 身甲是 scale/lamellar 类（走 POM）· 真正的"重 plate 肩下也是 plate"的款式较少。符合 vanilla 本来的 9 件也偏少的事实。

### 2.4 · CFM 分布

```
Culture.vlandia:  35 件  (Western 系列最多 · 骑士武备）
Culture.sturgia:  14 件  (Northern vendel 系列 + closed cavalry)
Culture.khuzait:  11 件  (Eastern lamellar faceguard 系列)
Culture.empire:   10 件  (Imperial closed guarded + cataphract)
Culture.aserai:   10 件  (Darshi + closed mail nasal)
```

---

## 3 · Tier B 未处理 · 待下一 session 人工审

### 3.1 · PBS Tier B (1 件)

| id | body | arm | leg | culture | name |
|---|---|---|---|---|---|
| `AR_khuzait_armor_a` | 45 | 16 | 17 | khuzait | Eastern Plated Armor |

**分析**: 名字 "Plated Armor" 符合 PBS 语义 · 但 "plated" 单字没触发 `plated_leather` compound cue。body=45 偏高 · 应该升到 Tier A。可下一轮 reclassifier 加宽 cue。

### 3.2 · CFM Tier B (62 件 · 需人工审)

归档: `_scratch/OSA_phase0/strict_cfm.tierB.json`

**分析分组**:
- 🟢 **可能是真 CFM（看模型实际有 mail aventail）**: Highland/Imperial 系列 "With Faceguard" · head_armor 30+ · 名字强 cue 但无 mail 词：
  - `AR_battania_helmet_j/k/n/o/v/v2` (Highland Steel/Bronze With Faceguard)
  - `AR_empire_crested_helm_a` / `AR_empire_lord_helmet_a`
  - `AR_vlandia_lord_helmet_c` (Goggled Bandedhelm)
- 🟡 **边界 case · head_armor 低 · 可能是轻盔**: Southern Closed Archer Helmet (head=21) · Spiked Helmet (head=23)
- 🔴 **明确 Open · 不该归 CFM**: `AR_battania_helmet_l/m` (Highland **Open** Steel/Bronze With Faceguard) · 已被 CFM_OPEN_ANTI 拦下 Tier B · 下 session 应 reject 到 head_rejected

**建议后续**:
- 用户下 session 开实际游戏 · 用 Retinues editor 预览每件 Tier B 头盔模型 · 眼见是否有 mail aventail
- 批量通过的加入 OSAIds.cs CFM 数组
- 明确开面的从 Tier B 剔除

### 3.3 · Rejected (理由记录)

- **Plate BodyArmor rejected 10 件**: material_type=Plate 但 body_armor 偏低 + 无 cue · 可能是 OSA 作者错标 material_type（本该 Leather/Chainmail）· 这些就算入 POM/PBS 也没意义（RBM 检查 MaterialType==Plate 才查白名单）· 下一轮 Phase 1 MaterialType audit 一起解决
- **HeadArmor rejected 701 件**: 多数是开面/半盔/头巾 · 不符合 CFM 语义 · 正确

---

## 4 · 本机 build + deploy 验证

**铁律 1 核对**:

```
Build: dotnet build -c Release
Build succeeded. 0 Warning(s). 0 Error(s). Time Elapsed 00:00:00.91
产物: src/bin/Release/OSABalanceRBMBridge.dll (22016 bytes, 从 8192 增长 ~2.7×)

Deploy: deploy.ps1 (auto D:/E: detect)
目标: D:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\OpenSourceArmouryRBMBalance\
文件时间戳: 2026-10-13 06:07 (匹配 build 时间)
  - SubModule.xml ✓
  - bin\Win64_Shipping_Client\OSABalanceRBMBridge.dll (22016 bytes) ✓
  - bin\Win64_Shipping_Client\OSABalanceRBMBridge.pdb (3152 bytes) ✓
  - ModuleData\OSABalance_*.xml x4 ✓
```

本机不能跑游戏（Steam single-session · 主力机在用 CBTB）· **smoke test 等主力机 CBTB session 结束后由主力机执行**。预期主菜单消息（现在 300 件版）:

```
OSABalance RBMBridge: PBS 9->30, CFM 16->96, POM 20->207
```

---

## 5 · 风险与限制

### 5.1 · 可能的误报

| 情况 | 影响 | 对策 |
|---|---|---|
| OSA Plate 身甲被错分到 POM · 其实是 PBS | 肩部走 Chainmail 曲线（本应 Plate）· 肩部**过弱** 10-20% | Phase 0.B v5 迭代 · 或在 Phase 5 实游戏观察后手动调整 |
| OSA Plate 身甲被错分到 PBS · 其实是 POM | 肩下走 Plate 曲线（本应 Chainmail）· 肩下**过硬** 10-20% | 同上 |
| CFM Tier A 的 "With Lamellar Faceguard" 实际模型没 lamellar | 面部命中吃 mail 保护 · 本应吃无甲伤害 · **过强** | 下 session 可视化复核 Tier A · 一般 lamellar faceguard mesh 都真实存在 · 风险低 |

### 5.2 · 不处理的东西

- **MaterialType audit** · Phase 1 工作 · 跟 B 正交并发
- **字段完整性 audit** · Phase 1 工作
- **OSW blade damage_factor** · Phase 3 可选项
- **主力机实游戏数字校准** · Phase 5

---

## 6 · 下一步

**主力机 CBTB 结束后立刻可做**:
1. `git pull && .\OpenSourceArmouryRBMBalance\deploy.ps1 -GameRoot "E:\..."` （已有本次产物）
2. 启游戏主菜单看绿字 `PBS 9->30, CFM 16->96, POM 20->207`
3. 开 custom battle · 装 OSA 板鳞/闭面盔 · 观察被打肩/面的实际反馈

**本机下一步建议**:
- **Phase 0.B v5**（可选 · 1 h）: 加 `plated` 单词 cue 把 `AR_khuzait_armor_a` 捞回 PBS Tier A · 跑 regression
- **Phase 1 字段 audit**（跟 CBTB 完全正交）· Journal §3040+ 开工锚点 · 2-3 session

**Phase 2 (deploy 1835 决议到 override XML) 阻塞于 Phase 1 完成**。
