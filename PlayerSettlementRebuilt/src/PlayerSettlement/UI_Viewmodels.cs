using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Xml;
using Bannerlord.ButterLib.HotKeys;
using Bannerlord.UIExtenderEx;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;
using Bannerlord.UIExtenderEx.ViewModels;
using BannerlordPlayerSettlement.Behaviours;
using BannerlordPlayerSettlement.Descriptors;
using BannerlordPlayerSettlement.Extensions;
using BannerlordPlayerSettlement.HotKeys;
using BannerlordPlayerSettlement.Patches;
using BannerlordPlayerSettlement.Patches.Compatibility.Interfaces;
using BannerlordPlayerSettlement.Saves;
using BannerlordPlayerSettlement.UI.Viewmodels;
using BannerlordPlayerSettlement.Utils;
using HarmonyLib;
using Helpers;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using SandBox;
using SandBox.Missions.AgentBehaviors;
using SandBox.Missions.MissionLogics;
using SandBox.Missions.MissionLogics.Towns;
using SandBox.Objects.AreaMarkers;
using SandBox.View.Map;
using SandBox.View.Map.Managers;
using SandBox.View.Map.Visuals;
using SandBox.ViewModelCollection;
using SandBox.ViewModelCollection.MapSiege;
using SandBox.ViewModelCollection.Nameplate;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Incidents;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar;
using TaleWorlds.MountAndBlade.Objects;
using TaleWorlds.MountAndBlade.Source.Objects;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;
using TaleWorlds.SaveSystem.Resolvers;
using TaleWorlds.ScreenSystem;


namespace BannerlordPlayerSettlement.UI.Viewmodels
{
	[ViewModelMixin("RefreshValues", true)]
	public class MapBarExtensionVM : BaseViewModelMixin<MapBarVM>
	{
		public static MapBarExtensionVM? Current;

		private PlayerSettlementInfoVM? _playerSettlementInfo;

		[DataSourceProperty]
		public PlayerSettlementInfoVM? PlayerSettlementInfo
		{
			get
			{
				return _playerSettlementInfo;
			}
			set
			{
				if (value != _playerSettlementInfo)
				{
					_playerSettlementInfo = value;
					MapBarVM viewModel = base.ViewModel;
					if (viewModel != null)
					{
						((ViewModel)viewModel).OnPropertyChangedWithValue<PlayerSettlementInfoVM>(value, "PlayerSettlementInfo");
					}
				}
			}
		}

		public MapBarExtensionVM(MapBarVM vm)
			: base(vm)
		{
			Current = this;
			_playerSettlementInfo = new PlayerSettlementInfoVM(this, vm);
		}

		public void Tick(float dt)
		{
			_playerSettlementInfo?.Tick(dt);
		}

		public override void OnRefresh()
		{
			base.OnRefresh();
			PlayerSettlementInfoVM? playerSettlementInfo = _playerSettlementInfo;
			if (playerSettlementInfo != null)
			{
				((ViewModel)playerSettlementInfo).RefreshValues();
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			_playerSettlementInfo = null;
			Current = null;
		}
	}
	public class PlayerSettlementBuildVM : ViewModel
	{
		private readonly PlayerSettlementInfoVM? owner;

		private readonly SettlementType settlementType = SettlementType.None;

		private bool _isCreatePlayerSettlementEnabled = true;

		private bool _isCreatePlayerSettlementVisible = true;

		private HintViewModel? _disableReasonHint;

		[DataSourceProperty]
		public bool IsCreatePlayerSettlementAllowed
		{
			get
			{
				return _isCreatePlayerSettlementEnabled && _isCreatePlayerSettlementVisible;
			}
			set
			{
				if (value != _isCreatePlayerSettlementEnabled)
				{
					_isCreatePlayerSettlementEnabled = value;
					((ViewModel)this).OnPropertyChangedWithValue(value, "IsCreatePlayerSettlementAllowed");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCreatePlayerSettlementVisible
		{
			get
			{
				return _isCreatePlayerSettlementVisible;
			}
			set
			{
				if (value != _isCreatePlayerSettlementVisible)
				{
					_isCreatePlayerSettlementVisible = value;
					((ViewModel)this).OnPropertyChangedWithValue(value, "IsCreatePlayerSettlementVisible");
				}
			}
		}

		[DataSourceProperty]
		public string CreatePlayerSettlementText => (settlementType == SettlementType.Village) ? ((object)new TextObject("{=player_settlement_13}Build a Village", (Dictionary<string, object>)null)).ToString() : ((settlementType == SettlementType.Castle) ? ((object)new TextObject("{=player_settlement_19}Build a Castle", (Dictionary<string, object>)null)).ToString() : ((object)new TextObject("{=player_settlement_04}Build a Town", (Dictionary<string, object>)null)).ToString());

		[DataSourceProperty]
		public HintViewModel? DisableHint
		{
			get
			{
				return _disableReasonHint;
			}
			set
			{
				if (value != _disableReasonHint)
				{
					_disableReasonHint = value;
					((ViewModel)this).OnPropertyChangedWithValue<HintViewModel>(value, "DisableHint");
				}
			}
		}

		public PlayerSettlementBuildVM()
		{
		}

		public PlayerSettlementBuildVM(PlayerSettlementInfoVM owner, SettlementType settlementType)
		{
			this.owner = owner;
			this.settlementType = settlementType;
		}

		[DataSourceMethod]
		public void ExecuteCreatePlayerSettlement()
		{
			if (settlementType != 0 && PlayerSettlementBehaviour.Instance != null)
			{
				PlayerSettlementBehaviour.Instance.SettlementRequest = settlementType;
				IsCreatePlayerSettlementAllowed = false;
				IsCreatePlayerSettlementVisible = false;
				PlayerSettlementInfoVM? playerSettlementInfoVM = owner;
				if (playerSettlementInfoVM != null)
				{
					((ViewModel)playerSettlementInfoVM).RefreshValues();
				}
				Campaign.Current.TimeControlMode = (CampaignTimeControlMode)1;
			}
		}

		internal void CalculateEnabled()
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Expected O, but got Unknown
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Expected O, but got Unknown
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Expected O, but got Unknown
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_058b: Expected O, but got Unknown
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Expected O, but got Unknown
			//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f3: Expected O, but got Unknown
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Expected O, but got Unknown
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Expected O, but got Unknown
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Expected O, but got Unknown
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Expected O, but got Unknown
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Expected O, but got Unknown
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Expected O, but got Unknown
			//IL_0574: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Expected O, but got Unknown
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Expected O, but got Unknown
			TextObject val = null;
			if (Main.Settings == null || PlayerSettlementInfo.Instance == null || PlayerSettlementBehaviour.Instance == null)
			{
				val = new TextObject("", (Dictionary<string, object>)null);
				IsCreatePlayerSettlementAllowed = false;
				IsCreatePlayerSettlementVisible = false;
				DisableHint = new HintViewModel(val, (string)null);
				return;
			}
			if (PlayerSettlementInfo.Instance.Towns == null)
			{
				PlayerSettlementInfo.Instance.Towns = new List<PlayerSettlementItem>();
			}
			if (PlayerSettlementInfo.Instance.Castles == null)
			{
				PlayerSettlementInfo.Instance.Castles = new List<PlayerSettlementItem>();
			}
			if (PlayerSettlementInfo.Instance.PlayerVillages == null)
			{
				PlayerSettlementInfo.Instance.PlayerVillages = new List<PlayerSettlementItem>();
			}
			if (settlementType == SettlementType.Town && PlayerSettlementInfo.Instance.Towns.Count >= Main.Settings.MaxTowns)
			{
				if (val == null)
				{
					val = new TextObject("{=player_settlement_h_08}Maximum number of towns have been built", (Dictionary<string, object>)null);
				}
				IsCreatePlayerSettlementAllowed = false;
				IsCreatePlayerSettlementVisible = false;
			}
			else if (settlementType == SettlementType.Castle && PlayerSettlementInfo.Instance.Castles.Count >= Main.Settings.MaxCastles)
			{
				if (val == null)
				{
					val = new TextObject("{=player_settlement_h_09}Maximum number of castles have been built", (Dictionary<string, object>)null);
				}
				IsCreatePlayerSettlementAllowed = false;
				IsCreatePlayerSettlementVisible = false;
			}
			else if (settlementType == SettlementType.Village && PlayerSettlementInfo.Instance.TotalVillages >= int.MaxValue)
			{
				if (val == null)
				{
					val = new TextObject("{=player_settlement_h_10}Maximum number of villages have been built", (Dictionary<string, object>)null);
				}
				IsCreatePlayerSettlementAllowed = false;
				IsCreatePlayerSettlementVisible = false;
			}
			else if (PlayerSettlementBehaviour.Instance.ReachedMax || PlayerSettlementBehaviour.Instance.HasRequest || PlayerSettlementBehaviour.Instance.IsPlacingSettlement || PlayerSettlementBehaviour.Instance.IsPlacingGate || PlayerSettlementBehaviour.Instance.IsPlacingPort)
			{
				if (val == null)
				{
					val = new TextObject(" - ", (Dictionary<string, object>)null);
				}
				IsCreatePlayerSettlementAllowed = false;
				IsCreatePlayerSettlementVisible = false;
			}
			else
			{
				IsCreatePlayerSettlementVisible = true;
			}
			if (Main.Settings != null && Main.Settings.Enabled)
			{
				if ((Game.Current == null || Campaign.Current == null || Hero.MainHero == null) && val == null)
				{
					val = new TextObject("{=player_settlement_h_02}Not in an active game!", (Dictionary<string, object>)null);
				}
				if (Main.Settings.RequireClanTier)
				{
					Hero mainHero = Hero.MainHero;
					int? obj;
					if (mainHero == null)
					{
						obj = null;
					}
					else
					{
						Clan clan = mainHero.Clan;
						obj = ((clan != null) ? new int?(clan.Tier) : null);
					}
					int? num = obj;
					if (num.GetValueOrDefault() < Main.Settings.RequiredClanTier)
					{
						if (val == null)
						{
							val = new TextObject("{=player_settlement_h_03}Clan tier too low. {TIER} required", (Dictionary<string, object>)null);
						}
						val.SetTextVariable("TIER", Main.Settings.RequiredClanTier);
					}
				}
				if (Main.Settings.SingleConstruction && (PlayerSettlementInfo.Instance.PlayerVillages.Any(delegate(PlayerSettlementItem t)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					CampaignTime buildEnd5 = t.BuildEnd;
					return (buildEnd5).IsFuture;
				}) || PlayerSettlementInfo.Instance.Towns.Any(delegate(PlayerSettlementItem t)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					CampaignTime buildEnd3 = t.BuildEnd;
					return (buildEnd3).IsFuture || t.Villages.Any(delegate(PlayerSettlementItem v)
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0006: Unknown result type (might be due to invalid IL or missing references)
						CampaignTime buildEnd4 = v.BuildEnd;
						return (buildEnd4).IsFuture;
					});
				}) || PlayerSettlementInfo.Instance.Castles.Any(delegate(PlayerSettlementItem c)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					CampaignTime buildEnd = c.BuildEnd;
					return (buildEnd).IsFuture || c.Villages.Any(delegate(PlayerSettlementItem v)
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0006: Unknown result type (might be due to invalid IL or missing references)
						CampaignTime buildEnd2 = v.BuildEnd;
						return (buildEnd2).IsFuture;
					});
				})) && val == null)
				{
					val = new TextObject("{=player_settlement_h_06}Construction in progress", (Dictionary<string, object>)null);
				}
				if (settlementType == SettlementType.Village && Main.Settings.RequireVillageGold)
				{
					Hero mainHero2 = Hero.MainHero;
					if (((mainHero2 != null) ? mainHero2.Gold : 0) < Main.Settings.RequiredVillageGold)
					{
						if (val == null)
						{
							val = new TextObject("{=player_settlement_h_05}Not enough funds ({CURRENT_FUNDS}/{REQUIRED_FUNDS})", (Dictionary<string, object>)null);
						}
						TextObject obj2 = val;
						Hero mainHero3 = Hero.MainHero;
						obj2.SetTextVariable("CURRENT_FUNDS", (mainHero3 != null) ? mainHero3.Gold : 0);
						val.SetTextVariable("REQUIRED_FUNDS", Main.Settings.RequiredVillageGold);
					}
				}
				else if (settlementType == SettlementType.Town && Main.Settings.RequireGold)
				{
					Hero mainHero4 = Hero.MainHero;
					if (((mainHero4 != null) ? mainHero4.Gold : 0) < Main.Settings.RequiredGold)
					{
						if (val == null)
						{
							val = new TextObject("{=player_settlement_h_05}Not enough funds ({CURRENT_FUNDS}/{REQUIRED_FUNDS})", (Dictionary<string, object>)null);
						}
						TextObject obj3 = val;
						Hero mainHero5 = Hero.MainHero;
						obj3.SetTextVariable("CURRENT_FUNDS", (mainHero5 != null) ? mainHero5.Gold : 0);
						val.SetTextVariable("REQUIRED_FUNDS", Main.Settings.RequiredGold);
					}
				}
				else if (settlementType == SettlementType.Castle && Main.Settings.RequireCastleGold)
				{
					Hero mainHero6 = Hero.MainHero;
					if (((mainHero6 != null) ? mainHero6.Gold : 0) < Main.Settings.RequiredCastleGold)
					{
						if (val == null)
						{
							val = new TextObject("{=player_settlement_h_05}Not enough funds ({CURRENT_FUNDS}/{REQUIRED_FUNDS})", (Dictionary<string, object>)null);
						}
						TextObject obj4 = val;
						Hero mainHero7 = Hero.MainHero;
						obj4.SetTextVariable("CURRENT_FUNDS", (mainHero7 != null) ? mainHero7.Gold : 0);
						val.SetTextVariable("REQUIRED_FUNDS", Main.Settings.RequiredCastleGold);
					}
				}
				if (settlementType == SettlementType.Village && PlayerSettlementBehaviour.Instance.GetPotentialVillageBoundOwners().Count() == 0 && val == null)
				{
					val = new TextObject("{=player_settlement_h_11}No candidate for village to be bound to", (Dictionary<string, object>)null);
				}
			}
			else
			{
				val = new TextObject("{=player_settlement_h_04}Player Settlement not enabled", (Dictionary<string, object>)null);
			}
			if (val == (TextObject)null)
			{
				val = TextObjectCompat.Empty;
				IsCreatePlayerSettlementAllowed = true;
			}
			else
			{
				IsCreatePlayerSettlementAllowed = false;
				Settings? settings = Main.Settings;
				if (settings == null || settings.HideButtonUntilReady)
				{
					IsCreatePlayerSettlementVisible = false;
				}
			}
			if (!IsCreatePlayerSettlementAllowed)
			{
				DisableHint = new HintViewModel(val, (string)null);
				return;
			}
			DisableHint = null;
			IsCreatePlayerSettlementVisible = true;
		}
	}
	public class PlayerSettlementInfoVM : MapInfoVM
	{
		private readonly MapBarExtensionVM? mapBarExtensionVM;

		private readonly MapBarVM? mapBarVM;

		private PlayerSettlementBuildVM? _playerTownBuildInfo;

		private PlayerSettlementBuildVM? _playerVillageBuildInfo;

		private PlayerSettlementBuildVM? _playerCastleBuildInfo;

		private HintViewModel? _disableReasonHint;

		private bool forceHide = false;

		[DataSourceProperty]
		public PlayerSettlementBuildVM? PlayerTownBuildInfo
		{
			get
			{
				return _playerTownBuildInfo;
			}
			set
			{
				if (value != _playerTownBuildInfo)
				{
					_playerTownBuildInfo = value;
					((ViewModel)this).OnPropertyChangedWithValue<PlayerSettlementBuildVM>(value, "PlayerTownBuildInfo");
				}
			}
		}

		[DataSourceProperty]
		public PlayerSettlementBuildVM? PlayerVillageBuildInfo
		{
			get
			{
				return _playerVillageBuildInfo;
			}
			set
			{
				if (value != _playerVillageBuildInfo)
				{
					_playerVillageBuildInfo = value;
					((ViewModel)this).OnPropertyChangedWithValue<PlayerSettlementBuildVM>(value, "PlayerVillageBuildInfo");
				}
			}
		}

		[DataSourceProperty]
		public PlayerSettlementBuildVM? PlayerCastleBuildInfo
		{
			get
			{
				return _playerCastleBuildInfo;
			}
			set
			{
				if (value != _playerCastleBuildInfo)
				{
					_playerCastleBuildInfo = value;
					((ViewModel)this).OnPropertyChangedWithValue<PlayerSettlementBuildVM>(value, "PlayerCastleBuildInfo");
				}
			}
		}

		[DataSourceProperty]
		public bool IsOverallAllowed
		{
			get
			{
				PlayerSettlementBuildVM? playerCastleBuildInfo = PlayerCastleBuildInfo;
				int result;
				if (playerCastleBuildInfo == null || !playerCastleBuildInfo.IsCreatePlayerSettlementAllowed)
				{
					PlayerSettlementBuildVM? playerTownBuildInfo = PlayerTownBuildInfo;
					if (playerTownBuildInfo == null || !playerTownBuildInfo.IsCreatePlayerSettlementAllowed)
					{
						result = ((PlayerVillageBuildInfo?.IsCreatePlayerSettlementAllowed ?? false) ? 1 : 0);
						goto IL_003e;
					}
				}
				result = 1;
				goto IL_003e;
				IL_003e:
				return (byte)result != 0;
			}
		}

		[DataSourceProperty]
		public bool IsOverallVisible
		{
			get
			{
				int result;
				if (Main.Settings != null && !Main.Settings.ImmersiveMode && !forceHide)
				{
					PlayerSettlementBuildVM? playerCastleBuildInfo = PlayerCastleBuildInfo;
					if (playerCastleBuildInfo == null || !playerCastleBuildInfo.IsCreatePlayerSettlementVisible)
					{
						PlayerSettlementBuildVM? playerTownBuildInfo = PlayerTownBuildInfo;
						if (playerTownBuildInfo == null || !playerTownBuildInfo.IsCreatePlayerSettlementVisible)
						{
							result = ((PlayerVillageBuildInfo?.IsCreatePlayerSettlementVisible ?? false) ? 1 : 0);
							goto IL_005c;
						}
					}
					result = 1;
				}
				else
				{
					result = 0;
				}
				goto IL_005c;
				IL_005c:
				return (byte)result != 0;
			}
		}

		[DataSourceProperty]
		public HintViewModel? DisableHint
		{
			get
			{
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Expected O, but got Unknown
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Expected O, but got Unknown
				if (!IsOverallAllowed)
				{
					TextObject val = new TextObject("{=player_settlement_h_07}Cannot build town: {TOWN_REASON}\r\n\r\nCannot build castle: {CASTLE_REASON}\r\n\r\nCannot build village: {VILLAGE_REASON}", (Dictionary<string, object>)null);
					val.SetTextVariable("CASTLE_REASON", ((object)_playerCastleBuildInfo?.DisableHint?.HintText)?.ToString() ?? " - ");
					val.SetTextVariable("TOWN_REASON", ((object)_playerTownBuildInfo?.DisableHint?.HintText)?.ToString() ?? " - ");
					val.SetTextVariable("VILLAGE_REASON", ((object)_playerVillageBuildInfo?.DisableHint?.HintText)?.ToString() ?? " - ");
					_disableReasonHint = new HintViewModel(val, (string)null);
					return _disableReasonHint;
				}
				_disableReasonHint = null;
				return _disableReasonHint;
			}
		}

		public PlayerSettlementInfoVM()
		{
		}

		public PlayerSettlementInfoVM(MapBarExtensionVM mapBarExtensionVM, MapBarVM mapBarVM)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			this.mapBarExtensionVM = mapBarExtensionVM;
			this.mapBarVM = mapBarVM;
			_playerTownBuildInfo = new PlayerSettlementBuildVM(this, SettlementType.Town);
			_playerVillageBuildInfo = new PlayerSettlementBuildVM(this, SettlementType.Village);
			_playerCastleBuildInfo = new PlayerSettlementBuildVM(this, SettlementType.Castle);
			((MapInfoVM)this).ExtendHint = new HintViewModel(new TextObject("{=player_settlement_h_12}Show/Hide player settlement build options", (Dictionary<string, object>)null), (string)null);
			CalculateEnabled();
		}

		public void Tick(float dt)
		{
			MapBarVM? obj = mapBarVM;
			if (((obj != null) ? obj.MapTimeControl : null) != null)
			{
				bool flag = !mapBarVM.MapTimeControl.IsCenterPanelEnabled || Settlement.CurrentSettlement != null;
				if (forceHide != flag)
				{
					((ViewModel)this).RefreshValues();
					forceHide = flag;
					((ViewModel)this).OnPropertyChangedWithValue(IsOverallVisible, "IsOverallVisible");
				}
			}
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			CalculateEnabled();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			_playerCastleBuildInfo = null;
			_playerTownBuildInfo = null;
			_playerVillageBuildInfo = null;
		}

		private void CalculateEnabled()
		{
			_playerTownBuildInfo?.CalculateEnabled();
			_playerCastleBuildInfo?.CalculateEnabled();
			_playerVillageBuildInfo?.CalculateEnabled();
			((ViewModel)this).OnPropertyChangedWithValue(IsOverallVisible, "IsOverallVisible");
			((ViewModel)this).OnPropertyChangedWithValue<HintViewModel>(DisableHint, "DisableHint");
			((ViewModel)this).OnPropertyChangedWithValue(IsOverallAllowed, "IsOverallAllowed");
			if (!IsOverallAllowed)
			{
				((MapInfoVM)this).IsInfoBarExtended = false;
			}
		}
	}
}
