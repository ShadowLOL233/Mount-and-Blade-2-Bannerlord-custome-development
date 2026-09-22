using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BetterPatrolsBrake
{
    // Removes two BetterPatrols Harmony patches that are the identified cause
    // of strategy-map fast-forward lag:
    //
    //   1. BetterPatrols.PatrolFrequentRethinkPatch (Postfix on
    //      PatrolPartiesCampaignBehavior.HourlyTickParty) - unconditionally
    //      sets mobileParty.Ai.RethinkAtNextHourlyTick=true every hourly tick,
    //      breaking vanilla AI throttling.
    //
    //   2. BetterPatrols.PatrolResumeScoringPatch (Postfix on
    //      PatrolPartiesCampaignBehavior.AiHourlyTick) - reflectively invokes
    //      CalculatePatrollingScoreForSettlement for the patrol's home and
    //      every bound village per hourly tick.
    //
    // Both patches were identified by decompile 2026-09-22. See journal section
    // for the fast-forward-lag investigation notes.
    //
    // We keep BetterPatrols' other ~24 patches (PatrolSizeTable, QualityTable,
    // WanderRadius, ReplenishThreshold, DefendVillage, etc.) which are one-shot
    // config or event-driven and don't contribute to the per-hour hot loop.
    //
    // Dependency on BetterPatrols is SOFT (resolved via AccessTools.TypeByName)
    // so this mod is harmless when BetterPatrols is not loaded.
    //
    // Must load AFTER BetterPatrols so its PatchAll has already registered the
    // patches we're about to unpatch. SubModule.xml declares BetterPatrols as
    // a LoadBeforeThis optional dependency.
    public class SubModule : MBSubModuleBase
    {
        private const string BetterPatrolsHarmonyId = "better_patrols";
        private const string OurHarmonyId = "PSCacheWarmup.BetterPatrolsBrake";

        private static readonly string LogPath =
            Path.Combine(BasePath.Name, "Configs", "ModLogs", "BetterPatrolsBrake.log");
        private static readonly object LogLock = new object();

        private static void Log(string msg)
        {
            try
            {
                lock (LogLock)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
                    File.AppendAllText(LogPath,
                        string.Format("[{0:yyyy-MM-ddTHH:mm:ss.fff}] {1}\n",
                            DateTime.Now, msg));
                }
            }
            catch
            {
                // never break the mod for logging failure
            }
        }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Log("========== OnSubModuleLoad ==========");

            // Soft-dep check: is BetterPatrols even loaded?
            var probeType = AccessTools.TypeByName("BetterPatrols.PatrolFrequentRethinkPatch");
            if (probeType == null)
            {
                Log("BetterPatrols not detected (probe type not resolvable). Silent no-op.");
                return;
            }
            Log("BetterPatrols detected (probe type resolved: " + probeType.FullName + ").");

            var harmony = new Harmony(OurHarmonyId);

            // Target 1: PatrolPartiesCampaignBehavior.HourlyTickParty
            TryUnpatch(harmony,
                targetType: typeof(PatrolPartiesCampaignBehavior),
                methodName: "HourlyTickParty",
                targetPatchName: "PatrolFrequentRethinkPatch");

            // Target 2: PatrolPartiesCampaignBehavior.AiHourlyTick
            TryUnpatch(harmony,
                targetType: typeof(PatrolPartiesCampaignBehavior),
                methodName: "AiHourlyTick",
                targetPatchName: "PatrolResumeScoringPatch");

            InformationManager.DisplayMessage(new InformationMessage(
                "[BetterPatrolsBrake] v0.1 loaded. BetterPatrols hourly-tick hot patches removed. "
                + "Log: Configs\\ModLogs\\BetterPatrolsBrake.log"));
        }

        private static void TryUnpatch(Harmony harmony, Type targetType, string methodName, string targetPatchName)
        {
            var method = AccessTools.Method(targetType, methodName);
            if (method == null)
            {
                Log("  " + targetType.Name + "." + methodName
                    + " not found (vanilla API changed?). Skipping " + targetPatchName + ".");
                return;
            }

            // Snapshot patch state before/after so we can log whether the target patch
            // was actually there.
            var beforeInfo = Harmony.GetPatchInfo(method);
            int beforeCount = beforeInfo != null && beforeInfo.Postfixes != null
                ? beforeInfo.Postfixes.Count : 0;

            try
            {
                harmony.Unpatch(method, HarmonyPatchType.Postfix, BetterPatrolsHarmonyId);
                Log("  Unpatch call OK on " + targetType.Name + "." + methodName
                    + " Postfix by owner=" + BetterPatrolsHarmonyId
                    + " (target patch: " + targetPatchName + ")");
            }
            catch (Exception ex)
            {
                Log("  Unpatch FAILED on " + targetType.Name + "." + methodName
                    + ": " + ex.GetType().Name + " " + ex.Message);
                return;
            }

            var afterInfo = Harmony.GetPatchInfo(method);
            int afterCount = afterInfo != null && afterInfo.Postfixes != null
                ? afterInfo.Postfixes.Count : 0;
            Log("    Postfix count before=" + beforeCount + " after=" + afterCount
                + (beforeCount > afterCount
                    ? " (successfully removed " + (beforeCount - afterCount) + ")"
                    : " (no change - target patch may not have been present)"));
        }
    }
}
