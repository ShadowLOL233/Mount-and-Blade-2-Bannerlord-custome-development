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
    // v1.0 · Quota enforcer wrapper around CYT's whitelist filter.
    //
    // CYT installs a filter on MapEventSide.AllocateTroops that is a pure
    // whitelist (returns true iff troop.StringId is in CYT._selectedTroops).
    // vanilla then iterates its _readyTroopsPriorityList in priority order
    // and lets the FIRST selected troop with enough stock fill
    // numberToAllocate.  Other selected troops get zero slots.
    //
    // Example from a real battle (log 2026-10-06 18:24):
    //   _selectedTroops = { main_hero, menavliaton, cataphract }
    //   numberToAllocate = 171
    //   priorityList order: ...cataphract×668 ... menavliaton×483 ...
    //   result: 170 cataphract + 1 hero · Menavlion gets 0
    //
    // User expectation (CYT UI shows per-troop count: 1 hero + 120 Menavlion
    // + 50 cataphract = 171): spawn exactly 120 Menavlion and 50 cataphract.
    //
    // Fix · Wrap CYT's filter so each troop's allocation is capped at the
    // Number recorded in CYT._actualTroopRoster:
    //
    //   quotas[sid] = _actualTroopRoster[sid].Number        (one-shot read)
    //   allocated[sid] = 0                                   (fresh per call)
    //   wrapped(descriptor, party):
    //     sid = party.Troops[descriptor].Troop.StringId
    //     if (quotas[sid] exists AND allocated[sid] >= quotas[sid]) return false
    //     if (!inner(descriptor, party)) return false      (preserve CYT whitelist)
    //     allocated[sid]++
    //     return true
    //
    // Fresh allocated dict per AllocateTroops call · vanilla may call multiple
    // times (initial + reinforcement waves); each wave re-reads CYT's quotas
    // which is what the user wants (each wave respects the current config).
    //
    // Guards:
    //   · Only wraps when customAllocationConditions is non-null (i.e. CYT
    //     installed its filter — defender side / simulation leaves it null)
    //   · Only wraps when CYT._actualTroopRoster is non-null and has entries
    //   · Priority Last + HarmonyAfter("choose_your_troops") so we see CYT's
    //     filter after it ran
    [HarmonyPatch(typeof(MapEventSide), "AllocateTroops")]
    internal static class QuotaEnforcerPatch
    {
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        [HarmonyAfter("choose_your_troops")]
        private static void Prefix(ref Func<UniqueTroopDescriptor, MapEventParty, bool> customAllocationConditions)
        {
            try
            {
                if (customAllocationConditions == null) return;

                var quotas = ReadCYTQuotas();
                if (quotas == null || quotas.Count == 0)
                {
                    UTMPatchLog.Info("QuotaEnforcer · skip · no quotas available");
                    return;
                }

                var allocated = new Dictionary<string, int>(quotas.Count);
                var inner = customAllocationConditions;

                customAllocationConditions = (descriptor, party) =>
                {
                    try
                    {
                        var troop = party.Troops[descriptor].Troop;
                        if (troop == null) return false;
                        string sid = troop.StringId;

                        if (quotas.TryGetValue(sid, out int cap))
                        {
                            allocated.TryGetValue(sid, out int cur);
                            if (cur >= cap) return false;
                        }

                        if (!inner(descriptor, party)) return false;

                        allocated.TryGetValue(sid, out int cur2);
                        allocated[sid] = cur2 + 1;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        UTMPatchLog.Exception("QuotaEnforcer · wrapped filter", ex);
                        return false;
                    }
                };

                UTMPatchLog.Info("QuotaEnforcer · installed · quotas=["
                    + string.Join(",", quotas.Select(kv => kv.Key + "=" + kv.Value)) + "]");
            }
            catch (Exception ex)
            {
                UTMPatchLog.Exception("QuotaEnforcer.Prefix", ex);
            }
        }

        private static Dictionary<string, int> ReadCYTQuotas()
        {
            try
            {
                var cytAsm = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "ChooseYourTroops");
                if (cytAsm == null) return null;

                var behType = cytAsm.GetType("ChooseYourTroops.ChooseYourTroopsBehavior");
                if (behType == null) return null;

                var rosterField = behType.GetField("_actualTroopRoster",
                    BindingFlags.NonPublic | BindingFlags.Static);
                var roster = rosterField?.GetValue(null);
                if (roster == null) return null;

                var getRosterM = roster.GetType().GetMethod("GetTroopRoster");
                var rosterList = getRosterM?.Invoke(roster, null) as IEnumerable;
                if (rosterList == null) return null;

                var quotas = new Dictionary<string, int>();
                foreach (var el in rosterList)
                {
                    try
                    {
                        // TroopRosterElement: Character is a public field,
                        // but Number is a property (backing field _number is private).
                        var elType = el.GetType();
                        var ch = elType.GetField("Character")?.GetValue(el) as BasicCharacterObject;
                        if (ch == null || ch.StringId == null) continue;
                        var numProp = elType.GetProperty("Number");
                        int num = (int)(numProp?.GetValue(el) ?? 0);
                        if (num > 0) quotas[ch.StringId] = num;
                    }
                    catch { }
                }
                return quotas;
            }
            catch (Exception ex)
            {
                UTMPatchLog.Exception("ReadCYTQuotas", ex);
                return null;
            }
        }
    }
}
