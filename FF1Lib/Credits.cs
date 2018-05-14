using RomUtilities;
using System.Collections.Generic;
using System.Linq;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		// A fun list of initial victory pages. Classic mistranslations.
		private static readonly List<string[]> VictoryMessages = new List<string[]>
		{
			new [] // Snow Brothers
			{
				"",
				"Congratulation!",
				"",
				"    Let's  Go",
				"     Stage 2",
			},
			new [] // Pro Wrestling
			{
				"",
				"  Ranked No. 1",
				"",
				"A Winner Is You",
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
			new [] // Castlevania
			{
				"YOU PLAYED THE",
				"GREATEST ROLE",
				"IN THIS STORY."
			},
			new [] // Bad Dudes
			{
				"HEY WARRIORS,",
				"THANKS FOR",
				"DEFEATING CHAOS.",
				"LET'S GO FOR A",
				"BURGER....",
				"HA! HA! HA! HA!"
			},
			new [] // Metroid
			{
				"GREAT !!",
				"YOU FULFILED",
				"YOUR MISSION. IT",
				"WILL REVIVE",
				"PEACE IN SPACE.",
				"BUT,IT MAY BE",
				"INVADED BY THE",
				"OTHER CHAOS.",
				//"PRAY FOR A TRUE",
				//"PEACE IN SPACE!"
			},
			new [] // Dragon Warrior
			{
				"And thus the",
				"tale comes to an",
				"end.... unless",
				"Garland returns",
				"again."
			},
			new [] // Guardian Legend
			{
				"MISSION",
				"  COMPLETE.",
				"",
				"YOU ARE THE",
				"  GREATEST",
				"     PLAYER."
			},
			new [] // Zelda
			{
				"YOU ARE GREAT!",
				"YOU HAVE AN",
				"AMAZING WISDOM",
				"AND POWER!",
				"END OF",
				"'FINAL FANTASY",
				"1 RANDOMIZER'"
			},
			new [] // Ghosts and Goblins
			{
				"CONGRATURATION",
				"THIS STORY IS",
				"HAPPY END.",
				"THANK YOU."
			},
			new [] // Friday the 13th
			{
				"YOU HAVE FINALLY",
				"MANAGED TO",
				"DEFEAT CHAOS....",
				"BUT IS HE REALLY",
				"DEAD? WE'RE NOT",
				"TELLING!!",
				"",
				"END.."
			}
		};

		// Story pages. The first set is before the credits.
		// The second set is the ending cinematic with counters.
		private static readonly List<string[]> BridgeStory = new List<string[]>
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
				"  Septimus",
			},
			new []
			{
				"  Community",
				"",
				"  Czardia",
				"  Edgeworth",
				"  ichbinmiah",
				"  Taralyn",
				"  Trinton",
				"  xIceBlue",
			},
			new []
			{
				"  Playtesting",
				"",
				"jkoper, Kababesh",
				"DarkwingDuck.sda",
				"Miaut, Ayntlerz ",
				"gameboy9, sbq92 ",
				"EunosXX, Jaylow7",
				"  theCubeMiser",
				"  Beefucurry",
			},
		};

		private static readonly string[] ThankYou = new []
		{
			"",
			"   Thank You",
			"",
			"",
			"  For Playing!",
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
			BridgeStory.ForEach(story => pages.Add(FF1Text.TextToStory(story)));

			// An unused escape code from DrawComplexString is overridden allowing the following:
			// 1010 XX ADDR, Where 1010 enters the escape sequence, the next byte the the size of
			// the integer to print, and the next two bytes as a pointer to it. It is then copied
			// over the gold value, so 04 will follow as the next escape sequence to print gold.
			/*
			    Tracked Stats:
				Steps:          $60A0 (24-bit)
				Hard Resets:    $64A3 (16-bit)
				Soft Resets:    $64A5 (16-bit)
				Battles:        $60A7 (16-bit)
				Ambushes:       $60A9 (16-bit)
				Strike Firsts:  $60AB (16-bit)
				Close Calls...: $60AD (16-bit)
				Damage Dealt:   $60AF (24-bit)
				Damage Taken:   $60B2 (24-bit)
				Perished:       $64B5 (8-bit)
				"Nothing Here": $60B6 (8-bit)
				Chests Opened:  $60B7 (8-bit)
				Can't Hold:     $60B9 (8-bit)
			*/
			Blob[] movementStats =
			{
				FF1Text.TextToBytes("Movement Stats", true, FF1Text.Delimiter.Line),
				FF1Text.TextToBytes("", true, FF1Text.Delimiter.Line),
				FF1Text.TextToBytes("Steps     ", true, FF1Text.Delimiter.Empty), Blob.FromHex("101003A0600405"),
				FF1Text.TextToBytes("Resets    ", true, FF1Text.Delimiter.Empty), Blob.FromHex("101002A5640405"),
				FF1Text.TextToBytes("Power off ", true, FF1Text.Delimiter.Empty), Blob.FromHex("101002A3640405"),
				FF1Text.TextToBytes("Nthng Here", true, FF1Text.Delimiter.Empty), Blob.FromHex("101001B6600405"),
				FF1Text.TextToBytes("Can't Hold", true, FF1Text.Delimiter.Empty), Blob.FromHex("101001B9600405"),
				FF1Text.TextToBytes("Chests Opened", true, FF1Text.Delimiter.Line),
				Blob.FromHex("FFFFFF101001B76004"), FF1Text.TextToBytes(" of 241", true, FF1Text.Delimiter.Null),
			};

			Blob[] battleResults =
			{
				FF1Text.TextToBytes("Battle Results", true, FF1Text.Delimiter.Line),
				FF1Text.TextToBytes("", true, FF1Text.Delimiter.Line),
				FF1Text.TextToBytes("Battles   ", true, FF1Text.Delimiter.Empty), Blob.FromHex("101002A7600405"),
				FF1Text.TextToBytes("Ambushes  ", true, FF1Text.Delimiter.Empty), Blob.FromHex("101002A9600405"),
				FF1Text.TextToBytes("Struck 1st", true, FF1Text.Delimiter.Empty), Blob.FromHex("101002AB600405"),
				FF1Text.TextToBytes("Close call", true, FF1Text.Delimiter.Empty), Blob.FromHex("101002AD600405"),
				FF1Text.TextToBytes("Perished  ", true, FF1Text.Delimiter.Empty), Blob.FromHex("101001B5640400"),
			};

			Blob[] combatStats =
			{
				FF1Text.TextToBytes("Combat Stats", true, FF1Text.Delimiter.Line),
				FF1Text.TextToBytes("", true, FF1Text.Delimiter.Line),
				FF1Text.TextToBytes("Damage Dealt", true, FF1Text.Delimiter.Line),
				FF1Text.TextToBytes("        ",     true, FF1Text.Delimiter.Empty), Blob.FromHex("101003AF6004919901"),
				FF1Text.TextToBytes("Damage Taken", true, FF1Text.Delimiter.Line),
				FF1Text.TextToBytes("        ",     true, FF1Text.Delimiter.Empty), Blob.FromHex("101003B26004919900"),
			};

			pages.Add(FF1Text.TextToStory(VictoryMessages[rng.Between(0, VictoryMessages.Count - 1)]));
			pages.Add(Blob.Concat(movementStats));
			pages.Add(Blob.Concat(battleResults));
			pages.Add(Blob.Concat(combatStats));
			pages.Add(FF1Text.TextToStory(ThankYou));

			Blob storyText = PackageTextBlob(pages, 0xA800);
			System.Diagnostics.Debug.Assert(storyText.Length <= 0x0500, "Story text too large!");

			Put(0x36800, storyText);
			Data[0x36E00] = (byte)(BridgeStory.Count);
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
			texts.ForEach(text => pages.Add(FF1Text.TextToCredits(text)));

			// Clobber the number of pages to render before we insert in the pointers.
			Data[0x37873] = (byte)pages.Count;

			Blob credits = PackageTextBlob(pages, 0xBB00); 
			System.Diagnostics.Debug.Assert(credits.Length <= 0x0100, "Credits too large: " + credits.Length);
			Put(0x37B00, credits);
		}

		private Blob PackageTextBlob(List<Blob> pages, ushort baseAddr)
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
