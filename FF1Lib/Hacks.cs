using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace FF1Lib
{
	public enum PoolSize
	{
		[Description("4 characters")]
		Size4,
		[Description("5 characters")]
		Size5,
		[Description("6 characters")]
		Size6,
		[Description("7 characters")]
		Size7,
		[Description("8 characters")]
		Size8,
	}

	public enum spellDataBytes
	{
		Accuracy,
		Effect,
		Element,
		Target,
		Routine,
		GFX,
		Palette,
		Null
	}
	public partial class FF1Rom : NesRom
	{
		public const int Nop = 0xEA;
		public const int SardaOffset = 0x393E9;
		public const int SardaSize = 7;
		public const int CanoeSageOffset = 0x39482;
		public const int CanoeSageSize = 5;
		public const int PartyShuffleOffset = 0x312E0;
		public const int PartyShuffleSize = 3;
		public const int MapSpriteOffset = 0x03400;
		public const int MapSpriteSize = 3;
		public const int MapSpriteCount = 16;

		public const int CaravanFairyCheck = 0x7C4E5;
		public const int CaravanFairyCheckSize = 7;

		public const string BattleBoxDrawInFrames = "06"; // Half normal (Must divide 12)
		public const string BattleBoxDrawInRows = "02";

		public const string BattleBoxUndrawFrames = "04"; // 2/3 normal (Must  divide 12)
		public const string BattleBoxUndrawRows = "03";

		// Required for npc quest item randomizing
		public void PermanentCaravan()
		{
			Put(CaravanFairyCheck, Enumerable.Repeat((byte)Nop, CaravanFairyCheckSize).ToArray());
		}


		public void EnableEarlySarda(NPCdata npcdata)
		{
			npcdata.GetTalkArray(ObjectId.Sarda)[(int)TalkArrayPos.requirement_id] = 0x00;
		}

		public void EnableEarlySage(NPCdata npcdata)
		{
			npcdata.GetTalkArray(ObjectId.CanoeSage)[(int)TalkArrayPos.requirement_id] = 0x00;
			InsertDialogs(0x2B, "The FIENDS are waking.\nTake this and go defeat\nthem!\n\n\nReceived #");
		}

		public void PartyRoulette()
		{
			// First, disable the 'B' button to prevent going back after roulette spin
			Data[0x39C92] = 0xF7; // F7 here is really a relative jump value of -9
			Data[0x39C9B] = 0xF7;
			Data[0x39CA4] = 0xF7;
			Data[0x39CBD] = 0xEA;
			Data[0x39CBE] = 0xEA;

			// Then skip the check for directionaly input and just constantly cycle class selection 0 through 5
			Put(0x39D25, Enumerable.Repeat((byte)0xEA, 14).ToArray());
		}

		private enum FF1Class
		{
			Fighter = 0,
			Thief = 1,
			BlackBelt = 2,
			RedMage = 3,
			WhiteMage = 4,
			BlackMage = 5,
			Knight = 6,
			Ninja = 7,
			Master = 8,
			RedWiz = 9,
			WhiteWiz = 10,
			BlackWiz = 11,
			None = 12,
		}


		private readonly List<byte> AllowedSlotBitmasks = new List<byte> { 0x01, 0x02, 0x04, 0x08 };

		private readonly List<FF1Class> DefaultChoices = Enumerable.Range(0, 6).Select(x => (FF1Class)x).ToList();

		void UpdateCharacterFromOptions(int slotNumber, bool forced, IList<FF1Class> options, MT19337 rng)
		{
			const int lut_PtyGenBuf = 0x784AA;       // offset for party generation buffer LUT
			const int lut_ClassPreferences = 0x78114;  // classes LUT

			var i = slotNumber - 1;

			if (forced) // if forced
			{
				FF1Class forcedclass;
				if (options.Any())
				{
					forcedclass = options.PickRandom(rng);
				}
				else
				{
					forcedclass = (FF1Class)(Enum.GetValues(typeof(FF1Class))).
						GetValue(rng.Between(0, slotNumber == 1 ? 11 : 12));
				}
				options.Clear();
				options.Add(forcedclass);
			}

			// don't make any changes if there's nothing to do
			if (!options.Any()) return;

			//byte allowedFlags = 0b0000_0000;
			foreach (FF1Class option in options)
			{
				Data[lut_ClassPreferences + (((int)option == 12) ? 0 : (int)option + 1)] |= AllowedSlotBitmasks[i];
			}

			// set default member
			var defaultclass = (forced || !DefaultChoices.SequenceEqual(options)) ? (int)options.PickRandom(rng) : slotNumber - 1;
			Data[lut_PtyGenBuf + i * 0x10] = defaultclass == 12 ? (byte)0xFF : (byte)defaultclass;

			options.Clear();
		}

		public void PartyComposition(MT19337 rng, Flags flags, Preferences preferences)
		{
			var options = new List<FF1Class>();

			// Set bitmask for each slots (AllowedSlotBitmasks)
			PutInBank(0x1E, 0x8110, Blob.FromHex("01020408"));

			// Zero out allowed classes lut since we're going to bitwise OR it
			PutInBank(0x1E, 0x8114, Blob.FromHex("00000000000000000000000000"));

			// Do each slot - so ugly!
			if ((flags.FIGHTER1 ?? false)) options.Add(FF1Class.Fighter);
			if ((flags.THIEF1 ?? false)) options.Add(FF1Class.Thief);
			if ((flags.BLACK_BELT1 ?? false)) options.Add(FF1Class.BlackBelt);
			if ((flags.RED_MAGE1 ?? false)) options.Add(FF1Class.RedMage);
			if ((flags.WHITE_MAGE1 ?? false)) options.Add(FF1Class.WhiteMage);
			if ((flags.BLACK_MAGE1 ?? false)) options.Add(FF1Class.BlackMage);
			if ((flags.KNIGHT1 ?? false)) options.Add(FF1Class.Knight);
			if ((flags.NINJA1 ?? false)) options.Add(FF1Class.Ninja);
			if ((flags.MASTER1 ?? false)) options.Add(FF1Class.Master);
			if ((flags.RED_WIZ1 ?? false)) options.Add(FF1Class.RedWiz);
			if ((flags.WHITE_WIZ1 ?? false)) options.Add(FF1Class.WhiteWiz);
			if ((flags.BLACK_WIZ1 ?? false)) options.Add(FF1Class.BlackWiz);
			UpdateCharacterFromOptions(1, (flags.FORCED1 ?? false), options, rng);

			if ((flags.FIGHTER2 ?? false)) options.Add(FF1Class.Fighter);
			if ((flags.THIEF2 ?? false)) options.Add(FF1Class.Thief);
			if ((flags.BLACK_BELT2 ?? false)) options.Add(FF1Class.BlackBelt);
			if ((flags.RED_MAGE2 ?? false)) options.Add(FF1Class.RedMage);
			if ((flags.WHITE_MAGE2 ?? false)) options.Add(FF1Class.WhiteMage);
			if ((flags.BLACK_MAGE2 ?? false)) options.Add(FF1Class.BlackMage);
			if ((flags.NONE_CLASS2 ?? false)) options.Add(FF1Class.None);
			if ((flags.KNIGHT2 ?? false)) options.Add(FF1Class.Knight);
			if ((flags.NINJA2 ?? false)) options.Add(FF1Class.Ninja);
			if ((flags.MASTER2 ?? false)) options.Add(FF1Class.Master);
			if ((flags.RED_WIZ2 ?? false)) options.Add(FF1Class.RedWiz);
			if ((flags.WHITE_WIZ2 ?? false)) options.Add(FF1Class.WhiteWiz);
			if ((flags.BLACK_WIZ2 ?? false)) options.Add(FF1Class.BlackWiz);
			UpdateCharacterFromOptions(2, (flags.FORCED2 ?? false), options, rng);

			if ((flags.FIGHTER3 ?? false)) options.Add(FF1Class.Fighter);
			if ((flags.THIEF3 ?? false)) options.Add(FF1Class.Thief);
			if ((flags.BLACK_BELT3 ?? false)) options.Add(FF1Class.BlackBelt);
			if ((flags.RED_MAGE3 ?? false)) options.Add(FF1Class.RedMage);
			if ((flags.WHITE_MAGE3 ?? false)) options.Add(FF1Class.WhiteMage);
			if ((flags.BLACK_MAGE3 ?? false)) options.Add(FF1Class.BlackMage);
			if ((flags.NONE_CLASS3 ?? false)) options.Add(FF1Class.None);
			if ((flags.KNIGHT3 ?? false)) options.Add(FF1Class.Knight);
			if ((flags.NINJA3 ?? false)) options.Add(FF1Class.Ninja);
			if ((flags.MASTER3 ?? false)) options.Add(FF1Class.Master);
			if ((flags.RED_WIZ3 ?? false)) options.Add(FF1Class.RedWiz);
			if ((flags.WHITE_WIZ3 ?? false)) options.Add(FF1Class.WhiteWiz);
			if ((flags.BLACK_WIZ3 ?? false)) options.Add(FF1Class.BlackWiz);
			UpdateCharacterFromOptions(3, (flags.FORCED3 ?? false), options, rng);

			if ((flags.FIGHTER4 ?? false)) options.Add(FF1Class.Fighter);
			if ((flags.THIEF4 ?? false)) options.Add(FF1Class.Thief);
			if ((flags.BLACK_BELT4 ?? false)) options.Add(FF1Class.BlackBelt);
			if ((flags.RED_MAGE4 ?? false)) options.Add(FF1Class.RedMage);
			if ((flags.WHITE_MAGE4 ?? false)) options.Add(FF1Class.WhiteMage);
			if ((flags.BLACK_MAGE4 ?? false)) options.Add(FF1Class.BlackMage);
			if ((flags.NONE_CLASS4 ?? false)) options.Add(FF1Class.None);
			if ((flags.KNIGHT4 ?? false)) options.Add(FF1Class.Knight);
			if ((flags.NINJA4 ?? false)) options.Add(FF1Class.Ninja);
			if ((flags.MASTER4 ?? false)) options.Add(FF1Class.Master);
			if ((flags.RED_WIZ4 ?? false)) options.Add(FF1Class.RedWiz);
			if ((flags.WHITE_WIZ4 ?? false)) options.Add(FF1Class.WhiteWiz);
			if ((flags.BLACK_WIZ4 ?? false)) options.Add(FF1Class.BlackWiz);
			UpdateCharacterFromOptions(4, (flags.FORCED4 ?? false), options, rng);

			// Load stats for None
			PutInBank(0x1F, 0xC783, Blob.FromHex("2080B3C931F053EA"));
			PutInBank(0x00, 0xB380, Blob.FromHex("BD0061C9FFD013A9019D0161A9009D07619D08619D0961A931600A0A0A0AA860"));

			//clinic stuff
			PutInBank(0x0E, 0xA6F3, Blob.FromHex("203E9B"));
			PutInBank(0x0E, 0x9B3E, Blob.FromHex("BD0061C9FFD00568684C16A7BD016160"));

			// curing ailments out of battle, allow the waste of things in battle
			PutInBank(0x0E, 0xB388, Blob.FromHex("2077C2EAEAEAEAEAEAEAEA"));
			PutInBank(0x1F, 0xC277, CreateLongJumpTableEntry(0x0F, 0x8BB0));
			PutInBank(0x0F, 0x8BB0, Blob.FromHex("A5626A6A6A29C0AABD0061C9FFD003A90060BD016160"));

			// Better Battle sprite, in 0C_9910_DrawCharacter.asm
			PutInBank(0x0F, 0x8BD0, Blob.FromHex("A86A6A6AAABD0061C9FFF00898AABDA86B8DB36860"));
			PutInBank(0x0C, 0x9910, Blob.FromHex("20A4C8C9FFD001608A0A0AA8"));
			PutInBank(0x1F, 0xC8A4, CreateLongJumpTableEntry(0x0F, 0x8BD0));

			// MapMan for Nones and Fun% Party Leader
			byte leader = (byte)((byte)preferences.MapmanSlot << 6);
			Data[0x7D8BC] = leader;
			PutInBank(0x1F, 0xE92E, Blob.FromHex("20608FEAEAEAEA"));
			PutInBank(0x02, 0x8F60, Blob.FromHex($"A9008510AD{leader:X2}61C9FFF00160A92560"));

			// Draw Complex String extension for class FF
			PutInBank(0x1F, 0xC27D, Blob.FromHex("C9FFF0041869F060203EE0A997853EA9C2853F2045DE4C4EE0EA973CA800"));
			PutInBank(0x1F, 0xDF0C, Blob.FromHex("207DC2"));

			// Skip targeting None with aoe spells
			PutInBank(0x0C, 0xB453, Blob.FromHex("20AAC8C9FFF015EA"));
			PutInBank(0x1F, 0xC8AA, CreateLongJumpTableEntry(0x0F, 0x8BF0));
			PutInBank(0x0F, 0x8BF0, Blob.FromHex("ADCE6BAA6A6A6AA8B90061C9FFF0068A09808D8A6C60"));

			// Spell level up change to allow any class to gain spell charges, see 1B_8818_LvlUp_LevelUp.asm
			PutInBank(0x1B, 0x88D7, Blob.FromHex("AE8E68A001B182A02848B184DD02899005684A4CFA88684A900948B184186901918468C8C030D0E14C1C89000000090909040400090909"));

			// To allow all promoted classes
			EnableTwelveClasses();
		}
		// Deprecated, delete if there's no revolt for it to come back 2020-12-17
		public void LinearMPGrowth()
		{
			// Change MP growth to be linear (every 3 levels) as a fix for random promotion 
			var levelUpStats = Get(NewLevelUpDataOffset, 588).Chunk(49 * 2);
			var rmArray = Enumerable.Repeat((byte)0x00, 49).ToList();
			var wbmArray = Enumerable.Repeat((byte)0x00, 49).ToList();
			var rmCount = new List<int> { 2, 2, 2, 2, 2, 2, 2, 2 };
			var wbmCount = new List<int> { 2, 2, 2, 2, 2, 2, 2, 2 };
			var rmMinLevel = new List<int> { 2, 2, 6, 10, 15, 20, 25, 31 };
			var wbmMinLevel = new List<int> { 2, 2, 5, 8, 12, 16, 20, 25 };
			var bitArray = new List<byte> { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };

			for (int i = 0; i < 49; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					if (rmMinLevel[j] <= i + 2)
						rmCount[j]++;

					if (rmCount[j] >= 3)
					{
						rmArray[i] |= bitArray[j];
						rmCount[j] = 0;
					}

					if (wbmMinLevel[j] <= i + 2)
						wbmCount[j]++;

					if (wbmCount[j] >= 3)
					{
						wbmArray[i] |= bitArray[j];
						wbmCount[j] = 0;
					}
				}
			}

			for (int i = 0; i < 49; i++)
			{
				levelUpStats[3][i * 2 + 1] = rmArray[i];
				levelUpStats[4][i * 2 + 1] = wbmArray[i];
				levelUpStats[5][i * 2 + 1] = wbmArray[i];
			}

			// Insert level up data
			Put(NewLevelUpDataOffset, levelUpStats.SelectMany(x => (byte[])x).ToArray());
		}

		public void PubReplaceClinic(MT19337 rng, Flags flags)
		{
			// Copy some CHR data to make the Tavern look more like one.
			const int ShopTileDataOffet = 0x24000;
			const int TileSize = 16;
			const int ArmorTileOffset = 14 * 1 * TileSize;
			const int ClinicTileOffset = 14 * 4 * TileSize;
			const int ItemTileOffset = 14 * 6 * TileSize;
			const int CaravanTileOffset = 14 * 7 * TileSize;
			const int DecorationOffset = TileSize * 4;
			const int VendorOffset = TileSize * 8;

			Put(ShopTileDataOffet + ClinicTileOffset, Get(ShopTileDataOffet + CaravanTileOffset, TileSize * 4)); // Tablecloth
			Put(ShopTileDataOffet + ClinicTileOffset + DecorationOffset, Get(ShopTileDataOffet + ItemTileOffset + DecorationOffset, TileSize * 4)); // Barrels of fine ale
			Put(ShopTileDataOffet + ClinicTileOffset + VendorOffset, Get(ShopTileDataOffet + ArmorTileOffset + VendorOffset, TileSize * 6)); // Armorer tending bar
			Put(0x03250, Get(0x03258, 4)); // Caravan palette

			List<byte> options = new List<byte> { };
			if ((flags.TAVERN1 ?? false)) options.Add(0x0);
			if ((flags.TAVERN2 ?? false)) options.Add(0x1);
			if ((flags.TAVERN3 ?? false)) options.Add(0x2);
			if ((flags.TAVERN4 ?? false)) options.Add(0x3);
			if ((flags.TAVERN5 ?? false)) options.Add(0x4);
			if ((flags.TAVERN6 ?? false)) options.Add(0x5);

			if (options.Count == 0) options = new List<byte> { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5 };
			List<byte> pub_lut = new List<byte> { };
			while (pub_lut.Count < 7)
			{
				options.Shuffle(rng);
				pub_lut.AddRange(options);
			}
			pub_lut.Insert(3, (byte)0xFF); // Will break if Melmond ever gets a clinic, Nones will need to be hired dead, this results in them being alive.
			Put(0x38066, Blob.FromHex("9D8A9F8E9B97")); // Replaces "CLINIC" with "TAVERN"
														// EnterClinic
			PutInBank(0x0E, 0xA5A1, Blob.FromHex("60A9008524852520589DA902856318201C9DB0ECAD62008D0D0320EDA6B0034C6CA620D7A6B0DAAD62008D0C03209BAA20C2A8B0CCAD6200D0C72089A6AD0C03186D0C036D0C03AABD10036A6A6A29C0AAAD0D03F0458A690A8510A96185118A488512A9638513A000A90091109112C8C00A30F7C040D0F59D266120E99CA448B90A9D9D00612071C2A00E20799068AA900918BD006169069D0061A9019D0A61A9009D016120349D200CE92078A7A921205BAA2043A7A5240525F0F74CA2A52043A7A5240525F0F74CA1A5A923205BAAA9008D24008D25004C60A6EAEAEAEAEAEAEAEAEAEAEAEAEA"));
			PutInBank(0x0E, 0x9D0A, pub_lut.Take(8).ToArray());
			// Clinic_SelectTarget followed by ClinicBuildNameString
			PutInBank(0x0E, 0xA6D7, Blob.FromHex("A903203BAA20EDA6A910853EA903853F2032AA4C07A9A000A200866320959DD01C8A2A2A2A29036910991003A900991103A90199120398186903A8E6638A186940AAD0D8A900991003A563C90160EAEA"));
			// Moved routine
			PutInBank(0x0E, 0xA3F8, Blob.FromHex("7D9C"));
			PutInBank(0x0E, 0x9C7D, Blob.FromHex("A5626A6A6A29C08D0A03A666D01EA018AE0A03BD1861F032C8BD1961F02CC8BD1A61F026C8BD1B61F0203860A01CAE0A03BD1C61F025C8BD1D61F01FC8BD1E61F019C8BD1F61F013386098186D0A03AAAD0C0338E91B9D0061186098186D0A03AAAD0C0338E9439D00611860"));
			// New routine that unequips all gear and sets all substats to zero
			PutInBank(0x0E, 0x9CE9, Blob.FromHex("188A69188510A9618511A007B110297F91108810F7A900A0089110C8C00ED0F960"));

			PutInBank(0x1F, 0xC271, CreateLongJumpTableEntry(0x00, 0xC783));

			//ReviveorBuy routine
			PutInBank(0x0E, 0x9D1C, Blob.FromHex("A903203BAAA912853EA99D853F2032AAA9028D62004C07A9"));

			// Revive Hire text options
			PutInBank(0x0E, 0x9D12, Blob.FromHex("9BA8B9AC320191AC2300"));

			// Clinic_InitialText followed by ShouldSkipChar followed by "Hire a\n" text
			PutInBank(0x0E, 0x9D58, Blob.FromHex("205BAAA0FFC8B9B09D991003D0F7A902991003C8A648BD0A9D69F0991003C8A905991003C8A9C5991003C8A900991003A9108D3E00A9038D3F004C32AAAD0D03D010BD0061C9FFD003A90160BD0161C90160BD0061C9FF6091AC23FFA40100"));

			// New routine to level up replaced character and zero some stuff, needs new level up stuff in bank 1B
			PutInBank(0x0E, 0x9D34, Blob.FromHex("A99D48A94B48A98748A9A9488A182A2A2A8510A91B4C03FEA9008D24008D25008D012060"));

			Data[0x101A] = 0x13;
			Data[0x109A] = 0x13;
			Data[0x111A] = 0x76;
			Data[0x119A] = 0x77;

			if (flags.RecruitmentModeHireOnly ?? false)
			{
				PutInBank(0x0E, 0xA5B0, Blob.FromHex("EAEAEAEAEAA901EA"));
				PutInBank(0x0E, 0xA5C7, Blob.FromHex("D9"));
			}

			if (!flags.RecruitmentModeReplaceOnlyNone ?? false)
			{
				PutInBank(0x0E, 0x9DAA, Blob.FromHex("A90060"));
			}


		}

		public void DisablePartyShuffle()
		{
			var nops = new byte[PartyShuffleSize];
			for (int i = 0; i < nops.Length; i++)
			{
				nops[i] = Nop;
			}
			Put(PartyShuffleOffset, nops);

			nops = new byte[2];
			for (int i = 0; i < nops.Length; i++)
			{
				nops[i] = Nop;
			}
			Put(0x39A6B, nops);
			Put(0x39A74, nops);
		}

		public void EnableSpeedHacks()
		{
			// Screen wipe
			Data[0x7D6EE] = 0x08; // These two values must evenly divide 224 (0xE0), default are 2 and 4
			Data[0x7D6F5] = 0x10;
			Data[0x7D713] = 0x0A; // These two values must evenly divide 220 (0xDC), default are 2 and 4
			Data[0x7D71A] = 0x14; // Don't ask me why they aren't the same, it's the number of scanlines to stop the loop at

			// Dialogue boxes
			Data[0x7D620] = 0x0B; // These two values must evenly divide 88 (0x58), the size of the dialogue box
			Data[0x7D699] = 0x0B;

			// Battle entry
			Data[0x7D90A] = 0x11; // This can be just about anything, default is 0x41, sfx lasts for 0x20

			// All kinds of palette cycling
			Data[0x7D955] = 0x00; // This value is ANDed with a counter, 0x03 is default, 0x01 is double speed, 0x00 is quadruple

			// Battle
			Data[0x31ECE] = 0x60; // Double character animation speed
			Data[0x2DFB4] = 0x04; // Quadruple run speed

			Data[0x33D4B] = 0x04; // Explosion effect count (big enemies), default 6
			Data[0x33CCD] = 0x04; // Explosion effect count (small enemies), default 8
			Data[0x33DAA] = 0x04; // Explosion effect count (mixed enemies), default 15

			// Draw and Undraw Battle Boxes faster
			Put(0x2DA12, Blob.FromHex("3020181008040201")); // More practical Respond Rates
			Put(0x7F4AA, Blob.FromHex($"ADFC6048A90F2003FE20008AA9{BattleBoxDrawInFrames}8517A90F2003FE20208A2085F4C617D0F1682003FE60"));
			Put(0x7F4FF, Blob.FromHex($"ADFC6048A90F2003FE20808AA9{BattleBoxUndrawFrames}8517A90F2003FE20A08A2085F4C617D0F1682003FE60"));

			// Gain multiple levels at once.
			//Put(0x2DD82, Blob.FromHex("20789f20579f48a5802907c907f008a58029f0690785806820019c4ce89b")); old
			PutInBank(0x1B, 0x8850, Blob.FromHex("4C4888"));
			// Skip stat up messages
			PutInBank(0x1B, 0x89F0, Blob.FromHex("4CE38B"));

			// Default Response Rate 8 (0-based)
			Data[0x384CB] = 0x07; // Initialize respondrate to 7
			Put(0x3A153, Blob.FromHex("4CF0BF")); // Replace reset respond rate with a JMP to...
			Put(0x3BFF0, Blob.FromHex("A90785FA60")); // Set respondrate to 7

			// Faster Lineup Modifications
			var animationOffsets = new List<int> { 0x39AA0, 0x39AB4, 0x39B10, 0x39B17, 0x39B20, 0x39B27 };
			animationOffsets.ForEach(addr => Data[addr] = 0x04);

			// Move NPCs out of the way.
			MoveNpc(MapId.Coneria, 0, 0x11, 0x02, inRoom: false, stationary: true); // North Coneria Soldier
			MoveNpc(MapId.Coneria, 4, 0x12, 0x14, inRoom: false, stationary: true); // South Coneria Gal
			MoveNpc(MapId.Coneria, 7, 0x1E, 0x0B, inRoom: false, stationary: true); // East Coneria Guy
			MoveNpc(MapId.Elfland, 0, 0x27, 0x18, inRoom: false, stationary: true); // Efland Entrance Elf
			MoveNpc(MapId.Onrac, 13, 0x29, 0x1B, inRoom: false, stationary: true); // Onrac Guy
			MoveNpc(MapId.Lefein, 3, 0x21, 0x07, inRoom: false, stationary: true); // Lefein Guy
																				   //MoveNpc(MapId.Waterfall, 1, 0x0C, 0x34, inRoom: false, stationary: false); // OoB Bat!
			MoveNpc(MapId.EarthCaveB3, 10, 0x09, 0x0B, inRoom: true, stationary: false); // Earth Cave Bat B3
			MoveNpc(MapId.EarthCaveB3, 7, 0x0B, 0x0B, inRoom: true, stationary: false); // Earth Cave Bat B3
			MoveNpc(MapId.EarthCaveB3, 8, 0x0A, 0x0C, inRoom: true, stationary: false); // Earth Cave Bat B3
			MoveNpc(MapId.EarthCaveB3, 9, 0x09, 0x25, inRoom: false, stationary: false); // Earth Cave Bat B3
			MoveNpc(MapId.EarthCaveB5, 1, 0x22, 0x34, inRoom: false, stationary: false); // Earth Cave Bat B5
			MoveNpc(MapId.ConeriaCastle1F, 5, 0x07, 0x0F, inRoom: false, stationary: true); // Coneria Ghost Lady

			MoveNpc(MapId.Pravoka, 4, 0x1F, 0x05, inRoom: false, stationary: true); // Pravoka Old Man
			MoveNpc(MapId.Pravoka, 5, 0x08, 0x0E, inRoom: false, stationary: true); // Pravoka Woman
		}

		public void EnableConfusedOldMen(MT19337 rng)
		{
			List<(byte, byte)> coords = new List<(byte, byte)> {
				( 0x2A, 0x0A ), ( 0x28, 0x0B ), ( 0x26, 0x0B ), ( 0x24, 0x0A ), ( 0x23, 0x08 ), ( 0x23, 0x06 ),
				( 0x24, 0x04 ), ( 0x26, 0x03 ), ( 0x28, 0x03 ), ( 0x28, 0x04 ), ( 0x2B, 0x06 ), ( 0x2B, 0x08 )
			};
			coords.Shuffle(rng);

			List<int> sages = Enumerable.Range(0, 12).ToList(); // But the 12th Sage is actually id 12, not 11.
			sages.ForEach(sage => MoveNpc(MapId.CrescentLake, sage < 11 ? sage : 12, coords[sage].Item1, coords[sage].Item2, inRoom: false, stationary: false));
		}

		public void EnableIdentifyTreasures()
		{
			InsertDialogs(0xF1 + 0x50, "Can't hold\n#");
			InsertDialogs(0xF1, "Can't hold\n#");
		}

		public void EnableDash()
		{
			Put(0x7D077, Blob.FromHex("A5424A69004A69000A242050014A853460"));
		}

		public void EnableBuyTen()
		{
			Put(0x380FF, Blob.FromHex("100001110001120001130000"));
			Put(0x38248, Blob.FromHex("8BB8BC018BB8BCFF8180018EBB5B00"));
			Put(0x3A8E4, Blob.FromHex("A903203BAAA9122026AAA90485634C07A9A903203BAAA91F2026AAA90385634C07A9EA"));
			Put(0x3A32C, Blob.FromHex("71A471A4"));
			Put(0x3A45A, Blob.FromHex("2066A420EADD20EFA74CB9A3A202BD0D039510CA10F860A909D002A925205BAAA5664A9009EAEAEAEAEAEAEAEAEA20F5A8B0E3A562F0054A90DCA909856AA90D205BAA2057A8B0D82076AAA66AF017861318A200A003BD0D0375109D0D03E888D0F4C613D0EB2068AA20C2A8B0ADA562D0A9208CAA9005A9104C77A4AE0C03BD206038656AC9649005A90C4C77A49D206020F3A4A9134C77A4A200A00338BD1C60FD0D039D1C60E888D0F34CEFA7"));
			Put(0x3AA65, Blob.FromHex("2076AAA90E205BAA2066A4208E8E4C32AAA662BD00038D0C0320B9ECA202B5109D0D03CA10F860A202BD0D03DD1C60D004CA10F51860"));
			Put(0x3A390, Blob.FromHex("208CAA"));
			Put(0x3A3E0, Blob.FromHex("208CAA"));
			Put(0x3A39D, Blob.FromHex("20F3A4"));
			Put(0x3A404, Blob.FromHex("20F3A4"));
			Put(0x3AACB, Blob.FromHex("18A202B5106A95109D0D03CA10F5"));
		}

		public void EnableBuyQuantity()
		{
			Put(0x39E00, Blob.FromHex("ad0a0385104c668eae0c03bd2060186d0a03c9649001609d206060a903203baaa9018d0a03a520290f856120009e2032aa2043a7a525d056a524d05aa520290fc561f0ed8561c900f0e7c904f02fc908f01ac901f00ace0a03d0d0ee0a03d0cbee0a03c964d0c4ce0a03d0bfad0a0318690a8d0a03c96490b2a96310f5ad0a0338e90af0021002a9018d0a03109d38a90085248525601890f6"));
			Put(0x39E99, Blob.FromHex("a90e205baaa5620a0a0a186916aabd00038d0c0320b9ecae0a03a9008d0b038d0e038d0f0318ad0b0365108d0b03ad0e0365118d0e03ad0f0369008d0f03b005caf00dd0e1a9ff8d0b038d0e038d0f03ad0f038512ad0e038511ad0b03851020429f2032aa60"));
			Put(0x39EFF, Blob.FromHex("ad1e60cd0f03f0049016b016ad1d60cd0e03f004900ab00aad1c60cd0b03b00238601860ad1c6038ed0b038d1c60ad1d60ed0e038d1d60ad1e60ed0f038d1e604cefa74c8e8e"));
			Put(0x3A494, Blob.FromHex("201b9eb0e820999e20c2a8b0e0a562d0dc20ff9e9008a910205baa4c81a420089e9008a90c205baa4c81a420239fa913205baa4c81a4eaeaea"));
		}

		public void EnableSaveOnDeath(Flags flags)
		{
			// rewrite rando's GameOver routine to jump to a new section that will save the game data
			PutInBank(0x1B, 0x801A, Blob.FromHex("4CF58F"));

			// write new routine to save data at game over (the game will save when you clear the final textbox and not before), see 1B_8FF5_GameOverAndRestart.asm
			var saveondeath_standardmid = "AD0460D02EAD0060F04FAD0160CD0164D008AD0260CD0264F03FAD016038E9078D1060AD026038E9078D1160A9048D1460D026AD056038E9078D1060AD066038E9078D1160A9018D1460AD0060F00AA9988D0160A9A98D0260";
			var saveondeath_dwmodemid = "AD0460F00AA9998D0560A9A58D0660AD0060F00AA9988D0160A9A98D0260A9928D1060A99E8D11604E1E606E1D606E1C60EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA";
			var saveondeath_part1 = "20E38BA200BD0061C9FFF041BD0C619D0A61BD0D619D0B61BD28639D2063BD29639D2163BD2A639D2263BD2B639D2363BD2C639D2463BD2D639D2563BD2E639D2663BD2F639D2763A9009D01618A186940AAD0B1";
			var saveondeath_part2 = "A200BD00609D0064BD00619D0065BD00629D0066BD00639D0067E8D0E5A9558DFE64A9AA8DFF64A9008DFD64A200187D00647D00657D00667D0067E8D0F149FF8DFD644C1D80";

			var saveondeath = saveondeath_part1 + (flags.SaveGameDWMode ? saveondeath_dwmodemid : saveondeath_standardmid) + saveondeath_part2;

			PutInBank(0x1B, 0x8FF5, Blob.FromHex(saveondeath));
		}

		public void ShuffleAstos(Flags flags, NPCdata npcdata, TalkRoutines talkroutines, MT19337 rng)
		{
			// NPC pool to swap Astos with
			List<ObjectId> npcpool = new List<ObjectId> { ObjectId.Astos, ObjectId.Bahamut, ObjectId.CanoeSage, ObjectId.CubeBot, ObjectId.ElfDoc,
			ObjectId.Fairy, ObjectId.King, ObjectId.Matoya, ObjectId.Nerrick, ObjectId.Princess2, ObjectId.Smith,
			ObjectId.Titan, ObjectId.Unne, ObjectId.Sarda, ObjectId.ElfPrince, ObjectId.Lefein };

			// Select random npc
			ObjectId newastos = npcpool.PickRandom(rng);

			// If Astos, we're done here
			if (newastos == ObjectId.Astos) return;
			
			// If not get NPC talk routine, get NPC object
			var talkscript = npcdata.GetRoutine(newastos);

			// Switch astos to Talk_GiveItemOnItem;
			npcdata.SetRoutine(ObjectId.Astos, newTalkRoutines.Talk_GiveItemOnItem);

			// Get items name
			var newastositem = FormattedItemName((Item)npcdata.GetTalkArray(newastos)[(int)TalkArrayPos.item_id]);
			var nwkingitem = FormattedItemName((Item)npcdata.GetTalkArray(ObjectId.Astos)[(int)TalkArrayPos.item_id]);

			// Custom dialogs for Astos NPC and the Kindly Old King
			List<(byte, string)> astosdialogs = new List<(byte, string)>
			{
				(0x00, ""),
				(0x02, "You have ruined my plans\nto steal this " + newastositem + "!\nThe princess will see\nthrough my disguise.\nTremble before the might\nof Astos, the Dark King!"),
				(0x00, ""),(0x00, ""),(0x00, ""),
				(0x0C, "You found the HERB?\nCurses! The Elf Prince\nmust never awaken.\nOnly then shall I,\nAstos, become\nthe King of ALL Elves!"),
				(0x0E, "Is this a dream?.. Are\nyou, the LIGHT WARRIORS?\nHA! Thank you for waking\nme! I am actually Astos,\nKing of ALL Elves! You\nwon't take my " + newastositem + "!"),
				(0x12, "My CROWN! Oh, but it\ndoesn't go with this\noutfit at all. You keep\nit. But thanks! Here,\ntake this also!\n\nReceived " + nwkingitem),
				(0x14, "Oh, wonderful!\nNice work! Yes, this TNT\nis just what I need to\nblow open the vault.\nSoon more than\nthe " + newastositem + " will\nbelong to Astos,\nKing of Dark Dwarves!"),
				(0x16, "ADAMANT!! Now let me\nmake this " + newastositem + "..\nAnd now that I have\nthis, you shall take a\nbeating from Astos,\nthe Dark Blacksmith!"),
				(0x19, "You found my CRYSTAL and\nwant my " + newastositem + "? Oh!\nI can see!! And now, you\nwill see the wrath of\nAstos, the Dark Witch!"),
				(0x1C, "Finally! With this SLAB,\nI shall conquer Lefein\nand her secrets will\nbelong to Astos,\nthe Dark Scholar!"),
				(0x00, ""),
				(0x1E, "Can't you take a hint?\nI just want to be left\nalone with my " + newastositem + "!\nI even paid a Titan to\nguard the path! Fine.\nNow you face Astos,\nKing of the Hermits!"),
				(0x20, "Really, a rat TAIL?\nYou think this is what\nwould impress me?\nIf you want to prove\nyourself, face off with\nAstos, the Dark Dragon!"),
				(0xCD, "Kupo?.. Lali ho?..\nMugu mugu?.. Fine! You\nare in the presence of\nAstos, the Dark Thief!\nI stole their " + newastositem + "\nfair and square!"),
				(0x00, ""),
				(0x27, "Boop Beep Boop..\nError! Malfunction!..\nI see you are not\nfooled. It is I, Astos,\nKing of the Dark Robots!\nYou shall never have\nthis " + newastositem + "!"),
				(0x06, "This " + newastositem + " has passed\nfrom Queen to Princess\nfor 2000 years. It would\nhave been mine if you\nhadn't rescued me! Now\nyou face Astos, the\nDark Queen!"),
				(0x23, "I, Astos the Dark Fairy,\nam free! The other\nfairies trapped me in\nthat BOTTLE! I'd give\nyou this " + newastositem + " in\nthanks, but I would\nrather just kill you."),
				(0x2A, "If you want pass, give\nme the RUBY..\nHa, it mine! Now, you in\ntrouble. Me am Astos,\nKing of the Titans!"),
				(0x2B, "Curses! Do you know how\nlong it took me to\ninfiltrate these grumpy\nold men and steal\nthe " + newastositem + "?\nNow feel the wrath of\nAstos, the Dark Sage!")
			};

			InsertDialogs(astosdialogs[(int)newastos].Item1, astosdialogs[(int)newastos].Item2);
			InsertDialogs(astosdialogs[(int)ObjectId.Astos].Item1, astosdialogs[(int)ObjectId.Astos].Item2);

			if (talkscript == newTalkRoutines.Talk_Titan || talkscript == newTalkRoutines.Talk_ElfDocUnne)
			{
				// Skip giving item for Titan, ElfDoc or Unne
				talkroutines.ReplaceChunk(newTalkRoutines.Talk_Astos, Blob.FromHex("20109F"), Blob.FromHex("EAEAEA"));
				talkroutines.ReplaceChunk(newTalkRoutines.Talk_Astos, Blob.FromHex("A9F060"), Blob.FromHex("4C4396"));
				npcdata.SetRoutine(newastos, newTalkRoutines.Talk_Astos);
			}
			else if (talkscript == newTalkRoutines.Talk_GiveItemOnFlag)
			{
				// Check for a flag instead of an item                          
				talkroutines.ReplaceChunk(newTalkRoutines.Talk_Astos, Blob.FromHex("A674F005BD2060F0"), Blob.FromHex("A474F00520799090"));
				npcdata.SetRoutine(newastos, newTalkRoutines.Talk_Astos);
			}
			else if (talkscript == newTalkRoutines.Talk_Nerrick || talkscript == newTalkRoutines.Talk_GiveItemOnItem || talkscript == newTalkRoutines.Talk_TradeItems)
			{
				// Just set NPC to Astos routine
				npcdata.SetRoutine(newastos, newTalkRoutines.Talk_Astos);
			}
			else if (talkscript == newTalkRoutines.Talk_Bahamut)
			{
				// Change routine to check for Tail, give promotion and trigger the battle at the same time, see 11_8200_TalkRoutines.asm
				talkroutines.Replace(newTalkRoutines.Talk_Bahamut, Blob.FromHex("AD2D60D003A57160E67DA572203D96A575200096A476207F9020739220AE952018964C439660"));
			}

			// Set battle
			npcdata.GetTalkArray(newastos)[(int)TalkArrayPos.battle_id] = 0x7D;
		}

		private void EnableEasyMode()
		{
			ScaleEncounterRate(0.20, 0.20);
			var enemies = Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);
			foreach (var enemy in enemies)
			{
				var hp = BitConverter.ToUInt16(enemy, 4);
				hp = (ushort)(hp * 0.1);
				var hpBytes = BitConverter.GetBytes(hp);
				Array.Copy(hpBytes, 0, enemy, 4, 2);
			}

			Put(EnemyOffset, enemies.SelectMany(enemy => enemy.ToBytes()).ToArray());
		}

		public void EasterEggs()
		{
			Put(0x2ADDE, Blob.FromHex("91251A682CFF8EB1B74DB32505FFBE9296991E2F1AB6A4A9A8BE05FFFFFFFFFFFF9B929900"));
		}

		/// <summary>
		/// Unused method, but this would allow a non-npc shuffle king to build bridge without rescuing princess
		/// </summary>
		public void EnableEarlyKing(NPCdata npcdata)
		{
			npcdata.GetTalkArray(ObjectId.King)[(int)TalkArrayPos.requirement_id] = 0x00;
			InsertDialogs(0x02, "To aid you on your\nquest, please take this.\n\n\n\nReceived #");
		}

		public void EnableFreeBridge()
		{
			// Set the default bridge_vis byte on game start to true. It's a mother beautiful bridge - and it's gonna be there.
			Data[0x3008] = 0x01;
		}

		public void EnableFreeShip()
		{
			Data[0x3000] = 1;
			Data[0x3001] = 152;
			Data[0x3002] = 169;
		}

		public void EnableFreeAirship()
		{
			Data[0x3004] = 1;
			Data[0x3005] = 153;
			Data[0x3006] = 165;
		}

		public void EnableFreeCanal(bool npcShuffleEnabled, NPCdata npcdata)
		{
			Data[0x300C] = 0;

			// Put safeguard to prevent softlock if TNT is turned in (as it will remove the Canal)
			if (!npcShuffleEnabled)
				npcdata.GetTalkArray(ObjectId.Nerrick)[(int)TalkArrayPos.item_id] = (byte)Item.Cabin;
		}

		public void EnableFreeCanoe()
		{
			Data[0x3012] = 0x01;
		}

		public void EnableCanalBridge()
		{
			// Inline edit to draw the isthmus or the bridge, but never the open canal anymore.
			// See 0F_8780_IsOnEitherBridge for this and the IsOnBridge replacement called from below.
			Put(0x7E3BB, Blob.FromHex("A266A0A420DFE3B0E1A908AE0C60F002A910"));

			/**
			 *  A slight wrinkle from normal cross page jump in that we need to preserve the status register,
			 *  since the carry bit is what's used to determine if you're on a bridge or canal, of course.
			 *
			    LDA $60FC
				PHA
				LDA #$0F
				JSR $FE03 ;SwapPRG_L
				JSR $8780
				PLA
				PHP
				JSR $FE03 ;SwapPRG_L
				PLP
				RTS
			**/
			Put(0x7C64D, Blob.FromHex("ADFC6048A90F2003FE20808768082003FE2860"));
		}

		public void EnableChaosRush()
		{
			// Overwrite Keylocked door in ToFR tileset with normal door.
			Put(0x0F76, Blob.FromHex("0300"));
		}

		public void EnableFreeLute()
		{
			Data[0x3020 + (int)Item.Lute] = 0x01;
		}

		public void EnableFreeTail()
		{
			Data[0x3020 + (int)Item.Tail] = 0x01;
		}

		public void EnableFreeOrbs()
		{
			const int initItemOffset = 0x3020;
			Data[initItemOffset + (int)Item.EarthOrb] = 0x01;
			Data[initItemOffset + (int)Item.FireOrb] = 0x01;
			Data[initItemOffset + (int)Item.WaterOrb] = 0x01;
			Data[initItemOffset + (int)Item.AirOrb] = 0x01;
		}

		public void ChangeUnrunnableRunToWait()
		{
			// See Unrunnable.asm
			// Replace DrawCommandMenu with a cross page jump to a replacement that swaps RUN for WAIT if the battle is unrunnable.
			// The last 5 bytes here are the null terminated WAIT string (stashed in some leftover space of the original subroutine)
			Put(0x7F700, Blob.FromHex("ADFC6048A90F2003FE204087682003FE4C48F6A08A929D00"));

			// Replace some useless code with a special handler for unrunnables that prints 'Nothing Happens'
			// We then update the unrunnable branch to point here instead of the generic Can't Run handler
			// See Disch's comments here: Battle_PlayerTryRun  [$A3D8 :: 0x323E8]
			Put(0x32409, Blob.FromHex("189005A94E4C07AAEAEAEAEAEAEAEA"));
			// new delta to special unrunnable message handler done in 
		}

		public void SeparateUnrunnables()
		{
			// See SetRunnability.asm
			// replace a segment of code in PrepareEnemyFormation with a JSR to a new routine in dummied ROM space in bank 0B
			Put(0x2E141, Blob.FromHex("200C9B"));
			// move the rest of PrepareEnemyFormation up pad the excised code with NOPs
			Put(0x2E144, Get(0x2E160, 0x1E));
			PutInBank(0x0B, 0xA162, Enumerable.Repeat((byte)0xEA, 0x1C).ToArray());
			// write the new routine
			Put(0x2DB0C, Blob.FromHex("AD6A001023AD926D8D8A6DAD936D8D8B6DA2008E886D8E896D8E8C6D8E8D6DAD916D29FE8D916D60AD916D29FD8D916D60"));
			// change checks for unrunnability in bank 0C to check last two bits instead of last bit
			Put(0x313D3, Blob.FromHex("03")); // changes AND #$01 to AND #$03 when checking start of battle for unrunnability
											  // the second change is done in AllowStrikeFirstAndSurprise, which checks the unrunnability in battle
											  // alter the default formation data to set unrunnability of a formation to both sides if the unrunnable flag is set
			var formData = Get(FormationDataOffset, FormationDataSize * FormationCount).Chunk(FormationDataSize);
			for (int i = 0; i < NormalFormationCount; ++i)
			{
				if ((formData[i][UnrunnableOffset] & 0x01) != 0)
					formData[i][UnrunnableOffset] |= 0x02;
			}
			formData[126][UnrunnableOffset] |= 0x02; // set unrunnability for WzSahag/R.Sahag fight
			formData[127][UnrunnableOffset] |= 0x02; // set unrunnability for IronGol fight

			Put(FormationsOffset, formData.SelectMany(formation => formation.ToBytes()).ToArray());
		}

		public void ImproveTurnOrderRandomization(MT19337 rng)
		{
			// Shuffle the initial bias so enemies are no longer always at the start initially.
			List<byte> turnOrder = new List<byte> { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x80, 0x81, 0x82, 0x83 };
			turnOrder.Shuffle(rng);
			Put(0x3215C, turnOrder.ToArray());

			// Rewrite turn order shuffle to Fisher-Yates.
			Put(0x3217A, Blob.FromHex("A90C8D8E68A900AE8E68205DAEA8AE8E68EAEAEAEAEAEA"));
		}

		public void EnableInventoryAutosort()
		{
			//see 0F_8670_SortInventory.asm
			Put(0x7EF58, Blob.FromHex("A90F2003FE20008DEAEAEA"));
			PutInBank(0x0F, 0x8D00, Blob.FromHex("8663A9009D0003A900856218A000A219BD2060F0058A990003C8E8E01CD0F1A216BD2060F0058A990003C8E8E019D0F1A200BD2060F0058A990003C8E8E011D0F1C8AD3560F00399000360"));
		}

		public void EnableCritNumberDisplay()
		{
			// Overwrite the normal critical hit handler by calling ours instead
			PutInBank(0x0C, 0xA94B, Blob.FromHex("20D1CFEAEA"));
			PutInBank(0x1F, 0xCFD1, CreateLongJumpTableEntry(0x0F, 0x92A0));
		}

		public void EnableMelmondGhetto(bool enemizerOn)
		{
			// Set town desert tile to random encounters.
			// If enabled, trap tile shuffle will change that second byte to 0x00 afterward.
			Data[0x00864] = 0x0A;
			Data[0x00865] = enemizerOn ? (byte)0x00 : (byte)0x80;

			// Give Melmond Desert backdrop
			Data[0x0334D] = (byte)Backdrop.Desert;

			if (!enemizerOn) // if enemizer formation shuffle is on, it will have assigned battles to Melmond already
				Put(0x2C218, Blob.FromHex("0F0F8F2CACAC7E7C"));
		}

		public void NoDanMode()
		{
			// Instead of looping through the 'check to see if characters are alive' thing, just set it to 4 and then remove the loop.
			// EA EA EA EA EA EA (sports)
			Put(0x6CB43, Blob.FromHex("A204A004EAEAEAEAEAEAEAEAEAEAEAEAEA"));

		}

		public void ShuffleWeaponPermissions(MT19337 rng)
		{
			const int WeaponPermissionsOffset = 0x3BF50;
			ShuffleGearPermissions(rng, WeaponPermissionsOffset);
		}

		public void ShuffleArmorPermissions(MT19337 rng)
		{
			const int ArmorPermissionsOffset = 0x3BFA0;
			ShuffleGearPermissions(rng, ArmorPermissionsOffset);
		}

		public void ShuffleGearPermissions(MT19337 rng, int offset)
		{
			const int PermissionsSize = 2;
			const int PermissionsCount = 40;

			// lut_ClassEquipBit: ;  FT   TH   BB   RM   WM   BM      KN   NJ   MA   RW   WW   BW
			// .WORD               $800,$400,$200,$100,$080,$040,   $020,$010,$008,$004,$002,$001
			var mask = 0x0820; // Fighter/Knight class bit lut. Each class is a shift of this.
			var order = Enumerable.Range(0, 6).ToList();
			order.Shuffle(rng);

			var oldPermissions = Get(offset, PermissionsSize * PermissionsCount).ToUShorts();
			var newPermissions = oldPermissions.Select(item =>
			{
				UInt16 shuffled = 0x0000;
				for (int i = 0; i < 6; ++i)
				{
					// Shift the mask into each class's slot, then AND with vanilla permission.
					// Shift left to vanilla fighter, shift right into new permission.
					shuffled |= (ushort)(((item & (mask >> i)) << i) >> order[i]);
				}
				return shuffled;
			});

			Put(offset, Blob.FromUShorts(newPermissions.ToArray()));
		}

		public void BattleMagicMenuWrapAround()
		{
			// Allow wrapping up or down in the battle magic menu, see 0C_9C9E_MenuSelection_Magic.asm
			PutInBank(0x0C, 0x9C9E, Blob.FromHex("ADB36829F0C980F057C940F045C920F005C910F01160ADAB6A2903C903D00320D29CEEAB6A60ADAB6A2903D00320D29CCEAB6A60EEF86AADF86A29018DF86AA901200FF2201BF260"));

			// Zero out empty space
			var emptySpace = new byte[0x0A];
			PutInBank(0X0C, 0x9CE6, emptySpace);
		}
		public void EnableCardiaTreasures(MT19337 rng, Map cardia)
		{
			// Assign items to the chests.
			// Incomplete.

			// Put the chests in Cardia
			var room = Map.CreateEmptyRoom((3, 4), 1);
			room[1, 1] = 0x75;
			cardia.Put((0x2A, 0x07), room);
			cardia[0x0B, 0x2B] = (byte)Tile.Doorway;

			room[1, 1] = 0x76;
			cardia.Put((0x26, 0x1C), room);
			cardia[0x20, 0x27] = (byte)Tile.Doorway;
		}

		public void CannotSaveOnOverworld()
		{
			// Hacks the game to disallow saving on the overworld with Tents, Cabins, or Houses
			Put(0x3B2F9, Blob.FromHex("1860"));
			// Change Item using text to avoid confusion
			PutInBank(0x0E, 0x87B0, FF1Text.TextToBytes("\n\nSAVING DISABLED!"));
			PutInBank(0x0E, 0x87E7, FF1Text.TextToBytes("\n\nSAVING DISABLED!"));
			PutInBank(0x0E, 0x8825, FF1Text.TextToBytes("\n\nSAVING DISABLED!"));
		}

		public void CannotSaveAtInns()
		{
			// Hacks the game so that Inns do not save the game
			Put(0x3A53D, Blob.FromHex("EAEAEA"));
			// Change Inn text to avoid confusion
			PutInBank(0x0E, 0x81BB, FF1Text.TextToBytes("Welcome\n  ..\nStay to\nheal\nyour\nwounds?"));
			PutInBank(0x0E, 0x81DC, FF1Text.TextToBytes("Don't\nforget\n.."));
			PutInBank(0x0E, 0x81FC, FF1Text.TextToBytes("Your\ngame\nhasn't\nbeen\nsaved."));
		}
		public void EnableTwelveClasses()
		{
			// Expand characters shown in party creation screen; set to 0C for all promoted classes
			PutInBank(0x1E, 0x80F5, Blob.FromHex("0C"));
			// Reduce count to $10 in PtyGen_DrawChars because we only loaded one row of sprites
			PutInBank(0x1E, 0x8373, Blob.FromHex("EA"));

			// New CHRLoad routine so we can load promoted classes' sprites in memory by only loading one row of sprites instead of two
			var newfCHRLoad = Blob.FromHex("A000B1108D0720C8D0F8E611E611CAD0F160");

			// Put new CHRLoad routine
			PutInBank(0x1F, 0xEAD5, Blob.FromHex("20EBFE"));
			PutInBank(0x1F, 0xFEEB, newfCHRLoad);

			// Update LoadStats routine added by PartyComp to load the right stats for promoted classes
			PutInBank(0x00, 0xB380, Blob.FromHex("BD0061C9FFD013A9019D0161A9009D07619D08619D0961A93160C9069002E9060A0A0A0AA860"));

			// Starting magic for promoted classes. Instead of checking for the class ID of the characters, we compare with the unused class ID in the starting stats array.
			PutInBank(0x1F, 0xC7CA, Blob.FromHex("B940B0"));

			// New promotion routine to not bug out with already promoted classes and allow random promotion; works with Nones; see 11_95AE_DoClassChange.asm
			PutInBank(newTalkRoutinesBank, 0x95AE, Blob.FromHex("A20020C595A24020C595A28020C595A2C020C595E65660BC00613006B9F09D9D006160"));

			// lut for standard promotion, can be modified or randomized
			PutInBank(newTalkRoutinesBank, 0x9DF0, Blob.FromHex("060708090A0B060708090A0B"));
		}

		public void EnableRandomPromotions(Flags flags, MT19337 rng)
		{
			// Need EnableTwelveClasses()
			// Promotions list & class names list
			List<sbyte> promotions = new List<sbyte> { 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B };
			List<string> className = new List<string> { "Fi", "Th", "BB", "RM", "WM", "BM", "Kn", "Ni", "Ma", "RW", "WW", "BW" };

			var levelUpStats = Get(NewLevelUpDataOffset, 588).Chunk(49 * 2);
			var iscaster = new List<bool>();

			for (int i = 0; i < 6; i++)
			{
				if (levelUpStats[i][47 * 2 + 1] == 0)
					iscaster.Add(false);
				else if (levelUpStats[i][47 * 2 + 1] == 0xFF)
					iscaster.Add(true);
				else if ((levelUpStats[i][47 * 2 + 1] & 0x01) != 0)
					iscaster.Add(false);
				else
					iscaster.Add(true);
			}

			iscaster.AddRange(iscaster);

			// Include base classes
			if (flags.IncludeBaseClasses ?? false)
			{
				promotions.AddRange(new List<sbyte> { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05 });
				promotions.Shuffle(rng);
			}
			else
			{
				// If not shuffle list first then add promoted classes so that already promoted classes don't bug
				promotions.Shuffle(rng);
				promotions.AddRange(new List<sbyte> { 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B });
			}

			var recomp = new List<byte> { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

			for (int i = 0; i < 12; i++)
			{
				if (iscaster[i] == false && iscaster[promotions[i]] == true)
					recomp[promotions[i]] = 0x01;
			}

			// Insert randomized promotions
			PutInBank(newTalkRoutinesBank, 0x9DF0, Blob.FromSBytes(promotions.ToArray()));

			// Change class names to spoil to what they randomly promote
			if (flags.RandomPromotionsSpoilers ?? false)
			{
				var itemNames = ReadText(FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, 256);
				for (int i = 0; i < 12; i++)
					itemNames[0xF0 + i] = className[i] + " - " + className[promotions[i]];
				WriteText(itemNames, FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextOffset);
			}

			// Modify DoClassChange, see 1B_910F_ResetMP.asm
			PutInBank(newTalkRoutinesBank, 0x95C8, Blob.FromHex("3009B9F09D9D006120D09D60"));
			PutInBank(newTalkRoutinesBank, 0x9DD0, Blob.FromHex("A91148A9FE48A90648A99148A90F48A91B4C03FE"));

			// MP Recomp Routine, see 1B_910F_ResetMP.asm
			PutInBank(0x1B, 0x910F, Blob.FromHex("60BD0061A8B90091F0F6A96385858684A9281865848584A000A9029184C8A9009184C8C008D0F9BD00610AA8B9718A8582B9728A8583A9018510BD26618511E6118612BD0061AAA001B182A00048B184DD02899005684A4C7691684A900948B184186901918468C8C008D0E118A90265828582A90065838583E610A510C511D0C6A612A96385118610A9201865108510A000B1844A9110C8C008D0F660"));

			// Recomp lut
			PutInBank(0x1B, 0x9100, recomp.ToArray());
		}

		public void EnablePoolParty(Flags flags, MT19337 rng)
		{
			// Need EnableTwelveClasses()
			// New DoPartyGen_OnCharacter and update references; see 1E_85B0_DoPartyGen_OnCharacter.asm
			PutInBank(0x1E, 0x85B0, Blob.FromHex("A667BD01030D41038D4103A9FF8D4003BD0103C900F00718EE40032A90FA20A480A9008522200F82A522EAEAEAEAEAA667AC4003A524F013BD0003C9FFF009BD01034D41038D41034C2C81A525F0118AC900F00AA9009D0103A9FF9D00033860A520290FC561F0B98561C900F0B3C898C9099002A0008C4003B944862C4103F0ED9D0103B942039D0003A901853720B0824CD1858040201008040201"));
			PutInBank(0x1E, 0x8032, Blob.FromHex("B085"));
			PutInBank(0x1E, 0x803B, Blob.FromHex("B085"));
			PutInBank(0x1E, 0x8044, Blob.FromHex("B085"));
			PutInBank(0x1E, 0x804D, Blob.FromHex("B085"));

			// Routine to load the random pool in memory
			PutInBank(0x1E, 0x80C1, Blob.FromHex("A23FBDAA849D0003CA10F7A209BD50869D4003CA10F760"));

			// Zero out free space
			sbyte[] zerofill = new sbyte[0x54];
			PutInBank(0x1E, 0x80D8, Blob.FromSBytes(zerofill));

			// Change reference in NewGamePartyGeneration
			PutInBank(0x1E, 0x801E, Blob.FromHex("20C180EAEAEAEAEAEAEAEA"));

			// Standard party pool lut, byte1 = selection; byte2 = availability mask; byte3-10: characters pool
			PutInBank(0x1E, 0x8650, Blob.FromHex("00FC0001020304050607"));

			int size = 6;
			Blob sizebyte = Blob.FromHex("");

			switch (flags.PoolSize)
			{
				case PoolSize.Size4: size = 4; sizebyte = Blob.FromHex("F0"); break;
				case PoolSize.Size5: size = 5; sizebyte = Blob.FromHex("F8"); break;
				case PoolSize.Size6: size = 6; sizebyte = Blob.FromHex("FC"); break;
				case PoolSize.Size7: size = 7; sizebyte = Blob.FromHex("FE"); break;
				case PoolSize.Size8: size = 8; sizebyte = Blob.FromHex("FF"); break;
			}

			var mainClassList = new List<FF1Class>();

			int fiCount = ((bool)flags.FIGHTER1 ? 1 : 0) + ((bool)flags.FIGHTER2 ? 1 : 0) + ((bool)flags.FIGHTER3 ? 1 : 0) + ((bool)flags.FIGHTER4 ? 1 : 0);
			int thCount = ((bool)flags.THIEF1 ? 1 : 0) + ((bool)flags.THIEF2 ? 1 : 0) + ((bool)flags.THIEF3 ? 1 : 0) + ((bool)flags.THIEF4 ? 1 : 0);
			int bbCount = ((bool)flags.BLACK_BELT1 ? 1 : 0) + ((bool)flags.BLACK_BELT2 ? 1 : 0) + ((bool)flags.BLACK_BELT3 ? 1 : 0) + ((bool)flags.BLACK_BELT4 ? 1 : 0);
			int rmCount = ((bool)flags.RED_MAGE1 ? 1 : 0) + ((bool)flags.RED_MAGE2 ? 1 : 0) + ((bool)flags.RED_MAGE3 ? 1 : 0) + ((bool)flags.RED_MAGE4 ? 1 : 0);
			int wmCount = ((bool)flags.WHITE_MAGE1 ? 1 : 0) + ((bool)flags.WHITE_MAGE2 ? 1 : 0) + ((bool)flags.WHITE_MAGE3 ? 1 : 0) + ((bool)flags.WHITE_MAGE4 ? 1 : 0);
			int bmCount = ((bool)flags.BLACK_MAGE1 ? 1 : 0) + ((bool)flags.BLACK_MAGE2 ? 1 : 0) + ((bool)flags.BLACK_MAGE3 ? 1 : 0) + ((bool)flags.BLACK_MAGE4 ? 1 : 0);
			int knCount = ((bool)flags.KNIGHT1 ? 1 : 0) + ((bool)flags.KNIGHT2 ? 1 : 0) + ((bool)flags.KNIGHT3 ? 1 : 0) + ((bool)flags.KNIGHT4 ? 1 : 0);
			int niCount = ((bool)flags.NINJA1 ? 1 : 0) + ((bool)flags.NINJA2 ? 1 : 0) + ((bool)flags.NINJA3 ? 1 : 0) + ((bool)flags.NINJA4 ? 1 : 0);
			int maCount = ((bool)flags.MASTER1 ? 1 : 0) + ((bool)flags.MASTER2 ? 1 : 0) + ((bool)flags.MASTER3 ? 1 : 0) + ((bool)flags.MASTER4 ? 1 : 0);
			int rwCount = ((bool)flags.RED_WIZ1 ? 1 : 0) + ((bool)flags.RED_WIZ2 ? 1 : 0) + ((bool)flags.RED_WIZ3 ? 1 : 0) + ((bool)flags.RED_WIZ4 ? 1 : 0);
			int wwCount = ((bool)flags.WHITE_WIZ1 ? 1 : 0) + ((bool)flags.WHITE_WIZ2 ? 1 : 0) + ((bool)flags.WHITE_WIZ3 ? 1 : 0) + ((bool)flags.WHITE_WIZ4 ? 1 : 0);
			int bwCount = ((bool)flags.BLACK_WIZ1 ? 1 : 0) + ((bool)flags.BLACK_WIZ2 ? 1 : 0) + ((bool)flags.BLACK_WIZ3 ? 1 : 0) + ((bool)flags.BLACK_WIZ4 ? 1 : 0);

			if (fiCount > 1) mainClassList.Add(FF1Class.Fighter);
			if (thCount > 1) mainClassList.Add(FF1Class.Thief);
			if (bbCount > 1) mainClassList.Add(FF1Class.BlackBelt);
			if (rmCount > 1) mainClassList.Add(FF1Class.RedMage);
			if (wmCount > 1) mainClassList.Add(FF1Class.WhiteMage);
			if (bmCount > 1) mainClassList.Add(FF1Class.BlackMage);
			if (knCount > 1) mainClassList.Add(FF1Class.Knight);
			if (niCount > 1) mainClassList.Add(FF1Class.Ninja);
			if (maCount > 1) mainClassList.Add(FF1Class.Master);
			if (rwCount > 1) mainClassList.Add(FF1Class.RedWiz);
			if (wwCount > 1) mainClassList.Add(FF1Class.WhiteWiz);
			if (bwCount > 1) mainClassList.Add(FF1Class.BlackWiz);

			if (mainClassList.Count == 0) mainClassList = new List<FF1Class> { FF1Class.Fighter, FF1Class.Thief, FF1Class.BlackBelt, FF1Class.RedMage, FF1Class.WhiteMage, FF1Class.BlackMage };

			Blob pool = Blob.FromHex("");
			for (int i = 0; i < size; i++)
				pool += Blob.FromSBytes(new List<sbyte> { (sbyte)mainClassList.PickRandom(rng) }.ToArray());

			// Pool size : 4 0xF0; 5 0xF8; 6 0xFC; 7 0xFE; 8 0xFF)
			PutInBank(0x1E, 0x8650, Blob.FromHex("00") + sizebyte + pool);

			// Starting party composition
			PutInBank(0x1E, 0x84AA, pool.SubBlob(0, 1));
			PutInBank(0x1E, 0x84BA, Blob.FromHex("FF"));
			PutInBank(0x1E, 0x84CA, Blob.FromHex("FF"));
			PutInBank(0x1E, 0x84DA, Blob.FromHex("FF"));
		}
		public void MonsterInABox(MT19337 rng, Flags flags)
		{
			const int lut_TreasureOffset = 0x3100;

			// Replace OpenTreasureChest routine, see 11_8EC0_CheckTrap.asm
			PutInBank(0x1F, 0xDD78, Blob.FromHex("A9002003FEA645BD00B18561A9112003FE20B08E8A60"));

			// Check for trapped monster routine, see 11_8EC0_CheckTrap.asm
			PutInBank(0x11, 0x8EB0, Blob.FromHex("A561202096B030A645BD008FF025856AA9C0203D96A56A200096A903CD866BD00820189668684C43961820E98E201896A2F06020E98E60AA60A911855818A5612093DDA445B90062090499006260"));

			InsertDialogs(0x110, "Monster-in-a-box!"); // 0xC0

			// Select treasure
			var chestList = ItemLocations.AllTreasures.Where(x => x.IsUnused == false && !ItemLists.AllQuestItems.Contains(x.Item)).ToList();
			chestList.Shuffle(rng);
			chestList.RemoveRange(0, chestList.Count() - 40);

			var chestMonsterList = new byte[0x100];
			var treasureList = Get(lut_TreasureOffset, 0x100);

			// Get encounters
			List<byte> encounters;
			encounters = Enumerable.Range(128, FirstBossEncounterIndex).Select(value => (byte)value).ToList();
			encounters.Add(0xFF); // IronGOL

			if ((bool)flags.TrappedChests)
			{
				foreach (var chest in chestList)
					chestMonsterList[(chest.Address - lut_TreasureOffset)] = encounters.SpliceRandom(rng);
			}

			if ((bool)flags.TCMasaGuardian)
			{
				for (int i = 0; i < 0x100; i++)
				{
					if (treasureList[i] == (byte)Item.Masamune)
						chestMonsterList[i] = (byte)WarMECHFormationIndex;
				}
			}

			if ((bool)flags.TrappedShards)
			{
				for (int i = 0; i < 0x100; i++)
				{
					if (treasureList[i] == (byte)Item.Shard)
						chestMonsterList[i] = encounters.SpliceRandom(rng);
				}
			}

			// Insert trapped chest list
			PutInBank(0x11, 0x8F00, chestMonsterList);
		}
		public class TargetNpc
		{
			public ObjectId linkedNPC { get; set; }
			public MapId targetMap { get; set; }
			public (int, int) newPosition { get; set; }
			public Boolean inRoom { get; set; }
			public Boolean stationary { get; set; }
			public String newDialogue { get; set; }

			public TargetNpc(ObjectId objectId, MapId mapid, (int, int) pos, Boolean inroom, Boolean stat, string dialog)
			{
				linkedNPC = objectId;
				targetMap = mapid;
				newPosition = pos;
				inRoom = inroom;
				stationary = stat;
				newDialogue = dialog;
			}
		}

		public void ClassAsNPC(Flags flags, TalkRoutines talkroutines, NPCdata npcdata, List<MapId> flippedmaps, MT19337 rng)
		{
			var crescentSages = new List<ObjectId> { ObjectId.CrescentSage2, ObjectId.CrescentSage3, ObjectId.CrescentSage4, ObjectId.CrescentSage5, ObjectId.CrescentSage6, ObjectId.CrescentSage7, ObjectId.CrescentSage8, ObjectId.CrescentSage9, ObjectId.CrescentSage10 };
			var keyNpc = new List<TargetNpc> {
				new TargetNpc(ObjectId.Princess1, MapId.ConeriaCastle2F, (0x0D, 0x05), true, true, "I won't rest until\nthe Princess is rescued!\n\n..What? Me?"),
				new TargetNpc(ObjectId.Matoya, MapId.MatoyasCave, (0x06,0x03), true, false, "I'm Matoya's apprentice!\n..She only needs me for\nreading her grimoires."),
				new TargetNpc(ObjectId.Bikke, MapId.Pravoka, (0,0), false, true, "It is an evil voyage.\nIf Captain Bikke has his\nway, I will never see\nhome again.\n\nYet I do not fear Kraken.\nI fear the wrath of God."),
				new TargetNpc(ObjectId.ElfDoc, MapId.ElflandCastle, (0x07, 0x05), true, false, "I swore to find a cure\nfor the Prince's curse.\nIf only I could find\nthat elusive Astos.."),
				new TargetNpc(ObjectId.Astos, MapId.NorthwestCastle, (0x11,0x07), true, true, "While the Crown is\nmissing, I can attest\nthat this is indeed\nthe REAL King of\nNorthwest Castle."),
				new TargetNpc(ObjectId.Unne, MapId.Melmond, (0x1D, 0x02), false, true, "I'm also trying\nto discover the secret\nof Lefeinish!"),
				new TargetNpc(ObjectId.Unne, MapId.Lefein, (0,0), false, false, "Lu..pa..?\nLu..pa..?"),
				new TargetNpc(ObjectId.Vampire, MapId.SardasCave, (0x14, 0x01), true, false, "Sarda told me to sort\nthese garlic pots and\nvases until the Vampire\nis killed."),
				new TargetNpc(ObjectId.CanoeSage, MapId.CrescentLake, (0,0), false, true, "I came here to learn\neverything about the\nFiend of Earth. You got\nto respect such a\ndangerous adversary."),
				new TargetNpc(ObjectId.Fairy, MapId.Gaia, (0x2F, 0x14), false, true, "I'm trying to get\nwhat's at the bottom\nof the pond.\n\nMaybe if I drained it.."),
				new TargetNpc(ObjectId.Smith, MapId.DwarfCave, (0x08, 0x02), true, false, "I'm sure it will be a\nbadass sword! Like with\na huge blade, and a gun\nas the hilt, and you can\ntrigger it..\nI can't wait!"),
				new TargetNpc(ObjectId.Nerrick, MapId.DwarfCave, (0x0F, 0x2D), false, true, "Digging a canal is hard\nbut honest work.\n\n..Can't wait to be done\nwith it."),
			};

			var eventNpc = new List<(ObjectId, MapId)> { (ObjectId.ElflandCastleElf3, MapId.ElflandCastle), (ObjectId.MelmondMan1, MapId.Melmond), (ObjectId.MelmondMan3, MapId.Melmond), (ObjectId.MelmondMan4, MapId.Melmond), (ObjectId.MelmondMan8, MapId.Melmond), (ObjectId.DwarfcaveDwarf6, MapId.DwarfCave), (ObjectId.ConeriaCastle1FWoman2, MapId.ConeriaCastle1F), (ObjectId.ElflandElf2, MapId.Elfland), (ObjectId.ElflandElf5, MapId.Elfland) };
			var classSprite = new List<byte> { 0xEE, 0xEF, 0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9 };
			var classNames = new List<string> { "Fighter", "Thief", "Black Belt", "Red Mage", "White Mage", "Black Mage", "Knight", "Ninja", "Master", "Red Wizard", "White Wizard", "Black Wizard" };
			var readyString = new List<string> { "Well, that's that.\nLet's go.", "Onward to new\nadventures!", "I knew you'd come back\nfor me!", "......", "I'm the leader now,\nright?", "The Reaper is always\njust a step behind me..", "O.. Okay.. I hope it's\nnot too scary out there.", "Yes!\nI made it on the team!", "A bold choice, let's\nsee if it pays off.", "Alright, let's do this!", "I obey, master.", "They say I'm the best.", "I see, we have the same\ngoal. Let's join forces.", "My.. name? Huh..", "Just don't put me first\nagainst Kraken.", "I'm taking care of the\nGPs from now on!", "It's Saturday night.\nI've got no date, a\nbottle of Shasta, and\nmy all Rush mixtape.\nLet's rock.", "Life insurance?\nNo, I don't have any.\nWhy?", "Let's put an end to\nthis madness.", "Finally, some action!", "You convinced me. I will\njoin your noble cause.", "Evil never rests. I will\nfight by your side.", "Edward wants to join\nthe party."};

			var baseClassList = new List<FF1Class> { FF1Class.Fighter, FF1Class.Thief, FF1Class.BlackBelt, FF1Class.RedMage, FF1Class.WhiteMage, FF1Class.BlackMage };
			var promoClassList = new List<FF1Class> { FF1Class.Knight, FF1Class.Ninja, FF1Class.Master, FF1Class.RedWiz, FF1Class.WhiteWiz, FF1Class.BlackWiz };
			var selectList = new List<FF1Class>();
			var classList = new List<FF1Class>();

			// Repurpose the LineupMenu to select which character get replaced
			// New Routine when coming from a dialogue
			PutInBank(0x0E, 0x98C0, Blob.FromHex("A565F0034C149AA525D034A524D023A520290CC564F02C8564290CF026C908F007A561186908D005A56138E908291F856160A9008524A8A5610A0A0AAA9002A425686860")); 
			PutInBank(0x0E, 0x9911, Blob.FromHex("A5228565")); // Store joy_select to read it
			PutInBank(0x0E, 0x99D7, Blob.FromHex("20C098")); // Hijack LineupMenu_ProcessJoy
			PutInBank(0x1F, 0xCA5C, Blob.FromHex("201199")); // Jump a bit earlier SM
			PutInBank(0x1F, 0xC1BC, Blob.FromHex("201199")); // Jump a bit earlier OW
			PutInBank(0x1F, 0xCA4E, Blob.FromHex("EAEAEAEA")); // Don't zero out joy_select

			// New talk routine to add class
			var talk_class = talkroutines.Add(Blob.FromHex("A470F005207990903DA571203D9620A49FC000D02BA5739D0061A9009D26619D01619D0B619D0D6120649F20509FA00E207990900320C59520879FA4762073922018964C4396A57260"));

			// Routines to switch the class (clear stats, equipment, new stats, levelup)
			PutInBank(newTalkRoutinesBank, 0x9F50, Blob.FromHex("A91148A9FE48A90648A9C748A98248A9004C03FEA0188610A9618511B110297F9110C8C020D0F5A000A9638511A9009110C8C02FD0F960A91148A9FE48A90648A98748A9A9488A4A4A4A4A4A4A8510A91B4C03FEA91148A9FE48A90648A99948A91048A9008565A90E4C03FE"));

			var totalKeyNPC = (bool)flags.ClassAsNpcKeyNPC ? Math.Min(flags.ClassAsNpcCount, 12) : 0;
			var totalAllNPC = ((bool)flags.ClassAsNpcFiends ? 4 : 0) + totalKeyNPC;

			// Select promoted or base classes list
			if ((bool)flags.ClassAsNpcPromotion)
				selectList = promoClassList;
			else
				selectList = baseClassList;

			selectList.Shuffle(rng);

			// Populate random classes list
			for (int i = 0; i < totalAllNPC; i++)
			{
				if (i < 6 && !(bool)flags.ClassAsNpcDuplicate)
					classList.Add(selectList[i]);
				else
					classList.Add(selectList.PickRandom(rng));
			}

			Dictionary<int, string> newDialogs = new Dictionary<int, string>();

			// Generate the new NPCs
			if ((bool)flags.ClassAsNpcKeyNPC && (flags.ClassAsNpcCount > 0))
			{
				keyNpc.Shuffle(rng);
				var selectedNpc = keyNpc.GetRange(0, totalKeyNPC);

				foreach (var npc in selectedNpc)
				{
					ObjectId targetNpc = ObjectId.None;
					MapId originMap = MapId.Cardia;
					int targetIndex = 16;
					(int, int) targetCoord = (0, 0);
					bool targetInRoom = false;
					bool targetStationary = true;

					// Bikke, Lefein and CanoeSage use local NPCs
					if (npc.linkedNPC == ObjectId.Bikke)
					{
						targetNpc = ObjectId.PravokaMan2;
						originMap = MapId.Pravoka;
						var tempNpc = FindNpc(originMap, targetNpc);
						var bikkeNpc = FindNpc(originMap, ObjectId.Bikke);
						targetIndex = tempNpc.Index;
						targetCoord = (bikkeNpc.Coord.x-1, bikkeNpc.Coord.y-1);
						targetInRoom = tempNpc.InRoom;
						targetStationary = true;
					}
					else if (npc.linkedNPC == ObjectId.Unne && npc.targetMap == MapId.Lefein)
					{
						targetNpc = ObjectId.LefeinMan11;
						originMap = MapId.Lefein;
						var tempNpc = FindNpc(originMap, targetNpc);
						targetIndex = tempNpc.Index;
						targetCoord = tempNpc.Coord;
						targetInRoom = tempNpc.InRoom;
						targetStationary = tempNpc.Stationary;
						var tempdiagid = npcdata.GetTalkArray(targetNpc)[1];
						npcdata.GetTalkArray(targetNpc)[1] = npcdata.GetTalkArray(targetNpc)[2];
						npcdata.GetTalkArray(targetNpc)[2] = tempdiagid;
					}
					else if (npc.linkedNPC == ObjectId.CanoeSage)
					{
						targetNpc = crescentSages.PickRandom(rng);
						originMap = MapId.CrescentLake;
						var tempNpc = FindNpc(originMap, targetNpc);
						targetIndex = tempNpc.Index;
						targetCoord = tempNpc.Coord;
						targetInRoom = tempNpc.InRoom;
						targetStationary = tempNpc.Stationary;
					}
					else // For all the other key NPCs, we kidnap a NPC from another town
					{
						var selectTarget = eventNpc.SpliceRandom(rng);
						targetNpc = selectTarget.Item1;
						originMap = selectTarget.Item2;
						var tempNpc = FindNpc(originMap, targetNpc);
						targetIndex = FindNpc(npc.targetMap, ObjectId.None).Index;
						targetCoord = npc.newPosition;
						targetInRoom = npc.inRoom;
						targetStationary = npc.stationary;
						SetNpc(originMap, tempNpc.Index, ObjectId.None, 0x00, 0x00, false, false);
					}

					SetNpc(npc.targetMap, targetIndex, targetNpc, targetCoord.Item1, targetCoord.Item2, targetInRoom, targetStationary);
					npcdata.GetTalkArray(targetNpc)[0] = (byte)npc.linkedNPC;
					npcdata.GetTalkArray(targetNpc)[3] = (byte)classList.First();

					npcdata.SetRoutine(targetNpc, (newTalkRoutines)talk_class);
					Data[MapObjGfxOffset + (byte)targetNpc] = classSprite[(int)classList.First()];

					newDialogs.Add(npcdata.GetTalkArray(targetNpc)[1], readyString.SpliceRandom(rng) + "\n\n" + classNames[(int)classList.First()] + " joined.");
					newDialogs.Add(npcdata.GetTalkArray(targetNpc)[2], npc.newDialogue);

					classList.RemoveRange(0, 1);
				}
			}

			if ((bool)flags.ClassAsNpcFiends)
			{
				// Check if maps are flipped
				bool earthB5flipped = flippedmaps.Contains(MapId.EarthCaveB5);
				bool volcanoB5flipped = flippedmaps.Contains(MapId.GurguVolcanoB5);
				bool seaB5flipped = flippedmaps.Contains(MapId.SeaShrineB5);

				var dungeonNpc = new List<ObjectId> { ObjectId.MelmondMan6, ObjectId.GaiaMan4, ObjectId.OnracPunk1, ObjectId.GaiaMan1 };

				SetNpc(MapId.Melmond, 8, ObjectId.None, 0x12, 0x18, false, false);
				SetNpc(MapId.Gaia, FindNpc(MapId.Gaia, ObjectId.GaiaMan4).Index, ObjectId.None, 0x12, 0x18, false, false);
				SetNpc(MapId.Onrac, 6, ObjectId.None, 0x12, 0x18, false, false);
				SetNpc(MapId.Gaia, 1, ObjectId.None, 0x12, 0x18, false, false);

				var earthX = earthB5flipped ? (0x3F - ((bool)flags.ClassAsNpcForcedFiends ? 0x0C : 0x0D)) : ((bool)flags.ClassAsNpcForcedFiends ? 0x0C : 0x0D);
				var volcanoX = volcanoB5flipped ? (0x3F - ((bool)flags.ClassAsNpcForcedFiends ? 0x07 : 0x05)) : ((bool)flags.ClassAsNpcForcedFiends ? 0x07 : 0x05);
				var seaX = seaB5flipped ? (0x3F - ((bool)flags.ClassAsNpcForcedFiends ? 0x0C : 0x0A)) : ((bool)flags.ClassAsNpcForcedFiends ? 0x0C : 0x0A);

				SetNpc(MapId.EarthCaveB5, 0x0C, ObjectId.MelmondMan6, earthX, 0x28, true, true);
				SetNpc(MapId.GurguVolcanoB5, 0x02, ObjectId.GaiaMan4, volcanoX, 0x35, true, true);
				SetNpc(MapId.SeaShrineB5, 0x01, ObjectId.OnracPunk1, seaX, 0x07, true, true);
				SetNpc(MapId.SkyPalace5F, 0x02, ObjectId.GaiaMan1, (bool)flags.ClassAsNpcForcedFiends ? 0x07 : 0x09, 0x03, true, true);

				// Restore the default color if Required WarMech is enabled so Tiamat's NPC don't look too weird
				Data[0x029AB] = 0x30; 

				for (int i = 0; i < 4; i++)
				{
					newDialogs.Add(npcdata.GetTalkArray(dungeonNpc[i])[1], readyString.SpliceRandom(rng) + "\n\n" + classNames[(int)classList[i]] + " joined.");
					npcdata.GetTalkArray(dungeonNpc[i])[0] = 0x00;
					npcdata.GetTalkArray(dungeonNpc[i])[3] = (byte)(classList[i]);
					npcdata.SetRoutine(dungeonNpc[i], (newTalkRoutines)talk_class);
					Data[MapObjGfxOffset + (byte)dungeonNpc[i]] = classSprite[(int)classList[i]];
				}
			}

			InsertDialogs(newDialogs);
		}
		public enum shopInfoWordsIndex
		{
			wpAtk = 0,
			wpHit,
			wpCrt,
			arDef,
			arEva,
			targetAllEnemies,
			targetSingleEnemy,
			targetCaster,
			targetAllAllies,
			targetOneAlly,
			elementNone,
			elementNoneShort,
			elementStatus,
			elementStatusShort,
			elementPoison,
			elementPoisonShort,
			elementTime,
			elementTimeShort,
			elementDeath,
			elementDeathShort,
			elementFire,
			elementFireShort,
			elementIce,
			elementIceShort,
			elementLit,
			elementLitShort,
			elementEarth,
			elementEarthShort,
			statusDead,
			statusDeadShort,
			statusStone,
			statusStoneShort,
			statusPoison,
			statusPoisonShort,
			statusBlind,
			statusBlindShort,
			statusStun,
			statusStunShort,
			statusSleep,
			statusSleepShort,
			statusMute,
			statusMuteShort,
			statusConfuse,
			statusConfuseShort,
			routineNull,
			routineDamage,
			routineDmgUndead,
			routineStats,
			routineSlow,
			routineFear,
			routineCure,
			routineHealStats,
			routineDefense,
			routineResistElement,
			routineFast,
			routineRaiseAttack,
			routineReduceEvade,
			routineFullCure,
			routineRaiseEvade,
			routineVoidResist,
			routinePowerWord,
			routineHealPoison,
			routineRevive,
			routineFullRevive,
			routineWarp,
			routineHealStone,
			routineTeleport,
			hurtSpecial,
			hurtDragon,
			hurtGiant,
			hurtUndead,
			hurtWere,
			hurtWater,
			hurtMage,
			hurtRegen
		}

		public static List<string> shopInfoWordsList = new List<string> {
			" ATK +", " HIT +", " CRT +", " DEF +", " EVA -", "All Enemies", "Single Enemy", "Caster",
			"All Allies", "One Ally", "None", "None", "Status", "Stat", "Poison", "Pois", "Time", "Time", "Death", "Deat", "Fire", "Fire",
			"Ice", "Ice", "Lightng", "Lit.", "Earth", "Eart", "Dead", "Dead", "Petrified", "Ptr.", "Poisoned", "Pois", "Blind", "Blnd",
			"Paralyzed", "Para", "Asleep", "Slep", "Silenced", "Mute", "Confused", "Conf", "Null", "Damage", "Dmg Undead", "Inflict Stat",
			"Halve Hits", "Reduce Moral", "Recover HP", "Reduce Stat", "Raise Def.", "Resist Elem.", "Double Hits", "Raise Attack", "Reduce Evade",
			"Full Recover", "Raise Evade", "Void Resist.", "PW Status", "Heal Poison", "Revive", "Full Revive", "Go one floor\n back",
			"Heal Stoned", "Teleport out\n of dungeons", "Magical", "Dragon", "Giant", "Undead", "Were", "Water", "Mage", "Regen"
		};
		public void ShopUpgrade()
		{
			// Modify DrawShopPartySprites to use new DrawOBSprite routines, see 0E_9500_ShopUpgrade.asm
			PutInBank(0x0E, 0xAA04, Blob.FromHex("205795"));
			PutInBank(0x0E, 0xAA0D, Blob.FromHex("205795"));
			PutInBank(0x0E, 0xAA16, Blob.FromHex("205795"));
			PutInBank(0x0E, 0xAA23, Blob.FromHex("4C5795"));

			// Insert new routines in Common Shop Loop 
			PutInBank(0x0E, 0xA931, Blob.FromHex("200095"));

			// Extra routines for Shops info, see 0E_9500_ShopUpgrade.asm
			PutInBank(0x0E, 0x9500, Blob.FromHex("A564C928F038A566C904B035C902B017A20020D495A24020D495A28020D495A2C020D4954C4195A200208B95A240208B95A280208B95A2C0208B954C41952047952027A74C2696A9008DD66A8DDA6A8DDE6A8DE26A6060AA4A8510BD0061A8B9A4EC8511BD0161F011C901F0E9C903F004A9038511A9144C8395A5104A4A4AAABDD66A18651085104C24EC8A8515BD00610AAABD00AD8510BD01AD8511A662BD000338E9B0851229078513A5124A4A4AA8B1108514A613BD38AC2514F005A9004CC595A90E8510A5154A4A4A4AAAA5109DD66A608A8515BD00610AAABDB9BC8512BDBABC8513A662BD000338C944B01638E91C0AAABD50BF25128510BD51BF251305104C1896E9440AAABDA0BF25128510BDA1BF25130510C9019005A9004CC595A90E4CC595A522F073A564C928F06D208D96A90E85572063E0A53E8512A53F8513A662BD0003380AAAB00DBD0093853EBD0193853F4C6396BD0094853EBD0194853FA9118557A90E85582036DEA512853EA513853FA90085222027A7A520C561F0F7A9008522208D964C46E1A9018538A9128539A90E853CA90A853D60"));

			// Modify DrawComplexString, this sets control code 14-19 to use a new words table in bank 11
			//  could be used to move some stuff in items name table and make some space
			//  see 1F_DEBC_DrawComplexString.asm
			PutInBank(0x1F, 0xDEBC, Blob.FromHex("C910B005A2204C83DEC914B07B")); // Change branching to enable CC14
			PutInBank(0x1F, 0xDF44, Blob.FromHex("4CCEDF")); // Jump to routine because we're too far, put in unused char weapons CC
			PutInBank(0x1F, 0xDFCE, Blob.FromHex("A91185572003FE4CA099")); // Routine, put in unused char weapons routine

			// Routine to load the right word from the new words table
			PutInBank(0x11, 0x99A0, Blob.FromHex("A91185578558B13EE63ED002E63F203EE00AAAB00BBD009A853EBD019A4CC899BD009B853EBD019B853F2045DEA90E85584C4EE0"));

			const int weaponOffset = 0x1C; // $28 entries
			const int armorOffset = 0x44; // $28 entries
			const int spellOffset = 0xB0; // $40 entries

			var weaponsData = new List<Weapon>();
			var armorsData = new List<Armor>();

			for (int i = 0; i < WeaponCount; i++)
				weaponsData.Add(new Weapon(i, this));

			for (int i = 0; i < ArmorCount; i++)
				armorsData.Add(new Armor(i, this));

			var spellsData = GetSpells();

			// 12 char per row, 5 rows
			var descriptionsList = new List<string>();

			for (int i = 0; i < weaponOffset; i++)
				descriptionsList.Add("");

			// Insert the new words table
			int offsetWordsPointers = 0x9A00;
			int offsetWords = 0x9B00;
			var pointersWords = new ushort[shopInfoWordsList.Count()];
			Blob generatedWords = Blob.FromHex("");

			for (int i = 0; i < shopInfoWordsList.Count(); i++)
			{
				var blob = FF1Text.TextToBytes(shopInfoWordsList[i], useDTE: true);

				generatedWords += blob;

				pointersWords[i] = (ushort)(offsetWords);
				offsetWords += blob.Length;
			}

			PutInBank(0x11, 0x9B00, generatedWords);
			PutInBank(0x11, offsetWordsPointers, Blob.FromUShorts(pointersWords));

			
			// Build the info boxes
			for (int i = weaponOffset; i < armorOffset; i++)
				descriptionsList.Add("\n" + GenerateWeaponDescription(i - weaponOffset));

			for (int i = armorOffset; i < (armorOffset + 0x28); i++)
				descriptionsList.Add("\n" + GenerateArmorDescription(i - armorOffset));

			for (int i = (armorOffset + 0x28); i < spellOffset; i++)
				descriptionsList.Add("");

			for (int i = spellOffset; i < spellOffset + 0x40; i++)
				descriptionsList.Add(" " + GenerateSpellDescription(i, spellsData[i - spellOffset].Data));

			// Convert all dialogues to bytes
			int offset = 0xA000;
			var pointers = new ushort[descriptionsList.Count()];
			Blob generatedText = Blob.FromHex("");

			for (int i = 0; i < descriptionsList.Count(); i++)
			{
				var blob = new byte[] { 0x02, (byte)i } + FF1Text.TextToBytesInfo(descriptionsList[i], useDTE: true);
				if (blob.Length <= 3)
					blob = new byte[0];

				generatedText += blob;

				pointers[i] = (ushort)(offset);
				offset += blob.Length;
			}

			// Check if dialogs are too long
			if (generatedText.Length > 0x2000)
				throw new Exception("Dialogs maximum length exceeded.");

			// Insert dialogs
			PutInBank(0x11, 0xA000, generatedText);
			PutInBank(0x0E, 0x9300, Blob.FromUShorts(pointers));

		}
		public string GenerateWeaponDescription(int weaponid)
		{
			const int spellOffset = 0xB0; // $40 entries

			var element = new List<(int, string, string)> { (0x00, "¤" + ((int)shopInfoWordsIndex.elementNone).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementNoneShort).ToString("X2")), (0x01, "¤" + ((int)shopInfoWordsIndex.elementStatus).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementStatusShort).ToString("X2")), (0x02, "¤" + ((int)shopInfoWordsIndex.elementPoison).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementPoisonShort).ToString("X2")), (0x04, "¤" + ((int)shopInfoWordsIndex.elementTime).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementTimeShort).ToString("X2")), (0x08, "¤" + ((int)shopInfoWordsIndex.elementDeath).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementDeathShort).ToString("X2")), (0x10, "¤" + ((int)shopInfoWordsIndex.elementFire).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementFireShort).ToString("X2")), (0x20, "¤" + ((int)shopInfoWordsIndex.elementIce).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementIceShort).ToString("X2")), (0x40, "¤" + ((int)shopInfoWordsIndex.elementLit).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementLitShort).ToString("X2")), (0x80, "¤" + ((int)shopInfoWordsIndex.elementEarth).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementEarthShort).ToString("X2")) };
			var hurt = new List<(int, string)> { (0x00, "¤" + ((int)shopInfoWordsIndex.elementNone).ToString("X2")), (0x01, "¤" + ((int)shopInfoWordsIndex.hurtSpecial).ToString("X2")), (0x02, "¤" + ((int)shopInfoWordsIndex.hurtDragon).ToString("X2")), (0x04, "¤" + ((int)shopInfoWordsIndex.hurtGiant).ToString("X2")), (0x08, "¤" + ((int)shopInfoWordsIndex.hurtUndead).ToString("X2")), (0x10, "¤" + ((int)shopInfoWordsIndex.hurtWere).ToString("X2")), (0x20, "¤" + ((int)shopInfoWordsIndex.hurtWater).ToString("X2")), (0x40, "¤" + ((int)shopInfoWordsIndex.hurtMage).ToString("X2")), (0x80, "¤" + ((int)shopInfoWordsIndex.hurtRegen).ToString("X2")) };
			var shortDelimiter = new List<string> { "\n ", ", ", "\n ", ", ", "\n ", ", " };

			var weapondata = new Weapon(weaponid, this);

			var description = "¤" + ((int)shopInfoWordsIndex.wpAtk).ToString("X2") + weapondata.Damage + "\n¤" + ((int)shopInfoWordsIndex.wpHit).ToString("X2") + weapondata.HitBonus + "\n¤" + ((int)shopInfoWordsIndex.wpCrt).ToString("X2") + weapondata.Crit;

			var activeElement = new List<(int, string, string)>();
			var activeHurt = new List<(int, string)>();

			foreach ((int, string, string) effect in element)
				if ((effect.Item1 & weapondata.ElementalWeakness) > 0)
					activeElement.Add(effect);

			foreach ((int, string) effect in hurt)
				if ((effect.Item1 & weapondata.TypeWeakness) > 0)
					activeHurt.Add(effect);

			bool showElement = (weapondata.SpellIndex == 0x00) || (activeHurt.Count == 0);

			if (activeHurt.Count == 0)
				description += "\n";
			else if (activeHurt.Count >= 1 && activeHurt.Count <= 7)
				description += "\n\nHurt " + activeHurt.First().Item2;
			else if (activeHurt.Count == 8)
				description += "\n\nHurt All";

			if (activeElement.Count == 0)
				description += "";
			else if (activeElement.Count >= 1 && activeElement.Count <= 7 && showElement == true)
				description += "\n" + activeElement.First().Item3 + " Element";
			else if (activeElement.Count == 8 && showElement == true)
				description += "\nAll Elements";

			if (weapondata.SpellIndex != 0x00)
				description += "\n" + "Cast $" + ((int)weapondata.SpellIndex + spellOffset - 1).ToString("X2");

			return description;
		}
		public string GenerateArmorDescription(int armorid)
		{
			const int spellOffset = 0xB0; // $40 entries

			var element = new List<(int, string, string)> { (0x00, "¤" + ((int)shopInfoWordsIndex.elementNone).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementNoneShort).ToString("X2")), (0x01, "¤" + ((int)shopInfoWordsIndex.elementStatus).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementStatusShort).ToString("X2")), (0x02, "¤" + ((int)shopInfoWordsIndex.elementPoison).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementPoisonShort).ToString("X2")), (0x04, "¤" + ((int)shopInfoWordsIndex.elementTime).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementTimeShort).ToString("X2")), (0x08, "¤" + ((int)shopInfoWordsIndex.elementDeath).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementDeathShort).ToString("X2")), (0x10, "¤" + ((int)shopInfoWordsIndex.elementFire).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementFireShort).ToString("X2")), (0x20, "¤" + ((int)shopInfoWordsIndex.elementIce).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementIceShort).ToString("X2")), (0x40, "¤" + ((int)shopInfoWordsIndex.elementLit).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementLitShort).ToString("X2")), (0x80, "¤" + ((int)shopInfoWordsIndex.elementEarth).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementEarthShort).ToString("X2")) };
			var status = new List<(int, string, string)> { (0x01, "¤" + ((int)shopInfoWordsIndex.statusDead).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusDeadShort).ToString("X2")), (0x02, "¤" + ((int)shopInfoWordsIndex.statusStone).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusStoneShort).ToString("X2")), (0x04, "¤" + ((int)shopInfoWordsIndex.statusPoison).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusPoisonShort).ToString("X2")), (0x08, "¤" + ((int)shopInfoWordsIndex.statusBlind).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusBlindShort).ToString("X2")), (0x10, "¤" + ((int)shopInfoWordsIndex.statusStun).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusStunShort).ToString("X2")), (0x20, "¤" + ((int)shopInfoWordsIndex.statusSleep).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusSleepShort).ToString("X2")), (0x40, "¤" + ((int)shopInfoWordsIndex.statusMute).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusMuteShort).ToString("X2")), (0x80, "¤" + ((int)shopInfoWordsIndex.statusConfuse).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusConfuseShort).ToString("X2")) };
			var shortDelimiter = new List<string> { "\n ", ", ", "\n ", ", ", "\n ", ", " };

			var armordata = new Armor(armorid, this);

			var description = "¤" + ((int)shopInfoWordsIndex.arDef).ToString("X2") + armordata.Absorb + "\n¤" + ((int)shopInfoWordsIndex.arEva).ToString("X2") + armordata.Weight;

			var activeElementStatus = new List<(int, string, string)>();

			foreach ((int, string, string) effect in element)
				if ((effect.Item1 & armordata.ElementalResist) > 0)
					activeElementStatus.Add(effect);

			if (activeElementStatus.Count == 0)
				description += "\n";
			else if (activeElementStatus.Count == 1)
				description += "\n\nResistance\n " + activeElementStatus[0].Item2;
			else if (activeElementStatus.Count <= 3)
			{
				description += "\n\nResist " + activeElementStatus[0].Item3;

				for (int i = 1; i < activeElementStatus.Count; i++)
					description += shortDelimiter[i - 1] + activeElementStatus[i].Item3;
			}
			else if (activeElementStatus.Count <= 6)
			{
				description += "\n\nResist " + activeElementStatus[0].Item3 + "\n " + activeElementStatus[1].Item3 + "and " + (activeElementStatus.Count - 2) + "+";
			}
			else if (activeElementStatus.Count == 7)
			{
				description += "\n\nResist all\n except ";
				foreach ((int, string, string) effect in status)
					description += (effect.Item1 & armordata.ElementalResist) == 0 ? (effect.Item3) : "";
			}
			else
				description += "\n\nResist all";

			if (armordata.SpellIndex != 0x00)
				description += "\n" + "Cast $" + ((int)armordata.SpellIndex + spellOffset - 1).ToString("X2");

			return description;
		}
		public string GenerateSpellDescription(int spellid, Blob spelldata)
		{
			var target = new List<(int, string)> { (0x01, "¤" + ((int)shopInfoWordsIndex.targetAllEnemies).ToString("X2")), (0x02, "¤" + ((int)shopInfoWordsIndex.targetSingleEnemy).ToString("X2")), (0x04, "¤" + ((int)shopInfoWordsIndex.targetCaster).ToString("X2")), (0x08, "¤" + ((int)shopInfoWordsIndex.targetAllAllies).ToString("X2")), (0x10, "¤" + ((int)shopInfoWordsIndex.targetOneAlly).ToString("X2")) };
			var element = new List<(int, string, string)> { (0x00, "¤" + ((int)shopInfoWordsIndex.elementNone).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementNoneShort).ToString("X2")), (0x01, "¤" + ((int)shopInfoWordsIndex.elementStatus).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementStatusShort).ToString("X2")), (0x02, "¤" + ((int)shopInfoWordsIndex.elementPoison).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementPoisonShort).ToString("X2")), (0x04, "¤" + ((int)shopInfoWordsIndex.elementTime).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementTimeShort).ToString("X2")), (0x08, "¤" + ((int)shopInfoWordsIndex.elementDeath).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementDeathShort).ToString("X2")), (0x10, "¤" + ((int)shopInfoWordsIndex.elementFire).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementFireShort).ToString("X2")), (0x20, "¤" + ((int)shopInfoWordsIndex.elementIce).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementIceShort).ToString("X2")), (0x40, "¤" + ((int)shopInfoWordsIndex.elementLit).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementLitShort).ToString("X2")), (0x80, "¤" + ((int)shopInfoWordsIndex.elementEarth).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementEarthShort).ToString("X2")) };
			var status = new List<(int, string, string)> { (0x01, "¤" + ((int)shopInfoWordsIndex.statusDead).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusDeadShort).ToString("X2")), (0x02, "¤" + ((int)shopInfoWordsIndex.statusStone).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusStoneShort).ToString("X2")), (0x04, "¤" + ((int)shopInfoWordsIndex.statusPoison).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusPoisonShort).ToString("X2")), (0x08, "¤" + ((int)shopInfoWordsIndex.statusBlind).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusBlindShort).ToString("X2")), (0x10, "¤" + ((int)shopInfoWordsIndex.statusStun).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusStunShort).ToString("X2")), (0x20, "¤" + ((int)shopInfoWordsIndex.statusSleep).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusSleepShort).ToString("X2")), (0x40, "¤" + ((int)shopInfoWordsIndex.statusMute).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusMuteShort).ToString("X2")), (0x80, "¤" + ((int)shopInfoWordsIndex.statusConfuse).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusConfuseShort).ToString("X2")) };
			var routine = new List<(int, string)> { (0x00, "¤" + ((int)shopInfoWordsIndex.routineNull).ToString("X2")), (0x01, "¤" + ((int)shopInfoWordsIndex.routineDamage).ToString("X2")), (0x02, "¤" + ((int)shopInfoWordsIndex.routineDmgUndead).ToString("X2")), (0x03, "¤" + ((int)shopInfoWordsIndex.routineStats).ToString("X2")), (0x04, "¤" + ((int)shopInfoWordsIndex.routineSlow).ToString("X2")), (0x05, "¤" + ((int)shopInfoWordsIndex.routineFear).ToString("X2")), (0x06, "¤" + ((int)shopInfoWordsIndex.routineCure).ToString("X2")), (0x07, "¤" + ((int)shopInfoWordsIndex.routineCure).ToString("X2")), (0x08, "¤" + ((int)shopInfoWordsIndex.routineHealStats).ToString("X2")), (0x09, "¤" + ((int)shopInfoWordsIndex.routineDefense).ToString("X2")), (0x0A, "¤" + ((int)shopInfoWordsIndex.routineResistElement).ToString("X2")), (0x0C, "¤" + ((int)shopInfoWordsIndex.routineFast).ToString("X2")), (0x0D, "¤" + ((int)shopInfoWordsIndex.routineRaiseAttack).ToString("X2")), (0x0E, "¤" + ((int)shopInfoWordsIndex.routineReduceEvade).ToString("X2")), (0x0F, "¤" + ((int)shopInfoWordsIndex.routineFullCure).ToString("X2")), (0x10, "¤" + ((int)shopInfoWordsIndex.routineRaiseEvade).ToString("X2")), (0x11, "¤" + ((int)shopInfoWordsIndex.routineVoidResist).ToString("X2")), (0x12, "¤" + ((int)shopInfoWordsIndex.routinePowerWord).ToString("X2")) };
			var oobroutine = new List<(int, string)> { (0x00, "¤" + ((int)shopInfoWordsIndex.routineCure).ToString("X2")), (0x01, "¤" + ((int)shopInfoWordsIndex.routineCure).ToString("X2")), (0x02, "¤" + ((int)shopInfoWordsIndex.routineCure).ToString("X2")), (0x03, "¤" + ((int)shopInfoWordsIndex.routineFullCure).ToString("X2")), (0x04, "¤" + ((int)shopInfoWordsIndex.routineCure).ToString("X2")), (0x05, "¤" + ((int)shopInfoWordsIndex.routineCure).ToString("X2")), (0x06, "¤" + ((int)shopInfoWordsIndex.routineCure).ToString("X2")), (0x07, "¤" + ((int)shopInfoWordsIndex.routineHealPoison).ToString("X2")), (0x08, "¤" + ((int)shopInfoWordsIndex.routineRevive).ToString("X2")), (0x09, "¤" + ((int)shopInfoWordsIndex.routineFullRevive).ToString("X2")), (0x0A, "¤" + ((int)shopInfoWordsIndex.routineWarp).ToString("X2")), (0x0B, "¤" + ((int)shopInfoWordsIndex.routineHealStone).ToString("X2")), (0x0C, "¤" + ((int)shopInfoWordsIndex.routineTeleport).ToString("X2")) };
			var shortDelimiter = new List<string> { "\n ", ", ", "\n ", ", ", "\n ", ", " };
			var oobSpells = new List<int>();

			for(int i = 0; i < oobroutine.Count; i++)
				oobSpells.Add(Get(MagicOutOfBattleOffset + MagicOutOfBattleSize * i, 1)[0]);

			var routineDesc = "";
			var activeElementStatus = new List<(int, string, string)>();

			switch ((int)spelldata[(int)spellDataBytes.Routine])
			{
				case 0:
					routineDesc = oobroutine.Find(x => x.Item1 == oobSpells.FindIndex(x => x == spellid)).Item2;
					break;
				case int n when (n >= 0x01 && n <= 0x02):
					routineDesc = routine.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Routine]).Item2 + "\n " + spelldata[(int)spellDataBytes.Effect] * 2 + "-" + spelldata[(int)spellDataBytes.Effect] * 4 + " DMG";
					break;
				case int n when (n == 0x03 || n == 0x08 || n == 0x12):
					var tempStatus = "";

					foreach ((int, string, string) effect in status)
						if ((effect.Item1 & spelldata[(int)spellDataBytes.Effect]) > 0)
							activeElementStatus.Add(effect);

					if (activeElementStatus.Count == 0)
						tempStatus = "\n None";
					else if (activeElementStatus.Count <= 3)
						tempStatus = string.Join(string.Empty, activeElementStatus.SelectMany(x => "\n " + x.Item2));
					else if (activeElementStatus.Count <= 6)
					{
						for (int i = 0; i < activeElementStatus.Count; i++)
							tempStatus += shortDelimiter[i] + activeElementStatus[i].Item3;
					}
					else if (activeElementStatus.Count == 7)
					{
						tempStatus = "\n All, except";
						foreach ((int, string, string) effect in status)
							tempStatus += (effect.Item1 & spelldata[(int)spellDataBytes.Effect]) == 0 ? ("\n " + effect.Item2) : "";
					}
					else
						tempStatus = "\n All";

					routineDesc = routine.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Routine]).Item2 + tempStatus;
					break;
				case int n when (n == 0x04 || n == 0x0C || n == 0x0F || n == 0x11):
					routineDesc = routine.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Routine]).Item2;
					break;
				case int n when (n == 0x05 || n == 0x0E):
					routineDesc = routine.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Routine]).Item2 + "\n -" + spelldata[(int)spellDataBytes.Effect] + " pts";
					break;
				case int n when (n >= 0x06 && n <= 0x07):
					routineDesc = routine.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Routine]).Item2 + "\n " + spelldata[(int)spellDataBytes.Effect] + "-" + spelldata[(int)spellDataBytes.Effect] * 2 + " HP";
					break;
				case int n when (n == 0x09 || n == 0x10):
					routineDesc = routine.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Routine]).Item2 + "\n +" + spelldata[(int)spellDataBytes.Effect] + " pts";
					break;
				case int n when (n == 0x0A):
					var temp = "";

					foreach ((int, string, string) elem in element)
						if((elem.Item1 & spelldata[(int)spellDataBytes.Effect]) > 0)
							activeElementStatus.Add(elem);

					if (activeElementStatus.Count == 0)
						temp = "\n None";
					else if (activeElementStatus.Count <= 3)
						temp = string.Join(string.Empty, activeElementStatus.SelectMany(x => "\n " + x.Item2));
					else if (activeElementStatus.Count <= 6)
					{ 
						for(int i = 0; i < activeElementStatus.Count; i++)
							temp += shortDelimiter[i] + activeElementStatus[i].Item3;
					}
					else if (activeElementStatus.Count == 7)
					{
						temp = "\n All, except";
						foreach ((int, string, string) elem in element)
							temp += (elem.Item1 & spelldata[(int)spellDataBytes.Effect]) == 0 ? ("\n " + elem.Item2) : "";
					}
					else
						temp = "\n All";

					routineDesc = routine.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Routine]).Item2 + temp;
					break;
				case int n when (n == 0x0D):
					routineDesc = routine.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Routine]).Item2 + "\n +" + spelldata[(int)spellDataBytes.Effect] + " ATK\n +" + spelldata[(int)spellDataBytes.Accuracy] + " HIT";
					break;
			}

			var elementString = (spelldata[(int)spellDataBytes.Element] == 0x40 ? "" : " ") + element.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Element]).Item2;
			var spellstring = elementString + "\n" + target.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Target]).Item2 + "\n\n" + routineDesc;
			return spellstring;
		}

		public void Spooky(TalkRoutines talkroutines, NPCdata npcdata, MT19337 rng, Flags flags)
		{
			const byte FiendsEncounter = 0x77;

			byte encLich1 = 0x7A;
			const byte encLich2 = 0x73;

			const byte bossVamp = 0xFC;
			const byte bossZombie = 0x04;
			const byte bossGhost = 0x46;
			const byte bossGeist = 0x0F;
			const byte bossZomBull = 0xB2;
			const byte bossZombieD = 0xCB;
			const byte bossDracolich = 0x58;
			const byte bossSentinel = 0xFD;
			const byte bossLichMech = 0x7A;

			const byte encVampire = 0x7C;
			const byte encChaos = 0x7B;
			const byte encZombieGhoul = 0x04;
			const byte encGhoulGeist = 0x08;
			const byte encSpecGeist = 0x0F;
			const byte encPhantomGhost = 0x46;
			const byte encAstos = 0x7D;
			const byte encIronGol = 0x58;
			const byte encZombullTroll = 0x32;
			const byte encZombieD = 0x4B;
			const byte encWarMech = 0x56;

			var zombieDialog = new List<byte> { 0x32, 0x33, 0x34, 0x36 };

			Dictionary<int, string> evilDialogs = new Dictionary<int, string>();

			var encountersData = new Encounters(this);

			for (int i = 0; i < 4; i++)
			{
				if (encountersData.formations[FiendsEncounter + i].enemy1 == 0x77)
					encLich1 = (byte)(FiendsEncounter + i);
			}

			// Phantom is Lich, and put Lich1 as Lich2 b-side
			encountersData.formations[encLich2].pattern = FormationPattern.Mixed;
			encountersData.formations[encLich2].spriteSheet = FormationSpriteSheet.ImageGeistWormEye;
			encountersData.formations[encLich2].enemy2 = 0x77;
			encountersData.formations[encLich2].gfxOffset1 = (int)FormationGFX.Sprite4;
			encountersData.formations[encLich2].gfxOffset2 = (int)FormationGFX.Sprite4;
			encountersData.formations[encLich2].palette1 = 0x16;
			encountersData.formations[encLich2].paletteAssign1 = 0;
			encountersData.formations[encLich2].paletteAssign2 = 0;
			encountersData.formations[encLich2].minmax1 = (1, 1);
			encountersData.formations[encLich2].minmax2 = (0, 0);
			encountersData.formations[encLich2].minmaxB1 = (0, 0);
			encountersData.formations[encLich2].minmaxB2 = (1, 1);
			encountersData.formations[encLich2].unrunnableB = true;

			// Add WzVamp to Vampire encounter
			encountersData.formations[encVampire].enemy1 = 0x3D;
			encountersData.formations[encVampire].enemy2 = 0x3C;
			encountersData.formations[encVampire].minmax1 = (1, 1);
			encountersData.formations[encVampire].minmax2 = (0, 0);
			encountersData.formations[encVampire].palette1 = 0x20;
			encountersData.formations[encVampire].palette2 = 0x1F;
			encountersData.formations[encVampire].paletteAssign1 = 0;
			encountersData.formations[encVampire].paletteAssign2 = 1;
			encountersData.formations[encVampire].minmaxB1 = (0, 0);
			encountersData.formations[encVampire].minmaxB2 = (1, 1);
			encountersData.formations[encVampire].unrunnableA = true;
			encountersData.formations[encVampire].unrunnableB = true;

			// Add Sentinel boss (w WarMech sprite) to Astos encounter
			encountersData.formations[encAstos].enemy2 = 0x60;
			encountersData.formations[encAstos].pattern = FormationPattern.Mixed;
			encountersData.formations[encAstos].gfxOffset2 = (int)FormationGFX.Sprite4;
			encountersData.formations[encAstos].palette2 = 0x2F;
			encountersData.formations[encAstos].paletteAssign2 = 1;
			encountersData.formations[encAstos].minmaxB1 = (0, 0);
			encountersData.formations[encAstos].minmaxB2 = (1, 1);
			encountersData.formations[encAstos].unrunnableB = true;

			// Create new zombie encounters to make space, make Geist/Zombie bosses
			encountersData.formations[encZombieGhoul].minmax1 = (1, 1);
			encountersData.formations[encZombieGhoul].minmax2 = (0, 0);
			encountersData.formations[encZombieGhoul].unrunnableA = true;

			encountersData.formations[encGhoulGeist].minmax1 = (0, 2);
			encountersData.formations[encGhoulGeist].minmax2 = (0, 0);
			encountersData.formations[encGhoulGeist].minmax3 = (1, 3);
			encountersData.formations[encGhoulGeist].enemy3 = 0x2B;
			encountersData.formations[encGhoulGeist].gfxOffset3 = (int)FormationGFX.Sprite2;
			encountersData.formations[encGhoulGeist].paletteAssign3 = 1;
			encountersData.formations[encGhoulGeist].minmaxB1 = (0, 3);
			encountersData.formations[encGhoulGeist].minmaxB2 = (1, 4);

			encountersData.formations[encSpecGeist].minmax1 = (0, 0);
			encountersData.formations[encSpecGeist].minmax2 = (1, 1);
			encountersData.formations[encSpecGeist].unrunnableA = true;

			// Replace Phantom with Ghost boss
			encountersData.formations[encPhantomGhost].minmax1 = (1, 1);
			encountersData.formations[encPhantomGhost].minmax2 = (0, 0);
			encountersData.formations[encPhantomGhost].unrunnableA = true;

			// Modify zomBull encounter for zomBull boss
			encountersData.formations[encZombullTroll].minmax1 = (1, 4);
			encountersData.formations[encZombullTroll].minmax2 = (0, 0);
			encountersData.formations[encZombullTroll].minmaxB1 = (1, 1);
			encountersData.formations[encZombullTroll].minmaxB2 = (0, 0);
			encountersData.formations[encZombullTroll].unrunnableB = true;

			// Modify zombieD encounter for zombieD boss
			encountersData.formations[encZombieD].minmax1 = (2, 4);
			encountersData.formations[encZombieD].minmaxB1 = (1, 1);
			encountersData.formations[encZombieD].unrunnableB = true;

			// Modify ironGol encounter for Dracolich (Phantom) boss
			encountersData.formations[encIronGol].enemy2 = Enemy.Phantom;
			encountersData.formations[encIronGol].gfxOffset2 = (int)FormationGFX.Sprite3;
			encountersData.formations[encIronGol].palette2 = 0x16;
			encountersData.formations[encIronGol].paletteAssign2 = 1;
			encountersData.formations[encIronGol].minmax1 = (0, 0);
			encountersData.formations[encIronGol].minmax2 = (1, 1);
			encountersData.formations[encIronGol].unrunnableA = true;

			// Modify Lich1 encounter for Lich? (WarMech) boss
			encountersData.formations[encLich1].pattern = FormationPattern.Fiends;
			encountersData.formations[encLich1].spriteSheet = FormationSpriteSheet.KaryLich;
			encountersData.formations[encLich1].gfxOffset1 = (int)FormationGFX.Sprite3;
			encountersData.formations[encLich1].enemy1 = Enemy.WarMech;
			encountersData.formations[encLich1].minmax1 = (1, 1);
			encountersData.formations[encLich1].palette1 = 0x07;
			encountersData.formations[encLich1].palette2 = 0x07;
			encountersData.formations[encLich1].unrunnableA = true;

			// Lich is Chaos
			encountersData.formations[encChaos].pattern = FormationPattern.Fiends;
			encountersData.formations[encChaos].spriteSheet = FormationSpriteSheet.KaryLich;
			encountersData.formations[encChaos].gfxOffset1 = 0x01;
			encountersData.formations[encChaos].gfxOffset2 = 0x00;
			encountersData.formations[encChaos].palette1 = 0x36;
			encountersData.formations[encChaos].palette2 = 0x37;

			encountersData.Write(this);

			// Update Phantom trap tile
			if (Data[0x0FAD] == encPhantomGhost)
				Data[0x0FAD] = bossDracolich;

			// Switch WarMechEncounter B formation to not get it in Sky
			var FormationsLists = Get(ZoneFormationsOffset, ZoneFormationsSize * ZoneCount);

			for (int i = 0; i < ZoneFormationsSize * ZoneCount; i++)
			{
				switch (FormationsLists[i])
				{
					case bossZombie:
						FormationsLists[i] = encGhoulGeist;
						break;
					case bossGeist:
						FormationsLists[i] = encGhoulGeist + 0x80;
						break;
					case bossZombieD:
						FormationsLists[i] = encZombieD;
						break;
					case bossZomBull:
						FormationsLists[i] = encZombullTroll;
						break;
					case bossGhost:
						FormationsLists[i] = bossDracolich;
						break;
					case encWarMech:
						FormationsLists[i] = bossLichMech;
						break;
					case bossDracolich:
						FormationsLists[i] = encIronGol + 0x80;
						break;
				}
			}

			// Change WarMech encounter in dialogue

			Put(ZoneFormationsOffset, FormationsLists);

			// Make Chaos and WarMech Undead, Phantom a Dragon
			var statsEnemies = Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);
			statsEnemies[0x7F][0x10] |= 0x08; // Chaos
			statsEnemies[0x76][0x10] |= 0x08; // WarMech
			statsEnemies[0x33][0x10] |= 0x02; // Phantom
			Put(EnemyOffset, statsEnemies.SelectMany(enemy => enemy.ToBytes()).ToArray());

			//Update enemies names
			var enemyText = ReadText(EnemyTextPointerOffset, EnemyTextPointerBase, EnemyCount);

			enemyText[0x33] = "DRACLICH"; //Phantom > to DrakLich?
			enemyText[118] = "LICH?"; // WarMech > Lich?
			enemyText[119] = "PHANTOM"; // Lich1 > Phantom
			enemyText[120] = ""; // Lich2 > Phantom
			enemyText[127] = "LICH"; // Chaos > Lich
			WriteText(enemyText, EnemyTextPointerOffset, EnemyTextPointerBase, EnemyTextOffset);

			var lich2name = Get(EnemyTextPointerOffset + 119 * 2, 2); // Lich2 point to Phantom1
			Put(EnemyTextPointerOffset + 120 * 2, lich2name);

			// Scale Undeads
			ScaleSingleEnemyStats(0x15, 125, 125, false, false, null, false, 125, 125, GetEvadeIntFromFlag(flags.EvadeCap)); // Bone
			ScaleSingleEnemyStats(0x16, 125, 125, false, false, null, false, 125, 125, GetEvadeIntFromFlag(flags.EvadeCap)); // R.Bone
			ScaleSingleEnemyStats(0x24, 125, 125, false, false, null, false, 125, 125, GetEvadeIntFromFlag(flags.EvadeCap)); // ZomBull
			ScaleSingleEnemyStats(0x27, 125, 125, false, false, null, false, 125, 125, GetEvadeIntFromFlag(flags.EvadeCap)); // Shadow
			ScaleSingleEnemyStats(0x28, 125, 125, false, false, null, false, 125, 125, GetEvadeIntFromFlag(flags.EvadeCap)); // Image
			ScaleSingleEnemyStats(0x29, 125, 125, false, false, null, false, 125, 125, GetEvadeIntFromFlag(flags.EvadeCap)); // Wraith
			ScaleSingleEnemyStats(0x2A, 125, 125, false, false, null, false, 125, 125, GetEvadeIntFromFlag(flags.EvadeCap)); // Ghost
			ScaleSingleEnemyStats(0x2B, 125, 125, false, false, null, false, 125, 125, GetEvadeIntFromFlag(flags.EvadeCap)); // Zombie
			ScaleSingleEnemyStats(0x2C, 125, 125, false, false, null, false, 125, 125, GetEvadeIntFromFlag(flags.EvadeCap)); // Ghoul
			ScaleSingleEnemyStats(0x2D, 125, 125, false, false, null, false, 125, 125, GetEvadeIntFromFlag(flags.EvadeCap)); // Geist
			ScaleSingleEnemyStats(0x2E, 125, 125, false, false, null, false, 125, 125, GetEvadeIntFromFlag(flags.EvadeCap)); // Specter
			ScaleSingleEnemyStats(0x33, 125, 125, false, false, null, false, 125, 125, GetEvadeIntFromFlag(flags.EvadeCap)); // Phantom
			ScaleSingleEnemyStats(0x3C, 125, 125, false, false, null, false, 125, 125, GetEvadeIntFromFlag(flags.EvadeCap)); // Vampire
			ScaleSingleEnemyStats(0x3D, 125, 125, false, false, null, false, 125, 125, GetEvadeIntFromFlag(flags.EvadeCap)); // WzVampire
			ScaleSingleEnemyStats(0x44, 125, 125, false, false, null, false, 125, 125, GetEvadeIntFromFlag(flags.EvadeCap)); // Zombie D
			ScaleSingleEnemyStats(0x4F, 125, 125, false, false, null, false, 125, 125, GetEvadeIntFromFlag(flags.EvadeCap)); // Mummy
			ScaleSingleEnemyStats(0x50, 125, 125, false, false, null, false, 125, 125, GetEvadeIntFromFlag(flags.EvadeCap)); // WzMummy
			ScaleSingleEnemyStats(0x77, 120, 120, false, false, null, false, 120, 120, GetEvadeIntFromFlag(flags.EvadeCap)); // Lich1
			ScaleSingleEnemyStats(0x78, 120, 120, false, false, null, false, 120, 120, GetEvadeIntFromFlag(flags.EvadeCap)); // Lich2
			ScaleSingleEnemyStats(0x7F, 110, 110, false, false, null, false, 110, 110, GetEvadeIntFromFlag(flags.EvadeCap)); // Chaos

			// Intro
			Blob intro = FF1Text.TextToStory(new string[]
			{
				"I was flipping through the", "",
				"Book of Death when I thought", "",
				"how marvelous it would be to", "",
				"be a necromancer.", "",
				"How fantastic.", "",
				"To go beyond human.", "",
				"A living corpse. An undead.", "",
				"I am damned! I am evil!", "",
				"I MAIM AND KILL", "",
				"I AM A BEAST OF BRUTAL WILL", "",
				"I AM DEATH..", "", "",
				"I. AM. LICH."
			});

			Console.WriteLine(intro.Length);
			System.Diagnostics.Debug.Assert(intro.Length <= 208);
			Put(0x37F20, intro);

			var validTalk = new List<newTalkRoutines> { newTalkRoutines.Talk_norm, newTalkRoutines.Talk_GoBridge, newTalkRoutines.Talk_ifearthfire, newTalkRoutines.Talk_ifearthvamp, newTalkRoutines.Talk_ifevent, newTalkRoutines.Talk_ifitem, newTalkRoutines.Talk_ifkeytnt, newTalkRoutines.Talk_ifvis, newTalkRoutines.Talk_Invis, newTalkRoutines.Talk_4Orb, newTalkRoutines.Talk_kill };
			var invalidZombie = new List<ObjectId> { ObjectId.Bat, ObjectId.GaiaBroom, ObjectId.MatoyaBroom1, ObjectId.MatoyaBroom2, ObjectId.MatoyaBroom3, ObjectId.MatoyaBroom4, ObjectId.MirageRobot1, ObjectId.MirageRobot2, ObjectId.MirageRobot3, ObjectId.SkyRobot, ObjectId.LutePlate, ObjectId.RodPlate };
			var validZombie = new List<ObjectId>();

			if (flags.HintsVillage ?? false)
				invalidZombie.AddRange(new List<ObjectId> { ObjectId.ConeriaOldMan, ObjectId.PravokaOldMan, ObjectId.ElflandScholar1, ObjectId.MelmondOldMan2, ObjectId.CrescentSage11, ObjectId.OnracOldMan2, ObjectId.GaiaWitch, ObjectId.LefeinMan12 });

			if (flags.HintsDungeon ?? false)
				invalidZombie.AddRange(new List<ObjectId> { ObjectId.OnracPunk2, ObjectId.DwarfcaveDwarf4, ObjectId.CardiaDragon2, ObjectId.SkyRobot, ObjectId.Mermaid3 });

			// Change base NPCs' scripts to Talk_fight
			for (int i = 0; i < 0xD0; i++)
			{
				if (validTalk.Contains(npcdata.GetRoutine((ObjectId)i)) && !(invalidZombie.Contains((ObjectId)i)))
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_fight);
					if ((i >= 0x85 && i <= 0x90) || i == 0x9B)
						npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.battle_id] = bossZombieD;
					else
						npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.battle_id] = bossZombie;

					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2] = zombieDialog.PickRandom(rng);
					validZombie.Add((ObjectId)i);
				}
			}

			// New routines to fight and give item
			var battleUnne = talkroutines.Add(Blob.FromHex("A674F005BD2060F01AE67DA572203D96A575200096A476207F902073922018964C4396A57060"));
			var battleGiveOnFlag = talkroutines.Add(Blob.FromHex("A474F0052079909027A5738561202096B020A572203D96A575200096A476207F90207392A5611820109F2018964C4396A9F060A57060"));
			var battleGiveOnItem = talkroutines.Add(Blob.FromHex("A674F006EABD2060F029A5738561202096F022E67DA572203D96A575200096A476207F90207392A5611820109F2018964C4396A57060"));
			var battleBahamut = talkroutines.Add(Blob.FromHex("AD2D60D003A57160E67DA572203D96A575200096A476207F9020739220AE952018964C439660"));
			talkroutines.ReplaceChunk(newTalkRoutines.Talk_Bikke, Blob.FromHex("A57260A57060"), Blob.FromHex("207392A57260"));

			var lichReplace = talkroutines.Add(Blob.FromHex("A572203D96A575200096A476207F90207392A47320A4902018964C4396"));

			// Update Garland's script
			npcdata.SetRoutine(ObjectId.Garland, newTalkRoutines.Talk_CoOGuy);

			// Change dialogues
			evilDialogs.Add(0x32, "Braaaaain!");
			evilDialogs.Add(0x33, "Barf!");
			evilDialogs.Add(0x34, "Uaaaaaargh!");
			evilDialogs.Add(0x36, "Groaaarn!");

			
			evilDialogs.Add(0x04, "What the hell!?\nThat princess is crazy,\nshe tried to bite me!\n\nThat's it. Screw that.\nI'm going home.");

			evilDialogs.Add(0x02, "What is going on!? My\nguard tried to kill me!\nUgh.. this is a deep\nwound.. I don't feel so\nwell..\nGwooorrrgl!\n\nReceived #");
			npcdata.GetTalkArray(ObjectId.King)[(int)TalkArrayPos.battle_id] = bossZombie;
			npcdata.SetRoutine(ObjectId.King, (newTalkRoutines)battleGiveOnFlag);

			evilDialogs.Add(0x06, "So, you are.. the..\nLIGHTarrgaar..\nWarglb..\n\nBraaaain..\n\nReceived #");
			npcdata.GetTalkArray(ObjectId.Princess2)[(int)TalkArrayPos.battle_id] = bossZombie;
			npcdata.SetRoutine(ObjectId.Princess2, (newTalkRoutines)battleGiveOnItem);

			evilDialogs.Add(0x08, "Aaaaarrr! The LIGHT\nWARRIORS have been\ncursed too!\n\nGet 'em, boys!");
			evilDialogs.Add(0x09, "Okay then, guess I'll go\nto the pub, have a nice\ncold pint, and wait for\nall this to blow over.\n\nReceived #");

			evilDialogs.Add(0x0E, "At last I wake up from\nmy eternal slumber.\nCome, LIGHT WARRIORS,\nembrace the darkness,\njoin me in death..\n\nReceived #");
			npcdata.GetTalkArray(ObjectId.ElfPrince)[(int)TalkArrayPos.battle_id] = bossVamp;
			npcdata.SetRoutine(ObjectId.ElfPrince, (newTalkRoutines)battleGiveOnFlag);

			evilDialogs.Add(0x0C, "Yes, yes, the master\nwill be pleased. Let's\nclean this place up\nbefore he wakes.\nStarting with you!");
			npcdata.GetTalkArray(ObjectId.ElfDoc)[(int)TalkArrayPos.battle_id] = bossGeist;
			npcdata.SetRoutine(ObjectId.ElfDoc, (newTalkRoutines)battleUnne);

			if (npcdata.GetRoutine(ObjectId.Astos) != newTalkRoutines.Talk_Astos)
			{
				evilDialogs.Add(0x12, "Did you ever dance with\nthe devil in the pale\nmoonlight?\n\nReceived #");
				npcdata.GetTalkArray(ObjectId.Astos)[(int)TalkArrayPos.battle_id] = bossVamp;
				npcdata.SetRoutine(ObjectId.Astos, (newTalkRoutines)battleGiveOnItem);
			}

			evilDialogs.Add(0x13, "The world is going to\nhell, but this won't\nstop me from digging\nmy canal!");
			evilDialogs.Add(0x14, "Excellent! Finally,\nnow Lich's undead army\ncan flow through the\nrest of the world!\n\nReceived #");
			npcdata.GetTalkArray(ObjectId.Nerrick)[(int)TalkArrayPos.battle_id] = bossVamp;
			npcdata.SetRoutine(ObjectId.Nerrick, (newTalkRoutines)battleGiveOnItem);

			evilDialogs.Add(0x15, "I never thought I'd\nhave to forge the\nweapon that would slay\nmy brothers. Bring me\nADAMANT, quick!");
			evilDialogs.Add(0x16, "You were too slow,\nLIGHT WARRIORS. You have\nforsaken me!\nJoin my damned soul in\nthe afterworld!\n\nReceived #");
			npcdata.GetTalkArray(ObjectId.Smith)[(int)TalkArrayPos.battle_id] = bossGhost;
			npcdata.SetRoutine(ObjectId.Smith, (newTalkRoutines)battleGiveOnItem);

			evilDialogs.Add(0x17, "Pfah! Everyone else can\nrot in Hell for all\nI care, I'm  perfectly\nsafe here!");
			evilDialogs.Add(0x19, "SCRIIIIIIIIIIIIIIIIIIII!\n\nReceived #");
			npcdata.GetTalkArray(ObjectId.Matoya)[(int)TalkArrayPos.battle_id] = bossGeist;
			npcdata.SetRoutine(ObjectId.Matoya, (newTalkRoutines)battleGiveOnItem);

			evilDialogs.Add(0x1C, "Now, listen to me, a\nbasic word from\nLeifeinish is Lu..\nHack! Cough! Sorry,\nLu..lu..paaaargh!");
			npcdata.GetTalkArray(ObjectId.Unne)[(int)TalkArrayPos.battle_id] = bossGeist;
			npcdata.SetRoutine(ObjectId.Unne, (newTalkRoutines)battleUnne);

			evilDialogs.Add(0x1D, "Ah, humans who wish to\npay me tribute. What?\nYou miserable little\npile of secrets!\nEnough talk! Have at you!");

			evilDialogs.Add(0x1E, "I.. HUNGER!\n\nReceived #");
			npcdata.GetTalkArray(ObjectId.Sarda)[(int)TalkArrayPos.battle_id] = bossZomBull;
			npcdata.SetRoutine(ObjectId.Sarda, (newTalkRoutines)battleGiveOnFlag);

			evilDialogs.Add(0x20, "The TAIL! Impressive..\nYes, yes, you are indeed\nworthy..\n\nWorthy of dying by my\nown claws!");
			npcdata.GetTalkArray(ObjectId.Bahamut)[(int)TalkArrayPos.battle_id] = bossDracolich;
			npcdata.GetTalkArray(ObjectId.Bahamut)[3] = 0x1F;
			npcdata.SetRoutine(ObjectId.Bahamut, (newTalkRoutines)battleBahamut);

			evilDialogs.Add(0x23, "Come play with me,\nLIGHT WARRIORS.\nFor ever and ever\nand ever..\n\nReceived #");
			npcdata.GetTalkArray(ObjectId.Fairy)[(int)TalkArrayPos.battle_id] = bossGhost;
			npcdata.SetRoutine(ObjectId.Fairy, (newTalkRoutines)battleGiveOnItem);

			evilDialogs.Add(0x27, "Exterminate.\n\n\n\n\nReceived #");
			npcdata.GetTalkArray(ObjectId.CubeBot)[(int)TalkArrayPos.battle_id] = bossSentinel;
			npcdata.SetRoutine(ObjectId.CubeBot, (newTalkRoutines)battleGiveOnItem);

			evilDialogs.Add(0x2B, "My friends..\nMy colleagues..\nNow.. I join them..\n\nReceived #");
			npcdata.GetTalkArray(ObjectId.CanoeSage)[(int)TalkArrayPos.battle_id] = bossZomBull;
			npcdata.SetRoutine(ObjectId.CanoeSage, (newTalkRoutines)battleGiveOnItem);

			evilDialogs.Add(0xCD, "Luuuuu.. paaaargh!\n\n\n\nReceived #");
			npcdata.GetTalkArray(ObjectId.Lefein)[(int)TalkArrayPos.battle_id] = bossZomBull;
			npcdata.SetRoutine(ObjectId.Lefein, (newTalkRoutines)battleGiveOnFlag);

			evilDialogs.Add(0xFA, "Sorry, LIGHT WARRIORS,\nbut your LICH is in\nanother castle!\n\nMwahahahaha!");
			evilDialogs.Add(0x2F, "You did well fighting\nmy Army of Darkness,\nLIGHT WARRIORS! But it\nis for naught!\nI am UNSTOPPABLE!\nThis time, YOU are\nthe SPEEDBUMP!");
			evilDialogs.Add(0x30, "HAHA! Alright, enough\nplaying around.");

			InsertDialogs(evilDialogs);

			// Update Chaos sprite
			Put(0x2F00 + 0x18, Blob.FromHex("00"));
			Put(0x2F00 + 0x19, Blob.FromHex("01"));
			Put(0x2F00 + 0x1A, Blob.FromHex("00"));
			Data[MapObjGfxOffset + 0x1A] = 0x0F;
			Data[MapObjGfxOffset + 0x19] = 0x0F;
			PutInBank(0x00, 0xA000 + ((byte)MapId.TempleOfFiendsRevisitedChaos * 0x30) + 0x18, Blob.FromHex("0F0F13300F0F1530"));

			npcdata.SetRoutine((ObjectId)0x19, (newTalkRoutines)lichReplace);

			for (int i = 0; i < 4; i++)
			{
				if (npcdata.GetTalkArray((ObjectId)(0x1B + i))[(int)TalkArrayPos.battle_id] == encLich1)
					npcdata.GetTalkArray((ObjectId)(0x1B + i))[(int)TalkArrayPos.battle_id] = encLich2 + 0x80;
			}

			npcdata.GetTalkArray((ObjectId)0x19)[0] = 0x2F;
			npcdata.GetTalkArray((ObjectId)0x19)[(int)TalkArrayPos.battle_id] = bossLichMech;
			npcdata.GetTalkArray((ObjectId)0x19)[2] = 0x2F;
			npcdata.GetTalkArray((ObjectId)0x19)[3] = 0x1A;

			npcdata.GetTalkArray(ObjectId.WarMECH)[(int)TalkArrayPos.battle_id] = bossLichMech;

			// Switch princess
			Data[MapSpriteOffset + ((byte)MapId.TempleOfFiends * MapSpriteCount + 1) * MapSpriteSize] = (byte)ObjectId.Princess2;
			Data[MapSpriteOffset + ((byte)MapId.ConeriaCastle2F * MapSpriteCount + 1) * MapSpriteSize] = (byte)ObjectId.None;
			Put(0x2F00 + 0x12, Blob.FromHex("01"));
			Put(0x2F00 + 0x03, Blob.FromHex("02"));

			// Change NPC's color 
			Data[0x2000 + ((byte)MapId.Coneria * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapId.Coneria * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapId.ConeriaCastle1F * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapId.ConeriaCastle1F * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapId.ConeriaCastle2F * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapId.ConeriaCastle2F * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapId.Pravoka * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapId.Pravoka * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapId.Elfland * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapId.Elfland * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapId.ElflandCastle * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapId.ElflandCastle * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapId.DwarfCave * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapId.DwarfCave * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapId.Melmond * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapId.Melmond * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapId.CrescentLake * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapId.CrescentLake * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapId.Gaia * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapId.Gaia * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapId.Onrac * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapId.Onrac * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapId.SeaShrineB1 * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapId.SeaShrineB1 * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapId.Lefein * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapId.Lefein * 0x30) + 0x18 + 0x07] = 0x3A;


			// Let zombies roam free
			var npcMap = new List<MapId> { MapId.Cardia, MapId.BahamutsRoomB2, MapId.Coneria, MapId.ConeriaCastle1F, MapId.ConeriaCastle2F, MapId.CrescentLake, MapId.DwarfCave, MapId.Elfland, MapId.ElflandCastle, MapId.Gaia, MapId.Lefein, MapId.Melmond, MapId.Onrac, MapId.Pravoka };

			foreach (var map in npcMap)
			{
				for (var i = 0; i < 0x10; i++)
				{
					int offset = MapSpriteOffset + ((byte)map * MapSpriteCount + i) * MapSpriteSize;
					if (validZombie.Contains((ObjectId)Data[offset]))
						Data[offset + 1] &= 0b10111111;
				}
			}
		}
	}
}

