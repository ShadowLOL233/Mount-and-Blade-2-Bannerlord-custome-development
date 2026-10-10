# CheatsGuard v1.0.0

**目的**：BannerlordCheats v3.0.3.0 在 1.4.7 上一次性 apply 163 个 Harmony patch · 若其中任一 patch 抛非 `HarmonyException` 的异常（Cheats 只 catch `HarmonyException`）· 整个 patch loop 崩 · Campaign 启动直接死。CheatsGuard 拦截 Cheats 的每个 patch 尝试 · **加宽异常捕获 + 逐个 log + skip-list 支持** · 让你精确定位坏 patch · 屏蔽后其他 150+ patch 正常工作。

## 工作原理

`[HarmonyPatch(typeof(HarmonyLib.PatchClassProcessor), nameof(PatchClassProcessor.Patch))]`

- **Prefix**：读取 `PatchClassProcessor.containerType` 私有字段 · 若命名空间以 `BannerlordCheats.Patches` 开头 → log "TRY X" · 若 X 在 skip list 内 → log "SKIP X" 并短路
- **Finalizer**：catch **所有** `Exception`（Cheats 只 catch `HarmonyException`）· log "FAIL X: ExceptionType" · **吞掉异常**保 Cheats 的 foreach loop 继续
- **其他 mod 的 Harmony patch 不受影响**（namespace 前缀过滤）

## 首次诊断流程

1. **启用 CheatsGuard**（LauncherData 已加 · IsSelected=true）· 保 Cheats 也启用
2. **启动游戏 · 进 Campaign · 崩 or 稳定**
3. **不管崩没崩**，读 `Modules/CheatsGuard/Logs/cheats_guard.log`：
   - **稳定**：说明所有 Cheats patch 都成功 apply 了（Finalizer 只做 log）· 你可以直接玩
   - **崩溃**：log 最后一条 "TRY X" 但没对应 "OK X" → **X 就是元凶**
   - **崩溃但 log 有 "FAIL X: ..."**：Finalizer 捕获了 · X 是元凶（且原因写了）
4. **加 skip · 重启**：编辑 `ModuleData/config.xml`：
   ```xml
   <SkipClasses>
     <SkipClass>X 的完整类名</SkipClass>
   </SkipClasses>
   ```
5. **重复 3-4 直到稳定**

## Log 示例

正常场景：
```
[04:31:41.123] CheatsGuard log opened (VerboseLog=True · SkipCount=0)
[04:31:41.234] CheatsGuard patch installed. Waiting for BannerlordCheats to run its patch loop at Campaign start...
[04:35:12.567] TRY     BannerlordCheats.Patches.Combat.Invincible
[04:35:12.578] OK      BannerlordCheats.Patches.Combat.Invincible
[04:35:12.589] TRY     BannerlordCheats.Patches.Combat.OneHitKill
[04:35:12.598] OK      BannerlordCheats.Patches.Combat.OneHitKill
...
```

崩溃（Finalizer 抓到）：
```
[04:35:12.567] TRY     BannerlordCheats.Patches.Combat.SomePatch
[04:35:12.598] FAIL    BannerlordCheats.Patches.Combat.SomePatch   AmbiguousMatchException: Ambiguous match found.
  inner  ...
[04:35:12.610] TRY     BannerlordCheats.Patches.Combat.NextPatch    ← 继续了 · 说明 Finalizer 救了场
```

崩溃（Native · Finalizer 也救不了）：
```
[04:35:12.567] TRY     BannerlordCheats.Patches.Combat.SomeNativeBadPatch
                    ← log 到这就停了 · 进程死 · SomeNativeBadPatch 就是元凶
```

## 已知限制

- **Native access violation** 无法被 managed try/catch 捕获 · 那种情况 Finalizer 也救不了 · 但 Prefix 的 "TRY X" log 已经精准定位到元凶
- CheatsGuard 依赖 Cheats 已装 · 若 Cheats 未启用 · CheatsGuard 无害（Prefix 检查 namespace · 不会误触其他 mod）

## Config 详解

`ModuleData/config.xml`:
```xml
<CheatsGuard>
  <VerboseLog>true</VerboseLog>  <!-- 记录每次 TRY/OK · false 只记 SKIP/FAIL -->
  <SkipClasses>
    <SkipClass>BannerlordCheats.Patches.X.Y</SkipClass>
    <SkipClass>BannerlordCheats.Patches.A.B</SkipClass>
  </SkipClasses>
</CheatsGuard>
```

## Deploy

```powershell
.\deploy.ps1
```

Launcher 里加载顺序：`Harmony → SandBoxCore/Sandbox → Cheats → CheatsGuard`（SubModule.xml 已声明 `LoadAfter Cheats`）

## 版本

- v1.0.0 (2026-09-26) · 首发 · Prefix + Finalizer + file log + skip-list
