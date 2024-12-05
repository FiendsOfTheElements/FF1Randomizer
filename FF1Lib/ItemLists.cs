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

		public static readonly IReadOnlyList<Item> LegendaryArmorTier =
		new List<Item> {
			Item.OpalArmor, Item.DragonArmor, Item.Opal,
			Item.OpalShield, Item.AegisShield,
			Item.Ribbon, Item.Ribbon, Item.Ribbon,
			Item.PowerGauntlets,
		};

		public static readonly IReadOnlyList<Item> RareWeaponTier =
		new List<Item> {
			Item.FlameSword, Item.IceSword, Item.DragonSword, Item.GiantSword,
			Item.SunSword, Item.CoralSword, Item.WereSword, Item.RuneSword,
			Item.LightAxe, Item.HealRod, Item.MageRod, Item.Defense,
			Item.WizardRod, Item.CatClaw, Item.ThorHammer, Item.BaneSword,
			Item.PowerRod, // Usually spellcasting and good in most flagsets; ideally added situationally if this section gets overhauled
		};

		public static readonly IReadOnlyList<Item> RareArmorTier =
		new List<Item> {
			Item.OpalHelm, Item.OpalGauntlets,
			Item.SteelArmor, Item.FlameArmor, Item.IceArmor, Item.Gold,
			Item.WhiteShirt, Item.BlackShirt, Item.FlameShield, Item.IceShield,
			Item.ProCape, Item.ProCape, Item.HealHelm,
			Item.ZeusGauntlets, Item.ProRing, Item.ProRing,
		};

		public static readonly IReadOnlyList<Item> CommonWeaponTier =
		new List<Item>
		{
			Item.WoodenNunchucks, Item.SmallKnife, Item.WoodenRod,
			Item.Rapier, Item.IronHammer, Item.ShortSword, Item.HandAxe,
			Item.Scimitar, Item.IronNunchucks, Item.LargeKnife, Item.IronStaff,
			Item.Sabre, Item.LongSword, Item.GreatAxe, Item.Falchon, Item.SilverKnife,
			Item.SilverSword, Item.SilverHammer, Item.SilverAxe, 
		};

		public static readonly IReadOnlyList<Item> CommonArmorTier =
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
				.Where(x => x >= Item.Ship || (x > Item.None && x <= Item.Oxyale))
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

		public static readonly IReadOnlyCollection<Item> UnusedGoldItems =
			new List<Item>
			{
				Item.Gold25, Item.Gold30, Item.Gold55, Item.Gold70, Item.Gold85, Item.Gold135,
				Item.Gold240, Item.Gold255, Item.Gold260, Item.Gold300, Item.Gold315, Item.Gold350,
				Item.Gold530, Item.Gold9300, Item.Gold14050, Item.Gold14720, Item.Gold15000,
				Item.Gold17490, Item.Gold19990, Item.Gold20000, Item.Gold20010
			};

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
