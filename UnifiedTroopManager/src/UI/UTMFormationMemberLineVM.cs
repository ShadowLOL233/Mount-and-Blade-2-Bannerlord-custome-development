using TaleWorlds.Library;

namespace UnifiedTroopManager.UI
{
    // One line inside a Formation slot's expanded member list · "Hoplite(30)".
    // Needs to be a ViewModel (not a bare string) because MBBindingList + ItemTemplate
    // in Gauntlet requires each item to expose DataSourceProperty-tagged bindings.
    public sealed class UTMFormationMemberLineVM : ViewModel
    {
        private string _lineText;

        public UTMFormationMemberLineVM(string lineText)
        {
            _lineText = lineText ?? string.Empty;
        }

        [DataSourceProperty]
        public string LineText
        {
            get => _lineText;
            set { if (_lineText != value) { _lineText = value; OnPropertyChangedWithValue(value); } }
        }
    }
}
