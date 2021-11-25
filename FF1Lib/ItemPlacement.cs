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
			placement = flags.PredictivePlacement && checker is SanityCheckerV2 ? new PredictivePlacement() : new GuidedItemPlacement();

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

			if ((bool)_flags.GuaranteedRuseItem && !(_flags.NoItemMagic ?? false))
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
			if ((bool)_flags.IsFloaterRemoved)
			{
			    unincentivizedQuestItems.Remove(Item.Floater);
			    unincentivizedQuestItems.Add(ReplacementItem);
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

			if ((bool)_flags.IsBridgeFree || (_flags.OwMapExchange == OwMapExchanges.Desert))
			{
				placedItems = placedItems.Select(x => x.Item != Item.Bridge ? x : NewItemPlacement(x, ReplacementItem)).ToList();
			}
			if ((bool)_flags.IsShipFree)
			{
				placedItems = placedItems.Select(x => x.Item != Item.Ship ? x : NewItemPlacement(x, ReplacementItem)).ToList();
			}
			if ((bool)_flags.IsCanalFree)
			{
				placedItems = placedItems.Select(x => x.Item != Item.Canal ? x : NewItemPlacement(x, ReplacementItem)).ToList();
			}
			if ((bool)_flags.FreeCanoe || (_flags.OwMapExchange == OwMapExchanges.Desert))
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
					if (placedItem is TreasureChest && placedItem.Item != Item.Shard && placedItem.Item != ReplacementItem && !((bool)_flags.GuaranteedRuseItem && placedItem.Item == Item.PowerRod && !(_flags.NoItemMagic ?? false)))
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

		public Item SelectVendorItem(List<Item> incentives, List<Item> nonincentives, List<Item> treasurePool, IEnumerable<IRewardSource> incentiveLocationPool, MT19337 rng)
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

		protected List<IRewardSource> IncentivizedDungeons(List<IRewardSource> preBlackOrbUnincentivizedLocationPool)
		{
			var placedDungeons = new List<IRewardSource>();

			foreach (var incentiveLocation in _incentivesData.IncentiveLocations.ToList())
			{
				if (incentiveLocation.GetType().Equals(typeof(TreasureChest)))
					placedDungeons.AddRange(_incentivesData.AllValidPreBlackOrbItemLocations.Where(x => ItemLocations.MapLocationToOverworldLocations[x.MapLocation] == ItemLocations.MapLocationToOverworldLocations[incentiveLocation.MapLocation] && x.GetType().Equals(typeof(TreasureChest))));
			}

			return preBlackOrbUnincentivizedLocationPool.Where(x => !placedDungeons.Contains(x)).ToList();
		}
	}

	public enum KeyItemPlacementMode
	{
		[Description("Guided")]
		Vanilla,

		[Description("Predictive")]
		Predictive,

		[Description("Predictive Unsafe")]
		PredictiveUnsafe
	}

	public enum LoosePlacementMode
	{
		[Description("Chaotic")]
		Vanilla,

		[Description("Spread")]
		Spread,

		[Description("Forward")]
		Forward
	}
}
