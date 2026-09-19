using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace Tier6Injector
{
    public class Tier6InjectorSubModule : MBSubModuleBase
    {
        private const int GearQuantity = 20;
        private const int HorseQuantity = 20;
        private const ItemObject.ItemTiers MinTier = ItemObject.ItemTiers.Tier3;

        private bool _hotkeyLatched;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            if (game.GameType is Campaign)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Tier Gear Injector loaded. Press Ctrl+Alt+I to add Tier 3-6 gear (best modifier) to your party."));
            }
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);

            bool ctrl = Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl);
            bool alt = Input.IsKeyDown(InputKey.LeftAlt) || Input.IsKeyDown(InputKey.RightAlt);
            bool i = Input.IsKeyDown(InputKey.I);
            bool combo = ctrl && alt && i;

            if (combo && !_hotkeyLatched)
            {
                _hotkeyLatched = true;
                TryInject();
            }
            else if (!combo)
            {
                _hotkeyLatched = false;
            }
        }

        private static void TryInject()
        {
            if (Campaign.Current == null || MobileParty.MainParty == null)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Tier Gear Injector: no active campaign / main party. Load a save first."));
                return;
            }

            var roster = MobileParty.MainParty.ItemRoster;
            var items = MBObjectManager.Instance.GetObjectTypeList<ItemObject>();

            int gearStacks = 0, horseStacks = 0, modified = 0;
            foreach (var item in items)
            {
                if (item == null) continue;
                if (item.Tier < MinTier) continue;

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
            }

            InformationManager.DisplayMessage(new InformationMessage(
                "Tier Gear Injector: injected " + gearStacks + " gear x" + GearQuantity
                + ", " + horseStacks + " mount x" + HorseQuantity
                + " (Tier " + ((int)MinTier + 1) + "-6, " + modified + " with best modifier)."));
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
    }
}
