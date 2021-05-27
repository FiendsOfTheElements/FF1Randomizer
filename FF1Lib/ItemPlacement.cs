using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace FF1Lib
{
	public class InsaneException : Exception
	{
		public InsaneException(string message)
			: base(message) { }
	}

	public abstract class ItemPlacement
	{
		private const Item ReplacementItem = Item.Cabin;

		private List<IRewardSource> TrapChests = new List<IRewardSource>()
		{
			ItemLocations.ToFTopRight2, // In-Out trap tile forced
			ItemLocations.ToFBottomRight, // Locked trap tile
			ItemLocations.MarshCave12, // Locked, center, trap tile
			ItemLocations.EarthCave10, // Entrance trap, vanilla wooden shield
			ItemLocations.EarthCave12, // Room before Vampire, trapped
			ItemLocations.EarthCave20, // Far corner chest in Earth B4, top room
			ItemLocations.Volcano6, // Armory trapped tile
			ItemLocations.Volcano15, // Top hairpin, trapped, vanilla Giant Sword
			ItemLocations.Volcano31, // Trapped far room B4, vanilla flame shield
			ItemLocations.VolcanoMajor, // Red D chest
			ItemLocations.IceCave6, // Trapped chest to right of floater
			ItemLocations.IceCaveMajor, // Vanilla floater
			ItemLocations.IceCave15, // In-out trap tile forced B3, vanilla Silver Gauntlet
			ItemLocations.SeaShrine9, // B4 trapped chest, vanilla Power Gauntlet
			ItemLocations.SeaShrine3, // In-Out trap Sharknado chest
			ItemLocations.SkyPalace33, // Top chest B3, vanilla Pro-Ring
		};

		public static ItemPlacement Create(IItemPlacementFlags flags, IncentiveData incentivesData, List<Item> allTreasures, ItemShopSlot caravanItemLocation, OverworldMap overworldMap, ISanityChecker checker)
		{
			ItemPlacement placement;
			placement = new GuidedItemPlacement();

			placement._flags = flags;
			placement._incentivesData = incentivesData;
			placement._allTreasures = allTreasures;
			placement._caravanItemLocation = caravanItemLocation;
			placement._overworldMap = overworldMap;
			placement._checker = checker;

			return placement;
		}

		protected struct ItemPlacementContext
		{
			public List<IRewardSource> Forced;
			public List<Item> Incentivized;
			public List<Item> Unincentivized;
			public List<Item> Shards;
			public IReadOnlyCollection<Item> AllTreasures;
		}

		protected struct ItemPlacementResult
		{
			public List<IRewardSource> PlacedItems;
			public List<Item> RemainingTreasures;
		}

		protected int _sanityCounter = 0;
		protected IItemPlacementFlags _flags;
		protected IncentiveData _incentivesData;
		protected List<Item> _allTreasures;
		protected ItemShopSlot _caravanItemLocation;
		protected OverworldMap _overworldMap;
		protected ISanityChecker _checker;

		protected abstract ItemPlacementResult DoSanePlacement(MT19337 rng, ItemPlacementContext ctx);

		public List<IRewardSource> PlaceSaneItems(MT19337 rng, FF1Rom rom)
		{
			var incentivePool = _incentivesData.IncentiveItems.Where(x => _allTreasures.Contains(x)).ToList();
			var forcedItems = _incentivesData.ForcedItemPlacements.ToList();

			var unincentivizedQuestItems =
				ItemLists.AllQuestItems
					.Where(x => !incentivePool.Contains(x) &&
								_allTreasures.Contains(x) &&
								!forcedItems.Any(y => y.Item == x))
								.ToList();
			var shards = new List<Item>();

			var treasurePool = _allTreasures.ToList();

			if ((bool)_flags.GuaranteedRuseItem)
			{
				unincentivizedQuestItems.Add(Item.PowerRod);
			}
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
			while (treasurePool.Remove(Item.Shard))
			{
				shards.Add(Item.Shard);
			}

			ItemPlacementContext ctx = new ItemPlacementContext
			{
				Forced = forcedItems,
				Incentivized = incentivePool,
				Unincentivized = unincentivizedQuestItems,
				Shards = shards,
				AllTreasures = treasurePool.ToList(),
			};

			ItemPlacementResult result = DoSanePlacement(rng, ctx);
			List<IRewardSource> placedItems = result.PlacedItems;

			treasurePool = result.RemainingTreasures;

			if ((bool)_flags.FreeBridge)
			{
				placedItems = placedItems.Select(x => x.Item != Item.Bridge ? x : NewItemPlacement(x, ReplacementItem)).ToList();
			}
			if ((bool)_flags.IsFloaterRemoved)
			{
				placedItems = placedItems.Select(x => x.Item != Item.Floater ? x : NewItemPlacement(x, ReplacementItem)).ToList();
			}
			if ((bool)_flags.IsShipFree)
			{
				placedItems = placedItems.Select(x => x.Item != Item.Ship ? x : NewItemPlacement(x, ReplacementItem)).ToList();
			}
			if ((bool)_flags.IsCanalFree)
			{
				placedItems = placedItems.Select(x => x.Item != Item.Canal ? x : NewItemPlacement(x, ReplacementItem)).ToList();
			}
			if ((bool)_flags.FreeCanoe)
			{
				placedItems = placedItems.Select(x => x.Item != Item.Canoe ? x : NewItemPlacement(x, ReplacementItem)).ToList();
			}
			if ((bool)_flags.FreeLute)
			{
				placedItems = placedItems.Select(x => x.Item != Item.Lute ? x : NewItemPlacement(x, ReplacementItem)).ToList();
			}
			if ((bool)_flags.FreeTail || (bool)_flags.NoTail)
			{
				placedItems = placedItems.Select(x => x.Item != Item.Tail ? x : NewItemPlacement(x, ReplacementItem)).ToList();
			}

			//setup jingle for "incentive treasures", the placed items should just be key items, loose incentive items
			if (_flags.IncentiveChestItemsFanfare)
			{
				foreach (var placedItem in placedItems)
				{
					//dont make shards jingle that'd be annoying
					//dont make free items that get replaced, aka cabins, jingle
					if (placedItem is TreasureChest && placedItem.Item != Item.Shard && placedItem.Item != ReplacementItem)
					{
						rom.Put(placedItem.Address - FF1Rom.TreasureOffset + FF1Rom.TreasureJingleOffset, new byte[] { 0x01 });
					}
				}
			}

			if (_flags.Spoilers || Debugger.IsAttached)
			{
				// Output to the console only.
				Utilities.WriteSpoilerLine($"ItemPlacement::PlaceSaneItems required {_sanityCounter} iterations.", true);

				// Start of the item spoiler log output.
				Utilities.WriteSpoilerLine("Item     Entrance  ->  Floor  ->  Source                             Requirements");
				Utilities.WriteSpoilerLine("----------------------------------------------------------------------------------------------------");

				var sorted = placedItems.Where(item => item.Item != Item.Shard).ToList();
				sorted.Sort((IRewardSource lhs, IRewardSource rhs) => lhs.Item.ToString().CompareTo(rhs.Item.ToString()));
				sorted.ForEach(item =>
				{
					if (_overworldMap.FullLocationRequirements.TryGetValue(item.MapLocation, out var flr))
					{
						var overworldLocation = item.MapLocation.ToString();
						if (_overworldMap.OverriddenOverworldLocations != null && _overworldMap.OverriddenOverworldLocations.TryGetValue(item.MapLocation, out var overriden))
						{
							overworldLocation = overriden.ToString();
						}

						var itemStr = item.Item.ToString().PadRight(9);
						var locStr = $"{overworldLocation} -> {item.MapLocation} -> {item.Name} ".PadRight(60);
						var changes = $"({String.Join(" OR ", flr.Item1.Select(mapChange => mapChange.ToString()).ToArray())})";
						var reqs = flr.Item2.ToString().CompareTo("None") == 0 ? "" : $" AND {flr.Item2.ToString()}";
						Utilities.WriteSpoilerLine($"{itemStr}{locStr}{changes}{reqs}");
					}
				});
			}

			// 8. Place all remaining unincentivized treasures or incentivized non-quest items that weren't placed
			var itemLocationPool = _incentivesData.AllValidItemLocations.ToList();
			itemLocationPool = itemLocationPool.Where(x => !x.IsUnused && !placedItems.Any(y => y.Address == x.Address)).ToList();

			MoreConsumableChests.Work(_flags, treasurePool, rng);

			if((bool)_flags.NoXcalber)
			{
				//xcal can not be in the treasure pool due to forced item placements of fetch quest npc
				if(treasurePool.Contains(Item.Xcalber))
				{
					treasurePool.Remove(Item.Xcalber);
					treasurePool.Add(Item.Cabin);
				}
			}

			if ((bool)_flags.NoMasamune)
			{
				// Remove Masamune chest from shuffle
				treasurePool.Remove(Item.Masamune);
				treasurePool.Add(Item.Cabin);
			}
			else if((bool)_flags.GuaranteedMasamune)
			{
				// Remove Masamune chest from shuffle, Remove Cabin from item pool
				itemLocationPool = itemLocationPool.Where(x => !x.Equals(ItemLocations.ToFRMasmune)).ToList();
				treasurePool.Remove(Item.Cabin);

				// Send Masamune Home is ignored when Masamune is incentivized
				if (!incentivePool.Contains(Item.Masamune))
				{
					if ((bool)_flags.SendMasamuneHome)
					{
						// Remove Masamune from treasure pool (This will also causes Masamune to not be placed by RandomLoot)
						treasurePool.Remove(Item.Masamune);
						treasurePool.Add(Item.Cabin);
					}
				}
			}

			foreach (var placedItem in placedItems)
			{
				incentivePool.Remove(placedItem.Item);
			}
			treasurePool.AddRange(incentivePool);

			Debug.Assert(treasurePool.Count() == itemLocationPool.Count());

			if ((bool)_flags.RandomLoot)
			{
				// We want to leave out anything incentivized (and thus already placed), but
				// add all the other stuff that you can't find in vanilla.
				var randomTreasure = treasurePool.ToList();
				randomTreasure.AddRange(ItemLists.CommonWeaponTier);
				randomTreasure.AddRange(ItemLists.CommonArmorTier);
				randomTreasure.Add(Item.CatClaw);
				randomTreasure.Add(Item.SteelArmor);

				ItemGenerator generator = new ItemGenerator(randomTreasure, _flags.WorldWealth);
				treasurePool = treasurePool.Select(treasure => generator.GetItem(rng)).ToList();
			}

			if ((bool)_flags.BetterTrapChests)
			{
				// First we'll make a list of all 'notable' treasure.
				var notableTreasureList = new List<Item>()
					.Concat(ItemLists.UberTier)
					.Concat(ItemLists.LegendaryWeaponTier)
					.Concat(ItemLists.LegendaryArmorTier)
					.Concat(ItemLists.RareWeaponTier)
					.Concat(ItemLists.RareArmorTier);
				// Convert the list to a HashSet since we'll be doing lookups in it.
				var notableTreasure = new HashSet<Item>(notableTreasureList);

				// We sort the treasure pool based on value (sort of) and pull out the highest ranked ones to put
				// in the trap chests we picked out.
				var notableTreasurePool = treasurePool.Where(item => notableTreasure.Contains(item)).ToList();

				// Since some chests might be incentivized, remove those that aren't in the pool.
				var trapChestPool = TrapChests.Where(chest => itemLocationPool.Contains(chest));

				foreach (var chest in trapChestPool)
				{
					// It seems unlikely that is possible, but just in case.
					if (!notableTreasurePool.Any()) break;

					// Pick a random treasure and place it.
					var treasure = notableTreasurePool.SpliceRandom(rng);
					placedItems.Add(NewItemPlacement(chest, treasure));

					// Since it was placed, remove both the item and location from the remaining pool.
					treasurePool.Remove(treasure);
					itemLocationPool.Remove(chest);
				}

				// This should still be true at the end, so make sure it is
				Debug.Assert(treasurePool.Count() == itemLocationPool.Count());
			}

			treasurePool.Shuffle(rng);
			itemLocationPool.Shuffle(rng);

			var leftovers = treasurePool.Zip(itemLocationPool, (treasure, location) => NewItemPlacement(location, treasure));
			placedItems.AddRange(leftovers);
			return placedItems;
		}

		public Item SelectVendorItem(List<Item> incentives, List<Item> nonincentives, List<Item> treasurePool, List<IRewardSource> incentiveLocationPool, MT19337 rng)
		{
			if (!(bool)_flags.NPCItems) return Item.Bottle;

			var itemShopItem = Item.Cabin;
			var validShopIncentives = incentives.Where(x => x > Item.None && x <= Item.Soft).ToList();
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

			return itemShopItem;
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
	}

	public class RandomItemPlacement : ItemPlacement
	{
		protected override ItemPlacementResult DoSanePlacement(MT19337 rng, ItemPlacementContext ctx)
		{
			var itemLocationPool = _incentivesData.AllValidItemLocations.ToList();
			var incentiveLocationPool = _incentivesData.IncentiveLocations.ToList();
			var keyLocations = _incentivesData.KeyLocations.ToList();
			var canoeLocations = _incentivesData.CanoeLocations.ToList();
			var bridgeLocations = _incentivesData.BridgeLocations.ToList();
			var shipLocations = _incentivesData.ShipLocations.ToList();

			var canoeObsoletesBridge = (bool)_flags.MapCanalBridge && (bool)_flags.MapConeriaDwarves;
			var canoeObsoletesShip = (bool)_flags.MapCanalBridge ? ((bool)_flags.MapConeriaDwarves || (bool)_flags.MapVolcanoIceRiver) : ((bool)_flags.MapConeriaDwarves && (bool)_flags.MapVolcanoIceRiver);

			Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullLocationRequirements = _overworldMap.FullLocationRequirements;
			Dictionary<MapLocation, OverworldTeleportIndex> overridenOverworld = _overworldMap.OverriddenOverworldLocations;

			var startingPotentialAccess = _overworldMap.StartingPotentialAccess;
			var startingMapLocations = _checker.AccessibleMapLocations(startingPotentialAccess, MapChange.None, fullLocationRequirements);
			var earlyMapLocations = _checker.AccessibleMapLocations(startingPotentialAccess | AccessRequirement.Crystal, MapChange.Bridge, fullLocationRequirements);

			List<IRewardSource> placedItems = null;
			List<Item> treasurePool = null;
			var itemShopItem = Item.Bottle;

			_sanityCounter = 0;

			do
			{
				_sanityCounter++;
				if (_sanityCounter > 20) throw new InsaneException("RandomItemPlacement Failure!");
				// 1. (Re)Initialize lists inside of loop
				placedItems = ctx.Forced.ToList();
				var incentives = ctx.Incentivized.ToList();
				var nonincentives = ctx.Unincentivized.ToList();
				treasurePool = ctx.AllTreasures.ToList();
				incentives.Shuffle(rng);
				nonincentives.Shuffle(rng);

				if ((bool)_flags.NPCItems)
				{
					// 2. Place caravan item first because among incentive locations it has the smallest set of possible items
					itemShopItem = SelectVendorItem(incentives, nonincentives, treasurePool, incentiveLocationPool, rng);
					placedItems.Add(new ItemShopSlot(_caravanItemLocation, itemShopItem));

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

					if (!(startingCanoeAvailable && canoeObsoletesBridge) && !startingShipAvailable && !((bool)_flags.FreeBridge))
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
			} while (!_checker.CheckSanity(placedItems, fullLocationRequirements, _flags).Complete);

			return new ItemPlacementResult { PlacedItems = placedItems, RemainingTreasures = treasurePool };
		}
	}

	public class GuidedItemPlacement : ItemPlacement
	{
		protected override ItemPlacementResult DoSanePlacement(MT19337 rng, ItemPlacementContext ctx)
		{
			_sanityCounter = 0;
			var incentiveLocationPool = _incentivesData.IncentiveLocations.ToList();
			var preBlackOrbLocationPool = _incentivesData.AllValidPreBlackOrbItemLocations.ToList();
			var preBlackOrbUnincentivizedLocationPool = preBlackOrbLocationPool.Where(x => !incentiveLocationPool.Any(y => y.Address == x.Address)).ToList();
			if ((bool)_flags.LooseExcludePlacedDungeons)
				preBlackOrbUnincentivizedLocationPool = IncentivizedDungeons();

			Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullLocationRequirements = _overworldMap.FullLocationRequirements;
			Dictionary<MapLocation, OverworldTeleportIndex> overridenOverworld = _overworldMap.OverriddenOverworldLocations;

			List<IRewardSource> placedItems = null;
			List<Item> treasurePool = null;
			var itemShopItem = Item.Bottle;

			do
			{
				_sanityCounter++;
				if (_sanityCounter > 20) throw new InsaneException("Item Placement could not meet incentivization requirements!");
				// 1. (Re)Initialize lists inside of loop
				placedItems = ctx.Forced.ToList();
				var incentives = ctx.Incentivized.ToList();
				var nonincentives = ctx.Unincentivized.ToList();
				var shards = ctx.Shards.ToList();
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
					List<Item> fixedPlacements = new List<Item> { Item.Key, Item.Bridge, Item.Canoe };
					List<Item> nextPlacements = new List<Item> { Item.Ship, Item.Canal };
					List<Item> lastPlacements = new List<Item> { Item.Floater, Item.Lute, Item.Crown, Item.Crystal, Item.Herb, Item.Tnt, Item.Adamant,
						Item.Slab, Item.Ruby, Item.Rod, Item.Chime, Item.Tail, Item.Cube, Item.Bottle, Item.Oxyale };

					if ((bool)_flags.EarlierRuby) {
						nextPlacements.Add(Item.Ruby);
						lastPlacements.Remove(Item.Ruby);
					}
					if ((bool)_flags.IsFloaterRemoved) {
					    lastPlacements.Remove(Item.Floater);
					}

					nextPlacements.Shuffle(rng);
					lastPlacements.Shuffle(rng);
					var allPlacements = fixedPlacements.Concat(nextPlacements).Concat(lastPlacements);

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
							placedItems.Add(NewItemPlacement(rewardSources.PickRandom(rng), item));
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
				if ((bool)_flags.LooseExcludePlacedDungeons) {
				    looseLocationPool = preBlackOrbUnincentivizedLocationPool;
				} else {
				    looseLocationPool = preBlackOrbLocationPool;
				}
				var leftoverItemLocations = looseLocationPool.Where(x => !placedItems.Any(y => y.Address == x.Address)
									       && _checker.IsRewardSourceAccessible(x, fulfilledRequirements, accessibleMapLocations)).ToList();


				// Place leftover items before placing shards, because if LooseExcludePlacedDungeons is set,
				// and there are lots of incentive locations, leftoverItemLocations pool may be small.
				foreach (var leftoverItem in leftoverItems)
				{
					placedItems.Add(NewItemPlacement(leftoverItemLocations.SpliceRandom(rng), leftoverItem));
				}

				if (shards.Count > 0) {
				    // Place shards.  These are not affected by the LooseExcludePlacedDungeons flag.
				    leftoverItemLocations = preBlackOrbLocationPool.Where(x => !placedItems.Any(y => y.Address == x.Address)
											  && _checker.IsRewardSourceAccessible(x, fulfilledRequirements, accessibleMapLocations)).ToList();
				    foreach (var shard in shards)
				    {
					placedItems.Add(NewItemPlacement(leftoverItemLocations.SpliceRandom(rng), shard));
				    }
				}

				// 7. Check sanity and loop if needed
			} while (!_checker.CheckSanity(placedItems, fullLocationRequirements, _flags).Complete);

			return new ItemPlacementResult { PlacedItems = placedItems, RemainingTreasures = treasurePool };
		}

		private List<IRewardSource> IncentivizedDungeons()
		{
			var placedDungeons = new List<IRewardSource>();

			foreach (var incentiveLocation in _incentivesData.IncentiveLocations.ToList())
			{
				if(incentiveLocation.GetType().Equals(typeof(TreasureChest)))
					placedDungeons.AddRange(_incentivesData.AllValidPreBlackOrbItemLocations.Where(x => ItemLocations.MapLocationToOverworldLocations[x.MapLocation] == ItemLocations.MapLocationToOverworldLocations[incentiveLocation.MapLocation] && x.GetType().Equals(typeof(TreasureChest))));
			}

			return _incentivesData.AllValidPreBlackOrbItemLocations.Where(x => !placedDungeons.Contains(x)).ToList();
		}
	}
}
