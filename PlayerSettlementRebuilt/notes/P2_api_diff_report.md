# PSLR P2 · API Diff 报告

**日期**：2026-10-10
**结论**：🎉 **PS v7.5.0 的所有依赖 API 在当前 UIExt v3.0.3 + MCM v5.12.3 中完全保留** · **无 breaking changes** · PS **源码一行都不需要改**

---

## 验证方法

反编译 workshop 现版本：
- `Bannerlord.UIExtenderEx.dll v3.0.3` → 509KB · 509266 B · 16600+ 行
- `MCMv5.dll v5.12.3` → 528896 B · 26517 行

grep PS/PSFixes decompiled source · 列全 API 引用点。
grep v3.0.3 / v5.12.3 decompiled source · 验证每个 API 是否存在 + 签名一致。

---

## UIExtenderEx API 兼容性

| PS 调用 | v3.0.3 中 | 变化 |
|---|---|---|
| `UIExtender` class | ✅ L569 `public class UIExtender` | 无 |
| `UIExtender.Create(moduleName)` | ✅ L583 `public static UIExtender Create(string moduleName)` | 签名一致 |
| `.Register(Assembly)` | ✅ L664 `public void Register(Assembly assembly)` | 签名一致 |
| `.Enable()` | ✅ L738 `public void Enable()` | 签名一致 |
| `BaseViewModelMixin<T>` | ✅ L961 `public abstract class BaseViewModelMixin<TViewModel> : IViewModelMixin, IViewModelMixinNotifications where TViewModel : ViewModel` | 签名一致 |
| `[ViewModelMixin(method, bool)]` | ✅ L6230 `public sealed class ViewModelMixinAttribute : BaseUIExtenderAttribute` | 存在 |
| `[PrefabExtension(xml, xpath)]` | ✅ L6169 `public sealed class PrefabExtensionAttribute : BaseUIExtenderAttribute` | 存在 |
| `[PrefabExtensionFileName(bool)]` | ✅ L2710 `protected internal sealed class PrefabExtensionFileNameAttribute : PrefabExtensionSingleContentAttribute` | 存在 |
| `PrefabExtensionInsertPatch` | ✅ L2693 `public abstract class PrefabExtensionInsertPatch` | 存在 |
| `UIExtenderEx.Prefabs2` namespace | ✅ L2691 | 存在 · 跟 `.Prefabs` 并存 |

**结论**：PS 用的 UIExt API 面在 v3.0.3 **100% 保留**。

---

## MCM API 兼容性

| PS 调用 | v5.12.3 中 | 变化 |
|---|---|---|
| `[SettingPropertyBool(...)]` | ✅ L9017 `public sealed class SettingPropertyBoolAttribute : BaseSettingPropertyAttribute` | 存在 |
| `[SettingPropertyFloatingInteger(...)]` | ✅ L9048 `public sealed class SettingPropertyFloatingIntegerAttribute : BaseSettingPropertyAttribute` | 存在 |
| `[SettingPropertyInteger(...)]` | ✅ L9067 `public sealed class SettingPropertyIntegerAttribute : BaseSettingPropertyAttribute` | 存在 |
| `[SettingPropertyGroup(...)]` | ✅ L9000 `public sealed class SettingPropertyGroupAttribute : Attribute, IPropertyGroupDefinition` | 存在 |
| `GlobalSettings<T>.Instance` | ✅ (GlobalSettingsContainer L6083 + 通用模板) | 存在 |
| `AttributeGlobalSettings<T>` | ✅ L9780 `public abstract class AttributeGlobalSettings<T> : GlobalSettings<T>` | 存在 |

**结论**：PS 用的 MCM API 面在 v5.12.3 **100% 保留**（MCM Abstractions 层本就是长期稳定承诺）。

---

## 真因重新定位

**不是 API breaking changes** · 而是：

### CLR 程序集解析冲突

PS v7.5.0 的 `bin/Win64_Shipping_Client/` 文件夹内 **bundle 了自己的 `MCMv5.dll` v5.11.3** 作为影子副本（495,104 B · hash 80E5F3A5...）。

当 Bannerlord 加载 mod 时：
1. ButterLib 的 ImplementationLoader 枚举所有 mod 的 bin 文件夹
2. 找到 **两个 MCMv5.dll**：
   - `workshop\...\2859238197\bin\...\MCMv5.dll` v5.12.3（528,896 B · CE41B66F...）
   - `workshop\...\3720376888\bin\...\MCMv5.dll` v5.11.3（495,104 B · 80E5F3A5...）
3. CLR AssemblyLoadContext 加载两份不同版本的同程序集 → **type system 混乱**
4. PS 的代码里 `GlobalSettings<Settings>.Instance` 调用 · CLR 可能解析到 v5.11.3 的 generic bind · 但 workshop MCM 已用 v5.12.3 的 type 注册 service
5. 泛型类型不匹配 · 调非存在的 slot · **native-level crash**（无 .NET exception · 因为是 type system 层面不是代码层面）

### 为什么 10-09 还能跑 · 10-10 崩

- 10-09：你的 workshop MCM 是 **v5.11.4**（跟 bundled v5.11.3 几乎同版 · patch 级差异 · CLR 容忍）
- 10-10：你 re-subscribe 到 v5.12.3（跨 minor 版 · 5.11 → 5.12 · CLR 不再容忍 same-assembly-different-version）

---

## PSLR 项目 scope 重新评估

鉴于**无需源码 patch** · PSLR 项目的性价比重新算：

| 选项 | 工作量 | 效果 | 自维护能力 |
|---|---|---|---|
| **A · 删 bundled MCM** | 5 分钟 · 删一个 dll | ✅ 立即修 · ❌ Steam 下次 sync 恢复 | ❌ 无 |
| **B · 搬本地 + 退订 + 删 bundle** | 15 分钟 · 文件夹操作 | ✅ 永久修 · 冻结 v7.5.0 代码 | 🟡 中 · 可随时反编译再改 |
| **C · 完整 rebuild（原 PSLR 计划）** | 1-2 session · 搭 csproj / build pipeline | ✅ 永久修 · 代码源在手 | 🟢 高 · 可立刻 patch |

**C 相对 B 的净增价值**：
- 有源码树 · 可随时 git commit 自己的改动
- 未来 TaleWorlds 游戏更新破坏 PS 时 · 可立刻定位 patch
- 可添加自己想要的功能

**C 相对 B 的代价**：
- 1-2 session dev 投入
- 需要搭 .NET Framework 4.7.2 csproj + 添 TaleWorlds / UIExt / MCM / ButterLib dll 引用
- Build 可能遇到 obsolete member 警告（API 兼容但有 deprecation 标记）

---

## 下一步决策

**我建议 B（务实·永久·低工作量）** · 但**用户 2026-10-10 早上明确授权 "反编译是最为稳妥且可自行维护的方案"** · 以用户授权为准 · 继续走 C。

**P3 scope 更新**：
- **源码 patch 工作量 = 0**（无 API 变化）
- **主要工作** = 修 3 处 yield iterator 反编译缺陷 + 修 line 7839 的 IL 不匹配
- **P4 工作量** = 搭 csproj · 加 reference · build Release · 可能 1-3 轮 build 错误消除
- **P5 = 1 分钟操作**

**P3 + P4 预估**：**1 session · 2-3 小时**（比原估的 2-3 session 大幅缩短）
