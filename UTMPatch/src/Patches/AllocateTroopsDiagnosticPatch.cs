using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;
using UTMPatch.Util;

namespace UTMPatch.Patches
{
    // v0.3 · Diagnostic-only Prefix+Postfix on MapEventSide.AllocateTroops.
    //
    // Dumps the ground truth that lets us distinguish between three
    // hypotheses for why only Crossbowman spawns:
    //
    //   A · CYT _actualTroopRoster really only contains Crossbowman
    //       (user UI showed three types but saved state only has one)
    //   B · _actualTroopRoster has three types but _selectedTroops is a
    //       stale cache from an earlier battle (CYT only builds on null)
    //   C · _actualTroopRoster + _selectedTroops both correct but
    //       vanilla _readyTroopsPriorityList iteration order lets one
    //       troop fill numberToAllocate before others get a chance
    //
    // Everything via reflection so UTMPatch doesn't take a build-time
    // dependency on CYT.dll.  Runs at Priority.Last with HarmonyAfter so
    // we observe the state AFTER CYT's Prefix has installed its filter.
    [HarmonyPatch(typeof(MapEventSide), "AllocateTroops")]
    internal static class AllocateTroopsDiagnosticPatch
    {
        private static int _callCounter;

        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        [HarmonyAfter("choose_your_troops")]
        private static void Prefix(MapEventSide __instance,
                                   int numberToAllocate,
                                   ref Func<UniqueTroopDescriptor, MapEventParty, bool> customAllocationConditions)
        {
            try
            {
                int callNum = ++_callCounter;
                UTMPatchLog.Info("--- AllocateTroops#" + callNum + " Prefix (post-CYT) ---");
                UTMPatchLog.Info("  numberToAllocate=" + numberToAllocate
                    + " · hasFilter=" + (customAllocationConditions != null)
                    + " · MissionSide=" + __instance.MissionSide);

                DumpCYTState("  CYT");
                DumpReadyTroopsPriorityList(__instance, "  priorityList", limit: 50);
            }
            catch (Exception ex) { UTMPatchLog.Exception("AllocateTroopsDiag.Prefix", ex); }
        }

        [HarmonyPostfix]
        private static void Postfix(MapEventSide __instance,
                                    List<UniqueTroopDescriptor> troopsList,
                                    int numberToAllocate)
        {
            try
            {
                int allocCount = troopsList?.Count ?? -1;
                UTMPatchLog.Info("  AllocateTroops#" + _callCounter + " Postfix · allocated=" + allocCount);

                if (troopsList == null) return;
                var counts = new Dictionary<string, int>();
                foreach (var desc in troopsList)
                {
                    try
                    {
                        var ch = __instance.GetAllocatedTroop(desc);
                        string sid = ch?.StringId ?? "<null>";
                        counts[sid] = counts.TryGetValue(sid, out var c) ? c + 1 : 1;
                    }
                    catch { }
                }
                foreach (var kv in counts.OrderByDescending(x => x.Value))
                    UTMPatchLog.Info("    " + kv.Key + " × " + kv.Value);
            }
            catch (Exception ex) { UTMPatchLog.Exception("AllocateTroopsDiag.Postfix", ex); }
        }

        private static void DumpCYTState(string prefix)
        {
            try
            {
                var cytAsm = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "ChooseYourTroops");
                if (cytAsm == null) { UTMPatchLog.Info(prefix + " · assembly not loaded"); return; }

                var behType = cytAsm.GetType("ChooseYourTroops.ChooseYourTroopsBehavior");
                if (behType == null) { UTMPatchLog.Info(prefix + " · behavior type not found"); return; }

                bool setupSide = (bool)(GetStatic(behType, "_setupSide") ?? false);
                bool haveChosen = (bool)(GetStatic(behType, "_haveChosenTroops") ?? false);
                int initSpawn = (int)(GetStatic(behType, "_initialPlayerSpawn") ?? 0);
                UTMPatchLog.Info(prefix + " · setupSide=" + setupSide
                    + " · haveChosenTroops=" + haveChosen
                    + " · initialPlayerSpawn=" + initSpawn);

                var rosterObj = GetStatic(behType, "_actualTroopRoster");
                if (rosterObj == null)
                {
                    UTMPatchLog.Info(prefix + " · _actualTroopRoster=null");
                }
                else
                {
                    var totalProp = rosterObj.GetType().GetProperty("TotalManCount");
                    int total = totalProp == null ? -1 : (int)totalProp.GetValue(rosterObj);
                    UTMPatchLog.Info(prefix + " · _actualTroopRoster TotalMan=" + total);

                    var getRosterM = rosterObj.GetType().GetMethod("GetTroopRoster");
                    var rosterList = getRosterM?.Invoke(rosterObj, null) as IEnumerable;
                    if (rosterList != null)
                    {
                        foreach (var el in rosterList)
                        {
                            try
                            {
                                // TroopRosterElement: Character is a public field,
                                // but Number/WoundedNumber are properties (backing fields are private).
                                var elType = el.GetType();
                                var ch = elType.GetField("Character")?.GetValue(el) as BasicCharacterObject;
                                string sid = ch?.StringId ?? "<null>";
                                int num = (int)(elType.GetProperty("Number")?.GetValue(el) ?? 0);
                                int wnd = (int)(elType.GetProperty("WoundedNumber")?.GetValue(el) ?? 0);
                                UTMPatchLog.Info("    roster: " + sid + " N=" + num + " W=" + wnd);
                            }
                            catch { }
                        }
                    }
                }

                var patchesType = behType.GetNestedType("EncounterGameMenuBehavior",
                        BindingFlags.Public | BindingFlags.NonPublic)
                    ?.GetNestedType("MapEventSidePatches",
                        BindingFlags.Public | BindingFlags.NonPublic);
                if (patchesType != null)
                {
                    var selField = patchesType.GetField("_selectedTroops",
                        BindingFlags.NonPublic | BindingFlags.Static);
                    var sel = selField?.GetValue(null) as HashSet<string>;
                    if (sel == null)
                        UTMPatchLog.Info(prefix + " · _selectedTroops=null (will rebuild on this call)");
                    else
                        UTMPatchLog.Info(prefix + " · _selectedTroops count=" + sel.Count
                            + " · [" + string.Join(",", sel) + "]");
                }
                else
                {
                    UTMPatchLog.Info(prefix + " · MapEventSidePatches nested type not found");
                }
            }
            catch (Exception ex) { UTMPatchLog.Exception(prefix, ex); }
        }

        private static object GetStatic(Type type, string name)
        {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Static);
            return f?.GetValue(null);
        }

        private static void DumpReadyTroopsPriorityList(MapEventSide side, string prefix, int limit)
        {
            try
            {
                var f = AccessTools.Field(typeof(MapEventSide), "_readyTroopsPriorityList");
                if (f == null) { UTMPatchLog.Info(prefix + " · field not found"); return; }
                var list = f.GetValue(side) as IList;
                if (list == null) { UTMPatchLog.Info(prefix + " · null"); return; }

                var counts = new Dictionary<string, int>();
                for (int i = 0; i < list.Count; i++)
                {
                    try
                    {
                        var tuple = list[i];
                        var item1 = tuple.GetType().GetField("Item1").GetValue(tuple);
                        var troop = item1.GetType().GetProperty("Troop").GetValue(item1) as BasicCharacterObject;
                        string sid = troop?.StringId ?? "<null>";
                        counts[sid] = counts.TryGetValue(sid, out var c) ? c + 1 : 1;
                    }
                    catch { }
                }
                UTMPatchLog.Info(prefix + " · total=" + list.Count + " · unique=" + counts.Count);
                foreach (var kv in counts.OrderByDescending(x => x.Value).Take(limit))
                    UTMPatchLog.Info("    priority: " + kv.Key + " × " + kv.Value);
            }
            catch (Exception ex) { UTMPatchLog.Exception(prefix, ex); }
        }
    }
}
