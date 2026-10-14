using System;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace CustomBattleTroopBuilder
{
    /// <summary>
    /// Shared reflection-based clone helpers · used by CBTBEditorVM.CompleteClone (first clone)
    /// 和 CBTBClonesRegistry.Restore (after Bannerlord Game re-init wipes MBObjectManager).
    ///
    /// BasicCharacterObject v1.4.7 真实 accessibility:
    /// - FillFrom(BasicCharacterObject): protected · 需 reflection
    /// - Name: read-only property · 需 reflection 设 _basicName field (protected TextObject)
    /// - IsSoldier: public getter / private setter · 需 reflection 设 setter
    /// </summary>
    public static class CBTBCloneHelper
    {
        private static MethodInfo? _fillFromMethod;
        private static FieldInfo? _basicNameField;
        private static MethodInfo? _isSoldierSetter;
        private static bool _resolved;

        private static void EnsureResolved()
        {
            if (_resolved) return;
            try
            {
                _fillFromMethod = typeof(BasicCharacterObject).GetMethod("FillFrom",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null, new Type[] { typeof(BasicCharacterObject) }, null);

                _basicNameField = typeof(BasicCharacterObject).GetField("_basicName",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                var isSoldierProp = typeof(BasicCharacterObject).GetProperty("IsSoldier",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                _isSoldierSetter = isSoldierProp?.GetSetMethod(nonPublic: true);
            }
            catch (Exception e)
            {
                CBTBSubModule.Log($"[CBTB.Helper] EnsureResolved failed: {e}");
            }
            _resolved = true;
        }

        public static bool CallFillFrom(BasicCharacterObject target, BasicCharacterObject source)
        {
            EnsureResolved();
            if (_fillFromMethod == null || target == null || source == null) return false;
            try
            {
                _fillFromMethod.Invoke(target, new object[] { source });
                return true;
            }
            catch (Exception e)
            {
                CBTBSubModule.Log($"[CBTB.Helper] FillFrom reflection failed: {e}");
                return false;
            }
        }

        public static bool SetName(BasicCharacterObject target, string displayName)
        {
            EnsureResolved();
            if (target == null || displayName == null) return false;
            try
            {
                if (_basicNameField != null)
                {
                    _basicNameField.SetValue(target, new TextObject(displayName));
                    return true;
                }
                var nameProp = typeof(BasicCharacterObject).GetProperty("Name");
                var setter = nameProp?.GetSetMethod(nonPublic: true);
                if (setter != null)
                {
                    setter.Invoke(target, new object[] { new TextObject(displayName) });
                    return true;
                }
            }
            catch (Exception e)
            {
                CBTBSubModule.Log($"[CBTB.Helper] SetName reflection failed: {e}");
            }
            return false;
        }

        public static bool SetIsSoldier(BasicCharacterObject target, bool value)
        {
            EnsureResolved();
            if (_isSoldierSetter == null || target == null) return false;
            try
            {
                _isSoldierSetter.Invoke(target, new object[] { value });
                return true;
            }
            catch (Exception e)
            {
                CBTBSubModule.Log($"[CBTB.Helper] SetIsSoldier reflection failed: {e}");
                return false;
            }
        }

        /// <summary>
        /// Full clone sequence · reused by CompleteClone (first) and Registry.Restore (re-create after Game re-init).
        /// Returns the new instance (already registered in MBObjectManager) or null on failure.
        /// </summary>
        public static BasicCharacterObject? CloneTroop(BasicCharacterObject source, string newStringId, string displayName)
        {
            if (source == null || string.IsNullOrWhiteSpace(newStringId)) return null;

            BasicCharacterObject? newTroop;
            try
            {
                newTroop = MBObjectManager.Instance?.CreateObject<BasicCharacterObject>(newStringId);
            }
            catch (Exception e)
            {
                CBTBSubModule.Log($"[CBTB.Helper] CreateObject({newStringId}) threw: {e}");
                return null;
            }

            if (newTroop == null)
            {
                CBTBSubModule.Log($"[CBTB.Helper] CreateObject({newStringId}) returned null");
                return null;
            }

            CallFillFrom(newTroop, source);
            SetName(newTroop, displayName ?? newStringId);
            SetIsSoldier(newTroop, source.IsSoldier);
            return newTroop;
        }
    }
}
