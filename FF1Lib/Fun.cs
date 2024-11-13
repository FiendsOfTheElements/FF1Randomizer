using System.ComponentModel;

namespace FF1Lib
{
	public enum MusicShuffle
	{
		[Description("None")]
		None = 0,
		[Description("Standard")]
		Standard,
		[Description("Nonsensical")]
		Nonsensical,
		[Description("Disable Music")]
		MusicDisabled
	}

	public enum MenuColor
	{
		[Description("Default Blue")]
		Blue = 0x01,
		[Description("Dark Blue")]
		DarkBlue = 0x02,
		[Description("Purple")]
		Purple = 0x03,
		[Description("Pink")]
		Pink = 0x04,
		[Description("Red")]
		Red = 0x05,
		[Description("Orange")]
		Orange = 0x06,
		[Description("Dark Orange")]
		DarkOrange = 0x07,
		[Description("Brown")]
		Brown = 0x08,
		[Description("Light Green")]
		LightGreen = 0x09,
		[Description("Green")]
		Green = 0x0A,
		[Description("Dark Green")]
		DarkGreen = 0x0B,
		[Description("Cyan")]
		Cyan = 0x0C,
		[Description("Black")]
		Black = 0x0F,
	}

	public enum MapmanSlot
	{
		[Description("Leader")]
		Leader = 0x00,
		[Description("Second")]
		Second = 0x01,
		[Description("Third")]
		Third = 0x02,
		[Description("Fourth")]
		Fourth = 0x03,
	}
	public enum TitanSnack
	{
		[Description("Ruby")]
		Ruby = 0,
		[Description("Other Minerals")]
		Minerals = 1,
		[Description("Junk Food")]
		Junk = 2,
		[Description("Healthy Food")]
		Healthy = 3,
		[Description("Beverages")]
		Beverages = 4,
		[Description("All")]
		All = 5,
	}

	public enum Fate
	{
		[Description("Spare")]
		Spare = 0,
		[Description("Kill")]
		Kill = 2,
	}
	public partial class FF1Rom
	{
		public const int TyroPaletteOffset = 0x30FC5;
		public const int TyroSpriteOffset = 0x20560;

		public const int PaletteOffset = 0x30F20;
		public const int PaletteSize = 4;
		public const int PaletteCount = 64;

	    public void FunEnemyNames(Flags flags, Preferences preferences, MT19337 rng)
		{
			bool teamSteak = preferences.TeamSteak;
			bool oldTeamSteak = preferences.OldTeamSteak;
			bool altFiends = (bool)flags.AlternateFiends;

			if (!preferences.FunEnemyNames || flags.EnemizerEnabled)
			{
				return;
			}

			// ReadEnemyText() and WriteEnemyText() use some extra room in the ROM,
			// so no need to worry about the number of bytes in the names now.
			var enemyText = ReadEnemyText();

			enemyText[1] = "GrUMP";    // +0  GrIMP
			enemyText[2] = "RURURU";   // +2  WOLF
			enemyText[3] = "GrrrWOLF"; // +2  GrWOLF
			enemyText[5] = "BrrrWOLF"; // +2  FrWOLF
			if (teamSteak && !oldTeamSteak)
			{
				enemyText[23] = "CREPE";
				enemyText[24] = "CRULLER";
			}
			enemyText[28] = "GeORGE";  // +0  GrOGRE

			// "WzOGRE"
			if (rng.Between(1, 10) >= 5) {
			    enemyText[29] = "DIRGE";  // -1
			} else {
			    enemyText[29] = "GROVER"; // +0
			}

			enemyText[30] = "R.SNEK";     // +3  ASP
			enemyText[31] = "GrSNEK";     // +1  COBRA
			enemyText[32] = "SeaSNEK";    // -1  SeaSNAKE
			enemyText[40] = "iMAGE";      // +0  IMAGE
			if (teamSteak && !oldTeamSteak)
			{
				enemyText[47] = "GRUB";		// WORM
				enemyText[49] = "MealWORM";	// Grey W
			}
			enemyText[48] = "SANDWICH";   // +2  Sand W
			enemyText[51] = "WrongEYE";   //     Phantom
			enemyText[53] = "SNEKLADY";   // +0  GrMEDUSA
			enemyText[56] = "EXPEDE";     // +2  PEDE
			enemyText[61] = "EDWARD";     // +0  WzVAMP
			enemyText[63] = "ARGYLE";     // -1  R.GOYLE
			enemyText[66] = "White D";    // +0  Frost D
			enemyText[72] = "MtlSLIME";   // +3  SLIME
			enemyText[77] = "FnPOLICE";   // +0  R.ANKYLO
			enemyText[80] = "MOMMY";      // -2  WzMUMMY
			enemyText[81] = "BIRB";       // -4  COCTRICE
			enemyText[82] = "R.BIRB";     // -2  PERILISK
			if (teamSteak && !oldTeamSteak)
			{
				enemyText[83] = "WYNGS"; 	// WYVERN
				enemyText[84] = "HotWYNGS";	// WYRM
				enemyText[91] = "GUAC";		// OCHO
			}
			else
			{
				enemyText[83] = "Y BURN";	// +0  WYVERN
			}
			if (teamSteak)
			{
				enemyText[85] = "STEAK";  // +1  TYRO
				enemyText[86] = "T.BONE"; // +1  T REX
			}

			enemyText[92] = "NACHO";      // -1  NAOCHO
			enemyText[94] = "HYDRANT";    // +0  R.HYDRA
			enemyText[100] = "LadySNEK";  // +2  GrNAGA
			enemyText[106] = "Green D";   // +2  Gas D
			enemyText[111] = "BATMAN";    // +0  BADMAN
			enemyText[112] = "OKAYMAN";   // +0  EVILMAN
			if (!altFiends) {
			    enemyText[119] = "S.BUMP";    // +2  LICH
			    enemyText[120] = "S.BUMP";    // +2  LICH
			    enemyText[121] = "KELLY";     // +1  KARY
			    enemyText[122] = "KELLY";     // +1  KARY
			}

			
			WriteEnemyText(enemyText);
		}

		public void PaletteSwap(bool enable, MT19337 rng)
		{
			if (!enable)
			{
				return;
			}

			var palettes = Get(PaletteOffset, PaletteSize * PaletteCount).Chunk(PaletteSize);

			palettes.Shuffle(rng);

			Put(PaletteOffset, Blob.Concat(palettes));
		}

		public void OldTeamSteak(bool enable)
		{
			if (!enable)
			{
				return;
			}

			Put(TyroPaletteOffset, Blob.FromHex("302505"));
			Put(TyroSpriteOffset, Blob.FromHex(
				"00000000000000000000000000000000" + "00000000000103060000000000000001" + "001f3f60cf9f3f7f0000001f3f7fffff" + "0080c07f7f87c7e60000008080f8f8f9" + "00000080c0e0f0780000000000000080" + "00000000000000000000000000000000" +
				"00000000000000000000000000000000" + "0c1933676f6f6f6f03070f1f1f1f1f1f" + "ffffffffffffffffffffffffffffffff" + "e6e6f6fbfdfffffff9f9f9fcfefefefe" + "3c9e4e26b6b6b6b6c0e0f0f878787878" + "00000000000000000000000000000000" +
				"00000000000000000000000000000000" + "6f6f6f6f673b190f1f1f1f1f1f070701" + "fffffec080f9fbffffffffffff8787ff" + "ff3f1f1f3ffdf9f3fefefefefefefefc" + "b6b6b6b6b6b6b6b67878787878787878" + "00000000000000000000000000000000" +
				"00000000000000000000000000000000" + "07070706060707070100000101010101" + "ffffff793080c0f0fffc3086cfffffff" + "e7fefcf9f26469e3f80103070f9f9e1c" + "264c983060c08000f8f0e0c080000000" + "00000000000000000000000000000000" +
				"00000000000000000000000000000000" + "07070706060301010101010101000000" + "f9f9f9797366ece8fefefefefcf97377" + "c68c98981830606038706060e0c08080" + "00000000000000000000000000000000" + "00000000000000000000000000000000" +
				"00000000000000000000000000000000" + "01010101010000000000000000000000" + "fb9b9b9b98ff7f006767676767000000" + "6060606060c080008080808080000000" + "00000000000000000000000000000000" + "00000000000000000000000000000000"));
		}

		public void DynamicWindowColor(MenuColor menuColor)
		{
			// This is an overhaul of LoadBorderPalette_Blue that enhances it to JSR to
			// DrawMapPalette first. That allows us to wrap that with a dynamic load of
			// the bg color after it sets it to the default one.
			/*
				LoadBorderPalette_Dynamic:
				JSR $D862 ; JSR to DrawMapPalette
				LDY $60FB ; Load dynamic palette color to Y

				LoadBorderPalette_Y:
				LDA $60FC ; Back up current bank
				PHA
				LDA #$0F
				JSR $FE03 ; SwapPRG_L
				JSR $8700 ; Jump out to palette writing code. Dynamic Color in Y
				PLA
				JSR $FE03 ; SwapPRG_L
				RTS
			*/
			Put(0x7EB29, Blob.FromHex("2062D8ACFB60ADFC6048A90F2003FE200087682003FE60"));

			// The battle call site needs black not the dynamic color so we jump right to
			// the operation after that when calling from battle init.
			Put(0x7EB90, Blob.FromHex("A00F4C2FEB"));

			// Modify two calls to DrawMapPalette to call our LoadBorderPalette_Dynamic which
			// starts with a JSR to DrawMapPalette and then adds the dynamic menu color.
			Put(0x7CF8F, Blob.FromHex("29EB"));
			Put(0x7CF1C, Blob.FromHex("29EB"));

			// Modify Existing calls to LoadBorderPalette_Blue up three bytes to where it starts
			Put(0x7EAB7, Blob.FromHex("2CEB"));
			Put(0x7EB58, Blob.FromHex("2CEB"));

			// There are two unfinished bugs in the equipment menu that use palettes 1 and 2
			// for no reason and need to use 3 now. They are all mirrors in vanilla.
			Put(0x3BE53, Blob.FromHex("EAEAEAEAEAEAEAEAEAEA"));
			Data[0x3BEF7] = 0x60;

			// Finally we need to also make the lit orb palette dynamic so lit orbs bg matches.
			// I copy the original code and add LDA/STA at the end for the bg color, and put
			// it over some unused garbage at the bottom of Bank E @ [$BF3A :: 0x3BF4A]
			/*
				LDX #$0B ; Straight copy from EnterMainMenu
				Loop:
				  LDA $AD78, X
				  STA $03C0, X
				  DEX
				  BPL Loop

				LDA $60FB ; Newly added to load and set dynamic palette color for lit orb
				STA $03C2
				RTS
			*/
			Put(0x3BF3A, Blob.FromHex("A20BBD78AD9DC003CA10F7ADFB608DC2038DC60360"));
			Put(0x3ADC2, Blob.FromHex("203ABFEAEAEAEAEAEAEAEA"));

			// Dynamic location initial value
			Data[0x30FB] = (byte)menuColor;

			// Hardcoded spot for opening "cinematic"
			Data[0x03A11C] = (byte)menuColor;
			Data[0x03A2D3] = (byte)menuColor;
		}

		public void EnableModernBattlefield(bool enable)
		{
			if (!enable)
			{
				return;
			}

			// Since we're changing the window color in the battle scene we need to ensure that
			// $FF tile remains opaque like in the menu screen. That battle init code
			// overwrites it with transparent so we skip that code here. Since this is fast
			// enough we end up saving a frame we add a wait for VBlank to take the same total time.
			Put(0x7F369, Blob.FromHex("2000FEEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));
			Put(0x7EB90, Blob.FromHex("4C29EB"));

			// Don't draw big battle boxes around the enemies, party, and individual stats.
			// Instead draw one box in the lower right corner around the new player stats.
			Put(0x7F2E4, Blob.FromHex("A9198538A913A206A00A20E2F3EAEAEAEAEAEAEAEAEAEAEAEAEA"));
			Put(0x7F2FB, Blob.FromHex("EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));

			// The bottom row of these boxes was occluded by the Command Menu and enemy list so
			// there is code to redraw it whenever it would be exposed that we early return to skip.
			Data[0x7F62D] = 0x60;

			// To match later games and make better use of screen real estate we move all the bottom
			// boxes down a tile. This requires rewriting most of the battle box and text positioning
			// LUTs. They are largely formatted in a HeaderByte, X, Y, W, H system.
			// lut_EnemyRosterBox      HDXXYYWWHH
			Put(0x7F9E4, Blob.FromHex("0001010B0A"));

			//                           BOX      TEXT
			// lut_CombatBoxes:        HDXXYYWWHHHDXXYY
			Put(0x7F9E9, Blob.FromHex("0001010A04010202" + // attacker name
									  "000B010C04010C02" + // their attack("FROST", "2Hits!" etc)
									  "0001040A04010205" + // defender name
									  "000B040B04010C05" + // damage
									  "0001071804010208"));// bottom message("Terminated", "Critical Hit", etc)

			// lut_BattleCommandBox    HDXXYYWWHH
			Put(0x7FA1B, Blob.FromHex("000C010D0A"));

			// lut for Command Text    HDXXYYTPTR
			Put(0x7FA20, Blob.FromHex("010E0239FA" + // FIGHT
									  "010E043EFA" + // MAGIC
									  "010E0643FA" + // DRINK
									  "010E0848FA" + // ITEM
									  "0114024DFA"));// RUN

			// Edit lut for where to draw character status
			Put(0x32B21, Blob.FromHex("9A22DA221A235A23"));

			// Move character sprites to the right edge and space them a little apart
			byte xCoord = 0xD8;
			Data[0x31741] = xCoord;
			Data[0x3197D] = xCoord;                // overwrite hardcoded when stoned
			Data[0x31952] = (byte)(xCoord - 0x08); // 8px to the left when dead.

			byte yCoord = 0x2B;
			for (int i = 0; i < 4; ++i)
			{
				Data[0x3174F + (i * 5)] = (byte)(yCoord + (i * 28));
			}

			// Update Several 16 Byte LUTS related to cursors
			for (int i = 0; i < 8; ++i)
			{
				Data[0x31F85 + i * 2] = (byte)(xCoord - 0x10);            // X Coord of Character targeting cursor
				Data[0x31F86 + i * 2] = (byte)(yCoord + 4 + ((i % 4) * 0x1C));  // Y Coord of Character targeting cursor
				Data[0x31F76 + i * 2] = (byte)(0xA7 + (Math.Min(i, 4) % 4 * 0x10)); // Y Coord of Command Menu cursor (last four are identical)
			}

			// Start backdrop rows two tiles left and one upward. This matches FF2/3
			Data[0x7F34B] = 0x20;
			Data[0x7F352] = 0x40;
			Data[0x7F359] = 0x60;
			Data[0x7F360] = 0x80;

			// Adjust backdrop to draw in sets of 8 tiles so it appears to keep repeating.
			Put(0x7F38C, Blob.FromHex("A008849B20A0F320A0F320A0F3EAEAEAEA"));

			// Shorten the DrawStatus section to print only name or status, not both. Just one line.
			/* ASM Snippet
				LDY #$01        ; Need a one offset into
				LDA ($82), Y    ; btl_ob_charstat_ptr + 1 is status
				LSR             ; Shift dead bit to carry
				BCS skip        ; If dead print name
				BEQ skip        ; If healthy print name
				LDY #$09        ; otherwise load 9 to print status string
				skip:
				JSR $AAFC       ; JSR DrawStatusRow
			*/
			Put(0x32AB0, Blob.FromHex("A001B1824AB004F002A00920FCAAEAEAEAEAEAEA"));

			// Overwrite the upper portion of the default attribute table to all bg palette
			Put(0x7F400, Blob.FromHex("0000000000000000"));
			Put(0x7F408, Blob.FromHex("0000000000000000"));

			// Fix NT bits inside the drawing sequence of mixed enemies for expanded backdrop.
			// Before it would reset the palette to greyscale because it used to be borders.
			Data[0x2E6C9] = 0x70;
			Data[0x2E6CD] = 0xB0;

			// Fix NT bits inside chaostsa.bin and fiendtsa.bin
			Data[0x2D4C8] = 0xB0;
			for (int i = 0; i < 0x0140; i += 0x50)
			{
				Data[0x2D320 + i] = 0x00;
			}
		}

		public void UseVariablePaletteForCursorAndStone(bool enable)
		{
			if (!enable)
			{
				return;
			}

			// The masamune uses the same palette as the cursor and stone characters
			// so we can free up a whole palette if we reset the varies palette to
			// the masamune palette after every swing and magic annimation. The only
			// drawback is that stoned characters will flash with attacks and magic.

			// Change UpdateVariablePalette to edit Palette 3
			Data[0x32B35] = 0xA4;
			Data[0x32B3B] = 0xA5;
			Data[0x32B41] = 0xA6;
			Data[0x32B46] = 0xA3;
			Data[0x32B4E] = 0xA6;

			// Make magic use palette 3
			Data[0x318F0] = 0x03;

			// Make sparks use palette 3
			Data[0x33E47] = 0x03;

			// Weapon palettes are embedded in this lut with their coordinates.
			Put(0x3202C, Blob.FromHex("0001020303030303"));
			Put(0x32034, Blob.FromHex("0100030243434343"));

			// Increase loop variable to do 12 colors when fading sprites in and out for inn animation.
			Data[0x7FF23] = 12;
			Data[0x7FF43] = 12;
			Data[0x7FF65] = 12;

			// Enable this feature by rewriting the JSR BattleFrame inside UpdateSprites_BattleFrame
			Put(0x31904, Blob.FromHex("20F1FD207CA060"));
		}
		public void ChangeLute(bool changelute, DialogueData dialogues, MT19337 rng)
		{
			if (!changelute)
			{
				return;
			}

			var newInstruments = new List<string> {"BASS", "LYRE", "HARP", "VIOLA", "CELLO", "PIANO", "ORGAN", "FLUTE", "OBOE", "PICCOLO", "FLUTE", "WHISTLE", "HORN", "TRUMPET",
				"BAGPIPE", "DRUM", "VIOLIN", "DBLBASS", "GUITAR", "BANJO", "FIDDLE", "MNDOLIN", "CLARNET", "BASSOON", "TROMBON", "TUBA", "BUGLE", "MARIMBA", "XYLOPHN","SNARE D",
				"BASSDRM", "TMBRINE", "CYMBALS", "TRIANGL", "COWBELL", "GONG", "TRUMPET", "SAX", "TIMPANI", "B GRAND", "HRDYGRD", "FLUGEL", "SONG", "KAZOO", "FOGHORN", "AIRHORN",
				"VUVUZLA", "OCARINA", "PANFLUT", "SITAR", "HRMNICA", "UKULELE", "THREMIN", "DITTY", "JINGLE", "LIMRICK", "POEM", "HAIKU", "OCTBASS", "HRPSCRD", "FLUBA", "AEOLUS",
				"TESLA", "STLDRUM", "DGERIDO", "WNDCHIM" };

			//var dialogs = ReadText(dialogsPointerOffset, dialogsPointerBase, dialogsPointerCount);

			var newLute = newInstruments.PickRandom(rng);
			// handle extra dialogues that might contain the LUTE if the NPChints flag is enabled or if Astos Shuffle is enabled
			var dialogsUpdate = SubstituteKeyItemInExtraNPCDialogues("LUTE", newLute, dialogues); ;
			var princessDialogue = dialogues[0x06].Split(new string[] { "LUTE" }, System.StringSplitOptions.RemoveEmptyEntries);
			var monkDialogue = dialogues[0x35].Split(new string[] { "LUTE" }, System.StringSplitOptions.RemoveEmptyEntries);

			if (princessDialogue.Length > 1)
				dialogsUpdate.Add(0x06, princessDialogue[0] + newLute + princessDialogue[1]);

			if (monkDialogue.Length > 1)
				dialogsUpdate.Add(0x35, monkDialogue[0] + newLute + monkDialogue[1].Substring(0,14) + "\n" + monkDialogue[1].Substring(15, 10).Replace('\n',' '));

			if (dialogsUpdate.Count > 0)
				dialogues.InsertDialogues(dialogsUpdate);

			ItemsText[(int)Item.Lute] = newLute;
		}

		public void HurrayDwarfFate(Fate fate, NpcObjectData npcdata, DialogueData dialogues, MT19337 rng)
		{
			if (fate == Fate.Spare)
			{
				// Protect Hurray Dwarf from NPC guillotine
				npcdata[ObjectId.DwarfcaveDwarfHurray].Script = TalkScripts.Talk_norm;
			}
			else
			{
				// Whether NPC guillotine is on or not, kill Hurray Dwarf
				npcdata[ObjectId.DwarfcaveDwarfHurray].Script = TalkScripts.Talk_kill;

				// Change the dialogue
				var dialogueStrings = new List<string>
				{
				    "No! I'm gonna disappear.\nYou'll never see\nme again. Please,\nI don't want to die.",
					"If you strike me down,\nI shall become more\npowerful than you can\npossibly imagine.",
					"Freeeeedom!!",
					"I've seen things you\npeople wouldn't believe.\nAll those moments will\nbe lost in time..\nlike tears in rain..\nTime to die.",
					"Become vengeance, David.\nBecome wrath.",
					"My only regret..\nis that I have boneitis.",
					"No, not the bees!\nNOT THE BEES!\nAAAAAAAAGH!\nTHEY'RE IN MY EYES!\nMY EYES! AAAAAAAAAAGH!",
					"This is blasphemy!\nThis is madness!",
					"Not like this..\nnot like this..",
					"Suicide squad, attack!\n\n\n\nThat showed 'em, huh?",
					"Well, what are you\nwaiting for?\nDo it. DO IT!!",
					"The path you walk on has\nno end. Each step you\ntake is paved with the\ncorpses of your enemies.\nTheir souls will haunt\nyou forever. Hear me!\nMy spirit will be\nwatching you!",
					"K-Kefka..!\nY-you're insane.."
				};

				//Put new dialogue to E6 since another Dwarf also says hurray
				dialogues[0xE6] = dialogueStrings.PickRandom(rng);
				npcdata[ObjectId.DwarfcaveDwarfHurray].Dialogue1 = 0xE6;
				npcdata[ObjectId.DwarfcaveDwarfHurray].Dialogue2 = 0xE6;
				npcdata[ObjectId.DwarfcaveDwarfHurray].Dialogue3 = 0xE6;
			}
		}

		public void TitanSnack(TitanSnack snack, NpcObjectData npcdata, DialogueData dialogues, MT19337 rng)
		{
			if (snack == FF1Lib.TitanSnack.Ruby)
			{
				return;
			}

			var snackOptions = new List<string>(); // { "NEWRUBY(max 7 characters);NEWRUBYPLURALIZED(max 8 characters);IS/ARE(relating to plural form);DESCRIPTOR(max 6 characters);ONOMATOPOEIA(max 6 chars, how ingestion sounds)" }
			var mineralSnacks = new List<string> { "DIAMOND;DIAMONDS;ARE;SWEET;CRUNCH", "GEODE;GEODES;ARE;SWEET;CRUNCH", "COAL;COAL;IS;SMOKY;CRUNCH", "PEARL;PEARLS;ARE;SWEET;CRUNCH", "FOSSIL;FOSSILS;ARE;SWEET;CRUNCH", "EMERALD;EMERALDS;ARE;SWEET;CRUNCH", "TOPAZ;TOPAZ;IS;SWEET;CRUNCH", "QUARTZ;QUARTZ;IS;SWEET;CRUNCH", "ONYX;ONYXES;ARE;SWEET;CRUNCH", "MARBLE;MARBLE;IS;SWEET;CRUNCH", "AMETHST;AMETHST;IS;SWEET;CRUNCH", "JADE;JADES;ARE;SWEET;CRUNCH", "SAPHIRE;SAPHIRE;IS;SWEET;CRUNCH", "GRANITE;GRANITE;IS;SWEET;CRUNCH", "OBSDIAN;OBSDIAN;IS;SWEET;CRUNCH", "CONCRET;CONCRET;IS;SALTY;CRUNCH", "ASPHALT;ASPHALT;IS;SALTY;CRUNCH", "PUMICE;PUMICE;IS;SWEET;CRUNCH", "LIMESTN;LIMESTN;IS;SOUR;CRUNCH", "SNDSTON;SNDSTON;IS;SALTY;CRUNCH", "MYTHRL;MYTHRL;IS;SWEET;CRUNCH" };
			var junkFoodSnacks = new List<string> { "DANISH;DANISHES;ARE;SWEET;MUNCH", "HOT DOG;HOT DOGS;ARE;GREAT;MUNCH", "TACO;TACOS;ARE;GREAT;MUNCH", "SUB;SUBS;ARE;GREAT;MUNCH", "PIZZA;PIZZA;IS;YUMMY;MUNCH", "BURGER;BURGERS;ARE;YUMMY;MUNCH", "EGGROLL;EGGROLLS;ARE;YUMMY;MUNCH", "BISCUIT;BISCUITS;ARE;YUMMY;MUNCH", "WAFFLE;WAFFLES;ARE;YUMMY;MUNCH", "CAKE;CAKE;IS;SWEET;MUNCH", "PIE;PIE;IS;SWEET;MUNCH", "DONUT;DONUTS;ARE;SWEET;MUNCH", "FRIES;FRIES;ARE;SALTY;MUNCH", "CHIPS;CHIPS;ARE;SALTY;CRUNCH", "CANDY;CANDY;IS;SWEET;MUNCH", "PANCAKE;PANCAKES;ARE;SWEET;MUNCH", "ICE CRM;ICE CRM;IS;CREAMY;MUNCH", "PUDDING;PUDDING;IS;YUMMY;MUNCH", "BROWNIE;BROWNIES;ARE;SWEET;MUNCH", "CRAYON;CRAYONS;ARE;WEIRD;MUNCH", "GLUE;GLUE;IS;WEIRD;MUNCH", "PASTE;PASTE;IS;WEIRD;MUNCH", "LASAGNA;LASAGNA;IS;YUMMY;MUNCH", "POUTINE;POUTINE;IS;GREAT;MUNCH", "PASTA;PASTA;IS;YUMMY;MUNCH", "RAMEN;RAMEN;IS;GREAT;MUNCH", "STEAK;STEAK;IS;GREAT;MUNCH", "NACHOS;NACHOS;ARE;SALTY;CRUNCH", "BACON;BACON;IS;SALTY;MUNCH", "MUTTON;MUTTON;IS;GREAT;MUNCH", "BAGEL;BAGELS;ARE;GREAT;MUNCH", "CHEESE;CHEESE;IS;GREAT;MUNCH", "POPCORN;POPCORN;IS;SALTY;MUNCH", "CHICKEN;CHICKEN;IS;GREAT;MUNCH", "BEEF;BEEF;IS;GREAT;MUNCH", "HAM;HAM;IS;GREAT;MUNCH", "BOLOGNA;BOLOGNA;IS;GREAT;MUNCH", "HOAGIE;HOAGIES;ARE;GREAT;MUNCH", "FILET;FILET;IS;DIVINE;MUNCH", "LOBSTER;LOBSTER;IS;DIVINE;MUNCH", "SHEPPIE;SHEPPIE;IS;SAVORY;MUNCH", "MEATLOF;MEATLOF;IS;SAVORY;MUNCH", "ENCHLDA;ENCHLDAS;ARE;CHEESY;MUNCH", "BAKLAVA;BAKLAVAS;ARE;SWEET;MUNCH","CANNOLI;CANNOLI;ARE;SWEET;CRUNCH","TIRMISU;TIRMISU;IS;SWEET;MUNCH","CHZQAKE;CHZQAKE;IS;CREAMY;MUNCH","PIEROGI;PIEROGIES;ARE;YUMMY;MUNCH","KEBAB;KEBABS;ARE;YUMMY;MUNCH","KOFTE;KOFTE;IS;YUMMY;MUNCH" };
			var healthySnacks = new List<string> { "EDAMAME;EDAMAME;IS;SALTY;MUNCH", "SALAD;SALAD;IS;GREAT;MUNCH", "APPLE;APPLES;ARE;SWEET;CRUNCH", "PEAR;PEARS;ARE;SWEET;MUNCH", "MELON;MELONS;ARE;SWEET;MUNCH", "ORANGE;ORANGES;ARE;SWEET;MUNCH", "LEMON;LEMONS;ARE;SOUR;MUNCH", "YOGURT;YOGURT;IS;GREAT;MUNCH", "GRANOLA;GRANOLA;IS;GREAT;CRUNCH", "SPINACH;SPINACH;IS;YUMMY;MUNCH", "EGG;EGGS;ARE;YUMMY;MUNCH", "GRAPES;GRAPES;ARE;YUMMY;MUNCH", "OATMEAL;OATMEAL;IS;GREAT;MUNCH", "TOFU;TOFU;IS;WEIRD;MUNCH", "CABBAGE;CABBAGE;IS;FRESH;MUNCH", "LETTUCE;LETTUCE;IS;FRESH;MUNCH", "TOMATO;TOMATOES;ARE;YUMMY;MUNCH", "SUSHI;SUSHI;IS;FISHY;MUNCH", "TUNA;TUNA;IS;FISHY;MUNCH", "SALMON;SALMON;IS;FISHY;MUNCH", "FISH;FISH;IS;FRESH;MUNCH", "BEANS;BEANS;ARE;YUMMY;MUNCH", "CEREAL;CEREAL;IS;GREAT;MUNCH", "PRETZEL;PRETZELS;ARE;SALTY;MUNCH", "EGGSALD;EGGSALAD;IS;GREAT;MUNCH", "RICE;RICE;IS;PLAIN;MUNCH", "CAVIAR;CAVIAR;IS;DIVINE;MUNCH" };
			var beverages = new List<string> { "BEER;BEER;IS;SMOOTH;GULP", "WINE;WINE;IS;RICH;GULP", "TEA;TEA;IS;FRESH;GULP", "COFFEE;COFFEE;IS;FRESH;GULP", "COLA;COLA;IS;SWEET;GULP", "COCOA;COCOA;IS;SWEET;GULP", "ICEDTEA;ICEDTEA;IS;SWEET;GULP", "LMONADE;LEMONADE;IS;SWEET;GULP", "MILK;MILK;IS;GREAT;GULP", "LATTE;LATTES;ARE;CREAMY;GULP", "WATER;WATER;IS;FRESH;GULP", "TEQUILA;TEQUILA;IS;SMOOTH;GULP" };

			switch (snack)
			{
				case FF1Lib.TitanSnack.Minerals:
					snackOptions = mineralSnacks;
					break;
				case FF1Lib.TitanSnack.Junk:
					snackOptions = junkFoodSnacks;
					break;
				case FF1Lib.TitanSnack.Healthy:
					snackOptions = healthySnacks;
					break;
				case FF1Lib.TitanSnack.Beverages:
					snackOptions = beverages;
					break;
				case FF1Lib.TitanSnack.All:
					// combine all lists together
					foreach (string mineral in mineralSnacks) { snackOptions.Add(mineral); }
					foreach (string junkFood in junkFoodSnacks) { snackOptions.Add(junkFood); }
					foreach (string healthySnack in healthySnacks) { snackOptions.Add(healthySnack); }
					foreach (string beverage in beverages) { snackOptions.Add(beverage); }
					break;
				default:
					return;
			}
			var randomRuby = snackOptions.PickRandom(rng);

			var newRubyItemDescription = "A tasty treat."; // Replaces "A large red stone." (can't be too long else it'll overwrite next phrase: "The plate shatters,")
			var newTitanCraving = "is hungry."; // replaces "eat gems." (can't be too long or will appear outside window)
			if (beverages.Contains(randomRuby))
			{
				newRubyItemDescription = "A tasty drink.";
				newTitanCraving = "is thirsty.";
			}
			else if (mineralSnacks.Contains(randomRuby))
			{
				newRubyItemDescription = "Feels heavy.";
			}
			// replace "A red stone." item description (0x38671) originally "8AFFAF2FAA1A23A724285AC000"
			MenuText.MenuStrings[(int)FF1Text.MenuString.UseRuby] = FF1Text.TextToBytes(newRubyItemDescription, useDTE: true);

			// phrase parts
			var newRubyContent = randomRuby.Split(";");
			var newRuby = newRubyContent[0];
			var newRubyPluralized = newRubyContent[1];
			var newRubySubjectVerbAgreement = newRubyContent[2];
			var newRubyTastes = newRubyContent[3];
			var newRubyOnomatopoeia = newRubyContent[4];
			var newRubyArticle = ""; // newRubySubjectVerbAgreement;
			if (newRubySubjectVerbAgreement == "ARE")
			{
				if (newRuby[0] == 'A' || newRuby[0] == 'E' || newRuby[0] == 'I' || newRuby[0] == 'O' || newRuby[0] == 'U')
				{
					newRubyArticle = "an ";
				}
				else
				{
					newRubyArticle = "a ";
				}
			}

			// handle extra dialogues that might contain the RUBY if the NPChints flag is enabled
			var dialogsUpdate = SubstituteKeyItemInExtraNPCDialogues("RUBY", newRuby, dialogues);

			// begin substitute phrase parts
			var titanDeepDungeon = dialogues[0x29].Split(new string[] { "a RUBY" }, System.StringSplitOptions.RemoveEmptyEntries);
			var titanDialogue = dialogues[0x2A].Split(new string[] { "RUBY", "Crunch, crunch, crunch,", "sweet", "Rubies are" }, System.StringSplitOptions.RemoveEmptyEntries);
			var melmondManDialogue = dialogues[0x7B].Split(new string[] { "eats gems.", "RUBIES" }, System.StringSplitOptions.RemoveEmptyEntries);

			// Bring me a {newRuby} if you
			// wish to skip to floor 22.
			if (titanDeepDungeon.Length > 1)
			{
				dialogsUpdate.Add(0x29, titanDeepDungeon[0] + newRubyArticle + newRuby + titanDeepDungeon[1]);
			}

			// If you want pass, give
			// me the {newRuby}..
			// {Onomatopoeia}, {onomatopoeia}, {onomatopoeia},
			// mmm, it tastes so {newRubyTastes}.
			// {newRubyPluralized} {newRubySubjectVerbAgreement} my favorite.
			if (titanDialogue.Length > 3)
			{
				dialogsUpdate.Add(0x2A, titanDialogue[0] + newRuby + titanDialogue[1]
					+ CapitalizeFirstLowercaseRest(newRubyOnomatopoeia) + ", " + newRubyOnomatopoeia.ToLower() + ", " + newRubyOnomatopoeia.ToLower() + "," + titanDialogue[2]
					+ newRubyTastes.ToLower() + titanDialogue[3]
					+ CapitalizeFirstLowercaseRest(newRubyPluralized) + " " + newRubySubjectVerbAgreement.ToLower() + titanDialogue[4]);
			}
			else if (titanDialogue.Length > 1)
			{
				// handle Shuffle Astos, alternate Titan dialog "If you want pass, give\nme the RUBY..\nHa, it mine! Now, you in\ntrouble. Me am Astos,\nKing of the Titans!"
				dialogsUpdate.Add(0x2A, titanDialogue[0] + newRuby + titanDialogue[1]);
			}
			// The Titan who lives in
			// the tunnel {newTitanCraving}
			// He loves {newRubyPluralized}.
			if (melmondManDialogue.Length > 2)
				dialogsUpdate.Add(0x7B, melmondManDialogue[0] + newTitanCraving + melmondManDialogue[1] + newRubyPluralized + melmondManDialogue[2]);
			// end substitute phrase parts

			if (dialogsUpdate.Count > 0)
				dialogues.InsertDialogues(dialogsUpdate);

			// substitute key item
			ItemsText[(int)Item.Ruby] = newRuby;
		}

		private Dictionary <int,String> SubstituteKeyItemInExtraNPCDialogues(string original, string replacement, DialogueData dialogues)
		{
			var dialogsUpdate = new Dictionary<int, string>();
			// Add extra dialogues that might contain the {original} if the NPChints flag is enabled or if Astos Shuffle is enabled
			var otherNPCs = new List<byte> {
				0x45, 0x53, 0x69, 0x82, 0xA0, 0xAA, 0xCB, 0xDC, 0x9D, 0x70, 0xE3, 0xE1, 0xB6, // NPChints
				0x02, 0x0E, 0x12, 0x14, 0x16, 0x19, 0x1E, 0xCD, 0x27, 0x23, 0x2B // ShuffleAstos
			};

			for (int i = 0; i < otherNPCs.Count(); i++)
			{
				var tempDialogue = dialogues[otherNPCs[i]].Split(new string[] { original.ToUpper().Trim() }, System.StringSplitOptions.RemoveEmptyEntries);
				if (tempDialogue.Length > 1)
					dialogsUpdate.Add(otherNPCs[i], tempDialogue[0] + replacement.ToUpper().Trim() + tempDialogue[1]);
			}

			return dialogsUpdate;
		}

		private static string CapitalizeFirstLowercaseRest(string s)
		{
			return String.Format("{0}{1}", s.First().ToString().ToUpper(), s.Substring(1).ToLower());
		}

	}

}
