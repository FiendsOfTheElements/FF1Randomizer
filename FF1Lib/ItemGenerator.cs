using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FF1Lib
{
	public class ItemGenerator
	{
		// Similar to vanilla, but without duplicates, and with CatClaw
		private static readonly List<Item> WeaponPool =
			new List<Item> {
				Item.ShortSword, Item.LargeKnife, Item.Sabre, Item.LongSword,
				Item.GreatAxe, Item.Falchon, Item.SilverKnife, Item.SilverHammer,
				Item.SilverAxe, Item.FlameSword, Item.IceSword, Item.DragonSword,
				Item.GiantSword, Item.SunSword, Item.CoralSword, Item.WereSword,
				Item.RuneSword, Item.PowerRod, Item.LightAxe, Item.HealRod,
				Item.MageRod, Item.Defense, Item.WizardRod, Item.Vorpal,
				Item.CatClaw, Item.ThorHammer, Item.BaneSword, Item.Katana,
				Item.Xcalber, Item.Masamune, Item.IronStaff,
			};

		// Similar to vanilla, but without duplicates or wood, and with Buckler
		private static readonly List<Item> ArmorPool =
			new List<Item> {
				Item.Cloth, Item.IronArmor, Item.FlameArmor, Item.IceArmor,
				Item.OpalArmor, Item.DragonArmor, Item.Copper, Item.Silver,
				Item.Gold, Item.Opal, Item.WhiteShirt, Item.BlackShirt,
				Item.IronShield, Item.SilverShield, Item.FlameShield, Item.IceShield,
				Item.OpalShield, Item.AegisShield, Item.Buckler, Item.ProCape,
				Item.Cap, Item.IronHelm, Item.SilverHelm, Item.OpalHelm,
				Item.HealHelm, Item.Ribbon, Item.IronGauntlets, Item.SilverGauntlets,
				Item.ZeusGauntlets, Item.OpalGauntlets, Item.ProRing,
			};

		private List<Item> _items, _weapons, _armor, _gold;

		public ItemGenerator(IItemPlacementFlags flags, IncentiveData incentives, List<Item> treasurePool)
		{
			_items = ItemLists.AllConsumables.ToList();
			_gold = new List<Item>(ItemLists.AllGoldTreasure).Where(g => treasurePool.Contains(g)).ToList();
			_weapons = WeaponPool;
			_armor = ArmorPool;
			_armor.Add(Item.ProCape);
			_armor.Add(Item.Ribbon);
		}

		public Item GenerateItem(MT19337 rng)
		{
			// Type:          Vanilla Chance:  Random Chance:
			// Consumable      36 / 234         32 / 234
			// Weapon          35 / 234         32 / 234
			// Armor           51 / 234         64 / 234
			// Gold           112 / 234        106 / 234
			int dart = rng.Between(0, 233);
			var pool = dart < 32 ? _items : dart < 64 ? _weapons : dart < 128 ? _armor : _gold;
			return pool.PickRandom(rng);
		}

	}
}
