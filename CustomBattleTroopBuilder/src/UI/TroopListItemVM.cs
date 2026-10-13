using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CustomBattleTroopBuilder.UI
{
    public class TroopListItemVM : ViewModel
    {
        public BasicCharacterObject Character { get; }

        public string StringId { get; }
        public string DisplayName { get; }
        public string CultureId { get; }
        public int Tier { get; }
        public int Level { get; }
        public bool IsSoldier { get; }

        private readonly Action<TroopListItemVM>? _onSelect;

        private string _summaryLine = "";
        private bool _isSelected;

        public TroopListItemVM(BasicCharacterObject character, Action<TroopListItemVM>? onSelect = null)
        {
            Character = character;
            _onSelect = onSelect;

            StringId = character.StringId ?? "<null>";
            DisplayName = character.Name?.ToString() ?? StringId;
            CultureId = character.Culture?.StringId ?? "neutral";
            Level = character.Level;
            IsSoldier = character.IsSoldier;
            try { Tier = character.GetBattleTier(); } catch { Tier = 0; }

            var role =
                character.IsMounted ? "cav" :
                character.IsRanged  ? "rng" :
                                      "inf";
            _summaryLine = $"[T{Tier} · L{Level} · {CultureId} · {role}]  {DisplayName}  ({StringId})";
        }

        [DataSourceProperty]
        public string SummaryLine
        {
            get => _summaryLine;
            set
            {
                if (value != _summaryLine)
                {
                    _summaryLine = value;
                    OnPropertyChangedWithValue(value, "SummaryLine");
                }
            }
        }

        [DataSourceProperty]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChangedWithValue(value, "IsSelected");
                }
            }
        }

        public void ExecuteSelect()
        {
            _onSelect?.Invoke(this);
        }
    }
}
