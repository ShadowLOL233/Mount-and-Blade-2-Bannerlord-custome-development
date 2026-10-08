using System;
using SandBox.View.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using UnifiedTroopManager.Data;
using UnifiedTroopManager.Patches;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.UI;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.Behaviors
{
    // Phase 1 stub · registers the "Manage Troops" option on the encounter menu.
    // Phase 2 will wire the click handler to a Gauntlet screen.
    public sealed class UTMEncounterBehavior : CampaignBehaviorBase
    {
        private const string MenuOptionId = "utm_manage_troops";

        // CYT registers only into "encounter" and "menu_siege_strategies" with the 5-arg
        // AddGameMenuOption overload. Copying that shape exactly to eliminate variables.
        // Extra menu ids and the index=0 hint from earlier attempts are dropped — they
        // likely conflicted with vanilla's own layout.
        private static readonly string[] TargetMenuIds = new[]
        {
            "encounter",
            "menu_siege_strategies",
        };

        public override void RegisterEvents()
        {
            try
            {
                CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, OnGameMenuOpened);
                // CRITICAL: menu options MUST be added via OnAfterSessionLaunchedEvent, not
                // OnGameStart. CYT does the same. AddGameMenuOption calls made during OnGameStart
                // register with the CampaignGameStarter but the UI never picks them up because the
                // menu system's option cache is built later — after session launch. 8+ diagnostic
                // rounds were spent chasing this; leave this listener wired here.
                CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, Install);
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMEncounterBehavior.RegisterEvents", ex);
            }
        }

        public override void SyncData(IDataStore dataStore) { }

        // Diagnostic tracer · records every game menu Bannerlord opens so we can confirm
        // whether the pre-battle screen the user sees is actually 'encounter' or one of
        // the branches. Cheap; runs a handful of times per session.
        private static void OnGameMenuOpened(MenuCallbackArgs args)
        {
            try
            {
                string id = args?.MenuContext?.GameMenu?.StringId ?? "<null>";
                UTMLog.Info("GameMenu opened · id=" + id);

                // R1 note: previous SAFETY NET block restored MainParty.MemberRoster
                // from UTMBattleState.RosterSnapshot — removed with the B6-A roster
                // filter architecture. UTM no longer mutates MainParty, so leaked
                // state is no longer possible from that path.
                //
                // Still reset per-battle state on any map-menu re-entry just in
                // case SetupActive got stuck on an aborted encounter.
                if (Mission.Current == null && UTMBattleState.SetupActive)
                {
                    UTMBattleState.ResetAll();
                    MissionSpawnTroopPatch.ResetRoundRobin();
                    UTMLog.Info("GameMenu · reset stale UTMBattleState · menu=" + id);
                }
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMEncounterBehavior.OnGameMenuOpened", ex);
            }
        }

        public void Install(CampaignGameStarter starter)
        {
            if (starter == null) return;
            foreach (var menuId in TargetMenuIds)
            {
                try
                {
                    // 5-arg overload exactly like CYT · no isLeave / index / isRepeatable overrides.
                    starter.AddGameMenuOption(
                        menuId,
                        MenuOptionId,
                        "Manage Troops (UTM)",
                        OnCondition,
                        OnConsequence);
                    UTMLog.Info("Menu button '" + MenuOptionId + "' installed into '" + menuId + "'.");
                }
                catch (Exception ex)
                {
                    UTMLog.Exception("UTMEncounterBehavior.Install · " + menuId, ex);
                }
            }
        }

        // Log at Info so we don't depend on the debug flag pipeline being wired.
        // The extra chatter (one line per option poll per menu open) is acceptable
        // during scaffold diagnostics and can be muted later.
        private static bool OnCondition(MenuCallbackArgs args)
        {
            try
            {
                var s = UTMSettings.Instance;
                bool enabled = s != null && s.MasterEnabled && s.ShowEncounterMenuButton;
                UTMLog.Info("OnCondition called · settingsEnabled=" + enabled + " menu=" + (args?.MenuContext?.GameMenu?.StringId ?? "?"));
                if (!enabled) return false;
                // TroopSelection · same LeaveType CYT uses for its "Select troops" red button.
                args.optionLeaveType = GameMenuOption.LeaveType.TroopSelection;
                return true;
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMEncounterBehavior.OnCondition", ex);
                return false;
            }
        }

        private static void OnConsequence(MenuCallbackArgs args)
        {
            try
            {
                // Add the view INSIDE the encounter menu context (not on top of it via
                // ScreenManager.PushScreen). PushScreen let the encounter menu keep ticking
                // underneath and produced a delayed native crash after ~15–30s.
                var ctx = args?.MenuContext?.Handler as MenuViewContext;
                if (ctx == null)
                {
                    UTMLog.Warn("OnConsequence · menu handler is not MenuViewContext · cannot host view");
                    InformationManager.DisplayMessage(new InformationMessage(
                        "[UTM] Cannot open Manage Troops here — menu context is not a Sandbox view.", Colors.Red));
                    return;
                }
                var roster = MobileParty.MainParty?.MemberRoster;
                if (roster == null)
                {
                    UTMLog.Warn("OnConsequence · MainParty.MemberRoster is null");
                    InformationManager.DisplayMessage(new InformationMessage(
                        "[UTM] Cannot open — main party roster is unavailable.", Colors.Red));
                    return;
                }
                ctx.AddMenuView<UTMTroopManagerView>(new object[] { roster, RosterSelection.Current, PartyPlanRuntime.Current });
                UTMLog.Info("Encounter menu · added UTMTroopManagerView · roster=" + roster.Count
                    + " · priorPlan=" + (PartyPlanRuntime.Current?.Formations.Count ?? 0) + " entries");
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMEncounterBehavior.OnConsequence", ex);
                InformationManager.DisplayMessage(new InformationMessage(
                    "[UTM] Manage Troops screen failed to open: " + ex.Message, Colors.Red));
            }
        }
    }
}
