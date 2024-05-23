using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public partial class GameClasses
	{
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

	}
}
