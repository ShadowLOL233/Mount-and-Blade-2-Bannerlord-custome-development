using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace CustomBattleTroopBuilder.UI
{
    public class CBTBEditorVM : ViewModel
    {
        // Modes for the Combat/All/Civilian toggle row.
        private const int ModeCombat = 0;
        private const int ModeAll = 1;
        private const int ModeCivilian = 2;

        private readonly Action _closeAction;

        private string _title = "CBTB Editor · Phase 3a";
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

        private readonly List<TroopListItemVM> _allTroops;
        private MBBindingList<TroopListItemVM> _troops = new MBBindingList<TroopListItemVM>();

        public CBTBEditorVM(Action closeAction)
        {
            _closeAction = closeAction;
            _allTroops = BuildAllTroops();
            RefreshFilter();
        }

        private static List<TroopListItemVM> BuildAllTroops()
        {
            var list = new List<TroopListItemVM>();
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

                        list.Add(new TroopListItemVM(ch));
                    }
                }
            }
            catch (Exception e)
            {
                CBTBSubModule.Log($"[CBTB] BuildAllTroops failed: {e}");
            }

            list.Sort((a, b) =>
            {
                int c = string.Compare(a.CultureId, b.CultureId, StringComparison.OrdinalIgnoreCase);
                if (c != 0) return c;
                c = a.Tier.CompareTo(b.Tier);
                if (c != 0) return c;
                c = a.Level.CompareTo(b.Level);
                if (c != 0) return c;
                return string.Compare(a.StringId, b.StringId, StringComparison.OrdinalIgnoreCase);
            });

            return list;
        }

        private void RefreshFilter()
        {
            _troops.Clear();
            var q = _searchText?.Trim() ?? "";

            IEnumerable<TroopListItemVM> src = _allTroops;

            // Mode filter: Combat / Civilian split uses BasicCharacterObject.IsSoldier
            // (vanilla classifies "soldiers" into a true/false bucket · civilian
            // templates / tavern workers / dummies all land on IsSoldier=false).
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

        [DataSourceProperty]
        public string Title
        {
            get => _title;
            set
            {
                if (value != _title) { _title = value; OnPropertyChangedWithValue(value, "Title"); }
            }
        }

        [DataSourceProperty]
        public string CloseText
        {
            get => _closeText;
            set
            {
                if (value != _closeText) { _closeText = value; OnPropertyChangedWithValue(value, "CloseText"); }
            }
        }

        [DataSourceProperty]
        public string SearchHint
        {
            get => _searchHint;
            set
            {
                if (value != _searchHint) { _searchHint = value; OnPropertyChangedWithValue(value, "SearchHint"); }
            }
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
            set
            {
                if (value != _countText) { _countText = value; OnPropertyChangedWithValue(value, "CountText"); }
            }
        }

        [DataSourceProperty]
        public MBBindingList<TroopListItemVM> Troops
        {
            get => _troops;
            set
            {
                if (value != _troops) { _troops = value; OnPropertyChangedWithValue(value, "Troops"); }
            }
        }

        [DataSourceProperty] public string ModeCombatText    { get => _modeCombatText;    set { if (value != _modeCombatText)    { _modeCombatText    = value; OnPropertyChangedWithValue(value, "ModeCombatText"); } } }
        [DataSourceProperty] public string ModeAllText       { get => _modeAllText;       set { if (value != _modeAllText)       { _modeAllText       = value; OnPropertyChangedWithValue(value, "ModeAllText"); } } }
        [DataSourceProperty] public string ModeCivilianText  { get => _modeCivilianText;  set { if (value != _modeCivilianText)  { _modeCivilianText  = value; OnPropertyChangedWithValue(value, "ModeCivilianText"); } } }

        [DataSourceProperty] public bool IsModeCombat   { get => _isModeCombat;   set { if (value != _isModeCombat)   { _isModeCombat   = value; OnPropertyChangedWithValue(value, "IsModeCombat"); } } }
        [DataSourceProperty] public bool IsModeAll      { get => _isModeAll;      set { if (value != _isModeAll)      { _isModeAll      = value; OnPropertyChangedWithValue(value, "IsModeAll"); } } }
        [DataSourceProperty] public bool IsModeCivilian { get => _isModeCivilian; set { if (value != _isModeCivilian) { _isModeCivilian = value; OnPropertyChangedWithValue(value, "IsModeCivilian"); } } }

        public void ExecuteClose()
        {
            _closeAction?.Invoke();
        }
    }
}
