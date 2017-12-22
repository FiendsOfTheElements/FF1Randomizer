using RomUtilities;
using System.Collections.Generic;
using System.Linq;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		// A fun list of initial victory pages. Classic mistranslations.
		private static readonly List<string[]> victoryMessages = new List<string[]>
		{
			new [] // Snow Brothers
			{
				"",
				"Congratulation!",
				"",
				"    Let's  Go",
				"     Stage 2",
			},
			new [] // Bubble Bobble
			{
				"",
				"Now it is the",
				"beginning of a",
				"fantastic story.",
				"Let us make a",
				"journey to the",
				"cave of monster!",
			},
			new [] // Pro Wrestling
			{
				"",
				"   Ranked #1",
				"",
				"A Winner Is You",
			},
			new [] // Unknown Game
			{
				"",
				"   Game Over",
				"",
				"Return of Ganon",
			},
			new [] // Friday the 13th
			{
				"",
				"  You And Your",
				"Friends Are dead.",
				"",
				"   Game Over",
			},
			new [] // Ghostbusters
			{
				"Conglaturation!!",
				"",
				"You have comple-",
				"ted a great game",
				"and prooved the",
				"justice of our",
				"culture.",
			},
			new [] // Shadowgate
			{
				"",
				"",
				"It's a sad thing",
				"that your adven-",
				"tures have ended",
				"here!!",
			},
			new [] // Bionic Commando
			{
				"",
				"",
				"Get the heck out",
				"  of here, you",
				"     nerd!",
			},
			new [] // Zelda
			{
				"",
				"",
				"Pay me for the",
				"  door repair",
				"    charge.",
			},
			new [] // Zelda
			{
				"",
				"",
				"What a horrible",
				" night to have",
				"   a curse.",
			},
		};

		// Story pages. The first set is before the credits.
		// The second set is the ending cinematic with counters.
		private static readonly List<string[]> storyPages = new List<string[]>
		{
			new []
			{
				"",
				" Final  Fantasy ",
				"",
				"",
				"   Randomizer   ",
			},
			new []
			{
				"  Development   ",
				"",
				"  Entroper",
				"  MeridianBC",
				"  tartopan",
				"  nitz",
			},
			new []
			{
				"  Playtesting",
				"",
				"jkoper, Kababesh",
				"DarkwingDuck.sda",
				"Miaut, Ayntlerz ",
				"czardia,gameboy9",
				"EunosXX, Jaylow7",
				"  theCubeMiser",
				"  Beefucurry",
			},
		};

		private static readonly List<string[]> endingPages = new List<string[]>
		{
			new []
			{
				"",
				"   GAME  OVER",
				"",
				"",
				"    YOU  WIN!",
			},
			new []
			{
				"",
				"   Thank You",
				"",
				"",
				"  For Playing!",
			},
		};

		public void RollCredits(MT19337 rng)
		{
			SetupStoryPages(rng);
			SetupBridgeCredits();
		}

		private void SetupStoryPages(MT19337 rng)
		{
			// Setup DrawComplexString hijack for a particular escape sequence. See Credits.asm.
			Put(0x7DFA8, Blob.FromHex("A000A200B13EE63ED002E63F9510E8E003D0F18C1D608C1E60B111991C60C8C410D0F64C45DE"));

			List<Blob> pages = new List<Blob>();
			foreach (string[] text in storyPages)
			{
				pages.Add(FF1Text.TextToStory(text));
			}
			foreach (string[] text in endingPages)
			{
				pages.Add(FF1Text.TextToStory(text));
			}

			// An unused escape code from DrawComplexString is overridden allowing the following:
			// 1010 XX ADDR, Where 1010 enters the escape sequence, the next byte the the size of
			// the integer to print, and the next two bytes as a pointer to it. It is then copied
			// over the gold value, so 04 will follow as the next escape sequence to print gold.
			Blob[] stats =
				{
				FF1Text.TextToBytes("Final Results", true, FF1Text.Delimiter.Line),
				FF1Text.TextToBytes("", true, FF1Text.Delimiter.Line),
				FF1Text.TextToBytes("Gold      ", true, FF1Text.Delimiter.Empty), Blob.FromHex("0405"),
				FF1Text.TextToBytes("Steps     ", true, FF1Text.Delimiter.Empty), Blob.FromHex("101003A0600405"),
				FF1Text.TextToBytes("Resets    ", true, FF1Text.Delimiter.Empty), Blob.FromHex("101002A5640405"),
				FF1Text.TextToBytes("Hard Boots", true, FF1Text.Delimiter.Empty), Blob.FromHex("101002A3640400"),
			};
			pages.Add(Blob.Concat(stats));

			// Add fun goodbye message
			pages.Add(FF1Text.TextToStory(victoryMessages[rng.Between(0, victoryMessages.Count - 1)]));

			Blob storyText = packageTextBlob(pages, 0xA800);
			System.Diagnostics.Debug.Assert(storyText.Length <= 0x0500, "Story text too large!");

			Put(0x36800, storyText);
			Data[0x36E00] = (byte)(storyPages.Count);
			Data[0x36E01] = (byte)(pages.Count - 1);
		}

		private void SetupBridgeCredits()
		{
			// Wallpaper over the JSR to the NASIR CRC to circumvent their neolithic DRM.
			Put(0x7CF34, Blob.FromHex("EAEAEA"));

			// Actual Credits. Each string[] is a page. Each "" skips a line, duh.
			// The lines have zero padding on all sides, and 16 usable characters in length.
			// Don't worry about the inefficiency of spaces as they are all trimmed and the
			// leading spaces are used to increment the PPU ptr precisely to save ROM space.
			List<string[]> texts = new List<string[]>
			{
				new [] {
					"",
					"  Playtesting   ",
					"",
					"  Xarnax42",
					"  TristalMTG",
					"  PanzerDave",
					"  Ategenos",
					"  Buffalax",
					"  crimsonavix",
				},
				new []
				{
					" Special Thanks ",
					"",
					"fcoughlin, Disch",
					"Paulygon, anomie",
					"Derangedsquirrel",
					"AstralEsper, and",
					"",
					" The Entire FFR ",
					"    Community   ",
				},
				new []
				{
					"",
					"",
					"   Programmed   ",
					"       By       ",
					"",
					"E N T R O P E R "
				},
			};

			// Accumulate all our Credits pages before we set up the string pointer array.
			List<Blob> pages = new List<Blob>();
			foreach (string[] text in texts)
			{
				pages.Add(FF1Text.TextToCredits(text));
			}

			// Clobber the number of pages to render before we insert in the pointers.
			Data[0x37873] = (byte)pages.Count;

			Blob credits = packageTextBlob(pages, 0xBB00); 
			System.Diagnostics.Debug.Assert(credits.Length <= 0x0100, "Credits too large: " + credits.Length);
			Put(0x37B00, credits);
		}

		private Blob packageTextBlob(List<Blob> pages, ushort baseAddr)
		{
			// The first pointer is immediately after the pointer table.
			List<ushort> ptrs = new List<ushort> { (ushort)(baseAddr + pages.Count * 2) };
			for (int i = 1; i < pages.Count; ++i)
			{
				ptrs.Add((ushort)(ptrs.Last() + pages[i - 1].Length));
			}

			pages.Insert(0, Blob.FromUShorts(ptrs.ToArray()));
			return Blob.Concat(pages);
		}
	}
}
