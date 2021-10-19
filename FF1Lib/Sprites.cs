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

	    const int BATTLEPATTERNTABLE_OFFSET =			0x1C000;

	    // nametables
	    const int FIENDDRAW_TABLE =						0x2D2E0;
	    const int CHAOSDRAW_TABLE =						0x2D420;
	    const int BATTLEPALETTE_OFFSET =				0x30F20;

	    const int MAPMAN_DOWN = 0;
	    const int MAPMAN_UP = 1;
	    const int MAPMAN_SIDE1 = 1;
	    const int MAPMAN_SIDE2 = 2;

	    const int OVERWORLDPALETTE_OFFSET =				0x380;
	    const int OVERWORLDPALETTE_ASSIGNMENT =			0x300;
	    const int OVERWORLDPATTERNTABLE_OFFSET =		0x8000;
	    const int OVERWORLDPATTERNTABLE_ASSIGNMENT =	0x100;

	    const int TILESETPATTERNTABLE_OFFSET =			0xC000;
	    const int TILESETPATTERNTABLE_ASSIGNMENT =		0x1000;
	    const int TILESETPALETTE_ASSIGNMENT =			0x400;
	    const int TILESET_TILEDATA =					0x800;
	    const int MAPPALETTE_OFFSET =					0x2000;

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
		    for (int i = 1; i < headPal.Count; i++) {
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
		    for (int i = 1; i < bodyPal.Count; i++) {
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

	    Rgba32[] NESpalette = new Rgba32[]{
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

	    public void SetCustomPlayerSprites(Stream readStream, bool threePalettes) {
		IImageFormat format;
		Image<Rgba32> image = Image.Load<Rgba32>(readStream, out format);


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

	    byte chrIndex(byte[] tile, List<byte[]> chrEntries, int maxEntries) {
		byte b;
		for (b = 0; b < chrEntries.Count; b++) {
		    if (Enumerable.SequenceEqual(tile, chrEntries[b])) {
			return b;
		    }
		}
		if (b < maxEntries) {
		    chrEntries.Add(tile);
		    return (byte)(chrEntries.Count-1);
		}
		return 0xff;
	    }

	    bool makeNTPalette(List<Rgba32> colors, Rgba32[] NESpalette,
				out List<byte> pal, Dictionary<Rgba32, byte> toNEScolor) {
		pal = new List<byte>();

		// always have black at 0
		pal.Add(0x0f);

		for (int i = 0; i < colors.Count; i++) {
		    byte selected = selectColor(colors[i], NESpalette);
		    toNEScolor[colors[i]] = selected;
		    int idx = pal.IndexOf(selected);
		    if (idx == -1) {
			idx = pal.Count;
			pal.Add(selected);
		    }
		}

		if (pal.Count > 4) {
		    return false;
		}

		return true;
	    }

	    public bool isSubsetPalette(List<byte> outer, List<byte> inner) {
		foreach (var i in inner) {
		    var j = outer.IndexOf(i);
		    if (j == -1) {
			return false;
		    }
		}
		return true;
	    }

	    public List<byte> MergePalette(List<byte> outer, List<byte> inner) {
		var unique = new List<byte>();
		foreach (var i in inner) {
		    if (!unique.Contains(i)) {
			unique.Add(i);
		    }
		}
		foreach (var i in outer) {
		    if (!unique.Contains(i)) {
			unique.Add(i);
		    }
		}
		if (unique.Count <= 4) {
		    return unique;
		}
		return null;
	    }

	    public int findPalette(List<List<byte>> haystack, List<byte> needle) {
		for (int i = 0; i < haystack.Count; i++) {
		    if (isSubsetPalette(haystack[i], needle)) {
			return i;
		    }
		}
		return -1;
	    }

	    public void colorToPaletteIndex(List<byte> palette, Dictionary<Rgba32, byte> toNEScolor, out Dictionary<Rgba32, byte> toNESindex) {
		toNESindex = new Dictionary<Rgba32, byte>();
		for (int i = 0; i < palette.Count; i++) {
		    foreach (var kv in toNEScolor) {
			if (kv.Value == palette[i]) {
			    toNESindex[kv.Key] = (byte)i;
			}
		    }
		}
	    }

	    public bool mergePalettes(List<List<byte>> inPalettes, List<List<byte>> merged, int maxPals) {
		for (int i = 0; i < inPalettes.Count; i++) {
		    if (inPalettes[i].Count == 4) {
			if (findPalette(merged, inPalettes[i]) == -1) {
			    if (merged.Count == maxPals) {
				return false;
			    }
			    merged.Add(inPalettes[i]);
			}
		    }
		}
		if (merged.Count == maxPals) {
		    return true;
		}
		var minimize = new List<List<byte>>();
		for (int i = 0; i < inPalettes.Count; i++) {
		    if (inPalettes[i].Count <= 3) {
			if (findPalette(merged, inPalettes[i]) == -1) {
			    minimize.Add(inPalettes[i]);
			}
		    }
		}
		for (int i = 0; i < minimize.Count; i++) {
		    for (int j = 0; j < minimize.Count; j++) {
 			if (i == j) {
			    continue;
			}
			if (isSubsetPalette(minimize[i], minimize[j])) {
			    minimize.RemoveAt(j);
			    j--;
			} else {
			    var newpal = MergePalette(minimize[i], minimize[j]);
			    if (newpal != null) {
				minimize[i] = newpal;
				minimize.RemoveAt(j);
				j--;
			    }
			}
		    }
		}
		for (int i = 0; i < minimize.Count; i++) {
		    while (minimize[i].Count < 4) {
			minimize[i].Add(0xF);
		    }
		}
		merged.AddRange(minimize);
		if (merged.Count > maxPals) {
		    return false;
		}
		return false;
	    }

	    public void SetCustomMapGraphics(Stream readStream,
					     int maxCHR,
					     int maxPal,
					     int[] PALETTE_OFFSET,
					     int PALETTE_ASSIGNMENT,
					     int PATTERNTABLE_OFFSET,
					     int PATTERNTABLE_ASSIGNMENT)
	    {
		IImageFormat format;
		Image<Rgba32> image = Image.Load<Rgba32>(readStream, out format);

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

		for(int imagecount = 0; imagecount < 128; imagecount += 1) {
		    int top = (imagecount / 16) * 16;
		    int left = (imagecount % 16) * 16;

		    var firstUnique = new Dictionary<Rgba32, int>();
		    var colors = new List<Rgba32>();
		    for (int y = top; y < (top+16); y++) {
			for (int x = left; x < (left+16); x++) {
			    if (!colors.Contains(image[x,y])) {
				firstUnique[image[x,y]] = (x<<16 | y);
				colors.Add(image[x,y]);
			    }
			}
		    }
		    List<byte> pal;
		    if (!makeNTPalette(colors, NESpalette, out pal, toNEScolor)) {
			Console.WriteLine($"Failed importing overworld tile at {left}, {top}, too many unique colors (limit 4 unique colors):");
			for (int i = 0; i < pal.Count; i++) {
			    Console.WriteLine($"NES palette {i}: ${pal[i],2:X}");
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
		if (!mergePalettes(candidateMapPals, mapPals, maxPal)) {
		}

		//int maxCHR = 245;
		int excessCHR = 0;
		Console.WriteLine($"mapPals {mapPals.Count}");

		for(int imagecount = 0; imagecount < 128; imagecount += 1) {
		    int top = (imagecount / 16) * 16;
		    int left = (imagecount % 16) * 16;

		    int palidx = findPalette(mapPals, candidateMapPals[imagecount]);
		    if  (palidx == -1) {
			palidx = 0;
		    }
		    var usepal = (byte)palidx;

		    Dictionary<Rgba32, byte> index;
		    colorToPaletteIndex(mapPals[usepal], toNEScolor, out index);

		    Put(PALETTE_ASSIGNMENT + imagecount, new byte[] {(byte)((usepal << 6) + (usepal << 4) + (usepal << 2) + (usepal))});

		    foreach (var loadchr in new ValueTuple<int, int, int>[] {
			    (0, 0, 0),
				(8, 0, 128),
				(0, 8, 256),
				(8, 8, 384)})
		    {
			byte idx = chrIndex(makeTile(image, top+loadchr.Item2, left+loadchr.Item1, index), chrEntries, maxCHR);
			if (idx == 0xff) {
			    Console.WriteLine($"Error importing CHR at {left+loadchr.Item1}, {top+loadchr.Item2}, in map tile {imagecount} too many unique CHR");
			    idx = 0;
			    excessCHR++;
			}
			Put(PATTERNTABLE_ASSIGNMENT + loadchr.Item3 + imagecount, new byte[]{idx});
		    }
		}

		if (mapPals.Count > 4) {
		    Console.WriteLine($"!!! More than 4 unique 4-color palettes ({mapPals.Count})");
		}
		if (excessCHR > 0) {
		    Console.WriteLine($"!!! More than {maxCHR} unique 8x8 tiles, must eliminate {excessCHR} excess tiles");
		}

		for (int i = 0; i < Math.Min(4, mapPals.Count); i++) {
		    foreach (var j in PALETTE_OFFSET) {
			Put(j + i*4, mapPals[i].ToArray());
		    }
		}

		for (int i = 0; i < chrEntries.Count; i++) {
		    Put(PATTERNTABLE_OFFSET + (i * 16), EncodeForPPU(chrEntries[i]));
		}
	    }

	    public void FiendImport(Image<Rgba32> image, int sizeX, int sizeY,
				    int imageOffsetX, int imageOffsetY, List<byte[]> chrEntries,
				    int nametableDest, int paletteDest1, int paletteDest2) {

		List<List<byte>> candidatePals = new List<List<byte>>();
		var toNEScolor = new Dictionary<Rgba32, byte>();

		for(int areaY = 0; areaY < sizeY/2; areaY += 1) {
		    for(int areaX = 0; areaX < sizeX/2; areaX += 1) {
			int top = imageOffsetY + areaY * 16;
			int left = imageOffsetX + areaX * 16;

			//Console.WriteLine($"area {areaX} {areaY}    pixel {left} {top}   candpal {candidatePals.Count}");

			var firstUnique = new Dictionary<Rgba32, int>();
			var colors = new List<Rgba32>();
			for (int y = top; y < (top+16); y++) {
			    for (int x = left; x < (left+16); x++) {
				if (!colors.Contains(image[x,y])) {
				    firstUnique[image[x,y]] = (x<<16 | y);
				    colors.Add(image[x,y]);
				}
			    }
			}
			List<byte> pal;
			if (!makeNTPalette(colors, NESpalette, out pal, toNEScolor)) {
			    Console.WriteLine($"Failed importing fiend at {left}, {top}, too many unique colors (limit 4 unique colors):");
			    for (int i = 0; i < pal.Count; i++) {
				Console.WriteLine($"NES palette {i}: ${pal[i],2:X}");
			    }
			    return;
			}
			candidatePals.Add(pal);
		    }
		}

		var fiendPals = new List<List<byte>>();
		if (!mergePalettes(candidatePals, fiendPals, 2)) {
		    Console.WriteLine($"Too many unique 4-color palettes");
		}

		byte[] attributeTable = new byte[16];
		byte[] nametable = new byte[sizeX*sizeY];

		attributeTable[0] = (byte)((3 << 4) | 3);
		attributeTable[4] = (byte)((3 << 4) | 3);
		attributeTable[8] = (byte)((3 << 4) | 3);
		attributeTable[12] = (byte)((3 << 6) | (3 << 4) | 3);
		attributeTable[13] = (byte)((3 << 6) | (3 << 4));
		attributeTable[14] = (byte)((3 << 6) | (3 << 4));
		attributeTable[15] = (byte)((3 << 6) | (3 << 4));

		// iterate on 16x16 "areas"
		for(int areaY = 0; areaY < sizeY/2; areaY += 1) {
		    for(int areaX = 0; areaX < sizeX/2; areaX += 1) {
			var srcPalIdx = areaY * (sizeX/2) + areaX;

			//Console.WriteLine($"area {areaX} {areaY}       candpal {srcPalIdx}");

			int palidx = findPalette(fiendPals, candidatePals[srcPalIdx]);
			if  (palidx == -1) {
			    palidx = 0;
			}
			var usepal = (byte)palidx;

			Dictionary<Rgba32, byte> index;
			colorToPaletteIndex(fiendPals[usepal], toNEScolor, out index);

			// Console.WriteLine($"fiendPals[{usepal}]");
			// foreach (var i in fiendPals[usepal]) {
			//     Console.WriteLine($"${i,2:X}");
			// }
			// Console.WriteLine($"index");
			// foreach (var i in index) {
			//     Console.WriteLine($"{i.Key}: {i.Value}");
			// }

			int aX, aY;
			if (sizeX == 8) {
			    aX = areaX + 2;
			    aY = areaY + 2;
			} else {
			    aX = areaX + 1;
			    aY = areaY + 1;
			}

			// boss palettes are 2nd and 3rd
			usepal += 1;

			byte v = 0;
			if (aY % 2 == 0 && aX % 2 == 0) {
			    v |= usepal;
			}
			if (aY % 2 == 0 && aX % 2 == 1) {
			    v |= (byte)(usepal << 2);
			}
			if (aY % 2 == 1 && aX % 2 == 0) {
			    v |= (byte)(usepal << 4);
			}
			if (aY % 2 == 1 && aX % 2 == 1) {
			    v |= (byte)(usepal << 6);
			}
			int ati = ((aY/2) * 4) + (aX/2);
			if (ati < attributeTable.Length) {
			    attributeTable[ati] |= v;
			}

			// put the attribute table
			for(int tilesY = areaY*2; tilesY < (areaY+1)*2; tilesY += 1) {
			    for(int tilesX = areaX*2; tilesX < (areaX+1)*2; tilesX += 1) {
				int top = imageOffsetY + tilesY * 8;
				int left = imageOffsetX + tilesX * 8;
				byte idx = chrIndex(makeTile(image, top, left, index), chrEntries, 110);
				if (idx == 0xff) {
				    Console.WriteLine($"Error importing CHR at {left}, {top}, too many unique CHR ");
				    idx = 0;
				}
				// put the NT
				nametable[tilesY*sizeX + tilesX] = (byte)(18 + idx);
			    }
			}
		    }
		}
		Put(nametableDest, nametable);
		Put(nametableDest+nametable.Length, attributeTable);
		Put(paletteDest1, fiendPals[0].ToArray());
		if (fiendPals.Count == 2) {
		    Put(paletteDest2, fiendPals[1].ToArray());
		}
	    }

	    public void SetCustomFiendGraphics(Stream fiends) {
		// 0B_92E0
		//    $50 bytes of TSA for all 4 fiend graphics (resulting in $140 bytes of data total)
		//      $40 bytes of NT TSA (8x8 image)
		//      $10 bytes of attributes  (4x4)

		// 0B_9420
		//    $C0 bytes of TSA for chaos
		//     $A8 bytes of NT TSA (14x12 image)
		//     $10 bytes of attributes  (4x4x)

		var formations = LoadFormations();

		const int TIAMAT1 = 0x77;
		const int KRAKEN1 = 0x78;
		const int KARY1 = 0x79;
		const int LICH1 = 0x7A;

		IImageFormat format;

		Image<Rgba32> image = Image.Load<Rgba32>(fiends, out format);
		{
		    List<byte[]> CHR = new List<byte[]>();
		    FiendImport(image, 8, 8,  0, 0, CHR, FIENDDRAW_TABLE + (0x50 * 1), BATTLEPALETTE_OFFSET+(formations[LICH1].pal1 * 4), BATTLEPALETTE_OFFSET+(formations[LICH1].pal2 * 4));
		    FiendImport(image, 8, 8, 64, 0, CHR, FIENDDRAW_TABLE + (0x50 * 0), BATTLEPALETTE_OFFSET+(formations[KARY1].pal1 * 4), BATTLEPALETTE_OFFSET+(formations[KARY1].pal2 * 4));
		    if (CHR.Count < 110) {
			int offset = BATTLEPATTERNTABLE_OFFSET + (formations[LICH1].tileset * 2048) + (18 * 16);
			for (int i = 0; i < CHR.Count; i++) {
			    Put(offset + (i*16), EncodeForPPU(CHR[i]));
			}
		    } else {
			Console.WriteLine($"Error importing Lich and Kary, too many unique CHR ({CHR.Count}), must be less than 110 unique 8x8 tiles between both fiends");
		    }
		}
		{
		    List<byte[]> CHR = new List<byte[]>();
		    FiendImport(image, 8, 8,  0, 64, CHR, FIENDDRAW_TABLE + (0x50 * 2), BATTLEPALETTE_OFFSET+(formations[KRAKEN1].pal1 * 4), BATTLEPALETTE_OFFSET+(formations[KRAKEN1].pal2 * 4));
		    FiendImport(image, 8, 8, 64, 64, CHR, FIENDDRAW_TABLE + (0x50 * 3), BATTLEPALETTE_OFFSET+(formations[TIAMAT1].pal1 * 4), BATTLEPALETTE_OFFSET+(formations[TIAMAT1].pal2 * 4));
		    if (CHR.Count < 110) {
			int offset = BATTLEPATTERNTABLE_OFFSET + (formations[KRAKEN1].tileset * 2048) + (18 * 16);
			for (int i = 0; i < CHR.Count; i++) {
			    Put(offset + (i*16), EncodeForPPU(CHR[i]));
			}
		    } else {
			Console.WriteLine($"Error importing Kraken and Tiamat, too many unique CHR ({CHR.Count}), must be less than 110 unique 8x8 tiles between both fiends");
		    }
		}
	    }

	    public void SetCustomChaosGraphics(Stream chaos) {
		var formations = LoadFormations();
		IImageFormat format;
		const int CHAOS = 0x7B;
		Image<Rgba32> image = Image.Load<Rgba32>(chaos, out format);
		List<byte[]> CHR = new List<byte[]>();
		FiendImport(image, 14, 12,  0, 0, CHR, CHAOSDRAW_TABLE, BATTLEPALETTE_OFFSET+(formations[CHAOS].pal1 * 4), BATTLEPALETTE_OFFSET+(formations[CHAOS].pal2 * 4));
		if (CHR.Count < 110) {
		    int offset = BATTLEPATTERNTABLE_OFFSET + (formations[CHAOS].tileset * 2048) + (18 * 16);
		    for (int i = 0; i < CHR.Count; i++) {
			Put(offset + (i*16), EncodeForPPU(CHR[i]));
		    }
		} else {
		    Console.WriteLine($"Error importing Chaos, too many unique CHR ({CHR.Count}), must be less than 110 unique 8x8 tiles");
		}
	    }

	    public void SetCustomBattleBackdrop(Stream backdrop) {
		//const int BATTLEBACKDROPASSIGNMENT_OFFSET =		0x3310;
		const int BATTLEBACKDROPPALETTE_OFFSET =		0x3200;

		IImageFormat format;
		Image<Rgba32> image = Image.Load<Rgba32>(backdrop, out format);
		var toNEScolor = new Dictionary<Rgba32, byte>();

		for (int count = 0; count < 16; count++) {
		    Console.WriteLine($"Importing backdrop {count}");
		    int top = count*32;
		    int left = 0;

		    var firstUnique = new Dictionary<Rgba32, int>();
		    var colors = new List<Rgba32>();
		    for (int y = top; y < (top+32); y++) {
			for (int x = left; x < (left+32); x++) {
			    if (!colors.Contains(image[x,y])) {
				firstUnique[image[x,y]] = (x<<16 | y);
				colors.Add(image[x,y]);
			    }
			}
		    }
		    List<byte> pal;
		    if (!makeNTPalette(colors, NESpalette, out pal, toNEScolor)) {
			Console.WriteLine($"Failed importing battle backdrop at {left}, {top}, too many unique colors (limit 4 unique colors):");
			for (int i = 0; i < pal.Count; i++) {
			    Console.WriteLine($"NES palette {i}: ${pal[i],2:X}");
			}
			continue;
		    }

		    Dictionary<Rgba32, byte> index;
		    colorToPaletteIndex(pal, toNEScolor, out index);

		    Put(BATTLEBACKDROPPALETTE_OFFSET + (count * 4), pal.ToArray());

		    int n = 1;
		    for (int y = top; y < (top+32); y += 8) {
			for (int x = left; x < (left+32); x += 8) {
			    var tile = makeTile(image, y, x, index);
			    Put(BATTLEPATTERNTABLE_OFFSET + (count * 2048) + (n*16), EncodeForPPU(tile));
			    n++;
			}
		    }

		}
	    }

	    public void SetCustomWeaponGraphics(Stream stream) {
		IImageFormat format;
		Image<Rgba32> image = Image.Load<Rgba32>(stream, out format);

		const int WEAPONMAGICGRAPHIC_OFFSET =			0x26800;

		Dictionary<Rgba32, byte> index = new Dictionary<Rgba32, byte> {
		    { new Rgba32(0x00, 0x00, 0x00), 0 },
		    { new Rgba32(0x7b, 0x7b, 0x7b), 1 },
		    { new Rgba32(0xbd, 0xbd, 0xbd), 2 },
		    { new Rgba32(0xff, 0xff, 0xff), 3 }
		};

		int n = 0;
		for (int w = 0; w < 12; w++) {
		    for (int y = 0; y < 16; y += 8) {
			for (int x = (w*16); x < (w+1)*16; x += 8) {
			    var tile = makeTile(image, y, x, index);
			    Put(WEAPONMAGICGRAPHIC_OFFSET + (n*16), EncodeForPPU(tile));
			    n++;
			}
		    }
		}
	    }
	}
}
