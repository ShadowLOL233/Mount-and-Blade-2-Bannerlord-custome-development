using System;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // Phase 3 target · assign each spawned agent to its planned formation.
    // DESIGN §6.1 row 8 + §4.1 OnAgentSpawned pseudocode.
    // Signature note: TaleWorlds ships several SpawnTroop overloads.
    // We target the most-used one by name and let Harmony pick the best match;
    // if signature drifts across game versions the try/catch keeps the game alive.
    [HarmonyPatch]
    internal static class MissionSpawnTroopPatch
    {
        private static System.Reflection.MethodBase TargetMethod()
        {
            // Prefer the (IAgentOriginBase, bool, Formation, bool, bool, bool, int, int, bool, bool, bool, Vec3?, Vec2?, string, string, int, bool)
            // overload · resolve by name to survive minor param tweaks between patches.
            foreach (var m in typeof(Mission).GetMethods(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic))
            {
                if (m.Name == nameof(Mission.SpawnTroop)) return m;
            }
            return null;
        }

        [HarmonyPostfix]
        private static void Postfix(Agent __result)
        {
            try
            {
                var s = UTMSettings.Instance;
                if (s == null || !s.MasterEnabled || !s.ApplyFormationPlans) return;
                if (__result == null) return;
                // Phase 3 · pick planned formation via SplitAllocator and reassign.
            }
            catch (Exception ex)
            {
                UTMLog.Exception("MissionSpawnTroopPatch.Postfix", ex);
            }
        }
    }
}
