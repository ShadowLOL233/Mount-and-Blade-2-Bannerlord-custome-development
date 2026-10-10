using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;


namespace PlayerSettlementFixes;

public sealed class PlayerSettlementFixesSubModule : MBSubModuleBase
{
	private static Harmony _harmony;

	protected override void OnSubModuleLoad()
	{
		((MBSubModuleBase)this).OnSubModuleLoad();
		_harmony = _harmony ?? new Harmony("com.b0tlanner.bannerlord.playersettlement.fixes");
		PlayerSettlementFixesLoader.Initialize(_harmony);
	}
}
public static class PlayerSettlementFixesLoader
{
	private static Harmony _harmony;

	private static bool _initialized;

	public static void Initialize(Harmony harmony)
	{
		if (_initialized)
		{
			return;
		}
		_initialized = true;
		_harmony = harmony;
		int num = 0;
		int num2 = 0;
		Type[] typesFromAssembly = AccessTools.GetTypesFromAssembly(typeof(PlayerSettlementFixesLoader).Assembly);
		foreach (Type type in typesFromAssembly)
		{
			try
			{
				List<MethodInfo> list = harmony.CreateClassProcessor(type).Patch();
				if (list != null && list.Count > 0)
				{
					num++;
				}
			}
			catch (Exception ex)
			{
				num2++;
				LogError("Skipped incompatible patch '" + type.FullName + "': " + ex.Message);
			}
		}
		try
		{
			SiegeEnginesContainerRuntimeFix.Apply(harmony);
		}
		catch (Exception ex2)
		{
			LogError("SiegeEnginesContainerRuntimeFix.Apply: " + ex2.Message);
		}
		try
		{
			int num3 = RemoveStaleMergedPatches(harmony);
			if (num3 > 0)
			{
				LogError($"PlayerSettlementFixes: removed {num3} stale patch(es) from the old fixes copy merged inside PlayerSettlement.dll (superseded by this DLL; also silences the legacy SIEGE_DIAG log spam).");
			}
		}
		catch (Exception ex3)
		{
			LogError("RemoveStaleMergedPatches: " + ex3.Message);
		}
		LogError($"PlayerSettlementFixes initialized: {num} patch class(es) applied, {num2} skipped.");
	}

	private static int RemoveStaleMergedPatches(Harmony harmony)
	{
		int num = 0;
		Assembly assembly = typeof(PlayerSettlementFixesLoader).Assembly;
		foreach (MethodBase item in Harmony.GetAllPatchedMethods().ToList())
		{
			Patches patchInfo = Harmony.GetPatchInfo(item);
			if (patchInfo == null)
			{
				continue;
			}
			List<Patch> list = new List<Patch>();
			if (patchInfo.Prefixes != null)
			{
				list.AddRange(patchInfo.Prefixes);
			}
			if (patchInfo.Postfixes != null)
			{
				list.AddRange(patchInfo.Postfixes);
			}
			if (patchInfo.Finalizers != null)
			{
				list.AddRange(patchInfo.Finalizers);
			}
			if (patchInfo.Transpilers != null)
			{
				list.AddRange(patchInfo.Transpilers);
			}
			foreach (Patch item2 in list)
			{
				try
				{
					MethodInfo patchMethod = item2.PatchMethod;
					Type type = patchMethod?.DeclaringType;
					if (!(type == null) && type.Namespace == "PlayerSettlementFixes" && type.Assembly != assembly)
					{
						harmony.Unpatch(item, patchMethod);
						num++;
					}
				}
				catch (Exception ex)
				{
					LogError("RemoveStaleMergedPatches (single): " + ex.Message);
				}
			}
		}
		return num;
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
		}
		catch
		{
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
		string[] array = new string[3] { "Towns", "Castles", "OverwriteSettlements" };
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
					PlayerBuiltFortificationAiFix.EnsureFortificationInFactionCaches(val);
					EnsureCampPositions(val);
				}
			}
		}
	}

	internal static void EnsureCampPositions(Settlement settlement)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Town town = settlement.Town;
		if (town == null)
		{
			return;
		}
		bool flag = HasUsableCampPositions(town.BesiegerCampPositions1);
		bool flag2 = HasUsableCampPositions(town.BesiegerCampPositions2);
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

	private static bool HasUsableCampPositions(MatrixFrame[] positions)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (positions == null || positions.Length == 0)
		{
			return false;
		}
		foreach (MatrixFrame val in positions)
		{
			if (Math.Abs(val.origin.x) > 0.001f || Math.Abs(val.origin.y) > 0.001f)
			{
				return true;
			}
		}
		return false;
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
public static class BesiegerCampPositionsOnBuildCompleteFix
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method("BannerlordPlayerSettlement.Behaviours.PlayerSettlementBehaviour:NotifyComplete");
	}

	private static void Postfix(object item)
	{
		try
		{
			object obj = item?.GetType().GetMethod("GetSettlement")?.Invoke(item, null);
			Settlement val = (Settlement)((obj is Settlement) ? obj : null);
			if (val != null && val.IsFortification)
			{
				PlayerBuiltFortificationAiFix.EnsureFortificationInFactionCaches(val);
				BesiegerCampPositionsFallbackFix.EnsureCampPositions(val);
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("BesiegerCampPositionsOnBuildCompleteFix: " + ex.Message);
		}
	}
}
[HarmonyPatch]
public static class BesiegerCampPositionsAfterVisualStartupFix
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method("BannerlordPlayerSettlement.Patches.PartyVisualPatch:OnStartup");
	}

	private static void Postfix()
	{
		try
		{
			BesiegerCampPositionsFallbackFix.EnsureAllPlayerSettlementsCampPositions();
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("BesiegerCampPositionsAfterVisualStartupFix: " + ex.Message);
		}
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
public static class PlayerBuiltFortificationAiFix
{
	[CompilerGenerated]
	private sealed class _GetPlayerBuiltFortificationsInternal_d__14 : IEnumerable<Settlement>, IEnumerable, IEnumerator<Settlement>, IDisposable, IEnumerator
	{
		private int __1__state;

		private Settlement __2__current;

		private int __l__initialThreadId;

		private bool includeOverwrites;

		public bool __3__includeOverwrites;

		private Type _infoType_5__2;

		private object _info_5__3;

		private HashSet<Settlement> _seen_5__4;

		private string[] __7__wrap4;

		private int __7__wrap5;

		private IEnumerator __7__wrap6;

		Settlement IEnumerator<Settlement>.Current
		{
			[DebuggerHidden]
			get
			{
				return __2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return __2__current;
			}
		}

		[DebuggerHidden]
		public _GetPlayerBuiltFortificationsInternal_d__14(int __1__state)
		{
			this.__1__state = __1__state;
			__l__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = __1__state;
			if (num == -3 || num == 1)
			{
				try
				{
				}
				finally
				{
					__m__Finally1();
				}
			}
			_infoType_5__2 = null;
			_info_5__3 = null;
			_seen_5__4 = null;
			__7__wrap4 = null;
			__7__wrap6 = null;
			__1__state = -2;
		}

		private bool MoveNext()
		{
			try
			{
				int num = __1__state;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					__1__state = -3;
					goto IL_0190;
				}
				__1__state = -1;
				_infoType_5__2 = AccessTools.TypeByName("BannerlordPlayerSettlement.Saves.PlayerSettlementInfo");
				_info_5__3 = (_infoType_5__2?.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public))?.GetValue(null);
				if (_info_5__3 == null)
				{
					return false;
				}
				string[] array = ((!includeOverwrites) ? new string[2] { "Towns", "Castles" } : new string[3] { "Towns", "Castles", "OverwriteSettlements" });
				_seen_5__4 = new HashSet<Settlement>();
				__7__wrap4 = array;
				__7__wrap5 = 0;
				goto IL_01b8;
				IL_0190:
				while (__7__wrap6.MoveNext())
				{
					object current = __7__wrap6.Current;
					object obj = (current?.GetType().GetField("Settlement", BindingFlags.Instance | BindingFlags.Public))?.GetValue(current);
					Settlement val = (Settlement)((obj is Settlement) ? obj : null);
					if (val != null && val.IsFortification && _seen_5__4.Add(val))
					{
						__2__current = val;
						__1__state = 1;
						return true;
					}
				}
				__m__Finally1();
				__7__wrap6 = null;
				goto IL_01aa;
				IL_01aa:
				__7__wrap5++;
				goto IL_01b8;
				IL_01b8:
				if (__7__wrap5 < __7__wrap4.Length)
				{
					string name = __7__wrap4[__7__wrap5];
					if (_infoType_5__2.GetField(name, BindingFlags.Instance | BindingFlags.Public)?.GetValue(_info_5__3) is IList list)
					{
						__7__wrap6 = list.GetEnumerator();
						__1__state = -3;
						goto IL_0190;
					}
					goto IL_01aa;
				}
				__7__wrap4 = null;
				return false;
			}
			catch
			{
				//try-fault
				((IDisposable)this).Dispose();
				throw;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void __m__Finally1()
		{
			__1__state = -1;
			if (__7__wrap6 is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<Settlement> IEnumerable<Settlement>.GetEnumerator()
		{
			_GetPlayerBuiltFortificationsInternal_d__14 _GetPlayerBuiltFortificationsInternal_d__;
			if (__1__state == -2 && __l__initialThreadId == Environment.CurrentManagedThreadId)
			{
				__1__state = 0;
				_GetPlayerBuiltFortificationsInternal_d__ = this;
			}
			else
			{
				_GetPlayerBuiltFortificationsInternal_d__ = new _GetPlayerBuiltFortificationsInternal_d__14(0);
			}
			_GetPlayerBuiltFortificationsInternal_d__.includeOverwrites = __3__includeOverwrites;
			return _GetPlayerBuiltFortificationsInternal_d__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<Settlement>)this).GetEnumerator();
		}
	}

	private static readonly MethodInfo ClanOnFortificationAdded = AccessTools.Method(typeof(Clan), "OnFortificationAdded");

	private static readonly MethodInfo KingdomOnFortificationAdded = AccessTools.Method(typeof(Kingdom), "OnFortificationAdded");

	private static readonly FieldInfo ClanFiefsCache = AccessTools.Field(typeof(Clan), "_fiefsCache");

	private static readonly FieldInfo ClanSettlementsCache = AccessTools.Field(typeof(Clan), "_settlementsCache");

	private static readonly FieldInfo KingdomFiefsCache = AccessTools.Field(typeof(Kingdom), "_fiefsCache");

	private static readonly FieldInfo KingdomSettlementsCache = AccessTools.Field(typeof(Kingdom), "_settlementsCache");

	private static MethodInfo _avgClosestTownDistanceMethod;

	private static bool _avgClosestTownDistanceUnavailable;

	private const float FallbackMilitiaUnitPower = 0.7f;

	private static MethodBase TargetMethod()
	{
		return AccessTools.Method(typeof(AiMilitaryBehavior), "FindBestTargetAndItsValueForFaction");
	}

	private static void Postfix(ArmyTypes missionType, PartyThinkParams p, float ourStrength)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if ((int)missionType == 0 && p != null)
			{
				TryAddPlayerBuiltSiegeScores(p.MobilePartyOf, p, ourStrength, p.WillGatherAnArmy);
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("PlayerBuiltFortificationAiFix target postfix: " + ex.Message);
		}
	}

	internal static void EnsureFortificationInFactionCaches(Settlement settlement)
	{
		try
		{
			if (settlement == null || !settlement.IsFortification || settlement.Town == null)
			{
				return;
			}
			Clan ownerClan = settlement.OwnerClan;
			if (ownerClan == null)
			{
				return;
			}
			bool flag = ContainsReference(ClanFiefsCache?.GetValue(ownerClan), settlement.Town);
			NormalizeCacheList(ClanFiefsCache?.GetValue(ownerClan), settlement.Town, "owner clan fiefs");
			NormalizeCacheList(ClanSettlementsCache?.GetValue(ownerClan), settlement, "owner clan settlements");
			if (!((List<Town>)(object)ownerClan.Fiefs).Contains(settlement.Town))
			{
				if (!flag)
				{
					ClanOnFortificationAdded?.Invoke(ownerClan, new object[1] { settlement.Town });
				}
				else
				{
					AddToCacheList(ClanFiefsCache?.GetValue(ownerClan), settlement.Town);
				}
			}
			else if (!((List<Settlement>)(object)ownerClan.Settlements).Contains(settlement))
			{
				AddToCacheList(ClanSettlementsCache?.GetValue(ownerClan), settlement);
			}
			Kingdom kingdom = ownerClan.Kingdom;
			bool flag2 = kingdom != null && ContainsReference(KingdomFiefsCache?.GetValue(kingdom), settlement.Town);
			if (kingdom != null)
			{
				NormalizeCacheList(KingdomFiefsCache?.GetValue(kingdom), settlement.Town, "kingdom fiefs");
				NormalizeCacheList(KingdomSettlementsCache?.GetValue(kingdom), settlement, "kingdom settlements");
			}
			if (kingdom != null && !((List<Town>)(object)kingdom.Fiefs).Contains(settlement.Town))
			{
				if (!flag2)
				{
					KingdomOnFortificationAdded?.Invoke(kingdom, new object[1] { settlement.Town });
				}
				else
				{
					AddToCacheList(KingdomFiefsCache?.GetValue(kingdom), settlement.Town);
				}
			}
			else if (kingdom != null && !((List<Settlement>)(object)kingdom.Settlements).Contains(settlement))
			{
				AddToCacheList(KingdomSettlementsCache?.GetValue(kingdom), settlement);
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("EnsureFortificationInFactionCaches: " + ex.Message);
		}
	}

	private static void NormalizeCacheList(object cacheList, object keepItem, string cacheName)
	{
		try
		{
			if (cacheList == null || keepItem == null)
			{
				return;
			}
			List<int> list = new List<int>();
			int num = 0;
			foreach (object item in (IEnumerable)cacheList)
			{
				if (item == keepItem || object.Equals(item, keepItem))
				{
					list.Add(num);
				}
				num++;
			}
			for (int num2 = list.Count - 1; num2 >= 1; num2--)
			{
				cacheList.GetType().GetMethod("RemoveAt")?.Invoke(cacheList, new object[1] { list[num2] });
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("NormalizeCacheList: " + ex.Message);
		}
	}

	private static bool ContainsReference(object cacheList, object target)
	{
		try
		{
			if (cacheList == null || target == null)
			{
				return false;
			}
			foreach (object item in (IEnumerable)cacheList)
			{
				if (item == target || object.Equals(item, target))
				{
					return true;
				}
			}
		}
		catch
		{
		}
		return false;
	}

	private static void AddToCacheList(object cacheList, object item)
	{
		try
		{
			if (cacheList != null && item != null && !ContainsReference(cacheList, item))
			{
				cacheList.GetType().GetMethod("Add")?.Invoke(cacheList, new object[1] { item });
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("AddToCacheList: " + ex.Message);
		}
	}

	internal static IEnumerable<Settlement> GetPlayerBuiltFortifications()
	{
		return GetPlayerBuiltFortificationsInternal(includeOverwrites: true);
	}

	internal static IEnumerable<Settlement> GetNewlyCreatedPlayerBuiltFortifications()
	{
		return GetPlayerBuiltFortificationsInternal(includeOverwrites: false);
	}

	[IteratorStateMachine(typeof(_GetPlayerBuiltFortificationsInternal_d__14))]
	private static IEnumerable<Settlement> GetPlayerBuiltFortificationsInternal(bool includeOverwrites)
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _GetPlayerBuiltFortificationsInternal_d__14(-2)
		{
			__3__includeOverwrites = includeOverwrites
		};
	}

	internal static void TryAddPlayerBuiltSiegeScores(MobileParty mobileParty, PartyThinkParams p, float ourStrength, bool willGatherArmy, float scoreMultiplier = 1f)
	{
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if (!IsUsableEnemyWarParty(mobileParty) || p == null)
		{
			return;
		}
		NavigationType val = default(NavigationType);
		float num = default(float);
		bool flag = default(bool);
		AIBehaviorData item = default(AIBehaviorData);
		float num5 = default(float);
		foreach (Settlement newlyCreatedPlayerBuiltFortification in GetNewlyCreatedPlayerBuiltFortifications())
		{
			EnsureFortificationInFactionCaches(newlyCreatedPlayerBuiltFortification);
			if (newlyCreatedPlayerBuiltFortification.OwnerClan != Clan.PlayerClan || !IsValidSiegeTargetForParty(mobileParty, newlyCreatedPlayerBuiltFortification))
			{
				continue;
			}
			AiHelper.GetBestNavigationTypeAndAdjustedDistanceOfSettlementForMobileParty(mobileParty, newlyCreatedPlayerBuiltFortification, false, ref val, ref num, ref flag);
			if ((int)val == 0 || num == float.MaxValue)
			{
				continue;
			}
			float maxSiegeReach = GetMaxSiegeReach(mobileParty);
			if (num > maxSiegeReach)
			{
				continue;
			}
			float num2 = EstimateDefenderStrength(newlyCreatedPlayerBuiltFortification);
			float num3 = Math.Max(ourStrength, mobileParty.GetTotalLandStrengthWithFollowers(true));
			float num4 = (willGatherArmy ? 1.15f : 1.35f);
			if (num3 < Math.Max(80f, num2 * num4))
			{
				continue;
			}
			(item)..ctor((IMapPoint)(object)newlyCreatedPlayerBuiltFortification, (AiBehavior)5, val, willGatherArmy, flag, false);
			if (!p.TryGetBehaviorScore(ref item, ref num5) || !(num5 > 0f))
			{
				float num6 = Math.Max(0.1f, 1f - num / Math.Max(1f, maxSiegeReach));
				float num7 = 0f;
				try
				{
					num7 = Campaign.Current.Models.TargetScoreCalculatingModel.GetTargetScoreForFaction(newlyCreatedPlayerBuiltFortification, (ArmyTypes)0, mobileParty, num3);
				}
				catch
				{
					num7 = 0f;
				}
				if (num7 <= 0f)
				{
					float num8 = Math.Min(2f, num3 / Math.Max(80f, num2));
					num7 = 3f * num8;
				}
				float num9 = num7 * num6 * Math.Max(0.5f, scoreMultiplier);
				if (!(num9 <= 0f))
				{
					(AIBehaviorData, float) tuple = (item, num9);
					p.AddBehaviorScore(ref tuple);
				}
			}
		}
	}

	private static float GetMaxSiegeReach(MobileParty mobileParty)
	{
		try
		{
			if (!_avgClosestTownDistanceUnavailable && _avgClosestTownDistanceMethod == null)
			{
				_avgClosestTownDistanceMethod = AccessTools.Method(typeof(Campaign), "GetAverageDistanceBetweenClosestTwoTownsWithNavigationType");
				if (_avgClosestTownDistanceMethod == null)
				{
					_avgClosestTownDistanceUnavailable = true;
				}
			}
			if (_avgClosestTownDistanceMethod != null)
			{
				object obj = (object)(NavigationType)1;
				PropertyInfo propertyInfo = AccessTools.Property(typeof(MobileParty), "NavigationCapability");
				if (propertyInfo != null)
				{
					obj = propertyInfo.GetValue(mobileParty);
				}
				float num = Convert.ToSingle(_avgClosestTownDistanceMethod.Invoke(Campaign.Current, new object[1] { obj }));
				if (num > 0f && !float.IsNaN(num) && !float.IsInfinity(num))
				{
					return num * 3f;
				}
			}
		}
		catch
		{
		}
		return Campaign.MapDiagonal * 0.25f;
	}

	private static bool IsUsableEnemyWarParty(MobileParty mobileParty)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Invalid comparison between Unknown and I4
		if (mobileParty != null && !mobileParty.IsMilitia && !mobileParty.IsCaravan && !mobileParty.IsVillager && !mobileParty.IsBandit && !mobileParty.IsPatrolParty && !mobileParty.IsDisbanding && mobileParty.LeaderHero != null && mobileParty.MapFaction != null && mobileParty.MapFaction.IsKingdomFaction)
		{
			Settlement currentSettlement = mobileParty.CurrentSettlement;
			if (((currentSettlement != null) ? currentSettlement.SiegeEvent : null) == null)
			{
				if (mobileParty.Army != null && mobileParty.Army.LeaderParty != mobileParty)
				{
					return false;
				}
				if ((int)mobileParty.Objective == 1)
				{
					return false;
				}
				return true;
			}
		}
		return false;
	}

	private static bool IsValidSiegeTargetForParty(MobileParty mobileParty, Settlement settlement)
	{
		if (settlement != null && settlement.IsFortification && settlement.Town != null && settlement.MapFaction != null && settlement.MapFaction != mobileParty.MapFaction)
		{
			PartyBase party = settlement.Party;
			if (((party != null) ? party.MapEvent : null) == null)
			{
				SiegeEvent siegeEvent = settlement.SiegeEvent;
				if (siegeEvent != null)
				{
					BesiegerCamp besiegerCamp = siegeEvent.BesiegerCamp;
					IFaction val = ((besiegerCamp != null) ? besiegerCamp.MapFaction : null);
					if (val == null || val != mobileParty.MapFaction)
					{
						return false;
					}
				}
				return FactionManager.IsAtWarAgainstFaction(mobileParty.MapFaction, settlement.MapFaction);
			}
		}
		return false;
	}

	private static float EstimateDefenderStrength(Settlement settlement)
	{
		float num = 0f;
		try
		{
			Town town = settlement.Town;
			object obj;
			if (town == null)
			{
				obj = null;
			}
			else
			{
				MobileParty garrisonParty = ((Fief)town).GarrisonParty;
				obj = ((garrisonParty != null) ? garrisonParty.Party : null);
			}
			if (obj != null)
			{
				num += ((Fief)settlement.Town).GarrisonParty.Party.EstimatedStrength;
			}
			CultureObject culture = settlement.Culture;
			CharacterObject val = ((culture != null) ? culture.MeleeMilitiaTroop : null);
			float num2 = ((val != null) ? Campaign.Current.Models.MilitaryPowerModel.GetDefaultTroopPower(val) : 0.7f);
			num += Math.Max(0f, settlement.Militia) * num2;
			foreach (MobileParty item in (List<MobileParty>)(object)settlement.Parties)
			{
				if (((item != null) ? item.Party : null) != null)
				{
					Town town2 = settlement.Town;
					if (item != ((town2 != null) ? ((Fief)town2).GarrisonParty : null))
					{
						num += item.Party.EstimatedStrength;
					}
				}
			}
		}
		catch
		{
			num += Math.Max(0f, settlement.Militia) * 0.7f;
		}
		return Math.Max(80f, num);
	}
}
[HarmonyPatch(typeof(AiMilitaryBehavior), "AiHourlyTick")]
public static class PlayerBuiltFortificationHourlyAiFix
{
	private static void Postfix(MobileParty mobileParty, PartyThinkParams p)
	{
		try
		{
			if (mobileParty == null || p == null)
			{
				return;
			}
			PlayerBuiltFortificationAiFix.TryAddPlayerBuiltSiegeScores(mobileParty, p, mobileParty.GetTotalLandStrengthWithFollowers(true), willGatherArmy: false);
			try
			{
				Settlement val = mobileParty.BesiegedSettlement;
				if (val == null && mobileParty.MapEvent != null && mobileParty.MapEvent.IsSiegeAssault)
				{
					val = mobileParty.MapEvent.MapEventSettlement;
				}
				if (val != null && PlayerBuiltSiegeMachinePlacementFix.IsPlayerSettlement(val))
				{
					p.DoNotChangeBehavior = true;
				}
			}
			catch
			{
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("PlayerBuiltFortificationHourlyAiFix: " + ex.Message);
		}
	}
}
[HarmonyPatch]
public static class PlayerBuiltSiegeMachinePlacementFix
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method("SandBox.View.Map.Visuals.SettlementVisual:AddSiegeMachine");
	}

	private static void Prefix(object __instance, ref MatrixFrame globalFrame, BattleSideEnum side, int slotIndex)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (IsPlayerSettlement(PlayerSiege.PlayerSiegeEvent?.BesiegedSettlement))
			{
				SnapFrameToMapTerrain(ref globalFrame);
				Mat3 rotation = globalFrame.rotation;
				(rotation).OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				globalFrame.rotation = rotation;
				if (slotIndex < 0)
				{
					PlayerSettlementFixesLoader.LogError($"PlayerBuiltSiegeMachinePlacementFix: corrected invalid slot {slotIndex} side={side}.");
				}
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("PlayerBuiltSiegeMachinePlacementFix prefix: " + ex.Message);
		}
	}

	internal static bool IsPlayerSettlement(Settlement settlement)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (settlement == null)
		{
			return false;
		}
		string stringId = ((MBObjectBase)settlement).StringId;
		if (stringId != null && stringId.StartsWith("player_settlement_", StringComparison.Ordinal))
		{
			return true;
		}
		foreach (Settlement playerBuiltFortification in PlayerBuiltFortificationAiFix.GetPlayerBuiltFortifications())
		{
			if (playerBuiltFortification == settlement)
			{
				return true;
			}
		}
		return false;
	}

	private static void SnapFrameToMapTerrain(ref MatrixFrame globalFrame)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Campaign current = Campaign.Current;
		IMapScene val = ((current != null) ? current.MapSceneWrapper : null);
		if (val != null)
		{
			float z = globalFrame.origin.z;
			CampaignVec2 val2 = default(CampaignVec2);
			(val2)..ctor(new Vec2(globalFrame.origin.x, globalFrame.origin.y), true);
			val.GetHeightAtPoint(ref val2, ref z);
			if (!float.IsNaN(z) && !float.IsInfinity(z))
			{
				globalFrame.origin.z = z + 0.08f;
			}
		}
	}
}
[HarmonyPatch]
public static class SiegeMachineCirclesBoundsFix
{
	private static bool _loggedUnsafeCircleSet;

	private static MethodBase TargetMethod()
	{
		return AccessTools.Method("SandBox.View.Map.Managers.SettlementVisualManager:TickSiegeMachineCircles");
	}

	private static bool Prefix(ref GameEntity[] ____defenderMachinesCircleEntities, ref GameEntity[] ____attackerRangedMachinesCircleEntities, ref GameEntity[] ____attackerRamMachinesCircleEntities, ref GameEntity[] ____attackerTowerMachinesCircleEntities)
	{
		try
		{
			if (!HasUsableEntities(____defenderMachinesCircleEntities, 4) || !HasUsableEntities(____attackerRangedMachinesCircleEntities, 4) || !HasUsableEntities(____attackerRamMachinesCircleEntities, 1) || !HasUsableEntities(____attackerTowerMachinesCircleEntities, 2))
			{
				if (!_loggedUnsafeCircleSet)
				{
					_loggedUnsafeCircleSet = true;
					PlayerSettlementFixesLoader.LogError("SiegeMachineCirclesBoundsFix: skipped a TickSiegeMachineCircles tick because a settlement's circle-entity arrays are smaller than the engine slots Bannerlord expects.");
				}
				return false;
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("SiegeMachineCirclesBoundsFix prefix: " + ex.Message);
			return false;
		}
		return true;
	}

	private static bool HasUsableEntities(GameEntity[] entities, int minimumCount)
	{
		if (entities == null || entities.Length < minimumCount)
		{
			return false;
		}
		for (int i = 0; i < minimumCount; i++)
		{
			if (entities[i] == (GameEntity)null)
			{
				return false;
			}
		}
		return true;
	}

	private static Exception Finalizer(Exception __exception)
	{
		if (__exception is IndexOutOfRangeException || __exception is NullReferenceException)
		{
			PlayerSettlementFixesLoader.LogError("SiegeMachineCirclesBoundsFix: suppressed " + __exception.GetType().Name);
			return null;
		}
		return __exception;
	}
}
[HarmonyPatch(typeof(Town), "GetWallLevel")]
public static class TownWallLevelZeroGuardFix
{
	private static void Postfix(Town __instance, ref int __result)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (__result == 0)
			{
				Settlement settlement = ((SettlementComponent)__instance).Settlement;
				if (settlement != null && PlayerBuiltSiegeMachinePlacementFix.IsPlayerSettlement(settlement))
				{
					__result = 1;
				}
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("TownWallLevelZeroGuardFix: " + ex.Message);
		}
	}
}
[HarmonyPatch(typeof(Army), "FindBestGatheringSettlementAndMoveTheLeader")]
public static class ArmyFindBestGatheringSettlementNullFix
{
	private static Exception Finalizer(Exception __exception, Settlement focusSettlement)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (__exception is NullReferenceException)
		{
			string text = ((focusSettlement != null) ? ((MBObjectBase)focusSettlement).StringId : "null");
			PlayerSettlementFixesLoader.LogError("ArmyFindBestGatheringSettlementNullFix: suppressed vanilla NRE while redirecting a gathering army away from " + text + " (no reachable fallback settlement found).");
			return null;
		}
		return __exception;
	}
}
[HarmonyPatch]
public static class DefenderSiegeEngineSlotsFix
{
	private static readonly HashSet<string> _logged = new HashSet<string>();

	private static PropertyInfo _frameAndVisualOfEnginesProp;

	private static PropertyInfo _pointerProp;

	private static MethodBase TargetMethod()
	{
		return AccessTools.Method("SandBox.View.Map.Visuals.SettlementVisual:UpdateDefenderSiegeEntitiesCache");
	}

	private static void Postfix(object __instance)
	{
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Type type = __instance.GetType();
			object obj = AccessTools.Property(type, "MapEntity")?.GetValue(__instance);
			object obj2 = ((obj is PartyBase) ? obj : null);
			Settlement val = ((obj2 != null) ? ((PartyBase)obj2).Settlement : null);
			if (val == null || !PlayerBuiltSiegeMachinePlacementFix.IsPlayerSettlement(val))
			{
				return;
			}
			FieldInfo fieldInfo = AccessTools.Field(type, "_defenderRangedEngineSpawnEntitiesCacheForCurrentLevel");
			GameEntity[] array = AccessTools.Field(type, "_defenderRangedEngineSpawnEntitiesForAllLevels")?.GetValue(__instance) as GameEntity[];
			if (fieldInfo == null || array == null)
			{
				return;
			}
			GameEntity[] array2 = fieldInfo.GetValue(__instance) as GameEntity[];
			if (array2 != null && array2.Length <= 4)
			{
				return;
			}
			Town town = val.Town;
			int num = ((town == null) ? 1 : town.GetWallLevel());
			if (num < 1)
			{
				num = 1;
			}
			if (num > 3)
			{
				num = 3;
			}
			bool flag = false;
			GameEntity[] array3;
			if (array.Length >= 3 && array.Length % 3 == 0 && IsLevelTriplesLayout(array, 3))
			{
				int num2 = array.Length / 3;
				array3 = (GameEntity[])(object)new GameEntity[num2];
				for (int i = 0; i < num2; i++)
				{
					GameEntity val2 = array[i * 3];
					GameEntity val3 = array[i * 3 + 1];
					GameEntity val4 = array[i * 3 + 2];
					GameEntity[] array4 = (GameEntity[])(object)new GameEntity[3] { val2, val3, val4 };
					GameEntity val5 = array4[num - 1];
					if (IsOutlierAmongSiblings(val5, array4, 2.25f))
					{
						GameEntity val6 = NearestNonOutlierSibling(val5, array4, num, 2.25f);
						if (val6 != (GameEntity)null && val6 != val5)
						{
							val5 = val6;
							flag = true;
						}
					}
					array3[i] = val5;
				}
			}
			else
			{
				string suffix = "_l" + num;
				array3 = array.Where((GameEntity e) => e != (GameEntity)null && e.Name != null && e.Name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)).ToArray();
			}
			if (array3 == null || array3.Length == 0)
			{
				return;
			}
			fieldInfo.SetValue(__instance, array3);
			RebuildEngineClickTargets(array, array3, __instance, out var added, out var removed);
			int num3 = -1;
			FieldInfo fieldInfo2 = AccessTools.Field(type, "_defenderBreachableWallEntitiesCacheForCurrentLevel");
			GameEntity[] array5 = fieldInfo2?.GetValue(__instance) as GameEntity[];
			int wallSectionCount = val.WallSectionCount;
			if (fieldInfo2 != null && array5 != null && wallSectionCount > 0)
			{
				GameEntity[] array6 = array5.Where((GameEntity e) => e != (GameEntity)null).ToArray();
				if (array6.Length > wallSectionCount)
				{
					GameEntity[] array7 = (GameEntity[])(object)new GameEntity[wallSectionCount];
					for (int j = 0; j < wallSectionCount; j++)
					{
						array7[j] = array6[(int)((long)j * (long)array6.Length / wallSectionCount)];
					}
					fieldInfo2.SetValue(__instance, array7);
					num3 = wallSectionCount;
				}
				else
				{
					num3 = array6.Length;
				}
			}
			string stringId = ((MBObjectBase)val).StringId;
			string item = $"{stringId}|{num}|{array.Length}|{((array2 != null) ? array2.Length : (-1))}|{array3.Length}|{flag}|{num3}";
			if (_logged.Add(item))
			{
				PlayerSettlementFixesLoader.LogError($"DefenderSiegeEngineSlotsFix [{stringId}]: WallLevel={num} AllLevels={array.Length} " + $"CacheBefore={((array2 != null) ? array2.Length : (-1))} CacheAfter={array3.Length} OutlierCorrected={flag} " + $"ClickTargets(added={added},removed={removed}) WallIcons={num3}.");
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("DefenderSiegeEngineSlotsFix: " + ex.Message);
		}
	}

	private static void RebuildEngineClickTargets(GameEntity[] all, GameEntity[] keep, object settlementVisual, out int added, out int removed)
	{
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		added = 0;
		removed = 0;
		try
		{
			if (_frameAndVisualOfEnginesProp == null)
			{
				Type type = AccessTools.TypeByName("SandBox.View.Map.MapScreen");
				_frameAndVisualOfEnginesProp = ((type != null) ? AccessTools.Property(type, "FrameAndVisualOfEngines") : null);
			}
			if (!(_frameAndVisualOfEnginesProp?.GetValue(null) is IDictionary dictionary))
			{
				return;
			}
			Type type2 = dictionary.GetType();
			Type type3 = ((type2.IsGenericType && type2.GetGenericArguments().Length == 2) ? type2.GetGenericArguments()[1] : null);
			if (type3 == null)
			{
				return;
			}
			HashSet<GameEntity> hashSet = new HashSet<GameEntity>(keep);
			GameEntity[] array = keep;
			foreach (GameEntity val in array)
			{
				if (val == (GameEntity)null || val.ChildCount <= 0)
				{
					continue;
				}
				GameEntity child = val.GetChild(0);
				if (child == (GameEntity)null)
				{
					continue;
				}
				object enginePointer = GetEnginePointer(child);
				if (enginePointer != null)
				{
					bool flag = dictionary.Contains(enginePointer);
					object value = Activator.CreateInstance(type3, val.GetGlobalFrame(), settlementVisual);
					dictionary[enginePointer] = value;
					if (!flag)
					{
						added++;
					}
				}
			}
			array = all;
			foreach (GameEntity val2 in array)
			{
				if (val2 == (GameEntity)null || hashSet.Contains(val2) || val2.ChildCount <= 0)
				{
					continue;
				}
				GameEntity child2 = val2.GetChild(0);
				if (!(child2 == (GameEntity)null))
				{
					object enginePointer2 = GetEnginePointer(child2);
					if (enginePointer2 != null && dictionary.Contains(enginePointer2))
					{
						dictionary.Remove(enginePointer2);
						removed++;
					}
				}
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("RebuildEngineClickTargets: " + ex.Message);
		}
	}

	private static object GetEnginePointer(GameEntity entity)
	{
		try
		{
			if (_pointerProp == null)
			{
				_pointerProp = AccessTools.Property(((object)entity).GetType(), "Pointer");
			}
			return _pointerProp?.GetValue(entity);
		}
		catch
		{
			return null;
		}
	}

	private static bool IsLevelTriplesLayout(GameEntity[] all, int levels)
	{
		for (int i = 0; i < all.Length; i++)
		{
			GameEntity val = all[i];
			if (val == (GameEntity)null || val.Name == null)
			{
				return false;
			}
			if (!val.Name.EndsWith("_l" + (i % levels + 1), StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
		}
		return true;
	}

	private static bool TryGetXY(GameEntity e, out float x, out float y)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		x = 0f;
		y = 0f;
		if (e == (GameEntity)null)
		{
			return false;
		}
		MatrixFrame globalFrame = e.GetGlobalFrame();
		x = globalFrame.origin.x;
		y = globalFrame.origin.y;
		return true;
	}

	private static float DistSq(GameEntity a, GameEntity b)
	{
		if (!TryGetXY(a, out var x, out var y) || !TryGetXY(b, out var x2, out var y2))
		{
			return float.MaxValue;
		}
		float num = x - x2;
		float num2 = y - y2;
		return num * num + num2 * num2;
	}

	private static bool IsOutlierAmongSiblings(GameEntity target, GameEntity[] siblings, float thresholdSq)
	{
		float num = float.MaxValue;
		foreach (GameEntity val in siblings)
		{
			if (!(val == (GameEntity)null) && !(val == target))
			{
				float num2 = DistSq(target, val);
				if (num2 < num)
				{
					num = num2;
				}
			}
		}
		if (num == float.MaxValue)
		{
			return false;
		}
		return num > thresholdSq;
	}

	private static GameEntity NearestNonOutlierSibling(GameEntity target, GameEntity[] siblings, int wallLevel, float thresholdSq)
	{
		GameEntity result = null;
		int num = int.MaxValue;
		for (int i = 0; i < siblings.Length; i++)
		{
			GameEntity val = siblings[i];
			if (!(val == (GameEntity)null) && !(val == target) && !IsOutlierAmongSiblings(val, siblings, thresholdSq))
			{
				int num2 = Math.Abs(i + 1 - wallLevel);
				if (num2 < num)
				{
					num = num2;
					result = val;
				}
			}
		}
		return result;
	}
}
[HarmonyPatch(typeof(Army), "IsAnotherEnemyBesiegingTarget")]
public static class ArmyIsAnotherEnemyBesiegingTargetNullFix
{
	private static bool _logged;

	private static Exception Finalizer(Exception __exception, ref bool __result)
	{
		if (__exception is NullReferenceException)
		{
			__result = false;
			if (!_logged)
			{
				_logged = true;
				PlayerSettlementFixesLoader.LogError("ArmyIsAnotherEnemyBesiegingTargetNullFix: suppressed vanilla NRE in Army.IsAnotherEnemyBesiegingTarget; returning false so the army keeps its siege instead of crashing.");
			}
			return null;
		}
		return __exception;
	}
}
[HarmonyPatch]
public static class WallSectionTooltipBoundsFix
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method("SandBox.ViewModelCollection.SandBoxUIHelper:GetWallSectionTooltip");
	}

	private static void Prefix(Settlement settlement, ref int wallIndex)
	{
		try
		{
			if (settlement != null && settlement.IsFortification)
			{
				int num = ((ICollection)settlement.SettlementWallSectionHitPointsRatioList)?.Count ?? 0;
				if (num <= 0)
				{
					wallIndex = 0;
				}
				else if (wallIndex < 0 || wallIndex >= num)
				{
					wallIndex = (wallIndex % num + num) % num;
				}
			}
		}
		catch
		{
		}
	}
}
[HarmonyPatch]
public static class OnMapHoverSiegeEngineDiagnostic
{
	private static bool _logged;

	private static MethodBase TargetMethod()
	{
		return AccessTools.Method("SandBox.View.Map.Visuals.SettlementVisual:OnMapHoverSiegeEngine");
	}

	private static void Finalizer(Exception __exception)
	{
		if (__exception != null && !_logged)
		{
			_logged = true;
			PlayerSettlementFixesLoader.LogError("HOVER_EX: " + __exception.GetType().Name + ": " + __exception.Message + " | " + __exception.StackTrace);
		}
	}
}
[HarmonyPatch]
public static class PlayerSettlementWallStateFix
{
	private static readonly HashSet<string> _logged = new HashSet<string>();

	private static MethodBase TargetMethod()
	{
		return AccessTools.Method("SandBox.View.Map.Visuals.SettlementVisual:RefreshWallState");
	}

	private static void Postfix(object __instance, GameEntity[] ____defenderBreachableWallEntitiesForAllLevels)
	{
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (____defenderBreachableWallEntitiesForAllLevels == null || ____defenderBreachableWallEntitiesForAllLevels.Length == 0)
			{
				return;
			}
			object obj = AccessTools.Property(__instance.GetType(), "MapEntity")?.GetValue(__instance);
			object obj2 = ((obj is PartyBase) ? obj : null);
			Settlement val = ((obj2 != null) ? ((PartyBase)obj2).Settlement : null);
			if (val == null || !PlayerBuiltSiegeMachinePlacementFix.IsPlayerSettlement(val))
			{
				return;
			}
			MBReadOnlyList<float> settlementWallSectionHitPointsRatioList = val.SettlementWallSectionHitPointsRatioList;
			int num = ((ICollection)settlementWallSectionHitPointsRatioList)?.Count ?? 0;
			if (num <= 0)
			{
				return;
			}
			int num2 = 0;
			for (int i = 0; i < ____defenderBreachableWallEntitiesForAllLevels.Length; i++)
			{
				GameEntity val2 = ____defenderBreachableWallEntitiesForAllLevels[i];
				if (val2 == (GameEntity)null)
				{
					continue;
				}
				bool flag = ((List<float>)(object)settlementWallSectionHitPointsRatioList)[i % num] <= 0f;
				foreach (GameEntity child in val2.GetChildren())
				{
					if (!(child == (GameEntity)null))
					{
						if (child.HasTag("map_broken_wall"))
						{
							child.SetVisibilityExcludeParents(flag);
							num2++;
						}
						else if (child.Name != null && child.Name.IndexOf("wall", StringComparison.OrdinalIgnoreCase) >= 0)
						{
							child.SetVisibilityExcludeParents(!flag);
							num2++;
						}
					}
				}
			}
			string stringId = ((MBObjectBase)val).StringId;
			string item = stringId + "|" + ____defenderBreachableWallEntitiesForAllLevels.Length + "|" + num;
			if (_logged.Add(item))
			{
				PlayerSettlementFixesLoader.LogError($"PlayerSettlementWallStateFix [{stringId}]: segments={____defenderBreachableWallEntitiesForAllLevels.Length} wallSections={num} " + $"meshesAdjusted={num2}.");
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("PlayerSettlementWallStateFix: " + ex.Message);
		}
	}
}
[HarmonyPatch]
public static class WallSectionIconHitpointsFix
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method("SandBox.ViewModelCollection.MapSiege.MapSiegePOIVM:RefreshHitpoints");
	}

	private static void Postfix(object __instance, ref float ____bindCurrentHitpoints, ref float ____bindMaxHitpoints, ref int ____bindMachineType)
	{
		try
		{
			Settlement val = PlayerSiege.PlayerSiegeEvent?.BesiegedSettlement;
			if (val == null || !PlayerBuiltSiegeMachinePlacementFix.IsPlayerSettlement(val))
			{
				return;
			}
			Type type = __instance.GetType();
			object obj = AccessTools.Property(type, "Type")?.GetValue(__instance);
			if (obj == null || Convert.ToInt32(obj) != 0)
			{
				return;
			}
			object obj2 = AccessTools.Property(type, "MachineIndex")?.GetValue(__instance);
			if (obj2 == null)
			{
				return;
			}
			int num = Convert.ToInt32(obj2);
			if (num < 0)
			{
				return;
			}
			MBReadOnlyList<float> settlementWallSectionHitPointsRatioList = val.SettlementWallSectionHitPointsRatioList;
			int num2 = ((ICollection)settlementWallSectionHitPointsRatioList)?.Count ?? 0;
			if (num2 > 0 && num >= num2)
			{
				int wallSectionCount = val.WallSectionCount;
				if (wallSectionCount > 0)
				{
					float num3 = val.MaxWallHitPoints / (float)wallSectionCount;
					float num4 = ((List<float>)(object)settlementWallSectionHitPointsRatioList)[num % num2] * num3;
					____bindMaxHitpoints = num3;
					____bindCurrentHitpoints = num4;
					____bindMachineType = ((num4 <= 0f) ? 1 : 0);
				}
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("WallSectionIconHitpointsFix: " + ex.Message);
		}
	}
}
[HarmonyPatch(typeof(DisbandArmyAction), "ApplyByCohesionDepleted")]
public static class PlayerSettlementBesiegerCohesionFix
{
	private static bool _logged;

	private static bool Prefix(Army army)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (army == null)
			{
				return true;
			}
			IMapPoint aiBehaviorObject = army.AiBehaviorObject;
			object obj = ((aiBehaviorObject is Settlement) ? aiBehaviorObject : null);
			if (obj == null)
			{
				MobileParty leaderParty = army.LeaderParty;
				obj = ((leaderParty != null) ? leaderParty.BesiegedSettlement : null);
			}
			Settlement val = (Settlement)obj;
			if (val == null || !val.IsFortification || !PlayerBuiltSiegeMachinePlacementFix.IsPlayerSettlement(val))
			{
				return true;
			}
			MobileParty leaderParty2 = army.LeaderParty;
			IFaction val2 = ((leaderParty2 != null) ? leaderParty2.MapFaction : null);
			if (val2 == null || val.MapFaction == val2)
			{
				return true;
			}
			if (!FactionManager.IsAtWarAgainstFaction(val2, val.MapFaction))
			{
				return true;
			}
			if (army.Cohesion <= 0.1f)
			{
				return true;
			}
			if (!_logged)
			{
				_logged = true;
				PlayerSettlementFixesLoader.LogError($"PlayerSettlementBesiegerCohesionFix: kept army besieging {((MBObjectBase)val).StringId} together (cohesion={army.Cohesion:F1}) instead of disbanding.");
			}
			return false;
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("PlayerSettlementBesiegerCohesionFix: " + ex.Message);
			return true;
		}
	}
}
[HarmonyPatch(typeof(BesiegerCamp), "StartingAssaultOnBesiegedSettlementIsLogical")]
public static class PlayerSettlementAssaultLogicFix
{
	private const float RequiredStrengthRatio = 2.5f;

	private static bool _logged;

	private static void Postfix(BesiegerCamp __instance, ref bool __result)
	{
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (__result)
			{
				return;
			}
			SiegeEvent val = ((__instance != null) ? __instance.SiegeEvent : null);
			Settlement val2 = val?.BesiegedSettlement;
			if (val2 == null || !PlayerBuiltSiegeMachinePlacementFix.IsPlayerSettlement(val2) || !__instance.IsPreparationComplete)
			{
				return;
			}
			bool flag = false;
			foreach (SiegeEngineConstructionProgress item in val.GetSiegeEventSide((BattleSideEnum)1).SiegeEngines.AllSiegeEngines())
			{
				if (item != null && item.IsConstructed && (item.SiegeEngine == DefaultSiegeEngineTypes.Ram || item.SiegeEngine == DefaultSiegeEngineTypes.ImprovedRam || item.SiegeEngine == DefaultSiegeEngineTypes.SiegeTower))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			float num = 0f;
			foreach (PartyBase item2 in __instance.GetInvolvedPartiesForEventType((BattleTypes)5))
			{
				if (item2 != null)
				{
					num += item2.CalculateCurrentStrength();
				}
			}
			float num2 = 0f;
			foreach (PartyBase item3 in val2.GetInvolvedPartiesForEventType((BattleTypes)5))
			{
				if (item3 != null && item3.IsMobile && item3.MobileParty != null && item3.MobileParty.CurrentSettlement == val2)
				{
					num2 += item3.CalculateCurrentStrength();
				}
			}
			if (num2 < 1f)
			{
				num2 = 1f;
			}
			if (num >= num2 * 2.5f)
			{
				__result = true;
				if (!_logged)
				{
					_logged = true;
					PlayerSettlementFixesLoader.LogError("PlayerSettlementAssaultLogicFix: cleared assault on " + ((MBObjectBase)val2).StringId + " " + $"(attackers={num:F0} defenders={num2:F0}, has assault engine, prep complete).");
				}
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("PlayerSettlementAssaultLogicFix: " + ex.Message);
		}
	}
}
[HarmonyPatch(typeof(BesiegerCamp), "GetSiegeCampPartyPosition")]
public static class SiegeCampEmptyFramesCrashFix
{
	private static bool _logged;

	private static bool Prefix(MobileParty mobileParty, MatrixFrame[] siegeCamp1GlobalFrames, ref CampaignVec2 __result)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (siegeCamp1GlobalFrames != null && siegeCamp1GlobalFrames.Length != 0)
		{
			return true;
		}
		__result = (CampaignVec2)((mobileParty != null) ? mobileParty.Position : default(CampaignVec2));
		if (!_logged)
		{
			_logged = true;
			PlayerSettlementFixesLoader.LogError("SiegeCampEmptyFramesCrashFix: a besieged settlement had no siege-camp frames on load; kept the besieger at its saved position instead of crashing (compatibility with overhaul/custom-map mods like Empires of Europe 1700).");
		}
		return false;
	}
}
[HarmonyPatch]
public static class MapDistanceNavCacheNullFix
{
	private static bool _logged;

	private static MethodBase TargetMethod()
	{
		return AccessTools.Method(typeof(DefaultMapDistanceModel), "GetDistance", new Type[6]
		{
			typeof(Settlement),
			typeof(Settlement),
			typeof(bool),
			typeof(bool),
			typeof(NavigationType),
			typeof(float).MakeByRefType()
		});
	}

	private static Exception Finalizer(Exception __exception, ref float __result, ref float landRatio)
	{
		if (__exception is NullReferenceException)
		{
			__result = float.MaxValue;
			landRatio = 1f;
			if (!_logged)
			{
				_logged = true;
				PlayerSettlementFixesLoader.LogError("MapDistanceNavCacheNullFix: suppressed a navigation-cache NRE while computing settlement distances during load (a player settlement not yet in the nav mesh); treated the pair as unreachable so the save loads. Compatibility with overhaul/custom-map mods like Empires of Europe 1700.");
			}
			return null;
		}
		return __exception;
	}
}
[HarmonyPatch(typeof(DefaultMapDistanceModel), "GetClosestEntranceToFace")]
public static class ClosestEntranceToFaceNullFix
{
	private static bool _logged;

	private static Exception Finalizer(Exception __exception, ref (Settlement, bool) __result)
	{
		if (__exception is NullReferenceException)
		{
			__result = (null, false);
			if (!_logged)
			{
				_logged = true;
				PlayerSettlementFixesLoader.LogError("ClosestEntranceToFaceNullFix: suppressed a navigation-cache NRE in GetClosestEntranceToFace during a party AI tick; returned no settlement so the AI uses its fallback distance instead of crashing. Compatibility with overhaul/custom-map mods like Empires of Europe 1700.");
			}
			return null;
		}
		return __exception;
	}
}
[HarmonyPatch(typeof(DefaultMapDistanceModel), "GetNeighborsOfFortification")]
public static class FortificationNeighborsNullFix
{
	private static bool _logged;

	private static Exception Finalizer(Exception __exception, ref MBReadOnlyList<Settlement> __result)
	{
		if (__exception is NullReferenceException)
		{
			__result = new MBReadOnlyList<Settlement>();
			if (!_logged)
			{
				_logged = true;
				PlayerSettlementFixesLoader.LogError("FortificationNeighborsNullFix: suppressed a navigation-cache NRE while the AI scored besieging a player settlement (no neighbour data in the cache); returned an empty neighbour list so the AI tick continues. Compatibility with overhaul/custom-map mods like Empires of Europe 1700.");
			}
			return null;
		}
		return __exception;
	}
}
[HarmonyPatch(typeof(DefaultMapDistanceModel), "GetLandRatioOfPathBetweenSettlements")]
public static class LandRatioNavCacheNullFix
{
	private static bool _logged;

	private static Exception Finalizer(Exception __exception, ref float __result)
	{
		if (__exception is NullReferenceException)
		{
			__result = 1f;
			if (!_logged)
			{
				_logged = true;
				PlayerSettlementFixesLoader.LogError("LandRatioNavCacheNullFix: suppressed a navigation-cache NRE in GetLandRatioOfPathBetweenSettlements (a player settlement not yet in the nav mesh); returned a default land ratio of 1 instead of crashing.");
			}
			return null;
		}
		return __exception;
	}
}
[HarmonyPatch]
public static class PlayerSettlementWallTierVisibilityFix
{
	private static readonly HashSet<string> _logged = new HashSet<string>();

	private static MethodBase TargetMethod()
	{
		return AccessTools.Method("SandBox.View.Map.Visuals.SettlementVisual:SetSettlementLevelVisibility");
	}

	private static void Postfix(object __instance)
	{
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Type type = __instance.GetType();
			object obj = AccessTools.Property(type, "MapEntity")?.GetValue(__instance);
			object obj2 = ((obj is PartyBase) ? obj : null);
			Settlement val = ((obj2 != null) ? ((PartyBase)obj2).Settlement : null);
			if (val == null || !PlayerBuiltSiegeMachinePlacementFix.IsPlayerSettlement(val))
			{
				return;
			}
			int num;
			if (val.IsFortification)
			{
				Town town = val.Town;
				num = ((town == null) ? 1 : town.GetWallLevel());
			}
			else
			{
				if (!val.IsVillage || val.Village == null)
				{
					return;
				}
				float hearth = val.Village.Hearth;
				num = ((hearth < 200f) ? 1 : ((hearth < 600f) ? 2 : 3));
			}
			if (num < 1)
			{
				num = 1;
			}
			if (num > 3)
			{
				num = 3;
			}
			object obj3 = AccessTools.Property(type, "StrategicEntity")?.GetValue(__instance);
			GameEntity val2 = (GameEntity)((obj3 is GameEntity) ? obj3 : null);
			if (val2 == (GameEntity)null)
			{
				return;
			}
			List<GameEntity> list = new List<GameEntity>();
			val2.GetChildrenRecursive(ref list);
			int num2 = 0;
			int num3 = 0;
			foreach (GameEntity item2 in list)
			{
				if (item2 == (GameEntity)null)
				{
					continue;
				}
				int nameEncodedTier = GetNameEncodedTier(item2.Name);
				if (nameEncodedTier != 0)
				{
					bool flag = nameEncodedTier == num;
					item2.SetVisibilityExcludeParents(flag);
					if (flag)
					{
						num2++;
					}
					else
					{
						num3++;
					}
				}
			}
			if (num3 > 0 || num2 > 0)
			{
				string stringId = ((MBObjectBase)val).StringId;
				string item = $"{stringId}|{num}|{num2}|{num3}";
				if (_logged.Add(item))
				{
					PlayerSettlementFixesLoader.LogError($"PlayerSettlementWallTierVisibilityFix [{stringId}]: Tier={num} " + "(" + (val.IsFortification ? "wall level" : "production level") + ") " + $"tierEntitiesShown={num2} tierEntitiesHidden={num3}.");
				}
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("PlayerSettlementWallTierVisibilityFix: " + ex.Message);
		}
	}

	internal static int GetNameEncodedTier(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return 0;
		}
		for (int i = 1; i <= 3; i++)
		{
			string text = "_l" + i;
			if (name.EndsWith(text, StringComparison.OrdinalIgnoreCase) || name.IndexOf(text + "_", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return i;
			}
		}
		return 0;
	}
}
[HarmonyPatch]
public static class PlacementPreviewWallTierFix
{
	private static readonly HashSet<string> _logged = new HashSet<string>();

	private static MethodBase TargetMethod()
	{
		return AccessTools.Method("BannerlordPlayerSettlement.Behaviours.PlayerSettlementBehaviour:UpdateSettlementVisualEntity");
	}

	private static void Postfix(object __instance)
	{
		try
		{
			Type type = __instance.GetType();
			object obj = AccessTools.Field(type, "settlementVisualEntity")?.GetValue(__instance);
			GameEntity val = (GameEntity)((obj is GameEntity) ? obj : null);
			if (val == (GameEntity)null)
			{
				return;
			}
			object obj2 = AccessTools.Field(type, "OverwriteRequest")?.GetValue(__instance);
			Settlement val2 = (Settlement)((obj2 is Settlement) ? obj2 : null);
			if (val2 == null)
			{
				object obj3 = AccessTools.Field(type, "ReSettlementRequest")?.GetValue(__instance);
				object obj4 = obj3?.GetType().GetField("Settlement", BindingFlags.Instance | BindingFlags.Public)?.GetValue(obj3);
				val2 = (Settlement)((obj4 is Settlement) ? obj4 : null);
			}
			if (val2 == null)
			{
				foreach (Settlement item2 in (List<Settlement>)(object)Settlement.All)
				{
					if (item2 != null && !item2.IsVisible && (item2.IsFortification || item2.IsVillage) && (item2.Town != null || item2.Village != null))
					{
						val2 = item2;
						break;
					}
				}
			}
			int num;
			if (val2 != null && val2.IsVillage && val2.Village != null)
			{
				float hearth = val2.Village.Hearth;
				num = ((hearth < 200f) ? 1 : ((hearth < 600f) ? 2 : 3));
			}
			else
			{
				int? obj5;
				if (val2 == null)
				{
					obj5 = null;
				}
				else
				{
					Town town = val2.Town;
					obj5 = ((town != null) ? new int?(town.GetWallLevel()) : null);
				}
				int? num2 = obj5;
				num = num2.GetValueOrDefault(1);
			}
			if (num < 1)
			{
				num = 1;
			}
			if (num > 3)
			{
				num = 3;
			}
			List<GameEntity> list = new List<GameEntity>();
			val.GetChildrenRecursive(ref list);
			int num3 = 0;
			int num4 = 0;
			foreach (GameEntity item3 in list)
			{
				if (item3 == (GameEntity)null)
				{
					continue;
				}
				int nameEncodedTier = PlayerSettlementWallTierVisibilityFix.GetNameEncodedTier(item3.Name);
				if (nameEncodedTier != 0)
				{
					bool flag = nameEncodedTier == num;
					item3.SetVisibilityExcludeParents(flag);
					if (flag)
					{
						num3++;
					}
					else
					{
						num4++;
					}
				}
			}
			if (num4 > 0 || num3 > 0)
			{
				string item = $"{val.Name}|{num}|{num3}|{num4}";
				if (_logged.Add(item))
				{
					PlayerSettlementFixesLoader.LogError($"PlacementPreviewWallTierFix: preview Tier={num} " + $"tierEntitiesShown={num3} tierEntitiesHidden={num4}.");
				}
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("PlacementPreviewWallTierFix: " + ex.Message);
		}
	}
}
[HarmonyPatch]
public static class PlayerSettlementGroundLevelFix
{
	private static readonly HashSet<string> _logged = new HashSet<string>();

	private static MethodBase TargetMethod()
	{
		return AccessTools.Method("SandBox.View.Map.Visuals.SettlementVisual:OnStartup");
	}

	private static void Postfix(object __instance)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Type type = __instance.GetType();
			object obj = AccessTools.Property(type, "MapEntity")?.GetValue(__instance);
			object obj2 = ((obj is PartyBase) ? obj : null);
			Settlement val = ((obj2 != null) ? ((PartyBase)obj2).Settlement : null);
			if (val == null)
			{
				return;
			}
			string stringId = ((MBObjectBase)val).StringId;
			if (stringId == null || !stringId.StartsWith("player_settlement_", StringComparison.Ordinal))
			{
				return;
			}
			object obj3 = AccessTools.Property(type, "StrategicEntity")?.GetValue(__instance);
			GameEntity val2 = (GameEntity)((obj3 is GameEntity) ? obj3 : null);
			if (val2 == (GameEntity)null)
			{
				return;
			}
			Campaign current = Campaign.Current;
			IMapScene val3 = ((current != null) ? current.MapSceneWrapper : null);
			if (val3 == null)
			{
				return;
			}
			MatrixFrame frame = val2.GetFrame();
			float z = frame.origin.z;
			CampaignVec2 val4 = default(CampaignVec2);
			(val4)..ctor(new Vec2(frame.origin.x, frame.origin.y), true);
			val3.GetHeightAtPoint(ref val4, ref z);
			if (!float.IsNaN(z) && !float.IsInfinity(z))
			{
				float num = frame.origin.z - z;
				if (Math.Abs(num) > 0.02f && _logged.Add(stringId))
				{
					PlayerSettlementFixesLoader.LogError($"PlayerSettlementGroundLevelFix [{stringId}]: pivot sits {num:+0.00;-0.00} units relative " + "to campaign terrain (measurement only, no correction applied).");
				}
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("PlayerSettlementGroundLevelFix: " + ex.Message);
		}
	}
}
[HarmonyPatch]
public static class VillageDeepEditIndexDiagnostic
{
	private static readonly HashSet<string> _logged = new HashSet<string>();

	private static MethodBase TargetMethod()
	{
		return AccessTools.Method("SandBox.View.Map.Visuals.SettlementVisual:OnStartup");
	}

	private static void Postfix(object __instance)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Type type = __instance.GetType();
			object obj = AccessTools.Property(type, "MapEntity")?.GetValue(__instance);
			object obj2 = ((obj is PartyBase) ? obj : null);
			Settlement val = ((obj2 != null) ? ((PartyBase)obj2).Settlement : null);
			if (val == null || !val.IsVillage)
			{
				return;
			}
			string stringId = ((MBObjectBase)val).StringId;
			if (stringId == null || !stringId.StartsWith("player_settlement_", StringComparison.Ordinal) || !_logged.Add(stringId))
			{
				return;
			}
			Type type2 = AccessTools.TypeByName("BannerlordPlayerSettlement.Saves.PlayerSettlementInfo");
			object obj3 = AccessTools.Property(type2, "Instance")?.GetValue(null);
			object obj4 = ((obj3 == null) ? null : AccessTools.Method(type2, "FindSettlement")?.Invoke(obj3, new object[1] { val }));
			if (obj4 == null || !(AccessTools.Field(obj4.GetType(), "DeepEdits")?.GetValue(obj4) is IEnumerable enumerable))
			{
				return;
			}
			object obj5 = AccessTools.Property(type, "StrategicEntity")?.GetValue(__instance);
			GameEntity val2 = (GameEntity)((obj5 is GameEntity) ? obj5 : null);
			if (val2 == (GameEntity)null)
			{
				return;
			}
			List<GameEntity> list = new List<GameEntity>();
			val2.GetChildrenRecursive(ref list);
			List<string> list2 = new List<string> { $"VillageDeepEditIndexDiagnostic [{stringId}]: finalChildCount={list.Count}." };
			int num = 0;
			int num2 = 0;
			foreach (object item in enumerable)
			{
				num++;
				Type type3 = item.GetType();
				int num3 = (int)(AccessTools.Field(type3, "Index")?.GetValue(item) ?? ((object)(-1)));
				bool flag = (bool)(AccessTools.Field(type3, "IsDeleted")?.GetValue(item) ?? ((object)false));
				string text = (AccessTools.Field(type3, "Name")?.GetValue(item) as string) ?? "";
				if (num3 < 0)
				{
					list2.Add($"  edit(root offset, IsDeleted={flag})");
					continue;
				}
				string text2 = ((num3 >= list.Count) ? "OUT OF RANGE" : ((list[num3] == (GameEntity)null) ? "(null entity)" : list[num3].Name));
				bool flag2 = !string.Equals(text, text2, StringComparison.Ordinal);
				if (flag2)
				{
					num2++;
				}
				list2.Add($"  edit(Index={num3}, IsDeleted={flag}, capturedName=\"{text}\") " + "-> entity at that index now = \"" + text2 + "\"" + (flag2 ? "  ** MISMATCH **" : ""));
			}
			if (num > 0)
			{
				if (num2 > 0)
				{
					list2.Insert(1, $"  {num2}/{num} edit(s) point at a DIFFERENT entity than when captured - deep edits (deletions/moves) on this village very likely landed on the wrong target.");
				}
				PlayerSettlementFixesLoader.LogError(string.Join("\n", list2));
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("VillageDeepEditIndexDiagnostic: " + ex.Message);
		}
	}
}
[HarmonyPatch]
public static class RebuildPartyRetargetFix
{
	private static readonly object _listenerOwner = new object();

	private static MethodBase TargetMethod()
	{
		return AccessTools.Method("BannerlordPlayerSettlement.Behaviours.PlayerSettlementBehaviour:OnSessionLaunched");
	}

	private static void Postfix()
	{
		try
		{
			SubscribeToRebuildEvent();
			int num = 0;
			foreach (Settlement playerBuiltFortification in PlayerBuiltFortificationAiFix.GetPlayerBuiltFortifications())
			{
				num += RetargetPartiesApproaching(playerBuiltFortification);
			}
			if (num > 0)
			{
				PlayerSettlementFixesLoader.LogError($"RebuildPartyRetargetFix: re-issued movement for {num} part(ies) stuck approaching player settlements at session start.");
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("RebuildPartyRetargetFix (session launch): " + ex.Message);
		}
	}

	private static void SubscribeToRebuildEvent()
	{
		try
		{
			if (!(AccessTools.Property(AccessTools.TypeByName("BannerlordPlayerSettlement.Behaviours.PlayerSettlementBehaviour"), "SettlementRebuildEvent")?.GetValue(null) is IMbEvent<Settlement> val))
			{
				PlayerSettlementFixesLoader.LogError("RebuildPartyRetargetFix: SettlementRebuildEvent not available; rebuild retargeting will only run at session start.");
			}
			else
			{
				val.AddNonSerializedListener(_listenerOwner, (Action<Settlement>)OnSettlementRebuilt);
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("RebuildPartyRetargetFix subscribe: " + ex.Message);
		}
	}

	private static void OnSettlementRebuilt(Settlement settlement)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (settlement != null)
			{
				int num = RetargetPartiesApproaching(settlement);
				bool flag = false;
				try
				{
					CampaignVec2 gatePosition = settlement.GatePosition;
					PathFaceRecord face = (gatePosition).Face;
					flag = (face).IsValid();
				}
				catch
				{
				}
				PlayerSettlementFixesLoader.LogError("RebuildPartyRetargetFix [" + ((MBObjectBase)settlement).StringId + "]: settlement rebuilt in place; " + $"re-issued movement for {num} approaching part(ies); gateFaceValid={flag}.");
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("RebuildPartyRetargetFix (rebuild event): " + ex.Message);
		}
	}

	private static int RetargetPartiesApproaching(Settlement settlement)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Invalid comparison between Unknown and I4
		int num = 0;
		foreach (MobileParty item in ((IEnumerable<MobileParty>)MobileParty.All).ToList())
		{
			try
			{
				if (item != null && item != MobileParty.MainParty && item.CurrentSettlement == null && (item.TargetSettlement == settlement || item.ShortTermTargetSettlement == settlement))
				{
					item.Ai.RethinkAtNextHourlyTick = true;
					if ((int)item.DefaultBehavior == 2 && item.TargetSettlement == settlement && !item.IsCurrentlyAtSea && !item.IsTargetingPort)
					{
						item.SetMoveGoToSettlement(settlement, (NavigationType)1, false);
					}
					num++;
				}
			}
			catch (Exception ex)
			{
				PlayerSettlementFixesLoader.LogError("RetargetPartiesApproaching: " + ex.Message);
			}
		}
		return num;
	}
}
[HarmonyPatch]
public static class NewTownWallLevelFix
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method("BannerlordPlayerSettlement.Behaviours.PlayerSettlementBehaviour:InitTownBuildings");
	}

	private static void Postfix(Settlement townSettlement)
	{
		NewSettlementWallLevelHelper.ClampNewFortificationsToLevelOne(townSettlement);
	}
}
[HarmonyPatch]
public static class NewCastleWallLevelFix
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method("BannerlordPlayerSettlement.Behaviours.PlayerSettlementBehaviour:InitCastleBuildings");
	}

	private static void Postfix(Settlement castleSettlement)
	{
		NewSettlementWallLevelHelper.ClampNewFortificationsToLevelOne(castleSettlement);
	}
}
internal static class NewSettlementWallLevelHelper
{
	internal static void ClampNewFortificationsToLevelOne(Settlement settlement)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Town val = settlement?.Town;
			if (val == null || val.Buildings == null)
			{
				return;
			}
			string stringId = ((MBObjectBase)settlement).StringId;
			if (stringId == null || !stringId.StartsWith("player_settlement_", StringComparison.Ordinal))
			{
				return;
			}
			Building val2 = null;
			foreach (Building item in (List<Building>)(object)val.Buildings)
			{
				if (((item != null) ? item.BuildingType : null) == DefaultBuildingTypes.SettlementFortifications || ((item != null) ? item.BuildingType : null) == DefaultBuildingTypes.CastleFortifications)
				{
					val2 = item;
					break;
				}
			}
			if (val2 == null || val2.CurrentLevel <= 1)
			{
				return;
			}
			int currentLevel = val2.CurrentLevel;
			val2.CurrentLevel = 1;
			try
			{
				if (val.BuildingsInProgress != null && !val2.BuildingType.IsDailyProject && !val.BuildingsInProgress.Contains(val2))
				{
					val.BuildingsInProgress.Enqueue(val2);
				}
			}
			catch (Exception ex)
			{
				PlayerSettlementFixesLoader.LogError("ClampNewFortificationsToLevelOne enqueue: " + ex.Message);
			}
			PartyBase party = settlement.Party;
			if (party != null)
			{
				party.SetVisualAsDirty();
			}
			PlayerSettlementFixesLoader.LogError("NewSettlementWallLevelFix [" + stringId + "]: new settlement's Fortifications randomly rolled " + $"level {currentLevel}; set to level 1 so walls start wooden and upgrade normally.");
		}
		catch (Exception ex2)
		{
			PlayerSettlementFixesLoader.LogError("ClampNewFortificationsToLevelOne: " + ex2.Message);
		}
	}
}
[HarmonyPatch]
[HarmonyPriority(800)]
public static class GatePortPlacementEscapeFix
{
	private static bool _logged;

	private static MethodBase TargetMethod()
	{
		return AccessTools.DeclaredMethod(AccessTools.TypeByName("SandBox.View.Map.MapScreen"), "TaleWorlds.CampaignSystem.GameState.IMapStateHandler.AfterWaitTick");
	}

	private static bool Prefix(object __instance)
	{
		try
		{
			Type type = AccessTools.TypeByName("BannerlordPlayerSettlement.Behaviours.PlayerSettlementBehaviour");
			object obj = AccessTools.Property(type, "Instance")?.GetValue(null);
			if (obj == null)
			{
				return true;
			}
			bool flag = (bool)(AccessTools.Property(type, "IsPlacingGate")?.GetValue(obj) ?? ((object)false));
			bool flag2 = (bool)(AccessTools.Property(type, "IsPlacingPort")?.GetValue(obj) ?? ((object)false));
			if (!flag && !flag2)
			{
				return true;
			}
			object obj2 = AccessTools.Property(__instance.GetType(), "Input")?.GetValue(__instance);
			if (obj2 == null)
			{
				return true;
			}
			Type type2 = AccessTools.TypeByName("TaleWorlds.InputSystem.IInputContext");
			MethodInfo methodInfo = ((type2 != null) ? AccessTools.Method(type2, "IsHotKeyReleased", new Type[1] { typeof(string) }) : null);
			if (!(methodInfo != null) || !(bool)methodInfo.Invoke(obj2, new object[1] { "ToggleEscapeMenu" }))
			{
				return true;
			}
			AccessTools.Method(type, "Reset")?.Invoke(obj, null);
			if (!_logged)
			{
				_logged = true;
				PlayerSettlementFixesLoader.LogError("GatePortPlacementEscapeFix: cancelled gate/port placement on Escape. The base mod only refreshed the ghost model in this case instead of cancelling, leaving IsPlacingGate/IsPlacingPort stuck true - every subsequent map click, including entering a finished settlement, was being silently swallowed by the placement-mode click handler.");
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("GatePortPlacementEscapeFix: " + ex.Message);
		}
		return true;
	}
}
[HarmonyPatch]
public static class RebuiltSettlementVisibilityHealFix
{
	[CompilerGenerated]
	private sealed class _GetAllTrackedPlayerSettlements_d__2 : IEnumerable<Settlement>, IEnumerable, IEnumerator<Settlement>, IDisposable, IEnumerator
	{
		private int __1__state;

		private Settlement __2__current;

		private int __l__initialThreadId;

		private Type _infoType_5__2;

		private object _info_5__3;

		private HashSet<Settlement> _seen_5__4;

		private string[] __7__wrap4;

		private int __7__wrap5;

		private IEnumerator __7__wrap6;

		private IEnumerator<Settlement> __7__wrap7;

		Settlement IEnumerator<Settlement>.Current
		{
			[DebuggerHidden]
			get
			{
				return __2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return __2__current;
			}
		}

		[DebuggerHidden]
		public _GetAllTrackedPlayerSettlements_d__2(int __1__state)
		{
			this.__1__state = __1__state;
			__l__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = __1__state;
			if ((uint)(num - -4) <= 1u || num == 1)
			{
				try
				{
					if (num == -4 || num == 1)
					{
						try
						{
						}
						finally
						{
							__m__Finally2();
						}
					}
				}
				finally
				{
					__m__Finally1();
				}
			}
			_infoType_5__2 = null;
			_info_5__3 = null;
			_seen_5__4 = null;
			__7__wrap4 = null;
			__7__wrap6 = null;
			__7__wrap7 = null;
			__1__state = -2;
		}

		private bool MoveNext()
		{
			try
			{
				int num = __1__state;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					__1__state = -4;
					goto IL_015c;
				}
				__1__state = -1;
				_infoType_5__2 = AccessTools.TypeByName("BannerlordPlayerSettlement.Saves.PlayerSettlementInfo");
				_info_5__3 = ((!(_infoType_5__2 != null)) ? null : AccessTools.Property(_infoType_5__2, "Instance")?.GetValue(null));
				if (_info_5__3 == null)
				{
					return false;
				}
				_seen_5__4 = new HashSet<Settlement>();
				__7__wrap4 = new string[4] { "Towns", "Castles", "OverwriteSettlements", "PlayerVillages" };
				__7__wrap5 = 0;
				goto IL_019e;
				IL_0176:
				if (__7__wrap6.MoveNext())
				{
					object current = __7__wrap6.Current;
					__7__wrap7 = SettlementAndNestedVillages(current, _seen_5__4).GetEnumerator();
					__1__state = -4;
					goto IL_015c;
				}
				__m__Finally1();
				__7__wrap6 = null;
				goto IL_0190;
				IL_015c:
				if (__7__wrap7.MoveNext())
				{
					Settlement current2 = __7__wrap7.Current;
					__2__current = current2;
					__1__state = 1;
					return true;
				}
				__m__Finally2();
				__7__wrap7 = null;
				goto IL_0176;
				IL_019e:
				if (__7__wrap5 < __7__wrap4.Length)
				{
					string name = __7__wrap4[__7__wrap5];
					if (_infoType_5__2.GetField(name, BindingFlags.Instance | BindingFlags.Public)?.GetValue(_info_5__3) is IList list)
					{
						__7__wrap6 = list.GetEnumerator();
						__1__state = -3;
						goto IL_0176;
					}
					goto IL_0190;
				}
				__7__wrap4 = null;
				return false;
				IL_0190:
				__7__wrap5++;
				goto IL_019e;
			}
			catch
			{
				//try-fault
				((IDisposable)this).Dispose();
				throw;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void __m__Finally1()
		{
			__1__state = -1;
			if (__7__wrap6 is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}

		private void __m__Finally2()
		{
			__1__state = -3;
			if (__7__wrap7 != null)
			{
				__7__wrap7.Dispose();
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<Settlement> IEnumerable<Settlement>.GetEnumerator()
		{
			if (__1__state == -2 && __l__initialThreadId == Environment.CurrentManagedThreadId)
			{
				__1__state = 0;
				return this;
			}
			return new _GetAllTrackedPlayerSettlements_d__2(0);
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<Settlement>)this).GetEnumerator();
		}
	}

	[CompilerGenerated]
	private sealed class _SettlementAndNestedVillages_d__3 : IEnumerable<Settlement>, IEnumerable, IEnumerator<Settlement>, IDisposable, IEnumerator
	{
		private int __1__state;

		private Settlement __2__current;

		private int __l__initialThreadId;

		private object item;

		public object __3__item;

		private HashSet<Settlement> seen;

		public HashSet<Settlement> __3__seen;

		private IEnumerator __7__wrap1;

		private IEnumerator<Settlement> __7__wrap2;

		Settlement IEnumerator<Settlement>.Current
		{
			[DebuggerHidden]
			get
			{
				return __2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return __2__current;
			}
		}

		[DebuggerHidden]
		public _SettlementAndNestedVillages_d__3(int __1__state)
		{
			this.__1__state = __1__state;
			__l__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = __1__state;
			if ((uint)(num - -4) <= 1u || num == 2)
			{
				try
				{
					if (num == -4 || num == 2)
					{
						try
						{
						}
						finally
						{
							__m__Finally2();
						}
					}
				}
				finally
				{
					__m__Finally1();
				}
			}
			__7__wrap1 = null;
			__7__wrap2 = null;
			__1__state = -2;
		}

		private bool MoveNext()
		{
			try
			{
				switch (__1__state)
				{
				default:
					return false;
				case 0:
				{
					__1__state = -1;
					if (item == null)
					{
						return false;
					}
					object obj = item.GetType().GetField("Settlement", BindingFlags.Instance | BindingFlags.Public)?.GetValue(item);
					Settlement val = (Settlement)((obj is Settlement) ? obj : null);
					if (val != null && seen.Add(val))
					{
						__2__current = val;
						__1__state = 1;
						return true;
					}
					goto IL_0092;
				}
				case 1:
					__1__state = -1;
					goto IL_0092;
				case 2:
					{
						__1__state = -4;
						goto IL_0138;
					}
					IL_0138:
					if (__7__wrap2.MoveNext())
					{
						Settlement current = __7__wrap2.Current;
						__2__current = current;
						__1__state = 2;
						return true;
					}
					__m__Finally2();
					__7__wrap2 = null;
					goto IL_0152;
					IL_0152:
					if (__7__wrap1.MoveNext())
					{
						object current2 = __7__wrap1.Current;
						__7__wrap2 = SettlementAndNestedVillages(current2, seen).GetEnumerator();
						__1__state = -4;
						goto IL_0138;
					}
					__m__Finally1();
					__7__wrap1 = null;
					return false;
					IL_0092:
					if (!(item.GetType().GetField("Villages", BindingFlags.Instance | BindingFlags.Public)?.GetValue(item) is IList list))
					{
						return false;
					}
					__7__wrap1 = list.GetEnumerator();
					__1__state = -3;
					goto IL_0152;
				}
			}
			catch
			{
				//try-fault
				((IDisposable)this).Dispose();
				throw;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void __m__Finally1()
		{
			__1__state = -1;
			if (__7__wrap1 is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}

		private void __m__Finally2()
		{
			__1__state = -3;
			if (__7__wrap2 != null)
			{
				__7__wrap2.Dispose();
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<Settlement> IEnumerable<Settlement>.GetEnumerator()
		{
			_SettlementAndNestedVillages_d__3 _SettlementAndNestedVillages_d__;
			if (__1__state == -2 && __l__initialThreadId == Environment.CurrentManagedThreadId)
			{
				__1__state = 0;
				_SettlementAndNestedVillages_d__ = this;
			}
			else
			{
				_SettlementAndNestedVillages_d__ = new _SettlementAndNestedVillages_d__3(0);
			}
			_SettlementAndNestedVillages_d__.item = __3__item;
			_SettlementAndNestedVillages_d__.seen = __3__seen;
			return _SettlementAndNestedVillages_d__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<Settlement>)this).GetEnumerator();
		}
	}

	private static MethodBase TargetMethod()
	{
		return AccessTools.Method("BannerlordPlayerSettlement.Behaviours.PlayerSettlementBehaviour:OnSessionLaunched");
	}

	private static void Postfix()
	{
		try
		{
			int num = 0;
			foreach (Settlement allTrackedPlayerSettlement in GetAllTrackedPlayerSettlements())
			{
				if (allTrackedPlayerSettlement != null && !allTrackedPlayerSettlement.IsVisible)
				{
					allTrackedPlayerSettlement.IsVisible = true;
					PartyBase party = allTrackedPlayerSettlement.Party;
					if (party != null)
					{
						party.SetVisualAsDirty();
					}
					num++;
				}
			}
			if (num > 0)
			{
				PlayerSettlementFixesLoader.LogError($"RebuiltSettlementVisibilityHealFix: found {num} tracked player settlement(s) left " + "invisible from an interrupted rebuild (the game was closed/crashed mid-placement before visibility could be restored); made visible again so they can be entered.");
			}
		}
		catch (Exception ex)
		{
			PlayerSettlementFixesLoader.LogError("RebuiltSettlementVisibilityHealFix: " + ex.Message);
		}
	}

	[IteratorStateMachine(typeof(_GetAllTrackedPlayerSettlements_d__2))]
	private static IEnumerable<Settlement> GetAllTrackedPlayerSettlements()
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _GetAllTrackedPlayerSettlements_d__2(-2);
	}

	[IteratorStateMachine(typeof(_SettlementAndNestedVillages_d__3))]
	private static IEnumerable<Settlement> SettlementAndNestedVillages(object item, HashSet<Settlement> seen)
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _SettlementAndNestedVillages_d__3(-2)
		{
			__3__item = item,
			__3__seen = seen
		};
	}
}
