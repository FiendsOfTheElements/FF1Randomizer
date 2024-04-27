using RomUtilities;
using System.ComponentModel;
using System.Reflection;

namespace FF1Lib
{
	public enum IncentivePlacementType
	{
		[Description("Vanilla")]
		Vanilla,
		[Description("Random")]
		RandomAtLocation
	}

	public enum IncentivePlacementTypeEarth
	{
		[Description("Vanilla")]
		Vanilla,
		[Description("Random")]
		RandomAtLocation,
		[Description("Random Pre-Rod")]
		RandomPreRod,
		[Description("Random Post-Rod")]
		RandomPostRod
	}
	public enum IncentivePlacementTypeSky
	{
		[Description("Vanilla")]
		Vanilla,
		[Description("Random")]
		RandomAtLocation,
		[Description("Random Mirage")]
		RandomMirage,
		[Description("Random Sky")]
		RandomSky
	}
	public enum IncentivePlacementTypeSea
	{
		[Description("Vanilla")]
		Vanilla,
		[Description("Random")]
		RandomAtLocation,
		[Description("Random Unlocked")]
		RandomUnlocked,
		[Description("Random Locked")]
		RandomLocked
	}
	public enum IncentivePlacementTypeVolcano
	{
		[Description("Vanilla")]
		Vanilla,
		[Description("Random")]
		RandomAtLocation,
		[Description("Random Deep")]
		RandomDeep,
		[Description("Random Shallow")]
		RandomShallow
	}

	public class IncentiveData
	{
		public ItemShopSlot ShopSlot { get; set; }
		public IncentiveData(FF1Rom rom, MT19337 rng, List<(string, Item)> plandoItems, Flags flags)
		{
			ShopSlot = ItemLocations.CaravanItemShop1;

			ProcessIncentiveData(plandoItems, rng, flags);
			ProcessShopData(flags);
			ProcessTreasurePool(rom, rng, flags);
		}
		private void ProcessIncentiveData(List<(string, Item)> plandoItems, MT19337 rng, Flags flags)
		{
			// Process flags
			List<(bool, Item)> incentivizedItemsFlags = new() {
				((bool)flags.IncentivizeBridge, Item.Bridge),
				((bool)flags.IncentivizeShip, Item.Ship),
				((bool)flags.IncentivizeCanal, Item.Canal),
				((bool)flags.IncentivizeLute, Item.Lute),
				((bool)flags.IncentivizeCrown, Item.Crown),
				((bool)flags.IncentivizeCrystal, Item.Crystal),
				((bool)flags.IncentivizeHerb, Item.Herb),
				((bool)flags.IncentivizeKey, Item.Key),
				((bool)flags.IncentivizeTnt, Item.Tnt),
				((bool)flags.IncentivizeAdamant, Item.Adamant),
				((bool)flags.IncentivizeSlab, Item.Slab),
				((bool)flags.IncentivizeRuby, Item.Ruby),
				((bool)flags.IncentivizeRod, Item.Rod),
				((bool)flags.IncentivizeFloater, Item.Floater),
				((bool)flags.IncentivizeChime, Item.Chime),
				((bool)flags.IncentivizePromotion, Item.Tail),
				((bool)flags.IncentivizeCube, Item.Cube),
				((bool)flags.IncentivizeBottle, Item.Bottle),
				((bool)flags.IncentivizeOxyale, Item.Oxyale),
				((bool)flags.IncentivizeCanoe, Item.Canoe),
				((bool)flags.IncentivizeXcalber, Item.Xcalber),
				((bool)flags.IncentivizeMasamune, Item.Masamune),
				((bool)flags.IncentivizeKatana, Item.Katana),
				((bool)flags.IncentivizeVorpal, Item.Vorpal),
				((bool)flags.IncentivizeRibbon, Item.Ribbon),
				((bool)flags.IncentivizeRibbon2, Item.Ribbon),
				((bool)flags.IncentivizeOpal, Item.Opal),
				((bool)flags.Incentivize65K, Item.Gold65000),
				((bool)flags.IncentivizeBad, Item.Cloth),
				((bool)flags.IncentivizeDefCastArmor, Item.WhiteShirt),
				((bool)flags.IncentivizeOffCastArmor, Item.BlackShirt),
				((bool)flags.IncentivizeOtherCastArmor, Item.PowerGauntlets),
				((bool)flags.IncentivizePowerRod, Item.PowerRod),
				((bool)flags.IncentivizeDefCastWeapon, Item.Defense),
				((bool)flags.IncentivizeOffCastWeapon, Item.ThorHammer),
				((bool)flags.IncentivizeOtherCastWeapon, Item.BaneSword),
			};

			List<(bool, IRewardSource)> incentivizedNpcsFlags = new() {
				((bool)flags.IncentivizeKingConeria, ItemLocations.KingConeria),
				((bool)flags.IncentivizePrincess, ItemLocations.Princess),
				((bool)flags.IncentivizeMatoya, ItemLocations.Matoya),
				((bool)flags.IncentivizeBikke, ItemLocations.Bikke),
				((bool)flags.IncentivizeElfPrince, ItemLocations.ElfPrince),
				((bool)flags.IncentivizeAstos, ItemLocations.Astos),
				((bool)flags.IncentivizeNerrick, ItemLocations.Nerrick),
				((bool)flags.IncentivizeSmith, ItemLocations.Smith),
				((bool)flags.IncentivizeSarda, ItemLocations.Sarda),
				((bool)flags.IncentivizeCanoeSage, ItemLocations.CanoeSage),
				((bool)flags.IncentivizeCubeBot, ItemLocations.CubeBot),
				((bool)flags.IncentivizeFairy, ItemLocations.Fairy),
				((bool)flags.IncentivizeLefein, ItemLocations.Lefein),
				((bool)flags.IncentivizeCaravan, ItemLocations.CaravanItemShop1),
			};

			List<(bool, Item)> removedItemsFlags = new()
			{
				((bool)flags.NoMasamune, Item.Masamune),
				((bool)flags.NoXcalber, Item.Xcalber),
				((bool)flags.NoTail, Item.Tail),
				((bool)flags.IsFloaterRemoved, Item.Floater),
				//((bool)flags.IsCanoeFree, Item.Canoe),
				//((bool)flags.FreeLute, Item.Lute),
				//((bool)flags.FreeRod, Item.Rod),
				//((bool)flags.FreeTail, Item.Tail),
				//((bool)flags.IsBridgeFree, Item.Bridge),
				//((bool)flags.IsCanalFree, Item.Canal),
				//((bool)flags.IsShipFree, Item.Ship),
			};

			List<(bool, Item)> freeItemsFlags = new()
			{
				((bool)flags.IsCanoeFree, Item.Canoe),
				((bool)flags.FreeLute, Item.Lute),
				((bool)flags.FreeRod, Item.Rod),
				((bool)flags.FreeTail, Item.Tail),
				((bool)flags.IsBridgeFree, Item.Bridge),
				((bool)flags.IsCanalFree, Item.Canal),
				((bool)flags.IsShipFree, Item.Ship),
			};

			List<(bool, IRewardSource)> removedNPCItemsLocations = new()
			{
				((bool)flags.IsCanoeFree, ItemLocations.CanoeSage),
				((bool)flags.FreeLute, ItemLocations.Princess),
				((bool)flags.FreeRod, ItemLocations.Sarda),
				((bool)flags.IsBridgeFree, ItemLocations.KingConeria),
				((bool)flags.IsShipFree, ItemLocations.Bikke),
			};

			List<(bool, IRewardSource)> removedNPCFetchItemsLocations = new()
			{
				((bool)flags.NoXcalber, ItemLocations.Smith),
				((bool)flags.IsCanalFree, ItemLocations.Nerrick),
			};

			List<(Item item, IRewardSource source)> removedItemsLocations = new()
			{
				(Item.Canoe, ItemLocations.CanoeSage),
				(Item.Lute, ItemLocations.Princess),
				(Item.Rod, ItemLocations.Sarda),
				(Item.Bridge, ItemLocations.KingConeria),
				(Item.Ship, ItemLocations.Bikke),
				(Item.Xcalber, ItemLocations.Smith),
				(Item.Canal, ItemLocations.Nerrick),
			};

			RemovedItems = removedItemsFlags.Where(i => i.Item1).Select(i => i.Item2).ToList();
			FreeItems = freeItemsFlags.Where(i => i.Item1).Select(i => i.Item2).ToList();

			List<Item> itemsToNotPlace = RemovedItems.Concat(FreeItems).ToList();

			// Process Plando/Forced placements
			var forcedItemPlacements = ItemLocations.AllOtherItemLocations.ToList();

			if (!(flags.NPCItems ?? false))
			{
				forcedItemPlacements.AddRange(ItemLocations.AllNPCFreeItemLocationsExcludingVendor.Except(removedItemsLocations.Where(x => itemsToNotPlace.Contains(x.item)).Select(x => x.source).ToList()));
				forcedItemPlacements.Add(ShopSlot);
			}

			if (!(flags.NPCFetchItems ?? false))
			{
				forcedItemPlacements.AddRange(ItemLocations.AllNPCFetchItemLocations.Except(removedItemsLocations.Where(x => itemsToNotPlace.Contains(x.item)).Select(x => x.source).ToList()));
			}
			else if (flags.NoOverworld)
			{
				forcedItemPlacements.Add(ItemLocations.Nerrick);
			}

			if ((!flags.Treasures ?? false)) forcedItemPlacements.AddRange(ItemLocations.AllTreasures);

			if (flags.GuaranteedMasamune ?? false)
			{
				forcedItemPlacements.Add(ItemLocations.ToFRMasmune);
			}

			//List<Item> removedItems = removedItemsFlags.Where(x => x.Item1 == true).Select(x => x.Item2).ToList();

			List<Item> incentivePool = incentivizedItemsFlags.Where(x => x.Item1 == true).Select(x => x.Item2).ToList();

			List<IRewardSource> incentiveLocationPool = incentivizedNpcsFlags.Where(x => x.Item1 == true).Select(x => x.Item2).ToList();

			incentiveLocationPool.AddRange(SelectIncentivizedChests(flags, rng));

			forcedItemPlacements.RemoveAll(i => plandoItems.Select(p => p.Item1).Contains(i.Name));

			Dictionary<string, IRewardSource> rewardSourceDict = typeof(ItemLocations)
				.GetFields()
				.Where(t => t.FieldType.BaseType == typeof(RewardSourceBase))
				.ToDictionary(t => t.Name, t => (IRewardSource)t.GetValue(null));

			foreach (var item in plandoItems)
			{
				forcedItemPlacements.Add(ItemPlacement.NewItemPlacement(rewardSourceDict[item.Item1], item.Item2));
			}

			var itemLocationPool =
				ItemLocations.AllTreasures.Concat(ItemLocations.AllNPCItemLocations)
						  .Where(x => !x.IsUnused && !forcedItemPlacements.Any(y => y.Address == x.Address))
						  .ToList();



			/*
			var test1 = typeof(ItemLocations).GetFields();
			var test2 = test1.Where(t => t.GetType() == typeof(TreasureChest) || t.FieldType.DeclaringType == typeof(NpcReward) || t.FieldType.DeclaringType == typeof(ItemShopSlot)).ToList();
			var test3 = test1.Where(t => t.FieldType.BaseType == typeof(RewardSourceBase)).ToList();
			var test4 = test3.ToDictionary(t => t.Name, t => (IRewardSource)t.GetValue(null));
			*/


			incentivePool = incentivePool
				.Except(RemovedItems)
				.Except(FreeItems)
				.Except(forcedItemPlacements.Select(x => x.Item).Where(x => x != Item.Masamune || !(bool)flags.GuaranteedMasamune))
				.ToList();

			/*
			foreach (var item in forcedItemPlacements.Select(x => x.Item))
			{
				if ((bool)flags.GuaranteedMasamune && item == Item.Masamune)
				{
					continue;
				}
				incentivePool.Remove(item);
			}

			foreach (var item in removedItems)
			{
				incentivePool.Remove(item);
			}*/

			KeyItemsToPlace = KeyItems
				.Except(RemovedItems)
				.Except(FreeItems)
				.Except(forcedItemPlacements.Select(f => f.Item)
				.ToList());

			List<MapLocation> endgameMapLocations = new() { MapLocation.TempleOfFiends2, MapLocation.TempleOfFiends3, MapLocation.TempleOfFiendsAir, MapLocation.TempleOfFiendsChaos, MapLocation.TempleOfFiendsEarth, MapLocation.TempleOfFiendsFire, MapLocation.TempleOfFiendsPhantom, MapLocation.TempleOfFiendsWater };
			List<MapLocation> nonEndgameMapLocations = Enum.GetValues<MapLocation>().Except(endgameMapLocations).ToList();

			ForcedItemPlacements = forcedItemPlacements.ToList();
			IncentiveItems = incentivePool.ToList();

			//RemovedItems = removedItems.ToList();

			IncentiveLocations = incentiveLocationPool
							 .Where(x => !forcedItemPlacements.Any(y => y.Address == x.Address))
							 .ToList();

			AllValidItemLocations = itemLocationPool.ToList();
			AllValidPreBlackOrbItemLocations = AllValidItemLocations
							 .Where(x => nonEndgameMapLocations.Contains(x.MapLocation) && nonEndgameMapLocations.Contains((x as NpcReward)?.SecondLocation ?? MapLocation.StartingLocation))
							 .ToList();
			AllValidPreBlackOrbItemLocationsPlusForced = itemLocationPool.Concat(forcedItemPlacements)
							 .Where(x => nonEndgameMapLocations.Contains(x.MapLocation) && nonEndgameMapLocations.Contains((x as NpcReward)?.SecondLocation ?? MapLocation.StartingLocation))
							 .ToList();
		}
		private void ProcessShopData(Flags flags)
		{
			// Process Shop Flags
			var excludeItemsFromRandomShops = new List<Item>();

			if ((bool)flags.Shops)
			{
				if ((bool)flags.Treasures)
				{
					excludeItemsFromRandomShops = ForcedItemPlacements.Select(x => x.Item).Concat(IncentiveItems).ToList();
				}

				if (!((bool)flags.RandomWaresIncludesSpecialGear))
				{
					excludeItemsFromRandomShops.AddRange(ItemLists.SpecialGear);

					if (flags.GuaranteedDefenseItem != GuaranteedDefenseItem.None && !(flags.ItemMagicMode == ItemMagicMode.None))
						excludeItemsFromRandomShops.Add(Item.PowerRod);

					if (flags.GuaranteedPowerItem != GuaranteedPowerItem.None && !(flags.ItemMagicMode == ItemMagicMode.None))
						excludeItemsFromRandomShops.Add(Item.PowerGauntlets);
				}

				if ((bool)flags.NoMasamune)
				{
					excludeItemsFromRandomShops.Add(Item.Masamune);
				}

				if ((bool)flags.NoXcalber)
				{
					excludeItemsFromRandomShops.Add(Item.Xcalber);
				}
			}

			ExcludedItemsFromShops = excludeItemsFromRandomShops;
		}
		private void ProcessTreasurePool(FF1Rom rom, MT19337 rng, Flags flags)
		{
			TreasurePool = typeof(ItemLocations)
				.GetFields()
				.Where(t => t.FieldType.BaseType == typeof(TreasureChest))
				.Select(t => (TreasureChest)t.GetValue(null))
				.Select(t => t.Item)
				.Except(KeyItems)
				.ToList();



			// Process Item Generation Here


			// We need to kick stuff out for shards, extconsumable, xpchests
			// We'll do a full item > quality thingy, but for now a simple list will suffice
			List<(Item, ItemQuality)> itemRanking = new();
			itemRanking.AddRange(Enumerable.Range((int)Item.Tent, 6).Select(i => ((Item)i, ItemQuality.Common)));
			itemRanking.AddRange(Enumerable.Range((int)Item.WoodenNunchucks, 19).Select(i => ((Item)i, ItemQuality.Common)));
			itemRanking.AddRange(Enumerable.Range((int)Item.FlameSword, 14).Select(i => ((Item)i, ItemQuality.Rare)));
			itemRanking.AddRange(Enumerable.Range((int)Item.CatClaw, 3).Select(i => ((Item)i, ItemQuality.Rare)));
			itemRanking.AddRange(new List<(Item, ItemQuality)> { (Item.Vorpal, ItemQuality.Legendary), (Item.Xcalber, ItemQuality.Legendary), (Item.Katana, ItemQuality.Legendary) });
			itemRanking.Add((Item.Masamune, ItemQuality.Legendary));
			itemRanking.AddRange(ItemLists.CommonArmorTier.Select(i => ((Item)i, ItemQuality.Common)));
			itemRanking.AddRange(ItemLists.RareArmorTier.Select(i => ((Item)i, ItemQuality.Rare)));
			itemRanking.AddRange(ItemLists.LegendaryArmorTier.Select(i => ((Item)i, ItemQuality.Legendary)));
			itemRanking.AddRange(Enumerable.Range((int)Item.Gold10, 31).Select(i => ((Item)i, ItemQuality.Common)));
			itemRanking.AddRange(Enumerable.Range((int)Item.Gold1020, 22).Select(i => ((Item)i, ItemQuality.Rare)));
			itemRanking.AddRange(Enumerable.Range((int)Item.Gold10000, 15).Select(i => ((Item)i, ItemQuality.Legendary)));

			// So we have all the available treasure chests
			// Get our difference
			var reserved = IncentiveItems.Except(KeyItems).Count() + KeyItemsToPlace.Count();
			var


			// 2 chests + 1 chest incentive + 1 npc incentive, 2 key items
			// treasure give us 3 items, minus KI = 2 items
			// i incentive npc only, but the 2 ki
			// so 1 - 2 => -1
			var incentiveDiff = IncentiveLocations.Count() - IncentiveItems.Count();



		}

		private List<IRewardSource> SelectIncentivizedChests(IIncentiveFlags flags, MT19337 rng)
		{
			List<IRewardSource> incentiveLocationPool = new();

			if (flags.IncentivizeVolcano ?? false)
			{
				if (flags.VolcanoIncentivePlacementType == IncentivePlacementTypeVolcano.RandomAtLocation)
				{
					incentiveLocationPool.Add(ItemLocations.Volcano.ToList().SpliceRandom(rng));
				}
				else if (flags.VolcanoIncentivePlacementType == IncentivePlacementTypeVolcano.RandomShallow)
				{
					incentiveLocationPool.Add(ItemLocations.VolcanoShallow.ToList().SpliceRandom(rng));
				}
				else if (flags.VolcanoIncentivePlacementType == IncentivePlacementTypeVolcano.RandomDeep)
				{
					incentiveLocationPool.Add(ItemLocations.VolcanoDeep.ToList().SpliceRandom(rng));
				}
				else
				{
					incentiveLocationPool.Add(ItemLocations.VolcanoMajor);
				}
			}
			if (flags.IncentivizeEarth ?? false)
			{
				if (flags.EarthIncentivePlacementType == IncentivePlacementTypeEarth.RandomAtLocation)
				{
					incentiveLocationPool.Add(ItemLocations.EarthCave.ToList().SpliceRandom(rng));
				}
				else if (flags.EarthIncentivePlacementType == IncentivePlacementTypeEarth.RandomPostRod)
				{
					incentiveLocationPool.Add(ItemLocations.EarthCaveFloor4.ToList().SpliceRandom(rng));
				}
				else if (flags.EarthIncentivePlacementType == IncentivePlacementTypeEarth.RandomPreRod)
				{
					incentiveLocationPool.Add(ItemLocations.EarthCavePreRod.ToList().SpliceRandom(rng));
				}
				else
				{
					incentiveLocationPool.Add(ItemLocations.EarthCaveMajor);
				}
			}
			if (flags.IncentivizeMarsh ?? false)
			{
				if (flags.MarshIncentivePlacementType == IncentivePlacementType.RandomAtLocation)
				{
					incentiveLocationPool.Add(ItemLocations.MarshCaveUnlocked.ToList().SpliceRandom(rng));
				}
				else
				{
					incentiveLocationPool.Add(ItemLocations.MarshCaveMajor);
				}
			}
			if (flags.IncentivizeMarshKeyLocked ?? false)
			{
				if (flags.MarshLockedIncentivePlacementType == IncentivePlacementType.RandomAtLocation)
				{
					incentiveLocationPool.Add(ItemLocations.MarshCaveLocked.ToList().SpliceRandom(rng));
				}
				else
				{
					incentiveLocationPool.Add(ItemLocations.MarshCave13);
				}
			}
			if (flags.IncentivizeSkyPalace ?? false)
			{
				if (flags.SkyPalaceIncentivePlacementType == IncentivePlacementTypeSky.RandomAtLocation)
				{
					incentiveLocationPool.Add(ItemLocations.SkyPalace.Concat(ItemLocations.MirageTower).ToList().SpliceRandom(rng));
				}
				else if (flags.SkyPalaceIncentivePlacementType == IncentivePlacementTypeSky.RandomSky)
				{
					incentiveLocationPool.Add(ItemLocations.SkyPalace.ToList().SpliceRandom(rng));
				}
				else if (flags.SkyPalaceIncentivePlacementType == IncentivePlacementTypeSky.RandomMirage)
				{
					incentiveLocationPool.Add(ItemLocations.MirageTower.ToList().SpliceRandom(rng));
				}
				else
				{
					incentiveLocationPool.Add(ItemLocations.SkyPalaceMajor);
				}
			}
			if (flags.IncentivizeSeaShrine ?? false)
			{
				if (flags.SeaShrineIncentivePlacementType == IncentivePlacementTypeSea.RandomAtLocation)
				{
					incentiveLocationPool.Add(ItemLocations.SeaShrine.ToList().SpliceRandom(rng));
				}
				else if (flags.SeaShrineIncentivePlacementType == IncentivePlacementTypeSea.RandomLocked)
				{
					if ((bool)flags.MermaidPrison)
					{
						incentiveLocationPool.Add(ItemLocations.SeaShrineMermaids.Append(ItemLocations.SeaShrineLocked).ToList().SpliceRandom(rng));
					}
					else
					{
						incentiveLocationPool.Add(ItemLocations.SeaShrineLocked);
					}
				}
				else if (flags.SeaShrineIncentivePlacementType == IncentivePlacementTypeSea.RandomUnlocked)
				{
					if ((bool)flags.MermaidPrison)
					{
						incentiveLocationPool.Add(ItemLocations.SeaShrineUnlockedMinusMermaids.ToList().SpliceRandom(rng));
					}
					else
					{
						incentiveLocationPool.Add(ItemLocations.SeaShrineUnlocked.ToList().SpliceRandom(rng));
					}
				}
				else
				{
					incentiveLocationPool.Add(ItemLocations.SeaShrineMajor);
				}
			}
			if (flags.IncentivizeConeria ?? false)
			{
				if (flags.CorneriaIncentivePlacementType == IncentivePlacementType.RandomAtLocation)
				{
					incentiveLocationPool.Add(ItemLocations.Coneria.ToList().SpliceRandom(rng));
				}
				else
				{
					incentiveLocationPool.Add(ItemLocations.ConeriaMajor);
				}
			}
			if (flags.IncentivizeIceCave ?? false)
			{
				if (flags.IceCaveIncentivePlacementType == IncentivePlacementType.RandomAtLocation)
				{
					incentiveLocationPool.Add(ItemLocations.IceCave.ToList().SpliceRandom(rng));
				}
				else
				{
					incentiveLocationPool.Add(ItemLocations.IceCaveMajor);
				}
			}

			if (flags.IncentivizeOrdeals ?? false)
			{
				if (flags.OrdealsIncentivePlacementType == IncentivePlacementType.RandomAtLocation)
				{
					incentiveLocationPool.Add(ItemLocations.Ordeals.ToList().SpliceRandom(rng));
				}
				else
				{
					incentiveLocationPool.Add(ItemLocations.OrdealsMajor);
				}
			}
			if (flags.IncentivizeTitansTrove ?? false)
			{
				if (flags.TitansIncentivePlacementType == IncentivePlacementType.RandomAtLocation)
				{
					incentiveLocationPool.Add(ItemLocations.TitansTunnel.ToList().SpliceRandom(rng));
				}
				else
				{
					incentiveLocationPool.Add(ItemLocations.TitansTunnel1);
				}
			}
			if (flags.IncentivizeCardia ?? false)
			{
				if (flags.CardiaIncentivePlacementType == IncentivePlacementType.RandomAtLocation)
				{
					incentiveLocationPool.Add(ItemLocations.Cardia.ToList().SpliceRandom(rng));
				}
				else
				{
					incentiveLocationPool.Add(ItemLocations.Cardia4);
				}
			}

			return incentiveLocationPool;
		}
		private List<Item> KeyItems = new() { Item.Lute, Item.Bridge, Item.Ship, Item.Crown, Item.Crystal, Item.Herb, Item.Crystal, Item.Key, Item.Tnt, Item.Canal, Item.Ruby, Item.Rod, Item.Canoe, Item.Floater, Item.Tail, Item.Bottle, Item.Oxyale, Item.Cube, Item.Slab, Item.Chime };
		public IEnumerable<IRewardSource> ForcedItemPlacements { get; private set; }
		public IEnumerable<IRewardSource> AllValidItemLocations { get; private set; }
		public IEnumerable<IRewardSource> AllValidPreBlackOrbItemLocations { get; private set; }
		public IEnumerable<IRewardSource> AllValidPreBlackOrbItemLocationsPlusForced { get; private set; }
		public IEnumerable<IRewardSource> IncentiveLocations { get; private set; }
		public IEnumerable<Item> IncentiveItems { get; private set; }
		public IEnumerable<Item> UnincentiveKeyItems { get => KeyItems.Except(IncentiveItems).Except(FreeItems).Except(RemovedItems); }
		public IEnumerable<Item> KeyItemsToPlace { get; private set; }
		public IEnumerable<Item> RemovedItems { get; private set; }
		public IEnumerable<Item> FreeItems { get; private set; }
		public IEnumerable<Item> TreasurePool { get; private set; }
		public IEnumerable<Item> Shards { get; private set; }
		public IEnumerable<Item> ExcludedItemsFromShops { get; private set; }
	}
}
