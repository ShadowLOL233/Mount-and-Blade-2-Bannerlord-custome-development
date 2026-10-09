# 萨迪厄斯帝国 · 兵种命名设计（Thaddeus Empire — Troop Naming）

**状态**：**🟡 v0.5 启动**（2026-10-08 用户三条新决策）· 皇家卫队换阵（瓦兰吉 → Hetairos+Hypaspistes · ✅ 已落地）· §三 §四 §五 旧通用池废弃（✅ 已落地）· **基础常备军转多文化整合路线**（⏳ v0.5 骨架待设计 · 下方 §基础主线 v0.3 节 pending 重构）。本文件**着重兵种命名**，背景设定仅作命名依据的骨架。**完成进度见下方 Status 节。**

**2026-10-08 用户三条新决策**：
1. **皇家卫队换阵**：瓦兰吉卫队（Pelekyphoros 御斧卫 / Varangos 卫兵）→ **Thaddian Hetairos（伙伴骑）+ Thaddian Hypaspistes（皇家持盾卫）** · 亚历山大马其顿旗舰单位组合 · ✅ 已落地见下方 ★ 皇家卫队节
2. **常备军抛弃单纯拜占庭+辅助军模式** → 转为**卡拉迪亚大陆各文化与军事整合**的多文化融合路线 · 跟精锐线 10 分支平起平坐（文化层不再是常备军/精锐的分野 · tier 深度才是）· ⏳ 骨架待设计
3. **§三 §四 废弃**（旧 v0.1 通用池） · §五 同步废弃（原已标废） · ✅ 已落地见下方 §三、四、五 废弃说明

**世界观基座**：骑马与砍杀 II 卡拉迪亚（Calradia）。

---

## ★ 命名基调决策（v0.4 定案 · 2026-09-24 · v0.5 增补 · 2026-10-08）

| # | 决策 | 含义 / 影响 |
|---|---|---|
| **1** | **偏希腊 · 不转硬拉丁** | 保持 Byzantine 希腊为主基调；G 野战军现存拉丁名（Legionarius/Palatinus/Menaulatos）**保留**但不再向外扩张；后续新命名默认走希腊 |
| **2** | **不重命名 vanilla 帝国兵** | 本工作**仅覆盖 Retinues 自定义兵**（+ 玩家 clan Champion/Guard）；vanilla 帝国兵原名不动 · 无需搞替换 mod |
| **3** | **帝国形容词固定 `Thaddian`** | 弃用 `Thaddean / Sadian` 备选；全文所有 "萨迪厄斯…" 前缀统一 `Thaddian` |
| **4** | **Tier 深度 T7 封顶** | 现有顶点（Basilikos Klibanophoros / Athanatoi / Aristotoxos Oreinos / Vardariotai Skythikoi / Mameloukos Furusi / Faris Prodromos / 等）**就是 T7 定档**；不再向 T8-T10 铺 · 不占 `MaxTroopTier=10` 的上限 |
| **5** | ~~帝国无外籍军团 Foederati 作废~~ **叙事依据升级为 α 熔炉吸纳**（决策 **8** 升级）| 2026-09-24 原文 "归化 vs Foederati" 的二分法已被 2026-10-08 的"α 熔炉吸纳" 取代；精锐线 10 分支结构保留但叙事从"归化外族"改为"传承军团"（Caracalla 公民权模型） |
| **6** | ~~归化线 T6 封顶 · 无御用~~ **叙事依据失效 · 结构保留**（决策 **8** 升级）| α 下外族概念消失 · "外族不列御用" 的 blood ceiling 叙事依据不成立；但精锐线 T7 二分（3 纯帝国顶 + 7 文化传承顶）**结构保留** · 作为"核心传统 vs 文化传承"的区分 · 不再是血统天花板；下 session 用户可拍板是否放开 T7 Basilikos 前缀 |
| **7** | **归化四族并入精锐 Tagmata（§八 A 案）** | 诺德(**G**) / 斯特吉亚(**H**) / 瓦兰迪亚(**I**) / 巴旦尼亚(**J**) 各成一支分支并入精锐主线 · 每支 T5→T7（3 tier） · 沿用"族源身份词根"命名模式 · 在 α 下这些**不再是"归化外族"而是"文化传承军团"** |
| **8 (新)** | **α 熔炉吸纳 · Caracalla 公民权模型**（2026-10-08 用户拍板）| 常备军抛弃 v0.3 的"拜占庭+补丁"模式 → **4 分支 24 兵种全帝国通用名**（见 ★ 基础主线 v0.5）· 外族概念在常备军 XML 层消失 · 所有兵种 `culture="Culture.thaddian"` · 族源词根留给精锐线 T6-T7 传承 · 现代类比 Gurkha Rifles / Coldstream Guards 的荣誉传承制 · 对应 Journal §2621 的 2026-09-24 旧方案 |
| **9 (新)** | **皇家卫队换阵 · Hetairoi + Hypaspistai**（2026-10-08 用户拍板）| Retinues House Champion/Guard = **Thaddian Hetairos（伙伴骑）+ Thaddian Hypaspistes（持盾卫）** · 亚历山大马其顿旗舰单位 · 完全希腊纯正 · 旧瓦兰吉卫队（Pelekyphoros/Varangos）出局（词根下放到常备军/精锐 G 诺德分支作传承） |

---

## 完成进度 / 待办（Status · 2026-10-08 更新 · v0.5 启动）

### ✅ 已完成
- **★ 精锐主线（Tagmata / 学院）v0.4 · 10 分支**：root T2 → 顶点 T7，命名完整（结构不动 · 叙事从"归化"升级为"传承军团"，决策 #8）
  - 纯帝国 3 支：A 重骑（Basilikos Klibanophoros T7）· B 重步（Athanatoi T7）· C 步射（Aristotoxos Oreinos T7）
  - 文化传承 7 支：D Vardariotai Skythikoi T7 · E Mameloukos Furusi T7 · F Faris Prodromos T7 · G Sekyriphoros Ulfhednar T7 · H Rhos Bogatyr T7 · I Latinikoi Milites T7 · J Rhomphaiaphoros Areios T7
- **★ 皇家卫队 v0.5（2026-10-08 换阵）**：Retinues House Champion = **Thaddian Hetairos（伙伴骑）** · House Guard = **Thaddian Hypaspistes（持盾卫）** · 亚历山大希腊旗舰单位 · 纯希腊无族源味
- **★ 基础主线 v0.5（2026-10-08 α 熔炉吸纳重构）** · 4 分支 24 兵种：
  - A 持盾长枪重步 7 支（Kontaratos → Palatinus/Menaulatos Palatinos 双顶）
  - B 冲击重骑 3 支（Kavallarios → Kataphraktos T5 封顶）**新增**
  - C 远程线 9 支（弓/弩/连弩 · 保留 v0.3）
  - D 骑射/标枪骑 5 支（Hippotoxotes → Skythikon/Mardaitos T5 封顶）**新增**
- **★ 命名基调决策（v0.4 定案 2026-09-24 + v0.5 增补 2026-10-08）** — 见上方决策表 1-9 条
- **✅ §三 §四 §五 作废**（2026-10-08）· 保留废弃说明块 · 词根下放到 v0.5 常备军/精锐传承

### ⬜ 未完成 / 待办
- ⬜ **精锐线 T7 御用限制是否放开**（决策 **6** α 下叙事依据失效）：7 支文化传承顶（Skythikoi/Furusi/Prodromos/Ulfhednar/Bogatyr/Milites/Areios）是否可加 Basilikos 前缀 · 下 session 用户拍板
- ⬜ **装备与技能数值配置**：精锐 10 支 + 常备军 24 支 + 皇家卫队 2 支 = 36 支的 Equipment Set + Skill 数值全待填
- ⬜ **Thaddian 自定义文化 mod**（Journal §2621 调研已完成）：α 熔炉采纳后 · 可开工建 `ThaddeusCulture` 自研 mod（sp_cultures.xml + spnpccharacters.xml） · 工作范围与 scope（clan-level vs AI kingdom）待拍板
- ⬜ **Retinues 御林军改名 blocker 实测**：玩家称帝后 Retinues `TroopBuilder.MakeRetinueName` 会自动改成 King's/Queen's Champion + Royal Guard——Hetairos/Hypaspistes 是否被覆盖需实测
- ⬜ **§一 "设定基调" + §二 "命名体系规则" 跟 α 对齐**（现文第 2/3 条还提"三元融合 / 蛮族外籍军团" · α 下语义已变 · 下 session 用户发话再修）

---

## 一、设定基调（命名的依据）

萨迪厄斯帝国（**Thaddeus Empire**，形容词 **Thaddian**）是承接旧卡拉迪亚帝国的新帝国——**其对旧帝国的关系 = 拜占庭帝国之于古罗马**。因此命名体系直接沿用并扩展卡拉迪亚帝国已有的**希腊-拉丁（Greco-Roman / Byzantine）**风格。

三条设定 → 三个命名层：

1. **希腊-拉丁核心**（帝国文化）：正统军团用**拜占庭希腊语 + 晚期罗马拉丁语**命名（Skoutatoi / Kataphraktoi / Tagmata…）。
2. **三元融合**（帝国 + 库扎伊特 + 阿塞莱）：草原骑射与沙漠骑兵被"帝国化"收编，用**希腊语给外族兵团起名**（拜占庭真实做法：Tourkopouloi「突厥之子」、Vardariotai、Mardaitai）。
3. **蛮族外籍军团（Foederati）**：诺德/斯特吉亚/瓦兰迪亚/巴旦尼亚不被排斥，而是作为**帝国外籍军团**编入，各保留鲜明外族风味但冠以帝国建制名（拜占庭真实做法：瓦兰吉卫队 Varangoi、拉丁尼康 Latinikon、罗斯 Rhos）。

> 一句话命名哲学：**核心用希腊-拉丁，外族用"希腊化的外族名"**——既统一又能一眼看出出身。

---

## 二、命名体系规则

- **格式**：`In-game 名（希/拉丁）` · `中文` · `（来源文化 / 角色 / 典故）`。In-game 用英文/转写名（游戏与 Retinues 显示英文）。
- **tier 递进**：低阶用通用兵称（Neosyllektos 新兵 → Stratiotes 士兵），高阶升为**兵团专名 + 建制前缀**（`…Tagmatikos` 军团、`Basilikos…` 御用、`…tes Vigla` 禁卫值更）。
- **建制前缀（表身份/精锐）**：
  - `Basilikos / Basilikon`（Βασιλικός）= 皇帝御用 → 顶级近卫
  - `Tagmatikos`（Τάγμα）= 中央常备军团 → 精锐
  - `Thematikos`（Θέμα）= 军区地方兵 → 中低阶正规
  - `Palatinos`（Palatine）= 宫廷卫
- **外籍军团**：整支保留族名词根（Varangos / Frangos / Rhos / Rhomphaiaphoros），精锐再加 `…of the Hetaireia`（御从近卫）。

---

## ★ 精锐主线定稿（Thaddeus Elite Line v0.4 · 2026-09-24 结构定案 · 2026-10-08 α 叙事升级）

> **🟡 2026-10-08 α 熔炉吸纳叙事升级**（决策 **8**）：v0.4 的 "归化四族 G/H/I/J" **结构不动** · 但叙事**从"归化外族"升级为"文化传承军团"**（Caracalla 公民权模型 · 现代类比 Gurkha Rifles）。D-J 7 支在 α 下**不再是外族** · 而是帝国内部的"保留族源荣誉名的传承单位"。决策 #5 "外族不列御用" 的 blood ceiling 叙事依据**失效** · 但 T7 二分（3 纯帝国顶 + 7 文化传承顶）**结构保留** · 作为"核心传统 vs 文化传承" 的区分 · 不再是血统天花板。**T7 Basilikos 前缀是否放开给 7 支文化传承顶 · 下 session 用户拍板**。
>
> **v0.4 结构性升级**：帝国不设外籍军团（决策 #5），4 文化传承族并入精锐 Tagmata 作 G/H/I/J 支，全部 T7 封顶（决策 #4）。**T7 二分**：3 支纯帝国走 Basilikos 御用/Athanatoi/Aristotoxos 顶（A/B/C）· 7 支文化传承用**族源特色精锐头衔**（Skythikoi/Furusi/Prodromos/Ulfhednar/Bogatyr/Milites/Areios）· α 下不再是"外族天花板" · 而是"核心军团 vs 传承军团" 的身份区分。
>
> `*` = 分支节点。tier 定值 root **T2** → 顶级 **T7**（决策 #4：T7 封顶，不铺 T8+）。**此节为精锐线权威结构**，§三/§四/§五 已废弃。对应原版帝国精锐线 `Vigla Recruit→Equite→Heavy Horseman→Cataphract→Elite Cataphract`。

### 拓扑一览（10 分支）
```
Thaddian Ephebos 军事学院新兵 (T2)*
├─A 骑兵 Kavallarios(T3)* → 重骑 Kataphraktos(T4)* → 具装骑 Klibanarios(T5)* → 重型具装 Klibanophoros(T6) → 御用 Basilikos Klibanophoros(T7)
│        │(骑兵*T3)   └─I 拉丁重骑 Frangos(T5) → Latinikos Kavallarios(T6) → Latinikoi Milites(T7)
│        │(重骑*T4)   └─F 法里斯 Faris(T5) → Faris Palaios(T6) → Faris Prodromos(T7)
│        │(具装*T5)   ├─D 库扎近卫 Vardariotes(T6) → Vardariotai Skythikoi(T7)
│        │(具装*T5)   └─E 马穆鲁克 Mameloukos(T6) → Mameloukos Furusi(T7)
├─B 重步 Skoutatos(T3)* → Hoplites(T4) → Klibanophoros Pezos(T5) → Aniketoi(T6) → Athanatoi(T7)
│        │(重步*T3)   ├─G 诺德斧步 Sekyriphoros(T5) → Sekyriphoros Palaios(T6) → Sekyriphoros Ulfhednar(T7)
│        │(重步*T3)   ├─H 罗斯亲兵 Rhos(T5) → Rhos Palaios(T6) → Rhos Bogatyr(T7)
│        │(重步*T3)   └─J 大刃兵 Rhomphaiaphoros(T5) → Rhomphaiaphoros Epilektos(T6) → Rhomphaiaphoros Areios(T7)
└─C 精锐弓 Toxotes Epilektos(T3)* → Makrotoxotes(T4) → Eustochos(T5) → Aristotoxos(T6) → Aristotoxos Oreinos(T7)
```

### T7 顶点二分（v0.4 决策产物）
- **纯帝国 3 顶**：A `Basilikos Klibanophoros`（御用）· B `Athanatoi`（不朽者）· C `Aristotoxos Oreinos`（至强弓手）
- **归化 7 顶**（各族独有精锐头衔）：D Skythikoi · E Furusi · F Prodromos · G Ulfhednar · H Bogatyr · I Milites · J Areios

### 根节点
- **Thaddian Ephebos** · 萨迪厄斯军事学院新兵 · T2 `*`
  - 典故：**Ephebeia（ἐφηβεία）= 古希腊城邦的国家青年军事训练制度**，字面即"军事学院学员/受训青年"，完美对应"军事学院新兵"。备选：`Scholarios Neaniskos`（御学少年卫）。

### A · 重骑兵主线（原版帝国具装骑延伸）
| In-game | 中文 | Tier | 典故 |
|---|---|---|---|
| Thaddian Kavallarios | 萨迪厄斯骑兵 | T3 | καβαλλάριος 骑兵 |
| Thaddian Kataphraktos `*` | 萨迪厄斯重骑兵 | T4 | κατάφρακτος 铁甲骑 |
| Thaddian Klibanarios `*` | 萨迪厄斯具装骑兵 | T5 | κλιβανάριος 人马全甲 |
| Thaddian Klibanophoros | 萨迪厄斯重型具装骑兵 | T6 | κλιβανοφόρος 窑炉重骑 |
| **Basilikos Klibanophoros** | 萨迪厄斯 klibanophoros（御用）| T7 | 御用顶级重骑 |

> klibanarios ≈ klibanophoros 史上是近义（皆人马全甲），故用**建制前缀分梯度**：具装=Klibanarios、重型=Klibanophoros、顶点=御用 Basilikos。

### B · 重步兵线（重骑线的下马步兵特化）
| In-game | 中文 | Tier | 典故 |
|---|---|---|---|
| Thaddian Skoutatos | 萨迪厄斯重步兵 | T3 | σκουτάτος 持盾重步 |
| Thaddian Hoplites | 萨迪厄斯精锐重步兵 | T4 | ὁπλίτης 精锐枪盾步（Menaulatos 已让给野战军反骑）|
| Thaddian Klibanophoros Pezos | 萨迪厄斯具装重步兵 | T5 | πεζός 下马具装步 |
| **Thaddian Aniketoi** | 萨迪厄斯常胜军 | T6 | ἀνίκητος 不败/常胜 |
| **Thaddian Athanatoi** | 萨迪尔斯不朽军 | T7 | Ἀθάνατοι 拜占真实精锐军团「不朽者」|

### C · 步射线（原版巴旦尼亚贵族弓延伸）
| In-game | 中文 | Tier | 典故 |
|---|---|---|---|
| Thaddian Toxotes Epilektos `*` | 萨迪厄斯精锐弓手 | T3 | ἐπίλεκτος 精选弓 |
| Thaddian Makrotoxotes | 萨迪厄斯精锐长弓手 | T4 | μακρός 长弓 |
| Thaddian Eustochos | 萨迪厄斯神射手 | T5 | εὔστοχος 弹无虚发 |
| Thaddian Aristotoxos | 萨迪厄斯神射冠军 | T6 | ἄριστος+τόξον 至强弓手 |
| **Thaddian Aristotoxos Oreinos** | 萨迪厄斯高地神射冠军 | T7 | ὀρεινός 高地，呼应巴旦血统 |

### D · 库扎伊特近卫骑（从 具装骑兵`*` 分出 · 库兹特贵族之子）
| In-game | 中文 | Tier | 典故 |
|---|---|---|---|
| Thaddian Vardariotes | 萨迪厄斯近卫骑军 | T6 | Βαρδαριῶται 拜占**真实存在**的突厥裔皇家近卫骑射，完美对应"草原贵族入近卫" |
| **Vardariotai Skythikoi** | 萨迪厄斯斯基泰式瓦尔达尔骑军 | T7 | Σκυθικόν 拜占庭对突厥系辅助军团的正式称号 · 非御用（决策 #5：外族不列御用） |

### E · 阿塞莱马穆鲁克弓骑（从 具装骑兵 分出）
| In-game | 中文 | Tier | 典故 |
|---|---|---|---|
| Thaddian Mameloukos | 萨迪厄斯马穆鲁克骑兵 | T6 | 马穆鲁克弓骑 |
| **Mameloukos Furusi** | 萨迪厄斯精锐马穆鲁克骑兵 | T7 | فروسية Furusiyya = Mamluk 骑士术传统 · 精英骑士素养 · 非御用（决策 #5） |

### F · 阿塞莱法里斯冲击/标枪骑（从 重骑兵`*` 分出 · 阿塞莱青年军）
| In-game | 中文 | Tier | 典故 |
|---|---|---|---|
| Thaddian Faris | 萨迪厄斯法里斯 | T5 | 保留 Faris 身份词（同 Mameloukos 处理）|
| Thaddian Faris Palaios | 萨迪厄斯法里斯老兵 | T6 | παλαιός 老练 |
| **Thaddian Faris Prodromos** | 萨迪厄斯先锋法里斯 | T7 | Πρόδρομοι 拜占**真实**先锋轻枪骑，正合"先锋" · 非御用 |

### G · 诺德归化 · 双手斧重步（从 重步`*T3` 分出 · v0.4 新增 · 决策 #5 归化四族）
| In-game | 中文 | Tier | 典故 |
|---|---|---|---|
| Thaddian Sekyriphoros | 萨迪厄斯诺德斧步 | T5 | σεκυριφόρος 持斧者（与皇家卫队 Pelekyphoros 拉开：Pelek 大斧、Sekyr 单手斧/战斧）|
| Thaddian Sekyriphoros Palaios | 萨迪厄斯诺德斧步老兵 | T6 | παλαιός 老练 |
| **Thaddian Sekyriphoros Ulfhednar** | 萨迪厄斯诺德狼皮斧步 | T7 | Úlfhéðnar 北欧真实狼皮战士传统（狂战一支）· 族源精锐 · 非御用 |

### H · 斯特吉亚归化 · 罗斯亲兵重步（从 重步`*T3` 分出 · v0.4 新增）
| In-game | 中文 | Tier | 典故 |
|---|---|---|---|
| Thaddian Rhos | 萨迪厄斯罗斯武士 | T5 | Ῥῶς 拜占**真实存在**的罗斯佣兵词根 |
| Thaddian Rhos Palaios | 萨迪厄斯罗斯亲兵 | T6 | παλαιός 老练 · 呼应 druzhina 亲兵传统 |
| **Thaddian Rhos Bogatyr** | 萨迪厄斯罗斯勇士 | T7 | Богатырь 罗斯民间英雄传统（bogatyr 波加特尔勇士）· 族源精锐 · 非御用 |

### I · 瓦兰迪亚归化 · 拉丁重骑（从 骑兵`*T3` 分出 · v0.4 新增 · 弩已在基础常备军 C 弩线覆盖，此支只做重骑）
| In-game | 中文 | Tier | 典故 |
|---|---|---|---|
| Thaddian Frangos | 萨迪厄斯法兰克重骑 | T5 | Φράγγοι 拜占对法兰克/西欧人的称呼 |
| Thaddian Latinikos Kavallarios | 萨迪厄斯拉丁重骑 | T6 | Λατινικόν 拜占**真实**拉丁尼康重骑军团 |
| **Latinikoi Milites** | 萨迪厄斯拉丁精锐骑 | T7 | Milites 拉丁"精锐战士"（中世纪指骑士）· 呼应决策 #1 "偏希腊但拉丁可留" · 非御用 |

### J · 巴旦尼亚归化 · 大刃兵（从 重步`*T3` 分出 · v0.4 新增 · 林弓已在 C 线 Aristotoxos Oreinos T7 覆盖，此支只做大刃）
| In-game | 中文 | Tier | 典故 |
|---|---|---|---|
| Thaddian Rhomphaiaphoros | 萨迪厄斯大刃兵 | T5 | ῥομφαία 色雷斯大砍刀 = 巴旦 falx |
| Thaddian Rhomphaiaphoros Epilektos | 萨迪厄斯大刃精锐 | T6 | ἐπίλεκτος 精选 |
| **Thaddian Rhomphaiaphoros Areios** | 萨迪厄斯战神大刃 | T7 | Ἄρειος 战神阿瑞斯的形容词（"如战神般"）· 族源精锐 · 非御用 |

---

## ★ 皇家卫队（Hetairoi · Retinues 御林军 · 2026-10-08 用户换阵定稿）

> **v0.5 换阵**（2026-10-08 用户决策）：旧瓦兰吉卫队（Pelekyphoros 御斧卫 + Varangos 卫兵）**作废**。新编制 = **伙伴骑兵 + 皇家精锐重型步兵**双槽配对——亚历山大马其顿两大旗舰单位（伙伴骑 Hetairoi + 持盾精锐步 Hypaspistai）直接对应 Retinues 的 Champion/Guard 两槽。
>
> **史实呼应**：Hetairoi（ἑταῖροι "伙伴"）= 马其顿亚历山大的国王贴身重骑；Hypaspistai（ὑπασπισταί "持盾者"）= 亚历山大的精锐持盾步兵近卫。两单位出自同一战役组合、天然配对。拜占庭 Hetaireia 御从团继承 Hetairoi 一线、同根。
>
> **诺德归宿迁移**：原瓦兰吉卫队承担的"诺德文化归宿"角色 · v0.5 下**完全由精锐线 G 支 Sekyriphoros / Sekyriphoros Ulfhednar 承担** + 新常备军 G 诺德分支（见下方 ★ 基础主线 v0.5 重构）· 皇家卫队槽**专属希腊纯正**。

| Retinues 槽 | In-game | 中文 | 说明 |
|---|---|---|---|
| RetinueElite（House Champion）| **Thaddian Hetairos** | 萨迪厄斯伙伴骑 | 皇帝贴身精锐重骑 · 1v1 冲击冠军（ἑταῖρος 伙伴）· 对应 Champion 的"顶级单单位"定位 |
| RetinueBasic（House Guard）| **Thaddian Hypaspistes** | 萨迪厄斯皇家持盾卫 | 皇帝贴身精锐重步 · 盾墙 + 长枪 · 守护 Champion 两翼（ὑπασπιστής 持盾者）|

- **标志装备**：
  - Hetairos：重甲冲击骑 · xyston / kontos 冲击长矛 + 单手剑 + 大盾（圆形 pelta 或椭圆 thureos）· 具装战马
  - Hypaspistes：重甲重步 · 长矛（持盾者原生武器）+ 单手剑 + aspis 大圆盾
- **命名基调一致性**：两名都走纯希腊古词源 · 跟 v0.4 决策 #1 "偏希腊 · 不转硬拉丁" 一致 · 跟精锐线 T7 顶点无撞车（精锐 A 支 T7 = Basilikos Klibanophoros · 具装御骑 · 本节 Hetairos 是无 Basilikos 前缀的伙伴骑 · 定位不同）
- ⚠ 玩家称帝后 Retinues 会自动把名字改成 King's/Queen's Champion + Royal Guard（`TroopBuilder.MakeRetinueName`）——想固定"Hetairos / Hypaspistes"名需在 Retinues 里手设并**实测是否被自动改名覆盖**（blocker 跟旧瓦兰吉版本一致 · 换阵不影响这个坑）
- ⚠ 旧 Pelekyphoros / Varangos 名字词根**彻底移出皇家卫队** · 但 Pelekyphoros 词根（持大斧者）可挪到新常备军 G 诺德分支作兵种名 · 待 ★ 基础主线 v0.5 重构时定

---

## ★ 基础主线定稿（Thaddeus 常备军 / Numeri · v0.5 · 2026-10-08 α 熔炉吸纳重构）

> **v0.5 重构定案**（2026-10-08 用户选 α 熔炉吸纳 · Caracalla 公民权模型）：常备军**抛弃 v0.3 的"希腊-拉丁 + 库扎伊特步行铁浮屠"的"拜占庭+补丁"模式** · 转为 **4 分支 24 兵种全帝国通用名** · 外族概念在常备军 XML 层**消失**（所有兵都是 `culture="Culture.thaddian"`）· 族源词根（Varangos/Rhos/Frangos/Vardariotes/Mameloukos/Faris/Sekyriphoros 等）**全留给精锐线 T6-T7 作传承军团** · 常备军真正做到 "标准职业军"。
>
> **跟 v0.3 的核心差异**：
> - **A 支** 收编（砍 Skirites 陷阵/Phalangites 方阵/Panklibanos Pezos 步行铁浮屠 · 全并入 Legionarius 野战军一条路径）
> - **B 重骑线新增**（Kavallarios → Prokoursator → Kataphraktos）· 补 v0.3 缺的骑兵
> - **C 远程线保留**（v0.3 原 B/C/D 弓/弩/连弩合为 C 一条）
> - **D 骑射线新增**（Hippotoxotes/Skythikon/Mardaitos 全希腊词 · 代表熔炉吸纳的草原传统 · 不叫 Tourkopoulos/Vardariotes）
>
> 后缀规则：**卫队**=`Phylax`（φύλαξ 卫）· **精锐/重**=`Epilektos / Barys` · **御营**=`Palatinos`。root **T1** · 主线到 **T5-T6** · 分支节点标 `*`。

### 拓扑（v0.5 · 4 分支 24 兵种）
```
Thaddian Neosyllektos 征召兵 (T1)*
│
├─A 持盾长枪重步线（守备 + 野战突击双路径）
│   Kontaratos 枪兵(T2) → Thyreophoros 持盾步(T3)* → Thyreophoros Phylax 盾卫(T4)*
│                                                   ├─(G1) Legionarius 野战军团(T5) → Palatinus 御营野战(T6)
│                                                   └─(G2) Menaulatos 反骑长柄(T5) → Menaulatos Palatinos(T6)
│
├─B 冲击重骑线【新增】
│   Thaddian Kavallarios 帝国骑兵(T2) → Prokoursator 先驱骑(T3)* → Thaddian Kataphraktos 铁甲骑(T5)
│
├─C 远程线（弓/弩/连弩 三子段）
│   Toxotes Dokimos 受训弓(T2) → Toxotes 弓兵(T3)* → Toxotes Phylax 弓卫(T4) → Toxophylax 长弓卫(T5)
│   └(弓兵*) Tzangrator 弩手(T4)* → Tzangrator Barys 重弩手(T5) → Tzangrator Phylax 重弩卫(T6)
│            └(弩手*) Polybolos 连弩手(T5) → Polybolos Phylax 重连弩卫(T6)
│
└─D 骑射/标枪骑线【新增 · 熔炉吸纳草原传统 · 全希腊词】
    Prokoursator Elaphros 轻先驱(T2) → Hippotoxotes 帝国骑射(T3)* → Skythikon 草原军团骑射(T5)
    └(帝国骑射*) Akontistes Hippikos 骑标枪(T4) → Mardaitos 帝国边军(T5)
```

**兵种总数**：1 根 + A 7 + B 3 + C 9 + D 5 = **24 支** (含分支节点)

**跟 v0.3 的删除/新增 diff**：
- ❌ **删**：E 陷阵（Skirites/Panklibanos Pezos）· F 方阵（Phalangites/Phalangites Barys）· 原 B 弓/C 弩/D 连弩**分支号重编为 C 一条线**
- ✅ **保留**：A 枪盾 3 支 · G 野战军 4 支（Legionarius/Palatinus/Menaulatos/Menaulatos Palatinos）· 原 B/C/D 远程 9 支
- ➕ **新增**：**B 重骑线 3 支** · **D 骑射线 5 支**（Prokoursator Elaphros 为新词 · Hippotoxotes/Skythikon/Akontistes Hippikos/Mardaitos）

### 根节点
| In-game | 中文 | Tier | 典故 |
|---|---|---|---|
| Thaddian Neosyllektos `*` | 萨迪厄斯征召兵 | T1 | νεοσύλλεκτος 新募 |

### A · 持盾长枪重步线（防御守备 + 野战突击 · 双顶点路径）

> **教条**：A 支 T2-T4 为**守备盾墙**（长枪 + 大盾） · T5-T6 分两条出路：**G1 Legionarius/Palatinus** 走拉丁 register "进攻突击"（短剑 + scutum + pila 破阵）· **G2 Menaulatos/Menaulatos Palatinos** 走希腊词 "反骑锚点"（menaulion 反骑粗矛 + 大盾）。**三级升华（皆真实建制）**：盾卫 T4 → 野战军 T5 → 御营 T6 · T7 留给精锐线 Tagmata。
>
> **α 熔炉吸纳下的重编**：原 v0.3 的 E 陷阵（Skirites/Panklibanos Pezos）+ F 方阵（Phalangites/Phalangites Barys）**全部砍除**（库扎伊特步行铁浮屠的"外族试验兵"概念在 α 下消失） · 其战术位（反骑硬墙 + 重阵压制）由 G2 Menaulatos Palatinos 一支收编。

| In-game | 中文 | Tier | 装备(定) | 典故 |
|---|---|---|---|---|
| Kontaratos | 常备军枪兵 | T2 | 长枪 + 小盾 | κοντάρι 长枪 |
| Thyreophoros `*` | 常备军持盾步兵 | T3 | 长枪 + 大盾 | θυρεός 大盾持盾步 |
| Thyreophoros Phylax `*` | 常备军盾卫 | T4 | 长枪 + 大盾 | φύλαξ 卫；顶点分出 G1 Legionarius / G2 Menaulatos |
| Legionarius | 野战军团步兵 (G1 T5) | T5 | 单手剑 + 大盾(scutum) + pila 重标枪 | 拉丁 legionarius；野战突击破阵主力 |
| **Palatinus** | 野战御营军团步兵 (G1 T6) | T6 | 精甲 + 单手剑 + 大盾 + pila | Palatini 御营精锐野战军团；**职业军 G1 顶点** |
| Menaulatos | 野战长柄步兵 (G2 T5) | T5 | menaulion 反骑粗矛 + 大盾 | μεναύλατος 反重骑 · 收编原 Skirites/Phalangites 战术位 |
| **Menaulatos Palatinos** | 野战御营长柄步兵 (G2 T6) | T6 | 精甲 + menaulion + 大盾 | 御营反骑锚点；**职业军 G2 顶点** |

> **Hastati / Principes / Triarii 不用**：它们是原版**"背弃军团"(Legion of the Betrayed)** 的兵（`legion_of_the_betrayed_tier_1/2/3`，被放逐旧帝国残党用的共和罗马三线古名），**非 Thaddian 制式**。萨迪厄斯军改用职业野战军团（Legionarius + Menaulatos）取代旧三线；三线只作为"背弃军团"遗民残存于世界，整编派可招其为传统主义雇佣辅助。

### B · 冲击重骑线（新增 · 补 v0.3 缺的骑兵 · 全帝国通用名）

> **α 核心**：常备军重骑**不走族源分支**（库扎重骑、马穆鲁克、法里斯、法兰克重骑 · 全留给精锐线 D/E/F/I T6-T7）· 常备军 B 支就是"帝国标准重骑" · 用希腊通用词 Kavallarios/Kataphraktos · 教条学院步兵一样是"职业军帝国"的马匹版本。
>
> **跟精锐线 A 支的关系**：精锐 A 支 Kavallarios(T3) → Kataphraktos(T4) → Klibanarios(T5) → Klibanophoros(T6) → Basilikos Klibanophoros(T7) · 常备军 B 支只到 Kataphraktos(T5) · T5 封顶 · 把 Klibanarios/Klibanophoros 的具装档位留给精锐 A 支 · 兵种名刻意重叠（T2 Kavallarios / T5 Kataphraktos）暗示"精锐 A 从常备军 B 提拔"。

| In-game | 中文 | Tier | 装备(定) | 典故 |
|---|---|---|---|---|
| Thaddian Kavallarios | 常备军帝国骑兵 | T2 | 中甲 + 骑枪 + 单手剑 + 中盾 | καβαλλάριος 骑兵 · 跟精锐 A T3 Thaddian Kavallarios 同名（精锐从常备军提拔）|
| Prokoursator `*` | 常备军先驱骑 | T3 | 中甲 + kontos 骑枪 + 单手剑 + 中盾 | προκουρσάτωρ 前哨侦骑 |
| **Thaddian Kataphraktos** | 常备军铁甲骑兵 | T5 | 重甲 + kontos 骑枪 + 单手剑 + 中盾 + 中甲马 | κατάφρακτος 铁甲冲骑 · 常备军 B 顶点；具装档位（Klibanarios/Klibanophoros）留给精锐 A |

### C · 远程线（弓/弩/连弩 三子段 · 保留 v0.3 现状 · α 下无改动）

> v0.3 的 B 弓 / C 弩 / D 连弩 三条线合并编号为 C 一条远程大线。希腊词池完整 · α 下保留。

#### C1 · 弓兵子段
| In-game | 中文 | Tier | 装备(定) | 典故 |
|---|---|---|---|---|
| Toxotes Dokimos | 常备军受训弓兵 | T2 | 布甲 + 弓 + 箭袋 | δόκιμος 受训 |
| Toxotes `*` | 常备军弓兵 | T3 | 皮甲 + 弓 + 箭袋 | τοξότης |
| Toxotes Phylax | 常备军弓卫 | T4 | 皮甲 + 弓 + 箭袋 | 弓手卫 |
| Toxophylax | 常备军长弓卫 | T5 | 中甲 + 单手剑 + 弓 + 箭袋×2 | τοξοφύλαξ 弓卫 |

#### C2 · 弩手子段（弓兵`*`T3 分支）
| In-game | 中文 | Tier | 装备(定) | 典故 |
|---|---|---|---|---|
| Tzangrator `*` | 常备军弩手 | T4 | 皮甲 + 弩 + 弩箭袋 | τζαγγρᾶτορες 拜占庭**真实**弩兵词（tzangra=弩）|
| Tzangrator Barys | 常备军重弩手 | T5 | 中甲 + 重弩 + 弩箭袋 | βαρύς 重 |
| Tzangrator Phylax | 常备军重弩卫 | T6 | 中甲 + 单手 + 大盾 + 弩 + 弩箭袋 | 持盾重弩卫 |

#### C3 · 连弩子段（弩手`*`T4 分支）
| In-game | 中文 | Tier | 装备(定) | 典故 |
|---|---|---|---|---|
| Polybolos | 常备军连弩手 | T5 | 皮甲 + 单手 + 连弩 + 弩箭袋 | πολυβόλος 古希腊连发弩炮 · 正合"连弩" |
| Polybolos Phylax | 常备军重连弩卫 | T6 | 中甲 + 单手 + 连弩 + 弩箭袋×2 | 重连弩卫 |

### D · 骑射/标枪骑线（新增 · α 熔炉吸纳的草原传统 · 全希腊词 · 不叫 Tourkopoulos/Vardariotes）

> **α 核心**：常备军骑射**不走库扎草原族源名**（Tourkopoulos/Hippotoxotes Skythikos/Vardariotes 等族源词 · 全留给精锐线 D 支 T6-T7） · 常备军 D 支用**帝国希腊通用词**（Hippotoxotes 字面"马上弓手" · Skythikon 字面"斯基泰式军团" 但在拜占庭语境里已是**军团建制名**而非族名 · 跟 Legionarius 一样）。
>
> **历史依据**：Σκυθικόν（Skythikon）在拜占庭史里是**帝国正规军军团**（东线骑射军团）· 哪怕成员多半是突厥裔也早已归化为帝国公民 · 完美对应 α 熔炉。Mardaitai（马尔达伊特）同理：边境定居的阿拉伯裔但已是帝国正规军边军建制。
>
> **跟精锐线 D 支的关系**：精锐 D 支 Vardariotes(T6) → Vardariotai Skythikoi(T7) 是**顶级传承军团** · 保留族源味 · 常备军 D 支 Hippotoxotes/Skythikon 是**标准职业骑射** · 两者映射"职业军 → 传承御卫"的升华链。

| In-game | 中文 | Tier | 装备(定) | 典故 |
|---|---|---|---|---|
| Prokoursator Elaphros | 常备军轻先驱骑 | T2 | 轻甲 + 弓 + 单手剑 + 小盾 | ἐλαφρός 轻装 · 跟 B 支 Prokoursator T3 区分（Elaphros=轻装先驱 · 走骑射流） |
| Hippotoxotes `*` | 常备军帝国骑射 | T3 | 皮甲 + 弓 + 单手剑 + 箭袋 | ἱπποτοξότης 字面"马上弓手" · 帝国通用词 |
| **Skythikon** | 常备军草原军团骑射 | T5 | 中甲 + 弓 + 单手剑 + 箭袋×2 | Σκυθικόν 拜占庭**真实**东线骑射军团建制 · 军团名非族名 · α 下合理 |
| Akontistes Hippikos | 常备军骑标枪兵 | T4 | 皮甲 + 单手剑 + 投枪×多 + 小盾 | ἀκοντιστής ἱππικός 马上标枪散兵 · 帝国骑射*T3 分支 |
| **Mardaitos** | 常备军帝国边军 | T5 | 中甲 + 单手剑 + 投枪 + 弓(可选) + 中盾 | Μαρδαΐται 拜占庭**真实**边军建制（阿拉伯裔归化定居边军）· α 熔炉吸纳完美例 |

---

## 三、四、五（已废弃 · 2026-10-08 用户决策）

> **章节废弃说明**：
> - **原 §三 "核心帝国军团"（旧 v0.1 拜占庭正统命名池）**、**§四 "融合军团"（旧 v0.1 库扎伊特/阿塞莱融合池）**、**§五 "蛮族外籍军团 Foederati"** 三节于 **2026-10-08 用户明示作废** · 内容已不再承担任何权威角色。
> - **废弃原因**：
>   1. §三/§四 的词池在 2026-09-24 v0.4 定案时已被 **★ 精锐主线定稿**（10 分支 T2-T7 完整体系）+ **★ 基础主线定稿**（常备军 Numeri）取代 · 自身已标 "备选参考"
>   2. §五 Foederati 框架在 2026-09-24 决策 #5 "帝国无外籍军团" 已标废
>   3. 2026-10-08 用户决策：**常备军抛弃单纯拜占庭+辅助军模式** → 新 ★ 基础主线 v0.5 重构（见下方 §九）将走**多文化整合路线** · 与精锐线 10 分支平起平坐（tier 深度区分 · 文化层区分不再是常备军/精锐的分野）
>   4. 皇家卫队 2026-10-08 换阵：瓦兰吉卫队 → Hetairos + Hypaspistes（见上方 ★ 皇家卫队节）· §五 瓦兰吉词池完全出局皇家卫队
> - **仍保留的词根**（供下方新常备军 v0.5 重构参考）：Pelekyphoros（持大斧者）/ Rhos（罗斯）/ Rhomphaiaphoros（大刃）/ Frangos（法兰克）/ Tzangrator（弩手）/ Oreinos（山地）/ Toxotes（弓手）等希腊词根**依然有效** · 作为 v0.5 多文化整合的命名词源池 · 新常备军分支按需引用
> - **git 历史保全**：被删内容在 `git log` 中仍可追溯 · 本次编辑等于"软归档"

<!-- 原 §三.1~§三.4 / §四.1~§四.2 / §五.1~§五.4 内容已删除 · 废弃说明在上
     恢复节号从 §六 开始（§六 命名速查 / §七 定夺 / §八 开放结构性问题 均保留原节号） -->

---

## 六、命名速查（一句话记忆）

> ⚠ **本节为旧通用命名池速查（v0.1 时代）· 已被 v0.4 精锐主线 10 分支 + 基础主线 + 皇家卫队取代**。查现行体系请以文首"★ 精锐主线定稿 v0.4" 拓扑图 + T7 二分表为准；此处保留仅作词根参考。

### 现行速查（v0.5 · 2026-10-08 更新）
- **精锐 10 支 T7 顶点**：Basilikos Klibanophoros / Athanatoi / Aristotoxos Oreinos / Vardariotai Skythikoi / Mameloukos Furusi / Faris Prodromos / Sekyriphoros Ulfhednar / Rhos Bogatyr / Latinikoi Milites / Rhomphaiaphoros Areios
- **基础常备军**：v0.3 pending 重构 v0.5（多文化整合路线 · 待下 session 定稿） · v0.3 原步兵顶 Palatinus/Menaulatos Palatinos 作参考
- **皇家卫队**（2026-10-08 换阵）：**Thaddian Hetairos（Champion · 伙伴骑）· Thaddian Hypaspistes（Guard · 皇家持盾卫）** · 旧 Pelekyphoros/Varangos 已出局

### 旧通用池速查（保留作词根参考 · 对应 §三 §四 §五 废弃节）
- **步兵**：Skoutatos → Hoplites → Menaulatos → **Scholarios**
- **弓兵**：Psilos → Toxotes → Sagittarios → **Vigla Toxotes**
- **重骑**：Kavallarios → Kataphraktos → Klibanophoros → **Hetairos**（词根 · 作精锐线 A 支 T4 Kataphraktos 等的典故来源 · 不再是重骑顶）
- **骑射（库扎伊特）**：Tourkopoulos → Hippotoxotes → **Vardariotes**
- **沙漠（阿塞莱）**：Mardaites → Mameloukos → **Athanatos**
- **诺德**：Varangos → Pelekyphoros → 御斧卫（旧瓦兰吉卫队体系 · 2026-10-08 词根下放到常备军 G 诺德分支待用）
- **斯特吉亚**：Rhos → 罗斯卫队
- **瓦兰迪亚**：Frangos → Latinikon Kavallarios（+ Tzangrator 弩）
- **巴旦尼亚**：Keltos → **Rhomphaiaphoros**

---

## 七、待你定夺（命名方向的分叉）· ✅ 全部已定案

> **2026-09-24 定案** — 5 问全部由用户拍板，见文首 "★ 命名基调决策（v0.4 定案）" 表。本节保留原问题以留痕，答案汇总：
>
> 1. **希腊 vs 拉丁** → **偏希腊、不转硬拉丁**（现存 Legionarius/Palatinus 保留但不扩张）
> 2. **重命名 vanilla 帝国兵** → **否**（仅 Retinues + 玩家 clan 自定义兵）
> 3. **帝国形容词** → **`Thaddian`**
> 4. **Tier 深度** → **T7 封顶**（不铺 T8-T10）
> 5. **外籍军团** → **无**（归化为帝国建制；§五 待重构；见 §八）

---

## 八、开放结构性问题（决定 #5 引出 · 待用户拍板）

**问题**：诺德/斯特吉亚/瓦兰迪亚/巴旦尼亚这四族的兵，既然**归化为帝国建制**（决定 #5），它们该以什么**结构位置**存在？

**已有的归化范例**（在精锐主线 Tagmata 里）：
- D 库扎伊特近卫 = 具装骑兵 T5 分出 → Vardariotes T6 → Basilikos Vardariotes T7
- E 阿塞莱马穆鲁克 = 具装骑兵 T5 分出 → Mameloukos T6 → Basilikos Mameloukos T7
- F 阿塞莱法里斯 = 重骑兵 T4 分出 → Faris T5 → Faris Palaios T6 → Faris Prodromos T7

即"精锐线内接一条归化分支 · 保留族源身份词 + 帝国 Basilikos 前缀顶点"。

**三条路让你选**：

| 选项 | 做法 | 优 | 劣 |
|---|---|---|---|
| **A. 并入精锐线**（延用 D/E/F 模式） | 精锐 Tagmata 加 G/H/I/J 四支分支：诺德归化（斧步）/ 罗斯归化（斧枪）/ 拉丁归化（重骑/弩）/ 高地归化（大刃/林弓）· 分支节点从精锐 root Ephebos 下的 A 骑/B 步/C 弓 分出 | **结构统一** · 与库扎/阿塞莱同规格 · 完全体现"归化 = 精锐一部分" | 精锐线膨胀（6→10 分支）· 每分支只能到 T7 有限深度 |
| **B. 独立成"归化线"**（并列于常备军 Numeri / 精锐 Tagmata） | 新建第三条主线 "**Thaddian Symmachiarioi 帝国归化军团**"（Συμμαχιάριοι = 拜占实际存在的归化协同军词）· 4 族各成独立子树 root T2 → 顶点 T7 | 分类清晰 · 归化 = 独立建制层 · 4 族有铺陈空间 | 与 D/E/F 精锐里已归化的库扎/阿塞莱**语义冲突**（为什么库扎在精锐、诺德在归化线？）除非把 D/E/F 也搬过来 |
| **C. 塞进基础常备军 Numeri 做辅助** | 4 族成为常备军的辅助支（步兵/骑兵之外的第三类）· 定位在 Numeri 建制内、非精锐 | 呼应"常备军 = 帝国骨干"的定位 · 归化就是普通建制 | 削弱族源特色（诺德斧步塞进 Numeri 显得平淡）· 与已有精锐 D/E/F 的高档归化冲突 |

**我倾向 A**（并入精锐线）：
- 与已有 D/E/F 库扎伊特/阿塞莱归化分支**完全同构**、语义一致
- 保持精锐 Tagmata 作为"帝国最高战力集合"的定位
- 4 族各出**一条或两条**分支（不必每族都到 T7 · 有些可以只到 T6）
- Varangian Guard 保持"皇家卫队"独立于此（史实定位）
- 命名照旧："族源身份词根" + Thaddian/Basilikos 建制前缀（照抄 D/E/F 的模式）

如果走 A · 需要你定的下一层细节：
1. 每族取几条分支（1 条 or 2 条）
2. 每族的核心兵种概念（诺德斧步是必然 · 罗斯是斧或长矛 · 瓦兰迪亚是重骑或弩 · 巴旦尼亚是大刃或林弓）
3. 每分支从精锐 root 的哪个节点分出（A 骑 / B 步 / C 弓 · 或者独立 root）

---

*草案由 Claude Code 协助生成 2026-09-20；v0.4 命名基调决策 2026-09-24。命名以拜占庭军事术语为骨架。*
