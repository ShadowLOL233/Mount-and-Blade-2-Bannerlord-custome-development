# Equipment Spawner Mod

自研的 Bannerlord v1.4.7 小模组（前身 `Tier6Injector`；2026-09-20 改名+个人库；同日 v1.5 加 OSA 全 tier 注入 + 每城仓库）。

## 五个热键

| 组合键 | 动作 |
|---|---|
| **Ctrl + Alt + I** | 注入 `Tier3-Tier6` vanilla 装备/坐骑各 ×20，**外加 OSA 系列所有 tier**（id/mesh 前缀匹配 AR_/AD_/AO_/BA_/DZ_/TV_/ao_/ap_/bl_/hmj_/tv_）。每件挂 `ItemModifierGroup` 最高质 modifier |
| **Ctrl + Alt + O** | 主队背包 → **个人库**（跟着玩家走）|
| **Ctrl + Alt + P** | 个人库 → 主队背包 |
| **Ctrl + Alt + U** | 主队背包 → **当前城仓**（要求你在自己 clan 拥有的 fief 内）|
| **Ctrl + Alt + Y** | 当前城仓 → 主队背包 |

个人库 = 单一 `ItemRoster`；城仓 = `Dictionary<Settlement.StringId, ItemRoster>`——每个 fief 独立一个库、只在**该 fief 内**能存/取。两个都通过 `CampaignBehaviorBase.SyncData` 持久化到存档。

## 用意

- **OSA 全 tier 注入**（v1.5）：OSA 大量 T1-T2 民用款式（keffiyeh/roman_hat 等）也想上身对比，`IsOsaItem` 白名单让这类物品也进入主队库存
- **个人库**（v1.4）：`Ctrl+Alt+I` 一堆 1-2 万 kg 装备会秒爆主队 4-6k kg 上限——出征前 `O` 甩进库、要装备时 `P` 拿回
- **城仓**（v1.5）：每座 fief 一个仓库，符合"回大本营取备用套装"的直觉；远征时不必带全部备用甲 → 在自己 fief 部署常驻套装

## 覆盖范围

**装备类**：HeadArmor / BodyArmor / ChestArmor / LegArmor / HandArmor / Cape / Shield / OneHandedWeapon / TwoHandedWeapon / Polearm / Bow / Crossbow / Thrown / Sling / Pistol / Musket / Arrows / Bolts / SlingStones / Bullets。

**坐骑类**：Horse / HorseHarness（同样 ×20）。

食物、贸易品、Goods、Banner、Animal、Book、Invalid 等不注入、也不进个人库。

**注**：Pistol/Musket/Bullets 是 NavalDLC 类型；启用了 NavalDLC 才有效果。

## 目录结构

```
EquipmentSpawnerMod/
├── SubModule.xml                        # 模组清单
├── src/
│   ├── EquipmentSpawnerSubModule.cs    # SubModule + PersonalStashBehavior
│   └── EquipmentSpawnerMod.csproj      # net472 构建配置
├── deploy.ps1                           # build + copy 到游戏 Modules
└── README.md
```

## 构建 & 部署

```powershell
# 关掉 Bannerlord 启动器（会锁 mod DLL）
cd C:\Users\situj\git\Mount-and-Blade-2-Bannerlord-custome-development\EquipmentSpawnerMod
.\deploy.ps1
```

`deploy.ps1` 会：

1. `dotnet build -c Release` 编译 `src/EquipmentSpawnerMod.csproj`
2. 拷 `SubModule.xml` → `Modules\EquipmentSpawnerMod\SubModule.xml`
3. 拷 `EquipmentSpawnerMod.dll` → `Modules\EquipmentSpawnerMod\bin\Win64_Shipping_Client\`

## 启用

启动器 → Mods → 勾选 **Equipment Spawner Mod**（拖到 `Sandbox` 之后即可）→ Play。**首次装：需要保存 + 读档一次让存档写入 PersonalStash 字段**，之后 `Ctrl+Alt+O/P` 才能工作；`Ctrl+Alt+I` 首次即可用（不依赖 behavior）。

## 卸载

启动器里取消勾选，或者删 `Modules\EquipmentSpawnerMod\`。
**注意**：存档里会残留 `EquipmentSpawnerMod_PersonalStash` 字段引用。Bannerlord 存档系统对未知字段是宽容的（跳过读入不报错），但残留字节会一直在。要彻底净化就在卸模组前 `Ctrl+Alt+P` 把库清空再存一次。

## 兼容性

- **加载顺序不敏感**：无 Harmony patch，只轮询热键 + 一个 CampaignBehavior
- **RBM / Retinues / CYT 都不冲突**：物品来自 vanilla 的 `MBObjectManager`，不动装备槽也不改 troop 定义
- **物品清单包含所有其他 mod 注册的 Tier3-6 物品**（BahamutArmory / OSA / swadian armoury / XorberaxLegacy 等只要注册到 `MBObjectManager` 且标 Tier3-6 就一并给）

## API 核对（2026-09-20 用 ilspycmd 对 v1.4.7 DLL 核过）

| API | 位置 | 状态 |
|---|---|---|
| `ItemObject.ItemTiers.Tier3..Tier6` | `TaleWorlds.Core.ItemObject.ItemTiers` | ✓ |
| `ItemObject.ItemTypeEnum.*`（20 项装备 + 2 项坐骑） | 同上 | ✓（HandArmor 不是 HandsArmor）|
| `InformationManager.DisplayMessage` | `TaleWorlds.Library.InformationManager` | ✓ |
| `Input.IsKeyDown(InputKey)` | `TaleWorlds.InputSystem` | ✓ |
| `MobileParty.MainParty.ItemRoster.AddToCounts` | `TaleWorlds.CampaignSystem.Roster.ItemRoster` | ✓（`ItemRoster` 在 `Roster` 子命名空间）|
| `ItemRoster` 无参构造 + `Clear()` + `Count` + `GetElementCopyAtIndex(int)` | 同上 | ✓ |
| `MBObjectManager.Instance.GetObjectTypeList<ItemObject>` | `TaleWorlds.ObjectSystem` | ✓ |
| `MBSubModuleBase.OnGameStart(Game, IGameStarter)` | `TaleWorlds.MountAndBlade` | ✓（`protected internal virtual`，override 必须 `protected` 匹配）|
| `CampaignBehaviorBase.SyncData(IDataStore)` / `RegisterEvents()` | `TaleWorlds.CampaignSystem.CampaignBehaviorBase` | ✓（`abstract`，必须实现）|
| `IDataStore.SyncData<T>(string, ref T)` | `TaleWorlds.CampaignSystem.IDataStore` | ✓ |
| `Campaign.Current.GetCampaignBehavior<T>()` | `TaleWorlds.CampaignSystem.Campaign` | ✓ |
| `CampaignGameStarter.AddBehavior(CampaignBehaviorBase)` | 同上 | ✓ |

## 设计选择记录

- **热键 vs 自动**：热键完全无状态，卸模组即消失（唯一状态＝个人库，写在存档里、`SyncData` 有序处理）
- **CampaignBehavior 挂个人库**：`ItemRoster` 是 vanilla `[SaveableField]` 结构，可直接 `dataStore.SyncData<ItemRoster>()` 序列化；不需要写自己的 `TypeDefiner`
- **O/P 都用 `IsGear + isMount` 过滤**：避免误搬食物/贸易品。转账走 `AddToCounts(elt, +n)` + `AddToCounts(elt, -n)` 对称扣加，无遗漏
- **反向 for 循环 + `GetElementCopyAtIndex`**：`AddToCounts(-n)` 会引起 `RemoveZeroCountsFromRoster` 重排，正向迭代会漏，反向遍历安全
- **每次 I 键都堆 ×20 不去重**：故意的；多按几次可以攒很多套
- **不做 MCM 面板**：数量常量少，配置价值 < MCM 依赖成本

## 修改常量

编辑 `src/EquipmentSpawnerSubModule.cs`：

```csharp
private const int GearQuantity = 20;
private const int HorseQuantity = 20;
private const ItemObject.ItemTiers MinTier = ItemObject.ItemTiers.Tier3;
```

热键：`OnApplicationTick` 里改 `InputKey.I / O / P`。
