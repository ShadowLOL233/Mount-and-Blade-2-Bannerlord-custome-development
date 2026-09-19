# 新增编队 / 战场规模 mod 安装手续

**用途**：在现有 20 mod 组合之上，新增"严格按兵种编队（全面战争式，8 编队上限内）+ 放大战场规模"的三个 mod。

> ⚠ 本文件**只含安装说明与官方下载链接**，不分发任何第三方 mod 二进制。请从下方 Nexus 链接自行下载（尊重各 mod 作者的 permissions）。
>
> 完整机制背景与兼容性分析见 [`ModdingJournal.md`](./ModdingJournal.md) 的"战斗编队 / 部署机制"与"待下载 / 计划安装"两节。

---

## 一、待装 mod 与官方来源

| Mod | 版本 | Nexus | 依赖 | 作用 |
|---|---|---|---|---|
| **TroopClassifier** | v0.2.0 | [mods/12104](https://www.nexusmods.com/mountandblade2bannerlord/mods/12104) | Native / SandBoxCore | 兵种分类库（Formation Manager 硬前置）|
| **Stop Shuffling You Fools – Formation Manager** | v0.5.2 | [mods/11869](https://www.nexusmods.com/mountandblade2bannerlord/mods/11869) | Harmony · UIExtenderEx · MCM · **TroopClassifier** | 队伍界面把兵种钉到编队 I–VIII，持久化 + 防增援洗牌 |
| **BattleSizeResized** | v2.0.4 (for 1.4.x) | [mods/8177](https://www.nexusmods.com/mountandblade2bannerlord/mods/8177) | Harmony · ButterLib · UIExtenderEx · MCM | 改战场/攻城/海战兵力上限 + 增援波阈值 + 大战 LOD 优化 |

## 二、前置库（当前环境已装，无需额外操作）

| 库 | 需求版本 | 你的版本 | 状态 |
|---|---|---|---|
| Bannerlord.Harmony | ≥ 2.4.2 | 2.4.2.248 | ✅ |
| Bannerlord.ButterLib | — | 2.12.0 | ✅ |
| Bannerlord.UIExtenderEx | ≥ 2.13.2 | 2.13.3 | ✅ |
| Bannerlord.MBOptionScreen (MCM) | ≥ 5.11.4 | 5.12.3 | ✅ |

## 三、安装步骤

1. **完全关闭** Bannerlord 启动器（`TaleWorlds.MountAndBlade.Launcher` 会锁 mod DLL）。
2. 从上表 Nexus 链接下载三个 mod 的压缩包（TroopClassifier / Stop Shuffling / BattleSizeResized）。
3. 解压，把每个 mod 的**模块文件夹**（内含 `SubModule.xml` 的那一层）放进游戏的 Modules 目录：
   ```
   E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\
   ```
   放好后应存在：`Modules\TroopClassifier\`、`Modules\FormationManager\`、`Modules\BattleSizeResized\`
   （注意：Nexus 手动 mod 装进**游戏 Modules 目录**，不是 Workshop 的 `workshop\content\261550\`）
4. 打开启动器，在 Single Player 页勾选这三个模块。
5. 按下方"加载顺序"排序。
6. 启动游戏。

## 四、加载顺序（在现有列表基础上插入）

```
Native / SandBoxCore / Sandbox / StoryMode / CustomBattle / NavalDLC
Bannerlord.Harmony
Bannerlord.ButterLib
Bannerlord.UIExtenderEx
Bannerlord.MBOptionScreen (MCM)
TroopClassifier                ← 新增，必须在 FormationManager 之前
[你现有的 RBM / RTSCamera / ImprovedGarrisons / Retinues / CYT 等]
BattleSizeResized              ← 新增
FormationManager               ← 新增，放在 CYT / Retinues 之后
```

规则：所有库在最前；`TroopClassifier` 必须早于 `FormationManager`；`FormationManager` 放在同样改造队伍界面的 CYT / Retinues 之后再试。

## 五、首次启动排查

启动后**先看日志**（不进战役也能看）：

- `...\Configs\ModLogs\trace<日期>.txt` — Harmony/AccessTools 找不到方法的告警
- `...\Configs\ModLogs\butterlib<日期>.txt` — 托管异常

重点查 `MissingMethodException` / `TroopClassifier` 相关报错：若出现，说明 FormationManager 0.5.2（针对 TroopClassifier v0.1.1 编译）与 v0.2.0 有签名差异，退回 FormationManager 的 Nexus "Requirements" 栏拿精确 v0.1.1 替换。

## 六、验证清单（开**新存档**小规模试，别直接上主战役）

- [ ] 启动器/日志无 `TroopClassifier` MissingMethod 报错
- [ ] **队伍界面正常渲染**（与 CYT + Retinues 三家共存的最高风险点）
- [ ] 队伍界面每个兵种旁出现编队徽章（I–VIII），可点选指定
- [ ] **进战斗不崩**（与 RBM 的 MissionAgentSpawn patch 共处）
- [ ] 指定的编队生效，且**增援波不再把编队洗乱**
- [ ] BattleSizeResized 的兵力设定在 MCM 里可调；战场人数按设定放大
- [ ] 大战性能/稳定可接受（先中档兵力，配合其 LOD/增援阈值选项）

## 七、兼容性摘要

| Mod | 判定 | 要点 |
|---|---|---|
| TroopClassifier v0.2.0 | ✅ 低风险 | 纯库仅依赖 Native/SandBoxCore，零冲突面；v0.2.0 满足 FormationManager 的 v0.1.1 要求，绑定符号 `TroopClassifier`/`TroopRoleClassifier`/`Classify` 在 v0.2.0 均存在 |
| BattleSizeResized v2.0.4 | ✅ 低风险 | 全 Postfix 补丁（非破坏）；唯一注意是与 PSR/RBM/DismembermentPlus 叠加时超高兵力压性能 |
| Stop Shuffling (FormationManager) v0.5.2 | ⚠ 需实测 | 前置已齐；patch 队伍界面 UI（与 CYT/Retinues 重叠）+ MissionAgentSpawn（与 RBM 重叠）+ OrderOfBattle（与 RTSCamera 重叠），三处需新存档实测 |

详细补丁面分析见 [`ModdingJournal.md`](./ModdingJournal.md)。
