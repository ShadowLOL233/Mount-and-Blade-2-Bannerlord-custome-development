using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace CustomBattleTroopBuilder.UI
{
    public class CBTBEditorVM : ViewModel
    {
        private const int ModeCombat = 0;
        private const int ModeAll = 1;
        private const int ModeCivilian = 2;

        private readonly Action _closeAction;

        private string _title = "CBTB Editor · v0.5.0 · Pending Phase 3c Test Battle + Phase 4 Equipment + Phase 5 Skill";
        private string _closeText = "Close";
        private string _searchHint = "Search by StringId / Name / Culture…";
        private string _searchText = "";
        private string _countText = "";

        private string _modeCombatText = "Combat";
        private string _modeAllText = "All";
        private string _modeCivilianText = "Civilian";
        private bool _isModeCombat = true;
        private bool _isModeAll = false;
        private bool _isModeCivilian = false;
        private int _mode = ModeCombat;

        private string _previewHeading = "";
        private string _previewBody = "";
        private bool _hasSelection = false;
        private string _cloneText = "Clone";

        private readonly List<TroopListItemVM> _allTroops = new List<TroopListItemVM>();
        private MBBindingList<TroopListItemVM> _troops = new MBBindingList<TroopListItemVM>();
        private TroopListItemVM? _selectedTroop;

        public CBTBEditorVM(Action closeAction)
        {
            _closeAction = closeAction;
            RefreshAllTroopsFromPool();
            RefreshPreview();
        }

        /// <summary>
        /// Rebuild _allTroops from MBObjectManager · called at ctor + after Clone.
        /// Preserves selected troop reference if its StringId still exists.
        /// </summary>
        private void RefreshAllTroopsFromPool()
        {
            var prevSelectedId = _selectedTroop?.StringId;

            _allTroops.Clear();
            try
            {
                var pool = MBObjectManager.Instance?.GetObjectTypeList<BasicCharacterObject>();
                if (pool != null)
                {
                    foreach (var ch in pool)
                    {
                        if (ch == null) continue;
                        if (ch.IsHero) continue;
                        if (ch.IsPlayerCharacter) continue;

                        var sid = ch.StringId ?? "";
                        if (sid.Length == 0) continue;

                        _allTroops.Add(new TroopListItemVM(ch, OnSelectTroop));
                    }
                }
            }
            catch (Exception e)
            {
                CBTBSubModule.Log($"[CBTB] RefreshAllTroopsFromPool failed: {e}");
            }

            _allTroops.Sort((a, b) =>
            {
                int c = string.Compare(a.CultureId, b.CultureId, StringComparison.OrdinalIgnoreCase);
                if (c != 0) return c;
                c = a.Tier.CompareTo(b.Tier);
                if (c != 0) return c;
                c = a.Level.CompareTo(b.Level);
                if (c != 0) return c;
                return string.Compare(a.StringId, b.StringId, StringComparison.OrdinalIgnoreCase);
            });

            if (prevSelectedId != null)
            {
                _selectedTroop = _allTroops.FirstOrDefault(t => t.StringId == prevSelectedId);
                if (_selectedTroop != null) _selectedTroop.IsSelected = true;
            }

            RefreshFilter();
        }

        private void RefreshFilter()
        {
            _troops.Clear();
            var q = _searchText?.Trim() ?? "";

            IEnumerable<TroopListItemVM> src = _allTroops;

            if (_mode == ModeCombat)
                src = src.Where(t => t.IsSoldier);
            else if (_mode == ModeCivilian)
                src = src.Where(t => !t.IsSoldier);

            if (q.Length > 0)
            {
                src = src.Where(t =>
                    t.StringId.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    t.DisplayName.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    t.CultureId.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            int shown = 0;
            foreach (var t in src)
            {
                _troops.Add(t);
                shown++;
            }

            CountText = $"Showing {shown} / {_allTroops.Count} troops";
            CBTBSubModule.Log($"[CBTB] RefreshFilter · mode={_mode} · query=\"{q}\" · shown={shown}/{_allTroops.Count}");
        }

        private void OnSelectTroop(TroopListItemVM troop)
        {
            if (_selectedTroop == troop) return;
            if (_selectedTroop != null) _selectedTroop.IsSelected = false;
            _selectedTroop = troop;
            if (troop != null) troop.IsSelected = true;
            RefreshPreview();
            CBTBSubModule.Log($"[CBTB] OnSelectTroop · {troop?.StringId ?? "<null>"}");
        }

        private void RefreshPreview()
        {
            if (_selectedTroop == null)
            {
                PreviewHeading = "No troop selected";
                PreviewBody = "Click a row on the left to see details here.\n\n" +
                              "Clone button becomes enabled once a selection is made.";
                HasSelection = false;
                return;
            }

            var ch = _selectedTroop.Character;
            PreviewHeading = _selectedTroop.DisplayName;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"StringId:        {ch.StringId}");
            sb.AppendLine($"Culture:         {ch.Culture?.StringId ?? "neutral"}");
            sb.AppendLine($"Tier (battle):   {_selectedTroop.Tier}");
            sb.AppendLine($"Level:           {ch.Level}");
            sb.AppendLine();
            sb.AppendLine($"IsSoldier:       {ch.IsSoldier}");
            sb.AppendLine($"IsHero:          {ch.IsHero}");
            sb.AppendLine($"IsMounted:       {ch.IsMounted}");
            sb.AppendLine($"IsRanged:        {ch.IsRanged}");
            sb.AppendLine($"IsInfantry:      {ch.IsInfantry}");
            sb.AppendLine($"IsFemale:        {ch.IsFemale}");
            sb.AppendLine();
            sb.AppendLine($"FormationClass:  {ch.DefaultFormationClass}");
            sb.AppendLine($"FormationGroup:  {ch.DefaultFormationGroup}");
            sb.AppendLine($"Age:             {ch.Age}");
            sb.AppendLine($"Race:            {ch.Race}");
            PreviewBody = sb.ToString();
            HasSelection = true;
        }

        private void SetMode(int newMode)
        {
            if (_mode == newMode) return;
            _mode = newMode;
            IsModeCombat = (newMode == ModeCombat);
            IsModeAll = (newMode == ModeAll);
            IsModeCivilian = (newMode == ModeCivilian);
            RefreshFilter();
        }

        public void ExecuteSetModeCombat() => SetMode(ModeCombat);
        public void ExecuteSetModeAll() => SetMode(ModeAll);
        public void ExecuteSetModeCivilian() => SetMode(ModeCivilian);

        public void ExecuteClone()
        {
            if (_selectedTroop == null)
            {
                ShowInfoInquiry("Clone Troop", "No troop selected. Pick one from the list first.");
                return;
            }

            var source = _selectedTroop.Character;
            var suggestedId = source.StringId + "_clone";

            InformationManager.ShowTextInquiry(new TextInquiryData(
                titleText: "Clone Troop · New StringId",
                text: $"Source: {source.StringId}\nSource name: {source.Name?.ToString() ?? "<none>"}\n\n" +
                      "Enter a new unique StringId (lowercase · no spaces · ascii only recommended):",
                isAffirmativeOptionShown: true,
                isNegativeOptionShown: true,
                affirmativeText: "Next",
                negativeText: "Cancel",
                affirmativeAction: (newId) => OnStringIdEntered(source, newId),
                negativeAction: () => CBTBSubModule.Log("[CBTB] Clone cancelled at StringId step"),
                defaultInputText: suggestedId));
        }

        private void OnStringIdEntered(BasicCharacterObject source, string newStringId)
        {
            if (string.IsNullOrWhiteSpace(newStringId))
            {
                ShowInfoInquiry("Clone Troop", "StringId cannot be empty · clone cancelled.");
                return;
            }
            newStringId = newStringId.Trim();

            BasicCharacterObject? existing = null;
            try { existing = MBObjectManager.Instance?.GetObject<BasicCharacterObject>(newStringId); }
            catch { existing = null; }

            if (existing != null)
            {
                ShowInfoInquiry("Clone Troop · Duplicate",
                    $"StringId '{newStringId}' already exists in the pool.\nPick another.");
                return;
            }

            var suggestedName = (source.Name?.ToString() ?? source.StringId) + " (Clone)";
            InformationManager.ShowTextInquiry(new TextInquiryData(
                titleText: "Clone Troop · Display Name",
                text: $"New StringId: {newStringId}\n\n" +
                      "Enter the display name shown in Custom Battle UI:",
                isAffirmativeOptionShown: true,
                isNegativeOptionShown: true,
                affirmativeText: "Clone",
                negativeText: "Cancel",
                affirmativeAction: (name) => CompleteClone(source, newStringId, name),
                negativeAction: () => CBTBSubModule.Log("[CBTB] Clone cancelled at Name step"),
                defaultInputText: suggestedName));
        }

        private void CompleteClone(BasicCharacterObject source, string newStringId, string displayName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(displayName))
                    displayName = newStringId;

                var newTroop = CBTBCloneHelper.CloneTroop(source, newStringId, displayName);

                if (newTroop == null)
                {
                    ShowInfoInquiry("Clone Failed", "CBTBCloneHelper.CloneTroop returned null (CreateObject or reflection failed · see log).");
                    return;
                }

                // Register blueprint for Restore after Bannerlord Game re-init (Back-to-menu wipes pool)
                CBTBClonesRegistry.Register(newStringId, displayName, source.StringId);

                CBTBSubModule.Log($"[CBTB] Cloned '{source.StringId}' → '{newStringId}' · name='{displayName}' · IsSoldier={newTroop.IsSoldier}");

                // Rebuild list · new troop will appear
                _selectedTroop = null;
                RefreshAllTroopsFromPool();
                var newItem = _allTroops.FirstOrDefault(t => t.StringId == newStringId);
                if (newItem != null) OnSelectTroop(newItem);

                InformationManager.ShowInquiry(new InquiryData(
                    titleText: "Clone Successful",
                    text:
                        $"Created BasicCharacterObject:\n" +
                        $"  StringId:  {newStringId}\n" +
                        $"  Name:      {displayName}\n" +
                        $"  Source:    {source.StringId}\n" +
                        $"  IsSoldier: {newTroop.IsSoldier}\n\n" +
                        "✅ Clone saved to CBTB blueprint registry (session-persistent)\n\n" +
                        "ℹ v0.5.0 架构转向 · 弃 Phase 3b-2b (vanilla Custom Battle picker 对抗 · 4 次 bootstrap crash 证实是技术死胡同)\n\n" +
                        "下阶段 Retinues 模式复现 (独立于 vanilla UI):\n" +
                        "  • Phase 3c · \"Test Battle\" button (editor 内直接开战)\n" +
                        "  • Phase 4 · Equipment 10-slot picker (ItemObject.All 配装)\n" +
                        "  • Phase 5 · Skill editor (20+ skills slider)\n" +
                        "  • Phase 6 · JSON save/load preset (跨 Bannerlord process)",
                    isAffirmativeOptionShown: true,
                    isNegativeOptionShown: false,
                    affirmativeText: "OK",
                    negativeText: null,
                    affirmativeAction: () => { },
                    negativeAction: null));
            }
            catch (Exception e)
            {
                CBTBSubModule.Log($"[CBTB] CompleteClone outer failed: {e}");
                ShowInfoInquiry("Clone Failed", $"Unexpected error: {e.Message}");
            }
        }

        private static void ShowInfoInquiry(string title, string body)
        {
            InformationManager.ShowInquiry(new InquiryData(
                titleText: title, text: body,
                isAffirmativeOptionShown: true, isNegativeOptionShown: false,
                affirmativeText: "OK", negativeText: null,
                affirmativeAction: () => { }, negativeAction: null));
        }

        public void ExecuteClose() => _closeAction?.Invoke();

        [DataSourceProperty]
        public string Title
        {
            get => _title;
            set { if (value != _title) { _title = value; OnPropertyChangedWithValue(value, "Title"); } }
        }

        [DataSourceProperty]
        public string CloseText
        {
            get => _closeText;
            set { if (value != _closeText) { _closeText = value; OnPropertyChangedWithValue(value, "CloseText"); } }
        }

        [DataSourceProperty]
        public string SearchHint
        {
            get => _searchHint;
            set { if (value != _searchHint) { _searchHint = value; OnPropertyChangedWithValue(value, "SearchHint"); } }
        }

        [DataSourceProperty]
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value != _searchText)
                {
                    _searchText = value ?? "";
                    OnPropertyChangedWithValue(_searchText, "SearchText");
                    RefreshFilter();
                }
            }
        }

        [DataSourceProperty]
        public string CountText
        {
            get => _countText;
            set { if (value != _countText) { _countText = value; OnPropertyChangedWithValue(value, "CountText"); } }
        }

        [DataSourceProperty]
        public MBBindingList<TroopListItemVM> Troops
        {
            get => _troops;
            set { if (value != _troops) { _troops = value; OnPropertyChangedWithValue(value, "Troops"); } }
        }

        [DataSourceProperty] public string ModeCombatText    { get => _modeCombatText;    set { if (value != _modeCombatText)    { _modeCombatText    = value; OnPropertyChangedWithValue(value, "ModeCombatText"); } } }
        [DataSourceProperty] public string ModeAllText       { get => _modeAllText;       set { if (value != _modeAllText)       { _modeAllText       = value; OnPropertyChangedWithValue(value, "ModeAllText"); } } }
        [DataSourceProperty] public string ModeCivilianText  { get => _modeCivilianText;  set { if (value != _modeCivilianText)  { _modeCivilianText  = value; OnPropertyChangedWithValue(value, "ModeCivilianText"); } } }

        [DataSourceProperty] public bool IsModeCombat   { get => _isModeCombat;   set { if (value != _isModeCombat)   { _isModeCombat   = value; OnPropertyChangedWithValue(value, "IsModeCombat"); } } }
        [DataSourceProperty] public bool IsModeAll      { get => _isModeAll;      set { if (value != _isModeAll)      { _isModeAll      = value; OnPropertyChangedWithValue(value, "IsModeAll"); } } }
        [DataSourceProperty] public bool IsModeCivilian { get => _isModeCivilian; set { if (value != _isModeCivilian) { _isModeCivilian = value; OnPropertyChangedWithValue(value, "IsModeCivilian"); } } }

        [DataSourceProperty] public string PreviewHeading { get => _previewHeading; set { if (value != _previewHeading) { _previewHeading = value; OnPropertyChangedWithValue(value, "PreviewHeading"); } } }
        [DataSourceProperty] public string PreviewBody    { get => _previewBody;    set { if (value != _previewBody)    { _previewBody    = value; OnPropertyChangedWithValue(value, "PreviewBody"); } } }
        [DataSourceProperty] public bool   HasSelection   { get => _hasSelection;   set { if (value != _hasSelection)   { _hasSelection   = value; OnPropertyChangedWithValue(value, "HasSelection"); } } }
        [DataSourceProperty] public string CloneText      { get => _cloneText;      set { if (value != _cloneText)      { _cloneText      = value; OnPropertyChangedWithValue(value, "CloneText"); } } }
    }
}
