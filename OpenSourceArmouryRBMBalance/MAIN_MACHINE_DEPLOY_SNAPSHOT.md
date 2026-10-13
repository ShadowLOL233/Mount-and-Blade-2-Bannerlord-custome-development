# OS 系列 · 主力机部署快照（E:）

**快照时间**：2026-10-12（CBTB v0.3.2 session 结束时）
**生成设备**：主力机（Bannerlord 装 E:\SteamLibrary · 游戏 v1.4.7 build 117484）
**用途**：第二设备（D:）开发时参照主力机实际 game modules 下的真实 state · 避免 "仓库源有新 v1.3.0 code · 但主力机还是旧 v1.2.0 deploy" 的 disconnect

---

## 1 · OS 系列 Workshop 本体（E:\SteamLibrary\steamapps\workshop\content\261550\）

| Workshop ID | Mod Id | Name | Version | 大小 |
|---|---|---|---:|---:|
| `3011479883` | OpenSourceArmory | Open Source Armory | **v2.0.0** | 7,940 MB |
| `3010990914` | OpenSourceSaddlery | Open Source Saddlery | **v2.0.0** | 1,071 MB |
| `3010984416` | OpenSourceWeaponry | Open Source Weaponry | **v2.0.1** | 506 MB |
| `2859251492` | RBM | (RBM) Realistic Battle Mod | **v4.5.4** | 12.55 MB |
| `3635788184` | RBM_WS | (RBM WS) RBM War Sails Submod | **v4.5.4** | 0.48 MB |

**合计**：~9.5 GB 装备 asset（主要在 OSA）。装备 asset 不在 Modules 下拷贝 · launcher 从 workshop 直接 load。

**升级策略**：workshop mod 由 Steam 自动更新 · 自研 OSABalance 的 whitelist / override 需跟新 version 对齐（新版 OSW 若新增 items · OSABalance 的 whitelist 需扩展）。

---

## 2 · OpenSourceArmouryRBMBalance 自研 mod · 主力机当前 state

**位置**：`E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\OpenSourceArmouryRBMBalance\`

| 项 | 主力机值 | 仓库源值（2026-10-13 已 pull） | 差距 |
|---|---|---|---|
| SubModule.xml Version | **v1.2.0** | **v1.3.0**（Phase 0 RBM Bridge） | 🔴 需 deploy |
| DLL（bin/Win64_Shipping_Client/） | **0 个**（纯 XML override · 无 Harmony 代码） | 待 build（csproj = `OSABalanceRBMBridge.csproj`） | 🔴 需 build+deploy |
| ModuleData/*.xml 产物 | 4 个（见下表） | 一致（主力机已 deploy） | ✅ 对齐 |

### 2.1 Override XML 产物（主力机 ModuleData/）

| 文件 | 大小 | 行数 | `<Item>` 数 | 最后修改 | 用途 |
|---|---:|---:|---:|---|---|
| `OSABalance_armor_override.xml` | 760 KB | 9,527 | 1,588 | 2026-09-23 00:04 | 第一代批量 armor override |
| `OSABalance_manual_override.xml` | 834 KB | 10,560 | **1,760** | 2026-09-29 03:20 | **OSA v2 收官** · 7 文化全 6 类别决议 |
| `OSABalance_pieces_override.xml` | 10 KB | 181 | 0 | 2026-09-20 06:41 | 装备 piece 层（预留 · 当前无实际 item） |
| `OSABalance_shield_override.xml` | 113 KB | 918 | **114** | 2026-09-25 05:24 | **OSW v2 Shield 已 deploy** |

**里程碑验证**：
- ✅ OSA v2 **已 deploy**（manual_override.xml 09-29 时间戳 · 1760 items 跟 Journal §53 一致）
- ✅ OSW v2 Shield 段 **已 deploy**（shield_override.xml 09-25 时间戳 · 114 items 跟 Journal §55 一致 · Journal 里标 "pending deploy" 的内容其实已经 deploy · 这是 memory/Journal 过期）

### 2.2 仓库源**新增**代码（主力机未 build+deploy）

Phase 0 RBM Bridge · 2026-10-13 第二设备归档：

- `src/OSABalanceRBMBridge.csproj`（2.3 KB）· .NET 项目文件
- `src/SubModule.cs`（1.5 KB）· MBSubModuleBase 入口 · Harmony register
- `src/OSAIds.cs`（2.2 KB）· OSA item ID 清单（15 件 safe subset · Phase 0.A）
- `src/WhitelistInjector.cs`（3.8 KB）· Harmony patch 把 OSA 的 item 注入 RBM 的 PBS/CFM/POM whitelist

**预期行为**（主力机 build+deploy 后）：
- 主菜单看绿字 `PBS 9->11, CFM 16->22, POM 20->27` （Phase 0 smoke test 验证 15 件 OSA item 成功注入 RBM 3 bucket）
- RBM 物理伤害模型将对这 15 件 OSA 装备生效（之前它们走 vanilla 简化模型）

---

## 3 · Launcher 启用状态（OneDrive\Documents\...\Configs\LauncherData.xml）

| Mod Id | IsSelected |
|---|---|
| `RBM` | ✅ true |
| `RBM_WS` | ✅ true |
| `OpenSourceArmory` | ✅ true |
| `OpenSourceSaddlery` | ✅ true |
| `OpenSourceWeaponry` | ✅ true |
| `OpenSourceArmouryRBMBalance` | ✅ true |
| `RBMPlayerStaminaPoiseBuff` | ✅ true（自研 RBM stamina 补丁） |

7/7 全启用 · load order 由 LauncherData 的 ModDatas 顺序决定。

---

## 4 · 里程碑总览（跟 Journal/memory 对账）

| 项目 | Journal/Memory 记录 | 主力机实际 state | 对账结论 |
|---|---|---|---|
| OSA v2（manual_override 1760 件） | ✅ 2026-09-28 收官 | ✅ 09-29 deploy 时间戳对上 | ✅ 一致 |
| OSW v2 Shield（114 件 shield_override） | 🟡 Journal 标 "pending deploy" | ✅ 09-25 05:24 deploy 时间戳 | 🟡 **Journal/memory 过期 · 实际已 deploy** |
| OSA v3 跨文化（head/body/arm/leg 跨 7 文化统一） | 🔴 立项 scope 已明确 | 未开工 | 一致 · 待后续 session |
| OSW Shield Tier Fix（Strapped Iron T2 bug） | 🔴 次级（让位 PSLR） | 未修复 | 一致 · 待后续 session |
| OSABalance v1.3.0 Phase 0 RBM Bridge | ✅ 线上 2026-10-13 落地 | 🔴 **未 build+deploy** | 🔴 主力机需跑 smoke test |

---

## 5 · 主力机下一步 action items（按设备优先度 marker）

### 🎯 立即执行（最高优先度）

1. **OSABalance v1.3.0 smoke test**（Phase 0 RBM Bridge）：
   ```powershell
   cd 'C:\Users\situj\git\Mount-and-Blade-2-Bannerlord-custome-development\OpenSourceArmouryRBMBalance'
   .\deploy.ps1
   ```
   - deploy.ps1 应执行 build + copy DLL + copy SubModule.xml（v1.3.0）
   - 启动游戏主菜单 · 看绿字 `PBS 9->11, CFM 16->22, POM 20->27`
   - 若绿字正确 → Phase 0.A 验证通过 · 后续 Phase 0.B（15 件 → ~300 件 production scope）
   - 若没绿字或崩 · 回报到 Journal · 第二设备排查

### 后续按序

2. **OSW Shield Tier Fix**（1.5h 单 session · 修 Strapped Iron m=T2 bug）
3. **OSA v3 跨文化统一**（4-5 session · 可拆分）
4. **OSW v2 后续段** · P2-P5（blade/piece/edge/crafted · 用户当前明示延后）

---

## 6 · 快照维护策略

- **何时更新本文件**：
  - 任何 OSABalance 版本升级（v1.3.0 → v1.3.1 等）
  - OSA/OSS/OSW workshop 版本变化
  - launcher 启用状态变化
  - override XML 大改（item 数量 ±50 以上）
- **何时不动**：仓库源代码日常改动不触发快照更新 · 只有**主力机 Modules 下 deploy 后**才更新

---

## 7 · 诊断脚本（可复用）

主力机扫描 OS state 一键命令：

```powershell
# OS 相关 game modules
Get-ChildItem 'E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules' -Directory |
  Where-Object { $_.Name -match '^(OpenSource|OSA|OSS|OSW)' }

# OS 相关 workshop
Get-ChildItem 'E:\SteamLibrary\steamapps\workshop\content\261550' -Directory | ForEach-Object {
  $sub = Join-Path $_.FullName 'SubModule.xml'
  if (Test-Path $sub) {
    $x = [xml](Get-Content $sub)
    if ($x.Module.Id.value -match 'OpenSource|RBM') {
      "$($_.Name) : $($x.Module.Name.value) $($x.Module.Version.value)"
    }
  }
}

# OSABalance override XML item 数
Get-ChildItem 'E:\...\Modules\OpenSourceArmouryRBMBalance\ModuleData\*.xml' | ForEach-Object {
  "$($_.Name) : $((Select-String -Path $_.FullName -Pattern '<Item ' -SimpleMatch).Count) items"
}
```
