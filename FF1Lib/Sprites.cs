using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RomUtilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
	    // Copied from FFHackster.  The Hackster offsets included
	    // the ROM header, but FFR doesn't, so subtract that out.
	    //
	    const int CHARBATTLEPIC_OFFSET =			0x25010 - 0x10;
	    const int CHARBATTLEPALETTE_OFFSET =		0x3EBB5;
	    const int CHARBATTLEPALETTE_ASSIGNMENT1 =		0x3204C;
	    const int CHARBATTLEPALETTE_ASSIGNMENT2 =		0x3ECB4;
	    const int MAPMANGRAPHIC_OFFSET =			0x9010 - 0x10;

	    const int MAPMAN_DOWN = 0;
	    const int MAPMAN_UP = 1;
	    const int MAPMAN_SIDE1 = 1;
	    const int MAPMAN_SIDE2 = 2;

	    public byte[] EncodeForPPU(byte[] tile) {
		// Take an array of 64 bytes with a ordinary linear
		// encoding (left to right, top to bottom, one byte
		// per pixel, valid values 0-3) and return the
		// 16-byte, dual-plane encoding used by the NES PPU.
		//
		// see https://wiki.nesdev.com/w/index.php/PPU_pattern_tables

		var ppuformat = new byte[16];  // starts out all zero per C# spec

		for (int i = 0; i < 64; i++) {
		    var val = tile[i];
		    var bit0 = val & 0x01;
		    var bit1 = (val >> 1) & 0x01;
		    var row = (i >> 3) & 0x07;
		    var col = 7 - (i & 0x07);
		    ppuformat[row] |= (byte)(bit0 << col);    // write bit0 to the first plane
		    ppuformat[row+8] |= (byte)(bit1 << col);  // write bit1 to the second plane
		}
		return ppuformat;
	    }

	    byte selectColor(Rgba32 px, Rgba32[] table ) {
		int min_dif = 1000;
		byte idx = 0;
		for (int i = 0; i < table.Length; i++) {
		    int dif = Math.Abs(px.R - table[i].R);
		    dif += Math.Abs(px.G - table[i].G);
		    dif += Math.Abs(px.B - table[i].B);
		    if (dif < min_dif) {
			min_dif = dif;
			idx = (byte)i;
		    }
		}
		if (idx == 0x0D) {
		    idx = 0x0F;
		}
		return idx;
	    }

	    bool makePalette(List<Rgba32> colors, Rgba32 transparent, Rgba32[] table,
			     out List<byte> pal,
			     out Dictionary<Rgba32,byte> toIndex) {
		pal = new List<byte>();
		toIndex = new Dictionary<Rgba32,byte>();
		toIndex[transparent] = 0;
		for (int i = 0; i < colors.Count; i++) {
		    if (colors[i] == transparent) {
			continue;
		    }
		    byte selected = selectColor(colors[i], table);
		    int idx = pal.IndexOf(selected);
		    if (idx == -1) {
			pal.Add(selected);
			idx = pal.Count;
		    }
		    toIndex[colors[i]] = (byte)idx;
		}
		pal.Insert(0, 0x0F);
		if (pal.Count > 4) {
		    return false;
		}
		while (pal.Count < 4) {
		    pal.Add(0x0F);
		}
		return true;
	    }

	    byte[] makeTile(Image<Rgba32> image, int top, int left, Dictionary<Rgba32,byte> toIndex) {
		var newtile = new byte[64];
		int px = 0;
		for (int y = top; y < (top+8); y++) {
		    for (int x = left; x < (left+8); x++) {
			newtile[px] = toIndex[image[x,y]];
			px++;
		    }
		}
		return newtile;
	    }

	    public void SetCustomPlayerSprites(Stream readStream) {
		IImageFormat format;
		Image<Rgba32> image = Image.Load<Rgba32>(readStream, out format);

		var NESpalette = new Rgba32[]{
		    new Rgba32(0x7f, 0x7f, 0x7f),
		    new Rgba32(0x00, 0x00, 0xff),
		    new Rgba32(0x00, 0x00, 0xbf),
		    new Rgba32(0x47, 0x2b, 0xbf),
		    new Rgba32(0x97, 0x00, 0x87),
		    new Rgba32(0xab, 0x00, 0x23),
		    new Rgba32(0xab, 0x13, 0x00),
		    new Rgba32(0x8b, 0x17, 0x00),
		    new Rgba32(0x53, 0x30, 0x00),
		    new Rgba32(0x00, 0x78, 0x00),
		    new Rgba32(0x00, 0x6b, 0x00),
		    new Rgba32(0x00, 0x5b, 0x00),
		    new Rgba32(0x00, 0x43, 0x58),
		    new Rgba32(0x00, 0x00, 0x00),
		    new Rgba32(0x00, 0x00, 0x00),
		    new Rgba32(0x00, 0x00, 0x00),

		    new Rgba32(0xbf, 0xbf, 0xbf),
		    new Rgba32(0x00, 0x78, 0xf8),
		    new Rgba32(0x00, 0x58, 0xf8),
		    new Rgba32(0x6b, 0x47, 0xff),
		    new Rgba32(0xdb, 0x00, 0xcd),
		    new Rgba32(0xe7, 0x00, 0x5b),
		    new Rgba32(0xf8, 0x38, 0x00),
		    new Rgba32(0xe7, 0x5f, 0x13),
		    new Rgba32(0xaf, 0x7f, 0x00),
		    new Rgba32(0x00, 0xb8, 0x00),
		    new Rgba32(0x00, 0xab, 0x00),
		    new Rgba32(0x00, 0xab, 0x47),
		    new Rgba32(0x00, 0x8b, 0x8b),
		    new Rgba32(0x00, 0x00, 0x00),
		    new Rgba32(0x00, 0x00, 0x00),
		    new Rgba32(0x00, 0x00, 0x00),

		    new Rgba32(0xff, 0xff, 0xff),
		    new Rgba32(0x3f, 0xbf, 0xff),
		    new Rgba32(0x6b, 0x88, 0xff),
		    new Rgba32(0x98, 0x78, 0xf8),
		    new Rgba32(0xf8, 0x78, 0xf8),
		    new Rgba32(0xf8, 0x58, 0x98),
		    new Rgba32(0xf8, 0x78, 0x58),
		    new Rgba32(0xff, 0xa3, 0x47),
		    new Rgba32(0xf8, 0xb8, 0x00),
		    new Rgba32(0xb8, 0xf8, 0x18),
		    new Rgba32(0x5b, 0xdb, 0x57),
		    new Rgba32(0x58, 0xf8, 0x98),
		    new Rgba32(0x00, 0xeb, 0xdb),
		    new Rgba32(0x78, 0x78, 0x78),
		    new Rgba32(0x00, 0x00, 0x00),
		    new Rgba32(0x00, 0x00, 0x00),

		    new Rgba32(0xff, 0xff, 0xff),
		    new Rgba32(0xa7, 0xe7, 0xff),
		    new Rgba32(0xb8, 0xb8, 0xf8),
		    new Rgba32(0xd8, 0xb8, 0xf8),
		    new Rgba32(0xf8, 0xb8, 0xf8),
		    new Rgba32(0xfb, 0xa7, 0xc3),
		    new Rgba32(0xf0, 0xd0, 0xb0),
		    new Rgba32(0xff, 0xe3, 0xab),
		    new Rgba32(0xfb, 0xdb, 0x7b),
		    new Rgba32(0xd8, 0xf8, 0x78),
		    new Rgba32(0xb8, 0xf8, 0xb8),
		    new Rgba32(0xb8, 0xf8, 0xd8),
		    new Rgba32(0x00, 0xff, 0xff),
		    new Rgba32(0xf8, 0xd8, 0xf8),
		    new Rgba32(0x00, 0x00, 0x00),
		    new Rgba32(0x00, 0x00, 0x00)
		};

		// Unpromoted classes
		for (int cur_class = 0; cur_class < 12; cur_class++) {
		    int top = 24 + (40*(cur_class >= 6 ? cur_class-6 : cur_class));
		    int left = ((cur_class >= 6) ? 104 : 0);

		    // the mapman head tiles have a different palette
		    // than the body tiles.
		    var headColors = new List<Rgba32>();
		    for (int y = top; y < (top+8); y++) {
			for (int x = left; x < (left+64); x++) {
			    if (!headColors.Contains(image[x,y])) {
				headColors.Add(image[x,y]);
			    }
			}
		    }
		    List<byte> headPal;
		    Dictionary<Rgba32,byte> headIndex;
		    if (!makePalette(headColors, new Rgba32(0xFF, 0x00, 0xFF), NESpalette, out headPal, out headIndex)) {
			Console.WriteLine($"Failed importing top half of mapman for {ClassNames[cur_class]}, too many unique colors (limit 3 unique colors + magenta for transparent):");
			for (int i = 0; i < headPal.Count; i++) {
			    Console.WriteLine($"NES palette {i}: {headPal[i]}");
			}
			foreach (var i in headIndex) {
			    Console.WriteLine($"RGB to index {i.Key}: {i.Value}");
			}
			continue;
		    }

		    var bodyColors = new List<Rgba32>();
		    for (int y = top+8; y < (top+16); y++) {
			for (int x = left; x < (left+64); x++) {
			    if (!bodyColors.Contains(image[x,y])) {
				bodyColors.Add(image[x,y]);
			    }
			}
		    }
		    List<byte> bodyPal;
		    Dictionary<Rgba32,byte> bodyIndex;
		    if (!makePalette(bodyColors, new Rgba32(0xFF, 0x00, 0xFF), NESpalette, out bodyPal, out bodyIndex)) {
			Console.WriteLine($"Failed importing bottom half of mapman for {ClassNames[cur_class]}, too many unique colors (limit 3 unique colors + magenta for transparent):");
			for (int i = 0; i < bodyPal.Count; i++) {
			    Console.WriteLine($"NES palette {i}: {bodyPal[i]}");
			}
			foreach (var i in bodyIndex) {
			    Console.WriteLine($"RGB to index {i.Key}: {i.Value}");
			}
			continue;
		    }

		    for (int mapmanPos = 0; mapmanPos < 4; mapmanPos++) {
			top = 24 + (40*(cur_class >= 6 ? cur_class-6 : cur_class));
			left = ((cur_class >= 6) ? 104 : 0) + (mapmanPos*16);

			var headTileLeft = makeTile(image, top, left, headIndex);
			var headTileRight = makeTile(image, top, left+8, headIndex);

			var bodyTileLeft = makeTile(image, top+8, left, bodyIndex);
			var bodyTileRight = makeTile(image, top+8, left+8, bodyIndex);

			Put(MAPMANGRAPHIC_OFFSET + (cur_class << 8) + (mapmanPos * 16*4) + (16*0),  EncodeForPPU(headTileLeft));
			Put(MAPMANGRAPHIC_OFFSET + (cur_class << 8) + (mapmanPos * 16*4) + (16*1),  EncodeForPPU(headTileRight));
			Put(MAPMANGRAPHIC_OFFSET + (cur_class << 8) + (mapmanPos * 16*4) + (16*2),  EncodeForPPU(bodyTileLeft));
			Put(MAPMANGRAPHIC_OFFSET + (cur_class << 8) + (mapmanPos * 16*4) + (16*3),  EncodeForPPU(bodyTileRight));
		    }

		    int lut_MapmanPalettes = 0x8150;
		    int offsetIntoLut = cur_class << 3;
		    // Write the palettes into a new LUT in bank $0F
		    // Will be read using the code below.
		    Console.WriteLine($"writing to {lut_MapmanPalettes + offsetIntoLut} {lut_MapmanPalettes + offsetIntoLut + 4}");
		    PutInBank(0x0F, lut_MapmanPalettes + offsetIntoLut,       headPal.ToArray());
		    PutInBank(0x0F, lut_MapmanPalettes + offsetIntoLut + 4,   bodyPal.ToArray());
		}

		// code in asm/0F_8150_MapmanPalette.asm

		// Replace the original code which loads the mapman
		// "palette" (it actually only changes 2 colors and
		// leaves the blank and "skin tone" alone)
		// With a jump to a new routine in bank 0F which loads
		// two complete 4 color palettes.
		PutInBank(0x1F, 0xD8B6, Blob.FromHex("A90F2003FE4CB081EAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));
		PutInBank(0x0F, 0x81B0, Blob.FromHex("AD00610A0A0A6908AAA008BD508199D003CA8810F660"));
	    }
	}
}
