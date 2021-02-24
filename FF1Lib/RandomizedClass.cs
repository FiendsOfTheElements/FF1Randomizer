using RomUtilities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace FF1Lib
{
	public partial class FF1Rom
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
		}
		public enum AuthClass
		{
			Fighter = 0,
			Thief = 1,
			BlackBelt = 2,
			RedMage = 3,
			WhiteMage = 4,
			BlackMage = 5,
		}
		public class BonusMalus
		{
			public List<Item> Equipment { get; set; }
			public List<bool> SpellList { get; set; }
			public List<bool> StatGrowth { get; set; }
			public int StatMod { get; set; }
			public int StatMod2 { get; set; }
			public RankedType TargetStat { get; set; }
			public BonusMalusAction Action { get; set; }
			public string Description { get; set; }
			public List<byte> SpcGrowth { get; set; }
			public List<AuthClass> ClassList { get; set; }
			public BonusMalus(BonusMalusAction action, string description, int mod = 0, int mod2 = 0, List<Item> equipment = null, List<bool> binarylist = null, List<byte> bytelist = null, List<AuthClass> authclass = null)
			{
				Action = action;
				Description = description;
				StatMod = mod;
				StatMod2 = mod2;
				Equipment = equipment;
				SpellList = binarylist;
				StatGrowth = binarylist;
				if (bytelist == null)
				{
					SpcGrowth = Enumerable.Repeat((byte)0x00, 49).ToList();
				}
				else
				{
					SpcGrowth = bytelist;
				}

				if (authclass == null)
				{
					ClassList = new List<AuthClass> { AuthClass.Fighter, AuthClass.Thief, AuthClass.BlackBelt, AuthClass.RedMage, AuthClass.WhiteMage, AuthClass.BlackMage };
				}
				else
				{
					ClassList = authclass;
				}
			}
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

		public class ClassData
		{
			// Indivdual class stats container
			public List<Item> WeaponPermissions { get; set; }
			public List<Item> ArmourPermissions { get; set; }
			public List<bool> WhitePermissions { get; set; }
			public List<bool> BlackPermissions { get; set; }
			public byte EquipmentBit { get; set; }
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
			public ClassData()
			{
				Promoted = false;
				ClassID = 0;
				HpStarting = 1;
				StrStarting = 0;
				AgiStarting = 0;
				IntStarting = 0;
				VitStarting = 0;
				LckStarting = 0;
				DmgStarting = 0;
				HitStarting = 0;
				EvaStarting = 0;
				MDefStarting = 0;
				SpCStarting = 0;
				HitGrowth = 0;
				MDefGrowth = 0;
				MaxSpC = 0;
				HpGrowth = Enumerable.Repeat(false, 49).ToList();
				StrGrowth = Enumerable.Repeat(false, 49).ToList();
				AgiGrowth = Enumerable.Repeat(false, 49).ToList();
				IntGrowth = Enumerable.Repeat(false, 49).ToList();
				VitGrowth = Enumerable.Repeat(false, 49).ToList();
				LckGrowth = Enumerable.Repeat(false, 49).ToList();
				SpCGrowth = Enumerable.Repeat((byte)0x00, 49).ToList();
				WhitePermissions = Enumerable.Repeat(false, 4 * 8).ToList();
				BlackPermissions = Enumerable.Repeat(false, 4 * 8).ToList();
				ArmourPermissions = new List<Item>();
				WeaponPermissions = new List<Item>();
				MagicRanks = new List<string> { "- ", "- ", "- " };
				Ranks = Enumerable.Repeat((Rank)0, Enum.GetNames(typeof(RankedType)).Length).ToList();
			}

			public byte[] StartingStatsArray()
			{
				List<byte> startingStatsArray = new() { ClassID, HpStarting, StrStarting, AgiStarting, IntStarting, VitStarting, LckStarting, DmgStarting, HitStarting, EvaStarting, MDefStarting, SpCStarting, 0x00, 0x00, 0x00, 0x00 };
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
				List<byte> levelUp = new();

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
				for (int j = 0; j < 49; j++)
				{
					HpGrowth[j] = (levelUpStats[j * 2] & 0x20) != 0;
					StrGrowth[j] = (levelUpStats[j * 2] & 0x10) != 0;
					AgiGrowth[j] = (levelUpStats[j * 2] & 0x08) != 0;
					IntGrowth[j] = (levelUpStats[j * 2] & 0x04) != 0;
					VitGrowth[j] = (levelUpStats[j * 2] & 0x02) != 0;
					LckGrowth[j] = (levelUpStats[j * 2] & 0x01) != 0;
					SpCGrowth[j] = levelUpStats[(j * 2) + 1];
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
			public void GetMagicPermissions(byte[] magicPermissions)
			{

				List<byte> whiteArray = new() { 0x80, 0x40, 0x20, 0x10 };
				List<byte> blackArray = new() { 0x08, 0x04, 0x02, 0x01 };

				for (int i = 0; i < 8; i++)
				{
					for (int j = 0; j < 4; j++)
					{
						WhitePermissions[(i * 4) + j] = (~magicPermissions[i] & whiteArray[j]) > 0;
						BlackPermissions[(i * 4) + j] = (~magicPermissions[i] & blackArray[j]) > 0;
					}
				}
			}

			public byte[] MagicPermissions()
			{
				List<byte> whiteArray = new() { 0x80, 0x40, 0x20, 0x10 };
				List<byte> blackArray = new() { 0x08, 0x04, 0x02, 0x01 };
				List<byte> magicpermissions = new();

				for (int i = 0; i < 8; i++)
				{
					int tempPermission = 0x00;
					for (int j = 0; j < 4; j++)
					{
						if (WhitePermissions[(i * 4) + j])
						{
							tempPermission |= whiteArray[j];
						}

						if (BlackPermissions[(i * 4) + j])
						{
							tempPermission |= blackArray[j];
						}
					}
					magicpermissions.Add((byte)~tempPermission);
				}

				return magicpermissions.ToArray();
			}

		}
		public void RandomizeClass(MT19337 rng, Flags flags, string[] itemnames)
		{
			List<ClassData> classData = new()
			{
				new ClassData(),
				new ClassData(),
				new ClassData(),
				new ClassData(),
				new ClassData(),
				new ClassData(),
				new ClassData(),
				new ClassData(),
				new ClassData(),
				new ClassData(),
				new ClassData(),
				new ClassData()
			};

			// Permission bit for each class
			List<ushort> equipmentPermissionBit = new() { 0x800, 0x400, 0x200, 0x100, 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };

			// New equipement permissions list
			List<ushort> wpNewPermissions = Enumerable.Repeat((ushort)0xF000, 40).ToList();
			List<ushort> arNewPermissions = Enumerable.Repeat((ushort)0xF000, 40).ToList();

			// Strings to build info screen in game
			List<string> rankString = new() { "-", "E", "D", "C", "B", "A", "S" };
			List<string> symboleString = new() { "@S", "@H", "@K", "@X", "@F", "@N", "@A", "@s", "@h", "@G", "HP", "Str", "Agi", "Int", "Vit", "Lck", "Ht%", "MDf", "Wt", "Bk", "Sp" };

			// Addresses
			const int lut_LvlUpHitRateBonus = 0x6CA59;
			const int lut_LvlUpMagDefBonus = 0x6CA65;
			const int StartingStatsOffset = 0x3040;
			const int NewLevelUpDataOffset = 0x6CDA9;
			const int MagicPermissionsOffset = 0x3AD18;
			const int lut_WeaponPermissions = 0x3BF50;
			const int lut_ArmorPermissions = 0x3BFA0;
			const int lut_MaxMP = 0x6C902;

			// Starting Stats awards MP to allow any class to start with spell charges
			PutInBank(0x1F, 0xC7CA, Blob.FromHex("B94BB09D20639D286360"));
			PutInBank(0x00, 0xB07B, Blob.FromHex("02"));
			PutInBank(0x00, 0xB08B, Blob.FromHex("02"));
			PutInBank(0x00, 0xB09B, Blob.FromHex("02"));

			// Status jump to new menu wait, switch banks, see 0E_A0E0_MenuWaitForBtn_SFX_Status.asm
			PutInBank(0x0E, 0xB50D, Blob.FromHex("4CE0A0"));
			PutInBank(0x0E, 0xA0E0, Blob.FromHex("A98948A90F48A91E85574C03FE"));

			// EnterInfoMenu, see 1E_8800_DrawInfoBox.asm
			PutInBank(0x1E, 0x8800, Blob.FromHex("203CC4A5674A4A4A4A4AB015A200205B83A220205B83A9118538A90285394C3388A210205B83A230205B83A9038538A9028539A667BD0003C9FFD002A90C0AAA207188A98048A9C0484C1A85A9118538A9028539A667BD0061C9FFD002A90C0AAA207188A9B648A91248A90E85574C03FEA90D853CA91A853DA9008D0120A90085378A482063E0A970853EA989853FA538853AE63AA539853BE63BA91E855785582036DE68AABD5089853EBD5189853FA53B186902853B2036DE205E8560"));

			// StatusWaitForBtn_SFX, see 1E_8800_DrawInfoBox.asm
			PutInBank(0x1E, 0x8910, Blob.FromHex("202C85A5240522D00FA525F0F3A9008525A90E85574C03FEA9008524852585224C4C88"));

			// InfoScreen in PtyGen and Status screen
			// DoPartyGen_OnCharacter change to check for Select button, see 1E_8800_DrawInfoBox.asm
			if (flags.EnablePoolParty ?? false)
			{
				// We just reproduce EnablePoolParty()'s new DoPartyGen_OnCharacter and add the select button
				PutInBank(0x1E, 0x85B0, Blob.FromHex("A667BD01030D41038D4103A9FF8D4003BD0103C900F00718EE40032A90FA20A480A9008522200F82A522F0034C0088A667AC4003A524F013BD0003C9FFF009BD01034D41038D41034C2C81A525F0118AC900F00AA9009D0103A9FF9D00033860A520290FC561F0B98561C900F0B3C898C9099002A0008C4003B944862C4103F0ED9D0103B942039D0003A901853720B0824CD1858040201008040201"));
				PutInBank(0x1E, 0x8843, Blob.FromHex("A98548A9AF48"));
			}
			else
			{
				Blob partypermissions = Get(0x78110, 0x11);
				PutInBank(0x1E, 0x80C1, Blob.FromHex("A6678A4A4A4A4AA8B9B085859020A480A9008522200F82A522F0034C0088A524D049A525F0023860A520290FC561F0E08561C900F0DAA667BD0003186901C90CD002A9FF9D0003A8C8B9B4852490F0E8A901853720B0824CD180"));
				PutInBank(0x1E, 0x85B0, partypermissions);
			}

			// Get data
			List<Blob> startingStats = Get(StartingStatsOffset, 0x60).Chunk(0x10);
			List<Blob> levelUpStats = Get(NewLevelUpDataOffset, 588).Chunk(49 * 2);
			List<byte> hitGrowth = Get(lut_LvlUpHitRateBonus, 12).ToBytes().ToList();
			List<byte> mdefGrowthBase = Get(lut_LvlUpMagDefBonus, 6).ToBytes().ToList();
			List<byte> mdefGrowthPromo = Get(lut_LvlUpMagDefBonus + 6, 6).ToBytes().ToList();
			List<Blob> magicPermissions = Get(MagicPermissionsOffset, 8 * 12).Chunk(8);
			Blob maxChargeList = Get(lut_MaxMP, 12);
			ushort[] wpPermissions = Get(lut_WeaponPermissions, 80).ToUShorts();
			ushort[] arPermissions = Get(lut_ArmorPermissions, 80).ToUShorts();

			// Populate stats
			for (int i = 0; i < 6; i++)
			{
				classData[i].GetStartingStatsArray(startingStats[i]);
				classData[i + 6].GetStartingStatsArray(startingStats[i]);
				classData[i].GetLevelUpArray(levelUpStats[i]);
				classData[i + 6].GetLevelUpArray(levelUpStats[i]);
				classData[i].GetMagicPermissions(magicPermissions[i]);
				classData[i + 6].GetMagicPermissions(magicPermissions[i + 6]);
				classData[i].HitGrowth = hitGrowth[i];
				classData[i + 6].HitGrowth = hitGrowth[i + 6];
				classData[i].MDefGrowth = mdefGrowthBase[i];
				classData[i + 6].MDefGrowth = mdefGrowthPromo[i];
				classData[i].MaxSpC = maxChargeList[i];
				classData[i + 6].MaxSpC = maxChargeList[i + 6];
			}

			// Equipment permissions
			for (int i = 0; i < 40; i++)
			{
				for (int j = 0; j < 12; j++)
				{
					if ((~wpPermissions[i] & equipmentPermissionBit[j]) > 0)
					{
						classData[j].WeaponPermissions.Add((Item)(i + 28));
					}

					if ((~arPermissions[i] & equipmentPermissionBit[j]) > 0)
					{
						classData[j].ArmourPermissions.Add((Item)(i + 68));
					}
				}
			}

			List<string> bonusmalusDescription = new();

			// Chaos Mode enabled?
			if ((bool)flags.RandomizeClassChaos)
			{
				DoRandomizeClassChaosMode(ref classData, ((bool)flags.MagicLevelsMixed && (bool)flags.MagicPermissions) || ((bool)flags.SpellcrafterMixSpells && !(bool)flags.SpellcrafterRetainPermissions), rng);
			}
			else
			{
				bonusmalusDescription = DoRandomizeClassNormalMode(ref classData, rng, itemnames, flags.RandomizeClassMaxBonus, flags.RandomizeClassMaxMalus, (bool)flags.RandomizeClassNoCasting);
			}

			// Update equipment permissions
			for (int i = 0; i < 40; i++)
			{
				for (int j = 0; j < 12; j++)
				{
					if (classData[j].WeaponPermissions.Contains((Item)(i + 28)))
					{
						wpNewPermissions[i] |= equipmentPermissionBit[j];
					}

					if (classData[j].ArmourPermissions.Contains((Item)(i + 68)))
					{
						arNewPermissions[i] |= equipmentPermissionBit[j];
					}
				}
				wpNewPermissions[i] = (ushort)~wpNewPermissions[i];
				arNewPermissions[i] = (ushort)~arNewPermissions[i];
			}

			// Update Starting Attack and Evasion
			for (int i = 0; i < 12; i++)
			{
				classData[i].DmgStarting = (byte)Math.Min(classData[i].StrStarting / 2, 255);
				classData[i].EvaStarting = (byte)Math.Min(classData[i].AgiStarting + 48, 255);
			}

			// Insert starting stats
			Put(0x3040, classData.GetRange(0, 6).SelectMany(x => x.StartingStatsArray()).ToArray());

			// Insert level up data
			Put(NewLevelUpDataOffset, classData.GetRange(0, 6).SelectMany(x => x.LevelUpArray()).ToArray());

			// Insert hit% and mdef growth rate
			Put(lut_LvlUpHitRateBonus, classData.Select(x => x.HitGrowth).ToArray());
			Put(lut_LvlUpMagDefBonus, classData.Select(x => x.MDefGrowth).ToArray());

			// Insert max spell charges array
			Put(lut_MaxMP, classData.Select(x => x.MaxSpC).ToArray());

			// Inset spell permissions
			Put(MagicPermissionsOffset, classData.SelectMany(x => x.MagicPermissions()).ToArray());

			// Insert equipment permissions
			PutInBank(0x0E, 0xBF50, Blob.FromUShorts(wpNewPermissions.ToArray()));
			PutInBank(0x0E, 0xBFA0, Blob.FromUShorts(arNewPermissions.ToArray()));

			// dataScreen
			int totalByte = 0;
			string templateScreen = "";
			Blob screenBlob = Blob.FromHex("00");
			List<string> dataScreen = new();

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
			{
				templateScreen = "BONUS";
			}

			// Insert template
			screenBlob = FF1Text.TextToBytes(templateScreen, true, FF1Text.Delimiter.Null);
			PutInBank(0x1E, 0x8970, screenBlob);
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
							if (classData[i + 6].Ranks[j] > classData[i].Ranks[j])
							{
								if (j == (int)RankedType.White)
								{
									promoChange += classData[i + 6].MagicRanks[0] + "W";
								}
								else if (j == (int)RankedType.Black)
								{
									promoChange += classData[i + 6].MagicRanks[1] + "B";
								}
								else
								{
									promoChange += symboleString[j] + rankString[(int)classData[i + 6].Ranks[j]];
								}

								if (promoChange.Split('\n').Last().Length > (11 - 4))
								{
									promoChange += "\n";
								}
								else
								{
									promoChange += " ";
								}
							}
						}
					}

					// Generate data screen
					string dataChaosScreen =
						rankString[(int)classData[i].Ranks[(int)RankedType.Strength]] + " " +
						rankString[(int)classData[i].Ranks[(int)RankedType.Agility]] + " " +
						rankString[(int)classData[i].Ranks[(int)RankedType.Intellect]] + " " +
						rankString[(int)classData[i].Ranks[(int)RankedType.Vitality]] + " " +
						rankString[(int)classData[i].Ranks[(int)RankedType.Luck]] +
						"\n\n\n" +
						" " + classData[i].HitGrowth + "   " +
						classData[i].MDefGrowth + "   " +
						rankString[(int)classData[i].Ranks[(int)RankedType.HP]] +
						"\n\n\n\n" +
						" " + classData[i].MagicRanks[0] + "  " +
						classData[i].MagicRanks[1] + "  " +
						rankString[(int)classData[i].Ranks[(int)RankedType.Charges]] +
						//rankString[(int)classData[i].Ranks[(int)RankedType.Black]] + "   " +
						//rankString[(int)classData[i].Ranks[(int)RankedType.Charges]] +
						"\n\n\n\n" +
						rankString[(int)classData[i].Ranks[(int)RankedType.Swords]] + " " +
						rankString[(int)classData[i].Ranks[(int)RankedType.Hammers]] + " " +
						rankString[(int)classData[i].Ranks[(int)RankedType.Knives]] + " " +
						rankString[(int)classData[i].Ranks[(int)RankedType.Axes]] + " " +
						rankString[(int)classData[i].Ranks[(int)RankedType.Staves]] + " " +
						rankString[(int)classData[i].Ranks[(int)RankedType.Nunchucks]] +
						"\n\n\n\n" +
						rankString[(int)classData[i].Ranks[(int)RankedType.Armors]] + " " +
						rankString[(int)classData[i].Ranks[(int)RankedType.Shields]] + " " +
						rankString[(int)classData[i].Ranks[(int)RankedType.Helmets]] + " " +
						rankString[(int)classData[i].Ranks[(int)RankedType.Gauntlets]] +
						"\n\n\n" +
						string.Join("", promoChange);

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
				Blob tempBlob = FF1Text.TextToBytes(dataScreen[i], true, FF1Text.Delimiter.Null);
				PutInBank(0x1E, 0x8970 + totalByte, tempBlob);
				int tempAddress = 0x8970 + totalByte;
				PutInBank(0x1E, 0x8950 + (i * 2), new byte[] { (byte)(tempAddress % 0x100), (byte)(tempAddress / 0x100) });
				totalByte += tempBlob.Length;
			}

			// Insert null data screen for None
			PutInBank(0x1E, 0x8970 + totalByte, Blob.FromHex("00"));
			int noneAddress = 0x8970 + totalByte;
			PutInBank(0x1E, 0x8950 + 24, new byte[] { (byte)(noneAddress % 0x100), (byte)(noneAddress / 0x100) });
		}

		public List<string> DoRandomizeClassNormalMode(ref List<ClassData> classData, MT19337 rng, string[] itemnames, int maxbonus, int maxmalus, bool noCastingBonus)
		{
			// Equipment lists
			List<Item> equipFighterArmor = new()
			{
				Item.WoodenArmor,
				Item.ChainArmor,
				Item.SilverArmor,
				Item.IronArmor,
				Item.FlameArmor,
				Item.IceArmor,
				Item.SteelArmor,
				Item.Buckler,
				Item.WoodenShield,
				Item.IronShield,
				Item.ProCape,
				Item.SilverShield,
				Item.FlameShield,
				Item.IceShield,
				Item.WoodenHelm,
				Item.IronHelm,
				Item.SilverHelm,
				Item.CopperGauntlets,
				Item.IronGauntlets,
				Item.SilverGauntlets,
				Item.PowerGauntlets
			};

			List<Item> equipRedMageArmor = new() { Item.WoodenArmor, Item.ChainArmor, Item.SilverArmor, Item.Buckler, Item.ProCape };

			List<Item> equipKnightArmor = new(equipFighterArmor)
			{
				Item.DragonArmor,
				Item.OpalArmor,
				Item.AegisShield,
				Item.OpalShield,
				Item.HealHelm,
				Item.OpalHelm,
				Item.PowerGauntlets,
				Item.ZeusGauntlets,
				Item.OpalGauntlets
			};

			List<Item> equipFighterWeapon = new()
			{
				Item.Rapier,
				Item.Scimitar,
				Item.ShortSword,
				Item.LongSword,
				Item.Falchon,
				Item.Sabre,
				Item.SilverSword,
				Item.WereSword,
				Item.RuneSword,
				Item.DragonSword,
				Item.CoralSword,
				Item.GiantSword,
				Item.FlameSword,
				Item.IceSword,
				Item.SunSword,
				Item.SmallKnife,
				Item.WoodenRod,
				Item.IronHammer,
				Item.HandAxe,
				Item.LargeKnife,
				Item.IronStaff,
				Item.GreatAxe,
				Item.SilverAxe,
				Item.SilverKnife,
				Item.SilverHammer,
				Item.PowerRod,
				Item.LightAxe
			};

			List<Item> equipKnightWeapon = new(equipFighterWeapon) { Item.Defense, Item.Vorpal, Item.CatClaw, Item.ThorHammer, Item.BaneSword, Item.Xcalber };

			List<Item> equipThiefWeapon = new()
			{
				Item.SmallKnife,
				Item.Rapier,
				Item.Scimitar,
				Item.LargeKnife,
				Item.Sabre,
				Item.Falchon,
				Item.SilverKnife,
				Item.DragonSword,
				Item.CoralSword,
				Item.RuneSword,
				Item.Masamune
			};

			// Create exceptions for hit bonus
			List<AuthClass> hitBonusClass = new();

			for (int i = 0; i < 6; i++)
			{
				if (classData[i].HitGrowth < 4)
				{
					hitBonusClass.Add((AuthClass)i);
				}
			}

			// Spells lists
			List<bool> nullSpells = Enumerable.Repeat(false, 4 * 8).ToList();

			List<bool> lv1WhiteSpells = new(classData[4].WhitePermissions.GetRange(0, 4).Concat(nullSpells.GetRange(0, 28)));

			List<bool> lv1BlackSpells = new(classData[5].BlackPermissions.GetRange(0, 4).Concat(nullSpells.GetRange(0, 28)));

			List<bool> lv3WhiteSpells = new(classData[6].WhitePermissions);
			List<bool> lv4BlackSpells = new(classData[7].BlackPermissions);

			List<bool> wmWhiteSpells = new(classData[4].WhitePermissions);
			List<bool> bmBlackSpells = new(classData[5].BlackPermissions);

			List<bool> wwWhiteSpells = new(classData[10].WhitePermissions);
			List<bool> bwBlackSpells = new(classData[11].BlackPermissions);

			// MP Growth Lists
			List<byte> rmMPlist = new(classData[3].SpCGrowth);

			List<byte> improvedMPlist = new()
			{
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00,
				0xFF,
				0x00
			};
			List<byte> exKnightMPlist = new()
			{
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00,
				0x07,
				0x00
			};
			List<byte> exNinjaMPlist = new()
			{
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00,
				0x0F,
				0x00
			};

			// Normal Bonuses List
			List<BonusMalus> bonusNormal = new()
			{
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
				new BonusMalus(BonusMalusAction.HitMod, "+10 Hit%", mod: 10, authclass: hitBonusClass),
				new BonusMalus(BonusMalusAction.HitMod, "+15 Hit%", mod: 15, authclass: hitBonusClass),
				new BonusMalus(BonusMalusAction.HitMod, "+20 Hit%", mod: 20, authclass: hitBonusClass),
				new BonusMalus(BonusMalusAction.MDefMod, "+10 MDef", mod: 10),
				new BonusMalus(BonusMalusAction.MDefMod, "+15 MDef", mod: 15),
				new BonusMalus(BonusMalusAction.MDefMod, "+20 MDef", mod: 20),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+" + itemnames[(int)Item.ThorHammer], equipment: new List<Item> { Item.ThorHammer }),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+" + itemnames[(int)Item.Vorpal], equipment: new List<Item> { Item.Vorpal }),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+" + itemnames[(int)Item.Defense], equipment: new List<Item> { Item.Defense }),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+" + itemnames[(int)Item.Katana], equipment: new List<Item> { Item.Katana }),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+" + itemnames[(int)Item.Xcalber], equipment: new List<Item> { Item.Xcalber }),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+" + itemnames[(int)Item.WhiteShirt], equipment: new List<Item> { Item.WhiteShirt }),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+" + itemnames[(int)Item.BlackShirt], equipment: new List<Item> { Item.BlackShirt }),
				new BonusMalus(BonusMalusAction.StrGrowth, "Fightr Str.", binarylist: classData[(int)AuthClass.Fighter].StrGrowth, authclass: new List<AuthClass> { AuthClass.Thief, AuthClass.BlackBelt, AuthClass.RedMage, AuthClass.WhiteMage, AuthClass.BlackMage }),
				new BonusMalus(BonusMalusAction.LckGrowth, "Thief Luck", binarylist: classData[(int)AuthClass.Thief].LckGrowth, authclass: new List<AuthClass> { AuthClass.Fighter, AuthClass.BlackBelt, AuthClass.RedMage, AuthClass.WhiteMage, AuthClass.BlackMage }),
				new BonusMalus(BonusMalusAction.VitGrowth, "B.Belt Vit.", binarylist: classData[(int)AuthClass.BlackBelt].VitGrowth, authclass: new List<AuthClass> { AuthClass.Fighter, AuthClass.Thief, AuthClass.RedMage, AuthClass.WhiteMage, AuthClass.BlackMage }),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+Thief @S", equipment: equipThiefWeapon, authclass: new List<AuthClass> { AuthClass.BlackBelt, AuthClass.WhiteMage, AuthClass.BlackMage }),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Red Mage @A", equipment: equipRedMageArmor, authclass: new List<AuthClass> { AuthClass.Thief, AuthClass.BlackBelt, AuthClass.WhiteMage, AuthClass.BlackMage }),
				new BonusMalus(BonusMalusAction.SpcMod, "+2 Lv1 MP", mod: 2, authclass: new List<AuthClass> { AuthClass.RedMage, AuthClass.WhiteMage, AuthClass.BlackMage }),
				new BonusMalus(BonusMalusAction.SpcMod, "+2 Lv1 MP", mod: 2, authclass: new List<AuthClass> { AuthClass.RedMage, AuthClass.WhiteMage, AuthClass.BlackMage }),
			};

			// Strong Bonuses List
			List<BonusMalus> bonusStrong = new()
			{
				new BonusMalus(BonusMalusAction.StrMod, "+40 Str.", mod: 40),
				new BonusMalus(BonusMalusAction.AgiMod, "+40 Agi.", mod: 40),
				new BonusMalus(BonusMalusAction.VitMod, "+40 Vit.", mod: 40),
				new BonusMalus(BonusMalusAction.LckMod, "+15 Luck", mod: 15),
				new BonusMalus(BonusMalusAction.HpMod, "+80 HP", mod: 80),
				new BonusMalus(BonusMalusAction.MDefGrowth, "+2 MDef/Lv", mod: 2),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+Fighter @S", equipment: equipFighterWeapon, authclass: new List<AuthClass> { AuthClass.Thief, AuthClass.BlackBelt, AuthClass.WhiteMage, AuthClass.BlackMage }),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Fighter @A", equipment: equipFighterArmor, authclass: new List<AuthClass> { AuthClass.Thief, AuthClass.BlackBelt, AuthClass.WhiteMage, AuthClass.BlackMage, AuthClass.RedMage }),
				new BonusMalus(BonusMalusAction.SpcGrowth, "Improved MP", bytelist: improvedMPlist, authclass: new List<AuthClass> { AuthClass.RedMage, AuthClass.WhiteMage, AuthClass.BlackMage }),
				new BonusMalus(BonusMalusAction.PowerRW, "Sage Class", binarylist: wmWhiteSpells.Concat(bmBlackSpells).Concat(wwWhiteSpells).Concat(bwBlackSpells).ToList(), authclass: new List<AuthClass> { AuthClass.RedMage }),
				new BonusMalus(BonusMalusAction.WhiteSpellcaster, "White W. Sp", binarylist: wwWhiteSpells, authclass: new List<AuthClass> { AuthClass.WhiteMage }),
				new BonusMalus(BonusMalusAction.BlackSpellcaster, "Black W. Sp", binarylist: bwBlackSpells, authclass: new List<AuthClass> { AuthClass.BlackMage }),
				//new BonusMalus(BonusMalusAction.EquipmentAdd, "+Knight\n Weapons", equipment: equipKnightWeapon, authclass: new List<AuthClass> { AuthClass.Thief, AuthClass.BlackBelt, AuthClass.WhiteMage, AuthClass.BlackMage } ),
			};

			if (!noCastingBonus)
			{
				bonusNormal.Add(new BonusMalus(BonusMalusAction.WhiteSpellcaster, "L1 White Sp", mod: 2, mod2: 0, binarylist: lv1WhiteSpells, authclass: new List<AuthClass> { AuthClass.Fighter, AuthClass.Thief, AuthClass.BlackBelt, AuthClass.RedMage, AuthClass.BlackMage }));
				bonusNormal.Add(new BonusMalus(BonusMalusAction.BlackSpellcaster, "L1 Black Sp", mod: 2, mod2: 0, binarylist: lv1BlackSpells, authclass: new List<AuthClass> { AuthClass.Fighter, AuthClass.Thief, AuthClass.BlackBelt, AuthClass.WhiteMage }));
				bonusNormal.Add(new BonusMalus(BonusMalusAction.WhiteSpellcaster, "Knight Sp", mod: 2, mod2: classData[6].MaxSpC, binarylist: lv3WhiteSpells, bytelist: exKnightMPlist, authclass: new List<AuthClass> { AuthClass.Fighter, AuthClass.Thief, AuthClass.BlackBelt, AuthClass.BlackMage }));
				bonusNormal.Add(new BonusMalus(BonusMalusAction.BlackSpellcaster, "Ninja Sp", mod: 2, mod2: classData[7].MaxSpC, binarylist: lv4BlackSpells, bytelist: exNinjaMPlist, authclass: new List<AuthClass> { AuthClass.Fighter, AuthClass.Thief, AuthClass.BlackBelt, AuthClass.WhiteMage }));
				bonusStrong.Add(new BonusMalus(BonusMalusAction.WhiteSpellcaster, "White M. Sp", mod: 2, mod2: classData[4].MaxSpC, binarylist: wmWhiteSpells, bytelist: rmMPlist, authclass: new List<AuthClass> { AuthClass.Fighter, AuthClass.Thief, AuthClass.BlackBelt, AuthClass.RedMage, AuthClass.BlackMage }));
				bonusStrong.Add(new BonusMalus(BonusMalusAction.BlackSpellcaster, "Black M. Sp", mod: 2, mod2: classData[5].MaxSpC, binarylist: bmBlackSpells, bytelist: rmMPlist, authclass: new List<AuthClass> { AuthClass.Fighter, AuthClass.Thief, AuthClass.BlackBelt, AuthClass.WhiteMage }));
			}

			// Maluses List
			List<BonusMalus> malusNormal = new()
			{
				new BonusMalus(BonusMalusAction.StrMod, "-10 Str.", mod: -10),
				new BonusMalus(BonusMalusAction.StrMod, "-10 Str.", mod: -10),
				new BonusMalus(BonusMalusAction.StrMod, "-15 Str.", mod: -15),
				new BonusMalus(BonusMalusAction.StrGrowth, "BlackM Str.", binarylist: classData[(int)AuthClass.BlackMage].StrGrowth, authclass: new List<AuthClass> { AuthClass.Fighter, AuthClass.Thief, AuthClass.BlackBelt, AuthClass.RedMage }),
				new BonusMalus(BonusMalusAction.AgiMod, "-10 Agi.", mod: -10),
				new BonusMalus(BonusMalusAction.AgiMod, "-10 Agi.", mod: -10),
				new BonusMalus(BonusMalusAction.AgiMod, "-15 Agi.", mod: -15),
				new BonusMalus(BonusMalusAction.AgiGrowth, "BlackM Agi.", binarylist: classData[(int)AuthClass.BlackMage].AgiGrowth, authclass: new List<AuthClass> { AuthClass.Fighter, AuthClass.Thief, AuthClass.BlackBelt }),
				new BonusMalus(BonusMalusAction.VitMod, "-10 Vit.", mod: -10),
				new BonusMalus(BonusMalusAction.VitMod, "-10 Vit.", mod: -10),
				new BonusMalus(BonusMalusAction.VitMod, "-15 Vit.", mod: -15),
				new BonusMalus(BonusMalusAction.VitGrowth, "BlackM Vit.", binarylist: classData[(int)AuthClass.BlackMage].VitGrowth, authclass: new List<AuthClass> { AuthClass.Fighter, AuthClass.BlackBelt, AuthClass.RedMage }),
				new BonusMalus(BonusMalusAction.LckMod, "-5 Luck", mod: -5),
				new BonusMalus(BonusMalusAction.LckMod, "-5 Luck", mod: -5),
				new BonusMalus(BonusMalusAction.LckMod, "-10 Luck", mod: -10),
				new BonusMalus(BonusMalusAction.HpMod, "-20 HP", mod: -20),
				new BonusMalus(BonusMalusAction.HpMod, "-20 HP", mod: -20),
				new BonusMalus(BonusMalusAction.HpMod, "-30 HP", mod: -30),
				new BonusMalus(BonusMalusAction.HpGrowth, "BlackM HP", binarylist: classData[(int)AuthClass.BlackMage].HpGrowth, authclass: new List<AuthClass> { AuthClass.Fighter }),
				new BonusMalus(BonusMalusAction.HitMod, "-10 Hit%", mod: -10),
				new BonusMalus(BonusMalusAction.MDefMod, "-10 MDef", mod: -10),
				new BonusMalus(BonusMalusAction.HitGrowth, "-1 Hit%/Lv", mod: -1),
				new BonusMalus(BonusMalusAction.MDefGrowth, "-1 MDef/Lv", mod: -1),
				new BonusMalus(BonusMalusAction.ArmorRemove, "-" + itemnames[(int)Item.Ribbon], equipment: new List<Item> { Item.Ribbon }),
				new BonusMalus(BonusMalusAction.ArmorRemove, "-" + itemnames[(int)Item.ProRing], equipment: new List<Item> { Item.ProRing }),
				new BonusMalus(BonusMalusAction.ArmorRemove, "No @B", equipment: new List<Item> { Item.Gold, Item.Opal, Item.Silver, Item.Copper }),
				new BonusMalus(BonusMalusAction.WeaponReplace, "Thief @S", equipment: equipThiefWeapon, authclass: new List<AuthClass> { AuthClass.Fighter, AuthClass.RedMage }),
				new BonusMalus(BonusMalusAction.SpcMax, "-4 Max MP", mod: -4, authclass: new List<AuthClass> { AuthClass.RedMage, AuthClass.WhiteMage, AuthClass.BlackMage }),
				new BonusMalus(BonusMalusAction.NoPromoMagic, "No Promo Sp", mod: 0, mod2: 0, binarylist: nullSpells, authclass: new List<AuthClass> { AuthClass.Fighter, AuthClass.Thief }),
				//new BonusMalus(BonusMalusAction.BlackNewSpellcaster, "Black Mage\n Spells", mod: 2, mod2: 9, binarylist: bmBlackSpells, bytelist: rmMPlist, authclass: new List<AuthClass> { AuthClass.Fighter, AuthClass.Thief, AuthClass.BlackBelt }),
			};

			if (Rng.Between(rng, 0, 10) == 0)
			{
				malusNormal.Add(new BonusMalus(BonusMalusAction.IntMod, "+80 Int.", mod: 80));
			}

			List<List<BonusMalus>> assignedBonusMalus = new() { new List<BonusMalus>(), new List<BonusMalus>(), new List<BonusMalus>(), new List<BonusMalus>(), new List<BonusMalus>(), new List<BonusMalus>() };

			// Shuffle bonuses and maluses
			bonusNormal.Shuffle(rng);
			bonusStrong.Shuffle(rng);
			malusNormal.Shuffle(rng);

			// Select one incentivized class that will received a strong bonus
			int luckyDude = Rng.Between(rng, 0, 5);

			List<string> descriptionList = new();

			// Distribute bonuses and maluses, we go backward (from BM to Fi) so we have enough malus for BM
			for (int i = 5; i >= 0; i--)
			{
				List<(int, string)> tempstring = new();

				if (i == luckyDude && maxbonus > 0)
				{
					while (!bonusStrong.First().ClassList.Contains((AuthClass)i))
					{
						bonusStrong.Shuffle(rng);
					}

					assignedBonusMalus[i].Add(bonusStrong.First());
					tempstring.Add((0, bonusStrong.First().Description));
					bonusStrong.RemoveRange(0, 1);

					for (int j = 1; j < maxbonus; j++)
					{
						while (!bonusNormal.First().ClassList.Contains((AuthClass)i))
						{
							bonusNormal.Shuffle(rng);
						}

						assignedBonusMalus[i].Add(bonusNormal.First());
						tempstring.Add((0, bonusNormal.First().Description));
						bonusNormal.RemoveRange(0, 1);
					}
				}
				else
				{
					for (int j = 0; j < maxbonus; j++)
					{
						while (!bonusNormal.First().ClassList.Contains((AuthClass)i))
						{
							bonusNormal.Shuffle(rng);
						}

						assignedBonusMalus[i].Add(bonusNormal.First());
						tempstring.Add((0, bonusNormal.First().Description));
						bonusNormal.RemoveRange(0, 1);
					}
				}

				for (int j = 0; j < maxmalus; j++)
				{
					while (!malusNormal.First().ClassList.Contains((AuthClass)i) || assignedBonusMalus[i].Select(x => x.Action).ToList().Contains(malusNormal.First().Action))
					{
						malusNormal.Shuffle(rng);
					}

					assignedBonusMalus[i].Add(malusNormal.First());
					if (malusNormal.First().Action == BonusMalusAction.IntMod)
					{
						tempstring.Add((0, malusNormal.First().Description));
					}
					else
					{
						tempstring.Add((1, malusNormal.First().Description));
					}

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

				foreach (BonusMalus bonusmalus in assignedBonusMalus[i])
				{
					switch (bonusmalus.Action)
					{
						case BonusMalusAction.StrMod:
							classData[i].StrStarting = (byte)Math.Max(classData[i].StrStarting + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.AgiMod:
							classData[i].AgiStarting = (byte)Math.Max(classData[i].AgiStarting + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.IntMod:
							classData[i].IntStarting = (byte)Math.Max(classData[i].IntStarting + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.VitMod:
							classData[i].VitStarting = (byte)Math.Max(classData[i].VitStarting + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.LckMod:
							classData[i].LckStarting = (byte)Math.Max(classData[i].LckStarting + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.HitMod:
							classData[i].HitStarting = (byte)Math.Max(classData[i].HitStarting + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.MDefMod:
							classData[i].MDefStarting = (byte)Math.Max(classData[i].MDefStarting + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.HpMod:
							classData[i].HpStarting = (byte)Math.Max(classData[i].HpStarting + bonusmalus.StatMod, 1);
							break;
						case BonusMalusAction.StrGrowth:
							classData[i].StrGrowth = bonusmalus.StatGrowth;
							break;
						case BonusMalusAction.AgiGrowth:
							classData[i].AgiGrowth = bonusmalus.StatGrowth;
							break;
						case BonusMalusAction.IntGrowth:
							classData[i].IntGrowth = bonusmalus.StatGrowth;
							break;
						case BonusMalusAction.VitGrowth:
							classData[i].VitGrowth = bonusmalus.StatGrowth;
							break;
						case BonusMalusAction.LckGrowth:
							classData[i].LckGrowth = bonusmalus.StatGrowth;
							break;
						case BonusMalusAction.HpGrowth:
							classData[i].HpGrowth = bonusmalus.StatGrowth;
							break;
						case BonusMalusAction.HitGrowth:
							classData[i].HitGrowth = (byte)Math.Max(classData[i].HitGrowth + bonusmalus.StatMod, 0);
							classData[i + 6].HitGrowth = (byte)Math.Max(classData[i + 6].HitGrowth + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.MDefGrowth:
							classData[i].MDefGrowth = (byte)Math.Max(classData[i].MDefGrowth + bonusmalus.StatMod, 0);
							classData[i + 6].MDefGrowth = (byte)Math.Max(classData[i + 6].MDefGrowth + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.WeaponAdd:
							classData[i].WeaponPermissions.AddRange(bonusmalus.Equipment);
							classData[i + 6].WeaponPermissions.AddRange(bonusmalus.Equipment);
							break;
						case BonusMalusAction.WeaponRemove:
							classData[i].WeaponPermissions = classData[i].WeaponPermissions.Except(bonusmalus.Equipment).ToList();
							classData[i + 6].WeaponPermissions = classData[i + 6].WeaponPermissions.Except(bonusmalus.Equipment).ToList();
							break;
						case BonusMalusAction.WeaponReplace:
							classData[i].WeaponPermissions = bonusmalus.Equipment;
							classData[i + 6].WeaponPermissions = bonusmalus.Equipment;
							break;
						case BonusMalusAction.ArmorAdd:
							classData[i].ArmourPermissions.AddRange(bonusmalus.Equipment);
							classData[i + 6].ArmourPermissions.AddRange(bonusmalus.Equipment);
							break;
						case BonusMalusAction.ArmorRemove:
							classData[i].ArmourPermissions = classData[i].ArmourPermissions.Except(bonusmalus.Equipment).ToList();
							classData[i + 6].ArmourPermissions = classData[i + 6].ArmourPermissions.Except(bonusmalus.Equipment).ToList();
							break;
						case BonusMalusAction.ArmorReplace:
							classData[i].ArmourPermissions = bonusmalus.Equipment;
							classData[i + 6].ArmourPermissions = bonusmalus.Equipment;
							break;
						case BonusMalusAction.SpcMod:
							classData[i].SpCStarting = (byte)Math.Max(classData[i].SpCStarting + bonusmalus.StatMod, 0);
							classData[i + 6].SpCStarting = (byte)Math.Max(classData[i + 6].SpCStarting + bonusmalus.StatMod, 0);
							break;
						case BonusMalusAction.WhiteSpellcaster:
							if (classData[i].SpCStarting < (byte)bonusmalus.StatMod)
							{
								classData[i].SpCStarting = (byte)bonusmalus.StatMod;
							}

							if (classData[i].MaxSpC < (byte)bonusmalus.StatMod2)
							{
								classData[i].MaxSpC = (byte)bonusmalus.StatMod2;
							}

							if (classData[i].SpCGrowth.Select(x => (int)x).ToList().Sum() < bonusmalus.SpcGrowth.Select(x => (int)x).ToList().Sum())
							{
								classData[i].SpCGrowth = bonusmalus.SpcGrowth;
							}

							if (classData[i + 6].SpCStarting < (byte)bonusmalus.StatMod)
							{
								classData[i + 6].SpCStarting = (byte)bonusmalus.StatMod;
							}

							if (classData[i + 6].MaxSpC < (byte)bonusmalus.StatMod2)
							{
								classData[i + 6].MaxSpC = (byte)bonusmalus.StatMod2;
							}

							classData[i].WhitePermissions = classData[i].WhitePermissions.Zip(bonusmalus.SpellList, (x, y) => x || y).ToList();
							classData[i + 6].WhitePermissions = classData[i + 6].WhitePermissions.Zip(bonusmalus.SpellList, (x, y) => x || y).ToList();
							break;
						case BonusMalusAction.BlackSpellcaster:
							if (classData[i].SpCStarting < (byte)bonusmalus.StatMod)
							{
								classData[i].SpCStarting = (byte)bonusmalus.StatMod;
							}

							if (classData[i].MaxSpC < (byte)bonusmalus.StatMod2)
							{
								classData[i].MaxSpC = (byte)bonusmalus.StatMod2;
							}

							if (classData[i].SpCGrowth.Select(x => (int)x).ToList().Sum() < bonusmalus.SpcGrowth.Select(x => (int)x).ToList().Sum())
							{
								classData[i].SpCGrowth = bonusmalus.SpcGrowth;
							}

							if (classData[i + 6].SpCStarting < (byte)bonusmalus.StatMod)
							{
								classData[i + 6].SpCStarting = (byte)bonusmalus.StatMod;
							}

							if (classData[i + 6].MaxSpC < (byte)bonusmalus.StatMod2)
							{
								classData[i + 6].MaxSpC = (byte)bonusmalus.StatMod2;
							}

							classData[i].BlackPermissions = classData[i].BlackPermissions.Zip(bonusmalus.SpellList, (x, y) => x || y).ToList();
							classData[i + 6].BlackPermissions = classData[i + 6].BlackPermissions.Zip(bonusmalus.SpellList, (x, y) => x || y).ToList();
							break;
						case BonusMalusAction.SpcMax:
							classData[i].MaxSpC = (byte)Math.Max(classData[i].MaxSpC + bonusmalus.StatMod, 1);
							classData[i + 6].MaxSpC = (byte)Math.Max(classData[i + 6].MaxSpC + bonusmalus.StatMod, 1);
							break;
						case BonusMalusAction.SpcGrowth:
							classData[i].SpCGrowth = bonusmalus.SpcGrowth;
							classData[i + 6].SpCGrowth = bonusmalus.SpcGrowth;
							break;
						case BonusMalusAction.PowerRW:
							classData[i].WhitePermissions = bonusmalus.SpellList.GetRange(0, 32);
							classData[i].BlackPermissions = bonusmalus.SpellList.GetRange(32, 32);
							classData[i + 6].WhitePermissions = bonusmalus.SpellList.GetRange(64, 32);
							classData[i + 6].BlackPermissions = bonusmalus.SpellList.GetRange(96, 32);
							break;
						case BonusMalusAction.NoPromoMagic:
							classData[i + 6].WhitePermissions = bonusmalus.SpellList;
							classData[i + 6].BlackPermissions = bonusmalus.SpellList;
							classData[i + 6].MaxSpC = 0;
							classData[i + 6].SpCStarting = 0;
							break;
						case BonusMalusAction.None:
							break;
						default:
							break;
					}
				}
			}
			return descriptionList;
		}

		public void DoRandomizeClassChaosMode(ref List<ClassData> classData, bool mixSpellsAndKeepPerm, MT19337 rng)
		{
			// Ranked list of equipment
			List<List<Item>> arArmor = new();
			arArmor.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>() });
			arArmor.Add(new List<Item> { Item.Cloth, Item.Copper, Item.Silver, Item.Gold, Item.Opal });
			arArmor.Add(new List<Item>(arArmor[2]) { Item.WoodenArmor });
			arArmor.Add(new List<Item>(arArmor[3]) { Item.ChainArmor, Item.SilverArmor });
			arArmor.Add(new List<Item>(arArmor[4]) { Item.IronArmor, Item.FlameArmor, Item.IceArmor, Item.SteelArmor });
			arArmor.Add(new List<Item>(arArmor[5]) { Item.DragonArmor, Item.OpalArmor });

			List<List<Item>> arShield = new();
			arShield.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>() });
			arShield.Add(new List<Item> { Item.ProCape });
			arShield.Add(new List<Item>(arShield[3]) { Item.Buckler });
			arShield.Add(new List<Item>(arShield[4]) { Item.WoodenShield, Item.IronShield, Item.SilverShield, Item.IceShield, Item.FlameShield });
			arShield.Add(new List<Item>(arShield[5]) { Item.OpalShield, Item.AegisShield });

			List<List<Item>> arHelmet = new();
			arHelmet.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>() });
			arHelmet.Add(new List<Item> { Item.Ribbon, Item.Cap });
			arHelmet.Add(new List<Item>(arHelmet[3]) { Item.WoodenHelm, Item.IronHelm, Item.SilverHelm });
			arHelmet.Add(new List<Item>(arHelmet[4]) { Item.HealHelm });
			arHelmet.Add(new List<Item>(arHelmet[5]) { Item.OpalHelm });

			List<List<Item>> arGauntlet = new();
			arGauntlet.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>() });
			arGauntlet.Add(new List<Item> { Item.Gloves, Item.ProRing });
			arGauntlet.Add(new List<Item>(arGauntlet[3]) { Item.CopperGauntlets, Item.IronGauntlets, Item.SilverGauntlets });
			arGauntlet.Add(new List<Item>(arGauntlet[4]) { Item.PowerGauntlets, Item.ZeusGauntlets });
			arGauntlet.Add(new List<Item>(arGauntlet[5]) { Item.OpalGauntlets });

			List<List<Item>> wpHammer = new();
			wpHammer.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>() });
			wpHammer.Add(new List<Item> { Item.IronHammer, Item.SilverHammer, Item.ThorHammer });

			List<List<Item>> wpStaff = new();
			wpStaff.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>() });
			wpStaff.Add(new List<Item> { Item.WoodenRod, Item.IronStaff, Item.PowerRod, Item.MageRod, Item.WizardRod, Item.HealRod });

			List<List<Item>> wpKnife = new();
			wpKnife.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>() });
			wpKnife.Add(new List<Item> { Item.SmallKnife, Item.LargeKnife, Item.SilverKnife, Item.CatClaw });

			List<List<Item>> wpNunchuck = new();
			wpNunchuck.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>() });
			wpNunchuck.Add(new List<Item> { Item.WoodenNunchucks, Item.IronNunchucks });

			List<List<Item>> wpAxe = new();
			wpAxe.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>() });
			wpAxe.Add(new List<Item> { Item.GreatAxe, Item.HandAxe, Item.LightAxe, Item.SilverAxe });

			List<List<Item>> wpSword = new();
			wpSword.AddRange(new List<List<Item>> { new List<Item>(), new List<Item>(), new List<Item>(), new List<Item>() });
			wpSword.Add(new List<Item> { Item.Rapier, Item.Scimitar, Item.Sabre, Item.Falchon, Item.RuneSword, Item.DragonSword, Item.CoralSword });
			wpSword.Add(new List<Item>(wpSword[4]) { Item.ShortSword, Item.LongSword, Item.SilverSword, Item.WereSword, Item.GiantSword, Item.FlameSword, Item.IceSword, Item.SunSword });
			wpSword.Add(new List<Item>(wpSword[5]) { Item.Vorpal, Item.BaneSword, Item.Defense });

			// Spell charge ranks to distribute
			List<Rank> startSpellcharges = new() { Rank.A, Rank.A, Rank.S, Rank.S, Rank.S, Rank.A };
			List<Rank> promoSpellcharges = new() { Rank.B, Rank.C, Rank.B, Rank.C, Rank.B, Rank.C };

			// Equipment ranks to distribute
			List<(RankedType, Rank)> startWeapons = new() { (RankedType.Swords, Rank.A), (RankedType.Swords, Rank.A), (RankedType.Swords, Rank.B), (RankedType.Nunchucks, Rank.S), (RankedType.Axes, Rank.S), (RankedType.Axes, Rank.S), (RankedType.Hammers, Rank.S), (RankedType.Hammers, Rank.S), (RankedType.Knives, Rank.S), (RankedType.Knives, Rank.S), (RankedType.Staves, Rank.S), (RankedType.Staves, Rank.S) };
			List<(RankedType, Rank)> promoWeapons = new() { (RankedType.Swords, Rank.S), (RankedType.Swords, Rank.S), (RankedType.Swords, Rank.S), (RankedType.Nunchucks, Rank.S), (RankedType.Axes, Rank.S), (RankedType.Hammers, Rank.S), (RankedType.Knives, Rank.S), (RankedType.Staves, Rank.S) };

			List<(RankedType, Rank)> startArmors = new() { (RankedType.Armors, Rank.A), (RankedType.Armors, Rank.B), (RankedType.Armors, Rank.C), (RankedType.Armors, Rank.C), (RankedType.Armors, Rank.D), (RankedType.Armors, Rank.D) };
			List<(RankedType, Rank)> promoArmors = new() { (RankedType.Armors, Rank.A), (RankedType.Armors, Rank.S) };

			List<(RankedType, Rank)> startShields = new() { (RankedType.Shields, Rank.A), (RankedType.Shields, Rank.B), (RankedType.Shields, Rank.B), (RankedType.Shields, Rank.C), (RankedType.Shields, Rank.C) };
			List<(RankedType, Rank)> promoShields = new() { (RankedType.Shields, Rank.A), (RankedType.Shields, Rank.S) };

			List<(RankedType, Rank)> startHelmets = new() { (RankedType.Helmets, Rank.B), (RankedType.Helmets, Rank.C), (RankedType.Helmets, Rank.C), (RankedType.Helmets, Rank.C), (RankedType.Helmets, Rank.C), (RankedType.Helmets, Rank.C) };
			List<(RankedType, Rank)> promoHelmets = new() { (RankedType.Helmets, Rank.A), (RankedType.Helmets, Rank.S) };

			List<(RankedType, Rank)> startGauntlets = new() { (RankedType.Gauntlets, Rank.B), (RankedType.Gauntlets, Rank.C), (RankedType.Gauntlets, Rank.C), (RankedType.Gauntlets, Rank.C), (RankedType.Gauntlets, Rank.C), (RankedType.Gauntlets, Rank.C) };
			List<(RankedType, Rank)> promoGauntlets = new() { (RankedType.Gauntlets, Rank.A), (RankedType.Gauntlets, Rank.S) };

			List<(Rank, Rank, int, int)> chargesRank = new()
			{
				(Rank.F, Rank.B, 0, 6),
				(Rank.F, Rank.B, 1, 7),
				(Rank.F, Rank.F, 2, 8),
				(Rank.A, Rank.A, 3, 9),
				(Rank.S, Rank.S, 4, 10),
				(Rank.S, Rank.S, 5, 11)
			};

			List<string> classBaseString = new() { "Fi", "Th", "Bb", "Rm", "Wm", "Bm" };
			List<string> classPromoString = new() { "Kn", "Ni", "Ma", "Rw", "Ww", "Bw" };

			// new arrays
			List<List<byte>> newChargeList = new();
			byte[] newMaxChargeList = Enumerable.Repeat((byte)0x00, 12).ToArray();

			// Get shuffle data 
			List<byte> shuffleStartingStats = new();
			List<List<bool>> shuffleLevelUp = new();
			List<List<bool>> shuffleHP = new();
			List<byte> shuffleHit = new();

			shuffleStartingStats.AddRange(classData.GetRange(0, 6).Select(x => x.StrStarting).ToList());
			shuffleStartingStats.AddRange(classData.GetRange(0, 6).Select(x => x.AgiStarting).ToList());
			shuffleStartingStats.AddRange(classData.GetRange(0, 6).Select(x => x.IntStarting).ToList());
			shuffleStartingStats.AddRange(classData.GetRange(0, 6).Select(x => x.VitStarting).ToList());
			shuffleStartingStats.AddRange(classData.GetRange(0, 6).Select(x => x.LckStarting).ToList());
			shuffleStartingStats.AddRange(classData.GetRange(0, 6).Select(x => x.HitStarting).ToList());
			shuffleStartingStats.AddRange(classData.GetRange(0, 6).Select(x => x.MDefStarting).ToList());
			shuffleLevelUp.AddRange(classData.GetRange(0, 6).Select(x => x.StrGrowth).ToList());
			shuffleLevelUp.AddRange(classData.GetRange(0, 6).Select(x => x.AgiGrowth).ToList());
			shuffleLevelUp.AddRange(classData.GetRange(0, 6).Select(x => x.IntGrowth).ToList());
			shuffleLevelUp.AddRange(classData.GetRange(0, 6).Select(x => x.VitGrowth).ToList());
			shuffleLevelUp.AddRange(classData.GetRange(0, 6).Select(x => x.LckGrowth).ToList());
			shuffleHP.AddRange(classData.GetRange(0, 6).Select(x => x.HpGrowth).ToList());
			shuffleHit.AddRange(classData.GetRange(0, 6).Select(x => x.HitGrowth).ToList());

			List<byte> mdefGrowthBase = classData.GetRange(0, 6).Select(x => x.MDefGrowth).ToList();
			List<byte> mdefGrowthPromo = classData.GetRange(6, 6).Select(x => x.MDefGrowth).ToList();

			List<KeyValuePair<byte, byte>> shuffleMDef = Enumerable.Zip(mdefGrowthBase, mdefGrowthPromo, (mdefGrowthBase, mdefGrowthPromo) => new KeyValuePair<byte, byte>(mdefGrowthBase, mdefGrowthPromo)).ToList();

			List<List<bool>> whitePermBase = classData.GetRange(0, 6).Select(x => x.WhitePermissions).ToList();
			List<List<bool>> whitePermPromo = classData.GetRange(6, 6).Select(x => x.WhitePermissions).ToList();
			List<List<bool>> blackPermBase = classData.GetRange(0, 6).Select(x => x.BlackPermissions).ToList();
			List<List<bool>> blackPermPromo = classData.GetRange(6, 6).Select(x => x.BlackPermissions).ToList();
			List<int> shuffleWhitePermissions = new() { 0, 1, 2, 3, 4, 5 };
			List<int> shuffleBlackPermissions = new() { 0, 1, 2, 3, 4, 5 };

			// Actual Shuffle
			shuffleStartingStats.Shuffle(rng);
			shuffleLevelUp.Shuffle(rng);
			shuffleHP.Shuffle(rng);
			shuffleHit.Shuffle(rng);
			shuffleMDef.Shuffle(rng);
			shuffleWhitePermissions.Shuffle(rng);
			if (mixSpellsAndKeepPerm)
			{
				shuffleBlackPermissions = shuffleWhitePermissions;
			}
			else
			{
				shuffleBlackPermissions.Shuffle(rng);
			}

			// Generate Ranks
			int maxStats = shuffleStartingStats.Max();
			int minStats = shuffleStartingStats.Min();
			int maxLvStats = shuffleLevelUp.Select(x => x.GetRange(0, 24).Where(y => y).Count()).Max();
			int minLvStats = shuffleLevelUp.Select(x => x.GetRange(0, 24).Where(y => y).Count()).Min();
			int spreadStats = (maxLvStats + maxStats - minLvStats - minStats) / 5;

			// For HP, max is a Lv25 Fighter average HP, min is a Lv25 Black Mage average HP
			int maxLvHp = 555;
			int minLvHp = 255;
			int spreadLvHp = (maxLvHp - minLvHp) / 4;

			List<Rank> statsRanks = new();
			List<Rank> hpRanks = new();
			Rank[] magicRanks = Enumerable.Repeat(Rank.F, 24).ToArray();

			for (int i = 0; i < shuffleLevelUp.Count(); i++)
			{
				if (shuffleStartingStats[i] + shuffleLevelUp[i].GetRange(0, 24).Where(x => x).Count() > (maxLvStats + maxStats - spreadStats))
				{
					statsRanks.Add(Rank.S);
				}
				else if (shuffleStartingStats[i] + shuffleLevelUp[i].GetRange(0, 24).Where(x => x).Count() > (maxLvStats + maxStats - (spreadStats * 2)))
				{
					statsRanks.Add(Rank.A);
				}
				else if (shuffleStartingStats[i] + shuffleLevelUp[i].GetRange(0, 24).Where(x => x).Count() > (maxLvStats + maxStats - (spreadStats * 3)))
				{
					statsRanks.Add(Rank.B);
				}
				else if (shuffleStartingStats[i] + shuffleLevelUp[i].GetRange(0, 24).Where(x => x).Count() > (maxLvStats + maxStats - (spreadStats * 4)))
				{
					statsRanks.Add(Rank.C);
				}
				else if (shuffleStartingStats[i] + shuffleLevelUp[i].GetRange(0, 24).Where(x => x).Count() > (maxLvStats + maxStats - (spreadStats * 5)))
				{
					statsRanks.Add(Rank.D);
				}
				else
				{
					statsRanks.Add(Rank.E);
				}
			}

			for (int i = 0; i < shuffleHP.Count(); i++)
			{
				int hpAverage25 = classData[i].HpStarting +
					(shuffleHP[i].GetRange(0, 24).Where(x => x).Count() * 23) +
					(((shuffleStartingStats[(i * 7) + 3] / 4) + (shuffleLevelUp[(i * 5) + 3].Where(x => x).Count() / 8)) * 24);

				if (hpAverage25 > maxLvHp)
				{
					hpRanks.Add(Rank.S);
				}
				else if (hpAverage25 > (maxLvHp - spreadLvHp))
				{
					hpRanks.Add(Rank.A);
				}
				else if (hpAverage25 > (maxLvHp - (spreadLvHp * 2)))
				{
					hpRanks.Add(Rank.B);
				}
				else if (hpAverage25 > (maxLvHp - (spreadLvHp * 3)))
				{
					hpRanks.Add(Rank.C);
				}
				else if (hpAverage25 > (maxLvHp - (spreadLvHp * 4)))
				{
					hpRanks.Add(Rank.D);
				}
				else
				{
					hpRanks.Add(Rank.E);
				}
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
					default:
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
					default:
						break;
				}
			}

			// Update data
			for (int i = 0; i < 6; i++)
			{
				classData[i].GetStartingStats(shuffleStartingStats.GetRange(i * 7, 7), statsRanks.GetRange(i * 5, 5));
				classData[i + 6].GetStartingStats(shuffleStartingStats.GetRange(i * 7, 7), statsRanks.GetRange(i * 5, 5));

				classData[i].GetLevelUp(shuffleLevelUp.GetRange(i * 5, 5));
				classData[i + 6].GetLevelUp(shuffleLevelUp.GetRange(i * 5, 5));

				classData[i].HpGrowth = shuffleHP[i];
				classData[i + 6].HpGrowth = shuffleHP[i];
				classData[i].Ranks[(int)RankedType.HP] = hpRanks[i];
				classData[i + 6].Ranks[(int)RankedType.HP] = hpRanks[i];

				classData[i].HitGrowth = shuffleHit[i];
				classData[i + 6].HitGrowth = shuffleHit[i];
				classData[i].MDefGrowth = shuffleMDef[i].Key;
				classData[i + 6].MDefGrowth = shuffleMDef[i].Value;

				classData[i].WhitePermissions = whitePermBase[shuffleWhitePermissions[i]];
				classData[i + 6].WhitePermissions = whitePermPromo[shuffleWhitePermissions[i]];
				classData[i].Ranks[(int)RankedType.White] = magicRanks[i];
				if (magicRanks[i] > Rank.F)
				{
					classData[i].MagicRanks[0] = classBaseString[shuffleWhitePermissions[i]];
				}

				classData[i + 6].Ranks[(int)RankedType.White] = magicRanks[i + 6];
				if (magicRanks[i + 6] > Rank.F)
				{
					classData[i + 6].MagicRanks[0] = classPromoString[shuffleWhitePermissions[i]];
				}

				classData[i].BlackPermissions = blackPermBase[shuffleBlackPermissions[i]];
				classData[i + 6].BlackPermissions = blackPermPromo[shuffleBlackPermissions[i]];
				classData[i].Ranks[(int)RankedType.Black] = magicRanks[i + 12];
				if (magicRanks[i + 12] > Rank.F)
				{
					classData[i].MagicRanks[1] = classBaseString[shuffleBlackPermissions[i]];
				}

				classData[i + 6].Ranks[(int)RankedType.Black] = magicRanks[i + 18];
				if (magicRanks[i + 18] > Rank.F)
				{
					classData[i + 6].MagicRanks[1] = classPromoString[shuffleBlackPermissions[i]];
				}
			}


			// Shuffle spell charges, we do this after shuffling spells
			//  because we want to give spell charges to actual spellcasters
			startSpellcharges.Shuffle(rng);
			promoSpellcharges.Shuffle(rng);
			List<List<byte>> chargeList = classData.GetRange(0, 12).Select(x => x.SpCGrowth).ToList();
			List<byte> maxCharges = classData.GetRange(0, 12).Select(x => x.MaxSpC).ToList();

			for (int i = 0; i < 6; i++)
			{
				if (classData[i].Ranks[(int)RankedType.White] > Rank.F || classData[i].Ranks[(int)RankedType.Black] > Rank.F)
				{
					int tempClass = chargesRank.Find(x => x.Item1 == startSpellcharges.First()).Item3;
					classData[i].Ranks[(int)RankedType.Charges] = startSpellcharges.First();
					classData[i + 6].Ranks[(int)RankedType.Charges] = startSpellcharges.First();
					classData[i].MagicRanks[2] = classBaseString[tempClass];
					classData[i + 6].MagicRanks[2] = classPromoString[tempClass];
					classData[i].SpCGrowth = chargeList[tempClass].ToList();
					classData[i].MaxSpC = maxCharges[tempClass];
					classData[i + 6].MaxSpC = maxCharges[tempClass + 6];
					classData[i].SpCStarting = 0x02;
					classData[i + 6].SpCStarting = 0x02;
					startSpellcharges.RemoveRange(0, 1);
				}
				else if (classData[i + 6].Ranks[(int)RankedType.Black] > Rank.F)
				{
					int tempClass = 1;
					classData[i].Ranks[(int)RankedType.Charges] = Rank.F;
					classData[i + 6].Ranks[(int)RankedType.Charges] = Rank.B;
					classData[i + 6].MagicRanks[2] = classPromoString[tempClass];
					classData[i].SpCGrowth = chargeList[tempClass].ToList();
					classData[i + 6].SpCGrowth = chargeList[tempClass].ToList();
					classData[i].MaxSpC = maxCharges[tempClass];
					classData[i + 6].MaxSpC = maxCharges[tempClass + 6];
					classData[i].SpCStarting = 0x00;
					classData[i + 6].SpCStarting = 0x00;
					promoSpellcharges.RemoveRange(0, 1);
				}
				else if (classData[i + 6].Ranks[(int)RankedType.White] > Rank.F)
				{
					int tempClass = 0;
					classData[i].Ranks[(int)RankedType.Charges] = Rank.F;
					classData[i + 6].Ranks[(int)RankedType.Charges] = Rank.B;
					classData[i + 6].MagicRanks[2] = classPromoString[tempClass];
					classData[i].SpCGrowth = chargeList[tempClass].ToList();
					classData[i + 6].SpCGrowth = chargeList[tempClass].ToList();
					classData[i].MaxSpC = maxCharges[tempClass];
					classData[i + 6].MaxSpC = maxCharges[tempClass + 6];
					classData[i].SpCStarting = 0x00;
					classData[i + 6].SpCStarting = 0x00;
					promoSpellcharges.RemoveRange(0, 1);
				}
				else
				{
					classData[i].Ranks[(int)RankedType.Charges] = Rank.F;
					classData[i + 6].Ranks[(int)RankedType.Charges] = Rank.F;
					classData[i].MaxSpC = 0x00;
					classData[i + 6].MaxSpC = 0x00;
					classData[i].SpCStarting = 0x00;
					classData[i + 6].SpCStarting = 0x00;
				}
			}

			// Distribute equipment permissions
			foreach ((RankedType, Rank) x in startWeapons)
			{
				int select = Rng.Between(rng, 0, 5);

				while (classData[select].Ranks[(int)x.Item1] >= x.Item2)
				{
					select = Rng.Between(rng, 0, 5);
				}

				classData[select].Ranks[(int)x.Item1] = x.Item2;
				classData[select + 6].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach ((RankedType, Rank) x in promoWeapons)
			{
				int select = Rng.Between(rng, 6, 11);

				while (classData[select].Ranks[(int)x.Item1] >= x.Item2)
				{
					select = Rng.Between(rng, 6, 11);
				}

				classData[select].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach ((RankedType, Rank) x in startArmors)
			{
				int select = Rng.Between(rng, 0, 5);

				while (classData[select].Ranks[(int)x.Item1] >= x.Item2)
				{
					select = Rng.Between(rng, 0, 5);
				}

				classData[select].Ranks[(int)x.Item1] = x.Item2;
				classData[select + 6].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach ((RankedType, Rank) x in promoArmors)
			{
				int select = Rng.Between(rng, 6, 11);

				while (classData[select].Ranks[(int)x.Item1] >= x.Item2)
				{
					select = Rng.Between(rng, 6, 11);
				}

				classData[select].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach ((RankedType, Rank) x in startShields)
			{
				int select = Rng.Between(rng, 0, 5);

				while (classData[select].Ranks[(int)x.Item1] >= x.Item2)
				{
					select = Rng.Between(rng, 0, 5);
				}

				classData[select].Ranks[(int)x.Item1] = x.Item2;
				classData[select + 6].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach ((RankedType, Rank) x in promoShields)
			{
				int select = Rng.Between(rng, 6, 11);

				while (classData[select].Ranks[(int)x.Item1] >= x.Item2)
				{
					select = Rng.Between(rng, 6, 11);
				}

				classData[select].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach ((RankedType, Rank) x in startHelmets)
			{
				int select = Rng.Between(rng, 0, 5);

				while (classData[select].Ranks[(int)x.Item1] >= x.Item2)
				{
					select = Rng.Between(rng, 0, 5);
				}

				classData[select].Ranks[(int)x.Item1] = x.Item2;
				classData[select + 6].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach ((RankedType, Rank) x in promoHelmets)
			{
				int select = Rng.Between(rng, 6, 11);

				while (classData[select].Ranks[(int)x.Item1] >= x.Item2)
				{
					select = Rng.Between(rng, 6, 11);
				}

				classData[select].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach ((RankedType, Rank) x in startGauntlets)
			{
				int select = Rng.Between(rng, 0, 5);

				while (classData[select].Ranks[(int)x.Item1] >= x.Item2)
				{
					select = Rng.Between(rng, 0, 5);
				}

				classData[select].Ranks[(int)x.Item1] = x.Item2;
				classData[select + 6].Ranks[(int)x.Item1] = x.Item2;
			}

			foreach ((RankedType, Rank) x in promoGauntlets)
			{
				int select = Rng.Between(rng, 6, 11);

				while (classData[select].Ranks[(int)x.Item1] >= x.Item2)
				{
					select = Rng.Between(rng, 6, 11);
				}

				classData[select].Ranks[(int)x.Item1] = x.Item2;
			}

			// Add individual equipment for each equipment right
			for (int i = 0; i < 12; i++)
			{
				classData[i].WeaponPermissions.Clear();
				classData[i].WeaponPermissions.AddRange(wpSword[(int)classData[i].Ranks[(int)RankedType.Swords]]);
				classData[i].WeaponPermissions.AddRange(wpHammer[(int)classData[i].Ranks[(int)RankedType.Hammers]]);
				classData[i].WeaponPermissions.AddRange(wpKnife[(int)classData[i].Ranks[(int)RankedType.Knives]]);
				classData[i].WeaponPermissions.AddRange(wpAxe[(int)classData[i].Ranks[(int)RankedType.Axes]]);
				classData[i].WeaponPermissions.AddRange(wpStaff[(int)classData[i].Ranks[(int)RankedType.Staves]]);
				classData[i].WeaponPermissions.AddRange(wpNunchuck[(int)classData[i].Ranks[(int)RankedType.Nunchucks]]);
				classData[i].WeaponPermissions.Add(Item.Masamune);
				classData[i].ArmourPermissions.Clear();
				classData[i].ArmourPermissions.AddRange(arArmor[(int)classData[i].Ranks[(int)RankedType.Armors]]);
				classData[i].ArmourPermissions.AddRange(arShield[(int)classData[i].Ranks[(int)RankedType.Shields]]);
				classData[i].ArmourPermissions.AddRange(arHelmet[(int)classData[i].Ranks[(int)RankedType.Helmets]]);
				classData[i].ArmourPermissions.AddRange(arGauntlet[(int)classData[i].Ranks[(int)RankedType.Gauntlets]]);
			}

			// Add class exclusive equipment
			classData[6].WeaponPermissions.Add(Item.Xcalber);
			classData[7].WeaponPermissions.Add(Item.Katana);
			classData[10].ArmourPermissions.Add(Item.WhiteShirt);
			classData[11].ArmourPermissions.Add(Item.BlackShirt);

		}
	}
}
