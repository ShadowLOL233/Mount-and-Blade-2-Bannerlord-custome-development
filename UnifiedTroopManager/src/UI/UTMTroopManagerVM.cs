using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using UnifiedTroopManager.Data;
using UnifiedTroopManager.Patches;
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
        private MBBindingList<UTMPresetItemVM> _presets;
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
        // Preset dialog state · bound to IsVisible on the overlay widgets in XML.
        // Only one dialog can be open at a time; opening Save closes Load and vice
        // versa (handled in the Open* commands).
        private bool _isSaveDialogOpen;
        private bool _isLoadDialogOpen;
        private string _pendingPresetName;
        private string _loadDialogStatus;

        public UTMTroopManagerVM(TroopRoster sourceRoster, RosterSelection currentSelection, BattlePlan currentPlan, Action onClose)
        {
            _onClose = onClose;
            _sourceRoster = sourceRoster;
            _troops = new MBBindingList<UTMTroopEntryVM>();
            _formationSlots = new MBBindingList<UTMFormationSlotVM>();
            _presets = new MBBindingList<UTMPresetItemVM>();
            _headerText = "Manage Troops";
            _totalText = "Total: 0";
            _sortLabel = "Sort: default";
            _formationPanelHeader = "Select a troop on the left";
            _formationDistributionSummary = string.Empty;
            _formationPanelEnabled = false;
            _sortMode = SortMode.Default;
            _pendingPresetName = string.Empty;
            _loadDialogStatus = string.Empty;
            BuildEntries(currentSelection, currentPlan);
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
        public MBBindingList<UTMPresetItemVM> Presets
        {
            get => _presets;
            set { if (_presets != value) { _presets = value; OnPropertyChangedWithValue(value); } }
        }

        [DataSourceProperty]
        public bool IsSaveDialogOpen
        {
            get => _isSaveDialogOpen;
            set { if (_isSaveDialogOpen != value) { _isSaveDialogOpen = value; OnPropertyChangedWithValue(value); } }
        }

        [DataSourceProperty]
        public bool IsLoadDialogOpen
        {
            get => _isLoadDialogOpen;
            set { if (_isLoadDialogOpen != value) { _isLoadDialogOpen = value; OnPropertyChangedWithValue(value); } }
        }

        // Two-way bound to the Save dialog's EditableTextWidget so the user's typing
        // round-trips into the VM. ExecuteConfirmSavePreset reads this value when the
        // Save button is clicked.
        [DataSourceProperty]
        public string PendingPresetName
        {
            get => _pendingPresetName;
            set { if (_pendingPresetName != value) { _pendingPresetName = value; OnPropertyChangedWithValue(value); } }
        }

        [DataSourceProperty]
        public string LoadDialogStatus
        {
            get => _loadDialogStatus;
            set { if (_loadDialogStatus != value) { _loadDialogStatus = value; OnPropertyChangedWithValue(value); } }
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

        // === Preset: Save / Load ================================================
        // Both Save and Load now open an inline dialog overlay (independent widget
        // panels in the same prefab, toggled via IsSaveDialogOpen / IsLoadDialogOpen)
        // instead of the vanilla TextInquiry / MultiSelectionInquiry popups. Keeps
        // the mod's visual language consistent with the rest of UTM and lets the
        // player manage presets without leaving the Manage Troops screen.
        //
        // Dialog state is mutually exclusive — opening one closes the other.

        public void ExecuteSavePreset()
        {
            try
            {
                PendingPresetName = string.Empty;
                IsLoadDialogOpen = false;
                IsSaveDialogOpen = true;
            }
            catch (Exception ex)
            {
                UTMLog.Exception("ExecuteSavePreset", ex);
            }
        }

        public void ExecuteCancelSavePreset()
        {
            IsSaveDialogOpen = false;
            PendingPresetName = string.Empty;
        }

        // Save button inside the Save dialog · writes current state to disk and
        // closes the dialog. Empty / whitespace names are silently ignored so the
        // dialog stays open and the player can finish typing.
        public void ExecuteConfirmSavePreset()
        {
            try
            {
                var name = (PendingPresetName ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(name))
                {
                    InformationManager.DisplayMessage(new InformationMessage("[UTM] Preset name cannot be empty."));
                    return;
                }
                var preset = BuildPresetFromCurrentState(name);
                if (UTMPresetStore.Save(preset))
                {
                    InformationManager.DisplayMessage(new InformationMessage(
                        "[UTM] Preset '" + name + "' saved · " + preset.Roster.Count + " troops"));
                    IsSaveDialogOpen = false;
                    PendingPresetName = string.Empty;
                }
                else
                {
                    InformationManager.DisplayMessage(new InformationMessage(
                        "[UTM] Failed to save preset '" + name + "' (see log)"));
                }
            }
            catch (Exception ex)
            {
                UTMLog.Exception("ExecuteConfirmSavePreset", ex);
            }
        }

        public void ExecuteLoadPreset()
        {
            try
            {
                IsSaveDialogOpen = false;
                RefreshPresetList();
                IsLoadDialogOpen = true;
            }
            catch (Exception ex)
            {
                UTMLog.Exception("ExecuteLoadPreset", ex);
            }
        }

        public void ExecuteCloseLoadDialog()
        {
            IsLoadDialogOpen = false;
        }

        private void RefreshPresetList()
        {
            _presets.Clear();
            var items = UTMPresetStore.LoadAll();
            foreach (var p in items)
                _presets.Add(new UTMPresetItemVM(p, OnPresetItemLoad, OnPresetItemDelete));
            LoadDialogStatus = items.Count == 0
                ? "No saved presets yet. Save one from the main screen first."
                : items.Count + " preset(s)";
        }

        private void OnPresetItemLoad(UTMPresetItemVM item)
        {
            try
            {
                if (item == null || item.RawPreset == null) return;
                ApplyPreset(item.RawPreset);
                IsLoadDialogOpen = false;
            }
            catch (Exception ex)
            {
                UTMLog.Exception("OnPresetItemLoad", ex);
            }
        }

        private void OnPresetItemDelete(UTMPresetItemVM item)
        {
            try
            {
                if (item == null || item.RawPreset == null) return;
                if (UTMPresetStore.Delete(item.RawPreset.Name))
                {
                    _presets.Remove(item);
                    LoadDialogStatus = _presets.Count == 0
                        ? "No saved presets yet. Save one from the main screen first."
                        : _presets.Count + " preset(s)";
                    InformationManager.DisplayMessage(new InformationMessage(
                        "[UTM] Preset '" + item.RawPreset.Name + "' deleted"));
                }
            }
            catch (Exception ex)
            {
                UTMLog.Exception("OnPresetItemDelete", ex);
            }
        }

        private UTMPreset BuildPresetFromCurrentState(string name)
        {
            var preset = new UTMPreset
            {
                Name = name,
                Created = DateTime.Now
            };
            foreach (var entry in _troops)
            {
                if (entry.Bring > 0)
                    preset.Roster[entry.TroopStringId] = entry.Bring;
                if (entry.PlannedFormations.Count > 0)
                    preset.Formations[entry.TroopStringId] = entry.PlannedFormations.OrderBy(x => x).ToList();
            }
            return preset;
        }

        private void ApplyPreset(UTMPreset preset)
        {
            try
            {
                int applied = 0, skipped = 0;
                foreach (var entry in _troops)
                {
                    int targetBring = preset.Roster.TryGetValue(entry.TroopStringId, out var c) ? c : 0;
                    entry.SetBringSilent(targetBring);

                    // Rebuild formations: clear existing, add from preset.
                    var current = entry.PlannedFormations.ToList();
                    foreach (var idx in current) entry.RemoveFormation(idx);
                    if (preset.Formations.TryGetValue(entry.TroopStringId, out var slots))
                    {
                        foreach (var s in slots) entry.AddFormation(s);
                        applied++;
                    }
                }
                // Troops listed in preset but absent from party → counted as skipped.
                foreach (var kv in preset.Roster)
                    if (!_troops.Any(t => t.TroopStringId == kv.Key)) skipped++;

                RefreshTotal();
                RefreshFormationSlotHighlights();
                InformationManager.DisplayMessage(new InformationMessage(
                    "[UTM] Preset '" + preset.Name + "' loaded · " + applied + " plans applied · " + skipped + " troops skipped (not in party)"));
                UTMLog.Info("Preset applied · " + preset.Name + " · applied=" + applied + " skipped=" + skipped);
            }
            catch (Exception ex)
            {
                UTMLog.Exception("ApplyPreset", ex);
            }
        }

        public void ExecuteSave()
        {
            var selection = new RosterSelection();
            var plan = new BattlePlan();
            int planCount = 0;
            foreach (var entry in _troops)
            {
                if (entry.Bring > 0)
                    selection.Counts[entry.TroopStringId] = entry.Bring;
                if (entry.PlannedFormations.Count > 0)
                {
                    planCount++;
                    var orderedSlots = entry.PlannedFormations.OrderBy(x => x).ToList();
                    plan.Formations[entry.TroopStringId] = orderedSlots;
                    var joined = string.Join(",", orderedSlots.Select(FormationRoman));
                    UTMLog.Info("Plan · " + entry.TroopStringId + " → " + joined);
                }
            }
            RosterSelection.SetCurrent(selection);
            PartyPlanRuntime.SetCurrent(plan);
            UTMLog.Info("UI · Save · " + selection.Counts.Count + " types selected · total=" + _selectedTotal
                + " · " + planCount + " formation plans · PartyPlan published");
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
            if (entry == null) { UTMLog.Warn("OnTroopFocused · null entry"); return; }
            bool wasSelected = _selectedTroops.Contains(entry);
            if (wasSelected)
            {
                _selectedTroops.Remove(entry);
                entry.IsFocused = false;
            }
            else
            {
                _selectedTroops.Add(entry);
                entry.IsFocused = true;
            }
            UTMLog.Info("OnTroopFocused · " + entry.TroopStringId
                + " · " + (wasSelected ? "removed" : "added")
                + " · selection=" + _selectedTroops.Count);
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
        private void BuildEntries(RosterSelection prior, BattlePlan priorPlan)
        {
            _troops.Clear();
            if (_sourceRoster == null) return;

            try
            {
                // Safety-net restore: if a previous battle's modify snapshot
                // leaked (encounter cancelled before Finish patch ran etc.) and
                // we're currently NOT in a mission, roll back the modify now so
                // this UI session sees the full roster AND the player's save
                // file doesn't carry the reduced counts forward.
                if (Mission.Current == null
                    && UTMBattleState.RosterModified
                    && UTMBattleState.RosterSnapshot.Count > 0)
                {
                    try
                    {
                        var mp = MobileParty.MainParty;
                        if (mp != null && mp.MemberRoster != null)
                        {
                            int restoredTypes = 0, restoredCount = 0;
                            foreach (var kv in UTMBattleState.RosterSnapshot)
                            {
                                try
                                {
                                    mp.MemberRoster.AddToCounts(kv.Key, kv.Value);
                                    restoredTypes++;
                                    restoredCount += kv.Value;
                                }
                                catch (Exception innerEx)
                                {
                                    UTMLog.Exception("Safety restore(" + kv.Key?.StringId + ")", innerEx);
                                }
                            }
                            UTMLog.Warn("UI open · safety restore · " + restoredTypes + " types · " + restoredCount
                                + " troops · previous battle's RosterSnapshot was leaked");
                        }
                    }
                    catch (Exception restoreEx)
                    {
                        UTMLog.Exception("Safety restore block", restoreEx);
                    }
                    finally
                    {
                        UTMBattleState.ResetAll();
                        MissionSpawnTroopPatch.ResetRoundRobin();
                    }
                }

                // Build entries from the (now hopefully complete) MainParty
                // roster. For the "last battle still active" edge case where
                // Mission.Current != null and we can't safely restore, merge
                // the snapshot virtually: display the full count so the player
                // sees every troop type that exists on paper, even though the
                // live roster is temporarily reduced.
                for (int i = 0; i < _sourceRoster.Count; i++)
                {
                    var el = _sourceRoster.GetElementCopyAtIndex(i);
                    if (el.Character == null) continue;
                    if (el.Character.IsHero) continue;
                    int healthy = el.Number - el.WoundedNumber;
                    // Virtual merge: add back whatever this type still owes to
                    // the snapshot so UI reflects the complete party.
                    if (UTMBattleState.RosterModified
                        && UTMBattleState.RosterSnapshot.TryGetValue(el.Character, out int owed)
                        && owed > 0)
                    {
                        healthy += owed;
                    }
                    if (healthy <= 0) continue;

                    int initialBring = healthy;
                    if (prior != null && prior.Counts.TryGetValue(el.Character.StringId, out int stored))
                        initialBring = Math.Min(stored, healthy);

                    var entry = new UTMTroopEntryVM(
                        el.Character,
                        inParty: healthy,
                        initialBring: initialBring,
                        onCountChanged: OnEntryCountChanged,
                        onFocus: OnTroopFocused);
                    RestorePlannedFormations(entry, priorPlan);
                    _troops.Add(entry);
                }

                // Also add snapshot-only entries: character types that were
                // ENTIRELY removed from the live roster (healthy went to 0
                // after modify, so GetElementCopyAtIndex may skip them).
                if (UTMBattleState.RosterModified && UTMBattleState.RosterSnapshot.Count > 0)
                {
                    var alreadyListed = new HashSet<string>();
                    foreach (var entry in _troops) alreadyListed.Add(entry.TroopStringId);
                    foreach (var kv in UTMBattleState.RosterSnapshot)
                    {
                        if (kv.Key == null) continue;
                        if (alreadyListed.Contains(kv.Key.StringId)) continue;
                        if (kv.Value <= 0) continue;
                        if (kv.Key.IsHero) continue;

                        int initialBring = 0;
                        if (prior != null && prior.Counts.TryGetValue(kv.Key.StringId, out int stored))
                            initialBring = Math.Min(stored, kv.Value);

                        var entry = new UTMTroopEntryVM(
                            kv.Key,
                            inParty: kv.Value,
                            initialBring: initialBring,
                            onCountChanged: OnEntryCountChanged,
                            onFocus: OnTroopFocused);
                        RestorePlannedFormations(entry, priorPlan);
                        _troops.Add(entry);
                    }
                }

                UTMLog.Info("UI · built " + _troops.Count + " troop entries"
                    + (UTMBattleState.RosterModified ? " (snapshot merged)" : "")
                    + (priorPlan != null && !priorPlan.IsEmpty ? " · PartyPlan restored (" + priorPlan.Formations.Count + " entries)" : ""));
            }
            catch (Exception ex)
            {
                UTMLog.Exception("UTMTroopManagerVM.BuildEntries", ex);
            }
        }

        // Re-populate an entry's PlannedFormations from the live PartyPlan
        // snapshot. Without this, re-opening the UI after Save Selection would
        // show every troop with no formation plan — and the next Save would
        // overwrite the live PartyPlanRuntime with an empty plan, silently
        // breaking the OoB class pool override and letting vanilla classify.
        private static void RestorePlannedFormations(UTMTroopEntryVM entry, BattlePlan priorPlan)
        {
            if (priorPlan == null || priorPlan.IsEmpty) return;
            if (!priorPlan.Formations.TryGetValue(entry.TroopStringId, out var slots)) return;
            if (slots == null) return;
            foreach (var slot in slots)
            {
                if (slot < 0 || slot > 7) continue;
                entry.AddFormation(slot);
            }
        }

        private void BuildFormationSlots()
        {
            _formationSlots.Clear();
            for (int i = 0; i < 8; i++)
            {
                int idx = i;
                // Plain "Formation I..VIII" · no class suffix. B6-C Step 1
                // overrides each OoB card's class to match what UTM routes
                // into the slot (via DeploymentFormationClass +
                // RefreshFormation), so the vanilla "slot N is permanently
                // class X" assumption no longer holds. Slots are empty
                // containers · the class is whatever troops you put in them.
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

                // AssignedCount + MemberLines reflect the WHOLE party's assignment
                // to this slot (not just the active selection) so each Formation's
                // composition is always visible even when the user tabs through
                // different rows. The selection highlight is a separate signal.
                int totalSum = 0;
                var members = new List<string>();
                foreach (var t in _troops)
                {
                    if (!t.HasFormation(slot.Index)) continue;
                    int cnt = t.GetSplitCount(slot.Index);
                    if (cnt <= 0) continue;
                    totalSum += cnt;
                    members.Add(t.TroopName + " (" + cnt + ")");
                }
                slot.SetAssigned(totalSum);
                slot.SetMembers(members);
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
