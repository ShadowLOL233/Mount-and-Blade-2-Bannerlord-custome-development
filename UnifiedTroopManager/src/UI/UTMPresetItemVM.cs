using System;
using TaleWorlds.Library;
using UnifiedTroopManager.Data;

namespace UnifiedTroopManager.UI
{
    // One row in the Load Preset list panel. Keeps a reference to the backing
    // UTMPreset so the parent VM can read all fields when the user clicks Load /
    // Delete — avoids re-reading from disk on every click.
    public sealed class UTMPresetItemVM : ViewModel
    {
        private readonly Action<UTMPresetItemVM> _onLoad;
        private readonly Action<UTMPresetItemVM> _onDelete;

        public UTMPreset RawPreset { get; }

        private string _name;
        private string _subtitle;

        public UTMPresetItemVM(UTMPreset preset,
            Action<UTMPresetItemVM> onLoad,
            Action<UTMPresetItemVM> onDelete)
        {
            RawPreset = preset;
            _onLoad = onLoad;
            _onDelete = onDelete;
            _name = preset.Name ?? "(unnamed)";
            _subtitle = preset.Created.ToString("yyyy-MM-dd HH:mm")
                + " · " + preset.Roster.Count + " troops"
                + " · " + preset.Formations.Count + " plans";
        }

        [DataSourceProperty]
        public string Name
        {
            get => _name;
            set { if (_name != value) { _name = value; OnPropertyChangedWithValue(value); } }
        }

        [DataSourceProperty]
        public string Subtitle
        {
            get => _subtitle;
            set { if (_subtitle != value) { _subtitle = value; OnPropertyChangedWithValue(value); } }
        }

        public void ExecuteLoad() { _onLoad?.Invoke(this); }
        public void ExecuteDelete() { _onDelete?.Invoke(this); }
    }
}
