using System;

namespace CheatsGuard.MCM
{
    // Marks a CheatsGuardMCMSettings property as mirroring a public static field on
    // RBMConfig.RBMConfig. SubModule.SyncFromMcm reflects over these on every
    // PropertyChanged and writes the value through RBMBridge.SetField.
    //
    // Maintenance 3-field trio does NOT carry this attribute — they are player-scoped
    // by MaintenancePlayerScopePatch and must not touch the global RBMConfig statics.
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    internal sealed class RbmFieldAttribute : Attribute
    {
        public string Name { get; }
        public RbmFieldAttribute(string name) { Name = name; }
    }
}
