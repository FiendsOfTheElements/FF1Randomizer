using System;
using System.Collections.Generic;
using System.Linq;
namespace FF1Lib
{
	public static class ItemLists
	{
		public static readonly IReadOnlyCollection<Item> Tier1Requirements =
			new List<Item> {
			Item.EarthOrb, Item.FireOrb, Item.WaterOrb, Item.AirOrb,
			Item.Canoe, Item.Floater, Item.Ship, Item.Bridge,
			Item.Lute, Item.Key, Item.Rod, Item.Oxyale, Item.Cube
		};

		public static readonly IReadOnlyCollection<Item> Tier2Requirements =
			new List<Item> {
			Item.Tnt, Item.Herb, Item.Crystal, Item.Crown,
			Item.Slab, Item.Chime, Item.Bottle, Item.Ruby
		};

		public static readonly IReadOnlyCollection<Item> NiceOptionals1 =
			new List<Item> {
			Item.Tail, Item.Adamant, Item.Masamune, Item.Xcalber, Item.Ribbon
		};

		public static readonly IReadOnlyCollection<Item> NiceOptionals2 =
			new List<Item> {
			Item.ProRing, Item.ProCape, Item.Opal, Item.DragonArmor, Item.Katana,
			Item.WhiteShirt, Item.BlackShirt, Item.PowerGauntlets, Item.ZeusGauntlets,
			Item.Defense
		};

		public static readonly IReadOnlyCollection<Item> NiceOptionals3 =
			new List<Item> {
			Item.HealHelm, Item.AegisShield, Item.BaneSword, Item.ThorHammer,
			Item.MageRod, Item.WizardRod, Item.LightAxe, Item.HealRod, Item.Gold65000
		};

		public static readonly IReadOnlyCollection<Item> AllConsumables =
			new List<Item> {
			Item.Tent, Item.Cabin, Item.House, Item.Heal, Item.Pure, Item.Soft
		};

		public static readonly IReadOnlyCollection<Item> AllOrbs =
			new List<Item> {
			Item.EarthOrb, Item.FireOrb, Item.WaterOrb, Item.AirOrb
		};

		public static readonly IReadOnlyCollection<Item> AllNonTreasureChestItems =
			new List<Item> {
			Item.Bridge, Item.Ship, Item.Canoe, Item.Canal,
			Item.Lute, Item.Crystal, Item.Herb, Item.Key,
			Item.Rod, Item.Cube, Item.Bottle, Item.Oxyale,
			Item.Chime, Item.Xcalber
		};

		public static readonly IReadOnlyCollection<Item> AllQuestItems =
			Enum.GetValues(typeof(Item))
				.Cast<Item>()
				.Where(x => x > Item.Gold65000 || (x > Item.None && x <= Item.Oxyale))
				.ToList();

		public static readonly IReadOnlyCollection<Item> AllWeapons =
			Enum.GetValues(typeof(Item))
				.Cast<Item>()
				.Where(x => x >= Item.WoodenNunchucks && x <= Item.Masamune)
				.ToList();

		public static readonly IReadOnlyCollection<Item> AllArmor =
			Enum.GetValues(typeof(Item))
				.Cast<Item>()
				.Where(x => x >= Item.Cloth && x <= Item.ProRing)
				.ToList();

		public static readonly IReadOnlyCollection<Item> AllGoldTreasure =
			Enum.GetValues(typeof(Item))
				.Cast<Item>()
				.Where(x => x >= Item.Gold10 && x <= Item.Gold65000)
				.ToList();

		public static readonly IReadOnlyCollection<Item> AllMagicItem =
			new List<Item> {
			Item.WhiteShirt, Item.BlackShirt, Item.HealHelm, Item.ZeusGauntlets,
			Item.PowerGauntlets, Item.LightAxe, Item.HealRod, Item.MageRod,
			Item.Defense, Item.WizardRod, Item.ThorHammer, Item.BaneSword
			};

		public static readonly Dictionary<byte, string> TextLookup =
			Enum.GetValues(typeof(Item))
				.Cast<byte>()
				.ToDictionary(x => x,
							  x => Enum.GetName(typeof(Item), x));
	}
}
