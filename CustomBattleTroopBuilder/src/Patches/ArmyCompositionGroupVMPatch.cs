using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;
// 禁 `using TaleWorlds.MountAndBlade.CustomBattle;` · 遵守 [[bannerlord-cross-dll-typeof]] 铁律
// v0.4.6 ROLLBACK: v0.4.5 加 using CustomBattleTroopBuilder + inline Registry.Restore
// 导致 bootstrap crash (第 4 次 · 跟之前完全同模式 · log 卡 shader warn · CBTB OnSubModuleLoad 没触发)
// 不理解 crash 真因 · 保守回滚到 v0.4.4 的 Patch 行为 · 优先游戏能启动

namespace CustomBattleTroopBuilder.Patches
{
    // Phase 3b-2b v0.4.3: Harmony patch ArmyCompositionGroupVM..ctor · 让 CBTB clone
    // 立刻出现在 Custom Battle 文化 troop picker.
    //
    // v0.4.1 失败教训（第 3 次 bootstrap crash · 入 memory [[bannerlord-cross-dll-typeof]]）：
    //   - typeof(ArmyCompositionGroupVM) + typed param CtorPostfix(ArmyCompositionGroupVM __instance)
    //     触发 CBTB.dll 加载时 eager verify metadata · touch CustomBattle.dll
    //   - 可能 timing 问题或 assembly resolve 失败 · 抛 TypeLoadException 更早于 OnSubModuleLoad
    //   - try/catch 在 OnSubModuleLoad 无效 · 游戏 bootstrap crash
    //
    // v0.4.3 修复：
    //   - 零 typed reference (无 using TaleWorlds.MountAndBlade.CustomBattle · 无 typeof(ArmyComp...))
    //   - CtorPostfix(object __instance) · runtime 内 __instance.GetType() 拿类型
    //   - Apply() 用 AccessTools.TypeByName() lazy resolve · 不触发 eager verify
    //   - Apply() 调用延迟到 OnBeforeInitialModuleScreenSetAsRoot (CustomBattle.dll ready 后)
    //
    // 用户 UX 流程：
    //   1. Custom Battle · 进 editor · clone troop (pool N+1 · snapshot 还是 N)
    //   2. Close editor · Back · 再 Custom Battle (新 ctor · patch 跑 · snapshot = N+1)
    //   3. 展开 culture slot · 看到 clone
    public static class ArmyCompositionGroupVMPatch
    {
        private const string TargetTypeName = "TaleWorlds.MountAndBlade.CustomBattle.ArmyCompositionGroupVM";
        private const string TargetFieldName = "_allCharacterObjects";

        private static FieldInfo? _allCharObjField;
        private static PropertyInfo? _isObsoleteProp;
        private static bool _isObsoleteResolved;

        public static void Apply(Harmony harmony)
        {
            try
            {
                var vmType = AccessTools.TypeByName(TargetTypeName);
                if (vmType == null)
                {
                    CBTBSubModule.Log($"[CBTB.Patch] TypeByName returned null for {TargetTypeName} · patch inactive");
                    return;
                }

                _allCharObjField = vmType.GetField(TargetFieldName,
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (_allCharObjField == null)
                {
                    CBTBSubModule.Log($"[CBTB.Patch] field {TargetFieldName} not found on {vmType.FullName} · patch inactive");
                    return;
                }

                var ctors = vmType.GetConstructors(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (ctors == null || ctors.Length == 0)
                {
                    CBTBSubModule.Log($"[CBTB.Patch] no constructors visible on {vmType.FullName} · patch inactive");
                    return;
                }

                var postfix = typeof(ArmyCompositionGroupVMPatch)
                    .GetMethod(nameof(CtorPostfix), BindingFlags.Static | BindingFlags.Public);

                if (postfix == null)
                {
                    CBTBSubModule.Log("[CBTB.Patch] CtorPostfix MethodInfo not found · patch inactive");
                    return;
                }

                int patched = 0;
                foreach (var ctor in ctors)
                {
                    try
                    {
                        harmony.Patch(ctor, postfix: new HarmonyMethod(postfix));
                        patched++;
                    }
                    catch (Exception ex)
                    {
                        CBTBSubModule.Log($"[CBTB.Patch] skip ctor overload · {ex.Message}");
                    }
                }

                CBTBSubModule.Log($"[CBTB.Patch] {vmType.FullName} ctor patched · {patched}/{ctors.Length} ctor(s) hooked · field={_allCharObjField.Name}");
            }
            catch (Exception e)
            {
                CBTBSubModule.Log($"[CBTB.Patch] Apply failed: {e}");
            }
        }

        // CRITICAL: param 必须是 object · 不是 typed ArmyCompositionGroupVM
        // 否则 CBTB.dll metadata 里 record typed reference · 触发 eager verify crash
        public static void CtorPostfix(object __instance)
        {
            try
            {
                if (_allCharObjField == null || __instance == null) return;

                var pool = MBObjectManager.Instance?.GetObjectTypeList<BasicCharacterObject>();
                if (pool == null)
                {
                    CBTBSubModule.Log("[CBTB.Patch] CtorPostfix · MBObjectManager pool null · skip");
                    return;
                }

                int poolTotal = pool.Count;
                var fresh = new List<BasicCharacterObject>();
                foreach (var ch in pool)
                {
                    if (ch == null) continue;
                    if (ch.IsHero) continue;
                    if (!ch.IsSoldier) continue;
                    if (IsObsolete(ch)) continue;
                    fresh.Add(ch);
                }

                _allCharObjField.SetValue(__instance, fresh);

                CBTBSubModule.Log($"[CBTB.Patch] CtorPostfix · pool={poolTotal} · fresh snapshot={fresh.Count}");
            }
            catch (Exception e)
            {
                CBTBSubModule.Log($"[CBTB.Patch] CtorPostfix failed: {e}");
            }
        }

        private static bool IsObsolete(BasicCharacterObject ch)
        {
            if (!_isObsoleteResolved)
            {
                _isObsoleteProp = typeof(BasicCharacterObject).GetProperty("IsObsolete",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                _isObsoleteResolved = true;
            }

            if (_isObsoleteProp == null) return false;
            try
            {
                var v = _isObsoleteProp.GetValue(ch);
                return v is bool b && b;
            }
            catch { return false; }
        }
    }
}
