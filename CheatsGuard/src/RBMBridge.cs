using System;
using System.Linq;
using System.Reflection;

namespace CheatsGuard
{
    // Reflection-based bridge to RBMConfig statics.
    //
    // Why reflection: CheatsGuard must not hard-depend on RBM being installed —
    // if RBM is absent (or renamed a field in a future version) we degrade to
    // a silent no-op instead of failing to load.
    //
    // Target fields (all `public static` on RBMConfig.RBMConfig):
    //   troopMaintenanceFraction : float  (default 0.005)
    //   recruitMaintenanceDays   : int    (default 20)
    //   independentMaintenancePurseFraction : float (default 1.0)
    //
    // RBM re-saves the XML config.xml on OnGameEnd / OnSaveOverEvent, so we push
    // values in on load + on every MCM change.
    internal static class RBMBridge
    {
        private static Type _rbmConfigType;
        private static FieldInfo _troopMaintenanceFraction;
        private static FieldInfo _recruitMaintenanceDays;
        private static FieldInfo _independentPurseFraction;
        private static bool _resolved;

        public static bool IsAvailable => Resolve();

        private static bool Resolve()
        {
            if (_resolved) return _rbmConfigType != null;
            _resolved = true;

            try
            {
                // RBM's config lives in RBMConfig.dll as `RBMConfig.RBMConfig` (namespace + class name collide).
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (asm.IsDynamic) continue;
                    string name;
                    try { name = asm.GetName().Name; } catch { continue; }
                    if (name != "RBMConfig") continue;

                    _rbmConfigType = asm.GetType("RBMConfig.RBMConfig", throwOnError: false);
                    if (_rbmConfigType != null) break;
                }

                if (_rbmConfigType == null)
                {
                    LogFile.Write("RBMBridge: RBMConfig assembly / RBMConfig.RBMConfig type not found — maintenance sliders will be inert.");
                    return false;
                }

                _troopMaintenanceFraction = _rbmConfigType.GetField("troopMaintenanceFraction",           BindingFlags.Public | BindingFlags.Static);
                _recruitMaintenanceDays   = _rbmConfigType.GetField("recruitMaintenanceDays",             BindingFlags.Public | BindingFlags.Static);
                _independentPurseFraction = _rbmConfigType.GetField("independentMaintenancePurseFraction", BindingFlags.Public | BindingFlags.Static);

                LogFile.Write($"RBMBridge: RBMConfig found · fields resolved = " +
                              $"troopMaintenanceFraction:{_troopMaintenanceFraction != null} · " +
                              $"recruitMaintenanceDays:{_recruitMaintenanceDays != null} · " +
                              $"independentPurseFraction:{_independentPurseFraction != null}");
                return true;
            }
            catch (Exception ex)
            {
                LogFile.Write($"RBMBridge.Resolve failed: {ex.GetType().Name}: {ex.Message}");
                _rbmConfigType = null;
                return false;
            }
        }

        public static void Apply(float troopMaintenanceFraction, int recruitMaintenanceDays, float independentPurseFraction)
        {
            if (!Resolve()) return;
            try
            {
                _troopMaintenanceFraction?.SetValue(null, troopMaintenanceFraction);
                _recruitMaintenanceDays?.SetValue(null, recruitMaintenanceDays);
                _independentPurseFraction?.SetValue(null, independentPurseFraction);
                LogFile.Write($"RBMBridge.Apply: fraction={troopMaintenanceFraction:0.####} · recruitFreeDays={recruitMaintenanceDays} · indepPurse={independentPurseFraction:0.##}");
            }
            catch (Exception ex)
            {
                LogFile.Write($"RBMBridge.Apply failed: {ex.GetType().Name}: {ex.Message}");
            }
        }

        // Generic setter for arbitrary RBMConfig.RBMConfig public static field. Returns
        // true on success; false silently when RBM is missing, the field name is unknown,
        // or the type doesn't line up (in which case the write is skipped). Used by
        // SubModule.SyncFromMcm to fan out ~90 MCM-mirrored fields via reflection.
        public static bool SetField(string fieldName, object value)
        {
            if (!Resolve() || string.IsNullOrEmpty(fieldName)) return false;
            try
            {
                var f = _rbmConfigType.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
                if (f == null) return false;
                var target = value;
                // Bannerlord's MCM sometimes emits int for float sliders when Order/Min/Max
                // hint suggests an integer scale; coerce as needed.
                if (value != null && f.FieldType != value.GetType())
                {
                    if (f.FieldType == typeof(float) && value is int iv)      target = (float)iv;
                    else if (f.FieldType == typeof(int)  && value is float fv) target = (int)fv;
                    else if (f.FieldType == typeof(string))                    target = value.ToString();
                }
                f.SetValue(null, target);
                return true;
            }
            catch (Exception ex)
            {
                LogFile.Write($"RBMBridge.SetField({fieldName}) failed: {ex.GetType().Name}: {ex.Message}");
                return false;
            }
        }

        // Read current RBM values (for logging / diagnostics).
        public static (float fraction, int recruitDays, float indepPurse)? ReadCurrent()
        {
            if (!Resolve()) return null;
            try
            {
                var f = _troopMaintenanceFraction != null ? (float)_troopMaintenanceFraction.GetValue(null) : 0f;
                var d = _recruitMaintenanceDays   != null ? (int)  _recruitMaintenanceDays.GetValue(null)   : 0;
                var p = _independentPurseFraction != null ? (float)_independentPurseFraction.GetValue(null) : 0f;
                return (f, d, p);
            }
            catch { return null; }
        }
    }
}
