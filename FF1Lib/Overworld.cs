using FF1Lib.Procgen;
using FF1Lib.Sanity;
using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FF1Lib
{
	public class Overworld
	{

		private const int lut_BtlBackdrops = 0xB300;
		private const int lut_BtlBackdrops_Bank = 0x00;

		private OverworldMap overworldMap;
		private OwMapExchange owMapExchange;
		//private OwMapExchangeData owMapExchangeData;
		private OwLocationData locations;
		private ShipLocations shipLocations;
		private DomainData domains;
		private ZoneFormations zoneFormations;
		private TileSet owTileSet;

		private Flags flags;
		private FF1Rom rom;
		private MT19337 rng;

		public List<Backdrop> BattleBackdrops { get; private set; }
		public TileSet TileSet { get => owTileSet; }
		public List<List<byte>> DecompressedMap { get => overworldMap.MapBytes; }
		public OwMapExchangeData MapExchangeData { get => owMapExchange?.Data; }
		public OwMapExchange MapExchange { get => owMapExchange; }
		public OverworldMap OverworldMap { get => overworldMap; }
		public OwLocationData Locations { get => locations; }
		public ShipLocations ShipLocations { get => shipLocations; }
		public Overworld(FF1Rom _rom, Flags _flags, ZoneFormations _zoneFormations, MT19337 _rng)
		{
			flags = _flags;
			rom = _rom;
			rng = _rng;

			locations = new OwLocationData(rom);
			overworldMap = new OverworldMap(rom, flags);
			domains = new DomainData(rom);
			zoneFormations = _zoneFormations;
			owTileSet = new TileSet(rom, 0xFF);
			//domains.LoadTable();
			//owMapExchange = new(rom, );
			//locations = new OwLocationData(_rom);
			shipLocations = OwMapExchange.GetDefaultShipLocations(locations);
			BattleBackdrops = rom.GetFromBank(lut_BtlBackdrops_Bank, lut_BtlBackdrops, 0x80).ToBytes().Select(b => (Backdrop)b).ToList();
			//_owMapExchange = await OwMapExchange.FromFlags(this, _overworldMap, flags, rng);


		}
		public void Write()
		{
			//domains.StoreTable();
			locations.StoreData();
			owTileSet.StoreData();
			rom.PutInBank(lut_BtlBackdrops_Bank, lut_BtlBackdrops, BattleBackdrops.Select(b => (byte)b).ToArray());
			//overworldMap.
		}
		public async Task LoadMapExchange()
		{
			owMapExchange = await OwMapExchange.FromFlags(rom, overworldMap, flags, rng);
			owMapExchange?.UpdateBridgeSprite();
			if (owMapExchange != null)
			{
				UpdateOverworld(owMapExchange);
				shipLocations = new(locations, owMapExchange.Data.ShipLocations);
			}
			//locations = o
			//shipLocations = owMapExchange?.ShipLocations ?? OwMapExchange.GetDefaultShipLocations(locations);
		}
		public void UpdateOverworld(OwMapExchange owdata)
		{
			if (owdata.Data.DecompressedMapRows != null)
			{
				overworldMap.SwapMap(owdata.Data.DecompressedMapRows);
			}
			else
			{
				overworldMap.SwapMap(owdata.Name + ".ffm");
			}

			locations.GetFrom(owdata.Data);

			owdata.UpdateDomains(zoneFormations);
		}
		public SCCoords SetShipLocation(int dungeonindex)
		{
			return shipLocations.SetShipLocation(dungeonindex);
		}
		public ShipLocation GetShipLocation(int dungeonindex)
		{
			return shipLocations.GetShipLocation(dungeonindex);
		}
		public void Update(Teleporters teleporters)
		{
			shipLocations.UpdateDocks(flags);

			if ((bool)flags.ShuffleChimeAccess)
			{
				ShuffleChime(rng, teleporters, (bool)flags.ShuffleChimeIncludeTowns);
			}

			if ((bool)flags.IsAirshipFree)
			{
				locations.AirShipLocation = locations.StartingLocation;
			}

			if ((bool)flags.MapBridgeLefein && !flags.DisableOWMapModifications)
			{
				locations.BridgeLocation = new SCCoords(230, 123);
			}

			overworldMap.ApplyMapEdits();
		}
		public void ShuffleChime(MT19337 rng, Teleporters teleporters, bool includeTowns)
		{
			List<(OverworldTeleportIndex teleport, List<byte> tiles)> dungeons = new()
			{
				(OverworldTeleportIndex.EarthCave1, new() { OverworldTiles.EARTH_CAVE }),
				(OverworldTeleportIndex.ElflandCastle, new() { OverworldTiles.ELFLAND_CASTLE_W, OverworldTiles.ELFLAND_CASTLE_E }),
				(OverworldTeleportIndex.MirageTower1, new() { OverworldTiles.MIRAGE_BOTTOM }),
				(OverworldTeleportIndex.NorthwestCastle, new() { OverworldTiles.ASTOS_CASTLE_W, OverworldTiles.ASTOS_CASTLE_E }),
				(OverworldTeleportIndex.IceCave1, new() { OverworldTiles.ICE_CAVE }),
				(OverworldTeleportIndex.DwarfCave, new() { OverworldTiles.DWARF_CAVE }),
				(OverworldTeleportIndex.MatoyasCave, new() { OverworldTiles.MATOYAS_CAVE }),
				(OverworldTeleportIndex.TitansTunnelEast, new() { OverworldTiles.TITAN_CAVE_E }),
				//(OverworldTeleportIndex.TitansTunnelWest, new() { OverworldTiles.TITAN_CAVE_W }),
				(OverworldTeleportIndex.CastleOrdeals1, new() { OverworldTiles.ORDEALS_CASTLE_W, OverworldTiles.ORDEALS_CASTLE_E }),
				(OverworldTeleportIndex.SardasCave, new() { OverworldTiles.SARDAS_CAVE }),
				(OverworldTeleportIndex.TempleOfFiends1, new() { OverworldTiles.TOF_ENTRANCE_W, OverworldTiles.TOF_ENTRANCE_E }),
				(OverworldTeleportIndex.GurguVolcano1, new() { OverworldTiles.VOLCANO_TOP_W, OverworldTiles.VOLCANO_TOP_E }),
				(OverworldTeleportIndex.BahamutCave1, new() { OverworldTiles.BAHAMUTS_CAVE }),
				(OverworldTeleportIndex.MarshCave1, new() { OverworldTiles.MARSH_CAVE }),
			};

			List<(OverworldTeleportIndex teleport, List<byte> tiles)> titanWest = new()
			{
				(OverworldTeleportIndex.TitansTunnelWest, new() { OverworldTiles.TITAN_CAVE_W }),
			};

			List<(OverworldTeleportIndex teleport, List<byte> tiles)> towns = new()
			{
				(OverworldTeleportIndex.Pravoka, new() { OverworldTiles.PRAVOKA }),
				(OverworldTeleportIndex.Elfland, new() { OverworldTiles.ELFLAND }),
				(OverworldTeleportIndex.Melmond, new() { OverworldTiles.MELMOND }),
				(OverworldTeleportIndex.CrescentLake, new() { OverworldTiles.CRESCENT_LAKE }),
				(OverworldTeleportIndex.Gaia, new() { OverworldTiles.GAIA }),
				(OverworldTeleportIndex.Onrac, new() { OverworldTiles.ONRAC }),
				(OverworldTeleportIndex.Lefein, new() { OverworldTiles.LEFEIN }),
			};

			// Get entrances leading to Titan's Tunnel
			var titanEntrances = teleporters.OverworldTeleporters.Where(t => t.Value.Index == MapIndex.TitansTunnel).Select(t => t.Key).ToList();

			List<(OverworldTeleportIndex teleport, List<byte> tiles)> candidates = new(dungeons);

			if (includeTowns) candidates.AddRange(towns);

			// Remove Chime requirement from any tiles having it
			var chimeTiles = owTileSet.Tiles.Where(t => (t.Properties.TilePropFunc & TilePropFunc.OWTP_SPEC_MASK) == TilePropFunc.OWTP_SPEC_CHIME).ToList();

			foreach (var tile in chimeTiles)
			{
				var tp = tile.Properties;
				tp.TilePropFunc &= ~TilePropFunc.OWTP_SPEC_CHIME;
				tile.Properties = tp;
			}

			// Pick a valid chime entrance
			bool validChimePlacement = false;
			List<byte> tilesToUpdate = new();
			List<(OverworldTeleportIndex teleport, List<byte> tiles)> canditatesWithTitan = candidates.Concat(titanWest).ToList();

			while (!validChimePlacement)
			{
				var chime = candidates.PickRandom(rng);
				tilesToUpdate = new(chime.tiles);

				// Special check for Titan's Tunnel (we want to block both sides)
				if (titanEntrances.Contains(chime.teleport))
				{
					var potentialEntrances = titanEntrances.Intersect(canditatesWithTitan.Select(c => c.teleport)).ToList();

					// Pick a new candidate if the other entrance isn't valid
					if (potentialEntrances.Count() != 2)
					{
						continue;
					}
					else
					{
						tilesToUpdate = new();
						foreach (var entrance in potentialEntrances)
						{
							tilesToUpdate.AddRange(canditatesWithTitan.Find(t => t.teleport == entrance).tiles);
						}
					}
				}

				validChimePlacement = true;
			}

			// Update tiles
			foreach (var i in tilesToUpdate)
			{
				var tp = owTileSet.Tiles[i].Properties;
				tp.TilePropFunc |= TilePropFunc.OWTP_SPEC_CHIME;
				owTileSet.Tiles[i].Properties = tp;
			}
		}
	}
}
