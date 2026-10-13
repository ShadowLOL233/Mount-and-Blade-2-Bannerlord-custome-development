using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace OSABalance.RBMBridge
{
    // Appends OSA item IDs to three private static readonly string[] fields in
    // RBMCombat.{DamageRework,ArmorRework}. Field signatures and consumer methods
    // verified via ilspycmd decompile 2026-10-13 (see _scratch/OSA_phase0/PHASE0_RECON.md §1).
    //
    // readonly override is legal on .NET Framework 4.7.2 (Bannerlord v1.4.7 target).
    // No Harmony patch needed — fields are plain data read via Array.IndexOf at hit time.
    internal static class WhitelistInjector
    {
        public struct Report
        {
            public bool DamageReworkFound;
            public bool ArmorReworkFound;
            public int PlateBelowShoulderBefore, PlateBelowShoulderAfter;
            public int ClosedFaceMailBefore,     ClosedFaceMailAfter;
            public int PlateOverMailBefore,      PlateOverMailAfter;
            public string Error;

            public bool Ok => DamageReworkFound && ArmorReworkFound && Error == null;

            public string Summary()
            {
                if (!Ok) return "OSABalance RBMBridge: FAILED " + (Error ?? "type not resolved");
                return string.Format(
                    "OSABalance RBMBridge: PBS {0}->{1}, CFM {2}->{3}, POM {4}->{5}",
                    PlateBelowShoulderBefore, PlateBelowShoulderAfter,
                    ClosedFaceMailBefore,     ClosedFaceMailAfter,
                    PlateOverMailBefore,      PlateOverMailAfter);
            }
        }

        public static Report Inject()
        {
            var r = new Report();
            try
            {
                var damageRework = AccessTools.TypeByName("RBMCombat.DamageRework");
                var armorRework  = AccessTools.TypeByName("RBMCombat.ArmorRework");
                r.DamageReworkFound = damageRework != null;
                r.ArmorReworkFound  = armorRework  != null;
                if (!r.DamageReworkFound || !r.ArmorReworkFound) return r;

                AppendField(damageRework, "PlateBelowShoulderArmorIds", OSAIds.PlateBelowShoulder,
                    out r.PlateBelowShoulderBefore, out r.PlateBelowShoulderAfter);
                AppendField(damageRework, "ClosedFaceMailHelmetIds", OSAIds.ClosedFaceMail,
                    out r.ClosedFaceMailBefore, out r.ClosedFaceMailAfter);
                AppendField(armorRework, "PlateOverMailBodyArmorIds", OSAIds.PlateOverMail,
                    out r.PlateOverMailBefore, out r.PlateOverMailAfter);
            }
            catch (Exception ex)
            {
                r.Error = ex.GetType().Name + ": " + ex.Message;
            }
            return r;
        }

        private static void AppendField(Type t, string fieldName, string[] additions,
                                        out int before, out int after)
        {
            before = -1; after = -1;
            var fi = AccessTools.Field(t, fieldName); // AccessTools: NonPublic|Static included
            if (fi == null)
                throw new InvalidOperationException("Field not found: " + t.FullName + "." + fieldName);
            var cur = (string[])fi.GetValue(null);
            before = cur?.Length ?? 0;
            var merged = (cur ?? new string[0]).Concat(additions).Distinct().ToArray();
            fi.SetValue(null, merged);
            // Verify the write landed.
            var readback = (string[])fi.GetValue(null);
            after = readback?.Length ?? 0;
            if (after != merged.Length)
                throw new InvalidOperationException("Write-back verification failed: " + fieldName);
        }
    }
}
