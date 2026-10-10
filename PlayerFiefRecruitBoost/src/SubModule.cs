using System;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace PlayerFiefRecruitBoost
{
    public class SubModule : MBSubModuleBase
    {
        private const string HarmonyId = "PlayerFiefRecruitBoost.Patch";
        private static bool _patched;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            if (_patched) return;
            try
            {
                Config.Load();
                var harmony = new Harmony(HarmonyId);
                harmony.PatchAll(typeof(SubModule).Assembly);
                _patched = true;

                if (Config.LogOnLoad)
                {
                    var line = string.Format(
                        "[PFRB] loaded · SlotRegen×{0:0.##} · UnlockMaxTier={1} · IncludeBoundVillages={2}",
                        Config.SlotRegenMultiplier,
                        Config.UnlockMaxTierInOwnFiefs,
                        Config.IncludeBoundVillages);
                    InformationManager.DisplayMessage(new InformationMessage(line, Colors.Cyan));
                }
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "[PFRB] patch install FAILED: " + ex.Message, Colors.Red));
            }
        }
    }
}
