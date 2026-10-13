using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.CustomBattle;

namespace CustomBattleTroopBuilder.UI
{
    [ViewModelMixin("RefreshValues", true)]
    public class CBTBCustomBattleMenuMixin : BaseViewModelMixin<CustomBattleVM>
    {
        public CBTBCustomBattleMenuMixin(CustomBattleVM vm) : base(vm)
        {
            CBTBSubModule.Log("[CBTB] CustomBattleMenuMixin ctor · attached to CustomBattleVM");
        }

        [DataSourceProperty]
        public string TroopBuilderButtonText => "Troop Builder";

        [DataSourceMethod]
        public void ExecuteOpenTroopBuilder()
        {
            CBTBSubModule.Log("[CBTB] mixin.ExecuteOpenTroopBuilder invoked");
            CBTBSubModule.OpenEditor();
        }
    }
}
