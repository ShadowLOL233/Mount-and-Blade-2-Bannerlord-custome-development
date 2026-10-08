using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;
using UnifiedTroopManager.Data;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Patches
{
    // R2 · UTM-native quota enforcer on MapEventSide.AllocateTroops.
    //
    // Mechanism (identical in spirit to UTMPatch.QuotaEnforcerPatch, but
    // sourced from UTM's own RosterSelection — not CYT._actualTroopRoster):
    //
    //   quotas[sid] = RosterSelection.Current.Counts[sid]   (user picked these)
    //   allocated[sid] = 0                                    (fresh per call)
    //   wrapped filter(descriptor, party):
    //     sid = party.Troops[descriptor].Troop.StringId
    //     if (sid not in quotas) return false                  (not selected)
    //     if (allocated[sid] >= quotas[sid]) return false     (cap reached)
    //     allocated[sid]++
    //     return true
    //
    // Why this REPLACES rather than wraps any existing filter:
    //   · UTM is designed to supersede CYT (user intent 2026-10-06). If CYT
    //     is still loaded during transition it also installs a filter here
    //     via its own Prefix. We overwrite that assignment because UTM owns
    //     the roster-selection domain in this architecture.
    //   · Priority Last + HarmonyAfter("choose_your_troops") makes our
    //     Prefix the last to run on this method, so our customAllocationConditions
    //     is the one vanilla sees.
    //
    // Guards:
    //   · Skip if MasterEnabled off or RosterFilterEnabled off (D6 safety)
    //   · Skip if RosterSelection.Current is null or empty (encounter without
    //     UTM UI engagement — vanilla / CYT get a free pass)
    //   · Hero slots: a hero's quota is implicitly 1 if listed in Counts;
    //     the user's UI puts hero StringId → 1 when the hero is enlisted.
    //     Hero agent spawn also goes through this filter so they must be
    //     whitelisted.
    [HarmonyPatch(typeof(MapEventSide), "AllocateTroops")]
    internal static class UTMQuotaEnforcerPatch
    {
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        [HarmonyAfter("choose_your_troops")]
        private static void Prefix(MapEventSide __instance,
                                   ref Func<UniqueTroopDescriptor, MapEventParty, bool> customAllocationConditions)
        {
            try
            {
                var s = UTMSettings.Instance;
                if (s == null || !s.MasterEnabled || !s.RosterFilterEnabled) return;

                // Hideout uses vanilla's 15-troop UI; UTM isn't engaged there
                // and must not filter spawn with stale RosterSelection.
                if (UTMBattleState.IsHideoutBattle())
                {
                    UTMLog.Info("UTMQuotaEnforcer · skip · hideout battle (vanilla handles)");
                    return;
                }

                var selection = RosterSelection.Current;
                if (selection == null || selection.IsEmpty) return;

                // Only enforce on the player side.  Vanilla calls AllocateTroops
                // once per side; non-player sides should run through their own
                // allocation untouched.
                var mapEvent = __instance?.MapEvent;
                if (mapEvent == null) return;
                var playerSide = mapEvent.PlayerSide;
                if (__instance.MissionSide != playerSide) return;

                var quotas = new Dictionary<string, int>(selection.Counts);
                var allocated = new Dictionary<string, int>(quotas.Count);
                int[] filterCallCount = { 0 };
                const int diagCap = 30;

                // Snapshot main-hero StringId for the player-hero safety
                // bypass (always allow player's own hero through, even if
                // bring=0 in the UI — vanilla battle init requires a player
                // hero on the player side).
                string mainHeroSid = Hero.MainHero?.CharacterObject?.StringId;

                customAllocationConditions = (descriptor, party) =>
                {
                    int call = ++filterCallCount[0];
                    bool diag = call <= diagCap;
                    try
                    {
                        var troop = party.Troops[descriptor].Troop as CharacterObject;
                        if (troop == null)
                        {
                            if (diag) UTMLog.Info("filter#" + call + " · <null troop> · reject");
                            return false;
                        }

                        string sid = troop.StringId;

                        // Safety bypass · only player's own hero. Other heroes
                        // (companions, army-leader lords) must go through the
                        // quotas gate — unconditionally allowing them created
                        // "army lord stuffed into player formation" crashes.
                        if (sid == mainHeroSid)
                        {
                            if (diag) UTMLog.Info("filter#" + call + " · " + sid + " · mainHero · allow");
                            return true;
                        }

                        if (!quotas.TryGetValue(sid, out int cap))
                        {
                            if (diag) UTMLog.Info("filter#" + call + " · " + sid + " · notInQuotas · reject");
                            return false;
                        }
                        allocated.TryGetValue(sid, out int cur);
                        if (cur >= cap)
                        {
                            if (diag) UTMLog.Info("filter#" + call + " · " + sid + " · capReached(" + cur + "/" + cap + ") · reject");
                            return false;
                        }
                        allocated[sid] = cur + 1;
                        if (diag) UTMLog.Info("filter#" + call + " · " + sid + " · allow (" + (cur + 1) + "/" + cap + ")");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        UTMLog.Exception("UTMQuotaEnforcer · wrapped filter #" + call, ex);
                        return false;
                    }
                };

                UTMBattleState.PlayerSide = playerSide;
                UTMBattleState.SetupActive = true;

                UTMLog.Info("UTMQuotaEnforcer · installed on " + __instance.MissionSide
                    + " · quotas=[" + string.Join(",", quotas.Select(kv => kv.Key + "=" + kv.Value)) + "]");
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMQuotaEnforcerPatch.Prefix", ex);
            }
        }
    }
}
