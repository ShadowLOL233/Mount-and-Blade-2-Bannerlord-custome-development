# Tier6Injector

自研的 Bannerlord v1.4.7 小模组。**热键 Ctrl+Alt+I** → 把当前所有 `Tier3-Tier6` 的装备类物品各 ×20 塞进主队背包，另附对应 tier 的战马/战马鞍 ×20。每件物品自动挂上其 `ItemModifierGroup` 里质量最高的 `ItemModifier`（Legendary > Masterwork > Fine > Common > Inferior > Poor）。

无 Harmony patch、无 CampaignBehavior 侵入、不改写存档结构 —— 关模组即完全撤除。

---

## 目录结构

```
Tier6Injector/
├── SubModule.xml                 # 模组清单，部署到 Modules\Tier6Injector\
├── src/
│   ├── Tier6InjectorSubModule.cs # 主入口（MBSubModuleBase 继承）
│   └── Tier6Injector.csproj      # net472 构建配置
├── deploy.ps1                    # build + copy 到游戏 Modules 目录
└── README.md
```

## 覆盖范围（Tier 6 且 ItemType 属于以下之一）

- HeadArmor / BodyArmor / ChestArmor / LegArmor / HandArmor / Cape
- Shield
- OneHandedWeapon / TwoHandedWeapon / Polearm
- Bow / Crossbow / Thrown / Sling / Pistol / Musket
- Arrows / Bolts / SlingStones / Bullets

Horse / HorseHarness ×20 每种（马放在背包里不占队伍容量，只有"骑上"才占坐骑槽；Tier3-6 全量马约 20+ 种，每种 20 匹 = 400+ 匹滚屏但无副作用）。

食物、贸易品、Goods、Banner、Animal、Book、Invalid 等一律不给。

**注**：Pistol/Musket/Bullets 是 NavalDLC 类型；启用了 NavalDLC 才有效果，否则清单里 tier6 数量为 0，无副作用。

## 构建前置

- **.NET SDK 6+**（能够 target `net472` 即可；SDK 自带 MSBuild 无需另装 VS）
  - 当前机器 `dotnet --version` 报 "No .NET SDKs were found" —— 只有 runtime，没 SDK
  - 装法（PowerShell 管理员）：`winget install --id Microsoft.DotNet.SDK.8`
  - 装完关掉当前终端再新开一个，`dotnet --list-sdks` 应能列出 8.x
- 游戏本体在 `E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\`（其他路径需要改 `csproj` 里的 `BannerlordBin` 或 `deploy.ps1` 的 `-GameRoot`）
- 版本锁定 **v1.4.7 build 117484**（其他版本 API 可能漂移，见 API 核对表）

## 构建 & 部署

```powershell
# 关掉 Bannerlord 启动器（launcher 会锁 mod DLL）
cd C:\Users\situj\git\Mount-and-Blade-2-Bannerlord-custome-development\Tier6Injector
.\deploy.ps1
```

`deploy.ps1` 会：

1. `dotnet build -c Release` 编译 `src/Tier6Injector.csproj`
2. 拷 `SubModule.xml` → `Modules\Tier6Injector\SubModule.xml`
3. 拷 `Tier6Injector.dll` → `Modules\Tier6Injector\bin\Win64_Shipping_Client\`

## 启用

开 Bannerlord 官方启动器 → **Mods** 标签 → 勾选 **Tier6 Injector**（拖到 `Sandbox` 之后即可，加载顺序不敏感）→ Play。

## 使用

进战役读档后（或新开一场战役），战略地图或队伍面板任意场景下：

- 按 **Ctrl + Alt + I**
- 消息栏出现 `Tier6 Injector: injected N gear stacks x99, M horse/harness stacks x3.`
- 打开主队背包（I 键）看装备到位

再按一次热键会再堆 99 —— **不去重**，故意让你想要多套时可以连按。

## 卸载

启动器里取消勾选 **Tier6 Injector**，或者直接删 `Modules\Tier6Injector\`。存档不受影响（Tier6Injector 无 SaveableFields，不写存档）。

## 兼容性

- **加载顺序无所谓**：不 patch 任何东西，只在 `OnApplicationTick` 里轮询热键 → 触发时调 vanilla 的 `ItemRoster.AddToCounts`
- **与 RBM / Retinues / CYT 等无冲突**：物品对象来自 vanilla 的 `MBObjectManager`，不动装备槽也不改 troop
- **物品清单包含所有 mod 加进来的 Tier 6 物品**（例如 BahamutArmory / swadian armoury / XorberaxLegacy 如果之后启用）——只要它们注册到 `MBObjectManager` 且标 `Tier6`，就会一并给

## API 核对结果（2026-09-19 用 dnSpy 对 v1.4.7 DLL 核过）

| API | 位置 | 状态 |
|---|---|---|
| `ItemObject.ItemTiers.Tier6` | `TaleWorlds.Core.dll` → `TaleWorlds.Core.ItemObject+ItemTiers` | ✓ |
| `ItemObject.ItemTypeEnum.HeadArmor/BodyArmor/ChestArmor/LegArmor/HandArmor/Cape` | 同上 → `ItemTypeEnum` | ✓（是 `HandArmor` 不是 `HandsArmor`）|
| `ItemObject.ItemTypeEnum.OneHandedWeapon/TwoHandedWeapon/Polearm/Bow/Crossbow/Thrown/Sling/Pistol/Musket` | 同上 | ✓ |
| `ItemObject.ItemTypeEnum.Arrows/Bolts/SlingStones/Bullets` | 同上 | ✓ |
| `ItemObject.ItemTypeEnum.Horse/HorseHarness` | 同上 | ✓ |
| `InformationManager.DisplayMessage(InformationMessage)` | `TaleWorlds.Library.dll` → `TaleWorlds.Library.InformationManager` | ✓ |
| `Input.IsKeyDown(InputKey)` / `IsKeyPressed(InputKey)` | `TaleWorlds.InputSystem.dll` | ✓（HotKey.cs 里就用同款 pattern）|
| `InputKey.LeftControl/RightControl/LeftAlt/RightAlt/I` | `TaleWorlds.InputSystem.InputKey` | ✓ |
| `MobileParty.MainParty.ItemRoster.AddToCounts(ItemObject, int)` | `TaleWorlds.CampaignSystem.dll` | 未直接核，标准 vanilla API，Retinues 大量使用 |
| `MBObjectManager.Instance.GetObjectTypeList<ItemObject>()` | `TaleWorlds.ObjectSystem.dll` | 未直接核，标准 vanilla API |
| `MBSubModuleBase.OnApplicationTick(float)` / `OnGameInitializationFinished(Game)` | `TaleWorlds.MountAndBlade.dll` | 未直接核，标准生命周期 hook |

**首次 build 若报错**：优先查上表最后 3 条，用 `dnSpy.Console.exe -t <TypeName> <path>` 核对签名后微调即可。

## 设计选择记录

- **为什么用热键不是自动**：自动 OnSessionLaunched 需要写 SaveableField 记录 "已注入" 防止读档重复堆 —— 那样等于污染存档结构；热键则完全无状态，卸模组即消失
- **数量选择（v1.3）**：装备 ×20、马 ×20。20 足够全 companion + retinue 换装 + 备用囤货，同时避免装备栏被 99 stack 塞得滚屏找不着东西
- **为什么"最高珍稀度"用 ItemQuality 排序**：Bannerlord 的 `ItemModifier` 有一个显式的 `ItemQuality { Poor, Inferior, Common, Fine, Masterwork, Legendary }` 枚举，直接选组内 quality 最高的即"最高珍稀度"；不用 `PriceMultiplier` 因为它对同 quality 的不同属性侧重（+damage vs +speed）会有波动，quality 是唯一稳定信号
- **为什么不做 in-game config 面板**：MCM 需要额外 mod 依赖、需要写额外 XML。这个模组作用点固定，配置价值低于依赖成本

## 修改常量

需要改数量：编辑 `src/Tier6InjectorSubModule.cs`：

```csharp
private const int GearQuantity = 20;   // 每种装备的数量
private const int HorseQuantity = 20;  // 每种马/鞍的数量
```

改完 `.\deploy.ps1` 重新构建部署。

需要改热键：同文件 `OnApplicationTick` 里的 `InputKey.LeftControl / LeftAlt / I` 三个键。
