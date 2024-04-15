using System.ComponentModel;

namespace FF1Lib
{
	public enum PoolSize
	{
		[Description("4 Characters")]
		Size4,
		[Description("5 Characters")]
		Size5,
		[Description("6 Characters")]
		Size6,
		[Description("7 Characters")]
		Size7,
		[Description("8 Characters")]
		Size8,
	}

	public partial class FF1Rom : NesRom
	{
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

		public enum FF1Class
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


		public void PartyGeneration(MT19337 rng, Flags flags, Preferences preferences)
		{
			PartyComposition(rng, flags);

			EnableNones((byte)preferences.MapmanSlot);
			AnyClassGainMP();
			EnableTwelveClasses();

			if((bool)flags.EnablePoolParty)
			{ 
				EnablePoolParty(flags, rng);
			}
		}
		void UpdateCharacterFromOptions(int slotNumber, bool forced, IList<FF1Class> options, MT19337 rng)
		{
			const int lut_PtyGenBuf = 0x784AA;       // offset for party generation buffer LUT
			const int lut_ClassPreferences = 0x78114;  // classes LUT

			if (forced) // if forced
			{
				FF1Class forcedclass;
				if (options.Any())
				{
					forcedclass = options.PickRandom(rng);
				}
				// No available classes from flags - choose an unpromoted class at random
				else
				{
					forcedclass = (FF1Class)(Enum.GetValues(typeof(FF1Class))).
						GetValue(rng.Between(0, 5));
				}
				options.Clear();
				options.Add(forcedclass);
			}

			// Just update allowed bitmasks for default classes if no selection, then exit
			if (!options.Any())
			{
				foreach (FF1Class option in DefaultChoices)
				{
					Data[lut_ClassPreferences + (int)option + 1] |= AllowedSlotBitmasks[(slotNumber - 1)];
				}
				return;
			}

			//byte allowedFlags = 0b0000_0000;
			foreach (FF1Class option in options)
			{
				Data[lut_ClassPreferences + (((int)option == 12) ? 0 : (int)option + 1)] |= AllowedSlotBitmasks[(slotNumber - 1)];
			}

			// set default member
			var defaultclass = (forced || !DefaultChoices.SequenceEqual(options)) ? (int)options.PickRandom(rng) : slotNumber - 1;
			Data[lut_PtyGenBuf + (slotNumber - 1) * 0x10] = defaultclass == 12 ? (byte)0xFF : (byte)defaultclass;

			options.Clear();
		}
		public void PartyComposition(MT19337 rng, Flags flags)
		{
			var options = new List<FF1Class>();

			// Set bitmask for each slots (AllowedSlotBitmasks)
			PutInBank(0x1E, 0x8110, Blob.FromHex("01020408"));

			// Zero out allowed classes lut since we're going to bitwise OR it
			PutInBank(0x1E, 0x8114, Blob.FromHex("00000000000000000000000000"));

			// Link flags and classes
			var classSelection = new List<(bool, bool, bool, bool, FF1Class)> {
				((bool)flags.FIGHTER1, (bool)flags.FIGHTER2, (bool)flags.FIGHTER3, (bool)flags.FIGHTER4, FF1Class.Fighter ),
				((bool)flags.THIEF1, (bool)flags.THIEF2, (bool)flags.THIEF3, (bool)flags.THIEF4, FF1Class.Thief ),
				((bool)flags.BLACK_BELT1, (bool)flags.BLACK_BELT2, (bool)flags.BLACK_BELT3, (bool)flags.BLACK_BELT4, FF1Class.BlackBelt ),
				((bool)flags.RED_MAGE1, (bool)flags.RED_MAGE2, (bool)flags.RED_MAGE3, (bool)flags.RED_MAGE4, FF1Class.RedMage ),
				((bool)flags.WHITE_MAGE1, (bool)flags.WHITE_MAGE2, (bool)flags.WHITE_MAGE3, (bool)flags.WHITE_MAGE4, FF1Class.WhiteMage ),
				((bool)flags.BLACK_MAGE1, (bool)flags.BLACK_MAGE2, (bool)flags.BLACK_MAGE3, (bool)flags.BLACK_MAGE4, FF1Class.BlackMage ),
				(false, (bool)flags.NONE_CLASS2, (bool)flags.NONE_CLASS3, (bool)flags.NONE_CLASS4, FF1Class.None ),
				((bool)flags.KNIGHT1, (bool)flags.KNIGHT2, (bool)flags.KNIGHT3, (bool)flags.KNIGHT4, FF1Class.Knight ),
				((bool)flags.NINJA1, (bool)flags.NINJA2, (bool)flags.NINJA3, (bool)flags.NINJA4, FF1Class.Ninja ),
				((bool)flags.MASTER1, (bool)flags.MASTER2, (bool)flags.MASTER3, (bool)flags.MASTER4, FF1Class.Master ),
				((bool)flags.RED_WIZ1, (bool)flags.RED_WIZ2, (bool)flags.RED_WIZ3, (bool)flags.RED_WIZ4, FF1Class.RedWiz ),
				((bool)flags.WHITE_WIZ1, (bool)flags.WHITE_WIZ2, (bool)flags.WHITE_WIZ3, (bool)flags.WHITE_WIZ4, FF1Class.WhiteWiz ),
				((bool)flags.BLACK_WIZ1, (bool)flags.BLACK_WIZ2, (bool)flags.BLACK_WIZ3, (bool)flags.BLACK_WIZ4, FF1Class.BlackWiz ),
			};

			// Do each slot
			options = classSelection.Where(x => x.Item1 == true).Select(x => x.Item5).ToList();
			UpdateCharacterFromOptions(1, (flags.FORCED1 ?? false), options, rng);

			options = classSelection.Where(x => x.Item2 == true).Select(x => x.Item5).ToList();
			UpdateCharacterFromOptions(2, (flags.FORCED2 ?? false), options, rng);

			options = classSelection.Where(x => x.Item3 == true).Select(x => x.Item5).ToList();
			UpdateCharacterFromOptions(3, (flags.FORCED3 ?? false), options, rng);

			options = classSelection.Where(x => x.Item4 == true).Select(x => x.Item5).ToList();
			UpdateCharacterFromOptions(4, (flags.FORCED4 ?? false), options, rng);
		}
		public void AnyClassGainMP()
		{
			// Spell level up change to allow any class to gain spell charges, see 1B_8818_LvlUp_LevelUp.asm
			PutInBank(0x1B, 0x88D7, Blob.FromHex("AE8E68A001B182A02848B184DD02899005684A4CFA88684A900948B184186901918468C8C030D0E14C1C89000000090909040400090909"));
		}
		public void EnableNones(byte mapmanSlot)
		{
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
			byte leader = (byte)((byte)mapmanSlot << 6);
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
		}

		public void PubReplaceClinic(MT19337 rng, MapIndex attackedTown, Flags flags)
		{
			if (!(bool)flags.RecruitmentMode)
			{
				return;
			}

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
			while (pub_lut.Count < 8)
			{
				options.Shuffle(rng);
				pub_lut.AddRange(options);
			}
			if (!(bool)flags.MelmondClinic && !(bool)flags.RandomVampAttack)
			{
				pub_lut.Insert(3, (byte)0xFF);
			}
			else if ((bool)flags.RandomVampAttack)
			{
				pub_lut.Insert((int)attackedTown, (byte)0xFF);
			}

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

			int levelUpTargetA = (bool)flags.RandomizeClass ? 0xB5 : 0x87;
			int levelUpTargetB = (bool)flags.RandomizeClass ? 0xFF : 0xA9;
			
			// New routine to level up replaced character and zero some stuff, needs new level up stuff in bank 1B
			PutInBank(0x0E, 0x9D34, Blob.FromHex($"AD0D03F018A99D48A95048A9{levelUpTargetA:X2}48A9{levelUpTargetB:X2}488A182A2A2A8510A91B4C03FE4C0098"));
			PutInBank(0x0E, 0x9800, Blob.FromHex($"A9008D24008D25008D012060"));

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
			PutInBank(0x11, 0x95AE, Blob.FromHex("A20020C595A24020C595A28020C595A2C020C595E65660BC00613006B9F09D9D006160"));

			// lut for standard promotion, can be modified or randomized
			PutInBank(0x11, 0x9DF0, Blob.FromHex("060708090A0B060708090A0B"));
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
			PutInBank(0x11, 0x9DF0, Blob.FromSBytes(promotions.ToArray()));

			// Change class names to spoil to what they randomly promote
			if (flags.RandomPromotionsSpoilers ?? false)
			{
				for (int i = 0; i < 12; i++)
					ItemsText[0xF0 + i] = className[i] + " - " + className[promotions[i]];
			}

			// Modify DoClassChange, see 1B_910F_ResetMP.asm
			PutInBank(0x11, 0x95C8, Blob.FromHex("3009B9F09D9D006120D09D60"));
			PutInBank(0x11, 0x9DD0, Blob.FromHex("A91148A9FE48A90648A99148A90F48A91B4C03FE"));

			// MP Recomp Routine, see 1B_910F_ResetMP.asm
			PutInBank(0x1B, 0x910F, Blob.FromHex("60BD0061A8B90091F0F6A96385858684A9281865848584A000A9029184C8A9009184C8C008D0F9BD00610AA8B9718A8582B9728A8583A9018510BD26618511E6118612BD0061AAA001B182A00048B184DD02899005684A4C7691684A900948B184186901918468C8C008D0E118A90265828582A90065838583E610A510C511D0C6A612A96385118610A9201865108510A000B1844A9110C8C008D0F660"));

			// Recomp lut
			PutInBank(0x1B, 0x9100, recomp.ToArray());
		}

		public void EnablePoolParty(Flags flags, MT19337 rng)
		{
			string selectButtonCode = "EAEAEAEAEA";

			if ((bool)flags.RandomizeClass || (bool)flags.Transmooglifier || (bool)flags.RandomizeClassChaos)
			{
				selectButtonCode = "F0034C0088";
			}

			// New DoPartyGen_OnCharacter and update references; see 1E_85B0_DoPartyGen_OnCharacter.asm
			PutInBank(0x1E, 0x85B0, Blob.FromHex($"A667BD01030D41038D4103A9FF8D4003BD0103C900F00718EE40032A90FA20A480A9008522200F82A522{selectButtonCode}A667AC4003A524F013BD0003C9FFF009BD01034D41038D41034C2C81A525F0118AC900F00AA9009D0103A9FF9D00033860A520290FC561F0B98561C900F0B3C898C9099002A0008C4003B944862C4103F0ED9D0103B942039D0003A901853720B0824CD1858040201008040201"));
			PutInBank(0x1E, 0x8032, Blob.FromHex("B085"));
			PutInBank(0x1E, 0x803B, Blob.FromHex("B085"));
			PutInBank(0x1E, 0x8044, Blob.FromHex("B085"));
			PutInBank(0x1E, 0x804D, Blob.FromHex("B085"));

			// Routine to load the random pool in memory
			PutInBank(0x1E, 0x80C1, Blob.FromHex("A23FBDAA849D0003CA10F7A209BD50869D4003CA10F760"));

			// Zero out free space
			byte[] zerofill = new byte[0x54];
			PutInBank(0x1E, 0x80D8, zerofill);

			// Change reference in NewGamePartyGeneration
			PutInBank(0x1E, 0x801E, Blob.FromHex("20C180EAEAEAEAEAEAEAEA"));

			// Standard party pool lut, byte1 = selection; byte2 = availability mask; byte3-10: characters pool
			PutInBank(0x1E, 0x8650, Blob.FromHex("00FC0001020304050607"));

			List<(PoolSize pool, int size, byte mask)> parameters = new()
			{
				(PoolSize.Size4, 4, 0xF0),
				(PoolSize.Size5, 5, 0xF8),
				(PoolSize.Size6, 6, 0xFC),
				(PoolSize.Size7, 7, 0xFE),
				(PoolSize.Size8, 8, 0xFF),
			};

			var selectedParameters = parameters.Find(x => x.pool == flags.PoolSize);

			List<(bool, FF1Class)> flagsClass = new()
			{
				((bool)flags.DraftFighter, FF1Class.Fighter),
				((bool)flags.DraftThief, FF1Class.Thief),
				((bool)flags.DraftBlackBelt, FF1Class.BlackBelt),
				((bool)flags.DraftRedMage, FF1Class.RedMage),
				((bool)flags.DraftWhiteMage, FF1Class.WhiteMage),
				((bool)flags.DraftBlackMage, FF1Class.BlackMage),
				((bool)flags.DraftKnight, FF1Class.Knight),
				((bool)flags.DraftNinja, FF1Class.Ninja),
				((bool)flags.DraftMaster, FF1Class.Master),
				((bool)flags.DraftRedWiz, FF1Class.RedWiz),
				((bool)flags.DraftWhiteWiz, FF1Class.WhiteWiz),
				((bool)flags.DraftBlackWiz, FF1Class.BlackWiz),
			};

			List<FF1Class> allowedClasses = flagsClass.Where(x => x.Item1 == true).Select(x => x.Item2).ToList();

			if (!allowedClasses.Any())
			{
				allowedClasses = new () { FF1Class.Fighter, FF1Class.Thief, FF1Class.BlackBelt, FF1Class.RedMage, FF1Class.WhiteMage, FF1Class.BlackMage };
			}

			List<byte> selectedClasses = new();

			if (flags.SafePoolParty)
			{
				var meleeList = allowedClasses.Where(c => c == FF1Class.Fighter || c == FF1Class.Thief || c == FF1Class.BlackBelt || c == FF1Class.Knight || c == FF1Class.Ninja || c == FF1Class.Master).ToList();
				if (meleeList.Count < 2) meleeList = new List<FF1Class> { FF1Class.Fighter, FF1Class.Thief, FF1Class.BlackBelt };

				selectedClasses.Add((byte)meleeList.SpliceRandom(rng));

				var mageList = allowedClasses.Where(c => c == FF1Class.RedMage || c == FF1Class.WhiteMage || c == FF1Class.BlackMage || c == FF1Class.RedWiz || c == FF1Class.WhiteWiz || c == FF1Class.BlackWiz).ToList();
				if (mageList.Count < 2) mageList = new List<FF1Class> { FF1Class.RedMage, FF1Class.WhiteMage, FF1Class.BlackMage };

				selectedClasses.Add((byte)mageList.SpliceRandom(rng));
				selectedClasses.Add((byte)mageList.SpliceRandom(rng));
			}

			while (selectedClasses.Count < selectedParameters.size)
			{
				selectedClasses.Add((byte)allowedClasses.PickRandom(rng));
			}

			// Pool size : 4 0xF0; 5 0xF8; 6 0xFC; 7 0xFE; 8 0xFF)
			PutInBank(0x1E, 0x8650, new byte[] { 0x00, selectedParameters.mask });
			PutInBank(0x1E, 0x8652, selectedClasses.ToArray());

			// Starting party composition
			PutInBank(0x1E, 0x84AA, new byte[] { selectedClasses[0], 0x80 });
			PutInBank(0x1E, 0x84BA, Blob.FromHex("FF"));
			PutInBank(0x1E, 0x84CA, Blob.FromHex("FF"));
			PutInBank(0x1E, 0x84DA, Blob.FromHex("FF"));
		}
		public class TargetNpc
		{
			public ObjectId linkedNPC { get; set; }
			public MapIndex targetMap { get; set; }
			public (int, int) newPosition { get; set; }
			public Boolean inRoom { get; set; }
			public Boolean stationary { get; set; }
			public String newDialogue { get; set; }

			public TargetNpc(ObjectId objectId, MapIndex MapIndex, (int, int) pos, Boolean inroom, Boolean stat, string dialog)
			{
				linkedNPC = objectId;
				targetMap = MapIndex;
				newPosition = pos;
				inRoom = inroom;
				stationary = stat;
				newDialogue = dialog;
			}
		}

		public readonly List<string> ClassNames = new List<string> { "Fighter", "Thief", "Black Belt", "Red Mage", "White Mage", "Black Mage", "Knight", "Ninja", "Master", "Red Wizard", "White Wizard", "Black Wizard" };

		public void ClassAsNPC(Flags flags, TalkRoutines talkroutines, NPCdata npcdata, List<MapIndex> flippedmaps, List<MapIndex> vflippedmaps, MT19337 rng)
		{
			var crescentSages = new List<ObjectId> { ObjectId.CrescentSage2, ObjectId.CrescentSage3, ObjectId.CrescentSage4, ObjectId.CrescentSage5, ObjectId.CrescentSage6, ObjectId.CrescentSage7, ObjectId.CrescentSage8, ObjectId.CrescentSage9, ObjectId.CrescentSage10 };
			var keyNpc = new List<TargetNpc> {
				new TargetNpc(ObjectId.Princess1, MapIndex.ConeriaCastle2F, (0x0D, 0x05), true, true, "I won't rest until\nthe Princess is rescued!\n\n..What? Me?"),
				new TargetNpc(ObjectId.Matoya, MapIndex.MatoyasCave, (0x06,0x03), true, false, "I'm Matoya's apprentice!\n..She only needs me for\nreading her grimoires."),
				new TargetNpc(ObjectId.Bikke, MapIndex.Pravoka, (0,0), false, true, "It is an evil voyage.\nIf Captain Bikke has his\nway, I will never see\nhome again.\n\nYet I do not fear Kraken.\nI fear the wrath of God."),
				new TargetNpc(ObjectId.ElfDoc, MapIndex.ElflandCastle, (0x07, 0x05), true, false, "I swore to find a cure\nfor the Prince's curse.\nIf only I could find\nthat elusive Astos.."),
				new TargetNpc(ObjectId.Astos, MapIndex.NorthwestCastle, (0x11,0x07), true, true, "While the Crown is\nmissing, I can attest\nthat this is indeed\nthe REAL King of\nNorthwest Castle."),
				new TargetNpc(ObjectId.Unne, MapIndex.Melmond, (0x1D, 0x02), false, true, "I'm also trying\nto discover the secret\nof Lefeinish!"),
				new TargetNpc(ObjectId.Unne, MapIndex.Lefein, (0,0), false, false, "Lu..pa..?\nLu..pa..?"),
				new TargetNpc(ObjectId.Vampire, MapIndex.SardasCave, (0x04, 0x02), true, true, "The Vampire put a curse\non me, it will only be\nlifted on his death.\nAnd I can't do anything\nwhile you fight.\nIt's a shame."),
				new TargetNpc(ObjectId.CanoeSage, MapIndex.CrescentLake, (0,0), false, true, "I came here to learn\neverything about the\nFiend of Earth. You got\nto respect such a\ndangerous adversary."),
				new TargetNpc(ObjectId.Fairy, MapIndex.Gaia, (0x2F, 0x14), false, true, "I'm trying to get\nwhat's at the bottom\nof the pond.\n\nMaybe if I drained it.."),
				new TargetNpc(ObjectId.Smith, MapIndex.DwarfCave, (0x08, 0x02), true, false, "I'm sure it will be a\nbadass sword! Like with\na huge blade, and a gun\nas the hilt, and you can\ntrigger it..\nI can't wait!"),
				new TargetNpc(ObjectId.Nerrick, MapIndex.DwarfCave, (0x0F, 0x2D), false, true, "Digging a canal is hard\nbut honest work.\n\n..Can't wait to be done\nwith it."),
			};

			var eventNpc = new List<(ObjectId, MapIndex)> { (ObjectId.ElflandCastleElf3, MapIndex.ElflandCastle), (ObjectId.MelmondMan1, MapIndex.Melmond), (ObjectId.MelmondMan3, MapIndex.Melmond), (ObjectId.MelmondMan4, MapIndex.Melmond), (ObjectId.MelmondMan8, MapIndex.Melmond), (ObjectId.DwarfcaveDwarf6, MapIndex.DwarfCave), (ObjectId.ConeriaCastle1FWoman2, MapIndex.ConeriaCastle1F), (ObjectId.ElflandElf2, MapIndex.Elfland), (ObjectId.ElflandElf5, MapIndex.Elfland) };
			var classSprite = new List<byte> { 0xEE, 0xEF, 0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9 };

			var readyString = new List<string> { "Well, that's that.\nLet's go.", "Onward to new\nadventures!", "I knew you'd come back\nfor me!", "......", "I'm the leader now,\nright?", "The Reaper is always\njust a step behind me..", "O.. Okay.. I hope it's\nnot too scary out there.", "Yes!\nI made it on the team!", "A bold choice, let's\nsee if it pays off.", "Alright, let's do this!", "I obey, master.", "They say I'm the best.", "I see, we have the same\ngoal. Let's join forces.", "My.. name? Huh..", "Just don't put me first\nagainst Kraken.", "I'm taking care of the\nGPs from now on!", "It's Saturday night.\nI've got no date, a\nbottle of Shasta, and\nmy all Rush mixtape.\nLet's rock.", "Life insurance?\nNo, I don't have any.\nWhy?", "Let's put an end to\nthis madness.", "Finally, some action!", "You convinced me. I will\njoin your noble cause.", "Evil never rests. I will\nfight by your side.", "Edward wants to join\nthe party." };

			var classSelection = new List<(bool, FF1Class, FF1Class)> {
				((bool)flags.TAVERN1, FF1Class.Fighter, FF1Class.Knight ),
				((bool)flags.TAVERN2, FF1Class.Thief, FF1Class.Ninja ),
				((bool)flags.TAVERN3, FF1Class.BlackBelt, FF1Class.Master ),
				((bool)flags.TAVERN4, FF1Class.RedMage, FF1Class.RedWiz ),
				((bool)flags.TAVERN5, FF1Class.WhiteMage, FF1Class.WhiteWiz ),
				((bool)flags.TAVERN6, FF1Class.BlackMage, FF1Class.BlackWiz ),
			};

			var baseClassList = classSelection.Where(x => x.Item1 == true).Select(x => x.Item2).ToList();
			if (!baseClassList.Any())
			{
				baseClassList = classSelection.Select(x => x.Item2).ToList();
			}
			var promoClassList = classSelection.Where(x => x.Item1 == true).Select(x => x.Item3).ToList();
			if (!promoClassList.Any())
			{
				promoClassList = classSelection.Select(x => x.Item3).ToList();
			}
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
			PutInBank(newTalkRoutinesBank, 0x9F50, Blob.FromHex("A91148A9FE48A90648A9C748A98248A9004C03FEA0188610A9618511B110297F9110C8C020D0F5A000A9638511A9009110C8C030D0F960A91148A9FE48A90648A9B548A9FF488A4A4A4A4A4A4A8510A91B4C03FEA91148A9FE48A90648A99948A91048A9008565A90E4C03FE"));

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
				if (i < selectList.Count && !(bool)flags.ClassAsNpcDuplicate)
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
					MapIndex originMap = MapIndex.Cardia;
					int targetIndex = 16;
					(int, int) targetCoord = (0, 0);
					bool targetInRoom = false;
					bool targetStationary = true;

					// Bikke, Lefein and CanoeSage use local NPCs
					if (npc.linkedNPC == ObjectId.Bikke)
					{
						targetNpc = ObjectId.PravokaMan2;
						originMap = MapIndex.Pravoka;
						var tempNpc = FindNpc(originMap, targetNpc);
						var bikkeNpc = FindNpc(originMap, ObjectId.Bikke);
						targetIndex = tempNpc.Index;
						targetCoord = (bikkeNpc.Coord.x - 1, bikkeNpc.Coord.y - 1);
						targetInRoom = tempNpc.InRoom;
						targetStationary = true;
					}
					else if (npc.linkedNPC == ObjectId.Unne && npc.targetMap == MapIndex.Lefein)
					{
						targetNpc = ObjectId.LefeinMan11;
						originMap = MapIndex.Lefein;
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
						originMap = MapIndex.CrescentLake;
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

					newDialogs.Add(npcdata.GetTalkArray(targetNpc)[1], readyString.SpliceRandom(rng) + "\n\n" + ClassNames[(int)classList.First()] + " joined.");
					newDialogs.Add(npcdata.GetTalkArray(targetNpc)[2], npc.newDialogue);

					classList.RemoveRange(0, 1);
				}
			}

			if ((bool)flags.ClassAsNpcFiends)
			{
				// Check if maps are flipped
				bool earthB5flipped = flippedmaps.Contains(MapIndex.EarthCaveB5);
				bool volcanoB5flipped = flippedmaps.Contains(MapIndex.GurguVolcanoB5);
				bool seaB5flipped = flippedmaps.Contains(MapIndex.SeaShrineB5);

				bool earthB5vflipped = vflippedmaps.Contains(MapIndex.EarthCaveB5);
				bool volcanoB5vflipped = vflippedmaps.Contains(MapIndex.GurguVolcanoB5);
				bool seaB5vflipped = vflippedmaps.Contains(MapIndex.SeaShrineB5);

				var dungeonNpc = new List<ObjectId> { ObjectId.MelmondMan6, ObjectId.GaiaMan4, ObjectId.OnracPunk1, ObjectId.GaiaMan1 };

				SetNpc(MapIndex.Melmond, 8, ObjectId.None, 0x12, 0x18, false, false);
				SetNpc(MapIndex.Gaia, FindNpc(MapIndex.Gaia, ObjectId.GaiaMan4).Index, ObjectId.None, 0x12, 0x18, false, false);
				SetNpc(MapIndex.Onrac, 6, ObjectId.None, 0x12, 0x18, false, false);
				SetNpc(MapIndex.Gaia, 1, ObjectId.None, 0x12, 0x18, false, false);

				var earthX = earthB5flipped ? (0x3F - ((bool)flags.ClassAsNpcForcedFiends ? 0x0C : 0x0D)) : ((bool)flags.ClassAsNpcForcedFiends ? 0x0C : 0x0D);
				var volcanoX = volcanoB5flipped ? (0x3F - ((bool)flags.ClassAsNpcForcedFiends ? 0x07 : 0x05)) : ((bool)flags.ClassAsNpcForcedFiends ? 0x07 : 0x05);
				var seaX = seaB5flipped ? (0x3F - ((bool)flags.ClassAsNpcForcedFiends ? 0x0C : 0x0A)) : ((bool)flags.ClassAsNpcForcedFiends ? 0x0C : 0x0A);

				var earthY = earthB5vflipped ? 0x16 : 0x28;
				var volcanoY = volcanoB5vflipped ? 0x0A : 0x35;
				var seaY = seaB5vflipped ? 0x38  : 0x07;

				SetNpc(MapIndex.EarthCaveB5, 0x0C, ObjectId.MelmondMan6, earthX, earthY, true, true);
				SetNpc(MapIndex.GurguVolcanoB5, 0x02, ObjectId.GaiaMan4, volcanoX, volcanoY, true, true);
				SetNpc(MapIndex.SeaShrineB5, 0x01, ObjectId.OnracPunk1, seaX, seaY, true, true);
				SetNpc(MapIndex.SkyPalace5F, 0x02, ObjectId.GaiaMan1, (bool)flags.ClassAsNpcForcedFiends ? 0x07 : 0x09, 0x03, true, true);

				// Restore the default color if Required WarMech is enabled so Tiamat's NPC don't look too weird
				Data[0x029AB] = 0x30;

				for (int i = 0; i < 4; i++)
				{
					newDialogs.Add(npcdata.GetTalkArray(dungeonNpc[i])[1], readyString.SpliceRandom(rng) + "\n\n" + ClassNames[(int)classList[i]] + " joined.");
					npcdata.GetTalkArray(dungeonNpc[i])[0] = 0x00;
					npcdata.GetTalkArray(dungeonNpc[i])[3] = (byte)(classList[i]);
					npcdata.SetRoutine(dungeonNpc[i], (newTalkRoutines)talk_class);
					Data[MapObjGfxOffset + (byte)dungeonNpc[i]] = classSprite[(int)classList[i]];
				}
			}

			InsertDialogs(newDialogs);
		}
	}
}
