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
using Path = System.IO.Path;
using Debug = TaleWorlds.Library.Debug;
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
			TaleWorlds.Library.Debug.SetCrashReportCustomString(e.Message);
			TaleWorlds.Library.Debug.SetCrashReportCustomStack(e.StackTrace);
			Print(e.Message, Colours.Red, isDebug: false, onlyDisplay: true);
			ToFile(new List<string> { e.Message, e.StackTrace });
		}

		public void SilentException(Exception e)
		{
			LogManager.EventTracer.Trace(e.Message, 2);
			TaleWorlds.Library.Debug.SetCrashReportCustomString(e.Message);
			TaleWorlds.Library.Debug.SetCrashReportCustomStack(e.StackTrace);
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
