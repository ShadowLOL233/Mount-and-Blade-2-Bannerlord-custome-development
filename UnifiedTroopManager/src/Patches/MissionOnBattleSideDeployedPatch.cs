using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using UnifiedTroopManager.Data;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // B6-B second-pass reassign.
    //
    // MissionSpawnTroopPatch's per-spawn Postfix sets agent.Formation at the
    // moment vanilla hands back the fresh Agent — the diagnostic log confirms
    // this works (reassigned=690/839 in testing). But vanilla then runs a
    // class-based cleanup during Mission.OnBattleSideDeployed that re-groups
    // every agent by its DefaultFormationClass, which blows away the UTM
    // reassignment:
    //   - Cataphract (DefaultFormationClass=Cavalry=2) assigned by UTM to
    //     Formation 7 → vanilla pushes back to Formation 2
    //   - Infantry troops no matter their UTM slot → all pile in Formation 0
    //
    // This Postfix runs AFTER vanilla's cleanup and iterates every player-side
    // agent one more time, applying the PartyPlan definitively. Vanilla has no
    // further override step after OnBattleSideDeployed in field battles.
    //
    // Uses a local round-robin counter (not the SpawnTroop one, which is
    // already consumed per spawn) so split plans like "Legionary → I,II,III"
    // still distribute evenly here when enumerating the final agent list.
    [HarmonyPatch(typeof(Mission), "OnBattleSideDeployed")]
    internal static class MissionOnBattleSideDeployedPatch
    {
        [HarmonyPostfix]
        private static void Postfix(BattleSideEnum side)
        {
            // Re-enabled 2026-10-07 · second-pass formation reassign after
            // vanilla's class-based cleanup.
            try
            {
                var s = UTMSettings.Instance;
                if (s == null || !s.MasterEnabled || !s.ApplyFormationPlans) return;
                if (UTMBattleState.IsHideoutBattle()) return;
                if (side != UTMBattleState.PlayerSide) return;

                var plan = PartyPlanRuntime.Current;
                if (plan == null || plan.IsEmpty) return;

                var mission = Mission.Current;
                if (mission == null) return;
                var playerTeam = mission.PlayerTeam;
                if (playerTeam == null)
                {
                    UTMLog.Warn("OnBattleSideDeployed.Postfix · playerTeam null · side=" + side);
                    return;
                }

                var localCounters = new Dictionary<string, int>();
                int visited = 0, reassigned = 0, alreadyCorrect = 0,
                    skipHero = 0, skipNotInPlan = 0, skipNullTarget = 0, skipNullChar = 0;

                foreach (var agent in mission.Agents)
                {
                    if (agent == null) continue;
                    if (agent.Team != playerTeam) continue;
                    if (!agent.IsHuman) continue;
                    visited++;

                    var ch = agent.Character as CharacterObject;
                    if (ch == null) { skipNullChar++; continue; }
                    if (ch.IsHero) { skipHero++; continue; }

                    if (!plan.Formations.TryGetValue(ch.StringId, out var slots))
                    {
                        skipNotInPlan++;
                        continue;
                    }
                    if (slots == null || slots.Count == 0) continue;

                    int slot;
                    if (slots.Count == 1)
                    {
                        slot = slots[0];
                    }
                    else
                    {
                        localCounters.TryGetValue(ch.StringId, out int n);
                        slot = slots[n % slots.Count];
                        localCounters[ch.StringId] = n + 1;
                    }
                    if (slot < 0 || slot > 7) continue;

                    var target = playerTeam.GetFormation((FormationClass)slot);
                    if (target == null) { skipNullTarget++; continue; }

                    if (agent.Formation == target)
                    {
                        alreadyCorrect++;
                        continue;
                    }
                    agent.Formation = target;
                    reassigned++;
                }

                UTMLog.Info("OnBattleSideDeployed.Postfix · player side · visited=" + visited
                    + " · reassigned=" + reassigned
                    + " · alreadyCorrect=" + alreadyCorrect
                    + " · skip(hero=" + skipHero
                    + ", notInPlan=" + skipNotInPlan
                    + ", nullTarget=" + skipNullTarget
                    + ", nullChar=" + skipNullChar + ")");

                // GROUND TRUTH DIAGNOSTIC · after both passes, walk every Team
                // Formation and dump its real agent composition. If this log
                // shows agents actually sitting in the UTM-planned slots,
                // reassignment worked and the player's "nothing changed"
                // perception is a UI label artifact. If it shows everyone piled
                // in slot 0, our `agent.Formation = X` writes aren't sticking.
                try
                {
                    foreach (var formation in playerTeam.FormationsIncludingEmpty)
                    {
                        if (formation == null) continue;
                        int count = formation.CountOfUnits;
                        if (count == 0)
                        {
                            UTMLog.Info("Formation " + formation.FormationIndex + " · EMPTY");
                            continue;
                        }
                        // Count agents per CharacterObject StringId
                        var dist = new Dictionary<string, int>();
                        formation.ApplyActionOnEachUnit(a =>
                        {
                            var ch = a?.Character as CharacterObject;
                            if (ch == null) return;
                            dist.TryGetValue(ch.StringId, out int c);
                            dist[ch.StringId] = c + 1;
                        });
                        var top = string.Join(", ", dist.OrderByDescending(kv => kv.Value)
                            .Take(5)
                            .Select(kv => kv.Key + "×" + kv.Value));
                        UTMLog.Info("Formation " + formation.FormationIndex + " · " + count + " units · " + top);
                    }
                }
                catch (Exception diagEx)
                {
                    UTMLog.Exception("Per-formation diagnostic", diagEx);
                }
            }
            catch (Exception ex)
            {
                UTMLog.Exception("MissionOnBattleSideDeployedPatch.Postfix", ex);
            }
        }
    }
}
