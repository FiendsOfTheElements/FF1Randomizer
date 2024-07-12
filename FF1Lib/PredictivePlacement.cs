using FF1Lib.Sanity;

namespace FF1Lib
{
	public class PredictivePlacement : ItemPlacement
	{
		static HashSet<Item> MapChangeItems = new HashSet<Item> { Item.Bridge, Item.Canoe, Item.Ship, Item.Canal, Item.Floater, Item.Ruby, Item.Chime };
		static HashSet<Item> FetchQuestItems = new HashSet<Item> { Item.Crystal, Item.Herb, Item.Adamant, Item.Slab, Item.Bottle };
		static HashSet<Item> GatingItems = new HashSet<Item> { Item.Crown, Item.Key, Item.Lute, Item.Rod, Item.Cube, Item.Oxyale, Item.Tnt };
		static HashSet<Item> KeyItems = new HashSet<Item>(MapChangeItems.Concat(FetchQuestItems).Concat(GatingItems));

		//Thy algorithm sllows flexible placement of many items. The average order is set by 3 Factors.
		//A chance modifier(ExceptionWeights)
		//A cycle based restriction(ExceptionMinCycled
		//A restriction based on how many RewardSources are availble

		//The ExceptionWeights directly modify the chance items will be picked when they are ellegible by the algorithm
		//The bridge should be earlier, so it's given a high weight. The floater should be late so it'S given a low weight.
		//This system explicitly allows the bridge to be placed after the floater, it just makes it less likely
		Dictionary<Item, float> SafeExceptionWeights = new Dictionary<Item, float>
		{
			{ Item.Bridge, 2.0f },
			{ Item.Key, 0.25f },
			{ Item.Canoe, 0.75f },
			{ Item.Floater, 0.25f },
			{ Item.Chime, 0.25f },
			{ Item.Cube, 0.25f },
			{ Item.Oxyale, 0.25f },
			{ Item.Lute, 0.25f },
			{ Item.Adamant, 0.75f },
			{ Item.Herb, 0.75f },
			{ Item.Crown, 0.75f },
			{ Item.Crystal, 0.75f },
			{ Item.Bottle, 0.75f },
			{ Item.Slab, 0.75f },
			{ Item.Tnt, 0.75f }
		};

		//Each cycle the algorithm tries to place an item. It randomly(as long as it works) decides each cycle to place either an incentive or a nonincetive item.
		//It may be that it doesn't find an item of the chosen type that continues the seed(e.g. there isn't any incentive that opens up the seed anymore)
		//Then it doesn't place anything in that cycle. If it strictly need the floater to continue, it wastes cycles until the Floater becomes ellegible(cycle 10)
		//So as long as it finds a way forward, the floater will be late, but theoretically it still could be the first item it places.
		Dictionary<Item, int> SafeExceptioMinCycles = new Dictionary<Item, int>
		{
			{ Item.Key, 5 },
			{ Item.Ship, 2 },
			{ Item.Canoe, 3 },
			{ Item.Floater, 10 },
			{ Item.Chime, 5 },
			{ Item.Cube, 7 },
			{ Item.Oxyale, 5 }
		};

		//This is the only hard restriction. If there's less than 50 RewardSources accessible, the Oxyale may not be placed.
		//The Chime, Cube, Oxyale aren't that important to continue the seed(there's stil the ship, the canoe, the key, the floater...) so it doesn't create problems.
		//But it's a safeguard, that the algorithm doesn't force you into sky at the beginning of the game.
		Dictionary<Item, int> SafeMinAccessibility = new Dictionary<Item, int>
		{
			{ Item.Chime, 60 },
			{ Item.Cube, 75 },
			{ Item.Oxyale, 50 }
		};

		//Same as above, but used with "Allow Unsafe placement"
		//Doesn't place as high restrictions on the Chime, Cube and Oxyale
		Dictionary<Item, float> UnsafeExceptionWeights = new Dictionary<Item, float>
		{
			{ Item.Bridge, 2.0f },
			{ Item.Key, 0.25f },
			{ Item.Canoe, 0.75f },
			{ Item.Floater, 0.25f },
			{ Item.Lute, 0.25f },
			{ Item.Oxyale, 1.25f },
			{ Item.Chime, 1.25f },
			{ Item.Adamant, 0.75f },
			{ Item.Herb, 0.75f },
			{ Item.Crown, 0.75f },
			{ Item.Crystal, 0.75f },
			{ Item.Bottle, 0.75f },
			{ Item.Slab, 0.75f },
			{ Item.Tnt, 0.75f }
		};

		Dictionary<Item, int> UnsafeExceptioMinCycles = new Dictionary<Item, int>
		{
			{ Item.Key, 5 },
			{ Item.Ship, 2 },
			{ Item.Canoe, 3 },
			{ Item.Floater, 10 },
			{ Item.Lute, 10 }
		};

		Dictionary<Item, int> UnsafeMinAccessibility = new Dictionary<Item, int>
		{
			{ Item.Chime, 5 },
			{ Item.Cube, 5 },
			{ Item.Oxyale, 5 }
		};

		//Incentivized Equipment should be late in the game, as such it has a lower chance and can only be placed at cycle 10 or higher.
		private float NonKeyItemWeight = 0.5f;
		private float NonKeyItemMinCycle = 10;

		//That's the thresholds the algorithm uses to ensure it always has a way forward(hopefully)
		//If there are 2 or less incentive locations available it's has to be careful not to waste any incentive locations
		//The reason for 2 is, that when you have the canoe and still are in the inner sea, neither the ship, nor the canal opens something up by themselves.
		//It requires both to continue. If it always has 2 incentive locations availble, it can always place both ship and canal and therefore open up the seed.
		//If there are more than 3 incentive locations available, it's in safe mode(incentive wise) and is happy to place duds(for variation)
		//but less likely to place items that open up the seed even further
		//In between is somewhere in between(chance wise)
		private int CriticalNonIncentiveSourceCount = 5;
		private int SafeNonIncentiveSourceCount = 20;
		private int CriticalIncentiveSourceCount = 2;
		private int SafeIncentiveSourceCount = 3;

		Dictionary<Item, float> exceptionWeights;
		Dictionary<Item, int> exceptionMinCycles;
		Dictionary<Item, int> minAccessibility;

		SCLogic logic;
		OwLocationData locations;
		Dictionary<int, SCLogicRewardSource> logicSources;
		SCRequirements freeRequirements;
		SCRequirements currentRequirements;

		protected override ItemPlacementResult DoSanePlacement(MT19337 rng, OwLocationData _locations)
		{
			locations = _locations;

			exceptionWeights = flags.AllowUnsafePlacement || (flags.Entrances ?? false) ? UnsafeExceptionWeights : SafeExceptionWeights;
			exceptionMinCycles = flags.AllowUnsafePlacement || (flags.Entrances ?? false) ? UnsafeExceptioMinCycles : SafeExceptioMinCycles;
			minAccessibility = flags.AllowUnsafePlacement || (flags.Entrances ?? false) ? UnsafeMinAccessibility : SafeMinAccessibility;

			sanityCounter = 0;
			var incentiveLocationPool = new HashSet<IRewardSource>(placementContext.IncentiveLocations, new RewardSourceEqualityComparer());
			var preBlackOrbLocationPool = placementContext.AllValidPreBlackOrbItemLocationsPlusForced.ToList();
			var preBlackOrbUnincentivizedLocationPool = preBlackOrbLocationPool.Where(x => !incentiveLocationPool.Any(y => y.Address == x.Address)).ToList();
			if ((bool)flags.LooseExcludePlacedDungeons)
				preBlackOrbUnincentivizedLocationPool = IncentivizedDungeons(preBlackOrbUnincentivizedLocationPool);

			var unincentivizedLocationPool = new HashSet<IRewardSource>(preBlackOrbUnincentivizedLocationPool, new RewardSourceEqualityComparer());

			var allRewardSources = preBlackOrbLocationPool.Append(new ItemShopSlot(shopItemLocation, Item.None)).ToList();

			List<IRewardSource> placedItems = null;
			List<Item> treasurePool = null;

			freeRequirements = BuildFreeRequirements();

			bool placementFailed;
			do
			{
				((SanityCheckerV2)checker).SetShipLocation(255);

				BuildLogic(allRewardSources);

				placementFailed = false;

				var balancedPicker = new RewardSourcePicker(0.5, flags.LooseItemsNpcBalance ? 7.0 : 1.0, checker);

				sanityCounter++;
				if (sanityCounter > 3) throw new InsaneException("Item Placement could not meet incentivization requirements!");

				placedItems = placementContext.ForcedItemPlacements.ToList();
				var incentives = new HashSet<Item>(placementContext.IncentiveItems);
				var nonincentives = placementContext.UnincentiveKeyItems.ToList();
				var shards = placementContext.Shards.ToList();
				treasurePool = placementContext.TreasurePool.ToList();
				var state = PlacementState.Normal;

				// Check if Ship was plandoed, set appropriate dock if so
				if (placedItems.TryFind(s => s.Item == Item.Ship, out var result))
				{
					((SanityCheckerV2)checker).SetShipLocation((int)logic.GetShipIndex(currentRequirements, result));
				}

				while (incentives.Count() > incentiveLocationPool.Count())
				{
					nonincentives.Add(incentives.SpliceRandom(rng));
				}

				if (nonincentives.Concat(incentives).Any())
				{
					HashSet<Item> allPlacements = new HashSet<Item>(nonincentives.Concat(incentives));
					//HashSet<Item> allKeyItems = new HashSet<Item>(MapChangeItems.Concat(FetchQuestItems).Concat(GatingItems).Intersect(allPlacements));
					HashSet<Item> allKeyItems = placementContext.KeyItemsToPlace.ToHashSet();

					//The sanity checker currently doesn't allow tracking which shops are available
					//It could be easily added, but a little randomnes can't hurt(or so I'm thinking)
					//So it places the vendoritem upfront.
					if ((bool)flags.NPCItems)
					{
						var itemShopItem = SelectVendorItem(incentives.ToList(), nonincentives, treasurePool, incentiveLocationPool, rng);
						placedItems.Add(new ItemShopSlot(shopItemLocation, itemShopItem));

						allPlacements.Remove(itemShopItem);
						allKeyItems.Remove(itemShopItem);
					}

					if (flags.Archipelago && allKeyItems.Contains(Item.Bridge))
					{
						var accessibleSources = GetAllAccessibleRewardSources(preBlackOrbUnincentivizedLocationPool, placedItems);

						var rewardSource = balancedPicker.Pick(accessibleSources, flags.LooseItemsForwardPlacement, flags.LooseItemsSpreadPlacement, false, rng);
						placedItems.Add(NewItemPlacement(rewardSource, Item.Bridge));

						allPlacements.Remove(Item.Bridge);
						allKeyItems.Remove(Item.Bridge);
					}

					//Since it can waste cycles, this number needs to be high enough to be able to place all incentives and all nonincentives and still have some leeway
					//When it's done it breaks anyway
					//When it reached enough cycles so no cycle based restricitons apply and can place neither an incentive nor a nonincentive it also breaks
					for (int cycle = 1; cycle <= 100; cycle++)
					{
						var accessibleSources = GetAllAccessibleRewardSources(preBlackOrbLocationPool, placedItems);

						//look how many rewardsources of each type are available
						var incSourceCount = accessibleSources.Where(s => incentiveLocationPool.Contains(s)).Count();
						var nonSourceCount = accessibleSources.Count - incSourceCount;

						List<(Item item, int incCount, int nonCount)> candidates = new List<(Item item, int incCount, int nonCount)>();

						//go through each KI, place it in the testRewardSource, run the sanity checker and see how many RewardSources are available afterwards.
						foreach (var item in allKeyItems)
						{
							if (flags.ShipCanalBeforeFloater && item == Item.Floater && (allKeyItems.Contains(Item.Ship) || allKeyItems.Contains(Item.Canal))) continue;

							var accessibleSources2 = GetAllAccessibleRewardSources(preBlackOrbLocationPool, placedItems, item);

							var incCount = accessibleSources2.Where(s => incentiveLocationPool.Contains(s)).Count();
							var nonCount = accessibleSources2.Count - incCount - nonSourceCount;
							incCount -= incSourceCount;

							//if the placed item is an incentive, it will take up an incentive location when it will be placed
							//and vice versa
							if (incentives.Contains(item)) incCount--;
							if (!incentives.Contains(item)) nonCount--;

							candidates.Add((item, incCount, nonCount));
						}

						//Since it can happen, that neither the ship, nor the canal by themselves open up something, but the combination does,
						//here the combination is checked
						//it grabs another Rewardsource, places Ship and Canal, runs the SanityChecker and calculates the results
						if (allKeyItems.Contains(Item.Ship) && allKeyItems.Contains(Item.Canal))
						{
							var accessibleSources2 = GetAllAccessibleRewardSources(preBlackOrbLocationPool, placedItems, Item.Ship, Item.Canal);

							var incCount = accessibleSources2.Where(s => incentiveLocationPool.Contains(s)).Count();
							var nonCount = accessibleSources2.Count - incCount - nonSourceCount;
							incCount -= incSourceCount;

							if (incentives.Contains(Item.Ship)) incCount--;
							if (!incentives.Contains(Item.Ship)) nonCount--;

							if (incentives.Contains(Item.Canal)) incCount--;
							if (!incentives.Contains(Item.Canal)) nonCount--;

							//it gets the ship and canal candidate from the list, later it's put back in
							var shipCandidate = candidates.FirstOrDefault(c => c.item == Item.Ship);
							var canalCandidate = candidates.FirstOrDefault(c => c.item == Item.Canal);

							candidates.Remove(shipCandidate);
							candidates.Remove(canalCandidate);

							//half of the opened up incentive locations and nonincentive locations should go to either item(rounded down)
							//In order to continue the seed, Ship and Canal must open up 2 incentive locations, since they may take up 2
							//it's not an exact estimate, since it doesn'T differentiate between them beeing incentive or nonincentive items
							incCount /= 2;
							nonCount /= 2;

							if (shipCandidate.incCount < incCount && canalCandidate.incCount < incCount && incSourceCount > 1)
							{
								shipCandidate.incCount = incCount;
								canalCandidate.incCount = incCount;
							}

							if (shipCandidate.nonCount < nonCount && canalCandidate.nonCount < nonCount && nonSourceCount > 1)
							{
								shipCandidate.nonCount = nonCount;
								canalCandidate.nonCount = nonCount;
							}

							candidates.Add(shipCandidate);
							candidates.Add(canalCandidate);
						}

						var incKeyItemCount = incentives.Where(i => allKeyItems.Contains(i)).Count();
						var nonKeyItemCount = nonincentives.Where(i => allKeyItems.Contains(i)).Count();

						List<(float weight, Item item)> weightedCandidates;

						var incItemCount = allPlacements.Where(i => incentives.Contains(i)).Count();

						//here it randomly decides whether to place and incentive or a non incentive
						//the chance is based on the number of incentive and nonincentive items
						//If however it couldn't place an incentive item in the last cycle, it'll always try a nonincentive this time
						//and vice versa
						if (state == PlacementState.Incentive || state == PlacementState.Normal && rng.Between(1, allPlacements.Count) <= incItemCount || flags.LaterLoose && state == PlacementState.Normal && cycle <= 5)
						{
							//Filter out nonincentive items
							candidates = candidates.Where(c => incentives.Contains(c.item)).ToList();
							var nonKeyCandidates = allPlacements.Where(i => !allKeyItems.Contains(i) && incentives.Contains(i));

							if (incSourceCount <= 1 || incSourceCount <= CriticalIncentiveSourceCount && allKeyItems.Contains(Item.Ship) && allKeyItems.Contains(Item.Canal))
							{
								//we are in incentive critical mode
								//the function takes weights for different categories of items(depending of how they're expected they change the amount of incentive locations available)
								//bin0 doesn't open up a new location(it therefore reduces the incentive location count by one)
								//bin1 opens up 1 location and therefore keeps the count the same
								//bin2 opens up 2 locations and therefore increases the amount by 1
								//bin3 opens up more than two locations. It has two parameters here, more on that further down

								//Anyway, we can't afford to reduce the incentive location count here(bin0)
								//We also don't neccesarily want to keep it the same, but that's okish(bin1)
								//we want to increase the amount(bin2, bin3)
								weightedCandidates = AssignWeights(candidates, 0.0f, 0.5f, 1.0f, 1.0f, 0.5f, cycle, accessibleSources.Count);

							}
							else if (incSourceCount <= SafeIncentiveSourceCount)
							{
								//We have 1 incentive location to squander, not a lot, but sufficient
								//We don't want to reduce it or keep it the same, but it would still be ok(bin0, bin1)
								//We'd like to increase it a little(bin2)
								//but not too much(bin3)
								weightedCandidates = AssignWeights(candidates, 0.5f, 0.5f, 1.0f, 0.5f, 0.25f, cycle, accessibleSources.Count);

								//it's also ok to place a NonKeyItem, wa have an incentive location to squander after all
								//but don't make it too likely
								weightedCandidates.AddRange(nonKeyCandidates.Select(i => (GetItemWeight(i, 0.5f, cycle, accessibleSources.Count), i)));
							}
							else
							{
								//We have at least 2 incentive locations to squander(considered a lot)
								//We like reducing it by one(for variety)
								//We like keeping it the same(for variety)
								//We also like to increase it slightly(for variety)
								//But we don't like to increase it a lot, but then again it's not a problem, so make it less likely
								weightedCandidates = AssignWeights(candidates, 1.0f, 1.0f, 1.0f, 0.25f, 0.25f, cycle, accessibleSources.Count);

								//Also we like placing NoneKeyItems
								weightedCandidates.AddRange(nonKeyCandidates.Select(i => (GetItemWeight(i, 1.0f, cycle, accessibleSources.Count), i)));
							}

							//If we didn't find any candidates, but we have enough locations to place all remaining items, then we just do that
							//Otherwise the algorithm would refuse to give up the last 2 incentive locations
							if (weightedCandidates.Sum(i => i.weight) == 0 && incItemCount <= incSourceCount)
							{
								weightedCandidates.AddRange(allPlacements.Where(i => incentives.Contains(i)).Select(i => (1.0f, i)));
							}

							//If we don't have any candidates and not enough incentive locations to place all, we can't place an incentive in this cycle
							if (weightedCandidates.Sum(i => i.weight) == 0)
							{
								//If it's at least cycle 15 and therefore no cycle based restricctions apply
								//And in the last cycle we tried to place a NonIncentive
								//we had all KI options available, but couldn't place any of them, so we're doomed
								if (cycle >= 15 && state != PlacementState.Normal)
								{
									placementFailed = true;
									break;
								}

								//since we couldn't place an incentive, we won't be able to do so next cycle(same results), so we don't try to do that next cycle
								//This heavily speeds up the situations when there are very few loose(low chance of trying to place a loose), but a loose is required to go on.
								state = PlacementState.NonIncentice;
								continue;
							}
						}
						else
						{
							//same as above, but for nonincentive items.
							//The main difference is that the RewardSource thresholds are way higher
							//While it's not a safety concern, cou generally want a lot more chest's available than neccesary to be in the confort zone
							candidates = candidates.Where(c => !incentives.Contains(c.item)).ToList();
							var nonKeyCandidates = allPlacements.Where(i => !allKeyItems.Contains(i) && !incentives.Contains(i));

							if (nonSourceCount <= CriticalNonIncentiveSourceCount)
							{
								weightedCandidates = AssignWeights2(candidates, 0.0f, 0.5f, 1.0f, 1.0f, 0.5f, cycle, accessibleSources.Count);
							}
							else if (nonSourceCount <= SafeNonIncentiveSourceCount)
							{
								weightedCandidates = AssignWeights2(candidates, 0.5f, 0.5f, 1.0f, 0.5f, 0.25f, cycle, accessibleSources.Count);
								weightedCandidates.AddRange(nonKeyCandidates.Select(i => (GetItemWeight(i, 0.5f, cycle, accessibleSources.Count), i)));
							}
							else
							{
								weightedCandidates = AssignWeights2(candidates, 1.0f, 1.0f, 1.0f, 0.25f, 0.25f, cycle, accessibleSources.Count);
								weightedCandidates.AddRange(nonKeyCandidates.Select(i => (GetItemWeight(i, 1.0f, cycle, accessibleSources.Count), i)));
							}

							if (weightedCandidates.Sum(i => i.weight) == 0)
							{
								if (cycle >= 15 && state != PlacementState.Normal)
								{
									placementFailed = true;
									break;
								}

								state = PlacementState.Incentive;
								continue;
							}
						}

						//The above weighting strategy works with this mindset:
						//If it's an all incentive seed, we just need to manage the incentive locations
						//If it's an all loose seed, we just need to manage the nonincentive locations
						//If it's something in between, we switch between those two ideas.
						//It works, because a nonincentive item goes into a nonincentive chest and can't negatively impact the incentive locations.
						//and vice versa.
						//There is a flaw in this simplistic approach. When placing a nonincentive, it could additionally weigh in if it's desired for incentive placment.
						//But that's a thing for another time to think through

						//remove the candidates with weight 0(should be unneccesary, but anyway)
						weightedCandidates = weightedCandidates.Where(w => w.weight > 0).ToList();

						//shuffle the list(should be totally unneccesary, but it makes me more comfortable)
						weightedCandidates.Shuffle(rng);

						//randomly select an item from the weighted list
						Item nextPlacment = Item.None;

						float sum = weightedCandidates.Sum(i => i.weight);

						var r = rng.Next() / (float)uint.MaxValue * sum;

						sum = 0.0f;
						foreach (var s in weightedCandidates)
						{
							sum += s.weight;
							if (r <= sum)
							{
								nextPlacment = s.item;
								break;
							}
						}

						//if we didn't find one, just select a random one(this shouldn't happen, but safe is safe)
						if (nextPlacment == Item.None) nextPlacment = weightedCandidates.PickRandom(rng).item;

						//place the item(same code as in GuidedPlacement to have compatibility with other flags)
						if (incentives.Contains(nextPlacment))
						{
							var rewardSources = accessibleSources.Where(s => incentiveLocationPool.Contains(s)).ToList();

							//can't happen per definition. We already know, that there is a location available for the item(but in case anything unexpectedly goes wrong sure)
							if (rewardSources.Count == 0) continue;

							var rewardSource = balancedPicker.Pick(rewardSources, false, flags.LooseItemsSpreadPlacement, false, rng);
							placedItems.Add(NewItemPlacement(rewardSource, nextPlacment));
						}
						else
						{
							var rewardSources = accessibleSources.Where(s => unincentivizedLocationPool.Contains(s)).ToList();
							if (rewardSources.Count == 0) continue;

							var rewardSource = balancedPicker.Pick(rewardSources, flags.LooseItemsForwardPlacement, flags.LooseItemsSpreadPlacement, false, rng);
							placedItems.Add(NewItemPlacement(rewardSource, nextPlacment));
						}

						//remove the item from the lists of items to place
						allPlacements.Remove(nextPlacment);
						allKeyItems.Remove(nextPlacment);

						if (nextPlacment == Item.Ship) ((SanityCheckerV2)checker).SetShipLocation((int)logic.GetShipIndex(currentRequirements, placedItems.Last()));

						//we placed an item so we should randomly select incentive/nonincentive next cycle
						state = PlacementState.Normal;

						//we're finished
						if (allPlacements.Count == 0) break;
					}

					//we couldn't place all items or encountered a placement problem
					if (allPlacements.Count > 0 || placementFailed)
					{
						continue;
					}
				}

				//place the shards(almost same code as in GuidedPlacement)
				if (!placementFailed && shards.Count > 0)
				{
					var leftoverItemLocations = GetAllAccessibleRewardSources(preBlackOrbLocationPool, placedItems);

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

				//finally check the placement(if we arrive here, it's safe, but let's do it anyway in case something goes wrong)
			} while (placementFailed || !checker.CheckSanity(placedItems, null, flags, false).Complete);

			return new ItemPlacementResult { PlacedItems = placedItems, RemainingTreasures = treasurePool };
		}

		private static Dictionary<Item, SCRequirements> itemRequirements = new()
		{
			{ Item.Lute, SCRequirements.Lute },
			{ Item.Bridge, SCRequirements.Bridge },
			{ Item.Ship, SCRequirements.Ship },
			{ Item.Crown, SCRequirements.Crown },
			{ Item.Crystal, SCRequirements.Crystal },
			{ Item.Herb, SCRequirements.Herb },
			{ Item.Key, SCRequirements.Key },
			{ Item.Tnt, SCRequirements.Tnt },
			{ Item.Canal, SCRequirements.Canal },
			{ Item.Ruby, SCRequirements.Ruby },
			{ Item.Rod, SCRequirements.Rod },
			{ Item.Canoe, SCRequirements.Canoe },
			{ Item.Floater, SCRequirements.Floater },
			{ Item.Oxyale, SCRequirements.Oxyale },
			{ Item.Slab, SCRequirements.Slab },
			{ Item.Chime, SCRequirements.Chime },
			{ Item.Cube, SCRequirements.Cube },
		};

		private SCRequirements BuildFreeRequirements()
		{
			var requirements = SCRequirements.None;

			foreach (var freeItem in placementContext.FreeItems)
			{
				if (itemRequirements.TryGetValue(freeItem, out var reqs))
				{
					requirements |= reqs;
				}
			}

			return requirements;
		}

		private void BuildLogic(List<IRewardSource> preBlackOrbLocationPool)
		{
			logic = new SCLogic(base.rom, ((SanityCheckerV2)base.checker).Main, preBlackOrbLocationPool, locations, base.flags, false);
			logicSources = logic.RewardSources.ToDictionary(r => r.RewardSource.Address);
			//var missingsources = preBlackOrbLocationPool.Where(i => !logicSources.TryGetValue(i.Address, out var l)).ToList();
		}

		//retrieve all accessible RewardSources(with no item in them) from the SanityChecker
		private List<IRewardSource> GetAllAccessibleRewardSources(IEnumerable<IRewardSource> preBlackOrbLocationPool, List<IRewardSource> placedItems, Item item1 = Item.None, Item item2 = Item.None)
		{
			var placedItems2 = placedItems.Select(i => logicSources.TryGetValue(i.Address, out var l) ? new SCLogicRewardSource { Requirements = l.Requirements, RewardSource = i } : null).Where(l => l != null).ToList();

			SCRequirements requirements = freeRequirements;

			if (item1 != Item.None) requirements = requirements.AddItem(item1);
			if (item2 != Item.None) requirements = requirements.AddItem(item2);

			List<SCLogicRewardSource> toRemove = new List<SCLogicRewardSource>(placedItems2.Count);

			var maxCount = placedItems2.Count;
			for (int i = 0; i < maxCount; i++)
			{
				foreach (var r in placedItems2.Where(l => l.Requirements.IsAccessible(requirements)))
				{
					toRemove.Add(r);
					requirements = requirements.AddItem(r.RewardSource.Item);
				}

				foreach (var r in toRemove) placedItems2.Remove(r);

				if (toRemove.Count == 0) break;
				if (placedItems2.Count == 0) break;

				toRemove.Clear();
			}

			if (item1 == Item.None && item2 == Item.None)
			{
				currentRequirements = requirements;
			}

			return preBlackOrbLocationPool.Where(i => logicSources.TryGetValue(i.Address, out var l) && l.Requirements.IsAccessible(requirements)).Where(x => !placedItems.Any(y => y.Address == x.Address)).ToList();
		}

		//Assign weights to all candidates based on the state of the seed, the restrictions and the bin weights
		private List<(float weight, Item item)> AssignWeights(List<(Item item, int incCount, int nonCount)> candidates, float bin0, float bin1, float bin2, float bin3, float bin3red, int cycle, int accessibility)
		{
			var weightedCandidates = new List<(float weight, Item item)>();

			//bin0 reduces the amount of locations by 1
			weightedCandidates.AddRange(candidates.Where(c => c.incCount < 0).Select(c => (GetItemWeight(c.item, bin0, cycle, accessibility), c.item)));

			//bin1 keeps it the same
			weightedCandidates.AddRange(candidates.Where(c => c.incCount == 0).Select(c => (GetItemWeight(c.item, bin1, cycle, accessibility), c.item)));

			//bin2 increases it by one
			weightedCandidates.AddRange(candidates.Where(c => c.incCount == 1).Select(c => (GetItemWeight(c.item, bin2, cycle, accessibility), c.item)));

			//bin3 increases it by more than 1
			//we would like to weigh the candidates towards items that open up less
			//we don't want to give you the floater, if the ship would suffice for the moment
			//the first item in this bin is given tha bin3 weight
			//subsequent items are given ever smaller fractions of that weight defined by bin3red
			float weight = bin3;
			foreach (var c in candidates.Where(c => c.incCount >= 2).OrderBy(c => c.incCount))
			{
				weightedCandidates.Add((GetItemWeight(c.item, weight, cycle, accessibility), c.item));
				weight *= bin3red;
			}

			return weightedCandidates;
		}

		//this function applies the special rules defined at the beginning of the class
		//if there aren't enough available RewardSources for this item return a weight of 0
		//if not enough cycles have passed for this item return a weight of 0
		//if there is an exceptionweight for this item apply it as a factor
		private float GetItemWeight(Item item, float weight, int cycle, int accessibility)
		{
			if (minAccessibility.TryGetValue(item, out var minAccess) && accessibility < minAccess) return 0.0f;

			if (exceptionMinCycles.TryGetValue(item, out var minCycle) && cycle < minCycle) return 0.0f;
			if (!KeyItems.Contains(item) && cycle < NonKeyItemMinCycle) return 0.0f;

			if (exceptionWeights.TryGetValue(item, out var factor)) return weight * factor;
			if (!KeyItems.Contains(item)) return weight * NonKeyItemWeight;
			return weight;
		}

		//the same as the one above, but for nonincentives
		//only difference are the bin thresholds and the variable names
		//bin0 reduces ocations by 1
		//bin1 keeps it the same or oncreases it by up to 4
		//bin2 increases it by up to 19
		//bin3 increases it by at least 20
		private List<(float weight, Item item)> AssignWeights2(List<(Item item, int incCount, int nonCount)> candidates, float bin0, float bin1, float bin2, float bin3, float bin3red, int cycle, int accessibility)
		{
			var weightedCandidates = new List<(float weight, Item item)>();

			weightedCandidates.AddRange(candidates.Where(c => c.nonCount < 0).Select(c => (GetItemWeight(c.item, bin0, cycle, accessibility), c.item)));
			weightedCandidates.AddRange(candidates.Where(c => c.nonCount >= 0 && c.nonCount < 5).Select(c => (GetItemWeight(c.item, bin1, cycle, accessibility), c.item)));
			weightedCandidates.AddRange(candidates.Where(c => c.nonCount >= 5 && c.nonCount < 20).Select(c => (GetItemWeight(c.item, bin2, cycle, accessibility), c.item)));

			float weight = bin3;
			foreach (var c in candidates.Where(c => c.nonCount >= 20).OrderBy(c => c.nonCount))
			{
				weightedCandidates.Add((GetItemWeight(c.item, weight, cycle, accessibility), c.item));
				weight *= bin3red;
			}

			return weightedCandidates;
		}

		private enum PlacementState
		{
			Normal,
			Incentive,
			NonIncentice
		}
	}
}
