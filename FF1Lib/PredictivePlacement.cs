using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace FF1Lib
{
	public class PredictivePlacement : ItemPlacement
	{
		static HashSet<Item> MapChangeItems = new HashSet<Item> { Item.Bridge, Item.Canoe, Item.Ship, Item.Canal, Item.Floater, Item.Ruby, Item.Chime };
		static HashSet<Item> FetchQuestItems = new HashSet<Item> { Item.Crystal, Item.Herb, Item.Adamant, Item.Slab, Item.Bottle };
		static HashSet<Item> GatingItems = new HashSet<Item> { Item.Crown, Item.Key, Item.Lute, Item.Rod, Item.Cube, Item.Oxyale, Item.Tnt };
		static HashSet<Item> KeyItems = new HashSet<Item>(MapChangeItems.Concat(FetchQuestItems).Concat(GatingItems));

		Dictionary<Item, float> SafeExceptionWeights = new Dictionary<Item, float>
		{
			{ Item.Bridge, 2.0f },
			{ Item.Key, 0.25f },
			{ Item.Canoe, 0.75f },
			{ Item.Floater, 0.25f },
			{ Item.Chime, 0.25f },
			{ Item.Cube, 0.25f },
			{ Item.Oxyale, 0.25f }
		};

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

		Dictionary<Item, int> SafeMinAccessibility = new Dictionary<Item, int>
		{
			{ Item.Key, 15 },
			{ Item.Floater, 100 },
			{ Item.Chime, 60 },
			{ Item.Cube, 75 },
			{ Item.Oxyale, 50 }
		};

		Dictionary<Item, float> UnsafeExceptionWeights = new Dictionary<Item, float>
		{
			{ Item.Bridge, 2.0f },
			{ Item.Key, 0.25f },
			{ Item.Canoe, 0.75f },
			{ Item.Floater, 0.25f },
			{ Item.Chime, 0.5f },
			{ Item.Cube, 0.5f },
			{ Item.Oxyale, 0.5f }
		};

		Dictionary<Item, int> UnsafeExceptioMinCycles = new Dictionary<Item, int>
		{
			{ Item.Key, 5 },
			{ Item.Ship, 2 },
			{ Item.Canoe, 3 },
			{ Item.Floater, 10 },
			{ Item.Chime, 3 },
			{ Item.Cube, 3 },
			{ Item.Oxyale, 3 }
		};

		Dictionary<Item, int> UnsafeMinAccessibility = new Dictionary<Item, int>
		{
			{ Item.Key, 15 },
			{ Item.Floater, 50 },
			{ Item.Chime, 5 },
			{ Item.Cube, 5 },
			{ Item.Oxyale, 5 }
		};

		private float NonKeyItemWeight = 0.5f;
		private float NonKeyItemMinCycle = 10;

		private int CriticalNonIncentiveSourceCount = 5;
		private int SafeNonIncentiveSourceCount = 20;
		private int CriticalIncentiveSourceCount = 2;
		private int SafeIncentiveSourceCount = 3;

		Dictionary<Item, float> exceptionWeights;
		Dictionary<Item, int> exceptionMinCycles;
		Dictionary<Item, int> minAccessibility;

		protected override ItemPlacementResult DoSanePlacement(MT19337 rng, ItemPlacementContext ctx)
		{
			exceptionWeights = _flags.AllowUnsafePlacement || (_flags.Entrances ?? false) ? UnsafeExceptionWeights : SafeExceptionWeights;
			exceptionMinCycles = _flags.AllowUnsafePlacement || (_flags.Entrances ?? false) ? UnsafeExceptioMinCycles : SafeExceptioMinCycles;
			minAccessibility = _flags.AllowUnsafePlacement || (_flags.Entrances ?? false) ? UnsafeMinAccessibility : SafeMinAccessibility;

			_sanityCounter = 0;
			var incentiveLocationPool = new HashSet<IRewardSource>(_incentivesData.IncentiveLocations, new RewardSourceEqualityComparer());
			var preBlackOrbLocationPool = _incentivesData.AllValidPreBlackOrbItemLocations.ToList();
			var preBlackOrbUnincentivizedLocationPool = preBlackOrbLocationPool.Where(x => !incentiveLocationPool.Any(y => y.Address == x.Address)).ToList();
			if ((bool)_flags.LooseExcludePlacedDungeons)
				preBlackOrbUnincentivizedLocationPool = IncentivizedDungeons(preBlackOrbUnincentivizedLocationPool);

			var unincentivizedLocationPool = new HashSet<IRewardSource>(preBlackOrbUnincentivizedLocationPool, new RewardSourceEqualityComparer());

			List<IRewardSource> placedItems = null;
			List<Item> treasurePool = null;

			bool placementFailed;
			do
			{
				placementFailed = false;

				var balancedPicker = new RewardSourcePicker(0.5, _flags.LooseItemsNpcBalance ? 7.0 : 1.0, _checker);

				_sanityCounter++;
				if (_sanityCounter > 20) throw new InsaneException("Item Placement could not meet incentivization requirements!");

				placedItems = ctx.Forced.ToList();
				var incentives = new HashSet<Item>(ctx.Incentivized);
				var nonincentives = ctx.Unincentivized.ToList();
				var shards = ctx.Shards.ToList();
				treasurePool = ctx.AllTreasures.ToList();

				while (incentives.Count() > incentiveLocationPool.Count())
				{
					nonincentives.Add(incentives.SpliceRandom(rng));
				}

				if (((bool)_flags.NPCItems) || ((bool)_flags.NPCFetchItems))
				{
					HashSet<Item> allPlacements = new HashSet<Item>(nonincentives.Concat(incentives));
					HashSet<Item> allKeyItems = new HashSet<Item>(MapChangeItems.Concat(FetchQuestItems).Concat(GatingItems));

					if ((bool)_flags.IsFloaterRemoved)
					{
						allKeyItems.Remove(Item.Floater);
					}

					var itemShopItem = SelectVendorItem(incentives.ToList(), nonincentives, treasurePool, incentiveLocationPool, rng);
					placedItems.Add(new ItemShopSlot(_caravanItemLocation, itemShopItem));

					allPlacements.Remove(itemShopItem);
					allKeyItems.Remove(itemShopItem);

					for (int cycle = 1; cycle <= 100; cycle++)
					{
						var (_, mapLocations, requirements) = _checker.CheckSanity(placedItems, null, _flags);
						var accessibleSources = GetAllAccessibleRewardSources(preBlackOrbLocationPool, placedItems);

						var incSourceCount = accessibleSources.Where(s => incentiveLocationPool.Contains(s)).Count();
						var nonSourceCount = accessibleSources.Count - incSourceCount;

						var testSource = accessibleSources.FirstOrDefault();
						if (testSource == null)
						{
							placementFailed = true;
							break;
						}

						List<(Item item, int incCount, int nonCount)> candidates = new List<(Item item, int incCount, int nonCount)>();

						foreach (var item in allKeyItems)
						{
							placedItems.Add(NewItemPlacement(testSource, item));
							_ = _checker.CheckSanity(placedItems, null, _flags);
							placedItems.RemoveAt(placedItems.Count - 1);

							var accessibleSources2 = GetAllAccessibleRewardSources(preBlackOrbLocationPool, placedItems);

							var incCount = accessibleSources2.Where(s => incentiveLocationPool.Contains(s)).Count();
							var nonCount = accessibleSources2.Count - incCount - nonSourceCount;
							incCount -= incSourceCount;

							if (incentives.Contains(item) && !incentiveLocationPool.Contains(testSource)) { incCount--; nonCount++; }
							if (!incentives.Contains(item) && incentiveLocationPool.Contains(testSource)) { incCount++; nonCount--; }

							candidates.Add((item, incCount, nonCount));
						}

						if (allKeyItems.Contains(Item.Ship) && allKeyItems.Contains(Item.Canal))
						{
							var testSource2 = accessibleSources.LastOrDefault();
							if (testSource2 == null || testSource.Address == testSource2.Address)
							{
								placementFailed = true;
								break;
							}

							placedItems.Add(NewItemPlacement(testSource, Item.Ship));
							placedItems.Add(NewItemPlacement(testSource2, Item.Canal));
							_ = _checker.CheckSanity(placedItems, null, _flags);
							placedItems.RemoveAt(placedItems.Count - 1);
							placedItems.RemoveAt(placedItems.Count - 1);

							var accessibleSources2 = GetAllAccessibleRewardSources(preBlackOrbLocationPool, placedItems);

							var incCount = accessibleSources2.Where(s => incentiveLocationPool.Contains(s)).Count();
							var nonCount = accessibleSources2.Count - incCount - nonSourceCount;
							incCount -= incSourceCount;

							if (incentives.Contains(Item.Ship) && !incentiveLocationPool.Contains(testSource)) { incCount--; nonCount++; }
							if (!incentives.Contains(Item.Ship) && incentiveLocationPool.Contains(testSource)) { incCount++; nonCount--; }

							if (incentives.Contains(Item.Canal) && !incentiveLocationPool.Contains(testSource2)) { incCount--; nonCount++; }
							if (!incentives.Contains(Item.Canal) && incentiveLocationPool.Contains(testSource2)) { incCount++; nonCount--; }

							var shipCandidate = candidates.FirstOrDefault(c => c.item == Item.Ship);
							var canalCandidate = candidates.FirstOrDefault(c => c.item == Item.Canal);

							candidates.Remove(shipCandidate);
							candidates.Remove(canalCandidate);

							incCount /= 2;
							nonCount /= 2;

							if (shipCandidate.incCount < incCount && canalCandidate.incCount < incCount)
							{
								shipCandidate.incCount = incCount;
								canalCandidate.incCount = incCount;
							}

							if (shipCandidate.nonCount < nonCount && canalCandidate.nonCount < nonCount)
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

						if (rng.Between(1, allPlacements.Count) <= incItemCount)
						{
							candidates = candidates.Where(c => incentives.Contains(c.item)).ToList();

							//place incentive
							if (incSourceCount <= CriticalIncentiveSourceCount)
							{
								weightedCandidates = AssignWeights(candidates, 0.0f, 0.5f, 1.0f, 1.0f, 0.5f, cycle, accessibleSources.Count);

							}
							else if (incSourceCount <= SafeIncentiveSourceCount)
							{
								weightedCandidates = AssignWeights(candidates, 0.5f, 0.5f, 1.0f, 0.5f, 0.25f, cycle, accessibleSources.Count);
								weightedCandidates.AddRange(allPlacements.Where(i => !allKeyItems.Contains(i) && incentives.Contains(i)).Select(i => (GetItemWeight(i, 0.5f, cycle, accessibleSources.Count), i)));
							}
							else
							{
								weightedCandidates = AssignWeights(candidates, 1.0f, 1.0f, 1.0f, 0.25f, 0.25f, cycle, accessibleSources.Count);
								weightedCandidates.AddRange(allPlacements.Where(i => !allKeyItems.Contains(i) && incentives.Contains(i)).Select(i => (GetItemWeight(i, 1.0f, cycle, accessibleSources.Count), i)));
							}

							if (weightedCandidates.Sum(i => i.weight) == 0 && incItemCount <= incSourceCount)
							{
								weightedCandidates.AddRange(allPlacements.Where(i => incentives.Contains(i) && incentives.Contains(i)).Select(i => (1.0f, i)));
							}
						}
						else
						{
							candidates = candidates.Where(c => !incentives.Contains(c.item)).ToList();

							//place nonincentive
							if (nonSourceCount <= CriticalNonIncentiveSourceCount)
							{
								weightedCandidates = AssignWeights2(candidates, 0.0f, 0.5f, 1.0f, 1.0f, 0.5f, cycle, accessibleSources.Count);
							}
							else if (nonSourceCount <= SafeNonIncentiveSourceCount)
							{
								weightedCandidates = AssignWeights2(candidates, 0.5f, 0.5f, 1.0f, 0.5f, 0.25f, cycle, accessibleSources.Count);
								weightedCandidates.AddRange(allPlacements.Where(i => !allKeyItems.Contains(i)).Select(i => (GetItemWeight(i, 0.5f, cycle, accessibleSources.Count), i)));
							}
							else
							{
								weightedCandidates = AssignWeights2(candidates, 1.0f, 1.0f, 1.0f, 0.25f, 0.25f, cycle, accessibleSources.Count);
								weightedCandidates.AddRange(allPlacements.Where(i => !allKeyItems.Contains(i)).Select(i => (GetItemWeight(i, 1.0f, cycle, accessibleSources.Count), i)));
							}
						}

						if (weightedCandidates.Sum(i => i.weight) == 0) continue;

						weightedCandidates = weightedCandidates.Where(w => w.weight > 0).ToList();
						weightedCandidates.Shuffle(rng);

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

						if (nextPlacment == Item.None) nextPlacment = weightedCandidates.PickRandom(rng).item;

						if (incentives.Contains(nextPlacment))
						{
							var rewardSources = accessibleSources.Where(s => incentiveLocationPool.Contains(s)).ToList();
							if (rewardSources.Count == 0) continue;

							var rewardSource = balancedPicker.Pick(rewardSources, false, _flags.LooseItemsSpreadPlacement, false, rng);
							placedItems.Add(NewItemPlacement(rewardSource, nextPlacment));
						}
						else
						{
							var rewardSources = accessibleSources.Where(s => !incentiveLocationPool.Contains(s)).ToList();
							if (rewardSources.Count == 0) continue;

							var rewardSource = balancedPicker.Pick(rewardSources, _flags.LooseItemsForwardPlacement, _flags.LooseItemsSpreadPlacement, false, rng);
							placedItems.Add(NewItemPlacement(rewardSource, nextPlacment));
						}

						allPlacements.Remove(nextPlacment);
						allKeyItems.Remove(nextPlacment);

						if (allPlacements.Count == 0) break;
					}

					if (allPlacements.Count > 0 || placementFailed)
					{
						continue;
					}
				}

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

			} while (placementFailed || !_checker.CheckSanity(placedItems, null, _flags).Complete);

			return new ItemPlacementResult { PlacedItems = placedItems, RemainingTreasures = treasurePool };
		}

		private List<IRewardSource> GetAllAccessibleRewardSources(IEnumerable<IRewardSource> preBlackOrbLocationPool, List<IRewardSource> placedItems)
		{
			return preBlackOrbLocationPool.Where(x => !placedItems.Any(y => y.Address == x.Address)
							&&  _checker.IsRewardSourceAccessible(x, AccessRequirement.None , null))
							.ToList();
		}

		private List<(float weight, Item item)> AssignWeights(List<(Item item, int incCount, int nonCount)> candidates, float bin0, float bin1, float bin2, float bin3, float bin3red, int cycle, int accessibility)
		{
			var weightedCandidates = new List<(float weight, Item item)>();

			weightedCandidates.AddRange(candidates.Where(c => c.incCount < 0).Select(c => (GetItemWeight(c.item, bin0, cycle, accessibility), c.item)));
			weightedCandidates.AddRange(candidates.Where(c => c.incCount == 0).Select(c => (GetItemWeight(c.item, bin1, cycle, accessibility), c.item)));
			weightedCandidates.AddRange(candidates.Where(c => c.incCount == 1).Select(c => (GetItemWeight(c.item, bin2, cycle, accessibility), c.item)));
			
			float weight = bin3;
			foreach (var c in candidates.Where(c => c.incCount >= 2).OrderBy(c => c.incCount))
			{
				weightedCandidates.Add((GetItemWeight(c.item, weight, cycle, accessibility), c.item));
				weight *= bin3red;
			}

			return weightedCandidates;
		}

		private float GetItemWeight(Item item, float weight, int cycle, int accessibility)
		{
			if (minAccessibility.TryGetValue(item, out var minAccess) && accessibility < minAccess) return 0.0f;

			if (exceptionMinCycles.TryGetValue(item, out var minCycle) && cycle < minCycle) return 0.0f;
			if (!KeyItems.Contains(item) && cycle < NonKeyItemMinCycle) return 0.0f;

			if (exceptionWeights.TryGetValue(item, out var factor)) return weight * factor;
			if (!KeyItems.Contains(item)) return weight * NonKeyItemWeight;
			return weight;
		}

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
	}
}
