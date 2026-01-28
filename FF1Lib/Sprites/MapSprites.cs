using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using SixLabors.ImageSharp.ColorSpaces;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
    {

		const int OVERWORLDPALETTE_OFFSET = 0x380;
		const int OVERWORLDPALETTE_ASSIGNMENT = 0x300;
		const int OVERWORLDPATTERNTABLE_OFFSET = 0x8000;
		const int OVERWORLDPATTERNTABLE_ASSIGNMENT = 0x100;
		const int TILESETPATTERNTABLE_OFFSET = 0xC000;
		const int TILESETPATTERNTABLE_ASSIGNMENT = 0x1000;
		const int TILESETPALETTE_ASSIGNMENT = 0x400;
		const int TILESET_TILEDATA = 0x800;
		const int MAPPALETTE_OFFSET = 0x2000;

		const int MAPTILESET_ASSIGNMENT = 0x2CC0;
        public async Task SetCustomMapGraphics(Stream readStream,
						 int maxCHR,
						 int maxPal,
						 int[] PALETTE_OFFSET,
						 int PALETTE_ASSIGNMENT,
						 int PATTERNTABLE_OFFSET,
						 int PATTERNTABLE_ASSIGNMENT,
						 bool towntiles = false)
		{
			//IImageFormat format;
			Image<Rgba32> image = Image.Load<Rgba32>(readStream);

			// palette for each terrain tile stored 0-127, each value is 0-3
			// starting from OVERWORLDPALETTE_ASSIGNMENT

			// usepalette = (cart->ROM[OVERWORLDPALETTE_ASSIGNMENT + imagecount] & 3) << 2;

			// each terrain tile consists of 4 actual pattern table tiles
			// OVERWORLDPATTERNTABLE_ASSIGNMENT is four sequential tables 128 bytes long
			// giving the patterntable entry for:
			// upper left, upper right, lower left, lower right

			// offset = OVERWORLDPATTERNTABLE_ASSIGNMENT + imagecount;

			// DrawTile(&mDC,0,0,cart,OVERWORLDPATTERNTABLE_OFFSET + (cart->ROM[offset] << 4),&palette[0][usepalette],cart->dat.TintTiles[0][imagecount]);
			// DrawTile(&mDC,8,0,cart,OVERWORLDPATTERNTABLE_OFFSET + (cart->ROM[offset + 128] << 4),&palette[0][usepalette],cart->dat.TintTiles[0][imagecount]);
			// DrawTile(&mDC,0,8,cart,OVERWORLDPATTERNTABLE_OFFSET + (cart->ROM[offset + 256] << 4),&palette[0][usepalette],cart->dat.TintTiles[0][imagecount]);
			// DrawTile(&mDC,8,8,cart,OVERWORLDPATTERNTABLE_OFFSET + (cart->ROM[offset + 384] << 4),&palette[0][usepalette],cart->dat.TintTiles[0][imagecount]);

			List<byte[]> chrEntries = new List<byte[]>();

			List<List<byte>> candidateMapPals = new List<List<byte>>();
			var toNEScolor = new Dictionary<Rgba32, byte>();

			for (int imagecount = 0; imagecount < 128; imagecount += 1)
			{
				int top = (imagecount / 16) * 16;
				int left = (imagecount % 16) * 16;

				var firstUnique = new Dictionary<Rgba32, int>();
				var colors = new List<Rgba32>();
				for (int y = top; y < (top + 16); y++)
				{
					for (int x = left; x < (left + 16); x++)
					{
						if (!colors.Contains(image[x, y]))
						{
							firstUnique[image[x, y]] = (x << 16 | y);
							colors.Add(image[x, y]);
						}
					}
				}
				List<byte> pal;
				if (!makeNTPalette(colors, NESpalette, out pal, toNEScolor))
				{
					await this.Progress($"WARNING: Failed importing map tile at {left}, {top}, too many unique colors (limit 4 unique colors):", 1 + pal.Count);
					for (int i = 0; i < pal.Count; i++)
					{
						await this.Progress($"WARNING: NES palette {i}: ${pal[i],2:X}");
					}
					/*foreach (var i in index) {
						int c = firstUnique[i.Key];
						Console.WriteLine($"RGB to index {i.Key}: {i.Value}  first appears at {c>>16}, {c & 0xFFFF}");
						}*/
					return;
				}
				candidateMapPals.Add(pal);
			}

			var mapPals = new List<List<byte>>();
			if (!mergePalettes(candidateMapPals, mapPals, maxPal))
			{

			}

			if (towntiles && mapPals.Count >= 4)
			{
				// check for palettes compatible with dialogue palette: [0x0F, 0x00, 0x01, 0x30]
				List<int> compatiblePals = new();
				for (int i = 0; i < mapPals.Count; i++)
				{
					var thisPal = mapPals[i];
					// see if the palette contains the following colors:
					if (thisPal.Contains(0x0F) && thisPal.Contains(0x00) && thisPal.Contains(0x30))
					{
						compatiblePals.Add(i);	
					}
				}
				// If there is one or more, swap the last one into position 3 (0 indexed)
				// we won't use that palette in game because it's overwritten for dialogues and menus,
				// possibly with a special color. More importantly, we care about moving the palette
				// that was in position 3 to one of the first 3, assuming that the imported tileset had
				// 4 palettes. If there's more than 4, we want to try to preserve the first 3 palettes if possible,
				// so this will swap in a later compatible palette...
				if (compatiblePals.Count != 0)
				{
					mapPals.Swap(compatiblePals.Last(),3);
					// now make sure the palette is in the correct order
					if (mapPals[3].Count < 4)
					{
						mapPals[3].Add(0x01);
					}
					byte extra = mapPals[3].First(i => i != 0x0F && i != 0x00 && i != 0x30);
					mapPals[3] = new() {0x0f,0x00,extra,0x30};
				}
				else
				{
					await this.Progress($"WARNING: town tilesheet has at least 4 palettes, but none of them is compatible with dialogue palette (needs to have black, white, dark gray, and one other color)");
				}
			}
			
			
			//int maxCHR = 245;
			int excessCHR = 0;
			Console.WriteLine($"mapPals {mapPals.Count}");

			for (int imagecount = 0; imagecount < 128; imagecount += 1)
			{
				int top = (imagecount / 16) * 16;
				int left = (imagecount % 16) * 16;

				int palidx = findPalette(mapPals, candidateMapPals[imagecount]);
				if (palidx == -1)
				{
					palidx = 0;
				}
				var usepal = (byte)palidx;

				Dictionary<Rgba32, byte> index;
				colorToPaletteIndex(mapPals[usepal], toNEScolor, out index);

				Put(PALETTE_ASSIGNMENT + imagecount, new byte[] { (byte)((usepal << 6) + (usepal << 4) + (usepal << 2) + (usepal)) });

				foreach (var loadchr in new ValueTuple<int, int, int>[]
						{ (0, 0, 0), (8, 0, 128), (0, 8, 256), (8, 8, 384) })
				{
					byte idx = chrIndex(makeTile(image, top + loadchr.Item2, left + loadchr.Item1, index), chrEntries, maxCHR);
					if (idx == 0xff)
					{
						await this.Progress($"WARNING: Error importing CHR at {left + loadchr.Item1}, {top + loadchr.Item2}, in map tile {imagecount} too many unique CHR");
						idx = 0;
						excessCHR++;
					}
					Put(PATTERNTABLE_ASSIGNMENT + loadchr.Item3 + imagecount, new byte[] { idx });
				}
			}

			if (mapPals.Count > 4)
			{
				await this.Progress($"WARNING: More than 4 unique 4-color palettes ({mapPals.Count})");
			}
			if (excessCHR > 0)
			{
				await this.Progress($"WARNING: More than {maxCHR} unique 8x8 tiles, must eliminate {excessCHR} excess tiles");
			}

			for (int i = 0; i < Math.Min(4, mapPals.Count); i++)
			{
				foreach (var j in PALETTE_OFFSET)
				{
					Put(j + i * 4, mapPals[i].ToArray());
				}
			}

			for (int i = 0; i < chrEntries.Count; i++)
			{
				Put(PATTERNTABLE_OFFSET + (i * 16), EncodeForPPU(chrEntries[i]));
			}
		}


        void renderTile(Image<Rgba32> img, byte[] tile, byte[] pal, int x, int y)
		{
			for (int i = 0; i < 64; i++)
			{
				img[x+(i%8), y+(i/8)] = NESpalette[pal[tile[i]]];
			}
	    }

	    public byte GetMapTilesetIndex(MapIndex MapIndex)
		{
			return Get(MAPTILESET_ASSIGNMENT + (int)MapIndex, 1)[0];
	    }

	    Image<Rgba32> exportMapTiles(MapIndex MapIndex,
									 bool inside,
									 int PATTERNTABLE_OFFSET,
									 int PATTERNTABLE_ASSIGNMENT)
		{
			var tileset = GetMapTilesetIndex(MapIndex);
			var tilesetProps = new TileSet(this, tileset);

			List<byte[]> palette = new();
			if (!inside)
			{
				palette.Add(Get(MAPPALETTE_OFFSET + ((int)MapIndex * 0x30) + 0, 4));
				palette.Add(Get(MAPPALETTE_OFFSET + ((int)MapIndex * 0x30) + 4, 4));
				palette.Add(Get(MAPPALETTE_OFFSET + ((int)MapIndex * 0x30) + 8, 4));
				palette.Add(Get(MAPPALETTE_OFFSET + ((int)MapIndex * 0x30) + 12, 4));
			}
			else
			{
				palette.Add(Get(MAPPALETTE_OFFSET + ((int)MapIndex * 0x30) + 0x20 + 0, 4));
				palette.Add(Get(MAPPALETTE_OFFSET + ((int)MapIndex * 0x30) + 0x20 + 4, 4));
				palette.Add(Get(MAPPALETTE_OFFSET + ((int)MapIndex * 0x30) + 0x20 + 8, 4));
				palette.Add(Get(MAPPALETTE_OFFSET + ((int)MapIndex * 0x30) + 0x20 + 12, 4));
			}

			var output = new Image<Rgba32>(16 * 16, 8 * 16);
			for (int imagecount = 0; imagecount < 128; imagecount += 1)
			{
				var pal = (int)tilesetProps.Tiles[imagecount].Palette;

				var pt1 = tilesetProps.Tiles[imagecount].TopLeftTile;
				var pt2 = tilesetProps.Tiles[imagecount].TopRightTile;
				var pt3 = tilesetProps.Tiles[imagecount].BottomLeftTile;
				var pt4 = tilesetProps.Tiles[imagecount].BottomRightTile;

				var chr1 = Get(PATTERNTABLE_OFFSET + (tileset << 11) + (pt1 * 16), 16);
				var chr2 = Get(PATTERNTABLE_OFFSET + (tileset << 11) + (pt2 * 16), 16);
				var chr3 = Get(PATTERNTABLE_OFFSET + (tileset << 11) + (pt3 * 16), 16);
				var chr4 = Get(PATTERNTABLE_OFFSET + (tileset << 11) + (pt4 * 16), 16);

				var dc1 = DecodePPU(chr1);
				var dc2 = DecodePPU(chr2);
				var dc3 = DecodePPU(chr3);
				var dc4 = DecodePPU(chr4);

				renderTile(output, dc1, palette[pal & 3], (imagecount % 16) * 16, (imagecount / 16) * 16);
				renderTile(output, dc2, palette[pal & 3], (imagecount % 16) * 16 + 8, (imagecount / 16) * 16);
				renderTile(output, dc3, palette[pal & 3], (imagecount % 16) * 16, (imagecount / 16) * 16 + 8);
				renderTile(output, dc4, palette[pal & 3], (imagecount % 16) * 16 + 8, (imagecount / 16) * 16 + 8);
			}

			return output;
		}

		public Image<Rgba32> ExportMapTiles(MapIndex MapIndex, bool inside)
		{
			return exportMapTiles(MapIndex, inside, TILESETPATTERNTABLE_OFFSET, TILESETPATTERNTABLE_ASSIGNMENT);
		}

		public Image<Rgba32> RenderMap(List<Map> maps, MapIndex MapIndex, bool inside)
		{
			var tiles = ExportMapTiles(MapIndex, inside);

			var output = new Image<Rgba32>(64 * 16, 64 * 16);

			for (int y = 0; y < 64; y++)
			{
				for (int x = 0; x < 64; x++)
				{
					var t = maps[(int)MapIndex][y, x];
					var tile_row = t / 16;
					var tile_col = t % 16;
					var src = tiles.Clone(d => d.Crop(new Rectangle(tile_col * 16, tile_row * 16, 16, 16)));
					output.Mutate(d => d.DrawImage(src, new Point(x * 16, y * 16), 1));
				}
			}

			return output;
		}

    }
}