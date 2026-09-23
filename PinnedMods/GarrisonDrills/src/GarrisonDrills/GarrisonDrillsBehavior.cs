using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace GarrisonDrills;

public class GarrisonDrillsBehavior : CampaignBehaviorBase
{
	private sealed class TrainTier
	{
		public readonly string NameKey;

		public readonly int Xp;

		public readonly int Gold;

		public TrainTier(string nameKey, int xp, int gold)
		{
			NameKey = nameKey;
			Xp = xp;
			Gold = gold;
		}
	}

	private const int HoursPerPulse = 6;

	private static readonly TrainTier[] Tiers = new TrainTier[3]
	{
		new TrainTier("{=TYT_NameBasic}Basic", 200, 5),
		new TrainTier("{=TYT_NameAdvanced}Advanced", 600, 20),
		new TrainTier("{=TYT_NameMasterful}Masterful", 1000, 40)
	};

	private const string SelectMenu = "tyt_training_select";

	private const string TrainingMenu = "tyt_training";

	private bool _isTraining;

	private int _tierIndex;

	private CampaignTime _nextPulseTime;

	public override void RegisterEvents()
	{
		CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
	}

	public override void SyncData(IDataStore dataStore)
	{
		dataStore.SyncData("GarrisonDrills_IsTraining", ref _isTraining);
		dataStore.SyncData("GarrisonDrills_TierIndex", ref _tierIndex);
		dataStore.SyncData("GarrisonDrills_NextPulse", ref _nextPulseTime);
	}

	private void OnSessionLaunched(CampaignGameStarter starter)
	{
		try
		{
			AddMenus(starter);
		}
		catch (Exception ex)
		{
			InformationManager.DisplayMessage(new InformationMessage("[GarrisonDrills] menu add failed: " + ex.Message, Colors.Red));
		}
	}

	public void AddMenus(CampaignGameStarter starter)
	{
		starter.AddGameMenuOption("town", "garrisondrills_train", "{=TYT_TrainOption}Train troops", StartCondition, OpenSelectMenu);
		starter.AddGameMenuOption("castle", "garrisondrills_train", "{=TYT_TrainOption}Train troops", StartCondition, OpenSelectMenu);
		starter.AddGameMenu("tyt_training_select", "{=TYT_SelectMenuText}Choose how hard to drill your troops. Higher intensity grants more experience over time, but costs more gold.", SelectMenuInit, GameMenu.MenuOverlayType.SettlementWithBoth);
		starter.AddGameMenuOption("tyt_training_select", "tyt_tier_0", "{=TYT_TierBasic}Basic training (+200 xp, 5{GOLD_ICON} per soldier every 6h)", (MenuCallbackArgs args) => TierCondition(args, 0), delegate
		{
			StartTier(0);
		});
		starter.AddGameMenuOption("tyt_training_select", "tyt_tier_1", "{=TYT_TierAdvanced}Advanced training (+600 xp, 20{GOLD_ICON} per soldier every 6h)", (MenuCallbackArgs args) => TierCondition(args, 1), delegate
		{
			StartTier(1);
		});
		starter.AddGameMenuOption("tyt_training_select", "tyt_tier_2", "{=TYT_TierMasterful}Masterful training (+1000 xp, 40{GOLD_ICON} per soldier every 6h)", (MenuCallbackArgs args) => TierCondition(args, 2), delegate
		{
			StartTier(2);
		});
		starter.AddGameMenuOption("tyt_training_select", "tyt_select_back", "{=TYT_Back}Back", BackCondition, OnBackOption, isLeave: true);
		starter.AddWaitGameMenu("tyt_training", "{=TYT_TrainingText}Your troops are drilling here. Every 6 hours they gain experience, and you pay for their training. Choose 'Stop training' to finish.", TrainingInit, TrainingCondition, null, TrainingTick, GameMenu.MenuAndOptionType.WaitMenuHideProgressAndHoursOption, GameMenu.MenuOverlayType.SettlementWithBoth);
		starter.AddGameMenuOption("tyt_training", "tyt_stop", "{=TYT_StopOption}Stop training", StopCondition, OnStopOption, isLeave: true);
	}

	private bool StartCondition(MenuCallbackArgs args)
	{
		args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
		try
		{
			return CountTrainableSoldiers(MobileParty.MainParty) > 0;
		}
		catch
		{
			return true;
		}
	}

	private void OpenSelectMenu(MenuCallbackArgs args)
	{
		GameMenu.SwitchToMenu("tyt_training_select");
	}

	private void SelectMenuInit(MenuCallbackArgs args)
	{
	}

	private bool TierCondition(MenuCallbackArgs args, int tierIndex)
	{
		args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
		TrainTier trainTier = Tiers[tierIndex];
		int num = SafeTrainableCount() * trainTier.Gold;
		if (Hero.MainHero.Gold < num)
		{
			TextObject textObject = new TextObject("{=TYT_NeedGold}Not enough gold: one training pulse costs {COST}{GOLD_ICON}.");
			textObject.SetTextVariable("COST", num);
			args.IsEnabled = false;
			args.Tooltip = textObject;
		}
		return true;
	}

	private void StartTier(int tierIndex)
	{
		_tierIndex = ((tierIndex >= 0 && tierIndex < Tiers.Length) ? tierIndex : 0);
		_isTraining = true;
		_nextPulseTime = CampaignTime.Now + CampaignTime.Hours(6f);
		GameMenu.SwitchToMenu("tyt_training");
	}

	private bool BackCondition(MenuCallbackArgs args)
	{
		args.optionLeaveType = GameMenuOption.LeaveType.Leave;
		return true;
	}

	private void OnBackOption(MenuCallbackArgs args)
	{
		GameMenu.SwitchToMenu(BackMenuId());
	}

	private void TrainingInit(MenuCallbackArgs args)
	{
		if (PlayerEncounter.Current != null)
		{
			PlayerEncounter.Current.IsPlayerWaiting = true;
		}
		MobileParty.MainParty?.SetMoveModeHold();
	}

	private bool TrainingCondition(MenuCallbackArgs args)
	{
		return true;
	}

	private void TrainingTick(MenuCallbackArgs args, CampaignTime dt)
	{
		if (_isTraining && CampaignTime.Now >= _nextPulseTime)
		{
			_nextPulseTime = CampaignTime.Now + CampaignTime.Hours(6f);
			DoPulse(args);
		}
	}

	private bool StopCondition(MenuCallbackArgs args)
	{
		args.optionLeaveType = GameMenuOption.LeaveType.Leave;
		return true;
	}

	private void OnStopOption(MenuCallbackArgs args)
	{
		StopTraining(args, "{=TYT_Stopped}Training finished.", Colors.White);
	}

	private void DoPulse(MenuCallbackArgs args)
	{
		MobileParty mainParty = MobileParty.MainParty;
		int num = CountTrainableSoldiers(mainParty);
		if (num <= 0)
		{
			StopTraining(args, "{=TYT_NoTroops}No upgradeable troops to train.", Colors.White);
			return;
		}
		TrainTier trainTier = CurrentTier();
		int num2 = num * trainTier.Gold;
		if (Hero.MainHero.Gold < num2)
		{
			StopTraining(args, "{=TYT_OutOfGold}Training stopped: not enough gold.", Colors.Red);
			return;
		}
		GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, num2, disableNotification: true);
		ApplyTraining(mainParty, trainTier.Xp);
		TextObject textObject = new TextObject("{=TYT_PulseMsg}{TIER} training: experience gained by {COUNT} soldiers (-{COST}{GOLD_ICON}).");
		textObject.SetTextVariable("TIER", new TextObject(trainTier.NameKey));
		textObject.SetTextVariable("COUNT", num);
		textObject.SetTextVariable("COST", num2);
		InformationManager.DisplayMessage(new InformationMessage(textObject.ToString(), Colors.Green));
	}

	private void StopTraining(MenuCallbackArgs args, string messageKey, Color color)
	{
		_isTraining = false;
		if (PlayerEncounter.Current != null)
		{
			PlayerEncounter.Current.IsPlayerWaiting = false;
		}
		if (messageKey != null)
		{
			InformationManager.DisplayMessage(new InformationMessage(new TextObject(messageKey).ToString(), color));
		}
		GameMenu.SwitchToMenu(BackMenuId());
	}

	private TrainTier CurrentTier()
	{
		int num = _tierIndex;
		if (num < 0 || num >= Tiers.Length)
		{
			num = 0;
		}
		return Tiers[num];
	}

	private static string BackMenuId()
	{
		Settlement currentSettlement = Settlement.CurrentSettlement;
		if (currentSettlement == null || !currentSettlement.IsCastle)
		{
			return "town";
		}
		return "castle";
	}

	private static int SafeTrainableCount()
	{
		try
		{
			return CountTrainableSoldiers(MobileParty.MainParty);
		}
		catch
		{
			return 0;
		}
	}

	private static int CountTrainableSoldiers(MobileParty party)
	{
		TroopRoster troopRoster = party?.MemberRoster;
		if (troopRoster == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < troopRoster.Count; i++)
		{
			if (IsTrainable(troopRoster.GetCharacterAtIndex(i)))
			{
				num += troopRoster.GetElementNumber(i);
			}
		}
		return num;
	}

	private static void ApplyTraining(MobileParty party, int xpPerSoldier)
	{
		TroopRoster memberRoster = party.MemberRoster;
		List<KeyValuePair<CharacterObject, int>> list = new List<KeyValuePair<CharacterObject, int>>();
		for (int i = 0; i < memberRoster.Count; i++)
		{
			CharacterObject characterAtIndex = memberRoster.GetCharacterAtIndex(i);
			int elementNumber = memberRoster.GetElementNumber(i);
			if (elementNumber > 0 && IsTrainable(characterAtIndex))
			{
				list.Add(new KeyValuePair<CharacterObject, int>(characterAtIndex, elementNumber));
			}
		}
		foreach (KeyValuePair<CharacterObject, int> item in list)
		{
			memberRoster.AddXpToTroop(item.Key, item.Value * xpPerSoldier);
		}
	}

	private static bool IsTrainable(CharacterObject c)
	{
		if (c != null && c.IsRegular && c.UpgradeTargets != null)
		{
			return c.UpgradeTargets.Length != 0;
		}
		return false;
	}
}
