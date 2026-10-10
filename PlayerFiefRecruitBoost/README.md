# Player Fief Recruit Boost (PFRB)

**问题**：装了 PSR (PartySizeReunited) 5000 上限后，AI 军队早期即千人 · 玩家小封地 vanilla 招募速率不变 → 起步死循环（打不过大军 → 招不够兵 → 村被 raid → 更招不到）。

**解法**：非对称 buff · **只对玩家氏族拥有的 fief 生效** · 加速志愿兵日填充 + 自家 fief 无 relation 门槛。AI 招募链**完全不动**。

## 两条 patch

| Patch | 目标 | 效果 |
|---|---|---|
| A · `DefaultVolunteerModel.GetDailyVolunteerProductionProbability` postfix | 玩家氏族拥有的 town/castle + 其绑定 village | 日填充概率 × `SlotRegenMultiplier`（默认 3.0，clamp ≤1.0） |
| B · `DefaultVolunteerModel.MaximumIndexHeroCanRecruitFromHero` postfix | 玩家 recruit 自家 fief notable | 无视 relation · 强制 tier 上限 6（vanilla 需 relation 100） |

两条都用 `[HarmonyAfter("RBM", "com.rbmcombat.harmony")]` 兜底 · 但 **RBM v4.5.0 实测无 volunteer 相关 patch**（我 ilspycmd 扫过 6 个 RBM dll · 0 命中）· 无冲突面。

## 兼容性

**已核对无冲突**（2026-09-26 反编译）：
- **RBM v4.5.0**：0 volunteer 相关 patch
- **ImprovedGarrisons v4.2.0.7**：0 volunteer 相关 patch  
- **Retinues v1.4.14.31**：`VolunteerSwapForPlayer` 是**后置**换兵种（本 mod 前置改概率/tier）· 正交
- **PlayerSettlement v7.5.0**：PS 建的村/城仍走 vanilla model · 本 mod 直接对 vanilla model patch · 通用生效

## 配置

`Modules/PlayerFiefRecruitBoost/ModuleData/config.xml`：

```xml
<PlayerFiefRecruitBoost>
  <SlotRegenMultiplier>3.0</SlotRegenMultiplier>       <!-- 1.0-20.0 · 越高招兵越快 -->
  <UnlockMaxTierInOwnFiefs>true</UnlockMaxTierInOwnFiefs>  <!-- false = 仍按 relation -->
  <IncludeBoundVillages>true</IncludeBoundVillages>    <!-- 你城堡/城镇绑定的村是否算自家 fief -->
  <LogOnLoad>true</LogOnLoad>                          <!-- 加载时消息栏打提示确认生效 -->
</PlayerFiefRecruitBoost>
```

**建议起手**：`SlotRegenMultiplier=3.0` 实测；若"起步压力"消失但仍慢 → 试 5.0；若发现自家 fief 招兵爆量抢镇上 pool → 降到 2.0。

## 部署

```powershell
.\deploy.ps1
```

首次 deploy 会把 `SubModule.xml` + `bin/Win64_Shipping_Client/PlayerFiefRecruitBoost.dll` + `ModuleData/config.xml` 拷到 `Modules/PlayerFiefRecruitBoost/`。**重跑 deploy 不覆盖你已改的 config.xml**。

首次跑游戏 · 消息栏应见：
```
[PFRB] loaded · SlotRegen×3.0 · UnlockMaxTier=True · IncludeBoundVillages=True
```

Launcher 里勾"Player Fief Recruit Boost"（建议顺序：`Harmony → 其余现有 mod → PlayerFiefRecruitBoost`，SubModule.xml 已声明 LoadAfter Retinues/PS/RBM）。

## D · 治本（不在本 mod 范围）

结合以下已装内容/建议：
- **BetterPatrols** + **IG `NPCSpawnGuards`**（已启用 · 抵消 CP2 v4.0.2 加载失败的 Bug #6）→ 减少村被 raid 频率
- **Retinues** 里设计低装备成本 T2/T3 → 新兵快速转量
- **ChooseYourTroops** 挑精英上阵 · 剩下的做 garrison

## 版本

- v1.0.0 (2026-09-26) · 首发 · 2 patch · XML config · 无 MCM（后续 v1.1 视需求加）
