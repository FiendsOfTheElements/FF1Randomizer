using System.ComponentModel;
using FF1Lib.Helpers;
using RomUtilities;

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

	public enum ClassRandomizationMode
	{
		[Description("None")]
		None = 0,
		[Description("Blursings")]
		Blursings,
		[Description("Transmooglifier")]
		Transmooglifier,
		[Description("Chaos")]
		Chaos
	}

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
		StartWithKI,
		InnateSpells,
		LearnLampRibbon,
		ASpellsAutocast,
		LearDarkEvade,
		LearnSleepMDef,
		LearnSlowAbsorb,
		MasaCurse,
		RibbonCurse,
		DualWieldKnife,
	}

	public partial class GameClasses
	{
		private List<ClassData> _classes;
		private GearPermissions _weaponPermissions;
		private GearPermissions _armorPermissions;
		private SpellPermissions _spellPermissions;

		//const int lut_LvlUpHitRateBonus = 0x6CA59;
		//const int lut_LvlUpMagDefBonus = 0x6CA65;
		const int lut_InnateResist = 0x6D400;
		const int lut_MaxMP = 0x6C902;
		const int lut_MpGainOnMaxMpGainClasses = 0x6D830;
		const int NewLevelUpDataOffset = 0x6CDA9;
		const int old_lut_LvlUpHitRateBonus = 0x2CA59;
		const int old_lut_LvlUpMagDefBonus = 0x2CA65;
		//const int old_lut_InnateResist = 0x2D400;
		//const int old_lut_MaxMP = 0x2C902;
		//const int old_lut_MpGainOnMaxMpGainClasses = 0x2D830;
		const int old_LevelUpDataOffset = 0x2CDA9;
		const int StartingStatsOffset = 0x3040;

		const int startingStatsBank = 0x00;
		const int startingStatsOffset = 0xB040;

		const int classesDataBank = 0x0B;
		const int lut_LvlUpHitRateBonus = 0x9DDC;
		const int lut_LvlUpMagDefBonus = 0x9DE8;
		const int levelUpDataOffset = 0x9094;


		const int newClassesDataBank = 0x1B;
		const int new_lut_LvlUpHitRateBonus = 0x8A59;
		const int new_lut_LvlUpMagDefBonus = 0x8A65;
		const int new_levelUpDataOffset = 0x8DA9;
		public GameClasses(GearPermissions weapPerm, GearPermissions armorPerm, SpellPermissions spellPerm, FF1Rom rom)
		{
			_classes = new List<ClassData>();

			_weaponPermissions = weapPerm;
			_armorPermissions = armorPerm;
			_spellPermissions = spellPerm;

			// Addresses
			var startingStats = rom.GetFromBank(startingStatsBank, startingStatsOffset, 0x60).Chunk(0x10);
			var levelUpStats = rom.GetFromBank(classesDataBank, levelUpDataOffset, 588).Chunk(49 * 2);
			var hitGrowth = rom.GetFromBank(classesDataBank, lut_LvlUpHitRateBonus, 12).ToBytes().ToList();
			var mdefGrowthBase = rom.GetFromBank(classesDataBank, lut_LvlUpMagDefBonus, 6).ToBytes().ToList();
			var mdefGrowthPromo = rom.GetFromBank(classesDataBank, lut_LvlUpMagDefBonus + 6, 6).ToBytes().ToList();
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
			rom.PutInBank(startingStatsBank, startingStatsOffset, _classes.GetRange(0, 6).SelectMany(x => x.StartingStatsArray()).ToArray());

			// Insert level up data
			rom.PutInBank(newClassesDataBank, new_levelUpDataOffset, _classes.GetRange(0, 6).SelectMany(x => x.LevelUpArray()).ToArray());

			// Insert hit% and mdef growth rate
			rom.PutInBank(newClassesDataBank, new_lut_LvlUpHitRateBonus, _classes.Select(x => x.HitGrowth).ToArray());
			rom.PutInBank(newClassesDataBank, new_lut_LvlUpMagDefBonus, _classes.Select(x => x.MDefGrowth).ToArray());

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
			if ((int)flags.ThiefHitBuff == (int)ThiefHit.Vanilla)
			{
				return;
			}

			int hits = (int)flags.ThiefHitBuff;

			_classes[(int)Classes.Thief].HitGrowth = (byte)hits;
			_classes[(int)Classes.Ninja].HitGrowth = (byte)hits;
		}

		public void RaiseRedMageHitRate(Flags flags)
		{
			if ((int)flags.RedMageHitBuff == (int)RedMageHit.Vanilla)
			{
				return;
			}

			int hits = (int)flags.RedMageHitBuff;

			_classes[(int)Classes.RedMage].HitGrowth = (byte)hits;
			_classes[(int)Classes.RedWizard].HitGrowth = (byte)hits;
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

		public void CustomSpellPermissions(Flags flags, MT19337 rng)
		{
			PinkMage(flags);
			BlackKnight(flags);
			WhiteNinja(flags);
			Knightlvl4(flags, rng);
		}

		public void PinkMage(Flags flags)
		{
			if (!(bool)flags.PinkMage)
			{
				return;

			}




			var whiteSlots = _spellPermissions[Classes.RedMage].Where(s => ((int)s % 8) < 4).ToList();
			var blackSlots = _spellPermissions[Classes.RedMage].Where(s => ((int)s % 8) >= 4).ToList();

			var whitetoblackslots = whiteSlots.Select(s => (s + 4)).ToList();
			var blacktowhiteslots = blackSlots.Select(s => (s - 4)).ToList();
			_spellPermissions.ClearPermissions(Classes.RedMage);
			var allRMslots = whitetoblackslots.Concat(blacktowhiteslots).ToList();
			_spellPermissions.AddPermissionsRange(allRMslots.Select(x => (Classes.RedMage, x)).ToList());

			var whiteSlotW = _spellPermissions[Classes.RedWizard].Where(s => ((int)s % 8) < 4).ToList();
			var BlackSlotW = _spellPermissions[Classes.RedWizard].Where(s => ((int)s % 8) >= 4).ToList();

			var whitetoblackW = whiteSlotW.Select(s => (s + 4)).ToList();
			var blacktowhiteW = BlackSlotW.Select(s => (s - 4)).ToList();
			_spellPermissions.ClearPermissions(Classes.RedWizard);
			var allRWslots = whitetoblackW.Concat(blacktowhiteW).ToList();
			_spellPermissions.AddPermissionsRange(allRWslots.Select(x => (Classes.RedWizard, x)).ToList());


		}

		private static List<byte> KnightLvl4Charge = new() { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000,
														0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00001111, 0b00000000, 0b00001111, 0b00000000, 0b00001111,
														0b00000000, 0b00001111, 0b00000000, 0b00001111, 0b00000000, 0b00001111, 0b00000000, 0b00001111, 0b00000000,
														0b00001111, 0b00000000, 0b00001111, 0b00000000, 0b00001111, 0b00000000, 0b00001111, 0b00000000, 0b00001111,
														0b00000000, 0b00001111, 0b00000000, 0b00001111, 0b00000000, 0b00001111, 0b00000000, 0b00001111, 0b00000000,
														0b00001111, 0b00000000, 0b00001111, 0b00000000 };


		public void BlackKnight(Flags flags)
		{
			if (!(bool)flags.BlackKnight)
			{
				return;

			}

			if ((bool)flags.BlackKnightKeep)
			{

				var whiteslotsK = _spellPermissions[Classes.Knight].Where(s => ((int)s % 8) < 4).ToList();
				var whitetoblackslotsK = whiteslotsK.Select(s => (s + 4)).ToList();
				var knightslots = whitetoblackslotsK.ToList();
				_spellPermissions.ClearPermissions(Classes.Knight);
				_spellPermissions.AddPermissionsRange(knightslots.Select(x => (Classes.Knight, x)).ToList());
				_classes[(int)Classes.Knight].SpCGrowth = KnightLvl4Charge;
				_classes[(int)Classes.Fighter].SpCGrowth = KnightLvl4Charge;
			}

			if (!(bool)flags.BlackKnightKeep)
			{

				_spellPermissions.ClearPermissions(Classes.Knight);
				var ninjaPer = _spellPermissions[Classes.Ninja].ToList();
				_spellPermissions.AddPermissionsRange(ninjaPer.Select(x => (Classes.Knight, x)).ToList());
				_classes[(int)Classes.Knight].SpCGrowth = KnightLvl4Charge;
				_classes[(int)Classes.Fighter].SpCGrowth = KnightLvl4Charge;

			}
		}

		public void WhiteNinja(Flags flags)
		{
			if (!(bool)flags.WhiteNinja)
			{
				return;

			}
			if (!(bool)flags.WhiteNinjaKeep)
			{

				_spellPermissions.ClearPermissions(Classes.Ninja);
				var knighPer = _spellPermissions[Classes.Knight].ToList();
				_spellPermissions.AddPermissionsRange(knighPer.Select(x => (Classes.Ninja, x)).ToList());

			}

			if ((bool)flags.WhiteNinjaKeep)
			{

				var blackNinja = _spellPermissions[Classes.Ninja].Where(s => ((int)s % 8) >= 4).ToList();
				var whitetoblackN = blackNinja.Select(s => (s - 4)).ToList();
				_spellPermissions.ClearPermissions(Classes.Ninja);
				_spellPermissions.AddPermissionsRange(whitetoblackN.Select(x => (Classes.Ninja, x)).ToList());
			}

		}
		public void Knightlvl4(Flags flags, MT19337 rng)
		{
			if (!(bool)flags.Knightlvl4)
			{
				return;

			}

			if (!(bool)flags.BlackKnight)
			{
				var kilvl4 = rng.Between(0, 3);

				if (kilvl4 == 0)
				{
					_spellPermissions.AddPermission(Classes.Knight, SpellSlots.White4Slot1);
				}

				if (kilvl4 == 1)
				{
					_spellPermissions.AddPermission(Classes.Knight, SpellSlots.White4Slot2);
				}

				if (kilvl4 == 2)
				{
					_spellPermissions.AddPermission(Classes.Knight, SpellSlots.White4Slot3);
				}

				if (kilvl4 == 3)
				{
					_spellPermissions.AddPermission(Classes.Knight, SpellSlots.White4Slot4);
				}
				_classes[(int)Classes.Knight].SpCGrowth = KnightLvl4Charge;
				_classes[(int)Classes.Fighter].SpCGrowth = KnightLvl4Charge;
			}
			if ((bool)flags.BlackKnight && (bool)flags.BlackKnightKeep)

			{
				var kilvl4B = rng.Between(0, 3);

				if (kilvl4B == 0)
				{
					_spellPermissions.AddPermission(Classes.Knight, SpellSlots.Black4Slot1);
				}

				if (kilvl4B == 1)
				{
					_spellPermissions.AddPermission(Classes.Knight, SpellSlots.Black4Slot2);
				}

				if (kilvl4B == 2)
				{
					_spellPermissions.AddPermission(Classes.Knight, SpellSlots.Black4Slot3);
				}

				if (kilvl4B == 3)
				{
					_spellPermissions.AddPermission(Classes.Knight, SpellSlots.Black4Slot4);
				}
			}

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
			if (flags.RandomizeClassMode == ClassRandomizationMode.None)
				return;

			RandomizeClassHacks(flags, rom);

			if (flags.RandomizeClassMode == ClassRandomizationMode.Transmooglifier)
				Transmooglify(flags, rng, rom);
			else if (flags.RandomizeClassMode == ClassRandomizationMode.Blursings)
				RandomizeClassBlursings(flags, rng, olditemnames, itemnames, rom);
			else if (flags.RandomizeClassMode == ClassRandomizationMode.Chaos)
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
			if (flags.RandomizeClassMode == ClassRandomizationMode.Transmooglifier)
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
			} else {
				// We just reproduce EnablePoolParty()'s new DoPartyGen_OnCharacter and add the select button
				rom.PutInBank(0x1E, 0x8843, Blob.FromHex("A98548A9AF48"));
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
		public void ProcessStartWithRoutines(Flags flags, List<int> blursesValues, FF1Rom rom)
		{
			if (!spellsLearning.Any())
			{
				BuildSpellIdDict(rom);
			}

			// See 1B_B000_StartWithRoutines.asm
			// Utilities
			rom.PutInBank(0x1B, 0xB000, Blob.FromHex("B90061C90C9002A90C8410A8B1EDC90108A4102860B90061C90C9002A90C8410A8B1ED08A4102860AAA5828511A58318690285128AA000D111F007C8C02090F718603860"));

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
			Blob lut_MpStart = _classes.Select(x => (byte)(x.ToggleBlursings.Contains(BonusMalusAction.StartWithMp) ? 0x01 : 0x00)).ToArray();
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
			rom.PutInBank(0x1B, 0xB080, Blob.FromHex($"A908C5F2F0034C00B320C3B020F8B020AEB1202FB12060B12000B7207AB72092B72000B8201FB82045B8206BB82099B84CB3B0A90085EDA9B285EEA000B1822003B060A90D85EDA9B285EEA000B1822003B0D023A018B1823011C8B182300CC8B1823007C8B1823002A900297FC923D006A00FA9{catclawcrit:X2}918060A91A85EDA9B285EEA000B1822003B0D025A018B1823011C8B182300CC8B1823007C8B1823002A900297FC924D008A009B180180A918060A93485EDA9B285EEA000B1822003B0D01FA01CB1821003204FB1C8C020D0F460297FC905D00A9848A00BA902918068A860A9A985EDA9B285EE8A48A200A000B1822003B0D036A01CB18210032097B1C8C020D0F4E003D02418A007B18069B49002A9FF918068AA60297FC902D002E860C91BD002E860C911D001E86068AA60A000B182AABD27B2F006A00D1180918060"));


			rom.PutInBank(0x1B, 0xB700, Blob.FromHex($"A000841084118412B182AABD41B2F069A018B182101E297FAAE002F010E00AF00CE010F008E023F004E026D027E610E6114C54B7297FAAE002F017E00AF013E010F00FE023F004E026D009E611E6124C54B7E611C8C01CD0B9A510F01CA511C9029016A512F005A9044C6EB7A9028510A00CB180186510918060A000B182AABD4EB2F00DA9{spellsLearning[Spell.LAMP].Id:X2}2028B09006A00AA9FF918060A0008410B182AABD5BB2F061A9{spellsLearning[Spell.AFIR].Id:X2}2028B09006A51009108510A9{spellsLearning[Spell.AICE].Id:X2}2028B09006A51009208510A9{spellsLearning[Spell.ALIT].Id:X2}2028B09006A51009408510A9{spellsLearning[Spell.ARUB].Id:X2}2028B09006A51009898510A9{spellsLearning[Spell.AMUT].Id:X2}2028B09006A51009018510AD0A6805108D0A68AD1C6805108D1C68AD2E6805108D2E68AD406805108D406860A000B182AABD68B2F014A9{spellsLearning[Spell.DARK].Id:X2}2028B0900D18A007B18069509002A9FF918060A000B182AABD75B2F01BA9{spellsLearning[Spell.SLOW].Id:X2}2028B0B007A9{spellsLearning[Spell.SLO2].Id:X2}2028B0900D18A008B18069289002A9FF918060A000B182AABD82B2F01BA9{spellsLearning[Spell.SLEP].Id:X2}2028B0B007A9{spellsLearning[Spell.SLP2].Id:X2}2028B0900D18A006B18069509002A9FF918060A000B182AABD8FB2F0238510A01CB18210032086B8C8C020D0F460297FC920D00C9848A001B1820510918268A860A000B182AABD9CB2F01F8510A018B18210034CB4B8C8C01CD0F460297FC928D008A001B1820510918260"));

			// Insert luts
			Blob lut_Blackbelts = _classes.Select(x => (byte)(x.UnarmedAttack ? 0x01 : 0x00)).ToArray();
			Blob lut_CatClaws = _classes.Select(x => (byte)(x.ToggleBlursings.Contains(BonusMalusAction.CatClawMaster) ? 0x01 : 0x00)).ToArray();
			Blob lut_ThorHammer = _classes.Select(x => (byte)(x.ToggleBlursings.Contains(BonusMalusAction.ThorMaster) ? 0x01 : 0x00)).ToArray();
			Blob lut_Hunter = _classes.Select(x => (byte)x.HurtType).ToArray();
			Blob lut_SteelArmor = _classes.Select(x => (byte)(x.ToggleBlursings.Contains(BonusMalusAction.SteelLord) ? 0x01 : 0x00)).ToArray();
			Blob lut_KnifeDualWield = _classes.Select(x => (byte)(x.ToggleBlursings.Contains(BonusMalusAction.DualWieldKnife) ? 0x01 : 0x00)).ToArray();
			Blob lut_LampResist = _classes.Select(x => (byte)(x.ToggleBlursings.Contains(BonusMalusAction.LearnLampRibbon) ? 0x01 : 0x00)).ToArray();
			Blob lut_ASpellAutoCast = _classes.Select(x => (byte)(x.ToggleBlursings.Contains(BonusMalusAction.ASpellsAutocast) ? 0x01 : 0x00)).ToArray();
			Blob lut_DarkEvade = _classes.Select(x => (byte)(x.ToggleBlursings.Contains(BonusMalusAction.LearDarkEvade) ? 0x01 : 0x00)).ToArray();
			Blob lut_SlowAbsorb = _classes.Select(x => (byte)(x.ToggleBlursings.Contains(BonusMalusAction.LearnSlowAbsorb) ? 0x01 : 0x00)).ToArray();
			Blob lut_SleepMDef = _classes.Select(x => (byte)(x.ToggleBlursings.Contains(BonusMalusAction.LearnSleepMDef) ? 0x01 : 0x00)).ToArray();
			Blob lut_RibbonCurse = _classes.Select(x => (byte)x.RibbonCurse).ToArray();
			Blob lut_MasaCurse = _classes.Select(x => (byte)x.MasaCurse).ToArray();
			Blob lut_WoodArmors = _classes.Select(x => (byte)(x.ToggleBlursings.Contains(BonusMalusAction.WoodAdept) ? 0x01 : 0x00)).ToArray();

			rom.PutInBank(0x1B, 0xB200, lut_Blackbelts + new byte[] { 0x00 } +
				lut_CatClaws + new byte[] { 0x00 } +
				lut_ThorHammer + new byte[] { 0x00 } +
				lut_Hunter + new byte[] { 0x00 } +
				lut_SteelArmor + new byte[] { 0x00 } +
				lut_KnifeDualWield + new byte[] { 0x00 } +
				lut_LampResist + new byte[] { 0x00 } +
				lut_ASpellAutoCast + new byte[] { 0x00 } +
				lut_DarkEvade + new byte[] { 0x00 } +
				lut_SlowAbsorb + new byte[] { 0x00 } +
				lut_SleepMDef + new byte[] { 0x00 } +
				lut_RibbonCurse + new byte[] { 0x00 } +
				lut_MasaCurse + new byte[] { 0x00 } +
				lut_WoodArmors + new byte[] { 0x00 });

			// Recruit Mode Switcher
			rom.PutInBank(0x1B, 0xB600, Blob.FromHex("A5108513A8B913B6A8202EB3A51385104CAA87004080C0"));
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
		public List<BonusMalusAction> ToggleBlursings { get; set; }
		public bool UnarmedAttack { get; set; }
		public byte HurtType { get; set; }
		public byte MasaCurse { get; set; }
		public byte RibbonCurse { get; set; }
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
			UnarmedAttack = (classid == 2 || classid == 8);
			HurtType = 0x00;
			MasaCurse = 0x00;
			RibbonCurse = 0x00;
			StartingKeyItem = Item.None;
			InnateSpells = new() { new SpellSlotInfo(), new SpellSlotInfo(), new SpellSlotInfo() };
			ToggleBlursings = new();
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
}
