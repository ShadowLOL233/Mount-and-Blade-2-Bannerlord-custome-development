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

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints | DebuggableAttribute.DebuggingModes.EnableEditAndContinue)]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyCompany("PlayerSettlement")]
[assembly: AssemblyConfiguration("Beta_Debug")]
[assembly: AssemblyFileVersion("7.5.0.0")]
[assembly: AssemblyInformationalVersion("7.5.0")]
[assembly: AssemblyProduct("PlayerSettlement")]
[assembly: AssemblyTitle("PlayerSettlement")]
[assembly: AssemblyVersion("7.5.0.0")]
namespace Microsoft.CodeAnalysis
{
	[CompilerGenerated]
	[Microsoft.CodeAnalysis.Embedded]
	internal sealed class EmbeddedAttribute : Attribute
	{
	}
}
namespace System.Runtime.CompilerServices
{
	[CompilerGenerated]
	[Microsoft.CodeAnalysis.Embedded]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = false)]
	internal sealed class NullableAttribute : Attribute
	{
		public readonly byte[] NullableFlags;

		public NullableAttribute(byte P_0)
		{
			NullableFlags = new byte[1] { P_0 };
		}

		public NullableAttribute(byte[] P_0)
		{
			NullableFlags = P_0;
		}
	}
	[CompilerGenerated]
	[Microsoft.CodeAnalysis.Embedded]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	internal sealed class NullableContextAttribute : Attribute
	{
		public readonly byte Flag;

		public NullableContextAttribute(byte P_0)
		{
			Flag = P_0;
		}
	}
}
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
				where mi != thisModule && mi.Id != thisModule.Id && mi.DependedModules.FindIndex((DependedModule dp) => ((DependedModule)(ref dp)).ModuleId == thisModule.Id) != -1
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
namespace BannerlordPlayerSettlement.Utils
{
	public static class Colours
	{
		public static Color ImportantTextColor => Color.FromUint(15822118u);

		public static Color Error => new Color(45390f, 8670f, 8670f, 1f);

		public static Color Warn => new Color(48195f, 9690f, 0f, 1f);

		public static Color Aqua => new Color(0f, 1f, 1f, 1f);

		public static Color Black => new Color(0f, 0f, 0f, 1f);

		public static Color Blue => new Color(0f, 0f, 1f, 1f);

		public static Color Gray => new Color(0.5f, 0.5f, 0.5f, 1f);

		public static Color Green => new Color(0f, 1f, 0f, 1f);

		public static Color LimeGreen => new Color(0.5f, 1f, 0f, 1f);

		public static Color Orange => new Color(1f, 0.5f, 0f, 1f);

		public static Color Purple => new Color(1f, 0f, 1f, 1f);

		public static Color Red => new Color(1f, 0f, 0f, 1f);

		public static Color SkyBlue => new Color(0f, 0.5f, 1f, 1f);

		public static Color White => new Color(1f, 1f, 1f, 1f);

		public static Color Yellow => new Color(1f, 1f, 0f, 1f);

		public static Color Magenta => Color.FromUint(16711807u);

		public static Color ForestGreen => Color.FromUint(16711807u);
	}
	internal sealed class GameLog
	{
		private const string BeginMultiLine = "=======================================================================================================================\\";

		private const string BeginMultiLineDebug = "===================================================   D E B U G   =====================================================\\";

		private const string EndMultiLine = "=======================================================================================================================/";

		public readonly string Module;

		public readonly string LogDir;

		public readonly string LogFile;

		public readonly string LogPath;

		private bool _lastMsgWasMultiLine = false;

		private TextWriter Writer { get; set; }

		public void Info(string text)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			Print(text, Colours.White);
		}

		public void Info(List<string> text)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			Print(text, Colours.White);
		}

		public void Debug(string text)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			Print(text, Colours.Magenta, isDebug: true);
		}

		public void Debug(List<string> text)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			Print(text, Colours.Magenta, isDebug: true);
		}

		public void NotifyBad(Exception e)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			LogManager.EventTracer.Trace(e.Message, 2);
			Debug.SetCrashReportCustomString(e.Message);
			Debug.SetCrashReportCustomStack(e.StackTrace);
			Print(e.Message, Colours.Red, isDebug: false, onlyDisplay: true);
			ToFile(new List<string> { e.Message, e.StackTrace });
		}

		public void SilentException(Exception e)
		{
			LogManager.EventTracer.Trace(e.Message, 2);
			Debug.SetCrashReportCustomString(e.Message);
			Debug.SetCrashReportCustomStack(e.StackTrace);
			ToFile(new List<string> { e.Message, e.StackTrace });
		}

		public void NotifyBad(string text)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			Print(text, Colours.Red);
		}

		public void NotifyBad(List<string> text)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			Print(text, Colours.Red);
		}

		public void NotifyNeutral(string text)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			Print(text, Colours.SkyBlue);
		}

		public void NotifyNeutral(List<string> text)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			Print(text, Colours.SkyBlue);
		}

		public void NotifyGood(string text)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			Print(text, Colours.ForestGreen);
		}

		public void NotifyGood(List<string> text)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			Print(text, Colours.ForestGreen);
		}

		public void Print(string text, Color color, bool isDebug = false, bool onlyDisplay = false)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			InformationManager.DisplayMessage(new InformationMessage(text, color));
			if (!onlyDisplay)
			{
				ToFile(text, isDebug);
			}
		}

		public void Print(List<string> lines, Color color, bool isDebug = false, bool onlyDisplay = false)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			foreach (string line in lines)
			{
				InformationManager.DisplayMessage(new InformationMessage(line, color));
			}
			if (!onlyDisplay)
			{
				ToFile(lines, isDebug);
			}
		}

		public void ToFile(string line, bool isDebug = false)
		{
			if (Writer != null)
			{
				_lastMsgWasMultiLine = false;
				Writer.WriteLine(isDebug ? (">> " + line) : line);
				Writer.Flush();
			}
		}

		public void ToFile(List<string> lines, bool isDebug = false)
		{
			if (Writer == null || lines.Count == 0)
			{
				return;
			}
			if (lines.Count == 1)
			{
				ToFile(lines[0], isDebug);
				return;
			}
			if (!_lastMsgWasMultiLine)
			{
				Writer.WriteLine(isDebug ? "===================================================   D E B U G   =====================================================\\" : "=======================================================================================================================\\");
			}
			_lastMsgWasMultiLine = true;
			foreach (string line in lines)
			{
				Writer.WriteLine(line);
			}
			Writer.WriteLine("=======================================================================================================================/");
			Writer.Flush();
		}

		public GameLog(string moduleName, bool truncate = false, string? logName = null)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrEmpty(moduleName))
			{
				throw new ArgumentException("moduleName: string cannot be null or empty");
			}
			string fileFullPath = Common.PlatformFileHelper.GetFileFullPath(new PlatformFilePath(new PlatformDirectoryPath((PlatformFileType)1, "logs"), string.IsNullOrEmpty(logName) ? (moduleName + ".log") : (moduleName + "." + logName + ".log")));
			Module = moduleName + "." + GetType().Name;
			LogDir = Path.GetDirectoryName(fileFullPath);
			LogFile = Path.GetFileName(fileFullPath);
			LogPath = fileFullPath;
			if (!Directory.Exists(LogDir))
			{
				Directory.CreateDirectory(LogDir);
			}
			bool flag = File.Exists(LogPath);
			try
			{
				Writer = TextWriter.Synchronized(new StreamWriter(LogPath, !truncate, Encoding.UTF8, 32768));
			}
			catch (Exception arg)
			{
				Console.WriteLine("================================  EXCEPTION  ================================");
				Console.WriteLine(Module + ": Failed to create StreamWriter!");
				Console.WriteLine("Path: " + LogPath);
				Console.WriteLine($"Truncate: {truncate}");
				Console.WriteLine($"Preexisting Path: {flag}");
				Console.WriteLine("Exception Information:");
				Console.WriteLine($"{arg}");
				Console.WriteLine("=============================================================================");
				throw;
			}
			Writer.NewLine = "\n";
			List<string> list = new List<string> { $"{Module} created at: {DateTimeOffset.Now:yyyy/MM/dd H:mm zzz}" };
			if (flag && !truncate)
			{
				Writer.WriteLine("\n");
				list.Add("NOTE: Any prior log messages in this file may have no relation to this session.");
			}
			ToFile(list, isDebug: true);
		}
	}
	internal static class LogManager
	{
		internal static class EventTracer
		{
			private static readonly ConcurrentDictionary<string, bool> _stackTraceMap = new ConcurrentDictionary<string, bool>();

			[MethodImpl(MethodImplOptions.NoInlining)]
			internal static void Trace(string extraInfo, int framesToSkip = 1)
			{
				Trace(new List<string> { extraInfo }, framesToSkip + 1);
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			internal static void Trace(List<string>? extraInfo = null, int framesToSkip = 1)
			{
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				if (!EnableTracer)
				{
					return;
				}
				StackTrace stackTrace = new StackTrace(framesToSkip, fNeedFileInfo: true);
				StackFrame[] frames = stackTrace.GetFrames();
				MethodBase method = frames[0].GetMethod();
				List<string> list = new List<string>
				{
					$"Code Event Invoked: {method.DeclaringType}.{method.Name}",
					$"Real Timestamp:     {DateTime.Now:MM/dd H:mm:ss.fff}"
				};
				if (Campaign.Current != null)
				{
					list.AddRange(new List<string>
					{
						$"Campaign Time:              {CampaignTime.Now}",
						$"Campaign Current Time:      {Campaign.CurrentTime}"
					});
				}
				string text = stackTrace.ToString();
				if (text.Length > 2)
				{
					text = text.Replace("\r\n", "\n");
					text = text.Remove(text.Length - 1, 1);
					if (_stackTraceMap.TryAdd(text, value: true))
					{
						list.AddRange(new List<string>
						{
							string.Empty,
							"Stack Trace:",
							text
						});
					}
				}
				if (extraInfo != null && extraInfo.Count > 0)
				{
					list.AddRange(new List<string>
					{
						string.Empty,
						"Extra Information:"
					});
					if (extraInfo.Count > 1)
					{
						list.Add(string.Empty);
					}
					list.AddRange(extraInfo);
				}
				Log.ToFile(list, isDebug: true);
			}
		}

		internal static GameLog Log = new GameLog(Main.Name, truncate: true, "debug");

		internal static bool EnableTracer { get; set; } = false;

	}
	public static class ModulePrefab
	{
		public static XmlDocument LoadResourceAsXML(string embedPath)
		{
			using Stream stream = typeof(ModulePrefab).Assembly.GetManifestResourceStream(embedPath);
			if (stream == null)
			{
				throw new NullReferenceException("Could not find embed resource '" + embedPath + "'!");
			}
			using XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings
			{
				IgnoreComments = true
			});
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(reader);
			return xmlDocument;
		}

		public static string LoadModuleFile(string moduleName, params string[] filePaths)
		{
			string moduleFilePath = GetModuleFilePath(moduleName, filePaths);
			if (!File.Exists(moduleFilePath))
			{
				throw new FileNotFoundException("Unable to find specified file", moduleFilePath);
			}
			return File.ReadAllText(moduleFilePath);
		}

		public static XmlDocument LoadModuleFileAsXML(string moduleName, params string[] filePaths)
		{
			string moduleFilePath = GetModuleFilePath(moduleName, filePaths);
			if (!File.Exists(moduleFilePath))
			{
				throw new FileNotFoundException("Unable to find specified file", moduleFilePath);
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(File.ReadAllText(moduleFilePath));
			return xmlDocument;
		}

		private static string GetModuleFilePath(string moduleName, string[] filePaths)
		{
			List<string> list = new List<string>();
			list.Add(ModuleHelper.GetModuleInfo(moduleName).FolderPath);
			list.AddRange(filePaths);
			return Path.Combine(list.ToArray());
		}
	}
}
namespace BannerlordPlayerSettlement.UI.Widgets
{
	public class PlayerSettlementInfoWidget : Widget
	{
		public delegate void MapBarExtendStateChangeEvent(bool newState);

		private ButtonWidget _extendButtonWidget;

		private bool _isInfoBarExtended;

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MapBarExtendStateChangeEvent m_OnMapInfoBarExtendStateChange;

		[Editor(false)]
		public ButtonWidget ExtendButtonWidget
		{
			get
			{
				return _extendButtonWidget;
			}
			set
			{
				if (_extendButtonWidget != value)
				{
					_extendButtonWidget = value;
					((PropertyOwnerObject)this).OnPropertyChanged<ButtonWidget>(value, "ExtendButtonWidget");
					if (!_extendButtonWidget.ClickEventHandlers.Contains(OnExtendButtonClick))
					{
						_extendButtonWidget.ClickEventHandlers.Add(OnExtendButtonClick);
					}
					RefreshVerticalVisual();
				}
			}
		}

		[Editor(false)]
		public bool IsInfoBarExtended
		{
			get
			{
				return _isInfoBarExtended;
			}
			set
			{
				if (_isInfoBarExtended != value)
				{
					_isInfoBarExtended = value;
					((PropertyOwnerObject)this).OnPropertyChanged(value, "IsInfoBarExtended");
					MapBarExtendStateChangeEvent onMapInfoBarExtendStateChange = this.OnMapInfoBarExtendStateChange;
					if (onMapInfoBarExtendStateChange != null)
					{
						onMapInfoBarExtendStateChange.Invoke(IsInfoBarExtended);
					}
				}
			}
		}

		public event MapBarExtendStateChangeEvent OnMapInfoBarExtendStateChange
		{
			[CompilerGenerated]
			add
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				MapBarExtendStateChangeEvent val = this.m_OnMapInfoBarExtendStateChange;
				MapBarExtendStateChangeEvent val2;
				do
				{
					val2 = val;
					MapBarExtendStateChangeEvent value2 = (MapBarExtendStateChangeEvent)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
					val = Interlocked.CompareExchange(ref this.m_OnMapInfoBarExtendStateChange, value2, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				MapBarExtendStateChangeEvent val = this.m_OnMapInfoBarExtendStateChange;
				MapBarExtendStateChangeEvent val2;
				do
				{
					val2 = val;
					MapBarExtendStateChangeEvent value2 = (MapBarExtendStateChangeEvent)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
					val = Interlocked.CompareExchange(ref this.m_OnMapInfoBarExtendStateChange, value2, val2);
				}
				while (val != val2);
			}
		}

		public PlayerSettlementInfoWidget(UIContext context)
			: base(context)
		{
			((Widget)this).AddState("Disabled");
		}

		private void OnExtendButtonClick(Widget widget)
		{
			IsInfoBarExtended = !IsInfoBarExtended;
			RefreshBarExtendState();
		}

		protected override void OnUpdate(float dt)
		{
			((Widget)this).OnUpdate(dt);
			RefreshBarExtendState();
		}

		private void RefreshBarExtendState()
		{
			if (IsInfoBarExtended && ((Widget)this).CurrentState != "Extended")
			{
				((Widget)this).SetState("Extended");
				RefreshVerticalVisual();
			}
			else if (!IsInfoBarExtended && ((Widget)this).CurrentState != "Default")
			{
				((Widget)this).SetState("Default");
				RefreshVerticalVisual();
			}
		}

		private void RefreshVerticalVisual()
		{
			foreach (Style style in ((BrushWidget)ExtendButtonWidget).Brush.Styles)
			{
				for (int i = 0; i < style.LayerCount; i++)
				{
					style.GetLayer(i).VerticalFlip = !IsInfoBarExtended;
				}
			}
		}
	}
}
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
					return ((CampaignTime)(ref buildEnd5)).IsFuture;
				}) || PlayerSettlementInfo.Instance.Towns.Any(delegate(PlayerSettlementItem t)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					CampaignTime buildEnd3 = t.BuildEnd;
					return ((CampaignTime)(ref buildEnd3)).IsFuture || t.Villages.Any(delegate(PlayerSettlementItem v)
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0006: Unknown result type (might be due to invalid IL or missing references)
						CampaignTime buildEnd4 = v.BuildEnd;
						return ((CampaignTime)(ref buildEnd4)).IsFuture;
					});
				}) || PlayerSettlementInfo.Instance.Castles.Any(delegate(PlayerSettlementItem c)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					CampaignTime buildEnd = c.BuildEnd;
					return ((CampaignTime)(ref buildEnd)).IsFuture || c.Villages.Any(delegate(PlayerSettlementItem v)
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0006: Unknown result type (might be due to invalid IL or missing references)
						CampaignTime buildEnd2 = v.BuildEnd;
						return ((CampaignTime)(ref buildEnd2)).IsFuture;
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
			((MapInfoVM)this).RefreshValues();
			CalculateEnabled();
		}

		public override void OnFinalize()
		{
			((ViewModel)this).OnFinalize();
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
namespace BannerlordPlayerSettlement.UI.Extensions
{
	[PrefabExtension("MapBar", "/Prefab/Window/Widget/Children")]
	[PrefabExtension("NewAncientMapBar", "/Prefab/Window/Widget/Children")]
	[PrefabExtension("NewMapBar", "/Prefab/Window/Widget/Children")]
	public class MapBarExtension : PrefabExtensionInsertPatch
	{
		public override InsertType Type => (InsertType)3;

		[PrefabExtensionFileName(false)]
		public string GetPrefabExtension => "PlayerSettlementInfoWidget.xml";
	}
	[PrefabExtension("MapBar", "descendant::VisualDefinitions")]
	[PrefabExtension("NewAncientMapBar", "descendant::VisualDefinitions")]
	[PrefabExtension("NewMapBar", "descendant::VisualDefinitions")]
	public class MapBarVisualDefnitionExtension : PrefabExtensionInsertPatch
	{
		public override InsertType Type => (InsertType)3;

		[PrefabExtensionFileName(false)]
		public string GetPrefabExtension => "PlayerSettlementInfoWidgetVisualDefinition.xml";
	}
}
namespace BannerlordPlayerSettlement.Saves
{
	internal sealed class CustomSaveableTypeDefiner : SaveableTypeDefiner
	{
		public const int SaveBaseId_b0tlanner0 = 300711200;

		public const int SaveBaseId = 300711204;

		public CustomSaveableTypeDefiner()
			: base(300711204)
		{
		}

		protected override void DefineClassTypes()
		{
			((SaveableTypeDefiner)this).DefineClassTypes();
			((SaveableTypeDefiner)this).AddClassDefinition(typeof(Mat3Saveable), 9, (IObjectResolver)null);
			((SaveableTypeDefiner)this).AddClassDefinition(typeof(Vec3Saveable), 10, (IObjectResolver)null);
			((SaveableTypeDefiner)this).AddClassDefinition(typeof(TransformSaveable), 11, (IObjectResolver)null);
			((SaveableTypeDefiner)this).AddClassDefinition(typeof(DeepTransformEdit), 12, (IObjectResolver)null);
			((SaveableTypeDefiner)this).AddClassDefinition(typeof(PlayerSettlementItem), 3, (IObjectResolver)null);
			((SaveableTypeDefiner)this).AddClassDefinition(typeof(OverwriteSettlementItem), 13, (IObjectResolver)null);
			((SaveableTypeDefiner)this).AddClassDefinition(typeof(PlayerSettlementInfo), 2, (IObjectResolver)null);
			((SaveableTypeDefiner)this).AddClassDefinition(typeof(SettlementMetaV3), 5, (IObjectResolver)null);
			((SaveableTypeDefiner)this).AddClassDefinition(typeof(MetaV3), 7, (IObjectResolver)null);
		}

		protected override void DefineContainerDefinitions()
		{
			((SaveableTypeDefiner)this).DefineContainerDefinitions();
			((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(List<DeepTransformEdit>));
			((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(DeepTransformEdit[]));
			((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(List<PlayerSettlementItem>));
			((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(PlayerSettlementItem[]));
			((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(List<OverwriteSettlementItem>));
			((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(OverwriteSettlementItem[]));
			((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(List<SettlementMetaV3>));
			((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(SettlementMetaV3[]));
		}
	}
	public class DeepTransformEdit
	{
		[SaveableField(601)]
		public int Index = -1;

		[SaveableField(602)]
		public string Name = "";

		[SaveableField(603)]
		public TransformSaveable? Transform;

		[SaveableField(604)]
		public bool IsDeleted;
	}
	public class Mat3Saveable
	{
		[SaveableField(301)]
		public float sx;

		[SaveableField(302)]
		public float sy;

		[SaveableField(303)]
		public float sz;

		[SaveableField(304)]
		public float fx;

		[SaveableField(305)]
		public float fy;

		[SaveableField(306)]
		public float fz;

		[SaveableField(307)]
		public float ux;

		[SaveableField(308)]
		public float uy;

		[SaveableField(309)]
		public float uz;

		public Mat3Saveable()
		{
		}

		public Mat3Saveable(Mat3 source)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			sx = source.s.x;
			sy = source.s.y;
			sz = source.s.z;
			fx = source.f.x;
			fy = source.f.y;
			fz = source.f.z;
			ux = source.u.x;
			uy = source.u.y;
			uz = source.u.z;
		}

		public Mat3 ToMat3()
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			return new Mat3(sx, sy, sz, fx, fy, fz, ux, uy, uz);
		}

		public static implicit operator Mat3(Mat3Saveable source)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return source.ToMat3();
		}

		public static implicit operator Mat3Saveable?(Mat3? source)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			if (!source.HasValue)
			{
				return null;
			}
			return new Mat3Saveable(source.Value);
		}
	}
	public class MetaV1_2
	{
		public Settlement? playerSettlement;

		public string displayName;

		public string identifier;

		public float buildTime;

		public string? savedModuleVersionRaw;

		public string savedModuleVersion;

		public int villageCount;

		public List<PlayerSettlementItem> villages = new List<PlayerSettlementItem>();

		public static MetaV1_2? ReadFile(string userDir, string moduleName, ref string configDir)
		{
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			MetaV1_2 metaV1_ = new MetaV1_2();
			string text = Path.Combine(configDir, "meta.bin");
			if (File.Exists(text))
			{
				string text2 = File.ReadAllText(text);
				string contents = text2 ?? "";
				string[] array = text2.Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
				metaV1_.identifier = array[0].Base64Decode();
				metaV1_.displayName = array[1].Base64Decode();
				if (!float.TryParse(array[2].Base64Decode(), out metaV1_.buildTime) && !float.TryParse(array[2], out metaV1_.buildTime))
				{
					LogManager.Log.NotifyBad("Unable to read save data!");
					LogManager.EventTracer.Trace("Unable to read save data!\r\n    " + text2 + "\r\n");
					PlayerSettlementBehaviour.OldSaveLoaded = true;
					return null;
				}
				metaV1_.savedModuleVersionRaw = ((array.Length > 3) ? array[3] : null);
				metaV1_.savedModuleVersion = ((array.Length > 3) ? array[3].Base64Decode() : "0.0.0");
				metaV1_.villageCount = ((array.Length > 4) ? (int.TryParse(array[4].Base64Decode(), out metaV1_.villageCount) ? metaV1_.villageCount : 0) : 0);
				if (metaV1_.villageCount > 0 && array.Length > 5)
				{
					string[] array2 = array[5].Base64Decode().Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
					for (int i = 0; i < array2.Length; i++)
					{
						string[] array3 = array2[i].Base64Decode().Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
						string itemIdentifier = array3[0].Base64Decode();
						string settlementName = array3[1].Base64Decode();
						float builtAt = float.Parse(array3[2].Base64Decode());
						metaV1_.villages.Add(new PlayerSettlementItem
						{
							ItemIdentifier = itemIdentifier,
							SettlementName = settlementName,
							BuiltAt = builtAt
						});
					}
				}
				if (metaV1_.savedModuleVersion != Main.Version)
				{
					if (metaV1_.savedModuleVersion == "1.0.0.0")
					{
						if (metaV1_.buildTime - 5f > Campaign.CurrentTime)
						{
							PlayerSettlementBehaviour.OldSaveLoaded = true;
							return null;
						}
						PlayerSettlementBehaviour.UpdateUniqueGameId();
						string sourceDirName = configDir;
						configDir = Path.Combine(userDir, "Configs", moduleName, Campaign.Current.UniqueGameId);
						Directory.Move(sourceDirName, configDir);
						text = Path.Combine(configDir, "meta.bin");
						PlayerSettlementBehaviour.TriggerSaveAfterUpgrade = true;
					}
					if (array.Length > 3)
					{
						array[3] = Main.Version.Base64Encode();
						text2 = string.Join("\r\n", array);
					}
					else
					{
						text2 += "\r\n";
						text2 += Main.Version.Base64Encode();
					}
					File.WriteAllText(text + ".bak", contents);
					File.WriteAllText(text, text2);
					LogManager.Log.Print("Updated " + Main.DisplayName + " to " + Main.Version, Colours.Purple);
				}
				metaV1_.playerSettlement = MBObjectManager.Instance.GetObject<Settlement>(metaV1_.identifier);
				return metaV1_;
			}
			return null;
		}

		public MetaV3? Convert(string configDir)
		{
			string text = Path.Combine(configDir, "PlayerSettlement.xml");
			if (!File.Exists(text))
			{
				return null;
			}
			string oldTownVillageXML1 = Path.Combine(configDir, "PlayerSettlementVillage_1.xml");
			string oldTownVillageXML2 = Path.Combine(configDir, "PlayerSettlementVillage_2.xml");
			string oldTownVillageXML3 = Path.Combine(configDir, "PlayerSettlementVillage_3.xml");
			MetaV3 metaV = new MetaV3
			{
				SavedModuleVersion = Main.Version,
				Towns = new List<SettlementMetaV3>(),
				Castles = new List<SettlementMetaV3>(),
				ExtraVillages = new List<SettlementMetaV3>()
			};
			string xML = File.ReadAllText(text);
			metaV.Towns.Add(new SettlementMetaV3
			{
				XML = xML,
				BuildTime = buildTime,
				DisplayName = displayName,
				Identifier = int.Parse(identifier.Replace("player_settlement_town_", "")),
				settlement = playerSettlement,
				Villages = villages.Select(delegate(PlayerSettlementItem v, int i)
				{
					string xML2 = null;
					switch (i)
					{
					case 0:
						if (File.Exists(oldTownVillageXML1))
						{
							xML2 = File.ReadAllText(oldTownVillageXML1);
						}
						break;
					case 1:
						if (File.Exists(oldTownVillageXML2))
						{
							xML2 = File.ReadAllText(oldTownVillageXML2);
						}
						break;
					case 2:
						if (File.Exists(oldTownVillageXML3))
						{
							xML2 = File.ReadAllText(oldTownVillageXML3);
						}
						break;
					}
					return new SettlementMetaV3
					{
						XML = xML2,
						BuildTime = v.BuiltAt,
						DisplayName = v.SettlementName,
						Identifier = i + 1,
						settlement = v.Settlement,
						Villages = new List<SettlementMetaV3>()
					};
				}).ToList()
			});
			string text2 = Path.Combine(configDir, "meta.bin");
			if (File.Exists(text2))
			{
				if (File.Exists(text2 + ".bak"))
				{
					File.Delete(text2 + ".bak");
				}
				File.Move(text2, text2 + ".bak");
			}
			if (File.Exists(text))
			{
				if (File.Exists(text + ".bak"))
				{
					File.Delete(text + ".bak");
				}
				File.Move(text, text + ".bak");
			}
			if (File.Exists(oldTownVillageXML1))
			{
				if (File.Exists(oldTownVillageXML1 + ".bak"))
				{
					File.Delete(oldTownVillageXML1 + ".bak");
				}
				File.Move(oldTownVillageXML1, oldTownVillageXML1 + ".bak");
			}
			if (File.Exists(oldTownVillageXML2))
			{
				if (File.Exists(oldTownVillageXML2 + ".bak"))
				{
					File.Delete(oldTownVillageXML2 + ".bak");
				}
				File.Move(oldTownVillageXML2, oldTownVillageXML2 + ".bak");
			}
			if (File.Exists(oldTownVillageXML3))
			{
				if (File.Exists(oldTownVillageXML3 + ".bak"))
				{
					File.Delete(oldTownVillageXML3 + ".bak");
				}
				File.Move(oldTownVillageXML3, oldTownVillageXML3 + ".bak");
			}
			PlayerSettlementBehaviour.TriggerSaveAfterUpgrade = true;
			return metaV;
		}
	}
	[JsonObject(/*Could not decode attribute arguments.*/)]
	public class MetaV3
	{
		[JsonProperty]
		[SaveableProperty(301)]
		public string SavedModuleVersion { get; set; } = Main.Version;


		[JsonProperty]
		[SaveableProperty(302)]
		public List<SettlementMetaV3> Towns { get; set; } = new List<SettlementMetaV3>();


		[JsonProperty]
		[SaveableProperty(303)]
		public List<SettlementMetaV3> Castles { get; set; } = new List<SettlementMetaV3>();


		[JsonProperty]
		[SaveableProperty(304)]
		public List<SettlementMetaV3> ExtraVillages { get; set; } = new List<SettlementMetaV3>();


		[JsonProperty]
		[SaveableProperty(305)]
		public List<SettlementMetaV3> OverwriteSettlements { get; set; } = new List<SettlementMetaV3>();


		public static MetaV3? Create(PlayerSettlementInfo playerSettlementInfo)
		{
			if (playerSettlementInfo?.Towns == null)
			{
				return null;
			}
			MetaV3 metaV = new MetaV3
			{
				SavedModuleVersion = Main.Version,
				Castles = new List<SettlementMetaV3>(),
				Towns = new List<SettlementMetaV3>(),
				ExtraVillages = new List<SettlementMetaV3>(),
				OverwriteSettlements = new List<SettlementMetaV3>()
			};
			foreach (PlayerSettlementItem town in playerSettlementInfo.Towns)
			{
				metaV.Towns.Add(new SettlementMetaV3
				{
					XML = town.ItemXML,
					BuildTime = town.BuiltAt,
					DisplayName = town.SettlementName,
					Identifier = town.Identifier,
					settlement = town.Settlement,
					StringId = town.StringId,
					PrefabId = town.PrefabId,
					Version = town.Version,
					Villages = town.Villages.Select((PlayerSettlementItem v) => new SettlementMetaV3
					{
						XML = v.ItemXML,
						BuildTime = v.BuiltAt,
						DisplayName = v.SettlementName,
						Identifier = v.Identifier,
						settlement = v.Settlement,
						StringId = v.StringId,
						PrefabId = v.PrefabId,
						Version = v.Version,
						Villages = new List<SettlementMetaV3>()
					}).ToList()
				});
			}
			foreach (PlayerSettlementItem castle in playerSettlementInfo.Castles)
			{
				metaV.Castles.Add(new SettlementMetaV3
				{
					XML = castle.ItemXML,
					BuildTime = castle.BuiltAt,
					DisplayName = castle.SettlementName,
					Identifier = castle.Identifier,
					settlement = castle.Settlement,
					StringId = castle.StringId,
					PrefabId = castle.PrefabId,
					Version = castle.Version,
					Villages = castle.Villages.Select((PlayerSettlementItem v) => new SettlementMetaV3
					{
						XML = v.ItemXML,
						BuildTime = v.BuiltAt,
						DisplayName = v.SettlementName,
						Identifier = v.Identifier,
						settlement = v.Settlement,
						StringId = v.StringId,
						PrefabId = v.PrefabId,
						Version = v.Version,
						Villages = new List<SettlementMetaV3>()
					}).ToList()
				});
			}
			if (playerSettlementInfo.PlayerVillages != null)
			{
				foreach (PlayerSettlementItem playerVillage in playerSettlementInfo.PlayerVillages)
				{
					metaV.ExtraVillages.Add(new SettlementMetaV3
					{
						XML = playerVillage.ItemXML,
						BuildTime = playerVillage.BuiltAt,
						DisplayName = playerVillage.SettlementName,
						Identifier = playerVillage.Identifier,
						settlement = playerVillage.Settlement,
						StringId = playerVillage.StringId,
						PrefabId = playerVillage.PrefabId,
						Version = playerVillage.Version,
						Villages = new List<SettlementMetaV3>()
					});
				}
			}
			if (playerSettlementInfo.OverwriteSettlements != null)
			{
				foreach (OverwriteSettlementItem overwriteSettlement in playerSettlementInfo.OverwriteSettlements)
				{
					metaV.OverwriteSettlements.Add(new SettlementMetaV3
					{
						XML = overwriteSettlement.ItemXML,
						BuildTime = overwriteSettlement.BuiltAt,
						DisplayName = overwriteSettlement.SettlementName,
						settlement = overwriteSettlement.Settlement,
						StringId = overwriteSettlement.StringId,
						PrefabId = overwriteSettlement.PrefabId,
						Version = overwriteSettlement.Version
					});
				}
			}
			return metaV;
		}
	}
	public class OverwriteSettlementItem : ISettlementItem
	{
		[SaveableField(112)]
		public string? ItemXML = null;

		[SaveableField(113)]
		public Settlement? Settlement = null;

		[SaveableField(115)]
		public string SettlementName = null;

		[SaveableField(116)]
		public float BuiltAt = -1f;

		[SaveableField(117)]
		public bool BuildComplete = false;

		[SaveableField(204)]
		public int Type = 0;

		[SaveableField(206)]
		public Mat3Saveable? RotationMat3 = null;

		[SaveableField(207)]
		public string Version = null;

		[SaveableField(208)]
		public string StringId = null;

		[SaveableField(209)]
		public string PrefabId = null;

		[SaveableField(210)]
		public List<DeepTransformEdit>? DeepEdits = new List<DeepTransformEdit>();

		public CampaignTime BuildEnd
		{
			get
			{
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				if (Main.Settings == null || !Main.Settings.Enabled)
				{
					return CampaignTime.Never;
				}
				if (Main.Settings.InstantBuild)
				{
					return CampaignTime.Now;
				}
				CampaignTime val = CampaignTime.Hours(BuiltAt - 5f);
				int num = Main.Settings.RebuildTownDurationDays;
				if (Type == 3)
				{
					num = Main.Settings.RebuildCastleDurationDays;
				}
				else if (Type == 2)
				{
					num = Main.Settings.RebuildVillageDurationDays;
				}
				return val + CampaignTime.Days((float)num);
			}
		}

		public SettlementType GetSettlementType()
		{
			return (SettlementType)Type;
		}

		public static string EncyclopediaLink(string StringId)
		{
			return Campaign.Current.EncyclopediaManager.GetIdentifier(typeof(Settlement)) + "-" + StringId;
		}

		public static TextObject EncyclopediaLinkWithName(string StringId, TextObject Name)
		{
			return HyperlinkTexts.GetSettlementHyperlinkText(EncyclopediaLink(StringId), Name);
		}

		public void SetBuildComplete(bool completed)
		{
			BuildComplete = completed;
		}

		public string? GetSettlementName()
		{
			return SettlementName;
		}

		public Settlement? GetSettlement()
		{
			return Settlement;
		}
	}
	public class PlayerSettlementInfo
	{
		[Obsolete("Replaced with `Towns[0]`")]
		[SaveableField(102)]
		public string? PlayerSettlementXML = null;

		[Obsolete("Replaced with `Towns[0]`")]
		[SaveableField(103)]
		public Settlement? PlayerSettlement = null;

		private static PlayerSettlementInfo? _instance;

		[Obsolete("Replaced with `Towns[0]`")]
		[SaveableField(104)]
		public string PlayerSettlementIdentifier = null;

		[Obsolete("Replaced with `Towns[0]`")]
		[SaveableField(105)]
		public string PlayerSettlementName = "{=player_settlement_n_01}Player Settlement";

		[Obsolete("Replaced with `Towns[0]`")]
		[SaveableField(106)]
		public float BuiltAt = -1f;

		[Obsolete("Replaced with `Towns[0]`")]
		[SaveableField(107)]
		public bool BuildComplete = false;

		[SaveableField(111)]
		public List<PlayerSettlementItem>? PlayerVillages = new List<PlayerSettlementItem>();

		[SaveableField(211)]
		public List<PlayerSettlementItem> Towns = new List<PlayerSettlementItem>();

		[SaveableField(212)]
		public List<PlayerSettlementItem> Castles = new List<PlayerSettlementItem>();

		[SaveableField(213)]
		public List<OverwriteSettlementItem> OverwriteSettlements = new List<OverwriteSettlementItem>();

		public static PlayerSettlementInfo? Instance
		{
			get
			{
				return _instance;
			}
			set
			{
				_instance = value;
			}
		}

		public int TotalVillages => (Towns?.SelectMany((PlayerSettlementItem t) => t.Villages ?? new List<PlayerSettlementItem>()) ?? new List<PlayerSettlementItem>()).Concat(Castles?.SelectMany((PlayerSettlementItem c) => c.Villages ?? new List<PlayerSettlementItem>()) ?? new List<PlayerSettlementItem>()).Concat(PlayerVillages ?? new List<PlayerSettlementItem>()).Count();

		public void OnLoad()
		{
			try
			{
				if (Towns == null)
				{
					Towns = new List<PlayerSettlementItem>();
				}
				if (Castles == null)
				{
					Castles = new List<PlayerSettlementItem>();
				}
				if (PlayerVillages == null)
				{
					PlayerVillages = new List<PlayerSettlementItem>();
				}
				if (PlayerSettlement != null && PlayerSettlement.Town != null)
				{
					PlayerSettlementItem playerSettlementItem = new PlayerSettlementItem
					{
						BuildComplete = BuildComplete,
						BuiltAt = BuiltAt,
						Identifier = int.Parse(PlayerSettlementIdentifier.Replace("player_settlement_town_", "")),
						Type = 1,
						ItemIdentifier = PlayerSettlementIdentifier,
						ItemXML = PlayerSettlementXML,
						Settlement = PlayerSettlement,
						SettlementName = PlayerSettlementName,
						Villages = new List<PlayerSettlementItem>()
					};
					if (PlayerVillages != null)
					{
						for (int i = 0; i < PlayerVillages.Count; i++)
						{
							int num = i + 1;
							PlayerSettlementItem playerSettlementItem2 = PlayerVillages[i];
							PlayerSettlementItem item = new PlayerSettlementItem
							{
								BuildComplete = playerSettlementItem2.BuildComplete,
								BuiltAt = playerSettlementItem2.BuiltAt,
								Identifier = num,
								Type = 2,
								ItemIdentifier = $"{PlayerSettlementIdentifier}_village_{num}",
								ItemXML = playerSettlementItem2.ItemXML,
								Settlement = playerSettlementItem2.Settlement,
								SettlementName = playerSettlementItem2.SettlementName,
								Villages = new List<PlayerSettlementItem>()
							};
							playerSettlementItem.Villages.Add(item);
						}
					}
					Towns.Add(playerSettlementItem);
					PlayerSettlement = null;
					PlayerVillages = new List<PlayerSettlementItem>();
				}
				SandBoxManager val = SandBoxManager.Instance;
				if (val == null)
				{
					Campaign current = Campaign.Current;
					val = ((current != null) ? current.SandBoxManager : null);
				}
				if (val == null)
				{
					Game current2 = Game.Current;
					val = ((current2 != null) ? current2.GetGameHandler<SandBoxManager>() : null);
				}
				CampaignGameStarter gameStarter = val.GameStarter;
				CampaignBehaviorBase obj = gameStarter.CampaignBehaviors.FirstOrDefault((CampaignBehaviorBase b) => b is CraftingCampaignBehavior);
				CraftingCampaignBehavior val2 = (CraftingCampaignBehavior)(object)((obj is CraftingCampaignBehavior) ? obj : null);
				foreach (PlayerSettlementItem town in Towns)
				{
					if (val2 != null && val2.CraftingOrders != null && town != null && town.Settlement != null && town.Settlement.Town != null)
					{
						val2.AddTown(town.Settlement.Town, out Dictionary<Town, CraftingOrderSlots> _);
					}
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
		}

		public PlayerSettlementItem? FindSettlement(Settlement? settlement)
		{
			Settlement settlement2 = settlement;
			if (settlement2 == null)
			{
				return null;
			}
			if (settlement2.IsTown)
			{
				return Towns?.FirstOrDefault((PlayerSettlementItem t) => t.Settlement == settlement2);
			}
			if (settlement2.IsCastle)
			{
				return Castles?.FirstOrDefault((PlayerSettlementItem c) => c.Settlement == settlement2);
			}
			if (settlement2.IsVillage)
			{
				return PlayerVillages?.FirstOrDefault((PlayerSettlementItem v) => v.Settlement == settlement2) ?? Towns.SelectMany((PlayerSettlementItem t) => t.Villages)?.FirstOrDefault((PlayerSettlementItem v) => v.Settlement == settlement2) ?? Castles.SelectMany((PlayerSettlementItem c) => c.Villages)?.FirstOrDefault((PlayerSettlementItem v) => v.Settlement == settlement2);
			}
			return null;
		}

		public int GetVillageNumber(Settlement bound, out PlayerSettlementItem? target)
		{
			if (bound.IsTown)
			{
				target = Towns.FirstOrDefault((PlayerSettlementItem t) => t.Settlement == bound);
				if (target == null)
				{
					return -1;
				}
				return (target.Villages?.Count ?? 0) + 1;
			}
			if (bound.IsCastle)
			{
				target = Castles.FirstOrDefault((PlayerSettlementItem t) => t.Settlement == bound);
				if (target == null)
				{
					return -1;
				}
				return (target.Villages?.Count ?? 0) + 1;
			}
			target = null;
			return -1;
		}
	}
	public interface ISettlementItem
	{
		void SetBuildComplete(bool completed);

		string? GetSettlementName();

		Settlement? GetSettlement();
	}
	public class PlayerSettlementItem : ISettlementItem
	{
		[SaveableField(112)]
		public string? ItemXML = null;

		[SaveableField(113)]
		public Settlement? Settlement = null;

		[Obsolete("Replaced with `Identifier`")]
		[SaveableField(114)]
		public string ItemIdentifier = null;

		[SaveableField(115)]
		public string SettlementName = "{=player_settlement_n_01}Player Settlement";

		[SaveableField(116)]
		public float BuiltAt = -1f;

		[SaveableField(117)]
		public bool BuildComplete = false;

		[SaveableField(201)]
		public List<PlayerSettlementItem> Villages = new List<PlayerSettlementItem>();

		[Obsolete("Replaced with `StringId`")]
		[SaveableField(202)]
		public int Identifier = 1;

		[SaveableField(204)]
		public int Type = 0;

		[SaveableField(206)]
		public Mat3Saveable? RotationMat3 = null;

		[SaveableField(207)]
		public string Version = null;

		[SaveableField(208)]
		public string StringId = null;

		[SaveableField(209)]
		public string PrefabId = null;

		[SaveableField(210)]
		public List<DeepTransformEdit>? DeepEdits = new List<DeepTransformEdit>();

		[SaveableField(211)]
		public bool IsRebuild = false;

		public CampaignTime BuildEnd
		{
			get
			{
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				if (Main.Settings == null || !Main.Settings.Enabled)
				{
					return CampaignTime.Never;
				}
				if (Main.Settings.InstantBuild)
				{
					return CampaignTime.Now;
				}
				CampaignTime val = CampaignTime.Hours(BuiltAt - 5f);
				int num = (IsRebuild ? Main.Settings.RebuildTownDurationDays : Main.Settings.BuildDurationDays);
				if (Type == 3)
				{
					num = (IsRebuild ? Main.Settings.RebuildCastleDurationDays : Main.Settings.BuildCastleDurationDays);
				}
				else if (Type == 2)
				{
					num = (IsRebuild ? Main.Settings.RebuildVillageDurationDays : Main.Settings.BuildVillageDurationDays);
				}
				return val + CampaignTime.Days((float)num);
			}
		}

		public SettlementType GetSettlementType()
		{
			return (SettlementType)Type;
		}

		public static string EncyclopediaLink(string StringId)
		{
			return Campaign.Current.EncyclopediaManager.GetIdentifier(typeof(Settlement)) + "-" + StringId;
		}

		public static TextObject EncyclopediaLinkWithName(string StringId, TextObject Name)
		{
			return HyperlinkTexts.GetSettlementHyperlinkText(EncyclopediaLink(StringId), Name);
		}

		public void SetBuildComplete(bool completed)
		{
			BuildComplete = completed;
		}

		public string? GetSettlementName()
		{
			return SettlementName;
		}

		public Settlement? GetSettlement()
		{
			return Settlement;
		}
	}
	[JsonObject(/*Could not decode attribute arguments.*/)]
	public class SettlementMetaV3
	{
		public Settlement? settlement;

		private XmlDocument? _doc = null;

		[SaveableField(336)]
		public string Version = null;

		[SaveableField(337)]
		public string StringId = null;

		[SaveableField(338)]
		public string PrefabId = null;

		public XmlDocument Document
		{
			get
			{
				if (_doc == null)
				{
					_doc = new XmlDocument();
					_doc.LoadXml(XML);
				}
				return _doc;
			}
		}

		[JsonProperty]
		[SaveableProperty(331)]
		public string XML { get; set; }

		[JsonProperty]
		[SaveableProperty(332)]
		public string DisplayName { get; set; }

		[Obsolete("Replaced with `StringId`")]
		[JsonProperty]
		[SaveableProperty(333)]
		public int Identifier { get; set; }

		[JsonProperty]
		[SaveableProperty(334)]
		public float BuildTime { get; set; }

		[JsonProperty]
		[SaveableProperty(335)]
		public List<SettlementMetaV3> Villages { get; set; } = new List<SettlementMetaV3>();

	}
	public enum SettlementType
	{
		None,
		Town,
		Village,
		Castle
	}
	public class TransformSaveable
	{
		[SaveableField(502)]
		public Mat3Saveable? RotationScale;

		[SaveableField(503)]
		public Vec3Saveable? Position;

		[SaveableField(504)]
		public Vec3Saveable? Offsets;
	}
	public class Vec3Saveable
	{
		[SaveableField(401)]
		public float x;

		[SaveableField(402)]
		public float y;

		[SaveableField(403)]
		public float z;

		[SaveableField(404)]
		public float w;

		public Vec3Saveable()
		{
		}

		public Vec3Saveable(Vec3 source)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			x = source.x;
			y = source.y;
			z = source.z;
			w = source.w;
		}

		public Vec3 ToVec3()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			return new Vec3(x, y, z, w);
		}

		public static implicit operator Vec3(Vec3Saveable? source)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			return source?.ToVec3() ?? Vec3.Zero;
		}

		public static implicit operator Vec3Saveable?(Vec3? source)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			if (!source.HasValue)
			{
				return null;
			}
			return new Vec3Saveable(source.Value);
		}
	}
}
namespace BannerlordPlayerSettlement.Patches
{
	[HarmonyPatch(typeof(Building))]
	public static class BuildingPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch("CurrentLevel", MethodType.Getter)]
		public static void GetCurrentLevel(ref Building __instance, ref int __result)
		{
			try
			{
				if (__result < 1)
				{
					__result = 1;
					__instance.CurrentLevel = __result;
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
		}
	}
	[HarmonyPatch(typeof(BuildingsCampaignBehavior))]
	public static class BuildingsCampaignBehaviorPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch("DailyTickSettlement")]
		public static bool DailyTickSettlement(ref BuildingsCampaignBehavior __instance, Settlement settlement)
		{
			if (settlement.IsFortification)
			{
				Town town = settlement.Town;
				if (((SettlementComponent)town).Owner.Settlement.OwnerClan != Clan.PlayerClan)
				{
					return true;
				}
				if (!town.CurrentBuilding.BuildingType.IsDailyProject && (settlement.IsPlayerBuilt() || settlement.IsOverwritten(out OverwriteSettlementItem _)))
				{
					try
					{
						TickCurrentBuildingForTown(town);
					}
					catch (Exception e)
					{
						LogManager.Log.NotifyBad(e);
					}
					return false;
				}
			}
			return true;
		}

		[HarmonyPrefix]
		[HarmonyPatch("TickCurrentBuildingForTown")]
		public static bool PreTickCurrentBuildingForTown(ref BuildingsCampaignBehavior __instance, Town town)
		{
			if (town.BuildingsInProgress.Count == 0)
			{
				Building val = ((IEnumerable<Building>)town.Buildings).FirstOrDefault((Building b) => b.BuildingType.IsDailyProject && b.IsCurrentlyDefault);
				if (val == null)
				{
					val = ((IEnumerable<Building>)town.Buildings).FirstOrDefault((Building b) => b.BuildingType.IsDailyProject);
				}
				if (val != null)
				{
					BuildingHelper.ChangeDefaultBuilding(val, town);
					val.IsCurrentlyDefault = true;
				}
				return false;
			}
			return true;
		}

		[HarmonyFinalizer]
		[HarmonyPatch("TickCurrentBuildingForTown")]
		public static Exception? FixTickCurrentBuildingForTown(Exception? __exception, ref BuildingsCampaignBehavior __instance)
		{
			if (__exception != null)
			{
				LogManager.Log.NotifyBad(__exception);
			}
			return null;
		}

		private static void TickCurrentBuildingForTown(Town town)
		{
			if (town.BuildingsInProgress.Count == 0)
			{
				Building val = ((IEnumerable<Building>)town.Buildings).FirstOrDefault((Building b) => b.BuildingType.IsDailyProject && b.IsCurrentlyDefault);
				if (val == null)
				{
					val = ((IEnumerable<Building>)town.Buildings).FirstOrDefault((Building b) => b.BuildingType.IsDailyProject);
				}
				if (val != null)
				{
					BuildingHelper.ChangeDefaultBuilding(val, town);
					val.IsCurrentlyDefault = true;
				}
				return;
			}
			if (town.BuildingsInProgress.Peek().CurrentLevel == 3)
			{
				town.BuildingsInProgress.Dequeue();
			}
			if (((SettlementComponent)town).Owner.Settlement.IsUnderSiege || Extensions.IsEmpty<Building>((IEnumerable<Building>)town.BuildingsInProgress))
			{
				return;
			}
			BuildingConstructionModel buildingConstructionModel = Campaign.Current.Models.BuildingConstructionModel;
			Building val2 = town.BuildingsInProgress.Peek();
			val2.BuildingProgress += town.Construction;
			int num = (((SettlementComponent)town).IsCastle ? buildingConstructionModel.CastleBoostCost : buildingConstructionModel.TownBoostCost);
			if (town.BoostBuildingProcess > 0)
			{
				town.BoostBuildingProcess -= num;
				if (town.BoostBuildingProcess < 0)
				{
					town.BoostBuildingProcess = 0;
				}
			}
			BuildingHelper.CheckIfBuildingIsComplete(val2);
		}
	}
	[HarmonyPatch(typeof(Incident))]
	public static class IncidentPatch
	{
		[HarmonyFinalizer]
		[HarmonyPatch("CanIncidentBeInvoked")]
		public static Exception FixCanIncidentBeInvoked(object __exception, ref Incident __instance, ref bool __result)
		{
			if (__exception != null)
			{
				if (__exception != null)
				{
					if (__exception is Exception e)
					{
						LogManager.Log.SilentException(e);
					}
					else
					{
						LogManager.Log.ToFile(__exception.ToString());
					}
					__result = false;
				}
			}
			return null;
		}
	}
	[HarmonyPatch(typeof(MapBarVM))]
	public static class MapBarVMPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch("Tick")]
		public static void Tick(ref MapBarVM __instance, float dt)
		{
			try
			{
				MapBarExtensionVM.Current?.Tick(dt);
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
		}
	}
	[HarmonyPatch(typeof(MapCameraView))]
	public static class MapCameraViewPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch("OnBeforeTick")]
		public static bool OnBeforeTick(ref MapCameraView __instance, ref InputInformation inputInformation)
		{
			PlayerSettlementBehaviour.Instance?.OnBeforeTick(ref inputInformation);
			return true;
		}

		[HarmonyPrefix]
		[HarmonyPatch("GetMapCameraInput")]
		public static bool GetMapCameraInput(ref bool __result, ref MapCameraView __instance, InputInformation inputInformation)
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			if (PlayerSettlementBehaviour.Instance != null && !PlayerSettlementBehaviour.Instance.IsPlacingGate && !PlayerSettlementBehaviour.Instance.IsPlacingPort && PlayerSettlementBehaviour.Instance.IsPlacingSettlement)
			{
				GameState activeState = Game.Current.GameStateManager.ActiveState;
				MapState val = (MapState)(object)((activeState is MapState) ? activeState : null);
				if (val != null)
				{
					IMapStateHandler handler = val.Handler;
					MapScreen val2 = (MapScreen)(object)((handler is MapScreen) ? handler : null);
					if (val2 != null && (((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.RotateModifierKey.GetInputKey()) || ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.CycleModifierKey.GetInputKey()) || ((ScreenLayer)val2.SceneLayer).Input.IsKeyDown(Main.Submodule.ScaleModifierKey.GetInputKey())))
					{
						__result = true;
						return false;
					}
				}
			}
			return true;
		}
	}
	[HarmonyPatch(typeof(MapScene))]
	public static class MapScenePatch
	{
		[HarmonyPrefix]
		[HarmonyPatch("AddNewEntityToMapScene")]
		public static bool AddNewEntityToMapScene(ref MapScene __instance, ref Scene ____scene, ref string entityId, ref Vec2 position)
		{
			try
			{
				PlayerSettlementItem playerSettlementItem = null;
				OverwriteSettlementItem overwriteSettlementItem = null;
				string obj = entityId;
				if (obj != null && obj.IsPlayerBuiltStringId(out playerSettlementItem))
				{
					if (entityId != null && entityId.StartsWith("player_settlement_town_"))
					{
						try
						{
							string s = entityId.Replace("player_settlement_town_", "").Split('_')[0];
							if (int.TryParse(s, out var result) && result > 35)
							{
								entityId = (entityId.Contains("village") ? $"player_settlement_town_1_village_{int.Parse(entityId.Split('_').Last())}" : "player_settlement_town_1");
							}
						}
						catch (Exception)
						{
						}
					}
					string prefabId = playerSettlementItem?.PrefabId ?? entityId;
					GameEntity val = ((IMapScene)(object)__instance).AddPrefabEntityToMapScene(ref ____scene, ref entityId, ref position, ref prefabId);
					if (val != (GameEntity)null)
					{
						return false;
					}
				}
				else
				{
					string obj2 = entityId;
					if (obj2 != null && obj2.IsOverwritten(out overwriteSettlementItem))
					{
						GameEntity campaignEntityWithName = ____scene.GetCampaignEntityWithName(entityId);
						if (campaignEntityWithName != (GameEntity)null)
						{
							campaignEntityWithName.ClearEntity();
						}
						string prefabId2 = overwriteSettlementItem?.PrefabId ?? entityId;
						GameEntity val2 = ((IMapScene)(object)__instance).AddPrefabEntityToMapScene(ref ____scene, ref entityId, ref position, ref prefabId2);
						if (val2 != (GameEntity)null)
						{
							return false;
						}
					}
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return true;
		}

		public static GameEntity? AddPrefabEntityToMapScene(this IMapScene __instance, ref Scene ____scene, ref string entityId, ref Vec2 position, ref string prefabId, Action<Exception> onError = null)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				GameEntity val = GameEntity.Instantiate(____scene, prefabId, true, true, "");
				if (val != (GameEntity)null)
				{
					if (entityId != prefabId)
					{
						val.Name = entityId;
					}
					Vec3 val2 = new Vec3(position.x, position.y, 0f, -1f);
					val2.z = ____scene.GetGroundHeightAtPosition(((Vec2)(ref position)).ToVec3(0f), (BodyFlags)544321929);
					Vec3 localPosition = val2;
					val.SetLocalPosition(localPosition);
					return val;
				}
			}
			catch (Exception ex)
			{
				LogManager.Log.NotifyBad(ex);
				onError?.Invoke(ex);
			}
			return null;
		}
	}
	[HarmonyPatch(typeof(MapScreen))]
	public static class MapScreenPatch
	{
		private static MethodInfo GetFrameAndVisualOfEngines;

		private static MethodInfo GetDesiredDecalColorMethod;

		private static MethodInfo GetDesiredMaterialNameMethod;

		private static MethodInfo IsEscapedMethod;

		public static Dictionary<UIntPtr, Tuple<MatrixFrame, SettlementVisual>> FrameAndVisualOfEngines()
		{
			return (Dictionary<UIntPtr, Tuple<MatrixFrame, SettlementVisual>>)GetFrameAndVisualOfEngines.Invoke(null, null);
		}

		public static uint GetDesiredDecalColor(this MapScreen mapScreen, bool isPrepOver, bool isHovered, bool isEnemy, bool isEmpty, bool isPlayerLeader)
		{
			return (uint)GetDesiredDecalColorMethod.Invoke(mapScreen, new object[5] { isPrepOver, isHovered, isEnemy, isEmpty, isPlayerLeader });
		}

		public static string GetDesiredMaterialName(this MapScreen mapScreen, bool isRanged, bool isAttacker, bool isEmpty, bool isTower)
		{
			return (string)GetDesiredMaterialNameMethod.Invoke(mapScreen, new object[4] { isRanged, isAttacker, isEmpty, isTower });
		}

		private static bool CheckIsEscaped(this MapView mapView)
		{
			try
			{
				return (bool)IsEscapedMethod.Invoke(mapView, new object[0]);
			}
			catch (Exception)
			{
				return false;
			}
		}

		static MapScreenPatch()
		{
			GetFrameAndVisualOfEngines = AccessTools.Property(typeof(MapScreen), "FrameAndVisualOfEngines").GetMethod;
			GetDesiredDecalColorMethod = AccessTools.Method(typeof(MapScreen), "GetDesiredDecalColor");
			GetDesiredMaterialNameMethod = AccessTools.Method(typeof(MapScreen), "GetDesiredMaterialName");
			IsEscapedMethod = AccessTools.Method(typeof(MapView), "IsEscaped") ?? AccessTools.DeclaredMethod(typeof(MapView), "IsEscaped");
			Main.Harmony?.Patch(AccessTools.DeclaredMethod(typeof(MapScreen), "TaleWorlds.CampaignSystem.GameState.IMapStateHandler.AfterWaitTick"), new HarmonyMethod(typeof(MapScreenPatch), "AfterWaitTick"));
		}

		public static bool AfterWaitTick(ref MapScreen __instance, ref MapViewsContainer ____mapViewsContainer, float dt)
		{
			if (((ScreenLayer)__instance.SceneLayer).Input.IsHotKeyReleased("ToggleEscapeMenu") && PlayerSettlementBehaviour.Instance != null && (PlayerSettlementBehaviour.Instance.IsPlacingSettlement || PlayerSettlementBehaviour.Instance.IsPlacingPort || PlayerSettlementBehaviour.Instance.IsPlacingGate) && !____mapViewsContainer.IsThereAnyViewIsEscaped())
			{
				if (PlayerSettlementBehaviour.Instance.IsPlacingGate || PlayerSettlementBehaviour.Instance.IsPlacingPort)
				{
					PlayerSettlementBehaviour.Instance.RefreshVisualSelection();
					return false;
				}
				PlayerSettlementBehaviour.Instance.Reset();
				((BaseViewModelMixin<MapBarVM>)MapBarExtensionVM.Current)?.OnRefresh();
				return false;
			}
			return true;
		}

		[HarmonyPrefix]
		[HarmonyPatch("HandleLeftMouseButtonClick")]
		public static bool HandleLeftMouseButtonClick(ref MapScreen __instance, MapEntityVisual visualOfSelectedEntity, CampaignVec2 intersectionPoint, PathFaceRecord mouseOverFaceIndex, bool isDoubleClick)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			if (((ScreenLayer)__instance.SceneLayer).Input.GetIsMouseActive() && PlayerSettlementBehaviour.Instance != null && PlayerSettlementBehaviour.Instance.IsPlacingPort && !intersectionPoint.IsOnLand && ((PathFaceRecord)(ref mouseOverFaceIndex)).IsValid() && PlayerSettlementBehaviour.Instance.PlacementSupported)
			{
				PlayerSettlementBehaviour.Instance.ApplyNow();
				return false;
			}
			if (((ScreenLayer)__instance.SceneLayer).Input.GetIsMouseActive() && PlayerSettlementBehaviour.Instance != null && PlayerSettlementBehaviour.Instance.IsPlacingGate && intersectionPoint.IsOnLand && PlayerSettlementBehaviour.Instance.PlacementSupported)
			{
				PlayerSettlementBehaviour.Instance.StartPortPlacement();
				return false;
			}
			if (((ScreenLayer)__instance.SceneLayer).Input.GetIsMouseActive() && PlayerSettlementBehaviour.Instance != null && PlayerSettlementBehaviour.Instance.IsPlacingSettlement && intersectionPoint.IsOnLand && PlayerSettlementBehaviour.Instance.PlacementSupported)
			{
				if (PlayerSettlementBehaviour.Instance.IsDeepEdit)
				{
					return false;
				}
				PlayerSettlementBehaviour.Instance.StartGatePlacement();
				return false;
			}
			return true;
		}

		[HarmonyPostfix]
		[HarmonyPatch("CheckCursorState")]
		public static void CheckCursorState(ref MapScreen __instance)
		{
			if (((ScreenLayer)__instance.SceneLayer).Input.GetIsMouseActive() && PlayerSettlementBehaviour.Instance != null && (PlayerSettlementBehaviour.Instance.IsPlacingPort || PlayerSettlementBehaviour.Instance.IsPlacingGate || PlayerSettlementBehaviour.Instance.IsPlacingSettlement))
			{
				((ScreenLayer)__instance.SceneLayer).ActiveCursor = (CursorType)(PlayerSettlementBehaviour.Instance.PlacementSupported ? 1 : 10);
			}
		}
	}
	[HarmonyPatch(typeof(MapSiegePOIVM))]
	public static class MapSiegePOIVMPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch("GetDesiredMachine")]
		public static bool GetDesiredMachine(ref MapSiegePOIVM __instance, ref SiegeEngineConstructionProgress __result)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Expected I4, but got Unknown
			try
			{
				if (PlayerSiege.PlayerSiegeEvent == null)
				{
					__result = null;
					return true;
				}
				if (!PlayerSiege.PlayerSiegeEvent.BesiegedSettlement.IsPlayerBuilt() && !PlayerSiege.PlayerSiegeEvent.BesiegedSettlement.IsOverwritten(out OverwriteSettlementItem _))
				{
					return true;
				}
				POIType type = __instance.Type;
				POIType val = type;
				switch ((int)val)
				{
				case 0:
					__result = null;
					return true;
				case 1:
					if (__instance.MachineIndex >= PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide((BattleSideEnum)0).SiegeEngines.DeployedRangedSiegeEngines.Length)
					{
						__result = null;
						return false;
					}
					__result = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide((BattleSideEnum)0).SiegeEngines.DeployedRangedSiegeEngines[__instance.MachineIndex];
					return true;
				case 2:
				case 3:
					if (__instance.MachineIndex >= PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide((BattleSideEnum)1).SiegeEngines.DeployedMeleeSiegeEngines.Length)
					{
						__result = null;
						return false;
					}
					__result = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide((BattleSideEnum)1).SiegeEngines.DeployedMeleeSiegeEngines[__instance.MachineIndex];
					return true;
				case 4:
					if (__instance.MachineIndex >= PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide((BattleSideEnum)1).SiegeEngines.DeployedRangedSiegeEngines.Length)
					{
						__result = null;
						return false;
					}
					__result = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide((BattleSideEnum)1).SiegeEngines.DeployedRangedSiegeEngines[__instance.MachineIndex];
					return true;
				default:
					__result = null;
					return true;
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return true;
		}

		[HarmonyFinalizer]
		[HarmonyPatch("GetDesiredMachine")]
		public static Exception? FixGetDesiredMachine(Exception? __exception, ref MapSiegePOIVM __instance)
		{
			if (__exception != null)
			{
				LogManager.Log.NotifyBad(__exception);
			}
			return null;
		}

		[HarmonyPrefix]
		[HarmonyPatch("RefreshHitpoints")]
		public static bool RefreshHitpoints(ref MapSiegePOIVM __instance, ref float ____bindCurrentHitpoints, ref float ____bindMaxHitpoints, ref int ____bindMachineType)
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Invalid comparison between Unknown and I4
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Invalid comparison between Unknown and I4
			try
			{
				if (PlayerSiege.PlayerSiegeEvent == null)
				{
					____bindCurrentHitpoints = 0f;
					____bindMaxHitpoints = 0f;
					return true;
				}
				if (!PlayerSiege.PlayerSiegeEvent.BesiegedSettlement.IsPlayerBuilt() && !PlayerSiege.PlayerSiegeEvent.BesiegedSettlement.IsOverwritten(out OverwriteSettlementItem _))
				{
					return true;
				}
				POIType type = __instance.Type;
				if ((int)type > 0)
				{
					if (type - 1 > 3)
					{
						return true;
					}
					if (__instance.Machine == null)
					{
						____bindCurrentHitpoints = 0f;
						____bindMaxHitpoints = 0f;
						return true;
					}
					if (__instance.Machine.IsActive)
					{
						____bindCurrentHitpoints = __instance.Machine.Hitpoints;
						____bindMaxHitpoints = __instance.Machine.MaxHitPoints;
						return true;
					}
					if (__instance.Machine.IsBeingRedeployed)
					{
						____bindCurrentHitpoints = __instance.Machine.RedeploymentProgress;
						____bindMaxHitpoints = 1f;
						return true;
					}
					____bindCurrentHitpoints = __instance.Machine.Progress;
					____bindMaxHitpoints = 1f;
					return true;
				}
				MBReadOnlyList<float> settlementWallSectionHitPointsRatioList = PlayerSiege.PlayerSiegeEvent.BesiegedSettlement.SettlementWallSectionHitPointsRatioList;
				____bindMaxHitpoints = PlayerSiege.PlayerSiegeEvent.BesiegedSettlement.MaxWallHitPoints / (float)PlayerSiege.PlayerSiegeEvent.BesiegedSettlement.WallSectionCount;
				if (__instance.MachineIndex >= ((List<float>)(object)settlementWallSectionHitPointsRatioList).Count)
				{
					____bindCurrentHitpoints = 0f;
					____bindMachineType = 0;
					return false;
				}
				____bindCurrentHitpoints = ((List<float>)(object)settlementWallSectionHitPointsRatioList)[__instance.MachineIndex] * ____bindMaxHitpoints;
				____bindMachineType = ((____bindCurrentHitpoints <= 0f) ? 1 : 0);
				return false;
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return true;
		}

		[HarmonyFinalizer]
		[HarmonyPatch("RefreshHitpoints")]
		public static Exception? FixRefreshHitpoints(Exception? __exception, ref MapSiegePOIVM __instance)
		{
			if (__exception != null)
			{
				LogManager.Log.NotifyBad(__exception);
			}
			return null;
		}
	}
	[HarmonyPatch(typeof(MissionAgentHandler))]
	public static class MissionAgentHandlerPatch
	{
		private static FastInvokeHandler DisableUnavailableWaypointsMethod = MethodInvoker.GetHandler(AccessTools.Method(typeof(MissionAgentHandler), "DisableUnavailableWaypoints"));

		private static FastInvokeHandler RemoveDeactivatedUsablePlacesFromListMethod = MethodInvoker.GetHandler(AccessTools.Method(typeof(MissionAgentHandler), "RemoveDeactivatedUsablePlacesFromList"));

		[HarmonyPrefix]
		[HarmonyPatch("GetAllProps")]
		public static bool GetAllProps(ref MissionAgentHandler __instance, ref int ____disabledFaceId, ref int ____disabledFaceIdForAnimals, ref Dictionary<string, List<UsableMachine>> ____usablePoints)
		{
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
				if (!settlement.IsPlayerBuilt())
				{
					return true;
				}
				GameEntity val = ((MissionBehavior)__instance).Mission.Scene.FindEntityWithTag("navigation_mesh_deactivator");
				if (val != (GameEntity)null)
				{
					NavigationMeshDeactivator firstScriptOfType = val.GetFirstScriptOfType<NavigationMeshDeactivator>();
					____disabledFaceId = firstScriptOfType.DisableFaceWithId;
					____disabledFaceIdForAnimals = firstScriptOfType.DisableFaceWithIdForAnimals;
				}
				____usablePoints.Clear();
				WeakGameEntity gameEntity;
				foreach (UsableMachine item in MBExtensions.FindAllWithType<UsableMachine>((IEnumerable<MissionObject>)((MissionBehavior)__instance).Mission.MissionObjects))
				{
					gameEntity = ((ScriptComponentBehavior)item).GameEntity;
					string[] tags = ((WeakGameEntity)(ref gameEntity)).Tags;
					foreach (string key in tags)
					{
						if (!____usablePoints.ContainsKey(key))
						{
							____usablePoints.Add(key, new List<UsableMachine>());
						}
						____usablePoints[key].Add(item);
					}
				}
				if (Settlement.CurrentSettlement.IsTown || Settlement.CurrentSettlement.IsVillage)
				{
					foreach (AreaMarker item2 in MBExtensions.FindAllWithType<AreaMarker>((IEnumerable<MissionObject>)((MissionBehavior)__instance).Mission.ActiveMissionObjects).ToList())
					{
						string tag = item2.Tag;
						if (tag == null)
						{
							continue;
						}
						AreaMarker val2 = item2;
						string text = ((!item2.Tag.Contains("workshop")) ? null : "unaffected_by_area");
						List<UsableMachine> usableMachinesInRange = val2.GetUsableMachinesInRange(text);
						if (!____usablePoints.ContainsKey(tag))
						{
							____usablePoints.Add(tag, new List<UsableMachine>());
						}
						foreach (UsableMachine item3 in usableMachinesInRange)
						{
							foreach (KeyValuePair<string, List<UsableMachine>> ____usablePoint in ____usablePoints)
							{
								if (____usablePoint.Value.Contains(item3))
								{
									____usablePoint.Value.Remove(item3);
								}
							}
							gameEntity = ((ScriptComponentBehavior)item3).GameEntity;
							if (!((WeakGameEntity)(ref gameEntity)).HasTag("hold_tag_always"))
							{
								foreach (UsableMachine item4 in usableMachinesInRange)
								{
									gameEntity = ((ScriptComponentBehavior)item4).GameEntity;
									if (!((WeakGameEntity)(ref gameEntity)).HasTag(tag))
									{
										gameEntity = ((ScriptComponentBehavior)item4).GameEntity;
										((WeakGameEntity)(ref gameEntity)).AddTag(tag);
									}
								}
							}
							else
							{
								gameEntity = ((ScriptComponentBehavior)item3).GameEntity;
								string text2 = ((WeakGameEntity)(ref gameEntity)).Tags[0] + "_" + item2.Tag;
								gameEntity = ((ScriptComponentBehavior)item3).GameEntity;
								((WeakGameEntity)(ref gameEntity)).AddTag(text2);
								if (____usablePoints.ContainsKey(text2))
								{
									____usablePoints[text2].Add(item3);
									continue;
								}
								____usablePoints.Add(text2, new List<UsableMachine>());
								____usablePoints[text2].Add(item3);
							}
						}
						if (____usablePoints.ContainsKey(tag))
						{
							Dictionary<string, List<UsableMachine>> usp = ____usablePoints;
							usableMachinesInRange.RemoveAll((UsableMachine x) => usp[tag].Contains(x));
							if (usableMachinesInRange.Count > 0)
							{
								____usablePoints[tag].AddRange(usableMachinesInRange);
							}
						}
						foreach (UsableMachine item5 in item2.GetUsableMachinesWithTagInRange("unaffected_by_area"))
						{
							gameEntity = ((ScriptComponentBehavior)item5).GameEntity;
							string key2 = ((WeakGameEntity)(ref gameEntity)).Tags[0];
							foreach (KeyValuePair<string, List<UsableMachine>> ____usablePoint2 in ____usablePoints)
							{
								if (____usablePoint2.Value.Contains(item5))
								{
									____usablePoint2.Value.Remove(item5);
								}
							}
							if (!____usablePoints.ContainsKey(key2))
							{
								____usablePoints.Add(key2, new List<UsableMachine>());
								____usablePoints[key2].Add(item5);
							}
							else
							{
								____usablePoints[key2].Add(item5);
							}
						}
					}
				}
				DisableUnavailableWaypointsMethod(__instance);
				RemoveDeactivatedUsablePlacesFromListMethod(__instance);
				return false;
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return true;
		}

		[HarmonyFinalizer]
		[HarmonyPatch("GetAllProps")]
		public static Exception? FixGetAllProps(Exception? __exception, ref MissionAgentHandler __instance)
		{
			if (__exception != null)
			{
				Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
				if (!settlement.IsPlayerBuilt())
				{
					return __exception;
				}
				LogManager.Log.NotifyBad(__exception);
			}
			return null;
		}
	}
	[HarmonyPatch(typeof(NotableSpawnPointHandler))]
	public static class NotableSpawnPointHandlerPatch
	{
		private static readonly FastInvokeHandler FindAndSetChildInvoker = MethodInvoker.GetHandler(AccessTools.Method(typeof(NotableSpawnPointHandler), "FindAndSetChild"));

		private static readonly FastInvokeHandler ActivateParentSetInsideWorkshopInvoker = MethodInvoker.GetHandler(AccessTools.Method(typeof(NotableSpawnPointHandler), "ActivateParentSetInsideWorkshop"));

		private static readonly FastInvokeHandler ActivateParentSetOutsideWorkshopInvoker = MethodInvoker.GetHandler(AccessTools.Method(typeof(NotableSpawnPointHandler), "ActivateParentSetOutsideWorkshop"));

		public static void FindAndSetChild(this NotableSpawnPointHandler notableSpawnPointHandler, GameEntity childGameEntity)
		{
			FindAndSetChildInvoker(notableSpawnPointHandler, childGameEntity);
		}

		public static void ActivateParentSetInsideWorkshop(this NotableSpawnPointHandler notableSpawnPointHandler, WorkshopAreaMarker areaMarker)
		{
			ActivateParentSetInsideWorkshopInvoker(notableSpawnPointHandler, areaMarker);
		}

		public static void ActivateParentSetOutsideWorkshop(this NotableSpawnPointHandler notableSpawnPointHandler)
		{
			ActivateParentSetOutsideWorkshopInvoker(notableSpawnPointHandler);
		}

		[HarmonyPrefix]
		[HarmonyPatch("OnBehaviorInitialize")]
		public static bool OnBehaviorInitialize(ref NotableSpawnPointHandler __instance, ref List<Hero> ____workshopAssignedHeroes, ref int ____merchantNotableCount, ref int ____gangLeaderNotableCount, ref int ____preacherNotableCount, ref int ____artisanNotableCount, ref int ____ruralNotableCount)
		{
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
				if (!settlement.IsPlayerBuilt())
				{
					return true;
				}
				List<GameEntity> list = Mission.Current.Scene.FindEntitiesWithTag("sp_notables_parent").ToList();
				____workshopAssignedHeroes = new List<Hero>();
				foreach (Hero item in (List<Hero>)(object)settlement.Notables)
				{
					if (item.IsGangLeader)
					{
						____gangLeaderNotableCount++;
					}
					else if (item.IsPreacher)
					{
						____preacherNotableCount++;
					}
					else if (item.IsArtisan)
					{
						____artisanNotableCount++;
					}
					else if (item.IsRuralNotable || item.IsHeadman)
					{
						____ruralNotableCount++;
					}
					else if (item.IsMerchant)
					{
						____merchantNotableCount++;
					}
				}
				foreach (GameEntity item2 in list.ToList())
				{
					foreach (GameEntity child in item2.GetChildren())
					{
						__instance.FindAndSetChild(child);
					}
					foreach (WorkshopAreaMarker item3 in (from x in MBExtensions.FindAllWithType<WorkshopAreaMarker>((IEnumerable<MissionObject>)((MissionBehavior)__instance).Mission.ActiveMissionObjects).ToList()
						orderby ((AreaMarker)x).AreaIndex
						select x).ToList())
					{
						if (((AreaMarker)item3).IsPositionInRange(item2.GlobalPosition) && item3.GetWorkshop() != null && ((SettlementArea)((IEnumerable<Workshop>)((SettlementArea)item3.GetWorkshop()).Owner.OwnedWorkshops).First((Workshop x) => !x.WorkshopType.IsHidden)).Tag == ((AreaMarker)item3).Tag)
						{
							__instance.ActivateParentSetInsideWorkshop(item3);
							list.Remove(item2);
							break;
						}
					}
				}
				foreach (GameEntity item4 in list)
				{
					foreach (GameEntity child2 in item4.GetChildren())
					{
						__instance.FindAndSetChild(child2);
					}
					__instance.ActivateParentSetOutsideWorkshop();
				}
				return false;
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return true;
		}

		[HarmonyFinalizer]
		[HarmonyPatch("OnBehaviorInitialize")]
		public static Exception? FixOnBehaviorInitialize(Exception? __exception, ref NotableSpawnPointHandler __instance)
		{
			if (__exception != null)
			{
				Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
				if (!settlement.IsPlayerBuilt())
				{
					return __exception;
				}
				LogManager.Log.NotifyBad(__exception);
			}
			return null;
		}
	}
	[HarmonyPatch(typeof(SettlementVisual))]
	public static class PartyVisualPatch
	{
		private static MethodInfo SetStrategicEntity = AccessTools.Property(typeof(SettlementVisual), "StrategicEntity").SetMethod;

		private static MethodInfo SetTownPhysicalEntities = AccessTools.Property(typeof(SettlementVisual), "TownPhysicalEntities").SetMethod;

		private static MethodInfo SetCircleLocalFrame = AccessTools.Property(typeof(SettlementVisual), "CircleLocalFrame").SetMethod;

		private static MethodInfo GetMapScene = AccessTools.Property(typeof(SettlementVisual), "MapScene").GetMethod;

		private static MethodInfo PopulateSiegeEngineFrameListsFromChildren = AccessTools.Method(typeof(SettlementVisual), "PopulateSiegeEngineFrameListsFromChildren");

		private static MethodInfo UpdateDefenderSiegeEntitiesCache = AccessTools.Method(typeof(SettlementVisual), "UpdateDefenderSiegeEntitiesCache");

		private static FastInvokeHandler AddNewPartyVisualForPartyInvoker = MethodInvoker.GetHandler(AccessTools.Method(typeof(SettlementVisualManager), "AddNewPartyVisualForParty"));

		public static void AddNewPartyVisualForParty(this SettlementVisualManager partyVisualManager, PartyBase partyBase)
		{
			AddNewPartyVisualForPartyInvoker(partyVisualManager, partyBase);
		}

		public static Scene MapScene(this SettlementVisual __instance)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			return (Scene)GetMapScene.Invoke(__instance, null);
		}

		[HarmonyPrefix]
		[HarmonyPatch("OnMapHoverSiegeEngine")]
		public static bool OnMapHoverSiegeEngine(ref SettlementVisual __instance, ref GameEntity[] ____attackerBatteringRamSpawnEntities, ref GameEntity[] ____defenderBreachableWallEntitiesCacheForCurrentLevel, ref GameEntity[] ____defenderRangedEngineSpawnEntitiesCacheForCurrentLevel, ref GameEntity[] ____attackerRangedEngineSpawnEntities, ref GameEntity[] ____attackerSiegeTowerSpawnEntities, ref MatrixFrame ____hoveredSiegeEntityFrame, MatrixFrame engineFrame)
		{
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				bool flag = ((MapEntityVisual<PartyBase>)(object)__instance).MapEntity != null && ((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement.IsPlayerBuilt();
				if ((PlayerSiege.PlayerSiegeEvent == null || (!PlayerSiege.PlayerSiegeEvent.BesiegedSettlement.IsPlayerBuilt() && !PlayerSiege.PlayerSiegeEvent.BesiegedSettlement.IsOverwritten(out OverwriteSettlementItem _))) && !flag)
				{
					return true;
				}
				if (PlayerSiege.PlayerSiegeEvent == null)
				{
					return true;
				}
				try
				{
					for (int i = 0; i < ____attackerBatteringRamSpawnEntities.Length; i++)
					{
						MatrixFrame globalFrame = ____attackerBatteringRamSpawnEntities[i].GetGlobalFrame();
						if (((MatrixFrame)(ref globalFrame)).NearlyEquals(engineFrame, 1E-05f))
						{
							if ((ref ____hoveredSiegeEntityFrame) != (ref globalFrame))
							{
								SiegeEngineConstructionProgress val = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide((BattleSideEnum)1).SiegeEngines.DeployedMeleeSiegeEngines[i];
								InformationManager.ShowTooltip(typeof(List<TooltipProperty>), new object[1] { SandBoxUIHelper.GetSiegeEngineInProgressTooltip(val) });
							}
							return false;
						}
					}
					for (int j = 0; j < ____attackerSiegeTowerSpawnEntities.Length; j++)
					{
						MatrixFrame globalFrame2 = ____attackerSiegeTowerSpawnEntities[j].GetGlobalFrame();
						if (((MatrixFrame)(ref globalFrame2)).NearlyEquals(engineFrame, 1E-05f))
						{
							if ((ref ____hoveredSiegeEntityFrame) != (ref globalFrame2))
							{
								SiegeEngineConstructionProgress val2 = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide((BattleSideEnum)1).SiegeEngines.DeployedMeleeSiegeEngines[____attackerBatteringRamSpawnEntities.Length + j];
								InformationManager.ShowTooltip(typeof(List<TooltipProperty>), new object[1] { SandBoxUIHelper.GetSiegeEngineInProgressTooltip(val2) });
							}
							return false;
						}
					}
					for (int k = 0; k < ____attackerRangedEngineSpawnEntities.Length; k++)
					{
						MatrixFrame globalFrame3 = ____attackerRangedEngineSpawnEntities[k].GetGlobalFrame();
						if (((MatrixFrame)(ref globalFrame3)).NearlyEquals(engineFrame, 1E-05f))
						{
							if ((ref ____hoveredSiegeEntityFrame) != (ref globalFrame3))
							{
								SiegeEngineConstructionProgress val3 = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide((BattleSideEnum)1).SiegeEngines.DeployedRangedSiegeEngines[k];
								InformationManager.ShowTooltip(typeof(List<TooltipProperty>), new object[1] { SandBoxUIHelper.GetSiegeEngineInProgressTooltip(val3) });
							}
							return false;
						}
					}
					for (int l = 0; l < ____defenderRangedEngineSpawnEntitiesCacheForCurrentLevel.Length; l++)
					{
						MatrixFrame globalFrame4 = ____defenderRangedEngineSpawnEntitiesCacheForCurrentLevel[l].GetGlobalFrame();
						if (((MatrixFrame)(ref globalFrame4)).NearlyEquals(engineFrame, 1E-05f))
						{
							if ((ref ____hoveredSiegeEntityFrame) != (ref globalFrame4))
							{
								SiegeEngineConstructionProgress val4 = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide((BattleSideEnum)0).SiegeEngines.DeployedRangedSiegeEngines[l];
								InformationManager.ShowTooltip(typeof(List<TooltipProperty>), new object[1] { SandBoxUIHelper.GetSiegeEngineInProgressTooltip(val4) });
							}
							return false;
						}
					}
					for (int m = 0; m < ____defenderBreachableWallEntitiesCacheForCurrentLevel.Length; m++)
					{
						MatrixFrame globalFrame5 = ____defenderBreachableWallEntitiesCacheForCurrentLevel[m].GetGlobalFrame();
						if (((MatrixFrame)(ref globalFrame5)).NearlyEquals(engineFrame, 1E-05f))
						{
							if ((ref ____hoveredSiegeEntityFrame) != (ref globalFrame5) && ((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.IsSettlement)
							{
								InformationManager.ShowTooltip(typeof(List<TooltipProperty>), new object[1] { SandBoxUIHelper.GetWallSectionTooltip(((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement, m) });
							}
							return false;
						}
					}
					____hoveredSiegeEntityFrame = MatrixFrame.Identity;
				}
				catch (Exception ex)
				{
					LogManager.Log.Info(ex.ToString());
				}
				return false;
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return true;
		}

		[HarmonyFinalizer]
		[HarmonyPatch("OnMapHoverSiegeEngine")]
		public static Exception? FixOnMapHoverSiegeEngine(Exception? __exception, ref SettlementVisual __instance)
		{
			if (__exception != null)
			{
				LogManager.Log.NotifyBad(__exception);
			}
			return null;
		}

		[HarmonyPrefix]
		[HarmonyPatch("OnStartup")]
		public static bool OnStartup(ref SettlementVisual __instance, ref Dictionary<int, List<GameEntity>> ____gateBannerEntitiesWithLevels)
		{
			//IL_0a69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0780: Unknown result type (might be due to invalid IL or missing references)
			//IL_0785: Unknown result type (might be due to invalid IL or missing references)
			//IL_078e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0794: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_058d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				OverwriteSettlementItem overwriteSettlementItem = null;
				bool flag = ((MapEntityVisual<PartyBase>)(object)__instance).MapEntity != null && ((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement.IsPlayerBuilt();
				bool flag2 = ((MapEntityVisual<PartyBase>)(object)__instance).MapEntity != null && ((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement.IsOverwritten(out overwriteSettlementItem);
				if (!flag && !flag2)
				{
					return true;
				}
				bool flag3 = false;
				if (!flag2)
				{
					SetStrategicEntity.Invoke(__instance, new object[1] { __instance.MapScene().GetCampaignEntityWithName(((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Id) });
				}
				if (__instance.StrategicEntity == (GameEntity)null)
				{
					IMapScene mapSceneWrapper = Campaign.Current.MapSceneWrapper;
					string stringId = ((MBObjectBase)((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement).StringId;
					CampaignVec2 position = ((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement.Position;
					mapSceneWrapper.AddNewEntityToMapScene(stringId, ref position);
					SetStrategicEntity.Invoke(__instance, new object[1] { __instance.MapScene().GetCampaignEntityWithName(((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Id) });
				}
				if (__instance.StrategicEntity != (GameEntity)null && overwriteSettlementItem == null)
				{
					PlayerSettlementItem playerSettlementItem = PlayerSettlementInfo.Instance?.FindSettlement(((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement);
					if (playerSettlementItem?.RotationMat3 != null)
					{
						MatrixFrame frame = __instance.StrategicEntity.GetFrame();
						frame.rotation = playerSettlementItem.RotationMat3;
						__instance.StrategicEntity.SetFrame(ref frame, true);
					}
					if (playerSettlementItem?.DeepEdits != null)
					{
						GameEntity strategicEntity = __instance.StrategicEntity;
						List<GameEntity> list = new List<GameEntity>();
						strategicEntity.GetChildrenRecursive(ref list);
						foreach (DeepTransformEdit deepEdit in playerSettlementItem.DeepEdits)
						{
							GameEntity val = ((deepEdit.Index < 0) ? strategicEntity : list[deepEdit.Index]);
							MatrixFrame frame2 = val.GetFrame();
							frame2.rotation = ((deepEdit?.Transform?.RotationScale != null) ? ((Mat3)deepEdit.Transform.RotationScale) : frame2.rotation);
							if (deepEdit.Index >= 0)
							{
								frame2.origin = ((deepEdit?.Transform?.Position != null) ? ((Vec3)deepEdit.Transform.Position) : frame2.origin);
							}
							else
							{
								frame2.origin = ((deepEdit?.Transform?.Offsets != null) ? (frame2.origin + (Vec3)deepEdit.Transform.Offsets) : frame2.origin);
							}
							val.SetFrame(ref frame2, true);
						}
						try
						{
							foreach (DeepTransformEdit item in from d in playerSettlementItem.DeepEdits.AsEnumerable().Reverse()
								where d.IsDeleted && d.Index >= 0
								select d)
							{
								if (item.Index >= 0)
								{
									GameEntity entity = list[item.Index];
									entity.ClearEntity();
								}
							}
						}
						catch (Exception ex)
						{
							LogManager.EventTracer.Trace(new List<string> { ex.Message, ex.StackTrace });
						}
					}
				}
				if (__instance.StrategicEntity != (GameEntity)null && overwriteSettlementItem != null)
				{
					if (overwriteSettlementItem?.RotationMat3 != null)
					{
						MatrixFrame frame3 = __instance.StrategicEntity.GetFrame();
						frame3.rotation = overwriteSettlementItem.RotationMat3;
						__instance.StrategicEntity.SetFrame(ref frame3, true);
					}
					if (overwriteSettlementItem?.DeepEdits != null)
					{
						GameEntity strategicEntity2 = __instance.StrategicEntity;
						List<GameEntity> list2 = new List<GameEntity>();
						strategicEntity2.GetChildrenRecursive(ref list2);
						foreach (DeepTransformEdit deepEdit2 in overwriteSettlementItem.DeepEdits)
						{
							GameEntity val2 = ((deepEdit2.Index < 0) ? strategicEntity2 : list2[deepEdit2.Index]);
							MatrixFrame frame4 = val2.GetFrame();
							frame4.rotation = ((deepEdit2?.Transform?.RotationScale != null) ? ((Mat3)deepEdit2.Transform.RotationScale) : frame4.rotation);
							if (deepEdit2.Index >= 0)
							{
								frame4.origin = ((deepEdit2?.Transform?.Position != null) ? ((Vec3)deepEdit2.Transform.Position) : frame4.origin);
							}
							else
							{
								frame4.origin = ((deepEdit2?.Transform?.Offsets != null) ? (frame4.origin + (Vec3)deepEdit2.Transform.Offsets) : frame4.origin);
							}
							val2.SetFrame(ref frame4, true);
						}
						try
						{
							foreach (DeepTransformEdit item2 in from d in overwriteSettlementItem.DeepEdits.AsEnumerable().Reverse()
								where d.IsDeleted && d.Index >= 0
								select d)
							{
								if (item2.Index >= 0)
								{
									GameEntity entity2 = list2[item2.Index];
									entity2.ClearEntity();
								}
							}
						}
						catch (Exception ex2)
						{
							LogManager.EventTracer.Trace(new List<string> { ex2.Message, ex2.StackTrace });
						}
					}
				}
				bool flag4 = false;
				if (((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement.IsFortification)
				{
					List<GameEntity> list3 = new List<GameEntity>();
					__instance.StrategicEntity.GetChildrenRecursive(ref list3);
					PopulateSiegeEngineFrameListsFromChildren.Invoke(__instance, new object[1] { list3 });
					UpdateDefenderSiegeEntitiesCache.Invoke(__instance, null);
					SetTownPhysicalEntities.Invoke(__instance, new object[1] { list3.FindAll((GameEntity x) => x.HasTag("bo_town")) });
					List<GameEntity> list4 = new List<GameEntity>();
					Dictionary<int, List<GameEntity>> dictionary = new Dictionary<int, List<GameEntity>>
					{
						{
							1,
							new List<GameEntity>()
						},
						{
							2,
							new List<GameEntity>()
						},
						{
							3,
							new List<GameEntity>()
						}
					};
					foreach (GameEntity item3 in list3)
					{
						if (item3.HasTag("main_map_city_gate"))
						{
							MatrixFrame globalFrame = item3.GetGlobalFrame();
							NavigationHelper.IsPositionValidForNavigationType(new CampaignVec2(((Vec3)(ref globalFrame.origin)).AsVec2, true), (NavigationType)1);
							flag4 = true;
							list4.Add(item3);
						}
						if (item3.HasTag("map_settlement_circle"))
						{
							SetCircleLocalFrame.Invoke(__instance, new object[1] { item3.GetGlobalFrame() });
							flag3 = true;
							item3.SetVisibilityExcludeParents(false);
							list4.Add(item3);
						}
						if (item3.HasTag("map_banner_placeholder"))
						{
							int upgradeLevelOfEntity = item3.Parent.GetUpgradeLevelOfEntity();
							if (upgradeLevelOfEntity != 0)
							{
								dictionary[upgradeLevelOfEntity].Add(item3);
							}
							else
							{
								dictionary[1].Add(item3);
								dictionary[2].Add(item3);
								dictionary[3].Add(item3);
							}
							list4.Add(item3);
						}
					}
					____gateBannerEntitiesWithLevels = dictionary;
					if (((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement.IsFortification)
					{
						List<MatrixFrame> list5 = default(List<MatrixFrame>);
						List<MatrixFrame> list6 = default(List<MatrixFrame>);
						Campaign.Current.MapSceneWrapper.GetSiegeCampFrames(((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement, ref list5, ref list6);
						((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement.Town.BesiegerCampPositions1 = list5.ToArray();
						((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement.Town.BesiegerCampPositions2 = list6.ToArray();
					}
					foreach (GameEntity item4 in list4)
					{
						item4.Remove(112);
					}
					if (!flag4 && !((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement.IsTown)
					{
						bool isCastle = ((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement.IsCastle;
					}
					bool flag5 = false;
					if (((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.IsSettlement)
					{
						foreach (GameEntity child in __instance.StrategicEntity.GetChildren())
						{
							if (child.HasTag("main_map_city_port"))
							{
								MatrixFrame globalFrame2 = child.GetGlobalFrame();
								NavigationHelper.IsPositionValidForNavigationType(new CampaignVec2(((Vec3)(ref globalFrame2.origin)).AsVec2, false), (NavigationType)2);
								flag5 = true;
							}
						}
						if ((flag5 || !((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement.HasPort) && flag5)
						{
							bool hasPort = ((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement.HasPort;
						}
					}
				}
				if (!flag3)
				{
					SetCircleLocalFrame.Invoke(__instance, new object[1] { MatrixFrame.Identity });
					MatrixFrame circleLocalFrame = ((MapEntityVisual)__instance).CircleLocalFrame;
					Mat3 rotation = circleLocalFrame.rotation;
					if (((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement.IsVillage)
					{
						((Mat3)(ref rotation)).ApplyScaleLocal(1.75f);
					}
					else if (((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement.IsTown)
					{
						((Mat3)(ref rotation)).ApplyScaleLocal(5.75f);
					}
					else if (!((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement.IsCastle)
					{
						((Mat3)(ref rotation)).ApplyScaleLocal(1.75f);
					}
					else
					{
						((Mat3)(ref rotation)).ApplyScaleLocal(2.75f);
					}
					circleLocalFrame.rotation = rotation;
					SetCircleLocalFrame.Invoke(__instance, new object[1] { circleLocalFrame });
				}
				__instance.StrategicEntity.SetVisibilityExcludeParents(((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.IsVisible);
				__instance.StrategicEntity.SetReadyToRender(true);
				__instance.StrategicEntity.SetEntityEnvMapVisibility(false);
				List<GameEntity> list7 = new List<GameEntity>();
				__instance.StrategicEntity.GetChildrenRecursive(ref list7);
				if (!MapScreen.VisualsOfEntities.ContainsKey(((NativeObject)__instance.StrategicEntity).Pointer))
				{
					MapScreen.VisualsOfEntities.Add(((NativeObject)__instance.StrategicEntity).Pointer, (MapEntityVisual)(object)__instance);
				}
				foreach (GameEntity item5 in list7)
				{
					if (!MapScreen.VisualsOfEntities.ContainsKey(((NativeObject)item5).Pointer) && !MapScreenPatch.FrameAndVisualOfEngines().ContainsKey(((NativeObject)item5).Pointer))
					{
						MapScreen.VisualsOfEntities.Add(((NativeObject)item5).Pointer, (MapEntityVisual)(object)__instance);
					}
				}
				__instance.StrategicEntity.SetAsPredisplayEntity();
				return false;
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return true;
		}

		[HarmonyFinalizer]
		[HarmonyPatch("OnStartup")]
		public static Exception FixOnStartup(object __exception, ref SettlementVisual __instance)
		{
			if (__exception != null)
			{
				if (__exception != null)
				{
					if (__exception is Exception e)
					{
						LogManager.Log.NotifyBad(e);
					}
					else
					{
						LogManager.Log.NotifyBad(__exception.ToString());
					}
				}
			}
			return null;
		}
	}
	[HarmonyPatch(typeof(PlayerTownVisitCampaignBehavior))]
	public static class PlayerTownVisitCampaignBehaviorPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch("game_menu_town_on_init")]
		public static bool game_menu_town_on_init(MenuCallbackArgs args)
		{
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				CampaignTime buildEnd;
				OverwriteSettlementItem overwriteSettlementItem;
				if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsPlayerBuilt())
				{
					PlayerSettlementItem playerSettlementItem = PlayerSettlementInfo.Instance?.Towns?.FirstOrDefault((PlayerSettlementItem t) => t.Settlement == Settlement.CurrentSettlement);
					if (Main.Settings.InstantBuild)
					{
						return true;
					}
					if (playerSettlementItem == null)
					{
						goto IL_008c;
					}
					buildEnd = playerSettlementItem.BuildEnd;
					if (((CampaignTime)(ref buildEnd)).IsFuture)
					{
						goto IL_008c;
					}
				}
				else if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsOverwritten(out overwriteSettlementItem))
				{
					if (Main.Settings.InstantBuild)
					{
						return true;
					}
					if (overwriteSettlementItem == null)
					{
						goto IL_00fb;
					}
					buildEnd = overwriteSettlementItem.BuildEnd;
					if (((CampaignTime)(ref buildEnd)).IsFuture)
					{
						goto IL_00fb;
					}
				}
				goto end_IL_0001;
				IL_008c:
				Campaign.Current.CurrentMenuContext.SwitchToMenu("player_settlement_construction");
				return false;
				IL_00fb:
				Campaign.Current.CurrentMenuContext.SwitchToMenu("player_settlement_construction");
				return false;
				end_IL_0001:;
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return true;
		}

		[HarmonyPrefix]
		[HarmonyPatch("game_menu_village_on_init")]
		public static bool game_menu_village_on_init(MenuCallbackArgs args)
		{
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				CampaignTime buildEnd;
				OverwriteSettlementItem overwriteSettlementItem;
				if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsPlayerBuilt())
				{
					if (Main.Settings.InstantBuild)
					{
						return true;
					}
					PlayerSettlementItem playerSettlementItem = PlayerSettlementInfo.Instance?.PlayerVillages?.FirstOrDefault((PlayerSettlementItem v) => v.Settlement == Settlement.CurrentSettlement) ?? PlayerSettlementInfo.Instance?.Towns?.SelectMany((PlayerSettlementItem t) => t.Villages)?.FirstOrDefault((PlayerSettlementItem v) => v.Settlement == Settlement.CurrentSettlement) ?? PlayerSettlementInfo.Instance?.Castles?.SelectMany((PlayerSettlementItem t) => t.Villages)?.FirstOrDefault((PlayerSettlementItem v) => v.Settlement == Settlement.CurrentSettlement);
					if (playerSettlementItem == null)
					{
						goto IL_0165;
					}
					buildEnd = playerSettlementItem.BuildEnd;
					if (((CampaignTime)(ref buildEnd)).IsFuture)
					{
						goto IL_0165;
					}
				}
				else if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsOverwritten(out overwriteSettlementItem))
				{
					if (Main.Settings.InstantBuild)
					{
						return true;
					}
					if (overwriteSettlementItem == null)
					{
						goto IL_01d4;
					}
					buildEnd = overwriteSettlementItem.BuildEnd;
					if (((CampaignTime)(ref buildEnd)).IsFuture)
					{
						goto IL_01d4;
					}
				}
				goto end_IL_0001;
				IL_01d4:
				Campaign.Current.CurrentMenuContext.SwitchToMenu("player_settlement_construction");
				return false;
				IL_0165:
				Campaign.Current.CurrentMenuContext.SwitchToMenu("player_settlement_construction");
				return false;
				end_IL_0001:;
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return true;
		}

		[HarmonyPrefix]
		[HarmonyPatch("game_menu_castle_on_init")]
		public static bool game_menu_castle_on_init(MenuCallbackArgs args)
		{
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				CampaignTime buildEnd;
				OverwriteSettlementItem overwriteSettlementItem;
				if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsPlayerBuilt())
				{
					PlayerSettlementItem playerSettlementItem = PlayerSettlementInfo.Instance?.Castles?.FirstOrDefault((PlayerSettlementItem t) => t.Settlement == Settlement.CurrentSettlement);
					if (Main.Settings.InstantBuild)
					{
						return true;
					}
					if (playerSettlementItem == null)
					{
						goto IL_008c;
					}
					buildEnd = playerSettlementItem.BuildEnd;
					if (((CampaignTime)(ref buildEnd)).IsFuture)
					{
						goto IL_008c;
					}
				}
				else if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsOverwritten(out overwriteSettlementItem))
				{
					if (Main.Settings.InstantBuild)
					{
						return true;
					}
					if (overwriteSettlementItem == null)
					{
						goto IL_00fb;
					}
					buildEnd = overwriteSettlementItem.BuildEnd;
					if (((CampaignTime)(ref buildEnd)).IsFuture)
					{
						goto IL_00fb;
					}
				}
				goto end_IL_0001;
				IL_008c:
				Campaign.Current.CurrentMenuContext.SwitchToMenu("player_settlement_construction");
				return false;
				IL_00fb:
				Campaign.Current.CurrentMenuContext.SwitchToMenu("player_settlement_construction");
				return false;
				end_IL_0001:;
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return true;
		}
	}
	[HarmonyPatch(typeof(Scene))]
	public static class ScenePatch
	{
		[HarmonyPrefix]
		[HarmonyPatch("GetCampaignEntityWithName")]
		public static bool GetCampaignEntityWithName(ref Scene __instance, ref string name)
		{
			try
			{
				if (name.IsPlayerBuiltStringId() && name != null && name.StartsWith("player_settlement_town_"))
				{
					try
					{
						string s = name.Replace("player_settlement_town_", "").Split('_')[0];
						if (int.TryParse(s, out var result) && result > 35)
						{
							name = (name.Contains("village") ? $"player_settlement_town_1_village_{int.Parse(name.Split('_').Last())}" : "player_settlement_town_1");
						}
					}
					catch (Exception)
					{
					}
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return true;
		}
	}
	[HarmonyPatch(typeof(SettlementNameplatesVM))]
	public static class SettlementNameplatesVMPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch("OnSettlementOwnerChanged")]
		private static bool OnSettlementOwnerChanged(ref SettlementNameplatesVM __instance, Settlement settlement, bool openToClaim, Hero newOwner, Hero previousOwner, Hero capturerHero, ChangeOwnerOfSettlementDetail detail)
		{
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Invalid comparison between Unknown and I4
			try
			{
				SettlementNameplateVM val = ((IEnumerable<SettlementNameplateVM>)__instance.AllNameplates).FirstOrDefault((SettlementNameplateVM n) => n.Settlement == settlement);
				if (val != null)
				{
					((NameplateVM)val).RefreshDynamicProperties(true);
				}
				if (val != null)
				{
					((NameplateVM)val).RefreshRelationStatus();
				}
				foreach (Village boundVillage in (List<Village>)(object)settlement.BoundVillages)
				{
					List<SettlementNameplateVM> list = ((IEnumerable<SettlementNameplateVM>)__instance.AllNameplates).Where((SettlementNameplateVM n) => n.Settlement.IsVillage && n.Settlement.Village == boundVillage).ToList();
					SettlementNameplateVM val2;
					if (list.Count > 1)
					{
						List<string> list2 = new List<string>
						{
							$"Found {list.Count} settlements that share a village component!",
							$"Village: {boundVillage}"
						};
						list2.AddRange(list.Select((SettlementNameplateVM v) => "\t" + ((object)v.Settlement).ToString()));
						LogManager.EventTracer.Trace(list2);
						val2 = list.SingleOrDefault((SettlementNameplateVM v) => v.Settlement == ((SettlementComponent)boundVillage).Settlement);
					}
					else
					{
						val2 = list.SingleOrDefault();
					}
					if (val2 != null)
					{
						((NameplateVM)val2).RefreshDynamicProperties(true);
					}
					if (val2 != null)
					{
						((NameplateVM)val2).RefreshRelationStatus();
					}
				}
				if ((int)detail == 6)
				{
					SettlementNameplateVM val3 = ((IEnumerable<SettlementNameplateVM>)__instance.AllNameplates).FirstOrDefault((SettlementNameplateVM n) => n.Settlement == settlement);
					if (val3 == null)
					{
						return false;
					}
					val3.OnRebelliousClanFormed(newOwner.Clan);
					return false;
				}
				if (previousOwner != null && previousOwner.IsRebel)
				{
					SettlementNameplateVM val4 = ((IEnumerable<SettlementNameplateVM>)__instance.AllNameplates).FirstOrDefault((SettlementNameplateVM n) => n.Settlement == settlement);
					if (val4 == null)
					{
						return false;
					}
					val4.OnRebelliousClanDisbanded(previousOwner.Clan);
				}
				return false;
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return true;
		}
	}
	[HarmonyPatch(typeof(SettlementVisualManager))]
	public static class SettlementVisualManagerPatch
	{
		private static MethodInfo GetDesiredDecalColorMethod = AccessTools.Method(typeof(SettlementVisualManager), "GetDesiredDecalColor");

		private static MethodInfo GetDesiredMaterialNameMethod = AccessTools.Method(typeof(SettlementVisualManager), "GetDesiredMaterialName");

		public static uint GetDesiredDecalColor(this SettlementVisualManager instance, bool isHovered, bool isEnemy, bool isEmpty, bool isPlayerLeader)
		{
			return (uint)GetDesiredDecalColorMethod.Invoke(instance, new object[4] { isHovered, isEnemy, isEmpty, isPlayerLeader });
		}

		public static string GetDesiredMaterialName(this SettlementVisualManager instance, bool isRanged, bool isAttacker, bool isTower)
		{
			return (string)GetDesiredMaterialNameMethod.Invoke(instance, new object[3] { isRanged, isAttacker, isTower });
		}

		[HarmonyPrefix]
		[HarmonyPatch("TickSiegeMachineCircles")]
		public static bool TickSiegeMachineCircles(ref SettlementVisualManager __instance, ref UIntPtr ____hoveredSiegeEntityID, ref GameEntity[] ____defenderMachinesCircleEntities, ref GameEntity[] ____attackerRangedMachinesCircleEntities, ref GameEntity[] ____attackerRamMachinesCircleEntities, ref GameEntity[] ____attackerTowerMachinesCircleEntities)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Invalid comparison between Unknown and I4
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Invalid comparison between Unknown and I4
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Invalid comparison between Unknown and I4
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ee: Invalid comparison between Unknown and I4
			//IL_0520: Unknown result type (might be due to invalid IL or missing references)
			//IL_0525: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0677: Unknown result type (might be due to invalid IL or missing references)
			//IL_067c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0681: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				SiegeEvent playerSiegeEvent = PlayerSiege.PlayerSiegeEvent;
				bool isPlayerLeader = playerSiegeEvent != null && playerSiegeEvent.IsPlayerSiegeEvent && Campaign.Current.Models.EncounterModel.GetLeaderOfSiegeEvent(playerSiegeEvent, PlayerSiege.PlayerSide) == Hero.MainHero;
				bool? obj;
				if (playerSiegeEvent == null)
				{
					obj = null;
				}
				else
				{
					BesiegerCamp besiegerCamp = playerSiegeEvent.BesiegerCamp;
					obj = ((besiegerCamp != null) ? new bool?(besiegerCamp.IsPreparationComplete) : null);
				}
				bool? flag = obj;
				bool valueOrDefault = flag.GetValueOrDefault();
				Settlement settlement = playerSiegeEvent?.BesiegedSettlement;
				if (!settlement.IsPlayerBuilt() && !settlement.IsOverwritten(out OverwriteSettlementItem _))
				{
					return true;
				}
				try
				{
					ISiegeEventSide siegeEventSide = playerSiegeEvent.GetSiegeEventSide((BattleSideEnum)0);
					SiegeEngineConstructionProgress[] array = ((siegeEventSide != null) ? siegeEventSide.SiegeEngines : null)?.DeployedRangedSiegeEngines;
					if (array != null && array.Length < 4)
					{
						Array.Resize(ref array, 4);
					}
					ISiegeEventSide siegeEventSide2 = playerSiegeEvent.GetSiegeEventSide((BattleSideEnum)1);
					SiegeEnginesContainer val = ((siegeEventSide2 != null) ? siegeEventSide2.SiegeEngines : null);
					SiegeEngineConstructionProgress[] array2 = val?.DeployedRangedSiegeEngines;
					if (array != null && array.Length < 4)
					{
						Array.Resize(ref array, 4);
					}
					SiegeEngineConstructionProgress[] array3 = val?.DeployedMeleeSiegeEngines;
					if (array3 != null && array3.Length < 3)
					{
						Array.Resize(ref array3, 3);
					}
				}
				catch (Exception ex)
				{
					LogManager.EventTracer.Trace(new List<string> { ex.Message, ex.StackTrace });
				}
				SettlementVisual settlementVisual = __instance.GetSettlementVisual(playerSiegeEvent.BesiegedSettlement);
				Tuple<MatrixFrame, SettlementVisual> tuple = null;
				if (____hoveredSiegeEntityID != UIntPtr.Zero)
				{
					tuple = MapScreenPatch.FrameAndVisualOfEngines()[____hoveredSiegeEntityID];
				}
				MatrixFrame globalFrame;
				for (int i = 0; i < settlementVisual.GetDefenderRangedSiegeEngineFrames().Length; i++)
				{
					if (i < playerSiegeEvent.GetSiegeEventSide((BattleSideEnum)0).SiegeEngines.DeployedRangedSiegeEngines.Length)
					{
						bool isEmpty = playerSiegeEvent.GetSiegeEventSide((BattleSideEnum)0).SiegeEngines.DeployedRangedSiegeEngines[i] == null;
						bool isEnemy = (int)PlayerSiege.PlayerSide > 0;
						string desiredMaterialName = __instance.GetDesiredMaterialName(isRanged: true, isAttacker: false, isTower: false);
						GameEntityComponent componentAtIndex = ____defenderMachinesCircleEntities[i].GetComponentAtIndex(0, (ComponentType)7);
						Decal val2 = (Decal)(object)((componentAtIndex is Decal) ? componentAtIndex : null);
						Material material = val2.GetMaterial();
						string text = ((!((NativeObject)(object)material != (NativeObject)null)) ? null : material.Name);
						if (text != desiredMaterialName)
						{
							val2.SetMaterial(Material.GetFromResource(desiredMaterialName));
						}
						bool isHovered;
						if (tuple == null)
						{
							isHovered = false;
						}
						else
						{
							globalFrame = ____defenderMachinesCircleEntities[i].GetGlobalFrame();
							isHovered = ((MatrixFrame)(ref globalFrame)).NearlyEquals(tuple.Item1, 1E-05f);
						}
						uint desiredDecalColor = __instance.GetDesiredDecalColor(isHovered, isEnemy, isEmpty, isPlayerLeader);
						if (desiredDecalColor != val2.GetFactor1())
						{
							val2.SetFactor1(desiredDecalColor);
						}
					}
				}
				for (int j = 0; j < settlementVisual.GetAttackerRangedSiegeEngineFrames().Length; j++)
				{
					if (j < playerSiegeEvent.GetSiegeEventSide((BattleSideEnum)1).SiegeEngines.DeployedRangedSiegeEngines.Length)
					{
						bool isEmpty2 = playerSiegeEvent.GetSiegeEventSide((BattleSideEnum)1).SiegeEngines.DeployedRangedSiegeEngines[j] == null;
						bool isEnemy2 = (int)PlayerSiege.PlayerSide != 1;
						string desiredMaterialName2 = __instance.GetDesiredMaterialName(isRanged: true, isAttacker: true, isTower: false);
						GameEntityComponent componentAtIndex2 = ____attackerRangedMachinesCircleEntities[j].GetComponentAtIndex(0, (ComponentType)7);
						Decal val3 = (Decal)(object)((componentAtIndex2 is Decal) ? componentAtIndex2 : null);
						Material material2 = val3.GetMaterial();
						string text2 = ((!((NativeObject)(object)material2 != (NativeObject)null)) ? null : material2.Name);
						if (text2 != desiredMaterialName2)
						{
							val3.SetMaterial(Material.GetFromResource(desiredMaterialName2));
						}
						bool isHovered2;
						if (tuple == null)
						{
							isHovered2 = false;
						}
						else
						{
							globalFrame = ____attackerRangedMachinesCircleEntities[j].GetGlobalFrame();
							isHovered2 = ((MatrixFrame)(ref globalFrame)).NearlyEquals(tuple.Item1, 1E-05f);
						}
						uint desiredDecalColor2 = __instance.GetDesiredDecalColor(isHovered2, isEnemy2, isEmpty2, isPlayerLeader);
						if (desiredDecalColor2 != val3.GetFactor1())
						{
							val3.SetFactor1(desiredDecalColor2);
						}
					}
				}
				for (int k = 0; k < settlementVisual.GetAttackerBatteringRamSiegeEngineFrames().Length; k++)
				{
					if (k < playerSiegeEvent.GetSiegeEventSide((BattleSideEnum)1).SiegeEngines.DeployedMeleeSiegeEngines.Length)
					{
						bool isEmpty3 = playerSiegeEvent.GetSiegeEventSide((BattleSideEnum)1).SiegeEngines.DeployedMeleeSiegeEngines[k] == null;
						bool isEnemy3 = (int)PlayerSiege.PlayerSide != 1;
						string desiredMaterialName3 = __instance.GetDesiredMaterialName(isRanged: false, isAttacker: true, isTower: false);
						GameEntityComponent componentAtIndex3 = ____attackerRamMachinesCircleEntities[k].GetComponentAtIndex(0, (ComponentType)7);
						Decal val4 = (Decal)(object)((componentAtIndex3 is Decal) ? componentAtIndex3 : null);
						Material material3 = val4.GetMaterial();
						string text3 = ((!((NativeObject)(object)material3 != (NativeObject)null)) ? null : material3.Name);
						if (text3 != desiredMaterialName3)
						{
							val4.SetMaterial(Material.GetFromResource(desiredMaterialName3));
						}
						bool isHovered3;
						if (tuple == null)
						{
							isHovered3 = false;
						}
						else
						{
							globalFrame = ____attackerRamMachinesCircleEntities[k].GetGlobalFrame();
							isHovered3 = ((MatrixFrame)(ref globalFrame)).NearlyEquals(tuple.Item1, 1E-05f);
						}
						uint desiredDecalColor3 = __instance.GetDesiredDecalColor(isHovered3, isEnemy3, isEmpty3, isPlayerLeader);
						if (desiredDecalColor3 != val4.GetFactor1())
						{
							val4.SetFactor1(desiredDecalColor3);
						}
					}
				}
				for (int l = 0; l < settlementVisual.GetAttackerTowerSiegeEngineFrames().Length; l++)
				{
					if (settlementVisual.GetAttackerBatteringRamSiegeEngineFrames().Length + l < playerSiegeEvent.GetSiegeEventSide((BattleSideEnum)1).SiegeEngines.DeployedMeleeSiegeEngines.Length)
					{
						bool isEmpty4 = playerSiegeEvent.GetSiegeEventSide((BattleSideEnum)1).SiegeEngines.DeployedMeleeSiegeEngines[settlementVisual.GetAttackerBatteringRamSiegeEngineFrames().Length + l] == null;
						bool isEnemy4 = (int)PlayerSiege.PlayerSide != 1;
						string desiredMaterialName4 = __instance.GetDesiredMaterialName(isRanged: false, isAttacker: true, isTower: true);
						GameEntityComponent componentAtIndex4 = ____attackerTowerMachinesCircleEntities[l].GetComponentAtIndex(0, (ComponentType)7);
						Decal val5 = (Decal)(object)((componentAtIndex4 is Decal) ? componentAtIndex4 : null);
						Material material4 = val5.GetMaterial();
						string text4 = ((!((NativeObject)(object)material4 != (NativeObject)null)) ? null : material4.Name);
						if (text4 != desiredMaterialName4)
						{
							val5.SetMaterial(Material.GetFromResource(desiredMaterialName4));
						}
						bool isHovered4;
						if (tuple == null)
						{
							isHovered4 = false;
						}
						else
						{
							globalFrame = ____attackerTowerMachinesCircleEntities[l].GetGlobalFrame();
							isHovered4 = ((MatrixFrame)(ref globalFrame)).NearlyEquals(tuple.Item1, 1E-05f);
						}
						uint desiredDecalColor4 = __instance.GetDesiredDecalColor(isHovered4, isEnemy4, isEmpty4, isPlayerLeader);
						if (desiredDecalColor4 != val5.GetFactor1())
						{
							val5.SetFactor1(desiredDecalColor4);
						}
					}
				}
				return false;
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return true;
		}

		[HarmonyFinalizer]
		[HarmonyPatch("TickSiegeMachineCircles")]
		public static Exception? FixTickSiegeMachineCircles(Exception? __exception, ref MapScreen __instance)
		{
			if (__exception != null)
			{
				LogManager.Log.NotifyBad(__exception);
			}
			return null;
		}
	}
	[HarmonyPatch(typeof(SiegeEnginesContainer))]
	public static class SiegeEnginesContainerPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch("DeploySiegeEngineAtIndex")]
		public static void DeploySiegeEngineAtIndex(ref SiegeEnginesContainer __instance, SiegeEngineConstructionProgress siegeEngine, int index)
		{
			try
			{
				SiegeEngineConstructionProgress[] array = (siegeEngine.SiegeEngine.IsRanged ? __instance.DeployedRangedSiegeEngines : __instance.DeployedMeleeSiegeEngines);
				if (index >= array.Length)
				{
					Array.Resize(ref array, index + 1);
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch("RemoveDeployedSiegeEngine")]
		public static void RemoveDeployedSiegeEngine(ref SiegeEnginesContainer __instance, int index, bool isRanged, bool moveToReserve)
		{
			try
			{
				SiegeEngineConstructionProgress[] array = (isRanged ? __instance.DeployedRangedSiegeEngines : __instance.DeployedMeleeSiegeEngines);
				if (index >= array.Length)
				{
					Array.Resize(ref array, index + 1);
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
		}

		[HarmonyFinalizer]
		[HarmonyPatch("DeploySiegeEngineAtIndex")]
		public static Exception? FixDeploySiegeEngineAtIndex(Exception? __exception, ref SiegeEnginesContainer __instance, SiegeEngineConstructionProgress siegeEngine, int index)
		{
			if (__exception != null)
			{
				LogManager.Log.NotifyBad(__exception);
			}
			return null;
		}

		[HarmonyFinalizer]
		[HarmonyPatch("RemoveDeployedSiegeEngine")]
		public static Exception? FixRemoveDeployedSiegeEngine(Exception? __exception, ref SiegeEnginesContainer __instance, int index, bool isRanged, bool moveToReserve)
		{
			if (__exception != null)
			{
				LogManager.Log.NotifyBad(__exception);
			}
			return null;
		}
	}
	[HarmonyPatch(typeof(TownManagementVM))]
	public static class TownManagementVMPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch("OnProjectSelectionDone")]
		public static bool OnProjectSelectionDone(ref TownManagementVM __instance, ref Settlement ____settlement)
		{
			try
			{
				if (__instance.ProjectSelection.CurrentDailyDefault == null)
				{
					__instance.ProjectSelection.CurrentDailyDefault = ((IEnumerable<SettlementDailyProjectVM>)__instance.ProjectSelection.DailyDefaultList).FirstOrDefault();
				}
				if (__instance.ProjectSelection.CurrentDailyDefault == null)
				{
					LogManager.Log.NotifyBad("[TownManagementVM] (ProjectSelection.CurrentDailyDefault) is null!");
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return true;
		}
	}
	[HarmonyPatch(typeof(WorkshopMissionHandler))]
	public static class WorkshopMissionHandlerPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch("InitShopSigns")]
		public static bool InitShopSigns(ref WorkshopMissionHandler __instance, ref Settlement ____settlement, ref List<Tuple<Workshop, GameEntity>> ____workshopSignEntities)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Invalid comparison between Unknown and I4
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
				Settlement settlement = ((locationEncounter != null) ? locationEncounter.Settlement : null);
				if (!settlement.IsPlayerBuilt() && !____settlement.IsPlayerBuilt())
				{
					return true;
				}
				if ((int)Campaign.Current.GameMode == 1 && ____settlement != null && ____settlement.IsTown)
				{
					List<GameEntity> list = ((MissionBehavior)__instance).Mission.Scene.FindEntitiesWithTag("shop_sign").ToList();
					while (true)
					{
						IL_0079:
						foreach (WorkshopAreaMarker item2 in MBExtensions.FindAllWithType<WorkshopAreaMarker>((IEnumerable<MissionObject>)((MissionBehavior)__instance).Mission.ActiveMissionObjects).ToList())
						{
							if (____settlement.Town.Workshops == null || ____settlement.Town.Workshops.Length == 0)
							{
								continue;
							}
							Workshop workshops = ____settlement.Town.Workshops[((AreaMarker)item2).AreaIndex];
							if (!____workshopSignEntities.All((Tuple<Workshop, GameEntity> x) => x.Item1 != workshops))
							{
								continue;
							}
							for (int i = 0; i < list.Count; i++)
							{
								GameEntity val = list[i];
								if (((AreaMarker)item2).IsPositionInRange(val.GlobalPosition))
								{
									____workshopSignEntities.Add(new Tuple<Workshop, GameEntity>(workshops, val));
									list.RemoveAt(i);
									goto IL_0079;
								}
							}
						}
						break;
					}
					foreach (Tuple<Workshop, GameEntity> ____workshopSignEntity in ____workshopSignEntities)
					{
						GameEntity item = ____workshopSignEntity.Item2;
						WorkshopType workshopType = ____workshopSignEntity.Item1.WorkshopType;
						item.ClearComponents();
						item.AddMultiMesh(MetaMesh.GetCopy((workshopType != null) ? workshopType.SignMeshName : "shop_sign_merchantavailable", true, false), true);
					}
				}
				return false;
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return true;
		}

		[HarmonyFinalizer]
		[HarmonyPatch("InitShopSigns")]
		public static Exception? FixInitShopSigns(Exception? __exception, ref WorkshopMissionHandler __instance, ref Settlement ____settlement)
		{
			if (__exception != null)
			{
				LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
				Settlement settlement = ((locationEncounter != null) ? locationEncounter.Settlement : null);
				if (!settlement.IsPlayerBuilt() && !____settlement.IsPlayerBuilt())
				{
					return __exception;
				}
				LogManager.Log.NotifyBad(__exception);
			}
			return null;
		}
	}
}
namespace BannerlordPlayerSettlement.Patches.Compatibility
{
	public class CalradianPatrolsV2Compatibility : ICompatibilityPatch
	{
		public class Behaviour : CampaignBehaviorBase
		{
			public CalradianPatrolsV2Compatibility? Owner = null;

			public override void RegisterEvents()
			{
				PlayerSettlementBehaviour.SettlementCreatedEvent?.AddNonSerializedListener((object)this, (Action<Settlement>)delegate(Settlement settlement)
				{
					try
					{
						if (settlement.IsTown)
						{
							CampaignGameStarter gameStarter = SandBoxManager.Instance.GameStarter;
							CampaignBehaviorBase val = gameStarter.CampaignBehaviors.FirstOrDefault((CampaignBehaviorBase b) => ((object)b).GetType() == Owner?.behaviorType);
							if (val != null)
							{
								CampaignBehaviorBase obj = val;
								if (Owner?._autoRecruitsField != null)
								{
									object value = Owner._autoRecruitsField.GetValue(obj);
									if (value is Dictionary<Settlement, bool> dictionary)
									{
										dictionary[settlement] = false;
										UpdateKnownTowns(dictionary);
									}
								}
							}
						}
					}
					catch (Exception e)
					{
						LogManager.Log.NotifyBad(e);
					}
				});
			}

			public override void SyncData(IDataStore dataStore)
			{
			}
		}

		private Assembly? assembly;

		private Type? behaviorType;

		private FieldInfo? _autoRecruitsField;

		public bool IsEnabled => assembly != null && behaviorType != null;

		public void AddBehaviors(CampaignGameStarter gameInitializer)
		{
			if (IsEnabled)
			{
				Behaviour behaviour = new Behaviour
				{
					Owner = this
				};
				gameInitializer.AddBehavior((CampaignBehaviorBase)(object)behaviour);
			}
		}

		public void PatchAfterMenus(Harmony harmony)
		{
		}

		public void PatchSubmoduleLoad(Harmony harmony)
		{
			assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.FullName.StartsWith("CalradianPatrolsV2, ", StringComparison.InvariantCultureIgnoreCase));
			if (assembly != null)
			{
				behaviorType = assembly.GetType("CalradianPatrols.Behaviors.PatrolsCampaignBehavior", throwOnError: false, ignoreCase: true);
				if (behaviorType != null)
				{
					_autoRecruitsField = AccessTools.Field(behaviorType, "_autoRecruits");
					harmony.Patch(AccessTools.Method(behaviorType, "OnSettlementEntered"), new HarmonyMethod(typeof(CalradianPatrolsV2Compatibility), "OnSettlementEntered"));
					harmony.Patch(AccessTools.Method(behaviorType, "HourlyTickSettlement"), new HarmonyMethod(typeof(CalradianPatrolsV2Compatibility), "HourlyTickSettlement"));
				}
			}
		}

		private static void HourlyTickSettlement(ref Dictionary<Settlement, bool> ____autoRecruits, object __instance, Settlement settlement)
		{
			Dictionary<Settlement, bool> dictionary = ____autoRecruits;
			if (settlement != null && dictionary != null && settlement.IsTown && !dictionary.ContainsKey(settlement) && settlement.IsPlayerBuilt())
			{
				dictionary[settlement] = false;
			}
		}

		private static void OnSettlementEntered(ref Dictionary<Settlement, bool> ____autoRecruits, object __instance, MobileParty patrolParty, Settlement settlement, Hero hero)
		{
			Dictionary<Settlement, bool> autoRecruits = ____autoRecruits;
			UpdateKnownTowns(autoRecruits);
		}

		private static void UpdateKnownTowns(Dictionary<Settlement, bool> _autoRecruits)
		{
			if (_autoRecruits == null || PlayerSettlementInfo.Instance == null)
			{
				return;
			}
			List<Settlement> list = CollectPlayerBuiltTowns();
			foreach (Settlement item in list)
			{
				if (item != null && item.IsTown && !_autoRecruits.ContainsKey(item))
				{
					_autoRecruits[item] = false;
				}
			}
		}

		private static List<Settlement> CollectPlayerBuiltTowns()
		{
			List<Settlement> list = new List<Settlement>();
			if (PlayerSettlementInfo.Instance == null)
			{
				return list;
			}
			list.AddRange(PlayerSettlementInfo.Instance.Towns?.Select((PlayerSettlementItem t) => t.Settlement) ?? new List<Settlement>());
			return list;
		}
	}
	public class CustomSpawnsCompatibility : ICompatibilityPatch
	{
		public class Behaviour : CampaignBehaviorBase
		{
			public CustomSpawnsCompatibility? Owner = null;

			public override void RegisterEvents()
			{
				PlayerSettlementBehaviour.SettlementCreatedEvent?.AddNonSerializedListener((object)this, (Action<Settlement>)delegate(Settlement settlement)
				{
					try
					{
						if (settlement.IsVillage)
						{
							CampaignGameStarter gameStarter = SandBoxManager.Instance.GameStarter;
							CampaignBehaviorBase val = gameStarter.CampaignBehaviors.FirstOrDefault((CampaignBehaviorBase b) => ((object)b).GetType() == Owner?.DevestationMetricDataBehaviorType);
							if (val != null)
							{
								CampaignBehaviorBase obj = val;
								if (Owner?._settlementToDevestationField != null)
								{
									object value = Owner._settlementToDevestationField.GetValue(obj);
									if (value is Dictionary<Settlement, float> dictionary)
									{
										dictionary[settlement] = 0f;
									}
								}
							}
						}
					}
					catch (Exception e)
					{
						LogManager.Log.NotifyBad(e);
					}
				});
			}

			public override void SyncData(IDataStore dataStore)
			{
			}
		}

		private Assembly? customSpawnsAssembly;

		private Type? DevestationMetricDataBehaviorType;

		private FieldInfo? _settlementToDevestationField;

		public bool IsEnabled => customSpawnsAssembly != null && DevestationMetricDataBehaviorType != null;

		public void AddBehaviors(CampaignGameStarter gameInitializer)
		{
			if (IsEnabled)
			{
				Behaviour behaviour = new Behaviour
				{
					Owner = this
				};
				gameInitializer.AddBehavior((CampaignBehaviorBase)(object)behaviour);
			}
		}

		public void PatchAfterMenus(Harmony harmony)
		{
		}

		public void PatchSubmoduleLoad(Harmony harmony)
		{
			customSpawnsAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.FullName.StartsWith("CustomSpawns, "));
			if (customSpawnsAssembly != null)
			{
				DevestationMetricDataBehaviorType = customSpawnsAssembly.GetType("CustomSpawns.CampaignData.Implementations.DevestationMetricData", throwOnError: false, ignoreCase: true);
				if (DevestationMetricDataBehaviorType != null)
				{
					_settlementToDevestationField = AccessTools.Field(DevestationMetricDataBehaviorType, "_settlementToDevestation");
					harmony.Patch(AccessTools.Method(DevestationMetricDataBehaviorType, "OnMapEventEnded"), new HarmonyMethod(typeof(CustomSpawnsCompatibility), "OnMapEventEnded"));
					harmony.Patch(AccessTools.Method(DevestationMetricDataBehaviorType, "GetDevestation"), new HarmonyMethod(typeof(CustomSpawnsCompatibility), "GetDevestation"));
					harmony.Patch(AccessTools.Method(DevestationMetricDataBehaviorType, "OnSettlementDaily"), new HarmonyMethod(typeof(CustomSpawnsCompatibility), "OnSettlementDaily"));
				}
			}
		}

		private static void OnSettlementDaily(ref Dictionary<Settlement, float> ____settlementToDevestation, object __instance, Settlement s)
		{
			Dictionary<Settlement, float> dictionary = ____settlementToDevestation;
			if (s != null && dictionary != null && s.IsVillage && !dictionary.ContainsKey(s) && s.IsPlayerBuilt())
			{
				dictionary[s] = 0f;
			}
		}

		private static void GetDevestation(ref float __result, ref Dictionary<Settlement, float> ____settlementToDevestation, object __instance, Settlement s)
		{
			Dictionary<Settlement, float> dictionary = ____settlementToDevestation;
			if (s != null && dictionary != null && s.IsVillage && !dictionary.ContainsKey(s) && s.IsPlayerBuilt())
			{
				dictionary[s] = 0f;
			}
		}

		private static void OnMapEventEnded(ref Dictionary<Settlement, float> ____settlementToDevestation, object __instance, MapEvent e)
		{
			Dictionary<Settlement, float> settlementToDevestation = ____settlementToDevestation;
			UpdateKnownVillages(settlementToDevestation);
		}

		private static void UpdateKnownVillages(Dictionary<Settlement, float> _settlementToDevestation)
		{
			if (_settlementToDevestation == null || PlayerSettlementInfo.Instance == null)
			{
				return;
			}
			List<Settlement> list = CollectPlayerBuiltVillages();
			foreach (Settlement item in list)
			{
				if (item != null && item.IsVillage && !_settlementToDevestation.ContainsKey(item))
				{
					_settlementToDevestation[item] = 0f;
				}
			}
		}

		private static List<Settlement> CollectPlayerBuiltVillages()
		{
			List<Settlement> list = new List<Settlement>();
			if (PlayerSettlementInfo.Instance == null)
			{
				return list;
			}
			list.AddRange(PlayerSettlementInfo.Instance.Towns?.SelectMany((PlayerSettlementItem t) => t.Villages?.Select((PlayerSettlementItem v) => v.Settlement)) ?? new List<Settlement>());
			list.AddRange(PlayerSettlementInfo.Instance.Castles?.SelectMany((PlayerSettlementItem c) => c.Villages?.Select((PlayerSettlementItem v) => v.Settlement)) ?? new List<Settlement>());
			list.AddRange(PlayerSettlementInfo.Instance.PlayerVillages?.Select((PlayerSettlementItem v) => v.Settlement) ?? new List<Settlement>());
			return list;
		}
	}
	public class LifeInCalradia_HousingCompatibility : ICompatibilityPatch
	{
		private Assembly? assembly;

		private Type? behaviorType;

		public bool IsEnabled => assembly != null && behaviorType != null;

		public void AddBehaviors(CampaignGameStarter gameInitializer)
		{
		}

		public void PatchAfterMenus(Harmony harmony)
		{
		}

		public void PatchSubmoduleLoad(Harmony harmony)
		{
			assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.FullName.StartsWith("LifeInCalradia_Housing, ", StringComparison.InvariantCultureIgnoreCase));
			if (assembly != null)
			{
				behaviorType = assembly.GetType("LifeInCalradia_Housing.Behaviors.HousingBehavior", throwOnError: false, ignoreCase: true);
				if (behaviorType != null)
				{
					harmony.Patch(AccessTools.Method(behaviorType, "GetOrCreateSettlementHousing"), new HarmonyMethod(typeof(LifeInCalradia_HousingCompatibility), "GetOrCreateSettlementHousing"));
				}
			}
		}

		private static bool GetOrCreateSettlementHousing(ref object __result, object __instance, Settlement settlement)
		{
			if (settlement != null && settlement.IsPlayerBuilt() && ((List<Hero>)(object)settlement.Notables).Count == 0)
			{
				__result = null;
				return false;
			}
			return true;
		}
	}
	public class WarSailsDLCCompatibility : ICompatibilityPatch
	{
		public class Behaviour : CampaignBehaviorBase
		{
			public WarSailsDLCCompatibility? Owner = null;

			private Type ShipProductionCampaignBehaviorType = null;

			private MethodInfo DailyTickTownMethod = null;

			private bool HasLoaded { get; set; }

			private Assembly WarSailsAssembly
			{
				get
				{
					return Owner?.assembly;
				}
				set
				{
					if (Owner != null)
					{
						Owner.assembly = value;
					}
				}
			}

			public override void RegisterEvents()
			{
				PlayerSettlementBehaviour.SettlementCreatedEvent?.AddNonSerializedListener((object)this, (Action<Settlement>)SettlementCreated);
				PlayerSettlementBehaviour.SettlementRebuildEvent?.AddNonSerializedListener((object)this, (Action<Settlement>)SettlementRebuilt);
				PlayerSettlementBehaviour.SettlementOverwriteEvent?.AddNonSerializedListener((object)this, (Action<Settlement>)SettlementOverwritten);
				CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnNewGameCreated);
				CampaignEvents.OnGameEarlyLoadedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnGameEarlyLoaded);
				CampaignEvents.DailyTickEvent.AddNonSerializedListener((object)this, (Action)DailyTick);
				CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener((object)this, (Action<Settlement>)DailyTickSettlement);
				CampaignEvents.DailyTickTownEvent.AddNonSerializedListener((object)this, (Action<Town>)DailyTickTown);
				CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener((object)this, (Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementDetail>)SettlementOwnerChanged);
			}

			private void SettlementCreated(Settlement settlement)
			{
				CheckSettlementPort(settlement, forceShips: true);
			}

			private void SettlementRebuilt(Settlement settlement)
			{
				CheckSettlementPort(settlement, forceShips: false);
				CheckShipyard(settlement);
			}

			private void SettlementOverwritten(Settlement settlement)
			{
				CheckSettlementPort(settlement, forceShips: false);
			}

			private void CheckSettlementPort(Settlement settlement, bool forceShips)
			{
				try
				{
					if (!settlement.IsTown)
					{
						return;
					}
					Town town = settlement.Town;
					CampaignGameStarter gameStarter = SandBoxManager.Instance.GameStarter;
					if (!settlement.HasPort)
					{
						return;
					}
					if (ShipProductionCampaignBehaviorType == null)
					{
						if (WarSailsAssembly == null)
						{
							WarSailsAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.FullName.StartsWith("NavalDLC, ", StringComparison.InvariantCultureIgnoreCase));
						}
						if (WarSailsAssembly != null)
						{
							ShipProductionCampaignBehaviorType = WarSailsAssembly.GetType("NavalDLC.CampaignBehaviors.ShipProductionCampaignBehavior", throwOnError: false, ignoreCase: true);
						}
					}
					if (ShipProductionCampaignBehaviorType != null)
					{
						CampaignBehaviorBase val = gameStarter.CampaignBehaviors.FirstOrDefault((CampaignBehaviorBase c) => ((object)c).GetType() == ShipProductionCampaignBehaviorType);
						if (DailyTickTownMethod == null)
						{
							DailyTickTownMethod = AccessTools.Method(ShipProductionCampaignBehaviorType, "DailyTickTown");
						}
						if (DailyTickTownMethod != null && val != null)
						{
							try
							{
								DailyTickTownMethod.Invoke(val, new object[1] { town });
							}
							catch (Exception e)
							{
								LogManager.Log.NotifyBad(e);
							}
						}
					}
					if (town.AvailableShips != null && ((List<Ship>)(object)town.AvailableShips).Count == 0 && forceShips)
					{
						ForceAddTownShips(town);
					}
					if (town.AvailableShips == null)
					{
						return;
					}
					for (int i = 0; i < ((List<Ship>)(object)town.AvailableShips).Count; i++)
					{
						Ship val2 = ((List<Ship>)(object)town.AvailableShips)[i];
						try
						{
							if (val2.Owner == null)
							{
								val2.Owner = settlement.Party;
							}
						}
						catch (Exception e2)
						{
							LogManager.Log.NotifyBad(e2);
						}
					}
				}
				catch (Exception e3)
				{
					LogManager.Log.NotifyBad(e3);
				}
			}

			private void SettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementDetail detail)
			{
				DailyTickSettlement(settlement);
			}

			private void DailyTickTown(Town town)
			{
				DailyTickSettlement(((SettlementComponent)town).Settlement);
			}

			private void DailyTickSettlement(Settlement settlement)
			{
				if (settlement != null && settlement.HasPort && settlement.IsTown)
				{
					Town town = settlement.Town;
					if (((town != null) ? town.AvailableShips : null) != null)
					{
						FixSettlementPortAndShips(settlement);
						CheckShipyard(settlement);
					}
				}
			}

			private void DailyTick()
			{
				try
				{
					LogManager.EventTracer.Trace();
					FixPortsAndShips();
				}
				catch (Exception e)
				{
					LogManager.Log.NotifyBad(e);
				}
			}

			public override void SyncData(IDataStore dataStore)
			{
				if (!dataStore.IsSaving)
				{
					OnLoad();
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
				FixPortsAndShips();
				HasLoaded = true;
			}

			private void FixPortsAndShips()
			{
				List<Settlement> list = CollectPlayerBuiltPorts();
				for (int i = 0; i < list.Count; i++)
				{
					try
					{
						Settlement portSettlement = list[i];
						FixSettlementPortAndShips(portSettlement);
						CheckShipyard(portSettlement);
					}
					catch (Exception e)
					{
						LogManager.Log.NotifyBad(e);
					}
				}
			}

			private static void CheckShipyard(Settlement portSettlement)
			{
				if (portSettlement.HasPort && portSettlement.IsTown)
				{
					Building shipyard = portSettlement.Town.GetShipyard();
					if (shipyard == null)
					{
						LogManager.Log.NotifyBad($"{portSettlement} has a port but not a shipyard!");
					}
				}
			}

			private static void FixSettlementPortAndShips(Settlement portSettlement)
			{
				try
				{
					Town town = portSettlement.Town;
					if (town.AvailableShips == null)
					{
						return;
					}
					for (int i = 0; i < ((List<Ship>)(object)town.AvailableShips).Count; i++)
					{
						try
						{
							Ship val = ((List<Ship>)(object)town.AvailableShips)[i];
							if (val.Owner == null)
							{
								val.Owner = portSettlement.Party;
							}
						}
						catch (Exception e)
						{
							LogManager.Log.NotifyBad(e);
						}
					}
				}
				catch (Exception e2)
				{
					LogManager.Log.NotifyBad(e2);
				}
			}

			public static void ForceAddTownShips(Town town, int count = 5)
			{
				//IL_019e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Expected O, but got Unknown
				if (!Main.IsWarSails)
				{
					return;
				}
				try
				{
					MBList<ShipHull> val = Extensions.ToMBList<ShipHull>(((IEnumerable<Kingdom>)Kingdom.All).SelectMany((Kingdom x) => (IEnumerable<ShipHull>)x.Culture.AvailableShipHulls));
					string empty = string.Empty;
					for (int i = 0; i < count; i++)
					{
						try
						{
							ShipHull randomShipHull = GetRandomShipHull(town);
							if (randomShipHull == null)
							{
								continue;
							}
							Ship val2 = new Ship(randomShipHull);
							List<ShipUpgradePiece> availableShipUpgradePieces = town.GetAvailableShipUpgradePieces();
							Extensions.Shuffle<ShipUpgradePiece>((IList<ShipUpgradePiece>)availableShipUpgradePieces);
							foreach (KeyValuePair<string, ShipSlot> availableSlot in val2.ShipHull.AvailableSlots)
							{
								if (!(MBRandom.RandomFloat > 0.5f))
								{
									continue;
								}
								int num = MBRandom.RandomInt(availableShipUpgradePieces.Count);
								for (int j = 0; j < availableShipUpgradePieces.Count; j++)
								{
									ShipUpgradePiece val3 = availableShipUpgradePieces[(j + num) % availableShipUpgradePieces.Count];
									if (val3.DoesPieceMatchSlot(availableSlot.Value))
									{
										val2.SetPieceAtSlot(availableSlot.Key, val3);
										break;
									}
								}
							}
							ChangeShipOwnerAction.ApplyByProduction(((SettlementComponent)town).Settlement.Party, val2);
							((CampaignEventReceiver)CampaignEventDispatcher.Instance).OnShipCreated(val2, ((SettlementComponent)town).Settlement);
						}
						catch (Exception e)
						{
							LogManager.Log.NotifyBad(e);
						}
					}
					ExplainedNumber val4 = default(ExplainedNumber);
					town.AddEffectOfBuildings((BuildingEffectEnum)28, ref val4);
					int num2 = (int)((ExplainedNumber)(ref val4)).ResultNumber;
					if (((List<Ship>)(object)town.AvailableShips).Count >= num2)
					{
						TryRemoveExcessShipsFromTown(town, num2);
					}
				}
				catch (Exception e2)
				{
					LogManager.Log.NotifyBad(e2);
				}
			}

			private static void TryRemoveExcessShipsFromTown(Town town, int idealShipCountForTown)
			{
				int num = ((List<Ship>)(object)town.AvailableShips).Count - idealShipCountForTown;
				if (num <= 0)
				{
					return;
				}
				List<Ship> shipsOfOtherCulture = ((IEnumerable<Ship>)town.AvailableShips).Where((Ship x) => !((List<ShipHull>)(object)town.Culture.AvailableShipHulls).Contains(x.ShipHull)).ToList();
				foreach (Ship item in shipsOfOtherCulture)
				{
					if (MBRandom.RandomFloat < 0.7f)
					{
						DestroyShipAction.Apply(item);
						num--;
						if (num < 0)
						{
							break;
						}
					}
				}
				if (num <= 0)
				{
					return;
				}
				foreach (Ship item2 in ((IEnumerable<Ship>)town.AvailableShips).Where((Ship x) => !shipsOfOtherCulture.Contains(x)).ToList())
				{
					if (MBRandom.RandomFloat < 0.3f)
					{
						DestroyShipAction.Apply(item2);
						num--;
						if (num < 0)
						{
							break;
						}
					}
				}
			}

			private static ShipHull GetRandomShipHull(Town town)
			{
				MBList<(ShipHull, float)> availableShipHullsForTown = GetAvailableShipHullsForTown(town);
				if (((List<(ShipHull, float)>)(object)availableShipHullsForTown).Count == 0)
				{
					Debug.FailedAssert("Could not find ships to create.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\CampaignBehaviors\\ShipProductionCampaignBehavior.cs", "GetRandomShipHull", 231);
				}
				return MBRandom.ChooseWeighted<ShipHull>((IReadOnlyList<ValueTuple<ShipHull, float>>)availableShipHullsForTown);
			}

			private static MBList<(ShipHull, float)> GetAvailableShipHullsForTown(Town town)
			{
				MBList<(ShipHull, float)> val = new MBList<(ShipHull, float)>();
				foreach (ShipHull item in (List<ShipHull>)(object)town.Culture.AvailableShipHulls)
				{
					if (CanTownCreateShipFromHull(town, item))
					{
						((List<(ShipHull, float)>)(object)val).Add((item, item.ProductionBuildWeight));
					}
				}
				return val;
			}

			private static bool CanTownCreateShipFromHull(Town town, ShipHull shipHull)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Expected I4, but got Unknown
				ShipType type = shipHull.Type;
				if (1 == 0)
				{
				}
				bool result = (int)type switch
				{
					0 => town.GetShipyard().CurrentLevel > 0, 
					1 => town.GetShipyard().CurrentLevel > 1, 
					2 => town.GetShipyard().CurrentLevel == 3, 
					_ => false, 
				};
				if (1 == 0)
				{
				}
				return result;
			}
		}

		private Assembly? assembly;

		private Type? ShipTradeCampaignBehaviorType;

		private Type? NavalDLCShipCostModelType;

		private Type? NavalBuildingTypesType;

		private Type? NavalDLCExtensionsType;

		private Type? NavalDLCMapDistanceModelType;

		public bool IsEnabled => Main.IsWarSails && assembly != null && ShipTradeCampaignBehaviorType != null;

		public void AddBehaviors(CampaignGameStarter gameInitializer)
		{
			if (IsEnabled)
			{
				Behaviour behaviour = new Behaviour
				{
					Owner = this
				};
				gameInitializer.AddBehavior((CampaignBehaviorBase)(object)behaviour);
			}
		}

		public void PatchAfterMenus(Harmony harmony)
		{
		}

		public void PatchSubmoduleLoad(Harmony harmony)
		{
			if (!Main.IsWarSails)
			{
				return;
			}
			assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.FullName.StartsWith("NavalDLC, ", StringComparison.InvariantCultureIgnoreCase));
			if (assembly != null)
			{
				ShipTradeCampaignBehaviorType = assembly.GetType("NavalDLC.CampaignBehaviors.ShipTradeCampaignBehavior", throwOnError: false, ignoreCase: true);
				if (ShipTradeCampaignBehaviorType != null)
				{
					harmony.Patch(AccessTools.Method(ShipTradeCampaignBehaviorType, "TryPurchasingShipFromTown"), new HarmonyMethod(typeof(WarSailsDLCCompatibility), "TryPurchasingShipFromTown"), null, null, new HarmonyMethod(typeof(WarSailsDLCCompatibility), "FixExceptions"));
				}
				NavalDLCMapDistanceModelType = assembly.GetType("NavalDLC.GameComponents.NavalDLCMapDistanceModel", throwOnError: false, ignoreCase: true);
				if (NavalDLCMapDistanceModelType != null)
				{
					harmony.Patch(AccessTools.Method(NavalDLCMapDistanceModelType, "GetDistance", new Type[5]
					{
						typeof(Settlement),
						typeof(Settlement),
						typeof(bool),
						typeof(bool),
						typeof(NavigationType)
					}), new HarmonyMethod(typeof(WarSailsDLCCompatibility), "GetDistance"), new HarmonyMethod(typeof(WarSailsDLCCompatibility), "PostGetDistance"), null, new HarmonyMethod(typeof(WarSailsDLCCompatibility), "FixGetDistanceExceptions"));
				}
				NavalDLCShipCostModelType = assembly.GetType("NavalDLC.GameComponents.NavalDLCShipCostModel", throwOnError: false, ignoreCase: true);
				if (NavalDLCShipCostModelType != null)
				{
					harmony.Patch(AccessTools.Method(NavalDLCShipCostModelType, "GetShipTradeValue"), new HarmonyMethod(typeof(WarSailsDLCCompatibility), "GetShipTradeValue"), null, null, new HarmonyMethod(typeof(WarSailsDLCCompatibility), "FixExceptions"));
					harmony.Patch(AccessTools.Method(NavalDLCShipCostModelType, "GetShipBaseValue"), new HarmonyMethod(typeof(WarSailsDLCCompatibility), "GetShipBaseValue"), null, null, new HarmonyMethod(typeof(WarSailsDLCCompatibility), "FixExceptions"));
				}
				NavalBuildingTypesType = assembly.GetType("NavalDLC.Settlements.Building.NavalBuildingTypes", throwOnError: false, ignoreCase: true);
				if (NavalBuildingTypesType != null)
				{
					NavalDLCExtensions.SettlementShipyardProp = AccessTools.Property(NavalBuildingTypesType, "SettlementShipyard");
				}
				NavalDLCExtensionsType = assembly.GetType("NavalDLC.NavalDLCExtensions", throwOnError: false, ignoreCase: true);
				if (NavalDLCExtensionsType != null)
				{
					NavalDLCExtensions.GetShipyardMethod = AccessTools.Method(NavalDLCExtensionsType, "GetShipyard");
					NavalDLCExtensions.GetAvailableShipUpgradePiecesMethod = AccessTools.Method(NavalDLCExtensionsType, "GetAvailableShipUpgradePieces");
				}
			}
		}

		private static void PostGetDistance(ref float __result, ref MapDistanceModel __instance, Settlement fromSettlement, Settlement toSettlement, bool isFromPort, bool isTargetingPort, NavigationType navigationCapability)
		{
			if (!fromSettlement.IsOverwritten(out OverwriteSettlementItem _) && !toSettlement.IsOverwritten(out OverwriteSettlementItem _) && !fromSettlement.IsPlayerBuilt() && !toSettlement.IsPlayerBuilt())
			{
			}
		}

		private static bool GetDistance(ref float __result, ref MapDistanceModel __instance, Settlement fromSettlement, Settlement toSettlement, bool isFromPort, bool isTargetingPort, NavigationType navigationCapability)
		{
			if (fromSettlement.IsOverwritten(out OverwriteSettlementItem _))
			{
				return true;
			}
			if (toSettlement.IsOverwritten(out OverwriteSettlementItem _))
			{
				return true;
			}
			if (fromSettlement.IsPlayerBuilt())
			{
				return true;
			}
			if (toSettlement.IsPlayerBuilt())
			{
				return true;
			}
			return true;
		}

		public static Exception FixGetDistanceExceptions(ref float __result, object __exception, ref MapDistanceModel __instance, Settlement fromSettlement, Settlement toSettlement, bool isFromPort, bool isTargetingPort, NavigationType navigationCapability)
		{
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Expected I4, but got Unknown
			if (__exception != null)
			{
				if (__exception != null)
				{
					LogManager.Log.NotifyBad($"Failed for from: {fromSettlement} (HasPort: {fromSettlement.HasPort}, isFromPort: {isFromPort}), to: {toSettlement} (HasPort: {toSettlement.HasPort}, isTargetingPort: {isTargetingPort}), navigationCapability: {(int)navigationCapability} - {((object)(NavigationType)(ref navigationCapability)).ToString()}, __result: {__result}");
					if (__exception is Exception e)
					{
						LogManager.Log.NotifyBad(e);
					}
					else
					{
						LogManager.Log.NotifyBad(__exception.ToString());
					}
					__result = float.MaxValue;
				}
			}
			return null;
		}

		private static bool GetShipTradeValue(ShipCostModel __instance, Ship ship, PartyBase seller, PartyBase buyer)
		{
			if (((ship != null) ? ship.Owner : null) == null || ((ship != null) ? ship.Name : null) == (TextObject)null)
			{
				return false;
			}
			return true;
		}

		private static bool GetShipBaseValue(Ship ship, bool applyAiDiscount, PartyBase owner)
		{
			if (((ship != null) ? ship.Owner : null) == null || ((ship != null) ? ship.Name : null) == (TextObject)null)
			{
				return false;
			}
			return true;
		}

		public static Exception FixExceptions(object __exception, ref object __instance)
		{
			if (__exception != null)
			{
				if (__exception != null)
				{
					if (__exception is Exception e)
					{
						LogManager.Log.NotifyBad(e);
					}
					else
					{
						LogManager.Log.NotifyBad(__exception.ToString());
					}
				}
			}
			return null;
		}

		private static bool TryPurchasingShipFromTown(object __instance, MobileParty mobileParty, Town town)
		{
			if (town.AvailableShips != null)
			{
				foreach (Ship item in (List<Ship>)(object)town.AvailableShips)
				{
					if (item.Owner == null)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		private static List<Settlement> CollectPlayerBuiltPorts()
		{
			List<Settlement> list = new List<Settlement>();
			if (PlayerSettlementInfo.Instance == null)
			{
				return list;
			}
			list.AddRange((PlayerSettlementInfo.Instance.Towns?.Select((PlayerSettlementItem t) => t.Settlement) ?? new List<Settlement>()).Where((Settlement t) => t.HasPort));
			list.AddRange((PlayerSettlementInfo.Instance.OverwriteSettlements?.Select((OverwriteSettlementItem t) => t.Settlement) ?? new List<Settlement>()).Where((Settlement t) => t.IsTown && !t.IsCastle && t.HasPort));
			return list.ToList();
		}
	}
	internal static class NavalDLCExtensions
	{
		internal static MethodInfo GetShipyardMethod;

		internal static MethodInfo GetAvailableShipUpgradePiecesMethod;

		internal static PropertyInfo SettlementShipyardProp;

		internal static MethodInfo SetPieceAtSlotMethod;

		internal static BuildingType SettlementShipyard => (BuildingType)((SettlementShipyardProp != null) ? /*isinst with value type is only supported in some contexts*/: null);

		public static Building GetShipyard(this Town town)
		{
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Expected O, but got Unknown
			Building val = null;
			try
			{
				if (GetShipyardMethod != null)
				{
					object obj = GetShipyardMethod.Invoke(null, new object[1] { town });
					val = (Building)((obj is Building) ? obj : null);
				}
				if (val == null)
				{
					val = ((IEnumerable<Building>)town.Buildings).FirstOrDefault((Building b) => b.BuildingType == SettlementShipyard);
				}
				if (val == null && SettlementShipyard != null)
				{
					val = new Building(SettlementShipyard, town, 0f, SettlementShipyard.StartLevel);
					((List<Building>)(object)town.Buildings).Add(val);
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return val;
		}

		public static List<ShipUpgradePiece> GetAvailableShipUpgradePieces(this Town town)
		{
			if (GetAvailableShipUpgradePiecesMethod != null)
			{
				return GetAvailableShipUpgradePiecesMethod.Invoke(null, new object[1] { town }) as List<ShipUpgradePiece>;
			}
			return new List<ShipUpgradePiece>();
		}

		public static void SetPieceAtSlot(this Ship ship, string slotTag, ShipUpgradePiece upgradePiece)
		{
			if (SetPieceAtSlotMethod == null)
			{
				SetPieceAtSlotMethod = AccessTools.Method(typeof(Ship), "SetPieceAtSlot");
			}
			if (SetPieceAtSlotMethod != null)
			{
				SetPieceAtSlotMethod.Invoke(ship, new object[2] { slotTag, upgradePiece });
			}
		}
	}
}
namespace BannerlordPlayerSettlement.Patches.Compatibility.Interfaces
{
	public interface ICompatibilityPatch
	{
		void PatchSubmoduleLoad(Harmony harmony);

		void PatchAfterMenus(Harmony harmony);

		void AddBehaviors(CampaignGameStarter gameInitializer);
	}
}
namespace BannerlordPlayerSettlement.HotKeys
{
	public class BasicHotKey : HotKeyBase
	{
		public BasicHotKey(string displayName, string description, InputKey defaultKey, string category, string uniqueness)
			: base("BasicHotKey" + ((object)(InputKey)(ref defaultKey)).ToString() + uniqueness, displayName, description, defaultKey, category)
		{
		}//IL_001c: Unknown result type (might be due to invalid IL or missing references)

	}
	public class ModifierKey : HotKeyBase
	{
		private static int count;

		public ModifierKey(string displayName, string description, InputKey defaultKey, string category)
			: base("ModifierKey" + ((object)(InputKey)(ref defaultKey)).ToString() + count, displayName, description, defaultKey, category)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			count++;
		}
	}
}
namespace BannerlordPlayerSettlement.Extensions
{
	public static class CampaignExtensions
	{
		public static CampaignBehaviorManager? GetCampaignBehaviorManager(this Campaign campaign)
		{
			if (campaign == null)
			{
				return null;
			}
			object value = AccessTools.Field(typeof(Campaign), "_campaignBehaviorManager").GetValue(campaign);
			return (CampaignBehaviorManager?)((value is CampaignBehaviorManager) ? value : null);
		}

		public static object? GetCampaignBehaviorDataStore(this Campaign campaign)
		{
			return campaign.GetCampaignBehaviorManager()?.GetCampaignBehaviorDataStore();
		}

		public static object? GetCampaignBehaviorDataStore(this CampaignBehaviorManager manager)
		{
			if (manager == null)
			{
				return null;
			}
			return AccessTools.Field(typeof(CampaignBehaviorManager), "_campaignBehaviorDataStore").GetValue(manager);
		}

		public static IDataStore? GetStore(this Campaign campaign, CampaignBehaviorBase campaignBehavior)
		{
			return campaign.GetCampaignBehaviorManager()?.GetStore(campaignBehavior);
		}

		public static IDataStore? GetStore(this CampaignBehaviorManager manager, CampaignBehaviorBase campaignBehavior)
		{
			try
			{
				object campaignBehaviorDataStore = manager.GetCampaignBehaviorDataStore();
				if (campaignBehaviorDataStore == null)
				{
					return null;
				}
				if (!(AccessTools.Field(campaignBehaviorDataStore.GetType(), "_behaviorDict").GetValue(campaignBehaviorDataStore) is IDictionary dictionary))
				{
					return null;
				}
				string stringId = campaignBehavior.StringId;
				if (dictionary.Contains(stringId))
				{
					object obj = dictionary[stringId];
					return (IDataStore?)((obj is IDataStore) ? obj : null);
				}
				List<KeyValuePair<string, IDataStore>> list = new List<KeyValuePair<string, IDataStore>>();
				foreach (DictionaryEntry item in dictionary)
				{
					string key = item.Key as string;
					object value = item.Value;
					list.Add(new KeyValuePair<string, IDataStore>(key, (IDataStore)((value is IDataStore) ? value : null)));
				}
				string name = ((object)campaignBehavior).GetType().Name;
				foreach (KeyValuePair<string, IDataStore> item2 in list)
				{
					if (!item2.Key.Contains(name))
					{
						continue;
					}
					dictionary.Remove(item2.Key);
					dictionary.Add(stringId, item2.Value);
					return item2.Value;
				}
			}
			catch (Exception e)
			{
				LogManager.Log.NotifyBad(e);
			}
			return null;
		}

		public static void SetCampaignGameLoadingType(this Campaign instance, GameLoadingType gameLoadingType)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			FieldInfo fieldInfo = AccessTools.Field(typeof(Campaign), "_gameLoadingType");
			fieldInfo.SetValue(instance, gameLoadingType);
		}

		public static T AsCampaignGameLoadingType<T>(this Campaign instance, GameLoadingType gameLoadingType, Func<T> func)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			FieldInfo fieldInfo = AccessTools.Field(typeof(Campaign), "_gameLoadingType");
			object value = fieldInfo.GetValue(instance);
			try
			{
				fieldInfo.SetValue(instance, gameLoadingType);
				return func();
			}
			finally
			{
				fieldInfo.SetValue(instance, value);
			}
		}
	}
	public static class CraftingCampaignBehaviorExtensions
	{
		private static FieldInfo _craftingOrdersField = AccessTools.Field(typeof(CraftingCampaignBehavior), "_craftingOrders");

		public static bool AddTown(this CraftingCampaignBehavior craftingCampaignBehavior, Town town, out Dictionary<Town, CraftingOrderSlots>? _craftingOrders)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			_craftingOrders = null;
			try
			{
				_craftingOrders = _craftingOrdersField.GetValue(craftingCampaignBehavior) as Dictionary<Town, CraftingOrderSlots>;
				if (_craftingOrders == null)
				{
					return false;
				}
				if (_craftingOrders.ContainsKey(town))
				{
					return true;
				}
				_craftingOrders[town] = new CraftingOrderSlots();
				MBList<Hero> val = new MBList<Hero>();
				Settlement settlement = ((SettlementComponent)town).Settlement;
				((List<Hero>)(object)val).AddRange((IEnumerable<Hero>)settlement.HeroesWithoutParty);
				foreach (MobileParty item in (List<MobileParty>)(object)settlement.Parties)
				{
					if (item.LeaderHero != null && !item.IsMainParty)
					{
						((List<Hero>)(object)val).Add(item.LeaderHero);
					}
				}
				if (((List<Hero>)(object)val).Count > 0)
				{
					for (int i = 0; i < 6; i++)
					{
						if (craftingCampaignBehavior.CraftingOrders[settlement.Town].GetAvailableSlot() > -1)
						{
							craftingCampaignBehavior.CreateTownOrder(Extensions.GetRandomElement<Hero>(val), i);
						}
					}
				}
				((List<Hero>)(object)val).Clear();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
	public static class GameEntityExtensions
	{
		public static void ClearEntity(this GameEntity? entity)
		{
			if (!(entity != (GameEntity)null))
			{
				return;
			}
			try
			{
				try
				{
					MapScreen.VisualsOfEntities.Remove(((NativeObject)entity).Pointer);
				}
				catch (Exception e)
				{
					LogManager.Log.NotifyBad(e);
				}
				foreach (GameEntity item in entity.GetChildren().ToList())
				{
					try
					{
						MapScreen.VisualsOfEntities.Remove(((NativeObject)item).Pointer);
						item.Remove(112);
					}
					catch (Exception e2)
					{
						LogManager.Log.NotifyBad(e2);
					}
				}
				try
				{
					entity.ClearEntityComponents(true, true, true);
					entity.ClearOnlyOwnComponents();
					entity.ClearComponents();
				}
				catch (Exception e3)
				{
					LogManager.Log.NotifyBad(e3);
				}
				entity.Remove(112);
			}
			catch (Exception e4)
			{
				LogManager.Log.NotifyBad(e4);
			}
		}
	}
	public static class HotKeyBaseExtensions
	{
		public static InputKey GetInputKey(this HotKeyBase hotKeyBase)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			GameKey val = HotKeyBase.op_Implicit(hotKeyBase);
			return val.KeyboardKey.InputKey;
		}
	}
	public static class InputContextExtensions
	{
		public static InputKey GetKey(string toUse, InputKey defaultKey)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			InputKey result;
			try
			{
				toUse = ((toUse.Length == 1) ? toUse.ToUpper() : toUse);
				result = (InputKey)Enum.Parse(typeof(InputKey), toUse);
			}
			catch (Exception)
			{
				return defaultKey;
			}
			return result;
		}

		public static InputKey GetKeyFrom(this IInputContext inputContext, string toUse, InputKey defaultKey)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return GetKey(toUse, defaultKey);
		}

		public static InputKey GetKeyFrom(this IInputManager inputMan, string toUse, InputKey defaultKey)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return GetKey(toUse, defaultKey);
		}
	}
	public static class RecruitmentCampaignBehaviorExtensions
	{
		private static FastInvokeHandler UpdateCurrentMercenaryTroopAndCountMethod = MethodInvoker.GetHandler(AccessTools.Method(typeof(RecruitmentCampaignBehavior), "UpdateCurrentMercenaryTroopAndCount"));

		private static FastInvokeHandler UpdateVolunteersOfNotablesInSettlementMethod = MethodInvoker.GetHandler(AccessTools.Method(typeof(RecruitmentCampaignBehavior), "UpdateVolunteersOfNotablesInSettlement"));

		public static void NewSettlementBuilt(this RecruitmentCampaignBehavior recruitmentCampaignBehavior, Settlement settlement)
		{
			if (settlement.IsTown)
			{
				UpdateCurrentMercenaryTroopAndCountMethod(recruitmentCampaignBehavior, settlement.Town, true);
			}
			UpdateVolunteersOfNotablesInSettlementMethod(recruitmentCampaignBehavior, settlement);
		}
	}
	public static class SettlementExtensions
	{
		private static MethodInfo GatePositionSetter = AccessTools.Property(typeof(Settlement), "GatePosition").SetMethod;

		private static MethodInfo BoundSetter = AccessTools.Property(typeof(Village), "Bound").SetMethod;

		public static void SetName(this Settlement settlement, TextObject name)
		{
			AccessTools.Field(typeof(Settlement), "_name").SetValue(settlement, name);
		}

		public static void SetBound(this Settlement settlement, Settlement boundTarget)
		{
			settlement.Village.SetBound(boundTarget);
		}

		public static void SetBound(this Village village, Settlement boundTarget)
		{
			BoundSetter.Invoke(village, new object[1] { boundTarget });
		}

		public static bool IsPlayerBuilt(this Settlement? settlement)
		{
			return ((settlement == null) ? null : ((MBObjectBase)settlement).StringId?.IsPlayerBuiltStringId()).GetValueOrDefault();
		}

		public static bool IsOverwritten(this Settlement? settlement, out OverwriteSettlementItem? overwriteSettlementItem)
		{
			overwriteSettlementItem = null;
			return ((settlement == null) ? null : ((MBObjectBase)settlement).StringId?.IsOverwritten(out overwriteSettlementItem)).GetValueOrDefault();
		}

		public static bool IsPlayerBuilt(this Settlement? settlement, out PlayerSettlementItem? playerSettlementItem)
		{
			playerSettlementItem = null;
			return ((settlement == null) ? null : ((MBObjectBase)settlement).StringId?.IsPlayerBuiltStringId(out playerSettlementItem)).GetValueOrDefault();
		}

		public static bool IsPlayerBuiltStringId(this string? stringId)
		{
			PlayerSettlementItem playerSettlementItem;
			return stringId.IsPlayerBuiltStringId(out playerSettlementItem);
		}

		public static bool IsPlayerBuiltStringId(this string? stringId, out PlayerSettlementItem? playerSettlementItem)
		{
			string stringId2 = stringId;
			if (string.IsNullOrEmpty(stringId2))
			{
				playerSettlementItem = null;
				return false;
			}
			if (PlayerSettlementInfo.Instance != null)
			{
				PlayerSettlementItem playerSettlementItem2 = PlayerSettlementInfo.Instance.Towns.FirstOrDefault(delegate(PlayerSettlementItem t)
				{
					Settlement? settlement5 = t.Settlement;
					return ((settlement5 != null) ? ((MBObjectBase)settlement5).StringId : null) == stringId2 || t.StringId == stringId2;
				});
				if (playerSettlementItem2 != null)
				{
					playerSettlementItem = playerSettlementItem2;
					return true;
				}
				PlayerSettlementItem playerSettlementItem3 = PlayerSettlementInfo.Instance.Castles.FirstOrDefault(delegate(PlayerSettlementItem t)
				{
					Settlement? settlement4 = t.Settlement;
					return ((settlement4 != null) ? ((MBObjectBase)settlement4).StringId : null) == stringId2 || t.StringId == stringId2;
				});
				if (playerSettlementItem3 != null)
				{
					playerSettlementItem = playerSettlementItem3;
					return true;
				}
				PlayerSettlementItem playerSettlementItem4 = PlayerSettlementInfo.Instance.PlayerVillages?.FirstOrDefault(delegate(PlayerSettlementItem v)
				{
					Settlement? settlement3 = v.Settlement;
					return ((settlement3 != null) ? ((MBObjectBase)settlement3).StringId : null) == stringId2 || v.StringId == stringId2;
				}) ?? PlayerSettlementInfo.Instance.Towns.SelectMany((PlayerSettlementItem t) => t.Villages).FirstOrDefault(delegate(PlayerSettlementItem v)
				{
					Settlement? settlement2 = v.Settlement;
					return ((settlement2 != null) ? ((MBObjectBase)settlement2).StringId : null) == stringId2 || v.StringId == stringId2;
				}) ?? PlayerSettlementInfo.Instance.Castles.SelectMany((PlayerSettlementItem c) => c.Villages).FirstOrDefault(delegate(PlayerSettlementItem v)
				{
					Settlement? settlement = v.Settlement;
					return ((settlement != null) ? ((MBObjectBase)settlement).StringId : null) == stringId2 || v.StringId == stringId2;
				});
				if (playerSettlementItem4 != null)
				{
					playerSettlementItem = playerSettlementItem4;
					return true;
				}
			}
			playerSettlementItem = null;
			return stringId2.StartsWith("player_settlement_town_") || stringId2.StartsWith("player_settlement_castle_");
		}

		public static bool IsOverwritten(this string? stringId, out OverwriteSettlementItem? overwriteSettlementItem)
		{
			string stringId2 = stringId;
			if (string.IsNullOrEmpty(stringId2))
			{
				overwriteSettlementItem = null;
				return false;
			}
			if (PlayerSettlementInfo.Instance != null)
			{
				if (PlayerSettlementInfo.Instance.OverwriteSettlements == null)
				{
					PlayerSettlementInfo.Instance.OverwriteSettlements = new List<OverwriteSettlementItem>();
				}
				OverwriteSettlementItem overwriteSettlementItem2 = PlayerSettlementInfo.Instance.OverwriteSettlements.FirstOrDefault(delegate(OverwriteSettlementItem t)
				{
					Settlement? settlement = t.Settlement;
					return ((settlement != null) ? ((MBObjectBase)settlement).StringId : null) == stringId2 || t.StringId == stringId2;
				});
				if (overwriteSettlementItem2 != null)
				{
					overwriteSettlementItem = overwriteSettlementItem2;
					return true;
				}
			}
			overwriteSettlementItem = null;
			return false;
		}

		public static SettlementType GetSettlementType(this Settlement? settlement)
		{
			if (settlement == null)
			{
				return SettlementType.None;
			}
			if (settlement.IsVillage)
			{
				return SettlementType.Village;
			}
			if (settlement.IsCastle)
			{
				return SettlementType.Castle;
			}
			if (settlement.IsTown)
			{
				return SettlementType.Town;
			}
			return SettlementType.None;
		}
	}
	public static class StringExtensions
	{
		public static string Base64Encode(this string plainText, bool silent = true)
		{
			try
			{
				byte[] bytes = Encoding.UTF8.GetBytes(plainText);
				return Convert.ToBase64String(bytes);
			}
			catch (Exception)
			{
				if (silent)
				{
					return plainText;
				}
				throw;
			}
		}

		public static string Base64Decode(this string base64EncodedData, bool silent = true)
		{
			try
			{
				byte[] bytes = Convert.FromBase64String(base64EncodedData);
				return Encoding.UTF8.GetString(bytes);
			}
			catch (Exception)
			{
				if (silent)
				{
					return base64EncodedData;
				}
				throw;
			}
		}
	}
	public static class TextObjectExtensions
	{
		public static TextObject AsEmpty(this TextObject? t)
		{
			if (t != (TextObject)null)
			{
				t.Value = "";
				t.Attributes?.Clear();
				return t;
			}
			return TextObjectCompat.Empty;
		}
	}
	public static class TextObjectCompat
	{
		public static readonly TextObject Empty = new TextObject("", (Dictionary<string, object>)null);
	}
	public static class TournamentCampaignBehaviorExtensions
	{
		private static FastInvokeHandler ConsiderStartOrEndTournamentMethod = MethodInvoker.GetHandler(AccessTools.Method(typeof(TournamentCampaignBehavior), "ConsiderStartOrEndTournament"));

		public static void NewTownBuilt(this TournamentCampaignBehavior tournamentCampaignBehavior, Town town)
		{
			ConsiderStartOrEndTournamentMethod(tournamentCampaignBehavior, town);
		}
	}
	public static class VillageGoodProductionCampaignBehaviorExtensions
	{
		private static FastInvokeHandler TickProductionsMethod = MethodInvoker.GetHandler(AccessTools.Method(typeof(VillageGoodProductionCampaignBehavior), "TickProductions"));

		private static FastInvokeHandler SetTradeBoundMethod = MethodInvoker.GetHandler(AccessTools.Property(typeof(Village), "TradeBound").SetMethod);

		public static void SetTradeBound(this Village village, Settlement? tradeBound)
		{
			SetTradeBoundMethod(village, tradeBound);
		}

		public static void NewVillageBuilt(this VillageGoodProductionCampaignBehavior villageGoodProductionCampaignBehavior, Village village)
		{
			bool flag = true;
			TickProductionsMethod(villageGoodProductionCampaignBehavior, ((SettlementComponent)village).Settlement, flag);
			TryToAssignTradeBoundForVillage(village);
		}

		private static void TryToAssignTradeBoundForVillage(Village village)
		{
			Settlement val = SettlementHelper.FindNearestSettlementToSettlement(((SettlementComponent)village).Settlement, (NavigationType)1, (Func<Settlement, bool>)((Settlement x) => x.IsTown && ((SettlementComponent)x.Town).MapFaction == ((SettlementComponent)village).Settlement.MapFaction));
			if (val != null && Campaign.Current.Models.MapDistanceModel.GetDistance(val, ((SettlementComponent)village).Settlement, false, false, (NavigationType)1) < 150f)
			{
				village.SetTradeBound(val);
				return;
			}
			Settlement val2 = SettlementHelper.FindNearestSettlementToSettlement(((SettlementComponent)village).Settlement, (NavigationType)1, (Func<Settlement, bool>)((Settlement x) => x.IsTown && ((SettlementComponent)x.Town).MapFaction != ((SettlementComponent)village).Settlement.MapFaction && !((SettlementComponent)x.Town).MapFaction.IsAtWarWith(((SettlementComponent)village).Settlement.MapFaction) && Campaign.Current.Models.MapDistanceModel.GetDistance(x, ((SettlementComponent)village).Settlement, false, false, (NavigationType)1) <= 150f));
			if (val2 != null && Campaign.Current.Models.MapDistanceModel.GetDistance(val2, ((SettlementComponent)village).Settlement, false, false, (NavigationType)1) < 150f)
			{
				village.SetTradeBound(val);
			}
			else
			{
				village.SetTradeBound(null);
			}
		}
	}
	public static class WorkshopsCampaignBehaviorExtensions
	{
		private static FastInvokeHandler BuildArtisanWorkshopMethod = MethodInvoker.GetHandler(AccessTools.Method(typeof(WorkshopsCampaignBehavior), "BuildArtisanWorkshop"));

		private static FastInvokeHandler BuildWorkshopForHeroAtGameStartMethod = MethodInvoker.GetHandler(AccessTools.Method(typeof(WorkshopsCampaignBehavior), "BuildWorkshopForHeroAtGameStart"));

		public static void NewTownBuilt(this WorkshopsCampaignBehavior behavior, Town town)
		{
			town.InitializeWorkshops(Campaign.Current.Models.WorkshopModel.DefaultWorkshopCountInSettlement);
			BuildArtisanWorkshopMethod(behavior, town);
			for (int i = 1; i < town.Workshops.Length; i++)
			{
				Hero notableOwnerForWorkshop = Campaign.Current.Models.WorkshopModel.GetNotableOwnerForWorkshop(town.Workshops[i]);
				if (notableOwnerForWorkshop != null)
				{
					BuildWorkshopForHeroAtGameStartMethod(behavior, notableOwnerForWorkshop);
				}
			}
		}
	}
}
namespace BannerlordPlayerSettlement.Descriptors
{
	public class CultureSettlementTemplate
	{
		public string FromModule;

		public string TemplateModifier;

		public XmlDocument Document;

		public string CultureId;
	}
	public class PlayerSettlementItemTemplate
	{
		public XmlNode ItemXML;

		public string Id;

		public string Culture;

		public int Type = 0;
	}
}
namespace BannerlordPlayerSettlement.Behaviours
{
	public class PlayerSettlementBehaviour : CampaignBehaviorBase
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class <>c
		{
			public static readonly <>c <>9 = new <>c();

			public static Func<SettlementMetaV3, string> <>9__76_0;

			public static OnConditionDelegate <>9__79_0;

			public static OnConsequenceDelegate <>9__79_1;

			public static OnClickableConditionDelegate <>9__79_2;

			public static Func<DeepTransformEdit, bool> <>9__104_2;

			public static Func<DeepTransformEdit, bool> <>9__104_0;

			public static Func<DeepTransformEdit, bool> <>9__111_0;

			public static Func<CultureObject, InquiryElement> <>9__122_3;

			public static Func<XmlNode, bool> <>9__123_9;

			public static Func<CultureObject, InquiryElement> <>9__123_3;

			public static Func<CampaignBehaviorBase, bool> <>9__124_6;

			public static Func<CultureObject, InquiryElement> <>9__124_4;

			public static Func<Building, int> <>9__126_3;

			public static Func<Building, bool> <>9__126_0;

			public static Func<Building, bool> <>9__126_1;

			public static Func<Settlement, bool> <>9__128_0;

			public static Func<Settlement, InquiryElement> <>9__128_1;

			public static Func<CampaignBehaviorBase, bool> <>9__129_6;

			public static Func<CampaignBehaviorBase, bool> <>9__129_7;

			public static Func<CultureObject, InquiryElement> <>9__129_4;

			public static Func<CampaignBehaviorBase, bool> <>9__136_8;

			public static Func<Hero, bool> <>9__136_9;

			public static Func<CampaignBehaviorBase, bool> <>9__136_6;

			public static Func<CampaignBehaviorBase, bool> <>9__136_7;

			public static Func<CultureObject, InquiryElement> <>9__136_4;

			public static Func<Building, int> <>9__140_3;

			public static Func<Building, bool> <>9__140_0;

			public static Func<Building, bool> <>9__140_1;

			public static Func<PlayerSettlementItemTemplate, int, string> <>9__144_0;

			public static Action<bool, string> <>9__144_4;

			public static Action <>9__144_3;

			public static Func<VillageType, bool> <>9__147_0;

			public static Func<Settlement, bool> <>9__150_0;

			public static Func<Settlement, bool> <>9__153_0;

			internal string <SyncData>b__76_0(SettlementMetaV3 o)
			{
				return o.StringId;
			}

			internal bool <SetupConversationDialogues>b__79_0()
			{
				return conv_rebuild_condition();
			}

			internal void <SetupConversationDialogues>b__79_1()
			{
				conv_rebuild_consequence();
			}

			internal bool <SetupConversationDialogues>b__79_2(out TextObject t)
			{
				return conv_rebuild_clickable(out t);
			}

			internal bool <OnBeforeTick>b__104_2(DeepTransformEdit d)
			{
				return d.IsDeleted && d.Index >= 0;
			}

			internal bool <OnBeforeTick>b__104_0(DeepTransformEdit dte)
			{
				return dte.Index < 0;
			}

			internal bool <HideDeletedDeepEdits>b__111_0(DeepTransformEdit dte)
			{
				return dte.IsDeleted && dte.Index >= 0;
			}

			internal InquiryElement <Overwrite>b__122_3(CultureObject c)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Expected O, but got Unknown
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Expected O, but got Unknown
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Expected O, but got Unknown
				return new InquiryElement((object)c, ((object)((BasicCultureObject)c).Name).ToString(), (ImageIdentifier)new BannerImageIdentifier(new Banner(((BasicCultureObject)c).Banner), false), true, ((object)((BasicCultureObject)c).Name).ToString());
			}

			internal bool <Rebuild>b__123_9(XmlNode n)
			{
				return !(n is XmlComment);
			}

			internal InquiryElement <Rebuild>b__123_3(CultureObject c)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Expected O, but got Unknown
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Expected O, but got Unknown
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Expected O, but got Unknown
				return new InquiryElement((object)c, ((object)((BasicCultureObject)c).Name).ToString(), (ImageIdentifier)new BannerImageIdentifier(new Banner(((BasicCultureObject)c).Banner), false), true, ((object)((BasicCultureObject)c).Name).ToString());
			}

			internal bool <BuildCastle>b__124_6(CampaignBehaviorBase b)
			{
				return b is RecruitmentCampaignBehavior;
			}

			internal InquiryElement <BuildCastle>b__124_4(CultureObject c)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Expected O, but got Unknown
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Expected O, but got Unknown
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Expected O, but got Unknown
				return new InquiryElement((object)c, ((object)((BasicCultureObject)c).Name).ToString(), (ImageIdentifier)new BannerImageIdentifier(new Banner(((BasicCultureObject)c).Banner), false), true, ((object)((BasicCultureObject)c).Name).ToString());
			}

			internal int <InitCastleBuildings>b__126_3(Building k)
			{
				return k.CurrentLevel;
			}

			internal bool <InitCastleBuildings>b__126_0(Building b)
			{
				return b.BuildingType.IsDailyProject && b.IsCurrentlyDefault;
			}

			internal bool <InitCastleBuildings>b__126_1(Building b)
			{
				return b.BuildingType.IsDailyProject;
			}

			internal bool <BuildVillage>b__128_0(Settlement s)
			{
				return s != null;
			}

			internal InquiryElement <BuildVillage>b__128_1(Settlement c)
			{
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Expected O, but got Unknown
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Expected O, but got Unknown
				return new InquiryElement((object)c, ((object)c.Name).ToString(), (ImageIdentifier)new CharacterImageIdentifier(CharacterCode.CreateFrom((BasicCharacterObject)(object)((c.IsTown || c.IsCastle) ? (c.Town.Governor ?? Hero.MainHero) : Hero.MainHero).CharacterObject)), true, ((object)(c.EncyclopediaText ?? c.Name)).ToString());
			}

			internal bool <BuildVillageFor>b__129_6(CampaignBehaviorBase b)
			{
				return b is VillageGoodProductionCampaignBehavior;
			}

			internal bool <BuildVillageFor>b__129_7(CampaignBehaviorBase b)
			{
				return b is RecruitmentCampaignBehavior;
			}

			internal InquiryElement <BuildVillageFor>b__129_4(CultureObject c)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Expected O, but got Unknown
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Expected O, but got Unknown
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Expected O, but got Unknown
				return new InquiryElement((object)c, ((object)((BasicCultureObject)c).Name).ToString(), (ImageIdentifier)new BannerImageIdentifier(new Banner(((BasicCultureObject)c).Banner), false), true, ((object)((BasicCultureObject)c).Name).ToString());
			}

			internal bool <BuildTown>b__136_8(CampaignBehaviorBase b)
			{
				return b is WorkshopsCampaignBehavior;
			}

			internal bool <BuildTown>b__136_9(Hero x)
			{
				return x.IsGangLeader;
			}

			internal bool <BuildTown>b__136_6(CampaignBehaviorBase b)
			{
				return b is CraftingCampaignBehavior;
			}

			internal bool <BuildTown>b__136_7(CampaignBehaviorBase b)
			{
				return b is RecruitmentCampaignBehavior;
			}

			internal InquiryElement <BuildTown>b__136_4(CultureObject c)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Expected O, but got Unknown
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Expected O, but got Unknown
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Expected O, but got Unknown
				return new InquiryElement((object)c, ((object)((BasicCultureObject)c).Name).ToString(), (ImageIdentifier)new BannerImageIdentifier(new Banner(((BasicCultureObject)c).Banner), false), true, ((object)((BasicCultureObject)c).Name).ToString());
			}

			internal int <InitTownBuildings>b__140_3(Building k)
			{
				return k.CurrentLevel;
			}

			internal bool <InitTownBuildings>b__140_0(Building b)
			{
				return b.BuildingType.IsDailyProject && b.IsCurrentlyDefault;
			}

			internal bool <InitTownBuildings>b__140_1(Building b)
			{
				return b.BuildingType.IsDailyProject;
			}

			internal string <UpdateSettlementVisualEntity>b__144_0(PlayerSettlementItemTemplate a, int idx)
			{
				return $"\t\t{a.Id} - Culture: '{a.Culture}', Type: '{a.Type}', Variant: '{idx + 1}'";
			}

			internal void <UpdateSettlementVisualEntity>b__144_4(bool b, string s)
			{
				Utilities.QuitGame();
			}

			internal void <UpdateSettlementVisualEntity>b__144_3()
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

			internal bool <GetVillageTypeInquiry>b__147_0(VillageType v)
			{
				return v != null;
			}

			internal bool <GetPotentialVillageBoundOwners>b__150_0(Settlement s)
			{
				return (s.IsTown && (((List<Village>)(object)s.BoundVillages)?.Count ?? 0) < Main.Settings.MaxVillagesPerTown) || (s.IsCastle && (((List<Village>)(object)s.BoundVillages)?.Count ?? 0) < Main.Settings.MaxVillagesPerCastle);
			}

			internal bool <CalculateVillageOwner>b__153_0(Settlement s)
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
			object obj = <>c.<>9__79_0;
			if (obj == null)
			{
				OnConditionDelegate val = () => conv_rebuild_condition();
				<>c.<>9__79_0 = val;
				obj = (object)val;
			}
			object obj2 = <>c.<>9__79_1;
			if (obj2 == null)
			{
				OnConsequenceDelegate val2 = delegate
				{
					conv_rebuild_consequence();
				};
				<>c.<>9__79_1 = val2;
				obj2 = (object)val2;
			}
			object obj3 = <>c.<>9__79_2;
			if (obj3 == null)
			{
				OnClickableConditionDelegate val3 = delegate(out TextObject t)
				{
					return conv_rebuild_clickable(out t);
				};
				<>c.<>9__79_2 = val3;
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
							if (!((CampaignTime)(ref buildEnd)).IsFuture)
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
							if (!((CampaignTime)(ref buildEnd)).IsFuture)
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
							if (!((CampaignTime)(ref buildEnd)).IsFuture)
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
								if (!((CampaignTime)(ref buildEnd)).IsFuture)
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
							if (!((CampaignTime)(ref buildEnd)).IsFuture)
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
								if (!((CampaignTime)(ref buildEnd)).IsFuture)
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
				frame.origin.z = val2.MapScene.GetWaterLevelAtPosition(((Vec3)(ref frame.origin)).AsVec2, true, false);
				val5 = new Vec3(0.25f, 0.25f, 0.25f, -1f);
				((MatrixFrame)(ref frame)).Scale(ref val5);
				MatrixFrame frame2 = ghostPortVisualEntity.GetFrame();
				portPlacementFrame = frame;
				SetFrame(ghostPortVisualEntity, ref frame);
				bool flag2 = ((PathFaceRecord)(ref nullFaceRecord)).IsValid() && !flag;
				CampaignVec2 val6 = default(CampaignVec2);
				((CampaignVec2)(ref val6))..ctor(((Vec3)(ref origin)).AsVec2, flag);
				flag2 = flag2 && NavigationHelper.IsPositionValidForNavigationType(val6, (NavigationType)2);
				if (flag2)
				{
					int[] invalidTerrainTypesForNavigationType = Campaign.Current.Models.PartyNavigationModel.GetInvalidTerrainTypesForNavigationType((NavigationType)2);
					if (invalidTerrainTypesForNavigationType.Contains(nullFaceRecord.FaceGroupIndex) || invalidTerrainTypesForNavigationType.Contains(((CampaignVec2)(ref val6)).Face.FaceGroupIndex))
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
				((MatrixFrame)(ref frame3)).Scale(ref val5);
				MatrixFrame frame4 = ghostGateVisualEntity.GetFrame();
				gatePlacementFrame = frame3;
				SetFrame(ghostGateVisualEntity, ref frame3);
				bool flag4 = ((PathFaceRecord)(ref nullFaceRecord2)).IsValid() && flag3 && nullFaceRecord2.FaceIslandIndex == MobileParty.MainParty.CurrentNavigationFace.FaceIslandIndex;
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
				((Mat3)(ref globalFrame.rotation)).RotateAboutUp(0f - num18);
				if (flag23)
				{
					float num19 = num16;
					((Mat3)(ref globalFrame.rotation)).RotateAboutSide(0f - num19);
				}
				else
				{
					float num20 = num16;
					((Mat3)(ref globalFrame.rotation)).RotateAboutForward(0f - num20);
				}
				val5 = new Vec3(deepEditScale, deepEditScale, deepEditScale, -1f);
				((MatrixFrame)(ref globalFrame)).Scale(ref val5);
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
					bool flag29 = ((PathFaceRecord)(ref nullFaceRecord3)).IsValid() && nullFaceRecord3.FaceIslandIndex == MobileParty.MainParty.CurrentNavigationFace.FaceIslandIndex;
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
				val.SetTextVariable("HELP_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)(InputKey)(ref val2)).ToString(), 1f));
				val2 = (InputKey)1;
				val.SetTextVariable("ESC_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)(InputKey)(ref val2)).ToString(), 1f));
				val2 = Main.Submodule.DeepEditApplyKey.GetInputKey();
				val.SetTextVariable("APPLY_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)(InputKey)(ref val2)).ToString(), 1f));
				val2 = Main.Submodule.DeepEditToggleKey.GetInputKey();
				val.SetTextVariable("DEEP_EDIT_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)(InputKey)(ref val2)).ToString(), 1f));
				val.SetTextVariable("CYCLE_MODIFIER_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.CycleModifierKey)).ToString(), 1f));
				val.SetTextVariable("CYCLE_BACK_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.CycleBackKey)).ToString(), 1f));
				val.SetTextVariable("CYCLE_NEXT_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.CycleNextKey)).ToString(), 1f));
				val.SetTextVariable("MOVE_UP_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.MoveUpKey)).ToString(), 1f));
				val.SetTextVariable("MOVE_DOWN_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)HotKeyBase.op_Implicit(Main.Submodule.MoveDownKey)).ToString(), 1f));
				val2 = (InputKey)224;
				val.SetTextVariable("MOUSE_CLICK", HyperlinkTexts.GetKeyHyperlinkText(((object)(InputKey)(ref val2)).ToString(), 1f));
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
					((Vec3)(ref origin))..ctor(frame.origin.x, frame.origin.y, 0f, -1f);
					Vec2 val = new Vec2(frame.origin.x, frame.origin.y);
					origin.z = scene.GetGroundHeightAtPosition(((Vec2)(ref val)).ToVec3(0f), (BodyFlags)544321929);
					entity.SetLocalPosition((frame.origin = origin) + (Vec3)(((??)offset) ?? Vec3.Zero));
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
				val.SetTextVariable("HELP_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)(InputKey)(ref val2)).ToString(), 1f));
				val2 = (InputKey)1;
				val.SetTextVariable("ESC_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)(InputKey)(ref val2)).ToString(), 1f));
				val2 = (InputKey)224;
				val.SetTextVariable("MOUSE_CLICK", HyperlinkTexts.GetKeyHyperlinkText(((object)(InputKey)(ref val2)).ToString(), 1f));
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
				val.SetTextVariable("HELP_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)(InputKey)(ref val2)).ToString(), 1f));
				val2 = (InputKey)1;
				val.SetTextVariable("ESC_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)(InputKey)(ref val2)).ToString(), 1f));
				val2 = (InputKey)224;
				val.SetTextVariable("MOUSE_CLICK", HyperlinkTexts.GetKeyHyperlinkText(((object)(InputKey)(ref val2)).ToString(), 1f));
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
					if (((CampaignTime)(ref buildEnd)).IsFuture)
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
						val3 = ((Vec3)(ref value.origin)).AsVec2;
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
							val4 = ((Vec3)(ref value.origin)).AsVec2;
							goto IL_046a;
						}
					}
					CampaignVec2 portPosition = target.PortPosition;
					val4 = ((CampaignVec2)(ref portPosition)).ToVec2();
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
					xmlAttribute.Value = ((Vec2)(ref val6)).X.ToString();
					xmlNode.Attributes.SetNamedItem(xmlAttribute);
				}
				else
				{
					xmlNode.Attributes["gate_posX"].Value = ((Vec2)(ref val6)).X.ToString();
				}
				if (xmlNode.Attributes["gate_posY"] == null)
				{
					XmlAttribute xmlAttribute2 = xmlNode.OwnerDocument.CreateAttribute("gate_posY");
					xmlAttribute2.Value = ((Vec2)(ref val6)).Y.ToString();
					xmlNode.Attributes.SetNamedItem(xmlAttribute2);
				}
				else
				{
					xmlNode.Attributes["gate_posY"].Value = ((Vec2)(ref val6)).Y.ToString();
				}
				goto IL_03f7;
				IL_046a:
				Vec2 val7 = val4;
				if (val7 != Vec2.Invalid)
				{
					if (xmlNode.Attributes["port_posX"] == null)
					{
						XmlAttribute xmlAttribute3 = xmlNode.OwnerDocument.CreateAttribute("port_posX");
						xmlAttribute3.Value = ((Vec2)(ref val7)).X.ToString();
						xmlNode.Attributes.SetNamedItem(xmlAttribute3);
					}
					else
					{
						xmlNode.Attributes["port_posX"].Value = ((Vec2)(ref val7)).X.ToString();
					}
					if (xmlNode.Attributes["port_posY"] == null)
					{
						XmlAttribute xmlAttribute4 = xmlNode.OwnerDocument.CreateAttribute("port_posY");
						xmlAttribute4.Value = ((Vec2)(ref val7)).Y.ToString();
						xmlNode.Attributes.SetNamedItem(xmlAttribute4);
					}
					else
					{
						xmlNode.Attributes["port_posY"].Value = ((Vec2)(ref val7)).Y.ToString();
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
				text = text.Replace("{{G_POS_X}}", ((Vec2)(ref val6)).X.ToString());
				text = text.Replace("{{G_POS_Y}}", ((Vec2)(ref val6)).Y.ToString());
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
				xmlNode.Attributes["posX"].Value = ((Vec2)(ref val9)).X.ToString();
				xmlNode.Attributes["posY"].Value = ((Vec2)(ref val9)).Y.ToString();
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
								val5 = ((Vec3)(ref value.origin)).AsVec2;
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
						val2 = ((Vec3)(ref value.origin)).AsVec2;
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
				text = text.Replace("{{G_POS_X}}", ((Vec2)(ref val4)).X.ToString());
				text = text.Replace("{{G_POS_Y}}", ((Vec2)(ref val4)).Y.ToString());
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
					xmlAttribute2.Value = ((Vec2)(ref val4)).X.ToString();
					xmlNode2.Attributes.SetNamedItem(xmlAttribute2);
				}
				else
				{
					xmlNode2.Attributes["gate_posX"].Value = ((Vec2)(ref val4)).X.ToString();
				}
				if (xmlNode2.Attributes["gate_posY"] == null)
				{
					XmlAttribute xmlAttribute3 = xmlNode2.OwnerDocument.CreateAttribute("gate_posY");
					xmlAttribute3.Value = ((Vec2)(ref val4)).Y.ToString();
					xmlNode2.Attributes.SetNamedItem(xmlAttribute3);
				}
				else
				{
					xmlNode2.Attributes["gate_posY"].Value = ((Vec2)(ref val4)).Y.ToString();
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
							val6 = ((Vec3)(ref value.origin)).AsVec2;
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
				xmlNode2.Attributes["posX"].Value = ((Vec2)(ref val7)).X.ToString();
				xmlNode2.Attributes["posY"].Value = ((Vec2)(ref val7)).Y.ToString();
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
								val5 = ((Vec3)(ref value.origin)).AsVec2;
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
						xmlAttribute4.Value = ((Vec2)(ref val8)).X.ToString();
						xmlNode2.Attributes.SetNamedItem(xmlAttribute4);
					}
					else
					{
						xmlNode2.Attributes["port_posX"].Value = ((Vec2)(ref val8)).X.ToString();
					}
					if (xmlNode2.Attributes["port_posY"] == null)
					{
						XmlAttribute xmlAttribute5 = xmlNode2.OwnerDocument.CreateAttribute("port_posY");
						xmlAttribute5.Value = ((Vec2)(ref val8)).Y.ToString();
						xmlNode2.Attributes.SetNamedItem(xmlAttribute5);
					}
					else
					{
						xmlNode2.Attributes["port_posY"].Value = ((Vec2)(ref val8)).Y.ToString();
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
					val = ((Vec3)(ref value.origin)).AsVec2;
					goto IL_0098;
				}
			}
			val = MobileParty.MainParty.GetPosition2D;
			goto IL_0098;
			IL_02a5:
			XmlNode xmlNode;
			Vec2 val2;
			xmlNode.Attributes["posX"].Value = ((Vec2)(ref val2)).X.ToString();
			xmlNode.Attributes["posY"].Value = ((Vec2)(ref val2)).Y.ToString();
			xmlNode.Attributes["name"].Value = settlementName;
			xmlNode.Attributes["owner"].Value = "Faction." + ((MBObjectBase)Hero.MainHero.Clan).StringId;
			xmlNode.Attributes["culture"].Value = "Culture." + ((MBObjectBase)culture).StringId;
			TextObject val3 = new TextObject("{=player_settlement_24}{SETTLEMENT_NAME} was founded by {HERO_NAME} of the {FACTION_TERM} on {BUILD_TIME}", (Dictionary<string, object>)null);
			string text;
			val3.SetTextVariable("SETTLEMENT_NAME", PlayerSettlementItem.EncyclopediaLinkWithName(text, new TextObject(settlementName, (Dictionary<string, object>)null)));
			val3.SetTextVariable("HERO_NAME", Hero.MainHero.EncyclopediaLinkWithName);
			val3.SetTextVariable("FACTION_TERM", Hero.MainHero.Clan.EncyclopediaLinkWithName);
			CampaignTime now = CampaignTime.Now;
			val3.SetTextVariable("BUILD_TIME", ((object)(CampaignTime)(ref now)).ToString());
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
			text2 = text2.Replace("{{G_POS_X}}", ((Vec2)(ref val4)).X.ToString());
			text2 = text2.Replace("{{G_POS_Y}}", ((Vec2)(ref val4)).Y.ToString());
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
				xmlAttribute2.Value = ((Vec2)(ref val4)).X.ToString();
				xmlNode.Attributes.SetNamedItem(xmlAttribute2);
			}
			else
			{
				xmlNode.Attributes["gate_posX"].Value = ((Vec2)(ref val4)).X.ToString();
			}
			if (xmlNode.Attributes["gate_posY"] == null)
			{
				XmlAttribute xmlAttribute3 = xmlNode.OwnerDocument.CreateAttribute("gate_posY");
				xmlAttribute3.Value = ((Vec2)(ref val4)).Y.ToString();
				xmlNode.Attributes.SetNamedItem(xmlAttribute3);
			}
			else
			{
				xmlNode.Attributes["gate_posY"].Value = ((Vec2)(ref val4)).Y.ToString();
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
						val5 = ((Vec3)(ref value.origin)).AsVec2;
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
					float resultNumber = ((ExplainedNumber)(ref val2)).ResultNumber;
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
					val = ((Vec3)(ref value.origin)).AsVec2;
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
			xmlNode.Attributes["posX"].Value = ((Vec2)(ref val2)).X.ToString();
			xmlNode.Attributes["posY"].Value = ((Vec2)(ref val2)).Y.ToString();
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
			val3.SetTextVariable("BUILD_TIME", ((object)(CampaignTime)(ref now)).ToString());
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
								num2 = ((Vec2)(ref getPosition2D)).Distance(settlement.GetPosition2D);
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
				val.SetTextVariable("HELP_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)(InputKey)(ref val2)).ToString(), 1f));
				val2 = (InputKey)1;
				val.SetTextVariable("ESC_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)(InputKey)(ref val2)).ToString(), 1f));
				val2 = (InputKey)224;
				val.SetTextVariable("MOUSE_CLICK", HyperlinkTexts.GetKeyHyperlinkText(((object)(InputKey)(ref val2)).ToString(), 1f));
				val2 = Main.Submodule.DeepEditToggleKey.GetInputKey();
				val.SetTextVariable("DEEP_EDIT_KEY", HyperlinkTexts.GetKeyHyperlinkText(((object)(InputKey)(ref val2)).ToString(), 1f));
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
					val = ((Vec3)(ref value.origin)).AsVec2;
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
						val2 = ((Vec3)(ref value.origin)).AsVec2;
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
			xmlNode.Attributes["posX"].Value = ((Vec2)(ref val3)).X.ToString();
			xmlNode.Attributes["posY"].Value = ((Vec2)(ref val3)).Y.ToString();
			xmlNode.Attributes["name"].Value = settlementName;
			xmlNode.Attributes["owner"].Value = "Faction." + ((MBObjectBase)Hero.MainHero.Clan).StringId;
			xmlNode.Attributes["culture"].Value = "Culture." + ((MBObjectBase)culture).StringId;
			TextObject val4 = new TextObject("{=player_settlement_24}{SETTLEMENT_NAME} was founded by {HERO_NAME} of the {FACTION_TERM} on {BUILD_TIME}", (Dictionary<string, object>)null);
			val4.SetTextVariable("SETTLEMENT_NAME", PlayerSettlementItem.EncyclopediaLinkWithName(text2, new TextObject(settlementName, (Dictionary<string, object>)null)));
			val4.SetTextVariable("HERO_NAME", Hero.MainHero.EncyclopediaLinkWithName);
			val4.SetTextVariable("FACTION_TERM", Hero.MainHero.Clan.EncyclopediaLinkWithName);
			CampaignTime now = CampaignTime.Now;
			val4.SetTextVariable("BUILD_TIME", ((object)(CampaignTime)(ref now)).ToString());
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
			text3 = text3.Replace("{{G_POS_X}}", ((Vec2)(ref val5)).X.ToString());
			text3 = text3.Replace("{{G_POS_Y}}", ((Vec2)(ref val5)).Y.ToString());
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
					xmlAttribute2.Value = ((Vec2)(ref val6)).X.ToString();
					xmlNode.Attributes.SetNamedItem(xmlAttribute2);
				}
				else
				{
					xmlNode.Attributes["port_posX"].Value = ((Vec2)(ref val6)).X.ToString();
				}
				if (xmlNode.Attributes["port_posY"] == null)
				{
					XmlAttribute xmlAttribute3 = xmlNode.OwnerDocument.CreateAttribute("port_posY");
					xmlAttribute3.Value = ((Vec2)(ref val6)).Y.ToString();
					xmlNode.Attributes.SetNamedItem(xmlAttribute3);
				}
				else
				{
					xmlNode.Attributes["port_posY"].Value = ((Vec2)(ref val6)).Y.ToString();
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
						val7 = ((Vec3)(ref value.origin)).AsVec2;
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
				xmlAttribute4.Value = ((Vec2)(ref val5)).X.ToString();
				xmlNode.Attributes.SetNamedItem(xmlAttribute4);
			}
			else
			{
				xmlNode.Attributes["gate_posX"].Value = ((Vec2)(ref val5)).X.ToString();
			}
			if (xmlNode.Attributes["gate_posY"] == null)
			{
				XmlAttribute xmlAttribute5 = xmlNode.OwnerDocument.CreateAttribute("gate_posY");
				xmlAttribute5.Value = ((Vec2)(ref val5)).Y.ToString();
				xmlNode.Attributes.SetNamedItem(xmlAttribute5);
			}
			else
			{
				xmlNode.Attributes["gate_posY"].Value = ((Vec2)(ref val5)).Y.ToString();
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
							val = ((Vec3)(ref value.origin)).AsVec2;
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
namespace System.Runtime.CompilerServices
{
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal static class IsExternalInit
	{
	}
}
namespace PlayerSettlementFixes
{
	public static class PlayerSettlementFixesLoader
	{
		private static Harmony _harmony;

		public static void Initialize(Harmony harmony)
		{
			_harmony = harmony;
			try
			{
				harmony.PatchAll(typeof(PlayerSettlementFixesLoader).Assembly);
				SiegeEnginesContainerRuntimeFix.Apply(harmony);
			}
			catch (Exception ex)
			{
				LogError("PlayerSettlementFixes failed to initialize: " + ex);
			}
		}

		public static void EnsureSiegeFixApplied()
		{
			if (_harmony != null)
			{
				SiegeEnginesContainerRuntimeFix.Apply(_harmony);
			}
		}

		public static void LogError(string msg)
		{
			try
			{
				string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mount and Blade II Bannerlord", "CrashHandler");
				Directory.CreateDirectory(text);
				File.AppendAllText(Path.Combine(text, "player_settlement_fixes.log"), "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + msg + Environment.NewLine);
			}
			catch
			{
			}
			try
			{
				object obj2 = (AccessTools.TypeByName("BannerlordPlayerSettlement.Utils.LogManager")?.GetField("Log", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))?.GetValue(null);
				obj2?.GetType().GetMethod("ToFile", new Type[2]
				{
					typeof(string),
					typeof(bool)
				})?.Invoke(obj2, new object[2]
				{
					"[PlayerSettlementFixes] " + msg,
					false
				});
			}
			catch
			{
			}
		}
	}
	[HarmonyPatch]
	public static class SiegeFixBootstrap
	{
		private static MethodBase TargetMethod()
		{
			return AccessTools.Method("BannerlordPlayerSettlement.Main:OnSubModuleLoad");
		}

		private static void Postfix()
		{
			try
			{
				SiegeEnginesContainerRuntimeFix.Apply(new Harmony("com.b0tlanner.bannerlord.playersettlement.fixes"));
			}
			catch (Exception ex)
			{
				PlayerSettlementFixesLoader.LogError("SiegeFixBootstrap: " + ex.Message);
			}
		}
	}
	[HarmonyPatch]
	public static class DailyTickCastlesNullFix
	{
		private static MethodBase TargetMethod()
		{
			return AccessTools.Method("BannerlordPlayerSettlement.Behaviours.PlayerSettlementBehaviour:DailyTick");
		}

		private static void Prefix()
		{
			try
			{
				Type type = AccessTools.TypeByName("BannerlordPlayerSettlement.Saves.PlayerSettlementInfo");
				object obj = (type?.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public))?.GetValue(null);
				if (obj != null)
				{
					FieldInfo field = type.GetField("Castles", BindingFlags.Instance | BindingFlags.Public);
					if ((object)field == null || field.GetValue(obj) == null)
					{
						field.SetValue(obj, Activator.CreateInstance(typeof(List<>).MakeGenericType(AccessTools.TypeByName("BannerlordPlayerSettlement.Saves.PlayerSettlementItem"))));
					}
				}
			}
			catch (Exception ex)
			{
				PlayerSettlementFixesLoader.LogError("DailyTickCastlesNullFix: " + ex.Message);
			}
		}
	}
	[HarmonyPatch]
	public static class InitCastleBuildingsNullFix
	{
		private static MethodBase TargetMethod()
		{
			return AccessTools.Method("BannerlordPlayerSettlement.Behaviours.PlayerSettlementBehaviour:InitCastleBuildings");
		}

		private static Exception Finalizer(Exception __exception, Settlement castleSettlement)
		{
			if (__exception is NullReferenceException)
			{
				try
				{
					SafeInitBuildings(castleSettlement, "building_castle");
				}
				catch
				{
				}
				return null;
			}
			return __exception;
		}

		internal static void SafeInitBuildings(Settlement settlement, string prefix)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Expected O, but got Unknown
			Town town = settlement.Town;
			int val = MBRandom.RandomInt(1, 4);
			foreach (BuildingType bt in (List<BuildingType>)(object)BuildingType.All)
			{
				if (!((IEnumerable<Building>)town.Buildings).Any((Building k) => ((MBObjectBase)k.BuildingType).StringId == ((MBObjectBase)bt).StringId) && ((MBObjectBase)bt).StringId.StartsWith(prefix))
				{
					((List<Building>)(object)town.Buildings).Add(new Building(bt, town, 0f, Math.Max(val, bt.StartLevel)));
				}
			}
			foreach (Building item in ((IEnumerable<Building>)town.Buildings).OrderByDescending((Building k) => k.CurrentLevel))
			{
				if (item.CurrentLevel != 3 && item.CurrentLevel != item.BuildingType.StartLevel && !item.BuildingType.IsDailyProject && !town.BuildingsInProgress.Contains(item))
				{
					town.BuildingsInProgress.Enqueue(item);
				}
			}
			Building val2 = ((IEnumerable<Building>)town.Buildings).FirstOrDefault((Building b) => b.BuildingType.IsDailyProject && b.IsCurrentlyDefault);
			if (val2 == null)
			{
				val2 = ((IEnumerable<Building>)town.Buildings).FirstOrDefault((Building b) => b.BuildingType.IsDailyProject);
				if (val2 != null)
				{
					BuildingHelper.ChangeDefaultBuilding(val2, town);
					val2.IsCurrentlyDefault = true;
				}
			}
			if (val2 != null)
			{
				val2.IsCurrentlyDefault = true;
			}
		}
	}
	[HarmonyPatch]
	public static class InitTownBuildingsNullFix
	{
		private static MethodBase TargetMethod()
		{
			return AccessTools.Method("BannerlordPlayerSettlement.Behaviours.PlayerSettlementBehaviour:InitTownBuildings");
		}

		private static Exception Finalizer(Exception __exception, Settlement townSettlement)
		{
			if (__exception is NullReferenceException)
			{
				try
				{
					InitCastleBuildingsNullFix.SafeInitBuildings(townSettlement, "building_settlement");
				}
				catch
				{
				}
				return null;
			}
			return __exception;
		}
	}
	public static class SiegeEnginesContainerRuntimeFix
	{
		public static void Apply(Harmony harmony)
		{
			try
			{
				Type type = AccessTools.TypeByName("TaleWorlds.CampaignSystem.Siege.SiegeEnginesContainer");
				if (!(type == null))
				{
					MethodInfo methodInfo = AccessTools.Method(type, "DeploySiegeEngineAtIndex");
					MethodInfo methodInfo2 = AccessTools.Method(type, "RemoveDeployedSiegeEngine");
					MethodInfo method = typeof(SiegeEnginesContainerRuntimeFix).GetMethod("DeployPrefix", BindingFlags.Static | BindingFlags.Public);
					MethodInfo method2 = typeof(SiegeEnginesContainerRuntimeFix).GetMethod("RemovePrefix", BindingFlags.Static | BindingFlags.Public);
					if (methodInfo != null && method != null)
					{
						harmony.Patch(methodInfo, new HarmonyMethod(method));
					}
					if (methodInfo2 != null && method2 != null)
					{
						harmony.Patch(methodInfo2, new HarmonyMethod(method2));
					}
				}
			}
			catch (Exception ex)
			{
				PlayerSettlementFixesLoader.LogError("SiegeEnginesContainerRuntimeFix: " + ex.Message);
			}
		}

		public static void DeployPrefix(object __instance, object siegeEngine, int index)
		{
			try
			{
				Type type = __instance.GetType();
				bool flag = false;
				try
				{
					object obj = siegeEngine.GetType().GetProperty("SiegeEngine")?.GetValue(siegeEngine);
					if (obj != null && obj.GetType().GetProperty("IsRanged")?.GetValue(obj) is bool flag2)
					{
						flag = flag2;
					}
				}
				catch
				{
					return;
				}
				string name = (flag ? "DeployedRangedSiegeEngines" : "DeployedMeleeSiegeEngines");
				int val = (flag ? 4 : 3);
				FieldInfo fieldInfo = AccessTools.Field(type, name);
				Array array = (Array)fieldInfo.GetValue(__instance);
				if (array == null || index >= array.Length)
				{
					int length = Math.Max(index + 1, val);
					Array array2 = Array.CreateInstance(array?.GetType().GetElementType() ?? AccessTools.TypeByName("TaleWorlds.CampaignSystem.Siege.SiegeEngineConstructionProgress"), length);
					if (array != null)
					{
						Array.Copy(array, array2, array.Length);
					}
					fieldInfo.SetValue(__instance, array2);
				}
			}
			catch
			{
			}
		}

		public static void RemovePrefix(object __instance, int index, bool isRanged, bool moveToReserve)
		{
			try
			{
				Type type = __instance.GetType();
				string name = (isRanged ? "DeployedRangedSiegeEngines" : "DeployedMeleeSiegeEngines");
				int val = (isRanged ? 4 : 3);
				FieldInfo fieldInfo = AccessTools.Field(type, name);
				Array array = (Array)fieldInfo.GetValue(__instance);
				if (array == null || index >= array.Length)
				{
					int length = Math.Max(index + 1, val);
					Array array2 = Array.CreateInstance(array?.GetType().GetElementType() ?? AccessTools.TypeByName("TaleWorlds.CampaignSystem.Siege.SiegeEngineConstructionProgress"), length);
					if (array != null)
					{
						Array.Copy(array, array2, array.Length);
					}
					fieldInfo.SetValue(__instance, array2);
				}
			}
			catch
			{
			}
		}
	}
	[HarmonyPatch(typeof(BuildingsCampaignBehavior), "DailyTickSettlement")]
	public static class BuildingsDailyTickNullFix
	{
		private static void Prefix(Settlement settlement)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!settlement.IsFortification)
				{
					return;
				}
				Town town = settlement.Town;
				if (((SettlementComponent)town).Owner.Settlement.OwnerClan == Clan.PlayerClan && town.CurrentBuilding == null)
				{
					Building val = ((IEnumerable<Building>)town.Buildings).FirstOrDefault((Building b) => b.BuildingType.IsDailyProject && b.IsCurrentlyDefault) ?? ((IEnumerable<Building>)town.Buildings).FirstOrDefault((Building b) => b.BuildingType.IsDailyProject);
					if (val != null)
					{
						BuildingHelper.ChangeDefaultBuilding(val, town);
						val.IsCurrentlyDefault = true;
					}
				}
			}
			catch
			{
			}
		}
	}
	[HarmonyPatch]
	public static class BesiegerCampPositionsFallbackFix
	{
		private static MethodBase TargetMethod()
		{
			return AccessTools.Method("BannerlordPlayerSettlement.Behaviours.PlayerSettlementBehaviour:DailyTick");
		}

		private static void Postfix()
		{
			try
			{
				EnsureAllPlayerSettlementsCampPositions();
				LogSiegeDiagnostics();
			}
			catch
			{
			}
		}

		private static void LogSiegeDiagnostics()
		{
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Type type = AccessTools.TypeByName("BannerlordPlayerSettlement.Saves.PlayerSettlementInfo");
				object obj = (type?.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public))?.GetValue(null);
				if (obj == null)
				{
					return;
				}
				string[] array = new string[2] { "Towns", "Castles" };
				foreach (string name in array)
				{
					if (!(type.GetField(name, BindingFlags.Instance | BindingFlags.Public)?.GetValue(obj) is IList list))
					{
						continue;
					}
					foreach (object item in list)
					{
						object obj2 = (item?.GetType().GetField("Settlement", BindingFlags.Instance | BindingFlags.Public))?.GetValue(item);
						Settlement val = (Settlement)((obj2 is Settlement) ? obj2 : null);
						if (val == null)
						{
							continue;
						}
						string stringId = ((MBObjectBase)val).StringId;
						bool isFortification = val.IsFortification;
						IFaction mapFaction = val.MapFaction;
						string text = ((mapFaction == null) ? null : ((object)mapFaction.Name)?.ToString()) ?? "null";
						bool flag = mapFaction != null && mapFaction.IsKingdomFaction;
						List<string> list2 = new List<string>();
						try
						{
							foreach (Kingdom item2 in (List<Kingdom>)(object)Campaign.Current.Kingdoms)
							{
								if (item2.MapFaction != mapFaction && FactionManager.IsAtWarAgainstFaction((IFaction)(object)item2, mapFaction))
								{
									list2.Add(((MBObjectBase)item2).StringId);
								}
							}
						}
						catch
						{
						}
						float num = 0f;
						try
						{
							Town town = val.Town;
							int? obj4;
							if (town == null)
							{
								obj4 = null;
							}
							else
							{
								MobileParty garrisonParty = ((Fief)town).GarrisonParty;
								if (garrisonParty == null)
								{
									obj4 = null;
								}
								else
								{
									PartyBase party = garrisonParty.Party;
									if (party == null)
									{
										obj4 = null;
									}
									else
									{
										TroopRoster memberRoster = party.MemberRoster;
										obj4 = ((memberRoster != null) ? new int?(memberRoster.TotalHealthyCount) : null);
									}
								}
							}
							int? num2 = obj4;
							num = num2.GetValueOrDefault();
						}
						catch
						{
						}
						float num3 = 0f;
						try
						{
							num3 = val.Militia;
						}
						catch
						{
						}
						Town town2 = val.Town;
						bool flag2 = town2 != null && town2.BesiegerCampPositions1?.Length > 0;
						Town town3 = val.Town;
						bool flag3 = town3 != null && town3.BesiegerCampPositions2?.Length > 0;
						PlayerSettlementFixesLoader.LogError($"SIEGE_DIAG [{stringId}]: IsFort={isFortification} Faction={text} IsKingdom={flag} " + $"Garrison={num:F0} Militia={num3:F0} " + $"CampPos1={flag2} CampPos2={flag3} " + "EnemiesAtWar=[" + string.Join(",", list2) + "]");
					}
				}
			}
			catch (Exception ex)
			{
				PlayerSettlementFixesLoader.LogError("LogSiegeDiagnostics error: " + ex.Message);
			}
		}

		internal static void EnsureAllPlayerSettlementsCampPositions()
		{
			Type type = AccessTools.TypeByName("BannerlordPlayerSettlement.Saves.PlayerSettlementInfo");
			object obj = (type?.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public))?.GetValue(null);
			if (obj == null)
			{
				return;
			}
			string[] array = new string[2] { "Towns", "Castles" };
			foreach (string name in array)
			{
				if (!(type.GetField(name, BindingFlags.Instance | BindingFlags.Public)?.GetValue(obj) is IList list))
				{
					continue;
				}
				foreach (object item in list)
				{
					object obj2 = (item?.GetType().GetField("Settlement", BindingFlags.Instance | BindingFlags.Public))?.GetValue(item);
					Settlement val = (Settlement)((obj2 is Settlement) ? obj2 : null);
					if (val != null && val.IsFortification)
					{
						EnsureCampPositions(val);
					}
				}
			}
		}

		internal static void EnsureCampPositions(Settlement settlement)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			Town town = settlement.Town;
			if (town == null)
			{
				return;
			}
			bool flag = town.BesiegerCampPositions1 != null && town.BesiegerCampPositions1.Length != 0;
			bool flag2 = town.BesiegerCampPositions2 != null && town.BesiegerCampPositions2.Length != 0;
			if (!(flag && flag2))
			{
				CampaignVec2 position = settlement.Position;
				float x = ((CampaignVec2)(ref position)).X;
				position = settlement.Position;
				float y = ((CampaignVec2)(ref position)).Y;
				if (!flag)
				{
					town.BesiegerCampPositions1 = GenerateSiegeCampPositions(x, y, 4, 6f, 0f);
				}
				if (!flag2)
				{
					town.BesiegerCampPositions2 = GenerateSiegeCampPositions(x, y, 4, 6f, (float)Math.PI);
				}
			}
		}

		private static MatrixFrame[] GenerateSiegeCampPositions(float cx, float cy, int count, float radius, float angleOffset)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			MatrixFrame[] array = (MatrixFrame[])(object)new MatrixFrame[count];
			for (int i = 0; i < count; i++)
			{
				float num = angleOffset + (float)((double)i * Math.PI * 2.0 / (double)count);
				float num2 = cx + radius * (float)Math.Cos(num);
				float num3 = cy + radius * (float)Math.Sin(num);
				MatrixFrame identity = MatrixFrame.Identity;
				identity.origin = new Vec3(num2, num3, 0f, -1f);
				array[i] = identity;
			}
			return array;
		}
	}
	[HarmonyPatch]
	public static class BesiegerCampPositionsOnLoadFix
	{
		private static MethodBase TargetMethod()
		{
			return AccessTools.Method("BannerlordPlayerSettlement.Behaviours.PlayerSettlementBehaviour:OnLoad");
		}

		private static void Postfix()
		{
			try
			{
				BesiegerCampPositionsFallbackFix.EnsureAllPlayerSettlementsCampPositions();
			}
			catch
			{
			}
		}
	}
	[HarmonyPatch]
	public static class IssuesCampaignBehaviorNotablesFix
	{
		private static MethodBase TargetMethod()
		{
			return AccessTools.Method(typeof(IssuesCampaignBehavior), "CreateAnIssueForSettlementNotables");
		}

		private static Exception Finalizer(Exception __exception, Settlement settlement)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (__exception is NullReferenceException)
			{
				try
				{
					MBObjectBase val = (MBObjectBase)settlement;
					string text = (((int)val != 0) ? val.StringId : null);
					if (text != null && (text.StartsWith("player_settlement_town_") || text.StartsWith("player_settlement_castle_") || text.StartsWith("player_settlement_village_")))
					{
						PlayerSettlementFixesLoader.LogError("IssuesCampaignBehaviorNotablesFix: suppressed NRE for " + text);
						return null;
					}
				}
				catch
				{
				}
			}
			return __exception;
		}
	}
	[HarmonyPatch]
	public static class IssuesCampaignBehaviorClanNoblesFix
	{
		private static MethodBase TargetMethod()
		{
			return AccessTools.Method(typeof(IssuesCampaignBehavior), "CreateAnIssueForClanNobles");
		}

		private static Exception Finalizer(Exception __exception, Clan clan)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (__exception is NullReferenceException)
			{
				try
				{
					MBObjectBase val = (MBObjectBase)clan;
					PlayerSettlementFixesLoader.LogError("IssuesCampaignBehaviorClanNoblesFix: suppressed NRE for clan " + (((int)val != 0) ? val.StringId : null));
					return null;
				}
				catch
				{
					return null;
				}
			}
			return __exception;
		}
	}
	[HarmonyPatch]
	public static class SiegeMachineCirclesBoundsFix
	{
		private static MethodBase TargetMethod()
		{
			return AccessTools.Method("BannerlordPlayerSettlement.Patches.SettlementVisualManagerPatch:TickSiegeMachineCircles");
		}

		private static Exception Finalizer(Exception __exception)
		{
			if (__exception is IndexOutOfRangeException || __exception is NullReferenceException)
			{
				return null;
			}
			return __exception;
		}
	}
}
