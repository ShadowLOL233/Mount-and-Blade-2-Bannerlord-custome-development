using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace GarrisonDrills;

public class SubModule : MBSubModuleBase
{
	protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
	{
		base.OnGameStart(game, gameStarterObject);
		if (game.GameType is Campaign && gameStarterObject is CampaignGameStarter campaignGameStarter)
		{
			try
			{
				campaignGameStarter.AddBehavior(new GarrisonDrillsBehavior());
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage("[GarrisonDrills] init failed: " + ex.Message, Colors.Red));
			}
		}
	}

	protected override void OnBeforeInitialModuleScreenSetAsRoot()
	{
		base.OnBeforeInitialModuleScreenSetAsRoot();
		InformationManager.DisplayMessage(new InformationMessage("[GarrisonDrills] active.", Colors.Green));
	}
}
