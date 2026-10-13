using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.ScreenSystem;

namespace CustomBattleTroopBuilder.UI
{
    public class CBTBEditorScreen : ScreenBase
    {
        private GauntletLayer? _layer;
        private GauntletMovieIdentifier? _movie;
        private CBTBEditorVM? _vm;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _vm = new CBTBEditorVM(ClosePopScreen);
            _layer = new GauntletLayer("CBTBEditor", 1000, true);
            _layer.InputRestrictions.SetInputRestrictions();
            _layer.IsFocusLayer = true;
            _movie = _layer.LoadMovie("CBTBEditor", _vm);
            AddLayer(_layer);
            ScreenManager.TrySetFocus(_layer);
            CBTBSubModule.Log("[CBTB] CBTBEditorScreen.OnInitialize · loaded prefab · troop pool enum done");
        }

        protected override void OnFinalize()
        {
            if (_layer != null)
            {
                if (_movie != null) _layer.ReleaseMovie(_movie);
                RemoveLayer(_layer);
                _layer = null;
                _movie = null;
            }
            _vm = null;
            base.OnFinalize();
        }

        protected override void OnFrameTick(float dt)
        {
            base.OnFrameTick(dt);
            if (_layer != null && _layer.Input.IsKeyPressed(InputKey.Escape))
            {
                ClosePopScreen();
            }
        }

        private void ClosePopScreen()
        {
            CBTBSubModule.Log("[CBTB] CBTBEditorScreen Close invoked");
            ScreenManager.PopScreen();
        }
    }
}
