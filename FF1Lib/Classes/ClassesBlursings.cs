using FF1Lib.Helpers;
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
		public List<string> DoRandomizeClassNormalMode(MT19337 rng, List<string> olditemnames, ItemNames itemnames, Flags flags, FF1Rom rom)
		{
			// Hardcoded spell lists and MP Growth lists for trickier blessings
			var wmWhiteSpells = _spellPermissions[Classes.WhiteMage].ToList();
			var bmBlackSpells = _spellPermissions[Classes.BlackMage].ToList();

			var wwWhiteSpells = _spellPermissions[Classes.WhiteWizard].ToList();
			var bwBlackSpells = _spellPermissions[Classes.BlackWizard].ToList();

			var exKnightMPlist = new List<byte> { 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07,
				0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00, 0x07, 0x00 };
			var exNinjaMPlist = new List<byte> { 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F,
				0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00, 0x0F, 0x00 };

			// Blursings List
			List<BonusMalus> baseTierBlessings = new();
			Dictionary<Classes, List<BonusMalus>> classBlessings = new()
			{
				{ Classes.Fighter, new() },
				{ Classes.Thief, new() },
				{ Classes.BlackBelt, new() },
				{ Classes.RedMage, new() },
				{ Classes.WhiteMage, new() },
				{ Classes.BlackMage, new() },
			};
			List<BonusMalus> maluses = new();

			int maxbonuses = flags.RandomizeClassMaxBonus;
			int maxmaluses = flags.RandomizeClassMaxMalus;
			var descriptionList = new List<string>();

			// Generate Blursings List
			GenerateLists(baseTierBlessings, classBlessings, maluses, olditemnames, itemnames, flags, rng, rom);

			// Distribute bonuses and maluses
			bool validBlursingsDistribution = false;

			var startWithKiBlurses = StartWithKeyItems(flags, rng, olditemnames);

			Dictionary<Classes, List<BonusMalus>> assignedBlessings = new();
			Dictionary<Classes, List<BonusMalus>> assignedMaluses = new();

			List<Classes> classList = Enumerable.Range(0, 6).Select(c => (Classes)c).ToList();
			List<Classes> validClasses = new();

			while (!validBlursingsDistribution)
			{
				validBlursingsDistribution = true;

				Dictionary<Classes, bool> classBlessingsAvailable = classList.ToDictionary(c => c, c => true);
				assignedBlessings = classList.ToDictionary(c => c, c => new List<BonusMalus>());
				assignedMaluses = classList.ToDictionary(c => c, c => new List<BonusMalus>());

				List<BonusMalus> placedBlursings = new();
				validClasses = new(classList);

				// Apply Start With Key Items blessings, doesn't count toward the max number of blessings
				if ((bool)flags.RandomizeClassKeyItems)
				{
					maxbonuses = (maxbonuses > 0) ? maxbonuses + 1 : 0;
					
					foreach (var gameclass in validClasses)
					{
						assignedBlessings[gameclass].Add(startWithKiBlurses[(int)gameclass]);
					}
				}

				// Apply all other blessings
				while (maxbonuses > 0 && validClasses.Any())
				{
					// pick a random class for fairness
					var currentClass = validClasses.PickRandom(rng);

					// 25% to pick from the 5 exlclusive blessing list, giving a 5% chance per seed per blessing
					bool pickClassBlessing = classBlessingsAvailable[currentClass] && rng.Between(1, 4) == 1;

					if (pickClassBlessing)
					{
						var validBlessings = classBlessings[currentClass].Where(x => !assignedBlessings[currentClass].Select(y => y.Action).ToList().Contains(x.Action)).ToList();

						if (validBlessings.Any())
						{
							var pickedBlursing = validBlessings.PickRandom(rng);
							assignedBlessings[currentClass].Add(pickedBlursing);
						}
						classBlessingsAvailable[currentClass] = false;
					}
					else
					{
						var validBlessings = baseTierBlessings.Where(x => !placedBlursings.Contains(x) && x.ClassList.Contains(currentClass) && !assignedBlessings[currentClass].Select(y => y.Action).ToList().Contains(x.Action)).ToList();

						// There's no good blessings left for this class, reset and try again
						if (!validBlessings.Any())
						{
							validBlursingsDistribution = false;
							break;
						}

						var pickedBlursing = validBlessings.PickRandom(rng);
						assignedBlessings[currentClass].Add(pickedBlursing);
						placedBlursings.Add(pickedBlursing);
					}

					if (assignedBlessings[currentClass].Count >= maxbonuses)
					{
						validClasses.Remove(currentClass);
					}
				}

				// No need to continue to maluses if we don't have a valid distribution
				if (!validBlursingsDistribution)
				{
					continue;
				}

				validClasses = new(classList);

				// Distribute maluses
				while (maxmaluses > 0 && validClasses.Any())
				{
					var currentClass = validClasses.PickRandom(rng);

					var validMaluses = maluses.Where(x => !placedBlursings.Contains(x) &&
						x.ClassList.Contains(currentClass) &&
						!assignedBlessings[currentClass].Select(y => y.Action).ToList().Contains(x.Action) &&
						!assignedMaluses[currentClass].Select(y => y.Action).ToList().Contains(x.Action) &&
						!(x.Action == BonusMalusAction.CantLearnSpell && assignedBlessings[currentClass].Where(y => y.Action == BonusMalusAction.InnateSpells).SelectMany(x => x.SpellsMod).ToList().Contains(x.SpellSlotMod)) &&
						!(x.Action == BonusMalusAction.NoPromoMagic && assignedBlessings[currentClass].Where(y => y.Action == BonusMalusAction.MpGainOnMaxMpGain).Any()) &&
						!(x.Action == BonusMalusAction.RibbonCurse && assignedBlessings[currentClass].Where(y => y.Action == BonusMalusAction.InnateResist && y.StatMod == 0xFF).Any())
						).ToList();

					if (!validMaluses.Any())
					{
						validBlursingsDistribution = false;
						break;
					}

					var pickedBlursing = validMaluses.PickRandom(rng);
					assignedMaluses[currentClass].Add(pickedBlursing);
					placedBlursings.Add(pickedBlursing);

					if (assignedMaluses[currentClass].Count >= maxmaluses)
					{
						validClasses.Remove(currentClass);
					}
				}
			}

			// Generate Descriptions
			validClasses = new() { Classes.Fighter, Classes.Thief, Classes.BlackBelt, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage };

			foreach (var gameclass in validClasses)
			{

				var blessingsString = string.Join("\n\n", assignedBlessings[gameclass].Select(b => b.Description).ToList());
				var malusesString = string.Join("\n\n", assignedMaluses[gameclass].Select(b => b.Description).ToList());

				descriptionList.Add(blessingsString + "\n\n\nMALUS\n\n" + malusesString);
			}

			// Apply bonuses and maluses to stats
			for (int i = 0; i < 6; i++)
			{
				// Order the list so bonuses/maluses interact correctly
				List<BonusMalusAction> priorityAction = new() { BonusMalusAction.SpcMax, BonusMalusAction.CantLearnSpell };

				List<BonusMalus> assignedBonusMalus = assignedBlessings[(Classes)i].Concat(assignedMaluses[(Classes)i]).ToList();

				assignedBonusMalus.Reverse();

				assignedBonusMalus = assignedBonusMalus
					.Where(x => !priorityAction.Contains(x.Action))
					.ToList()
					.Concat(assignedBonusMalus.Where(x => priorityAction.Contains(x.Action)).ToList())
					.ToList();

				foreach (var bonusmalus in assignedBonusMalus)
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
							if (bonusmalus.StatMod2 != 99)
							{
								_classes[i].MDefGrowth = (byte)Math.Max(_classes[i].MDefGrowth + bonusmalus.StatMod, 0);
							}
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
							if ((byte)bonusmalus.StatMod != 99)
							{
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
							if ((byte)bonusmalus.StatMod != 99)
							{
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
							if ((byte)bonusmalus.StatMod == 1)
							{
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
							_classes[i].ToggleBlursings.Add(BonusMalusAction.StartWithMp);
							_classes[i + 6].ToggleBlursings.Add(BonusMalusAction.StartWithMp);
							break;
						case BonusMalusAction.Hunter:
							_classes[i].HurtType |= (byte)bonusmalus.StatMod;
							_classes[i + 6].HurtType |= (byte)bonusmalus.StatMod;
							break;
						case BonusMalusAction.UnarmedAttack:
							if (bonusmalus.StatMod == 99)
							{
								_classes[i].UnarmedAttack = false;
							}
							else
							{
								_classes[i].UnarmedAttack = true;
							}
							_classes[i + 6].UnarmedAttack = true;
							break;
						case BonusMalusAction.ThorMaster:
							_classes[i].ToggleBlursings.Add(BonusMalusAction.ThorMaster);
							_classes[i + 6].ToggleBlursings.Add(BonusMalusAction.ThorMaster);
							_weaponPermissions.AddPermissionsRange(new List<(Classes, Item)> { ((Classes)(i), Item.ThorHammer) });
							_weaponPermissions.AddPermissionsRange(new List<(Classes, Item)> { ((Classes)(i + 6), Item.ThorHammer) });
							break;
						case BonusMalusAction.CatClawMaster:
							_classes[i].ToggleBlursings.Add(BonusMalusAction.CatClawMaster);
							_classes[i + 6].ToggleBlursings.Add(BonusMalusAction.CatClawMaster);
							_weaponPermissions.AddPermissionsRange(new List<(Classes, Item)> { ((Classes)(i), Item.CatClaw) });
							_weaponPermissions.AddPermissionsRange(new List<(Classes, Item)> { ((Classes)(i + 6), Item.CatClaw) });
							break;
						case BonusMalusAction.SteelLord:
							_classes[i].ToggleBlursings.Add(BonusMalusAction.SteelLord);
							_classes[i + 6].ToggleBlursings.Add(BonusMalusAction.SteelLord);
							break;
						case BonusMalusAction.WoodAdept:
							_classes[i].ToggleBlursings.Add(BonusMalusAction.WoodAdept);
							_classes[i + 6].ToggleBlursings.Add(BonusMalusAction.WoodAdept);
							_armorPermissions.AddPermissionsRange(new List<(Classes, Item)> { ((Classes)i, Item.WoodenArmor), ((Classes)i, Item.WoodenHelm), ((Classes)i, Item.WoodenShield), ((Classes)(i + 6), Item.WoodenArmor), ((Classes)(i + 6), Item.WoodenHelm), ((Classes)(i + 6), Item.WoodenShield) });
							break;
						case BonusMalusAction.StartWithKI:
							_classes[i].StartingKeyItem = (Item)bonusmalus.StatMod;
							_classes[i + 6].StartingKeyItem = (Item)bonusmalus.StatMod;
							break;
						case BonusMalusAction.InnateSpells:
							_classes[i].InnateSpells = bonusmalus.SpellsMod.ToList();
							_classes[i + 6].InnateSpells = bonusmalus.SpellsMod.ToList();
							break;
						case BonusMalusAction.DualWieldKnife:
							_classes[i].ToggleBlursings.Add(BonusMalusAction.DualWieldKnife);
							_classes[i + 6].ToggleBlursings.Add(BonusMalusAction.DualWieldKnife);
							break;
						case BonusMalusAction.LearnLampRibbon:
							_classes[i].ToggleBlursings.Add(BonusMalusAction.LearnLampRibbon);
							_classes[i + 6].ToggleBlursings.Add(BonusMalusAction.LearnLampRibbon);
							break;
						case BonusMalusAction.LearDarkEvade:
							_classes[i].ToggleBlursings.Add(BonusMalusAction.LearDarkEvade);
							_classes[i + 6].ToggleBlursings.Add(BonusMalusAction.LearDarkEvade);
							break;
						case BonusMalusAction.LearnSleepMDef:
							_classes[i].ToggleBlursings.Add(BonusMalusAction.LearnSleepMDef);
							_classes[i + 6].ToggleBlursings.Add(BonusMalusAction.LearnSleepMDef);
							break;
						case BonusMalusAction.LearnSlowAbsorb:
							_classes[i].ToggleBlursings.Add(BonusMalusAction.LearnSlowAbsorb);
							_classes[i + 6].ToggleBlursings.Add(BonusMalusAction.LearnSlowAbsorb);
							break;
						case BonusMalusAction.ASpellsAutocast:
							_classes[i].ToggleBlursings.Add(BonusMalusAction.ASpellsAutocast);
							_classes[i + 6].ToggleBlursings.Add(BonusMalusAction.ASpellsAutocast);
							break;
						case BonusMalusAction.MasaCurse:
							_classes[i].MasaCurse = (byte)bonusmalus.StatMod;
							_classes[i + 6].MasaCurse = (byte)bonusmalus.StatMod;
							break;
						case BonusMalusAction.RibbonCurse:
							_classes[i].RibbonCurse = (byte)bonusmalus.StatMod;
							_classes[i + 6].RibbonCurse = (byte)bonusmalus.StatMod;
							break;
					}
				}
			}

			return descriptionList;
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
}
