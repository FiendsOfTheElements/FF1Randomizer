using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using static FF1Lib.OverworldMap;

namespace FF1Lib
{
	public partial class FF1Rom
	{
		byte OPEN_CHEST_TILE = 0x7F;
		byte OPEN_CHEST_UL_CHR_IDX = 0x7E;
		byte OPEN_CHEST_UR_CHR_IDX = 0x7F;

		int ORB_SPRITE_LOCATION = 0xA600;
		int PLATE_SPRITE_LOCATION = 0xBD00;
		
		public class TileDetails
		{
			public Dictionary<Rgba32, byte> palette { get; set; }
			public List<int> tilenums { get; set; } = new List<int>();
		}
		public async Task ReplaceDungeonTileGraphics(Image<Rgba32> tilesImage)
		{
			int DUNGEON_WIDTH = 96;
			int DUNGEON_HEIGHT = 96;

			//this array defines the crop rectangles to split the incoming image into individual tileset images
			ValueTuple<int, int, int, int>[] splitPoints = new ValueTuple<int, int, int, int>[]
			{
				(0,				  0,			  DUNGEON_WIDTH, DUNGEON_HEIGHT),
				(DUNGEON_WIDTH,	  0,			  DUNGEON_WIDTH, DUNGEON_HEIGHT),
				(DUNGEON_WIDTH*2, 0,			  DUNGEON_WIDTH, DUNGEON_HEIGHT),
				(0,				  DUNGEON_HEIGHT, DUNGEON_WIDTH, DUNGEON_HEIGHT + 16),
				(DUNGEON_WIDTH,	  DUNGEON_HEIGHT, DUNGEON_WIDTH, DUNGEON_HEIGHT + 16),
				(DUNGEON_WIDTH*2, DUNGEON_HEIGHT, DUNGEON_WIDTH, DUNGEON_HEIGHT + 16),
			};
			TileSets[] order = new TileSets[] { TileSets.MarshMirage, TileSets.EarthTitanVolcano, TileSets.ToFSeaShrine,
												TileSets.MatoyaDwarfCardiaIceWaterfall, TileSets.SkyCastle, TileSets.ToFR };

			List<Image<Rgba32>> dungeonTilesets = new List<Image<Rgba32>>();

			foreach (var s in splitPoints)
			{
				Rectangle rect = new Rectangle(s.Item1, s.Item2, s.Item3, s.Item4);
				dungeonTilesets.Add(tilesImage.Clone(ctx => ctx.Crop(rect)));
			}
			for (int i = 0; i < dungeonTilesets.Count; i++)
			{
				await ReplaceTileGraphics(order[i], dungeonTilesets[i]);
			}
			

		}

		public async Task ReplaceTileGraphics(TileSets tileSet, Image<Rgba32> tilesImage)
		{
			int maxCHR = 128;
			Image<Rgba32> orbSprite;
			Image<Rgba32> plateSprite;

			TileGraphicsRefData tileGraphicsRefData = new TileGraphicsRefData(this);
			List<TileDetails> tileReference = tileGraphicsRefData.DetailsForTileset(tileSet);

			List<Image<Rgba32>> tiles = SplitTiles(tilesImage);


			List<byte[]> chrEntries = new List<byte[]>();
			int excessCHR = 0;

			//Each 8x8 tile can be pointed to by multiple map tiles, so unique 8x8 tiles
			//are stored in chrEntries and map tiles point to whichever ones they need
			for (int imageidx = 0; imageidx < tiles.Count;  imageidx++)
			{
				var palette = tileReference[imageidx].palette;
				var tileimage = tiles[imageidx];

				if (palette == tileGraphicsRefData.spritePalette)
				{
					//TODO: handle sprite import for rod plate and orb
					if (tileReference[imageidx].tilenums.Contains(TileGraphicsRefData.PLATE_SPRITE_TILE))
					{
						plateSprite = tiles[imageidx];
						ReplacePlateSprite(plateSprite);
					}
					if (tileReference[imageidx].tilenums.Contains(TileGraphicsRefData.ORB_SPRITE_TILE))
					{
						orbSprite = tiles[imageidx];
						ReplaceOrbSprite(orbSprite);
					}
				}
				else
				{
					//loop through each 8x8 square of an imported 16x16 tile
					foreach (var loadchr in new ValueTuple<int, int, int>[]
								{ (0, 0, 0), (8, 0, 128), (0, 8, 256), (8, 8, 384) })
					{
						//get or create 8x8 tile bytes
						byte idx = chrIndex(makeTile(tileimage, loadchr.Item2, loadchr.Item1, palette), chrEntries, maxCHR);
						if (idx == 0xff)
						{
							await this.Progress($"WARNING: Error importing CHR at {loadchr.Item1}, {loadchr.Item2}, in image tile {imageidx} too many unique CHR");
							idx = 0;
							excessCHR++;
						}
						//update the map tile details of every tile that should point to this 8x8 CHR data
						foreach (int tileidx in tileReference[imageidx].tilenums)
						{
							Put(TILESETPATTERNTABLE_ASSIGNMENT + (TILESETPATTERNTABLE_ASSIGNMENT_SIZEPERSET * (int)tileSet) + loadchr.Item3 + tileidx, new byte[] { idx });
						}
					}
				}
			}
			
			if (excessCHR > 0)
			{
				await this.Progress($"WARNING: More than {maxCHR} unique 8x8 tiles, must eliminate {excessCHR} excess tiles");
			}

			//write CHR data to ROM
			for (int i = 0; i < chrEntries.Count; i++)
			{
				Put(TILESETPATTERNTABLE_OFFSET + (TILESETPATTERNTABLE_SIZEPERSET * (int)tileSet) + (i * 16), EncodeForPPU(chrEntries[i]));
			}

			//place open chest tiles in their correct positions
			int openChestIdx = tileGraphicsRefData.OpenChestImageIndexForTileset(tileSet);
			var openChestUL = makeTile(tiles[openChestIdx], 0, 0, tileReference[openChestIdx].palette);
			var openChestUR = makeTile(tiles[openChestIdx], 8, 0, tileReference[openChestIdx].palette);
			Put(TILESETPATTERNTABLE_OFFSET + (TILESETPATTERNTABLE_SIZEPERSET * (int)tileSet) + (OPEN_CHEST_UL_CHR_IDX * 16), EncodeForPPU(openChestUL));
			Put(TILESETPATTERNTABLE_OFFSET + (TILESETPATTERNTABLE_SIZEPERSET * (int)tileSet) + (OPEN_CHEST_UR_CHR_IDX * 16), EncodeForPPU(openChestUR));

		}

		//Split an image into 16x16 images
		private static List<Image<Rgba32>> SplitTiles(Image<Rgba32> tilemap)
		{
			int tilesX = tilemap.Width / 16;
			int tilesY = tilemap.Height / 16;

			List<Image<Rgba32>> tiles = new List<Image<Rgba32>>();
			for (int y = 0; y < tilesY; y++)
			{
				for (int x = 0; x < tilesX; x++)
				{
					Rectangle rect = new Rectangle(x * 16, y * 16, 16, 16);
					tiles.Add(tilemap.Clone(ctx => ctx.Crop(rect)));
				}
			}
			return tiles;
		}

		private void ReplacePlateSprite(Image<Rgba32> plateSprite)
		{
			int SPRITE_SIZE = 0x40;
			Dictionary<Rgba32, byte> topPalette = new Dictionary<Rgba32, byte>()
			{
				{Rgba32.ParseHex("FF00FFFF"), 0},  //magenta
				{NESpalette[0x30], 1},
				{NESpalette[0x00], 2},
				{NESpalette[0x10], 3}
			};
			Dictionary<Rgba32, byte> bottomPalette = new Dictionary<Rgba32, byte>()
			{
				{Rgba32.ParseHex("FF00FFFF"), 0},  //magenta
				{NESpalette[0x00], 1},
				{NESpalette[0x30], 2},
				{NESpalette[0x10], 3}
			};

			byte[] spriteBytes = new byte[SPRITE_SIZE];
			EncodeForPPU(makeTileQuantize(plateSprite, 0, 0, topPalette	  )).CopyTo(spriteBytes, 0x00);
			EncodeForPPU(makeTileQuantize(plateSprite, 0, 8, topPalette	  )).CopyTo(spriteBytes, 0x10);
			EncodeForPPU(makeTileQuantize(plateSprite, 8, 0, bottomPalette)).CopyTo(spriteBytes, 0x20);
			EncodeForPPU(makeTileQuantize(plateSprite, 8, 8, bottomPalette)).CopyTo(spriteBytes, 0x30);

			Put(PLATE_SPRITE_LOCATION + (SPRITE_SIZE * 0), spriteBytes);
			Put(PLATE_SPRITE_LOCATION + (SPRITE_SIZE * 1), spriteBytes);
			Put(PLATE_SPRITE_LOCATION + (SPRITE_SIZE * 2), spriteBytes);
			Put(PLATE_SPRITE_LOCATION + (SPRITE_SIZE * 3), spriteBytes);
		}
		private void ReplaceOrbSprite(Image<Rgba32> orbSprite)
		{
			int SPRITE_SIZE = 0x40;
			Dictionary<Rgba32, byte> orbPalette = new Dictionary<Rgba32, byte>()
			{
				{Rgba32.ParseHex("FF00FFFF"), 0},
				{NESpalette[0x00], 1},
				{NESpalette[0x10], 2},
				{NESpalette[0x30], 3}
			};

			byte[] spriteBytes = new byte[SPRITE_SIZE];
			EncodeForPPU(makeTileQuantize(orbSprite, 0, 0, orbPalette)).CopyTo(spriteBytes, 0x00);
			EncodeForPPU(makeTileQuantize(orbSprite, 0, 8, orbPalette)).CopyTo(spriteBytes, 0x10);
			EncodeForPPU(makeTileQuantize(orbSprite, 8, 0, orbPalette)).CopyTo(spriteBytes, 0x20);
			EncodeForPPU(makeTileQuantize(orbSprite, 8, 8, orbPalette)).CopyTo(spriteBytes, 0x30);

			Put(ORB_SPRITE_LOCATION + (SPRITE_SIZE * 0), spriteBytes);
			Put(ORB_SPRITE_LOCATION + (SPRITE_SIZE * 1), spriteBytes);
			Put(ORB_SPRITE_LOCATION + (SPRITE_SIZE * 2), spriteBytes);
			Put(ORB_SPRITE_LOCATION + (SPRITE_SIZE * 3), spriteBytes);
		}

		//LUTs for mapping an imported image to their corresponding tiles and palettes used for reading those tiles
		//Reference lists must be in the same order as the tiles they correspond to in the imported image
		private class TileGraphicsRefData
		{
			public static byte PLATE_SPRITE_TILE = 0x80;
			public static byte ORB_SPRITE_TILE = 0x81;

			FF1Rom rom;
			public Dictionary<Rgba32, byte> outRoomPalette { get; }
			public Dictionary<Rgba32, byte> inRoomPalette { get; } //rooms have their palettes inverted in rom
			public Dictionary<Rgba32, byte> roomWallPalette { get; } //compromise to allow all detail to be shown properly
			public Dictionary<Rgba32, byte> spritePalette { get; }
			List<TileDetails> CastleTileReference, MarshMirageReference, EarthTitanVolcanoReference, ToFSeaShrineReference, MatoyaDwarfCardiaIceWaterfallReference, SkyCastleReference, ToFRReference;
			Dictionary<TileSets,  List<TileDetails>> TileSetDetailsMap;

			public TileGraphicsRefData(FF1Rom rom)
			{
				this.rom = rom;
				outRoomPalette = new Dictionary<Rgba32, byte>()
				{
					{rom.NESpalette[0x0F], 0},
					{rom.NESpalette[0x00], 1},
					{rom.NESpalette[0x10], 2},
					{rom.NESpalette[0x30], 3}
				};
				//Palettes for tiles that go inside a room are inverted in ROM
				inRoomPalette = new Dictionary<Rgba32, byte>()
				{
					{rom.NESpalette[0x30], 0},
					{rom.NESpalette[0x10], 1},
					{rom.NESpalette[0x00], 2},
					{rom.NESpalette[0x0F], 3}
				};
				//This is a compromise between the in room and out of room palettes to make tile design easier
				roomWallPalette = new Dictionary<Rgba32, byte>()
				{
					{rom.NESpalette[0x00], 0},
					{rom.NESpalette[0x0F], 1},
					{rom.NESpalette[0x10], 2},
					{rom.NESpalette[0x30], 3}
				};
				spritePalette = new Dictionary<Rgba32, byte>()
				{
					{Rgba32.ParseHex("FF00FFFF"), 0},
					{rom.NESpalette[0x00], 1},
					{rom.NESpalette[0x10], 2},
					{rom.NESpalette[0x30], 3}
				};
				TileDetails UNUSED_TILE = new() { palette = roomWallPalette, tilenums = new() { } };

				CastleTileReference = new List<TileDetails>
				{

					//imported tilesheet row 1
					new() {palette = roomWallPalette, tilenums = new() {0x00} }, //room UL
					new() {palette = roomWallPalette, tilenums = new() {0x01} }, //room U
					new() {palette = roomWallPalette, tilenums = new() {0x02} }, //room UR
					new() {palette = outRoomPalette,  tilenums = new() {0x34} }, //wall UL
					new() {palette = outRoomPalette,  tilenums = new() {0x30} }, //wall
					new() {palette = outRoomPalette,  tilenums = new() {0x35} }, //wall UR
					//row 2
					new() {palette = roomWallPalette, tilenums = new() {0x03} }, //room L
					new() {palette = roomWallPalette, tilenums = new() {0x04, 0x59, 0x5A, 0x5B, 0x5D, 0x5E, 0x5F, 0x0A, 0x0C, 0x0E, 0x13} }, //room C
					new() {palette = roomWallPalette, tilenums = new() {0x05} }, //room R
					new() {palette = outRoomPalette,  tilenums = new() {0x32} }, //wall L
					new() {palette = outRoomPalette,  tilenums = new() {0x31, 0x3A, 0x60} }, //stone floor
					new() {palette = outRoomPalette,  tilenums = new() {0x33} }, //wall R
					//row 3
					new() {palette = roomWallPalette, tilenums = new() {0x06} }, //room DL
					new() {palette = roomWallPalette, tilenums = new() {0x07, 0x5C} }, //room D
					new() {palette = roomWallPalette, tilenums = new() {0x08} }, //room DR
					new() {palette = outRoomPalette,  tilenums = new() {0x38, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x58} }, //pillar
					new() {palette = outRoomPalette,  tilenums = new() {0x44, 0x45} }, //stairs up
					new() {palette = outRoomPalette,  tilenums = new() {0x48, 0x49} }, //stairs down
					//row 4
					new() {palette = outRoomPalette,  tilenums = new() {0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E,
																		0x2F, 0x3E, 0x3F, 0x41, 0x42, 0x43, 0x46, 0x47, 0x4A, 0x4B, 0x56, 0x57, 0x79,
																		0x7A, 0x7B, 0x7C, 0x7D, 0x7E} }, //empty black
					new() {palette = outRoomPalette,  tilenums = new() {0x36, 0x3B} }, //door closed
					new() {palette = outRoomPalette,  tilenums = new() {0x37} }, //door open
					new() {palette = outRoomPalette,  tilenums = new() {0x09, 0x0B} }, //empty black 2
					new() {palette = outRoomPalette,  tilenums = new() {0x21, 0x39, 0x3D, 0x40} }, //grass
					new() {palette = outRoomPalette,  tilenums = new() {0x3C} }, //solid blue
					//row 5
					new() {palette = inRoomPalette,  tilenums = new() {0x11} }, //stool L
					new() {palette = inRoomPalette,  tilenums = new() {0x15} }, //table U
					new() {palette = inRoomPalette,  tilenums = new() {0x12} }, //stool R
					new() {palette = inRoomPalette,  tilenums = new() {0x1E} }, //throne UL
					new() {palette = inRoomPalette,  tilenums = new() {0x1F} }, //throne U
					new() {palette = inRoomPalette,  tilenums = new() {0x20} }, //throne UR
					//row 6
					new() {palette = inRoomPalette,  tilenums = new() {0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E,
																	   0x6F, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78} }, //chest closed
					new() {palette = inRoomPalette,  tilenums = new() {0x16} }, //table C
					new() {palette = inRoomPalette,  tilenums = new() {0x0F} }, //bed U
					new() {palette = inRoomPalette,  tilenums = new() {0x1B} }, //throne DL
					new() {palette = inRoomPalette,  tilenums = new() {0x1C, 0x61, 0x62} }, //throne D
					new() {palette = inRoomPalette,  tilenums = new() {0x1D} }, //throne DR
					//row 7
					new() {palette = inRoomPalette,  tilenums = new() {0x7F} }, //chest open
					new() {palette = inRoomPalette,  tilenums = new() {0x17} }, //table D
					new() {palette = inRoomPalette,  tilenums = new() {0x10} }, //bed D
					new() {palette = inRoomPalette,  tilenums = new() {0x19} }, //ornament L
					new() {palette = inRoomPalette,  tilenums = new() {0x0D, 0x14, 0x18} }, //small table
					new() {palette = inRoomPalette,  tilenums = new() {0x1A} }, //ornament R
				};
				MarshMirageReference = new List<TileDetails>
				{
					//imported tilesheet row 1
					new() {palette = roomWallPalette, tilenums = new() {0x00} }, //room UL
					new() {palette = roomWallPalette, tilenums = new() {0x01} }, //room U
					new() {palette = roomWallPalette, tilenums = new() {0x02} }, //room UR
					new() {palette = outRoomPalette,  tilenums = new() {0x34} }, //wall UL
					new() {palette = outRoomPalette,  tilenums = new() {0x30} }, //wall
					new() {palette = outRoomPalette,  tilenums = new() {0x35} }, //wall UR
					//row 2
					new() {palette = roomWallPalette, tilenums = new() {0x03} }, //room L
					new() {palette = roomWallPalette, tilenums = new() {0x04, 0x19, 0x1A, 0x1B, 0x1D, 0x2D, 0x2E, 0x2F} }, //room C
					new() {palette = roomWallPalette, tilenums = new() {0x05} }, //room R
					new() {palette = outRoomPalette,  tilenums = new() {0x32} }, //wall L
					new() {palette = outRoomPalette,  tilenums = new() {0x31, 0x3A, 0x40} }, //stone floor
					new() {palette = outRoomPalette,  tilenums = new() {0x33} }, //wall R
					//row 3
					new() {palette = roomWallPalette, tilenums = new() {0x06} }, //room DL
					new() {palette = roomWallPalette, tilenums = new() {0x07, 0x1C} }, //room D
					new() {palette = roomWallPalette, tilenums = new() {0x08} }, //room DR
					new() {palette = outRoomPalette,  tilenums = new() {0x39} }, //pillar
					new() {palette = outRoomPalette,  tilenums = new() {0x26, 0x27, 0x28} }, //stairs up
					new() {palette = outRoomPalette,  tilenums = new() {0x20, 0x21, 0x22, 0x23} }, //stairs down
					//row 4
					new() {palette = outRoomPalette,  tilenums = new() {0x0B, 0x0C, 0x0F, 0x10, 0x11, 0x13, 0x15, 0x16,	0x18,
						0x1E, 0x1F, 0x24, 0x25, 0x29, 0x2A, 0x2B, 0x3C, 0x3D, 0x3E, 0x3F, 0x67, 0x68, 0x69,
						0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77,	0x78, 0x79, 0x7A,
						0x7B, 0x7C, 0x7D, 0x7E } }, //empty black
					new() {palette = outRoomPalette,  tilenums = new() {0x36, 0x3B} }, //door closed
					new() {palette = outRoomPalette,  tilenums = new() {0x37} }, //door open
					UNUSED_TILE,
					UNUSED_TILE,
					UNUSED_TILE,
					//row 5
					new() {palette = inRoomPalette,  tilenums = new() { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49,
						0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A,
						0x5B, 0x5C, 0x5D, 0x5E, 0x5F, 0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67 } }, //closed chest
					new() {palette = inRoomPalette,  tilenums = new() { 0x12 } }, //pillar 2
					new() {palette = inRoomPalette,  tilenums = new() { 0x0D } }, //stacked squares
					new() {palette = inRoomPalette,  tilenums = new() { 0x2C, 0x38 } }, //teleport to sky
					new() {palette = inRoomPalette,  tilenums = new() { 0x09 } }, //ladder up
					new() {palette = inRoomPalette,  tilenums = new() { 0x0A } }, //ladder down
					//row 6
					new() {palette = inRoomPalette,  tilenums = new() { 0x7F } }, //open chest
					new() {palette = inRoomPalette,  tilenums = new() { 0x0E } }, //computers 1
					new() {palette = inRoomPalette,  tilenums = new() { 0x13 } }, //computers 2
					new() {palette = inRoomPalette,  tilenums = new() { 0x14 } }, //computers 3
					new() {palette = inRoomPalette,  tilenums = new() { 0x11 } }, //computers 3
					new() {palette = inRoomPalette,  tilenums = new() { 0x17 } }, //decoration
				};

				EarthTitanVolcanoReference = new List<TileDetails>
				{
					//imported tilesheet row 1
					new() {palette = roomWallPalette, tilenums = new() {0x00} }, //room UL
					new() {palette = roomWallPalette, tilenums = new() {0x01} }, //room U
					new() {palette = roomWallPalette, tilenums = new() {0x02} }, //room UR
					new() {palette = outRoomPalette,  tilenums = new() {0x34} }, //wall UL
					new() {palette = outRoomPalette,  tilenums = new() {0x30} }, //wall
					new() {palette = outRoomPalette,  tilenums = new() {0x35} }, //wall UR
					//row 2
					new() {palette = roomWallPalette, tilenums = new() {0x03} }, //room L
					new() {palette = roomWallPalette, tilenums = new() {0x04, 0x09, 0x0A, 0x0C, 0x12, 0x17, 0x1A, 0x1D, 0x1E, 0x21, 0x22, 0x23, 0x2E, 0x2F, 0x40} }, //room C
					new() {palette = roomWallPalette, tilenums = new() {0x05} }, //room R
					new() {palette = outRoomPalette,  tilenums = new() {0x32} }, //lava wall L
					new() {palette = outRoomPalette,  tilenums = new() {0x0B, 0x1B, 0x1C, 0x20, 0x31, 0x3A, 0x41} }, //lava floor
					new() {palette = outRoomPalette,  tilenums = new() {0x33} }, //lava wall R
					//row 3
					new() {palette = roomWallPalette, tilenums = new() {0x06} }, //room DL
					new() {palette = roomWallPalette, tilenums = new() {0x07} }, //room D
					new() {palette = roomWallPalette, tilenums = new() {0x08} }, //room DR
					new() {palette = outRoomPalette,  tilenums = new() {0x1A, 0x38, 0x39, 0x3C} }, //empty black
					new() {palette = outRoomPalette,  tilenums = new() {0x15, 0x16, 0x18, 0x19} }, //stairs up
					new() {palette = outRoomPalette,  tilenums = new() {0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D} }, //stairs down
					//row 4
					UNUSED_TILE,
					new() {palette = outRoomPalette,  tilenums = new() {0x36, 0x3B} }, //door closed
					new() {palette = outRoomPalette,  tilenums = new() {0x37} }, //door open
					new() {palette = outRoomPalette,  tilenums = new() {0x3D} }, //damage tile
					new() {palette = outRoomPalette,  tilenums = new() {0x3E} }, //stalagmite 1
					new() {palette = outRoomPalette,  tilenums = new() {0x3F} }, //stalagmite 2
					//row 5
					new() {palette = inRoomPalette,   tilenums = new() { 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C,
						 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F,
						 0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71, 0x72,
						 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E } }, //chest closed
					new() {palette = inRoomPalette,   tilenums = new() {0x0D} }, //candelabra
					new() {palette = inRoomPalette,   tilenums = new() {0x0F, 0x1F} }, //exit warp tile
					UNUSED_TILE,
					new() {palette = inRoomPalette,   tilenums = new() {0x13} }, //spikes 1
					new() {palette = inRoomPalette,   tilenums = new() {0x14} }, //spikes 2
					//row 6
					new() {palette = inRoomPalette,   tilenums = new() {0x7F} }, //open chest
					new() {palette = inRoomPalette,   tilenums = new() {0x10} }, //orb altar L
					new() {palette = inRoomPalette,   tilenums = new() {0x0E, 0x17} }, //orb altar C
					new() {palette = inRoomPalette,   tilenums = new() {0x11} }, //orb altar R
					new() {palette = spritePalette,   tilenums = new() {PLATE_SPRITE_TILE} }, //rod plate
					new() {palette = spritePalette,   tilenums = new() {ORB_SPRITE_TILE} }, //orb
				};
				ToFSeaShrineReference = new List<TileDetails>
				{
					//imported tilesheet row 1
					new() {palette = roomWallPalette, tilenums = new() {0x00} }, //room UL
					new() {palette = roomWallPalette, tilenums = new() {0x01} }, //room U
					new() {palette = roomWallPalette, tilenums = new() {0x02} }, //room UR
					new() {palette = outRoomPalette,  tilenums = new() {0x34} }, //wall UL
					new() {palette = outRoomPalette,  tilenums = new() {0x30} }, //wall
					new() {palette = outRoomPalette,  tilenums = new() {0x35} }, //wall UR
					//row 2
					new() {palette = roomWallPalette, tilenums = new() {0x03} }, //room L
					new() {palette = roomWallPalette, tilenums = new() {0x04, 0x0A, 0x0C, 0x0D, 0x15, 0x16, 0x17, 0x20, 0x21, 0x22, 0x23, 0x24, 0x53, 0x54} }, //room C
					new() {palette = roomWallPalette, tilenums = new() {0x05} }, //room R
					new() {palette = outRoomPalette,  tilenums = new() {0x32} }, //wall L
					new() {palette = outRoomPalette,  tilenums = new() {0x31, 0x3A, 0x55} }, //floor
					new() {palette = outRoomPalette,  tilenums = new() {0x33} }, //wall R
					//row 3
					new() {palette = roomWallPalette, tilenums = new() {0x06} }, //room DL
					new() {palette = roomWallPalette, tilenums = new() {0x07} }, //room D
					new() {palette = roomWallPalette, tilenums = new() {0x08} }, //room DR
					new() {palette = outRoomPalette,  tilenums = new() {0x38} }, //pillar
					new() {palette = outRoomPalette,  tilenums = new() {0x42, 0x43, 0x44, 0x45, 0x46} }, //stairs up
					new() {palette = outRoomPalette,  tilenums = new() {0x4B, 0x4C, 0x4D, 0x4E, 0x4F} }, //stairs down
					//row 4
					new() {palette = outRoomPalette,  tilenums = new() {0x09, 0x0B, 0x0E, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E,
						0x1F, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x39, 0x3D, 0x3E, 0x3F, 0x47, 0x48,
						0x49, 0x4A, 0x50, 0x51, 0x52, 0x7C, 0x7D, 0x7E} }, //empty black
					new() {palette = outRoomPalette,  tilenums = new() {0x36, 0x3B} }, //door closed
					new() {palette = outRoomPalette,  tilenums = new() {0x37} }, //door open
					UNUSED_TILE,
					new() {palette = outRoomPalette,  tilenums = new() {0x3C} }, //water
					new() {palette = outRoomPalette,  tilenums = new() {0x41} }, //sea entrance
					//row 5
					new() {palette = inRoomPalette ,  tilenums = new() {0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F,
						0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71,
						0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B} }, //chest closed
					UNUSED_TILE,
					new() {palette = inRoomPalette ,  tilenums = new() {0x0F} }, //exit warp tile
					new() {palette = inRoomPalette ,  tilenums = new() {0x40} }, //ToFR entrance warp tile
					UNUSED_TILE,
					UNUSED_TILE,
					//row 6
					new() {palette = inRoomPalette ,  tilenums = new() {0x7F} }, //chest open
					new() {palette = inRoomPalette,   tilenums = new() {0x12} }, //orb altar L
					new() {palette = inRoomPalette,   tilenums = new() {0x13} }, //orb altar C
					new() {palette = inRoomPalette,   tilenums = new() {0x14} }, //orb altar R
					new() {palette = inRoomPalette,   tilenums = new() {0x10} }, //statue L
					new() {palette = inRoomPalette,   tilenums = new() {0x11} }, //statue R
				};
				MatoyaDwarfCardiaIceWaterfallReference = new List<TileDetails>
				{
					//imported tilesheet row 1
					new() {palette = roomWallPalette, tilenums = new() {0x00} }, //room UL
					new() {palette = roomWallPalette, tilenums = new() {0x01} }, //room U
					new() {palette = roomWallPalette, tilenums = new() {0x02} }, //room UR
					new() {palette = outRoomPalette,  tilenums = new() {0x34} }, //wall UL
					new() {palette = outRoomPalette,  tilenums = new() {0x30} }, //wall
					new() {palette = outRoomPalette,  tilenums = new() {0x35} }, //wall UR
					//row 2
					new() {palette = roomWallPalette, tilenums = new() {0x03} }, //room L
					new() {palette = roomWallPalette, tilenums = new() {0x04, 0x0A, 0x0C, 0x41, 0x42, 0x43, 0x45, 0x46, 0x47} }, //room C
					new() {palette = roomWallPalette, tilenums = new() {0x05} }, //room R
					new() {palette = outRoomPalette,  tilenums = new() {0x32} }, //wall L
					new() {palette = outRoomPalette,  tilenums = new() {0x31, 0x3A, 0x49} }, //floor
					new() {palette = outRoomPalette,  tilenums = new() {0x33} }, //wall R
					//row 3
					new() {palette = roomWallPalette, tilenums = new() {0x06} }, //room DL
					new() {palette = roomWallPalette, tilenums = new() {0x07, 0x40, 0x44, 0x48} }, //room D
					new() {palette = roomWallPalette, tilenums = new() {0x08} }, //room DR
					new() {palette = outRoomPalette,  tilenums = new() {0x38} }, //well
					new() {palette = outRoomPalette,  tilenums = new() {0x18, 0x19, 0x1A, 0x1B} }, //stairs up
					new() {palette = outRoomPalette,  tilenums = new() {0x28, 0x29, 0x2A, 0x2B, 0x2C } }, //stairs down
					//row 4
					new() {palette = outRoomPalette,  tilenums = new() {0x09, 0x0B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x23,
																		0x24, 0x25, 0x26, 0x27, 0x2F, 0x3C, 0x3E, 0x3F, 0x4A} }, //empty black
					new() {palette = outRoomPalette,  tilenums = new() {0x36, 0x3B} }, //door closed
					new() {palette = outRoomPalette,  tilenums = new() {0x37} }, //door open
					new() {palette = outRoomPalette,  tilenums = new() {0x39} }, //damage tile
					UNUSED_TILE,
					new() {palette = outRoomPalette,  tilenums = new() {0x3D} }, //rubble wall
					//row 5
					new() {palette = inRoomPalette,   tilenums = new() {0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54,
						0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F, 0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66,
						0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78,
						0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E } }, //chest closed
					new() {palette = inRoomPalette,   tilenums = new() {0x14} }, //bed top
					new() {palette = inRoomPalette,   tilenums = new() {0x0F} }, //chair
					new() {palette = inRoomPalette,   tilenums = new() {0x12} }, //pots
					UNUSED_TILE,
					new() {palette = inRoomPalette,   tilenums = new() {0x2D, 0x2E} }, //drop down hole
					//row 6
					new() {palette = inRoomPalette,   tilenums = new() {0x7F} }, //chest open
					new() {palette = inRoomPalette,   tilenums = new() {0x15} }, //bed bottom
					new() {palette = inRoomPalette,   tilenums = new() {0x10} }, //table
					UNUSED_TILE,
					new() {palette = inRoomPalette,   tilenums = new() {0x11} }, //fireplace
					new() {palette = inRoomPalette,   tilenums = new() {0x17} }, //hammer
					//row 7
					UNUSED_TILE,
					UNUSED_TILE,
					new() {palette = inRoomPalette,   tilenums = new() {0x0D} }, //candelabra
					new() {palette = inRoomPalette,   tilenums = new() {0x13} }, //skull
					new() {palette = inRoomPalette,   tilenums = new() {0x0E} }, //sword
					new() {palette = inRoomPalette,   tilenums = new() {0x16} }, //anvil
				};

				SkyCastleReference = new List<TileDetails>
				{
					//imported tilesheet row 1
					new() {palette = roomWallPalette, tilenums = new() {0x00} }, //room UL
					new() {palette = roomWallPalette, tilenums = new() {0x01} }, //room U
					new() {palette = roomWallPalette, tilenums = new() {0x02} }, //room UR
					new() {palette = outRoomPalette,  tilenums = new() {0x34} }, //wall UL
					new() {palette = outRoomPalette,  tilenums = new() {0x30} }, //wall
					new() {palette = outRoomPalette,  tilenums = new() {0x35} }, //wall UR
					//row 2
					new() {palette = roomWallPalette, tilenums = new() {0x03} }, //room L
					new() {palette = roomWallPalette, tilenums = new() {0x04, 0x0F, 0x10, 0x11, 0x15, 0x17, 0x49, 0x4A} }, //room C
					new() {palette = roomWallPalette, tilenums = new() {0x05} }, //room R
					new() {palette = outRoomPalette,  tilenums = new() {0x32} }, //wall L
					new() {palette = outRoomPalette,  tilenums = new() {0x31, 0x3A, 0x4B} }, //floor
					new() {palette = outRoomPalette,  tilenums = new() {0x33} }, //wall R
					//row 3
					new() {palette = roomWallPalette, tilenums = new() {0x06} }, //room DL
					new() {palette = roomWallPalette, tilenums = new() {0x07} }, //room D
					new() {palette = roomWallPalette, tilenums = new() {0x08} }, //room DR
					new() {palette = outRoomPalette,  tilenums = new() {0x09, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20, 0x21,
						0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x3C, 0x3D, 0x3E,
						0x3F, 0x46,	0x47, 0x48, 0x6E, 0x6F, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A,
						0x7B, 0x7C, 0x7D, 0x7E} }, //empty black
					new() {palette = outRoomPalette,  tilenums = new() {0x41, 0x42, 0x43, 0x44} }, //teleporter up
					new() {palette = outRoomPalette,  tilenums = new() {0x45} }, //teleporter down
					//row 4
					UNUSED_TILE,
					new() {palette = outRoomPalette,  tilenums = new() {0x36, 0x3B} }, //door closed
					new() {palette = outRoomPalette,  tilenums = new() {0x37} }, //door open
					UNUSED_TILE,
					new() {palette = outRoomPalette,  tilenums = new() {0x38} }, //water
					new() {palette = outRoomPalette,  tilenums = new() {0x19, 0x39} }, //background
					//row 5
					new() {palette = inRoomPalette,   tilenums = new() {0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54,
						0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F, 0x60, 0x61, 0x62, 0x63, 0x64, 0x65,
						0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D} }, //chest closed
					UNUSED_TILE,
					new() {palette = inRoomPalette,   tilenums = new() {0x0A} }, //exit warp tile
					UNUSED_TILE,
					UNUSED_TILE,
					new() {palette = inRoomPalette,   tilenums = new() {0x40} }, //warp back to mirage
					//row 6
					new() {palette = inRoomPalette,   tilenums = new() {0x7F} }, //chest open
					new() {palette = inRoomPalette,   tilenums = new() {0x0C} }, //orb altar L
					new() {palette = inRoomPalette,   tilenums = new() {0x0D} }, //orb altar C
					new() {palette = inRoomPalette,   tilenums = new() {0x0E} }, //orb altar R
					new() {palette = inRoomPalette,   tilenums = new() {0x18} }, //tablet
					new() {palette = inRoomPalette,   tilenums = new() {0x16} }, //chair
					//row 7
					UNUSED_TILE,
					UNUSED_TILE,
					new() {palette = inRoomPalette,   tilenums = new() {0x0B} }, //triangle floor
					new() {palette = inRoomPalette,   tilenums = new() {0x12} }, //computers 1
					new() {palette = inRoomPalette,   tilenums = new() {0x13} }, //computers 2
					new() {palette = inRoomPalette,   tilenums = new() {0x14} }, //computers 3
				};

				ToFRReference = new List<TileDetails>
				{
					//imported tilesheet row 1
					new() {palette = roomWallPalette, tilenums = new() {0x00} }, //room UL
					new() {palette = roomWallPalette, tilenums = new() {0x01} }, //room U
					new() {palette = roomWallPalette, tilenums = new() {0x02} }, //room UR
					new() {palette = outRoomPalette,  tilenums = new() {0x34} }, //wall UL
					new() {palette = outRoomPalette,  tilenums = new() {0x30} }, //wall
					new() {palette = outRoomPalette,  tilenums = new() {0x35} }, //wall UR
					//row 2
					new() {palette = roomWallPalette, tilenums = new() {0x03} }, //room L
					new() {palette = roomWallPalette, tilenums = new() {0x04, 0x0C, 0x0D, 0x20, 0x55} }, //room C
					new() {palette = roomWallPalette, tilenums = new() {0x05} }, //room R
					new() {palette = outRoomPalette,  tilenums = new() {0x32} }, //wall L
					new() {palette = outRoomPalette,  tilenums = new() {0x31, 0x3A, 0x3E, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C} }, //floor
					new() {palette = outRoomPalette,  tilenums = new() {0x33} }, //wall R
					//row 3
					new() {palette = roomWallPalette, tilenums = new() {0x06} }, //room DL
					new() {palette = roomWallPalette, tilenums = new() {0x07, 0x56} }, //room D
					new() {palette = roomWallPalette, tilenums = new() {0x08} }, //room DR
					new() {palette = outRoomPalette,  tilenums = new() {0x38} }, //pillar
					new() {palette = outRoomPalette,  tilenums = new() {0x41, 0x42, 0x43} }, //stairs up
					new() {palette = outRoomPalette,  tilenums = new() {0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, } }, //stairs down
					//row 4
					new() {palette = outRoomPalette,  tilenums = new() {0x0B, 0x0E, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D,
						0x1E, 0x1F,	0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x3F, 0x44, 0x45, 0x46, 0x47,
						0x48, 0x49, 0x4A, 0x54, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71, 0x72,
						0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E} }, //empty black
					new() {palette = outRoomPalette,  tilenums = new() {0x36, 0x3B} }, //door closed
					new() {palette = outRoomPalette,  tilenums = new() {0x37} }, //door open
					new() {palette = outRoomPalette,  tilenums = new() {0x39} }, //pedestal
					new() {palette = outRoomPalette,  tilenums = new() {0x3C} }, //grass background
					new() {palette = outRoomPalette,  tilenums = new() {0x3D} }, //blue background
					//row 5
					new() {palette = inRoomPalette,   tilenums = new() {0x5D, 0x5E, 0x5F, 0x60, 0x61, 0x62, 0x63, 0x64} }, //chest closed
					UNUSED_TILE,
					new() {palette = inRoomPalette,   tilenums = new() {0x0F} }, //decoration above lute plate
					new() {palette = inRoomPalette,   tilenums = new() {0x40} }, //warp tile
					new() {palette = inRoomPalette,   tilenums = new() {0x09} }, //ladder up
					new() {palette = inRoomPalette,   tilenums = new() {0x0A} }, //ladder down
					//row 6
					new() {palette = inRoomPalette,   tilenums = new() {0x7F} }, //chest open
					new() {palette = inRoomPalette,   tilenums = new() {0x12} }, //orb altar L
					new() {palette = inRoomPalette,   tilenums = new() {0x13} }, //orb altar C
					new() {palette = inRoomPalette,   tilenums = new() {0x14} }, //orb altar R
					new() {palette = inRoomPalette,   tilenums = new() {0x10} }, //statue L
					new() {palette = inRoomPalette,   tilenums = new() {0x11} }, //statue R
					//row 7
					new() {palette = inRoomPalette,   tilenums = new() {0x15} }, //tablet
					UNUSED_TILE,
					new() {palette = inRoomPalette,   tilenums = new() {0x2C} }, //earth decoration
					new() {palette = inRoomPalette,   tilenums = new() {0x2D} }, //water decoration
					new() {palette = inRoomPalette,   tilenums = new() {0x2E} }, //fire decoration
					new() {palette = inRoomPalette,   tilenums = new() {0x2F} }, //air decoration
				};

				TileSetDetailsMap = new Dictionary<TileSets, List<TileDetails>>()
				{
					{ TileSets.Castle, CastleTileReference },
					{ TileSets.MarshMirage, MarshMirageReference },
					{ TileSets.EarthTitanVolcano, EarthTitanVolcanoReference },
					{ TileSets.ToFSeaShrine, ToFSeaShrineReference },
					{ TileSets.MatoyaDwarfCardiaIceWaterfall, MatoyaDwarfCardiaIceWaterfallReference },
					{ TileSets.SkyCastle, SkyCastleReference },
					{ TileSets.ToFR, ToFRReference }
				};
			}
			public List<TileDetails> DetailsForTileset(TileSets tileSets)
			{
				return TileSetDetailsMap[tileSets];
			}
			public int OpenChestImageIndexForTileset(TileSets tileSets)
			{
				var tileref = TileSetDetailsMap[tileSets];
				TileDetails chesttiledetails = tileref.Find(x => x.tilenums.Contains(rom.OPEN_CHEST_TILE));
				int chestidx = tileref.IndexOf(chesttiledetails);
				return chestidx;
			}
		}
	}
}
