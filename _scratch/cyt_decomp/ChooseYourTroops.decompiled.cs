#define TRACE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using HarmonyLib;
using Helpers;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using Microsoft.CodeAnalysis;
using SandBox.Missions.MissionLogics;
using SandBox.View;
using SandBox.View.Map;
using SandBox.View.Menu;
using SandBox.ViewModelCollection.Input;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.CampaignSystem.TroopSuppliers;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TroopSelection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints | DebuggableAttribute.DebuggingModes.EnableEditAndContinue)]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyCompany("ChooseYourTroops")]
[assembly: AssemblyConfiguration("Debug")]
[assembly: AssemblyFileVersion("0.0.1.0")]
[assembly: AssemblyInformationalVersion("0.0.1")]
[assembly: AssemblyProduct("ChooseYourTroops")]
[assembly: AssemblyTitle("ChooseYourTroops")]
[assembly: AssemblyVersion("0.0.1.0")]
namespace Microsoft.CodeAnalysis
{
	[CompilerGenerated]
	[Embedded]
	internal sealed class EmbeddedAttribute : Attribute
	{
	}
}
namespace System.Runtime.CompilerServices
{
	[CompilerGenerated]
	[Embedded]
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
	[Embedded]
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
namespace ChooseYourTroops
{
	[HarmonyPatch(typeof(SandBoxBattleMissionSpawnHandler), "AfterStart")]
	internal class SandBoxBattleMissionSpawnHandlerPatch
	{
		private static MethodInfo CreateSandBoxBattleWaveSpawnSettingsMethod = AccessTools.Method(typeof(SandBoxMissionSpawnHandler), "CreateSandBoxBattleWaveSpawnSettings", (Type[])null, (Type[])null);

		[HarmonyPrefix]
		private static bool Prefix(ref DefaultBattleMissionAgentSpawnLogic ____missionAgentSpawnLogic, ref MapEvent ____mapEvent)
		{
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			if (____mapEvent != null)
			{
				int battleSize = ____missionAgentSpawnLogic.BattleSize;
				int numberOfInvolvedMen = ____mapEvent.GetNumberOfInvolvedMen((BattleSideEnum)0);
				int numberOfInvolvedMen2 = ____mapEvent.GetNumberOfInvolvedMen((BattleSideEnum)1);
				int num = numberOfInvolvedMen;
				int num2 = numberOfInvolvedMen2;
				int num3 = num + num2;
				if (num3 > battleSize)
				{
					float defenderAdvantageFactor = (float)battleSize / ((float)num * ((float)battleSize * 2f / (float)num3));
					if ((float)num < (float)battleSize / 2f)
					{
						defenderAdvantageFactor = (float)num3 / (float)battleSize;
					}
					____missionAgentSpawnLogic.SetSpawnHorses((BattleSideEnum)0, !Mission.Current.IsSiegeBattle);
					____missionAgentSpawnLogic.SetSpawnHorses((BattleSideEnum)1, !Mission.Current.IsSiegeBattle);
					MissionSpawnSettings val = (MissionSpawnSettings)CreateSandBoxBattleWaveSpawnSettingsMethod.Invoke(null, new object[0]);
					((MissionSpawnSettings)(ref val)).DefenderAdvantageFactor = defenderAdvantageFactor;
					____missionAgentSpawnLogic.InitWithSinglePhase(numberOfInvolvedMen, numberOfInvolvedMen2, num, num2, true, true, ref val);
					return false;
				}
				return true;
			}
			return true;
		}
	}
	[HarmonyPatch(typeof(SandBoxSiegeMissionSpawnHandler), "AfterStart")]
	internal class SandBoxSiegeMissionSpawnHandlerPatch
	{
		[HarmonyPrefix]
		private static bool Prefix(ref DefaultBattleMissionAgentSpawnLogic ____missionAgentSpawnLogic, ref MapEvent ____mapEvent)
		{
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			if (____mapEvent != null)
			{
				int battleSize = ____missionAgentSpawnLogic.BattleSize;
				int numberOfInvolvedMen = ____mapEvent.GetNumberOfInvolvedMen((BattleSideEnum)0);
				int numberOfInvolvedMen2 = ____mapEvent.GetNumberOfInvolvedMen((BattleSideEnum)1);
				int num = numberOfInvolvedMen;
				int num2 = numberOfInvolvedMen2;
				int num3 = num + num2;
				if (num3 > battleSize)
				{
					float defenderAdvantageFactor = (float)battleSize / ((float)num * ((float)battleSize * 2f / (float)num3));
					if ((float)num < (float)battleSize / 2f)
					{
						defenderAdvantageFactor = (float)num3 / (float)battleSize;
					}
					____missionAgentSpawnLogic.SetSpawnHorses((BattleSideEnum)0, false);
					____missionAgentSpawnLogic.SetSpawnHorses((BattleSideEnum)1, false);
					MissionSpawnSettings val = MissionSpawnSettings.CreateDefaultSpawnSettings();
					((MissionSpawnSettings)(ref val)).DefenderAdvantageFactor = defenderAdvantageFactor;
					____missionAgentSpawnLogic.InitWithSinglePhase(numberOfInvolvedMen, numberOfInvolvedMen2, num, num2, true, true, ref val);
					return false;
				}
				return true;
			}
			return true;
		}
	}
	internal sealed class ChooseYourTroopsConfig : AttributeGlobalSettings<ChooseYourTroopsConfig>
	{
		private static readonly Dictionary<int, int> battleSizes = new Dictionary<int, int>
		{
			{ 0, 200 },
			{ 1, 300 },
			{ 2, 400 },
			{ 3, 500 },
			{ 4, 600 },
			{ 5, 800 },
			{ 6, 1000 }
		};

		public static int CurrentBattleSizeLimit
		{
			get
			{
				ChooseYourTroopsConfig instance = GlobalSettings<ChooseYourTroopsConfig>.Instance;
				return (instance != null && instance.SupportForMaximumTroops) ? 2040 : battleSizes[BannerlordConfig.BattleSize];
			}
		}

		public override string Id => "choose_your_troops";

		public override string DisplayName => "Choose Your Troops";

		public override string FolderName => "ChooseYourTroops";

		public override string FormatType => "json";

		[SettingPropertyGroup("{=general}General", GroupOrder = 1)]
		[SettingPropertyBool("{=max_battle_size_support_option}Support for maximum battle size mods (READ DESCRIPTION)", Order = 1, RequireRestart = false, HintText = "{=max_battle_size_support_option_description}Allows to choose the maximum troops allowed by engine in battle (2040), another mod is required to handle it in battle like BattleSize Resized.")]
		public bool SupportForMaximumTroops { get; set; } = false;

	}
	public class CYTGameMenuTroopSelectionVM : ViewModel
	{
		private Dictionary<TroopSelectionItemVM, int> _maxAmounts = new Dictionary<TroopSelectionItemVM, int>();

		private List<TroopSelectionItemVM> _troopsCountedByTwo = new List<TroopSelectionItemVM>();

		private SpriteCategory _category;

		private bool _isAscending;

		private int _infantryAmount;

		private int _archerAmount;

		private int _cavalryAmount;

		private int _horseArcherAmount;

		private SelectorVM<SelectorItemVM> _sortSelector;

		private string _OrderByTierText;

		private string _OrderByNameText;

		private string _OrderByCountText;

		private string _OrderByCultureText;

		private string _OrderByClassText;

		private readonly Action<TroopRoster> _onDone;

		private readonly TroopRoster _fullRoster;

		private readonly TroopRoster _initialSelections;

		private readonly Func<CharacterObject, bool> _canChangeChangeStatusOfTroop;

		private readonly int _maxSelectableTroopCount;

		private readonly int _minSelectableTroopCount;

		private readonly TextObject _titleTextObject = new TextObject("{=cyt_initial_troops}Initial Troops", (Dictionary<string, object>)null);

		private readonly TextObject _chosenTitleTextObject = new TextObject("{=cyt_chosen_troops}Chosen Troops", (Dictionary<string, object>)null);

		private int _currentTotalSelectedTroopCount;

		public bool IsFiveStackModifierActive;

		public bool IsEntireStackModifierActive;

		private InputKeyItemVM _doneInputKey;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _resetInputKey;

		private bool _isEnabled;

		private bool _isDoneEnabled;

		private string _doneText;

		private string _cancelText;

		private string _titleText;

		private string _clearSelectionText;

		private string _automaticSelectionText;

		private string _resetToSavedText;

		private string _dismissSelectionText;

		private string _currentSelectedAmountText;

		private string _currentSelectedAmountTitle;

		private MBBindingList<TroopSelectionItemVM> _troops;

		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return _doneInputKey;
			}
			set
			{
				if (value != _doneInputKey)
				{
					_doneInputKey = value;
					((ViewModel)this).OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAscending
		{
			get
			{
				return _isAscending;
			}
			set
			{
				if (value != _isAscending)
				{
					_isAscending = value;
					((ViewModel)this).OnPropertyChangedWithValue(value, "IsAscending");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return _cancelInputKey;
			}
			set
			{
				if (value != _cancelInputKey)
				{
					_cancelInputKey = value;
					((ViewModel)this).OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM ResetInputKey
		{
			get
			{
				return _resetInputKey;
			}
			set
			{
				if (value != _resetInputKey)
				{
					_resetInputKey = value;
					((ViewModel)this).OnPropertyChangedWithValue<InputKeyItemVM>(value, "ResetInputKey");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return _isEnabled;
			}
			set
			{
				if (value != _isEnabled)
				{
					_isEnabled = value;
					((ViewModel)this).OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsDoneEnabled
		{
			get
			{
				return _isDoneEnabled;
			}
			set
			{
				if (value != _isDoneEnabled)
				{
					_isDoneEnabled = value;
					((ViewModel)this).OnPropertyChangedWithValue(value, "IsDoneEnabled");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<TroopSelectionItemVM> Troops
		{
			get
			{
				return _troops;
			}
			set
			{
				if (value != _troops)
				{
					_troops = value;
					((ViewModel)this).OnPropertyChangedWithValue<MBBindingList<TroopSelectionItemVM>>(value, "Troops");
				}
			}
		}

		[DataSourceProperty]
		public string DoneText
		{
			get
			{
				return _doneText;
			}
			set
			{
				if (value != _doneText)
				{
					_doneText = value;
					((ViewModel)this).OnPropertyChangedWithValue<string>(value, "DoneText");
				}
			}
		}

		[DataSourceProperty]
		public string CancelText
		{
			get
			{
				return _cancelText;
			}
			set
			{
				if (value != _cancelText)
				{
					_cancelText = value;
					((ViewModel)this).OnPropertyChangedWithValue<string>(value, "CancelText");
				}
			}
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return _titleText;
			}
			set
			{
				if (value != _titleText)
				{
					_titleText = value;
					((ViewModel)this).OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public string ClearSelectionText
		{
			get
			{
				return _clearSelectionText;
			}
			set
			{
				if (value != _clearSelectionText)
				{
					_clearSelectionText = value;
					((ViewModel)this).OnPropertyChangedWithValue<string>(value, "ClearSelectionText");
				}
			}
		}

		[DataSourceProperty]
		public string AutomaticSelectionText
		{
			get
			{
				return _automaticSelectionText;
			}
			set
			{
				if (value != _automaticSelectionText)
				{
					_automaticSelectionText = value;
					((ViewModel)this).OnPropertyChangedWithValue<string>(value, "AutomaticSelectionText");
				}
			}
		}

		[DataSourceProperty]
		public string ResetToSavedText
		{
			get
			{
				return _resetToSavedText;
			}
			set
			{
				if (value != _resetToSavedText)
				{
					_resetToSavedText = value;
					((ViewModel)this).OnPropertyChangedWithValue<string>(value, "ResetToSavedText");
				}
			}
		}

		[DataSourceProperty]
		public string DismissSelectionText
		{
			get
			{
				return _dismissSelectionText;
			}
			set
			{
				if (value != _dismissSelectionText)
				{
					_dismissSelectionText = value;
					((ViewModel)this).OnPropertyChangedWithValue<string>(value, "DismissSelectionText");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentSelectedAmountText
		{
			get
			{
				return _currentSelectedAmountText;
			}
			set
			{
				if (value != _currentSelectedAmountText)
				{
					_currentSelectedAmountText = value;
					((ViewModel)this).OnPropertyChangedWithValue<string>(value, "CurrentSelectedAmountText");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentSelectedAmountTitle
		{
			get
			{
				return _currentSelectedAmountTitle;
			}
			set
			{
				if (value != _currentSelectedAmountTitle)
				{
					_currentSelectedAmountTitle = value;
					((ViewModel)this).OnPropertyChangedWithValue<string>(value, "CurrentSelectedAmountTitle");
				}
			}
		}

		[DataSourceProperty]
		public int InfantryAmount
		{
			get
			{
				return _infantryAmount;
			}
			set
			{
				if (value != _infantryAmount)
				{
					_infantryAmount = value;
					((ViewModel)this).OnPropertyChangedWithValue(value, "InfantryAmount");
				}
			}
		}

		[DataSourceProperty]
		public int ArcherAmount
		{
			get
			{
				return _archerAmount;
			}
			set
			{
				if (value != _archerAmount)
				{
					_archerAmount = value;
					((ViewModel)this).OnPropertyChangedWithValue(value, "ArcherAmount");
				}
			}
		}

		[DataSourceProperty]
		public int CavalryAmount
		{
			get
			{
				return _cavalryAmount;
			}
			set
			{
				if (value != _cavalryAmount)
				{
					_cavalryAmount = value;
					((ViewModel)this).OnPropertyChangedWithValue(value, "CavalryAmount");
				}
			}
		}

		[DataSourceProperty]
		public int HorseArcherAmount
		{
			get
			{
				return _horseArcherAmount;
			}
			set
			{
				if (value != _horseArcherAmount)
				{
					_horseArcherAmount = value;
					((ViewModel)this).OnPropertyChangedWithValue(value, "HorseArcherAmount");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> SortSelector
		{
			get
			{
				return _sortSelector;
			}
			set
			{
				if (value != _sortSelector)
				{
					_sortSelector = value;
					((ViewModel)this).OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "SortSelector");
				}
			}
		}

		[DataSourceProperty]
		public string OrderByTierText
		{
			get
			{
				return _OrderByTierText;
			}
			set
			{
				if (value != _OrderByTierText)
				{
					_OrderByTierText = value;
					((ViewModel)this).OnPropertyChangedWithValue<string>(value, "OrderByTierText");
				}
			}
		}

		[DataSourceProperty]
		public string OrderByNameText
		{
			get
			{
				return _OrderByNameText;
			}
			set
			{
				if (value != _OrderByNameText)
				{
					_OrderByNameText = value;
					((ViewModel)this).OnPropertyChangedWithValue<string>(value, "OrderByNameText");
				}
			}
		}

		[DataSourceProperty]
		public string OrderByCountText
		{
			get
			{
				return _OrderByCountText;
			}
			set
			{
				if (value != _OrderByCountText)
				{
					_OrderByCountText = value;
					((ViewModel)this).OnPropertyChangedWithValue<string>(value, "OrderByCountText");
				}
			}
		}

		[DataSourceProperty]
		public string OrderByCultureText
		{
			get
			{
				return _OrderByCultureText;
			}
			set
			{
				if (value != _OrderByCultureText)
				{
					_OrderByCultureText = value;
					((ViewModel)this).OnPropertyChangedWithValue<string>(value, "OrderByCultureText");
				}
			}
		}

		[DataSourceProperty]
		public string OrderByClassText
		{
			get
			{
				return _OrderByClassText;
			}
			set
			{
				if (value != _OrderByClassText)
				{
					_OrderByClassText = value;
					((ViewModel)this).OnPropertyChangedWithValue<string>(value, "OrderByClassText");
				}
			}
		}

		public CYTGameMenuTroopSelectionVM(TroopRoster fullRoster, TroopRoster initialSelections, Func<CharacterObject, bool> canChangeChangeStatusOfTroop, Action<TroopRoster> onDone, int maxSelectableTroopCount, int minSelectableTroopCount)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			_canChangeChangeStatusOfTroop = canChangeChangeStatusOfTroop;
			_onDone = onDone;
			_fullRoster = fullRoster;
			_initialSelections = initialSelections;
			_maxSelectableTroopCount = maxSelectableTroopCount;
			_minSelectableTroopCount = minSelectableTroopCount;
			InitList();
			((ViewModel)this).RefreshValues();
			OnCurrentSelectedAmountChange();
			SpriteData spriteData = UIResourceManager.SpriteData;
			_category = spriteData.SpriteCategories["ui_partyscreen"];
			_category.Load((ITwoDimensionResourceContext)(object)UIResourceManager.ResourceContext, UIResourceManager.ResourceDepot);
		}

		public sealed override void RefreshValues()
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Expected O, but got Unknown
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Expected O, but got Unknown
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Expected O, but got Unknown
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Expected O, but got Unknown
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Expected O, but got Unknown
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Expected O, but got Unknown
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Expected O, but got Unknown
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Expected O, but got Unknown
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Expected O, but got Unknown
			((ViewModel)this).RefreshValues();
			TitleText = ((object)_titleTextObject).ToString();
			CurrentSelectedAmountTitle = ((object)_chosenTitleTextObject).ToString();
			DoneText = ((object)GameTexts.FindText("str_done", (string)null)).ToString();
			CancelText = ((object)GameTexts.FindText("str_cancel", (string)null)).ToString();
			ClearSelectionText = ((object)new TextObject("{=QMNWbmao}Clear Selection", (Dictionary<string, object>)null)).ToString();
			AutomaticSelectionText = ((object)new TextObject("{=cyt_automatic_selection}Automatic Selection", (Dictionary<string, object>)null)).ToString();
			ResetToSavedText = ((object)new TextObject("{=cyt_reset_to_saved}Reset To Saved", (Dictionary<string, object>)null)).ToString();
			DismissSelectionText = ((object)new TextObject("{=cyt_dismiss_selection}Dismiss Selection", (Dictionary<string, object>)null)).ToString();
			OrderByTierText = ((object)new TextObject("{=cyt_order_by_tier}Order by tier", (Dictionary<string, object>)null)).ToString();
			OrderByNameText = ((object)new TextObject("{=cyt_order_by_name}Order by name", (Dictionary<string, object>)null)).ToString();
			OrderByCountText = ((object)new TextObject("{=cyt_order_by_count}Order by count", (Dictionary<string, object>)null)).ToString();
			OrderByClassText = ((object)new TextObject("{=cyt_order_by_class}Order by class", (Dictionary<string, object>)null)).ToString();
			OrderByCultureText = ((object)new TextObject("{=cyt_order_by_culture}Order by culture", (Dictionary<string, object>)null)).ToString();
			InfantryAmount = _infantryAmount;
			ArcherAmount = _archerAmount;
			CavalryAmount = _cavalryAmount;
			HorseArcherAmount = _horseArcherAmount;
		}

		private void InitList()
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			Troops = new MBBindingList<TroopSelectionItemVM>();
			_currentTotalSelectedTroopCount = 0;
			foreach (TroopRosterElement item in (List<TroopRosterElement>)(object)_fullRoster.GetTroopRoster())
			{
				TroopRosterElement current = item;
				TroopSelectionItemVM val = new TroopSelectionItemVM(current, (Action<TroopSelectionItemVM>)OnAddCount, (Action<TroopSelectionItemVM>)OnRemoveCount);
				val.IsLocked = !_canChangeChangeStatusOfTroop(current.Character) || ((TroopRosterElement)(ref current)).Number - ((TroopRosterElement)(ref current)).WoundedNumber <= 0;
				if (ChooseYourTroopsBehavior.DoesTroopCountByTwo(val.Troop.Character))
				{
					_troopsCountedByTwo.Add(val);
					val.Name += " (+2)";
				}
				((Collection<TroopSelectionItemVM>)(object)Troops).Add(val);
				int troopCount = _initialSelections.GetTroopCount(current.Character);
				if (troopCount > 0)
				{
					val.CurrentAmount = troopCount;
					ChangeTotalSelectedAccordingToTroop(troopCount, val);
					for (int i = 0; i < troopCount; i++)
					{
						ChangeTroopValue(((BasicCharacterObject)val.Troop.Character).DefaultFormationClass, isSum: true);
					}
				}
			}
			Troops.Sort((IComparer<TroopSelectionItemVM>)new CYTTroopItemComparer("class", isAscending: false));
			_maxAmounts = _troopsCountedByTwo.ToDictionary((TroopSelectionItemVM troop) => troop, (TroopSelectionItemVM troop) => int.Parse(troop.MaxAmount.ToString()));
		}

		private void ChangeTotalSelectedAccordingToTroop(int amount, TroopSelectionItemVM troopItem)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			bool flag = GlobalSettings<ChooseYourTroopsConfig>.Instance?.SupportForMaximumTroops ?? false;
			if (flag)
			{
				if (ChooseYourTroopsBehavior.DoesTroopCountByTwo(troopItem.Troop.Character))
				{
					_currentTotalSelectedTroopCount += amount * 2;
				}
				else
				{
					_currentTotalSelectedTroopCount += amount;
				}
				if (_currentTotalSelectedTroopCount > _maxSelectableTroopCount)
				{
					IsEntireStackModifierActive = false;
					IsFiveStackModifierActive = false;
					OnRemoveCount(troopItem);
				}
			}
			else
			{
				_currentTotalSelectedTroopCount += amount;
			}
			if (!flag)
			{
				return;
			}
			foreach (TroopSelectionItemVM item in _troopsCountedByTwo)
			{
				if (_maxAmounts.TryGetValue(item, out var value))
				{
					item.MaxAmount = ((_currentTotalSelectedTroopCount + 2 <= _maxSelectableTroopCount) ? value : ((item.CurrentAmount == 0) ? 2 : item.CurrentAmount));
				}
			}
		}

		private void OnRemoveCount(TroopSelectionItemVM troopItem)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			if (troopItem.CurrentAmount > 0)
			{
				int num = 1;
				if (IsEntireStackModifierActive)
				{
					num = troopItem.CurrentAmount;
				}
				else if (IsFiveStackModifierActive)
				{
					num = MathF.Min(troopItem.CurrentAmount, 5);
				}
				troopItem.CurrentAmount -= num;
				ChangeTotalSelectedAccordingToTroop(-num, troopItem);
				for (int i = 0; i < num; i++)
				{
					ChangeTroopValue(((BasicCharacterObject)troopItem.Troop.Character).DefaultFormationClass, isSum: false);
				}
			}
			OnCurrentSelectedAmountChange();
		}

		private void OnAddCount(TroopSelectionItemVM troopItem)
		{
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			if (troopItem.CurrentAmount < troopItem.MaxAmount && _currentTotalSelectedTroopCount < _maxSelectableTroopCount)
			{
				int num = 1;
				if (IsEntireStackModifierActive)
				{
					num = MathF.Min(troopItem.MaxAmount - troopItem.CurrentAmount, _maxSelectableTroopCount - _currentTotalSelectedTroopCount);
				}
				else if (IsFiveStackModifierActive)
				{
					num = MathF.Min(MathF.Min(troopItem.MaxAmount - troopItem.CurrentAmount, _maxSelectableTroopCount - _currentTotalSelectedTroopCount), 5);
				}
				troopItem.CurrentAmount += num;
				ChangeTotalSelectedAccordingToTroop(num, troopItem);
				for (int i = 0; i < num; i++)
				{
					ChangeTroopValue(((BasicCharacterObject)troopItem.Troop.Character).DefaultFormationClass, isSum: true);
				}
			}
			OnCurrentSelectedAmountChange();
		}

		private void OnCurrentSelectedAmountChange()
		{
			foreach (TroopSelectionItemVM item in (Collection<TroopSelectionItemVM>)(object)Troops)
			{
				item.IsRosterFull = _currentTotalSelectedTroopCount >= _maxSelectableTroopCount;
			}
			GameTexts.SetVariable("LEFT", _currentTotalSelectedTroopCount);
			GameTexts.SetVariable("RIGHT", _maxSelectableTroopCount);
			CurrentSelectedAmountText = ((object)GameTexts.FindText("str_LEFT_over_RIGHT_in_paranthesis", (string)null)).ToString();
			IsDoneEnabled = _currentTotalSelectedTroopCount <= _maxSelectableTroopCount && _currentTotalSelectedTroopCount >= _minSelectableTroopCount;
		}

		private void OnDone()
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			TroopRoster val = TroopRoster.CreateDummyTroopRoster();
			foreach (TroopSelectionItemVM item in (Collection<TroopSelectionItemVM>)(object)Troops)
			{
				if (item.CurrentAmount > 0)
				{
					val.AddToCounts(item.Troop.Character, item.CurrentAmount, false, 0, 0, true, -1);
				}
			}
			IsEnabled = false;
			Common.DynamicInvokeWithLog((Delegate)_onDone, new object[1] { val });
		}

		public void ExecuteDone()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Expected O, but got Unknown
			if (_currentTotalSelectedTroopCount < _maxSelectableTroopCount)
			{
				string text = ((object)new TextObject("{=z2Slmx4N}There are still some room for more soldiers. Do you want to proceed?", (Dictionary<string, object>)null)).ToString();
				InformationManager.ShowInquiry(new InquiryData(TitleText, text, true, true, ((object)GameTexts.FindText("str_yes", (string)null)).ToString(), ((object)GameTexts.FindText("str_no", (string)null)).ToString(), (Action)OnDone, (Action)null, "", 0f, (Action)null, (Func<ValueTuple<bool, string>>)null, (Func<ValueTuple<bool, string>>)null), false, false);
			}
			else
			{
				OnDone();
			}
		}

		public void ExecuteCancel()
		{
			IsEnabled = false;
		}

		public void ExecuteReset()
		{
			InitList();
			OnCurrentSelectedAmountChange();
		}

		public void ExecuteResetToSaved()
		{
			ExecuteReset();
		}

		public void ExecuteDismissSelection()
		{
			ChooseYourTroopsBehavior.DismissSelection();
			ExecuteCancel();
		}

		public void ExecuteAutomaticSelection()
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			ExecuteClearSelection();
			FormationClass[] array = new FormationClass[4];
			RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			FormationClass[] source = (FormationClass[])(object)array;
			List<FormationClass> list = source.Where(HasSelectableTroopsForFormationClass).ToList();
			if (list.Count == 0)
			{
				return;
			}
			while (true)
			{
				List<TroopSelectionItemVM> list2 = new List<TroopSelectionItemVM>(list.Count);
				int num = 0;
				foreach (FormationClass item in list)
				{
					TroopSelectionItemVM selectableTroopForFormationClass = GetSelectableTroopForFormationClass(item);
					if (selectableTroopForFormationClass == null)
					{
						list2.Clear();
						break;
					}
					int troopCost = GetTroopCost(selectableTroopForFormationClass);
					if (_currentTotalSelectedTroopCount + num + troopCost > _maxSelectableTroopCount)
					{
						list2.Clear();
						break;
					}
					list2.Add(selectableTroopForFormationClass);
					num += troopCost;
				}
				if (list2.Count == 0)
				{
					break;
				}
				foreach (TroopSelectionItemVM item2 in list2)
				{
					OnAddCount(item2);
				}
			}
			foreach (FormationClass item3 in list)
			{
				while (_currentTotalSelectedTroopCount < _maxSelectableTroopCount)
				{
					TroopSelectionItemVM selectableTroopForFormationClass2 = GetSelectableTroopForFormationClass(item3);
					if (selectableTroopForFormationClass2 == null)
					{
						break;
					}
					int troopCost2 = GetTroopCost(selectableTroopForFormationClass2);
					if (_currentTotalSelectedTroopCount + troopCost2 > _maxSelectableTroopCount)
					{
						break;
					}
					OnAddCount(selectableTroopForFormationClass2);
				}
			}
		}

		public void ExecuteClearSelection()
		{
			Troops.ApplyActionOnAllItems((Action<TroopSelectionItemVM>)delegate(TroopSelectionItemVM troopItem)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				if (_canChangeChangeStatusOfTroop(troopItem.Troop.Character))
				{
					int currentAmount = troopItem.CurrentAmount;
					for (int i = 0; i < currentAmount; i++)
					{
						troopItem.ExecuteRemove();
					}
				}
			});
		}

		private bool HasSelectableTroopsForFormationClass(FormationClass formationClass)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return ((IEnumerable<TroopSelectionItemVM>)Troops).Any((TroopSelectionItemVM troopSelectionItemVM) => ((BasicCharacterObject)troopSelectionItemVM.Troop.Character).DefaultFormationClass == formationClass && !troopSelectionItemVM.IsLocked && troopSelectionItemVM.CurrentAmount < troopSelectionItemVM.MaxAmount);
		}

		private TroopSelectionItemVM GetSelectableTroopForFormationClass(FormationClass formationClass)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return (from troopSelectionItemVM in (IEnumerable<TroopSelectionItemVM>)Troops
				where ((BasicCharacterObject)troopSelectionItemVM.Troop.Character).DefaultFormationClass == formationClass && !troopSelectionItemVM.IsLocked && troopSelectionItemVM.CurrentAmount < troopSelectionItemVM.MaxAmount
				orderby troopSelectionItemVM.CurrentAmount, troopSelectionItemVM.MaxAmount descending
				select troopSelectionItemVM).FirstOrDefault();
		}

		private int GetTroopCost(TroopSelectionItemVM troopSelectionItemVM)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			ChooseYourTroopsConfig instance = GlobalSettings<ChooseYourTroopsConfig>.Instance;
			return (instance == null || !instance.SupportForMaximumTroops || !ChooseYourTroopsBehavior.DoesTroopCountByTwo(troopSelectionItemVM.Troop.Character)) ? 1 : 2;
		}

		public void OrderByTier()
		{
			Troops.Sort((IComparer<TroopSelectionItemVM>)new CYTTroopItemComparer("tier", IsAscending));
			((ViewModel)this).RefreshValues();
		}

		public void OrderByClass()
		{
			Troops.Sort((IComparer<TroopSelectionItemVM>)new CYTTroopItemComparer("class", IsAscending));
			((ViewModel)this).RefreshValues();
		}

		public void OrderByName()
		{
			Troops.Sort((IComparer<TroopSelectionItemVM>)new CYTTroopItemComparer("name", IsAscending));
			((ViewModel)this).RefreshValues();
		}

		public void OrderByCount()
		{
			Troops.Sort((IComparer<TroopSelectionItemVM>)new CYTTroopItemComparer("count", IsAscending));
			((ViewModel)this).RefreshValues();
		}

		public void OrderByCulture()
		{
			Troops.Sort((IComparer<TroopSelectionItemVM>)new CYTTroopItemComparer("culture", IsAscending));
			((ViewModel)this).RefreshValues();
		}

		public override void OnFinalize()
		{
			((ViewModel)this).OnFinalize();
			InputKeyItemVM cancelInputKey = CancelInputKey;
			if (cancelInputKey != null)
			{
				((ViewModel)cancelInputKey).OnFinalize();
			}
			InputKeyItemVM doneInputKey = DoneInputKey;
			if (doneInputKey != null)
			{
				((ViewModel)doneInputKey).OnFinalize();
			}
			InputKeyItemVM resetInputKey = ResetInputKey;
			if (resetInputKey != null)
			{
				((ViewModel)resetInputKey).OnFinalize();
				_category.Unload();
			}
		}

		public void ChangeTroopValue(FormationClass formationClass, bool isSum)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected I4, but got Unknown
			switch ((int)formationClass)
			{
			case 0:
				if (isSum)
				{
					InfantryAmount++;
				}
				else
				{
					InfantryAmount--;
				}
				break;
			case 1:
				if (isSum)
				{
					ArcherAmount++;
				}
				else
				{
					ArcherAmount--;
				}
				break;
			case 2:
				if (isSum)
				{
					CavalryAmount++;
				}
				else
				{
					CavalryAmount--;
				}
				break;
			case 3:
				if (isSum)
				{
					HorseArcherAmount++;
				}
				else
				{
					HorseArcherAmount--;
				}
				break;
			}
		}

		private void ExecuteToggleOrder()
		{
			IsAscending = !IsAscending;
		}

		public void SetCancelInputKey(HotKey hotkey)
		{
			CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		public void SetDoneInputKey(HotKey hotkey)
		{
			DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		public void SetResetInputKey(HotKey hotkey)
		{
			ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}
	}
	public class CYTGauntletMenuTroopSelectionView : MenuView
	{
		private readonly Action<TroopRoster> _onDone;

		private readonly TroopRoster _fullRoster;

		private readonly TroopRoster _initialSelections;

		private readonly Func<CharacterObject, bool> _changeChangeStatusOfTroop;

		private readonly int _maxSelectableTroopCount;

		private readonly int _minSelectableTroopCount;

		private GauntletLayer _layerAsGauntletLayer;

		private CYTGameMenuTroopSelectionVM _dataSource;

		private GauntletMovieIdentifier _movie;

		public CYTGauntletMenuTroopSelectionView(TroopRoster fullRoster, TroopRoster initialSelections, Func<CharacterObject, bool> changeChangeStatusOfTroop, Action<TroopRoster> onDone, int maxSelectableTroopCount, int minSelectableTroopCount)
		{
			_onDone = onDone;
			_fullRoster = fullRoster;
			_initialSelections = initialSelections;
			_changeChangeStatusOfTroop = changeChangeStatusOfTroop;
			_maxSelectableTroopCount = maxSelectableTroopCount;
			_minSelectableTroopCount = minSelectableTroopCount;
		}

		public CYTGauntletMenuTroopSelectionView()
		{
			throw new NotImplementedException();
		}

		protected override void OnInitialize()
		{
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Expected O, but got Unknown
			((SandboxView)this).OnInitialize();
			_dataSource = new CYTGameMenuTroopSelectionVM(_fullRoster, _initialSelections, _changeChangeStatusOfTroop, OnDone, _maxSelectableTroopCount, _minSelectableTroopCount)
			{
				IsEnabled = true
			};
			_dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			_dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			_dataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
			((SandboxView)this).Layer = (ScreenLayer)new GauntletLayer("MapTroopSelection", 206, false);
			ref GauntletLayer layerAsGauntletLayer = ref _layerAsGauntletLayer;
			ScreenLayer layer = ((SandboxView)this).Layer;
			layerAsGauntletLayer = (GauntletLayer)(object)((layer is GauntletLayer) ? layer : null);
			((SandboxView)this).Layer.InputRestrictions.SetInputRestrictions(true, (InputUsageMask)7);
			((SandboxView)this).Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			((SandboxView)this).Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			_movie = _layerAsGauntletLayer.LoadMovie("CYTGameMenuTroopSelection", (ViewModel)(object)_dataSource);
			((SandboxView)this).Layer.IsFocusLayer = true;
			ScreenManager.TrySetFocus((ScreenLayer)(object)_layerAsGauntletLayer);
			((MenuView)this).MenuViewContext.AddLayer(((SandboxView)this).Layer);
			ScreenBase topScreen = ScreenManager.TopScreen;
			MapScreen val = (MapScreen)(object)((topScreen is MapScreen) ? topScreen : null);
			if (val != null)
			{
				val.SetIsInHideoutTroopManage(true);
			}
		}

		private void OnDone(TroopRoster obj)
		{
			MapScreen.Instance.SetIsInHideoutTroopManage(false);
			((MenuView)this).MenuViewContext.CloseTroopSelection();
			Action<TroopRoster> onDone = _onDone;
			if (onDone != null)
			{
				Common.DynamicInvokeWithLog((Delegate)onDone, new object[1] { obj });
			}
		}

		protected override void OnFinalize()
		{
			((SandboxView)this).Layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(((SandboxView)this).Layer);
			((ViewModel)_dataSource).OnFinalize();
			_dataSource = null;
			_layerAsGauntletLayer.ReleaseMovie(_movie);
			((MenuView)this).MenuViewContext.RemoveLayer(((SandboxView)this).Layer);
			_movie = null;
			((SandboxView)this).Layer = null;
			_layerAsGauntletLayer = null;
			MapScreen.Instance.SetIsInHideoutTroopManage(false);
			((SandboxView)this).OnFinalize();
		}

		protected override void OnFrameTick(float dt)
		{
			((SandboxView)this).OnFrameTick(dt);
			if (_dataSource != null)
			{
				_dataSource.IsFiveStackModifierActive = ((SandboxView)this).Layer.Input.IsHotKeyDown("FiveStackModifier");
				_dataSource.IsEntireStackModifierActive = ((SandboxView)this).Layer.Input.IsHotKeyDown("EntireStackModifier");
			}
			ScreenLayer layer = ((SandboxView)this).Layer;
			if (layer != null && layer.Input.IsHotKeyPressed("Exit"))
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				_dataSource.ExecuteCancel();
			}
			else
			{
				ScreenLayer layer2 = ((SandboxView)this).Layer;
				if (layer2 != null && layer2.Input.IsHotKeyPressed("Confirm"))
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					_dataSource.ExecuteDone();
				}
				else
				{
					ScreenLayer layer3 = ((SandboxView)this).Layer;
					if (layer3 != null && layer3.Input.IsHotKeyPressed("Reset"))
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						_dataSource.ExecuteReset();
					}
				}
			}
			CYTGameMenuTroopSelectionVM dataSource = _dataSource;
			if (dataSource != null && !dataSource.IsEnabled)
			{
				((MenuView)this).MenuViewContext.CloseTroopSelection();
			}
		}
	}
	public class CYTTroopItemComparer : IComparer<TroopSelectionItemVM>
	{
		private static string _orderingType;

		private bool _IsAscending;

		public CYTTroopItemComparer(string orderingType = "default", bool isAscending = true)
		{
			_orderingType = orderingType;
			_IsAscending = isAscending;
		}

		public int Compare(TroopSelectionItemVM x, TroopSelectionItemVM y)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Expected I4, but got Unknown
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			if (((BasicCharacterObject)y.Troop.Character).IsPlayerCharacter)
			{
				return 1;
			}
			if (((BasicCharacterObject)y.Troop.Character).IsHero)
			{
				if (((BasicCharacterObject)x.Troop.Character).IsPlayerCharacter)
				{
					return -1;
				}
				if (((BasicCharacterObject)x.Troop.Character).IsHero)
				{
					return ((BasicCharacterObject)y.Troop.Character).Level - ((BasicCharacterObject)x.Troop.Character).Level;
				}
				return 1;
			}
			if (((BasicCharacterObject)x.Troop.Character).IsPlayerCharacter || ((BasicCharacterObject)x.Troop.Character).IsHero)
			{
				return -1;
			}
			int num = 0;
			switch (_orderingType)
			{
			case "tier":
				num = x.Troop.Character.Tier - y.Troop.Character.Tier;
				break;
			case "name":
				num = ((object)((BasicCharacterObject)y.Troop.Character).Name).ToString().CompareTo(((object)((BasicCharacterObject)x.Troop.Character).Name).ToString());
				break;
			case "class":
				num = ((BasicCharacterObject)y.Troop.Character).DefaultFormationClass - ((BasicCharacterObject)x.Troop.Character).DefaultFormationClass;
				break;
			case "count":
			{
				TroopRosterElement troop = y.Troop;
				int number = ((TroopRosterElement)(ref troop)).Number;
				troop = x.Troop;
				num = ((number <= ((TroopRosterElement)(ref troop)).Number) ? 1 : (-1));
				break;
			}
			case "culture":
				num = ((MBObjectBase)y.Troop.Character.Culture).StringId.CompareTo(((MBObjectBase)x.Troop.Character.Culture).StringId);
				break;
			case "default":
				num = ((BasicCharacterObject)y.Troop.Character).Level - ((BasicCharacterObject)x.Troop.Character).Level;
				break;
			}
			if (!_IsAscending)
			{
				num = -num;
			}
			return num;
		}
	}
	public class ChooseYourTroopsBehavior : CampaignBehaviorBase
	{
		public static class EncounterGameMenuBehavior
		{
			[HarmonyPatch(typeof(MapEventSide), "AllocateTroops")]
			public static class MapEventSidePatches
			{
				private static HashSet<string>? _selectedTroops;

				public static void Reset()
				{
					_selectedTroops = null;
				}

				[HarmonyPrefix]
				private static void Prefix(ref List<UniqueTroopDescriptor> troopsList, int numberToAllocate, ref Func<UniqueTroopDescriptor, MapEventParty, bool> customAllocationConditions, MapEventSide __instance)
				{
					//IL_013e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0148: Expected O, but got Unknown
					//IL_0039: Unknown result type (might be due to invalid IL or missing references)
					//IL_003e: Unknown result type (might be due to invalid IL or missing references)
					//IL_009d: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
					//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
					try
					{
						if (!_setupSide || _actualTroopRoster == null || __instance.MapEvent.IsPlayerSimulation || __instance.MissionSide != _playerSide)
						{
							return;
						}
						if (numberToAllocate != _initialPlayerSpawn)
						{
							customAllocationConditions = null;
							return;
						}
						if (_selectedTroops == null)
						{
							_selectedTroops = new HashSet<string>();
							foreach (TroopRosterElement item in (List<TroopRosterElement>)(object)_actualTroopRoster.GetTroopRoster())
							{
								TroopRosterElement current = item;
								if (current.Character != null)
								{
									int num = ((TroopRosterElement)(ref current)).Number - ((TroopRosterElement)(ref current)).WoundedNumber;
									if (num > 0)
									{
										_selectedTroops.Add(((MBObjectBase)current.Character).StringId);
									}
								}
							}
						}
						customAllocationConditions = delegate(UniqueTroopDescriptor descriptor, MapEventParty party)
						{
							//IL_0008: Unknown result type (might be due to invalid IL or missing references)
							//IL_0009: Unknown result type (might be due to invalid IL or missing references)
							//IL_000e: Unknown result type (might be due to invalid IL or missing references)
							try
							{
								FlattenedTroopRosterElement val = party.Troops[descriptor];
								CharacterObject troop = ((FlattenedTroopRosterElement)(ref val)).Troop;
								if (troop == null)
								{
									return false;
								}
								return _selectedTroops.Contains(((MBObjectBase)troop).StringId);
							}
							catch
							{
								return false;
							}
						};
					}
					catch (Exception arg)
					{
						InformationManager.DisplayMessage(new InformationMessage($"CYT AllocateTroops Error:\n{arg}"));
					}
				}
			}
		}

		[HarmonyPatch(typeof(PlayerEncounter), "FinishEncounterInternal")]
		private static class PlayerEncounterPrefix
		{
			[HarmonyPrefix]
			private static void Prefix()
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				EncounterGameMenuBehavior.MapEventSidePatches.Reset();
				_actualTroopRoster = null;
				_playerSide = (BattleSideEnum)(-1);
				_setupSide = false;
				_haveChosenTroops = false;
				_playerArmySize = 0;
				_enemyArmySize = 0;
				_initialPlayerSpawn = 0;
			}
		}

		[HarmonyPatch(typeof(DefaultBattleMissionAgentSpawnLogic), "AfterStart")]
		private static class AfterStartPostfix
		{
			[HarmonyPostfix]
			private static void Postfix(DefaultBattleMissionAgentSpawnLogic __instance)
			{
				if (_haveChosenTroops)
				{
					_setupSide = true;
				}
			}
		}

		[HarmonyPatch(typeof(LordsHallFightMissionController), "OnCreated")]
		private static class InitializeMissionPrefix
		{
			[HarmonyPrefix]
			private static void Prefix()
			{
				if (_setupSide)
				{
					_setupSide = false;
					_haveChosenTroops = false;
				}
			}
		}

		[HarmonyPatch(typeof(Mission), "OnBattleSideDeployed")]
		private static class OnBattleSideDeployedPostfix
		{
			[HarmonyPostfix]
			private static void Postfix(BattleSideEnum side)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				if (side == _playerSide)
				{
					_playerArmySize = 0;
				}
				else
				{
					_enemyArmySize = 0;
				}
			}
		}

		[HarmonyPatch(typeof(DefaultBattleMissionAgentSpawnLogic), "Init")]
		private static class DefaultBattleMissionAgentSpawnLogicInitPostfix
		{
			[HarmonyPostfix]
			private static void Postfix(bool spawnDefenders, bool spawnAttackers, in MissionSpawnSettings reinforcementSpawnSettings, DefaultBattleMissionAgentSpawnLogic __instance)
			{
				//IL_0294: Unknown result type (might be due to invalid IL or missing references)
				//IL_029e: Expected O, but got Unknown
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Invalid comparison between Unknown and I4
				//IL_0094: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Invalid comparison between Unknown and I4
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Expected O, but got Unknown
				//IL_0120: Unknown result type (might be due to invalid IL or missing references)
				//IL_0127: Expected O, but got Unknown
				try
				{
					if (!_haveChosenTroops)
					{
						return;
					}
					FieldInfo fieldInfo = AccessTools.Field(typeof(DefaultBattleMissionAgentSpawnLogic), "_battleSideSpawnContexts");
					Array array = fieldInfo.GetValue(__instance) as Array;
					FieldInfo fieldInfo2 = AccessTools.Field(typeof(DefaultBattleMissionAgentSpawnLogic), "_phases");
					object[] array2 = (object[])fieldInfo2.GetValue(__instance);
					for (int i = 0; i < array2.Length; i++)
					{
						List<object> list = ((IEnumerable)array2[i]).Cast<object>().ToList();
						FieldInfo fieldInfo3 = AccessTools.Field(list[0].GetType(), "InitialSpawnNumber");
						bool flag = ((i == 0) ? ((int)_playerSide == 0) : ((int)_playerSide == 1));
						int num;
						if (flag)
						{
							num = _initialPlayerSpawn;
						}
						else
						{
							ChooseYourTroopsConfig instance = GlobalSettings<ChooseYourTroopsConfig>.Instance;
							if (instance != null && instance.SupportForMaximumTroops)
							{
								object value = array.GetValue(i);
								PartyGroupTroopSupplier obj = (PartyGroupTroopSupplier)AccessTools.Field(MissionBattleSideSpawnContextType, "_troopSupplier").GetValue(value);
								MapEventSide obj2 = (MapEventSide)AccessTools.Property(typeof(PartyGroupTroopSupplier), "PartyGroup").GetValue(obj);
								List<(FlattenedTroopRosterElement, MapEventParty, float)> source = (List<(FlattenedTroopRosterElement, MapEventParty, float)>)AccessTools.Field(typeof(MapEventSide), "_readyTroopsPriorityList").GetValue(obj2);
								source = source.OrderByDescending(((FlattenedTroopRosterElement, MapEventParty, float) x) => x.Item3).ToList();
								num = GetEnemyInitialSpawn(source.Select(((FlattenedTroopRosterElement, MapEventParty, float) item) => item.Item1));
							}
							else
							{
								num = EnemyInitialSpawn;
							}
						}
						fieldInfo3.SetValue(list[0], num);
						AccessTools.Field(list[0].GetType(), "RemainingSpawnNumber").SetValue(list[0], flag ? (_playerArmySize - _initialPlayerSpawn) : (_enemyArmySize - EnemyInitialSpawn));
					}
					if (array != null)
					{
						object value2 = array.GetValue(0);
						object value3 = array.GetValue(1);
						MethodInfo methodInfo = AccessTools.Method(value2.GetType(), "SetSpawnTroops", (Type[])null, (Type[])null);
						methodInfo.Invoke(value2, new object[1] { spawnDefenders });
						methodInfo.Invoke(value3, new object[1] { spawnAttackers });
					}
				}
				catch (Exception)
				{
					InformationManager.DisplayMessage(new InformationMessage("ChooseYourTroops Error at DefaultBattleMissionAgentSpawnLogic Init Postfix"));
					_setupSide = false;
				}
			}
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class <>c
		{
			public static readonly <>c <>9 = new <>c();

			public static Action<IMission> <>9__14_0;

			public static Action<PartyBase> <>9__14_1;

			public static Func<MapEventParty, PartyBase> <>9__17_0;

			public static OnConditionDelegate <>9__21_0;

			public static OnConsequenceDelegate <>9__21_1;

			public static OnConditionDelegate <>9__21_2;

			public static OnConsequenceDelegate <>9__21_3;

			public static Func<TroopRosterElement, bool> <>9__23_1;

			public static Func<MapEventParty, bool> <>9__25_2;

			public static Func<PartyBase, int> <>9__25_0;

			public static Func<PartyBase, int> <>9__25_1;

			internal void <RegisterEvents>b__14_0(IMission mission)
			{
				CheckTroopRoster();
				EncounterGameMenuBehavior.MapEventSidePatches.Reset();
			}

			internal void <RegisterEvents>b__14_1(PartyBase party)
			{
				if (party == MobileParty.MainParty.Party && Mission.Current == null)
				{
					CheckTroopRoster();
				}
			}

			internal PartyBase <GetCurrentPlayerTroopRoster>b__17_0(MapEventParty x)
			{
				return x.Party;
			}

			internal bool <AddGamesMenu>b__21_0(MenuCallbackArgs args)
			{
				return !OnNavalMapEvent && SelectTroopsCondition(args);
			}

			internal void <AddGamesMenu>b__21_1(MenuCallbackArgs args)
			{
				OpenTroopsSelector(args, _playerArmySize);
			}

			internal bool <AddGamesMenu>b__21_2(MenuCallbackArgs args)
			{
				return !OnNavalMapEvent && SelectTroopsCondition(args);
			}

			internal void <AddGamesMenu>b__21_3(MenuCallbackArgs args)
			{
				OpenTroopsSelector(args, _playerArmySize);
			}

			internal bool <OpenTroopsSelector>b__23_1(TroopRosterElement x)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				return ((BasicCharacterObject)x.Character).IsHero && ((TroopRosterElement)(ref x)).Number > ((TroopRosterElement)(ref x)).WoundedNumber;
			}

			internal bool <IsPlayerArmyBig>b__25_2(MapEventParty x)
			{
				return !x.IsNpcParty;
			}

			internal int <IsPlayerArmyBig>b__25_0(PartyBase x)
			{
				return x.NumberOfHealthyMembers;
			}

			internal int <IsPlayerArmyBig>b__25_1(PartyBase x)
			{
				return x.NumberOfHealthyMembers;
			}
		}

		private static BattleSideEnum _playerSide = (BattleSideEnum)(-1);

		private static TroopRoster? _actualTroopRoster = null;

		private static List<(FlattenedTroopRosterElement, MapEventParty, float)> _actualReadyTroopsPriorityList = null;

		private static bool _setupSide = false;

		private static bool _haveChosenTroops = false;

		private static int _playerArmySize;

		private static int _enemyArmySize;

		private static int _initialPlayerSpawn;

		private static readonly Type MissionBattleSideSpawnContextType = AccessTools.TypeByName("TaleWorlds.MountAndBlade.MissionBattleSideSpawnContext");

		public static BattleSideEnum PlayerSide => _playerSide;

		public static TroopRoster? ActualTroopRoster => _actualTroopRoster;

		public static bool OnNavalMapEvent
		{
			get
			{
				MapEvent playerMapEvent = MapEvent.PlayerMapEvent;
				return playerMapEvent != null && playerMapEvent.IsNavalMapEvent;
			}
		}

		private static int EnemyInitialSpawn => (_enemyArmySize <= ChooseYourTroopsConfig.CurrentBattleSizeLimit - _initialPlayerSpawn) ? _enemyArmySize : (ChooseYourTroopsConfig.CurrentBattleSizeLimit - _initialPlayerSpawn);

		public override void RegisterEvents()
		{
			CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener((object)this, (Action<IMission>)delegate
			{
				CheckTroopRoster();
				EncounterGameMenuBehavior.MapEventSidePatches.Reset();
			});
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener((object)this, (Action)CheckTroopRoster);
			CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)AddGamesMenu);
			CampaignEvents.OnPartySizeChangedEvent.AddNonSerializedListener((object)this, (Action<PartyBase>)delegate(PartyBase party)
			{
				if (party == MobileParty.MainParty.Party && Mission.Current == null)
				{
					CheckTroopRoster();
				}
			});
		}

		internal static bool DoesTroopCountByTwo(CharacterObject characterObject)
		{
			ChooseYourTroopsConfig instance = GlobalSettings<ChooseYourTroopsConfig>.Instance;
			return instance != null && instance.SupportForMaximumTroops && ((BasicCharacterObject)characterObject).HasMount();
		}

		internal static void DismissSelection()
		{
			EncounterGameMenuBehavior.MapEventSidePatches.Reset();
			_actualTroopRoster = null;
			_haveChosenTroops = false;
			Campaign.Current.CurrentMenuContext.Refresh();
		}

		private static TroopRoster GetCurrentPlayerTroopRoster()
		{
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			TroopRoster val = MobileParty.MainParty.MemberRoster;
			if (MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty)
			{
				val = TroopRoster.CreateDummyTroopRoster();
				foreach (MobileParty item in (List<MobileParty>)(object)MobileParty.MainParty.Army.Parties)
				{
					if (MobileParty.MainParty.Army.DoesLeaderPartyAndAttachedPartiesContain(item))
					{
						val.Add(item.MemberRoster);
					}
				}
			}
			else if ((PlayerEncounter.Battle != null && ((List<MapEventParty>)(object)PlayerEncounter.Battle.PartiesOnSide(_playerSide)).Count > 1) || (PlayerSiege.BesiegedSettlement != null && PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide(PlayerSiege.PlayerSide).GetInvolvedPartiesForEventType((BattleTypes)5).Count() > 1 && MobileParty.MainParty.Army == null))
			{
				val = TroopRoster.CreateDummyTroopRoster();
				List<PartyBase> list = ((PlayerEncounter.Battle != null) ? ((IEnumerable<MapEventParty>)PlayerEncounter.Battle.PartiesOnSide(_playerSide)).Select((MapEventParty x) => x.Party).ToList() : PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide(PlayerSiege.PlayerSide).GetInvolvedPartiesForEventType((BattleTypes)5).ToList());
				foreach (PartyBase item2 in list)
				{
					val.Add(item2.MemberRoster);
				}
			}
			return val;
		}

		private static void CheckTroopRoster()
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			if (_actualTroopRoster == null)
			{
				return;
			}
			MBList<TroopRosterElement> troopRoster = GetCurrentPlayerTroopRoster().GetTroopRoster();
			MBList<TroopRosterElement> troopRoster2 = _actualTroopRoster.GetTroopRoster();
			_actualTroopRoster.Clear();
			foreach (TroopRosterElement item in (List<TroopRosterElement>)(object)troopRoster2)
			{
				TroopRosterElement actualTroop = item;
				TroopRosterElement val = ((IEnumerable<TroopRosterElement>)troopRoster).FirstOrDefault(delegate(TroopRosterElement x)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					CharacterObject character2 = x.Character;
					string obj2 = ((character2 != null) ? ((MBObjectBase)character2).StringId : null);
					CharacterObject character3 = actualTroop.Character;
					return obj2 == ((character3 != null) ? ((MBObjectBase)character3).StringId : null);
				});
				if (val.Character == null)
				{
					CharacterObject character = actualTroop.Character;
					object obj;
					if (character == null)
					{
						obj = null;
					}
					else
					{
						Hero heroObject = character.HeroObject;
						if (heroObject == null)
						{
							obj = null;
						}
						else
						{
							MobileParty partyBelongedTo = heroObject.PartyBelongedTo;
							obj = ((partyBelongedTo != null) ? partyBelongedTo.Army : null);
						}
					}
					MobileParty mainParty = MobileParty.MainParty;
					if (obj == ((mainParty != null) ? mainParty.Army : null))
					{
						val = actualTroop;
					}
				}
				if (val.Character != null && ((TroopRosterElement)(ref val)).Number > 0 && ((TroopRosterElement)(ref val)).Number != ((TroopRosterElement)(ref val)).WoundedNumber)
				{
					int val2 = ((TroopRosterElement)(ref val)).Number - ((TroopRosterElement)(ref val)).WoundedNumber;
					int number = Math.Min(((TroopRosterElement)(ref actualTroop)).Number, val2);
					TroopRosterElement val3 = actualTroop;
					((TroopRosterElement)(ref val3)).Number = number;
					_actualTroopRoster.Add(val3);
				}
			}
		}

		private static void OnDone(TroopRoster troopRoster, MenuCallbackArgs args)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected O, but got Unknown
			if (troopRoster.TotalManCount < _initialPlayerSpawn)
			{
				_initialPlayerSpawn = troopRoster.TotalManCount;
			}
			if (!_haveChosenTroops || (_haveChosenTroops && !((object)troopRoster).Equals((object)_actualTroopRoster)))
			{
				InformationManager.DisplayMessage(new InformationMessage(((object)new TextObject("{=troops_selected_text}Troops selected!", (Dictionary<string, object>)null)).ToString(), new Color(15f, 70f, 160f, 1f)));
			}
			_actualTroopRoster = troopRoster;
			_haveChosenTroops = true;
			TroopRoster? actualTroopRoster = _actualTroopRoster;
			GameTexts.SetVariable("SELECTED_AMOUNT", (actualTroopRoster != null) ? actualTroopRoster.TotalManCount : 0);
			args.MenuContext.Refresh();
		}

		public static bool SelectTroopsCondition(MenuCallbackArgs args)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Expected O, but got Unknown
			_ = _setupSide;
			GameTexts.SetVariable("SELECTED_AMOUNT", (_actualTroopRoster != null) ? _actualTroopRoster.TotalManCount : 0);
			args.optionLeaveType = (LeaveType)19;
			args.IsEnabled = true;
			bool result = IsPlayerArmyBig();
			if (_playerArmySize <= 1)
			{
				args.IsEnabled = false;
				args.Tooltip = new TextObject("{=select_your_troops_tooltip}There is only one person or less on your party.", (Dictionary<string, object>)null);
			}
			return result;
		}

		private static void AddGamesMenu(CampaignGameStarter gameSystemInitializer)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Expected O, but got Unknown
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Expected O, but got Unknown
			object obj = <>c.<>9__21_0;
			if (obj == null)
			{
				OnConditionDelegate val = (MenuCallbackArgs args) => !OnNavalMapEvent && SelectTroopsCondition(args);
				<>c.<>9__21_0 = val;
				obj = (object)val;
			}
			object obj2 = <>c.<>9__21_1;
			if (obj2 == null)
			{
				OnConsequenceDelegate val2 = delegate(MenuCallbackArgs args)
				{
					OpenTroopsSelector(args, _playerArmySize);
				};
				<>c.<>9__21_1 = val2;
				obj2 = (object)val2;
			}
			gameSystemInitializer.AddGameMenuOption("encounter", "select_troops", "{=select_your_troops_text}Select troops ({SELECTED_AMOUNT} selected).", (OnConditionDelegate)obj, (OnConsequenceDelegate)obj2, false, -1, false, (object)null);
			object obj3 = <>c.<>9__21_2;
			if (obj3 == null)
			{
				OnConditionDelegate val3 = (MenuCallbackArgs args) => !OnNavalMapEvent && SelectTroopsCondition(args);
				<>c.<>9__21_2 = val3;
				obj3 = (object)val3;
			}
			object obj4 = <>c.<>9__21_3;
			if (obj4 == null)
			{
				OnConsequenceDelegate val4 = delegate(MenuCallbackArgs args)
				{
					OpenTroopsSelector(args, _playerArmySize);
				};
				<>c.<>9__21_3 = val4;
				obj4 = (object)val4;
			}
			gameSystemInitializer.AddGameMenuOption("menu_siege_strategies", "select_troops_menu_siege_strategies", "{=select_your_troops_text}Select troops ({SELECTED_AMOUNT} selected).", (OnConditionDelegate)obj3, (OnConsequenceDelegate)obj4, false, -1, false, (object)null);
		}

		private static void OpenTroopSelection(MenuCallbackArgs args, TroopRoster fullRoster, TroopRoster initialSelections, Func<CharacterObject, bool> canChangeChangeStatusOfTroop, Action<TroopRoster> onDone, int maxSelectableTroopCount, int minSelectableTroopCount)
		{
			if (args.MenuContext.Handler != null)
			{
				IMenuContextHandler handler = args.MenuContext.Handler;
				MenuViewContext val = (MenuViewContext)(object)((handler is MenuViewContext) ? handler : null);
				if (val != null)
				{
					AccessTools.Field(typeof(MenuViewContext), "_menuTroopSelection").SetValue(val, val.AddMenuView<CYTGauntletMenuTroopSelectionView>(new object[6] { fullRoster, initialSelections, canChangeChangeStatusOfTroop, onDone, maxSelectableTroopCount, minSelectableTroopCount }));
				}
			}
		}

		public static void OpenTroopsSelector(MenuCallbackArgs args, int maxInitialPlayerTroops)
		{
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			MenuCallbackArgs args2 = args;
			CheckTroopRoster();
			if (_playerArmySize + _enemyArmySize > ChooseYourTroopsConfig.CurrentBattleSizeLimit && (_playerArmySize > ChooseYourTroopsConfig.CurrentBattleSizeLimit / 2 || _enemyArmySize > ChooseYourTroopsConfig.CurrentBattleSizeLimit / 2))
			{
				if (_enemyArmySize < _playerArmySize)
				{
					maxInitialPlayerTroops = ChooseYourTroopsConfig.CurrentBattleSizeLimit / 2 + (ChooseYourTroopsConfig.CurrentBattleSizeLimit / 2 - _enemyArmySize);
				}
				if (_playerArmySize > ChooseYourTroopsConfig.CurrentBattleSizeLimit / 2 && _enemyArmySize > ChooseYourTroopsConfig.CurrentBattleSizeLimit / 2)
				{
					maxInitialPlayerTroops = ChooseYourTroopsConfig.CurrentBattleSizeLimit / 2;
				}
			}
			TroopRoster currentPlayerTroopRoster = GetCurrentPlayerTroopRoster();
			_initialPlayerSpawn = maxInitialPlayerTroops;
			TroopRoster val = TroopRoster.CreateDummyTroopRoster();
			if (_actualTroopRoster != null)
			{
				TroopRoster val2 = _actualTroopRoster.CloneRosterData();
				foreach (TroopRosterElement item in (List<TroopRosterElement>)(object)val2.GetTroopRoster())
				{
					TroopRosterElement current = item;
					if (((TroopRosterElement)(ref current)).WoundedNumber > 0)
					{
						val2.AddToCounts(current.Character, -((TroopRosterElement)(ref current)).WoundedNumber, false, 0, 0, true, -1);
					}
				}
				if (val2.TotalManCount > maxInitialPlayerTroops)
				{
					for (int i = maxInitialPlayerTroops - 1 - val.TotalManCount; i < val2.Count - maxInitialPlayerTroops; i++)
					{
						val2.RemoveTroop(val2.GetCharacterAtIndex(i), 1, default(UniqueTroopDescriptor), 0);
					}
				}
				foreach (TroopRosterElement item2 in (List<TroopRosterElement>)(object)val2.GetTroopRoster())
				{
					TroopRosterElement current2 = item2;
					val.AddToCounts(current2.Character, ((TroopRosterElement)(ref current2)).Number - ((TroopRosterElement)(ref current2)).WoundedNumber, false, 0, 0, true, -1);
				}
			}
			else
			{
				foreach (TroopRosterElement item3 in ((IEnumerable<TroopRosterElement>)currentPlayerTroopRoster.GetTroopRoster()).Where((TroopRosterElement x) => ((BasicCharacterObject)x.Character).IsHero && ((TroopRosterElement)(ref x)).Number > ((TroopRosterElement)(ref x)).WoundedNumber))
				{
					val.Add(item3);
				}
				TroopRoster strongestAndPriorTroops = MobilePartyHelper.GetStrongestAndPriorTroops(MobileParty.MainParty, maxInitialPlayerTroops - val.TotalManCount, false);
				val.Add(strongestAndPriorTroops);
			}
			OpenTroopSelection(args2, currentPlayerTroopRoster, val, CanChangeStatusOfTroop, delegate(TroopRoster roster)
			{
				OnDone(roster, args2);
			}, maxInitialPlayerTroops, 1);
		}

		private static bool CanChangeStatusOfTroop(CharacterObject character)
		{
			return !((BasicCharacterObject)character).IsPlayerCharacter;
		}

		private static bool IsPlayerArmyBig()
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Expected O, but got Unknown
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			if (PlayerEncounter.Battle != null && PlayerEncounter.Battle.IsFinalized)
			{
				return false;
			}
			if (MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty.Name != MobileParty.MainParty.Name)
			{
				return false;
			}
			if (PlayerEncounter.Battle != null && PlayerSiege.BesiegedSettlement == null && MobileParty.MainParty.Army == null && ((IEnumerable<MapEventParty>)PlayerEncounter.Battle.PartiesOnSide(PlayerEncounter.Battle.PlayerSide)).Count((MapEventParty x) => !x.IsNpcParty) > 1 && PlayerEncounter.Battle.GetLeaderParty(PlayerEncounter.Battle.PlayerSide).Name != PartyBase.MainParty.Name)
			{
				return false;
			}
			if (PlayerEncounter.Battle == null && MapEvent.PlayerMapEvent == null)
			{
				PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
				if (((encounteredParty != null) ? encounteredParty.MapEvent : null) == null)
				{
					if (PlayerSiege.BesiegedSettlement != null)
					{
						_playerArmySize = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide(PlayerSiege.PlayerSide).GetInvolvedPartiesForEventType((BattleTypes)5).Sum((PartyBase x) => x.NumberOfHealthyMembers);
						_enemyArmySize = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide(Extensions.GetOppositeSide(PlayerSiege.PlayerSide)).GetInvolvedPartiesForEventType((BattleTypes)5).Sum((PartyBase x) => x.NumberOfHealthyMembers);
						_playerSide = PlayerSiege.PlayerSide;
						goto IL_0242;
					}
					InformationManager.DisplayMessage(new InformationMessage("ChooseYourTroops Error: Enemy party or besieged settlement not found."));
					return false;
				}
			}
			object obj = PlayerEncounter.Battle;
			if (obj == null)
			{
				obj = MapEvent.PlayerMapEvent;
				if (obj == null)
				{
					PartyBase encounteredParty2 = PlayerEncounter.EncounteredParty;
					obj = ((encounteredParty2 != null) ? encounteredParty2.MapEvent : null);
				}
			}
			MapEvent val = (MapEvent)obj;
			_playerSide = val.PlayerSide;
			try
			{
				_playerArmySize = val.GetMapEventSide(val.PlayerSide).GetTotalHealthyTroopCountOfSide();
				_enemyArmySize = val.GetMapEventSide(Extensions.GetOppositeSide(val.PlayerSide)).GetTotalHealthyTroopCountOfSide();
			}
			catch (Exception)
			{
				return false;
			}
			goto IL_0242;
			IL_0242:
			return true;
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<TroopRoster>("actualTroopRoster", ref _actualTroopRoster);
		}

		private static int GetEnemyInitialSpawn(IEnumerable<FlattenedTroopRosterElement> troops)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			foreach (FlattenedTroopRosterElement item in _actualTroopRoster.ToFlattenedRoster())
			{
				FlattenedTroopRosterElement current = item;
				num += ((!DoesTroopCountByTwo(((FlattenedTroopRosterElement)(ref current)).Troop)) ? 1 : 2);
			}
			int num2 = ChooseYourTroopsConfig.CurrentBattleSizeLimit - num;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < troops.Count(); i++)
			{
				FlattenedTroopRosterElement val = troops.ElementAt(i);
				int num5 = ((!DoesTroopCountByTwo(((FlattenedTroopRosterElement)(ref val)).Troop)) ? 1 : 2);
				if (num4 + num5 > num2)
				{
					break;
				}
				num4 += num5;
				num3++;
			}
			return num3;
		}
	}
	public class SubModule : MBSubModuleBase
	{
		private Harmony _harmony;

		protected override void OnSubModuleLoad()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			((MBSubModuleBase)this).OnSubModuleLoad();
			_harmony = new Harmony("choose_your_troops");
			_harmony.PatchAll();
		}

		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			((MBSubModuleBase)this).OnCampaignStart(game, (object)gameStarterObject);
			CampaignGameStarter val = (CampaignGameStarter)(object)((gameStarterObject is CampaignGameStarter) ? gameStarterObject : null);
			if (val != null)
			{
				val.AddBehavior((CampaignBehaviorBase)(object)new ChooseYourTroopsBehavior());
			}
		}
	}
}
namespace HarmonyLib.BUTR.Extensions
{
	[ExcludeFromCodeCoverage]
	internal readonly struct AccessCacheHandle
	{
		internal enum MemberType
		{
			Any,
			Static,
			Instance
		}

		private delegate object AccessCacheCtorDelegate();

		private delegate FieldInfo GetFieldInfoDelegate(object instance, Type type, string name, MemberType memberType = MemberType.Any, bool declaredOnly = false);

		private delegate PropertyInfo GetPropertyInfoDelegate(object instance, Type type, string name, MemberType memberType = MemberType.Any, bool declaredOnly = false);

		private delegate MethodBase GetMethodInfoDelegate(object instance, Type type, string name, Type[] arguments, MemberType memberType = MemberType.Any, bool declaredOnly = false);

		private static readonly Type Blank;

		private static readonly AccessCacheCtorDelegate? AccessCacheCtorMethod;

		private static readonly GetFieldInfoDelegate? GetFieldInfoMethod;

		private static readonly GetPropertyInfoDelegate? GetPropertyInfoMethod;

		private static readonly GetMethodInfoDelegate? GetMethodInfoMethod;

		private readonly object _accessCache;

		static AccessCacheHandle()
		{
			Blank = typeof(Harmony);
			AccessCacheCtorMethod = AccessTools2.GetDeclaredConstructorDelegate<AccessCacheCtorDelegate>("HarmonyLib.AccessCache");
			GetFieldInfoMethod = AccessTools2.GetDelegateObjectInstance<GetFieldInfoDelegate>("HarmonyLib.AccessCache:GetFieldInfo");
			GetPropertyInfoMethod = AccessTools2.GetDelegateObjectInstance<GetPropertyInfoDelegate>("HarmonyLib.AccessCache:GetPropertyInfo");
			GetMethodInfoMethod = AccessTools2.GetDelegateObjectInstance<GetMethodInfoDelegate>("HarmonyLib.AccessCache:GetMethodInfo");
		}

		public static AccessCacheHandle? Create()
		{
			object obj = AccessCacheCtorMethod?.Invoke();
			return (obj != null) ? new AccessCacheHandle?(new AccessCacheHandle(obj)) : null;
		}

		private AccessCacheHandle(object accessCache)
		{
			_accessCache = accessCache;
		}

		public FieldInfo? GetFieldInfo(Type type, string name, MemberType memberType = MemberType.Any, bool declaredOnly = false)
		{
			return GetFieldInfoMethod?.Invoke(_accessCache, type, name, memberType, declaredOnly);
		}

		public PropertyInfo? GetPropertyInfo(Type type, string name, MemberType memberType = MemberType.Any, bool declaredOnly = false)
		{
			return GetPropertyInfoMethod?.Invoke(_accessCache, type, name, memberType, declaredOnly);
		}

		public MethodBase? GetMethodInfo(Type type, string name, Type[] arguments, MemberType memberType = MemberType.Any, bool declaredOnly = false)
		{
			return GetMethodInfoMethod?.Invoke(_accessCache, type, name, arguments, memberType, declaredOnly);
		}
	}
	internal static class AccessTools2
	{
		[ExcludeFromCodeCoverage]
		private readonly struct DynamicMethodDefinitionHandle
		{
			private readonly object _dynamicMethodDefinition;

			public static DynamicMethodDefinitionHandle? Create(string name, Type returnType, Type[] parameterTypes)
			{
				return (Helper.DynamicMethodDefinitionCtor == null) ? null : new DynamicMethodDefinitionHandle?(new DynamicMethodDefinitionHandle(Helper.DynamicMethodDefinitionCtor(name, returnType, parameterTypes)));
			}

			public DynamicMethodDefinitionHandle(object dynamicMethodDefinition)
			{
				_dynamicMethodDefinition = dynamicMethodDefinition;
			}

			public ILGeneratorHandle? GetILGenerator()
			{
				return (Helper.GetILGenerator == null) ? null : new ILGeneratorHandle?(new ILGeneratorHandle(Helper.GetILGenerator(_dynamicMethodDefinition)));
			}

			public MethodInfo? Generate()
			{
				return (Helper.Generate == null) ? null : Helper.Generate(_dynamicMethodDefinition);
			}
		}

		[ExcludeFromCodeCoverage]
		private readonly struct ILGeneratorHandle
		{
			private readonly object _ilGenerator;

			public ILGeneratorHandle(object ilGenerator)
			{
				_ilGenerator = ilGenerator;
			}

			public void Emit(OpCode opcode)
			{
				Helper.Emit1?.Invoke(_ilGenerator, opcode);
			}

			public void Emit(OpCode opcode, FieldInfo field)
			{
				Helper.Emit2?.Invoke(_ilGenerator, opcode, field);
			}

			public void Emit(OpCode opcode, Type type)
			{
				Helper.Emit3?.Invoke(_ilGenerator, opcode, type);
			}
		}

		[ExcludeFromCodeCoverage]
		private static class Helper
		{
			public delegate object DynamicMethodDefinitionCtorDelegate(string name, Type returnType, Type[] parameterTypes);

			public delegate object GetILGeneratorDelegate(object instance);

			public delegate void Emit1Delegate(object instance, OpCode opcode);

			public delegate void Emit2Delegate(object instance, OpCode opcode, FieldInfo field);

			public delegate void Emit3Delegate(object instance, OpCode opcode, Type type);

			public delegate MethodInfo GenerateDelegate(object instance);

			public static readonly DynamicMethodDefinitionCtorDelegate? DynamicMethodDefinitionCtor;

			public static readonly GetILGeneratorDelegate? GetILGenerator;

			public static readonly Emit1Delegate? Emit1;

			public static readonly Emit2Delegate? Emit2;

			public static readonly Emit3Delegate? Emit3;

			public static readonly GenerateDelegate? Generate;

			static Helper()
			{
				DynamicMethodDefinitionCtor = GetDeclaredConstructorDelegate<DynamicMethodDefinitionCtorDelegate>("MonoMod.Utils.DynamicMethodDefinition", new Type[3]
				{
					typeof(string),
					typeof(Type),
					typeof(Type[])
				});
				GetILGenerator = GetDelegateObjectInstance<GetILGeneratorDelegate>("MonoMod.Utils.DynamicMethodDefinition:GetILGenerator", Type.EmptyTypes);
				Emit1 = GetDelegateObjectInstance<Emit1Delegate>("System.Reflection.Emit.ILGenerator:Emit", new Type[1] { typeof(OpCode) });
				Emit2 = GetDelegateObjectInstance<Emit2Delegate>("System.Reflection.Emit.ILGenerator:Emit", new Type[2]
				{
					typeof(OpCode),
					typeof(FieldInfo)
				});
				Emit3 = GetDelegateObjectInstance<Emit3Delegate>("System.Reflection.Emit.ILGenerator:Emit", new Type[2]
				{
					typeof(OpCode),
					typeof(Type)
				});
				Generate = GetDelegateObjectInstance<GenerateDelegate>("MonoMod.Utils.DynamicMethodDefinition:Generate", Type.EmptyTypes);
			}

			public static bool IsValid(bool logErrorInTrace = true)
			{
				if (DynamicMethodDefinitionCtor == null)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError("AccessTools2.Helper.IsValid: DynamicMethodDefinitionCtor is null");
					}
					return false;
				}
				if (GetILGenerator == null)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError("AccessTools2.Helper.IsValid: GetILGenerator is null");
					}
					return false;
				}
				if (Emit1 == null)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError("AccessTools2.Helper.IsValid: Emit1 is null");
					}
					return false;
				}
				if (Emit2 == null)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError("AccessTools2.Helper.IsValid: Emit2 is null");
					}
					return false;
				}
				if (Emit3 == null)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError("AccessTools2.Helper.IsValid: Emit3 is null");
					}
					return false;
				}
				if (Generate == null)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError("AccessTools2.Helper.IsValid: Generate is null");
					}
					return false;
				}
				return true;
			}
		}

		private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
		{
			typeof(long),
			typeof(ulong),
			typeof(int),
			typeof(uint),
			typeof(short),
			typeof(ushort),
			typeof(byte),
			typeof(sbyte)
		};

		public static ConstructorInfo? DeclaredConstructor(Type type, Type[]? parameters = null, bool searchForStatic = false, bool logErrorInTrace = true)
		{
			if ((object)type == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.DeclaredConstructor: 'type' is null");
				}
				return null;
			}
			if (parameters == null)
			{
				parameters = Type.EmptyTypes;
			}
			BindingFlags bindingAttr = (searchForStatic ? (AccessTools.allDeclared & ~BindingFlags.Instance) : (AccessTools.allDeclared & ~BindingFlags.Static));
			return type.GetConstructor(bindingAttr, null, parameters, new ParameterModifier[0]);
		}

		public static ConstructorInfo? Constructor(Type type, Type[]? parameters = null, bool searchForStatic = false, bool logErrorInTrace = true)
		{
			Type[] parameters2 = parameters;
			if ((object)type == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.ConstructorInfo: 'type' is null");
				}
				return null;
			}
			if (parameters2 == null)
			{
				parameters2 = Type.EmptyTypes;
			}
			BindingFlags flags = (searchForStatic ? (AccessTools.all & ~BindingFlags.Instance) : (AccessTools.all & ~BindingFlags.Static));
			return FindIncludingBaseTypes(type, (Type t) => t.GetConstructor(flags, null, parameters2, new ParameterModifier[0]));
		}

		public static ConstructorInfo? DeclaredConstructor(string typeString, Type[]? parameters = null, bool searchForStatic = false, bool logErrorInTrace = true)
		{
			if (string.IsNullOrWhiteSpace(typeString))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Constructor: 'typeString' is null or whitespace/empty");
				}
				return null;
			}
			Type type = TypeByName(typeString, logErrorInTrace);
			if ((object)type == null)
			{
				return null;
			}
			return DeclaredConstructor(type, parameters, searchForStatic, logErrorInTrace);
		}

		public static ConstructorInfo? Constructor(string typeString, Type[]? parameters = null, bool searchForStatic = false, bool logErrorInTrace = true)
		{
			if (string.IsNullOrWhiteSpace(typeString))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Constructor: 'typeString' is null or whitespace/empty");
				}
				return null;
			}
			Type type = TypeByName(typeString, logErrorInTrace);
			if ((object)type == null)
			{
				return null;
			}
			return Constructor(type, parameters, searchForStatic, logErrorInTrace);
		}

		public static TDelegate? GetDeclaredConstructorDelegate<TDelegate>(Type type, Type[]? parameters = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			ConstructorInfo constructorInfo = DeclaredConstructor(type, parameters, searchForStatic: false, logErrorInTrace);
			return ((object)constructorInfo != null) ? GetDelegate<TDelegate>(constructorInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetConstructorDelegate<TDelegate>(Type type, Type[]? parameters = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			ConstructorInfo constructorInfo = Constructor(type, parameters, searchForStatic: false, logErrorInTrace);
			return ((object)constructorInfo != null) ? GetDelegate<TDelegate>(constructorInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDeclaredConstructorDelegate<TDelegate>(string typeString, Type[]? parameters = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			ConstructorInfo constructorInfo = DeclaredConstructor(typeString, parameters, searchForStatic: false, logErrorInTrace);
			return ((object)constructorInfo != null) ? GetDelegate<TDelegate>(constructorInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetConstructorDelegate<TDelegate>(string typeString, Type[]? parameters = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			ConstructorInfo constructorInfo = Constructor(typeString, parameters, searchForStatic: false, logErrorInTrace);
			return ((object)constructorInfo != null) ? GetDelegate<TDelegate>(constructorInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetPropertyGetterDelegate<TDelegate>(PropertyInfo propertyInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = propertyInfo?.GetGetMethod(nonPublic: true);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetPropertySetterDelegate<TDelegate>(PropertyInfo propertyInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = propertyInfo?.GetSetMethod(nonPublic: true);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetPropertyGetterDelegate<TDelegate>(object? instance, PropertyInfo propertyInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = propertyInfo?.GetGetMethod(nonPublic: true);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetPropertySetterDelegate<TDelegate>(object? instance, PropertyInfo propertyInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = propertyInfo?.GetSetMethod(nonPublic: true);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDeclaredPropertyGetterDelegate<TDelegate>(Type type, string name, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = DeclaredPropertyGetter(type, name, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDeclaredPropertySetterDelegate<TDelegate>(Type type, string name, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = DeclaredPropertySetter(type, name, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetPropertyGetterDelegate<TDelegate>(Type type, string name, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = PropertyGetter(type, name, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetPropertySetterDelegate<TDelegate>(Type type, string name, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = PropertySetter(type, name, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDeclaredPropertyGetterDelegate<TDelegate>(object? instance, Type type, string method, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = DeclaredPropertyGetter(type, method, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDeclaredPropertySetterDelegate<TDelegate>(object? instance, Type type, string method, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = DeclaredPropertySetter(type, method, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetPropertyGetterDelegate<TDelegate>(object? instance, Type type, string method, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = PropertyGetter(type, method, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetPropertySetterDelegate<TDelegate>(object? instance, Type type, string method, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = PropertySetter(type, method, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDeclaredPropertyGetterDelegate<TDelegate>(string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = DeclaredPropertyGetter(typeColonPropertyName, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDeclaredPropertySetterDelegate<TDelegate>(string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = DeclaredPropertySetter(typeColonPropertyName, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetPropertyGetterDelegate<TDelegate>(string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = PropertyGetter(typeColonPropertyName, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetPropertySetterDelegate<TDelegate>(string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = PropertySetter(typeColonPropertyName, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDeclaredPropertyGetterDelegate<TDelegate>(object? instance, string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = DeclaredPropertyGetter(typeColonPropertyName, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDeclaredPropertySetterDelegate<TDelegate>(object? instance, string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = DeclaredPropertySetter(typeColonPropertyName, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetPropertyGetterDelegate<TDelegate>(object? instance, string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = PropertyGetter(typeColonPropertyName, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetPropertySetterDelegate<TDelegate>(object? instance, string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = PropertySetter(typeColonPropertyName, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDelegate<TDelegate>(ConstructorInfo constructorInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			if ((object)constructorInfo == null)
			{
				return null;
			}
			MethodInfo method = typeof(TDelegate).GetMethod("Invoke");
			if ((object)method == null)
			{
				return null;
			}
			if (!method.ReturnType.IsAssignableFrom(constructorInfo.DeclaringType))
			{
				return null;
			}
			ParameterInfo[] parameters = method.GetParameters();
			ParameterInfo[] constructorParameters = constructorInfo.GetParameters();
			if (parameters.Length - constructorParameters.Length != 0 && !ParametersAreEqual(parameters, constructorParameters))
			{
				return null;
			}
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "instance");
			List<ParameterExpression> list = parameters.Select((ParameterInfo pi, int i) => Expression.Parameter(pi.ParameterType, $"p{i}")).ToList();
			List<Expression> arguments = list.Select((ParameterExpression pe, int i) => (pe.IsByRef || pe.Type.Equals(constructorParameters[i].ParameterType)) ? ((Expression)pe) : ((Expression)Expression.Convert(pe, constructorParameters[i].ParameterType))).ToList();
			Expression expression = Expression.New(constructorInfo, arguments);
			UnaryExpression body = Expression.Convert(expression, method.ReturnType);
			try
			{
				return Expression.Lambda<TDelegate>(body, list).Compile();
			}
			catch (Exception arg)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError($"AccessTools2.GetDelegate<{typeof(TDelegate).FullName}>: Error while compiling lambds expression '{arg}'");
				}
				return null;
			}
		}

		public static TDelegate? GetDelegate<TDelegate>(object? instance, MethodInfo methodInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			if ((object)methodInfo == null)
			{
				return null;
			}
			MethodInfo method = typeof(TDelegate).GetMethod("Invoke");
			if ((object)method == null)
			{
				return null;
			}
			bool flag = method.ReturnType.IsEnum || methodInfo.ReturnType.IsEnum;
			bool flag2 = method.ReturnType.IsNumeric() || methodInfo.ReturnType.IsNumeric();
			if (!flag && !flag2 && !method.ReturnType.IsAssignableFrom(methodInfo.ReturnType))
			{
				return null;
			}
			ParameterInfo[] parameters = method.GetParameters();
			ParameterInfo[] methodParameters = methodInfo.GetParameters();
			bool flag3 = parameters.Length - methodParameters.Length == 0 && ParametersAreEqual(parameters, methodParameters);
			bool flag4 = instance != null;
			bool flag5 = parameters.Length - methodParameters.Length == 1 && (parameters[0].ParameterType.IsAssignableFrom(methodInfo.DeclaringType) || methodInfo.DeclaringType.IsAssignableFrom(parameters[0].ParameterType));
			if (!flag4 && !flag5 && !methodInfo.IsStatic)
			{
				return null;
			}
			if (flag4 && methodInfo.IsStatic)
			{
				return null;
			}
			if (flag4 && !methodInfo.IsStatic && !methodInfo.DeclaringType.IsAssignableFrom(instance.GetType()))
			{
				return null;
			}
			if (flag3 && flag5)
			{
				return null;
			}
			if (flag4 && (flag5 || !flag3))
			{
				return null;
			}
			if (flag5 && (flag4 || flag3))
			{
				return null;
			}
			if (!flag5 && !flag4 && !flag3)
			{
				return null;
			}
			ParameterExpression parameterExpression = (flag5 ? Expression.Parameter(parameters[0].ParameterType, "instance") : null);
			List<ParameterExpression> list = parameters.Skip(flag5 ? 1 : 0).Select((ParameterInfo pi, int i) => Expression.Parameter(pi.ParameterType, $"p{i}")).ToList();
			List<Expression> arguments = list.Select((ParameterExpression pe, int i) => (pe.IsByRef || pe.Type.Equals(methodParameters[i].ParameterType)) ? ((Expression)pe) : ((Expression)Expression.Convert(pe, methodParameters[i].ParameterType))).ToList();
			MethodCallExpression methodCallExpression = ((!flag4) ? (flag3 ? Expression.Call(methodInfo, arguments) : ((!flag5) ? null : (parameterExpression.Type.Equals(methodInfo.DeclaringType) ? Expression.Call(parameterExpression, methodInfo, arguments) : Expression.Call(Expression.Convert(parameterExpression, methodInfo.DeclaringType), methodInfo, arguments)))) : (instance.GetType().Equals(methodInfo.DeclaringType) ? Expression.Call(Expression.Constant(instance), methodInfo, arguments) : Expression.Call(Expression.Convert(Expression.Constant(instance), instance.GetType()), methodInfo, arguments)));
			if (methodCallExpression == null)
			{
				return null;
			}
			UnaryExpression body = Expression.Convert(methodCallExpression, method.ReturnType);
			try
			{
				IEnumerable<ParameterExpression> parameters2;
				if (!flag5)
				{
					IEnumerable<ParameterExpression> enumerable = list;
					parameters2 = enumerable;
				}
				else
				{
					parameters2 = new List<ParameterExpression> { parameterExpression }.Concat(list);
				}
				return Expression.Lambda<TDelegate>(body, parameters2).Compile();
			}
			catch (Exception arg)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError($"AccessTools2.GetDelegate<{typeof(TDelegate).FullName}>: Error while compiling lambds expression '{arg}'");
				}
				return null;
			}
		}

		public static TDelegate? GetDelegate<TDelegate>(MethodInfo methodInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			return GetDelegate<TDelegate>(null, methodInfo, logErrorInTrace);
		}

		public static TDelegate? GetDelegateObjectInstance<TDelegate>(MethodInfo methodInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			return GetDelegate<TDelegate>(methodInfo, logErrorInTrace);
		}

		public static bool IsNumeric(this Type myType)
		{
			return NumericTypes.Contains(myType);
		}

		private static bool ParametersAreEqual(ParameterInfo[] delegateParameters, ParameterInfo[] methodParameters)
		{
			if (delegateParameters.Length - methodParameters.Length == 0)
			{
				for (int i = 0; i < methodParameters.Length; i++)
				{
					if (delegateParameters[i].ParameterType.IsByRef != methodParameters[i].ParameterType.IsByRef)
					{
						return false;
					}
					bool flag = delegateParameters[i].ParameterType.IsEnum || methodParameters[i].ParameterType.IsEnum;
					bool flag2 = delegateParameters[i].ParameterType.IsNumeric() || methodParameters[i].ParameterType.IsNumeric();
					if (!flag && !flag2 && !delegateParameters[i].ParameterType.IsAssignableFrom(methodParameters[i].ParameterType))
					{
						return false;
					}
				}
				return true;
			}
			if (delegateParameters.Length - methodParameters.Length == 1)
			{
				for (int j = 0; j < methodParameters.Length; j++)
				{
					if (delegateParameters[j + 1].ParameterType.IsByRef != methodParameters[j].ParameterType.IsByRef)
					{
						return false;
					}
					bool flag3 = delegateParameters[j + 1].ParameterType.IsEnum || methodParameters[j].ParameterType.IsEnum;
					bool flag4 = delegateParameters[j + 1].ParameterType.IsNumeric() || methodParameters[j].ParameterType.IsNumeric();
					if (!flag3 && !flag4 && !delegateParameters[j + 1].ParameterType.IsAssignableFrom(methodParameters[j].ParameterType))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public static TDelegate? GetDelegate<TDelegate, TInstance>(TInstance instance, MethodInfo methodInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			return GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace);
		}

		public static TDelegate? GetDeclaredDelegateObjectInstance<TDelegate>(Type type, string method, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = DeclaredMethod(type, method, parameters, generics, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegateObjectInstance<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDelegateObjectInstance<TDelegate>(Type type, string method, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = Method(type, method, parameters, generics, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegateObjectInstance<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDeclaredDelegateObjectInstance<TDelegate>(string typeSemicolonMethod, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = DeclaredMethod(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegateObjectInstance<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDelegateObjectInstance<TDelegate>(string typeSemicolonMethod, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = Method(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegateObjectInstance<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDeclaredDelegate<TDelegate>(Type type, string method, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = DeclaredMethod(type, method, parameters, generics, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDelegate<TDelegate>(Type type, string method, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = Method(type, method, parameters, generics, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDeclaredDelegate<TDelegate>(string typeSemicolonMethod, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = DeclaredMethod(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDelegate<TDelegate>(string typeSemicolonMethod, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = Method(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDeclaredDelegate<TDelegate, TInstance>(TInstance instance, string method, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			object result;
			if (instance != null)
			{
				MethodInfo methodInfo = DeclaredMethod(instance.GetType(), method, parameters, generics, logErrorInTrace);
				if ((object)methodInfo != null)
				{
					result = GetDelegate<TDelegate, TInstance>(instance, methodInfo, logErrorInTrace);
					goto IL_0037;
				}
			}
			result = null;
			goto IL_0037;
			IL_0037:
			return (TDelegate?)result;
		}

		public static TDelegate? GetDelegate<TDelegate, TInstance>(TInstance instance, string method, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			object result;
			if (instance != null)
			{
				MethodInfo methodInfo = Method(instance.GetType(), method, parameters, generics, logErrorInTrace);
				if ((object)methodInfo != null)
				{
					result = GetDelegate<TDelegate, TInstance>(instance, methodInfo, logErrorInTrace);
					goto IL_0037;
				}
			}
			result = null;
			goto IL_0037;
			IL_0037:
			return (TDelegate?)result;
		}

		public static TDelegate? GetDeclaredDelegate<TDelegate>(object? instance, Type type, string method, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = DeclaredMethod(type, method, parameters, generics, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDelegate<TDelegate>(object? instance, Type type, string method, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = Method(type, method, parameters, generics, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDeclaredDelegate<TDelegate>(object? instance, string typeSemicolonMethod, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = DeclaredMethod(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : null;
		}

		public static TDelegate? GetDelegate<TDelegate>(object? instance, string typeSemicolonMethod, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = Method(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			return ((object)methodInfo != null) ? GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : null;
		}

		public static FieldInfo? DeclaredField(Type type, string name, bool logErrorInTrace = true)
		{
			if ((object)type == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.DeclaredField: 'type' is null");
				}
				return null;
			}
			if (name == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError($"AccessTools2.DeclaredField: type '{type}', 'name' is null");
				}
				return null;
			}
			FieldInfo field = type.GetField(name, AccessTools.allDeclared);
			if ((object)field == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError($"AccessTools2.DeclaredField: Could not find field for type '{type}' and name '{name}'");
				}
				return null;
			}
			return field;
		}

		public static FieldInfo? Field(Type type, string name, bool logErrorInTrace = true)
		{
			string name2 = name;
			if ((object)type == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Field: 'type' is null");
				}
				return null;
			}
			if (name2 == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError($"AccessTools2.Field: type '{type}', 'name' is null");
				}
				return null;
			}
			FieldInfo fieldInfo = FindIncludingBaseTypes(type, (Type t) => t.GetField(name2, AccessTools.all));
			if ((object)fieldInfo == null && logErrorInTrace)
			{
				Trace.TraceError($"AccessTools2.Field: Could not find field for type '{type}' and name '{name2}'");
			}
			return fieldInfo;
		}

		public static FieldInfo? DeclaredField(string typeColonFieldname, bool logErrorInTrace = true)
		{
			if (!TryGetComponents(typeColonFieldname, out Type type, out string name, logErrorInTrace))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Field: Could not find type or field for '" + typeColonFieldname + "'");
				}
				return null;
			}
			return DeclaredField(type, name, logErrorInTrace);
		}

		public static FieldInfo? Field(string typeColonFieldname, bool logErrorInTrace = true)
		{
			if (!TryGetComponents(typeColonFieldname, out Type type, out string name, logErrorInTrace))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Field: Could not find type or field for '" + typeColonFieldname + "'");
				}
				return null;
			}
			return Field(type, name, logErrorInTrace);
		}

		public static FieldRef<object, F>? FieldRefAccess<F>(string typeColonFieldname, bool logErrorInTrace = true)
		{
			if (!TryGetComponents(typeColonFieldname, out Type type, out string name, logErrorInTrace))
			{
				Trace.TraceError("AccessTools2.FieldRefAccess: Could not find type or field for '" + typeColonFieldname + "'");
				return null;
			}
			return FieldRefAccess<F>(type, name, logErrorInTrace);
		}

		public static FieldRef<T, F>? FieldRefAccess<T, F>(string fieldName, bool logErrorInTrace = true) where T : class
		{
			if (fieldName == null)
			{
				return null;
			}
			FieldInfo instanceField = GetInstanceField(typeof(T), fieldName, logErrorInTrace);
			if ((object)instanceField == null)
			{
				return null;
			}
			return FieldRefAccessInternal<T, F>(instanceField, needCastclass: false, logErrorInTrace);
		}

		public static FieldRef<object, F>? FieldRefAccess<F>(Type type, string fieldName, bool logErrorInTrace = true)
		{
			if ((object)type == null)
			{
				return null;
			}
			if (fieldName == null)
			{
				return null;
			}
			FieldInfo fieldInfo = Field(type, fieldName, logErrorInTrace);
			if ((object)fieldInfo == null)
			{
				return null;
			}
			if (!fieldInfo.IsStatic)
			{
				Type declaringType = fieldInfo.DeclaringType;
				if ((object)declaringType != null)
				{
					if (declaringType.IsValueType)
					{
						if (logErrorInTrace)
						{
							Trace.TraceError("AccessTools2.FieldRefAccess<object, " + typeof(F).FullName + ">: FieldDeclaringType must be a class");
						}
						return null;
					}
					return FieldRefAccessInternal<object, F>(fieldInfo, needCastclass: true, logErrorInTrace);
				}
			}
			return null;
		}

		public static FieldRef<T, F>? FieldRefAccess<T, F>(FieldInfo fieldInfo, bool logErrorInTrace = true) where T : class
		{
			if ((object)fieldInfo == null)
			{
				return null;
			}
			if (!fieldInfo.IsStatic)
			{
				Type declaringType = fieldInfo.DeclaringType;
				if ((object)declaringType != null)
				{
					if (declaringType.IsValueType)
					{
						if (logErrorInTrace)
						{
							Trace.TraceError("AccessTools2.FieldRefAccess<" + typeof(T).FullName + ", " + typeof(F).FullName + ">: FieldDeclaringType must be a class");
						}
						return null;
					}
					bool? flag = FieldRefNeedsClasscast(typeof(T), declaringType, logErrorInTrace);
					if (flag.HasValue)
					{
						bool valueOrDefault = flag.GetValueOrDefault();
						if (true)
						{
							return FieldRefAccessInternal<T, F>(fieldInfo, valueOrDefault, logErrorInTrace);
						}
					}
					return null;
				}
			}
			return null;
		}

		private static FieldRef<T, F>? FieldRefAccessInternal<T, F>(FieldInfo fieldInfo, bool needCastclass, bool logErrorInTrace = true) where T : class
		{
			if (!Helper.IsValid(logErrorInTrace))
			{
				return null;
			}
			if (fieldInfo.IsStatic)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.FieldRefAccessInternal<" + typeof(T).FullName + ", " + typeof(F).FullName + ">: Field must not be static");
				}
				return null;
			}
			if (!ValidateFieldType<F>(fieldInfo, logErrorInTrace))
			{
				return null;
			}
			Type typeFromHandle = typeof(T);
			Type declaringType = fieldInfo.DeclaringType;
			DynamicMethodDefinitionHandle? dynamicMethodDefinitionHandle = DynamicMethodDefinitionHandle.Create("__refget_" + typeFromHandle.Name + "_fi_" + fieldInfo.Name, typeof(F).MakeByRefType(), new Type[1] { typeFromHandle });
			ILGeneratorHandle? iLGeneratorHandle = dynamicMethodDefinitionHandle?.GetILGenerator();
			if (iLGeneratorHandle.HasValue)
			{
				ILGeneratorHandle valueOrDefault = iLGeneratorHandle.GetValueOrDefault();
				if (true)
				{
					valueOrDefault.Emit(OpCodes.Ldarg_0);
					if (needCastclass)
					{
						valueOrDefault.Emit(OpCodes.Castclass, declaringType);
					}
					valueOrDefault.Emit(OpCodes.Ldflda, fieldInfo);
					valueOrDefault.Emit(OpCodes.Ret);
					return dynamicMethodDefinitionHandle?.Generate()?.CreateDelegate(typeof(FieldRef<T, F>)) as FieldRef<T, F>;
				}
			}
			return null;
		}

		private static bool? FieldRefNeedsClasscast(Type delegateInstanceType, Type declaringType, bool logErrorInTrace = true)
		{
			bool flag = false;
			if (delegateInstanceType != declaringType)
			{
				flag = delegateInstanceType.IsAssignableFrom(declaringType);
				if (!flag && !declaringType.IsAssignableFrom(delegateInstanceType))
				{
					if (logErrorInTrace)
					{
						Trace.TraceError($"AccessTools2.FieldRefNeedsClasscast: FieldDeclaringType must be assignable from or to T (FieldRefAccess instance type) - 'instanceOfT is FieldDeclaringType' must be possible, delegateInstanceType '{delegateInstanceType}', declaringType '{declaringType}'");
					}
					return null;
				}
			}
			return flag;
		}

		public static FieldRef<object, TField>? FieldRefAccess<TField>(FieldInfo fieldInfo)
		{
			return ((object)fieldInfo == null) ? null : AccessTools.FieldRefAccess<object, TField>(fieldInfo);
		}

		public static MethodInfo? DeclaredMethod(Type type, string name, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true)
		{
			if ((object)type == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.DeclaredMethod: 'type' is null");
				}
				return null;
			}
			if (name == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError($"AccessTools2.DeclaredMethod: type '{type}', 'name' is null");
				}
				return null;
			}
			MethodInfo methodInfo;
			if (parameters == null)
			{
				try
				{
					methodInfo = type.GetMethod(name, AccessTools.allDeclared);
				}
				catch (AmbiguousMatchException ex)
				{
					methodInfo = type.GetMethod(name, AccessTools.allDeclared, null, Type.EmptyTypes, new ParameterModifier[0]);
					if ((object)methodInfo == null)
					{
						if (logErrorInTrace)
						{
							Trace.TraceError($"AccessTools2.DeclaredMethod: Ambiguous match for type '{type}' and name '{name}' and parameters '{((parameters != null) ? GeneralExtensions.Description(parameters) : null)}', '{ex}'");
						}
						return null;
					}
				}
			}
			else
			{
				methodInfo = type.GetMethod(name, AccessTools.allDeclared, null, parameters, new ParameterModifier[0]);
			}
			if ((object)methodInfo == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError($"AccessTools2.DeclaredMethod: Could not find method for type '{type}' and name '{name}' and parameters '{((parameters != null) ? GeneralExtensions.Description(parameters) : null)}'");
				}
				return null;
			}
			if (generics != null)
			{
				methodInfo = methodInfo.MakeGenericMethod(generics);
			}
			return methodInfo;
		}

		public static MethodInfo? Method(Type type, string name, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true)
		{
			string name2 = name;
			Type[] parameters2 = parameters;
			if ((object)type == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Method: 'type' is null");
				}
				return null;
			}
			if (name2 == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError($"AccessTools2.Method: type '{type}', 'name' is null");
				}
				return null;
			}
			MethodInfo methodInfo;
			if (parameters2 == null)
			{
				try
				{
					methodInfo = FindIncludingBaseTypes(type, (Type t) => t.GetMethod(name2, AccessTools.all));
				}
				catch (AmbiguousMatchException ex)
				{
					methodInfo = FindIncludingBaseTypes(type, (Type t) => t.GetMethod(name2, AccessTools.all, null, Type.EmptyTypes, new ParameterModifier[0]));
					if ((object)methodInfo == null)
					{
						if (logErrorInTrace)
						{
							object[] obj = new object[4] { type, name2, null, null };
							Type[] array = parameters2;
							obj[2] = ((array != null) ? GeneralExtensions.Description(array) : null);
							obj[3] = ex;
							Trace.TraceError(string.Format("AccessTools2.Method: Ambiguous match for type '{0}' and name '{1}' and parameters '{2}', '{3}'", obj));
						}
						return null;
					}
				}
			}
			else
			{
				methodInfo = FindIncludingBaseTypes(type, (Type t) => t.GetMethod(name2, AccessTools.all, null, parameters2, new ParameterModifier[0]));
			}
			if ((object)methodInfo == null)
			{
				if (logErrorInTrace)
				{
					string arg = name2;
					Type[] array2 = parameters2;
					Trace.TraceError($"AccessTools2.Method: Could not find method for type '{type}' and name '{arg}' and parameters '{((array2 != null) ? GeneralExtensions.Description(array2) : null)}'");
				}
				return null;
			}
			if (generics != null)
			{
				methodInfo = methodInfo.MakeGenericMethod(generics);
			}
			return methodInfo;
		}

		public static MethodInfo? DeclaredMethod(string typeColonMethodname, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true)
		{
			if (!TryGetComponents(typeColonMethodname, out Type type, out string name, logErrorInTrace))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Method: Could not find type or property for '" + typeColonMethodname + "'");
				}
				return null;
			}
			return DeclaredMethod(type, name, parameters, generics, logErrorInTrace);
		}

		public static MethodInfo? Method(string typeColonMethodname, Type[]? parameters = null, Type[]? generics = null, bool logErrorInTrace = true)
		{
			if (!TryGetComponents(typeColonMethodname, out Type type, out string name, logErrorInTrace))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Method: Could not find type or property for '" + typeColonMethodname + "'");
				}
				return null;
			}
			return Method(type, name, parameters, generics, logErrorInTrace);
		}

		public static PropertyInfo? DeclaredProperty(Type type, string name, bool logErrorInTrace = true)
		{
			if ((object)type == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.DeclaredProperty: 'type' is null");
				}
				return null;
			}
			if (name == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError($"AccessTools2.DeclaredProperty: type '{type}', 'name' is null");
				}
				return null;
			}
			PropertyInfo property = type.GetProperty(name, AccessTools.allDeclared);
			if ((object)property == null && logErrorInTrace)
			{
				Trace.TraceError($"AccessTools2.DeclaredProperty: Could not find property for type '{type}' and name '{name}'");
			}
			return property;
		}

		public static PropertyInfo? Property(Type type, string name, bool logErrorInTrace = true)
		{
			string name2 = name;
			if ((object)type == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Property: 'type' is null");
				}
				return null;
			}
			if (name2 == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError($"AccessTools2.Property: type '{type}', 'name' is null");
				}
				return null;
			}
			PropertyInfo propertyInfo = FindIncludingBaseTypes(type, (Type t) => t.GetProperty(name2, AccessTools.all));
			if ((object)propertyInfo == null && logErrorInTrace)
			{
				Trace.TraceError($"AccessTools2.Property: Could not find property for type '{type}' and name '{name2}'");
			}
			return propertyInfo;
		}

		public static MethodInfo? DeclaredPropertyGetter(Type type, string name, bool logErrorInTrace = true)
		{
			return DeclaredProperty(type, name, logErrorInTrace)?.GetGetMethod(nonPublic: true);
		}

		public static MethodInfo? DeclaredPropertySetter(Type type, string name, bool logErrorInTrace = true)
		{
			return DeclaredProperty(type, name, logErrorInTrace)?.GetSetMethod(nonPublic: true);
		}

		public static MethodInfo? PropertyGetter(Type type, string name, bool logErrorInTrace = true)
		{
			return Property(type, name, logErrorInTrace)?.GetGetMethod(nonPublic: true);
		}

		public static MethodInfo? PropertySetter(Type type, string name, bool logErrorInTrace = true)
		{
			return Property(type, name, logErrorInTrace)?.GetSetMethod(nonPublic: true);
		}

		public static PropertyInfo? DeclaredProperty(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			if (!TryGetComponents(typeColonPropertyName, out Type type, out string name, logErrorInTrace))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.DeclaredProperty: Could not find type or property for '" + typeColonPropertyName + "'");
				}
				return null;
			}
			return DeclaredProperty(type, name, logErrorInTrace);
		}

		public static PropertyInfo? Property(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			if (!TryGetComponents(typeColonPropertyName, out Type type, out string name, logErrorInTrace))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Property: Could not find type or property for '" + typeColonPropertyName + "'");
				}
				return null;
			}
			return Property(type, name, logErrorInTrace);
		}

		public static MethodInfo? DeclaredPropertySetter(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			return DeclaredProperty(typeColonPropertyName, logErrorInTrace)?.GetSetMethod(nonPublic: true);
		}

		public static MethodInfo? DeclaredPropertyGetter(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			return DeclaredProperty(typeColonPropertyName, logErrorInTrace)?.GetGetMethod(nonPublic: true);
		}

		public static MethodInfo? PropertyGetter(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			return Property(typeColonPropertyName, logErrorInTrace)?.GetGetMethod(nonPublic: true);
		}

		public static MethodInfo? PropertySetter(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			return Property(typeColonPropertyName, logErrorInTrace)?.GetSetMethod(nonPublic: true);
		}

		public static FieldRef<TField>? StaticFieldRefAccess<TField>(string typeColonFieldname, bool logErrorInTrace = true)
		{
			if (!TryGetComponents(typeColonFieldname, out Type type, out string name, logErrorInTrace))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.StaticFieldRefAccess: Could not find type or field for '" + typeColonFieldname + "'");
				}
				return null;
			}
			return StaticFieldRefAccess<TField>(type, name, logErrorInTrace);
		}

		public static FieldRef<F>? StaticFieldRefAccess<F>(FieldInfo fieldInfo, bool logErrorInTrace = true)
		{
			if ((object)fieldInfo == null)
			{
				return null;
			}
			return StaticFieldRefAccessInternal<F>(fieldInfo, logErrorInTrace);
		}

		public static FieldRef<TField>? StaticFieldRefAccess<TField>(Type type, string fieldName, bool logErrorInTrace = true)
		{
			FieldInfo fieldInfo = Field(type, fieldName, logErrorInTrace);
			if ((object)fieldInfo == null)
			{
				return null;
			}
			return StaticFieldRefAccess<TField>(fieldInfo, logErrorInTrace);
		}

		private static FieldRef<F>? StaticFieldRefAccessInternal<F>(FieldInfo fieldInfo, bool logErrorInTrace = true)
		{
			if (!Helper.IsValid(logErrorInTrace))
			{
				return null;
			}
			if (!fieldInfo.IsStatic)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.StaticFieldRefAccessInternal<" + typeof(F).FullName + ">: Field must be static");
				}
				return null;
			}
			if (!ValidateFieldType<F>(fieldInfo, logErrorInTrace))
			{
				return null;
			}
			DynamicMethodDefinitionHandle? dynamicMethodDefinitionHandle = DynamicMethodDefinitionHandle.Create("__refget_" + (fieldInfo.DeclaringType?.Name ?? "null") + "_static_fi_" + fieldInfo.Name, typeof(F).MakeByRefType(), new Type[0]);
			ILGeneratorHandle? iLGeneratorHandle = dynamicMethodDefinitionHandle?.GetILGenerator();
			if (iLGeneratorHandle.HasValue)
			{
				ILGeneratorHandle valueOrDefault = iLGeneratorHandle.GetValueOrDefault();
				if (true)
				{
					valueOrDefault.Emit(OpCodes.Ldsflda, fieldInfo);
					valueOrDefault.Emit(OpCodes.Ret);
					return dynamicMethodDefinitionHandle?.Generate()?.CreateDelegate(typeof(FieldRef<F>)) as FieldRef<F>;
				}
			}
			return null;
		}

		public static StructFieldRef<T, F>? StructFieldRefAccess<T, F>(string fieldName, bool logErrorInTrace = true) where T : struct
		{
			if (string.IsNullOrEmpty(fieldName))
			{
				return null;
			}
			FieldInfo instanceField = GetInstanceField(typeof(T), fieldName, logErrorInTrace);
			if ((object)instanceField == null)
			{
				return null;
			}
			return StructFieldRefAccessInternal<T, F>(instanceField, logErrorInTrace);
		}

		public static StructFieldRef<T, F>? StructFieldRefAccess<T, F>(FieldInfo? fieldInfo, bool logErrorInTrace = true) where T : struct
		{
			if ((object)fieldInfo == null)
			{
				return null;
			}
			if (!ValidateStructField<T, F>(fieldInfo, logErrorInTrace))
			{
				return null;
			}
			return StructFieldRefAccessInternal<T, F>(fieldInfo, logErrorInTrace);
		}

		private static StructFieldRef<T, F>? StructFieldRefAccessInternal<T, F>(FieldInfo fieldInfo, bool logErrorInTrace = true) where T : struct
		{
			if (!ValidateFieldType<F>(fieldInfo, logErrorInTrace))
			{
				return null;
			}
			DynamicMethodDefinitionHandle? dynamicMethodDefinitionHandle = DynamicMethodDefinitionHandle.Create("__refget_" + typeof(T).Name + "_struct_fi_" + fieldInfo.Name, typeof(F).MakeByRefType(), new Type[1] { typeof(T).MakeByRefType() });
			ILGeneratorHandle? iLGeneratorHandle = dynamicMethodDefinitionHandle?.GetILGenerator();
			if (iLGeneratorHandle.HasValue)
			{
				ILGeneratorHandle valueOrDefault = iLGeneratorHandle.GetValueOrDefault();
				if (true)
				{
					valueOrDefault.Emit(OpCodes.Ldarg_0);
					valueOrDefault.Emit(OpCodes.Ldflda, fieldInfo);
					valueOrDefault.Emit(OpCodes.Ret);
					return dynamicMethodDefinitionHandle?.Generate()?.CreateDelegate(typeof(StructFieldRef<T, F>)) as StructFieldRef<T, F>;
				}
			}
			return null;
		}

		public static IEnumerable<Assembly> AllAssemblies()
		{
			return from a in AppDomain.CurrentDomain.GetAssemblies()
				where !a.FullName.StartsWith("Microsoft.VisualStudio")
				select a;
		}

		public static IEnumerable<Type> AllTypes()
		{
			return AllAssemblies().SelectMany((Assembly a) => GetTypesFromAssembly(a));
		}

		public static Type[] GetTypesFromAssembly(Assembly assembly, bool logErrorInTrace = true)
		{
			if ((object)assembly == null)
			{
				return Type.EmptyTypes;
			}
			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError($"AccessTools2.GetTypesFromAssembly: assembly {assembly} => {ex}");
				}
				return ex.Types.Where((Type type) => (object)type != null).ToArray();
			}
		}

		public static Type[] GetTypesFromAssemblyIfValid(Assembly assembly, bool logErrorInTrace = true)
		{
			if ((object)assembly == null)
			{
				return Type.EmptyTypes;
			}
			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException arg)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError($"AccessTools2.GetTypesFromAssemblyIfValid: assembly {assembly} => {arg}");
				}
				return Type.EmptyTypes;
			}
		}

		public static Type? TypeByName(string name, bool logErrorInTrace = true)
		{
			string name2 = name;
			if (string.IsNullOrEmpty(name2))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.TypeByName: 'name' is null or empty");
				}
				return null;
			}
			Type type = Type.GetType(name2, throwOnError: false);
			if ((object)type == null)
			{
				type = AllTypes().FirstOrDefault((Type t) => t.FullName == name2);
			}
			if ((object)type == null)
			{
				type = AllTypes().FirstOrDefault((Type t) => t.Name == name2);
			}
			if ((object)type == null && logErrorInTrace)
			{
				Trace.TraceError("AccessTools2.TypeByName: Could not find type named '" + name2 + "'");
			}
			return type;
		}

		public static T? FindIncludingBaseTypes<T>(Type type, Func<Type, T> func) where T : class
		{
			if ((object)type == null || func == null)
			{
				return null;
			}
			do
			{
				T val = func(type);
				if (val != null)
				{
					return val;
				}
				type = type.BaseType;
			}
			while ((object)type != null);
			return null;
		}

		private static FieldInfo? GetInstanceField(Type type, string fieldName, bool logErrorInTrace = true)
		{
			FieldInfo fieldInfo = Field(type, fieldName, logErrorInTrace);
			if ((object)fieldInfo == null)
			{
				return null;
			}
			if (fieldInfo.IsStatic)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError($"AccessTools2.GetInstanceField: Field must not be static, type '{type}', fieldName '{fieldName}'");
				}
				return null;
			}
			return fieldInfo;
		}

		private static bool ValidateFieldType<F>(FieldInfo? fieldInfo, bool logErrorInTrace = true)
		{
			if ((object)fieldInfo == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.ValidateFieldType<" + typeof(F).FullName + ">: 'fieldInfo' is null");
				}
				return false;
			}
			Type typeFromHandle = typeof(F);
			Type fieldType = fieldInfo.FieldType;
			if (typeFromHandle == fieldType)
			{
				return true;
			}
			if (fieldType.IsEnum)
			{
				Type underlyingType = Enum.GetUnderlyingType(fieldType);
				if (typeFromHandle != underlyingType)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError($"AccessTools2.ValidateFieldType<{typeof(F).FullName}>: FieldRefAccess return type must be the same as FieldType or FieldType's underlying integral type ({underlyingType}) for enum types, fieldInfo '{fieldInfo}'");
					}
					return false;
				}
			}
			else
			{
				if (fieldType.IsValueType)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError($"AccessTools2.ValidateFieldType<{typeof(F).FullName}>: FieldRefAccess return type must be the same as FieldType for value types, fieldInfo '{fieldInfo}'");
					}
					return false;
				}
				if (!typeFromHandle.IsAssignableFrom(fieldType))
				{
					if (logErrorInTrace)
					{
						Trace.TraceError("AccessTools2.ValidateFieldType<" + typeof(F).FullName + ">: FieldRefAccess return type must be assignable from FieldType for reference types");
					}
					return false;
				}
			}
			return true;
		}

		private static bool ValidateStructField<T, F>(FieldInfo? fieldInfo, bool logErrorInTrace = true) where T : struct
		{
			if ((object)fieldInfo == null)
			{
				return false;
			}
			if (fieldInfo.IsStatic)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.ValidateStructField<" + typeof(T).FullName + ", " + typeof(F).FullName + ">: Field must not be static");
				}
				return false;
			}
			if (fieldInfo.DeclaringType != typeof(T))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.ValidateStructField<" + typeof(T).FullName + ", " + typeof(F).FullName + ">: FieldDeclaringType must be T (StructFieldRefAccess instance type)");
				}
				return false;
			}
			return true;
		}

		private static bool TryGetComponents(string typeColonName, out Type? type, out string? name, bool logErrorInTrace = true)
		{
			if (string.IsNullOrWhiteSpace(typeColonName))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.TryGetComponents: 'typeColonName' is null or whitespace/empty");
				}
				type = null;
				name = null;
				return false;
			}
			string[] array = typeColonName.Split(':');
			if (array.Length != 2)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.TryGetComponents: typeColonName '" + typeColonName + "', name must be specified as 'Namespace.Type1.Type2:Name");
				}
				type = null;
				name = null;
				return false;
			}
			type = TypeByName(array[0], logErrorInTrace);
			name = array[1];
			return (object)type != null;
		}
	}
	internal static class HarmonyExtensions
	{
		public static bool TryPatch(this Harmony harmony, MethodBase? original, MethodInfo? prefix = null, MethodInfo? postfix = null, MethodInfo? transpiler = null, MethodInfo? finalizer = null)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			if ((object)original == null || ((object)prefix == null && (object)postfix == null && (object)transpiler == null && (object)finalizer == null))
			{
				Trace.TraceError("HarmonyExtensions.TryPatch: 'original' or all methods are null");
				return false;
			}
			HarmonyMethod val = (((object)prefix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(prefix));
			HarmonyMethod val2 = (((object)postfix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(postfix));
			HarmonyMethod val3 = (((object)transpiler == null) ? ((HarmonyMethod)null) : new HarmonyMethod(transpiler));
			HarmonyMethod val4 = (((object)finalizer == null) ? ((HarmonyMethod)null) : new HarmonyMethod(finalizer));
			try
			{
				harmony.Patch(original, val, val2, val3, val4);
			}
			catch (Exception arg)
			{
				Trace.TraceError($"HarmonyExtensions.TryPatch: Exception occurred: {arg}, original '{original}'");
				return false;
			}
			return true;
		}

		public static ReversePatcher? TryCreateReversePatcher(this Harmony harmony, MethodBase? original, MethodInfo? standin)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			if ((object)original == null || (object)standin == null)
			{
				Trace.TraceError("HarmonyExtensions.TryCreateReversePatcher: 'original' or 'standin' is null");
				return null;
			}
			try
			{
				return harmony.CreateReversePatcher(original, new HarmonyMethod(standin));
			}
			catch (Exception arg)
			{
				Trace.TraceError($"HarmonyExtensions.TryCreateReversePatcher: Exception occurred: {arg}, original '{original}'");
				return null;
			}
		}

		public static bool TryCreateReversePatcher(this Harmony harmony, MethodBase? original, MethodInfo? standin, out ReversePatcher? result)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			if ((object)original == null || (object)standin == null)
			{
				Trace.TraceError("HarmonyExtensions.TryCreateReversePatcher: 'original' or 'standin' is null");
				result = null;
				return false;
			}
			try
			{
				result = harmony.CreateReversePatcher(original, new HarmonyMethod(standin));
				return true;
			}
			catch (Exception arg)
			{
				Trace.TraceError($"HarmonyExtensions.TryCreateReversePatcher: Exception occurred: {arg}, original '{original}'");
				result = null;
				return false;
			}
		}
	}
	internal static class SymbolExtensions2
	{
		public static ConstructorInfo? GetConstructorInfo<TResult>(Expression<Func<TResult>> expression)
		{
			if (expression != null)
			{
				return GetConstructorInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static ConstructorInfo? GetConstructorInfo<T1, TResult>(Expression<Func<T1, TResult>> expression)
		{
			if (expression != null)
			{
				return GetConstructorInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static ConstructorInfo? GetConstructorInfo<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> expression)
		{
			if (expression != null)
			{
				return GetConstructorInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static ConstructorInfo? GetConstructorInfo<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>> expression)
		{
			if (expression != null)
			{
				return GetConstructorInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static ConstructorInfo? GetConstructorInfo<T1, T2, T3, T4, TResult>(Expression<Func<T1, T2, T3, T4, TResult>> expression)
		{
			if (expression != null)
			{
				return GetConstructorInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static ConstructorInfo? GetConstructorInfo<T1, T2, T3, T4, T5, TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> expression)
		{
			if (expression != null)
			{
				return GetConstructorInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static ConstructorInfo? GetConstructorInfo<T1, T2, T3, T4, T5, T6, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expression)
		{
			if (expression != null)
			{
				return GetConstructorInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static ConstructorInfo? GetConstructorInfo<T1, T2, T3, T4, T5, T6, T7, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expression)
		{
			if (expression != null)
			{
				return GetConstructorInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static ConstructorInfo? GetConstructorInfo<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
		{
			if (expression != null)
			{
				return GetConstructorInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static ConstructorInfo? GetConstructorInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
		{
			if (expression != null)
			{
				return GetConstructorInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static ConstructorInfo? GetConstructorInfo(LambdaExpression expression)
		{
			if (!(expression?.Body is NewExpression { Constructor: not null, Constructor: var constructor }))
			{
				return null;
			}
			return constructor;
		}

		public static FieldInfo? GetFieldInfo<T>(Expression<Func<T>> expression)
		{
			if (expression != null)
			{
				return GetFieldInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static FieldInfo? GetFieldInfo<T, TResult>(Expression<Func<T, TResult>> expression)
		{
			if (expression != null)
			{
				return GetFieldInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static FieldInfo? GetFieldInfo(LambdaExpression expression)
		{
			if (expression?.Body is MemberExpression { Member: FieldInfo member })
			{
				return member;
			}
			return null;
		}

		public static FieldRef<object, TField>? GetFieldRefAccess<TField>(Expression<Func<TField>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetFieldRefAccess<TField>((LambdaExpression)expression);
			}
			return null;
		}

		public static FieldRef<object, TField>? GetFieldRefAccess<TField>(LambdaExpression expression)
		{
			if (expression?.Body is MemberExpression { Member: FieldInfo member })
			{
				return (member == null) ? null : AccessTools2.FieldRefAccess<object, TField>(member);
			}
			return null;
		}

		public static FieldRef<TObject, TField>? GetFieldRefAccess<TObject, TField>(Expression<Func<TObject, TField>> expression) where TObject : class
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetFieldRefAccess<TObject, TField>((LambdaExpression)expression);
			}
			return null;
		}

		public static FieldRef<TObject, TField>? GetFieldRefAccess<TObject, TField>(LambdaExpression expression) where TObject : class
		{
			if (expression?.Body is MemberExpression { Member: FieldInfo member })
			{
				return (member == null) ? null : AccessTools2.FieldRefAccess<TObject, TField>(member);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo(Expression<Action> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1>(Expression<Action<T1>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1, T2>(Expression<Action<T1, T2>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1, T2, T3>(Expression<Action<T1, T2, T3>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1, T2, T3, T4, T5>(Expression<Action<T1, T2, T3, T4, T5>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1, T2, T3, T4, T5, T6>(Expression<Action<T1, T2, T3, T4, T5, T6>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1, T2, T3, T4, T5, T6, T7>(Expression<Action<T1, T2, T3, T4, T5, T6, T7>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<TResult>(Expression<Func<TResult>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1, TResult>(Expression<Func<T1, TResult>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1, T2, T3, T4, TResult>(Expression<Func<T1, T2, T3, T4, TResult>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1, T2, T3, T4, T5, TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1, T2, T3, T4, T5, T6, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1, T2, T3, T4, T5, T6, T7, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
		{
			if (expression != null)
			{
				return GetMethodInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetMethodInfo(LambdaExpression expression)
		{
			if (expression?.Body is MethodCallExpression { Method: { } method })
			{
				return method;
			}
			return null;
		}

		public static PropertyInfo? GetPropertyInfo<T>(Expression<Func<T>> expression)
		{
			if (expression != null)
			{
				return GetPropertyInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static PropertyInfo? GetPropertyInfo<T, TResult>(Expression<Func<T, TResult>> expression)
		{
			if (expression != null)
			{
				return GetPropertyInfo((LambdaExpression)expression);
			}
			return null;
		}

		public static PropertyInfo? GetPropertyInfo(LambdaExpression expression)
		{
			if (expression?.Body is MemberExpression { Member: PropertyInfo member })
			{
				return member;
			}
			return null;
		}

		public static MethodInfo? GetPropertyGetter<T>(Expression<Func<T>> expression)
		{
			if (expression != null)
			{
				return GetPropertyGetter((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetPropertyGetter<T, TResult>(Expression<Func<T, TResult>> expression)
		{
			if (expression != null)
			{
				return GetPropertyGetter((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetPropertyGetter(LambdaExpression expression)
		{
			if (expression?.Body is MemberExpression { Member: PropertyInfo member })
			{
				return member?.GetGetMethod(nonPublic: true);
			}
			return null;
		}

		public static MethodInfo? GetPropertySetter<T>(Expression<Func<T>> expression)
		{
			if (expression != null)
			{
				return GetPropertySetter((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetPropertySetter<T, TResult>(Expression<Func<T, TResult>> expression)
		{
			if (expression != null)
			{
				return GetPropertySetter((LambdaExpression)expression);
			}
			return null;
		}

		public static MethodInfo? GetPropertySetter(LambdaExpression expression)
		{
			if (expression?.Body is MemberExpression { Member: PropertyInfo member })
			{
				return member?.GetSetMethod(nonPublic: true);
			}
			return null;
		}

		public static FieldRef<TField>? GetStaticFieldRefAccess<TField>(Expression<Func<TField>> expression)
		{
			if (expression != null)
			{
				return GetStaticFieldRefAccess<TField>((LambdaExpression)expression);
			}
			return null;
		}

		public static FieldRef<TField>? GetStaticFieldRefAccess<TField>(LambdaExpression expression)
		{
			if (expression?.Body is MemberExpression { Member: FieldInfo member })
			{
				return (member == null) ? null : AccessTools2.StaticFieldRefAccess<TField>(member);
			}
			return null;
		}

		public static StructFieldRef<TObject, TField>? GetStructFieldRefAccess<TObject, TField>(Expression<Func<TField>> expression) where TObject : struct
		{
			if (expression != null)
			{
				return GetStructFieldRefAccess<TObject, TField>((LambdaExpression)expression);
			}
			return null;
		}

		public static StructFieldRef<TObject, TField>? GetStructFieldRefAccess<TObject, TField>(LambdaExpression expression) where TObject : struct
		{
			if (expression?.Body is MemberExpression { Member: FieldInfo member })
			{
				return (member == null) ? null : AccessTools2.StructFieldRefAccess<TObject, TField>(member);
			}
			return null;
		}
	}
	internal class Traverse2<T>
	{
		private readonly Traverse2 _traverse;

		public T? Value
		{
			get
			{
				return _traverse.GetValue<T>();
			}
			set
			{
				_traverse.SetValue(value);
			}
		}

		private Traverse2()
		{
			_traverse = new Traverse2(null);
		}

		public Traverse2(Traverse2 traverse)
		{
			_traverse = traverse;
		}
	}
	internal class Traverse2
	{
		private static readonly AccessCacheHandle? Cache;

		private readonly Type? _type;

		private readonly object? _root;

		private readonly MemberInfo? _info;

		private readonly MethodBase? _method;

		private readonly object[]? _params;

		public static Action<Traverse2, Traverse2> CopyFields;

		[MethodImpl(MethodImplOptions.Synchronized)]
		static Traverse2()
		{
			CopyFields = delegate(Traverse2 from, Traverse2 to)
			{
				if (from != null)
				{
					to?.SetValue(from.GetValue());
				}
			};
			if (!Cache.HasValue)
			{
				Cache = AccessCacheHandle.Create();
			}
		}

		public static Traverse2 Create(Type? type)
		{
			return new Traverse2(type);
		}

		public static Traverse2 Create<T>()
		{
			return Create(typeof(T));
		}

		public static Traverse2 Create(object? root)
		{
			return new Traverse2(root);
		}

		public static Traverse2 CreateWithType(string name)
		{
			return new Traverse2(AccessTools2.TypeByName(name));
		}

		private Traverse2()
		{
		}

		public Traverse2(Type? type)
		{
			_type = type;
		}

		public Traverse2(object? root)
		{
			_root = root;
			_type = root?.GetType();
		}

		private Traverse2(object? root, MemberInfo info, object[]? index)
		{
			_root = root;
			_type = root?.GetType() ?? AccessTools.GetUnderlyingType(info);
			_info = info;
			_params = index;
		}

		private Traverse2(object? root, MethodInfo method, object[]? parameter)
		{
			_root = root;
			_type = method.ReturnType;
			_method = method;
			_params = parameter;
		}

		public object? GetValue()
		{
			if (_info is FieldInfo fieldInfo)
			{
				return fieldInfo.GetValue(_root);
			}
			if (_info is PropertyInfo propertyInfo)
			{
				return propertyInfo.GetValue(_root, AccessTools.all, null, _params, CultureInfo.CurrentCulture);
			}
			MethodBase method = _method;
			if ((object)method != null)
			{
				return method.Invoke(_root, _params);
			}
			if (_root == null && (object)_type != null)
			{
				return _type;
			}
			return _root;
		}

		public T? GetValue<T>()
		{
			if (GetValue() is T result)
			{
				return result;
			}
			return default(T);
		}

		public object? GetValue(params object[] arguments)
		{
			return _method?.Invoke(_root, arguments);
		}

		public T? GetValue<T>(params object[] arguments)
		{
			if (_method?.Invoke(_root, arguments) is T result)
			{
				return result;
			}
			return default(T);
		}

		public Traverse2 SetValue(object value)
		{
			if (_info is FieldInfo fieldInfo && ((_root == null && fieldInfo.IsStatic) || _root != null))
			{
				fieldInfo.SetValue(_root, value, AccessTools.all, null, CultureInfo.CurrentCulture);
			}
			if (_info is PropertyInfo { SetMethod: not null } propertyInfo && ((_root == null && propertyInfo.SetMethod.IsStatic) || _root != null))
			{
				propertyInfo.SetValue(_root, value, AccessTools.all, null, _params, CultureInfo.CurrentCulture);
			}
			return this;
		}

		public Type? GetValueType()
		{
			if (_info is FieldInfo { FieldType: var fieldType })
			{
				return fieldType;
			}
			if (!(_info is PropertyInfo { PropertyType: var propertyType }))
			{
				return null;
			}
			return propertyType;
		}

		private Traverse2 Resolve()
		{
			if (_root == null)
			{
				if (_info is FieldInfo { IsStatic: not false })
				{
					return new Traverse2(GetValue());
				}
				if (_info is PropertyInfo propertyInfo && propertyInfo.GetGetMethod().IsStatic)
				{
					return new Traverse2(GetValue());
				}
				MethodBase method = _method;
				if ((object)method != null && method.IsStatic)
				{
					return new Traverse2(GetValue());
				}
				if ((object)_type != null)
				{
					return this;
				}
			}
			return new Traverse2(GetValue());
		}

		public Traverse2 Type(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return new Traverse2();
			}
			if ((object)_type == null)
			{
				return new Traverse2();
			}
			Type type = AccessTools.Inner(_type, name);
			if ((object)type == null)
			{
				return new Traverse2();
			}
			return new Traverse2(type);
		}

		public Traverse2 Field(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return new Traverse2();
			}
			Traverse2 traverse = Resolve();
			if ((object)traverse._type == null)
			{
				return new Traverse2();
			}
			FieldInfo fieldInfo = Cache?.GetFieldInfo(traverse._type, name);
			if ((object)fieldInfo == null)
			{
				return new Traverse2();
			}
			if (!fieldInfo.IsStatic && traverse._root == null)
			{
				return new Traverse2();
			}
			return new Traverse2(traverse._root, fieldInfo, null);
		}

		public Traverse2<T> Field<T>(string name)
		{
			return new Traverse2<T>(Field(name));
		}

		public List<string> Fields()
		{
			Traverse2 traverse = Resolve();
			return AccessTools.GetFieldNames(traverse._type);
		}

		public Traverse2 Property(string name, object[]? index = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				return new Traverse2();
			}
			Traverse2 traverse = Resolve();
			if ((object)traverse._type == null)
			{
				return new Traverse2();
			}
			PropertyInfo propertyInfo = Cache?.GetPropertyInfo(traverse._type, name);
			if ((object)propertyInfo == null)
			{
				return new Traverse2();
			}
			return new Traverse2(traverse._root, propertyInfo, index);
		}

		public Traverse2<T> Property<T>(string name, object[]? index = null)
		{
			return new Traverse2<T>(Property(name, index));
		}

		public List<string> Properties()
		{
			Traverse2 traverse = Resolve();
			return AccessTools.GetPropertyNames(traverse._type);
		}

		public Traverse2 Method(string name, params object[] arguments)
		{
			if (string.IsNullOrEmpty(name))
			{
				return new Traverse2();
			}
			Traverse2 traverse = Resolve();
			if ((object)traverse._type == null)
			{
				return new Traverse2();
			}
			Type[] types = AccessTools.GetTypes(arguments);
			MethodBase methodBase = Cache?.GetMethodInfo(traverse._type, name, types);
			if (!(methodBase is MethodInfo method))
			{
				return new Traverse2();
			}
			return new Traverse2(traverse._root, method, arguments);
		}

		public Traverse2 Method(string name, Type[] paramTypes, object[]? arguments = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				return new Traverse2();
			}
			Traverse2 traverse = Resolve();
			if ((object)traverse._type == null)
			{
				return new Traverse2();
			}
			MethodBase methodBase = Cache?.GetMethodInfo(traverse._type, name, paramTypes);
			if (!(methodBase is MethodInfo method))
			{
				return new Traverse2();
			}
			return new Traverse2(traverse._root, method, arguments);
		}

		public List<string> Methods()
		{
			Traverse2 traverse = Resolve();
			return AccessTools.GetMethodNames(traverse._type);
		}

		public bool FieldExists()
		{
			return _info is FieldInfo;
		}

		public bool PropertyExists()
		{
			return _info is PropertyInfo;
		}

		public bool MethodExists()
		{
			return (object)_method != null;
		}

		public bool TypeExists()
		{
			return (object)_type != null;
		}

		public static void IterateFields(object source, Action<Traverse2> action)
		{
			Action<Traverse2> action2 = action;
			if (action2 != null)
			{
				Traverse2 sourceTrv = Create(source);
				AccessTools.GetFieldNames(source).ForEach(delegate(string f)
				{
					action2(sourceTrv.Field(f));
				});
			}
		}

		public static void IterateFields(object source, object target, Action<Traverse2, Traverse2> action)
		{
			Action<Traverse2, Traverse2> action2 = action;
			if (action2 != null)
			{
				Traverse2 sourceTrv = Create(source);
				Traverse2 targetTrv = Create(target);
				AccessTools.GetFieldNames(source).ForEach(delegate(string f)
				{
					action2(sourceTrv.Field(f), targetTrv.Field(f));
				});
			}
		}

		public static void IterateFields(object source, object target, Action<string, Traverse2, Traverse2> action)
		{
			Action<string, Traverse2, Traverse2> action2 = action;
			if (action2 != null)
			{
				Traverse2 sourceTrv = Create(source);
				Traverse2 targetTrv = Create(target);
				AccessTools.GetFieldNames(source).ForEach(delegate(string f)
				{
					action2(f, sourceTrv.Field(f), targetTrv.Field(f));
				});
			}
		}

		public static void IterateProperties(object source, Action<Traverse2> action)
		{
			Action<Traverse2> action2 = action;
			if (action2 != null)
			{
				Traverse2 sourceTrv = Create(source);
				AccessTools.GetPropertyNames(source).ForEach(delegate(string f)
				{
					action2(sourceTrv.Property(f));
				});
			}
		}

		public static void IterateProperties(object source, object target, Action<Traverse2, Traverse2> action)
		{
			Action<Traverse2, Traverse2> action2 = action;
			if (action2 != null)
			{
				Traverse2 sourceTrv = Create(source);
				Traverse2 targetTrv = Create(target);
				AccessTools.GetPropertyNames(source).ForEach(delegate(string f)
				{
					action2(sourceTrv.Property(f), targetTrv.Property(f));
				});
			}
		}

		public static void IterateProperties(object source, object target, Action<string, Traverse2, Traverse2> action)
		{
			Action<string, Traverse2, Traverse2> action2 = action;
			if (action2 != null)
			{
				Traverse2 sourceTrv = Create(source);
				Traverse2 targetTrv = Create(target);
				AccessTools.GetPropertyNames(source).ForEach(delegate(string f)
				{
					action2(f, sourceTrv.Property(f), targetTrv.Property(f));
				});
			}
		}

		public override string? ToString()
		{
			return (_method ?? GetValue())?.ToString();
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
namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class AllowNullAttribute : Attribute
	{
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class DisallowNullAttribute : Attribute
	{
	}
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class DoesNotReturnAttribute : Attribute
	{
	}
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class DoesNotReturnIfAttribute : Attribute
	{
		public bool ParameterValue { get; }

		public DoesNotReturnIfAttribute(bool parameterValue)
		{
			ParameterValue = parameterValue;
		}
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class MaybeNullAttribute : Attribute
	{
	}
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class MaybeNullWhenAttribute : Attribute
	{
		public bool ReturnValue { get; }

		public MaybeNullWhenAttribute(bool returnValue)
		{
			ReturnValue = returnValue;
		}
	}
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class MemberNotNullAttribute : Attribute
	{
		public string[] Members { get; }

		public MemberNotNullAttribute(string member)
		{
			Members = new string[1] { member };
		}

		public MemberNotNullAttribute(params string[] members)
		{
			Members = members;
		}
	}
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class MemberNotNullWhenAttribute : Attribute
	{
		public bool ReturnValue { get; }

		public string[] Members { get; }

		public MemberNotNullWhenAttribute(bool returnValue, string member)
		{
			ReturnValue = returnValue;
			Members = new string[1] { member };
		}

		public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
		{
			ReturnValue = returnValue;
			Members = members;
		}
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class NotNullAttribute : Attribute
	{
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class NotNullIfNotNullAttribute : Attribute
	{
		public string ParameterName { get; }

		public NotNullIfNotNullAttribute(string parameterName)
		{
			ParameterName = parameterName;
		}
	}
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class NotNullWhenAttribute : Attribute
	{
		public bool ReturnValue { get; }

		public NotNullWhenAttribute(bool returnValue)
		{
			ReturnValue = returnValue;
		}
	}
}
