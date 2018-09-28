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

		private List<Item> _pool;

		public ItemGenerator(IItemPlacementFlags flags, IncentiveData incentives, List<Item> treasurePool)
		{
			List<Item> items = new List<Item>(ItemLists.AllConsumables);
			List<Item> gold = new List<Item>(ItemLists.AllGoldTreasure).Where(g => treasurePool.Contains(g)).ToList();

			List<Item> weapons = WeaponPool;
			List<Item> armor = WeaponPool;
			armor.Add(Item.ProCape);
			armor.Add(Item.Ribbon);

			// Type:          Random Chance:    Vanilla Chance:
			// Consumable     0.143             0.153...
			// Weapon         0.143             0.150...
			// Armor          0.286             0.218...
			// Gold           0.429             0.479...
			_pool = new List<Item>();
			_pool.AddRange(items);
			_pool.AddRange(weapons);
			_pool.AddRange(armor);
			_pool.AddRange(armor);
			_pool.AddRange(gold);
			_pool.AddRange(gold);
			_pool.AddRange(gold);
		}

		public Item GenerateItem(MT19337 rng)
		{
			return _pool.PickRandom(rng);
		}

	}
}
