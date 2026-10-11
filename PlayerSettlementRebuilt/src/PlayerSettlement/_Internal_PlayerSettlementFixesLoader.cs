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
using SiegeEnginesContainer = TaleWorlds.CampaignSystem.Siege.SiegeEvent.SiegeEnginesContainer;
using SiegeEngineConstructionProgress = TaleWorlds.CampaignSystem.Siege.SiegeEvent.SiegeEngineConstructionProgress;
using Debug = TaleWorlds.Library.Debug;
using Path = System.IO.Path;
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
				float x = (position).X;
				position = settlement.Position;
				float y = (position).Y;
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
					string text = ((val != null) ? val.StringId : null);
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
					PlayerSettlementFixesLoader.LogError("IssuesCampaignBehaviorClanNoblesFix: suppressed NRE for clan " + ((val != null) ? val.StringId : null));
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
