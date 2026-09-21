using System.Reflection;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using HarmonyLib;
using Retinues.GUI.Editor.VM.Equipment.List;
using TaleWorlds.Library;

namespace RetinuesCultureFilter.Mixins
{
    // Adds 6 IsSelected properties + 6 [DataSourceMethod] Execute methods to Retinues' sealed
    // EquipmentListVM. UIExtenderEx's ViewModelComponent scans mixins for [DataSourceMethod]
    // (Bannerlord.UIExtenderEx.Attributes, NOT TaleWorlds.Library) and calls VM.AddMethod()
    // so GauntletUI Command.Click bindings can resolve. Without that attribute, clicks are
    // silent no-ops.
    //
    // On click we call vm.RefreshFilter() rather than vm.Build(). Build() early-returns when
    // _needsRebuild is false (which it is after the first sort/page/click), so it wouldn't
    // trigger the list refresh. RefreshFilter() always ends in RebuildVisibleFromSnapshot when
    // needsRebuild is false — reliable trigger for our RebuildVisibleFromSnapshot patch to fire.
    //
    // We also reset _currentPageIndex to 0 before refresh so switching culture doesn't leave
    // the user stranded on a page that no longer exists after filtering.
    [ViewModelMixin]
    public sealed class EquipmentListVMMixin : BaseViewModelMixin<EquipmentListVM>
    {
        private static readonly FieldInfo CurrentPageIndexField =
            AccessTools.Field(typeof(EquipmentListVM), "_currentPageIndex");

        public EquipmentListVMMixin(EquipmentListVM vm) : base(vm) { }

        [DataSourceProperty] public bool CultureEmpireSelected   => CultureFilterState.CurrentIndex == 0;
        [DataSourceProperty] public bool CultureVlandiaSelected  => CultureFilterState.CurrentIndex == 1;
        [DataSourceProperty] public bool CultureAseraiSelected   => CultureFilterState.CurrentIndex == 2;
        [DataSourceProperty] public bool CultureBattaniaSelected => CultureFilterState.CurrentIndex == 3;
        [DataSourceProperty] public bool CultureSturgiaSelected  => CultureFilterState.CurrentIndex == 4;
        [DataSourceProperty] public bool CultureKhuzaitSelected  => CultureFilterState.CurrentIndex == 5;

        [DataSourceMethod] public void ExecuteSelectCultureEmpire()   { SetIndex(0); }
        [DataSourceMethod] public void ExecuteSelectCultureVlandia()  { SetIndex(1); }
        [DataSourceMethod] public void ExecuteSelectCultureAserai()   { SetIndex(2); }
        [DataSourceMethod] public void ExecuteSelectCultureBattania() { SetIndex(3); }
        [DataSourceMethod] public void ExecuteSelectCultureSturgia()  { SetIndex(4); }
        [DataSourceMethod] public void ExecuteSelectCultureKhuzait()  { SetIndex(5); }

        private void SetIndex(int i)
        {
            CultureFilterState.SetIndexToggle(i);
            RefreshCultureButtonStates();

            var vm = base.ViewModel;
            if (vm == null) return;
            if (CurrentPageIndexField != null) CurrentPageIndexField.SetValue(vm, 0);
            vm.RefreshFilter();
        }

        private void RefreshCultureButtonStates()
        {
            OnPropertyChangedWithValue(CultureEmpireSelected,   nameof(CultureEmpireSelected));
            OnPropertyChangedWithValue(CultureVlandiaSelected,  nameof(CultureVlandiaSelected));
            OnPropertyChangedWithValue(CultureAseraiSelected,   nameof(CultureAseraiSelected));
            OnPropertyChangedWithValue(CultureBattaniaSelected, nameof(CultureBattaniaSelected));
            OnPropertyChangedWithValue(CultureSturgiaSelected,  nameof(CultureSturgiaSelected));
            OnPropertyChangedWithValue(CultureKhuzaitSelected,  nameof(CultureKhuzaitSelected));
        }
    }
}
