using System.ComponentModel;
using FF1Lib.Helpers;

namespace FF1Lib
{
	public enum Rank
	{
		S = 6,
		A = 5,
		B = 4,
		C = 3,
		D = 2,
		E = 1,
		F = 0
	}
	public enum RankedType
	{
		Swords = 0,
		Hammers = 1,
		Knives = 2,
		Axes = 3,
		Staves = 4,
		Nunchucks = 5,

		Armors = 6,
		Shields = 7,
		Helmets = 8,
		Gauntlets = 9,

		HP = 10,
		Strength = 11,
		Agility = 12,
		Intellect = 13,
		Vitality = 14,
		Luck = 15,
		Hits = 16,
		MDef = 17,

		White = 18,
		Black = 19,
		Charges = 20,
	}
	public enum BlursesStartWithGold
	{
		Remove = -1,
		None = 0,
		Normal = 1,
		Thief = 5,
	}

	public enum MpGainOnMaxGain
	{
		[Description("None")]
		None = 0,
		[Description("Promoted Classes Only")]
		Promoted,
		[Description("All Classes")]
		All,
		[Description("Blessed Classes Only")]
		Blursed
	}

	public class GameClasses
	{
		private List<ClassData> _classes;
		private GearPermissions _weaponPermissions;
		private GearPermissions _armorPermissions;
		private SpellPermissions _spellPermissions;

		const int lut_LvlUpHitRateBonus = 0x6CA59;
		const int lut_LvlUpMagDefBonus = 0x6CA65;
		const int lut_InnateResist = 0x6D400;
		const int lut_MaxMP = 0x6C902;
		const int lut_MpGainOnMaxMpGainClasses = 0x6D830;
		const int StartingStatsOffset = 0x3040;
		const int NewLevelUpDataOffset = 0x6CDA9;

		public enum BonusMalusAction
		{
			None = 0,
			StrMod = 1,
			AgiMod = 2,
			IntMod = 3,
			VitMod = 4,
			LckMod = 5,
			HpMod = 6,
			HitMod = 7,
			MDefMod = 8,
			StrGrowth = 9,
			AgiGrowth = 10,
			IntGrowth = 11,
			VitGrowth = 12,
			LckGrowth = 13,
			HpGrowth = 14,
			HitGrowth = 15,
			MDefGrowth = 16,
			SpcMod = 17,
			SpcGrowth = 18,
			WeaponAdd = 19,
			WeaponRemove = 20,
			WeaponReplace = 21,
			ArmorAdd = 22,
			ArmorRemove = 23,
			ArmorReplace = 24,
			WhiteSpellcaster = 25,
			BlackSpellcaster = 27,
			SpcMax = 29,
			PowerRW = 30,
			NoPromoMagic = 31,
			LockpickingLevel = 32,
			InnateResist = 33,
			BonusXp = 34,
			MpGainOnMaxMpGain = 35,
			StartWithSpell,
			CantLearnSpell,
			StartWithGold,
			StartWithMp,
			UnarmedAttack,
			CatClawMaster,
			ThorMaster,
			SteelLord,
			WoodAdept,
			Hunter,
			Sleepy,
			Sick,
			StartWithKI,
			InnateSpells
		}

		public class BonusMalus
		{
			public List<Item> Equipment { get; set; }
			public List<SpellSlots> SpellList { get; set; }
			public List<bool> StatGrowth { get; set; }
			public int StatMod { get; set; }
			public int StatMod2 { get; set; }
			public RankedType TargetStat { get; set; }
			public BonusMalusAction Action { get; set; }
			public string Description { get; set; }
			public List<byte> SpcGrowth { get; set; }
			public List<Classes> ClassList { get; set; }
			public SpellSlotInfo SpellSlotMod { get; set; }
			public List<SpellSlotInfo> SpellsMod { get; set; }
			public BonusMalus(BonusMalusAction action, string description, int mod = 0, int mod2 = 0, List<Item> equipment = null, List<bool> binarylist = null, List<SpellSlots> spelllist = null, List<byte> bytelist = null, SpellSlotInfo spellslotmod = null, List<SpellSlotInfo> spellsmod = null, List<Classes> Classes = null)
			{
				Action = action;
				Description = description;
				StatMod = mod;
				StatMod2 = mod2;
				Equipment = equipment;
				SpellList = spelllist;
				StatGrowth = binarylist;
				SpellSlotMod = spellslotmod;
				SpellsMod = spellsmod;
				if (bytelist == null)
					SpcGrowth = Enumerable.Repeat((byte)0x00, 49).ToList();
				else
					SpcGrowth = bytelist;
				if (Classes == null)
					ClassList = Enum.GetValues<Classes>().ToList();
				else
					ClassList = Classes;
			}
		}

		public GameClasses(GearPermissions weapPerm, GearPermissions armorPerm, SpellPermissions spellPerm, FF1Rom rom)
		{
			_classes = new List<ClassData>();

			_weaponPermissions = weapPerm;
			_armorPermissions = armorPerm;
			_spellPermissions = spellPerm;

			// Addresses
			var startingStats = rom.Get(StartingStatsOffset, 0x60).Chunk(0x10);
			var levelUpStats = rom.Get(NewLevelUpDataOffset, 588).Chunk(49 * 2);
			var hitGrowth = rom.Get(lut_LvlUpHitRateBonus, 12).ToBytes().ToList();
			var mdefGrowthBase = rom.Get(lut_LvlUpMagDefBonus, 6).ToBytes().ToList();
			var mdefGrowthPromo = rom.Get(lut_LvlUpMagDefBonus + 6, 6).ToBytes().ToList();
			var maxChargeList = new byte[] { 0x00, 0x00, 0x00, 0x09, 0x09, 0x09, 0x04, 0x04, 0x00, 0x09, 0x09, 0x09 };

			// Populate stats
			for (int i = 0; i < 6; i++)
			{
				_classes.Add(new ClassData((byte)i, startingStats[i], levelUpStats[i], hitGrowth[i], mdefGrowthBase[i], maxChargeList[i], _weaponPermissions, _armorPermissions, _spellPermissions));
			}

			for (int i = 0; i < 6; i++)
			{
				_classes.Add(new ClassData((byte)(i + 6), startingStats[i], levelUpStats[i], hitGrowth[i + 6], mdefGrowthPromo[i], maxChargeList[i + 6], _weaponPermissions, _armorPermissions, _spellPermissions));
			}
		}

		public void Write(FF1Rom rom)
		{

			foreach (var classdata in _classes)
			{
				classdata.RecalculateAttackEvasion();
			}

			// Insert starting stats
			rom.Put(0x3040, _classes.GetRange(0, 6).SelectMany(x => x.StartingStatsArray()).ToArray());

			// Insert level up data
			rom.Put(NewLevelUpDataOffset, _classes.GetRange(0, 6).SelectMany(x => x.LevelUpArray()).ToArray());

			// Insert hit% and mdef growth rate
			rom.Put(lut_LvlUpHitRateBonus, _classes.Select(x => x.HitGrowth).ToArray());
			rom.Put(lut_LvlUpMagDefBonus, _classes.Select(x => x.MDefGrowth).ToArray());

			// Insert max spell charges array
			rom.Put(lut_MaxMP, _classes.Select(x => x.MaxSpC).ToArray());

			// Insert Innate Resists
			rom.Put(lut_InnateResist, _classes.Select(x => x.InnateResist).ToArray());
		}
		public ClassData this[Classes index]
		{
			get
			{
				return _classes[(int)index];
			}

			set
			{
				_classes[(int)index] = value;
			}
		}
		public void RaiseThiefHitRate(Flags flags)
		{
			if (!(bool)flags.ThiefHitRate)
			{
				return;
			}

			_classes[(int)Classes.Thief].HitGrowth = 4;
			_classes[(int)Classes.Ninja].HitGrowth = 4;
		}
		public void BuffThiefAGI(Flags flags)
		{
			if (flags.ThiefAgilityBuff == ThiefAGI.Vanilla)
			{
				return;
			}

			// Increase thief starting agility, agility
			// growth, and starting evade to make it more
			// viable as a first-slot character.
			// See git commit message for details.

			switch (flags.ThiefAgilityBuff)
			{
				case ThiefAGI.Agi30:
					_classes[(int)Classes.Thief].AgiStarting = 30;
					break;
				case ThiefAGI.Agi50:
					_classes[(int)Classes.Thief].AgiStarting = 50;
					break;
				case ThiefAGI.Agi80:
					_classes[(int)Classes.Thief].AgiStarting = 80;
					break;
				case ThiefAGI.Agi100:
					_classes[(int)Classes.Thief].AgiStarting = 100;
					break;
				case ThiefAGI.Agi120:
					_classes[(int)Classes.Thief].AgiStarting = 120;
					break;
				default:
					break;
			}

			_classes[(int)Classes.Thief].AgiGrowth = Enumerable.Repeat(true, 49).ToList();
			_classes[(int)Classes.Thief].EvaStarting = (byte)Math.Min(_classes[(int)Classes.Thief].AgiStarting + 48, 255);
		}

		public void EarlierHighTierMagicCharges(Flags flags)
		{
			if (!(bool)flags.EarlierHighTierMagic)
			{
				return;
			}
													  // Right-most bits are Tier-1 Charges, Left-most are Tier-8; each byte is a Level Up (Now maxes at L45 for WM/BM)
			var EarlierHighTierMP_WMBMByteList = new List<byte> { 0b00000011, 0b00000010, 0b00000001, 0b00000110, 0b00000100, 0b00000001, 0b00001100, 0b00001010, 0b00000001,
													  0b00011100, 0b00000010, 0b00010000, 0b00101001, 0b00010100, 0b00100000, 0b01010010, 0b00101000, 0b01000001, 0b10100100,
													  0b00010000, 0b11000000, 0b00101000, 0b10000011, 0b01000000, 0b00010100, 0b10000010, 0b01101000, 0b00000100, 0b10010000,
													  0b01000010, 0b00100000, 0b10001000, 0b01000000, 0b00000100, 0b00010000, 0b10000000, 0b00100000, 0b01000000, 0b00001000,
													  0b10000000, 0b00010000, 0b00100000, 0b01000000, 0b10000000,
													  0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 };

			var EarlierHighTierMP_RMByteList = new List<byte> { 0b00000011, 0b00000010, 0b00000001, 0b00000010, 0b00000100, 0b00000101, 0b00000010, 0b00000100, 0b00001001,
													0b00001000, 0b00000100, 0b00011010, 0b00000001, 0b00110000, 0b00001100, 0b00000010, 0b00110000, 0b01001000, 0b00010100,
													0b00100001, 0b11000001, 0b00000010, 0b10111000, 0b01000000, 0b00000100, 0b10100000, 0b01000010, 0b00010000, 0b00001000,
													0b10100000, 0b00000100, 0b01000000, 0b00000010, 0b00010000, 0b10000000, 0b01001000, 0b00100000, 0b00000100, 0b10010000,
													0b01000000, 0b00001000, 0b00100000, 0b10000000, 0b00010000, 0b01000000, 0b10000000, 0b00100000, 0b01000000, 0b10000000 };

			// Note that capitalization matters - "SpcGrowth" is different
			_classes[(int)Classes.RedMage].SpCGrowth = EarlierHighTierMP_RMByteList;
			_classes[(int)Classes.RedWizard].SpCGrowth = EarlierHighTierMP_RMByteList;
			_classes[(int)Classes.WhiteMage].SpCGrowth = EarlierHighTierMP_WMBMByteList;
			_classes[(int)Classes.WhiteWizard].SpCGrowth = EarlierHighTierMP_WMBMByteList;
			_classes[(int)Classes.BlackMage].SpCGrowth = EarlierHighTierMP_WMBMByteList;
			_classes[(int)Classes.BlackWizard].SpCGrowth = EarlierHighTierMP_WMBMByteList;
		}

		public void SetMPMax(Flags flags)
		{
			if (!(bool)flags.ChangeMaxMP)
			{
				return;
			}

			_classes[(int)Classes.RedMage].MaxSpC = (byte)flags.RedMageMaxMP;
			_classes[(int)Classes.RedWizard].MaxSpC = (byte)flags.RedMageMaxMP;
			_classes[(int)Classes.WhiteMage].MaxSpC = (byte)flags.WhiteMageMaxMP;
			_classes[(int)Classes.WhiteWizard].MaxSpC = (byte)flags.WhiteMageMaxMP;
			_classes[(int)Classes.BlackMage].MaxSpC = (byte)flags.BlackMageMaxMP;
			_classes[(int)Classes.BlackWizard].MaxSpC = (byte)flags.BlackMageMaxMP;
			_classes[(int)Classes.Knight].MaxSpC = (byte)flags.KnightMaxMP;
			_classes[(int)Classes.Ninja].MaxSpC = (byte)flags.NinjaMaxMP;
		}

		public void SetMpGainOnMaxGain(Flags flags, FF1Rom rom)
		{
			if(flags.MpGainOnMaxGainMode != MpGainOnMaxGain.None)
			{
				//jump from old mp up routine to new one at 1B:983C
				rom.PutInBank(0x1B, 0x88D7, Blob.FromHex("4C3C98"));

				string classMpUpTable = "000000000000000000000000";
				if (flags.MpGainOnMaxGainMode == MpGainOnMaxGain.Promoted)
				{
					classMpUpTable = "000000000000010101010101";
				} else if(flags.MpGainOnMaxGainMode == MpGainOnMaxGain.All)
				{
					classMpUpTable = "010101010101010101010101";
				}

				//blursed will get handled by blursed classes.
				rom.PutInBank(0x1B, 0x9830, Blob.FromHex(classMpUpTable+ "AE8E68A001B182A02848B184DD02899005684A4C7398684A901D48B1841869019184BD3098F00F984838E908A8B184186901918468A868C8C030D0CD4C1C89"));
			}
		}

		public void Randomize(Flags flags, MT19337 rng, List<string> olditemnames, ItemNames itemnames, FF1Rom rom)
		{
			if (!(bool)flags.RandomizeClass && !(bool)flags.Transmooglifier && !(bool)flags.RandomizeClassChaos)
				return;

			RandomizeClassHacks(flags, rom);

			if ((bool)flags.Transmooglifier)
				Transmooglify(flags, rng, rom);
			else if ((bool)flags.RandomizeClass)
				RandomizeClassBlursings(flags, rng, olditemnames, itemnames, rom);
			else if ((bool)flags.RandomizeClassChaos)
				RandomizeClassChaos(flags, rng, rom);
		}

		public void Transmooglify(Flags flags, MT19337 rng, FF1Rom rom)
		{
			// The MEAT
			Transmooglifier transmooglifier = new Transmooglifier();
			transmooglifier.Transmooglify(flags, rng, rom);

			// Description screen
			List<string> dataScreen = new List<string>();

			dataScreen.AddRange(transmooglifier.classDescriptions);
			dataScreen.AddRange(transmooglifier.classDescriptions); // Add again for Promo Classes

			CreateDataScreens("", dataScreen, rom);
		}

		public void RandomizeClassChaos(Flags flags, MT19337 rng, FF1Rom rom)
		{
			// Strings to build info screen in game
			List<string> rankString = new List<string> { "-", "E", "D", "C", "B", "A", "S" };
			List<string> symboleString = new List<string> { "@S", "@H", "@K", "@X", "@F", "@N", "@A", "@s", "@h", "@G", "HP", "Str", "Agi", "Int", "Vit", "Lck", "Ht%", "MDf", "Wt", "Bk", "Sp" };

			// The MEAT
			DoRandomizeClassChaosMode(((bool)flags.MagicLevelsMixed && (bool)flags.MagicPermissions) || ((bool)flags.SpellcrafterMixSpells && !(bool)flags.SpellcrafterRetainPermissions), (flags.ThiefAgilityBuff != ThiefAGI.Vanilla), rng, rom);

			// Description screen
			var templateScreen =
				"STATS".PadRight(11) + "\n" +
				"S A I V L".PadRight(11) + "\n" +
				"? ? ? ? ?".PadRight(11) + "\n\n" +
				"Ht% MDf HP".PadRight(11) + "\n" +
				" ?   ?   ?".PadRight(11) + "\n\n" +
				"MAGIC".PadRight(11) + "\n" +
				"Wht Blk SpC".PadRight(11) + "\n" +
				" ?   ?   ?".PadRight(11) + "\n\n" +
				"WEAPONS".PadRight(11) + "\n" +
				"@S @H @K @X @F @N".PadRight(11) + "\n" +
				"? ? ? ? ? ?".PadRight(11) + "\n\n" +
				"ARMORS".PadRight(11) + "\n" +
				"@A @s @h @G".PadRight(11) + "\n" +
				"? ? ? ?".PadRight(11) + "\n\n" +
				"PROMOTION".PadRight(11);

			List<string> dataScreen = new List<string>();
			for (int i = 0; i < 12; i++)
			{
				// Generate promo change data
				string promoChange = "";
				if (i < 6)
				{
					for (int j = 0; j < Enum.GetNames(typeof(RankedType)).Length - 1; j++)
					{
						if (_classes[i + 6].Ranks[j] > _classes[i].Ranks[j])
						{
							if (j == (int)RankedType.White)
							{
								promoChange += _classes[i + 6].MagicRanks[0] + "W";
							}
							else if (j == (int)RankedType.Black)
							{
								promoChange += _classes[i + 6].MagicRanks[1] + "B";
							}
							else
								promoChange += symboleString[j] + rankString[(int)_classes[i + 6].Ranks[j]];

							if (promoChange.Split('\n').Last().Length > (11 - 4))
								promoChange += "\n";
							else
								promoChange += " ";
						}
					}
				}

				// Generate data screen
				var dataChaosScreen =
					rankString[(int)_classes[i].Ranks[(int)RankedType.Strength]] + " " +
					rankString[(int)_classes[i].Ranks[(int)RankedType.Agility]] + " " +
					rankString[(int)_classes[i].Ranks[(int)RankedType.Intellect]] + " " +
					rankString[(int)_classes[i].Ranks[(int)RankedType.Vitality]] + " " +
					rankString[(int)_classes[i].Ranks[(int)RankedType.Luck]] +
					"\n\n\n" +
					" " + _classes[i].HitGrowth + "   " +
					_classes[i].MDefGrowth + "   " +
					rankString[(int)_classes[i].Ranks[(int)RankedType.HP]] +
					"\n\n\n\n" +
					" " + _classes[i].MagicRanks[0] + "  " +
					_classes[i].MagicRanks[1] + "  " +
					rankString[(int)_classes[i].Ranks[(int)RankedType.Charges]] +
					//rankString[(int)classData[i].Ranks[(int)RankedType.Black]] + "   " +
					//rankString[(int)classData[i].Ranks[(int)RankedType.Charges]] +
					"\n\n\n\n" +
					rankString[(int)_classes[i].Ranks[(int)RankedType.Swords]] + " " +
					rankString[(int)_classes[i].Ranks[(int)RankedType.Hammers]] + " " +
					rankString[(int)_classes[i].Ranks[(int)RankedType.Knives]] + " " +
					rankString[(int)_classes[i].Ranks[(int)RankedType.Axes]] + " " +
					rankString[(int)_classes[i].Ranks[(int)RankedType.Staves]] + " " +
					rankString[(int)_classes[i].Ranks[(int)RankedType.Nunchucks]] +
					"\n\n\n\n" +
					rankString[(int)_classes[i].Ranks[(int)RankedType.Armors]] + " " +
					rankString[(int)_classes[i].Ranks[(int)RankedType.Shields]] + " " +
					rankString[(int)_classes[i].Ranks[(int)RankedType.Helmets]] + " " +
					rankString[(int)_classes[i].Ranks[(int)RankedType.Gauntlets]] +
					"\n\n\n" +
					String.Join("", promoChange);

				dataScreen.Add(dataChaosScreen);
			}

			CreateDataScreens(templateScreen, dataScreen, rom);
		}

		public void RandomizeClassBlursings(Flags flags, MT19337 rng, List<string> olditemnames, ItemNames itemnames, FF1Rom rom)
		{
			// The MEAT
			List<string> bonusmalusDescription = new List<string>();
			bonusmalusDescription = DoRandomizeClassNormalMode(rng, olditemnames, itemnames, flags, rom);

			// Description screen
			var templateScreen = "BONUS";

			List<string> dataScreen = new List<string>();
			dataScreen.AddRange(bonusmalusDescription.Concat(bonusmalusDescription));

			CreateDataScreens(templateScreen, dataScreen, rom);
		}

		public void RandomizeClassHacks(Flags flags, FF1Rom rom)
		{
			// Starting Stats awards MP to allow any class to start with spell charges
			rom.PutInBank(0x1F, 0xC7CA, Blob.FromHex("B94BB09D20639D286360"));
			_classes[(int)Classes.RedMage].SpCStarting = 2;
			_classes[(int)Classes.WhiteMage].SpCStarting = 2;
			_classes[(int)Classes.BlackMage].SpCStarting = 2;
			_classes[(int)Classes.RedWizard].SpCStarting = 2;
			_classes[(int)Classes.WhiteWizard].SpCStarting = 2;
			_classes[(int)Classes.BlackWizard].SpCStarting = 2;

			// Status jump to new menu wait, switch banks, see 0E_A0E0_MenuWaitForBtn_SFX_Status.asm
			rom.PutInBank(0x0E, 0xB50D, Blob.FromHex("4CE0A0"));
			rom.PutInBank(0x0E, 0xA0E0, Blob.FromHex("A98948A90F48A91E85574C03FE"));

			// EnterInfoMenu, see 1E_8800_DrawInfoBox.asm
			//rom.PutInBank(0x1E, 0x8800, Blob.FromHex("203CC4A5674A4A4A4A4AB015A200205B83A220205B83A9118538A90285394C3388A210205B83A230205B83A9038538A9028539A667BD0003C9FFD002A90C0AAA207188A98048A9C0484C1A85A9118538A9028539A667BD0061C9FFD002A90C0AAA207188A9B648A91248A90E85574C03FEA90D853CA91A853DA9008D0120A90085378A482063E0A970853EA989853FA538853AE63AA539853BE63BA91E855785582036DE68AABD5089853EBD5189853FA53B186902853B2036DE205E8560"));
			string InfoMenuHeight = "02";
			if ((bool)flags.Transmooglifier)
			{
				InfoMenuHeight = "00";
			}
			// EnterInfoMenu, see 1E_8800_DrawInfoBox.asm
			rom.PutInBank(0x1E, 0x8800, Blob.FromHex($"203CC4A5674A4A4A4A4AB015A200205B83A220205B83A9118538A90285394C3388A210205B83A230205B83A9038538A9028539A667BD0003C9FFD002A90C0AAA207188A98048A9C0484C1A85A9118538A9028539A667BD0061C9FFD002A90C0AAA207188A9B648A91248A90E85574C03FEA90D853CA91A853DA9008D0120A90085378A482063E0A970853EA989853FA538853AE63AA539853BE63BA91E855785582036DE68AABD5089853EBD5189853FA53B1869{InfoMenuHeight}853BA567482036DE688567205E8560"));

			// StatusWaitForBtn_SFX, see 1E_8800_DrawInfoBox.asm
			rom.PutInBank(0x1E, 0x8910, Blob.FromHex("202C85A5240522D00FA525F0F3A9008525A90E85574C03FEA9008524852585224C4C88"));

			// InfoScreen in PtyGen and Status screen
			// DoPartyGen_OnCharacter change to check for Select button, see 1E_8800_DrawInfoBox.asm
			if (!(bool)flags.EnablePoolParty)
			{
				var partypermissions = rom.Get(0x78110, 0x11);
				rom.PutInBank(0x1E, 0x80C1, Blob.FromHex("A6678A4A4A4A4AA8B9B085859020A480A9008522200F82A522F0034C0088A524D049A525F0023860A520290FC561F0E08561C900F0DAA667BD0003186901C90CD002A9FF9D0003A8C8B9B4852490F0E8A901853720B0824CD180"));
				rom.PutInBank(0x1E, 0x85B0, partypermissions);
			}
		}

		public void CreateDataScreens(string templateScreen, List<string> dataScreen, FF1Rom rom)
		{
			// dataScreen
			int totalByte = 0;
			var screenBlob = Blob.FromHex("00");

			// Insert template
			screenBlob = FF1Text.TextToBytes(templateScreen, true, FF1Text.Delimiter.Null, true);
			rom.PutInBank(0x1E, 0x8970, screenBlob);
			totalByte += screenBlob.Length;

			// Insert class data screen
			for (int i = 0; i < 12; i++)
			{
				var tempBlob = FF1Text.TextToBytes(dataScreen[i], true, FF1Text.Delimiter.Null, true);
				rom.PutInBank(0x1E, 0x8970 + totalByte, tempBlob);
				var tempAddress = 0x8970 + totalByte;
				rom.PutInBank(0x1E, 0x8950 + (i * 2), new byte[] { (byte)(tempAddress % 0x100), (byte)(tempAddress / 0x100) });
				totalByte += tempBlob.Length;
			}

			// Insert null data screen for None
			rom.PutInBank(0x1E, 0x8970 + totalByte, Blob.FromHex("00"));
			var noneAddress = 0x8970 + totalByte;
			rom.PutInBank(0x1E, 0x8950 + 24, new byte[] { (byte)(noneAddress % 0x100), (byte)(noneAddress / 0x100) });
		}
					
		public List<string> DoRandomizeClassNormalMode(MT19337 rng, List<string> olditemnames, ItemNames itemnames, Flags flags, FF1Rom rom)
		{
			// Equipment lists
			List<Item> braceletList = new();
			List<Item> ringList = new();
			for (int i = (int)Item.Cloth; i < (int)Item.ProRing; i++)
			{
				if (itemnames[i].Contains("@B"))
				{
					braceletList.Add((Item)i);
				}
			}

			List<Item> bannableArmor = new List<Item> { Item.Ribbon };
			bannableArmor.AddRange(braceletList);
			if (!(bool)flags.ArmorCrafter)
			{
				bannableArmor.Add(Item.ProRing);
			}

			List<Item> equipFighterArmor = _armorPermissions[Classes.Fighter].ToList().Where(x => !bannableArmor.Contains(x)).ToList();
			List<Item> equipRedMageArmor = _armorPermissions[Classes.RedMage].ToList().Where(x => !bannableArmor.Contains(x)).ToList();

			List<Item> equipFighterArmorFull = _armorPermissions[Classes.Fighter].ToList();
			List<Item> equipRedWizardArmorFull = _armorPermissions[Classes.RedWizard].ToList();

			List<Item> equipFighterWeapon = _weaponPermissions[Classes.Fighter].ToList();
			List<Item> equipThiefWeapon = _weaponPermissions[Classes.Thief].ToList();

			List<Item> equipAxes = new();
			for (int i = (int)Item.WoodenNunchucks; i <= (int)Item.Masamune; i++)
			{
				if (itemnames[i].Contains("@X"))
				{
					equipAxes.Add((Item)i);
				}
			}

			List<Item> equipShirts = new();
			for (int i = (int)Item.Cloth; i < (int)Item.ProRing; i++)
			{
				if (itemnames[i].Contains("@T"))
				{
					equipShirts.Add((Item)i);
				}
			}
			List<Item> equipShields = new();
			for (int i = (int)Item.Cloth; i < (int)Item.ProRing; i++)
			{
				if (itemnames[i].Contains("@s"))
				{
					equipShields.Add((Item)i);
				}
			}
			List<Item> equipGauntletsHelmets = new();
			for (int i = (int)Item.Cloth; i < (int)Item.ProRing; i++)
			{
				if (itemnames[i].Contains("@G"))
				{
					equipGauntletsHelmets.Add((Item)i);
				}
				else if (itemnames[i].Contains("@h"))
				{
					equipGauntletsHelmets.Add((Item)i);
				}
			}


			List<Item> equipLegendaryWeapons = new() { Item.Vorpal, Item.Katana, Item.Xcalber };

			// Create exceptions for hit bonus
			var hitBonusClass = new List<Classes>();

			for (int i = 0; i < 6; i++)
			{
				if (_classes[i].HitGrowth < 4)
					hitBonusClass.Add((Classes)i);
			}

			// Spells lists
			var nullSpells = Enumerable.Repeat(false, 4 * 8).ToList();

			var lv1WhiteSpells = _spellPermissions[Classes.WhiteMage].OrderBy(x => x).ToList().GetRange(0, 4).ToList();

			var lv1BlackSpells = _spellPermissions[Classes.BlackMage].OrderBy(x => x).ToList().GetRange(0, 4).ToList();

			var lv3WhiteSpells = _spellPermissions[Classes.Knight].ToList();
			var lv4BlackSpells = _spellPermissions[Classes.Ninja].ToList();

			var wmWhiteSpells = _spellPermissions[Classes.WhiteMage].ToList();
			var bmBlackSpells = _spellPermissions[Classes.BlackMage].ToList();

			var wwWhiteSpells = _spellPermissions[Classes.WhiteWizard].ToList();
			var bwBlackSpells = _spellPermissions[Classes.BlackWizard].ToList();

			// MP Growth Lists
			var rmMPlist = new List<byte>(_classes[(int)Classes.RedMage].SpCGrowth);

			var improvedMPlist = new List<byte> { 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF,
				0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00 };
			var exKnightMPlist = new List<byte> { 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07,
				0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00 };
			var exNinjaMPlist = new List<byte> { 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F,
				0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00 };

			// Normal Bonuses List
			var bonusNormal = new List<BonusMalus> {
				new BonusMalus(BonusMalusAction.StrMod, "+10 Str.", mod: 10),
				new BonusMalus(BonusMalusAction.StrMod, "+20 Str.", mod: 20),
				new BonusMalus(BonusMalusAction.AgiMod, "+15 Agi.", mod: 15),
				new BonusMalus(BonusMalusAction.AgiMod, "+25 Agi.", mod: 25),
				new BonusMalus(BonusMalusAction.VitMod, "+10 Vit.", mod: 10),
				new BonusMalus(BonusMalusAction.VitMod, "+20 Vit.", mod: 20),
				new BonusMalus(BonusMalusAction.LckMod, "+5 Luck", mod: 5),
				new BonusMalus(BonusMalusAction.LckMod, "+10 Luck", mod: 10),
				new BonusMalus(BonusMalusAction.HpMod, "+20 HP", mod: 20),
				new BonusMalus(BonusMalusAction.HpMod, "+40 HP", mod: 40),
				new BonusMalus(BonusMalusAction.HitMod, "+10 Hit%", mod: 10, Classes: hitBonusClass ),
				new BonusMalus(BonusMalusAction.HitMod, "+20 Hit%", mod: 20, Classes: hitBonusClass ),
				new BonusMalus(BonusMalusAction.MDefMod, "+10 MDef", mod: 10),
				new BonusMalus(BonusMalusAction.MDefMod, "+20 MDef", mod: 20),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+Equip @X", equipment: equipAxes, Classes: new List<Classes> { Classes.Thief, Classes.BlackBelt, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage } ),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+Legendary@S", equipment: equipLegendaryWeapons),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Equip @T", equipment: equipShirts),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Equip @s", equipment: equipShields, Classes: new List<Classes> { Classes.Thief, Classes.BlackBelt, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage } ),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Equip @G+@h", equipment: equipGauntletsHelmets, Classes: new List<Classes> { Classes.Thief, Classes.BlackBelt, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage } ),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+Thief @S", equipment: equipThiefWeapon, Classes: new List<Classes> { Classes.BlackBelt, Classes.WhiteMage, Classes.BlackMage } ),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Red Mage @A", equipment: equipRedMageArmor, Classes: new List<Classes> { Classes.Thief, Classes.BlackBelt, Classes.WhiteMage, Classes.BlackMage } ),
				new BonusMalus(BonusMalusAction.SpcMod, "+2 Lv1 MP", mod: 2, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }),
				new BonusMalus(BonusMalusAction.StartWithMp, "+1 MP LvAll", Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }),
				new BonusMalus(BonusMalusAction.ThorMaster, "Improved\n Thor@H", equipment: new List<Item>() { Item.ThorHammer }, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.WhiteMage }),
				new BonusMalus(BonusMalusAction.Hunter, "Hurt Undead", mod: 0x18),
				new BonusMalus(BonusMalusAction.Hunter, "Hurt Dragon", mod: 0x02),
			};

			if (!(bool)flags.Weaponizer)
			{
				bonusNormal.Add(new BonusMalus(BonusMalusAction.CatClawMaster, "Improved\n CatClaw", equipment: new List<Item>() { Item.CatClaw }, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.RedMage, Classes.BlackMage }));
			}

			// Strong Bonuses List
			var bonusStrong = new List<BonusMalus> {
				new BonusMalus(BonusMalusAction.StrMod, "+40 Str.", mod: 40),
				new BonusMalus(BonusMalusAction.AgiMod, "+50 Agi.", mod: 50),
				new BonusMalus(BonusMalusAction.VitMod, "+40 Vit.", mod: 40),
				new BonusMalus(BonusMalusAction.LckMod, "+15 Luck", mod: 15, Classes: new List<Classes> { Classes.Fighter, Classes.BlackBelt, Classes.WhiteMage, Classes.RedMage, Classes.BlackMage } ),
				new BonusMalus(BonusMalusAction.HpMod, "+80 HP", mod: 80),
				new BonusMalus(BonusMalusAction.MDefGrowth, "+2 MDef/Lv", mod: 2, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+Fighter @S", equipment: equipFighterWeapon, Classes: new List<Classes> { Classes.Thief, Classes.BlackBelt, Classes.WhiteMage, Classes.BlackMage } ),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Fighter @A", equipment: equipFighterArmor, Classes: new List<Classes> { Classes.Thief, Classes.BlackBelt, Classes.WhiteMage, Classes.BlackMage, Classes.RedMage } ),
				new BonusMalus(BonusMalusAction.SpcGrowth, "Improved MP", bytelist: improvedMPlist, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage } ),
				new BonusMalus(BonusMalusAction.PowerRW, "Sage", mod: 1, spelllist: wmWhiteSpells.Concat(bmBlackSpells).Concat(wwWhiteSpells).Concat(bwBlackSpells).ToList(), Classes: new List<Classes> { Classes.RedMage }),
				new BonusMalus(BonusMalusAction.Hunter, "Hurt All", mod: 0xFF),
				//new BonusMalus(BonusMalusAction.UnarmedAttack, "Monk", Classes: new List<Classes> { Classes.WhiteMage }), need extra work
			};

			// Maluses List
			var malusNormal = new List<BonusMalus> {
				new BonusMalus(BonusMalusAction.StrMod, "-10 Str.", mod: -10, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt, Classes.WhiteMage, Classes.RedMage } ),
				new BonusMalus(BonusMalusAction.StrMod, "-20 Str.", mod: -20, Classes: new List<Classes> { Classes.Fighter }),
				new BonusMalus(BonusMalusAction.AgiMod, "-10 Agi.", mod: -10),
				new BonusMalus(BonusMalusAction.AgiMod, "-20 Agi.", mod: -20, Classes: new List<Classes> { Classes.Thief }),
				new BonusMalus(BonusMalusAction.VitMod, "-10 Vit.", mod: -10, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt, Classes.WhiteMage, Classes.RedMage } ),
				new BonusMalus(BonusMalusAction.VitMod, "-20 Vit.", mod: -20, Classes: new List<Classes> { Classes.BlackBelt }),
				new BonusMalus(BonusMalusAction.LckMod, "-5 Luck", mod: -5),
				new BonusMalus(BonusMalusAction.LckMod, "-10 Luck", mod: -10, Classes: new List<Classes> { Classes.Thief, Classes.BlackMage }),
				new BonusMalus(BonusMalusAction.HpMod, "-15 HP", mod: -15),
				new BonusMalus(BonusMalusAction.HpMod, "-30 HP", mod: -30),
				new BonusMalus(BonusMalusAction.HpGrowth, "BlackM HP", binarylist: _classes[(int)Classes.BlackMage].HpGrowth, Classes: new List<Classes> { Classes.Fighter }),
				new BonusMalus(BonusMalusAction.HitMod, "-10 Hit%", mod: -10),
				new BonusMalus(BonusMalusAction.MDefMod, "-10 MDef", mod: -10),
				new BonusMalus(BonusMalusAction.HitGrowth, "-1 Hit%/Lv", mod: -1),
				new BonusMalus(BonusMalusAction.MDefGrowth, "-1 MDef/Lv", mod: -1),
				new BonusMalus(BonusMalusAction.ArmorRemove, "No @B", equipment: braceletList),
				new BonusMalus(BonusMalusAction.WeaponReplace, "Thief @S", equipment: equipThiefWeapon, Classes: new List<Classes> { Classes.Fighter, Classes.RedMage } ),
				new BonusMalus(BonusMalusAction.SpcMax, "-4 Max MP", mod: -4, Classes: new List<Classes> {  Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }),
				//new BonusMalus(BonusMalusAction.Sick, "Sick"), these were too powerful for man
				//new BonusMalus(BonusMalusAction.Sleepy, "Sleepy"),
			};

			// Negative amounts are processed separately in ProcessStartWithRoutines, because they affect the Assembly code
			// If changing the Malus gold labels below, change those as well to alter the actual number used
			if (flags.StartingGold == StartingGold.None) {
				bonusNormal.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+200 GP", mod: 2));
				bonusStrong.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+1400 GP", mod: 14, Classes: new List<Classes> { Classes.Thief }));
			}

			else if (flags.StartingGold == StartingGold.Gp100) {
				bonusNormal.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+400 GP", mod: 4));
				bonusStrong.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+2000 GP", mod: 20, Classes: new List<Classes> { Classes.Thief }));
				malusNormal.Add(new BonusMalus(BonusMalusAction.StartWithGold, "-50 GP", mod: -1));
			}
			else if (flags.StartingGold == StartingGold.Gp200) {
				bonusNormal.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+600 GP", mod: 6));
				bonusStrong.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+3000 GP", mod: 30, Classes: new List<Classes> { Classes.Thief }));
				malusNormal.Add(new BonusMalus(BonusMalusAction.StartWithGold, "-100 GP", mod: -1));
			}
			else if (flags.StartingGold == StartingGold.Gp400 || flags.StartingGold == StartingGold.RandomLow) {
				bonusNormal.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+800 GP", mod: 8));
				bonusStrong.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+4000 GP", mod: 40, Classes: new List<Classes> { Classes.Thief }));
				malusNormal.Add(new BonusMalus(BonusMalusAction.StartWithGold, "-150 GP", mod: -1));
			}
			else if (flags.StartingGold == StartingGold.Gp800) {
				bonusNormal.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+1500 GP", mod: 15));
				bonusStrong.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+6000 GP", mod: 60, Classes: new List<Classes> { Classes.Thief }));
				malusNormal.Add(new BonusMalus(BonusMalusAction.StartWithGold, "-350 GP", mod: -1));
			}
			else if (flags.StartingGold == StartingGold.Gp2500) {
				bonusNormal.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+5000 GP", mod: 50));
				bonusStrong.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+20,000 GP", mod: 200, Classes: new List<Classes> { Classes.Thief }));
				malusNormal.Add(new BonusMalus(BonusMalusAction.StartWithGold, "-1100 GP", mod: -1));
			}
			else if (flags.StartingGold == StartingGold.Gp9999) {
				bonusNormal.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+20,000 GP", mod: 200));
				// Strong bonus doesn't make sense with gold already so high, so isn't created
				malusNormal.Add(new BonusMalus(BonusMalusAction.StartWithGold, "-4500 GP", mod: -1));
			}
			// These are so much starting gold that bonuses for it no longer make sense
			//else if (flags.StartingGold == StartingGold.Gp65535 || flags.StartingGold == StartingGold.RandomHigh)

			if (!(bool)flags.NoMasamune)
			{
				malusNormal.Add(new BonusMalus(BonusMalusAction.WeaponRemove, "No " + olditemnames[(int)Item.Masamune], equipment: new List<Item> { Item.Masamune }));
			}

			if (flags.RibbonMode == RibbonMode.Vanilla)
			{
				malusNormal.Add(new BonusMalus(BonusMalusAction.ArmorRemove, "No " + olditemnames[(int)Item.Ribbon], equipment: new List<Item> { Item.Ribbon }));
			}

			// Do not add Promo-based blursings if there is no ability to promote
			if (!((bool)flags.NoTail && !(bool)flags.FightBahamut))
			{
				bonusNormal.Add(new BonusMalus(BonusMalusAction.ArmorAdd, "Promo FI @A", mod: 99, equipment: equipFighterArmor, Classes: new List<Classes> { Classes.BlackBelt, Classes.WhiteMage, Classes.BlackMage, Classes.RedMage }));
				bonusNormal.Add(new BonusMalus(BonusMalusAction.PowerRW, "Promo Sage", mod: 0, spelllist: wmWhiteSpells.Concat(bmBlackSpells).Concat(wwWhiteSpells).Concat(bwBlackSpells).ToList(), Classes: new List<Classes> { Classes.RedMage }));

				malusNormal.Add(new BonusMalus(BonusMalusAction.ArmorReplace, "No Promo @A", mod: 99, equipment: equipFighterArmorFull, Classes: new List<Classes> { Classes.Fighter }));
				malusNormal.Add(new BonusMalus(BonusMalusAction.ArmorReplace, "Promo RW @A", mod: 99, equipment: equipRedWizardArmorFull, Classes: new List<Classes> { Classes.Thief }));
				malusNormal.Add(new BonusMalus(BonusMalusAction.NoPromoMagic, "No Promo Sp", mod: 0, mod2: 0, binarylist: nullSpells, Classes: new List<Classes> { Classes.Fighter, Classes.Thief }));
			}
			

			if (!(bool)flags.ArmorCrafter)
			{
				malusNormal.Add(new BonusMalus(BonusMalusAction.ArmorRemove, "-" + olditemnames[(int)Item.ProRing], equipment: new List<Item> { Item.ProRing }));
				bonusStrong.Add(new BonusMalus(BonusMalusAction.SteelLord, "Steel@A\n Cast Fast", Classes: new List<Classes> { Classes.Fighter }));
				bonusNormal.Add(new BonusMalus(BonusMalusAction.WoodAdept, "Wood@A@s@h Set\n Add Evade"));
			}

			if (Rng.Between(rng, 0, 10) == 0)
			{
				malusNormal.Add(new BonusMalus(BonusMalusAction.IntMod, "+80 Int.", mod: 80));
			}

			// Single Spells Bonus/Malus
			bonusNormal.AddRange(CreateSpellBonuses(rom, rng, flags));
			malusNormal.AddRange(CreateSpellMaluses(rom, rng, flags));

			if ((bool)flags.RandomizeClassCasting)
			{ 
				var magicBonuses = CreateMagicBonuses(rom, rng, flags);

				bonusNormal.AddRange(magicBonuses.Item1);
				bonusStrong.AddRange(magicBonuses.Item2);
			}

			// Add Lockpicking Bonus/Malus
			if ((bool)flags.Lockpicking && flags.LockpickingLevelRequirement < 50)
			{
				malusNormal.Add(new BonusMalus(BonusMalusAction.LockpickingLevel, "LateLockpik", mod: 10, Classes: new List<Classes> { Classes.Thief }));
			}

			if ((bool)flags.Lockpicking && flags.LockpickingLevelRequirement > 1)
			{
				bonusNormal.Add(new BonusMalus(BonusMalusAction.LockpickingLevel, "EarlyLokpik", mod: -10, Classes: new List<Classes> { Classes.Thief }));
			}

			// Add Natural Resist Bonuses
			bonusStrong.Add(new BonusMalus(BonusMalusAction.InnateResist, "Res. All", mod: 0xFF, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.RedMage, Classes.BlackMage, Classes.WhiteMage }));
			bonusStrong.Add(new BonusMalus(BonusMalusAction.InnateResist, "Res. PEDTS", mod: (int)(SpellElement.Poison | SpellElement.Earth | SpellElement.Death | SpellElement.Time | SpellElement.Status), Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.RedMage, Classes.BlackMage, Classes.WhiteMage }));
			bonusNormal.Add(CreateRandomResistBonusMalus(rng));
			bonusNormal.Add(CreateRandomResistBonusMalus(rng));

			// Add XP Bonuses
			if ((bool)flags.RandomizeClassIncludeXpBonus)
			{
				bonusStrong.Add(new BonusMalus(BonusMalusAction.BonusXp, "+50% XP", mod: 150, Classes: new List<Classes> { Classes.Fighter, Classes.BlackBelt }));
				bonusStrong.Add(new BonusMalus(BonusMalusAction.BonusXp, "+100% XP", mod: 200, Classes: new List<Classes> { Classes.Thief, Classes.RedMage, Classes.BlackMage, Classes.WhiteMage }));

				bonusNormal.Add(new BonusMalus(BonusMalusAction.BonusXp, "+50% XP", mod: 150, Classes: new List<Classes> { Classes.Thief, Classes.RedMage, Classes.BlackMage, Classes.WhiteMage }));
				bonusNormal.Add(new BonusMalus(BonusMalusAction.BonusXp, "+50% XP", mod: 150, Classes: new List<Classes> { Classes.Thief, Classes.RedMage, Classes.BlackMage, Classes.WhiteMage }));
			}

			// Add Max MP on MP Gain Bonus
			if (flags.MpGainOnMaxGainMode == MpGainOnMaxGain.Blursed)
			{
				bonusNormal.Add(new BonusMalus(BonusMalusAction.MpGainOnMaxMpGain, "Max+Mp+", Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));
			}

			var assignedBonusMalus = new List<List<BonusMalus>> { new List<BonusMalus>(), new List<BonusMalus>(), new List<BonusMalus>(), new List<BonusMalus>(), new List<BonusMalus>(), new List<BonusMalus>() };

			// Shuffle bonuses and maluses
			bonusStrong.Shuffle(rng);
			bonusNormal.AddRange(bonusStrong.GetRange(0, 3));
			bonusNormal.Shuffle(rng);
			malusNormal.Shuffle(rng);

			var descriptionList = new List<string>();

			// Distribute bonuses and maluses
			int maxbonus = flags.RandomizeClassMaxBonus;
			int maxmalus = flags.RandomizeClassMaxMalus;

			bool validBlursingsDistribution = false;

			var startWithKiBlurses = StartWithKeyItems(flags, rng, olditemnames);

			while (!validBlursingsDistribution)
			{
				validBlursingsDistribution = true;
				assignedBonusMalus = new();
				descriptionList = new();

				for (int i = 0; i < 6; i++)
				{
					var tempstring = new List<(int, string)>();
					var bonuscount = 0;
					var maluscount = 0;
					assignedBonusMalus.Add(new List<BonusMalus>());

					if ((bool)flags.RandomizeClassKeyItems)
					{
						assignedBonusMalus[i].Add(startWithKiBlurses.SpliceRandom(rng));
						tempstring.Add((0, assignedBonusMalus[i].First().Description));
					}

					while (bonuscount < maxbonus)
					{
						var validBonuses = bonusNormal.Where(x => x.ClassList.Contains((Classes)i) && !assignedBonusMalus[i].Select(y => y.Action).ToList().Contains(x.Action)).ToList();

						if (!validBonuses.Any())
						{
							validBlursingsDistribution = false;
							break;
						}

						validBonuses.Shuffle(rng);
						assignedBonusMalus[i].Add(validBonuses.First());
						tempstring.Add((0, validBonuses.First().Description));
						bonusNormal.Remove(validBonuses.First());
						bonuscount++;
					}

					while (maluscount < maxmalus)
					{
						var validMaluses = malusNormal.Where(x => x.ClassList.Contains((Classes)i) &&
							!assignedBonusMalus[i].Select(y => y.Action).ToList().Contains(x.Action) &&
							!(x.Action == BonusMalusAction.CantLearnSpell && assignedBonusMalus[i].Where(y => y.Action == BonusMalusAction.InnateSpells).SelectMany(x => x.SpellsMod).ToList().Contains(x.SpellSlotMod)) &&
							!(x.Action == BonusMalusAction.NoPromoMagic && assignedBonusMalus[i].Where(y => y.Action == BonusMalusAction.MpGainOnMaxMpGain).Any()) &&
							!(x.Action == BonusMalusAction.ArmorRemove && x.Equipment.Contains(Item.Ribbon) && assignedBonusMalus[i].Where(y => y.Action == BonusMalusAction.InnateResist && y.StatMod == 0xFF).Any())).ToList();

						if (!validMaluses.Any())
						{
							validBlursingsDistribution = false;
							break;
						}

						validMaluses.Shuffle(rng);
						assignedBonusMalus[i].Add(validMaluses.First());
						if (validMaluses.First().Action == BonusMalusAction.IntMod)
						{
							tempstring.Add((0, validMaluses.First().Description));
						}
						else
						{
							tempstring.Add((1, validMaluses.First().Description));
						}

						malusNormal.Remove(validMaluses.First());
						maluscount++;
					}

					if (!validBlursingsDistribution)
					{
						break;
					}

					descriptionList.Add(string.Join("\n\n", tempstring.Where(x => x.Item1 == 0).Select(x => x.Item2)) + "\n\n\nMALUS\n\n" + string.Join("\n\n", tempstring.Where(x => x.Item1 == 1).Select(x => x.Item2)));
				}
			}

			// Apply bonuses and maluses to stats
			for (int i = 0; i < 6; i++)
			{
				// Order the list so bonuses/maluses interact correctly
				List<BonusMalusAction> priorityAction = new() { BonusMalusAction.SpcMax, BonusMalusAction.CantLearnSpell };

				assignedBonusMalus[i].Reverse();

				assignedBonusMalus[i] = assignedBonusMalus[i]
					.Where(x => !priorityAction.Contains(x.Action))
					.ToList()
					.Concat(assignedBonusMalus[i].Where(x => priorityAction.Contains(x.Action)).ToList())
					.ToList();

				foreach (var bonusmalus in assignedBonusMalus[i])
				{
					switch (bonusmalus.Action)
					{
						case BonusMalusAction.StrMod:
							_classes[i].StrStarting = (byte)Math.Max(_classes[i].StrStarting + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.AgiMod:
							_classes[i].AgiStarting = (byte)Math.Max(_classes[i].AgiStarting + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.IntMod:
							_classes[i].IntStarting = (byte)Math.Max(_classes[i].IntStarting + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.VitMod:
							_classes[i].VitStarting = (byte)Math.Max(_classes[i].VitStarting + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.LckMod:
							_classes[i].LckStarting = (byte)Math.Max(_classes[i].LckStarting + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.HitMod:
							_classes[i].HitStarting = (byte)Math.Max(_classes[i].HitStarting + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.MDefMod:
							_classes[i].MDefStarting = (byte)Math.Max(_classes[i].MDefStarting + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.HpMod:
							_classes[i].HpStarting = (byte)Math.Max(_classes[i].HpStarting + bonusmalus.StatMod, 1);
							break;
						case BonusMalusAction.StrGrowth:
							_classes[i].StrGrowth = bonusmalus.StatGrowth;
							break;
						case BonusMalusAction.AgiGrowth:
							_classes[i].AgiGrowth = bonusmalus.StatGrowth;
							break;
						case BonusMalusAction.IntGrowth:
							_classes[i].IntGrowth = bonusmalus.StatGrowth;
							break;
						case BonusMalusAction.VitGrowth:
							_classes[i].VitGrowth = bonusmalus.StatGrowth;
							break;
						case BonusMalusAction.LckGrowth:
							_classes[i].LckGrowth = bonusmalus.StatGrowth;
							break;
						case BonusMalusAction.HpGrowth:
							_classes[i].HpGrowth = bonusmalus.StatGrowth;
							break;
						case BonusMalusAction.HitGrowth:
							_classes[i].HitGrowth = (byte)Math.Max(_classes[i].HitGrowth + bonusmalus.StatMod, 0);
							_classes[i + 6].HitGrowth = (byte)Math.Max(_classes[i + 6].HitGrowth + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.MDefGrowth:
							_classes[i].MDefGrowth = (byte)Math.Max(_classes[i].MDefGrowth + bonusmalus.StatMod, 0);
							_classes[i + 6].MDefGrowth = (byte)Math.Max(_classes[i + 6].MDefGrowth + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.WeaponAdd:
							_weaponPermissions.AddPermissionsRange(bonusmalus.Equipment.Select(x => ((Classes)i, x)).ToList());
							_weaponPermissions.AddPermissionsRange(bonusmalus.Equipment.Select(x => ((Classes)(i + 6), x)).ToList());
							break;
						case BonusMalusAction.WeaponRemove:
							_weaponPermissions.RemovePermissionsRange(bonusmalus.Equipment.Select(x => ((Classes)i, x)).ToList());
							_weaponPermissions.RemovePermissionsRange(bonusmalus.Equipment.Select(x => ((Classes)(i + 6), x)).ToList());
							break;
						case BonusMalusAction.WeaponReplace:
							_weaponPermissions[(Classes)i] = bonusmalus.Equipment;
							_weaponPermissions[(Classes)(i + 6)] = bonusmalus.Equipment;
							break;
						case BonusMalusAction.ArmorAdd:
							// mod 99 used to indicate it's for promo only
							if ((byte)bonusmalus.StatMod != 99) {
								_armorPermissions.AddPermissionsRange(bonusmalus.Equipment.Select(x => ((Classes)i, x)).ToList());
							}
							_armorPermissions.AddPermissionsRange(bonusmalus.Equipment.Select(x => ((Classes)(i + 6), x)).ToList());
							break;
						case BonusMalusAction.ArmorRemove:
							_armorPermissions.RemovePermissionsRange(bonusmalus.Equipment.Select(x => ((Classes)i, x)).ToList());
							_armorPermissions.RemovePermissionsRange(bonusmalus.Equipment.Select(x => ((Classes)(i + 6), x)).ToList());
							break;
						case BonusMalusAction.ArmorReplace:
							// mod 99 used to indicate it's for promo only
							if ((byte)bonusmalus.StatMod != 99) {
								_armorPermissions[(Classes)i] = bonusmalus.Equipment;
							}
							_armorPermissions[(Classes)(i + 6)] = bonusmalus.Equipment;
							break;
						case BonusMalusAction.SpcMod:
							_classes[i].SpCStarting = (byte)Math.Max(_classes[i].SpCStarting + bonusmalus.StatMod, 0);
							_classes[i + 6].SpCStarting = (byte)Math.Max(_classes[i + 6].SpCStarting + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.WhiteSpellcaster:
							if (_classes[i].SpCStarting < (byte)bonusmalus.StatMod)
								_classes[i].SpCStarting = (byte)bonusmalus.StatMod;
							if (_classes[i].MaxSpC < (byte)bonusmalus.StatMod2)
								_classes[i].MaxSpC = (byte)bonusmalus.StatMod2;
							if (i == (int)Classes.Thief && bonusmalus.SpcGrowth.Select(x => (int)x).ToList().Sum() == exKnightMPlist.Select(x => (int)x).ToList().Sum())
								_classes[i].SpCGrowth = exNinjaMPlist; // Edge case for thief getting Knight Sp
							else if (_classes[i].SpCGrowth.Select(x => (int)x).ToList().Sum() < bonusmalus.SpcGrowth.Select(x => (int)x).ToList().Sum())
								_classes[i].SpCGrowth = bonusmalus.SpcGrowth;

							if (_classes[i + 6].SpCStarting < (byte)bonusmalus.StatMod)
								_classes[i + 6].SpCStarting = (byte)bonusmalus.StatMod;
							if (_classes[i + 6].MaxSpC < (byte)bonusmalus.StatMod2)
								_classes[i + 6].MaxSpC = (byte)bonusmalus.StatMod2;

							_spellPermissions.AddPermissionsRange(bonusmalus.SpellList.Select(x => ((Classes)i, x)).ToList());
							_spellPermissions.AddPermissionsRange(bonusmalus.SpellList.Select(x => ((Classes)(i + 6), x)).ToList());
							break;
						case BonusMalusAction.BlackSpellcaster:
							if (_classes[i].SpCStarting < (byte)bonusmalus.StatMod)
								_classes[i].SpCStarting = (byte)bonusmalus.StatMod;
							if (_classes[i].MaxSpC < (byte)bonusmalus.StatMod2)
								_classes[i].MaxSpC = (byte)bonusmalus.StatMod2;
							if (_classes[i].SpCGrowth.Select(x => (int)x).ToList().Sum() < bonusmalus.SpcGrowth.Select(x => (int)x).ToList().Sum())
								_classes[i].SpCGrowth = bonusmalus.SpcGrowth;

							if (_classes[i + 6].SpCStarting < (byte)bonusmalus.StatMod)
								_classes[i + 6].SpCStarting = (byte)bonusmalus.StatMod;
							if (_classes[i + 6].MaxSpC < (byte)bonusmalus.StatMod2)
								_classes[i + 6].MaxSpC = (byte)bonusmalus.StatMod2;
							_spellPermissions.AddPermissionsRange(bonusmalus.SpellList.Select(x => ((Classes)i, x)).ToList());
							_spellPermissions.AddPermissionsRange(bonusmalus.SpellList.Select(x => ((Classes)(i + 6), x)).ToList());
							break;
						case BonusMalusAction.SpcMax:
							_classes[i].MaxSpC = (byte)Math.Max(_classes[i].MaxSpC + bonusmalus.StatMod, 1);
							_classes[i + 6].MaxSpC = (byte)Math.Max(_classes[i + 6].MaxSpC + bonusmalus.StatMod, 1);
							break;
						case BonusMalusAction.SpcGrowth:
							_classes[i].SpCGrowth = bonusmalus.SpcGrowth;
							_classes[i + 6].SpCGrowth = bonusmalus.SpcGrowth;
							break;
						case BonusMalusAction.PowerRW:
							// Strong blessing applies unpromoted; regular only applies promoted
							if ((byte)bonusmalus.StatMod == 1) {
								_spellPermissions[(Classes)i] = wmWhiteSpells.Concat(bmBlackSpells).ToList();
							}
							_spellPermissions[(Classes)(i + 6)] = wwWhiteSpells.Concat(bwBlackSpells).ToList();
							break;
						case BonusMalusAction.NoPromoMagic:
							_spellPermissions.ClearPermissions((Classes)i + 6);
							_classes[i + 6].MaxSpC = 0;
							_classes[i + 6].SpCStarting = 0;
							break;
						case BonusMalusAction.LockpickingLevel:
							int newLockPickingLevel = flags.LockpickingLevelRequirement + bonusmalus.StatMod;
							if ((bool)flags.Lockpicking)
							{
								//constrain lp level to 1-50
								newLockPickingLevel = Math.Max(1, newLockPickingLevel);
								newLockPickingLevel = Math.Min(50, newLockPickingLevel);
								rom.SetLockpickingLevel(newLockPickingLevel);
							}
							break;
						case BonusMalusAction.InnateResist:
							_classes[i].InnateResist = (byte)bonusmalus.StatMod;
							_classes[i + 6].InnateResist = (byte)bonusmalus.StatMod;
							break;
						case BonusMalusAction.BonusXp:
							double scale = bonusmalus.StatMod / 100.0;
							rom.ScaleAltExp(scale, (FF1Rom.FF1Class)i);
							break;
						case BonusMalusAction.MpGainOnMaxMpGain:
							rom.Put(lut_MpGainOnMaxMpGainClasses + i, Blob.FromHex("01"));
							rom.Put(lut_MpGainOnMaxMpGainClasses + i + 6, Blob.FromHex("01"));
							break;
						case BonusMalusAction.StartWithSpell:
							_classes[i].StartingSpell = bonusmalus.SpellSlotMod;
							_classes[i + 6].StartingSpell = bonusmalus.SpellSlotMod;
							break;
						case BonusMalusAction.CantLearnSpell:
							_spellPermissions.RemovePermission((Classes)i, (SpellSlots)bonusmalus.SpellSlotMod.BattleId);
							_spellPermissions.RemovePermission((Classes)(i + 6), (SpellSlots)bonusmalus.SpellSlotMod.BattleId);
							break;
						case BonusMalusAction.StartWithGold:
							_classes[i].StartWithGold = (BlursesStartWithGold)bonusmalus.StatMod;
							_classes[i + 6].StartWithGold = (BlursesStartWithGold)bonusmalus.StatMod;
							break;
						case BonusMalusAction.StartWithMp:
							_classes[i].StartWithMp = true;
							_classes[i + 6].StartWithMp = true;
							break;
						case BonusMalusAction.Hunter:
							_classes[i].HurtType |= (byte)bonusmalus.StatMod;
							_classes[i + 6].HurtType |= (byte)bonusmalus.StatMod;
							break;
						case BonusMalusAction.UnarmedAttack:
							_classes[i].UnarmedAttack = true;
							_classes[i + 6].UnarmedAttack = true;
							break;
						case BonusMalusAction.ThorMaster:
							_classes[i].ThorMaster = true;
							_classes[i + 6].ThorMaster = true;
							_weaponPermissions.AddPermissionsRange(new List<(Classes, Item)> { ((Classes)(i), Item.ThorHammer) });
							_weaponPermissions.AddPermissionsRange(new List<(Classes, Item)> { ((Classes)(i + 6), Item.ThorHammer) });
							break;
						case BonusMalusAction.CatClawMaster:
							_classes[i].CatClawMaster = true;
							_classes[i + 6].CatClawMaster = true;
							_weaponPermissions.AddPermissionsRange(new List<(Classes, Item)> { ((Classes)(i), Item.CatClaw) });
							_weaponPermissions.AddPermissionsRange(new List<(Classes, Item)> { ((Classes)(i + 6), Item.CatClaw) });
							break;
						case BonusMalusAction.SteelLord:
							_classes[i].SteelLord = true;
							_classes[i + 6].SteelLord = true;
							break;
						case BonusMalusAction.WoodAdept:
							_classes[i].WoodAdept = true;
							_classes[i + 6].WoodAdept = true;
							_armorPermissions.AddPermissionsRange(new List<(Classes, Item)> { ((Classes)i, Item.WoodenArmor), ((Classes)i, Item.WoodenHelm), ((Classes)i, Item.WoodenShield), ((Classes)(i+6), Item.WoodenArmor), ((Classes)(i+6), Item.WoodenHelm), ((Classes)(i+6), Item.WoodenShield) });
							break;
						case BonusMalusAction.Sick:
							_classes[i].Sick = true;
							_classes[i + 6].Sick = true;
							break;
						case BonusMalusAction.Sleepy:
							_classes[i].Sleepy = true;
							_classes[i + 6].Sleepy = true;
							break;
						case BonusMalusAction.StartWithKI:
							_classes[i].StartingKeyItem = (Item)bonusmalus.StatMod;
							_classes[i + 6].StartingKeyItem = (Item)bonusmalus.StatMod;
							break;
						case BonusMalusAction.InnateSpells:
							_classes[i].InnateSpells = bonusmalus.SpellsMod.ToList();
							_classes[i + 6].InnateSpells = bonusmalus.SpellsMod.ToList();
							break;
					}
				}
			}

			return descriptionList;
		}

		public void DoRandomizeClassChaosMode(bool mixSpellsAndKeepPerm, bool buffedthief, MT19337 rng, FF1Rom rom)
		{
			// Ranked list of equipment
			List<Weapon> weaponsList = new();
			for (int i = 0; i < 40; i++)
			{
				weaponsList.Add(new Weapon(i, rom));
				if (weaponsList.Last().Icon == WeaponIcon.NONE)
				{
					weaponsList.Last().Icon = _weaponPermissions[Classes.BlackWizard].Contains((Item)weaponsList.Last().Id) ? WeaponIcon.KNIFE : WeaponIcon.SWORD;
				}
			}

			List<List<Item>> arArmor = new List<List<Item>>();
			arArmor.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>() });
			arArmor.Add(new List<Item> { Item.Cloth, Item.Copper, Item.Silver, Item.Gold, Item.Opal });
			arArmor.Add(new List<Item>(arArmor[2]) { Item.WoodenArmor });
			arArmor.Add(new List<Item>(arArmor[3]) { Item.ChainArmor, Item.SilverArmor });
			arArmor.Add(new List<Item>(arArmor[4]) { Item.IronArmor, Item.FlameArmor, Item.IceArmor, Item.SteelArmor });
			arArmor.Add(new List<Item>(arArmor[5]) { Item.DragonArmor, Item.OpalArmor });

			List<List<Item>> arShield = new List<List<Item>>();
			arShield.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>() });
			arShield.Add(new List<Item> { Item.ProCape });
			arShield.Add(new List<Item>(arShield[3]) { Item.Buckler });
			arShield.Add(new List<Item>(arShield[4]) { Item.WoodenShield, Item.IronShield, Item.SilverShield, Item.IceShield, Item.FlameShield });
			arShield.Add(new List<Item>(arShield[5]) { Item.OpalShield, Item.AegisShield });

			List<List<Item>> arHelmet = new List<List<Item>>();
			arHelmet.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>() });
			arHelmet.Add(new List<Item> { Item.Ribbon, Item.Cap });
			arHelmet.Add(new List<Item>(arHelmet[3]) { Item.WoodenHelm, Item.IronHelm, Item.SilverHelm });
			arHelmet.Add(new List<Item>(arHelmet[4]) { Item.HealHelm });
			arHelmet.Add(new List<Item>(arHelmet[5]) { Item.OpalHelm });

			List<List<Item>> arGauntlet = new List<List<Item>>();
			arGauntlet.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>() });
			arGauntlet.Add(new List<Item> { Item.Gloves, Item.ProRing });
			arGauntlet.Add(new List<Item>(arGauntlet[3]) { Item.CopperGauntlets, Item.IronGauntlets, Item.SilverGauntlets });
			arGauntlet.Add(new List<Item>(arGauntlet[4]) { Item.PowerGauntlets, Item.ZeusGauntlets });
			arGauntlet.Add(new List<Item>(arGauntlet[5]) { Item.OpalGauntlets });

			List<List<Item>> wpHammer = new List<List<Item>>();
			wpHammer.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>() });
			wpHammer.Add(weaponsList.Where(x => x.Icon == WeaponIcon.HAMMER).Select(x => x.Id).ToList());

			List<List<Item>> wpStaff = new List<List<Item>>();
			wpStaff.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>() });
			wpStaff.Add(weaponsList.Where(x => x.Icon == WeaponIcon.STAFF).Select(x => x.Id).ToList());

			List<List<Item>> wpKnife = new List<List<Item>>();
			wpKnife.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>() });
			wpKnife.Add(weaponsList.Where(x => x.Icon == WeaponIcon.KNIFE).Select(x => x.Id).ToList());

			List<List<Item>> wpNunchuck = new List<List<Item>>();
			wpNunchuck.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>() });
			wpNunchuck.Add(weaponsList.Where(x => x.Icon == WeaponIcon.CHUCK).Select(x => x.Id).ToList());

			List<List<Item>> wpAxe = new List<List<Item>>();
			wpAxe.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>() });
			wpAxe.Add(weaponsList.Where(x => x.Icon == WeaponIcon.AXE).Select(x => x.Id).ToList());

			List<List<Item>> wpSword = new List<List<Item>>();
			wpSword.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>() });

			var swordsList = weaponsList.Where(x => x.Icon == WeaponIcon.SWORD).Select(x => x.Id).ToList();
			var figherPermissions = _weaponPermissions[Classes.Fighter].Where(x => swordsList.Contains(x)).ToList();
			var thiefPermissions = _weaponPermissions[Classes.Thief].Where(x => swordsList.Contains(x)).ToList();
			var knightPermissions = _weaponPermissions[Classes.Knight].Where(x => x != Item.Xcalber).ToList();

			bool noBSwords = (figherPermissions.Count == thiefPermissions.Count);

			if (noBSwords)
			{
				wpSword.Add(new List<Item>());
			}
			else
			{
				wpSword.Add(swordsList.Where(x => thiefPermissions.Contains(x)).ToList());
			}

			wpSword.Add(swordsList.Where(x => figherPermissions.Contains(x)).ToList());
			wpSword.Add(swordsList.Where(x => knightPermissions.Contains(x)).ToList());

			// Spell charge ranks to distribute
			var startSpellcharges = new List<Rank> { Rank.A, Rank.A, Rank.S, Rank.S, Rank.S, Rank.A };
			var promoSpellcharges = new List<Rank> { Rank.B, Rank.C, Rank.B, Rank.C, Rank.B, Rank.C };

			// Equipment ranks to distribute
			var startWeapons = new List<(RankedType, Rank)> { (RankedType.Swords, Rank.A), (RankedType.Swords, Rank.A), noBSwords ? (RankedType.Swords, Rank.A) : (RankedType.Swords, Rank.B), (RankedType.Nunchucks, Rank.S), (RankedType.Axes, Rank.S), (RankedType.Axes, Rank.S), (RankedType.Hammers, Rank.S), (RankedType.Hammers, Rank.S), (RankedType.Knives, Rank.S), (RankedType.Knives, Rank.S), (RankedType.Staves, Rank.S), (RankedType.Staves, Rank.S) };
			var promoWeapons = new List<(RankedType, Rank)> { (RankedType.Swords, Rank.S), (RankedType.Swords, Rank.S), (RankedType.Swords, Rank.S), (RankedType.Nunchucks, Rank.S), (RankedType.Axes, Rank.S), (RankedType.Hammers, Rank.S), (RankedType.Knives, Rank.S), (RankedType.Staves, Rank.S) };

			var startArmors = new List<(RankedType, Rank)> { (RankedType.Armors, Rank.A), (RankedType.Armors, Rank.B), (RankedType.Armors, Rank.C), (RankedType.Armors, Rank.C), (RankedType.Armors, Rank.D), (RankedType.Armors, Rank.D) };
			var promoArmors = new List<(RankedType, Rank)> { (RankedType.Armors, Rank.A), (RankedType.Armors, Rank.S) };

			var startShields = new List<(RankedType, Rank)> { (RankedType.Shields, Rank.A), (RankedType.Shields, Rank.B), (RankedType.Shields, Rank.B), (RankedType.Shields, Rank.C), (RankedType.Shields, Rank.C) };
			var promoShields = new List<(RankedType, Rank)> { (RankedType.Shields, Rank.A), (RankedType.Shields, Rank.S) };

			var startHelmets = new List<(RankedType, Rank)> { (RankedType.Helmets, Rank.B), (RankedType.Helmets, Rank.C), (RankedType.Helmets, Rank.C), (RankedType.Helmets, Rank.C), (RankedType.Helmets, Rank.C), (RankedType.Helmets, Rank.C) };
			var promoHelmets = new List<(RankedType, Rank)> { (RankedType.Helmets, Rank.A), (RankedType.Helmets, Rank.S) };

			var startGauntlets = new List<(RankedType, Rank)> { (RankedType.Gauntlets, Rank.B), (RankedType.Gauntlets, Rank.C), (RankedType.Gauntlets, Rank.C), (RankedType.Gauntlets, Rank.C), (RankedType.Gauntlets, Rank.C), (RankedType.Gauntlets, Rank.C) };
			var promoGauntlets = new List<(RankedType, Rank)> { (RankedType.Gauntlets, Rank.A), (RankedType.Gauntlets, Rank.S) };

			var chargesRank = new List<(Rank, Rank, int, int)> {
				(Rank.F, Rank.B, 0, 6),
				(Rank.F, Rank.B, 1, 7),
				(Rank.F, Rank.F, 2, 8),
				(Rank.A, Rank.A, 3, 9),
				(Rank.S, Rank.S, 4, 10),
				(Rank.S, Rank.S, 5, 11)
			};

			List<string> classBaseString = new List<string> { "Fi", "Th", "Bb", "Rm", "Wm", "Bm" };
			List<string> classPromoString = new List<string> { "Kn", "Ni", "Ma", "Rw", "Ww", "Bw" };

			// new arrays
			var newChargeList = new List<List<byte>>();
			var newMaxChargeList = Enumerable.Repeat((byte)0x00, 12).ToArray();

			// Get shuffle data
			var shuffleStartingStats = new List<byte>();
			var shuffleLevelUp = new List<List<bool>>();
			var shuffleHP = new List<List<bool>>();
			var shuffleHit = new List<byte>();

			shuffleStartingStats.AddRange(_classes.GetRange(0, 6).Select(x => x.StrStarting).ToList());
			shuffleStartingStats.AddRange(_classes.GetRange(0, 6).Select(x => x.AgiStarting).ToList());
			shuffleStartingStats.AddRange(_classes.GetRange(0, 6).Select(x => x.IntStarting).ToList());
			shuffleStartingStats.AddRange(_classes.GetRange(0, 6).Select(x => x.VitStarting).ToList());
			shuffleStartingStats.AddRange(_classes.GetRange(0, 6).Select(x => x.LckStarting).ToList());
			shuffleStartingStats.AddRange(_classes.GetRange(0, 6).Select(x => x.HitStarting).ToList());
			shuffleStartingStats.AddRange(_classes.GetRange(0, 6).Select(x => x.MDefStarting).ToList());
			shuffleLevelUp.AddRange(_classes.GetRange(0, 6).Select(x => x.StrGrowth).ToList());
			shuffleLevelUp.AddRange(_classes.GetRange(0, 6).Select(x => x.AgiGrowth).ToList());
			shuffleLevelUp.AddRange(_classes.GetRange(0, 6).Select(x => x.IntGrowth).ToList());
			shuffleLevelUp.AddRange(_classes.GetRange(0, 6).Select(x => x.VitGrowth).ToList());
			shuffleLevelUp.AddRange(_classes.GetRange(0, 6).Select(x => x.LckGrowth).ToList());
			shuffleHP.AddRange(_classes.GetRange(0, 6).Select(x => x.HpGrowth).ToList());
			shuffleHit.AddRange(_classes.GetRange(0, 6).Select(x => x.HitGrowth).ToList());

			var mdefGrowthBase = _classes.GetRange(0, 6).Select(x => x.MDefGrowth).ToList();
			var mdefGrowthPromo = _classes.GetRange(6, 6).Select(x => x.MDefGrowth).ToList();

			var shuffleMDef = Enumerable.Zip(mdefGrowthBase, mdefGrowthPromo, (mdefGrowthBase, mdefGrowthPromo) => new KeyValuePair<byte, byte>(mdefGrowthBase, mdefGrowthPromo)).ToList();

			var whitePermissions = _spellPermissions.GetWhitePermissions().ToList();
			var blackPermissions = _spellPermissions.GetBlackPermissions().ToList();
			var shuffleWhitePermissions = new List<int> { 0, 1, 2, 3, 4, 5 };
			var shuffleBlackPermissions = new List<int> { 0, 1, 2, 3, 4, 5 };

			// Actual Shuffle
			shuffleStartingStats.Shuffle(rng);
			shuffleLevelUp.Shuffle(rng);
			shuffleHP.Shuffle(rng);
			shuffleHit.Shuffle(rng);
			shuffleMDef.Shuffle(rng);
			shuffleWhitePermissions.Shuffle(rng);
			if (mixSpellsAndKeepPerm)
				shuffleBlackPermissions = shuffleWhitePermissions;
			else
				shuffleBlackPermissions.Shuffle(rng);

			// Generate Ranks
			int maxStats = shuffleStartingStats.Max();
			if (buffedthief)
			{
				maxStats = shuffleStartingStats.Where(x => x < maxStats).Max();
			}
			int minStats = shuffleStartingStats.Min();
			int maxLvStats = shuffleLevelUp.Select(x => x.GetRange(0, 24).Where(y => y == true).Count()).Max();
			int minLvStats = shuffleLevelUp.Select(x => x.GetRange(0, 24).Where(y => y == true).Count()).Min();
			int spreadStats = (maxLvStats + maxStats - minLvStats - minStats) / 5;

			// For HP, max is a Lv25 Fighter average HP, min is a Lv25 Black Mage average HP
			int maxLvHp = 555;
			int minLvHp = 255;
			int spreadLvHp = (maxLvHp - minLvHp) / 4;

			var statsRanks = new List<Rank>();
			var hpRanks = new List<Rank>();
			var magicRanks = Enumerable.Repeat(Rank.F, 24).ToArray();

			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					if (shuffleStartingStats[i * 7 + j] + shuffleLevelUp[i * 5 + j].GetRange(0, 24).Where(x => x == true).Count() > (maxLvStats + maxStats - spreadStats))
						statsRanks.Add(Rank.S);
					else if (shuffleStartingStats[i * 7 + j] + shuffleLevelUp[i * 5 + j].GetRange(0, 24).Where(x => x == true).Count() > (maxLvStats + maxStats - spreadStats * 2))
						statsRanks.Add(Rank.A);
					else if (shuffleStartingStats[i * 7 + j] + shuffleLevelUp[i * 5 + j].GetRange(0, 24).Where(x => x == true).Count() > (maxLvStats + maxStats - spreadStats * 3))
						statsRanks.Add(Rank.B);
					else if (shuffleStartingStats[i * 7 + j] + shuffleLevelUp[i * 5 + j].GetRange(0, 24).Where(x => x == true).Count() > (maxLvStats + maxStats - spreadStats * 4))
						statsRanks.Add(Rank.C);
					else if (shuffleStartingStats[i * 7 + j] + shuffleLevelUp[i * 5 + j].GetRange(0, 24).Where(x => x == true).Count() > (maxLvStats + maxStats - spreadStats * 5))
						statsRanks.Add(Rank.D);
					else
						statsRanks.Add(Rank.E);
				}
			}

			for (int i = 0; i < shuffleHP.Count(); i++)
			{
				var hpAverage25 = _classes[i].HpStarting +
					(shuffleHP[i].GetRange(0, 24).Where(x => x == true).Count() * 23) +
					(shuffleStartingStats[i * 7 + 3] / 4 + shuffleLevelUp[i * 5 + 3].Where(x => x == true).Count() / 8) * 24;

				if (hpAverage25 > (maxLvHp))
					hpRanks.Add(Rank.S);
				else if (hpAverage25 > (maxLvHp - spreadLvHp))
					hpRanks.Add(Rank.A);
				else if (hpAverage25 > (maxLvHp - spreadLvHp * 2))
					hpRanks.Add(Rank.B);
				else if (hpAverage25 > (maxLvHp - spreadLvHp * 3))
					hpRanks.Add(Rank.C);
				else if (hpAverage25 > (maxLvHp - spreadLvHp * 4))
					hpRanks.Add(Rank.D);
				else
					hpRanks.Add(Rank.E);
			}

			for (int i = 0; i < 6; i++)
			{
				switch (shuffleWhitePermissions[i])
				{
					case 0:
						magicRanks[i + 6] = Rank.C;
						break;
					case 3:
						magicRanks[i] = Rank.C;
						magicRanks[i + 6] = Rank.B;
						break;
					case 4:
						magicRanks[i] = Rank.A;
						magicRanks[i + 6] = Rank.S;
						break;
				}

				switch (shuffleBlackPermissions[i])
				{
					case 1:
						magicRanks[i + 18] = Rank.B;
						break;
					case 3:
						magicRanks[i + 12] = Rank.B;
						magicRanks[i + 18] = Rank.A;
						break;
					case 5:
						magicRanks[i + 12] = Rank.A;
						magicRanks[i + 18] = Rank.S;
						break;
				}
			}

			// Update data
			for (int i = 0; i < 6; i++)
			{
				_classes[i].GetStartingStats(shuffleStartingStats.GetRange(i * 7, 7), statsRanks.GetRange(i * 5, 5));
				_classes[i + 6].GetStartingStats(shuffleStartingStats.GetRange(i * 7, 7), statsRanks.GetRange(i * 5, 5));

				_classes[i].GetLevelUp(shuffleLevelUp.GetRange(i * 5, 5));
				_classes[i + 6].GetLevelUp(shuffleLevelUp.GetRange(i * 5, 5));

				_classes[i].HpGrowth = shuffleHP[i];
				_classes[i + 6].HpGrowth = shuffleHP[i];
				_classes[i].Ranks[(int)RankedType.HP] = hpRanks[i];
				_classes[i + 6].Ranks[(int)RankedType.HP] = hpRanks[i];

				_classes[i].HitGrowth = shuffleHit[i];
				_classes[i + 6].HitGrowth = shuffleHit[i];
				_classes[i].MDefGrowth = shuffleMDef[i].Key;
				_classes[i + 6].MDefGrowth = shuffleMDef[i].Value;

				_spellPermissions.ClearPermissions((Classes)i);
				_spellPermissions.ClearPermissions((Classes)(i + 6));

				_classes[i].Ranks[(int)RankedType.White] = magicRanks[i];

				if (magicRanks[i] > Rank.F)
				{
					_spellPermissions.AddPermissionsRange(whitePermissions.Find(x => x.Item1 == (Classes)shuffleWhitePermissions[i]).Item2.Select(x => ((Classes)i, x)).ToList());
					_classes[i].MagicRanks[0] = classBaseString[shuffleWhitePermissions[i]];
				}

				_classes[i + 6].Ranks[(int)RankedType.White] = magicRanks[i + 6];

				if (magicRanks[i + 6] > Rank.F)
				{
					_spellPermissions.AddPermissionsRange(whitePermissions.Find(x => x.Item1 == (Classes)(shuffleWhitePermissions[i] + 6)).Item2.Select(x => ((Classes)(i + 6), x)).ToList());
					_classes[i + 6].MagicRanks[0] = classPromoString[shuffleWhitePermissions[i]];
				}

				_classes[i].Ranks[(int)RankedType.Black] = magicRanks[i + 12];

				if (magicRanks[i + 12] > Rank.F)
				{
					_spellPermissions.AddPermissionsRange(blackPermissions.Find(x => x.Item1 == (Classes)shuffleBlackPermissions[i]).Item2.Select(x => ((Classes)i, x)).ToList());
					_classes[i].MagicRanks[1] = classBaseString[shuffleBlackPermissions[i]];
				}

				_classes[i + 6].Ranks[(int)RankedType.Black] = magicRanks[i + 18];

				if (magicRanks[i + 18] > Rank.F)
				{
					_spellPermissions.AddPermissionsRange(blackPermissions.Find(x => x.Item1 == (Classes)(shuffleBlackPermissions[i] + 6)).Item2.Select(x => ((Classes)(i + 6), x)).ToList());
					_classes[i + 6].MagicRanks[1] = classPromoString[shuffleBlackPermissions[i]];
				}
			}


			// Shuffle spell charges, we do this after shuffling spells
			//  because we want to give spell charges to actual spellcasters
			startSpellcharges.Shuffle(rng);
			promoSpellcharges.Shuffle(rng);
			var chargeList = _classes.GetRange(0, 12).Select(x => x.SpCGrowth).ToList();
			var maxCharges = _classes.GetRange(0, 12).Select(x => x.MaxSpC).ToList();

			for (int i = 0; i < 6; i++)
			{
				if (_classes[i].Ranks[(int)RankedType.White] > Rank.F || _classes[i].Ranks[(int)RankedType.Black] > Rank.F)
				{
					var tempClass = chargesRank.Find(x => x.Item1 == startSpellcharges.First()).Item3;
					_classes[i].Ranks[(int)RankedType.Charges] = startSpellcharges.First();
					_classes[i + 6].Ranks[(int)RankedType.Charges] = startSpellcharges.First();
					_classes[i].MagicRanks[2] = classBaseString[tempClass];
					_classes[i + 6].MagicRanks[2] = classPromoString[tempClass];
					_classes[i].SpCGrowth = chargeList[tempClass].ToList();
					_classes[i].MaxSpC = maxCharges[tempClass];
					_classes[i + 6].MaxSpC = maxCharges[tempClass + 6];
					_classes[i].SpCStarting = 0x02;
					_classes[i + 6].SpCStarting = 0x02;
					startSpellcharges.RemoveRange(0, 1);
				}
				else if (_classes[i + 6].Ranks[(int)RankedType.Black] > Rank.F)
				{
					var tempClass = 1;
					_classes[i].Ranks[(int)RankedType.Charges] = Rank.F;
					_classes[i + 6].Ranks[(int)RankedType.Charges] = Rank.B;
					_classes[i + 6].MagicRanks[2] = classPromoString[tempClass];
					_classes[i].SpCGrowth = chargeList[tempClass].ToList();
					_classes[i + 6].SpCGrowth = chargeList[tempClass].ToList();
					_classes[i].MaxSpC = maxCharges[tempClass];
					_classes[i + 6].MaxSpC = maxCharges[tempClass + 6];
					_classes[i].SpCStarting = 0x00;
					_classes[i + 6].SpCStarting = 0x00;
					promoSpellcharges.RemoveRange(0, 1);
				}
				else if (_classes[i + 6].Ranks[(int)RankedType.White] > Rank.F)
				{
					var tempClass = 0;
					_classes[i].Ranks[(int)RankedType.Charges] = Rank.F;
					_classes[i + 6].Ranks[(int)RankedType.Charges] = Rank.B;
					_classes[i + 6].MagicRanks[2] = classPromoString[tempClass];
					_classes[i].SpCGrowth = chargeList[tempClass].ToList();
					_classes[i + 6].SpCGrowth = chargeList[tempClass].ToList();
					_classes[i].MaxSpC = maxCharges[tempClass];
					_classes[i + 6].MaxSpC = maxCharges[tempClass + 6];
					_classes[i].SpCStarting = 0x00;
					_classes[i + 6].SpCStarting = 0x00;
					promoSpellcharges.RemoveRange(0, 1);
				}
				else
				{
					_classes[i].Ranks[(int)RankedType.Charges] = Rank.F;
					_classes[i + 6].Ranks[(int)RankedType.Charges] = Rank.F;
					_classes[i].MaxSpC = 0x00;
					_classes[i + 6].MaxSpC = 0x00;
					_classes[i].SpCStarting = 0x00;
					_classes[i + 6].SpCStarting = 0x00;
				}
			}

			// Distribute equipment permissions
			foreach (var x in startWeapons)
			{
				int select = Rng.Between(rng, 0, 5);

				while (_classes[select].Ranks[(int)x.Item1] >= x.Item2)
					select = Rng.Between(rng, 0, 5);

				_classes[select].Ranks[(int)x.Item1] = x.Item2;
				_classes[select + 6].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach (var x in promoWeapons)
			{
				int select = Rng.Between(rng, 6, 11);

				while (_classes[select].Ranks[(int)x.Item1] >= x.Item2)
					select = Rng.Between(rng, 6, 11);

				_classes[select].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach (var x in startArmors)
			{
				int select = Rng.Between(rng, 0, 5);

				while (_classes[select].Ranks[(int)x.Item1] >= x.Item2)
					select = Rng.Between(rng, 0, 5);

				_classes[select].Ranks[(int)x.Item1] = x.Item2;
				_classes[select + 6].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach (var x in promoArmors)
			{
				int select = Rng.Between(rng, 6, 11);

				while (_classes[select].Ranks[(int)x.Item1] >= x.Item2)
					select = Rng.Between(rng, 6, 11);

				_classes[select].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach (var x in startShields)
			{
				int select = Rng.Between(rng, 0, 5);

				while (_classes[select].Ranks[(int)x.Item1] >= x.Item2)
					select = Rng.Between(rng, 0, 5);

				_classes[select].Ranks[(int)x.Item1] = x.Item2;
				_classes[select + 6].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach (var x in promoShields)
			{
				int select = Rng.Between(rng, 6, 11);

				while (_classes[select].Ranks[(int)x.Item1] >= x.Item2)
					select = Rng.Between(rng, 6, 11);

				_classes[select].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach (var x in startHelmets)
			{
				int select = Rng.Between(rng, 0, 5);

				while (_classes[select].Ranks[(int)x.Item1] >= x.Item2)
					select = Rng.Between(rng, 0, 5);

				_classes[select].Ranks[(int)x.Item1] = x.Item2;
				_classes[select + 6].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach (var x in promoHelmets)
			{
				int select = Rng.Between(rng, 6, 11);

				while (_classes[select].Ranks[(int)x.Item1] >= x.Item2)
					select = Rng.Between(rng, 6, 11);

				_classes[select].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach (var x in startGauntlets)
			{
				int select = Rng.Between(rng, 0, 5);

				while (_classes[select].Ranks[(int)x.Item1] >= x.Item2)
					select = Rng.Between(rng, 0, 5);

				_classes[select].Ranks[(int)x.Item1] = x.Item2;
				_classes[select + 6].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach (var x in promoGauntlets)
			{
				int select = Rng.Between(rng, 6, 11);

				while (_classes[select].Ranks[(int)x.Item1] >= x.Item2)
					select = Rng.Between(rng, 6, 11);

				_classes[select].Ranks[(int)x.Item1] = x.Item2;
			}

			// Add individual equipment for each equipment right
			for (int i = 0; i < 12; i++)
			{
				_weaponPermissions.ClearPermissions((Classes)i);
				_weaponPermissions.AddPermissionsRange(wpSword[(int)_classes[i].Ranks[(int)RankedType.Swords]].Select(x => ((Classes)i, x)).ToList());
				_weaponPermissions.AddPermissionsRange(wpHammer[(int)_classes[i].Ranks[(int)RankedType.Hammers]].Select(x => ((Classes)i, x)).ToList());
				_weaponPermissions.AddPermissionsRange(wpKnife[(int)_classes[i].Ranks[(int)RankedType.Knives]].Select(x => ((Classes)i, x)).ToList());
				_weaponPermissions.AddPermissionsRange(wpAxe[(int)_classes[i].Ranks[(int)RankedType.Axes]].Select(x => ((Classes)i, x)).ToList());
				_weaponPermissions.AddPermissionsRange(wpStaff[(int)_classes[i].Ranks[(int)RankedType.Staves]].Select(x => ((Classes)i, x)).ToList());
				_weaponPermissions.AddPermissionsRange(wpNunchuck[(int)_classes[i].Ranks[(int)RankedType.Nunchucks]].Select(x => ((Classes)i, x)).ToList());
				_weaponPermissions.AddPermission((Classes)i, Item.Masamune);

				_armorPermissions.ClearPermissions((Classes)i);
				_armorPermissions.AddPermissionsRange(arArmor[(int)_classes[i].Ranks[(int)RankedType.Armors]].Select(x => ((Classes)i, x)).ToList());
				_armorPermissions.AddPermissionsRange(arShield[(int)_classes[i].Ranks[(int)RankedType.Shields]].Select(x => ((Classes)i, x)).ToList());
				_armorPermissions.AddPermissionsRange(arHelmet[(int)_classes[i].Ranks[(int)RankedType.Helmets]].Select(x => ((Classes)i, x)).ToList());
				_armorPermissions.AddPermissionsRange(arGauntlet[(int)_classes[i].Ranks[(int)RankedType.Gauntlets]].Select(x => ((Classes)i, x)).ToList());
			}

			// Add class exclusive equipment
			_weaponPermissions.AddPermission(Classes.Knight, Item.Xcalber);
			_weaponPermissions.AddPermission(Classes.Ninja, Item.Katana);
			_armorPermissions.AddPermission(Classes.WhiteWizard, Item.WhiteShirt);
			_armorPermissions.AddPermission(Classes.BlackWizard, Item.BlackShirt);
		}

		public BonusMalus CreateRandomResistBonusMalus(MT19337 rng)
		{
			byte innateResistValue = 0x00;
			string description = "Res. ";
			List<SpellElement> elements = Enum.GetValues(typeof(SpellElement)).Cast<SpellElement>().ToList();
			elements.Remove(SpellElement.Any);
			elements.Remove(SpellElement.All);

			//3 picks but can get a none
			for (int picks = 0; picks < 3; picks++)
			{

				SpellElement pickedElement = elements.SpliceRandom(rng);
				switch (pickedElement)
				{
					case SpellElement.Any:
					case SpellElement.None:
					case SpellElement.All:
					    break;
					case SpellElement.Status:
						description += "S";
						break;
					case SpellElement.Poison:
						description += "P";
						break;
					case SpellElement.Time:
						description += "T";
						break;
					case SpellElement.Death:
						description += "D";
						break;
					case SpellElement.Fire:
						description += "F";
						break;
					case SpellElement.Ice:
						description += "I";
						break;
					case SpellElement.Lightning:
						description += "L";
						break;
					case SpellElement.Earth:
						description += "E";
						break;
				}

				innateResistValue |= (byte)pickedElement;
			}

			return new BonusMalus(BonusMalusAction.InnateResist, description, mod: innateResistValue);
		}

		public List<BonusMalus> CreateSpellBonuses(FF1Rom rom, MT19337 rng, Flags flags)
		{
			List<BonusMalus> spellBlursings = new();

			SpellHelper spellHelper = new(rom);

			List<List<byte>> blackSpellList = new();
			List<List<byte>> whiteSpellList = new();

			blackSpellList.Add(spellHelper.FindSpells(SpellRoutine.Fast, SpellTargeting.Any).Select(x => (byte)x.Id).ToList()); // Fast
			blackSpellList.Add(spellHelper.FindSpells(SpellRoutine.Sabr, SpellTargeting.OneCharacter).Select(x => (byte)x.Id).ToList()); // Tmpr
			//blackSpellList.Add(spellHelper.FindSpells(SpellRoutine.Sabr, SpellTargeting.Self).Where(s => s.Info.effect <= 18).Select(x => (byte)x.Id).ToList()); // Sabr
			blackSpellList.Add(new List<byte> { (byte)(rom.Get(FF1Rom.MagicOutOfBattleOffset + FF1Rom.MagicOutOfBattleSize * 10, 1)[0]) }); // Warp
			// Include LOCK/LOK2 as long as accuracy isn't too low
			if ((flags.LockMode != LockHitMode.Vanilla) && (flags.LockMode != LockHitMode.Accuracy107)) {
				blackSpellList.Add(spellHelper.FindSpells(SpellRoutine.Lock, SpellTargeting.Any).Select(x => (byte)x.Id).ToList()); // Lock or Lok2
			}

			whiteSpellList.Add(spellHelper.FindSpells(SpellRoutine.Life, SpellTargeting.OneCharacter).Select(x => (byte)x.Id).ToList()); // Life
			//whiteSpellList.Add(spellHelper.FindSpells(SpellRoutine.Ruse, SpellTargeting.Self).Select(x => (byte)x.Id).ToList()); // Ruse
			whiteSpellList.Add(spellHelper.FindSpells(SpellRoutine.Ruse, SpellTargeting.AllCharacters).Where(s => s.Info.effect <= 50).Select(x => (byte)x.Id).ToList()); // Inv2
			whiteSpellList.Add(spellHelper.FindSpells(SpellRoutine.Heal, SpellTargeting.OneCharacter).Where(s => s.Info.effect >= 70 && s.Info.effect <= 140).Select(x => (byte)x.Id).ToList()); //Cur3
			whiteSpellList.Add(spellHelper.FindSpells(SpellRoutine.Heal, SpellTargeting.AllCharacters).Where(s => s.Info.effect >= 24 && s.Info.effect <= 40 && s.Info.Level <= 4).Select(x => (byte)x.Id).ToList()); //Hel2 if Lvl 4 or Less
			whiteSpellList.Add(new List<byte> { (byte)(rom.Get(FF1Rom.MagicOutOfBattleOffset + FF1Rom.MagicOutOfBattleSize * 12, 1)[0]) }); // Exit

			foreach (var spell in blackSpellList)
			{
				if (spell.Any())
				{
					var test = SpellSlotStructure.GetSpellSlots();

					SpellSlotInfo spellId = SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == spell.PickRandom(rng));
					if (spellId != null)
					{
						spellBlursings.Add(new BonusMalus(BonusMalusAction.InnateSpells, "+" + rom.ItemsText[(int)spellId.NameId], spellsmod: new List<SpellSlotInfo> { spellId, new SpellSlotInfo(), new SpellSlotInfo() }, Classes: new List<Classes> { Classes.RedMage, Classes.BlackMage }));
					}
				}
			}

			foreach (var spell in whiteSpellList)
			{
				if (spell.Any())
				{
					var test = SpellSlotStructure.GetSpellSlots();
					var pickedSpell = spell.PickRandom(rng);
					SpellSlotInfo spellId = SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == pickedSpell);
					if (spellId != null)
					{
						spellBlursings.Add(new BonusMalus(BonusMalusAction.InnateSpells, "+" + rom.ItemsText[(int)spellId.NameId], spellsmod: new List<SpellSlotInfo> { spellId, new SpellSlotInfo(), new SpellSlotInfo() }, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage }));
					}
				}
			}

			return spellBlursings;
		}
		public List<BonusMalus> CreateSpellMaluses(FF1Rom rom, MT19337 rng, Flags flags)
		{

			List<BonusMalus> spellBlursings = new();

			SpellHelper spellHelper = new(rom);

			List<List<byte>> spellList = new();

			spellList.Add(spellHelper.FindSpells(SpellRoutine.Fast, SpellTargeting.Any).Select(x => (byte)x.Id).ToList()); // Fast
			spellList.Add(spellHelper.FindSpells(SpellRoutine.Sabr, SpellTargeting.OneCharacter).Select(x => (byte)x.Id).ToList()); // Tmpr
			spellList.Add(spellHelper.FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies, SpellElement.None).Where(s => s.Info.effect >= 100).Select(x => (byte)x.Id).ToList()); // Nuke
			spellList.Add(spellHelper.FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies).Where(s => s.Info.effect >= 50 && s.Info.elem != SpellElement.None).Select(x => (byte)x.Id).ToList()); // Fir3, Ice3, or Lit3
			spellList.Add(new List<byte> { (byte)(rom.Get(FF1Rom.MagicOutOfBattleOffset + (FF1Rom.MagicOutOfBattleSize * 10), 1)[0]) }); // Warp
			// Include LOCK/LOK2 as long as accuracy isn't too low
			if ((flags.LockMode != LockHitMode.Vanilla) && (flags.LockMode != LockHitMode.Accuracy107))
			{
				spellList.Add(spellHelper.FindSpells(SpellRoutine.Lock, SpellTargeting.Any).Select(x => (byte)x.Id).ToList()); // Lock or Lok2
			}

			spellList.Add(spellHelper.FindSpells(SpellRoutine.Life, SpellTargeting.OneCharacter).Select(x => (byte)x.Id).ToList()); // Life
			spellList.Add(spellHelper.FindSpells(SpellRoutine.Ruse, SpellTargeting.AllCharacters).Select(x => (byte)x.Id).ToList()); // Inv2
			spellList.Add(spellHelper.FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies, SpellElement.None).Where(s => s.Info.effect > 70 && s.Info.effect < 100).Select(x => (byte)x.Id).ToList()); // Fade
			spellList.Add(new List<byte> { (byte)(rom.Get(FF1Rom.MagicOutOfBattleOffset + (FF1Rom.MagicOutOfBattleSize * 12), 1)[0]) }); // Exit
			spellList.Add(spellHelper.FindSpells(SpellRoutine.Heal, SpellTargeting.OneCharacter).Where(s => s.Info.effect >= 70 && s.Info.effect <= 200).Select(x => (byte)x.Id).ToList()); // Cur3
			spellList.Add(spellHelper.FindSpells(SpellRoutine.Heal, SpellTargeting.AllCharacters).Where(s => s.Info.effect >= 42 && s.Info.effect <= 100).Select(x => (byte)x.Id).ToList()); // Hel3
			spellList.Add(spellHelper.FindSpells(SpellRoutine.DefElement, SpellTargeting.Any).Where(s => s.Info.status == SpellStatus.Any).Select(x => (byte)x.Id).ToList()); // Wall

			foreach (var spell in spellList)
			{
				if (spell.Any())
				{
					List<Classes> validClasses = new();
					SpellSlotInfo spellId = SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == spell.PickRandom(rng));

					if (spellId == null)
					{
						continue;
					}

					if (_spellPermissions[Classes.RedMage].Where(x => x == (SpellSlots)spellId.BattleId).Any())
					{
						validClasses.Add(Classes.RedMage);
					}

					if (_spellPermissions[Classes.BlackMage].Where(x => x == (SpellSlots)spellId.BattleId).Any())
					{
						validClasses.Add(Classes.BlackMage);
					}

					if (_spellPermissions[Classes.WhiteMage].Where(x => x == (SpellSlots)spellId.BattleId).Any())
					{
						validClasses.Add(Classes.WhiteMage);
					}

					if (validClasses.Any())
					{
						spellBlursings.Add(new BonusMalus(BonusMalusAction.CantLearnSpell, "No " + rom.ItemsText[(int)spellId.NameId], spellslotmod: spellId, Classes: validClasses));
					}
				}
			}

			return spellBlursings;
		}

		public (List<BonusMalus>, List<BonusMalus>) CreateMagicBonuses(FF1Rom rom, MT19337 rng, Flags flags)
		{
			List<BonusMalus> spellBlursingsStrong = new();
			List<BonusMalus> spellBlursingsNormal = new();

			SpellHelper spellHelper = new(rom);

			List<List<byte>> blackSpellList = new();
			List<List<byte>> whiteSpellList = new();


			List<byte> spellNuke = spellHelper.FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies).Where(s => s.Info.elem == SpellElement.None && s.Info.effect >= 100).Select(x => (byte)x.Id).ToList(); // Nuke
			List<byte> spellElem3 =spellHelper.FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies).Where(s => s.Info.effect >= 50 && s.Info.elem != SpellElement.None).Select(x => (byte)x.Id).ToList(); 
			List<byte> spellElem2 = spellHelper.FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies).Where(s => s.Info.elem != SpellElement.None && s.Info.effect >= 30 && s.Info.effect < 50).Select(x => (byte)x.Id).ToList(); 
			List<byte> spellFast = spellHelper.FindSpells(SpellRoutine.Fast, SpellTargeting.Any).Select(x => (byte)x.Id).ToList(); // Fast
			List<byte> spellTmpr = spellHelper.FindSpells(SpellRoutine.Sabr, SpellTargeting.OneCharacter).Select(x => (byte)x.Id).ToList(); // Tmpr
			List<byte> spellSabr = spellHelper.FindSpells(SpellRoutine.Sabr, SpellTargeting.Self).Where(s => s.Info.effect <= 18).Select(x => (byte)x.Id).ToList(); // Sabr
			List<byte> spellWarp = new List<byte> { (byte)(rom.Get(FF1Rom.MagicOutOfBattleOffset + FF1Rom.MagicOutOfBattleSize * 10, 1)[0]) }; // Warp
			List<byte> spellLife = spellHelper.FindSpells(SpellRoutine.Life, SpellTargeting.OneCharacter).Select(x => (byte)x.Id).ToList(); // Life
			List<byte> spellRuse = spellHelper.FindSpells(SpellRoutine.Ruse, SpellTargeting.Self).Select(x => (byte)x.Id).ToList(); // Ruse
			List<byte> spellInv2 = spellHelper.FindSpells(SpellRoutine.Ruse, SpellTargeting.AllCharacters).Where(s => s.Info.effect <= 50).Select(x => (byte)x.Id).ToList(); // Inv2
			List<byte> spellCur3 = spellHelper.FindSpells(SpellRoutine.Heal, SpellTargeting.OneCharacter).Where(s => s.Info.effect >= 70 && s.Info.effect <= 140).Select(x => (byte)x.Id).ToList(); //Cur3
			List<byte> spellCur4 = spellHelper.FindSpells(SpellRoutine.FullHeal, SpellTargeting.OneCharacter).Select(x => (byte)x.Id).ToList(); //Cur4
			List<byte> spellHel2 = spellHelper.FindSpells(SpellRoutine.Heal, SpellTargeting.AllCharacters).Where(s => s.Info.effect >= 24 && s.Info.effect <= 40).Select(x => (byte)x.Id).ToList(); //Hel2
			List<byte> spellHel3 = spellHelper.FindSpells(SpellRoutine.Heal, SpellTargeting.AllCharacters).Where(s => s.Info.effect > 40).Select(x => (byte)x.Id).ToList(); //Hel3
			List<byte> spellExit = new List<byte> { (byte)(rom.Get(FF1Rom.MagicOutOfBattleOffset + FF1Rom.MagicOutOfBattleSize * 12, 1)[0]) }; // Exit
			List<byte> spellCleaning = spellHelper.FindSpells(SpellRoutine.CureAilment, SpellTargeting.Any).Select(x => (byte)x.Id).ToList();
			List<byte> spellDoom = spellHelper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any).Where(s => s.Info.effect == (byte)SpellStatus.Death || s.Info.effect == (byte)SpellStatus.Stone).Select(x => (byte)x.Id).ToList();


			if (spellNuke.Any())
			{
				var selectedSpell = spellNuke.PickRandom(rng);
				SpellSlotInfo spellId = SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == selectedSpell);

				if (spellId != null)
				{
					spellBlursingsStrong.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Nuke Magic", spellsmod: new List<SpellSlotInfo> { spellId, spellId, spellId }, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));
				}
			}

			if (spellElem3.Count >= 3)
			{
				List<SpellSlotInfo> spells = new();

				for (int i = 0; i < 3; i++)
				{
					var selectedSpell = spellElem3.SpliceRandom(rng);
					var spellId = SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == selectedSpell);

					if (spellId != null)
					{
						spells.Add(spellId);
					}
				}

				if (spells.Count == 3)
				{
					spellBlursingsNormal.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Elem+ Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], spells[2] }, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));

					spellBlursingsStrong.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Elem+ Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], spells[2] }, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt }));
				}
			}

			if (spellElem2.Count >= 3)
			{
				List<SpellSlotInfo> spells = new();

				for (int i = 0; i < 3; i++)
				{
					var selectedSpell = spellElem2.SpliceRandom(rng);
					var spellId = SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == selectedSpell);

					if (spellId != null)
					{
						spells.Add(spellId);
					}
				}

				if (spells.Count == 3)
				{
					spellBlursingsNormal.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Elem Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], spells[2] }));
				}
			}

			if (spellCleaning.Count >= 3)
			{
				List<SpellSlotInfo> spells = new();

				for (int i = 0; i < 3; i++)
				{
					var selectedSpell = spellCleaning.SpliceRandom(rng);
					var spellId = SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == selectedSpell);

					if (spellId != null)
					{
						spells.Add(spellId);
					}
				}

				if (spells.Count == 3)
				{
					spellBlursingsNormal.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Clean Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], spells[2] }));
				}
			}

			if (spellDoom.Count >= 3)
			{
				List<SpellSlotInfo> spells = new();

				for (int i = 0; i < 3; i++)
				{
					var selectedSpell = spellDoom.SpliceRandom(rng);
					var spellId = SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == selectedSpell);

					if (spellId != null)
					{
						spells.Add(spellId);
					}
				}
				if (spells.Count == 3)
				{
					spellBlursingsNormal.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Doom Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], spells[2] }, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));
				}
			}

			if (spellCur3.Any() && spellHel2.Any() && spellLife.Any())
			{
				List<SpellSlotInfo> spells = new();

				spells.Add(SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == spellCur3.PickRandom(rng)));
				spells.Add(SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == spellHel2.PickRandom(rng)));
				spells.Add(SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == spellLife.PickRandom(rng)));

				if (spells.Where(x => x != null).Count() == 3)
				{
					spellBlursingsNormal.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Heal Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], spells[2] }));
				}
			}

			if (spellCur4.Any() && spellHel3.Any() && spellLife.Any())
			{
				List<SpellSlotInfo> spells = new();

				spells.Add(SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == spellCur4.PickRandom(rng)));
				spells.Add(SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == spellHel3.PickRandom(rng)));
				spells.Add(SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == spellLife.PickRandom(rng)));

				if (spells.Where(x => x != null).Count() == 3)
				{
					spellBlursingsNormal.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Heal+ Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], spells[2] }, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));

					spellBlursingsStrong.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Heal+ Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], spells[2] }, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt }));

				}
			}

			if (spellRuse.Any() && spellSabr.Any())
			{
				List<SpellSlotInfo> spells = new();

				spells.Add(SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == spellRuse.PickRandom(rng)));
				spells.Add(SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == spellSabr.PickRandom(rng)));
				spells.Add(new SpellSlotInfo());

				if (spells.Where(x => x != null).Count() == 3)
				{
					spellBlursingsStrong.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Self Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], spells[2] }, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt }));
				}
			}

			if (spellTmpr.Any() && spellFast.Any() && spellInv2.Any())
			{
				List<SpellSlotInfo> spells = new();

				spells.Add(SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == spellTmpr.PickRandom(rng)));
				spells.Add(SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == spellFast.PickRandom(rng)));
				spells.Add(SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == spellInv2.PickRandom(rng)));

				if (spells.Where(x => x != null).Count() == 3)
				{
					spellBlursingsNormal.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Buff Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], spells[2] }, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));

					spellBlursingsStrong.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Buff Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], spells[2] }, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt }));
				}
			}

			if (spellWarp.Any() && spellExit.Any())
			{
				List<SpellSlotInfo> spells = new();

				spells.Add(SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == spellWarp.PickRandom(rng)));
				spells.Add(SpellSlotStructure.GetSpellSlots().Find(x => x.NameId == spellExit.PickRandom(rng)));
				spells.Add(new SpellSlotInfo());

				if (spells.Where(x => x != null).Count() == 3)
				{
					spellBlursingsNormal.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Tele Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], spells[2] }));
				}
			}

			return (spellBlursingsNormal, spellBlursingsStrong);
		}
		public void ProcessStartWithRoutines(Flags flags, List<int> blursesValues, FF1Rom rom)
		{
			// See 1B_B000_StartWithRoutines.asm
			// Utilities
			rom.PutInBank(0x1B, 0xB000, Blob.FromHex("B90061C90C9002A90C8410A8B1EDC90108A4102860B90061C90C9002A90C8410A8B1ED08A4102860"));

			// Party Initial Setup Hijack
			rom.PutInBank(0x1F, 0xC0AC, Blob.FromHex("2012D828EAEAEAEAEAEAEA"));
			rom.PutInBank(0x1F, 0xD812, rom.CreateLongJumpTableEntry(0x1B, 0xB080));

			
			// The labels for the Malus Gold Amounts are processed separately in Randomize -
			// Change those as well when changing the numbers below
			int MalusGoldAmount = 150;
			// No malus available for 0GP start
			//if (flags.StartingGold == StartingGold.None)
			if (flags.StartingGold == StartingGold.Gp100) {
				MalusGoldAmount = 50;
			}
			else if (flags.StartingGold == StartingGold.Gp200) {
				MalusGoldAmount = 100;
			}
			else if (flags.StartingGold == StartingGold.Gp400 || flags.StartingGold == StartingGold.RandomLow) {
				MalusGoldAmount = 150;
			}
			else if (flags.StartingGold == StartingGold.Gp800) {
				MalusGoldAmount = 350;
			}
			else if (flags.StartingGold == StartingGold.Gp2500) {
				MalusGoldAmount = 1100;
			}
			else if (flags.StartingGold == StartingGold.Gp9999) {
				MalusGoldAmount = 4500;
			}

			// StartWith Initialization Routine
			rom.PutInBank(0x1B, 0xB300, Blob.FromHex($"A9B348A92048A91B48A9FE48A90648A9DD48A99948A97F48A9FF48A91E484C07FEA000202EB398186940A8D0F660203EB32075B320C7B32006B4202EB460A98085EDA9B485EEA2002015B0F027AAA900851085118512E000F01718A96465108510A90065118511A90065128512CA1890E520EADD60A98D85EDA9B485EEA2002000B0D042E8A90085108511851218A9{(MalusGoldAmount % 0x100):X2}65108510A9{((MalusGoldAmount / 0x100) % 0x100):X2}65118511A9{(MalusGoldAmount / 0x10000):X2}65128512AD1C6038E5108D1C60AD1D60E5118D1D60AD1E60E5128D1E60B00BA9008D1C608D1D608D1E6060A203A91C8511A9638512A99A85EDA9B485EE2015B0F0129111E611A90D1865ED85ED9002E6EECAD0E9E003F01198186907AABD20631869029D20639D286360A200A9C185EDA9B485EE2000B0F001609848AAA007E8BD20631869019D20639D286388D0F068A860A200A9CE85EDA9B485EE2015B0F006AAA9019D006060"));

			// Insert luts
			Blob lut_IncreaseGP = _classes.Select(x => (byte)(x.StartWithGold != BlursesStartWithGold.Remove ? (byte)x.StartWithGold : 0x00)).ToArray();
			Blob lut_DecreaseGP = _classes.Select(x => (byte)(x.StartWithGold == BlursesStartWithGold.Remove ? 0x01 : 0x00)).ToArray();
			Blob lut_InnateSpells01 = _classes.Select(x => (byte)(x.InnateSpells[0].Level > 0 ? x.InnateSpells[0].BattleId + 1 : 0x00)).ToArray();
			Blob lut_InnateSpells02 = _classes.Select(x => (byte)(x.InnateSpells[1].Level > 0 ? x.InnateSpells[1].BattleId + 1 : 0x00)).ToArray();
			Blob lut_InnateSpells03 = _classes.Select(x => (byte)(x.InnateSpells[2].Level > 0 ? x.InnateSpells[2].BattleId + 1 : 0x00)).ToArray();
			Blob lut_StartSpellsSpell = _classes.Select(x => (byte)x.StartingSpell.MenuId).ToArray();
			Blob lut_MpStart = _classes.Select(x => (byte)(x.StartWithMp ? 0x01 : 0x00)).ToArray();
			Blob lut_StartingKeyItems = _classes.Select(x => x.StartingKeyItem == Item.Canoe ? (byte)0x12 : (x.StartingKeyItem == Item.None ? (byte)0x00 : (byte)(x.StartingKeyItem + 0x20))).ToArray();

			rom.PutInBank(0x1B, 0xB480, lut_IncreaseGP + new byte[] { 0x00 } +
				lut_DecreaseGP + new byte[] { 0x00 } +
				lut_InnateSpells01 + new byte[] { 0x00 } +
				lut_InnateSpells02 + new byte[] { 0x00 } +
				lut_InnateSpells03 + new byte[] { 0x00 } +
				lut_MpStart + new byte[] { 0x00 } +
				lut_StartingKeyItems + new byte[] { 0x00 });

			// Compute catclawcrit outside of the game because there's not data on gear blursing
			int catclawcrit = 6 * (flags.WeaponCritRate ? 10 : 5);

			if ((bool)flags.RandomWeaponBonus && !(bool)flags.Weaponizer)
			{
				int blursevalue = blursesValues[Item.CatClaw - Item.WoodenNunchucks];
				catclawcrit += 3 * blursevalue;
			}

			// Battle Hijack, take over part of BB unarmed check
			rom.PutInBank(0x0C, 0xADBB, Blob.FromHex("2012D8F00760EAEAEAEAEAEA"));
			//rom.PutInBank(0x1F, 0xC271, rom.CreateLongJumpTableEntry(0x1B, 0xB080));

			// Battle StartWith
			rom.PutInBank(0x1B, 0xB080, Blob.FromHex($"A908C5F2F0034C00B320ABB020E0B02096B12017B12048B14C9BB0A90085EDA9B285EEA000B1822003B060A90D85EDA9B285EEA000B1822003B0D023A018B1823011C8B182300CC8B1823007C8B1823002A900297FC923D006A00FA9{catclawcrit:X2}918060A91A85EDA9B285EEA000B1822003B0D025A018B1823011C8B182300CC8B1823007C8B1823002A900297FC924D008A009B180180A918060A94E85EDA9B285EEA000B1822003B0D01FA01CB18210032037B1C8C020D0F460297FC905D00A9848A00BA902918068A860A95B85EDA9B285EE8A48A200A000B1822003B0D036A01CB1821003207FB1C8C020D0F4E003D02418A007B18069789002A9FF918068AA60297FC902D002E860C91BD002E860C911D001E86068AA60A000B182AABD27B2F006A00D1180918060EAEAEAEAEAEAEAA93485EDA9B285EEA000B1822003B0D008A001B1820920918260A94185EDA9B285EEA000B1822003B0D00F20E7FC2903D008A001B1820904918260"));

			// Insert luts
			Blob lut_Blackbelts = _classes.Select(x => (byte)(x.UnarmedAttack ? 0x01 : 0x00)).ToArray();
			Blob lut_CatClaws = _classes.Select(x => (byte)(x.CatClawMaster ? 0x01 : 0x00)).ToArray();
			Blob lut_ThorHammer = _classes.Select(x => (byte)(x.ThorMaster ? 0x01 : 0x00)).ToArray();
			Blob lut_Hunter = _classes.Select(x => (byte)x.HurtType).ToArray();
			Blob lut_Sleepy = _classes.Select(x => (byte)(x.Sleepy ? 0x01 : 0x00)).ToArray();
			Blob lut_Sick = _classes.Select(x => (byte)(x.Sick ? 0x01 : 0x00)).ToArray();
			Blob lut_SteelArmor = _classes.Select(x => (byte)(x.SteelLord ? 0x01 : 0x00)).ToArray();
			Blob lut_WoodArmors = _classes.Select(x => (byte)(x.WoodAdept ? 0x01 : 0x00)).ToArray();

			rom.PutInBank(0x1B, 0xB200, lut_Blackbelts + new byte[] { 0x00 } +
				lut_CatClaws + new byte[] { 0x00 } +
				lut_ThorHammer + new byte[] { 0x00 } +
				lut_Hunter + new byte[] { 0x00 } +
				lut_Sleepy + new byte[] { 0x00 } +
				lut_Sick + new byte[] { 0x00 } +
				lut_SteelArmor + new byte[] { 0x00 } +
				lut_WoodArmors + new byte[] { 0x00 });

			// Recruit Mode Switcher
			rom.PutInBank(0x1B, 0xB600, Blob.FromHex("A5108513A8B913B6A8202EB3A51385104CAA87004080C0"));
		}

		private List<BonusMalus> StartWithKeyItems(Flags flags, MT19337 rng, List<string> olditemnames)
		{
			List<BonusMalus> kiBlursings = new()
			{
				new BonusMalus(BonusMalusAction.StartWithKI, "+" + olditemnames[(int)Item.Crown], mod: (int)Item.Crown),
				new BonusMalus(BonusMalusAction.StartWithKI, "+" + olditemnames[(int)Item.Crystal], mod: (int)Item.Crystal),
				new BonusMalus(BonusMalusAction.StartWithKI, "+" + olditemnames[(int)Item.Herb], mod: (int)Item.Herb),
				new BonusMalus(BonusMalusAction.StartWithKI, "+" + olditemnames[(int)Item.Tnt], mod: (int)Item.Tnt),
				new BonusMalus(BonusMalusAction.StartWithKI, "+" + olditemnames[(int)Item.Adamant], mod: (int)Item.Adamant),
				new BonusMalus(BonusMalusAction.StartWithKI, "+" + olditemnames[(int)Item.Slab], mod: (int)Item.Slab),
				new BonusMalus(BonusMalusAction.StartWithKI, "+" + olditemnames[(int)Item.Ruby], mod: (int)Item.Ruby),
				new BonusMalus(BonusMalusAction.StartWithKI, "+" + olditemnames[(int)Item.Rod], mod: (int)Item.Rod),
				new BonusMalus(BonusMalusAction.StartWithKI, "+" + olditemnames[(int)Item.Chime], mod: (int)Item.Chime),
				new BonusMalus(BonusMalusAction.StartWithKI, "+" + olditemnames[(int)Item.Cube], mod: (int)Item.Cube),
				new BonusMalus(BonusMalusAction.StartWithKI, "+" + olditemnames[(int)Item.Bottle], mod: (int)Item.Bottle),
				new BonusMalus(BonusMalusAction.StartWithKI, "+" + olditemnames[(int)Item.Oxyale], mod: (int)Item.Oxyale),
			};

			if (!(bool)flags.FreeLute)
			{
				kiBlursings.Add(new BonusMalus(BonusMalusAction.StartWithKI, "+" + olditemnames[(int)Item.Lute], mod: (int)Item.Lute));
			}

			if (!(bool)flags.FreeTail && !(bool)flags.NoTail)
			{
				kiBlursings.Add(new BonusMalus(BonusMalusAction.StartWithKI, "+" + olditemnames[(int)Item.Tail], mod: (int)Item.Tail));
			}

			if (!(bool)flags.Lockpicking)
			{
				kiBlursings.Add(new BonusMalus(BonusMalusAction.StartWithKI, "+" + olditemnames[(int)Item.Key], mod: (int)Item.Key));
			}

			if (flags.NoOverworld)
			{
				kiBlursings.Add(new BonusMalus(BonusMalusAction.StartWithKI, "+SIGIL", mod: (int)Item.Floater));
			}

			if (flags.NoOverworld && !(bool)flags.IsCanoeFree)
			{
				kiBlursings.Add(new BonusMalus(BonusMalusAction.StartWithKI, "+MARK", mod: (int)Item.Canoe));
			}

			kiBlursings.Shuffle(rng);

			return kiBlursings.GetRange(0, 6);
		}
	}
	public class ClassData
	{
		// Indivdual class stats container
		private GearPermissions _weaponPermissions;
		private GearPermissions _armorPermissions;
		private SpellPermissions _spellPermissions;
		public bool Promoted { get; set; }
		public byte ClassID { get; set; }
		public byte HpStarting { get; set; }
		public byte StrStarting { get; set; }
		public byte AgiStarting { get; set; }
		public byte IntStarting { get; set; }
		public byte VitStarting { get; set; }
		public byte LckStarting { get; set; }
		public byte DmgStarting { get; set; }
		public byte HitStarting { get; set; }
		public byte EvaStarting { get; set; }
		public byte MDefStarting { get; set; }
		public byte SpCStarting { get; set; }
		public byte HitGrowth { get; set; }
		public byte MDefGrowth { get; set; }
		public List<bool> HpGrowth { get; set; }
		public List<bool> StrGrowth { get; set; }
		public List<bool> AgiGrowth { get; set; }
		public List<bool> IntGrowth { get; set; }
		public List<bool> VitGrowth { get; set; }
		public List<bool> LckGrowth { get; set; }
		public List<byte> SpCGrowth { get; set; }
		public byte MaxSpC { get; set; }
		public List<string> MagicRanks { get; set; }
		public List<Rank> Ranks { get; set; }
		public byte InnateResist { get; set; }
		public SpellSlotInfo StartingSpell { get; set; }
		public BlursesStartWithGold StartWithGold { get; set; }
		public bool StartWithMp { get; set; }
		public bool UnarmedAttack { get; set; }
		public bool CatClawMaster { get; set; }
		public bool ThorMaster { get; set; }
		public byte HurtType { get; set; }
		public bool Sleepy { get; set; }
		public bool Sick { get; set; }
		public bool WoodAdept { get; set; }
		public bool SteelLord { get; set; }
		public List<SpellSlotInfo> InnateSpells { get; set; }
		public Item StartingKeyItem { get; set; }

		public ClassData(byte classid, byte[] startingStats, byte[] levelUpStats, byte hitgrowth, byte mdefgrowth, byte maxspc, GearPermissions weapPerm, GearPermissions armorPerm, SpellPermissions spellPerm)
		{
			ClassID = classid;
			GetStartingStatsArray(startingStats);
			GetLevelUpArray(levelUpStats);
			HitGrowth = hitgrowth;
			MDefGrowth = mdefgrowth;
			MaxSpC = maxspc;
			_weaponPermissions = weapPerm;
			_armorPermissions = armorPerm;
			_spellPermissions = spellPerm;

			// For Chaos Mode
			MagicRanks = new List<string> { "- ", "- ", "- " };
			Ranks = Enumerable.Repeat((Rank)0, Enum.GetNames(typeof(RankedType)).Length).ToList();

			// For blursings
			InnateResist = 0;
			StartingSpell = new SpellSlotInfo();
			StartWithGold = BlursesStartWithGold.None;
			StartWithMp = false;
			UnarmedAttack = (classid == 2 || classid == 8);
			CatClawMaster = false;
			ThorMaster = false;
			HurtType = 0x00;
			Sleepy = false;
			Sick = false;
			WoodAdept = false;
			SteelLord = false;
			StartingKeyItem = Item.None;
			InnateSpells = new() { new SpellSlotInfo(), new SpellSlotInfo(), new SpellSlotInfo() };
		}

		public byte[] StartingStatsArray()
		{
			var startingStatsArray = new List<byte> { ClassID, HpStarting, StrStarting, AgiStarting, IntStarting, VitStarting, LckStarting, DmgStarting, HitStarting, EvaStarting, MDefStarting, SpCStarting, 0x00, 0x00, 0x00, 0x00 };
			return startingStatsArray.ToArray();
		}

		public void GetStartingStatsArray(byte[] startingStats)
		{
			ClassID = startingStats[0];
			HpStarting = startingStats[1];
			StrStarting = startingStats[2];
			AgiStarting = startingStats[3];
			IntStarting = startingStats[4];
			VitStarting = startingStats[5];
			LckStarting = startingStats[6];
			DmgStarting = startingStats[7];
			HitStarting = startingStats[8];
			EvaStarting = startingStats[9];
			MDefStarting = startingStats[10];
			SpCStarting = startingStats[11];
		}

		public void GetStartingStats(List<byte> startingStats, List<Rank> startingRank)
		{
			StrStarting = startingStats[0];
			AgiStarting = startingStats[1];
			IntStarting = startingStats[2];
			VitStarting = startingStats[3];
			LckStarting = startingStats[4];
			HitStarting = startingStats[5];
			MDefStarting = startingStats[6];

			Ranks[(int)RankedType.Strength] = startingRank[0];
			Ranks[(int)RankedType.Agility] = startingRank[1];
			Ranks[(int)RankedType.Intellect] = startingRank[2];
			Ranks[(int)RankedType.Vitality] = startingRank[3];
			Ranks[(int)RankedType.Luck] = startingRank[4];
		}

		public byte[] LevelUpArray()
		{
			var levelUp = new List<byte>();
			for (int j = 0; j < 49; j++)
			{
				byte tempStats = 0x00;

				tempStats |= HpGrowth[j] ? (byte)0x20 : (byte)0x00;
				tempStats |= StrGrowth[j] ? (byte)0x10 : (byte)0x00;
				tempStats |= AgiGrowth[j] ? (byte)0x08 : (byte)0x00;
				tempStats |= IntGrowth[j] ? (byte)0x04 : (byte)0x00;
				tempStats |= VitGrowth[j] ? (byte)0x02 : (byte)0x00;
				tempStats |= LckGrowth[j] ? (byte)0x01 : (byte)0x00;
				levelUp.Add(tempStats);
				levelUp.Add(SpCGrowth[j]);
			}
			return levelUp.ToArray();
		}
		public void GetLevelUpArray(byte[] levelUpStats)
		{
			HpGrowth = Enumerable.Repeat(false, 49).ToList();
			StrGrowth = Enumerable.Repeat(false, 49).ToList();
			AgiGrowth = Enumerable.Repeat(false, 49).ToList();
			IntGrowth = Enumerable.Repeat(false, 49).ToList();
			VitGrowth = Enumerable.Repeat(false, 49).ToList();
			LckGrowth = Enumerable.Repeat(false, 49).ToList();
			SpCGrowth = Enumerable.Repeat((byte)0x00, 49).ToList();

			for (int j = 0; j < 49; j++)
			{
				HpGrowth[j] = (levelUpStats[j * 2] & (byte)0x20) == 0 ? false : true;
				StrGrowth[j] = (levelUpStats[j * 2] & (byte)0x10) == 0 ? false : true;
				AgiGrowth[j] = (levelUpStats[j * 2] & (byte)0x08) == 0 ? false : true;
				IntGrowth[j] = (levelUpStats[j * 2] & (byte)0x04) == 0 ? false : true;
				VitGrowth[j] = (levelUpStats[j * 2] & (byte)0x02) == 0 ? false : true;
				LckGrowth[j] = (levelUpStats[j * 2] & (byte)0x01) == 0 ? false : true;
				SpCGrowth[j] = levelUpStats[j * 2 + 1];
			}
		}
		public void GetLevelUp(List<List<bool>> levelUpStats)
		{
			StrGrowth = levelUpStats[0];
			AgiGrowth = levelUpStats[1];
			IntGrowth = levelUpStats[2];
			VitGrowth = levelUpStats[3];
			LckGrowth = levelUpStats[4];
		}
		public void RecalculateAttackEvasion()
		{
				DmgStarting = (byte)Math.Min(StrStarting / 2, 255);
				EvaStarting = (byte)Math.Min(AgiStarting + 48, 255);
		}

	}
	partial class FF1Rom
	{

	}
}
