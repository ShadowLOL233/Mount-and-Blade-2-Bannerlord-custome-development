using System.Collections.Generic;
using System.Reflection;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Library;

namespace EquipmentSpawnerMod.Inventory
{
    // Mixin on vanilla SPInventoryVM. Adds 6 culture-filter bindings + 1 inject-to-stash button
    // (visible only when the inventory screen is in Stash mode — our menu option OR vanilla
    // Settlement.Stash from town_keep/castle "Open stash"). Inject button clicks target the
    // ItemRoster captured by CaptureOpenScreenAsStashPatch.
    //
    // On click: update shared state, notify property bindings, and re-run vanilla per-item
    // filtering (which our Harmony postfix on UpdateFilteredStatusOfItem hooks into to add
    // the culture check). vm.LeftItemListVM + vm.RightItemListVM are iterated and each item
    // has UpdateFilteredStatusOfItem invoked via reflection since that method is private.
    [ViewModelMixin]
    public sealed class SPInventoryVMCultureMixin : BaseViewModelMixin<SPInventoryVM>
    {
        private static readonly MethodInfo UpdateFilteredStatusMethod =
            AccessTools.Method(typeof(SPInventoryVM), "UpdateFilteredStatusOfItem");

        // SPInventoryVM._usageType is set at construction to the active InventoryState.InventoryMode
        // (Default / Loot / Stash / Warehouse / Trade / ...). Read it via reflection to gate the
        // inject button — we only want it on Stash-mode screens, not on trader / loot / character
        // inventory where injecting would be confusing or wrong.
        private static readonly FieldInfo UsageTypeField =
            AccessTools.Field(typeof(SPInventoryVM), "_usageType");

        public SPInventoryVMCultureMixin(SPInventoryVM vm) : base(vm)
        {
            // Fresh state each time the inventory screen opens (a new mixin instance is
            // created per VM instantiation). Keeps behavior predictable across sessions.
            InventoryCultureFilterState.Reset();
        }

        [DataSourceProperty]
        public bool EqsmShowInjectButton
        {
            get
            {
                var vm = base.ViewModel;
                if (vm == null || UsageTypeField == null) return false;
                var mode = UsageTypeField.GetValue(vm);
                if (mode == null) return false;
                // InventoryMode.Stash == 4 in the enum (Default=0, Trade=1, Loot=2, Charity=3, Stash=4, Warehouse=5).
                // Compare by name to survive minor enum-value shuffles across game versions.
                return mode.ToString() == "Stash";
            }
        }

        [DataSourceMethod]
        public void ExecuteEqsmInjectToStash()
        {
            var target = StashSessionState.CurrentStash;
            EquipmentSpawnerSubModule.InjectInto(target);
        }

        [DataSourceProperty] public bool EqsmCultureEmpireSelected   => InventoryCultureFilterState.CurrentIndex == 0;
        [DataSourceProperty] public bool EqsmCultureVlandiaSelected  => InventoryCultureFilterState.CurrentIndex == 1;
        [DataSourceProperty] public bool EqsmCultureAseraiSelected   => InventoryCultureFilterState.CurrentIndex == 2;
        [DataSourceProperty] public bool EqsmCultureBattaniaSelected => InventoryCultureFilterState.CurrentIndex == 3;
        [DataSourceProperty] public bool EqsmCultureSturgiaSelected  => InventoryCultureFilterState.CurrentIndex == 4;
        [DataSourceProperty] public bool EqsmCultureKhuzaitSelected  => InventoryCultureFilterState.CurrentIndex == 5;

        [DataSourceMethod] public void ExecuteEqsmSelectCultureEmpire()   { SetIndex(0); }
        [DataSourceMethod] public void ExecuteEqsmSelectCultureVlandia()  { SetIndex(1); }
        [DataSourceMethod] public void ExecuteEqsmSelectCultureAserai()   { SetIndex(2); }
        [DataSourceMethod] public void ExecuteEqsmSelectCultureBattania() { SetIndex(3); }
        [DataSourceMethod] public void ExecuteEqsmSelectCultureSturgia()  { SetIndex(4); }
        [DataSourceMethod] public void ExecuteEqsmSelectCultureKhuzait()  { SetIndex(5); }

        private void SetIndex(int i)
        {
            InventoryCultureFilterState.SetIndexToggle(i);
            RefreshButtonStates();
            TriggerReFilter();
        }

        private void RefreshButtonStates()
        {
            OnPropertyChangedWithValue(EqsmCultureEmpireSelected,   nameof(EqsmCultureEmpireSelected));
            OnPropertyChangedWithValue(EqsmCultureVlandiaSelected,  nameof(EqsmCultureVlandiaSelected));
            OnPropertyChangedWithValue(EqsmCultureAseraiSelected,   nameof(EqsmCultureAseraiSelected));
            OnPropertyChangedWithValue(EqsmCultureBattaniaSelected, nameof(EqsmCultureBattaniaSelected));
            OnPropertyChangedWithValue(EqsmCultureSturgiaSelected,  nameof(EqsmCultureSturgiaSelected));
            OnPropertyChangedWithValue(EqsmCultureKhuzaitSelected,  nameof(EqsmCultureKhuzaitSelected));
        }

        private void TriggerReFilter()
        {
            var vm = base.ViewModel;
            if (vm == null || UpdateFilteredStatusMethod == null) return;
            ApplyToList(vm, vm.LeftItemListVM);
            ApplyToList(vm, vm.RightItemListVM);
        }

        private static void ApplyToList(SPInventoryVM vm, IEnumerable<SPItemVM> list)
        {
            if (list == null) return;
            foreach (var item in list)
            {
                if (item == null) continue;
                UpdateFilteredStatusMethod.Invoke(vm, new object[] { item });
            }
        }
    }
}
