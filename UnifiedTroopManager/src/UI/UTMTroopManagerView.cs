using System;
using SandBox.View.Menu;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;
using UnifiedTroopManager.Data;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.UI
{
    // Phase 2B · Manage Troops view · hosted inside the encounter MenuViewContext.
    // See [[bannerlord-menu-view]] memory for the architecture rationale — ScreenBase +
    // PushScreen is banned because it races with the underlying menu and native-crashes.
    public sealed class UTMTroopManagerView : MenuView
    {
        private const string MovieName = "UTMTroopManager";

        private TroopRoster _sourceRoster;
        private RosterSelection _priorSelection;
        private UTMTroopManagerVM _vm;
        private GauntletLayer _layer;
        private bool _closing;

        // Required to satisfy `where T : new()` on AddMenuView<T>; SandBoxViewCreator
        // resolves the real ctor from the params passed to AddMenuView so this is dead
        // code — throwing makes accidental use loud.
        public UTMTroopManagerView()
        {
            throw new NotImplementedException(
                "UTMTroopManagerView must be created via MenuViewContext.AddMenuView<T>(args).");
        }

        public UTMTroopManagerView(TroopRoster sourceRoster, RosterSelection priorSelection)
        {
            _sourceRoster = sourceRoster;
            _priorSelection = priorSelection;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            try
            {
                _vm = new UTMTroopManagerVM(_sourceRoster, _priorSelection, RequestClose);
                _layer = new GauntletLayer("UTMManageTroops", 210);
                // Match CYT's OnInitialize sequence: input restrictions + hotkey categories
                // MUST be set before LoadMovie · otherwise button click events don't route
                // to the loaded VM commands (only ESC keeps working since it reads global
                // InputManager). Skipping RegisterHotKeyCategory was why Reset/±/Save
                // buttons in Phase 2B Deploy 1 were unresponsive.
                _layer.InputRestrictions.SetInputRestrictions();
                _layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
                _layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
                _layer.LoadMovie(MovieName, _vm);
                _layer.IsFocusLayer = true;
                ScreenManager.TrySetFocus(_layer);
                MenuViewContext.AddLayer(_layer);
                UTMLog.Info("UTMTroopManagerView · OnInitialize · roster=" +
                    (_sourceRoster != null ? _sourceRoster.Count : 0) + " types");
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMTroopManagerView.OnInitialize", ex);
                InformationManager.DisplayMessage(new InformationMessage(
                    "[UTM] Manage Troops failed to open: " + ex.Message, Colors.Red));
                RequestClose();
            }
        }

        protected override void OnFinalize()
        {
            try
            {
                if (_layer != null)
                {
                    _layer.InputRestrictions.ResetInputRestrictions();
                    _layer.IsFocusLayer = false;
                    MenuViewContext?.RemoveLayer(_layer);
                    _layer = null;
                }
                _vm = null;
                UTMLog.Info("UTMTroopManagerView · OnFinalize");
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMTroopManagerView.OnFinalize", ex);
            }
            base.OnFinalize();
        }

        protected override void OnFrameTick(float dt)
        {
            base.OnFrameTick(dt);
            try
            {
                if (_closing) return;
                if (Input.IsKeyReleased(InputKey.Escape))
                {
                    UTMLog.Info("UTMTroopManagerView · ESC · closing");
                    RequestClose();
                }
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMTroopManagerView.OnFrameTick", ex);
                RequestClose();
            }
        }

        private void RequestClose()
        {
            if (_closing) return;
            _closing = true;
            try
            {
                MenuViewContext?.RemoveMenuView(this);
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMTroopManagerView.RequestClose", ex);
            }
        }
    }
}
