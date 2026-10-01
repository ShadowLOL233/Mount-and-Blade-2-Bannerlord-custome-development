using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace UnifiedTroopManager.Util
{
    // Shared per-battle state used by the Harmony patches to coordinate across
    // vanilla's spawn / encounter lifecycle without re-querying the UI VM.
    //
    //   SetupActive — true once DefaultBattleMissionAgentSpawnLogic.AfterStart
    //                 has run with a non-empty RosterSelection. Diagnostic flag.
    //   PlayerSide  — cached BattleSideEnum from the spawn logic.
    //
    //   RosterSnapshot — character → how many were temporarily removed from
    //                 MobileParty.MainParty.MemberRoster so the battle sees
    //                 only the player's selected set / counts. Restored by
    //                 PlayerEncounterFinishPatch after the battle.
    //   RosterModified — latch that guards against double-apply on retry /
    //                 join-battle re-entry.
    //
    // NOTE on persistence: this state is in-memory only · MVP. If the player
    // quits the game mid-battle, the RosterSnapshot is lost and the removed
    // troops are permanently gone from the party roster. Future work can
    // serialize the snapshot in a CampaignBehaviorBase.SyncData hook.
    public static class UTMBattleState
    {
        public static bool SetupActive;
        public static BattleSideEnum PlayerSide = BattleSideEnum.None;

        public static readonly Dictionary<CharacterObject, int> RosterSnapshot
            = new Dictionary<CharacterObject, int>();
        public static bool RosterModified;

        public static void ResetAll()
        {
            SetupActive = false;
            PlayerSide = BattleSideEnum.None;
            RosterSnapshot.Clear();
            RosterModified = false;
        }
    }
}
