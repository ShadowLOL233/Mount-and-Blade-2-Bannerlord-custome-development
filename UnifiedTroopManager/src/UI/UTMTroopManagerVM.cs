using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using UnifiedTroopManager.Data;
using UnifiedTroopManager.Util;

namespace UnifiedTroopManager.UI
{
    // Root VM · Phase 2B Deploy B1. Adds:
    //   · FocusedTroop + FormationSlots (8) for the right-side Formation panel
    //   · ExecuteSelectFormation · click a Formation slot to assign planned formation
    //   · Save logs both roster and formation plans (persistence in Deploy B2)
    public sealed class UTMTroopManagerVM : ViewModel
    {
        private enum SortMode { Default, TierDesc, TierAsc, TypeThenTierDesc }

        private readonly Action _onClose;
        private readonly TroopRoster _sourceRoster;

        private MBBindingList<UTMTroopEntryVM> _troops;
        private MBBindingList<UTMFormationSlotVM> _formationSlots;
        // Multi-select mode · click a row toggles it into/out of this set.
        // Right-panel Formation buttons operate on ALL entries in the set at once.
        private readonly HashSet<UTMTroopEntryVM> _selectedTroops = new HashSet<UTMTroopEntryVM>();
        private string _headerText;
        private string _totalText;
        private string _sortLabel;
        private string _formationPanelHeader;
        private string _formationDistributionSummary;
        private bool _formationPanelEnabled;
        private int _selectedTotal;
        private SortMode _sortMode;

        public UTMTroopManagerVM(TroopRoster sourceRoster, RosterSelection currentSelection, Action onClose)
        {
            _onClose = onClose;
            _sourceRoster = sourceRoster;
            _troops = new MBBindingList<UTMTroopEntryVM>();
            _formationSlots = new MBBindingList<UTMFormationSlotVM>();
            _headerText = "Manage Troops";
            _totalText = "Total: 0";
            _sortLabel = "Sort: default";
            _formationPanelHeader = "Select a troop on the left";
            _formationDistributionSummary = string.Empty;
            _formationPanelEnabled = false;
            _sortMode = SortMode.Default;
            BuildEntries(currentSelection);
            BuildFormationSlots();
            RefreshTotal();
            // Auto-select the first troop so the Formation panel has meaningful content
            // out of the box. Additional troops are added by clicking their rows.
            if (_troops.Count > 0)
                OnTroopFocused(_troops[0]);
        }

        [DataSourceProperty]
        public MBBindingList<UTMTroopEntryVM> Troops
        {
            get => _troops;
            set { if (_troops != value) { _troops = value; OnPropertyChangedWithValue(value); } }
        }

        [DataSourceProperty]
        public MBBindingList<UTMFormationSlotVM> FormationSlots
        {
            get => _formationSlots;
            set { if (_formationSlots != value) { _formationSlots = value; OnPropertyChangedWithValue(value); } }
        }

        [DataSourceProperty]
        public string HeaderText
        {
            get => _headerText;
            set { if (_headerText != value) { _headerText = value; OnPropertyChangedWithValue(value); } }
        }

        [DataSourceProperty]
        public string TotalText
        {
            get => _totalText;
            set { if (_totalText != value) { _totalText = value; OnPropertyChangedWithValue(value); } }
        }

        [DataSourceProperty]
        public string SortLabel
        {
            get => _sortLabel;
            set { if (_sortLabel != value) { _sortLabel = value; OnPropertyChangedWithValue(value); } }
        }

        [DataSourceProperty]
        public string FormationPanelHeader
        {
            get => _formationPanelHeader;
            set { if (_formationPanelHeader != value) { _formationPanelHeader = value; OnPropertyChangedWithValue(value); } }
        }

        [DataSourceProperty]
        public string FormationDistributionSummary
        {
            get => _formationDistributionSummary;
            set { if (_formationDistributionSummary != value) { _formationDistributionSummary = value; OnPropertyChangedWithValue(value); } }
        }

        [DataSourceProperty]
        public bool FormationPanelEnabled
        {
            get => _formationPanelEnabled;
            set { if (_formationPanelEnabled != value) { _formationPanelEnabled = value; OnPropertyChangedWithValue(value); } }
        }

        // --- Commands wired from XML ---
        public void ExecuteReset()
        {
            foreach (var entry in _troops) entry.SetBringSilent(entry.MaxAvailable);
            RefreshTotal();
            UTMLog.Info("UI · Reset · all set to Max");
        }

        public void ExecuteAllZero()
        {
            foreach (var entry in _troops) entry.SetBringSilent(0);
            RefreshTotal();
            UTMLog.Info("UI · All Zero");
        }

        public void ExecuteAllMax()
        {
            foreach (var entry in _troops) entry.SetBringSilent(entry.MaxAvailable);
            RefreshTotal();
            UTMLog.Info("UI · All Max");
        }

        public void ExecuteSortByTier()
        {
            _sortMode = _sortMode == SortMode.TierDesc ? SortMode.TierAsc : SortMode.TierDesc;
            ApplySort();
            SortLabel = _sortMode == SortMode.TierDesc ? "Sort: Tier ↓" : "Sort: Tier ↑";
        }

        public void ExecuteSortByType()
        {
            _sortMode = SortMode.TypeThenTierDesc;
            ApplySort();
            SortLabel = "Sort: Type (Inf/Skir/Rng/Cav)";
        }

        public void ExecuteCancel()
        {
            UTMLog.Info("UI · Cancel · discarding selection");
            _onClose?.Invoke();
        }

        public void ExecuteSave()
        {
            var selection = new RosterSelection();
            int planCount = 0;
            foreach (var entry in _troops)
            {
                if (entry.Bring > 0)
                    selection.Counts[entry.TroopStringId] = entry.Bring;
                if (entry.PlannedFormations.Count > 0)
                {
                    planCount++;
                    var joined = string.Join(",",
                        entry.PlannedFormations
                            .OrderBy(x => x)
                            .Select(FormationRoman));
                    UTMLog.Info("Plan · " + entry.TroopStringId + " → " + joined);
                }
            }
            RosterSelection.SetCurrent(selection);
            UTMLog.Info("UI · Save · " + selection.Counts.Count + " types selected · total=" + _selectedTotal
                + " · " + planCount + " formation plans");
            _onClose?.Invoke();
        }

        // Batch-mode formation toggle across the whole selection. Semantic:
        //   · If ALL selected troops already have this formation → remove from all
        //   · Otherwise → add to all (a partial state resolves to "everyone has it")
        // This keeps the selection consistent, so a second click always removes.
        private void OnFormationSlotClicked(int formationIndex)
        {
            if (_selectedTroops.Count == 0)
            {
                UTMLog.Warn("Formation click ignored · no troops selected");
                return;
            }
            bool allHave = _selectedTroops.All(t => t.HasFormation(formationIndex));
            if (allHave)
            {
                foreach (var t in _selectedTroops) t.RemoveFormation(formationIndex);
                UTMLog.Info("Batch · Formation " + FormationRoman(formationIndex) + " removed from " + _selectedTroops.Count);
            }
            else
            {
                foreach (var t in _selectedTroops) t.AddFormation(formationIndex);
                UTMLog.Info("Batch · Formation " + FormationRoman(formationIndex) + " added to " + _selectedTroops.Count);
            }
            RefreshFormationSlotHighlights();
        }

        // --- Row click · toggle troop in/out of selection set ---
        private void OnTroopFocused(UTMTroopEntryVM entry)
        {
            if (entry == null) return;
            if (_selectedTroops.Contains(entry))
            {
                _selectedTroops.Remove(entry);
                entry.IsFocused = false;
            }
            else
            {
                _selectedTroops.Add(entry);
                entry.IsFocused = true;
            }
            UpdatePanelHeader();
            RefreshFormationSlotHighlights();
        }

        private void UpdatePanelHeader()
        {
            int n = _selectedTroops.Count;
            if (n == 0)
            {
                FormationPanelHeader = "Select troops on the left";
                FormationPanelEnabled = false;
            }
            else if (n == 1)
            {
                FormationPanelHeader = "Formation for: " + _selectedTroops.First().TroopName;
                FormationPanelEnabled = true;
            }
            else
            {
                FormationPanelHeader = "Batch edit · " + n + " troops selected";
                FormationPanelEnabled = true;
            }
        }

        // Same-row count change · refresh split counts + distribution summary too.

        // --- internals ---
        private void BuildEntries(RosterSelection prior)
        {
            _troops.Clear();
            if (_sourceRoster == null) return;

            try
            {
                for (int i = 0; i < _sourceRoster.Count; i++)
                {
                    var el = _sourceRoster.GetElementCopyAtIndex(i);
                    if (el.Character == null) continue;
                    if (el.Character.IsHero) continue;
                    int healthy = el.Number - el.WoundedNumber;
                    if (healthy <= 0) continue;

                    int initialBring = healthy;
                    if (prior != null && prior.Counts.TryGetValue(el.Character.StringId, out int stored))
                        initialBring = Math.Min(stored, healthy);

                    _troops.Add(new UTMTroopEntryVM(
                        el.Character,
                        inParty: healthy,
                        initialBring: initialBring,
                        onCountChanged: OnEntryCountChanged,
                        onFocus: OnTroopFocused));
                }
                UTMLog.Info("UI · built " + _troops.Count + " troop entries from roster");
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMTroopManagerVM.BuildEntries", ex);
            }
        }

        private void BuildFormationSlots()
        {
            _formationSlots.Clear();
            for (int i = 0; i < 8; i++)
            {
                int idx = i;
                // Just "Formation I" · "Formation II" · etc. No class-name suffix — the
                // Infantry/Ranged/Cavalry etc. names refer to vanilla DefaultFormationClass
                // (a character property) not to what each slot IS · so the suffix misled
                // users into thinking slots were type-locked. They aren't · assign any
                // troop to any slot.
                _formationSlots.Add(new UTMFormationSlotVM(
                    idx,
                    "Formation " + FormationRoman(idx),
                    OnFormationSlotClicked));
            }
        }

        private void RefreshFormationSlotHighlights()
        {
            foreach (var slot in _formationSlots)
            {
                // Highlight only when every selected troop has this formation.
                bool allHave = _selectedTroops.Count > 0
                    && _selectedTroops.All(t => t.HasFormation(slot.Index));
                slot.IsCurrent = allHave;
                // Count = total troops routed to this formation across the selection.
                int sum = 0;
                foreach (var t in _selectedTroops) sum += t.GetSplitCount(slot.Index);
                slot.SetAssigned(sum);
            }
            RefreshFormationDistributionSummary();
        }

        private void RefreshFormationDistributionSummary()
        {
            int n = _selectedTroops.Count;
            if (n == 0)
            {
                FormationDistributionSummary = string.Empty;
                return;
            }
            if (n == 1)
            {
                var only = _selectedTroops.First();
                int pcount = only.PlannedFormations.Count;
                if (pcount == 0)
                    FormationDistributionSummary = only.Bring + " troops · no formation plan (vanilla default)";
                else if (pcount == 1)
                    FormationDistributionSummary = only.Bring + " troops in 1 formation";
                else
                    FormationDistributionSummary = only.Bring + " troops split across " + pcount + " formations";
            }
            else
            {
                int totalBring = _selectedTroops.Sum(t => t.Bring);
                FormationDistributionSummary = n + " troop types selected · " + totalBring + " total troops";
            }
        }

        private void ApplySort()
        {
            var list = new List<UTMTroopEntryVM>();
            foreach (var e in _troops) list.Add(e);

            Comparison<UTMTroopEntryVM> cmp;
            switch (_sortMode)
            {
                case SortMode.TierDesc:
                    cmp = (a, b) =>
                    {
                        int t = b.TierSortKey.CompareTo(a.TierSortKey);
                        return t != 0 ? t : string.Compare(a.TroopName, b.TroopName, StringComparison.OrdinalIgnoreCase);
                    };
                    break;
                case SortMode.TierAsc:
                    cmp = (a, b) =>
                    {
                        int t = a.TierSortKey.CompareTo(b.TierSortKey);
                        return t != 0 ? t : string.Compare(a.TroopName, b.TroopName, StringComparison.OrdinalIgnoreCase);
                    };
                    break;
                case SortMode.TypeThenTierDesc:
                    cmp = (a, b) =>
                    {
                        int t = a.TypeSortKey.CompareTo(b.TypeSortKey);
                        if (t != 0) return t;
                        int tier = b.TierSortKey.CompareTo(a.TierSortKey);
                        return tier != 0 ? tier : string.Compare(a.TroopName, b.TroopName, StringComparison.OrdinalIgnoreCase);
                    };
                    break;
                default:
                    return;
            }
            list.Sort(cmp);

            _troops.Clear();
            foreach (var e in list) _troops.Add(e);
        }

        private void OnEntryCountChanged(UTMTroopEntryVM entry)
        {
            RefreshTotal();
            // If the changed row is in the current selection, the distribution across
            // formations changes too · refresh the right-panel numbers.
            if (_selectedTroops.Contains(entry))
                RefreshFormationSlotHighlights();
        }

        private void RefreshTotal()
        {
            int total = 0;
            foreach (var entry in _troops) total += entry.Bring;
            _selectedTotal = total;
            TotalText = "Total selected: " + total;
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

    }
}
