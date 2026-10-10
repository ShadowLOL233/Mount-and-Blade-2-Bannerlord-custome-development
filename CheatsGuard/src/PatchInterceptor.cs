using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace CheatsGuard
{
    // Wraps HarmonyLib.PatchClassProcessor.Patch() around each Cheats patch class.
    // Only intercepts when the container type namespace starts with "BannerlordCheats.Patches"
    // — other mods' Harmony patches pass through unmodified.
    //
    // Prefix : log "Patching X" · consult skip-list · skip if listed
    // Finalizer : catch any exception (Cheats itself only catches HarmonyException)
    //             log it and swallow so the outer foreach loop keeps going
    //
    // The `containerType` field on PatchClassProcessor is private — read via
    // Harmony AccessTools.Field, cached in a static delegate for speed.

    [HarmonyPatch(typeof(PatchClassProcessor), nameof(PatchClassProcessor.Patch))]
    internal static class PatchInterceptor
    {
        private const string CheatsNsPrefix = "BannerlordCheats.Patches";

        private static readonly FieldInfo ContainerTypeField =
            AccessTools.Field(typeof(PatchClassProcessor), "containerType");

        private static Type GetContainerType(PatchClassProcessor pcp)
        {
            if (ContainerTypeField == null) return null;
            try { return (Type)ContainerTypeField.GetValue(pcp); }
            catch { return null; }
        }

        [HarmonyPrefix]
        static bool Prefix(PatchClassProcessor __instance, ref System.Collections.Generic.List<MethodInfo> __result)
        {
            var t = GetContainerType(__instance);
            if (t == null) return true;
            var fqn = t.FullName ?? string.Empty;

            // Skip-list gate applies to BannerlordCheats.Patches.*
            if (fqn.StartsWith(CheatsNsPrefix, StringComparison.Ordinal))
            {
                // Nuclear option: skip every single Cheats patch
                if (Config.SkipAllCheatsPatches)
                {
                    if (Config.VerboseLog)
                        LogFile.Write($"SKIP-ALL {fqn}");
                    __result = new System.Collections.Generic.List<MethodInfo>();
                    return false;
                }
                if (Config.IsSkipped(fqn))
                {
                    LogFile.Write($"SKIP    {fqn}   (per config skip-list)");
                    __result = new System.Collections.Generic.List<MethodInfo>();
                    return false;  // don't run original
                }
                if (Config.VerboseLog)
                    LogFile.Write($"TRY     {fqn}");
            }
            else if (fqn.StartsWith("BannerlordCheats.", StringComparison.Ordinal))
            {
                // Non-patch Cheats types (Settings, L10N, SubModule, etc.).
                // Log so we can see full iteration order — catches crash-between-patches cases.
                if (Config.VerboseLog)
                    LogFile.Write($"try(nonpatch) {fqn}");
            }
            return true;  // let original Patch() run
        }

        [HarmonyFinalizer]
        static Exception Finalizer(PatchClassProcessor __instance, Exception __exception)
        {
            var t = GetContainerType(__instance);
            var fqn = t?.FullName ?? "<unknown>";

            if (fqn.StartsWith(CheatsNsPrefix, StringComparison.Ordinal))
            {
                if (__exception == null)
                {
                    if (Config.VerboseLog)
                        LogFile.Write($"OK      {fqn}");
                    return null;
                }
                LogFile.Write($"FAIL    {fqn}   {__exception.GetType().Name}: {__exception.Message}");
                if (__exception.InnerException != null)
                    LogFile.Write($"  inner  {__exception.InnerException.GetType().Name}: {__exception.InnerException.Message}");
                return null;  // swallow so Cheats' foreach continues
            }
            else if (fqn.StartsWith("BannerlordCheats.", StringComparison.Ordinal))
            {
                if (__exception != null)
                {
                    LogFile.Write($"FAIL(nonpatch) {fqn}   {__exception.GetType().Name}: {__exception.Message}");
                    if (__exception.InnerException != null)
                        LogFile.Write($"  inner  {__exception.InnerException.GetType().Name}: {__exception.InnerException.Message}");
                    return null;  // swallow
                }
            }

            return __exception;  // pass-through for other mods
        }
    }

    // Also catch failures during PatchClassProcessor CONSTRUCTION (before Patch() runs).
    // TypeLoadException from HarmonyPatch attribute reading typeof(X) where X doesn't exist
    // in 1.4.7 would kill Cheats' foreach loop BEFORE our Patch prefix ever runs. This
    // Finalizer catches construction failures and lets iteration continue.
    [HarmonyPatch]
    internal static class ConstructorInterceptor
    {
        static MethodBase TargetMethod()
        {
            // Find the (Harmony, Type) constructor of PatchClassProcessor
            return typeof(PatchClassProcessor).GetConstructors()
                .FirstOrDefault(c =>
                {
                    var ps = c.GetParameters();
                    return ps.Length == 2
                        && ps[0].ParameterType == typeof(Harmony)
                        && ps[1].ParameterType == typeof(Type);
                });
        }

        [HarmonyPrefix]
        static void Prefix(Type type)
        {
            if (type == null) return;
            var fqn = type.FullName ?? string.Empty;
            if (fqn.StartsWith("BannerlordCheats.", StringComparison.Ordinal) && Config.VerboseLog)
                LogFile.Write($"CTOR    {fqn}");
        }

        [HarmonyFinalizer]
        static Exception Finalizer(Type type, Exception __exception)
        {
            if (type == null || __exception == null) return __exception;
            var fqn = type.FullName ?? string.Empty;
            if (!fqn.StartsWith("BannerlordCheats.", StringComparison.Ordinal))
                return __exception;

            LogFile.Write($"CTOR FAIL {fqn}   {__exception.GetType().Name}: {__exception.Message}");
            if (__exception.InnerException != null)
                LogFile.Write($"  inner    {__exception.InnerException.GetType().Name}: {__exception.InnerException.Message}");
            return null;  // swallow so loop continues
        }
    }
}
