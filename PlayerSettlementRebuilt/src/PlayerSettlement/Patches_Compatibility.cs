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
using NavigationType = TaleWorlds.CampaignSystem.Party.MobileParty.NavigationType;
using ChangeOwnerOfSettlementDetail = TaleWorlds.CampaignSystem.Actions.ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail;
using Debug = TaleWorlds.Library.Debug;
using ShipType = TaleWorlds.Core.ShipHull.ShipType;
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
					MBList<ShipHull> val = TaleWorlds.Library.Extensions.ToMBList<ShipHull>(((IEnumerable<Kingdom>)Kingdom.All).SelectMany((Kingdom x) => (IEnumerable<ShipHull>)x.Culture.AvailableShipHulls));
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
							TaleWorlds.Core.Extensions.Shuffle<ShipUpgradePiece>((IList<ShipUpgradePiece>)availableShipUpgradePieces);
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
					int num2 = (int)(val4).ResultNumber;
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
					LogManager.Log.NotifyBad($"Failed for from: {fromSettlement} (HasPort: {fromSettlement.HasPort}, isFromPort: {isFromPort}), to: {toSettlement} (HasPort: {toSettlement.HasPort}, isTargetingPort: {isTargetingPort}), navigationCapability: {(int)navigationCapability} - {((object)navigationCapability).ToString()}, __result: {__result}");
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

		// decomp-fix: original ternary used `isinst` with value type · ILSpy failed to emit true-branch
		// Reconstructed: when SettlementShipyardProp is a static PropertyInfo returning BuildingType
		internal static BuildingType SettlementShipyard => (BuildingType)SettlementShipyardProp?.GetValue(null);

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
