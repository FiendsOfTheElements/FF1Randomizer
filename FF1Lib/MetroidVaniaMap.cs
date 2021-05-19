using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RomUtilities;

namespace FF1Lib
{
	public partial class FF1Rom
	{
		public enum TeleporterGraphic {
			Upstairs = 0,
			Downstairs,
			LadderDown,
			LadderUp,
			Hole,
			Well,
			Teleporter,
			Door
		}
		private Dictionary<byte, List<byte>> LoadUnusedTileIds(List<Map> maps, FF1Rom rom)
		{
			MapTileSets MapTileSets;
			Dictionary<byte, List<byte>> UnusedTilesbyTileSet;

			MapTileSets = new MapTileSets(rom);
			byte[] possibleTileIds = new byte[128];
			for (byte i = 0; i < 128; i++) possibleTileIds[i] = i;

			// Remove Closed door tile, since it's not used in the map, but still needed
			possibleTileIds[0x37] = 0;  

			UnusedTilesbyTileSet = Enum.GetValues<MapId>()
				.GroupBy(m => MapTileSets[m])
				.Select(t => (t.Key, t.Select(m => maps[(int)m]
						.Select(e => e.Value))
					.SelectMany(x => x)
					.Distinct()
					.ToDictionary(x => x)))
				.Select(t => (t.Key, possibleTileIds.Where(i => !t.Item2.ContainsKey(i)).ToList()))
				.ToDictionary(t => t.Key, t => t.Item2);

			return UnusedTilesbyTileSet;
		}

		public void NoOverworld(OverworldMap overworldmap, List<Map> maps, TalkRoutines talkroutines, NPCdata npcdata, List<MapId> flippedmaps, Flags flags, MT19337 rng)
		{
			LoadInTown(overworldmap);
			ApplyMapMods(maps, flippedmaps, (bool)flags.LefeinSuperStore);
			CreateTeleporters(maps, flippedmaps, rng);
			PrepNPCs(talkroutines, npcdata, flippedmaps, flags, rng);
			UpdateBackgrounds();
		}

		public void LoadInTown(OverworldMap overworldmap)
		{
			// If saved at Inn, spawn directly in the town
			PutInBank(0x1E, 0x9000, Blob.FromHex("2054C4AD10608527AD11608528AD1460854685422025902096C6BD00048544BD0104854560A91E48A9FE48A906484CFDC6"));
			PutInBank(0x1F, 0xC0B7, Blob.FromHex("A91E2003FE200090244510034CE2C1EAEAEAEAEA"));

			// Spawn at coneria castle with new game
			PutInBank(0x00, 0xB010, Blob.FromHex("9298"));

			// Hijack SaveGame to reset scrolls if we didn't come from overworld
			PutInBank(0x0E, 0x9DC0, Blob.FromHex("0000000000000000000000000000000000000000000000000000000000000000A648BDC09D8527BDD09D85284C69AB"));
			PutInBank(0x0E, 0xA53D, Blob.FromHex("20E09D"));

			// Exit/Warp teleport you to Coneria
			PutInBank(0x0E, 0x9DA0, Blob.FromHex("A2FF9AA9928527A9988528A9C048A9BE48A99048A91348A91E4C03FE"));
			PutInBank(0x0E, 0xB0FF, Blob.FromHex("4CA09D"));

			var townTileList = new List<byte> { 0x49, 0x4A, 0x4C, 0x4D, 0x4E, 0x5A, 0x5D, 0x6D };
			var townPosList = new List<(byte, byte)> { (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00) };

			var compresedMap = overworldmap.GetCompressedMapRows();
			var decompressedMap = overworldmap.DecompressMapRows(compresedMap);

			for (int x = 0; x < decompressedMap[0].Count; x++)
			{
				for (int y = 0; y < decompressedMap.Count; y++)
				{
					var tileId = decompressedMap[y][x];
					if (townTileList.Contains(tileId))
					{
						townPosList[townTileList.FindIndex(x => x == tileId)] = ((byte)(x - 7), (byte)(y - 7));
					}
				}
			}

			// Put positions
			PutInBank(0x0E, 0x9DC0, townPosList.Select(x => x.Item1).ToArray());
			PutInBank(0x0E, 0x9DD0, townPosList.Select(x => x.Item2).ToArray());
		}
		public void ApplyMapMods(List<Map> maps, List<MapId> flippedmaps, bool lefeinmart)
		{
			// Coneria
			var coneriaNorthwall = new List<Blob> { Blob.FromHex("0404040404") };
			var coneriaSouthwall = new List<Blob> { Blob.FromHex("0E0E0E0E0E") };

			maps[(int)MapId.Coneria].Put((0x0E, 0x00), coneriaNorthwall.ToArray());
			maps[(int)MapId.Coneria].Put((0x0E, 0x17), coneriaSouthwall.ToArray());
			maps[(int)MapId.Coneria][0x0C, 0x1F] = 0x0E;

			// Pravoka
			var pravokaSouthwall = new List<Blob> {
				Blob.FromHex("0C0E000010101000000E0A"),
				Blob.FromHex("0304040404040404040405")
			};

			maps[(int)MapId.Pravoka].Put((0x0E, 0x1F), pravokaSouthwall.ToArray());

			// Elfland
			var elflandEastwall = new List<Blob> {
				Blob.FromHex("101031"),
				Blob.FromHex("0F0E0E"),
				Blob.FromHex("0F4747"),
				Blob.FromHex("0F4747"),
				Blob.FromHex("0F4747"),
				Blob.FromHex("0F4747"),
				Blob.FromHex("0F4747"),
				Blob.FromHex("0F4747"),
				Blob.FromHex("0F4747"),
				Blob.FromHex("0F4747"),
				Blob.FromHex("0E4747"),
			};

			var elflandWestwall = new List<Blob> {
				Blob.FromHex("0F"),
				Blob.FromHex("0F"),
				Blob.FromHex("0F"),
			};

			var elflandNorthwall = new List<Blob> {
				Blob.FromHex("0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E"),
			};

			var elflandEastwall2 = new List<Blob> {
				Blob.FromHex("0F"),
				Blob.FromHex("0F"),
				Blob.FromHex("0F"),
				Blob.FromHex("0F"),
				Blob.FromHex("0F"),
				Blob.FromHex("0F"),
				Blob.FromHex("0F"),
				Blob.FromHex("0F"),
				Blob.FromHex("0F"),
				Blob.FromHex("0F"),
				Blob.FromHex("0F"),
				Blob.FromHex("0F"),
				Blob.FromHex("0F"),
				Blob.FromHex("0F"),
				Blob.FromHex("0F"),
				Blob.FromHex("0F"),
			};

			maps[(int)MapId.Elfland].Put((0x27, 0x16), elflandEastwall.ToArray());
			maps[(int)MapId.Elfland].Put((0x29, 0x00), elflandEastwall2.ToArray());
			maps[(int)MapId.Elfland].Put((0x10, 0x00), elflandNorthwall.ToArray());
			maps[(int)MapId.Elfland].Put((0x00, 0x04), elflandWestwall.ToArray());
			maps[(int)MapId.Elfland][0x05, 0x04] = 0x00;

			// Melmond
			var melmondWestwall = new List<Blob> {
				Blob.FromHex("4731"),
				Blob.FromHex("470B"),
				Blob.FromHex("470C"),
				Blob.FromHex("470C"),
				Blob.FromHex("470C"),
				Blob.FromHex("470C"),
				Blob.FromHex("470C"),
				Blob.FromHex("470C"),
				Blob.FromHex("470C"),
				Blob.FromHex("470C"),
				Blob.FromHex("470C"),
				Blob.FromHex("470C"),
				Blob.FromHex("470C"),
				Blob.FromHex("470C"),
			};

			var melmondSouthwall = new List<Blob> {
				Blob.FromHex("040404040404"),
			};

			maps[(int)MapId.Melmond].Put((0x01, 0x05), melmondWestwall.ToArray());
			maps[(int)MapId.Melmond].Put((0x0E, 0x1B), melmondSouthwall.ToArray());

			// Crescent Lake
			var crescentSouthwall = new List<Blob> {
				Blob.FromHex("070707"),
			};
			maps[(int)MapId.CrescentLake].Put((0x0A, 0x17), crescentSouthwall.ToArray());
			maps[(int)MapId.CrescentLake][0x00, 0x13] = 0x0E;

			// Gaia
			maps[(int)MapId.Gaia].Replace(0x47, 0x0F);

			// Onrac
			var onracWestwall = new List<Blob> {
				Blob.FromHex("0F0F"),
				Blob.FromHex("0F0F"),
				Blob.FromHex("0F0F"),
				Blob.FromHex("0F0E"),
				Blob.FromHex("0F00"),
				Blob.FromHex("0F0F"),
				Blob.FromHex("0F0F"),
				Blob.FromHex("0F0F"),
			};

			var onracSouthwall = new List<Blob> {
				Blob.FromHex("0F0F"),
				Blob.FromHex("0F0F"),
				Blob.FromHex("0E0E"),
			};

			maps[(int)MapId.Onrac].Put((0x00, 0x08), onracWestwall.ToArray());
			maps[(int)MapId.Onrac].Put((0x10, 0x25), onracSouthwall.ToArray());

			// Lefein
			var lefeinSouthwall = new List<Blob> {
				Blob.FromHex("070707070707070707"),
			};

			var lefeinSouthedge = new List<Blob> {
				Blob.FromHex("0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E"),
			};
			var lefeinSouthedge2 = new List<Blob> {
				Blob.FromHex("0E0E"),
			};

			var lefeinNorthedge = new List<Blob> {
				Blob.FromHex("0E0E0E0E0E0E0E0E0E0E0E"),
			};
			var lefeinNorthedge2 = new List<Blob> {
				Blob.FromHex("0E0E0E0E"),
			};

			var lefeinNonteleport = new List<Blob> {
				Blob.FromHex("0000"),
			};

			maps[(int)MapId.Lefein].Put((0x0F, 0x17), lefeinSouthwall.ToArray());
			maps[(int)MapId.Lefein].Put((0x27, 0x09), lefeinSouthedge.ToArray());
			maps[(int)MapId.Lefein].Put((0x00, 0x09), lefeinSouthedge2.ToArray());
			if (lefeinmart)
			{
				maps[(int)MapId.Lefein].Put((0x24, 0x02), lefeinNorthedge2.ToArray());
				maps[(int)MapId.Lefein].Put((0x00, 0x01), lefeinSouthedge2.ToArray());
				maps[(int)MapId.Lefein].Put((0x00, 0x02), lefeinNonteleport.ToArray());
				maps[(int)MapId.Lefein][0x01, 0x33] = 0x0E;
				maps[(int)MapId.Lefein][0x01, 0x3F] = 0x0E;
			}
			else
			{
				maps[(int)MapId.Lefein].Put((0x26, 0x02), lefeinNorthedge.ToArray());
				maps[(int)MapId.Lefein].Put((0x3C, 0x02), lefeinNorthedge2.ToArray());
				maps[(int)MapId.Lefein].Put((0x00, 0x02), lefeinSouthedge2.ToArray());
				maps[(int)MapId.Lefein][0x01, 0x39] = 0x0E;
			}

			maps[(int)MapId.Lefein][0x03, 0x00] = 0x00;
			maps[(int)MapId.Lefein][0x04, 0x00] = 0x00;

			// Coneria Castle
			var coneriacastleMarshBox = new List<Blob> {
				Blob.FromHex("3031000102"),
				Blob.FromHex("3131030405"),
				Blob.FromHex("3030030405"),
				Blob.FromHex("3131060708"),
				Blob.FromHex("3131303B30"),
				Blob.FromHex("3131313A31"),
			};
			var coneriacastleWaterfallBox = new List<Blob> {
				Blob.FromHex("0001010102"),
				Blob.FromHex("0304040405"),
				Blob.FromHex("0319041A05"),
				Blob.FromHex("0607070708"),
				Blob.FromHex("3030363030"),
				Blob.FromHex("21323A3331"),
			};
			var coneriacastleNorthwall = new List<Blob> { Blob.FromHex("303030") };

			maps[(int)MapId.ConeriaCastle1F].Put((0x14, 0x1B), coneriacastleMarshBox.ToArray());
			maps[(int)MapId.ConeriaCastle1F].Put((0x00, 0x07), coneriacastleWaterfallBox.ToArray());
			maps[(int)MapId.ConeriaCastle1F].Put((0x0B, 0x05), coneriacastleNorthwall.ToArray());
			maps[(int)MapId.ConeriaCastle1F][0x23, 0x0C] = 0x30;

			// Elfland Castle
			var elflandcastleSouthwall = new List<Blob> {
				Blob.FromHex("3031313130"),
				Blob.FromHex("3131313131"),
				Blob.FromHex("3030303030"),
				Blob.FromHex("2121212121"),
				Blob.FromHex("2121212121"),
			};

			var elflandcastleNorthbox = new List<Blob> {
				Blob.FromHex("30300001"),
				Blob.FromHex("21310304"),
				Blob.FromHex("21310607"),
				Blob.FromHex("21313030"),
			};

			var elflandcastleEastwall = new List<Blob> {
				Blob.FromHex("35"),
				Blob.FromHex("33"),
				Blob.FromHex("33"),
				Blob.FromHex("33"),
			};

			maps[(int)MapId.ElflandCastle][0x07, 0x17] = 0x31;
			maps[(int)MapId.ElflandCastle][0x10, 0x02] = 0x30;
			maps[(int)MapId.ElflandCastle].Put((0x0E, 0x1B), elflandcastleSouthwall.ToArray());
			maps[(int)MapId.ElflandCastle].Put((0x11, 0x00), elflandcastleNorthbox.ToArray());
			maps[(int)MapId.ElflandCastle].Put((0x19, 0x03), elflandcastleEastwall.ToArray());

			// Northwest Castle
			var northwestcastleEastwall = new List<Blob> {
				Blob.FromHex("33"),
				Blob.FromHex("33"),
				Blob.FromHex("33"),
				Blob.FromHex("33"),
				Blob.FromHex("33"),
				Blob.FromHex("33"),
				Blob.FromHex("33"),
			};

			maps[(int)MapId.NorthwestCastle][0x18, 0x16] = 0x30;
			maps[(int)MapId.NorthwestCastle][0x17, 0x1C] = 0x30;
			maps[(int)MapId.NorthwestCastle][0x0A, 0x07] = 0x30;
			maps[(int)MapId.NorthwestCastle].Put((0x09, 0x15), coneriacastleNorthwall.ToArray());
			maps[(int)MapId.NorthwestCastle].Put((0x1D, 0x0A), northwestcastleEastwall.ToArray());

			// Ordeals
			var ordealsRoom = new List<Blob> {
				Blob.FromHex("19041A"),
			};

			var ordealsSouthwall = new List<Blob> {
				Blob.FromHex("303030"),
				Blob.FromHex("212121"),
			};

			maps[(int)MapId.CastleOfOrdeals1F][0x05, 0x14] = 0x36;
			maps[(int)MapId.CastleOfOrdeals1F][0x06, 0x14] = 0x3A;
			maps[(int)MapId.CastleOfOrdeals1F].Put((0x15, 0x02), ordealsRoom.ToArray());
			maps[(int)MapId.CastleOfOrdeals1F].Put((0x0B, 0x15), ordealsSouthwall.ToArray());

			// Ice Cave B1
			var iceOnracroom = new List<Blob> {
				Blob.FromHex("0001010102"),
				Blob.FromHex("0313041305"),
				Blob.FromHex("0607070708"),
				flippedmaps.Contains(MapId.IceCaveB1) ? Blob.FromHex("3036303030") : Blob.FromHex("3030303630"),
			};

			if (flippedmaps.Contains(MapId.IceCaveB1))
			{
				maps[(int)MapId.IceCaveB1].Put((0x3F - 0x04, 0x00), iceOnracroom.ToArray());
				maps[(int)MapId.IceCaveB1][0x04, 0x3F - 0x03] = 0x3A;
			}
			else
			{
				maps[(int)MapId.IceCaveB1].Put((0x00, 0x00), iceOnracroom.ToArray());
				maps[(int)MapId.IceCaveB1][0x04, 0x03] = 0x3A;
			}

			// Dwarf's Cave
			var dwarfTunnel1 = new List<Blob> { Blob.FromHex("3D3D3D313D") };
			var dwarfTunnel2 = new List<Blob> { Blob.FromHex("31313131313131313131") };

			maps[(int)MapId.DwarfCave].Put((0x0C, 0x2F), dwarfTunnel1.ToArray());
			maps[(int)MapId.DwarfCave].Put((0x10, 0x36), dwarfTunnel2.ToArray());

			// Matoya's Cave
			var matoyaTileReclaim = new List<Blob> { Blob.FromHex("3C3C3C3C") };
			maps[(int)MapId.MatoyasCave].Put((0x14, 0x0D), matoyaTileReclaim.ToArray());

			// Marsh Cave B1
			var marshConeriaBox = new List<Blob> {
				Blob.FromHex("000102"),
				Blob.FromHex("030405"),
				Blob.FromHex("060708"),
				Blob.FromHex("303B30"),
				Blob.FromHex("403A40"),
			};

			if (flippedmaps.Contains(MapId.MarshCaveB1))
			{

				maps[(int)MapId.MarshCaveB1].Put((0x3F - (0x02 + 0x14), 0x18), marshConeriaBox.ToArray());
				maps[(int)MapId.MarshCaveB1].Put((0x3F - (0x02 + 0x2A), 0x16), marshConeriaBox.ToArray());
			}
			else
			{
				maps[(int)MapId.MarshCaveB1].Put((0x14, 0x18), marshConeriaBox.ToArray());
				maps[(int)MapId.MarshCaveB1].Put((0x2A, 0x16), marshConeriaBox.ToArray());
			}

			// Cardia - Caravan
			var cardiaCaravan = new List<Blob> {
				Blob.FromHex("000101010102"),
				Blob.FromHex("034F04100708"),
				Blob.FromHex("060707083630"),
				Blob.FromHex("343030303A31"),
			};

			maps[(int)MapId.Cardia].Put((0x24, 0x1A), cardiaCaravan.ToArray());
		}
		public void CreateTeleporters(List<Map> maps, List<MapId> flippedmaps, MT19337 rng)
		{
			// Teleporter creation
			// New function from here
			var availableTiles = LoadUnusedTileIds(maps, this);
			List<TeleporterTileSM> newTeleporterTiles = new();

			int FlippedX(MapId map, int pos) => flippedmaps.Contains(map) ? 0x3F - pos : pos;
			void UpdateMapTile(MapId map, int x, int y, byte tile) => maps[(int)map][y, FlippedX(map, x)] = tile;

			// Find valid tiles for Waterfall
			var waterfallValidTiles = maps[(int)MapId.Waterfall].Where(x => x.Tile == (Tile)0x49).ToList();
			var waterfallLefeinStairs = waterfallValidTiles.SpliceRandom(rng);
			var waterfallStairs = maps[(int)MapId.Waterfall].Where(x => x.Tile == (Tile)0x18).First();

			byte teleportIDtracker = 0x41;

			// Coneria
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x0C, 0x22, (byte)MapId.ConeriaCastle1F, false, (int)TileSets.Town, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.Coneria, 0x10, 0x16, newTeleporterTiles.Last().TileID);

			// Pravoka
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x01, 0x0B, (byte)MapId.MatoyasCave, false, (int)TileSets.Town, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.Pravoka, 0x13, 0x1E, newTeleporterTiles.Last().TileID);

			// Elfland
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x10, 0x17, (byte)MapId.ElflandCastle, false, (int)TileSets.Town, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.Elfland, 0x28, 0x16, newTeleporterTiles.Last().TileID);

			// Melmond
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x15, 0x0D, (byte)MapId.SardasCave, false, (int)TileSets.Town, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.Melmond, 0x03, 0x10, newTeleporterTiles.Last().TileID);

			// Crescent Lake
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x19, 0x36, (byte)MapId.DwarfCave, false, (int)TileSets.Town, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.CrescentLake, 0x0B, 0x16, newTeleporterTiles.Last().TileID);
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x1B, 0x0F, (byte)MapId.GurguVolcanoB1, false, (int)TileSets.Town, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.CrescentLake, 0x25, 0x01, newTeleporterTiles.Last().TileID);

			// Gaia
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x16, 0x02, (byte)MapId.CastleOfOrdeals1F, true, (int)TileSets.Town, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.Gaia, 0x29, 0x34, newTeleporterTiles.Last().TileID);
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x11, 0x1F, (byte)MapId.MirageTower1F, false, (int)TileSets.Town, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.Gaia, 0x3D, 0x18, newTeleporterTiles.Last().TileID);

			// Onrac
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x02, 0x01, (byte)MapId.IceCaveB1, true, (int)TileSets.Town, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.Onrac, 0x01, 0x0C, newTeleporterTiles.Last().TileID);

			// Lefein
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, (byte)waterfallLefeinStairs.X, (byte)waterfallLefeinStairs.Y, (byte)MapId.Waterfall, false, (int)TileSets.Town, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.Lefein, 0x13, 0x15, newTeleporterTiles.Last().TileID);

			// Coneria Castle
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x10, 0x16, (byte)MapId.Coneria, false, (int)TileSets.Castle, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.ConeriaCastle1F, 0x0C, 0x22, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x15, 0x19, (byte)MapId.MarshCaveB1, true, (int)TileSets.Castle, TilePalette.RoomPalette1, TeleporterGraphic.LadderDown, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.ConeriaCastle1F, 0x17, 0x1D, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x14, 0x1E, (byte)MapId.TempleOfFiends, false, (int)TileSets.Castle, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.ConeriaCastle1F, 0x02, 0x1D, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, (byte)waterfallStairs.X, (byte)waterfallStairs.Y, (byte)MapId.Waterfall, false, (int)TileSets.Castle, TilePalette.RoomPalette1, TeleporterGraphic.LadderDown, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.ConeriaCastle1F, 0x02, 0x09, newTeleporterTiles.Last().TileID);

			// Elfland Castle
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x28, 0x16, (byte)MapId.Elfland, false, (int)TileSets.Castle, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.ElflandCastle, 0x10, 0x17, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x2B, 0x17, (byte)MapId.MarshCaveB1, true, (int)TileSets.Castle, TilePalette.RoomPalette1, TeleporterGraphic.LadderDown, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.ElflandCastle, 0x14, 0x01, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x0A, 0x14, (byte)MapId.NorthwestCastle, false, (int)TileSets.Castle, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.ElflandCastle, 0x02, 0x02, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x07, 0x01, (byte)MapId.IceCaveB1, false, (int)TileSets.Castle, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.ElflandCastle, 0x0C, 0x11, newTeleporterTiles.Last().TileID);

			// Northwest Castle
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x2D, 0x30, (byte)MapId.MarshCaveB3, false, (int)TileSets.Castle, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.NorthwestCastle, 0x16, 0x17, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x02, 0x02, (byte)MapId.ElflandCastle, false, (int)TileSets.Castle, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.NorthwestCastle, 0x0A, 0x14, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x07, 0x13, (byte)MapId.CastleOfOrdeals1F, false, (int)TileSets.Castle, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.NorthwestCastle, 0x1C, 0x03, newTeleporterTiles.Last().TileID);

			// Ordeals
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x29, 0x34, (byte)MapId.Gaia, false, (int)TileSets.Castle, TilePalette.RoomPalette1, TeleporterGraphic.LadderDown, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.CastleOfOrdeals1F, 0x16, 0x02, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x0B, 0x0E, (byte)MapId.TitansTunnel, false, (int)TileSets.Castle, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.CastleOfOrdeals1F, 0x11, 0x13, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x1C, 0x03, (byte)MapId.NorthwestCastle, false, (int)TileSets.Castle, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.CastleOfOrdeals1F, 0x07, 0x13, newTeleporterTiles.Last().TileID);

			// Temple of Fiends
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x02, 0x1D, (byte)MapId.ConeriaCastle1F, false, (int)TileSets.ToFSeaShrine, TilePalette.OutPalette1, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.TempleOfFiends, 0x14, 0x1E, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x16, 0x0B, (byte)MapId.DwarfCave, false, (int)TileSets.ToFSeaShrine, TilePalette.OutPalette1, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.TempleOfFiends, 0x14, 0x06, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x0F, 0x0B, (byte)MapId.MatoyasCave, false, (int)TileSets.ToFSeaShrine, TilePalette.OutPalette1, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.TempleOfFiends, 0x08, 0x1B, newTeleporterTiles.Last().TileID);

			// Volcano B1
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x25, 0x01, (byte)MapId.CrescentLake, false, (int)TileSets.EarthTitanVolcano, TilePalette.OutPalette1, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.GurguVolcanoB1, 0x1B, 0x0F, newTeleporterTiles.Last().TileID);

			// Ice Cave B1
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x0C, 0x11, (byte)MapId.ElflandCastle, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette1, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.IceCaveB1, 0x07, 0x01, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x01, 0x0C, (byte)MapId.Onrac, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.RoomPalette1, TeleporterGraphic.Hole, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.IceCaveB1, 0x02, 0x01, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x06, 0x1D, (byte)MapId.IceCaveB2, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.RoomPalette1, TeleporterGraphic.Hole, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.IceCaveB1, 0x06, 0x1B, newTeleporterTiles.Last().TileID);

			// Waterfall
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x13, 0x15, (byte)MapId.Lefein, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.Waterfall, waterfallLefeinStairs.X, waterfallLefeinStairs.Y, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x02, 0x09, (byte)MapId.ConeriaCastle1F, true, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.Waterfall, waterfallStairs.X, waterfallStairs.Y, newTeleporterTiles.Last().TileID);

			// Dwarf's Cave
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x14, 0x06, (byte)MapId.TempleOfFiends, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.DwarfCave, 0x16, 0x0B, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x12, 0x0D, (byte)MapId.SardasCave, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.DwarfCave, 0x0C, 0x30, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x0B, 0x16, (byte)MapId.CrescentLake, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.DwarfCave, 0x19, 0x36, newTeleporterTiles.Last().TileID);

			// Matoya
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x13, 0x1E, (byte)MapId.Pravoka, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.MatoyasCave, 0x01, 0x0B, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x08, 0x1B, (byte)MapId.TempleOfFiends, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.MatoyasCave, 0x0F, 0x0B, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x07, 0x07, (byte)MapId.MarshCaveB2, true, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.RoomPalette1, TeleporterGraphic.LadderDown, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.MatoyasCave, 0x0E, 0x02, newTeleporterTiles.Last().TileID);

			// Sarda
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x0C, 0x30, (byte)MapId.DwarfCave, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.SardasCave, 0x12, 0x0D, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x03, 0x10, (byte)MapId.Melmond, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.SardasCave, 0x15, 0x0D, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x05, 0x03, (byte)MapId.TitansTunnel, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.SardasCave, 0x02, 0x0A, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x17, 0x18, (byte)MapId.EarthCaveB1, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.RoomPalette1, TeleporterGraphic.LadderDown, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.SardasCave, 0x15, 0x02, newTeleporterTiles.Last().TileID);

			// Marsh Cave B1
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x17, 0x1D, (byte)MapId.ConeriaCastle1F, true, (int)TileSets.MarshMirage, TilePalette.RoomPalette1, TeleporterGraphic.LadderUp, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.MarshCaveB1, 0x15, 0x19, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x14, 0x01, (byte)MapId.ElflandCastle, true, (int)TileSets.MarshMirage, TilePalette.RoomPalette1, TeleporterGraphic.LadderUp, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.MarshCaveB1, 0x2B, 0x17, newTeleporterTiles.Last().TileID);

			// Mirage 1F [0x18, 0x3D]
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x3D, 0x18, (byte)MapId.Gaia, false, (int)TileSets.MarshMirage, TilePalette.OutPalette1, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.MirageTower1F, 0x11, 0x1F, newTeleporterTiles.Last().TileID);

			// Marsh Cave B2
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x0E, 0x02, (byte)MapId.MatoyasCave, true, (int)TileSets.MarshMirage, TilePalette.RoomPalette1, TeleporterGraphic.LadderUp, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.MarshCaveB2, 0x07, 0x07, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x01, 0x07, (byte)MapId.MarshCaveB1, false, (int)TileSets.MarshMirage, TilePalette.OutPalette1, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.MarshCaveB2, 0x12, 0x10, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x20, 0x34, (byte)MapId.MarshCaveB1, true, (int)TileSets.MarshMirage, TilePalette.RoomPalette1, TeleporterGraphic.LadderUp, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.MarshCaveB2, 0x22, 0x25, newTeleporterTiles.Last().TileID);

			// Marsh Cave B3
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x3A, 0x3A, (byte)MapId.MarshCaveB2, false, (int)TileSets.MarshMirage, TilePalette.OutPalette1, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.MarshCaveB3, 0x05, 0x06, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x16, 0x17, (byte)MapId.NorthwestCastle, false, (int)TileSets.MarshMirage, TilePalette.OutPalette1, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.MarshCaveB3, 0x2D, 0x30, newTeleporterTiles.Last().TileID);

			// Earth B5
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x13, 0x24, (byte)MapId.Cardia, false, (int)TileSets.EarthTitanVolcano, TilePalette.RoomPalette1, TeleporterGraphic.LadderDown, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps, 0x0F));

			// Volcano B2
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x0C, 0x08, (byte)MapId.GurguVolcanoB1, false, (int)TileSets.EarthTitanVolcano, TilePalette.OutPalette1, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.GurguVolcanoB2, 0x1E, 0x20, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x31, 0x06, (byte)MapId.IceCaveB2, true, (int)TileSets.EarthTitanVolcano, TilePalette.RoomPalette1, TeleporterGraphic.LadderUp, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.GurguVolcanoB2, 0x05, 0x02, newTeleporterTiles.Last().TileID);

			// Volcano B5
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x0C, 0x0F, (byte)MapId.Cardia, false, (int)TileSets.EarthTitanVolcano, TilePalette.RoomPalette1, TeleporterGraphic.LadderDown, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps, 0x1F));

			// Ice Cave B2
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x1E, 0x0B, (byte)MapId.IceCaveB1, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.IceCaveB2, 0x1E, 0x02, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x05, 0x04, (byte)MapId.IceCaveB3, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.IceCaveB2, 0x37, 0x05, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x05, 0x02, (byte)MapId.GurguVolcanoB2, true, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.RoomPalette1, TeleporterGraphic.LadderDown, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.IceCaveB2, 0x31, 0x06, newTeleporterTiles.Last().TileID);

			// Ice Cave B3
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x01, 0x1E, (byte)MapId.IceCaveB2, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.IceCaveB3, 0x03, 0x02, newTeleporterTiles.Last().TileID);

			// Mirage 2F
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x0E, 0x1F, (byte)MapId.MirageTower1F, false, (int)TileSets.MarshMirage, TilePalette.OutPalette1, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.MirageTower2F, 0x10, 0x1F, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x25, 0x20, (byte)MapId.SeaShrineB5, true, (int)TileSets.MarshMirage, TilePalette.RoomPalette1, TeleporterGraphic.LadderUp, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.MirageTower2F, 0x10, 0x09, newTeleporterTiles.Last().TileID);

			// Sea Shrine B2
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x31, 0x25, (byte)MapId.SeaShrineB3, false, (int)TileSets.ToFSeaShrine, TilePalette.OutPalette1, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.SeaShrineB2, 0x36, 0x29, newTeleporterTiles.Last().TileID);

			// Sea Shrine B3
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x3D, 0x31, (byte)MapId.SeaShrineB2, false, (int)TileSets.ToFSeaShrine, TilePalette.OutPalette1, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.SeaShrineB3, 0x30, 0x0A, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x3F, 0x06, (byte)MapId.SeaShrineB4, false, (int)TileSets.ToFSeaShrine, TilePalette.OutPalette1, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.SeaShrineB3, 0x2F, 0x27, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x2E, 0x1E, (byte)MapId.Onrac, false, (int)TileSets.ToFSeaShrine, TilePalette.OutPalette1, TeleporterGraphic.Well, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps, 0x41));

			// Sea Shrine B4
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x2F, 0x1D, (byte)MapId.SeaShrineB3, false, (int)TileSets.ToFSeaShrine, TilePalette.OutPalette1, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.SeaShrineB3, 0x2D, 0x14, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x02, 0x02, (byte)MapId.SeaShrineB3, false, (int)TileSets.ToFSeaShrine, TilePalette.OutPalette1, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.SeaShrineB4, 0x3D, 0x31, newTeleporterTiles.Last().TileID);

			// Sea Shrine B5
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x10, 0x09, (byte)MapId.MirageTower2F, true, (int)TileSets.ToFSeaShrine, TilePalette.RoomPalette1, TeleporterGraphic.LadderDown, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.SeaShrineB5, 0x25, 0x20, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x12, 0x03, (byte)MapId.SeaShrineB4, false, (int)TileSets.ToFSeaShrine, TilePalette.OutPalette1, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.SeaShrineB5, 0x32, 0x30, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x1E, 0x12, (byte)MapId.Cardia, false, (int)TileSets.ToFSeaShrine, TilePalette.RoomPalette1, TeleporterGraphic.Teleporter, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps, 0x0F));

			// Titan
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x11, 0x13, (byte)MapId.CastleOfOrdeals1F, false, (int)TileSets.EarthTitanVolcano, TilePalette.OutPalette1, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps, 0x15));

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x02, 0x0A, (byte)MapId.SardasCave, false, (int)TileSets.EarthTitanVolcano, TilePalette.OutPalette1, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps, 0x16));

			// Random Cardia
			List<TeleporterSM> cardiaTeleporters = new();
			List<(byte, byte, byte)> cardiaCoordList = new() {
				(0x3A, 0x37, (byte)MapId.Cardia),
				(0x2B, 0x1D, (byte)MapId.Cardia),
				(0x02, 0x02, (byte)MapId.BahamutsRoomB1),
			};

			cardiaCoordList.Shuffle(rng);

			var waterfallCardiaStairs = waterfallValidTiles.SpliceRandom(rng);
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, cardiaCoordList[0].Item1, cardiaCoordList[0].Item2, cardiaCoordList[0].Item3, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.Waterfall, waterfallCardiaStairs.X, waterfallCardiaStairs.Y, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, cardiaCoordList[1].Item1, cardiaCoordList[1].Item2, cardiaCoordList[1].Item3, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.IceCaveB1, 0x19, 0x12, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, cardiaCoordList[2].Item1, cardiaCoordList[2].Item2, cardiaCoordList[2].Item3, false, (int)TileSets.Town, TilePalette.OutPalette2, TeleporterGraphic.Downstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.Gaia, 0x3D, 0x3E, newTeleporterTiles.Last().TileID);

			foreach (var teleport in newTeleporterTiles)
			{
				teleport.Write(this);
			}

			// Caravan Door
			TileSM CaravanDoor = new TileSM(availableTiles[(byte)TileSets.MatoyaDwarfCardiaIceWaterfall].First(), (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleportTilesGraphics[TeleporterGraphic.Door][(int)TileSets.MatoyaDwarfCardiaIceWaterfall], (byte)(TilePropFunc.TP_SPEC_DOOR), 0x46);
			UpdateMapTile(MapId.Cardia, 0x28, 0x1C, CaravanDoor.ID);
			availableTiles[(byte)TileSets.MatoyaDwarfCardiaIceWaterfall].RemoveRange(0, 1);

			CaravanDoor.Write(this);

			// ToFR Chest in Sky and Sea as a reward 
			List<byte> ToFRchestsList = new() { 0xF8, 0xF9, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE };
			TileSM ExtraChest = new TileSM(availableTiles[(byte)TileSets.SkyCastle].First(), (int)TileSets.SkyCastle, TilePalette.RoomPalette1, new List<byte> { 0x2A, 0x2B, 0x3A, 0x3B }, (byte)(TilePropFunc.TP_SPEC_TREASURE | TilePropFunc.TP_NOMOVE), ToFRchestsList.SpliceRandom(rng));
			UpdateMapTile(MapId.SkyPalace5F, 0x07, 0x01, ExtraChest.ID);
			availableTiles[(byte)TileSets.SkyCastle].RemoveRange(0, 1);

			TileSM ExtraChest2 = new TileSM(availableTiles[(byte)TileSets.MatoyaDwarfCardiaIceWaterfall].First(), (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleportTilesGraphics[TeleporterGraphic.Well][(int)TileSets.MatoyaDwarfCardiaIceWaterfall], (byte)(TilePropFunc.TP_SPEC_TREASURE | TilePropFunc.TP_NOMOVE), ToFRchestsList.SpliceRandom(rng));
			UpdateMapTile(MapId.Cardia, 0x2A, 0x14, ExtraChest2.ID);
			availableTiles[(byte)TileSets.MatoyaDwarfCardiaIceWaterfall].RemoveRange(0, 1);

			ExtraChest.Write(this);
			ExtraChest2.Write(this);

			// Reset spawning position for Coneria Castle and towns
			Data[0x02C01 + (int)MapId.ConeriaCastle1F] = 0x0C;
			Data[0x02C21 + (int)MapId.ConeriaCastle1F] = 0x1E;

			Data[0x02C01 + (int)MapId.Coneria] = 0x0B;
			Data[0x02C21 + (int)MapId.Coneria] = 0x13;

			Data[0x02C01 + (int)MapId.Pravoka] = 0x13;
			Data[0x02C21 + (int)MapId.Pravoka] = 0x05;

			Data[0x02C01 + (int)MapId.Elfland] = 0x24;
			Data[0x02C21 + (int)MapId.Elfland] = 0x15;

			Data[0x02C01 + (int)MapId.Melmond] = 0x09;
			Data[0x02C21 + (int)MapId.Melmond] = 0x0E;

			Data[0x02C01 + (int)MapId.CrescentLake] = 0x0B;
			Data[0x02C21 + (int)MapId.CrescentLake] = 0x14;

			Data[0x02C01 + (int)MapId.Gaia] = 0x21;
			Data[0x02C21 + (int)MapId.Gaia] = 0x35;

			Data[0x02C01 + (int)MapId.Onrac] = 0x06;
			Data[0x02C21 + (int)MapId.Onrac] = 0x15;

			Data[0x02C01 + (int)MapId.Lefein] = 0x11;
			Data[0x02C21 + (int)MapId.Lefein] = 0x07;

		}

		public void PrepNPCs(TalkRoutines talkroutines, NPCdata npcdata, List<MapId> flippedmaps, Flags flags, MT19337 rng)
		{
			// New Talk routines
			var talk_Floater = talkroutines.Add(Blob.FromHex("AD2B60D003A57160A476207392A57260"));
			var talk_Chime = talkroutines.Add(Blob.FromHex("AD2C60D003A57160A476207392A57260"));
			var talk_Canoe = talkroutines.Add(Blob.FromHex("AD1260D003A57160A476207392A57260"));

			// Orbs
			SetNpc(MapId.ConeriaCastle1F, 0x02, ObjectId.ConeriaCastle1FWoman1, 0x02, 0x09, true, true); // Dialog+Routine
			Data[MapObjGfxOffset + (byte)ObjectId.ConeriaCastle1FWoman1] = 0x04;
			npcdata.SetRoutine(ObjectId.ConeriaCastle1FWoman1, (newTalkRoutines)talk_Floater);
			npcdata.GetTalkArray(ObjectId.ConeriaCastle1FWoman1)[(int)TalkArrayPos.dialogue_2] = 0x37;
			npcdata.GetTalkArray(ObjectId.ConeriaCastle1FWoman1)[(int)TalkArrayPos.dialogue_3] = 0x36;

			SetNpc(MapId.CastleOfOrdeals1F, 0x01, ObjectId.LefeinMan10, 0x16, 0x02, true, true); //Dialog+Routine
			SetNpc(MapId.Lefein, 0x0A, 0x00, 0x16, 0x02, true, true);
			Data[MapObjGfxOffset + (byte)ObjectId.LefeinMan10] = 0x04;
			npcdata.SetRoutine(ObjectId.LefeinMan10, (newTalkRoutines)talk_Floater);
			npcdata.GetTalkArray(ObjectId.LefeinMan10)[(int)TalkArrayPos.dialogue_2] = 0x37;
			npcdata.GetTalkArray(ObjectId.LefeinMan10)[(int)TalkArrayPos.dialogue_3] = 0x36;

			SetNpc(MapId.IceCaveB1, 0x01, ObjectId.LefeinMan6, flippedmaps.Contains(MapId.IceCaveB1) ? 0x3F - 0x02 : 0x02, 0x01, true, true); //Dialog+Routine
			SetNpc(MapId.Lefein, 0x06, 0x00, 0x16, 0x02, true, true);
			Data[MapObjGfxOffset + (byte)ObjectId.LefeinMan6] = 0x04;
			npcdata.SetRoutine(ObjectId.LefeinMan6, (newTalkRoutines)talk_Floater);
			npcdata.GetTalkArray(ObjectId.LefeinMan6)[(int)TalkArrayPos.dialogue_2] = 0x37;
			npcdata.GetTalkArray(ObjectId.LefeinMan6)[(int)TalkArrayPos.dialogue_3] = 0x36;

			// Canoe people
			SetNpc(MapId.CrescentLake, 0x0D, ObjectId.CrescentWoman, 0x25, 0x02, false, true); // Dialog+Routine
			npcdata.SetRoutine(ObjectId.CrescentWoman, (newTalkRoutines)talk_Canoe);
			npcdata.GetTalkArray(ObjectId.CrescentWoman)[(int)TalkArrayPos.dialogue_2] = 0x5C;
			npcdata.GetTalkArray(ObjectId.CrescentWoman)[(int)TalkArrayPos.dialogue_3] = 0xC2;

			SetNpc(MapId.ElflandCastle, 0x03, ObjectId.ElflandCastleElf2, 0x0E, 0x11, false, true); // Dialog+Routine
			npcdata.SetRoutine(ObjectId.ElflandCastleElf2, (newTalkRoutines)talk_Canoe);
			npcdata.GetTalkArray(ObjectId.ElflandCastleElf2)[(int)TalkArrayPos.dialogue_2] = 0x5C;
			npcdata.GetTalkArray(ObjectId.ElflandCastleElf2)[(int)TalkArrayPos.dialogue_3] = 0xC2;

			SetNpc(MapId.CastleOfOrdeals1F, 0x00, ObjectId.CastleOrdealsOldMan, 0x02, 0x02, true, true); //Dialog+Routine.
			npcdata.SetRoutine(ObjectId.CastleOrdealsOldMan, (newTalkRoutines)talk_Canoe);
			npcdata.GetTalkArray(ObjectId.CastleOrdealsOldMan)[(int)TalkArrayPos.dialogue_2] = 0x5C;
			if ((bool)flags.EarlyOrdeals)
			{
				npcdata.GetTalkArray(ObjectId.CastleOrdealsOldMan)[(int)TalkArrayPos.dialogue_3] = 0xC2;
			}
			else
			{
				npcdata.GetTalkArray(ObjectId.CastleOrdealsOldMan)[(int)TalkArrayPos.dialogue_3] = 0x2D;
			}

			// Coneria Castle
			SetNpc(MapId.ConeriaCastle1F, 0x04, ObjectId.ConeriaCastle1FGuard2, 0x15, 0x1F, false, true); // Dialog+Routine
			npcdata.SetRoutine(ObjectId.ConeriaCastle1FGuard2, newTalkRoutines.Talk_norm);
			npcdata.GetTalkArray(ObjectId.ConeriaCastle1FGuard2)[(int)TalkArrayPos.dialogue_2] = 0x35;

			SetNpc(MapId.Cardia, 0x09, ObjectId.ConeriaCastle1FScholar, 0x26, 0x1B, true, true); // No check
			npcdata.SetRoutine(ObjectId.ConeriaCastle1FScholar, newTalkRoutines.Talk_norm);
			npcdata.GetTalkArray(ObjectId.ConeriaCastle1FScholar)[(int)TalkArrayPos.dialogue_2] = 0xA1;

			// Chime bot
			SetNpc(MapId.Gaia, 0x03, ObjectId.GaiaScholar2, 0x35, 0x1A, false, true); //Dialog+Routine
			Data[MapObjGfxOffset + (byte)ObjectId.GaiaScholar2] = 0x15;
			npcdata.SetRoutine(ObjectId.GaiaScholar2, (newTalkRoutines)talk_Chime);
			npcdata.GetTalkArray(ObjectId.GaiaScholar2)[(int)TalkArrayPos.dialogue_2] = 0xD5;
			npcdata.GetTalkArray(ObjectId.GaiaScholar2)[(int)TalkArrayPos.dialogue_3] = 0xD9;

			// Switch Key dialogue
			npcdata.GetTalkArray(ObjectId.ConeriaCastle1FOldMan2)[(int)TalkArrayPos.dialogue_2] = 0x40;

			MoveNpc(MapId.DwarfCave, 0x00, 0x0F, 0x2F, false, true);
			MoveNpc(MapId.NorthwestCastle, 0x02, 0x1C, 0x01, false, false);
			MoveNpc(MapId.Onrac, 0x09, 0x11, 0x23, false, false);

			UpdateItemName(Item.Floater, "SIGIL  ");

			Put(0x2B5F4, FF1Text.TextToBytes("MARK", useDTE: false));

			Dictionary<int, string> newDialogues = new() {
				{ 0x35, "A foul smell seeps in\nfrom this door, as if\nsome rotting bog stews\non the other side." },
				{ 0x42,	"This door oozes with\nputrid molds, stopping\nits invasion from a\nnauseating cave." },

				{ 0x36, "The SIGIL unlocks the\nmagic barrier and a new\npath is revealed." },
				{ 0x37,	"The force field emits\na wintry glow as arcane\nsymbols swirl inside it.\nWhat runes could make\nit vanish?" },
				{ 0xA1,	"Can you hear this sound?\nThis is the rumblings of\nan ancient waterfall\nhidden below the castle." },
				
				{ 0x5C,	"Only those who received\nLukahn's blessing may\ngo beyond this point." },
				{ 0xD5,	"Only the owner of the\nCHIME can rightfully\nenter Mirage." }, // Gaia Robot
				{ 0xD9,	"That sound...\nEXECUTE STEP ASIDE\nROUTINE." }, // Gaia Robot
				{ 0xC2,	"You wear Lukahn's mark,\nLIGHT WARRIORS. May it\nprotect you as you enter\nthese vile premises." },

				{ 0x2D,	"So you have Lukahn's\nfavor, but know that\nyou won't go far\nwithout the CROWN!" }, // Ordeals

				{ 0x13,	"A rock blocks\nconstruction of my\ntunnel.\nIf I only had TNT." }, // Nerrick
				{ 0x14,	"Oh, wonderful!\nNice work! Yes, yes\nindeed, this TNT is just\nwhat I need to finish my\ntunnel. Now excuse me\nwhile I get to work!" }, // Nerrick

				{ 0x2C,	"I am Lukahn.\nNow all legends and\nprophecy will be\nfulfilled. Our path has\nbeen decided.\nCome back to me once the\nEarth FIEND is vanquised." }, // Lukkanh
				{ 0x8C,	"400 years ago, we lost\ncontrol of the Wind.\n200 years later we lost\nthe Water,\nthen Earth,\nand Fire followed. The\nPowers that bind this\nworld are gone." }, // Lukkanh

			};

			if ((bool)!flags.NPCItems)
			{
				newDialogues.Add(0x2B, "Great job vanquishing\nthe Earth FIEND. Now,\nthe Fire FIEND wakes.\nWith my blessing; go to\nthe VOLCANO, and defeat\nthat FIEND also!");
			}

			InsertDialogs(newDialogues);

			// Palettes changes
			PutInBank(0x00, 0xA000 + ((byte)MapId.IceCaveB1 * 0x30) + 0x18, Blob.FromHex("0031000100003101"));
		}

		public void UpdateBackgrounds()
		{
			var lut_BtlBackdrops = 0xB300;
			var lut_BtlBackdrops_Bank = 0x00;

			List<(MapId, Backdrop)> backgroundList = new()
			{
				(MapId.Coneria, Backdrop.Grass),
				(MapId.Pravoka, Backdrop.Water),
				(MapId.Elfland, Backdrop.Grass),
				(MapId.Melmond, Backdrop.Grass),
				(MapId.CrescentLake, Backdrop.Grass),
				(MapId.Gaia, Backdrop.Grass),
				(MapId.Onrac, Backdrop.Forest),
				(MapId.Lefein, Backdrop.Forest),
				(MapId.ConeriaCastle1F, Backdrop.Castle),
				(MapId.ElflandCastle, Backdrop.Castle),
				(MapId.NorthwestCastle, Backdrop.Castle),
				(MapId.CastleOfOrdeals1F, Backdrop.Castle),
				(MapId.TempleOfFiends, Backdrop.TempleOfFiends),
				(MapId.EarthCaveB1, Backdrop.EarthCave),
				(MapId.GurguVolcanoB1, Backdrop.Volcano),
				(MapId.IceCaveB1, Backdrop.IceCave),
				(MapId.Cardia, Backdrop.Cave),
				(MapId.BahamutsRoomB1, Backdrop.Cave),
				(MapId.Waterfall, Backdrop.Waterfall),
				(MapId.DwarfCave, Backdrop.MarshCave),
				(MapId.MatoyasCave, Backdrop.Cave),
				(MapId.SardasCave, Backdrop.Cave),
				(MapId.MarshCaveB1, Backdrop.MarshCave),
				(MapId.MirageTower1F, Backdrop.Tower),
				(MapId.ConeriaCastle2F, Backdrop.Castle),
				(MapId.CastleOfOrdeals2F, Backdrop.Castle),
				(MapId.CastleOfOrdeals3F, Backdrop.Castle),
				(MapId.MarshCaveB2, Backdrop.MarshCave),
				(MapId.MarshCaveB3, Backdrop.MarshCave),
				(MapId.EarthCaveB2, Backdrop.EarthCave),
				(MapId.EarthCaveB3, Backdrop.EarthCave),
				(MapId.EarthCaveB4, Backdrop.EarthCave),
				(MapId.EarthCaveB5, Backdrop.EarthCave),
				(MapId.GurguVolcanoB2, Backdrop.Volcano),
				(MapId.GurguVolcanoB3, Backdrop.Volcano),
				(MapId.GurguVolcanoB4, Backdrop.Volcano),
				(MapId.GurguVolcanoB5, Backdrop.Volcano),
				(MapId.IceCaveB2, Backdrop.IceCave),
				(MapId.IceCaveB3, Backdrop.IceCave),
				(MapId.BahamutsRoomB2, Backdrop.Cave),
				(MapId.MirageTower2F, Backdrop.Tower),
				(MapId.MirageTower3F, Backdrop.Tower),
				(MapId.SeaShrineB5, Backdrop.SeaShrine),
				(MapId.SeaShrineB4, Backdrop.SeaShrine),
				(MapId.SeaShrineB3, Backdrop.SeaShrine),
				(MapId.SeaShrineB2, Backdrop.SeaShrine),
				(MapId.SeaShrineB1, Backdrop.SeaShrine),
				(MapId.SkyPalace1F, Backdrop.Tower),
				(MapId.SkyPalace2F, Backdrop.Tower),
				(MapId.SkyPalace3F, Backdrop.Tower),
				(MapId.SkyPalace4F, Backdrop.Tower),
				(MapId.SkyPalace5F, Backdrop.Tower),
				(MapId.TempleOfFiendsRevisited1F, Backdrop.TempleOfFiends),
				(MapId.TempleOfFiendsRevisited2F, Backdrop.TempleOfFiends),
				(MapId.TempleOfFiendsRevisited3F, Backdrop.TempleOfFiends),
				(MapId.TempleOfFiendsRevisitedEarth, Backdrop.TempleOfFiends),
				(MapId.TempleOfFiendsRevisitedFire, Backdrop.TempleOfFiends),
				(MapId.TempleOfFiendsRevisitedWater, Backdrop.TempleOfFiends),
				(MapId.TempleOfFiendsRevisitedAir, Backdrop.TempleOfFiends),
				(MapId.TempleOfFiendsRevisitedChaos, Backdrop.TempleOfFiends),
				(MapId.TitansTunnel, Backdrop.Cave),
			};

			PutInBank(0x1F, 0xEA2D, Blob.FromHex("A648")); // Use current map as ID for loading background, instead of ow tile
			PutInBank(0x1F, 0xEB81, Blob.FromHex("A648"));
			PutInBank(lut_BtlBackdrops_Bank, lut_BtlBackdrops, backgroundList.Select(x => (byte)x.Item2).ToArray());
		}

		public class TileSM
		{
			private byte _attribute;
			private byte _TSAul;
			private byte _TSAur;
			private byte _TSAdl;
			private byte _TSAdr;
			private byte _property1;
			private byte _property2;
			private int _tileSetOrigin;
			private byte _tileSetID;

			const int BANK_SMINFO = 0x00;
			const int lut_TileSMsetAttr = 0x8400; // BANK_SMINFO - must be on $400 byte bound  - 0x80 x8
			const int lut_TileSMsetProp = 0x8800; // BANK_SMINFO - page                        - 0x100 bytes x 8  (2 bytes per)
			const int lut_TileSMsetTSA = 0x9000;  // BANK_SMINFO - page                        - 0x80 bytes x4 x8 => ul, ur, dl, dr
			//const int lut_SMPalettes = 0xA000;    // BANK_SMINFO - $1000 byte bound            - 0x30 bytes x8?

			public TilePalette Palette
			{
				get { return (TilePalette)_attribute; }
				set { _attribute = (byte)value; }
			}
			public List<byte> TileGraphic
			{
				get { return new List<byte> { _TSAul, _TSAur, _TSAdl, _TSAdr }; }
				set
				{
					_TSAul = value[0];
					_TSAur = value[1];
					_TSAdl = value[2];
					_TSAdr = value[3];
				}
			}
			public byte PropertyType
			{
				get { return _property1; }
				set
				{
					_property1 = value;
				}
			}
			public byte PropertyValue
			{
				get { return _property2; }
				set
				{
					_property2 = value;
				}
			}
			public byte ID
			{
				get { return _tileSetID; }
				set
				{
					_tileSetID = value;
				}
			}
			public int TileSet
			{
				get { return _tileSetOrigin; }
				set
				{
					_tileSetOrigin = value;
				}
			}
			public TileSM(byte id, int tileset, TilePalette palette, List<byte> tilegraphics, byte property1, byte property2)
			{
				_tileSetID = id;
				_tileSetOrigin = tileset;
				_attribute = (byte)palette;
				_property1 = property1;
				_property2 = property2;
				_TSAul = tilegraphics[0];
				_TSAur = tilegraphics[1];
				_TSAdl = tilegraphics[2];
				_TSAdr = tilegraphics[3];
			}
			public TileSM(byte id, int tileset, FF1Rom rom)
			{
				Read(id, tileset, rom);
			}
			public void Write(FF1Rom rom)
			{
				rom.PutInBank(BANK_SMINFO, lut_TileSMsetAttr + (_tileSetOrigin * 0x80) + _tileSetID, new byte[] { _attribute });
				rom.PutInBank(BANK_SMINFO, lut_TileSMsetProp + (_tileSetOrigin * 0x100) + (_tileSetID * 2), new byte[] { _property1, _property2 });
				rom.PutInBank(BANK_SMINFO, lut_TileSMsetTSA + (_tileSetOrigin * 0x200) + _tileSetID, new byte[] { _TSAul });
				rom.PutInBank(BANK_SMINFO, lut_TileSMsetTSA + (_tileSetOrigin * 0x200) + 0x80 + _tileSetID, new byte[] { _TSAur });
				rom.PutInBank(BANK_SMINFO, lut_TileSMsetTSA + (_tileSetOrigin * 0x200) + 0x100 + _tileSetID, new byte[] { _TSAdl });
				rom.PutInBank(BANK_SMINFO, lut_TileSMsetTSA + (_tileSetOrigin * 0x200) + 0x180 + _tileSetID, new byte[] { _TSAdr });
			}
			public void Read(byte id, int tileset, FF1Rom rom)
			{
				_tileSetID = id;
				_tileSetOrigin = tileset;
				_attribute = rom.GetFromBank(BANK_SMINFO, lut_TileSMsetAttr + (_tileSetOrigin * 0x80) + _tileSetID, 1)[0];
				_property1 = rom.GetFromBank(BANK_SMINFO, lut_TileSMsetProp + (_tileSetOrigin * 0x100) + (_tileSetID * 2), 2)[0];
				_property2 = rom.GetFromBank(BANK_SMINFO, lut_TileSMsetProp + (_tileSetOrigin * 0x100) + (_tileSetID * 2), 2)[1];
				_TSAul = rom.GetFromBank(BANK_SMINFO, lut_TileSMsetTSA + (_tileSetOrigin * 0x200) + _tileSetID, 1)[0];
				_TSAur = rom.GetFromBank(BANK_SMINFO, lut_TileSMsetTSA + (_tileSetOrigin * 0x200) + 0x80 + _tileSetID, 1)[0];
				_TSAdl = rom.GetFromBank(BANK_SMINFO, lut_TileSMsetTSA + (_tileSetOrigin * 0x200) + 0x100 + _tileSetID, 1)[0];
				_TSAdr = rom.GetFromBank(BANK_SMINFO, lut_TileSMsetTSA + (_tileSetOrigin * 0x200) + 0x180 + _tileSetID, 1)[0];
			}
		}

		public class TeleporterSM
		{
			private byte _x;
			private byte _y;
			private byte _target;
			private int _id;
			private bool _inroom;

			const int BANK_TELEPORTINFO = 0x0F;
			const int lut_NormTele_X_ext = 0xB000;
			const int lut_NormTele_Y_ext = 0xB100;
			const int lut_NormTele_Map_ext = 0xB200;
			//const int NormTele_ext_qty = 0x100;

			public byte X
			{
				get { return (byte)(_x & 0b01111111); }
				set
				{
					_x = (byte)(value | (_inroom ? 0b10000000 : 0b00000000));
				}
			}
			public byte Y
			{
				get { return (byte)(_y & 0b01111111); }
				set
				{
					_y = (byte)(value | 0b10000000);
				}
			}
			public byte Destination
			{
				get { return _target; }
				set
				{
					_target = value;
				}
			}
			public int ID
			{
				get { return _id; }
				set
				{
					_id = value;
				}
			}
			public TeleporterSM(int id, byte x, byte y, byte destination, bool inroom)
			{
				_id = id;
				_inroom = inroom;
				_x = (byte)(x | (_inroom ? 0b10000000 : 0b00000000));
				_y = (byte)(y | 0b10000000);
				_target = destination;
			}
			public void Write(FF1Rom rom)
			{
				rom.PutInBank(BANK_TELEPORTINFO, lut_NormTele_X_ext + _id, new byte[] { _x });
				rom.PutInBank(BANK_TELEPORTINFO, lut_NormTele_Y_ext + _id, new byte[] { _y });
				rom.PutInBank(BANK_TELEPORTINFO, lut_NormTele_Map_ext + _id, new byte[] { _target });
			}
		}
		public class TeleporterTileSM
		{
			private TeleporterSM _teleporter;
			private TileSM _tile;

			public TeleporterTileSM(int teleporter_id, byte target_x, byte target_y, byte destination, bool inroom, int tileset, TilePalette palette, TeleporterGraphic graphic, byte property1, Dictionary<byte, List<byte>> freetiles, List<MapId> flippedmaps, byte tile_id = 0x00)
			{
				_teleporter = new TeleporterSM(teleporter_id, flippedmaps.Contains((MapId)destination) ? (byte)(0x3F - target_x) : target_x, target_y, destination, inroom);
				if (tile_id > 0)
				{
					_tile = new TileSM(tile_id, tileset, palette, TeleportTilesGraphics[graphic][tileset], property1, (byte)teleporter_id);
				}
				else
				{
					_tile = new TileSM(freetiles[(byte)tileset].First(), tileset, palette, TeleportTilesGraphics[graphic][tileset], property1, (byte)teleporter_id);
					freetiles[(byte)tileset].RemoveAt(0);
				}
			}
			public byte TileID
			{
				get { return _tile.ID; }
			}
			public void Write(FF1Rom rom)
			{
				_teleporter.Write(rom);
				_tile.Write(rom);
			}
		}

		public enum TilePalette : byte
		{
			RoomPalette1 = 0x00,
			RoomPalette2 = 0x55,
			OutPalette1 = 0xAA,
			OutPalette2 = 0xFF,
		}

		public enum TileSets
		{
			Town = 0,
			Castle,
			EarthTitanVolcano,
			MatoyaDwarfCardiaIceWaterfall,
			MarshMirage,
			ToFSeaShrine,
			SkyCastle,
			ToFR
		}

		public static Dictionary<TeleporterGraphic, List<List<byte>>> TeleportTilesGraphics = new()
		{
			{
				TeleporterGraphic.Upstairs,
				new List<List<byte>> {
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x26, 0x27, 0x36, 0x37 },
					new List<byte> { 0x26, 0x27, 0x36, 0x37 },
					new List<byte> { 0x26, 0x27, 0x36, 0x37 },
					new List<byte> { 0x26, 0x27, 0x36, 0x37 },
					new List<byte> { 0x26, 0x27, 0x36, 0x37 },
					new List<byte> { 0x26, 0x27, 0x36, 0x37 },
					new List<byte> { 0x26, 0x27, 0x36, 0x37 },
				}
			},

			{
				TeleporterGraphic.Downstairs,
				new List<List<byte>> {
					new List<byte> { 0x04, 0x05, 0x14, 0x15 },
					new List<byte> { 0x28, 0x29, 0x38, 0x39 },
					new List<byte> { 0x28, 0x29, 0x38, 0x39 },
					new List<byte> { 0x28, 0x29, 0x38, 0x39 },
					new List<byte> { 0x28, 0x29, 0x38, 0x39 },
					new List<byte> { 0x28, 0x29, 0x38, 0x39 },
					new List<byte> { 0x62, 0x63, 0x72, 0x73 },
					new List<byte> { 0x28, 0x29, 0x38, 0x39 },
				}
			},

			{
				TeleporterGraphic.LadderDown,
				new List<List<byte>> {
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x2E, 0x2F, 0x3E, 0x3F },
					new List<byte> { 0x2E, 0x2F, 0x3E, 0x3F },
					new List<byte> { 0x2E, 0x2F, 0x3E, 0x3F },
					new List<byte> { 0x2E, 0x2F, 0x3E, 0x3F },
					new List<byte> { 0x2E, 0x2F, 0x3E, 0x3F },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x2E, 0x2F, 0x3E, 0x3F },
				}
			},

			{
				TeleporterGraphic.LadderUp,
				new List<List<byte>> {
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x10, 0x6A, 0x10, 0x6A },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x10, 0x61, 0x10, 0x61 },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x10, 0x6E, 0x10, 0x6E },
				}
			},

			{
				TeleporterGraphic.Hole,
				new List<List<byte>> {
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x4C, 0x4D, 0x5C, 0x5D },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				}
			},

			{
				TeleporterGraphic.Well,
				new List<List<byte>> {
					new List<byte> { 0x42, 0x43, 0x52, 0x43 },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x66, 0x67, 0x76, 0x77 },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x48, 0x49, 0x58, 0x59 },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				}
			},

			{
				TeleporterGraphic.Teleporter,
				new List<List<byte>> {
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x00, 0x00, 0x00, 0x00 },
					new List<byte> { 0x2C, 0x2D, 0x3C, 0x3D },
					new List<byte> { 0x42, 0x43, 0x52, 0x53 },
					new List<byte> { 0x64, 0x65, 0x74, 0x75 },
					new List<byte> { 0x42, 0x43, 0x52, 0x53 },
				}
			},

			{
				TeleporterGraphic.Door,
				new List<List<byte>> {
					new List<byte> { 0x24, 0x25, 0x34, 0x35 },
					new List<byte> { 0x22, 0x23, 0x32, 0x33 },
					new List<byte> { 0x22, 0x23, 0x32, 0x33 },
					new List<byte> { 0x22, 0x23, 0x32, 0x33 },
					new List<byte> { 0x22, 0x23, 0x32, 0x33 },
					new List<byte> { 0x22, 0x23, 0x32, 0x33 },
					new List<byte> { 0x22, 0x23, 0x32, 0x33 },
					new List<byte> { 0x22, 0x23, 0x32, 0x33 },
				}
			},

		};
	}
}
