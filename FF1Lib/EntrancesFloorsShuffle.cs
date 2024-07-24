using RomUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public partial class Teleporters
	{
		private List<string> _log;
		public int ConeriaTownEntranceItemShopIndex = 0;
		public void ShuffleEntrancesAndFloors(OverworldMap overworld, MT19337 rng, Flags flags)
		{
			if (((bool)flags.Entrances || (bool)flags.Floors || (bool)flags.Towns) &&
				((bool)flags.ChestsKeyItems) &&
				((bool)flags.NPCItems) &&
				flags.GameMode == GameModes.Standard)
			{
				// Yeah amazing coding, we'll fix it in post
			}
			else
			{
				return;
			}

			_log = new();

			// Since we're going to move all the entrances around, we're going to change the requirements
			// for just about everything. Most interestingly the Titan's Tunnel is going to connect totally
			// different overworld regions, so we need to setup new defaults for Titan's West and Sarda since
			// though the airship is the only way to those entrances. We will have to go back and calculate
			// the implications of the Titan's Tunnel later.

			// Initial dictionaries.
			// Anything removed from these will get shuffled, anything remaining is considered fixed in place.
			var placedMaps = VanillaOverworldTeleports.ToDictionary(x => x.Key, x => x.Value);
			var placedFloors = VanillaStandardTeleports.ToDictionary(x => x.Key, x => x.Value);
			var placedExits = new Dictionary<ExitTeleportIndex, Coordinate>();
			if ((bool)flags.Towns)
			{
				if ((bool)flags.IncludeConeria)
					placedMaps.Remove(OverworldTeleportIndex.Coneria);

				placedMaps.Remove(OverworldTeleportIndex.Pravoka);
				placedMaps.Remove(OverworldTeleportIndex.Elfland);
				placedMaps.Remove(OverworldTeleportIndex.Melmond);
				placedMaps.Remove(OverworldTeleportIndex.CrescentLake);
				placedMaps.Remove(OverworldTeleportIndex.Onrac);
				placedMaps.Remove(OverworldTeleportIndex.Gaia);
				placedMaps.Remove(OverworldTeleportIndex.Lefein);
			}
			if ((bool)flags.Entrances)
			{
				// Skip towns as they would be removed by the above.
				// Remove SeaShrine1 so Onrac could lead to anywhere.
				var keepers = VanillaOverworldTeleports.Where(x => x.Key >= OverworldTeleportIndex.Coneria && x.Key <= OverworldTeleportIndex.Lefein).Select(x => x.Key).ToList();

				if (!(bool)flags.EntrancesIncludesDeadEnds)
				{
					keepers.Add(OverworldTeleportIndex.Cardia1);
					keepers.Add(OverworldTeleportIndex.Cardia5);
				}

				if ((bool)flags.IsFloaterRemoved && !(bool)flags.IsAirshipFree)
				{
					if (!(bool)flags.MapBahamutCardiaDock)
					{
						keepers.Add(OverworldTeleportIndex.Cardia1);
						keepers.Add(OverworldTeleportIndex.BahamutCave1);
					}
					keepers.Add(OverworldTeleportIndex.Cardia2);
					keepers.Add(OverworldTeleportIndex.Cardia4);
					keepers.Add(OverworldTeleportIndex.Cardia5);
					keepers.Add(OverworldTeleportIndex.Cardia6);
					keepers.Add(OverworldTeleportIndex.TitansTunnelWest);
				}

				if (flags.GameMode == GameModes.Standard && flags.OwMapExchange != OwMapExchanges.None)
				{
					// Don't move Titan's tunnel on custom/procgen maps
					keepers.Add(OverworldTeleportIndex.TitansTunnelEast);
					keepers.Add(OverworldTeleportIndex.TitansTunnelWest);
				}

				placedMaps = placedMaps.Where(x => keepers.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
				placedFloors.Remove(TeleportIndex.SeaShrine1);
				overworld.FixUnusedDefaultBackdrops();
			}
			if ((bool)flags.Floors && (bool)flags.Entrances)
			{
				//placedFloors = new Dictionary<TeleportIndex, TeleportDestination>();
				placedFloors = IceCaveLoopTeleporters;
			};

			// All the finished destinations. We don't ever want to make anything new target one of these since some fixed map or floor already does.
			var placedDestinations = placedMaps.Values.Select(x => x.Destination).Concat(placedFloors.Values.Select(x => x.Destination)).ToList();

			// Grab a list of all non-unused overworld entrances, regardless of whether or not we've already placed them.
			var townEntrances = TownEntrances;
			var nonTownEntrances = Enum.GetValues(typeof(OverworldTeleportIndex)).Cast<OverworldTeleportIndex>().Where(x => !townEntrances.Contains(x) && x < OverworldTeleportIndex.Unused1).ToList();
			var shuffledOverworldCount = townEntrances.Count(x => !placedMaps.ContainsKey(x)) + nonTownEntrances.Count(x => !placedMaps.ContainsKey(x));

			// Grab a list of floors we haven't placed yet that can be shuffled. i.e. not including bottom of ice cave or ordeals.
			var towns = TownTeleports.Where(x => !placedDestinations.Contains(x.Destination)).Select(x => new TeleportDestination(x)).ToList();
			var topfloors = NonTownForcedTopFloors.Where(x => !placedDestinations.Contains(x.Destination)).Select(x => new TeleportDestination(x)).ToList();
			var subfloors = FreePlacementFloors.Where(x => !placedDestinations.Contains(x.Destination)).Select(x => new TeleportDestination(x)).ToList();
			var deadEnds = new List<TeleportDestination>();

			towns.Shuffle(rng);
			// Don't allow Lefein or Melmond to be the first town since we need an item shop.
			while (towns.Any() && (towns.First().Destination == MapLocation.Lefein || towns.First().Destination == MapLocation.Melmond))
			{
				towns.Add(towns.First());
				towns.RemoveAt(0);
			}

			topfloors.Shuffle(rng);
			subfloors.Shuffle(rng);

			// Set aside a number of dead ends to accomodate all the extra branches that come about from forking floors.
			// We need to ensure that at least this many dead ends come after the forks to avoid loose ends.
			int extraForks = subfloors.Where(x => x.Teleports.Count() > 1).SelectMany(x => x.Teleports.Skip(1)).Count();
			extraForks += subfloors.Any() ? 1 : 0;
			for (var i = 0; i < extraForks; i++)
			{
				var firstIndexOfDeadEnd = subfloors.TakeWhile(x => x.Teleports.Any()).Count();
				deadEnds.Add(subfloors[firstIndexOfDeadEnd]);
				subfloors.RemoveAt(firstIndexOfDeadEnd);
			}

			// Deep "castles" for now just allows a deep ToFR but with refactoring could include others.
			// Ordeals is a candidate but it would require map edits - it has an EXIT not a WARP due to its internal teleports.
			if ((bool)flags.DeepCastlesPossible && (bool)flags.AllowDeepCastles)
			{
				topfloors = topfloors.Where(floor => floor.Destination != OverworldTeleporters[OverworldTeleportIndex.TempleOfFiends1].Destination).ToList();
				deadEnds.Add(OverworldTeleporters[OverworldTeleportIndex.TempleOfFiends1]);
			}

			if ((bool)flags.DeepTownsPossible && (bool)flags.AllowDeepTowns)
			{
				// If we're shuffling Coneria in with the towns we keep one aside to put in the Coneria entrance.
				// The first element of Towns has an Inn, Item Shop, and Clinic, which we ensured above.
				List<TeleportDestination> startingTown = new List<TeleportDestination>();
				if ((bool)flags.IncludeConeria && !(bool)flags.AllowUnsafeStartArea)
				{
					startingTown.Add(towns.First());
					towns.RemoveAt(0);
				}

				subfloors.AddRange(towns);
				towns.Clear();
				towns.AddRange(startingTown);
			}

			// Shuffle again now that we've removed some to be placed at the end. Maybe unnecessary.
			subfloors.Shuffle(rng);
			deadEnds.Shuffle(rng);

			// This will be the initial dataset from which we attempt to create a workable overworld and dungeon floor shuffling.
			var destinations = towns.Concat(topfloors).Concat(subfloors).Concat(deadEnds).ToList();
			var sanity = 0;
			Dictionary<OverworldTeleportIndex, TeleportDestination> shuffled;
			Dictionary<TeleportIndex, TeleportDestination> shuffledFloors;
			Dictionary<ExitTeleportIndex, Coordinate> shuffledExits;
			do
			{
				sanity++;
				if (sanity > 100)
					throw new InsaneException("Overworld Map Shuffle sanity exceeds 100 iterations.");
				var i = 0; // overworld entrance destination counter
				var j = 0; // underworld floor destination counter

				// Analogues to all the placed dictionaries above. Everything "placed" will be copied here,
				// and everything else will be shuffled as it's moved into these dictionaries, before sanity.
				shuffled = new Dictionary<OverworldTeleportIndex, TeleportDestination>();
				shuffledFloors = new Dictionary<TeleportIndex, TeleportDestination>();
				shuffledExits = new Dictionary<ExitTeleportIndex, Coordinate>();
				var teleports = new List<TeleportIndex>();

				// Overworld to First Floor Shuffle Loop
				var shuffleTowns = townEntrances.ToList();
				var shuffleEntrances = nonTownEntrances.ToList();

				// We keep the town destinations at the front of that list, so if we want to match towns
				// with town entrances we just keep them at the front of shuffleEntrances too.
				if ((bool)flags.EntrancesMixedWithTowns)
				{
					// If we're keeping the start area safe, move the Coneria entrance to the start
					// of shuffleEntrances so that it always gets matched with the first, good town.
					bool removedConeria = false;
					if (!(bool)flags.AllowUnsafeStartArea)
					{
						removedConeria = shuffleTowns.Remove(OverworldTeleportIndex.Coneria);
						Debug.Assert(removedConeria);
					}

					shuffleEntrances = shuffleTowns.Concat(shuffleEntrances).ToList();
					shuffleEntrances.Shuffle(rng);

					if (removedConeria)
						shuffleEntrances.Insert(0, OverworldTeleportIndex.Coneria);
				}
				else
				{
					shuffleTowns.Shuffle(rng);
					shuffleEntrances.Shuffle(rng);
					shuffleEntrances = shuffleTowns.Concat(shuffleEntrances).ToList();
				}

				while (shuffleEntrances.Any())
				{
					// Grab the next overworld entrance at random and write it to the shuffled output.
					var owti = shuffleEntrances.First();
					shuffleEntrances.RemoveAt(0);

					var destination = placedMaps.ContainsKey(owti) ? placedMaps[owti] : destinations[i++];
					shuffled[owti] = destination;
					//UpdateOverworldOverride(destination.Destination, owti);

					if (destination.Exit != ExitTeleportIndex.None)
					{
						// Exiting floors like Fiend Orb Teleporters need to be updated to new OW coords.

						shuffledExits.Add(destination.Exit, OverworldCoordinates[owti]);
					}

					// If this destination has continuing teleports we loop and handle them now.
					teleports.AddRange(destination.Teleports);
					while (teleports.Any())
					{
						// Grab the next underworld floor teleporter at random and write it to the shuffled floor output.
						var teleport = teleports.SpliceRandom(rng);
						var floor = placedFloors.ContainsKey(teleport) ? placedFloors[teleport] : destinations[shuffledOverworldCount + j++];
						teleports.AddRange(floor.Teleports); // Keep looping until a dead end.
						shuffledFloors[teleport] = floor;
						//UpdateOverworldOverride(floor.Destination, owti);
						if (floor.Exit != ExitTeleportIndex.None)
						{
							// Exiting floors like Fiend Orb Teleporters need to be updated to new OW coords.
							shuffledExits.Add(floor.Exit, OverworldCoordinates[owti]);
						}
					}
				}
			} while (!CheckEntranceSanity(shuffled, (bool)flags.AllowUnsafeStartArea));

			// If the Coneria Entrance goes *directly* to a town, we make its index the one that
			// gets the guaranteed PURE and SOFT.
			switch (shuffled[OverworldTeleportIndex.Coneria].Destination)
			{
				case MapLocation.Pravoka: ConeriaTownEntranceItemShopIndex = 1; break;
				case MapLocation.Elfland: ConeriaTownEntranceItemShopIndex = 2; break;
				case MapLocation.CrescentLake: ConeriaTownEntranceItemShopIndex = 3; break;
				case MapLocation.Gaia: ConeriaTownEntranceItemShopIndex = 4; break; // Gaia before Onrac!
				case MapLocation.Onrac: ConeriaTownEntranceItemShopIndex = 5; break;
				default: ConeriaTownEntranceItemShopIndex = 0; break;
			}

			// Pretty print map data
			foreach (var map in shuffled.OrderBy(x => x.Key))
			{
				OverworldTeleporters[map.Key] = new TeleportDestination(map.Value);
				overworld.UpdatePalettes(map.Key, map.Value);

				_log.Add($"{map.Key.ToString().PadRight(30)}{map.Value.SpoilerText}");
				var teleports = map.Value.Teleports.ToList();

				while (teleports.Any())
				{
					var teleport = teleports.SpliceRandom(rng);
					var innerMap = shuffledFloors[teleport];
					teleports.AddRange(innerMap.Teleports);
					StandardMapTeleporters[teleport] = new TeleportDestination(innerMap);
					if (map.Key != OverworldTeleportIndex.None)
					{
						overworld.UpdatePalettes(map.Key, innerMap);
					}
					_log.Add($"\t{teleport.ToString().PadRight(30)}{innerMap.SpoilerText.Trim()} ({OverworldMap.OverworldToPalette[map.Key]} tint)");
				}
			}

			// Write Exit teleport coords back to the ROM
			foreach (var exit in shuffledExits)
			{
				ExitTeleporters[exit.Key] = new TeleportDestination(MapIndex.Cardia, exit.Value);
			}

			// Now it's time to update all the requirements for treasure sanity checking.
			var allTeleportLocations = shuffled.Select(x => x.Value.Destination).Concat(shuffledFloors.Select(x => x.Value.Destination)).Distinct().ToList();

			// Find out what two entrances the titan's tunnel now connects, and create new lists of MapLocations thusly connected.
			var titanEast = shuffled.Single(x => x.Value.Destination == MapLocation.TitansTunnelEast);
			var titanWest = shuffled.Single(x => x.Value.Destination == MapLocation.TitansTunnelWest);
			_log.Add($"{titanWest.Key} is TitansTunnelWest and connects to {titanEast.Key}");

			if (Debugger.IsAttached)
			{
				Dump();
			}

			// Put together a nice final mapping of MapLocations to requirements, in the shuffled map world.
			var standardMapLookup = StandardMapLocations;
			var destinationsByLocation = shuffled.ToDictionary(x => ItemLocations.OverworldToMapLocation[x.Key], x => x.Value);
			destinationsByLocation = destinationsByLocation
				.Concat(shuffledFloors.Select(x => new KeyValuePair<MapLocation, TeleportDestination>(standardMapLookup[x.Key], x.Value)))
				.ToDictionary(x => x.Key, x => x.Value);

			/* EXTREME LOGGING ///////////////////////////////////////////////////////////////////////////////////
			{

				Console.WriteLine("ALL TELEPORT LOCATIONS:");
				Console.WriteLine(String.Join(", ", allTeleportLocations.ToArray()));
				Console.WriteLine("DEFAULT REQS:");
				defaultRequirements.All(item =>
				{
					if (item.Value.TeleportLocation != null)
						Console.WriteLine($"{item.Key.ToString()} requires {item.Value.TeleportLocation}");
					return true;
				});
				Console.WriteLine("destination by location:");
				destinationsByLocation.All(item =>
				{
					Console.WriteLine($"{item.Key.ToString()} has destination {item.Value.Destination.ToString()}");
					return true;
				});
				Console.WriteLine("NEW REQS:");
				newRequirements.All(item =>
				{
					if (item.Value.TeleportLocation != null)
						Console.WriteLine($"{item.Key.ToString()} requires {item.Value.TeleportLocation}");
					return true;
				});
				Console.WriteLine("DICT REQS:");
				FloorLocationRequirements.All(item =>
				{
					Console.WriteLine($"{item.Key.ToString()} requires {item.Value.Item1} - {item.Value.Item2}");
					return true;
				});
			}
			/////////////////////////////////////////////////////////////////////////////////////////////////////*/
		}
		public bool CheckEntranceSanity(IEnumerable<KeyValuePair<OverworldTeleportIndex, TeleportDestination>> shuffledEntrances, bool allowDanger)
		{
			var starterLocation = shuffledEntrances.Any(x => StartingLocations.Contains(x.Key) && StarterDestinations.Contains(x.Value.Destination));
			var titansConnections =
				shuffledEntrances.Any(x => x.Value.Destination == MapLocation.TitansTunnelEast && ConnectedLocations.Contains(x.Key)) &&
				shuffledEntrances.Any(x => x.Value.Destination == MapLocation.TitansTunnelWest && ConnectedLocations.Contains(x.Key));

			bool isSafe(OverworldTeleportIndex owti)
			{
				return shuffledEntrances.Any(x => x.Key == owti && SafeLocations.Contains(x.Value.Destination));
			}

			int dangerCount = 6;
			if (isSafe(OverworldTeleportIndex.Coneria)) --dangerCount;
			if (isSafe(OverworldTeleportIndex.ConeriaCastle1)) --dangerCount;
			if (isSafe(OverworldTeleportIndex.TempleOfFiends1)) --dangerCount;
			if (isSafe(OverworldTeleportIndex.DwarfCave)) --dangerCount;
			if (isSafe(OverworldTeleportIndex.MatoyasCave)) --dangerCount;
			if (isSafe(OverworldTeleportIndex.Pravoka)) --dangerCount;

			return titansConnections && (allowDanger || (starterLocation && dangerCount <= 3));
		}

		public void Dump()
		{
			_log.ForEach(Console.WriteLine);
		}
		private static readonly List<MapLocation> StarterDestinations = new List<MapLocation> {
				MapLocation.TempleOfFiends1, MapLocation.Cardia6, MapLocation.Cardia4,
				MapLocation.Cardia2, MapLocation.MatoyasCave, MapLocation.DwarfCave,
				MapLocation.SeaShrineMermaids
			};
		private static readonly List<MapLocation> SafeLocations = new List<MapLocation> {
				MapLocation.IceCave3, MapLocation.GurguVolcano4, MapLocation.GurguVolcano5,
				MapLocation.SeaShrine5, MapLocation.SeaShrine6,
				MapLocation.BahamutCave1, MapLocation.BahamutCave2,
				MapLocation.ElflandCastle, MapLocation.NorthwestCastle, MapLocation.ConeriaCastle1,
				MapLocation.SardasCave, MapLocation.Cardia1, MapLocation.Cardia5,
				MapLocation.Coneria, MapLocation.Pravoka, MapLocation.Elfland, MapLocation.Melmond,
				MapLocation.CrescentLake, MapLocation.Gaia, MapLocation.Onrac, MapLocation.Lefein

			}.Concat(StarterDestinations).Distinct().ToList();
		private static readonly List<OverworldTeleportIndex> ConnectedLocations = new List<OverworldTeleportIndex> {
				OverworldTeleportIndex.ConeriaCastle1, OverworldTeleportIndex.Coneria, OverworldTeleportIndex.TempleOfFiends1,
				OverworldTeleportIndex.MatoyasCave, OverworldTeleportIndex.Pravoka,
				OverworldTeleportIndex.ElflandCastle, OverworldTeleportIndex.Elfland, OverworldTeleportIndex.NorthwestCastle, OverworldTeleportIndex.MarshCave1,
				OverworldTeleportIndex.Melmond, OverworldTeleportIndex.EarthCave1, OverworldTeleportIndex.TitansTunnelEast,
				OverworldTeleportIndex.SardasCave, OverworldTeleportIndex.TitansTunnelWest,
				OverworldTeleportIndex.Cardia1, OverworldTeleportIndex.BahamutCave1,
				OverworldTeleportIndex.GurguVolcano1, OverworldTeleportIndex.IceCave1,
				OverworldTeleportIndex.Onrac
			};
		private static readonly List<OverworldTeleportIndex> StartingLocations = new List<OverworldTeleportIndex> {
				OverworldTeleportIndex.Coneria, OverworldTeleportIndex.ConeriaCastle1, OverworldTeleportIndex.TempleOfFiends1
			};
	}
}
