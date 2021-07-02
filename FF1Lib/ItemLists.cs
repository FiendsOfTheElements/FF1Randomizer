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

		public static readonly IReadOnlyList<Item> UberTier =
		new List<Item> {
			Item.Masamune,
		};

		public static readonly IReadOnlyList<Item> LegendaryWeaponTier =
		new List<Item> {
			Item.Vorpal, Item.Katana, Item.Xcalber,
		};

		public static readonly IReadOnlyCollection<Item> LegendaryArmorTier =
		new List<Item> {
			Item.OpalArmor, Item.DragonArmor, Item.Opal, Item.OpalShield,
			Item.OpalShield, Item.AegisShield, Item.OpalHelm, Item.Ribbon,
			Item.Ribbon, Item.Ribbon, Item.OpalGauntlets, Item.OpalGauntlets,
		};

		public static readonly IReadOnlyList<Item> RareWeaponTier =
		new List<Item> {
			Item.FlameSword, Item.IceSword, Item.DragonSword, Item.GiantSword,
			Item.SunSword, Item.CoralSword, Item.WereSword, Item.RuneSword,
			Item.LightAxe, Item.HealRod, Item.MageRod, Item.Defense,
			Item.WizardRod, Item.CatClaw, Item.ThorHammer, Item.BaneSword,
		};

		public static readonly IReadOnlyCollection<Item> RareArmorTier =
		new List<Item> {
			Item.SteelArmor, Item.FlameArmor, Item.IceArmor, Item.Gold,
			Item.WhiteShirt, Item.BlackShirt, Item.FlameShield, Item.IceShield,
			Item.ProCape, Item.ProCape, Item.HealHelm,
			Item.ZeusGauntlets, Item.PowerGauntlets, Item.ProRing, Item.ProRing,
		};

		public static readonly IReadOnlyList<Item> CommonWeaponTier =
		new List<Item>
		{
			Item.WoodenNunchucks, Item.SmallKnife, Item.WoodenRod,
			Item.Rapier, Item.IronHammer, Item.ShortSword, Item.HandAxe,
			Item.Scimitar, Item.IronNunchucks, Item.LargeKnife, Item.IronStaff,
			Item.Sabre, Item.LongSword, Item.GreatAxe, Item.Falchon, Item.SilverKnife,
			Item.SilverSword, Item.SilverHammer, Item.SilverAxe, Item.PowerRod,
		};

		public static readonly IReadOnlyCollection<Item> CommonArmorTier =
		new List<Item>
		{
			Item.Cloth, Item.WoodenArmor, Item.ChainArmor, Item.IronArmor, Item.SilverArmor,
			Item.Copper, Item.Silver, Item.WoodenShield, Item.IronShield, Item.SilverShield,
			Item.Buckler, Item.Cap, Item.WoodenHelm, Item.IronHelm, Item.SilverHelm,
			Item.Gloves, Item.CopperGauntlets, Item.IronGauntlets, Item.SilverGauntlets
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

		public static readonly IReadOnlyCollection<Item> SpecialGear =
			new List<Item> {
				Item.LightAxe, Item.ThorHammer, Item.Defense, Item.WizardRod, Item.MageRod, Item.BaneSword,
				Item.Xcalber, Item.SunSword, Item.Masamune, Item.Katana, Item.Vorpal, Item.HealRod,
				Item.HealHelm, Item.WhiteShirt, Item.BlackShirt, Item.PowerGauntlets, Item.ZeusGauntlets,
				Item.DragonArmor, Item.AegisShield, Item.Ribbon, Item.Opal
			};

		public static readonly IReadOnlyCollection<Item> AllGoldTreasure =
			Enum.GetValues(typeof(Item))
				.Cast<Item>()
				.Where(x => x >= Item.Gold10 && x <= Item.Gold65000)
				.ToList();

		public static readonly IReadOnlyCollection<Item> BigGoldTreasure =
			Enum.GetValues(typeof(Item))
				.Cast<Item>()
				.Where(x => x >= Item.Gold5000 && x <= Item.Gold65000)
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

		public static List<Item> GetIncentivePool(Flags flags)
		{
			var incentivePool = new List<Item>();

			if (flags.IncentivizeMasamune ?? false) incentivePool.Add(Item.Masamune);
			if (flags.IncentivizeKatana ?? false) incentivePool.Add(Item.Katana);
			if (flags.IncentivizeVorpal ?? false) incentivePool.Add(Item.Vorpal);
			if (flags.IncentivizeDefCastWeapon ?? false) incentivePool.Add(Item.Defense);
			if (flags.IncentivizeOffCastWeapon ?? false) incentivePool.Add(Item.ThorHammer);
			if (flags.IncentivizeOpal ?? false) incentivePool.Add(Item.Opal);
			if (flags.IncentivizeOtherCastArmor ?? false) incentivePool.Add(Item.PowerGauntlets);
			if (flags.IncentivizeDefCastArmor ?? false) incentivePool.Add(Item.WhiteShirt);
			if (flags.IncentivizeOffCastArmor ?? false) incentivePool.Add(Item.BlackShirt);
			if (flags.IncentivizeRibbon ?? false) incentivePool.Add(Item.Ribbon);
			if (flags.IncentivizeSlab ?? false) incentivePool.Add(Item.Slab);
			if (flags.IncentivizeRuby ?? false) incentivePool.Add(Item.Ruby);
			if (flags.IncentivizeFloater ?? false) incentivePool.Add(Item.Floater);
			if (flags.IncentivizeTnt ?? false) incentivePool.Add(Item.Tnt);
			if (flags.IncentivizeCrown ?? false) incentivePool.Add(Item.Crown);
			if (flags.IncentivizeTail ?? false) incentivePool.Add(Item.Tail);
			if (flags.IncentivizeAdamant ?? false) incentivePool.Add(Item.Adamant);

			if (flags.IncentivizeBridge) incentivePool.Add(Item.Bridge);
			if (flags.IncentivizeLute ?? false) incentivePool.Add(Item.Lute);
			if (flags.IncentivizeShip ?? false) incentivePool.Add(Item.Ship);
			if (flags.IncentivizeRod ?? false) incentivePool.Add(Item.Rod);
			if (flags.IncentivizeCanoe ?? false) incentivePool.Add(Item.Canoe);
			if (flags.IncentivizeCube ?? false) incentivePool.Add(Item.Cube);
			if (flags.IncentivizeBottle ?? false) incentivePool.Add(Item.Bottle);

			if (flags.IncentivizeKey ?? false) incentivePool.Add(Item.Key);
			if (flags.IncentivizeCrystal ?? false) incentivePool.Add(Item.Crystal);
			if (flags.IncentivizeOxyale ?? false) incentivePool.Add(Item.Oxyale);
			if (flags.IncentivizeCanal ?? false) incentivePool.Add(Item.Canal);
			if (flags.IncentivizeHerb ?? false) incentivePool.Add(Item.Herb);
			if (flags.IncentivizeChime ?? false) incentivePool.Add(Item.Chime);
			if (flags.IncentivizeXcalber ?? false) incentivePool.Add(Item.Xcalber);

			return incentivePool.Concat(ItemLists.AllQuestItems).Distinct().ToList();
		}

		public static List<string> GetIncentiveChests(Flags flags)
		{
			var incentivizedChests = new List<string>();

			if (flags.IncentivizeEarth ?? false) incentivizedChests.Add(ItemLocations.EarthCaveMajor.Name);
			if (flags.IncentivizeIceCave ?? false) incentivizedChests.Add(ItemLocations.IceCaveMajor.Name);
			if (flags.IncentivizeMarsh ?? false) incentivizedChests.Add(ItemLocations.MarshCaveMajor.Name);
			if (flags.IncentivizeMarshKeyLocked ?? false) incentivizedChests.Add(ItemLocations.MarshCave13.Name);
			if (flags.IncentivizeOrdeals ?? false) incentivizedChests.Add(ItemLocations.OrdealsMajor.Name);
			if (flags.IncentivizeSeaShrine ?? false) incentivizedChests.Add(ItemLocations.SeaShrineMajor.Name);
			if (flags.IncentivizeSkyPalace ?? false) incentivizedChests.Add(ItemLocations.SkyPalaceMajor.Name);
			if (flags.IncentivizeTitansTrove ?? false) incentivizedChests.Add(ItemLocations.TitansTunnel1.Name);
			if (flags.IncentivizeVolcano ?? false) incentivizedChests.Add(ItemLocations.VolcanoMajor.Name);
			if (flags.IncentivizeConeria ?? false) incentivizedChests.Add(ItemLocations.ConeriaMajor.Name);

			return incentivizedChests;
		}
	}
}
