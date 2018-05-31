using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RomUtilities;

namespace FF1Lib
{
	public struct MapEdit
	{
		public byte X { get; set; }
		public byte Y { get; set; }
		public byte Tile { get; set; }
	}

	public class OverworldMap
	{
		private readonly FF1Rom _rom;
		private readonly Dictionary<Palette, Blob> _palettes;

		private List<string> _log;

		public const int MapPaletteOffset = 0x2000;
		public const int MapPaletteSize = 48;
		public const int MapCount = 64;

		public OverworldMap(FF1Rom rom, IMapEditFlags flags, Dictionary<Palette, Blob> palettes)
		{
			_rom = rom;
			_palettes = palettes;
			_log = new List<string>();

			var mapLocationRequirements = ItemLocations.MapLocationRequirements.ToDictionary(x => x.Key, x => x.Value.ToList());
			var floorLocationRequirements = ItemLocations.MapLocationFloorRequirements.ToDictionary(x => x.Key, x => x.Value);

			if (flags.MapOnracDock)
			{
				MapEditsToApply.Add(OnracDock);
				mapLocationRequirements[MapLocation.Onrac].Add(MapChange.Ship | MapChange.Canal);
				mapLocationRequirements[MapLocation.Caravan].Add(MapChange.Ship | MapChange.Canal | MapChange.Canoe);
				mapLocationRequirements[MapLocation.Waterfall].Add(MapChange.Ship | MapChange.Canal | MapChange.Canoe);
			}
			if (flags.MapMirageDock)
			{
				MapEditsToApply.Add(MirageDock);
				mapLocationRequirements[MapLocation.MirageTower1].Add(MapChange.Ship | MapChange.Canal | MapChange.Chime);
				//mapLocationRequirements[MapLocation.Lefein].Add(MapChange.Ship | MapChange.Canal | MapChange.Chime);
			}
			if (flags.MapVolcanoIceRiver)
			{
				MapEditsToApply.Add(VolcanoIceRiver);
				mapLocationRequirements[MapLocation.GurguVolcano1].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.CrescentLake].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.Elfland].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.ElflandCastle].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.NorthwestCastle].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.MarshCave1].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.AirshipLocation].Add(MapChange.Bridge | MapChange.Canoe);
				if (flags.MapCanalBridge)
					mapLocationRequirements[MapLocation.DwarfCave].Add(MapChange.Bridge | MapChange.Canoe);
			}
			if (flags.MapConeriaDwarves)
			{
				MapEditsToApply.Add(ConeriaToDwarves);
				mapLocationRequirements[MapLocation.DwarfCave].Add(MapChange.None);
				if (flags.MapCanalBridge)
				{
					mapLocationRequirements[MapLocation.GurguVolcano1].Add(MapChange.Canoe);
					mapLocationRequirements[MapLocation.CrescentLake].Add(MapChange.Canoe);
					mapLocationRequirements[MapLocation.Elfland].Add(MapChange.Canoe);
					mapLocationRequirements[MapLocation.ElflandCastle].Add(MapChange.Canoe);
					mapLocationRequirements[MapLocation.NorthwestCastle].Add(MapChange.Canoe);
					mapLocationRequirements[MapLocation.MarshCave1].Add(MapChange.Canoe);
					mapLocationRequirements[MapLocation.AirshipLocation].Add(MapChange.Canoe);
					if (flags.MapVolcanoIceRiver)
					{
						mapLocationRequirements[MapLocation.IceCave1].Add(MapChange.Canoe);
						mapLocationRequirements[MapLocation.Pravoka].Add(MapChange.Canoe);
						mapLocationRequirements[MapLocation.MatoyasCave].Add(MapChange.Canoe);
					}
				}
			}

			if (flags.TitansTrove)
			{
				mapLocationRequirements[MapLocation.TitansTunnelWest] = new List<MapChange> {
					MapChange.Canal | MapChange.Ship | MapChange.TitanFed, MapChange.Airship | MapChange.TitanFed };
			}

			if (flags.CrownlessOrdeals)
			{
				floorLocationRequirements[MapLocation.CastleOrdealsMaze] = new Tuple<MapLocation, AccessRequirement>(MapLocation.CastleOrdeals1, AccessRequirement.None);
			}

			MapLocationRequirements = mapLocationRequirements;
			FloorLocationRequirements = floorLocationRequirements;
			UpdateFullLocationRequirements();
		}

		const int teleportEntranceXOffset = 0x2C00;
		const int teleportEntranceYOffset = 0x2C20;
		const int teleportEntranceMapIndexOffset = 0x2C40;
		const int teleportExitXOffset = 0x2C60;
		const int teleportExitYOffset = 0x2C70;

		const int teleportTilesetOffset = 0x2CC0;

		const int teleportXOffset = 0x2D00;
		const int teleportYOffset = 0x2D40;
		const int teleportMapIndexOffset = 0x2D80;

		public static Dictionary<Palette, Blob> GeneratePalettes(List<Blob> vanillaPalettes)
		{
			Dictionary<Palette, Blob> palettes = new Dictionary<Palette, Blob>();
			palettes.Add(Palette.Town, vanillaPalettes[(int)MapIndex.ConeriaTown]);
			palettes.Add(Palette.Castle, vanillaPalettes[(int)MapIndex.ConeriaCastle1F]);
			palettes.Add(Palette.Greyscale, vanillaPalettes[(int)MapIndex.TempleOfFiends]);
			palettes.Add(Palette.Orange, vanillaPalettes[(int)MapIndex.EarthCaveB1]);
			palettes.Add(Palette.DarkOrange, vanillaPalettes[(int)MapIndex.EarthCaveB4]);
			palettes.Add(Palette.PaleRed, vanillaPalettes[(int)MapIndex.GurguVolcanoB1]);
			palettes.Add(Palette.Red, vanillaPalettes[(int)MapIndex.GurguVolcanoB4]);
			palettes.Add(Palette.PaleBlue, vanillaPalettes[(int)MapIndex.IceCaveB1]);
			palettes.Add(Palette.Teal, vanillaPalettes[(int)MapIndex.Cardia]);
			palettes.Add(Palette.DarkTeal, vanillaPalettes[(int)MapIndex.Waterfall]);
			palettes.Add(Palette.Purple, vanillaPalettes[(int)MapIndex.MatoyasCave]);
			palettes.Add(Palette.Yellow, vanillaPalettes[(int)MapIndex.SardasCave]);
			palettes.Add(Palette.PaleGreen, vanillaPalettes[(int)MapIndex.MarshCaveB1]);
			palettes.Add(Palette.Green, vanillaPalettes[(int)MapIndex.MarshCaveB3]);
			palettes.Add(Palette.Blue, vanillaPalettes[(int)MapIndex.SeaShrineB5]);
			palettes.Add(Palette.LightBlue, vanillaPalettes[(int)MapIndex.SeaShrineB1]);
			palettes.Add(Palette.DarkBlue, vanillaPalettes[(int)MapIndex.TitansTunnel]);
			palettes.Add(Palette.Bluescale, vanillaPalettes[(int)MapIndex.SkyPalace1F]);

			palettes.Add(Palette.YellowGreen, Blob.Concat(
				Blob.FromHex("0F3838380F3838180F0A19290F000130"),
				vanillaPalettes[(int)MapIndex.BahamutCaveB2].SubBlob(16, 16),
				Blob.FromHex("0F10170F0F0F18080F0F0B180F000130")
			));
			palettes.Add(Palette.Pink, Blob.Concat(
				Blob.FromHex("0F2424240F2424140F0414280F000130"),
				vanillaPalettes[(int)MapIndex.MarshCaveB2].SubBlob(16, 16),
				Blob.FromHex("0F10000F0F0F14250F0F04080F000130")
			));
			palettes.Add(Palette.Flame, Blob.Concat(
				Blob.FromHex("0F3838380F3838180F0616280F000130"),
				vanillaPalettes[(int)MapIndex.MirageTower2F].SubBlob(16, 16),
				Blob.FromHex("0F10170F0F0F07180F0F06180F000130")
			));

			return palettes;
		}

		public void PutPalette(OverworldTeleportIndex source, TeleportDestination destination)
		{
			if (!destination.OwnsPalette)
			{
				return;
			}

			var palette = OverworldToPalette[source];

			if (destination.Index <= MapIndex.Lefein) // Towns
			{
				// Towns are given arbitrary palettes to help provide color when dungeons take their place.
				// But if a town ends up in place of another town, the default palette is appropriate.
				if (source < OverworldTeleportIndex.Coneria || source > OverworldTeleportIndex.Lefein)
				{
					Console.WriteLine($"Tinting {destination.Index} {palette}.");
					_rom.Put(MapPaletteOffset + (int)destination.Index * MapPaletteSize + 1, _palettes[palette].SubBlob(9, 2));
					_rom.Put(MapPaletteOffset + (int)destination.Index * MapPaletteSize + 6, _palettes[palette].SubBlob(9, 1));
					_rom.Put(MapPaletteOffset + (int)destination.Index * MapPaletteSize + 33, _palettes[palette].SubBlob(9, 2));
					_rom.Put(MapPaletteOffset + (int)destination.Index * MapPaletteSize + 38, _palettes[palette].SubBlob(9, 1));
				}
			}
			else if (destination.Index < MapIndex.EarthCaveB1) // Castles - just tint the lawns.
			{
				_rom.Put(MapPaletteOffset + (int)destination.Index * MapPaletteSize + 8, _palettes[palette].SubBlob(8, 8));
				_rom.Put(MapPaletteOffset + (int)destination.Index * MapPaletteSize + 40, _palettes[palette].SubBlob(40, 8));
			}
			else // Dungeons
			{
				_rom.Put(MapPaletteOffset + (int)destination.Index * MapPaletteSize, _palettes[palette].SubBlob(0, 16));

				// Some maps have greyscale objects (Chests / Pillars) that look wrong when tinted (usually brown).
				var index = FixedObjectPaletteDestinations.Contains(destination.Index) ? 36 : 32;
				_rom.Put(MapPaletteOffset + (int)destination.Index * MapPaletteSize + index, _palettes[palette].SubBlob(index, 48 - index));
			}
		}

		public void PutOverworldTeleport(OverworldTeleportIndex index, TeleportDestination teleport)
		{
			_rom[teleportEntranceXOffset + (byte)index] = teleport.CoordinateX;
			_rom[teleportEntranceYOffset + (byte)index] = teleport.CoordinateY;
			_rom[teleportEntranceMapIndexOffset + (byte)index] = (byte)teleport.Index;

			PutPalette(index, teleport);
		}

		public void PutStandardTeleport(TeleportIndex index, TeleportDestination teleport, OverworldTeleportIndex overworldEntryPoint)
		{
			_rom[teleportXOffset + (byte)index] = teleport.CoordinateX;
			_rom[teleportYOffset + (byte)index] = teleport.CoordinateY;
			_rom[teleportMapIndexOffset + (byte)index] = (byte)teleport.Index;

			PutPalette(overworldEntryPoint, teleport);
		}

		public void ShuffleEntrancesAndFloors(MT19337 rng, IFloorShuffleFlags flags)
		{
			// Disable the Princess Warp back to Castle Coneria
			_rom.Put(0x392CA, Blob.FromHex("EAEAEA"));

			// Since we're going to move all the entrances around, we're going to change the requirements
			// for just about everything. Most interestingly the Titan's Tunnel is going to connect totally
			// different overworld regions, so we need to setup new defaults for Titan's West and Sarda since
			// though the airship is the only way to those entrances. We will have to go back and calculate
			// the implications of the Titan's Tunnel later.
			var defaultRequirements = MapLocationRequirements
				.ToDictionary(x => x.Key, x => new LocationRequirement(x.Value))
				.Concat(FloorLocationRequirements.ToDictionary(x => x.Key, x => new LocationRequirement(x.Value)))
				.ToDictionary(x => x.Key, x => x.Value);
			defaultRequirements[MapLocation.SardasCave] = new LocationRequirement(new List<MapChange> { MapChange.Airship });
			defaultRequirements[MapLocation.TitansTunnelWest] = new LocationRequirement(new List<MapChange> { MapChange.Airship });

			// Initial dictionaries.
			// Anything removed from these will get shuffled, anything remaining is considered fixed in place.
			var placedMaps = TeleportShuffle.VanillaOverworldTeleports.ToDictionary(x => x.Key, x => x.Value);
			var placedFloors = TeleportShuffle.VanillaStandardTeleports.ToDictionary(x => x.Key, x => x.Value);
			var placedExits = new Dictionary<ExitTeleportIndex, Coordinate>();
			if (flags.Towns)
			{
				// Conspicuously missing is Coneria; we do not shuffle it .... yet.
				placedMaps.Remove(OverworldTeleportIndex.Pravoka);
				placedMaps.Remove(OverworldTeleportIndex.Elfland);
				placedMaps.Remove(OverworldTeleportIndex.Melmond);
				placedMaps.Remove(OverworldTeleportIndex.CrescentLake);
				placedMaps.Remove(OverworldTeleportIndex.Onrac);
				placedMaps.Remove(OverworldTeleportIndex.Gaia);
				placedMaps.Remove(OverworldTeleportIndex.Lefein);
			}
			if (flags.Entrances)
			{
				// Skip towns as they would be removed by the above.
				// Remove SeaShrine1 so Onrac could lead to anywhere.
				placedMaps = placedMaps
					.Where(x => x.Key >= OverworldTeleportIndex.Coneria && x.Key <= OverworldTeleportIndex.Lefein)
					.ToDictionary(x => x.Key, x => x.Value);
				placedFloors.Remove(TeleportIndex.SeaShrine1);

				FixUnusedDefaultBackdrops();
			}
			if (flags.Floors)
			{
				placedFloors = new Dictionary<TeleportIndex, TeleportDestination>();
			}

			// All the finished destinations. We don't ever want to make anything new target one of these since some fixed map or floor already does.
			var placedDestinations = placedMaps.Values.Select(x => x.Destination).Concat(placedFloors.Values.Select(x => x.Destination)).ToList();

			// Grab a list of all non-unused overworld entrances, regardless of whether or not we've already placed them.
			var maps = Enum.GetValues(typeof(OverworldTeleportIndex)).Cast<OverworldTeleportIndex>().Where(x => x < OverworldTeleportIndex.Unused1).ToList();
			var shuffledOverworldCount = maps.Count(x => !placedMaps.ContainsKey(x));

			// Grab a list of floors we haven't placed yet that can be shuffled. i.e. not including bottom of ice cave or ordeals.
			var destinations = TeleportShuffle.FreePlacementFloors.Where(x => !placedDestinations.Contains(x.Destination)).ToList();

			// Set aside a number of dead ends to accomodate all the extra branches that come about from forking floors.
			// We need to ensure that at least this many dead ends come after the forks to avoid loose ends.
			var extraForks = destinations.Where(x => x.Teleports.Count() > 1).SelectMany(x => x.Teleports.Skip(1)).Count();
			if (destinations.Any()) extraForks++;
			var minimumDeadEndsAtEnd = new List<TeleportDestination>();
			for (var i = 0; i < extraForks; i++)
			{
				var firstIndexOfDeadEnd = destinations.TakeWhile(x => x.Teleports.Any()).Count();
				minimumDeadEndsAtEnd.Add(destinations[firstIndexOfDeadEnd]);
				destinations.RemoveAt(firstIndexOfDeadEnd);
			}

			// Shuffle the destinations and keep the ones that aren't already placed, and then bolt-on the held over dead ends from above.
			// This will be the initial dataset from which we attempt to create a workable overworld and dungeon floor shuffling.
			destinations.Shuffle(rng);
			destinations = TeleportShuffle.ForcedTopFloors.Where(x => !placedDestinations.Contains(x.Destination)).Concat(destinations).Concat(minimumDeadEndsAtEnd).ToList();
			var sanity = 0;
			Dictionary<OverworldTeleportIndex, TeleportDestination> shuffled;
			Dictionary<TeleportIndex, TeleportDestination> shuffledFloors;
			Dictionary<ExitTeleportIndex, Coordinate> shuffledExits;
			do
			{
				sanity++;
				if (sanity > 500)
					throw new InsaneException();
				var i = 0; // overworld entrance destination counter
				var j = 0; // underworld floor destination counter

				// Analogues to all the placed dictionaries above. Everything "placed" will be copied here,
				// and everything else will be shuffled as it's moved into these dictionaries, before sanity.
				shuffled = new Dictionary<OverworldTeleportIndex, TeleportDestination>();
				shuffledFloors = new Dictionary<TeleportIndex, TeleportDestination>();
				shuffledExits = new Dictionary<ExitTeleportIndex, Coordinate>();
				var teleports = new List<TeleportIndex>();

				// Main Floor Shuffle Loop
				var shuffleMaps = maps.ToList();
				while (shuffleMaps.Any())
				{
					// Grab the next overworld entrance at random and write it to the shuffled output.
					var owti = shuffleMaps.SpliceRandom(rng);
					var destination = placedMaps.ContainsKey(owti) ? placedMaps[owti] : destinations[i++];
					shuffled[owti] = destination;

					if (destination.Exit != ExitTeleportIndex.None)
					{
						// Exiting floors like Fiend Orb Teleporters need to be updated to new OW coords.
						shuffledExits.Add(destination.Exit, TeleportShuffle.OverworldCoordinates[owti]);
					}

					// If this destination has continuting teleports we recurse the handle them now.
					teleports.AddRange(destination.Teleports);
					while (teleports.Any())
					{
						// Grab the next underworld floor teleporter at random and write it to the shuffled floor output.
						var teleport = teleports.SpliceRandom(rng);
						var floor = placedFloors.ContainsKey(teleport) ? placedFloors[teleport] : destinations[shuffledOverworldCount + j++];
						teleports.AddRange(floor.Teleports); // Keep looping until a dead end.
						shuffledFloors[teleport] = floor;
						if (floor.Exit != ExitTeleportIndex.None)
						{
							// Exiting floors like Fiend Orb Teleporters need to be updated to new OW coords.
							shuffledExits.Add(floor.Exit, TeleportShuffle.OverworldCoordinates[owti]);
						}
					}
				}
			} while (!CheckEntranceSanity(shuffled, flags.AllowStartAreaDanager));

			// Pretty print map data
			foreach (var map in shuffled.OrderBy(x => x.Key))
			{
				PutOverworldTeleport(map.Key, map.Value);
				var name = Enum.GetName(typeof(OverworldTeleportIndex), map.Key);
				name += string.Join("", Enumerable.Repeat(" ", Math.Max(1, 30 - name.Length)).ToList());
				_log.Add($"{name}{map.Value.SpoilerText}");
				var teleports = map.Value.Teleports.ToList();

				while (teleports.Any())
				{
					var teleport = teleports.SpliceRandom(rng);
					var innerMap = shuffledFloors[teleport];
					teleports.AddRange(innerMap.Teleports);
					PutStandardTeleport(teleport, innerMap, map.Key);
					var innerName = Enum.GetName(typeof(TeleportIndex), teleport);
					innerName += string.Join("", Enumerable.Repeat(" ", Math.Max(1, 30 - innerName.Length)).ToList());
					_log.Add($"\t{innerName}{innerMap.SpoilerText.Trim()} ({Enum.GetName(typeof(Palette), OverworldToPalette[map.Key])} tint)");
				}
			}

			// Write Exit teleport coords back to the ROM
			foreach (var exit in shuffledExits)
			{
				_rom[teleportExitXOffset + (byte)exit.Key] = exit.Value.X;
				_rom[teleportExitYOffset + (byte)exit.Key] = exit.Value.Y;
			}

			// Now it's time to update all the requirements for treasure sanity checking.
			// Grab a list of all distinct destinations, and also create a couple lists of lists -
			// All the contiguous walkable regions, and all the contiguous canoeable regions. Perhaps this belongs in another file?
			var allTeleportLocations = shuffled.Select(x => x.Value.Destination).Concat(shuffledFloors.Select(x => x.Value.Destination)).Distinct().ToList();
			var walkableNodes = new List<List<OverworldTeleportIndex>> {
				new List<OverworldTeleportIndex>{OverworldTeleportIndex.ConeriaCastle1, OverworldTeleportIndex.Coneria, OverworldTeleportIndex.TempleOfFiends1},
				new List<OverworldTeleportIndex>{OverworldTeleportIndex.MatoyasCave, OverworldTeleportIndex.Pravoka},
				new List<OverworldTeleportIndex>{OverworldTeleportIndex.ElflandCastle, OverworldTeleportIndex.Elfland, OverworldTeleportIndex.NorthwestCastle, OverworldTeleportIndex.MarshCave1},
				new List<OverworldTeleportIndex>{OverworldTeleportIndex.Melmond, OverworldTeleportIndex.EarthCave1, OverworldTeleportIndex.TitansTunnelEast},
				new List<OverworldTeleportIndex>{OverworldTeleportIndex.SardasCave, OverworldTeleportIndex.TitansTunnelWest},
				new List<OverworldTeleportIndex>{OverworldTeleportIndex.Cardia1, OverworldTeleportIndex.BahamutCave1}
			};
			var canoeableNodes = new List<List<OverworldTeleportIndex>> {
				new List<OverworldTeleportIndex>{OverworldTeleportIndex.MatoyasCave, OverworldTeleportIndex.Pravoka, OverworldTeleportIndex.IceCave1},
				new List<OverworldTeleportIndex>{
				OverworldTeleportIndex.ElflandCastle, OverworldTeleportIndex.Elfland, OverworldTeleportIndex.NorthwestCastle, OverworldTeleportIndex.MarshCave1,
				OverworldTeleportIndex.CrescentLake, OverworldTeleportIndex.GurguVolcano1, OverworldTeleportIndex.DwarfCave},
				new List<OverworldTeleportIndex>{OverworldTeleportIndex.Onrac, OverworldTeleportIndex.Waterfall}
			};

			// Find out what two entrances the titan's tunnel now connects, and create new lists of MapLocations thusly connected.
			var titanEast = shuffled.Single(x => x.Value.Destination == MapLocation.TitansTunnelEast);
			var titanWest = shuffled.Single(x => x.Value.Destination == MapLocation.TitansTunnelWest);
			var titanWalkLocations = walkableNodes.Where(x => x.Contains(titanEast.Key) || x.Contains(titanWest.Key)).SelectMany(x => x).Distinct().Select(x => shuffled[x].Destination);
			var titanCanoeLocations = canoeableNodes.Where(x => x.Contains(titanEast.Key) || x.Contains(titanWest.Key)).SelectMany(x => x).Distinct().Select(x => shuffled[x].Destination);

			// Put together a nice final mapping of MapLocations to requirements, in the shuffled map world.
			var standardMapLookup = TeleportShuffle.StandardMapLocations;
			var destinationsByLocation = shuffled.ToDictionary(x => ItemLocations.OverworldToMapLocation[x.Key], x => x.Value);
			destinationsByLocation = destinationsByLocation
				.Concat(shuffledFloors.Select(x => new KeyValuePair<MapLocation, TeleportDestination>(standardMapLookup[x.Key], x.Value)))
				.ToDictionary(x => x.Key, x => x.Value);
			var newRequirements = defaultRequirements
				.ToDictionary(x => !allTeleportLocations.Contains(x.Key) ? x.Key :
							  destinationsByLocation[x.Key].Destination,
							  x => x.Value);

			// Segregate out the simple mapping into Entrances and Floors
			MapLocationRequirements = newRequirements.Where(x => x.Value.MapChanges != null).ToDictionary(x => x.Key, x => x.Value.MapChanges.ToList());
			FloorLocationRequirements = newRequirements.Where(x => x.Value.MapChanges == null).ToDictionary(x => x.Key, x => x.Value.TeleportLocation);

			if (flags.Verbose)
			{
				dump();

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

			// Titan's Tunnel adjustments. The Titan Fed requirement implies access to one side of the tunnel,
			// so it is sufficient to say feeding the titan will grant access to all walkable and canoeable nodes
			// from either entrance.
			foreach (var key in titanWalkLocations)
			{
				MapLocationRequirements[key].Add(MapChange.TitanFed);
			}
			foreach (var key in titanCanoeLocations)
			{
				MapLocationRequirements[key].Add(MapChange.TitanFed | MapChange.Canoe);
			}

			UpdateFullLocationRequirements();
		}

		private void UpdateFullLocationRequirements()
		{
			// Flatten into one Dictionary
			FullLocationRequirements = new Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>>();
			foreach (var mlr in MapLocationRequirements)
			{
				FullLocationRequirements.Add(mlr.Key, new Tuple<List<MapChange>, AccessRequirement>(mlr.Value, AccessRequirement.None));
			}

			foreach (var flr in FloorLocationRequirements)
			{
				List<MapLocation> cycleFinder = new List<MapLocation>();
				AccessRequirement requirements = AccessRequirement.None;

				MapLocation location = flr.Key;
				while (FloorLocationRequirements.TryGetValue(location, out var floorRequirement))
				{
					if (cycleFinder.Contains(location))
					{
						Console.WriteLine("Floor requirement cycle???");
						throw new InsaneException();
					}

					cycleFinder.Add(location);
					requirements |= floorRequirement.Item2;
					location = floorRequirement.Item1;
				}
				var found = MapLocationRequirements.TryGetValue(location, out var overworldRequirements);
				if (found)
				{
					FullLocationRequirements.Add(flr.Key, new Tuple<List<MapChange>, AccessRequirement>(overworldRequirements, requirements));
				}
				else
				{
					Console.WriteLine("Floors missing????");
					throw new InsaneException();
				}
			}
		}

		private void FixUnusedDefaultBackdrops()
		{
			Blob backdrops = _rom.Get(0x03300, 128);
			backdrops[0x2F] = (byte)Backdrop.EarthCave; // Dwarf Cave
			backdrops[0x32] = (byte)Backdrop.SeaShrine; // Matoya's Cave
			backdrops[0x3A] = (byte)Backdrop.MarshCave; // Sarda's Cave
			backdrops[0x4A] = (byte)Backdrop.IceCave; // Pravoka
			backdrops[0x4C] = (byte)Backdrop.Forest; // Elfland
			backdrops[0x4D] = (byte)Backdrop.Desert; // Melmond
			backdrops[0x4E] = (byte)Backdrop.River; // Crescent Lake
			backdrops[0x5A] = (byte)Backdrop.EarthCave; // Gaia
			backdrops[0x6C] = (byte)Backdrop.Waterfall; // Bahamut's Room
			backdrops[0x6D] = (byte)Backdrop.Tower; // Lefein
			_rom.Put(0x03300, backdrops);
		}

		public bool CheckEntranceSanity(IEnumerable<KeyValuePair<OverworldTeleportIndex, TeleportDestination>> shuffledEntrances,
										bool allowUnsafe = false)
		{
			var starterDestinations = new List<MapLocation> {
				MapLocation.TempleOfFiends1, MapLocation.Cardia6, MapLocation.Cardia4,
				MapLocation.Cardia2, MapLocation.MatoyasCave, MapLocation.DwarfCave,
				MapLocation.SeaShrineMermaids
			};
			var townsWithShops = new List<MapLocation> {
				MapLocation.Coneria
				//, MapLocation.Pravoka, MapLocation.Elfland, MapLocation.CresentLake, MapLocation.Gaia
			};
			var safeLocations = new List<MapLocation> {
				MapLocation.IceCave3, MapLocation.GurguVolcano4, MapLocation.GurguVolcano5,
				MapLocation.SeaShrine5, MapLocation.SeaShrine6,
				MapLocation.BahamutCave1, MapLocation.BahamutCave2,
				MapLocation.ElflandCastle, MapLocation.NorthwestCastle, MapLocation.ConeriaCastle1,
				MapLocation.SardasCave, MapLocation.Cardia1, MapLocation.Cardia5,
				MapLocation.Pravoka, MapLocation.Elfland, MapLocation.Melmond,
				MapLocation.CrescentLake, MapLocation.Gaia, MapLocation.Onrac, MapLocation.Lefein

			}.Concat(starterDestinations).Concat(townsWithShops).ToList();
			var connectedLocations = new List<OverworldTeleportIndex> {
				OverworldTeleportIndex.ConeriaCastle1, OverworldTeleportIndex.Coneria, OverworldTeleportIndex.TempleOfFiends1,
				OverworldTeleportIndex.MatoyasCave, OverworldTeleportIndex.Pravoka,
				OverworldTeleportIndex.ElflandCastle, OverworldTeleportIndex.Elfland, OverworldTeleportIndex.NorthwestCastle, OverworldTeleportIndex.MarshCave1,
				OverworldTeleportIndex.Melmond, OverworldTeleportIndex.EarthCave1, OverworldTeleportIndex.TitansTunnelEast,
				OverworldTeleportIndex.SardasCave, OverworldTeleportIndex.TitansTunnelWest,
				OverworldTeleportIndex.Cardia1, OverworldTeleportIndex.BahamutCave1,
				OverworldTeleportIndex.GurguVolcano1, OverworldTeleportIndex.IceCave1,
				OverworldTeleportIndex.Onrac
			};
			var startingLocations = new List<OverworldTeleportIndex> {
				OverworldTeleportIndex.ConeriaCastle1, OverworldTeleportIndex.TempleOfFiends1
			};
			var townStart =
				shuffledEntrances.Any(x => x.Key == OverworldTeleportIndex.Coneria && townsWithShops.Contains(x.Value.Destination));
			var starterLocation =
				shuffledEntrances.Any(x => startingLocations.Contains(x.Key) && starterDestinations.Contains(x.Value.Destination));
			var dangerLocationAtConeriaCastle =
				!shuffledEntrances.Any(x => x.Key == OverworldTeleportIndex.ConeriaCastle1 && safeLocations.Contains(x.Value.Destination));
			var dangerLocationAtToF =
				!shuffledEntrances.Any(x => x.Key == OverworldTeleportIndex.TempleOfFiends1 && safeLocations.Contains(x.Value.Destination));
			var dangerLocationAtDwarf =
				!shuffledEntrances.Any(x => x.Key == OverworldTeleportIndex.DwarfCave && safeLocations.Contains(x.Value.Destination));
			var dangerLocationAtMatoya =
				!shuffledEntrances.Any(x => x.Key == OverworldTeleportIndex.MatoyasCave && safeLocations.Contains(x.Value.Destination));
			var titansConnections =
				shuffledEntrances.Any(x => x.Value.Destination == MapLocation.TitansTunnelEast && connectedLocations.Contains(x.Key)) &&
				shuffledEntrances.Any(x => x.Value.Destination == MapLocation.TitansTunnelWest && connectedLocations.Contains(x.Key));
			var dangerDanger = dangerLocationAtConeriaCastle || dangerLocationAtToF || dangerLocationAtDwarf || dangerLocationAtMatoya;

			return townStart && starterLocation && titansConnections && (allowUnsafe || !dangerDanger);
		}

		public void dump()
		{
			_log.ForEach(Console.WriteLine);
		}

		public Dictionary<MapLocation, List<MapChange>> MapLocationRequirements;
		public Dictionary<MapLocation, Tuple<MapLocation, AccessRequirement>> FloorLocationRequirements;
		public Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> FullLocationRequirements;

		public const byte GrassTile = 0x00;
		public const byte GrassBottomRightCoast = 0x06;
		public const byte OceanTile = 0x17;
		public const byte RiverTile = 0x44;
		public const byte MountainTopLeft = 0x10;
		public const byte MountainTopMid = 0x11;
		public const byte MountainMid = 0x21;
		public const byte MountainBottomLeft = 0x30;
		public const byte MountainBottomMid = 0x31;
		public const byte MountainBottomRight = 0x33;
		public const byte ForestMid = 0x14;
		public const byte ForestBottomMid = 0x24;
		public const byte ForestBottomRight = 0x25;
		public const byte ForestBottomLeft = 0x23;
		public const byte DockBottomMid = 0x78;
		public const byte DockRightMid = 0x1F;

		public static List<MapEdit> OnracDock =
			new List<MapEdit>
			{
				new MapEdit{X = 50, Y = 78, Tile = ForestBottomRight},
				new MapEdit{X = 51, Y = 78, Tile = DockBottomMid},
				new MapEdit{X = 52, Y = 78, Tile = DockBottomMid},
				new MapEdit{X = 51, Y = 77, Tile = ForestBottomMid},
				new MapEdit{X = 52, Y = 77, Tile = ForestBottomMid},
				new MapEdit{X = 51, Y = 79, Tile = OceanTile},
				new MapEdit{X = 52, Y = 79, Tile = OceanTile}
			};
		public static List<MapEdit> MirageDock =
			new List<MapEdit>
			{
				new MapEdit{X = 208, Y = 90, Tile = DockBottomMid},
				new MapEdit{X = 209, Y = 90, Tile = DockBottomMid}
			};
		public static List<MapEdit> ConeriaToDwarves =
			new List<MapEdit>
			{
				new MapEdit{X = 124, Y = 138, Tile = MountainBottomLeft},
				new MapEdit{X = 124, Y = 139, Tile = GrassTile},
				new MapEdit{X = 125, Y = 139, Tile = MountainBottomLeft},
				new MapEdit{X = 125, Y = 140, Tile = GrassTile},
				new MapEdit{X = 126, Y = 140, Tile = MountainBottomLeft},
				new MapEdit{X = 127, Y = 140, Tile = MountainBottomMid},
				new MapEdit{X = 128, Y = 140, Tile = MountainBottomMid},
				new MapEdit{X = 129, Y = 140, Tile = MountainBottomMid},
				new MapEdit{X = 126, Y = 141, Tile = GrassTile},
				new MapEdit{X = 127, Y = 141, Tile = GrassTile},
				new MapEdit{X = 128, Y = 141, Tile = GrassTile},
				new MapEdit{X = 129, Y = 141, Tile = GrassTile},
				new MapEdit{X = 130, Y = 141, Tile = MountainBottomLeft}
			};
		public static List<MapEdit> VolcanoIceRiver =
			new List<MapEdit>
			{
				new MapEdit{X = 209, Y = 189, Tile = MountainBottomRight},
				new MapEdit{X = 210, Y = 189, Tile = GrassTile},
				new MapEdit{X = 208, Y = 190, Tile = RiverTile},
				new MapEdit{X = 209, Y = 190, Tile = RiverTile},
				new MapEdit{X = 210, Y = 190, Tile = RiverTile},
				new MapEdit{X = 211, Y = 190, Tile = RiverTile},
				new MapEdit{X = 209, Y = 191, Tile = MountainTopLeft},
				new MapEdit{X = 210, Y = 191, Tile = MountainTopMid},
				new MapEdit{X = 211, Y = 191, Tile = MountainTopMid}
			};
		public static List<MapEdit> CanalSoftLockMountain =
			new List<MapEdit>
			{
				new MapEdit{X = 101, Y = 161, Tile = MountainTopLeft},
				new MapEdit{X = 102, Y = 161, Tile = MountainTopMid},
				new MapEdit{X = 103, Y = 161, Tile = MountainMid},
				new MapEdit{X = 101, Y = 162, Tile = MountainBottomLeft},
				new MapEdit{X = 102, Y = 162, Tile = MountainBottomMid},
				new MapEdit{X = 103, Y = 162, Tile = MountainBottomRight}
			};
		public static Dictionary<OverworldTeleportIndex, Palette> OverworldToPalette =
			new Dictionary<OverworldTeleportIndex, Palette>
			{
				{OverworldTeleportIndex.Coneria,            Palette.YellowGreen},
				{OverworldTeleportIndex.Pravoka,            Palette.Bluescale},
				{OverworldTeleportIndex.Elfland,            Palette.Pink},
				{OverworldTeleportIndex.Melmond,            Palette.Flame},
				{OverworldTeleportIndex.CrescentLake,       Palette.YellowGreen},
				{OverworldTeleportIndex.Gaia,               Palette.Orange},
				{OverworldTeleportIndex.Onrac,              Palette.Blue},
				{OverworldTeleportIndex.Lefein,             Palette.Purple},
				{OverworldTeleportIndex.ConeriaCastle1,     Palette.Yellow},
				{OverworldTeleportIndex.ElflandCastle,      Palette.Green},
				{OverworldTeleportIndex.NorthwestCastle,    Palette.PaleGreen},
				{OverworldTeleportIndex.CastleOrdeals1,     Palette.Greyscale},
				{OverworldTeleportIndex.TempleOfFiends1,    Palette.Greyscale},
				{OverworldTeleportIndex.DwarfCave,          Palette.DarkOrange},
				{OverworldTeleportIndex.MatoyasCave,        Palette.Purple},
				{OverworldTeleportIndex.SardasCave,         Palette.Yellow},
				{OverworldTeleportIndex.MarshCave1,         Palette.PaleGreen},
				{OverworldTeleportIndex.EarthCave1,         Palette.Orange},
				{OverworldTeleportIndex.GurguVolcano1,      Palette.Red},
				{OverworldTeleportIndex.IceCave1,           Palette.PaleBlue},
				{OverworldTeleportIndex.Cardia1,            Palette.Teal},
				{OverworldTeleportIndex.Cardia2,            Palette.Teal},
				{OverworldTeleportIndex.Cardia4,            Palette.Teal},
				{OverworldTeleportIndex.Cardia5,            Palette.Teal},
				{OverworldTeleportIndex.Cardia6,            Palette.Teal},
				{OverworldTeleportIndex.BahamutCave1,       Palette.DarkTeal},
				{OverworldTeleportIndex.Waterfall,          Palette.DarkTeal},
				{OverworldTeleportIndex.MirageTower1,       Palette.DarkOrange},
				{OverworldTeleportIndex.TitansTunnelEast,   Palette.DarkBlue},
				{OverworldTeleportIndex.TitansTunnelWest,   Palette.DarkBlue},
				{OverworldTeleportIndex.Unused1,            Palette.Greyscale},
				{OverworldTeleportIndex.Unused2,            Palette.Greyscale},
			};

		public enum Palette
		{
			Town = 0,
			Castle = 1,
			Greyscale = 2,
			Orange = 3,
			DarkOrange = 4,
			PaleRed = 5,
			Red = 6,
			PaleBlue = 7,
			Teal = 8,
			DarkTeal = 9,
			Purple = 10,
			Yellow = 11,
			PaleGreen = 12,
			Green = 13,
			Blue = 14,
			LightBlue = 15,
			DarkBlue = 16,
			Bluescale = 17,
			YellowGreen = 18,
			Pink = 19,
			Flame = 20,
		};

		public static List<MapIndex> FixedObjectPaletteDestinations = new List<MapIndex>
		{
			MapIndex.MirageTower1F, MapIndex.MirageTower2F, MapIndex.MirageTower3F, MapIndex.SkyPalace1F,
			MapIndex.SkyPalace2F, MapIndex.SkyPalace3F, MapIndex.SkyPalace4F, MapIndex.SkyPalace5F,
		};

		public List<List<MapEdit>> MapEditsToApply = new List<List<MapEdit>>();

		const int bankStart = 0x4000;

		public List<List<byte>> GetCompressedMapRows()
		{

			var pointers = _rom.Get(bankStart, 512).ToUShorts().Select(x => x - bankStart);
			var mapRows = pointers.Select(x =>
			{
				var mapRow = _rom.Get(x, 256).ToBytes();
				var result = new List<byte>();
				var index = 0;
				while (index < 256 && mapRow[index] != 255)
				{
					result.Add(mapRow[index]);
					index++;
				}
				result.Add(mapRow[index]);
				return result;
			}).ToList();
			return mapRows;
		}

		public List<List<byte>> DecompressMapRows(List<List<byte>> compressedRows)
		{
			var mapRows = new List<List<byte>>();
			var run = 0;
			foreach (var compressedRow in compressedRows)
			{
				byte tile = 0;
				var row = new List<byte>();
				var tileIndex = 0;
				while (row.Count() < 256)
				{
					tile = compressedRow[tileIndex];
					if (tile < 0x80)
					{
						row.Add(tile);
					}
					else if (tile == 0xFF)
					{
						for (var i = tileIndex; i < 256; i++)
						{
							row.Add(0x17);
						}
					}
					else
					{
						tileIndex++;
						run = compressedRow[tileIndex];
						if (run == 0)
							run = 256;
						tile -= 0x80;
						for (var i = 0; i < run; i++)
						{
							row.Add(tile);
						}
					}
					tileIndex++;
				}
				mapRows.Add(row);
			}
			return mapRows;
		}

		public void DebugWriteDecompressedMap(List<List<byte>> decompressedRows)
		{
			foreach (var row in decompressedRows)
			{
				foreach (var tile in row)
				{
					Debug.Write($"{tile:X2}");
				}
				Debug.Write("\n");
			}
		}

		public void ApplyMapEdits()
		{
			var compresedMap = GetCompressedMapRows();
			var decompressedMap = DecompressMapRows(compresedMap);
			var editedMap = decompressedMap;
			foreach (var mapEdit in MapEditsToApply)
			{
				editedMap = ApplyMapEdits(editedMap, mapEdit);
			}
			var recompressedMap = CompressMapRows(editedMap);
			PutCompressedMapRows(recompressedMap);
		}

		public List<List<byte>> ApplyMapEdits(List<List<byte>> decompressedRows, List<MapEdit> mapEdits)
		{
			foreach (var mapEdit in mapEdits)
			{
				decompressedRows[mapEdit.Y][mapEdit.X] = mapEdit.Tile;
			}
			return decompressedRows;
		}

		public List<List<byte>> CompressMapRows(List<List<byte>> decompressedRows)
		{
			var outputMap = new List<List<byte>>();
			foreach (var row in decompressedRows)
			{
				var outputRow = new List<byte>();
				byte tile = 0;
				byte runCount = 1;
				if (row.Distinct().Count() == 1)
				{
					outputMap.Add(new List<byte> { 0x97, 0x00, 0xFF });
					continue;
				}
				for (var tileIndex = 0; tileIndex < 256; tileIndex++)
				{
					tile = row[tileIndex];
					if (tileIndex != 255 && tile == row[tileIndex + 1])
					{
						runCount++;
						continue;
					}
					if (runCount == 1)
					{
						outputRow.Add(tile);
						continue;
					}
					outputRow.Add((byte)(tile + 0x80));
					outputRow.Add(runCount);
					runCount = 1;
				}
				outputRow.Add(0xFF);
				outputMap.Add(outputRow);
			}
			return outputMap;
		}

		public void PutCompressedMapRows(List<List<byte>> compressedRows)
		{
			var pointerBase = 0x4000;
			var outputBase = 0x4200;
			var outputOffset = 0;
			for (int i = 0; i < compressedRows.Count; i++)
			{
				var outputRow = compressedRows[i];
				_rom.Put(pointerBase + i * 2, Blob.FromUShorts(new ushort[] { (ushort)(outputBase + pointerBase + outputOffset) }));
				_rom.Put(outputBase + outputOffset, outputRow.ToArray());
				outputOffset += outputRow.Count;
			}

			if (outputOffset > 0x4000)
				throw new InvalidOperationException("Modified map was too large to recompress and fit into a single bank.");
		}
	}
}
