using System;
using TaleWorlds.Library;

namespace CustomBattleTroopBuilder.UI
{
    public class CBTBEditorVM : ViewModel
    {
        private readonly Action _closeAction;
        private string _title = "CBTB Editor";
        private string _bodyText = "";
        private string _closeText = "Close";

        public CBTBEditorVM(Action closeAction, int troopPoolCount)
        {
            _closeAction = closeAction;
            _bodyText =
                $"CBTB v0.1.0 · Phase 2b-1 placeholder\n\n" +
                $"Independent Gauntlet editor screen is live.\n" +
                $"Troop pool: {troopPoolCount} BasicCharacterObject\n\n" +
                $"Phase 2b-2 (next session): Custom Battle menu button + UIExt mixin\n" +
                $"Phase 3+: base troop picker + equipment / skill editor + JSON persistence";
        }

        [DataSourceProperty]
        public string Title
        {
            get => _title;
            set
            {
                if (value != _title)
                {
                    _title = value;
                    OnPropertyChangedWithValue(value, "Title");
                }
            }
        }

        [DataSourceProperty]
        public string BodyText
        {
            get => _bodyText;
            set
            {
                if (value != _bodyText)
                {
                    _bodyText = value;
                    OnPropertyChangedWithValue(value, "BodyText");
                }
            }
        }

        [DataSourceProperty]
        public string CloseText
        {
            get => _closeText;
            set
            {
                if (value != _closeText)
                {
                    _closeText = value;
                    OnPropertyChangedWithValue(value, "CloseText");
                }
            }
        }

        public void ExecuteClose()
        {
            _closeAction?.Invoke();
        }
    }
}
