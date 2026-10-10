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
					val2.z = ____scene.GetGroundHeightAtPosition((position).ToVec3(0f), (BodyFlags)544321929);
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
			if (((ScreenLayer)__instance.SceneLayer).Input.GetIsMouseActive() && PlayerSettlementBehaviour.Instance != null && PlayerSettlementBehaviour.Instance.IsPlacingPort && !intersectionPoint.IsOnLand && (mouseOverFaceIndex).IsValid() && PlayerSettlementBehaviour.Instance.PlacementSupported)
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
					string[] tags = (gameEntity).Tags;
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
							if (!(gameEntity).HasTag("hold_tag_always"))
							{
								foreach (UsableMachine item4 in usableMachinesInRange)
								{
									gameEntity = ((ScriptComponentBehavior)item4).GameEntity;
									if (!(gameEntity).HasTag(tag))
									{
										gameEntity = ((ScriptComponentBehavior)item4).GameEntity;
										(gameEntity).AddTag(tag);
									}
								}
							}
							else
							{
								gameEntity = ((ScriptComponentBehavior)item3).GameEntity;
								string text2 = (gameEntity).Tags[0] + "_" + item2.Tag;
								gameEntity = ((ScriptComponentBehavior)item3).GameEntity;
								(gameEntity).AddTag(text2);
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
							string key2 = (gameEntity).Tags[0];
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
						if ((globalFrame).NearlyEquals(engineFrame, 1E-05f))
						{
							if (____hoveredSiegeEntityFrame != globalFrame)
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
						if ((globalFrame2).NearlyEquals(engineFrame, 1E-05f))
						{
							if (____hoveredSiegeEntityFrame != globalFrame2)
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
						if ((globalFrame3).NearlyEquals(engineFrame, 1E-05f))
						{
							if (____hoveredSiegeEntityFrame != globalFrame3)
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
						if ((globalFrame4).NearlyEquals(engineFrame, 1E-05f))
						{
							if (____hoveredSiegeEntityFrame != globalFrame4)
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
						if ((globalFrame5).NearlyEquals(engineFrame, 1E-05f))
						{
							if (____hoveredSiegeEntityFrame != globalFrame5 && ((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.IsSettlement)
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
							NavigationHelper.IsPositionValidForNavigationType(new CampaignVec2((globalFrame.origin).AsVec2, true), (NavigationType)1);
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
								NavigationHelper.IsPositionValidForNavigationType(new CampaignVec2((globalFrame2.origin).AsVec2, false), (NavigationType)2);
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
						(rotation).ApplyScaleLocal(1.75f);
					}
					else if (((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement.IsTown)
					{
						(rotation).ApplyScaleLocal(5.75f);
					}
					else if (!((MapEntityVisual<PartyBase>)(object)__instance).MapEntity.Settlement.IsCastle)
					{
						(rotation).ApplyScaleLocal(1.75f);
					}
					else
					{
						(rotation).ApplyScaleLocal(2.75f);
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
					if ((buildEnd).IsFuture)
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
					if ((buildEnd).IsFuture)
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
					if ((buildEnd).IsFuture)
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
					if ((buildEnd).IsFuture)
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
					if ((buildEnd).IsFuture)
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
					if ((buildEnd).IsFuture)
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
							isHovered = (globalFrame).NearlyEquals(tuple.Item1, 1E-05f);
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
							isHovered2 = (globalFrame).NearlyEquals(tuple.Item1, 1E-05f);
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
							isHovered3 = (globalFrame).NearlyEquals(tuple.Item1, 1E-05f);
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
							isHovered4 = (globalFrame).NearlyEquals(tuple.Item1, 1E-05f);
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
