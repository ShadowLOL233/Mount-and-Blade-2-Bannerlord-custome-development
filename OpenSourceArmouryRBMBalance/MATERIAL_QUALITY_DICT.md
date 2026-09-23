# 材质品质字典（Material Quality Dictionary）

**用途**：`OpenSourceArmouryRBMBalance` v2 平衡工作的**权威字典**。vanilla 映射得到 base 值后按此表加/减 delta，实现"些许区别性"（避免视觉不同物品数值完全一致），同时保持 vanilla+RBM 平衡曲线作为主锚点。

**判定流程**：
1. **Base 值**来自 [`VANILLA_REFERENCE.md`](./VANILLA_REFERENCE.md) 的 vanilla 参照
2. **应用字典 delta**：按物品名字（或 id，仅 Lord）里出现的词加/减
3. **叠加规则**：多词求和，单件 head 单项上限 **±5**
4. **优先级规则**（v8 精化 · 2026-09-22）：物品若已通过**直接 vanilla 匹配**得到 base 值，**结构性字典词**（Heavy / Face Plate / Metal Stripes / Ridge / Face Guard / Cataphracts / Closed / Visored）不再叠加——vanilla 已含总体设计，避免双重加成。**品质字典词**（Lord / Noble / Gilded / Silvered / Jeweled / Iron / Bronze）可继续叠加。
   - **direct match 有效条件**：`base_type + aventail + 主要结构性词` 三者都对齐才算直匹配。例如 OSA "Heavy Spangenhelm With Mail" 匹配 vanilla `heavy_nasalhelm_over_imperial_mail` **无效**（Spangenhelm ≠ Nasalhelm，跨 base_type），应回退到 base `ironlame_feathered_spangenhelm_over_mail` (95/12/25) + Heavy dict (+4/+3/+3/+2)
   - vanilla `heavy_nasalhelm_over_imperial_mail` 匹配 OSA "Noble Heavy Nasalhelm" **有效**（base_type + aventail + Heavy 全对齐）

**数值精度规则**（2026-09-22 定）：
- **Armor 值（head_armor / body_armor / arm_armor）必须整数**（不允许小数）
- **Weight 允许小数**（游戏 UI 显示 1 位小数，用于精细区分）

---

## 字典表 v4（2026-09-22）

### 结构性/工艺加强类（影响 head + body + arm + weight）

| 词 | Δ head | Δ body | Δ arm | Δ wt | 匹配范围 | 语义 |
|---|---:|---:|---:|---:|---|---|
| **`Cataphract / Cataphracts`** | **+7** | **+7** | **+5** | **+2** | 名字 | 重骑重装级：全脸闭合 + 完整链甲颈肩 + 加固构造 |
| **`Face Plate / Faceplate / Closed / Visored`** | **+5** | **+5** | **+3** | **+1** | 名字 | 面罩/闭式全脸覆盖 / 带 visor 机构 |
| **`Heavy`** | **+4** | **+3** | **+3** | **+2** | 名字 | 加厚构造，覆盖全方位（比 Ridge/Face Guard 略强、比 Face Plate 弱）|
| **`Metal Stripes / Metal Strips / Ridge / Face Guard / Faceguard`** | **+4** | **+3** | **+2** | **+1** | 名字 | 局部金属加固：加固带 / 帽顶凸脊 / 面颊铁片保护 |
| **`Lord`** | **+2** | **+1** | **+1** | +0 | 名字 / id | 领主级、全方位精工（精工不加体重）|

### 单纯品质类（只影响 head）

| 词 | Δ head | 匹配范围 | 语义 |
|---|---:|---|---|
| `Noble` | +2 | 名字 | 贵族版工艺 |
| `Gilded` | +2 | 名字 | 镀金精工 |
| `Jeweled` | +2 | 名字 | 镶宝石贵族级 |
| `Silvered` | **+1** | 名字 | 镀银精工（银比金常见，2026-09-22 从 +2 修订） |
| `Iron` | +1 | 名字 | 铁质硬度 |
| `Bronze` | -1 | 名字 | 青铜较软 |

### 纯装饰类（零护甲影响）

| 词 | 类别 |
|---|---|
| `Decorated` | 装饰（无工艺暗示） |
| `Crowned / Crested / Feathered / Plumed / Redcrest` | 顶饰 |
| `Southern / Eastern / Northern / Western` | 地域风格 |

### body/arm 修饰的边界规则

**Dictionary body/arm 加成仅当 base 值 > 0 才生效**：如果 vanilla base 是 "bare"（b=0/a=0，比如 `tall_helmet` / `leather_cap`），dictionary 的 body/arm +1 不生效，只有 head 修饰生效。理由：dictionary 无法凭空创造 aventail/颈肩结构。

例：`AR_intercisa_helmet_b` (Open Ridged) base = `tall_helmet` 84/0/0 + Metal Crest+2h（b/a 保持 0）= **86/0/0**。

---

## 品质阶梯速览

```
Bronze -1   <   Iron/Silvered +1  <   Noble/Gilded/Jeweled +2 (head only)
                                                              ≈   Lord +2/+1/+1 (no wt)
                                                              <   Metal Stripes/Ridge/Face Guard +4/+3/+2 (+1 wt)
                                                              ≈   Heavy +4/+3/+3 (+2 wt, 全方位加厚)
                                                              <   Face Plate/Closed/Visored +5/+5/+3 (+1 wt)
                                                              <   Cataphracts +7/+7/+5 (+2 wt)
```

## 未来扩展候选词

遇到新词时先在 [`BALANCE_V2_LOG.md`](./BALANCE_V2_LOG.md) 立项讨论再入字典。可能的候选：

- `Reinforced / Studded / Layered / Master` → 类似 Iron，+1-2 head 或全方位 +1
- `Cheap / Rusted / Broken / Ragged` → 类似 Bronze，-1-2 head
- `Chieftain's / Warlord's / King's / Emperor's / Royal` → 类似 Lord，全方位加成
- `Steel` → 需研究：可能 +1-2 head（比 Iron 高、比 Gilded 低）
- `Studded / Banded` → 类似 Metal Stripes？还是独立类别？
- **Face Guard / Faceguard**（分开 Face Plate）→ 讨论中，见 §Face Guard 分档决议

---

## 修订历史

- **v1**（2026-09-22）：初版，Gilded/Noble/Jeweled/Iron/Bronze + Southern/Feathered/... 零影响
- **v2**（2026-09-22）：加 Lord（+2/+1/+1，唯一全方位）
- **v3**（2026-09-22）：加 Face Plate/Closed（+5/+5/+3）、Metal Stripes/Ridge（+2/+1/+1）+ 优先级规则（直接 vanilla 匹配不叠加）
- **v4**（2026-09-22）：Silvered 从 +2 → +1（形成 Bronze < Iron/Silvered < Gilded/Jeweled/Noble < Lord 阶梯）
- **v5**（2026-09-22）：合并 `Face Guard / Faceguard` 到 Metal Stripes/Ridge 档，同时全档 delta 从 +2/+1/+1 → **+4/+3/+2**（vanilla `helmet_with_faceguard` vs `tall_helmet` = +10/+12/+0 提示"局部加固/面护"实际护甲贡献远大于原 dict 值，字典取保守中值 +4/+3/+2）
- **v6**（2026-09-22）：`Visored` 加入 Face Plate/Closed 档（+5/+5/+3）——visor 机构与全脸闭合防护量级相当
- **v7**（2026-09-22）：拆 `Cataphracts` 为独立顶档（+7/+7/+5，+2 wt）；结构性/工艺加强档全部加 wt 修饰（Metal Stripes/Ridge/Face Guard +1 wt、Face Plate/Closed/Visored +1 wt、Cataphracts +2 wt）；明确数值精度规则（armor 整数、weight 允许小数）
- **v8**（2026-09-22）：`Heavy` 加入结构性档（+4/+3/+3, +2 wt）；精化 direct-match 规则要求 base_type+aventail+结构词三者对齐（修复 `Heavy Spangenhelm With Mail` 错匹到 `heavy_nasalhelm_*` 的跨 base_type 问题）
