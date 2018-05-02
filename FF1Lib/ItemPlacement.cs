using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RomUtilities;

namespace FF1Lib
{
	public static class ItemPlacement
	{
		public static List<IRewardSource> PlaceSaneItems(MT19337 rng,
														IItemPlacementFlags flags,
														IncentiveData incentivesData,
														List<Item> allTreasures,
														ItemShopSlot caravanItemLocation,
														Dictionary<MapLocation, List<MapChange>> mapLocationRequirements)
		{
			long sanityCounter = 0;
			List<IRewardSource> placedItems;

			var canoeObsoletesBridge = flags.MapCanalBridge && flags.MapConeriaDwarves;
			var canoeObsoletesShip = flags.MapCanalBridge ? (flags.MapConeriaDwarves || flags.MapVolcanoIceRiver) : (flags.MapConeriaDwarves && flags.MapVolcanoIceRiver);
			var incentiveLocationPool = incentivesData.IncentiveLocations.ToList();
			var incentivePool = incentivesData.IncentiveItems.Where(x => allTreasures.Contains(x)).ToList();
			var forcedItems = incentivesData.ForcedItemPlacements.ToList();
			var keyLocations = incentivesData.KeyLocations.ToList();
			var canoeLocations = incentivesData.CanoeLocations.ToList();
			var bridgeLocations = incentivesData.BridgeLocations.ToList();
			var shipLocations = incentivesData.ShipLocations.ToList();
			var itemLocationPool = incentivesData.AllValidItemLocations.ToList();
			var startingMapLocations = mapLocationRequirements.Where(x => x.Value.Any(y => y == MapChange.None)).Select(x => x.Key);
			var earlyMapLocations = mapLocationRequirements.Where(x => x.Value.Any(y => MapChange.Bridge.HasFlag(y))).Select(x => x.Key);

			var unincentivizedQuestItems =
				ItemLists.AllQuestItems
					.Where(x => !incentivePool.Contains(x) &&
								allTreasures.Contains(x) &&
								!forcedItems.Any(y => y.Item == x))
								.ToList();

			var treasurePool = allTreasures.ToList();

			foreach (var incentive in incentivePool)
			{
				treasurePool.Remove(incentive);
			}
			foreach (var placement in forcedItems)
			{
				treasurePool.Remove(placement.Item);
			}
			foreach (var questItem in unincentivizedQuestItems)
			{
				treasurePool.Remove(questItem);
			}

			var itemShopItem = Item.Bottle;

			while (treasurePool.Remove(Item.Shard))
			{
				unincentivizedQuestItems.Add(Item.Shard);
			}
			do
			{
				sanityCounter++;
				if (sanityCounter > 10000) throw new InvalidOperationException("Invalid flag set");
				// 1. (Re)Initialize lists inside of loop
				placedItems = forcedItems.ToList();
				var incentives = incentivePool.ToList();
				var nonincentives = unincentivizedQuestItems.ToList();
				incentives.Shuffle(rng);
				nonincentives.Shuffle(rng);

				if (flags.NPCItems)
				{
					// 2. Place caravan item first because among incentive locations it has the smallest set of possible items
					var validShopIncentives = incentives
						.Where(x => x > Item.None && x <= Item.Soft)
						.ToList();
					if (validShopIncentives.Any() && incentiveLocationPool.Any(x => x.Address == ItemLocations.CaravanItemShop1.Address))
					{
						itemShopItem = validShopIncentives.PickRandom(rng);
						incentives.Remove(itemShopItem);
					}
					else
					{
						itemShopItem = treasurePool.Concat(nonincentives).Where(x => x > Item.None && x <= Item.Soft).ToList().PickRandom(rng);
						nonincentives.Remove(itemShopItem);
					}
					placedItems.Add(new ItemShopSlot(caravanItemLocation, itemShopItem));

					// 3. Place key and canoe next because among incentive locations these have the greatest initial impact
					if (incentives.Remove(Item.Key) || nonincentives.Remove(Item.Key))
					{
						placedItems.Add(NewItemPlacement(keyLocations.PickRandom(rng), Item.Key));
					}
					if (incentives.Remove(Item.Canoe) || nonincentives.Remove(Item.Canoe))
					{
						placedItems.Add(NewItemPlacement(canoeLocations.Where(x => !placedItems.Any(y => y.Address == x.Address)).ToList().PickRandom(rng), Item.Canoe));
					}

					var startingCanoeAvailable = placedItems.Any(x => x.Item == Item.Canoe && startingMapLocations.Contains(x.MapLocation));
					var earlyCanoeAvailable = placedItems.Any(x => x.Item == Item.Canoe && earlyMapLocations.Contains(x.MapLocation));
					var earlyKeyAvailable = placedItems.Any(x => x.Item == Item.Key && earlyMapLocations.Contains(x.MapLocation));

					// 4. Place Bridge and Ship next since the valid location lists are so small, unless canoe is available and map edits are applied
					if (!earlyCanoeAvailable || !canoeObsoletesShip)
					{
						var remainingShipLocations =
							shipLocations
								.Where(x => !placedItems.Any(y => y.Address == x.Address) &&
										(earlyKeyAvailable || !x.AccessRequirement.HasFlag(AccessRequirement.Key)))
								.ToList();
						if (!remainingShipLocations.Any()) continue;
						if (incentives.Remove(Item.Ship) && remainingShipLocations.Any(x => incentiveLocationPool.Any(y => y.Address == x.Address)))
						{
							remainingShipLocations =
								remainingShipLocations
									.Where(x => incentiveLocationPool.Any(y => y.Address == x.Address))
									.ToList();
						}
						nonincentives.Remove(Item.Ship);
						placedItems.Add(NewItemPlacement(remainingShipLocations.PickRandom(rng), Item.Ship));
					}

					var startingShipAvailable = placedItems.Any(x => x.Item == Item.Ship && startingMapLocations.Contains(x.MapLocation));

					if (!(startingCanoeAvailable && canoeObsoletesBridge) && !startingShipAvailable)
					{
						var startingKeyAvailable = earlyKeyAvailable && placedItems.Any(x => x.Item == Item.Key && startingMapLocations.Contains(x.MapLocation));

						var remainingBridgeLocations =
							bridgeLocations
								.Where(x => !placedItems.Any(y => y.Address == x.Address) &&
										(startingKeyAvailable || !x.AccessRequirement.HasFlag(AccessRequirement.Key)))
								.ToList();
						if (!remainingBridgeLocations.Any()) continue;
						if (incentives.Remove(Item.Bridge) && remainingBridgeLocations.Any(x => incentiveLocationPool.Any(y => y.Address == x.Address)))
						{
							remainingBridgeLocations =
								remainingBridgeLocations
									.Where(x => incentiveLocationPool.Any(y => y.Address == x.Address))
									.ToList();
						}
						nonincentives.Remove(Item.Bridge);
						placedItems.Add(NewItemPlacement(remainingBridgeLocations.PickRandom(rng), Item.Bridge));
					}
				}

				// 5. Then place all incentive locations that don't have special logic
				var incentiveLocations = incentiveLocationPool.Where(x => !placedItems.Any(y => y.Address == x.Address) && x.Address != ItemLocations.CaravanItemShop1.Address).ToList();
				incentiveLocations.Shuffle(rng);
				foreach (var incentiveLocation in incentiveLocations)
				{
					if (!incentives.Any()) break;
					placedItems.Add(NewItemPlacement(incentiveLocation, incentives.SpliceRandom(rng)));
				}

				// 6. Then place remanining incentive items and unincentivized quest items in any other chest before ToFR
				var leftoverItems = incentives.Concat(nonincentives).ToList();
				leftoverItems.Remove(itemShopItem);
				leftoverItems.Shuffle(rng);
				var leftoverItemLocations =
					itemLocationPool
						 .Where(x => !ItemLocations.ToFR.Any(y => y.Address == x.Address) &&
								!x.IsUnused && !placedItems.Any(y => y.Address == x.Address))
						 .ToList();
				foreach (var leftoverItem in leftoverItems)
				{
					placedItems.Add(NewItemPlacement(leftoverItemLocations.SpliceRandom(rng), leftoverItem));
				}

				// 7. Check sanity and loop if needed
			} while (!CheckSanity(placedItems, mapLocationRequirements, flags));

			// 8. Place all remaining unincentivized treasures or incentivized non-quest items that weren't placed
			var i = 0;
			itemLocationPool =
				itemLocationPool
					 .Where(x => !x.IsUnused && !placedItems.Any(y => y.Address == x.Address))
					.ToList();
			foreach (var placedItem in placedItems)
			{
				incentivePool.Remove(placedItem.Item);
			}
			foreach (var unplacedIncentive in incentivePool)
			{
				treasurePool.Add(unplacedIncentive);
			}
			treasurePool.Shuffle(rng);
			itemLocationPool.Shuffle(rng);
			foreach (var remainingTreasure in itemLocationPool)
			{
				placedItems.Add(NewItemPlacement(remainingTreasure, treasurePool[i]));
				i++;
			}

			//Debug.WriteLine($"Sanity Check Fails: {sanityCounter}");
			return placedItems;
		}

		public static IRewardSource NewItemPlacement(IRewardSource copyFromSource, Item newItem)
		{
			if (copyFromSource is MapObject)
			{
				return new MapObject(copyFromSource as MapObject, newItem);
			}
			else if (copyFromSource is ItemShopSlot)
			{
				return new ItemShopSlot(copyFromSource as ItemShopSlot, newItem);
			}
			else
			{
				return new TreasureChest(copyFromSource, newItem);
			}
		}

		public static bool CheckSanity(List<IRewardSource> treasurePlacements,
										Dictionary<MapLocation, List<MapChange>> mapLocationRequirements,
										IVictoryConditionFlags victoryConditions)
		{
			const int maxIterations = 20;
			var currentIteration = 0;
			var currentAccess = AccessRequirement.None;
			if (victoryConditions.ShardHunt)
			{
				currentAccess |= AccessRequirement.Lute;
			}

			var currentMapChanges = MapChange.None;

			Func<IEnumerable<MapLocation>> currentMapLocations =
				() => mapLocationRequirements
					.Where(x => x.Value
						   .Any(y => currentMapChanges.HasFlag(y))).Select(x => x.Key);
			Func<IEnumerable<IRewardSource>> currentItemLocations =
				() => treasurePlacements
						   .Where(x =>
						   {
							   var locations = currentMapLocations().ToList();
							   return locations.Contains(x.MapLocation) &&
										currentAccess.HasFlag(x.AccessRequirement) &&
											   (!(x is MapObject) || locations.Contains(((MapObject)x).SecondLocation));
						   });

			var accessibleLocationCount = currentItemLocations().Count();
			var requiredAccess = AccessRequirement.All;
			var requiredMapChanges = new List<MapChange> { MapChange.All };

			if (victoryConditions.OnlyRequireGameIsBeatable)
			{
				var winTheGameAccess = ItemLocations.ChaosReward.AccessRequirement;
				var winTheGameLocation = ItemLocations.ChaosReward.MapLocation;
				requiredAccess = winTheGameAccess;
				requiredMapChanges = mapLocationRequirements[winTheGameLocation];
			}

			while (!currentAccess.HasFlag(requiredAccess) ||
				   !requiredMapChanges.Any(x => currentMapChanges.HasFlag(x)))
			{
				if (currentIteration > maxIterations)
				{
					throw new InvalidOperationException($"Sanity Check hit max iterations: {currentIteration}");
				}
				currentIteration++;
				var currentItems = currentItemLocations().Select(x => x.Item).ToList();

				if (!currentAccess.HasFlag(AccessRequirement.Key) &&
					currentItems.Contains(Item.Key))
					currentAccess |= AccessRequirement.Key;
				if (!currentMapChanges.HasFlag(MapChange.Bridge) &&
					currentItems.Contains(Item.Bridge) &&
					currentMapLocations().Contains(MapLocation.BridgeLocation))
					currentMapChanges |= MapChange.Bridge;
				if (!currentAccess.HasFlag(AccessRequirement.Crown) &&
					currentItems.Contains(Item.Crown))
					currentAccess |= AccessRequirement.Crown;
				if (!currentAccess.HasFlag(AccessRequirement.Crystal) &&
					currentItems.Contains(Item.Crystal))
					currentAccess |= AccessRequirement.Crystal;
				if (!currentAccess.HasFlag(AccessRequirement.Herb) &&
					currentItems.Contains(Item.Herb))
					currentAccess |= AccessRequirement.Herb;
				if (!currentMapChanges.HasFlag(MapChange.Canoe) &&
					currentItems.Contains(Item.Canoe))
					currentMapChanges |= MapChange.Canoe;
				if (!currentMapChanges.HasFlag(MapChange.Ship) &&
					currentItems.Contains(Item.Ship))
					currentMapChanges |= MapChange.Ship;
				if (!currentAccess.HasFlag(AccessRequirement.Tnt) &&
					currentItems.Contains(Item.Tnt))
					currentAccess |= AccessRequirement.Tnt;
				if (!currentAccess.HasFlag(AccessRequirement.Adamant) &&
					currentItems.Contains(Item.Adamant))
					currentAccess |= AccessRequirement.Adamant;
				if (!currentMapChanges.HasFlag(MapChange.Canal) &&
					currentItems.Contains(Item.Canal) &&
					currentMapChanges.HasFlag(MapChange.Ship))
					currentMapChanges |= MapChange.Canal;
				if (!currentMapChanges.HasFlag(MapChange.TitanFed) &&
					currentItems.Contains(Item.Ruby) &&
					currentMapLocations().Contains(MapLocation.TitansTunnelEast))
					currentMapChanges |= MapChange.TitanFed;
				if (!currentAccess.HasFlag(AccessRequirement.Rod) &&
					currentItems.Contains(Item.Rod))
					currentAccess |= AccessRequirement.Rod;
				if (!currentAccess.HasFlag(AccessRequirement.Slab) &&
					currentItems.Contains(Item.Slab))
					currentAccess |= AccessRequirement.Slab;
				if (!currentMapChanges.HasFlag(MapChange.Airship) &&
					(currentItems.Contains(Item.Floater)) && // || currentItems.Contains(Item.Airship)) &&
					currentMapLocations().Contains(MapLocation.AirshipLocation))
					currentMapChanges |= MapChange.Airship;

				if (!currentAccess.HasFlag(AccessRequirement.Bottle) &&
					currentItems.Contains(Item.Bottle))
					currentAccess |= AccessRequirement.Bottle;
				if (!currentAccess.HasFlag(AccessRequirement.Oxyale) &&
					currentItems.Contains(Item.Oxyale))
					currentAccess |= AccessRequirement.Oxyale;
				if (!currentMapChanges.HasFlag(MapChange.Chime) &&
					currentItems.Contains(Item.Chime))
					currentMapChanges |= MapChange.Chime;
				if (!currentAccess.HasFlag(AccessRequirement.Cube) &&
					currentItems.Contains(Item.Cube))
					currentAccess |= AccessRequirement.Cube;
				if (!currentAccess.HasFlag(AccessRequirement.EarthOrb) &&
					currentItems.Contains(Item.EarthOrb))
					currentAccess |= AccessRequirement.EarthOrb;
				if (!currentAccess.HasFlag(AccessRequirement.FireOrb) &&
					currentItems.Contains(Item.FireOrb))
					currentAccess |= AccessRequirement.FireOrb;
				if (!currentAccess.HasFlag(AccessRequirement.WaterOrb) &&
					currentItems.Contains(Item.WaterOrb))
					currentAccess |= AccessRequirement.WaterOrb;
				if (!currentAccess.HasFlag(AccessRequirement.AirOrb) &&
					currentItems.Contains(Item.AirOrb))
					currentAccess |= AccessRequirement.AirOrb;
				if (!currentAccess.HasFlag(AccessRequirement.Lute) &&
					currentItems.Contains(Item.Lute))
					currentAccess |= AccessRequirement.Lute;

				var newCount = currentItemLocations().Count();
				if (newCount <= accessibleLocationCount)
				{
					return false;
				}
				accessibleLocationCount = newCount;
			}

			return true;
		}
	}
}
