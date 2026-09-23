using System;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace GarrisonDrillsMessageEnhancer
{
    public class SubModule : MBSubModuleBase
    {
        private const string HarmonyId = "GarrisonDrillsMessageEnhancer.Patch";
        private static bool _patched;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            if (_patched) return;
            try
            {
                var harmony = new Harmony(HarmonyId);
                harmony.PatchAll(typeof(SubModule).Assembly);
                _patched = true;
                InformationManager.DisplayMessage(new InformationMessage(
                    "[GD Msg Enhancer] patch installed.", Colors.Cyan));
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "[GD Msg Enhancer] patch install FAILED: " + ex.Message, Colors.Red));
            }
        }
    }
}
