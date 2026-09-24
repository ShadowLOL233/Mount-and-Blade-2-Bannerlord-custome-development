using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Bannerlord.UIExtenderEx;
using HarmonyLib;
using Retinues.Game.Wrappers;
using Retinues.GUI.Editor;
using Retinues.GUI.Editor.VM.Equipment.List;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RetinuesCultureFilter
{
    public class SubModule : MBSubModuleBase
    {
        private const string ExtenderId = "RetinuesCultureFilter";

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            var harmony = new Harmony(ExtenderId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            var extender = UIExtender.Create(ExtenderId);
            extender.Register(Assembly.GetExecutingAssembly());
            extender.Enable();
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            if (game.GameType is Campaign)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Retinues Culture Filter v1.5 loaded. Culture buttons available in the Retinues equipment editor."));
            }
        }
    }

    // Shared state driving the mixin's IsSelected bindings and the Harmony filter patch.
    // CurrentIndex == -1 means "no filter, show all cultures" (default on editor open).
    // CurrentIndex 0..6 selects one of the seven cultures (6 main + Nord sub-culture).
    public static class CultureFilterState
    {
        public static readonly string[] CycleOrder = new[]
        {
            "empire",   // 0
            "vlandia",  // 1
            "aserai",   // 2
            "battania", // 3
            "sturgia",  // 4
            "khuzait",  // 5
            "nord"      // 6
        };

        public static readonly string[] DisplayNames = new[]
        {
            "Empire",
            "Vlandia",
            "Aserai",
            "Battania",
            "Sturgia",
            "Khuzait",
            "Nord"
        };

        // Culture aliases: reserved for future sub-culture -> parent-culture folding.
        // Currently empty — earlier design (Nord -> Sturgia) was based on a wrong premise.
        // NavalDLC/ModuleData/items.xml adds 82 items tagged Culture.nord (Berserker/Vendel/
        // Nordic/Northman/Norse/Nord King series). spcultures.xml only defines Nord as a
        // Sturgia-cloned sub-culture, but the Naval DLC promotes it to a full kingdom with
        // its own equipment. So filtering by Nord returns those 82 items directly — no alias
        // needed. Kept for future minor cultures (vakken/darshi) that may lack own items.
        public static readonly System.Collections.Generic.Dictionary<string, string[]> CultureAliases =
            new System.Collections.Generic.Dictionary<string, string[]>();

        public static int CurrentIndex = -1;

        public static string CurrentCultureId =>
            (CurrentIndex >= 0 && CurrentIndex < CycleOrder.Length) ? CycleOrder[CurrentIndex] : "";

        // Get the full set of culture ids the current filter matches (usually 1, but aliased
        // filters like Nord match multiple: nord + sturgia).
        public static string[] CurrentMatchingCultureIds
        {
            get
            {
                string cid = CurrentCultureId;
                if (string.IsNullOrEmpty(cid)) return System.Array.Empty<string>();
                string[] aliases;
                if (CultureAliases.TryGetValue(cid, out aliases)) return aliases;
                return new[] { cid };
            }
        }

        public static string CurrentDisplayName =>
            (CurrentIndex >= 0 && CurrentIndex < DisplayNames.Length) ? DisplayNames[CurrentIndex] : "None";

        // Toggle: clicking the currently-selected culture clears the filter (returns to default).
        public static void SetIndexToggle(int i)
        {
            int newIndex = (i == CurrentIndex) ? -1 : i;
            if (newIndex < -1 || newIndex >= CycleOrder.Length) return;
            CurrentIndex = newIndex;
            InformationManager.DisplayMessage(new InformationMessage(
                "Retinues Culture Filter: [" + CurrentDisplayName + "]"));
        }
    }

    // Prefix + Postfix on Retinues' RebuildVisibleFromSnapshot — the single choke point every
    // list refresh path funnels through (Build, ExecuteNext/PrevPage, ExecuteSortByXxx,
    // OnFilterTextChanged, RefreshFilter). Temporarily replaces _fullTuples[snapshotKey] with a
    // culture-filtered copy before the method runs; restores the original in Postfix. Sort,
    // search, and pagination all operate on the filtered subset — page counts stay accurate,
    // no empty pages, all matches visible across pages.
    [HarmonyPatch(typeof(EquipmentListVM), "RebuildVisibleFromSnapshot")]
    public static class RebuildVisibleFilterPatch
    {
        private sealed class SavedState
        {
            public IDictionary FullTuples;
            public object SnapshotKey;
            public object OriginalList;
        }

        private static readonly FieldInfo FullTuplesField =
            AccessTools.Field(typeof(EquipmentListVM), "_fullTuples");
        private static readonly MethodInfo GetSnapshotKeyMethod =
            AccessTools.Method(typeof(EquipmentListVM), "GetSnapshotKey");
        private static readonly Type ItemTupleType =
            AccessTools.Inner(typeof(EquipmentListVM), "ItemTuple");
        private static readonly FieldInfo ItemTupleItemField =
            (ItemTupleType != null) ? AccessTools.Field(ItemTupleType, "Item") : null;

        public static void Prefix(EquipmentListVM __instance, out object __state)
        {
            __state = null;
            string wanted = CultureFilterState.CurrentCultureId;
            if (string.IsNullOrEmpty(wanted)) return;
            if (FullTuplesField == null || GetSnapshotKeyMethod == null
                || ItemTupleType == null || ItemTupleItemField == null) return;

            var slot = State.Slot;
            var snapshotKey = GetSnapshotKeyMethod.Invoke(null, new object[] { slot });

            var fullTuples = FullTuplesField.GetValue(__instance) as IDictionary;
            if (fullTuples == null || !fullTuples.Contains(snapshotKey)) return;
            var original = fullTuples[snapshotKey] as IList;
            if (original == null) return;

            var listType = typeof(List<>).MakeGenericType(ItemTupleType);
            var filtered = Activator.CreateInstance(listType) as IList;
            if (filtered == null) return;

            // Resolve aliases once outside the loop (Nord -> [nord, sturgia], others -> [self]).
            var matchIds = CultureFilterState.CurrentMatchingCultureIds;

            foreach (var tuple in original)
            {
                var item = ItemTupleItemField.GetValue(tuple) as WItem;
                if (item == null) continue;
                var culture = item.Culture;
                var cid = culture?.StringId?.ToLowerInvariant() ?? "";
                bool matched = false;
                for (int i = 0; i < matchIds.Length; i++)
                {
                    if (cid == matchIds[i]) { matched = true; break; }
                }
                if (matched) filtered.Add(tuple);
            }

            fullTuples[snapshotKey] = filtered;
            __state = new SavedState
            {
                FullTuples = fullTuples,
                SnapshotKey = snapshotKey,
                OriginalList = original
            };
        }

        public static void Postfix(object __state)
        {
            if (__state is SavedState s)
            {
                s.FullTuples[s.SnapshotKey] = s.OriginalList;
            }
        }
    }
}
