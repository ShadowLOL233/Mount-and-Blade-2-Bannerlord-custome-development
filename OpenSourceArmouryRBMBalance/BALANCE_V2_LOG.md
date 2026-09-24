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

## 状态图例
- 🔵 log-only · 决议已定案，XML 未改（低价值 cosmetic 类，v1 现值可接受，避免 XML churn）
- 🟡 pending deploy · XML 已改，等下次关游戏 + `deploy.ps1`
- 🟢 deployed · 已 deploy，等 in-game 观察
- ✅ verified · 用户实机验证通过
- ❌ rollback · 实测有问题已回滚
