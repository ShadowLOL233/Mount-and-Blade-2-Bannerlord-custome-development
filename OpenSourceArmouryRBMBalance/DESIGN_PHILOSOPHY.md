  OSA Balance v2 · 设计哲学

**用途**：`OpenSourceArmouryRBMBalance` 的设计基调宪法。从 2026-09-23 帝国头盔 165 件手工审的经验中提炼，适用于所有 OSA 装备类型（HeadArmor / BodyArmor / Cape / HandArmor / LegArmor / HorseHarness）。

**制定日期**：2026-09-23
**制定依据**：帝国头盔 165 件 v2 手工审的经验回顾 + 用户反馈信号

**相关文档**：
- [`BALANCE_V2_LOG.md`](./BALANCE_V2_LOG.md) — 逐件手工审工作日志（权威决议记录）
- [`VANILLA_REFERENCE.md`](./VANILLA_REFERENCE.md) — vanilla+RBM 已完成调整的权威锚点表
- [`MATERIAL_QUALITY_DICT.md`](./MATERIAL_QUALITY_DICT.md) — 结构/品质/装饰词字典

---

## 目录

- [Meta 观察](#meta-观察)
- [两条铁律](#两条铁律)
- [通用原则](#通用原则)
- [按装备类型的应用](#按装备类型的应用)
- [工作流纪律](#工作流纪律)
- [用户反馈信号识别](#用户反馈信号识别)
- [vanilla 锚点速查](#vanilla-锚点速查)
- [核心教训](#核心教训)

---

## Meta 观察

**v1 脚本系统性失败在三处**（v2 手工审出发点）：

1. **完全忽略视觉/命名信息**：`AR_empire_lord_helmet_a` (Faceguard) 与 `AR_empire_helmet_l` (Banded) 被同样按 head + aventail 公式处理，但一个是 Lord tier 一个是 Ridge-adjacent 中档
2. **Aventail 公式套错结构**：给纯 Ridge 头盔（无 mail 内衬）自动加 arm=25-30 假装它有 mail wrap
3. **Weight 处理粗糙**：所有 Cataphract-tier 头盔都掉到 wt 1.5-1.7，明显低估 heavy mail 构造真实重量

**v2 手工审的本质**：用 vanilla+RBM 已完成的**语义-数值双重锚定**替代 v1 的机械公式。

---

## 两条铁律

### 🔒 铁律 1 · vanilla+RBM 是唯一权威基础

**核心**：不擅自设定超越 vanilla+RBM 尺度的绝对值。RBM 已做的取舍视为设计终稿，OSA 只负责与其**同尺度对齐**，不建立独立尺度。

**执行五条**：
1. 先查 [`VANILLA_REFERENCE.md`](./VANILLA_REFERENCE.md) 找 vanilla+RBM 直匹配
2. 直匹配抄 vanilla 值不做主观微调
3. 无 direct match 走家族 fallback（aventail suffix 表 + base_type 家族均值 + 品质词 delta）
4. 越权 anchor（如 Faceguard 94 套 Roman 家族全档）= 禁止
5. 即使 vanilla 值看起来偏高/偏低也不二次调整 · OSA 只对齐尺度不建立独立尺度

**历史违规案例**：2026-09-23 Roman Helmet 家族首版把 Faceguard 家族 anchor 94/12/0 套到全 12 件顶档 → 用户驳回"过于夸张"。修订后按 vanilla 三档 direct match 通过。

### 🔒 铁律 2 · 头 > 身 > 臂（HeadArmor 专属）

**核心**：所有 HeadArmor 类物品必须满足 **head_armor > body_armor > arm_armor** 严格序。

**原因**：
- 物理直觉（头部是主要防护部位）
- 视觉一致性（头盔外观越"重装"应先反映在头档、次反映在颈档、最后才是肩档）
- 颈甲延伸（gorget）覆盖 body 有限
- 肩甲/aventail 覆盖 arm 更少

**执行**：即使 vanilla+RBM 参照物品本身违反此序（如 `roundkettle_over_imperial_mail` 92/0/20 · `imperial_nasal_helm` 97/12/25），OSA 平衡时**允许调整 body/arm 分布使其符合头 > 身 > 臂**，只要 raw 总和保持在 vanilla 家族尺度内。

**追溯适用**：已归档决议若违反本条铁律需追溯修正。2026-09-23 首批追溯：Roman Helmet #11/#12（Stripped Cloth 系）· Nasalhelm #1（`ao_imperial_nasal_helmet`）· Scale Coif o/o2。

**用户 quote**："所有头盔应当都需要做到头甲 > 身甲 > 臂甲"

---

## 通用原则

### 3.1 命名解析优先级

```
文化前缀 (Imperial/Southern/Nordic...)     → 定 tier 基调
    ↓
结构性词 (Faceguard/Closed/Heavy/Cataphract...) → 走结构性字典 dict delta
    ↓
品质词 (Gilded/Silvered/Noble/Iron/Bronze...)   → 走品质字典 head-only delta
    ↓
装饰词 (Plumed/Feathered/Decorated/Crested...)  → 严格 0 armor
```

### 3.2 "Imperial 双层内衬"原则（用户 2026-09-23 拍板）

**Imperial 前缀 = 领主装备 + 精工头盔 + 精工链甲/皮甲内衬**

对 OSA-only Imperial 物品，允许 body/arm 加成反映内衬（如 Palatine 60/38 · Cataphract 62/36），但**不套到 vanilla 本体上**（如 `empire_battle_crown_west` 保持 vanilla 109/12/0 不加 Imperial 双层）。

**跨类型推演**：
- Body armor 的"Imperial 双层" = 外板甲 + 内衬 gambeson/mail，体现在 arm/leg 覆盖
- Cape 的"Imperial 双层" = 斗篷 mantle + 内层肩甲结构
- HandArmor 的"Imperial 双层" = 皮革/布内衬 + 外层金属手甲

### 3.3 Tier 是 UI 信号不是差异化机制

公式 `tier = clamp(round((raw × ItemMult × 0.1) - 0.4), 0, 6) - 1` 在 h ≥ 38 就 clamp 到 T5。

**同 tier 内靠绝对值差异化**：Sagittarius Cloth 62 vs Cataphract Goggled 144 都是 T5，但战场护甲差 2.3 倍。**不要**通过降低 armor 值让物品落到 T4/T5——会破坏 vanilla-derived 真实性。

### 3.4 Direct-match 三要素对齐规则（字典 v8）

真直匹配要求：**base_type + aventail + 主要结构性词**三者对齐。
- ✓ OSA "Heavy Nasalhelm over Mail" ↔ vanilla `heavy_nasalhelm_over_imperial_mail`
- ✗ OSA "Heavy Spangenhelm With Mail" ↔ vanilla `heavy_nasalhelm_over_imperial_mail`（Spangenhelm ≠ Nasalhelm）

直匹配后**结构性字典不再叠加**，**品质字典可继续叠加**。

### 3.5 顶点参照约束

每个装备类型有 vanilla 顶点，作为 OSA 家族数值的绝对天花板。

| 装备类型 | vanilla 顶点 | raw 上限 |
|---|---|---:|
| HeadArmor | `imperial_goggled_helmet` 144/82/45 | 300 |
| BodyArmor | 待查 vanilla lord body armor | 待查 |
| Cape | 待查 vanilla lord cape | 待查 |
| HandArmor | 待查 vanilla gauntlet | 待查 |
| LegArmor | 待查 vanilla greaves | 待查 |

**任何 OSA 物品 raw 值不超过对应 vanilla 顶点**，即使 Imperial 前缀也不例外。精英差距（-20~-80 缺口）必须保留。

### 3.6 Weight 是一等公民

- **Chainmail 材质** = 全 mail 构造 = wt 3.8-5.0
- **Heavy / Closed / Cataphracts 结构** = +1-2 wt
- **Coif/Full mail liner** = wt 3.5+
- **装饰词不加 wt**
- **品质词不加 wt**

**v1 script 经常 wt 掉太多**（Cataphract 头盔掉到 1.5wt），v2 必须归正到符合真实构造的重量。

### 3.7 "有升有降"的诚实审计

**正确姿态**（避免"什么都上调"的偏见）：
- v1 明显过 buff（如 Roman Helmet _b 系 h=78-90 但 base 应 20-30 tier）→ 下调
- v1 明显漏 buff（如 Roman Helmet _a 系 h=20 未动）→ 上调
- v1 尺度合理仅 body/arm 分布错 → 只调分布
- vanilla 直匹配物品被 v1 反 nerf（如 `empire_battle_crown_west` 109→85）→ 回归 vanilla

---

## 按装备类型的应用

### 4.1 HeadArmor（本轮已实证 · 165 件）

- **铁律 2** 头 > 身 > 臂 严格适用
- **顶点**：Goggled 144/82/45（raw 300）
- **工作流**：家族划分 → mesh 细分 → aventail 分档 → 品质微调
- **子群策略**：Cataphract / Lord / Palatine / Ridge / Kettle / Nasalhelm / Spangenhelm / Roman / ... 各自独立分档

### 4.2 BodyArmor（推演 · 待实证）

- **拟议铁律**：`body_armor ≥ arm_armor ≥ leg_armor`（torso 主防护，臂/腿由 body 延伸）
- **Imperial 双层理解**：外板甲 + 内衬 gambeson/mail
- **跨槽结构注意**：RBM/OSA BodyArmor 有 body 兼 arm+leg 现象，头 > 身 > 臂类似逻辑推演出 body ≥ arm ≥ leg
- **顶点**：需查 vanilla lord body armor (`imperial_scale_armor` 类)

### 4.3 Cape（2026-09-23 铁律已立 · 二部分律）

**🔒 Cape 家族铁律 · 第一部分：命名二分律**
- **A. 有 "shoulder" 或 "pauldron(s)" 命名**：允许 body + arm 同时 > 0，但**body > arm 严格序**
- **B. 无 shoulder/pauldron 命名**（Cape/Cloak/Sash/Focale/Pelt/Collar 等）：**arm 必须 = 0**

**理由**：命名反映 mesh 视觉外形——带 shoulder/pauldron 的 mesh 明显有肩甲结构，无此命名的 mesh 是纯斗篷/披风。

**🔒 Cape 家族铁律 · 第二部分：arm mesh-tiered 分档律**（2026-09-23 方案 C）

Cape (A 组) 的 arm 值按 mesh 视觉覆盖度分档：

| mesh 档次 | arm 值 | 命名特征 | 覆盖度含义 |
|---|---:|---|---|
| Elite Heavy 顶档（Gilded 品质） | **25** | Gilded / Heavy | 甲片覆盖整个肩膀 + 上臂到肘 |
| Standard Shoulders | **20** | 标准 Lamellar Shoulders | 肩膀 + 上臂上半 |
| Standard Pauldrons | **12** | Pauldrons 命名 | 仅肩膀（较小 mesh） |
| Studded Strip / lightweight | **6-8** | Studded / Strip | 局部加固 |
| Chainmail Shoulders 中档 | **10-12** | Mail Shoulders | 链甲护肩 |
| Leather Shoulders 轻档 | **4-8** | Leather Shoulders | 皮革护肩 |

**arm 堆叠机制**（重要设计取舍）：Bannerlord 引擎 arm 计算加法制 `final = HeadArmor.arm + BodyArmor.arm + Cape.arm + HandArmor.arm`，不区分手臂子区间。**OSA 精英兵 arm 总值比 RBM 基线高 ~25 点（26%）**——**故意的设计取舍**：换取 OSA "Cape 特色" 忠实反映 mesh 视觉覆盖度。用户认可"物理层面 Pauldron 覆盖上臂 + Gauntlet 覆盖前臂"的分工，接受引擎层面的 arm 加法所带来的膨胀。

**vanilla 顶点参照放宽**：vanilla `imperial_lamellar_shoulders` 是 55/0/3.5（raw 55 · body only）。OSA 允许 body ≤ vanilla body（如 42/25 raw 67），因为 OSA 是"分配到 shoulder + upper_arm 两块 mesh"而非"堆积在同一块 mesh"。**只要 body ≤ vanilla body 顶（55），arm 加成属 OSA 特色不算越权铁律 1**。

**89 件 Empire Cape 里**：59 件（有 shoulder/pauldron）保留 arm 特色 · 30 件（body only）追溯清 arm 到 0。

**Empire Cape 家族梯度**（vanilla body only + OSA arm 按 mesh 分档）：
```
Leather 护肩 12-27 body / arm 4-8   → Chainmail 护肩 35 body / arm 10-12
Plate Pauldron 15-31 body / arm 12  → Plate Heavy Lamellar 55 body / arm 25 ⭐
```

**跨类型推演**：其他文化 Cape 类型（Vlandia/Battania/Sturgia/Aserai/Khuzait/Nord）默认沿用此二部分律。特别注意 Nord vanilla 有 arm max 8（可能存在 shoulder-类似 mesh · 待实证）。

**🔒 Cape 家族铁律 · 第三部分：视觉判断优先律**（2026-09-23 F 家族揭示）

**命名允许 ≠ 数值强制**。命名二分律（第一部分）规定"有 shoulder/pauldron 命名的 Cape **允许** arm > 0"，但实际 arm 值必须通过 **mesh 视觉覆盖度确认**。若命名含 shoulder/pauldron 但 mesh 视觉**不覆盖上臂**（如纯覆盖 shoulder body 或 body），则 arm **必须 = 0**，覆盖命名默认值。

**arm mesh-tiered 分档律扩展**（加入"部分覆盖"档）：

| mesh 视觉档 | arm 值 |
|---|---:|
| Elite Heavy 顶档 · 完整肩+上臂覆盖 | 25 |
| Standard Shoulders · 肩+上臂上半 | 20 |
| **部分上臂覆盖**（如 Harness Over Scale · 小于 Lamellar） | **15** |
| Standard Pauldrons · 仅肩 | 12 |
| Chainmail Shoulders 中档 | 10-12 |
| Studded Strip / Leather 轻档 | 6-8 |
| **纯 body 无上臂覆盖**（Scale/Alternating/Steel Scale 类） | **0** |

**用户 quote 佐证**："scale shoulders 的 mesh 似乎只覆盖身体，并不覆盖肩膀" · "with lamellar 的版本才有覆盖大臂的扎甲铁片" · "Decorated Leather Harness Over Scale 是同时覆盖了肩部和大臂，但大臂的护甲覆盖面积要比 Lamellar 系列要小"

### 4.4 HandArmor（简单）

- 仅 arm_armor 字段
- 线性档次：手套/半甲/全 gauntlet
- **顶点**：`imperial_gauntlets` 类

### 4.5 LegArmor（简单 · 注意排除）

- 仅 leg_armor 字段
- 线性档次：护胫/全 greaves
- **排除**：靴/鞋（`shoes|boots|moccasins` id 关键词跳过），民用鞋不是护具（2026-09-20 已在旧日志确认此规则）

### 4.6 HorseHarness（独立维度）

- 马甲 body_armor（马身覆盖）+ reins_mesh + charge/maneuver/speed bonus
- 完全不同的设计维度 · 需要独立分析
- v1/v1.1 未处理，Saddlery avg 58.5 vs RBM 28.9 → 反向下调 ×0.5（旧日志已识别）

---

## 工作流纪律

### 5.1 XML-first 验证（用户曾发现 bug）

**引用日志前必须核实 XML**。
- 展示家族对比表前先 grep XML 拿真实数据
- 不凭 OSA 源 CSV 或日志推断 v1 现值
- v1 XML 与 OSA 源可能大幅偏离（v1 script 已 buff 过）

### 5.2 归档模式（🔵 log-only）

不是所有决议都需要立刻改 XML。两类情况归档而非执行：
- **v1 现值已接近 v2 目标**（改 XML 收益小于打断成本，如 Roman Helmet _b/_d 系）
- **cosmetic 低价值物品**（帽/头饰类）

用户 quote："不需要进入修改工作，归档即可"

### 5.3 显示名 bug 检测

XML 里 `<Item name="{=xxx}显示名"...>` 与 id 语义不符时（如 `ao_imperial_pointed_helmet_with_mail_coif` 显示为 "With Lamellar Strips"），**按 id 语义处理数值**，日志记录 bug 待未来 language XML 修正。

### 5.4 同名双件处理（用户 in-game 发现）

XML 里两个不同 id 但同显示名时（如 `echerian_elite_helm` 与 `imperial_lord_helm_mail` 都显示 "Imperial Closed Guarded Lord Helmet"），**必须同数值处理**——玩家 in-game 无法区分。

### 5.5 漏审侦查

系统 grep 时按显示名 + id 双向核对。发现漏审必须补齐（Lord 家族补 `_a`/`_b` 是先例，Elite 家族追加 `imperial_lord_helm_mail` 是先例）。

### 5.6 追溯适用律

新铁律建立后**必须扫描已归档决议**。发现违反必须追溯修正而非"新规定不追溯"。

### 5.7 状态图例

- 🔵 **log-only** · 决议已定案，XML 未改（低价值 cosmetic 类 · v1 现值可接受 · 避免 XML churn）
- 🟡 **pending deploy** · XML 已改，等下次关游戏 + `deploy.ps1`
- 🟢 **deployed** · 已 deploy，等 in-game 观察
- ✅ **verified** · 用户实机验证通过
- ❌ **rollback** · 实测有问题已回滚

---

## 用户反馈信号识别

session 中形成的信号-响应映射：

| 用户信号 | 含义 | 正确调整 |
|---|---|---|
| "过于夸张" | 越权套 anchor / 超 vanilla 尺度 | 换更合适的 vanilla 锚点 |
| "过低" / "身甲臂甲不能低" | Imperial 内衬没体现 | body/arm 上调到 60+/35+ 档 |
| "数值反过来更好" | 违反物理/视觉直觉 | 建立新铁律 |
| "我在游戏里看到两个" | 数据一致性问题 | 显示名 grep + 同步修改 |
| "不需要修改工作，归档即可" | 归档模式 | 🔵 log-only 状态 |
| "你考虑过 X 了吗" | 我漏了某维度 | 补上并道歉 |
| "数值没有问题" / "很好" / "可以" | 确认通过 | 归档进入下一批 |
| "带有 Imperial 前缀的几乎都是领主装备" | 定义分类语义 | 内化为规则 |

---

## vanilla 锚点速查

### HeadArmor 阶梯（Empire · 已实证）

```
Cloth Coif 14-22  →  Leather 14-38  →  Mail Coif 38-41  →  Nasalhelm 55-97
                                                              ↓
Roundkettle 47-92  →  Spangenhelm 65-97  →  Lord Faceguard 94  →  Plumed 104
                                                                    ↓
Battle Crown 104-109  →  Metal Strips Lord 120  →  Jeweled 125  →  Noble Guard 123
                                                                    ↓
Guarded Lord 130  →  Heavy Nasal Mail 124  →  Goggled Cataphract 144/82/45 ⭐顶点
```

### 结构字典（v8）

```
Cataphracts +7/+7/+5/+2wt (顶档 · elite mail 全罩)
Face Plate/Closed/Visored +5/+5/+3/+1wt (面罩全脸闭合)
Heavy +4/+3/+3/+2wt (全方位加厚)
Metal Strips/Ridge/Face Guard +4/+3/+2/+1wt (局部加固)
Lord +2/+1/+1/+0wt (全方位精工)
```

### 品质字典

```
Bronze -1  <  Iron/Silvered +1  <  Gilded/Noble/Jeweled +2 (head only)
```

### 装饰字典（0 armor）

```
Plumed / Feathered / Decorated / Crested / Redcrest
Crowned / Southern / Eastern / Northern / Western
```

---

## 核心教训

### 教训 1 · 不要机械套公式

v1 script 是反面教材。每个物品需要**命名解析 + 视觉推理 + vanilla 锚定**三重决策。任何"一律 head × 1.5"的自动化 buff 逻辑都会失败。

### 教训 2 · 用户视觉判断权重最高

我看不到 in-game 装备外观，用户看得到。任何数值决议需要 user 视觉信号确认，特别是：
- Mesh 差异化判定（Cone > Pointed？Iron > Brass？）
- Aventail 覆盖范围（皮革条覆盖到哪？）
- 结构性词判定（Scout 系有没有 Faceguard？）
- 同名双件识别

### 教训 3 · 铁律是渐进立法而非一次性设计

vanilla+RBM 铁律（Iron Rule 1）和 头 > 身 > 臂 铁律（Iron Rule 2）都是在**被驳回后从错误案例中提炼出来**的。未来还会有第三、第四条铁律等着被立——每次用户驳回都是一次立法机会。

### 教训 4 · 归档不等于完成

🔵 log-only 状态表示"决议已审"但不代表 XML 生效。**当 bulk XML deploy 时机成熟**（可能是文化全收官或用户主动 deploy 阶段），需要扫描所有 🔵 决议并批量应用到 XML。

### 教训 5 · 家族边界模糊时按 id 语义

- `AR_empire_lord_helmet_f` 有 "Spangenhelm" 也有 "Lord" 命名 → 按 id `lord_helmet` 归 Lord 家族
- `AR_empire_legatus_helm_a` 有 "Crested Lord" 命名 → 按 id `legatus_helm` 归 Legatus 家族
- 家族分类以 id 前缀为准，装饰词/结构词二次判定

---

## 修订历史

- **v1.0** (2026-09-23)：从帝国头盔 165 件手工审经验中提炼首版
  - 铁律 1（vanilla+RBM 唯一权威）
  - 铁律 2（头 > 身 > 臂 HeadArmor 专属）
  - 通用 7 条原则
  - 6 类装备应用推演
  - 7 条工作流纪律
  - 用户反馈信号识别表
