# RBM Reference — RBM 修改后的原版物品全量数据

**生成时间**：2026-09-21
**数据源**：RBM v4.5.0 (workshop `2859251492`) + RBM_WS v4.5.0 (workshop `3635788184`) 的 ModuleData/*.xml
**用途**：把 RBM 覆盖 vanilla 后的所有武器 / 装备 / 马 / 马铠数值扫成扁平 CSV，作为**OSA×RBM 平衡工作的 RBM 侧基线数据**。搭配 `OSA_Reference/` 使用可做同 Type 同 material 同 tier 的直接对比。
**重新生成**：RBM 或 RBM_WS 更新后跑 `scripts/extract_rbm.ps1` 一次即可（幂等）。

**Bannerlord XML 加载语义**：同 `id` 的 Item / CraftingPiece 后加载者完全替换前者（whole-node replacement）。RBM/RBM_WS 覆盖 vanilla 数百件物品，所以本 CSV 里的每一行就是"RBM 版本的这件物品"—— vanilla 原值除非另扫 `Modules/Native/ModuleData/*.xml` 否则看不到。

---

## 数据规模

| 表 | 行数 | 列数 | 内容 |
|---|---:|---:|---|
| `data/rbm_items.csv` | 1213 | 135 | 所有 `<Item>`：armor / weapon / horse / horse harness / shield / bow / arrow / bolt / thrown / sling / goods |
| `data/rbm_crafted_items.csv` | 108 | 9 | 所有 `<CraftedItem>`（lances / gladius / axe / mace / sword 等预组装武器）meta |
| `data/rbm_crafted_items_pieces.csv` | 309 | 7 | CraftedItem → Piece 引用关系表 |
| `data/rbm_crafting_pieces.csv` | 400 | 35 | `<CraftingPiece>`：347 Blade + 53 Handle（Guard/Pommel 未加自定义，走 vanilla）|

**Workshop id → mod 名对应**：
- `2859251492` → **RBM 主体**（armour 全套 + horses + shields + ranged + lances + gladius + crafting pieces）
- `3635788184` → **RBM_WS**（RBM Weapons+Shields 扩展）

**RBM ModuleData 里被本脚本跳过的非物品 XML**（顶层非 `<Items>` / `<CraftingPieces>`）：`ItemModifiers` / `EquipmentRosters` / `NPCCharacters` / `CraftingTemplates` / `SiegeEngineTypes` / `WorkshopTypes` / `WeaponDescriptions` / `strings` / `partyTemplates` / `SkillSets` / `base`（native tweaks）—— 均非物品数值数据。

---

## Type 分布（rbm_items.csv）

| Type | 数量 | 说明 |
|---|---:|---|
| HeadArmor | 290 | 头盔 / 帽子 / hood |
| BodyArmor | 274 | 身甲 |
| Shield | 137 | 大/小盾 |
| Cape | 111 | 披风 / 肩甲 |
| Horse | 50 | 骑乘/驮兽 |
| LegArmor | 48 | 腿甲 |
| Bow | 43 | 弓 |
| Arrows | 43 | 箭 |
| HandArmor | 41 | 手甲 |
| HorseHarness | 34 | 马铠 |
| Thrown | 32 | 投掷 |
| SlingStones | 30 | 抛石 |
| Bolts | 26 | 弩箭 |
| Crossbow | 26 | 弩 |
| Goods | 22 | 交易物 |
| Animal | 3 | 非骑乘 animal |
| OneHandedWeapon | 3 | 直接 Item 武器（gladius/couched 等）|

**source_mod 分布**：RBM 1081 items + RBM_WS 132 items = 1213

**对比 OSA**：OSA 1648 items 全是护甲 + shield + thrown。RBM 覆盖面**更广**——除了护甲，还覆盖 vanilla 全部 horse、bow/arrow/bolt/crossbow、lance、sword blade 等。CraftedItem 108 件是 RBM 用 crafting 系统实现的 vanilla 武器（lance、gladius 等），要看具体武器数值走 `crafted_items → crafted_items_pieces → crafting_pieces` 三表 join。

---

## 关键平衡快照（RBM 尺度）

### 护甲值 min/avg/max（各 Type × material）

**HeadArmor.head_armor**：
| material | n | min | avg | max | OSA avg | ×RBM/OSA |
|---|---:|---:|---:|---:|---:|---:|
| Plate | 207 | 4 | **82.0** | 150 | 38.1 | **2.15** |
| Chainmail | 4 | 37 | 38.8 | 41 | 43.0 | 0.90 |
| Leather | 26 | 6 | 23.6 | 62 | 8.9 | 2.65 |
| Cloth | 53 | 6 | 11.9 | 26 | 8.1 | 1.47 |

（Plate/Leather RBM 显著高于 OSA → 印证 journal §12 里的 OSA buff 系数 ×1.6-2.0；Chainmail 头盔 OSA 已略强于 RBM，符合"HeadArmor Chainmail 不 buff"结论。）

**BodyArmor.body_armor**：
| material | n | min | avg | max | OSA avg | ×RBM/OSA |
|---|---:|---:|---:|---:|---:|---:|
| Plate | 43 | 32 | **71.8** | 135 | 40.9 | 1.75 |
| Chainmail | 52 | 0 | 50.6 | 93 | 33.1 | 1.53 |
| Leather | 62 | 0 | 19.1 | 48 | 18.2 | 1.05 |
| Cloth | 117 | 0 | 11.0 | 28 | 8.4 | 1.31 |

**HorseHarness.body_armor**：
| material | n | min | avg | max | OSA avg | ×RBM/OSA |
|---|---:|---:|---:|---:|---:|---:|
| Chainmail | 14 | 0 | **28.9** | 50 | 58.5 | 0.49 |
| Plate | 2 | 47 | 47.0 | 47 | 57.6 | 0.82 |
| Leather | 10 | 5 | 8.3 | 15 | 34.4 | 0.24 |
| Cloth | 8 | 5 | 7.8 | 10 | 12.0 | 0.65 |

**⚠ HorseHarness 反转**：RBM 版 vanilla 马铠 avg 显著**低于** OSA 版。因为 vanilla 马铠数值本来就不高（30 以下常见），RBM 保留 vanilla 数值；OSA 的马铠是**新加内容**（Saddlery 110 件），设计尺度就高很多。→ **马铠平衡** OSA 数值远大于 RBM 一档，若做 OSA×RBM 平衡这块可能反向下调 OSA 而非上调。

### Horse（RBM 覆盖的 50 匹 vanilla 马）

| 字段 | n | min | avg | max | 说明 |
|---|---:|---:|---:|---:|---|
| `Horse.charge_damage` | 50 | 3 | **11.5** | 35 | 冲锋伤害系数 |
| `Horse.maneuver` | 50 | 39 | 63.7 | 82 | 转向敏捷 |
| `Horse.speed` | 50 | 31 | 49.7 | 65 | 移动速度 |
| `Horse.extra_health` | 48 | 0 | 111.9 | 290 | 额外血量（在 base horse HP 之上）|
| `Horse.body_length` | 50 | 92 | 104.3 | 119 | 身长（判定用）|

### CraftingPiece.Blade damage_factor（武器 base 伤害系数）

**347 Blade + 53 Handle** 完全覆盖 vanilla crafting piece pool（RBM 不改 Guard / Pommel，那两类走 vanilla 原值）。

| dir | damage_type | n | min | avg | max | OSA avg (18 blade) | ×OSA/RBM |
|---|---|---:|---:|---:|---:|---:|---:|
| Swing | Cut | 260 | 0.20 | **0.95** | 1.70 | 3.18 | 3.35 |
| Swing | Blunt | 38 | 0.30 | 0.76 | 1.30 | 2.56 | 3.37 |
| Swing | Pierce | 1 | 0.90 | 0.90 | 0.90 | — | — |
| Thrust | Pierce | 242 | 0.20 | **0.87** | 1.40 | 1.58 | 1.82 |
| Thrust | Blunt | 2 | 0.50 | 0.90 | 1.30 | — | — |

（印证 journal §12 里 OSA Blade 武器 factor 下调 ×0.28-0.33 —— OSA blade avg swing 3.18 → 应压到 RBM 尺度 0.95 附近 = ×0.30。）

---

## 极值 & 观察项

**RBM Ranged thrust_damage top 3**（Bannerlord 装载攻城武器/剧情道具偏离 combat 常规尺度）：
- `ballista_c_projectile_pot_storyline` (BallistaBoulder) — 3500 damage
- `calradian_fire_RBM` (Bolt, RBM_WS 剧情弹药) — 1500
- `ballista_c_projectile_burning` (Arrow) — 200

**HeadArmor Plate max = 150** — 明显 RBM 顶级重甲头盔（如 `lord_helmet` / `armored_bascinet`）拉到 vanilla 3× 尺度，符合 RBM "高等级重甲能扛砍杀多刀"设计意图。

**Horse.charge_damage max = 35** — 极值应是 `charger_horse` 或 lord's steed，avg 11.5 说明中低段马是量产战马占多数。

---

## CSV 字段释义

**通用列**（所有 CSV）：
- `source_mod` — RBM / RBM_WS
- `source_file` — 原 XML 文件名（如 `RBMCombat_head_armors.xml`，方便追回源）

**rbm_items.csv 关键列**（135 列全清单见 CSV 头行）：

顶层：`id` / `name` / `Type` / `culture` / `subtype` / `mesh` / `weight` / `value` / `difficulty` / `appearance` / `item_category` / `is_merchandise` / `item_holsters`

**Armor 子结构**（前缀 `Armor.`）—— 与 OSA_Reference 一致 —— 见 `OSA_Reference/README.md`。RBM 里额外可见字段：
- `Armor.stealth_factor` — 潜行加成（如 hood 类 headarmor 有）

**Weapon 子结构**（前缀 `Weapon.`）：
- `Weapon.weapon_class` / `Weapon.ammo_class`
- `Weapon.weapon_length` / `Weapon.thrust_damage` / `Weapon.thrust_speed` / `Weapon.swing_damage` / `Weapon.swing_speed` / `Weapon.speed_rating` / `Weapon.accuracy` / `Weapon.missile_speed`
- `Weapon.stack_amount` / `Weapon.item_usage` / `Weapon.modifier_group`
- `Weapon.WeaponFlags.<flag_name>` — 一系列 bool

**Horse 子结构**（前缀 `Horse.`，**RBM_Reference 独有**，OSA 无 Horse item）：
- `Horse.monster` — Monster.horse / Monster.camel / Monster.mule
- `Horse.maneuver` — 转向速度
- `Horse.speed` — 移动速度
- `Horse.charge_damage` — 冲锋伤害系数
- `Horse.body_length` — 长度（用于范围判定）
- `Horse.extra_health` — 额外血量
- `Horse.is_mountable` / `Horse.is_pack_animal` — 骑乘 / 驮兽 布尔
- `Horse.modifier_group` — horse modifier 池
- （**跳过**了 `Horse.AdditionalMeshes` / `Horse.Materials` / `Horse.MeshMultipliers`—— 视觉资产，与战斗数值无关）

**Flags 前缀**：`Flags.<flag>` = "true"

---

**rbm_crafting_pieces.csv 关键列**：
- `id` / `name` / `piece_type`（Blade / Handle）/ `tier` / `culture` / `length` / `weight`
- `is_hidden` — 隐藏 piece（tournament 训练版本、废弃 asset 等）
- `CraftingCost` — 打造花费（仅部分 piece 有）
- `BladeData.blade_length` / `.blade_width` / `.stack_amount` / `.physics_material` / `.body_name`
- `BladeData.Swing.damage_type` / `BladeData.Swing.damage_factor`
- `BladeData.Thrust.damage_type` / `BladeData.Thrust.damage_factor`
- `BuildData.piece_offset` / `BuildData.next_piece_offset`
- `Flags.Flag.name` — piece 特性

---

## 使用示例

**同 Type 同 material 对比 RBM vs OSA**：
```powershell
$rbm = Import-Csv RBM_Reference/data/rbm_items.csv
$osa = Import-Csv OSA_Reference/data/osa_items.csv

# BodyArmor Plate 全档 avg 对比
$rbmVals = $rbm | Where-Object { $_.Type -eq 'BodyArmor' -and $_.'Armor.material_type' -eq 'Plate' } |
           ForEach-Object { [int]$_.'Armor.body_armor' }
$osaVals = $osa | Where-Object { $_.Type -eq 'BodyArmor' -and $_.'Armor.material_type' -eq 'Plate' } |
           ForEach-Object { [int]$_.'Armor.body_armor' }
"RBM Plate body: n={0} avg={1:N1}" -f $rbmVals.Count, ($rbmVals | Measure-Object -Average).Average
"OSA Plate body: n={0} avg={1:N1}" -f $osaVals.Count, ($osaVals | Measure-Object -Average).Average
```

**找 RBM 里所有 Sturgia 高端头盔**：
```powershell
$rbm | Where-Object { $_.Type -eq 'HeadArmor' -and $_.culture -eq 'Culture.sturgia' -and [int]$_.'Armor.head_armor' -ge 60 } |
    Sort-Object { [int]$_.'Armor.head_armor' } -Descending |
    Select-Object id, name, 'Armor.head_armor', 'Armor.material_type', weight | Format-Table -AutoSize
```

**看 RBM 一匹具体马的完整属性**（例如 `armored_horse`）：
```powershell
$rbm | Where-Object { $_.Type -eq 'Horse' -and $_.id -eq 'armored_horse' } |
    Select-Object * | Format-List
```

**join CraftedItem 看 lance 的实际数值**：
```powershell
$ci    = Import-Csv RBM_Reference/data/rbm_crafted_items.csv
$cip   = Import-Csv RBM_Reference/data/rbm_crafted_items_pieces.csv
$cp    = Import-Csv RBM_Reference/data/rbm_crafting_pieces.csv

# vlandia_lance_1_t3 的所有 piece 数值
$ci | Where-Object { $_.id -eq 'vlandia_lance_1_t3' }
$cip | Where-Object { $_.crafted_item_id -eq 'vlandia_lance_1_t3' } |
    ForEach-Object {
        $piece = $cp | Where-Object { $_.id -eq $_.id } | Select-Object -First 1
        [pscustomobject]@{
            slot = $_.Type
            piece_id = $_.id
            scale = $_.scale_factor
            length = $piece.length
            swing_dmg = $piece.'BladeData.Swing.damage_factor'
            thrust_dmg = $piece.'BladeData.Thrust.damage_factor'
        }
    } | Format-Table -AutoSize
```

---

## 重新生成

Workshop mod 更新后：
```powershell
& RBM_Reference/scripts/extract_rbm.ps1
```
- 默认从 `E:\SteamLibrary\steamapps\workshop\content\261550\` 读；`-WorkshopRoot` 参数可覆盖
- 输出直接覆盖 `RBM_Reference/data/*.csv`，用 `git diff` 一眼看到 RBM 更新改了哪些数值
- 幂等

---

## 已知限制

- **CraftedItem 引用的 Piece 有可能是 vanilla**：本 CSV 只含 RBM 自定义 400 CraftingPiece。CraftedItem 若引用 vanilla piece（如某些 Handle / Guard / Pommel），需另扫 `Modules/Native/ModuleData/crafting_pieces.xml` 补齐
- **`<AdditionalMeshes>` / `<Materials>` / `<MeshMultipliers>` 视觉子树跳过**：Horse item 有一大堆颜色/mesh 变体资产声明，与战斗数值无关，脚本内 `$skipSubtrees` 硬跳过
- **多 `<Flag>` 子节点 collision**：CraftingPiece 里若有多个 `<Flag>`，Flatten-Element 只保留最后一个 `Flags.Flag.name`。RBM 大部分 piece 只 1-2 个 flag 无问题
- **XML 注释里的物品**（如 comment out 的 experimental piece）自动跳过
- **column set 是 union**：不同 Type 用不到的列显示空白（比如 Horse item 的 Armor.* 全空），正常
- **数据是"RBM 装载后的运行时状态"**：RBM XML 覆盖 vanilla 后，游戏引擎实际看到的就是这里的值。vanilla 原值需另扫 Native
