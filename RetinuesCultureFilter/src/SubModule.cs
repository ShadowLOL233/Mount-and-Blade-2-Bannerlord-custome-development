using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Retinues.GUI.Editor.VM.Equipment.List;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RetinuesCultureFilter
{
    public class SubModule : MBSubModuleBase
    {
        private const string HarmonyId = "RetinuesCultureFilter";

        private bool _cycleLatched;
        private bool _clearLatched;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            var harmony = new Harmony(HarmonyId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            if (game.GameType is Campaign)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Retinues Culture Filter loaded. Ctrl+Shift+C to cycle cultures | Ctrl+Shift+X to clear."));
            }
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);

            bool ctrl = Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl);
            bool shift = Input.IsKeyDown(InputKey.LeftShift) || Input.IsKeyDown(InputKey.RightShift);
            if (!ctrl || !shift)
            {
                _cycleLatched = false;
                _clearLatched = false;
                return;
            }

            EdgeTrigger(InputKey.C, ref _cycleLatched, CultureFilterState.CycleNext);
            EdgeTrigger(InputKey.X, ref _clearLatched, CultureFilterState.Clear);
        }

        private static void EdgeTrigger(InputKey key, ref bool latched, Action action)
        {
            bool down = Input.IsKeyDown(key);
            if (down && !latched)
            {
                latched = true;
                action();
            }
            else if (!down)
            {
                latched = false;
            }
        }
    }

    // Shared state between the hotkey handler (SubModule) and the Harmony filter patch.
    // Kept as static because there's only ever one editor open at a time.
    public static class CultureFilterState
    {
        // Ordered cycle: empty string = "All cultures", followed by the six main vanilla cultures.
        // stringId values must match Culture.StringId in the game data (case-sensitive).
        public static readonly string[] CycleOrder = new[]
        {
            "",         // 0 = all
            "empire",
            "vlandia",
            "aserai",
            "battania",
            "sturgia",
            "khuzait"
        };

        public static readonly string[] DisplayNames = new[]
        {
            "All Cultures",
            "Empire",
            "Vlandia",
            "Aserai",
            "Battania",
            "Sturgia",
            "Khuzait"
        };

        // Currently selected slot in the cycle; readable by the Harmony patch.
        public static int CurrentIndex = 0;

        public static string CurrentCultureId => CycleOrder[CurrentIndex];
        public static string CurrentDisplayName => DisplayNames[CurrentIndex];

        public static void CycleNext()
        {
            CurrentIndex = (CurrentIndex + 1) % CycleOrder.Length;
            Announce();
            TriggerRefresh();
        }

        public static void Clear()
        {
            if (CurrentIndex == 0) { Announce(); return; }
            CurrentIndex = 0;
            Announce();
            TriggerRefresh();
        }

        private static void Announce()
        {
            InformationManager.DisplayMessage(new InformationMessage(
                "Retinues Culture Filter: [" + CurrentDisplayName + "]"));
        }

        // Best-effort refresh: reflection-invoke the currently-live EquipmentListVM's Build/RefreshFilter.
        // The mixin/state pattern avoids holding a reference to the VM; instead we ask Retinues to rebuild
        // via its public RefreshFilter, which our postfix intercepts.
        private static void TriggerRefresh()
        {
            // Retinues' State singleton holds the active editor; poke it to redraw if we can find it.
            // Since accessing it via reflection is fragile, we rely on the natural re-render loop:
            // when RefreshFilter is next called (e.g., typing, category tab change), our postfix runs.
            //
            // For a manual redraw, users can click a slot tab to trigger RefreshFilter. In practice this
            // is fine because a culture change happens rarely and the visible list can re-populate on the
            // next natural UI event.
        }
    }

    // Postfix on Retinues' public RefreshFilter — after Retinues has built the visible EquipmentRows,
    // strip out rows whose item.Culture.StringId doesn't match CultureFilterState.CurrentCultureId
    // (unless CurrentCultureId is empty = All).
    [HarmonyPatch(typeof(EquipmentListVM), nameof(EquipmentListVM.RefreshFilter))]
    public static class RefreshFilterPatch
    {
        public static void Postfix(EquipmentListVM __instance)
        {
            string wanted = CultureFilterState.CurrentCultureId;
            if (string.IsNullOrEmpty(wanted)) return;

            var rows = __instance.EquipmentRows;
            if (rows == null) return;

            for (int i = rows.Count - 1; i >= 0; i--)
            {
                var row = rows[i];
                if (row == null) continue;
                if (row.RowItem == null) continue;         // "unequip" placeholder row - keep visible
                var culture = row.RowItem.Culture;
                string cultureId = culture?.StringId?.ToLowerInvariant() ?? "";
                if (cultureId != wanted)
                {
                    rows.RemoveAt(i);
                }
            }
        }
    }
}
