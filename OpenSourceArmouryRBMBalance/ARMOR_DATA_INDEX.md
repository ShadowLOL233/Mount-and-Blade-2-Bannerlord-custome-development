# 护甲数据总表 · 索引

**生成日期**：2026-10-10
**覆盖范围**：vanilla（Native）+ RBM v4.5.0 override + OSA v2.0.0 + OSW v2.0.1 全量合并
**合并后总条目**：**2913 件护甲**（跨 5 槽 · 10 文化 · T-1 到 T5）

---

## 5 槽拆分文档

| 槽位 | 中文名 | 文档 | 件数 | T4+ 件数 |
|---|---|---|---:|---:|
| HeadArmor | 头盔 | [`ARMOR_DATA_HEAD.md`](./ARMOR_DATA_HEAD.md) | 1328 | 772 |
| BodyArmor | 身甲 | [`ARMOR_DATA_BODY.md`](./ARMOR_DATA_BODY.md) | 855 | 416 |
| Cape | 肩甲 / 披风 | [`ARMOR_DATA_SHOULDER.md`](./ARMOR_DATA_SHOULDER.md) | 474 | 54 |
| HandArmor | 臂甲 / 手甲 | [`ARMOR_DATA_ARM.md`](./ARMOR_DATA_ARM.md) | 117 | 28 |
| LegArmor | 腿甲 | [`ARMOR_DATA_LEG.md`](./ARMOR_DATA_LEG.md) | 139 | 19 |

**T4+ = Tier 4 和 5 件数**（精锐 / 顶级档位 · 玩家 endgame 装备的主要池子）。

---

## 数据来源分布

| Source | 含义 | 头盔 | 身甲 | 肩甲 | 臂甲 | 腿甲 | 合计 |
|---|---|---:|---:|---:|---:|---:|---:|
| **Vanilla** | RBM / OSA 都没碰的 Native 原值 | 203 | 193 | 53 | 30 | 30 | **509** |
| **RBM** | RBM v4.5.0 整节替换过的 vanilla | 282 | 274 | 111 | 41 | 48 | **756** |
| **OSA** | OSA / OSW 新增 或 覆盖的物品 | 843 | 388 | 310 | 46 | 61 | **1648** |
| **合计** | | 1328 | 855 | 474 | 117 | 139 | **2913** |

**读法**：
- `Src=Vanilla` 行 = 玩家装了 RBM / OSA 还是看到 vanilla 原值的物品（通常是 MP-only 或冷门条目）
- `Src=RBM` 行 = RBM 已经调整过数值 · 这是玩家实际看到的"修正后 vanilla"
- `Src=OSA` 行 = OSA/OSW 补的新物品（大部分）或 OSA 又覆盖了 RBM 的少数 case

---

## 文化分布

| Culture | 件数 | 占比 |
|---|---:|---:|
| empire | 526 | 18.1% |
| battania | 458 | 15.7% |
| aserai | 435 | 14.9% |
| vlandia | 433 | 14.9% |
| neutral_culture | 348 | 11.9% |
| sturgia | 301 | 10.3% |
| khuzait | 301 | 10.3% |
| nord | 78 | 2.7% |
| looters | 30 | 1.0% |
| (none) | 3 | 0.1% |

`neutral_culture` 是 Native 的"文化中立"池（looter / bandit / 通用装）· `nord` 是 NavalDLC（Viking Conquest）相关资产 · `looters` 是 bandit 专用。

---

## Tier 公式速查（vanilla `DefaultItemValueModel.CalculateArmorTier`）

```
raw    = 1.2·head + body + leg + arm
scaled = raw × typeMultiplier
tier   = clamp(round(scaled × 0.1 − 0.4), 0, 6) − 1
```

| 槽位 | typeMultiplier |
|---|---:|
| Cape（肩甲） | **1.8** · 最高 |
| HandArmor（臂甲）| 1.7 |
| LegArmor（腿甲） | 1.6 |
| HeadArmor（头盔） | 1.2 |
| BodyArmor（身甲）| **1.0** · 最低 |

**含义**：同样 raw armor 下 · 肩甲档位最容易上 T5 · 身甲最难（身甲数值密度要求最高）。

---

## Tier 全槽分布

| Tier | 头盔 | 身甲 | 肩甲 | 臂甲 | 腿甲 | 合计 | 占比 |
|---|---:|---:|---:|---:|---:|---:|---:|
| T5 | 593 | 305 | 29 | 20 | 13 | 960 | 33.0% |
| T4 | 179 | 111 | 25 | 8 | 6 | 329 | 11.3% |
| T3 | 175 | 82 | 143 | 19 | 22 | 441 | 15.1% |
| T2 | 106 | 90 | 41 | 31 | 33 | 301 | 10.3% |
| T1 | 157 | 119 | 92 | 8 | 24 | 400 | 13.7% |
| T0 | 55 | 83 | 112 | 15 | 17 | 282 | 9.7% |
| T-1 | 63 | 65 | 32 | 16 | 24 | 200 | 6.9% |

**观察**：
- **头盔池偏重装化**（T5=44.7% · T4+=58.1%）· OSA 极大扩充了精锐头盔 variant
- **身甲池 T5 密集**（305 / 855 = 35.7%）· RBM 已把许多 mail / lamellar 身甲提到 T5 档
- **肩甲池偏轻装化**（T5+T4=11.4% · T1 以下 59.5%）· 大量披风 / 肩披是装饰性低防护
- **臂甲池 T2-T3 居多**（50/117 = 42.7%）· T5 20 件是重装手套精锐档
- **腿甲池数值集中低档**（T0+T-1=29.5% · T1 以下 46.0%）· 多数腿甲走轻装靴底

---

## 数据管线

```
Vanilla_Reference/data/vanilla_items.csv   (Native)
RBM_Reference/data/rbm_items.csv            (workshop 2859251492 + 3635788184)
OSA_Reference/data/osa_items.csv            (workshop 3011479883 + 3010984416)
    |
    v
src/extract_armor_tables.ps1               (合并 3 CSV · 算 tier · 分 5 槽 JSON)
    |
    v
src/armor_data_{head,body,shoulder,arm,leg,all}.json
    |
    v
src/build_armor_md.ps1                      (JSON -> ASCII 表格)
    |
    v
src/armor_tables_{head,body,shoulder,arm,leg}.md
    |
    v
src/compose_armor_docs.ps1                  (Chinese 前言 + ASCII 表格拼接)
    |
    v
ARMOR_DATA_{HEAD,BODY,SHOULDER,ARM,LEG}.md  (本仓库根)
```

**重跑**（CSV 更新后）：
```powershell
& src/extract_armor_tables.ps1
& src/build_armor_md.ps1
& src/compose_armor_docs.ps1
```

---

## 配合 OSA v3 跨文化同材质统一（P0 立项）

本总表的直接用途是支撑 ModdingJournal §P0 OSA v3 (2026-10-09 立项) 的扫描阶段：

1. **扫描阶段**：以 (slot, materialType, tier) 为 key 分组 · 看每组内 7 文化 head/body/arm/leg 四围 variance
2. **变异识别**：组内差 > 5 点的 instance 直接列入 `OSA_V3_CROSS_CULTURE_SCAN.md` 决议清单
3. **保留 override**：Khuzait 华夏 / Aserai 波斯 / Nord 维京的精工线用户已明示保留

**scope 边界**（跟 Shield 工作 pipeline 独立）：
- 本表只覆盖 **5 件护甲** · 不含盾（见 `SHIELD_DATA_COMPLETE.md`）
- 本表只覆盖**非马具护甲** · 不含 HorseHarness（如要可同样 pipeline 扩）

---

## 使用建议

**查某件具体护甲**：
1. 按槽选对应文档
2. `Ctrl+F` 搜 StringId 或显示名
3. 旁边列直接读 Tier / Price / h/b/a/l

**找同文化同 tier 的护甲对比**：
1. 按槽选文档
2. 滚到对应文化章节
3. 内部已按 tier desc 排序 · 同 tier 相邻

**对比跨文化同材质**：
1. 需要跨文件 grep · 推荐直接查 `src/armor_data_all.json` 用 jq 或 PowerShell `Where-Object` 过滤：
   ```powershell
   $all = Get-Content src/armor_data_all.json | ConvertFrom-Json
   $all | Where-Object { $_.slot -eq 'HeadArmor' -and $_.materialType -eq 'Plate' -and $_.tier -ge 4 } | Format-Table culture, headArmor, bodyArmor, displayName
   ```

---

## 相关文档

- [`SHIELD_DATA_COMPLETE.md`](./SHIELD_DATA_COMPLETE.md) · 盾牌数据总表（114 件 · 29 材质分组）
- [`BALANCE_V2_LOG.md`](./BALANCE_V2_LOG.md) · OSA v2 平衡决议 full log（1835 决议 + 323 override）· 本表 tier 公式出处
- [`OSW_BALANCE_PROPOSAL.md`](./OSW_BALANCE_PROPOSAL.md) · OSW 武器平衡方案
- [`DESIGN_PHILOSOPHY.md`](./DESIGN_PHILOSOPHY.md) · 整体设计哲学
- `../ModdingJournal.md §P0 OSA v3` · 跨文化同材质统一立项
