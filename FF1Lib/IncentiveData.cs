﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using RomUtilities;

namespace FF1Lib
{
	public enum IncentivePlacementType
	{
		[Description("Vanilla")]
		Vanilla,
		[Description("Random")]
		RandomAtLocation
	}

	public enum IncentivePlacementTypeGated
	{
		[Description("Vanilla")]
		Vanilla,
		[Description("Random")]
		RandomAtLocation,
		[Description("Before Gating")]
		RandomNoGating,
		[Description("Behind Gating")]
		RandomBehindGating
	}

	public class IncentiveData
	{
		public OverworldMap OverworldMap { get; private set; }

		ISanityChecker _checker;

		public IncentiveData(MT19337 rng, IIncentiveFlags flags, OverworldMap map, ItemShopSlot shopSlot, ISanityChecker checker)
		{
			OverworldMap = map;
			_checker = checker;

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
				((bool)flags.FreeTail, Item.Tail),
				((bool)flags.IsBridgeFree, Item.Bridge),
				((bool)flags.IsCanalFree, Item.Canal),
				((bool)flags.IsShipFree, Item.Ship),
			};

			List<(bool, IRewardSource)> removedNPCItemsLocations = new()
			{
				((bool)flags.IsCanoeFree, ItemLocations.CanoeSage),
				((bool)flags.FreeLute, ItemLocations.Princess),
				((bool)flags.IsBridgeFree, ItemLocations.KingConeria),
				((bool)flags.IsShipFree, ItemLocations.Bikke),
			};

			List<(bool, IRewardSource)> removedNPCFetchItemsLocations = new()
			{
				((bool)flags.NoXcalber, ItemLocations.Smith),
				((bool)flags.IsCanalFree, ItemLocations.Nerrick),
			};

			Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullLocationRequirements = map.FullLocationRequirements;
			var forcedItemPlacements = ItemLocations.AllOtherItemLocations.ToList();

			if (!(flags.NPCItems ?? false))
			{
				forcedItemPlacements.AddRange(ItemLocations.AllNPCFreeItemLocationsExcludingVendor.Except(removedNPCItemsLocations.Where(x => x.Item1 == true).Select(x => x.Item2).ToList()));
				forcedItemPlacements.Add(shopSlot);
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

			if ((bool)flags.EarlyOrdeals)
			{
				forcedItemPlacements =
					forcedItemPlacements
						.Select(x => ((x as TreasureChest)?.AccessRequirement.HasFlag(AccessRequirement.Crown) ?? false)
								? new TreasureChest(x, x.Item, x.AccessRequirement & ~AccessRequirement.Crown)
								: x).ToList();
				itemLocationPool =
					itemLocationPool
						.Select(x => ((x as TreasureChest)?.AccessRequirement.HasFlag(AccessRequirement.Crown) ?? false)
								? new TreasureChest(x, x.Item, x.AccessRequirement & ~AccessRequirement.Crown)
								: x).ToList();
				incentiveLocationPool =
					incentiveLocationPool
						.Select(x => ((x as TreasureChest)?.AccessRequirement.HasFlag(AccessRequirement.Crown) ?? false)
							? new TreasureChest(x, x.Item, x.AccessRequirement & ~AccessRequirement.Crown)
							: x).ToList();
			}
			if ((bool)flags.EarlyKing)
			{
				forcedItemPlacements =
						forcedItemPlacements
							.Select(x => x.Address == ItemLocations.KingConeria.Address
									? new MapObject(ObjectId.King, MapLocation.ConeriaCastle2, x.Item)
									: x).ToList();
				itemLocationPool =
						itemLocationPool
							.Select(x => x.Address == ItemLocations.KingConeria.Address
									? new MapObject(ObjectId.King, MapLocation.ConeriaCastle2, x.Item)
									: x).ToList();
				incentiveLocationPool =
						incentiveLocationPool
							.Select(x => x.Address == ItemLocations.KingConeria.Address
									? new MapObject(ObjectId.King, MapLocation.ConeriaCastle2, x.Item)
									: x).ToList();
			}
			if ((bool)flags.EarlySage)
			{
				forcedItemPlacements =
						forcedItemPlacements
							.Select(x => x.Address == ItemLocations.CanoeSage.Address
									? new MapObject(ObjectId.CanoeSage, MapLocation.CrescentLake, x.Item)
									: x).ToList();
				itemLocationPool =
						itemLocationPool
							.Select(x => x.Address == ItemLocations.CanoeSage.Address
									? new MapObject(ObjectId.CanoeSage, MapLocation.CrescentLake, x.Item)
									: x).ToList();
				incentiveLocationPool =
						incentiveLocationPool
							.Select(x => x.Address == ItemLocations.CanoeSage.Address
									? new MapObject(ObjectId.CanoeSage, MapLocation.CrescentLake, x.Item)
									: x).ToList();
			}
			if ((bool)flags.EarlySarda)
			{
				forcedItemPlacements =
					forcedItemPlacements
						.Select(x => x.Address == ItemLocations.Sarda.Address
								? new MapObject(ObjectId.Sarda, MapLocation.SardasCave, x.Item)
								: x).ToList();
				itemLocationPool =
					itemLocationPool
						.Select(x => x.Address == ItemLocations.Sarda.Address
								? new MapObject(ObjectId.Sarda, MapLocation.SardasCave, x.Item)
								: x).ToList();
				incentiveLocationPool =
					incentiveLocationPool
						.Select(x => x.Address == ItemLocations.Sarda.Address
								? new MapObject(ObjectId.Sarda, MapLocation.SardasCave, x.Item)
								: x).ToList();
			}

			MapLocation elfDoctorLocation = map.ObjectiveNPCs[ObjectId.ElfDoc];
			if (elfDoctorLocation != MapLocation.ElflandCastle)
			{
				forcedItemPlacements =
					forcedItemPlacements
						.Select(x => x.Address == ItemLocations.ElfPrince.Address
								? new MapObject(ObjectId.ElfPrince, MapLocation.ElflandCastle, x.Item, AccessRequirement.Herb, ObjectId.ElfDoc, requiredSecondLocation: elfDoctorLocation)
								: x).ToList();
				itemLocationPool =
					itemLocationPool
						.Select(x => x.Address == ItemLocations.ElfPrince.Address
								? new MapObject(ObjectId.ElfPrince, MapLocation.ElflandCastle, x.Item, AccessRequirement.Herb, ObjectId.ElfDoc, requiredSecondLocation: elfDoctorLocation)
								: x).ToList();
				incentiveLocationPool =
					incentiveLocationPool
						.Select(x => x.Address == ItemLocations.ElfPrince.Address
								? new MapObject(ObjectId.ElfPrince, MapLocation.ElflandCastle, x.Item, AccessRequirement.Herb, ObjectId.ElfDoc, requiredSecondLocation: elfDoctorLocation)
								: x).ToList();
			}

			MapLocation unneLocation = map.ObjectiveNPCs[ObjectId.Unne];
			if (unneLocation != MapLocation.Melmond)
			{
				forcedItemPlacements =
					forcedItemPlacements
						.Select(x => x.Address == ItemLocations.Lefein.Address
								? new MapObject(ObjectId.Lefein, MapLocation.Lefein, x.Item, AccessRequirement.Slab, ObjectId.Unne, requiredSecondLocation: unneLocation)
								: x).ToList();
				itemLocationPool =
					itemLocationPool
						.Select(x => x.Address == ItemLocations.Lefein.Address
								? new MapObject(ObjectId.Lefein, MapLocation.Lefein, x.Item, AccessRequirement.Slab, ObjectId.Unne, requiredSecondLocation: unneLocation)
								: x).ToList();
				incentiveLocationPool =
					incentiveLocationPool
						.Select(x => x.Address == ItemLocations.Lefein.Address
								? new MapObject(ObjectId.Lefein, MapLocation.Lefein, x.Item, AccessRequirement.Slab, ObjectId.Unne, requiredSecondLocation: unneLocation)
								: x).ToList();
			}

			foreach (var item in forcedItemPlacements.Select(x => x.Item))
			{
				incentivePool.Remove(item);
			}

			foreach (var item in removedItems)
			{
				incentivePool.Remove(item);
			}

			var nonEndgameMapLocations = _checker.AccessibleMapLocations(~AccessRequirement.BlackOrb, MapChange.All, fullLocationRequirements);

			ForcedItemPlacements = forcedItemPlacements.ToList();
			IncentiveItems = incentivePool.ToList();

			RemovedItems = removedItems.ToList();

			IncentiveLocations = incentiveLocationPool
							 .Where(x => !forcedItemPlacements.Any(y => y.Address == x.Address))
							 .ToList();

			AllValidItemLocations = itemLocationPool.ToList();
			AllValidPreBlackOrbItemLocations = AllValidItemLocations
							 .Where(x => nonEndgameMapLocations.Contains(x.MapLocation) && nonEndgameMapLocations.Contains((x as MapObject)?.SecondLocation ?? MapLocation.StartingLocation))
							 .ToList();
		}

		private List<IRewardSource> SelectIncentivizedChests(IIncentiveFlags flags, MT19337 rng)
		{
			List<IRewardSource> incentiveLocationPool = new();

			if (flags.IncentivizeVolcano ?? false)
			{
				if (flags.VolcanoIncentivePlacementType == IncentivePlacementType.RandomAtLocation)
				{
					incentiveLocationPool.Add(ItemLocations.Volcano.ToList().SpliceRandom(rng));
				}
				else
				{
					incentiveLocationPool.Add(ItemLocations.VolcanoMajor);
				}
			}
			if (flags.IncentivizeEarth ?? false)
			{
				if (flags.EarthIncentivePlacementType == IncentivePlacementTypeGated.RandomAtLocation)
				{
					incentiveLocationPool.Add(ItemLocations.EarthCave.ToList().SpliceRandom(rng));
				}
				else if (flags.EarthIncentivePlacementType == IncentivePlacementTypeGated.RandomBehindGating)
				{
					incentiveLocationPool.Add(ItemLocations.EarthCaveFloor4.ToList().SpliceRandom(rng));
				}
				else if (flags.EarthIncentivePlacementType == IncentivePlacementTypeGated.RandomNoGating)
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
				if (flags.SkyPalaceIncentivePlacementType == IncentivePlacementTypeGated.RandomAtLocation)
				{
					incentiveLocationPool.Add(ItemLocations.SkyPalace.Concat(ItemLocations.MirageTower).ToList().SpliceRandom(rng));
				}
				else if (flags.SkyPalaceIncentivePlacementType == IncentivePlacementTypeGated.RandomBehindGating)
				{
					incentiveLocationPool.Add(ItemLocations.SkyPalace.ToList().SpliceRandom(rng));
				}
				else if (flags.SkyPalaceIncentivePlacementType == IncentivePlacementTypeGated.RandomNoGating)
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
				if (flags.SeaShrineIncentivePlacementType == IncentivePlacementTypeGated.RandomAtLocation)
				{
					incentiveLocationPool.Add(ItemLocations.SeaShrine.ToList().SpliceRandom(rng));
				}
				else if (flags.SeaShrineIncentivePlacementType == IncentivePlacementTypeGated.RandomBehindGating)
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
				else if (flags.SeaShrineIncentivePlacementType == IncentivePlacementTypeGated.RandomNoGating)
				{
					incentiveLocationPool.Add(ItemLocations.SeaShrineUnlocked.ToList().SpliceRandom(rng));
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
		public IEnumerable<IRewardSource> IncentiveLocations { get; }
		public IEnumerable<Item> IncentiveItems { get; }
		public IEnumerable<Item> RemovedItems { get; }
	}
}
