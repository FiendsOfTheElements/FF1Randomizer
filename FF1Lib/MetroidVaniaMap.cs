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

			/*
			// Copy Tileset in bank 0x0F
			var lut_Tilesets = GetFromBank(0x00, 0xACC0, 0x40);
			PutInBank(0x0F, 0x9500, lut_Tilesets);
			
			// Turn Exit teleports to entrance teleports, expand to 64
			PutInBank(0x1F, 0xC9AA, Blob.FromHex("A90F2003FE2000964C8EC9"));
			PutInBank(0x0F, 0x9600, Blob.FromHex("A645BD40958510BD80958511BDC0958548AABD00958549200092688510688511A52948A52A48A50D48A54848A54948A51148A5104860"));

			// Test teleports
			PutInBank(0x0F, 0x9540, Blob.FromHex("02020202020202020202"));
			PutInBank(0x0F, 0x9580, Blob.FromHex("09090909090909090909"));
			PutInBank(0x0F, 0x95C0, Blob.FromHex("08080808080808080808"));
			*/

			// Coneria
			var northwall = new List<Blob> { Blob.FromHex("0404040404") };
			var eastwall = new List<Blob> { Blob.FromHex("0E") };
			var southexit = new List<Blob> { Blob.FromHex("FE") };
			maps[(int)MapId.Coneria].Put((0x0E, 0x00), northwall.ToArray());
			maps[(int)MapId.Coneria].Put((0x1F, 0x0C), eastwall.ToArray());
			maps[(int)MapId.Coneria].Put((0x10, 0x17), southexit.ToArray());

			PutInBank(0x00, 0x9000 + 0x7E, Blob.FromHex("04"));
			PutInBank(0x00, 0x9000 + 0x80 + 0x7E, Blob.FromHex("05"));
			PutInBank(0x00, 0x9000 + 0x100 + 0x7E, Blob.FromHex("14"));
			PutInBank(0x00, 0x9000 + 0x180 + 0x7E, Blob.FromHex("15"));

			var test = new smTile();

			test.Palette = TilePalette.RoomPalette1;

			/*

			BANK_SMINFO = $00
			lut_SMTilesetAttr  = $8400   ; BANK_SMINFO - must be on $400 byte bound  - 0x80 x8
			lut_SMTilesetProp  = $8800   ; BANK_SMINFO - page                        - 0x100 bytes x 8  (2 bytes per) 
			lut_SMTilesetTSA   = $9000   ; BANK_SMINFO - page                        - 0x80 bytes x4 x8 => ul, ur, dl, dr
			lut_SMPalettes     = $A000   ; BANK_SMINFO - $1000 byte bound            - 0x30 bytes x8?


			up stairs
			down stairs
			ladder
			hole

			 */



			/*
				List<Blob> restoredInn = new List<Blob>
						{
							Blob.FromHex("171717"),
							Blob.FromHex("1B251B"),
							Blob.FromHex("1C711C"),
						};
				maps[(int)MapId.Lefein].Put((0x10, 0x04), restoredInn.ToArray());
			*/
		}

		public class smTile
		{
			private byte _attribute;
			private byte _TSAul;
			private byte _TSAur;
			private byte _TSAdl;
			private byte _TSAdr;
			//private byte _property1;
			//private byte _property2;
			//private int _tileSetOrigin;

			const int BANK_SMINFO = 0x00;
			const int lut_SMTilesetAttr = 0x8400; // BANK_SMINFO - must be on $400 byte bound  - 0x80 x8
			const int lut_SMTilesetProp = 0x8800; // BANK_SMINFO - page                        - 0x100 bytes x 8  (2 bytes per)
			const int lut_SMTilesetTSA = 0x9000;  // BANK_SMINFO - page                        - 0x80 bytes x4 x8 => ul, ur, dl, dr
			const int lut_SMPalettes = 0xA000;    // BANK_SMINFO - $1000 byte bound            - 0x30 bytes x8?

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

		}

		public enum TilePalette : byte
		{
			RoomPalette1 = 0x00,
			RoomPalette2 = 0x55,
			OutPalette1 = 0xAA,
			OutPalette2 = 0xFF,
		}

	}
}
