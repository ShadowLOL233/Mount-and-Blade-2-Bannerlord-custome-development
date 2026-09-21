using System.Collections.Generic;
using System.Reflection;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Library;

namespace EquipmentSpawnerMod.Inventory
{
    // Mixin on vanilla SPInventoryVM. Adds 6 [DataSourceMethod] Execute methods + 6
    // [DataSourceProperty] IsSelected bindings, driving the culture button row injected via
    // InventoryCultureButtonRowInsert.
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

        public SPInventoryVMCultureMixin(SPInventoryVM vm) : base(vm)
        {
            // Fresh state each time the inventory screen opens (a new mixin instance is
            // created per VM instantiation). Keeps behavior predictable across sessions.
            InventoryCultureFilterState.Reset();
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
