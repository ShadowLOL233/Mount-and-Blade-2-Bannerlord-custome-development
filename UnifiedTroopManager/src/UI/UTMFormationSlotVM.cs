using System;
using TaleWorlds.Library;

namespace UnifiedTroopManager.UI
{
    // One button in the right-side Formation panel · represents Formation I..VIII.
    // Label combines Roman numeral + FormationClass name for clarity
    // ("Formation V · Skirmisher"). IsCurrent turns on when the focused troop's
    // PlannedFormationIndex equals this slot's Index.
    public sealed class UTMFormationSlotVM : ViewModel
    {
        private readonly Action<int> _onClick;
        private string _label;
        private bool _isCurrent;
        private int _assignedCount;
        private string _assignedText;

        public int Index { get; }

        public UTMFormationSlotVM(int index, string label, Action<int> onClick)
        {
            Index = index;
            _label = label;
            _onClick = onClick;
            _assignedCount = 0;
            _assignedText = string.Empty;
        }

        [DataSourceProperty]
        public string Label
        {
            get => _label;
            set { if (_label != value) { _label = value; OnPropertyChangedWithValue(value); } }
        }

        [DataSourceProperty]
        public bool IsCurrent
        {
            get => _isCurrent;
            set { if (_isCurrent != value) { _isCurrent = value; OnPropertyChangedWithValue(value); } }
        }

        // Number of troops assigned to this slot for the focused troop (split share).
        // 0 when not part of the plan.
        [DataSourceProperty]
        public int AssignedCount
        {
            get => _assignedCount;
            set { if (_assignedCount != value) { _assignedCount = value; OnPropertyChangedWithValue(value); } }
        }

        [DataSourceProperty]
        public string AssignedText
        {
            get => _assignedText;
            set { if (_assignedText != value) { _assignedText = value; OnPropertyChangedWithValue(value); } }
        }

        public void SetAssigned(int count)
        {
            AssignedCount = count;
            AssignedText = count > 0 ? "(" + count + ")" : string.Empty;
        }

        public void ExecuteSelect()
        {
            _onClick?.Invoke(Index);
        }
    }
}
