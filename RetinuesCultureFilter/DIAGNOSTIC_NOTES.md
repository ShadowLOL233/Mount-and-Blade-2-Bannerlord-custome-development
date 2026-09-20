# RetinuesCultureFilter — 诊断笔记（2026-09-20，用户实测 v1.0 后收集）

**上下文**：v1.0 部署后，用户在 Retinues Troop Editor 里按 Ctrl+Shift+C 切换文化 → 点装备槽 → **列表毫无变化**。以下为 sess 结尾深挖出的根因 + 具体下次开发步骤。写在这里是防 chat 上下文被清后信息丢失。

---

## Root cause 定位：patch 打错方法了

反编译 `Retinues.GUI.Editor.VM.Equipment.List.EquipmentListVM` 得到调用链：

```
用户点装备槽 UI
    ↓ vanilla EquipmentIndex 触发 State.Slot 变化
    ↓ 触发 EquipmentListVM.OnSlotChange()  (protected override)
        │  内部：
        │     _needsRebuild = true;
        │     ... (无 RefreshFilter 调用!)
        │     Build();             ← 直接调 Build，跳过 RefreshFilter
        │
用户改搜索文本
    ↓ OnFilterTextChanged()
        │     RebuildVisibleFromSnapshot();  (private)
        │
用户切派系 (dropdown)
    ↓ OnFactionChange()
        │     _needsRebuild = true;
        │     Build();             ← 也直接调 Build
```

**结论**：`OnSlotChange` / `OnFactionChange` 都**直接调 `Build()`**，从不经过 `RefreshFilter`。

我们 v1.0 的 Harmony postfix 挂在 `RefreshFilter` 上——**只有当用户改搜索文本才会触发**，点装备槽/切派系时不触发。这就是"过滤没效果"的原因。

## Fix 步骤（下次 session 第 1 件事）

**改一行**：把 postfix target 从 `RefreshFilter` 改到 `Build`。

```csharp
[HarmonyPatch(typeof(EquipmentListVM), nameof(EquipmentListVM.Build))]
public static class BuildFilterPatch
{
    public static void Postfix(EquipmentListVM __instance)
    {
        string wanted = CultureFilterState.CurrentCultureId;
        if (string.IsNullOrEmpty(wanted)) return;
        var rows = __instance.EquipmentRows;
        if (rows == null) return;
        for (int i = rows.Count - 1; i >= 0; i--)
        {
            var row = rows[i];
            if (row == null || row.RowItem == null) continue;
            string cultureId = row.RowItem.Culture?.StringId?.ToLowerInvariant() ?? "";
            if (cultureId != wanted) rows.RemoveAt(i);
        }
    }
}
```

**关键校验**（fix 后必测）：
1. Build() 是 `public void Build()`——Harmony 可直接 patch，无需 AccessTools + BindingFlags
2. Build() 内部头部有 `if (!_needsRebuild) return;` early-return——**postfix 仍会执行**（Harmony postfix 在原方法返回后跑，不管走哪个 path）
3. Build() 内部有 cache 命中路径（第二次点同 slot），也走 `RebuildVisibleFromSnapshot()` 填 EquipmentRows——postfix 后 EquipmentRows 已填好，可以过滤

**同时改的地方**：v1.0 的 `TriggerRefresh()` 现在是个空函数（注释写"依赖自然触发"）。fix 后依然靠自然触发，但由于 Build 在 slot-change 时会跑，用户只需切换一次装备槽即可看到过滤生效（不像 v1.0 需要改搜索文本才行）。

**若 fix 后还不生效的备选诊断**：
- Retinues 的 `[SafeClass]` attribute 可能在 IL 层重写方法（用 Cecil/Fody）。若真如此，`Build()` 编译后的 IL 已被 try/catch 包裹，但 Harmony postfix 依然应在原方法返回后跑。**Harmony 与 SafeClass 混用不冲突**——这是 CLR 层，不是 IL 层
- 若 patch 装了但没跑，检查启动 log 里 Harmony 是否报 "unable to find method"——若报，说明方法签名对不上，用 `AccessTools.Method(typeof(EquipmentListVM), "Build")` 显式定位
- 若 patch 跑了但没效果，检查 `Culture.StringId` 实际值——用 `InformationManager.DisplayMessage($"row.Culture={cultureId}, wanted={wanted}")` 打消息栏观察

---

## Culture.StringId 值格式确认

RBM/vanilla item XML 里写 `culture="Culture.aserai"`（含 "Culture." 前缀）。Bannerlord XML 解析器**剥掉前缀**，实际 `BasicCultureObject.StringId = "aserai"`（**小写、无前缀**）。

`WCulture.StringId => Base?.StringId ?? Name`——直接透传 vanilla StringId，所以也是 `"aserai"` / `"empire"` / `"vlandia"` / `"battania"` / `"sturgia"` / `"khuzait"` / `"looters"` / `"neutral_culture"` / `"nord"`。

我们 v1.0 的 CycleOrder 值 `["", "empire", "vlandia", "aserai", "battania", "sturgia", "khuzait"]` **格式正确**——不用改。

其他 vanilla culture 也存在（nord/looters/neutral_culture）但通常无装备。**未来可能想加**：mod 加的自定义 faction 会有新 StringId，v1.0 硬编码 6 文化会漏。v1.2 可用 `Campaign.Current.ObjectManager.GetObjectTypeList<CultureObject>()` 动态枚举。

---

## v1.1 可视 dropdown UI 开发前置信息

用户明确说 fix v1.0 filter 优先于 v1.1 dropdown。但下次 session 若还有 usage 可以做的话，以下是核实过的技术底：

### XAML 注入点（`ClanScreen_TroopsPanel.xml`）

Retinues 用 `PrefabExtensionInsertPatch` 把整个 368 KB 的 troop editor prefab 注入到某处。装备列表相关的注入点：

- **Sort Row**（第 2983-3025 行）：`<ListPanel Id="SortButtons" DataSource="{EquipmentList}" ...>` 里有 4 个 `SortButtonWidget`（Name/Category/Tier/Value）
- **Filter Row**（第 3027-3081 行）：类似 `ListPanel Id="SortButtons"`（**同 Id 但另一个** panel），里面有 `<EditableTextWidget Text="@FilterText" ...>` 搜索框
- **List Panel**（第 3083+ 行）：装备行本体

**推荐注入位置**：在 Filter Row（3027-3081）**之后**插入一个新 ListPanel 放 7 个文化按钮。用 `PrefabExtensionInsertAsSiblingPatch` + XPath 定位 Filter Row 的 ListPanel。

### 用什么 widget 做"文化按钮"

**不要用真 dropdown**——vanilla Bannerlord 没有独立 `<Dropdown>` widget，character creation 用的是 SelectorButton + 弹层 popup，复杂。

**改用 `SortButtonWidget`（Retinues 已用）**——7 个按钮排一行，每个 IsSelected 绑定 mixin 的 `CultureEmpireSelected` 等布尔属性。视觉与现有 Sort Row 一致，用户熟悉。

模板参考（复制自 Retinues 现有第 2986 行）：

```xml
<SortButtonWidget DoNotPassEventsToChildren="true"
                  WidthSizePolicy="Fixed" HeightSizePolicy="Fixed"
                  SuggestedHeight="!Clan.Members.Sort.1.Height"
                  SuggestedWidth="120"
                  Brush="Clan.Members.Sort.1"
                  Command.Click="ExecuteSelectCultureEmpire"
                  IsSelected="@CultureEmpireSelected"
                  SortState="@CultureEmpireState"
                  SortVisualWidget="Text\NameSortArrow"
                  UpdateChildrenStates="true">
  <Children>
    <TextWidget Id="Text" WidthSizePolicy="CoverChildren" HeightSizePolicy="CoverChildren"
                HorizontalAlignment="Center" VerticalAlignment="Center"
                Brush="Clan.LeftPanel.Header.Text" Text="Empire"/>
  </Children>
</SortButtonWidget>
```

7 组这样的重复即可。

### UIExtenderEx 注入 API

**关键类**（已反编译核实）：
- `Bannerlord.UIExtenderEx.Prefabs2.PrefabExtensionInsertPatch` — 主 insert
- `Bannerlord.UIExtenderEx.Prefabs.PrefabExtensionInsertAsSiblingPatch` — sibling insert（我们要的）
- `Bannerlord.UIExtenderEx.Attributes.PrefabExtensionAttribute` — 类装饰器

**用法示例**（代码骨架）：

```csharp
[PrefabExtension("ClanScreen_TroopsPanel", 
                 "descendant::ListPanel[@Id='SortButtons'][.//EditableTextWidget[@Text='@FilterText']]",
                 PrefabExtensionInsertAsSiblingPatch.InsertType.Append)]
public class CultureButtonRowInsert : PrefabExtensionInsertAsSiblingPatch
{
    [PrefabExtensionFileName]
    public string GetXmlFilename() => "CultureButtonRow.xml";
}
```

XPath 用 `[.//EditableTextWidget[@Text='@FilterText']]` 精确定位到含搜索框的那个 ListPanel（避免与 Sort Row 的同名 ListPanel 混淆）。

### VM mixin

用 `[ViewModelMixin(typeof(EquipmentListVM))]`，加：
- `[DataSourceProperty] bool CultureAllSelected => CultureFilterState.CurrentIndex == 0;` （不能直接读静态，需 `OnPropertyChanged` refresh）
- 7 个 `Execute*` 方法调 `CultureFilterState.SetIndex(i)` 再触发 `OnPropertyChangedWithValue` 通知 UI

**注意**：`EquipmentListVM` 是 `sealed`——只能用 mixin，不能继承。UIExtenderEx 的 `[ViewModelMixin]` 支持 sealed target。

### Retinues 的 `[SafeClass]` attribute

反编译看：SafeClassAttribute 类**只定义了默认值属性**（PublicOnly / IncludeAccessors / SwallowByDefault 等），**在 Retinues.dll 里未见 IL 重写器或运行时 scanner** 使用它。可能是：
1. 作者预留了 marker，还未实装拦截逻辑（TODO）
2. 通过反射在别处扫描（我没找到）
3. 通过 Fody/Cecil 在 build 时重写 IL（DLL 里应该有痕迹）

**结论**：对 Harmony 无实际影响，可忽略。若发现 patch 结果被吞（比如 postfix 里 throw 但看不见），检查 Retinues log `Configs\ModLogs\` 有无异常记录。

---

## 下次开发的开工顺序建议

1. **【urgent】fix v1.0 filter**：把 postfix 从 `RefreshFilter` 改到 `Build`（1 行代码 + rebuild + deploy，~5 min）
2. **实机验证**：Ctrl+Shift+C 切文化 → 点装备槽 → 列表按选中文化过滤
3. **若 fix OK 且用户还想要 v1.1 dropdown UI**：
   - 建 `RetinuesCultureFilter/GUI/PrefabExtensions/CultureButtonRow.xml`（7 SortButtonWidget）
   - 加 `RetinuesCultureFilter.Mixins.EquipmentListVMMixin`（DataSourceProperty + Execute methods）
   - 加 `[PrefabExtension(...)]` 装饰器类
   - SubModule.cs 加 `new UIExtender("RetinuesCultureFilter").Register(...).Enable();`
   - rebuild + deploy
4. **可选 v1.2**：动态枚举 CultureObject（支持 mod 加的 faction），把 `CycleOrder` 硬编码改为 runtime enumerated

---

## 已核实的 API 边界（供下次快速上手）

| API | 类型 | 位置 |
|---|---|---|
| `EquipmentListVM.Build()` | **public** void | Retinues.GUI.Editor.VM.Equipment.List |
| `EquipmentListVM.RefreshFilter()` | public override void | 同上 |
| `EquipmentListVM.OnSlotChange()` | protected override void（点装备槽触发）| 同上 |
| `EquipmentListVM.OnFactionChange()` | protected override void | 同上 |
| `EquipmentListVM.EquipmentRows` | public `MBBindingList<EquipmentRowVM>` | 同上 |
| `EquipmentRowVM.RowItem` | public readonly `WItem` | Retinues.GUI.Editor.VM.Equipment.List |
| `WItem.Culture` | public WCulture | Retinues.Game.Wrappers |
| `WCulture.StringId` | override string（透传 vanilla）| Retinues.Game.Wrappers |
| `SortButtonWidget` | vanilla Bannerlord widget | Native prefabs |
| `PrefabExtensionInsertAsSiblingPatch` | UIExtenderEx | Bannerlord.UIExtenderEx.Prefabs |

---

## 版本上下文快照

- Bannerlord Native v1.4.7 (build 117484)
- Retinues v1.4.14.31（Workshop 3599557394）
- Bannerlord.UIExtenderEx v2.13.3
- Bannerlord.Harmony v2.4.2.248
- RetinuesCultureFilter v1.0.0（本 mod，当前状态：filter broken，等 Build fix）
