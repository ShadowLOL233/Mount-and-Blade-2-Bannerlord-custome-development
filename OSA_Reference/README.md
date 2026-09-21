# OSA Reference — 三 mod 全量物品数据

**生成时间**：2026-09-21
**数据源**：OSA v2.0.0（护甲）+ OSW v2.0.1（武器）+ Saddlery v2.0.0（马具）workshop XML
**用途**：为 OSA×RBM 平衡工作提供**完整字段的可查数据基础**——所有物品的 id / name / 数值 / flag 都进 CSV，Excel 打开可排序、过滤、透视。
**重新生成**：workshop 更新后跑 `scripts/extract_osa.ps1` 一次即可（幂等）。

---

## 数据规模

| 表 | 行数 | 列数 | 内容 |
|---|---:|---:|---|
| `data/osa_items.csv` | 1881 | 101 | 所有 `<Item>`：armor / weapon / shield / thrown / horse harness / bolt |
| `data/osa_crafted_items.csv` | 140 | 8 | 所有 `<CraftedItem>`（预组装武器）meta（id / template / culture）|
| `data/osa_crafted_items_pieces.csv` | 512 | 7 | CraftedItem → Piece 引用关系表（每 CraftedItem ~3.7 pieces）|
| `data/osa_crafting_pieces.csv` | 50 | 37 | `<CraftingPiece>`：18 Blade + 13 Pommel + 11 Handle + 8 Guard |

**Workshop id → mod 名对应**（journal 之前记的顺序有误，此处已修正）：
- `3011479883` → **OSA**（护甲，1648 件）
- `3010984416` → **OSW**（武器 + 盾 + crafting pieces，共 123 Item + 140 CraftedItem + 50 Piece）
- `3010990914` → **Saddlery**（马铠 HorseHarness 110 件）

---

## Type 分布（osa_items.csv）

| Type | 数量 | source_mod |
|---|---:|---|
| HeadArmor | 843 | OSA |
| BodyArmor | 388 | OSA |
| Cape (含肩甲) | 310 | OSA |
| Shield | 114 | OSW |
| HorseHarness | 110 | Saddlery |
| LegArmor | 61 | OSA |
| HandArmor | 46 | OSA |
| Thrown | 7 | OSW |
| Crossbow | 1 | OSW |
| Bolts | 1 | OSW |

**注意**：绝大多数武器（1h axe/mace/sword、polearm、2h、bow 等）在 OSA 里以 `<CraftedItem>` 出现——它们不带独立数值，运行时由所引用的 CraftingPiece（Blade / Guard / Handle / Pommel）组装伤害/长度/重量。**要查武器实际数值** → 走 `osa_crafted_items.csv` 找目标 `<CraftedItem>` → `osa_crafted_items_pieces.csv` 找它引用的 Piece id → `osa_crafting_pieces.csv` 拿 Piece 数值（**OSA 自定义 Piece 只 50 件，其余引用 vanilla Piece 不在本表**）。

---

## 关键平衡快照

### 护甲值 min/avg/max（各 Type × material）

**HeadArmor.head_armor**：
| material | n | min | avg | max |
|---|---:|---:|---:|---:|
| Plate     | 772 | 10 | 38.1 | 58 |
| Chainmail | 20  | 20 | 43.0 | 54 |
| Leather   | 27  | 2  | 8.9  | 24 |
| Cloth     | 24  | 3  | 8.1  | 20 |

**BodyArmor.body_armor**：
| material | n | min | avg | max |
|---|---:|---:|---:|---:|
| Plate     | 206 | 18 | 40.9 | 57 |
| Chainmail | 78  | 25 | 33.1 | 51 |
| Leather   | 46  | 1  | 18.2 | 34 |
| Cloth     | 58  | 0  | 8.4  | 19 |

**HorseHarness.body_armor**：
| material | n | min | avg | max |
|---|---:|---:|---:|---:|
| Chainmail | 40 | 12 | 58.5 | 75 |
| Plate     | 40 | 30 | 57.6 | 75 |
| Leather   | 29 | 9  | 34.4 | 60 |
| Cloth     | 1  | 12 | 12.0 | 12 |

（Cape / HandArmor / LegArmor / 跨槽字段完整数据都在 CSV 里，Excel 里按 Type 过滤即可自算。）

### CraftingPiece.Blade damage_factor（武器 base 伤害）

18 件 OSA 自定义 Blade：
| dir | damage_type | n | min | avg | max |
|---|---|---:|---:|---:|---:|
| Swing  | Cut    | 5  | 1.60 | 3.18 | 3.90 |
| Swing  | Blunt  | 8  | 1.86 | 2.56 | 3.30 |
| Thrust | Pierce | 10 | 0.40 | 1.58 | 2.80 |

（与 journal §12 里"武器 override 系数"表可直接交叉比对——OSW Blade avg 已远高于 RBM 基线，`OpenSourceArmouryRBMBalance` 已下调。）

### 极值物品（top-3 by weight）

- **BodyArmor**：`AR_vlandia_scale_a` (36.9) / `TV_empire_armor_m` (34) / `TV_empire_armor_n` (34)
- **HeadArmor**：`varangian_guard_helmet_a` (5.8) / `varangian_guard_helmet_c` (5.4) / `full_helm_over_mail_coif_z` (5.0)
- **Cape**：`AR_imperial_shoulders_y` (14) / `AR_vlandia_shoulders_f` (11.4)
- **HorseHarness**：`AR_horse_armor_zap2` / `_zaq2` / `_e` 并列 145

---

## Culture 分布

| culture | 件数 |
|---|---:|
| Culture.empire | 424 |
| Culture.aserai | 338 |
| Culture.battania | 320 |
| Culture.vlandia | 314 |
| Culture.khuzait | 244 |
| Culture.sturgia | 202 |
| (无 culture 标注) | 38 |
| Culture.looters | 1 |

---

## CSV 字段释义

**通用列**（所有 CSV）：
- `source_mod` — OSA / OSW / Saddlery
- `source_file` — 原 XML 文件名

**osa_items.csv 主要列**（101 列全清单见 CSV 头行）：

| 列 | 说明 |
|---|---|
| `id` | 唯一 StringId（如 `AR_aserai_armor_a`）|
| `name` | 本地化 key + fallback 英文名（如 `{=osa_ar_...}Southern Leather Coat`）|
| `Type` | BodyArmor / HeadArmor / Cape / HandArmor / LegArmor / HorseHarness / Shield / Thrown / Crossbow / Bolts |
| `culture` | Culture.<empire/aserai/battania/vlandia/khuzait/sturgia/looters> |
| `mesh` | 3D 模型 mesh 名 |
| `weight` | 重量（kg 尺度）|
| `value` | 商店基础价格（可能为空 → 用 CalculateItemValue 算）|
| `difficulty` | 使用需求（力量/技能）|
| `appearance` | 视觉贴图权重（不参与战斗数值）|
| `subtype` | body_armor / head_armor（vanilla 分类冗余字段）|
| `has_lower_holster_priority` / `item_holsters` / `AmmoOffset` / `holster_position_shift` | 装备槽/位置定位 |
| `is_merchandise` | false = 只能自建/掉落，不在市集售卖 |

**Armor 子结构**（前缀 `Armor.`）：
- `Armor.head_armor` / `Armor.body_armor` / `Armor.arm_armor` / `Armor.leg_armor` — 四个 slot 护甲值
- `Armor.material_type` — Cloth / Leather / Chainmail / Plate（伤害衰减 model 输入）
- `Armor.modifier_group` — 铁匠 modifier 池分类（cloth / leather / chain / plate 等）
- `Armor.covers_body` — 是否算作 body 覆盖（用于胸+腹判定）
- `Armor.has_gender_variations` — 是否有 M/F 变体
- `Armor.hair_cover_type` / `Armor.beard_cover_type` — 头盔遮盖发/须的模式
- `Armor.mane_cover_type` / `Armor.maneuver_bonus` / `Armor.speed_bonus` / `Armor.charge_bonus` / `Armor.family_type` / `Armor.reins_mesh` — HorseHarness 专属

**Weapon 子结构**（前缀 `Weapon.`）——本次 OSW/OSA 里 Item 直接带 Weapon 的只有 Thrown/Crossbow/Bolt/Shield，武器主力在 CraftedItem：
- `Weapon.weapon_class` / `Weapon.ammo_class` — 分类
- `Weapon.weapon_length` / `Weapon.thrust_damage` / `Weapon.thrust_speed` / `Weapon.swing_damage` / `Weapon.swing_speed` — 常规武器数值
- `Weapon.thrust_damage_type` / `Weapon.swing_damage_type` — Pierce / Cut / Blunt
- `Weapon.accuracy` / `Weapon.missile_speed` / `Weapon.stack_amount` — 弹药/远程专属
- `Weapon.item_usage` — 动画绑定
- `Weapon.WeaponFlags.<flag_name>` — 一系列 bool 标记（RangedWeapon / Consumable / CanPenetrateShield 等）

**Flags 前缀**：
- `Flags.<flag_name>` — Item 顶层 flag（Civilian / UseTeamColor 等）

---

**osa_crafting_pieces.csv 主要列**：
- `id` / `name` / `piece_type`（Blade / Guard / Handle / Pommel）/ `tier` / `culture` / `mesh` / `weight` / `length`
- `BladeData.blade_length` / `BladeData.blade_width` / `BladeData.stack_amount` / `BladeData.physics_material` / `BladeData.body_name`
- `BladeData.Swing.damage_type` / `BladeData.Swing.damage_factor` — 挥砍伤害系数（乘 blade_length 得实际伤害）
- `BladeData.Thrust.damage_type` / `BladeData.Thrust.damage_factor` — 突刺伤害系数
- `StatContributions.armor_bonus` — Guard piece 提供的护手护甲
- `BuildData.piece_offset` / `BuildData.next_piece_offset` — 组装偏移（视觉/物理）
- `Materials.Material.id` / `Materials.Material.count` — 打造材料需求
- `Flags.Flag.name` — piece flag（`CanBePickedUpFromCorpse` / `Civilian` 等）
- `is_default` / `CraftingCost` — 部分 piece 特殊标记

---

## 使用示例

**Excel 打开 CSV 后**：
1. 数据 → 从文本导入（UTF-8 编码，逗号分隔）
2. 全选 → 插入 → 表格（Ctrl+T）→ 打开筛选下拉
3. 用 `Type` / `Armor.material_type` / `culture` 组合过滤，即可看某类目下所有物品数值
4. 数据透视表：行 = culture，列 = Armor.material_type，值 = avg(Armor.body_armor)

**PowerShell 查询举例**：
```powershell
$items = Import-Csv OSA_Reference/data/osa_items.csv

# 所有 Aserai 的 Plate 头盔按 head_armor 降序
$items | Where-Object { $_.Type -eq 'HeadArmor' -and $_.culture -eq 'Culture.aserai' -and $_.'Armor.material_type' -eq 'Plate' } |
    Sort-Object { [int]$_.'Armor.head_armor' } -Descending |
    Select-Object id, name, 'Armor.head_armor', weight | Format-Table -AutoSize

# 找 arm_armor > 30 的 BodyArmor（意味着"整套上身+肩护"设计）
$items | Where-Object { $_.Type -eq 'BodyArmor' -and [int]$_.'Armor.arm_armor' -gt 30 } |
    Select-Object id, name, culture, 'Armor.body_armor', 'Armor.arm_armor', 'Armor.leg_armor', weight
```

---

## 重新生成

Workshop mod 更新后（或想加字段/换切分策略）：
```powershell
& OSA_Reference/scripts/extract_osa.ps1
```
- 默认从 `E:\SteamLibrary\steamapps\workshop\content\261550\` 读；`-WorkshopRoot` 参数可覆盖
- 输出直接覆盖 `OSA_Reference/data/*.csv`，diff 一下可看到哪个物品被 workshop 更新了
- 幂等：多跑几次不改变结果

**已知限制**：
- **CraftedItem 数值不完整**：CraftedItem 引用的 Piece 大部分是 vanilla Piece（不在 OSA 自定义 50 Piece 里），本 CSV 只能看 CraftedItem 组成结构，看不到 vanilla Piece 数值（需另扫 vanilla `Modules/Native/ModuleData/crafting_pieces.xml`）
- **XML 注释里的 item 不入表**：如 OSW crafting_pieces.xml 里第 342-569 行被注释掉的 20 件 `_blunt` 训练件（tournament-hidden），XmlDocument 自动跳过。反编译 journal §12 有记
- **多 `<Flag>` 子节点会 collision**：CraftingPiece 里 `<Flags><Flag name=...>` 多个 Flag 时，本脚本只留最后一个（`Flags.Flag.name`）——只在 CraftingPiece 表 relevant，`osa_items.csv` 的 Flags 是纯 attribute 无此问题
- **column set 是 union**：不同 Type 用不到的列会显示空白，正常
