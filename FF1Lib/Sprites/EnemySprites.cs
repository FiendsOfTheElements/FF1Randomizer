using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using SixLabors.ImageSharp.ColorSpaces;
using System.Dynamic;

namespace FF1Lib
{
	

	public partial class FF1Rom : NesRom
    {

		const int ENEMYGRAPHIC_OFFSET = 0x1C120;
		const int FIENDDRAW_TABLE = 0x2D2E0;
		const int CHAOSDRAW_TABLE = 0x2D420;
		const int BATTLEPALETTE_OFFSET = 0x30F20;

		// enum for the 52 normal enemy sprites, in order as they appear in the rom.
		// the name is the associated enemy with the lowest ID.
		// There might be a good reason to make this an abstract class with int members
		// to make it consistent with the Enemy class, especially since we're actually
		// using the integers to index arrays.
		public enum EnemySprite : int
		{
			Imp	  	 = 0,
			Wolf	 = 1,
			Iguana	 = 2,
			Giant	 = 3,
			Sahag	 = 4,
			Pirate	 = 5,
			Shark	 = 6,
			OddEye	 = 7,
			Bone	 = 8,
			Creep	 = 9,
			Hyena	 = 10,
			Ogre 	 = 11,
			Asp 	 = 12,
			Scorpion = 13,
			Bull 	 = 14,
			Troll	 = 15,
			Shadow   = 16,
			Zombie   = 17,
			Worm 	 = 18,
			Eye 	 = 19,
			Medusa	 = 20,
			Catman   = 21,
			Pede 	 = 22,
			Tiger 	 = 23,
			Vampire  = 24,
			Gargoyle = 25,
			Earth 	 = 26,
			FrostD   = 27,
			Scum 	 = 28,
			Spider	 = 29,
			Manticor = 30,
			RAnkylo  = 31,
			Mummy	 = 32,
			Coctrice = 33,
			Wyvern   = 34,
			Tyro 	 = 35,
			Caribe   = 36,
			Gator 	 = 37,
			Ocho 	 = 38,
			Hydra	 = 39,
			Guard    = 40,
			Water 	 = 41,
			Naga 	 = 42,
			Chimera  = 43,
			Wizard   = 44,
			Garland  = 45,
			GasD 	 = 46,
			MudGol   = 47,
			Badman   = 48,
			Astos 	 = 49,
			Madpony  = 50,
			WarMech  = 51
		}

		// dictionary mapping enemy ID to enemy sprite number in vanilla
		// LATER decide how this should work with Enemizer / new formations.
		// Also decide whether writing it out explicitly is worth the space,
		// since this could also be imported from the rom.
		Dictionary<int, EnemySprite> enemySpriteMap = new Dictionary<int, EnemySprite>
		{
			{Enemy.Imp, 	 EnemySprite.Imp},
			{Enemy.GrImp, 	 EnemySprite.Imp},
			{Enemy.Wolf, 	 EnemySprite.Wolf},
			{Enemy.GrWolf, 	 EnemySprite.Wolf},
			{Enemy.WrWolf, 	 EnemySprite.Wolf},
			{Enemy.FrWolf, 	 EnemySprite.Wolf},
			{Enemy.Iguana, 	 EnemySprite.Iguana},
			{Enemy.Agama, 	 EnemySprite.Iguana},
			{Enemy.Sauria, 	 EnemySprite.Iguana},
			{Enemy.Giant, 	 EnemySprite.Giant},
			{Enemy.FrGiant,  EnemySprite.Giant},
			{Enemy.RGiant, 	 EnemySprite.Giant},
			{Enemy.Sahag, 	 EnemySprite.Sahag},
			{Enemy.RSahag, 	 EnemySprite.Sahag},
			{Enemy.WzSahag,  EnemySprite.Sahag},
			{Enemy.Pirate, 	 EnemySprite.Pirate},
			{Enemy.Kyzoku, 	 EnemySprite.Pirate},
			{Enemy.Shark, 	 EnemySprite.Shark},
			{Enemy.GrShark,  EnemySprite.Shark},
			{Enemy.OddEye, 	 EnemySprite.OddEye},
			{Enemy.BigEye, 	 EnemySprite.OddEye},
			{Enemy.Bone, 	 EnemySprite.Bone},
			{Enemy.RBone, 	 EnemySprite.Bone},
			{Enemy.Creep, 	 EnemySprite.Creep},
			{Enemy.Crawl, 	 EnemySprite.Creep},
			{Enemy.Hyena, 	 EnemySprite.Hyena},
			{Enemy.Cerebus,  EnemySprite.Hyena},
			{Enemy.Ogre, 	 EnemySprite.Ogre},
			{Enemy.GrOgre, 	 EnemySprite.Ogre},
			{Enemy.WzOgre, 	 EnemySprite.Ogre},
			{Enemy.Asp, 	 EnemySprite.Asp},
			{Enemy.Cobra, 	 EnemySprite.Asp},
			{Enemy.SeaSnake, EnemySprite.Asp},
			{Enemy.Scorpion, EnemySprite.Scorpion},
			{Enemy.Lobster,  EnemySprite.Scorpion},
			{Enemy.Bull, 	 EnemySprite.Bull},
			{Enemy.ZomBull,  EnemySprite.Bull},
			{Enemy.Troll, 	 EnemySprite.Troll},
			{Enemy.SeaTroll, EnemySprite.Troll},
			{Enemy.Shadow, 	 EnemySprite.Shadow},
			{Enemy.Image, 	 EnemySprite.Shadow},
			{Enemy.Wraith, 	 EnemySprite.Shadow},
			{Enemy.Ghost, 	 EnemySprite.Shadow},
			{Enemy.Zombie, 	 EnemySprite.Zombie},
			{Enemy.Ghoul, 	 EnemySprite.Zombie},
			{Enemy.Geist, 	 EnemySprite.Zombie},
			{Enemy.Specter,  EnemySprite.Zombie},
			{Enemy.Worm, 	 EnemySprite.Worm},
			{Enemy.SandW, 	 EnemySprite.Worm},
			{Enemy.GreyW, 	 EnemySprite.Worm},
			{Enemy.Eye, 	 EnemySprite.Eye},
			{Enemy.Phantom,  EnemySprite.Eye},
			{Enemy.Medusa, 	 EnemySprite.Medusa},
			{Enemy.GrMedusa, EnemySprite.Medusa},
			{Enemy.Catman, 	 EnemySprite.Catman},
			{Enemy.Mancat, 	 EnemySprite.Catman},
			{Enemy.Pede, 	 EnemySprite.Pede},
			{Enemy.GrPede, 	 EnemySprite.Pede},
			{Enemy.Tiger, 	 EnemySprite.Tiger},
			{Enemy.SaberT, 	 EnemySprite.Tiger},
			{Enemy.Vampire,  EnemySprite.Vampire},
			{Enemy.WzVamp, 	 EnemySprite.Vampire},
			{Enemy.Gargoyle, EnemySprite.Gargoyle},
			{Enemy.RGoyle, 	 EnemySprite.Gargoyle},
			{Enemy.Earth, 	 EnemySprite.Earth},
			{Enemy.Fire, 	 EnemySprite.Earth},
			{Enemy.FrostD, 	 EnemySprite.FrostD},
			{Enemy.RedD, 	 EnemySprite.FrostD},
			{Enemy.ZombieD,  EnemySprite.FrostD},
			{Enemy.Scum, 	 EnemySprite.Scum},
			{Enemy.Muck, 	 EnemySprite.Scum},
			{Enemy.Ooze, 	 EnemySprite.Scum},
			{Enemy.Slime, 	 EnemySprite.Scum},
			{Enemy.Spider, 	 EnemySprite.Spider},
			{Enemy.Arachnid, EnemySprite.Spider},
			{Enemy.Manticor, EnemySprite.Manticor},
			{Enemy.Sphinx, 	 EnemySprite.Manticor},
			{Enemy.RAnkylo,  EnemySprite.RAnkylo},
			{Enemy.Ankylo, 	 EnemySprite.RAnkylo},
			{Enemy.Mummy, 	 EnemySprite.Mummy},
			{Enemy.WzMummy,  EnemySprite.Mummy},
			{Enemy.Coctrice, EnemySprite.Coctrice},
			{Enemy.Perilisk, EnemySprite.Coctrice},
			{Enemy.Wyvern, 	 EnemySprite.Wyvern},
			{Enemy.Wyrm, 	 EnemySprite.Wyvern},
			{Enemy.Tyro, 	 EnemySprite.Tyro},
			{Enemy.TRex, 	 EnemySprite.Tyro},
			{Enemy.Caribe, 	 EnemySprite.Caribe},
			{Enemy.RCaribe,  EnemySprite.Caribe},
			{Enemy.Gator, 	 EnemySprite.Gator},
			{Enemy.FrGator,  EnemySprite.Gator},
			{Enemy.Ocho, 	 EnemySprite.Ocho},
			{Enemy.Naocho, 	 EnemySprite.Ocho},
			{Enemy.Hydra, 	 EnemySprite.Hydra},
			{Enemy.RHydra, 	 EnemySprite.Hydra},
			{Enemy.Guard, 	 EnemySprite.Guard},
			{Enemy.Sentry, 	 EnemySprite.Guard},
			{Enemy.Water, 	 EnemySprite.Water},
			{Enemy.Air, 	 EnemySprite.Water},
			{Enemy.Naga, 	 EnemySprite.Naga},
			{Enemy.GrNaga, 	 EnemySprite.Naga},
			{Enemy.Chimera,  EnemySprite.Chimera},
			{Enemy.Jimera, 	 EnemySprite.Chimera},
			{Enemy.Wizard, 	 EnemySprite.Wizard},
			{Enemy.Sorcerer, EnemySprite.Wizard},
			{Enemy.Garland,  EnemySprite.Garland},
			{Enemy.GasD, 	 EnemySprite.GasD},
			{Enemy.BlueD, 	 EnemySprite.GasD},
			{Enemy.MudGol, 	 EnemySprite.MudGol},
			{Enemy.RockGol,  EnemySprite.MudGol},
			{Enemy.IronGol,  EnemySprite.MudGol},
			{Enemy.Badman, 	 EnemySprite.Badman},
			{Enemy.Evilman,  EnemySprite.Badman},
			{Enemy.Astos, 	 EnemySprite.Astos},
			{Enemy.Mage, 	 EnemySprite.Astos},
			{Enemy.Fighter,  EnemySprite.Astos},
			{Enemy.Madpony,  EnemySprite.Madpony},
			{Enemy.Nitemare, EnemySprite.Madpony},
			{Enemy.WarMech,  EnemySprite.WarMech}
		};

		public class EnemyPatternData
		{
			/*
				Container class for enemy sprite pattern data.
				This stores each of the enemy sprites (excluding the fiends and chaos) in an internal array of byte[] arrays.
				Routines that modify enemy sprites should now read from and write to this container instead of
				directly to the rom. The sprites are indexed by their order in the rom; in addition, we can access
				individual sprites by name using the EnemySprite enum.

				Enemy sprites are stored in "sheets" of 2 small and 2 large sprites,
				interleaved with battle background tiles (and some tiles of Japanese Hiragama
				characters を っ ゃ ゅ ょ "wo" "sokuon" "ya" "yu" "yo" which appear in several places
				throughout the rom, which the randomizer replaces with other tiles, for instance the "+"
				character used in the names of weapons or armor that have rolled up). 
				The palette data is stored elsewhere and is applied per formation/enemy.

				13 sheets with 4 sprites per sheet makes 52 sprites total.
				Small sprites are 32x32 pixels, comprising 16 8x8 16-byte tiles (256 == 0x100 bytes per sprite)
				Large sprites are 48x48 pixels, comprising 36 8x8 16-byte tiles (576 == 0x240 bytes per sprite)
				Total sheet size is 1664 = 0x680 bytes
				The skip from one sheet to the next is 2048 = 0x800 bytes.

				For efficiency reasons, the game orders these sprites differently in the game programming,
				using a LUT to map the game programming ordering to the sprite ordering as stored in the ROM.
				The sprite ordering per sheet as respected in this container is:
				  0: Small1
				  1: Small2
				  2: Large1
				  3: Large2
				In the game, however, this is mapped so that small sprites are even numbers and large sprites are odd:
				  0: Small1
				  1: Large1
				  2: Small2
				  3: Large2
				
				This class has GetLUT() and SetLUT() methods to access via the LUT indices rather than ROM ordering indices.

			*/
			//const int ENEMYGRAPHIC_OFFSET = 0x1C120;
			// private const int bank = 0x07;
			// Since enemy sprite data spans two banks in the ROM, we use raw ROM addresses
			private const int mainOffset = 0x1C120;
			//small enemy sprite size in bytes (16 8x8 tiles)
			private const int smallSize = 0x100;
			//large enemy sprite size in bytes (36 8x8 tiles)
			private const int largeSize = 0x240;
			private const int sheetSize = 0x680;
			private const int totalSprites = 52;
			private const int totalSheets = 13;
			private const int spritesPerSheet = 4;
			private const int sheetSkip = 0x800;
			private readonly int[] sizes = new int[]{smallSize,smallSize,largeSize,largeSize};
			private readonly int[] offsets = new int[]{0, smallSize, smallSize*2, smallSize*2 + largeSize};

			private FF1Rom rom;
			private byte[][] _enemySpriteData;



			private int LUTtoIndex(int LUTindex)
			{
				int masked = LUTindex & 0b11111100;
				int index = (LUTindex % 4) switch
				{
					0 => masked,
					2 => masked + 1,
					1 => masked + 2,
					3 => masked + 3,
					_ => -1 
				};
				//Console.WriteLine($"LUTindex = {LUTindex}, mapped index = {index}");
				return index;
			}

			private (int, int) GetOffsetAndSize(int spriteNumber)
			{
				
				int sheet = spriteNumber / spritesPerSheet;
				int sprite = spriteNumber % spritesPerSheet;

				
				int offset = mainOffset + sheet*sheetSkip + offsets[sprite];
				int size = sizes[sprite];

				return (offset, size);
			}

			// public EnemyPatternData()
			// {
			// 	_enemySpriteData = new byte[totalSprites][];
			// }

			public EnemyPatternData(FF1Rom rom)
			{
				this.rom = rom;
				_enemySpriteData = new byte[totalSprites][];
				for (int i = 0; i < totalSprites; i++)
				{
					(int offset, int size) = GetOffsetAndSize(i);
					//Console.WriteLine($"Sprite = {i}, Offset = {offset:X}, Size = {size:X}");
					_enemySpriteData[i] = rom.Get(offset, size);
				}
			}

			public byte[] this[int index]
			{
				get => _enemySpriteData[index];
				//set => _enemySpriteData[index] = value;
				set
				{
					if (value.Length == _enemySpriteData[index].Length)
					{
						_enemySpriteData[index] = value;
					}
					else
					{
						throw new IndexOutOfRangeException("Enemy sprite set operation failed: unexpected length.");
					}
				}
			}

			// use EnemySprite enum as indexer
			public byte[] this[EnemySprite index] => this[(int)index];

			// Get and Set methods using the interleaved image LUT values (see comments at the beginning of the class)
			public byte[] GetLUT(int LUTindex) => this[LUTtoIndex(LUTindex)];

			public void SetLUT(int LUTindex, byte[] value) => this[LUTtoIndex(LUTindex)] = value;
			

			public byte[][] ToArray() => (byte[][])_enemySpriteData.Clone();
			

			public List<byte[]> ToList()
			{
				return _enemySpriteData.ToList<byte[]>();
			}

			public void CopyFrom(byte[][] newData, int index = 0)
			{
				// do some bound and size checking before altering data -- maybe not necessary
				// if we're going to throw an Exception anyway...
				bool good = true;
				for (int i = 0; good & i < newData.Length; i++)
				{
					if (newData[i].Length != _enemySpriteData[index + i].Length)
					{
						good = false;
					}
				}
				if (good)
				{
					for (int i = 0; i < _enemySpriteData.Length; i++)
					{
						_enemySpriteData[index + i] = newData[i];
					}
				}
				else
				{
					throw new Exception("Enemy sprite CopyFrom operation failed: one or more values are the wrong size.");
				}
			}

			public void Write()
			{
				for (int i = 0; i < totalSprites; i++)
				{
					(int offset, int _) = GetOffsetAndSize(i);
					this.rom.Put(offset,_enemySpriteData[i]);
				}
				return;
			}

			// Import an image; We could care more whether the dimensions are correct, but since this will mostly not be a user feature,
			// we can afford to just check whether the byte[] is the right length to replace the target enemy sprite data, and assume
			// that we know what we're doing.
			// Default behavior is to import the entire image, using the GrayscaleIndex in Sprites.cs. Grayscale is necessary because
			// palettes are assigned in formations, not in the pattern data.
			public void ImportImage(Image<Rgba32> image, EnemySprite enemy, int top = 0, int left = 0, int heightInTiles = 0, int widthInTiles = 0)
			{
				
				int index = (int)enemy;
				byte[] data = rom.MakePatternData(image, rom.GrayscaleIndex, top, left, widthInTiles, heightInTiles);
				if (data.Length == _enemySpriteData[index].Length)
				{
					_enemySpriteData[index] = data;
				}
				else
				{
					throw new Exception("Enemy sprite image import failed: unexpected size.");
				}

			}

			// get the data for the 4 sprites in a given sheet. We could also have one that returns a flat byte[] with
			// all of this data: basically data for contiguous tiles, ready to be written into the rom. Can't really think
			// of a use for this right now because we want to read/write to the rom only from this container.
			public byte[][] GetSheet(int sheetIndex)
			{
				
				return _enemySpriteData.AsSpan().Slice(sheetIndex * spritesPerSheet, spritesPerSheet).ToArray();
			}

			
		}


		public void ImportFunEnemyImage(EnemySprite sprite)
		{
			string spriteFile = $"{(int)sprite}_{sprite}.png";
			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var path = assembly.GetManifestResourceNames().First(str => str.EndsWith(spriteFile));
			var stream = assembly.GetManifestResourceStream(path);

			EnemySprites.ImportImage(Image.Load<Rgba32>(stream), sprite);
		}

		// This will eventually be merged into Fun Enemies along with Team Steak
        public void SetRobotChickenGraphics(Preferences preferences)
		{
			if (preferences.RobotChicken)
			{
				//const int WARMECHGRAPHIC_OFFSET = 0x22560;
				// var assembly = System.Reflection.Assembly.GetExecutingAssembly();
				// var robochknPath = assembly.GetManifestResourceNames().First(str => str.EndsWith("51_WarMech.png"));
				// var robochknStream = assembly.GetManifestResourceStream(robochknPath);

				// Image<Rgba32> image = Image.Load<Rgba32>(robochknStream);

				// EnemySprites.ImportImage(image, EnemySprite.WarMech);
				ImportFunEnemyImage(EnemySprite.WarMech);

			}
		}


		public void SetTeamSteakGraphics(SteakSprite teamSteak)
		{
			// NOTE: this is all going to change shortly, so there are some basic hacks here to let team steak play with the
			// EnemyPatternData class.
			// don't need to check team STEAK preferences because this is called in Fun.cs where that check has already been done
			// arrays contain offset into rom, sprite size in pixels and x,y coords for location of sprite in spritesheet

			var SteakSpriteList = new List<EnemySprite> {};

			if (teamSteak == SteakSprite.Steak || teamSteak == SteakSprite.All)
			{
				SteakSpriteList.Add(EnemySprite.Wyvern);
				SteakSpriteList.Add(EnemySprite.Tyro);
			}
			if (teamSteak == SteakSprite.Sandwiches || teamSteak == SteakSprite.All)
			{
				SteakSpriteList.Add(EnemySprite.Worm);
			}
			if (teamSteak == SteakSprite.Nachos || teamSteak == SteakSprite.All)
			{
				SteakSpriteList.Add(EnemySprite.Ocho);
			}
			if (teamSteak == SteakSprite.Pastries || teamSteak == SteakSprite.All)
			{
				SteakSpriteList.Add(EnemySprite.Creep);
			}

			foreach (EnemySprite sprite in SteakSpriteList)
			{
				ImportFunEnemyImage(sprite);
			}
		}
		
		public void SetCustomEnemyGraphics(Stream stream) {
			Image<Rgba32> image = Image.Load<Rgba32>(stream);


			// ep for "enemyPack"
			// 13 enemy packs total
			// process each enemy pack one by one
			for (int ep = 0; ep < 13; ep++)
			{
				// epx and epy are the x and y coordinates of successive enemy packs.
				int epx = (ep % 4) * 96;
				int epy = (ep / 4) * 80;

				// each enemy pack contains two 32x32 sprites and 2 48x48 sprites.

				int[] enemyDims = {4,4,6,6}; // dimensions in tiles
				int[] xCoord = {0, 32, 0, 48};
				int[] yCoord = {0, 0, 32, 32};


				// e is the enemy iterator
				// process one enemy in the pack at a time
				for (int e = 0; e < 4; e++)
				{

					int x = epx + xCoord[e];
					int y = epy + yCoord[e];
					int size = enemyDims[e];
					int index = ep * 4 + e;
					EnemySprites.ImportImage(image,(EnemySprite)index, y, x, size, size);
				}
			}
		}


		// LATER: the fiend/chaos sprite stuff should probably also work with a container class, but it's not pressing.
		// right now alt fiends probably doesn't play well with resource packs, and there could be problems with
		// planned fun fiend sprites as well. More important to take care with flag interactions and to be sure
		//  things run in a logical order in Randomize.cs
        public void FiendImport(Image<Rgba32> image, int sizeX, int sizeY,
				    int imageOffsetX, int imageOffsetY, List<byte[]> chrEntries,
				    int nametableDest, int paletteDest1, int paletteDest2) {

			List<List<byte>> candidatePals = new List<List<byte>>();
			var toNEScolor = new Dictionary<Rgba32, byte>();

			for(int areaY = 0; areaY < sizeY/2; areaY += 1)
			{
				for(int areaX = 0; areaX < sizeX/2; areaX += 1)
				{
					int top = imageOffsetY + areaY * 16;
					int left = imageOffsetX + areaX * 16;

					//Console.WriteLine($"area {areaX} {areaY}    pixel {left} {top}   candpal {candidatePals.Count}");

					var firstUnique = new Dictionary<Rgba32, int>();
					var colors = new List<Rgba32>();
					for (int y = top; y < (top+16); y++)
					{
						for (int x = left; x < (left+16); x++)
						{
							if (!colors.Contains(image[x,y]))
							{
								firstUnique[image[x,y]] = (x<<16 | y);
								colors.Add(image[x,y]);
							}
						}
					}
					List<byte> pal;
					if (!makeNTPalette(colors, NESpalette, out pal, toNEScolor)) {
						//await this.Progress($"WARNING: Failed importing fiend at {left}, {top}, too many unique colors (limit 4 unique colors):", 1+pal.Count);
						Console.WriteLine($"WARNING: Failed importing fiend at {left}, {top}, too many unique colors (limit 4 unique colors):", 1 + pal.Count);
						for (int i = 0; i < pal.Count; i++)
						{
							//await this.Progress($"WARNING: NES palette {i}: ${pal[i],2:X}");
							Console.WriteLine($"WARNING: NES palette {i}: ${pal[i],2:X}");
						}
						return;
					}
					candidatePals.Add(pal);
				}
			}

			var fiendPals = new List<List<byte>>();
			if (!mergePalettes(candidatePals, fiendPals, 2))
			{
				//await this.Progress($"WARNING: Too many unique 4-color palettes");
				Console.WriteLine($"WARNING: Too many unique 4-color palettes");
			}

			byte[] blankTile = new byte[16] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
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
			for(int areaY = 0; areaY < sizeY/2; areaY += 1)
			{
				for(int areaX = 0; areaX < sizeX/2; areaX += 1)
				{
					var srcPalIdx = areaY * (sizeX/2) + areaX;

					//Console.WriteLine($"area {areaX} {areaY}       candpal {srcPalIdx}");

					int palidx = findPalette(fiendPals, candidatePals[srcPalIdx]);
					if  (palidx == -1)
					{
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
					if (sizeX == 8)
					{
						aX = areaX + 2;
						aY = areaY + 2;
					}
					else
					{
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
					for(int tilesY = areaY*2; tilesY < (areaY+1)*2; tilesY += 1)
					{
						for(int tilesX = areaX*2; tilesX < (areaX+1)*2; tilesX += 1)
						{
							int top = imageOffsetY + tilesY * 8;
							int left = imageOffsetX + tilesX * 8;
							byte[] tile = makeTile(image,top,left,index);
							byte idx;
							// tile 0 -- just before the battle background tiles -- is always blank; 
							// vanilla uses it to draw blank tiles in fiend and chaos sprites.
							// We want to do the same thing so that the blank tile doesn't take up space
							// in our chrEntries.
							if (Enumerable.SequenceEqual(EncodeForPPU(tile),blankTile))
							{
								idx = 0;
							}
							else 
							{
								idx = (byte)(chrIndex(tile, chrEntries, 110) + 18);
							}
							if (idx == 0xff)
							{
								//await this.Progress($"WARNING: Error importing CHR at {left}, {top}, too many unique CHR ");
								idx = 0;
							}
							// put the NT
							nametable[tilesY*sizeX + tilesX] = idx;
						}
					}
				}
			}
			Put(nametableDest, nametable);
			Put(nametableDest+nametable.Length, attributeTable);
			Put(paletteDest1, fiendPals[0].ToArray());
			if (fiendPals.Count == 2)
			{
				Put(paletteDest2, fiendPals[1].ToArray());
			}
	    }

	    const int TIAMAT1 = 0x77;
	    const int KRAKEN1 = 0x78;
	    const int KARY1 = 0x79;
	    const int LICH1 = 0x7A;
		const int BATTLEPATTERNTABLE_OFFSET = 0x1C000;

	    //public async Task<bool> SetLichKaryGraphics(Stream lich, Stream kary) {
		public bool SetLichKaryGraphics(Stream lich, Stream kary) {
			var formations = LoadFormations();
			//IImageFormat format;
			Image<Rgba32> lichImage = Image.Load<Rgba32>(lich);
			Image<Rgba32> karyImage = Image.Load<Rgba32>(kary);
			List<byte[]> CHR = new List<byte[]>();
			/*
			await FiendImport(lichImage, 8, 8,  0, 0, CHR, FIENDDRAW_TABLE + (0x50 * 1), BATTLEPALETTE_OFFSET+(formations[LICH1].pal1 * 4), BATTLEPALETTE_OFFSET+(formations[LICH1].pal2 * 4));
			await FiendImport(karyImage, 8, 8, 0, 0, CHR, FIENDDRAW_TABLE + (0x50 * 0), BATTLEPALETTE_OFFSET+(formations[KARY1].pal1 * 4), BATTLEPALETTE_OFFSET+(formations[KARY1].pal2 * 4));
			*/
			FiendImport(lichImage, 8, 8,  0, 0, CHR, FIENDDRAW_TABLE + (0x50 * 1), BATTLEPALETTE_OFFSET+(formations[LICH1].pal1 * 4), BATTLEPALETTE_OFFSET+(formations[LICH1].pal2 * 4));
			FiendImport(karyImage, 8, 8, 0, 0, CHR, FIENDDRAW_TABLE + (0x50 * 0), BATTLEPALETTE_OFFSET+(formations[KARY1].pal1 * 4), BATTLEPALETTE_OFFSET+(formations[KARY1].pal2 * 4));
			// Strictly speaking, the max CHR count should be 106,
			// so that it doesn't clobber the "+" tile we use in weapon and armor names.
			// We could rotate the locations of these tiles to provide a little more room if needed...
			if (CHR.Count < 110)
			{
				int offset = BATTLEPATTERNTABLE_OFFSET + (formations[LICH1].tileset * 2048) + (18 * 16);
				for (int i = 0; i < CHR.Count; i++)
				{
					Put(offset + (i * 16), EncodeForPPU(CHR[i]));
				}
				return true;
			}
			else
			{
				return false;
			}
	    }

	    //public async Task<bool> SetKrakenTiamatGraphics(Stream kraken, Stream tiamat) {
		public bool SetKrakenTiamatGraphics(Stream kraken, Stream tiamat) {
			var formations = LoadFormations();
			//IImageFormat format;
			Image<Rgba32> krakenImage = Image.Load<Rgba32>(kraken);
			Image<Rgba32> tiamatImage = Image.Load<Rgba32>(tiamat);
			List<byte[]> CHR = new List<byte[]>();
			/*
			await FiendImport(krakenImage, 8, 8,  0, 0, CHR, FIENDDRAW_TABLE + (0x50 * 2), BATTLEPALETTE_OFFSET+(formations[KRAKEN1].pal1 * 4), BATTLEPALETTE_OFFSET+(formations[KRAKEN1].pal2 * 4));
			await FiendImport(tiamatImage, 8, 8,  0, 0, CHR, FIENDDRAW_TABLE + (0x50 * 3), BATTLEPALETTE_OFFSET+(formations[TIAMAT1].pal1 * 4), BATTLEPALETTE_OFFSET+(formations[TIAMAT1].pal2 * 4));
			*/
			FiendImport(krakenImage, 8, 8, 0, 0, CHR, FIENDDRAW_TABLE + (0x50 * 2), BATTLEPALETTE_OFFSET + (formations[KRAKEN1].pal1 * 4), BATTLEPALETTE_OFFSET + (formations[KRAKEN1].pal2 * 4));
			FiendImport(tiamatImage, 8, 8, 0, 0, CHR, FIENDDRAW_TABLE + (0x50 * 3), BATTLEPALETTE_OFFSET + (formations[TIAMAT1].pal1 * 4), BATTLEPALETTE_OFFSET + (formations[TIAMAT1].pal2 * 4));
			if (CHR.Count < 110)
			{
				int offset = BATTLEPATTERNTABLE_OFFSET + (formations[KRAKEN1].tileset * 2048) + (18 * 16);
				for (int i = 0; i < CHR.Count; i++)
				{
					Put(offset + (i*16), EncodeForPPU(CHR[i]));
				}
				return true;
			}
			else
			{
				return false;
			}
	    }

	    public async Task SetCustomFiendGraphics(Stream fiends) {
			// 0B_92E0
			//    $50 bytes of TSA for all 4 fiend graphics (resulting in $140 bytes of data total)
			//      $40 bytes of NT TSA (8x8 image)
			//      $10 bytes of attributes  (4x4)

			// 0B_9420
			//    $C0 bytes of TSA for chaos
			//     $A8 bytes of NT TSA (14x12 image)
			//     $10 bytes of attributes  (4x4x)

			var formations = LoadFormations();

			//IImageFormat format;

			Image<Rgba32> image = Image.Load<Rgba32>(fiends);
			{
				List<byte[]> CHR = new List<byte[]>();
					/*
				await FiendImport(image, 8, 8,  0, 0, CHR, FIENDDRAW_TABLE + (0x50 * 1), BATTLEPALETTE_OFFSET+(formations[LICH1].pal1 * 4), BATTLEPALETTE_OFFSET+(formations[LICH1].pal2 * 4));
				await FiendImport(image, 8, 8, 64, 0, CHR, FIENDDRAW_TABLE + (0x50 * 0), BATTLEPALETTE_OFFSET+(formations[KARY1].pal1 * 4), BATTLEPALETTE_OFFSET+(formations[KARY1].pal2 * 4));
					*/
				FiendImport(image, 8, 8, 0, 0, CHR, FIENDDRAW_TABLE + (0x50 * 1), BATTLEPALETTE_OFFSET + (formations[LICH1].pal1 * 4), BATTLEPALETTE_OFFSET + (formations[LICH1].pal2 * 4));
				FiendImport(image, 8, 8, 64, 0, CHR, FIENDDRAW_TABLE + (0x50 * 0), BATTLEPALETTE_OFFSET + (formations[KARY1].pal1 * 4), BATTLEPALETTE_OFFSET + (formations[KARY1].pal2 * 4));
				if (CHR.Count < 110)
				{
					int offset = BATTLEPATTERNTABLE_OFFSET + (formations[LICH1].tileset * 2048) + (18 * 16);
					for (int i = 0; i < CHR.Count; i++)
					{
						Put(offset + (i*16), EncodeForPPU(CHR[i]));
					}
				}
				else
				{
					await this.Progress($"WARNING: Error importing Lich and Kary, too many unique CHR ({CHR.Count}), must be less than 110 unique 8x8 tiles between both fiends");
				}
			}
			{
				List<byte[]> CHR = new List<byte[]>();
				/*
				await FiendImport(image, 8, 8,  0, 64, CHR, FIENDDRAW_TABLE + (0x50 * 2), BATTLEPALETTE_OFFSET+(formations[KRAKEN1].pal1 * 4), BATTLEPALETTE_OFFSET+(formations[KRAKEN1].pal2 * 4));
				await FiendImport(image, 8, 8, 64, 64, CHR, FIENDDRAW_TABLE + (0x50 * 3), BATTLEPALETTE_OFFSET+(formations[TIAMAT1].pal1 * 4), BATTLEPALETTE_OFFSET+(formations[TIAMAT1].pal2 * 4));
				*/
				FiendImport(image, 8, 8, 0, 64, CHR, FIENDDRAW_TABLE + (0x50 * 2), BATTLEPALETTE_OFFSET + (formations[KRAKEN1].pal1 * 4), BATTLEPALETTE_OFFSET + (formations[KRAKEN1].pal2 * 4));
				FiendImport(image, 8, 8, 64, 64, CHR, FIENDDRAW_TABLE + (0x50 * 3), BATTLEPALETTE_OFFSET + (formations[TIAMAT1].pal1 * 4), BATTLEPALETTE_OFFSET + (formations[TIAMAT1].pal2 * 4));
				if (CHR.Count < 110)
				{
					int offset = BATTLEPATTERNTABLE_OFFSET + (formations[KRAKEN1].tileset * 2048) + (18 * 16);
					for (int i = 0; i < CHR.Count; i++)
					{
						Put(offset + (i*16), EncodeForPPU(CHR[i]));
					}
				}
				else
				{
					await this.Progress($"WARNING: Error importing Kraken and Tiamat, too many unique CHR ({CHR.Count}), must be less than 110 unique 8x8 tiles between both fiends");
				}
			}
	    }

	    public async Task SetCustomChaosGraphics(Stream chaos) {
			var formations = LoadFormations();
			//IImageFormat format;
			const int CHAOS = 0x7B;
			Image<Rgba32> image = Image.Load<Rgba32>(chaos);
			List<byte[]> CHR = new List<byte[]>();
			//await FiendImport(image, 14, 12,  0, 0, CHR, CHAOSDRAW_TABLE, BATTLEPALETTE_OFFSET+(formations[CHAOS].pal1 * 4), BATTLEPALETTE_OFFSET+(formations[CHAOS].pal2 * 4));
			FiendImport(image, 14, 12,  0, 0, CHR, CHAOSDRAW_TABLE, BATTLEPALETTE_OFFSET+(formations[CHAOS].pal1 * 4), BATTLEPALETTE_OFFSET+(formations[CHAOS].pal2 * 4));
			if (CHR.Count < 110)
			{
				int offset = BATTLEPATTERNTABLE_OFFSET + (formations[CHAOS].tileset * 2048) + (18 * 16);
				for (int i = 0; i < CHR.Count; i++)
				{
					Put(offset + (i*16), EncodeForPPU(CHR[i]));
				}
			}
			else
			{
				await this.Progress($"WARNING: Error importing Chaos, too many unique CHR ({CHR.Count}), must be less than 110 unique 8x8 tiles");
			}
	    }

	    public async Task SetCustomBattleBackdrop(Stream backdrop) {
			//const int BATTLEBACKDROPASSIGNMENT_OFFSET = 0x3310;
			const int BATTLEBACKDROPPALETTE_OFFSET = 0x3200;

			//IImageFormat format;
			Image<Rgba32> image = Image.Load<Rgba32>(backdrop);
			if (image.Size.Width < 32 || image.Size.Height < 512)
			{
				await this.Progress($"WARNING: Invalid battle backdrops image size.  Expected 32x512, got {image.Size.Width}x{image.Size.Height}. Skipping import...");
				return;
			}
			var toNEScolor = new Dictionary<Rgba32, byte>();

			for (int count = 0; count < 16; count++)
			{
				Console.WriteLine($"Importing backdrop {count}");
				int top = count*32;
				int left = 0;

				var firstUnique = new Dictionary<Rgba32, int>();
				var colors = new List<Rgba32>();
				for (int y = top; y < (top+32); y++)
				{
					for (int x = left; x < (left+32); x++)
					{
						if (!colors.Contains(image[x,y]))
						{
							firstUnique[image[x,y]] = (x<<16 | y);
							colors.Add(image[x,y]);
						}
					}
				}
				List<byte> pal;
				if (!makeNTPalette(colors, NESpalette, out pal, toNEScolor))
				{
					await this.Progress($"WARNING: Failed importing battle backdrop at {left}, {top}, too many unique colors (limit 4 unique colors):", 1+pal.Count);
					for (int i = 0; i < pal.Count; i++)
					{
						await this.Progress($"WARNING: NES palette {i}: ${pal[i],2:X}");
					}
					continue;
				}

				Dictionary<Rgba32, byte> index;
				colorToPaletteIndex(pal, toNEScolor, out index);

				Put(BATTLEBACKDROPPALETTE_OFFSET + (count * 4), pal.ToArray());

				int n = 1;
				for (int y = top; y < (top+32); y += 8)
				{
					for (int x = left; x < (left+32); x += 8)
					{
						var tile = makeTile(image, y, x, index);
						Put(BATTLEPATTERNTABLE_OFFSET + (count * 2048) + (n*16), EncodeForPPU(tile));
						n++;
					}
				}

			}
	    }

        
    }
}