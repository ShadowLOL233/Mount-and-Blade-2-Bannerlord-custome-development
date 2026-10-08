using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using UTMPatch.Util;

namespace UTMPatch
{
    public class UTMPatchSubModule : MBSubModuleBase
    {
        private const string HarmonyId = "com.situjingzhou.utmpatch";
        private Harmony _harmony;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                UTMPatchLog.Info("UTMPatch v1.0.1 OnSubModuleLoad · running on "
                    + Assembly.GetExecutingAssembly().GetName().Version);

                var fmLoaded = AppDomain.CurrentDomain.GetAssemblies()
                    .Any(a => a.GetName().Name == "FormationManager");
                var cytLoaded = AppDomain.CurrentDomain.GetAssemblies()
                    .Any(a => a.GetName().Name == "ChooseYourTroops");
                UTMPatchLog.Info("Mod presence · FormationManager=" + fmLoaded + " · ChooseYourTroops=" + cytLoaded);

                if (!fmLoaded)
                {
                    UTMPatchLog.Warn("FormationManager not loaded · UTMPatch's OoB undo would be destructive without FM · skipping Harmony.PatchAll");
                    return;
                }

                _harmony = new Harmony(HarmonyId);
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                UTMPatchLog.Info("Harmony.PatchAll · done");
            }
            catch (Exception ex)
            {
                UTMPatchLog.Exception("OnSubModuleLoad", ex);
            }
        }
    }
}
