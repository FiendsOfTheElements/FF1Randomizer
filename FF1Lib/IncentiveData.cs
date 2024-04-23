using System.ComponentModel;

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
		public IncentiveData(MT19337 rng, Flags flags)
		{
			ShopSlot = ItemLocations.CaravanItemShop1;

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

			var forcedItemPlacements = ItemLocations.AllOtherItemLocations.ToList();

			if (!(flags.NPCItems ?? false))
			{
				forcedItemPlacements.AddRange(ItemLocations.AllNPCFreeItemLocationsExcludingVendor.Except(removedNPCItemsLocations.Where(x => x.Item1 == true).Select(x => x.Item2).ToList()));
				forcedItemPlacements.Add(ShopSlot);
			}

			if (!(flags.NPCFetchItems ?? false))
			{
				forcedItemPlacements.AddRange(ItemLocations.AllNPCFetchItemLocations.Except(removedNPCFetchItemsLocations.Where(x => x.Item1 == true).Select(x => x.Item2).ToList()));
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

			List<Item> removedItems = removedItemsFlags.Where(x => x.Item1 == true).Select(x => x.Item2).ToList();

			List<Item> incentivePool = incentivizedItemsFlags.Where(x => x.Item1 == true).Select(x => x.Item2).ToList();

			List<IRewardSource> incentiveLocationPool = incentivizedNpcsFlags.Where(x => x.Item1 == true).Select(x => x.Item2).ToList();

			incentiveLocationPool.AddRange(SelectIncentivizedChests(flags, rng));

			var itemLocationPool =
				ItemLocations.AllTreasures.Concat(ItemLocations.AllNPCItemLocations)
						  .Where(x => !x.IsUnused && !forcedItemPlacements.Any(y => y.Address == x.Address))
						  .ToList();

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
			}

			List<MapLocation> endgameMapLocations = new() { MapLocation.TempleOfFiends2, MapLocation.TempleOfFiends3, MapLocation.TempleOfFiendsAir, MapLocation.TempleOfFiendsChaos, MapLocation.TempleOfFiendsEarth, MapLocation.TempleOfFiendsFire, MapLocation.TempleOfFiendsPhantom, MapLocation.TempleOfFiendsWater };
			List<MapLocation> nonEndgameMapLocations = Enum.GetValues<MapLocation>().Except(endgameMapLocations).ToList();

			ForcedItemPlacements = forcedItemPlacements.ToList();
			IncentiveItems = incentivePool.ToList();

			RemovedItems = removedItems.ToList();

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

		public IEnumerable<IRewardSource> ForcedItemPlacements { get; }
		public IEnumerable<IRewardSource> AllValidItemLocations { get; }
		public IEnumerable<IRewardSource> AllValidPreBlackOrbItemLocations { get; }
		public IEnumerable<IRewardSource> AllValidPreBlackOrbItemLocationsPlusForced { get; }
		public IEnumerable<IRewardSource> IncentiveLocations { get; }
		public IEnumerable<Item> IncentiveItems { get; }
		public IEnumerable<Item> RemovedItems { get; }
		public IEnumerable<Item> ExcludedItemsFromShops { get; }
	}
}
