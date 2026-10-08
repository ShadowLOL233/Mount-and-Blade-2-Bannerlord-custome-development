using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;
using UTMPatch.Util;

namespace UTMPatch.Patches
{
    // v0.2 · Diagnostic-only Postfix on OrderOfBattleVM.Initialize.
    //
    // Runs AFTER OoBUndoFMPatch (which itself runs after FM).  Dumps the
    // post-everything state so we can see exactly where troops go missing:
    //
    //   · For every ActiveAgent on the player team: StringId,
    //     DefaultFormationClass, Formation.FormationIndex (or <null>).
    //   · For every OoB slot: FormationIndex, Classes[*].Class (joined),
    //     Formation.CountOfUnits.
    //
    // Expected normal outcome (3 troops picked via CYT, no FM plan):
    //   agents · menavlion -> Formation 0 (Infantry) · crossbowman -> Formation 1
    //   (Ranged) · elite_cataphract -> Formation 2 (Cavalry)
    //
    // Observed symptom (user 2026-10-06): only Crossbowman visible.  Two
    // possibilities we need to distinguish from the log:
    //   A · Menavlion/Cataphract agents never spawned at preview time
    //       → zero entries for them under "agents" dump
    //   B · They spawned but Formation is null or wrong slot
    //       → entries present under "agents" dump but with surprising Formation
    //   C · They spawned with correct Formation but UI hides them due to our
    //       Classes = NumberOfAllFormations undo
    //       → entries present with correct Formation, but slot's CountOfUnits
    //         == 0 and Classes all show "NumberOfAllFormations"
    //
    // Runs at priority Last (after OoBUndoFM) with HarmonyAfter("com.formationmanager"),
    // so it logs the final state after both FM and our undo.
    [HarmonyPatch]
    internal static class OoBDiagnosticPatch
    {
        private static MethodBase TargetMethod()
            => AccessTools.Method(typeof(OrderOfBattleVM), nameof(OrderOfBattleVM.Initialize));

        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last - 1)]
        [HarmonyAfter("com.formationmanager", "com.situjingzhou.utmpatch")]
        private static void Postfix(OrderOfBattleVM __instance)
        {
            try
            {
                UTMPatchLog.Info("=== OoB Diagnostic dump start ===");

                var team = Mission.Current?.PlayerTeam;
                if (team == null)
                {
                    UTMPatchLog.Warn("Diagnostic · PlayerTeam null");
                    return;
                }

                // === Agent dump · grouped by StringId with count ===
                var groups = new Dictionary<string, (int count, string defCls, HashSet<string> formations)>();
                int totalAgents = 0;
                foreach (var agent in team.ActiveAgents)
                {
                    if (agent == null) continue;
                    var ch = agent.Character;
                    if (ch == null) continue;
                    totalAgents++;
                    string sid = ch.StringId ?? "<null>";
                    string defCls = ch.DefaultFormationClass.ToString();
                    string formIdx = agent.Formation == null
                        ? "<null>"
                        : ((int)agent.Formation.FormationIndex).ToString();

                    if (!groups.TryGetValue(sid, out var g))
                        g = (0, defCls, new HashSet<string>());
                    g.count++;
                    g.formations.Add(formIdx);
                    groups[sid] = g;
                }

                UTMPatchLog.Info("Agents · total=" + totalAgents + " · uniqueTroops=" + groups.Count);
                foreach (var kv in groups.OrderBy(x => x.Key))
                {
                    UTMPatchLog.Info("  " + kv.Key
                        + " × " + kv.Value.count
                        + " · defCls=" + kv.Value.defCls
                        + " · formations=[" + string.Join(",", kv.Value.formations) + "]");
                }

                // === Slot dump ===
                var slots = (__instance.FormationsFirstHalf
                                ?? Enumerable.Empty<OrderOfBattleFormationItemVM>())
                    .Concat(__instance.FormationsSecondHalf
                                ?? Enumerable.Empty<OrderOfBattleFormationItemVM>())
                    .Where(f => f != null)
                    .ToList();

                UTMPatchLog.Info("Slots · total=" + slots.Count);
                foreach (var slot in slots)
                {
                    try
                    {
                        var f = slot.Formation;
                        int fIdx = f == null ? -1 : (int)f.FormationIndex;
                        int countOfUnits = f == null ? -1 : f.CountOfUnits;
                        var classList = slot.Classes == null
                            ? "<null>"
                            : string.Join(",", slot.Classes.Select(c => c == null ? "<null>" : c.Class.ToString()));
                        int lockedCount = slot.Classes == null
                            ? 0
                            : slot.Classes.Count(c => c != null && c.IsLocked);
                        UTMPatchLog.Info("  slot FormationIndex=" + fIdx
                            + " · CountOfUnits=" + countOfUnits
                            + " · Classes=[" + classList + "]"
                            + " · lockedClasses=" + lockedCount);
                    }
                    catch (Exception perSlot)
                    {
                        UTMPatchLog.Exception("Diagnostic · per-slot", perSlot);
                    }
                }

                UTMPatchLog.Info("=== OoB Diagnostic dump end ===");
            }
            catch (Exception ex)
            {
                UTMPatchLog.Exception("OoBDiagnosticPatch.Postfix", ex);
            }
        }
    }
}
