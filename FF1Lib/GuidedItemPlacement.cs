namespace FF1Lib
{
	public class GuidedItemPlacement : ItemPlacement
	{
		protected override ItemPlacementResult DoSanePlacement(MT19337 rng, ItemPlacementContext ctx)
		{
			_sanityCounter = 0;
			var incentiveLocationPool = _incentivesData.IncentiveLocations.ToList();
			var preBlackOrbLocationPool = _incentivesData.AllValidPreBlackOrbItemLocations.ToList();
			var preBlackOrbUnincentivizedLocationPool = preBlackOrbLocationPool.Where(x => !incentiveLocationPool.Any(y => y.Address == x.Address)).ToList();
			if ((bool)_flags.LooseExcludePlacedDungeons)
				preBlackOrbUnincentivizedLocationPool = IncentivizedDungeons(preBlackOrbUnincentivizedLocationPool);

			Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullLocationRequirements = _overworldMap.FullLocationRequirements;
			//Dictionary<MapLocation, OverworldTeleportIndex> overridenOverworld = _overworldMap.OverriddenOverworldLocations;

			List<IRewardSource> placedItems = null;
			List<Item> treasurePool = null;
			var itemShopItem = Item.Bottle;

			bool placementFailed;
			do
			{
				placementFailed = false;

				//That number(7.0) is a "tuned" parameter. I divided the number of chests by the number of npcs. Took half of that and looked through some spoiler logs to see if it was too high or too low.
				var balancedPicker = new RewardSourcePicker(0.5, _flags.LooseItemsNpcBalance ? 7.0 : 1.0, _checker);

				_sanityCounter++;
				if (_sanityCounter > 20) throw new InsaneException("Item Placement could not meet incentivization requirements!");
				// 1. (Re)Initialize lists inside of loop
				placedItems = ctx.Forced.ToList();
				var incentives = ctx.Incentivized.ToList();
				var nonincentives = ctx.Unincentivized.ToList();
				var shards = ctx.Shards.ToList();
				var removedItems = ctx.Removed.ToList();
				treasurePool = ctx.AllTreasures.ToList();
				incentives.Shuffle(rng);
				nonincentives.Shuffle(rng);

				while (incentives.Count() > incentiveLocationPool.Count())
				{
					nonincentives.Add(incentives.SpliceRandom(rng));
				}

				if (((bool)_flags.NPCItems) || ((bool)_flags.NPCFetchItems))
				{
					// Identify but don't place caravan item first because among incentive locations it has the smallest set of possible items
					itemShopItem = SelectVendorItem(incentives, nonincentives, treasurePool, incentiveLocationPool, rng);

					// unrestricted items can get placed anywhere in the game. Everything else will only be placed where we can reach at this point.
					List<Item> unrestricted = new List<Item> { Item.Key, Item.Canoe, Item.Floater };

					// We will place these items in this very order so that we can ensure the bridge is early, and we don't need the floater to find the ship.
					List<Item> fixedPlacements;
					if (_flags.Archipelago)
					{
						fixedPlacements = new List<Item> { Item.Bridge, Item.Key, Item.Canoe };
					}
					else
					{
						fixedPlacements = new List<Item> { Item.Key, Item.Bridge, Item.Canoe };
					}

					List<Item> nextPlacements = new List<Item> { Item.Ship, Item.Canal };
					List<Item> lastPlacements = new List<Item> { Item.Floater, Item.Lute, Item.Crown, Item.Crystal, Item.Herb, Item.Tnt, Item.Adamant,
						Item.Slab, Item.Ruby, Item.Rod, Item.Chime, Item.Tail, Item.Cube, Item.Bottle, Item.Oxyale };

					if ((bool)_flags.EarlierRuby)
					{
						nextPlacements.Add(Item.Ruby);
						lastPlacements.Remove(Item.Ruby);
					}
					if ((bool)_flags.IsFloaterRemoved)
					{
						lastPlacements.Remove(Item.Floater);
					}

					if (_flags.DesertOfDeath && !(bool)_flags.IsShipFree)
					{
						nextPlacements.Remove(Item.Ship);
						fixedPlacements.Add(Item.Ship);
						unrestricted.Add(Item.Ship);
					}

					// Different placement priorities for NoOverworld
					if (((IItemShuffleFlags)_flags).NoOverworld)
					{
						List<Item> nodeItems = new() { Item.Floater, Item.Canoe };
						List<Item> corridorItems = new() { Item.Tnt, Item.Ruby };
						List<Item> fetchItems = new() { Item.Crown, Item.Herb, Item.Crystal };

						if ((bool)_flags.Entrances)
						{
							nodeItems.Add(Item.Key);
							corridorItems.Add(Item.Oxyale);
							corridorItems.Add(Item.Chime);

							unrestricted = new List<Item> { };
							fixedPlacements = new List<Item> { nodeItems.SpliceRandom(rng), corridorItems.SpliceRandom(rng) };
							nextPlacements = new List<Item> { nodeItems.SpliceRandom(rng), corridorItems.SpliceRandom(rng) };
							lastPlacements = new List<Item> { Item.Lute, Item.Crown, Item.Crystal, Item.Herb, Item.Adamant,
							Item.Slab, Item.Tail, Item.Cube, Item.Bottle, Item.Rod, Item.Chime, Item.Bridge, Item.Ship, Item.Canal };

							lastPlacements.AddRange(nodeItems);
							lastPlacements.AddRange(corridorItems);
						}
						else
						{
							unrestricted = new List<Item> { Item.Key };
							fixedPlacements = new List<Item> { Item.Key, fetchItems.SpliceRandom(rng), fetchItems.SpliceRandom(rng), corridorItems.SpliceRandom(rng) };
							nextPlacements = new List<Item> { nodeItems.SpliceRandom(rng), corridorItems.SpliceRandom(rng) };
							lastPlacements = new List<Item> { Item.Lute, Item.Adamant,
						Item.Slab, Item.Tail, Item.Cube, Item.Bottle, Item.Oxyale, Item.Rod, Item.Chime, Item.Bridge, Item.Ship, Item.Canal };

							lastPlacements.AddRange(nodeItems);
							lastPlacements.AddRange(corridorItems);
							lastPlacements.AddRange(fetchItems);
						}
					}

					nextPlacements.Shuffle(rng);
					lastPlacements.Shuffle(rng);
					var allPlacements = fixedPlacements.Concat(nextPlacements).Concat(lastPlacements).Except(removedItems);

					foreach (var item in allPlacements)
					{
						if (placedItems.Any(x => x.Item == item))
							continue;

						if (item == itemShopItem)
						{
							placedItems.Add(new ItemShopSlot(_caravanItemLocation, itemShopItem));
							itemShopItem = Item.None;
							continue;
						}

						var (_, mapLocations, requirements) = _checker.CheckSanity(placedItems, fullLocationRequirements, _flags);

						var isIncentive = incentives.Contains(item);
						var locationPool = isIncentive ? incentiveLocationPool : preBlackOrbUnincentivizedLocationPool;
						var itemPool = isIncentive ? incentives : nonincentives;

						System.Diagnostics.Debug.Assert(itemPool.Contains(item));

						// Can we find a home for this item at this point in the exploration?
						var rewardSources = locationPool.Where(x => !placedItems.Any(y => y.Address == x.Address)
							&& x.Address != ItemLocations.CaravanItemShop1.Address
							&& (unrestricted.Contains(item) || _checker.IsRewardSourceAccessible(x, requirements, mapLocations)))
							.ToList();

						// If we can great, if not, we'll revisit after we get through everything.
						if (rewardSources.Any())
						{
							itemPool.Remove(item);
							var rewardSource = balancedPicker.Pick(rewardSources, _flags.LooseItemsForwardPlacement && !isIncentive, _flags.LooseItemsSpreadPlacement, isIncentive, rng);
							placedItems.Add(NewItemPlacement(rewardSource, item));
						}
					}
				}

				// This is a junk item that didn't get placed in the above loop.
				if (!placedItems.Any(item => item.Address == _caravanItemLocation.Address))
				{
					placedItems.Add(new ItemShopSlot(_caravanItemLocation, itemShopItem));
				}

				// 5. Then place all incentive locations that don't have special logic
				var incentiveLocations = incentiveLocationPool.Where(x => !placedItems.Any(y => y.Address == x.Address) && x.Address != ItemLocations.CaravanItemShop1.Address).ToList();
				incentiveLocations.Shuffle(rng);
				foreach (var incentiveLocation in incentiveLocations)
				{
					if (!incentives.Any()) break;
					placedItems.Add(NewItemPlacement(incentiveLocation, incentives.SpliceRandom(rng)));
				}

				// 6. Then place remanining incentive items, unincentivized quest items, and shards in any other chest before ToFR
				var leftoverItems = incentives.Concat(nonincentives).ToList();
				leftoverItems.Shuffle(rng);

				var (_, accessibleMapLocations, fulfilledRequirements) = _checker.CheckSanity(placedItems, fullLocationRequirements, _flags);

				IEnumerable<IRewardSource> looseLocationPool;
				if ((bool)_flags.LooseExcludePlacedDungeons)
				{
					looseLocationPool = preBlackOrbUnincentivizedLocationPool;
				}
				else
				{
					looseLocationPool = preBlackOrbLocationPool;
				}
				var leftoverItemLocations = looseLocationPool.Where(x => !placedItems.Any(y => y.Address == x.Address)
										   && _checker.IsRewardSourceAccessible(x, fulfilledRequirements, accessibleMapLocations)).ToList();


				// Place leftover items before placing shards, because if LooseExcludePlacedDungeons is set,
				// and there are lots of incentive locations, leftoverItemLocations pool may be small.
				foreach (var leftoverItem in leftoverItems)
				{
					if (!leftoverItemLocations.Any())
					{
						placementFailed = true;
						break;
					}

					placedItems.Add(NewItemPlacement(leftoverItemLocations.SpliceRandom(rng), leftoverItem));

					//If the placed item is a KI, update the accessible locations
					if (ItemLists.AllQuestItems.Contains(leftoverItem))
					{
						(_, accessibleMapLocations, fulfilledRequirements) = _checker.CheckSanity(placedItems, fullLocationRequirements, _flags);

						leftoverItemLocations = looseLocationPool.Where(x => !placedItems.Any(y => y.Address == x.Address)
						   && _checker.IsRewardSourceAccessible(x, fulfilledRequirements, accessibleMapLocations)).ToList();
					}
				}

				if (!placementFailed && shards.Count > 0)
				{
					// Place shards.  These are not affected by the LooseExcludePlacedDungeons flag.
					leftoverItemLocations = preBlackOrbLocationPool.Where(x => !placedItems.Any(y => y.Address == x.Address)
											  && _checker.IsRewardSourceAccessible(x, fulfilledRequirements, accessibleMapLocations)).ToList();

					foreach (var shard in shards)
					{
						if (!leftoverItemLocations.Any())
						{
							placementFailed = true;
							break;
						}

						placedItems.Add(NewItemPlacement(leftoverItemLocations.SpliceRandom(rng), shard));
					}
				}

				// 7. Check sanity and loop if needed
			} while (placementFailed || !_checker.CheckSanity(placedItems, fullLocationRequirements, _flags).Complete);

			return new ItemPlacementResult { PlacedItems = placedItems, RemainingTreasures = treasurePool };
		}
	}
}
