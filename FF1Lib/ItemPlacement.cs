using RomUtilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace FF1Lib
{
	public class InsaneException : Exception
	{
		public InsaneException(string message)
			: base(message) { }
	}

	public static class ItemPlacement
	{
		public static List<IRewardSource> PlaceSaneItems(MT19337 rng,
														IItemPlacementFlags flags,
														IncentiveData incentivesData,
														List<Item> allTreasures,
														ItemShopSlot caravanItemLocation,
														OverworldMap overworldMap)
		{
			Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullLocationRequirements = overworldMap.FullLocationRequirements;
			Dictionary<MapLocation, OverworldTeleportIndex> overridenOverworld = overworldMap.OverriddenOverworldLocations;

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
			var startingPotentialAccess = overworldMap.StartingPotentialAccess;
			var startingMapLocations = AccessibleMapLocations(startingPotentialAccess, MapChange.None, fullLocationRequirements);
			var earlyMapLocations = AccessibleMapLocations(startingPotentialAccess | AccessRequirement.Crystal, MapChange.Bridge, fullLocationRequirements);

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

			// Cache the final unincentivized pool so we can reset it each iteration of the loop.
			IReadOnlyCollection<Item> cachedTreasurePool = new ReadOnlyCollection<Item>(treasurePool.ToList());

			do
			{
				sanityCounter++;
				if (sanityCounter > 500) throw new InsaneException("Sanity Counter exceeds 500 iterations!");
				// 1. (Re)Initialize lists inside of loop
				placedItems = forcedItems.ToList();
				var incentives = incentivePool.ToList();
				var nonincentives = unincentivizedQuestItems.ToList();
				treasurePool = cachedTreasurePool.ToList();
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
						itemShopItem = treasurePool.Concat(nonincentives).Where(x => x > Item.None && x <= Item.Soft && x != Item.Shard).ToList().PickRandom(rng);
						if (!nonincentives.Remove(itemShopItem))
						{
							treasurePool.Remove(itemShopItem);
						}
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

					var startingCanoeAvailable =
						placedItems.Any(x => x.Item == Item.Canoe && startingMapLocations.Contains(x.MapLocation) &&
							startingMapLocations.Contains((x as MapObject)?.SecondLocation ?? MapLocation.StartingLocation));
					var earlyCanoeAvailable =
						placedItems.Any(x => x.Item == Item.Canoe && earlyMapLocations.Contains(x.MapLocation) &&
							earlyMapLocations.Contains((x as MapObject)?.SecondLocation ?? MapLocation.StartingLocation));
					var earlyKeyAvailable =
						placedItems.Any(x => x.Item == Item.Key && earlyMapLocations.Contains(x.MapLocation) &&
							earlyMapLocations.Contains((x as MapObject)?.SecondLocation ?? MapLocation.StartingLocation));

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

					var startingShipAvailable =
						placedItems.Any(x => x.Item == Item.Ship && startingMapLocations.Contains(x.MapLocation) &&
							startingMapLocations.Contains((x as MapObject)?.SecondLocation ?? MapLocation.StartingLocation));

					if (!(startingCanoeAvailable && canoeObsoletesBridge) && !startingShipAvailable)
					{
						var startingKeyAvailable =
							earlyKeyAvailable && placedItems.Any(x => x.Item == Item.Key && startingMapLocations.Contains(x.MapLocation) &&
								startingMapLocations.Contains((x as MapObject)?.SecondLocation ?? MapLocation.StartingLocation));

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
			} while (!CheckSanity(placedItems, fullLocationRequirements, flags));

			if (Debugger.IsAttached)
			{
				Console.WriteLine($"ItemPlacement::PlaceSaneItems required {sanityCounter} iterations.");
				Console.WriteLine("");
				Console.WriteLine("Item     Entrance  ->  Floor  ->  Source                   Requirements");
				Console.WriteLine("------------------------------------------------------------------------------------------");

				var sorted = placedItems.Where(item => item.Item != Item.Shard).ToList();
				sorted.Sort((IRewardSource lhs, IRewardSource rhs) => lhs.Item.ToString().CompareTo(rhs.Item.ToString()));
				sorted.ForEach(item =>
				{
					if (fullLocationRequirements.TryGetValue(item.MapLocation, out var flr))
					{
						var overworldLocation = item.MapLocation.ToString();
						if (overridenOverworld != null && overridenOverworld.TryGetValue(item.MapLocation, out var overriden))
						{
							overworldLocation = overriden.ToString();
						}

						var itemStr = item.Item.ToString().PadRight(9);
						var locStr = $"{overworldLocation} -> {item.MapLocation} -> {item.Name} ".PadRight(50);
						var changes = $"{String.Join(" | ", flr.Item1.Select(mapChange => mapChange.ToString()).ToArray())}";
						var reqs = flr.Item2.ToString().CompareTo("None") == 0 ? "" : $" AND {flr.Item2.ToString()}";
						Console.WriteLine($"{itemStr}{locStr}{changes}{reqs}");
					}
				});
			}

			// 8. Place all remaining unincentivized treasures or incentivized non-quest items that weren't placed
			itemLocationPool = itemLocationPool.Where(x => !x.IsUnused && !placedItems.Any(y => y.Address == x.Address)).ToList();
			foreach (var placedItem in placedItems)
			{
				incentivePool.Remove(placedItem.Item);
			}
			treasurePool.AddRange(incentivePool);

			Debug.Assert(treasurePool.Count() == itemLocationPool.Count());
			treasurePool.Shuffle(rng);
			itemLocationPool.Shuffle(rng);

			if (flags.RandomLoot)
			{
				// We want to leave out anything incentivized (and thus already placed), but
				// add all the other stuff that you can't find in vanilla.
				var randomTreasure = treasurePool.ToList();
				randomTreasure.AddRange(ItemLists.CommonWeaponTier);
				randomTreasure.AddRange(ItemLists.CommonArmorTier);
				randomTreasure.Add(Item.CatClaw);
				randomTreasure.Add(Item.SteelArmor);

				ItemGenerator generator = new ItemGenerator(randomTreasure, flags.WorldWealth);
				treasurePool = treasurePool.Select(treasure => generator.GetItem(rng)).ToList();
			}

			var leftovers = treasurePool.Zip(itemLocationPool, (treasure, location) => NewItemPlacement(location, treasure));
			placedItems.AddRange(leftovers);
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

		public static IEnumerable<MapLocation> AccessibleMapLocations(
										AccessRequirement currentAccess,
										MapChange currentMapChanges,
										Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullLocationRequirements)
		{
			return fullLocationRequirements.Where(x => x.Value.Item1.Any(y => currentMapChanges.HasFlag(y) && currentAccess.HasFlag(x.Value.Item2))).Select(x => x.Key);
		}

		public static bool CheckSanity(List<IRewardSource> treasurePlacements,
										Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullLocationRequirements,
										IVictoryConditionFlags victoryConditions)
		{
			const int maxIterations = 20;
			var currentIteration = 0;
			var currentAccess = AccessRequirement.None;
			if (victoryConditions.ShortToFR)
			{
				currentAccess |= AccessRequirement.Lute;
			}

			var currentMapChanges = MapChange.None;
			if (victoryConditions.FreeBridge)
			{
				currentMapChanges |= MapChange.Bridge;
			}
			if (victoryConditions.FreeAirship)
			{
				currentMapChanges |= MapChange.Airship;
			}
			if (victoryConditions.FreeCanal)
			{
				currentMapChanges |= MapChange.Canal;
			}

			IEnumerable<MapLocation> currentMapLocations()
			{
				return fullLocationRequirements.Where(x => x.Value.Item1.Any(y => currentMapChanges.HasFlag(y) && currentAccess.HasFlag(x.Value.Item2))).Select(x => x.Key);
			}
			IEnumerable<IRewardSource> currentItemLocations()
			{
				var locations = currentMapLocations().ToList();
				return treasurePlacements.Where(x =>
				{
					return locations.Contains(x.MapLocation) && currentAccess.HasFlag(x.AccessRequirement) &&
						locations.Contains((x as MapObject)?.SecondLocation ?? MapLocation.StartingLocation);
				});
			}

			var requiredAccess = AccessRequirement.All;
			var requiredMapChanges = new List<MapChange> { MapChange.All };

			if (victoryConditions.OnlyRequireGameIsBeatable)
			{
				var winTheGameAccess = ItemLocations.ChaosReward.AccessRequirement;
				var winTheGameLocation = ItemLocations.ChaosReward.MapLocation;
				requiredAccess = winTheGameAccess;
				requiredMapChanges = fullLocationRequirements[winTheGameLocation].Item1;
			}

			var accessibleLocationCount = 0;
			while (!currentAccess.HasFlag(requiredAccess) ||
				   !requiredMapChanges.Any(x => currentMapChanges.HasFlag(x)))
			{
				if (currentIteration > maxIterations)
				{
					throw new InvalidOperationException($"Sanity Check hit max iterations: {currentIteration}");
				}

				currentIteration++;
				var accessibleLocations = currentItemLocations();
				var currentItems = accessibleLocations.Select(x => x.Item).ToList();
				if (accessibleLocations.Count() <= accessibleLocationCount)
				{
					return false;
				}
				accessibleLocationCount = accessibleLocations.Count();

				if (!currentAccess.HasFlag(AccessRequirement.Key) &&
					currentItems.Contains(Item.Key))
					currentAccess |= AccessRequirement.Key;
				if (!currentMapChanges.HasFlag(MapChange.Bridge) &&
					currentItems.Contains(Item.Bridge))
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
					(currentMapLocations().Contains(MapLocation.TitansTunnelEast) ||
					currentMapLocations().Contains(MapLocation.TitansTunnelWest)))
				{
					currentMapChanges |= MapChange.TitanFed;
					currentAccess |= AccessRequirement.Ruby;
				}
				if (!currentAccess.HasFlag(AccessRequirement.Rod) &&
					currentItems.Contains(Item.Rod))
					currentAccess |= AccessRequirement.Rod;
				if (!currentAccess.HasFlag(AccessRequirement.Slab) &&
					currentItems.Contains(Item.Slab))
					currentAccess |= AccessRequirement.Slab;
				if (!currentMapChanges.HasFlag(MapChange.Airship) &&
					(currentItems.Contains(Item.Floater)) &&
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


			}

			return true;
		}
	}
}
