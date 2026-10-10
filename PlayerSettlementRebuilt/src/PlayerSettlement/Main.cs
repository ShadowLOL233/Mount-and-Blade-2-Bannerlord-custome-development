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


namespace BannerlordPlayerSettlement
{
	public class Main : MBSubModuleBase
	{
		public static readonly string Version = $"{typeof(Main).Assembly.GetName().Version}";

		public static readonly Version Feature_StringId_Version = new Version("4.0.0.0");

		public static readonly Version Feature_PrefabId_Version = new Version("5.0.0.0");

		public static readonly Version Feature_Component_Fix_Version = new Version("5.1.0.0");

		public static readonly string Name = typeof(Main).Namespace;

		public static readonly string DisplayName = "Player Settlement";

		public static readonly string HarmonyDomain = "com.b0tlanner.bannerlord." + Name.ToLower();

		public static readonly string ModuleName = "PlayerSettlement";

		public static readonly string DefaultCategory = ModuleName;

		public static readonly string CycleCategory = ModuleName + "Cycle";

		public static readonly string RotateCategory = ModuleName + "Rotate";

		public static readonly string ScaleCategory = ModuleName + "Scale";

		public static readonly string DeleteCategory = ModuleName + "Delete";

		public static Settings? Settings;

		private bool _loaded;

		public static Harmony? Harmony;

		private UIExtender? _extender;

		public static Main? Submodule = null;

		private static List<ICompatibilityPatch> HarmonyCompatPatches = LoadCompatPatches().ToList();

		public Dictionary<string, List<CultureSettlementTemplate>> CultureTemplates;

		private string? _blacklistFile;

		private ModifierKey helpKey;

		private ModifierKey cycleModifierKey;

		private ModifierKey rotateModifierKey;

		private ModifierKey rotateAlternateModifierKey;

		private ModifierKey scaleModifierKey;

		private ModifierKey deepEditToggleKey;

		private ModifierKey deepEditApplyKey;

		private ModifierKey cycleBackKey;

		private ModifierKey cycleNextKey;

		private ModifierKey moveUpKey;

		private ModifierKey moveDownKey;

		private ModifierKey scaleSmallerKey;

		private ModifierKey scaleBiggerKey;

		private ModifierKey rotatePreviousKey;

		private ModifierKey rotateNextKey;

		private ModifierKey rotateBackwardsKey;

		private ModifierKey rotateForwardKey;

		private BasicHotKey deleteKey;

		private BasicHotKey unDeleteModifierKey;

		private static readonly MBList<string?> _blacklistedTemplates = new MBList<string>();

		public HotKeyBase HelpKey => (HotKeyBase)(object)helpKey;

		public HotKeyBase CycleModifierKey => (HotKeyBase)(object)cycleModifierKey;

		public HotKeyBase RotateModifierKey => (HotKeyBase)(object)rotateModifierKey;

		public HotKeyBase RotateAlternateModifierKey => (HotKeyBase)(object)rotateAlternateModifierKey;

		public HotKeyBase ScaleModifierKey => (HotKeyBase)(object)scaleModifierKey;

		public HotKeyBase DeepEditToggleKey => (HotKeyBase)(object)deepEditToggleKey;

		public HotKeyBase DeepEditApplyKey => (HotKeyBase)(object)deepEditApplyKey;

		public HotKeyBase CycleBackKey => (HotKeyBase)(object)cycleBackKey;

		public HotKeyBase CycleNextKey => (HotKeyBase)(object)cycleNextKey;

		public HotKeyBase MoveUpKey => (HotKeyBase)(object)moveUpKey;

		public HotKeyBase MoveDownKey => (HotKeyBase)(object)moveDownKey;

		public HotKeyBase ScaleSmallerKey => (HotKeyBase)(object)scaleSmallerKey;

		public HotKeyBase ScaleBiggerKey => (HotKeyBase)(object)scaleBiggerKey;

		public HotKeyBase RotatePreviousKey => (HotKeyBase)(object)rotatePreviousKey;

		public HotKeyBase RotateNextKey => (HotKeyBase)(object)rotateNextKey;

		public HotKeyBase RotateBackwardsKey => (HotKeyBase)(object)rotateBackwardsKey;

		public HotKeyBase RotateForwardKey => (HotKeyBase)(object)rotateForwardKey;

		public HotKeyBase DeleteKey => (HotKeyBase)(object)deleteKey;

		public HotKeyBase UnDeleteModifierKey => (HotKeyBase)(object)unDeleteModifierKey;

		public string? BlacklistFile => _blacklistFile;

		public static MBReadOnlyList<string?> BlacklistedTemplates => (MBReadOnlyList<string?>)(object)_blacklistedTemplates;

		public static bool IsWarSails => ModuleHelper.IsModuleActive("NavalDLC");

		public Main()
		{
			Submodule = this;
		}

		protected override void OnSubModuleLoad()
		{
			try
			{
				((MBSubModuleBase)this).OnSubModuleLoad();
				LogManager.EnableTracer = true;
				Harmony = new Harmony(HarmonyDomain);
				Harmony.PatchAll();
				foreach (ICompatibilityPatch harmonyCompatPatch in HarmonyCompatPatches)
				{
					harmonyCompatPatch.PatchSubmoduleLoad(Harmony);
				}
				_extender = UIExtender.Create(ModuleName);
				_extender.Register(typeof(Main).Assembly);
				_extender.Enable();
				LogManager.EventTracer.Trace();
			}
			catch (Exception e)
			{
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}
				LogManager.Log.NotifyBad(e);
			}
		}

		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Expected O, but got Unknown
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Expected O, but got Unknown
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Expected O, but got Unknown
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Expected O, but got Unknown
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Expected O, but got Unknown
			try
			{
				if (GlobalSettings<BannerlordPlayerSettlement.Settings>.Instance != null && GlobalSettings<BannerlordPlayerSettlement.Settings>.Instance != Settings)
				{
					Settings = GlobalSettings<BannerlordPlayerSettlement.Settings>.Instance;
					Settings.PropertyChanged += Settings_OnPropertyChanged;
					LogManager.EventTracer.Trace();
				}
				if (!_loaded)
				{
					LogManager.Log.Print("Loaded " + DisplayName, Colours.ImportantTextColor);
					_loaded = true;
					foreach (ICompatibilityPatch harmonyCompatPatch in HarmonyCompatPatches)
					{
						harmonyCompatPatch.PatchAfterMenus(Harmony);
					}
					CultureTemplates = GatherTemplates();
					GameText gameText = Module.CurrentModule.GlobalTextManager.GetGameText("str_key_category_name");
					HotKeyManager val = HotKeyManager.CreateWithOwnCategory(DefaultCategory, DefaultCategory);
					if (val != null)
					{
						TextObject val2 = new TextObject("{=player_settlement_n_85}Player Settlements", (Dictionary<string, object>)null);
						gameText.AddVariationWithId(DefaultCategory, val2, new List<ChoiceTag>());
						helpKey = val.Add<ModifierKey>(new ModifierKey("{=player_settlement_n_73}Show Help", "{=player_settlement_n_74}During player settlement placement when building, will show contextual help info.", (InputKey)59, DefaultCategory));
						cycleModifierKey = val.Add<ModifierKey>(new ModifierKey("{=player_settlement_n_75}Cycle Mode", "{=player_settlement_n_76}During player settlement placement when building, will switch to cycle mode when held.", (InputKey)42, DefaultCategory));
						rotateModifierKey = val.Add<ModifierKey>(new ModifierKey("{=player_settlement_n_77}Rotation Mode", "{=player_settlement_n_78}During player settlement placement when building, will switch to rotation mode when held.", (InputKey)56, DefaultCategory));
						scaleModifierKey = val.Add<ModifierKey>(new ModifierKey("{=player_settlement_n_79}Scale Mode", "{=player_settlement_n_80}During player settlement placement when building, will switch to scaling mode when held.", (InputKey)29, DefaultCategory));
						deepEditToggleKey = val.Add<ModifierKey>(new ModifierKey("{=player_settlement_n_81}Deep Edit Toggle", "{=player_settlement_n_82}During player settlement placement when building, will toggle between placement and deep edit modes.", (InputKey)15, DefaultCategory));
						deepEditApplyKey = val.Add<ModifierKey>(new ModifierKey("{=player_settlement_n_83}Deep Edit Finish", "{=player_settlement_n_84}During player settlement placement when building in deep edit mode, will finish and apply either to gate position selection when applicable or confirmation to finalise", (InputKey)57, DefaultCategory));
					}
					HotKeyManager val3 = HotKeyManager.CreateWithOwnCategory(CycleCategory, CycleCategory);
					if (val3 != null)
					{
						TextObject val4 = new TextObject("{=player_settlement_n_86}Player Settlements: Cycle Mode", (Dictionary<string, object>)null);
						gameText.AddVariationWithId(CycleCategory, val4, new List<ChoiceTag>());
						cycleBackKey = val3.Add<ModifierKey>(new ModifierKey("{=player_settlement_n_87}Cycle Back", "{=player_settlement_n_88}During player settlement placement when building, will cycle to the previous settlement model (or previous submodel when in deep edit mode).", (InputKey)16, CycleCategory));
						cycleNextKey = val3.Add<ModifierKey>(new ModifierKey("{=player_settlement_n_89}Cycle Next", "{=player_settlement_n_90}During player settlement placement when building, will cycle to the next settlement model (or next submodel when in deep edit mode).", (InputKey)18, CycleCategory));
						moveDownKey = val3.Add<ModifierKey>(new ModifierKey("{=player_settlement_n_91}Move Down", "{=player_settlement_n_92}During player settlement placement when building, will move the selected object down.", (InputKey)31, CycleCategory));
						moveUpKey = val3.Add<ModifierKey>(new ModifierKey("{=player_settlement_n_93}Move Up", "{=player_settlement_n_94}During player settlement placement when building, will move the selected object up.", (InputKey)17, CycleCategory));
					}
					HotKeyManager val5 = HotKeyManager.CreateWithOwnCategory(ScaleCategory, ScaleCategory);
					if (val5 != null)
					{
						TextObject val6 = new TextObject("{=player_settlement_n_95}Player Settlements: Scale Mode", (Dictionary<string, object>)null);
						gameText.AddVariationWithId(ScaleCategory, val6, new List<ChoiceTag>());
						scaleSmallerKey = val5.Add<ModifierKey>(new ModifierKey("{=player_settlement_n_96}Scale Down", "{=player_settlement_n_97}During player settlement placement when building, will scale down to the model.", (InputKey)16, ScaleCategory));
						scaleBiggerKey = val5.Add<ModifierKey>(new ModifierKey("{=player_settlement_n_98}Scale Up", "{=player_settlement_n_99}During player settlement placement when building, will scale up to the model.", (InputKey)18, ScaleCategory));
					}
					HotKeyManager val7 = HotKeyManager.CreateWithOwnCategory(RotateCategory, RotateCategory);
					if (val7 != null)
					{
						TextObject val8 = new TextObject("{=player_settlement_n_100}Player Settlements: Rotate Mode", (Dictionary<string, object>)null);
						gameText.AddVariationWithId(RotateCategory, val8, new List<ChoiceTag>());
						rotatePreviousKey = val7.Add<ModifierKey>(new ModifierKey("{=player_settlement_n_101}Rotate Left", "{=player_settlement_n_102}During player settlement placement when building, will rotate the model to the left.", (InputKey)16, RotateCategory));
						rotateNextKey = val7.Add<ModifierKey>(new ModifierKey("{=player_settlement_n_103}Rotate Right", "{=player_settlement_n_104}During player settlement placement when building, will rotate the model to the right.", (InputKey)18, RotateCategory));
						rotateBackwardsKey = val7.Add<ModifierKey>(new ModifierKey("{=player_settlement_n_105}Rotate Back", "{=player_settlement_n_106}During player settlement placement when building, will rotate the model backwards.", (InputKey)31, RotateCategory));
						rotateForwardKey = val7.Add<ModifierKey>(new ModifierKey("{=player_settlement_n_107}Rotate Forward", "{=player_settlement_n_108}During player settlement placement when building, will rotate the model forwards.", (InputKey)17, RotateCategory));
						rotateAlternateModifierKey = val7.Add<ModifierKey>(new ModifierKey("{=player_settlement_n_109}Alternate Rotation Axis", "{=player_settlement_n_110}During player settlement placement when building, will switch to alternate rotation mode when held together with the 'Rotation Modifier'. This changes the axis of rotation when using forwards and backwards rotation keys.", (InputKey)29, RotateCategory));
					}
					HotKeyManager val9 = HotKeyManager.CreateWithOwnCategory(DeleteCategory, DeleteCategory);
					if (val9 != null)
					{
						TextObject val10 = new TextObject("{=player_settlement_n_130}Player Settlements: Deep Edit Mode", (Dictionary<string, object>)null);
						gameText.AddVariationWithId(DeleteCategory, val10, new List<ChoiceTag>());
						deleteKey = val9.Add<BasicHotKey>(new BasicHotKey("{=player_settlement_n_131}Delete Selection", "{=player_settlement_n_132}During player settlement placement when building in deep edit mode, will delete the selected model.", (InputKey)14, DeleteCategory, "delete"));
						unDeleteModifierKey = val9.Add<BasicHotKey>(new BasicHotKey("{=player_settlement_n_133}Un-Delete Mode", "{=player_settlement_n_134}During player settlement placement when building in deep edit mode, when held, the 'Delete Selection' button will instead undo the previous delete.", (InputKey)42, DeleteCategory, "undelete"));
					}
					if (val != null)
					{
						val.Build();
					}
					if (val3 != null)
					{
						val3.Build();
					}
					if (val5 != null)
					{
						val5.Build();
					}
					if (val7 != null)
					{
						val7.Build();
					}
					if (val9 != null)
					{
						val9.Build();
					}
				}
				LogManager.EventTracer.Trace();
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
		}

		private Dictionary<string, List<CultureSettlementTemplate>> GatherTemplates()
		{
			Dictionary<string, List<CultureSettlementTemplate>> dictionary = new Dictionary<string, List<CultureSettlementTemplate>>();
			try
			{
				string[] modulesNames = Utilities.GetModulesNames();
				foreach (ModuleInfo templateModule in GetTemplateModules())
				{
					try
					{
						if (templateModule == null || !modulesNames.Contains(templateModule.Id))
						{
							continue;
						}
						string text = ((templateModule != null) ? templateModule.FolderPath : null);
						if (string.IsNullOrEmpty(text))
						{
							continue;
						}
						string text2 = Path.Combine(text, "SubModule.xml");
						if (!File.Exists(text2))
						{
							continue;
						}
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.Load(text2);
						string text3 = xmlDocument.SelectSingleNode("descendant::PlayerSettlementsTemplates")?.Attributes?["path"]?.Value;
						string text4 = xmlDocument.SelectSingleNode("descendant::PlayerSettlementsWarSailsTemplates")?.Attributes?["path"]?.Value;
						string text5 = xmlDocument.SelectSingleNode("descendant::PlayerSettlementsTemplatesBlacklist")?.Attributes?["path"]?.Value;
						if (!string.IsNullOrEmpty(text5) && File.Exists(Path.Combine(text, text5)))
						{
							IEnumerable<string> enumerable = from line in File.ReadAllLines(Path.Combine(text, text5))
								select line?.Trim() into line
								where !string.IsNullOrEmpty(line)
								select line;
							((List<string>)(object)_blacklistedTemplates).AddRange(enumerable ?? new List<string>());
							if (templateModule.Id == ModuleName)
							{
								_blacklistFile = Path.Combine(text, text5);
							}
						}
						if (!string.IsNullOrEmpty(text3) && Directory.Exists(Path.Combine(text, text3)))
						{
							text3 = Path.Combine(text, text3);
							string[] files = Directory.GetFiles(text3);
							foreach (string text6 in files)
							{
								try
								{
									if (!string.IsNullOrEmpty(text6) && File.Exists(text6))
									{
										XmlDocument xmlDocument2 = new XmlDocument();
										try
										{
											xmlDocument2.Load(text6);
										}
										catch (Exception ex)
										{
											LogManager.EventTracer.Trace(new List<string> { ex.Message, ex.StackTrace });
											goto end_IL_0244;
										}
										CultureSettlementTemplate cultureSettlementTemplate = new CultureSettlementTemplate
										{
											FromModule = templateModule.Id,
											Document = xmlDocument2,
											TemplateModifier = (xmlDocument2.ChildNodes?[0]?.Attributes?["template_modifier"]?.Value ?? ""),
											CultureId = (xmlDocument2.ChildNodes?[0]?.Attributes?["culture_template"]?.Value ?? "")
										};
										if (!dictionary.ContainsKey(cultureSettlementTemplate.CultureId))
										{
											dictionary[cultureSettlementTemplate.CultureId] = new List<CultureSettlementTemplate>();
										}
										dictionary[cultureSettlementTemplate.CultureId].Add(cultureSettlementTemplate);
									}
									end_IL_0244:;
								}
								catch (Exception e)
								{
									LogManager.Log.NotifyBad(e);
								}
							}
							LogManager.Log.NotifyGood("Loaded '" + templateModule.Name + "' Templates");
						}
						if (!IsWarSails || string.IsNullOrEmpty(text4) || !Directory.Exists(Path.Combine(text, text4)))
						{
							continue;
						}
						text4 = Path.Combine(text, text4);
						string[] files2 = Directory.GetFiles(text4);
						foreach (string text7 in files2)
						{
							try
							{
								if (!string.IsNullOrEmpty(text7) && File.Exists(text7))
								{
									XmlDocument xmlDocument3 = new XmlDocument();
									try
									{
										xmlDocument3.Load(text7);
									}
									catch (Exception ex2)
									{
										LogManager.EventTracer.Trace(new List<string> { ex2.Message, ex2.StackTrace });
										goto end_IL_0434;
									}
									CultureSettlementTemplate cultureSettlementTemplate2 = new CultureSettlementTemplate
									{
										FromModule = templateModule.Id,
										Document = xmlDocument3,
										TemplateModifier = (xmlDocument3.ChildNodes?[0]?.Attributes?["template_modifier"]?.Value ?? ""),
										CultureId = (xmlDocument3.ChildNodes?[0]?.Attributes?["culture_template"]?.Value ?? "")
									};
									if (!dictionary.ContainsKey(cultureSettlementTemplate2.CultureId))
									{
										dictionary[cultureSettlementTemplate2.CultureId] = new List<CultureSettlementTemplate>();
									}
									dictionary[cultureSettlementTemplate2.CultureId].Add(cultureSettlementTemplate2);
								}
								end_IL_0434:;
							}
							catch (Exception e2)
							{
								LogManager.Log.NotifyBad(e2);
							}
						}
						LogManager.Log.NotifyGood("Loaded '" + templateModule.Name + "' Templates for War Sails DLC");
					}
					catch (Exception e3)
					{
						LogManager.Log.NotifyBad(e3);
					}
				}
			}
			catch (Exception e4)
			{
				LogManager.Log.NotifyBad(e4);
			}
			return dictionary;
		}

		public void UpdateBlacklist(params string[] newBlacklistItems)
		{
			((List<string>)(object)_blacklistedTemplates).AddRange((IEnumerable<string>)newBlacklistItems);
			if (!string.IsNullOrEmpty(BlacklistFile))
			{
				File.AppendAllText(BlacklistFile, "\r\n");
				File.AppendAllLines(BlacklistFile, newBlacklistItems);
			}
		}

		private IEnumerable<ModuleInfo> GetTemplateModules()
		{
			ModuleInfo thisModule = ModuleHelper.GetModuleInfo(ModuleName);
			return new List<ModuleInfo> { thisModule }.Union(from mi in ModuleHelper.GetModules((Func<ModuleInfo, bool>)null)
				where mi != thisModule && mi.Id != thisModule.Id && mi.DependedModules.FindIndex((DependedModule dp) => (dp).ModuleId == thisModule.Id) != -1
				select mi);
		}

		protected override void OnGameStart(Game game, IGameStarter starterObject)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			try
			{
				((MBSubModuleBase)this).OnGameStart(game, starterObject);
				if (game.GameType is Campaign)
				{
					CampaignGameStarter gameInitializer = (CampaignGameStarter)starterObject;
					AddBehaviors(gameInitializer);
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
		}

		public override void RegisterSubModuleObjects(bool isSavedCampaign)
		{
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Expected O, but got Unknown
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Expected O, but got Unknown
			//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b5: Expected O, but got Unknown
			//IL_09d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e1: Expected O, but got Unknown
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Expected O, but got Unknown
			//IL_0760: Unknown result type (might be due to invalid IL or missing references)
			//IL_076a: Expected O, but got Unknown
			PlayerSettlementBehaviour.OldSaveLoaded = false;
			PlayerSettlementBehaviour.TriggerSaveAfterUpgrade = false;
			if (!(MBObjectManager.Instance != null && isSavedCampaign))
			{
				return;
			}
			MetaV3 metaV = null;
			if (PlayerSettlementBehaviour.Instance != null)
			{
				IDataStore store = Campaign.Current.GetStore((CampaignBehaviorBase)(object)PlayerSettlementBehaviour.Instance);
				PlayerSettlementBehaviour.Instance.LoadEarlySync(store);
				metaV = PlayerSettlementBehaviour.Instance.MetaV3;
			}
			if (metaV == null)
			{
				string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mount and Blade II Bannerlord");
				string name = Name;
				string configDir = Path.Combine(text, "Configs", name, Campaign.Current.UniqueGameId);
				if (!Directory.Exists(configDir))
				{
					return;
				}
				MetaV1_2 metaV1_ = MetaV1_2.ReadFile(text, name, ref configDir);
				if (metaV1_ != null)
				{
					metaV = metaV1_.Convert(configDir);
				}
				if (metaV == null)
				{
					return;
				}
				PlayerSettlementBehaviour.TriggerSaveAfterUpgrade = true;
			}
			for (int i = 0; i < metaV.Towns.Count; i++)
			{
				SettlementMetaV3 settlementMetaV = metaV.Towns[i];
				if (settlementMetaV.BuildTime - 5f > Campaign.CurrentTime)
				{
					PlayerSettlementBehaviour.OldSaveLoaded = true;
					continue;
				}
				if (settlementMetaV.settlement == null || !((MBObjectBase)settlementMetaV.settlement).IsReady)
				{
					XmlDocument document = settlementMetaV.Document;
					MBObjectManager.Instance.LoadXml(document, false);
					string text2 = ((string.IsNullOrEmpty(settlementMetaV.Version) || string.IsNullOrEmpty(settlementMetaV.StringId) || new Version(settlementMetaV.Version).CompareTo(Feature_StringId_Version) < 0) ? $"player_settlement_town_{settlementMetaV.Identifier}" : settlementMetaV.StringId);
					settlementMetaV.settlement = MBObjectManager.Instance.GetObject<Settlement>(text2);
					if (settlementMetaV.settlement != null && !((MBObjectBase)settlementMetaV.settlement).IsReady)
					{
						MBObjectManager.Instance.UnregisterObject((MBObjectBase)(object)settlementMetaV.settlement);
						MBObjectManager.Instance.LoadXml(document, false);
						settlementMetaV.settlement = MBObjectManager.Instance.GetObject<Settlement>(text2);
					}
				}
				if (settlementMetaV.settlement != null && !string.IsNullOrEmpty(settlementMetaV.DisplayName))
				{
					settlementMetaV.settlement.SetName(new TextObject(settlementMetaV.DisplayName, (Dictionary<string, object>)null));
				}
				for (int j = 0; j < settlementMetaV.Villages.Count; j++)
				{
					SettlementMetaV3 settlementMetaV2 = settlementMetaV.Villages[j];
					if (settlementMetaV2.BuildTime - 5f > Campaign.CurrentTime)
					{
						PlayerSettlementBehaviour.OldSaveLoaded = true;
						continue;
					}
					XmlDocument document2 = settlementMetaV2.Document;
					string text3 = ((string.IsNullOrEmpty(settlementMetaV2.Version) || string.IsNullOrEmpty(settlementMetaV2.StringId) || new Version(settlementMetaV2.Version).CompareTo(Feature_StringId_Version) < 0) ? $"player_settlement_town_{settlementMetaV.Identifier}_village_{settlementMetaV2.Identifier}" : settlementMetaV2.StringId);
					settlementMetaV2.settlement = MBObjectManager.Instance.GetObject<Settlement>(text3);
					if (settlementMetaV2.settlement == null || !((MBObjectBase)settlementMetaV2.settlement).IsReady)
					{
						MBObjectManager.Instance.LoadXml(document2, false);
						settlementMetaV2.settlement = MBObjectManager.Instance.GetObject<Settlement>(text3);
						if (settlementMetaV2.settlement != null && !((MBObjectBase)settlementMetaV2.settlement).IsReady)
						{
							MBObjectManager.Instance.LoadXml(document2, false);
							settlementMetaV2.settlement = MBObjectManager.Instance.GetObject<Settlement>(text3);
						}
					}
					if (settlementMetaV2.settlement != null && !string.IsNullOrEmpty(settlementMetaV2.DisplayName))
					{
						settlementMetaV2.settlement.SetName(new TextObject(settlementMetaV2.DisplayName, (Dictionary<string, object>)null));
					}
				}
			}
			for (int k = 0; k < metaV.Castles.Count; k++)
			{
				int num = k + 1;
				SettlementMetaV3 settlementMetaV3 = metaV.Castles[k];
				if (settlementMetaV3.BuildTime - 5f > Campaign.CurrentTime)
				{
					PlayerSettlementBehaviour.OldSaveLoaded = true;
					continue;
				}
				XmlDocument document3 = settlementMetaV3.Document;
				if (settlementMetaV3.settlement == null || !((MBObjectBase)settlementMetaV3.settlement).IsReady)
				{
					string text4 = ((string.IsNullOrEmpty(settlementMetaV3.Version) || string.IsNullOrEmpty(settlementMetaV3.StringId) || new Version(settlementMetaV3.Version).CompareTo(Feature_StringId_Version) < 0) ? $"player_settlement_castle_{settlementMetaV3.Identifier}" : settlementMetaV3.StringId);
					MBObjectManager.Instance.LoadXml(document3, false);
					settlementMetaV3.settlement = MBObjectManager.Instance.GetObject<Settlement>(text4);
					if (settlementMetaV3.settlement != null && !((MBObjectBase)settlementMetaV3.settlement).IsReady)
					{
						MBObjectManager.Instance.LoadXml(document3, false);
						settlementMetaV3.settlement = MBObjectManager.Instance.GetObject<Settlement>(text4);
					}
				}
				if (settlementMetaV3.settlement != null && !string.IsNullOrEmpty(settlementMetaV3.DisplayName))
				{
					settlementMetaV3.settlement.SetName(new TextObject(settlementMetaV3.DisplayName, (Dictionary<string, object>)null));
				}
				for (int l = 0; l < settlementMetaV3.Villages.Count; l++)
				{
					SettlementMetaV3 settlementMetaV4 = settlementMetaV3.Villages[l];
					if (settlementMetaV4.BuildTime - 5f > Campaign.CurrentTime)
					{
						PlayerSettlementBehaviour.OldSaveLoaded = true;
						continue;
					}
					XmlDocument document4 = settlementMetaV4.Document;
					string text5 = ((string.IsNullOrEmpty(settlementMetaV4.Version) || string.IsNullOrEmpty(settlementMetaV4.StringId) || new Version(settlementMetaV4.Version).CompareTo(Feature_StringId_Version) < 0) ? $"player_settlement_castle_{settlementMetaV3.Identifier}_village_{settlementMetaV4.Identifier}" : settlementMetaV4.StringId);
					settlementMetaV4.settlement = MBObjectManager.Instance.GetObject<Settlement>(text5);
					if (settlementMetaV4.settlement == null || !((MBObjectBase)settlementMetaV4.settlement).IsReady)
					{
						MBObjectManager.Instance.LoadXml(document4, false);
						settlementMetaV4.settlement = MBObjectManager.Instance.GetObject<Settlement>(text5);
						if (settlementMetaV4.settlement != null && !((MBObjectBase)settlementMetaV4.settlement).IsReady)
						{
							MBObjectManager.Instance.LoadXml(document4, false);
							settlementMetaV4.settlement = MBObjectManager.Instance.GetObject<Settlement>(text5);
						}
					}
					if (settlementMetaV4.settlement != null && !string.IsNullOrEmpty(settlementMetaV4.DisplayName))
					{
						settlementMetaV4.settlement.SetName(new TextObject(settlementMetaV4.DisplayName, (Dictionary<string, object>)null));
					}
				}
			}
			if (metaV.ExtraVillages != null)
			{
				for (int m = 0; m < metaV.ExtraVillages.Count; m++)
				{
					SettlementMetaV3 settlementMetaV5 = metaV.ExtraVillages[m];
					if (settlementMetaV5.BuildTime - 5f > Campaign.CurrentTime)
					{
						PlayerSettlementBehaviour.OldSaveLoaded = true;
						continue;
					}
					string stringId = settlementMetaV5.StringId;
					MBObjectManager.Instance.LoadXml(settlementMetaV5.Document, false);
					settlementMetaV5.settlement = MBObjectManager.Instance.GetObject<Settlement>(stringId);
					if (settlementMetaV5.settlement != null && !((MBObjectBase)settlementMetaV5.settlement).IsReady)
					{
						MBObjectManager.Instance.LoadXml(settlementMetaV5.Document, false);
						settlementMetaV5.settlement = MBObjectManager.Instance.GetObject<Settlement>(stringId);
					}
					if (settlementMetaV5.settlement != null && !string.IsNullOrEmpty(settlementMetaV5.DisplayName))
					{
						settlementMetaV5.settlement.SetName(new TextObject(settlementMetaV5.DisplayName, (Dictionary<string, object>)null));
					}
				}
			}
			if (metaV.OverwriteSettlements == null)
			{
				return;
			}
			for (int n = 0; n < metaV.OverwriteSettlements.Count; n++)
			{
				SettlementMetaV3 settlementMetaV6 = metaV.OverwriteSettlements[n];
				if (settlementMetaV6.BuildTime - 5f > Campaign.CurrentTime)
				{
					PlayerSettlementBehaviour.OldSaveLoaded = true;
					continue;
				}
				string stringId2 = settlementMetaV6.StringId;
				MBObjectManager.Instance.LoadXml(settlementMetaV6.Document, false);
				settlementMetaV6.settlement = MBObjectManager.Instance.GetObject<Settlement>(stringId2);
				if (settlementMetaV6.settlement != null && !((MBObjectBase)settlementMetaV6.settlement).IsReady)
				{
					MBObjectManager.Instance.LoadXml(settlementMetaV6.Document, false);
					settlementMetaV6.settlement = MBObjectManager.Instance.GetObject<Settlement>(stringId2);
				}
				if (settlementMetaV6.settlement != null && !string.IsNullOrEmpty(settlementMetaV6.DisplayName))
				{
					settlementMetaV6.settlement.SetName(new TextObject(settlementMetaV6.DisplayName, (Dictionary<string, object>)null));
				}
			}
		}

		private void AddBehaviors(CampaignGameStarter gameInitializer)
		{
			try
			{
				gameInitializer.AddBehavior((CampaignBehaviorBase)(object)new PlayerSettlementBehaviour());
				foreach (ICompatibilityPatch harmonyCompatPatch in HarmonyCompatPatches)
				{
					harmonyCompatPatch.AddBehaviors(gameInitializer);
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
		}

		protected static void Settings_OnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			try
			{
				if (sender is BannerlordPlayerSettlement.Settings && args.PropertyName == "SAVE_TRIGGERED")
				{
					try
					{
						((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
					}
					catch (Exception)
					{
					}
					LogManager.EventTracer.Trace();
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
		}

		private static IEnumerable<ICompatibilityPatch> LoadCompatPatches()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			foreach (Assembly assembly in array)
			{
				Type[] types = assembly.GetTypes();
				foreach (Type type in types)
				{
					if (typeof(ICompatibilityPatch).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
					{
						object inst = null;
						try
						{
							inst = type.CreateInstance();
						}
						catch (Exception ex)
						{
							Exception e = ex;
							LogManager.Log.NotifyBad(e);
						}
						if (inst is ICompatibilityPatch compatibilityPatch)
						{
							yield return compatibilityPatch;
						}
					}
				}
			}
		}
	}
	public class SaveHandler
	{
		public enum SaveMechanism
		{
			Overwrite,
			Auto,
			Temporary
		}

		private static SaveHandler _instance = new SaveHandler();

		private static PropertyInfo ActiveSaveSlotNameProp = AccessTools.Property(typeof(MBSaveLoad), "ActiveSaveSlotName");

		private static MethodInfo GetNextAvailableSaveNameMethod = AccessTools.Method(typeof(MBSaveLoad), "GetNextAvailableSaveName");

		public static SaveHandler Instance => _instance;

		public static void SaveLoad(SaveMechanism saveMechanism = SaveMechanism.Overwrite, Action<SaveMechanism, string>? afterSave = null)
		{
			Instance.SaveAndLoad(saveMechanism, afterSave);
		}

		public static void SaveOnly(bool overwrite = true)
		{
			Instance.Save(overwrite);
		}

		public void SaveAndLoad(SaveMechanism saveMechanism = SaveMechanism.Overwrite, Action<SaveMechanism, string>? afterSave = null)
		{
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Expected O, but got Unknown
			Action<SaveMechanism, string> afterSave2 = afterSave;
			string saveName = (string)ActiveSaveSlotNameProp.GetValue(null);
			if (saveName == null)
			{
				saveName = (string)GetNextAvailableSaveNameMethod.Invoke(null, new object[0]);
				ActiveSaveSlotNameProp.SetValue(null, saveName);
			}
			CampaignEvents.OnSaveOverEvent.AddNonSerializedListener((object)Instance, (Action<bool, string>)delegate(bool b, string s)
			{
				Instance.ApplyInternal(saveMechanism, saveName, b, s, afterSave2);
			});
			if (saveMechanism == SaveMechanism.Overwrite)
			{
				Campaign.Current.SaveHandler.SaveAs(saveName);
			}
			else
			{
				Campaign.Current.SaveHandler.SaveAs(saveName + ((object)new TextObject("{=player_settlement_n_02} (auto)", (Dictionary<string, object>)null)).ToString());
			}
		}

		public void Save(bool overwrite = true)
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected O, but got Unknown
			string text = (string)ActiveSaveSlotNameProp.GetValue(null);
			if (text == null)
			{
				text = (string)GetNextAvailableSaveNameMethod.Invoke(null, new object[0]);
				ActiveSaveSlotNameProp.SetValue(null, text);
			}
			if (overwrite)
			{
				Campaign.Current.SaveHandler.SaveAs(text);
			}
			else
			{
				Campaign.Current.SaveHandler.SaveAs(text + ((object)new TextObject("{=player_settlement_n_02} (auto)", (Dictionary<string, object>)null)).ToString());
			}
		}

		private void ApplyInternal(SaveMechanism saveMechanism, string originalSaveName, bool isSaveSuccessful, string newSaveGameName, Action<SaveMechanism, string>? afterSave = null)
		{
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Expected O, but got Unknown
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Expected O, but got Unknown
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Expected O, but got Unknown
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Expected O, but got Unknown
			((IMbEventBase)CampaignEvents.OnSaveOverEvent).ClearListeners((object)this);
			if (!isSaveSuccessful)
			{
				return;
			}
			afterSave?.Invoke(saveMechanism, newSaveGameName);
			SaveGameFileInfo saveFileWithName = MBSaveLoad.GetSaveFileWithName(newSaveGameName);
			if (saveFileWithName != null && !saveFileWithName.IsCorrupted)
			{
				SandBoxSaveHelper.TryLoadSave(saveFileWithName, (Action<LoadResult>)delegate(LoadResult loadResult)
				{
					if (saveMechanism == SaveMechanism.Temporary)
					{
						MBSaveLoad.DeleteSaveGame(newSaveGameName);
						SaveGameFileInfo saveFileWithName2 = MBSaveLoad.GetSaveFileWithName(originalSaveName);
						if (saveFileWithName2 != null && !saveFileWithName2.IsCorrupted)
						{
							ActiveSaveSlotNameProp.SetValue(null, originalSaveName);
						}
					}
					StartGame(loadResult);
				}, (Action)null);
			}
			else
			{
				InformationManager.ShowInquiry(new InquiryData(((object)new TextObject("{=oZrVNUOk}Error", (Dictionary<string, object>)null)).ToString(), ((object)new TextObject("{=t6W3UjG0}Save game file appear to be corrupted. Try starting a new campaign or load another one from Saved Games menu.", (Dictionary<string, object>)null)).ToString(), true, false, ((object)new TextObject("{=yS7PvrTD}OK", (Dictionary<string, object>)null)).ToString(), (string)null, (Action)null, (Action)null, "", 0f, (Action)null, (Func<ValueTuple<bool, string>>)null, (Func<ValueTuple<bool, string>>)null), false, false);
			}
		}

		public void StartGame(LoadResult loadResult)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Expected O, but got Unknown
			if (Game.Current != null)
			{
				ScreenManager.PopScreen();
				GameStateManager.Current.CleanStates(0);
				GameStateManager.Current = Module.CurrentModule.GlobalGameStateManager;
			}
			MBSaveLoad.OnStartGame(loadResult);
			MBGameManager.StartNewGame((MBGameManager)new SandBoxGameManager(loadResult));
		}
	}
	public class Settings : AttributeGlobalSettings<Settings>
	{
		private const string CreateNewSave_Hint = "{=player_settlement_n_135}Create a new save when building. By default, a temporary save will be made, or if the below is disabled, the current active save will be overwritten instead.  [ Default: OFF ]";

		private const string UseTempSave_Hint = "{=player_settlement_n_136}Make a temporary save and remove after load. When disabled, will overwrite the current save unless the above is enabled.  [ Default: ON ]";

		private const string HideButtonUntilReady_Hint = "{=player_settlement_n_05}Always hides the build panel until requirements are met for at least one build option. \r\nWhen using dialogue options, only available options will show, otherwise the unavailable ones will have hints describing why they are not available.  [ Default: OFF ]";

		private const string ImmersiveMode_Hint = "{=player_settlement_n_07}Always hides the build panel. Building can only be started by discussing with a companion (if enabled).  [ Default: OFF ]";

		private const string NoDialogue_Hint = "{=player_settlement_n_09}Removes the build conversation options. Building can only be started using the build panel (if enabled).  [ Default: OFF ]";

		private const string SettlementPlacement_Hint = "{=player_settlement_n_11}Allows choosing the position and rotation to place the settlement. When disabled will use the player party current position.  [ Default: ON ]";

		private const string MouseRotationModifier_Hint = "{=player_settlement_n_13}Speed at which mouse movement rotates settlement. \r\nSettlement rotation applies when 'Alt' is held.  [ Default: 50% ]";

		private const string KeyRotationModifier_Hint = "{=player_settlement_n_15}Speed at which rotation keys affect settlement when rotating. \r\nDefault game rotation keys are 'Q' and 'E', unless remapped. \r\nSettlement rotation applies when 'Alt' is held. [ Default: 100% ]";

		private const string SelectedCultureOnly_Hint = "{=player_settlement_n_17}Will limit settlement options to selected culture only. \r\nOtherwise will allow settlement options for all cultures. \r\nCycle visually between options by holding 'Shift' and using cycle keys. \r\nDefault game cycle keys are 'Q' and 'E', unless remapped.  [ Default: ON ]";

		private const string CycleSpeed_Hint = "{=player_settlement_n_19}Speed at which settlements will visually cycle during placement while holding 'Shift' and a cycle key, or speed at which settlements will scale while holding 'Ctrl' and a scale key.\r\nCycle visually between options by holding 'Shift' and using cycle keys. \r\nScale by holding 'Ctrl' and using scale keys. \r\nDefault cycle and scale keys are 'Q' and 'E', unless remapped.  [ Default: 50% ]";

		private const string AllowGatePosition_Hint = "{=player_settlement_n_68}Allow choosing settlement gate position when applicable. When not enabled, all settlement entry is at the center.  [ Default: ON ]";

		private const string DisableAutoHints_Hint = "{=player_settlement_n_71}Will disable automatic hints shown during settlement placement. Hints can still be shown by pressing the help key, by default 'F1' unless remapped.  [ Default: OFF ]";

		private const string HintDurationSeconds_Hint = "{=player_settlement_n_117}Duration to display hints (in seconds). This is in addition to the base time that the game applies by default. [ Default: 3 ]";

		private const string Enabled_Hint = "{=player_settlement_n_21}Enables Player Settlement mod and adds the option map screen.  [ Default: ON ]";

		private const string RequireClanTier_Hint = "{=player_settlement_n_22}Requires clan to be specified tier before being allowed to create a settlement.  [ Default: ON ]";

		private const string RequiredClanTier_Hint = "{=player_settlement_n_24}Specified tier required before being allowed to create a settlement.  [ Default: 4 ]";

		private const string RequireGold_Hint = "{=player_settlement_n_26}Requires a specified cost in local currency to build new town.  [ Default: ON ]";

		private const string RequiredGold_Hint = "{=player_settlement_n_28}Specified cost in local currency to build new town.  [ Default: 10 000 ]";

		private const string RebuildTownRequiredGold_Hint = "{=player_settlement_n_124}Specified cost in local currency to rebuild a town.  [ Default: 5 000 ]";

		private const string InstantBuild_Hint = "{=player_settlement_n_30}Skip required build duration and instantly completes town construction.  [ Default: OFF ]";

		private const string BuildDurationDays_Hint = "{=player_settlement_n_32}Specified days before town is done being built.  [ Default: 7 ]";

		private const string BuildCastleDurationDays_Hint = "{=player_settlement_n_111}Specified days before castle is done being built.  [ Default: 7 ]";

		private const string BuildVillageDurationDays_Hint = "{=player_settlement_n_112}Specified days before village is done being built.  [ Default: 3 ]";

		private const string RebuildTownDurationDays_Hint = "{=player_settlement_n_118}Specified days before town is done being rebuilt.  [ Default: 5 ]";

		private const string RebuildCastleDurationDays_Hint = "{=player_settlement_n_119}Specified days before castle is done being rebuilt.  [ Default: 4 ]";

		private const string RebuildVillageDurationDays_Hint = "{=player_settlement_n_120}Specified days before village is done being rebuilt.  [ Default: 2 ]";

		private const string ForcePlayerCulture_Hint = "{=player_settlement_n_34}Will use the player culture for the town. By default when this is OFF, the town culture can be chosen.  [ Default: OFF ]";

		private const string RequireVillageGold_Hint = "{=player_settlement_n_36}Requires a specified cost in local currency to build new village.  [ Default: ON ]";

		private const string RequiredVillageGold_Hint = "{=player_settlement_n_38}Specified cost in local currency to build new village.  [ Default: 3 000 ]";

		private const string RebuildVillageRequiredGold_Hint = "{=player_settlement_n_126}Specified cost in local currency to rebuild a village.  [ Default: 1 000 ]";

		private const string AutoAllocateVillageType_Hint = "{=player_settlement_n_40}Will automatically determine the type of village, which determines its primary product. By default when this is OFF, the type can be chosen.  [ Default: OFF ]";

		private const string AutoDetermineVillageOwner_Hint = "{=player_settlement_n_42}Will automatically determine the bound town/castle for the village. By default when this is OFF, the bound settlement can be chosen.  [ Default: OFF ]";

		private const string RequireCastleGold_Hint = "{=player_settlement_n_44}Requires a specified cost in local currency to build new castle.  [ Default: ON ]";

		private const string RequiredCastleGold_Hint = "{=player_settlement_n_46}Specified cost in local currency to build new castle.  [ Default: 7 500 ]";

		private const string RebuildCastleRequiredGold_Hint = "{=player_settlement_n_128}Specified cost in local currency to rebuild a castle.  [ Default: 3 500 ]";

		private const string MaxTowns_Hint = "{=player_settlement_n_48}Maximum number of player built towns allowed. At least one town is required.  [ Default: 10 ]";

		private const string MaxVillagesPerTown_Hint = "{=player_settlement_n_50}Maximum number of player built villages per town allowed.  [ Default: 5 ]";

		private const string MaxCastles_Hint = "{=player_settlement_n_52}Maximum number of player built castles allowed. At least one town is required first.  [ Default: 15 ]";

		private const string MaxVillagesPerCastle_Hint = "{=player_settlement_n_54}Maximum number of player built villages per castle allowed.  [ Default: 4 ]";

		private const string SingleConstruction_Hint = "{=player_settlement_n_56}Will require in progress construction to finish before being allowed to build next settlement. By default when this is OFF, multiple settlement construction can be done at once.  [ Default: OFF ]";

		private const string AddInitialGarrison_Hint = "{=player_settlement_n_58}Will add an initial garrison for new towns and castles.  [ Default: ON ]";

		private const string AddInitialMilitia_Hint = "{=player_settlement_n_60}Will add initial militia for new settlements.  [ Default: ON ]";

		private const string AddInitialNotables_Hint = "{=player_settlement_n_62}Will add initial notables for new towns and villages.  [ Default: ON ]";

		public const int HardMaxTowns = 150;

		public const int HardMaxVillagesPerTown = 50;

		public const int HardMaxCastles = 150;

		public const int HardMaxVillagesPerCastle = 50;

		public const int HardMaxVillages = int.MaxValue;

		public override string Id => Main.Name + "_v1";

		public override string DisplayName => Main.DisplayName;

		public override string FolderName => Main.Name;

		public override string FormatType => "json";

		[SettingPropertyBool("{=player_settlement_n_04}Create new save on build", HintText = "{=player_settlement_n_135}Create a new save when building. By default, a temporary save will be made, or if the below is disabled, the current active save will be overwritten instead.  [ Default: OFF ]", RequireRestart = false, Order = 0, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_66}Saves", GroupOrder = 0)]
		public bool CreateNewSave { get; set; } = false;


		[SettingPropertyBool("{=player_settlement_n_137}Use a temporary save for build and load", HintText = "{=player_settlement_n_136}Make a temporary save and remove after load. When disabled, will overwrite the current save unless the above is enabled.  [ Default: ON ]", RequireRestart = false, Order = 1, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_66}Saves")]
		public bool UseTempSave { get; set; } = true;


		internal SaveHandler.SaveMechanism SaveMechanism => CreateNewSave ? SaveHandler.SaveMechanism.Auto : (UseTempSave ? SaveHandler.SaveMechanism.Temporary : SaveHandler.SaveMechanism.Overwrite);

		[SettingPropertyBool("{=player_settlement_n_06}Always Hide Until Ready", HintText = "{=player_settlement_n_05}Always hides the build panel until requirements are met for at least one build option. \r\nWhen using dialogue options, only available options will show, otherwise the unavailable ones will have hints describing why they are not available.  [ Default: OFF ]", RequireRestart = false, Order = 0, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_67}User Interface", GroupOrder = 1)]
		public bool HideButtonUntilReady { get; set; } = false;


		[SettingPropertyBool("{=player_settlement_n_08}Immersive Mode", HintText = "{=player_settlement_n_07}Always hides the build panel. Building can only be started by discussing with a companion (if enabled).  [ Default: OFF ]", RequireRestart = false, Order = 1, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_67}User Interface")]
		public bool ImmersiveMode { get; set; } = false;


		[SettingPropertyBool("{=player_settlement_n_10}No Conversation Options", HintText = "{=player_settlement_n_09}Removes the build conversation options. Building can only be started using the build panel (if enabled).  [ Default: OFF ]", RequireRestart = false, Order = 1, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_67}User Interface")]
		public bool NoDialogue { get; set; } = false;


		[SettingPropertyBool("{=player_settlement_n_12}Enable Settlement Placement", HintText = "{=player_settlement_n_11}Allows choosing the position and rotation to place the settlement. When disabled will use the player party current position.  [ Default: ON ]", RequireRestart = false, Order = 20, IsToggle = true)]
		[SettingPropertyGroup("{=player_settlement_n_70}Settlement Placement", GroupOrder = 2)]
		public bool SettlementPlacement { get; set; } = true;


		[SettingPropertyFloatingInteger("{=player_settlement_n_14}Mouse Rotation Speed", 0.01f, 10f, "#0%", HintText = "{=player_settlement_n_13}Speed at which mouse movement rotates settlement. \r\nSettlement rotation applies when 'Alt' is held.  [ Default: 50% ]", RequireRestart = false, Order = 21)]
		[SettingPropertyGroup("{=player_settlement_n_70}Settlement Placement")]
		public float MouseRotationModifier { get; set; } = 0.5f;


		[SettingPropertyFloatingInteger("{=player_settlement_n_16}Key Rotation Speed", 0.01f, 10f, "#0%", HintText = "{=player_settlement_n_15}Speed at which rotation keys affect settlement when rotating. \r\nDefault game rotation keys are 'Q' and 'E', unless remapped. \r\nSettlement rotation applies when 'Alt' is held. [ Default: 100% ]", RequireRestart = false, Order = 22)]
		[SettingPropertyGroup("{=player_settlement_n_70}Settlement Placement")]
		public float KeyRotationModifier { get; set; } = 1f;


		[SettingPropertyBool("{=player_settlement_n_18}Selected Culture Only", HintText = "{=player_settlement_n_17}Will limit settlement options to selected culture only. \r\nOtherwise will allow settlement options for all cultures. \r\nCycle visually between options by holding 'Shift' and using cycle keys. \r\nDefault game cycle keys are 'Q' and 'E', unless remapped.  [ Default: ON ]", RequireRestart = false, Order = 23, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_70}Settlement Placement")]
		public bool SelectedCultureOnly { get; set; } = true;


		[SettingPropertyFloatingInteger("{=player_settlement_n_20}Settlement Cycle Speed", 0.01f, 10f, "#0%", HintText = "{=player_settlement_n_19}Speed at which settlements will visually cycle during placement while holding 'Shift' and a cycle key, or speed at which settlements will scale while holding 'Ctrl' and a scale key.\r\nCycle visually between options by holding 'Shift' and using cycle keys. \r\nScale by holding 'Ctrl' and using scale keys. \r\nDefault cycle and scale keys are 'Q' and 'E', unless remapped.  [ Default: 50% ]", RequireRestart = false, Order = 24)]
		[SettingPropertyGroup("{=player_settlement_n_70}Settlement Placement")]
		public float CycleSpeed { get; set; } = 2f;


		[SettingPropertyBool("{=player_settlement_n_69}Allow Setting Gate Position", HintText = "{=player_settlement_n_68}Allow choosing settlement gate position when applicable. When not enabled, all settlement entry is at the center.  [ Default: ON ]", RequireRestart = false, Order = 25, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_70}Settlement Placement")]
		public bool AllowGatePosition { get; set; } = true;


		[SettingPropertyBool("{=player_settlement_n_72}Disable Automatic Hints", HintText = "{=player_settlement_n_71}Will disable automatic hints shown during settlement placement. Hints can still be shown by pressing the help key, by default 'F1' unless remapped.  [ Default: OFF ]", RequireRestart = false, Order = 26, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_70}Settlement Placement")]
		public bool DisableAutoHints { get; set; } = false;


		[SettingPropertyInteger("{=player_settlement_n_116}Hint Duration", 1, 120, "0", HintText = "{=player_settlement_n_117}Duration to display hints (in seconds). This is in addition to the base time that the game applies by default. [ Default: 3 ]", RequireRestart = false, Order = 27)]
		[SettingPropertyGroup("{=player_settlement_n_70}Settlement Placement")]
		public int HintDurationSeconds { get; set; } = 3;


		[SettingPropertyBool("{=player_settlement_n_64}Enabled", HintText = "{=player_settlement_n_21}Enables Player Settlement mod and adds the option map screen.  [ Default: ON ]", RequireRestart = true, Order = 0, IsToggle = true)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements", GroupOrder = 3)]
		public bool Enabled { get; set; } = true;


		[SettingPropertyBool("{=player_settlement_n_23}Require Clan Tier", HintText = "{=player_settlement_n_22}Requires clan to be specified tier before being allowed to create a settlement.  [ Default: ON ]", RequireRestart = false, Order = 1, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public bool RequireClanTier { get; set; } = true;


		[SettingPropertyInteger("{=player_settlement_n_25}Required Clan Tier", 1, 6, "0", HintText = "{=player_settlement_n_24}Specified tier required before being allowed to create a settlement.  [ Default: 4 ]", RequireRestart = false, Order = 2)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public int RequiredClanTier { get; set; } = 4;


		[SettingPropertyBool("{=player_settlement_n_27}Require Town Cost", HintText = "{=player_settlement_n_26}Requires a specified cost in local currency to build new town.  [ Default: ON ]", RequireRestart = false, Order = 3, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public bool RequireGold { get; set; } = true;


		[SettingPropertyInteger("{=player_settlement_n_29}Required Town Cost", 1, 100000000, "0", HintText = "{=player_settlement_n_28}Specified cost in local currency to build new town.  [ Default: 10 000 ]", RequireRestart = false, Order = 4)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public int RequiredGold { get; set; } = 10000;


		[SettingPropertyInteger("{=player_settlement_n_125}Required Town Rebuild Cost", 1, 100000000, "0", HintText = "{=player_settlement_n_124}Specified cost in local currency to rebuild a town.  [ Default: 5 000 ]", RequireRestart = false, Order = 4)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public int RebuildTownRequiredGold { get; set; } = 5000;


		[SettingPropertyBool("{=player_settlement_n_31}Instant Build", HintText = "{=player_settlement_n_30}Skip required build duration and instantly completes town construction.  [ Default: OFF ]", RequireRestart = false, Order = 5, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public bool InstantBuild { get; set; } = false;


		[SettingPropertyInteger("{=player_settlement_n_113}Build Duration Days (Town)", 1, 2480, "0", HintText = "{=player_settlement_n_32}Specified days before town is done being built.  [ Default: 7 ]", RequireRestart = false, Order = 6)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public int BuildDurationDays { get; set; } = 7;


		[SettingPropertyInteger("{=player_settlement_n_114}Build Duration Days (Castle)", 1, 2480, "0", HintText = "{=player_settlement_n_111}Specified days before castle is done being built.  [ Default: 7 ]", RequireRestart = false, Order = 6)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public int BuildCastleDurationDays { get; set; } = 7;


		[SettingPropertyInteger("{=player_settlement_n_115}Build Duration Days (Village)", 1, 2480, "0", HintText = "{=player_settlement_n_112}Specified days before village is done being built.  [ Default: 3 ]", RequireRestart = false, Order = 6)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public int BuildVillageDurationDays { get; set; } = 3;


		[SettingPropertyInteger("{=player_settlement_n_121}Rebuild Duration Days (Town)", 1, 2480, "0", HintText = "{=player_settlement_n_118}Specified days before town is done being rebuilt.  [ Default: 5 ]", RequireRestart = false, Order = 6)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public int RebuildTownDurationDays { get; set; } = 5;


		[SettingPropertyInteger("{=player_settlement_n_122}Rebuild Duration Days (Castle)", 1, 2480, "0", HintText = "{=player_settlement_n_119}Specified days before castle is done being rebuilt.  [ Default: 4 ]", RequireRestart = false, Order = 6)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public int RebuildCastleDurationDays { get; set; } = 7;


		[SettingPropertyInteger("{=player_settlement_n_123}Rebuild Duration Days (Village)", 1, 2480, "0", HintText = "{=player_settlement_n_120}Specified days before village is done being rebuilt.  [ Default: 2 ]", RequireRestart = false, Order = 6)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public int RebuildVillageDurationDays { get; set; } = 2;


		[SettingPropertyBool("{=player_settlement_n_35}Use Player Culture", HintText = "{=player_settlement_n_34}Will use the player culture for the town. By default when this is OFF, the town culture can be chosen.  [ Default: OFF ]", RequireRestart = false, Order = 7, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public bool ForcePlayerCulture { get; set; } = false;


		[SettingPropertyBool("{=player_settlement_n_37}Require Village Cost", HintText = "{=player_settlement_n_36}Requires a specified cost in local currency to build new village.  [ Default: ON ]", RequireRestart = false, Order = 8, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public bool RequireVillageGold { get; set; } = true;


		[SettingPropertyInteger("{=player_settlement_n_39}Required Village Cost", 1, 100000000, "0", HintText = "{=player_settlement_n_38}Specified cost in local currency to build new village.  [ Default: 3 000 ]", RequireRestart = false, Order = 9)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public int RequiredVillageGold { get; set; } = 3000;


		[SettingPropertyInteger("{=player_settlement_n_127}Required Village Rebuild Cost", 1, 100000000, "0", HintText = "{=player_settlement_n_126}Specified cost in local currency to rebuild a village.  [ Default: 1 000 ]", RequireRestart = false, Order = 9)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public int RebuildVillageRequiredGold { get; set; } = 1000;


		[SettingPropertyBool("{=player_settlement_n_41}Auto Allocate Village Type", HintText = "{=player_settlement_n_40}Will automatically determine the type of village, which determines its primary product. By default when this is OFF, the type can be chosen.  [ Default: OFF ]", RequireRestart = false, Order = 10, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public bool AutoAllocateVillageType { get; set; } = false;


		[SettingPropertyBool("{=player_settlement_n_43}Auto Determine Village Bound Settlement", HintText = "{=player_settlement_n_42}Will automatically determine the bound town/castle for the village. By default when this is OFF, the bound settlement can be chosen.  [ Default: OFF ]", RequireRestart = false, Order = 11, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public bool AutoDetermineVillageOwner { get; set; } = false;


		[SettingPropertyBool("{=player_settlement_n_45}Require Castle Cost", HintText = "{=player_settlement_n_44}Requires a specified cost in local currency to build new castle.  [ Default: ON ]", RequireRestart = false, Order = 12, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public bool RequireCastleGold { get; set; } = true;


		[SettingPropertyInteger("{=player_settlement_n_47}Required Castle Cost", 1, 100000000, "0", HintText = "{=player_settlement_n_46}Specified cost in local currency to build new castle.  [ Default: 7 500 ]", RequireRestart = false, Order = 13)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public int RequiredCastleGold { get; set; } = 7500;


		[SettingPropertyInteger("{=player_settlement_n_129}Required Castle Rebuild Cost", 1, 100000000, "0", HintText = "{=player_settlement_n_128}Specified cost in local currency to rebuild a castle.  [ Default: 3 500 ]", RequireRestart = false, Order = 13)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public int RebuildCastleRequiredGold { get; set; } = 3500;


		[SettingPropertyInteger("{=player_settlement_n_49}Maximum Allowed Towns", 1, 150, "0", HintText = "{=player_settlement_n_48}Maximum number of player built towns allowed. At least one town is required.  [ Default: 10 ]", RequireRestart = false, Order = 14)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public int MaxTowns { get; set; } = 150;


		[SettingPropertyInteger("{=player_settlement_n_51}Maximum Allowed Villages Per Town", 0, 50, "0", HintText = "{=player_settlement_n_50}Maximum number of player built villages per town allowed.  [ Default: 5 ]", RequireRestart = false, Order = 15)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public int MaxVillagesPerTown { get; set; } = 50;


		[SettingPropertyInteger("{=player_settlement_n_53}Maximum Allowed Castles", 0, 150, "0", HintText = "{=player_settlement_n_52}Maximum number of player built castles allowed. At least one town is required first.  [ Default: 15 ]", RequireRestart = false, Order = 16)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public int MaxCastles { get; set; } = 150;


		[SettingPropertyInteger("{=player_settlement_n_55}Maximum Allowed Villages Per Castle", 0, 50, "0", HintText = "{=player_settlement_n_54}Maximum number of player built villages per castle allowed.  [ Default: 4 ]", RequireRestart = false, Order = 17)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public int MaxVillagesPerCastle { get; set; } = 50;


		[SettingPropertyBool("{=player_settlement_n_57}Single Construction At a Time", HintText = "{=player_settlement_n_56}Will require in progress construction to finish before being allowed to build next settlement. By default when this is OFF, multiple settlement construction can be done at once.  [ Default: OFF ]", RequireRestart = false, Order = 19, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public bool SingleConstruction { get; set; } = false;


		[SettingPropertyBool("{=player_settlement_n_59}Add Initial Garrison", HintText = "{=player_settlement_n_58}Will add an initial garrison for new towns and castles.  [ Default: ON ]", RequireRestart = false, Order = 20, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public bool AddInitialGarrison { get; set; } = true;


		[SettingPropertyBool("{=player_settlement_n_61}Add Initial Militia", HintText = "{=player_settlement_n_60}Will add initial militia for new settlements.  [ Default: ON ]", RequireRestart = false, Order = 21, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public bool AddInitialMilitia { get; set; } = true;


		[SettingPropertyBool("{=player_settlement_n_63}Add Initial Notables", HintText = "{=player_settlement_n_62}Will add initial notables for new towns and villages.  [ Default: ON ]", RequireRestart = false, Order = 22, IsToggle = false)]
		[SettingPropertyGroup("{=player_settlement_n_65}Player Settlements")]
		public bool AddInitialNotables { get; set; } = true;

	}
}
