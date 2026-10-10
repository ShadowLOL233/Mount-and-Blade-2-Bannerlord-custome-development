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


namespace BannerlordPlayerSettlement.Behaviours
{
	public class PlayerSettlementBehaviour : CampaignBehaviorBase
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class __c
		{
			public static readonly __c __9 = new __c();

			public static Func<SettlementMetaV3, string> __9__76_0;

			public static OnConditionDelegate __9__79_0;

			public static OnConsequenceDelegate __9__79_1;

			public static OnClickableConditionDelegate __9__79_2;

			public static Func<DeepTransformEdit, bool> __9__104_2;

			public static Func<DeepTransformEdit, bool> __9__104_0;

			public static Func<DeepTransformEdit, bool> __9__111_0;

			public static Func<CultureObject, InquiryElement> __9__122_3;

			public static Func<XmlNode, bool> __9__123_9;

			public static Func<CultureObject, InquiryElement> __9__123_3;

			public static Func<CampaignBehaviorBase, bool> __9__124_6;

			public static Func<CultureObject, InquiryElement> __9__124_4;

			public static Func<Building, int> __9__126_3;

			public static Func<Building, bool> __9__126_0;

			public static Func<Building, bool> __9__126_1;

			public static Func<Settlement, bool> __9__128_0;

			public static Func<Settlement, InquiryElement> __9__128_1;

			public static Func<CampaignBehaviorBase, bool> __9__129_6;

			public static Func<CampaignBehaviorBase, bool> __9__129_7;

			public static Func<CultureObject, InquiryElement> __9__129_4;

			public static Func<CampaignBehaviorBase, bool> __9__136_8;

			public static Func<Hero, bool> __9__136_9;

			public static Func<CampaignBehaviorBase, bool> __9__136_6;

			public static Func<CampaignBehaviorBase, bool> __9__136_7;

			public static Func<CultureObject, InquiryElement> __9__136_4;

			public static Func<Building, int> __9__140_3;

			public static Func<Building, bool> __9__140_0;

			public static Func<Building, bool> __9__140_1;

			public static Func<PlayerSettlementItemTemplate, int, string> __9__144_0;

			public static Action<bool, string> __9__144_4;

			public static Action __9__144_3;

			public static Func<VillageType, bool> __9__147_0;

			public static Func<Settlement, bool> __9__150_0;

			public static Func<Settlement, bool> __9__153_0;

			internal string _SyncData_b__76_0(SettlementMetaV3 o)
			{
				return o.StringId;
			}

			internal bool _SetupConversationDialogues_b__79_0()
			{
				return conv_rebuild_condition();
			}

			internal void _SetupConversationDialogues_b__79_1()
			{
				conv_rebuild_consequence();
			}

			internal bool _SetupConversationDialogues_b__79_2(out TextObject t)
			{
				return conv_rebuild_clickable(out t);
			}

			internal bool _OnBeforeTick_b__104_2(DeepTransformEdit d)
			{
				return d.IsDeleted && d.Index >= 0;
			}

			internal bool _OnBeforeTick_b__104_0(DeepTransformEdit dte)
			{
				return dte.Index < 0;
			}

			internal bool _HideDeletedDeepEdits_b__111_0(DeepTransformEdit dte)
			{
				return dte.IsDeleted && dte.Index >= 0;
			}

			internal InquiryElement _Overwrite_b__122_3(CultureObject c)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Expected O, but got Unknown
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Expected O, but got Unknown
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Expected O, but got Unknown
				return new InquiryElement((object)c, ((object)((BasicCultureObject)c).Name).ToString(), (ImageIdentifier)new BannerImageIdentifier(new Banner(((BasicCultureObject)c).Banner), false), true, ((object)((BasicCultureObject)c).Name).ToString());
			}

			internal bool _Rebuild_b__123_9(XmlNode n)
			{
				return !(n is XmlComment);
			}

			internal InquiryElement _Rebuild_b__123_3(CultureObject c)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Expected O, but got Unknown
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Expected O, but got Unknown
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Expected O, but got Unknown
				return new InquiryElement((object)c, ((object)((BasicCultureObject)c).Name).ToString(), (ImageIdentifier)new BannerImageIdentifier(new Banner(((BasicCultureObject)c).Banner), false), true, ((object)((BasicCultureObject)c).Name).ToString());
			}

			internal bool _BuildCastle_b__124_6(CampaignBehaviorBase b)
			{
				return b is RecruitmentCampaignBehavior;
			}

			internal InquiryElement _BuildCastle_b__124_4(CultureObject c)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Expected O, but got Unknown
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Expected O, but got Unknown
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Expected O, but got Unknown
				return new InquiryElement((object)c, ((object)((BasicCultureObject)c).Name).ToString(), (ImageIdentifier)new BannerImageIdentifier(new Banner(((BasicCultureObject)c).Banner), false), true, ((object)((BasicCultureObject)c).Name).ToString());
			}

			internal int _InitCastleBuildings_b__126_3(Building k)
			{
				return k.CurrentLevel;
			}

			internal bool _InitCastleBuildings_b__126_0(Building b)
			{
				return b.BuildingType.IsDailyProject && b.IsCurrentlyDefault;
			}

			internal bool _InitCastleBuildings_b__126_1(Building b)
			{
				return b.BuildingType.IsDailyProject;
			}

			internal bool _BuildVillage_b__128_0(Settlement s)
			{
				return s != null;
			}

			internal InquiryElement _BuildVillage_b__128_1(Settlement c)
			{
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Expected O, but got Unknown
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Expected O, but got Unknown
				return new InquiryElement((object)c, ((object)c.Name).ToString(), (ImageIdentifier)new CharacterImageIdentifier(CharacterCode.CreateFrom((BasicCharacterObject)(object)((c.IsTown || c.IsCastle) ? (c.Town.Governor ?? Hero.MainHero) : Hero.MainHero).CharacterObject)), true, ((object)(c.EncyclopediaText ?? c.Name)).ToString());
			}

			internal bool _BuildVillageFor_b__129_6(CampaignBehaviorBase b)
			{
				return b is VillageGoodProductionCampaignBehavior;
			}

			internal bool _BuildVillageFor_b__129_7(CampaignBehaviorBase b)
			{
				return b is RecruitmentCampaignBehavior;
			}

			internal InquiryElement _BuildVillageFor_b__129_4(CultureObject c)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Expected O, but got Unknown
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Expected O, but got Unknown
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Expected O, but got Unknown
				return new InquiryElement((object)c, ((object)((BasicCultureObject)c).Name).ToString(), (ImageIdentifier)new BannerImageIdentifier(new Banner(((BasicCultureObject)c).Banner), false), true, ((object)((BasicCultureObject)c).Name).ToString());
			}

			internal bool _BuildTown_b__136_8(CampaignBehaviorBase b)
			{
				return b is WorkshopsCampaignBehavior;
			}

			internal bool _BuildTown_b__136_9(Hero x)
			{
				return x.IsGangLeader;
			}

			internal bool _BuildTown_b__136_6(CampaignBehaviorBase b)
			{
				return b is CraftingCampaignBehavior;
			}

			internal bool _BuildTown_b__136_7(CampaignBehaviorBase b)
			{
				return b is RecruitmentCampaignBehavior;
			}

			internal InquiryElement _BuildTown_b__136_4(CultureObject c)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Expected O, but got Unknown
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Expected O, but got Unknown
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Expected O, but got Unknown
				return new InquiryElement((object)c, ((object)((BasicCultureObject)c).Name).ToString(), (ImageIdentifier)new BannerImageIdentifier(new Banner(((BasicCultureObject)c).Banner), false), true, ((object)((BasicCultureObject)c).Name).ToString());
			}

			internal int _InitTownBuildings_b__140_3(Building k)
			{
				return k.CurrentLevel;
			}

			internal bool _InitTownBuildings_b__140_0(Building b)
			{
				return b.BuildingType.IsDailyProject && b.IsCurrentlyDefault;
			}

			internal bool _InitTownBuildings_b__140_1(Building b)
			{
				return b.BuildingType.IsDailyProject;
			}

			internal string _UpdateSettlementVisualEntity_b__144_0(PlayerSettlementItemTemplate a, int idx)
			{
				return $"\t\t{a.Id} - Culture: '{a.Culture}', Type: '{a.Type}', Variant: '{idx + 1}'";
			}

			internal void _UpdateSettlementVisualEntity_b__144_4(bool b, string s)
			{
				Utilities.QuitGame();
			}

			internal void _UpdateSettlementVisualEntity_b__144_3()
			{
				InformationManager.HideInquiry();
				if (!CampaignOptions.IsIronmanMode)
				{
					SaveHandler.SaveLoad(SaveHandler.SaveMechanism.Auto);
				}
				else
				{
					Utilities.QuitGame();
				}
			}

			internal bool _GetVillageTypeInquiry_b__147_0(VillageType v)
			{
				return v != null;
			}

			internal bool _GetPotentialVillageBoundOwners_b__150_0(Settlement s)
			{
				return (s.IsTown && (((List<Village>)(object)s.BoundVillages)?.Count ?? 0) < Main.Settings.MaxVillagesPerTown) || (s.IsCastle && (((List<Village>)(object)s.BoundVillages)?.Count ?? 0) < Main.Settings.MaxVillagesPerCastle);
			}

			internal bool _CalculateVillageOwner_b__153_0(Settlement s)
			{
				return (s.IsTown && (((List<Village>)(object)s.BoundVillages)?.Count ?? 0) < Main.Settings.MaxVillagesPerTown) || (s.IsCastle && (((List<Village>)(object)s.BoundVillages)?.Count ?? 0) < Main.Settings.MaxVillagesPerCastle);
			}
		}

		private static FastInvokeHandler FillGarrisonPartyOnNewGameInvoker = MethodInvoker.GetHandler(AccessTools.Method(typeof(GarrisonTroopsCampaignBehavior), "FillGarrisonPartyOnNewGame"));

		public const string PlayerSettlementUnderConstructionMenu = "player_settlement_construction";

		public static PlayerSettlementBehaviour? Instance = null;

		public static bool OldSaveLoaded = false;

		public static bool TriggerSaveAfterUpgrade = false;

		public static bool TriggerSaveLoadAfterUpgrade = false;

		public SettlementType SettlementRequest = SettlementType.None;

		public PlayerSettlementItem? ReSettlementRequest = null;

		public Settlement? OverwriteRequest = null;

		private PlayerSettlementInfo _playerSettlementInfo = new PlayerSettlementInfo();

		private MetaV3? _metaV3 = null;

		private readonly MbEvent<Settlement> _settlementCreated = new MbEvent<Settlement>();

		private readonly MbEvent<Settlement> _settlementBuildComplete = new MbEvent<Settlement>();

		private readonly MbEvent<Settlement> _settlementRebuild = new MbEvent<Settlement>();

		private readonly MbEvent<Settlement> _settlementOverwrite = new MbEvent<Settlement>();

		private readonly MbEvent _onReset = new MbEvent();

		private const string GhostSettlementEntityId = "player_settlement_ghost";

		private List<PlayerSettlementItemTemplate> availableModels = new List<PlayerSettlementItemTemplate>();

		private int currentModelOptionIdx = -1;

		private GameEntity? settlementVisualEntity = null;

		private MatrixFrame? settlementPlacementFrame = null;

		private Action? applyPending = null;

		private float holdTime = 0f;

		private string? settlementVisualPrefab = null;

		private bool deepEdit = false;

		private GameEntity? currentDeepTarget;

		private float deepEditScale = 1f;

		private string? deepEditPrefab = null;

		private List<DeepTransformEdit> deepTransformEdits = new List<DeepTransformEdit>();

		private List<GameEntity> settlementVisualEntityChildren = new List<GameEntity>();

		private const string GhostGateEntityId = "player_settlement_ghost_gate";

		private const string GhostGatePrefabId = "player_settlement_ghost_gate";

		private const string GhostPortEntityId = "player_settlement_ghost_gate";

		private const string GhostPortPrefabId = "player_settlement_ghost_gate";

		private GameEntity? ghostGateVisualEntity = null;

		private MatrixFrame? gatePlacementFrame = null;

		private GameEntity? ghostPortVisualEntity = null;

		private MatrixFrame? portPlacementFrame = null;

		private bool gateSupported = false;

		private bool portSupported = false;

		private static Exception? previousVisualUpdateException = null;

		private static readonly FastInvokeHandler SetUniqueGameId = MethodInvoker.GetHandler(AccessTools.Property(typeof(Campaign), "UniqueGameId").SetMethod);

		public MetaV3? MetaV3 => _metaV3;

		public static IMbEvent<Settlement>? SettlementCreatedEvent => (IMbEvent<Settlement>?)(object)Instance?._settlementCreated;

		public static IMbEvent<Settlement>? SettlementBuildCompleteEvent => (IMbEvent<Settlement>?)(object)Instance?._settlementBuildComplete;

		public static IMbEvent<Settlement>? SettlementRebuildEvent => (IMbEvent<Settlement>?)(object)Instance?._settlementRebuild;

		public static IMbEvent<Settlement>? SettlementOverwriteEvent => (IMbEvent<Settlement>?)(object)Instance?._settlementOverwrite;

		public static IMbEvent? OnResetEvent => (IMbEvent?)(object)Instance?._onReset;

		private bool HasLoaded { get; set; }

		public bool ReachedMax
		{
			get
			{
				if (PlayerSettlementInfo.Instance == null || Main.Settings == null)
				{
					return true;
				}
				return PlayerSettlementInfo.Instance.Towns.Count >= Main.Settings.MaxTowns && PlayerSettlementInfo.Instance.Castles.Count >= Main.Settings.MaxCastles && PlayerSettlementInfo.Instance.TotalVillages >= int.MaxValue;
			}
		}

		public bool HasRequest => SettlementRequest != 0 || ReSettlementRequest != null || OverwriteRequest != null;

		public bool IsPlacingSettlement => settlementVisualEntity != (GameEntity)null && applyPending != null;

		public bool IsDeepEdit => deepEdit && currentDeepTarget != (GameEntity)null && IsPlacingSettlement && !IsPlacingGate && !IsPlacingPort;

		public bool IsPlacingGate => ghostGateVisualEntity != (GameEntity)null && applyPending != null;

		public bool IsPlacingPort => ghostPortVisualEntity != (GameEntity)null && applyPending != null;

		public bool PlacementSupported { get; set; }

		public PlayerSettlementBehaviour()
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Expected O, but got Unknown
			Instance = this;
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnSessionLaunched);
			CampaignEvents.TickEvent.AddNonSerializedListener((object)this, (Action<float>)Tick);
			CampaignEvents.DailyTickEvent.AddNonSerializedListener((object)this, (Action)DailyTick);
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnNewGameCreated);
			CampaignEvents.OnGameEarlyLoadedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnGameEarlyLoaded);
			CampaignEvents.SettlementEntered.AddNonSerializedListener((object)this, (Action<MobileParty, Settlement, Hero>)SettlementEntered);
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener((object)this, (Action<MobileParty, Settlement>)SettlementLeft);
		}

		private void SettlementLeft(MobileParty arg1, Settlement arg2)
		{
			if (arg1 == MobileParty.MainParty)
			{
				MapBarExtensionVM.Current?.Tick(0f);
			}
		}

		private void SettlementEntered(MobileParty arg1, Settlement arg2, Hero arg3)
		{
			if (arg1 == MobileParty.MainParty || arg3 == Hero.MainHero)
			{
				MapBarExtensionVM.Current?.Tick(0f);
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
			try
			{
				if (dataStore.IsSaving)
				{
					_playerSettlementInfo = PlayerSettlementInfo.Instance ?? new PlayerSettlementInfo();
					_metaV3 = BannerlordPlayerSettlement.Saves.MetaV3.Create(_playerSettlementInfo);
				}
				dataStore.SyncData<PlayerSettlementInfo>("PlayerSettlement_PlayerSettlementInfo", ref _playerSettlementInfo);
				dataStore.SyncData<MetaV3>("PlayerSettlement_MetaV3", ref _metaV3);
				if (_playerSettlementInfo == null)
				{
					_playerSettlementInfo = new PlayerSettlementInfo();
				}
				if (dataStore.IsSaving)
				{
					return;
				}
				if (_playerSettlementInfo.OverwriteSettlements != null)
				{
					List<OverwriteSettlementItem> list = _playerSettlementInfo.OverwriteSettlements.Distinct().ToList();
					if (list.Count != _playerSettlementInfo.OverwriteSettlements.Count)
					{
						LogManager.Log.ToFile($"Duplicate overwrites found! {list.Count}/{_playerSettlementInfo.OverwriteSettlements.Count} are unique.");
						_playerSettlementInfo.OverwriteSettlements = list;
					}
				}
				if (_metaV3.OverwriteSettlements != null)
				{
					List<SettlementMetaV3> list2 = Extensions.DistinctBy<SettlementMetaV3, string>((IEnumerable<SettlementMetaV3>)_metaV3.OverwriteSettlements, (Func<SettlementMetaV3, string>)((SettlementMetaV3 o) => o.StringId)).ToList();
					if (list2.Count != _metaV3.OverwriteSettlements.Count)
					{
						LogManager.Log.ToFile($"Duplicate overwrites found! {list2.Count}/{_metaV3.OverwriteSettlements.Count} are unique.");
						_metaV3.OverwriteSettlements = list2;
					}
				}
				OnLoad();
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
		}

		public void LoadEarlySync(IDataStore? dataStore)
		{
			try
			{
				if (dataStore != null && !dataStore.IsSaving)
				{
					dataStore.SyncData<MetaV3>("PlayerSettlement_MetaV3", ref _metaV3);
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
		}

		private void OnSessionLaunched(CampaignGameStarter starter)
		{
			SetupGameMenus(starter);
			SetupConversationDialogues(starter);
		}

		private void SetupConversationDialogues(CampaignGameStarter starter)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			//IL_0037: Expected O, but got Unknown
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Expected O, but got Unknown
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Expected O, but got Unknown
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Expected O, but got Unknown
			//IL_0114: Expected O, but got Unknown
			//IL_0114: Expected O, but got Unknown
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Expected O, but got Unknown
			//IL_0156: Expected O, but got Unknown
			//IL_0156: Expected O, but got Unknown
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Expected O, but got Unknown
			//IL_0198: Expected O, but got Unknown
			//IL_0198: Expected O, but got Unknown
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Expected O, but got Unknown
			starter.AddPlayerLine("player_settlement_build_options_intro", "hero_main_options", "player_settlement_build_options_response", "{=player_settlement_25}I would like to review my options for building a settlement.", new OnConditionDelegate(conv_build_start_condition), (OnConsequenceDelegate)null, 100, new OnClickableConditionDelegate(conv_build_start_clickable), (OnPersuasionOptionDelegate)null);
			starter.AddDialogLine("player_settlement_build_options_response_dialogue", "player_settlement_build_options_response", "player_settlement_build_options_choices", "{=k7ebznzr}Yes?", (OnConditionDelegate)null, (OnConsequenceDelegate)null, 100, (OnClickableConditionDelegate)null);
			object obj = __c.__9__79_0;
			if (obj == null)
			{
				OnConditionDelegate val = () => conv_rebuild_condition();
				__c.__9__79_0 = val;
				obj = (object)val;
			}
			object obj2 = __c.__9__79_1;
			if (obj2 == null)
			{
				OnConsequenceDelegate val2 = delegate
				{
					conv_rebuild_consequence();
				};
				__c.__9__79_1 = val2;
				obj2 = (object)val2;
			}
			object obj3 = __c.__9__79_2;
			if (obj3 == null)
			{
				OnClickableConditionDelegate val3 = delegate(out TextObject t)
				{
					return conv_rebuild_clickable(out t);
				};
				__c.__9__79_2 = val3;
				obj3 = (object)val3;
			}
			starter.AddPlayerLine("player_settlement_rebuild_settlement", "player_settlement_build_options_choices", "close_window", "{=player_settlement_40}We should rebuild this settlement.", (OnConditionDelegate)obj, (OnConsequenceDelegate)obj2, 100, (OnClickableConditionDelegate)obj3, (OnPersuasionOptionDelegate)null);
			starter.AddPlayerLine("player_settlement_build_town", "player_settlement_build_options_choices", "close_window", "{=player_settlement_26}We should build a town.", new OnConditionDelegate(conv_build_town_condition), (OnConsequenceDelegate)delegate
			{
				conv_build_consequence(SettlementType.Town);
			}, 100, new OnClickableConditionDelegate(conv_build_town_clickable), (OnPersuasionOptionDelegate)null);
			starter.AddPlayerLine("player_settlement_build_village", "player_settlement_build_options_choices", "close_window", "{=player_settlement_27}We should build a village.", new OnConditionDelegate(conv_build_village_condition), (OnConsequenceDelegate)delegate
			{
				conv_build_consequence(SettlementType.Village);
			}, 100, new OnClickableConditionDelegate(conv_build_village_clickable), (OnPersuasionOptionDelegate)null);
			starter.AddPlayerLine("player_settlement_build_castle", "player_settlement_build_options_choices", "close_window", "{=player_settlement_28}We should build a castle.", new OnConditionDelegate(conv_build_castle_condition), (OnConsequenceDelegate)delegate
			{
				conv_build_consequence(SettlementType.Castle);
			}, 100, new OnClickableConditionDelegate(conv_build_castle_clickable), (OnPersuasionOptionDelegate)null);
			starter.AddPlayerLine("player_settlement_build_nothing", "player_settlement_build_options_choices", "close_window", "{=player_settlement_29}Nevermind.", (OnConditionDelegate)null, (OnConsequenceDelegate)null, 100, (OnClickableConditionDelegate)null, (OnPersuasionOptionDelegate)null);
		}

		private static void conv_rebuild_consequence()
		{
			PlayerSettlementItem playerSettlementItem = null;
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.Owner == Hero.MainHero && Instance != null)
			{
				Settlement.CurrentSettlement.IsPlayerBuilt(out playerSettlementItem);
				Instance.ReSettlementRequest = playerSettlementItem;
				if (playerSettlementItem == null)
				{
					Instance.OverwriteRequest = Settlement.CurrentSettlement;
				}
				((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
				Mission current = Mission.Current;
				if (current != null)
				{
					current.EndMission();
				}
				if (PlayerEncounter.Current != null)
				{
					PlayerEncounter.LeaveEncounter = true;
				}
			}
		}

		private static bool conv_rebuild_clickable(out TextObject explanation, bool noConversation = false)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Expected O, but got Unknown
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			explanation = new TextObject("", (Dictionary<string, object>)null);
			if (!noConversation && (Hero.OneToOneConversationHero == null || Hero.OneToOneConversationHero.Clan != Clan.PlayerClan))
			{
				return false;
			}
			if (Main.Settings == null || PlayerSettlementInfo.Instance == null || Instance == null)
			{
				return false;
			}
			if (!noConversation && Main.Settings.NoDialogue)
			{
				return false;
			}
			PlayerSettlementItem playerSettlementItem = null;
			if (Settlement.CurrentSettlement == null || Settlement.CurrentSettlement.Owner != Hero.MainHero)
			{
				return false;
			}
			Settlement.CurrentSettlement.IsPlayerBuilt(out playerSettlementItem);
			if (!Settlement.CurrentSettlement.IsVillage && !noConversation)
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				object obj;
				if (currentSettlement == null)
				{
					obj = null;
				}
				else
				{
					Town town = currentSettlement.Town;
					obj = ((town != null) ? town.Governor : null);
				}
				if (obj != Hero.OneToOneConversationHero)
				{
					return false;
				}
			}
			bool flag = false;
			switch (playerSettlementItem?.GetSettlementType() ?? Settlement.CurrentSettlement?.GetSettlementType() ?? SettlementType.None)
			{
			default:
				flag = false;
				break;
			case SettlementType.Town:
			{
				flag = !Main.Settings.RequireGold || Hero.MainHero.Clan.Gold >= Main.Settings.RebuildTownRequiredGold;
				explanation = (TextObject)(flag ? ((object)explanation) : ((object)new TextObject("{=player_settlement_h_05}Not enough funds ({CURRENT_FUNDS}/{REQUIRED_FUNDS})", (Dictionary<string, object>)null)));
				TextObject obj4 = explanation;
				Hero mainHero3 = Hero.MainHero;
				obj4.SetTextVariable("CURRENT_FUNDS", (mainHero3 != null) ? mainHero3.Gold : 0);
				explanation.SetTextVariable("REQUIRED_FUNDS", Main.Settings.RebuildTownRequiredGold);
				break;
			}
			case SettlementType.Village:
			{
				flag = !Main.Settings.RequireVillageGold || Hero.MainHero.Clan.Gold >= Main.Settings.RebuildVillageRequiredGold;
				explanation = (TextObject)(flag ? ((object)explanation) : ((object)new TextObject("{=player_settlement_h_05}Not enough funds ({CURRENT_FUNDS}/{REQUIRED_FUNDS})", (Dictionary<string, object>)null)));
				TextObject obj3 = explanation;
				Hero mainHero2 = Hero.MainHero;
				obj3.SetTextVariable("CURRENT_FUNDS", (mainHero2 != null) ? mainHero2.Gold : 0);
				explanation.SetTextVariable("REQUIRED_FUNDS", Main.Settings.RebuildVillageRequiredGold);
				break;
			}
			case SettlementType.Castle:
			{
				flag = !Main.Settings.RequireCastleGold || Hero.MainHero.Clan.Gold >= Main.Settings.RebuildCastleRequiredGold;
				explanation = (TextObject)(flag ? ((object)explanation) : ((object)new TextObject("{=player_settlement_h_05}Not enough funds ({CURRENT_FUNDS}/{REQUIRED_FUNDS})", (Dictionary<string, object>)null)));
				TextObject obj2 = explanation;
				Hero mainHero = Hero.MainHero;
				obj2.SetTextVariable("CURRENT_FUNDS", (mainHero != null) ? mainHero.Gold : 0);
				explanation.SetTextVariable("REQUIRED_FUNDS", Main.Settings.RebuildCastleRequiredGold);
				break;
			}
			}
			return flag;
		}

		private static bool conv_rebuild_condition(bool noConversation = false)
		{
			if (!noConversation && (Hero.OneToOneConversationHero == null || Hero.OneToOneConversationHero.Clan != Clan.PlayerClan))
			{
				return false;
			}
			if (Main.Settings == null || PlayerSettlementInfo.Instance == null || Instance == null || Instance.HasRequest)
			{
				return false;
			}
			if (!noConversation && Main.Settings.NoDialogue)
			{
				return false;
			}
			PlayerSettlementItem playerSettlementItem = null;
			if (Settlement.CurrentSettlement == null || Settlement.CurrentSettlement.Owner != Hero.MainHero)
			{
				return false;
			}
			Settlement.CurrentSettlement.IsPlayerBuilt(out playerSettlementItem);
			if (!Settlement.CurrentSettlement.IsVillage && !noConversation)
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				object obj;
				if (currentSettlement == null)
				{
					obj = null;
				}
				else
				{
					Town town = currentSettlement.Town;
					obj = ((town != null) ? town.Governor : null);
				}
				if (obj != Hero.OneToOneConversationHero)
				{
					return false;
				}
			}
			if (Main.Settings.Enabled && !Main.Settings.HideButtonUntilReady)
			{
				return true;
			}
			SettlementType settlementType = playerSettlementItem?.GetSettlementType() ?? Settlement.CurrentSettlement?.GetSettlementType() ?? SettlementType.None;
			if (1 == 0)
			{
			}
			bool result = settlementType switch
			{
				SettlementType.Town => !Main.Settings.RequireGold || Hero.MainHero.Clan.Gold >= Main.Settings.RebuildTownRequiredGold, 
				SettlementType.Village => !Main.Settings.RequireVillageGold || Hero.MainHero.Clan.Gold >= Main.Settings.RebuildVillageRequiredGold, 
				SettlementType.Castle => !Main.Settings.RequireCastleGold || Hero.MainHero.Clan.Gold >= Main.Settings.RebuildCastleRequiredGold, 
				_ => false, 
			};
			if (1 == 0)
			{
			}
			return result;
		}

		private bool conv_build_castle_clickable(out TextObject? explanation)
		{
			((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
			explanation = MapBarExtensionVM.Current?.PlayerSettlementInfo?.PlayerCastleBuildInfo?.DisableHint?.HintText ?? null;
			return (MapBarExtensionVM.Current?.PlayerSettlementInfo?.PlayerCastleBuildInfo?.IsCreatePlayerSettlementAllowed).GetValueOrDefault();
		}

		private bool conv_build_castle_condition()
		{
			((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
			return (Main.Settings != null && !Main.Settings.HideButtonUntilReady) || (MapBarExtensionVM.Current?.PlayerSettlementInfo?.PlayerCastleBuildInfo?.IsCreatePlayerSettlementAllowed).GetValueOrDefault();
		}

		private bool conv_build_village_clickable(out TextObject? explanation)
		{
			((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
			explanation = MapBarExtensionVM.Current?.PlayerSettlementInfo?.PlayerVillageBuildInfo?.DisableHint?.HintText ?? null;
			return (MapBarExtensionVM.Current?.PlayerSettlementInfo?.PlayerVillageBuildInfo?.IsCreatePlayerSettlementAllowed).GetValueOrDefault();
		}

		private bool conv_build_village_condition()
		{
			((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
			return (Main.Settings != null && !Main.Settings.HideButtonUntilReady) || (MapBarExtensionVM.Current?.PlayerSettlementInfo?.PlayerVillageBuildInfo?.IsCreatePlayerSettlementAllowed).GetValueOrDefault();
		}

		private bool conv_build_town_clickable(out TextObject? explanation)
		{
			((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
			explanation = MapBarExtensionVM.Current?.PlayerSettlementInfo?.PlayerTownBuildInfo?.DisableHint?.HintText ?? null;
			return (MapBarExtensionVM.Current?.PlayerSettlementInfo?.PlayerTownBuildInfo?.IsCreatePlayerSettlementAllowed).GetValueOrDefault();
		}

		private bool conv_build_town_condition()
		{
			((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
			return (Main.Settings != null && !Main.Settings.HideButtonUntilReady) || (MapBarExtensionVM.Current?.PlayerSettlementInfo?.PlayerTownBuildInfo?.IsCreatePlayerSettlementAllowed).GetValueOrDefault();
		}

		private bool conv_build_start_clickable(out TextObject? explanation)
		{
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Expected O, but got Unknown
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Expected O, but got Unknown
			((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
			explanation = MapBarExtensionVM.Current?.PlayerSettlementInfo?.DisableHint?.HintText ?? null;
			TextObject explanation2;
			bool flag = conv_rebuild_clickable(out explanation2);
			if (!flag && explanation2 != (TextObject)null && !string.IsNullOrEmpty(((object)explanation2).ToString()))
			{
				TextObject val = new TextObject("{=player_settlement_h_13}Cannot rebuild settlement: {REASON}", (Dictionary<string, object>)null);
				val.SetTextVariable("REASON", explanation2);
				if (explanation == (TextObject)null)
				{
					explanation = val;
				}
				else
				{
					explanation = new TextObject(((object)explanation).ToString() + "\r\n\r\n" + ((object)val).ToString(), (Dictionary<string, object>)null);
				}
			}
			return flag || (MapBarExtensionVM.Current?.PlayerSettlementInfo?.IsOverallAllowed).GetValueOrDefault();
		}

		private bool conv_build_start_condition()
		{
			if (Hero.OneToOneConversationHero == null || Hero.OneToOneConversationHero.Clan != Clan.PlayerClan)
			{
				return false;
			}
			if (Main.Settings == null || Main.Settings.NoDialogue)
			{
				return false;
			}
			((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
			bool flag = conv_rebuild_condition();
			return Main.Settings != null && Main.Settings.Enabled && !HasRequest && (!Main.Settings.HideButtonUntilReady || flag || (MapBarExtensionVM.Current?.PlayerSettlementInfo?.IsOverallAllowed).GetValueOrDefault());
		}

		private void conv_build_consequence(SettlementType settlementType)
		{
			SettlementRequest = settlementType;
			((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
			Campaign.Current.TimeControlMode = (CampaignTimeControlMode)1;
			Mission current = Mission.Current;
			if (current != null)
			{
				current.EndMission();
			}
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.LeaveEncounter = true;
			}
		}

		public void SetupGameMenus(CampaignGameStarter campaignGameSystemStarter)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			//IL_0053: Expected O, but got Unknown
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Expected O, but got Unknown
			//IL_0085: Expected O, but got Unknown
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Expected O, but got Unknown
			//IL_00b7: Expected O, but got Unknown
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Expected O, but got Unknown
			//IL_00e9: Expected O, but got Unknown
			try
			{
				campaignGameSystemStarter.AddGameMenu("player_settlement_construction", "{=!}{SETTLEMENT_INFO}", new OnInitDelegate(game_menu_town_under_construction_on_init), (MenuOverlayType)3, (MenuFlags)0, (object)null);
				campaignGameSystemStarter.AddGameMenuOption("player_settlement_construction", "town_leave", "{=3sRdGQou}Leave", new OnConditionDelegate(game_menu_town_under_construction_town_leave_on_condition), new OnConsequenceDelegate(game_menu_town_under_construction_settlement_leave_on_consequence), true, -1, false, (object)null);
				campaignGameSystemStarter.AddGameMenuOption("town", "leave_rebuild", "{=player_settlement_41}Rebuild Settlement", new OnConditionDelegate(game_menu_rebuild_condition), new OnConsequenceDelegate(game_menu_rebuild_consequence), false, -1, false, (object)null);
				campaignGameSystemStarter.AddGameMenuOption("castle", "leave_rebuild", "{=player_settlement_41}Rebuild Settlement", new OnConditionDelegate(game_menu_rebuild_condition), new OnConsequenceDelegate(game_menu_rebuild_consequence), false, -1, false, (object)null);
				campaignGameSystemStarter.AddGameMenuOption("village", "leave_rebuild", "{=player_settlement_41}Rebuild Settlement", new OnConditionDelegate(game_menu_rebuild_condition), new OnConsequenceDelegate(game_menu_rebuild_consequence), false, -1, false, (object)null);
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
		}

		private static void game_menu_rebuild_consequence(MenuCallbackArgs args)
		{
			conv_rebuild_consequence();
			try
			{
				PlayerEncounter.LeaveSettlement();
				PlayerEncounter.Finish(true);
				Campaign.Current.TimeControlMode = (CampaignTimeControlMode)2;
			}
			catch (Exception ex)
			{
				LogManager.EventTracer.Trace(new List<string> { ex.Message, ex.StackTrace });
			}
		}

		private static bool game_menu_rebuild_condition(MenuCallbackArgs args)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			args.optionLeaveType = (LeaveType)16;
			MobileParty mainParty = MobileParty.MainParty;
			if (((mainParty != null) ? mainParty.Army : null) != null || Main.Settings == null || Main.Settings.ImmersiveMode)
			{
				return false;
			}
			bool flag = conv_rebuild_condition(noConversation: true);
			if (!flag)
			{
				return flag;
			}
			args.IsEnabled = conv_rebuild_clickable(out var explanation, noConversation: true);
			args.Tooltip = explanation;
			return flag;
		}

		[GameMenuInitializationHandler("player_settlement_construction")]
		public static void game_menu_town_under_construction_menu_enter_sound_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetPanelSound("event:/ui/panels/settlement_city");
			args.MenuContext.SetAmbientSound("event:/map/ambient/node/settlements/2d/city");
			args.MenuContext.SetBackgroundMeshName(Settlement.CurrentSettlement.SettlementComponent.WaitMeshName);
		}

		private static void game_menu_town_under_construction_settlement_leave_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.LeaveSettlement();
			PlayerEncounter.Finish(true);
			Campaign.Current.SaveHandler.SignalAutoSave();
		}

		private static bool game_menu_town_under_construction_town_leave_on_condition(MenuCallbackArgs args)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			args.optionLeaveType = (LeaveType)16;
			if (MobileParty.MainParty.Army == null)
			{
				return true;
			}
			return MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty;
		}

		private static void game_menu_town_under_construction_on_init(MenuCallbackArgs args)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Expected O, but got Unknown
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Expected O, but got Unknown
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Expected O, but got Unknown
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Expected O, but got Unknown
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Expected O, but got Unknown
			Settlement currentSettlement = Settlement.CurrentSettlement;
			TextObject val = ((currentSettlement.OwnerClan != Clan.PlayerClan) ? new TextObject("{=UWzQsHA2}{SETTLEMENT_LINK} is governed by {LORD.LINK}, {FACTION_OFFICIAL} of the {FACTION_TERM}. {PROSPERITY_INFO} {MORALE_INFO}", (Dictionary<string, object>)null) : new TextObject("{=kXVHwjoV}You have arrived at your fief of {SETTLEMENT_LINK}. {PROSPERITY_INFO} {MORALE_INFO}", (Dictionary<string, object>)null));
			HeroHelper.SetPropertiesToTextObject(currentSettlement.OwnerClan.Leader, val, "LORD");
			string text = ((MBObjectBase)currentSettlement.OwnerClan.Leader.MapFaction.Culture).StringId;
			if (currentSettlement.OwnerClan.Leader.IsFemale)
			{
				text += "_f";
			}
			if (currentSettlement.OwnerClan.Leader == Hero.MainHero && !Hero.MainHero.MapFaction.IsKingdomFaction)
			{
				val.SetTextVariable("FACTION_TERM", Hero.MainHero.Clan.EncyclopediaLinkWithName);
				val.SetTextVariable("FACTION_OFFICIAL", new TextObject("{=hb30yQPN}leader", (Dictionary<string, object>)null));
			}
			else
			{
				val.SetTextVariable("FACTION_TERM", currentSettlement.MapFaction.EncyclopediaLinkWithName);
				if (currentSettlement.OwnerClan.MapFaction.IsKingdomFaction && currentSettlement.OwnerClan.Leader == currentSettlement.OwnerClan.Leader.MapFaction.Leader)
				{
					val.SetTextVariable("FACTION_OFFICIAL", GameTexts.FindText("str_faction_ruler", text));
				}
				else
				{
					val.SetTextVariable("FACTION_OFFICIAL", GameTexts.FindText("str_faction_official", text));
				}
			}
			val.SetTextVariable("SETTLEMENT_LINK", currentSettlement.EncyclopediaLinkWithName);
			HeroHelper.SetPropertiesToTextObject(currentSettlement, val, "SETTLEMENT_OBJECT");
			val.SetTextVariable("PROSPERITY_INFO", "\r\n");
			val.SetTextVariable("MORALE_INFO", new TextObject("{=player_settlement_06}This settlement is currently still under construction.", (Dictionary<string, object>)null));
			MBTextManager.SetTextVariable("SETTLEMENT_INFO", val, false);
			Campaign.Current.GameMenuManager.MenuLocations.Clear();
			if (currentSettlement.IsTown)
			{
				args.MenuTitle = new TextObject("{=mVKcvY2U}Town Center", (Dictionary<string, object>)null);
			}
			else if (currentSettlement.IsCastle)
			{
				args.MenuTitle = new TextObject("{=sVXa3zFx}Castle", (Dictionary<string, object>)null);
			}
			else
			{
				args.MenuTitle = new TextObject("{=Ua6CNLBZ}Village", (Dictionary<string, object>)null);
			}
		}

		private void OnNewGameCreated(CampaignGameStarter starter)
		{
			try
			{
				OnLoad();
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
		}

		private void OnGameEarlyLoaded(CampaignGameStarter starter)
		{
			try
			{
				if (!HasLoaded)
				{
					OnLoad();
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
		}

		private void OnLoad()
		{
			if (_playerSettlementInfo == null)
			{
				_playerSettlementInfo = new PlayerSettlementInfo();
			}
			PlayerSettlementInfo.Instance = _playerSettlementInfo;
			PlayerSettlementInfo.Instance.OnLoad();
			HasLoaded = true;
		}

		private void DailyTick()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				LogManager.EventTracer.Trace();
				if (OldSaveLoaded)
				{
					TextObject val = new TextObject("{=player_settlement_08}A player town has been created on a later save. Older saves are not supported and could cause save corruption or town 'ghosting'.", (Dictionary<string, object>)null);
					MBInformationManager.AddQuickInformation(val, 0, (BasicCharacterObject)null, (Equipment)null, "");
					LogManager.Log.NotifyBad(((object)val).ToString());
					OldSaveLoaded = false;
				}
				else if (TriggerSaveLoadAfterUpgrade)
				{
					TriggerSaveLoadAfterUpgrade = false;
					SaveHandler.SaveLoad();
				}
				else if (TriggerSaveAfterUpgrade)
				{
					TriggerSaveAfterUpgrade = false;
					SaveHandler.SaveOnly();
				}
				else
				{
					if (Main.Settings == null || !Main.Settings.Enabled)
					{
						return;
					}
					((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
					if (PlayerSettlementInfo.Instance == null)
					{
						return;
					}
					List<PlayerSettlementItem> list = PlayerSettlementInfo.Instance.PlayerVillages;
					if (list == null)
					{
						list = (PlayerSettlementInfo.Instance.PlayerVillages = new List<PlayerSettlementItem>());
					}
					List<OverwriteSettlementItem> list2 = PlayerSettlementInfo.Instance.OverwriteSettlements;
					if (list2 == null)
					{
						list2 = (PlayerSettlementInfo.Instance.OverwriteSettlements = new List<OverwriteSettlementItem>());
					}
					CampaignTime buildEnd;
					for (int i = 0; i < list.Count; i++)
					{
						PlayerSettlementItem playerSettlementItem = list[i];
						if (!playerSettlementItem.BuildComplete)
						{
							buildEnd = playerSettlementItem.BuildEnd;
							if (!(buildEnd).IsFuture)
							{
								NotifyComplete(playerSettlementItem);
							}
						}
					}
					for (int j = 0; j < list2.Count; j++)
					{
						OverwriteSettlementItem overwriteSettlementItem = list2[j];
						if (!overwriteSettlementItem.BuildComplete)
						{
							buildEnd = overwriteSettlementItem.BuildEnd;
							if (!(buildEnd).IsFuture)
							{
								NotifyComplete(overwriteSettlementItem);
							}
						}
					}
					List<PlayerSettlementItem> list3 = PlayerSettlementInfo.Instance.Towns;
					if (list3 == null)
					{
						list3 = (PlayerSettlementInfo.Instance.Towns = new List<PlayerSettlementItem>());
					}
					for (int k = 0; k < list3.Count; k++)
					{
						PlayerSettlementItem playerSettlementItem2 = list3[k];
						if (!playerSettlementItem2.BuildComplete)
						{
							buildEnd = playerSettlementItem2.BuildEnd;
							if (!(buildEnd).IsFuture)
							{
								NotifyComplete(playerSettlementItem2);
							}
						}
						List<PlayerSettlementItem> list4 = playerSettlementItem2.Villages;
						if (list4 == null)
						{
							list4 = (playerSettlementItem2.Villages = new List<PlayerSettlementItem>());
						}
						for (int l = 0; l < list4.Count; l++)
						{
							PlayerSettlementItem playerSettlementItem3 = list4[l];
							if (!playerSettlementItem3.BuildComplete)
							{
								buildEnd = playerSettlementItem3.BuildEnd;
								if (!(buildEnd).IsFuture)
								{
									NotifyComplete(playerSettlementItem3);
								}
							}
						}
					}
					List<PlayerSettlementItem> list5 = PlayerSettlementInfo.Instance.Castles;
					if (list5 == null)
					{
						list5 = (PlayerSettlementInfo.Instance.Towns = new List<PlayerSettlementItem>());
					}
					for (int m = 0; m < list5.Count; m++)
					{
						PlayerSettlementItem playerSettlementItem4 = list5[m];
						if (!playerSettlementItem4.BuildComplete)
						{
							buildEnd = playerSettlementItem4.BuildEnd;
							if (!(buildEnd).IsFuture)
							{
								NotifyComplete(playerSettlementItem4);
							}
						}
						List<PlayerSettlementItem> list6 = playerSettlementItem4.Villages;
						if (list6 == null)
						{
							list6 = (playerSettlementItem4.Villages = new List<PlayerSettlementItem>());
						}
						for (int n = 0; n < list6.Count; n++)
						{
							PlayerSettlementItem playerSettlementItem5 = list6[n];
							if (!playerSettlementItem5.BuildComplete)
							{
								buildEnd = playerSettlementItem5.BuildEnd;
								if (!(buildEnd).IsFuture)
								{
									NotifyComplete(playerSettlementItem5);
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
		}

		public void NotifyComplete(ISettlementItem item)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			item.SetBuildComplete(completed: true);
			TextObject val = new TextObject("{=player_settlement_07}{TOWN} construction has completed!", (Dictionary<string, object>)null);
			val.SetTextVariable("TOWN", item.GetSettlementName());
			MBInformationManager.AddQuickInformation(val, 0, (BasicCharacterObject)null, (Equipment)null, "");
			LogManager.Log.NotifyGood(((object)val).ToString());
			_settlementBuildComplete.Invoke(item.GetSettlement());
			Campaign.Current.TimeControlMode = (CampaignTimeControlMode)0;
		}

		public void OnBeforeTick(ref InputInformation inputInformation)
		{
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0830: Unknown result type (might be due to invalid IL or missing references)
			//IL_0851: Unknown result type (might be due to invalid IL or missing references)
			//IL_087e: Unknown result type (might be due to invalid IL or missing references)
			//IL_089f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0baa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
			//IL_17c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_17cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_17cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_17d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_17d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_17d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_16c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_16e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0525: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_17e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_17ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_17ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_17f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_1810: Unknown result type (might be due to invalid IL or missing references)
			//IL_1812: Unknown result type (might be due to invalid IL or missing references)
			//IL_1814: Unknown result type (might be due to invalid IL or missing references)
			//IL_1816: Unknown result type (might be due to invalid IL or missing references)
			//IL_184a: Unknown result type (might be due to invalid IL or missing references)
			//IL_184c: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_18a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_18d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_18d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_18fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_191b: Unknown result type (might be due to invalid IL or missing references)
			//IL_193c: Unknown result type (might be due to invalid IL or missing references)
			//IL_195d: Unknown result type (might be due to invalid IL or missing references)
			//IL_197e: Unknown result type (might be due to invalid IL or missing references)
			//IL_199f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0701: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0706: Unknown result type (might be due to invalid IL or missing references)
			//IL_19d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_19dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_19df: Unknown result type (might be due to invalid IL or missing references)
			//IL_19e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a00: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a02: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a04: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a06: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a52: Unknown result type (might be due to invalid IL or missing references)
			//IL_13a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_1489: Unknown result type (might be due to invalid IL or missing references)
			//IL_0787: Unknown result type (might be due to invalid IL or missing references)
			//IL_0789: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_079f: Unknown result type (might be due to invalid IL or missing references)
			//IL_077e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0780: Unknown result type (might be due to invalid IL or missing references)
			//IL_0752: Unknown result type (might be due to invalid IL or missing references)
			//IL_073d: Unknown result type (might be due to invalid IL or missing references)
			//IL_073f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0757: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f49: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c26: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_15b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d06: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c80: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c85: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c99: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1d3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_165c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cc0: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cc2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cc7: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_1cce: Unknown result type (might be due to invalid IL or missing references)
			//IL_10e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ce3: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ce5: Unknown result type (might be due to invalid IL or missing references)
			//IL_11fa: Unknown result type (might be due to invalid IL or missing references)
			if (Main.Settings == null || !Main.Settings.Enabled || PlayerSettlementInfo.Instance == null)
			{
				return;
			}
			GameState activeState = Game.Current.GameStateManager.ActiveState;
			MapState val = (MapState)(object)((activeState is MapState) ? activeState : null);
			if (val == null)
			{
				return;
			}
			IMapStateHandler handler = val.Handler;
			MapScreen val2 = (MapScreen)(object)((handler is MapScreen) ? handler : null);
			if (val2 == null)
			{
				return;
			}
			float num2 = default(float);
			Vec3 val5 = default(Vec3);
			if (ghostPortVisualEntity != (GameEntity)null)
			{
				Campaign.Current.TimeControlMode = (CampaignTimeControlMode)0;
				Campaign.Current.SetTimeControlModeLock(true);
				if (val2.Input.IsKeyReleased(Main.Submodule.HelpKey.GetInputKey()))
				{
					ShowPortPosHelp(forceShow: true);
				}
				Vec3 zero = Vec3.Zero;
				Vec3 zero2 = Vec3.Zero;
				val2.SceneLayer.SceneView.TranslateMouse(ref zero, ref zero2, -1f);
				Vec3 val3 = zero;
				Vec3 val4 = zero2;
				PathFaceRecord nullFaceRecord = PathFaceRecord.NullFaceRecord;
				float num = default(float);
				bool flag = default(bool);
				val2.GetCursorIntersectionPoint(ref val3, ref val4, ref num, ref val5, ref nullFaceRecord, ref flag, (BodyFlags)79617);
				Vec3 origin = default(Vec3);
				val2.GetCursorIntersectionPoint(ref val3, ref val4, ref num2, ref origin, ref nullFaceRecord, ref flag, (BodyFlags)1128209);
				MatrixFrame frame = MatrixFrame.Identity;
				frame.origin = origin;
				frame.origin.z = val2.MapScene.GetWaterLevelAtPosition((frame.origin).AsVec2, true, false);
				val5 = new Vec3(0.25f, 0.25f, 0.25f, -1f);
				(frame).Scale(ref val5);
				MatrixFrame frame2 = ghostPortVisualEntity.GetFrame();
				portPlacementFrame = frame;
				SetFrame(ghostPortVisualEntity, ref frame);
				bool flag2 = (nullFaceRecord).IsValid() && !flag;
				CampaignVec2 val6 = default(CampaignVec2);
				(val6)..ctor((origin).AsVec2, flag);
				flag2 = flag2 && NavigationHelper.IsPositionValidForNavigationType(val6, (NavigationType)2);
				if (flag2)
				{
					int[] invalidTerrainTypesForNavigationType = Campaign.Current.Models.PartyNavigationModel.GetInvalidTerrainTypesForNavigationType((NavigationType)2);
					if (invalidTerrainTypesForNavigationType.Contains(nullFaceRecord.FaceGroupIndex) || invalidTerrainTypesForNavigationType.Contains((val6).Face.FaceGroupIndex))
					{
						flag2 = false;
					}
				}
				((ScreenLayer)val2.SceneLayer).ActiveCursor = (CursorType)(flag2 ? 1 : 10);
				PlacementSupported = flag2;
				return;
			}
			if (ghostGateVisualEntity != (GameEntity)null)
			{
				Campaign.Current.TimeControlMode = (CampaignTimeControlMode)0;
				Campaign.Current.SetTimeControlModeLock(true);
				if (val2.Input.IsKeyReleased(Main.Submodule.HelpKey.GetInputKey()))
				{
					ShowGatePosHelp(forceShow: true);
				}
				Vec3 zero3 = Vec3.Zero;
				Vec3 zero4 = Vec3.Zero;
				val2.SceneLayer.SceneView.TranslateMouse(ref zero3, ref zero4, -1f);
				Vec3 val7 = zero3;
				Vec3 val8 = zero4;
				PathFaceRecord nullFaceRecord2 = PathFaceRecord.NullFaceRecord;
				float num3 = default(float);
				bool flag3 = default(bool);
				val2.GetCursorIntersectionPoint(ref val7, ref val8, ref num3, ref val5, ref nullFaceRecord2, ref flag3, (BodyFlags)79617);
				Vec3 origin2 = default(Vec3);
				val2.GetCursorIntersectionPoint(ref val7, ref val8, ref num2, ref origin2, ref nullFaceRecord2, ref flag3, (BodyFlags)79633);
				MatrixFrame frame3 = MatrixFrame.Identity;
				frame3.origin = origin2;
				val5 = new Vec3(0.25f, 0.25f, 0.25f, -1f);
				(frame3).Scale(ref val5);
				MatrixFrame frame4 = ghostGateVisualEntity.GetFrame();
				gatePlacementFrame = frame3;
				SetFrame(ghostGateVisualEntity, ref frame3);
				bool flag4 = (nullFaceRecord2).IsValid() && flag3 && nullFaceRecord2.FaceIslandIndex == MobileParty.MainParty.CurrentNavigationFace.FaceIslandIndex;
				((ScreenLayer)val2.SceneLayer).ActiveCursor = (CursorType)(flag4 ? 1 : 10);
				PlacementSupported = flag4;
				return;
			}
			bool flag5 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyReleased(Main.Submodule.DeepEditToggleKey.GetInputKey());
			if (IsPlacingSettlement && flag5)
			{
				ToggleDeepEdit();
			}
			else
			{
				if (!IsPlacingSettlement)
				{
					return;
				}
				Campaign.Current.TimeControlMode = (CampaignTimeControlMode)0;
				Campaign.Current.SetTimeControlModeLock(true);
				if (val2.Input.IsKeyReleased(Main.Submodule.HelpKey.GetInputKey()))
				{
					if (IsDeepEdit)
					{
						ShowDeepEditHelp(forceShow: true);
					}
					else
					{
						ShowSettlementPlacementHelp(forceShow: true);
					}
				}
				if (deepEditPrefab == settlementVisualPrefab)
				{
					foreach (DeepTransformEdit deepTransformEdit13 in deepTransformEdits)
					{
						GameEntity val9 = (GameEntity)((deepTransformEdit13.Index < 0) ? ((object)settlementVisualEntity) : ((object)settlementVisualEntityChildren[deepTransformEdit13.Index]));
						MatrixFrame frame5 = val9.GetFrame();
						frame5.rotation = ((deepTransformEdit13?.Transform?.RotationScale != null) ? ((Mat3)deepTransformEdit13.Transform.RotationScale) : frame5.rotation);
						if (deepTransformEdit13.Index >= 0)
						{
							frame5.origin = ((deepTransformEdit13?.Transform?.Position != null) ? ((Vec3)deepTransformEdit13.Transform.Position) : frame5.origin);
						}
						SetFrame(val9, ref frame5, atGround: false);
						if (deepTransformEdit13 != null && deepTransformEdit13.IsDeleted)
						{
							val9.SetVisibilityExcludeParents(false);
						}
						else if (val9 != (GameEntity)null)
						{
							val9.SetVisibilityExcludeParents(true);
						}
					}
				}
				if (IsDeepEdit && currentDeepTarget != (GameEntity)null && !currentDeepTarget.IsVisibleIncludeParents())
				{
					UpdateDeepTarget(forward: true);
					return;
				}
				if (val2.Input.IsKeyDown(Main.Submodule.DeepEditApplyKey.GetInputKey()) && IsDeepEdit)
				{
					ToggleDeepEdit(showHelp: false);
					if (deepEditPrefab == settlementVisualPrefab)
					{
						foreach (DeepTransformEdit deepTransformEdit14 in deepTransformEdits)
						{
							GameEntity val10 = (GameEntity)((deepTransformEdit14.Index < 0) ? ((object)settlementVisualEntity) : ((object)settlementVisualEntityChildren[deepTransformEdit14.Index]));
							MatrixFrame frame6 = val10.GetFrame();
							frame6.rotation = ((deepTransformEdit14?.Transform?.RotationScale != null) ? ((Mat3)deepTransformEdit14.Transform.RotationScale) : frame6.rotation);
							if (deepTransformEdit14.Index >= 0)
							{
								frame6.origin = ((deepTransformEdit14?.Transform?.Position != null) ? ((Vec3)deepTransformEdit14.Transform.Position) : frame6.origin);
							}
							else
							{
								frame6.origin = ((deepTransformEdit14?.Transform?.Offsets != null) ? (frame6.origin + (Vec3)deepTransformEdit14.Transform.Offsets) : frame6.origin);
							}
							val10.SetFrame(ref frame6, true);
							if (deepTransformEdit14 != null && deepTransformEdit14.IsDeleted)
							{
								val10.SetVisibilityExcludeParents(false);
							}
							else if (val10 != (GameEntity)null)
							{
								val10.SetVisibilityExcludeParents(true);
							}
						}
					}
					StartGatePlacement();
					return;
				}
				bool flag6 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.ScaleModifierKey.GetInputKey());
				bool flag7 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.CycleModifierKey.GetInputKey());
				if (flag6)
				{
					bool flag8 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyReleased(Main.Submodule.ScaleSmallerKey.GetInputKey());
					bool flag9 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyReleased(Main.Submodule.ScaleBiggerKey.GetInputKey());
					bool flag10 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.ScaleSmallerKey.GetInputKey());
					bool flag11 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.ScaleBiggerKey.GetInputKey());
					float num4 = 1f / Main.Settings.CycleSpeed;
					if (flag11)
					{
						if (MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
						{
							holdTime = Time.ApplicationTime;
						}
						else if (holdTime + num4 < Time.ApplicationTime)
						{
							deepEditScale += 0.02f;
							holdTime = 0f;
							MarkEdited(currentDeepTarget);
							return;
						}
					}
					else if (flag10)
					{
						if (MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
						{
							holdTime = Time.ApplicationTime;
						}
						else if (holdTime + num4 < Time.ApplicationTime)
						{
							deepEditScale -= 0.02f;
							if (deepEditScale <= 0.15f)
							{
								deepEditScale = 0.1f;
							}
							holdTime = 0f;
							MarkEdited(currentDeepTarget);
							return;
						}
					}
					else
					{
						holdTime = 0f;
					}
					if (flag9 && MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
					{
						deepEditScale += 0.02f;
						MarkEdited(currentDeepTarget);
						return;
					}
					if (flag8 && MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
					{
						deepEditScale -= 0.02f;
						if (deepEditScale <= 0.15f)
						{
							deepEditScale = 0.1f;
						}
						MarkEdited(currentDeepTarget);
						return;
					}
				}
				else if (flag7)
				{
					bool flag12 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyReleased(Main.Submodule.CycleBackKey.GetInputKey());
					bool flag13 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyReleased(Main.Submodule.CycleNextKey.GetInputKey());
					bool flag14 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.CycleBackKey.GetInputKey());
					bool flag15 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.CycleNextKey.GetInputKey());
					bool flag16 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyReleased(Main.Submodule.MoveUpKey.GetInputKey());
					bool flag17 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyReleased(Main.Submodule.MoveDownKey.GetInputKey());
					bool flag18 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.MoveUpKey.GetInputKey());
					bool flag19 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.MoveDownKey.GetInputKey());
					float num5 = 1f / Main.Settings.CycleSpeed;
					if (IsDeepEdit)
					{
						if (flag15)
						{
							if (MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
							{
								holdTime = Time.ApplicationTime;
							}
							else if (holdTime + num5 < Time.ApplicationTime)
							{
								UpdateDeepTarget(forward: true);
								holdTime = 0f;
								return;
							}
						}
						else if (flag14)
						{
							if (MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
							{
								holdTime = Time.ApplicationTime;
							}
							else if (holdTime + num5 < Time.ApplicationTime)
							{
								UpdateDeepTarget(forward: false);
								holdTime = 0f;
								return;
							}
						}
						else if (flag18)
						{
							if (MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
							{
								holdTime = Time.ApplicationTime;
							}
							else if (holdTime + num5 < Time.ApplicationTime)
							{
								float num6 = 0.02f;
								DeepTransformEdit deepTransformEdit = MarkEdited(currentDeepTarget);
								if (currentDeepTarget != settlementVisualEntity && deepTransformEdit?.Transform?.Position != null)
								{
									deepTransformEdit.Transform.Position.z += num6;
								}
								else if (currentDeepTarget == settlementVisualEntity && deepTransformEdit?.Transform != null)
								{
									if (deepTransformEdit.Transform.Offsets == null)
									{
										deepTransformEdit.Transform.Offsets = Vec3.Zero;
									}
									deepTransformEdit.Transform.Offsets.z += num6;
								}
								holdTime = 0f;
								return;
							}
						}
						else if (flag19)
						{
							if (MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
							{
								holdTime = Time.ApplicationTime;
							}
							else if (holdTime + num5 < Time.ApplicationTime)
							{
								float num7 = 0.02f;
								DeepTransformEdit deepTransformEdit2 = MarkEdited(currentDeepTarget);
								if (currentDeepTarget != settlementVisualEntity && deepTransformEdit2?.Transform?.Position != null)
								{
									deepTransformEdit2.Transform.Position.z -= num7;
								}
								else if (currentDeepTarget == settlementVisualEntity && deepTransformEdit2?.Transform != null)
								{
									if (deepTransformEdit2.Transform.Offsets == null)
									{
										deepTransformEdit2.Transform.Offsets = Vec3.Zero;
									}
									deepTransformEdit2.Transform.Offsets.z -= num7;
								}
								holdTime = 0f;
								return;
							}
						}
						else
						{
							holdTime = 0f;
						}
						if (flag13 && flag7 && MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
						{
							UpdateDeepTarget(forward: true);
							return;
						}
						if (flag12 && flag7 && MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
						{
							UpdateDeepTarget(forward: false);
							return;
						}
						if (flag16 && flag7 && MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
						{
							float num8 = 0.02f;
							DeepTransformEdit deepTransformEdit3 = MarkEdited(currentDeepTarget);
							if (currentDeepTarget != settlementVisualEntity && deepTransformEdit3?.Transform?.Position != null)
							{
								deepTransformEdit3.Transform.Position.z += num8;
							}
							else if (currentDeepTarget == settlementVisualEntity && deepTransformEdit3?.Transform != null)
							{
								if (deepTransformEdit3.Transform.Offsets == null)
								{
									deepTransformEdit3.Transform.Offsets = Vec3.Zero;
								}
								deepTransformEdit3.Transform.Offsets.z += num8;
							}
							return;
						}
						if (flag17 && flag7 && MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
						{
							float num9 = 0.02f;
							DeepTransformEdit deepTransformEdit4 = MarkEdited(currentDeepTarget);
							if (currentDeepTarget != settlementVisualEntity && deepTransformEdit4?.Transform?.Position != null)
							{
								deepTransformEdit4.Transform.Position.z -= num9;
							}
							else if (currentDeepTarget == settlementVisualEntity && deepTransformEdit4?.Transform != null)
							{
								if (deepTransformEdit4.Transform.Offsets == null)
								{
									deepTransformEdit4.Transform.Offsets = Vec3.Zero;
								}
								deepTransformEdit4.Transform.Offsets.z -= num9;
							}
							return;
						}
					}
					else
					{
						if (flag15)
						{
							if (MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
							{
								holdTime = Time.ApplicationTime;
							}
							else if (holdTime + num5 < Time.ApplicationTime)
							{
								UpdateSettlementVisualEntity(forward: true);
								holdTime = 0f;
								return;
							}
						}
						else if (flag14)
						{
							if (MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
							{
								holdTime = Time.ApplicationTime;
							}
							else if (holdTime + num5 < Time.ApplicationTime)
							{
								UpdateSettlementVisualEntity(forward: false);
								holdTime = 0f;
								return;
							}
						}
						else if (flag18)
						{
							if (MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
							{
								holdTime = Time.ApplicationTime;
							}
							else if (holdTime + num5 < Time.ApplicationTime)
							{
								float num10 = 0.02f;
								DeepTransformEdit deepTransformEdit5 = MarkEdited(settlementVisualEntity);
								if (deepTransformEdit5?.Transform != null)
								{
									if (deepTransformEdit5.Transform.Offsets == null)
									{
										deepTransformEdit5.Transform.Offsets = Vec3.Zero;
									}
									deepTransformEdit5.Transform.Offsets.z += num10;
								}
								holdTime = 0f;
								return;
							}
						}
						else if (flag19)
						{
							if (MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
							{
								holdTime = Time.ApplicationTime;
							}
							else if (holdTime + num5 < Time.ApplicationTime)
							{
								float num11 = 0.02f;
								DeepTransformEdit deepTransformEdit6 = MarkEdited(settlementVisualEntity);
								if (deepTransformEdit6?.Transform != null)
								{
									if (deepTransformEdit6.Transform.Offsets == null)
									{
										deepTransformEdit6.Transform.Offsets = Vec3.Zero;
									}
									deepTransformEdit6.Transform.Offsets.z -= num11;
								}
								holdTime = 0f;
								return;
							}
						}
						else
						{
							holdTime = 0f;
						}
						if (flag13 && flag7 && MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
						{
							UpdateSettlementVisualEntity(forward: true);
							return;
						}
						if (flag12 && flag7 && MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
						{
							UpdateSettlementVisualEntity(forward: false);
							return;
						}
						if (flag16 && flag7 && MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
						{
							float num12 = 0.02f;
							DeepTransformEdit deepTransformEdit7 = MarkEdited(settlementVisualEntity);
							if (deepTransformEdit7?.Transform != null)
							{
								if (deepTransformEdit7.Transform.Offsets == null)
								{
									deepTransformEdit7.Transform.Offsets = Vec3.Zero;
								}
								deepTransformEdit7.Transform.Offsets.z += num12;
							}
							return;
						}
						if (flag17 && flag7 && MBMath.ApproximatelyEqualsTo(holdTime, 0f, 1E-05f))
						{
							float num13 = 0.02f;
							DeepTransformEdit deepTransformEdit8 = MarkEdited(settlementVisualEntity);
							if (deepTransformEdit8?.Transform != null)
							{
								if (deepTransformEdit8.Transform.Offsets == null)
								{
									deepTransformEdit8.Transform.Offsets = Vec3.Zero;
								}
								deepTransformEdit8.Transform.Offsets.z -= num13;
							}
							return;
						}
					}
				}
				else
				{
					holdTime = 0f;
				}
				if (IsDeepEdit)
				{
					bool flag20 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.UnDeleteModifierKey.GetInputKey());
					if (((ScreenLayer)val2.SceneLayer).Input.IsKeyReleased(Main.Submodule.DeleteKey.GetInputKey()))
					{
						if (flag20)
						{
							DeepTransformEdit deepTransformEdit9 = deepTransformEdits.LastOrDefault((DeepTransformEdit d) => d.IsDeleted && d.Index >= 0);
							if (deepTransformEdit9 != null)
							{
								deepTransformEdit9.IsDeleted = false;
							}
							UpdateDeepTarget(currentDeepTarget);
							return;
						}
						if (currentDeepTarget != settlementVisualEntity)
						{
							GameEntity? obj = currentDeepTarget;
							if (obj != null && obj.IsVisibleIncludeParents())
							{
								DeepTransformEdit deepTransformEdit10 = MarkEdited(currentDeepTarget);
								if (deepTransformEdit10 != null)
								{
									deepTransformEdit10.IsDeleted = true;
								}
								UpdateDeepTarget(forward: true);
								return;
							}
						}
					}
				}
				MatrixFrame frame7 = settlementVisualEntity.GetFrame();
				PathFaceRecord nullFaceRecord3 = PathFaceRecord.NullFaceRecord;
				MatrixFrame frame8 = frame7;
				if (!IsDeepEdit)
				{
					Vec3 zero5 = Vec3.Zero;
					Vec3 zero6 = Vec3.Zero;
					val2.SceneLayer.SceneView.TranslateMouse(ref zero5, ref zero6, -1f);
					Vec3 val11 = zero5;
					Vec3 val12 = zero6;
					float num14 = default(float);
					bool flag21 = default(bool);
					val2.GetCursorIntersectionPoint(ref val11, ref val12, ref num14, ref val5, ref nullFaceRecord3, ref flag21, (BodyFlags)79617);
					Vec3 origin3 = default(Vec3);
					val2.GetCursorIntersectionPoint(ref val11, ref val12, ref num2, ref origin3, ref nullFaceRecord3, ref flag21, (BodyFlags)79633);
					frame8.origin = origin3;
				}
				DeepTransformEdit deepTransformEdit11 = deepTransformEdits.FirstOrDefault((DeepTransformEdit dte) => dte.Index < 0);
				SetFrame(settlementVisualEntity, ref frame8, atGround: true, deepTransformEdit11?.Transform?.Offsets);
				GameEntity editTarget = currentDeepTarget ?? settlementVisualEntity;
				MatrixFrame globalFrame = editTarget.GetGlobalFrame();
				float num15 = 0f;
				float num16 = 0f;
				bool flag22 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.RotateModifierKey.GetInputKey());
				bool flag23 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.RotateAlternateModifierKey.GetInputKey());
				bool flag24 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.RotatePreviousKey.GetInputKey());
				bool flag25 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.RotateNextKey.GetInputKey());
				bool flag26 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.RotateForwardKey.GetInputKey());
				bool flag27 = ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.RotateBackwardsKey.GetInputKey());
				if (IsDeepEdit && flag22 && ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown((InputKey)224))
				{
					Vec3 zero7 = Vec3.Zero;
					Vec3 zero8 = Vec3.Zero;
					val2.SceneLayer.SceneView.TranslateMouse(ref zero7, ref zero8, -1f);
					Vec3 val13 = zero7;
					Vec3 val14 = zero8;
					float num17 = default(float);
					bool flag28 = default(bool);
					val2.GetCursorIntersectionPoint(ref val13, ref val14, ref num17, ref val5, ref nullFaceRecord3, ref flag28, (BodyFlags)79617);
					Vec3 val15 = default(Vec3);
					val2.GetCursorIntersectionPoint(ref val13, ref val14, ref num2, ref val15, ref nullFaceRecord3, ref flag28, (BodyFlags)79633);
					globalFrame.origin.x = val15.x;
					globalFrame.origin.y = val15.y;
					MarkEdited(editTarget);
				}
				if (flag26 && flag22)
				{
					num16 = inputInformation.Dt * 2f;
					MarkEdited(editTarget);
				}
				else if (flag27 && flag22)
				{
					num16 = inputInformation.Dt * -2f;
					MarkEdited(editTarget);
				}
				num16 = num16 * 2.75f * inputInformation.Dt;
				if (flag24 && flag22)
				{
					num15 = inputInformation.Dt * 2f;
					MarkEdited(editTarget);
				}
				else if (flag25 && flag22)
				{
					num15 = inputInformation.Dt * -2f;
					MarkEdited(editTarget);
				}
				num15 += inputInformation.HorizontalCameraInput * 1.75f * inputInformation.Dt;
				if (inputInformation.RightMouseButtonDown && flag22)
				{
					num15 += 0.01f * inputInformation.MouseSensitivity * inputInformation.MouseMoveX;
					num15 *= Main.Settings.MouseRotationModifier / 5f;
					MarkEdited(editTarget);
				}
				else if (flag22 && (flag24 || flag25))
				{
					num15 *= Main.Settings.KeyRotationModifier / 5f;
					MarkEdited(editTarget);
				}
				float num18 = num15;
				(globalFrame.rotation).RotateAboutUp(0f - num18);
				if (flag23)
				{
					float num19 = num16;
					(globalFrame.rotation).RotateAboutSide(0f - num19);
				}
				else
				{
					float num20 = num16;
					(globalFrame.rotation).RotateAboutForward(0f - num20);
				}
				val5 = new Vec3(deepEditScale, deepEditScale, deepEditScale, -1f);
				(globalFrame).Scale(ref val5);
				deepEditScale = 1f;
				editTarget.SetGlobalFrame(ref globalFrame, true);
				DeepTransformEdit deepTransformEdit12 = deepTransformEdits.FirstOrDefault((DeepTransformEdit dte) => dte.Index == settlementVisualEntityChildren.IndexOf(editTarget));
				if (deepTransformEdit12 != null)
				{
					MatrixFrame frame9 = editTarget.GetFrame();
					deepTransformEdit12.Transform = new TransformSaveable
					{
						Position = ((deepTransformEdit12.Index < 0) ? null : ((Vec3Saveable?)frame9.origin)),
						Offsets = ((deepTransformEdit12.Index < 0) ? ((Vec3Saveable?)(frame9.origin - frame8.origin)) : null),
						RotationScale = frame9.rotation
					};
				}
				settlementPlacementFrame = settlementVisualEntity.GetFrame();
				if (!IsDeepEdit)
				{
					bool flag29 = (nullFaceRecord3).IsValid() && nullFaceRecord3.FaceIslandIndex == MobileParty.MainParty.CurrentNavigationFace.FaceIslandIndex;
					((ScreenLayer)val2.SceneLayer).ActiveCursor = (CursorType)(flag29 ? 1 : 10);
					PlacementSupported = flag29;
				}
			}
		}

		private static void ShowDeepEditHelp(bool forceShow = false)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			if (!Main.Settings.DisableAutoHints || forceShow)
			{
				TextObject val = new TextObject("{=player_settlement_38}Press {HELP_KEY} for help. \r\nPress {APPLY_KEY} to apply or press {ESC_KEY} to cancel.  \r\nUse {DEEP_EDIT_KEY} to switch from deep edit mode to placement mode. \r\nUse {CYCLE_MODIFIER_KEY} and {CYCLE_BACK_KEY} / {CYCLE_NEXT_KEY} to change selected sub model.\r\nUse {ROTATE_MODIFIER_KEY} and {MOUSE_CLICK} to reposition. \r\nUse {ROTATE_MODIFIER_KEY} and {ROTATE_BACK_KEY} / {ROTATE_NEXT_KEY} to change rotation. \r\nUse {ROTATE_MODIFIER_KEY} and {ROTATE_FORWARD_KEY} / {ROTATE_BACKWARD_KEY} to change forward rotation. \r\nUse {ROTATE_MODIFIER_KEY} + {ROTATE_MODIFIER_ALTERNATE} and {ROTATE_FORWARD_KEY} / {ROTATE_BACKWARD_KEY} to change axis rotation. \r\nUse {SCALE_MODIFIER_KEY} and {SCALE_BACK_KEY} / {SCALE_NEXT_KEY} to change scale. \r\nUse {CYCLE_MODIFIER_KEY} and {MOVE_UP_KEY} / {MOVE_DOWN_KEY} to move up or down. \r\nUse {DELETE_KEY} to delete selection. \r\nUse {UNDELETE_MODIFIER_KEY} and {DELETE_KEY} to un-delete previous deletion.", (Dictionary<string, object>)null);
				InputKey val2 = Main.Submodule.HelpKey.GetInputKey();
				val.SetTextVariable("HELP_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)val2).ToString(), 1f));
				val2 = (InputKey)1;
				val.SetTextVariable("ESC_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)val2).ToString(), 1f));
				val2 = Main.Submodule.DeepEditApplyKey.GetInputKey();
				val.SetTextVariable("APPLY_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)val2).ToString(), 1f));
				val2 = Main.Submodule.DeepEditToggleKey.GetInputKey();
				val.SetTextVariable("DEEP_EDIT_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)val2).ToString(), 1f));
				val.SetTextVariable("CYCLE_MODIFIER_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.CycleModifierKey)).ToString(), 1f));
				val.SetTextVariable("CYCLE_BACK_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.CycleBackKey)).ToString(), 1f));
				val.SetTextVariable("CYCLE_NEXT_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.CycleNextKey)).ToString(), 1f));
				val.SetTextVariable("MOVE_UP_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.MoveUpKey)).ToString(), 1f));
				val.SetTextVariable("MOVE_DOWN_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.MoveDownKey)).ToString(), 1f));
				val2 = (InputKey)224;
				val.SetTextVariable("MOUSE_CLICK", HyperlinkTexts.GetKeyHyperlinkText(((object)val2).ToString(), 1f));
				val.SetTextVariable("ROTATE_MODIFIER_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.RotateModifierKey)).ToString(), 1f));
				val.SetTextVariable("ROTATE_MODIFIER_ALTERNATE", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.RotateAlternateModifierKey)).ToString(), 1f));
				val.SetTextVariable("ROTATE_BACK_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.RotatePreviousKey)).ToString(), 1f));
				val.SetTextVariable("ROTATE_NEXT_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.RotateNextKey)).ToString(), 1f));
				val.SetTextVariable("ROTATE_FORWARD_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.RotateForwardKey)).ToString(), 1f));
				val.SetTextVariable("ROTATE_BACKWARD_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.RotateBackwardsKey)).ToString(), 1f));
				val.SetTextVariable("SCALE_MODIFIER_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.ScaleModifierKey)).ToString(), 1f));
				val.SetTextVariable("SCALE_BACK_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.ScaleSmallerKey)).ToString(), 1f));
				val.SetTextVariable("SCALE_NEXT_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.ScaleBiggerKey)).ToString(), 1f));
				val.SetTextVariable("UNDELETE_MODIFIER_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.UnDeleteModifierKey)).ToString(), 1f));
				val.SetTextVariable("DELETE_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.DeleteKey)).ToString(), 1f));
				int num = Main.Settings.HintDurationSeconds * 1000;
				Hero mainHero = Hero.MainHero;
				MBInformationManager.AddQuickInformation(val, num, (BasicCharacterObject)(object)((mainHero != null) ? mainHero.CharacterObject : null), (Equipment)null, "");
			}
		}

		private DeepTransformEdit? MarkEdited(GameEntity? currentDeepTarget)
		{
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			if (settlementVisualEntity == (GameEntity)null || currentDeepTarget == (GameEntity)null)
			{
				LogManager.EventTracer.Trace($"Unable to mark edited: settlementVisualEntity = {settlementVisualEntity} currentDeepTarget = {currentDeepTarget}");
				return null;
			}
			if (deepEditPrefab != settlementVisualPrefab)
			{
				deepTransformEdits.Clear();
				deepEditPrefab = settlementVisualPrefab;
			}
			int idx = settlementVisualEntityChildren.IndexOf(currentDeepTarget);
			DeepTransformEdit deepTransformEdit = deepTransformEdits.FirstOrDefault((DeepTransformEdit dte) => dte.Index == idx);
			if (deepTransformEdit == null)
			{
				MatrixFrame frame = currentDeepTarget.GetFrame();
				deepTransformEdit = new DeepTransformEdit
				{
					Index = idx,
					Name = currentDeepTarget.Name,
					Transform = new TransformSaveable
					{
						Position = frame.origin,
						Offsets = Vec3.Zero,
						RotationScale = frame.rotation
					},
					IsDeleted = false
				};
				deepTransformEdits.Add(deepTransformEdit);
			}
			GameEntity? obj = settlementVisualEntity;
			LogManager.EventTracer.Trace($"Mark edited: settlementVisualEntity = {((obj != null) ? obj.Name : null)} currentDeepTarget = {((currentDeepTarget != null) ? currentDeepTarget.Name : null)} idx = {idx}");
			return deepTransformEdit;
		}

		private void ToggleDeepEdit(bool showHelp = true)
		{
			deepEdit = !deepEdit;
			if (deepEdit)
			{
				if (showHelp)
				{
					ShowDeepEditHelp();
				}
				deepEditPrefab = settlementVisualPrefab;
				UpdateDeepTarget(settlementVisualEntity);
			}
			else
			{
				if (showHelp)
				{
					ShowSettlementPlacementHelp();
				}
				RefreshVisualSelection();
			}
		}

		private void UpdateDeepTarget(bool forward)
		{
			bool flag = false;
			if (forward)
			{
				int num = settlementVisualEntityChildren.IndexOf(currentDeepTarget);
				if (num < 0)
				{
					num = 0;
					do
					{
						if (UpdateDeepTarget(num))
						{
							flag = true;
							break;
						}
						num++;
						if (num == settlementVisualEntityChildren.Count)
						{
							UpdateDeepTarget(-1);
							break;
						}
					}
					while (!flag);
					return;
				}
				do
				{
					num++;
					if (num == settlementVisualEntityChildren.Count)
					{
						UpdateDeepTarget(-1);
						break;
					}
					if (UpdateDeepTarget(num))
					{
						flag = true;
						break;
					}
				}
				while (!flag);
				return;
			}
			int num2 = settlementVisualEntityChildren.IndexOf(currentDeepTarget);
			if (num2 < 0)
			{
				num2 = settlementVisualEntityChildren.Count - 1;
				do
				{
					if (UpdateDeepTarget(num2))
					{
						flag = true;
						break;
					}
					num2--;
					if (num2 < 0)
					{
						UpdateDeepTarget(-1);
						break;
					}
				}
				while (!flag);
				return;
			}
			do
			{
				num2--;
				if (num2 < 0)
				{
					UpdateDeepTarget(-1);
					break;
				}
				if (UpdateDeepTarget(num2))
				{
					flag = true;
					break;
				}
			}
			while (!flag);
		}

		private bool UpdateDeepTarget(GameEntity? target)
		{
			if (target == (GameEntity)null)
			{
				ResetDeepEdits();
				return false;
			}
			int idx = settlementVisualEntityChildren.IndexOf(target);
			return UpdateDeepTarget(idx);
		}

		private bool UpdateDeepTarget(int idx)
		{
			RefreshVisualSelection();
			HideDeletedDeepEdits();
			GameEntity parent2 = (currentDeepTarget = (GameEntity?)((idx < 0) ? ((object)settlementVisualEntity) : ((object)settlementVisualEntityChildren[idx])));
			GameEntity? obj = currentDeepTarget;
			if (obj == null || !obj.IsVisibleIncludeParents())
			{
				return false;
			}
			return UpdateEntities(parent2);
			static bool UpdateEntities(GameEntity parent)
			{
				bool flag = false;
				List<GameEntity> list = parent.GetEntityAndChildren().ToList();
				for (int i = 0; i < list.Count; i++)
				{
					GameEntity val = list[i];
					if (val != parent)
					{
						flag = UpdateEntities(val) || flag;
					}
					MetaMesh metaMesh = val.GetMetaMesh(0);
					if (!((NativeObject)(object)metaMesh == (NativeObject)null))
					{
						for (int j = 0; j < metaMesh.MeshCount; j++)
						{
							Mesh meshAtIndex = metaMesh.GetMeshAtIndex(j);
							Material material = meshAtIndex.GetMaterial();
							meshAtIndex.SetMaterial("plain_green");
							flag = true;
						}
					}
				}
				return flag;
			}
		}

		private void HideDeletedDeepEdits()
		{
			if (deepTransformEdits == null)
			{
				return;
			}
			foreach (DeepTransformEdit item in deepTransformEdits.Where((DeepTransformEdit dte) => dte.IsDeleted && dte.Index >= 0))
			{
				try
				{
					GameEntity val = settlementVisualEntityChildren[item.Index];
					val.SetVisibilityExcludeParents(false);
				}
				catch (Exception ex)
				{
					LogManager.EventTracer.Trace(new List<string> { ex.Message, ex.StackTrace });
				}
			}
		}

		private void SetFrame(GameEntity? entity, ref MatrixFrame frame, bool atGround = true, Vec3? offset = null)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			if (entity != (GameEntity)null)
			{
				entity.SetFrame(ref frame, true);
				if (atGround)
				{
					Scene scene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
					Vec3 origin = default(Vec3);
					(origin)..ctor(frame.origin.x, frame.origin.y, 0f, -1f);
					Vec2 val = new Vec2(frame.origin.x, frame.origin.y);
					origin.z = scene.GetGroundHeightAtPosition((val).ToVec3(0f), (BodyFlags)544321929);
					entity.SetLocalPosition((frame.origin = origin) + (Vec3)(offset ?? Vec3.Zero));
				}
			}
		}

		public void StartPortPlacement()
		{
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Expected O, but got Unknown
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Expected O, but got Unknown
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Expected O, but got Unknown
			portPlacementFrame = null;
			if (Main.Settings == null || !Main.IsWarSails || !portSupported)
			{
				ApplyNow();
				return;
			}
			GameState activeState = Game.Current.GameStateManager.ActiveState;
			MapState val = (MapState)(object)((activeState is MapState) ? activeState : null);
			if (val == null)
			{
				return;
			}
			IMapStateHandler handler = val.Handler;
			MapScreen val2 = (MapScreen)(object)((handler is MapScreen) ? handler : null);
			if (val2 == null || applyPending == null || !((ScreenLayer)val2.SceneLayer).Input.GetIsMouseActive() || !PlacementSupported)
			{
				return;
			}
			string text = ((object)new TextObject("{=player_settlement_44}Add a Port", (Dictionary<string, object>)null)).ToString();
			TextObject val3 = new TextObject("{=player_settlement_45}Do you want to add a port to this settlement?", (Dictionary<string, object>)null);
			InformationManager.ShowInquiry(new InquiryData(text, ((object)val3).ToString(), true, true, ((object)GameTexts.FindText("str_yes", (string)null)).ToString(), ((object)GameTexts.FindText("str_no", (string)null)).ToString(), (Action)delegate
			{
				InformationManager.HideInquiry();
				try
				{
					PlacementSupported = false;
					ShowPortPosHelp();
					ShowGhostPortVisualEntity(retry: true);
				}
				catch (Exception e)
				{
					LogManager.Log.NotifyBad(e);
				}
			}, (Action)delegate
			{
				ApplyNow();
			}, "", 0f, (Action)null, (Func<ValueTuple<bool, string>>)null, (Func<ValueTuple<bool, string>>)null), true, false);
		}

		public void StartGatePlacement()
		{
			if (Main.Settings == null || !Main.Settings.AllowGatePosition || !gateSupported)
			{
				StartPortPlacement();
				return;
			}
			GameState activeState = Game.Current.GameStateManager.ActiveState;
			MapState val = (MapState)(object)((activeState is MapState) ? activeState : null);
			if (val == null)
			{
				return;
			}
			IMapStateHandler handler = val.Handler;
			MapScreen val2 = (MapScreen)(object)((handler is MapScreen) ? handler : null);
			if (val2 == null || applyPending == null || !((ScreenLayer)val2.SceneLayer).Input.GetIsMouseActive() || !PlacementSupported)
			{
				return;
			}
			try
			{
				PlacementSupported = false;
				ShowGatePosHelp();
				ShowGhostGateVisualEntity(retry: true);
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
		}

		private static void ShowPortPosHelp(bool forceShow = false)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			if (!Main.Settings.DisableAutoHints || forceShow)
			{
				TextObject val = new TextObject("{=player_settlement_43}Choose your port position. \r\nPress {HELP_KEY} for help. \r\nClick {MOUSE_CLICK} anywhere to apply or press {ESC_KEY} to go back to previous.", (Dictionary<string, object>)null);
				InputKey val2 = Main.Submodule.HelpKey.GetInputKey();
				val.SetTextVariable("HELP_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)val2).ToString(), 1f));
				val2 = (InputKey)1;
				val.SetTextVariable("ESC_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)val2).ToString(), 1f));
				val2 = (InputKey)224;
				val.SetTextVariable("MOUSE_CLICK", HyperlinkTexts.GetKeyHyperlinkText(((object)val2).ToString(), 1f));
				int num = Main.Settings.HintDurationSeconds * 1000;
				Hero mainHero = Hero.MainHero;
				MBInformationManager.AddQuickInformation(val, num, (BasicCharacterObject)(object)((mainHero != null) ? mainHero.CharacterObject : null), (Equipment)null, "");
			}
		}

		private static void ShowGatePosHelp(bool forceShow = false)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			if (!Main.Settings.DisableAutoHints || forceShow)
			{
				TextObject val = new TextObject("{=player_settlement_36}Choose your gate position. \r\nPress {HELP_KEY} for help. \r\nClick {MOUSE_CLICK} anywhere to apply or press {ESC_KEY} to go back to settlement placement.", (Dictionary<string, object>)null);
				InputKey val2 = Main.Submodule.HelpKey.GetInputKey();
				val.SetTextVariable("HELP_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)val2).ToString(), 1f));
				val2 = (InputKey)1;
				val.SetTextVariable("ESC_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)val2).ToString(), 1f));
				val2 = (InputKey)224;
				val.SetTextVariable("MOUSE_CLICK", HyperlinkTexts.GetKeyHyperlinkText(((object)val2).ToString(), 1f));
				int num = Main.Settings.HintDurationSeconds * 1000;
				Hero mainHero = Hero.MainHero;
				MBInformationManager.AddQuickInformation(val, num, (BasicCharacterObject)(object)((mainHero != null) ? mainHero.CharacterObject : null), (Equipment)null, "");
			}
		}

		public void ApplyNow()
		{
			GameState activeState = Game.Current.GameStateManager.ActiveState;
			MapState val = (MapState)(object)((activeState is MapState) ? activeState : null);
			if (val == null)
			{
				return;
			}
			IMapStateHandler handler = val.Handler;
			MapScreen val2 = (MapScreen)(object)((handler is MapScreen) ? handler : null);
			if (val2 == null || applyPending == null || !((ScreenLayer)val2.SceneLayer).Input.GetIsMouseActive() || !PlacementSupported)
			{
				return;
			}
			try
			{
				Action action = applyPending;
				action();
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
		}

		public void Reset()
		{
			Campaign.Current.TimeControlMode = (CampaignTimeControlMode)0;
			Campaign.Current.SetTimeControlModeLock(false);
			availableModels?.Clear();
			currentModelOptionIdx = -1;
			ClearEntities();
			settlementPlacementFrame = null;
			applyPending = null;
			settlementVisualPrefab = null;
			SettlementRequest = SettlementType.None;
			ReSettlementRequest = null;
			OverwriteRequest = null;
			gateSupported = false;
			portSupported = false;
			ghostGateVisualEntity = null;
			ghostPortVisualEntity = null;
			gatePlacementFrame = null;
			portPlacementFrame = null;
			ResetDeepEdits();
			settlementVisualEntityChildren.Clear();
			PlacementSupported = false;
			MbEvent onReset = _onReset;
			if (onReset != null)
			{
				onReset.Invoke();
			}
			LogManager.EventTracer.Trace();
		}

		private void ResetDeepEdits()
		{
			deepTransformEdits.Clear();
			deepEditScale = 1f;
			deepEdit = false;
			deepEditPrefab = null;
			currentDeepTarget = null;
		}

		private void ClearEntities()
		{
			settlementVisualPrefab = null;
			settlementVisualEntity?.ClearEntity();
			settlementVisualEntity = null;
			ghostGateVisualEntity?.ClearEntity();
			ghostGateVisualEntity = null;
			ghostPortVisualEntity?.ClearEntity();
			ghostPortVisualEntity = null;
			LogManager.EventTracer.Trace();
		}

		public void Tick(float delta)
		{
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			if (Main.Settings == null || !Main.Settings.Enabled || PlayerSettlementInfo.Instance == null || Settlement.CurrentSettlement != null || Hero.MainHero.IsPrisoner || PlayerEncounter.Current != null || Mission.Current != null)
			{
				return;
			}
			if (SettlementRequest == SettlementType.Town)
			{
				LogManager.EventTracer.Trace("Build requested for Town");
				Reset();
				BuildTown();
				return;
			}
			if (SettlementRequest == SettlementType.Village)
			{
				LogManager.EventTracer.Trace("Build requested for Village");
				Reset();
				BuildVillage();
				return;
			}
			if (SettlementRequest == SettlementType.Castle)
			{
				LogManager.EventTracer.Trace("Build requested for Castle");
				Reset();
				BuildCastle();
				return;
			}
			List<string> list;
			if (ReSettlementRequest != null)
			{
				list = new List<string> { "Rebuild requested for " + (ReSettlementRequest.StringId ?? ReSettlementRequest.Identifier.ToString()) };
				if (!string.IsNullOrEmpty(ReSettlementRequest.PrefabId))
				{
					list.Add("Current prefab: $" + ReSettlementRequest.PrefabId);
				}
				if (!ReSettlementRequest.BuildComplete)
				{
					CampaignTime buildEnd = ReSettlementRequest.BuildEnd;
					if ((buildEnd).IsFuture)
					{
						goto IL_01a4;
					}
				}
				list.Add("Build was completed before");
				goto IL_01a4;
			}
			if (OverwriteRequest == null)
			{
				return;
			}
			List<string> extraInfo = new List<string> { "Rebuild requested for " + ((MBObjectBase)OverwriteRequest).StringId };
			LogManager.EventTracer.Trace(extraInfo);
			Settlement target2 = OverwriteRequest;
			Reset();
			target2.IsVisible = false;
			IMbEvent? onResetEvent = OnResetEvent;
			if (onResetEvent != null)
			{
				onResetEvent.AddNonSerializedListener((object)target2, (Action)delegate
				{
					OnResetEvent.ClearListeners((object)target2);
					target2.IsVisible = true;
				});
			}
			Overwrite(target2);
			return;
			IL_01a4:
			LogManager.EventTracer.Trace(list);
			PlayerSettlementItem target = ReSettlementRequest;
			Reset();
			target.Settlement.IsVisible = false;
			IMbEvent? onResetEvent2 = OnResetEvent;
			if (onResetEvent2 != null)
			{
				onResetEvent2.AddNonSerializedListener((object)target, (Action)delegate
				{
					OnResetEvent.ClearListeners((object)target);
					target.Settlement.IsVisible = true;
				});
			}
			Rebuild(target);
		}

		private void Overwrite(Settlement target)
		{
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Expected O, but got Unknown
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Expected O, but got Unknown
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Expected O, but got Unknown
			SettlementType settlementType = (target.IsVillage ? SettlementType.Village : (target.IsCastle ? SettlementType.Castle : (target.IsTown ? SettlementType.Town : SettlementType.None)));
			if (settlementType == SettlementType.None)
			{
				Reset();
				return;
			}
			PlacementSupported = false;
			gateSupported = settlementType != SettlementType.Village;
			portSupported = Main.IsWarSails && settlementType == SettlementType.Town && target.HasPort;
			TextObject val = new TextObject("{=player_settlement_42}Rebuild {SETTLEMENT}", (Dictionary<string, object>)null);
			val.SetTextVariable("SETTLEMENT", target.Name);
			InformationManager.ShowTextInquiry(new TextInquiryData(((object)val).ToString(), ((object)new TextObject("{=player_settlement_03}What would you like to name your settlement?", (Dictionary<string, object>)null)).ToString(), true, true, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), ((object)GameTexts.FindText("str_cancel", (string)null)).ToString(), (Action<string>)delegate(string settlementName)
			{
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0132: Expected O, but got Unknown
				//IL_011f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0112: Unknown result type (might be due to invalid IL or missing references)
				//IL_0164: Unknown result type (might be due to invalid IL or missing references)
				//IL_016a: Expected O, but got Unknown
				//IL_0157: Unknown result type (might be due to invalid IL or missing references)
				//IL_014a: Unknown result type (might be due to invalid IL or missing references)
				//IL_020a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0216: Expected O, but got Unknown
				Campaign.Current.TimeControlMode = (CampaignTimeControlMode)0;
				if (string.IsNullOrEmpty(settlementName))
				{
					settlementName = ((object)target.Name).ToString();
				}
				if (Main.Settings.ForcePlayerCulture)
				{
					if (Main.Settings.AutoAllocateVillageType || settlementType != SettlementType.Village)
					{
						Apply(settlementName, Hero.MainHero.Culture, (settlementType == SettlementType.Village) ? AutoCalculateVillageType(-1) : null);
					}
					else
					{
						PlayerSettlementBehaviour playerSettlementBehaviour = this;
						string settlementName2 = settlementName;
						CultureObject culture2 = Hero.MainHero.Culture;
						Village village2 = target.Village;
						playerSettlementBehaviour.DetermineVillageType(settlementName2, culture2, (village2 != null) ? village2.Bound : null, -1, Apply);
					}
				}
				else
				{
					TextObject val11 = ((settlementType == SettlementType.Castle) ? new TextObject("{=player_settlement_20}Choose castle culture", (Dictionary<string, object>)null) : ((settlementType == SettlementType.Village) ? new TextObject("{=player_settlement_11}Choose village culture", (Dictionary<string, object>)null) : new TextObject("{=player_settlement_09}Choose town culture", (Dictionary<string, object>)null)));
					TextObject val12 = ((settlementType == SettlementType.Castle) ? new TextObject("{=player_settlement_21}Choose the culture for {CASTLE}", (Dictionary<string, object>)null) : ((settlementType == SettlementType.Village) ? new TextObject("{=player_settlement_12}Choose the culture for {VILLAGE}", (Dictionary<string, object>)null) : new TextObject("{=player_settlement_10}Choose the culture for {TOWN}", (Dictionary<string, object>)null)));
					val12.SetTextVariable("CASTLE", settlementName);
					val12.SetTextVariable("TOWN", settlementName);
					val12.SetTextVariable("VILLAGE", settlementName);
					List<InquiryElement> list = GetCultures(mainOnly: true).Select((Func<CultureObject, InquiryElement>)((CultureObject c) => new InquiryElement((object)c, ((object)((BasicCultureObject)c).Name).ToString(), (ImageIdentifier)new BannerImageIdentifier(new Banner(((BasicCultureObject)c).Banner), false), true, ((object)((BasicCultureObject)c).Name).ToString()))).ToList();
					MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(((object)val11).ToString(), ((object)val12).ToString(), list, false, 1, 1, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), (string)null, (Action<List<InquiryElement>>)delegate(List<InquiryElement> args)
					{
						object obj = args?.FirstOrDefault()?.Identifier;
						CultureObject culture3 = (CultureObject)(((obj is CultureObject) ? obj : null) ?? Hero.MainHero.Culture);
						if (Main.Settings.AutoAllocateVillageType || settlementType != SettlementType.Village)
						{
							Apply(settlementName, culture3, AutoCalculateVillageType(-1));
						}
						else
						{
							PlayerSettlementBehaviour playerSettlementBehaviour2 = this;
							string settlementName3 = settlementName;
							Village village3 = target.Village;
							playerSettlementBehaviour2.DetermineVillageType(settlementName3, culture3, (village3 != null) ? village3.Bound : null, -1, Apply);
						}
					}, (Action<List<InquiryElement>>)null, "", false), false, false);
				}
			}, (Action)delegate
			{
				InformationManager.HideInquiry();
				Reset();
				((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
			}, false, (Func<string, Tuple<bool, string>>)FactionHelper.IsKingdomNameApplicable, "", ((object)target.Name).ToString()), true, false);
			void Apply(string settlementName, CultureObject culture, string? villageType)
			{
				string villageType2 = villageType;
				settlementPlacementFrame = null;
				availableModels?.Clear();
				switch (settlementType)
				{
				case SettlementType.Village:
					if (Main.Settings.SelectedCultureOnly && Main.Submodule.CultureTemplates.ContainsKey(((MBObjectBase)culture).StringId))
					{
						availableModels = Main.Submodule.CultureTemplates[((MBObjectBase)culture).StringId].SelectMany((CultureSettlementTemplate cst) => SelectVillageTemplates(cst, target.Village.Bound.IsCastle)).ToList();
						currentModelOptionIdx = -1;
					}
					if (availableModels == null || availableModels.Count == 0)
					{
						availableModels = Main.Submodule.CultureTemplates.Values.SelectMany((List<CultureSettlementTemplate> c) => c.SelectMany((CultureSettlementTemplate cst) => SelectVillageTemplates(cst, target.Village.Bound.IsCastle))).ToList();
						currentModelOptionIdx = availableModels.FindIndex((PlayerSettlementItemTemplate a) => a.Culture == ((MBObjectBase)culture).StringId) - 1;
					}
					break;
				case SettlementType.Castle:
					if (Main.Settings.SelectedCultureOnly && Main.Submodule.CultureTemplates.ContainsKey(((MBObjectBase)culture).StringId))
					{
						availableModels = Main.Submodule.CultureTemplates[((MBObjectBase)culture).StringId].SelectMany(SelectCastleTemplates).ToList();
						currentModelOptionIdx = -1;
					}
					if (availableModels == null || availableModels.Count == 0)
					{
						availableModels = Main.Submodule.CultureTemplates.Values.SelectMany((List<CultureSettlementTemplate> c) => c.SelectMany(SelectCastleTemplates)).ToList();
						currentModelOptionIdx = availableModels.FindIndex((PlayerSettlementItemTemplate a) => a.Culture == ((MBObjectBase)culture).StringId) - 1;
					}
					break;
				case SettlementType.Town:
					if (Main.Settings.SelectedCultureOnly && Main.Submodule.CultureTemplates.ContainsKey(((MBObjectBase)culture).StringId))
					{
						availableModels = Main.Submodule.CultureTemplates[((MBObjectBase)culture).StringId].SelectMany(SelectTownTemplates).ToList();
						currentModelOptionIdx = -1;
					}
					if (availableModels == null || availableModels.Count == 0)
					{
						availableModels = Main.Submodule.CultureTemplates.Values.SelectMany((List<CultureSettlementTemplate> c) => c.SelectMany(SelectTownTemplates)).ToList();
						currentModelOptionIdx = availableModels.FindIndex((PlayerSettlementItemTemplate a) => a.Culture == ((MBObjectBase)culture).StringId) - 1;
					}
					break;
				default:
					InformationManager.HideInquiry();
					if (!Main.Settings.SettlementPlacement)
					{
						Reset();
						((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
					}
					return;
				}
				if (!Main.Settings.SettlementPlacement)
				{
					ConfirmAndApply();
				}
				else
				{
					StartSettlementPlacement();
					applyPending = delegate
					{
						ConfirmAndApply();
					};
				}
				void ConfirmAndApply()
				{
					//IL_0047: Unknown result type (might be due to invalid IL or missing references)
					//IL_0051: Expected O, but got Unknown
					//IL_0035: Unknown result type (might be due to invalid IL or missing references)
					//IL_003f: Expected O, but got Unknown
					//IL_0023: Unknown result type (might be due to invalid IL or missing references)
					//IL_002d: Expected O, but got Unknown
					//IL_008e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0094: Expected O, but got Unknown
					//IL_0081: Unknown result type (might be due to invalid IL or missing references)
					//IL_0074: Unknown result type (might be due to invalid IL or missing references)
					//IL_0104: Unknown result type (might be due to invalid IL or missing references)
					//IL_0110: Expected O, but got Unknown
					string text2 = ((settlementType == SettlementType.Village) ? ((object)new TextObject("{=player_settlement_13}Build a Village", (Dictionary<string, object>)null)).ToString() : ((settlementType == SettlementType.Castle) ? ((object)new TextObject("{=player_settlement_19}Build a Castle", (Dictionary<string, object>)null)).ToString() : ((object)new TextObject("{=player_settlement_04}Build a Town", (Dictionary<string, object>)null)).ToString()));
					TextObject val10 = ((settlementType == SettlementType.Village) ? new TextObject("{=player_settlement_14}Are you sure you want to build your village here?", (Dictionary<string, object>)null) : ((settlementType == SettlementType.Castle) ? new TextObject("{=player_settlement_18}Are you sure you want to build your castle here?", (Dictionary<string, object>)null) : new TextObject("{=player_settlement_05}Are you sure you want to build your town here?", (Dictionary<string, object>)null)));
					InformationManager.ShowInquiry(new InquiryData(text2, ((object)val10).ToString(), true, true, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), ((object)GameTexts.FindText("str_cancel", (string)null)).ToString(), (Action)delegate
					{
						InformationManager.HideInquiry();
						ApplyPlaced(settlementName, culture, villageType2);
					}, (Action)delegate
					{
						InformationManager.HideInquiry();
						if (!Main.Settings.SettlementPlacement)
						{
							Reset();
							((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
						}
					}, "", 0f, (Action)null, (Func<ValueTuple<bool, string>>)null, (Func<ValueTuple<bool, string>>)null), true, false);
				}
			}
			void ApplyPlaced(string settlementName, CultureObject culture, string? villageType)
			{
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_042d: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0457: Unknown result type (might be due to invalid IL or missing references)
				//IL_045c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0465: Unknown result type (might be due to invalid IL or missing references)
				//IL_043c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0441: Unknown result type (might be due to invalid IL or missing references)
				//IL_0445: Unknown result type (might be due to invalid IL or missing references)
				//IL_046a: Unknown result type (might be due to invalid IL or missing references)
				//IL_046c: Unknown result type (might be due to invalid IL or missing references)
				//IL_046e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0805: Unknown result type (might be due to invalid IL or missing references)
				//IL_080f: Expected O, but got Unknown
				Village village = target.Village;
				Settlement val2 = ((village != null) ? village.Bound : null);
				if (currentModelOptionIdx < 0)
				{
					currentModelOptionIdx = new Random().Next(0, availableModels.Count);
				}
				ref MatrixFrame? reference = ref settlementPlacementFrame;
				MatrixFrame value;
				Vec2 val3;
				if (reference.HasValue)
				{
					reference.GetValueOrDefault();
					if (true)
					{
						value = settlementPlacementFrame.Value;
						val3 = (value.origin).AsVec2;
						goto IL_009f;
					}
				}
				val3 = MobileParty.MainParty.GetPosition2D;
				goto IL_009f;
				IL_03f7:
				Vec2 val4;
				if (portSupported && Main.IsWarSails)
				{
					ref MatrixFrame? reference2 = ref portPlacementFrame;
					if (reference2.HasValue)
					{
						reference2.GetValueOrDefault();
						if (true)
						{
							value = portPlacementFrame.Value;
							val4 = (value.origin).AsVec2;
							goto IL_046a;
						}
					}
					CampaignVec2 portPosition = target.PortPosition;
					val4 = (portPosition).ToVec2();
					goto IL_046a;
				}
				goto IL_0587;
				IL_02f3:
				Vec2 val5;
				Vec2 val6 = (Vec2)val5;
				XmlNode xmlNode;
				if (xmlNode.Attributes["gate_posX"] == null)
				{
					XmlAttribute xmlAttribute = xmlNode.OwnerDocument.CreateAttribute("gate_posX");
					xmlAttribute.Value = (val6).X.ToString();
					xmlNode.Attributes.SetNamedItem(xmlAttribute);
				}
				else
				{
					xmlNode.Attributes["gate_posX"].Value = (val6).X.ToString();
				}
				if (xmlNode.Attributes["gate_posY"] == null)
				{
					XmlAttribute xmlAttribute2 = xmlNode.OwnerDocument.CreateAttribute("gate_posY");
					xmlAttribute2.Value = (val6).Y.ToString();
					xmlNode.Attributes.SetNamedItem(xmlAttribute2);
				}
				else
				{
					xmlNode.Attributes["gate_posY"].Value = (val6).Y.ToString();
				}
				goto IL_03f7;
				IL_046a:
				Vec2 val7 = val4;
				if (val7 != Vec2.Invalid)
				{
					if (xmlNode.Attributes["port_posX"] == null)
					{
						XmlAttribute xmlAttribute3 = xmlNode.OwnerDocument.CreateAttribute("port_posX");
						xmlAttribute3.Value = (val7).X.ToString();
						xmlNode.Attributes.SetNamedItem(xmlAttribute3);
					}
					else
					{
						xmlNode.Attributes["port_posX"].Value = (val7).X.ToString();
					}
					if (xmlNode.Attributes["port_posY"] == null)
					{
						XmlAttribute xmlAttribute4 = xmlNode.OwnerDocument.CreateAttribute("port_posY");
						xmlAttribute4.Value = (val7).Y.ToString();
						xmlNode.Attributes.SetNamedItem(xmlAttribute4);
					}
					else
					{
						xmlNode.Attributes["port_posY"].Value = (val7).Y.ToString();
					}
				}
				goto IL_0587;
				IL_0587:
				TextObject encyclopediaText = target.EncyclopediaText;
				if (xmlNode.Attributes["text"] == null)
				{
					XmlAttribute xmlAttribute5 = xmlNode.OwnerDocument.CreateAttribute("text");
					xmlAttribute5.Value = ((object)encyclopediaText).ToString();
					xmlNode.Attributes.SetNamedItem(xmlAttribute5);
				}
				else
				{
					xmlNode.Attributes["text"].Value = ((object)encyclopediaText).ToString();
				}
				string text = "<Settlements>" + xmlNode.OuterXml + "</Settlements>";
				text = text.Replace("{{G_POS_X}}", (val6).X.ToString());
				text = text.Replace("{{G_POS_Y}}", (val6).Y.ToString());
				target.IsOverwritten(out OverwriteSettlementItem overwriteSettlementItem);
				if (overwriteSettlementItem == null)
				{
					overwriteSettlementItem = new OverwriteSettlementItem();
				}
				overwriteSettlementItem.ItemXML = text;
				overwriteSettlementItem.Type = 1;
				overwriteSettlementItem.SettlementName = settlementName;
				overwriteSettlementItem.RotationMat3 = settlementPlacementFrame?.rotation;
				overwriteSettlementItem.DeepEdits = new List<DeepTransformEdit>((deepEditPrefab == settlementVisualPrefab && deepTransformEdits != null) ? deepTransformEdits : new List<DeepTransformEdit>());
				overwriteSettlementItem.Version = Main.Version;
				string stringId;
				overwriteSettlementItem.StringId = stringId;
				PlayerSettlementItemTemplate playerSettlementItemTemplate;
				overwriteSettlementItem.PrefabId = playerSettlementItemTemplate.Id;
				if (PlayerSettlementInfo.Instance.OverwriteSettlements == null)
				{
					PlayerSettlementInfo.Instance.OverwriteSettlements = new List<OverwriteSettlementItem>();
				}
				if (!PlayerSettlementInfo.Instance.OverwriteSettlements.Contains(overwriteSettlementItem))
				{
					PlayerSettlementInfo.Instance.OverwriteSettlements.Add(overwriteSettlementItem);
				}
				else
				{
					LogManager.Log.ToFile($"Overwriting a previous overwrite settlement: {overwriteSettlementItem.Settlement} - {overwriteSettlementItem.StringId}", isDebug: true);
				}
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(text);
				MBObjectManager.Instance.LoadXml(xmlDocument, false);
				Settlement val8 = (overwriteSettlementItem.Settlement = MBObjectManager.Instance.GetObject<Settlement>(overwriteSettlementItem.StringId));
				if (settlementType == SettlementType.Village && val2 != null)
				{
					val8.SetBound(val2);
				}
				val8.SetName(new TextObject(settlementName, (Dictionary<string, object>)null));
				val8.Party.SetLevelMaskIsDirty();
				val8.IsVisible = true;
				val8.IsInspected = true;
				val8.Party.SetVisualAsDirty();
				overwriteSettlementItem.BuiltAt = Campaign.CurrentTime;
				overwriteSettlementItem.BuildComplete = false;
				switch (settlementType)
				{
				case SettlementType.Town:
					if (Main.Settings.RequireGold)
					{
						GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, val8, Main.Settings.RebuildTownRequiredGold, true);
					}
					break;
				case SettlementType.Village:
					if (Main.Settings.RequireVillageGold)
					{
						GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, val8, Main.Settings.RebuildVillageRequiredGold, true);
					}
					break;
				case SettlementType.Castle:
					if (Main.Settings.RequireCastleGold)
					{
						GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, val8, Main.Settings.RebuildCastleRequiredGold, true);
					}
					break;
				}
				_settlementOverwrite?.Invoke(val8);
				IMbEvent? onResetEvent = OnResetEvent;
				if (onResetEvent != null)
				{
					onResetEvent.ClearListeners((object)target);
				}
				SaveHandler.SaveLoad(Main.Settings.SaveMechanism);
				return;
				IL_009f:
				Vec2 val9 = val3;
				val6 = val9;
				playerSettlementItemTemplate = availableModels[currentModelOptionIdx];
				xmlNode = playerSettlementItemTemplate.ItemXML.CloneNode(deep: true);
				stringId = ((MBObjectBase)target).StringId;
				string stringId2 = ((MBObjectBase)target.SettlementComponent).StringId;
				xmlNode.Attributes["id"].Value = stringId;
				xmlNode.Attributes["posX"].Value = (val9).X.ToString();
				xmlNode.Attributes["posY"].Value = (val9).Y.ToString();
				xmlNode.Attributes["name"].Value = settlementName;
				xmlNode.Attributes["culture"].Value = "Culture." + ((MBObjectBase)culture).StringId;
				if (xmlNode.Attributes["owner"] != null)
				{
					xmlNode.Attributes["owner"].Value = "Faction." + ((MBObjectBase)Hero.MainHero.Clan).StringId;
				}
				XmlNode xmlNode2 = xmlNode.SelectSingleNode((settlementType == SettlementType.Village) ? "descendant::Village" : "descendant::Town");
				xmlNode2.Attributes["id"].Value = stringId2;
				if (settlementType != SettlementType.Village)
				{
					if (gateSupported && Main.Settings.AllowGatePosition && gatePlacementFrame.HasValue)
					{
						ref MatrixFrame? reference3 = ref gatePlacementFrame;
						if (reference3.HasValue)
						{
							reference3.GetValueOrDefault();
							if (true)
							{
								value = gatePlacementFrame.Value;
								val5 = (value.origin).AsVec2;
								goto IL_02f3;
							}
						}
						val5 = val6;
						goto IL_02f3;
					}
					goto IL_03f7;
				}
				xmlNode2.Attributes["village_type"].Value = "VillageType." + villageType;
				xmlNode2.Attributes["bound"].Value = "Settlement." + ((val2 != null) ? ((MBObjectBase)val2).StringId : null);
				goto IL_0587;
			}
		}

		private void Rebuild(PlayerSettlementItem target)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Expected O, but got Unknown
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected O, but got Unknown
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Expected O, but got Unknown
			SettlementType settlementType = target.GetSettlementType();
			gateSupported = settlementType != SettlementType.Village;
			portSupported = Main.IsWarSails && settlementType == SettlementType.Town;
			PlacementSupported = false;
			InformationManager.ShowTextInquiry(new TextInquiryData(((object)new TextObject("{=player_settlement_39}Rebuild Player Settlement", (Dictionary<string, object>)null)).ToString(), ((object)new TextObject("{=player_settlement_03}What would you like to name your settlement?", (Dictionary<string, object>)null)).ToString(), true, true, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), ((object)GameTexts.FindText("str_cancel", (string)null)).ToString(), (Action<string>)delegate(string settlementName)
			{
				//IL_0147: Unknown result type (might be due to invalid IL or missing references)
				//IL_014d: Expected O, but got Unknown
				//IL_013a: Unknown result type (might be due to invalid IL or missing references)
				//IL_012d: Unknown result type (might be due to invalid IL or missing references)
				//IL_017f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0185: Expected O, but got Unknown
				//IL_0172: Unknown result type (might be due to invalid IL or missing references)
				//IL_0165: Unknown result type (might be due to invalid IL or missing references)
				//IL_0225: Unknown result type (might be due to invalid IL or missing references)
				//IL_0231: Expected O, but got Unknown
				Campaign.Current.TimeControlMode = (CampaignTimeControlMode)0;
				if (string.IsNullOrEmpty(settlementName))
				{
					settlementName = target.SettlementName;
				}
				if (Main.Settings.ForcePlayerCulture)
				{
					if (Main.Settings.AutoAllocateVillageType || settlementType != SettlementType.Village)
					{
						Apply(settlementName, Hero.MainHero.Culture, (settlementType == SettlementType.Village) ? AutoCalculateVillageType(target.Identifier) : null);
					}
					else
					{
						PlayerSettlementBehaviour playerSettlementBehaviour = this;
						string settlementName2 = settlementName;
						CultureObject culture2 = Hero.MainHero.Culture;
						Settlement? settlement2 = target.Settlement;
						object bound;
						if (settlement2 == null)
						{
							bound = null;
						}
						else
						{
							Village village2 = settlement2.Village;
							bound = ((village2 != null) ? village2.Bound : null);
						}
						playerSettlementBehaviour.DetermineVillageType(settlementName2, culture2, (Settlement?)bound, target.Identifier, Apply);
					}
				}
				else
				{
					TextObject val10 = ((settlementType == SettlementType.Castle) ? new TextObject("{=player_settlement_20}Choose castle culture", (Dictionary<string, object>)null) : ((settlementType == SettlementType.Village) ? new TextObject("{=player_settlement_11}Choose village culture", (Dictionary<string, object>)null) : new TextObject("{=player_settlement_09}Choose town culture", (Dictionary<string, object>)null)));
					TextObject val11 = ((settlementType == SettlementType.Castle) ? new TextObject("{=player_settlement_21}Choose the culture for {CASTLE}", (Dictionary<string, object>)null) : ((settlementType == SettlementType.Village) ? new TextObject("{=player_settlement_12}Choose the culture for {VILLAGE}", (Dictionary<string, object>)null) : new TextObject("{=player_settlement_10}Choose the culture for {TOWN}", (Dictionary<string, object>)null)));
					val11.SetTextVariable("CASTLE", settlementName);
					val11.SetTextVariable("TOWN", settlementName);
					val11.SetTextVariable("VILLAGE", settlementName);
					List<InquiryElement> list = GetCultures(mainOnly: true).Select((Func<CultureObject, InquiryElement>)((CultureObject c) => new InquiryElement((object)c, ((object)((BasicCultureObject)c).Name).ToString(), (ImageIdentifier)new BannerImageIdentifier(new Banner(((BasicCultureObject)c).Banner), false), true, ((object)((BasicCultureObject)c).Name).ToString()))).ToList();
					MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(((object)val10).ToString(), ((object)val11).ToString(), list, false, 1, 1, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), (string)null, (Action<List<InquiryElement>>)delegate(List<InquiryElement> args)
					{
						object obj2 = args?.FirstOrDefault()?.Identifier;
						CultureObject culture3 = (CultureObject)(((obj2 is CultureObject) ? obj2 : null) ?? Hero.MainHero.Culture);
						if (Main.Settings.AutoAllocateVillageType || settlementType != SettlementType.Village)
						{
							Apply(settlementName, culture3, AutoCalculateVillageType(target.Identifier));
						}
						else
						{
							PlayerSettlementBehaviour playerSettlementBehaviour2 = this;
							string settlementName3 = settlementName;
							Settlement? settlement3 = target.Settlement;
							object bound2;
							if (settlement3 == null)
							{
								bound2 = null;
							}
							else
							{
								Village village3 = settlement3.Village;
								bound2 = ((village3 != null) ? village3.Bound : null);
							}
							playerSettlementBehaviour2.DetermineVillageType(settlementName3, culture3, (Settlement?)bound2, target.Identifier, Apply);
						}
					}, (Action<List<InquiryElement>>)null, "", false), false, false);
				}
			}, (Action)delegate
			{
				InformationManager.HideInquiry();
				Reset();
				((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
			}, false, (Func<string, Tuple<bool, string>>)FactionHelper.IsKingdomNameApplicable, "", target.SettlementName), true, false);
			void Apply(string settlementName, CultureObject culture, string? villageType)
			{
				string villageType2 = villageType;
				settlementPlacementFrame = null;
				availableModels?.Clear();
				switch (settlementType)
				{
				case SettlementType.Village:
					if (Main.Settings.SelectedCultureOnly && Main.Submodule.CultureTemplates.ContainsKey(((MBObjectBase)culture).StringId))
					{
						availableModels = Main.Submodule.CultureTemplates[((MBObjectBase)culture).StringId].SelectMany((CultureSettlementTemplate cst) => SelectVillageTemplates(cst, target.Settlement.Village.Bound.IsCastle)).ToList();
						currentModelOptionIdx = -1;
					}
					if (availableModels == null || availableModels.Count == 0)
					{
						availableModels = Main.Submodule.CultureTemplates.Values.SelectMany((List<CultureSettlementTemplate> c) => c.SelectMany((CultureSettlementTemplate cst) => SelectVillageTemplates(cst, target.Settlement.Village.Bound.IsCastle))).ToList();
						currentModelOptionIdx = availableModels.FindIndex((PlayerSettlementItemTemplate a) => a.Culture == ((MBObjectBase)culture).StringId) - 1;
					}
					break;
				case SettlementType.Castle:
					if (Main.Settings.SelectedCultureOnly && Main.Submodule.CultureTemplates.ContainsKey(((MBObjectBase)culture).StringId))
					{
						availableModels = Main.Submodule.CultureTemplates[((MBObjectBase)culture).StringId].SelectMany(SelectCastleTemplates).ToList();
						currentModelOptionIdx = -1;
					}
					if (availableModels == null || availableModels.Count == 0)
					{
						availableModels = Main.Submodule.CultureTemplates.Values.SelectMany((List<CultureSettlementTemplate> c) => c.SelectMany(SelectCastleTemplates)).ToList();
						currentModelOptionIdx = availableModels.FindIndex((PlayerSettlementItemTemplate a) => a.Culture == ((MBObjectBase)culture).StringId) - 1;
					}
					break;
				case SettlementType.Town:
					if (Main.Settings.SelectedCultureOnly && Main.Submodule.CultureTemplates.ContainsKey(((MBObjectBase)culture).StringId))
					{
						availableModels = Main.Submodule.CultureTemplates[((MBObjectBase)culture).StringId].SelectMany(SelectTownTemplates).ToList();
						currentModelOptionIdx = -1;
					}
					if (availableModels == null || availableModels.Count == 0)
					{
						availableModels = Main.Submodule.CultureTemplates.Values.SelectMany((List<CultureSettlementTemplate> c) => c.SelectMany(SelectTownTemplates)).ToList();
						currentModelOptionIdx = availableModels.FindIndex((PlayerSettlementItemTemplate a) => a.Culture == ((MBObjectBase)culture).StringId) - 1;
					}
					break;
				default:
					InformationManager.HideInquiry();
					if (!Main.Settings.SettlementPlacement)
					{
						Reset();
						((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
					}
					return;
				}
				string curModelPrefab = ((!string.IsNullOrEmpty(target.PrefabId)) ? target.PrefabId : ((!string.IsNullOrEmpty(target.StringId)) ? target.StringId : ((MBObjectBase)target.Settlement).StringId));
				int num = availableModels?.FindIndex((PlayerSettlementItemTemplate a) => a.Id == curModelPrefab) ?? (-1);
				if (num >= 0)
				{
					currentModelOptionIdx = num - 1;
					settlementVisualPrefab = (deepEditPrefab = curModelPrefab);
					deepTransformEdits = new List<DeepTransformEdit>(target.DeepEdits ?? new List<DeepTransformEdit>());
				}
				if (!Main.Settings.SettlementPlacement)
				{
					ConfirmAndApply();
				}
				else
				{
					StartSettlementPlacement();
					applyPending = delegate
					{
						ConfirmAndApply();
					};
				}
				void ConfirmAndApply()
				{
					//IL_0047: Unknown result type (might be due to invalid IL or missing references)
					//IL_0051: Expected O, but got Unknown
					//IL_0035: Unknown result type (might be due to invalid IL or missing references)
					//IL_003f: Expected O, but got Unknown
					//IL_0023: Unknown result type (might be due to invalid IL or missing references)
					//IL_002d: Expected O, but got Unknown
					//IL_008e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0094: Expected O, but got Unknown
					//IL_0081: Unknown result type (might be due to invalid IL or missing references)
					//IL_0074: Unknown result type (might be due to invalid IL or missing references)
					//IL_0104: Unknown result type (might be due to invalid IL or missing references)
					//IL_0110: Expected O, but got Unknown
					string text2 = ((settlementType == SettlementType.Village) ? ((object)new TextObject("{=player_settlement_13}Build a Village", (Dictionary<string, object>)null)).ToString() : ((settlementType == SettlementType.Castle) ? ((object)new TextObject("{=player_settlement_19}Build a Castle", (Dictionary<string, object>)null)).ToString() : ((object)new TextObject("{=player_settlement_04}Build a Town", (Dictionary<string, object>)null)).ToString()));
					TextObject val9 = ((settlementType == SettlementType.Village) ? new TextObject("{=player_settlement_14}Are you sure you want to build your village here?", (Dictionary<string, object>)null) : ((settlementType == SettlementType.Castle) ? new TextObject("{=player_settlement_18}Are you sure you want to build your castle here?", (Dictionary<string, object>)null) : new TextObject("{=player_settlement_05}Are you sure you want to build your town here?", (Dictionary<string, object>)null)));
					InformationManager.ShowInquiry(new InquiryData(text2, ((object)val9).ToString(), true, true, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), ((object)GameTexts.FindText("str_cancel", (string)null)).ToString(), (Action)delegate
					{
						InformationManager.HideInquiry();
						ApplyPlaced(settlementName, culture, villageType2);
					}, (Action)delegate
					{
						InformationManager.HideInquiry();
						if (!Main.Settings.SettlementPlacement)
						{
							Reset();
							((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
						}
					}, "", 0f, (Action)null, (Func<ValueTuple<bool, string>>)null, (Func<ValueTuple<bool, string>>)null), true, false);
				}
			}
			void ApplyPlaced(string settlementName, CultureObject culture, string villageType)
			{
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_035b: Unknown result type (might be due to invalid IL or missing references)
				//IL_064d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0654: Expected O, but got Unknown
				//IL_0372: Unknown result type (might be due to invalid IL or missing references)
				//IL_0377: Unknown result type (might be due to invalid IL or missing references)
				//IL_0380: Unknown result type (might be due to invalid IL or missing references)
				//IL_0364: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0385: Unknown result type (might be due to invalid IL or missing references)
				//IL_0756: Unknown result type (might be due to invalid IL or missing references)
				//IL_075b: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_04da: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0501: Unknown result type (might be due to invalid IL or missing references)
				//IL_0503: Unknown result type (might be due to invalid IL or missing references)
				//IL_084e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0858: Expected O, but got Unknown
				Settlement? settlement = target.Settlement;
				object obj;
				if (settlement == null)
				{
					obj = null;
				}
				else
				{
					Village village = settlement.Village;
					obj = ((village != null) ? village.Bound : null);
				}
				Settlement val = (Settlement)obj;
				if (currentModelOptionIdx < 0)
				{
					currentModelOptionIdx = new Random().Next(0, availableModels.Count);
				}
				ref MatrixFrame? reference = ref settlementPlacementFrame;
				MatrixFrame value;
				Vec2 val2;
				if (reference.HasValue)
				{
					reference.GetValueOrDefault();
					if (true)
					{
						value = settlementPlacementFrame.Value;
						val2 = (value.origin).AsVec2;
						goto IL_00ab;
					}
				}
				val2 = MobileParty.MainParty.GetPosition2D;
				goto IL_00ab;
				IL_061c:
				XmlNode xmlNode;
				TextObject val3 = new TextObject((xmlNode.Attributes["text"] != null) ? xmlNode.Attributes["text"].Value : "", (Dictionary<string, object>)null);
				XmlNode xmlNode2;
				if (xmlNode2.Attributes["text"] == null)
				{
					XmlAttribute xmlAttribute = xmlNode2.OwnerDocument.CreateAttribute("text");
					xmlAttribute.Value = ((object)val3).ToString();
					xmlNode2.Attributes.SetNamedItem(xmlAttribute);
				}
				else
				{
					xmlNode2.Attributes["text"].Value = ((object)val3).ToString();
				}
				string text = "<Settlements>" + xmlNode2.OuterXml + "</Settlements>";
				Vec2 val4;
				text = text.Replace("{{G_POS_X}}", (val4).X.ToString());
				text = text.Replace("{{G_POS_Y}}", (val4).Y.ToString());
				target.ItemXML = text;
				target.SettlementName = settlementName;
				target.RotationMat3 = settlementPlacementFrame?.rotation;
				target.DeepEdits = new List<DeepTransformEdit>((deepEditPrefab == settlementVisualPrefab && deepTransformEdits != null) ? deepTransformEdits : new List<DeepTransformEdit>());
				target.Version = Main.Version;
				PlayerSettlementItemTemplate playerSettlementItemTemplate;
				target.PrefabId = playerSettlementItemTemplate.Id;
				string value2;
				target.StringId = value2;
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(text);
				MBObjectManager.Instance.LoadXml(xmlDocument, false);
				Settlement @object = MBObjectManager.Instance.GetObject<Settlement>(target.StringId);
				target.Settlement = @object;
				if (settlementType == SettlementType.Village && val != null)
				{
					@object.SetBound(val);
				}
				@object.SetName(new TextObject(settlementName, (Dictionary<string, object>)null));
				@object.Party.SetLevelMaskIsDirty();
				@object.IsVisible = true;
				@object.IsInspected = true;
				@object.Party.SetVisualAsDirty();
				target.IsRebuild = target.BuildComplete;
				target.BuiltAt = Campaign.CurrentTime;
				target.BuildComplete = false;
				switch (settlementType)
				{
				case SettlementType.Town:
					if (Main.Settings.RequireGold)
					{
						GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, @object, Main.Settings.RebuildTownRequiredGold, true);
					}
					break;
				case SettlementType.Village:
					if (Main.Settings.RequireVillageGold)
					{
						GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, @object, Main.Settings.RebuildVillageRequiredGold, true);
					}
					break;
				case SettlementType.Castle:
					if (Main.Settings.RequireCastleGold)
					{
						GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, @object, Main.Settings.RebuildCastleRequiredGold, true);
					}
					break;
				}
				_settlementRebuild?.Invoke(@object);
				IMbEvent? onResetEvent = OnResetEvent;
				if (onResetEvent != null)
				{
					onResetEvent.ClearListeners((object)target);
				}
				SaveHandler.SaveLoad(Main.Settings.SaveMechanism);
				return;
				IL_0385:
				Vec2 val5;
				val4 = (Vec2)val5;
				if (xmlNode2.Attributes["gate_posX"] == null)
				{
					XmlAttribute xmlAttribute2 = xmlNode2.OwnerDocument.CreateAttribute("gate_posX");
					xmlAttribute2.Value = (val4).X.ToString();
					xmlNode2.Attributes.SetNamedItem(xmlAttribute2);
				}
				else
				{
					xmlNode2.Attributes["gate_posX"].Value = (val4).X.ToString();
				}
				if (xmlNode2.Attributes["gate_posY"] == null)
				{
					XmlAttribute xmlAttribute3 = xmlNode2.OwnerDocument.CreateAttribute("gate_posY");
					xmlAttribute3.Value = (val4).Y.ToString();
					xmlNode2.Attributes.SetNamedItem(xmlAttribute3);
				}
				else
				{
					xmlNode2.Attributes["gate_posY"].Value = (val4).Y.ToString();
				}
				goto IL_0489;
				IL_0489:
				Vec2 val6;
				if (portSupported && Main.IsWarSails && portPlacementFrame.HasValue)
				{
					ref MatrixFrame? reference2 = ref portPlacementFrame;
					if (reference2.HasValue)
					{
						reference2.GetValueOrDefault();
						if (true)
						{
							value = portPlacementFrame.Value;
							val6 = (value.origin).AsVec2;
							goto IL_04ff;
						}
					}
					val6 = Vec2.Invalid;
					goto IL_04ff;
				}
				goto IL_061c;
				IL_00ab:
				Vec2 val7 = val2;
				val4 = val7;
				playerSettlementItemTemplate = availableModels[currentModelOptionIdx];
				XmlDocument xmlDocument2 = new XmlDocument();
				xmlDocument2.LoadXml(target.ItemXML);
				xmlNode = xmlDocument2.ChildNodes[0].ChildNodes.OfType<XmlNode>().FirstOrDefault((XmlNode n) => !(n is XmlComment));
				xmlNode2 = playerSettlementItemTemplate.ItemXML.CloneNode(deep: true);
				value2 = xmlNode.Attributes["id"].Value;
				XmlNode xmlNode3 = xmlNode.SelectSingleNode((settlementType == SettlementType.Village) ? "descendant::Village" : "descendant::Town");
				string value3 = xmlNode3.Attributes["id"].Value;
				xmlNode2.Attributes["id"].Value = value2;
				xmlNode2.Attributes["posX"].Value = (val7).X.ToString();
				xmlNode2.Attributes["posY"].Value = (val7).Y.ToString();
				xmlNode2.Attributes["name"].Value = settlementName;
				xmlNode2.Attributes["culture"].Value = "Culture." + ((MBObjectBase)culture).StringId;
				if (xmlNode2.Attributes["owner"] != null)
				{
					xmlNode2.Attributes["owner"].Value = "Faction." + ((MBObjectBase)Hero.MainHero.Clan).StringId;
				}
				XmlNode xmlNode4 = xmlNode2.SelectSingleNode((settlementType == SettlementType.Village) ? "descendant::Village" : "descendant::Town");
				xmlNode4.Attributes["id"].Value = value3;
				if (settlementType != SettlementType.Village)
				{
					if (gateSupported && Main.Settings.AllowGatePosition && gatePlacementFrame.HasValue)
					{
						ref MatrixFrame? reference3 = ref gatePlacementFrame;
						if (reference3.HasValue)
						{
							reference3.GetValueOrDefault();
							if (true)
							{
								value = gatePlacementFrame.Value;
								val5 = (value.origin).AsVec2;
								goto IL_0385;
							}
						}
						val5 = val4;
						goto IL_0385;
					}
					goto IL_0489;
				}
				xmlNode4.Attributes["village_type"].Value = "VillageType." + villageType;
				xmlNode4.Attributes["bound"].Value = "Settlement." + ((val != null) ? ((MBObjectBase)val).StringId : null);
				goto IL_061c;
				IL_04ff:
				Vec2 val8 = val6;
				if (val8 != Vec2.Invalid)
				{
					if (xmlNode2.Attributes["port_posX"] == null)
					{
						XmlAttribute xmlAttribute4 = xmlNode2.OwnerDocument.CreateAttribute("port_posX");
						xmlAttribute4.Value = (val8).X.ToString();
						xmlNode2.Attributes.SetNamedItem(xmlAttribute4);
					}
					else
					{
						xmlNode2.Attributes["port_posX"].Value = (val8).X.ToString();
					}
					if (xmlNode2.Attributes["port_posY"] == null)
					{
						XmlAttribute xmlAttribute5 = xmlNode2.OwnerDocument.CreateAttribute("port_posY");
						xmlAttribute5.Value = (val8).Y.ToString();
						xmlNode2.Attributes.SetNamedItem(xmlAttribute5);
					}
					else
					{
						xmlNode2.Attributes["port_posY"].Value = (val8).Y.ToString();
					}
				}
				goto IL_061c;
			}
		}

		private void BuildCastle()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Expected O, but got Unknown
			PlacementSupported = false;
			gateSupported = true;
			Campaign.Current.TimeControlMode = (CampaignTimeControlMode)0;
			InformationManager.ShowTextInquiry(new TextInquiryData(((object)new TextObject("{=player_settlement_02}Create Player Settlement", (Dictionary<string, object>)null)).ToString(), ((object)new TextObject("{=player_settlement_03}What would you like to name your settlement?", (Dictionary<string, object>)null)).ToString(), true, true, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), ((object)GameTexts.FindText("str_cancel", (string)null)).ToString(), (Action<string>)delegate(string settlementName)
			{
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Expected O, but got Unknown
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Expected O, but got Unknown
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Expected O, but got Unknown
				//IL_0108: Unknown result type (might be due to invalid IL or missing references)
				//IL_0114: Expected O, but got Unknown
				Campaign.Current.TimeControlMode = (CampaignTimeControlMode)0;
				if (string.IsNullOrEmpty(settlementName))
				{
					settlementName = ((object)new TextObject("{=player_settlement_n_01}Player Settlement", (Dictionary<string, object>)null)).ToString();
				}
				if (Main.Settings.ForcePlayerCulture)
				{
					Apply(settlementName, Hero.MainHero.Culture);
				}
				else
				{
					TextObject val4 = new TextObject("{=player_settlement_20}Choose castle culture", (Dictionary<string, object>)null);
					TextObject val5 = new TextObject("{=player_settlement_21}Choose the culture for {CASTLE}", (Dictionary<string, object>)null);
					val5.SetTextVariable("CASTLE", settlementName);
					List<InquiryElement> list = GetCultures(mainOnly: true).Select((Func<CultureObject, InquiryElement>)((CultureObject c) => new InquiryElement((object)c, ((object)((BasicCultureObject)c).Name).ToString(), (ImageIdentifier)new BannerImageIdentifier(new Banner(((BasicCultureObject)c).Banner), false), true, ((object)((BasicCultureObject)c).Name).ToString()))).ToList();
					MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(((object)val4).ToString(), ((object)val5).ToString(), list, false, 1, 1, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), (string)null, (Action<List<InquiryElement>>)delegate(List<InquiryElement> args)
					{
						object obj2 = args?.FirstOrDefault()?.Identifier;
						CultureObject culture2 = (CultureObject)(((obj2 is CultureObject) ? obj2 : null) ?? Hero.MainHero.Culture);
						Apply(settlementName, culture2);
					}, (Action<List<InquiryElement>>)null, "", false), false, false);
				}
			}, (Action)delegate
			{
				InformationManager.HideInquiry();
				Reset();
				((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
			}, false, (Func<string, Tuple<bool, string>>)FactionHelper.IsKingdomNameApplicable, "", ""), true, false);
			void Apply(string settlementName, CultureObject culture)
			{
				settlementPlacementFrame = null;
				availableModels?.Clear();
				if (Main.Settings.SelectedCultureOnly && Main.Submodule.CultureTemplates.ContainsKey(((MBObjectBase)culture).StringId))
				{
					availableModels = Main.Submodule.CultureTemplates[((MBObjectBase)culture).StringId].SelectMany(SelectCastleTemplates).ToList();
					currentModelOptionIdx = -1;
				}
				if (availableModels == null || availableModels.Count == 0)
				{
					availableModels = Main.Submodule.CultureTemplates.Values.SelectMany((List<CultureSettlementTemplate> c) => c.SelectMany(SelectCastleTemplates)).ToList();
					currentModelOptionIdx = availableModels.FindIndex((PlayerSettlementItemTemplate a) => a.Culture == ((MBObjectBase)culture).StringId) - 1;
				}
				if (!Main.Settings.SettlementPlacement)
				{
					ConfirmAndApply();
				}
				else
				{
					StartSettlementPlacement();
					applyPending = delegate
					{
						ConfirmAndApply();
					};
				}
				void ConfirmAndApply()
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0011: Expected O, but got Unknown
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_001e: Expected O, but got Unknown
					//IL_0071: Unknown result type (might be due to invalid IL or missing references)
					//IL_007d: Expected O, but got Unknown
					string text = ((object)new TextObject("{=player_settlement_19}Build a Castle", (Dictionary<string, object>)null)).ToString();
					TextObject val3 = new TextObject("{=player_settlement_18}Are you sure you want to build your castle here?", (Dictionary<string, object>)null);
					InformationManager.ShowInquiry(new InquiryData(text, ((object)val3).ToString(), true, true, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), ((object)GameTexts.FindText("str_cancel", (string)null)).ToString(), (Action)delegate
					{
						InformationManager.HideInquiry();
						ApplyPlaced(settlementName, culture);
					}, (Action)delegate
					{
						InformationManager.HideInquiry();
						if (!Main.Settings.SettlementPlacement)
						{
							Reset();
							((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
						}
					}, "", 0f, (Action)null, (Func<ValueTuple<bool, string>>)null, (Func<ValueTuple<bool, string>>)null), true, false);
				}
			}
			void ApplyPlaced(string settlementName, CultureObject culture)
			{
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Expected O, but got Unknown
				PlayerSettlementItem castleItem;
				Settlement val = CreateCastle(settlementName, culture, out castleItem);
				val.Town.OwnerClan = Hero.MainHero.Clan;
				val.SetName(new TextObject(settlementName, (Dictionary<string, object>)null));
				val.Party.SetLevelMaskIsDirty();
				val.IsVisible = true;
				val.IsInspected = true;
				((Fief)val.Town).FoodStocks = val.Town.FoodStocksUpperLimit();
				val.Party.SetVisualAsDirty();
				SettlementVisualManager.Current.AddNewPartyVisualForParty(val.Party);
				val.OnGameCreated();
				((MBObjectBase)val).AfterInitialized();
				val.OnFinishLoadState();
				Town town = val.Town;
				InitCastleBuildings(val);
				castleItem.BuiltAt = Campaign.CurrentTime;
				if (Main.Settings.RequireCastleGold)
				{
					GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, val, Main.Settings.RequiredCastleGold, true);
				}
				else
				{
					val.SettlementComponent.ChangeGold(3000);
				}
				if (Main.Settings.AddInitialGarrison)
				{
					val.AddGarrisonParty();
					GarrisonTroopsCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<GarrisonTroopsCampaignBehavior>();
					if (campaignBehavior != null && val.Town != null)
					{
						FillGarrisonPartyOnNewGameInvoker(campaignBehavior, val.Town);
					}
					val.SetGarrisonWagePaymentLimit(Campaign.Current.Models.PartyWageModel.MaxWagePaymentLimit);
				}
				if (Main.Settings.AddInitialMilitia)
				{
					val.Militia = val.Town.MilitiaChange * 45f;
				}
				CampaignGameStarter gameStarter = SandBoxManager.Instance.GameStarter;
				CampaignBehaviorBase obj = gameStarter.CampaignBehaviors.FirstOrDefault((CampaignBehaviorBase b) => b is RecruitmentCampaignBehavior);
				RecruitmentCampaignBehavior val2 = (RecruitmentCampaignBehavior)(object)((obj is RecruitmentCampaignBehavior) ? obj : null);
				if (val2 != null)
				{
					RecruitmentCampaignBehavior recruitmentCampaignBehavior = val2;
					if (true)
					{
						recruitmentCampaignBehavior.NewSettlementBuilt(val);
					}
				}
				_settlementCreated.Invoke(val);
				SaveHandler.SaveLoad(Main.Settings.SaveMechanism);
			}
		}

		private List<PlayerSettlementItemTemplate> SelectCastleTemplates(CultureSettlementTemplate cst)
		{
			List<PlayerSettlementItemTemplate> list = new List<PlayerSettlementItemTemplate>();
			try
			{
				IEnumerable<XmlNode> enumerable = cst.Document.SelectNodes("descendant::Settlement[@template_type='Castle']").OfType<XmlNode>();
				foreach (XmlNode item in enumerable)
				{
					string value = item.Attributes["id"].Value;
					if (((List<string>)(object)Main.BlacklistedTemplates).Contains(value))
					{
						LogManager.EventTracer.Trace("Skipped blacklisted template: " + value);
						continue;
					}
					list.Add(new PlayerSettlementItemTemplate
					{
						Id = value,
						ItemXML = item,
						Type = 3,
						Culture = cst.CultureId
					});
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return list;
		}

		private static void InitCastleBuildings(Settlement castleSettlement)
		{
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Expected O, but got Unknown
			Town town = castleSettlement.Town;
			int val = MBRandom.RandomInt(1, 4);
			int num = 1;
			foreach (BuildingType all2 in (List<BuildingType>)(object)BuildingType.All)
			{
				if (!((IEnumerable<Building>)town.Buildings).Any((Building k) => ((MBObjectBase)k.BuildingType).StringId == ((MBObjectBase)all2).StringId) && ((MBObjectBase)all2).StringId.StartsWith("building_castle"))
				{
					((List<Building>)(object)town.Buildings).Add(new Building(all2, town, 0f, Math.Max(val, all2.StartLevel)));
				}
			}
			foreach (Building item in ((IEnumerable<Building>)town.Buildings).OrderByDescending((Building k) => k.CurrentLevel))
			{
				if (item.CurrentLevel != 3 && item.CurrentLevel != item.BuildingType.StartLevel && !item.BuildingType.IsDailyProject)
				{
					town.BuildingsInProgress.Enqueue(item);
				}
			}
			Building val2 = ((IEnumerable<Building>)castleSettlement.Town.Buildings).FirstOrDefault((Building b) => b.BuildingType.IsDailyProject && b.IsCurrentlyDefault);
			if (val2 == null)
			{
				val2 = ((IEnumerable<Building>)castleSettlement.Town.Buildings).FirstOrDefault((Building b) => b.BuildingType.IsDailyProject);
				BuildingHelper.ChangeDefaultBuilding(val2, castleSettlement.Town);
				val2.IsCurrentlyDefault = true;
			}
			if (val2 != null)
			{
				val2.IsCurrentlyDefault = true;
			}
		}

		private Settlement CreateCastle(string settlementName, CultureObject culture, out PlayerSettlementItem castleItem)
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Expected O, but got Unknown
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Expected O, but got Unknown
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			if (currentModelOptionIdx < 0)
			{
				currentModelOptionIdx = new Random().Next(0, availableModels.Count);
			}
			if (PlayerSettlementInfo.Instance.Castles == null)
			{
				PlayerSettlementInfo.Instance.Castles = new List<PlayerSettlementItem>();
			}
			ref MatrixFrame? reference = ref settlementPlacementFrame;
			MatrixFrame value;
			Vec2 val;
			if (reference.HasValue)
			{
				reference.GetValueOrDefault();
				if (true)
				{
					value = settlementPlacementFrame.Value;
					val = (value.origin).AsVec2;
					goto IL_0098;
				}
			}
			val = MobileParty.MainParty.GetPosition2D;
			goto IL_0098;
			IL_02a5:
			XmlNode xmlNode;
			Vec2 val2;
			xmlNode.Attributes["posX"].Value = (val2).X.ToString();
			xmlNode.Attributes["posY"].Value = (val2).Y.ToString();
			xmlNode.Attributes["name"].Value = settlementName;
			xmlNode.Attributes["owner"].Value = "Faction." + ((MBObjectBase)Hero.MainHero.Clan).StringId;
			xmlNode.Attributes["culture"].Value = "Culture." + ((MBObjectBase)culture).StringId;
			TextObject val3 = new TextObject("{=player_settlement_24}{SETTLEMENT_NAME} was founded by {HERO_NAME} of the {FACTION_TERM} on {BUILD_TIME}", (Dictionary<string, object>)null);
			string text;
			val3.SetTextVariable("SETTLEMENT_NAME", PlayerSettlementItem.EncyclopediaLinkWithName(text, new TextObject(settlementName, (Dictionary<string, object>)null)));
			val3.SetTextVariable("HERO_NAME", Hero.MainHero.EncyclopediaLinkWithName);
			val3.SetTextVariable("FACTION_TERM", Hero.MainHero.Clan.EncyclopediaLinkWithName);
			CampaignTime now = CampaignTime.Now;
			val3.SetTextVariable("BUILD_TIME", ((object)now).ToString());
			if (xmlNode.Attributes["text"] == null)
			{
				XmlAttribute xmlAttribute = xmlNode.OwnerDocument.CreateAttribute("text");
				xmlAttribute.Value = ((object)val3).ToString();
				xmlNode.Attributes.SetNamedItem(xmlAttribute);
			}
			else
			{
				xmlNode.Attributes["text"].Value = ((object)val3).ToString();
			}
			string text2 = "<Settlements>" + xmlNode.OuterXml + "</Settlements>";
			Vec2 val4;
			text2 = text2.Replace("{{G_POS_X}}", (val4).X.ToString());
			text2 = text2.Replace("{{G_POS_Y}}", (val4).Y.ToString());
			int identifier;
			PlayerSettlementItemTemplate playerSettlementItemTemplate;
			castleItem = new PlayerSettlementItem
			{
				ItemXML = text2,
				Identifier = identifier,
				Type = 3,
				SettlementName = settlementName,
				RotationMat3 = settlementPlacementFrame?.rotation,
				DeepEdits = new List<DeepTransformEdit>((deepEditPrefab == settlementVisualPrefab && deepTransformEdits != null) ? deepTransformEdits : new List<DeepTransformEdit>()),
				Version = Main.Version,
				StringId = text,
				PrefabId = playerSettlementItemTemplate.Id
			};
			PlayerSettlementInfo.Instance.Castles.Add(castleItem);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(text2);
			Campaign.Current.AsCampaignGameLoadingType((GameLoadingType)1, delegate
			{
				MBObjectManager.Instance.LoadXml(doc, false);
				return true;
			});
			Settlement @object = MBObjectManager.Instance.GetObject<Settlement>(castleItem.StringId);
			castleItem.Settlement = @object;
			return @object;
			IL_01a1:
			Vec2 val5;
			val4 = (Vec2)val5;
			if (xmlNode.Attributes["gate_posX"] == null)
			{
				XmlAttribute xmlAttribute2 = xmlNode.OwnerDocument.CreateAttribute("gate_posX");
				xmlAttribute2.Value = (val4).X.ToString();
				xmlNode.Attributes.SetNamedItem(xmlAttribute2);
			}
			else
			{
				xmlNode.Attributes["gate_posX"].Value = (val4).X.ToString();
			}
			if (xmlNode.Attributes["gate_posY"] == null)
			{
				XmlAttribute xmlAttribute3 = xmlNode.OwnerDocument.CreateAttribute("gate_posY");
				xmlAttribute3.Value = (val4).Y.ToString();
				xmlNode.Attributes.SetNamedItem(xmlAttribute3);
			}
			else
			{
				xmlNode.Attributes["gate_posY"].Value = (val4).Y.ToString();
			}
			goto IL_02a5;
			IL_0098:
			val2 = val;
			identifier = PlayerSettlementInfo.Instance.Castles.Count + 1;
			val4 = val2;
			playerSettlementItemTemplate = availableModels[currentModelOptionIdx];
			string text3 = MBRandom.RandomInt().ToString();
			text = playerSettlementItemTemplate.Id + "_random_" + text3;
			xmlNode = playerSettlementItemTemplate.ItemXML.CloneNode(deep: true);
			xmlNode.Attributes["id"].Value = text;
			XmlNode xmlNode2 = xmlNode.SelectSingleNode("descendant::Town");
			xmlNode2.Attributes["id"].Value = text + "_castle_comp";
			if (gateSupported && Main.Settings.AllowGatePosition && gatePlacementFrame.HasValue)
			{
				ref MatrixFrame? reference2 = ref gatePlacementFrame;
				if (reference2.HasValue)
				{
					reference2.GetValueOrDefault();
					if (true)
					{
						value = gatePlacementFrame.Value;
						val5 = (value.origin).AsVec2;
						goto IL_01a1;
					}
				}
				val5 = val4;
				goto IL_01a1;
			}
			goto IL_02a5;
		}

		private void BuildVillage()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Expected O, but got Unknown
			PlacementSupported = false;
			gateSupported = false;
			if (Main.Settings.AutoDetermineVillageOwner)
			{
				BuildVillageFor(null);
				return;
			}
			TextObject val = new TextObject("{=player_settlement_22}Choose village bound settlement", (Dictionary<string, object>)null);
			TextObject val2 = new TextObject("{=player_settlement_23}Choose the settlement to which this village is bound", (Dictionary<string, object>)null);
			List<InquiryElement> list = (from s in GetPotentialVillageBoundOwners()
				where s != null
				select s).Select((Func<Settlement, InquiryElement>)((Settlement c) => new InquiryElement((object)c, ((object)c.Name).ToString(), (ImageIdentifier)new CharacterImageIdentifier(CharacterCode.CreateFrom((BasicCharacterObject)(object)((c.IsTown || c.IsCastle) ? (c.Town.Governor ?? Hero.MainHero) : Hero.MainHero).CharacterObject)), true, ((object)(c.EncyclopediaText ?? c.Name)).ToString()))).ToList();
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(((object)val).ToString(), ((object)val2).ToString(), list, false, 1, 1, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), (string)null, (Action<List<InquiryElement>>)delegate(List<InquiryElement> args)
			{
				object obj = args?.FirstOrDefault()?.Identifier;
				Settlement bound = (Settlement)((obj is Settlement) ? obj : null);
				BuildVillageFor(bound);
			}, (Action<List<InquiryElement>>)delegate
			{
				InformationManager.HideInquiry();
				Reset();
				((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
			}, "", false), false, false);
		}

		private void BuildVillageFor(Settlement? bound)
		{
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Expected O, but got Unknown
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Expected O, but got Unknown
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Expected O, but got Unknown
			Settlement bound2 = bound;
			if (bound2 == null)
			{
				bound2 = CalculateVillageOwner();
			}
			if (bound2 == null)
			{
				InformationManager.HideInquiry();
				Reset();
				((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
				return;
			}
			PlayerSettlementItem boundTarget;
			int villageNumber = PlayerSettlementInfo.Instance.GetVillageNumber(bound2, out boundTarget);
			if (boundTarget != null && boundTarget.Villages == null)
			{
				boundTarget.Villages = new List<PlayerSettlementItem>();
			}
			Campaign.Current.TimeControlMode = (CampaignTimeControlMode)0;
			InformationManager.ShowTextInquiry(new TextInquiryData(((object)new TextObject("{=player_settlement_02}Create Player Settlement", (Dictionary<string, object>)null)).ToString(), ((object)new TextObject("{=player_settlement_03}What would you like to name your settlement?", (Dictionary<string, object>)null)).ToString(), true, true, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), ((object)GameTexts.FindText("str_cancel", (string)null)).ToString(), (Action<string>)delegate(string settlementName)
			{
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Expected O, but got Unknown
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Expected O, but got Unknown
				//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Expected O, but got Unknown
				//IL_0168: Unknown result type (might be due to invalid IL or missing references)
				//IL_0174: Expected O, but got Unknown
				Campaign.Current.TimeControlMode = (CampaignTimeControlMode)0;
				if (string.IsNullOrEmpty(settlementName))
				{
					settlementName = ((object)new TextObject("{=player_settlement_n_01}Player Settlement", (Dictionary<string, object>)null)).ToString();
				}
				if (Main.Settings.ForcePlayerCulture)
				{
					if (Main.Settings.AutoAllocateVillageType)
					{
						Apply(settlementName, Hero.MainHero.Culture, AutoCalculateVillageType(villageNumber));
					}
					else
					{
						DetermineVillageType(settlementName, Hero.MainHero.Culture, bound2, villageNumber, Apply);
					}
				}
				else
				{
					TextObject val6 = new TextObject("{=player_settlement_11}Choose village culture", (Dictionary<string, object>)null);
					TextObject val7 = new TextObject("{=player_settlement_12}Choose the culture for {VILLAGE}", (Dictionary<string, object>)null);
					val7.SetTextVariable("VILLAGE", settlementName);
					List<InquiryElement> list = GetCultures(mainOnly: true).Select((Func<CultureObject, InquiryElement>)((CultureObject c) => new InquiryElement((object)c, ((object)((BasicCultureObject)c).Name).ToString(), (ImageIdentifier)new BannerImageIdentifier(new Banner(((BasicCultureObject)c).Banner), false), true, ((object)((BasicCultureObject)c).Name).ToString()))).ToList();
					MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(((object)val6).ToString(), ((object)val7).ToString(), list, false, 1, 1, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), (string)null, (Action<List<InquiryElement>>)delegate(List<InquiryElement> args)
					{
						object obj3 = args?.FirstOrDefault()?.Identifier;
						CultureObject culture2 = (CultureObject)(((obj3 is CultureObject) ? obj3 : null) ?? Hero.MainHero.Culture);
						if (Main.Settings.AutoAllocateVillageType)
						{
							Apply(settlementName, culture2, AutoCalculateVillageType(villageNumber));
						}
						else
						{
							DetermineVillageType(settlementName, culture2, bound2, villageNumber, Apply);
						}
					}, (Action<List<InquiryElement>>)null, "", false), false, false);
				}
			}, (Action)delegate
			{
				InformationManager.HideInquiry();
				Reset();
				((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
			}, false, (Func<string, Tuple<bool, string>>)FactionHelper.IsKingdomNameApplicable, "", ""), true, false);
			void Apply(string settlementName, CultureObject culture, string villageType)
			{
				settlementPlacementFrame = null;
				availableModels?.Clear();
				if (Main.Settings.SelectedCultureOnly && Main.Submodule.CultureTemplates.ContainsKey(((MBObjectBase)culture).StringId))
				{
					availableModels = Main.Submodule.CultureTemplates[((MBObjectBase)culture).StringId].SelectMany((CultureSettlementTemplate cst) => SelectVillageTemplates(cst, bound2.IsCastle)).ToList();
					currentModelOptionIdx = -1;
				}
				if (availableModels == null || availableModels.Count == 0)
				{
					availableModels = Main.Submodule.CultureTemplates.Values.SelectMany((List<CultureSettlementTemplate> c) => c.SelectMany((CultureSettlementTemplate cst) => SelectVillageTemplates(cst, bound2.IsCastle))).ToList();
					currentModelOptionIdx = availableModels.FindIndex((PlayerSettlementItemTemplate a) => a.Culture == ((MBObjectBase)culture).StringId) - 1;
				}
				if (!Main.Settings.SettlementPlacement)
				{
					ConfirmAndApply();
				}
				else
				{
					StartSettlementPlacement();
					applyPending = delegate
					{
						ConfirmAndApply();
					};
				}
				void ConfirmAndApply()
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0011: Expected O, but got Unknown
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_001e: Expected O, but got Unknown
					//IL_008e: Unknown result type (might be due to invalid IL or missing references)
					//IL_009a: Expected O, but got Unknown
					string text = ((object)new TextObject("{=player_settlement_13}Build a Village", (Dictionary<string, object>)null)).ToString();
					TextObject val5 = new TextObject("{=player_settlement_14}Are you sure you want to build your village here?", (Dictionary<string, object>)null);
					InformationManager.ShowInquiry(new InquiryData(text, ((object)val5).ToString(), true, true, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), ((object)GameTexts.FindText("str_cancel", (string)null)).ToString(), (Action)delegate
					{
						InformationManager.HideInquiry();
						ApplyPlaced(settlementName, culture, villageType);
					}, (Action)delegate
					{
						InformationManager.HideInquiry();
						if (!Main.Settings.SettlementPlacement)
						{
							Reset();
							((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
						}
					}, "", 0f, (Action)null, (Func<ValueTuple<bool, string>>)null, (Func<ValueTuple<bool, string>>)null), true, false);
				}
			}
			void ApplyPlaced(string settlementName, CultureObject culture, string villageType)
			{
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Expected O, but got Unknown
				//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01db: Unknown result type (might be due to invalid IL or missing references)
				PlayerSettlementItem villageItem;
				Settlement val = CreateVillage(settlementName, culture, bound2, boundTarget, villageType, villageNumber, out villageItem);
				val.SetBound(bound2);
				val.SetName(new TextObject(settlementName, (Dictionary<string, object>)null));
				val.Party.SetLevelMaskIsDirty();
				val.IsVisible = true;
				val.IsInspected = true;
				val.Party.SetVisualAsDirty();
				SettlementVisualManager.Current.AddNewPartyVisualForParty(val.Party);
				val.OnGameCreated();
				((MBObjectBase)val).AfterInitialized();
				val.OnFinishLoadState();
				Village village = val.Village;
				villageItem.BuiltAt = Campaign.CurrentTime;
				if (Main.Settings.RequireVillageGold)
				{
					GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, val, Main.Settings.RequiredVillageGold, true);
				}
				else
				{
					((SettlementComponent)village).ChangeGold(3000);
				}
				if (Main.Settings.AddInitialMilitia)
				{
					val.Militia = val.Village.MilitiaChange * 45f;
				}
				if (Main.Settings.AddInitialNotables)
				{
					int targetNotableCountForSettlement = Campaign.Current.Models.NotableSpawnModel.GetTargetNotableCountForSettlement(val, (Occupation)22);
					for (int i = 0; i < targetNotableCountForSettlement; i++)
					{
						HeroCreator.CreateNotable((Occupation)22, val);
					}
					int targetNotableCountForSettlement2 = Campaign.Current.Models.NotableSpawnModel.GetTargetNotableCountForSettlement(val, (Occupation)20);
					for (int j = 0; j < targetNotableCountForSettlement2; j++)
					{
						HeroCreator.CreateNotable((Occupation)20, val);
					}
					PostNotablesAdded(val);
				}
				float num = 0f;
				foreach (var item in (List<(ItemObject, float)>)(object)village.VillageType.Productions)
				{
					ExplainedNumber val2 = Campaign.Current.Models.VillageProductionCalculatorModel.CalculateDailyProductionAmount(village, item.Item1);
					float resultNumber = (val2).ResultNumber;
					num += (float)item.Item1.Value * resultNumber;
				}
				village.TradeTaxAccumulated = (int)(num * (0.6f + 0.3f * MBRandom.RandomFloat) * Campaign.Current.Models.ClanFinanceModel.RevenueSmoothenFraction());
				CampaignGameStarter gameStarter = SandBoxManager.Instance.GameStarter;
				CampaignBehaviorBase obj = gameStarter.CampaignBehaviors.FirstOrDefault((CampaignBehaviorBase b) => b is VillageGoodProductionCampaignBehavior);
				VillageGoodProductionCampaignBehavior val3 = (VillageGoodProductionCampaignBehavior)(object)((obj is VillageGoodProductionCampaignBehavior) ? obj : null);
				if (val3 != null)
				{
					VillageGoodProductionCampaignBehavior villageGoodProductionCampaignBehavior = val3;
					if (true)
					{
						villageGoodProductionCampaignBehavior.NewVillageBuilt(village);
					}
				}
				CampaignBehaviorBase obj2 = gameStarter.CampaignBehaviors.FirstOrDefault((CampaignBehaviorBase b) => b is RecruitmentCampaignBehavior);
				RecruitmentCampaignBehavior val4 = (RecruitmentCampaignBehavior)(object)((obj2 is RecruitmentCampaignBehavior) ? obj2 : null);
				if (val4 != null)
				{
					RecruitmentCampaignBehavior recruitmentCampaignBehavior = val4;
					if (true)
					{
						recruitmentCampaignBehavior.NewSettlementBuilt(val);
					}
				}
				_settlementCreated.Invoke(villageItem.Settlement);
				SaveHandler.SaveLoad(Main.Settings.SaveMechanism);
			}
		}

		private List<PlayerSettlementItemTemplate> SelectVillageTemplates(CultureSettlementTemplate cst, bool forCastle)
		{
			List<PlayerSettlementItemTemplate> list = new List<PlayerSettlementItemTemplate>();
			try
			{
				IEnumerable<XmlNode> enumerable = cst.Document.SelectNodes("descendant::Settlement[@template_type='Village']").OfType<XmlNode>();
				foreach (XmlNode item in enumerable)
				{
					string text = item.Attributes["id"].Value.Replace("{{OWNER_TYPE}}", forCastle ? "castle" : "town");
					if (((List<string>)(object)Main.BlacklistedTemplates).Contains(text))
					{
						LogManager.EventTracer.Trace("Skipped blacklisted template: " + text);
						continue;
					}
					list.Add(new PlayerSettlementItemTemplate
					{
						Id = text,
						ItemXML = item,
						Type = 2,
						Culture = cst.CultureId
					});
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return list;
		}

		private void DetermineVillageType(string settlementName, CultureObject culture, Settlement? bound, int villageNumber, Action<string, CultureObject, string> Apply)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Expected O, but got Unknown
			List<InquiryElement> villageTypeInquiry = GetVillageTypeInquiry();
			TextObject val = new TextObject("{=player_settlement_15}Choose village type", (Dictionary<string, object>)null);
			TextObject val2 = new TextObject("{=player_settlement_16}Choose the type of primary product for {VILLAGE}", (Dictionary<string, object>)null);
			val2.SetTextVariable("VILLAGE", settlementName);
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(((object)val).ToString(), ((object)val2).ToString(), villageTypeInquiry, false, 1, 1, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), (string)null, (Action<List<InquiryElement>>)delegate(List<InquiryElement> args)
			{
				object obj = args?.FirstOrDefault()?.Identifier;
				object obj2 = ((obj is VillageType) ? obj : null);
				string arg = ((obj2 != null) ? ((MBObjectBase)obj2).StringId : null) ?? AutoCalculateVillageType(villageNumber);
				Apply(settlementName, culture, arg);
			}, (Action<List<InquiryElement>>)null, "", false), false, false);
		}

		private Settlement CreateVillage(string settlementName, CultureObject culture, Settlement? bound, PlayerSettlementItem? boundTarget, string villageType, int villageNumber, out PlayerSettlementItem villageItem)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Expected O, but got Unknown
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Expected O, but got Unknown
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			if (currentModelOptionIdx < 0)
			{
				currentModelOptionIdx = new Random().Next(0, availableModels.Count);
			}
			ref MatrixFrame? reference = ref settlementPlacementFrame;
			Vec2 val;
			if (reference.HasValue)
			{
				reference.GetValueOrDefault();
				if (true)
				{
					MatrixFrame value = settlementPlacementFrame.Value;
					val = (value.origin).AsVec2;
					goto IL_0074;
				}
			}
			val = MobileParty.MainParty.GetPosition2D;
			goto IL_0074;
			IL_0074:
			Vec2 val2 = val;
			PlayerSettlementItemTemplate playerSettlementItemTemplate = availableModels[currentModelOptionIdx];
			XmlNode xmlNode = playerSettlementItemTemplate.ItemXML.CloneNode(deep: true);
			string text = MBRandom.RandomInt().ToString();
			string text2 = playerSettlementItemTemplate.Id + "_random_" + text;
			xmlNode.Attributes["id"].Value = text2;
			xmlNode.Attributes["posX"].Value = (val2).X.ToString();
			xmlNode.Attributes["posY"].Value = (val2).Y.ToString();
			xmlNode.Attributes["name"].Value = settlementName;
			xmlNode.Attributes["culture"].Value = "Culture." + ((MBObjectBase)culture).StringId;
			XmlNode xmlNode2 = xmlNode.SelectSingleNode("descendant::Village");
			xmlNode2.Attributes["id"].Value = xmlNode2.Attributes["id"].Value.Replace("{{OWNER_TYPE}}", (bound != null && bound.IsCastle) ? "castle" : "town") + "_random_" + text;
			xmlNode2.Attributes["village_type"].Value = "VillageType." + villageType;
			xmlNode2.Attributes["bound"].Value = "Settlement." + ((bound != null) ? ((MBObjectBase)bound).StringId : null);
			TextObject val3 = new TextObject("{=player_settlement_24}{SETTLEMENT_NAME} was founded by {HERO_NAME} of the {FACTION_TERM} on {BUILD_TIME}", (Dictionary<string, object>)null);
			val3.SetTextVariable("SETTLEMENT_NAME", PlayerSettlementItem.EncyclopediaLinkWithName(text2, new TextObject(settlementName, (Dictionary<string, object>)null)));
			val3.SetTextVariable("HERO_NAME", Hero.MainHero.EncyclopediaLinkWithName);
			val3.SetTextVariable("FACTION_TERM", Hero.MainHero.Clan.EncyclopediaLinkWithName);
			CampaignTime now = CampaignTime.Now;
			val3.SetTextVariable("BUILD_TIME", ((object)now).ToString());
			if (xmlNode.Attributes["text"] == null)
			{
				XmlAttribute xmlAttribute = xmlNode.OwnerDocument.CreateAttribute("text");
				xmlAttribute.Value = ((object)val3).ToString();
				xmlNode.Attributes.SetNamedItem(xmlAttribute);
			}
			else
			{
				xmlNode.Attributes["text"].Value = ((object)val3).ToString();
			}
			string text3 = "<Settlements>" + xmlNode.OuterXml + "</Settlements>";
			villageItem = new PlayerSettlementItem
			{
				ItemXML = text3,
				Identifier = villageNumber,
				Type = 2,
				SettlementName = settlementName,
				RotationMat3 = settlementPlacementFrame?.rotation,
				DeepEdits = new List<DeepTransformEdit>((deepEditPrefab == settlementVisualPrefab && deepTransformEdits != null) ? deepTransformEdits : new List<DeepTransformEdit>()),
				Version = Main.Version,
				StringId = text2,
				PrefabId = playerSettlementItemTemplate.Id
			};
			if (boundTarget == null)
			{
				if (PlayerSettlementInfo.Instance.PlayerVillages == null)
				{
					PlayerSettlementInfo.Instance.PlayerVillages = new List<PlayerSettlementItem>();
				}
				PlayerSettlementInfo.Instance.PlayerVillages.Add(villageItem);
			}
			else
			{
				boundTarget.Villages.Add(villageItem);
			}
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(text3);
			Campaign.Current.AsCampaignGameLoadingType((GameLoadingType)1, delegate
			{
				MBObjectManager.Instance.LoadXml(doc, false);
				return true;
			});
			Settlement @object = MBObjectManager.Instance.GetObject<Settlement>(villageItem.StringId);
			villageItem.Settlement = @object;
			return @object;
		}

		private void PostNotablesAdded(Settlement settlement)
		{
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < ((List<Hero>)(object)settlement.Notables).Count; i++)
			{
				Hero val = ((List<Hero>)(object)settlement.Notables)[i];
				foreach (Hero item in (List<Hero>)(object)settlement.MapFaction.AliveLords)
				{
					if (item != val && item == item.Clan.Leader && item.MapFaction == settlement.MapFaction)
					{
						float chanceOfConflict = (float)HeroHelper.NPCPersonalityClashWithNPC(val, item) * 0.01f * 2.5f;
						float randomFloat = MBRandom.RandomFloat;
						float num = Campaign.MapDiagonal;
						foreach (Settlement item2 in (List<Settlement>)(object)item.Clan.Settlements)
						{
							float num2;
							if (settlement != item2)
							{
								Vec2 getPosition2D = item2.GetPosition2D;
								num2 = (getPosition2D).Distance(settlement.GetPosition2D);
							}
							else
							{
								num2 = 0f;
							}
							float num3 = num2;
							if (!(num3 >= num))
							{
								num = num3;
							}
						}
						float num4 = ((num < 100f) ? (1f - num / 100f) : 0f);
						float num5 = num4 * MBRandom.RandomFloat + (1f - num4);
						if (MBRandom.RandomFloat < 0.2f)
						{
							num5 = 1f / (0.5f + 0.5f * num5);
						}
						randomFloat *= num5;
						if (randomFloat > 1f)
						{
							randomFloat = 1f;
						}
						DetermineRelation(val, item, randomFloat, chanceOfConflict);
					}
					for (int j = i + 1; j < ((List<Hero>)(object)settlement.Notables).Count; j++)
					{
						Hero val2 = ((List<Hero>)(object)settlement.Notables)[j];
						float chanceOfConflict2 = (float)HeroHelper.NPCPersonalityClashWithNPC(val, val2) * 0.01f * 2.5f;
						float randomValue = MBRandom.RandomFloat;
						if (val.CharacterObject.Occupation == val2.CharacterObject.Occupation)
						{
							randomValue = 1f - 0.25f * MBRandom.RandomFloat;
						}
						DetermineRelation(val, val2, randomValue, chanceOfConflict2);
					}
				}
			}
			int num6 = 50;
			for (int k = 0; k < num6; k++)
			{
				foreach (Hero item3 in (List<Hero>)(object)Hero.AllAliveHeroes)
				{
					if (item3.IsNotable)
					{
						UpdateNotableSupport(item3);
					}
				}
			}
		}

		private void UpdateNotableSupport(Hero notable)
		{
			if (notable.SupporterOf != null)
			{
				int relation = notable.GetRelation(notable.SupporterOf.Leader);
				if (relation < 0)
				{
					notable.SupporterOf = null;
				}
				else if (relation < 50)
				{
					float num = (float)(50 - relation) / 500f;
					if (MBRandom.RandomFloat < num)
					{
						notable.SupporterOf = null;
					}
				}
				return;
			}
			foreach (Clan nonBanditFaction in Clan.NonBanditFactions)
			{
				if (nonBanditFaction.Leader == null)
				{
					continue;
				}
				int relation2 = notable.GetRelation(nonBanditFaction.Leader);
				if (relation2 > 50)
				{
					float num2 = (float)(relation2 - 50) / 2000f;
					if (!(MBRandom.RandomFloat >= num2))
					{
						notable.SupporterOf = nonBanditFaction;
					}
				}
			}
		}

		private void DetermineRelation(Hero hero1, Hero hero2, float randomValue, float chanceOfConflict)
		{
			float num = 0.3f;
			if (randomValue < num)
			{
				int num2 = (int)((num - randomValue) * (num - randomValue) / (num * num) * 100f);
				if (num2 > 0)
				{
					ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero1, hero2, num2, true);
				}
			}
			else if (randomValue > 1f - chanceOfConflict)
			{
				int num3 = -(int)((randomValue - (1f - chanceOfConflict)) * (randomValue - (1f - chanceOfConflict)) / (chanceOfConflict * chanceOfConflict) * 100f);
				if (num3 < 0)
				{
					ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero1, hero2, num3, true);
				}
			}
		}

		private void BuildTown()
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Expected O, but got Unknown
			PlacementSupported = false;
			gateSupported = true;
			portSupported = Main.IsWarSails;
			Campaign.Current.TimeControlMode = (CampaignTimeControlMode)0;
			InformationManager.ShowTextInquiry(new TextInquiryData(((object)new TextObject("{=player_settlement_02}Create Player Settlement", (Dictionary<string, object>)null)).ToString(), ((object)new TextObject("{=player_settlement_03}What would you like to name your settlement?", (Dictionary<string, object>)null)).ToString(), true, true, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), ((object)GameTexts.FindText("str_cancel", (string)null)).ToString(), (Action<string>)delegate(string settlementName)
			{
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Expected O, but got Unknown
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Expected O, but got Unknown
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Expected O, but got Unknown
				//IL_0108: Unknown result type (might be due to invalid IL or missing references)
				//IL_0114: Expected O, but got Unknown
				Campaign.Current.TimeControlMode = (CampaignTimeControlMode)0;
				if (string.IsNullOrEmpty(settlementName))
				{
					settlementName = ((object)new TextObject("{=player_settlement_n_01}Player Settlement", (Dictionary<string, object>)null)).ToString();
				}
				if (Main.Settings.ForcePlayerCulture)
				{
					Apply(settlementName, Hero.MainHero.Culture);
				}
				else
				{
					TextObject val5 = new TextObject("{=player_settlement_09}Choose town culture", (Dictionary<string, object>)null);
					TextObject val6 = new TextObject("{=player_settlement_10}Choose the culture for {TOWN}", (Dictionary<string, object>)null);
					val6.SetTextVariable("TOWN", settlementName);
					List<InquiryElement> list = GetCultures(mainOnly: true).Select((Func<CultureObject, InquiryElement>)((CultureObject c) => new InquiryElement((object)c, ((object)((BasicCultureObject)c).Name).ToString(), (ImageIdentifier)new BannerImageIdentifier(new Banner(((BasicCultureObject)c).Banner), false), true, ((object)((BasicCultureObject)c).Name).ToString()))).ToList();
					MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(((object)val5).ToString(), ((object)val6).ToString(), list, false, 1, 1, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), (string)null, (Action<List<InquiryElement>>)delegate(List<InquiryElement> args)
					{
						object obj4 = args?.FirstOrDefault()?.Identifier;
						CultureObject culture2 = (CultureObject)(((obj4 is CultureObject) ? obj4 : null) ?? Hero.MainHero.Culture);
						Apply(settlementName, culture2);
					}, (Action<List<InquiryElement>>)null, "", false), false, false);
				}
			}, (Action)delegate
			{
				InformationManager.HideInquiry();
				Reset();
				((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
			}, false, (Func<string, Tuple<bool, string>>)FactionHelper.IsKingdomNameApplicable, "", ""), true, false);
			void Apply(string settlementName, CultureObject culture)
			{
				settlementPlacementFrame = null;
				availableModels?.Clear();
				if (Main.Settings.SelectedCultureOnly && Main.Submodule.CultureTemplates.ContainsKey(((MBObjectBase)culture).StringId))
				{
					availableModels = Main.Submodule.CultureTemplates[((MBObjectBase)culture).StringId].SelectMany(SelectTownTemplates).ToList();
					currentModelOptionIdx = -1;
				}
				if (availableModels == null || availableModels.Count == 0)
				{
					availableModels = Main.Submodule.CultureTemplates.Values.SelectMany((List<CultureSettlementTemplate> c) => c.SelectMany(SelectTownTemplates)).ToList();
					currentModelOptionIdx = availableModels.FindIndex((PlayerSettlementItemTemplate a) => a.Culture == ((MBObjectBase)culture).StringId) - 1;
				}
				if (!Main.Settings.SettlementPlacement)
				{
					ConfirmAndApply();
				}
				else
				{
					StartSettlementPlacement();
					applyPending = delegate
					{
						ConfirmAndApply();
					};
				}
				void ConfirmAndApply()
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0011: Expected O, but got Unknown
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_001e: Expected O, but got Unknown
					//IL_0071: Unknown result type (might be due to invalid IL or missing references)
					//IL_007d: Expected O, but got Unknown
					string text = ((object)new TextObject("{=player_settlement_04}Build a Town", (Dictionary<string, object>)null)).ToString();
					TextObject val4 = new TextObject("{=player_settlement_05}Are you sure you want to build your town here?", (Dictionary<string, object>)null);
					InformationManager.ShowInquiry(new InquiryData(text, ((object)val4).ToString(), true, true, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), ((object)GameTexts.FindText("str_cancel", (string)null)).ToString(), (Action)delegate
					{
						InformationManager.HideInquiry();
						ApplyPlaced(settlementName, culture);
					}, (Action)delegate
					{
						InformationManager.HideInquiry();
						if (!Main.Settings.SettlementPlacement)
						{
							Reset();
							((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
						}
					}, "", 0f, (Action)null, (Func<ValueTuple<bool, string>>)null, (Func<ValueTuple<bool, string>>)null), true, false);
				}
			}
			void ApplyPlaced(string settlementName, CultureObject culture)
			{
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Expected O, but got Unknown
				PlayerSettlementItem townItem;
				Settlement val = CreateTown(settlementName, culture, out townItem);
				val.Town.OwnerClan = Hero.MainHero.Clan;
				val.SetName(new TextObject(settlementName, (Dictionary<string, object>)null));
				val.Party.SetLevelMaskIsDirty();
				val.IsVisible = true;
				val.IsInspected = true;
				((Fief)val.Town).FoodStocks = val.Town.FoodStocksUpperLimit();
				val.Party.SetVisualAsDirty();
				SettlementVisualManager.Current.AddNewPartyVisualForParty(val.Party);
				val.OnGameCreated();
				((MBObjectBase)val).AfterInitialized();
				val.OnFinishLoadState();
				Town town = val.Town;
				InitTownBuildings(val);
				townItem.BuiltAt = Campaign.CurrentTime;
				if (Main.Settings.RequireGold)
				{
					GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, val, Main.Settings.RequiredGold, true);
				}
				else
				{
					val.SettlementComponent.ChangeGold(3000);
				}
				CampaignGameStarter gameStarter = SandBoxManager.Instance.GameStarter;
				if (Main.Settings.AddInitialGarrison)
				{
					val.AddGarrisonParty();
					GarrisonTroopsCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<GarrisonTroopsCampaignBehavior>();
					if (campaignBehavior != null && val.Town != null)
					{
						FillGarrisonPartyOnNewGameInvoker(campaignBehavior, val.Town);
					}
					val.SetGarrisonWagePaymentLimit(Campaign.Current.Models.PartyWageModel.MaxWagePaymentLimit);
				}
				if (Main.Settings.AddInitialMilitia)
				{
					val.Militia = val.Town.MilitiaChange * 45f;
				}
				if (Main.Settings.AddInitialNotables)
				{
					int targetNotableCountForSettlement = Campaign.Current.Models.NotableSpawnModel.GetTargetNotableCountForSettlement(val, (Occupation)17);
					for (int i = 0; i < targetNotableCountForSettlement; i++)
					{
						HeroCreator.CreateNotable((Occupation)17, val);
					}
					int targetNotableCountForSettlement2 = Campaign.Current.Models.NotableSpawnModel.GetTargetNotableCountForSettlement(val, (Occupation)18);
					for (int j = 0; j < targetNotableCountForSettlement2; j++)
					{
						HeroCreator.CreateNotable((Occupation)18, val);
					}
					int targetNotableCountForSettlement3 = Campaign.Current.Models.NotableSpawnModel.GetTargetNotableCountForSettlement(val, (Occupation)21);
					for (int k = 0; k < targetNotableCountForSettlement3; k++)
					{
						HeroCreator.CreateNotable((Occupation)21, val);
					}
					PostNotablesAdded(val);
				}
				foreach (ItemCategory item in (List<ItemCategory>)(object)ItemCategories.All)
				{
					if (item.IsValid)
					{
						town.MarketData.AddDemand(item, 3f);
						town.MarketData.AddSupply(item, 2f);
					}
				}
				town.MarketData.UpdateStores();
				if (Main.Settings.AddInitialNotables && ((List<Hero>)(object)val.Notables).Count > 0)
				{
					CampaignBehaviorBase obj = gameStarter.CampaignBehaviors.FirstOrDefault((CampaignBehaviorBase b) => b is WorkshopsCampaignBehavior);
					WorkshopsCampaignBehavior val2 = (WorkshopsCampaignBehavior)(object)((obj is WorkshopsCampaignBehavior) ? obj : null);
					if (val2 != null)
					{
						WorkshopsCampaignBehavior behavior = val2;
						if (true)
						{
							behavior.NewTownBuilt(town);
						}
					}
					int num = MBRandom.RandomInt(0, val.Alleys.Count);
					IEnumerable<Hero> source = ((IEnumerable<Hero>)val.Notables).Where((Hero x) => x.IsGangLeader);
					for (int l = num; l < num + 2; l++)
					{
						val.Alleys[l % val.Alleys.Count].SetOwner(source.ElementAt(l % source.Count()));
					}
				}
				CampaignBehaviorBase obj2 = gameStarter.CampaignBehaviors.FirstOrDefault((CampaignBehaviorBase b) => b is CraftingCampaignBehavior);
				((CraftingCampaignBehavior)(object)((obj2 is CraftingCampaignBehavior) ? obj2 : null))?.AddTown(town, out Dictionary<Town, CraftingOrderSlots> _);
				CampaignBehaviorBase obj3 = gameStarter.CampaignBehaviors.FirstOrDefault((CampaignBehaviorBase b) => b is RecruitmentCampaignBehavior);
				RecruitmentCampaignBehavior val3 = (RecruitmentCampaignBehavior)(object)((obj3 is RecruitmentCampaignBehavior) ? obj3 : null);
				if (val3 != null)
				{
					RecruitmentCampaignBehavior recruitmentCampaignBehavior = val3;
					if (true)
					{
						recruitmentCampaignBehavior.NewSettlementBuilt(val);
					}
				}
				_settlementCreated.Invoke(townItem.Settlement);
				SaveHandler.SaveLoad(Main.Settings.SaveMechanism);
			}
		}

		private List<PlayerSettlementItemTemplate> SelectTownTemplates(CultureSettlementTemplate cst)
		{
			List<PlayerSettlementItemTemplate> list = new List<PlayerSettlementItemTemplate>();
			try
			{
				IEnumerable<XmlNode> enumerable = cst.Document.SelectNodes("descendant::Settlement[@template_type='Town']").OfType<XmlNode>();
				foreach (XmlNode item in enumerable)
				{
					string value = item.Attributes["id"].Value;
					if (((List<string>)(object)Main.BlacklistedTemplates).Contains(value))
					{
						LogManager.EventTracer.Trace("Skipped blacklisted template: " + value);
						continue;
					}
					list.Add(new PlayerSettlementItemTemplate
					{
						Id = value,
						ItemXML = item,
						Type = 1,
						Culture = cst.CultureId
					});
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return list;
		}

		private void StartSettlementPlacement()
		{
			UpdateSettlementVisualEntity(forward: true, retry: true);
			ShowSettlementPlacementHelp();
		}

		private static void ShowSettlementPlacementHelp(bool forceShow = false)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			if (!Main.Settings.DisableAutoHints || forceShow)
			{
				TextObject val = new TextObject("{=player_settlement_37}Choose your settlement. \r\nPress {HELP_KEY} for help. \r\nClick {MOUSE_CLICK} anywhere to apply or press {ESC_KEY} to cancel.  \r\nUse {DEEP_EDIT_KEY} to switch to deep edit mode. \r\nUse {CYCLE_MODIFIER_KEY} and {CYCLE_BACK_KEY} / {CYCLE_NEXT_KEY} to change visual options.\r\nUse {ROTATE_MODIFIER_KEY} and {ROTATE_BACK_KEY} / {ROTATE_NEXT_KEY} to change rotation. \r\nUse {ROTATE_MODIFIER_KEY} and {ROTATE_FORWARD_KEY} / {ROTATE_BACKWARD_KEY} to change forward rotation. \r\nUse {ROTATE_MODIFIER_KEY} + {ROTATE_MODIFIER_ALTERNATE} and {ROTATE_FORWARD_KEY} / {ROTATE_BACKWARD_KEY} to change axis rotation. \r\nUse {SCALE_MODIFIER_KEY} and {SCALE_BACK_KEY} / {SCALE_NEXT_KEY} to change scale. \r\nUse {CYCLE_MODIFIER_KEY} and {MOVE_UP_KEY} / {MOVE_DOWN_KEY} to move up or down.", (Dictionary<string, object>)null);
				InputKey val2 = Main.Submodule.HelpKey.GetInputKey();
				val.SetTextVariable("HELP_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)val2).ToString(), 1f));
				val2 = (InputKey)1;
				val.SetTextVariable("ESC_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)val2).ToString(), 1f));
				val2 = (InputKey)224;
				val.SetTextVariable("MOUSE_CLICK", HyperlinkTexts.GetKeyHyperlinkText(((object)val2).ToString(), 1f));
				val2 = Main.Submodule.DeepEditToggleKey.GetInputKey();
				val.SetTextVariable("DEEP_EDIT_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)val2).ToString(), 1f));
				val.SetTextVariable("CYCLE_MODIFIER_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.CycleModifierKey)).ToString(), 1f));
				val.SetTextVariable("CYCLE_BACK_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.CycleBackKey)).ToString(), 1f));
				val.SetTextVariable("CYCLE_NEXT_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.CycleNextKey)).ToString(), 1f));
				val.SetTextVariable("MOVE_UP_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.MoveUpKey)).ToString(), 1f));
				val.SetTextVariable("MOVE_DOWN_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.MoveDownKey)).ToString(), 1f));
				val.SetTextVariable("ROTATE_MODIFIER_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.RotateModifierKey)).ToString(), 1f));
				val.SetTextVariable("ROTATE_MODIFIER_ALTERNATE", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.RotateAlternateModifierKey)).ToString(), 1f));
				val.SetTextVariable("ROTATE_BACK_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.RotatePreviousKey)).ToString(), 1f));
				val.SetTextVariable("ROTATE_NEXT_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.RotateNextKey)).ToString(), 1f));
				val.SetTextVariable("ROTATE_FORWARD_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.RotateForwardKey)).ToString(), 1f));
				val.SetTextVariable("ROTATE_BACKWARD_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.RotateBackwardsKey)).ToString(), 1f));
				val.SetTextVariable("SCALE_MODIFIER_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.ScaleModifierKey)).ToString(), 1f));
				val.SetTextVariable("SCALE_BACK_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.ScaleSmallerKey)).ToString(), 1f));
				val.SetTextVariable("SCALE_NEXT_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.ScaleBiggerKey)).ToString(), 1f));
				int num = Main.Settings.HintDurationSeconds * 1000;
				Hero mainHero = Hero.MainHero;
				MBInformationManager.AddQuickInformation(val, num, (BasicCharacterObject)(object)((mainHero != null) ? mainHero.CharacterObject : null), (Equipment)null, "");
			}
		}

		private static void InitTownBuildings(Settlement townSettlement)
		{
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Expected O, but got Unknown
			Town town = townSettlement.Town;
			foreach (BuildingType all1 in (List<BuildingType>)(object)BuildingType.All)
			{
				int val = MBRandom.RandomInt(1, 4);
				if (!((IEnumerable<Building>)town.Buildings).Any((Building k) => ((MBObjectBase)k.BuildingType).StringId == ((MBObjectBase)all1).StringId))
				{
					bool flag = ((MBObjectBase)all1).StringId.StartsWith("building_settlement");
					bool flag2 = ((MBObjectBase)all1).StringId.StartsWith("building_shipyard");
					if (flag || (townSettlement.HasPort && flag2 && Main.IsWarSails))
					{
						((List<Building>)(object)town.Buildings).Add(new Building(all1, town, 0f, all1.IsDailyProject ? 1 : Math.Max(val, all1.StartLevel)));
					}
				}
			}
			foreach (Building item in ((IEnumerable<Building>)town.Buildings).OrderByDescending((Building k) => k.CurrentLevel))
			{
				if (item.CurrentLevel != 3 && item.CurrentLevel != item.BuildingType.StartLevel && !item.BuildingType.IsDailyProject)
				{
					town.BuildingsInProgress.Enqueue(item);
				}
			}
			Building val2 = ((IEnumerable<Building>)townSettlement.Town.Buildings).FirstOrDefault((Building b) => b.BuildingType.IsDailyProject && b.IsCurrentlyDefault);
			if (val2 == null)
			{
				val2 = ((IEnumerable<Building>)townSettlement.Town.Buildings).FirstOrDefault((Building b) => b.BuildingType.IsDailyProject);
				BuildingHelper.ChangeDefaultBuilding(val2, townSettlement.Town);
				val2.IsCurrentlyDefault = true;
			}
			if (val2 != null)
			{
				val2.IsCurrentlyDefault = true;
			}
		}

		private Settlement CreateTown(string settlementName, CultureObject culture, out PlayerSettlementItem townItem)
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ed: Expected O, but got Unknown
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Expected O, but got Unknown
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_065e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			if (currentModelOptionIdx < 0)
			{
				currentModelOptionIdx = new Random().Next(0, availableModels.Count);
			}
			if (PlayerSettlementInfo.Instance.Towns == null)
			{
				PlayerSettlementInfo.Instance.Towns = new List<PlayerSettlementItem>();
			}
			ref MatrixFrame? reference = ref settlementPlacementFrame;
			MatrixFrame value;
			Vec2 val;
			if (reference.HasValue)
			{
				reference.GetValueOrDefault();
				if (true)
				{
					value = settlementPlacementFrame.Value;
					val = (value.origin).AsVec2;
					goto IL_0098;
				}
			}
			val = MobileParty.MainParty.GetPosition2D;
			goto IL_0098;
			IL_0236:
			Vec2 val2;
			if (portSupported && Main.IsWarSails && portPlacementFrame.HasValue)
			{
				ref MatrixFrame? reference2 = ref portPlacementFrame;
				if (reference2.HasValue)
				{
					reference2.GetValueOrDefault();
					if (true)
					{
						value = portPlacementFrame.Value;
						val2 = (value.origin).AsVec2;
						goto IL_0298;
					}
				}
				val2 = Vec2.Invalid;
				goto IL_0298;
			}
			goto IL_03b4;
			IL_03b4:
			string text = MBRandom.RandomInt().ToString();
			PlayerSettlementItemTemplate playerSettlementItemTemplate;
			string text2 = playerSettlementItemTemplate.Id + "_random_" + text;
			XmlNode xmlNode;
			xmlNode.Attributes["id"].Value = text2;
			XmlNode xmlNode2 = xmlNode.SelectSingleNode("descendant::Town");
			xmlNode2.Attributes["id"].Value = text2 + "_town_comp";
			Vec2 val3;
			xmlNode.Attributes["posX"].Value = (val3).X.ToString();
			xmlNode.Attributes["posY"].Value = (val3).Y.ToString();
			xmlNode.Attributes["name"].Value = settlementName;
			xmlNode.Attributes["owner"].Value = "Faction." + ((MBObjectBase)Hero.MainHero.Clan).StringId;
			xmlNode.Attributes["culture"].Value = "Culture." + ((MBObjectBase)culture).StringId;
			TextObject val4 = new TextObject("{=player_settlement_24}{SETTLEMENT_NAME} was founded by {HERO_NAME} of the {FACTION_TERM} on {BUILD_TIME}", (Dictionary<string, object>)null);
			val4.SetTextVariable("SETTLEMENT_NAME", PlayerSettlementItem.EncyclopediaLinkWithName(text2, new TextObject(settlementName, (Dictionary<string, object>)null)));
			val4.SetTextVariable("HERO_NAME", Hero.MainHero.EncyclopediaLinkWithName);
			val4.SetTextVariable("FACTION_TERM", Hero.MainHero.Clan.EncyclopediaLinkWithName);
			CampaignTime now = CampaignTime.Now;
			val4.SetTextVariable("BUILD_TIME", ((object)now).ToString());
			if (xmlNode.Attributes["text"] == null)
			{
				XmlAttribute xmlAttribute = xmlNode.OwnerDocument.CreateAttribute("text");
				xmlAttribute.Value = ((object)val4).ToString();
				xmlNode.Attributes.SetNamedItem(xmlAttribute);
			}
			else
			{
				xmlNode.Attributes["text"].Value = ((object)val4).ToString();
			}
			string text3 = "<Settlements>" + xmlNode.OuterXml + "</Settlements>";
			Vec2 val5;
			text3 = text3.Replace("{{G_POS_X}}", (val5).X.ToString());
			text3 = text3.Replace("{{G_POS_Y}}", (val5).Y.ToString());
			int identifier;
			townItem = new PlayerSettlementItem
			{
				ItemXML = text3,
				Identifier = identifier,
				Type = 1,
				SettlementName = settlementName,
				RotationMat3 = settlementPlacementFrame?.rotation,
				DeepEdits = new List<DeepTransformEdit>((deepEditPrefab == settlementVisualPrefab && deepTransformEdits != null) ? deepTransformEdits : new List<DeepTransformEdit>()),
				Version = Main.Version,
				StringId = text2,
				PrefabId = playerSettlementItemTemplate.Id
			};
			PlayerSettlementInfo.Instance.Towns.Add(townItem);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(text3);
			Campaign.Current.AsCampaignGameLoadingType((GameLoadingType)1, delegate
			{
				MBObjectManager.Instance.LoadXml(doc, false);
				return true;
			});
			Settlement @object = MBObjectManager.Instance.GetObject<Settlement>(townItem.StringId);
			townItem.Settlement = @object;
			return @object;
			IL_0298:
			Vec2 val6 = val2;
			if (val6 != Vec2.Invalid)
			{
				if (xmlNode.Attributes["port_posX"] == null)
				{
					XmlAttribute xmlAttribute2 = xmlNode.OwnerDocument.CreateAttribute("port_posX");
					xmlAttribute2.Value = (val6).X.ToString();
					xmlNode.Attributes.SetNamedItem(xmlAttribute2);
				}
				else
				{
					xmlNode.Attributes["port_posX"].Value = (val6).X.ToString();
				}
				if (xmlNode.Attributes["port_posY"] == null)
				{
					XmlAttribute xmlAttribute3 = xmlNode.OwnerDocument.CreateAttribute("port_posY");
					xmlAttribute3.Value = (val6).Y.ToString();
					xmlNode.Attributes.SetNamedItem(xmlAttribute3);
				}
				else
				{
					xmlNode.Attributes["port_posY"].Value = (val6).Y.ToString();
				}
			}
			goto IL_03b4;
			IL_0098:
			val3 = val;
			identifier = PlayerSettlementInfo.Instance.Towns.Count + 1;
			val5 = val3;
			playerSettlementItemTemplate = availableModels[currentModelOptionIdx];
			xmlNode = playerSettlementItemTemplate.ItemXML.CloneNode(deep: true);
			Vec2 val7;
			if (gateSupported && Main.Settings.AllowGatePosition && gatePlacementFrame.HasValue)
			{
				ref MatrixFrame? reference3 = ref gatePlacementFrame;
				if (reference3.HasValue)
				{
					reference3.GetValueOrDefault();
					if (true)
					{
						value = gatePlacementFrame.Value;
						val7 = (value.origin).AsVec2;
						goto IL_0132;
					}
				}
				val7 = val5;
				goto IL_0132;
			}
			goto IL_0236;
			IL_0132:
			val5 = val7;
			if (xmlNode.Attributes["gate_posX"] == null)
			{
				XmlAttribute xmlAttribute4 = xmlNode.OwnerDocument.CreateAttribute("gate_posX");
				xmlAttribute4.Value = (val5).X.ToString();
				xmlNode.Attributes.SetNamedItem(xmlAttribute4);
			}
			else
			{
				xmlNode.Attributes["gate_posX"].Value = (val5).X.ToString();
			}
			if (xmlNode.Attributes["gate_posY"] == null)
			{
				XmlAttribute xmlAttribute5 = xmlNode.OwnerDocument.CreateAttribute("gate_posY");
				xmlAttribute5.Value = (val5).Y.ToString();
				xmlNode.Attributes.SetNamedItem(xmlAttribute5);
			}
			else
			{
				xmlNode.Attributes["gate_posY"].Value = (val5).Y.ToString();
			}
			goto IL_0236;
		}

		public void RefreshVisualSelection()
		{
			deepEditScale = 1f;
			currentDeepTarget = null;
			currentModelOptionIdx--;
			UpdateSettlementVisualEntity(forward: true, retry: true);
		}

		private void UpdateSettlementVisualEntity(bool forward, bool retry = false)
		{
			//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Expected O, but got Unknown
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Expected O, but got Unknown
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_055f: Expected O, but got Unknown
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Expected O, but got Unknown
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Expected O, but got Unknown
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			bool result = default(bool);
			while (true)
			{
				try
				{
					LogManager.EventTracer.Trace($"UpdateSettlementVisualEntity forward={forward} noRetry={retry}");
					if (forward)
					{
						currentModelOptionIdx++;
						if (currentModelOptionIdx >= availableModels.Count)
						{
							currentModelOptionIdx = 0;
						}
					}
					else
					{
						currentModelOptionIdx--;
						if (currentModelOptionIdx < 0)
						{
							currentModelOptionIdx = availableModels.Count - 1;
						}
					}
					if (currentModelOptionIdx < 0)
					{
						currentModelOptionIdx = 0;
					}
					ClearEntities();
					ref MatrixFrame? reference = ref settlementPlacementFrame;
					Vec2 val;
					if (reference.HasValue)
					{
						reference.GetValueOrDefault();
						if (true)
						{
							MatrixFrame value = settlementPlacementFrame.Value;
							val = (value.origin).AsVec2;
							goto IL_00f7;
						}
					}
					val = MobileParty.MainParty.GetPosition2D;
					goto IL_00f7;
					IL_00f7:
					Vec2 val2 = val;
					PlayerSettlementItemTemplate playerSettlementItemTemplate = availableModels[currentModelOptionIdx];
					Debug.Print("Requesting swap model for settlement build to: " + playerSettlementItemTemplate.Id, 2, (DebugColor)10, 17592186044416uL);
					List<string> list = new List<string>
					{
						"Requesting swap model for settlement build to: " + playerSettlementItemTemplate.Id,
						$"Available models: {availableModels.Count}"
					};
					list.AddRange(availableModels.Select((PlayerSettlementItemTemplate a, int idx) => $"\t\t{a.Id} - Culture: '{a.Culture}', Type: '{a.Type}', Variant: '{idx + 1}'"));
					LogManager.EventTracer.Trace(list);
					Scene ____scene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
					Vec2 position = val2;
					string prefabId = playerSettlementItemTemplate.Id;
					string entityId = "player_settlement_ghost";
					Exception addError = null;
					settlementVisualEntity = Campaign.Current.MapSceneWrapper.AddPrefabEntityToMapScene(ref ____scene, ref entityId, ref position, ref prefabId, delegate(Exception ex)
					{
						addError = ex;
					});
					if (addError != null && addError is AccessViolationException)
					{
						throw new AccessViolationException(addError.Message, addError);
					}
					GameEntity campaignEntityWithName = ____scene.GetCampaignEntityWithName("player_settlement_ghost");
					if (settlementVisualEntity != campaignEntityWithName)
					{
						LogManager.EventTracer.Trace("settlementVisualEntity != settlementVisualEntity2 - Prefab: '" + prefabId + "', Entity: 'player_settlement_ghost'");
					}
					GameEntity? obj = settlementVisualEntity;
					if (obj != null)
					{
						obj.AddBodyFlags((BodyFlags)(-251674689), true);
					}
					previousVisualUpdateException = null;
					if (settlementVisualEntity == (GameEntity)null)
					{
						Reset();
					}
					settlementVisualEntityChildren.Clear();
					GameEntity? obj2 = settlementVisualEntity;
					if (obj2 != null)
					{
						obj2.GetChildrenRecursive(ref settlementVisualEntityChildren);
					}
					settlementVisualPrefab = prefabId;
					if (settlementVisualPrefab != deepEditPrefab)
					{
						ResetDeepEdits();
					}
					break;
				}
				catch (Exception ex2)
				{
					bool flag = previousVisualUpdateException != null;
					if (!flag)
					{
						previousVisualUpdateException = ex2;
					}
					LogManager.Log.NotifyBad(ex2);
					string environmentVariable = Environment.GetEnvironmentVariable("ALWAYS_BLACKLIST");
					if (!(environmentVariable != null && bool.TryParse(environmentVariable, out result) && result))
					{
						goto IL_03e2;
					}
					LogManager.Log.NotifyNeutral("Always blacklist is enabled. If exception is AccessViolationException, model will be blacklisted and next will be attempted");
					if (!(ex2 is AccessViolationException))
					{
						goto IL_03e2;
					}
					try
					{
						PlayerSettlementItemTemplate playerSettlementItemTemplate2 = availableModels[currentModelOptionIdx];
						LogManager.Log.NotifyBad($"\r\n\r\nTemplate: {playerSettlementItemTemplate2.Id} - Culture: '{playerSettlementItemTemplate2.Culture}', Type: '{playerSettlementItemTemplate2.Type}'");
						Main.Submodule?.UpdateBlacklist(playerSettlementItemTemplate2.Id);
					}
					catch (Exception)
					{
					}
					goto end_IL_02f8;
					IL_03e2:
					if (retry)
					{
						UpdateSettlementVisualEntity(forward);
						break;
					}
					if (ex2 is AccessViolationException || previousVisualUpdateException is AccessViolationException)
					{
						string text = ((object)new TextObject("{=player_settlement_30}Corrupt Template", (Dictionary<string, object>)null)).ToString();
						TextObject val3 = new TextObject("{=player_settlement_31}Player Settlements has encountered a corrupt template. \nPlease screenshot and report this along with the log file at '{LOG_PATH}'. \n{ERROR_DETAIL}\n\nYour game will now have a new emergency save created to avoid crashing. It is highly recommended to close the application before loading.", (Dictionary<string, object>)null);
						val3.SetTextVariable("LOG_PATH", LogManager.Log.LogPath);
						string text2 = "\r\n" + previousVisualUpdateException.Message;
						try
						{
							PlayerSettlementItemTemplate playerSettlementItemTemplate3 = availableModels[currentModelOptionIdx];
							text2 += $"\r\n\r\nTemplate: {playerSettlementItemTemplate3.Id} - Culture: '{playerSettlementItemTemplate3.Culture}', Type: '{playerSettlementItemTemplate3.Type}'";
							Main.Submodule?.UpdateBlacklist(playerSettlementItemTemplate3.Id);
						}
						catch (Exception)
						{
						}
						val3.SetTextVariable("ERROR_DETAIL", text2);
						TextObject val4 = new TextObject("{=player_settlement_32}Close Game", (Dictionary<string, object>)null);
						TextObject val5 = new TextObject("{=player_settlement_33}Attempt Recovery", (Dictionary<string, object>)null);
						InformationManager.ShowInquiry(new InquiryData(text, ((object)val3).ToString(), true, !CampaignOptions.IsIronmanMode, ((object)val4).ToString(), ((object)val5).ToString(), (Action)delegate
						{
							InformationManager.HideInquiry();
							if (!CampaignOptions.IsIronmanMode)
							{
								CampaignEvents.OnSaveOverEvent.AddNonSerializedListener((object)this, (Action<bool, string>)delegate
								{
									Utilities.QuitGame();
								});
								SaveHandler.SaveOnly(overwrite: false);
							}
							else
							{
								Utilities.QuitGame();
							}
						}, (Action)delegate
						{
							InformationManager.HideInquiry();
							if (!CampaignOptions.IsIronmanMode)
							{
								SaveHandler.SaveLoad(SaveHandler.SaveMechanism.Auto);
							}
							else
							{
								Utilities.QuitGame();
							}
						}, "", 0f, (Action)null, (Func<ValueTuple<bool, string>>)null, (Func<ValueTuple<bool, string>>)null), true, false);
						break;
					}
					if (flag)
					{
						throw;
					}
					break;
					end_IL_02f8:;
				}
			}
		}

		private void ShowGhostPortVisualEntity(bool retry = false)
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				LogManager.EventTracer.Trace($"ShowGhostPortVisualEntity noRetry={retry}");
				portPlacementFrame = null;
				Debug.Print("Requesting swap model for port ghost build to: player_settlement_ghost_gate", 2, (DebugColor)10, 17592186044416uL);
				LogManager.EventTracer.Trace("Requesting swap model for port ghost build to: player_settlement_ghost_gate");
				Scene ____scene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
				Vec2 position = MobileParty.MainParty.GetPosition2D;
				string prefabId = "player_settlement_ghost_gate";
				string entityId = "player_settlement_ghost_gate";
				ghostPortVisualEntity = Campaign.Current.MapSceneWrapper.AddPrefabEntityToMapScene(ref ____scene, ref entityId, ref position, ref prefabId);
				GameEntity campaignEntityWithName = ____scene.GetCampaignEntityWithName("player_settlement_ghost_gate");
				if (ghostPortVisualEntity != campaignEntityWithName)
				{
					LogManager.EventTracer.Trace("settlementVisualEntity != settlementVisualEntity2 - Prefab: '" + prefabId + "', Entity: '" + entityId + "'");
				}
				GameEntity? obj = ghostPortVisualEntity;
				if (obj != null)
				{
					obj.AddBodyFlags((BodyFlags)(-251674689), true);
				}
				previousVisualUpdateException = null;
				if (ghostPortVisualEntity == (GameEntity)null)
				{
					ClearEntities();
					ApplyNow();
				}
			}
			catch (Exception e)
			{
				bool flag = previousVisualUpdateException != null;
				if (!flag)
				{
					previousVisualUpdateException = e;
				}
				LogManager.Log.NotifyBad(e);
				if (retry)
				{
					ShowGhostPortVisualEntity();
				}
				else if (flag)
				{
					throw;
				}
			}
		}

		private void ShowGhostGateVisualEntity(bool retry = false)
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				LogManager.EventTracer.Trace($"ShowGhostGateVisualEntity noRetry={retry}");
				gatePlacementFrame = null;
				Debug.Print("Requesting swap model for gate ghost build to: player_settlement_ghost_gate", 2, (DebugColor)10, 17592186044416uL);
				LogManager.EventTracer.Trace("Requesting swap model for gate ghost build to: player_settlement_ghost_gate");
				Scene ____scene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
				Vec2 position = MobileParty.MainParty.GetPosition2D;
				string prefabId = "player_settlement_ghost_gate";
				string entityId = "player_settlement_ghost_gate";
				ghostGateVisualEntity = Campaign.Current.MapSceneWrapper.AddPrefabEntityToMapScene(ref ____scene, ref entityId, ref position, ref prefabId);
				GameEntity campaignEntityWithName = ____scene.GetCampaignEntityWithName("player_settlement_ghost_gate");
				if (ghostGateVisualEntity != campaignEntityWithName)
				{
					LogManager.EventTracer.Trace("settlementVisualEntity != settlementVisualEntity2 - Prefab: '" + prefabId + "', Entity: '" + entityId + "'");
				}
				GameEntity? obj = ghostGateVisualEntity;
				if (obj != null)
				{
					obj.AddBodyFlags((BodyFlags)(-251674689), true);
				}
				previousVisualUpdateException = null;
				if (ghostGateVisualEntity == (GameEntity)null)
				{
					ClearEntities();
					ApplyNow();
				}
			}
			catch (Exception e)
			{
				bool flag = previousVisualUpdateException != null;
				if (!flag)
				{
					previousVisualUpdateException = e;
				}
				LogManager.Log.NotifyBad(e);
				if (retry)
				{
					ShowGhostGateVisualEntity();
				}
				else if (flag)
				{
					throw;
				}
			}
		}

		private List<InquiryElement> GetVillageTypeInquiry()
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Expected O, but got Unknown
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Expected O, but got Unknown
			List<InquiryElement> list = new List<InquiryElement>();
			MBReadOnlyList<VillageType> objects = Game.Current.ObjectManager.GetObjects<VillageType>((Func<VillageType, bool>)((VillageType v) => v != null));
			for (int i = 0; i < ((List<VillageType>)(object)objects).Count; i++)
			{
				try
				{
					VillageType val = ((List<VillageType>)(object)objects)[i];
					if (val != null)
					{
						list.Add(new InquiryElement((object)val, ((object)val.ShortName).ToString(), (ImageIdentifier)new ItemImageIdentifier(val.PrimaryProduction, ""), true, ((object)val.PrimaryProduction.Name).ToString()));
					}
				}
				catch (Exception e)
				{
					LogManager.Log.NotifyBad(e);
				}
			}
			return list;
		}

		private string AutoCalculateVillageType(int villageNumber)
		{
			try
			{
				InquiryElement randomElement = Extensions.GetRandomElement<InquiryElement>((IReadOnlyList<InquiryElement>)GetVillageTypeInquiry());
				object identifier = randomElement.Identifier;
				VillageType val = (VillageType)((identifier is VillageType) ? identifier : null);
				if (val != null)
				{
					return ((MBObjectBase)val).StringId;
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return villageNumber switch
			{
				2 => "lumberjack", 
				3 => "iron_mine", 
				_ => "swine_farm", 
			};
		}

		public IEnumerable<CultureObject> GetCultures(bool mainOnly = false)
		{
			foreach (CultureObject objectTypeList in (List<CultureObject>)(object)MBObjectManager.Instance.GetObjectTypeList<CultureObject>())
			{
				if (!mainOnly || ((BasicCultureObject)objectTypeList).IsMainCulture)
				{
					yield return objectTypeList;
				}
			}
		}

		public IEnumerable<Settlement?> GetPotentialVillageBoundOwners()
		{
			if (PlayerSettlementInfo.Instance == null || Main.Settings == null || PlayerSettlementInfo.Instance.TotalVillages >= int.MaxValue)
			{
				return new List<Settlement>();
			}
			Hero mainHero = Hero.MainHero;
			object obj;
			if (mainHero == null)
			{
				obj = null;
			}
			else
			{
				Clan clan = mainHero.Clan;
				obj = ((clan != null) ? clan.Settlements : null);
			}
			if (obj != null && ((List<Settlement>)(object)Hero.MainHero.Clan.Settlements).Count != 0)
			{
				IEnumerable<Settlement> enumerable = ((IEnumerable<Settlement>)Hero.MainHero.Clan.Settlements).Where((Settlement s) => (s.IsTown && (((List<Village>)(object)s.BoundVillages)?.Count ?? 0) < Main.Settings.MaxVillagesPerTown) || (s.IsCastle && (((List<Village>)(object)s.BoundVillages)?.Count ?? 0) < Main.Settings.MaxVillagesPerCastle));
				return enumerable ?? new List<Settlement>();
			}
			return new List<Settlement>();
		}

		public static (string oldId, string newId) UpdateUniqueGameId()
		{
			string uniqueGameId = Campaign.Current.UniqueGameId;
			string text = MiscHelper.GenerateCampaignId(12);
			SetUniqueGameId(Campaign.Current, text);
			return (oldId: uniqueGameId, newId: Campaign.Current.UniqueGameId);
		}

		public Settlement? CalculateVillageOwner()
		{
			if (PlayerSettlementInfo.Instance == null || Main.Settings == null || PlayerSettlementInfo.Instance.TotalVillages >= int.MaxValue)
			{
				return null;
			}
			if (PlayerSettlementInfo.Instance.PlayerVillages == null)
			{
				PlayerSettlementInfo.Instance.PlayerVillages = new List<PlayerSettlementItem>();
			}
			if (PlayerSettlementInfo.Instance.Towns == null)
			{
				PlayerSettlementInfo.Instance.Towns = new List<PlayerSettlementItem>();
			}
			if (PlayerSettlementInfo.Instance.Castles == null)
			{
				PlayerSettlementInfo.Instance.Castles = new List<PlayerSettlementItem>();
			}
			Hero mainHero = Hero.MainHero;
			object obj;
			if (mainHero == null)
			{
				obj = null;
			}
			else
			{
				Clan clan = mainHero.Clan;
				obj = ((clan != null) ? clan.Settlements : null);
			}
			if (obj == null || ((List<Settlement>)(object)Hero.MainHero.Clan.Settlements).Count == 0)
			{
				return null;
			}
			return ((IEnumerable<Settlement>)Hero.MainHero.Clan.Settlements).FirstOrDefault((Settlement s) => (s.IsTown && (((List<Village>)(object)s.BoundVillages)?.Count ?? 0) < Main.Settings.MaxVillagesPerTown) || (s.IsCastle && (((List<Village>)(object)s.BoundVillages)?.Count ?? 0) < Main.Settings.MaxVillagesPerCastle));
		}
	}
}
