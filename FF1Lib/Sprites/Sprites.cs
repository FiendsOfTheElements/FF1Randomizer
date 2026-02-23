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
		

		/*
			There's no such thing as a standard NES reference palette, because the NES PPU
			didn't encode RGB. Instead, it generated the composite signal directly from a set
			of 64 colors. Importing sprites requires finding the best fit to those 64 colors.
			How they are actually rendered on screen from an emulator depends on what palette
			the emulator is using. YMMV, etc.
		*/
		public Rgba32[] NESpalette = new Rgba32[]
		{
			//         R     G     B      Color number
			new Rgba32(0x7b, 0x7b, 0x7b), //0x00
			new Rgba32(0x00, 0x00, 0xff), //0x01
			new Rgba32(0x00, 0x00, 0xbf), //0x02
			new Rgba32(0x47, 0x2b, 0xbf), //0x03
			new Rgba32(0x97, 0x00, 0x87), //0x04
			new Rgba32(0xab, 0x00, 0x23), //0x05
			new Rgba32(0xab, 0x13, 0x00), //0x06
			new Rgba32(0x8b, 0x17, 0x00), //0x07
			new Rgba32(0x53, 0x30, 0x00), //0x08
			new Rgba32(0x00, 0x78, 0x00), //0x09
			new Rgba32(0x00, 0x6b, 0x00), //0x0A
			new Rgba32(0x00, 0x5b, 0x00), //0x0B
			new Rgba32(0x00, 0x43, 0x58), //0x0C
			new Rgba32(0x00, 0x00, 0x00), //0x0D
			new Rgba32(0x00, 0x00, 0x00), //0x0E
			new Rgba32(0x00, 0x00, 0x00), //0x0F

			new Rgba32(0xbd, 0xbd, 0xbd), //0x10
			new Rgba32(0x00, 0x78, 0xf8), //0x11
			new Rgba32(0x00, 0x58, 0xf8), //0x12
			new Rgba32(0x6b, 0x47, 0xff), //0x13
			new Rgba32(0xdb, 0x00, 0xcd), //0x14
			new Rgba32(0xe7, 0x00, 0x5b), //0x15
			new Rgba32(0xf8, 0x38, 0x00), //0x16
			new Rgba32(0xe7, 0x5f, 0x13), //0x17
			new Rgba32(0xaf, 0x7f, 0x00), //0x18
			new Rgba32(0x00, 0xb8, 0x00), //0x19
			new Rgba32(0x00, 0xab, 0x00), //0x1A
			new Rgba32(0x00, 0xab, 0x47), //0x1B
			new Rgba32(0x00, 0x8b, 0x8b), //0x1C
			new Rgba32(0x00, 0x00, 0x00), //0x1D
			new Rgba32(0x00, 0x00, 0x00), //0x1E
			new Rgba32(0x00, 0x00, 0x00), //0x1F

			new Rgba32(0xf8, 0xf8, 0xf8), //0x20
			new Rgba32(0x3f, 0xbf, 0xff), //0x21
			new Rgba32(0x6b, 0x88, 0xff), //0x22
			new Rgba32(0x98, 0x78, 0xf8), //0x23
			new Rgba32(0xf8, 0x78, 0xf8), //0x24
			new Rgba32(0xf8, 0x58, 0x98), //0x25
			new Rgba32(0xf8, 0x78, 0x58), //0x26
			new Rgba32(0xff, 0xa3, 0x47), //0x27
			new Rgba32(0xf8, 0xb8, 0x00), //0x28
			new Rgba32(0xb8, 0xf8, 0x18), //0x29
			new Rgba32(0x5b, 0xdb, 0x57), //0x2A
			new Rgba32(0x58, 0xf8, 0x98), //0x2B
			new Rgba32(0x00, 0xeb, 0xdb), //0x2C
			new Rgba32(0x78, 0x78, 0x78), //0x2D
			new Rgba32(0x00, 0x00, 0x00), //0x2E
			new Rgba32(0x00, 0x00, 0x00), //0x2F

			new Rgba32(0xff, 0xff, 0xff), //0x30
			new Rgba32(0xa7, 0xe7, 0xff), //0x31
			new Rgba32(0xb8, 0xb8, 0xf8), //0x32
			new Rgba32(0xd8, 0xb8, 0xf8), //0x33
			new Rgba32(0xf8, 0xb8, 0xf8), //0x34
			new Rgba32(0xfb, 0xa7, 0xc3), //0x35
			new Rgba32(0xf0, 0xd0, 0xb0), //0x36
			new Rgba32(0xff, 0xe3, 0xab), //0x37
			new Rgba32(0xfb, 0xdb, 0x7b), //0x38
			new Rgba32(0xd8, 0xf8, 0x78), //0x39
			new Rgba32(0xb8, 0xf8, 0xb8), //0x3A
			new Rgba32(0xb8, 0xf8, 0xd8), //0x3B
			new Rgba32(0x00, 0xff, 0xff), //0x3C
			new Rgba32(0xf8, 0xd8, 0xf8), //0x3D
			new Rgba32(0x00, 0x00, 0x00), //0x3E
			new Rgba32(0x00, 0x00, 0x00)  //0x3F
		};



		// This maps potentially problematic colors from the NES Palette to identical (or near identical) safe colors.
		Dictionary<byte, byte> colorReduction = new Dictionary<byte, byte>() {
			{ 0x0D, 0x0F },
			{ 0x0E, 0x0F },
			{ 0x1D, 0x0F },
			{ 0x1E, 0x0F },
			{ 0x1F, 0x0F },
			{ 0x2E, 0x0F },
			{ 0x2F, 0x0F },
			{ 0x3E, 0x0F },
			{ 0x3F, 0x0F },
			{ 0x2D, 0x00 },
			{ 0x3D, 0x10 },
			{ 0x20, 0x30 }
		};

		/*
			Grayscale palette for custom enemy and weapon sprites. Actual palettes will be applied
			in-game. In general, FF1 enemy palettes have the following format:
			Color 0: black
			Color 1: accent color
			Color 2: light main color
			Color 3; dark main color
			Follow the reference enemy sprite sheet for how these map to the Grayscale palette.
		*/
		public Dictionary<Rgba32, byte> GrayscaleIndex = new Dictionary<Rgba32, byte> {
			{ new Rgba32(0x00, 0x00, 0x00), 0 }, //0x0f black
			{ new Rgba32(0xff, 0xff, 0xff), 1 }, //0x30 white
			{ new Rgba32(0xbd, 0xbd, 0xbd), 2 }, //0x10 light gray
			{ new Rgba32(0x7b, 0x7b, 0x7b), 3 }  //0x00 dark gray
		};

		/*
			Grayscale palette for custom NPC sprites. Actual palettes will be applied in-game.
			In general, FF1 NPC palettes have the following format, similar to mapman sprites:
			Color 0: transparent
			Color 1: black
			Color 2: dark color
			Color 3: light color
		*/
		public Dictionary<Rgba32, byte> NPCGrayscaleIndex = new Dictionary<Rgba32, byte> {
			{ new Rgba32(0xff, 0x00, 0xff), 0 }, //magenta as transparent
			{ new Rgba32(0x00, 0x00, 0x00), 1 }, //0x0f black 
			{ new Rgba32(0x7b, 0x7b, 0x7b), 2 }, //0x00 dark gray
			{ new Rgba32(0xbd, 0xbd, 0xbd), 3 }  //0x10 light gray
		};


		public byte selectColor(Rgba32 px, Rgba32[] NESpalette)
		{
			/*
				This function takes an RGB color and selects the closest color from the NES palette,
				based on the Pythagorean distance in RGB space.
			*/

			// initialize with a large distance
			int min_dif = 1000000000;
			byte idx = 0;

			/*
				Go through each color in the NES palette, and find the color with the minimum distance from the source pixel.
				In a 3-dimensional space, distance from a point to the origin follows a generalized Pythagorean formula:
					D^2 == A^2 + B^2 + C^2
				To find the actual distance, we would take the square root of both sides of that equation, but since we're trying
				to minimize distance rather than use the distance as a number for anything, we can just minimize the distance squared
				and skip a potentially costly square root.
			*/
			for (int i = 0; i < NESpalette.Length; i++)
			{
				int dif = (NESpalette[i].R - px.R) * (NESpalette[i].R - px.R);
				dif += (NESpalette[i].G - px.G) * (NESpalette[i].G - px.G);
				dif += (NESpalette[i].B - px.B) * (NESpalette[i].B - px.B);
				if (dif < min_dif)
				{
					min_dif = dif;
					idx = (byte)i;
				}
			}

			// map problem colors to safe colors in the NES Palette
			if (colorReduction.ContainsKey(idx))
			{
				idx = colorReduction[idx];
			}
			return idx;
		}


		public byte[] EncodeForPPU(byte[] tile)
		{
			// Take an array of 64 bytes (i.e. an 8x8 tile) with an ordinary linear
			// encoding (left to right, top to bottom, one byte
			// per pixel, valid values 0-3) and return the
			// 16-byte, dual-plane encoding used by the NES PPU.
			//
			// see https://wiki.nesdev.com/w/index.php/PPU_pattern_tables

			var ppuformat = new byte[16];  // starts out all zero per C# spec

			for (int i = 0; i < 64; i++)
			{
				var val = tile[i];
				var bit0 = val & 0x01;
				var bit1 = (val >> 1) & 0x01;
				var row = (i >> 3) & 0x07;
				var col = 7 - (i & 0x07);
				ppuformat[row] |= (byte)(bit0 << col);    // write bit0 to the first plane
				ppuformat[row + 8] |= (byte)(bit1 << col);  // write bit1 to the second plane
			}
			return ppuformat;
		}

		public static byte[] DecodePPU(byte[] ppuformat)
		{
			// Read the 16-byte, dual-plane encoding used by the NES PPU
			// and return an array of 64 bytes (i.e. an 8x8 tile) with an ordinary linear
			// encoding (left to right, top to bottom, one byte
			// per pixel, valid values 0-3).
			//
			// see https://wiki.nesdev.com/w/index.php/PPU_pattern_tables

			var tile = new byte[64];

			for (int i = 0; i < 64; i++)
			{
				var row = (i >> 3) & 0x07;
				var col = 7 - (i & 0x07);
				var bit0 = (ppuformat[row] & (1 << col)) >> col;    // read bit0 from the first plane
				var bit1 = (ppuformat[row + 8] & (1 << col)) >> col;  // read bit1 from the second plane
				tile[i] = (byte)((bit1 << 1) | bit0);
			}
			return tile;
		}


		public byte[] makeTile(Image<Rgba32> image, int top, int left, Dictionary<Rgba32, byte> toIndex)
		{
			var newtile = new byte[64];
			int px = 0;
			for (int y = top; y < (top + 8); y++)
			{
				for (int x = left; x < (left + 8); x++)
				{
					newtile[px] = toIndex[image[x, y]];
					px++;
				}
			}
			return newtile;
		}


		// this makeTile() has a different signature from the one above:
		// last parameter is Dictionary<byte, byte> rather than Dictionary<Rbga32, byte>
		public byte[] makeTile(Image<Rgba32> image, int top, int left, Dictionary<byte, byte> index)
		{
			var newtile = new byte[64];
			int px = 0;
			for (int y = top; y < (top + 8); y++)
			{
				for (int x = left; x < (left + 8); x++)
				{
					newtile[px] = index[selectColor(image[x, y], NESpalette)];
					px++;
				}
			}
			return newtile;
		}

		// Finds the closest color to a provided list of colors.
		// Duplicates some functionality of selectColor() above -- find a way to merge?
		private Rgba32 FindClosestColor(Rgba32 color, List<Rgba32> destColors)
		{
			List<double> distances = new List<double>();
			for (int i = 0; i < destColors.Count; i++)
			{
				distances.Add(
					Math.Pow((color.R - destColors[i].R), 2) +
					Math.Pow((color.G - destColors[i].G), 2) +
					Math.Pow((color.B - destColors[i].B), 2)
				);
			}
			int min = distances.IndexOf(distances.Min());
			return destColors[min];
		}
		private byte[] makeTileQuantize(Image<Rgba32> image, int top, int left, Dictionary<Rgba32, byte> toIndex)
		{
			HashSet<Rgba32> srcColors = new HashSet<Rgba32>();
			for (int y = top; y < (top + 8); y++)
			{
				for (int x = left; x < (left + 8); x++)
				{
					srcColors.Add(image[x, y]);
				}
			}
			var imageColorToIndexColor = new Dictionary<Rgba32, Rgba32>();
			foreach (Rgba32 color in srcColors)
			{
				imageColorToIndexColor[color] = FindClosestColor(color, toIndex.Keys.ToList());
			}

			var newtile = new byte[64];
			int px = 0;

			for (int y = top; y < (top + 8); y++)
			{
				for (int x = left; x < (left + 8); x++)
				{
					newtile[px] = toIndex[imageColorToIndexColor[image[x, y]]];
					px++;
				}
			}
			return newtile;
		}

		// General function for converting a rectangular section of an image to contiguous 8x8 tiles
		// encoded for PPU. Order of tiles goes left-to-right and then top-to-bottom, like reading English.
		// The top and left parameters point to a pixel in the source image, default being the topmost-leftmost pixel.
		// The two width parameters indicate the dimensions of the area to encode, in 8x8 tiles.
		// If given width or height < 1 (default == 0), width and height will be set to go as far to the right and bottom
		// of the source image as possible, without going past either edge.
		// Thus the default behavior is to import the entire image.
		// No bounds checking is performed, so errors will be thrown if it attempts to access a pixel that isn't there.
		public byte[] MakePatternData(Image<Rgba32> image, Dictionary<Rgba32,byte> toIndex, int top = 0, int left = 0, int widthInTiles = 0, int heightInTiles = 0)
		{
			var data = new List<byte>();
			int width = widthInTiles;
			int height = heightInTiles;
			byte[] tile;
			if (widthInTiles < 1)
			{
				width = (image.Width - left) / 8;
				width = width < 1 ? 1 : width;
			}
			if (heightInTiles < 1)
			{
				height = (image.Height - top) / 8;
				height = height < 1 ? 1 : height;
			}
			
			for (int h = 0; h < height; h++  )
			{
				for(int w = 0; w < width; w++)
				{
					try
					{
						tile = makeTile(image, top + 8*h, left + 8*w, toIndex);
					}
					catch (KeyNotFoundException)
					{
						tile = makeTileQuantize(image,top + 8*h, left + 8*w, toIndex);
					}
					data.AddRange(EncodeForPPU(tile));
				}
			}	

			return data.ToArray();
		}

		
		byte chrIndex(byte[] tile, List<byte[]> chrEntries, int maxEntries)
		{
			byte b;
			for (b = 0; b < chrEntries.Count; b++)
			{
				if (Enumerable.SequenceEqual(tile, chrEntries[b]))
				{
					return b;
				}
			}
			if (b < maxEntries)
			{
				chrEntries.Add(tile);
				return (byte)(chrEntries.Count - 1);
			}
			return 0xff;
		}

		bool makeNTPalette(List<Rgba32> colors, Rgba32[] NESpalette,
				out List<byte> pal, Dictionary<Rgba32, byte> toNEScolor)
		{
			pal = new List<byte>();

			// always have black at 0
			pal.Add(0x0f);

			for (int i = 0; i < colors.Count; i++)
			{
				byte selected = selectColor(colors[i], NESpalette);
				toNEScolor[colors[i]] = selected;
				int idx = pal.IndexOf(selected);
				if (idx == -1)
				{
					idx = pal.Count;
					pal.Add(selected);
				}
			}

			if (pal.Count > 4)
			{
				return false;
			}

			return true;
		}

		public bool isSubsetPalette(List<byte> outer, List<byte> inner)
		{
			foreach (var i in inner)
			{
				var j = outer.IndexOf(i);
				if (j == -1)
				{
					return false;
				}
			}
			return true;
		}

		public List<byte> MergePalette(List<byte> outer, List<byte> inner)
		{
			var unique = new List<byte>();
			foreach (var i in inner)
			{
				if (!unique.Contains(i))
				{
					unique.Add(i);
				}
			}
			foreach (var i in outer)
			{
				if (!unique.Contains(i))
				{
					unique.Add(i);
				}
			}
			if (unique.Count <= 4)
			{
				return unique;
			}
			return null;
		}

		public int findPalette(List<List<byte>> haystack, List<byte> needle)
		{
			for (int i = 0; i < haystack.Count; i++)
			{
				if (isSubsetPalette(haystack[i], needle))
				{
					return i;
				}
			}
			return -1;
		}

		public void colorToPaletteIndex(List<byte> palette, Dictionary<Rgba32, byte> toNEScolor, out Dictionary<Rgba32, byte> toNESindex)
		{
			toNESindex = new Dictionary<Rgba32, byte>();
			for (int i = 0; i < palette.Count; i++)
			{
				foreach (var kv in toNEScolor)
				{
					if (kv.Value == palette[i])
					{
						toNESindex[kv.Key] = (byte)i;
					}
				}
			}
		}

		public bool mergePalettes(List<List<byte>> inPalettes, List<List<byte>> merged, int maxPals)
		{
			for (int i = 0; i < inPalettes.Count; i++)
			{
				if (inPalettes[i].Count == 4)
				{
					if (findPalette(merged, inPalettes[i]) == -1)
					{
						if (merged.Count == maxPals)
						{
							return false;
						}
						merged.Add(inPalettes[i]);
					}
				}
			}
			if (merged.Count == maxPals)
			{
				return true;
			}
			var minimize = new List<List<byte>>();
			for (int i = 0; i < inPalettes.Count; i++)
			{
				if (inPalettes[i].Count <= 3)
				{
					if (findPalette(merged, inPalettes[i]) == -1)
					{
						minimize.Add(inPalettes[i]);
					}
				}
			}
			for (int i = 0; i < minimize.Count; i++)
			{
				for (int j = 0; j < minimize.Count; j++)
				{
					if (i == j)
					{
						continue;
					}
					if (isSubsetPalette(minimize[i], minimize[j]))
					{
						minimize.RemoveAt(j);
						j--;
					}
					else
					{
						var newpal = MergePalette(minimize[i], minimize[j]);
						if (newpal != null)
						{
							minimize[i] = newpal;
							minimize.RemoveAt(j);
							j--;
						}
					}
				}
			}
			for (int i = 0; i < minimize.Count; i++)
			{
				while (minimize[i].Count < 4)
				{
					minimize[i].Add(0xF);
				}
			}
			merged.AddRange(minimize);
			if (merged.Count > maxPals)
			{
				return false;
			}
			return false;
		}

	}
}
