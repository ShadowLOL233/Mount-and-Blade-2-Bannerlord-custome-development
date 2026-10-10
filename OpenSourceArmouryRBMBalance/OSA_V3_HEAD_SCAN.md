# OSA v3 跨文化头盔平衡 · Phase 1 扫描 + 决议清单

**生成日期**：2026-10-10
**scope**：HeadArmor（头盔）跨文化同材质同样式 variance 识别
**数据源**：`src/armor_data_head.json`（Vanilla + RBM + OSA 合并后 1328 件头盔）
**扫描工具**：`src/scan_head_cross_culture.ps1`

---

## 扫描方法

### 分组键 · (材质 × 样式签名 × tier)

**样式签名（style_sig）算法**：
1. 显示名小写化
2. **aventail 短语规范化**（整体识别避免拆词）：
   `over mail coif` → `OVERMAILCOIF` · `over mail` → `OVERMAIL` · `over leather` → `OVERLEATHER`
   `with mail` → `WITHMAIL` · `with faceguard` → `WITHFACEGUARD` 等共 13 种 pattern
3. **剥装饰词**（结构无影响 · 对应 BALANCE_V2 L295）：
   `plumed`, `crested`, `crowned`, `feathered`, `gilded`, `jeweled`, `southern`, `eastern`, `northern`,
   `bronze`, `iron`, `steel`, `golden`, `silver`, `noble`, `decorated`, `redcrest`, `lordly`,
   `ornate`, `fine`, `fancy`, `rough`, `simple`, `plain`, `old`, `new`, `worn`, `rusty`, `polished`
4. **剥文化前缀**：`imperial`, `khuzait`, `sturgian`, `vlandian`, `battanian`, `aserai`, `nordic` 等
5. **剥通用名词**：`helmet`, `helm`
6. 剩余 token 空格连接作签名

### 排除规则

- **mp_ 前缀物品**：multiplayer-only 版本（出于 MP 平衡故意压低数值 · 不参与 SP 跨文化比较）
- **neutral_culture / looters / (none) 文化**：没有有效文化归属
- **variance 阈值 > 5**：bucket flag 条件 · 跨文化均值差 ≤ 5 视为可接受偏差

### Tier 公式

```
raw    = 1.2·head + body + leg + arm
scaled = raw × 1.2            // HeadArmor typeMultiplier
tier   = clamp(round(scaled × 0.1 − 0.4), 0, 6) − 1
```

---

## 扫描结果概览

| 维度 | 数值 |
|---|---:|
| 总 bucket（mat × style × tier） | 914 |
| 跨文化 bucket（≥ 2 文化） | 21 |
| **Flagged（variance > 5）** | **9** |

**观察**：
- 914 个 bucket 中只有 21 个真正跨文化 · 说明**绝大多数头盔样式是文化专属**（Spangenhelm = Empire-only，Battle Crown = 4 文化共用，Nasal Helm over Mail = Empire/Sturgia/Vlandia/Nord 共用）
- 9 个 flagged bucket **全部在 Plate T5** · 说明问题集中在**高级板甲头盔**
- 其他 tier/材质跨文化基本已经同步（RBM + OSA v2 之前的平衡工作已覆盖）

---

## 🔴 高亮：数据异常候选

**`spiked_helmet_with_facemask`**（RBM · 挂 Khuzait 文化）数值异常：
- head=119, **body=90, arm=40**, leg=0, weight=4.5

**头盔 body=90 arm=40 不合理**。对比同族两件 OSA 版本 `TV_khuzait_lord_helmet_e` / `AR_vaegir_helmet_g`：都是 h=51 b=0 a=0。这件 RBM 数值疑似误把 BodyArmor 字段写到了 HeadArmor 条目里。

**待用户决议**：
- (A) 修复到正常头盔范围 · h=119 b=12 a=0（保 head 顶，zero 掉 body/arm 异常）
- (B) 对齐 OSA 两件兄弟 · h=51 b=0 a=0（统一为中档）
- (C) 保留当前值 · 作为 Khuzait 内部 override 特例（不推荐 · 跟 Khuzait 华夏精工线 overlapping 不清）

---

## 9 个 Flagged Bucket 详情 + 决议建议

### 排序：按 variance 降序

| # | 样式 | var | items | 跨文化范围 | 建议动作 |
|---|---|---:|---:|---|---|
| 1 | battle crown T5 | 81 | 4 | aserai 44 → sturgia 125 | 🟡 建议向中位 90-100 收敛 |
| 2 | cataphract T5 | 78 | 2 | khuzait 52 → empire 130 | 🟡 待用户判断：52 是否 Khuzait 华夏降档特例 |
| 3 | guard T5 | 60 | 2 | battania 63 → empire 123 | 🟢 **false positive**（"Noble Guard" vs "Old Guard" 不是同件装备）|
| 4 | (generic) T5 | 56 | 9 | 44 → 125 | 🟡 OSA 装饰变体欠 buff · 建议对齐 RBM baseline |
| 5 | cap T5 | 36 | 4 | khuzait 45 → aserai 100 | 🟢 **false positive**（Aserai 3 件是 tier 阶梯 · 不是同档）|
| 6 | spiked with facemask T5 | 34 | 3 | sturgia/khuzait 51 → RBM 119 | 🔴 **数据异常** · 见上方高亮 |
| 7 | nasal T5 | 26 | 3 | sturgia 49 → nord 75 | 🟡 Sturgia 2 件 OSA 欠 buff |
| 8 | closed warlord T5 | 14 | 2 | nord 104 → sturgia 118 | 🟢 轻度偏差 · 可忽略或微调 |
| 9 | nasalhelm OVERMAIL T5 | 13.5 | 3 | sturgia 75 → empire 90 | 🟡 Sturgia RBM 偏低 12-15 点 |

---

### Bucket 1 · `battle crown` T5 · variance=81

| Culture | H | B | A | L | Wt | Price | Src | Display Name | StringId |
|---|---:|---:|---:|---:|---:|---:|---|---|---|
| aserai | 44 | 0 | 0 | 0 | 2.8 | auto | OSA | Ornate Southern Battle Crown | `TV_aserai_lord_helmet_c` |
| empire | 104 | 12 | 0 | 0 | 2.1 | auto | RBM | Jeweled Plumed Battle Crown | `empire_battle_crown_north` |
| khuzait | 90 | 12 | 0 | 0 | 2.7 | auto | RBM | Khuzait Battle Crown | `khuzait_battle_crown` |
| sturgia | 125 | 12 | 0 | 0 | 2.2 | auto | RBM | Battle Crown | `sturgian_battle_crown` |

**诊断**：
- Sturgia 125 顶 · Empire 104 · Khuzait 90 · Aserai 44（OSA · 严重欠 buff）
- RBM 三件已经形成"维京风顶 → 帝国中 → 游牧次"的北-中-东梯度（105-125 窄 band）
- Aserai 作为"南方装饰型" · 游戏内视觉是花哨的轻装 crown · 44 跟实际视觉偏轻装匹配，但 tier 定在 T5 不合理

**选项**：
- **A · 统一到 100-105 中档**：sturgia 125→105 · empire 104→不变 · khuzait 90→100 · aserai 44→100 · 完全同质化
- **B · 保留梯度 · 只 buff Aserai**：sturgia 125 保 · empire 104 保 · khuzait 90 保 · aserai 44→**90**（跟 khuzait 对齐 · 保留北 → 东 → 南梯度）
- **C · 全部放到 115**：均值化到最高

**🟢 建议 B**：保留现有梯度 + 只 buff Aserai 到 90 · 符合用户 "保留文化差异 + 消除数值异常" 的 OSA v2 原则。

---

### Bucket 2 · `cataphract` T5 · variance=78

| Culture | H | B | A | L | Wt | Price | Src | Display Name | StringId |
|---|---:|---:|---:|---:|---:|---:|---|---|---|
| empire | 130 | 30 | 0 | 0 | 3.7 | auto | RBM | Royal Cataphract Helmet | `empire_guarded_lord_helmet` |
| khuzait | 52 | 0 | 0 | 0 | 3.9 | auto | OSA | Eastern Cataphract Helmet | `AR_khuzait_lord_helmet_b` |

**诊断**：
- Empire Royal Cataphract h=130 b=30 是 vanilla + RBM 的 "Cataphract 真顶"（journal §48 2026-09-24 定锚点）
- Khuzait Eastern Cataphract h=52 是 OSA 补的 variant · 装饰 Khuzait skin · 本质应该同档
- 命名含 "Cataphract" 应承 Cataphract 定位 · 当前 52 实际是 T3 水平

**待用户确认**：
- `AR_khuzait_lord_helmet_b` **是否属于 Khuzait 华夏精工线**（需保护不动）？
- 若是：52 是 user intent · 保留（但则命名误导）
- 若不是：应 buff 到 ≥120 · 可能数值 h=125, b=25, a=0, weight=3.8（略低于 Empire 以体现"东方精工 ≤ 帝国本阵"）

**🟢 建议 "等用户 1 分钟确认"** · 然后：
- 若非华夏线：buff 到 h=125 b=25 a=0 wt=3.8
- 若是华夏线：加 note "保留 by user override" 跳过 · 并考虑改名

---

### Bucket 3 · `guard` T5 · variance=60 · 🟢 false positive

| Culture | H | B | A | L | Wt | Price | Src | Display Name | StringId |
|---|---:|---:|---:|---:|---:|---:|---|---|---|
| battania | 63 | 0 | 0 | 0 | 2.5 | auto | RBM | Old Guard Helmet | `celtic_helmet` |
| empire | 123 | 10 | 0 | 0 | 3.5 | auto | RBM | Noble Guard Helmet | `empire_lord_helmet` |

**诊断**：
- "Noble Guard" ≠ "Old Guard" · 命名碰撞导致 style_sig 塌陷
- `empire_lord_helmet` 是 Cataphract 级 apex · 123 合理
- `celtic_helmet` ("Old Guard") 是 Celtic 老式装备 · 63 合理

**决议**：**跳过 · 命名误伤 · 保持原值**

---

### Bucket 4 · `(generic)` T5 · variance=56

| Culture | H | B | A | L | Wt | Price | Src | Display Name | StringId |
|---|---:|---:|---:|---:|---:|---:|---|---|---|
| empire | 125 | 14 | 20 | 0 | 3.8 | auto | RBM | Imperial Jeweled Helmet | `empire_jewelled_helmet` |
| empire | 104 | 24 | 0 | 0 | 2.9 | auto | RBM | Plumed Helmet | `plumed_helmet` |
| empire | 50 | 0 | 0 | 0 | 3.8 | auto | OSA | Imperial Feathered Jeweled Helmet | `AR_empire_lord_helmet_e` |
| empire | 48 | 0 | 0 | 0 | 3.1 | auto | OSA | Imperial Plumed Helmet | `empire_battle_crown_west` |
| empire | 48 | 0 | 0 | 0 | 3.9 | auto | OSA | Imperial Crowned Helmet | `AR_Imperial_Crowned_Helmet_a` |
| khuzait | 44 | 0 | 0 | 0 | 4.6 | auto | OSA | Eastern Lordly Plumed Decorated Helmet | `TV_khuzait_helmet_y4` |
| nord | 55 | 12 | 7 | 0 | 2 | auto | RBM | Nordic Iron Helmet | `nord_helmet_fa` |
| sturgia | 49 | 0 | 0 | 0 | 3.15 | auto | OSA | Northern Gilded Steel Helmet | `TV_sturgia_helmet_h` |
| vlandia | 100 | 10 | 0 | 0 | 3.7 | auto | RBM | Crowned Helmet | `western_crowned_helmet` |

**诊断**：
- (generic) 签名意味着 "剥光装饰词后啥也不剩" · 这个 bucket 把所有"纯 Imperial-Plumed-Crowned-类"的装饰变体混到一起
- RBM 三件（125/104/100）形成"顶 → 中 → 中"的 T5 真档
- **5 件 OSA 变体（48-55）严重欠 buff** · 应是 RBM baseline 的视觉 variant · 却数值掉到 T2-T3 水平

**选项 · 5 件 OSA 该怎么对齐**：
- **A · 全对齐到 100**：OSA 5 件统一到 h=100 b=10 · 匹配 Vlandia Crowned
- **B · 分文化对齐**：
  - 3 件 Imperial-* → h=104 b=14 匹配 `plumed_helmet`（empire 自家 RBM baseline）
  - khuzait Eastern Lordly → h=90 b=12 匹配 `khuzait_battle_crown`
  - sturgia Northern → h=100 b=12 匹配 Sturgia Battle Crown 的次档
- **C · 保留 OSA 低档**：OSA 变体定位为"轻装装饰盔" · 不 buff · 不同于 Cataphract 顶档

**🟢 建议 B**：按文化分别对齐自家 RBM baseline · 保持文化内 tier 分化。

---

### Bucket 5 · `cap` T5 · variance=36 · 🟢 false positive

| Culture | H | B | A | L | Wt | Price | Src | Display Name | StringId |
|---|---:|---:|---:|---:|---:|---:|---|---|---|
| aserai | 100 | 22 | 0 | 0 | 3.9 | auto | RBM | Lordly Steel Cap | `southern_lord_helmet` |
| aserai | 97 | 12 | 25 | 0 | 3.5 | auto | RBM | Noble Steel Cap | `southern_noble_helmet` |
| aserai | 46 | 12 | 0 | 0 | 1.2 | auto | RBM | Steel Cap | `desert_helmet` |
| khuzait | 45 | 0 | 0 | 0 | 1.4 | auto | RBM | Ornate Steel Cap | `khuzait_crown` |

**诊断**：
- Aserai 3 件 "Steel Cap" 实际是**3 个 tier 的梯度**（lordly 100 顶 → noble 97 次 → 基础 46）
- "lordly" / "noble" / 空 → style_sig 都变 `cap` · 塌陷到同桶
- Khuzait "Ornate Steel Cap" 46 跟 Aserai 基础版 46 对齐 · 没有真实跨文化差

**决议**：**跳过** · style_sig 把 tier 梯度塌到同桶导致 false positive。

**后续改进**：若需要 Phase 1.5 二次扫描 · 把 "lordly" 从装饰词挪到结构词（它影响 tier 梯度）· 或加 "head_armor bucket" 二级分 group。

---

### Bucket 6 · `spiked with facemask` T5 · variance=34 · 🔴 数据异常

| Culture | H | B | A | L | Wt | Price | Src | Display Name | StringId |
|---|---:|---:|---:|---:|---:|---:|---|---|---|
| khuzait | **119** | **90** | **40** | 0 | 4.5 | auto | **RBM** | Spiked Helmet With Facemask | `spiked_helmet_with_facemask` |
| khuzait | 51 | 0 | 0 | 0 | 4.5 | auto | OSA | Decorated Spiked Helmet With Facemask | `TV_khuzait_lord_helmet_e` |
| sturgia | 51 | 0 | 0 | 0 | 4.5 | auto | OSA | Northern Spiked Helmet With Decorated Facemask | `AR_vaegir_helmet_g` |

**诊断**：
- RBM `spiked_helmet_with_facemask` body=90 arm=40 → 头盔有 body=90 不合理（头盔 body 通常 0-30）
- 公式验证：raw = 1.2×119 + 90 + 40 = 272.8 · scaled = 272.8 × 1.2 = 327.4 · tier = round(32.74-0.4) clamp 6 - 1 = 5 → 虽然还是 T5 · 但 scaled 远超正常 T5 upper bound 200
- 疑似 RBM XML 数据录入错 · 把别的 slot 的值错填到 head armor 条目里

**选项**：
- **A · 对齐同族 OSA**：h=119→**51** b=90→**0** a=40→**0**（跟两个 OSA 兄弟对齐 · 作为"基础 Khuzait 面罩盔"）
- **B · 保 head 修 body/arm**：h=119 保留 · b=90→**12** a=40→**0**（相信 RBM 的 head 买意 · 清掉异常的 body/arm 污染 · tier 还是 T5）
- **C · 不动 · 作 Khuzait 内部 override 特例**（**不推荐** · body=90 在战场效果异常）

**🟢 建议 B**：相信 RBM 对 head 的意图（119 = Cataphract 级）· 清 body/arm 异常。

---

### Bucket 7 · `nasal` T5 · variance=26

| Culture | H | B | A | L | Wt | Price | Src | Display Name | StringId |
|---|---:|---:|---:|---:|---:|---:|---|---|---|
| nord | 75 | 12 | 7 | 0 | 2.2 | auto | RBM | Nordic Nasal Helmet | `nord_helmet_ea` |
| sturgia | 49 | 0 | 0 | 0 | 3.15 | auto | OSA | Northern Gilded Steel Nasal Helmet | `TV_sturgia_helmet_g` |
| sturgia | 49 | 0 | 0 | 0 | 1.8 | auto | OSA | Northern Feathered Nasal Helmet | `ao_sturgian_feathered_helmet` |

**诊断**：
- Nord & Sturgia 都是"北方文化" · 都用 Nasal Helm · 应近似
- Nord `nord_helmet_ea` h=75 b=12 a=7（RBM 中档档位 · "Over Padded Cloth" 风）
- Sturgia 2 件 OSA 版本 h=49 b=0 a=0 · 欠 buff 到 "no aventail" 档

**选项**：
- **A · 全对齐 Nord**：Sturgia 2 件 → h=75 b=12 a=7 wt=2.2
- **B · 保留 Sturgia 轻装定位**：Sturgia 2 件 → h=65 b=8 a=12（Cat 2 "With Mail" 内衬档位 · 比 Nord 弱一档但不至于 49）
- **C · 保留原值**：承认 Sturgia 这 2 件是 "Gilded/Feathered 装饰变体" · 跟 Nord 不同

**🟢 建议 B**：Sturgia 2 件 buff 到 Cat 2 "With Mail" 档 · 保留北方文化内次档位 · 比 Nord 稍轻符合 Sturgia 相对 Nord 的轻装定位。

---

### Bucket 8 · `closed warlord` T5 · variance=14

| Culture | H | B | A | L | Wt | Price | Src | Display Name | StringId |
|---|---:|---:|---:|---:|---:|---:|---|---|---|
| nord | 104 | 64 | 40 | 0 | 4.1 | auto | RBM | Northern Closed Warlord Helmet | `closed_goggled_helmet` |
| sturgia | 118 | 82 | 30 | 0 | 1.8 | auto | RBM | Closed Warlord Helmet | `sturgian_helmet_closed` |

**诊断**：
- 两件都是 RBM-buffed "Closed Warlord" T5 顶档
- Sturgia 118/82/30 对比 Nord 104/64/40 · head 差 14 · body 差 18 · arm 差 -10
- **数值整体可比** · 分化在 body(sturgia 顶) vs arm(nord 顶)
- **可能是设计意图**：Sturgia 版"胸锁更紧" · Nord 版"臂甲延伸更长"

**🟢 建议 跳过 or 微调**：
- 若保留文化分化：不动 · 14 点差异在"同档不同风" 可接受范围
- 若强制统一：取中位 h=111 b=73 a=35 · 两件都调

---

### Bucket 9 · `nasalhelm OVERMAIL` T5 · variance=13.5

| Culture | H | B | A | L | Wt | Price | Src | Display Name | StringId |
|---|---:|---:|---:|---:|---:|---:|---|---|---|
| empire | 90 | 12 | 25 | 0 | 3.9 | auto | RBM | Iron Nasalhelm over Mail | `ironlame_nasalhelm_over_imperial_mail` |
| empire | 87 | 12 | 25 | 0 | 3.7 | auto | RBM | Bronze Nasalhelm over Mail | `leatherlame_nasalhelm_over_imperial_mail` |
| sturgia | 75 | 12 | 17 | 0 | 1.8 | auto | RBM | Nasalhelm over Mail | `nasalhelm_over_mail` |

**诊断**：
- 三件都是 RBM · "Nasalhelm over Mail" Cat 5 "Over Mail" 档位（BALANCE_V2 L307 锚点 95/12/25）
- Empire 2 件 87-90/12/25 跟锚点吻合 · Sturgia 1 件 75/12/17 偏低 12-15 点
- 可能是 Sturgia 版本故意轻装 · 也可能是 RBM 疏忽

**选项**：
- **A · Sturgia 对齐 Empire**：75→**87** · 17→**25**（匹配 Cat 5 baseline）
- **B · 保留 Sturgia 轻装定位**：75→**82** · 17→**22**（略低于 Empire · 保留 "北方简朴" 风）
- **C · 不动**

**🟢 建议 B**：微 buff Sturgia 到 82/12/22 · 保留轻装文化差。

---

## 📋 Phase 1 决议清单（待用户审 · 1 文档内一次拍板）

### 🔴 必修（1 处 · 数据异常）

| # | 物品 | 建议 | 影响面 |
|---|---|---|---|
| 6 | `spiked_helmet_with_facemask` | **B**：保 h=119 · 清 b=90→12 a=40→0 | Khuzait 内部 1 件 · 修 RBM 数据录入错 |

### 🟡 推荐修（6 处 · 真跨文化欠 buff）

| # | 物品 | 建议 | 影响面 |
|---|---|---|---|
| 1 | `TV_aserai_lord_helmet_c` (Battle Crown Aserai 变体)| h=44→**90** | 1 件 |
| 2 | `AR_khuzait_lord_helmet_b` (Eastern Cataphract) | **等用户确认是否华夏线** · 否则 h=52→125 | 1 件 · 需用户 1 分钟判断 |
| 4a | 3 件 Imperial 装饰变体 (`AR_empire_lord_helmet_e`, `empire_battle_crown_west`, `AR_Imperial_Crowned_Helmet_a`) | h→**104** b→**14** 匹配 `plumed_helmet` | 3 件 |
| 4b | `TV_khuzait_helmet_y4` (Eastern Lordly Plumed) | h=44→**90** 匹配 Khuzait Battle Crown | 1 件 |
| 4c | `TV_sturgia_helmet_h` (Northern Gilded Steel) | h=49→**100** 匹配 Sturgia 次档 | 1 件 |
| 7 | Sturgia 2 件 Nasal (`TV_sturgia_helmet_g`, `ao_sturgian_feathered_helmet`) | h=49→**65** b=0→**8** a=0→**12** (Cat 2 With Mail) | 2 件 |
| 9 | `nasalhelm_over_mail` Sturgia | h=75→**82** a=17→**22** | 1 件 |

### 🟢 建议跳过（2 处 · false positive）

| # | 原因 |
|---|---|
| 3 | "Noble Guard" ≠ "Old Guard" 命名碰撞 · 两件实际不同档 |
| 5 | Aserai "Steel Cap" 3 件是 tier 梯度 · 不是同档 |

### 🟡 可选微调（1 处）

| # | 物品 | 建议 |
|---|---|---|
| 8 | `closed_goggled_helmet` + `sturgian_helmet_closed` | 保留文化分化（body-vs-arm 顶各一） · 或中位化 |

---

## 💡 预期影响

**修 Phase 1 决议后**（🔴+🟡 全采纳）：

| 维度 | 当前 | 修复后 |
|---|---:|---:|
| flagged bucket | 9 | 2-3（跳过的 false positive 保留） |
| 需调 items | - | 11 件（1 数据异常 + 10 跨文化欠 buff） |
| 影响文化 | - | aserai(1) · khuzait(2) · empire(3) · sturgia(4) · nord(0 不动) |

**跟保留 override 的关系**：
- Khuzait 华夏精工线：**问题 #2** 需用户判断 · 其他 Khuzait items 不涉及
- Aserai 波斯精工线：**问题 #1** Aserai Battle Crown 不是波斯精工（OSA 新件 · 可动）
- Nord 维京精工线：**问题 #7/#8** Nord 侧全部作锚点不动 · 只调 Sturgia

---

## 下一步工作流

1. **用户审本文档** · 对 🔴+🟡 项逐条拍板（可全采纳 · 可部分 · 可反对）
2. **问题 #2 (Khuzait Cataphract)** 需用户单独确认是否属华夏精工线
3. 用户决议后进入 **Phase 2**：
   - 修 `src/manual_override.ps1` 加 11 条 helmet-level override 规则
   - 或新建 `manual_override_head.ps1`（跟 shield 走同样 pipeline 模式）
   - 重跑 → deploy → 归档到 `BALANCE_V2_LOG.md`
4. **Phase 3（可选）**：扩展同 pipeline 到其他 4 槽（身甲/肩甲/臂甲/腿甲）

---

## 文件与管线清单

**本次产出**：
- `OSA_V3_HEAD_SCAN.md`（本文件 · Phase 1 决议清单）
- `src/scan_head_cross_culture.ps1`（扫描脚本 · 可重跑）
- `src/head_style_cross_culture_scan.json`（结构化扫描结果 · 可反复 drill）
- `src/head_scan_flagged_tables.md`（9 个 bucket 的 ASCII 表 · 本文档引用源）

**依赖的既有数据**：
- `src/armor_data_head.json`（1328 件头盔合并数据 · 本次扫描输入）
- `ARMOR_DATA_HEAD.md`（头盔总表 · 查具体物品用）
- `BALANCE_V2_LOG.md §Tier 公式` + `§Spangenhelm 7 Aventail 分类`（锚点来源）

**重跑扫描**：
```powershell
& src/scan_head_cross_culture.ps1    # 输出 head_style_cross_culture_scan.json + Top-20 到控制台
```
