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
using GameLoadingType = TaleWorlds.CampaignSystem.Campaign.GameLoadingType;
using Debug = TaleWorlds.Library.Debug;
using Path = System.IO.Path;
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
			base.DefineClassTypes();
			AddClassDefinition(typeof(Mat3Saveable), 9, (IObjectResolver)null);
			AddClassDefinition(typeof(Vec3Saveable), 10, (IObjectResolver)null);
			AddClassDefinition(typeof(TransformSaveable), 11, (IObjectResolver)null);
			AddClassDefinition(typeof(DeepTransformEdit), 12, (IObjectResolver)null);
			AddClassDefinition(typeof(PlayerSettlementItem), 3, (IObjectResolver)null);
			AddClassDefinition(typeof(OverwriteSettlementItem), 13, (IObjectResolver)null);
			AddClassDefinition(typeof(PlayerSettlementInfo), 2, (IObjectResolver)null);
			AddClassDefinition(typeof(SettlementMetaV3), 5, (IObjectResolver)null);
			AddClassDefinition(typeof(MetaV3), 7, (IObjectResolver)null);
		}

		protected override void DefineContainerDefinitions()
		{
			base.DefineContainerDefinitions();
			ConstructContainerDefinition(typeof(List<DeepTransformEdit>));
			ConstructContainerDefinition(typeof(DeepTransformEdit[]));
			ConstructContainerDefinition(typeof(List<PlayerSettlementItem>));
			ConstructContainerDefinition(typeof(PlayerSettlementItem[]));
			ConstructContainerDefinition(typeof(List<OverwriteSettlementItem>));
			ConstructContainerDefinition(typeof(OverwriteSettlementItem[]));
			ConstructContainerDefinition(typeof(List<SettlementMetaV3>));
			ConstructContainerDefinition(typeof(SettlementMetaV3[]));
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
