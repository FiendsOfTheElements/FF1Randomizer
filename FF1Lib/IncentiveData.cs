using System;
using System.Collections.Generic;
using System.Linq;

namespace FF1Lib
{
	public class IncentiveData
	{
		public IncentiveData(IIncentiveFlags flags,
							Dictionary<MapLocation, List<MapChange>> mapLocationRequirements)
		{
			var forcedItemPlacements = ItemLocations.AllOtherItemLocations.ToList();
			if (!flags.NPCItems) forcedItemPlacements.AddRange(ItemLocations.AllNonTreasureItemLocations);
			if (!flags.Treasures) forcedItemPlacements.AddRange(ItemLocations.AllTreasures);
			var incentivePool = new List<Item>();
			if (flags.IncentivizeBridge)
			{
				incentivePool.Add(Item.Bridge);
			}
			if (flags.IncentivizeShip)
			{
				incentivePool.Add(Item.Ship);
			}
			if (flags.IncentivizeCanal)
			{
				incentivePool.Add(Item.Canal);
			}
			if (flags.IncentivizeLute)
			{
				incentivePool.Add(Item.Lute);
			}
			if (flags.IncentivizeCrown)
			{
				incentivePool.Add(Item.Crown);
			}
			if (flags.IncentivizeCrystal)
			{
				incentivePool.Add(Item.Crystal);
			}
			if (flags.IncentivizeHerb)
			{
				incentivePool.Add(Item.Herb);
			}
			if (flags.IncentivizeKey)
			{
				incentivePool.Add(Item.Key);
			}
			if (flags.IncentivizeTnt)
			{
				incentivePool.Add(Item.Tnt);
			}
			if (flags.IncentivizeAdamant)
			{
				incentivePool.Add(Item.Adamant);
			}
			if (flags.IncentivizeSlab)
			{
				incentivePool.Add(Item.Slab);
			}
			if (flags.IncentivizeRuby)
			{
				incentivePool.Add(Item.Ruby);
			}
			if (flags.IncentivizeRod)
			{
				incentivePool.Add(Item.Rod);
			}
			if (flags.IncentivizeFloater)
			{
				incentivePool.Add(Item.Floater);
			}
			if (flags.IncentivizeChime)
			{
				incentivePool.Add(Item.Chime);
			}
			if (flags.IncentivizeTail)
			{
				incentivePool.Add(Item.Tail);
			}
			if (flags.IncentivizeCube)
			{
				incentivePool.Add(Item.Cube);
			}
			if (flags.IncentivizeBottle)
			{
				incentivePool.Add(Item.Bottle);
			}
			if (flags.IncentivizeOxyale)
			{
				incentivePool.Add(Item.Oxyale);
			}
			if (flags.IncentivizeCanoe)
			{
				incentivePool.Add(Item.Canoe);
			}

			if (flags.IncentivizeXcalber)
			{
				incentivePool.Add(Item.Xcalber);
			}
			if (flags.IncentivizeMasamune)
			{
				incentivePool.Add(Item.Masamune);
			}
			if (flags.IncentivizeRibbon)
			{
				incentivePool.Add(Item.Ribbon);
			}
			if (flags.IncentivizeRibbon2)
			{
				incentivePool.Add(Item.Ribbon);
			}
			if (flags.IncentivizePowerGauntlet)
			{
				incentivePool.Add(Item.PowerGauntlets);
			}
			if (flags.IncentivizeWhiteShirt)
			{
				incentivePool.Add(Item.WhiteShirt);
			}
			if (flags.IncentivizeBlackShirt)
			{
				incentivePool.Add(Item.BlackShirt);
			}
			if (flags.IncentivizeOpal)
			{
				incentivePool.Add(Item.Opal);
			}
			if (flags.Incentivize65K)
			{
				incentivePool.Add(Item.Gold65000);
			}
			if (flags.IncentivizeBad)
			{
				incentivePool.Add(Item.Cloth);
			}

			var incentiveLocationPool = new List<IRewardSource>();
			if (flags.IncentivizeKingConeria)
			{
				incentiveLocationPool.Add(ItemLocations.KingConeria);
			}
			if (flags.IncentivizePrincess)
			{
				incentiveLocationPool.Add(ItemLocations.Princess);
			}
			if (flags.IncentivizeMatoya)
			{
				incentiveLocationPool.Add(ItemLocations.Matoya);
			}
			if (flags.IncentivizeBikke)
			{
				incentiveLocationPool.Add(ItemLocations.Bikke);
			}
			if (flags.IncentivizeElfPrince)
			{
				incentiveLocationPool.Add(ItemLocations.ElfPrince);
			}
			if (flags.IncentivizeAstos)
			{
				incentiveLocationPool.Add(ItemLocations.Astos);
			}
			if (flags.IncentivizeNerrick)
			{
				incentiveLocationPool.Add(ItemLocations.Nerrick);
			}
			if (flags.IncentivizeSmith)
			{
				incentiveLocationPool.Add(ItemLocations.Smith);
			}
			if (flags.IncentivizeSarda)
			{
				incentiveLocationPool.Add(ItemLocations.Sarda);
			}
			if (flags.IncentivizeCanoeSage)
			{
				incentiveLocationPool.Add(ItemLocations.CanoeSage);
			}
			if (flags.IncentivizeCubeBot)
			{
				incentiveLocationPool.Add(ItemLocations.CubeBot);
			}
			if (flags.IncentivizeFairy)
			{
				incentiveLocationPool.Add(ItemLocations.Fairy);
			}
			if (flags.IncentivizeLefein)
			{
				incentiveLocationPool.Add(ItemLocations.Lefein);
			}
			if (flags.IncentivizeVolcano)
			{
				incentiveLocationPool.Add(ItemLocations.VolcanoMajor);
			}
			if (flags.IncentivizeEarth)
			{
				incentiveLocationPool.Add(ItemLocations.EarthCaveMajor);
			}
			if (flags.IncentivizeMarsh)
			{
				incentiveLocationPool.Add(ItemLocations.MarshCaveMajor);
			}
			if (flags.IncentivizeSeaShrine)
			{
				incentiveLocationPool.Add(ItemLocations.SeaShrineLocked);
			}
			if (flags.IncentivizeConeria)
			{
				incentiveLocationPool.Add(ItemLocations.ConeriaMajor);
			}
			if (flags.IncentivizeIceCave)
			{
				incentiveLocationPool.Add(ItemLocations.IceCaveMajor);
			}
			if (flags.IncentivizeOrdeals)
			{
				incentiveLocationPool.Add(ItemLocations.OrdealsMajor);
			}
			var itemLocationPool =
				ItemLocations.AllTreasures.Concat(ItemLocations.AllNPCItemLocations)
						  .Where(x => !x.IsUnused && !forcedItemPlacements.Any(y => y.Address == x.Address))
						  .ToList();
			if (flags.EarlyOrdeals)
			{
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
			if (flags.EarlySage)
			{
				itemLocationPool =
						itemLocationPool
							.Select(x => x.Address == ItemLocations.CanoeSage.Address
									? new MapObject(ObjectId.CanoeSage, MapLocation.CresentLake, x.Item)
									: x).ToList();
				incentiveLocationPool =
						incentiveLocationPool
							.Select(x => x.Address == ItemLocations.CanoeSage.Address
									? new MapObject(ObjectId.CanoeSage, MapLocation.CresentLake, x.Item)
									: x).ToList();
			}
			if (flags.EarlySarda)
			{
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
			
			var allMapLocations = Enum.GetValues(typeof(MapLocation))
									  .Cast<MapLocation>().ToList();
			var startingMapLocations = allMapLocations.Where(x => mapLocationRequirements[x].Any(y => y == MapChange.None));
			var bridgeLocations =
				ItemLocations.AllQuestItemLocations
					.Where(x => x.AccessRequirement == AccessRequirement.None &&
								startingMapLocations.Contains(x.MapLocation)).ToList();
			if (incentivePool.Remove(Item.Bridge))
			{
				foreach (var incentiveBridgeLocation in incentiveLocationPool.Where(x => bridgeLocations.Any(y => y.Address == x.Address)).ToList())
				{
					bridgeLocations.Add(incentiveBridgeLocation);
					bridgeLocations.Add(incentiveBridgeLocation);
					bridgeLocations.Add(incentiveBridgeLocation);
				}
			}
							 
			var validShipMapLocations =
				allMapLocations.Where(x => mapLocationRequirements[x].Any(y => MapChange.Bridge.HasFlag(y)));
			var shipLocations =
				ItemLocations.AllQuestItemLocations
					.Where(x => AccessRequirement.Crystal.HasFlag(x.AccessRequirement) &&
								validShipMapLocations.Contains(x.MapLocation)).ToList();
			if (incentivePool.Remove(Item.Ship))
			{
				foreach (var incentiveShipLocation in incentiveLocationPool.Where(x => shipLocations.Any(y => y.Address == x.Address)).ToList())
				{
					shipLocations.Add(incentiveShipLocation);
					shipLocations.Add(incentiveShipLocation);
					shipLocations.Add(incentiveShipLocation);
					shipLocations.Add(incentiveShipLocation);
				}
			}
			
			
			foreach(var item in forcedItemPlacements.Select(x => x.Item))
			{
				incentivePool.Remove(item);
			}
			ForcedItemPlacements = forcedItemPlacements.ToList();
			IncentiveItems = incentivePool.ToList();
			BridgeLocations = bridgeLocations
							 .Where(x => !forcedItemPlacements.Any(y => y.Address == x.Address))
							 .ToList();
			ShipLocations = shipLocations
							 .Where(x => !forcedItemPlacements.Any(y => y.Address == x.Address))
							 .ToList();
			IncentiveLocations = incentiveLocationPool
							 .Where(x => !forcedItemPlacements.Any(y => y.Address == x.Address))
							 .ToList();
			AllValidItemLocations = itemLocationPool.ToList();
		}

		public IEnumerable<IRewardSource> BridgeLocations { get; }
		public IEnumerable<IRewardSource> ShipLocations { get; }
		public IEnumerable<IRewardSource> ForcedItemPlacements { get; }
		public IEnumerable<IRewardSource> AllValidItemLocations { get; }
		public IEnumerable<IRewardSource> IncentiveLocations { get; }
		public IEnumerable<Item> IncentiveItems { get; }

	}
}
