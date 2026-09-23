# OSA Balance v2 · 手工审工作日志

**范围**：`OpenSourceArmouryRBMBalance` v1 脚本按 tier 分层的 buff 逻辑漏掉了大量视觉上明明是重装但 raw armor 值偏低的物品（比如 stylized/decorative 头盔）。v2 通过**用户 in-game 观察 → 手工逐件审**弥补这个缺口，按**文化 → 装备类型**顺序推进。

**工作流约定**：
- 用户在游戏内观察某物品数值失衡 → 提出目标数值
- Claude 改 `ModuleData/OSABalance_armor_override.xml` （或 `_pieces_override.xml`）
- **不 deploy**——用户游戏开着无法看装备属性；累积一批改动后由用户手动关游戏 → 跑 `deploy.ps1` → 重启
- 每件改动在本文档记录：**id / 名称 / 文化 / 类型 / 前 → 后数值 / tier 计算 / 部署状态**
- 用户实机验证后在本文档标 ✅

**优先方法（2026-09-22 定）**：
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

---

## 状态图例
- 🟡 pending deploy · XML 已改，等下次关游戏 + `deploy.ps1`
- 🟢 deployed · 已 deploy，等 in-game 观察
- ✅ verified · 用户实机验证通过
- ❌ rollback · 实测有问题已回滚
