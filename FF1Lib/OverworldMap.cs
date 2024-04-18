using System.Diagnostics;
using FF1Lib.Sanity;
using FF1Lib.Procgen;
using System;

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

		private Teleporters _teleporters;

		public Teleporters Teleporters { get { return _teleporters; } set { _teleporters = value; } }

		private enum WalkableRegion
		{
			ConeriaRegion = 0,
			ElflandRegion = 1,
			PravokaRegion = 2,
			MelmondRegion = 3,
			SardaRegion = 4,
			BahamutRegion = 5,
			LefeinRegion = 6    // when MapGaiaMountainPass
		}

		private enum CanoeableRegion
		{
			ElflandRegion = 0,
			PravokaRegion = 1,
			OnracRegion = 2,
		}

		private List<string> _log;
		private List<List<byte>> _decompressedMap;

		public const int MapPaletteOffset = 0x2000;
		public const int MapPaletteSize = 48;
		public const int MapCount = 64;

		public List<List<byte>> MapBytes { get => _decompressedMap; }

		public OverworldMap(FF1Rom rom, IMapEditFlags flags)
		{
			_rom = rom;
			_palettes = GeneratePalettes(rom.Get(MapPaletteOffset, MapCount * MapPaletteSize).Chunk(MapPaletteSize));
			_log = new List<string>();

			_decompressedMap = DecompressMapRows(GetCompressedMapRows());

			var mapLocationRequirements = ItemLocations.MapLocationRequirements.ToDictionary(x => x.Key, x => x.Value.ToList());
			var floorLocationRequirements = ItemLocations.MapLocationFloorRequirements.ToDictionary(x => x.Key, x => x.Value);
			
			if ((bool)flags.AirBoat)
			{
				foreach(var location in mapLocationRequirements)
				{
					if (location.Value.Contains(MapChange.Airship | MapChange.Canoe))
					{
						location.Value.Remove(MapChange.Airship | MapChange.Canoe);
						location.Value.Add(MapChange.Airship | MapChange.Ship);
					}
				}
			}

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

			if (flags.GameMode == GameModes.Standard && flags.OwMapExchange == OwMapExchanges.None) {
			    // Can only apply map edits to vanilla-ish maps


			// NOTE: mapLocationRequirements information (for all of these map changes) is no longer used by the map generator and does nothing (TODO: Delete it all)
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
			if ((bool)flags.MapAirshipDock && !flags.DisableOWMapModifications)
			{
				MapEditsToApply.Add(AirshipDock);
				mapLocationRequirements[MapLocation.AirshipLocation].Add(MapChange.Ship | MapChange.Canal);
			}
			if ((bool)flags.MapBahamutCardiaDock && !flags.DisableOWMapModifications)
			{
				MapEditsToApply.Add(BahamutCardiaDock);
				mapLocationRequirements[MapLocation.BahamutCave1].Add(MapChange.Ship | MapChange.Canal);
				mapLocationRequirements[MapLocation.Cardia1].Add(MapChange.Ship | MapChange.Canal);
			}
			if ((bool)flags.MapLefeinRiver && !flags.DisableOWMapModifications) {
			    MapEditsToApply.Add(LefeinRiverDock);
			    mapLocationRequirements[MapLocation.Lefein].Add(MapChange.Ship | MapChange.Canal | MapChange.Canoe);
			}
			if ((bool)flags.MapBridgeLefein && !flags.DisableOWMapModifications) {
					// Moves the Bridge to its new home below Lefein
					OwLocationData _OwLocationData = new(rom);
					_OwLocationData.LoadData();
					_OwLocationData.BridgeLocation = new SCCoords(230, 123);
					_OwLocationData.StoreData();

			    MapEditsToApply.Add(BridgeToLefein);
					mapLocationRequirements[MapLocation.Lefein].Add(MapChange.Bridge);
					mapLocationRequirements[MapLocation.MatoyasCave].Add(MapChange.None);
					mapLocationRequirements[MapLocation.Pravoka].Add(MapChange.None);
					mapLocationRequirements[MapLocation.IceCave1].Add(MapChange.Canoe);
			}
			if ((bool)flags.MapGaiaMountainPass && !flags.DisableOWMapModifications) {
			    MapEditsToApply.Add(GaiaMountainPass);
					// If Lefein River Dock is on, then Gaia also becomes Ship-accessible
			    if ((bool)flags.MapLefeinRiver) {
			        mapLocationRequirements[MapLocation.Gaia].Add(MapChange.Ship | MapChange.Canal | MapChange.Canoe);
			    }
					// If Lefein Bridge is on, then Gaia is also Bridge-accessible
					if ((bool)flags.MapBridgeLefein) {
							mapLocationRequirements[MapLocation.Gaia].Add(MapChange.Bridge);
					}
			    _walkableNodes[WalkableRegion.LefeinRegion] = new List<OverworldTeleportIndex>{OverworldTeleportIndex.Gaia, OverworldTeleportIndex.Lefein };
			}
			if ((bool)flags.MapHighwayToOrdeals && !flags.DisableOWMapModifications)
			{
				MapEditsToApply.Add(HighwayToOrdeals);
			}
			if ((bool)flags.MapRiverToMelmond && !flags.DisableOWMapModifications) {
				MapEditsToApply.Add(RiverToMelmond);
				// With Early Open Progression, you only need a Canoe
				if ((bool)flags.MapConeriaDwarves) {
					mapLocationRequirements[MapLocation.Melmond].Add(MapChange.Canoe);
					mapLocationRequirements[MapLocation.TitansTunnelEast].Add(MapChange.Canoe);
					mapLocationRequirements[MapLocation.EarthCave1].Add(MapChange.Canoe);
				}
				else {
					mapLocationRequirements[MapLocation.Melmond].Add(MapChange.Ship | MapChange.Canoe);
					mapLocationRequirements[MapLocation.TitansTunnelEast].Add(MapChange.Ship | MapChange.Canoe);
					mapLocationRequirements[MapLocation.EarthCave1].Add(MapChange.Ship | MapChange.Canoe);
				}
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
					if ((bool)flags.MapDwarvesNorthwest && !flags.DisableOWMapModifications)
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

			if (((IVictoryConditionFlags)flags).NoOverworld)
			{
				_walkableNodes[WalkableRegion.ConeriaRegion].Clear();
				_walkableNodes[WalkableRegion.PravokaRegion].Clear();
				_walkableNodes[WalkableRegion.ElflandRegion].Clear();
				//_walkableNodes[WalkableRegion.LefeinRegion].Clear();
				_walkableNodes[WalkableRegion.MelmondRegion].Clear();
				_walkableNodes[WalkableRegion.SardaRegion].Clear();

				_canoeableNodes[CanoeableRegion.ElflandRegion].Clear();
				_canoeableNodes[CanoeableRegion.PravokaRegion].Clear();
				_canoeableNodes[CanoeableRegion.OnracRegion].Clear();

				// Coneria Island
				mapLocationRequirements[MapLocation.Coneria].Clear();
				mapLocationRequirements[MapLocation.Coneria].Add(MapChange.None);
				_walkableNodes[WalkableRegion.ConeriaRegion].Add(OverworldTeleportIndex.Coneria);

				mapLocationRequirements[MapLocation.ConeriaCastle1].Clear();
				mapLocationRequirements[MapLocation.ConeriaCastle1].Add(MapChange.None);
				_walkableNodes[WalkableRegion.ConeriaRegion].Add(OverworldTeleportIndex.ConeriaCastle1);

				mapLocationRequirements[MapLocation.TempleOfFiends1].Clear();
				mapLocationRequirements[MapLocation.TempleOfFiends1].Add(MapChange.None);
				_walkableNodes[WalkableRegion.ConeriaRegion].Add(OverworldTeleportIndex.TempleOfFiends1);

				mapLocationRequirements[MapLocation.MatoyasCave].Clear();
				mapLocationRequirements[MapLocation.MatoyasCave].Add(MapChange.None);
				_walkableNodes[WalkableRegion.ConeriaRegion].Add(OverworldTeleportIndex.MatoyasCave);

				mapLocationRequirements[MapLocation.Pravoka].Clear();
				mapLocationRequirements[MapLocation.Pravoka].Add(MapChange.None);
				_walkableNodes[WalkableRegion.ConeriaRegion].Add(OverworldTeleportIndex.Pravoka);

				mapLocationRequirements[MapLocation.MarshCave1].Clear();
				mapLocationRequirements[MapLocation.MarshCave1].Add(MapChange.None);
				_walkableNodes[WalkableRegion.ConeriaRegion].Add(OverworldTeleportIndex.MarshCave1);

				mapLocationRequirements[MapLocation.NorthwestCastle].Clear();
				mapLocationRequirements[MapLocation.NorthwestCastle].Add(MapChange.None);
				_walkableNodes[WalkableRegion.ConeriaRegion].Add(OverworldTeleportIndex.NorthwestCastle);

				mapLocationRequirements[MapLocation.Elfland].Clear();
				mapLocationRequirements[MapLocation.Elfland].Add(MapChange.None);
				_walkableNodes[WalkableRegion.ConeriaRegion].Add(OverworldTeleportIndex.Elfland);

				mapLocationRequirements[MapLocation.ElflandCastle].Clear();
				mapLocationRequirements[MapLocation.ElflandCastle].Add(MapChange.None);
				_walkableNodes[WalkableRegion.ConeriaRegion].Add(OverworldTeleportIndex.ElflandCastle);

				mapLocationRequirements[MapLocation.DwarfCave].Clear();
				mapLocationRequirements[MapLocation.DwarfCave].Add(MapChange.None);
				_walkableNodes[WalkableRegion.ConeriaRegion].Add(OverworldTeleportIndex.DwarfCave);

				mapLocationRequirements[MapLocation.TitansTunnelEast].Clear();
				mapLocationRequirements[MapLocation.TitansTunnelEast].Add(MapChange.None);
				_walkableNodes[WalkableRegion.ConeriaRegion].Add(OverworldTeleportIndex.TitansTunnelEast);

				mapLocationRequirements[MapLocation.AirshipLocation].Clear();
				mapLocationRequirements[MapLocation.AirshipLocation].Add(MapChange.None);

				// Caravan Island
				mapLocationRequirements[MapLocation.Caravan].Clear();
				mapLocationRequirements[MapLocation.Caravan].Add(MapChange.Airship | MapChange.Canoe);

				mapLocationRequirements[MapLocation.Onrac].Clear();
				mapLocationRequirements[MapLocation.Onrac].Add(MapChange.Airship | MapChange.Canoe);
				_walkableNodes[WalkableRegion.PravokaRegion].Add(OverworldTeleportIndex.Onrac);
				_canoeableNodes[CanoeableRegion.OnracRegion].Add(OverworldTeleportIndex.Onrac);

				mapLocationRequirements[MapLocation.Cardia5].Clear();
				mapLocationRequirements[MapLocation.Cardia5].Add(MapChange.Airship | MapChange.Canoe);
				_walkableNodes[WalkableRegion.PravokaRegion].Add(OverworldTeleportIndex.Cardia5);
				_canoeableNodes[CanoeableRegion.OnracRegion].Add(OverworldTeleportIndex.Cardia5);

				mapLocationRequirements[MapLocation.Cardia6].Clear();
				mapLocationRequirements[MapLocation.Cardia6].Add(MapChange.Airship | MapChange.Canoe);
				_walkableNodes[WalkableRegion.PravokaRegion].Add(OverworldTeleportIndex.Cardia6);
				_canoeableNodes[CanoeableRegion.OnracRegion].Add(OverworldTeleportIndex.Cardia6);

				mapLocationRequirements[MapLocation.BahamutCave1].Clear();
				mapLocationRequirements[MapLocation.BahamutCave1].Add(MapChange.Airship | MapChange.Canoe);
				_walkableNodes[WalkableRegion.PravokaRegion].Add(OverworldTeleportIndex.BahamutCave1);
				_canoeableNodes[CanoeableRegion.OnracRegion].Add(OverworldTeleportIndex.BahamutCave1);


				// Mirage Island
				mapLocationRequirements[MapLocation.Waterfall].Clear();
				mapLocationRequirements[MapLocation.Waterfall].Add(MapChange.Airship);
				_walkableNodes[WalkableRegion.SardaRegion].Add(OverworldTeleportIndex.Waterfall);

				mapLocationRequirements[MapLocation.Gaia].Clear();
				mapLocationRequirements[MapLocation.Gaia].Add(MapChange.Airship);
				_walkableNodes[WalkableRegion.SardaRegion].Add(OverworldTeleportIndex.Gaia);


				mapLocationRequirements[MapLocation.MirageTower1].Clear();
				mapLocationRequirements[MapLocation.MirageTower1].Add(MapChange.Airship | MapChange.Chime);
				_walkableNodes[WalkableRegion.SardaRegion].Add(OverworldTeleportIndex.MirageTower1);

				mapLocationRequirements[MapLocation.Lefein].Clear();
				mapLocationRequirements[MapLocation.Lefein].Add(MapChange.Airship);
				_walkableNodes[WalkableRegion.SardaRegion].Add(OverworldTeleportIndex.Lefein);

				// Melmond Island
				mapLocationRequirements[MapLocation.TitansTunnelWest].Clear();
				mapLocationRequirements[MapLocation.TitansTunnelWest].Add(MapChange.Ship | MapChange.Canal);
				_walkableNodes[WalkableRegion.MelmondRegion].Add(OverworldTeleportIndex.TitansTunnelWest);

				mapLocationRequirements[MapLocation.EarthCave1].Clear();
				mapLocationRequirements[MapLocation.EarthCave1].Add(MapChange.Ship | MapChange.Canal);
				mapLocationRequirements[MapLocation.EarthCave1].Add(MapChange.TitanFed);
				_walkableNodes[WalkableRegion.MelmondRegion].Add(OverworldTeleportIndex.EarthCave1);

				mapLocationRequirements[MapLocation.SardasCave].Clear();
				mapLocationRequirements[MapLocation.SardasCave].Add(MapChange.Ship | MapChange.Canal);
				mapLocationRequirements[MapLocation.SardasCave].Add(MapChange.TitanFed);
				_walkableNodes[WalkableRegion.MelmondRegion].Add(OverworldTeleportIndex.SardasCave);

				mapLocationRequirements[MapLocation.CrescentLake].Clear();
				mapLocationRequirements[MapLocation.CrescentLake].Add(MapChange.Ship | MapChange.Canal);
				mapLocationRequirements[MapLocation.CrescentLake].Add(MapChange.TitanFed);
				_walkableNodes[WalkableRegion.MelmondRegion].Add(OverworldTeleportIndex.CrescentLake);

				mapLocationRequirements[MapLocation.Melmond].Clear();
				mapLocationRequirements[MapLocation.Melmond].Add(MapChange.Ship | MapChange.Canal);
				mapLocationRequirements[MapLocation.Melmond].Add(MapChange.TitanFed);
				_walkableNodes[WalkableRegion.MelmondRegion].Add(OverworldTeleportIndex.Melmond);

				floorLocationRequirements[MapLocation.Cardia4] =
	new Tuple<MapLocation, AccessRequirement>(MapLocation.EarthCaveLich, AccessRequirement.Rod);
				mapLocationRequirements = mapLocationRequirements.Where(x => x.Key != MapLocation.Cardia4).ToDictionary(x => x.Key, x => x.Value.ToList());

				// Volcano Island
				mapLocationRequirements[MapLocation.CastleOrdeals1].Clear();
				mapLocationRequirements[MapLocation.CastleOrdeals1].Add(MapChange.Ship | MapChange.Canal | MapChange.Canoe);
				_walkableNodes[WalkableRegion.ElflandRegion].Add(OverworldTeleportIndex.CastleOrdeals1);
				_canoeableNodes[CanoeableRegion.ElflandRegion].Add(OverworldTeleportIndex.CastleOrdeals1);

				mapLocationRequirements[MapLocation.GurguVolcano1].Clear();
				mapLocationRequirements[MapLocation.GurguVolcano1].Add(MapChange.Ship | MapChange.Canal | MapChange.Canoe);
				_walkableNodes[WalkableRegion.ElflandRegion].Add(OverworldTeleportIndex.GurguVolcano1);
				_canoeableNodes[CanoeableRegion.ElflandRegion].Add(OverworldTeleportIndex.GurguVolcano1);

				mapLocationRequirements[MapLocation.IceCave1].Clear();
				mapLocationRequirements[MapLocation.IceCave1].Add(MapChange.Ship | MapChange.Canal | MapChange.Canoe);
				_walkableNodes[WalkableRegion.ElflandRegion].Add(OverworldTeleportIndex.IceCave1);
				_canoeableNodes[CanoeableRegion.ElflandRegion].Add(OverworldTeleportIndex.IceCave1);

				mapLocationRequirements[MapLocation.Cardia1].Clear();
				mapLocationRequirements[MapLocation.Cardia1].Add(MapChange.Ship | MapChange.Canal | MapChange.Canoe);
				_canoeableNodes[CanoeableRegion.ElflandRegion].Add(OverworldTeleportIndex.Cardia1);
				_walkableNodes[WalkableRegion.ElflandRegion].Add(OverworldTeleportIndex.Cardia1);

				mapLocationRequirements[MapLocation.Cardia2].Clear();
				mapLocationRequirements[MapLocation.Cardia2].Add(MapChange.Ship | MapChange.Canal | MapChange.Canoe);
				_canoeableNodes[CanoeableRegion.ElflandRegion].Add(OverworldTeleportIndex.Cardia2);
				_walkableNodes[WalkableRegion.ElflandRegion].Add(OverworldTeleportIndex.Cardia2);
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

		const int teleportXOffset = 0x3F000;
		const int teleportYOffset = 0x3F100;
		const int teleportMapIndexOffset = 0x3F200;

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

        public void UpdatePalettes(OverworldTeleportIndex oti, TeleportDestination teleport)
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

			if (overworldEntryPoint != OverworldTeleportIndex.None) {
			    UpdatePalettes(overworldEntryPoint, teleport);
			}
		}

		public void UpdateOverworldOverride(MapLocation location, OverworldTeleportIndex owti)
		{
			OverriddenOverworldLocations[location] = owti;
			if (ConnectedMapLocations.TryGetValue(location, out var subLocations))
			{
				subLocations.ForEach(sublocation => OverriddenOverworldLocations[sublocation] = owti);
			}
		}

		public void ShuffleEntrancesAndFloors(MT19337 rng, Teleporters teleporters, Flags flags)
		{
			if (((bool)flags.Entrances || (bool)flags.Floors || (bool)flags.Towns) && ((bool)flags.Treasures) && ((bool)flags.NPCItems) && flags.GameMode == GameModes.Standard)
			{
				// Yeah amazing coding, we'll fix it in post
			}
			else
			{
				return;
			}

			_teleporters = teleporters;

			OverriddenOverworldLocations = new Dictionary<MapLocation, OverworldTeleportIndex>
			{
				[MapLocation.StartingLocation] = OverworldTeleportIndex.Coneria,
				[MapLocation.AirshipLocation] = OverworldTeleportIndex.Elfland,
				[MapLocation.Caravan] = OverworldTeleportIndex.Onrac
			};

			// Disabling the Princess Warp back to Castle Coneria moved to FF1Rom

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
				var keepers = _teleporters.VanillaOverworldTeleports.Where(x => x.Key >= OverworldTeleportIndex.Coneria && x.Key <= OverworldTeleportIndex.Lefein).Select(x => x.Key).ToList();

				if (!(bool)flags.EntrancesIncludesDeadEnds)
				{
					keepers.Add(OverworldTeleportIndex.Cardia1);
					keepers.Add(OverworldTeleportIndex.Cardia5);
				}

				if ((bool)flags.IsFloaterRemoved && !(bool)flags.IsAirshipFree)
				{
				    if (!(bool)flags.MapBahamutCardiaDock) {
					keepers.Add(OverworldTeleportIndex.Cardia1);
					keepers.Add(OverworldTeleportIndex.BahamutCave1);
				    }
				    keepers.Add(OverworldTeleportIndex.Cardia2);
				    keepers.Add(OverworldTeleportIndex.Cardia4);
				    keepers.Add(OverworldTeleportIndex.Cardia5);
				    keepers.Add(OverworldTeleportIndex.Cardia6);
				    keepers.Add(OverworldTeleportIndex.TitansTunnelWest);

				    defaultRequirements[MapLocation.SardasCave] = new LocationRequirement(new List<MapChange> { MapChange.TitanFed });
				    defaultRequirements[MapLocation.TitansTunnelWest] = new LocationRequirement(new List<MapChange> { MapChange.TitanFed });
				}

				if (flags.GameMode == GameModes.Standard && flags.OwMapExchange != OwMapExchanges.None) {
				    // Don't move Titan's tunnel on custom/procgen maps
				    keepers.Add(OverworldTeleportIndex.TitansTunnelEast);
				    keepers.Add(OverworldTeleportIndex.TitansTunnelWest);
				}

				placedMaps = placedMaps.Where(x => keepers.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
				placedFloors.Remove(TeleportIndex.SeaShrine1);
				FixUnusedDefaultBackdrops();
			}
			if ((bool)flags.Floors && (bool)flags.Entrances)
			{
				//placedFloors = new Dictionary<TeleportIndex, TeleportDestination>();
				placedFloors = _teleporters.IceCaveLoopTeleporters;
			};

			// All the finished destinations. We don't ever want to make anything new target one of these since some fixed map or floor already does.
			var placedDestinations = placedMaps.Values.Select(x => x.Destination).Concat(placedFloors.Values.Select(x => x.Destination)).ToList();

			// Grab a list of all non-unused overworld entrances, regardless of whether or not we've already placed them.
			var townEntrances = _teleporters.TownEntrances;
			var nonTownEntrances = Enum.GetValues(typeof(OverworldTeleportIndex)).Cast<OverworldTeleportIndex>().Where(x => !townEntrances.Contains(x) && x < OverworldTeleportIndex.Unused1).ToList();
			var shuffledOverworldCount = townEntrances.Count(x => !placedMaps.ContainsKey(x)) + nonTownEntrances.Count(x => !placedMaps.ContainsKey(x));

			// Grab a list of floors we haven't placed yet that can be shuffled. i.e. not including bottom of ice cave or ordeals.
			var towns = _teleporters.TownTeleports.Where(x => !placedDestinations.Contains(x.Destination)).Select(x => new TeleportDestination(x)).ToList();
			var topfloors = _teleporters.NonTownForcedTopFloors.Where(x => !placedDestinations.Contains(x.Destination)).Select(x => new TeleportDestination(x)).ToList();
			var subfloors = _teleporters.FreePlacementFloors.Where(x => !placedDestinations.Contains(x.Destination)).Select(x => new TeleportDestination(x)).ToList();
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
				topfloors = topfloors.Where(floor => floor.Destination != _teleporters.OverworldTeleporters[OverworldTeleportIndex.TempleOfFiends1].Destination).ToList();
				deadEnds.Add(_teleporters.OverworldTeleporters[OverworldTeleportIndex.TempleOfFiends1]);
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
						//UpdateOverworldOverride(floor.Destination, owti);
						if (floor.Exit != ExitTeleportIndex.None)
						{
							// Exiting floors like Fiend Orb Teleporters need to be updated to new OW coords.
							shuffledExits.Add(floor.Exit, _teleporters.OverworldCoordinates[owti]);
						}
					}
				}
			} while (!CheckEntranceSanity(shuffled, (bool)flags.AllowUnsafeStartArea));

			// If the Coneria Entrance goes *directly* to a town, we make its index the one that
			// gets the guaranteed PURE and SOFT.
			switch (shuffled[OverworldTeleportIndex.Coneria].Destination)
			{
				case MapLocation.Pravoka: ConeriaTownEntranceItemShopIndex = 1;	break;
				case MapLocation.Elfland: ConeriaTownEntranceItemShopIndex = 2;	break;
				case MapLocation.CrescentLake: ConeriaTownEntranceItemShopIndex = 3; break;
				case MapLocation.Gaia: ConeriaTownEntranceItemShopIndex = 4; break; // Gaia before Onrac!
				case MapLocation.Onrac: ConeriaTownEntranceItemShopIndex = 5; break;
				default: ConeriaTownEntranceItemShopIndex = 0; break;
			}

			// Pretty print map data
			foreach (var map in shuffled.OrderBy(x => x.Key))
			{
				//PutOverworldTeleport(map.Key, map.Value);
				_teleporters.OverworldTeleporters[map.Key] = new TeleportDestination(map.Value);
				UpdatePalettes(map.Key, map.Value);

				_log.Add($"{map.Key.ToString().PadRight(30)}{map.Value.SpoilerText}");
				var teleports = map.Value.Teleports.ToList();

				while (teleports.Any())
				{
					var teleport = teleports.SpliceRandom(rng);
					var innerMap = shuffledFloors[teleport];
					teleports.AddRange(innerMap.Teleports);
					//_teleporters.StandardMapTeleporters[teleport].SetTeleporter(innerMap.Index, innerMap.Coordinates);
					_teleporters.StandardMapTeleporters[teleport] = new TeleportDestination(innerMap);
					if (map.Key != OverworldTeleportIndex.None)
					{
						UpdatePalettes(map.Key, innerMap);
					}
					//PutStandardTeleport(teleport, innerMap, map.Key);
					_log.Add($"\t{teleport.ToString().PadRight(30)}{innerMap.SpoilerText.Trim()} ({OverworldToPalette[map.Key]} tint)");
				}
			}

			// Write Exit teleport coords back to the ROM
			foreach (var exit in shuffledExits)
			{
				_teleporters.ExitTeleporters[exit.Key] = new TeleportDestination(MapIndex.Cardia, exit.Value);
				/*
				_rom[teleportExitXOffset + (byte)exit.Key] = exit.Value.X;
				_rom[teleportExitYOffset + (byte)exit.Key] = exit.Value.Y;*/
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
			/*
			foreach (var key in titanWalkLocations)
			{
				MapLocationRequirements[key].Add(MapChange.TitanFed);
			}
			foreach (var key in titanCanoeLocations)
			{
				MapLocationRequirements[key].Add(MapChange.TitanFed | MapChange.Canoe);
			}

			UpdateFullLocationRequirements();*/
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

		public Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> FullLocationRequirements;
		public Dictionary<MapLocation, OverworldTeleportIndex> OverriddenOverworldLocations;
		public Dictionary<ObjectId, MapLocation> ObjectiveNPCs;
		public AccessRequirement StartingPotentialAccess;
		public int ConeriaTownEntranceItemShopIndex = 0;

		public const byte GrassTile = 0x00;
		public const byte OceanTile = 0x17;

		public const byte MarshTile = 0x55;
		public const byte MarshTopLeft = 0x62;
		public const byte MarshTopRight = 0x63;
		public const byte MarshBottomLeft = 0x72;
		public const byte MarshBottomRight = 0x73;

		public const byte MountainTopLeft = 0x10;
		public const byte MountainTopMid = 0x11;
		public const byte MountainTopRight = 0x12;
		public const byte MountainMidLeft = 0x20;
		public const byte MountainMid = 0x21;
		public const byte MountainMidRight = 0x22;
		public const byte MountainBottomLeft = 0x30;
		public const byte MountainBottomMid = 0x31;
		public const byte MountainBottomRight = 0x33;

		public const byte RiverTile = 0x44;
		public const byte RiverTopLeft = 0x40;
		public const byte RiverTopRight = 0x41;
		public const byte RiverBottomLeft = 0x50;
		public const byte RiverBottomRight = 0x51;

		public const byte ForestTopLeft = 0x03;
		public const byte ForestTopMid = 0x04;
		public const byte ForestTopRight = 0x05;
		public const byte ForestMidLeft = 0x13;
		public const byte ForestMid = 0x14;
		public const byte ForestMidRight = 0x15;
		public const byte ForestBottomLeft = 0x23;
		public const byte ForestBottomMid = 0x24;
		public const byte ForestBottomRight = 0x25;

		public const byte DockBottomMid = 0x78;
		public const byte DockRightMid = 0x1F;
		public const byte DockLeftMid = 0x0F;

		//public const byte Ocean = 0x17;
		// These are the tiny bits of jaggedness at the edge of a grass tile to make it look nice next to Ocean
		public const byte CoastLeft = 0x16;
		public const byte CoastRight = 0x18;
		public const byte CoastTop = 0x07;
		public const byte CoastBottom = 0x27;

		// The directions here refer to where the grass-side is, so "CoastTopLeft" is placed on the bottom-right
		public const byte CoastTopLeft = 0x06;
		public const byte CoastBottomLeft = 0x26;
		public const byte CoastTopRight = 0x08;
		public const byte CoastBottomRight = 0x28;

		// The special grassy effect around e.g. Elfland or Gaia
		public const byte GrassyMid = 0x54;
		public const byte GrassTopLeft = 0x60;
		public const byte GrassTopRight = 0x61;
		public const byte GrassBottomLeft = 0x70;
		public const byte GrassBottomRight = 0x71;

		public const byte DesertMid = 0x45;
		public const byte DesertTopLeft = 0x42;
		public const byte DesertTopRight = 0x43;
		public const byte DesertBottomLeft = 0x52;
		public const byte DesertBottomRight = 0x53;

		// Reference OverworldTiles.cs in the procGen if you need a tile not yet listed here, e.g. more Docks

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
		public static List<MapEdit> BahamutCardiaDock =
			new List<MapEdit>
			{
			    new MapEdit{X = 0x5f, Y = 0x33, Tile = ForestBottomRight},
			    new MapEdit{X = 0x5f, Y = 0x34, Tile = GrassTile},
			    new MapEdit{X = 0x60, Y = 0x34, Tile = GrassTile},
			    new MapEdit{X = 0x61, Y = 0x34, Tile = GrassTile},
			    new MapEdit{X = 0x62, Y = 0x34, Tile = GrassTile},
			    new MapEdit{X = 0x60, Y = 0x35, Tile = GrassTile},
			    new MapEdit{X = 0x61, Y = 0x35, Tile = DockBottomMid},
			    new MapEdit{X = 0x62, Y = 0x35, Tile = DockBottomMid},
			    new MapEdit{X = 0x63, Y = 0x35, Tile = GrassTile},
			};
		public static List<MapEdit> LefeinRiverDock =
			new List<MapEdit>
			{
			    new MapEdit{X = 0xE0, Y = 0x3A, Tile = RiverTile},
			    new MapEdit{X = 0xE0, Y = 0x3B, Tile = RiverTile},
			    new MapEdit{X = 0xE0, Y = 0x3C, Tile = RiverBottomLeft},
			    new MapEdit{X = 0xE1, Y = 0x3C, Tile = RiverTopRight},
			    new MapEdit{X = 0xE1, Y = 0x3D, Tile = RiverTile},
			    new MapEdit{X = 0xE1, Y = 0x3E, Tile = RiverTile},
			    new MapEdit{X = 0xDF, Y = 0x3B, Tile = ForestTopRight},
			    new MapEdit{X = 0xDF, Y = 0x3C, Tile = ForestMidRight},
			    new MapEdit{X = 0xE0, Y = 0x3D, Tile = ForestTopRight},
			    new MapEdit{X = 0xE0, Y = 0x3E, Tile = ForestBottomRight},
			    new MapEdit{X = 0xE1, Y = 0x3B, Tile = ForestBottomLeft},
			    new MapEdit{X = 0xE2, Y = 0x3C, Tile = ForestMidLeft},
			    new MapEdit{X = 0xE2, Y = 0x3D, Tile = ForestMidLeft},
			    new MapEdit{X = 0xE2, Y = 0x3E, Tile = ForestBottomLeft},
			};
		public static List<MapEdit> BridgeToLefein =
			new List<MapEdit>
			{
					//Top Lefein Side
					new MapEdit{X = 228, Y = 120, Tile = CoastRight},
					new MapEdit{X = 229, Y = 120, Tile = ForestMidLeft},
					new MapEdit{X = 230, Y = 120, Tile = ForestMidRight},
					new MapEdit{X = 231, Y = 120, Tile = CoastLeft},
					new MapEdit{X = 228, Y = 121, Tile = CoastRight},
					new MapEdit{X = 229, Y = 121, Tile = ForestBottomLeft},
					new MapEdit{X = 230, Y = 121, Tile = ForestBottomRight},
					new MapEdit{X = 231, Y = 121, Tile = CoastLeft},
					new MapEdit{X = 229, Y = 122, Tile = CoastTopRight},
					new MapEdit{X = 230, Y = 122, Tile = GrassTile},
					new MapEdit{X = 231, Y = 122, Tile = CoastLeft},
					// Bottom Pravoka side
					new MapEdit{X = 229, Y = 124, Tile = CoastRight},
					new MapEdit{X = 230, Y = 124, Tile = GrassTile},
					new MapEdit{X = 231, Y = 124, Tile = CoastBottomLeft},
					new MapEdit{X = 229, Y = 125, Tile = CoastRight},
					new MapEdit{X = 230, Y = 125, Tile = GrassTile},
					new MapEdit{X = 231, Y = 125, Tile = GrassTile},
					new MapEdit{X = 232, Y = 125, Tile = CoastLeft},
					new MapEdit{X = 229, Y = 126, Tile = CoastRight},
					new MapEdit{X = 230, Y = 126, Tile = GrassTile},
					new MapEdit{X = 231, Y = 126, Tile = GrassTile},
					new MapEdit{X = 232, Y = 126, Tile = CoastLeft},
					// Landbridge above Coneria
					new MapEdit{X = 150, Y = 151, Tile = CoastRight},
					new MapEdit{X = 150, Y = 152, Tile = CoastBottomRight},
					new MapEdit{X = 151, Y = 152, Tile = GrassTile},
					new MapEdit{X = 152, Y = 152, Tile = GrassTile},
					new MapEdit{X = 153, Y = 152, Tile = GrassTile},
					new MapEdit{X = 154, Y = 152, Tile = CoastTopLeft},
					// Delete Matoya Dock
					new MapEdit{X = 156, Y = 141, Tile = GrassTile},
					new MapEdit{X = 157, Y = 141, Tile = GrassTile},
					new MapEdit{X = 158, Y = 141, Tile = GrassTile},
					new MapEdit{X = 159, Y = 141, Tile = GrassTile},
					new MapEdit{X = 156, Y = 142, Tile = CoastTopRight},
					new MapEdit{X = 157, Y = 142, Tile = GrassTile},
					new MapEdit{X = 158, Y = 142, Tile = GrassTile},
					new MapEdit{X = 159, Y = 142, Tile = GrassTile},
			};
			public static List<MapEdit> HighwayToOrdeals =
			new List<MapEdit>
			{
					// Mirage to Ordeals
					new MapEdit{X = 186, Y = 49, Tile = MountainBottomRight},
					new MapEdit{X = 187, Y = 49, Tile = DesertTopLeft},
					new MapEdit{X = 188, Y = 49, Tile = DesertTopRight},
					new MapEdit{X = 189, Y = 49, Tile = MountainBottomLeft},

					new MapEdit{X = 186, Y = 48, Tile = MountainTopRight},
					new MapEdit{X = 187, Y = 48, Tile = GrassBottomLeft},
					new MapEdit{X = 188, Y = 48, Tile = GrassBottomRight},
					new MapEdit{X = 189, Y = 48, Tile = MountainTopLeft},

					new MapEdit{X = 185, Y = 47, Tile = MountainTopRight},
					new MapEdit{X = 186, Y = 47, Tile = GrassBottomLeft},
					new MapEdit{X = 187, Y = 47, Tile = GrassyMid},
					new MapEdit{X = 188, Y = 47, Tile = GrassyMid},
					new MapEdit{X = 189, Y = 47, Tile = GrassTopRight},
					new MapEdit{X = 190, Y = 47, Tile = MountainTopLeft},

					//Lefein to Mirage
					new MapEdit{X = 209, Y = 50, Tile = DesertMid},
					new MapEdit{X = 209, Y = 51, Tile = DesertBottomRight},
					new MapEdit{X = 208, Y = 51, Tile = DesertMid},
					new MapEdit{X = 208, Y = 52, Tile = DesertBottomRight},

					new MapEdit{X = 209, Y = 52, Tile = GrassTile},
					new MapEdit{X = 210, Y = 52, Tile = GrassTile},
					new MapEdit{X = 210, Y = 53, Tile = GrassTile},
					new MapEdit{X = 211, Y = 53, Tile = GrassTile},
					new MapEdit{X = 210, Y = 54, Tile = GrassTile},

					new MapEdit{X = 208, Y = 53, Tile = MountainTopLeft},
					new MapEdit{X = 209, Y = 53, Tile = MountainTopRight},
					new MapEdit{X = 209, Y = 54, Tile = MountainMidRight},

					new MapEdit{X = 210, Y = 51, Tile = MountainBottomLeft},
					new MapEdit{X = 211, Y = 52, Tile = MountainBottomLeft},
			};

		public static List<MapEdit> RiverToMelmond =
			new List<MapEdit>
			{
					//Top Side Mountain
					new MapEdit{X = 84, Y = 146, Tile = CoastBottomRight},
					new MapEdit{X = 80, Y = 147, Tile = CoastBottomRight},
					new MapEdit{X = 77, Y = 149, Tile = CoastBottomRight},
					new MapEdit{X = 78, Y = 148, Tile = CoastBottomRight},
					new MapEdit{X = 83, Y = 146, Tile = CoastBottom},
					new MapEdit{X = 82, Y = 146, Tile = CoastBottom},
					new MapEdit{X = 81, Y = 146, Tile = CoastBottom},
					new MapEdit{X = 79, Y = 147, Tile = CoastBottom},

					new MapEdit{X = 81, Y = 147, Tile = MountainTopLeft},
					new MapEdit{X = 81, Y = 148, Tile = MountainMid},
					new MapEdit{X = 82, Y = 147, Tile = MountainTopMid},
					new MapEdit{X = 83, Y = 147, Tile = MountainTopMid},
					new MapEdit{X = 84, Y = 147, Tile = MountainTopRight},
					new MapEdit{X = 78, Y = 151, Tile = MountainMidLeft},
					new MapEdit{X = 78, Y = 152, Tile = MountainBottomLeft},
					new MapEdit{X = 79, Y = 152, Tile = MountainBottomRight},
					new MapEdit{X = 79, Y = 151, Tile = MountainMidRight},
					new MapEdit{X = 79, Y = 150, Tile = MountainMidRight},
					new MapEdit{X = 80, Y = 149, Tile = MountainBottomMid},
					new MapEdit{X = 78, Y = 149, Tile = MountainTopLeft},
					new MapEdit{X = 80, Y = 148, Tile = MountainTopMid},
					new MapEdit{X = 79, Y = 148, Tile = MountainTopLeft},
					new MapEdit{X = 79, Y = 149, Tile = MountainMid},

					//Bottom Side Mountain
					new MapEdit{X = 86, Y = 151, Tile = CoastTopLeft},
					new MapEdit{X = 87, Y = 150, Tile = CoastTopLeft},
					new MapEdit{X = 85, Y = 152, Tile = CoastTop},
					new MapEdit{X = 88, Y = 149, Tile = MarshBottomLeft},

					new MapEdit{X = 82, Y = 148, Tile = MountainBottomMid},
					new MapEdit{X = 83, Y = 148, Tile = MountainBottomRight},
					new MapEdit{X = 85, Y = 149, Tile = MountainTopLeft},
					new MapEdit{X = 84, Y = 150, Tile = MountainTopMid},
					new MapEdit{X = 84, Y = 151, Tile = MountainBottomMid},
					new MapEdit{X = 85, Y = 151, Tile = MountainBottomRight},
					new MapEdit{X = 83, Y = 150, Tile = MountainTopLeft},
					new MapEdit{X = 81, Y = 149, Tile = MountainBottomRight},
					new MapEdit{X = 86, Y = 147, Tile = MountainTopLeft},
					new MapEdit{X = 87, Y = 147, Tile = MountainTopRight},
					new MapEdit{X = 86, Y = 148, Tile = MountainMidLeft},
					new MapEdit{X = 87, Y = 148, Tile = MountainMidRight},
					new MapEdit{X = 87, Y = 149, Tile = MountainBottomRight},
					new MapEdit{X = 86, Y = 149, Tile = MountainMid},
					new MapEdit{X = 86, Y = 150, Tile = MountainBottomRight},
					new MapEdit{X = 85, Y = 150, Tile = MountainMid},
					new MapEdit{X = 82, Y = 151, Tile = MountainTopMid},
					new MapEdit{X = 81, Y = 151, Tile = MountainTopLeft},
					new MapEdit{X = 81, Y = 152, Tile = MountainMidLeft},
					new MapEdit{X = 81, Y = 153, Tile = MountainBottomLeft},

					// River from Dwarf to Melmond
					new MapEdit{X = 85, Y = 147, Tile = RiverTile},
					new MapEdit{X = 85, Y = 148, Tile = RiverBottomRight},
					new MapEdit{X = 84, Y = 148, Tile = RiverTopLeft},
					new MapEdit{X = 84, Y = 149, Tile = RiverBottomRight},
					new MapEdit{X = 83, Y = 149, Tile = RiverTile},
					new MapEdit{X = 82, Y = 149, Tile = RiverTopLeft},
					new MapEdit{X = 82, Y = 150, Tile = RiverBottomRight},
					new MapEdit{X = 81, Y = 150, Tile = RiverTile},
					new MapEdit{X = 80, Y = 150, Tile = RiverTopLeft},
					new MapEdit{X = 80, Y = 151, Tile = RiverTile},
					new MapEdit{X = 80, Y = 152, Tile = RiverTile},
					new MapEdit{X = 80, Y = 153, Tile = MarshTile},
					new MapEdit{X = 79, Y = 153, Tile = MarshTile},
			};
		public static List<MapEdit> GaiaMountainPass =
			new List<MapEdit>
			{
			    new MapEdit{X = 0xD4, Y = 0x22, Tile = MountainBottomRight},
			    new MapEdit{X = 0xD3, Y = 0x23, Tile = MountainMidRight},
			    new MapEdit{X = 0xD4, Y = 0x23, Tile = GrassTopLeft},
			    new MapEdit{X = 0xD5, Y = 0x23, Tile = GrassyMid},
			    new MapEdit{X = 0xD6, Y = 0x23, Tile = GrassBottomRight},
			    new MapEdit{X = 0xD7, Y = 0x23, Tile = MountainMidLeft},

			    new MapEdit{X = 0xD3, Y = 0x24, Tile = MountainMidRight},
			    new MapEdit{X = 0xD4, Y = 0x24, Tile = GrassBottomLeft},
			    new MapEdit{X = 0xD5, Y = 0x24, Tile = GrassBottomRight},
			    new MapEdit{X = 0xD6, Y = 0x24, Tile = MountainTopLeft},

			    new MapEdit{X = 0xD3, Y = 0x25, Tile = MountainMidRight},
			    new MapEdit{X = 0xD4, Y = 0x25, Tile = GrassTile},
			    new MapEdit{X = 0xD5, Y = 0x25, Tile = MountainTopLeft},
			    new MapEdit{X = 0xD3, Y = 0x26, Tile = MountainMidRight},
			    new MapEdit{X = 0xD4, Y = 0x26, Tile = GrassTile},
			    new MapEdit{X = 0xD5, Y = 0x26, Tile = MountainMidLeft},
			    new MapEdit{X = 0xD3, Y = 0x27, Tile = MountainMidRight},
			    new MapEdit{X = 0xD4, Y = 0x27, Tile = GrassTile},
			    new MapEdit{X = 0xD5, Y = 0x27, Tile = MountainMidLeft},
			    new MapEdit{X = 0xD3, Y = 0x28, Tile = MountainMidRight},
			    new MapEdit{X = 0xD4, Y = 0x28, Tile = GrassTile},
			    new MapEdit{X = 0xD5, Y = 0x28, Tile = MountainBottomLeft},
			    new MapEdit{X = 0xD3, Y = 0x29, Tile = MountainMidRight},
			    new MapEdit{X = 0xD4, Y = 0x29, Tile = GrassTile},
			    new MapEdit{X = 0xD5, Y = 0x29, Tile = CoastTopLeft},
			    new MapEdit{X = 0xD3, Y = 0x2A, Tile = MountainMidRight},
			    new MapEdit{X = 0xD4, Y = 0x2A, Tile = GrassTile},
			    new MapEdit{X = 0xD5, Y = 0x2A, Tile = CoastLeft},
			    new MapEdit{X = 0xD3, Y = 0x2B, Tile = MountainMidRight},
			    new MapEdit{X = 0xD4, Y = 0x2B, Tile = GrassTile},
			    new MapEdit{X = 0xD5, Y = 0x2B, Tile = CoastLeft},
			    new MapEdit{X = 0xD3, Y = 0x2C, Tile = MountainMidRight},
			    new MapEdit{X = 0xD4, Y = 0x2C, Tile = GrassTile},
			    new MapEdit{X = 0xD5, Y = 0x2C, Tile = CoastBottomLeft},
			    new MapEdit{X = 0xD4, Y = 0x2D, Tile = MountainTopRight},
			    new MapEdit{X = 0xD5, Y = 0x2D, Tile = GrassTile},
			    new MapEdit{X = 0xD6, Y = 0x2D, Tile = CoastBottomLeft},
			    new MapEdit{X = 0xD5, Y = 0x2E, Tile = MountainTopRight},
			    new MapEdit{X = 0xD6, Y = 0x2E, Tile = GrassTile},
			    new MapEdit{X = 0xD7, Y = 0x2E, Tile = CoastBottomLeft},
			    new MapEdit{X = 0xD6, Y = 0x2F, Tile = MountainTopRight},
			    new MapEdit{X = 0xD7, Y = 0x2F, Tile = GrassTile},
			    new MapEdit{X = 0xD8, Y = 0x2F, Tile = CoastBottomLeft},
			    new MapEdit{X = 0xD7, Y = 0x30, Tile = MountainTopRight},
			    new MapEdit{X = 0xD8, Y = 0x30, Tile = ForestTopLeft},
			    new MapEdit{X = 0xD9, Y = 0x30, Tile = ForestTopRight},
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
				{ MapIndex.TempleOfFiends, new List<MapIndex> { MapIndex.TempleOfFiendsRevisited1F, MapIndex.TempleOfFiendsRevisited2F,
					MapIndex.TempleOfFiendsRevisited3F, MapIndex.TempleOfFiendsRevisitedEarth, MapIndex.TempleOfFiendsRevisitedFire,
					MapIndex.TempleOfFiendsRevisitedWater, MapIndex.TempleOfFiendsRevisitedAir, MapIndex.TempleOfFiendsRevisitedChaos } }
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
				OverworldTeleportIndex.Coneria, OverworldTeleportIndex.ConeriaCastle1, OverworldTeleportIndex.TempleOfFiends1
			};

		private static readonly Dictionary<MapLocation, (int x, int y)> ObjectiveNPCPositions = new Dictionary<MapLocation, (int x, int y)>
		{
			{ MapLocation.BahamutCave2, (0x15, 0x03) },
			{ MapLocation.Melmond, (0x1A, 0x01) },
			{ MapLocation.ElflandCastle, (0x09, 0x05) },
		};

		private static readonly Dictionary<MapLocation, MapIndex> ObjectiveNPCMapIndexs = new Dictionary<MapLocation, MapIndex>
		{
			{ MapLocation.BahamutCave2, MapIndex.BahamutCaveB2 },
			{ MapLocation.Melmond, MapIndex.Melmond },
			{ MapLocation.ElflandCastle, MapIndex.ElflandCastle },
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

		public void SwapMap(string fileName)
		{
			List<List<byte>> decompressedRows = new List<List<byte>>();

			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var resourcePath = assembly.GetManifestResourceNames().First(str => str.EndsWith(fileName));

			using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
			using (BinaryReader rd = new BinaryReader(stream))
			{
				for (int i = 0; i < 256; i++)
				{
					var row = rd.ReadBytes(256);
					decompressedRows.Add(new List<byte>(row));
				}
			}
			SwapMap(decompressedRows);
		}

	    public void SwapMap(List<List<byte>> decompressedRows)
		{
			_decompressedMap = decompressedRows;
			var recompressedMap = CompressMapRows(decompressedRows);
			PutCompressedMapRows(recompressedMap);
	    }

	    public void SwapMap(List<string> decompressedRows)
		{
			var rows = new List<List<byte>>();
			foreach (var c in decompressedRows) {
			    rows.Add(new List<byte>(Convert.FromBase64String(c)));
			}
			_decompressedMap = rows;
			var recompressedMap = CompressMapRows(rows);
			PutCompressedMapRows(recompressedMap);
	    }

		public void ApplyMapEdits()
		{
			/*var compresedMap = GetCompressedMapRows();
			var decompressedMap = DecompressMapRows(compresedMap);*/
			var editedMap = _decompressedMap;
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

		public static List<List<byte>> CompressMapRows(List<List<byte>> decompressedRows)
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

	    public const int MaximumMapDataSize = 0x3E00;

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

			if (outputOffset > MaximumMapDataSize)
				throw new InvalidOperationException($"Modified map was too large by {outputOffset - MaximumMapDataSize} bytes to recompress and fit into {MaximumMapDataSize} bytes of available space.");
		}
		//  Should be in NPC class
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

				_rom.SetNpc(ObjectiveNPCMapIndexs[location], 0, npc, x, y, inRoom, stationary);
			}
		}

	    public void ShuffleChime(MT19337 rng, bool includeTowns) {
		List<byte[]> dungeons = new List<byte[]> {
		    new byte[] { OverworldTiles.EARTH_CAVE },
		    new byte[] { OverworldTiles.ELFLAND_CASTLE_W, OverworldTiles.ELFLAND_CASTLE_E },
		    new byte[] { OverworldTiles.MIRAGE_BOTTOM },
		    new byte[] { OverworldTiles.ASTOS_CASTLE_W, OverworldTiles.ASTOS_CASTLE_E },
		    new byte[] { OverworldTiles.ICE_CAVE },
		    new byte[] { OverworldTiles.DWARF_CAVE },
		    new byte[] { OverworldTiles.MATOYAS_CAVE },
		    new byte[] { OverworldTiles.TITAN_CAVE_E, OverworldTiles.TITAN_CAVE_W },
		    new byte[] { OverworldTiles.ORDEALS_CASTLE_W, OverworldTiles.ORDEALS_CASTLE_E },
		    new byte[] { OverworldTiles.SARDAS_CAVE },
		    new byte[] { OverworldTiles.TOF_ENTRANCE_W, OverworldTiles.TOF_ENTRANCE_E },
		    new byte[] { OverworldTiles.VOLCANO_TOP_W, OverworldTiles.VOLCANO_TOP_E },
		    new byte[] { OverworldTiles.BAHAMUTS_CAVE },
		    new byte[] { OverworldTiles.MARSH_CAVE }
		};

		List<byte[]> towns = new List<byte[]>{
		    new byte[] { OverworldTiles.PRAVOKA },
		    new byte[] { OverworldTiles.ELFLAND },
		    new byte[] { OverworldTiles.MELMOND },
		    new byte[] { OverworldTiles.CRESCENT_LAKE },
		    new byte[] { OverworldTiles.GAIA },
		    new byte[] { OverworldTiles.ONRAC },
		    new byte[] { OverworldTiles.LEFEIN },
		};

		var candidates = new List<byte[]>(dungeons);

		if (includeTowns) {
		    candidates.AddRange(towns);
		}

		var tileset = new TileSet(this._rom, TileSet.OverworldIndex);
		tileset.LoadData();

		// clear existing
		for (int i = 0; i < tileset.Tiles.Count; i++) {
		    var tp = tileset.Tiles[i].Properties;
		    if ((tp.TilePropFunc & TilePropFunc.OWTP_SPEC_MASK) == TilePropFunc.OWTP_SPEC_CHIME) {
			tp.TilePropFunc &= ~TilePropFunc.OWTP_SPEC_CHIME;
			tileset.Tiles[i].Properties = tp;
		    }
		}

		var chime = candidates.SpliceRandom(rng);

		foreach (var i in chime) {
		    var tp = tileset.Tiles[i].Properties;
		    tp.TilePropFunc |= TilePropFunc.OWTP_SPEC_CHIME;
		    tileset.Tiles[i].Properties = tp;
		}

		tileset.StoreData();
	    }
	}
}
