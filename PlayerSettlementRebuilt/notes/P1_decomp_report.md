# PSLR P1 · 反编译阶段报告

**日期**：2026-10-10
**结论**：✅ **P1 实际提前完成**（用户 2026-09-22 已反编译 · workshop binary 9/16 以来未更新 · 现有源码对应当前 binary）

---

## 反编译源来源

| 文件 | 原始位置 | 大小 | 行数 | 类数 | 完整度 |
|---|---|---:|---:|---:|---|
| `PlayerSettlement.decompiled.cs` | `_investigation/PS/` | 545,102 B | 13,006 | 85 class / 2 interface | ✅ **完整无缺陷** |
| `PlayerSettlementFixes.decompiled.cs` | `_investigation/PSFixes/` | 99,724 B | ~2,800 | 43 class | ⚠ **3 处 yield iterator 反编译失败** · 需 P3 手工重建 |

---

## 对应的 binary

| binary | 大小 | 时间戳 |
|---|---:|---|
| `workshop\...\3720376888\bin\Win64_Shipping_Client\PlayerSettlement.dll` | 283,648 B | 2026-09-16 13:21:29 |
| `workshop\...\3720376888\bin\Win64_Shipping_Client\PlayerSettlementFixes.dll` | 64,512 B | 2026-09-16 13:21:30 |

反编译 2026-09-22 执行 · 9/22 之后 workshop 没动过 · 源码 **100% 对应当前 binary**。

---

## 识别到的关键依赖命名空间（PS 的 using 清单摘录）

**UIExtenderEx 相关**（**P2 重点扫描对象**）：
- `Bannerlord.UIExtenderEx`
- `Bannerlord.UIExtenderEx.Attributes`
- `Bannerlord.UIExtenderEx.Prefabs2`
- `Bannerlord.UIExtenderEx.ViewModels`

**MCM 相关**（**P2 次重点**）：
- `MCM.Abstractions.Attributes`
- `MCM.Abstractions.Attributes.v2`
- `MCM.Abstractions.Base.Global`

**ButterLib 相关**：
- `Bannerlord.ButterLib.HotKeys`

**其他重要**：
- `HarmonyLib` (Harmony v2.4.2.248 向后兼容 PS 要求的 v2.4.2 · OK)
- `Newtonsoft.Json` (设置序列化)
- `SandBox.View.Map.*` (地图层 UI patches)
- `TaleWorlds.CampaignSystem.*` (campaign 系统深度集成)

---

## 已知反编译缺陷（P3 要手工修）

PSFixes.cs 的 3 处 yield-return 失败位置：

| 行号 | 说明 |
|---|---|
| 1059 | `//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()` |
| 3250 | 同上 |
| 3257 | 同上 |

**修法参考**：ILSpy 对复杂 iterator 的 state machine 还原不完美 · 需要:
1. 看 decompiler 生成的 `<MethodName>d__N` 伪代码
2. 推测原 iterator 逻辑（通常是 `foreach` 展开）
3. 手工重写成等价 `yield return` 代码

---

## 下一步 · P2 工作预览

**目标**：扫描 PS 源码调用的所有 UIExt / MCM API · 对比 UIExtenderEx **v3.0.3** 的 public API · 列 breaking changes 清单。

**手段**：
1. `grep` 反编译源找所有 `UIExtenderEx.*` 使用点
2. `grep` 反编译源找所有 `MCM.*` 使用点
3. 反编译当前 workshop UIExtenderEx v3.0.3 的 public API 作对比参照
4. 列 3 类 breaking：**method renamed** / **signature changed** / **removed**

---

## 文件布局（PSLR 项目当前）

```
PlayerSettlementRebuilt/
├── decompiled_reference/
│   ├── PlayerSettlement.decompiled.cs       (545 KB · 不改 · 作参照)
│   └── PlayerSettlementFixes.decompiled.cs  ( 99 KB · 不改 · 作参照)
├── src/                                      (P3 产出 · 当前空)
└── notes/
    └── P1_decomp_report.md                   (本文件)
```

**铁律**：`decompiled_reference/` 内容**只读** · 不编辑。P3 要改源 · 复制到 `src/` 再改。
