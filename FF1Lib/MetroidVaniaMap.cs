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
		public enum ShuffleOperation {
			SwitchOrphanPair = 0,
			Switch2Pairs,
			Rotate3Pairs,
			Rotate3Orphans,
			Switch2Orphans
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
			// Exclude Waterfall, since it doesn't matter if it's flipped or not
			flippedmaps = flippedmaps.Where(x => x != MapId.Waterfall).ToList();

			LoadInTown(overworldmap);
			ApplyMapMods(maps, flippedmaps, (bool)flags.LefeinSuperStore);
			UpdateInRoomTeleporters();
			CreateTeleporters(maps, flippedmaps, rng);
			PrepNPCs(talkroutines, npcdata, flippedmaps, flags, rng);
			UpdateBackgrounds();
			if ((bool)flags.Entrances || (bool)flags.Towns)
			{
				ShuffleFloor(maps, flags, overworldmap, npcdata, flippedmaps, rng);
			}
		}

		public void LoadInTown(OverworldMap overworldmap)
		{
			var townTileList = new List<byte> { 0x49, 0x4A, 0x4C, 0x4D, 0x4E, 0x5A, 0x5D, 0x6D, 0x02 };
			var townPosList = new List<(byte, byte)> { (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00), (0x00, 0x00) };

			var decompressedMap = overworldmap.MapBytes;

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

			var coneria_x = townPosList[8].Item1;
			var coneria_y = townPosList[8].Item2;

			// If saved at Inn, spawn directly in the town
			PutInBank(0x1E, 0xA000, Blob.FromHex("2054C4AD10608527AD11608528AD1460854685422025A02096C6BD00048544BD0104854560A91E48A9FE48A906484CFDC6"));
			PutInBank(0x1F, 0xC0B7, Blob.FromHex("A91E2003FE2000A0244510034CE2C1EAEAEAEAEA"));

			// Spawn at coneria castle with new game
			PutInBank(0x00, 0xB010, Blob.FromHex($"{coneria_x:X2}{coneria_y:X2}"));

			// Hijack SaveGame to reset scrolls if we didn't come from overworld
			PutInBank(0x0E, 0x9DC0, Blob.FromHex("0000000000000000000000000000000000000000000000000000000000000000A648BDC09D8527BDD09D85284C69AB"));
			PutInBank(0x0E, 0xA53D, Blob.FromHex("20E09D"));

			// Exit/Warp teleport you to Coneria
			PutInBank(0x0E, 0x9F70, Blob.FromHex($"A2FF9AA9{coneria_x:X2}8527A9{coneria_y:X2}8528A9C048A9BE48A9A048A91348A91E4C03FE"));
			PutInBank(0x0E, 0xB0FF, Blob.FromHex("4C709F"));

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
				Blob.FromHex("0319041A05"),
				Blob.FromHex("0304040405"),
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

			// Cardia - Kraken Chest
			// Cardia - Caravan
			var cardiaKrakenChest = new List<Blob> {
				Blob.FromHex("00010102"),
				Blob.FromHex("03130405"),
				Blob.FromHex("06070708"),
				Blob.FromHex("30363030"),
				Blob.FromHex("313A3131"),
			};
			// Cardia - Caravan
			var cardiaCaravan = new List<Blob> {
				Blob.FromHex("000101010102"),
				Blob.FromHex("034F04101208"),
				Blob.FromHex("060707083630"),
				Blob.FromHex("343030303A31"),
			};

			maps[(int)MapId.Cardia].Put((0x24, 0x1A), cardiaCaravan.ToArray());
			maps[(int)MapId.Cardia].Put((0x2A, 0x07), cardiaKrakenChest.ToArray());
		}
		public void UpdateInRoomTeleporters()
		{
			byte vanillaTeleporters = 0x41;

			List<TeleporterSM> teleporters = new();
			List<TeleportIndex> inroomTeleporter = new() { TeleportIndex.CastleOrdealsBack, TeleportIndex.CastleOrdealsMaze, TeleportIndex.RescuePrincess, TeleportIndex.SkyPalace1, TeleportIndex.TempleOfFiends2, TeleportIndex.TempleOfFiends6, TeleportIndex.MarshCave3, TeleportIndex.IceCave4, TeleportIndex.IceCave7 };

			for (int i = 0; i < vanillaTeleporters; i++)
			{
				teleporters.Add(new TeleporterSM(this, i));
				if (inroomTeleporter.Contains((TeleportIndex)i))
				{
					teleporters.Last().InRoom = true;
				}
			}

			foreach (var teleporter in teleporters)
			{
				teleporter.Write(this);
			}


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
			UpdateMapTile(MapId.CrescentLake, 0x25, 0x00, newTeleporterTiles.Last().TileID);

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
			UpdateMapTile(MapId.ConeriaCastle1F, 0x02, 0x08, newTeleporterTiles.Last().TileID);

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
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x25, 0x00, (byte)MapId.CrescentLake, false, (int)TileSets.EarthTitanVolcano, TilePalette.OutPalette1, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.GurguVolcanoB1, 0x1B, 0x0F, newTeleporterTiles.Last().TileID);

			// Ice Cave B1
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x0C, 0x11, (byte)MapId.ElflandCastle, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette1, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.IceCaveB1, 0x07, 0x01, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x01, 0x0C, (byte)MapId.Onrac, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.RoomPalette1, TeleporterGraphic.Hole, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.IceCaveB1, 0x02, 0x01, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x34, 0x01, (byte)MapId.IceCaveB2, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.RoomPalette1, TeleporterGraphic.Hole, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.IceCaveB1, 0x06, 0x1B, newTeleporterTiles.Last().TileID);

			// Waterfall
			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x13, 0x15, (byte)MapId.Lefein, false, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
			UpdateMapTile(MapId.Waterfall, waterfallLefeinStairs.X, waterfallLefeinStairs.Y, newTeleporterTiles.Last().TileID);

			newTeleporterTiles.Add(new TeleporterTileSM(teleportIDtracker++, 0x02, 0x08, (byte)MapId.ConeriaCastle1F, true, (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, TeleporterGraphic.Upstairs, (byte)TilePropFunc.TP_TELE_NORM, availableTiles, flippedmaps));
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
			UpdateMapTile(MapId.SeaShrineB4, 0x2D, 0x14, newTeleporterTiles.Last().TileID);

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

			TileSM ExtraChest2 = new TileSM(availableTiles[(byte)TileSets.MatoyaDwarfCardiaIceWaterfall].First(), (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.RoomPalette1, new List<byte> { 0x2A, 0x2B, 0x3A, 0x3B }, (byte)(TilePropFunc.TP_SPEC_TREASURE | TilePropFunc.TP_NOMOVE), ToFRchestsList.SpliceRandom(rng));
			UpdateMapTile(MapId.Cardia, 0x2C, 0x08, ExtraChest2.ID);
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
			// Orbs
			SetNpc(MapId.ConeriaCastle1F, 0x02, ObjectId.ConeriaCastle1FWoman1, 0x02, 0x08, true, true); // Dialog+Routine
			Data[MapObjGfxOffset + (byte)ObjectId.ConeriaCastle1FWoman1] = 0x04;
			npcdata.SetRoutine(ObjectId.ConeriaCastle1FWoman1, newTalkRoutines.NoOW_Floater);
			npcdata.GetTalkArray(ObjectId.ConeriaCastle1FWoman1)[(int)TalkArrayPos.dialogue_2] = 0x37;
			npcdata.GetTalkArray(ObjectId.ConeriaCastle1FWoman1)[(int)TalkArrayPos.dialogue_3] = 0x36;

			SetNpc(MapId.CastleOfOrdeals1F, 0x01, ObjectId.LefeinMan10, 0x16, 0x02, true, true); //Dialog+Routine
			SetNpc(MapId.Lefein, 0x0A, 0x00, 0x16, 0x02, true, true);
			Data[MapObjGfxOffset + (byte)ObjectId.LefeinMan10] = 0x04;
			npcdata.SetRoutine(ObjectId.LefeinMan10, newTalkRoutines.NoOW_Floater);
			npcdata.GetTalkArray(ObjectId.LefeinMan10)[(int)TalkArrayPos.dialogue_2] = 0x37;
			npcdata.GetTalkArray(ObjectId.LefeinMan10)[(int)TalkArrayPos.dialogue_3] = 0x36;

			SetNpc(MapId.IceCaveB1, 0x01, ObjectId.LefeinMan6, flippedmaps.Contains(MapId.IceCaveB1) ? 0x3F - 0x02 : 0x02, 0x01, true, true); //Dialog+Routine
			SetNpc(MapId.Lefein, 0x06, 0x00, 0x16, 0x02, true, true);
			Data[MapObjGfxOffset + (byte)ObjectId.LefeinMan6] = 0x04;
			npcdata.SetRoutine(ObjectId.LefeinMan6, newTalkRoutines.NoOW_Floater);
			npcdata.GetTalkArray(ObjectId.LefeinMan6)[(int)TalkArrayPos.dialogue_2] = 0x37;
			npcdata.GetTalkArray(ObjectId.LefeinMan6)[(int)TalkArrayPos.dialogue_3] = 0x36;

			// Canoe people
			SetNpc(MapId.CrescentLake, 0x0D, ObjectId.CrescentWoman, 0x25, 0x02, false, true); // Dialog+Routine
			npcdata.SetRoutine(ObjectId.CrescentWoman, newTalkRoutines.NoOW_Canoe);
			npcdata.GetTalkArray(ObjectId.CrescentWoman)[(int)TalkArrayPos.dialogue_2] = 0x5C;
			npcdata.GetTalkArray(ObjectId.CrescentWoman)[(int)TalkArrayPos.dialogue_3] = 0xC2;

			SetNpc(MapId.ElflandCastle, 0x03, ObjectId.ElflandCastleElf2, 0x0E, 0x11, false, true); // Dialog+Routine
			npcdata.SetRoutine(ObjectId.ElflandCastleElf2, newTalkRoutines.NoOW_Canoe);
			npcdata.GetTalkArray(ObjectId.ElflandCastleElf2)[(int)TalkArrayPos.dialogue_2] = 0x5C;
			npcdata.GetTalkArray(ObjectId.ElflandCastleElf2)[(int)TalkArrayPos.dialogue_3] = 0xC2;

			SetNpc(MapId.CastleOfOrdeals1F, 0x00, ObjectId.CastleOrdealsOldMan, 0x02, 0x02, true, true); //Dialog+Routine.
			npcdata.SetRoutine(ObjectId.CastleOrdealsOldMan, newTalkRoutines.NoOW_Canoe);
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
			npcdata.SetRoutine(ObjectId.GaiaScholar2, newTalkRoutines.NoOW_Chime);
			npcdata.GetTalkArray(ObjectId.GaiaScholar2)[(int)TalkArrayPos.dialogue_2] = 0xD5;
			npcdata.GetTalkArray(ObjectId.GaiaScholar2)[(int)TalkArrayPos.dialogue_3] = 0xD9;

			// Nerrick
			//npcdata.SetRoutine(ObjectId.Nerrick, newTalkRoutines.NoOW_Nerrick);

			// Switch Key dialogue
			npcdata.GetTalkArray(ObjectId.ConeriaCastle1FOldMan2)[(int)TalkArrayPos.dialogue_2] = 0x40;

			MoveNpc(MapId.DwarfCave, 0x00, 0x0F, 0x2F, false, true);
			MoveNpc(MapId.NorthwestCastle, 0x02, 0x1C, 0x01, false, false);
			MoveNpc(MapId.Onrac, 0x09, 0x11, 0x23, false, false);

			ItemsText[(int)Item.Floater] = "SIGIL  ";
			ItemsText[(int)Item.EarthOrb] = "MARK   ";

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

		public void ShuffleFloor(List<Map> maps, Flags flags, OverworldMap overworldmap, NPCdata npcdata, List<MapId> flippedmaps, MT19337 rng)
		{
			int FlippedX(MapId map, int pos) => flippedmaps.Contains(map) ? 0x3F - pos : pos;

			var Tilesets = new List<List<TileSM>>();

			for (int i = 0; i < 8; i++)
			{
				Tilesets.Add(new List<TileSM>());

				for (int j = 0; j < 128; j++)
				{
					Tilesets.Last().Add(new TileSM((byte)j, i, this));
				}
			}

			List<TeleporterSM> teleporters = new();

			for (int i = 0; i < 256; i++)
			{
				teleporters.Add(new TeleporterSM(this, i));
			}

			// All valid locations that can be shuffled, with coordinates; exclude ToFR so it isn't shuffled
			List<MapArea> maparea = new()
			{
				new MapArea { location = MapLocation.Coneria, map = MapId.Coneria, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.Pravoka, map = MapId.Pravoka, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.Elfland, map = MapId.Elfland, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.Melmond, map = MapId.Melmond, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.CrescentLake, map = MapId.CrescentLake, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.Gaia, map = MapId.Gaia, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.Onrac, map = MapId.Onrac, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.Lefein, map = MapId.Lefein, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.ConeriaCastle1, map = MapId.ConeriaCastle1F, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.ElflandCastle, map = MapId.ElflandCastle, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.NorthwestCastle, map = MapId.NorthwestCastle, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.CastleOrdeals1, map = MapId.CastleOfOrdeals1F, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.TempleOfFiends1, map = MapId.TempleOfFiends, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.EarthCave1, map = MapId.EarthCaveB1, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.GurguVolcano1, map = MapId.GurguVolcanoB1, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.IceCave1, map = MapId.IceCaveB1, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 15), lr_corner = (64, 15) },
				new MapArea { location = MapLocation.IceCaveBackExit, map = MapId.IceCaveB1, ul_corner = (0, 16), ur_corner = (26, 16), ll_corner = (0, 64), lr_corner = (26, 64) },
				new MapArea { location = MapLocation.Cardia1, map = MapId.Cardia, ul_corner = (26, 5), ur_corner = (46, 5), ll_corner = (26, 23), lr_corner = (46, 23) }, // Empty reverse horshoe
				new MapArea { location = MapLocation.Cardia2, map = MapId.Cardia, ul_corner = (5, 5), ur_corner = (20, 5), ll_corner = (5, 26), lr_corner = (20, 26) }, // L 3 chests
				new MapArea { location = MapLocation.Cardia4, map = MapId.Cardia, ul_corner = (11, 30), ur_corner = (26, 30), ll_corner = (11, 45), lr_corner = (26, 45) }, // Well
				new MapArea { location = MapLocation.Cardia5, map = MapId.Cardia, ul_corner = (35, 26), ur_corner = (44, 26), ll_corner = (35, 42), lr_corner = (44, 42) }, // Caravan
				new MapArea { location = MapLocation.Cardia6, map = MapId.Cardia, ul_corner = (46, 38), ur_corner = (59, 38), ll_corner = (46, 57), lr_corner = (59, 57) }, // 7 chests
				new MapArea { location = MapLocation.BahamutCave1, map = MapId.BahamutsRoomB1, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.Waterfall, map = MapId.Waterfall, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.DwarfCave, map = MapId.DwarfCave, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.MatoyasCave, map = MapId.MatoyasCave, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.SardasCave, map = MapId.SardasCave, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.MarshCave1, map = MapId.MarshCaveB1, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.MirageTower1, map = MapId.MirageTower1F, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.ConeriaCastle2, map = MapId.ConeriaCastle2F, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.CastleOrdealsMaze, map = MapId.CastleOfOrdeals2F, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.CastleOrdealsTop, map = MapId.CastleOfOrdeals3F, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.MarshCaveTop, map = MapId.MarshCaveB2, ul_corner = (0, 0), ur_corner = (29, 0), ll_corner = (0, 26), lr_corner = (29, 26) },
				new MapArea { location = MapLocation.MarshCave3, map = MapId.MarshCaveB2, ul_corner = (28, 32), ur_corner = (60, 32), ll_corner = (28, 60), lr_corner = (60, 60) },
				new MapArea { location = MapLocation.MarshCaveBottom, map = MapId.MarshCaveB3, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.EarthCave2, map = MapId.EarthCaveB2, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.EarthCaveVampire, map = MapId.EarthCaveB3, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.EarthCave4, map = MapId.EarthCaveB4, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.EarthCaveLich, map = MapId.EarthCaveB5, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.GurguVolcano2, map = MapId.GurguVolcanoB2, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.GurguVolcano3, map = MapId.GurguVolcanoB3, ul_corner = (0, 0), ur_corner = (52, 0), ll_corner = (0, 12), lr_corner = (52, 12) },
				new MapArea { location = MapLocation.GurguVolcano4, map = MapId.GurguVolcanoB3, ul_corner = (0, 17), ur_corner = (64, 17), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.GurguVolcano5, map = MapId.GurguVolcanoB4, ul_corner = (0, 19), ur_corner = (24, 19), ll_corner = (0, 48), lr_corner = (24, 48) },
				new MapArea { location = MapLocation.GurguVolcano6, map = MapId.GurguVolcanoB4, ul_corner = (25, 0), ur_corner = (64, 0), ll_corner = (25, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.GurguVolcanoKary, map = MapId.GurguVolcanoB5, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.IceCave2, map = MapId.IceCaveB2, ul_corner = (0, 0), ur_corner = (32, 0), ll_corner = (0, 32), lr_corner = (32, 32) },
				new MapArea { location = MapLocation.IceCavePitRoom, map = MapId.IceCaveB2, ul_corner = (38, 0), ur_corner = (64, 0), ll_corner = (38, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.IceCave3, map = MapId.IceCaveB3, ul_corner = (0, 0), ur_corner = (10, 0), ll_corner = (0, 10), lr_corner = (10, 10) },
				new MapArea { location = MapLocation.IceCave5, map = MapId.IceCaveB3, ul_corner = (16, 0), ur_corner = (64, 0), ll_corner = (16, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.BahamutCave2, map = MapId.BahamutsRoomB2, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.MirageTower2, map = MapId.MirageTower2F, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.MirageTower3, map = MapId.MirageTower3F, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.SeaShrineKraken, map = MapId.SeaShrineB5, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.SeaShrine8, map = MapId.SeaShrineB4, ul_corner = (0, 0), ur_corner = (46, 0), ll_corner = (0, 30), lr_corner = (46, 30) },
				new MapArea { location = MapLocation.SeaShrine4, map = MapId.SeaShrineB4, ul_corner = (48, 0), ur_corner = (64, 0), ll_corner = (48, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.SeaShrine5, map = MapId.SeaShrineB3, ul_corner = (45, 36), ur_corner = (51, 36), ll_corner = (45, 40), lr_corner = (51, 40) },
				new MapArea { location = MapLocation.SeaShrine7, map = MapId.SeaShrineB3, ul_corner = (46, 3), ur_corner = (53, 3), ll_corner = (46, 30), lr_corner = (53, 30) },
				new MapArea { location = MapLocation.SeaShrine1, map = MapId.SeaShrineB3, ul_corner = (0, 0), ur_corner = (43, 0), ll_corner = (0, 64), lr_corner = (43, 64) },
				new MapArea { location = MapLocation.SeaShrine2, map = MapId.SeaShrineB2, ul_corner = (0, 0), ur_corner = (48, 0), ll_corner = (0, 64), lr_corner = (48, 64) },
				new MapArea { location = MapLocation.SeaShrine6, map = MapId.SeaShrineB2, ul_corner = (51, 39), ur_corner = (64, 39), ll_corner = (51, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.SeaShrineMermaids, map = MapId.SeaShrineB1, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.SkyPalace1, map = MapId.SkyPalace1F, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.SkyPalace2, map = MapId.SkyPalace2F, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.SkyPalace3, map = MapId.SkyPalace3F, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.SkyPalaceMaze, map = MapId.SkyPalace4F, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.SkyPalaceTiamat, map = MapId.SkyPalace5F, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
				new MapArea { location = MapLocation.TitansTunnelEast, map = MapId.TitansTunnel, ul_corner = (0, 0), ur_corner = (64, 0), ll_corner = (0, 64), lr_corner = (64, 64) },
			};

			//
			List<(MapId, TileSets)> tilesetList = new()
			{
				(MapId.Coneria, TileSets.Town),
				(MapId.Pravoka, TileSets.Town),
				(MapId.Elfland, TileSets.Town),
				(MapId.Melmond, TileSets.Town),
				(MapId.CrescentLake, TileSets.Town),
				(MapId.Gaia, TileSets.Town),
				(MapId.Onrac, TileSets.Town),
				(MapId.Lefein, TileSets.Town),
				(MapId.ConeriaCastle1F, TileSets.Castle),
				(MapId.ElflandCastle, TileSets.Castle),
				(MapId.NorthwestCastle, TileSets.Castle),
				(MapId.CastleOfOrdeals1F, TileSets.Castle),
				(MapId.TempleOfFiends, TileSets.ToFSeaShrine),
				(MapId.EarthCaveB1, TileSets.EarthTitanVolcano),
				(MapId.GurguVolcanoB1, TileSets.EarthTitanVolcano),
				(MapId.IceCaveB1, TileSets.MatoyaDwarfCardiaIceWaterfall),
				(MapId.Cardia, TileSets.MatoyaDwarfCardiaIceWaterfall),
				(MapId.BahamutsRoomB1, TileSets.MatoyaDwarfCardiaIceWaterfall),
				(MapId.Waterfall, TileSets.MatoyaDwarfCardiaIceWaterfall),
				(MapId.DwarfCave, TileSets.MatoyaDwarfCardiaIceWaterfall),
				(MapId.MatoyasCave, TileSets.MatoyaDwarfCardiaIceWaterfall),
				(MapId.SardasCave, TileSets.MatoyaDwarfCardiaIceWaterfall),
				(MapId.MarshCaveB1, TileSets.MarshMirage),
				(MapId.MirageTower1F, TileSets.MarshMirage),
				(MapId.ConeriaCastle2F, TileSets.Castle),
				(MapId.CastleOfOrdeals2F, TileSets.Castle),
				(MapId.CastleOfOrdeals3F, TileSets.Castle),
				(MapId.MarshCaveB2, TileSets.MarshMirage),
				(MapId.MarshCaveB3, TileSets.MarshMirage),
				(MapId.EarthCaveB2, TileSets.EarthTitanVolcano),
				(MapId.EarthCaveB3, TileSets.EarthTitanVolcano),
				(MapId.EarthCaveB4, TileSets.EarthTitanVolcano),
				(MapId.EarthCaveB5, TileSets.EarthTitanVolcano),
				(MapId.GurguVolcanoB2, TileSets.EarthTitanVolcano),
				(MapId.GurguVolcanoB3, TileSets.EarthTitanVolcano),
				(MapId.GurguVolcanoB4, TileSets.EarthTitanVolcano),
				(MapId.GurguVolcanoB5, TileSets.EarthTitanVolcano),
				(MapId.IceCaveB2, TileSets.MatoyaDwarfCardiaIceWaterfall),
				(MapId.IceCaveB3, TileSets.MatoyaDwarfCardiaIceWaterfall),
				(MapId.BahamutsRoomB2, TileSets.MatoyaDwarfCardiaIceWaterfall),
				(MapId.MirageTower2F, TileSets.MarshMirage),
				(MapId.MirageTower3F, TileSets.MarshMirage),
				(MapId.SeaShrineB5, TileSets.ToFSeaShrine),
				(MapId.SeaShrineB4, TileSets.ToFSeaShrine),
				(MapId.SeaShrineB3, TileSets.ToFSeaShrine),
				(MapId.SeaShrineB2, TileSets.ToFSeaShrine),
				(MapId.SeaShrineB1, TileSets.ToFSeaShrine),
				(MapId.SkyPalace1F, TileSets.SkyCastle),
				(MapId.SkyPalace2F, TileSets.SkyCastle),
				(MapId.SkyPalace3F, TileSets.SkyCastle),
				(MapId.SkyPalace4F, TileSets.SkyCastle),
				(MapId.SkyPalace5F, TileSets.SkyCastle),
				(MapId.TempleOfFiendsRevisited1F, TileSets.ToFR),
				(MapId.TempleOfFiendsRevisited2F, TileSets.ToFR),
				(MapId.TempleOfFiendsRevisited3F, TileSets.ToFR),
				(MapId.TempleOfFiendsRevisitedEarth, TileSets.ToFR),
				(MapId.TempleOfFiendsRevisitedFire, TileSets.ToFR),
				(MapId.TempleOfFiendsRevisitedWater, TileSets.ToFR),
				(MapId.TempleOfFiendsRevisitedAir, TileSets.ToFR),
				(MapId.TempleOfFiendsRevisitedChaos, TileSets.ToFR),
				(MapId.TitansTunnel, TileSets.EarthTitanVolcano),
			};

			List<(MapLocation, MapLocation)> TownEntrances = new() { };
			List<(MapLocation, MapLocation)> LocationEntrances = new() { };
			List<(MapLocation, MapLocation)> InvalidLocations = new() { };

			InvalidLocations = new() { (MapLocation.IceCavePitRoom, MapLocation.IceCave5), (MapLocation.IceCave5, MapLocation.IceCaveBackExit), (MapLocation.IceCaveBackExit, MapLocation.IceCave5), (MapLocation.IceCaveBackExit, MapLocation.IceCavePitRoom), (MapLocation.CastleOrdeals1, MapLocation.CastleOrdealsMaze), (MapLocation.CastleOrdealsMaze, MapLocation.CastleOrdealsMaze), (MapLocation.CastleOrdealsMaze, MapLocation.CastleOrdealsTop), (MapLocation.CastleOrdealsTop, MapLocation.CastleOrdeals1), (MapLocation.ConeriaCastle1, MapLocation.ConeriaCastle2), (MapLocation.ConeriaCastle2, MapLocation.ConeriaCastle1) };

			if ((bool)flags.Towns)
			{
				TownEntrances = new() { (MapLocation.MatoyasCave, MapLocation.Pravoka), (MapLocation.Pravoka, MapLocation.MatoyasCave), (MapLocation.ElflandCastle, MapLocation.Elfland), (MapLocation.Elfland, MapLocation.ElflandCastle), (MapLocation.SardasCave, MapLocation.Melmond), (MapLocation.Melmond, MapLocation.SardasCave), (MapLocation.DwarfCave, MapLocation.CrescentLake), (MapLocation.CrescentLake, MapLocation.DwarfCave), (MapLocation.IceCave1, MapLocation.Onrac), (MapLocation.Onrac, MapLocation.IceCave1), (MapLocation.CastleOrdeals1, MapLocation.Gaia), (MapLocation.Gaia, MapLocation.CastleOrdeals1), (MapLocation.Waterfall, MapLocation.Lefein), (MapLocation.Lefein, MapLocation.Waterfall) };

				if ((bool)flags.IncludeConeria)
				{
					TownEntrances.AddRange(new List<(MapLocation, MapLocation)> { (MapLocation.ConeriaCastle1, MapLocation.Coneria), (MapLocation.Coneria, MapLocation.ConeriaCastle1) });
				}
			}

			if ((bool)flags.Entrances && (bool)!flags.Floors)
			{
				LocationEntrances = new() { (MapLocation.ConeriaCastle1, MapLocation.Waterfall), (MapLocation.Waterfall, MapLocation.ConeriaCastle1), (MapLocation.MatoyasCave, MapLocation.MarshCave1), (MapLocation.MarshCave1, MapLocation.MatoyasCave), (MapLocation.SardasCave, MapLocation.EarthCave1), (MapLocation.TempleOfFiends1, MapLocation.MatoyasCave), (MapLocation.MatoyasCave, MapLocation.TempleOfFiends1), (MapLocation.TempleOfFiends1, MapLocation.DwarfCave), (MapLocation.DwarfCave, MapLocation.TempleOfFiends1), (MapLocation.SardasCave, MapLocation.TitansTunnelEast), (MapLocation.TitansTunnelEast, MapLocation.SardasCave), (MapLocation.ElflandCastle, MapLocation.NorthwestCastle), (MapLocation.NorthwestCastle, MapLocation.ElflandCastle), (MapLocation.NorthwestCastle, MapLocation.MarshCaveBottom), (MapLocation.MarshCaveBottom, MapLocation.NorthwestCastle), (MapLocation.NorthwestCastle, MapLocation.CastleOrdeals1), (MapLocation.CastleOrdeals1, MapLocation.NorthwestCastle), (MapLocation.CastleOrdeals1, MapLocation.TitansTunnelEast), (MapLocation.TitansTunnelEast, MapLocation.CastleOrdeals1), (MapLocation.ElflandCastle, MapLocation.IceCave1), (MapLocation.IceCave1, MapLocation.ElflandCastle), (MapLocation.Gaia, MapLocation.MirageTower1), (MapLocation.MirageTower1, MapLocation.Gaia), (MapLocation.Onrac, MapLocation.SeaShrine1), (MapLocation.SeaShrine1, MapLocation.Onrac), (MapLocation.CrescentLake, MapLocation.GurguVolcano1), (MapLocation.GurguVolcano1, MapLocation.CrescentLake) };

				if ((bool)flags.AllowDeepCastles)
				{
					LocationEntrances.AddRange(new List<(MapLocation, MapLocation)> { (MapLocation.ConeriaCastle1, MapLocation.TempleOfFiends1), (MapLocation.TempleOfFiends1, MapLocation.ConeriaCastle1) });
				}
			}

			if ((bool)flags.Floors)
			{
				if ((bool)!flags.AllowDeepCastles)
				{
					InvalidLocations.AddRange(new List<(MapLocation, MapLocation)> { (MapLocation.ConeriaCastle1, MapLocation.TempleOfFiends1), (MapLocation.TempleOfFiends1, MapLocation.ConeriaCastle1) });
				}

				if ((bool)!flags.IncludeConeria)
				{
					InvalidLocations.AddRange(new List<(MapLocation, MapLocation)> { (MapLocation.ConeriaCastle1, MapLocation.Coneria), (MapLocation.Coneria, MapLocation.ConeriaCastle1) });
				}

				if ((bool)!flags.Towns)
				{
					InvalidLocations.AddRange(new List<(MapLocation, MapLocation)> { (MapLocation.MatoyasCave, MapLocation.Pravoka), (MapLocation.Pravoka, MapLocation.MatoyasCave), (MapLocation.ElflandCastle, MapLocation.Elfland), (MapLocation.Elfland, MapLocation.ElflandCastle), (MapLocation.SardasCave, MapLocation.Melmond), (MapLocation.Melmond, MapLocation.SardasCave), (MapLocation.DwarfCave, MapLocation.CrescentLake), (MapLocation.CrescentLake, MapLocation.DwarfCave), (MapLocation.IceCave1, MapLocation.Onrac), (MapLocation.Onrac, MapLocation.IceCave1), (MapLocation.CastleOrdeals1, MapLocation.Gaia), (MapLocation.Gaia, MapLocation.CastleOrdeals1), (MapLocation.Waterfall, MapLocation.Lefein), (MapLocation.Lefein, MapLocation.Waterfall) });
				}
			}

			// Generate teleporters info
			var TeleportTiles = Tilesets.SelectMany(x => x.Where(y => (y.PropertyType & 0b1100_0000) == 0b1000_0000));
			var TilesetTeleportTiles = Tilesets.Select(x => x.Where(y => (y.PropertyType & 0b1100_0000) == 0b1000_0000).ToList()).ToList();

			List<(byte, MapLocation, MapLocation)> teleportersLocDest = new();

			for (int i = 0; i < maps.Count(); i++)
			{
				foreach (var teleporttile in TilesetTeleportTiles[(int)tilesetList[i].Item2])
				{
					if (maps[i].FindFirst(teleporttile.ID, out var x, out var y))
					{
						var targetteleporter = teleporters[teleporttile.PropertyValue];

						teleportersLocDest.Add((
							teleporttile.PropertyValue,
							maparea.Find(area => area.map == (MapId)i && area.ul_corner.Item1 <= FlippedX((MapId)i, x) && area.lr_corner.Item1 >= FlippedX((MapId)i, x) && area.ul_corner.Item2 <= y && area.lr_corner.Item2 >= y).location,
							maparea.Find(area => area.map == (MapId)targetteleporter.Destination && area.ul_corner.Item1 <= FlippedX((MapId)targetteleporter.Destination, targetteleporter.X) && area.lr_corner.Item1 >= FlippedX((MapId)targetteleporter.Destination, targetteleporter.X) && area.ul_corner.Item2 <= targetteleporter.Y && area.lr_corner.Item2 >= targetteleporter.Y).location));
					}
				}
			}

			teleportersLocDest = teleportersLocDest.Where(x => x.Item2 != MapLocation.StartingLocation && x.Item3 != MapLocation.StartingLocation).ToList();

			List<MapLocation> OrphanGateways = new() { MapLocation.BahamutCave1, MapLocation.Cardia5, MapLocation.Cardia6, MapLocation.Cardia1, MapLocation.EarthCave1, MapLocation.GurguVolcano3, MapLocation.MirageTower3 };

			List<(byte, byte)> TeleportersPair = new();
			List<byte> ProcessedTeleporters = new();

			List<(byte, byte)> TownTeleporters = new();
			List<(byte, byte)> PairedTeleporters = new();
			List<byte> OrphanTeleporters = new();
			List<int> GatewaysTeleporters = new();

			// Find the valid teleporters to shuffle and distribute them amongst the buckets
			foreach (var teleport in teleportersLocDest)
			{
				if (!ProcessedTeleporters.Contains((byte)teleport.Item1) && !InvalidLocations.Contains((teleport.Item2, teleport.Item3)))
				{
					var pair = teleportersLocDest.Where(x => x.Item2 == teleport.Item3 && x.Item3 == teleport.Item2);

					if (pair.Any())
					{
						if (TownEntrances.Contains((teleport.Item2, teleport.Item3)))
						{
							TownTeleporters.Add((teleport.Item1, pair.First().Item1));
						}
						else if ((bool)flags.Floors || LocationEntrances.Contains((teleport.Item2, teleport.Item3)))
						{
							PairedTeleporters.Add((teleport.Item1, pair.First().Item1));
						}

						ProcessedTeleporters.Add(teleport.Item1);
						ProcessedTeleporters.Add(pair.First().Item1);
					}
					else if((bool)flags.Floors || LocationEntrances.Contains((teleport.Item2, teleport.Item3)))
					{
						OrphanTeleporters.Add(teleport.Item1);
						if (OrphanGateways.Contains(teleport.Item3))
						{
							GatewaysTeleporters.Add(OrphanTeleporters.Count - 1);
						}
					}
				}
			}

			List<(byte, MapLocation, MapLocation)> newteleportersInOut = new();
			List<(byte, byte)> ComboTeleporters = new();
			List<int> ComboArray = new();
			List<int> PairsArray = new();
			List<int> OrphansArray = new();
			List<int> TownsArray = new();

			
			List<(ShuffleOperation, List<byte>)> operationsList = new();

			// Layout sanity variables
			var origin = MapLocation.ConeriaCastle1;
			List<byte> usedteleporters = new();
			List<(byte, MapLocation, MapLocation)> unreachableLocations = new();
			var locationToVisit = teleportersLocDest.Where(x => x.Item2 == origin).ToList();
			int _shuffleCounter = 0;

			do
			{
				_shuffleCounter++;
				if (_shuffleCounter > 100) throw new InsaneException("Location shuffling couldn't create a valid layout!");

				operationsList.Clear();
				newteleportersInOut.Clear();
				if ((bool)flags.Floors)
				{
					newteleportersInOut.AddRange(teleportersLocDest.Where(x => InvalidLocations.Contains((x.Item2, x.Item3))).ToList());
				}
				else
				{
					newteleportersInOut.AddRange(teleportersLocDest.Where(x => !LocationEntrances.Contains((x.Item2, x.Item3)) && !TownEntrances.Contains((x.Item2, x.Item3))).ToList());
				}
				
				PairsArray = Enumerable.Range(0, PairedTeleporters.Count).ToList();
				OrphansArray = Enumerable.Range(0, OrphanTeleporters.Count).ToList();
				TownsArray = Enumerable.Range(0, TownTeleporters.Count).ToList();

				// Shuffle one way gateways first
				var maxteleporterpairs = PairedTeleporters.Count;

				foreach (var index in GatewaysTeleporters)
				{
					var result = Rng.Between(rng, 0, maxteleporterpairs + GatewaysTeleporters.Count);

					if (result < maxteleporterpairs || OrphansArray.Count == 1)
					{
						var selectedpair = PairsArray.SpliceRandom(rng);
						var pairA = PairedTeleporters[selectedpair];
						var pairB = OrphanTeleporters[index];

						newteleportersInOut.Add((pairA.Item1, teleportersLocDest.Find(x => x.Item1 == pairB).Item2, teleportersLocDest.Find(x => x.Item1 == pairA.Item1).Item3));
						newteleportersInOut.Add((pairB, teleportersLocDest.Find(x => x.Item1 == pairA.Item1).Item2, teleportersLocDest.Find(x => x.Item1 == pairB).Item3));
						newteleportersInOut.Add((pairA.Item2, teleportersLocDest.Find(x => x.Item1 == pairA.Item2).Item2, teleportersLocDest.Find(x => x.Item1 == pairB).Item2));

						operationsList.Add((ShuffleOperation.SwitchOrphanPair, new List<byte> { pairA.Item1, pairA.Item2, pairB }));

						OrphansArray.Remove(index);
					}
				}

				if ((bool)flags.EntrancesMixedWithTowns)
				{
					ComboTeleporters = PairedTeleporters.Concat(TownTeleporters).ToList();
					ComboArray = PairsArray.Concat(TownsArray.Select(x => x + maxteleporterpairs)).ToList();
					TownsArray.Clear();
				}
				else
				{
					ComboTeleporters = PairedTeleporters;
					ComboArray = PairsArray;
				}

				// Shuffle Towns amongst themselves, only if entrances aren't mixed
				// If impair number, do a 3 way shuffle first, then switch pairs for the rest
				if (TownsArray.Count % 2 != 0)
				{
					var pairA = TownTeleporters[TownsArray.SpliceRandom(rng)];
					var pairB = TownTeleporters[TownsArray.SpliceRandom(rng)];
					var pairC = TownTeleporters[TownsArray.SpliceRandom(rng)];

					newteleportersInOut.Add((pairA.Item1, teleportersLocDest.Find(x => x.Item1 == pairC.Item1).Item2, teleportersLocDest.Find(x => x.Item1 == pairA.Item1).Item3));
					newteleportersInOut.Add((pairB.Item1, teleportersLocDest.Find(x => x.Item1 == pairA.Item1).Item2, teleportersLocDest.Find(x => x.Item1 == pairB.Item1).Item3));
					newteleportersInOut.Add((pairC.Item1, teleportersLocDest.Find(x => x.Item1 == pairB.Item1).Item2, teleportersLocDest.Find(x => x.Item1 == pairC.Item1).Item3));
					newteleportersInOut.Add((pairA.Item2, teleportersLocDest.Find(x => x.Item1 == pairB.Item2).Item2, teleportersLocDest.Find(x => x.Item1 == pairA.Item2).Item3));
					newteleportersInOut.Add((pairB.Item2, teleportersLocDest.Find(x => x.Item1 == pairC.Item2).Item2, teleportersLocDest.Find(x => x.Item1 == pairB.Item2).Item3));
					newteleportersInOut.Add((pairC.Item2, teleportersLocDest.Find(x => x.Item1 == pairA.Item2).Item2, teleportersLocDest.Find(x => x.Item1 == pairC.Item2).Item3));

					operationsList.Add((ShuffleOperation.Rotate3Pairs, new List<byte> { pairA.Item1, pairA.Item2, pairB.Item1, pairB.Item2, pairC.Item1, pairC.Item2 }));
				}

				while (TownsArray.Count > 0)
				{
					var pairA = TownTeleporters[TownsArray.SpliceRandom(rng)];
					var pairB = TownTeleporters[TownsArray.SpliceRandom(rng)];

					newteleportersInOut.Add((pairA.Item1, teleportersLocDest.Find(x => x.Item1 == pairB.Item1).Item2, teleportersLocDest.Find(x => x.Item1 == pairA.Item1).Item3));
					newteleportersInOut.Add((pairB.Item1, teleportersLocDest.Find(x => x.Item1 == pairA.Item1).Item2, teleportersLocDest.Find(x => x.Item1 == pairB.Item1).Item3));
					newteleportersInOut.Add((pairA.Item2, teleportersLocDest.Find(x => x.Item1 == pairB.Item2).Item2, teleportersLocDest.Find(x => x.Item1 == pairA.Item2).Item3));
					newteleportersInOut.Add((pairB.Item2, teleportersLocDest.Find(x => x.Item1 == pairA.Item2).Item2, teleportersLocDest.Find(x => x.Item1 == pairB.Item2).Item3));

					operationsList.Add((ShuffleOperation.Switch2Pairs, new List<byte> { pairA.Item1, pairA.Item2, pairB.Item1, pairB.Item2 }));
				}

				// Shuffle pairs amongst themselves
				// If impair number, do a 3 way shuffle first, then switch pairs for the rest
				if (ComboArray.Count % 2 != 0)
				{
					var pairA = ComboTeleporters[ComboArray.SpliceRandom(rng)];
					var pairB = ComboTeleporters[ComboArray.SpliceRandom(rng)];
					var pairC = ComboTeleporters[ComboArray.SpliceRandom(rng)];

					newteleportersInOut.Add((pairA.Item1, teleportersLocDest.Find(x => x.Item1 == pairC.Item1).Item2, teleportersLocDest.Find(x => x.Item1 == pairA.Item1).Item3));
					newteleportersInOut.Add((pairB.Item1, teleportersLocDest.Find(x => x.Item1 == pairA.Item1).Item2, teleportersLocDest.Find(x => x.Item1 == pairB.Item1).Item3));
					newteleportersInOut.Add((pairC.Item1, teleportersLocDest.Find(x => x.Item1 == pairB.Item1).Item2, teleportersLocDest.Find(x => x.Item1 == pairC.Item1).Item3));
					newteleportersInOut.Add((pairA.Item2, teleportersLocDest.Find(x => x.Item1 == pairB.Item2).Item2, teleportersLocDest.Find(x => x.Item1 == pairA.Item2).Item3));
					newteleportersInOut.Add((pairB.Item2, teleportersLocDest.Find(x => x.Item1 == pairC.Item2).Item2, teleportersLocDest.Find(x => x.Item1 == pairB.Item2).Item3));
					newteleportersInOut.Add((pairC.Item2, teleportersLocDest.Find(x => x.Item1 == pairA.Item2).Item2, teleportersLocDest.Find(x => x.Item1 == pairC.Item2).Item3));

					operationsList.Add((ShuffleOperation.Rotate3Pairs, new List<byte> { pairA.Item1, pairA.Item2, pairB.Item1, pairB.Item2, pairC.Item1, pairC.Item2 }));
				}

				while (ComboArray.Count > 0)
				{
					var pairA = ComboTeleporters[ComboArray.SpliceRandom(rng)];
					var pairB = ComboTeleporters[ComboArray.SpliceRandom(rng)];

					newteleportersInOut.Add((pairA.Item1, teleportersLocDest.Find(x => x.Item1 == pairB.Item1).Item2, teleportersLocDest.Find(x => x.Item1 == pairA.Item1).Item3));
					newteleportersInOut.Add((pairB.Item1, teleportersLocDest.Find(x => x.Item1 == pairA.Item1).Item2, teleportersLocDest.Find(x => x.Item1 == pairB.Item1).Item3));
					newteleportersInOut.Add((pairA.Item2, teleportersLocDest.Find(x => x.Item1 == pairB.Item2).Item2, teleportersLocDest.Find(x => x.Item1 == pairA.Item2).Item3));
					newteleportersInOut.Add((pairB.Item2, teleportersLocDest.Find(x => x.Item1 == pairA.Item2).Item2, teleportersLocDest.Find(x => x.Item1 == pairB.Item2).Item3));

					operationsList.Add((ShuffleOperation.Switch2Pairs, new List<byte> { pairA.Item1, pairA.Item2, pairB.Item1, pairB.Item2 }));
				}

				// Shuffle orphans amongst themselves
				// If impair number, do a 3 way shuffle first, then switch pairs for the rest
				if (OrphansArray.Count % 2 != 0)
				{
					var pairA = OrphanTeleporters[OrphansArray.SpliceRandom(rng)];
					var pairB = OrphanTeleporters[OrphansArray.SpliceRandom(rng)];
					var pairC = OrphanTeleporters[OrphansArray.SpliceRandom(rng)];

					newteleportersInOut.Add((pairA, teleportersLocDest.Find(x => x.Item1 == pairB).Item2, teleportersLocDest.Find(x => x.Item1 == pairA).Item3));
					newteleportersInOut.Add((pairB, teleportersLocDest.Find(x => x.Item1 == pairC).Item2, teleportersLocDest.Find(x => x.Item1 == pairB).Item3));
					newteleportersInOut.Add((pairC, teleportersLocDest.Find(x => x.Item1 == pairA).Item2, teleportersLocDest.Find(x => x.Item1 == pairC).Item3));

					operationsList.Add((ShuffleOperation.Rotate3Orphans, new List<byte> { pairA, pairB, pairC }));
				}

				while (OrphansArray.Count > 0)
				{
					var pairA = OrphanTeleporters[OrphansArray.SpliceRandom(rng)];
					var pairB = OrphanTeleporters[OrphansArray.SpliceRandom(rng)];

					newteleportersInOut.Add((pairA, teleportersLocDest.Find(x => x.Item1 == pairB).Item2, teleportersLocDest.Find(x => x.Item1 == pairA).Item3));
					newteleportersInOut.Add((pairB, teleportersLocDest.Find(x => x.Item1 == pairA).Item2, teleportersLocDest.Find(x => x.Item1 == pairB).Item3));

					operationsList.Add((ShuffleOperation.Switch2Orphans, new List<byte> { pairA, pairB }));
				}

				// Validate that all teleporters are reachable, if some aren't start over
				locationToVisit = newteleportersInOut.Where(x => x.Item2 == origin).ToList();
				usedteleporters.Clear();

				while (locationToVisit.Any())
				{
					usedteleporters.Add(locationToVisit.First().Item1);
					//Console.WriteLine(newteleportersInOut.Find(x => x.Item1 == locationToVisit.First().Item1).Item2 + " > " + newteleportersInOut.Find(x => x.Item1 == locationToVisit.First().Item1).Item3);
					locationToVisit.AddRange(newteleportersInOut.Where(x => x.Item2 == locationToVisit.First().Item3 && !usedteleporters.Contains(x.Item1)).ToList());
					locationToVisit.RemoveAt(0);
				}

				// Output unreachable locations for debugging
				unreachableLocations = newteleportersInOut.Where(x => !usedteleporters.Contains(x.Item1)).ToList();
				/*	
				foreach (var loc in unreachableLocations)
				{
					Console.WriteLine(loc.Item2 + " > " + loc.Item3);
				}
				Console.WriteLine("-------------");
				*/

			} while (unreachableLocations.Any());

			Console.WriteLine("Locations successfully suffled after " + _shuffleCounter + " iteration(s).");

			List<(TileSM, TeleporterSM)> TeleportersTiles = TeleportTiles.Select(x => (x, teleporters.Find(y => y.ID == x.PropertyValue))).ToList();

			// Now that we found a valid layout, actually update the teleporters
			foreach (var operation in operationsList)
			{
				if (operation.Item1 == ShuffleOperation.SwitchOrphanPair) // Pair w/ Orphan
				{
					var teleporterA1 = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[0]);
					var teleporterA2 = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[1]);
					var teleporterB = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[2]);

					var tempTeleporter1 = teleporterA1.Item2;

					// Because orphan don't have a twin teleporter, we need to find it's position and update the switch in teleporter
					foreach (var mapid in tilesetList.Where(x => x.Item2 == (TileSets)teleporterB.Item1.TileSet))
					{
						if (maps[(int)mapid.Item1].FindFirst(teleporterB.Item1.ID, out var x, out var y))
						{
							teleporters[teleporterA2.Item2.ID] = new TeleporterSM(teleporterA2.Item2.ID, (byte)x, (byte)y, (byte)mapid.Item1, (teleporterB.Item1.Palette <= TilePalette.RoomPalette2));
							break;
						}
					}

					teleporterA1.Item2 = teleporterB.Item2;
					teleporterB.Item2 = tempTeleporter1;

					teleporterA1.Item1.PropertyValue = (byte)teleporterA1.Item2.ID;
					teleporterA2.Item1.PropertyValue = (byte)teleporterA2.Item2.ID;
					teleporterB.Item1.PropertyValue = (byte)teleporterB.Item2.ID;
				}
				else if (operation.Item1 == ShuffleOperation.Switch2Pairs) // Pairs switch
				{
					var teleporterA1 = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[0]);
					var teleporterA2 = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[1]);
					var teleporterB1 = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[2]);
					var teleporterB2 = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[3]);

					var tempTeleporter1 = teleporterA1.Item2;
					var tempTeleporter2 = teleporterA2.Item2;

					teleporterA1.Item2 = teleporterB1.Item2;
					teleporterA2.Item2 = teleporterB2.Item2;
					teleporterB1.Item2 = tempTeleporter1;
					teleporterB2.Item2 = tempTeleporter2;

					teleporterA1.Item1.PropertyValue = (byte)teleporterA1.Item2.ID;
					teleporterA2.Item1.PropertyValue = (byte)teleporterA2.Item2.ID;
					teleporterB1.Item1.PropertyValue = (byte)teleporterB1.Item2.ID;
					teleporterB2.Item1.PropertyValue = (byte)teleporterB2.Item2.ID;
				}
				else if (operation.Item1 == ShuffleOperation.Rotate3Pairs) // 3 pairs rotation
				{
					var teleporterA1 = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[0]);
					var teleporterA2 = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[1]);
					var teleporterB1 = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[2]);
					var teleporterB2 = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[3]);
					var teleporterC1 = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[4]);
					var teleporterC2 = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[5]);

					var tempTeleporter1 = teleporterA1.Item2;
					var tempTeleporter2 = teleporterA2.Item2;

					teleporterA1.Item2 = teleporterC1.Item2;
					teleporterA2.Item2 = teleporterB2.Item2;
					teleporterC1.Item2 = teleporterB1.Item2;
					teleporterB2.Item2 = teleporterC2.Item2;
					teleporterB1.Item2 = tempTeleporter1;
					teleporterC2.Item2 = tempTeleporter2;

					teleporterA1.Item1.PropertyValue = (byte)teleporterA1.Item2.ID;
					teleporterA2.Item1.PropertyValue = (byte)teleporterA2.Item2.ID;
					teleporterB1.Item1.PropertyValue = (byte)teleporterB1.Item2.ID;
					teleporterB2.Item1.PropertyValue = (byte)teleporterB2.Item2.ID;
					teleporterC1.Item1.PropertyValue = (byte)teleporterC1.Item2.ID;
					teleporterC2.Item1.PropertyValue = (byte)teleporterC2.Item2.ID;
				}
				else if (operation.Item1 == ShuffleOperation.Rotate3Orphans) // 3 orphans rotation
				{
					var teleporterA = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[0]);
					var teleporterB = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[1]);
					var teleporterC = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[2]);

					var tempTeleporter1 = teleporterA.Item2;

					teleporterA.Item2 = teleporterB.Item2;
					teleporterB.Item2 = teleporterC.Item2;
					teleporterC.Item2 = tempTeleporter1;

					teleporterA.Item1.PropertyValue = (byte)teleporterA.Item2.ID;
					teleporterB.Item1.PropertyValue = (byte)teleporterB.Item2.ID;
					teleporterC.Item1.PropertyValue = (byte)teleporterC.Item2.ID;
				}
				else if (operation.Item1 == ShuffleOperation.Switch2Orphans) // 2 orphans switch
				{
					var teleporterA = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[0]);
					var teleporterB = TeleportersTiles.Find(x => x.Item2.ID == operation.Item2[1]);

					var tempTeleporter1 = teleporterA.Item2;

					teleporterA.Item2 = teleporterB.Item2;
					teleporterB.Item2 = tempTeleporter1;

					teleporterA.Item1.PropertyValue = (byte)teleporterA.Item2.ID;
					teleporterB.Item1.PropertyValue = (byte)teleporterB.Item2.ID;
				}
			}

			// Write the new tiles and teleporters to rom
			foreach (var tile in TeleportTiles)
			{
				tile.Write(this);
			}

			foreach (var teleport in teleporters)
			{
				teleport.Write(this);
			}

			// Set Orbs over stairs to avoid softlocks, altho it shouldn't happen anyway, no need for Mark npcs since you can always go back
			List<(MapId, byte, byte, ObjectId)> orbLocations = new() { (MapId.ConeriaCastle1F, 0x02, 0x08, ObjectId.ConeriaCastle1FWoman1), (MapId.CastleOfOrdeals1F, 0x16, 0x02, ObjectId.LefeinMan10), (MapId.IceCaveB1, (byte)FlippedX(MapId.IceCaveB1, 0x02), 0x01, ObjectId.LefeinMan6) };

			foreach (var source in orbLocations)
			{
				var targetile = maps[(int)source.Item1][(source.Item2, source.Item3)].Tile;

				var originTile = TeleportersTiles.Find(x => x.Item1.ID == (byte)targetile && x.Item1.TileSet == (int)tilesetList[(int)source.Item1].Item2);
				var originTeleporter = teleporters.Find(x => x.ID == originTile.Item1.PropertyValue);

				var freenpc = FindNpc((MapId)originTeleporter.Destination, (ObjectId.None));

				if (freenpc.Index > 0 || (freenpc.Index == 0 && freenpc.Coord == (0, 0)))
				{
					SetNpc((MapId)originTeleporter.Destination, freenpc.Index, source.Item4, originTeleporter.X, originTeleporter.Y, originTeleporter.InRoom, true);
					var sprite_palette = GetFromBank(0x00, 0xA000 + (originTeleporter.Destination * 0x30) + 0x18, 8);
					if (sprite_palette == Blob.FromHex("0F0000000F000000"))
					{
						PutInBank(0x00, 0xA000 + (originTeleporter.Destination * 0x30) + 0x18, GetFromBank(0x00, 0xA000 + ((byte)source.Item1 * 0x30) + 0x18, 8));
					}
				}
			}
		}

		public struct MapArea
		{
			public (int, int) ur_corner;
			public (int, int) ul_corner;
			public (int, int) lr_corner;
			public (int, int) ll_corner;
			public MapId map;
			public MapLocation location;
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
			public TileProp RawProperties()
			{
				return new TileProp { Byte1 = _property1, Byte2 = _property2 };
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
			public bool InRoom
			{
				get { return _inroom; }
				set
				{
					_inroom = value;
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
			public TeleData Raw()
			{
				return new TeleData { Map = (MapId)_target, X = (byte)(_x | (_inroom ? 0b1000_0000 : 0b0000_0000)), Y = (byte)(_y | 0b1000_0000) };
			}
			public TeleporterSM(int id, byte x, byte y, byte destination, bool inroom)
			{
				_id = id;
				_inroom = inroom;
				_x = (byte)(x | (_inroom ? 0b10000000 : 0b00000000));
				_y = (byte)(y | 0b10000000);
				_target = destination;
			}
			public TeleporterSM(FF1Rom rom, int id)
			{
				var byte0 = rom.GetFromBank(BANK_TELEPORTINFO, lut_NormTele_X_ext + id, 1);
				var byte1 = rom.GetFromBank(BANK_TELEPORTINFO, lut_NormTele_Y_ext + id, 1);
				var byte2 = rom.GetFromBank(BANK_TELEPORTINFO, lut_NormTele_Map_ext + id, 1);

				_id = id;
				_inroom = (byte0[0] & 0b10000000) > 0;
				_x = (byte)(byte0[0] & 0b0111_1111);
				_y = (byte)(byte1[0] & 0b0111_1111);
				_target = byte2[0];
			}
			public void Write(FF1Rom rom)
			{
				_x = (byte)(_x | (_inroom ? 0b1000_0000 : 0b0000_0000));
				_y = (byte)(_y | 0b1000_0000);

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
