# Retinues Culture Filter

自研 Bannerlord v1.4.7 小模组，给 **Retinues** mod 的 Troop Editor 装备选择列表增加**文化过滤器**——用于 OSA×RBM 数值平衡工作中借用 Retinues 内置的属性对比功能按文化分组审视装备。

## 用法

| 组合键 | 动作 |
|---|---|
| **Ctrl + Shift + C** | 循环切换文化过滤：All Cultures → Empire → Vlandia → Aserai → Battania → Sturgia → Khuzait → All |
| **Ctrl + Shift + X** | 清除过滤器（= All Cultures）|

切换后：
1. 消息栏显示当前过滤：`Retinues Culture Filter: [Aserai]`
2. 打开 Retinues Troop Editor → 装备编辑 → 点任意装备槽（触发 `RefreshFilter`）→ 列表只显示所选文化的装备
3. 装备名不含 "aserai" 等文化关键词的物品，也能通过我们的过滤器正确显现（借助 `WItem.Culture.StringId` 直接匹配）

## 与 Retinues 现有搜索的关系

Retinues 顶部搜索框本身已经在名称/类别/类型都不匹配时**降级到 Culture.Name 匹配**（`FilterText.Contains(culture)`）。但缺陷是：
1. 只在其他字段没匹配时才生效——**造成误配**
2. 依赖装备名字含关键词——**大量装备名字并不含文化名**

我们的 mod 直接读 `WItem.Culture.StringId` 进行精确匹配，绕过前面所有的 fuzzy 匹配。

## 架构

- **`SubModule.cs` · `SubModule`**：注册 Harmony、拦截 Ctrl+Shift+C/X 热键、循环调 `CultureFilterState.CycleNext/Clear`
- **`CultureFilterState`**：静态类持共享状态 `CurrentIndex` + 7 文化循环表；避免 mixin 复杂度
- **`RefreshFilterPatch`（Harmony postfix）**：拦截 `EquipmentListVM.RefreshFilter`，在 Retinues 完成默认过滤后，遍历 `EquipmentRows` 剔除 `row.RowItem.Culture.StringId != wanted` 的项；空 row（unequip 占位）保留

**没有 XAML 注入**：Retinues 的 `ClanScreen_TroopsPanel.xml` 是完整 368KB 的自定义 prefab（不是 vanilla widget 的 patch），跨 mod 再往里注 UI 元素容易随 Retinues 更新失效。当前热键循环方案对 Retinues 版本演进更鲁棒。

## 触发时机

- 我们的 postfix 只在 `RefreshFilter` **自然触发**时运行（切换装备槽、翻页、改搜索文本）——不主动请求 rebuild
- **实操**：按 Ctrl+Shift+C 切换后，随便点一下别的装备槽再回来，或改一下搜索文本，列表就更新了；如果想立即刷新可轻按搜索框任意键再删

如你觉得需要"按下热键立即刷新"，可以升级方案——通过反射调 `_needsRebuild = true` 再 `RefreshFilter()`。目前保守起见留给自然触发。

## 目录结构

```
RetinuesCultureFilter/
├── SubModule.xml                        # 依赖 Harmony + UIExtenderEx + Retinues
├── src/
│   ├── SubModule.cs                    # 主逻辑（<200 行）
│   └── RetinuesCultureFilter.csproj    # net472
├── deploy.ps1
└── README.md
```

## 前置

- **Harmony** (Workshop 2859188632)
- **UIExtenderEx** (Workshop 2859222409) - 未来加 dropdown UI 用
- **Retinues** (Workshop 3599557394)

## 部署

```powershell
cd C:\Users\situj\git\Mount-and-Blade-2-Bannerlord-custome-development\RetinuesCultureFilter
.\deploy.ps1
```

## 卸载

launcher 取消勾选或删 `Modules\RetinuesCultureFilter\`。无 SaveableField，存档兼容。

## v1.0 已知限制 + v1.1 potential

- 当前仅热键 UX，未在 Retinues 编辑器界面里加可视 dropdown。若你觉得需要 dropdown（例如打给别人用），v1.1 可以加 `PrefabExtensionInsertPatch` XAML 注入。
- 循环顺序固定为 6 vanilla 文化 + All，不含 mod 加的自定义文化（如任何加了新 faction 的 mod）。若需要动态支持所有 Culture.All，v1.1 可以扩。

## 版本

- v1.0.0 (2026-09-20)
- 数据源：Retinues v1.4.14.31
- 游戏：Bannerlord Native v1.4.7 (build 117484)
