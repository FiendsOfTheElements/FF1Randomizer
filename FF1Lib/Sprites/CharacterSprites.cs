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
		// Copied from FFHackster.
		// LATER fix these so they use banks and offsets within banks.
		const int CHARBATTLEPIC_OFFSET = 0x25000;



		const int SHOPKEEPERGRAPHIC_OFFSET = 0x24080;



		//const int lut_MapmanPalettes = 0x8150;
		const int MAPMANPALETTE_OFFSET = 0x8150;
		const int MAPMANGRAPHIC_OFFSET = 0x9000;
		const int VEHICLEGRAPHIC_OFFSET = 0x9D00;

		const int NPCGRAPHIC_OFFSET = 0xA200;

		const int MAPMAN_DOWN = 0;
		const int MAPMAN_UP = 1;
		const int MAPMAN_SIDE1 = 1;
		const int MAPMAN_SIDE2 = 2;

        bool makeMapmanPalette(List<Rgba32> colors, Rgba32[] NESpalette,
				 out List<byte> pal,
				 out Dictionary<Rgba32, byte> toIndex)
		{
			pal = new List<byte>();
			toIndex = new Dictionary<Rgba32, byte>();

			for (int i = 0; i < colors.Count; i++)
			{
				if (colors[i].R >= 250 && colors[i].G <= 5 && colors[i].B >= 250)
				{
					// treat magenta as transparent
					toIndex[colors[i]] = 0;
					continue;
				}

				byte selected = selectColor(colors[i], NESpalette);
				int idx = pal.IndexOf(selected);
				if (idx == -1)
				{
					// add 1 everything is going to get shifted
					// when the tranparent entry is added
					idx = pal.Count + 1;
					pal.Add(selected);
				}
				toIndex[colors[i]] = (byte)idx;
			}

			// insert the transparent entry.
			pal.Insert(0, 0x0F);

			if (pal.Count > 4)
			{
				return false;
			}
			while (pal.Count < 4)
			{
				pal.Add(0x0F);
			}

			return true;
		}


		bool makeBattlePalette(List<Rgba32> colors, Rgba32[] NESpalette,
				 out List<byte> pal,
				 out Dictionary<Rgba32, byte> toIndex)
		{
			var paltmp = new List<byte>();
			var toIndexTmp = new Dictionary<Rgba32, byte>();

			// transparent is black (because battle sprites
			// have a black background), it can be used as a
			// "color" in palette entry 0
			paltmp.Insert(0, 0x0F);

			for (int i = 0; i < colors.Count; i++)
			{
				if (colors[i].R <= 5 && colors[i].G <= 5 && colors[i].B <= 5)
				{
					// treat black as transparent
					toIndexTmp[colors[i]] = 0;
					continue;
				}

				byte selected = selectColor(colors[i], NESpalette);
				int idx = paltmp.IndexOf(selected);
				if (idx == -1)
				{
					idx = paltmp.Count;
					paltmp.Add(selected);
				}
				toIndexTmp[colors[i]] = (byte)idx;
			}

			if (paltmp.Count > 4)
			{
				pal = paltmp;
				toIndex = toIndexTmp;
				return false;
			}
			while (paltmp.Count < 4)
			{
				paltmp.Add(0x0F);
			}

			toIndex = new Dictionary<Rgba32, byte>();
			pal = new List<byte>(paltmp);
			// Need to keep black at position 0
			pal.RemoveAt(0);

			// Need to sort the palette & update the mapping so
			// that battle sprites that need to share palettes all
			// have the colors in the same order.
			pal.Sort();
			pal.Insert(0, 0x0F);
			foreach (var kv in toIndexTmp)
			{
				toIndex[kv.Key] = (byte)pal.IndexOf(paltmp[kv.Value]);
			}

			return true;
		}
        


		public async Task ImportMapman(Image<Rgba32> image, int cur_class, int top, int left, Rgba32[] NESpalette)
		{
			// the mapman head tiles have a different palette
			// than the body tiles.
			var headColors = new List<Rgba32>();
			var firstUnique = new Dictionary<Rgba32, int>();
			for (int y = top; y < (top + 8); y++)
			{
				for (int x = left; x < (left + 64); x++)
				{
					if (!headColors.Contains(image[x, y]))
					{
						firstUnique[image[x, y]] = (x << 16 | y);
						headColors.Add(image[x, y]);
					}
				}
			}

			List<byte> headPal;
			Dictionary<Rgba32, byte> headIndex;
			if (!makeMapmanPalette(headColors, NESpalette, out headPal, out headIndex))
			{
				await this.Progress($"WARNING: Failed importing top half of mapman for {ClassNames[cur_class]}, too many unique colors (limit 3 unique colors + magenta for transparent):");
				for (int i = 1; i < headPal.Count; i++)
				{
					await this.Progress($"WARNING: NES palette {i}: ${headPal[i],2:X}");
				}
				foreach (var i in headIndex)
				{
					int c = firstUnique[i.Key];
					await this.Progress($"WARNING: RGB to index {i.Key}: {i.Value}  first appears at {c >> 16}, {c & 0xFFFF}");
				}
				return;
			}

			var bodyColors = new List<Rgba32>();
			firstUnique = new Dictionary<Rgba32, int>();
			for (int y = top + 8; y < (top + 16); y++)
			{
				for (int x = left; x < (left + 64); x++)
				{
					if (!bodyColors.Contains(image[x, y]))
					{
						firstUnique[image[x, y]] = (x << 16 | y);
						bodyColors.Add(image[x, y]);
					}
				}
			}

			List<byte> bodyPal;
			Dictionary<Rgba32, byte> bodyIndex;
			if (!makeMapmanPalette(bodyColors, NESpalette, out bodyPal, out bodyIndex))
			{
				await this.Progress($"WARNING: Failed importing bottom half of mapman for {ClassNames[cur_class]}, too many unique colors (limit 3 unique colors + magenta for transparent):",
					  1 + bodyPal.Count + bodyIndex.Count);
				for (int i = 1; i < bodyPal.Count; i++)
				{
					await this.Progress($"WARNING: NES palette {i}: ${bodyPal[i],2:X}");
				}
				foreach (var i in bodyIndex)
				{
					int c = firstUnique[i.Key];
					await this.Progress($"WARNING: RGB to index {i.Key}: {i.Value}  first appears at {c >> 16}, {c & 0xFFFF}");
				}
				return;
			}

			for (int mapmanPos = 0; mapmanPos < 4; mapmanPos++)
			{
				var newleft = left + (mapmanPos * 16);

				var headTileLeft = makeTile(image, top, newleft, headIndex);
				var headTileRight = makeTile(image, top, newleft + 8, headIndex);

				var bodyTileLeft = makeTile(image, top + 8, newleft, bodyIndex);
				var bodyTileRight = makeTile(image, top + 8, newleft + 8, bodyIndex);

				Put(MAPMANGRAPHIC_OFFSET + (cur_class << 8) + (mapmanPos * 16 * 4) + (16 * 0), EncodeForPPU(headTileLeft));
				Put(MAPMANGRAPHIC_OFFSET + (cur_class << 8) + (mapmanPos * 16 * 4) + (16 * 1), EncodeForPPU(headTileRight));
				Put(MAPMANGRAPHIC_OFFSET + (cur_class << 8) + (mapmanPos * 16 * 4) + (16 * 2), EncodeForPPU(bodyTileLeft));
				Put(MAPMANGRAPHIC_OFFSET + (cur_class << 8) + (mapmanPos * 16 * 4) + (16 * 3), EncodeForPPU(bodyTileRight));
			}

			int offsetIntoLut = cur_class << 3;
			// Write the palettes into a new LUT in bank $0F
			// Will be read using the code below.
			//Console.WriteLine($"writing to {MAPMANPALETTE_OFFSET + offsetIntoLut} {MAPMANPALETTE_OFFSET + offsetIntoLut + 4}");
			PutInBank(0x0F, MAPMANPALETTE_OFFSET + offsetIntoLut, headPal.ToArray());
			PutInBank(0x0F, MAPMANPALETTE_OFFSET + offsetIntoLut + 4, bodyPal.ToArray());
		}

		public async Task<int> ImportBattleSprite(Image<Rgba32> image, int cur_class, int top, int left, Rgba32[] NESpalette, List<List<byte>> battlePals)
		{
			var firstUnique = new Dictionary<Rgba32, int>();
			var colors = new List<Rgba32>();

			for (int y = top; y < (top + 24); y++)
			{
				for (int x = left; x < (left + 104); x++)
				{
					if (x >= (left + 80) && y < (top + 8))
					{
						// This is the space above the dead character,
						// ignore it
						continue;
					}
					if (!colors.Contains(image[x, y]))
					{
						firstUnique[image[x, y]] = (x << 16 | y);
						colors.Add(image[x, y]);
					}
				}
			}

			List<byte> pal;
			Dictionary<Rgba32, byte> index;
			if (!makeBattlePalette(colors, NESpalette, out pal, out index))
			{
				await this.Progress($"WARNING: Failed importing battle sprites for {ClassNames[cur_class]}, too many unique colors (limit 3 unique colors + black):", 1 + pal.Count + index.Count);
				for (int i = 0; i < pal.Count; i++)
				{
					await this.Progress($"WARNING: NES palette {i}: ${pal[i],2:X}");
				}
				foreach (var i in index)
				{
					int c = firstUnique[i.Key];
					await this.Progress($"WARNING: RGB to index {i.Key}: {i.Value}  first appears at {c >> 16}, {c & 0xFFFF}");
				}
				return -1;
			}

			int usepal = -1;

			if (battlePals != null)
			{
				int b;
				for (b = 0; b < battlePals.Count; b++)
				{
					if (battlePals[b].Count == 0)
					{
						for (int j = 0; j < 4; j++) { battlePals[b].Add(pal[j]); }
						usepal = b;
						break;
					}
					else if (Enumerable.SequenceEqual(pal, battlePals[b]))
					{
						usepal = b;
						break;
					}
				}

				if (b == battlePals.Count)
				{
					await this.Progress($"WARNING: Failed importing battle sprites for {ClassNames[cur_class]}, has a different palette from other classes", 1 + pal.Count + (battlePals.Count * 3) + index.Count);
					if (battlePals.Count == 2)
					{
						await this.Progress($"WARNING: maybe you meant to enable Three Battle Palettes?", 1);
					}

					for (int i = 0; i < pal.Count; i++)
					{
						await this.Progress($"WARNING: palette for class, idx {i}: ${pal[i],2:X}");
					}
					for (b = 0; b < battlePals.Count; b++)
					{
						for (int i = 0; i < battlePals[b].Count; i++)
						{
							await this.Progress($"WARNING: palette {b}, idx {i}: ${battlePals[b][i],2:X}");
						}
					}
					foreach (var i in index)
					{
						await this.Progress($"WARNING: RGB to index {i.Key}: {i.Value}");
					}
					usepal = 0; // whatever
				}
			}

			// const byte ConstPicFormation[39] = {	//3 x 13 pic formation
			//     0, 1, 0, 1, 8, 9,14,15,20,21,255,255,255,
			//     2, 3, 2, 3,10,11,16,17,22,23,26,27,28,
			//     4, 5, 6, 7,12,13,18,19,24,25,29,30,31};

			var ConstPicFormationNoDup = new byte[39] {	//3 x 13 pic formation
				0, 1, 255, 255,  8, 9,14,15,20,21,255,255,255,
				2, 3, 255, 255, 10,11,16,17,22,23,26,27,28,
				4, 5,   6,   7, 12,13,18,19,24,25,29,30,31
			};

			for (int y = 0; y < 3; y++)
			{
				for (int x = 0; x < 13; x++)
				{
					var tileidx = ConstPicFormationNoDup[(y * 13) + x];
					if (tileidx == 255)
					{
						continue;
					}
					var tile = makeTile(image, top + (y * 8), left + (x * 8), index);
					Put(CHARBATTLEPIC_OFFSET + (cur_class << 9) + (tileidx * 16), EncodeForPPU(tile));
				}
			}

			if (usepal != -1)
			{
				// Set the palette to use for each class, this
				// one is used for character selection and
				// subscreen
				PutInBank(0x1F, 0xECA4 + cur_class, new byte[] { (byte)usepal });

				// it loads this one in battle, it is redundant with the one
				// in 1F but we have to set it anyway
				PutInBank(0x0C, 0xA03C + cur_class, new byte[] { (byte)usepal });
			}
			return usepal;
		}

		public async Task ImportVehicleSprite(Image<Rgba32> image, int top, int left, Rgba32[] NESpalette)
		{
				
			// vehicle sprites share a single palette, primarily because they can all appear
			// on the screen at the same time.


			var vehicleColors = new List<Rgba32>();
			var firstUnique = new Dictionary<Rgba32, int>();
			for (int y = top; y < (top + 48); y++)
			{
				for (int x = left; x < (left + 96); x++)
				{
					if (!vehicleColors.Contains(image[x, y]))
					{
						firstUnique[image[x, y]] = (x << 16 | y);
						vehicleColors.Add(image[x, y]);
					}
				}
			}

			List<byte> vehiclePal;
			Dictionary<Rgba32, byte> vehicleIndex;
			if (!makeMapmanPalette(vehicleColors, NESpalette, out vehiclePal, out vehicleIndex))
			{
				await this.Progress($"WARNING: Failed importing vehicle sprites, too many unique colors (limit 3 unique colors + magenta for transparent):",
					  1 + vehiclePal.Count + vehicleIndex.Count);
				for (int i = 1; i < vehiclePal.Count; i++)
				{
					await this.Progress($"WARNING: NES palette {i}: ${vehiclePal[i],2:X}");
				}
				foreach (var i in vehicleIndex)
				{
					int c = firstUnique[i.Key];
					await this.Progress($"WARNING: RGB to index {i.Key}: {i.Value}  first appears at {c >> 16}, {c & 0xFFFF}");
				}
				return;
			}


			
			for (int cur_vehicle =0; cur_vehicle < 3; cur_vehicle++)
			{
			

				for (int vehiclePos = 0; vehiclePos < 6; vehiclePos++)
				{
					var newtop = top + (cur_vehicle * 16);
					var newleft = left + (vehiclePos * 16);

					var tileTopLeft = makeTile(image, newtop, newleft, (vehicleIndex));
					var tileTopRight = makeTile(image, newtop, newleft + 8, (vehicleIndex));

					var tileBottomLeft = makeTile(image, newtop + 8, newleft, (vehicleIndex));
					var tileBottomRight = makeTile(image, newtop + 8, newleft + 8, (vehicleIndex));

					// rom sprites are arranged ship, airship, canoe
					//int[] vehicle_map = {2,0,1};

					Put(VEHICLEGRAPHIC_OFFSET + (cur_vehicle * 384) + (vehiclePos * 16 * 4) + (16 * 0), EncodeForPPU(tileTopLeft));
					Put(VEHICLEGRAPHIC_OFFSET + (cur_vehicle * 384) + (vehiclePos * 16 * 4) + (16 * 1), EncodeForPPU(tileTopRight));
					Put(VEHICLEGRAPHIC_OFFSET + (cur_vehicle * 384) + (vehiclePos * 16 * 4) + (16 * 2), EncodeForPPU(tileBottomLeft));
					Put(VEHICLEGRAPHIC_OFFSET + (cur_vehicle * 384) + (vehiclePos * 16 * 4) + (16 * 3), EncodeForPPU(tileBottomRight));
				}
			}



			// the vanilla vehicle palette is part of the overworld palette
			int offsetIntoLut = 0x398;
			Put(offsetIntoLut, vehiclePal.ToArray());
		}



		public async Task SetCustomPlayerSprites(Stream readStream, bool threePalettes, MapmanSlot mapmanLeader = MapmanSlot.Leader)
		{
			//IImageFormat format;
			Image<Rgba32> image = Image.Load<Rgba32>(readStream);
			if (image.Width != 208 || (image.Height != 240 && image.Height != 288))
			{
				await this.Progress($"WARNING: Custom player sprites have dimensions {image.Width}x{image.Height}, expected 208x240 or 208x288");
			}

			var battlePals = new List<List<byte>>();
			battlePals.Add(new List<byte>());
			battlePals.Add(new List<byte>());
			if (threePalettes)
			{
				battlePals.Add(new List<byte>());
			}

			for (int cur_class = 0; cur_class < 12; cur_class++)
			{
				await ImportMapman(image, cur_class, 24 + (40 * (cur_class >= 6 ? cur_class - 6 : cur_class)), ((cur_class >= 6) ? 104 : 0), NESpalette);
				await ImportBattleSprite(image, cur_class, (40 * (cur_class >= 6 ? cur_class - 6 : cur_class)), ((cur_class >= 6) ? 104 : 0), NESpalette, battlePals);
			}

			for (int i = 0; i < battlePals.Count; i++)
			{
				PutInBank(0x1F, 0xEBA5 + (i * 4), battlePals[i].ToArray());
			}

			
			// add palette for "none" mapman
			PutInBank(0x0F, MAPMANPALETTE_OFFSET + (13 << 3), new byte[] {0x0F, 0x0F, 0x12, 0x36,
										0x0F, 0x0F, 0x21, 0x36});

			// if the image contains vehicle sprites, process those
			if (image.Height == 288)
			{
				await ImportVehicleSprite(image, 240, 0, NESpalette);
			}

			// code in asm/0F_8150_MapmanPalette.asm

			// Replace the original code which loads the mapman
			// "palette" (it actually only changes 2 colors and
			// leaves the blank and "skin tone" alone)
			// With a jump to a new routine in bank 0F which loads
			// two complete 4 color palettes: either the mapman palettes
			// or the "none" palettes
			

			byte leader = (byte)((byte)mapmanLeader << 6);

			PutInBank(0x1F, 0xD8B6, Blob.FromHex("A90F2003FE4CC081EAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));
			PutInBank(0x0F, 0x81C0, Blob.FromHex($"AD{leader:X02}61C9FFD002A90D0A0A0A6908AAA008BD508199D003CA8810F660"));
		}

		public void SetCustomNPCGraphics(Stream stream) {
			Image<Rgba32> image = Image.Load<Rgba32>(stream);

			const int SHOPKEEPERGRAPHIC_SKIP = 0xE0;

			bool allColorsGood = true;
			// n is an iterator for successive 8x8 tiles in the ROM, added to the various offsets.
			int n = 0;
			// first process the shopkeepers
			// sk for x"shopkeeper"
			for (int sk = 0; sk < 8; sk++)
			{
				//skx is the x coordinate of successive shopkeepers
				int skx = sk * 16;
				
				
				// n needs to be reset for each shopkeeper
				n = 0;

				for (int y = 0; y < 24; y+=8)
				{
					for (int x = 0; x < 16; x+=8)
					{
						byte[] tile;
						try
						{
							tile = makeTile(image, y, skx+x, GrayscaleIndex);
						}
						catch (KeyNotFoundException)
						{
							if (allColorsGood)
							{
								Task task = Progress("WARNING: npcs.png contains incorrect colors. Quantizing.");
								allColorsGood = false;
							}
							tile = makeTileQuantize(image, y, skx+x, GrayscaleIndex);
						}
						Put(SHOPKEEPERGRAPHIC_OFFSET + sk*SHOPKEEPERGRAPHIC_SKIP + n*16, EncodeForPPU(tile));
						n++;
					}
				}
			}

			
			// next process the NPCs. we are leaving out the orb and plate map objects, so we have to skip those in the ROM.
			// 28 npcs total
			allColorsGood = true;
			// n is an iterator for successive 8x8 tiles in the ROM, added to the various offsets.
			// the NPC sprites are contiguous in ROM, so no need to reset it in this for loop.
			n = 0;

			for (int npc = 0; npc < 28; npc++)
			{
				int side = npc % 2;
				int row = npc / 2;

				for (int sprite = 0; sprite < 4; sprite++)
				{
					for (int y = 0; y < 16; y+=8)
					{
						for (int x = 0; x < 16; x+=8)
						{
							byte[] tile;
							try
							{
								tile = makeTile(image, row*16 + y + 24, side*64 + sprite*16 + x, NPCGrayscaleIndex);
							}
							catch (KeyNotFoundException)
							{
								if (allColorsGood)
								{
									Task task = Progress("WARNING: npcs.png contains incorrect colors. Quantizing.");
									allColorsGood = false;
								}
								tile = makeTileQuantize(image, row*16 + y + 24, side*64 + sprite*16 + x, NPCGrayscaleIndex);
							}
							Put(NPCGRAPHIC_OFFSET + n*16, EncodeForPPU(tile));
							n++;
							//each NPC is 16 tiles. In the ROM, orb is NPC 4 and plate is NPC 27; we skip those here.
							if (n == 4*16 || n == 27*16)
							{
								n+=16;
							}
							
						}
					}
				}
			}
		}


		public void SetCustomWeaponGraphics(Stream stream) {
			//IImageFormat format;
			Image<Rgba32> image = Image.Load<Rgba32>(stream);

			const int WEAPONMAGICGRAPHIC_OFFSET = 0x26800;

			int n = 0;
			for (int w = 0; w < 12; w++)
			{
				for (int y = 0; y < 16; y += 8)
				{
					for (int x = (w*16); x < (w+1)*16; x += 8)
					{
						byte[] tile;
						try
						{
							tile = makeTile(image, y, x, GrayscaleIndex);
						}
						catch (KeyNotFoundException)
						{
							Task task = Progress("WARNING: weapons.png contains incorrect colors. Quantizing.");
							tile = makeTileQuantize(image, y, x, GrayscaleIndex);
						}
						Put(WEAPONMAGICGRAPHIC_OFFSET + (n*16), EncodeForPPU(tile));
						n++;
					}
				}
			}
	    }

		public void SetCustomGearIcons(Stream stream)
		{
			//IImageFormat format;
			Image<Rgba32> image = Image.Load<Rgba32>(stream);

			// 0 = black
			// 1 = grey
			// 2 = blue
			// 3 = white

			Dictionary<Rgba32, byte> index = new Dictionary<Rgba32, byte>
			{
				{ new Rgba32(0x00, 0x00, 0x00), 0 },
				{ new Rgba32(0x7f, 0x7f, 0x7f), 1 },
				{ new Rgba32(116, 116, 116), 1 },
				{ new Rgba32(0x00, 0x00, 0xff), 2 },
				{ new Rgba32(36, 24, 140), 2 },
				{ new Rgba32(0xff, 0xff, 0xff), 3 },
				{ new Rgba32(252, 252, 252), 3 }
			};

			for (int w = 0; w < 11; w++)
			{
				byte[] tile;
				try
				{
					tile = makeTile(image, 0, (w * 8), index);
				}
				catch (KeyNotFoundException)
				{
					Task task = Progress("WARNING: gear_icons.png contains incorrect colors. Quantizing.");
					tile = makeTileQuantize(image, 0, (w * 8), index);
				}
				
				PutInBank(0x09, 0x8D40 + (w * 16), EncodeForPPU(tile));
				PutInBank(0x12, 0x8540 + (w * 16), EncodeForPPU(tile));
			}
	    }


		// These are terrible, but I need non-async functions for some goddamn reason
		public void ImportMapmanSync(Image<Rgba32> image, int cur_class, int top, int left, Rgba32[] NESpalette)
		{
			// the mapman head tiles have a different palette
			// than the body tiles.
			var headColors = new List<Rgba32>();
			var firstUnique = new Dictionary<Rgba32, int>();
			for (int y = top; y < (top + 8); y++)
			{
				for (int x = left; x < (left + 64); x++)
				{
					if (!headColors.Contains(image[x, y]))
					{
						firstUnique[image[x, y]] = (x << 16 | y);
						headColors.Add(image[x, y]);
					}
				}
			}

			List<byte> headPal;
			Dictionary<Rgba32, byte> headIndex;
			if (!makeMapmanPalette(headColors, NESpalette, out headPal, out headIndex))
			{
				Console.WriteLine($"WARNING: Failed importing top half of mapman for {ClassNames[cur_class]}, too many unique colors (limit 3 unique colors + magenta for transparent):");
				for (int i = 1; i < headPal.Count; i++)
				{
					Console.WriteLine($"WARNING: NES palette {i}: ${headPal[i],2:X}");
				}
				foreach (var i in headIndex)
				{
					int c = firstUnique[i.Key];
					Console.WriteLine($"WARNING: RGB to index {i.Key}: {i.Value}  first appears at {c >> 16}, {c & 0xFFFF}");
				}
				return;
			}

			var bodyColors = new List<Rgba32>();
			firstUnique = new Dictionary<Rgba32, int>();
			for (int y = top + 8; y < (top + 16); y++)
			{
				for (int x = left; x < (left + 64); x++)
				{
					if (!bodyColors.Contains(image[x, y]))
					{
						firstUnique[image[x, y]] = (x << 16 | y);
						bodyColors.Add(image[x, y]);
					}
				}
			}

			List<byte> bodyPal;
			Dictionary<Rgba32, byte> bodyIndex;
			if (!makeMapmanPalette(bodyColors, NESpalette, out bodyPal, out bodyIndex))
			{
				Console.WriteLine($"WARNING: Failed importing bottom half of mapman for {ClassNames[cur_class]}, too many unique colors (limit 3 unique colors + magenta for transparent):",
					  1 + bodyPal.Count + bodyIndex.Count);
				for (int i = 1; i < bodyPal.Count; i++)
				{
					Console.WriteLine($"WARNING: NES palette {i}: ${bodyPal[i],2:X}");
				}
				foreach (var i in bodyIndex)
				{
					int c = firstUnique[i.Key];
					Console.WriteLine($"WARNING: RGB to index {i.Key}: {i.Value}  first appears at {c >> 16}, {c & 0xFFFF}");
				}
				return;
			}

			for (int mapmanPos = 0; mapmanPos < 4; mapmanPos++)
			{
				var newleft = left + (mapmanPos * 16);

				var headTileLeft = makeTile(image, top, newleft, headIndex);
				var headTileRight = makeTile(image, top, newleft + 8, headIndex);

				var bodyTileLeft = makeTile(image, top + 8, newleft, bodyIndex);
				var bodyTileRight = makeTile(image, top + 8, newleft + 8, bodyIndex);

				Put(MAPMANGRAPHIC_OFFSET + (cur_class << 8) + (mapmanPos * 16 * 4) + (16 * 0), EncodeForPPU(headTileLeft));
				Put(MAPMANGRAPHIC_OFFSET + (cur_class << 8) + (mapmanPos * 16 * 4) + (16 * 1), EncodeForPPU(headTileRight));
				Put(MAPMANGRAPHIC_OFFSET + (cur_class << 8) + (mapmanPos * 16 * 4) + (16 * 2), EncodeForPPU(bodyTileLeft));
				Put(MAPMANGRAPHIC_OFFSET + (cur_class << 8) + (mapmanPos * 16 * 4) + (16 * 3), EncodeForPPU(bodyTileRight));
			}

			int offsetIntoLut = cur_class << 3;
			// Write the palettes into a new LUT in bank $0F
			// Will be read using the code below.
			//Console.WriteLine($"writing to {lut_MapmanPalettes + offsetIntoLut} {lut_MapmanPalettes + offsetIntoLut + 4}");
			PutInBank(0x0F, MAPMANGRAPHIC_OFFSET + offsetIntoLut, headPal.ToArray());
			PutInBank(0x0F, MAPMANGRAPHIC_OFFSET + offsetIntoLut + 4, bodyPal.ToArray());
		}

		public void ImportBattleSpriteSync(Image<Rgba32> image, int cur_class, int top, int left, Rgba32[] NESpalette)
		{
			int pal = -1;
			Dictionary<byte, byte> index = new Dictionary<byte, byte>();

			for (int y = top; y < (top + 24); y++)
			{
				for (int x = left; x < (left + 104); x++)
				{
					if (x >= (left + 80) && y < (top + 8))
					{
						// This is the space above the dead character,
						// ignore it
						continue;
					}

					if (pal ==-1 && (image[x, y] == NESpalette[48] || image[x, y] == NESpalette[22]))  // White or Red
					{
						pal = 1;
						// RW = 0F, 16, 30, 36
						index.Add(0x0F, 0);
						index.Add(0x16, 1);
						index.Add(0x30, 2);
						index.Add(0x36, 3);
						break;
					}
					else if (pal == -1 && (image[x, y] == NESpalette[33] || image[x, y] == NESpalette[40])) // Blue or Yellow
					{
						pal = 0;
						// YB = 0F, 28, 18, 21
						index.Add(0x0F, 0);
						index.Add(0x28, 1);
						index.Add(0x18, 2);
						index.Add(0x21, 3);
						break;
					}
				}
			}

			if (pal == -1)
			{
				Console.WriteLine($"WARNING: Failed importing battle sprites for {ClassNames[cur_class]}, does not belong in base palette sets):");
				return;
			}

			var ConstPicFormationNoDup = new byte[39] {	//3 x 13 pic formation
				0, 1, 255, 255,  8, 9,14,15,20,21,255,255,255,
				2, 3, 255, 255, 10,11,16,17,22,23,26,27,28,
				4, 5,   6,   7, 12,13,18,19,24,25,29,30,31};

			for (int y = 0; y < 3; y++)
			{
				for (int x = 0; x < 13; x++)
				{
					var tileidx = ConstPicFormationNoDup[(y * 13) + x];
					if (tileidx == 255)
					{
						continue;
					}
					var tile = makeTile(image, top + (y * 8), left + (x * 8), index);
					Put(CHARBATTLEPIC_OFFSET + (cur_class << 9) + (tileidx * 16), EncodeForPPU(tile));
				}
			}

			// Set the palette to use for each class, this
			// one is used for character selection and
			// subscreen
			PutInBank(0x1F, 0xECA4 + cur_class, new byte[] { (byte)pal });

			// it loads this one in battle, it is redundant with the one
			// in 1F but we have to set it anyway
			PutInBank(0x0C, 0xA03C + cur_class, new byte[] { (byte)pal });
		}




    }
}