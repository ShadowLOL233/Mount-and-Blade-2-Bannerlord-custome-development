using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;

namespace UnifiedTroopManager.UI
{
    // One row in the Manage Troops roster list · Phase 2B real data.
    // Backed by a CharacterObject from MobileParty.MainParty.MemberRoster.
    // Provides pre-computed sort keys for the VM's Tier / Type sort commands.
    public sealed class UTMTroopEntryVM : ViewModel
    {
        private readonly Action<UTMTroopEntryVM> _onCountChanged;
        private readonly Action<UTMTroopEntryVM> _onFocus;

        private CharacterImageIdentifierVM _visual;
        private string _troopName;
        private string _tierLabel;
        private int _bring;
        private int _maxAvailable;
        private string _countText;
        private string _planSummary;
        private readonly HashSet<int> _plannedFormations = new HashSet<int>();
        private bool _isFocused;

        public string TroopStringId { get; }
        public CharacterObject Character { get; }
        public int TierSortKey { get; }
        public int TypeSortKey { get; }
        // Hero rows are displayed in the roster list (added 2026-10-07) and are
        // pinned to the top by every sort mode. QuotaEnforcer bypasses hero
        // StringId checks so even bring=0 hero still spawns — the UI toggle on
        // hero rows is informational, not enforceable.
        public bool IsHero => Character != null && Character.IsHero;
        // Empty set = no plan (troop takes vanilla DefaultFormationClass).
        // 1 element = single-formation plan · 2+ elements = split plan (equal weights).
        public IReadOnlyCollection<int> PlannedFormations => _plannedFormations;

        public UTMTroopEntryVM(
            CharacterObject character,
            int inParty,
            int initialBring,
            Action<UTMTroopEntryVM> onCountChanged,
            Action<UTMTroopEntryVM> onFocus)
        {
            Character = character;
            TroopStringId = character.StringId;
            _visual = new CharacterImageIdentifierVM(CharacterCode.CreateFrom(character));
            _troopName = character.Name?.ToString() ?? character.StringId;
            _tierLabel = character.IsHero ? "Hero" : ("T" + character.Tier);
            _maxAvailable = Math.Max(0, inParty);
            _bring = Math.Max(0, Math.Min(initialBring, _maxAvailable));
            _onCountChanged = onCountChanged;
            _onFocus = onFocus;
            TierSortKey = character.Tier;
            TypeSortKey = ComputeTypeSortKey(character);
            RefreshCountText();
            RefreshPlanSummary();
        }

        // User asked for: 步兵 → Skirmisher → 弓兵 → 骑兵.
        // Map FormationClass → 4 buckets. Ties broken by tier desc then name in the VM sort.
        private static int ComputeTypeSortKey(CharacterObject c)
        {
            var fc = c.DefaultFormationClass;
            switch (fc)
            {
                case FormationClass.Infantry:
                case FormationClass.HeavyInfantry:
                    return 0;
                case FormationClass.Skirmisher:
                    return 1;
                case FormationClass.Ranged:
                    return 2;
                case FormationClass.Cavalry:
                case FormationClass.LightCavalry:
                case FormationClass.HeavyCavalry:
                case FormationClass.HorseArcher:
                    return 3;
                default:
                    return 4;
            }
        }

        [DataSourceProperty]
        public CharacterImageIdentifierVM Visual
        {
            get => _visual;
            set { if (_visual != value) { _visual = value; OnPropertyChangedWithValue(value); } }
        }

        [DataSourceProperty]
        public string TroopName
        {
            get => _troopName;
            set { if (_troopName != value) { _troopName = value; OnPropertyChangedWithValue(value); } }
        }

        [DataSourceProperty]
        public string TierLabel
        {
            get => _tierLabel;
            set { if (_tierLabel != value) { _tierLabel = value; OnPropertyChangedWithValue(value); } }
        }

        // Bring setter is written to by both button clicks (via SetBring) AND by the
        // SliderWidget's ValueInt two-way binding when the user drags the handle. In
        // either case we fire onCountChanged so the parent VM's Total refreshes.
        // SetBringSilent (used for batch All Zero / All Max) bypasses this via direct
        // field assignment to avoid O(N) Total recomputes during bulk operations.
        [DataSourceProperty]
        public int Bring
        {
            get => _bring;
            set
            {
                int clamped = Math.Max(0, Math.Min(_maxAvailable, value));
                if (_bring == clamped) return;
                _bring = clamped;
                OnPropertyChangedWithValue(clamped);
                RefreshCountText();
                _onCountChanged?.Invoke(this);
            }
        }

        [DataSourceProperty]
        public int MaxAvailable
        {
            get => _maxAvailable;
            set { if (_maxAvailable != value) { _maxAvailable = value; OnPropertyChangedWithValue(value); RefreshCountText(); } }
        }

        [DataSourceProperty]
        public string CountText
        {
            get => _countText;
            set { if (_countText != value) { _countText = value; OnPropertyChangedWithValue(value); } }
        }

        // For a visual progress bar (FillBar) showing Bring / MaxAvailable ratio.
        // Kept as int so FillBar's InitialAmount + MaxAmount attribute bindings can pull
        // integers directly · no float precision issues for typical roster sizes.
        [DataSourceProperty]
        public int BringForBar
        {
            get => _bring;
        }

        [DataSourceProperty]
        public bool IsFocused
        {
            get => _isFocused;
            set { if (_isFocused != value) { _isFocused = value; OnPropertyChangedWithValue(value); } }
        }

        [DataSourceProperty]
        public string PlanSummary
        {
            get => _planSummary;
            set { if (_planSummary != value) { _planSummary = value; OnPropertyChangedWithValue(value); } }
        }

        // Button click targets · bound via Command.Click in the ItemTemplate.
        public void ExecuteIncrement()     { SetBring(_bring + 1); }
        public void ExecuteDecrement()     { SetBring(_bring - 1); }
        public void ExecuteIncrementFive() { SetBring(_bring + 5); }
        public void ExecuteDecrementFive() { SetBring(_bring - 5); }
        public void ExecuteMax()           { SetBring(_maxAvailable); }
        public void ExecuteZero()          { SetBring(0); }

        // Right-panel focus · called by "Fmt" button in the row.
        public void ExecuteFocus() { _onFocus?.Invoke(this); }

        // Toggle Formation membership · called by parent VM when user clicks a
        // Formation slot in the right panel. Multi-select allowed (split mode).
        public bool HasFormation(int formationIndex) => _plannedFormations.Contains(formationIndex);

        public void ToggleFormation(int formationIndex)
        {
            if (!_plannedFormations.Remove(formationIndex))
                _plannedFormations.Add(formationIndex);
            RefreshPlanSummary();
        }

        // Explicit add / remove — used by the multi-select "batch edit" mode where
        // the parent VM enforces consistent state across the whole selection.
        public bool AddFormation(int formationIndex)
        {
            if (!_plannedFormations.Add(formationIndex)) return false;
            RefreshPlanSummary();
            return true;
        }

        public bool RemoveFormation(int formationIndex)
        {
            if (!_plannedFormations.Remove(formationIndex)) return false;
            RefreshPlanSummary();
            return true;
        }

        // Wipe every planned formation · used by UTMTroopManagerVM.ExecuteReset
        // so Reset actually resets the whole row (count + plan) rather than
        // leaving the previous preset's formation assignments stuck.
        public void ClearPlannedFormations()
        {
            if (_plannedFormations.Count == 0) return;
            _plannedFormations.Clear();
            RefreshPlanSummary();
        }

        // Integer split of Bring across all planned formations · remainder goes to
        // the lowest-indexed formations first (100 across I·II·III = 34/33/33).
        // Returns 0 if this formation isn't in the plan.
        public int GetSplitCount(int formationIndex)
        {
            if (!_plannedFormations.Contains(formationIndex)) return 0;
            int n = _plannedFormations.Count;
            if (n == 0) return 0;
            int baseCount = _bring / n;
            int remainder = _bring % n;
            var sorted = _plannedFormations.OrderBy(x => x).ToList();
            int rank = sorted.IndexOf(formationIndex);
            return baseCount + (rank < remainder ? 1 : 0);
        }

        private void RefreshPlanSummary()
        {
            if (_plannedFormations.Count == 0)
            {
                PlanSummary = "→ Default";
            }
            else if (_plannedFormations.Count == 1)
            {
                int only = _plannedFormations.First();
                PlanSummary = "→ " + FormationRoman(only) + "(" + _bring + ")";
            }
            else
            {
                var parts = _plannedFormations
                    .OrderBy(x => x)
                    .Select(i => FormationRoman(i) + "(" + GetSplitCount(i) + ")");
                PlanSummary = "→ " + string.Join(" ", parts);
            }
        }

        private static string FormationRoman(int i)
        {
            switch (i)
            {
                case 0: return "I";  case 1: return "II"; case 2: return "III"; case 3: return "IV";
                case 4: return "V";  case 5: return "VI"; case 6: return "VII"; case 7: return "VIII";
                default: return "?";
            }
        }

        // Batch set that skips the onCountChanged callback · parent RefreshTotal once.
        public void SetBringSilent(int n)
        {
            int clamped = Math.Max(0, Math.Min(_maxAvailable, n));
            if (clamped == _bring) return;
            _bring = clamped;
            OnPropertyChanged(nameof(Bring));
            RefreshCountText();
        }

        // Button click path · Bring setter handles clamp + notify + onCountChanged.
        private void SetBring(int n) { Bring = n; }

        private void RefreshCountText()
        {
            CountText = _bring + " / " + _maxAvailable;
            OnPropertyChanged(nameof(BringForBar));
        }
    }
}
