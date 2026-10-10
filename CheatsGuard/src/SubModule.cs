using System;
using System.ComponentModel;
using System.Reflection;
using CheatsGuard.CampaignBehaviors;
using CheatsGuard.MCM;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CheatsGuard
{
    public class SubModule : MBSubModuleBase
    {
        private const string HarmonyId = "CheatsGuard.Patch";
        private static bool _patched;
        private static bool _mcmBound;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            if (_patched) return;
            try
            {
                Config.Load();
                LogFile.Init();

                var harmony = new Harmony(HarmonyId);
                harmony.PatchAll(typeof(SubModule).Assembly);
                _patched = true;

                LogFile.Write($"CheatsGuard patch installed. Waiting for BannerlordCheats to run its patch loop at Campaign start...");

                InformationManager.DisplayMessage(new InformationMessage(
                    $"[CheatsGuard] loaded · SkipCount={Config.SkipSet.Count} · log at Modules/CheatsGuard/Logs/cheats_guard.log",
                    Colors.Cyan));
            }
            catch (Exception ex)
            {
                LogFile.Write($"CRITICAL: patch install failed: {ex}");
                InformationManager.DisplayMessage(new InformationMessage(
                    "[CheatsGuard] patch install FAILED: " + ex.Message, Colors.Red));
            }
        }

        // Wire MCM up once the initial module screen is about to appear — by then MCM's
        // own SubModule has registered its DI container and singletons are safe to access.
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            if (_mcmBound) return;
            _mcmBound = true;
            try
            {
                var settings = CheatsGuardMCMSettings.Instance;
                if (settings == null)
                {
                    LogFile.Write("MCM: CheatsGuardMCMSettings.Instance is null — MCM not ready. Config.xml values remain authoritative.");
                    return;
                }

                settings.PropertyChanged += OnMcmPropertyChanged;
                SyncFromMcm(settings);
                LogFile.Write("MCM: CheatsGuardMCMSettings bound · initial sync pushed to Config. RBM maintenance is player-scoped via MaintenancePlayerScopePatch.");
            }
            catch (Exception ex)
            {
                LogFile.Write($"MCM binding failed: {ex.GetType().Name}: {ex.Message}");
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            try
            {
                if (game.GameType is Campaign && gameStarterObject is CampaignGameStarter starter)
                {
                    starter.AddBehavior(new PassiveXpBehavior());
                    LogFile.Write("Campaign started · PassiveXpBehavior registered.");
                }
            }
            catch (Exception ex)
            {
                LogFile.Write($"OnGameStart failed: {ex.GetType().Name}: {ex.Message}");
            }
        }

        private static void OnMcmPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is CheatsGuardMCMSettings s) SyncFromMcm(s);
        }

        private static void SyncFromMcm(CheatsGuardMCMSettings s)
        {
            Config.EnableMoneyHotkey  = s.EnableMoneyHotkey;
            Config.ShowMoneyHotkeyTip = s.ShowMoneyHotkeyTip;
            Config.CheatsMoneySmall   = s.MoneySmall;
            Config.CheatsMoneyLarge   = s.MoneyLarge;

            Config.RBMTroopMaintenanceFraction = s.TroopMaintenanceFraction;
            Config.RBMRecruitMaintenanceDays   = s.RecruitMaintenanceDays;
            Config.RBMIndependentPurseFraction = s.IndependentPurseFraction;
            // Maintenance trio is intentionally NOT pushed to RBMConfig globals here —
            // MaintenancePlayerScopePatch does per-call swap so only the player clan sees them.

            Config.EnablePassiveDailyXp              = s.EnablePassiveDailyXp;
            Config.PassiveDailyXpAmount              = s.PassiveDailyXpAmount;
            Config.PassiveDailyXpIncludeCompanions   = s.PassiveDailyXpIncludeCompanions;

            ApplyRbmMirrorProperties(s);
        }

        // Reflect over CheatsGuardMCMSettings, find every property carrying [RbmField(name)],
        // and push its current value into the matching RBMConfig.RBMConfig static field.
        // Runs on every MCM change PropertyChanged, so numeric fields apply the next tick.
        private static void ApplyRbmMirrorProperties(CheatsGuardMCMSettings s)
        {
            if (!RBMBridge.IsAvailable) return;
            int ok = 0, miss = 0;
            var props = typeof(CheatsGuardMCMSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in props)
            {
                var attr = p.GetCustomAttribute<RbmFieldAttribute>();
                if (attr == null) continue;
                try
                {
                    if (RBMBridge.SetField(attr.Name, p.GetValue(s))) ok++;
                    else miss++;
                }
                catch (Exception ex)
                {
                    miss++;
                    LogFile.Write($"ApplyRbmMirror {p.Name} → {attr.Name} failed: {ex.GetType().Name}: {ex.Message}");
                }
            }
            LogFile.Write($"RBM mirror sync · ok={ok} · miss={miss}");
        }
    }
}
