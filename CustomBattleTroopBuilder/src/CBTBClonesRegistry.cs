using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace CustomBattleTroopBuilder
{
    /// <summary>
    /// Static blueprint registry · survives Bannerlord Game re-init.
    ///
    /// 2026-10-13 log 证据：Back-to-main-menu triggers OnBeforeInitialModuleScreenSetAsRoot
    /// → Bannerlord 重 init Game instance → MBObjectManager pool 清空 non-vanilla objects
    /// → CBTB clone 消失 (pool size 2223 → 2222 · snapshot 跟 clone 前一样)
    ///
    /// Fix: 保存 clone blueprint (new StringId + Name + source StringId · 不保存 instance ref)
    /// Restore(): 每次 OnBeforeInitialModuleScreenSetAsRoot 后遍历 registry · 从 source 重 clone
    ///
    /// Registry 本身是 static · 跨 Game re-init 存在 (只有 Bannerlord process 退出才丢)
    /// Phase 6 做 JSON save · 存 blueprint 到硬盘 · 跨 process 持久
    /// </summary>
    public static class CBTBClonesRegistry
    {
        public class CloneBlueprint
        {
            public string NewStringId = "";
            public string NewName = "";
            public string SourceStringId = "";
        }

        private static readonly List<CloneBlueprint> _blueprints = new List<CloneBlueprint>();

        public static int Count => _blueprints.Count;

        public static void Register(string newStringId, string newName, string sourceStringId)
        {
            if (string.IsNullOrWhiteSpace(newStringId) || string.IsNullOrWhiteSpace(sourceStringId)) return;
            // Dedup by new StringId
            for (int i = _blueprints.Count - 1; i >= 0; i--)
            {
                if (_blueprints[i].NewStringId == newStringId) _blueprints.RemoveAt(i);
            }
            _blueprints.Add(new CloneBlueprint
            {
                NewStringId = newStringId,
                NewName = newName ?? newStringId,
                SourceStringId = sourceStringId
            });
            CBTBSubModule.Log($"[CBTB.Registry] Register blueprint · {newStringId} (source={sourceStringId}) · total={_blueprints.Count}");
        }

        /// <summary>
        /// Called from OnBeforeInitialModuleScreenSetAsRoot (after Game re-init when pool is reset).
        /// Iterates all blueprints · re-creates missing clones from source troops.
        /// Idempotent · safe to call every lifecycle fire.
        /// </summary>
        public static void Restore()
        {
            if (_blueprints.Count == 0) return;
            if (MBObjectManager.Instance == null)
            {
                CBTBSubModule.Log("[CBTB.Registry] Restore skipped · MBObjectManager.Instance null");
                return;
            }

            int restored = 0, alreadyPresent = 0, failedSourceMissing = 0, failedCreate = 0;

            foreach (var bp in _blueprints)
            {
                try
                {
                    var existing = MBObjectManager.Instance.GetObject<BasicCharacterObject>(bp.NewStringId);
                    if (existing != null)
                    {
                        alreadyPresent++;
                        continue;
                    }

                    var source = MBObjectManager.Instance.GetObject<BasicCharacterObject>(bp.SourceStringId);
                    if (source == null)
                    {
                        failedSourceMissing++;
                        CBTBSubModule.Log($"[CBTB.Registry] Restore '{bp.NewStringId}' failed · source '{bp.SourceStringId}' not in pool");
                        continue;
                    }

                    var newTroop = CBTBCloneHelper.CloneTroop(source, bp.NewStringId, bp.NewName);
                    if (newTroop == null)
                    {
                        failedCreate++;
                    }
                    else
                    {
                        restored++;
                    }
                }
                catch (Exception e)
                {
                    failedCreate++;
                    CBTBSubModule.Log($"[CBTB.Registry] Restore '{bp.NewStringId}' threw: {e}");
                }
            }

            CBTBSubModule.Log(
                $"[CBTB.Registry] Restore · {restored} restored · {alreadyPresent} already-present · " +
                $"{failedSourceMissing} source-missing · {failedCreate} create-failed · total blueprints={_blueprints.Count}");
        }
    }
}
