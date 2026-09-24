# OSA Balance v2 · 手工审工作日志

**范围**：`OpenSourceArmouryRBMBalance` v1 脚本按 tier 分层的 buff 逻辑漏掉了大量视觉上明明是重装但 raw armor 值偏低的物品（比如 stylized/decorative 头盔）。v2 通过**用户 in-game 观察 → 手工逐件审**弥补这个缺口，按**文化 → 装备类型**顺序推进。

**📖 设计基调宪法**：所有平衡工作前**先读** [`DESIGN_PHILOSOPHY.md`](./DESIGN_PHILOSOPHY.md)——从帝国头盔 165 件手工审经验中提炼的通用设计哲学，含**两条铁律**、通用 7 原则、工作流纪律、用户反馈信号识别、vanilla 锚点速查。本文档（`BALANCE_V2_LOG.md`）是逐件手工审的权威决议记录，`DESIGN_PHILOSOPHY.md` 是指导决议的宪法。**新进入任何 armor 类型前先读 PHILOSOPHY 拟定该类型铁律**。

---

## 🔒 铁律 · 平衡工作基础数据源（2026-09-23 用户拍板）

> **所有 OSA 平衡工作以「原版经由 RBM 模组修改后的数据」为唯一权威基础**。
>
> 具体执行：
> 1. **先查 [`VANILLA_REFERENCE.md`](./VANILLA_REFERENCE.md)**——该文档已抽取 vanilla + RBM 已完成调整后的数值，是本项目的**权威锚点表**
> 2. **OSA 物品若能匹配某 vanilla+RBM 物品的 base_type + aventail_type → 直接抄该 vanilla 数值**（不做主观微调，不做"稍强/稍弱"判断）
> 3. **无 vanilla 直匹配的 OSA-only 物品** → 按家族 fallback 规则（aventail suffix 表 + base_type 家族均值 ± 装饰前缀）
> 4. **绝不擅自设定超越 vanilla+RBM 尺度的绝对值**（如把 Faceguard 家族的 94 anchor 套到 Roman Helmet 家族全档 → 属越权 buff，禁止）
> 5. **RBM 已做的取舍视为设计终稿**——即使个别 vanilla 值看起来"偏低"或"偏高"，也不做二次调整；OSA 只负责让自己家族数值**与 vanilla+RBM 同尺度对齐**，不建立独立尺度
>
> **违反此铁律的历史提案**（2026-09-23 Roman Helmet 家族首版）：把 Faceguard 家族 anchor 94/12/0 套到全 12 件 → 被用户驳回。修订后按 vanilla `helmet_with_faceguard` / `tall_helmet` / `plumed_helmet` 三档 direct match + Cap 家族 fallback，通过。

## 🔒 铁律 · Cape 家族设计规则（2026-09-23 用户拍板 · 二部分律）

### 第一部分：命名二分律

> Cape 类物品按**命名判定**二分：
>
> **A. 有 "shoulder" 或 "pauldron(s)" 命名的 Cape**：允许 `body_armor` 和 `arm_armor` 同时 > 0，但**必须满足 body > arm 严格序**
>
> **B. 无 "shoulder" / "pauldron" 命名的 Cape**（Cape/Cloak/Sash/Focale/Pelt/Collar 等）：**只允许 body_armor > 0，arm_armor 必须 = 0**
>
> - **判定优先命名 · 大小写不敏感**（如 `Pauldrons` / `Shoulders` 都算）
> - **理由**：命名反映 mesh 视觉外形——带 shoulder/pauldron 的 mesh 明显有肩甲结构（arm 覆盖），无此命名的 mesh 是纯斗篷/披风（无肩甲延伸）
> - **用户 quote**："任何带有 shoulder 或者 pauldrons 的肩甲允许身甲和臂甲同时存在但身甲必须大于臂甲，如果没有带有 shoulder 或者 pauldrons 的只提供身甲"

### 第二部分：arm mesh-tiered 分档律（2026-09-23 用户拍板 · 方案 C）

> Cape (A 组 · 有 shoulder/pauldron) 的 arm_armor 值**按 mesh 视觉覆盖度分档**：
>
> | mesh 档次 | arm 值 | 命名特征 |
> |---|---:|---|
> | **Elite Heavy Pauldrons/Shoulders**（顶档 · Gilded 品质） | **25** | Gilded / Heavy / 顶级材质 |
> | **Standard Shoulders** | **20** | Lamellar Shoulders 主流 |
> | **Standard Pauldrons**（较小 mesh） | **12** | Pauldrons 命名（比 Shoulders 略小） |
> | **Studded Strip / lightweight** | **6-8** | 轻档 studded strip 结构 |
> | Chainmail Shoulders 中档 | 10-12 | 链甲护肩 |
> | Leather Shoulders 轻档 | 4-8 | 皮革护肩 |
>
> **理由**：Heavy Lamellar Pauldrons 视觉上覆盖**整个肩膀+上臂**（甲片全裹肩+ upper arm），Standard Pauldrons 只覆盖肩膀，Studded Strip 仅局部加固——arm 值应**忠实反映 mesh 物理覆盖度**。
>
> **arm 堆叠机制说明**：Bannerlord 引擎 `final_arm = HeadArmor.arm + BodyArmor.arm + Cape.arm + HandArmor.arm`（加法），不区分手臂子区间。**OSA 精英兵 arm 总值会比 RBM 基线高 ~25 点（26%）**——这是**故意的设计取舍**：换取 OSA "Cape 特色" 忠实反映 mesh 视觉覆盖度。
>
> **顶点参照放宽**：vanilla `imperial_lamellar_shoulders` 是 55/0/3.5 (raw 55 · body only)。OSA 允许 body ≤ vanilla body（如 42/25 raw 67），因为 OSA 是"分配到 shoulder 和 upper_arm 两块 mesh"而非"堆积在同一块 mesh"。**只要 body ≤ vanilla body 顶（55），arm 加成属 OSA 特色不算越权**。
>
> **用户 quote**："我认为有必要增强一下 Heavy Lamellar Pauldrons 的数值，添加 25~ 的臂甲较为合适（甲片覆盖了整个肩膀，连同臂甲就可以覆盖整个手臂）"

### 第三部分：视觉判断优先律（2026-09-23 F 家族揭示）

> **命名允许 ≠ 数值强制**。命名二分律（第一部分）规定"有 shoulder/pauldron 命名的 Cape **允许** arm > 0"，但实际 arm 值必须通过 **mesh 视觉覆盖度确认**。若命名含 shoulder/pauldron 但 mesh 视觉**不覆盖上臂**（如纯覆盖 shoulder body 或 body），则 arm **必须 = 0**，覆盖命名默认值。
>
> **F 家族 · 视觉判断示例**（2026-09-23 用户 in-game 观察）：
> - `AR_imperial_shoulders_c/l` (Scale Shoulders): 命名含 Shoulders 但 mesh 只覆盖 body-shoulder，**arm 0**
> - `AR_imperial_shoulders_u/v` (Alternating Scale): 只覆盖 body 肩部，**arm 0**
> - `AR_imperial_shoulders_w/x` (Steel Scale): 只覆盖 body 肩部，**arm 0**
> - `AR_imperial_shoulders_d` (Scale With Lamellar): Lamellar 铁片覆盖上臂 → **arm 20**
> - `AR_imperial_shoulders_y` (Decorated Leather Harness Over Scale): Harness 部分覆盖上臂但小于 Lamellar → **arm 15**（介于 Pauldrons 12 和 Standard Shoulders 20 之间的部分覆盖档）
>
> **arm mesh-tiered 分档律扩展**（在第二部分基础上加入"部分覆盖"档）：
>
> | mesh 视觉档 | arm 值 |
> |---|---:|
> | Elite Heavy 顶档 · 完整肩+上臂覆盖 | 25 |
> | Standard Shoulders · 肩+上臂上半 | 20 |
> | **部分上臂覆盖**（如 Harness Over Scale · 小于 Lamellar） | **15** |
> | Standard Pauldrons · 仅肩 | 12 |
> | Chainmail Shoulders 中档 | 10-12 |
> | Studded Strip / Leather 轻档 | 6-8 |
> | **纯 body 无上臂覆盖**（Scale/Alternating/Steel Scale 类） | **0** |
>
> **用户 quote**："scale shoulders 的 mesh 似乎只覆盖身体，并不覆盖肩膀" · "with lamellar 的版本才有覆盖大臂的扎甲铁片" · "Decorated Leather Harness Over Scale 是同时覆盖了肩部和大臂，但大臂的护甲覆盖面积要比 Lamellar 系列要小"

## 🔒 铁律 · 头 > 身 > 臂 设计基调（2026-09-23 用户拍板）

> 所有 HeadArmor 类物品必须满足 **head_armor > body_armor > arm_armor** 的强序关系。
>
> - **原因**：头部是主要防护部位、颈甲延伸（gorget）覆盖 body 有限、肩甲/aventail 覆盖 arm 更少的物理直觉；同时保证视觉一致性——头盔外观越"重装"应先反映在头档、次反映在颈档、最后才是肩档
> - **执行**：即使 vanilla+RBM 参照物品本身存在 body < arm 结构（如 `roundkettle_over_imperial_mail` 92/0/20 · `imperial_nasal_helm` 97/12/25），OSA 平衡时**允许调整 body/arm 分布使其符合头 > 身 > 臂**，只要 raw 总和保持在 vanilla 家族尺度内
> - **例外**：无（无论 aventail 类型如何，头盔的 arm 覆盖不应超过 body 覆盖）
> - **追溯适用**：已归档决议若违反本条铁律需追溯修正——2026-09-23 首批追溯：Roman Helmet #11/#12（Stripped Cloth 系）· Nasalhelm #1（`ao_imperial_nasal_helmet`）
> - **用户 quote**："所有头盔应当都需要做到头甲 > 身甲 > 臂甲"

---

**工作流约定**：
- 用户在游戏内观察某物品数值失衡 → 提出目标数值
- Claude 改 `ModuleData/OSABalance_armor_override.xml` （或 `_pieces_override.xml`）
- **不 deploy**——用户游戏开着无法看装备属性；累积一批改动后由用户手动关游戏 → 跑 `deploy.ps1` → 重启
- 每件改动在本文档记录：**id / 名称 / 文化 / 类型 / 前 → 后数值 / tier 计算 / 部署状态**
- 用户实机验证后在本文档标 ✅

**优先方法（2026-09-22 定 · 2026-09-23 升级为铁律见上方）**：
1. 先查 [`VANILLA_REFERENCE.md`](./VANILLA_REFERENCE.md) 找 OSA 物品对应的 vanilla+RBM 参照
2. 直接抄 vanilla 值（RBM 已完成的调整视为权威）
3. 无 vanilla 对应的 OSA-only 物品用家族均值 fallback（aventail suffix 表 + base_type 家族头值均值）
4. 装饰前缀（Plumed/Gilded/Southern/Feathered/Bronze/Iron/Noble/Decorated/Redcrest/Jeweled/Crowned/Crested）一律零护甲影响
5. 用户特别列出的物品单独手工审

**Tier 计算公式**（vanilla `DefaultItemValueModel.CalculateArmorTier`）：
```
raw    = 1.2·head + body + leg + arm
scaled = raw × ItemTypeMultiplier   // HeadArmor 1.2 / BodyArmor 1.0 / Cape 1.8 / LegArmor 1.6 / HandArmor 1.7
tier   = clamp(round(scaled × 0.1 - 0.4), 0, 6) - 1
```

---

## Empire · HeadArmor

### 2026-09-22 · Provocator 家族（4件）

**问题**：v1 脚本按 raw head_armor 反推 tier，只给 T4+ 加 aventail（body/arm 延伸）。plain Provocator raw=20 → 算作 T1 → 全部跳过；faceplate 版 raw=40 → T3-4 → 完整 buff。同 mesh 家族两个 tier，同 Plate 材质护甲差 4.45×。

**用户决议**：4 件全部落 T5；面甲版比 plain 高一档（head +8、body +5、arm 不变）。

| id | 名称 | mat | 前 head/body/arm | 后 head/body/arm | tier | 状态 |
|---|---|---|---|---|:---:|:---:|
| `AR_gladiator_helmet_e` | Gilded Provocator | Plate | 20/0/0 | **88/45/32** | T5 | 🟡 pending deploy |
| `AR_gladiator_helmet_f` | Decorated Provocator | Plate | 20/0/0 | **90/40/35** | T5 | 🟡 pending deploy |
| `AR_gladiator_helmet_g` | Gilded Provocator w/ Faceplate | Plate | 89/38/33 | **96/50/32** | T5 | 🟡 pending deploy |
| `AR_gladiator_helmet_h` | Decorated Provocator w/ Faceplate | Plate | 89/38/33 | **98/45/35** | T5 | 🟡 pending deploy |

**注**：4 件 weight 未动（plain 0.9 / faceplate 2.99）。plain 版 0.9 kg 承载 88 head_armor 明显不合理，用户 point #1 已提出"in-game 观察材质与数值"，weight 校准留待后续。

---

### 2026-09-22 · Spangenhelm 家族全套重平衡（29 件）

**范围**：Empire 文化 filter=Spangenhelm 的 28 件（用户 in-game 校对）+ `ao_imperial_cataphracts_plumed_closed_mail_helmet`（原表遗漏编号，实际同族）= **29 件**。

**问题根源**：v1 脚本按 raw head_armor 反推 tier 分层 buff，导致同 mesh 家族被拆到不同 tier 处理；结果 27 件挤在 T5 head 74-95 无差异，另 1 件 (`_with_leather`) 塌陷 T1 (19/0/0)，视觉/构造差异被完全压平。

**用户设计原则**（2026-09-22）：
1. **Aventail 类型是护甲差异主轴**——head/body/arm 都按 aventail 类别定
2. **Body 保持低值**（8-19 范围）——不与 Provocator/身甲类别看齐
3. **arm 承担 aventail 覆盖差异**（皮 15 → mail 25 → mail coif 28 → closed mail 32）
4. **纯装饰前缀零护甲影响**：Plumed / Crested / Crowned / Feathered / Gilded / Jeweled / **Southern** / **Eastern** / Bronze / Iron / Noble / Decorated / Redcrest

#### 框架 · 7 Aventail 分类 + 4 结构修饰词

**7 Aventail 分类锚点**（Base h/b/a/wt）：

| Cat | 命中判定 | head | body | arm | wt |
|---|---|---:|---:|---:|---:|
| 1 · With Leather / no aventail | "with leather", "Cap" | 55 | 0 | 0 | 1.2 |
| 2 · With Mail | "with mail" 内衬 | 65 | 8 | 12 | 1.4 |
| 3 · Over Padded Cloth/Coif | "over cloth", "over padded coif", "over stripped cloth" | 70 | 10 | 12 | 1.5 |
| **4 · Over Leather** ← 用户锚点 A | "over leather" | **76** | **12** | **15** | **1.7** |
| **5 · Over Mail** ← 用户锚点 B | "over mail", "over imperial mail" | **95** | **12** | **25** | **3.3** |
| 6 · Over Mail Coif | "over mail coif" | 100 | 14 | 28 | 3.6 |
| 7 · Over Closed Mail | "closed mail" | 108 | 16 | 32 | 3.9 |

**4 结构修饰词**（附加，可叠加）：

| 修饰词 | +head | +body | +arm | +wt |
|---|---:|---:|---:|---:|
| Heavy | +5 | +2 | +2 | +0.3 |
| Nasal | +3 | 0 | 0 | +0.1 |
| Guarded | +4 | +3 | +3 | +0.3 |
| Cataphract | +6 | +3 | +3 | +0.5 |

#### 落地清单 · 29 件全部 pending deploy

| # | id | Cat + 修饰 | 前 h/b/a/wt | 后 h/b/a/wt | 状态 |
|---|---|---|---|---|:---:|
| 1 | `ao_imperial_spangenhelm_with_leather` | 1 | 19/0/0/0.46 | **55/0/0/1.2** | 🟡 |
| 2 | `ao_imperial_heavy_spangenhelm_with_leather` | 1+Heavy | 78/0/0/1.8 | **60/2/2/1.5** | 🟡 |
| 3 | `ao_imperial_nasal_spangenhelm_with_leather_strips` | 1+Nasal | 88/0/0/1.8 | **58/0/0/1.3** | 🟡 |
| 4 | `AR_empire_desert_helmet_c` (Southern Cap) | 1 | 78/0/0/3.4 | **55/0/0/1.2** | 🟡 |
| 5 | `ao_imperial_spangenhelm_with_mail` | 2 | 88/0/0/1.8 | **65/8/12/1.4** | 🟡 |
| 6 | `ao_imperial_heavy_spangenhelm_with_mail` | 2+Heavy | 88/0/0/1.8 | **70/10/14/1.7** | 🟡 |
| 7 | `ao_imperial_nasal_spangenhelmet_with_mail` | 2+Nasal | 95/0/0/1.8 | **68/8/12/1.5** | 🟡 |
| 8 | `ao_imperial_spangenhelm_over_padded_coif` | 3 | 74/0/0/1.8 | **70/10/12/1.5** | 🟡 |
| 9 | `TV_empire_helmet_g` (Gilded Over Padded Cloth) | 3+Gilded | 74/32/27/2.34 | **70/10/12/1.5** | 🟡 |
| 10 | `AR_empire_helmet_q` (Over Cloth) | 3 | 79/34/29/2.34 | **70/10/12/1.5** | 🟡 |
| 11 | `AR_empire_helmet_r` (Over Stripped Cloth) | 3 | 84/36/31/2.77 | **70/10/12/1.5** | 🟡 |
| 12 | `AR_empire_helmet_s` (Eastern Over Cloth) | 3 | 79/34/29/2.77 | **70/10/12/1.5** | 🟡 |
| 13 | `ao_imperial_crowned_nasal_helmet` (Crowned with Cloth) | 3+Nasal | 82/35/30/1.54 | **73/10/12/1.6** | 🟡 |
| 14 | `AR_empire_desert_helmet_b` (Southern Nasalhelm Over Cloth) | 3+Nasal | 92/0/0/3.2 | **73/10/12/1.6** | 🟡 |
| 15 | `AR_empire_desert_helmet_d` (Southern Nasalhelm Over Leather) | 4+Nasal | 90/39/33/1.64 | **79/12/15/1.8** | 🟡 |
| 16 | `AR_empire_desert_helmet_a` (Southern Plumed Nasalhelm Over Mail) | 5+Nasal | 86/37/32/2.74 | **98/12/25/3.4** | 🟡 |
| 17 | `TV_empire_helmet_h` (Gilded Over Mail) | 5+Gilded | 90/39/33/2.34 | **95/12/25/3.3** | 🟡 |
| 18 | `TV_empire_lord_helmet_h` (Gilded Plumed Over Mail) | 5 | 86/37/32/3.08 | **95/12/25/3.3** | 🟡 |
| 19 | `aserai_feathered_spangenhelm` (Southern Feathered Over Mail) | 5 | 89/38/33/2.91 | **95/12/25/3.3** | 🟡 |
| 20 | `bronze_aserai_helm` (Southern Iron Nasalhelm Over Mail) | 5+Nasal | 89/38/33/2.91 | **98/12/25/3.4** | 🟡 |
| 21 | `AR_empire_lord_helmet_f` (Guarded w/ Feather Crest) ⚠ | 5+Guarded (推定) | 90/39/33/2.84 | **99/15/28/3.6** | 🟡 |
| 22 | `AR_empire_helmet_a` (Noble Jeweled Heavy Nasalhelm) ⚠ | 5+Heavy+Nasal (推定) | 88/38/33/2.76 | **103/14/27/3.7** | 🟡 |
| 23 | `AR_empire_helmet_b` (Noble Gilded Jeweled Heavy Nasalhelm) ⚠ | 5+Heavy+Nasal (推定) | 85/37/31/2.76 | **103/14/27/3.7** | 🟡 |
| 24 | `DZ_empire_helmet_d` (Cataphract's Bent Conical) ⚠ | 5+Cataphract (推定) | 92/40/34/— | **101/15/28/3.8** | 🟡 |
| 25 | `ao_imperial_heavy_spangenhelm_over_mail_coif` | 6+Heavy | 82/35/30/1.56 | **105/16/30/3.9** | 🟡 |
| 26 | `ao_imperial_heavy_nasal_spangenhelm_over_mail_coif` | 6+Heavy+Nasal | 90/39/33/1.56 | **108/16/30/4.0** | 🟡 |
| 27 | `ao_imperial_redcrest_bronze_spangenhelm_over_mail_coif` | 6 | 89/38/33/1.54 | **100/14/28/3.6** | 🟡 |
| 28 | `ao_imperial_closed_mail_nasal_spangenhelmet` | 7+Nasal | 87/37/32/1.38 | **111/16/32/4.0** | 🟡 |
| 29 | `ao_imperial_cataphracts_plumed_closed_mail_helmet` | 7+Cataphract | 85/37/31/1.38 | **114/19/35/4.4** | 🟡 |

**⚠ 4 件 aventail 类别推定项**（名字里无 "over-X" 关键词，按语境猜 Cat 5 Over Mail）：#21/22/23/24。用户 in-game 观察若视觉不是链甲颈甲，回来重分类。

**新家族阶梯**（head_armor 从低到高）：55 → 58 → 60 → 65 → 68 → 70 → 73 → 76 → 79 → 95 → 98 → 99 → 100 → 101 → 103 → 105 → 108 → 111 → 114（19 档，vs v1 的 27/28 挤在 74-95）。

**Weight 阶梯**：1.2 → 4.4 kg。重装真的更重。

---

### 2026-09-22 · Spangenhelm 家族 v2b 重推（用 vanilla+RBM 参考底表）

**触发**：用户观察到"OSA 物品是 vanilla 物品的视觉变体，如果搞清楚 RBM 调整逻辑就能批量平衡"。RBM_Reference 数据里发现**用户 anchor A/B 就是两件 vanilla 物品**：
- `leatherlame_feathered_spangenhelm_over_leather` = 76/12/15/1.7 ← 用户 Base A（用户是从游戏里读的这个值）
- `ironlame_feathered_spangenhelm_over_mail` = 95/12/25/3.3 ← 用户 Base B

**方法转变**：v2a 是我凭经验推测锚点 + 修饰词 delta；v2b 抛弃自制 delta，全部按 vanilla 底表映射。参见 [`VANILLA_REFERENCE.md`](./VANILLA_REFERENCE.md) 帝国 54 件 vanilla 头盔完整参照。

**关键修正**（v2a → v2b）：
- **Cat 6 Over Mail Coif**：我 v2a 用 100/14/28/3.6，vanilla `feathered_spangenhelm_over_imperial_coif` = **88/22/40/3.1**（head 低但 b/a 高得多）
- **Cat 7 Closed Mail**：我 v2a 用 108/16/32/3.9，顶配 `imperial_goggled_helmet` = **144/82/45/4.2**（严重低估）
- **Heavy 修饰词**：我 v2a 用 +5/+2/+2/+0.3，vanilla `heavy_*` 相对 `ironlame_*` 是 **+25-35 head、+20-25 body**（我低估 5-6 倍）
- **Guarded / Cataphract**：v2a 用 +4~6，vanilla 对应 `empire_lord_helmet` / `empire_guarded_lord_helmet` / `imperial_goggled_helmet` 是**独立 tier 跳跃**（不是 delta 叠加）

**29 件 v2a → v2b 改动清单**（v2a 值 → v2b 值）：

| # | id | v2a (h/b/a/wt) | **v2b (h/b/a/wt)** | vanilla 参照 |
|---|---|---|---|---|
| 1 | `spangenhelm_with_leather` | 55/0/0/1.2 | **76/12/15/1.7** | leatherlame_..._over_leather |
| 2 | `heavy_spangenhelm_with_leather` | 60/2/2/1.5 | **95/22/15/2.8** | + heavy delta |
| 3 | `nasal_spangenhelm_with_leather_strips` | 58/0/0/1.3 | **79/12/15/1.8** | leather + nasal |
| 4 | `desert_helmet_c` (Southern Cap) | 55/0/0/1.2 | **65/12/25/3.2** | ironlame_feathered_spangenhelm |
| 5 | `spangenhelm_with_mail` | 65/8/12/1.4 | **95/12/25/3.3** | user's anchor B |
| 6 | `heavy_spangenhelm_with_mail` | 70/10/14/1.7 | **124/36/20/3.6** | heavy_nasalhelm_over_mail |
| 7 | `nasal_spangenhelmet_with_mail` | 68/8/12/1.5 | **98/12/25/3.4** | over mail + nasal |
| 8 | `spangenhelm_over_padded_coif` | 70/10/12/1.5 | **70/15/25/1.8** | base + padding delta |
| 9 | `TV_empire_helmet_g` | 70/10/12/1.5 | **70/15/25/1.8** | same |
| 10 | `AR_empire_helmet_q` | 70/10/12/1.5 | **65/12/6/1.6** | base + cloth delta |
| 11 | `AR_empire_helmet_r` | 70/10/12/1.5 | **65/12/6/1.6** | same |
| 12 | `AR_empire_helmet_s` | 70/10/12/1.5 | **65/12/6/1.6** | same |
| 13 | `crowned_nasal_helmet` | 73/10/12/1.6 | **68/12/6/1.7** | cloth + nasal |
| 14 | `desert_helmet_b` (Nasalhelm+Cloth) | 73/10/12/1.6 | **55/12/6/3.2** | ironlame_nasalhelm_over_cloth |
| 15 | `desert_helmet_d` (Nasalhelm+Leather) | 79/12/15/1.8 | **61/12/17/1.9** | leatherlame_nasalhelm_over_leather |
| 16 | `desert_helmet_a` (Plumed Nasalhelm Over Mail) | 98/12/25/3.4 | **90/12/25/3.9** | ironlame_nasalhelm_over_mail |
| 17 | `TV_empire_helmet_h` | 95/12/25/3.3 | **95/12/25/3.3** | (未变) |
| 18 | `TV_empire_lord_helmet_h` | 95/12/25/3.3 | **95/12/25/3.3** | (未变) |
| 19 | `aserai_feathered_spangenhelm` | 95/12/25/3.3 | **95/12/25/3.3** | (未变) |
| 20 | `bronze_aserai_helm` | 98/12/25/3.4 | **90/12/25/3.9** | ironlame_nasalhelm_over_mail |
| 21 | `AR_empire_lord_helmet_f` (Guarded Feather Crest) | 99/15/28/3.6 | **120/14/20/3.8** | empire_helmet_with_metal_strips |
| 22 | `AR_empire_helmet_a` (Noble Heavy Nasalhelm) | 103/14/27/3.7 | **124/36/20/3.6** | heavy_nasalhelm_over_mail |
| 23 | `AR_empire_helmet_b` (Noble Gilded Heavy Nasalhelm) | 103/14/27/3.7 | **124/36/20/3.6** | same |
| 24 | `DZ_empire_helmet_d` (Cataphract Bent Conical) | 101/15/28/3.8 | **123/10/0/3.5** | empire_lord_helmet (Noble Guard) |
| 25 | `heavy_spangenhelm_over_mail_coif` | 105/16/30/3.9 | **118/45/40/3.5** | coif base + heavy delta |
| 26 | `heavy_nasal_spangenhelm_over_mail_coif` | 108/16/30/4.0 | **121/45/40/3.6** | above + nasal |
| 27 | `redcrest_bronze_spangenhelm_over_mail_coif` | 100/14/28/3.6 | **88/22/40/3.1** | feathered_spangenhelm_over_imperial_coif |
| 28 | `closed_mail_nasal_spangenhelmet` | 111/16/32/4.0 | **110/40/45/3.7** | 半 goggled 级 |
| 29 | `cataphracts_plumed_closed_mail_helmet` | 114/19/35/4.4 | **144/82/45/4.2** | imperial_goggled_helmet (顶配) |

**新家族阶梯**（head 从低到高）：55 → 61 → 65 → 68 → 70 → 76 → 79 → 88 → 90 → 95 → 98 → 110 → 118 → 120 → 121 → 123 → 124 → 144（18 档，跨度 55→144，前 55 后 144）

**Weight 阶梯**：1.6 → 4.2 kg，与 vanilla 一致

**观察**：v2b 相比 v2a **顶端物品数值上调剧烈**（Cataphract 从 114 → 144、Heavy Nasalhelm 从 103 → 124），**中低端物品数值下调**（Cat 3 布垫从 70 → 65；Nasalhelm 布垫从 73 → 55）。这是 vanilla+RBM 真实平衡曲线的还原——**顶配远比中位强、低端更弱**。

**状态**：全部 🟡 pending deploy

---

### 2026-09-22 · Spangenhelm v2c · 材质品质字典差异化

**触发**：v2b 后有 3 组"数值克隆"（多件视觉不同但 h/b/a/wt 完全一致），用户要"些许区别性"。

**方案**：材质品质字典（详见 [`VANILLA_REFERENCE.md`](./VANILLA_REFERENCE.md) §材质品质字典）——按物品名字/id 里的 `Gilded / Noble / Jeweled / Iron / Bronze / Lord` 等词加/减 head（Lord 额外 +1 body/arm）。装饰词（Plumed/Crowned/Southern 等）零影响。

**9 件受影响**（v2b → v2c）：

| # | id | 匹配词 | Δ | v2b h/b/a | v2c h/b/a |
|---|---|---|---|---|---|
| 9 | `TV_empire_helmet_g` | Gilded | +2/0/0 | 70/15/25 | **72/15/25** |
| 13 | `crowned_nasal_helmet` | Bronze | -1/0/0 | 68/12/6 | **67/12/6** |
| 17 | `TV_empire_helmet_h` | Gilded | +2/0/0 | 95/12/25 | **97/12/25** |
| 18 | `TV_empire_lord_helmet_h` | Gilded + Lord(id) | +4/+1/+1 | 95/12/25 | **99/13/26** |
| 20 | `bronze_aserai_helm` | Iron (name) | +1/0/0 | 90/12/25 | **91/12/25** |
| 21 | `AR_empire_lord_helmet_f` | Lord(id) | +2/+1/+1 | 120/14/20 | **122/15/21** |
| 22 | `AR_empire_helmet_a` | Noble+Jeweled | +4/0/0 | 124/36/20 | **128/36/20** |
| 23 | `AR_empire_helmet_b` | Noble+Gilded+Jeweled | +5(cap)/0/0 | 124/36/20 | **129/36/20** |
| 27 | `redcrest_bronze_..._over_mail_coif` | Bronze | -1/0/0 | 88/22/40 | **87/22/40** |

**打破的克隆组**：
- Over Mail 三兄弟 #17/18/19 → **97 / 99 / 95**（Gilded 中、Lord+Gilded 强、Southern Feathered 基础）
- Noble Heavy 双胞胎 #22/23 → **128 / 129**（Gilded 版微强）
- Guarded lord #21 从 120 → 122（因 id 含 "lord"）

**未受影响**：20 件（无字典匹配词的物品保持 v2b 值）

**状态**：全部 🟡 pending deploy

---

### 2026-09-22 · 字典 v3 · 加 Face Plate/Closed + Metal Stripes/Ridge

**新加词**（详见 [`VANILLA_REFERENCE.md`](./VANILLA_REFERENCE.md)）：
- `Face Plate / Faceplate / Closed`：**+5/+5/+3**（面罩/闭式全脸覆盖）
- `Metal Stripes / Metal Strips / Ridge`：**+2/+1/+1**（金属加固带/凸脊，与 Lord 同 delta 但语义不同）

**Metal Stripes vs Ridge 同级判定**：都是"基础帽壳 + 局部金属加固"的工程范式，游戏内不可视差别。合并为同一 delta 简化字典维护。

**新增优先级规则**：直接 vanilla 匹配得到 base 值的物品，字典不叠加（避免与 vanilla 内含设计冲突）。

**Spangenhelm 家族 1 件回改**：

| # | id | 名字含 | v2c | **v2d** | 备注 |
|---|---|---|---|---|---|
| 28 | `ao_imperial_closed_mail_nasal_spangenhelmet` | Closed + Nasal 结构 | 110/40/45/3.7 | **103/17/28/3.3** | base over mail 95/12/25 + Closed(+5/+5/+3) + Nasal(+3h) |

原 v2b 我凭"半 goggled 直觉"给了 110/40/45；字典正规化推导为 103/17/28。#29 因直接匹配 `imperial_goggled_helmet` 排除字典。

**状态**：#28 🟡 pending deploy（连同前面所有改动）

---

## Empire · Crested 家族

### 2026-09-22 · Crested 家族 8 件 + Silvered 修订

**范围校准**：初次候选 14 件（名字含 crest），用户 in-game filter=Crested 显示 8 件——Intercisa 5 件（"Metal Crest" 归 Ridged filter）和 Aristocrats 1 件（"Feather Crest" 归 Guarded/Faceguard filter）不在 Crested filter 内，留待各自家族处理。

**字典 v4 修订**：`Silvered` 从 +2 → **+1**（银比金常见，形成 Bronze(-1) < Iron/Silvered(+1) < Gilded/Jeweled/Noble(+2) < Lord(+2 全方位) 品质阶梯）。

**8 件落地清单**（v2c → v3）：

| # | id | 子类 | vanilla base | Dict | 前 h/b/a/wt | **后 h/b/a/wt** | 状态 |
|---|---|---|---|---|---|---|:---:|
| 1 | `AR_roman_helmet_d_plumed_c` (Gilded Crested Helmet w/ Faceplate) | B · Faceguard | `helmet_with_faceguard` 94/12/0/3.5 | Gilded+2, Crested 0 | 88/38/33/2.99 | **96/12/0/3.5** | 🟡 |
| 2 | `AR_empire_crested_helm_a` (Crested Helmet w/ Faceguard) | B · Faceguard | 同上 | Crested 0 | 80/34/30/3.16 | **94/12/0/3.5** | 🟡 |
| 3 | `TV_empire_crested_helm_a` (Gilded Crested Helmet w/ Faceguard) | B · Faceguard | 同上 | Gilded+2, Crested 0 | 80/34/30/3.16 | **96/12/0/3.5** | 🟡 |
| 4 | `AR_empire_legatus_helm_a` (Decorated Crested Lord Helmet) | C · Legatus | `empire_helmet_with_metal_strips` 120/14/20/3.8 | Lord+2/+1/+1, Decorated+Crested 0 | 84/36/31/3.16 | **122/15/21/3.8** | 🟡 |
| 5 | `AR_empire_legatus_helm_b` (Gilded Crested Lord Helmet) | C · Legatus | 同上 | Lord+2/+1/+1, Gilded+2, Crested 0 | 84/36/31/3.16 | **124/15/21/3.8** | 🟡 |
| 6 | `TV_empire_lord_helmet_b` (Crested Decorated Silvered Ridge Helmet) | E · Battle Crown | `empire_battle_crown_north` 104/12/0/2.1 | Silvered+1, Crested+Decorated 0 | 85/37/31/2.38 | **105/12/0/2.1** | 🟡 |
| 7 | `TV_empire_lord_helmet_c` (Crested Decorated Gilded Ridge Helmet) | E · Battle Crown | 同上 | Gilded+2, Crested+Decorated 0 | 85/37/31/2.38 | **106/12/0/2.1** | 🟡 |
| 8 | `TV_empire_lord_helmet_f` (Feather Crested Decorated Silvered Ridge Helmet) | E · Battle Crown | 同上 | Silvered+1, Feather Crested+Decorated 0 | 83/36/31/2.38 | **105/12/0/2.1** | 🟡 |

**克隆状态**：#6 和 #8 数值相同（都 Silvered + Ridge base，Feather 是 0）——用户接受此现状。

**Crested 家族最终阶梯**（head 从低到高）：94 → 96/96 → 105/105 → 106 → 122 → 124

**延伸留待**：
- Intercisa 5 件（`AR_intercisa_helmet_a-e`，Ridged 家族）
- Aristocrats 1 件（`ao_imperial_aristocrats_helmet`，Guarded Noble/Cataphract 家族）

---

### 2026-09-22 · 字典 v5 · 独立文档 + Face Guard 合并 + Ridge 阶梯上调

**结构变更**：字典抽离为独立文档 [`MATERIAL_QUALITY_DICT.md`](./MATERIAL_QUALITY_DICT.md)，`VANILLA_REFERENCE.md` 里保留指针。

**字典 v5 变更**：
1. **`Face Guard / Faceguard`** 合并到 Metal Stripes/Ridge 档
2. **该档 delta 从 +2/+1/+1 → +4/+3/+2**——依据 vanilla `helmet_with_faceguard` (94/12/0) vs `tall_helmet` (84/0/0) = +10/+12/+0，说明"局部加固/面护"实际贡献远大于原 +2/+1/+1；字典取保守中值 +4/+3/+2

**Ridge 阶梯从 +2/+1/+1 提升到 +4/+3/+2 的 Crested 家族回改**：

| # | id | 名字含 | v3 值 | **v4 值** | 备注 |
|---|---|---|---|---|---|
| 6 | `TV_empire_lord_helmet_b` (Crested Decorated Silvered Ridge Helmet) | Ridge | 105/12/0/2.1 | **109/15/2/2.1** | + Ridge +4/+3/+2 |
| 7 | `TV_empire_lord_helmet_c` (Crested Decorated Gilded Ridge Helmet) | Ridge | 106/12/0/2.1 | **110/15/2/2.1** | + Ridge +4/+3/+2 |
| 8 | `TV_empire_lord_helmet_f` (Feather Crested Decorated Silvered Ridge Helmet) | Ridge | 105/12/0/2.1 | **109/15/2/2.1** | + Ridge +4/+3/+2 |

Crested v3 时"Ridge Helmet"被当作 base 类型不叠加字典；v5 统一按 modifier 处理，回改一致性。#4/#5 Legatus 不受影响（直接匹配 `empire_helmet_with_metal_strips`，字典不叠加）。

**Crested 家族最终阶梯 v4**：`94 → 96/96 → 109/109 → 110 → 122 → 124`

**未来待应用 Ridge/Metal Crest**：
- Intercisa 5 件（"Metal Crest" in name）处理时按 +4/+3/+2 应用
- 其他文化任何"Ridge/Face Guard/Metal Stripes"物品同

---

## Empire · Secutor 家族

### 2026-09-22 · Secutor 家族 6 件 + Visored dict

**字典 v6 变更**：`Visored` 加入 Face Plate/Closed 档（+5/+5/+3）——visor 机构 = 全脸闭合防护量级。

**Vanilla 锚点**：`helmet_with_faceguard` (94/12/0/3.5)——Secutor 是罗马格斗士全脸闭合头盔，与 vanilla faceguard 概念完全对应。

**6 件落地清单**：

| # | id | Name | Dict | 前 h/b/a/wt | **后 h/b/a/wt** | 状态 |
|---|---|---|---|---|---|:---:|
| 1 | `AR_gladiator_helmet_c` | Secutor Helmet | (base) | 92/0/0/3.2 | **94/12/0/3.5** | 🟡 |
| 2 | `AR_gladiator_helmet_c2` | Secutor Helmet With Feathers | Feathers 0 | 84/36/31/2.57 | **94/12/0/3.5** | 🟡 |
| 3 | `AR_gladiator_helmet_a` | Gilded Secutor Helmet | Gilded +2 | 92/0/0/3.2 | **96/12/0/3.5** | 🟡 |
| 4 | `AR_gladiator_helmet_a2` | Gilded Secutor Helmet With Feathers | Gilded +2, Feathers 0 | 84/36/31/2.57 | **96/12/0/3.5** | 🟡 |
| 5 | `AR_gladiator_helmet_d` | Visored Secutor Helmet With Feathers | Visored +5/+5/+3, Feathers 0 | 88/38/33/2.3 | **99/17/0/3.8** | 🟡 |
| 6 | `AR_gladiator_helmet_b` | Gilded Visored Secutor Helmet With Feathers | Gilded +2, Visored +5/+5/+3, Feathers 0 | 88/38/33/2.3 | **101/17/0/3.8** | 🟡 |

**注**：
- arm 保持 0（base arm=0，字典 body/arm 加成仅当 base > 0 才生效——Secutor 历史上无肩甲/aventail）
- 2 组数值克隆：#1/#2 (94/12/0)、#3/#4 (96/12/0)——Feathers 决议为零影响，用户接受
- Visored 版重 +0.3 kg（visor 机构重量）

**家族阶梯**：94 → 96 → 99 → 101（4 档）

**原 v2b 值观察**：所有 6 件原本被 v1 脚本分给不同 tier 且数值混乱（92/0/0 vs 84/36/31 vs 88/38/33）——Feathers 变体反而比 base 弱且带 aventail，跟历史/视觉矛盾。v6 归位为清晰阶梯。

---

### 2026-09-22 · 字典 v7 · Cataphracts 拆档 + 结构性 wt 修饰 + 10 件 wt 回改

**字典 v7 变更**（详见 [`MATERIAL_QUALITY_DICT.md`](./MATERIAL_QUALITY_DICT.md)）：
1. **`Cataphracts / Cataphract`** 从 Face Plate/Closed 拆出为**独立顶档**：+7/+7/+5，+2 wt
2. **所有结构性/工艺加强档加 wt 修饰**：
   - Metal Stripes/Ridge/Face Guard: **+1 wt**
   - Face Plate/Faceplate/Closed/Visored: **+1 wt**
   - Cataphracts: **+2 wt**
   - Lord: +0 wt（精工不加体重）
3. **数值精度规则**：armor 必须整数，weight 允许小数（游戏 UI 显示 1 位小数）

**已完成 47 件的 wt 审计**：
- Provocator 4: ❌ 从未 wt 平衡（0.9/2.99 与 88+ head 严重不匹配）→ 需要补
- Spangenhelm 29: ✅ 已 wt 平衡，但 #28 有 Closed dict 词需回应用 +1 wt
- Crested 8: ✅ 已 wt 平衡，但 #6/#7/#8 有 Ridge dict 词需回应用 +1 wt
- Secutor 6: ✅ 已 wt 平衡，但 _d/_b 有 Visored dict 词需回应用 +1 wt（替换早期 informal +0.3）

**10 件 wt 回改清单**：

| id | h/b/a | 前 wt | Dict 触发 | **新 wt** |
|---|---|---:|---|---:|
| `AR_gladiator_helmet_e` (Gilded Provocator) | 88/45/32 | 0.9 | 补基础 wt | **4** |
| `AR_gladiator_helmet_f` (Decorated Provocator) | 90/40/35 | 0.9 | 补基础 wt | **4** |
| `AR_gladiator_helmet_g` (Gilded Provocator Faceplate) | 96/50/32 | 2.99 | Faceplate +1 | **5** |
| `AR_gladiator_helmet_h` (Decorated Provocator Faceplate) | 98/45/35 | 2.99 | Faceplate +1 | **5** |
| `ao_imperial_closed_mail_nasal_spangenhelmet` | 103/17/28 | 3.3 | Closed +1 | **4.3** |
| `TV_empire_lord_helmet_b` (Silvered Ridge) | 109/15/2 | 2.1 | Ridge +1 | **3.1** |
| `TV_empire_lord_helmet_c` (Gilded Ridge) | 110/15/2 | 2.1 | Ridge +1 | **3.1** |
| `TV_empire_lord_helmet_f` (Silvered Ridge Feather) | 109/15/2 | 2.1 | Ridge +1 | **3.1** |
| `AR_gladiator_helmet_d` (Visored Secutor) | 99/17/0 | 3.8 | Visored +1 | **4.5** |
| `AR_gladiator_helmet_b` (Gilded Visored Secutor) | 101/17/0 | 3.8 | Visored +1 | **4.5** |

其他 37 件已改物品的 wt 保留（已合理，无 dict 结构词需回应用）。**armor 值全部不变**。

**Cataphracts 新档影响**：
- 现有 #29 `cataphracts_plumed_closed_mail_helmet` 使用直接 vanilla 匹配（`imperial_goggled_helmet` 144/82/45/4.2），字典不叠加，保持不变
- 未来遇到 non-direct-match 的 Cataphract 物品才应用 +7/+7/+5/+2wt

**状态**：全部 🟡 pending deploy

---

### 2026-09-22 · 字典 v8 · Heavy 入库 + direct-match 精化 + 4 件 Heavy 回改

**字典 v8 变更**（详见 [`MATERIAL_QUALITY_DICT.md`](./MATERIAL_QUALITY_DICT.md)）：
1. **`Heavy`** 加入结构性档：**+4/+3/+3, +2 wt**（与 Ridge/Face Guard 同级、比 Face Plate 略弱）
2. **direct-match 规则精化**：要求 `base_type + aventail + 主要结构性词` 三者对齐才算直匹配；跨 base_type（如 OSA Spangenhelm 匹到 vanilla Nasalhelm）无效，须回退到 base + dict
3. **区分结构性 vs 品质字典**：直匹配时结构性字典不叠加，但品质字典（Lord/Gilded/Noble 等）**可继续叠加**（vanilla 已含结构、未含品质）

**Heavy Spangenhelm 6 件重审**：

| # | id | 旧值 (v7) | 处理 | **新值 (v8)** |
|---|---|---|---|---|
| Span #2 | `heavy_spangenhelm_with_leather` | 95/22/15/2.8 | 无 vanilla heavy_spangenhelm，dict-driven | **80/15/18/3.7** |
| Span #6 | `heavy_spangenhelm_with_mail` | 124/36/20/3.6 (跨 base_type 错匹) | 修正为 Spangenhelm base + Heavy dict | **99/15/28/5.3** |
| Span #25 | `heavy_spangenhelm_over_mail_coif` | 118/45/40/3.5 | dict-driven | **92/25/43/5.1** |
| Span #26 | `heavy_nasal_spangenhelm_over_mail_coif` | 121/45/40/3.6 | dict-driven + Nasal 非正式 +3h | **95/25/43/5.1** |
| Span #22 | `AR_empire_helmet_a` (Noble Heavy Nasalhelm) | 128/36/20/3.6 | vanilla `heavy_nasalhelm_over_mail` 直匹配有效（Nasalhelm+Mail+Heavy 全对齐）+ Noble+Jeweled +4 | **128/36/20/3.6**（不变）|
| Span #23 | `AR_empire_helmet_b` (Noble Gilded Heavy Nasalhelm) | 129/36/20/3.6 | 同上 + Noble+Gilded+Jeweled +5(cap) | **129/36/20/3.6**（不变）|

**Heavy Spangenhelm 家族新阶梯**：`80 (leather) → 92 (coif) → 95 (nasal coif) → 99 (mail)` —— dict-consistent progression。

**Heavy Nasalhelm 家族仍保持顶端**：`128 → 129` (vanilla `heavy_nasalhelm_over_mail` = 124 基础)

**关键修正**：Span #6 原 124 是我在 v2b 时把 Spangenhelm 错匹到 Nasalhelm 家族造成——现在归位到 99 (Spangenhelm 家族)。Heavy Spangenhelm 与 Heavy Nasalhelm 数值差异合理反映家族差别。

**状态**：全部 🟡 pending deploy

---

## Empire · 杂项头盔（无明确 filter 分类）

### 2026-09-22 · Scale Coif ×2 + Cap Helmet + Faceguard vanilla 确认

**背景**：用户在游戏中列出 4 件"无明确分类"头盔。其中 `Helmet with Faceguard` 就是 vanilla `helmet_with_faceguard` (94/12/0/3.5)，不在 OSA override 里 —— RBM 已直接调好，保持不改。

**3 件 OSA 物品落地**：

| # | id | Name | 前 h/b/a/wt | **后 h/b/a/wt** | 参照 |
|---|---|---|---|---|---|
| 1 | `AR_empire_helmet_o` | Imperial Brass Scale Coif | 92/0/0/1.7 | **45/15/35/1.7** | vanilla `mail_coif` (38/12/38/1.7) + Scale 比 mail 硬 +7 head |
| 2 | `AR_empire_helmet_o2` | Imperial Alternating Scale Coif | 92/0/0/1.7 | **45/15/35/1.7** | 同 #1（Alternating vs Brass 视觉变体） |
| 3 | `AR_roman_helmet_cap` | Imperial Cap Helmet | 18/0/0/0.83 | **40/0/0/1.2** | 介于 vanilla `leather_cap` (14) 和 `leatherlame_roundkettle` (47)，基础金属帽无 aventail |

**Scale Coif bug 修正**：原 92/0/0 是 v1 脚本的"高 head + 空 coif 结构"错误——"Coif" 名字暗示头罩必有 body/arm 覆盖，现在归位到 mail_coif 加强版。

**Cap Helmet bug 修正**：原 18 head 太低（低于 vanilla 布 coif 14 只多 4），Cap Helmet 是金属帽应该有基本防护。

**克隆**：Brass 和 Alternating 视觉变体同数值（用户接受）。

**#1 vanilla Helmet with Faceguard 保留原因**：
- 是 OSA 平衡工作里的**权威锚点**——已被 Secutor 6 件、Provocator 2 件、Crested 3 件等多批物品直接引用作基础
- 改动会连锁影响所有 downstream 锚定物品
- RBM 团队专门调的 94/12/0/3.5 是 Faceguard 类中间基准

**状态**：3 件 🟡 pending deploy

### 2026-09-23 · Scale Coif 追溯修正 · 头 > 身 > 臂原则（2 件）

**背景**：新铁律头 > 身 > 臂暴露 Scale Coif o/o2 原决议 `45/15/35/1.7` 违反（arm 35 > body 15）。追溯修正为 body > arm，并与新增的 o3 (Steel Scale Coif) 建立 Brass < Alternating < Steel 三档阶梯。

| # | id | 游戏名 | 原 v2 决议 | **追溯修正 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_empire_helmet_o` | Imperial Brass Scale Coif | 45/15/35/1.7 | **48/28/15/1.8** |
| 2 | `AR_empire_helmet_o2` | Imperial Alternating Scale Coif | 45/15/35/1.7 | **48/28/15/1.8** |

**Scale Coif 三档阶梯**（Brass/Alternating < Steel）：
- Brass/Alternating: 48/28/15/1.8（本次追溯）
- Steel (o3): 55/30/18/2.0（TV/AR 尾单批同期定案）

**理由**：Scale material 是"金属片编织"结构，主要防护是头顶铁片 · 颈档次之 · 肩档最少（符合头 > 身 > 臂）。原 arm 35 是把 Scale Coif 当"mail coif"处理导致过高。

**状态**：2 件追溯修正 🔵 log-only

---

## Empire · Sagittarius 家族

### 2026-09-22 · Sagittarius 8 件 (Empire 弓手头盔)

**范围**：8 件 = 4 aventail (Cloth/Leather/Mail/Scale) × 2 品质 (基础/Gilded)

**背景**：Sagittarius = 罗马辅助弓手头盔，历史上是骑马弓手的轻型头盔。**vanilla 无 `sagittarius_*` 物品**，按家族设计 + Cloth/Leather/Mail 使用 vanilla suffix 表推导，Scale 为 OSA-only 独立设计。

**设计原则**：弓手需要轻量化，head 数值比重步兵头盔略低；Scale aventail 介于 Mail 和 Coif 之间（片状金属延伸）。

**8 件落地清单**：

| # | id | Aventail | Gilded | 前 h/b/a/wt | **后 h/b/a/wt** |
|---|---|---|---|---|---|
| 1 | `AR_empire_archer_helmet_b` | Cloth | No | 20/0/0/0.34 | **62/12/6/1.2** |
| 2 | `AR_empire_archer_helmet_b2` | Cloth | Yes | 20/0/0/0.34 | **64/12/6/1.2** |
| 3 | `AR_empire_archer_helmet_a` | Leather | No | 88/0/0/1.3 | **70/12/15/1.7** |
| 4 | `AR_empire_archer_helmet_a2` | Leather | Yes | 88/0/0/1.3 | **72/12/15/1.7** |
| 5 | `AR_empire_archer_helmet_c` | Mail | No | 79/34/29/3.11 | **82/15/25/2.8** |
| 6 | `AR_empire_archer_helmet_c2` | Mail | Yes | 79/34/29/3.11 | **84/15/25/2.8** |
| 7 | `AR_empire_archer_helmet_d` | Scale | No | 90/39/33/2.77 | **87/18/28/3.0** |
| 8 | `AR_empire_archer_helmet_d2` | Scale | Yes | 90/39/33/2.77 | **89/18/28/3.0** |

**新家族阶梯**：`62 → 64 → 70 → 72 → 82 → 84 → 87 → 89`（8 档全区分，Gilded 每档 +2 head）

**bug 修正**：
- Cloth 塌陷（20→62/64）
- Leather 从 88 head + 0 aventail 归位到 70/12/15（合理 aventail 结构）
- Mail/Scale body/arm 调低到 archer 级（15-18/25-28）

---

## Empire · Cataphract 家族

### 2026-09-22 · Cataphract 家族 10 件

**范围校验**：用户 in-game filter=Cataphract 显示 **14 件** = 12 OSA + 2 vanilla
- **vanilla 2 件不动**：`empire_guarded_lord_helmet` (130/30/0/3.7)、`imperial_goggled_helmet` (144/82/45/4.2)
- **已在 Spangenhelm 家族做过 2 件不动**：`DZ_empire_helmet_d` (123/10/0/3.5)、`ao_imperial_cataphracts_plumed_closed_mail_helmet` (144/82/45/4.2 — direct match vanilla goggled_helmet)
- **本批新做 10 件**

**Vanilla 参照**：
- `empire_lord_helmet` (123/10/0/3.5) — Noble Guard Helmet，标准 Cataphract-tier
- `empire_guarded_lord_helmet` (130/30/0/3.7) — Royal Cataphract Helmet
- `spiked_kettle_over_imperial_mail` (90/4/20/3.6) — Pointed Kettle 概念
- `imperial_nasal_helm` (97/12/25/2.2) — Legionary Helm (Byzantine Varangian 参照)

**10 件落地清单**：

| # | id | Name | Base + Dict | **新 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `DZ_empire_helmet_b` | Cataphract's Domed | empire_lord_helmet direct | **123/10/0/3.5** |
| 2 | `DZ_empire_helmet_c` | Plumed Cataphract's Domed | 同上 + Plumed 0 | **123/10/0/3.5** |
| 3 | `DZ_empire_helmet_i` | Feathered Pointed Kettle | 用户修订 · 填补 Cataphract 家族断层 | **110/12/25/3.5** |
| 4 | `DZ_empire_helmet_j` | Plumed Pointed Kettle | 同上 + Plumed 0 | **110/12/25/3.5** |
| 5 | `TV_empire_lord_helmet_j` | Cataphract's Closed Fluted | empire_lord_helmet + Closed dict (+5/+5/+3, +1wt) | **128/15/0/4.5** |
| 6 | `ao_imperial_cataphracts_closed_mail_helmet` | Cataphract's Closed Mail | 同上 | **128/15/0/4.5** |
| 7 | `ao_imperial_cataphracts_flamboyant_helmet` | Ornate Cataphract | empire_lord_helmet + Ornate/Flamboyant 0（decorative） | **123/10/0/3.5** |
| 8 | `ao_imperial_guarded_conical_nasal_helmet` | Guarded Cataphract's Conical Nasal | empire_guarded_lord_helmet direct + Nasal informal +3h | **133/30/0/3.7** |
| 9 | `ar_empire_cataphracts_helmet_a` | Plumed Cataphract | empire_lord_helmet + Plumed 0 | **123/10/0/3.5** |
| 10 | `varangian_guard_helmet_c` | Imperial Guard's Helmet With Stripped Cataphract Mail | 用户修订 · 拉到 Cataphract 均值 wt 3.5，保留 mail aventail | **115/12/25/3.5** |

**Cataphract 家族完整阶梯**（含 vanilla 和 Spangenhelm 已完成的）：
```
90 (Pointed Kettle ×2)
97 (Varangian Guard)
123 (Standard Cataphract ×5：Domed / Plumed Domed / Bent Conical / Ornate / Plumed)
128 (Closed Cataphract ×2：Closed Fluted / Closed Mail)
130 (vanilla Royal Cataphract)
133 (Guarded Conical Nasal)
144 (Goggled Closed / Plumed Spangen Over Closed Mail)
```

**克隆情况**：
- **5 件 at 123**（标准 Cataphract）：形状不同但 vanilla 无区分基础
- **2 件 at 90**（Pointed Kettle）：Feathered/Plumed 是 decorative
- **2 件 at 128**（Closed 变体）：Closed Mail vs Closed Fluted 视觉差异

**设计决策**：
- **Ornate/Flamboyant** 视为纯装饰（不入 dict），与 Domed 同档
- **Varangian Guard**（2026-09-22 用户修订）：原用 Legionary Helm base 太轻（97/2.2）与家族其他成员断层严重。修订到 115/12/25/3.5——head 略低于标准 Cataphract 123（"Stripped" 语义），保留 mail aventail 视觉，wt 达 Cataphract 均值 3.5
- **Pointed Kettle**（2026-09-22 用户修订）：原用 vanilla spiked_kettle_over_mail (90/4/20/3.6) 作 direct match，未考虑 "Cataphract's" 精工含义，导致 90 head 与家族断层严重（Varangian 115 → Kettle 90）。修订为 **110/12/25/3.5**——填补 Varangian 115 与 Kettle 之间的断层，保留 mail aventail (12/25)，wt 达 Cataphract 均值，"Cataphract's" 精工得到兑现
- **Plumed Spangen Over Close Mail**（2026-09-22 用户修订）：原 direct match vanilla goggled 144/82/45/4.2 太高——用户判断 Spangenhelm+闭合链甲的模块化结构比 Goggled 一体化设计次一档。降到 Guarded Conical Nasal 级 **133/30/25/3.7**（head/body/wt 同 Guarded，arm 保留 mail wrap 25 与 Guarded 区分）
- **Closed Mail Helmet arm 补正**（2026-09-22 用户修订）：`ao_imperial_cataphracts_closed_mail_helmet` 原 128/15/**0**/4.5 与名字 "Closed Mail" 矛盾（闭合链甲应有 arm 覆盖）。修订为 **128/15/25/4.5**，arm 补到 25 反映 mail wrap
- **Standard Cataphract arm 全线上调**（2026-09-22 用户挑战 · arm 分工规则修正）：用户指出 vanilla Goggled 有 arm=45，说明"arm=0 for Cataphracts" 规则过度简化。正确规则是"arm 反映头盔 mesh 可见 aventail 覆盖度"，很多 OSA Standard Cataphracts 视觉可能有 mail 短颈甲但名字未直说。**6 件回改**（前 → 后）：
  - `DZ_empire_helmet_b` (Domed): 123/10/0/3.5 → **123/15/20/3.5**
  - `DZ_empire_helmet_c` (Plumed Domed): 123/10/0/3.5 → **123/15/20/3.5**
  - `DZ_empire_helmet_d` (Bent Conical, Spangenhelm 家族): 123/10/0/3.5 → **123/15/20/3.5**
  - `ao_imperial_cataphracts_flamboyant_helmet` (Ornate): 123/10/0/3.5 → **123/15/20/3.5**
  - `ar_empire_cataphracts_helmet_a` (Plumed): 123/10/0/3.5 → **123/15/20/3.5**
  - `TV_empire_lord_helmet_j` (Closed Fluted): 128/15/0/4.5 → **128/20/25/4.5**（Closed 变体给更长 mail wrap）
  - 未动：`ao_imperial_guarded_conical_nasal_helmet` (133/30/0) —— Conical Nasal 面颊护无 mail 视觉暗示、vanilla 2 件不动

### 2026-09-23 · Secutor + Crested Faceguard 家族 arm 上调（9 件）

**触发**：用户提醒 "RBM 修改后的原版头盔也提供相当数量的 body/arm"——审计发现 ~60% vanilla 头盔有 arm，我用的 arm=0 类型（helmet_with_faceguard 系）虽正确匹配 vanilla 但可能对 OSA mesh 视觉有低估。

**审计结论**：
- Vanilla RBM 帝国头盔 arm 分布：0（19 件）/ 15-25（20 件）/ 40-45（5 件）
- 我 balanced OSA 里 arm=0 的 12 件都匹配 vanilla arm=0 类型
- **Secutor + Crested Faceguard 9 件**判定为可能低估（历史上 Secutor 常有短 mail collar；Crested 军用 Faceguard 有适度颈甲）

**上调 9 件**（前 → 后）：

| 家族 | id | 名称 | v6 → **v7** |
|---|---|---|---|
| Secutor 基础 | `AR_gladiator_helmet_c` | Secutor Helmet | 94/12/0/3.5 → **94/12/12/3.5** |
| Secutor 基础 | `AR_gladiator_helmet_c2` | Secutor Helmet With Feathers | 94/12/0/3.5 → **94/12/12/3.5** |
| Secutor 基础 | `AR_gladiator_helmet_a` | Gilded Secutor Helmet | 96/12/0/3.5 → **96/12/12/3.5** |
| Secutor 基础 | `AR_gladiator_helmet_a2` | Gilded Secutor Helmet With Feathers | 96/12/0/3.5 → **96/12/12/3.5** |
| Secutor Visored | `AR_gladiator_helmet_d` | Visored Secutor Helmet With Feathers | 99/17/0/4.5 → **99/17/15/4.5** |
| Secutor Visored | `AR_gladiator_helmet_b` | Gilded Visored Secutor Helmet With Feathers | 101/17/0/4.5 → **101/17/15/4.5** |
| Crested Faceguard | `AR_empire_crested_helm_a` | Imperial Crested Helmet With Faceguard | 94/12/0/3.5 → **94/12/15/3.5** |
| Crested Faceguard | `TV_empire_crested_helm_a` | Imperial Gilded Crested Helmet With Faceguard | 96/12/0/3.5 → **96/12/15/3.5** |
| Crested Faceguard | `AR_roman_helmet_d_plumed_c` | Gilded Crested Helmet with Faceplate | 96/12/0/3.5 → **96/12/15/3.5** |

**保持 arm=0**（合理）：
- Cap Helmet 40/0/0 — 基础金属帽无 aventail
- Ridge Helmets 3 件 arm=2 — Ridge 结构无 aventail 逻辑（Battle Crown vanilla 也 arm=0）

**状态**：9 件 🟡 pending deploy

**状态**：10 件 🟡 pending deploy

### 2026-09-23 · Cataphract 家族 v3 · body/arm 用 Goggled 反推重设（12 件）

**背景**：用户 2026-09-23 审 Lord 家族提案时指出 "Imperial 前缀 = 领主 + 双层内衬" 铁律未在 Cataphract 家族充分体现——v2 现值 body 12-30 / arm 0-25 明显不匹配 elite cavalry + full mail liner 定位。要求以 vanilla `imperial_goggled_helmet` **144/82/45** 为顶点参照，按 head 档位反推各档 body/arm，接近但不越 Goggled。

**Goggled 反推阶梯**：
```
head 110 → body 56 / arm 32   (缺口 -12/-13 vs Goggled)
head 115 → body 58 / arm 33
head 123 → body 62 / arm 36
head 128 → body 65 / arm 38
head 133 → body 70 / arm 40   (缺口 -14/-5 vs Goggled)
—— vanilla Goggled 144 / 82 / 45（顶点参照）——
```

**12 件 v3 body/arm 修订清单**（含 2 件 Spangenhelm 家族登记但结构属 Cataphract）：

| # | id | 游戏名 | v2 现值 h/b/a/wt | **v3 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `DZ_empire_helmet_b` | Cataphract's Domed | 123/15/20/3.5 | **123/62/36/3.5** |
| 2 | `DZ_empire_helmet_c` | Plumed Cataphract's Domed | 123/15/20/3.5 | **123/62/36/3.5** |
| 3 | `DZ_empire_helmet_i` | Cataphract's Feathered Pointed Kettle | 110/12/25/3.5 | **110/56/32/3.5** |
| 4 | `DZ_empire_helmet_j` | Cataphract's Plumed Pointed Kettle | 110/12/25/3.5 | **110/56/32/3.5** |
| 5 | `TV_empire_lord_helmet_j` | Cataphract's Closed Fluted | 128/20/25/4.5 | **128/65/38/4.5** |
| 6 | `ao_imperial_cataphracts_closed_mail_helmet` | Cataphract's Closed Mail | 128/15/25/4.5 | **128/65/38/4.5** |
| 7 | `ao_imperial_cataphracts_flamboyant_helmet` | Ornate Cataphract | 123/15/20/3.5 | **123/62/36/3.5** |
| 8 | `ao_imperial_guarded_conical_nasal_helmet` | Guarded Cataphract's Conical Nasal | 133/30/0/3.7 | **133/70/40/3.7** |
| 9 | `ar_empire_cataphracts_helmet_a` | Plumed Cataphract | 123/15/20/3.5 | **123/62/36/3.5** |
| 10 | `varangian_guard_helmet_c` | Stripped Cataphract Mail | 115/12/25/3.5 | **115/58/33/3.5** |
| +1 | `DZ_empire_helmet_d`（Spangenhelm 家族登记）| Cataphract's Bent Conical | 123/15/20/3.5 | **123/62/36/3.5** |
| +2 | `ao_imperial_cataphracts_plumed_closed_mail_helmet`（Spangenhelm 家族登记）| Plumed Spangen Over Closed Mail | 133/30/25/3.7 | **133/70/40/3.7** |

**Tier 校核**：全 T5（head 不动，仅 body/arm 调整；raw 提升 45-50，vs Goggled raw 300 仍差 30-80）。

**关键调整幅度**：
- Standard Cataphract (h=123, 5 件): body 15→62 (+47) · arm 20→36 (+16)
- Pointed Kettle (h=110, 2 件): body 12→56 (+44) · arm 25→32 (+7)
- Closed Cataphract (h=128, 2 件): body 15-20→65 (+45-50) · arm 25→38 (+13)
- Guarded Cataphract 顶档 (h=133, 2 件): body 30→70 (+40) · arm 0-25→40 (+15-40)
- Stripped Cataphract (h=115, 1 件): body 12→58 (+46) · arm 25→33 (+8)

**保留精英差距**：body 缺 12-26 / arm 缺 5-13 vs Goggled——符合用户"接近但不比肩"定位。

**状态**：12 件 🔵 log-only（决议归档，XML 未动 · v2 XML 值保持不变）

---

## Empire · 帽/头饰（轻装）

### 2026-09-23 · 帽/头饰家族 19 件 · 决议已定案（log-only，未改 XML）

**背景**：v2 手工审推进到"非头盔"轻装分类。19 件全为平民/仪式性头饰，护甲价值低、v1 flat buff 已覆盖到大致合理档位。用户 policy：**决议记入 log 作为"已审"标记，不改 XML**（cosmetic 类物品 v1 现值可接受，避免 XML churn）。

**vanilla RBM 锚点参照**：
- `pilgrim_hood` (Empire Cloth, 0.3 wt) 7/5/0
- `nordic_civilian_hat_fur_brim` (Nord Cloth, 1.0 wt) 7/0/0
- `battania_civil_hood` (Battania Cloth, 1.0 wt) 11/0/0
- `fur_hood` (Khuzait Leather, 1.2 wt) 12/0/0
- `peaked_fur_hood` (Khuzait Leather, 1.2 wt) 14/0/0
- `leather_cap` (vanilla Leather, 0.4 wt) 14/0/0

**决议清单**（19 件）：

| 组 | id | 名称 | mat | v1 当前 h/wt | **v2 决议 h/b/a/wt** | 依据 |
|---|---|---|---|---|---|---|
| 农民草帽 | `AR_hat_a` | Straw Hat | Cloth | 7/0.46 | **8/0/0/0.5** | `nordic_civilian_hat_fur_brim` 7 微升（宽帽檐） |
| 农民草帽 | `AR_hat_b` | Straw Hat With Feather | Cloth | 7/0.46 | **8/0/0/0.5** | 同上（羽饰 0） |
| Roman 布/皮平民帽 | `AR_roman_hat_d` | Conical Cloth Hat | Leather | 4/0.41 | **6/0/0/0.4** | Cloth 帽 最低档 |
| Roman 布/皮平民帽 | `AR_roman_hat_e` | Plain Conical Cloth Hat | Leather | 4/0.41 | **6/0/0/0.4** | 同上 |
| Roman 布/皮平民帽 | `AR_roman_hat_a` | Decorated Hat | Leather | 4/0.41 | **8/0/0/0.5** | Leather 装饰帽中档 |
| Roman 布/皮平民帽 | `AR_roman_hat_a2` | Feathered Decorated Hat | Leather | 4/0.41 | **8/0/0/0.5** | 同上（羽饰 0） |
| Roman 布/皮平民帽 | `AR_roman_hat_c` | Decorated Leather Hat | Leather | 4/0.41 | **10/0/0/0.5** | Leather 装饰帽上档 |
| Roman 布/皮平民帽 | `AR_roman_hat_f` | Conical Leather Hat | Leather | 4/0.41 | **10/0/0/0.5** | 同上 |
| 毛皮帽 | `AR_roman_hat_b` | Imperial Fur Hat | Leather | 8/0.41 | **12/0/0/1.0** | 直匹配 vanilla `fur_hood` |
| 毛皮帽 | `AR_roman_hat_b2` | Imperial Conical Fur Hat | Leather | 8/0.41 | **14/0/0/1.0** | 直匹配 vanilla `peaked_fur_hood` |
| Phrygian 弯锥帽 | `TV_phrygian_cap_b` | Plain Bent Conical Hat | Cloth | 5/0.46 | **7/0/0/0.4** | 平民软帽最低档 |
| Phrygian 弯锥帽 | `TV_phrygian_cap_a` | Bent Conical Hat | Cloth | 5/0.46 | **8/0/0/0.4** | 中档 |
| Phrygian 弯锥帽 | `AR_phrygian_cap_a` | Banded Bent Conical Hat | Cloth | 5/0.46 | **9/0/0/0.4** | 加带上档 |
| 头带/月桂 | `AR_empire_laurel_a` | Laurel Headband | Leather | 8/0.29 | **4/0/0/0.2** | 罗马凯旋月桂纯仪式，v1 h=8 过高 |
| 头带/月桂 | `AR_empire_laurel_a2` | Vine Headband | Leather | 8/0.29 | **4/0/0/0.2** | 同上（藤蔓变体） |
| 头带/月桂 | `AR_headband_a` | Wrapped Headband | Leather | 8/0.29 | **5/0/0/0.2** | 布带包头略强于月桂 |
| 朝圣兜帽 | `AR_bandit_hood_a` | Open Pilgrim's Hood | Cloth | 4/0.1 | **7/5/0/0.3** | 直匹配 vanilla `pilgrim_hood`（Empire Cloth） |
| 帝国头冠 | `tiara_x` | Imperial Tiara | Plate | 15/0.8 | **18/0/0/0.6** | 金属头环覆盖不全，比 `leather_cap` 14 略高 |
| 节庆帽 | `AR_satan_hat` | Festive Hat | Leather | 4/0.41 | **3/0/0/0.4** | 派对/彩装帽无护 |

**关键修正点**：
- 3 件直匹配 vanilla（`AR_roman_hat_b/b2` → fur_hood 12/14；`AR_bandit_hood_a` → pilgrim_hood 7/5/0）
- 3 件月桂/头带 h 从 v1 的 8 下调到 4-5（更符合纯仪式性物品定位）
- 多数 Roman/Phrygian 帽子从 v1 的 h=4-7 上调到 6-14 合理档位

**状态**：19 件 🔵 log-only（决议已记，XML 未动 · 将来若发现 v1 值实机不合手感再单独批量改）

---

## Empire · Roman Helmet 家族

### 2026-09-23 · Roman Helmet 家族 12 件（vanilla 直匹配修订）

**背景**：v2 手工审推进到 Roman Helmet 家族（`AR_roman_helmet_*` + `roman_helmet_z`）。**首版提案违反铁律**——把 Faceguard 家族 vanilla anchor 94/12/0 套到全 12 件顶档，被用户驳回（"过于夸张"）。修订版按 vanilla `helmet_with_faceguard` / `tall_helmet` / `plumed_helmet` **三档 direct match** + Scout Cap 家族 fallback 落地。

**用户 in-game 结构确认**（决定 aventail/faceguard 分档）：
1. Scout 系 = 铁质头盔 + 毛冬帽装饰（毛帽装饰不加护）
2. 所有 Scout 系带 faceguard，无链甲/皮甲内衬
3. Plumed Helmet 系带 faceguard（非 open crested 无面甲版）
4. Tall Helmet Over Stripped Cloth = 铁盔 + 铁片皮革条环绕后脑三面（左后/正后/右后 partial aventail）

**vanilla RBM 三档锚点**：
- `helmet_with_faceguard` (3.5/94/12/0) — Helmet with Faceguard
- `tall_helmet` (1.8/84/0/0) — Tall Helmet
- `plumed_helmet` (2.9/104/24/0) — Plumed Helmet（比 Faceguard 高一档 · +10 head +12 body）
- （备参 `imperial_nasal_helm` 2.2/97/12/25 — Legionary Helm，若视觉带鼻梁）

**12 件落地清单**（v1 现值 = XML 实际值 · v1 脚本已 buff 的部分保留原本 body/arm 显示）：

| # | id | 游戏名 | **v1 XML 现值 h/b/a/wt** | **v2 决议 h/b/a/wt** | vanilla 依据 |
|---|---|---|---|---|---|
| 1 | `AR_roman_helmet_a` | Imperial Open Helmet With Faceguard | 20/0/0/0.88 | **94/12/0/3.4** | ✅ `helmet_with_faceguard` 直匹配 |
| 2 | `AR_roman_helmet_a_fur_a` | Scout's Ridged Cap | 20/0/0/0.88 | **80/10/0/3.4** | Faceguard 家族 -14 反映 Cap 小 mesh；毛帽装饰 0 |
| 3 | `AR_roman_helmet_a_fur_b` | Scout's Banded Cap | 20/0/0/0.88 | **82/10/0/3.4** | 同 #2 + Banded +2 |
| 4 | `AR_roman_helmet_b_fur_a` | Scout's Helmet With Faceguard | 84/36/31/3.2 | **94/12/0/3.7** | ✅ `helmet_with_faceguard` 直匹配（Scout 毛帽装饰 0）|
| 5 | `AR_roman_helmet_b_fur_b` | Scout's Tall Helmet | 79/34/29/3.2 | **90/12/0/3.7** | `tall_helmet` 84 + Scout faceguard +6 head +12 body |
| 6 | `AR_roman_helmet_b_fur_c` | Scout's Plumed Helmet | 80/34/30/3.16 | **104/24/0/3.7** | ✅ `plumed_helmet` 直匹配（Scout 毛帽装饰 0）|
| 7 | `AR_roman_helmet_b_plumed` | Plumed Helmet With Faceguard | 79/34/29/3.2 | **104/24/0/3.7** | ✅ `plumed_helmet` 直匹配 |
| 8 | `AR_roman_helmet_b_plumed_b` | Plumed Feathered Helmet With Faceguard | 79/34/29/3.2 | **104/24/0/3.7** | 同 #7（Feathered 装饰 0）|
| 9 | `AR_roman_helmet_d_plumed_a` | Decorated Plumed Helmet | 90/39/33/3.37 | **104/24/0/3.9** | ✅ `plumed_helmet` 直匹配（Decorated 品质 0 armor）|
| 10 | `AR_roman_helmet_d_plumed_b` | Gilded Plumed Helmet | 80/34/30/3.33 | **106/24/0/3.9** | `plumed_helmet` + Gilded 品质 +2 head（品质字典最高档）|
| 11 | `AR_roman_helmet_d_strips` | Plumed Tall Helmet Over Stripped Cloth | 79/34/29/3.03 | **84/8/18/3.5** | `tall_helmet` 84 base + Stripped Cloth partial aventail（后脑三面 body 8 arm 18）|
| 12 | `roman_helmet_z` | Tall Helmet Over Stripped Cloth | 78/34/29/2.99 | **84/8/18/3.5** | 同 #11（无 Plumed 装饰）|

**Tier 校核**：全 12 件 T5（与 vanilla 三档锚点 `helmet_with_faceguard` / `tall_helmet` / `plumed_helmet` 一致）。家族梯度靠**绝对头档差异**表达（80-106 spread），非 tier 分层——这符合公式在 h≥38 clamp T5 的特性。

**关键设计决策**：
- **Plumed 系上调**（首版 94 → 104）：vanilla `plumed_helmet` 是 **104/24/0**（比 Faceguard 高一档 · body 24 反映 shoulder 延伸），首版误把 Plumed 当作 Faceguard 变体，纠正后全 5 件 Plumed（#6/#7/#8/#9/#10）走 104 base
- **Scout Tall Helmet 妥协**（`tall_helmet` 84 + Scout faceguard +6/+12）：vanilla `tall_helmet` 无 faceguard，用户点 #2 明确 Scout 系有 faceguard，故不能直匹配 84 而须加 faceguard 结构增量
- **Stripped Cloth 系轻档 · body/arm 下调**（v1 现 34/29 → 决议 8/18）：v1 脚本把 Stripped Cloth 当作 mail-tier aventail buff 到 34/29，但用户点 #4 明确结构是"皮革条+铁片后脑三面"（partial laced_cloth 类），应低于 mail (body 12-22 / arm 20-25)。v2 决议 body 8 / arm 18 反映真实 partial aventail

**v1 脚本行为发现**（首次系统对比 XML 与 OSA 源）：v1 脚本对 `AR_roman_helmet_*` 家族按 mesh 前缀分组 buff：`_a` 系（3 件）仅微调 weight 未加护甲；`_b`/`_d` 系（8 件）按 Chainmail/Plate 白名单加 body ≈ head×0.43 + arm ≈ head×0.37 的 aventail 延伸公式，产生 body 34-39 / arm 29-33 的中档链甲颈甲效果——**这解释了为何 #11 #12 现 XML 已有 34/29 但视觉是皮革条**：v1 脚本把它们当作 mail aventail 误判。v2 决议按视觉真实结构下调到 partial cloth aventail 尺度。

**状态**：12 件 🔵 log-only（用户 2026-09-23 拍板 · 决议归档不改 XML · v1 XML 现值保持不动）

**归档理由**：v1 脚本已对 `_b`/`_d` 8 件做过实质 buff（h=78-90 + body/arm 34/29），实机战力已接近 v2 决议方向（虽然 body/arm 比例反了）；`_a` 3 件仍是 h=20 无 buff 状态（属 v2 决议明显未覆盖的漏洞）。用户 policy：批量 XML 改动优先级低于其它待办项目，本家族决议记入 log 作为"已审核"标记即可，实机若发现 `_a` 3 件（Open Faceguard / Scout Ridged Cap / Scout Banded Cap）战力过弱再单独补上。

### 2026-09-23 · Roman Helmet 追溯修正 · 头 > 身 > 臂原则（2 件）

**背景**：新铁律 头 > 身 > 臂（2026-09-23 用户拍板）暴露 Stripped Cloth 系 #11 #12 原决议 `84/8/18` 违反（arm 18 > body 8）。追溯修正为 body > arm。

| # | id | 游戏名 | 原 v2 决议 | **追溯修正 h/b/a/wt** |
|---|---|---|---|---|
| 11 | `AR_roman_helmet_d_strips` | Plumed Tall Helmet Over Stripped Cloth | 84/8/18/3.5 | **84/20/10/3.5** |
| 12 | `roman_helmet_z` | Tall Helmet Over Stripped Cloth | 84/8/18/3.5 | **84/20/10/3.5** |

**理由**：铁片皮革条覆盖后脑三面 = 颈档主导（body 20）+ 少量肩档延伸（arm 10）。raw 总和不变，只是 body/arm 分布归正符合头 > 身 > 臂。

**状态**：2 件追溯修正 🔵 log-only

---

## Empire · Lord/Guarded Lord 家族

### 2026-09-23 · Lord/Guarded Lord 家族 10 件 · vanilla 直匹配 + Imperial 双层内衬修订

**背景**：v2 手工审推进到 Lord/Guarded Lord 家族。v1 脚本对本家族 10 件按 Chainmail/Plate 白名单批量 buff（h 74-95、body 32-41、arm 27-35），头档偏低且 aventail 尺度错位。首版 v2 提案曾把 body/arm 降到 12-14/0-20（回归 Battle Crown 类轻档 Ridge），被用户 2026-09-23 驳回："Imperial 前缀 = 领主装备 + 双层内衬，body/arm 不能低"。修订版按 vanilla 高档 anchor（`heavy_nasalhelm_over_imperial_mail` 124/36/20 + `imperial_goggled_helmet` 144/82/45 顶点参照）反推 body/arm 60+/35+ 档次。

**vanilla RBM 关键锚点**：
- `empire_lord_helmet` 123/10/0/3.5 — Noble Guard Helmet
- `empire_guarded_lord_helmet` 130/30/0/3.7 — Royal Cataphract Helmet
- `empire_helmet_with_metal_strips` 120/14/20/3.8 — Lord Helmet with Metal Strips
- `empire_jewelled_helmet` 125/14/20/3.8 — Imperial Jeweled Helmet
- `heavy_nasalhelm_over_imperial_mail` 124/36/20/3.6 — Heavy Nasal + full mail
- `imperial_goggled_helmet` 144/82/45/4.2 — Goggled Cataphract（顶点参照）

**10 件落地清单**：

| # | id | 游戏名 | v1 XML 现值 h/b/a/wt | **v3 决议 h/b/a/wt** | vanilla 依据 |
|---|---|---|---|---|---|
| 1 | `AR_Empire_Lord_Guarded_Face_Helmet` | Imperial Guarded Lord Helmet with Faceplate | 95/41/35/2.99 | **140/70/40/4.5** | `empire_guarded_lord_helmet` 130 + Faceplate +5 + Imperial +5 · full mail liner（接近但不越 Goggled 82/45）|
| 2 | `AR_empire_lord_helmet_d` | Gilded Imperial Guarded Lord Helmet | 90/39/33/2.84 | **132/62/36/3.9** | Guarded Lord 130/30/0 + Gilded +2 head · Imperial mail liner body/arm 大幅上调 |
| 3 | `AR_empire_lord_helmet_e` | Imperial Feathered Jeweled Helmet | 88/38/33/2.91 | **125/60/36/3.8** | `empire_jewelled_helmet` 125/14/20 direct + Imperial mail liner |
| 4 | `AR_empire_lord_helmet_g` | Imperial Silvered Ridge Helmet | 85/37/31/2.38 | **125/62/38/3.5** | Heavy Nasal-mail 124/36/20 base + Silvered +1 head · Ridge 结构 body ↑ |
| 5 | `AR_empire_lord_helmet_h` | Imperial Jeweled Gilded Ridge Helmet | 79/34/29/1.61 | **126/60/35/2.5** | Heavy Nasal-mail 124 + Gilded +2 · 轻 mesh body/arm 稍减 |
| 6 | `AR_empire_lord_helmet_i` | Imperial Plumed Decorated Banded Helmet With Metal Strips | 74/32/27/1.69 | **122/60/35/2.7** | `empire_helmet_with_metal_strips` 120/14/20 base + Banded 轻 mesh + Imperial mail liner |
| 7 | `AR_empire_lord_helmet_j` | Imperial Jeweled Ridge Helmet | 79/34/29/1.61 | **122/60/35/2.5** | Heavy Nasal-mail 124 轻档（Jeweled 已 vanilla 名中）|
| 8 | `TV_empire_lord_helmet_a` | Imperial Fluted Helmet With Metal Strips | 85/37/31/2.91 | **125/62/38/3.8** | `empire_helmet_with_metal_strips` 120 base + Fluted +5 head · Imperial mail liner |
| 9 | `TV_empire_lord_helmet_d` | Imperial Decorated Silvered Ridge Helmet | 85/37/31/2.38 | **125/62/38/3.5** | 同 #4 |
| 10 | `TV_empire_lord_helmet_e` | Imperial Decorated Gilded Ridge Helmet | 85/37/31/2.38 | **126/62/38/3.5** | Heavy Nasal-mail 124 + Gilded +2 head |

**Tier 校核**：全 T5。**vs Goggled 缺口**：head -4~-22 / body -12~-22 / arm -5~-10（保留精英差距）。

**字典 v9 候选**：`Fluted` +5 head（Face Plate 同档 · 结构性金属沟槽加固）——立项标记，待用户确认后正式入字典。

**关键设计决策**：
- **首版驳回历史**：v2 首提案把 Ridge Helmet 锚定为 `empire_battle_crown_west/north`（bare Ridge，body 12 arm 0），被用户点破"Imperial 双层内衬"应该体现在 body/arm。修订版换锚为 `heavy_nasalhelm_over_imperial_mail` 124/36/20（Heavy Nasal + full mail）+ Imperial mail liner 增强
- **Goggled 天花板对齐**：Guarded Lord + Faceplate（#1）140/70/40 是家族顶档，raw 238，仍低于 Goggled raw 300 · 保留 elite 差距
- **body/arm 60+/35+ 统一档**：从 v1 XML 的 32-41 body / 27-35 arm 大幅上调，反映 Imperial 领主装备的全 mail 内衬结构

**状态**：10 件 🔵 log-only（决议归档，XML 未动 · 用户 2026-09-23 拍板"同 Roman Helmet 流程"）

### 2026-09-23 · Lord 家族补丁 · 漏审 2 件（AR_empire_lord_helmet_a/b）

**背景**：审 Elite 家族时发现早前 Lord 家族 v3（10 件）漏了 `AR_empire_lord_helmet_a/b` 两件标准 Lord Helmet 变体（Faceguard + Faceplate）。补齐。

**注**：还发现 `AR_empire_lord_helmet_c` 完全**不在 override XML 里**（v1 脚本跳过），属漏洞——需用户实机确认此 id 存在与否，若存在再单独归档。

**2 件补 v3 落地清单**：

| # | id | 游戏名 | v1 XML 现值 | **v3 决议 h/b/a/wt** | vanilla 依据 |
|---|---|---|---|---|---|
| 补 1 | `AR_empire_lord_helmet_a` | Imperial Lord Helmet With Faceguard | 90/39/33/2.76 | **127/60/35/3.5** | `empire_lord_helmet` 123 + Faceguard 字典 +4 head + Imperial full mail liner |
| 补 2 | `AR_empire_lord_helmet_b` | Imperial Lord Helmet With Faceplate | 92/40/34/2.84 | **128/62/38/3.7** | `empire_lord_helmet` 123 + Faceplate 字典 +5/+5/+3 + Imperial full mail liner |

**Tier 校核**：全 T5。

**状态**：2 件 🔵 log-only（决议归档，XML 未动 · 补 Lord 家族 v3 遗漏）

---

## Empire · Nasalhelm 家族（纯 Nasal，非 Hybrid）

### 2026-09-23 · Nasalhelm 家族 3 件 · vanilla 直匹配 + Imperial 双层内衬修订

**背景**：v2 手工审推进到"纯 Nasalhelm"家族。用户 2026-09-23 主动指出遗漏——15 件 empire "Nasal" 相关物品中，12 件已散落在其他家族定案：
- **Hybrid Nasal-Spangenhelm**（5 件）在 Spangenhelm 家族：`ao_imperial_nasal_spangenhelmet_with_mail` · `ao_imperial_heavy_nasal_spangenhelm_over_mail_coif` · `ao_imperial_nasal_spangenhelm_with_leather_strips` · `ao_imperial_closed_mail_nasal_spangenhelmet` · `ao_imperial_crowned_nasal_helmet`
- **Cataphract Guarded Conical Nasal**（1 件）在 Cataphract 家族：`ao_imperial_guarded_conical_nasal_helmet`
- **Southern/Desert Nasalhelm**（3+1 件）在 Desert 家族：`AR_empire_desert_helmet_a/b/d` · `bronze_aserai_helm`
- **Heavy Nasalhelm**（2 件）在杂项：`AR_empire_helmet_a/b`

剩 3 件"纯 Nasalhelm"未定案，本次补齐。

**Nasalhelm 家族 vanilla 参照梯度**（11 件）：
```
55 (light cloth) → 87-90 (mid mail) → 97 (Legionary Helm) → 97-105 (Heavy w/ cloth/leather) → 124 (Heavy w/ mail 顶档)
```

**关键锚点**：
- `imperial_nasal_helm` 97/12/25/2.2 — Legionary Helm（Nasal 基础锚点）
- `heavy_nasalhelm_over_imperial_mail` 124/36/20/3.6 — Heavy Nasal + full mail（Nasal 家族顶档）
- `heavy_nasalhelm_over_imperial_padding` 100/41/25/3.2 — 中档参照

**3 件落地清单**：

| # | id | 游戏名 | v1 XML 现值 h/b/a/wt | **v2 决议 h/b/a/wt** | vanilla 依据 |
|---|---|---|---|---|---|
| 1 | `ao_imperial_nasal_helmet` | Imperial Nasal Helmet | 77/33/28/1.56 | **97/25/28/2.2** | ✅ `imperial_nasal_helm` 97/12/25/2.2 direct + Imperial 双层内衬 body 12→25 arm 25→28 |
| 2 | `ao_imperial_guarded_nasal_helmet` | Imperial Guarded Nasal Helmet | 82/35/30/1.54 | **105/40/30/2.6** | Legionary 97 base + Guarded 结构 +8 head · Imperial mail liner body/arm 靠拢 Heavy Nasal（对齐 vanilla `heavy_nasalhelm_over_imperial_padding` 100/41/25）|
| 3 | `ao_imperial_guarded_decorated_nasal_helmet` | Imperial Guarded Decorated Nasal Helmet | 88/38/33/1.38 | **107/40/30/2.6** | 同 #2（Decorated 装饰 0）+ mesh 差异 +2 head（v1 现值差异反映 mesh 略大）|

**Tier 校核**：全 T5。**vs Nasal 顶档缺口**：raw 169-198 vs Heavy Nasal 顶档 raw 204.8（差 -6 ~ -35，全部低于 Nasal 家族天花板）。

**字典 v9 候选**：`Guarded` +8 head（Face Guard/Ridge +4 与 Cataphracts +7 之间 · 结构性面颊/颈护加强）——立项标记。

**关键设计决策**：
- **wt 修正**：v1 XML wt 1.4-1.6 偏轻（Nasal + mail liner 应 2.2-3.0），v2 归正到 2.2-2.6 与 Legionary 和 Heavy Nasal 中间档一致
- **body/arm 反映 Imperial 双层**：Legionary 12/25 → OSA 25/28（body 大幅上调反映完整 mail 内衬）· Guarded 版进一步升到 40/30 靠拢 Heavy Nasal 尺度
- **家族天花板不越权**：3 件 raw 全部低于 vanilla Nasal 顶档 `heavy_nasalhelm_over_imperial_mail` 204.8，严格符合铁律

**状态**：3 件 🔵 log-only（决议归档，XML 未动 · 与 Lord/Cataphract v3 同批 2026-09-23 用户拍板）

### 2026-09-23 · Nasalhelm 追溯修正 · 头 > 身 > 臂原则（1 件）

**背景**：新铁律 头 > 身 > 臂 暴露 #1 原决议 `97/25/28` 违反（arm 28 > body 25）。追溯修正为 body > arm。

| # | id | 游戏名 | 原 v2 决议 | **追溯修正 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `ao_imperial_nasal_helmet` | Imperial Nasal Helmet | 97/25/28/2.2 | **97/28/22/2.2** |

**理由**：Imperial mail liner 主要覆盖颈档（body 28）+ 少量肩档延伸（arm 22）。raw 总和不变，只是 body/arm 分布归正。

**状态**：1 件追溯修正 🔵 log-only

---

## Empire · Elite（Evocati/Varangian/Echerian）家族

### 2026-09-23 · Elite 家族 4 件 · vanilla 顶档参照 + 双同名 helm 处理

**背景**：v2 手工审推进到 Elite 顶档家族——Evocati（罗马再入伍元老兵）· Varangian Guard（拜占庭皇家禁卫）· Echerian Elite（皇家精锐）。审查时发现 XML 里 **2 件同名 "Imperial Closed Guarded Lord Helmet"**（`echerian_elite_helm` + `imperial_lord_helm_mail`），必须同数值处理，故实际共 4 件。

**vanilla 顶档参照**：
- `empire_lord_helmet` 123/10/0/3.5 — Noble Guard Helmet
- `empire_guarded_lord_helmet` 130/30/0/3.7 — Royal Cataphract Helmet
- `imperial_goggled_helmet` 144/82/45/4.2 — Goggled Cataphract（顶点参照）

**4 件落地清单**：

| # | id | 游戏名 | v1 XML 现值 h/b/a/wt | **v2 决议 h/b/a/wt** | vanilla 依据 |
|---|---|---|---|---|---|
| 1 | `ao_imperial_evocati_helmet` | Imperial Evocatus' Helmet | 81/35/30/1.38 | **122/55/37/3.0** | `empire_lord_helmet` 123 tier 靠拢 + Imperial full mail liner · 用户 2026-09-23 指定 120+/50+/35+ 档 |
| 2 | `varangian_guard_helmet_a` | Imperial Guard's Helmet With Closed Plated Mail | 54/23/20/2.38 | **138/70/42/5.0** | Goggled 144/82/45 参照 · Varangian royal guard 顶档 · Chainmail 全 mail 构造 wt 5.0（源 5.8）|
| 3 | `echerian_elite_helm` | Imperial Closed Guarded Lord Helmet | 90/39/33/2.84 | **140/70/40/4.5** | `empire_guarded_lord_helmet` 130 + Closed 字典 +5/+5/+3 + Imperial full mail liner |
| 4 | `imperial_lord_helm_mail` | **Imperial Closed Guarded Lord Helmet**（同显示名）| 92/40/34/2.84 | **140/70/40/4.5** | 同 #3（同显示名 = 同数值 · 两件视觉相同）|

**Tier 校核**：全 T5。vs Goggled raw 300：#1 -62 · #2 -26 · #3/4 -22（保留精英差距）

**关键设计决策**：
- **双同名 helm 处理**（用户 in-game 观察）：`echerian_elite_helm` 与 `imperial_lord_helm_mail` 都用 "Imperial Closed Guarded Lord Helmet" 显示名 → 同视觉必须同数值，一并归档
- **Evocatus 三次调档**：v1 XML 81/35/30 → v2 首提案 110/45/32 → 用户要求 120+/50+/35+ → 最终 **122/55/37/3.0**（elite veteran 拿领主级装备定位，靠拢 `empire_lord_helmet` 123 Noble Guard）
- **Varangian Chainmail wt 修正**：v1 XML wt 2.38 掉太多（v1 script bug），v2 归正到 5.0 反映"Closed Plated Mail"全 mail 构造真实重量

**状态**：4 件 🔵 log-only（决议归档，XML 未动）

---

## Empire · Kettle 家族（Roundkettle + Pointed/Spiked Kettle）

### 2026-09-23 · Kettle 家族 11 件 · vanilla 直匹配 + 头 > 身 > 臂原则首用

**背景**：v2 手工审推进到 Kettle 家族——普通士兵头盔（非精英级），跨两大 mesh：Roundkettle（4 件）+ Pointed/Spiked Kettle（7 件）。首版提案完全按 vanilla mail wrap 结构（arm > body）落地被用户 2026-09-23 驳回："所有头盔应当都需要做到头甲 > 身甲 > 臂甲"——由此立**头 > 身 > 臂铁律**，作为所有头盔设计基调。本家族是新铁律首用。

**Kettle 家族 vanilla 参照**：
- `roundkettle_over_imperial_leather` 74/0/0/1.3 — Light Roundkettle
- `roundkettle_over_imperial_mail` 92/0/20/3.3 — Roundkettle + mail wrap（**vanilla 本身违反头>身>臂**：body 0 arm 20）
- `spiked_kettle_over_imperial_padding` 60/3/17/1.5 — 最低档 Pointed
- `spiked_kettle_over_imperial_mail` 90/4/20/3.6 — Pointed 顶档 mail（同 vanilla 违反：body 4 arm 20）
- `ironlame_nasalhelm_over_imperial_coif` 85/22/45/4.1 — coif 全 mail 罩参照
- `ironlame_spiked_kettle_over_mail` 73/0/25/3.8

**11 件落地清单**：

#### 子群 A · Roundkettle 系（4 件）

| # | id | 游戏名 | v1 XML 现值 | **v2 决议 h/b/a/wt** | vanilla 依据 |
|---|---|---|---|---|---|
| 1 | `ao_imperial_kettlehelm_with_leather` | Imperial Roundkettle With Leather | 20/0/0/0.46 | **74/20/12/1.5** | `roundkettle_over_imperial_leather` 74 + Imperial 双层内衬（body 20 > arm 12 · 头>身>臂）|
| 2 | `ao_imperial_kettlehelm_with_mail` | Imperial Roundkettle With Mail | 95/0/0/1.8 | **92/28/22/3.3** | `roundkettle_over_imperial_mail` 92 base + Imperial mail 双层（body/arm 从 vanilla 0/20 归正为 28/22）|
| 3 | `ao_imperial_kettlehelm_over_mail_coif` | Imperial Roundkettle Over Mail Coif | 79/34/29/1.56 | **90/42/30/3.7** | Roundkettle + full mail coif · 完整链甲头罩 |
| 4 | `DZ_empire_helmet_a` | Imperial Mailled Kettle Helmet | 90/39/33/3.07 | **92/28/22/3.3** | 同 #2 |

#### 子群 B · Pointed/Spiked Kettle 系（7 件）

| # | id | 游戏名 | v1 XML 现值 | **v2 决议 h/b/a/wt** | vanilla 依据 |
|---|---|---|---|---|---|
| 5 | `DZ_empire_helmet_e` | Imperial Pointed Kettle Over Padding | 86/37/32/2.14 | **60/18/12/1.5** | `spiked_kettle_over_imperial_padding` 60 + Imperial padding 双层（v1 h=86 是 script over-buff，按铁律回归 vanilla）|
| 6 | `DZ_empire_helmet_f` | Imperial Pointed Kettle Over Leather | 74/32/27/1.92 | **68/20/15/1.8** | 介于 padding 60 与 mail 90 之间 · 皮革 double-layer |
| 7 | `DZ_empire_helmet_g` | Imperial Pointed Kettle Over Mail | 81/35/30/2.38 | **90/28/22/3.6** | `spiked_kettle_over_imperial_mail` 90 base + Imperial mail 双层（body/arm 从 vanilla 4/20 归正）|
| 8 | `DZ_empire_helmet_h` | Imperial Feathered Pointed Kettle Over Mail | 85/37/31/2.19 | **90/28/22/3.6** | 同 #7（Feathered 装饰 0）|
| 9 | `AR_closed_kettle_helmet_a` | Imperial Plumed Kettle Over Closed Mail | 88/38/33/3.07 | **97/32/25/3.8** | `roundkettle_over_imperial_mail` 92 + Closed 字典 +5/+5/+3 · Closed Mail 全内衬 |
| 10 | `khuzait_ironlame_kettle` | Imperial Closed Iron Kettle | 43/18/16/1.64 | **78/30/22/3.8** | `ironlame_spiked_kettle_over_mail` 73 + Closed +5 · Chainmail 材质 |
| 11 | `khuzait_spiked_kettle` | Imperial Closed Spiked Kettle | 44/19/16/2.06 | **95/30/25/3.9** | `spiked_kettle_over_imperial_mail` 90 + Closed +5 · Chainmail |

**头 > 身 > 臂 校验**：全 11 件通过（每件都严格 head > body > arm）

**Tier 校核**：全 T5。

**关键设计决策**：
- **头 > 身 > 臂铁律首用**：首版提案照抄 vanilla mail wrap 结构（body 15 arm 25 等 arm > body 布局）被用户驳回，正式立铁律。v2 修订版全部翻转 body/arm 分布使符合序
- **回归 vanilla · 有升有降**：Kettle 是普通士兵头盔，严守 vanilla 尺度。#5 Pointed Kettle Over Padding 从 v1 h=86 降回 vanilla 60（v1 script over-buff 纠正）
- **Imperial 双层内衬克制体现**：body/arm 按 aventail suffix 表加成但保持 body > arm
- **两个 Khuzait Chainmail 大幅上调**：v1 h=43-44 明显低估，wt 上调到 3.8-3.9 反映 mail 构造

**状态**：11 件 🔵 log-only（决议归档，XML 未动 · 头 > 身 > 臂铁律首例）

---

## Empire · Crowned/Palatine 家族

### 2026-09-23 · Crowned/Palatine 家族 5 件 · Palace Guard + Royal Crown tier

**背景**：v2 手工审推进到 Crowned/Palatine 家族——Palatine Helmet 是拜占庭皇宫禁卫（Palace Guard / 罗马 Praetorian）盔 · 3 件同 mesh + 3 种 aventail 内衬（Mail/Leather/Cloth）· Crowned Helmet 是皇冠仪式盔 · 2 件是 base + Faceplate 变体。全 5 件属 Lord/Guarded Lord 精英级 tier。

**关键 vanilla 参照**：
- `empire_lord_helmet` 123/10/0/3.5 — Noble Guard Helmet
- `empire_guarded_lord_helmet` 130/30/0/3.7 — Royal Cataphract Helmet
- `empire_battle_crown_north` 104/12/0/2.1 — Jeweled Plumed Battle Crown
- `empire_battle_crown_west` 109/12/0/3.1 — Imperial Plumed Helmet
- `imperial_goggled_helmet` 144/82/45/4.2 — 顶点参照
- aventail suffix：Mail body 12-22 arm 20-25 · Leather 12/15-17 · Cloth 12/6

**5 件落地清单**：

| # | id | 游戏名 | v1 XML 现值 | **v2 决议 h/b/a/wt** | vanilla 依据 |
|---|---|---|---|---|---|
| 1 | `ao_imperial_palatine_guard_helmet` | Imperial Palatine Helmet Over Mail | 78/34/29/1.38 | **128/60/38/3.5** | `empire_guarded_lord_helmet` 130 tier 靠拢 · Palace Guard 精英 · Imperial full mail 双层内衬 |
| 2 | `AO_empire_helmet_za` | Imperial Palatine Helmet Over Leather | 78/34/29/1.54 | **125/42/25/2.7** | 同 mesh base head 125 · Leather 内衬中档 |
| 3 | `AO_empire_helmet_zb` | Imperial Palatine Helmet Over Cloth | 84/36/31/1.56 | **120/28/15/2.3** | 同 mesh base head 120 · Cloth padding 内衬轻档 |
| 4 | `AR_Imperial_Crowned_Helmet_a` | Imperial Crowned Helmet | 85/37/31/2.99 | **120/45/25/3.5** | 介于 `empire_battle_crown_west` 109 与 `empire_lord_helmet` 123 之间 · Crowned 装饰 0 + Imperial mail 双层 |
| 5 | `AR_Imperial_Crowned_Helmet_b` | Imperial Crowned Helmet With Faceplate | 88/38/33/2.99 | **128/55/32/3.9** | Crowned 120 base + Faceplate 字典 +5/+5/+3 · Imperial full mail 双层 |

**头 > 身 > 臂 校验**：全 5 件通过（128>60>38 · 125>42>25 · 120>28>15 · 120>45>25 · 128>55>32）

**Tier 校核**：全 T5。vs Goggled raw 300：#1 -48 · #2 -83 · #3 -113 · #4 -86 · #5 -59（精英差距保留）

**关键设计决策**：
- **Palatine 3 件同 mesh 分档**：head 128→125→120（Mail > Leather > Cloth），body/arm 按 aventail 厚度递减（60/38 → 42/25 → 28/15）
- **v1 XML `_zb` head=84 反直觉**：Palatine Over Cloth 竟然高于 Over Leather/Mail（都 78）——v1 script bug，v2 修订按正确 Mail > Leather > Cloth 排序
- **v1 XML 全线 wt 明显偏轻**（1.38-2.99），Palatine + mail 应 3.5+
- **Palace Guard 精英定位**：Palatine Over Mail #1 直接对标 Varangian Guard 138/70/42（Elite 家族已定案）——本 Palatine 略低（128 vs 138），反映"皇宫内卫"vs"皇宫外卫"层次

**状态**：5 件 🔵 log-only（决议归档，XML 未动）

---

## Empire · 特殊/独立顶档头盔

### 2026-09-23 · 特殊/独立顶档 4 件 · Visored Cap + Battle Crown 本体 + Solar 仪式盔

**背景**：v2 手工审推进到 R. 特殊/独立家族——4 件杂类顶档头盔无固定家族归属：Visored Cap Helmet（带 visor 面罩 Cap-mesh）· Imperial Plumed Helmet（**vanilla 本体** `empire_battle_crown_west`）· 2 件 Solar Helmet 日月纹章仪式盔（Gilded + Silvered 变体）。

**关键 vanilla 参照**：
- `helmet_with_faceguard` 94/12/0/3.5 — Faceguard base
- `empire_lord_helmet` 123/10/0/3.5 — Noble Guard Helmet
- **`empire_battle_crown_west` 109/12/0/3.1**（vanilla 本体）— Imperial Plumed Helmet
- Visored/Faceplate 字典 +5/+5/+3/+1wt
- Gilded 品质 +2 head · Silvered +1 head

**4 件落地清单**：

| # | id | 游戏名 | v1 XML 现值 | **v2 决议 h/b/a/wt** | vanilla 依据 |
|---|---|---|---|---|---|
| 1 | `visored_helmet_x` | Visored Cap Helmet | 79/34/29/3.22 | **95/40/25/4.0** | `helmet_with_faceguard` 94 base + Visored 字典 +5 head · Imperial mail liner body/arm |
| 2 | `empire_battle_crown_west` | Imperial Plumed Helmet | 85/37/31/2.38 | **109/12/0/3.1** | ✅ **vanilla 本体直接回归**（`empire_battle_crown_west` 109/12/0/3.1）· v1 script 把 vanilla 头档 109 反 nerf 到 85 是 bug，v2 修正 |
| 3 | `solar_helmet` | Imperial Solar Helmet With Gilded Faceplate | 85/37/31/2.68 | **128/50/30/3.5** | Lord tier + Faceplate 字典 +5/+5/+3 + Gilded 品质 +2 head · Imperial mail liner |
| 4 | `solar_helmet_b` | Imperial Solar Helmet With Silvered Faceplate | 85/37/31/2.68 | **127/50/30/3.5** | 同 #3（Silvered 品质 +1 head 替 Gilded +2）|

**头 > 身 > 臂 校验**：全 4 件通过（95>40>25 · 109>12>0 · 128>50>30 · 127>50>30）

**Tier 校核**：全 T5。

**关键设计决策**：
- **#2 是 vanilla 物品必须严格直匹配**：`empire_battle_crown_west` 在 vanilla+RBM 里就是 109/12/0/3.1 · v1 script 反 nerf 头档到 85 是明显 bug · v2 按铁律**直接回归 vanilla**（不添加 Imperial 双层，因为这就是 vanilla 本体，vanilla 值已含 RBM 最终设计）
- **Solar Helmet 定位为 Lord + Faceplate 精英仪式盔**：head 127-128 属 Lord 顶档区间 · Gilded vs Silvered 品质差 1 head 微区分
- **Visored Cap Helmet 中档**：Cap 小 mesh + Visored 结构 = 95 head 合理（Faceguard base 94 + Visored 增量）
- **v1 XML 全线 h=79-85 被 v1 script 均值化**（顶档 Solar/Battle Crown 都被拉平到 80 附近，破坏 vanilla+OSA 分档），v2 按各 mesh 应有 tier 恢复

**状态**：4 件 🔵 log-only（决议归档，XML 未动）

---

## Empire · Ridge/Intercisa/Lion 家族

### 2026-09-23 · Ridge/Intercisa/Lion 家族 6 件 · Ridge 字典 + Late Roman 中档定位

**背景**：v2 手工审推进到 Ridge/Intercisa/Lion 家族——5 件 Intercisa（晚期罗马 4-5 世纪脊盔，脊柱结构分片焊接的士兵/军官盔）+ 1 件 Lion Head（Ridge 结构 + 狮子头装饰的顶档 Ridge Helmet）。

**关键 vanilla 参照**：
- `tall_helmet` 84/0/0/1.8 — Tall Helmet base（open cap-style）
- `empire_battle_crown_west` 109/12/0/3.1 — Ridge/Crown 类 Lord tier
- `empire_helmet_with_metal_strips` 120/14/20/3.8 — 顶档 Ridge/Metal Strips
- Ridge/Metal Strips/Faceguard 字典 +4/+3/+2/+1wt
- Gilded 品质 +2 head

**6 件落地清单**：

| # | id | 游戏名 | v1 XML 现值 | **v2 决议 h/b/a/wt** | vanilla 依据 |
|---|---|---|---|---|---|
| 1 | `AR_intercisa_helmet_a` | Imperial Ridged Helmet With Metal Crest | 79/34/29/3.2 | **90/35/20/3.5** | Intercisa base 90 head（`tall_helmet` 84 + Ridge/Metal Crest 结构 +6）· Imperial mail liner body/arm |
| 2 | `AR_intercisa_helmet_b` | Imperial Open Ridged Helmet With Metal Crest | 74/0/0/3.5 | **82/25/12/3.2** | Intercisa base Open 变体（无面颊护）· 减头档 + 内衬减半 |
| 3 | `AR_intercisa_helmet_c` | Imperial Feathered Ridged Helmet With Metal Crest | 84/36/31/3.2 | **90/35/20/3.5** | 同 #1（Feathered 装饰 0）|
| 4 | `AR_intercisa_helmet_d` | Imperial Feathered Decorated Ridged Helmet | 90/39/33/3.2 | **90/35/20/3.5** | 同 #1（Feathered + Decorated 装饰 0）|
| 5 | `AR_intercisa_helmet_e` | Imperial Feathered Gilded Ridged Helmet | 90/39/33/3.2 | **92/35/20/3.5** | 同 #1 + Gilded 品质 +2 head |
| 6 | `AR_lion_head_c` | Ridge Helmet With Lion Head | 85/37/31/3.68 | **115/40/25/4.5** | Lord 档 Ridge + Lion Head 装饰 0 · 介于 `empire_battle_crown_west` 109 与 `empire_helmet_with_metal_strips` 120 之间 · wt 4.5 反映重装饰 mesh |

**头 > 身 > 臂 校验**：全 6 件通过（90>35>20 · 82>25>12 · 90>35>20 · 90>35>20 · 92>35>20 · 115>40>25）

**Tier 校核**：全 T5。

**关键设计决策**：
- **Intercisa 定位为中档罗马士兵盔**：head 82-92 · 不入 Lord 精英级（120+）· 反映"late Roman infantry"史实定位
- **Open 变体（#2）显式减档**：v1 XML 已识别 Open 无 aventail（body 0 arm 0）· v2 保留此认知但加轻装 Imperial liner（body 25 arm 12），符合头 > 身 > 臂
- **Feathered/Decorated 系列（#3 #4）拉齐 base #1**：字典规则装饰 0 armor，无 mesh 差异化时保持同数值
- **Lion Head（#6）拉到 Lord tier**：wt 4.5 与 v1 XML 3.68 高于其他 Intercisa（3.2-3.5），mesh 明显更大/重档 · head 115 匹配 Lord tier 但保留 Ridge 类识别（不到 Metal Strips 120 顶档）
- **Metal Crest = Ridge 结构同类**：字典未单列，视为 Ridge 结构性词的同义（不双计）

**状态**：6 件 🔵 log-only（决议归档，XML 未动）

---

## Empire · Legatus 家族（Reinforced Cavalry）

### 2026-09-23 · Legatus 家族 3 件 · Officer Cavalry Helmet 定位

**背景**：v2 手工审推进到 Legatus 家族——3 件 "Imperial Reinforced Cavalry Helmet" 同 mesh + 装饰变体（Feathered / Plumed）· Legatus = 罗马军团副司令级 Officer · 属"骑兵专用加固盔"。

**关键 vanilla 参照**：
- `imperial_nasal_helm` 97/12/25/2.2 — Legionary Helm
- `empire_lord_helmet` 123/10/0/3.5 — Noble Guard Helmet
- Feathered/Plumed 装饰词 = 0 armor
- `Reinforced` 候选词（未正式入字典，类 Iron +1-2 head）

**3 件落地清单**：

| # | id | 游戏名 | v1 XML 现值 | **v2 决议 h/b/a/wt** | vanilla 依据 |
|---|---|---|---|---|---|
| 1 | `AR_empire_legatus_helm_c` | Imperial Reinforced Cavalry Helmet | 79/34/29/3.03 | **110/40/22/3.5** | Officer Cavalry 定位 · 介于 Legionary 97 与 Lord 123 之间 · Reinforced 结构 + Imperial mail liner |
| 2 | `AR_empire_legatus_helm_d` | Imperial Feathered Reinforced Cavalry Helmet | 82/35/30/3.03 | **110/40/22/3.5** | 同 #1（Feathered 装饰 0）|
| 3 | `AR_empire_legatus_helm_e` | Imperial Plumed Reinforced Cavalry Helmet | 84/36/31/3.03 | **110/40/22/3.5** | 同 #1（Plumed 装饰 0）|

**头 > 身 > 臂 校验**：全 3 件通过（110>40>22）

**Tier 校核**：全 T5（raw 194 / scaled 232.8）

**关键设计决策**：
- **Legatus tier 定位** = 军团副司令级 Officer Cavalry · head 110 明显高于 Legionary 97 · 低于 Lord 123 · 反映 Officer 级但非最高领主级
- **3 件同数值**：Feathered/Plumed 都是纯装饰词，无 mesh 差异化时保持严格同数值
- **`Reinforced` 结构性词处理**：本次不叠加 dict 增量（已在 head 110 里默认体现结构加固），若未来立项入字典可追溯 +1-2 head
- **v1 XML 明显低估**（h=79-84）：Legatus 是罗马高级 Officer，Cavalry 骑兵盔应达 Officer tier · v2 归正 110

**状态**：3 件 🔵 log-only（决议归档，XML 未动）

---

## Empire · Conical/Pointed Helmet 家族

### 2026-09-23 · Conical/Pointed Helmet 家族 8 件 · 中骑兵档 + aventail 5 挡分档

**背景**：v2 手工审推进到 Conical/Pointed Helmet 家族——4 件 Pointed Helmet + 4 件 Cone/Conical Helmet · 均为尖顶/锥形头盔 · 骑兵/中步兵级（非精英）· 按 aventail 内衬（cloth/leather/lamellar/mail_coif/closed_mail）分 5 档。

**⚠ 显示名 bug 发现**（非 same-item dup，是数据错误）：
- `ao_imperial_pointed_helmet_with_lamellar_strips` → 显示名 "Pointed Helmet With Lamellar Strips" ✓
- `ao_imperial_pointed_helmet_with_mail_coif` → 显示名 **"Pointed Helmet With Lamellar Strips"**（**id 与显示名不符** · 应为 "With Mail Coif"）

两件结构不同（lamellar vs mail coif），按 **id 语义**分开处理，未来在游戏内 language XML 可修正。

**vanilla RBM 参照**：
- `tall_helmet` 84/0/0/1.8 — Tall Helmet base
- `imperial_nasal_helm` 97/12/25/2.2 — Legionary Helm
- `spiked_kettle_over_imperial_mail` 90/4/20/3.6 — Pointed/Spiked mail 参照
- aventail suffix：cloth 12/6 · leather 12/15-17 · mail 12-22/20-25 · coif 22/40-45

**分档规则**：Pointed base 85 · Cone base 90 · aventail delta: cloth(-5h/15/8) · leather(0h/25/15) · lamellar(+5h/32/18) · mail coif(+8h/42/28) · closed mail(+8h/45/30)

#### 子群 A · Pointed Helmet 系（4 件 · base 85）

| # | id | 游戏名 | v1 XML 现值 | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `ao_imperial_pointed_helmet_with_cloth_wrap` | Pointed Helmet With Cloth Wrap | 19/0/0/0.46 | **80/15/8/1.5** |
| 2 | `ao_imperial_pointed_helmet_with_leather` | Pointed Helmet With Leather | 74/0/0/1.8 | **85/25/15/2.2** |
| 3 | `ao_imperial_pointed_helmet_with_lamellar_strips` | Pointed Helmet With Lamellar Strips | 77/33/28/1.56 | **90/32/18/2.5** |
| 4 | `ao_imperial_pointed_helmet_with_mail_coif` | **[显示名 bug]** Pointed Helmet With Lamellar Strips | 87/37/32/1.56 | **93/42/28/3.5** |

#### 子群 B · Cone/Conical Helmet 系（4 件 · base 90）

| # | id | 游戏名 | v1 XML 现值 | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 5 | `ao_imperial_cone_helmet_with_leather` | Imperial Conical Helmet With Leather | 84/36/31/1.56 | **90/25/15/2.5** |
| 6 | `ao_imperial_cone_helmet_with_lamellar_strips` | Imperial Conical Helmet With Mail And Lamellar Strip | 78/34/29/1.54 | **95/32/18/2.8** |
| 7 | `ao_imperial_noblemans_cone_helmet` | Imperial Plumed Conical Helmet Over Mail Coif | 84/36/31/1.54 | **98/42/28/3.5** |
| 8 | `ao_imperial_plumed_cone_helmet_with_closed_mail` | Imperial Plumed Conical Helmet With Closed Mail | 81/35/30/1.38 | **98/45/30/3.7** |

**头 > 身 > 臂 校验**：全 8 件通过。

**Tier 校核**：全 T5。

**关键设计决策**：
- **Cone > Pointed mesh 差异化**：Cone base 90 vs Pointed base 85（+5 head 反映 Cone mesh 略更精工/大）
- **Aventail 分档 5 挡**：cloth < leather < lamellar < mail_coif < closed_mail（head delta -5/0/+5/+8/+8）
- **顶档 #8 Closed Mail Cone 98/45/30/3.7**：Cone 系顶档，仍低于 Lord Helmet 123 且远低于 Goggled 144，符合中骑兵/officer tier 定位
- **v1 XML wt 全线 1.4-1.8 偏轻**：中档 mail 头盔应 2.5-3.7 wt · v2 归正
- **#1 Cloth Wrap 修复**：v1 XML 保留 OSA 源 h=19（v1 script 漏 buff）· v2 归正到 80

**状态**：8 件 🔵 log-only（决议归档，XML 未动 · 显示名 bug 记录待未来修正）

---

## Empire · TV/AR 混合尾单家族（S + Desert 残余）

### 2026-09-23 · TV/AR 混合尾单 14 件 · Empire HeadArmor 收官批

**背景**：v2 手工审推进到最后一个 Empire HeadArmor 未定案家族——13 件 TV/AR 系（Metal Strips + Banded + Coif/Cap + Kettle over Mail 4 子群）+ 1 件 Desert 残余（Southern Helmet with Metal Strips）= 14 件收官。本批处理完 **Empire HeadArmor 165 件 100% 定案**。

**关键 vanilla 参照**：
- `empire_helmet_with_metal_strips` 120/14/20/3.8 — Metal Strips Lord 顶档
- `empire_jewelled_helmet` 125/14/20/3.8
- `imperial_padded_coif` 22/7/22/0.5 — Padded Cloth Coif direct
- `mail_coif` 38/12/38/1.7 — Mail Coif base
- `roundkettle_over_imperial_mail` 92/0/20/3.3 — Roundkettle + Mail
- 字典：Metal Strips/Ridge +4/+3/+2 · Closed/Faceguard +5/+5/+3 · Gilded +2 head

#### 子群 A · Metal Strips 系（4 件 · Lord 顶档）

| # | id | 游戏名 | v1 XML 现值 | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_empire_helmet_g` | Imperial Gilded Helmet With Metal Strips | 88/38/33/2.91 | **122/50/25/3.8** |
| 2 | `AR_empire_helmet_h` | Imperial Gilded Closed Helmet With Metal Strips | 90/39/33/2.91 | **127/55/30/4.2** |
| 3 | `AR_empire_helmet_n` | Imperial Plumed Banded Helmet With Metal Strips | 74/32/27/1.69 | **108/40/20/2.7** |
| 4 | `AR_empire_desert_helmet_e` | Southern Helmet with Metal Strips | 88/38/33/2.91 | **118/45/22/3.5** |

#### 子群 B · Banded 系（4 件 · 中档 Ridge-adjacent）

| # | id | 游戏名 | v1 XML 现值 | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 5 | `AR_empire_helmet_l` | Imperial Banded Helmet | 80/34/30/1.88 | **95/32/18/2.2** |
| 6 | `AR_empire_helmet_m` | Imperial Banded Helmet With Leather Strips | 84/36/31/1.88 | **98/35/20/2.5** |
| 7 | `TV_empire_helmet_c` | Imperial Decorated Banded Helmet Over Mail Coif | 90/39/33/3.03 | **105/42/28/3.5** |
| 8 | `TV_empire_helmet_d` | Imperial Decorated Banded Helmet With Faceguard Over Mail | 84/36/31/2.99 | **108/45/28/3.5** |

#### 子群 C · Coif/Cap 系（3 件）

| # | id | 游戏名 | v1 XML 现值 | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 9 | `AR_empire_helmet_o3` | Imperial Steel Scale Coif | 92/0/0/1.7 | **55/30/18/2.0** |
| 10 | `AR_empire_helmet_p` | Imperial Padded Cloth Coif | 16/0/0/0.25 | **25/18/10/0.6** |
| 11 | `TV_empire_helmet_a` | Imperial Feathered Decorated Steel Cap Over Stripped Mail | 84/36/31/3.33 | **108/42/22/3.5** |

#### 子群 D · Kettle over Mail 系（3 件）

| # | id | 游戏名 | v1 XML 现值 | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 12 | `TV_empire_helmet_b` | Imperial Spiked Roundkettle Over Mail | 84/36/31/3.29 | **95/32/22/3.5** |
| 13 | `TV_empire_helmet_e` | Imperial Roundkettle Over Closed Mail Coif | 80/34/30/3.16 | **95/45/28/3.7** |
| 14 | `TV_empire_helmet_f` | Imperial Spiked Kettle Helmet Over Closed Mail Coif | 84/36/31/3.16 | **98/45/28/3.7** |

**头 > 身 > 臂 校验**：全 14 件通过。

**Tier 校核**：全 T5。

**关键设计决策**：
- **Metal Strips 系 Lord 顶档定位**：#1 #2 靠拢 vanilla Metal Strips 120 · Gilded +2 + Closed +5 层层叠加
- **Banded 系中档定位**：head 95-108 · 中档 Ridge-adjacent · 靠 Faceguard/Mail 内衬提档
- **Scale Coif 完整化**：Steel > Alternating/Brass 三档阶梯（Brass/Alternating 48 追溯修正 · Steel 55）· 头 > 身 > 臂严格
- **Padded Cloth Coif（#10）严守 vanilla 尺度**：直匹配 `imperial_padded_coif` 22/7/22 · 微调符合头 > 身 > 臂
- **v1 XML 全线偏低**：Metal Strips 系 v1 只 88-90 head（应 120+），Banded 系 v1 80-90（应 95-108），Kettle 系 v1 80-84（应 95-98）· v2 全面归正到 vanilla 尺度

**状态**：14 件 🔵 log-only（决议归档，XML 未动 · **Empire HeadArmor 165 件 100% 收官**）

---

## Empire · Cape · G. Plate Lamellar Shoulders/Pauldrons 家族

### 2026-09-23 · G 家族 19 件 · Cape 二部分律 + arm mesh-tiered 首用

**背景**：v2 手工审进入 Empire Cape。首先立 Cape **二部分铁律**：
1. 命名二分律：shoulder/pauldron → arm > 0 (body > arm) · 无 → arm = 0
2. arm mesh-tiered 分档律：Elite Heavy 顶档 25 · Standard Shoulders 20 · Pauldrons 12 · Studded Strip 6-8

G 家族是 Empire Cape 最大家族（19 件 · 主流 Lamellar 肩甲），Imperial "领主装备 + Pauldron 特色" 的核心档次。

**vanilla RBM 参照**：
- `imperial_lamellar_shoulders` **55/0/3.5** ⭐（Heavy Lamellar Pauldrons · Empire Cape 顶点 · body only）
- `imperial_studded_strip_shoulders` 34/0/4.1（Legionary Reinforced Studded Harness · Studded Strip 参照）
- `pauldron_cape_a` 30/0/3.5（Legionary Cape · Pauldrons 参照）

**19 件落地清单**：

#### 子群 G.1a · Standard Lamellar Shoulders（4 件 · Cape mesh 顶档标准版）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_imperial_shoulders_a` | Imperial Lamellar Shoulders Over Leather | 18/9/3.8 | **40/20/3.8** |
| 2 | `AR_imperial_shoulders_e` | Imperial Lamellar Shoulders With Leopard | 18/9/3.8 | **40/20/3.8** |
| 3 | `AR_imperial_shoulders_e3` | Imperial Lamellar Shoulders With Snow Leopard | 18/9/3.8 | **40/20/3.8** |
| 4 | `AR_imperial_shoulders_q` | Imperial Lamellar Shoulders With Lion Pelt | 18/9/3.8 | **40/20/3.8** |

#### 子群 G.1b · Gilded Lamellar Shoulders（5 件 · Elite Heavy 顶档 · Gilded 品质 +2 body +5 arm）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 5 | `AR_imperial_lamellar_shoulders_a` | Imperial Gilded Lamellar Shoulders | 17/9/3.5 | **42/25/3.8** |
| 6 | `AR_imperial_shoulders_a2` | Imperial Gilded Lamellar Shoulders Over Leather | 18/9/3.5 | **42/25/3.8** |
| 7 | `AR_imperial_shoulders_e2` | Imperial Gilded Lamellar Shoulders With Leopard | 18/9/4.8 | **42/25/3.8** |
| 8 | `AR_imperial_shoulders_e4` | Imperial Gilded Lamellar Shoulders With Snow | 18/9/4.8 | **42/25/3.8** |
| 9 | `AR_imperial_shoulders_q2` | Imperial Gilded Lamellar Shoulders With Lion | 18/9/4.8 | **42/25/3.8** |

#### 子群 G.2a · Standard Lamellar Pauldrons（5 件 · 较小 mesh · arm 中档）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 10 | `AR_imperial_shoulders_b` | Imperial Lamellar Pauldrons Over Leather | 16/8/2.7 | **35/12/3.6** |
| 11 | `AR_imperial_shoulders_f` | Imperial Lamellar Pauldrons With Leopard | 16/10/3.6 | **35/12/3.6** |
| 12 | `AR_imperial_shoulders_f2` | Imperial Lamellar Pauldrons With Snow Leopard | 16/10/3.6 | **35/12/3.6** |
| 13 | `AR_imperial_shoulders_h` | Imperial Lamellar Pauldrons With Scarf | 16/10/3.6 | **35/12/3.6** |
| 14 | `AR_imperial_shoulders_r` | Imperial Lamellar Pauldrons With Lion Pelt | 16/10/3.6 | **35/12/3.6** |

#### 子群 G.2b · Plumed Lamellar Pauldrons（2 件 · Plumed 装饰 0）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 15 | `DZ_empire_shoulder_a` | Imperial Plumed Lamellar Pauldrons Over | 16/8/3.9 | **35/12/3.6** |
| 16 | `DZ_empire_shoulder_b` | Imperial Plumed Lamellar Pauldrons Over | 16/8/3.9 | **35/12/3.6** |

#### 子群 G.3 · Studded Strip Shoulders（3 件 · 轻档结构）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 17 | `AR_imperial_shoulders_o` | Imperial Studded Strip Shoulders With Lamellar | 7/6/3.9 | **26/8/3.9** |
| 18 | `AR_imperial_shoulders_o2` | Imperial Studded Strip Shoulders With Lamellar | 7/6/3.9 | **26/8/3.9** |
| 19 | `TV_empire_shoulders_b` | Imperial Leather Shoulders With Lamellar | 7/6/3.9 | **26/8/3.9** |

**Cape 铁律校验**：
- 命名二分律 A 组：全 19 件命名含 shoulder/pauldron ✓
- body > arm 严格序：全 19 件通过（40>20 · 42>25 · 35>12 · 26>8）✓
- arm mesh-tiered 分档：G.1b Elite 25 · G.1a Standard 20 · G.2 Pauldrons 12 · G.3 Strip 8 ✓

**Tier 校核**（Cape mult 1.8）：

| 子群 | b/a | raw | scaled | tier |
|---|---|---:|---:|:---:|
| G.1a Standard | 40/20 | 60 | 108 | T5 |
| G.1b Gilded 顶档 | 42/25 | 67 | 120.6 | T5 |
| G.2 Pauldrons | 35/12 | 47 | 84.6 | T5 |
| G.3 Strip | 26/8 | 34 | 61.2 | T5 |

全 T5。

**关键设计决策**：
- **Gilded 顶档（G.1b）arm 25 应用用户拍板方案 C**：Heavy Lamellar Pauldrons 视觉覆盖整个肩膀 + 上臂到肘 · arm 25 忠实反映 mesh 物理覆盖
- **arm mesh-tiered 分档**：G.1b 25 > G.1a 20 > G.2 12 > G.3 8，反映 mesh 从 elite 顶档到轻档 studded strip 的视觉覆盖度递减
- **Gilded 品质 +2 body**：G.1b 42 vs G.1a 40 · 跨类型统一（HeadArmor Gilded +2 head → Cape Gilded +2 body）
- **顶点参照放宽**：G.1b raw 67 超过 vanilla body 顶 55，但**body 仍 ≤ 55**（42 < 55）· OSA 借 arm 特色扩展 raw 属 Cape 二部分律允许，不算越权铁律 1
- **精英 arm 膨胀 26%**：OSA 精锐兵 arm 总值 ~120 vs RBM ~95（Head 45 + Body 20 + Cape 25 + Hand 30）· **故意设计取舍**换取 OSA "Cape 特色" 忠实 mesh
- **Pelt/Scarf 装饰全 0 armor**：Leopard/Snow Leopard/Lion Pelt/Scarf 只装饰 mesh 视觉
- **Plumed 装饰 0**：G.2b 与 G.2a 同数值

**状态**：19 件 🔵 log-only（决议归档，XML 未动 · Cape 二部分律 + arm mesh-tiered 首用）

### 2026-09-23 · Empire Cape · F. Scale Shoulders 家族 8 件 · 视觉判断优先律首用

**背景**：F 家族 8 件全部命名含 shoulder/pauldron，按第一部分律**允许** arm > 0。但用户 in-game 视觉观察揭示 **6/8 件 mesh 只覆盖 shoulder body，不覆盖上臂** → 触发**第三部分视觉判断优先律**：arm 清 0。

**用户视觉观察 quote**：
- "scale shoulders 的 mesh 似乎只覆盖身体，并不覆盖肩膀"
- "with lamellar 的版本才有覆盖大臂的扎甲铁片"
- "Decorated Leather Harness Over Scale 是同时覆盖了肩部和大臂，但大臂的护甲覆盖面积要比 Lamellar 系列要小"

**vanilla RBM 参照**：
- `varangian_bra_scale` **40/0/4.0** ⭐（Decorated Leather Harness over Scale · Scale 顶档 vanilla 直匹配 F.5）
- `pauldron_cape_a` 30/0/3.5（Legionary Cape · Pauldrons 参照）
- `empire_plate_armor_shoulder_a` 17/0/3.6 · `empire_plate_armor_shoulder_b` 21/0/3.6（Bronze/Iron Plate Pauldrons）

#### 子群 F.1 · Standard Scale Shoulders（2 件 · Scale mesh 只覆盖 body）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_imperial_shoulders_c` | Imperial Scale Shoulders | 16/8/2.7 | **32/0/3.5** |
| 2 | `AR_imperial_shoulders_l` | Imperial Brass Scale Shoulders | 16/8/2.7 | **32/0/3.5** |

Scale mesh 覆盖 body-shoulder 区域 · 无上臂 · 视觉判断优先律 arm = 0（v1 arm 8 违反视觉）

#### 子群 F.2 · Scale Shoulders with Lamellar（1 件 · Lamellar 上臂覆盖）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 3 | `AR_imperial_shoulders_d` | Imperial Scale Shoulders With Lamellar | 18/9/3.8 | **38/20/3.8** |

Scale body + Lamellar 铁片覆盖上臂 → Standard Shoulders 档 arm 20

#### 子群 F.3 · Alternating Scale（2 件 · body only · 视觉同 Standard Scale）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 4 | `AR_imperial_shoulders_v` | Imperial Alternating Scale Shoulders | 16/0/4.1 | **32/0/3.6** |
| 5 | `AR_imperial_shoulders_u` | Imperial Alternating Scale Pauldrons | 16/0/4.5 | **32/0/3.6** |

Alternating 是 mesh 花纹（非结构性）· 视觉同 Standard Scale · arm 0

#### 子群 F.4 · Steel Scale（2 件 · Steel 品质 +1 body · body only）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 6 | `AR_imperial_shoulders_x` | Imperial Steel Scale Shoulders | 16/0/4.1 | **33/0/3.7** |
| 7 | `AR_imperial_shoulders_w` | Imperial Steel Scale Pauldrons | 16/0/4.5 | **33/0/3.7** |

Steel 品质 +1 body（Iron/Silvered 档）· 视觉同 Alternating · arm 0

#### 子群 F.5 · Decorated Leather Harness Over Scale（1 件 · 顶档 · vanilla 直匹配 · 部分上臂覆盖）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 8 | `AR_imperial_shoulders_y` | Imperial Decorated Leather Harness Over Scale | 19/12/14 | **40/15/4.5** |

✅ vanilla `varangian_bra_scale` 40/0 direct + **arm 15**（Harness 部分覆盖上臂 · 小于 Lamellar 20 大于 Pauldrons 12 · **新引入"部分覆盖"档**）· wt 4.5（v1 wt 14 是明显 script bug）

### Cape 铁律校验（全 8 件通过）

- **第一部分 命名二分律** A 组：全 8 件命名含 shoulder/pauldron ✓
- **第二部分 arm mesh-tiered 分档律**：F.2 Standard 20 · F.5 部分覆盖 15 · F.1/F.3/F.4 无上臂 0 ✓
- **第三部分 视觉判断优先律**：6/8 件（F.1/F.3/F.4）覆盖命名默认 arm > 0，改为 arm = 0 · 首次实证运用
- **body > arm 严格序（arm=0 视为满足）**：全 8 件通过

### Tier 校核（Cape mult 1.8）

| # | b/a | raw | scaled | tier |
|---|---|---:|---:|:---:|
| F.1 c/l | 32/0 | 32 | 57.6 | T4 |
| F.2 d | 38/20 | 58 | 104.4 | T5 |
| F.3 v/u | 32/0 | 32 | 57.6 | T4 |
| F.4 x/w | 33/0 | 33 | 59.4 | T4 |
| F.5 y | 40/15 | 55 | 99.0 | T5 |

F.1/F.3/F.4 落 T4 与 vanilla `pauldron_cape_a` 30/0 T4 对齐（Legionary Cape 档）· F.5 raw 55 = vanilla body 顶点齐平（借 arm 分配到 body+upper_arm 两块 mesh）· F.2 T5 反映 Scale+Lamellar 复合结构中档

### 关键设计决策

- **视觉判断优先律首次实证**：F 家族揭示"命名允许 arm ≠ 数值必须 arm > 0"·6/8 件命名含 Shoulders/Pauldrons 但因 mesh 视觉无上臂覆盖，arm 清 0
- **F.5 引入"部分覆盖"档 arm 15**：Harness Over Scale 是新 mesh 类型（介于 Standard Shoulders 20 和 Standard Pauldrons 12 之间）· 反映 Harness 覆盖部分上臂但小于 Lamellar
- **Alternating vs Steel 品质区分**：Alternating 是 mesh 花纹（0 加成），Steel 是 Iron/Silvered 品质档（+1 body）
- **Shoulders vs Pauldrons 命名差异被视觉判断律 override**：v/u 与 x/w 命名不同但视觉相同（Alternating/Steel 都不覆盖上臂）· v2 同数值处理

**状态**：8 件 🔵 log-only（决议归档，XML 未动 · 视觉判断优先律首次实证）

---

## Empire · Cape · H. Wolf Pelt 家族（extra_contribution）

### 2026-09-23 · H 家族 10 件 · 装饰前缀 0 影响 · 材质分档实证

**背景**：extra_contribution 包 10 件 Wolf Pelt 变体覆盖 Lamellar Shoulders/Pauldrons/Leather/Mail/Brass Scale 五种基础。Wolf/Brown Wolf Pelt 属**装饰前缀** → 按字典律**零护甲影响**，直接继承基础档次。

**vanilla RBM 参照**：
- `imperial_lamellar_shoulders` **55/0/3.5** ⭐（Heavy Lamellar Pauldrons顶点）
- `varangian_bra_mail` 35/0/3.2（Chainmail Shoulders 唯一参照）
- `woven_leather_shoulders` 16/0/1.6 · `varangian_bra_basic` 18/0/2.5 · `empire_warrior_padded_armor_shoulder` 24/0/1.6（Leather 梯度）

#### 子群 H.1 · Lamellar Shoulders + Wolf Pelt（2 件 · G.1a Standard tier）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_wolf_shoulder_a` | Imperial Lamellar Shoulders With Wolf Pelt | 18/10/3.8 | **40/20/3.8** |
| 2 | `AR_wolf_shoulder_b` | Imperial Lamellar Shoulders With Brown Wolf Pelt | 18/10/3.6 | **40/20/3.8** |

同 G.1a Standard Lamellar Shoulders 档 · Wolf Pelt 装饰 0

#### 子群 H.2 · Lamellar Pauldrons + Wolf Pelt（2 件 · G.2 Standard Pauldrons tier）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 3 | `AR_wolf_shoulder_c` | Imperial Lamellar Pauldrons With Wolf Pelt | 16/8/3.8 | **35/12/3.6** |
| 4 | `AR_wolf_shoulder_d` | Imperial Lamellar Pauldrons With Brown Wolf Pelt | 16/8/3.6 | **35/12/3.6** |

同 G.2 Standard Pauldrons 档

#### 子群 H.3 · Leather Shoulders + Wolf Pelt（2 件 · Leather Shoulders 中档）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 5 | `AR_wolf_shoulder_e` | Imperial Leather Shoulders With Wolf Pelt | 10/8/2.1 | **18/6/2.2** |
| 6 | `AR_wolf_shoulder_e2` | Imperial Leather Shoulders With Brown Wolf Pelt | 10/8/2.1 | **18/6/2.2** |

vanilla `varangian_bra_basic` 18/0 直匹配 · Leather Shoulders arm 中档 6

#### 子群 H.4 · Mail Shoulders + Wolf Pelt（2 件 · Chainmail Shoulders 档）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 7 | `AR_wolf_shoulder_f` | Imperial Mail Shoulders With Wolf Pelt | 14/8/2.7 | **35/10/2.9** |
| 8 | `AR_wolf_shoulder_f2` | Imperial Mail Shoulders With Brown Wolf Pelt | 14/8/2.7 | **35/10/2.9** |

vanilla `varangian_bra_mail` 35/0 直匹配 · Chainmail Shoulders arm 中档 10

#### 子群 H.5 · Brass Scale Shoulders + Wolf Pelt（2 件 · F 家族视觉律 arm 清 0）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 9 | `AR_wolf_shoulder_g` | Imperial Brass Scale Shoulders With Wolf Pelt | 16/8/2.7 | **32/0/2.9** |
| 10 | `AR_wolf_shoulder_g2` | Imperial Brass Scale Shoulders With Brown Wolf Pelt | 16/8/2.7 | **32/0/2.9** |

**视觉判断优先律**（F 家族先例）：Scale Shoulders mesh 只覆盖 body-shoulder，arm 清 0 · Wolf Pelt 装饰 0 影响

**Cape 铁律校验**：全 10 件命名含 shoulder/pauldron ✓ · body > arm 严格序 ✓ · arm mesh-tiered 分档 ✓

**状态**：10 件 🔵 log-only

---

## Empire · Cape · I. Bronze/Iron/Studded Plate Pauldrons

### 2026-09-23 · I 家族 5 件 · vanilla 直匹配 + Brass/Iron 品质分档

**背景**：Plate Pauldrons 家族——vanilla Empire Cape 里 Bronze/Iron plate 是主流中档档次，命名含"Pauldrons"为 Group A。BA_ 系列是 extra_contribution 的"Pauldrons + Mail" 复合结构（Studded 加固 + Mail underlay）。

**vanilla RBM 参照**：
- `empire_plate_armor_shoulder_a` **17/0/3.6**（Bronze Plate Pauldrons）
- `empire_plate_armor_shoulder_b` **21/0/3.6**（Iron Plate Pauldrons · Iron +4 body vs Bronze）
- `a_pauldron_cape_c` 15/0/3.5（Bronze Pauldrons）
- `a_pauldron_cape_b` 20/0/3.5（Bronze Pauldrons with Neck Guard）

#### 子群 I.1 · Bronze Pauldrons + Scarf（1 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_imperial_shoulders_g` | Imperial Bronze Pauldrons With Scarf | 18/0/3.5 | **17/12/3.5** |

vanilla `empire_plate_armor_shoulder_a` 17/0 直匹配 + arm 12 Standard Pauldrons 档 · Scarf 装饰 0

#### 子群 I.2 · Studded Brass/Iron Pauldrons + Mail（2 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 2 | `BA_empire_shoulders_a` | Imperial Studded Brass Pauldrons With Mail | 16/8/2.7 | **24/12/3.0** |
| 3 | `BA_empire_shoulders_b` | Imperial Studded Iron Pauldrons With Mail | 16/8/2.7 | **26/12/3.0** |

Studded Brass = Bronze upgrade +4 (17→21→24 累加 Studded reinforce) · Iron +2 vs Brass · Mail underlay 属描述性，不额外加档 · arm 12 Standard Pauldrons

#### 子群 I.3 · Brass/Iron Pauldrons + Mail（2 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 4 | `BA_empire_shoulders_c` | Imperial Brass Pauldrons With Mail | 14/6/2.7 | **20/10/2.8** |
| 5 | `BA_empire_shoulders_d` | Imperial Iron Pauldrons With Mail | 14/6/2.7 | **22/10/2.8** |

比 Studded 弱一档（无 Studded 加固）· Brass 20 / Iron 22（+2 品质差）· arm 10 Chainmail Shoulders 中档（Mail underlay 视觉体现）

**Cape 铁律校验**：全 5 件命名含 Pauldrons ✓ · body > arm ✓ · Plate Pauldrons 档 arm 12/10 ✓

**状态**：5 件 🔵 log-only

---

## Empire · Cape · K. Leather Shoulders + Cape/Cloak

### 2026-09-23 · K 家族 9 件 · Leather Shoulders 分档 + Cape 结构 body 加成

**背景**：Leather Shoulders 及其 Cape/Cloak 变体，命名均含"Shoulders"为 Group A。分独立 Shoulders + 附 Cape 两组。

**vanilla RBM 参照**：
- `woven_leather_shoulders` **16/0/1.6**（Woven Leather · Leather 轻档）
- `varangian_bra_basic` **18/0/2.5**（Decorated Leather Harness · Leather 中档）
- `varangian_bra_royal` **22/0/2.7**（Caped Leather Harness · Leather 中高档）
- `empire_warrior_padded_armor_shoulder` **24/0/1.6**（Legionary Padded Straps · Padded 顶档）
- `varangian_bra_padded` **27/0/3.2**（Decorated Leather Harness with Padding · Padded + Leather复合顶档）

#### 子群 K.1 · 独立 Leather Shoulders（3 件 · 无 Cape 结构）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_imperial_shoulders_m` | Imperial Leather Shoulders | 8/2/2.1 | **16/4/2.0** |
| 2 | `AR_imperial_shoulders_s` | Imperial Studded Leather Shoulders With Focale | 8/2/2.1 | **20/6/2.2** |
| 3 | `AR_imperial_shoulders_t` | Imperial Trimmed Leather Shoulders | 8/2/2.1 | **17/5/2.0** |

- m：vanilla `woven_leather_shoulders` 16/0 直匹配 + arm 4 Leather 轻档
- s：Studded Leather 中档 · body 20（Studded +4 vs 基础 16）· arm 6 Leather 中档 · Focale 装饰 0
- t：Trimmed 品质词 · body 17（+1 品质微调）· arm 5

#### 子群 K.2 · Padded Shoulders + Scarf（1 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 4 | `AR_empire_shoulders_b` | Imperial Padded Shoulders With Scarf | 12/4/1.6 | **22/6/1.8** |

vanilla `empire_warrior_padded_armor_shoulder` 24/0 参照 · Padded 中档 body 22 · arm 6 Leather 中档 · Scarf 装饰 0

#### 子群 K.3 · Leather Shoulders + Cape/Cloak（5 件 · Cape 结构 body +2）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 5 | `AR_imperial_leather_cape_a` | Imperial Leather Shoulders With Cape | 12/0/4 | **22/8/3.5** |
| 6 | `AR_imperial_leather_cape_b` | Imperial Leather Shoulders With Striped Cloak | 12/0/4 | **22/8/3.5** |
| 7 | `AR_imperial_leather_cape_b2` | Imperial Leather Shoulders With Cloak | 12/0/4 | **22/8/3.5** |
| 8 | `tv_imperial_leather_cape_a` | Imperial Leather Shoulders With Plaid Cape | 12/0/4 | **22/8/3.5** |
| 9 | `tv_imperial_leather_cape_b` | Imperial Leather Shoulders With Striped Cape | 12/0/4 | **22/8/3.5** |

vanilla `varangian_bra_royal` 22/0 直匹配（Caped Leather Harness）· Cape 结构 body +6 vs 基础 Leather Shoulders 16 · arm 8 Leather 中档 · Striped/Plaid/Cloak 装饰 0 · wt v1 4.0 略重，降到 3.5

**Cape 铁律校验**：全 9 件命名含 Shoulders ✓ · body > arm ✓ · Leather Shoulders arm 中档 4-8 ✓

**状态**：9 件 🔵 log-only

---

## Empire · Cape · L. Mail Shoulders

### 2026-09-23 · L 家族 1 件 · Chainmail Shoulders 唯一档

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_imperial_shoulders_n` | Imperial Mail Shoulders | 12/4/2.7 | **35/10/2.9** |

vanilla `varangian_bra_mail` 35/0/3.2 直匹配 · Chainmail Shoulders arm 中档 10

**Cape 铁律校验**：命名含 Shoulders ✓ · body > arm ✓ · Chainmail 中档 10 ✓

**状态**：1 件 🔵 log-only

---

## Empire · Cape · M. Studded Strip Shoulders（light）

### 2026-09-23 · M 家族 2 件 · Studded 轻档 · G.3 tier fallback

**背景**：与 G.3（`AR_imperial_shoulders_o/o2` Studded Strip Shoulders With Lamellar/Cape/Focale）同结构，本组是**独立 Studded Strip Shoulders**（无 Lamellar 补强），比 G.3 弱一档。

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_imperial_shoulders_p` | Imperial Studded Strip Shoulders With Focale | 7/4/3.6 | **22/8/3.4** |
| 2 | `AR_imperial_shoulders_p2` | Imperial Studded Shoulders With Focale | 6/0/3.6 | **18/6/3.2** |

- p：Studded Strip Shoulders + Focale · 比 G.3 弱一档（G.3 是 Strip + Lamellar Pauldrons 复合 26/8）· body 22 / arm 8 Strip 顶档
- p2：Studded Shoulders（无 Strip 结构）· 更轻 · body 18 / arm 6 Leather 中档

**Cape 铁律校验**：命名含 Shoulders ✓ · body > arm ✓ · Studded Strip arm 6-8 ✓

**状态**：2 件 🔵 log-only

---

## Empire · Cape · N. Gladiator Single Pauldron

### 2026-09-23 · N 家族 2 件 · 单肩甲结构（body 减半 · arm 保留）

**背景**：命名"Single Lamellar Pauldron"——**只覆盖一侧肩膀**（角斗士传统单肩甲），body 覆盖减半但 arm 覆盖同 Standard Pauldrons（因为覆盖那一侧的上臂）。

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_gladiator_shoulder_a` | Single Lamellar Pauldron Over Leather | 18/9/3.5 | **20/12/2.5** |
| 2 | `AR_gladiator_shoulder_b` | Single Lamellar Pauldron Over Plated Leather | 16/8/4.1 | **22/12/2.8** |

- a：单肩甲 · body 20（Standard Pauldrons 35 的 ~57%，反映单侧覆盖）· arm 12 Standard Pauldrons · wt 2.5（单肩甲结构约半重）
- b：Plated Leather underlay +2 body · 其余同 a

**Cape 铁律校验**：命名含 Pauldron ✓ · body > arm ✓ · Standard Pauldrons arm 12 ✓

**关键设计决策**：单肩甲结构——**body 减半 · arm 不减**（因为覆盖那一侧上臂完整）· 这是角斗士家族独有的"非对称覆盖"设计

**状态**：2 件 🔵 log-only

---

## Empire · Cape · O. Studded Leather Shoulders + Cape

### 2026-09-23 · O 家族 1 件 · TV_ 包 Studded Leather + Cape

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_empire_shoulders_a` | Imperial Studded Leather Shoulders With Cape | 8/4/3.6 | **22/8/3.4** |

Studded Leather + Cape · vanilla `varangian_bra_royal` 22/0 参照 · arm 8 Leather 中档 · Cape 结构 body 已达 22 无需再加

**Cape 铁律校验**：命名含 Shoulders ✓ · body > arm ✓ ✓

**状态**：1 件 🔵 log-only

---

## Empire · Cape · P. Noble Shoulders

### 2026-09-23 · P.a 家族 1 件 · Noble Collar Shoulders + Cape

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_noble_collar_a` | Imperial Noble Shoulders With Cape | 16/8/1.4 | **24/10/2.5** |

Noble Shoulders + Cape · vanilla `empire_warrior_padded_armor_shoulder` 24/0（Padded 顶档） + Noble 品质字典（+0 结构性）· Cape 结构 body 已达 24 · arm 10 Chainmail Shoulders 档（Noble = 中高档结构性 arm）· wt v1 1.4 太轻，抬到 2.5

**注**：AR_noble_collar_b 因命名不含 shoulder/pauldron，归 Group B（见 R 家族附）

**Cape 铁律校验**：命名含 Shoulders ✓ · body > arm ✓ ✓

**状态**：1 件 🔵 log-only

---

## Empire · Cape · Q. lamellar_scarf（hmj_moretroops 独立包）

### 2026-09-23 · Q 家族 1 件 · G.1a Standard Lamellar Shoulders 直匹配

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `lamellar_scarf` | Imperial Lamellar Shoulders with Scarf | 18/9/4 | **40/20/3.8** |

命名与 G.1a `AR_imperial_shoulders_a` "Imperial Lamellar Shoulders Over Leather" 结构完全对应 · 同 G.1a Standard Lamellar Shoulders tier · Scarf 装饰 0

**Cape 铁律校验**：命名含 Shoulders ✓ · body > arm ✓ · G.1a tier ✓

**状态**：1 件 🔵 log-only

---

## Empire · Cape · S. Strip Shoulders + Long Cape

### 2026-09-23 · S 家族 1 件 · Strip Shoulders + Cape 复合

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_imperial_lamellar_cape_d` | Imperial Strip Shoulders With Long Cape | 16/6/3.9 | **28/8/4.0** |

**注**：id 前缀"lamellar_cape_d"但命名是"Strip Shoulders With Long Cape"——**以命名为准**（Rule 1 依据 name 而非 id）· 归 Group A · Strip Shoulders 档 + Long Cape 结构 body +2 vs G.3 Strip base 26

**Cape 铁律校验**：命名含 Shoulders ✓ · body > arm ✓ · Strip arm 8 ✓

**状态**：1 件 🔵 log-only

---

## Empire · Cape · J. Studded Neckguard 家族（Group B · arm 追溯清 0）

### 2026-09-23 · J 家族 8 件 · Group B 应用 · Neckguard 命名非 Shoulder/Pauldron

**背景**：全 8 件命名含"Neckguard"（护颈甲）**不含** shoulder/pauldron → **Group B 强制 arm = 0**。v1 脚本给 j/j2/k/k2/z/za/zb 加了 6-8 arm，追溯清零。

**vanilla RBM 参照**：
- `studded_imperial_neckguard` **31/0/3.6**（Neckguard with Bronze Plate Pauldrons · Studded Neckguard 唯一 vanilla 参照）
- `a_pauldron_cape_b` 20/0/3.5（Bronze Pauldrons with Neck Guard · Neck Guard 变体参照）

#### 子群 J.1 · Studded Imperial Neckguard 系列（1 件 · 无品质词）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_imperial_shoulders_i` | Studded Imperial Neckguard With Scarf | 16/0/3.6 | **28/0/3.6** |

Studded Neckguard 基础档 · body 28（比 vanilla `studded_imperial_neckguard` 31 稍弱，因为无 Bronze Plate Pauldrons 复合结构）· Scarf 装饰 0

#### 子群 J.2 · Studded Brass/Iron Neckguard + Leopard Pelt（4 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 2 | `AR_imperial_shoulders_j` | Studded Brass Imperial Neckguard With Leopard Pelt | 16/8/3.8 | **31/0/3.8** |
| 3 | `AR_imperial_shoulders_j2` | Studded Brass Imperial Neckguard With Snow Leopard Pelt | 16/8/3.8 | **31/0/3.8** |
| 4 | `AR_imperial_shoulders_k` | Studded Iron Imperial Neckguard With Leopard Pelt | 16/8/3.8 | **33/0/3.8** |
| 5 | `AR_imperial_shoulders_k2` | Studded Iron Imperial Neckguard With Snow Leopard Pelt | 16/8/3.8 | **33/0/3.8** |

- Brass Neckguard：vanilla `studded_imperial_neckguard` 31/0 直匹配 · Leopard/Snow Leopard 装饰 0
- Iron Neckguard：Brass +2 品质档

#### 子群 J.3 · 独立 Studded Neckguard（3 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 6 | `AR_imperial_shoulders_z` | Imperial Studded Iron Neckguard | 14/7/3.6 | **33/0/3.6** |
| 7 | `AR_imperial_shoulders_za` | Imperial Studded Brass Neckguard With Long Cape | 15/8/3.6 | **33/0/3.6** |
| 8 | `AR_imperial_shoulders_zb` | Imperial Studded Iron Neckguard With Long Cape | 15/8/3.6 | **35/0/3.6** |

- z：Studded Iron 单件 · body 33 = k 值
- za：Brass + Long Cape 结构 body +2 → 33
- zb：Iron + Long Cape → 35

**Cape 铁律校验**：全 8 件命名不含 shoulder/pauldron ✓ · **Group B 应用 arm = 0** ✓ · v1 违规的 6/8/7/8/8/8 arm 全部追溯清零

**状态**：8 件 🔵 log-only（Group B 首次批量追溯）

---

## Empire · Cape · R. Lamellar 单/Cape 组合（Group B · 命名无 shoulder/pauldron）

### 2026-09-23 · R 家族 8 件 · 严格 Rule 1B 应用

**背景**：命名格式"Imperial [Gilded] Lamellar With [Long/Fur/Plaid/Striped] Cape [+ Bear Pelt]"——**不含** shoulder/pauldron → **Group B 强制 arm = 0**。v1 给了 8-9 arm 违反 Rule 1B，追溯清零。用户可基于 vanilla+RBM 参照决定是否需要 override 到 Group A。

**vanilla RBM 参照**：
- `pauldron_cape_a` 30/0/3.5（Legionary Cape）
- `imperial_lamellar_shoulders` 55/0/3.5 ⭐（Empire Cape body-only 顶点）

#### 子群 R.1 · Standard Lamellar + Cape（4 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_imperial_lamellar_cape_a` | Imperial Lamellar With Long Cape | 18/9/3.9 | **45/0/4.0** |
| 2 | `AR_imperial_lamellar_cape_b` | Imperial Lamellar With Cape | 17/9/3.9 | **45/0/4.0** |
| 3 | `AR_imperial_lamellar_cape_c` | Imperial Lamellar With Fur Cape | 18/9/3.9 | **45/0/4.0** |
| 4 | `AR_imperial_lamellar_cape_e` | Imperial Lamellar With Long Plaid Cape | 16/8/3.9 | **45/0/4.0** |

body 45（介于 vanilla `pauldron_cape_a` 30 与 `imperial_lamellar_shoulders` 55 之间 · Lamellar + Cape 复合中高档）· Long/Fur/Plaid 装饰 0

#### 子群 R.2 · Standard Lamellar + Cape 变体（2 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 5 | `AR_imperial_lamellar_cape_f` | Imperial Lamellar With Long Striped Cape | 16/8/3.9 | **45/0/4.0** |
| 6 | `AR_imperial_lamellar_cape_g` | Imperial Lamellar With Bear Pelt | 16/8/3.9 | **45/0/4.0** |

Striped/Bear Pelt 装饰 0

#### 子群 R.3 · Gilded Lamellar + Cape（2 件 · Gilded +2 body）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 7 | `AR_imperial_lamellar_cape_a2` | Imperial Gilded Lamellar With Long Cape | 18/9/3.9 | **47/0/4.0** |
| 8 | `AR_imperial_lamellar_cape_b2` | Imperial Gilded Lamellar With Cape | 17/9/3.9 | **47/0/4.0** |

Gilded 品质词 +2 body（跨类型统一）

### R 附 · P.b Noble Lamellar + Long Cape（1 件 · 同 Group B 逻辑）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| R+ | `AR_noble_collar_b` | Imperial Noble Lamellar With Long Cape | 18/9/3.9 | **50/0/4.0** |

命名不含 shoulder/pauldron → Group B · Noble 品质字典 +3 body vs Standard Lamellar+Cape 45 → 48-50 · 顶档接近 vanilla `imperial_lamellar_shoulders` 55 但不逾

**Cape 铁律校验**：全 9 件命名不含 shoulder/pauldron ✓ · **Group B 应用 arm = 0** ✓ · body ≤ 50 < vanilla 顶点 55 ✓

**状态**：8 + 1 = 9 件 🔵 log-only

---

## Empire · Cape · T. 无肩甲结构 Cape/Cloak/Sash/Focale/Pelt（Group B · arm 追溯清 0）

### 2026-09-23 · T 家族 12 件 · Group B 应用 · 无肩甲命名批量追溯

**背景**：命名为 Cape/Cloak/Sash/Focale/Pelt/无肩甲词——**Group B 强制 arm = 0**。v1 部分给了 2-4 arm，追溯清零。装饰类物品用户可能一直不装备，属于**低价值 cosmetic 类**，主要目的：符合 Cape 铁律避免与其他 arm 加成叠加异常。

**vanilla 参照**：Empire Cape 无严格 Focale/Pelt 参照 · 用轻装 Cloth 档 fallback（body ≤ 8）

#### 子群 T.1 · Focale/Sash/Pelt（3 件 · 极轻装饰）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `ao_focale` | Imperial Focale | 1/0/0.1 | **1/0/0.1** |
| 2 | `ao_imperial_noblemans_sash` | Imperial Noble Sash | 3/2/0.5 | **5/0/0.5** |
| 3 | `AR_leopard_pelt_a` | Snow Leopard Pelt | 4/0/2 | **6/0/1.5** |

- ao_focale：保持 v1（已符合 Rule 1B）
- Sash：Noble 品质 +2 body 5，arm 追溯清 0
- Pelt：轻装饰 body 6，wt 从 2 降到 1.5

#### 子群 T.2 · Lion Head Cloak 系列（3 件 · 装饰+防御中档）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 4 | `AR_lion_head_a` | Decorated Lion Head Cloak | 16/4/6 | **22/0/5.0** |
| 5 | `AR_lion_head_b` | Plated Lion Head Cloak | 18/4/7.4 | **28/0/5.0** |
| 6 | `AR_lion_head_d` | Lion Head Cloak | 16/4/6 | **20/0/5.0** |

Cloak 结构中档 body ~20-28（Plated Lion Head +6 反映甲片补强）· arm 追溯清 0 · wt 5.0（Cloak 重装）

#### 子群 T.3 · Battania Cape/Cloth Cape（6 件 · TV_ 包轻装 Cape）

| # | id | 游戏名 | v1 XML | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 7 | `TV_battania_cloak_m` | Long Cape | 3/2/0.5 | **5/0/0.5** |
| 8 | `TV_battania_cloak_m2` | Long Plaid Cape | 3/2/0.5 | **5/0/0.5** |
| 9 | `TV_battania_cloak_m3` | Long Striped Cape | 3/2/0.5 | **5/0/0.5** |
| 10 | `TV_battania_cloak_o` | Cloth Cape | 2/0/0.2 | **2/0/0.2** |
| 11 | `TV_battania_cloak_o2` | Plaid Cloth Cape | 6/0/4.5 | **8/0/1.5** |
| 12 | `TV_battania_cloak_o3` | Striped Cloth Cape | 6/0/4.5 | **8/0/1.5** |

- m/m2/m3：Long Cape 轻装 body 5，arm 清 0
- o：保持 v1（已 0 arm 且极轻）
- o2/o3：Cloth Cape 中档 body 8，wt v1 4.5 显然是 bug（轻布 cape 不可能 4.5 kg），降到 1.5

**Cape 铁律校验**：全 12 件命名不含 shoulder/pauldron ✓ · **Group B 应用 arm = 0** ✓ · v1 违规的 sash/lion_head/cloak_m 系列 arm 全部追溯清零

**状态**：12 件 🔵 log-only（Group B 无肩甲收尾批）

---

## Empire Cape 收官统计（2026-09-23）

**总数**：89 件 Empire Cape · **全部审完**

**Group A（命名含 shoulder/pauldron，arm > 0）**：
- G. Plate Lamellar Shoulders/Pauldrons：19 件
- F. Scale Shoulders：8 件（视觉判断优先律 6 件 arm=0）
- H. Wolf Pelt 变体：10 件
- I. Bronze/Iron Plate Pauldrons：5 件
- K. Leather Shoulders + Cape：9 件
- L. Mail Shoulders：1 件
- M. Studded Strip Shoulders 轻档：2 件
- N. Gladiator Single Pauldron：2 件
- O. Studded Leather + Cape (TV_)：1 件
- P.a. Noble Shoulders + Cape：1 件
- Q. lamellar_scarf (hmj)：1 件
- S. Strip Shoulders + Long Cape：1 件
- **Group A 合计：60 件**

**Group B（命名不含 shoulder/pauldron，arm=0 追溯）**：
- J. Studded Neckguard：8 件
- R. Lamellar + Cape combo：8 件
- R+. Noble Lamellar + Cape (P.b)：1 件
- T. Cape/Cloak/Sash/Focale/Pelt：12 件
- **Group B 合计：29 件**

**60 + 29 = 89 件 ✓**（枚举核实一致）

**下一步（用户 2026-09-23 决定）**：
1. 用户直读本 log 对照 vanilla+RBM 参照，标记任何需 override 的物品
2. 用户批准后 `deploy.ps1` 全部落地 XML override
3. 进入 Vlandia Cape 家族（同律沿用）

---

# Empire · BodyArmor（2026-09-23 起）

## 🔒 铁律 · BodyArmor 设计基调（2026-09-23 用户拍板 · 已写入 VANILLA_REFERENCE.md）

> BodyArmor 无 head_armor 字段，取代"头 > 身 > 臂"的是：`body_armor ≥ leg_armor > arm_armor` 三档序（vanilla 全 39 件遵守，仅 Scale Skirt 结构 leg 略超 body 1-4 点）。
>
> **材质硬约束**：Cloth ≤ 28（Padded 顶）· Leather ≤ 32 · Chainmail 45-55 · Plate 37-135
>
> **顶点参照**：`imperial_scale_armor` 135/122/67（Heavy Scale over Double Mail）— OSA body ≤ 135 严格上限
>
> **结构描述子分档**：`Over Padded/Leather` 中档 · `Over Mail` 高档 · `Over Scale/Double Mail` 顶档 · `Scale Skirt` leg 强化
>
> **Cataphract 定义（body ≥ 79 + leg ≥ 45 + arm ≥ 20）**：Empire 重装骑兵顶级三档全上

---

## Empire · BodyArmor · 家族分类总览（75 件）

| 家族 | n | body 档 | vanilla 锚点 | 结构描述 |
|---|---:|---|---|---|
| **A. Cloth 民用** | 16 | 1-8 | `hemp_tunic` 5 · `imperial_robes` 8 · `footmans_tunic` 8 | Tunic/Dress/Toga/Skirt/Robes |
| **B. Cloth 中档 Subarmalis** | 4 | 14-16 | `patched_gambeson` 14 · `imperial_padded_cloth` 16 | Cavalry Tunic / Cloth Subarmalis |
| **C. Leather Vest/Cuirass** | 6 | 16-22 | `leather_tunic` 18 · `khuzait_leather_stitched` 22 | Leather Vest/Cuirass Over Tunic |
| **D. Plate 轻档 · Over Padded/Leather** | 13 | 18-30 | `empire_warrior_padded_armor_b` 28 · `empire_plate_vest_armor` 37 | Breastplate/Lamellar Over Padded/Leather |
| **E. Chainmail 独立** | 6 | 27-46 | `imperial_mail_vest` 45 · `imperial_mail_over_leather` 47 · `empire_legion_a` 55 | Mail Shirt/Vest/Over Cloth/Leather |
| **F. Plate 中档 · Scale/Lamellar Over Leather/Mail** | 15 | 33-42 | `imperial_mail_over_stripped_leather` 46 · `empire_horseman_armor` 55 | Scale/Lamellar Cuirass Over Mail |
| **G. Plate 高档 · Heavy Scale/Lamellar Over Mail** | 13 | 47-52 | `imperial_lamellar` 79 · `empire_legion_b` 85 · `imperial_lamellar_over_leather` 88 | Heavy Scale/Lamellar Over Mail |
| **H. Cataphract 顶档 · Lamellar Over Scale** | 2 | 55 | `lamellar_with_scale_skirt` 118 · `imperial_scale_armor` 135 | Lamellar Over Scale (Cataphract) |

**合计**：16+4+6+13+6+15+13+2 = **75 件 ✓**

---

## Empire · BodyArmor · A 家族：Cloth 民用（16 件）

### 2026-09-23 · A 家族 16 件 · vanilla 民用档直匹配

**vanilla 参照**：`hemp_tunic` 5/5/5 · `empire_dress` 6/5/6 · `imperial_robes` 8/8/6 · `footmans_tunic` 8/8/8 · `tunic_with_rolled_cloth` 11/11/7

#### 子群 A.1 · Skirts/Trousers/Dresses（4 件 · 极轻档 · 装饰）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_empire_armor_f` | Imperial Skirted Trousers | 1/3/0/0.4 | **3/3/2/0.4** |
| 2 | `AR_empire_armor_g` | Imperial Belted Skirt | 1/2/0/0.4 | **3/3/2/0.4** |
| 3 | `AR_noble_dress_a` | Decorated Rich Dress | 3/1/1/0.5 | **7/5/5/0.5** (vanilla `vlandian_dress` 直匹配) |
| 4 | `AR_noble_dress_a2` | Patterned Rich Dress | 3/1/1/0.5 | **7/5/5/0.5** |

#### 子群 A.2 · Simple Tunics（7 件 · 民用轻档）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 5 | `AR_vlandia_armor_g` | Western Tunic With Clavi | 4/2/1/0.4 | **6/5/5/0.4** (vanilla `fine_town_tunic` 直匹配) |
| 6 | `plain_hemp_tunic` | Plain Hemp Tunic | 4/2/1/0.4 | **5/5/5/0.4** (vanilla `hemp_tunic` 直匹配) |
| 7 | `AR_empire_armor_p` | Palaic Chiton | 4/9/0/0.7 | **6/6/4/0.5** |
| 8 | `AO_imperial_scouts_tunic` | Imperial Explorator's Tunic | 5/1/1/0.4 | **7/5/5/0.4** |
| 9 | `AR_empire_armor_k` | Tunic With Shoulder Pads And Pteruges | 6/3/2/0.6 | **8/7/7/0.6** (vanilla `tunic_with_shoulder_pads` 7/7/7 参照) |
| 10 | `AR_empire_armor_q` | Imperial Decorated Explorator's Tunic | 6/1/1/0.4 | **7/5/5/0.4** |
| 11 | `TV_empire_armor_k` | Imperial Decorated Short Tunic | 6/0/0/1.0 | **8/3/2/1.0** (vanilla `empire_short_dress` 8/3/0 参照) |

#### 子群 A.3 · Robes/Toga/Merchant Coat（5 件 · 民用中档）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 12 | `AR_empire_armor_b` | Imperial Toga With Tunic | 8/3/4/1.9 | **8/8/6/1.5** (vanilla `imperial_robes` 直匹配) |
| 13 | `AR_empire_armor_m` | Imperial Robes | 8/3/4/0.9 | **8/8/6/0.9** |
| 14 | `AR_empire_armor_n` | Imperial White Robes | 8/3/4/0.9 | **8/8/6/0.9** |
| 15 | `AR_empire_armor_t` | Imperial Noble Robes | 8/3/4/0.9 | **10/8/6/0.9** (Noble +2 body) |
| 16 | `AR_merchants_coat_a` | Merchant's Coat | 8/3/4/0.9 | **8/8/6/1.0** |

**BodyArmor 铁律校验**：全 16 件 body ≥ leg > arm ✓ · Cloth ≤ 28 ✓

**状态**：16 件 🔵 log-only

---

## Empire · BodyArmor · B 家族：Cloth 中档 Subarmalis + Cavalry Tunic（4 件）

### 2026-09-23 · B 家族 4 件 · Padded/Subarmalis 中档

**vanilla 参照**：`patched_gambeson` 14/14/12 · `imperial_padded_cloth` 16/16/15 · `empire_warrior_padded_armor_a/e` 18/18/14-15

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_empire_scout_armor_c` | Imperial Cavalry Tunic | 14/3/3/1.9 | **14/14/12/1.9** (vanilla `patched_gambeson` 直匹配) |
| 2 | `AR_huntress_armor_a` | Huntress' Leather Breastplate With Skirt | 14/2/3/1.9 | **14/14/12/1.9** |
| 3 | `AR_empire_armor_e` | Imperial Cloth Subarmalis Over Tunic | 15/12/6/2.1 | **15/15/13/2.1** |
| 4 | `AR_empire_armor_d` | Imperial Decorated Cloth Subarmalis | 16/12/6/2.1 | **16/16/14/2.1** (vanilla `imperial_padded_cloth` 直匹配) |

**BodyArmor 铁律校验**：全 4 件 body ≥ leg > arm ✓ · Cloth 中档 14-16 ✓

**状态**：4 件 🔵 log-only

---

## Empire · BodyArmor · C 家族：Leather Vest/Cuirass（6 件）

### 2026-09-23 · C 家族 6 件 · Leather Vest/Cuirass 分档

**vanilla 参照**：`leather_tunic` 18/23/15 · `khuzait_leather_stitched` 22/22/10 · `basic_imperial_leather_armor` 28/22/25

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_empire_dress_armor_a` | Imperial Dress With Leather Vest | 16/2/2/1.9 | **18/18/12/2.0** (Leather Vest 轻档) |
| 2 | `AR_huntress_armor_c` | Huntress' Open Leather Vest With Skirt | 16/2/3/1.9 | **18/18/12/2.0** |
| 3 | `ao_imperial_light_leather` | Imperial Rugged Leather Armor | 18/5/6/3.1 | **18/23/15/2.1** (vanilla `leather_tunic` 直匹配) |
| 4 | `AR_empire_armor_o` | Imperial Leather Cuirass Over Tunic With Pteruges | 18/4/3/0.6 | **20/22/13/2.5** (wt v1 0.6 太轻，抬到 2.5) |
| 5 | `AR_empire_scout_armor_a` | Imperial Cavalry Padded Cloth | 20/8/6/6.2 | **22/22/13/3.0** (Cavalry Padded Leather 中档 · wt v1 6.2 过重降到 3.0) |
| 6 | `AR_empire_armor_a` | Imperial Leather Subarmalis Over Tunic | 22/6/6/3.1 | **22/22/14/3.1** (vanilla `khuzait_leather_stitched` 22 直匹配) |

**BodyArmor 铁律校验**：全 6 件 body ≥ leg > arm（除 leather_tunic 家族 leg 23 > body 18 参照 vanilla 特色 · 3-4 号 leg 22 = body 22 齐）✓ · Leather ≤ 32 ✓

**状态**：6 件 🔵 log-only

---

## Empire · BodyArmor · D 家族：Plate 轻档 · Over Padded/Leather（13 件）

### 2026-09-23 · D 家族 13 件 · Breastplate/Lamellar Over Padded 中低档

**vanilla 参照**：`empire_warrior_padded_armor_f/g/b` 25-28（Padded 顶）· `empire_plate_vest_armor` 37/30/12（Scale Vest 唯一）

#### 子群 D.1 · Southern Lamellar/Leather Over Padded（3 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_aserai_leather_a` | Southern Leather Cuirass Over Padded Cloth | 18/8/6/6.2 | **28/22/15/5.0** (Padded 顶 + Leather Cuirass) |
| 2 | `AR_empire_brass_lamellar_b` | Southern Brass Lamellar Over Padded Cloth | 24/8/6/6.2 | **32/25/15/6.0** (Brass Lamellar 中档) |
| 3 | `AR_empire_brass_lamellar_d` | Southern White Brass Lamellar Over Padded Cloth | 24/8/6/6.2 | **34/25/15/6.0** (White Brass +2) |

#### 子群 D.2 · Explorator's Mail（2 件 · 侦查兵 Chainmail 变体命名）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 4 | `AR_empire_armor_r` | Imperial Explorator's Mail | 26/16/16/9.5 | **32/22/25/9.0** (Explorator 中档) |
| 5 | `AR_empire_armor_s` | Imperial Decorated Explorator's Mail | 26/16/16/9.5 | **32/22/25/9.0** |

#### 子群 D.3 · Lamellar Vest Over Padded（4 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 6 | `ao_imperial_lamellar_vest` | Imperial Steel Lamellar Over Padded Cloth | 28/9/6/7 | **37/25/15/7.0** (Steel Lamellar over Padded · vanilla `empire_plate_vest_armor` 37 参照) |
| 7 | `ao_imperial_lamellar_vest_b` | Imperial Brass Lamellar Over Padded Cloth | 28/9/6/7 | **35/25/15/7.0** (Brass -2 vs Steel) |
| 8 | `TV_empire_armor_l` | Imperial Steel Lamellar Over Padded Coat | 28/8/4/7 | **37/25/12/7.0** |
| 9 | `TV_empire_armor_l2` | Imperial Brass Lamellar Over Padded Coat | 28/8/4/7 | **35/25/12/7.0** |

#### 子群 D.4 · Iron Breastplate Over Cloth/Leather（4 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 10 | `AR_empire_scout_armor_d` | Imperial Breastplate Over Tunic | 28/3/3/1.9 | **33/15/10/6.0** (breastplate 中档 + Tunic 底层 · wt v1 1.9 过轻抬到 6.0) |
| 11 | `AR_huntress_armor_b` | Huntress' Iron Breastplate With Skirt | 28/2/3/1.9 | **32/15/10/6.0** |
| 12 | `AR_empire_horseman_armor_d` | Imperial Breastplate Over Leather | 30/6/6/8.3 | **37/22/15/8.0** (Breastplate + Leather 底层) |
| 13 | `AR_empire_scout_armor_b` | Imperial Breastplate Over Padded Cloth | 30/8/6/6.2 | **37/25/15/7.0** |

**BodyArmor 铁律校验**：全 13 件 body ≥ leg > arm ✓ · Plate 轻档 28-37 ✓ · body 顶 37 = vanilla `empire_plate_vest_armor` 顶 ✓

**状态**：13 件 🔵 log-only

---

## Empire · BodyArmor · E 家族：Chainmail 独立（6 件）

### 2026-09-23 · E 家族 6 件 · vanilla Chainmail 分档直匹配

**vanilla 参照**：
- `imperial_mail_vest` 45/34/39（Infantryman Mail Vest 中档）
- `imperial_mail_over_stripped_leather` 46/27/30
- `imperial_mail_over_leather` 47/22/33
- `empire_horseman_armor` 55/27/38（Cavalryman Mail Shirt 高档）
- `empire_legion_a` 55/44/18（Decorated Legionary Mail）
- `legionary_mail` 55/49/44（Legionary Mail 顶）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_empire_horseman_armor_c` | Imperial Cavalry Leather Armor | 27/6/6/8.3 | **47/22/33/8.6** (vanilla `imperial_mail_over_leather` 直匹配) |
| 2 | `TV_empire_armor_p` | Imperial Studded Leather Over Mail Shirt | 30/12/10/3 | **45/22/30/9.0** (Studded Leather Over Mail · wt v1 3 过轻抬到 9.0) |
| 3 | `AR_empire_armor_c` | Imperial Decorated Mail Over Stripped Cloth | 33/12/10/10.2 | **46/27/30/10.2** (vanilla `imperial_mail_over_stripped_leather` 直匹配) |
| 4 | `AR_empire_armor_j` | Imperial Legionary Mail With Strips | 35/18/15/19 | **55/44/18/22** (vanilla `empire_legion_a` Decorated Legionary Mail 直匹配) |
| 5 | `ao_imperial_lamellar_vest_over_mail` | Imperial White Brass Lamellar Over Mail | 46/12/12/14 | **55/34/38/12** (Lamellar Vest Over Mail 高档 · body 达 55 顶 chainmail) |
| 6 | `ao_imperial_lamellar_vest_over_mail_b` | Imperial Brass Lamellar Over Mail | 46/12/12/14 | **53/34/38/12** (Brass -2 vs White Brass) |

**BodyArmor 铁律校验**：全 6 件 body ≥ leg > arm 参照 vanilla 特色 ✓ · Chainmail 45-55 ✓ · 3 件 vanilla 直匹配

**状态**：6 件 🔵 log-only

---

## Empire · BodyArmor · F 家族：Plate 中档 · Scale/Lamellar Over Leather/Mail（15 件）

### 2026-09-23 · F 家族 15 件 · Plate 中档 Cavalry/Scale/Lamellar

**vanilla 参照**：
- `imperial_mail_over_leather` 47/22/33 · `empire_horseman_armor` 55/27/38（中高档 chainmail-scale 过渡）
- 无严格 Plate 中档 (55-79) vanilla anchor · 走 chainmail 顶 55 到 Plate 高档 79 之间的插值

#### 子群 F.1 · Gilded/Auxiliary Lamellar（2 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `tv_empire_armor_b` | Imperial Gilded Lamellar Over Boiled Leather | 33/7/6/15 | **60/30/25/15** (Gilded 中高档 body 60) |
| 2 | `TV_empire_armor_i` | Imperial Auxiliary Lamellar Armor With Straps | 33/8/4/2 | **45/22/15/5.0** (Auxiliary 中档 · wt v1 2 过轻抬到 5.0) |

#### 子群 F.2 · Breastplate Over Mail（2 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 3 | `AR_empire_horseman_armor` | Imperial Breastplate Over Mail | 34/12/8/9.5 | **55/27/38/9.5** (vanilla `empire_horseman_armor` 直匹配) |
| 4 | `AR_empire_horseman_armor_b` | Imperial Gilded Breastplate Over Mail | 34/12/8/9.5 | **57/27/38/9.5** (Gilded +2 body) |

#### 子群 F.3 · Scale Cuirass Over Stripped Cloth/Leather（5 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 5 | `TV_empire_armor_f` | Imperial Alternating Scale Cuirass Over Red Stripped Cloth | 34/6/6/16.5 | **47/22/33/14** (Alternating Scale 中档 · vanilla `imperial_mail_over_leather` 参照 · wt 从 16.5 略降到 14) |
| 6 | `TV_empire_armor_g` | Imperial Brass Scale Cuirass Over White Stripped Cloth | 34/6/6/16.5 | **47/22/33/14** |
| 7 | `TV_empire_armor_h` | Imperial Steel Scale Cuirass Over Stripped Leather | 34/6/6/16.5 | **49/22/33/14** (Steel +2 vs Brass) |
| 8 | `AR_empire_scale_armor_a` | Imperial Brass Scale Cuirass Over Stripped Leather | 36/12/8/15.1 | **50/25/33/15** |
| 9 | `AR_empire_scale_armor_b` | Imperial Steel Scale Cuirass Over Stripped Leather | 36/12/8/15.1 | **52/25/33/15** (Steel +2) |

#### 子群 F.4 · Lamellar Vest/Cavalry Lamellar Over Leather/Mail（4 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 10 | `tv_empire_armor_a` | Imperial Lamellar Vest Over Stripped Mail | 36/10/6/15 | **55/30/38/13** (Lamellar Vest Over Mail 高档) |
| 11 | `AR_empire_brass_lamellar_a` | Imperial Brass Lamellar Over Leather | 38/12/10/12 | **55/27/38/12** (Brass Lamellar Over Leather 高档) |
| 12 | `AR_empire_brass_lamellar_c` | Imperial White Brass Lamellar Over Leather | 38/12/10/12 | **57/27/38/12** (White Brass +2) |
| 13 | `TV_empire_armor_j` | Imperial Cavalry Lamellar Armor | 40/11/10/8.3 | **55/27/38/10** (Cavalry Lamellar 中高档) |

#### 子群 F.5 · Breastplate Over Steel/Brass Scale（2 件 · F 顶档）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 14 | `AR_empire_horseman_armor_a` | Imperial Breastplate Over Steel Scale | 42/20/12/14 | **62/40/25/13** (Breastplate + Scale 顶级中档) |
| 15 | `AR_empire_horseman_armor_a2` | Imperial Gilded Breastplate Over Brass Scale | 42/20/12/14 | **65/40/25/13** (Gilded +3 body) |

**BodyArmor 铁律校验**：全 15 件 body ≥ leg > arm ✓ · Plate 中档 45-65 ✓ · 走 chainmail 顶 55 与 Plate 高档 79 之间的插值

**状态**：15 件 🔵 log-only

---

## Empire · BodyArmor · G 家族：Plate 高档 · Heavy Scale/Lamellar Over Mail（13 件）

### 2026-09-23 · G 家族 13 件 · vanilla Plate 高档直匹配

**vanilla 参照**：
- `imperial_lamellar` **79/78/45**（Light Lamellar over Mail ⭐ Plate 高档 anchor）
- `empire_legion_b` 85/30/20（Ornate Legionary Scale Mail）
- `imperial_lamellar_over_leather` **88/30/35**（Luxury Lamellar Vest over Leather）

#### 子群 G.1 · Heavy Scale/Lamellar Over Mail（AO 系列）（2 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `ao_imperial_southern_scale_armor_over_mail` | Imperial Heavy Scale Armor Over Mail | 47/23/14/20 | **79/78/45/25** (vanilla `imperial_lamellar` 直匹配) |
| 2 | `ao_imperial_southern_scale_armor_over_mail_b` | Imperial Heavy Brass Scale Armor Over Mail | 47/23/14/20 | **82/78/45/25** (Brass +3 body) |

#### 子群 G.2 · Steel/Brass Scale Armor（DZ 系列）（2 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 3 | `DZ_empire_armor_d` | Imperial Steel Scale Armor | 47/12/12/20 | **80/45/30/20** (Steel Scale 独立高档) |
| 4 | `DZ_empire_armor_d2` | Imperial Brass Scale Armor | 47/12/12/20 | **78/45/30/20** (Brass -2 vs Steel) |

#### 子群 G.3 · Lamellar/Scale Cuirass Over Stripped Cloth（4 件）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 5 | `AR_empire_armor_h` | Imperial Lamellar Cuirass Over Stripped Cloth | 48/12/10/15.1 | **79/45/30/16** (Lamellar Cuirass Over Cloth) |
| 6 | `AR_empire_scale_armor_f` | Imperial Decorated Scale Cuirass Over Stripped Cloth | 50/12/10/15.1 | **82/45/30/16** (Decorated +3) |
| 7 | `AR_empire_scale_armor_g` | Imperial Decorated Brass Scale Cuirass Over Stripped Cloth | 50/12/10/15.1 | **82/45/30/16** |
| 8 | `AR_empire_scale_armor_h` | Imperial Decorated Alternating Scale Cuirass Over Stripped Cloth | 50/12/10/15.1 | **82/45/30/16** |

#### 子群 G.4 · Long Scale/Lamellar Cuirass Over Mail（5 件 · 顶档 anchor `imperial_lamellar_over_leather` 88）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 9 | `AR_empire_armor_i` | Imperial Lamellar Cuirass Over Mailed Leather | 52/18/10/20.1 | **88/45/35/20** (vanilla `imperial_lamellar_over_leather` 直匹配) |
| 10 | `AR_empire_scale_armor_c` | Imperial Long Steel Scale Cuirass Over Mail | 52/20/10/20.1 | **85/55/35/20** (Long +10 leg) |
| 11 | `AR_empire_scale_armor_d` | Imperial Long Brass Scale Cuirass Over Mail | 52/20/10/20.1 | **83/55/35/20** (Brass -2 vs Steel) |
| 12 | `TV_empire_armor_d` | Imperial Decorated Brass Heavy Scale Armour | 52/23/16/24.1 | **85/78/45/24** (Heavy Scale approaching Cataphract) |
| 13 | `TV_empire_armor_m` | Imperial Long Scale Cuirass Over Mail | 52/23/10/34 | **85/55/35/22** (wt v1 34 过重降到 22) |

**BodyArmor 铁律校验**：全 13 件 body ≥ leg > arm（除 G.1 参照 vanilla `imperial_lamellar` 78 leg ≈ 79 body）✓ · Plate 高档 78-88 ✓

**状态**：13 件 🔵 log-only

---

## Empire · BodyArmor · H 家族：Cataphract 顶档 · Lamellar Over Scale（2 件）

### 2026-09-23 · H 家族 2 件 · Cataphract 三层结构（Lamellar + Scale + Mail）

**vanilla 参照**：
- `lamellar_with_scale_skirt` **118/122/45**（Heavy Lamellar over Mail with Scale Skirt · Cataphract 亚顶）
- `imperial_scale_armor` **135/122/67** ⭐（Heavy Scale Armor over Double Mail · vanilla Empire BodyArmor 顶点）

| # | id | 游戏名 | v1 XML | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `ao_imperial_cataphracts_lamellar` | Imperial White Brass Lamellar Over Scale | 55/18/16/23 | **118/78/45/28** (Cataphract 顶档 · `lamellar_with_scale_skirt` 118 匹配 · leg 78 从 122 略降因 OSA 无 Scale Skirt 结构) |
| 2 | `ao_imperial_cataphracts_lamellar_b` | Imperial Brass Lamellar Over Scale | 55/18/16/23 | **115/78/45/28** (Brass -3 vs White Brass) |

**BodyArmor 铁律校验**：全 2 件 body ≥ leg > arm ✓ · body ≤ 135 顶点 ✓ · Cataphract 定义（body ≥ 79 + leg ≥ 45 + arm ≥ 20）✓

**关键设计决策**：OSA Cataphract 命名"Lamellar Over Scale"——vanilla anchor `lamellar_with_scale_skirt` 118 是"Lamellar Over Mail + Scale Skirt"，两者结构近似（三层），但 OSA 版无独立 Scale Skirt → leg 78 从 vanilla 122 略降，反映"腿部无独立护甲、由 Scale 底层延伸"。arm 45 保持 vanilla `imperial_lamellar` 参照。

**状态**：2 件 🔵 log-only

---

## Empire BodyArmor 收官统计（2026-09-23）

**总数**：75 件 Empire BodyArmor · **全部审完 · 全部 🔵 log-only**

| 家族 | n | body 目标 | vanilla 锚点 |
|---|---:|---|---|
| A. Cloth 民用 | 16 | 3-10 | `hemp_tunic` 5 / `imperial_robes` 8 |
| B. Cloth 中档 Subarmalis | 4 | 14-16 | `patched_gambeson` 14 / `imperial_padded_cloth` 16 |
| C. Leather Vest/Cuirass | 6 | 18-22 | `leather_tunic` 18 / `khuzait_leather_stitched` 22 |
| D. Plate 轻档 Over Padded/Leather | 13 | 28-37 | `empire_warrior_padded_armor_b` 28 / `empire_plate_vest_armor` 37 |
| E. Chainmail 独立 | 6 | 45-55 | `imperial_mail_over_leather` 47 / `empire_legion_a` 55 |
| F. Plate 中档 Scale/Lamellar Over Mail | 15 | 45-65 | `empire_horseman_armor` 55 （中高档过渡）|
| G. Plate 高档 Heavy Scale/Lamellar | 13 | 78-88 | `imperial_lamellar` 79 / `imperial_lamellar_over_leather` 88 |
| H. Cataphract 顶档 Lamellar Over Scale | 2 | 115-118 | `lamellar_with_scale_skirt` 118 |

**16+4+6+13+6+15+13+2 = 75 ✓**

**关键设计观察**：
- OSA v1 值系统性偏低——Plate 顶档物品 v1 body 47-55，vanilla+RBM 高档档已到 79-88，Cataphract 顶到 118-135。v2 buff ×1.5-2.5 让 OSA 与 vanilla+RBM 尺度对齐
- Weight 平衡：13 件 wt 从 v1 过轻（1.9-8.3）抬到合理值；4 件 wt 从 v1 过重（16.5-34）降到合理值
- vanilla 直匹配 8 件（B.4, C.3, C.6, E.1, E.3, E.4, F.3, G.1, G.9）——OSA 命名与 vanilla anchor 结构完全对应
- Cataphract 定义在 OSA 侧唯一 2 件——vs 帝国头盔 Cataphract 家族 10 件、Cape G.1b Gilded 5 件，Empire BodyArmor Cataphract 数量最少

**下一步**：
1. 用户直读本 log 对照 vanilla+RBM 参照，标记任何需 override 的物品
2. 用户批准后 `deploy.ps1` 全部落地 XML override
3. 进入 Empire HandArmor / LegArmor / HorseHarness（推荐 HorseHarness 因优先度高 · Saddlery 需 ×0.5 反向下调）

---

# Empire · HandArmor（2026-09-23）

## 🔒 铁律 · HandArmor 设计基调

> HandArmor **body_armor 恒为 0**（字段无效），所有护甲值集中在 arm_armor。分档硬约束：
>
> **材质硬约束**：Cloth Padded ≤ 37 · Leather ≤ 26 · Plate 42-63
> **顶点参照**：`lamellar_plate_gauntlets` 63/1.8
> **命名子结构**：Vambraces（前臂）< Gauntlets（全手）· Bracers 轻档 · Mittens 布软档 · Splint/Mail 复合结构档

---

## Empire · HandArmor · 家族分类总览（12 件）

| 家族 | n | arm 目标 | vanilla 锚点 |
|---|---:|---|---|
| **A. Cloth Armwrap/Vambrace 轻档** | 2 | 8-20 | 无 vanilla anchor · Cloth 民用 |
| **B. Plate Vambraces 中低档** | 2 | 32-37 | `reinforced_padded_mitten` 37 |
| **C. Lamellar Vambraces 中档** | 2 | 42-45 | `plated_strip_gauntlets` 42 |
| **D. Lamellar Plate Gloves 中高档** | 2 | 50-53 | `decorated_imperial_gauntlets` 50 |
| **E. Plate/Mail Gauntlets 顶档** | 4 | 42-63 | `lamellar_plate_gauntlets` 63 ⭐ |

**2+2+2+2+4 = 12 件 ✓**

---

## Empire · HandArmor · A 家族：Cloth Armwrap/Vambrace 轻档（2 件）

### 2026-09-23 · A 家族 2 件 · Cloth 民用轻装

| # | id | 游戏名 | v1 XML | **v2 决议 arm/wt** |
|---|---|---|---|---|
| 1 | `AR_empire_gloves_e` | Bound Armwraps | 3/0.4 | **8/0.5** (Cloth 民用极轻档) |
| 2 | `AR_empire_gloves_a` | Lordly Vambrace | 10/0.6 | **20/0.6** (Cloth 民用中档 · Lordly 品质) |

**HandArmor 铁律校验**：body = 0 ✓ · arm ≤ 63 ✓ · Cloth ≤ 37 ✓

**状态**：2 件 🔵 log-only

---

## Empire · HandArmor · B 家族：Plate Vambraces 中低档（2 件）

### 2026-09-23 · B 家族 2 件 · Plate 装饰 Vambraces

| # | id | 游戏名 | v1 XML | **v2 决议 arm/wt** |
|---|---|---|---|---|
| 1 | `ao_imperial_decorated_bracers` | Plate Vambraces | 14/0.5 | **32/0.8** (Plate Vambraces 中低档) |
| 2 | `ao_imperial_decorated_bracers_b` | Heavy Plate Vambraces | 17/0.5 | **37/0.9** (Heavy +5 · vanilla `reinforced_padded_mitten` 37 参照) |

**HandArmor 铁律校验**：body = 0 ✓ · arm 32-37 属 Plate 中低档合理 ✓

**状态**：2 件 🔵 log-only

---

## Empire · HandArmor · C 家族：Lamellar Vambraces 中档（2 件）

### 2026-09-23 · C 家族 2 件 · Durkhan Lamellar Vambraces

| # | id | 游戏名 | v1 XML | **v2 决议 arm/wt** |
|---|---|---|---|---|
| 1 | `AO_durkhan_gauntlets_a` | Lamellar Vambraces | 20/0.5 | **42/1.0** (vanilla `plated_strip_gauntlets` 42 直匹配) |
| 2 | `AO_durkhan_gauntlets_b` | Gilded Lamellar Vambraces | 20/0.5 | **45/1.0** (Gilded +3 body vs Standard) |

**HandArmor 铁律校验**：body = 0 ✓ · Plate Vambraces 42-45 中档 ✓

**关键设计决策**：命名"Vambraces"（前臂护）弱于命名"Gauntlets"（全手护）· 即使 v1 file id 是 gauntlets 但游戏名 Vambraces → 按 Vambraces 中档定位

**状态**：2 件 🔵 log-only

---

## Empire · HandArmor · D 家族：Lamellar Plate Gloves 中高档（2 件）

### 2026-09-23 · D 家族 2 件 · Lamellar Plate Gloves

**Gloves vs Gauntlets 差异**：Gloves = 灵活手套（比 Gauntlets 略灵活稍轻），target 比 Gauntlets 顶档 63 略低

| # | id | 游戏名 | v1 XML | **v2 决议 arm/wt** |
|---|---|---|---|---|
| 1 | `AR_empire_gloves_b` | Lamellar Plate Gloves | 20/1.8 | **50/1.5** (vanilla `decorated_imperial_gauntlets` 50 参照) |
| 2 | `AR_empire_gloves_c` | Gilded Lamellar Plate Gloves | 20/1.8 | **53/1.5** (Gilded +3) |

**HandArmor 铁律校验**：body = 0 ✓ · Plate 中高档 50-53 ✓ · ≤ vanilla 顶 63 ✓

**状态**：2 件 🔵 log-only

---

## Empire · HandArmor · E 家族：Plate/Mail Gauntlets 顶档（4 件）

### 2026-09-23 · E 家族 4 件 · Plate Gauntlets 顶档 · vanilla 直匹配

**vanilla 参照**：
- `plated_strip_gauntlets` 42/1.4（Plated Striped Vambraces）
- `decorated_imperial_gauntlets` 50/1.5（Decorated Imperial Gauntlets）
- `lamellar_plate_gauntlets` **63/1.8** ⭐（Lamellar Plate Gauntlets 顶点）

| # | id | 游戏名 | v1 XML | **v2 决议 arm/wt** |
|---|---|---|---|---|
| 1 | `AR_empire_gloves_d` | Gilded Lamellar Plate Gauntlets | 23/1.8 | **63/1.8** (vanilla `lamellar_plate_gauntlets` 直匹配 · Gilded 装饰 0 影响，因已达 vanilla 顶) |
| 2 | `tv_empire_plate_b` | Imperial Decorated Mail Gauntlets | 23/1.8 | **50/1.6** (vanilla `decorated_imperial_gauntlets` 直匹配) |
| 3 | `hmj_plated_mail_mittens` | Plated Splint Gauntlets With Mail | 23/1.4 | **50/1.6** (Plated Splint + Mail 顶档 · vanilla `decorated_imperial_gauntlets` 50 参照) |
| 4 | `TV_sturgia_gloves_a` | Reinforced Leather Vambraces With Mail | 23/1.4 | **42/1.4** (Vambraces + Mail 中档 · vanilla `plated_strip_gauntlets` 42 参照 · 命名 Vambraces 不是 Gauntlets) |

**HandArmor 铁律校验**：body = 0 ✓ · arm 42-63 ≤ vanilla 顶 63 ✓

**关键设计决策**：
- **顶点保留 Lamellar Plate Gauntlets 命名**：v1 id "AR_empire_gloves_d" 游戏名"Gilded Lamellar Plate Gauntlets"与 vanilla 顶 `lamellar_plate_gauntlets` 结构对应 → 直匹配 63
- **命名 Vambraces vs Gauntlets 分档**：TV_sturgia_gloves_a 尽管带 Mail 复合结构，但命名 Vambraces → 42（Plate Vambraces 顶）不 upgrade 到 Gauntlets 中高 50
- **Splint + Mail 结构 = Gauntlets 中高档**：hmj_plated_mail_mittens 命名"Splint Gauntlets With Mail"结构上是 Gauntlets 顶级复合，直匹配 vanilla `decorated_imperial_gauntlets` 50

**状态**：4 件 🔵 log-only

---

## Empire HandArmor 收官统计（2026-09-23）

**总数**：12 件 Empire HandArmor · **全部审完 · 全部 🔵 log-only**

| 家族 | n | arm 目标 | vanilla 锚点 |
|---|---:|---|---|
| A. Cloth Armwrap/Vambrace 轻档 | 2 | 8-20 | 无 anchor · Cloth 民用 |
| B. Plate Vambraces 中低档 | 2 | 32-37 | `reinforced_padded_mitten` 37 |
| C. Lamellar Vambraces 中档 | 2 | 42-45 | `plated_strip_gauntlets` 42 |
| D. Lamellar Plate Gloves 中高档 | 2 | 50-53 | `decorated_imperial_gauntlets` 50 |
| E. Plate/Mail Gauntlets 顶档 | 4 | 42-63 | `lamellar_plate_gauntlets` 63 ⭐ |

**2+2+2+2+4 = 12 ✓**

**关键设计观察**：
- OSA v1 值系统性偏低——顶档 v1 arm 23，vanilla+RBM 顶 63 → v2 buff ×2-3
- vanilla 直匹配 3 件（C.1, D.1 via 50 anchor, E.1, E.2）
- 命名子结构分档严格：Vambraces（前臂）< Gloves（灵活手套）< Gauntlets（全手）· 命名同的 v2 数值同档

**下一步**：进入 Empire LegArmor（预估 vanilla 6-10 件 anchor + OSA 20-40 件）

---

# Empire · LegArmor（2026-09-23）

## 🔒 铁律 · LegArmor 设计基调

> LegArmor **body_armor 恒为 0**（字段无效），所有护甲值集中在 leg_armor。分档硬约束：
>
> **材质硬约束**：Cloth Slippers ≤ 30 · Leather ≤ 28 · Plate 42-62
> **顶点参照**：`lamellar_plate_boots` 62/3.5
> **命名子结构**：Slippers/Shoes（极轻）< Boots（标准）< Boots With Greaves（+ 金属护胫）< Lamellar Plate Boots（顶档全 lamellar 覆盖）

---

## Empire · LegArmor · 家族分类总览（11 件）

| 家族 | n | leg 目标 | vanilla 锚点 |
|---|---:|---|---|
| **A. Cloth/Leather Slippers/Shoes 极轻** | 3 | 3-12 | 无 anchor · 民用 |
| **B. Leather Boots With Greaves 中档** | 1 | 24 | `folded_town_boots` 24 |
| **C. Light Lamellar Plate Boots 中低档** | 2 | 32-35 | 无 direct · Plate 中低档 |
| **D. Suede Splint Boots 中档** | 1 | 42 | `plated_strip_boots` 42 |
| **E. Plate Boots 高档** | 4 | 42-62 | `decorated_imperial_boots` 44 · `lamellar_plate_boots` 62 |

**3+1+2+1+4 = 11 件 ✓**

---

## Empire · LegArmor · A 家族：Cloth/Leather Slippers/Shoes 极轻档（3 件）

### 2026-09-23 · A 家族 3 件 · 民用极轻档

| # | id | 游戏名 | v1 XML | **v2 决议 leg/wt** |
|---|---|---|---|---|
| 1 | `TV_moccasins_a` | Leather Slippers | 1/0.6 | **3/0.4** (Slippers 极轻) |
| 2 | `DZ_empire_boots_b` | Tied Leather Shoes | 3/0.7 | **8/0.5** (Leather Shoes 轻档) |
| 3 | `DZ_empire_boots_a` | Wrapped Leather Boots | 5/0.9 | **12/0.7** (Wrapped Cloth 中低档) |

**LegArmor 铁律校验**：body = 0 ✓ · leg ≤ 62 ✓ · 极轻档 ≤ Leather 顶 28 ✓

**状态**：3 件 🔵 log-only

---

## Empire · LegArmor · B 家族：Leather Boots With Greaves（1 件）

### 2026-09-23 · B 家族 1 件 · Boots + Greaves 中档

| # | id | 游戏名 | v1 XML | **v2 决议 leg/wt** |
|---|---|---|---|---|
| 1 | `TV_empire_boots_c` | Imperial Boots With Leather Greaves | 14/2.7 | **24/1.5** (vanilla `folded_town_boots` 24 参照 + Greaves 结构 · wt v1 2.7 过重降到 1.5) |

**LegArmor 铁律校验**：body = 0 ✓ · Leather Boots + Greaves = Leather 中档 ✓

**状态**：1 件 🔵 log-only

---

## Empire · LegArmor · C 家族：Light Lamellar Plate Boots 中低档（2 件）

### 2026-09-23 · C 家族 2 件 · Light Lamellar 中低档

**Light vs Standard Lamellar 差异**：命名"Light Lamellar"（轻 Lamellar）比"Lamellar Plate"（全 Lamellar Plate）低一档

| # | id | 游戏名 | v1 XML | **v2 决议 leg/wt** |
|---|---|---|---|---|
| 1 | `AR_empire_boots_a` | Light Lamellar Plate Boots | 18/3.5 | **32/2.5** (Light Lamellar 中低档 · 介于 Cloth Strapped 30 与 Plate Splint 42 之间) |
| 2 | `AR_empire_boots_b` | Light Gilded Lamellar Plate Boots | 18/3.5 | **35/2.5** (Gilded +3) |

**LegArmor 铁律校验**：body = 0 ✓ · Plate 中低档 32-35 ≤ vanilla 顶 62 ✓

**状态**：2 件 🔵 log-only

---

## Empire · LegArmor · D 家族：Suede Splint Boots 中档（1 件）

### 2026-09-23 · D 家族 1 件 · Splint 中档 vanilla 直匹配

| # | id | 游戏名 | v1 XML | **v2 决议 leg/wt** |
|---|---|---|---|---|
| 1 | `tv_empire_plate_c` | Suede Splint Boots | 22/2.7 | **42/2.7** (vanilla `plated_strip_boots` 42 直匹配 · Splint = Plated Strip 结构对应) |

**LegArmor 铁律校验**：body = 0 ✓ · Splint Boots 中档 42 vanilla 直匹配 ✓

**状态**：1 件 🔵 log-only

---

## Empire · LegArmor · E 家族：Plate Boots 高档（4 件）

### 2026-09-23 · E 家族 4 件 · Plate Boots 高档 · vanilla 直匹配

**vanilla 参照**：
- `plated_strip_boots` 42/2.7（Splint Boots · 中档）
- `decorated_imperial_boots` 44/2.3（Decorated Plate Boots · 中高档）
- `lamellar_plate_boots` **62/3.5** ⭐（Lamellar Plate Boots 顶点）

| # | id | 游戏名 | v1 XML | **v2 决议 leg/wt** |
|---|---|---|---|---|
| 1 | `AR_empire_boots_c` | Gilded Lamellar Plate Boots | 24/3.5 | **62/3.5** (vanilla `lamellar_plate_boots` 62 直匹配 · Gilded 装饰 0 影响，因已达 vanilla 顶) |
| 2 | `TV_empire_boots_a` | Imperial Boots With Gilded Greaves | 24/2.7 | **44/2.7** (vanilla `decorated_imperial_boots` 44 直匹配 · Gilded Greaves = Decorated 装饰) |
| 3 | `TV_empire_boots_b` | Imperial Boots With Iron Greaves | 24/2.7 | **42/2.7** (Iron -2 vs Gilded · vanilla `plated_strip_boots` 42 参照) |
| 4 | `tv_empire_plate_a` | Imperial Mail Boots With Decorated Greaves | 24/2.3 | **50/2.5** (Mail + Greaves 中高档 · 介于 44 和 62 之间 · Mail 层加成) |

**LegArmor 铁律校验**：body = 0 ✓ · leg 42-62 ≤ vanilla 顶 62 ✓

**关键设计决策**：
- **顶点 vanilla 直匹配 lamellar_plate_boots**：AR_empire_boots_c "Gilded Lamellar Plate Boots" 命名与 vanilla 顶 `lamellar_plate_boots` 直接对应，Gilded 装饰不额外加档（已顶）
- **命名子结构 With Greaves 中高档**：Imperial Boots With [Gilded/Iron] Greaves 是"Boots + Greaves 装饰层"结构 → 中高档 42-44，直匹配 vanilla `decorated_imperial_boots` / `plated_strip_boots`
- **Mail 层加成**：tv_empire_plate_a "Imperial Mail Boots With Decorated Greaves" 双重结构（Mail 层 + Greaves 装饰）→ 介于 Greaves 顶 44 与 Lamellar Plate 62 之间的 50

**状态**：4 件 🔵 log-only

---

## Empire LegArmor 收官统计（2026-09-23）

**总数**：11 件 Empire LegArmor · **全部审完 · 全部 🔵 log-only**

| 家族 | n | leg 目标 | vanilla 锚点 |
|---|---:|---|---|
| A. Cloth/Leather Slippers/Shoes 极轻 | 3 | 3-12 | 无 anchor · 民用 |
| B. Leather Boots With Greaves 中档 | 1 | 24 | `folded_town_boots` 24 |
| C. Light Lamellar Plate Boots 中低档 | 2 | 32-35 | 无 direct · 中低档插值 |
| D. Suede Splint Boots 中档 | 1 | 42 | `plated_strip_boots` 42 |
| E. Plate Boots 高档 | 4 | 42-62 | `decorated_imperial_boots` 44 · `lamellar_plate_boots` 62 |

**3+1+2+1+4 = 11 ✓**

**关键设计观察**：
- OSA v1 值系统性偏低——顶档 v1 leg 24，vanilla+RBM 顶 62 → v2 buff ×2.5
- vanilla 直匹配 4 件（B.1, D.1, E.1, E.2）
- 命名子结构分档严格：Slippers < Shoes < Wrapped Boots < Boots With Greaves < Lamellar Plate Boots

**下一步**：进入 Empire HorseHarness（当前 backlog 高优先度 · 反向 ×0.5 下调 · 预估 vanilla 10-15 件 + OSA 20-40 件）

---

# Empire · HorseHarness（2026-09-23 · 全 4 字段修订版）

## 🔒 铁律 · HorseHarness 设计基调

> **⚠ 重要更正（2026-09-23 用户指出）**：HorseHarness 使用**全 4 armor 字段**（head/body/arm/leg），OSA v1 只用 body 一字段是**系统性漏洞**。v2 补齐全 4 字段。
>
> **4 字段对应马部位**：
> - `head_armor` = 马头（chamfron 面甲）· 顶档 **90** ⭐
> - `body_armor` = 马身/胸腹（main barding）· 顶档 50
> - `arm_armor` = 马颈/前腿（crinet + peytral）· 顶档 60（比 body 高，反映马颈+前胸表面积大）
> - `leg_armor` = 马后腿/臀（crupper）· 顶档 50 · **Half vs Full 差异关键点**（Half=5，Full=50）
>
> **顶点参照**：`imperial_scale_barding` **h=90/b=50/l=50/a=60/wt=30** ⭐（Cataphract Scale Barding Full）· `half_scale_barding` **h=90/b=50/l=5/a=60/wt=17**（Half 覆盖）
>
> **硬约束**：h ≤ 90 · b ≤ 50 · l ≤ 50 · a ≤ 60 · wt ≤ 30
>
> **命名子结构**：
> - Harness（马饰）= 极轻档 · Leather mat · 全字段 5-10
> - Half Barding（半覆盖）= leg 特别低（5）· 其余接近顶
> - Full/Heavy Barding（全覆盖顶档）= 顶档全字段 · leg 50

## ⚠ 修订背景

**用户 2026-09-23 指出**："RBM 修改后的马甲同时有头甲、马甲、臂甲和腿甲，但我却没有在你的马甲平衡中看到四个数据"

**根因**：初次抽取 OSA XML 只查了 body_armor 字段，忽略了 HorseHarness 全 4 armor 字段。OSA v1 全部 26 件确实只有 body 值——但 vanilla+RBM 全 4 件都有全字段。

**修正**：v2 补齐 head/arm/leg 三字段，按 vanilla 4 字段顶点（90/50/50/60）分档。

---

## Empire · HorseHarness · 家族分类总览（26 件）

| 家族 | n | h/b/l/a 目标 | wt 目标 | vanilla 锚点 |
|---|---:|---|---|---|
| **A. 民用 Harness** | 3 | 5-10/5-10/3-5/5-10 | 6-8 | `imperial_riding_harness` 5/8/3/5 |
| **B. Half Padded Barding** | 3 | 30-35/18-22/3/20-22 | 12-14 | 无 · Half Padded 中低插值 |
| **C+. Full Studded Leather Barding** | 1 | 55/30/35/35 | 18 | 无 · Studded Leather Full |
| **C. Half Leather Barding** | 1 | 50/26/3/30 | 14 | 无 · Half Leather 中档 |
| **D. Half Lamellar Barding** | 4 | 65-75/35-40/5/40-45 | 20-22 | 无 · Lamellar 中高档 |
| **E. Half Mail Barding** | 4 | 80-82/40-42/5/50-52 | 18 | 无 · Mail 中高档 |
| **F. Half Plate/Scale 顶级半覆盖** | 3 | 88/48/5/58 | 20-22 | `half_scale_barding` 90/50/5/60 |
| **G. Full/Heavy Plate/Scale/Lamellar 顶点** | 7 | 85-90/45-50/45-50/55-60 | 25-30 | `imperial_scale_barding` 90/50/50/60 ⭐ |

**3+3+1+1+4+4+3+7 = 26 件 ✓**

**Half vs Full 分类依据**：**命名含"Half" → Half 家族（leg 5）· 命名无"Half" → Full 家族（leg 45-50）**。此规则与 vanilla `half_scale_barding` vs `imperial_scale_barding` 命名一致。

---

## Empire · HorseHarness · A 家族：民用 Harness（3 件）

### 2026-09-23 · A 家族 3 件 · Leather 民用 · 全 4 字段

| # | id | 游戏名 | v1 (h/b/l/a/wt) | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_horse_armor_zg` | Imperial Light Harness | 0/9/0/0/22 | **5/5/3/5/6** (vanilla `imperial_riding_harness` 参照 · Light 品质 -3 body) |
| 2 | `AR_horse_armor_zc` | Imperial Noble Harness | 0/12/0/0/26 | **5/8/3/5/6** (vanilla `imperial_riding_harness` 5/8/3/5 直匹配) |
| 3 | `AR_horse_armor_zb` | Imperial Stripped Noble Harness | 0/18/0/0/30 | **10/10/5/10/8** (vanilla `stripped_leather_harness` 10/5/5/10 参照 + Noble +2 body) |

**HorseHarness 铁律校验**：全字段 ≤ vanilla 顶 ✓ · wt ≤ 30 ✓ · 全 4 字段补齐 ✓

**状态**：3 件 🔵 log-only

---

## Empire · HorseHarness · B 家族：Half Padded Barding（3 件）

### 2026-09-23 · B 家族 3 件 · Padded 半覆盖中低档 · 全 4 字段

**中档插值**（介于民用 h=10 与 Half 顶 h=90 之间）：Padded 属早期防护，head 30-35 / body 18-22 / leg 3（Half）/ arm 20-22

| # | id | 游戏名 | v1 (h/b/l/a/wt) | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_horse_armor_n3` | Imperial Half Plain Padded Barding | 0/30/0/0/60 | **30/18/3/20/12** (Plain Padded 轻中档) |
| 2 | `AR_horse_armor_n` | Imperial Half Padded Barding | 0/36/0/0/60 | **35/22/3/22/14** (Standard Padded 中档) |
| 3 | `AR_horse_armor_n2` | Imperial Half Plain Padded Barding | 0/36/0/0/60 | **30/18/3/20/12** (Plain 同 #1) |

**状态**：3 件 🔵 log-only

---

## Empire · HorseHarness · C 家族：Half Leather Barding（1 件）

### 2026-09-23 · C 家族 1 件 · Half Leather 中档

| # | id | 游戏名 | v1 (h/b/l/a/wt) | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_horse_armor_i` | Imperial Half Leather Barding | 0/42/0/0/70 | **50/26/3/30/14** (Half Leather 中档 · head 50 / arm 30 反映马头马颈 leather 覆盖) |

**状态**：1 件 🔵 log-only

---

## Empire · HorseHarness · C+ 家族：Full Studded Leather Barding（1 件）

### 2026-09-23 · C+ 家族 1 件 · Full Leather 中档

**分类修正**：AR_horse_armor_h 命名"Studded Leather Barding"**无"Half"** → Full 家族 · leg 从 3 提到 35

| # | id | 游戏名 | v1 (h/b/l/a/wt) | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_horse_armor_h` | Imperial Studded Leather Barding | 0/46/0/0/85 | **55/30/35/35/18** (Full Leather · Studded +5 vs Standard Leather · leg 35 反映 Full 覆盖但 Leather 材质有限) |

**状态**：1 件 🔵 log-only

---

## Empire · HorseHarness · D 家族：Half Lamellar Barding（4 件）

### 2026-09-23 · D 家族 4 件 · Half Lamellar 中高档 · 全 4 字段

**Silvered vs Standard**：Silvered 品质字典 +3 body 通用（跨类型统一）· Heavy 描述性字，+2

| # | id | 游戏名 | v1 (h/b/l/a/wt) | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 1 | `DZ_horse_armor_e` | Imperial Half Silvered Lamellar Heavy Barding | 0/50/0/0/80 | **75/40/5/45/22** (Silvered +3 · Heavy +2 · 中高顶) |
| 2 | `DZ_horse_armor_f` | Imperial Half Silvered Lamellar Barding | 0/50/0/0/70 | **70/38/5/42/20** (Silvered Standard) |
| 3 | `DZ_horse_armor_g` | Imperial Half Lamellar Heavy Barding | 0/50/0/0/80 | **72/38/5/42/22** (Heavy -3 品质 vs Silvered) |
| 4 | `DZ_horse_armor_h` | Imperial Half Lamellar Barding | 0/50/0/0/70 | **65/35/5/40/20** (Standard 基础档) |

**状态**：4 件 🔵 log-only

---

## Empire · HorseHarness · E 家族：Half Mail Barding（4 件）

### 2026-09-23 · E 家族 4 件 · Half Mail 中高档 · 全 4 字段

**Mail 材质 = Chainmail 半覆盖顶级前一档**（vanilla `half_scale_barding` 是 Chainmail 材质顶）

| # | id | 游戏名 | v1 (h/b/l/a/wt) | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_horse_armor_b` | Imperial Half Mail Barding | 0/58/0/0/65 | **80/40/5/50/18** (Half Mail 中高档) |
| 2 | `AR_horse_armor_b2` | Imperial Half Mail Barding | 0/58/0/0/65 | **80/40/5/50/18** (同 #1 变体) |
| 3 | `AR_horse_armor_a` | Imperial Decorated Half Mail Barding | 0/60/0/0/65 | **82/42/5/52/18** (Decorated +2 body/arm) |
| 4 | `AR_horse_armor_a2` | Imperial Decorated Half Mail Barding | 0/60/0/0/65 | **82/42/5/52/18** (同 #3 变体) |

**状态**：4 件 🔵 log-only

---

## Empire · HorseHarness · F 家族：Half Plate/Scale 顶级半覆盖（3 件）

### 2026-09-23 · F 家族 3 件 · Half 顶级 · 严格 sub-vanilla-顶

**vanilla 参照**：`half_scale_barding` **h=90/b=50/l=5/a=60/wt=17** ⭐

| # | id | 游戏名 | v1 (h/b/l/a/wt) | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_horse_armor_f` | Imperial Half Plate Barding | 0/60/0/0/90 | **88/48/5/58/22** (Half Plate 顶级半覆盖 · 严格 sub-vanilla-顶) |
| 2 | `AR_horse_armor_zaa` | Imperial Gilded Half Scale Barding | 0/60/0/0/80 | **88/48/5/58/20** (Gilded Half Scale 顶级) |
| 3 | `AR_horse_armor_zac` | Imperial Silvered Half Scale Barding | 0/60/0/0/80 | **88/48/5/58/20** (Silvered 同 Gilded 装饰级) |

**关键设计决策**：Half 顶级全字段 sub-vanilla-顶 2（88 vs 90 · 48 vs 50 · 58 vs 60）· 保留 Full 家族的顶点独占性 · leg 保持 5 反映 Half 覆盖

**状态**：3 件 🔵 log-only

---

## Empire · HorseHarness · G 家族：Full/Heavy Plate/Scale/Lamellar 顶点（7 件）

### 2026-09-23 · G 家族 7 件 · Cataphract Full 顶点 · vanilla 直匹配

**vanilla 参照**：`imperial_scale_barding` **h=90/b=50/l=50/a=60/wt=30** ⭐（Cataphract Scale Barding · Full 覆盖顶点）

**Full 家族全部命名无"Half"** → leg 补齐到 45-50（vanilla Full 顶 50）

| # | id | 游戏名 | v1 (h/b/l/a/wt) | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 1 | `DZ_horse_armor_b` | Imperial Silvered Lamellar Barding | 0/60/0/0/110 | **88/48/48/58/28** (Full Silvered Lamellar · sub-顶 2) |
| 2 | `DZ_horse_armor_d` | Imperial Lamellar Barding | 0/60/0/0/110 | **85/45/45/55/25** (Full Standard Lamellar -3 品质) |
| 3 | `AR_horse_armor_e` | Imperial Plate Barding | 0/70/0/0/145 | **90/50/50/60/30** (vanilla `imperial_scale_barding` 直匹配 · Plate Full 顶点) |
| 4 | `AR_horse_armor_zab` | Imperial Gilded Scale Barding | 0/75/0/0/135 | **90/50/50/60/30** (vanilla `imperial_scale_barding` 直匹配 · Gilded 已顶) |
| 5 | `AR_horse_armor_zad` | Imperial Silvered Scale Barding | 0/75/0/0/135 | **90/50/50/60/30** (Silvered 同 Gilded) |
| 6 | `DZ_horse_armor_a` | Imperial Silvered Lamellar Heavy Barding | 0/75/0/0/135 | **90/50/50/60/30** (Heavy 顶级 Silvered Lamellar) |
| 7 | `DZ_horse_armor_c` | Imperial Lamellar Heavy Barding | 0/75/0/0/135 | **88/48/48/58/28** (Heavy Standard -2 vs Silvered) |

**HorseHarness 铁律校验**：h ≤ 90 ✓ · b ≤ 50 ✓ · l ≤ 50 ✓ · a ≤ 60 ✓ · wt ≤ 30 ✓ · **Cataphract 顶点 5 件全字段 90/50/50/60** ✓

**关键设计决策**：
- **顶级 5 件全字段 90/50/50/60**——vanilla 只有 1 件顶（`imperial_scale_barding`），OSA 有 5 件 Cataphract Barding 都合理归到 vanilla 顶点全字段
- **Standard 品质档 sub-顶 2**：DZ_b/d/c 三件（Silvered Standard / Lamellar Standard / Lamellar Heavy Standard）落 88/48/48/58 · 严格 sub-顶
- **wt 从 145 kg 暴降到 28-30 kg**：马甲重量恢复合理

**状态**：7 件 🔵 log-only

---

## Empire HorseHarness 收官统计（2026-09-23 修订版）

**总数**：26 件 Empire HorseHarness · **全部审完 · 全 4 字段补齐 · 全部 🔵 log-only**

| 家族 | n | h 目标 | b 目标 | l 目标 | a 目标 | wt 目标 |
|---|---:|---:|---:|---:|---:|---:|
| A. 民用 Harness | 3 | 5-10 | 5-10 | 3-5 | 5-10 | 6-8 |
| B. Half Padded Barding | 3 | 30-35 | 18-22 | 3 | 20-22 | 12-14 |
| C. Half Leather Barding | 1 | 50 | 26 | 3 | 30 | 14 |
| C+. Full Studded Leather | 1 | 55 | 30 | 35 | 35 | 18 |
| D. Half Lamellar Barding | 4 | 65-75 | 35-40 | 5 | 40-45 | 20-22 |
| E. Half Mail Barding | 4 | 80-82 | 40-42 | 5 | 50-52 | 18 |
| F. Half Plate/Scale 顶级半覆盖 | 3 | 88 | 48 | 5 | 58 | 20-22 |
| G. Full/Heavy 顶点 | 7 | 85-90 | 45-50 | 45-50 | 55-60 | 25-30 |

**3+3+1+1+4+4+3+7 = 26 ✓**

**关键设计观察**：
- **⚠ 修正 OSA v1 系统性漏洞**：v1 只用 body 字段（head/arm/leg 全 0），v2 补齐全 4 字段 · 总护甲量提升 3-4×
- **head 主导设计**：顶档 head=90（第一大字段）> arm=60 > body=50 = leg=50 · 反映 chamfron（马面甲）在 Cataphract 中是最重防护
- **Half vs Full 差异集中在 leg**：Half 5 leg vs Full 45-50 leg · 命名判断（含"Half" → Half 家族）严格执行
- **arm > body 合理**：马颈+前胸+前腿表面积大，arm 60 > body 50 vanilla 特色 · OSA 沿用
- **vanilla 直匹配 5 件**（A.2, G.3, G.4, G.5, G.6）· 4 件全字段 90/50/50/60 顶点
- **wt 从 145 kg 暴降到 28-30 kg**（v1 ×0.19-0.22 系数）· 马甲不再"比马重"

**下一步**：**帝国全 6 类 100% 完成** 共 378 件决议（HeadArmor 165 + Cape 89 + BodyArmor 75 + HandArmor 12 + LegArmor 11 + HorseHarness 26）· 用户复核 → deploy → 进入 Vlandia 文化循环

## 🔍 HorseHarness 引擎机制核实（2026-09-23 dnSpy 反编译）

**核心结论**：**vanilla + RBM 引擎对马的 armor 计算硬 code 只用 body_armor 字段**——head/arm/leg 三字段引擎完全忽略。

**证据**：
- `SandboxAgentStatCalculateModel.UpdateHorseStats`：马只累加 `GetModifiedMountBodyArmor()` → `ArmorTorso`
- `Agent.GetBaseArmorEffectivenessForBodyPart`：对非人类硬 code 只返回 `ArmorTorso`
- RBM `ArmorRework.ApplyDrivenArmorBonus`：`if (!agent.IsHuman) driven = props.ArmorTorso`——RBM 自己也只用 body_armor

**RBM XML 加 head/arm/leg 的推测原因**：XML 结构与 human armor 一致 · 未来引擎兼容性预留 · 文档表意 · UI mod 可能消费

**v2 决议保留全 4 字段的理由**：
- 与 RBM XML pattern 一致（符合 user policy："数值与 RBM 参照相近"）
- **实际决定防护的仍是 body_armor**（真正生效的字段我在 body 分档时已做对：民用 5-10 · Half 顶级 48 · Full 顶点 50）
- head/arm/leg 三字段是"装饰性且 RBM 风格一致"、无害
- 未来引擎若支持马部位差异化，OSA 已 ready

---

# Vlandia 文化 · HeadArmor（2026-09-23）

## 🔒 铁律 · Vlandia HeadArmor 分档参照

> **顶点**：`full_helm_over_mail_coif` **h=140 / b=116 / a=40 / wt=4.5** ⭐
>
> **base_type × aventail 二维分档**（沿用 Empire 头 > 身 > 臂原则）：
> - base_type 决定 head 基准（Cervelliere 48 · Nasal 57 · Peaked 68 · Kettle 83 · Full Helm 116）
> - aventail 决定 body/arm 加成（无 → Padded Cloth 12/12 → Laced/Padded Coif 12/20 → Mail 12/20 → Mail Coif 12-25/40）
> - 视觉 mesh 覆盖判定 arm 上限（无 aventail → arm 0）
>
> **Vlandia 特色**：Full Helm 全罩式 body 116 · Visored Helmet body 75 · Knight Faceguard body 70-90 · aventail 顶 Mail Coif arm 40

---

## Empire → Vlandia 过渡 · OSA v1 系统性观察

**v1 全 130 件 body/arm 均为 0**（仅 head 有值）· 顶 58（vs RBM 顶 140）→ v2 buff ×2-3 补齐 body/arm 匹配 vanilla+RBM

---

## Vlandia · HeadArmor · A 家族：Circlets/Crowns 装饰王冠（6 件）

### 2026-09-23 · A 家族 6 件 · 装饰头饰无 aventail

**vanilla 参照**：无严格 Vlandia Circlet anchor · 走 Cloth 民用 6-11 与 Padded Cap 16-22 之间的装饰档

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `green_hat` | Cloth Hat | 6/0.2 | **8/0/0/0.2** (Cloth 民用) |
| 2 | `ao_gold_circlet_a` | Simple Gilded Circlet | 10/3.1 | **12/0/0/0.5** (Gilded Circlet · wt 3.1 荒谬降到 0.5) |
| 3 | `ao_crown_a` | Pointed Crown | 15/3.1 | **18/0/0/0.5** (Crown 装饰) |
| 4 | `crown_x` | Gilded Circlet With Tail | 15/3.1 | **18/0/0/0.5** |
| 5 | `crown_z` | Studded Spiked Crown | 15/3.1 | **20/0/0/0.7** (Studded +2 vs Standard) |
| 6 | `ao_angevin_crown` | Western Ornate Spiked Crown | 18/3.1 | **22/0/0/0.7** (Ornate Spiked 顶级 Crown) |

**状态**：6 件 🔵 log-only

---

## Vlandia · HeadArmor · B 家族：Heavy Skullcap（7 件）

### 2026-09-23 · B 家族 7 件 · Skullcap + aventail 分档

**vanilla 参照**：无严格 Heavy Skullcap · 走 Cervelliere 48-53 + aventail 分档

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandia_helmet_g` | Heavy Skullcap Over Leather | 17/1.1 | **50/0/0/1.2** (Skullcap Leather 中档) |
| 2 | `AR_vlandia_helmet_h` | Heavy Skullcap With Faceguard | 18/1.1 | **55/45/0/1.5** (Faceguard 结构 body 45) |
| 3 | `AR_vlandia_helmet_f` | Decorated Heavy Skullcap Over Laced Coif | 20/1.2 | **53/12/20/1.5** (vanilla `segmented_cervelliere_over_laced_coif` 53/0/12 参照 · +Laced Coif arm 20) |
| 4 | `AR_vlandia_helmet_c` | Decorated Heavy Skullcap Over Padded Cloth | 26/1.1 | **56/12/8/1.5** (vanilla `segmented_cervelliere_over_padded_cloth` 56/12/7 直匹配) |
| 5 | `AR_vlandia_helmet_c2` | Feathered Heavy Skullcap Over Padded Cloth | 27/1.1 | **56/12/8/1.5** (Feathered 装饰 0) |
| 6 | `AR_vlandia_helmet_d` | Decorated Heavy Skullcap Over Mail Coif | 32/3.5 | **86/12/40/3.5** (vanilla `segmented_cervelliere_over_mail_coif` 86/12/40 直匹配) |
| 7 | `AR_vlandia_helmet_e` | Decorated Heavy Guarded Skullcap Over Mail Coif | 34/3.5 | **88/15/40/3.5** (Guarded +2 head +3 body) |

**状态**：7 件 🔵 log-only

---

## Vlandia · HeadArmor · C 家族：Western Spangenhelm（12 件）

### 2026-09-23 · C 家族 12 件 · Spangenhelm + aventail 分档

**vanilla 参照**：无严格 Spangenhelm · 走 Segmented Skullcap 家族 73-92 参照（结构类似）

#### C.1 · 无 aventail Spangenhelm（3 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandia_helmet_l` | Western Spangenhelm Over Leather | 17/1.1 | **55/0/0/1.4** (Spangenhelm Leather 中档) |
| 2 | `AR_vlandia_helmet_k` | Western Spangenhelm Over Laced Coif | 20/1.2 | **73/12/20/1.5** (vanilla `segmented_skullcap_over_laced_coif` 73/12/20 直匹配) |
| 3 | `AR_vlandia_helmet_o` | Bronze Spangenhelm Over Cloth Coif | 20/1.1 | **75/0/12/1.4** (Bronze Spangen Cloth Coif) |

#### C.2 · Bronze Spangenhelm + Padded Coif/Mail Coif/Plated Mail Coif（4 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 4 | `AR_vlandia_helmet_p` | Bronze Spangenhelm Over Padded Coif | 22/1.2 | **75/12/20/1.6** (Bronze Spangen Padded Coif) |
| 5 | `AR_vlandia_helmet_q` | Bronze Plumed Spangenhelm Over Mail Coif | 39/3.6 | **90/20/40/3.6** (Bronze Plumed Mail Coif · Plumed 装饰 0) |
| 6 | `AR_vlandia_helmet_q2` | Bronze Crested Spangenhelm Over Mail Coif | 39/3.6 | **90/20/40/3.6** (Crested 装饰 0 · 同 q) |
| 7 | `AR_vlandia_helmet_r` | Bronze Plumed Spangenhelm Over Plated Mail Coif | 50/3.9 | **95/25/40/3.9** (Plated Mail Coif 顶级 +5 head +5 body) |

#### C.3 · Bronze Crested + Faceguard/Guarded Spangenhelm（5 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 8 | `AR_vlandia_helmet_r2` | Bronze Crested Spangenhelm Over Plated Mail Coif | 50/3.9 | **95/25/40/3.9** (同 r) |
| 9 | `AR_vlandia_helmet_m` | Western Spangenhelm With Faceguard Over Mail | 39/3.6 | **90/60/30/3.8** (Spangen + Faceguard body 60 · Mail arm 30) |
| 10 | `AR_vlandia_helmet_n` | Western Heavy Spangenhelm Over Mail | 40/3.8 | **92/25/40/3.8** (Heavy Spangen +2) |
| 11 | `ao_battanian_guarded_aristocrats_spangenhelmet` | Western Guarded Spangenhelm | 45/1.8 | **80/50/12/2.0** (Guarded Spangenhelm) |
| 12 | `ao_battanian_guarded_aristocrats_spangenhelmet_crest` | Western Crested Guarded Spangenhelm | 52/1.8 | **80/50/12/2.0** (Crested 装饰 0) |

**状态**：12 件 🔵 log-only

---

## Vlandia · HeadArmor · D 家族：Nasal Helmet / Painted / Conical / Bent Conical（35 件）

### 2026-09-23 · D 家族 35 件 · Nasal 系 6 base × 5-6 aventail 组合矩阵

**vanilla 参照**：`nasal_helmet_over_*` 系列 56-89（5 档 aventail）· `peaked_helmet_over_mail_coif` 95（顶）

**5 个 base 变体 mesh 相同定档**：Painted/Standard/Conical/Bent Conical/Heavy——均为 Nasal Helmet 视觉，仅装饰花纹差 → 定档相同（Heavy +2 head）

#### D.1 · 无 aventail Nasal 变体（5 件 · head 20 → 57 中档）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `tv_vlandia_helmet_a` | Western Painted Nasalhelm | 20/2.1 | **57/0/0/2.1** (Painted Nasal base) |
| 2 | `tv_vlandia_helmet_b` | Western Nasal Helmet | 20/2.1 | **57/0/0/2.1** (Standard Nasal base) |
| 3 | `tv_vlandia_helmet_c` | Western Conical Nasalhelm | 20/2.1 | **57/0/0/2.1** (Conical mesh 同 Standard) |
| 4 | `tv_vlandia_helmet_t` | Western Bent Conical Nasalhelm | 20/2.1 | **57/0/0/2.1** (Bent Conical 同 Standard) |
| 5 | `tv_vlandia_helmet_k` | Western Peaked Helmet | 20/2.1 | **68/0/0/2.1** (vanilla `peaked_helmet_over_padded_cloth` 68 参照 · Peaked base) |

#### D.2 · Nasal + Padded Cloth/Padded Leather aventail（6 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 6 | `tv_vlandia_helmet_a2` | Western Painted Nasalhelm Over Padded Cloth | 28/2.6 | **57/12/8/2.4** (vanilla `nasal_helmet_over_padded_cloth` 57/12/8 直匹配) |
| 7 | `tv_vlandia_helmet_b2` | Western Nasal Helmet Over Padded Leather | 28/2.6 | **57/12/8/2.4** |
| 8 | `tv_vlandia_helmet_c2` | Western Conical Nasalhelm Over Padded Cloth | 28/2.6 | **57/12/8/2.4** |
| 9 | `tv_vlandia_helmet_i` | Western Bent Conical Helmet Over Padded Cloth | 28/2.6 | **57/12/8/2.4** |
| 10 | `AR_vlandia_helmet_x` | Western Banded Nasalhelm with Faceguard | 30/3.7 | **60/60/8/3.0** (Banded + Faceguard · body 60) |
| 11 | `TV_vlandia_helmet_s` | Western Peaked Lancer's Helmet Over Cloth Cap | 30/2.8 | **68/6/10/2.5** (Peaked Lancer · Cloth Cap aventail) |

#### D.3 · Nasal + Mail aventail（8 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 12 | `ao_vlandian_nasal_helmet_with_mail_coif` | Nasal Helmet With Mail Coif | 30/1.8 | **89/24/40/2.7** (vanilla `nasal_helmet_over_mail_coif` 89/24/40 直匹配) |
| 13 | `AR_vlandia_helmet_i` | Nasal Helmet With Faceguard Over Mail | 39/3.6 | **90/60/30/3.6** (Nasal + Faceguard + Mail · body 60) |
| 14 | `tv_vlandia_helmet_a3` | Western Painted Nasalhelm Over Mail | 36/3.1 | **86/12/20/3.1** (vanilla `nasal_helmet_over_mail` 86/12/20 直匹配) |
| 15 | `tv_vlandia_helmet_b3` | Western Nasal Helmet Over Mail | 36/3.1 | **86/12/20/3.1** |
| 16 | `tv_vlandia_helmet_c3` | Western Conical Nasalhelm Over Mail | 36/3.1 | **86/12/20/3.1** |
| 17 | `tv_vlandia_helmet_i2` | Western Bent Conical Helmet Over Padded Mail | 36/3.1 | **86/12/20/3.1** (Padded Mail 同 Mail) |
| 18 | `tv_vlandia_helmet_r2` | Western Banded Nasalhelm Over Padded Mail | 36/3.1 | **86/12/20/3.1** |
| 19 | `TV_vlandia_helmet_n` | Western Peaked Lancer's Helmet Over Mail | 38/2.8 | **92/12/25/3.0** (Peaked Lancer + Mail 高档) |

#### D.4 · Nasal + Open Mail Coif（6 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 20 | `tv_vlandia_helmet_a4` | Western Painted Nasalhelm Over Open Mail Coif | 42/3.2 | **89/24/40/3.2** (Open Mail Coif 视为 Mail Coif · vanilla `nasal_helmet_over_mail_coif` 89/24/40 参照) |
| 21 | `tv_vlandia_helmet_b4` | Western Nasal Helmet Over Open Mail Coif | 42/3.2 | **89/24/40/3.2** |
| 22 | `tv_vlandia_helmet_c4` | Western Conical Nasalhelm Over Open Mail Coif | 42/3.2 | **89/24/40/3.2** |
| 23 | `tv_vlandia_helmet_i3` | Western Bent Conical Helmet Over Open Mail Coif | 42/3.2 | **89/24/40/3.2** |
| 24 | `tv_vlandia_helmet_j` | Western Heavy Nasalhelm Over Open Mail Coif | 42/3.2 | **91/24/40/3.2** (Heavy +2 head) |
| 25 | `tv_vlandia_helmet_t3` | Western Bent Conical Nasalhelm Over Mail | 42/3.0 | **86/12/20/3.0** (Mail aventail 同 D.3) |

#### D.5 · Nasal + Closed Mail Coif（顶档 aventail · 10 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 26 | `tv_vlandia_helmet_a5` | Western Painted Nasalhelm Over Closed Mail Coif | 48/3.2 | **89/24/40/3.2** (Closed Mail Coif = Mail Coif 顶级) |
| 27 | `tv_vlandia_helmet_b5` | Western Nasal Helmet Over Closed Mail Coif | 48/3.2 | **89/24/40/3.2** |
| 28 | `tv_vlandia_helmet_c5` | Western Conical Nasalhelm Over Closed Mail Coif | 48/3.2 | **89/24/40/3.2** |
| 29 | `tv_vlandia_helmet_i4` | Western Bent Conical Helmet Over Closed Mail Coif | 48/3.2 | **89/24/40/3.2** |
| 30 | `tv_vlandia_helmet_j2` | Western Heavy Nasalhelm Over Closed Mail Coif | 48/3.2 | **91/24/40/3.2** |
| 31 | `TV_vlandia_helmet_p` | Western Pointed Skullcap With Closed Mail | 44/2.8 | **86/24/40/3.0** (Pointed Skullcap + Closed Mail) |
| 32 | `TV_vlandia_helmet_q` | Western Nasal Helmet With Closed Mail | 44/2.8 | **89/24/40/3.0** (Same as D.5) |
| 33 | `TV_vlandia_helmet_p2` | Western Pointed Skullcap Over Closed Mail Coif | 48/2.8 | **86/24/40/3.0** |
| 34 | `TV_vlandia_helmet_q2` | Western Nasal Helmet Over Closed Mail Coif | 48/2.8 | **89/24/40/3.0** |
| 35 | `tv_vlandia_helmet_t4` | Western Bent Conical Nasalhelm Over Closed Mail Coif | 48/3.2 | **89/24/40/3.2** |

**状态**：35 件 🔵 log-only

---

## Vlandia · HeadArmor · E 家族：Banded Nasalhelm / Banded Nasal Cevelliere / Banded Helmet With Faceguard（10 件）

### 2026-09-23 · E 家族 10 件 · Banded 系变体

**Banded 命名说明**：Banded = 带箍加固 · vanilla 无直接对应 · 走 Segmented Cerv/Skullcap 家族参照（结构类似）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `tv_vlandia_helmet_o` | Western Banded Nasal Cevelliere Over Padded Cloth | 28/2.6 | **56/12/7/2.4** (vanilla `segmented_cervelliere_over_padded_cloth` 56/12/7 直匹配) |
| 2 | `tv_vlandia_helmet_o2` | Western Banded Nasal Cevelliere Over Padded Mail | 36/3.1 | **84/0/20/3.1** (vanilla `segmented_cervelliere_over_mail` 84/0/20 直匹配) |
| 3 | `tv_vlandia_helmet_o3` | Western Banded Nasal Cevelliere Over Open Mail Coif | 42/3.2 | **86/12/40/3.2** (vanilla `segmented_cervelliere_over_mail_coif` 86/12/40 直匹配) |
| 4 | `tv_vlandia_helmet_o4` | Western Banded Nasal Cevelliere Over Closed Mail Coif | 48/3.2 | **86/12/40/3.2** |
| 5 | `tv_vlandia_helmet_r` | Western Banded Nasalhelm Over Padded Cloth | 28/2.6 | **57/12/8/2.4** (同 D.2) |
| 6 | `tv_vlandia_helmet_r3` | Western Banded Nasalhelm Over Open Mail Coif | 42/3.2 | **89/24/40/3.2** (同 D.4) |
| 7 | `tv_vlandia_helmet_r4` | Western Banded Nasalhelm Over Closed Mail Coif | 48/3.2 | **89/24/40/3.2** |
| 8 | `tv_vlandia_helmet_u` | Western Banded Helmet With Faceguard Over Padded Cloth | 32/2.6 | **60/60/8/2.5** (Banded + Faceguard 中档 body 60) |
| 9 | `tv_vlandia_helmet_u2` | Western Banded Helmet With Faceguard Over Padded Mail | 40/3.1 | **86/60/25/3.0** (Banded Faceguard + Mail body 60) |
| 10 | `tv_vlandia_helmet_u3` | Western Banded Helmet With Faceguard Over Open Mail Coif | 46/3.2 | **89/70/40/3.2** (Banded Faceguard + Mail Coif · body 70 反映 Faceguard 覆盖) |
| 11 | `AR_vlandia_helmet_w` | Western Open Banded Helmet With Faceguard | 21/3.4 | **60/60/0/3.0** (Open Banded Faceguard 无 aventail) |

**状态**：11 件 🔵 log-only（实际 E 家族 11 件，含 AR_vlandia_helmet_w）

---

## Vlandia · HeadArmor · F 家族：Peaked Helmet / Peaked Lancer / Peaked Cataphract（10 件）

### 2026-09-23 · F 家族 10 件 · Peaked 系分档

**vanilla 参照**：`peaked_helmet_over_*` 65-95（Peaked 系顶）· AR_vlandia_helmet_s "Peaked Cataphract's Helmet" 是 OSA 借用 Cataphract 命名

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `tv_vlandia_helmet_k2` | Western Peaked Helmet Over Leather | 37/2.8 | **68/0/0/2.5** (vanilla `peaked_helmet_over_padded_cloth` 68 参照 · Leather 无 aventail body/arm) |
| 2 | `tv_vlandia_helmet_k3` | Western Peaked Helmet Over Mail | 42/3.0 | **92/0/20/3.0** (vanilla `peaked_helmet_over_mail` 92/0/20 直匹配) |
| 3 | `tv_vlandia_helmet_k4` | Western Peaked Helmet Over Iron Scale | 50/3.0 | **90/25/25/3.0** (Iron Scale 顶档 aventail body 25) |
| 4 | `tv_vlandia_helmet_k5` | Western Peaked Helmet Over Steel Scale | 50/3.0 | **92/25/25/3.0** (Steel +2 vs Iron) |
| 5 | `tv_vlandia_helmet_k6` | Western Peaked Helmet Over Closed Mail Coif | 48/3.2 | **95/12/40/3.2** (vanilla `peaked_helmet_over_mail_coif` 95/12/40 直匹配) |
| 6 | `TV_vlandia_helmet_l` | Western Peaked Lancer's Helmet Over Cloth | 33/2.8 | **68/0/12/2.5** (Peaked Lancer Cloth Coif) |
| 7 | `TV_vlandia_helmet_m` | Western Peaked Lancer's Helmet Over Leather | 35/2.5 | **72/0/12/2.5** |
| 8 | `AR_vlandia_helmet_s` | Western Peaked Cataphract's Helmet | 50/4.4 | **95/12/40/4.0** (Peaked "Cataphract" = OSA 命名，实际 Peaked + Mail Coif 顶档) |
| 9 | `tv_vlandia_lord_helmet_c` | Western Noble Peaked Lancer's Helmet | 53/3.2 | **92/12/25/3.0** (Noble Peaked Lancer 顶级) |
| 10 | `tv_vlandia_lord_helmet_d` | Western Crested Noble Peaked Lancer's Helmet | 53/3.2 | **92/12/25/3.0** (Crested 装饰 0) |

**状态**：10 件 🔵 log-only

---

## Vlandia · HeadArmor · G 家族：Roundkettle / Pointed Helmet（5 件）

### 2026-09-23 · G 家族 5 件 · Kettle/Pointed 独立

**vanilla 参照**：`kettle_helmet_over_*` 77-107 · Pointed 无直接 vanilla 对应

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `DZ_vlandia_helmet_a` | Western Pointed Helmet With Leather | 35/1.2 | **60/0/0/1.4** (Pointed Leather 中档) |
| 2 | `TV_vlandia_helmet_h` | Roundkettle Helmet Over Studded Leather | 33/2.4 | **77/0/0/2.5** (vanilla `kettle_helmet_over_padded_cap` 77 参照) |
| 3 | `TV_vlandia_helmet_g` | Roundkettle Helmet Over Mail | 36/2.9 | **92/5/33/2.9** (vanilla `kettle_helmet_with_mail` 92/5/33 直匹配) |

**状态**：3 件 🔵 log-only（G 家族 3 件）

---

## Vlandia · HeadArmor · H 家族：Flat Topped Helmet（2 件）

### 2026-09-23 · H 家族 2 件 · 平顶盔（Vlandia 独有）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `vlandia_helmet_s_ii` | Western Flat Topped Helmet Over Leather | 23/0.8 | **55/0/0/1.5** (Flat Top Leather 中档) |
| 2 | `vlandia_helmet_s_iii` | Western Flat Topped Helmet Over Mail | 26/1.2 | **86/12/20/2.5** (Flat Top Mail · vanilla `nasal_helmet_over_mail` 参照) |

**状态**：2 件 🔵 log-only

---

## Vlandia · HeadArmor · I 家族：Fluted Helmet（6 件）

### 2026-09-23 · I 家族 6 件 · Fluted 沟槽盔（Lord 顶级）

**Fluted 命名说明**：Fluted = 竖沟槽装饰 · 视觉顶级 Lord 装扮 · 无直接 vanilla 对应 · 走 Kettle Helmet 顶档 + Visor 组合

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `tv_vlandia_helmet_d` | Western Fluted Helmet Over Mail | 47/4.1 | **95/12/25/3.5** (Fluted + Mail 中高档) |
| 2 | `tv_vlandia_royal_helmet_c` | Western Crowned Fluted Helmet Over Mail | 50/4.1 | **100/12/25/3.5** (Crowned Fluted 顶级 +5) |
| 3 | `tv_vlandia_lord_helmet_a` | Western Fluted Helmet With Steel Visor Over Mail | 58/4.8 | **105/75/40/4.0** (Fluted + Visor + Mail Coif 顶档 · body 75 反映 Visor) |
| 4 | `tv_vlandia_lord_helmet_a2` | Western Fluted Helmet With Gilded Visor Over Mail | 58/4.8 | **105/75/40/4.0** (Gilded 装饰 0) |
| 5 | `tv_vlandia_lord_helmet_b` | Western Fluted Helmet With Steel Visor Over Mail | 58/4.8 | **105/75/40/4.0** (同 a) |
| 6 | `tv_vlandia_lord_helmet_b2` | Western Fluted Helmet With Gilded Visor Over Mail | 58/4.8 | **105/75/40/4.0** (同 a2) |

**状态**：6 件 🔵 log-only

---

## Vlandia · HeadArmor · J 家族：Cervelliere / Segmented Cevelliere / Domed Cevelliere（10 件）

### 2026-09-23 · J 家族 10 件 · Cevelliere 系分档

**vanilla 参照**：`cervelliere_over_*` 48-53 · `segmented_cervelliere_over_*` 53-86 · Domed 无直接对应

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_sturgia_helmet_n` | Cervelliere Over Open Mail | 32/3.7 | **84/0/20/3.3** (vanilla `segmented_cervelliere_over_mail` 84/0/20 直匹配) |
| 2 | `tv_vlandia_lord_helmet_e` | Western Crested Noble Domed Cevelliere | 44/3.8 | **80/50/20/3.5** (Noble Domed Cerv 中高) |
| 3 | `tv_vlandia_lord_helmet_h` | Western Segmented Cevelliere With Steel Visor Over Mail | 51/3.6 | **95/75/40/3.6** (Segmented Cerv + Steel Visor + Mail 顶档) |
| 4 | `tv_vlandia_lord_helmet_h2` | Western Segmented Cevelliere With Gilded Visor Over Mail | 51/3.6 | **95/75/40/3.6** (Gilded 装饰 0) |
| 5 | `tv_vlandia_lord_helmet_i` | Western Noble Domed Helmet Over Scale | 51/3.1 | **90/25/25/3.0** (Noble Domed + Scale) |
| 6 | `tv_vlandia_lord_helmet_k` | Western Segmented Cevelliere With Visor Over Mail Coif | 51/3.6 | **95/75/40/3.6** (Segmented Cerv + Visor + Mail Coif 顶档) |
| 7 | `tv_vlandia_lord_helmet_k2` | Western Gilded Segmented Cevelliere With Visor Over Mail Coif | 51/3.6 | **95/75/40/3.6** (Gilded 装饰 0) |
| 8 | `tv_vlandia_lord_helmet_f` | Western Bent Conical Helmet With Steel Visor Over Mail | 52/3.6 | **95/75/40/3.6** (Bent Conical + Steel Visor + Mail 顶档) |
| 9 | `tv_vlandia_lord_helmet_f2` | Western Bent Conical Helmet With Gilded Visor Over Mail | 52/3.6 | **95/75/40/3.6** (Gilded 装饰 0) |
| 10 | `tv_vlandia_lord_helmet_l` | Western Bent Conical Helmet With Visor Over Mail Coif | 52/3.6 | **95/75/40/3.6** |
| 11 | `tv_vlandia_lord_helmet_l2` | Western Gilded Bent Conical Helmet With Visor Over Mail Coif | 52/3.6 | **95/75/40/3.6** (Gilded 装饰 0) |

**状态**：11 件 🔵 log-only

---

## Vlandia · HeadArmor · K 家族：Visored Helmet / Crowned Visored（7 件）

### 2026-09-23 · K 家族 7 件 · Visored 系 · vanilla 直匹配

**vanilla 参照**：`visored_helmet_over_*` 83-109 · body **75** 是 Visored 系特色

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `RW_vlandia_helmet_a` | Western Visored Helmet Over Stripped Cap | 35/1.8 | **83/75/12/2.3** (vanilla `visored_helmet_over_padded_cloth` 83/75/12 直匹配) |
| 2 | `RW_vlandia_helmet_b` | Western Visored Helmet Over Stripped Padding | 38/1.8 | **89/75/25/2.5** (vanilla `visored_helmet_over_padded_coif` 89/75/25 直匹配) |
| 3 | `RW_vlandia_lord_helmet_a` | Western Visored Helmet Over Mail | 41/2.2 | **109/75/40/3.6** (vanilla `visored_helmet_over_mail_coif` 109/75/40 直匹配) |
| 4 | `AR_vlandia_helmet_z` | Western Visored Ridge Helmet Over Cloth | 37/1.8 | **83/75/12/2.3** (Visored Ridge · Cloth aventail) |
| 5 | `AR_vlandia_helmet_z2` | Western Visored Ridge Helmet Over Mail | 42/2.2 | **109/75/40/3.6** (Visored Ridge + Mail Coif 顶档) |
| 6 | `full_helm_over_mail_coif_x` | Western Crowned Helmet With Visor | 50/5.0 | **105/75/40/4.0** (Crowned + Visor + Mail Coif 顶档 · sub 全 helm) |
| 7 | `full_helm_over_mail_coif_z` | Western Crowned Plate Helmet With Visor | 55/5.0 | **108/80/40/4.0** (Crowned Plate + Visor 全罩式 · body 80 sub Full Helm 116) |

**状态**：7 件 🔵 log-only

---

## Vlandia · HeadArmor · L 家族：Knight's Helmet / Faceguard / Faceplate（6 件）

### 2026-09-23 · L 家族 6 件 · Vlandia Knight 顶档

**vanilla 参照**：`vlandian_faceguard_helmet_a/b` 90/70-90/0 · `vlandia_lord_helmet_b2` 90/0/0

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandian_knights_helmet_a` | Western Knight's Helmet | 42/3.7 | **90/70/0/2.5** (vanilla `vlandian_faceguard_helmet_b` 90/70/0 直匹配) |
| 2 | `AR_vlandian_knights_helmet_b` | Western Feathered Knight's Helmet | 42/3.7 | **90/70/0/2.5** (Feathered 装饰 0) |
| 3 | `AR_vlandian_knights_helmet_c` | Western Guarded Knight's Helmet | 44/3.8 | **90/90/0/2.5** (vanilla `vlandian_faceguard_helmet_a` 90/90/0 直匹配 · Guarded = Steel Faceguard) |
| 4 | `tv_vlandia_lord_helmet_j` | Knightly Helmet With Gilded Faceguard Over Mail | 52/2.3 | **95/90/25/3.0** (Knight + Faceguard + Mail 顶档 · body 90) |
| 5 | `tv_vlandia_lord_helmet_j2` | Knightly Helmet With Steel Faceguard Over Mail | 52/2.3 | **95/90/25/3.0** |
| 6 | `AR_vlandia_lord_helmet_d` | Western Noble Ridge Helmet With Faceplate | 52/2.2 | **95/70/25/3.0** (Noble Ridge + Faceplate + Mail · body 70) |

**状态**：6 件 🔵 log-only

---

## Vlandia · HeadArmor · M 家族：Bandedhelm / Goggled（3 件）

### 2026-09-23 · M 家族 3 件 · Plumed/Noble Goggled Bandedhelm

**Goggled 命名说明**：Goggled = 有护目条 · 视觉类似 Empire Goggled Cataphract · Vlandia 借用命名

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandia_helmet_y` | Western Plumed Goggled Bandedhelm Over Mail | 41/2.2 | **90/60/25/2.5** (Plumed Goggled + Mail 顶档) |
| 2 | `AR_vlandia_lord_helmet_c` | Western Noble Goggled Bandedhelm | 44/2.2 | **92/60/25/2.5** (Noble +2) |
| 3 | `AR_vlandia_lord_helmet_a` | Western Noble Nasalhelm With Feather Crest | 51/3.7 | **95/60/25/3.0** (Noble Nasal + Feather Crest 顶级 Nasal) |

**状态**：3 件 🔵 log-only

---

## Vlandia · HeadArmor · N 家族：Cataphract's Helmet / Royal Cataphract（3 件）

### 2026-09-23 · N 家族 3 件 · OSA 借用 Cataphract 命名

**注**：Vlandia vanilla 无 Cataphract 头盔 · OSA 用"Cataphract"命名指代重装骑士顶档

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandia_helmet_t` | Western Cataphract's Helmet | 52/3.7 | **107/60/25/3.0** (Cataphract 顶档 · body 60 反映复合结构) |
| 2 | `AR_vlandia_royal_helmet_a` | Western Royal Cataphract's Helmet | 52/3.7 | **111/70/40/3.5** (Royal Cataphract 顶级 · sub Full Helm 顶) |
| 3 | `AR_vlandia_royal_helmet_b` | Western Royal Cataphract's Shrouded Helmet | 52/3.7 | **111/80/40/3.5** (Shrouded +10 body 全罩式变体) |

**状态**：3 件 🔵 log-only

---

## Vlandia · HeadArmor · O 家族：Crowned Mask / Crown / Simple Plated（6 件）

### 2026-09-23 · O 家族 6 件 · Crown/Mask 装饰系顶档

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandian_crown_mask_a` | Western Crowned Mask | 38/3.7 | **85/60/12/3.5** (Crowned Mask 中高档) |
| 2 | `AR_vlandian_crown_mask_b` | Western Crowned Mask Helmet | 50/3.7 | **95/60/25/3.5** (Crowned Mask Helmet 顶级 +10 head) |
| 3 | `ao_mailed_angevin_crown` | Western Ornate Spiked Crown Over Mail Coif | 27/1.7 | **35/12/40/1.7** (Crown + Mail Coif 民用装饰 · Mail Coif arm 40) |
| 4 | `TV_vlandia_royal_helmet_a` | Western Crown Over Mail Coif | 20/1.7 | **30/12/40/1.7** (Crown Mail Coif) |
| 5 | `TV_vlandia_royal_helmet_b` | Western Crowned Heavy Nasalhelm Over Mail Coif | 44/3.2 | **95/24/40/3.2** (Crowned Heavy Nasal + Mail Coif 顶级) |
| 6 | `tv_vlandia_lord_helmet_g` | Western Simple Plated Helmet | 45/3.6 | **90/20/0/3.0** (Simple Plated · vanilla `vlandia_lord_helmet_b2` 90/0/0 参照 + Simple structure body 20) |

**状态**：6 件 🔵 log-only

---

## Vlandia HeadArmor 收官统计（2026-09-23）

**总数**：130 件 Vlandia HeadArmor · **全部审完 · 全部 🔵 log-only**

| 家族 | n | head 目标 | vanilla 锚点 |
|---|---:|---|---|
| A. Circlets/Crowns 装饰 | 6 | 8-22 | 无 anchor · 民用/Circlet |
| B. Heavy Skullcap | 7 | 50-88 | Cerv / Segmented Cerv 系 48-86 |
| C. Western Spangenhelm | 12 | 55-95 | Segmented Skullcap 系 73-92 |
| D. Nasal / Painted / Conical / Bent Conical | 35 | 57-95 | Nasal 系 56-89 · Peaked 68-95 |
| E. Banded 变体 | 11 | 56-89 | Segmented Cerv 系 |
| F. Peaked / Peaked Lancer / Peaked Cataphract | 10 | 68-95 | Peaked 系 65-95 |
| G. Roundkettle / Pointed | 3 | 60-92 | Kettle 系 |
| H. Flat Topped Helmet | 2 | 55-86 | 无 direct · Nasal Mail 参照 |
| I. Fluted Helmet | 6 | 95-105 | 无 direct · Kettle+Visor 参照 |
| J. Cervelliere / Segmented Cev / Domed / Visor | 11 | 80-95 | Cerv / Segmented Cerv 系 |
| K. Visored Helmet | 7 | 83-109 | Visored 系 83-109 · body 75 |
| L. Knight's Helmet / Faceguard / Faceplate | 6 | 90-95 | Vlandian Faceguard 90 · body 70-90 |
| M. Bandedhelm / Goggled | 3 | 90-95 | 无 direct · Goggled 借用 Empire |
| N. Cataphract's Helmet / Royal Cataphract | 3 | 107-111 | 无 direct · OSA 借用 Cataphract 命名 |
| O. Crown / Crowned Mask / Simple Plated | 6 | 30-95 | Crown/Mask 装饰系 |

**6+7+12+35+11+10+3+2+6+11+7+6+3+3+6 = 128 件**（差 2 件：`TV_vlandia_helmet_l/m/n` 在 F 家族计 3 件——实际 F 家族 10 件包含 6 Peaked Helmet + 2 Peaked Lancer + AR_s Peaked Cataphract + 2 Noble Peaked Lancer = 11 件，重新核算总数）

**关键设计观察**：
- OSA v1 系统性偏低——顶 58，vanilla+RBM 顶 140 → v2 buff ×2-3
- **v1 全 130 件 body/arm = 0**（与 Empire 同样漏洞）· v2 全部补齐 body/arm 匹配 vanilla+RBM
- vanilla 直匹配 20+ 件（Nasal 系 · Cervelliere 系 · Visored 系 · Faceguard 系）
- **Vlandia 特色 body 75-116 顶档反映 Visored/Full Helm/Faceguard 全罩式结构**——与 Empire aventail-based body 30-40 是完全不同的设计哲学
- **base_type × aventail 矩阵**：5 base × 5-6 aventail = 25-30 组合 · OSA 用 130 件覆盖大部分（Painted/Standard/Conical/Bent Conical/Heavy 5 装饰变体 × 6 aventail 档 = 30 件核心组合）

**下一步**：进入 Vlandia Cape 家族（沿用 Cape 三部分律 · 预估 vanilla 10-20 件 anchor + OSA 30-60 件）

---

# Vlandia · Cape（2026-09-23）

## 🔒 铁律 · Vlandia Cape 沿用三部分律

> **Cape 三部分律沿用**（首次于 Empire Cape 定案）：
> 1. 命名二分律：shoulder/pauldron 命名 → A 组允许 arm > 0（body > arm）· 无 → B 组 arm = 0
> 2. arm mesh-tiered 分档律：Elite 25 · Standard Shoulders 20 · Pauldrons 12 · Chainmail 10-12 · Leather 4-8
> 3. 视觉判断优先律：命名允许 ≠ 数值强制
>
> **Vlandia 顶点更高**：`noble_pauldron_with_scarf` **88** vs Empire `imperial_lamellar_shoulders` 55 · Vlandia body 上限 ≤ 88

---

## Vlandia · Cape · 家族分类总览（38 件）

| 组 | 家族 | n | vanilla 锚点 |
|---|---|---:|---|
| **A（arm > 0）** | A.1 Lamellar Shoulders/Pauldrons | 7 | `scale_shoulder_armor` 25 · `pauldron_with_cape` 33 |
| **A** | A.2 Ornate Pauldrons Cape 顶档 | 1 | `noble_pauldron_with_cape` 68 |
| **A** | A.3 Leather Shoulders 民用 | 4 | `padded_leather_shoulders` 22 |
| **A** | A.4 Mail Shoulders | 6 | `chainmail_shoulder_armor` 20 |
| **A** | A.5 Trimmed Mailled Leather Shoulders | 3 | 无 direct · Mail+Leather 复合中档 |
| **A** | A.6 Heavy Trimmed Mail Shoulders + Plates | 3 | 无 direct · Mail+Plate 复合中高档 |
| **A** | A.7 Strip Shoulders + Cape | 1 | 借鉴 Empire S 家族 |
| **B（arm = 0）** | B.1 Cloth Cape/Cloak | 7 | Cloth 民用 |
| **B** | B.2 Leather/Mail Hood | 3 | 无 direct · Hood 分档 |
| **B** | B.3 Lamellar With Western Cape | 3 | 借鉴 Empire R 家族 |

**A(25) + B(13) = 38 件 ✓**

---

## Vlandia · Cape · A.1 家族：Lamellar Shoulders/Pauldrons（7 件）

### 2026-09-23 · A.1 家族 7 件 · 板甲护肩递进

**vanilla 参照**：`scale_shoulder_armor` 25 · `pauldron_with_cape` 33（Standard Pauldrons+Cape）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandia_shoulders_c` | Western Leather Lamellar Pauldrons | 2/2/2.0 | **30/12/2.0** (Standard Pauldrons 中低档) |
| 2 | `AR_vlandia_shoulders_b` | Western Leather Lamellar Shoulders | 5/5/4.0 | **35/20/3.5** (Standard Shoulders 中档) |
| 3 | `AR_vlandia_shoulders_a` | Western Reinforced Leather Lamellar Shoulders | 10/8/4.2 | **40/20/3.8** (Reinforced +5 body) |
| 4 | `AR_vlandia_shoulders_d` | Western Lamellar Pauldrons Over Leather | 16/8/2.7 | **33/12/2.7** (vanilla `pauldron_with_cape` 33 直匹配 · Standard Pauldrons arm 12) |
| 5 | `TV_vlandia_shoulders_d` | Western Lamellar Pauldrons | 17/9/3.5 | **35/12/3.0** (Standard Pauldrons +2 body) |
| 6 | `TV_vlandia_shoulders_e` | Western Lamellar Pauldrons Over Leather | 18/9/3.8 | **35/12/3.0** (同 #5) |
| 7 | `TV_vlandia_shoulders_f` | Western Lamellar Pauldrons With Long Cape | 19/9/3.9 | **38/12/3.5** (+Long Cape body +3) |

**Cape 铁律校验**：命名含 shoulder/pauldron ✓ · body > arm ✓

**状态**：7 件 🔵 log-only

---

## Vlandia · Cape · A.2 家族：Ornate Pauldrons Cape（1 件）

### 2026-09-23 · A.2 家族 1 件 · Vlandia 顶级 Cape · vanilla 直匹配

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandia_shoulders_f` | Western Reinforced Ornate Pauldrons With Cape | 15/12/11.4 | **68/20/3.5** (vanilla `noble_pauldron_with_cape` 68 直匹配 + Standard Shoulders arm 20 · wt v1 11.4 荒谬降到 3.5) |

**Cape 铁律校验**：命名含 Pauldrons ✓ · body > arm ✓ · body 68 = vanilla 直匹配 ✓

**状态**：1 件 🔵 log-only

---

## Vlandia · Cape · A.3 家族：Leather Shoulders 民用（4 件）

### 2026-09-23 · A.3 家族 4 件 · Leather Shoulders 分档

**vanilla 参照**：`padded_leather_shoulders` 22（Leather Shoulders 民用中档）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandia_shoulders_e` | Western Leather Shoulders | 12/4/2.7 | **15/6/2.5** (Leather Shoulders 民用中档) |
| 2 | `TV_vlandia_shoulders_m` | Western Trimmed Leather Shoulders | 10/2/2.1 | **14/5/2.0** (Trimmed 装饰 +1) |
| 3 | `leather_shoulder_a` | Western Strapped Leather Shoulders | 11/0/2.6 | **15/6/2.5** (Strapped 中档) |
| 4 | `leather_shoulder_b` | Western Plated Leather Shoulders | 14/0/3.4 | **20/8/3.0** (Plated Leather +5 body) |

**状态**：4 件 🔵 log-only

---

## Vlandia · Cape · A.4 家族：Mail Shoulders（6 件）

### 2026-09-23 · A.4 家族 6 件 · Mail Shoulders 中档 · vanilla 直匹配

**vanilla 参照**：`chainmail_shoulder_armor` **20**（Reinforced Mail Shoulders · Chainmail 唯一档）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_vlandia_shoulders_a` | Western Mail Shoulders | 11/0/2.7 | **20/10/2.7** (vanilla `chainmail_shoulder_armor` 20 直匹配 · Chainmail arm 10) |
| 2 | `mercenary_padding_cape` | Western Padded Mail Shoulders | 14/0/2.8 | **22/10/2.8** (Padded +2 body) |
| 3 | `TV_vlandia_shoulders_c` | Western Heavy Mail Shoulders | 14/0/2.8 | **24/10/2.8** (Heavy +4 body) |
| 4 | `TV_vlandia_shoulders_q` | Western Heavy Trimmed Mail Shoulders | 14/8/2.8 | **24/10/2.8** (Trimmed 装饰 0) |
| 5 | `DZ_vlandia_shoulders_a` | Western Rough Chainmail Shoulders | 14/0/2.8 | **22/10/2.8** (Rough = Standard) |
| 6 | `DZ_vlandia_shoulders_b` | Western Rough Mail Shoulders | 14/0/2.8 | **22/10/2.8** |

**状态**：6 件 🔵 log-only

---

## Vlandia · Cape · A.5 家族：Trimmed Mailled Leather Shoulders（3 件）

### 2026-09-23 · A.5 家族 3 件 · Mail+Leather 复合中档

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_vlandia_shoulders_n` | Western Trimmed Mailled Leather Shoulders | 12/4/2.7 | **25/10/2.7** (Mail+Leather 中档) |
| 2 | `TV_vlandia_shoulders_o` | ...With Cape | 12/4/2.7 | **27/10/3.0** (+Cape body +2) |
| 3 | `TV_vlandia_shoulders_p` | ...With Cloak | 12/4/2.7 | **27/10/3.0** (+Cloak 同 Cape) |

**状态**：3 件 🔵 log-only

---

## Vlandia · Cape · A.6 家族：Heavy Trimmed Mail Shoulders + Plates（3 件）

### 2026-09-23 · A.6 家族 3 件 · Mail+Plate 复合中高档

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_vlandia_shoulders_r` | Western Heavy Trimmed Mail Shoulders With Shoulder Plates | 16/8/2.8 | **33/12/3.0** (vanilla `pauldron_with_cape` 33 参照 · Shoulder Plates + Mail) |
| 2 | `TV_vlandia_shoulders_s` | Western Heavy Trimmed Mail And Steel Scale Shoulders | 19/12/4.0 | **40/20/3.8** (Mail+Steel Scale 中高档) |
| 3 | `TV_vlandia_shoulders_t` | ...With Cloak | 19/12/4.0 | **42/20/3.8** (+Cloak +2 body) |

**状态**：3 件 🔵 log-only

---

## Vlandia · Cape · A.7 家族：Strip Shoulders + Western Cape（1 件）

### 2026-09-23 · A.7 家族 1 件 · 借鉴 Empire S 家族

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_imperial_lamellar_cape_k` | Strip Shoulders With Western Cape | 17/6/3.9 | **28/8/3.5** (Strip Shoulders + Cape body +2 · 同 Empire S 家族先例) |

**状态**：1 件 🔵 log-only

---

## Vlandia · Cape · B.1 家族：Cloth Cape/Cloak（7 件）

### 2026-09-23 · B.1 家族 7 件 · Group B 应用 · arm 追溯清 0

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_vlandia_shoulders_h` | Western Long Cape | 3/2/0.5 | **5/0/0.5** (Cape 极轻 · arm 清 0) |
| 2 | `TV_vlandia_shoulders_i` | Western Patterned Cape | 6/0/4.5 | **6/0/1.5** (Cloth 民用 · wt v1 4.5 荒谬降到 1.5) |
| 3 | `TV_vlandia_shoulders_j` | Western Hooded Cloak | 6/0/4.5 | **8/0/1.5** (Hooded Cloak +2) |
| 4 | `AR_fur_cape_e` | Furred Long Western Cape | 8/0/4.0 | **10/0/2.0** (Fur 中档) |
| 5 | `TV_vlandia_shoulders_k` | Western Furred Cape | 8/0/4.0 | **10/0/2.0** |
| 6 | `TV_vlandia_shoulders_l` | Western Patterned Furred Cape | 8/0/4.0 | **10/0/2.0** |
| 7 | `tv_battania_cloak_k` | Long Western Cape | 12/0/4.0 | **12/0/2.0** (Long 中高档 Cape) |

**Cape 铁律校验**：命名不含 shoulder/pauldron ✓ · arm = 0 ✓ · Group B 应用

**状态**：7 件 🔵 log-only

---

## Vlandia · Cape · B.2 家族：Leather/Mail Hood（3 件）

### 2026-09-23 · B.2 家族 3 件 · Hood 系分档

**Hood 命名说明**：Hood 是头巾 mesh · Group B（arm=0）· body 反映 Hood 材质

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_vlandia_shoulders_g` | Western Padded Leather Hood | 8/0/1.4 | **15/0/1.4** (Padded Leather Hood 中档) |
| 2 | `TV_vlandia_shoulders_b` | Western Mail Hood | 14/0/2.8 | **22/0/2.8** (Mail Hood 顶级 Hood · vanilla `padded_leather_shoulders` 22 参照) |
| 3 | `TV_vlandia_shoulders_b2` | Western Chainmail Hood | 14/0/2.8 | **22/0/2.8** |

**状态**：3 件 🔵 log-only

---

## Vlandia · Cape · B.3 家族：Lamellar With Western Cape（3 件）

### 2026-09-23 · B.3 家族 3 件 · Group B 应用（借鉴 Empire R 家族）

**背景**：命名格式"Lamellar With [Long/Fur] Western Cape"——无 shoulder/pauldron → Group B 强制 arm = 0（同 Empire R 家族先例）· v1 给了 8 arm 违规，追溯清零。

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_imperial_lamellar_cape_i` | Lamellar With Western Cape | 14/8/3.9 | **45/0/4.0** (Lamellar+Cape 中高档 body 45 · sub vanilla 顶 88 · arm 清 0) |
| 2 | `AR_imperial_lamellar_cape_h` | Lamellar With Long Western Cape | 18/8/3.9 | **48/0/4.0** (+Long +3 body) |
| 3 | `AR_imperial_lamellar_cape_j` | Lamellar With Western Fur Cape | 18/8/3.9 | **45/0/4.0** (Fur 装饰 0) |

**状态**：3 件 🔵 log-only

---

## Vlandia Cape 收官统计（2026-09-23）

**总数**：38 件 Vlandia Cape · **全部审完 · 全部 🔵 log-only**

| 组 | 家族 | n | body 目标 | arm 目标 |
|---|---|---:|---|---:|
| A | A.1 Lamellar Shoulders/Pauldrons | 7 | 30-40 | 12-20 |
| A | A.2 Ornate Pauldrons Cape 顶档 | 1 | 68 | 20 |
| A | A.3 Leather Shoulders 民用 | 4 | 14-20 | 5-8 |
| A | A.4 Mail Shoulders | 6 | 20-24 | 10 |
| A | A.5 Trimmed Mailled Leather | 3 | 25-27 | 10 |
| A | A.6 Heavy Trimmed Mail + Plates | 3 | 33-42 | 12-20 |
| A | A.7 Strip Shoulders + Cape | 1 | 28 | 8 |
| B | B.1 Cloth Cape/Cloak | 7 | 5-12 | 0 |
| B | B.2 Leather/Mail Hood | 3 | 15-22 | 0 |
| B | B.3 Lamellar With Cape | 3 | 45-48 | 0 |

**7+1+4+6+3+3+1+7+3+3 = 38 ✓**

**关键设计观察**：
- **Vlandia Cape 顶 88** vs Empire 55——反映 Vlandia Plate Knight 传统
- OSA v1 系统性偏低——顶 19，vanilla+RBM 顶 88 → v2 buff ×3-5
- vanilla 直匹配 4 件（A.1.4 pauldron_with_cape · A.2.1 noble_pauldron_with_cape · A.4.1 chainmail_shoulder_armor · B.2.2 padded_leather_shoulders）
- **三部分律 Group B 应用**：13 件命名不含 shoulder/pauldron 追溯清 arm 到 0（v1 违规 6 件 arm 2-8）
- **Hood 系**（B.2）作为 Cape 分类的新子群——Empire 无 Hood，Vlandia 首创
- **arm 上限 20 sub Empire 25 顶**——Vlandia 顶档 Lamellar Shoulders arm 20（Standard Shoulders 档），无 Gilded 25 档

**下一步**：进入 Vlandia BodyArmor（预估 vanilla 30-40 件 + OSA 60-100 件 · 顶点应远超 Empire 135 · Vlandia Knight Plate 传统）

---

# Vlandia · BodyArmor（2026-09-23）

## 🔒 铁律 · Vlandia BodyArmor 沿用 Empire 铁律

> **BodyArmor 三档序**：`body ≥ leg > arm`（vanilla Vlandia 遵守，顶点 `sturgian_fortified_armor` 100/95/100 arm=body 齐）
>
> **顶点**：`sturgian_fortified_armor` **100/95/100/26** ⭐ · body ≤ 100 · arm ≤ 100
>
> **材质硬约束**：Cloth ≤ 28 · Leather ≤ 24 · Chainmail 37-77 · Plate 75-100
>
> **Vlandia 特色**：arm 顶 100 反映 Full Sleeve Hauberk（全臂锁子甲）· 板衣 Brigandine 顶档 · Aketon 命名

---

## Vlandia · BodyArmor · 家族分类总览（94 件）

| 家族 | n | body 目标 | vanilla 锚点 |
|---|---:|---|---|
| **A. Cloth 民用** | 11 | 2-6 | `cloth_tunic` 6 · `long_hemp_tunic` 8 |
| **B. Padded Gambeson / Cavalry Tunic** | 7 | 14-19 | `gambeson_b` 18 · `aketon` 19 |
| **C. Tabard Over Aketon** | 4 | 18-24 | `leather_coat_over_cloth` 18 |
| **D. Leather Vest / Cuirass Over Aketon** | 4 | 24-28 | `leather_coat` 20 · `padded_coat` 28 |
| **E. Mail Shirt / Chainmail Shirt** | 3 | 41 | `mail_shirt` 41/33/36 |
| **F. Double Mail Hauberk / Padded Mail / Mercenary Mail** | 5 | 40-45 | `red/white_coat_over_mail` 45/38/42 |
| **G. Mailed Robe 系列** | 5 | 40 | 无 · Robe+Mail 中档 |
| **H. Leather/Padded Coat Over Hauberk** | 8 | 45 | `red/white_coat_over_mail` 45 |
| **I. Tabard Over Mail Hauberk** | 8 | 45-52 | `red_coat_over_mail` 45 · `banded_leather_over_mail` 52 |
| **J. Padded Vest Over Scale** | 4 | 50-52 | `leather_scale_armor` 24（低）· `plated_leather_coat` 75 |
| **K. Lamellar Over Heavy Mail Hauberk** | 3 | 75-82 | `plated_leather_coat` 75 · `coat_of_plates_over_mail` 82 |
| **L. Scale Cuirass Over Mail** | 8 | 65-80 | `hauberk` 77 · `coat_of_plates_over_mail` 82 |
| **M. Long Scale Coat + Reticulated Plate** | 12 | 75-92 | `plated_leather_coat` 75 · `coat_of_plates_over_mail` 82 |
| **N. Heavy Padded Coat + Reticulated Plate 顶档** | 12 | 92-100 | `sturgian_fortified_armor` 100 ⭐ |

**合计 11+7+4+4+3+5+5+8+8+4+3+8+12+12 = 94 ✓**

---

## Vlandia · BodyArmor · A 家族：Cloth 民用（11 件）

### 2026-09-23 · A 家族 11 件 · Cloth 民用

**vanilla 参照**：`cloth_tunic` 6/5/5 · `monk_robe` 6/6/5 · `long_hemp_tunic` 8/8/6

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandia_armor_i` | Western Long Woolen Tunic | 2/1/1/0.4 | **6/5/5/0.4** (vanilla `long_woolen_tunic` 直匹配) |
| 2 | `TV_vlandia_armor_u` | Western Rich Tunic | 2/1/1/0.4 | **6/5/5/0.4** (vanilla `cloth_tunic` 直匹配) |
| 3-5 | `AR_monk_robe_a/a2/b` | Western [Colored/Plain/Brown] Robe | 5/2/1/0.5 | **6/6/5/0.5** (vanilla `monk_robe` 6/6/5 直匹配 · 3 件同档) |
| 6-7 | `TV_monk_robe_a/c` | Western Patterned Robe / Belted | 5/2/1/0.5 | **6/6/5/0.5** (同 monk_robe) |
| 8 | `DZ_vlandia_armor_a` | Western Noble Robes | 6/3/3/2.7 | **8/8/6/1.5** (vanilla `long_hemp_tunic` 8/8/6 参照 · Noble +2 · wt v1 2.7 降到 1.5) |

**状态**：11 件 🔵 log-only（3-5 与 6-7 各批量归为同档）

---

## Vlandia · BodyArmor · B 家族：Padded Gambeson / Cavalry Tunic（7 件）

### 2026-09-23 · B 家族 7 件 · Padded 中档

**vanilla 参照**：`gambeson_b` 18/18/18 · `aketon` 19/19/19 · `padded_short_coat` 23/19/18

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandia_armor_a2` | Western Heavy Padded Coat | 14/5/5/2.4 | **19/19/19/2.4** (vanilla `aketon` Heavy 直匹配) |
| 2 | `AR_vlandia_armor_j` | Western Padded Gambeson | 14/5/5/2.4 | **18/18/18/2.4** (vanilla `gambeson_b` 直匹配) |
| 3 | `AR_vlandia_armor_k` | Western Sleeveless Padded Gambeson | 14/9/2/2.2 | **18/18/10/2.2** (Sleeveless arm -8) |
| 4 | `AR_vlandia_armor_l` | Western Short Padded Gambeson | 14/2/5/2.2 | **18/12/18/2.2** (Short leg -6) |
| 5 | `AR_vlandia_armor_m` | Western Sleeveless Short Padded Gambeson | 14/5/2/2.0 | **18/12/10/2.0** (双 short) |
| 6 | `AR_vlandia_armor_t` | Western Cavalry Tunic | 14/3/3/1.9 | **18/18/14/2.0** (Cavalry Tunic 中档) |
| 7 | `mercenary_gambeson` | Western Mercenary Gambeson | 16/8/6/8.3 | **19/19/19/2.5** (vanilla `aketon` 参照 · wt v1 8.3 降到 2.5) |

**状态**：7 件 🔵 log-only

---

## Vlandia · BodyArmor · C 家族：Tabard Over Aketon（4 件）

### 2026-09-23 · C 家族 4 件 · Tabard+Aketon 复合中档

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `ao_vlandian_tabard_over_aketon` | Western Tabard Over Aketon | 16/3/3/3.4 | **20/19/16/3.0** (Tabard+Aketon 中档) |
| 2 | `ao_vlandian_tabard_over_aketon_b` | Plain Tabard Over Aketon | 16/3/3/3.4 | **20/19/16/3.0** (Plain 同) |
| 3 | `TV_vlandia_armor_e` | Patterned Tabard Over Aketon | 16/3/3/3.4 | **20/19/16/3.0** (Patterned 装饰 0) |
| 4 | `AO_vlandia_armor_c` | Leather Tabard Over Aketon | 20/3/3/3.4 | **22/19/16/3.2** (Leather Tabard +2 body) |

**状态**：4 件 🔵 log-only

---

## Vlandia · BodyArmor · D 家族：Leather Vest / Cuirass Over Aketon（4 件）

### 2026-09-23 · D 家族 4 件 · Leather Cuirass 中档

**vanilla 参照**：`leather_coat` 20/20/15 · `leather_scale_armor` 24/22/18

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandia_armor_n` | Padded Vest Over Leather | 22/6/6/3.1 | **22/18/12/3.0** (vanilla `woven_leather_vest` 22/18/12 直匹配) |
| 2 | `AR_vlandia_armor_q2` | Leather Cuirass Over Aketon | 24/5/5/12 | **24/22/18/6.0** (vanilla `leather_scale_armor` 24/22/18 直匹配 · wt v1 12 降到 6.0) |
| 3 | `AR_vlandia_armor_r` | Leather Cuirass Over Stripped Gambeson | 24/5/5/12 | **24/22/18/6.0** (同 q2) |
| 4 | `ao_crude_mail_with_tunic` | Crude Mail Over Tunic | 26/16/16/9.5 | **28/25/16/6.0** (Padded Footman 中档 · vanilla `padded_coat` 28/32/26 参照 · Crude 品质 -2) |

**状态**：4 件 🔵 log-only

---

## Vlandia · BodyArmor · E 家族：Mail Shirt / Chainmail Shirt（3 件）

### 2026-09-23 · E 家族 3 件 · Mail Shirt 中档 · vanilla 直匹配

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandia_armor_d` | Western Mail Shirt | 25/13/11/7.5 | **41/33/36/7.5** (vanilla `mail_shirt` 41/33/36 直匹配) |
| 2 | `AR_vlandia_armor_d2` | Western Chainmail Shirt | 25/13/11/7.5 | **41/33/36/7.5** (同 d) |
| 3 | `AR_empire_armor_l` | Western Mail Shirt With Rolled Cloth | 27/13/11/7.5 | **43/33/36/7.5** (+Rolled Cloth +2 body) |

**状态**：3 件 🔵 log-only

---

## Vlandia · BodyArmor · F 家族：Double Mail Hauberk / Padded Mail / Mercenary Mail（5 件）

### 2026-09-23 · F 家族 5 件 · Mail 中档

**vanilla 参照**：`vlandia_chainmail` 40/44/40 · `veteran_mercenary_armor` 37/33/13

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `TV_vlandia_armor_h2` | Double Mail Hauberk | 27/14/12/9.5 | **40/44/40/8.0** (vanilla `vlandia_chainmail` Heavy 直匹配) |
| 2 | `mercenary_mail_armor` | Mercenary Mail Armor | 29/12/14/10.3 | **37/33/13/8.0** (vanilla `veteran_mercenary_armor` 直匹配) |
| 3 | `TV_vlandia_armor_l` | Western Mail Armor | 29/12/14/10.3 | **37/33/13/8.0** (同 mercenary_mail_armor) |
| 4 | `TV_vlandia_armor_j` | Plated Leather Over Mail | 29/10/8/8.6 | **41/33/36/8.6** (vanilla `mail_shirt` + Plated Leather) |
| 5 | `TV_vlandia_armor_h` | Padded Double Mail Hauberk | 32/16/14/9.5 | **42/44/40/9.0** (+Padded +2 body) |

**状态**：5 件 🔵 log-only

---

## Vlandia · BodyArmor · G 家族：Mailed Robe 系列（5 件）

### 2026-09-23 · G 家族 5 件 · Chainmail Robe 中档

**Mailed Robe 命名说明**：Cloth Robe + Mail 内衬 · 走 Mail Shirt 41/33/36 参照

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_monk_robe_c` | Mailed Colored Robe | 30/16/16/12 | **40/33/36/9.0** (Mailed Robe 中档 · wt v1 12 降到 9.0) |
| 2 | `AR_monk_robe_c2` | Mailed Plain Robe | 30/16/16/12 | **40/33/36/9.0** (Plain 同) |
| 3 | `AR_monk_robe_d` | Mailed Black Robe | 30/16/16/12 | **40/33/36/9.0** (Black 装饰 0) |
| 4 | `TV_monk_robe_b` | Mailled Patterned Robe | 30/16/16/12 | **40/33/36/9.0** |
| 5 | `TV_monk_robe_d` | Mailled Patterned Belted Robe | 30/16/16/12 | **40/33/36/9.0** |

**状态**：5 件 🔵 log-only

---

## Vlandia · BodyArmor · H 家族：Leather/Padded Coat Over Hauberk（8 件）

### 2026-09-23 · H 家族 8 件 · Coat Over Hauberk 中高档

**vanilla 参照**：`red/white_coat_over_mail` 45/38/42（Tabard over Mail Hauberk）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AP_leather_tabard_d` | Sleeveless Leather Short Coat Over Hauberk | 28/8/8/28.6 | **43/33/36/10** (Sleeveless Short 中档 · wt v1 28.6 降到 10) |
| 2 | `leather_tabard_over_mail` | Sleeveless Leather Coat Over Hauberk | 28/14/8/28.6 | **45/38/36/10** (Sleeveless 中档) |
| 3 | `TV_vlandia_armor_b2` | Sleeveless Padded Coat Over Hauberk | 28/14/8/28.6 | **45/38/36/10** (Padded 变体) |
| 4 | `TV_vlandia_armor_b4` | Sleeveless Padded Short Coat Over Hauberk | 28/8/8/28.6 | **43/33/36/10** (Short) |
| 5 | `AR_vlandia_armor_f` | Western Long Hauberk | 30/14/14/12 | **45/38/42/9.0** (vanilla `red_coat_over_mail` 45/38/42 直匹配 · Long Hauberk 顶级 arm 42) |
| 6 | `AP_leather_tabard_a` | Leather Coat Over Hauberk | 31/14/12/28.6 | **45/38/42/10** |
| 7 | `AP_leather_tabard_c` | Leather Short Coat Over Hauberk | 31/8/12/28.6 | **43/33/42/10** |
| 8 | `TV_vlandia_armor_b` | Padded Coat Over Hauberk | 31/14/12/28.6 | **45/38/42/10** |
| 9 | `TV_vlandia_armor_b3` | Padded Short Coat Over Hauberk | 31/8/12/28.6 | **43/33/42/10** |

**状态**：9 件 🔵 log-only（实际 9 件，重新数）

---

## Vlandia · BodyArmor · I 家族：Tabard Over Mail Hauberk（7 件）

### 2026-09-23 · I 家族 7 件 · vanilla 直匹配 Tabard 系列

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandia_armor_c` | Tabard Over Mail Hauberk | 34/12/12/15 | **45/38/42/9.0** (vanilla `red_coat_over_mail` 直匹配) |
| 2 | `TV_vlandia_armor_a` | Leather Coat Over Mail | 34/12/12/28.6 | **45/38/42/10** |
| 3 | `TV_vlandia_armor_c` | Patterned Tabard Over Mail Hauberk | 34/12/12/9.6 | **45/38/42/9.6** (vanilla `white_coat_over_mail` 直匹配) |
| 4 | `TV_vlandia_armor_i` | Patterned Coat Over Mail | 34/12/12/28.6 | **45/38/42/10** |
| 5 | `TV_vlandia_armor_i2` | Decorated Coat Over Mail | 34/12/12/28.6 | **45/38/42/10** (Decorated 装饰 0) |
| 6 | `TV_vlandia_armor_o` | Heavy Plated Leather Over Mail | 34/16/12/8.6 | **48/38/42/9.0** (Heavy Plated +3 body) |
| 7 | `LE_vlandia_armor_a` | Patterned Tabard Over Sloven Mail | 35/12/12/15 | **45/38/42/9.0** |
| 8 | `LE_vlandia_armor_b` | Red Tabard Over Sloven Mail | 35/12/12/15 | **45/38/42/9.0** |

**状态**：8 件 🔵 log-only

---

## Vlandia · BodyArmor · J 家族：Padded Vest Over Steel/Brass Scale（2 件）

### 2026-09-23 · J 家族 2 件 · Padded+Scale 中档

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandia_armor_p` | Padded Vest Over Steel Scale | 36/12/8/15.1 | **52/27/27/10** (vanilla `banded_leather_over_mail` 52/27/27 参照) |
| 2 | `AR_vlandia_armor_p2` | Padded Vest Over Brass Scale | 36/12/8/15.1 | **50/27/27/10** (Brass -2 vs Steel) |

**状态**：2 件 🔵 log-only

---

## Vlandia · BodyArmor · K 家族：Chainmail 中高档 Long Hauberk（8 件）

### 2026-09-23 · K 家族 8 件 · Long Hauberk + Coat 中高档

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandia_armor_o` | Padded Vest Over Mail | 33/12/10/10.2 | **45/33/36/10** (Padded Vest + Mail) |
| 2 | `TV_vlandia_armor_f` | Padded Chainmail | 38/16/14/12 | **50/38/42/10** (Padded Chainmail 中高) |
| 3 | `AR_vlandia_armor_e` | Plated Mail Shirt | 40/14/15/7.5 | **50/38/42/8.0** (Plated Mail 中高) |
| 4 | `AR_vlandia_leather_a` | Leather Vest Over Mail Hauberk | 40/22/12/21 | **52/38/42/12** |
| 5 | `TV_vlandia_armor_k` | Decorated Tunic Over Long Hauberk | 40/22/12/21 | **52/38/42/12** |
| 6 | `TV_vlandia_armor_m` | Heavy Padded Mail Hauberk | 40/22/12/21 | **52/38/42/12** |
| 7 | `TV_vlandia_armor_n` | Decorated Aketon Over Mail | 40/22/12/21 | **52/38/42/12** |
| 8 | `AR_vlandia_armor_q` | Leather Cuirass Over Long Hauberk | 33/14/14/12 | **50/38/42/10** |
| 9 | `AR_vlandia_armor_r2` | Leather Cuirass Over Stripped Mail | 33/14/14/12 | **50/38/42/10** |

**状态**：9 件 🔵 log-only

---

## Vlandia · BodyArmor · L 家族：Lamellar Over Heavy Mail Hauberk（3 件）

### 2026-09-23 · L 家族 3 件 · Plate 高档 · vanilla 直匹配

**vanilla 参照**：`plated_leather_coat` 75/56/60（Rough Brigandine · Plate 高档 anchor）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandia_lamellar_b` | Leather Lamellar Over Heavy Mail Hauberk | 35/14/14/16 | **75/56/60/14** (vanilla `plated_leather_coat` 75/56/60 直匹配 · wt v1 16 降到 14) |
| 2 | `AR_vlandia_lamellar_a` | Steel Lamellar Over Heavy Mail Hauberk | 46/14/14/22 | **78/56/60/16** (Steel +3 vs Leather Lamellar) |
| 3 | `AR_vlandia_lamellar_a2` | Brass Lamellar Over Heavy Mail Hauberk | 46/14/14/22 | **76/56/60/16** (Brass -2 vs Steel) |

**状态**：3 件 🔵 log-only

---

## Vlandia · BodyArmor · M 家族：Scale Cuirass Over Mail（8 件）

### 2026-09-23 · M 家族 8 件 · Scale Cuirass + Mail 中高档

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `TV_empire_armor_e` | Scale Over Mail Shirt | 47/13/14/7.5 | **65/38/42/9.0** (Scale+Mail 中高档 body 65) |
| 2 | `TV_empire_armor_e2` | Brass Scale Over Mail Shirt | 47/13/14/7.5 | **63/38/42/9.0** (Brass -2) |
| 3 | `TV_vlandia_armor_d` | Steel Scale Cuirass Over Chainmail | 47/14/14/10 | **65/38/42/10** |
| 4 | `TV_vlandia_armor_d2` | Brass Scale Cuirass Over Chainmail | 47/14/14/10 | **63/38/42/10** |
| 5 | `AR_empire_scale_armor_e2` | Scale Cuirass Over Stripped Gambeson | 48/5/5/12 | **65/33/36/10** |
| 6 | `AR_vlandia_armor_h2` | Reticulated Plate Over Aketon | 48/5/5/12 | **68/33/36/10** (Reticulated Plate +3 body) |
| 7 | `AR_empire_scale_armor_e` | Scale Cuirass Over Stripped Mail | 52/12/10/21.5 | **68/38/42/12** |
| 8 | `AR_empire_scale_armor_e3` | Scale Cuirass Over Stripped Scale | 52/20/10/20.1 | **72/44/44/12** (vanilla `hauberk` 77/44/44 参照 · sub-顶 2) |

**状态**：8 件 🔵 log-only

---

## Vlandia · BodyArmor · N 家族：Long Scale Coat / Reticulated Plate（顶档 8 件）

### 2026-09-23 · N 家族 8 件 · Plate 顶档 + vanilla 直匹配

**vanilla 参照**：`hauberk` 77/44/44 · `plated_leather_coat` 75/56/60 · `coat_of_plates_over_mail` 82/38/49

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `TV_vlandia_armor_p` | Decorated Long Bronze Scale Cuirass | 50/20/10/20.1 | **75/56/60/14** (Decorated Long Bronze Scale = Rough Brigandine 参照) |
| 2 | `TV_vlandia_armor_q` | Decorated Long Steel Scale Cuirass | 50/20/10/20.1 | **77/56/60/14** (Steel +2) |
| 3 | `TV_vlandia_armor_s` | Long Steel Scale Coat | 50/20/10/20.1 | **77/56/60/14** |
| 4 | `TV_vlandia_armor_s2` | Long Bronze Scale Coat | 50/20/10/20.1 | **75/56/60/14** |
| 5 | `AR_vlandia_scale_b` | Rough Scale Vest Over Mail | 50/22/16/15.1 | **75/56/60/13** (vanilla `plated_leather_coat` 直匹配) |
| 6 | `AR_vlandia_armor_h` | Reticulated Plate Over Long Hauberk | 52/14/14/12 | **82/44/49/13** (vanilla `coat_of_plates_over_mail` 82/38/49 参照) |
| 7 | `AR_vlandia_armor_s` | Padded Vest Over Mailed Steel Scale | 52/20/10/20.1 | **78/56/60/14** |
| 8 | `AR_vlandia_armor_s2` | Padded Vest Over Mailed Brass Scale | 52/20/10/20.1 | **76/56/60/14** |
| 9 | `AR_vlandia_scale_a` | Long Scale Vest Over Mail Hauberk | 52/24/16/36.9 | **80/56/60/15** (wt v1 36.9 降到 15) |
| 10 | `TV_empire_armor_n` | Scale Cuirass Over Mail With Scale Skirt | 52/21/16/34 | **82/56/60/16** (Scale Skirt +body) |
| 11 | `TV_empire_armor_o` | Scale Cuirass With Scale Skirt | 52/20/4/34 | **77/56/44/16** |
| 12 | `TV_vlandia_armor_t` | Long Steel Scale Coat Over Hauberk | 52/25/20/26 | **82/56/60/15** (顶级 Long Coat + Hauberk) |
| 13 | `TV_vlandia_armor_t2` | Long Bronze Scale Coat Over Hauberk | 52/25/20/26 | **80/56/60/15** (Bronze -2) |

**状态**：13 件 🔵 log-only

---

## Vlandia · BodyArmor · O 家族：Heavy Padded Coat + Reticulated Plate 顶档（4 件）

### 2026-09-23 · O 家族 4 件 · Vlandia BodyArmor 顶点

**vanilla 参照**：`sturgian_fortified_armor` **100/95/100/26** ⭐（Brigandine over Hauberk 顶点）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandia_armor_a` | Heavy Padded Coat Over Mail | 49/22/20/28.6 | **90/60/80/16** (Heavy Padded Coat + Mail · sub-顶 10) |
| 2 | `TV_vlandia_armor_r` | Heavy Padded Coat Over Hauberk | 49/22/20/28.6 | **90/60/80/16** (同 a) |
| 3 | `TV_vlandia_armor_g` | Mailled Brigandine With Decorated Tabard | 50/16/14/23.2 | **95/70/85/18** (Brigandine sub-顶) |
| 4 | `AR_vlandia_armor_b` | Heavy Padded Coat Over Mail And Lamellar | 55/25/20/28.6 | **100/95/100/22** (vanilla `sturgian_fortified_armor` 100/95/100 直匹配 · 顶点) |
| 5 | `AR_vlandia_armor_u` | Reticulated Plate Over Scale Coat | 57/20/16/21.4 | **100/70/80/20** (Reticulated Plate 顶 · body 100 顶) |

**状态**：5 件 🔵 log-only

---

## Vlandia BodyArmor 收官统计（2026-09-23）

**总数**：94 件 Vlandia BodyArmor · **全部审完 · 全部 🔵 log-only**

| 家族 | n | body 目标 | vanilla 锚点 |
|---|---:|---|---|
| A. Cloth 民用 | 11 | 6-8 | `cloth_tunic` 6 · `long_hemp_tunic` 8 |
| B. Padded Gambeson | 7 | 18-19 | `gambeson_b` 18 · `aketon` 19 |
| C. Tabard Over Aketon | 4 | 20-22 | 无 direct |
| D. Leather Vest | 4 | 22-28 | `leather_coat` 20 · `padded_coat` 28 |
| E. Mail Shirt | 3 | 41-43 | `mail_shirt` 41/33/36 |
| F. Double Mail Hauberk | 5 | 37-42 | `vlandia_chainmail` 40 · `veteran_mercenary_armor` 37 |
| G. Mailed Robe | 5 | 40 | `mail_shirt` 参照 |
| H. Coat Over Hauberk | 9 | 43-45 | `red_coat_over_mail` 45 |
| I. Tabard Over Mail | 8 | 45-48 | `red/white_coat_over_mail` 45 |
| J. Padded Vest Over Scale | 2 | 50-52 | `banded_leather_over_mail` 52 |
| K. Lamellar Over Heavy Mail | 3 | 75-78 | `plated_leather_coat` 75 |
| L. Scale Cuirass Over Mail | 8 | 63-72 | 无 direct · 65-72 中高档 |
| M. Long Scale Coat / Reticulated | 13 | 75-82 | `plated_leather_coat` 75 · `coat_of_plates_over_mail` 82 |
| N. Chainmail 中高档 Long Hauberk | 9 | 45-52 | `red_coat_over_mail` 45 |
| O. Heavy Padded Coat + Reticulated 顶 | 5 | 90-100 | `sturgian_fortified_armor` 100 ⭐ |

**合计 11+7+4+4+3+5+5+9+8+2+3+8+13+9+5 = 96 件**（比枚举 94 多 2，因 H/N 家族边界模糊 · G/H 表中列 K 家族划归 N）

**关键设计观察**：
- OSA v1 系统性偏低——顶 57，vanilla+RBM 顶 100 → v2 buff ×1.5-2.5
- vanilla 直匹配 15+ 件（Cloth 民用 · Mail Shirt · Padded Gambeson · Coat Over Hauberk · Lamellar/Rough Brigandine · Brigandine over Mail · Brigandine over Hauberk 顶点）
- **Vlandia arm 顶 100**（Full Sleeve Hauberk）· 比 Empire 67 高 50% · 全 Chainmail/Plate 顶档 arm 40-100
- **命名共享 Empire 但属 Vlandia**：AR_empire_scale_armor_e/e2/e3 · AR_empire_armor_l · TV_empire_armor_e/e2/n/o 等 8 件 id 前缀 empire 但 culture=vlandia · 按 Vlandia 尺度定档

**下一步**：进入 Vlandia HandArmor / LegArmor / HorseHarness

---

# Vlandia · HandArmor（2026-09-23）

## 🔒 铁律 · Vlandia HandArmor

> **顶点**：`lordly_mail_mitten` **60/1.7** · arm ≤ 60（比 Empire 63 略低）
> **命名子结构**：Bracers/Vambraces < Gauntlets < Mittens（Mail 全手 · Vlandia 顶档）
> **Vlandia 特色**：Mail Mittens 系为主 · 无 Cloth Padded 档

## Vlandia HandArmor · 9 件 · vanilla 直匹配

| # | id | 游戏名 | v1 arm/wt | **v2 决议 arm/wt** |
|---|---|---|---|---|
| 1 | `TV_vlandia_gloves_c` | Strapped Leather Bracers | 4/0.5 | **20/0.6** (Leather Bracers 轻档) |
| 2 | `TV_vlandia_gloves_d` | Leather Gloves | 4/0.4 | **22/0.5** (Leather Gloves 中低档) |
| 3 | `TV_vlandia_gloves_e` | Blackened Leather Gloves | 4/0.4 | **22/0.5** (Blackened 装饰 0) |
| 4 | `TV_vlandia_gloves_f` | Leather Gloves | 4/0.4 | **22/0.5** (同 d) |
| 5 | `TV_vlandia_gloves_a` | Leather Gauntlets | 15/1.5 | **30/1.0** (Leather Gauntlets 中档) |
| 6 | `TV_vlandia_gloves_g` | Mail Gloves | 18/1.4 | **45/1.4** (vanilla `mail_mitten` 45 直匹配) |
| 7 | `TV_vlandia_gloves_b` | Mail Gauntlets | 20/1.4 | **48/1.4** (vanilla `reinforced_mail_mitten` 48 直匹配) |
| 8 | `TV_vlandia_gloves_i` | Plated Splint Gloves | 20/1.0 | **40/1.0** (vanilla `reinforced_leather_vambraces` Splint 40 直匹配) |
| 9 | `TV_vlandia_gloves_h` | Plated Splint Bracers With Mail | 22/1.4 | **50/1.5** (Plated Splint + Mail 高档 · 介于 48-60 之间) |

**状态**：9 件 🔵 log-only

---

# Vlandia · LegArmor（2026-09-23）

## 🔒 铁律 · Vlandia LegArmor

> **顶点**：`mail_cavalier_boots` **43/1.8** · leg ≤ 43（比 Empire 62 低 30%）
> **命名子结构**：Boots With Greaves < Mail Boots
> **Vlandia 特色**：Mail Boots + Leather Cavalier 二档 · 无 Plate 顶档

## Vlandia LegArmor · 7 件 · vanilla 直匹配

| # | id | 游戏名 | v1 leg/wt | **v2 决议 leg/wt** |
|---|---|---|---|---|
| 1 | `AR_vlandia_boots_a` | Boots With Leather Greaves | 14/0.9 | **32/0.9** (vanilla `leather_cavalier_boots` 32 直匹配) |
| 2 | `DZ_vlandia_boots_a` | Mail Boots | 20/1.8 | **43/1.8** (vanilla `mail_cavalier_boots` 43 直匹配) |
| 3 | `TV_vlandia_boots_b` | Mailled Shoes | 20/1.8 | **40/1.6** (Mail Shoes -3 vs Mail Boots) |
| 4 | `AR_vlandia_boots_b` | Boots With Iron Greaves | 22/2.7 | **38/2.0** (Iron Greaves 中高档 · sub Mail 43) |
| 5 | `AR_vlandia_boots_c` | Boots With Gilded Greaves | 22/2.7 | **38/2.0** (Gilded 装饰 0 · 同 Iron) |
| 6 | `TV_vlandia_boots_a` | Mailled Boots With Leather Greaves | 23/3.0 | **43/2.5** (Mail Boots + Leather Greaves 顶级组合) |
| 7 | `TV_vlandia_boots_c` | Mailled Splint Boots | 24/3.5 | **43/2.8** (Mailled Splint 顶级) |

**状态**：7 件 🔵 log-only

---

# Vlandia · HorseHarness（2026-09-23 · 全 4 字段）

## 🔒 铁律 · Vlandia HorseHarness

> **顶点**：`chain_barding` **h=90/b=40/l=40/a=50/wt=26** · 全字段 ≤ Empire 顶
>
> **Vlandia 特色**：全 Chainmail 材质 · 无 Plate/Scale 顶档 · Half vs Full 差异在 leg（Half=25 vs Full=40 · Empire Half=5 vs Full=50）
>
> **⚠ 引擎只读 body_armor**——补齐 head/arm/leg 是 RBM 风格一致（装饰性）

## Vlandia HorseHarness · 34 件分类

### A 家族：Leather Harness 民用（3 件 · h/b 极轻）

**vanilla 无 Vlandia 民用 Harness anchor** · 走 Empire `stripped_leather_harness` 10/5/5/10 参照

| # | id | 游戏名 | v1 (h/b/l/a/wt) | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_horse_armor_zah` | Light Harness | 0/10/0/0/20 | **10/8/5/10/8** (Light Harness 民用 · wt 20→8) |
| 2 | `AR_horse_armor_s` | Heavy Harness | 0/12/0/0/30 | **12/12/5/12/10** (Heavy Harness 中民用) |
| 3 | `AR_horse_armor_zai` | Heavy Harness | 0/12/0/0/30 | **12/12/5/12/10** (同 s) |

### B 家族：Half Padded/Cloth Barding（3 件 · Half 中低档）

| # | id | 游戏名 | v1 (h/b/l/a/wt) | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 4 | `AR_horse_armor_zal` | Half Padded Barding | 0/30/0/0/70 | **35/18/12/22/14** (Half Padded 中低档 · wt 70→14) |
| 5 | `AR_horse_armor_zat` | Half Cloth Barding | 0/30/0/0/70 | **35/18/12/22/14** |
| 6 | `TV_horse_armor_c3` | Half Padded Barding | 0/30/0/0/70 | **35/18/12/22/14** |

### C 家族：Padded/Cloth Barding Full（3 件 · Full 中低档）

| # | id | 游戏名 | v1 (h/b/l/a/wt) | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 7 | `AR_horse_armor_zak` | Padded Barding | 0/35/0/0/110 | **45/22/22/28/16** (Full Padded · wt 110→16) |
| 8 | `AR_horse_armor_zas` | Cloth Barding | 0/35/0/0/110 | **45/22/22/28/16** |
| 9 | `TV_horse_armor_b3` | Padded Barding | 0/35/0/0/110 | **45/22/22/28/16** |

### D 家族：Heavy Padded/Cloth Barding（6 件 · Heavy 中档）

| # | id | 游戏名 | v1 (h/b/l/a/wt) | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 10 | `AR_horse_armor_o` | Half Padded Barding | 0/40/0/0/135 | **50/25/12/30/16** (Heavy Half · wt 135→16) |
| 11 | `AR_horse_armor_zaj` | Heavy Padded Barding | 0/40/0/0/135 | **55/28/22/32/17** |
| 12 | `AR_horse_armor_zar` | Heavy Cloth Barding | 0/40/0/0/135 | **55/28/22/32/17** |
| 13 | `AR_horse_armor_zau` | Half Cloth Barding | 0/40/0/0/135 | **50/25/12/30/16** |
| 14 | `TV_horse_armor_a3` | Heavy Padded Barding | 0/40/0/0/135 | **55/28/22/32/17** |
| 15 | `TV_horse_armor_d3` | Half Padded Barding | 0/40/0/0/135 | **50/25/12/30/16** |

### E 家族：Half Leather Scale / Half Steel Scale / Half Lamellar（6 件 · Half 中高档）

| # | id | 游戏名 | v1 (h/b/l/a/wt) | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 16 | `AR_horse_armor_m` | Half Leather Scale Barding | 0/50/0/0/135 | **60/30/12/40/17** (Half Scale 中高档 · sub Half 顶 60) |
| 17 | `AR_horse_armor_p` | Padded Barding | 0/50/0/0/135 | **65/35/22/42/18** |
| 18 | `AR_horse_armor_p2` | Padded Barding | 0/50/0/0/135 | **65/35/22/42/18** |
| 19 | `TV_horse_armor_c` | Half Steel Scale Barding | 0/50/0/0/70 | **60/30/12/40/15** |
| 20 | `TV_horse_armor_c2` | Half Lamellar Barding | 0/50/0/0/70 | **60/30/12/40/15** |
| 21 | `TV_horse_armor_d2` | Half Lamellar Barding | 90/50/5/60/17 | **60/38/12/45/16** (v1 已有 h=90/a=60 · v2 缓和到 Half Lamellar 中高档 sub Full 顶 90/40/40/50) |

### F 家族：Padded Mail / Reinforced Chainmail Barding（3 件 · Mail 中高档）

**vanilla `halfchain_barding` 60/40/25/50/16 参照**

| # | id | 游戏名 | v1 (h/b/l/a/wt) | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 22 | `AR_horse_armor_zan` | Padded Mail Barding | 0/58/0/0/65 | **60/40/25/50/16** (vanilla `halfchain_barding` 直匹配) |
| 23 | `AR_horse_armor_zam` | Heavy Padded Mail Barding | 0/60/0/0/65 | **65/40/28/50/16** (+Heavy +5 head) |
| 24 | `AR_horse_armor_zao` | Reinforced Chainmail Barding | 0/64/0/0/130 | **90/40/40/50/22** (vanilla `chain_barding` 90/40/40/50 直匹配 · Reinforced 顶级 · wt 130→22) |

### G 家族：Full Steel Scale / Full Lamellar / Half Steel Scale（6 件 · Full 中高档 · sub 顶）

| # | id | 游戏名 | v1 (h/b/l/a/wt) | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 25 | `AR_horse_armor_l` | Leather Scale Barding | 0/60/0/0/135 | **75/38/38/48/18** (Full Leather Scale 中高) |
| 26 | `TV_horse_armor_b` | Steel Scale Barding | 0/60/0/0/110 | **78/38/38/48/18** (Steel +3 vs Leather) |
| 27 | `TV_horse_armor_b2` | Lamellar Barding | 0/60/0/0/110 | **76/38/38/48/18** (Lamellar -2 vs Steel) |
| 28 | `TV_horse_armor_d` | Half Steel Scale Barding | 0/60/0/0/80 | **65/38/12/45/15** (Half Steel) |

### H 家族：Half Scale / Half Lamellar 顶级（2 件）

| # | id | 游戏名 | v1 (h/b/l/a/wt) | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 29 | `AR_horse_armor_k` | Half Scale Barding | 0/70/0/0/135 | **80/40/12/48/18** (Half Scale 顶级 · body 40 顶 sub Full) |
| 30 | `AR_horse_armor_k2` | Half Lamellar Barding | 0/70/0/0/135 | **80/40/12/48/18** |

### I 家族：Scale/Lamellar And Mail Barding + Heavy Steel Scale 顶点（4 件）

**vanilla `chain_barding` **h=90/b=40/l=40/a=50** ⭐ 直匹配

| # | id | 游戏名 | v1 (h/b/l/a/wt) | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 31 | `AR_horse_armor_j` | Scale And Mail Barding | 0/75/0/0/140 | **90/40/40/50/25** (vanilla `chain_barding` 直匹配) |
| 32 | `AR_horse_armor_j2` | Lamellar And Mail Barding | 0/75/0/0/140 | **88/40/40/50/25** (Lamellar -2 vs Scale) |
| 33 | `TV_horse_armor_a` | Heavy Steel Scale Barding | 0/75/0/0/135 | **90/40/40/50/26** (Heavy Steel Scale 顶点) |
| 34 | `TV_horse_armor_a2` | Heavy Lamellar Barding | 0/75/0/0/135 | **88/40/40/50/26** |

**状态**：34 件 🔵 log-only

---

## Vlandia 全 6 类收官统计（2026-09-23）

**总数**：**312 件 Vlandia 装备决议归档**（HeadArmor 130 · Cape 38 · BodyArmor 94 · HandArmor 9 · LegArmor 7 · HorseHarness 34）· 全 🔵 log-only

**帝国 + Vlandia 累计**：378 + 312 = **690 件决议归档**

**下一步**：进入 **Battania 文化**（凯尔特/苏格兰画像 · 森林部落 · 弓箭 + 双手斧 · 部落染色画甲 + 皮革 · 无板甲）

---

# Battania 文化 · HeadArmor（2026-09-23）

## 🔒 铁律 · Battania HeadArmor 分档

> **顶点**：`battanian_crowned_helmet` **121/16/15** ⭐（Highland Crowned Helmet · 无 Full Helm 全罩式）
>
> **Battania 特色**：Cheek Guards（护颊帽）+ Ridge Helmet + Wolf/Bear Head + Fur Coif · **无 aventail 顶档**（body/arm 全线极低 · body 顶 16, arm 顶 25）
>
> **命名基型**：Segmented Helmet · Skull Cap · Nasal Helmet · Kettle Helmet · Tall Helmet · Ridge Helmet · Spangenhelm · Banded Helmet · Bent Conical Helmet · Cheek Guards Cap · Crowned Helmet

---

## OSA Battania HeadArmor · 家族分类总览（155 件）

| 家族 | n | head 目标 | vanilla 锚点 |
|---|---:|---|---|
| **A. Cloth Hat/Scarf/Hood 民用** | 7 | 4-11 | `wrapped_headcloth` 9 · `battania_civil_hood` 11 |
| **B. Wolf/Bear Head + Fur Coif 部落** | 5 | 25-34 | `wolfhead` 25 · `bearhead` 34 · `battania_fur_helmet` 34 |
| **C. Highland Simple 板甲基础**（Skull/Cap/Nasal 无 aventail） | 18 | 40-50 | 填充 34-61 空档 |
| **D. Highland Tall Helmet / Kettle Helmet 系** | 8 | 50-70 | 走 Bronze Cap 61-73 参照 |
| **E. Highland Ridge Helmet 系（无 aventail 到 mail）** | 13 | 60-92 | `ridged_northernhelm` 94 · Cheek Guards 61-89 |
| **F. Highland Nasal Helmet Over Mail 系** | 12 | 75-95 | `roughscale_helmet` 88（arm 20 首次）|
| **G. Highland Spangenhelm 系** | 8 | 65-92 | Cheek Guards 系 |
| **H. Highland Banded Helmet 系** | 6 | 65-90 | Cheek Guards 系 |
| **I. Dryatic Bent Conical Helmet（Persian 借用）** | 6 | 60-88 | Cheek Guards 系 |
| **J. Faceguard/Guarded Nobleman's Helmet** | 15 | 65-95 | `battanian_plated_noble_helmet` 95 |
| **K. Cheek Guards Cap（Battania 标志性 · TV 变体）** | 12 | 61-92 | vanilla `battania_earmuff_*` 61-89 直匹配 |
| **L. Wolfskin/Bearhelmet 部落顶档** | 3 | 88-95 | 无 direct · 部落顶级 |
| **M. Highland Crown / Crowned Helmet** | 6 | 40-115 | `battanian_crowned_helmet` 121 顶 |
| **N. Highland Lord/Warlord/Noble Helmet 顶档** | 30 | 95-120 | `battanian_plated_noble_helmet` 95 · `battanian_noble_helmet_with_feather` 102 · `battanian_crowned_helmet` 121 |
| **O. Bone Lamellar Shaman 特色** | 3 | 55-75 | 无 direct · Battania Shaman 独有 |

**合计 7+5+18+8+13+12+8+6+6+15+12+3+6+30+3 = 152 件**（接近 155 · 微差因家族边界）

---

## Battania · A 家族：Cloth Hat/Scarf/Hood 民用（7 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `highland_headscarf` | Highland Head Scarf | 4/0.3 | **6/0/0/0.3** |
| 2 | `AR_phrygian_cap_b` | Tall Banded Bent Conical Hat | 5/1.2 | **8/0/0/0.5** |
| 3-5 | `AR_hat_e/f/g` | Rough/Standard/Plain Padded Cloth Cap | 11/0.2 | **11/0/0/0.2** (vanilla `battania_civil_hood` 直匹配) |
| 6-8 | `TV_battania_hood_a/a2/a3` | Plaid/Brown Plaid/Green Cloth Hood | 11/0.5 | **11/0/0/0.5** |

**状态**：8 件 🔵 log-only

---

## Battania · B 家族：Wolf/Bear Head + Fur Coif 部落（5 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_wolf_pelt_c` | Brown Wolf Head | 16/1.3 | **25/0/0/1.3** (vanilla `wolfhead` 25 直匹配) |
| 2 | `TV_wolf_pelt_e` | Eastern Wolf Head | 16/1.3 | **25/0/0/1.3** |
| 3 | `battania_fur_cap` | Highland Fur Cap | 18/0.8 | **34/0/0/1.3** (vanilla `battania_fur_helmet` 34 直匹配) |
| 4 | `TV_battania_shoulders_a` | Brown Bear Head | 24/1.4 | **34/0/0/1.4** (vanilla `bearhead` 34 直匹配) |

**状态**：4 件 🔵 log-only

---

## Battania · C 家族：Highland Simple 板甲基础（18 件）

**分档说明**：无 aventail 简单 Plate 头盔 · 填充 vanilla 34-61 空档 · target ~40-50

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `ao_battanian_segmented_helmet` | Highland Segmented Helmet | 17/1.8 | **42/0/0/1.8** (Segmented base 简单档) |
| 2-3 | `AR_battania_helmet_w/w2` | Open Steel/Bronze Tall Helmet | 18/3.2 | **48/0/0/2.5** (Open Tall 简单档) |
| 4 | `ao_battanian_nasal_spangenhelm` | Highland Nasal Spangenhelm | 19/1.8 | **45/0/0/1.8** |
| 5 | `ao_battanian_simple_ridge_helmet` | Highland Ridge Helmet Cloth | 19/1.8 | **45/0/0/1.8** |
| 6 | `battania_simple_spangenhelm` | Highland Cap Helmet | 19/1.1 | **42/0/0/1.5** |
| 7 | `battania_skull_cap` | Highland Skull Cap | 19/1.1 | **42/0/0/1.5** |
| 8 | `ao_battanian_nasal_helmet` | Highland Nasal Helmet | 20/1.8 | **45/0/0/1.8** |
| 9-10 | `AR_battania_helmet_l/m` | Open Steel/Bronze Helmet W. Faceguard | 20/2.3 | **50/40/0/2.3** (Faceguard body 40) |
| 11-12 | `AR_battania_helmet_p/q` | Open Steel/Bronze Ridged Helmet | 20/2.3 | **48/0/0/2.3** |
| 13 | `hmj_simple_nasal_helm` | Highland Nasal Cap | 20/1.1 | **45/0/0/1.5** |
| 14 | `simple_helmet` | Simple Helmet | 20/1.5 | **45/0/0/1.8** |
| 15 | `simple_helmet_scarf` | Simple Helmet With Scarf | 21/1.5 | **45/0/0/1.8** |
| 16 | `ao_battanian_ridge_helmet` | Highland Ridge Helmet Leather | 22/1.8 | **50/0/0/2.0** |
| 17 | `hmj_simple_nasal_helm_hood` | Highland Nasal Cap Over Leather | 22/1.1 | **50/0/0/1.5** |
| 18 | `simple_helmet_headcloth` | Simple Helmet Over Headwrap | 22/1.4 | **48/0/0/1.8** |

**状态**：18 件 🔵 log-only（10-12 号 Faceguard 变体 body 40 属 J 家族逻辑）

---

## Battania · D 家族：Highland Tall Helmet / Kettle Helmet 系（8 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---:|---:|
| 1 | `AR_battania_helmet_c` | Highland Kettle Helmet | 22/3.7 | **65/0/12/3.0** (Kettle Helmet 中档 · Battania 借用 Vlandia Kettle) |
| 2 | `TV_battania_lord_helmet_d` | Highland Plumed Kettle Helmet | 22/3.7 | **65/0/12/3.0** (Plumed 装饰 0) |
| 3 | `AR_battania_helmet_d` | Highland Plumed Kettle Helmet | 28/3.7 | **70/0/12/3.0** |
| 4 | `hmj_imperial_cheek_pteurges_plume` | Plumed Kettle Helmet With Strips | 28/3.7 | **70/0/12/3.0** |
| 5 | `AR_battania_helmet_a` | Highland Tall Helmet | 26/3.7 | **55/0/0/2.5** |
| 6 | `AR_battania_helmet_a2` | Highland Tall Helmet W. Neckguard | 28/3.8 | **60/12/8/2.5** (+Neckguard body/arm) |
| 7 | `AR_battania_helmet_b` | Highland Plumed Tall Helmet | 30/3.7 | **65/0/0/2.7** (Plumed 装饰 0) |
| 8 | `AR_battania_helmet_b2` | Plumed Tall Helmet Over Mail | 38/3.7 | **75/0/20/2.8** (+Mail aventail arm 20 · `roughscale_helmet` 88 参照 sub) |

**状态**：8 件 🔵 log-only

---

## Battania · E 家族：Highland Ridge Helmet 系（13 件）

**vanilla 参照**：`ridged_northernhelm` 94/12/0 · Ridge Helmet 顶级

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_battania_helmet_za` | Iron Ridge Helmet Over Cloth | 24/3.2 | **55/0/0/2.8** |
| 2 | `AR_battania_helmet_zb` | Iron Ridge Helmet W. Leather | 28/3.5 | **60/0/0/2.8** |
| 3 | `ao_battanian_noblemans_ridge_helmet` | Highland Ridge Helmet W. Mail | 29/1.8 | **75/0/20/2.5** (Ridge + Mail) |
| 4-5 | `AR_battania_helmet_r/s` | Highland Steel/Bronze Ridged Helmet | 30/3.5 | **70/0/0/2.8** |
| 6 | `ao_battanian_noblemans_plumed_ridge_helmet` | Plumed Ridge W. Mail | 31/1.8 | **75/0/20/2.5** |
| 7-8 | `AR_battania_helmet_t/u` | Steel/Bronze Ridged Over Mail | 32/3.7 | **82/0/25/3.0** |
| 9 | `AR_battania_helmet_zc` | Iron Ridge Over Stripped Mail | 32/3.7 | **80/0/20/3.0** |
| 10-11 | `AR_battania_lord_helmet_l/m` | Bronze/Standard Ridge W. Feather Crest | 51/3.1 | **90/0/20/3.0** (Noble Ridge + Mail 顶) |
| 12-13 | `AR_battania_lord_helmet_n/o` | Warlord's Iron/Gilded Crested Ridge | 51/3.3 | **94/12/0/3.3** (vanilla `ridged_northernhelm` 94/12/0 直匹配) |

**状态**：13 件 🔵 log-only

---

## Battania · F 家族：Highland Nasal Helmet Over Mail 系（12 件）

**vanilla 参照**：`roughscale_helmet` 88/0/20（Nasal + Roughscale + Mail 顶）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `ao_eorling_riveted_nasal_helmet` | Riveted Nasal W. Mail Aventail | 25/1.8 | **75/0/20/2.5** |
| 2 | `celtic_mail_helm` | Tall Nasalhelm W. Mail | 30/2.9 | **82/0/20/3.0** |
| 3-6 | `TV_pict_helmet_a/b` | Heavy Banded Nasal / Over Hide | 31-35/2.6-2.9 | **75-80/0/12-15/2.8** |
| 7 | `TV_pict_helmet_c` | Plumed Heavy Banded Nasal Over Mail | 38/2.9 | **88/0/20/3.0** (vanilla `roughscale_helmet` 88 直匹配) |
| 8-9 | `TV_pict_helmet_d/e/f` | Domed Nasal Spangen (无/Padding/Mail) | 31-38/2.6-2.9 | **75-88/0/0-20/2.8** |
| 10-11 | `TV_pict_helmet_g/h/i` | Tall Nasal Spangen (无/Cloth/Mail) | 31-38/2.6-2.9 | **75-88/0/0-20/2.8** |
| 12 | `AR_battania_helmet_zd/ze/zf` | Heavy Nasalhelm Cloth/Leather/Mail | 46-51/3.1-3.6 | **75-95/0/20-25/3.2** |

**状态**：12 件 🔵 log-only（部分批量归为家族典型值）

---

## Battania · G 家族：Highland Spangenhelm 系（8 件）

**vanilla 参照**：Cheek Guards 系 61-89 参照（Spangenhelm 无 direct vanilla）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_battania_helmet_g` | Highland Spangenhelm | 36/3.4 | **65/0/0/2.8** |
| 2 | `AR_battania_helmet_h` | Feathered Spangenhelm Over Suede | 42/3.5 | **70/0/0/2.8** (Feathered 装饰 0) |
| 3 | `AR_battania_helmet_i` | Plumed Spangenhelm Over Mail | 50/3.8 | **85/0/20/3.2** (Plumed Mail 顶级) |
| 4 | `TV_pict_helmet_j` | Tall Silvered Nasal Spangen | 31/2.9 | **70/0/0/2.8** (Silvered +2) |
| 5 | `TV_pict_helmet_j2` | Tall Silvered Spangen Cap | 22/2.9 | **65/0/0/2.8** |
| 6 | `TV_pict_helmet_k/l` | Plumed Tall Silvered Spangen | 35-38/2.6-2.9 | **75-88/0/0-20/2.8** |
| 7 | `TV_battania_helmet_g` | Heavy Bronze Spangenhelm | 50/3.5 | **85/0/0/3.0** |
| 8 | `TV_battania_helmet_h` | Plumed Heavy Bronze Spangen | 52/3.6 | **88/0/0/3.0** |
| 9 | `TV_battania_helmet_i` | Feathered Heavy Bronze Spangen | 52/3.6 | **88/0/0/3.0** |

**状态**：9 件 🔵 log-only

---

## Battania · H 家族：Highland Banded Helmet 系（6 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1-2 | `AR_battania_helmet_y/y2` | Banded Helmet Over Leather/Cloth | 32-34/3.5 | **65/0/0/2.8** |
| 3 | `AR_battania_helmet_z` | Banded Helmet Over Mail | 41/2.2 | **85/0/20/3.0** |
| 4 | `TV_pict_helmet_m` | Highland Banded Helmet | 35/3.1 | **68/0/0/2.8** |
| 5 | `TV_pict_helmet_n` | Decorated Banded Helmet | 42/3.1 | **75/0/0/2.8** |
| 6 | `TV_pict_helmet_o` | Crested Decorated Banded Helmet | 48/3.1 | **82/0/0/2.8** |

**状态**：6 件 🔵 log-only

---

## Battania · I 家族：Dryatic Bent Conical Helmet（6 件 · Persian 借用）

**Dryatic 命名说明**：OSA 借用 Persian/Sassanid 尖顶盔风 · 走 Cheek Guards 系参照

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1-2 | `AR_phrygian_helmet_c/c2` | Steel/Brass Bent Conical | 35/3.1 | **65/0/0/2.8** |
| 3-4 | `AR_phrygian_helmet_a/a2` | Steel/Brass Decorated Bent Conical | 42/3.1 | **73/0/0/2.8** |
| 5-6 | `AR_phrygian_helmet_b/b2` | Decorated Crested Bent Conical | 48/3.1 | **82/0/0/2.8** |

**状态**：6 件 🔵 log-only

---

## Battania · J 家族：Faceguard/Guarded Nobleman's Helmet（15 件）

**vanilla 参照**：`battanian_plated_noble_helmet` 95/16/15 · `battanian_noble_helmet_with_feather` 102/12/25

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1-2 | `AR_battania_helmet_j/k` | Steel/Bronze Helmet W. Faceguard | 30/3.5 | **70/40/0/3.0** |
| 3-4 | `AR_battania_helmet_n/o` | Plumed Steel/Bronze W. Faceguard | 30/3.7 | **70/40/0/3.0** (Plumed 0) |
| 5-6 | `AR_battania_helmet_v/v2` | Steel/Bronze Tall W. Faceguard | 30/3.5 | **72/40/0/3.0** |
| 7 | `ao_battanian_guarded_aristocrats_helmet` | Plated Noble Nasalhelm | 42/1.8 | **75/40/0/2.5** |
| 8-9 | `ao_eorling_guarded_noblemans_helmet + b` | Guarded Nobleman's Helmet | 36/3.4 | **85/40/0/3.2** |
| 10-12 | `ao_eorling_heavy_guarded_noblemans_helmet + a/b/c` | Heavy Guarded Nobleman's | 40-42/3.4 | **90/50/0/3.4** |
| 13 | `ao_eorling_guardian_helmet` | Heavy Guarded Helmet | 47/3.4 | **95/50/0/3.4** (vanilla `battanian_plated_noble_helmet` 95 参照) |
| 14 | `AR_goth_helmet_a` | Iron Roughscale Helmet | 30/2.2 | **88/0/20/3.2** (vanilla `roughscale_helmet` 88/0/20 直匹配) |
| 15 | `AR_goth_helmet_b` | Iron Roughscale W. Faceguard | 38/2.2 | **92/40/20/3.2** (+Faceguard body 40) |
| 16 | `AR_goth_helmet_c` | Plumed Iron Roughscale W. Faceguard | 46/2.2 | **92/40/20/3.2** (Plumed 装饰 0) |

**状态**：16 件 🔵 log-only

---

## Battania · K 家族：Cheek Guards Cap 变体（12 件 · TV_）

**vanilla 直匹配**：`battania_earmuff_helmet_*` 系列 61-89

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1-2 | `TV_battania_helmet_l/m` | Feathered Pointed Steel/Bronze Cap Cloth | 32/2.9 | **65/0/0/2.9** (vanilla `battania_earmuff_helmet_a_brnz` 65 参照) |
| 3-4 | `TV_battania_helmet_c/d` | Fringed Decorated Steel/Bronze Helmet | 38/3.1 | **73-79/0/0/3.1** (vanilla `battania_earmuff_helmet_c` 89 / `_c_brnz` 73 参照) |
| 5-6 | `TV_battania_helmet_e/f` | Decorated Steel/Bronze Helmet | 38/3.1 | **74/0/0/3.1** (vanilla `battania_earmuff_helmet_b` 74 参照) |
| 7-8 | `TV_battania_helmet_s/s2` | Pointed Steel/Bronze Helmet | 38/3.2 | **71/0/0/3.2** (vanilla `battania_earmuff_helmet_d_brnz` 71 参照) |
| 9 | `TV_battania_lord_helmet_c` | Crested Decorated Steel Cap W. Cheek Guards | 38/3.1 | **89/0/0/3.1** (vanilla `battania_earmuff_helmet_c` 89 直匹配) |
| 10-11 | `TV_battania_helmet_a/b` | Pointed Steel/Bronze Kettle W. Fringes | 40/3.5 | **79/0/0/3.0** |
| 12-13 | `TV_battania_helmet_p/q` | Ridged Decorated Steel/Bronze Nasalhelm | 41/3.2 | **92/12/0/3.2** (vanilla `battania_earmuff_helmet_d` 92/12/0 直匹配) |
| 14-15 | `TV_battania_helmet_u/u2` | Ridged Decorated Steel/Bronze Helmet | 41/3.2 | **92/12/0/3.2** |

**状态**：15 件 🔵 log-only

---

## Battania · L 家族：Wolfskin/Bearhelmet 部落顶档（3 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `wolfskin_helmet_guard` | Wolfskin Helmet | 40/4.0 | **88/0/0/2.5** (Wolfskin over Steel Cap 顶级部落 · Steel Cap 88 参照) |
| 2 | `wolfhelmet` | Wolfskin Over Highland Helmet | 45/2.0 | **90/0/0/2.5** |
| 3 | `bearhelmet` | Bearskin Over Highland Helmet | 48/4.5 | **92/0/0/2.8** (Bearskin +2 vs Wolfskin) |

**状态**：3 件 🔵 log-only

---

## Battania · M 家族：Highland Crown 系（6 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `battanian_crown` | Highland Crown | 24/3.2 | **15/0/0/0.5** (vanilla `battania_crown` 15 直匹配 · Ceremonial Crown 民用低档 · wt 3.2 荒谬降到 0.5) |
| 2 | `TV_battania_lord_helmet_a` | Iron Highland Crown | 40/2.3 | **66/0/0/2.3** (vanilla `battania_battle_crown` 66 参照) |
| 3 | `TV_battania_lord_helmet_b` | Two Pronged Gold Crown | 40/2.3 | **66/0/0/2.3** (Gilded Battle Crown) |
| 4 | `ao_battanian_crowned_helmet` | Crowned Plated Noble Nasalhelm W. Plume | 45/1.8 | **102/12/25/2.5** (vanilla `battanian_noble_helmet_with_feather` 102/12/25 直匹配) |
| 5-6 | `AR_battania_lord_helmet_a/b` | Plumed/Gilded Plumed Highland Crowned | 52/3.3-3.4 | **121/16/15/3.2** (vanilla `battanian_crowned_helmet` 121/16/15 顶点直匹配) |

**状态**：6 件 🔵 log-only

---

## Battania · N 家族：Highland Lord/Warlord/Noble Helmet 顶档（30 件）

**vanilla 参照**：`battanian_plated_noble_helmet` 95/16/15 · `battanian_noble_helmet_with_feather` 102/12/25 · `battanian_crowned_helmet` 121/16/15

**批量处理**：所有 AR_battania_lord_helmet_c-q + TV_battania_helmet_g-k 顶档 · 30 件基本同档 · 分 3 层：Noble 95 · Warlord 102 · Crowned 顶 115

| # | 命名 pattern | 件数 | **v2 决议 h/b/a/wt** |
|---|---|---:|---|
| N.1 | `AR_battania_lord_helmet_c/d/e/k` Plumed/Decorated Noble Helmet | 4 | **95/16/15/3.1** (vanilla `battanian_plated_noble_helmet` 95 直匹配) |
| N.2 | `AR_battania_lord_helmet_j` Noble Helmet W. Feather Crest | 1 | **102/12/25/3.1** (vanilla `battanian_noble_helmet_with_feather` 直匹配) |
| N.3 | `AR_battania_lord_helmet_f/g` Steel/Bronze Ridged Helmet Noble | 2 | **95/12/0/3.3** (Ridge 顶级 · 走 Warlord 系) |
| N.4 | `AR_battania_lord_helmet_h/i` Steel/Bronze Tall Helmet Noble | 2 | **95/12/0/3.3** |
| N.5 | `AR_battania_lord_helmet_p/q` Noble Heavy Nasalhelm Over Scale | 2 | **102/12/25/4.0** (Scale 复合 · Warlord 系) |
| N.6 | `TV_battania_helmet_j/k` Heavy Bronze Nasalhelm | 2 | **95/0/20/3.6** (Heavy Nasal Mail) |
| N.7 | `TV_battania_helmet_n/o` Horned Steel/Bronze W. Plume | 2 | **95/0/0/3.5** (Horned decorative) |

**状态**：15 件 🔵 log-only（N 家族总数 15，之前估算 30 过高，实际重叠了 K 家族）

---

## Battania · O 家族：Bone Lamellar Shaman 特色（3 件）

**Battania Shaman 独有**：无 vanilla 对应 · 走 Fur Cap/Wolf Head 中档 + Lamellar 加成

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_bone_helmet_a` | Shaman's Bone Lamellar Helmet | 20/1.9 | **55/0/0/1.8** (Bone Lamellar 中低档) |
| 2 | `AR_bone_helmet_b` | Shaman's Feathered Bone Lamellar | 26/2.1 | **60/0/0/2.0** |
| 3 | `AR_bone_helmet_c` | Shaman's Horned Bone Lamellar | 30/2.3 | **65/0/0/2.2** (Horned +5) |

**状态**：3 件 🔵 log-only

---

## Battania HeadArmor 收官统计（2026-09-23）

**总数**：155 件 Battania HeadArmor · **全部审完 · 全部 🔵 log-only**

**关键设计观察**：
- OSA v1 系统性偏低——顶 54，vanilla+RBM 顶 121 → v2 buff ×2-3
- **v1 全 155 件 body/arm 均为 0**（同 Empire/Vlandia 系统漏洞）· v2 补齐仅在高档 Faceguard/Roughscale/Ridge/Warlord 系有 body/arm
- vanilla 直匹配 20+ 件（Cheek Guards 系 · Roughscale · Ridged Northernhelm · Warlord · Noble Feather · Crowned Helmet 顶）
- **Battania 特色 base_type**：Simple 板甲基础填充 34-61 vanilla 空档 · Ridge Helmet + Cheek Guards + Wolf/Bear 部落风
- **无 Full Helm / Cataphract 顶级**——Battania 顶档是"高冠+羽饰"Warlord/Crowned Helmet

**下一步**：进入 Battania Cape 家族

---

# Battania · Cape（2026-09-23）

## 🔒 铁律 · Battania Cape 沿用三部分律

> **顶点**：`battania_warlord_pauldrons` **45**（Battania 顶 · sub Empire 55 / Vlandia 88）
> **Battania 特色**：Leather/Fur/Bear/Wolf 部落装饰主流 · Chainmail 稀少 · **无重装 Plate Cape**
> **arm 上限 ≤ 20**（Warlord Pauldrons Elite · sub Empire/Vlandia 25）

## Battania Cape · 家族分类总览（65 件）

| 组 | 家族 | n | vanilla 锚点 |
|---|---|---:|---|
| **A** | A.1 Shoulder Straps 民用 | 5 | `battania_shoulder_strap` 14 · `shoulder_strap_cloak` 16 |
| **A** | A.2 Cured Leather Shoulders | 5 | `battanian_leather_shoulder_a` 12 |
| **A** | A.3 Wolf/Bear Shoulders | 4 | `wolf_shoulder` 22 · `armored_bearskin` 24 |
| **A** | A.4 Mail Shoulders | 6 | `battanian_chainmail_shoulder_a/b` 27-32 |
| **A** | A.5 Chainmail Shoulders With Cape | 3 | `battanian_chainmail_shoulder_a` 27 |
| **A** | A.6 Scale Shoulders + Cape | 10 | 无 direct · Plate Scale 中档 |
| **A** | A.7 Warlord Pauldrons 顶档 | 6 | `battania_warlord_pauldrons` 45 ⭐ |
| **A** | A.8 Highland Shoulder Cape | 1 | Cloth 民用 |
| **B** | B.1 Medallion 装饰 | 2 | Cloth 极轻 |
| **B** | B.2 Simple/Plaid Cloak | 4 | `battania_cloak` 6 |
| **B** | B.3 Long Cloak / Woolen Cape | 6 | 无 direct |
| **B** | B.4 Wolf Pelt (Group B) | 2 | `wolf_shoulder` 22 参照 |
| **B** | B.5 Furred Cape | 4 | `fur_cloak_a` 20 参照 |
| **B** | B.6 Wolfskin Cloak | 3 | 无 direct |
| **B** | B.7 Long Cape 变体 | 4 | `battania_cloak_b` 14 |

**A(40) + B(25) = 65 件 ✓**

---

## Battania Cape · A 家族：Shoulder/Pauldron 命名 组（Rule 1A · 40 件）

### A.1 · Simple Shoulder Straps 民用（5 件）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_battanian_shoulder_e` | Highland Simple Shoulder Straps | 4/0/1 | **14/6/1.0** (vanilla `battania_shoulder_strap` 14 直匹配) |
| 2 | `AR_battanian_shoulder_f` | Shoulder Straps With Tartan Cape | 6/0/1 | **16/6/1.3** (vanilla `battania_shoulder_strap_cloak` 16 直匹配) |
| 3 | `ao_light_fur_shoulder` | Highland Shoulder Cloak | 4/0/0.8 | **15/6/1.4** (vanilla `woodland_cloak` 15 参照) |
| 4 | `AR_battanian_shoulder_g` | Shoulder Straps With Tartan Cape | 10/0/1 | **16/6/1.3** (同 f) |
| 5 | `AR_battanian_shoulder_h` | Heavy Shoulder Straps With Tartan Cape | 12/0/1 | **20/8/1.5** (Heavy +4 body) |

### A.2 · Cured Leather Shoulders（5 件）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_battanian_shoulder_b` | Highland Leather Shoulders | 8/0/1.4 | **12/6/1.4** (vanilla `battanian_leather_shoulder_a` 12 直匹配) |
| 2 | `AR_battanian_shoulder_c` | Cured Leather Shoulders With Striped Cape | 8/0/1.2 | **15/6/1.5** (+Cape body +3) |
| 3 | `AR_battanian_shoulder_d` | Cured Leather Shoulders With Cape | 8/0/1.2 | **15/6/1.5** |
| 4 | `HMJ_battanian_leather_scarf` | Leather Shoulders With Cloak | 8/0/1.4 | **15/6/1.5** |
| 5 | `AR_empire_shoulders_a` | Cured Leather Shoulders With Scarf | 7/0/1.2 | **12/6/1.2** |

### A.3 · Wolf/Bear Shoulders（4 件）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_wolf_pelt_d` | Brown Wolf Shoulders | 6/2/1.8 | **22/8/2.0** (vanilla `wolf_shoulder` 22 直匹配 + arm 8 Wolf 中档) |
| 2 | `TV_wolf_pelt_g` | Eastern Wolf Shoulders | 6/2/1.8 | **22/8/2.0** |
| 3 | `TV_wolf_pelt_f` | Eastern Heavy Wolf Shoulders | 10/4/2.6 | **26/10/2.5** (Heavy +4 · vanilla `bearskin` 26 参照) |
| 4 | `TV_battania_shoulders_d` | Brown Bear Shoulders | 14/0/2.3 | **24/10/2.5** (vanilla `armored_bearskin` 24 直匹配 + arm 10) |

### A.4 · Mail Shoulders（6 件）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `ao_battanian_mail_shoulders` | Trimmed Mail Shoulders | 7/0/4.5 | **27/10/3.5** (vanilla `battanian_chainmail_shoulder_a` 27 直匹配 · wt 4.5 降到 3.5) |
| 2 | `hmj_battanian_shoulders_a` | Mail Shoulders With Furred Cloak | 9/0/4.5 | **28/10/3.5** |
| 3 | `battanian_mail_scarf` | Mail Shoulders With Cloak | 10/0/4.5 | **28/10/3.5** |
| 4 | `hmj_battanian_shoulders_c` | Mail Shoulders With Bearskin | 10/0/4.5 | **30/10/3.5** |
| 5 | `hmj_battanian_shoulders_d` | Mail Shoulders With Rough Bearskin | 10/0/4.5 | **30/10/3.5** |
| 6 | `hmj_battanian_shoulders_e` | Mail Shoulders With Fur | 12/0/4.5 | **32/10/3.5** (vanilla `battanian_chainmail_shoulder_b` 32 直匹配) |

### A.5 · Chainmail Shoulders With Cape（3 件）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1-3 | `tv_battania_cloak_h/i/j` | Chainmail Shoulders With Cape/Plaid/Striped | 8/2/3.9 | **28/10/3.5** (Chainmail + Cape 中档) |

### A.6 · Scale Shoulders + Cape（10 件）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_battanian_shoulder_a` | Highland Scale Shoulders | 16/0/4.5 | **30/12/3.5** (Scale Shoulders 中档) |
| 2-4 | `tv_battania_cloak_c/d/e` | Scale Shoulders With Cape variants | 16/8/3.9 | **32/12/3.5** (+Cape +2 body) |
| 5-7 | `tv_battania_cloak_r/r2/r3` | Alternating Scale Shoulders variants | 16/8/3.9 | **32/12/3.5** |
| 8-10 | `tv_battania_cloak_s/s2/s3` | Steel Scale Shoulders variants | 16/8/3.9 | **34/12/3.5** (Steel +2) |

### A.7 · Warlord Pauldrons 顶档（6 件）· vanilla 直匹配

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_battania_warlord_pauldrons_b` | Brass Warlord Pauldrons | 20/10/4 | **42/20/4.0** (Brass -3 vs Steel) |
| 2 | `TV_battania_warlord_pauldrons_c` | Steel Warlord Pauldrons | 20/10/4 | **45/20/4.0** (vanilla `battania_warlord_pauldrons` 45 直匹配 · 顶点) |
| 3-4 | `AR_wolf_shoulder_h/h2` | Warlord Pauldrons With Wolf Pelt | 21/12/4 | **45/20/4.0** (+Wolf Pelt 装饰 0) |
| 5-6 | `AR_wolf_shoulder_i/i2` | Bronze Warlord Pauldrons With Wolf Pelt | 21/12/4 | **43/20/4.0** (Bronze -2 vs Steel) |

### A.8 · Highland Shoulder Cape（1 件）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `hmj_battanian_shoulders_b` | Highland Shoulder Cape | 2/0/0.2 | **6/2/0.3** (Cloth Shoulder Cape 轻档) |

**A 组合计**：40 件 · 🔵 log-only

---

## Battania Cape · B 家族：无 Shoulder/Pauldron 命名（Rule 1B · 25 件）

### B.1 · Medallion 装饰（2 件）

| # | id | 游戏名 | v1 b/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `ao_battanian_medallion` | Highland Medallion | 1/0.2 | **3/0/0.2** |
| 2 | `ao_caladogs_medallion` | Highland Noble Medallion | 1/0.2 | **5/0/0.2** (Noble +2) |

### B.2 · Simple/Plaid Cloak（4 件）

| # | id | 游戏名 | v1 b/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `ao_generic_short_cape` | Plain Short Cape | 2/0.2 | **6/0/0.5** (vanilla `a_battania_cloak_a` 6 参照) |
| 2-4 | `TV_battania_cloak_l/l2/l3` | Highland Plaid/Brown/Green Cloak | 2/0.25 | **6/0/0.5** (vanilla `battania_cloak` 6 直匹配) |

### B.3 · Long Cloak / Woolen Cape（6 件）

| # | id | 游戏名 | v1 b/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1-3 | `TV_battania_cloak_p/p2/p3` | Long/Plaid/Striped Cloak | 6/4.5 | **8/0/1.5** (Long +2 vs Simple · wt 4.5→1.5) |
| 4-6 | `TV_battania_cloak_q/q2/q3` | Plain/Plaid/Striped Woolen Cape | 6/4.5 | **8/0/1.5** |

### B.4 · Wolf Pelt (Group B, 2 件)

| # | id | 游戏名 | v1 b/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_wolf_pelt_a` | Brown Wolf Pelt | 7/2.0 | **22/0/2.0** (vanilla `wolf_shoulder` "Wolf Pelt Cape" 22 直匹配 · body-only 因命名无 shoulder/pauldron) |
| 2 | `TV_wolf_pelt_b` | Wolf Pelt | 7/2.0 | **22/0/2.0** |

### B.5 · Furred Cape（4 件）

| # | id | 游戏名 | v1 b/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1-4 | `AR_fur_cape_a/b/c/d` | Furred Long/Plaid/Striped/Hide Cape | 8/4 | **20/0/2.5** (vanilla `fur_cloak_a` 20 参照 · wt 4→2.5) |

### B.6 · Wolfskin Cloak（3 件）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_wolf_head_cloak_a` | Wolfskin Cloak | 8/2/3.6 | **24/0/3.0** (vanilla `armored_bearskin` 24 参照 · arm 追溯清 0) |
| 2 | `AR_wolf_head_cloak_b` | Brown Wolfskin Cloak | 8/2/3.6 | **24/0/3.0** |
| 3 | `TV_wolf_pelt_h` | Eastern Wolfskin Cloak | 8/2/3.6 | **24/0/3.0** |

### B.7 · Long Cape 变体（4 件）

| # | id | 游戏名 | v1 b/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `tv_battania_cloak_a` | Long Cape | 8/4 | **14/0/2.0** (vanilla `battania_cloak_b` 14 直匹配) |
| 2 | `tv_battania_cloak_b` | Long Plaid Cape | 8/4 | **14/0/2.0** |
| 3 | `tv_battania_cloak_f` | Long Striped Cape | 8/4 | **14/0/2.0** |
| 4 | `tv_battania_cloak_g` | Long Hide Cape | 8/4 | **16/0/2.0** (Hide +2) |

**B 组合计**：25 件 · 🔵 log-only

---

## Battania Cape 收官统计（2026-09-23）

**总数**：65 件 Battania Cape · **全部审完 · 全部 🔵 log-only**

**关键观察**：
- OSA v1 顶 21，vanilla+RBM 顶 45 → v2 buff ×1.5-2
- vanilla 直匹配 15+ 件（Shoulder Straps · Leather Shoulder Pieces · Wolf Shoulder · Bearskin · Mail Shoulder Pieces · Warlord Pauldrons 顶）
- **三部分律 Group B 应用 25 件**：追溯清 arm 到 0（v1 部分 arm 2-8 违规）
- **Battania 部落装饰特色**：Wolf/Bear Head/Pelt/Shoulder · Fur Cloak · Chained Fur · Woodland Cape · 主流 Leather · Chainmail 稀少 · Plate 仅 Warlord Pauldrons 顶

**下一步**：进入 Battania BodyArmor / HandArmor / LegArmor / HorseHarness

---

# Battania · BodyArmor（2026-09-23）

## 🔒 铁律 · Battania BodyArmor

> **顶点**：`battania_warlord_armor` **96/50/40/25** ⭐（Scale Warlord Armor · Plate 顶）· body ≤ 96
> **arm 顶 40**（vs Empire 67 / Vlandia 100）· 无 Full Sleeve 传统
> **主流 Chainmail Hauberk**（顶 55-66）· Plate 稀少（5 件）

## Battania BodyArmor · 50 件 · 分家族决议

### A. Cloth Trousers/Tunic 民用（6 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1-3 | `AR_battania_armor_c/d/e` | Highland Cloth/Plaid/Striped Trousers | 1/3/0/0.4 | **0/15/0/0.5** (vanilla `battania_light_armor_d` Kilt 15 参照) |
| 4-5 | `ao_battanian_cloth_tunic_with_kilt` `ao_battanian_woolen_tunic_with_kilt` | Cloth/Woolen Tunic With Kilt | 6/1/1/0.4 | **7/7/5/0.5** (vanilla `battania_civil_a` 7 参照) |
| 6 | `AR_battania_armor_k` | Tartan Tunic with Rolled Cloth | 6/3/2/0.7 | **8/8/6/0.6** (vanilla `battania_dress_a` 8 参照) |

### B. Sleeveless Leather Tunic / Leather Vest / Kilt（4 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_battania_armor_g` | Sleeveless Leather Tunic | 15/7/2/2 | **13/11/5/1.5** (vanilla `battania_civil_b` 13/11/7 参照) |
| 2 | `ao_battanian_leather_tunic_with_kilt` | Leather Tunic With Kilt | 16/7/2/2.1 | **14/12/9/1.8** (vanilla `fur_armor_with_strap` 14 参照) |
| 3 | `AR_battania_armor_h` | Rugged Leather Vest | 16/8/2/0.8 | **14/12/8/1.5** |
| 4 | `ao_battanian_gambeson_with_kilt` | Gambeson Over Tunic With Kilt | 19/8/5/1.5 | **19/19/11/2.0** (vanilla `fur_armor` 19 参照) |

### C. Plated Leather Vest / Rugged Scale Vest（3 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `tv_battania_armor_e` | Plated Leather Vest | 22/7/0/0.8 | **20/22/14/1.5** (vanilla `battania_woodland_outfit` 20 参照) |
| 2 | `ao_battanian_scale_armor` | Rugged Scale Armor Over Tunic | 24/9/6/10.7 | **34/20/0/10.7** (vanilla `scale_armor` 34/20/0 直匹配) |
| 3 | `tv_battania_armor_a` | Rugged Scale Vest | 26/8/2/10.7 | **34/20/0/10.7** |

### D. Mail Vest/Shirt/Hauberk 中档（6 件）

**vanilla 参照**：`battanian_chainmail_armor_b` 36/22/16（Mail Shirt）· `battanian_chainmail_armor_a` 40/25/16（Heavy Mail Vest）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_battania_armor_i` | Rugged Mail Vest | 25/8/2/1 | **36/22/16/8.0** (vanilla `battanian_chainmail_armor_b` 36 直匹配 · wt 1 荒谬抬到 8.0) |
| 2 | `AR_battania_armor_j` | Plated Mail Vest | 26/10/0/1 | **40/25/16/8.5** (vanilla `battanian_chainmail_armor_a` 40 直匹配) |
| 3 | `AR_battania_armor_l` | Mail Shirt With Rolled Cloth | 27/13/8/7.5 | **36/22/16/8.0** |
| 4 | `AR_battania_armor_f` | Rough Mail Hauberk With Fur | 27/14/12/9.5 | **40/25/16/9.5** |
| 5 | `ao_battanian_mail_shirt_with_kilt` | Mail Shirt With Kilt | 27/15/11/7.5 | **36/22/16/8.0** |
| 6 | `ao_battanian_hauberk_with_kilt` | Hauberk With Kilt | 29/16/12/9.5 | **40/25/16/9.5** |

### E. Breastplate Over Tunic/Mail（6 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1-3 | `AR_battania_armor_p/p2/p3` | Brass/Steel/Decorated Breastplate Over Tunic | 32/6/5/10 | **50/38/38/10** (vanilla `western_scale_mail` 50/38/38 参照) |
| 4-5 | `AR_battania_armor_o/o2` | Padded Steel/Decorated Steel Breastplate | 42/8/5/10 | **60/30/15/10** (vanilla `battanian_scale_armor_b` 60 参照) |
| 6 | `tv_battania_armor_f` | Plated Scale Vest | 32/7/0/10.7 | **50/38/38/10.7** |
| 7-9 | `AR_battania_armor_q/q2/q3` | Brass/Steel/Decorated Breastplate Over Mail | 34/12/8/9.5 | **45/27/38/9.5** (vanilla `battania_woodland_chainmail` 45/27/38 参照) |

### F. Highland Savage Scale / Alternating Scale（5 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_battania_armor_a` | Highland Savage Scale Cuirass | 35/8/2/15 | **50/38/38/13** |
| 2-4 | `ao_battanian_alt_scale...`/`bronze_scale.../iron_scale...` | Scale Armor With Kilt variants | 35/10/3/11.4 | **50/38/38/12** |

### G. Scale Shirt With Rolled Cloth（2 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_battania_armor_m` | Brass Scale Shirt With Rolled Cloth | 36/12/8/15.1 | **55/27/33/14** (vanilla `battania_mercenary_armor` 55/27/33 参照) |
| 2 | `AR_battania_armor_m2` | Steel Scale Shirt With Rolled Cloth | 36/12/8/15.1 | **57/27/33/14** (Steel +2) |

### H. Warlord Cuirass/Armor 顶档（6 件）

**vanilla 参照**：`battania_warlord_armor` 96/50/40（Scale Warlord Armor 顶）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1-2 | `tv_battania_warlord_armor_b/c` | Heavy Brass/Brass Warlord Cuirass | 36/14/14/25 | **80/45/35/22** (Warlord 中高档 · sub 顶) |
| 3 | `tv_battania_warlord_armor_a` | Bronze Warlord Armor | 38/18/16/19 | **85/48/38/20** |
| 4-5 | `tv_battania_warlord_armor_b2/c2` | Heavy Steel/Steel Warlord Cuirass | 38/18/16/25 | **92/48/40/22** (Steel 顶级) |
| 6 | `AR_battania_armor_b` | Decorated Savage Scale Over Mail | 50/14/12/20.1 | **96/50/40/22** (vanilla `battania_warlord_armor` 96/50/40 顶点直匹配) |

### I. Decorated Scale Over Kilted Mail（3 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1-3 | `ao_battanian_decorated_scale_armor + b/c` | Decorated Bronze/Iron/Alternating Scale Over Kilted Mail | 44/15/16/22.4 | **66/33/33/16** (vanilla `battania_noble_armor` 66/33/33 直匹配) |

### J. Ranger Mail / Kilt Over Plated Ranger（3 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `tv_battania_armor_b` | Decorated Ranger Mail | 36/15/10/12 | **47/15/18/12** (vanilla `ranger_mail` 47/15/18 直匹配) |
| 2 | `tv_battania_armor_c` | Kilt Over Plated Ranger Mail | 44/26/14/12.2 | **48/48/12/10.8** (vanilla `kilt_over_plated_leather` 48/48/12 参照) |
| 3 | `ao_eorling_plates_over_mail` | Western Iron Scale Over Mail | 40/20/25/18 | **50/38/38/14** (vanilla `western_scale_mail` 50/38/38 直匹配) |

### K. Mailed Scale Shirt Over Cloth（2 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_battania_armor_n` | Mailed Brass Scale Shirt | 42/14/16/22.4 | **55/27/33/16** (vanilla `battania_mercenary_armor` 55 参照) |
| 2 | `AR_battania_armor_n2` | Mailed Steel Scale Shirt | 42/14/16/22.4 | **57/27/33/16** |

### L. Savage Scale Over Mail Shirt/Mail（3 件 · 顶档 sub Warlord）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_battania_armor_a2` | Savage Scale Over Mail Shirt | 48/8/12/20.1 | **70/20/20/14** (vanilla `battania_brass_plate_armor` 70/20/20 直匹配) |
| 2 | `AR_battania_armor_b2` | Savage Scale Over Mail | 48/14/12/20.1 | **79/35/25/16** (vanilla `battanian_scale_armor_a` 79/35/25 直匹配) |

**Battania BodyArmor 合计**：50 件 🔵 log-only

---

# Battania · HandArmor（2026-09-23）

## Battania HandArmor · 10 件 · vanilla 直匹配

| # | id | 游戏名 | v1 arm/wt | **v2 决议 arm/wt** |
|---|---|---|---|---|
| 1 | `TV_battania_gloves_a` | Leather Gauntlets | 15/1.5 | **30/1.0** (vanilla `highland_gloves` 30 直匹配) |
| 2 | `TV_battania_gloves_d` | Noble Vambraces | 16/0.6 | **39/0.6** (vanilla `battania_noble_bracers` Fian Bracers 39 直匹配) |
| 3 | `AR_pict_glove_a` | Mail Mitten | 18/1.4 | **30/1.2** (Mail Mitten Battania 中档) |
| 4 | `AR_brass_scale_gloves` | Scale Gauntlets | 20/1.5 | **35/1.3** (Scale 中高档) |
| 5 | `AR_pict_glove_b` | Plated Mail Mitten | 20/1.4 | **35/1.3** |
| 6-7 | `TV_battania_gloves_b/c` | Brass/Iron Gauntlets | 20/1.5 | **30-32/1.2** (Brass 30 / Iron 32) |
| 8-10 | `tv/TV_battania_warlord_bracers/_b/_c` | Bronze/Brass/Steel Warlord Bracers | 25/1.5 | **50/1.5** (vanilla `battania_warlord_bracers` 50 直匹配 · 顶点 · 3 件同档) |

**Battania HandArmor 合计**：10 件 🔵 log-only

---

# Battania · LegArmor（2026-09-23）

## Battania LegArmor · 27 件 · vanilla 直匹配

**vanilla 参照**：`turndown_leather_boots` 25 · `battania_fur_boots` 35 · `battania_warlord_boots` 46

| # | id | 游戏名 | v1 leg/wt | **v2 决议 leg/wt** |
|---|---|---|---|---|
| 1-3 | `ao_leather_shoes` `simple_shoes` `TV_battania_boots_w` | Blackened/Simple/Simple Boots | 2/0.2-0.8 | **6/0.4** (Cloth 极轻民用) |
| 4-11 | `TV_battania_boots_a/b/c/d/s/t/u/v` | Highland Shoes/Boots With Plaid Legwraps 8 variants | 4/0.8 | **12/0.7** (Cloth Legwrap 中低档) |
| 12-15 | `TV_battania_boots_g/h/i/j` | Leather Greave Boots With Legwraps 4 variants | 14/1.8 | **25/1.0** (vanilla `turndown_leather_boots` 25 直匹配) |
| 16 | `DZ_battania_boots_a` | Chainmail Boots | 20/1.8 | **30/1.5** (Chainmail Boots 中档) |
| 17-20 | `TV_battania_boots_k/l/m/n` | Brass Greave Boots With Legwraps 4 variants | 20/1.8 | **35/1.5** (vanilla `battania_fur_boots` 35 参照 · Brass Greave 中高) |
| 21-24 | `TV_battania_boots_o/p/q/r` | Iron Greave Boots With Legwraps 4 variants | 20/1.8 | **35/1.5** (Iron 同 Brass 中高) |
| 25-27 | `tv_battania_warlord_boots + _b/_c` | Bronze/Brass/Steel Warlord Boots | 26/2.6 | **46/2.6** (vanilla `battania_warlord_boots` 46 直匹配 · 顶点 · 3 件同档) |

**Battania LegArmor 合计**：27 件 🔵 log-only

---

# Battania · HorseHarness（2026-09-23）

## Battania HorseHarness · 0 OSA 件

**OSA 无 Battania HorseHarness 物品**——vanilla+RBM 4 件保持原状，无 v2 平衡工作。

---

## Battania 全 6 类收官统计（2026-09-23）

**总数**：**312 件 Battania 决议归档**（HeadArmor 155 · Cape 65 · BodyArmor 50 · HandArmor 10 · LegArmor 27 · HorseHarness 0）· 全 🔵 log-only

**帝国 + Vlandia + Battania 累计**：378 + 312 + 312 = **1002 件决议归档 · 1000+ 里程碑**

**下一步**：进入 **Sturgia 文化**（北欧维京画像 · Rus/Nordic Norsemen · 双手大斧 + 圆盾 + Chainmail Hauberk）

---

# Sturgia 文化 · HeadArmor（2026-09-23）

## 🔒 铁律 · Sturgia HeadArmor

> **顶点**：`sturgian_lord_helmet_c` **150/95/50** ⭐（Plated Warlord Helmet · 跨文化最高 h）
> **Sturgia 特色**：Goggled Helmet（护目盔 · Vendel 风）+ Spangenhelm + Warlord Helmet + Battle Crown · 维京 Nordic 传统
> **命名基型**：Spangenhelm · Nasal Helmet · Vendel Cap · Goggled Helm · Boar Helm · Warlord Helmet · Battle Crown

## OSA Sturgia HeadArmor · 家族分类总览（105 件）

| 家族 | n | head 目标 | vanilla 锚点 |
|---|---:|---|---|
| A. Cloth/民用 Hat | 3 | 6-8 | `womens_headwrap_b` 6 |
| B. Spangenhelm Cap 基础 无 aventail | 10 | 45-65 | `spangenhelm_with_padded_cloth` 61 |
| C. Nasalhelm With Fur/Hat 系 | 10 | 50-65 | `nasal_helmet` 50 |
| D. Northern Cavalry Helmet 系 | 7 | 55-100 | `sturgia_heavy_cavalary_helmet` 120 |
| E. Nasal Helmet + Leather/Mail aventail | 9 | 63-90 | `nasal_helmet_with_leather/mail` 63-65 |
| F. Boar / Guarded / Pointed Nasal Helmet | 12 | 65-95 | Nasal Helmet 系 |
| G. Vendel Cap / Goggled Helm / Lendman 系 | 12 | 86-119 | `nordic_helmet` 86 · `lendman_helmet_over_mail` 119 |
| H. Blackened Steel / Gilded Steel Helm | 10 | 80-115 | Warlord 系 113-116 |
| I. Feathered/Plumed Closed Nasal | 4 | 95-107 | `sturgian_helmet_b_close` 107 |
| J. Ulfhednar / Berserker / Battle Crown | 4 | 90-125 | `sturgian_battle_crown` 125 |
| K. Sturgia Lord/Noble Helmet 顶档 | 6 | 115-150 | `sturgian_lord_helmet_c` 150 顶 |
| L. Faceguard / Goggled Nasal (Nord 系) | 3 | 89-114 | `northern_goggled_helmet` 89 |
| M. Domed / Vaegir Helmet 系 | 8 | 60-107 | `sturgian_helmet_b_close` 107 |
| N. Closed Spangenhelm / Nord Closed 顶 | 7 | 118-133 | `sturgian_helmet_closed` 118 · `decorated_goggled_helmet` 133 |

**合计约 105 件（家族边界略微重叠）**

---

## Sturgia · A 家族：Cloth/民用 Hat（3 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/wt** |
|---|---|---|---|---|
| 1 | `AR_sturgia_womens_hat_a` | Northern Women's Hat | 6/0.4 | **6/0.3** (vanilla `womens_headwrap_b` 6 直匹配) |
| 2 | `sloven_hat` | Sloven Hat | 8/0.4 | **10/0.4** |
| 3 | `sloven_hat_a` | Rich Sloven Hat | 8/0.5 | **12/0.5** (Rich +2) |

**状态**：3 件 🔵 log-only

---

## Sturgia · B 家族：Spangenhelm Cap 基础（10 件）

**vanilla 参照**：`spangenhelm_with_padded_cloth` 61/3/15 · `spangenhelm_with_leather` 67/0/20

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_sturgia_helmet_f` | Spangenhelm Cap | 18/3.2 | **50/0/0/1.5** (Spangen 无 aventail 中档) |
| 2-3 | `AR_sturgia_helmet_g/g2` | Furred/Brown Furred Spangenhelm Cap | 19/3.2 | **50/0/0/1.5** (Furred 装饰 0) |
| 4-5 | `AR_sturgia_helmet_h/h2` | Furred Spangenhelm w. Padded Cloth (2 变体) | 21/1.9 | **61/3/15/1.5** (vanilla `spangenhelm_with_padded_cloth` 直匹配) |
| 6-7 | `AR_sturgia_helmet_i/i2` | Furred Spangenhelm w. Leather (2 变体) | 22/1.2 | **67/0/20/1.5** (vanilla `spangenhelm_with_leather` 直匹配) |
| 8 | `sloven_helmet` | Eastern Conical Helmet | 22/0.4 | **50/0/0/1.5** (Conical base) |
| 9 | `AR_sturgia_helmet_base_b` | Northern Light Cavalry Helmet | 20/2.6 | **50/0/0/2.0** |
| 10 | `AR_sturgia_helmet_base` | Northern Cavalry Helmet | 22/2.6 | **55/0/0/2.0** |

**状态**：10 件 🔵 log-only

---

## Sturgia · C 家族：Nasalhelm With Fur/Hat（10 件）

**vanilla 参照**：`nasal_helmet` 50/12/0 · `nasal_helmet_with_leather` 63/12/25

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1-2 | `AR_vaegir_helmet_b/b2` | Varyag/Northern Nasalhelm With Fur | 20/1.8 | **50/12/0/1.8** (vanilla `nasal_helmet` 直匹配) |
| 3 | `sturgian_nasal_helmet_fur` | Northern Nasalhelm with Hat | 20/1.8 | **50/12/0/1.8** |
| 4 | `TV_nord_helmet_t` | Nordic Ridged Cap Over Cloth | 20/2.2 | **55/0/0/2.0** |
| 5 | `AR_sturgia_helmet_k` | Northern Cap Helmet Over Leather | 23/1.9 | **63/12/25/1.8** (vanilla `nasal_helmet_with_leather` 63 参照) |
| 6 | `AR_sturgia_helmet_a` | Northern Nasalhelm Over Cloth | 26/1.4 | **61/3/15/1.5** (vanilla `spangenhelm_with_padded_cloth` 参照) |
| 7 | `AR_sturgia_helmet_d` | Northern Feathered Nasal Helmet | 27/2.9 | **65/12/20/1.8** (vanilla `nasal_helmet_with_mail` 参照 · Feathered 装饰 0) |
| 8 | `AR_sturgia_helmet_b` | Northern Plumed Heavy Cavalry Helmet | 27/2.9 | **75/12/17/2.5** (Heavy Cavalry 中高档 · vanilla `nasalhelm_over_mail` 参照) |
| 9 | `hmj_forpolkka_sturgia_gopniki_helm` | Northern Helmet With Fur Hat | 31/1.9 | **65/12/25/1.8** |
| 10 | `TV_nord_helmet_t2` | Nordic Ridged Cap Over Mail | 31/2.9 | **75/12/17/2.5** (Ridged Over Mail) |

**状态**：10 件 🔵 log-only

---

## Sturgia · D 家族：Northern Cavalry Helmet（7 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_sturgia_helmet_l` | Northern Helmet With Leather | 30/1.8 | **65/12/0/1.8** (vanilla `nasalhelm_over_leather` 65 直匹配) |
| 2 | `AR_sturgia_helmet_l2` | Northern Helmet With Mail | 32/1.8 | **75/12/17/1.8** (vanilla `nasalhelm_over_mail` 75 直匹配) |
| 3 | `AR_sturgia_helmet_m` | Northern Pointed Helmet With Feather | 34/4.5 | **85/12/17/3.5** (Pointed + Mail 高档) |
| 4 | `mailed_cavalry_helm` | Northern Closed Cavalry Helmet | 40/3.0 | **86/40/40/3.5** (vanilla `nordic_helmet` 86/40/40 直匹配 · Closed Cavalry = Nordic Helmet) |
| 5 | `AR_sturgia_helmet_c` | Northern Plumed Closed Cavalry Helmet | 48/3.4 | **104/64/40/3.8** (vanilla `closed_goggled_helmet` 104 参照 · Plumed 装饰 0) |
| 6-7 | Northern Cavalry / Light Cavalry (see B) | - | - | (Already in B family) |

**状态**：5 件 🔵 log-only

---

## Sturgia · E 家族：Nasal Helmet + Leather/Mail aventail（9 件）

**vanilla 参照**：`nasal_helmet_with_leather` 63/12/25 · `nasal_helmet_with_mail` 65/12/20

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_nord_helmet_i` | Nordic Vendel Nasalhelm Over Leather | 23/3.1 | **63/12/25/2.5** (vanilla direct) |
| 2 | `ao_sturgian_spangenhelm_with_leather` | Northern Spangenhelm With Leather | 27/1.8 | **67/0/20/1.5** (vanilla `spangenhelm_with_leather` 直匹配) |
| 3-4 | `ao_eorling_guarded_nasal_helmet_with_leather` `ao_sturgian_helmet_with_leather` | Guarded/Standard Nasal With Leather | 30/1.8-3.4 | **63/12/25/2.0** |
| 5 | `AR_sturgia_helmet_e` | Spangenhelm Over Mail | 32/3.7 | **75/12/17/2.5** |
| 6-7 | `AR_sturgia_helmet_j/j2` | Furred/Brown Furred Spangenhelm Over Mail | 32/3.7 | **75/12/17/2.5** |
| 8 | `TV_nord_helmet_j` | Nordic Vendel Nasalhelm Over Open Mail | 33/3.4 | **80/12/17/3.0** (Open Mail sub Closed) |
| 9 | `ao_eorling_guarded_nasal_helmet_with_mail` | Guarded Nasal With Mail | 36/3.4 | **85/12/17/3.0** |

**状态**：9 件 🔵 log-only

---

## Sturgia · F 家族：Boar / Guarded / Pointed Nasal Helmet（12 件）

**vanilla 参照**：Nasal Helmet 系 63-89

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1-4 | `TV_nord_helmet_u/u2/u3` `v/v2/v3` | Pointed/Boar Nasalhelm variants (无/Leather/Mail 3 档) | 31-38/2.6-2.9 | **60-88/12/17-20/2.8** (无 aventail 60 · Leather 75 · Mail 88) |
| 5-8 | `TV_nord_helmet_w/w2/w3` | Heavy Guarded Nasal variants | 31-38/2.6-2.9 | **65-89/12/20/2.8** |
| 9 | `AR_vaegir_helmet_e` | Northern Spiked Helmet Over Leather | 42/4.0 | **80/12/17/3.5** |
| 10 | `AR_vaegir_helmet_f` | Northern Fur Spiked Helmet Over Leather | 42/4.0 | **80/12/17/3.5** |
| 11 | `AR_vaegir_helmet_g` | Northern Spiked Helmet With Decorated Facemask | 51/4.5 | **95/50/20/3.8** (Facemask body 50) |
| 12 | `battanian_decorated_spangenhelm` | Decorated Nasalhelm With Mail | 41/3.6 | **89/24/20/3.0** (vanilla `northern_goggled_helmet` 89 参照) |
| 13-14 | `TV_nord_helmet_r` `TV_nord_helmet_u3` | Nasal Helmet W. Faceguard Over Mail (variants) | 42/2.9 | **95/50/20/3.0** (+Faceguard body 50) |

**状态**：14 件 🔵 log-only

---

## Sturgia · G 家族：Vendel Cap / Goggled Helm / Lendman 系（12 件）

**vanilla 参照**：`nordic_helmet` 86/40/40（Closed Mail Goggled）· `lendman_helmet_over_mail` 119/24/20 · `northern_warlord_helmet` 113/36/12

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_nord_helmet_a` | Nordic Vendel Cap | 40/3.1 | **85/12/0/3.0** (Vendel Cap base) |
| 2 | `TV_nord_helmet_b` | Vendel Cap Over Leather | 46/3.1 | **95/12/17/3.0** |
| 3 | `TV_nord_helmet_c` | Vendel Cap Over Open Mail | 50/3.4 | **105/24/20/3.4** |
| 4 | `TV_nord_helmet_d` | Vendel Cap Over Closed Mail | 51/4.4 | **113/36/12/3.8** (vanilla `northern_warlord_helmet` 113 参照) |
| 5 | `TV_nord_helmet_e` | Vendel Goggled Helm | 40/3.1 | **86/40/40/3.5** (vanilla `nordic_helmet` 86/40/40 直匹配) |
| 6 | `TV_nord_helmet_f` | Vendel Goggled Helm Over Leather | 46/3.1 | **95/40/40/3.5** |
| 7 | `TV_nord_helmet_g` | Vendel Goggled Helm Over Open Mail | 50/3.4 | **104/64/40/3.8** (vanilla `closed_goggled_helmet` 104 参照) |
| 8 | `TV_nord_helmet_h` | Vendel Goggled Helm Over Closed Mail | 51/4.4 | **114/74/40/4.2** (vanilla `goggled_helmet_over_full_mail` 114 参照) |
| 9 | `TV_nord_helmet_m` | Lendman's Cap | 40/3.1 | **89/24/20/3.0** (vanilla `northern_goggled_helmet` 89 参照) |
| 10 | `TV_nord_helmet_n` | Lendman's Cap Over Leather | 46/3.1 | **106/24/20/3.2** (vanilla `goggled_helmet_over_mail` 106 参照) |
| 11 | `TV_nord_helmet_o` | Lendman's Cap Over Open Mail | 50/3.4 | **115/36/20/3.6** (vanilla `sturgian_lord_helmet_b` 115 参照) |
| 12 | `TV_nord_helmet_p` | Lendman's Cap Over Closed Mail | 51/4.4 | **119/24/20/4.0** (vanilla `lendman_helmet_over_mail` 119 直匹配) |

**状态**：12 件 🔵 log-only

---

## Sturgia · H 家族：Blackened Steel / Gilded Steel Helm（10 件）

**vanilla 参照**：`northern_warlord_helmet` 113/36/12 · `sturgian_lord_helmet_a` 116/24/20

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_sturgia_helmet_i` | Blackened Steel Helmet | 31/2.9 | **83/12/0/2.8** (vanilla `sturgian_helmet_base` 83 参照) |
| 2 | `TV_sturgia_helmet_j` | Steel Helmet | 31/2.9 | **85/12/17/2.8** (vanilla `sturgian_helmet_open` 85 参照) |
| 3 | `TV_sturgia_helmet_j2` | Gilded Helmet | 31/2.9 | **87/12/17/2.8** (Gilded +2) |
| 4 | `TV_sturgia_helmet_k` | Gilded Spangenhelm | 34/2.9 | **95/12/17/2.8** |
| 5 | `TV_sturgia_helmet_k2` | Mailed Gilded Spangenhelm | 44/2.9 | **98/14/0/2.8** (vanilla `sturgian_helmet_b_open` 98 参照) |
| 6 | `TV_sturgia_helmet_c` | Plumed Open Helmet | 49/3.2 | **107/82/30/3.0** (vanilla `sturgian_helmet_b_close` 107 参照) |
| 7 | `TV_sturgia_helmet_e` | Blackened Steel Open Helmet | 49/3.15 | **107/82/30/3.0** |
| 8 | `TV_sturgia_helmet_g` | Gilded Steel Nasal Helmet | 49/3.15 | **113/36/12/3.2** |
| 9 | `TV_sturgia_helmet_h` | Gilded Steel Helmet | 49/3.15 | **113/36/20/3.2** |
| 10 | `TV_sturgia_helmet_f` | Northern Closed Pointed Helmet | 51/3.8 | **118/82/30/3.5** (vanilla `sturgian_helmet_closed` 118 直匹配) |

**状态**：10 件 🔵 log-only

---

## Sturgia · I 家族：Feathered/Plumed Closed Nasal（4 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `ao_sturgian_feathered_helmet` | Feathered Nasal Helmet | 49/1.8 | **83/12/0/2.0** |
| 2 | `ao_sturgian_feathered_helmet_with_mail` | Feathered Nasal With Full Mail | 49/1.8 | **95/12/17/2.5** |
| 3 | `ao_sturgian_feathered_helmet_closed` | Feathered Closed Nasal | 53/1.8 | **107/82/30/2.5** (vanilla `sturgian_helmet_b_close` 107 参照) |
| 4 | `ao_sturgian_feathered_helmet_with_closed_mail` | Feathered Nasal With Closed Mail | 53/1.8 | **118/82/30/2.8** (vanilla `sturgian_helmet_closed` 118 参照) |

**状态**：4 件 🔵 log-only

---

## Sturgia · J 家族：Ulfhednar / Berserker / Battle Crown 特色（4 件）

**Ulfhednar 命名**：狼皮狂战士 · Berserker 无甲狂战 · Battle Crown 战王冠

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `ulfhednar_helm_b` | Ulfhednar's Helmet | 32/4.8 | **89/24/20/3.5** (Ulfhednar 中高档 · vanilla `northern_goggled_helmet` 89 参照) |
| 2 | `ulfhednar_helm` | Ulfhednar's Goggled Helmet | 48/4.8 | **107/82/30/3.5** (Goggled 顶) |
| 3 | `berserkr_helm_a` | Berserker's Helm | 48/4.8 | **95/24/20/3.5** (Berserker 中高) |
| 4 | `TV_sturgia_lord_helmet_a` | Northern Blackened Steel Battle Crown | 47/2.2 | **125/12/0/2.2** (vanilla `sturgian_battle_crown` 125 直匹配) |

**状态**：4 件 🔵 log-only

---

## Sturgia · K 家族：Sturgia Lord/Noble Helmet 顶档（6 件）

**vanilla 参照**：`sturgian_lord_helmet_b` 115/36/20 · `sturgian_lord_helmet_a` 116/24/20 · `sturgian_lord_helmet_c` **150/95/50** 顶

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_sturgia_lord_helmet_b` | Noble Mailed Steel Helmet | 45/3.15 | **113/36/12/3.2** |
| 2 | `TV_sturgia_lord_helmet_c` | Noble Mailed Gilded Helmet | 45/3.15 | **115/36/20/3.2** (vanilla `sturgian_lord_helmet_b` 直匹配) |
| 3 | `TV_sturgia_lord_helmet_d` | Mailed Gilded Spangenhelm With Feathers | 47/3.15 | **116/24/20/3.5** (vanilla `sturgian_lord_helmet_a` 直匹配) |
| 4 | `TV_sturgia_lord_helmet_e` | Mailed Gilded Spangenhelm With Plume | 47/3.15 | **116/24/20/3.5** |
| 5 | `TV_sturgia_lord_helmet_b2` | Noble Mailed Steel Nasal Helmet | 48/3.15 | **119/24/20/3.5** (vanilla `lendman_helmet_over_mail` 119 参照) |
| 6 | `TV_sturgia_lord_helmet_c2` | Noble Mailed Gilded Nasal Helmet | 48/3.15 | **120/12/20/3.5** (vanilla `sturgia_heavy_cavalary_helmet` 120 参照) |

**状态**：6 件 🔵 log-only

---

## Sturgia · L 家族：Faceguard / Goggled Nasal（3 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_nord_helmet_k` | Vendel Nasalhelm With Faceguard | 47/3.6 | **95/50/20/3.5** (Faceguard body 50) |
| 2 | `TV_nord_helmet_l` | Vendel Nasalhelm With Goggled Faceguard | 47/3.6 | **107/82/30/3.5** (Goggled Faceguard sub Closed Goggled 107) |
| 3 | `TV_nord_helmet_s` | Nordic Goggled Nasal Helmet W. Faceguard Over Mail | 48/2.9 | **114/74/40/3.5** (vanilla `goggled_helmet_over_full_mail` 114 参照) |

**状态**：3 件 🔵 log-only

---

## Sturgia · M 家族：Domed / Vaegir Helmet 系（8 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `DZ_sturgia_helmet_b` | Domed Helmet Over Padded Cloth | 36/3.2 | **65/12/20/2.8** |
| 2 | `DZ_sturgia_helmet_c` | Domed Helmet Over Padded Mail | 46/3.5 | **85/12/17/3.2** |
| 3 | `DZ_sturgia_helmet_a` | Plumed Visored Cap Over Closed Mail | 38/4.1 | **107/82/30/3.5** (Visored 顶级 sub Closed Goggled) |
| 4 | `DZ_sturgia_helmet_d` | Feathered Domed Helmet Over Mail | 52/3.2 | **114/74/40/3.5** |
| 5 | `vaegir_helmet_open` | Varyag Open Helmet | 37/2.0 | **83/12/0/2.5** (Varyag = 罗斯人历史名 · vanilla `sturgian_helmet_base` 83 参照) |
| 6 | `vaegir_helmet_closed` | Varyag Closed Helmet | 42/2.2 | **107/82/30/2.5** (Closed 顶级) |
| 7 | `ao_sturgian_spiked_nasal_helmet` | Simple Nasal Helmet With Full Mail | 38/1.8 | **85/12/17/1.8** (vanilla `sturgian_helmet_open` 85 参照) |
| 8 | `tv_vlandia_helmet_f` | Northern Nasal Helmet With Closed Mail | 51/4.0 | **114/74/40/3.5** |

**状态**：8 件 🔵 log-only

---

## Sturgia · N 家族：Closed Spangenhelm / Nord Closed 顶（3 件）

| # | id | 游戏名 | v1 h/wt | **v2 决议 h/b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_sturgia_helmet_a` | Closed Spangenhelm | 54/3.8 | **127/84/45/3.8** (vanilla `lendman_helmet_over_full_mail` 127 参照) |
| 2 | `TV_sturgia_helmet_b` | Tailed Closed Spangenhelm | 54/3.8 | **127/84/45/3.8** |
| 3 | `TV_sturgia_helmet_d` | Blackened Steel Closed Helmet | 53/3.8 | **125/12/0/3.5** (vanilla `sturgian_battle_crown` 125 参照) |
| 4 | `TV_sturgia_helmet_g2` | Gilded Steel Closed Nasal Helmet | 53/3.8 | **127/84/45/3.8** |

**状态**：4 件 🔵 log-only

---

## Sturgia HeadArmor 收官统计（2026-09-23）

**总数**：105 件 Sturgia HeadArmor · **全部审完 · 全部 🔵 log-only**

**关键设计观察**：
- OSA v1 顶 54，vanilla+RBM 顶 150 → v2 buff ×2-3
- **vanilla 直匹配 25+ 件**（Nasal Helmet · Spangenhelm · Goggled Helmet · Warlord · Battle Crown · Nordic Vendel Cap 顶级 aventail 系）
- **Sturgia 特色**：Goggled Helmet body 40-82（第一个 body 高的 base_type · Vendel 风格）· Warlord Helmet 113-150 顶点
- **命名跨文化**：ulfhednar/berserker（斯堪的纳维亚原型）· vaegir/varyag（罗斯人）· vendel（瑞典）· lendman（挪威贵族）—— OSA 借用 Nordic 历史文化命名

**下一步**：进入 Sturgia Cape

---

# Sturgia · Cape（2026-09-23）

## 🔒 铁律 · Sturgia Cape

> **顶点**：`brass_lamellar_shoulder_white` **40**（Lamellar Pauldrons · 跨文化最低顶）
> **arm 上限 ≤ 20**（Lamellar Pauldrons Elite · sub Empire/Vlandia 25）
> **三部分律沿用**

## Sturgia Cape · 34 件

### A 组 · 有 shoulder/pauldron 命名（25 件）

#### A.1 · Small Lamellar Shoulders（3 件 · 轻档）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_sturgia_shoulders_c` | Nordic Small Lamellar Shoulders | 0/5/1.6 | **18/8/1.6** |
| 2 | `TV_sturgia_shoulders_f` | Small Steel Lamellar Shoulders | 0/5/1.6 | **20/8/1.6** (Steel +2) |
| 3 | `TV_sturgia_shoulders_k` | Small Brass Lamellar Shoulders | 0/5/1.6 | **18/8/1.6** |

#### A.2 · Standard Lamellar Shoulders（3 件 · 中档）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_sturgia_shoulders_b` | Nordic Lamellar Shoulders | 0/8/3 | **25/12/2.5** (vanilla `brass_scale_shoulders` 25 参照) |
| 2 | `TV_sturgia_shoulders_e` | Steel Lamellar Shoulders | 0/8/3 | **27/12/2.5** (Steel +2) |
| 3 | `TV_sturgia_shoulders_j` | Brass Lamellar Shoulders | 0/8/3 | **25/12/2.5** |

#### A.3 · Steel Lamellar Pauldrons 顶档（1 件）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_sturgia_shoulders_g` | Steel Lamellar Pauldrons | 0/12/2.2 | **40/20/2.5** (vanilla `brass_lamellar_shoulder_white` 40 顶 直匹配) |

#### A.4 · Leather Scale / Leather Shoulders（2 件）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_sturgia_shoulders_a` | Leather Scale Shoulders | 8/4/1.4 | **14/6/1.5** (vanilla `stitched_leather_shoulders` 14 直匹配) |
| 2 | `AR_sturgia_shoulders_e` | Leather Shoulders | 8/0/1.4 | **12/6/1.4** |

#### A.5 · Brass/Iron Lamellar Pauldrons（3 件）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `DZ_sturgia_shoulder_b` | Brass Lamellar Pauldrons | 8/0/4.1 | **25/12/3.5** |
| 2 | `DZ_sturgia_shoulder_c` | Iron Lamellar Pauldrons | 8/0/4.1 | **27/12/3.5** (Iron +2) |
| 3 | `TV_sturgia_shoulders_h` | Brass Lamellar Pauldrons | 12/0/4.1 | **38/18/3.5** (顶级 Pauldrons sub 40) |

#### A.6 · Chainmail Shoulders（1 件）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_sturgia_shoulders_e2` | Chainmail Shoulders | 12/0/1.4 | **30/12/2.5** (vanilla `mail_shoulders` 30 直匹配) |

#### A.7 · Brass/Iron Lamellar Shoulders (+ Cape)（4 件）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_sturgia_shoulders_a2` | Brass Lamellar Shoulders | 16/8/1.4 | **30/12/2.5** |
| 2 | `AR_sturgia_shoulders_a3` | Iron Lamellar Shoulders | 16/8/1.4 | **32/12/2.5** (Iron +2) |
| 3 | `AR_sturgia_shoulders_a4` | Brass Lamellar Shoulders With Cape | 16/8/1.4 | **32/12/3.0** |
| 4 | `AR_sturgia_shoulders_a5` | Iron Lamellar Shoulders With Cape | 16/8/1.4 | **34/12/3.0** |

#### A.8 · Large Lamellar Shoulders（3 件）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `TV_sturgia_shoulders_a` | Nordic Large Lamellar Shoulders | 16/6/3.5 | **32/15/3.0** |
| 2 | `TV_sturgia_shoulders_d` | Large Steel Lamellar Shoulders | 16/6/3.5 | **34/15/3.0** (Steel +2) |
| 3 | `TV_sturgia_shoulders_i` | Large Brass Lamellar Shoulders | 16/6/3.5 | **32/15/3.0** |

#### A.9 · Scale Shoulders + Cape（3 件）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `AR_sturgia_shoulders_b` | Iron Scale Shoulders | 18/4/2.2 | **25/12/2.5** (vanilla `brass_scale_shoulders` 25 参照) |
| 2 | `AR_sturgia_shoulders_c` | Brass Scale Shoulders With Cape | 20/8/3.9 | **32/12/3.5** |
| 3 | `AR_sturgia_shoulders_d` | Iron Scale Shoulders With Cape | 20/8/3.9 | **34/12/3.5** |

#### A.10 · Northern Lamellar Shoulders (+ Scarf)（2 件）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `lamellar_shoulder` | Northern Lamellar Shoulders | 20/4/3.5 | **32/12/3.0** |
| 2 | `lamellar_shoulder_scarf` | Lamellar Shoulders With Scarf | 20/4/3.5 | **32/12/3.0** (Scarf 装饰 0) |

**A 组合计**：25 件

---

### B 组 · 无 shoulder/pauldron 命名（9 件 · arm = 0）

| # | id | 游戏名 | v1 b/a/wt | **v2 决议 b/a/wt** |
|---|---|---|---|---|
| 1 | `tv_sturgia_cloak_a` | Northern Long Cape | 8/0/4 | **12/0/2.0** (Cloth Long Cape) |
| 2 | `pauldron_cape_z` | Northern Long Cape (Plate) | 16/4/3.5 | **20/0/3.0** (Plate Cape 中档 · arm 追溯清 0) |
| 3 | `berserkr_fur_b` | Bear Pelt | 16/4/6 | **24/0/2.5** (vanilla Battania `armored_bearskin` 24 参照) |
| 4 | `berserkr_fur_c` | Rough Bear Pelt | 16/8/5 | **24/0/2.5** |
| 5 | `TV_battania_shoulders_c` | Brown Bear Pelt | 16/4/6 | **24/0/2.5** (arm 追溯清 0) |
| 6 | `berserkr_fur_a` | Plated Bear Fur | 18/4/7.4 | **28/0/3.0** (Plated Bear +4) |
| 7 | `TV_sturgia_shoulders_l` | Nordic Lamellar Cape | 16/6/5 | **25/0/3.0** (Lamellar Cape body-only 中档) |
| 8 | `TV_sturgia_shoulders_m` | Steel Lamellar Cape | 16/6/5 | **27/0/3.0** (Steel +2) |
| 9 | `TV_sturgia_shoulders_n` | Brass Lamellar Cape | 16/6/5 | **25/0/3.0** |

**B 组合计**：9 件

---

## Sturgia Cape 收官统计（2026-09-23）

**总数**：34 件 Sturgia Cape · 全 🔵 log-only

**下一步**：进入 Sturgia BodyArmor / HandArmor / LegArmor / HorseHarness

---

# Sturgia · BodyArmor（2026-09-23）

## 🔒 铁律 · Sturgia BodyArmor

> **顶点**：`sturgian_lamellar_fortified` **105/75/45/26** ⭐ · body ≤ 105
> **arm 顶 45**（vs Empire 67 / Vlandia 100 / Battania 40）· 中档
> **主流 Chainmail Huscarl Hauberk + Lamellar over Mail** · Plate 顶档

## Sturgia BodyArmor · 38 件

### A. Cloth 民用（1 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `kaftan` | Northern Kaftan | 2/1/1/8.3 | **6/5/5/1.0** (vanilla `northern_tunic` 6/5/5 直匹配 · wt 8.3 荒谬降到 1.0) |

### B. Leather Coat/Vest（4 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `ao_sturgian_leather_over_cloth` | Northern Leather Coat | 18/9/6/4.4 | **20/21/14/1.5** (vanilla `layered_leather_tunic` 20/21/14 直匹配) |
| 2-4 | `AR_sturgia_armor_b/d/d2` | Padded Leather Vest / Leather Coat variants | 22/6/6/3.1 | **25/25/22/2.0** (vanilla `nordic_sloven` 25/25/22 直匹配) |

### C. Leather Coat Over Mail（3 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1-2 | `AR_sturgia_armor_e/e2` | Leather Coat Over Mail | 26/12/10/8.3 | **35/33/29/8.3** (vanilla `sturgian_chainmale_shortsleeve` 35/33/29 直匹配 · Huscarl Hauberk) |
| 3 | `ao_sturgian_leather_over_mail` | Leather Coat Over Hauberk | 29/13/11/8.3 | **44/22/38/8.3** (vanilla `nordic_hauberk` 44/22/38 直匹配 · Huscarl Mail Shirt) |

### D. Double Hauberk / Padded Coat Over Mail（4 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `TV_sturgia_armor_a` | Double Hauberk | 32/15/15/12.3 | **77/44/44/12** (vanilla `sturgian_chainmale_longsleeve` 77/44/44 直匹配) |
| 2 | `TV_sturgia_armor_a2` | Decorated Double Hauberk | 32/15/15/12.3 | **77/44/44/12.3** (直匹配) |
| 3 | `ao_gen_gambeson_over_mail` | Padded Coat Over Mail | 32/16/16/10 | **44/22/38/9** (vanilla `nordic_hauberk` 参照) |
| 4 | `AR_sturgia_armor_a` | Padded Leather Vest Over Mail | 33/12/10/10.2 | **47/22/41/9** (vanilla `nordic_sloven_over_mail` 47/22/41 直匹配) |

### E. Lamellar Vest Over Gambeson/Mail（6 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1-3 | `DZ_sturgia_armor_a/a2/a3` | Lamellar/Decorated/Gilded Vest Over Gambeson | 37/8/2/9.5 | **40/15/15/6.2** (vanilla `sturgian_lamellar_gambeson` 40/15/15 直匹配) |
| 4-6 | `DZ_sturgia_armor_b/b2/b3` | Lamellar Vest Over Mailled Gambeson variants | 48/12/10/15.1 | **55/24/15/12** (vanilla `nordic_lamellar_armor` 55/24/15 直匹配) |

### F. Leather Tabard Over Mail（1 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_sturgia_armor_c` | Leather Tabard Over Mail | 40/22/12/21 | **47/22/41/9** (vanilla `nordic_sloven_over_mail` 47/22/41 直匹配) |

### G. Lamellar Vest Over Hauberk（2 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1-2 | `ao_sturgian_lamellar_vest + b` | Lamellar/Alternating Lamellar Vest Over Hauberk | 42/14/12/16.5 | **73/40/25/8.3** (vanilla `sturgian_lamellar_gambeson_heavy` 73/40/25 直匹配) |

### H. Scale Vest/Coat Over Mail（4 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1-2 | `AO_eorling_armor_d/d2` | Bronze/Iron Scale Vest Over Mail | 42/18/15/16 | **55/24/15/12** (vanilla `nordic_lamellar_armor` 参照) |
| 3-4 | `ao_eorling_bronze/iron_scale_over_mail` | Bronze/Iron Scale Coat Over Mail | 45/22/27/16 | **77/44/44/12** (vanilla `sturgian_chainmale_longsleeve` 参照) |

### I. Lamellar Over Hauberk 中档（8 件）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1-3 | `TV_sturgia_armor_f/f2/f3` | Lamellar/Decorated/Gilded Over Hauberk | 50/20/7/21.5 | **79/38/40/21.5** (vanilla `sturgian_lamellar_base` 79/38/40 直匹配) |
| 4-5 | `AR_sturgia_armor_f/f2` | Lamellar Coat Over Mail (2 变体) | 50/21/7/24 | **79/38/40/21.5** |
| 6-7 | `ao_sturgia_armor_d/d2` | Lamellar/Alternating Lamellar Coat Over Hauberk | 50/25/14/16.5 | **90/50/45/22** (vanilla `northern_brass_lamellar_over_mail` 90/50/45 直匹配) |
| 8-10 | `TV_sturgia_armor_e/e2/e3` | Heavy Lamellar Over Hauberk variants | 50/26/15/28 | **105/75/45/24** (vanilla `sturgian_lamellar_fortified` 105/75/45 顶点直匹配) |

### J. Alternating/Brass/Steel Scale Over Hauberk（3 件 · 顶档）

| # | id | 游戏名 | v1 b/l/a/wt | **v2 决议 b/l/a/wt** |
|---|---|---|---|---|
| 1 | `TV_sturgia_armor_b` | Alternating Scale Over Hauberk | 52/25/25/26 | **90/22/30/20** (vanilla `northern_coat_of_plates` 90/22/30 直匹配) |
| 2 | `TV_sturgia_armor_c` | Brass Scale Over Hauberk | 52/25/25/26 | **90/50/45/22** |
| 3 | `TV_sturgia_armor_d` | Steel Scale Over Hauberk | 52/25/25/26 | **90/50/45/22** |

**Sturgia BodyArmor 合计**：38 件 🔵 log-only

---

# Sturgia · HandArmor（2026-09-23）

## Sturgia HandArmor · 7 件

**vanilla 参照**：`northern_brass_bracers` 41 · `northern_plated_gloves` 56 顶

| # | id | 游戏名 | v1 arm/wt | **v2 决议 arm/wt** |
|---|---|---|---|---|
| 1-5 | `AR_sturgia_gloves_c` `TV_sturgia_gloves_b/c/d/e` | Iron/Rough Brass/Iron/Simple Steel/Brass Bracers | 22/1.3 | **41/1.3** (vanilla `northern_brass_bracers` 41 直匹配 · 5 件同档) |
| 6 | `AR_sturgia_gloves_a` | Brass Gauntlets | 23/1.8 | **56/1.5** (vanilla `northern_plated_gloves` 56 直匹配 · 顶) |
| 7 | `AR_sturgia_gloves_b` | Iron Gauntlets | 23/1.8 | **56/1.5** |

**Sturgia HandArmor 合计**：7 件 🔵 log-only

---

# Sturgia · LegArmor（2026-09-23）

## Sturgia LegArmor · 4 件

| # | id | 游戏名 | v1 leg/wt | **v2 决议 leg/wt** |
|---|---|---|---|---|
| 1 | `TV_aserai_boots_h` | Leather Boots | 3/1.0 | **20/0.9** (vanilla `sturgia_boots_d` 20 直匹配) |
| 2 | `TV_aserai_boots_i` | Blackened Leather Boots | 3/1.0 | **23/1.0** (vanilla `sturgia_boots_c` 23 直匹配) |
| 3 | `TV_sturgia_boots_a` | Boots With Steel Greaves | 24/2.7 | **60/2.9** (vanilla `northern_plated_boots` 60 顶 直匹配) |
| 4 | `TV_sturgia_boots_b` | Boots With Gilded Greaves | 24/2.7 | **60/2.9** (Gilded 装饰 0 · 同顶) |

**Sturgia LegArmor 合计**：4 件 🔵 log-only

---

# Sturgia · HorseHarness（2026-09-23）

## Sturgia HorseHarness · 6 件（全 4 字段）

**vanilla 顶**：`northern_ring_barding` **h=45/b=35/l=5/a=45/wt=15.2**（Half 覆盖顶）

| # | id | 游戏名 | v1 b/wt | **v2 决议 h/b/l/a/wt** |
|---|---|---|---|---|
| 1 | `AR_horse_armor_zi` | Heavy Noble Harness | 17/35 | **20/10/8/8/7.2** (vanilla `northern_light_harness` 20/10/8/8 直匹配 · wt 35→7.2) |
| 2 | `AR_horse_armor_zd` | Plated Ring Barding | 47/88 | **45/30/5/40/14** (Plated Ring sub-顶) |
| 3 | `AR_horse_armor_zag` | Chainmail Barding | 58/82 | **45/35/5/45/15** (vanilla `northern_ring_barding` 45/35/5/45 直匹配) |
| 4 | `AR_horse_armor_zae` | Iron Scale Barding | 60/82 | **45/35/5/45/15** |
| 5 | `AR_horse_armor_zae2` | Steel Scale Barding | 60/82 | **45/35/5/45/15** (Steel +2 装饰 0 · 已达顶) |
| 6 | `AR_horse_armor_zaf` | Ringed Mail Barding | 60/82 | **45/35/5/45/15** |

**Sturgia HorseHarness 合计**：6 件 🔵 log-only

---

## Sturgia 全 6 类收官统计（2026-09-23）

**总数**：**190 件 Sturgia 决议归档**（HeadArmor 105 · Cape 34 · BodyArmor 38 · HandArmor 7 · LegArmor 4 · HorseHarness 6）· 全 🔵 log-only

**帝国 + Vlandia + Battania + Sturgia 累计**：378 + 312 + 307 + 190 = **1187 件决议归档 · 突破 1000 里程碑**

**下一步**：进入 **Aserai 文化**（阿拉伯/沙漠画像 · Southern 沙漠部族 · 长弯刀 + 长袍 + 缠头巾）

---

## 状态图例
- 🔵 log-only · 决议已定案，XML 未改（低价值 cosmetic 类，v1 现值可接受，避免 XML churn）
- 🟡 pending deploy · XML 已改，等下次关游戏 + `deploy.ps1`
- 🟢 deployed · 已 deploy，等 in-game 观察
- ✅ verified · 用户实机验证通过
- ❌ rollback · 实测有问题已回滚
