using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using RomUtilities;
using DotNetAsm;

namespace FF1Lib
{
	// ReSharper disable once InconsistentNaming
	public static class FF1Text
	{
		public enum Delimiter
		{
			Empty = 0x100,
			Null = 0x00,
			Segment = 0x01,
			Line = 0x05
		}
		public enum MenuString
		{
			None = 0,
			CurrentGold,
			ItemMagicWeaponArmorStatus,
			MenuItemTitle,
			EmptyInventory,
			UseLuteSuccess,
			UseLute,
			UseCrown,
			UseCrystal,
			UseHerb,
			UseKey,
			UseTNT,
			UseAdamant,
			UseSlab,
			UseRuby,
			UseRodSuccess,
			UseRod,
			UseFloaterSuccess,
			UseFloater,
			UseChime,
			UseTail,
			UseCube,
			UseBottleSuccess,
			UseBottle,
			UseOxyale,
			UseCanoe,
			UseTentSuccess,
			UseTent,
			UseCabinSuccess,
			UseCabin,
			UseHouseSuccess,
			UseHouse,
			PotionHeal,
			PotionPure,
			PotionSoft,
			StatusName,
			StatusClass,
			StatusLevel,
			StatusExp,
			StatusLeftPanel,
			StatusRightPanel,
			MenuMagicName,
			MenuMagicLevels,
			MagicCure,
			MagicHeal,
			MagicPure,
			MagicLife,
			MagicWarp,
			MagicSoft,
			MagicExit,
			MagicOutOfMP,
			MagicCantUseHere,
			MenuWeaponTitle,
			MenuEquipTradeDrop,
			MenuEquipmentName1,
			Unknown55,
			MenuEquipmentName2,
			Unknown57,
			MenuEquipmentName3,
			Unknown59,
			MenuEquipmentName4,
			Unknown61,
			MenuArmorTitle,
			UseLodgingSave
		}

		private static readonly string[] TextByBytes;
		private static readonly Dictionary<string, byte> BytesByText = new Dictionary<string, byte>
		{
			{ "#", 0x03 },
			{ "\n", 0x05 },
			{ "e ", 0x1A },
			{ " t", 0x1B },
			{ "th", 0x1C },
			{ "he", 0x1D },
			{ "s ", 0x1E },
			{ "in", 0x1F },
			{ " a", 0x20 },
			{ "t ", 0x21 },
			{ "an", 0x22 },
			{ "re", 0x23 },
			{ " s", 0x24 },
			{ "er", 0x25 },
			{ "ou", 0x26 },
			{ "d ", 0x27 },
			{ "to", 0x28 },
			{ "n ", 0x29 },
			{ "ng", 0x2A },
			{ "ea", 0x2B },
			{ "es", 0x2C },
			{ " i", 0x2D },
			{ "o ", 0x2E },
			{ "ar", 0x2F },
			{ "is", 0x30 },
			{ " b", 0x31 },
			{ "ve", 0x32 },
			{ " w", 0x33 },
			{ "me", 0x34 },
			{ "or", 0x35 },
			{ " o", 0x36 },
			{ "st", 0x37 },
			{ " c", 0x38 },
			{ "at", 0x39 },
			{ "en", 0x3A },
			{ "nd", 0x3B },
			{ "on", 0x3C },
			{ "hi", 0x3D },
			{ "se", 0x3E },
			{ "as", 0x3F },
			{ "ed", 0x40 },
			{ "ha", 0x41 },
			{ " m", 0x42 },
			{ " f", 0x43 },
			{ "r ", 0x44 },
			{ "le", 0x45 },
			{ "ow", 0x46 },
			{ "g ", 0x47 },
			{ "ce", 0x48 },
			{ "om", 0x49 },
			{ "GI", 0x4A },
			{ "y ", 0x4B },
			{ "of", 0x4C },
			{ "ro", 0x4D },
			{ "ll", 0x4E },
			{ " p", 0x4F },
			{ " y", 0x50 },
			{ "ca", 0x51 },
			{ "MA", 0x52 },
			{ "te", 0x53 },
			{ "f ", 0x54 },
			{ "ur", 0x55 },
			{ "yo", 0x56 },
			{ "ti", 0x57 },
			{ "l ", 0x58 },
			{ " h", 0x59 },
			{ "ne", 0x5A },
			{ "it", 0x5B },
			{ "ri", 0x5C },
			{ "wa", 0x5D },
			{ "ac", 0x5E },
			{ "al", 0x5F },
			{ "we", 0x60 },
			{ "il", 0x61 },
			{ "be", 0x62 },
			{ "rs", 0x63 },
			{ "u ", 0x64 },
			{ " l", 0x65 },
			{ "ge", 0x66 },
			{ " d", 0x67 },
			{ "li", 0x68 },
			{ "..", 0x69 },
			{ "/", 0x7A },
			{ "+", 0x7B },
			{ "0", 0x80 },
			{ "1", 0x81 },
			{ "2", 0x82 },
			{ "3", 0x83 },
			{ "4", 0x84 },
			{ "5", 0x85 },
			{ "6", 0x86 },
			{ "7", 0x87 },
			{ "8", 0x88 },
			{ "9", 0x89 },
			{ "A", 0x8A },
			{ "B", 0x8B },
			{ "C", 0x8C },
			{ "D", 0x8D },
			{ "E", 0x8E },
			{ "F", 0x8F },
			{ "G", 0x90 },
			{ "H", 0x91 },
			{ "I", 0x92 },
			{ "J", 0x93 },
			{ "K", 0x94 },
			{ "L", 0x95 },
			{ "M", 0x96 },
			{ "N", 0x97 },
			{ "O", 0x98 },
			{ "P", 0x99 },
			{ "Q", 0x9A },
			{ "R", 0x9B },
			{ "S", 0x9C },
			{ "T", 0x9D },
			{ "U", 0x9E },
			{ "V", 0x9F },
			{ "W", 0xA0 },
			{ "X", 0xA1 },
			{ "Y", 0xA2 },
			{ "Z", 0xA3 },
			{ "a", 0xA4 },
			{ "b", 0xA5 },
			{ "c", 0xA6 },
			{ "d", 0xA7 },
			{ "e", 0xA8 },
			{ "f", 0xA9 },
			{ "g", 0xAA },
			{ "h", 0xAB },
			{ "i", 0xAC },
			{ "j", 0xAD },
			{ "k", 0xAE },
			{ "l", 0xAF },
			{ "m", 0xB0 },
			{ "n", 0xB1 },
			{ "o", 0xB2 },
			{ "p", 0xB3 },
			{ "q", 0xB4 },
			{ "r", 0xB5 },
			{ "s", 0xB6 },
			{ "t", 0xB7 },
			{ "u", 0xB8 },
			{ "v", 0xB9 },
			{ "w", 0xBA },
			{ "x", 0xBB },
			{ "y", 0xBC },
			{ "z", 0xBD },
			{ "'", 0xBE },
			{ ",", 0xBF },
			{ ".", 0xC0 },
			{ ";", 0xC1 }, //should never be used as a space character
			{ "-", 0xC2 },
			// this is a duplicate { "..", 0xC3 }, // could I use this?
			{ "!", 0xC4 },
			{ "?", 0xC5 },

			// I can put two in 7E, 7F
			// Not anymore, that's used for opened chests

			// 0xC6 Level icon (it has a sinlge different PIXEL so it's off limits despite being otherwise identical to the regular L...)
			// 0xC7 Equip icon (ditto but for E)


			// Base Game Icons
			{ "≈U", 0xCE }, // unarmed
			{ "≈R", 0xCF }, // rod
			{ "≈c", 0xD0 }, // scimitar
			{ "≈f", 0xD1 }, // falchion
			{ "≈r", 0xD2 }, // rapier
			{ "≈w", 0xD3 }, // shortsword

			{ "@S", 0xD4 }, // swords (long)
			{ "@H", 0xD5 }, // hammers
			{ "@K", 0xD6 }, // knives
			{ "@X", 0xD7 }, // axes
			{ "@F", 0xD8 }, // staves
			{ "@N", 0xD9 }, // nunchucks
			{ "@A", 0xDA }, // armors
			{ "@s", 0xDB }, // shields
			{ "@h", 0xDC }, // helmets
			{ "@G", 0xDD }, // gauntelets
			{ "@B", 0xDE }, // bracelets
			{ "@T", 0xDF }, // shirts
			{ "%", 0xE0 },
			{ "@p", 0xE1 }, // potion

			{ "€s", 0xE2 }, // status
			{ "€p", 0xE3 }, // poison
			{ "€T", 0xE4 }, // time
			{ "€d", 0xE5 }, // death
			{ "€f", 0xE6 }, // fire
			{ "€i", 0xE7 }, // ice
			{ "€t", 0xE8 }, // lightning
			{ "€e", 0xE9 }, // earth

			{ "§d", 0xEA }, // dead
			{ "§s", 0xEB },	// stone
			{ "§p", 0xEC }, // poison
			{ "§b", 0xED }, // blind
			{ "§P", 0xEE }, // stun
			{ "§Z", 0xEF }, // sleep
			{ "§M", 0xF0 }, // mute
			{ "§C", 0xF1 }, // confuse


			{ " ", 0xFF }
		};

		// Custom Icons
		private static readonly Dictionary<string, byte> Icons = new Dictionary<string, byte>
		{
			{ "≈U", 0xA1 }, // unarmed
			{ "≈R", 0xA2 }, // rod
			{ "≈c", 0xA3 }, // scimitar
			{ "≈f", 0xA4 }, // falchion
			{ "≈r", 0xA5 }, // rapier
			{ "≈w", 0xA6 }, // shortsword

			{ "ΩA", 0xA7 }, // all magic
			{ "ΩW", 0xA8 }, // white magic
			{ "ΩG", 0xA9 }, // grey magic
			{ "ΩB", 0xAA }, // black magic
			{ "ΩR", 0xAB }, // recovery magic
			{ "Ωc", 0xAC }, // health (cure) magic
			{ "Ωa", 0xAD }, // ailment magic
			{ "Ωl", 0xAE }, // life magic
			{ "Ωh", 0xAF }, // holy magic
			{ "Ωs", 0xB0 }, // space magic
			{ "Ωt", 0xB1 }, // tele magic
			{ "ΩU", 0xB2 }, // buff magic
			{ "ΩS", 0xB3 }, // self magic

			{ "€s", 0xB4 }, // status
			{ "€p", 0xB5 }, // poison
			{ "€T", 0xB6 }, // time
			{ "€d", 0xB7 }, // death
			{ "€f", 0xB8 }, // fire
			{ "€i", 0xB9 }, // ice
			{ "€t", 0xBA }, // lightning
			{ "€e", 0xBB }, // earth

			{ "§d", 0xBC }, // dead
			{ "§s", 0xBD },	// stone
			{ "§p", 0xBE }, // poison
			{ "§b", 0xBF }, // blind
			{ "§P", 0xC0 }, // stun
			{ "§Z", 0xC1 }, // sleep
			{ "§M", 0xC2 }, // mute
			{ "§C", 0xC3 }, // confuse


		};

		private static readonly Dictionary<string, byte> StatCodes = new Dictionary<string, byte>
		{
			{"stat", 0x10 },
			{"str", 0x07 },
			{"agi", 0x08 },
			{"int", 0x09 },
			{"vit", 0x0A },
			{"luck", 0x0B },
			{"damage", 0x3C },
			{"hit%", 0x3D },
			{"absorb", 0x3E },
			{"evade", 0x3F },
			{"mdef", 0x50 }
		};

		static FF1Text()
		{
			TextByBytes = new string[256];
			foreach (var key in BytesByText.Keys)
			{
				TextByBytes[BytesByText[key]] = key;
			}

			TextByBytes[0xC3] = "..";
		}

		public static string BytesToText(byte[] bytes)
		{
			var builder = new StringBuilder();
			foreach (var b in bytes)
			{
				builder.Append(TextByBytes[b]);
			}

			return builder.ToString();
		}

		public static bool isIcon(string charCode) {
			return Icons.ContainsKey(charCode);
		}

		public static Blob TextToBytes(string text, bool useDTE = true, Delimiter delimiter = Delimiter.Null, bool useExtraIcons = false)
		{
			Blob bytes = new byte[text.Length + 1];
			int i = 0, j = 0;
			while (i < text.Length - 1)
			{
				var twoChars = text.Substring(i, 2);
				if (BytesByText.ContainsKey(twoChars) && (useDTE || twoChars[0] == '@') || isIcon(twoChars))
				{
					if (isIcon(twoChars) && useExtraIcons)
					{
						bytes[j++] = 0x10;
						bytes[j++] = Icons[twoChars];
					}
					else
						bytes[j++] = BytesByText[twoChars];

					i += 2;
				}
				else
				{
					bytes[j++] = BytesByText[text[i++].ToString()];
				}
			}

			if (i < text.Length)
			{
				bytes[j++] = BytesByText[text[i++].ToString()];
			}

			if (delimiter != Delimiter.Empty)
			{
				bytes[j++] = (byte)delimiter;
			}

			return bytes.SubBlob(0, j);
		}
		public static Blob TextToBytesStats(string text, bool useDTE = true, Delimiter delimiter = Delimiter.Null)
		{
			//Convert strings containing control codes in curly braces, eg. "STR.   {stat}{str}"
			List<byte> bytes = new List<byte>();
			int textIndex = 0;
			while (textIndex < text.Length)
			{
				if (text[textIndex] == '{')
				{
					string code = text.Substring(textIndex+1, text.IndexOf('}', textIndex+1) - textIndex - 1);
					
					bytes.Add(StatCodes[code]);
					textIndex += code.Length + 2;
				}
				else
				{
					bytes.Add(BytesByText[text[textIndex].ToString()]);
					textIndex++;
				}
			}

			if (delimiter != Delimiter.Empty)
			{
				bytes.Add((byte)delimiter);
			}

			return bytes.ToArray();
		}

		public static Blob TextToBytesInfo(string text, bool useDTE = true, Delimiter delimiter = Delimiter.Null, bool useExtraIcons = false)
		{
			Blob bytes = new byte[text.Length + 1];
			int i = 0, j = 0;
			while (i < text.Length - 1)
			{
				var twoChars = text.Substring(i, 2);
				if (twoChars[0] == '¤') // Control Code 0x14 for second words table
				{
					bytes[j++] = 0x14;
					bytes[j++] = Blob.FromHex(text.Substring(i + 1, 2))[0];
					i += 3;
				}
				else if (twoChars[0] == '$') // Control code 0x02 for itemnames table
				{
					bytes[j++] = 0x02;
					bytes[j++] = Blob.FromHex(text.Substring(i + 1, 2))[0];
					i += 3;
				}
				else if (BytesByText.ContainsKey(twoChars) && (useDTE || twoChars[0] == '@') || isIcon(twoChars))
				{
					if (isIcon(twoChars) && useExtraIcons)
					{
						bytes[j++] = 0x10;
						bytes[j++] = Icons[twoChars];
					}
					else
						bytes[j++] = BytesByText[twoChars];

					i += 2;
				}
				else
				{
					bytes[j++] = BytesByText[text[i++].ToString()];
				}
			}

			if (i < text.Length)
			{
				bytes[j++] = BytesByText[text[i++].ToString()];
			}

			if (delimiter != Delimiter.Empty)
			{
				bytes[j++] = (byte)delimiter;
			}

			return bytes.SubBlob(0, j);
		}

		// This wraps TextToBytes for use with Credits pages.
		public static Blob TextToCredits(string[] lines)
		{
			// Starting PPU addr immediately inside the box without any padding.
			// Each line is 0x20 total characters.
			ushort topLeftOfBox = 0x20A5;

			List<Blob> buffers = new List<Blob>();
			for (int i = 0; i < lines.Length; ++i)
			{
				string line = lines[i].Trim();
				if (line == "")
				{
					continue;
				}

				int spaces = lines[i].Length - lines[i].TrimStart(' ').Length;
				ushort[] ppuPtr = { (ushort)(topLeftOfBox + (0x20 * i) + spaces) };
				buffers.Add(Blob.FromUShorts(ppuPtr));
				buffers.Add(TextToBytes(line, useDTE: false, delimiter: Delimiter.Segment));
			}

			if (buffers.Count != 0)
			{
				var lastBuffer = buffers[buffers.Count - 1];
				lastBuffer[lastBuffer.Length - 1] = (byte)Delimiter.Null;
			}

			return Blob.Concat(buffers);
		}

		// This wraps TextToBytes for use with Story pages (Before Credits and End of Game).
		public static Blob TextToStory(string[] lines)
		{
			List<Blob> buffers = new List<Blob>();
			for (int i = 0; i < lines.Length; ++i)
			{
				buffers.Add(TextToBytes(lines[i], useDTE: true, delimiter: Delimiter.Line));
			}

			if (buffers.Count != 0)
			{
				var lastBuffer = buffers[buffers.Count - 1];
				lastBuffer[lastBuffer.Length - 1] = (byte)Delimiter.Null;
			}

			return Blob.Concat(buffers);
		}

		// Returns a centered, whitespace padded line of 32 characters
		public static Blob TextToCopyrightLine(string text)
		{
			if (text.Length > 32)
			{
				text = text.Substring(0, 32);
				//throw new ArgumentOutOfRangeException();
			}

			var flagLeft = new string(' ', (int)Math.Ceiling((32 - text.Length) / 2.0));
			var flagRight = new string(' ', (int)Math.Floor((32 - text.Length) / 2.0));

			return TextToBytes(flagLeft + text + flagRight, false, Delimiter.Empty);
		}

		// Loads custom icons
		public static void AddNewIcons(FF1Rom rom, Flags flags)
		{
			// Icons have a 0x10 Control code indicating the next byte is pulled from the 0x00-0x7F tile reference instead of the 0x80-0xFF like regular fonts

			// Control Code 0x10 Blobs
			rom.PutInBank(0x1E, 0x8680, Blob.FromHex("C98090174980AE0220A6558E0620A6548E06208D0720E6544C4EE0AAA98448A9F348A90E4C03FE")); // Parse and draw the control code

			rom.PutInBank(0x0E, 0x84F4, Blob.FromHex("8A20708D203EDE4C4EE0")); // Setup for print char stat?
			rom.PutInBank(0x1F, 0xDF2D, Blob.FromHex("A91E2003FE8A4C8086")); // Swap and jump to our code in bank 1E


			//	New image blobs are imported into Bank 12, 0x8800. There is 0x800 of space but we can only use 0x600. as the last 0x200 is reserved for orbs.

			// Copy over the ORBS
			var tileset = rom.GetFromBank(0x0D, 0xB600, 0x0200);     // Get font tileset
			rom.PutInBank(0x12, 0x8800 + 0x600, tileset);            // Put it in bank 12

			if (flags.ShardHunt)
				rom.addShardIcon(0x12, 0x8800 + 0x760);			        // If we're shard hunt, add the shard to tiles 0x76 and 0x77 because we missed em

			// Change where the ORBS are loaded from and to so we can piggyback our icons - Now we load 8 lines from Bank 12 into 0000 of the PPU
			rom.PutInBank(0x1F, 0xEA9F, Blob.FromHex("2002EAA9122003FEA208A9008510A9888511A900"));

			// The code below handles loading in icons into the shop, since the shop doesn't have as much free space
			tileset = rom.GetFromBank(0x09, 0x8800, 0x0800);
			rom.PutInBank(0x12, 0x8000, tileset);                    // Put it in bank 12

			// Subvert shop loading code and redirect it to Bank 12 for better management
			rom.PutInBank(0x1F, 0xEA02, Blob.FromHex("A9122003FE200090A9092003FEA900A208205AE9EAEAEA"));
			rom.PutInBank(0x12, 0x9000, Blob.FromHex("A9008510A9808511A208A908205AE9A9008510A980851160"));
			rom.PutInBank(0x1F, 0xE99E, Blob.FromHex("A9122003FE202090"));
			rom.PutInBank(0x12, 0x9020, Blob.FromHex("A9008510A9808511A208A908205AE9AD0220A9118D062060"));
			

			// Load icons from images. Make your own if you'd like! Don't forget to add them to the icon dictionary
			//IImageFormat format;

			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var weaponiconsPath = assembly.GetManifestResourceNames().First(str => str.EndsWith("weapon_icons.png"));
			var spelliconsPath = assembly.GetManifestResourceNames().First(str => str.EndsWith("spell_icons.png"));
			var elementstatusiconsPath = assembly.GetManifestResourceNames().First(str => str.EndsWith("elementstatus_icons.png"));

			// 0 = black
			// 1 = grey
			// 2 = blue
			// 3 = white

			Dictionary<Rgba32, byte> index = new Dictionary<Rgba32, byte> {
				{ new Rgba32(0x00, 0x00, 0x00), 0 },
				{ new Rgba32(0x7f, 0x7f, 0x7f), 1 },
				{ new Rgba32(116, 116, 116), 1 },
				{ new Rgba32(0x00, 0x00, 0xff), 2 },
				{ new Rgba32(36, 24, 140), 2 },
				{ new Rgba32(0xff, 0xff, 0xff), 3 },
				{ new Rgba32(252, 252, 252), 3 }
			};

			// New Code
			int offset = 0x8800;
			offset += 16 * 32 + 16; // Left the first line blank for silly reasons. If more icons get added, clean this up

			// Weapons
			Image<Rgba32> image = Image.Load<Rgba32>(assembly.GetManifestResourceStream(weaponiconsPath));
			for (int w = 0; w < 6; w++)
			{
				rom.PutInBank(0x12, offset, rom.EncodeForPPU(getTile(w, index, image)));
				offset += 16;
			}

			// Spells
			image = Image.Load<Rgba32>(assembly.GetManifestResourceStream(spelliconsPath));
			for (int w = 0; w < 13; w++)
			{
				rom.PutInBank(0x12, offset, rom.EncodeForPPU(getTile(w, index, image)));
				offset += 16;
			}

			// These are the old icons, no images for these only THE BLOB
			image = Image.Load<Rgba32>(assembly.GetManifestResourceStream(elementstatusiconsPath));
			for (int w = 0; w < 16; w++)
			{
				rom.PutInBank(0x12, offset, rom.EncodeForPPU(getTile(w, index, image)));			// Non Shop Icons
				rom.PutInBank(0x12, 0x8620 + w*16, rom.EncodeForPPU(getTile(w, index, image)));     // Shop Icons
				offset += 16;
			}

			// Additional Icon space
			//
			// Everywhere font is available (Bank 0x9)
			//	• 2 icons		at 0x87E0
			//
			// Menu and Party Select (Bank 0x12)
			//  • 32 Icons		at 0x8810 (0x8800 is reserved for backgrounds)
			//	• 28 Icons		at 0x8C40
			//
			// Shop and battle (Bank 0x12)
			//	• 7 Icons		at 0x8470 (first slot is L, add 0x10 if not replacing it)
			//	• 5 Icons		at 0x8720
			//
			// Shard Hunt Only Icons available in Menu (Bank 0x12)
			//  • 20 Icons		at 0x8E00
			//
		}

		public static byte[] getTile(int imageTileIndex, Dictionary<Rgba32, byte> index, Image<Rgba32> image)
		{
			var newtile = new byte[64];
			int px = 0;
			for (int y = 0; y < (0 + 8); y++)
			{
				for (int x = (imageTileIndex * 8); x < ((imageTileIndex * 8) + 8); x++)
				{
					newtile[px] = index[image[x, y]];
					px++;
				}
			}

			return newtile;
		}

	}

	public class MenuText
	{
		public Blob[] MenuStrings { get; set; }
		private int stringCount = 0x40;
		private int pointerOffset = 0x38500;
		private int textOffset = 0x38580;
		private int pointerBase = 0x30000;
		public MenuText(FF1Rom rom)
		{
			MenuStrings = new Blob[stringCount];
			List<ushort> pointers = rom.Get(pointerOffset, 2 * stringCount).ToUShorts().ToList();

			for (int i = 0; i < pointers.Count - 1; i++)
			{
				//Use next pointer to determine length, strange bugs occur if we just look for null terminators for all strings
				MenuStrings[i] = rom.Get(pointerBase + pointers[i], pointers[i + 1] - pointers[i]);
			}
			//No next pointer, so look for null terminator for length of final string
			MenuStrings[stringCount-1] = rom.ReadUntil(pointerBase + pointers[stringCount-1], 0x00);
		}
		public void Write(FF1Rom rom)
		{
			int offset = textOffset;
			var plainstrings = from m in MenuStrings select FF1Text.BytesToText(m);
			ushort[] pointers = new ushort[stringCount];
			for (int i = 0; i < stringCount; i++)
			{
				rom.Put(offset, MenuStrings[i]);

				pointers[i] = (ushort)(offset - pointerBase);
				offset += MenuStrings[i].Length;
			}
			rom.Put(pointerOffset, Blob.FromUShorts(pointers.ToArray()));
		}
	}

	public class ItemNames
	{
		public const int ItemTextPointerOffset = 0x2B700;
		public const int ItemTextPointerCount = 256;
		public const int ItemTextPointerBase = 0x20000;
		public const int ItemTextOffset = 0x2B900;

		private List<string> _itemsTexts = new();
		private List<ushort> _itemsPointers = new();

		private Blob ReadUntil(FF1Rom rom, int offset, byte delimiter)
		{
			var bytes = new List<byte>();
			while (rom[offset] != delimiter && offset < 0x40000)
			{
				bytes.Add(rom[offset++]);
			}
			bytes.Add(delimiter);

			return bytes.ToArray();
		}

		public ItemNames(FF1Rom rom)
		{
			_itemsPointers = rom.Get(ItemTextPointerOffset, 2 * ItemTextPointerCount).ToUShorts().ToList();

			for (int i = 0; i < _itemsPointers.Count; i++)
			{
				_itemsTexts.Add(FF1Text.BytesToText(ReadUntil(rom, ItemTextPointerBase + _itemsPointers[i], 0x00)));
			}
		}

		public string this[int textid]
		{
			get => _itemsTexts[textid];
			set => _itemsTexts[textid] = value;
		}
		public List<string> ToList()
		{
			return new List<string>(_itemsTexts);
		}
		private void FixRibbonSpace()
		{
			if (_itemsTexts[(int)Item.Ribbon].Length > 7 && _itemsTexts[(int)Item.Ribbon][7] == ' ')
			{
				_itemsTexts[(int)Item.Ribbon] = _itemsTexts[(int)Item.Ribbon].Remove(7);
			}
		}
		public void Write(FF1Rom rom, List<Item> unusedGoldItems)
		{
			FixRibbonSpace();

			foreach (var golditem in unusedGoldItems)
			{
				_itemsTexts[(int)golditem] = "";
			}

			var duplicates = _itemsTexts.GroupBy(x => x)
			  .Where(g => g.Count() > 1)
			  .Select(y => y.Key)
			  .ToList();

			List<(string, int)> duplicateIndex = new();

			foreach (var item in duplicates)
			{
				duplicateIndex.Add((item, _itemsTexts.FindIndex(x => x == item)));
			}

			int offset = ItemTextOffset;

			for (int i = 0; i < ItemTextPointerCount; i++)
			{
				var duplicateFound = duplicateIndex.Where(x => x.Item1 == _itemsTexts[i]).ToList();

				if (duplicateFound.Any() && i != duplicateFound[0].Item2)
				{
					_itemsPointers[i] = _itemsPointers[duplicateFound[0].Item2];
				}
				else
				{
					var blob = FF1Text.TextToBytes(_itemsTexts[i], useDTE: false);
					rom.Put(offset, blob);
					_itemsPointers[i] = (ushort)(offset - ItemTextPointerBase);
					offset += blob.Length;
				}
			}

			if (offset >= 0x2C000)
			{
				throw new Exception("Items Text is too large to fit within available space.");
			}
			rom.Put(ItemTextPointerOffset, Blob.FromUShorts(_itemsPointers.ToArray()));
		}
	}

	public class BattleMessages {
	    // Note battle messages are moved into bank 1B by
	    // BankRearranging.Bank1B() so these offsets are specific
	    // to an FFR ROM offsets not vanilla.
	    public const int BattleTextPointers = 0x6C3BA;
	    public const int BattleTextBase = 0x64000;
	    public const int BattleTextPointerCount = 0x50;

	    private List<string> _battleTexts = new();
	    private List<ushort> _battleTextPointers = new();

	    private Blob ReadUntil(FF1Rom rom, int offset, byte delimiter)
	    {
		var bytes = new List<byte>();
		while (rom[offset] != delimiter && offset < 0x70000)
		{
		    bytes.Add(rom[offset++]);
		}
		bytes.Add(delimiter);

		return bytes.ToArray();
	    }

	    public BattleMessages(FF1Rom rom) {
		_battleTextPointers = rom.Get(BattleTextPointers, 2 * BattleTextPointerCount).ToUShorts().ToList();

		for (int i = 0; i < _battleTextPointers.Count; i++)
		{
		    _battleTexts.Add(FF1Text.BytesToText(ReadUntil(rom, BattleTextBase + _battleTextPointers[i], 0x00)));
		}
	    }

		public string this[int textid]
		{
			get => _battleTexts[textid];
			set => _battleTexts[textid] = value;
		}

	}

	public class EnemyText
	{
		public const int EnemyTextPointerOffset = 0x2D4E0;
	    public const int EnemyTextPointerBase = 0x24000;
	    public const int EnemyTextOffset = 0x2D5E0;
		public const int EnemyCount = 128;

		private string[] _enemyTexts;

		public EnemyText(FF1Rom rom)
		{
			_enemyTexts = rom.ReadText(EnemyTextPointerOffset, EnemyTextPointerBase, EnemyCount);
		}

		public void Write(FF1Rom rom)
		{
				// here we're splitting the enemy names into two parts, each with 64 names
			// each name can be a maximum of 8 character bytes + 1 null terminator byte = 9 bytes.
			// each bank of enemy text needs 9 * 64 = 0x240 bytes.

			// this address is 0x240 bytes before the fiend drawing tables at 0x2d2E0,
			// in the closest empty patch in the current bank.
			// This replaces the 0x2CFEC address used in some of the enemy text
			// routines throughout the randomizer.
			const int EnemyTextOffsetPart1 = 0x2D0A0;

			const int EnemyTextOffsetPart2 = EnemyTextOffset;

			
			var enemyTextPart1 = _enemyTexts.Take(EnemyCount/2).ToArray();
			var enemyTextPart2 = _enemyTexts.Skip(EnemyCount/2).ToArray(); 


			// write each bank of texts.
			// EnemyTextPointerOffset is the absolute address of the table of pointers to each name.
			// Each of these pointers gives a two-byte address to the text, relative to EnemyTextPointerBase
			// Therefore the pointers to enemyTextPart2 need to be written to EnemyTextPointerOffset + 64*2

			rom.WriteText(enemyTextPart1, EnemyTextPointerOffset, EnemyTextPointerBase,EnemyTextOffsetPart1);
			rom.WriteText(enemyTextPart2, EnemyTextPointerOffset + EnemyCount, EnemyTextPointerBase, EnemyTextOffsetPart2);
		}

		public string[] Get()
		{
			return (string[])_enemyTexts.Clone();
		}

		public void Set(string[] text)
		{
			if (text.Length == EnemyCount)
				_enemyTexts = (string[])text.Clone();
			else
				throw new Exception("Incorrect number of strings in enemy text array.");
		}

		public string this[int textid]
		{
			get => _enemyTexts[textid];
			set => _enemyTexts[textid] = value;
		}

	}
}
