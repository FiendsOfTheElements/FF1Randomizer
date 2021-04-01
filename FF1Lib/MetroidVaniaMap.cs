﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RomUtilities;

namespace FF1Lib
{
	public partial class FF1Rom
	{
		public void LoadInTown(OverworldMap overworldmap, List<Map> maps)
		{
			// If saved at Inn, spawn directly in the town

			PutInBank(0x1E, 0x9000, Blob.FromHex("2054C4AD10608527AD11608528AD1460854685422025902096C6BD00048544BD0104854560A91E48A9FE48A906484CFDC6"));
			PutInBank(0x1F, 0xC0B7, Blob.FromHex("A91E2003FE200090244510034CE2C1EAEAEAEAEA"));

			// Spawn at coneria castle with new game
			PutInBank(0x00, 0xB010, Blob.FromHex("9298"));

			// Hijack SaveGame to reset scrolls if we didn't come from overworld
			PutInBank(0x0E, 0x9DC0, Blob.FromHex("0000000000000000000000000000000000000000000000000000000000000000A648BDC09D8527BDD09D85284C69AB"));
			PutInBank(0x0E, 0xA53D, Blob.FromHex("20E09D"));


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

			// New function from here

			List<List<byte>> availableTiles = new()
			{
				new List<byte> { 0x3F, 0x40, 0x41, 0x42, 0x4B, 0x4C, 0x4D, 0x4E, 0x69, 0x6A, 0x6B, 0x6C, 0x74, 0x75, 0x76, 0x7D, 0x7E }, // Town
				new List<byte> { 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x41, 0x42, 0x43, 0x46, 0x47, 0x4A, 0x4B, 0x56, 0x57  }, // Castle
				new List<byte> { 0x1A, 0x7F }, // Earth, Titan, Volcano
				new List<byte> { 0x09, 0x0B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x2F }, // IceCave, etc.
				new List<byte> { 0x1E, 0x1F, 0x24, 0x25, 0x29, 0x2A, 0x2B, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F }, // Marsh, Mirage
				new List<byte> { 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x47, 0x48, 0x49, 0x4A }, // Sea, ToF
				new List<byte> { 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x6E, 0x6F, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F }, // Sky
				new List<byte> { 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A }, // ToFR
			};

			int townFreeTiles = 0;
			int castleFreeTiles = 0;
			int iceFreeTiles = 0;
			int marshFreeTiles = 0;
			int seaFreeTiles = 0;
			/*int earthFreeTiles = 0;
			int skyFreeTiles = 0;
			int tofrFreeTiles = 0;
			*/
			int townUsedTiles = 0;
			int castleUsedTiles = 0;
			int iceUsedTiles = 0;
			int marshUsedTiles = 0;
			int seaUsedTiles = 0;
			/*int earthUsedTiles = 0;
			int skyUsedTiles = 0;
			int tofrUsedTiles = 0;
			*/


			// Coneria
			var coneriaNorthwall = new List<Blob> { Blob.FromHex("0404040404") };

			maps[(int)MapId.Coneria].Put((0x0E, 0x00), coneriaNorthwall.ToArray());
			maps[(int)MapId.Coneria][0x17, 0x10] = 0x0E;
			maps[(int)MapId.Coneria][0x0C, 0x1F] = 0x0E;
			maps[(int)MapId.Coneria][0x16, 0x10] = availableTiles[(int)TileSets.Town][townUsedTiles++];

			// Pravoka
			var pravokaSouthwall = new List<Blob> {
				Blob.FromHex("0C0E000010101000000E0A"),
				Blob.FromHex("0304040404040404040405")
			};

			maps[(int)MapId.Pravoka].Put((0x0E, 0x1F), pravokaSouthwall.ToArray());
			maps[(int)MapId.Pravoka][0x1E, 0x13] = availableTiles[(int)TileSets.Town][townUsedTiles++];

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

			maps[(int)MapId.Elfland][0x16, 0x28] = availableTiles[(int)TileSets.Town][townUsedTiles++];

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

			maps[(int)MapId.Melmond][0x10, 0x03] = availableTiles[(int)TileSets.Town][townUsedTiles++];

			// Crescent Lake
			var crescentSouthwall = new List<Blob> {
				Blob.FromHex("070707"),
			};
			maps[(int)MapId.CrescentLake].Put((0x0A, 0x17), crescentSouthwall.ToArray());
			maps[(int)MapId.CrescentLake][0x00, 0x13] = 0x0E;

			maps[(int)MapId.CrescentLake][0x16, 0x0B] = availableTiles[(int)TileSets.Town][townUsedTiles++];



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

			maps[(int)MapId.ConeriaCastle1F][0x22, 0x0C] = availableTiles[(int)TileSets.Castle][castleUsedTiles++]; // To Coneria
			maps[(int)MapId.ConeriaCastle1F][0x1D, 0x17] = availableTiles[(int)TileSets.Castle][castleUsedTiles++]; // To Marsh
			maps[(int)MapId.ConeriaCastle1F][0x1D, 0x02] = availableTiles[(int)TileSets.Castle][castleUsedTiles++]; // To ToF
			//maps[(int)MapId.ConeriaCastle1F][0x09, 0x02] = availableTiles[(int)TileSets.Castle][castleUsedTiles++]; // To Waterfall

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

			maps[(int)MapId.ElflandCastle][0x17, 0x10] = availableTiles[(int)TileSets.Castle][castleUsedTiles++]; // To Elfland
			maps[(int)MapId.ElflandCastle][0x01, 0x14] = availableTiles[(int)TileSets.Castle][castleUsedTiles++]; // To Marsh B1
			maps[(int)MapId.ElflandCastle][0x02, 0x02] = availableTiles[(int)TileSets.Castle][castleUsedTiles++]; // To NW Castle
			maps[(int)MapId.ElflandCastle][0x11, 0x0C] = availableTiles[(int)TileSets.Castle][castleUsedTiles++]; // To Ice Cave

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

			maps[(int)MapId.NorthwestCastle][0x17, 0x16] = availableTiles[(int)TileSets.Castle][castleUsedTiles++]; // To Marsh
			maps[(int)MapId.NorthwestCastle][0x14, 0x0A] = availableTiles[(int)TileSets.Castle][castleUsedTiles++]; // To Elfland
			maps[(int)MapId.NorthwestCastle][0x03, 0x1C] = availableTiles[(int)TileSets.Castle][castleUsedTiles++]; // To Ordeals

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

			maps[(int)MapId.CastleOfOrdeals1F][0x02, 0x16] = availableTiles[(int)TileSets.Castle][castleUsedTiles++]; // To Gaia
			maps[(int)MapId.CastleOfOrdeals1F][0x13, 0x11] = availableTiles[(int)TileSets.Castle][castleUsedTiles++]; // To Titan's East
			maps[(int)MapId.CastleOfOrdeals1F][0x13, 0x07] = availableTiles[(int)TileSets.Castle][castleUsedTiles++]; // To NW Castle

			// Temple of Fiend
			maps[(int)MapId.TempleOfFiends][0x1E, 0x14] = availableTiles[(int)TileSets.ToFSeaShrine][seaUsedTiles++]; // To Coneria
			maps[(int)MapId.TempleOfFiends][0x06, 0x14] = availableTiles[(int)TileSets.ToFSeaShrine][seaUsedTiles++]; // To Dwarf
			maps[(int)MapId.TempleOfFiends][0x1B, 0x08] = availableTiles[(int)TileSets.ToFSeaShrine][seaUsedTiles++]; // To Matoya

			// Dwarf's Cave
			var dwarfTunnel1 = new List<Blob> { Blob.FromHex("3D3D3D313D") };
			var dwarfTunnel2 = new List<Blob> { Blob.FromHex("31313131313131313131") };

			maps[(int)MapId.DwarfCave].Put((0x0C, 0x2F), dwarfTunnel1.ToArray());
			maps[(int)MapId.DwarfCave].Put((0x10, 0x36), dwarfTunnel2.ToArray());

			maps[(int)MapId.DwarfCave][0x0B, 0x16] = availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceUsedTiles++]; // To ToF
			maps[(int)MapId.DwarfCave][0x30, 0x0C] = availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceUsedTiles++]; // To Sarda
			maps[(int)MapId.DwarfCave][0x36, 0x19] = availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceUsedTiles++]; // To Crescent

			// Matoya's Cave
			var matoyaTileReclaim = new List<Blob> { Blob.FromHex("3C3C3C3C") };
			maps[(int)MapId.MatoyasCave].Put((0x14, 0x0D), matoyaTileReclaim.ToArray());

			maps[(int)MapId.MatoyasCave][0x0B, 0x01] = availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceUsedTiles++]; // To ToF
			maps[(int)MapId.MatoyasCave][0x0B, 0x0F] = availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceUsedTiles++]; // To Pravoka
			maps[(int)MapId.MatoyasCave][0x02, 0x0E] = availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceUsedTiles++]; // To Marsh B2

			// Sarda's Cave
			maps[(int)MapId.SardasCave][0x0D, 0x12] = availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceUsedTiles++]; // To Dwarf Cave
			maps[(int)MapId.SardasCave][0x0D, 0x15] = availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceUsedTiles++]; // To Melmond
			maps[(int)MapId.SardasCave][0x0A, 0x02] = availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceUsedTiles++]; // To Tunnel W
			maps[(int)MapId.SardasCave][0x02, 0x15] = availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceUsedTiles++]; // To Earth

			// Marsh Cave B1
			var marshConeriaBox = new List<Blob> {
				Blob.FromHex("000102"),
				Blob.FromHex("030405"),
				Blob.FromHex("060708"),
				Blob.FromHex("303B30"),
				Blob.FromHex("403A40"),
			};

			maps[(int)MapId.MarshCaveB1].Put((0x14, 0x18), marshConeriaBox.ToArray());
			maps[(int)MapId.MarshCaveB1].Put((0x2A, 0x16), marshConeriaBox.ToArray());

			maps[(int)MapId.MarshCaveB1][0x19, 0x15] = availableTiles[(int)TileSets.MarshMirage][marshUsedTiles++]; // To Coneria Castle
			maps[(int)MapId.MarshCaveB1][0x17, 0x2B] = availableTiles[(int)TileSets.MarshMirage][marshUsedTiles++]; // To Elfland Castle

			// Marsh Cave B2
			maps[(int)MapId.MarshCaveB2][0x07, 0x07] = availableTiles[(int)TileSets.MarshMirage][marshUsedTiles++];
			maps[(int)MapId.MarshCaveB2][0x10, 0x12] = availableTiles[(int)TileSets.MarshMirage][marshUsedTiles++];
			maps[(int)MapId.MarshCaveB2][0x25, 0x22] = availableTiles[(int)TileSets.MarshMirage][marshUsedTiles++];

			// Marsh Cave B3
			maps[(int)MapId.MarshCaveB3][0x06, 0x05] = availableTiles[(int)TileSets.MarshMirage][marshUsedTiles++];
			maps[(int)MapId.MarshCaveB3][0x30, 0x2D] = availableTiles[(int)TileSets.MarshMirage][marshUsedTiles++];

			List<smTile> newTiles = new();
			List<smTeleporter> newTeleporters = new();

			var tilegraphics = new TeleportTilesGraphic();

			byte teleportIDtracker = 0x41;


			// Add the new teleporters, this must in the exact order as newTiles
			newTeleporters.AddRange(new List<smTeleporter>
			{
				// Coneria
				new smTeleporter(teleportIDtracker++, 0x0C, 0x22, (byte)MapId.ConeriaCastle1F, false),
				// Pravoka
				new smTeleporter(teleportIDtracker++, 0x01, 0x0B, (byte)MapId.MatoyasCave, false),
				// Elfland
				new smTeleporter(teleportIDtracker++, 0x10, 0x17, (byte)MapId.ElflandCastle, false),
				// Melmond
				new smTeleporter(teleportIDtracker++, 0x15, 0x0D, (byte)MapId.SardasCave, false),
				// Coneria Castle
				new smTeleporter(teleportIDtracker++, 0x10, 0x16, (byte)MapId.Coneria, false),
				new smTeleporter(teleportIDtracker++, 0x15, 0x19, (byte)MapId.MarshCaveB1, true),
				new smTeleporter(teleportIDtracker++, 0x14, 0x1E, (byte)MapId.TempleOfFiends, false),
				// Elfland Castle
				new smTeleporter(teleportIDtracker++, 0x28, 0x16, (byte)MapId.Elfland, false),
				new smTeleporter(teleportIDtracker++, 0x2B, 0x17, (byte)MapId.MarshCaveB1, true),
				new smTeleporter(teleportIDtracker++, 0x0A, 0x14, (byte)MapId.NorthwestCastle, false),
				new smTeleporter(teleportIDtracker++, 0x07, 0x01, (byte)MapId.IceCaveB1, false),
				// Northwest Castle
				new smTeleporter(teleportIDtracker++, 0x2D, 0x30, (byte)MapId.MarshCaveB3, false),
				new smTeleporter(teleportIDtracker++, 0x02, 0x02, (byte)MapId.ElflandCastle, false),
				new smTeleporter(teleportIDtracker++, 0x07, 0x13, (byte)MapId.CastleOfOrdeals1F, false),
				// Ordeals
				new smTeleporter(teleportIDtracker++, 0x10, 0x10, (byte)MapId.Gaia, false), // wip
				new smTeleporter(teleportIDtracker++, 0x0B, 0x0E, (byte)MapId.TitansTunnel, false),
				new smTeleporter(teleportIDtracker++, 0x1C, 0x03, (byte)MapId.NorthwestCastle, false),
				// Temple of Fiends
				new smTeleporter(teleportIDtracker++, 0x02, 0x1D, (byte)MapId.ConeriaCastle1F, false),
				new smTeleporter(teleportIDtracker++, 0x16, 0x0B, (byte)MapId.DwarfCave, false),
				new smTeleporter(teleportIDtracker++, 0x0F, 0x0B, (byte)MapId.MatoyasCave, false),
				// Dwarf's Cave
				new smTeleporter(teleportIDtracker++, 0x14, 0x06, (byte)MapId.TempleOfFiends, false),
				new smTeleporter(teleportIDtracker++, 0x12, 0x0D, (byte)MapId.SardasCave, false),
				new smTeleporter(teleportIDtracker++, 0x0B, 0x16, (byte)MapId.CrescentLake, false),
				// Matoya
				new smTeleporter(teleportIDtracker++, 0x13, 0x1E, (byte)MapId.Pravoka, false),
				new smTeleporter(teleportIDtracker++, 0x08, 0x1B, (byte)MapId.TempleOfFiends, false),
				new smTeleporter(teleportIDtracker++, 0x07, 0x07, (byte)MapId.MarshCaveB2, true),
				// Sarda
				new smTeleporter(teleportIDtracker++, 0x0C, 0x30, (byte)MapId.DwarfCave, false),
				new smTeleporter(teleportIDtracker++, 0x03, 0x10, (byte)MapId.Melmond, false),
				new smTeleporter(teleportIDtracker++, 0x05, 0x03, (byte)MapId.TitansTunnel, false),
				new smTeleporter(teleportIDtracker++, 0x17, 0x18, (byte)MapId.EarthCaveB1, false),
				// Marsh Cave B1
				new smTeleporter(teleportIDtracker++, 0x17, 0x1D, (byte)MapId.ConeriaCastle1F, true),
				new smTeleporter(teleportIDtracker++, 0x14, 0x01, (byte)MapId.ElflandCastle, true),
				// Marsh Cave B2
				new smTeleporter(teleportIDtracker++, 0x0E, 0x02, (byte)MapId.MatoyasCave, true),
				new smTeleporter(teleportIDtracker++, 0x01, 0x07, (byte)MapId.MarshCaveB1, false),
				new smTeleporter(teleportIDtracker++, 0x20, 0x34, (byte)MapId.MarshCaveB1, true),
				// Marsh Cave B3
				new smTeleporter(teleportIDtracker++, 0x3A, 0x3A, (byte)MapId.MarshCaveB2, false),
				new smTeleporter(teleportIDtracker++, 0x16, 0x17, (byte)MapId.NorthwestCastle, false),
				// Earth B5
				new smTeleporter(teleportIDtracker++, 0x13, 0x24, (byte)MapId.Cardia, false),
				// Titan
				new smTeleporter(teleportIDtracker++, 0x11, 0x13, (byte)MapId.CastleOfOrdeals1F, false),
				new smTeleporter(teleportIDtracker++, 0x02, 0x0A, (byte)MapId.SardasCave, false),

			});

			teleportIDtracker = 0x41;

			// Add the new tiles, this must in the exact order as newTeleporters
			newTiles.AddRange(new List<smTile>
			{
				// Coneria
				new smTile(availableTiles[(int)TileSets.Town][townFreeTiles++], (int)TileSets.Town, TilePalette.OutPalette2, tilegraphics.Downstairs((int)TileSets.Town), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				// Pravoka
				new smTile(availableTiles[(int)TileSets.Town][townFreeTiles++], (int)TileSets.Town, TilePalette.OutPalette2, tilegraphics.Downstairs((int)TileSets.Town), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				// Elfland
				new smTile(availableTiles[(int)TileSets.Town][townFreeTiles++], (int)TileSets.Town, TilePalette.OutPalette2, tilegraphics.Downstairs((int)TileSets.Town), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				// Melmond
				new smTile(availableTiles[(int)TileSets.Town][townFreeTiles++], (int)TileSets.Town, TilePalette.OutPalette2, tilegraphics.Downstairs((int)TileSets.Town), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				// Coneria Castle
				new smTile(availableTiles[(int)TileSets.Castle][castleFreeTiles++], (int)TileSets.Castle, TilePalette.OutPalette2, tilegraphics.Upstairs((int)TileSets.Castle), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.Castle][castleFreeTiles++], (int)TileSets.Castle, TilePalette.RoomPalette1, tilegraphics.LadderDown((int)TileSets.Castle), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.Castle][castleFreeTiles++], (int)TileSets.Castle, TilePalette.OutPalette2, tilegraphics.Downstairs((int)TileSets.Castle), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				// Elfland Castle
				new smTile(availableTiles[(int)TileSets.Castle][castleFreeTiles++], (int)TileSets.Castle, TilePalette.OutPalette2, tilegraphics.Upstairs((int)TileSets.Castle), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.Castle][castleFreeTiles++], (int)TileSets.Castle, TilePalette.RoomPalette1, tilegraphics.LadderDown((int)TileSets.Castle), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.Castle][castleFreeTiles++], (int)TileSets.Castle, TilePalette.OutPalette2, tilegraphics.Downstairs((int)TileSets.Castle), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.Castle][castleFreeTiles++], (int)TileSets.Castle, TilePalette.OutPalette2, tilegraphics.Downstairs((int)TileSets.Castle), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				// Northwest Castle
				new smTile(availableTiles[(int)TileSets.Castle][castleFreeTiles++], (int)TileSets.Castle, TilePalette.OutPalette2, tilegraphics.Downstairs((int)TileSets.Castle), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.Castle][castleFreeTiles++], (int)TileSets.Castle, TilePalette.OutPalette2, tilegraphics.Upstairs((int)TileSets.Castle), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.Castle][castleFreeTiles++], (int)TileSets.Castle, TilePalette.OutPalette2, tilegraphics.Downstairs((int)TileSets.Castle), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				// Ordeals
				new smTile(availableTiles[(int)TileSets.Castle][castleFreeTiles++], (int)TileSets.Castle, TilePalette.OutPalette2, tilegraphics.LadderDown((int)TileSets.Castle), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.Castle][castleFreeTiles++], (int)TileSets.Castle, TilePalette.OutPalette2, tilegraphics.Downstairs((int)TileSets.Castle), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.Castle][castleFreeTiles++], (int)TileSets.Castle, TilePalette.OutPalette2, tilegraphics.Upstairs((int)TileSets.Castle), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				// Temple of Fiends
				new smTile(availableTiles[(int)TileSets.ToFSeaShrine][seaFreeTiles++], (int)TileSets.ToFSeaShrine, TilePalette.OutPalette1, tilegraphics.Upstairs((int)TileSets.ToFSeaShrine), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.ToFSeaShrine][seaFreeTiles++], (int)TileSets.ToFSeaShrine, TilePalette.OutPalette1, tilegraphics.Downstairs((int)TileSets.ToFSeaShrine), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.ToFSeaShrine][seaFreeTiles++], (int)TileSets.ToFSeaShrine, TilePalette.OutPalette1, tilegraphics.Downstairs((int)TileSets.ToFSeaShrine), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				// Dwarf's Cave
				new smTile(availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceFreeTiles++], (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, tilegraphics.Upstairs((int)TileSets.MatoyaDwarfCardiaIceWaterfall), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceFreeTiles++], (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, tilegraphics.Downstairs((int)TileSets.MatoyaDwarfCardiaIceWaterfall), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceFreeTiles++], (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, tilegraphics.Upstairs((int)TileSets.MatoyaDwarfCardiaIceWaterfall), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				// Matoya
				new smTile(availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceFreeTiles++], (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, tilegraphics.Upstairs((int)TileSets.MatoyaDwarfCardiaIceWaterfall), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceFreeTiles++], (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, tilegraphics.Upstairs((int)TileSets.MatoyaDwarfCardiaIceWaterfall), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceFreeTiles++], (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.RoomPalette1, tilegraphics.LadderDown((int)TileSets.MatoyaDwarfCardiaIceWaterfall), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				// Sarda
				new smTile(availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceFreeTiles++], (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, tilegraphics.Upstairs((int)TileSets.MatoyaDwarfCardiaIceWaterfall), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceFreeTiles++], (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, tilegraphics.Upstairs((int)TileSets.MatoyaDwarfCardiaIceWaterfall), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceFreeTiles++], (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.OutPalette2, tilegraphics.Downstairs((int)TileSets.MatoyaDwarfCardiaIceWaterfall), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.MatoyaDwarfCardiaIceWaterfall][iceFreeTiles++], (int)TileSets.MatoyaDwarfCardiaIceWaterfall, TilePalette.RoomPalette1, tilegraphics.LadderDown((int)TileSets.MatoyaDwarfCardiaIceWaterfall), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				// Marsh Cave B1
				new smTile(availableTiles[(int)TileSets.MarshMirage][marshFreeTiles++], (int)TileSets.MarshMirage, TilePalette.RoomPalette1, tilegraphics.LadderUp((int)TileSets.MarshMirage), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.MarshMirage][marshFreeTiles++], (int)TileSets.MarshMirage, TilePalette.RoomPalette1, tilegraphics.LadderUp((int)TileSets.MarshMirage), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				// Marsh Cave B2
				new smTile(availableTiles[(int)TileSets.MarshMirage][marshFreeTiles++], (int)TileSets.MarshMirage, TilePalette.RoomPalette1, tilegraphics.LadderUp((int)TileSets.MarshMirage), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.MarshMirage][marshFreeTiles++], (int)TileSets.MarshMirage, TilePalette.OutPalette1, tilegraphics.Upstairs((int)TileSets.MarshMirage), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.MarshMirage][marshFreeTiles++], (int)TileSets.MarshMirage, TilePalette.RoomPalette1, tilegraphics.LadderUp((int)TileSets.MarshMirage), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				// Marsh Cave B3
				new smTile(availableTiles[(int)TileSets.MarshMirage][marshFreeTiles++], (int)TileSets.MarshMirage, TilePalette.OutPalette1, tilegraphics.Upstairs((int)TileSets.MarshMirage), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(availableTiles[(int)TileSets.MarshMirage][marshFreeTiles++], (int)TileSets.MarshMirage, TilePalette.OutPalette1, tilegraphics.Upstairs((int)TileSets.MarshMirage), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				// Earth B5
				new smTile(0x0F, (int)TileSets.EarthTitanVolcano, TilePalette.OutPalette1, tilegraphics.Downstairs((int)TileSets.EarthTitanVolcano), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				// Titan
				new smTile(0x15, (int)TileSets.EarthTitanVolcano, TilePalette.OutPalette1, tilegraphics.Upstairs((int)TileSets.EarthTitanVolcano), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
				new smTile(0x16, (int)TileSets.EarthTitanVolcano, TilePalette.OutPalette1, tilegraphics.Upstairs((int)TileSets.EarthTitanVolcano), (byte)TilePropFunc.TP_TELE_NORM, teleportIDtracker++),
			});

			foreach (var teleport in newTeleporters)
			{
				teleport.Write(this);
			}

			foreach (var tile in newTiles)
			{
				tile.Write(this);
			}
		}

		public class smTile
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
			const int lut_SMTilesetAttr = 0x8400; // BANK_SMINFO - must be on $400 byte bound  - 0x80 x8
			const int lut_SMTilesetProp = 0x8800; // BANK_SMINFO - page                        - 0x100 bytes x 8  (2 bytes per)
			const int lut_SMTilesetTSA = 0x9000;  // BANK_SMINFO - page                        - 0x80 bytes x4 x8 => ul, ur, dl, dr
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
			public smTile(byte id, int tileset, TilePalette palette, List<byte> tilegraphics, byte property1, byte property2)
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
			public void Write(FF1Rom rom)
			{
				rom.PutInBank(BANK_SMINFO, lut_SMTilesetAttr + (_tileSetOrigin * 0x80) + _tileSetID, new byte[] { _attribute });
				rom.PutInBank(BANK_SMINFO, lut_SMTilesetProp + (_tileSetOrigin * 0x100) + (_tileSetID * 2), new byte[] { _property1, _property2 });
				rom.PutInBank(BANK_SMINFO, lut_SMTilesetTSA + (_tileSetOrigin * 0x200) + _tileSetID, new byte[] { _TSAul });
				rom.PutInBank(BANK_SMINFO, lut_SMTilesetTSA + (_tileSetOrigin * 0x200) + 0x80 + _tileSetID, new byte[] { _TSAur });
				rom.PutInBank(BANK_SMINFO, lut_SMTilesetTSA + (_tileSetOrigin * 0x200) + 0x100 + _tileSetID, new byte[] { _TSAdl });
				rom.PutInBank(BANK_SMINFO, lut_SMTilesetTSA + (_tileSetOrigin * 0x200) + 0x180 + _tileSetID, new byte[] { _TSAdr });
			}
		}

		public class smTeleporter
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
					// _y = (byte)(value | (_inroom ? 0b1000000 : 0b00000000));
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
			public smTeleporter(int id, byte x, byte y, byte destination, bool inroom)
			{
				_id = id;
				_inroom = inroom;
				_x = (byte)(x | (_inroom ? 0b10000000 : 0b00000000));
				// _y = (byte)(y | (_inroom ? 0b1000000 : 0b00000000));
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
		public class TeleportTilesGraphic
		{
			private readonly List<List<byte>> _upstairs = new()
			{
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x26, 0x27, 0x36, 0x37 },
				new List<byte> { 0x26, 0x27, 0x36, 0x37 },
				new List<byte> { 0x26, 0x27, 0x36, 0x37 },
				new List<byte> { 0x26, 0x27, 0x36, 0x37 },
				new List<byte> { 0x26, 0x27, 0x36, 0x37 },
				new List<byte> { 0x26, 0x27, 0x36, 0x37 },
				new List<byte> { 0x26, 0x27, 0x36, 0x37 },
			};

			private readonly List<List<byte>> _downstairs = new()
			{
				new List<byte> { 0x04, 0x05, 0x14, 0x15 },
				new List<byte> { 0x28, 0x29, 0x38, 0x39 },
				new List<byte> { 0x28, 0x29, 0x38, 0x39 },
				new List<byte> { 0x28, 0x29, 0x38, 0x39 },
				new List<byte> { 0x28, 0x29, 0x38, 0x39 },
				new List<byte> { 0x28, 0x29, 0x38, 0x39 },
				new List<byte> { 0x62, 0x63, 0x72, 0x73 },
				new List<byte> { 0x28, 0x29, 0x38, 0x39 },
			};

			private readonly List<List<byte>> _ladderdown = new()
			{
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x2E, 0x2F, 0x3E, 0x3F },
				new List<byte> { 0x2E, 0x2F, 0x3E, 0x3F },
				new List<byte> { 0x2E, 0x2F, 0x3E, 0x3F },
				new List<byte> { 0x2E, 0x2F, 0x3E, 0x3F },
				new List<byte> { 0x2E, 0x2F, 0x3E, 0x3F },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x2E, 0x2F, 0x3E, 0x3F },
			};

			private readonly List<List<byte>> _ladderup = new()
			{
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x10, 0x6B, 0x10, 0x6B },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x10, 0x61, 0x10, 0x61 },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x10, 0x6E, 0x10, 0x6E },
			};

			private readonly List<List<byte>> _hole = new()
			{
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x4C, 0x4D, 0x5C, 0x5D },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
			};

			private readonly List<List<byte>> _well = new()
			{
				new List<byte> { 0x42, 0x43, 0x52, 0x43 },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x66, 0x67, 0x76, 0x77 },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
			};

			private readonly List<List<byte>> _teleporter = new()
			{
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x00, 0x00, 0x00, 0x00 },
				new List<byte> { 0x2C, 0x2D, 0x3C, 0x3D },
				new List<byte> { 0x42, 0x43, 0x52, 0x53 },
				new List<byte> { 0x64, 0x65, 0x74, 0x75 },
				new List<byte> { 0x42, 0x43, 0x52, 0x53 },
			};

			private readonly List<List<byte>> _door = new()
			{
				new List<byte> { 0x24, 0x25, 0x34, 0x35 },
				new List<byte> { 0x22, 0x23, 0x32, 0x33 },
				new List<byte> { 0x22, 0x23, 0x32, 0x33 },
				new List<byte> { 0x22, 0x23, 0x32, 0x33 },
				new List<byte> { 0x22, 0x23, 0x32, 0x33 },
				new List<byte> { 0x22, 0x23, 0x32, 0x33 },
				new List<byte> { 0x22, 0x23, 0x32, 0x33 },
				new List<byte> { 0x22, 0x23, 0x32, 0x33 },
			};

			public List<byte> Upstairs(int tileset)
			{
				return _upstairs[tileset];
			}
			public List<byte> Downstairs(int tileset)
			{
				return _downstairs[tileset];
			}
			public List<byte> LadderDown(int tileset)
			{
				return _ladderdown[tileset];
			}
			public List<byte> LadderUp(int tileset)
			{
				return _ladderup[tileset];
			}
			public List<byte> Hole(int tileset)
			{
				return _hole[tileset];
			}
			public List<byte> Well(int tileset)
			{
				return _well[tileset];
			}
			public List<byte> Teleporter(int tileset)
			{
				return _teleporter[tileset];
			}
			public List<byte> Door(int tileset)
			{
				return _door[tileset];
			}
		};
	}
}
