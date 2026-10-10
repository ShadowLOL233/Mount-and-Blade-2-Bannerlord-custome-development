using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Bannerlord.UIExtenderEx;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;
using Bannerlord.UIExtenderEx.ViewModels;
using FormationManager.Data;
using FormationManager.Models;
using FormationManager.Patches;
using FormationManager.UI;
using HarmonyLib;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;
using TroopClassifier;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyCompany("FormationManager")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyFileVersion("0.5.2")]
[assembly: AssemblyInformationalVersion("0.5.2+a905af2d299d5dfe7a1d1ea19e1db81bd1d87592")]
[assembly: AssemblyProduct("FormationManager")]
[assembly: AssemblyTitle("FormationManager")]
[assembly: AssemblyVersion("0.5.2.0")]
[module: RefSafetyRules(11)]
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
	[CompilerGenerated]
	[Microsoft.CodeAnalysis.Embedded]
	[AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
	internal sealed class RefSafetyRulesAttribute : Attribute
	{
		public readonly int Version;

		public RefSafetyRulesAttribute(int P_0)
		{
			Version = P_0;
		}
	}
}
namespace FormationManager
{
	public class Settings : AttributeGlobalSettings<Settings>
	{
		private bool _showAdvancedFormationEditor = true;

		public override string Id => "FormationManager_v0_5";

		public override string DisplayName => "Stop Shuffling, You Fools - Formation Manager";

		public override string FolderName => "FormationManager";

		public override string FormatType => "json";

		[SettingPropertyBool("Enable Formation Manager", RequireRestart = false, HintText = "When enabled, troops are assigned to formations according to your configured rules. Disable to restore vanilla behaviour.", Order = 0)]
		[SettingPropertyGroup("General", GroupOrder = 0)]
		public bool ModEnabled { get; set; } = true;


		[SettingPropertyBool("Enable Troop Role Defaults", RequireRestart = false, HintText = "Use role-based troop classification to choose initial formation slots. Explicit troop assignments always take priority, and OOB changes remain editable for the current battle.", Order = 0)]
		[SettingPropertyGroup("Troop Role Defaults", GroupOrder = 1)]
		public bool UsePartyManagerRoleDefaults { get; set; } = true;


		[SettingPropertyBool("Use Spawned Equipment Classification", RequireRestart = false, HintText = "Classify each troop from the equipment it actually rolled for this battle. Enabled by default. Turn this off to use the troop's standard equipment sets, as Formation Manager did before v0.5.", Order = 1)]
		[SettingPropertyGroup("Troop Role Defaults", GroupOrder = 1)]
		public bool UseSpawnedEquipmentClassification { get; set; } = true;


		[SettingPropertyInteger("Light Infantry Formation", 1, 8, "0", RequireRestart = false, HintText = "Default slot for unshielded, non-specialist foot melee troops.", Order = 2)]
		[SettingPropertyGroup("Troop Role Defaults", GroupOrder = 1)]
		public int LightInfantryFormation { get; set; } = 1;


		[SettingPropertyInteger("Shield Infantry Formation", 1, 8, "0", RequireRestart = false, HintText = "Default slot for foot troops carrying a shield.", Order = 3)]
		[SettingPropertyGroup("Troop Role Defaults", GroupOrder = 1)]
		public int ShieldInfantryFormation { get; set; } = 1;


		[SettingPropertyInteger("Shock Infantry Formation", 1, 8, "0", RequireRestart = false, HintText = "Default slot for two-handed and offensive-polearm infantry.", Order = 4)]
		[SettingPropertyGroup("Troop Role Defaults", GroupOrder = 1)]
		public int ShockInfantryFormation { get; set; } = 1;


		[SettingPropertyInteger("Pike Infantry Formation", 1, 8, "0", RequireRestart = false, HintText = "Default slot for anti-cavalry pike infantry.", Order = 5)]
		[SettingPropertyGroup("Troop Role Defaults", GroupOrder = 1)]
		public int PikeInfantryFormation { get; set; } = 1;


		[SettingPropertyInteger("Skirmisher Formation", 1, 8, "0", RequireRestart = false, HintText = "Default slot for foot troops whose primary role is throwing weapons.", Order = 6)]
		[SettingPropertyGroup("Troop Role Defaults", GroupOrder = 1)]
		public int SkirmisherFormation { get; set; } = 1;


		[SettingPropertyInteger("Foot Archer Formation", 1, 8, "0", RequireRestart = false, HintText = "Default slot for bow-using foot archers and slingers.", Order = 7)]
		[SettingPropertyGroup("Troop Role Defaults", GroupOrder = 1)]
		public int FootArcherFormation { get; set; } = 2;


		[SettingPropertyInteger("Crossbowman Formation", 1, 8, "0", RequireRestart = false, HintText = "Default slot for crossbow-using foot archers.", Order = 8)]
		[SettingPropertyGroup("Troop Role Defaults", GroupOrder = 1)]
		public int CrossbowmanFormation { get; set; } = 2;


		[SettingPropertyInteger("Melee Cavalry Formation", 1, 8, "0", RequireRestart = false, HintText = "Default slot for mounted melee troops.", Order = 9)]
		[SettingPropertyGroup("Troop Role Defaults", GroupOrder = 1)]
		public int MeleeCavalryRoleFormation { get; set; } = 3;


		[SettingPropertyInteger("Horse Archer Formation", 1, 8, "0", RequireRestart = false, HintText = "Default slot for mounted bow, crossbow, or sling troops.", Order = 10)]
		[SettingPropertyGroup("Troop Role Defaults", GroupOrder = 1)]
		public int HorseArcherRoleFormation { get; set; } = 4;


		[SettingPropertyBool("Enable Mid-Battle Reassignment", RequireRestart = false, HintText = "When enabled, troops can be moved between configured formations during battle as their mounted or ammo state changes.", Order = 0)]
		[SettingPropertyGroup("Mid-Battle Reassignment", GroupOrder = 3)]
		public bool AutoReassignmentEnabled { get; set; } = true;


		[SettingPropertyInteger("Infantry Formation", 1, 8, "0", RequireRestart = false, HintText = "Formation slot (1-8) for infantry and dismounted melee cavalry.", Order = 1)]
		[SettingPropertyGroup("Mid-Battle Reassignment", GroupOrder = 3)]
		public int InfantryFormation { get; set; } = 1;


		[SettingPropertyInteger("Archer Formation", 1, 8, "0", RequireRestart = false, HintText = "Formation slot (1-8) for active ranged troops and dismounted horse archers.", Order = 2)]
		[SettingPropertyGroup("Mid-Battle Reassignment", GroupOrder = 3)]
		public int ArcherFormation { get; set; } = 2;


		[SettingPropertyInteger("Cavalry Formation", 1, 8, "0", RequireRestart = false, HintText = "Formation slot (1-8) for active mounted troops.", Order = 3)]
		[SettingPropertyGroup("Mid-Battle Reassignment", GroupOrder = 3)]
		public int CavalryFormation { get; set; } = 3;


		[SettingPropertyInteger("Backline Formation", 1, 8, "0", RequireRestart = false, HintText = "Formation slot (1-8) for foot ranged troops that are out of ammo. Used only when backline reassignment is enabled.", Order = 4)]
		[SettingPropertyGroup("Mid-Battle Reassignment", GroupOrder = 3)]
		public int BacklineFormation { get; set; } = 8;


		[SettingPropertyInteger("Mounted Backline Formation", 1, 8, "0", RequireRestart = false, HintText = "Formation slot (1-8) for mounted ranged troops that are out of ammo. Used only when backline reassignment is enabled.", Order = 5)]
		[SettingPropertyGroup("Mid-Battle Reassignment", GroupOrder = 3)]
		public int MountedBacklineFormation { get; set; } = 8;


		[SettingPropertyBool("Enable Out-of-Ammo Backline Reassignment", RequireRestart = false, HintText = "When enabled, ranged troops with no ammo move to the configured backline formations. Enabled by default.", Order = 6)]
		[SettingPropertyGroup("Mid-Battle Reassignment", GroupOrder = 3)]
		public bool BacklineReassignmentEnabled { get; set; } = true;


		[SettingPropertyBool("Prioritize Weights in Small Stacks", RequireRestart = false, HintText = "When there are too few troops to satisfy every custom target, distribute by weight first. Disabled by default: give one troop to each active formation from I to VIII before distributing the remainder.", Order = 0)]
		[SettingPropertyGroup("Multi-Formation Splits", GroupOrder = 2)]
		public bool PrioritizeWeightsInSmallStacks { get; set; }

		[SettingPropertyBool("Enable Advanced Formation Plans", RequireRestart = false, HintText = "Show and apply advanced Target/Weight plans. Turning this off completely hides the advanced panel and archives every active plan. Turning it back on exposes the editor, but archived plans remain inactive until restored individually.", Order = 1)]
		[SettingPropertyGroup("Multi-Formation Splits", GroupOrder = 2)]
		public bool ShowAdvancedFormationEditor
		{
			get
			{
				return _showAdvancedFormationEditor;
			}
			set
			{
				if (_showAdvancedFormationEditor != value)
				{
					_showAdvancedFormationEditor = value;
					FormationAssignmentStore.SetAdvancedPlansEnabled(value);
				}
			}
		}

		[SettingPropertyBool("Autosave Advanced Formation Plans", RequireRestart = false, HintText = "Save advanced formation edits immediately when targets, weights, or active formations change. Disabled by default so edits remain a draft until the Save button is pressed.", Order = 2)]
		[SettingPropertyGroup("Multi-Formation Splits", GroupOrder = 2)]
		public bool AutosaveAdvancedFormationPlans { get; set; }

		[SettingPropertyBool("Lock Managed OOB Sliders", RequireRestart = false, HintText = "Lock Bannerlord's native weight sliders on formation cards seeded by Formation Manager. Turn this off to manually rebalance those cards in the Order of Battle screen.", Order = 3)]
		[SettingPropertyGroup("Multi-Formation Splits", GroupOrder = 2)]
		public bool LockManagedOobSliders { get; set; } = true;


		[SettingPropertyBool("Manage Allied Infantry (Opt-In)", RequireRestart = false, HintText = "Allow Formation Manager to place allied infantry into your formations. Disabled by default: only troops from your own party are managed.", Order = 0)]
		[SettingPropertyGroup("Allied Troop Control", GroupOrder = 4)]
		public bool ManageAlliedInfantry { get; set; }

		[SettingPropertyBool("Manage Allied Ranged Troops (Opt-In)", RequireRestart = false, HintText = "Allow Formation Manager to place allied foot archers and crossbowmen into your formations. Disabled by default.", Order = 1)]
		[SettingPropertyGroup("Allied Troop Control", GroupOrder = 4)]
		public bool ManageAlliedRanged { get; set; }

		[SettingPropertyBool("Manage Allied Cavalry (Opt-In)", RequireRestart = false, HintText = "Allow Formation Manager to place allied melee cavalry into your formations. Disabled by default.", Order = 2)]
		[SettingPropertyGroup("Allied Troop Control", GroupOrder = 4)]
		public bool ManageAlliedCavalry { get; set; }

		[SettingPropertyBool("Manage Allied Horse Archers (Opt-In)", RequireRestart = false, HintText = "Allow Formation Manager to place allied mounted archers into your formations. Disabled by default.", Order = 3)]
		[SettingPropertyGroup("Allied Troop Control", GroupOrder = 4)]
		public bool ManageAlliedHorseArchers { get; set; }
	}
	public class SubModule : MBSubModuleBase
	{
		private static Harmony? _harmony;

		private static UIExtender? _uiExtender;

		private static bool _uiExtenderInitialized;

		protected override void OnSubModuleLoad()
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			base.OnSubModuleLoad();
			try
			{
				string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				string path = Path.Combine(folderPath, "Mount and Blade II Bannerlord", "Configs", "FormationManager", "log.txt");
				if (File.Exists(path))
				{
					File.Delete(path);
				}
			}
			catch
			{
			}
			try
			{
				_harmony = new Harmony("com.formationmanager");
				_harmony.PatchAll();
				FormationManager.Data.Logger.Log("[SubModule] Harmony.PatchAll() succeeded.");
			}
			catch (Exception arg)
			{
				FormationManager.Data.Logger.Log($"[SubModule] Harmony.PatchAll() FAILED: {arg}");
			}
		}

		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			base.OnBeforeInitialModuleScreenSetAsRoot();
			if (_uiExtenderInitialized)
			{
				return;
			}
			_uiExtenderInitialized = true;
			try
			{
				_uiExtender = UIExtender.Create("FormationManager");
				_uiExtender.Register(typeof(SubModule).Assembly);
				_uiExtender.Enable();
			}
			catch (Exception ex)
			{
				try
				{
					string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mount and Blade II Bannerlord", "Configs");
					if (!Directory.Exists(text))
					{
						Directory.CreateDirectory(text);
					}
					string path = Path.Combine(text, "FormationManager_LoadError.txt");
					string text2 = ex.ToString();
					if (ex is ReflectionTypeLoadException ex2)
					{
						text2 += "\nLoader Exceptions:\n";
						Exception[] loaderExceptions = ex2.LoaderExceptions;
						foreach (Exception ex3 in loaderExceptions)
						{
							text2 = text2 + ex3.ToString() + "\n";
						}
					}
					File.WriteAllText(path, text2);
				}
				catch
				{
				}
			}
		}

		protected override void OnGameStart(Game game, IGameStarter gameStarter)
		{
			base.OnGameStart(game, gameStarter);
			try
			{
				BattleInitializationModel battleInitializationModel = gameStarter.Models.OfType<BattleInitializationModel>().FirstOrDefault();
				if (battleInitializationModel != null)
				{
					gameStarter.AddModel(new FormationManagerBattleInitializationModel(battleInitializationModel));
					FormationManager.Data.Logger.Log("[SubModule] Natively registered FormationManagerBattleInitializationModel successfully.");
				}
				else
				{
					FormationManager.Data.Logger.Log("[SubModule] Vanilla BattleInitializationModel not found to override.");
				}
			}
			catch (Exception arg)
			{
				FormationManager.Data.Logger.Log($"[SubModule] Failed to natively register battle initialization model: {arg}");
			}
		}

		public override void OnGameLoaded(Game game, object initializerObject)
		{
			base.OnGameLoaded(game, initializerObject);
			if (game.GameType is Campaign)
			{
				Hero mainHero = Hero.MainHero;
				if (mainHero != null)
				{
					FormationAssignmentStore.Load(mainHero.StringId);
				}
			}
		}

		public override void OnBeforeMissionBehaviorInitialize(Mission mission)
		{
			base.OnBeforeMissionBehaviorInitialize(mission);
			FormationManager.Data.Logger.Log("[SubModule] OnBeforeMissionBehaviorInitialize fired. Mission=" + ((object)mission)?.GetType()?.Name);
			if (!MissionGuards.CanAttachMissionBehavior(mission))
			{
				FormationManager.Data.Logger.Log("[SubModule] Mission is not on the supported regular-battle whitelist. Skipping FormationManager mission behavior.");
			}
			else
			{
				mission?.AddMissionBehavior(new MissionTroopReassignmentBehavior());
			}
		}

		public override void OnGameEnd(Game game)
		{
			base.OnGameEnd(game);
			FormationAssignmentStore.Save();
		}
	}
}
namespace FormationManager.UI
{
	internal sealed class AdvancedFormationPlanDraft
	{
		private readonly int[] _targets = new int[8];

		private readonly int[] _weights = new int[8];

		public bool IsInitialized { get; private set; }

		public bool IsDirty { get; private set; }

		public bool IsClearRequested { get; private set; }

		public IReadOnlyDictionary<int, int> Targets => Enumerable.Range(0, 8).ToDictionary((int index) => index, GetTarget);

		public IReadOnlyDictionary<int, int> Weights => Enumerable.Range(0, 8).ToDictionary((int index) => index, GetWeight);

		public int GetTarget(int formationIndex)
		{
			return _targets[formationIndex];
		}

		public int GetWeight(int formationIndex)
		{
			return _weights[formationIndex];
		}

		public bool IsActive(int formationIndex)
		{
			if (_targets[formationIndex] <= 0)
			{
				return _weights[formationIndex] > 0;
			}
			return true;
		}

		public void Initialize(TroopDeploymentPlan? plan, IEnumerable<int> baselineFormationIndices, bool hasArchivedPlan)
		{
			if (IsInitialized)
			{
				return;
			}
			ClearValues();
			if (plan != null)
			{
				LoadValues(plan);
			}
			else if (!hasArchivedPlan)
			{
				foreach (int item in baselineFormationIndices.Where(IsValidFormationIndex).Distinct())
				{
					_targets[item] = 1;
				}
			}
			IsInitialized = true;
			IsDirty = false;
			IsClearRequested = false;
		}

		public void Load(TroopDeploymentPlan plan)
		{
			ClearValues();
			LoadValues(plan);
			IsInitialized = true;
			IsDirty = true;
			IsClearRequested = false;
		}

		public void Reset()
		{
			ClearValues();
			IsInitialized = false;
			IsDirty = false;
			IsClearRequested = false;
		}

		public void Clear()
		{
			ClearValues();
			IsClearRequested = true;
			IsDirty = true;
		}

		public bool Activate(int formationIndex)
		{
			if (!IsValidFormationIndex(formationIndex) || IsActive(formationIndex))
			{
				return false;
			}
			_targets[formationIndex] = 1;
			IsClearRequested = false;
			IsDirty = true;
			return true;
		}

		public bool Deactivate(int formationIndex)
		{
			if (!IsValidFormationIndex(formationIndex) || !IsActive(formationIndex))
			{
				return false;
			}
			_targets[formationIndex] = 0;
			_weights[formationIndex] = 0;
			UpdateClearRequested();
			IsDirty = true;
			return true;
		}

		public bool SetTarget(int formationIndex, int value, int readyTroopCount)
		{
			if (!IsValidFormationIndex(formationIndex))
			{
				return false;
			}
			int num = (from index in Enumerable.Range(0, 8)
				where index != formationIndex
				select index).Sum((int index) => _targets[index]);
			int num2 = Math.Max(0, Math.Min(value, Math.Max(0, readyTroopCount - num)));
			if (_targets[formationIndex] == num2)
			{
				return false;
			}
			_targets[formationIndex] = num2;
			UpdateClearRequested();
			IsDirty = true;
			return true;
		}

		public bool SetWeight(int formationIndex, int value)
		{
			if (!IsValidFormationIndex(formationIndex))
			{
				return false;
			}
			int num = Math.Max(0, Math.Min(100, value));
			if (_weights[formationIndex] == num)
			{
				return false;
			}
			_weights[formationIndex] = num;
			UpdateClearRequested();
			IsDirty = true;
			return true;
		}

		public int GetTargetMaximum(int formationIndex, int readyTroopCount)
		{
			if (IsValidFormationIndex(formationIndex))
			{
				return Math.Max(0, readyTroopCount - (from index in Enumerable.Range(0, 8)
					where index != formationIndex
					select index).Sum((int index) => _targets[index]));
			}
			return 0;
		}

		public void MarkSaved()
		{
			IsDirty = false;
			IsClearRequested = false;
		}

		private void LoadValues(TroopDeploymentPlan plan)
		{
			if (plan.Mode == TroopDeploymentPlanMode.Custom)
			{
				foreach (KeyValuePair<int, int> item in plan.FormationTargets.Where((KeyValuePair<int, int> pair) => IsValidFormationIndex(pair.Key)))
				{
					_targets[item.Key] = item.Value;
				}
				{
					foreach (KeyValuePair<int, int> item2 in plan.FormationWeights.Where((KeyValuePair<int, int> pair) => IsValidFormationIndex(pair.Key)))
					{
						_weights[item2.Key] = item2.Value;
					}
					return;
				}
			}
			foreach (int item3 in plan.FormationIndices.Where(IsValidFormationIndex))
			{
				_weights[item3] = 1;
			}
		}

		private void UpdateClearRequested()
		{
			IsClearRequested = !_targets.Any((int value) => value > 0) && !_weights.Any((int value) => value > 0);
		}

		private void ClearValues()
		{
			Array.Clear(_targets, 0, _targets.Length);
			Array.Clear(_weights, 0, _weights.Length);
		}

		private static bool IsValidFormationIndex(int formationIndex)
		{
			if (formationIndex >= 0)
			{
				return formationIndex <= 7;
			}
			return false;
		}
	}
	[ViewModelMixin("RefreshValues")]
	internal sealed class PartyCharacterVMMixin : BaseViewModelMixin<PartyCharacterVM>
	{
		private static readonly string[] Labels = new string[9] { "—", "I", "II", "III", "IV", "V", "VI", "VII", "VIII" };

		private static readonly TroopRole[] RolePlanRoles;

		private static readonly ConditionalWeakTable<PartyCharacterVM, PartyCharacterVMMixin> Instances;

		private static readonly List<WeakReference<PartyCharacterVMMixin>> ActiveMixins;

		private static WeakReference<PartyCharacterVMMixin>? LastSelectedMixin;

		private static bool _autosaveHintShown;

		private readonly string[] _targetTexts = new string[8];

		private readonly string[] _assignedTexts = new string[8];

		private readonly string[] _weightTexts = new string[8];

		private readonly bool[] _editorFormationActive = new bool[8];

		private readonly AdvancedFormationPlanDraft _draft = new AdvancedFormationPlanDraft();

		private readonly Dictionary<TroopRole, int[]> _rolePlanDraft = new Dictionary<TroopRole, int[]>();

		private readonly Dictionary<TroopRole, bool> _roleSplitEnabledDraft = new Dictionary<TroopRole, bool>();

		private bool _hasRolePlanDraftChanges;

		private string _formationLabel = "—";

		private string _secondaryFormationLabel = "+";

		private bool _isFormationBadgeVisible;

		private bool _isSecondaryFormationBadgeVisible;

		private int _selectedCustomFormationIndex = -1;

		private bool _lastKnownSelected;

		[DataSourceProperty]
		public string FormationLabel
		{
			get
			{
				return _formationLabel;
			}
			set
			{
				if (_formationLabel != value)
				{
					_formationLabel = value;
					base.OnPropertyChanged("FormationLabel");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFormationBadgeVisible
		{
			get
			{
				return _isFormationBadgeVisible;
			}
			set
			{
				if (_isFormationBadgeVisible != value)
				{
					_isFormationBadgeVisible = value;
					base.OnPropertyChanged("IsFormationBadgeVisible");
				}
			}
		}

		[DataSourceProperty]
		public string SecondaryFormationLabel
		{
			get
			{
				return _secondaryFormationLabel;
			}
			set
			{
				if (_secondaryFormationLabel != value)
				{
					_secondaryFormationLabel = value;
					base.OnPropertyChanged("SecondaryFormationLabel");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSecondaryFormationBadgeVisible
		{
			get
			{
				return _isSecondaryFormationBadgeVisible;
			}
			set
			{
				if (_isSecondaryFormationBadgeVisible != value)
				{
					_isSecondaryFormationBadgeVisible = value;
					base.OnPropertyChanged("IsSecondaryFormationBadgeVisible");
				}
			}
		}

		[DataSourceProperty]
		public bool IsRolePlanBadgeVisible
		{
			get
			{
				if (IsPlayerHeroRow)
				{
					return FormationAssignmentStore.HasAnyRoleAssignments;
				}
				return false;
			}
		}

		[DataSourceProperty]
		public bool IsRolePlanEditorVisible
		{
			get
			{
				if (IsPlayerHeroRow)
				{
					return base.ViewModel?.IsSelected ?? false;
				}
				return false;
			}
		}

		[DataSourceProperty]
		public bool HasUnsavedRolePlanChanges => _hasRolePlanDraftChanges;

		[DataSourceProperty]
		public bool HasRolePlan => FormationAssignmentStore.HasAnyRoleAssignments;

		[DataSourceProperty]
		public bool CanRestoreRolePlan => FormationAssignmentStore.CanRestoreRoleAssignments;

		[DataSourceProperty]
		public string LightInfantryRoleSplitLabel => GetRoleSplitLabel((TroopRole)0);

		[DataSourceProperty]
		public bool IsLightInfantryRoleGridVisible => IsRoleGridVisible((TroopRole)0);

		[DataSourceProperty]
		public bool IsLightInfantryRoleFormation1Selected => IsRoleFormationMarked((TroopRole)0, 0);

		[DataSourceProperty]
		public bool IsLightInfantryRoleFormation2Selected => IsRoleFormationMarked((TroopRole)0, 1);

		[DataSourceProperty]
		public bool IsLightInfantryRoleFormation3Selected => IsRoleFormationMarked((TroopRole)0, 2);

		[DataSourceProperty]
		public bool IsLightInfantryRoleFormation4Selected => IsRoleFormationMarked((TroopRole)0, 3);

		[DataSourceProperty]
		public bool IsLightInfantryRoleFormation5Selected => IsRoleFormationMarked((TroopRole)0, 4);

		[DataSourceProperty]
		public bool IsLightInfantryRoleFormation6Selected => IsRoleFormationMarked((TroopRole)0, 5);

		[DataSourceProperty]
		public bool IsLightInfantryRoleFormation7Selected => IsRoleFormationMarked((TroopRole)0, 6);

		[DataSourceProperty]
		public bool IsLightInfantryRoleFormation8Selected => IsRoleFormationMarked((TroopRole)0, 7);

		[DataSourceProperty]
		public string ShieldInfantryRoleSplitLabel => GetRoleSplitLabel((TroopRole)1);

		[DataSourceProperty]
		public bool IsShieldInfantryRoleGridVisible => IsRoleGridVisible((TroopRole)1);

		[DataSourceProperty]
		public bool IsShieldInfantryRoleFormation1Selected => IsRoleFormationMarked((TroopRole)1, 0);

		[DataSourceProperty]
		public bool IsShieldInfantryRoleFormation2Selected => IsRoleFormationMarked((TroopRole)1, 1);

		[DataSourceProperty]
		public bool IsShieldInfantryRoleFormation3Selected => IsRoleFormationMarked((TroopRole)1, 2);

		[DataSourceProperty]
		public bool IsShieldInfantryRoleFormation4Selected => IsRoleFormationMarked((TroopRole)1, 3);

		[DataSourceProperty]
		public bool IsShieldInfantryRoleFormation5Selected => IsRoleFormationMarked((TroopRole)1, 4);

		[DataSourceProperty]
		public bool IsShieldInfantryRoleFormation6Selected => IsRoleFormationMarked((TroopRole)1, 5);

		[DataSourceProperty]
		public bool IsShieldInfantryRoleFormation7Selected => IsRoleFormationMarked((TroopRole)1, 6);

		[DataSourceProperty]
		public bool IsShieldInfantryRoleFormation8Selected => IsRoleFormationMarked((TroopRole)1, 7);

		[DataSourceProperty]
		public string ShockInfantryRoleSplitLabel => GetRoleSplitLabel((TroopRole)2);

		[DataSourceProperty]
		public bool IsShockInfantryRoleGridVisible => IsRoleGridVisible((TroopRole)2);

		[DataSourceProperty]
		public bool IsShockInfantryRoleFormation1Selected => IsRoleFormationMarked((TroopRole)2, 0);

		[DataSourceProperty]
		public bool IsShockInfantryRoleFormation2Selected => IsRoleFormationMarked((TroopRole)2, 1);

		[DataSourceProperty]
		public bool IsShockInfantryRoleFormation3Selected => IsRoleFormationMarked((TroopRole)2, 2);

		[DataSourceProperty]
		public bool IsShockInfantryRoleFormation4Selected => IsRoleFormationMarked((TroopRole)2, 3);

		[DataSourceProperty]
		public bool IsShockInfantryRoleFormation5Selected => IsRoleFormationMarked((TroopRole)2, 4);

		[DataSourceProperty]
		public bool IsShockInfantryRoleFormation6Selected => IsRoleFormationMarked((TroopRole)2, 5);

		[DataSourceProperty]
		public bool IsShockInfantryRoleFormation7Selected => IsRoleFormationMarked((TroopRole)2, 6);

		[DataSourceProperty]
		public bool IsShockInfantryRoleFormation8Selected => IsRoleFormationMarked((TroopRole)2, 7);

		[DataSourceProperty]
		public string PikeInfantryRoleSplitLabel => GetRoleSplitLabel((TroopRole)8);

		[DataSourceProperty]
		public bool IsPikeInfantryRoleGridVisible => IsRoleGridVisible((TroopRole)8);

		[DataSourceProperty]
		public bool IsPikeInfantryRoleFormation1Selected => IsRoleFormationMarked((TroopRole)8, 0);

		[DataSourceProperty]
		public bool IsPikeInfantryRoleFormation2Selected => IsRoleFormationMarked((TroopRole)8, 1);

		[DataSourceProperty]
		public bool IsPikeInfantryRoleFormation3Selected => IsRoleFormationMarked((TroopRole)8, 2);

		[DataSourceProperty]
		public bool IsPikeInfantryRoleFormation4Selected => IsRoleFormationMarked((TroopRole)8, 3);

		[DataSourceProperty]
		public bool IsPikeInfantryRoleFormation5Selected => IsRoleFormationMarked((TroopRole)8, 4);

		[DataSourceProperty]
		public bool IsPikeInfantryRoleFormation6Selected => IsRoleFormationMarked((TroopRole)8, 5);

		[DataSourceProperty]
		public bool IsPikeInfantryRoleFormation7Selected => IsRoleFormationMarked((TroopRole)8, 6);

		[DataSourceProperty]
		public bool IsPikeInfantryRoleFormation8Selected => IsRoleFormationMarked((TroopRole)8, 7);

		[DataSourceProperty]
		public string SkirmisherRoleSplitLabel => GetRoleSplitLabel((TroopRole)3);

		[DataSourceProperty]
		public bool IsSkirmisherRoleGridVisible => IsRoleGridVisible((TroopRole)3);

		[DataSourceProperty]
		public bool IsSkirmisherRoleFormation1Selected => IsRoleFormationMarked((TroopRole)3, 0);

		[DataSourceProperty]
		public bool IsSkirmisherRoleFormation2Selected => IsRoleFormationMarked((TroopRole)3, 1);

		[DataSourceProperty]
		public bool IsSkirmisherRoleFormation3Selected => IsRoleFormationMarked((TroopRole)3, 2);

		[DataSourceProperty]
		public bool IsSkirmisherRoleFormation4Selected => IsRoleFormationMarked((TroopRole)3, 3);

		[DataSourceProperty]
		public bool IsSkirmisherRoleFormation5Selected => IsRoleFormationMarked((TroopRole)3, 4);

		[DataSourceProperty]
		public bool IsSkirmisherRoleFormation6Selected => IsRoleFormationMarked((TroopRole)3, 5);

		[DataSourceProperty]
		public bool IsSkirmisherRoleFormation7Selected => IsRoleFormationMarked((TroopRole)3, 6);

		[DataSourceProperty]
		public bool IsSkirmisherRoleFormation8Selected => IsRoleFormationMarked((TroopRole)3, 7);

		[DataSourceProperty]
		public string FootArcherRoleSplitLabel => GetRoleSplitLabel((TroopRole)4);

		[DataSourceProperty]
		public bool IsFootArcherRoleGridVisible => IsRoleGridVisible((TroopRole)4);

		[DataSourceProperty]
		public bool IsFootArcherRoleFormation1Selected => IsRoleFormationMarked((TroopRole)4, 0);

		[DataSourceProperty]
		public bool IsFootArcherRoleFormation2Selected => IsRoleFormationMarked((TroopRole)4, 1);

		[DataSourceProperty]
		public bool IsFootArcherRoleFormation3Selected => IsRoleFormationMarked((TroopRole)4, 2);

		[DataSourceProperty]
		public bool IsFootArcherRoleFormation4Selected => IsRoleFormationMarked((TroopRole)4, 3);

		[DataSourceProperty]
		public bool IsFootArcherRoleFormation5Selected => IsRoleFormationMarked((TroopRole)4, 4);

		[DataSourceProperty]
		public bool IsFootArcherRoleFormation6Selected => IsRoleFormationMarked((TroopRole)4, 5);

		[DataSourceProperty]
		public bool IsFootArcherRoleFormation7Selected => IsRoleFormationMarked((TroopRole)4, 6);

		[DataSourceProperty]
		public bool IsFootArcherRoleFormation8Selected => IsRoleFormationMarked((TroopRole)4, 7);

		[DataSourceProperty]
		public string CrossbowmanRoleSplitLabel => GetRoleSplitLabel((TroopRole)5);

		[DataSourceProperty]
		public bool IsCrossbowmanRoleGridVisible => IsRoleGridVisible((TroopRole)5);

		[DataSourceProperty]
		public bool IsCrossbowmanRoleFormation1Selected => IsRoleFormationMarked((TroopRole)5, 0);

		[DataSourceProperty]
		public bool IsCrossbowmanRoleFormation2Selected => IsRoleFormationMarked((TroopRole)5, 1);

		[DataSourceProperty]
		public bool IsCrossbowmanRoleFormation3Selected => IsRoleFormationMarked((TroopRole)5, 2);

		[DataSourceProperty]
		public bool IsCrossbowmanRoleFormation4Selected => IsRoleFormationMarked((TroopRole)5, 3);

		[DataSourceProperty]
		public bool IsCrossbowmanRoleFormation5Selected => IsRoleFormationMarked((TroopRole)5, 4);

		[DataSourceProperty]
		public bool IsCrossbowmanRoleFormation6Selected => IsRoleFormationMarked((TroopRole)5, 5);

		[DataSourceProperty]
		public bool IsCrossbowmanRoleFormation7Selected => IsRoleFormationMarked((TroopRole)5, 6);

		[DataSourceProperty]
		public bool IsCrossbowmanRoleFormation8Selected => IsRoleFormationMarked((TroopRole)5, 7);

		[DataSourceProperty]
		public string MeleeCavalryRoleSplitLabel => GetRoleSplitLabel((TroopRole)6);

		[DataSourceProperty]
		public bool IsMeleeCavalryRoleGridVisible => IsRoleGridVisible((TroopRole)6);

		[DataSourceProperty]
		public bool IsMeleeCavalryRoleFormation1Selected => IsRoleFormationMarked((TroopRole)6, 0);

		[DataSourceProperty]
		public bool IsMeleeCavalryRoleFormation2Selected => IsRoleFormationMarked((TroopRole)6, 1);

		[DataSourceProperty]
		public bool IsMeleeCavalryRoleFormation3Selected => IsRoleFormationMarked((TroopRole)6, 2);

		[DataSourceProperty]
		public bool IsMeleeCavalryRoleFormation4Selected => IsRoleFormationMarked((TroopRole)6, 3);

		[DataSourceProperty]
		public bool IsMeleeCavalryRoleFormation5Selected => IsRoleFormationMarked((TroopRole)6, 4);

		[DataSourceProperty]
		public bool IsMeleeCavalryRoleFormation6Selected => IsRoleFormationMarked((TroopRole)6, 5);

		[DataSourceProperty]
		public bool IsMeleeCavalryRoleFormation7Selected => IsRoleFormationMarked((TroopRole)6, 6);

		[DataSourceProperty]
		public bool IsMeleeCavalryRoleFormation8Selected => IsRoleFormationMarked((TroopRole)6, 7);

		[DataSourceProperty]
		public string HorseArcherRoleSplitLabel => GetRoleSplitLabel((TroopRole)7);

		[DataSourceProperty]
		public bool IsHorseArcherRoleGridVisible => IsRoleGridVisible((TroopRole)7);

		[DataSourceProperty]
		public bool IsHorseArcherRoleFormation1Selected => IsRoleFormationMarked((TroopRole)7, 0);

		[DataSourceProperty]
		public bool IsHorseArcherRoleFormation2Selected => IsRoleFormationMarked((TroopRole)7, 1);

		[DataSourceProperty]
		public bool IsHorseArcherRoleFormation3Selected => IsRoleFormationMarked((TroopRole)7, 2);

		[DataSourceProperty]
		public bool IsHorseArcherRoleFormation4Selected => IsRoleFormationMarked((TroopRole)7, 3);

		[DataSourceProperty]
		public bool IsHorseArcherRoleFormation5Selected => IsRoleFormationMarked((TroopRole)7, 4);

		[DataSourceProperty]
		public bool IsHorseArcherRoleFormation6Selected => IsRoleFormationMarked((TroopRole)7, 5);

		[DataSourceProperty]
		public bool IsHorseArcherRoleFormation7Selected => IsRoleFormationMarked((TroopRole)7, 6);

		[DataSourceProperty]
		public bool IsHorseArcherRoleFormation8Selected => IsRoleFormationMarked((TroopRole)7, 7);

		[DataSourceProperty]
		public string LightInfantryRoleFormation1Label => GetRoleFormationLabel((TroopRole)0, 0);

		[DataSourceProperty]
		public string LightInfantryRoleFormation2Label => GetRoleFormationLabel((TroopRole)0, 1);

		[DataSourceProperty]
		public string LightInfantryRoleFormation3Label => GetRoleFormationLabel((TroopRole)0, 2);

		[DataSourceProperty]
		public string LightInfantryRoleFormation4Label => GetRoleFormationLabel((TroopRole)0, 3);

		[DataSourceProperty]
		public string LightInfantryRoleFormation5Label => GetRoleFormationLabel((TroopRole)0, 4);

		[DataSourceProperty]
		public string LightInfantryRoleFormation6Label => GetRoleFormationLabel((TroopRole)0, 5);

		[DataSourceProperty]
		public string LightInfantryRoleFormation7Label => GetRoleFormationLabel((TroopRole)0, 6);

		[DataSourceProperty]
		public string LightInfantryRoleFormation8Label => GetRoleFormationLabel((TroopRole)0, 7);

		[DataSourceProperty]
		public string ShieldInfantryRoleFormation1Label => GetRoleFormationLabel((TroopRole)1, 0);

		[DataSourceProperty]
		public string ShieldInfantryRoleFormation2Label => GetRoleFormationLabel((TroopRole)1, 1);

		[DataSourceProperty]
		public string ShieldInfantryRoleFormation3Label => GetRoleFormationLabel((TroopRole)1, 2);

		[DataSourceProperty]
		public string ShieldInfantryRoleFormation4Label => GetRoleFormationLabel((TroopRole)1, 3);

		[DataSourceProperty]
		public string ShieldInfantryRoleFormation5Label => GetRoleFormationLabel((TroopRole)1, 4);

		[DataSourceProperty]
		public string ShieldInfantryRoleFormation6Label => GetRoleFormationLabel((TroopRole)1, 5);

		[DataSourceProperty]
		public string ShieldInfantryRoleFormation7Label => GetRoleFormationLabel((TroopRole)1, 6);

		[DataSourceProperty]
		public string ShieldInfantryRoleFormation8Label => GetRoleFormationLabel((TroopRole)1, 7);

		[DataSourceProperty]
		public string ShockInfantryRoleFormation1Label => GetRoleFormationLabel((TroopRole)2, 0);

		[DataSourceProperty]
		public string ShockInfantryRoleFormation2Label => GetRoleFormationLabel((TroopRole)2, 1);

		[DataSourceProperty]
		public string ShockInfantryRoleFormation3Label => GetRoleFormationLabel((TroopRole)2, 2);

		[DataSourceProperty]
		public string ShockInfantryRoleFormation4Label => GetRoleFormationLabel((TroopRole)2, 3);

		[DataSourceProperty]
		public string ShockInfantryRoleFormation5Label => GetRoleFormationLabel((TroopRole)2, 4);

		[DataSourceProperty]
		public string ShockInfantryRoleFormation6Label => GetRoleFormationLabel((TroopRole)2, 5);

		[DataSourceProperty]
		public string ShockInfantryRoleFormation7Label => GetRoleFormationLabel((TroopRole)2, 6);

		[DataSourceProperty]
		public string ShockInfantryRoleFormation8Label => GetRoleFormationLabel((TroopRole)2, 7);

		[DataSourceProperty]
		public string PikeInfantryRoleFormation1Label => GetRoleFormationLabel((TroopRole)8, 0);

		[DataSourceProperty]
		public string PikeInfantryRoleFormation2Label => GetRoleFormationLabel((TroopRole)8, 1);

		[DataSourceProperty]
		public string PikeInfantryRoleFormation3Label => GetRoleFormationLabel((TroopRole)8, 2);

		[DataSourceProperty]
		public string PikeInfantryRoleFormation4Label => GetRoleFormationLabel((TroopRole)8, 3);

		[DataSourceProperty]
		public string PikeInfantryRoleFormation5Label => GetRoleFormationLabel((TroopRole)8, 4);

		[DataSourceProperty]
		public string PikeInfantryRoleFormation6Label => GetRoleFormationLabel((TroopRole)8, 5);

		[DataSourceProperty]
		public string PikeInfantryRoleFormation7Label => GetRoleFormationLabel((TroopRole)8, 6);

		[DataSourceProperty]
		public string PikeInfantryRoleFormation8Label => GetRoleFormationLabel((TroopRole)8, 7);

		[DataSourceProperty]
		public string SkirmisherRoleFormation1Label => GetRoleFormationLabel((TroopRole)3, 0);

		[DataSourceProperty]
		public string SkirmisherRoleFormation2Label => GetRoleFormationLabel((TroopRole)3, 1);

		[DataSourceProperty]
		public string SkirmisherRoleFormation3Label => GetRoleFormationLabel((TroopRole)3, 2);

		[DataSourceProperty]
		public string SkirmisherRoleFormation4Label => GetRoleFormationLabel((TroopRole)3, 3);

		[DataSourceProperty]
		public string SkirmisherRoleFormation5Label => GetRoleFormationLabel((TroopRole)3, 4);

		[DataSourceProperty]
		public string SkirmisherRoleFormation6Label => GetRoleFormationLabel((TroopRole)3, 5);

		[DataSourceProperty]
		public string SkirmisherRoleFormation7Label => GetRoleFormationLabel((TroopRole)3, 6);

		[DataSourceProperty]
		public string SkirmisherRoleFormation8Label => GetRoleFormationLabel((TroopRole)3, 7);

		[DataSourceProperty]
		public string FootArcherRoleFormation1Label => GetRoleFormationLabel((TroopRole)4, 0);

		[DataSourceProperty]
		public string FootArcherRoleFormation2Label => GetRoleFormationLabel((TroopRole)4, 1);

		[DataSourceProperty]
		public string FootArcherRoleFormation3Label => GetRoleFormationLabel((TroopRole)4, 2);

		[DataSourceProperty]
		public string FootArcherRoleFormation4Label => GetRoleFormationLabel((TroopRole)4, 3);

		[DataSourceProperty]
		public string FootArcherRoleFormation5Label => GetRoleFormationLabel((TroopRole)4, 4);

		[DataSourceProperty]
		public string FootArcherRoleFormation6Label => GetRoleFormationLabel((TroopRole)4, 5);

		[DataSourceProperty]
		public string FootArcherRoleFormation7Label => GetRoleFormationLabel((TroopRole)4, 6);

		[DataSourceProperty]
		public string FootArcherRoleFormation8Label => GetRoleFormationLabel((TroopRole)4, 7);

		[DataSourceProperty]
		public string CrossbowmanRoleFormation1Label => GetRoleFormationLabel((TroopRole)5, 0);

		[DataSourceProperty]
		public string CrossbowmanRoleFormation2Label => GetRoleFormationLabel((TroopRole)5, 1);

		[DataSourceProperty]
		public string CrossbowmanRoleFormation3Label => GetRoleFormationLabel((TroopRole)5, 2);

		[DataSourceProperty]
		public string CrossbowmanRoleFormation4Label => GetRoleFormationLabel((TroopRole)5, 3);

		[DataSourceProperty]
		public string CrossbowmanRoleFormation5Label => GetRoleFormationLabel((TroopRole)5, 4);

		[DataSourceProperty]
		public string CrossbowmanRoleFormation6Label => GetRoleFormationLabel((TroopRole)5, 5);

		[DataSourceProperty]
		public string CrossbowmanRoleFormation7Label => GetRoleFormationLabel((TroopRole)5, 6);

		[DataSourceProperty]
		public string CrossbowmanRoleFormation8Label => GetRoleFormationLabel((TroopRole)5, 7);

		[DataSourceProperty]
		public string MeleeCavalryRoleFormation1Label => GetRoleFormationLabel((TroopRole)6, 0);

		[DataSourceProperty]
		public string MeleeCavalryRoleFormation2Label => GetRoleFormationLabel((TroopRole)6, 1);

		[DataSourceProperty]
		public string MeleeCavalryRoleFormation3Label => GetRoleFormationLabel((TroopRole)6, 2);

		[DataSourceProperty]
		public string MeleeCavalryRoleFormation4Label => GetRoleFormationLabel((TroopRole)6, 3);

		[DataSourceProperty]
		public string MeleeCavalryRoleFormation5Label => GetRoleFormationLabel((TroopRole)6, 4);

		[DataSourceProperty]
		public string MeleeCavalryRoleFormation6Label => GetRoleFormationLabel((TroopRole)6, 5);

		[DataSourceProperty]
		public string MeleeCavalryRoleFormation7Label => GetRoleFormationLabel((TroopRole)6, 6);

		[DataSourceProperty]
		public string MeleeCavalryRoleFormation8Label => GetRoleFormationLabel((TroopRole)6, 7);

		[DataSourceProperty]
		public string HorseArcherRoleFormation1Label => GetRoleFormationLabel((TroopRole)7, 0);

		[DataSourceProperty]
		public string HorseArcherRoleFormation2Label => GetRoleFormationLabel((TroopRole)7, 1);

		[DataSourceProperty]
		public string HorseArcherRoleFormation3Label => GetRoleFormationLabel((TroopRole)7, 2);

		[DataSourceProperty]
		public string HorseArcherRoleFormation4Label => GetRoleFormationLabel((TroopRole)7, 3);

		[DataSourceProperty]
		public string HorseArcherRoleFormation5Label => GetRoleFormationLabel((TroopRole)7, 4);

		[DataSourceProperty]
		public string HorseArcherRoleFormation6Label => GetRoleFormationLabel((TroopRole)7, 5);

		[DataSourceProperty]
		public string HorseArcherRoleFormation7Label => GetRoleFormationLabel((TroopRole)7, 6);

		[DataSourceProperty]
		public string HorseArcherRoleFormation8Label => GetRoleFormationLabel((TroopRole)7, 7);

		[DataSourceProperty]
		public bool IsCustomEditorVisible => IsFormationBadgeVisible;

		[DataSourceProperty]
		public bool IsFormationEditorVisible
		{
			get
			{
				if (IsFormationBadgeVisible)
				{
					Settings instance = GlobalSettings<Settings>.Instance;
					if (instance == null || instance.ShowAdvancedFormationEditor)
					{
						return base.ViewModel?.IsSelected ?? false;
					}
				}
				return false;
			}
		}

		[DataSourceProperty]
		public bool HasAdvancedFormationPlan
		{
			get
			{
				TroopDeploymentPlan plan;
				if (AdvancedPlansEnabled && base.ViewModel?.Character != null)
				{
					return FormationAssignmentStore.TryGetDeploymentPlan(base.ViewModel.Character.StringId, out plan);
				}
				return false;
			}
		}

		[DataSourceProperty]
		public bool CanRestoreAdvancedFormationPlan
		{
			get
			{
				if (AdvancedPlansEnabled && base.ViewModel?.Character != null)
				{
					return FormationAssignmentStore.CanRestoreDeploymentPlan(base.ViewModel.Character.StringId);
				}
				return false;
			}
		}

		[DataSourceProperty]
		public bool HasUnsavedAdvancedFormationChanges => _draft.IsDirty;

		[DataSourceProperty]
		public bool IsFormationEditorSliderVisible
		{
			get
			{
				if (IsFormationEditorVisible)
				{
					return _selectedCustomFormationIndex >= 0;
				}
				return false;
			}
		}

		[DataSourceProperty]
		public bool IsCustomFormationSelected => _selectedCustomFormationIndex >= 0;

		[DataSourceProperty]
		public bool IsFormation1Selected => _selectedCustomFormationIndex == 0;

		[DataSourceProperty]
		public bool IsFormation2Selected => _selectedCustomFormationIndex == 1;

		[DataSourceProperty]
		public bool IsFormation3Selected => _selectedCustomFormationIndex == 2;

		[DataSourceProperty]
		public bool IsFormation4Selected => _selectedCustomFormationIndex == 3;

		[DataSourceProperty]
		public bool IsFormation5Selected => _selectedCustomFormationIndex == 4;

		[DataSourceProperty]
		public bool IsFormation6Selected => _selectedCustomFormationIndex == 5;

		[DataSourceProperty]
		public bool IsFormation7Selected => _selectedCustomFormationIndex == 6;

		[DataSourceProperty]
		public bool IsFormation8Selected => _selectedCustomFormationIndex == 7;

		[DataSourceProperty]
		public bool IsFormation1Active => _editorFormationActive[0];

		[DataSourceProperty]
		public bool IsFormation2Active => _editorFormationActive[1];

		[DataSourceProperty]
		public bool IsFormation3Active => _editorFormationActive[2];

		[DataSourceProperty]
		public bool IsFormation4Active => _editorFormationActive[3];

		[DataSourceProperty]
		public bool IsFormation5Active => _editorFormationActive[4];

		[DataSourceProperty]
		public bool IsFormation6Active => _editorFormationActive[5];

		[DataSourceProperty]
		public bool IsFormation7Active => _editorFormationActive[6];

		[DataSourceProperty]
		public bool IsFormation8Active => _editorFormationActive[7];

		[DataSourceProperty]
		public string Formation1ButtonLabel => GetFormationButtonLabel(0, "I");

		[DataSourceProperty]
		public string Formation2ButtonLabel => GetFormationButtonLabel(1, "II");

		[DataSourceProperty]
		public string Formation3ButtonLabel => GetFormationButtonLabel(2, "III");

		[DataSourceProperty]
		public string Formation4ButtonLabel => GetFormationButtonLabel(3, "IV");

		[DataSourceProperty]
		public string Formation5ButtonLabel => GetFormationButtonLabel(4, "V");

		[DataSourceProperty]
		public string Formation6ButtonLabel => GetFormationButtonLabel(5, "VI");

		[DataSourceProperty]
		public string Formation7ButtonLabel => GetFormationButtonLabel(6, "VII");

		[DataSourceProperty]
		public string Formation8ButtonLabel => GetFormationButtonLabel(7, "VIII");

		[DataSourceProperty]
		public string Formation1TargetText => _targetTexts[0];

		[DataSourceProperty]
		public string Formation2TargetText => _targetTexts[1];

		[DataSourceProperty]
		public string Formation3TargetText => _targetTexts[2];

		[DataSourceProperty]
		public string Formation4TargetText => _targetTexts[3];

		[DataSourceProperty]
		public string Formation5TargetText => _targetTexts[4];

		[DataSourceProperty]
		public string Formation6TargetText => _targetTexts[5];

		[DataSourceProperty]
		public string Formation7TargetText => _targetTexts[6];

		[DataSourceProperty]
		public string Formation8TargetText => _targetTexts[7];

		[DataSourceProperty]
		public string Formation1AssignedText => _assignedTexts[0];

		[DataSourceProperty]
		public string Formation2AssignedText => _assignedTexts[1];

		[DataSourceProperty]
		public string Formation3AssignedText => _assignedTexts[2];

		[DataSourceProperty]
		public string Formation4AssignedText => _assignedTexts[3];

		[DataSourceProperty]
		public string Formation5AssignedText => _assignedTexts[4];

		[DataSourceProperty]
		public string Formation6AssignedText => _assignedTexts[5];

		[DataSourceProperty]
		public string Formation7AssignedText => _assignedTexts[6];

		[DataSourceProperty]
		public string Formation8AssignedText => _assignedTexts[7];

		[DataSourceProperty]
		public string Formation1WeightText => _weightTexts[0];

		[DataSourceProperty]
		public string Formation2WeightText => _weightTexts[1];

		[DataSourceProperty]
		public string Formation3WeightText => _weightTexts[2];

		[DataSourceProperty]
		public string Formation4WeightText => _weightTexts[3];

		[DataSourceProperty]
		public string Formation5WeightText => _weightTexts[4];

		[DataSourceProperty]
		public string Formation6WeightText => _weightTexts[5];

		[DataSourceProperty]
		public string Formation7WeightText => _weightTexts[6];

		[DataSourceProperty]
		public string Formation8WeightText => _weightTexts[7];

		[DataSourceProperty]
		public string SelectedFormationLabel
		{
			get
			{
				if (_selectedCustomFormationIndex >= 0)
				{
					return "Formation " + Labels[_selectedCustomFormationIndex + 1] + " — click its numeral again to remove";
				}
				return "Select an active formation, or click + Add formation";
			}
		}

		[DataSourceProperty]
		public string SelectedFormationTargetText => $"Target: {SelectedFormationTarget}";

		[DataSourceProperty]
		public string SelectedFormationWeightText => $"Weight: {SelectedFormationWeight}";

		[DataSourceProperty]
		public int SelectedFormationTargetMax => GetSelectedTargetMaximum();

		[DataSourceProperty]
		public int SelectedFormationTarget
		{
			get
			{
				if (_selectedCustomFormationIndex >= 0)
				{
					return _draft.GetTarget(_selectedCustomFormationIndex);
				}
				return 0;
			}
			set
			{
				SetSelectedTarget(value);
			}
		}

		[DataSourceProperty]
		public int SelectedFormationWeight
		{
			get
			{
				if (_selectedCustomFormationIndex >= 0)
				{
					return _draft.GetWeight(_selectedCustomFormationIndex);
				}
				return 0;
			}
			set
			{
				SetSelectedWeight(value);
			}
		}

		private bool IsPlayerHeroRow
		{
			get
			{
				CharacterObject characterObject = base.ViewModel?.Character;
				if (characterObject != null && characterObject.HeroObject != null)
				{
					return characterObject.HeroObject == Hero.MainHero;
				}
				return false;
			}
		}

		private static bool AdvancedPlansEnabled => GlobalSettings<Settings>.Instance?.ShowAdvancedFormationEditor ?? true;

		public PartyCharacterVMMixin(PartyCharacterVM vm)
			: base(vm)
		{
			Instances.Remove(vm);
			Instances.Add(vm, this);
			ActiveMixins.RemoveAll((WeakReference<PartyCharacterVMMixin> reference) => !reference.TryGetTarget(out PartyCharacterVMMixin target) || target == this);
			ActiveMixins.Add(new WeakReference<PartyCharacterVMMixin>(this));
			vm.PropertyChanged += OnViewModelPropertyChanged;
			_lastKnownSelected = vm.IsSelected;
			Refresh();
		}

		[DataSourceMethod]
		public void ExecuteCycleFormation()
		{
			CharacterObject characterObject = base.ViewModel?.Character;
			if (characterObject == null)
			{
				return;
			}
			if (AdvancedPlansEnabled && FormationAssignmentStore.TryGetDeploymentPlan(characterObject.StringId, out TroopDeploymentPlan _))
			{
				ShowCustomPlanHint();
				return;
			}
			int num = FormationAssignmentStore.GetAssignment(characterObject.StringId) + 1;
			if (num > 7)
			{
				num = -1;
			}
			if (num < 0)
			{
				FormationAssignmentStore.ClearAssignment(characterObject.StringId);
			}
			else
			{
				FormationAssignmentStore.SetAssignment(characterObject.StringId, num);
			}
			FormationAssignmentStore.Save();
			Refresh();
		}

		[DataSourceMethod]
		public void ExecuteCycleLightInfantryRoleSplit()
		{
			CycleRoleSplit((TroopRole)0);
		}

		[DataSourceMethod]
		public void ExecuteCycleShieldInfantryRoleSplit()
		{
			CycleRoleSplit((TroopRole)1);
		}

		[DataSourceMethod]
		public void ExecuteCycleShockInfantryRoleSplit()
		{
			CycleRoleSplit((TroopRole)2);
		}

		[DataSourceMethod]
		public void ExecuteCyclePikeInfantryRoleSplit()
		{
			CycleRoleSplit((TroopRole)8);
		}

		[DataSourceMethod]
		public void ExecuteCycleSkirmisherRoleSplit()
		{
			CycleRoleSplit((TroopRole)3);
		}

		[DataSourceMethod]
		public void ExecuteCycleFootArcherRoleSplit()
		{
			CycleRoleSplit((TroopRole)4);
		}

		[DataSourceMethod]
		public void ExecuteCycleCrossbowmanRoleSplit()
		{
			CycleRoleSplit((TroopRole)5);
		}

		[DataSourceMethod]
		public void ExecuteCycleMeleeCavalryRoleSplit()
		{
			CycleRoleSplit((TroopRole)6);
		}

		[DataSourceMethod]
		public void ExecuteCycleHorseArcherRoleSplit()
		{
			CycleRoleSplit((TroopRole)7);
		}

		[DataSourceMethod]
		public void ExecuteToggleLightInfantryRoleFormation1()
		{
			ToggleRoleFormation((TroopRole)0, 0);
		}

		[DataSourceMethod]
		public void ExecuteToggleLightInfantryRoleFormation2()
		{
			ToggleRoleFormation((TroopRole)0, 1);
		}

		[DataSourceMethod]
		public void ExecuteToggleLightInfantryRoleFormation3()
		{
			ToggleRoleFormation((TroopRole)0, 2);
		}

		[DataSourceMethod]
		public void ExecuteToggleLightInfantryRoleFormation4()
		{
			ToggleRoleFormation((TroopRole)0, 3);
		}

		[DataSourceMethod]
		public void ExecuteToggleLightInfantryRoleFormation5()
		{
			ToggleRoleFormation((TroopRole)0, 4);
		}

		[DataSourceMethod]
		public void ExecuteToggleLightInfantryRoleFormation6()
		{
			ToggleRoleFormation((TroopRole)0, 5);
		}

		[DataSourceMethod]
		public void ExecuteToggleLightInfantryRoleFormation7()
		{
			ToggleRoleFormation((TroopRole)0, 6);
		}

		[DataSourceMethod]
		public void ExecuteToggleLightInfantryRoleFormation8()
		{
			ToggleRoleFormation((TroopRole)0, 7);
		}

		[DataSourceMethod]
		public void ExecuteToggleShieldInfantryRoleFormation1()
		{
			ToggleRoleFormation((TroopRole)1, 0);
		}

		[DataSourceMethod]
		public void ExecuteToggleShieldInfantryRoleFormation2()
		{
			ToggleRoleFormation((TroopRole)1, 1);
		}

		[DataSourceMethod]
		public void ExecuteToggleShieldInfantryRoleFormation3()
		{
			ToggleRoleFormation((TroopRole)1, 2);
		}

		[DataSourceMethod]
		public void ExecuteToggleShieldInfantryRoleFormation4()
		{
			ToggleRoleFormation((TroopRole)1, 3);
		}

		[DataSourceMethod]
		public void ExecuteToggleShieldInfantryRoleFormation5()
		{
			ToggleRoleFormation((TroopRole)1, 4);
		}

		[DataSourceMethod]
		public void ExecuteToggleShieldInfantryRoleFormation6()
		{
			ToggleRoleFormation((TroopRole)1, 5);
		}

		[DataSourceMethod]
		public void ExecuteToggleShieldInfantryRoleFormation7()
		{
			ToggleRoleFormation((TroopRole)1, 6);
		}

		[DataSourceMethod]
		public void ExecuteToggleShieldInfantryRoleFormation8()
		{
			ToggleRoleFormation((TroopRole)1, 7);
		}

		[DataSourceMethod]
		public void ExecuteToggleShockInfantryRoleFormation1()
		{
			ToggleRoleFormation((TroopRole)2, 0);
		}

		[DataSourceMethod]
		public void ExecuteToggleShockInfantryRoleFormation2()
		{
			ToggleRoleFormation((TroopRole)2, 1);
		}

		[DataSourceMethod]
		public void ExecuteToggleShockInfantryRoleFormation3()
		{
			ToggleRoleFormation((TroopRole)2, 2);
		}

		[DataSourceMethod]
		public void ExecuteToggleShockInfantryRoleFormation4()
		{
			ToggleRoleFormation((TroopRole)2, 3);
		}

		[DataSourceMethod]
		public void ExecuteToggleShockInfantryRoleFormation5()
		{
			ToggleRoleFormation((TroopRole)2, 4);
		}

		[DataSourceMethod]
		public void ExecuteToggleShockInfantryRoleFormation6()
		{
			ToggleRoleFormation((TroopRole)2, 5);
		}

		[DataSourceMethod]
		public void ExecuteToggleShockInfantryRoleFormation7()
		{
			ToggleRoleFormation((TroopRole)2, 6);
		}

		[DataSourceMethod]
		public void ExecuteToggleShockInfantryRoleFormation8()
		{
			ToggleRoleFormation((TroopRole)2, 7);
		}

		[DataSourceMethod]
		public void ExecuteTogglePikeInfantryRoleFormation1()
		{
			ToggleRoleFormation((TroopRole)8, 0);
		}

		[DataSourceMethod]
		public void ExecuteTogglePikeInfantryRoleFormation2()
		{
			ToggleRoleFormation((TroopRole)8, 1);
		}

		[DataSourceMethod]
		public void ExecuteTogglePikeInfantryRoleFormation3()
		{
			ToggleRoleFormation((TroopRole)8, 2);
		}

		[DataSourceMethod]
		public void ExecuteTogglePikeInfantryRoleFormation4()
		{
			ToggleRoleFormation((TroopRole)8, 3);
		}

		[DataSourceMethod]
		public void ExecuteTogglePikeInfantryRoleFormation5()
		{
			ToggleRoleFormation((TroopRole)8, 4);
		}

		[DataSourceMethod]
		public void ExecuteTogglePikeInfantryRoleFormation6()
		{
			ToggleRoleFormation((TroopRole)8, 5);
		}

		[DataSourceMethod]
		public void ExecuteTogglePikeInfantryRoleFormation7()
		{
			ToggleRoleFormation((TroopRole)8, 6);
		}

		[DataSourceMethod]
		public void ExecuteTogglePikeInfantryRoleFormation8()
		{
			ToggleRoleFormation((TroopRole)8, 7);
		}

		[DataSourceMethod]
		public void ExecuteToggleSkirmisherRoleFormation1()
		{
			ToggleRoleFormation((TroopRole)3, 0);
		}

		[DataSourceMethod]
		public void ExecuteToggleSkirmisherRoleFormation2()
		{
			ToggleRoleFormation((TroopRole)3, 1);
		}

		[DataSourceMethod]
		public void ExecuteToggleSkirmisherRoleFormation3()
		{
			ToggleRoleFormation((TroopRole)3, 2);
		}

		[DataSourceMethod]
		public void ExecuteToggleSkirmisherRoleFormation4()
		{
			ToggleRoleFormation((TroopRole)3, 3);
		}

		[DataSourceMethod]
		public void ExecuteToggleSkirmisherRoleFormation5()
		{
			ToggleRoleFormation((TroopRole)3, 4);
		}

		[DataSourceMethod]
		public void ExecuteToggleSkirmisherRoleFormation6()
		{
			ToggleRoleFormation((TroopRole)3, 5);
		}

		[DataSourceMethod]
		public void ExecuteToggleSkirmisherRoleFormation7()
		{
			ToggleRoleFormation((TroopRole)3, 6);
		}

		[DataSourceMethod]
		public void ExecuteToggleSkirmisherRoleFormation8()
		{
			ToggleRoleFormation((TroopRole)3, 7);
		}

		[DataSourceMethod]
		public void ExecuteToggleFootArcherRoleFormation1()
		{
			ToggleRoleFormation((TroopRole)4, 0);
		}

		[DataSourceMethod]
		public void ExecuteToggleFootArcherRoleFormation2()
		{
			ToggleRoleFormation((TroopRole)4, 1);
		}

		[DataSourceMethod]
		public void ExecuteToggleFootArcherRoleFormation3()
		{
			ToggleRoleFormation((TroopRole)4, 2);
		}

		[DataSourceMethod]
		public void ExecuteToggleFootArcherRoleFormation4()
		{
			ToggleRoleFormation((TroopRole)4, 3);
		}

		[DataSourceMethod]
		public void ExecuteToggleFootArcherRoleFormation5()
		{
			ToggleRoleFormation((TroopRole)4, 4);
		}

		[DataSourceMethod]
		public void ExecuteToggleFootArcherRoleFormation6()
		{
			ToggleRoleFormation((TroopRole)4, 5);
		}

		[DataSourceMethod]
		public void ExecuteToggleFootArcherRoleFormation7()
		{
			ToggleRoleFormation((TroopRole)4, 6);
		}

		[DataSourceMethod]
		public void ExecuteToggleFootArcherRoleFormation8()
		{
			ToggleRoleFormation((TroopRole)4, 7);
		}

		[DataSourceMethod]
		public void ExecuteToggleCrossbowmanRoleFormation1()
		{
			ToggleRoleFormation((TroopRole)5, 0);
		}

		[DataSourceMethod]
		public void ExecuteToggleCrossbowmanRoleFormation2()
		{
			ToggleRoleFormation((TroopRole)5, 1);
		}

		[DataSourceMethod]
		public void ExecuteToggleCrossbowmanRoleFormation3()
		{
			ToggleRoleFormation((TroopRole)5, 2);
		}

		[DataSourceMethod]
		public void ExecuteToggleCrossbowmanRoleFormation4()
		{
			ToggleRoleFormation((TroopRole)5, 3);
		}

		[DataSourceMethod]
		public void ExecuteToggleCrossbowmanRoleFormation5()
		{
			ToggleRoleFormation((TroopRole)5, 4);
		}

		[DataSourceMethod]
		public void ExecuteToggleCrossbowmanRoleFormation6()
		{
			ToggleRoleFormation((TroopRole)5, 5);
		}

		[DataSourceMethod]
		public void ExecuteToggleCrossbowmanRoleFormation7()
		{
			ToggleRoleFormation((TroopRole)5, 6);
		}

		[DataSourceMethod]
		public void ExecuteToggleCrossbowmanRoleFormation8()
		{
			ToggleRoleFormation((TroopRole)5, 7);
		}

		[DataSourceMethod]
		public void ExecuteToggleMeleeCavalryRoleFormation1()
		{
			ToggleRoleFormation((TroopRole)6, 0);
		}

		[DataSourceMethod]
		public void ExecuteToggleMeleeCavalryRoleFormation2()
		{
			ToggleRoleFormation((TroopRole)6, 1);
		}

		[DataSourceMethod]
		public void ExecuteToggleMeleeCavalryRoleFormation3()
		{
			ToggleRoleFormation((TroopRole)6, 2);
		}

		[DataSourceMethod]
		public void ExecuteToggleMeleeCavalryRoleFormation4()
		{
			ToggleRoleFormation((TroopRole)6, 3);
		}

		[DataSourceMethod]
		public void ExecuteToggleMeleeCavalryRoleFormation5()
		{
			ToggleRoleFormation((TroopRole)6, 4);
		}

		[DataSourceMethod]
		public void ExecuteToggleMeleeCavalryRoleFormation6()
		{
			ToggleRoleFormation((TroopRole)6, 5);
		}

		[DataSourceMethod]
		public void ExecuteToggleMeleeCavalryRoleFormation7()
		{
			ToggleRoleFormation((TroopRole)6, 6);
		}

		[DataSourceMethod]
		public void ExecuteToggleMeleeCavalryRoleFormation8()
		{
			ToggleRoleFormation((TroopRole)6, 7);
		}

		[DataSourceMethod]
		public void ExecuteToggleHorseArcherRoleFormation1()
		{
			ToggleRoleFormation((TroopRole)7, 0);
		}

		[DataSourceMethod]
		public void ExecuteToggleHorseArcherRoleFormation2()
		{
			ToggleRoleFormation((TroopRole)7, 1);
		}

		[DataSourceMethod]
		public void ExecuteToggleHorseArcherRoleFormation3()
		{
			ToggleRoleFormation((TroopRole)7, 2);
		}

		[DataSourceMethod]
		public void ExecuteToggleHorseArcherRoleFormation4()
		{
			ToggleRoleFormation((TroopRole)7, 3);
		}

		[DataSourceMethod]
		public void ExecuteToggleHorseArcherRoleFormation5()
		{
			ToggleRoleFormation((TroopRole)7, 4);
		}

		[DataSourceMethod]
		public void ExecuteToggleHorseArcherRoleFormation6()
		{
			ToggleRoleFormation((TroopRole)7, 5);
		}

		[DataSourceMethod]
		public void ExecuteToggleHorseArcherRoleFormation7()
		{
			ToggleRoleFormation((TroopRole)7, 6);
		}

		[DataSourceMethod]
		public void ExecuteToggleHorseArcherRoleFormation8()
		{
			ToggleRoleFormation((TroopRole)7, 7);
		}

		[DataSourceMethod]
		public void ExecuteSaveRolePlan()
		{
			SaveRolePlan();
		}

		[DataSourceMethod]
		public void ExecuteClearRolePlan()
		{
			ClearRolePlan();
		}

		[DataSourceMethod]
		public void ExecuteRestoreRolePlan()
		{
			RestoreRolePlan();
		}

		private string GetRoleFormationLabel(TroopRole role, int formationIndex)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if (!IsRoleFormationMarked(role, formationIndex))
			{
				return string.Empty;
			}
			return "✓";
		}

		private bool IsRoleFormationMarked(TroopRole role, int formationIndex)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return GetDisplayedRoleAssignments(role).Contains(formationIndex);
		}

		private bool IsRoleGridVisible(TroopRole role)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return GetDisplayedRoleSplitEnabled(role);
		}

		private string GetRoleSplitLabel(TroopRole role)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if (!GetDisplayedRoleSplitEnabled(role))
			{
				return "Default";
			}
			return "Split";
		}

		private void CycleRoleSplit(TroopRole role)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			SetRoleSplitEnabledDraft(role, !GetDisplayedRoleSplitEnabled(role));
			RefreshRolePlanBindings();
		}

		private void ToggleRoleFormation(TroopRole role, int formationIndex)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			List<int> list = GetDisplayedRoleAssignments(role).ToList();
			if (GetDisplayedRoleSplitEnabled(role))
			{
				if (!list.Remove(formationIndex))
				{
					list.Add(formationIndex);
				}
				SetRolePlanDraft(role, list);
				RefreshRolePlanBindings();
			}
		}

		private int[] GetDisplayedRoleAssignments(TroopRole role)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			if (!_rolePlanDraft.TryGetValue(role, out int[] value))
			{
				return FormationAssignmentStore.GetRoleAssignments(role);
			}
			return value;
		}

		private bool GetDisplayedRoleSplitEnabled(TroopRole role)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			if (!_roleSplitEnabledDraft.TryGetValue(role, out var value))
			{
				return FormationAssignmentStore.GetRoleSplitEnabled(role);
			}
			return value;
		}

		private void SetRolePlanDraft(TroopRole role, IEnumerable<int> assignments)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			_rolePlanDraft[role] = (from index in assignments.Where(FormationPlanNormalizer.IsValidFormationIndex).Distinct()
				orderby index
				select index).ToArray();
			_hasRolePlanDraftChanges = true;
		}

		private void SetRoleSplitEnabledDraft(TroopRole role, bool enabled)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			_roleSplitEnabledDraft[role] = enabled;
			_hasRolePlanDraftChanges = true;
		}

		private void SaveRolePlan()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			if (!_hasRolePlanDraftChanges)
			{
				return;
			}
			TroopRole[] rolePlanRoles = RolePlanRoles;
			foreach (TroopRole val in rolePlanRoles)
			{
				int[] value;
				bool flag = _rolePlanDraft.TryGetValue(val, out value);
				bool value2;
				bool flag2 = _roleSplitEnabledDraft.TryGetValue(val, out value2);
				if (flag || flag2)
				{
					if (!(flag2 ? value2 : FormationAssignmentStore.GetRoleSplitEnabled(val)))
					{
						FormationAssignmentStore.ClearRoleAssignment(val);
						continue;
					}
					FormationAssignmentStore.SetRoleSplitEnabled(val, enabled: true);
					FormationAssignmentStore.SetRoleAssignments(val, flag ? ((IEnumerable<int>)value) : ((IEnumerable<int>)FormationAssignmentStore.GetRoleAssignments(val)));
				}
			}
			FormationAssignmentStore.Save();
			_rolePlanDraft.Clear();
			_roleSplitEnabledDraft.Clear();
			_hasRolePlanDraftChanges = false;
			RefreshRolePlanBindings();
		}

		private void ClearRolePlan()
		{
			_rolePlanDraft.Clear();
			_roleSplitEnabledDraft.Clear();
			_hasRolePlanDraftChanges = false;
			FormationAssignmentStore.ClearRoleAssignments();
			FormationAssignmentStore.Save();
			RefreshRolePlanBindings();
		}

		private void RestoreRolePlan()
		{
			_rolePlanDraft.Clear();
			_roleSplitEnabledDraft.Clear();
			_hasRolePlanDraftChanges = false;
			if (FormationAssignmentStore.RestoreRoleAssignments())
			{
				FormationAssignmentStore.Save();
			}
			RefreshRolePlanBindings();
		}

		private void RefreshRolePlanBindings()
		{
			base.OnPropertyChanged("IsRolePlanBadgeVisible");
			base.OnPropertyChanged("HasUnsavedRolePlanChanges");
			base.OnPropertyChanged("HasRolePlan");
			base.OnPropertyChanged("CanRestoreRolePlan");
			string[] array = new string[9] { "LightInfantry", "ShieldInfantry", "ShockInfantry", "PikeInfantry", "Skirmisher", "FootArcher", "Crossbowman", "MeleeCavalry", "HorseArcher" };
			foreach (string text in array)
			{
				base.OnPropertyChanged(text + "RoleSplitLabel");
				base.OnPropertyChanged("Is" + text + "RoleGridVisible");
				for (int j = 1; j <= 8; j++)
				{
					base.OnPropertyChanged($"{text}RoleFormation{j}Label");
					base.OnPropertyChanged($"Is{text}RoleFormation{j}Selected");
				}
			}
		}

		[DataSourceMethod]
		public void ExecuteClearFormation()
		{
			CharacterObject characterObject = base.ViewModel?.Character;
			if (characterObject != null)
			{
				if (AdvancedPlansEnabled && FormationAssignmentStore.TryGetDeploymentPlan(characterObject.StringId, out TroopDeploymentPlan _))
				{
					ShowCustomPlanHint();
					return;
				}
				FormationAssignmentStore.ClearAssignment(characterObject.StringId);
				FormationAssignmentStore.Save();
				Refresh();
			}
		}

		[DataSourceMethod]
		public void ExecuteCycleSecondaryFormation()
		{
			CharacterObject characterObject = base.ViewModel?.Character;
			if (characterObject == null)
			{
				return;
			}
			if (AdvancedPlansEnabled && FormationAssignmentStore.TryGetDeploymentPlan(characterObject.StringId, out TroopDeploymentPlan _))
			{
				FormationAssignmentStore.ClearDeploymentPlan(characterObject.StringId);
				FormationAssignmentStore.Save();
				Refresh();
				return;
			}
			int assignment = FormationAssignmentStore.GetAssignment(characterObject.StringId);
			if (assignment < 0)
			{
				FormationAssignmentStore.SetAssignment(characterObject.StringId, 0);
			}
			else
			{
				int num = FormationAssignmentStore.GetSecondaryAssignment(characterObject.StringId);
				do
				{
					num++;
					if (num > 7)
					{
						num = -1;
						break;
					}
				}
				while (num == assignment);
				if (num < 0)
				{
					FormationAssignmentStore.ClearSecondaryAssignment(characterObject.StringId);
				}
				else
				{
					FormationAssignmentStore.SetSecondaryAssignment(characterObject.StringId, num);
				}
			}
			FormationAssignmentStore.Save();
			Refresh();
		}

		[DataSourceMethod]
		public void ExecuteClearSecondaryFormation()
		{
			CharacterObject characterObject = base.ViewModel?.Character;
			if (characterObject != null)
			{
				if (AdvancedPlansEnabled && FormationAssignmentStore.TryGetDeploymentPlan(characterObject.StringId, out TroopDeploymentPlan _))
				{
					FormationAssignmentStore.ClearDeploymentPlan(characterObject.StringId);
				}
				else
				{
					FormationAssignmentStore.ClearSecondaryAssignment(characterObject.StringId);
				}
				FormationAssignmentStore.Save();
				Refresh();
			}
		}

		[DataSourceMethod]
		public void ExecuteClearAdvancedFormationPlan()
		{
			if (base.ViewModel?.Character != null)
			{
				EnsureDraftInitialized();
				_draft.Clear();
				_selectedCustomFormationIndex = -1;
				MarkDraftChanged();
				RefreshEditorValues();
			}
		}

		[DataSourceMethod]
		public void ExecuteRestoreAdvancedFormationPlan()
		{
			CharacterObject characterObject = base.ViewModel?.Character;
			if (characterObject != null && FormationAssignmentStore.TryGetArchivedDeploymentPlanCopy(characterObject.StringId, out TroopDeploymentPlan planCopy))
			{
				_draft.Load(planCopy);
				_selectedCustomFormationIndex = -1;
				MarkDraftChanged();
				RefreshEditorValues();
			}
		}

		[DataSourceMethod]
		public void ExecuteSaveAdvancedFormationPlan()
		{
			CharacterObject characterObject = base.ViewModel?.Character;
			if (characterObject != null && _draft.IsInitialized && _draft.IsDirty)
			{
				if (_draft.IsClearRequested)
				{
					FormationAssignmentStore.ClearDeploymentPlan(characterObject.StringId);
				}
				else
				{
					FormationAssignmentStore.SaveCustomPlan(characterObject.StringId, _draft.Targets, _draft.Weights);
				}
				FormationAssignmentStore.Save();
				_draft.MarkSaved();
				base.OnPropertyChanged("HasUnsavedAdvancedFormationChanges");
				Refresh();
			}
		}

		[DataSourceMethod]
		public void ExecuteSelectCustomFormation1()
		{
			SelectCustomFormation(0);
		}

		[DataSourceMethod]
		public void ExecuteSelectCustomFormation2()
		{
			SelectCustomFormation(1);
		}

		[DataSourceMethod]
		public void ExecuteSelectCustomFormation3()
		{
			SelectCustomFormation(2);
		}

		[DataSourceMethod]
		public void ExecuteSelectCustomFormation4()
		{
			SelectCustomFormation(3);
		}

		[DataSourceMethod]
		public void ExecuteSelectCustomFormation5()
		{
			SelectCustomFormation(4);
		}

		[DataSourceMethod]
		public void ExecuteSelectCustomFormation6()
		{
			SelectCustomFormation(5);
		}

		[DataSourceMethod]
		public void ExecuteSelectCustomFormation7()
		{
			SelectCustomFormation(6);
		}

		[DataSourceMethod]
		public void ExecuteSelectCustomFormation8()
		{
			SelectCustomFormation(7);
		}

		private void SelectCustomFormation(int formationIndex)
		{
			if (!_editorFormationActive[formationIndex])
			{
				SelectOrActivateFormation(formationIndex);
				return;
			}
			if (_selectedCustomFormationIndex == formationIndex)
			{
				DeactivateFormation(formationIndex);
				return;
			}
			_selectedCustomFormationIndex = formationIndex;
			NotifyEditorSelectionChanged();
		}

		private string GetFormationButtonLabel(int formationIndex, string numeral)
		{
			if (_editorFormationActive[formationIndex])
			{
				if (_selectedCustomFormationIndex != formationIndex)
				{
					return numeral;
				}
				return "×";
			}
			return "+";
		}

		private void SetSelectedTarget(int value)
		{
			if (_selectedCustomFormationIndex >= 0)
			{
				EnsureDraftInitialized();
				if (_draft.SetTarget(_selectedCustomFormationIndex, value, GetReadyTroopCount()))
				{
					MarkDraftChanged();
					RefreshEditorValues();
				}
			}
		}

		private void SetSelectedWeight(int value)
		{
			if (_selectedCustomFormationIndex >= 0)
			{
				EnsureDraftInitialized();
				if (_draft.SetWeight(_selectedCustomFormationIndex, value))
				{
					MarkDraftChanged();
					RefreshEditorValues();
				}
			}
		}

		private int GetSelectedTargetMaximum()
		{
			if (_selectedCustomFormationIndex < 0)
			{
				return 0;
			}
			return _draft.GetTargetMaximum(_selectedCustomFormationIndex, GetReadyTroopCount());
		}

		private int GetReadyTroopCount()
		{
			CharacterObject characterObject = base.ViewModel?.Character;
			TroopRoster troopRoster = MobileParty.MainParty?.MemberRoster;
			if (characterObject == null || troopRoster == null)
			{
				return 0;
			}
			for (int i = 0; i < troopRoster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = troopRoster.GetElementCopyAtIndex(i);
				if (elementCopyAtIndex.Character == characterObject)
				{
					return Math.Max(0, elementCopyAtIndex.Number - elementCopyAtIndex.WoundedNumber);
				}
			}
			return 0;
		}

		[DataSourceMethod]
		public void ExecuteSelectOrActivateFormation1()
		{
			SelectOrActivateFormation(0);
		}

		[DataSourceMethod]
		public void ExecuteSelectOrActivateFormation2()
		{
			SelectOrActivateFormation(1);
		}

		[DataSourceMethod]
		public void ExecuteSelectOrActivateFormation3()
		{
			SelectOrActivateFormation(2);
		}

		[DataSourceMethod]
		public void ExecuteSelectOrActivateFormation4()
		{
			SelectOrActivateFormation(3);
		}

		[DataSourceMethod]
		public void ExecuteSelectOrActivateFormation5()
		{
			SelectOrActivateFormation(4);
		}

		[DataSourceMethod]
		public void ExecuteSelectOrActivateFormation6()
		{
			SelectOrActivateFormation(5);
		}

		[DataSourceMethod]
		public void ExecuteSelectOrActivateFormation7()
		{
			SelectOrActivateFormation(6);
		}

		[DataSourceMethod]
		public void ExecuteSelectOrActivateFormation8()
		{
			SelectOrActivateFormation(7);
		}

		[DataSourceMethod]
		public void ExecuteDeactivateFormation1()
		{
			DeactivateFormation(0);
		}

		[DataSourceMethod]
		public void ExecuteDeactivateFormation2()
		{
			DeactivateFormation(1);
		}

		[DataSourceMethod]
		public void ExecuteDeactivateFormation3()
		{
			DeactivateFormation(2);
		}

		[DataSourceMethod]
		public void ExecuteDeactivateFormation4()
		{
			DeactivateFormation(3);
		}

		[DataSourceMethod]
		public void ExecuteDeactivateFormation5()
		{
			DeactivateFormation(4);
		}

		[DataSourceMethod]
		public void ExecuteDeactivateFormation6()
		{
			DeactivateFormation(5);
		}

		[DataSourceMethod]
		public void ExecuteDeactivateFormation7()
		{
			DeactivateFormation(6);
		}

		[DataSourceMethod]
		public void ExecuteDeactivateFormation8()
		{
			DeactivateFormation(7);
		}

		private void SelectOrActivateFormation(int formationIndex)
		{
			if (base.ViewModel?.Character != null)
			{
				EnsureDraftInitialized();
				if (!_editorFormationActive[formationIndex])
				{
					_draft.Activate(formationIndex);
					MarkDraftChanged();
					RefreshEditorValues();
				}
				_selectedCustomFormationIndex = formationIndex;
				NotifyEditorSelectionChanged();
			}
		}

		private void DeactivateFormation(int formationIndex)
		{
			if (base.ViewModel?.Character != null && _editorFormationActive[formationIndex])
			{
				EnsureDraftInitialized();
				_draft.Deactivate(formationIndex);
				MarkDraftChanged();
				if (_selectedCustomFormationIndex == formationIndex)
				{
					_selectedCustomFormationIndex = -1;
				}
				RefreshEditorValues();
			}
		}

		private int[] GetBaselineFormationIndices()
		{
			CharacterObject characterObject = base.ViewModel?.Character;
			if (characterObject == null)
			{
				return Array.Empty<int>();
			}
			int[] assignments = FormationAssignmentStore.GetAssignments(characterObject.StringId);
			if (assignments.Length == 0)
			{
				return new int[1] { FormationAssignmentResolver.GetDefaultFormationIndex(characterObject, GlobalSettings<Settings>.Instance) };
			}
			return assignments;
		}

		private int[] GetEditorActiveFormationIndices()
		{
			EnsureDraftInitialized();
			return Enumerable.Range(0, 8).Where(_draft.IsActive).ToArray();
		}

		private void EnsureDraftInitialized()
		{
			if (!_draft.IsInitialized)
			{
				CharacterObject characterObject = base.ViewModel?.Character;
				TroopDeploymentPlan plan = null;
				bool hasArchivedPlan = false;
				if (characterObject != null)
				{
					FormationAssignmentStore.TryGetDeploymentPlan(characterObject.StringId, out plan);
					hasArchivedPlan = FormationAssignmentStore.CanRestoreDeploymentPlan(characterObject.StringId);
				}
				_draft.Initialize(plan, GetBaselineFormationIndices(), hasArchivedPlan);
				base.OnPropertyChanged("HasUnsavedAdvancedFormationChanges");
			}
		}

		private void MarkDraftChanged()
		{
			base.OnPropertyChanged("HasUnsavedAdvancedFormationChanges");
			Settings instance = GlobalSettings<Settings>.Instance;
			if (instance != null && instance.AutosaveAdvancedFormationPlans)
			{
				ExecuteSaveAdvancedFormationPlan();
			}
			else if (!_autosaveHintShown)
			{
				_autosaveHintShown = true;
				InformationManager.DisplayMessage(new InformationMessage("[Formation Manager] Changes are drafts. Enable 'Autosave Advanced Formation Plans' in Settings > Multi-Formation Splits to save each edit immediately."));
			}
		}

		private void ResetDraft()
		{
			_draft.Reset();
			base.OnPropertyChanged("HasUnsavedAdvancedFormationChanges");
		}

		private static void ShowCustomPlanHint()
		{
			InformationManager.DisplayMessage(new InformationMessage("[Formation Manager] Advanced formation allocation: expand this troop row to edit it or clear it."));
		}

		private void Refresh()
		{
			bool flag = GlobalSettings<Settings>.Instance?.ModEnabled ?? true;
			bool flag2 = TroopControlScope.IsEligibleTroop(base.ViewModel?.Character);
			IsFormationBadgeVisible = flag && base.ViewModel != null && !base.ViewModel.IsPrisoner && flag2;
			if (base.ViewModel?.Character == null || !flag || !flag2)
			{
				FormationLabel = "—";
				SecondaryFormationLabel = "+";
				IsSecondaryFormationBadgeVisible = false;
				_selectedCustomFormationIndex = -1;
				RefreshEditorValues();
				return;
			}
			string stringId = base.ViewModel.Character.StringId;
			TroopDeploymentPlan plan;
			bool flag3 = AdvancedPlansEnabled && FormationAssignmentStore.TryGetDeploymentPlan(stringId, out plan);
			if (!flag3 && !_draft.IsDirty)
			{
				ResetDraft();
			}
			int assignment = FormationAssignmentStore.GetAssignment(stringId);
			int secondaryAssignment = FormationAssignmentStore.GetSecondaryAssignment(stringId);
			FormationLabel = (flag3 ? "A" : ((assignment >= 0 && assignment <= 7) ? Labels[assignment + 1] : "—"));
			SecondaryFormationLabel = ((secondaryAssignment >= 0 && secondaryAssignment <= 7) ? Labels[secondaryAssignment + 1] : "+");
			IsSecondaryFormationBadgeVisible = IsFormationBadgeVisible && !flag3 && assignment >= 0;
			base.OnPropertyChanged("HasAdvancedFormationPlan");
			base.OnPropertyChanged("CanRestoreAdvancedFormationPlan");
			RefreshEditorValues();
		}

		private void RefreshEditorValues()
		{
			CharacterObject characterObject = base.ViewModel?.Character;
			EnsureDraftInitialized();
			Dictionary<int, int> dictionary = ((characterObject == null) ? new Dictionary<int, int>() : FormationAssignmentResolver.GetAllocatedCustomPlanCounts(characterObject, GetReadyTroopCount(), _draft.Targets, _draft.Weights, GlobalSettings<Settings>.Instance));
			int[] editorActiveFormationIndices = GetEditorActiveFormationIndices();
			for (int i = 0; i < 8; i++)
			{
				_editorFormationActive[i] = editorActiveFormationIndices.Contains(i);
				int value;
				int num = (dictionary.TryGetValue(i, out value) ? value : 0);
				int target = _draft.GetTarget(i);
				int weight = _draft.GetWeight(i);
				_targetTexts[i] = (_editorFormationActive[i] ? $"Target {target}" : string.Empty);
				_assignedTexts[i] = (_editorFormationActive[i] ? $"Assigned {num}" : "+ Add formation");
				_weightTexts[i] = (_editorFormationActive[i] ? $"Weight {weight}" : string.Empty);
				base.OnPropertyChanged($"IsFormation{i + 1}Active");
				base.OnPropertyChanged($"Formation{i + 1}ButtonLabel");
				base.OnPropertyChanged($"Formation{i + 1}TargetText");
				base.OnPropertyChanged($"Formation{i + 1}AssignedText");
				base.OnPropertyChanged($"Formation{i + 1}WeightText");
			}
			if (_selectedCustomFormationIndex >= 0 && !_editorFormationActive[_selectedCustomFormationIndex])
			{
				_selectedCustomFormationIndex = -1;
			}
			NotifyEditorSelectionChanged();
		}

		private void NotifyEditorSelectionChanged()
		{
			base.OnPropertyChanged("IsCustomFormationSelected");
			for (int i = 1; i <= 8; i++)
			{
				base.OnPropertyChanged($"IsFormation{i}Selected");
			}
			base.OnPropertyChanged("SelectedFormationLabel");
			base.OnPropertyChanged("SelectedFormationTarget");
			base.OnPropertyChanged("SelectedFormationTargetMax");
			base.OnPropertyChanged("SelectedFormationTargetText");
			base.OnPropertyChanged("SelectedFormationWeight");
			base.OnPropertyChanged("SelectedFormationWeightText");
			base.OnPropertyChanged("IsCustomEditorVisible");
			base.OnPropertyChanged("IsFormationEditorVisible");
			base.OnPropertyChanged("IsFormationEditorSliderVisible");
		}

		private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == "IsSelected")
			{
				HandleSelectionChanged();
			}
		}

		internal static void NotifySelectionChanged(PartyCharacterVM viewModel)
		{
			ActiveMixins.RemoveAll((WeakReference<PartyCharacterVMMixin> reference) => !reference.TryGetTarget(out PartyCharacterVMMixin _));
			WeakReference<PartyCharacterVMMixin>? lastSelectedMixin = LastSelectedMixin;
			if (lastSelectedMixin != null && lastSelectedMixin.TryGetTarget(out PartyCharacterVMMixin target) && ((BaseViewModelMixin<PartyCharacterVM>)(object)target).ViewModel != null && Instances.TryGetValue(((BaseViewModelMixin<PartyCharacterVM>)(object)target).ViewModel, out PartyCharacterVMMixin value))
			{
				value.HandleSelectionChanged();
			}
			if (Instances.TryGetValue(viewModel, out PartyCharacterVMMixin value2))
			{
				value2.HandleSelectionChanged();
				LastSelectedMixin = new WeakReference<PartyCharacterVMMixin>(value2);
			}
		}

		private void HandleSelectionChanged()
		{
			bool flag = base.ViewModel?.IsSelected ?? false;
			if (_lastKnownSelected != flag)
			{
				_lastKnownSelected = flag;
				if (!flag)
				{
					_selectedCustomFormationIndex = -1;
				}
				base.OnPropertyChanged("IsFormationEditorVisible");
				base.OnPropertyChanged("IsFormationEditorSliderVisible");
				if (IsPlayerHeroRow)
				{
					base.OnPropertyChanged("IsRolePlanEditorVisible");
					RefreshRolePlanBindings();
				}
				NotifyEditorSelectionChanged();
			}
		}

		public override void OnRefresh()
		{
			Refresh();
		}

		public override void OnFinalize()
		{
			if (base.ViewModel != null)
			{
				base.ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
				Instances.Remove(base.ViewModel);
			}
			ActiveMixins.RemoveAll((WeakReference<PartyCharacterVMMixin> reference) => !reference.TryGetTarget(out PartyCharacterVMMixin target2) || target2 == this);
			WeakReference<PartyCharacterVMMixin>? lastSelectedMixin = LastSelectedMixin;
			if (lastSelectedMixin != null && lastSelectedMixin.TryGetTarget(out PartyCharacterVMMixin target) && target == this)
			{
				LastSelectedMixin = null;
			}
		}

		static PartyCharacterVMMixin()
		{
			TroopRole[] array = new TroopRole[9];
			RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			RolePlanRoles = (TroopRole[])(object)array;
			Instances = new ConditionalWeakTable<PartyCharacterVM, PartyCharacterVMMixin>();
			ActiveMixins = new List<WeakReference<PartyCharacterVMMixin>>();
		}
	}
	[PrefabExtension("PartyTroopTuple", "descendant::InventoryTupleExtensionControlsWidget[@Id='Extension']")]
	public class PartyTroopTupleCustomSplitEditorPatch : PrefabExtensionInsertPatch
	{
		public override InsertType Type => (InsertType)4;

		[PrefabExtensionFileName(true)]
		public string MyXmlFile => "PartyTroopTupleCustomSplitEditor";
	}
	[PrefabExtension("PartyTroopTuple", "descendant::ButtonWidget[@Id='LockButton']")]
	public class PartyTroopTupleFormationBadgePatch : PrefabExtensionInsertPatch
	{
		public override InsertType Type => (InsertType)4;

		[PrefabExtensionFileName(true)]
		public string MyXmlFile => "PartyTroopTupleFormationBadge";
	}
	[PrefabExtension("PartyTroopTuple", "descendant::InventoryTupleExtensionControlsWidget[@Id='Extension']")]
	public class PartyTroopTupleRolePlanEditorPatch : PrefabExtensionInsertPatch
	{
		public override InsertType Type => (InsertType)4;

		[PrefabExtensionFileName(true)]
		public string MyXmlFile => "PartyTroopTupleRolePlanEditor";
	}
}
namespace FormationManager.Patches
{
	[HarmonyPatch(typeof(OrderOfBattleFormationItemVM), "RefreshFormation", new Type[]
	{
		typeof(Formation),
		typeof(DeploymentFormationClass),
		typeof(bool)
	})]
	internal static class RefreshFormationPatch
	{
		[HarmonyPrefix]
		private static void Prefix(OrderOfBattleFormationItemVM __instance, Formation formation, ref DeploymentFormationClass overriddenClass, ref bool mustExist)
		{
			Settings instance = GlobalSettings<Settings>.Instance;
			if (instance == null || !instance.ModEnabled || !MissionGuards.IsCurrentMissionSupported() || !OrderOfBattleDefaultSession.IsApplyingDefaults || OrderOfBattleDefaultSession.IsClearingEmptyCards || formation == null || overriddenClass != 0)
			{
				return;
			}
			int index = formation.Index;
			if (HasTroopsAssigned(index))
			{
				DeploymentFormationClass customAssignmentClass = GetCustomAssignmentClass(index);
				if (customAssignmentClass != 0)
				{
					overriddenClass = customAssignmentClass;
					mustExist = true;
					FormationManager.Data.Logger.Log($"[RefreshFormationPatch] Prefix overrode formation {index} to {overriddenClass}");
				}
			}
		}

		[HarmonyPostfix]
		private static void Postfix(OrderOfBattleFormationItemVM __instance, Formation formation)
		{
			Settings instance = GlobalSettings<Settings>.Instance;
			if (instance != null && instance.ModEnabled && MissionGuards.IsCurrentMissionSupported() && formation != null)
			{
				int index = formation.Index;
				SelectorVM<OrderOfBattleFormationClassSelectorItemVM> formationClassSelector = __instance.FormationClassSelector;
				if (formationClassSelector != null)
				{
					OrderOfBattleFormationClassSelectorItemVM selectedItem = formationClassSelector.SelectedItem;
					string arg = ((selectedItem != null) ? selectedItem.FormationClass.ToString() : "null");
					FormationManager.Data.Logger.Log($"[RefreshFormationPatch] Postfix for formation {index}: SelectedIndex={formationClassSelector.SelectedIndex}, SelectedItem.FormationClass={arg}");
				}
			}
		}

		private static bool HasTroopsAssigned(int formationIndex)
		{
			if (!FormationAssignmentResolver.HasCustomDefaults(GlobalSettings<Settings>.Instance))
			{
				return false;
			}
			MobileParty mainParty = MobileParty.MainParty;
			if (mainParty?.MemberRoster == null)
			{
				return false;
			}
			for (int i = 0; i < mainParty.MemberRoster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = mainParty.MemberRoster.GetElementCopyAtIndex(i);
				if (elementCopyAtIndex.Character != null && TroopControlScope.IsEligibleTroop(elementCopyAtIndex.Character))
				{
					int troopCount = elementCopyAtIndex.Number - elementCopyAtIndex.WoundedNumber;
					if (FormationAssignmentResolver.GetAssignedCountForFormation(elementCopyAtIndex.Character, troopCount, formationIndex, GlobalSettings<Settings>.Instance) > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static DeploymentFormationClass GetCustomAssignmentClass(int formationIndex)
		{
			if (!FormationAssignmentResolver.HasCustomDefaults(GlobalSettings<Settings>.Instance))
			{
				return GetVanillaDefaultClass(formationIndex);
			}
			MobileParty mainParty = MobileParty.MainParty;
			bool flag = false;
			if (mainParty?.MemberRoster != null)
			{
				for (int i = 0; i < mainParty.MemberRoster.Count; i++)
				{
					TroopRosterElement elementCopyAtIndex = mainParty.MemberRoster.GetElementCopyAtIndex(i);
					if (elementCopyAtIndex.Character != null && TroopControlScope.IsEligibleTroop(elementCopyAtIndex.Character) && elementCopyAtIndex.Number > elementCopyAtIndex.WoundedNumber)
					{
						int[] effectiveFormationIndices = FormationAssignmentResolver.GetEffectiveFormationIndices(elementCopyAtIndex.Character, GlobalSettings<Settings>.Instance);
						if (effectiveFormationIndices.Contains(formationIndex))
						{
							DeploymentFormationClass deploymentFormationClass = MapToDeploymentClass(elementCopyAtIndex.Character);
							FormationManager.Data.Logger.Log($"[RefreshFormationPatch] Formation {formationIndex + 1} card class resolved from {elementCopyAtIndex.Character.StringId}: {deploymentFormationClass}");
							return deploymentFormationClass;
						}
						if (effectiveFormationIndices.Length != 0)
						{
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				return DeploymentFormationClass.Unset;
			}
			return GetVanillaDefaultClass(formationIndex);
		}

		public static DeploymentFormationClass[] GetCompatibleFormationClasses(int formationIndex, Team team)
		{
			Settings settings = GlobalSettings<Settings>.Instance;
			HashSet<DeploymentFormationClass> hashSet = (from agent in team.ActiveAgents
				where settings != null && TroopControlScope.ShouldManage(agent, settings) && agent.Formation != null && agent.Formation.Index == formationIndex && agent.Character != null
				select MapToDeploymentClass(agent.Character) into deploymentClass
				where deploymentClass != DeploymentFormationClass.Unset
				select deploymentClass).Distinct().ToHashSet();
			if (hashSet.Count == 0)
			{
				return new DeploymentFormationClass[1] { GetCustomAssignmentClass(formationIndex) };
			}
			if (hashSet.All((DeploymentFormationClass deploymentClass) => deploymentClass == DeploymentFormationClass.Infantry || deploymentClass == DeploymentFormationClass.Ranged))
			{
				return new DeploymentFormationClass[2]
				{
					DeploymentFormationClass.Infantry,
					DeploymentFormationClass.Ranged
				}.Where(hashSet.Contains).ToArray();
			}
			if (hashSet.All((DeploymentFormationClass deploymentClass) => deploymentClass == DeploymentFormationClass.Cavalry || deploymentClass == DeploymentFormationClass.HorseArcher))
			{
				return new DeploymentFormationClass[2]
				{
					DeploymentFormationClass.Cavalry,
					DeploymentFormationClass.HorseArcher
				}.Where(hashSet.Contains).ToArray();
			}
			DeploymentFormationClass customAssignmentClass = GetCustomAssignmentClass(formationIndex);
			FormationManager.Data.Logger.Log($"[RefreshFormationPatch] Formation {formationIndex + 1} has an unsupported cross-family class mix; retaining single-class fallback {customAssignmentClass}.");
			return new DeploymentFormationClass[1] { customAssignmentClass };
		}

		private static DeploymentFormationClass GetVanillaDefaultClass(int formationIndex)
		{
			switch (formationIndex)
			{
			case 0:
			case 4:
			case 5:
				return DeploymentFormationClass.Infantry;
			case 1:
				return DeploymentFormationClass.Ranged;
			case 2:
			case 6:
			case 7:
				return DeploymentFormationClass.Cavalry;
			case 3:
				return DeploymentFormationClass.HorseArcher;
			default:
				return DeploymentFormationClass.Unset;
			}
		}

		public static DeploymentFormationClass MapToDeploymentClass(FormationClass fc)
		{
			switch (fc)
			{
			case FormationClass.Infantry:
			case FormationClass.NumberOfDefaultFormations:
			case FormationClass.HeavyInfantry:
				return DeploymentFormationClass.Infantry;
			case FormationClass.Ranged:
				return DeploymentFormationClass.Ranged;
			case FormationClass.Cavalry:
			case FormationClass.LightCavalry:
			case FormationClass.HeavyCavalry:
				return DeploymentFormationClass.Cavalry;
			case FormationClass.HorseArcher:
				return DeploymentFormationClass.HorseArcher;
			default:
				return DeploymentFormationClass.Unset;
			}
		}

		public static DeploymentFormationClass MapToDeploymentClass(BasicCharacterObject character)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected I4, but got Unknown
			Settings instance = GlobalSettings<Settings>.Instance;
			if (instance == null || !instance.UsePartyManagerRoleDefaults)
			{
				return MapToDeploymentClass(character.DefaultFormationClass);
			}
			TroopRole val = TroopRoleClassifier.Classify(character);
			switch (val - 4)
			{
			case 0:
			case 1:
				return DeploymentFormationClass.Ranged;
			case 2:
				return DeploymentFormationClass.Cavalry;
			case 3:
				return DeploymentFormationClass.HorseArcher;
			default:
				return DeploymentFormationClass.Infantry;
			}
		}

		public static DeploymentFormationClass MapToDeploymentClass(Agent agent)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Expected I4, but got Unknown
			if (agent == null)
			{
				return DeploymentFormationClass.Unset;
			}
			Settings instance = GlobalSettings<Settings>.Instance;
			if (instance == null || !instance.UsePartyManagerRoleDefaults || !instance.UseSpawnedEquipmentClassification)
			{
				return MapToDeploymentClass(agent.Character?.DefaultFormationClass ?? FormationClass.Infantry);
			}
			TroopRole val = TroopRoleClassifier.Classify(agent);
			switch (val - 4)
			{
			case 0:
			case 1:
				return DeploymentFormationClass.Ranged;
			case 2:
				return DeploymentFormationClass.Cavalry;
			case 3:
				return DeploymentFormationClass.HorseArcher;
			default:
				return DeploymentFormationClass.Infantry;
			}
		}
	}
	[HarmonyPatch(typeof(OrderOfBattleVM), "SetInitialHeroFormations")]
	internal static class SetInitialHeroFormationsPatch
	{
		[HarmonyPostfix]
		private static void Postfix(OrderOfBattleVM __instance)
		{
			Settings instance = GlobalSettings<Settings>.Instance;
			if (instance != null && instance.ModEnabled && MissionGuards.IsCurrentMissionSupported())
			{
				FormationManager.Data.Logger.Log("[SetInitialHeroFormationsPatch] Deferring card weights until exact OOB placement is complete.");
			}
		}
	}
	internal static class OrderOfBattleDefaultSession
	{
		private static Mission? _mission;

		private static bool _isApplyingDefaults;

		private static bool _isClearingEmptyCards;

		public static bool IsApplyingDefaults
		{
			get
			{
				if (_isApplyingDefaults)
				{
					return _mission == Mission.Current;
				}
				return false;
			}
		}

		public static bool IsClearingEmptyCards
		{
			get
			{
				if (IsApplyingDefaults)
				{
					return _isClearingEmptyCards;
				}
				return false;
			}
		}

		public static void Begin(Mission mission)
		{
			_mission = mission;
			_isApplyingDefaults = mission != null;
		}

		public static void Complete()
		{
			_isApplyingDefaults = false;
			_isClearingEmptyCards = false;
		}

		public static void ClearEmptyCards(Action clear)
		{
			_isClearingEmptyCards = true;
			try
			{
				clear();
			}
			finally
			{
				_isClearingEmptyCards = false;
			}
		}
	}
	[HarmonyPatch(typeof(OrderOfBattleVM), "Initialize")]
	internal static class OrderOfBattleVMInitializePatch
	{
		[HarmonyPrefix]
		private static void Prefix(Mission __0)
		{
			OrderOfBattleDefaultSession.Begin(__0);
		}

		[HarmonyPostfix]
		private static void Postfix(OrderOfBattleVM __instance)
		{
			try
			{
				Settings instance = GlobalSettings<Settings>.Instance;
				if (instance == null || !instance.ModEnabled || !MissionGuards.IsCurrentMissionSupported() || !FormationAssignmentResolver.HasCustomDefaults(instance))
				{
					return;
				}
				FormationManager.Data.Logger.Log("[OrderOfBattleVMInitializePatch] Applying role and troop assignment defaults to OOB...");
				Team team = Mission.Current?.PlayerTeam;
				if (team == null)
				{
					FormationManager.Data.Logger.Log("[OrderOfBattleVMInitializePatch] PlayerTeam is null!");
					return;
				}
				OobFormationLayout layout = OobFormationLayout.ForActiveAgents(team, instance);
				OobPreviewAssignmentApplier.Apply(team, layout, instance, "initial plan");
				List<OrderOfBattleFormationItemVM> formationsList = __instance.FormationsFirstHalf.Concat(__instance.FormationsSecondHalf).ToList();
				Dictionary<int, DeploymentFormationClass[]> dictionary = formationsList.Where((OrderOfBattleFormationItemVM item) => item.Formation != null).ToDictionary((OrderOfBattleFormationItemVM item) => item.Formation.Index, (OrderOfBattleFormationItemVM item) => (from deploymentClass in RefreshFormationPatch.GetCompatibleFormationClasses(item.Formation.Index, team)
					where deploymentClass != DeploymentFormationClass.Unset
					select deploymentClass).ToArray());
				foreach (OrderOfBattleFormationItemVM item in formationsList)
				{
					if (item.Formation == null)
					{
						continue;
					}
					int index = item.Formation.Index;
					DeploymentFormationClass[] array = dictionary[index];
					if (array.Length != 0)
					{
						item.RefreshFormation(item.Formation, ToNativeCardClass(array), mustExist: true);
						if (item.Classes != null && item.Classes.Count > 0 && array.Length == 1)
						{
							item.Classes[0].Class = MapToNativeClass(array[0]);
							if (item.Classes.Count > 1)
							{
								item.Classes[1].Class = FormationClass.NumberOfAllFormations;
							}
						}
					}
					else
					{
						item.RefreshFormation(item.Formation);
					}
				}
				OobWeightDistributor.DistributeWeights(__instance, layout);
				foreach (OrderOfBattleFormationItemVM item2 in formationsList)
				{
					item2.OnSizeChanged();
				}
				try
				{
					AccessTools.Method(typeof(OrderOfBattleVM), "RefreshWeights", (Type[])null, (Type[])null).Invoke(__instance, null);
				}
				catch (Exception arg)
				{
					FormationManager.Data.Logger.Log($"[OrderOfBattleVMInitializePatch] Failed to call RefreshWeights: {arg}");
				}
				__instance.OnUnitDeployed();
				OobPreviewAssignmentApplier.Apply(team, layout, instance, "after native class-pool distribution");
				OrderOfBattleDefaultSession.ClearEmptyCards(delegate
				{
					foreach (OrderOfBattleFormationItemVM item3 in formationsList)
					{
						if (item3.Formation != null && layout.GetFormationCount(item3.Formation.Index) == 0)
						{
							item3.RefreshFormation(item3.Formation);
							FormationManager.Data.Logger.Log($"[OrderOfBattleVMInitializePatch] Cleared empty formation {item3.Formation.Index + 1} after native deployment.");
						}
					}
				});
				foreach (OrderOfBattleFormationItemVM item4 in formationsList)
				{
					item4.OnSizeChanged();
				}
				OobWeightDistributor.LockManagedSliders(__instance, layout);
				string text = ((instance.LockManagedOobSliders && FormationAssignmentResolver.HasCustomDefaults(instance)) ? "managed native OOB sliders are locked" : "native OOB sliders remain available");
				FormationManager.Data.Logger.Log("[OrderOfBattleVMInitializePatch] Defaults applied; " + text + ".");
			}
			finally
			{
				OrderOfBattleDefaultSession.Complete();
			}
		}

		[HarmonyFinalizer]
		private static Exception? Finalizer(Exception? __exception)
		{
			OrderOfBattleDefaultSession.Complete();
			return __exception;
		}

		private static FormationClass MapToNativeClass(DeploymentFormationClass dfc)
		{
			return dfc switch
			{
				DeploymentFormationClass.Infantry => FormationClass.Infantry, 
				DeploymentFormationClass.Ranged => FormationClass.Ranged, 
				DeploymentFormationClass.Cavalry => FormationClass.Cavalry, 
				DeploymentFormationClass.HorseArcher => FormationClass.HorseArcher, 
				_ => FormationClass.NumberOfAllFormations, 
			};
		}

		private static DeploymentFormationClass ToNativeCardClass(DeploymentFormationClass[] classes)
		{
			bool num = classes.Contains(DeploymentFormationClass.Infantry);
			bool flag = classes.Contains(DeploymentFormationClass.Ranged);
			if (num && flag)
			{
				return DeploymentFormationClass.InfantryAndRanged;
			}
			bool num2 = classes.Contains(DeploymentFormationClass.Cavalry);
			bool flag2 = classes.Contains(DeploymentFormationClass.HorseArcher);
			if (num2 && flag2)
			{
				return DeploymentFormationClass.CavalryAndHorseArcher;
			}
			return classes[0];
		}
	}
	[HarmonyPatch(typeof(Mission), "SpawnTroop")]
	internal static class MissionAgentSpawnPatch
	{
		[HarmonyPostfix]
		private static void Postfix(Mission __instance, IAgentOriginBase troopOrigin, bool isPlayerSide, Agent __result)
		{
			if (__result == null || __result.IsMainAgent)
			{
				return;
			}
			Settings instance = GlobalSettings<Settings>.Instance;
			if (instance == null || !instance.ModEnabled || !FormationAssignmentResolver.HasCustomDefaults(instance) || !MissionGuards.IsSupportedRegularBattleMission(__instance) || !isPlayerSide || !TroopControlScope.ShouldManage(__result, instance))
			{
				return;
			}
			BasicCharacterObject troop = troopOrigin.Troop;
			if (troop == null)
			{
				return;
			}
			int num = FormationAssignmentResolver.ResolveFormationIndex(__result, troop, instance);
			if (num < 0 || num > 7)
			{
				return;
			}
			Team playerTeam = __instance.PlayerTeam;
			if (playerTeam == null)
			{
				return;
			}
			Formation formation = playerTeam.GetFormation((FormationClass)num);
			if (formation == null || formation.Team != playerTeam)
			{
				return;
			}
			try
			{
				__result.Formation = formation;
				FormationManager.Data.Logger.Log($"[MissionAgentSpawnPatch] Moved agent {troop.StringId} to formation {num} (Name: {troop.Name})");
			}
			catch (Exception ex)
			{
				FormationManager.Data.Logger.Log($"[MissionAgentSpawnPatch] Could not move {troop.StringId} to formation {num}: {ex.GetType().Name}: {ex.Message}");
			}
		}
	}
	[HarmonyPatch(typeof(Mission))]
	[HarmonyPatch(/*Could not decode attribute arguments.*/)]
	[HarmonyPatch(new Type[]
	{
		typeof(MissionInitializerRecord),
		typeof(MissionState),
		typeof(bool)
	})]
	internal static class MissionConstructorPatch
	{
		[HarmonyPostfix]
		private static void Postfix(Mission __instance)
		{
		}

		private static FormationClass OnGetAgentTroopClass(BattleSideEnum side, BasicCharacterObject character)
		{
			return character?.DefaultFormationClass ?? FormationClass.Infantry;
		}
	}
	internal static class MissionGuards
	{
		public static bool CanAttachMissionBehavior(Mission? mission)
		{
			if (mission == null)
			{
				return false;
			}
			if (mission.CombatType == Mission.MissionCombatType.Combat)
			{
				return IsBattleOrDeploymentStartup(mission.Mode);
			}
			return false;
		}

		public static bool IsSupportedRegularBattleMission(Mission? mission)
		{
			if (mission == null)
			{
				return false;
			}
			if (mission.CombatType == Mission.MissionCombatType.Combat && IsBattleOrDeployment(mission.Mode) && mission.PlayerTeam != null)
			{
				return HasSupportedRegularBattleBehavior(mission);
			}
			return false;
		}

		public static bool IsCurrentMissionSupported()
		{
			return IsSupportedRegularBattleMission(Mission.Current);
		}

		private static bool IsBattleOrDeploymentStartup(MissionMode mode)
		{
			if (mode != 0 && mode != MissionMode.Deployment)
			{
				return mode == MissionMode.Battle;
			}
			return true;
		}

		private static bool IsBattleOrDeployment(MissionMode mode)
		{
			if (mode != MissionMode.Deployment)
			{
				return mode == MissionMode.Battle;
			}
			return true;
		}

		private static bool HasSupportedRegularBattleBehavior(Mission mission)
		{
			return mission.HasMissionBehavior<BattleDeploymentHandler>();
		}
	}
	internal sealed class MissionTroopReassignmentBehavior : MissionBehavior
	{
		private const float ReassignmentIntervalSeconds = 3f;

		private float _elapsedSinceScan;

		public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

		public override void OnMissionTick(float dt)
		{
			if (!MissionGuards.IsCurrentMissionSupported())
			{
				return;
			}
			Settings instance = GlobalSettings<Settings>.Instance;
			if (instance != null && instance.ModEnabled && instance.AutoReassignmentEnabled && instance.BacklineReassignmentEnabled)
			{
				_elapsedSinceScan += dt;
				if (!(_elapsedSinceScan < 3f))
				{
					_elapsedSinceScan = 0f;
					ReassignPlayerTeamAgents("periodic scan");
				}
			}
		}

		public override void OnAgentDismount(Agent agent)
		{
			if (MissionGuards.IsCurrentMissionSupported())
			{
				ReassignAgent(agent, "dismount");
			}
		}

		public override void OnAgentMount(Agent agent)
		{
			if (MissionGuards.IsCurrentMissionSupported())
			{
				ReassignAgent(agent, "mount");
			}
		}

		private static void ReassignPlayerTeamAgents(string reason)
		{
			Team team = Mission.Current?.PlayerTeam;
			if (team == null)
			{
				return;
			}
			foreach (Agent activeAgent in team.ActiveAgents)
			{
				if (CanSafelyMoveAgent(activeAgent, team))
				{
					ReassignAgent(activeAgent, reason);
				}
			}
		}

		private static void ReassignAgent(Agent agent, string reason)
		{
			Settings instance = GlobalSettings<Settings>.Instance;
			if (instance == null || !instance.ModEnabled || !instance.AutoReassignmentEnabled || !IsManagedTroop(agent, instance))
			{
				return;
			}
			BasicCharacterObject character = agent.Character;
			if (character != null)
			{
				int targetFormationIndex = GetTargetFormationIndex(agent, character, instance, reason);
				if (targetFormationIndex >= 0)
				{
					MoveToFormation(agent, targetFormationIndex, reason);
				}
			}
		}

		private static int GetTargetFormationIndex(Agent agent, BasicCharacterObject character, Settings settings, string reason)
		{
			FormationClass defaultFormationClass = character.DefaultFormationClass;
			if (settings.BacklineReassignmentEnabled && IsRangedClass(defaultFormationClass) && IsOutOfAmmo(agent))
			{
				return FormationAssignmentResolver.ToFormationIndex(agent.HasMount ? settings.MountedBacklineFormation : settings.BacklineFormation);
			}
			if (reason == "periodic scan")
			{
				return -1;
			}
			if (!agent.HasMount)
			{
				if (IsMeleeCavalry(defaultFormationClass))
				{
					return FormationAssignmentResolver.ToFormationIndex(settings.InfantryFormation);
				}
				if (defaultFormationClass == FormationClass.HorseArcher)
				{
					return FormationAssignmentResolver.ToFormationIndex(settings.ArcherFormation);
				}
			}
			return FormationAssignmentResolver.ResolveFormationIndex(agent, character, settings);
		}

		private static bool IsManagedTroop(Agent agent, Settings settings)
		{
			return TroopControlScope.ShouldManage(agent, settings);
		}

		private static bool CanSafelyMoveAgent(Agent agent, Team expectedTeam)
		{
			if (agent == null)
			{
				return false;
			}
			if (agent.Mission != Mission.Current)
			{
				return false;
			}
			if (!agent.IsActive() || agent.State != AgentState.Active)
			{
				return false;
			}
			if (!agent.IsHuman || agent.IsMount || agent.IsMainAgent)
			{
				return false;
			}
			if (agent.Team != expectedTeam || !expectedTeam.IsPlayerTeam)
			{
				return false;
			}
			if (agent.Detachment != null || agent.IsDetachedFromFormation)
			{
				return false;
			}
			return true;
		}

		private static bool IsOutOfAmmo(Agent agent)
		{
			MissionEquipment equipment = agent.Equipment;
			if (equipment != null)
			{
				return !equipment.ContainsNonConsumableRangedWeaponWithAmmo();
			}
			return false;
		}

		private static bool IsRangedClass(FormationClass formationClass)
		{
			if (formationClass != FormationClass.Ranged)
			{
				return formationClass == FormationClass.HorseArcher;
			}
			return true;
		}

		private static bool IsMeleeCavalry(FormationClass formationClass)
		{
			if (formationClass != FormationClass.Cavalry && formationClass != FormationClass.HeavyCavalry)
			{
				return formationClass == FormationClass.LightCavalry;
			}
			return true;
		}

		private static void MoveToFormation(Agent agent, int formationIndex, string reason)
		{
			Mission current = Mission.Current;
			Team team = agent.Team;
			BasicCharacterObject character = agent.Character;
			if (current == null || team == null || character == null || !CanSafelyMoveAgent(agent, team))
			{
				return;
			}
			Formation formation = team.GetFormation((FormationClass)formationIndex);
			if (formation == null || formation.Team != team || agent.Formation == formation)
			{
				return;
			}
			try
			{
				agent.Formation = formation;
				FormationManager.Data.Logger.Log($"[MissionTroopReassignmentBehavior] Moved {character.StringId} to formation {formationIndex + 1} after {reason} (native class: {character.DefaultFormationClass}, has mount: {agent.HasMount}).");
			}
			catch (Exception ex)
			{
				FormationManager.Data.Logger.Log($"[MissionTroopReassignmentBehavior] Skipped moving {character.StringId} to formation {formationIndex + 1} after {reason}; Bannerlord rejected the move. AgentState={agent.State}, IsActive={agent.IsActive()}, IsDetached={agent.IsDetachedFromFormation}, HasDetachment={agent.Detachment != null}. Error={ex.GetType().Name}: {ex.Message}");
			}
		}
	}
	internal sealed class OobFormationLayout
	{
		private sealed class BalancedRemainderStrategy : IFormationRemainderStrategy
		{
			private readonly Dictionary<int, int> _totals;

			public BalancedRemainderStrategy(Dictionary<int, int> totals)
			{
				_totals = totals;
			}

			public void RecordBaseAllocation(int formationIndex, int count)
			{
				Add(formationIndex, count);
			}

			public int SelectRecipient(IReadOnlyDictionary<int, int> remainders, ISet<int> excludedRecipients)
			{
				ISet<int> excludedRecipients2 = excludedRecipients;
				int value;
				return (from pair in remainders
					where !excludedRecipients2.Contains(pair.Key)
					orderby pair.Value descending, _totals.TryGetValue(pair.Key, out value) ? value : 0, pair.Key
					select pair).First().Key;
			}

			public void RecordRemainderAllocation(int formationIndex)
			{
				Add(formationIndex, 1);
			}

			private void Add(int formationIndex, int count)
			{
				if (count > 0)
				{
					_totals[formationIndex] = (_totals.TryGetValue(formationIndex, out var value) ? (value + count) : count);
				}
			}
		}

		private readonly int[] _formationCounts = new int[8];

		private readonly int[,] _formationClassCounts = new int[8, 7];

		private readonly Dictionary<int, int> _agentFormationIndices = new Dictionary<int, int>();

		private readonly Dictionary<string, Dictionary<int, int>> _remainderTotals = new Dictionary<string, Dictionary<int, int>>();

		public int GetFormationCount(int formationIndex)
		{
			if (formationIndex < 0 || formationIndex >= _formationCounts.Length)
			{
				return 0;
			}
			return _formationCounts[formationIndex];
		}

		public int GetFormationClassCount(int formationIndex, DeploymentFormationClass deploymentClass)
		{
			if (formationIndex < 0 || formationIndex >= 8 || deploymentClass < DeploymentFormationClass.Unset || deploymentClass >= (DeploymentFormationClass)7)
			{
				return 0;
			}
			return _formationClassCounts[formationIndex, (int)deploymentClass];
		}

		public int GetFormationIndex(Agent? agent, BasicCharacterObject character, Settings? settings)
		{
			if (agent != null && _agentFormationIndices.TryGetValue(agent.Index, out var value))
			{
				return value;
			}
			return FormationAssignmentResolver.ResolveFormationIndex(agent, character, settings);
		}

		public static OobFormationLayout ForActiveAgents(Team? team, Settings? settings)
		{
			Settings settings2 = settings;
			OobFormationLayout oobFormationLayout = new OobFormationLayout();
			if (team == null)
			{
				return oobFormationLayout;
			}
			foreach (IGrouping<string, Agent> item in from agent in team.ActiveAgents
				where TroopControlScope.ShouldManage(agent, settings2)
				group agent by agent.Character.StringId into @group
				orderby @group.Key
				select @group)
			{
				List<Agent> list = item.OrderBy((Agent agent) => agent.Index).ToList();
				if (list.Count > 0)
				{
					oobFormationLayout.AddStack(list[0].Character, list.Count, list, settings2);
				}
			}
			return oobFormationLayout;
		}

		private void AddStack(BasicCharacterObject character, int troopCount, List<Agent> agents, Settings? settings)
		{
			if (FormationAssignmentResolver.UsesAgentRoleDefaults(character, settings))
			{
				foreach (Agent agent in agents)
				{
					int defaultFormationIndex = FormationAssignmentResolver.GetDefaultFormationIndex(agent, character, settings);
					_agentFormationIndices[agent.Index] = defaultFormationIndex;
					_formationCounts[defaultFormationIndex]++;
					int num = (int)RefreshFormationPatch.MapToDeploymentClass(agent);
					if (num >= 0 && num < 7)
					{
						_formationClassCounts[defaultFormationIndex, num]++;
					}
				}
				return;
			}
			int[] formationIndices;
			Dictionary<int, int> targets;
			Dictionary<int, int> weights;
			int surplus;
			Dictionary<int, int> dictionary = (FormationAssignmentResolver.TryGetEvenSplitTargets(character, out formationIndices, settings) ? FormationPlanAllocator.AllocateEvenly(formationIndices, troopCount, CreateRemainderStrategy(formationIndices.Select((int index) => $"{index}").ToArray())) : ((!FormationAssignmentResolver.TryGetWeightedCustomSurplus(character, troopCount, out targets, out weights, out surplus, settings)) ? FormationAssignmentResolver.GetAllocatedFormationCounts(character, troopCount, settings) : FormationAssignmentResolver.GetAllocatedFormationCounts(character, troopCount, settings, CreateRemainderStrategy((from pair in weights
				orderby pair.Key
				select $"{pair.Key}:{pair.Value}").ToArray()))));
			DeploymentFormationClass deploymentFormationClass = RefreshFormationPatch.MapToDeploymentClass(character);
			foreach (KeyValuePair<int, int> item in dictionary)
			{
				_formationCounts[item.Key] += item.Value;
				int num2 = (int)deploymentFormationClass;
				if (num2 >= 0 && num2 < 7)
				{
					_formationClassCounts[item.Key, num2] += item.Value;
				}
			}
			int num3 = 0;
			foreach (KeyValuePair<int, int> item2 in dictionary.OrderBy((KeyValuePair<int, int> pair) => pair.Key))
			{
				for (int i = 0; i < item2.Value; i++)
				{
					if (num3 >= agents.Count)
					{
						break;
					}
					_agentFormationIndices[agents[num3++].Index] = item2.Key;
				}
			}
		}

		private IFormationRemainderStrategy CreateRemainderStrategy(IEnumerable<string> signatureParts)
		{
			string key = string.Join(",", signatureParts);
			if (!_remainderTotals.TryGetValue(key, out Dictionary<int, int> value))
			{
				value = new Dictionary<int, int>();
				_remainderTotals.Add(key, value);
			}
			return new BalancedRemainderStrategy(value);
		}
	}
	internal static class OobPreviewAssignmentApplier
	{
		public static void Apply(Team team, OobFormationLayout layout, Settings settings, string stage)
		{
			foreach (Agent activeAgent in team.ActiveAgents)
			{
				BasicCharacterObject character = activeAgent.Character;
				if (character == null || !TroopControlScope.ShouldManage(activeAgent, settings))
				{
					continue;
				}
				int formationIndex = layout.GetFormationIndex(activeAgent, character, settings);
				if ((formationIndex < 0 || formationIndex > 7) ? true : false)
				{
					continue;
				}
				Formation formation = team.GetFormation((FormationClass)formationIndex);
				if (formation != null && formation.Team == team && activeAgent.Formation != formation)
				{
					try
					{
						activeAgent.Formation = formation;
						FormationManager.Data.Logger.Log($"[OobPreviewAssignmentApplier] Applied {stage}: {character.StringId} -> formation {formationIndex + 1} (Name: {character.Name})");
					}
					catch (Exception ex)
					{
						FormationManager.Data.Logger.Log("[OobPreviewAssignmentApplier] Could not apply " + stage + " for " + character.StringId + ": " + ex.GetType().Name + ": " + ex.Message);
					}
				}
			}
		}
	}
	internal static class OobWeightDistributor
	{
		public static void DistributeWeights(OrderOfBattleVM viewModel, OobFormationLayout layout)
		{
			if (!FormationAssignmentResolver.HasCustomDefaults(GlobalSettings<Settings>.Instance))
			{
				return;
			}
			int[,] classCounts = GetClassCounts(layout);
			int[] totals = GetTotals(classCounts);
			foreach (OrderOfBattleFormationItemVM item in viewModel.FormationsFirstHalf.Concat(viewModel.FormationsSecondHalf))
			{
				if (item.Formation == null)
				{
					continue;
				}
				int index = item.Formation.Index;
				foreach (OrderOfBattleFormationClassVM item2 in item.Classes.Where((OrderOfBattleFormationClassVM classViewModel) => !classViewModel.IsUnset))
				{
					ApplyWeight(item2, index, classCounts, totals);
				}
			}
		}

		public static void LockManagedSliders(OrderOfBattleVM viewModel, OobFormationLayout layout)
		{
			Settings instance = GlobalSettings<Settings>.Instance;
			if (instance == null || !instance.LockManagedOobSliders || !FormationAssignmentResolver.HasCustomDefaults(instance))
			{
				return;
			}
			int[,] classCounts = GetClassCounts(layout);
			foreach (OrderOfBattleFormationItemVM item in viewModel.FormationsFirstHalf.Concat(viewModel.FormationsSecondHalf))
			{
				if (item.Formation == null)
				{
					continue;
				}
				int index = item.Formation.Index;
				foreach (OrderOfBattleFormationClassVM item2 in item.Classes.Where((OrderOfBattleFormationClassVM classViewModel) => !classViewModel.IsUnset))
				{
					int num = (int)RefreshFormationPatch.MapToDeploymentClass(item2.Class);
					if (num >= 0 && num < 7 && classCounts[index, num] > 0)
					{
						item2.SetWeightAdjustmentLock(isLocked: true);
						item2.IsLocked = true;
						FormationManager.Data.Logger.Log($"[OobWeightDistributor] Locked formation {index + 1} {item2.Class} slider.");
					}
				}
			}
		}

		private static int[,] GetClassCounts(OobFormationLayout layout)
		{
			int[,] array = new int[8, 7];
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 7; j++)
				{
					array[i, j] = layout.GetFormationClassCount(i, (DeploymentFormationClass)j);
				}
			}
			return array;
		}

		private static int[] GetTotals(int[,] classCounts)
		{
			int[] array = new int[7];
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 7; j++)
				{
					array[j] += classCounts[i, j];
				}
			}
			return array;
		}

		private static void ApplyWeight(OrderOfBattleFormationClassVM classViewModel, int formationIndex, int[,] classCounts, int[] totalByClass)
		{
			int num = (int)RefreshFormationPatch.MapToDeploymentClass(classViewModel.Class);
			if ((num >= 0 && num < 7) || 1 == 0)
			{
				int weight = ((totalByClass[num] > 0) ? ((int)Math.Round((double)classCounts[formationIndex, num] / (double)totalByClass[num] * 100.0)) : 0);
				classViewModel.Weight = weight;
			}
		}
	}
	[HarmonyPatch(typeof(PartyCharacterVM), "ExecuteSetSelected")]
	internal static class PartyCharacterSelectionPatch
	{
		private static void Postfix(PartyCharacterVM __instance)
		{
			PartyCharacterVMMixin.NotifySelectionChanged(__instance);
		}
	}
	public static class QueryLibraryPatches
	{
		private static bool IsModActive()
		{
			if (GlobalSettings<Settings>.Instance != null)
			{
				return GlobalSettings<Settings>.Instance.ModEnabled;
			}
			return false;
		}

		[HarmonyPostfix]
		public static void IsInfantryPostfix(Agent a, ref bool __result)
		{
		}
	}
	internal static class TroopControlScope
	{
		public static bool IsEligibleTroop(BasicCharacterObject? character)
		{
			if (character != null)
			{
				if (character is CharacterObject characterObject)
				{
					return characterObject.HeroObject == null;
				}
				return true;
			}
			return false;
		}

		public static bool ShouldManage(Agent? agent, Settings settings)
		{
			if (agent == null || !agent.IsHuman || agent.IsMount || agent.IsMainAgent || !IsEligibleTroop(agent.Character))
			{
				return false;
			}
			if (agent.Team == null || !agent.Team.IsPlayerTeam)
			{
				return false;
			}
			if (IsFromMainParty(agent))
			{
				return true;
			}
			return IsAlliedClassEnabled(agent.Character.DefaultFormationClass, settings);
		}

		private static bool IsAlliedClassEnabled(FormationClass formationClass, Settings settings)
		{
			switch (formationClass)
			{
			case FormationClass.Ranged:
				return settings.ManageAlliedRanged;
			case FormationClass.HorseArcher:
				return settings.ManageAlliedHorseArchers;
			case FormationClass.Cavalry:
			case FormationClass.LightCavalry:
			case FormationClass.HeavyCavalry:
				return settings.ManageAlliedCavalry;
			default:
				return settings.ManageAlliedInfantry;
			}
		}

		private static bool IsFromMainParty(Agent agent)
		{
			object origin = agent.Origin;
			object obj = ((origin == null) ? null : AccessTools.Property(origin.GetType(), "Party")?.GetValue(origin));
			if (obj != null)
			{
				return obj == MobileParty.MainParty?.Party;
			}
			return false;
		}
	}
}
namespace FormationManager.Models
{
	public class FormationManagerBattleInitializationModel : BattleInitializationModel
	{
		private readonly BattleInitializationModel _parent;

		public FormationManagerBattleInitializationModel(BattleInitializationModel parent)
		{
			_parent = parent;
		}

		public override List<FormationClass> GetAllAvailableTroopTypes()
		{
			return _parent?.GetAllAvailableTroopTypes() ?? new List<FormationClass>();
		}

		protected override bool CanPlayerSideDeployWithOrderOfBattleAux()
		{
			if (_parent == null)
			{
				return false;
			}
			MethodInfo method = typeof(BattleInitializationModel).GetMethod("CanPlayerSideDeployWithOrderOfBattleAux", BindingFlags.Instance | BindingFlags.NonPublic);
			if (method == null)
			{
				return false;
			}
			return (bool)method.Invoke(_parent, null);
		}
	}
}
namespace FormationManager.Data
{
	internal static class FormationAssignmentResolver
	{
		public static bool HasCustomDefaults(Settings? settings)
		{
			if (!FormationAssignmentStore.HasAnyAssignments && !FormationAssignmentStore.HasAnyRoleAssignments)
			{
				return HasConfiguredDefaultFormations(settings);
			}
			return true;
		}

		public static int[] GetActivePlanFormationIndices(BasicCharacterObject character, Settings? settings)
		{
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			if (character == null)
			{
				return Array.Empty<int>();
			}
			if (AdvancedPlansEnabled(settings) && FormationAssignmentStore.TryGetDeploymentPlan(character.StringId, out TroopDeploymentPlan plan))
			{
				if (plan.Mode == TroopDeploymentPlanMode.Even)
				{
					return plan.FormationIndices.ToArray();
				}
				return (from index in plan.FormationTargets.Keys.Concat(plan.FormationWeights.Keys).Where(FormationPlanNormalizer.IsValidFormationIndex).Distinct()
					orderby index
					select index).ToArray();
			}
			int[] assignments = FormationAssignmentStore.GetAssignments(character.StringId);
			if (assignments.Length != 0)
			{
				return assignments;
			}
			int[] roleAssignments = FormationAssignmentStore.GetRoleAssignments(TroopRoleClassifier.Classify(character));
			if (roleAssignments.Length != 0)
			{
				return roleAssignments;
			}
			return Array.Empty<int>();
		}

		public static int[] GetEffectiveFormationIndices(BasicCharacterObject character, Settings? settings)
		{
			int[] activePlanFormationIndices = GetActivePlanFormationIndices(character, settings);
			if (activePlanFormationIndices.Length == 0)
			{
				return new int[1] { GetDefaultFormationIndex(character, settings) };
			}
			return activePlanFormationIndices;
		}

		public static int[] GetFormationIndices(BasicCharacterObject character, Settings? settings)
		{
			return GetEffectiveFormationIndices(character, settings);
		}

		public static int ResolveFormationIndex(Agent? agent, BasicCharacterObject character, Settings? settings)
		{
			if (TryGetEvenSplitTargets(character, out int[] formationIndices, settings))
			{
				if (agent == null)
				{
					return formationIndices[0];
				}
				int num = ((agent.Index != int.MinValue) ? Math.Abs(agent.Index) : 0);
				return formationIndices[num % formationIndices.Length];
			}
			if (AdvancedPlansEnabled(settings) && FormationAssignmentStore.TryGetDeploymentPlan(character.StringId, out TroopDeploymentPlan plan) && plan.Mode == TroopDeploymentPlanMode.Custom)
			{
				return GetDefaultFormationIndex(agent, character, settings);
			}
			int[] activePlanFormationIndices = GetActivePlanFormationIndices(character, settings);
			if (activePlanFormationIndices.Length == 0)
			{
				return GetDefaultFormationIndex(agent, character, settings);
			}
			if (activePlanFormationIndices.Length == 1 || agent == null)
			{
				return activePlanFormationIndices[0];
			}
			int num2 = agent.Index;
			if (num2 == int.MinValue)
			{
				num2 = 0;
			}
			return activePlanFormationIndices[Math.Abs(num2) % activePlanFormationIndices.Length];
		}

		public static int GetDefaultFormationIndex(BasicCharacterObject character, Settings? settings)
		{
			return GetDefaultFormationIndex(null, character, settings);
		}

		public static int GetDefaultFormationIndex(Agent? agent, BasicCharacterObject character, Settings? settings)
		{
			if (character == null)
			{
				return 0;
			}
			if (TryGetConfiguredDefaultFormationIndex(agent, character, settings, out var formationIndex))
			{
				return formationIndex;
			}
			return GetVanillaFormationIndex(character.DefaultFormationClass);
		}

		public static int GetAssignedCountForFormation(BasicCharacterObject character, int troopCount, int formationIndex, Settings? settings)
		{
			if (!GetAllocatedFormationCounts(character, troopCount, settings).TryGetValue(formationIndex, out var value))
			{
				return 0;
			}
			return value;
		}

		public static bool TryGetEvenSplitTargets(BasicCharacterObject character, out int[] formationIndices, Settings? settings = null)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			formationIndices = Array.Empty<int>();
			if (character == null)
			{
				return false;
			}
			if (AdvancedPlansEnabled(settings) && FormationAssignmentStore.TryGetDeploymentPlan(character.StringId, out TroopDeploymentPlan plan) && plan.Mode == TroopDeploymentPlanMode.Even)
			{
				formationIndices = plan.FormationIndices.ToArray();
				return formationIndices.Length >= 2;
			}
			int[] assignments = FormationAssignmentStore.GetAssignments(character.StringId);
			if (assignments.Length >= 2)
			{
				formationIndices = assignments;
				return true;
			}
			int[] roleAssignments = FormationAssignmentStore.GetRoleAssignments(TroopRoleClassifier.Classify(character));
			if (roleAssignments.Length < 2)
			{
				return false;
			}
			formationIndices = roleAssignments;
			return true;
		}

		public static bool TryGetWeightedCustomSurplus(BasicCharacterObject character, int troopCount, out Dictionary<int, int> targets, out Dictionary<int, int> weights, out int surplus, Settings? settings = null)
		{
			targets = new Dictionary<int, int>();
			weights = new Dictionary<int, int>();
			surplus = 0;
			if (character == null || !AdvancedPlansEnabled(settings) || !FormationAssignmentStore.TryGetDeploymentPlan(character.StringId, out TroopDeploymentPlan plan) || plan.Mode != TroopDeploymentPlanMode.Custom)
			{
				return false;
			}
			foreach (KeyValuePair<int, int> item in plan.FormationTargets.Where((KeyValuePair<int, int> pair) => pair.Key >= 0 && pair.Key <= 7 && pair.Value > 0))
			{
				targets[item.Key] = item.Value;
			}
			foreach (KeyValuePair<int, int> item2 in plan.FormationWeights.Where((KeyValuePair<int, int> pair) => pair.Key >= 0 && pair.Key <= 7 && pair.Value > 0))
			{
				weights[item2.Key] = item2.Value;
			}
			int num = targets.Values.Sum();
			surplus = troopCount - num;
			if (surplus > 0)
			{
				return weights.Count > 0;
			}
			return false;
		}

		public static Dictionary<int, int> GetAllocatedFormationCounts(BasicCharacterObject character, int troopCount, Settings? settings, IFormationRemainderStrategy? remainderStrategy = null)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			if (character == null || troopCount <= 0)
			{
				return dictionary;
			}
			if (TryGetEvenSplitTargets(character, out int[] formationIndices, settings))
			{
				return FormationPlanAllocator.AllocateEvenly(formationIndices, troopCount, remainderStrategy);
			}
			if (AdvancedPlansEnabled(settings) && FormationAssignmentStore.TryGetDeploymentPlan(character.StringId, out TroopDeploymentPlan plan) && plan.Mode == TroopDeploymentPlanMode.Custom)
			{
				return FormationPlanAllocator.AllocateCustom(plan, troopCount, GetDefaultFormationIndex(character, settings), settings?.PrioritizeWeightsInSmallStacks ?? false, remainderStrategy);
			}
			int[] assignments = FormationAssignmentStore.GetAssignments(character.StringId);
			if (assignments.Length == 1)
			{
				dictionary[assignments[0]] = troopCount;
				return dictionary;
			}
			dictionary[GetDefaultFormationIndex(character, settings)] = troopCount;
			return dictionary;
		}

		public static Dictionary<int, int> GetAllocatedCustomPlanCounts(BasicCharacterObject character, int troopCount, IReadOnlyDictionary<int, int> targets, IReadOnlyDictionary<int, int> weights, Settings? settings)
		{
			Dictionary<int, int> result = new Dictionary<int, int>();
			if (character == null || troopCount <= 0)
			{
				return result;
			}
			TroopDeploymentPlan troopDeploymentPlan = TroopDeploymentPlan.CreateCustom();
			foreach (KeyValuePair<int, int> target in targets)
			{
				troopDeploymentPlan.FormationTargets[target.Key] = target.Value;
			}
			foreach (KeyValuePair<int, int> weight in weights)
			{
				troopDeploymentPlan.FormationWeights[weight.Key] = weight.Value;
			}
			return FormationPlanAllocator.AllocateCustom(troopDeploymentPlan, troopCount, GetDefaultFormationIndex(character, settings), settings?.PrioritizeWeightsInSmallStacks ?? false);
		}

		private static bool AdvancedPlansEnabled(Settings? settings)
		{
			return settings?.ShowAdvancedFormationEditor ?? GlobalSettings<Settings>.Instance?.ShowAdvancedFormationEditor ?? true;
		}

		private static bool HasConfiguredDefaultFormations(Settings? settings)
		{
			if (settings == null)
			{
				return false;
			}
			if (settings.UsePartyManagerRoleDefaults)
			{
				if (!IsValidConfiguredFormation(settings.LightInfantryFormation) && !IsValidConfiguredFormation(settings.ShieldInfantryFormation) && !IsValidConfiguredFormation(settings.ShockInfantryFormation) && !IsValidConfiguredFormation(settings.PikeInfantryFormation) && !IsValidConfiguredFormation(settings.SkirmisherFormation) && !IsValidConfiguredFormation(settings.FootArcherFormation) && !IsValidConfiguredFormation(settings.CrossbowmanFormation) && !IsValidConfiguredFormation(settings.MeleeCavalryRoleFormation))
				{
					return IsValidConfiguredFormation(settings.HorseArcherRoleFormation);
				}
				return true;
			}
			if (!IsValidConfiguredFormation(settings.InfantryFormation) && !IsValidConfiguredFormation(settings.ArcherFormation))
			{
				return IsValidConfiguredFormation(settings.CavalryFormation);
			}
			return true;
		}

		internal static bool UsesAgentRoleDefaults(BasicCharacterObject character, Settings? settings)
		{
			if (character == null || settings == null || !settings.UsePartyManagerRoleDefaults || !settings.UseSpawnedEquipmentClassification)
			{
				return false;
			}
			return GetActivePlanFormationIndices(character, settings).Length == 0;
		}

		private static bool TryGetConfiguredDefaultFormationIndex(Agent? agent, BasicCharacterObject character, Settings? settings, out int formationIndex)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			formationIndex = 0;
			if (settings == null)
			{
				return false;
			}
			TroopRole role = ((settings.UseSpawnedEquipmentClassification && agent != null) ? TroopRoleClassifier.Classify(agent) : TroopRoleClassifier.Classify(character));
			int[] roleAssignments = FormationAssignmentStore.GetRoleAssignments(role);
			if (roleAssignments.Length != 0)
			{
				formationIndex = roleAssignments[0];
				return true;
			}
			int num = (settings.UsePartyManagerRoleDefaults ? GetRoleFormation(role, settings) : GetNativeClassFormation(character.DefaultFormationClass, settings));
			if (!IsValidConfiguredFormation(num))
			{
				return false;
			}
			formationIndex = num - 1;
			return true;
		}

		private static bool IsValidConfiguredFormation(int configuredFormation)
		{
			if (configuredFormation >= 1)
			{
				return configuredFormation <= 8;
			}
			return false;
		}

		private static int GetVanillaFormationIndex(FormationClass formationClass)
		{
			switch (formationClass)
			{
			case FormationClass.Ranged:
				return 1;
			case FormationClass.Cavalry:
			case FormationClass.LightCavalry:
			case FormationClass.HeavyCavalry:
				return 2;
			case FormationClass.HorseArcher:
				return 3;
			default:
				return 0;
			}
		}

		private static int GetRoleFormation(TroopRole role, Settings settings)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected I4, but got Unknown
			return (int)role switch
			{
				0 => settings.LightInfantryFormation, 
				1 => settings.ShieldInfantryFormation, 
				2 => settings.ShockInfantryFormation, 
				3 => settings.SkirmisherFormation, 
				4 => settings.FootArcherFormation, 
				5 => settings.CrossbowmanFormation, 
				6 => settings.MeleeCavalryRoleFormation, 
				7 => settings.HorseArcherRoleFormation, 
				8 => settings.PikeInfantryFormation, 
				_ => settings.InfantryFormation, 
			};
		}

		private static int GetNativeClassFormation(FormationClass formationClass, Settings? settings)
		{
			switch (formationClass)
			{
			case FormationClass.Ranged:
				return settings?.ArcherFormation ?? 0;
			case FormationClass.HorseArcher:
				return settings?.CavalryFormation ?? 0;
			case FormationClass.Cavalry:
			case FormationClass.LightCavalry:
			case FormationClass.HeavyCavalry:
				return settings?.CavalryFormation ?? 0;
			default:
				return settings?.InfantryFormation ?? 0;
			}
		}

		public static int ToFormationIndex(int configuredFormation)
		{
			if (configuredFormation < 1)
			{
				return 0;
			}
			if (configuredFormation > 8)
			{
				return 7;
			}
			return configuredFormation - 1;
		}
	}
	public static class FormationAssignmentStore
	{
		private class StorageModel
		{
			public Dictionary<string, int> Assignments { get; set; } = new Dictionary<string, int>();


			public Dictionary<string, int> SecondaryAssignments { get; set; } = new Dictionary<string, int>();


			public Dictionary<string, TroopDeploymentPlan> DeploymentPlans { get; set; } = new Dictionary<string, TroopDeploymentPlan>();


			public Dictionary<string, TroopDeploymentPlan> ArchivedDeploymentPlans { get; set; } = new Dictionary<string, TroopDeploymentPlan>();


			public Dictionary<string, TroopDeploymentPlan> PausedDeploymentPlans { get; set; } = new Dictionary<string, TroopDeploymentPlan>();


			public Dictionary<string, RoleAssignment> RoleAssignments { get; set; } = new Dictionary<string, RoleAssignment>();


			public Dictionary<string, RoleAssignment> ArchivedRoleAssignments { get; set; } = new Dictionary<string, RoleAssignment>();

		}

		public class RoleAssignment
		{
			public int PrimaryFormation { get; set; } = -1;


			public int SecondaryFormation { get; set; } = -1;


			public List<int>? FormationIndices { get; set; }

			public int SplitSize { get; set; }

			public bool SplitEnabled { get; set; }
		}

		private static string? _currentHeroId;

		private static Dictionary<string, int> _assignments = new Dictionary<string, int>();

		private static Dictionary<string, int> _secondaryAssignments = new Dictionary<string, int>();

		private static Dictionary<string, TroopDeploymentPlan> _deploymentPlans = new Dictionary<string, TroopDeploymentPlan>();

		private static Dictionary<string, TroopDeploymentPlan> _archivedDeploymentPlans = new Dictionary<string, TroopDeploymentPlan>();

		private static Dictionary<string, TroopDeploymentPlan> _pausedDeploymentPlans = new Dictionary<string, TroopDeploymentPlan>();

		private static Dictionary<string, RoleAssignment> _roleAssignments = new Dictionary<string, RoleAssignment>();

		private static Dictionary<string, RoleAssignment> _archivedRoleAssignments = new Dictionary<string, RoleAssignment>();

		private static bool _isDirty;

		public static bool HasAnyRoleAssignments
		{
			get
			{
				TroopRole result;
				return _roleAssignments.Keys.Any((string key) => Enum.TryParse<TroopRole>(key, out result) && GetRoleAssignments(result).Length != 0);
			}
		}

		public static bool CanRestoreRoleAssignments
		{
			get
			{
				if (!HasAnyRoleAssignments)
				{
					return _archivedRoleAssignments.Count > 0;
				}
				return false;
			}
		}

		public static bool HasAnyAssignments
		{
			get
			{
				if (_assignments.Count <= 0 && _secondaryAssignments.Count <= 0)
				{
					return _deploymentPlans.Count > 0;
				}
				return true;
			}
		}

		private static string GetConfigDir()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mount and Blade II Bannerlord", "Configs", "FormationManager");
		}

		private static string GetFilePath(string heroId)
		{
			return Path.Combine(GetConfigDir(), heroId + ".json");
		}

		public static void Load(string heroId)
		{
			_currentHeroId = heroId;
			_assignments = new Dictionary<string, int>();
			_secondaryAssignments = new Dictionary<string, int>();
			_deploymentPlans = new Dictionary<string, TroopDeploymentPlan>();
			_archivedDeploymentPlans = new Dictionary<string, TroopDeploymentPlan>();
			_pausedDeploymentPlans = new Dictionary<string, TroopDeploymentPlan>();
			_roleAssignments = new Dictionary<string, RoleAssignment>();
			_archivedRoleAssignments = new Dictionary<string, RoleAssignment>();
			_isDirty = false;
			string filePath = GetFilePath(heroId);
			if (!File.Exists(filePath))
			{
				return;
			}
			try
			{
				StorageModel storageModel = JsonConvert.DeserializeObject<StorageModel>(File.ReadAllText(filePath));
				if (storageModel?.Assignments != null)
				{
					_assignments = storageModel.Assignments;
				}
				if (storageModel?.SecondaryAssignments != null)
				{
					_secondaryAssignments = storageModel.SecondaryAssignments;
				}
				if (storageModel?.DeploymentPlans != null)
				{
					_deploymentPlans = storageModel.DeploymentPlans;
				}
				if (storageModel?.ArchivedDeploymentPlans != null)
				{
					_archivedDeploymentPlans = storageModel.ArchivedDeploymentPlans;
				}
				if (storageModel?.PausedDeploymentPlans != null)
				{
					_pausedDeploymentPlans = storageModel.PausedDeploymentPlans;
				}
				if (storageModel?.RoleAssignments != null)
				{
					_roleAssignments = storageModel.RoleAssignments;
				}
				if (storageModel?.ArchivedRoleAssignments != null)
				{
					_archivedRoleAssignments = storageModel.ArchivedRoleAssignments;
				}
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage("[FormationManager] Failed to load assignment data: " + ex.Message, new Color(0.9f, 0.3f, 0.3f)));
			}
			SetAdvancedPlansEnabled(GlobalSettings<Settings>.Instance?.ShowAdvancedFormationEditor ?? true);
		}

		public static void Save()
		{
			if (_currentHeroId == null || !_isDirty)
			{
				return;
			}
			try
			{
				string configDir = GetConfigDir();
				if (!Directory.Exists(configDir))
				{
					Directory.CreateDirectory(configDir);
				}
				string contents = JsonConvert.SerializeObject(new StorageModel
				{
					Assignments = _assignments,
					SecondaryAssignments = _secondaryAssignments,
					DeploymentPlans = _deploymentPlans,
					ArchivedDeploymentPlans = _archivedDeploymentPlans,
					PausedDeploymentPlans = _pausedDeploymentPlans,
					RoleAssignments = _roleAssignments,
					ArchivedRoleAssignments = _archivedRoleAssignments
				}, Formatting.Indented);
				File.WriteAllText(GetFilePath(_currentHeroId), contents);
				_isDirty = false;
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage("[FormationManager] Failed to save assignment data: " + ex.Message, new Color(0.9f, 0.3f, 0.3f)));
			}
		}

		public static int GetAssignment(string troopId)
		{
			if (!_assignments.TryGetValue(troopId, out var value))
			{
				return -1;
			}
			return value;
		}

		public static void SetAssignment(string troopId, int formationIndex)
		{
			if (formationIndex < 0)
			{
				ClearAssignment(troopId);
				return;
			}
			_assignments[troopId] = formationIndex;
			if (_secondaryAssignments.TryGetValue(troopId, out var value) && value == formationIndex)
			{
				_secondaryAssignments.Remove(troopId);
			}
			_isDirty = true;
		}

		public static void ClearAssignment(string troopId)
		{
			if (_assignments.Remove(troopId) | _secondaryAssignments.Remove(troopId))
			{
				_isDirty = true;
			}
		}

		public static int[] GetAssignments(string troopId)
		{
			int value;
			bool flag = _assignments.TryGetValue(troopId, out value) && FormationPlanNormalizer.IsValidFormationIndex(value);
			int value2;
			bool flag2 = _secondaryAssignments.TryGetValue(troopId, out value2) && FormationPlanNormalizer.IsValidFormationIndex(value2) && (!flag || value2 != value);
			if (flag && flag2)
			{
				return new int[2] { value, value2 };
			}
			if (flag)
			{
				return new int[1] { value };
			}
			if (flag2)
			{
				return new int[1] { value2 };
			}
			return Array.Empty<int>();
		}

		public static int GetSecondaryAssignment(string troopId)
		{
			if (!_secondaryAssignments.TryGetValue(troopId, out var value))
			{
				return -1;
			}
			return value;
		}

		public static void SetSecondaryAssignment(string troopId, int formationIndex)
		{
			if (!FormationPlanNormalizer.IsValidFormationIndex(formationIndex))
			{
				ClearSecondaryAssignment(troopId);
				return;
			}
			if (!_assignments.TryGetValue(troopId, out var value) || !FormationPlanNormalizer.IsValidFormationIndex(value))
			{
				SetAssignment(troopId, formationIndex);
				return;
			}
			if (value == formationIndex)
			{
				ClearSecondaryAssignment(troopId);
				return;
			}
			_secondaryAssignments[troopId] = formationIndex;
			_isDirty = true;
		}

		public static void ClearSecondaryAssignment(string troopId)
		{
			if (_secondaryAssignments.Remove(troopId))
			{
				_isDirty = true;
			}
		}

		public static int[] GetRoleAssignments(TroopRole role)
		{
			if (!_roleAssignments.TryGetValue(((object)(TroopRole)(ref role)).ToString(), out RoleAssignment value))
			{
				return Array.Empty<int>();
			}
			if (value.FormationIndices != null && value.FormationIndices.Count > 0)
			{
				return (from index in value.FormationIndices.Where(FormationPlanNormalizer.IsValidFormationIndex).Distinct()
					orderby index
					select index).ToArray();
			}
			bool flag = FormationPlanNormalizer.IsValidFormationIndex(value.PrimaryFormation);
			bool flag2 = FormationPlanNormalizer.IsValidFormationIndex(value.SecondaryFormation) && value.SecondaryFormation != value.PrimaryFormation;
			if (flag && flag2)
			{
				return new int[2] { value.PrimaryFormation, value.SecondaryFormation };
			}
			if (flag)
			{
				return new int[1] { value.PrimaryFormation };
			}
			if (flag2)
			{
				return new int[1] { value.SecondaryFormation };
			}
			return Array.Empty<int>();
		}

		public static bool GetRoleSplitEnabled(TroopRole role)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			if (!_roleAssignments.TryGetValue(((object)(TroopRole)(ref role)).ToString(), out RoleAssignment value))
			{
				return false;
			}
			if (value.SplitEnabled)
			{
				return true;
			}
			if (value.SplitSize <= 0)
			{
				return GetRoleAssignments(role).Length != 0;
			}
			return true;
		}

		public static void SetRoleSplitEnabled(TroopRole role, bool enabled)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			GetOrCreateRoleAssignment(role).SplitEnabled = enabled;
			_isDirty = true;
		}

		public static void ClearRoleAssignments()
		{
			if (_roleAssignments.Count != 0)
			{
				_archivedRoleAssignments = CloneRoleAssignments(_roleAssignments);
				_roleAssignments.Clear();
				_isDirty = true;
			}
		}

		public static bool RestoreRoleAssignments()
		{
			if (!CanRestoreRoleAssignments)
			{
				return false;
			}
			_roleAssignments = CloneRoleAssignments(_archivedRoleAssignments);
			_archivedRoleAssignments.Clear();
			_isDirty = true;
			return true;
		}

		public static void SetRoleAssignment(TroopRole role, int formationIndex)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (!FormationPlanNormalizer.IsValidFormationIndex(formationIndex))
			{
				ClearRoleAssignment(role);
				return;
			}
			RoleAssignment orCreateRoleAssignment = GetOrCreateRoleAssignment(role);
			orCreateRoleAssignment.PrimaryFormation = formationIndex;
			if (orCreateRoleAssignment.SecondaryFormation == formationIndex)
			{
				orCreateRoleAssignment.SecondaryFormation = -1;
			}
			orCreateRoleAssignment.FormationIndices = null;
			_isDirty = true;
		}

		public static void SetSecondaryRoleAssignment(TroopRole role, int formationIndex)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			RoleAssignment orCreateRoleAssignment = GetOrCreateRoleAssignment(role);
			if (!FormationPlanNormalizer.IsValidFormationIndex(formationIndex) || formationIndex == orCreateRoleAssignment.PrimaryFormation)
			{
				orCreateRoleAssignment.SecondaryFormation = -1;
			}
			else
			{
				orCreateRoleAssignment.SecondaryFormation = formationIndex;
			}
			orCreateRoleAssignment.FormationIndices = null;
			_isDirty = true;
		}

		public static void SetRoleAssignments(TroopRole role, IEnumerable<int> formationIndices)
		{
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			int[] array = (from index in formationIndices.Where(FormationPlanNormalizer.IsValidFormationIndex).Distinct()
				orderby index
				select index).ToArray();
			if (array.Length == 0)
			{
				if (_roleAssignments.TryGetValue(((object)(TroopRole)(ref role)).ToString(), out RoleAssignment value) && (value.SplitEnabled || value.SplitSize > 0))
				{
					value.PrimaryFormation = -1;
					value.SecondaryFormation = -1;
					value.FormationIndices = null;
					_isDirty = true;
				}
				else
				{
					ClearRoleAssignment(role);
				}
			}
			else
			{
				RoleAssignment orCreateRoleAssignment = GetOrCreateRoleAssignment(role);
				orCreateRoleAssignment.FormationIndices = array.ToList();
				orCreateRoleAssignment.PrimaryFormation = array[0];
				orCreateRoleAssignment.SecondaryFormation = ((array.Length > 1) ? array[1] : (-1));
				_isDirty = true;
			}
		}

		public static void ClearRoleAssignment(TroopRole role)
		{
			if (_roleAssignments.Remove(((object)(TroopRole)(ref role)).ToString()))
			{
				_isDirty = true;
			}
		}

		public static void ClearSecondaryRoleAssignment(TroopRole role)
		{
			if (_roleAssignments.TryGetValue(((object)(TroopRole)(ref role)).ToString(), out RoleAssignment value) && value.SecondaryFormation >= 0)
			{
				value.SecondaryFormation = -1;
				_isDirty = true;
			}
		}

		private static RoleAssignment GetOrCreateRoleAssignment(TroopRole role)
		{
			string key = ((object)(TroopRole)(ref role)).ToString();
			if (!_roleAssignments.TryGetValue(key, out RoleAssignment value))
			{
				value = new RoleAssignment();
				_roleAssignments[key] = value;
			}
			return value;
		}

		private static Dictionary<string, RoleAssignment> CloneRoleAssignments(Dictionary<string, RoleAssignment> source)
		{
			return source.ToDictionary<KeyValuePair<string, RoleAssignment>, string, RoleAssignment>((KeyValuePair<string, RoleAssignment> pair) => pair.Key, (KeyValuePair<string, RoleAssignment> pair) => new RoleAssignment
			{
				PrimaryFormation = pair.Value.PrimaryFormation,
				SecondaryFormation = pair.Value.SecondaryFormation,
				FormationIndices = pair.Value.FormationIndices?.ToList(),
				SplitSize = pair.Value.SplitSize,
				SplitEnabled = pair.Value.SplitEnabled
			});
		}

		public static bool TryGetDeploymentPlan(string troopId, out TroopDeploymentPlan? plan)
		{
			if (_deploymentPlans.TryGetValue(troopId, out TroopDeploymentPlan value))
			{
				if (FormationPlanNormalizer.Normalize(value))
				{
					_isDirty = true;
				}
				if (value.IsValid())
				{
					plan = value;
					return true;
				}
			}
			plan = null;
			return false;
		}

		public static void SaveCustomPlan(string troopId, IReadOnlyDictionary<int, int> targets, IReadOnlyDictionary<int, int> weights)
		{
			TroopDeploymentPlan troopDeploymentPlan = TroopDeploymentPlan.CreateCustom();
			foreach (KeyValuePair<int, int> item in targets.Where((KeyValuePair<int, int> pair) => FormationPlanNormalizer.IsValidFormationIndex(pair.Key) && pair.Value > 0))
			{
				troopDeploymentPlan.FormationTargets[item.Key] = item.Value;
			}
			foreach (KeyValuePair<int, int> item2 in weights.Where((KeyValuePair<int, int> pair) => FormationPlanNormalizer.IsValidFormationIndex(pair.Key) && pair.Value > 0))
			{
				troopDeploymentPlan.FormationWeights[item2.Key] = Math.Min(100, item2.Value);
			}
			if (troopDeploymentPlan.IsValid())
			{
				_deploymentPlans[troopId] = troopDeploymentPlan;
			}
			else
			{
				_deploymentPlans.Remove(troopId);
			}
			_archivedDeploymentPlans.Remove(troopId);
			_pausedDeploymentPlans.Remove(troopId);
			_isDirty = true;
		}

		public static void ClearDeploymentPlan(string troopId)
		{
			if (_deploymentPlans.TryGetValue(troopId, out TroopDeploymentPlan value))
			{
				_archivedDeploymentPlans[troopId] = value;
				_deploymentPlans.Remove(troopId);
				_isDirty = true;
			}
		}

		public static bool CanRestoreDeploymentPlan(string troopId)
		{
			if (TryGetArchivedDeploymentPlan(troopId, out TroopDeploymentPlan plan))
			{
				return plan.IsValid();
			}
			return false;
		}

		public static bool TryGetArchivedDeploymentPlanCopy(string troopId, out TroopDeploymentPlan? planCopy)
		{
			planCopy = null;
			if (!TryGetArchivedDeploymentPlan(troopId, out TroopDeploymentPlan plan))
			{
				return false;
			}
			FormationPlanNormalizer.Normalize(plan);
			if (!plan.IsValid())
			{
				return false;
			}
			planCopy = FormationPlanNormalizer.Clone(plan);
			return true;
		}

		public static bool RestoreDeploymentPlan(string troopId)
		{
			if (_deploymentPlans.ContainsKey(troopId) || !TryGetArchivedDeploymentPlan(troopId, out TroopDeploymentPlan plan))
			{
				return false;
			}
			FormationPlanNormalizer.Normalize(plan);
			if (!plan.IsValid())
			{
				_archivedDeploymentPlans.Remove(troopId);
				_pausedDeploymentPlans.Remove(troopId);
				_isDirty = true;
				return false;
			}
			_deploymentPlans[troopId] = plan;
			_archivedDeploymentPlans.Remove(troopId);
			_pausedDeploymentPlans.Remove(troopId);
			_isDirty = true;
			return true;
		}

		private static bool TryGetArchivedDeploymentPlan(string troopId, out TroopDeploymentPlan plan)
		{
			if (_archivedDeploymentPlans.TryGetValue(troopId, out plan))
			{
				return true;
			}
			return _pausedDeploymentPlans.TryGetValue(troopId, out plan);
		}

		public static void SetAdvancedPlansEnabled(bool enabled)
		{
			bool flag = false;
			if (!enabled)
			{
				foreach (KeyValuePair<string, TroopDeploymentPlan> item in _deploymentPlans.ToList())
				{
					_pausedDeploymentPlans[item.Key] = item.Value;
					_deploymentPlans.Remove(item.Key);
					flag = true;
				}
			}
			if (flag)
			{
				_isDirty = true;
			}
		}
	}
	internal interface IFormationRemainderStrategy
	{
		void RecordBaseAllocation(int formationIndex, int count);

		int SelectRecipient(IReadOnlyDictionary<int, int> remainders, ISet<int> excludedRecipients);

		void RecordRemainderAllocation(int formationIndex);
	}
	internal static class FormationPlanAllocator
	{
		public static Dictionary<int, int> AllocateEvenly(IEnumerable<int> formationIndices, int troopCount, IFormationRemainderStrategy? remainderStrategy = null)
		{
			int[] array = (from index in formationIndices.Where(FormationPlanNormalizer.IsValidFormationIndex).Distinct()
				orderby index
				select index).ToArray();
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			if (troopCount <= 0 || array.Length == 0)
			{
				return dictionary;
			}
			int count = troopCount / array.Length;
			int[] array2 = array;
			foreach (int formationIndex in array2)
			{
				AddCount(dictionary, formationIndex, count);
				remainderStrategy?.RecordBaseAllocation(formationIndex, count);
			}
			Dictionary<int, int> remainders = array.ToDictionary((int index) => index, (int _) => 0);
			HashSet<int> hashSet = new HashSet<int>();
			for (int num = troopCount % array.Length; num > 0; num--)
			{
				int num2 = SelectRecipient(remainders, hashSet, remainderStrategy);
				AddCount(dictionary, num2, 1);
				remainderStrategy?.RecordRemainderAllocation(num2);
				hashSet.Add(num2);
			}
			return dictionary;
		}

		public static Dictionary<int, int> AllocateCustom(TroopDeploymentPlan plan, int troopCount, int fallbackIndex, bool prioritizeWeightsInSmallStacks, IFormationRemainderStrategy? remainderStrategy = null)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			if (troopCount <= 0)
			{
				return dictionary;
			}
			Dictionary<int, int> dictionary2 = PositiveEntries(plan.FormationTargets);
			Dictionary<int, int> dictionary3 = PositiveEntries(plan.FormationWeights);
			int[] array = (from index in dictionary2.Keys.Concat(dictionary3.Keys).Distinct()
				orderby index
				select index).ToArray();
			if (array.Length == 0)
			{
				dictionary[fallbackIndex] = troopCount;
				return dictionary;
			}
			int num = dictionary2.Values.Sum();
			if (num == 0 && dictionary3.Count > 0 && !prioritizeWeightsInSmallStacks)
			{
				int troopCount2 = GiveOneToEachActive(dictionary, array, troopCount);
				AllocateProportionally(dictionary, dictionary3, troopCount2, remainderStrategy);
				return dictionary;
			}
			if (troopCount >= num)
			{
				foreach (KeyValuePair<int, int> item in dictionary2)
				{
					dictionary[item.Key] = item.Value;
				}
				int troopCount3 = troopCount - num;
				AllocateProportionally(dictionary, (dictionary3.Count > 0) ? dictionary3 : dictionary2, troopCount3, remainderStrategy);
				return dictionary;
			}
			if (prioritizeWeightsInSmallStacks)
			{
				AllocateProportionally(dictionary, (dictionary3.Count > 0) ? dictionary3 : dictionary2, troopCount, remainderStrategy);
				return dictionary;
			}
			int troopCount4 = GiveOneToEachActive(dictionary, array, troopCount);
			AllocateProportionally(dictionary, (dictionary3.Count > 0) ? dictionary3 : dictionary2, troopCount4, remainderStrategy);
			return dictionary;
		}

		private static Dictionary<int, int> PositiveEntries(IReadOnlyDictionary<int, int> source)
		{
			return (from pair in source
				where FormationPlanNormalizer.IsValidFormationIndex(pair.Key) && pair.Value > 0
				orderby pair.Key
				select pair).ToDictionary((KeyValuePair<int, int> pair) => pair.Key, (KeyValuePair<int, int> pair) => pair.Value);
		}

		private static int GiveOneToEachActive(Dictionary<int, int> allocation, IEnumerable<int> activeIndices, int troopCount)
		{
			int num = troopCount;
			foreach (int activeIndex in activeIndices)
			{
				if (num == 0)
				{
					break;
				}
				AddCount(allocation, activeIndex, 1);
				num--;
			}
			return num;
		}

		private static void AllocateProportionally(Dictionary<int, int> allocation, IReadOnlyDictionary<int, int> values, int troopCount, IFormationRemainderStrategy? remainderStrategy)
		{
			if (troopCount <= 0 || values.Count == 0)
			{
				return;
			}
			int num = values.Values.Sum((int value) => Math.Max(0, value));
			if (num <= 0)
			{
				return;
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			int i = 0;
			foreach (KeyValuePair<int, int> item in values.OrderBy((KeyValuePair<int, int> pair) => pair.Key))
			{
				int num2 = troopCount * item.Value;
				int num3 = num2 / num;
				AddCount(allocation, item.Key, num3);
				remainderStrategy?.RecordBaseAllocation(item.Key, num3);
				i += num3;
				dictionary[item.Key] = num2 % num;
			}
			HashSet<int> hashSet = new HashSet<int>();
			for (; i < troopCount; i++)
			{
				int num4 = SelectRecipient(dictionary, hashSet, remainderStrategy);
				AddCount(allocation, num4, 1);
				remainderStrategy?.RecordRemainderAllocation(num4);
				hashSet.Add(num4);
			}
		}

		private static int SelectRecipient(IReadOnlyDictionary<int, int> remainders, ISet<int> excludedRecipients, IFormationRemainderStrategy? remainderStrategy)
		{
			ISet<int> excludedRecipients2 = excludedRecipients;
			int num = remainderStrategy?.SelectRecipient(remainders, excludedRecipients2) ?? (-1);
			if (remainders.ContainsKey(num) && !excludedRecipients2.Contains(num))
			{
				return num;
			}
			return (from pair in remainders
				where !excludedRecipients2.Contains(pair.Key)
				orderby pair.Value descending, pair.Key
				select pair).First().Key;
		}

		private static void AddCount(Dictionary<int, int> allocation, int formationIndex, int count)
		{
			if (count > 0)
			{
				allocation[formationIndex] = (allocation.TryGetValue(formationIndex, out var value) ? (value + count) : count);
			}
		}
	}
	public static class Logger
	{
		private static readonly object LogLock = new object();

		public static void Log(string message)
		{
			try
			{
				lock (LogLock)
				{
					string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mount and Blade II Bannerlord", "Configs", "FormationManager");
					if (!Directory.Exists(text))
					{
						Directory.CreateDirectory(text);
					}
					File.AppendAllText(Path.Combine(text, "log.txt"), $"[{DateTime.Now:HH:mm:ss.fff}] {message}{Environment.NewLine}");
				}
			}
			catch
			{
			}
		}
	}
	public sealed class TroopDeploymentPlan
	{
		public TroopDeploymentPlanMode Mode { get; set; }

		public List<int> FormationIndices { get; set; } = new List<int>();


		public Dictionary<int, int> FormationCounts { get; set; } = new Dictionary<int, int>();


		public Dictionary<int, int> FormationTargets { get; set; } = new Dictionary<int, int>();


		public Dictionary<int, int> FormationWeights { get; set; } = new Dictionary<int, int>();


		public static TroopDeploymentPlan CreateEven(IEnumerable<int> formationIndices)
		{
			return new TroopDeploymentPlan
			{
				Mode = TroopDeploymentPlanMode.Even,
				FormationIndices = (from index in formationIndices.Distinct()
					orderby index
					select index).ToList()
			};
		}

		public static TroopDeploymentPlan CreateCustom()
		{
			return new TroopDeploymentPlan
			{
				Mode = TroopDeploymentPlanMode.Custom
			};
		}

		public bool IsValid()
		{
			if (Mode != 0)
			{
				if (!FormationTargets.Any((KeyValuePair<int, int> pair) => FormationPlanNormalizer.IsValidFormationIndex(pair.Key) && pair.Value > 0))
				{
					return FormationWeights.Any((KeyValuePair<int, int> pair) => FormationPlanNormalizer.IsValidFormationIndex(pair.Key) && pair.Value > 0);
				}
				return true;
			}
			if (FormationIndices.Count >= 2)
			{
				return FormationIndices.All(FormationPlanNormalizer.IsValidFormationIndex);
			}
			return false;
		}
	}
	public enum TroopDeploymentPlanMode
	{
		Even,
		Custom
	}
	internal static class FormationPlanNormalizer
	{
		public static bool IsValidFormationIndex(int formationIndex)
		{
			if (formationIndex >= 0)
			{
				return formationIndex <= 7;
			}
			return false;
		}

		public static bool Normalize(TroopDeploymentPlan plan)
		{
			TroopDeploymentPlan plan2 = plan;
			if (plan2.Mode != TroopDeploymentPlanMode.Custom)
			{
				return false;
			}
			bool result = false;
			TroopDeploymentPlan troopDeploymentPlan = plan2;
			if (troopDeploymentPlan.FormationTargets == null)
			{
				Dictionary<int, int> dictionary2 = (troopDeploymentPlan.FormationTargets = new Dictionary<int, int>());
			}
			troopDeploymentPlan = plan2;
			if (troopDeploymentPlan.FormationWeights == null)
			{
				Dictionary<int, int> dictionary2 = (troopDeploymentPlan.FormationWeights = new Dictionary<int, int>());
			}
			troopDeploymentPlan = plan2;
			if (troopDeploymentPlan.FormationCounts == null)
			{
				Dictionary<int, int> dictionary2 = (troopDeploymentPlan.FormationCounts = new Dictionary<int, int>());
			}
			foreach (KeyValuePair<int, int> item in plan2.FormationCounts.Where((KeyValuePair<int, int> pair) => IsValidFormationIndex(pair.Key) && pair.Value > 0))
			{
				if (!plan2.FormationTargets.ContainsKey(item.Key))
				{
					plan2.FormationTargets[item.Key] = item.Value;
					result = true;
				}
			}
			if (plan2.FormationCounts.Count > 0)
			{
				plan2.FormationCounts.Clear();
				result = true;
			}
			foreach (int item2 in plan2.FormationTargets.Keys.Where((int key) => !IsValidFormationIndex(key) || plan2.FormationTargets[key] <= 0).ToList())
			{
				plan2.FormationTargets.Remove(item2);
				result = true;
			}
			foreach (int item3 in plan2.FormationWeights.Keys.Where((int key) => !IsValidFormationIndex(key) || plan2.FormationWeights[key] <= 0).ToList())
			{
				plan2.FormationWeights.Remove(item3);
				result = true;
			}
			return result;
		}

		public static TroopDeploymentPlan Clone(TroopDeploymentPlan plan)
		{
			return new TroopDeploymentPlan
			{
				Mode = plan.Mode,
				FormationIndices = plan.FormationIndices.ToList(),
				FormationCounts = plan.FormationCounts.ToDictionary((KeyValuePair<int, int> pair) => pair.Key, (KeyValuePair<int, int> pair) => pair.Value),
				FormationTargets = plan.FormationTargets.ToDictionary((KeyValuePair<int, int> pair) => pair.Key, (KeyValuePair<int, int> pair) => pair.Value),
				FormationWeights = plan.FormationWeights.ToDictionary((KeyValuePair<int, int> pair) => pair.Key, (KeyValuePair<int, int> pair) => pair.Value)
			};
		}
	}
}
