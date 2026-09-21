using System.Collections.Generic;
using System.Reflection;
using Bannerlord.UIExtenderEx;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace EquipmentSpawnerMod
{
    public class EquipmentSpawnerSubModule : MBSubModuleBase
    {
        private const string ExtenderId = "EquipmentSpawnerMod";
        private const int GearQuantity = 20;
        private const int HorseQuantity = 20;
        private const ItemObject.ItemTiers MinTier = ItemObject.ItemTiers.Tier3;

        // OSA prefixes on item id / mesh name; matching either counts as OSA and bypasses tier check.
        private static readonly string[] OsaPrefixes = new[]
        {
            "AR_", "AD_", "AO_", "BA_", "DZ_", "TV_",
            "ao_", "ap_", "bl_", "hmj_", "tv_"
        };

        private bool _injectLatched;
        private bool _stashOutLatched;
        private bool _stashInLatched;
        private bool _townOutLatched;
        private bool _townInLatched;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            var harmony = new Harmony(ExtenderId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            var extender = UIExtender.Create(ExtenderId);
            extender.Register(Assembly.GetExecutingAssembly());
            extender.Enable();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (game.GameType is Campaign && gameStarterObject is CampaignGameStarter cgs)
            {
                cgs.AddBehavior(new PersonalStashBehavior());
                cgs.AddBehavior(new TownStashBehavior());
                AddStashMenuOptions(cgs);
            }
        }

        // Two menu options each in town/castle menus:
        //   1) "Manage personal equipment stash" — always visible (personal stash is portable)
        //   2) "Manage [Settlement]'s equipment stash" — only shown when the settlement is
        //      player-clan-owned (town stash is location-locked)
        // Both open the vanilla inventory screen (via InventoryScreenHelper.OpenScreenAsStash),
        // which gives us free-standing item transfer + quantity slider (Ctrl+click) + sort +
        // search — no custom UI needed for those. Culture filter is added on top via
        // SPInventoryVMCultureMixin + Inventory prefab extension.
        private static void AddStashMenuOptions(CampaignGameStarter cgs)
        {
            foreach (string menuId in new[] { "town", "castle" })
            {
                cgs.AddGameMenuOption(
                    menuId,
                    "eqsm_manage_personal_stash",
                    "Manage personal equipment stash",
                    args =>
                    {
                        args.optionLeaveType = GameMenuOption.LeaveType.Manage;
                        return true;
                    },
                    args =>
                    {
                        var behavior = GetStashBehavior();
                        if (behavior == null)
                        {
                            InformationManager.DisplayMessage(new InformationMessage(
                                "Equipment Spawner: personal stash behavior missing. Save + reload once."));
                            return;
                        }
                        InventoryScreenHelper.OpenScreenAsStash(behavior.Stash);
                    });

                cgs.AddGameMenuOption(
                    menuId,
                    "eqsm_manage_town_stash",
                    "Manage this settlement's equipment stash",
                    args =>
                    {
                        args.optionLeaveType = GameMenuOption.LeaveType.Manage;
                        var s = Settlement.CurrentSettlement;
                        return s != null && s.OwnerClan == Clan.PlayerClan;
                    },
                    args =>
                    {
                        var s = Settlement.CurrentSettlement;
                        var behavior = GetTownStashBehavior();
                        if (behavior == null || s == null)
                        {
                            InformationManager.DisplayMessage(new InformationMessage(
                                "Equipment Spawner: town stash behavior missing or not in a settlement."));
                            return;
                        }
                        InventoryScreenHelper.OpenScreenAsStash(behavior.GetOrCreateStashFor(s));
                    });
            }
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            if (game.GameType is Campaign)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Equipment Spawner v1.6.1 loaded. Town/castle menus: 'Manage ... equipment stash'. Hotkeys: Ctrl+Alt+I/O/P/U/Y still available."));
            }
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);

            bool ctrl = Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl);
            bool alt = Input.IsKeyDown(InputKey.LeftAlt) || Input.IsKeyDown(InputKey.RightAlt);
            if (!ctrl || !alt)
            {
                _injectLatched = false;
                _stashOutLatched = false;
                _stashInLatched = false;
                _townOutLatched = false;
                _townInLatched = false;
                return;
            }

            EdgeTrigger(InputKey.I, ref _injectLatched, TryInject);
            EdgeTrigger(InputKey.O, ref _stashOutLatched, TryPartyToStash);
            EdgeTrigger(InputKey.P, ref _stashInLatched, TryStashToParty);
            EdgeTrigger(InputKey.U, ref _townOutLatched, TryPartyToTownStash);
            EdgeTrigger(InputKey.Y, ref _townInLatched, TryTownStashToParty);
        }

        private static void EdgeTrigger(InputKey key, ref bool latched, System.Action action)
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

        private static bool RequireCampaign()
        {
            if (Campaign.Current == null || MobileParty.MainParty == null)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Equipment Spawner: no active campaign / main party. Load a save first."));
                return false;
            }
            return true;
        }

        private static PersonalStashBehavior GetStashBehavior()
        {
            return Campaign.Current?.GetCampaignBehavior<PersonalStashBehavior>();
        }

        private static TownStashBehavior GetTownStashBehavior()
        {
            return Campaign.Current?.GetCampaignBehavior<TownStashBehavior>();
        }

        private static Settlement GetCurrentOwnedSettlement()
        {
            Settlement s = Settlement.CurrentSettlement;
            if (s == null) s = MobileParty.MainParty?.CurrentSettlement;
            if (s == null) return null;
            if (s.OwnerClan != Clan.PlayerClan) return null;
            return s;
        }

        private static void TryInject()
        {
            if (!RequireCampaign()) return;

            var roster = MobileParty.MainParty.ItemRoster;
            var items = MBObjectManager.Instance.GetObjectTypeList<ItemObject>();

            int gearStacks = 0, horseStacks = 0, modified = 0, osaBypass = 0;
            foreach (var item in items)
            {
                if (item == null) continue;

                bool isOsa = IsOsaItem(item);
                if (!isOsa && item.Tier < MinTier) continue;

                bool isGear = IsGear(item.ItemType);
                bool isMount = item.ItemType == ItemObject.ItemTypeEnum.Horse
                            || item.ItemType == ItemObject.ItemTypeEnum.HorseHarness;
                if (!isGear && !isMount) continue;

                var modifier = BestModifier(item);
                var element = new EquipmentElement(item, modifier);
                int qty = isMount ? HorseQuantity : GearQuantity;
                roster.AddToCounts(element, qty);

                if (isGear) gearStacks++; else horseStacks++;
                if (modifier != null) modified++;
                if (isOsa && item.Tier < MinTier) osaBypass++;
            }

            InformationManager.DisplayMessage(new InformationMessage(
                "Equipment Spawner: injected " + gearStacks + " gear x" + GearQuantity
                + ", " + horseStacks + " mount x" + HorseQuantity
                + " (Tier " + ((int)MinTier + 1) + "-6 vanilla + all OSA; " + osaBypass + " OSA below T" + ((int)MinTier + 1)
                + " included; " + modified + " with best modifier)."));
        }

        private static bool IsOsaItem(ItemObject item)
        {
            string id = item.StringId ?? string.Empty;
            string mesh = item.MultiMeshName ?? string.Empty;
            for (int i = 0; i < OsaPrefixes.Length; i++)
            {
                string p = OsaPrefixes[i];
                if (id.StartsWith(p, System.StringComparison.Ordinal)) return true;
                if (mesh.StartsWith(p, System.StringComparison.Ordinal)) return true;
            }
            return false;
        }

        private static void TryPartyToStash()
        {
            if (!RequireCampaign()) return;
            var behavior = GetStashBehavior();
            if (behavior == null)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Equipment Spawner: personal stash behavior missing. Save + reload once with the mod enabled."));
                return;
            }

            TransferAll(MobileParty.MainParty.ItemRoster, behavior.Stash, "party -> personal stash", behavior.Stash.Count);
        }

        private static void TryStashToParty()
        {
            if (!RequireCampaign()) return;
            var behavior = GetStashBehavior();
            if (behavior == null)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Equipment Spawner: personal stash behavior missing. Save + reload once with the mod enabled."));
                return;
            }
            if (behavior.Stash.Count == 0)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Equipment Spawner: personal stash is empty."));
                return;
            }
            TransferAll(behavior.Stash, MobileParty.MainParty.ItemRoster, "personal stash -> party", -1);
        }

        private static void TryPartyToTownStash()
        {
            if (!RequireCampaign()) return;
            var behavior = GetTownStashBehavior();
            if (behavior == null)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Equipment Spawner: town stash behavior missing. Save + reload once with the mod enabled."));
                return;
            }
            Settlement s = GetCurrentOwnedSettlement();
            if (s == null)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Equipment Spawner: town stash requires being inside a settlement your clan owns."));
                return;
            }

            var stash = behavior.GetOrCreateStashFor(s);
            TransferAll(MobileParty.MainParty.ItemRoster, stash, "party -> " + s.Name.ToString() + " stash", stash.Count);
        }

        private static void TryTownStashToParty()
        {
            if (!RequireCampaign()) return;
            var behavior = GetTownStashBehavior();
            if (behavior == null)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Equipment Spawner: town stash behavior missing. Save + reload once with the mod enabled."));
                return;
            }
            Settlement s = GetCurrentOwnedSettlement();
            if (s == null)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Equipment Spawner: town stash requires being inside a settlement your clan owns."));
                return;
            }

            var stash = behavior.GetOrCreateStashFor(s);
            if (stash.Count == 0)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Equipment Spawner: " + s.Name.ToString() + " stash is empty."));
                return;
            }
            TransferAll(stash, MobileParty.MainParty.ItemRoster, s.Name.ToString() + " stash -> party", -1);
        }

        // Bulk-transfer all gear+mount stacks from src to dst; both rosters use ItemRoster ops.
        private static void TransferAll(ItemRoster src, ItemRoster dst, string label, int postCountHint)
        {
            int stacks = 0, totalCount = 0;
            for (int i = src.Count - 1; i >= 0; i--)
            {
                var elt = src.GetElementCopyAtIndex(i);
                if (elt.EquipmentElement.Item == null) continue;
                if (!IsTransferable(elt.EquipmentElement.Item.ItemType)) continue;
                int n = elt.Amount;
                if (n <= 0) continue;
                dst.AddToCounts(elt.EquipmentElement, n);
                src.AddToCounts(elt.EquipmentElement, -n);
                stacks++;
                totalCount += n;
            }
            string tail = postCountHint >= 0
                ? " Destination now: " + (postCountHint + stacks) + " stacks."
                : " Destination now: " + dst.Count + " stacks.";
            InformationManager.DisplayMessage(new InformationMessage(
                "Equipment Spawner: moved " + stacks + " stacks (" + totalCount + " items) " + label + "." + tail));
        }

        private static ItemModifier BestModifier(ItemObject item)
        {
            var group = item.ItemComponent?.ItemModifierGroup;
            if (group == null) return null;
            var mods = group.ItemModifiers;
            if (mods == null || mods.Count == 0) return null;

            ItemModifier best = null;
            for (int i = 0; i < mods.Count; i++)
            {
                var m = mods[i];
                if (m == null) continue;
                if (best == null || (int)m.ItemQuality > (int)best.ItemQuality)
                {
                    best = m;
                }
            }
            return best;
        }

        private static bool IsGear(ItemObject.ItemTypeEnum t)
        {
            switch (t)
            {
                case ItemObject.ItemTypeEnum.HeadArmor:
                case ItemObject.ItemTypeEnum.BodyArmor:
                case ItemObject.ItemTypeEnum.ChestArmor:
                case ItemObject.ItemTypeEnum.LegArmor:
                case ItemObject.ItemTypeEnum.HandArmor:
                case ItemObject.ItemTypeEnum.Cape:
                case ItemObject.ItemTypeEnum.Shield:
                case ItemObject.ItemTypeEnum.OneHandedWeapon:
                case ItemObject.ItemTypeEnum.TwoHandedWeapon:
                case ItemObject.ItemTypeEnum.Polearm:
                case ItemObject.ItemTypeEnum.Bow:
                case ItemObject.ItemTypeEnum.Crossbow:
                case ItemObject.ItemTypeEnum.Thrown:
                case ItemObject.ItemTypeEnum.Sling:
                case ItemObject.ItemTypeEnum.Pistol:
                case ItemObject.ItemTypeEnum.Musket:
                case ItemObject.ItemTypeEnum.Arrows:
                case ItemObject.ItemTypeEnum.Bolts:
                case ItemObject.ItemTypeEnum.SlingStones:
                case ItemObject.ItemTypeEnum.Bullets:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsTransferable(ItemObject.ItemTypeEnum t)
        {
            if (IsGear(t)) return true;
            return t == ItemObject.ItemTypeEnum.Horse
                || t == ItemObject.ItemTypeEnum.HorseHarness;
        }
    }

    public class PersonalStashBehavior : CampaignBehaviorBase
    {
        private ItemRoster _stash = new ItemRoster();

        public ItemRoster Stash => _stash;

        public override void RegisterEvents()
        {
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("EquipmentSpawnerMod_PersonalStash", ref _stash);
            if (_stash == null) _stash = new ItemRoster();
        }
    }

    // Per-settlement stash: player can deposit into any clan-owned settlement's stash; retrieval requires being in that same settlement.
    // Bannerlord's SaveableBasicTypeDefiner only pre-registers Dictionary<int/string, int/string/float/object>
    // container definitions. Dictionary<string, ItemRoster> is NOT registered by default -> SyncData throws
    // during save, aborting the whole save file. See EquipmentSpawnerTypeDefiner below.
    public class TownStashBehavior : CampaignBehaviorBase
    {
        // key = Settlement.StringId (persistent, unique). value = that settlement's stash roster.
        private Dictionary<string, ItemRoster> _stashes = new Dictionary<string, ItemRoster>();

        public ItemRoster GetOrCreateStashFor(Settlement s)
        {
            if (s == null) return null;
            ItemRoster r;
            if (!_stashes.TryGetValue(s.StringId, out r) || r == null)
            {
                r = new ItemRoster();
                _stashes[s.StringId] = r;
            }
            return r;
        }

        public override void RegisterEvents()
        {
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("EquipmentSpawnerMod_TownStashes", ref _stashes);
            if (_stashes == null) _stashes = new Dictionary<string, ItemRoster>();
        }
    }

    // Registers extra saveable container types this mod uses. Bannerlord auto-discovers all
    // SaveableTypeDefiner subclasses in loaded mods and calls their Define* hooks during save-system init.
    // saveBaseId 9527100 is our unique namespace; vanilla uses 30000, most mods start >= 1_000_000.
    public class EquipmentSpawnerTypeDefiner : SaveableTypeDefiner
    {
        public EquipmentSpawnerTypeDefiner() : base(9527100)
        {
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(Dictionary<string, ItemRoster>));
        }
    }
}
