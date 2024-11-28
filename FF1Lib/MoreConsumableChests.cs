using System.ComponentModel;

namespace FF1Lib
{
	public enum ConsumableChestSet
	{
		[Description("Vanilla")]
		Vanilla,

		[Description("Low")]
		All10,

		[Description("Medium")]
		All15,

		[Description("High")]
		All20,

		[Description("Extreme")]
		All30,

		[Description("The ..... Party Pack")]
		AllPureSoft,

		[Description("Random Low")]
		RandomLow,

		[Description("Random")]
		Random
	}

	public enum ExtConsumableChestSet
	{
		[Description("None")]
		None,

		[Description("Low")]
		All1,

		[Description("Medium")]
		All3,

		[Description("High")]
		All5,

		[Description("Extreme")]
		All10,

		[Description("Random Low")]
		RandomLow,

		[Description("Random")]
		Random
	}

	public static class MoreConsumableChests
	{
		public static void Work(IItemPlacementFlags flags, List<Item> treasurePool, MT19337 rng)
		{
			int count = treasurePool.Count;

			var consumableChestSet = ConsumableChestSets[flags.MoreConsumableChests];
			var extConsumableChestSet = flags.ExtConsumableSet != ExtConsumableSet.None ? ExtConsumableChestSets[flags.ExtConsumableChests] : ExtConsumableChestSets[ExtConsumableChestSet.None];

			if (flags.MoreConsumableChests == ConsumableChestSet.Random || flags.MoreConsumableChests == ConsumableChestSet.RandomLow)
			{
				consumableChestSet =
				(
					Tents: rng.Between(0, consumableChestSet.Tents),
					Cabins: rng.Between(0, consumableChestSet.Cabins),
					Houses: rng.Between(0, consumableChestSet.Houses),
					Heals: rng.Between(0, consumableChestSet.Heals),
					Pures: rng.Between(0, consumableChestSet.Pures),
					Softs: rng.Between(0, consumableChestSet.Softs)
				);
			}

			if (flags.ExtConsumableSet != ExtConsumableSet.None && (flags.ExtConsumableChests == ExtConsumableChestSet.Random || flags.ExtConsumableChests == ExtConsumableChestSet.RandomLow))
			{
				extConsumableChestSet =
				(
					WoodenNunchucks: rng.Between(0, extConsumableChestSet.WoodenNunchucks),
					SmallKnives: rng.Between(0, extConsumableChestSet.SmallKnives),
					WoodenRods: rng.Between(0, extConsumableChestSet.WoodenRods),
					Rapiers: rng.Between(0, extConsumableChestSet.Rapiers)
				);
			}

			int requestedchests = consumableChestSet.Tents + consumableChestSet.Cabins + consumableChestSet.Houses + consumableChestSet.Heals + consumableChestSet.Pures + consumableChestSet.Softs + extConsumableChestSet.WoodenNunchucks + extConsumableChestSet.SmallKnives + extConsumableChestSet.WoodenRods + extConsumableChestSet.Rapiers;

			int removedchests = 0;

			RemoveConsumableChests(flags, treasurePool, ref removedchests);
			RemoveGoldChests(treasurePool, requestedchests, ref removedchests);

			if (flags.ExtConsumableSet != ExtConsumableSet.None)
			{
				AddConsumableChests(treasurePool, extConsumableChestSet.WoodenNunchucks, Item.WoodenNunchucks, ref removedchests);
				AddConsumableChests(treasurePool, extConsumableChestSet.SmallKnives, Item.SmallKnife, ref removedchests);
				AddConsumableChests(treasurePool, extConsumableChestSet.WoodenRods, Item.WoodenRod, ref removedchests);
				AddConsumableChests(treasurePool, extConsumableChestSet.Rapiers, Item.Rapier, ref removedchests);
			}

			AddConsumableChests(treasurePool, consumableChestSet.Tents, Item.Tent, ref removedchests);
			AddConsumableChests(treasurePool, consumableChestSet.Cabins, Item.Cabin, ref removedchests);
			AddConsumableChests(treasurePool, consumableChestSet.Houses, Item.House, ref removedchests);
			AddConsumableChests(treasurePool, consumableChestSet.Heals, Item.Heal, ref removedchests);
			AddConsumableChests(treasurePool, consumableChestSet.Pures, Item.Pure, ref removedchests);
			AddConsumableChests(treasurePool, consumableChestSet.Softs, Item.Soft, ref removedchests);

			AddGoldChests(treasurePool, removedchests);

			if (treasurePool.Count != count) throw new Exception("Sorry, I f****d it up!");
		}

		private static void AddGoldChests(List<Item> treasurePool, int removedchests)
		{
			for (int i = 0; i < removedchests; i++) treasurePool.Add(ChestReductionList[i]);
		}

		private static void RemoveGoldChests(List<Item> treasurePool, int requestedchests, ref int removedchests)
		{
			for (int i = 0; i < ChestReductionList.Length; i++)
			{
				if (removedchests >= requestedchests) break;
				if (treasurePool.Remove(ChestReductionList[i])) removedchests++;
			}
		}

		private static void RemoveConsumableChests(IItemPlacementFlags flags, List<Item> treasurePool, ref int removedchests)
		{
			var consumableChests = treasurePool.Where(i => i == Item.Tent || i == Item.Cabin || i == Item.House || i == Item.Heal || i == Item.Pure || i == Item.Soft).ToList();

			if (flags.ExtConsumableSet != ExtConsumableSet.None)
			{
				consumableChests.AddRange(treasurePool.Where(i => i == Item.WoodenNunchucks || i == Item.SmallKnife || i == Item.WoodenRod || i == Item.Rapier).ToList());
			}

			foreach (var item in consumableChests)
			{
				if (treasurePool.Remove(item)) removedchests++;
			}
		}

		private static void AddConsumableChests(List<Item> treasurePool, int count, Item item, ref int removedchests)
		{
			for (int i = 0; i < count; i++)
			{
				if (removedchests > 0)
				{
					removedchests--;
					treasurePool.Add(item);
				}
			}
		}

		private static Dictionary<ConsumableChestSet, (int Tents, int Cabins, int Houses, int Heals, int Pures, int Softs)> ConsumableChestSets = new Dictionary<ConsumableChestSet, (int Tents, int Cabins, int Houses, int Heals, int Pures, int Softs)>
		{
			{ConsumableChestSet.Vanilla, (2, 7, 7, 10, 5, 5) },
			{ConsumableChestSet.All10, (4, 3, 3, 10, 10, 10) },
			{ConsumableChestSet.All15, (7, 4, 4, 15, 15, 15) },
			{ConsumableChestSet.All20, (8, 6, 6, 20, 20, 20) },
			{ConsumableChestSet.All30, (10, 10, 10, 30, 30, 30) },
			{ConsumableChestSet.AllPureSoft, (0, 0, 0, 0, 99, 99) },
			{ConsumableChestSet.RandomLow, (10, 10, 10, 30, 30, 30) },
			{ConsumableChestSet.Random, (33, 33, 33, 99, 99, 99) },
		};

		private static Dictionary<ExtConsumableChestSet, (int WoodenNunchucks, int SmallKnives, int WoodenRods, int Rapiers)> ExtConsumableChestSets = new Dictionary<ExtConsumableChestSet, (int WoodenNunchucks, int SmallKnives, int WoodenRods, int Rapiers)>
		{
			{ExtConsumableChestSet.None, (0, 0, 0, 0) },
			{ExtConsumableChestSet.All1, (1, 1, 1, 1) },
			{ExtConsumableChestSet.All3, (3, 3, 3, 3) },
			{ExtConsumableChestSet.All5, (5, 5, 5, 5) },
			{ExtConsumableChestSet.All10, (10, 10, 10, 10) },
			{ExtConsumableChestSet.RandomLow, (5, 5, 5, 5) },
			{ExtConsumableChestSet.Random, (15, 15, 15, 15) },
		};

		private static Item[] ChestReductionList = new Item[]
		{
			Item.Gold10,
			Item.Gold20,
			Item.Gold110,
			Item.Gold155,
			Item.Gold160,
			Item.Gold180,
			Item.Gold295,
			Item.Gold330,
			Item.Gold385,
			Item.Gold400,
			Item.Gold450,
			Item.Gold500,
			Item.Gold575,
			Item.Gold620,
			Item.Gold680,
			Item.Gold750,
			Item.Gold795,
			Item.Gold880,
			Item.Gold1020,
			Item.Gold1250,
			Item.Gold1455,
			Item.Gold1520,
			Item.Gold1760,
			Item.Gold1975,
			Item.Gold2000,
			Item.Gold2750,
			Item.Gold3400,
			Item.Gold4150,
			Item.Gold5000,
			Item.Gold5450,
			Item.Gold6400,
			Item.Gold6720,
			Item.Gold7340,
			Item.Gold7690,
			Item.Gold7900,
			Item.Gold8135,
			Item.Gold9000,
			Item.Gold9500,
			Item.Gold9900,
			Item.Gold10000,
			Item.Gold12350,
			Item.Gold13000,
			Item.Gold13450,
			Item.Gold18010,
			Item.Gold26000,
			Item.Gold45000,
			Item.Gold65000,
			Item.Gold10,
			Item.Gold180,
			Item.Gold330,
			Item.Gold385,
			Item.Gold450,
			Item.Gold575,
			Item.Gold620,
			Item.Gold795,
			Item.Gold880,
			Item.Gold1020,
			Item.Gold1250,
			Item.Gold1455,
			Item.Gold1520,
			Item.Gold1760,
			Item.Gold1975,
			Item.Gold2000,
			Item.Gold2750,
			Item.Gold3400,
			Item.Gold4150,
			Item.Gold5000,
			Item.Gold5450,
			Item.Gold6400,
			Item.Gold7340,
			Item.Gold7690,
			Item.Gold7900,
			Item.Gold8135,
			Item.Gold9000,
			Item.Gold9500,
			Item.Gold9900,
			Item.Gold10000,
			Item.Gold12350,
			Item.Gold13000,
			Item.WoodenRod,
			Item.SilverKnife,
			Item.IceSword,
			Item.LightAxe,
			Item.Cloth,
			Item.IronArmor,
			Item.SilverShield,
			Item.OpalShield,
			Item.SilverHelm,
			Item.HealHelm,
			Item.IronGauntlets,
			Item.SilverGauntlets,
			Item.OpalGauntlets,
			Item.SilverHelm,
			Item.SilverHelm,
			Item.Gold10,
			Item.Gold450,
			Item.Gold575,
			Item.Gold880,
			Item.Gold1455,
			Item.Gold1520,
			Item.Gold1760,
			Item.Gold2000,
			Item.Gold2750,
			Item.Gold3400,
			Item.Gold4150,
			Item.Gold5000,
			Item.Gold5450,
			Item.Gold7340,
			Item.Gold7900,
			Item.Gold8135,
			Item.Gold9500,
			Item.Gold9900,
			Item.Gold10000,
			Item.Gold12350,
			Item.WoodenRod,
			Item.ShortSword,
			Item.LargeKnife,
			Item.IronStaff,
			Item.Sabre,
			Item.GreatAxe,
			Item.Falchon,
			Item.SilverKnife,
			Item.SilverHammer,
			Item.SilverAxe,
			Item.IceSword,
			Item.GiantSword,
			Item.SunSword,
			Item.CoralSword,
			Item.WereSword,
			Item.RuneSword,
			Item.PowerRod,
			Item.LightAxe,
			Item.Defense,
			Item.ThorHammer,
			Item.BaneSword,
			Item.Cloth,
			Item.IronArmor,
			Item.SilverArmor,
			Item.IceArmor,
			Item.Copper,
			Item.Silver,
			Item.Gold,
			Item.WoodenShield,
			Item.IronShield,
			Item.SilverShield,
			Item.Cap,
			Item.WoodenHelm,
			Item.IronHelm,
			Item.SilverHelm,
			Item.CopperGauntlets,
			Item.IronGauntlets,
			Item.SilverGauntlets,
			Item.Gold575,
			Item.Gold880,
			Item.Gold1455,
			Item.Gold1520,
			Item.Gold2750,
			Item.Gold3400,
			Item.Gold4150,
			Item.Gold5000,
			Item.Gold5450,
			Item.Gold9900,
			Item.Gold2750,
			Item.Gold5000,
			Item.Gold2750
		};
	}
}
