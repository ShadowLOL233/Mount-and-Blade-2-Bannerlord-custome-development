using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace UnifiedTroopManager.UI
{
    // One button in the right-side Formation panel · represents Formation I..VIII.
    // IsCurrent turns on when every selected troop has this slot in its plan.
    // AssignedCount + MemberLines reflect the WHOLE party's assignment (not just
    // selection) so each Formation's full composition is always visible.
    public sealed class UTMFormationSlotVM : ViewModel
    {
        // Slot button layout constants. Must match the fixed sizes in the XML
        // (top row 36 · each member line 22 · 8-px bottom padding when non-empty).
        // ComputedHeight is driven from these so the VM and XML can't drift.
        private const int TopRowHeight = 36;
        private const int MemberLineHeight = 22;
        private const int NonEmptyBottomPad = 8;

        private readonly Action<int> _onClick;
        private string _label;
        private bool _isCurrent;
        private int _assignedCount;
        private string _assignedText;
        private MBBindingList<UTMFormationMemberLineVM> _memberLines;
        private float _computedHeight;

        public int Index { get; }

        public UTMFormationSlotVM(int index, string label, Action<int> onClick)
        {
            Index = index;
            _label = label;
            _onClick = onClick;
            _assignedCount = 0;
            _assignedText = string.Empty;
            _memberLines = new MBBindingList<UTMFormationMemberLineVM>();
            _computedHeight = TopRowHeight;
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

        // Number of troops assigned to this slot across the whole party.
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

        // One member line per troop routed to this slot · stacked vertically in the
        // XML's ItemTemplate. Replaces the earlier joined "A(x) · B(y)" single string
        // because the player asked for one-per-line layout to read compositions fast.
        [DataSourceProperty]
        public MBBindingList<UTMFormationMemberLineVM> MemberLines
        {
            get => _memberLines;
            set { if (_memberLines != value) { _memberLines = value; OnPropertyChangedWithValue(value); } }
        }

        // XML binds the slot ButtonWidget's SuggestedHeight to this. Driven explicitly
        // from SetMembers because Gauntlet does NOT propagate CoverChildren through a
        // nested ListPanel whose MBBindingList content changed · the parent widget
        // ends up locked at its initial measured height. Explicit height binding
        // forces a layout refresh every time members change.
        //   empty slot: 36 (just the label row)
        //   N members : 36 + 22*N + 8 bottom padding
        [DataSourceProperty]
        public float ComputedHeight
        {
            get => _computedHeight;
            set { if (_computedHeight != value) { _computedHeight = value; OnPropertyChangedWithValue(value); } }
        }

        public void SetAssigned(int count)
        {
            AssignedCount = count;
            AssignedText = count > 0 ? "(" + count + ")" : string.Empty;
        }

        // Rebuilds the member list in-place rather than swapping the MBBindingList
        // reference. Keeps DataSource binding stable while refreshing content, then
        // recomputes the slot's required height so the UI resizes to fit the content.
        public void SetMembers(IEnumerable<string> lines)
        {
            _memberLines.Clear();
            if (lines != null)
            {
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    _memberLines.Add(new UTMFormationMemberLineVM(line));
                }
            }
            int n = _memberLines.Count;
            ComputedHeight = TopRowHeight + n * MemberLineHeight + (n > 0 ? NonEmptyBottomPad : 0);
        }

        public void ExecuteSelect()
        {
            _onClick?.Invoke(Index);
        }
    }
}
