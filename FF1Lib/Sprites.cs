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
	    // Copied from FFHackster.
	    const int CHARBATTLEPIC_OFFSET =			0x25000;

	    const int MAPMANGRAPHIC_OFFSET =			0x9000;

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

	    Dictionary<byte, byte> colorReduction = new Dictionary<byte, byte>() {
		{ 0x0D, 0x0F },
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

	    //
	    byte selectColor(Rgba32 px, Rgba32[] NESpalette) {
		int min_dif = 1000000000;;
		byte idx = 0;

		/*
		var converted = new HSLColor(px);
		if (converted.L > .9) {
		    idx = 0x30; // white
		} else if (converted.L < .1) {
		    idx = 0x0F; // black
		} else {
		    for (int i = 0; i < table.Length; i++) {
			var dif = converted.WeightedDistance(table[i]);
			if (dif < min_dif) {
			    min_dif = dif;
			    idx = (byte)i;
			}
		    }
		    }*/

		for (int i = 0; i < NESpalette.Length; i++) {
		    int dif = (NESpalette[i].R - px.R)*(NESpalette[i].R - px.R);
		    dif += (NESpalette[i].G - px.G)*(NESpalette[i].G - px.G);
		    dif += (NESpalette[i].B - px.B)*(NESpalette[i].B - px.B);
		    if (dif < min_dif) {
			min_dif = dif;
			idx = (byte)i;
		    }
		}

		if (colorReduction.ContainsKey(idx)) {
		    idx = colorReduction[idx];
		}
		return idx;
	    }

	    bool makeMapmanPalette(List<Rgba32> colors, Rgba32[] NESpalette,
			     out List<byte> pal,
			     out Dictionary<Rgba32,byte> toIndex) {
		pal = new List<byte>();
		toIndex = new Dictionary<Rgba32,byte>();

		for (int i = 0; i < colors.Count; i++) {
		    if (colors[i].R >= 250 && colors[i].G <= 5 && colors[i].B >= 250) {
			// treat magenta as transparent
			toIndex[colors[i]] = 0;
			continue;
		    }

		    byte selected = selectColor(colors[i], NESpalette);
		    int idx = pal.IndexOf(selected);
		    if (idx == -1) {
			// add 1 everything is going to get shifted
			// when the tranparent entry is added
			idx = pal.Count + 1;
			pal.Add(selected);
		    }
		    toIndex[colors[i]] = (byte)idx;
		}

		// insert the transparent entry.
		pal.Insert(0, 0x0F);

		if (pal.Count > 4) {
		    return false;
		}
		while (pal.Count < 4) {
		    pal.Add(0x0F);
		}

		return true;
	    }


	    bool makeBattlePalette(List<Rgba32> colors, Rgba32[] NESpalette,
			     out List<byte> pal,
			     out Dictionary<Rgba32,byte> toIndex) {
		var paltmp = new List<byte>();
		var toIndexTmp = new Dictionary<Rgba32,byte>();

		// transparent is black (because battle sprites
		// have a black background), it can be used as a
		// "color" in palette entry 0
		paltmp.Insert(0, 0x0F);

		for (int i = 0; i < colors.Count; i++) {
		    if (colors[i].R <= 5 && colors[i].G <= 5 && colors[i].B <= 5) {
			// treat black as transparent
			toIndexTmp[colors[i]] = 0;
			continue;
		    }

		    byte selected = selectColor(colors[i], NESpalette);
		    int idx = paltmp.IndexOf(selected);
		    if (idx == -1) {
			idx = paltmp.Count;
			paltmp.Add(selected);
		    }
		    toIndexTmp[colors[i]] = (byte)idx;
		}

		if (paltmp.Count > 4) {
		    pal = paltmp;
		    toIndex = toIndexTmp;
		    return false;
		}
		while (paltmp.Count < 4) {
		    paltmp.Add(0x0F);
		}

		toIndex = new Dictionary<Rgba32,byte>();
		pal = new List<byte>(paltmp);
		// Need to keep black at position 0
		pal.RemoveAt(0);

		// Need to sort the palette & update the mapping so
		// that battle sprites that need to share palettes all
		// have the colors in the same order.
		pal.Sort();
		pal.Insert(0, 0x0F);
		foreach (var kv in toIndexTmp) {
		    toIndex[kv.Key] = (byte)pal.IndexOf(paltmp[kv.Value]);
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

	    const int lut_MapmanPalettes = 0x8150;

	    void ImportMapman(Image<Rgba32> image, int cur_class, Rgba32[] NESpalette) {
		int top = 24 + (40*(cur_class >= 6 ? cur_class-6 : cur_class));
		int left = ((cur_class >= 6) ? 104 : 0);

		// the mapman head tiles have a different palette
		// than the body tiles.
		var headColors = new List<Rgba32>();
		var firstUnique = new Dictionary<Rgba32, int>();
		for (int y = top; y < (top+8); y++) {
		    for (int x = left; x < (left+64); x++) {
			if (!headColors.Contains(image[x,y])) {
			    firstUnique[image[x,y]] = (x<<16 | y);
			    headColors.Add(image[x,y]);
			}
		    }
		}
		List<byte> headPal;
		Dictionary<Rgba32,byte> headIndex;
		if (!makeMapmanPalette(headColors, NESpalette, out headPal, out headIndex)) {
		    Console.WriteLine($"Failed importing top half of mapman for {ClassNames[cur_class]}, too many unique colors (limit 3 unique colors + magenta for transparent):");
		    for (int i = 0; i < headPal.Count; i++) {
			Console.WriteLine($"NES palette {i}: ${headPal[i],2:X}");
		    }
		    foreach (var i in headIndex) {
			int c = firstUnique[i.Key];
			Console.WriteLine($"RGB to index {i.Key}: {i.Value}  first appears at {c>>16}, {c & 0xFFFF}");
		    }
		    return;
		}

		var bodyColors = new List<Rgba32>();
		firstUnique = new Dictionary<Rgba32, int>();
		for (int y = top+8; y < (top+16); y++) {
		    for (int x = left; x < (left+64); x++) {
			if (!bodyColors.Contains(image[x,y])) {
			    firstUnique[image[x,y]] = (x<<16 | y);
			    bodyColors.Add(image[x,y]);
			}
		    }
		}
		List<byte> bodyPal;
		Dictionary<Rgba32,byte> bodyIndex;
		if (!makeMapmanPalette(bodyColors, NESpalette, out bodyPal, out bodyIndex)) {
		    Console.WriteLine($"Failed importing bottom half of mapman for {ClassNames[cur_class]}, too many unique colors (limit 3 unique colors + magenta for transparent):");
		    for (int i = 0; i < bodyPal.Count; i++) {
			Console.WriteLine($"NES palette {i}: ${bodyPal[i],2:X}");
		    }
		    foreach (var i in bodyIndex) {
			int c = firstUnique[i.Key];
			Console.WriteLine($"RGB to index {i.Key}: {i.Value}  first appears at {c>>16}, {c & 0xFFFF}");
		    }
		    return;
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

		int offsetIntoLut = cur_class << 3;
		// Write the palettes into a new LUT in bank $0F
		// Will be read using the code below.
		//Console.WriteLine($"writing to {lut_MapmanPalettes + offsetIntoLut} {lut_MapmanPalettes + offsetIntoLut + 4}");
		PutInBank(0x0F, lut_MapmanPalettes + offsetIntoLut,       headPal.ToArray());
		PutInBank(0x0F, lut_MapmanPalettes + offsetIntoLut + 4,   bodyPal.ToArray());
	    }

	    int ImportBattleSprites(Image<Rgba32> image, int cur_class, Rgba32[] NESpalette, List<List<byte>> battlePals) {
		int top = (40*(cur_class >= 6 ? cur_class-6 : cur_class));
		int left = ((cur_class >= 6) ? 104 : 0);

		var firstUnique = new Dictionary<Rgba32, int>();
		var colors = new List<Rgba32>();
		for (int y = top; y < (top+24); y++) {
		    for (int x = left; x < (left+104); x++) {
			if (x >= (left+80) && y < (top+8)) {
			    // This is the space above the dead character,
			    // ignore it
			    continue;
			}
			if (!colors.Contains(image[x,y])) {
			    firstUnique[image[x,y]] = (x<<16 | y);
			    colors.Add(image[x,y]);
			}
		    }
		}
		List<byte> pal;
		Dictionary<Rgba32,byte> index;
		if (!makeBattlePalette(colors, NESpalette, out pal, out index)) {
		    Console.WriteLine($"Failed importing battle sprites for {ClassNames[cur_class]}, too many unique colors (limit 3 unique colors + black):");
		    for (int i = 0; i < pal.Count; i++) {
			Console.WriteLine($"NES palette {i}: ${pal[i],2:X}");
		    }
		    foreach (var i in index) {
			int c = firstUnique[i.Key];
			Console.WriteLine($"RGB to index {i.Key}: {i.Value}  first appears at {c>>16}, {c & 0xFFFF}");
		    }
		    return -1;
		}

		int usepal = -1;
		int b;
		for (b = 0; b < battlePals.Count; b++) {
		    if (battlePals[b].Count == 0) {
			for (int j = 0; j < 4; j++) { battlePals[b].Add(pal[j]); }
			usepal = b;
			break;
		    } else if (Enumerable.SequenceEqual(pal, battlePals[b])) {
			usepal = b;
			break;
		    }
		}
		if (b == battlePals.Count) {
		    Console.WriteLine($"Failed importing battle sprites for {ClassNames[cur_class]}, has a different palette from other classes");

		    for (int i = 0; i < pal.Count; i++) {
			Console.WriteLine($"palette for class, idx {i}: ${pal[i],2:X}");
		    }
		    for (b = 0; b < battlePals.Count; b++) {
			for (int i = 0; i < battlePals[b].Count; i++) {
			    Console.WriteLine($"palette {b}, idx {i}: ${battlePals[b][i],2:X}");
			}
		    }
		    foreach (var i in index) {
			Console.WriteLine($"RGB to index {i.Key}: {i.Value}");
		    }
		    usepal = 0; // whatever
		}

		// const byte ConstPicFormation[39] = {	//3 x 13 pic formation
		//     0, 1, 0, 1, 8, 9,14,15,20,21,255,255,255,
		//     2, 3, 2, 3,10,11,16,17,22,23,26,27,28,
		//     4, 5, 6, 7,12,13,18,19,24,25,29,30,31};

		var ConstPicFormationNoDup = new byte[39] {	//3 x 13 pic formation
		    0, 1, 255, 255,  8, 9,14,15,20,21,255,255,255,
		    2, 3, 255, 255, 10,11,16,17,22,23,26,27,28,
		    4, 5,   6,   7, 12,13,18,19,24,25,29,30,31};

		for (int y = 0; y < 3; y++) {
		    for (int x = 0; x < 13; x++) {
			var tileidx = ConstPicFormationNoDup[(y*13) + x];
			if (tileidx == 255) {
			    continue;
			}
			var tile = makeTile(image, top + (y*8), left + (x*8), index);
			Put(CHARBATTLEPIC_OFFSET + (cur_class << 9) + (tileidx*16), EncodeForPPU(tile));
		    }
		}

		return usepal;
	    }

	    public void SetCustomPlayerSprites(Stream readStream, bool threePalettes) {
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

		    new Rgba32(0xf8, 0xf8, 0xf8),
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

		var battlePals = new List<List<byte>>();
		battlePals.Add(new List<byte>());
		battlePals.Add(new List<byte>());
		if (threePalettes) {
		    battlePals.Add(new List<byte>());
		}

		for (int cur_class = 0; cur_class < 12; cur_class++) {
		    ImportMapman(image, cur_class, NESpalette);
		    var palAssign = ImportBattleSprites(image, cur_class, NESpalette, battlePals);
		    if (palAssign != -1) {
			// Set the palette to use for each class, this
			// one is used for character selection and
			// subscreen
			PutInBank(0x1F, 0xECA4 + cur_class, new byte[] {(byte)palAssign});

			// it loads this one in battle, it is redundant with the one
			// in 1F but we have to set it anyway
			PutInBank(0x0C, 0xA03C + cur_class, new byte[] {(byte)palAssign});
		    }
		}

		for (int i = 0; i < battlePals.Count; i++) {
		    PutInBank(0x1F, 0xEBA5+(i*4), battlePals[i].ToArray());
		}

		// add palette for "none" mapman
		PutInBank(0x0F, lut_MapmanPalettes + (13 << 3), new byte[] {0x0F, 0x0F, 0x12, 0x36,
									    0x0F, 0x0F, 0x21, 0x36});

		// code in asm/0F_8150_MapmanPalette.asm

		// Replace the original code which loads the mapman
		// "palette" (it actually only changes 2 colors and
		// leaves the blank and "skin tone" alone)
		// With a jump to a new routine in bank 0F which loads
		// two complete 4 color palettes.
		PutInBank(0x1F, 0xD8B6, Blob.FromHex("A90F2003FE4CC081EAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));
		PutInBank(0x0F, 0x81C0, Blob.FromHex("AD0061C9FFD002A90D0A0A0A6908AAA008BD508199D003CA8810F660"));
	    }
	}
}
