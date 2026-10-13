# Phase 0 侦查报告 · OSABalance_RBMBridge

**日期**: 2026-10-13
**设备**: 第二设备（D:）· 游戏在 `D:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\`
**反编译工具**: `C:\Users\situj\.dotnet\tools\ilspycmd.exe` v8.2.0.7535
**目的**: 为 Phase 0 "RBM 兼容层" 收集反射目标 · 候选 ID 清单 · 代码骨架方案

---

## 1 · RBMCombat.dll 反射目标（已验证）

反编译产物: `_scratch/RBM_decomp/RBMCombat.decompiled.cs` · 13842 行
Assembly 元信息: `.NETFramework,Version=v4.7.2` · namespace `RBMCombat` · 入口 `RBMCombatPatcher.DoPatching(ref Harmony)`

### 1.1 · 三个白名单字段的权威签名

| 字段 | 容器类 | 容器可见性 | 字段签名 | 行号 | vanilla 条数 |
|---|---|---|---|---|---|
| `PlateBelowShoulderArmorIds` | `RBMCombat.DamageRework` | **internal** | `private static readonly string[]` | 1364 | 9 |
| `ClosedFaceMailHelmetIds` | `RBMCombat.DamageRework` | **internal** | `private static readonly string[]` | 1366 | 16 |
| `PlateOverMailBodyArmorIds` | `RBMCombat.ArmorRework` | **public** | `private static readonly string[]` | 1632 | 20 |

**跟 journal 旧记录的差异**:
- Journal §3016 写的 `RBMCombat.Patches.ArmorRework` → **错**，实际 namespace 是 `RBMCombat`，无 `.Patches` 子命名空间
- `PlateBelowShoulderArmorIds` 和 `ClosedFaceMailHelmetIds` 不在 `ArmorRework`，而在 **`DamageRework`**（internal class）
- 三者都是 `private static readonly string[]` · 两个容器类一个 public 一个 internal

### 1.2 · 消费点（Harmony Patch target 不是我们的事 · 我们只改数据）

| 字段 | 消费方法 | 行号 | 用途 |
|---|---|---|---|
| `ClosedFaceMailHelmetIds` | `DamageRework.*`（face hit 分支）| 346-350 | Plate 头盔命中面部 · 在列表 → mail 保护 |
| `PlateBelowShoulderArmorIds` | `DamageRework.*`（below-shoulder 分支）| 374-380 | Plate chest 的肩下区域 · 在列表 → 保持 Plate 曲线 |
| `PlateOverMailBodyArmorIds` | `ArmorRework.*` + `getShoulderArmorMaterial` 等 | 1731/1774 | Plate chest 的肩部区域 · 在列表 → 降级 Chainmail 曲线 |

**关键观察**: 我们只需要 append 到 `string[]` · 不需要 Harmony Patch 这些方法。只要字段数组被扩展，消费端 `Array.IndexOf(..., id) >= 0` 自然命中。

### 1.3 · 反射代码骨架（路径 α · 推荐）

```csharp
using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace OSABalance.RBMBridge
{
    internal static class WhitelistInjector
    {
        private const BindingFlags F = BindingFlags.NonPublic | BindingFlags.Static;

        public static bool TryInject()
        {
            var damageRework = AccessTools.TypeByName("RBMCombat.DamageRework"); // internal → TypeByName OK
            var armorRework  = AccessTools.TypeByName("RBMCombat.ArmorRework");
            if (damageRework == null || armorRework == null) return false;

            bool ok = true;
            ok &= Append(damageRework, "PlateBelowShoulderArmorIds", OSAIds.PlateBelowShoulder);
            ok &= Append(damageRework, "ClosedFaceMailHelmetIds",    OSAIds.ClosedFaceMail);
            ok &= Append(armorRework,  "PlateOverMailBodyArmorIds",  OSAIds.PlateOverMail);
            return ok;
        }

        private static bool Append(Type t, string fieldName, string[] additions)
        {
            var fi = AccessTools.Field(t, fieldName); // AccessTools 已含 NonPublic|Static
            if (fi == null) return false;
            var cur = (string[])fi.GetValue(null);
            var merged = cur.Concat(additions).Distinct().ToArray();
            fi.SetValue(null, merged); // .NET Framework 4.7.2 允许 reflection 覆写 readonly static
            return true;
        }
    }
}
```

**readonly 覆写合法性**: Bannerlord 跑 .NET Framework 4.7.2 · reflection `SetValue` on `private static readonly` **允许**（.NET 5+ 才被 `RuntimeFeature.IsCollectible` 限制）。RBM 自己也在 `DamageRework` 用 reflection 读 private 字段 · 技术上成立。

**调用时机**: `OnSubModuleLoad` 之后 · 第一次 Mission 之前。可直接在 OSABalance 自身 `OnSubModuleLoad` 调用（因为 SubModule.xml 已声明 `RBM order="LoadBeforeThis"` · RBM 的类此时已入内存）。无需 StaminaPoiseBuff 那种延迟到 `OnMissionBehaviorInitialize` 的 workaround（那是因为 RBMAI 延迟加载 · RBMCombat 不是）。

---

## 2 · OSA 候选 ID 分类（启发式分桶 · 待人工审）

扫描源: `D:\...\workshop\content\261550\3011479883\ModuleData\OSA_*.xml` · 14 个文件 · **1648 件 Item**

### 2.1 · Item 分布全景

| Type | 件数 |
|---|---|
| HeadArmor | 843 (含 40 `headArmor` 大小写变体) |
| BodyArmor | 388 |
| Cape | 310 |
| LegArmor | 61 |
| HandArmor | 46 |

| material_type | 件数 |
|---|---|
| Plate | 1210 |
| Leather | 158 |
| Cloth | 146 |
| Chainmail | 134 |

### 2.2 · 三白名单候选桶（启发式 · 需人工复审）

| 桶 | 匹配规则 | 候选数 | 文化分布 |
|---|---|---|---|
| **PlateOverMail** (POM) | BodyArmor+Plate · id/name 含 `scale/lamellar/over_mail/scale_cuirass` | **148** | aserai 40 · empire 31 · sturgia 23 · khuzait 21 · vlandia 18 · battania 15 |
| **PlateBelowShoulder** (PBS) | BodyArmor+Plate · id/name 含 `fortified/plated/heavy_armor/raider/cuirass/full_plate/coat_of_plates` | **11** | battania 5 · vlandia 4 · empire 1 · khuzait 1 |
| **ClosedFaceMail** (CFM) | HeadArmor · id/name 含 `closed/goggled/faceguard/bascinet/visor/full_helm` | **142** | vlandia 44 · sturgia 26 · empire 26 · aserai 21 · khuzait 15 · battania 10 |
| **Plate BodyArmor unclassified** | 不含上面任何 cue 的 Plate 身甲 | **47** | — |

**产物**:
- `_scratch/OSA_phase0/all_items.json` — 全 1648 件 flat index
- `_scratch/OSA_phase0/pom_candidates.json` — POM 148 件
- `_scratch/OSA_phase0/pbs_candidates.json` — PBS 11 件
- `_scratch/OSA_phase0/cfm_candidates.json` — CFM 142 件
- `_scratch/OSA_phase0/plate_unclassified.json` — Plate 身甲未分类 47 件

### 2.3 · 分类质量警告

1. **PBS 桶偏小且有误报**: 11 件里 `AR_aserai_leather_a` 名字说 leather 但 material_type=Plate · 需核对是不是 OSA 作者错标。真正 PBS 语义是"重 plate chest 不是 over-mail"，vanilla 代表是 `plated_leather_coat` / `khuzait_heavy_armor` / `sturgian_fortified_armor` · OSA 对应可能更多 · 需要**跟 POM 桶的 148 件做反向排查**（有些被 cue 分错类）。

2. **CFM 桶 cue 偏宽**: `southern_light_helmet_a` 之类的轻盔混进来是因为 name 里有 "Helmet" · 需要用更严格的"闭面+带 mail aventail"规则（visual mesh 层面 OSA XML 没信息 · 只能靠 head_armor 数值 + 名字正则 + 人工看模型）。

3. **Plate unclassified 47 件**: 像 `AR_aserai_armor_c` / `AR_aserai_armor_d` 的 "generic plate" · 没有 scale/lamellar/full plate 命名线索。**这是 Phase 1 字段 audit 要同时解决的事**（material_type 校正）。

**结论**: 候选清单可用于 Phase 0 技术验证（能跑通反射 + append）· 但**正式 deploy 前必须过人工复审**（Phase 1 一起做）。

---

## 3 · OSABalance 项目骨架 gap 分析

### 3.1 · 当前状态（已验证）

- `OpenSourceArmouryRBMBalance/` 根 · **纯 XML pipeline**（SubModule.xml + ModuleData + src/*.ps1 generator）
- **没有** csproj / cs 文件 / `<SubModules>` 节
- SubModule.xml line 29 显式 `<SubModules />`（空）

### 3.2 · 两条集成路径

**选项 A · 扩展 OSABalance（journal §3008 两选之一 · 推荐）**

优：
- 依赖已就位（OSA + OSW + RBM 都在 DependedModules）
- 用户只需切换 1 个 mod
- 概念集中（OSABalance 本就是"OSA + RBM 适配"的事）

需做:
1. SubModule.xml 加 `<SubModules><SubModule><Name>...<DLLName>OSABalance.dll<SubModuleClassType>OSABalance.SubModule</SubModuleClassType>` 等
2. `src/OSABalance.csproj` + `src/SubModule.cs` + `src/WhitelistInjector.cs`
3. deploy.ps1 加 `bin/Win64_Shipping_Client/*.dll` copy

**选项 β · 新建独立 mod `OSABalance_RBMBridge`**

优：
- 隔离失败（反射挂了不影响已 deploy 的 XML override）
- A/B 测试容易（取消订阅/禁用一个独立 mod）

需做（跟 α 相同 + 新建一个 mod 目录 + SubModule.xml + 独立依赖图）

**推荐 α**。理由：
1. Journal §3008 已把两选任一列为可行
2. XML override 和 Harmony 反射都是"OSA 跟 RBM 对账"的一体两面
3. 独立 mod 增加维护开销 · 用户 20 mod 已满

### 3.3 · 参考模板

`RBMPlayerStaminaPoiseBuff/src/` 是现成的 "Harmony on RBM" 参考 · 关键模式:
- csproj: `TargetFramework=net472` · `LangVersion=9` · `BannerlordBin` 跟 `WorkshopRoot` 可覆写环境变量（第二设备 D: 兼容）
- **不要 Reference RBMCombat.dll** · 全走 `AccessTools.TypeByName` 字符串解析（防 launcher 静态扫描失败）
- 失败软降级 · `_patchFailed` 标志位 + `InformationManager.DisplayMessage` 提示

---

## 4 · 风险与权衡

| 风险 | 评估 | 对策 |
|---|---|---|
| **readonly 覆写在 .NET 5+ 不保证可用** | 低（Bannerlord v1.4.7 仍 .NET Fx 4.7.2）| 若未来迁 .NET 6 · 转路径 β（Harmony Prefix 整个消费方法 · 自带 ID 列表） |
| **RBM 升级重排字段 / 改名** | 中（RBM 4.5.4 → 4.5.x · 字段名已稳定 2 版本）| 反射失败走 `_patchFailed` · 游戏不崩 · 用户看到提示手动处理 |
| **启发式分桶误报/漏报** | 高（见 §2.3）| Phase 1 跟字段 audit 一起做人工复审 · 禁 Phase 0 直接 deploy 到 workshop |
| **readonly SetValue 可能 .NET 内部缓存被绕过** | 极低（`static readonly` 不走 const inline · 直接读字段）| 单元验证：注入后 `GetValue` 应返回扩展数组 |

---

## 5 · 下一步建议

**Phase 0 侦查已出清单** · 下一步可动工项（任选）:

**A. 代码骨架 session**（1 session）:
1. 扩展 `OSABalance/SubModule.xml` 加 `<SubModules>` 节
2. 新建 `src/OSABalance.csproj` + `src/SubModule.cs` + `src/WhitelistInjector.cs` + `src/OSAIds.cs`
3. OSAIds.cs 先放"安全子集"（10-20 件最明确的 ID · 做反射 smoke test）
4. 本机编译 build · 第二设备 D: `$env:BannerlordBin` 覆盖

**B. ID 清单精炼 session**（0.5-1 session）:
1. 人工过 148 POM / 11 PBS / 142 CFM 候选 · 修 cue 规则
2. 过 Plate unclassified 47 件 · 分到 POM / PBS / 删除（Plate 误标）
3. 产出 `OSAIds.cs` 完整 ID 常量数组

**C. 转入 Phase 1 字段 audit**（2-3 session · journal 原计划）:
- 跟 ID 精炼并行 · 用现有 1835 决议清单 + PowerShell 扫 OSA XML · 建"字段缺口报告"
- Phase 0 ID 和 Phase 1 字段 audit 可以并发

**推荐顺序**: A（骨架 smoke test · 验证反射管用）→ B（ID 精炼）→ C（Phase 1）→ 主力机 build + 实游戏测
