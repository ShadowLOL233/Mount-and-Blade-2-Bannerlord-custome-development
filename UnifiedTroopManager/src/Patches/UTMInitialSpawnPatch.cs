using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using UnifiedTroopManager.Data;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // CRITICAL · pairs with UTMQuotaEnforcerPatch.
    //
    // Without this patch, vanilla's _battleSideSpawnContexts[i][0].InitialSpawnNumber
    // stays at the default (BattleSize-based count, e.g. 1024).  Our filter
    // only allows ~700 agents through, so vanilla's player-side allocation
    // stays short of its target and the player-side battle init silently
    // fails — all formations end up empty and vanilla crashes trying to
    // set orders on empty formations.
    //
    // Fix (mirrored from CYT's DefaultBattleMissionAgentSpawnLogicInitPostfix):
    //   For each phase's player-side spawn context, set
    //     InitialSpawnNumber   = RosterSelection.Current total count
    //     RemainingSpawnNumber = 0  (we don't do reinforcement waves)
    //
    // Uses pure reflection · no build-time dep on vanilla internals.
    // Guards against vanilla type/field renames across patches by null-checking
    // every reflection step.
    [HarmonyPatch(typeof(DefaultBattleMissionAgentSpawnLogic), "Init")]
    internal static class UTMInitialSpawnPatch
    {
        [HarmonyPostfix]
        private static void Postfix(DefaultBattleMissionAgentSpawnLogic __instance)
        {
            try
            {
                var s = UTMSettings.Instance;
                if (s == null || !s.MasterEnabled || !s.RosterFilterEnabled) return;

                // Hideout uses vanilla's own fixed-count spawn (15 for assault)
                // which UTM's selection total would corrupt.
                if (UTMBattleState.IsHideoutBattle())
                {
                    UTMLog.Info("InitialSpawnPatch · skip · hideout battle (vanilla handles)");
                    return;
                }

                var selection = RosterSelection.Current;
                if (selection == null || selection.IsEmpty)
                {
                    UTMLog.Info("InitialSpawnPatch · skip · no selection");
                    return;
                }

                int totalSelected = 0;
                foreach (var kv in selection.Counts) totalSelected += kv.Value;
                // Hero always included even if UI shows bring=0 (quota enforcer bypasses main_hero).
                var mainHero = Hero.MainHero?.CharacterObject;
                if (mainHero != null && !selection.Counts.ContainsKey(mainHero.StringId))
                    totalSelected += 1;

                var mapEvent = MapEvent.PlayerMapEvent;
                if (mapEvent == null)
                {
                    UTMLog.Warn("InitialSpawnPatch · no PlayerMapEvent");
                    return;
                }
                var playerSide = mapEvent.PlayerSide;

                var ctxField = AccessTools.Field(typeof(DefaultBattleMissionAgentSpawnLogic), "_battleSideSpawnContexts");
                var phasesField = AccessTools.Field(typeof(DefaultBattleMissionAgentSpawnLogic), "_phases");
                if (ctxField == null || phasesField == null)
                {
                    UTMLog.Error("InitialSpawnPatch · reflection fields not found");
                    return;
                }

                var ctxArray = ctxField.GetValue(__instance) as Array;
                var phasesArray = phasesField.GetValue(__instance) as Array;
                if (phasesArray == null)
                {
                    UTMLog.Warn("InitialSpawnPatch · _phases null");
                    return;
                }

                // Vanilla caps total(player, enemy) <= _battleSize. If we only
                // shrink the player phase but leave enemy phase at the vanilla
                // (potentially huge) default, their sum may exceed the cap and
                // vanilla silently aborts player-side spawn — formations all
                // empty → crash. CYT does the same two-phase rewrite.
                // We read vanilla's _battleSize so we cap enemy at (battleSize - player).
                int battleSizeCap = int.MaxValue;
                var battleSizeField = AccessTools.Field(typeof(DefaultBattleMissionAgentSpawnLogic), "_battleSize");
                if (battleSizeField != null)
                {
                    try { battleSizeCap = (int)battleSizeField.GetValue(__instance); }
                    catch { }
                }

                // Read vanilla's actual native agent cap at runtime instead of
                // hard-coding — users may install BattleSizeResized or similar
                // mods that raise MBAPI.IMBAgent.GetMaximumNumberOfAgents()
                // past the vanilla default. Vanilla itself already caps
                // _battleSize to MaxNumberOfTroopsForMission in the ctor, so
                // battleSizeCap <= MaxNumberOfTroopsForMission <= MaxNumberOfAgentsForMission.
                // Use MaxNumberOfAgentsForMission as the hard ceiling on
                // (player + enemy) total; trim player/enemy to fit.
                int actualMaxAgents = int.MaxValue;
                try
                {
                    var maxProp = AccessTools.Property(
                        typeof(DefaultBattleMissionAgentSpawnLogic),
                        "MaxNumberOfAgentsForMission");
                    if (maxProp != null)
                        actualMaxAgents = (int)maxProp.GetValue(null);
                }
                catch (Exception maxEx)
                {
                    UTMLog.Exception("InitialSpawnPatch · MaxNumberOfAgentsForMission reflection", maxEx);
                }

                // Only trim if player selection alone would exceed the native
                // agent cap minus a small enemy minimum. BattleSizeResized at
                // 2000 lets MaxNumberOfAgents be ~4000 so most selections fit.
                if (totalSelected > actualMaxAgents - 50)
                {
                    int newTotal = Math.Max(1, actualMaxAgents - 50);
                    UTMLog.Warn("InitialSpawnPatch · playerTotal " + totalSelected
                        + " trimmed to " + newTotal
                        + " (actualMaxAgents=" + actualMaxAgents + ") · enemy gets 50 min");
                    totalSelected = newTotal;
                }

                int modifiedPhases = 0;
                for (int i = 0; i < phasesArray.Length; i++)
                {
                    var phaseObj = phasesArray.GetValue(i);
                    if (phaseObj == null) continue;
                    var phaseList = ((IEnumerable)phaseObj).Cast<object>().ToList();
                    if (phaseList.Count == 0) continue;

                    var spawnCtx = phaseList[0];
                    var initField = AccessTools.Field(spawnCtx.GetType(), "InitialSpawnNumber");
                    var remField = AccessTools.Field(spawnCtx.GetType(), "RemainingSpawnNumber");
                    if (initField == null)
                    {
                        UTMLog.Warn("InitialSpawnPatch · phase " + i + " · InitialSpawnNumber field missing");
                        continue;
                    }

                    // phase i == 0 is Defender, i == 1 is Attacker (vanilla convention)
                    bool isPlayerPhase = ((i == 0) ? (playerSide == BattleSideEnum.Defender)
                                                   : (playerSide == BattleSideEnum.Attacker));

                    int oldInit = (int)initField.GetValue(spawnCtx);
                    int newInit;
                    if (isPlayerPhase)
                    {
                        newInit = totalSelected;
                    }
                    else
                    {
                        // Cap enemy spawn so player + enemy <= battleSizeCap.
                        // Keep enemy at min(vanilla, cap - player). Never go
                        // below 1 — vanilla needs at least one enemy to start.
                        int enemyCap = Math.Max(1, battleSizeCap - totalSelected);
                        newInit = Math.Min(oldInit, enemyCap);
                    }
                    initField.SetValue(spawnCtx, newInit);
                    if (remField != null && isPlayerPhase)
                        remField.SetValue(spawnCtx, 0);
                    modifiedPhases++;
                    UTMLog.Info("InitialSpawnPatch · phase " + i
                        + " · isPlayer=" + isPlayerPhase
                        + " · InitialSpawnNumber " + oldInit + " -> " + newInit);
                }

                UTMLog.Info("InitialSpawnPatch · done · modifiedPhases=" + modifiedPhases
                    + " · playerTotal=" + totalSelected
                    + " · battleSizeCap=" + battleSizeCap);
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMInitialSpawnPatch.Postfix", ex);
            }
        }
    }
}
