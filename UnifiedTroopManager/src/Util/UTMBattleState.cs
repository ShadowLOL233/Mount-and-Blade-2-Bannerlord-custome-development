using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace UnifiedTroopManager.Util
{
    // Shared per-battle state used across patches to coordinate without
    // re-querying the UI VM.
    //
    //   SetupActive — true once a battle is being set up with a non-empty
    //                 UTM RosterSelection. Diagnostic / gating flag.
    //   PlayerSide  — cached player side (set in R2 by the quota enforcer).
    //
    // R1 note: RosterSnapshot / RosterModified removed with the B6-A
    // roster-filter patches that owned them. UTM no longer mutates
    // MainParty.MemberRoster.
    public static class UTMBattleState
    {
        public static bool SetupActive;
        public static BattleSideEnum PlayerSide = BattleSideEnum.None;

        public static void ResetAll()
        {
            SetupActive = false;
            PlayerSide = BattleSideEnum.None;
        }

        // Hideout battles use vanilla's own 15-troop selection UI (menu id
        // "hideout_place") which UTM doesn't register into — our stale
        // RosterSelection from the previous field battle would otherwise
        // filter out heroes the player just picked in the vanilla UI.
        // Every UTM patch that touches battle spawn should early-return here.
        public static bool IsHideoutBattle()
        {
            try
            {
                var me = MapEvent.PlayerMapEvent;
                if (me == null) return false;
                var settlement = me.MapEventSettlement;
                return settlement != null && settlement.IsHideout;
            }
            catch (Exception) { return false; }
        }
    }
}
