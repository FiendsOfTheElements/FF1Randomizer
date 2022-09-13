using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using RomUtilities;

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

			// 0xC6 Level icon (it has a sinlge different PIXEL so it's off limits despite being otherwise identical to the regular L...)
			// 0xC7 Equip icon (ditto but for E)


			// Base Game Icons
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

		public static Blob TextToBytes(string text, bool useDTE = true, Delimiter delimiter = Delimiter.Null)
		{
			Blob bytes = new byte[text.Length + 1];
			int i = 0, j = 0;
			while (i < text.Length - 1)
			{
				var twoChars = text.Substring(i, 2);
				if (BytesByText.ContainsKey(twoChars) && (useDTE || twoChars[0] == '@') || isIcon(twoChars))
				{
					if (isIcon(twoChars))
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

		public static Blob TextToBytesInfo(string text, bool useDTE = true, Delimiter delimiter = Delimiter.Null)
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
					if (isIcon(twoChars))
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
		public static void AddNewIcons(FF1Rom rom)
		{
			// Icons have a 0x10 Control code indicating the next byte is pulled from the 0x00-0x7F tile reference instead of the 0x80-0xFF like regular fonts

			// Control Code 0x10 Blobs
			rom.PutInBank(0x1E, 0x8600, Blob.FromHex("C98090174980AE0220A6558E0620A6548E06208D0720E6544C4EE0AAA98448A9F348A90E4C03FE")); // Parse and draw the control code

			rom.PutInBank(0x0E, 0x84F4, Blob.FromHex("8A20708D203EDE4C4EE0")); // Setup for print char stat?
			rom.PutInBank(0x1F, 0xDF2D, Blob.FromHex("A91E2003FE8A4C0086")); // Swap and jump to our code in bank 1E


			//	New image blobs are imported into Bank 12, 0x8800. There is 0x800 of space but we can only use 0x600. as the last 0x200 is reserved for orbs.

			// Copy over the ORBS
			var tileset = rom.GetFromBank(0x0D, 0xB600, 0x0200);     // Get font tileset
			rom.PutInBank(0x12, 0x8800 + 0x600, tileset);              // Put it in bank 12

			// Change where the ORBS are loaded from and to so we can piggyback our icons - Now we load 8 lines from Bank 12 into 0000 of the PPU
			rom.PutInBank(0x1F, 0xEAA2, Blob.FromHex("A9122003FEA208A9008510A9888511A900"));

			// Load icons from images. Make your own if you'd like! Don't forget to add them to the icon dictionary

			IImageFormat format;

			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var weaponiconsPath = assembly.GetManifestResourceNames().First(str => str.EndsWith("weapon_icons.png"));
			var spelliconsPath = assembly.GetManifestResourceNames().First(str => str.EndsWith("spell_icons.png"));

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
			offset += 16 * 32 + 16; // First line needs to remain blank, it's used by the system for special commands

			// Weapons
			Image<Rgba32> image = Image.Load<Rgba32>(assembly.GetManifestResourceStream(weaponiconsPath), out format);
			for (int w = 0; w < 6; w++)
			{
				rom.PutInBank(0x12, offset, rom.EncodeForPPU(getTile(w, index, image)));
				offset += 16;
			}

			// Spells
			image = Image.Load<Rgba32>(assembly.GetManifestResourceStream(spelliconsPath), out format);
			for (int w = 0; w < 13; w++)
			{
				rom.PutInBank(0x12, offset, rom.EncodeForPPU(getTile(w, index, image)));
				offset += 16;
			}

			// These are the old icons, no images for these only THE BLOB
			var newIcons = "00183C3C18180018FFFFFFFFFFFFFFFF" + // E2 to F1
							"001812446036381CFFFFFFFFFFFFFFFF" +
							"0044BA6CE6FE7C38FFFFFFFFFFFFFFFF" +
							"00386CC66C7C3838FFFFFFFFFFFFFFFF" +
							"000C3872D88C8448FFFFFFFFFFFFFFFF" +
							"0010104410441010FFFFFFFFFFFFFFFF" +
							"00103070FE1C1810FFFFFFFFFFFFFFFF" +
							"00000000E7F3E7EFFFFFFFFFFFFFFFFF" +
							"007CFE92FE540038FFFFFFFFFFFFFFFF" +
							"003C7EBDD9B1523CFFFFFFFFFFFFFFFF" +
							"0038102828447C38FFFFFFFFFFFFFFFF" +
							"00006CDA926C0000FFFFFFFFFFFFFFFF" +
							"0080E82E82E82E02FFFFFFFFFFFFFFFF" +
							"0070102E4274080EFFFFFFFFFFFFFFFF" +
							"003C7EFFD57E3C0EFFFFFFFFFFFFFFFF" +
							"003C42421C100010FFFFFFFFFFFFFFFF";

			rom.PutInBank(0x12, offset, Blob.FromHex(newIcons));
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
		public void Write(FF1Rom rom, List<int> unusedGoldItems)
		{
			foreach (var golditem in unusedGoldItems)
			{
				_itemsTexts[golditem] = "";
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
}
