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
	    const int MAPMANPALETTE_OFFSET =			0x3B0;
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

	    public void SetCustomPlayerSprites(Stream readStream) {
		//dlg.graphicoffset = MAPMANGRAPHIC_OFFSET + (cur_class << 8);
		//dlg.paletteoffset = -cur_class - 1;

		IImageFormat format;
		Image<Rgba32> image = Image.Load<Rgba32>(readStream, out format);

		// Unpromoted classes
		for (int cur_class = 0; cur_class < 12; cur_class++) {
		    int top = 24 + (40*(cur_class >= 6 ? cur_class-6 : cur_class));
		    int left = (cur_class >= 6) ? 104 : 0;

		    Console.WriteLine($"top {top} left {left}");
		    // top two tiles can have a different palette than
		    // the bottom two tiles
		    var colors = new List<Rgba32>();
		    for (int y = top; y < (top+8); y++) {
			for (int x = left; x < (left+64); x++) {
			    if (!colors.Contains(image[x,y])) {
				colors.Add(image[x,y]);
			    }
			}
		    }
		    Console.WriteLine("Found top colors:");
		    for (int i = 0; i < colors.Count; i++) {
			Console.WriteLine(colors[i]);
		    }
		    colors = new List<Rgba32>();
		    for (int y = top+8; y < (top+16); y++) {
			for (int x = left; x < (left+64); x++) {
			    if (!colors.Contains(image[x,y])) {
				colors.Add(image[x,y]);
			    }
			}
		    }
		    Console.WriteLine("Found bottom colors:");
		    for (int i = 0; i < colors.Count; i++) {
			Console.WriteLine(colors[i]);
		    }
		}

		var NESpalette = new Rgba32[]{
		    new Rgba32(84, 84, 84),
		    new Rgba32(0, 30, 116),
		};

		/*		        8  16 144   48   0 136   68   0 100   92   0  48   84   4   0   60  24   0   32  42   0    8  58   0    0  64   0    0  60   0    0  50  60    0   0   0
152 150 152    8  76 196   48  50 236   92  30 228  136  20 176  160  20 100  152  34  32  120  60   0   84  90   0   40 114   0    8 124   0    0 118  40    0 102 120    0   0   0
236 238 236   76 154 236  120 124 236  176  98 236  228  84 236  236  88 180  236 106 100  212 136  32  160 170   0  116 196   0   76 208  32   56 204 108   56 180 204   60  60  60
236 238 236  168 204 236  188 188 236  212 178 236  236 174 236  236 174 212  236 180 176  228 196 144  204 210 120  180 222 120  168 226 144  152 226 180  160 214 228  160 162 160 */

		// to do:
		// 1) loop through unpromoted class
		// 2) loop through pixels
		// 3)

		/*var newtile1 = new byte[] {
		    0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01,
		    0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
		    0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
		    0x00, 0x01, 0x01, 0x01, 0x02, 0x01, 0x01, 0x01,
		    0x00, 0x01, 0x01, 0x01, 0x02, 0x01, 0x01, 0x01,
		    0x00, 0x01, 0x01, 0x01, 0x02, 0x01, 0x01, 0x01,
		    0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
		    0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
		};
		var newtile2 = new byte[] {
		    0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00,
		    0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00,
		    0x01, 0x01, 0x01, 0x02, 0x01, 0x01, 0x00, 0x00,
		    0x01, 0x01, 0x01, 0x02, 0x01, 0x01, 0x01, 0x00,
		    0x01, 0x01, 0x01, 0x02, 0x01, 0x01, 0x01, 0x00,
		    0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00,
		    0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00,
		    0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00,
		};
		var newtile3 = new byte[] {
		    0x00, 0x00, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00,
		    0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00,
		    0x00, 0x01, 0x00, 0x01, 0x02, 0x01, 0x00, 0x00,
		    0x00, 0x01, 0x00, 0x01, 0x02, 0x01, 0x01, 0x00,
		    0x00, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x00,
		    0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00,
		    0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00,
		    0x00, 0x00, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00,
		};
		var newtile4 = new byte[] {
		    0x00, 0x00, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00,
		    0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00,
		    0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00,
		    0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00,
		    0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00,
		    0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00,
		    0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00,
		    0x00, 0x00, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00,
		};

		var tile1 = EncodeForPPU(newtile1);
		var tile2 = EncodeForPPU(newtile2);
		var tile3 = EncodeForPPU(newtile3);
		var tile4 = EncodeForPPU(newtile4);

		int cur_class = 0;
		Put(MAPMANGRAPHIC_OFFSET + (cur_class << 8) + (MAPMAN_DOWN * 16*4) + (16*0),  tile1);
		Put(MAPMANGRAPHIC_OFFSET + (cur_class << 8) + (MAPMAN_DOWN * 16*4) + (16*1),  tile2);
		Put(MAPMANGRAPHIC_OFFSET + (cur_class << 8) + (MAPMAN_DOWN * 16*4) + (16*2),  tile3);
		Put(MAPMANGRAPHIC_OFFSET + (cur_class << 8) + (MAPMAN_DOWN * 16*4) + (16*3),  tile4);
		*/
	    }
	}
}
