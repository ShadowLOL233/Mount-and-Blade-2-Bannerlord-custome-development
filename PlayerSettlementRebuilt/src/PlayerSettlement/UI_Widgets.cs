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
using Editor = TaleWorlds.GauntletUI.EditorAttribute;
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
					OnPropertyChanged<ButtonWidget>(value, "ExtendButtonWidget");
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
					OnPropertyChanged(value, "IsInfoBarExtended");
					MapBarExtendStateChangeEvent onMapInfoBarExtendStateChange = this.m_OnMapInfoBarExtendStateChange;
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
			base.OnUpdate(dt);
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
