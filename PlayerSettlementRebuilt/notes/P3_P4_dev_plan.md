# PSLR P3-P4 · 开发方案设计

**日期**：2026-10-10
**scope**：完整 rebuild · 搭 csproj → fix decomp → build → deploy

---

## 1. 核心架构决策

### 1.1 产出几个 DLL · 哪些 bundle

**原 PS v7.5.0 workshop 产出**：
```
bin/Win64_Shipping_Client/
├── PlayerSettlement.dll          283,648 B   (我们要 rebuild)
├── PlayerSettlementFixes.dll      64,512 B   (我们要 rebuild)
├── PlayerSettlement.pdb            8,100 B
├── PlayerSettlementFixes.pdb      19,328 B
├── 0Harmony.dll                2,461,696 B   (第三方 bundle · 冗余)
├── MCMv5.dll                     495,104 B   (🔴 冲突真因 · v5.11.3)
├── Newtonsoft.Json.dll           115,856 B?  (第三方 bundle · 冗余)
└── System.Numerics.Vectors.dll   115,856 B   (BCL · 冗余)
```

**我们的 rebuild 产出**（🟢 **推荐**）：
```
bin/Win64_Shipping_Client/
├── PlayerSettlement.dll          (new · built from decomp source)
├── PlayerSettlement.pdb
├── PlayerSettlementFixes.dll     (new · built from decomp source)
└── PlayerSettlementFixes.pdb
```

**不 bundle 任何第三方 dll** · 全部走 reference-only：
- `0Harmony.dll` → 用 `workshop/2859188632/bin/.../0Harmony.dll`
- `MCMv5.dll` → 用 `workshop/2859238197/bin/.../MCMv5.dll`（**彻底消灭冲突根源**）
- `Newtonsoft.Json.dll` → 用 TaleWorlds 游戏自带的
- `System.Numerics.Vectors.dll` → 用 .NET Framework 自带的

**好处**：
- 彻底消灭 bundled MCM 冲突（根因）
- deploy 产物小（<500 KB）· 不污染 Modules 文件夹
- 自动 pick up workshop MCM 的任何未来更新

**风险**：
- 用户必须有 workshop 订阅：MCM v5.12.3 + Harmony + ButterLib + UIExtenderEx（已有 · 都在 enabled 列表）
- 任何一个缺失 · 游戏 launcher 会黄字警告（但 SubModule.xml DependedModule 已声明依赖 · 跟 vanilla PS 行为一致）

### 1.2 保留 2 个 SubModule 的架构

PlayerSettlement + PlayerSettlementFixes 分开保留。**不合并成 1 个 dll** · 原因：
- 保 PSFixes 独立性 · 如果 PS 作者未来复出 · PSFixes 可独立淘汰
- 存档兼容：Bannerlord 保存系统靠 full type name (`BannerlordPlayerSettlement.*`) 识别 · 合并 dll 可能换 Harmony id · 导致老存档加载异常
- Debug 友好：PS 跟 PSFixes 日志分离更清晰

---

## 2. 项目目录结构

```
Mount-and-Blade-2-Bannerlord-custome-development/PlayerSettlementRebuilt/
├── README.md                              (项目说明 + 原作者 attribution + 跟 Nexus 7298 的差异)
├── SubModule.xml                          (改 DependentVersion: UIExt v3.0.3 · MCM v5.12.3)
├── deploy.ps1                             (dotnet build Release + copy 到 Modules\PlayerSettlement\)
├── .gitignore                             (ignore bin/ obj/ decompiled_reference/)
├── decompiled_reference/                  (P1 产出 · 只读参照 · 不改)
│   ├── PlayerSettlement.decompiled.cs
│   ├── PlayerSettlementFixes.decompiled.cs
│   ├── UIExtenderEx_v3/
│   │   └── Bannerlord.UIExtenderEx.decompiled.cs
│   └── MCM_v5123/
│       └── MCMv5.decompiled.cs
├── src/
│   ├── PlayerSettlement/
│   │   ├── PlayerSettlement.csproj        (net472 · output PlayerSettlement.dll)
│   │   └── (所有 85 class 的 .cs 从 reference 拆分 or 单 .cs 整体)
│   └── PlayerSettlementFixes/
│       ├── PlayerSettlementFixes.csproj   (net472 · output PlayerSettlementFixes.dll)
│       └── (所有 43 class 的 .cs · 修 3 处 yield iterator)
├── ModuleData/                            (从 workshop/3720376888/ModuleData/ 整体复制)
│   ├── Player_Settlement_Templates/
│   ├── Player_Settlement_Templates_War_Sails/
│   └── template_blacklist.txt
└── notes/
    ├── P1_decomp_report.md                (✅ 已有)
    ├── P2_api_diff_report.md              (✅ 已有)
    ├── P3_P4_dev_plan.md                  (本文件)
    └── P5_P6_deploy_test.md               (P5-P6 开工后写)
```

**铁律**：`decompiled_reference/` 只读 · `src/` 是真工作区 · 从 reference copy 来后在 src 改。

---

## 3. 源码拆分 vs 整体保留

决策：**单一 .cs 文件整体保留** vs **按原 namespace/class 拆成多个 .cs**？

| 方案 | 优点 | 缺点 |
|---|---|---|
| **A · 单 cs 整体保留** | 简单 · 跟 reference 一一对应 · 修改容易 diff | 文件大（13006 行）· IDE 卡 · git blame 乱 |
| **B · 按 namespace / class 拆** | 符合 .NET 标准 · 可读性好 · git blame 清晰 | 拆分工作量 1-2h · 以后 upstream 无法合入（PS 作者弃坑了 · 无 upstream） |

🟢 **推荐 B**：PS 作者既然弃坑 · 没有 upstream 要合入 · 拆成正常 .NET 项目结构长远更好。

**拆分方案**：按原 reference 的 `namespace` 边界拆：
```
src/PlayerSettlement/
├── Main.cs                            (Main class · OnSubModuleLoad / OnBeforeInitialModuleScreenSetAsRoot)
├── Behaviours/
│   ├── PlayerSettlementBehaviour.cs
│   └── (其他 behaviour)
├── Descriptors/
├── Extensions/
├── HotKeys/
├── Patches/
│   └── Compatibility/
├── Saves/
├── UI/Viewmodels/
├── Utils/
├── Settings.cs                        (MCM Settings class)
└── PlayerSettlement.csproj
```

**拆分工作量** ~2 小时 · 不需要修改任何代码（纯 copy-paste · 按 class 边界切）。

---

## 4. Dependency Reference 清单

csproj 需要 reference 这些（**不 bundle · 只 reference**）：

### 4.1 TaleWorlds 游戏 DLLs（指向游戏 bin 文件夹）
```xml
<HintPath>$(GameDir)\bin\Win64_Shipping_Client\TaleWorlds.Core.dll</HintPath>
<HintPath>...\TaleWorlds.CampaignSystem.dll</HintPath>
<HintPath>...\TaleWorlds.MountAndBlade.dll</HintPath>
<HintPath>...\TaleWorlds.Engine.dll</HintPath>
<HintPath>...\TaleWorlds.Library.dll</HintPath>
<HintPath>...\TaleWorlds.ObjectSystem.dll</HintPath>
<HintPath>...\TaleWorlds.SaveSystem.dll</HintPath>
<HintPath>...\TaleWorlds.GauntletUI.dll</HintPath>
<HintPath>...\TaleWorlds.MountAndBlade.ViewModelCollection.dll</HintPath>
<HintPath>...\TaleWorlds.MountAndBlade.GauntletUI.dll</HintPath>
<HintPath>...\SandBox.dll</HintPath>
<HintPath>...\SandBoxCore.dll</HintPath>
<HintPath>...\StoryMode.dll</HintPath>
<HintPath>...\NavalDLC.dll</HintPath>
```

**$(GameDir)** 变量定义在 csproj 顶部：
```xml
<GameDir Condition="'$(GameDir)' == ''">E:\SteamLibrary\steamapps\common\Mount &amp; Blade II Bannerlord</GameDir>
```

### 4.2 Mod 框架 DLLs（指向 workshop · 都是 workshop 订阅的 mod）
```xml
<HintPath>E:\SteamLibrary\steamapps\workshop\content\261550\2859188632\bin\Win64_Shipping_Client\0Harmony.dll</HintPath>
<HintPath>...\2859232415\bin\...\Bannerlord.ButterLib.dll</HintPath>
<HintPath>...\2859222409\bin\...\Bannerlord.UIExtenderEx.dll</HintPath>
<HintPath>...\2859238197\bin\...\MCMv5.dll</HintPath>
```

**所有 Reference 的 Private=false**（告诉 csproj 不要 copy 这些到 output 目录）：
```xml
<Reference Include="MCMv5">
  <HintPath>...</HintPath>
  <Private>false</Private>
</Reference>
```

### 4.3 .NET Framework DLLs
```xml
<Reference Include="System" />
<Reference Include="System.Core" />
<Reference Include="System.Numerics" />
<Reference Include="System.Xml" />
<Reference Include="System.Xml.Linq" />
```

---

## 5. 需要手工修的 decomp 缺陷

### 5.1 PSFixes yield iterator × 3

**位置**：PSFixes.cs 行 1059 · 3250 · 3257
**症状**：`//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()`
**修法**：
1. 看 decomp 生成的 `<MethodName>d__N` 伪代码（state machine）
2. 根据 state transitions 还原原 iterator 逻辑
3. 手工重写成 `yield return` + `foreach` 风格

**预估**：每个 30 min · 共 1.5 h

### 5.2 PS IL mismatch × 1

**位置**：PS.cs 行 7839 `\IL_00bf: Expected O, but got Unknown`
**症状**：某处 IL instruction 跟 ILSpy 预期类型不符 · 通常是 `using var`、`fixed`、`unsafe` 块等
**修法**：
1. 看周边代码 context · 判断是什么构造
2. 若 `using var` · 改成标准 `using (var ...) { ... }`
3. 若 `fixed` · 加 `unsafe` 修饰
4. 实在不行 · 用 dnSpy 看原 IL · 手工翻译

**预估**：15-30 min

### 5.3 其他可能的 build errors

反编译源**首次 build 肯定不干净** · 常见坑：
- `[CompilerGenerated]` 字段名有 `<>` · 不能跨程序集访问 · 若 PSFixes 访问 PS 的 `<field>` · 需加 `InternalsVisibleTo`
- `?` nullable annotation 不一致 · 需加 `#nullable enable` 或清除所有 `?`
- `required` keyword · `init` setter 等 C# 11+ 特性 · 需调 `<LangVersion>` 到 `preview` 或 `latest`
- `global::` 前缀 · 移除
- `extern alias` 处理

**预估**：build 迭代 2-5 轮消误 · 共 1-2 h

---

## 6. SubModule.xml 修改

当前 workshop PS SubModule.xml 已经 fine · 只需微调 `DependentVersion` 到当前实际版本（避免 launcher 黄字警告）：

```xml
<DependedModules>
  <DependedModule Id="Bannerlord.Harmony" DependentVersion="v2.4.2" />
  <DependedModule Id="Bannerlord.ButterLib" DependentVersion="v2.12.0" />  <!-- 原 v2.10.2 升 -->
  <DependedModule Id="Bannerlord.UIExtenderEx" DependentVersion="v3.0.3" />  <!-- 原 v2.13.2 升 -->
  <DependedModule Id="Bannerlord.MBOptionScreen" DependentVersion="v5.12.3" />  <!-- 原 v5.11.3 升 -->
  <DependedModule Id="Native" DependentVersion="v1.4.7" />  <!-- 原 v1.3.9 升 -->
  <DependedModule Id="SandBoxCore" DependentVersion="v1.4.7" />
  <DependedModule Id="Sandbox" DependentVersion="v1.4.7" />
  <DependedModule Id="StoryMode" DependentVersion="v1.4.7" />
</DependedModules>
```

**也可改 `<Name>` 加 "(Rebuilt)" 标识** · 避免跟 workshop 版混淆：
```xml
<Name value="Player Settlement (Rebuilt)" />
```

**不改 `<Id>`** · 保 `PlayerSettlement` · 保证存档兼容。

---

## 7. Build Pipeline

```powershell
# deploy.ps1
param([string]$GameRoot = "E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord")

$repoRoot = $PSScriptRoot
$modDest = Join-Path $GameRoot "Modules\PlayerSettlement"

# Step 1: build both projects Release
dotnet build "$repoRoot\src\PlayerSettlement\PlayerSettlement.csproj" -c Release
dotnet build "$repoRoot\src\PlayerSettlementFixes\PlayerSettlementFixes.csproj" -c Release

# Step 2: ensure target dir exists
New-Item -ItemType Directory -Path "$modDest\bin\Win64_Shipping_Client" -Force | Out-Null
New-Item -ItemType Directory -Path "$modDest\ModuleData" -Force | Out-Null

# Step 3: copy SubModule.xml
Copy-Item "$repoRoot\SubModule.xml" "$modDest\SubModule.xml" -Force

# Step 4: copy built DLLs
Copy-Item "$repoRoot\src\PlayerSettlement\bin\Release\net472\PlayerSettlement.dll" "$modDest\bin\Win64_Shipping_Client\" -Force
Copy-Item "$repoRoot\src\PlayerSettlement\bin\Release\net472\PlayerSettlement.pdb" "$modDest\bin\Win64_Shipping_Client\" -Force
Copy-Item "$repoRoot\src\PlayerSettlementFixes\bin\Release\net472\PlayerSettlementFixes.dll" "$modDest\bin\Win64_Shipping_Client\" -Force
Copy-Item "$repoRoot\src\PlayerSettlementFixes\bin\Release\net472\PlayerSettlementFixes.pdb" "$modDest\bin\Win64_Shipping_Client\" -Force

# Step 5: copy ModuleData
Copy-Item "$repoRoot\ModuleData\*" "$modDest\ModuleData\" -Recurse -Force

# Step 6: verify (per 铁律 [[bannerlord-build-deploy]])
$psDll = "$modDest\bin\Win64_Shipping_Client\PlayerSettlement.dll"
if (Test-Path $psDll) {
    "✅ Deploy OK · PlayerSettlement.dll @ $((Get-Item $psDll).LastWriteTime)"
}
```

---

## 8. 测试清单（P6）

**Level 1 · 启动 · 必须通过**：
- [ ] launcher 显示 PlayerSettlement enabled · 无红 ⊘
- [ ] launch 游戏 · 不崩溃 · 到主菜单
- [ ] 查 `C:\ProgramData\...\logs\rgl_log_*.txt` · 无 PS 相关 error

**Level 2 · 加载存档 · 必须通过**：
- [ ] 加载老存档（10-09 之前的）· 不崩
- [ ] PS 设置界面可打开（MCM → Player Settlement）· 所有 setting 可见
- [ ] 原来创建的 PlayerSettlement 还在地图上

**Level 3 · 核心功能 · 必须通过**：
- [ ] 新建角色 · 到地图 · 右上角 MapBar 显示 "Build Settlement" 按钮
- [ ] 点按钮进入 settlement 建造流程 · UI 完整
- [ ] 完成一次 castle 建造 · settlement 出现在地图
- [ ] settlement 可访问 · 村民招募 · 守军管理 · 税收

**Level 4 · 退出 · 应该通过**：
- [ ] Exit to Main Menu · 不崩
- [ ] Exit Game · 不崩（Group Y 退出崩是 ERC2421 · 跟 PS 无关 · 跟 RTSCamera 可能有）

---

## 9. Rollback 方案

**Git branch 策略**：
1. 开工前 `git checkout -b pslr/dev` · 所有工作在此 branch
2. 每个 P 阶段完成 commit
3. 收官 merge 回 main

**Steam 回退**：
- 如果 rebuild 完全失败 · 用户重 Steam Subscribe `3720376888` → workshop 版 v7.5.0 回来 · 禁 UIExt v3 的其他 mod · 回归旧状态

**存档兼容性**：
- rebuild 保持 **100% 相同的类全名/namespace/字段名** · 存档不会 break
- rebuild 可能变的：Harmony patch id · 若有必要可用 `AttributeTargets.All` 复用相同 id

---

## 10. 🔴 决策点 · 需要你拍板

| # | 决策 | 建议 | 其他选项 |
|---|---|---|---|
| D1 | 产出几个 dll | 保 2 个（PS + PSFixes）| 合 1 个（破坏存档兼容风险）|
| D2 | 第三方 dll bundle 吗 | **全不 bundle**（根除 MCM 冲突）| 跟 workshop PS 一样 bundle（会复发问题）|
| D3 | 源码拆分策略 | 按 namespace / class 拆成多 .cs | 单 .cs 整体保留 |
| D4 | SubModule.xml Name | 改 "Player Settlement (Rebuilt)" | 保原 "Player Settlement" |
| D5 | ModuleData 同步 | 从 workshop copy 整个 ModuleData/ | 只搬核心文件 |
| D6 | git branch 工作 | 新 branch `pslr/dev` + 阶段 commit | 直接 main |
| D7 | PSFixes 处理 | 一起 rebuild · 修 3 处 yield iterator | 保留 workshop PSFixes.dll · 只 rebuild PS |

**我都已经填了推荐值** · 你只需确认或 override。

---

## 11. 时间预估

**完整路径**（P3 → P4 → P5 → P6）：

| Phase | 工作 | 时间 |
|---|---|---|
| P3-1 | 搭 PSLR csproj + reference 清单 | 30 min |
| P3-2 | 拆源码 reference → src/（按 namespace） | 1.5 h |
| P3-3 | 修 3 处 yield iterator + 1 处 IL mismatch | 1.5 h |
| P4-1 | 首次 build + 消 syntax/reference 错误（2-5 轮） | 1.5 h |
| P4-2 | 再 build + consistency check + 写 deploy.ps1 | 30 min |
| P5 | Steam 退订 + 本地 Modules 固化 | 10 min |
| P6 | 实测 L1-L4 + 归档 journal/REPLICATE | 1 h |

**总计**：**6-7 h · 1 session 内可完成**（大幅低于原估 2-3 session）

---

## 12. 开工前最后一问

你有没有想要**顺便加的功能**？（例如 Thaddian 文化专属 settlement 规则 · 允许建更多 settlement · 自定义 settlement 命名规则）

如果有 · 现在列出来 · P3-P4 可以一起实施 · 省得将来重复搭 build pipeline。
