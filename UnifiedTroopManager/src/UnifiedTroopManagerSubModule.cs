using System;
using System.ComponentModel;
using System.Reflection;
using Bannerlord.UIExtenderEx;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using UnifiedTroopManager.Behaviors;
using UnifiedTroopManager.Settings;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager
{
    // Phase 1 scaffold entry point (DESIGN §9 Phase 1).
    // Responsibilities right now:
    //   1. Bring up the log file
    //   2. Install Harmony patches with a single guarded PatchAll (every patch method
    //      is individually try/caught per O-8 · one broken patch cannot cascade)
    //   3. Register the UIExtenderEx container so Phase 2 UI has a place to plug in
    //   4. Bind MCM once the module screen is up and mirror DebugLogging
    //   5. Add the encounter-menu behavior stub on campaign start
    public sealed class UTMSubModule : MBSubModuleBase
    {
        private const string HarmonyId = "UnifiedTroopManager.Patch";
        private const string ExtenderId = "UnifiedTroopManager";

        private static bool _patched;
        private static bool _mcmBound;
        private static Harmony _harmony;
        private static UIExtender _extender;
        private static UTMSettings _boundSettings;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            if (_patched) return;
            try
            {
                UTMLog.Init();
                UTMLog.Info("UTMSubModule.OnSubModuleLoad · installing Harmony + UIExtenderEx");

                _harmony = new Harmony(HarmonyId);
                InstallHarmonyPatchesIndividually(_harmony);
                _patched = true;

                _extender = UIExtender.Create(ExtenderId);
                _extender.Register(Assembly.GetExecutingAssembly());
                _extender.Enable();

                InformationManager.DisplayMessage(new InformationMessage(
                    "[UTM] v0.1.0 scaffold loaded · Phase 2A UI shell.",
                    Colors.Cyan));
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMSubModule.OnSubModuleLoad", ex);
                InformationManager.DisplayMessage(new InformationMessage(
                    "[UTM] load failed: " + ex.Message, Colors.Red));
            }
        }

        // Per DESIGN Open Question O-8: single-patch failure never blocks the others.
        // PatchAll would abort at the first bad signature; we iterate and swallow
        // per-type exceptions so encounter menu / UI keep working even if one target
        // method's signature drifts across game versions.
        private static void InstallHarmonyPatchesIndividually(Harmony harmony)
        {
            int ok = 0, fail = 0;
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(HarmonyPatch), inherit: false).Length == 0)
                    continue;
                try
                {
                    harmony.CreateClassProcessor(type).Patch();
                    ok++;
                    UTMLog.Info("Harmony · patched " + type.FullName);
                }
                catch (Exception ex)
                {
                    fail++;
                    UTMLog.Exception("Harmony patch failed · " + type.FullName, ex);
                }
            }
            UTMLog.Info("Harmony install summary · ok=" + ok + " fail=" + fail);
        }

        // MCM's DI is only guaranteed ready by the initial module screen · same pattern
        // CheatsGuard uses. Bind once and mirror DebugLogging to the log utility.
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            if (_mcmBound) return;
            _mcmBound = true;
            try
            {
                var settings = UTMSettings.Instance;
                if (settings == null)
                {
                    UTMLog.Warn("MCM: UTMSettings.Instance is null · defaults will be used until MCM is ready.");
                    return;
                }
                settings.PropertyChanged += OnMcmPropertyChanged;
                _boundSettings = settings;
                // Force debug on temporarily so the encounter-menu OnCondition trace
                // shows up in the log; can be turned off from MCM once diagnosed.
                UTMLog.SetDebug(true);
                UTMLog.Info("MCM: UTMSettings bound · MasterEnabled=" + settings.MasterEnabled
                    + " ShowEncounterMenuButton=" + settings.ShowEncounterMenuButton
                    + " DebugLogging=" + settings.DebugLogging);
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMSubModule.OnBeforeInitialModuleScreenSetAsRoot", ex);
            }
        }

        private static void OnMcmPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                var settings = sender as UTMSettings;
                if (settings == null) return;
                if (e.PropertyName == nameof(UTMSettings.DebugLogging))
                {
                    UTMLog.SetDebug(settings.DebugLogging);
                    UTMLog.Info("DebugLogging toggled to " + settings.DebugLogging);
                }
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMSubModule.OnMcmPropertyChanged", ex);
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            try
            {
                if (!(game.GameType is Campaign)) return;
                if (!(gameStarterObject is CampaignGameStarter starter)) return;

                // Only register the behavior. The button install happens inside the
                // behavior's OnAfterSessionLaunchedEvent handler — see UTMEncounterBehavior
                // for the rationale (OnGameStart is too early for menu-option registration).
                starter.AddBehavior(new UTMEncounterBehavior());
                UTMLog.Info("Campaign behaviors registered.");
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMSubModule.OnGameStart", ex);
            }
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            if (!(game.GameType is Campaign)) return;
            InformationManager.DisplayMessage(new InformationMessage(
                "[UTM] v0.1.0 · encounter menu button 'Manage Troops' is wired · UI + logic land in Phase 2+.",
                Colors.Green));
        }

        // Bannerlord calls this on Exit-to-desktop as well as on module hot-reload.
        // Every step wrapped so one failure never blocks the rest — otherwise the unload
        // chain across all mods stalls and vanilla's crash reporter fires.
        protected override void OnSubModuleUnloaded()
        {
            try { UTMLog.Info("UTMSubModule.OnSubModuleUnloaded · starting cleanup"); }
            catch { }

            try
            {
                if (_boundSettings != null)
                {
                    _boundSettings.PropertyChanged -= OnMcmPropertyChanged;
                    _boundSettings = null;
                }
            }
            catch (Exception ex) { try { UTMLog.Exception("Unload · unbind MCM", ex); } catch { } }

            try
            {
                if (_extender != null)
                {
                    _extender.Disable();
                    _extender = null;
                }
            }
            catch (Exception ex) { try { UTMLog.Exception("Unload · UIExtender.Disable", ex); } catch { } }

            try
            {
                if (_harmony != null)
                {
                    _harmony.UnpatchAll(HarmonyId);
                    _harmony = null;
                }
            }
            catch (Exception ex) { try { UTMLog.Exception("Unload · Harmony.UnpatchAll", ex); } catch { } }

            _patched = false;
            _mcmBound = false;

            try { UTMLog.Info("UTMSubModule.OnSubModuleUnloaded · cleanup done"); } catch { }

            try { base.OnSubModuleUnloaded(); }
            catch (Exception ex) { try { UTMLog.Exception("Unload · base.OnSubModuleUnloaded", ex); } catch { } }
        }
    }
}
