using TaleWorlds.Library;

namespace EquipmentSpawnerMod.Inventory
{
    // Shared state driving the culture button mixin + the vanilla-filter postfix.
    // CurrentIndex == -1 means "no culture filter, show all" (default whenever inventory opens).
    // 0..5 selects one of the six main vanilla cultures. Same design as RetinuesCultureFilter.
    public static class InventoryCultureFilterState
    {
        public static readonly string[] CycleOrder = new[]
        {
            "empire",   // 0
            "vlandia",  // 1
            "aserai",   // 2
            "battania", // 3
            "sturgia",  // 4
            "khuzait"   // 5
        };

        public static readonly string[] DisplayNames = new[]
        {
            "Empire",
            "Vlandia",
            "Aserai",
            "Battania",
            "Sturgia",
            "Khuzait"
        };

        public static int CurrentIndex = -1;

        public static string CurrentCultureId =>
            (CurrentIndex >= 0 && CurrentIndex < CycleOrder.Length) ? CycleOrder[CurrentIndex] : "";

        public static string CurrentDisplayName =>
            (CurrentIndex >= 0 && CurrentIndex < DisplayNames.Length) ? DisplayNames[CurrentIndex] : "None";

        public static void SetIndexToggle(int i)
        {
            int newIndex = (i == CurrentIndex) ? -1 : i;
            if (newIndex < -1 || newIndex >= CycleOrder.Length) return;
            CurrentIndex = newIndex;
            InformationManager.DisplayMessage(new InformationMessage(
                "Inventory Culture Filter: [" + CurrentDisplayName + "]"));
        }

        // Reset to "no filter" — called when inventory opens/closes so state doesn't leak
        // across screen sessions (would surprise user opening a fresh inventory later).
        public static void Reset()
        {
            CurrentIndex = -1;
        }
    }
}
