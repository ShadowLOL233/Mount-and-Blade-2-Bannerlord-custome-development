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
using Debug = TaleWorlds.Library.Debug;
using NavigationType = TaleWorlds.CampaignSystem.Party.MobileParty.NavigationType;
using CraftingOrderSlots = TaleWorlds.CampaignSystem.CampaignBehaviors.CraftingCampaignBehavior.CraftingOrderSlots;
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

		public static void SetCampaignGameLoadingType(this Campaign instance, Campaign.GameLoadingType gameLoadingType)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			FieldInfo fieldInfo = AccessTools.Field(typeof(Campaign), "_gameLoadingType");
			fieldInfo.SetValue(instance, gameLoadingType);
		}

		public static T AsCampaignGameLoadingType<T>(this Campaign instance, Campaign.GameLoadingType gameLoadingType, Func<T> func)
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

		public static bool AddTown(this CraftingCampaignBehavior craftingCampaignBehavior, Town town, out Dictionary<Town, CraftingCampaignBehavior.CraftingOrderSlots>? _craftingOrders)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			_craftingOrders = null;
			try
			{
				_craftingOrders = _craftingOrdersField.GetValue(craftingCampaignBehavior) as Dictionary<Town, CraftingCampaignBehavior.CraftingOrderSlots>;
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
							craftingCampaignBehavior.CreateTownOrder(TaleWorlds.Core.Extensions.GetRandomElement<Hero>(val), i);
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
			GameKey val = (GameKey)hotKeyBase;
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
