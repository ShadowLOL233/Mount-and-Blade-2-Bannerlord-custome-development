using TaleWorlds.Library;

namespace UnifiedTroopManager.UI
{
    // One selectable formation slot (I..VIII) for the right-side dropdown.
    // Phase 2A · pure display; Phase 3 will wire the click into PartyPlanStore.
    public sealed class UTMFormationOptionVM : ViewModel
    {
        private string _label;
        private bool _isSelected;
        private readonly System.Action<int> _onPick;

        public int FormationIndex { get; }

        public UTMFormationOptionVM(int formationIndex, string label, bool isSelected, System.Action<int> onPick)
        {
            FormationIndex = formationIndex;
            _label = label;
            _isSelected = isSelected;
            _onPick = onPick;
        }

        [DataSourceProperty]
        public string Label
        {
            get => _label;
            set { if (_label != value) { _label = value; OnPropertyChangedWithValue(value); } }
        }

        [DataSourceProperty]
        public bool IsSelected
        {
            get => _isSelected;
            set { if (_isSelected != value) { _isSelected = value; OnPropertyChangedWithValue(value); } }
        }

        public void ExecutePick() { _onPick?.Invoke(FormationIndex); }
    }
}
