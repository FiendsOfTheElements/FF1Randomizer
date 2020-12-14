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
		private Dictionary<WalkableRegion, List<OverworldTeleportIndex>> _walkableNodes;
		private Dictionary<CanoeableRegion, List<OverworldTeleportIndex>> _canoeableNodes;
		private Dictionary<MapLocation, List<MapChange>> MapLocationRequirements;
		private Dictionary<MapLocation, Tuple<MapLocation, AccessRequirement>> FloorLocationRequirements;

		private TeleportShuffle _teleporters;

		private enum WalkableRegion
		{
			ConeriaRegion = 0,
			ElflandRegion = 1,
			PravokaRegion = 2,
			MelmondRegion = 3,
			SardaRegion = 4,
			BahamutRegion = 5,
		}

		private enum CanoeableRegion
		{
			ElflandRegion = 0,
			PravokaRegion = 1,
			OnracRegion = 2,
		}

		private List<string> _log;

		public const int MapPaletteOffset = 0x2000;
		public const int MapPaletteSize = 48;
		public const int MapCount = 64;

		public OverworldMap(FF1Rom rom, IMapEditFlags flags, Dictionary<Palette, Blob> palettes, TeleportShuffle teleporters)
		{
			_rom = rom;
			_palettes = palettes;
			_log = new List<string>();
			_teleporters = teleporters;

			var mapLocationRequirements = ItemLocations.MapLocationRequirements.ToDictionary(x => x.Key, x => x.Value.ToList());
			var floorLocationRequirements = ItemLocations.MapLocationFloorRequirements.ToDictionary(x => x.Key, x => x.Value);

			_walkableNodes = new Dictionary<WalkableRegion, List<OverworldTeleportIndex>> {
				{ WalkableRegion.ConeriaRegion, new List<OverworldTeleportIndex>{OverworldTeleportIndex.ConeriaCastle1, OverworldTeleportIndex.Coneria, OverworldTeleportIndex.TempleOfFiends1} },
				{ WalkableRegion.ElflandRegion, new List<OverworldTeleportIndex>{OverworldTeleportIndex.ElflandCastle, OverworldTeleportIndex.Elfland, OverworldTeleportIndex.NorthwestCastle, OverworldTeleportIndex.MarshCave1} },
				{ WalkableRegion.PravokaRegion, new List<OverworldTeleportIndex>{OverworldTeleportIndex.MatoyasCave, OverworldTeleportIndex.Pravoka} },
				{ WalkableRegion.MelmondRegion, new List<OverworldTeleportIndex>{OverworldTeleportIndex.Melmond, OverworldTeleportIndex.EarthCave1, OverworldTeleportIndex.TitansTunnelEast} },
				{ WalkableRegion.SardaRegion, new List<OverworldTeleportIndex>{OverworldTeleportIndex.SardasCave, OverworldTeleportIndex.TitansTunnelWest} },
				{ WalkableRegion.BahamutRegion, new List<OverworldTeleportIndex>{OverworldTeleportIndex.Cardia1, OverworldTeleportIndex.BahamutCave1 } }
			};

			_canoeableNodes = new Dictionary<CanoeableRegion, List<OverworldTeleportIndex>> {
				{ CanoeableRegion.ElflandRegion, new List<OverworldTeleportIndex>{
					OverworldTeleportIndex.ElflandCastle, OverworldTeleportIndex.Elfland, OverworldTeleportIndex.NorthwestCastle, OverworldTeleportIndex.MarshCave1,
					OverworldTeleportIndex.CrescentLake, OverworldTeleportIndex.GurguVolcano1} },
				{ CanoeableRegion.PravokaRegion, new List<OverworldTeleportIndex>{OverworldTeleportIndex.MatoyasCave, OverworldTeleportIndex.Pravoka, OverworldTeleportIndex.IceCave1} },
				{ CanoeableRegion.OnracRegion, new List<OverworldTeleportIndex>{OverworldTeleportIndex.Onrac, OverworldTeleportIndex.Waterfall} }
			};

			if ((bool)flags.MapOnracDock)
			{
				MapEditsToApply.Add(OnracDock);
				mapLocationRequirements[MapLocation.Onrac].Add(MapChange.Ship | MapChange.Canal);
				mapLocationRequirements[MapLocation.Caravan].Add(MapChange.Ship | MapChange.Canal | MapChange.Canoe);
				mapLocationRequirements[MapLocation.Waterfall].Add(MapChange.Ship | MapChange.Canal | MapChange.Canoe);
			}
			if ((bool)flags.MapMirageDock)
			{
				MapEditsToApply.Add(MirageDock);
				mapLocationRequirements[MapLocation.MirageTower1].Add(MapChange.Ship | MapChange.Canal | MapChange.Chime);
			}
			if ((bool)flags.MapAirshipDock)
			{
				MapEditsToApply.Add(AirshipDock);
				mapLocationRequirements[MapLocation.AirshipLocation].Add(MapChange.Ship | MapChange.Canal);
			}
			if ((bool)flags.MapVolcanoIceRiver)
			{
				MapEditsToApply.Add(VolcanoIceRiver);
				mapLocationRequirements[MapLocation.GurguVolcano1].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.CrescentLake].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.Elfland].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.ElflandCastle].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.NorthwestCastle].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.MarshCave1].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.AirshipLocation].Add(MapChange.Bridge | MapChange.Canoe);
				
				if ((bool)flags.MapCanalBridge)
				{
					mapLocationRequirements[MapLocation.DwarfCave].Add(MapChange.Bridge | MapChange.Canoe);
					_canoeableNodes[CanoeableRegion.ElflandRegion].Add(OverworldTeleportIndex.DwarfCave);
				}

				_canoeableNodes[CanoeableRegion.ElflandRegion].AddRange(_canoeableNodes[CanoeableRegion.PravokaRegion]);
				_canoeableNodes[CanoeableRegion.PravokaRegion].Clear();
			}
			if ((bool)flags.MapConeriaDwarves)
			{
				MapEditsToApply.Add(ConeriaToDwarves);
				mapLocationRequirements[MapLocation.DwarfCave].Add(MapChange.None);
				_walkableNodes[WalkableRegion.ConeriaRegion].Add(OverworldTeleportIndex.DwarfCave);

				if ((bool)flags.MapCanalBridge)
				{
					MapChange dwarvesToNorthwest = MapChange.Canoe;
					if ((bool)flags.MapDwarvesNorthwest)
					{
						MapEditsToApply.Add(DwarvesNorthwestGrass);
						dwarvesToNorthwest = MapChange.None;

						_walkableNodes[WalkableRegion.ConeriaRegion].AddRange(_walkableNodes[WalkableRegion.ElflandRegion]);
						_walkableNodes[WalkableRegion.ElflandRegion].Clear();
					}
					mapLocationRequirements[MapLocation.Elfland].Add(dwarvesToNorthwest);
					mapLocationRequirements[MapLocation.ElflandCastle].Add(dwarvesToNorthwest);
					mapLocationRequirements[MapLocation.NorthwestCastle].Add(dwarvesToNorthwest);
					mapLocationRequirements[MapLocation.MarshCave1].Add(dwarvesToNorthwest);

					mapLocationRequirements[MapLocation.GurguVolcano1].Add(MapChange.Canoe);
					mapLocationRequirements[MapLocation.CrescentLake].Add(MapChange.Canoe);
					mapLocationRequirements[MapLocation.AirshipLocation].Add(MapChange.Canoe);
					if ((bool)flags.MapVolcanoIceRiver)
					{
						mapLocationRequirements[MapLocation.IceCave1].Add(MapChange.Canoe);
						mapLocationRequirements[MapLocation.Pravoka].Add(MapChange.Canoe);
						mapLocationRequirements[MapLocation.MatoyasCave].Add(MapChange.Canoe);
					}
				}
			}

			if ((bool)flags.TitansTrove)
			{
				floorLocationRequirements[MapLocation.TitansTunnelRoom] =
					new Tuple<MapLocation, AccessRequirement>(MapLocation.TitansTunnelWest, AccessRequirement.Ruby);
			}

			if ((bool)flags.EarlyOrdeals)
			{
				floorLocationRequirements[MapLocation.CastleOrdealsMaze] = new Tuple<MapLocation, AccessRequirement>(MapLocation.CastleOrdeals1, AccessRequirement.None);
			}

			ObjectiveNPCs = new Dictionary<ObjectId, MapLocation>
			{
				{ ObjectId.Bahamut, MapLocation.BahamutCave2 },
				{ ObjectId.Unne, MapLocation.Melmond },
				{ ObjectId.ElfDoc, MapLocation.ElflandCastle },
			};

			StartingPotentialAccess = AccessRequirement.Key | AccessRequirement.Tnt | AccessRequirement.Adamant;
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

		public void PutPalette(OverworldTeleportIndex source, MapIndex index)
		{
			var palette = OverworldToPalette[source];

			if (index <= MapIndex.Lefein) // Towns
			{
				// Towns are given arbitrary palettes to help provide color when dungeons take their place.
				if (source != OverworldTeleportIndex.Coneria)
				{
					_rom.Put(MapPaletteOffset + (int)index * MapPaletteSize + 1, _palettes[palette].SubBlob(9, 2));
					_rom.Put(MapPaletteOffset + (int)index * MapPaletteSize + 6, _palettes[palette].SubBlob(9, 1));
					_rom.Put(MapPaletteOffset + (int)index * MapPaletteSize + 33, _palettes[palette].SubBlob(9, 2));
					_rom.Put(MapPaletteOffset + (int)index * MapPaletteSize + 38, _palettes[palette].SubBlob(9, 1));
				}
			}
			else if (index < MapIndex.EarthCaveB1) // Castles - just tint the lawns.
			{
				_rom.Put(MapPaletteOffset + (int)index * MapPaletteSize + 8, _palettes[palette].SubBlob(8, 8));
				_rom.Put(MapPaletteOffset + (int)index * MapPaletteSize + 40, _palettes[palette].SubBlob(40, 8));
			}
			else // Dungeons
			{
				_rom.Put(MapPaletteOffset + (int)index * MapPaletteSize, _palettes[palette].SubBlob(0, 16));

				// Some maps have greyscale objects (Chests / Pillars) that look wrong when tinted (usually brown).
				var paletteIndex = FixedObjectPaletteDestinations.Contains(index) ? 36 : 32;
				_rom.Put(MapPaletteOffset + (int)index * MapPaletteSize + paletteIndex, _palettes[palette].SubBlob(paletteIndex, 48 - paletteIndex));
			}
		}

        void UpdatePalettes(OverworldTeleportIndex oti, TeleportDestination teleport)
		{
            if (teleport.OwnsPalette)
			{
				var mapIndex = teleport.Index;
				PutPalette(oti, mapIndex);

                if (ContinuedMapIndexForPalettes.TryGetValue(mapIndex, out var list))
				{
					list.ForEach(map => PutPalette(oti, map));
				}
			}
		}

		public void PutOverworldTeleport(OverworldTeleportIndex index, TeleportDestination teleport)
		{
			_rom[teleportEntranceXOffset + (byte)index] = teleport.CoordinateX;
			_rom[teleportEntranceYOffset + (byte)index] = teleport.CoordinateY;
			_rom[teleportEntranceMapIndexOffset + (byte)index] = (byte)teleport.Index;

			UpdatePalettes(index, teleport);
		}

		public void PutStandardTeleport(TeleportIndex index, TeleportDestination teleport, OverworldTeleportIndex overworldEntryPoint)
		{
			_rom[teleportXOffset + (byte)index] = teleport.CoordinateX;
			_rom[teleportYOffset + (byte)index] = teleport.CoordinateY;
			_rom[teleportMapIndexOffset + (byte)index] = (byte)teleport.Index;

			UpdatePalettes(overworldEntryPoint, teleport);
		}

		public void UpdateOverworldOverride(MapLocation location, OverworldTeleportIndex owti)
		{
			OverriddenOverworldLocations[location] = owti;
			if (ConnectedMapLocations.TryGetValue(location, out var subLocations))
			{
				subLocations.ForEach(sublocation => OverriddenOverworldLocations[sublocation] = owti);
			}
		}

		public void ShuffleEntrancesAndFloors(MT19337 rng, IFloorShuffleFlags flags)
		{
			OverriddenOverworldLocations = new Dictionary<MapLocation, OverworldTeleportIndex>
			{
				[MapLocation.StartingLocation] = OverworldTeleportIndex.Coneria,
				[MapLocation.AirshipLocation] = OverworldTeleportIndex.Elfland,
				[MapLocation.Caravan] = OverworldTeleportIndex.Onrac
			};

			// Disable the Princess Warp back to Castle Coneria
			//if ((bool)flags.Entrances || (bool)flags.Floors) _rom.PutInBank(0x11, 0x9370 + 0x0F, Blob.FromHex("EAEAEA"));

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
			var placedMaps = _teleporters.VanillaOverworldTeleports.ToDictionary(x => x.Key, x => x.Value);
			var placedFloors = _teleporters.VanillaStandardTeleports.ToDictionary(x => x.Key, x => x.Value);
			var placedExits = new Dictionary<ExitTeleportIndex, Coordinate>();
			if ((bool)flags.Towns)
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
			if ((bool)flags.Entrances)
			{
				// Skip towns as they would be removed by the above.
				// Remove SeaShrine1 so Onrac could lead to anywhere.
				var keepers = _teleporters.VanillaOverworldTeleports.Where(x => x.Key >= OverworldTeleportIndex.Coneria && x.Key <= OverworldTeleportIndex.Lefein).Select(x => x.Key).ToList();

				if (!(bool)flags.EntrancesIncludesDeadEnds)
				{
					keepers.Add(OverworldTeleportIndex.Cardia1);
					keepers.Add(OverworldTeleportIndex.Cardia5);
				}

				placedMaps = placedMaps .Where(x => keepers.Contains(x.Key)) .ToDictionary(x => x.Key, x => x.Value);
				placedFloors.Remove(TeleportIndex.SeaShrine1);
				FixUnusedDefaultBackdrops();
			}
			if ((bool)flags.Floors && (bool)flags.Entrances)
			{
				placedFloors = new Dictionary<TeleportIndex, TeleportDestination>();
			}

			// All the finished destinations. We don't ever want to make anything new target one of these since some fixed map or floor already does.
			var placedDestinations = placedMaps.Values.Select(x => x.Destination).Concat(placedFloors.Values.Select(x => x.Destination)).ToList();

			// Grab a list of all non-unused overworld entrances, regardless of whether or not we've already placed them.
			var townEntrances = _teleporters.TownEntrances;
			var nonTownEntrances = Enum.GetValues(typeof(OverworldTeleportIndex)).Cast<OverworldTeleportIndex>().Where(x => !townEntrances.Contains(x) && x < OverworldTeleportIndex.Unused1).ToList();
			var shuffledOverworldCount = townEntrances.Count(x => !placedMaps.ContainsKey(x)) + nonTownEntrances.Count(x => !placedMaps.ContainsKey(x));

			// Grab a list of floors we haven't placed yet that can be shuffled. i.e. not including bottom of ice cave or ordeals.
			var towns = _teleporters.TownTeleports.Where(x => !placedDestinations.Contains(x.Destination)).ToList();
			var topfloors = _teleporters.NonTownForcedTopFloors.Where(x => !placedDestinations.Contains(x.Destination)).ToList();
			var subfloors = _teleporters.FreePlacementFloors.Where(x => !placedDestinations.Contains(x.Destination)).ToList();
			var deadEnds = new List<TeleportDestination>();

			towns.Shuffle(rng);
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
				topfloors = topfloors.Where(floor => floor.Destination != _teleporters.TempleOfFiends.Destination).ToList();
				deadEnds.Add(_teleporters.TempleOfFiends);
			}

			if ((bool)flags.DeepTownsPossible && (bool)flags.AllowDeepTowns)
			{
				subfloors.AddRange(towns);
				towns.Clear();
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
					shuffleEntrances = shuffleTowns.Concat(shuffleEntrances).ToList();
					shuffleEntrances.Shuffle(rng);
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
					UpdateOverworldOverride(destination.Destination, owti);

					if (destination.Exit != ExitTeleportIndex.None)
					{
						// Exiting floors like Fiend Orb Teleporters need to be updated to new OW coords.
						shuffledExits.Add(destination.Exit, _teleporters.OverworldCoordinates[owti]);
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
						UpdateOverworldOverride(floor.Destination, owti);
						if (floor.Exit != ExitTeleportIndex.None)
						{
							// Exiting floors like Fiend Orb Teleporters need to be updated to new OW coords.
							shuffledExits.Add(floor.Exit, _teleporters.OverworldCoordinates[owti]);
						}
					}
				}
			} while (!CheckEntranceSanity(shuffled, (bool)flags.AllowUnsafeStartArea));

			if (flags.Spoilers || Debugger.IsAttached)
			{
				Console.WriteLine($"OverworldMap::ShuffleEntrancesAndFloors() required {sanity} iterations.");
			}

			// Pretty print map data
			foreach (var map in shuffled.OrderBy(x => x.Key))
			{
				PutOverworldTeleport(map.Key, map.Value);
				_log.Add($"{map.Key.ToString().PadRight(30)}{map.Value.SpoilerText}");
				var teleports = map.Value.Teleports.ToList();

				while (teleports.Any())
				{
					var teleport = teleports.SpliceRandom(rng);
					var innerMap = shuffledFloors[teleport];
					teleports.AddRange(innerMap.Teleports);
					PutStandardTeleport(teleport, innerMap, map.Key);
					_log.Add($"\t{teleport.ToString().PadRight(30)}{innerMap.SpoilerText.Trim()} ({OverworldToPalette[map.Key]} tint)");
				}
			}

			// Write Exit teleport coords back to the ROM
			foreach (var exit in shuffledExits)
			{
				_rom[teleportExitXOffset + (byte)exit.Key] = exit.Value.X;
				_rom[teleportExitYOffset + (byte)exit.Key] = exit.Value.Y;
			}

			// Now it's time to update all the requirements for treasure sanity checking.
			var allTeleportLocations = shuffled.Select(x => x.Value.Destination).Concat(shuffledFloors.Select(x => x.Value.Destination)).Distinct().ToList();

			// Find out what two entrances the titan's tunnel now connects, and create new lists of MapLocations thusly connected.
			var titanEast = shuffled.Single(x => x.Value.Destination == MapLocation.TitansTunnelEast);
			var titanWest = shuffled.Single(x => x.Value.Destination == MapLocation.TitansTunnelWest);
			_log.Add($"{titanWest.Key} is TitansTunnelWest and connects to {titanEast.Key}");

			var titanWalkLocations = _walkableNodes.Values.Where(x => x.Contains(titanEast.Key) || x.Contains(titanWest.Key)).SelectMany(x => x).Distinct().Select(x => shuffled[x].Destination);
			var titanCanoeLocations = _canoeableNodes.Values.Where(x => x.Contains(titanEast.Key) || x.Contains(titanWest.Key)).SelectMany(x => x).Distinct().Select(x => shuffled[x].Destination);

			if (Debugger.IsAttached)
			{
				Dump();
			}

			// Put together a nice final mapping of MapLocations to requirements, in the shuffled map world.
			var standardMapLookup = _teleporters.StandardMapLocations;
			var destinationsByLocation = shuffled.ToDictionary(x => ItemLocations.OverworldToMapLocation[x.Key], x => x.Value);
			destinationsByLocation = destinationsByLocation
				.Concat(shuffledFloors.Select(x => new KeyValuePair<MapLocation, TeleportDestination>(standardMapLookup[x.Key], x.Value)))
				.ToDictionary(x => x.Key, x => x.Value);
			var newRequirements = defaultRequirements
				.ToDictionary(x => !allTeleportLocations.Contains(x.Key) ? x.Key :
							  destinationsByLocation[x.Key].Destination,
							  x => x.Value);

			// Segregate out the simple mapping into Entrances and Floors
			StartingPotentialAccess = AccessRequirement.AllExceptEnding;
			MapLocationRequirements = newRequirements.Where(x => x.Value.MapChanges != null).ToDictionary(x => x.Key, x => x.Value.MapChanges.ToList());
			FloorLocationRequirements = newRequirements.Where(x => x.Value.MapChanges == null).ToDictionary(x => x.Key, x => x.Value.TeleportLocation);

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
						throw new InsaneException($"Floor requirement cycle for: {location}");
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
					throw new InsaneException($"Orphaned unreachable floor: {location}?");
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
			backdrops[0x4C] = (byte)Backdrop.SeaShrine; // Elfland
			backdrops[0x4D] = (byte)Backdrop.Volcano; // Melmond
			backdrops[0x4E] = (byte)Backdrop.Marsh; // Crescent Lake
			backdrops[0x5A] = (byte)Backdrop.EarthCave; // Gaia
			backdrops[0x6C] = (byte)Backdrop.Waterfall; // Bahamut's Room
			backdrops[0x6D] = (byte)Backdrop.Tower; // Lefein
			_rom.Put(0x03300, backdrops);
		}

		public bool CheckEntranceSanity(IEnumerable<KeyValuePair<OverworldTeleportIndex, TeleportDestination>> shuffledEntrances, bool allowDanger)
		{
			var coneria = shuffledEntrances.Any(x => x.Key == OverworldTeleportIndex.Coneria && x.Value.Destination == MapLocation.Coneria);
			var starterLocation = shuffledEntrances.Any(x => StartingLocations.Contains(x.Key) && StarterDestinations.Contains(x.Value.Destination));
			var titansConnections =
				shuffledEntrances.Any(x => x.Value.Destination == MapLocation.TitansTunnelEast && ConnectedLocations.Contains(x.Key)) &&
				shuffledEntrances.Any(x => x.Value.Destination == MapLocation.TitansTunnelWest && ConnectedLocations.Contains(x.Key));

			bool isSafe(OverworldTeleportIndex owti)
			{
				return shuffledEntrances.Any(x => x.Key == owti && SafeLocations.Contains(x.Value.Destination));
			}

			int dangerCount = 5;
			if (isSafe(OverworldTeleportIndex.ConeriaCastle1)) --dangerCount;
			if (isSafe(OverworldTeleportIndex.TempleOfFiends1)) --dangerCount;
			if (isSafe(OverworldTeleportIndex.DwarfCave)) --dangerCount;
			if (isSafe(OverworldTeleportIndex.MatoyasCave)) --dangerCount;
			if (isSafe(OverworldTeleportIndex.Pravoka)) --dangerCount;

			return coneria && titansConnections && (allowDanger || (starterLocation && dangerCount <= 3));
		}

		public void Dump()
		{
			_log.ForEach(Console.WriteLine);
		}

		public Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> FullLocationRequirements;
		public Dictionary<MapLocation, OverworldTeleportIndex> OverriddenOverworldLocations;
		public Dictionary<ObjectId, MapLocation> ObjectiveNPCs;
		public AccessRequirement StartingPotentialAccess;

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
		public const byte CoastLeft = 0x16;

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
				new MapEdit{X = 209, Y = 90, Tile = DockBottomMid},
				new MapEdit{X = 208, Y = 91, Tile = OceanTile},
				new MapEdit{X = 209, Y = 91, Tile = OceanTile}
			};
		public static List<MapEdit> AirshipDock =
			new List<MapEdit>
			{
				new MapEdit{X = 216, Y = 244, Tile = DockBottomMid},
				new MapEdit{X = 217, Y = 244, Tile = DockBottomMid},
				new MapEdit{X = 216, Y = 245, Tile = OceanTile},
				new MapEdit{X = 217, Y = 245, Tile = OceanTile}
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
		public static List<MapEdit> DwarvesNorthwestGrass =
			new List<MapEdit>
			{
				new MapEdit{X = 104, Y = 171, Tile = GrassTile},
				new MapEdit{X = 105, Y = 171, Tile = GrassTile},
				new MapEdit{X = 106, Y = 171, Tile = CoastLeft}
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
		public static Dictionary<MapIndex, List<MapIndex>> ContinuedMapIndexForPalettes = 
            new Dictionary<MapIndex, List<MapIndex>>
		    {
				{ MapIndex.ConeriaCastle1F, new List<MapIndex> { MapIndex.ConeriaCastle2F } },
				{ MapIndex.CastleOrdeals1F, new List<MapIndex> { MapIndex.CastleOrdeals2F, MapIndex.CastleOrdeals3F } },
				{ MapIndex.IceCaveB2, new List<MapIndex> { MapIndex.IceCaveB3 } },
				{ MapIndex.TempleOfFiends, new List<MapIndex> { MapIndex.TempleOfFiends1F, MapIndex.TempleOfFiends2F,
					MapIndex.TempleOfFiends3F, MapIndex.TempleOfFiendsEarth, MapIndex.TempleOfFiendsFire,
					MapIndex.TempleOfFiendsWater, MapIndex.TempleOfFiendsAir, MapIndex.TempleOfFiendsChaos } }
			};
		public static Dictionary<MapLocation, List<MapLocation>> ConnectedMapLocations =
            new Dictionary<MapLocation, List<MapLocation>>
		    {
				{ MapLocation.ConeriaCastle1, new List<MapLocation> { MapLocation.ConeriaCastle2, MapLocation.ConeriaCastleRoom1, MapLocation.ConeriaCastleRoom2 } },
				{ MapLocation.ElflandCastle, new List<MapLocation> { MapLocation.ElflandCastleRoom1 } },
				{ MapLocation.NorthwestCastle, new List<MapLocation> { MapLocation.NorthwestCastleRoom2 } },
				{ MapLocation.CastleOrdeals1, new List<MapLocation> { MapLocation.CastleOrdealsMaze, MapLocation.CastleOrdealsTop } },
				{ MapLocation.IceCavePitRoom, new List<MapLocation> { MapLocation.IceCave5, MapLocation.IceCaveBackExit, MapLocation.IceCaveFloater } },
				{ MapLocation.DwarfCave, new List<MapLocation> { MapLocation.DwarfCaveRoom3 } },
				{ MapLocation.MarshCaveBottom, new List<MapLocation> { MapLocation.MarshCaveBottomRoom13, MapLocation.MarshCaveBottomRoom14, MapLocation.MarshCaveBottomRoom16 } },
				{ MapLocation.SeaShrine2, new List<MapLocation> { MapLocation.SeaShrine2Room2 } },
				{ MapLocation.TitansTunnelWest, new List<MapLocation> { MapLocation.TitansTunnelRoom } },
				{ MapLocation.TempleOfFiends1, new List<MapLocation> { MapLocation.TempleOfFiends1Room1, MapLocation.TempleOfFiends1Room2,
					MapLocation.TempleOfFiends1Room3, MapLocation.TempleOfFiends1Room4, MapLocation.TempleOfFiends2, MapLocation.TempleOfFiends3,
					MapLocation.TempleOfFiendsPhantom, MapLocation.TempleOfFiendsEarth, MapLocation.TempleOfFiendsFire,
					MapLocation.TempleOfFiendsWater, MapLocation.TempleOfFiendsAir, MapLocation.TempleOfFiendsChaos } }
			};

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
				OverworldTeleportIndex.ConeriaCastle1, OverworldTeleportIndex.TempleOfFiends1
			};

		private static readonly Dictionary<MapLocation, (int x, int y)> ObjectiveNPCPositions = new Dictionary<MapLocation, (int x, int y)>
		{
			{ MapLocation.BahamutCave2, (0x15, 0x03) },
			{ MapLocation.Melmond, (0x1A, 0x01) },
			{ MapLocation.ElflandCastle, (0x09, 0x05) },
		};

		private static readonly Dictionary<MapLocation, MapId> ObjectiveNPCMapIds = new Dictionary<MapLocation, MapId>
		{
			{ MapLocation.BahamutCave2, MapId.BahamutsRoomB2 },
			{ MapLocation.Melmond, MapId.Melmond },
			{ MapLocation.ElflandCastle, MapId.ElflandCastle },
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
				//if (row.Distinct().Count() == 1)
				//{
				//	outputMap.Add(new List<byte> { 0x97, 0x00, 0xFF });
				//	continue;
				//}
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

		public void ShuffleObjectiveNPCs(MT19337 rng)
		{
			var locations = ObjectiveNPCs.Values.ToList();
			foreach(var npc in ObjectiveNPCs.Keys.ToList())
			{
				var location = locations.SpliceRandom(rng);
				ObjectiveNPCs[npc] = location;

				var (x, y) = ObjectiveNPCPositions[location];
				y += (location == MapLocation.ElflandCastle && npc == ObjectId.Bahamut) ? 1 : 0;

				var inRoom = location != MapLocation.Melmond;
				var stationary = npc == ObjectId.Bahamut || (npc == ObjectId.ElfDoc && location == MapLocation.ElflandCastle);

				_rom.SetNpc(ObjectiveNPCMapIds[location], 0, npc, x, y, inRoom, stationary);
			}
		}
	}
}
