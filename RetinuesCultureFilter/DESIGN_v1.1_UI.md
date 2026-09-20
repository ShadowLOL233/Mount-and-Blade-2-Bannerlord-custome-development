# RetinuesCultureFilter v1.1 — 可视化文化过滤 UI 设计文档

**目标读者**：下次 session 的实施者（可能是你自己 or Claude 重开档载入）。**本文档 self-contained**——照做即可实施完整 v1.1，不需回翻聊天记录。

**前置**：v1.0 的 filter bug 已按 DIAGNOSTIC_NOTES.md 修好（`RefreshFilter` → `Build`）。v1.1 在此基础上加可视 UI 层。

---

## §1 设计决策与理由

### §1.1 选型：Segmented Button Row（分段按钮行），非真 dropdown popup

**放弃真 dropdown 的理由**：
- vanilla Bannerlord 没有独立 `<Dropdown>` widget class（Native prefabs 里 grep 无匹配）
- 角色创建界面用的是 `SelectorButton` + 弹层 popup 模式——需 3 个 widget（button/list/popup）+ 焦点管理逻辑，实现复杂
- Retinues 现有 Sort Row 就是用 4 个 `SortButtonWidget` 一字排开——沿用**视觉一致 + 无需新 widget**

**Segmented Button Row = 7 个 `SortButtonWidget` 排一行**：
- 顺序：`All | Empire | Vlandia | Aserai | Battania | Sturgia | Khuzait`
- 单选逻辑：任一时刻只有一个 IsSelected = true
- 位置：Retinues 现有 Filter Row（搜索框）之下、装备列表之上——插入一整个新 ListPanel

**优点**：
- 所有 7 个选项永远可见——用户看得到有什么可切
- 视觉与 Sort Row 完全同款——用户学一次就会
- 无 popup/焦点管理——mixin + XAML 组合即可，无需额外 Gauntlet 逻辑
- 键盘/手柄导航天然支持（`SortButtonWidget` 是 navigatable widget）

**缺点**：
- 占一行垂直空间（约 57 px，与 Sort Row 一致）
- 若日后要支持 modded factions（超过 6 个 vanilla 文化），button row 会变长——v1.2 可考虑真 dropdown

### §1.2 保留 v1.0 的 hotkey 循环

Ctrl+Shift+C / Ctrl+Shift+X 仍工作——UI 与 hotkey 双绑到同一个 `CultureFilterState.CurrentIndex`。热键改变 index 时通知 mixin 刷新 UI；点按钮时更新 index。

### §1.3 不加"按文化排序"作为第 5 个 sort 模式

**理由**：用户实际需求是"只看某文化的装备来对比数值"——**过滤**而非排序。按文化字母排序只是把 Empire 头盔和 Aserai 头盔集中显示，但你依然要在 mixed list 里找目标文化的东西。过滤直接把非目标文化剔除，更符合"文化平衡工作"的实际操作流。

若你日后想加"按文化排序"，是**独立的第二步**——扩 `SortMode` 私有 enum 需 Harmony patch，复杂度另计。v1.1 只做过滤。

---

## §2 目录结构（v1.1 新增/修改）

```
RetinuesCultureFilter/
├── SubModule.xml                         # (v1.0 已有；无需改)
├── src/
│   ├── RetinuesCultureFilter.csproj      # (需加 Bannerlord.UIExtenderEx.dll 引用 — v1.0 已引但未用)
│   ├── SubModule.cs                       # (改：加 UIExtender.Enable() 注册)
│   └── Mixins/                            # NEW
│       └── EquipmentListVMMixin.cs        # NEW mixin 主体
├── GUI/                                   # NEW 整个目录
│   └── PrefabExtensions/                  # NEW
│       └── CultureButtonRow.xml           # NEW XAML 片段
└── DIAGNOSTIC_NOTES.md + DESIGN_v1.1_UI.md
```

---

## §3 完整代码 · 拿来即用

### §3.1 `src/Mixins/EquipmentListVMMixin.cs`（全新文件）

```csharp
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using Retinues.GUI.Editor.VM.Equipment.List;

namespace RetinuesCultureFilter.Mixins
{
    // Mixin adds 7 IsSelected properties + 7 Execute methods to Retinues' sealed EquipmentListVM,
    // driving the visual toggle buttons in CultureButtonRow.xml. Shared state lives in the static
    // CultureFilterState class so hotkey (Ctrl+Shift+C) and UI stay in sync.
    [ViewModelMixin]
    public sealed class EquipmentListVMMixin : BaseViewModelMixin<EquipmentListVM>
    {
        public EquipmentListVMMixin(EquipmentListVM vm) : base(vm) { }

        // ---- IsSelected bindings (bind to SortButtonWidget.IsSelected="@CultureXxxSelected") ----
        [DataSourceProperty] public bool CultureAllSelected     => CultureFilterState.CurrentIndex == 0;
        [DataSourceProperty] public bool CultureEmpireSelected  => CultureFilterState.CurrentIndex == 1;
        [DataSourceProperty] public bool CultureVlandiaSelected => CultureFilterState.CurrentIndex == 2;
        [DataSourceProperty] public bool CultureAseraiSelected  => CultureFilterState.CurrentIndex == 3;
        [DataSourceProperty] public bool CultureBattaniaSelected=> CultureFilterState.CurrentIndex == 4;
        [DataSourceProperty] public bool CultureSturgiaSelected => CultureFilterState.CurrentIndex == 5;
        [DataSourceProperty] public bool CultureKhuzaitSelected => CultureFilterState.CurrentIndex == 6;

        // ---- Click handlers (bind to SortButtonWidget.Command.Click="ExecuteSelectCultureXxx") ----
        public void ExecuteSelectCultureAll()     { SetIndex(0); }
        public void ExecuteSelectCultureEmpire()  { SetIndex(1); }
        public void ExecuteSelectCultureVlandia() { SetIndex(2); }
        public void ExecuteSelectCultureAseraiI() { SetIndex(3); }
        public void ExecuteSelectCultureBattania(){ SetIndex(4); }
        public void ExecuteSelectCultureSturgia() { SetIndex(5); }
        public void ExecuteSelectCultureKhuzait() { SetIndex(6); }

        // Also called by SubModule.cs hotkey handler to refresh UI when Ctrl+Shift+C changes state.
        public void RefreshCultureButtonStates()
        {
            OnPropertyChangedWithValue(CultureAllSelected,      nameof(CultureAllSelected));
            OnPropertyChangedWithValue(CultureEmpireSelected,   nameof(CultureEmpireSelected));
            OnPropertyChangedWithValue(CultureVlandiaSelected,  nameof(CultureVlandiaSelected));
            OnPropertyChangedWithValue(CultureAseraiSelected,   nameof(CultureAseraiSelected));
            OnPropertyChangedWithValue(CultureBattaniaSelected, nameof(CultureBattaniaSelected));
            OnPropertyChangedWithValue(CultureSturgiaSelected,  nameof(CultureSturgiaSelected));
            OnPropertyChangedWithValue(CultureKhuzaitSelected,  nameof(CultureKhuzaitSelected));
        }

        private void SetIndex(int i)
        {
            CultureFilterState.SetIndex(i);   // updates the shared static; triggers filter re-apply
            RefreshCultureButtonStates();
            // Force list rebuild so filter effect visible immediately (no need to click a slot)
            base.ViewModel?.Build();
        }
    }
}
```

**关键机制**：
- `[ViewModelMixin]` on class → UIExtenderEx 自动关联到 `EquipmentListVM`
- `[DataSourceProperty]` → 属性可被 XAML `@` 语法引用
- `BaseViewModelMixin<T>.ViewModel` 是 target VM 引用
- `OnPropertyChangedWithValue(value, "name")` → 通知 XAML 重绑
- `base.ViewModel.Build()` → v1.0 fix 后 Build 是 public，可直接调触发刷新

### §3.2 `GUI/PrefabExtensions/CultureButtonRow.xml`（全新文件）

```xml
<!-- 7-button segmented culture filter row.
     Injected by RetinuesCultureFilter into Retinues' ClanScreen_TroopsPanel prefab,
     as sibling AFTER the existing Filter Row (which holds the search text box). -->
<ListPanel Id="CultureFilterRow"
           DataSource="{EquipmentList}"
           WidthSizePolicy="CoverChildren"
           HeightSizePolicy="Fixed"
           SuggestedHeight="57"
           RenderLate="true">
  <Children>
    <SortButtonWidget DoNotPassEventsToChildren="true"
                      WidthSizePolicy="Fixed" HeightSizePolicy="Fixed"
                      SuggestedHeight="!Clan.Members.Sort.1.Height" SuggestedWidth="105"
                      Brush="Clan.Members.Sort.1"
                      Command.Click="ExecuteSelectCultureAll"
                      IsSelected="@CultureAllSelected"
                      UpdateChildrenStates="true">
      <Children>
        <TextWidget WidthSizePolicy="CoverChildren" HeightSizePolicy="CoverChildren"
                    HorizontalAlignment="Center" VerticalAlignment="Center"
                    Brush="Clan.LeftPanel.Header.Text" Text="All"/>
      </Children>
    </SortButtonWidget>
    <SortButtonWidget DoNotPassEventsToChildren="true"
                      WidthSizePolicy="Fixed" HeightSizePolicy="Fixed"
                      SuggestedHeight="!Clan.Members.Sort.1.Height" SuggestedWidth="105"
                      Brush="Clan.Members.Sort.1"
                      Command.Click="ExecuteSelectCultureEmpire"
                      IsSelected="@CultureEmpireSelected"
                      UpdateChildrenStates="true">
      <Children>
        <TextWidget WidthSizePolicy="CoverChildren" HeightSizePolicy="CoverChildren"
                    HorizontalAlignment="Center" VerticalAlignment="Center"
                    Brush="Clan.LeftPanel.Header.Text" Text="Empire"/>
      </Children>
    </SortButtonWidget>
    <SortButtonWidget DoNotPassEventsToChildren="true"
                      WidthSizePolicy="Fixed" HeightSizePolicy="Fixed"
                      SuggestedHeight="!Clan.Members.Sort.1.Height" SuggestedWidth="105"
                      Brush="Clan.Members.Sort.1"
                      Command.Click="ExecuteSelectCultureVlandia"
                      IsSelected="@CultureVlandiaSelected"
                      UpdateChildrenStates="true">
      <Children>
        <TextWidget WidthSizePolicy="CoverChildren" HeightSizePolicy="CoverChildren"
                    HorizontalAlignment="Center" VerticalAlignment="Center"
                    Brush="Clan.LeftPanel.Header.Text" Text="Vlandia"/>
      </Children>
    </SortButtonWidget>
    <SortButtonWidget DoNotPassEventsToChildren="true"
                      WidthSizePolicy="Fixed" HeightSizePolicy="Fixed"
                      SuggestedHeight="!Clan.Members.Sort.1.Height" SuggestedWidth="105"
                      Brush="Clan.Members.Sort.1"
                      Command.Click="ExecuteSelectCultureAseraiI"
                      IsSelected="@CultureAseraiSelected"
                      UpdateChildrenStates="true">
      <Children>
        <TextWidget WidthSizePolicy="CoverChildren" HeightSizePolicy="CoverChildren"
                    HorizontalAlignment="Center" VerticalAlignment="Center"
                    Brush="Clan.LeftPanel.Header.Text" Text="Aserai"/>
      </Children>
    </SortButtonWidget>
    <SortButtonWidget DoNotPassEventsToChildren="true"
                      WidthSizePolicy="Fixed" HeightSizePolicy="Fixed"
                      SuggestedHeight="!Clan.Members.Sort.1.Height" SuggestedWidth="105"
                      Brush="Clan.Members.Sort.1"
                      Command.Click="ExecuteSelectCultureBattania"
                      IsSelected="@CultureBattaniaSelected"
                      UpdateChildrenStates="true">
      <Children>
        <TextWidget WidthSizePolicy="CoverChildren" HeightSizePolicy="CoverChildren"
                    HorizontalAlignment="Center" VerticalAlignment="Center"
                    Brush="Clan.LeftPanel.Header.Text" Text="Battania"/>
      </Children>
    </SortButtonWidget>
    <SortButtonWidget DoNotPassEventsToChildren="true"
                      WidthSizePolicy="Fixed" HeightSizePolicy="Fixed"
                      SuggestedHeight="!Clan.Members.Sort.1.Height" SuggestedWidth="105"
                      Brush="Clan.Members.Sort.1"
                      Command.Click="ExecuteSelectCultureSturgia"
                      IsSelected="@CultureSturgiaSelected"
                      UpdateChildrenStates="true">
      <Children>
        <TextWidget WidthSizePolicy="CoverChildren" HeightSizePolicy="CoverChildren"
                    HorizontalAlignment="Center" VerticalAlignment="Center"
                    Brush="Clan.LeftPanel.Header.Text" Text="Sturgia"/>
      </Children>
    </SortButtonWidget>
    <SortButtonWidget DoNotPassEventsToChildren="true"
                      WidthSizePolicy="Fixed" HeightSizePolicy="Fixed"
                      SuggestedHeight="!Clan.Members.Sort.1.Height" SuggestedWidth="105"
                      Brush="Clan.Members.Sort.1"
                      Command.Click="ExecuteSelectCultureKhuzait"
                      IsSelected="@CultureKhuzaitSelected"
                      UpdateChildrenStates="true">
      <Children>
        <TextWidget WidthSizePolicy="CoverChildren" HeightSizePolicy="CoverChildren"
                    HorizontalAlignment="Center" VerticalAlignment="Center"
                    Brush="Clan.LeftPanel.Header.Text" Text="Khuzait"/>
      </Children>
    </SortButtonWidget>
  </Children>
</ListPanel>
```

**7 个按钮 × 105 SuggestedWidth = 735 px 一整行**——Retinues Sort Row 单按钮 175 px × 4 = 700 px，我们略宽但同数量级，视觉相近。

### §3.3 `src/PrefabExtensions/CultureButtonRowInsert.cs`（全新文件）

```csharp
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace RetinuesCultureFilter.PrefabExtensions
{
    // Injects CultureButtonRow.xml into Retinues' ClanScreen_TroopsPanel prefab,
    // right after the ListPanel that contains the FilterText EditableTextWidget.
    // XPath deliberately checks for the descendant EditableTextWidget to disambiguate
    // between the Sort Row and Filter Row ListPanels (both use Id="SortButtons" in
    // Retinues' XML — they collide by name so we filter by child content).
    [PrefabExtension(
        prefabName: "ClanScreen_TroopsPanel",
        xpath: "descendant::ListPanel[@Id='SortButtons' and .//EditableTextWidget[@Text='@FilterText']]"
    )]
    public sealed class CultureButtonRowInsert : PrefabExtensionInsertPatch
    {
        public override int Index => 1;   // insert AFTER the Filter Row (as the next sibling)
        public override InsertType Type => InsertType.Append;

        [PrefabExtensionFileName(AssemblyRelative = true)]
        public string GetXmlFilename() => "CultureButtonRow";
        // UIExtenderEx will find CultureButtonRow.xml under GUI/PrefabExtensions/.
    }
}
```

**XPath 关键点**：`descendant::ListPanel[@Id='SortButtons' and .//EditableTextWidget[@Text='@FilterText']]` — 匹配 `Id='SortButtons'` 且包含 `@FilterText` 绑定的搜索框的那个 ListPanel。这样避免与 Sort Row（同 Id 但含 4 个 SortButtonWidget）混淆。

**验证 XPath**：Retinues 的 XML 第 3027 行 Filter Row `ListPanel Id="SortButtons"` 里有 `<EditableTextWidget Text="@FilterText" ...>`（第 3075 行）；Sort Row 第 2984 行同 Id 但只有 4 个 SortButtonWidget，无 EditableTextWidget——XPath 谓词精确排除。

### §3.4 修改 `src/SubModule.cs`

在现有 `OnSubModuleLoad` 里加 UIExtender 注册：

```csharp
using Bannerlord.UIExtenderEx;
// ... 现有 usings

public class SubModule : MBSubModuleBase
{
    private const string ExtenderId = "RetinuesCultureFilter";

    protected override void OnSubModuleLoad()
    {
        base.OnSubModuleLoad();

        // 1) Harmony patch — v1.0 patched RefreshFilter (WRONG), v1.1 patches Build
        var harmony = new Harmony(ExtenderId);
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        // 2) UIExtenderEx — register the mixin + prefab extension
        var extender = new UIExtender(ExtenderId);
        extender.Register(Assembly.GetExecutingAssembly());
        extender.Enable();
    }

    // ... 现有 hotkey handling 保持不变，但 CultureFilterState.CycleNext/Clear 里
    //     加一个 hook 通知 mixin 刷新 UI:
}

public static class CultureFilterState
{
    // ... 现有 CycleOrder / CurrentIndex / etc.

    public static void SetIndex(int i)
    {
        if (i < 0 || i >= CycleOrder.Length) return;
        CurrentIndex = i;
        Announce();
        NotifyMixinRefresh();  // NEW — see below
    }

    public static void CycleNext()
    {
        SetIndex((CurrentIndex + 1) % CycleOrder.Length);
    }

    public static void Clear()
    {
        SetIndex(0);
    }

    // NEW: notify the currently-live mixin instance to refresh its UI bindings.
    // If no mixin instance is alive (editor not open), this is a no-op.
    private static void NotifyMixinRefresh()
    {
        // The mixin instance is created by UIExtenderEx when EquipmentListVM is instantiated.
        // We can find it via UIExtender.GetMixin<T>() or by keeping a static weak reference.
        // Simplest: mixin registers itself on construction to a static list, unregisters on destroy.
        Mixins.EquipmentListVMMixin.RefreshAllLiveInstances();
    }
}
```

并在 mixin 里加：

```csharp
public sealed class EquipmentListVMMixin : BaseViewModelMixin<EquipmentListVM>
{
    private static readonly System.Collections.Generic.List<System.WeakReference<EquipmentListVMMixin>> _liveInstances
        = new System.Collections.Generic.List<System.WeakReference<EquipmentListVMMixin>>();

    public EquipmentListVMMixin(EquipmentListVM vm) : base(vm)
    {
        _liveInstances.Add(new System.WeakReference<EquipmentListVMMixin>(this));
    }

    public static void RefreshAllLiveInstances()
    {
        _liveInstances.RemoveAll(w => !w.TryGetTarget(out _));
        foreach (var w in _liveInstances)
            if (w.TryGetTarget(out var m))
                m.RefreshCultureButtonStates();
    }

    // ... 其余属性 + 方法照 §3.1
}
```

### §3.5 修改 `src/SubModule.cs` 的 Harmony patch target

**关键 v1.0→v1.1 变更**：把 `RefreshFilter` 改为 `Build`。

```csharp
// v1.0 (WRONG - filter doesn't trigger on slot clicks):
// [HarmonyPatch(typeof(EquipmentListVM), nameof(EquipmentListVM.RefreshFilter))]

// v1.1 (CORRECT):
[HarmonyPatch(typeof(EquipmentListVM), nameof(EquipmentListVM.Build))]
public static class BuildFilterPatch
{
    public static void Postfix(EquipmentListVM __instance)
    {
        string wanted = CultureFilterState.CurrentCultureId;
        if (string.IsNullOrEmpty(wanted)) return;   // "All" = no filter
        var rows = __instance.EquipmentRows;
        if (rows == null) return;
        for (int i = rows.Count - 1; i >= 0; i--)
        {
            var row = rows[i];
            if (row == null || row.RowItem == null) continue;   // keep unequip placeholder
            string cultureId = row.RowItem.Culture?.StringId?.ToLowerInvariant() ?? "";
            if (cultureId != wanted) rows.RemoveAt(i);
        }
    }
}
```

### §3.6 `src/RetinuesCultureFilter.csproj` 无需改动

v1.0 已经引了 `Bannerlord.UIExtenderEx.dll`（虽然 v1.0 未用其属性）。v1.1 直接用。

---

## §4 XAML 注入路径（关键之关键）

### §4.1 目标 prefab 名称

UIExtenderEx 的 `PrefabExtensionAttribute.prefabName` 需要精确匹配。Retinues 用 `ClanScreen_TroopsPanel` 作为它的 injected prefab 名称——**这就是我们要 target 的名称**。

**验证**：Retinues 的 `GUI/PrefabExtensions/ClanScreen/ClanScreen_TroopsPanel.xml` 就是这个 prefab 的内容体。UIExtenderEx 让多个 mod 可以对同一个 prefab 名称都做扩展——mod 加载顺序决定谁先应用（都会应用）。

**风险**：Retinues 是把整个 UI 用 `PrefabExtensionInsertPatch` 注入到 vanilla 的某个 encyclopedia prefab 里的——它自己的"prefab"实际上是一个 subtree。我们 target `ClanScreen_TroopsPanel` 时 UIExtenderEx 会先解析 Retinues 的 subtree，再在解析后的树里跑我们的 XPath。**理论上可行**——UIExtenderEx 官方 docs 支持"extend an extension"。若实测发现 XPath 找不到目标，备选路径见 §4.3。

### §4.2 XPath 精确定位

**完整 XPath**：`descendant::ListPanel[@Id='SortButtons' and .//EditableTextWidget[@Text='@FilterText']]`

**分解**：
- `descendant::ListPanel` — 树内任意深度的 ListPanel
- `[@Id='SortButtons']` — Id 属性 = "SortButtons"
- `[.//EditableTextWidget[@Text='@FilterText']]` — 其后代含 `Text='@FilterText'` 的 EditableTextWidget（`.//` 表示"任意深度后代"）

**为何这样写**：Retinues 用 `Id="SortButtons"` **两次**（Sort Row 第 2984 行 + Filter Row 第 3027 行），仅靠 Id 会撞。用"含 FilterText 后代"的谓词精确挑到 Filter Row。

### §4.3 备选注入路径（若 §4.2 XPath 失败）

**Plan B**：直接 target Filter Row 里的搜索框 EditableTextWidget，作为其 SIBLING 插入。

```csharp
xpath: "descendant::EditableTextWidget[@Text='@FilterText']"
// InsertType 改成 InsertAsSibling（插到 EditableTextWidget 之后）
```

**Plan C**：若跨-mod PrefabExtension 完全不 work，改用**运行时 UI 注入**——`OnAfterMissionCreated` 或 `OnGameInitializationFinished` 里拿到活跃的 GauntletMovie/Widget 引用，用反射插新 widget。**极后备**，复杂度高，避免。

---

## §5 逐步实施 checklist（下次 session 顺序）

以下步骤按 dependency 顺序、每一步独立可测：

- [ ] **1** 应用 v1.0 fix：把 `SubModule.cs` 里 `[HarmonyPatch(..., nameof(RefreshFilter))]` 改为 `nameof(Build)`；rebuild + deploy；实机验证 Ctrl+Shift+C 切文化 → 点装备槽 → filter 生效
- [ ] **2** 建 `src/Mixins/` 目录，写 `EquipmentListVMMixin.cs`（§3.1 内容）
- [ ] **3** 建 `GUI/PrefabExtensions/` 目录，写 `CultureButtonRow.xml`（§3.2 内容）
- [ ] **4** 建 `src/PrefabExtensions/CultureButtonRowInsert.cs`（§3.3 内容）
- [ ] **5** 改 `src/SubModule.cs`：加 `UIExtender.Enable()` 注册（§3.4）+ `CultureFilterState.SetIndex()` 加通知 mixin 逻辑
- [ ] **6** 改 `CultureFilterState.CycleNext/Clear` 内部都改调 `SetIndex()` 走通知路径
- [ ] **7** 版本升到 v1.1.0：`SubModule.xml` `<Version value="v1.1.0" />`
- [ ] **8** rebuild：`deploy.ps1`，观察 0 warn/0 err
- [ ] **9** LauncherData 更新 `<LastKnownVersion>v1.1.0.0</LastKnownVersion>`
- [ ] **10** 实机验证：
  - [ ] Retinues Troop Editor 打开后，Filter Row 下方出现 7 个文化按钮
  - [ ] 点 Aserai → 列表只剩 Aserai 装备（不需切槽）
  - [ ] 点 All → 列表恢复全部
  - [ ] 按 Ctrl+Shift+C → 按钮选中状态跟着切换
  - [ ] 存档 + 读档 → filter 状态不保留（这是期望——`CurrentIndex` 是 static、不 sync 存档；每次打开编辑器默认 "All"）
- [ ] **11** journal 加 modification #17 记录 v1.1 落地

---

## §6 排错清单（若某步失败）

| 症状 | 可能原因 | 快速排查 |
|---|---|---|
| button row 完全不出现 | XPath 未匹配 | 改用 §4.3 Plan B XPath；检查 `Configs\ModLogs\UIExtenderEx*.log` 有无 warn |
| button 出现但点击无反应 | mixin 未注册 | 检查 SubModule.cs 里 `extender.Enable()` 已调；检查 mixin 类是 `public sealed` + 有 `[ViewModelMixin]` |
| 点击生效但 IsSelected 不视觉反馈 | OnPropertyChanged 时机不对 | 确认 `RefreshCultureButtonStates()` 在 `SetIndex` 里被调；也可加 `NotifyPropertyChanged(nameof(...))` 兜底 |
| filter 生效但 hotkey 切换 UI 不响应 | `NotifyMixinRefresh` 没触发或找不到活 mixin | 检查 `_liveInstances` 是否被 GC 清空（WeakReference 语义）；改用 strong reference 或 `HashSet` |
| 编辑器不出现 mixin 属性 | UIExtenderEx 版本不兼容 | 确认 `Bannerlord.UIExtenderEx.dll` 是 v2.13.3；查 UIExtenderEx changelog 看 `[ViewModelMixin]` API 稳定性 |
| 点其他 mod 的装备（如 OSA）filter 不生效 | Culture 匹配错误 | 检查 `WItem.Culture.StringId` 实际值——加临时消息栏 log 印证 |

---

## §7 更远的 v1.2 备忘（不做但记录）

- **动态 culture 列表**：`Campaign.Current.ObjectManager.GetObjectTypeList<CultureObject>().Where(c => c.IsMainCulture)` 枚举——支持 modded factions；button row 需变动态数量的 ItemTemplate 而非硬 7 个
- **加"按文化排序"作为 5th sort mode**：扩 SortMode 私有 enum 需 Harmony `Transpiler`——比 postfix 复杂一档
- **持久化 filter 选择**：把 `CultureFilterState.CurrentIndex` 写进 MCM 或存档，每次读档恢复上次选中
- **可选：culture flag icon** 代替文字——需 Brush 定义，视觉更好但需要图像资源

---

## §8 关键 API 快查表（重复 DIAGNOSTIC_NOTES §API 供本文档独立完备）

| API | 类型 | Location |
|---|---|---|
| `EquipmentListVM.Build()` | **public** void | Retinues.GUI.Editor.VM.Equipment.List |
| `EquipmentListVM.EquipmentRows` | public MBBindingList\<EquipmentRowVM\> | 同上 |
| `EquipmentRowVM.RowItem` | public readonly WItem | Retinues.GUI.Editor.VM.Equipment.List |
| `WItem.Culture` | public WCulture | Retinues.Game.Wrappers |
| `WCulture.StringId` | override string（透传 vanilla；`"aserai"` 小写无前缀） | Retinues.Game.Wrappers |
| `SortButtonWidget` | Bannerlord vanilla widget（Native prefabs 已有）| — |
| `[ViewModelMixin]` | attribute | Bannerlord.UIExtenderEx.Attributes |
| `[PrefabExtension(prefabName, xpath)]` | attribute | Bannerlord.UIExtenderEx.Attributes |
| `PrefabExtensionInsertPatch` | base class | Bannerlord.UIExtenderEx.Prefabs2 |
| `UIExtender(id).Register(assembly).Enable()` | runtime API | Bannerlord.UIExtenderEx |
| `BaseViewModelMixin<T>.ViewModel` | inherited property | Bannerlord.UIExtenderEx.ViewModels |

---

## §9 版本上下文快照（供跨-session 兼容性对齐）

- Bannerlord Native **v1.4.7** (build 117484)
- Retinues **v1.4.14.31**（Workshop 3599557394）
- Bannerlord.UIExtenderEx **v2.13.3**（Workshop 2859222409）
- Bannerlord.Harmony **v2.4.2.248**（Workshop 2859188632）
- RetinuesCultureFilter 当前版本：**v1.0.0**（filter broken；等 v1.1 fix + UI）

**若上述任一版本改变**：重新反编译核对 API 是否漂移，特别是：
- Retinues 的 `EquipmentListVM` 方法签名
- UIExtenderEx 的 `[ViewModelMixin]` / `[PrefabExtension]` API
- Retinues 是否改了 XAML 结构（`ClanScreen_TroopsPanel.xml` 里 Sort/Filter Row 布局）
