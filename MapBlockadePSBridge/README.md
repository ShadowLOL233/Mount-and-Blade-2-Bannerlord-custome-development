# MapBlockade x PlayerSettlement Bridge

自研的桥接 mod。**目标**：让 PlayerSettlement 建的自建 settlement 被 MapBlockade 识别为可封锁目标——建城完成的瞬间，把该城注入 MapBlockade 的 `BlockadeReachabilityCache` 内部数据结构，然后触发 `RebuildAll` 重算全局可达性图。

**版本**：v0.1.0（Phase 2A · 最小可用版）——**只做注入 + 日志验证，不做寻路测试**

**前置**：MapBlockade v1.2.8 + PlayerSettlement v7.5.0 + Bannerlord v1.4.7 build 117484

---

## 工作原理

Phase 1 侦查（见 `ModdingJournal.md` 相关条目）确认 MapBlockade 的 obfuscation **只碰方法体，不碰 API 表面**，所有类名/字段名/方法名可读。桥接机制：

1. **订阅**：`OnGameInitializationFinished` 里通过反射订阅 `BannerlordPlayerSettlement.Behaviours.PlayerSettlementBehaviour.SettlementBuildCompleteEvent`
2. **面片圈选**：建城完成时，取 settlement 的 `Position2D`，遍历 `ReachabilityGraph.MaxFaceIndex` 内所有面片，用 `TryGetFaceCenter` 拿坐标，欧氏距离 &lt; `BlockedRadius` 的收入 `BlockedFis`；最近的一片作为 gate
3. **注入**：反射构造 `BlockadeReachabilityCache.BlockadeCityData` POCO，塞进私有 `_cities` 列表；同步更新 `_faceToCity`、`_gateFaceToCity`、`_wallGateByFace` 三个字典
4. **重算**：反射调用内部 `RebuildAll(string trigger)`

## Phase 2A 的验收标准

启动游戏、勾选 mod、进战役、用 PlayerSettlement 建一座城，观察消息栏：

- **理想**：`[PSBridge] subscribed to SettlementBuildCompleteEvent (radius=5)` 会话开始时出现
- **建城完成**：`[PSBridge] injected player_settlement_1 @ (X.X,Y.Y) idx=N faces=M gate=F`
- **不理想**：`[PSBridge] not subscribed - <原因>` 或 `[PSBridge] inject failed for <id>: <exception>`——把消息栏截图给我，我改

**Phase 2A 不承诺 AI 真的绕行**——只承诺注入没崩。真实寻路测试留 Phase 2B。

## 目录结构

```
MapBlockadePSBridge/
├── SubModule.xml                  硬依赖 MapBlockade + PlayerSettlement
├── src/
│   ├── BridgeSubModule.cs         MBSubModuleBase 入口
│   ├── BlockadeInjector.cs        反射 + 注入逻辑
│   └── MapBlockadePSBridge.csproj
├── deploy.ps1
└── README.md
```

## 构建 & 部署

```powershell
# 关掉 launcher（会锁 DLL）
cd C:\Users\situj\git\Mount-and-Blade-2-Bannerlord-custome-development\MapBlockadePSBridge
.\deploy.ps1
```

`deploy.ps1` 会：`dotnet build -c Release` → 拷 `SubModule.xml` + DLL 到 `Modules\MapBlockadePSBridge\`。

## 启用

Bannerlord 启动器 → **Mods** 标签：
1. 确认 **MapBlockade** 和 **PlayerSettlement** 都勾选
2. 勾选 **MapBlockade x PlayerSettlement Bridge**
3. **加载顺序**：桥接 mod 必须在 MapBlockade 和 PlayerSettlement 之后（SubModule.xml 已声明 `LoadBeforeThis`，启动器一般会自动排序，但保险起见手动检查一下）

## 关键常量

`src/BlockadeInjector.cs`：

```csharp
private const float BlockedRadius = 5.0f;
```

`BlockedRadius = 5.0` campaign 地图单位。参考 vanilla 数据：`map_blockades_base.xml` 里每个 settlement 的 outline 跨度约 10-30 单位。5.0 是保守初值，Phase 2B 会加 MCM 滑块。

## 已知局限（Phase 2A 明确不做的事）

- **无城墙可视化**：MapBlockade 的 "Blockade Towers on Walls" 依赖 XML 的 `outline` 几何数据，我们没提供，所以自建城不会有装饰性围墙贴图
- **无 grace period 转发**：自建城换主时 grace 期不会启动（`UpdateCityOwnership` 事件转发留 Phase 2B）
- **无 MCM 面板**：radius 硬编码，改要重编译
- **不测寻路**：只验证注入调用不崩，AI 是否真的绕行留 Phase 2B

## 已知风险 & 尚未实测的假设

- **`RebuildAll` 语义**：只刷 top-level cache 还是连底层 `ReachabilityGraph.Build` 也重算？后者若是必须但没触发，AI 寻路看不到注入的城。**Phase 2A 就是要验证这个**——如果消息栏没崩、但你观察不到 AI 绕行，Phase 2B 就补 `ReachabilityGraph.Build` 直接调用
- **Gate face 选择**：现在挑"距 settlement 中心最近的一片"当 gate，其实 gate 应该在"blocked 集合的外边缘"。vanilla 数据里 gate 是 outline 的中点。近似做法若过于粗糙，Phase 2B 改
- **反射字段名稳定性**：MapBlockade 若更新版本改字段名（`_cities` → `_settlements` 之类），桥接立即失效；`TryResolveReflection` 里的 `missing: ...` 报错会明确告诉你哪个坏了

## 卸载

启动器取消勾选，或删 `Modules\MapBlockadePSBridge\` 目录。注入的数据只在**内存**里，重启游戏后即消失（MapBlockade 从 XML 重载）；**不改写存档**。
