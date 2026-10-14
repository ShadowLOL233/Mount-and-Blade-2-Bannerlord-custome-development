using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.CustomBattle;
using TaleWorlds.ObjectSystem;

namespace CustomBattleTroopBuilder.Patches
{
    // Phase 3b-2b fix: Custom Battle 文化 troop picker 从 ArmyCompositionGroupVM._allCharacterObjects
    // 读 troop list · 这个字段在 ctor 时一次性 snapshot 从 MBObjectManager pool
    // (过滤 IsSoldier && !IsObsolete) · 之后不再 refresh · 所以 CBTB 运行时 clone 的
    // 新 BasicCharacterObject 看不到。
    //
    // 修复：ctor Postfix 时 reflection 覆写 _allCharacterObjects 为 fresh enumerate
    // 用户必须 Close editor → Back → 再进 Custom Battle · 触发 ctor 新 snapshot
    //
    // Why 不 patch PopulateTroopTypes: 更低频 patch target · 性能更好 · 1 patch 够用
    // 若 UX 不理想（必须重开菜单）· Phase 3b-2c 再加 PopulateTroopTypes patch
    //
    // 用 explicit TargetMethod 而不是 attribute[MethodType.Constructor] · 避开
    // [[harmony-private-method]] 教训：private/ambiguous target 可能 silent fail
    public static class ArmyCompositionGroupVMPatch
    {
        private static FieldInfo? _allCharObjField;

        public static void Apply(Harmony harmony)
        {
            try
            {
                var vmType = typeof(ArmyCompositionGroupVM);
                _allCharObjField = vmType.GetField("_allCharacterObjects",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (_allCharObjField == null)
                {
                    CBTBSubModule.Log("[CBTB.Patch] _allCharacterObjects field NOT FOUND on ArmyCompositionGroupVM · Phase 3b-2b fix inactive");
                    return;
                }

                var ctors = vmType.GetConstructors(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (ctors.Length == 0)
                {
                    CBTBSubModule.Log("[CBTB.Patch] ArmyCompositionGroupVM has no constructors visible · patch inactive");
                    return;
                }

                var postfix = typeof(ArmyCompositionGroupVMPatch)
                    .GetMethod(nameof(CtorPostfix), BindingFlags.Static | BindingFlags.Public);

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
                        CBTBSubModule.Log($"[CBTB.Patch] skip ctor {ctor} · {ex.Message}");
                    }
                }

                CBTBSubModule.Log($"[CBTB.Patch] ArmyCompositionGroupVM ctor patched · {patched}/{ctors.Length} ctor(s) hooked");
            }
            catch (Exception e)
            {
                CBTBSubModule.Log($"[CBTB.Patch] Apply failed: {e}");
            }
        }

        public static void CtorPostfix(ArmyCompositionGroupVM __instance)
        {
            try
            {
                if (_allCharObjField == null || __instance == null) return;

                var pool = MBObjectManager.Instance?.GetObjectTypeList<BasicCharacterObject>();
                if (pool == null)
                {
                    CBTBSubModule.Log("[CBTB.Patch] MBObjectManager pool null · skip");
                    return;
                }

                int poolTotal = pool.Count;
                var fresh = new List<BasicCharacterObject>();
                foreach (var ch in pool)
                {
                    if (ch == null) continue;
                    if (ch.IsHero) continue;
                    if (!ch.IsSoldier) continue;
                    // Note: BasicCharacterObject.IsObsolete may not exist in v1.4.7 ·
                    // vanilla filter checked via reflection to avoid compile break
                    if (IsObsolete(ch)) continue;
                    fresh.Add(ch);
                }

                _allCharObjField.SetValue(__instance, fresh);

                CBTBSubModule.Log($"[CBTB.Patch] ArmyCompositionGroupVM ctor postfix · pool={poolTotal} · fresh snapshot={fresh.Count}");
            }
            catch (Exception e)
            {
                CBTBSubModule.Log($"[CBTB.Patch] CtorPostfix failed: {e}");
            }
        }

        private static PropertyInfo? _isObsoleteProp;
        private static bool _isObsoleteResolved;

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
