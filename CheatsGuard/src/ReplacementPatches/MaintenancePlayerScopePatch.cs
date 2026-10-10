using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace CheatsGuard.ReplacementPatches
{
    // Restricts CheatsGuard's MCM Troop Maintenance overrides to the *player clan only*.
    //
    // Strategy: patch RBMCampaign.SpoilsPool.ChargeClanMaintenance (which receives a Clan
    // parameter). On every call:
    //   Prefix    - snapshot RBMConfig statics; if clan == Clan.PlayerClan, swap in
    //               MCM values via RBMBridge; RBM's body then runs with those overrides.
    //   Finalizer - restore RBMConfig statics from the snapshot, always, even on exception.
    //
    // Effect: player's maintenance follows the MCM sliders, every other clan keeps whatever
    // RBM itself loaded from its config.xml (or wherever RBM sourced the default).
    //
    // Target resolution is reflection-based so CheatsGuard still loads if RBM is absent.

    [HarmonyPatch]
    internal static class MaintenancePlayerScopePatch
    {
        static MethodBase TargetMethod()
        {
            try
            {
                var asm = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => { try { return a.GetName().Name == "RBMCampaign"; } catch { return false; } });
                if (asm == null)
                {
                    LogFile.Write("MaintenancePlayerScopePatch: RBMCampaign not loaded — patch inactive.");
                    return null;
                }
                var t = asm.GetType("RBMCampaign.SpoilsPool", throwOnError: false);
                if (t == null)
                {
                    LogFile.Write("MaintenancePlayerScopePatch: RBMCampaign.SpoilsPool type not found — patch inactive.");
                    return null;
                }
                var m = t.GetMethod("ChargeClanMaintenance",
                                    BindingFlags.Public | BindingFlags.Static);
                if (m == null)
                {
                    LogFile.Write("MaintenancePlayerScopePatch: ChargeClanMaintenance method not found — patch inactive.");
                    return null;
                }
                LogFile.Write($"MaintenancePlayerScopePatch: will patch {m}");
                return m;
            }
            catch (Exception ex)
            {
                LogFile.Write($"MaintenancePlayerScopePatch.TargetMethod error: {ex.GetType().Name}: {ex.Message}");
                return null;
            }
        }

        [ThreadStatic] private static float _savedFraction;
        [ThreadStatic] private static int   _savedDays;
        [ThreadStatic] private static float _savedPurse;
        [ThreadStatic] private static bool  _shouldRestore;

        [HarmonyPrefix]
        static void Prefix(Clan clan)
        {
            _shouldRestore = false;
            if (!RBMBridge.IsAvailable) return;
            if (clan == null || clan != Clan.PlayerClan) return;

            var snap = RBMBridge.ReadCurrent();
            if (snap == null) return;

            _savedFraction = snap.Value.fraction;
            _savedDays     = snap.Value.recruitDays;
            _savedPurse    = snap.Value.indepPurse;

            RBMBridge.Apply(
                Config.RBMTroopMaintenanceFraction,
                Config.RBMRecruitMaintenanceDays,
                Config.RBMIndependentPurseFraction);
            _shouldRestore = true;
        }

        [HarmonyFinalizer]
        static void Finalizer()
        {
            if (!_shouldRestore) return;
            _shouldRestore = false;
            RBMBridge.Apply(_savedFraction, _savedDays, _savedPurse);
        }
    }
}
