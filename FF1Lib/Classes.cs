using RomUtilities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

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
			BonusXp = 34
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
			public BonusMalus(BonusMalusAction action, string description, int mod = 0, int mod2 = 0, List<Item> equipment = null, List<bool> binarylist = null, List<SpellSlots> spelllist = null, List<byte> bytelist = null, List<Classes> Classes = null)
			{
				Action = action;
				Description = description;
				StatMod = mod;
				StatMod2 = mod2;
				Equipment = equipment;
				SpellList = spelllist;
				StatGrowth = binarylist;
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
			var maxChargeList = rom.Get(lut_MaxMP, 12);

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

		public void Randomize(Flags flags, MT19337 rng, List<string> olditemnames, ItemNames itemnames, FF1Rom rom)
		{
			if (!(bool)flags.RandomizeClass)
			{
				return;
			}

			// Strings to build info screen in game
			List<string> rankString = new List<string> { "-", "E", "D", "C", "B", "A", "S" };
			List<string> symboleString = new List<string> { "@S", "@H", "@K", "@X", "@F", "@N", "@A", "@s", "@h", "@G", "HP", "Str", "Agi", "Int", "Vit", "Lck", "Ht%", "MDf", "Wt", "Bk", "Sp" };

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
			rom.PutInBank(0x1E, 0x8800, Blob.FromHex("203CC4A5674A4A4A4A4AB015A200205B83A220205B83A9118538A90285394C3388A210205B83A230205B83A9038538A9028539A667BD0003C9FFD002A90C0AAA207188A98048A9C0484C1A85A9118538A9028539A667BD0061C9FFD002A90C0AAA207188A9B648A91248A90E85574C03FEA90D853CA91A853DA9008D0120A90085378A482063E0A970853EA989853FA538853AE63AA539853BE63BA91E855785582036DE68AABD5089853EBD5189853FA53B186902853B2036DE205E8560"));

			// StatusWaitForBtn_SFX, see 1E_8800_DrawInfoBox.asm
			rom.PutInBank(0x1E, 0x8910, Blob.FromHex("202C85A5240522D00FA525F0F3A9008525A90E85574C03FEA9008524852585224C4C88"));

			// InfoScreen in PtyGen and Status screen
			// DoPartyGen_OnCharacter change to check for Select button, see 1E_8800_DrawInfoBox.asm
			if (flags.EnablePoolParty ?? false)
			{
				// We just reproduce EnablePoolParty()'s new DoPartyGen_OnCharacter and add the select button
				rom.PutInBank(0x1E, 0x85B0, Blob.FromHex("A667BD01030D41038D4103A9FF8D4003BD0103C900F00718EE40032A90FA20A480A9008522200F82A522F0034C0088A667AC4003A524F013BD0003C9FFF009BD01034D41038D41034C2C81A525F0118AC900F00AA9009D0103A9FF9D00033860A520290FC561F0B98561C900F0B3C898C9099002A0008C4003B944862C4103F0ED9D0103B942039D0003A901853720B0824CD1858040201008040201"));
				rom.PutInBank(0x1E, 0x8843, Blob.FromHex("A98548A9AF48"));
			}
			else
			{
				var partypermissions = rom.Get(0x78110, 0x11);
				rom.PutInBank(0x1E, 0x80C1, Blob.FromHex("A6678A4A4A4A4AA8B9B085859020A480A9008522200F82A522F0034C0088A524D049A525F0023860A520290FC561F0E08561C900F0DAA667BD0003186901C90CD002A9FF9D0003A8C8B9B4852490F0E8A901853720B0824CD180"));
				rom.PutInBank(0x1E, 0x85B0, partypermissions);
			}


			List<string> bonusmalusDescription = new List<string>();

			// Chaos Mode enabled?
			if ((bool)flags.RandomizeClassChaos)
			{
				DoRandomizeClassChaosMode(((bool)flags.MagicLevelsMixed && (bool)flags.MagicPermissions) || ((bool)flags.SpellcrafterMixSpells && !(bool)flags.SpellcrafterRetainPermissions), (flags.ThiefAgilityBuff != ThiefAGI.Vanilla), rng, rom);
			}
			else
			{ 
				bonusmalusDescription = DoRandomizeClassNormalMode(rng, olditemnames, itemnames, flags, rom);
			}

			// dataScreen
			int totalByte = 0;
			var templateScreen = "";
			var screenBlob = Blob.FromHex("00");
			var dataScreen = new List<string>();

			// Generate template
			if ((bool)flags.RandomizeClassChaos)
			{
				templateScreen =
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
			}
			else
				templateScreen = "BONUS";

			// Insert template
			screenBlob = FF1Text.TextToBytes(templateScreen, true, FF1Text.Delimiter.Null);
			rom.PutInBank(0x1E, 0x8970, screenBlob);
			totalByte += screenBlob.Length;

			// Build individual dataScreens, calculate their pointers and insert them
			if ((bool)flags.RandomizeClassChaos)
			{
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
			}
			else
			{
				dataScreen.AddRange(bonusmalusDescription.Concat(bonusmalusDescription));
			}

			// Insert class data screen
			for (int i = 0; i < 12; i++)
			{
				var tempBlob = FF1Text.TextToBytes(dataScreen[i], true, FF1Text.Delimiter.Null);
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
			List<Item> equipRedMageArmor = _armorPermissions[Classes.RedMage].ToList().Where(x => !bannableArmor.Contains(x)).ToList(); ;
			List<Item> equipFighterWeapon = _weaponPermissions[Classes.Fighter].ToList();
			List<Item> equipThiefWeapon = _weaponPermissions[Classes.Thief].ToList();

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
				new BonusMalus(BonusMalusAction.StrMod, "+15 Str.", mod: 15),
				new BonusMalus(BonusMalusAction.StrMod, "+20 Str.", mod: 20),
				new BonusMalus(BonusMalusAction.AgiMod, "+10 Agi.", mod: 10),
				new BonusMalus(BonusMalusAction.AgiMod, "+15 Agi.", mod: 15),
				new BonusMalus(BonusMalusAction.AgiMod, "+20 Agi.", mod: 20),
				new BonusMalus(BonusMalusAction.VitMod, "+10 Vit.", mod: 10),
				new BonusMalus(BonusMalusAction.VitMod, "+15 Vit.", mod: 15),
				new BonusMalus(BonusMalusAction.VitMod, "+20 Vit.", mod: 20),
				new BonusMalus(BonusMalusAction.LckMod, "+5 Luck", mod: 5),
				new BonusMalus(BonusMalusAction.LckMod, "+7 Luck", mod: 7),
				new BonusMalus(BonusMalusAction.LckMod, "+10 Luck", mod: 10),
				new BonusMalus(BonusMalusAction.HpMod, "+20 HP", mod: 20),
				new BonusMalus(BonusMalusAction.HpMod, "+30 HP", mod: 30),
				new BonusMalus(BonusMalusAction.HpMod, "+40 HP", mod: 40),
				new BonusMalus(BonusMalusAction.HitMod, "+10 Hit%", mod: 10, Classes: hitBonusClass ),
				new BonusMalus(BonusMalusAction.HitMod, "+15 Hit%", mod: 15, Classes: hitBonusClass ),
				new BonusMalus(BonusMalusAction.HitMod, "+20 Hit%", mod: 20, Classes: hitBonusClass ),
				new BonusMalus(BonusMalusAction.MDefMod, "+10 MDef", mod: 10),
				new BonusMalus(BonusMalusAction.MDefMod, "+15 MDef", mod: 15),
				new BonusMalus(BonusMalusAction.MDefMod, "+20 MDef", mod: 20),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+" + olditemnames[(int)Item.ThorHammer], equipment: new List<Item> {  Item.ThorHammer }),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+" + olditemnames[(int)Item.Vorpal], equipment: new List<Item> {  Item.Vorpal }),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+" + olditemnames[(int)Item.Defense], equipment: new List<Item> {  Item.Defense }),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+" + olditemnames[(int)Item.Katana], equipment: new List<Item> {  Item.Katana }),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+" + olditemnames[(int)Item.Xcalber], equipment: new List<Item> {  Item.Xcalber }),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+" + olditemnames[(int)Item.WhiteShirt], equipment: new List<Item> {  Item.WhiteShirt }),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+" + olditemnames[(int)Item.BlackShirt], equipment: new List<Item> {  Item.BlackShirt }),
				new BonusMalus(BonusMalusAction.StrGrowth, "Fightr Str.", binarylist: _classes[(int)Classes.Fighter].StrGrowth, Classes: new List<Classes> { Classes.Thief, Classes.BlackBelt, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }),
				new BonusMalus(BonusMalusAction.LckGrowth, "Thief Luck", binarylist: _classes[(int)Classes.Thief].LckGrowth, Classes: new List<Classes> { Classes.Fighter, Classes.BlackBelt, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }),
				new BonusMalus(BonusMalusAction.VitGrowth, "B.Belt Vit.", binarylist: _classes[(int)Classes.BlackBelt].VitGrowth, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+Thief @S", equipment: equipThiefWeapon, Classes: new List<Classes> { Classes.BlackBelt, Classes.WhiteMage, Classes.BlackMage } ),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Red Mage @A", equipment: equipRedMageArmor, Classes: new List<Classes> { Classes.Thief, Classes.BlackBelt, Classes.WhiteMage, Classes.BlackMage } ),
				new BonusMalus(BonusMalusAction.SpcMod, "+2 Lv1 MP", mod: 2, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }),
				new BonusMalus(BonusMalusAction.SpcMod, "+2 Lv1 MP", mod: 2, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }),
			};

			// Strong Bonuses List
			var bonusStrong = new List<BonusMalus> {
				new BonusMalus(BonusMalusAction.StrMod, "+40 Str.", mod: 40),
				new BonusMalus(BonusMalusAction.AgiMod, "+40 Agi.", mod: 40),
				new BonusMalus(BonusMalusAction.VitMod, "+40 Vit.", mod: 40),
				new BonusMalus(BonusMalusAction.LckMod, "+15 Luck", mod: 15),
				new BonusMalus(BonusMalusAction.HpMod, "+80 HP", mod: 80),
				new BonusMalus(BonusMalusAction.MDefGrowth, "+2 MDef/Lv", mod: 2, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+Fighter @S", equipment: equipFighterWeapon, Classes: new List<Classes> { Classes.Thief, Classes.BlackBelt, Classes.WhiteMage, Classes.BlackMage } ),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Fighter @A", equipment: equipFighterArmor, Classes: new List<Classes> { Classes.Thief, Classes.BlackBelt, Classes.WhiteMage, Classes.BlackMage, Classes.RedMage } ),
				new BonusMalus(BonusMalusAction.SpcGrowth, "Improved MP", bytelist: improvedMPlist, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage } ),
				new BonusMalus(BonusMalusAction.PowerRW, "Sage Class", spelllist: wmWhiteSpells.Concat(bmBlackSpells).Concat(wwWhiteSpells).Concat(bwBlackSpells).ToList(), Classes: new List<Classes> { Classes.RedMage }),
				new BonusMalus(BonusMalusAction.WhiteSpellcaster, "White W. Sp", spelllist: wwWhiteSpells, Classes: new List<Classes> { Classes.WhiteMage }),
				new BonusMalus(BonusMalusAction.BlackSpellcaster, "Black W. Sp", spelllist: bwBlackSpells, Classes: new List<Classes> { Classes.BlackMage }),
			};

			// Maluses List
			var malusNormal = new List<BonusMalus> {
				new BonusMalus(BonusMalusAction.StrMod, "-10 Str.", mod: -10),
				new BonusMalus(BonusMalusAction.StrMod, "-10 Str.", mod: -10),
				new BonusMalus(BonusMalusAction.StrMod, "-15 Str.", mod: -15),
				new BonusMalus(BonusMalusAction.StrGrowth, "BlackM Str.", binarylist: _classes[(int)Classes.BlackMage].StrGrowth, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt, Classes.RedMage }),
				new BonusMalus(BonusMalusAction.AgiMod, "-10 Agi.", mod: -10),
				new BonusMalus(BonusMalusAction.AgiMod, "-10 Agi.", mod: -10),
				new BonusMalus(BonusMalusAction.AgiMod, "-15 Agi.", mod: -15),
				new BonusMalus(BonusMalusAction.AgiGrowth, "BlackM Agi.", binarylist: _classes[(int)Classes.BlackMage].AgiGrowth, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt }),
				new BonusMalus(BonusMalusAction.VitMod, "-10 Vit.", mod: -10),
				new BonusMalus(BonusMalusAction.VitMod, "-10 Vit.", mod: -10),
				new BonusMalus(BonusMalusAction.VitMod, "-15 Vit.", mod: -15),
				new BonusMalus(BonusMalusAction.VitGrowth, "BlackM Vit.", binarylist: _classes[(int)Classes.BlackMage].VitGrowth, Classes: new List<Classes> { Classes.Fighter, Classes.BlackBelt, Classes.RedMage }),
				new BonusMalus(BonusMalusAction.LckMod, "-5 Luck", mod: -5),
				new BonusMalus(BonusMalusAction.LckMod, "-5 Luck", mod: -5),
				new BonusMalus(BonusMalusAction.LckMod, "-10 Luck", mod: -10),
				new BonusMalus(BonusMalusAction.HpMod, "-20 HP", mod: -20),
				new BonusMalus(BonusMalusAction.HpMod, "-20 HP", mod: -20),
				new BonusMalus(BonusMalusAction.HpMod, "-30 HP", mod: -30),
				new BonusMalus(BonusMalusAction.HpGrowth, "BlackM HP", binarylist: _classes[(int)Classes.BlackMage].HpGrowth, Classes: new List<Classes> { Classes.Fighter }),
				new BonusMalus(BonusMalusAction.HitMod, "-10 Hit%", mod: -10),
				new BonusMalus(BonusMalusAction.MDefMod, "-10 MDef", mod: -10),
				new BonusMalus(BonusMalusAction.HitGrowth, "-1 Hit%/Lv", mod: -1),
				new BonusMalus(BonusMalusAction.MDefGrowth, "-1 MDef/Lv", mod: -1),
				new BonusMalus(BonusMalusAction.ArmorRemove, "-" + olditemnames[(int)Item.Ribbon], equipment: new List<Item> { Item.Ribbon }),
				new BonusMalus(BonusMalusAction.ArmorRemove, "No @B", equipment: braceletList),
				new BonusMalus(BonusMalusAction.WeaponReplace, "Thief @S", equipment: equipThiefWeapon, Classes: new List<Classes> { Classes.Fighter, Classes.RedMage } ),
				new BonusMalus(BonusMalusAction.SpcMax, "-4 Max MP", mod: -4, Classes: new List<Classes> {  Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }),
				new BonusMalus(BonusMalusAction.NoPromoMagic, "No Promo Sp", mod: 0, mod2: 0, binarylist: nullSpells, Classes: new List<Classes> { Classes.Fighter, Classes.Thief }),
			};

			if (!(bool)flags.ArmorCrafter)
			{
				malusNormal.Add(new BonusMalus(BonusMalusAction.ArmorRemove, "-" + olditemnames[(int)Item.ProRing], equipment: new List<Item> { Item.ProRing }));
			}

			if (Rng.Between(rng, 0, 10) == 0)
				malusNormal.Add(new BonusMalus(BonusMalusAction.IntMod, "+80 Int.", mod: 80));

			// Add Spellcasting Bonuses
			if ((bool)flags.RandomizeClassCasting)
			{
				bonusNormal.Add(new BonusMalus(BonusMalusAction.WhiteSpellcaster, "L1 White Sp", mod: 2, mod2: 0, spelllist: lv1WhiteSpells, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt, Classes.RedMage, Classes.BlackMage }));
				bonusNormal.Add(new BonusMalus(BonusMalusAction.BlackSpellcaster, "L1 Black Sp", mod: 2, mod2: 0, spelllist: lv1BlackSpells, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt, Classes.WhiteMage }));
				bonusNormal.Add(new BonusMalus(BonusMalusAction.WhiteSpellcaster, "Knight Sp", mod: 2, mod2: _classes[6].MaxSpC, spelllist: lv3WhiteSpells, bytelist: exKnightMPlist, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt, Classes.BlackMage }));
				bonusNormal.Add(new BonusMalus(BonusMalusAction.BlackSpellcaster, "Ninja Sp", mod: 2, mod2: _classes[7].MaxSpC, spelllist: lv4BlackSpells, bytelist: exNinjaMPlist, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt, Classes.WhiteMage }));
				bonusStrong.Add(new BonusMalus(BonusMalusAction.WhiteSpellcaster, "White M. Sp", mod: 2, mod2: _classes[4].MaxSpC, spelllist: wmWhiteSpells, bytelist: rmMPlist, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt, Classes.RedMage, Classes.BlackMage }));
				bonusStrong.Add(new BonusMalus(BonusMalusAction.BlackSpellcaster, "Black M. Sp", mod: 2, mod2: _classes[5].MaxSpC, spelllist: bmBlackSpells, bytelist: rmMPlist, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt, Classes.WhiteMage }));
			}

			// Add Lockpicking Bonus/Malus
			if ((bool)flags.Lockpicking && flags.LockpickingLevelRequirement < 50)
			{
				malusNormal.Add(new BonusMalus(BonusMalusAction.LockpickingLevel, "+10 Lp Lv", mod: 10, Classes: new List<Classes> { Classes.Thief }));
			}

			if ((bool)flags.Lockpicking && flags.LockpickingLevelRequirement > 1)
			{
				bonusNormal.Add(new BonusMalus(BonusMalusAction.LockpickingLevel, "-10 Lp Lv", mod: -10, Classes: new List<Classes> { Classes.Thief }));
			}

			// Add Natural Resist Bonuses
			if ((bool)flags.RandomizeClassIncludeNaturalResist)
			{
				bonusStrong.Add(new BonusMalus(BonusMalusAction.InnateResist, "Res All", mod: 0xFF, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.RedMage, Classes.BlackMage, Classes.WhiteMage }));
				bonusStrong.Add(new BonusMalus(BonusMalusAction.InnateResist, "Res PEDTS", mod: (int)(Element.POISON | Element.EARTH | Element.DEATH | Element.TIME | Element.STATUS), Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.RedMage, Classes.BlackMage, Classes.WhiteMage }));

				bonusNormal.Add(CreateRandomResistBonusMalus(rng));
				bonusNormal.Add(CreateRandomResistBonusMalus(rng));
			}

			if ((bool)flags.RandomizeClassIncludeXpBonus)
			{
				bonusStrong.Add(new BonusMalus(BonusMalusAction.BonusXp, "+50% XP", mod: 150, Classes: new List<Classes> { Classes.Fighter, Classes.BlackBelt }));
				bonusStrong.Add(new BonusMalus(BonusMalusAction.BonusXp, "+100% XP", mod: 200, Classes: new List<Classes> { Classes.Thief, Classes.RedMage, Classes.BlackMage, Classes.WhiteMage }));

				bonusNormal.Add(new BonusMalus(BonusMalusAction.BonusXp, "+50% XP", mod: 150, Classes: new List<Classes> { Classes.Thief, Classes.RedMage, Classes.BlackMage, Classes.WhiteMage }));
				bonusNormal.Add(new BonusMalus(BonusMalusAction.BonusXp, "+50% XP", mod: 150, Classes: new List<Classes> { Classes.Thief, Classes.RedMage, Classes.BlackMage, Classes.WhiteMage }));
			}

			var assignedBonusMalus = new List<List<BonusMalus>> { new List<BonusMalus>(), new List<BonusMalus>(), new List<BonusMalus>(), new List<BonusMalus>(), new List<BonusMalus>(), new List<BonusMalus>() };

			// Shuffle bonuses and maluses
			bonusNormal.Shuffle(rng);
			bonusStrong.Shuffle(rng);
			malusNormal.Shuffle(rng);

			// Select one incentivized class that will received a strong bonus
			int luckyDude = Rng.Between(rng, 0, 5);


			//Hand out the strong bonus first
			BonusMalus selectedStrongBonusMalus;

			if (flags.RandomizeClassMaxBonus > 0)
			{
				while (!bonusStrong.First().ClassList.Contains((Classes)luckyDude))
				{
					bonusStrong.Shuffle(rng);
				}

				selectedStrongBonusMalus = bonusStrong.First();
				assignedBonusMalus[luckyDude].Add(selectedStrongBonusMalus);
				bonusStrong.RemoveRange(0, 1);
			}
			var descriptionList = new List<string>();

			// Distribute bonuses and maluses, we go backward (from BM to Fi) so we have enough malus for BM
			int maxbonus = flags.RandomizeClassMaxBonus;
			int maxmalus = flags.RandomizeClassMaxMalus;
			for (int i = 5; i >= 0; i--)
			{
				var tempstring = new List<(int, string)>();
				if (i == luckyDude && assignedBonusMalus[luckyDude].Any()) tempstring.Add((0, assignedBonusMalus[luckyDude][0].Description));

				while (assignedBonusMalus[i].Count < maxbonus)
				{
					while (!bonusNormal.First().ClassList.Contains((Classes)i) || assignedBonusMalus[i].Where(x => x.Action == bonusNormal.First().Action).Any())
						bonusNormal.Shuffle(rng);

					var selectedNormalBonusMalus = bonusNormal.First();
					assignedBonusMalus[i].Add(selectedNormalBonusMalus);
					tempstring.Add((0, selectedNormalBonusMalus.Description));
					bonusNormal.RemoveRange(0, 1);
				}

				for (int j = 0; j < maxmalus; j++)
				{
					while (!malusNormal.First().ClassList.Contains((Classes)i) || assignedBonusMalus[i].Select(x => x.Action).ToList().Contains(malusNormal.First().Action))
						malusNormal.Shuffle(rng);

					assignedBonusMalus[i].Add(malusNormal.First());
					if (malusNormal.First().Action == BonusMalusAction.IntMod)
						tempstring.Add((0, malusNormal.First().Description));
					else
						tempstring.Add((1, malusNormal.First().Description));

					malusNormal.RemoveRange(0, 1);
				}

				descriptionList.Add(string.Join("\n\n", tempstring.Where(x => x.Item1 == 0).Select(x => x.Item2)) + "\n\n\nMALUS\n\n" + string.Join("\n\n", tempstring.Where(x => x.Item1 == 1).Select(x => x.Item2)));
			}

			// Reverse description list so it's not backward
			descriptionList.Reverse();
			// Apply bonuses and maluses to stats

			for (int i = 0; i < 6; i++)
			{
				// Reverse the list so that maluses are applied first and don't cancel out bonuses
				assignedBonusMalus[i].Reverse();

				// But put back Max Mp mod add the end so it doesn't get overwritten by spellcasting bonuses
				int spcMaxIndex = assignedBonusMalus[i].FindIndex(x => x.Action == BonusMalusAction.SpcMax);
				if (spcMaxIndex > -1)
				{
					BonusMalus tempSpcMax = assignedBonusMalus[i][spcMaxIndex];
					assignedBonusMalus[i].RemoveAt(spcMaxIndex);
					assignedBonusMalus[i].Add(tempSpcMax);
				}

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
							_armorPermissions.AddPermissionsRange(bonusmalus.Equipment.Select(x => ((Classes)i, x)).ToList());
							_armorPermissions.AddPermissionsRange(bonusmalus.Equipment.Select(x => ((Classes)(i + 6), x)).ToList());
							break;
						case BonusMalusAction.ArmorRemove:
							_armorPermissions.RemovePermissionsRange(bonusmalus.Equipment.Select(x => ((Classes)i, x)).ToList());
							_armorPermissions.RemovePermissionsRange(bonusmalus.Equipment.Select(x => ((Classes)(i + 6), x)).ToList());
							break;
						case BonusMalusAction.ArmorReplace:
							_armorPermissions[(Classes)i] = bonusmalus.Equipment;
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
							_spellPermissions[(Classes)i] = wmWhiteSpells.Concat(bmBlackSpells).ToList();
							_spellPermissions[(Classes)(i + 6)] = wwWhiteSpells.Concat(bwBlackSpells).ToList();
							break;
						case BonusMalusAction.NoPromoMagic:
							_spellPermissions.ClearPermissions((Classes)i);
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

				_classes[i + 6].Ranks[(int)RankedType.Black] = magicRanks[i + 6];

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
			string description = "Res ";
			List<Element> elements = Enum.GetValues(typeof(Element)).Cast<Element>().ToList();
			//3 picks but can get a none
			for (int picks = 0; picks < 3; picks++)
			{

				Element pickedElement = elements.SpliceRandom(rng);
				switch (pickedElement)
				{
					case Element.STATUS:
						description += "S";
						break;
					case Element.POISON:
						description += "P";
						break;
					case Element.TIME:
						description += "T";
						break;
					case Element.DEATH:
						description += "D";
						break;
					case Element.FIRE:
						description += "F";
						break;
					case Element.ICE:
						description += "I";
						break;
					case Element.LIGHTNING:
						description += "L";
						break;
					case Element.EARTH:
						description += "E";
						break;
				}

				innateResistValue |= (byte)pickedElement;
			}

			return new BonusMalus(BonusMalusAction.InnateResist, description, mod: innateResistValue);
		}

		private void SillyStuff(FF1Rom rom)
		{
			rom.PutInBank(0x15, 0x8000, Blob.FromHex("0885E96885EA686818A5EB690385EB9002E6ECA5EA48A5EC48A5EB48A9D748A9F548A5E960B90061C90C9002A90CA8B1EDC90160"));
			rom.PutInBank(0x15, 0x8200, Blob.FromHex("A91085EDA98285EEA4822025804C008000000100000000000100000000"));
			rom.PutInBank(0x0C, 0xADBB, rom.CreateLongJumpTableEntry(0x15, 0x8200) + Blob.FromHex("28EAEAEA"));

			rom.PutInBank(0x15, 0x8400, Blob.FromHex($"A9{rom[FF1Rom.OverworldThreatLevelOffset]:X2}A642E004D002A9{rom[FF1Rom.OceanThreatLevelOffset]:X2}489848A92C85EDA98485EEA000202580D00A68A868AACACA8A4C008068A8684C008000010000000000010000000000"));
			rom.PutInBank(0x1F, 0xC4FD, rom.CreateLongJumpTableEntry(0x15, 0x8400) + Blob.FromHex("28EAEAEA"));
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
			MagicRanks = new List<string> { "- ", "- ", "- " };
			Ranks = Enumerable.Repeat((Rank)0, Enum.GetNames(typeof(RankedType)).Length).ToList();
			InnateResist = 0;
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
