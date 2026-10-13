using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace OSABalance.RBMBridge
{
    // Entry point for OSA Balance RBM Bridge.
    //
    // SubModule.xml declares RBM/OSA/OSW as LoadBeforeThis, so by the time our
    // OnSubModuleLoad runs, RBMCombat types are in-process. We mutate the three
    // RBMCombat whitelist arrays directly — not via Harmony Patch — because the
    // consumers (DamageRework / ArmorRework hit routines) read the arrays fresh
    // at mission time via Array.IndexOf. No patch target needed.
    //
    // Unlike RBMPlayerStaminaPoiseBuff (which must defer to OnMissionBehaviorInitialize
    // because RBMAI loads lazily), RBMCombat is loaded eagerly by RBM.SubModule, so
    // OnSubModuleLoad is a valid injection point.
    public class SubModule : MBSubModuleBase
    {
        private static bool _injected;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            if (_injected) return;
            _injected = true;

            var report = WhitelistInjector.Inject();
            // Message rendered on main menu (first UI frame) via DisplayMessage queue.
            // Red for failure, green for success.
            var msg = new InformationMessage(report.Summary(),
                report.Ok ? Colors.Green : Colors.Red);
            InformationManager.DisplayMessage(msg);
        }
    }
}
