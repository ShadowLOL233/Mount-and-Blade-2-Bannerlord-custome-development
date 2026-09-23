using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace GarrisonDrillsMessageEnhancer
{
    // Prefix on GarrisonDrills.GarrisonDrillsBehavior.DoPulse.
    // Reimplements the pulse with a message that also shows XP-per-soldier and total XP.
    // Uses reflection to call the target's private helpers (CurrentTier, ApplyTraining,
    // CountTrainableSoldiers, StopTraining) so the game-logic branch parity is preserved
    // without duplicating helper bodies.
    [HarmonyPatch]
    internal static class DoPulsePatch
    {
        private static Type _behaviorType;
        private static MethodInfo _mCountTrainableSoldiers;
        private static MethodInfo _mCurrentTier;
        private static MethodInfo _mApplyTraining;
        private static MethodInfo _mStopTraining;
        private static FieldInfo  _fXp;
        private static FieldInfo  _fGold;
        private static FieldInfo  _fNameKey;

        static MethodBase TargetMethod()
        {
            _behaviorType = AccessTools.TypeByName("GarrisonDrills.GarrisonDrillsBehavior")
                ?? throw new InvalidOperationException("GarrisonDrills.GarrisonDrillsBehavior type not found (GarrisonDrillsPinned missing?)");

            _mCountTrainableSoldiers = AccessTools.Method(_behaviorType, "CountTrainableSoldiers", new[] { typeof(MobileParty) });
            _mCurrentTier            = AccessTools.Method(_behaviorType, "CurrentTier");
            _mApplyTraining          = AccessTools.Method(_behaviorType, "ApplyTraining", new[] { typeof(MobileParty), typeof(int) });
            _mStopTraining           = AccessTools.Method(_behaviorType, "StopTraining", new[] { typeof(MenuCallbackArgs), typeof(string), typeof(Color) });

            var tierType = AccessTools.Inner(_behaviorType, "TrainTier");
            _fXp      = AccessTools.Field(tierType, "Xp");
            _fGold    = AccessTools.Field(tierType, "Gold");
            _fNameKey = AccessTools.Field(tierType, "NameKey");

            return AccessTools.Method(_behaviorType, "DoPulse", new[] { typeof(MenuCallbackArgs) });
        }

        static bool Prefix(object __instance, MenuCallbackArgs args)
        {
            var mainParty = MobileParty.MainParty;
            int trainable = (int)_mCountTrainableSoldiers.Invoke(null, new object[] { mainParty });
            if (trainable <= 0)
            {
                _mStopTraining.Invoke(__instance, new object[] {
                    args, "{=TYT_NoTroops}No upgradeable troops to train.", Colors.White });
                return false;
            }

            object tier = _mCurrentTier.Invoke(__instance, null);
            int xpPerSoldier = (int)_fXp.GetValue(tier);
            int goldPerSoldier = (int)_fGold.GetValue(tier);
            string nameKey = (string)_fNameKey.GetValue(tier);
            int cost = trainable * goldPerSoldier;

            if (Hero.MainHero.Gold < cost)
            {
                _mStopTraining.Invoke(__instance, new object[] {
                    args, "{=TYT_OutOfGold}Training stopped: not enough gold.", Colors.Red });
                return false;
            }

            GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, cost, disableNotification: true);
            _mApplyTraining.Invoke(null, new object[] { mainParty, xpPerSoldier });

            int totalXp = trainable * xpPerSoldier;
            var msg = new TextObject("{=GDMsgEnh_Pulse}{TIER} training: {COUNT} soldiers each gained +{XP_PER} xp (total +{XP_TOTAL} xp, -{COST}{GOLD_ICON}).");
            msg.SetTextVariable("TIER", new TextObject(nameKey));
            msg.SetTextVariable("COUNT", trainable);
            msg.SetTextVariable("XP_PER", xpPerSoldier);
            msg.SetTextVariable("XP_TOTAL", totalXp);
            msg.SetTextVariable("COST", cost);
            InformationManager.DisplayMessage(new InformationMessage(msg.ToString(), Colors.Green));
            return false;
        }
    }
}
