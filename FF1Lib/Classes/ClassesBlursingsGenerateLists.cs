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


		public void GenerateLists(List<BonusMalus> baseTier, Dictionary<Classes, List<BonusMalus>> classesBlessingsLists, List<BonusMalus> maluses, List<string> olditemnames, ItemNames itemnames, Flags flags, MT19337 rng, FF1Rom rom)
		{
			// Equipment lists
			List<Item> braceletList = new();
			List<Item> ringList = new();
			for (int i = (int)Item.Cloth; i <= (int)Item.ProRing; i++)
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
			for (int i = (int)Item.Cloth; i <= (int)Item.ProRing; i++)
			{
				if (itemnames[i].Contains("@T"))
				{
					equipShirts.Add((Item)i);
				}
			}
			List<Item> equipShields = new();
			for (int i = (int)Item.Cloth; i <= (int)Item.ProRing; i++)
			{
				if (itemnames[i].Contains("@s") || itemnames[i].Contains("Buckl") || itemnames[i].Contains("ProCa"))
				{
					equipShields.Add((Item)i);
				}
			}
			List<Item> equipGauntletsHelmets = new();
			for (int i = (int)Item.Cloth; i <= (int)Item.ProRing; i++)
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

			// Base Tier list
			baseTier.AddRange(new List<BonusMalus>()
			{
				new BonusMalus(BonusMalusAction.StrMod, "+10 Str.", mod: 10),
				new BonusMalus(BonusMalusAction.AgiMod, "+15 Agi.", mod: 15),
				new BonusMalus(BonusMalusAction.VitMod, "+10 Vit.", mod: 10),
				new BonusMalus(BonusMalusAction.LckMod, "+5 Luck", mod: 5),
				new BonusMalus(BonusMalusAction.HpMod, "+20 HP", mod: 20),
				new BonusMalus(BonusMalusAction.HitMod, "+10 Hit%", mod: 10, Classes: hitBonusClass ),
				new BonusMalus(BonusMalusAction.MDefMod, "+10 MDef", mod: 10),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+Equip @X", equipment: equipAxes, Classes: new List<Classes> { Classes.Thief, Classes.BlackBelt, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage } ),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Equip @T", equipment: equipShirts),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Equip @s", equipment: equipShields, Classes: new List<Classes> { Classes.Thief, Classes.BlackBelt, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage } ),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Equip @G+@h", equipment: equipGauntletsHelmets, Classes: new List<Classes> { Classes.Thief, Classes.BlackBelt, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage } ),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+Thief @S", equipment: equipThiefWeapon, Classes: new List<Classes> { Classes.BlackBelt, Classes.WhiteMage, Classes.BlackMage } ),
				new BonusMalus(BonusMalusAction.SpcMod, "+2 Lv1 MP", mod: 2, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }),
				new BonusMalus(BonusMalusAction.StrMod, "+20 Str.", mod: 20),
				new BonusMalus(BonusMalusAction.AgiMod, "+25 Agi.", mod: 25),
				new BonusMalus(BonusMalusAction.VitMod, "+20 Vit.", mod: 20),
				new BonusMalus(BonusMalusAction.LckMod, "+10 Luck", mod: 10),
				new BonusMalus(BonusMalusAction.HpMod, "+40 HP", mod: 40),
				new BonusMalus(BonusMalusAction.HitMod, "+20 Hit%", mod: 20, Classes: hitBonusClass ),
				new BonusMalus(BonusMalusAction.MDefMod, "+20 MDef", mod: 20),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+Legendary@S", equipment: equipLegendaryWeapons),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Red Mage @A", equipment: equipRedMageArmor, Classes: new List<Classes> { Classes.Thief, Classes.BlackBelt, Classes.WhiteMage, Classes.BlackMage } ),
				new BonusMalus(BonusMalusAction.StartWithMp, "+1 MP LvAll", Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }),
				new BonusMalus(BonusMalusAction.ThorMaster, "Improved\n Thor@H", equipment: new List<Item>() { Item.ThorHammer }, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.WhiteMage }),
				new BonusMalus(BonusMalusAction.Hunter, "Hurt Undead", mod: 0x18),
				new BonusMalus(BonusMalusAction.Hunter, "Hurt Dragon", mod: 0x02),
				new BonusMalus(BonusMalusAction.InnateResist, "Res. PEDTS", mod: (int)(SpellElement.Poison | SpellElement.Earth | SpellElement.Death | SpellElement.Time | SpellElement.Status), Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.RedMage, Classes.BlackMage, Classes.WhiteMage }),
				CreateRandomResistBonusMalus(rng),
				CreateRandomResistBonusMalus(rng),
			});

			// High tier list, should be kept at max 20 blessings or so
			List<BonusMalus> highTier = new();

			highTier.AddRange(new List<BonusMalus>()
			{
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
				new BonusMalus(BonusMalusAction.InnateResist, "Res. All", mod: 0xFF, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.RedMage, Classes.BlackMage, Classes.WhiteMage })
			});

			// Base Malus List
			maluses.AddRange(new List<BonusMalus>()
			{
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

			});

			// Conditional Blursings
			if ((bool)flags.IntAffectsSpells)
			{
				baseTier.Add(new BonusMalus(BonusMalusAction.IntMod, "+10 Int.", mod: 10, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));
				baseTier.Add(new BonusMalus(BonusMalusAction.IntMod, "+20 Int.", mod: 20, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));
				highTier.Add(new BonusMalus(BonusMalusAction.IntMod, "+40 Int.", mod: 40, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));

				maluses.Add(new BonusMalus(BonusMalusAction.IntMod, "-10 Int.", mod: -10, Classes: new List<Classes> { Classes.Thief, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));
				maluses.Add(new BonusMalus(BonusMalusAction.IntMod, "-20 Int.", mod: -20, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));
			}

			if (!(bool)flags.Weaponizer)
			{
				baseTier.Add(new BonusMalus(BonusMalusAction.CatClawMaster, "Improved\n CatClaw", equipment: new List<Item>() { Item.CatClaw }, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.RedMage, Classes.BlackMage }));
				highTier.Add(new BonusMalus(BonusMalusAction.DualWieldKnife, "DualWield @K", Classes: new List<Classes> { Classes.Thief }));
			}

			// Ceate Spell Learning Blessings
			if (!(bool)flags.GenerateNewSpellbook)
			{
				highTier.AddRange(CreateSpellLearningBlessings(rom));
			}

			// Negative amounts are processed separately in ProcessStartWithRoutines, because they affect the Assembly code
			// If changing the Malus gold labels below, change those as well to alter the actual number used
			if (flags.StartingGold == StartingGold.None)
			{
				baseTier.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+200 GP", mod: 2));
				highTier.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+1400 GP", mod: 14, Classes: new List<Classes> { Classes.Thief }));
			}

			else if (flags.StartingGold == StartingGold.Gp100)
			{
				baseTier.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+400 GP", mod: 4));
				highTier.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+2000 GP", mod: 20, Classes: new List<Classes> { Classes.Thief }));
				maluses.Add(new BonusMalus(BonusMalusAction.StartWithGold, "-50 GP", mod: -1));
			}
			else if (flags.StartingGold == StartingGold.Gp200)
			{
				baseTier.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+600 GP", mod: 6));
				highTier.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+3000 GP", mod: 30, Classes: new List<Classes> { Classes.Thief }));
				maluses.Add(new BonusMalus(BonusMalusAction.StartWithGold, "-100 GP", mod: -1));
			}
			else if (flags.StartingGold == StartingGold.Gp400 || flags.StartingGold == StartingGold.RandomLow)
			{
				baseTier.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+800 GP", mod: 8));
				highTier.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+4000 GP", mod: 40, Classes: new List<Classes> { Classes.Thief }));
				maluses.Add(new BonusMalus(BonusMalusAction.StartWithGold, "-150 GP", mod: -1));
			}
			else if (flags.StartingGold == StartingGold.Gp800)
			{
				baseTier.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+1500 GP", mod: 15));
				highTier.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+6000 GP", mod: 60, Classes: new List<Classes> { Classes.Thief }));
				maluses.Add(new BonusMalus(BonusMalusAction.StartWithGold, "-350 GP", mod: -1));
			}
			else if (flags.StartingGold == StartingGold.Gp2500)
			{
				baseTier.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+5000 GP", mod: 50));
				highTier.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+20,000 GP", mod: 200, Classes: new List<Classes> { Classes.Thief }));
				maluses.Add(new BonusMalus(BonusMalusAction.StartWithGold, "-1100 GP", mod: -1));
			}
			else if (flags.StartingGold == StartingGold.Gp9999)
			{
				baseTier.Add(new BonusMalus(BonusMalusAction.StartWithGold, "+20,000 GP", mod: 200));
				// Strong bonus doesn't make sense with gold already so high, so isn't created
				maluses.Add(new BonusMalus(BonusMalusAction.StartWithGold, "-4500 GP", mod: -1));
			}
			// These are so much starting gold that bonuses for it no longer make sense
			//else if (flags.StartingGold == StartingGold.Gp65535 || flags.StartingGold == StartingGold.RandomHigh)

			List<(string name, byte value)> ailments = new()
			{
				("Poison", 0x04),
				("Stun", 0x10),
				("Sleep", 0x20),
				("Mute", 0x40),
			};

			if (!(bool)flags.NoMasamune)
			{
				var masastatus = ailments.PickRandom(rng);
				maluses.Add(new BonusMalus(BonusMalusAction.MasaCurse, "Masa Curse\n " + masastatus.name, mod: masastatus.value));
			}

			if (flags.RibbonMode == RibbonMode.Vanilla)
			{
				var ribbonstatus = ailments.PickRandom(rng);
				maluses.Add(new BonusMalus(BonusMalusAction.RibbonCurse, "Ribbn Curse\n " + ribbonstatus.name, mod: ribbonstatus.value));
			}



			// Do not add Promo-based blursings if there is no ability to promote
			if (!((bool)flags.NoTail && !(bool)flags.FightBahamut))
			{
				baseTier.Add(new BonusMalus(BonusMalusAction.ArmorAdd, "Promo FI @A", mod: 99, equipment: equipFighterArmor, Classes: new List<Classes> { Classes.BlackBelt, Classes.WhiteMage, Classes.BlackMage, Classes.RedMage }));
				highTier.Add(new BonusMalus(BonusMalusAction.PowerRW, "Promo Sage", mod: 0, spelllist: wmWhiteSpells.Concat(bmBlackSpells).Concat(wwWhiteSpells).Concat(bwBlackSpells).ToList(), Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));

				maluses.Add(new BonusMalus(BonusMalusAction.ArmorReplace, "No Promo @A", mod: 99, equipment: equipFighterArmorFull, Classes: new List<Classes> { Classes.Fighter }));
				maluses.Add(new BonusMalus(BonusMalusAction.ArmorReplace, "Promo RW @A", mod: 99, equipment: equipRedWizardArmorFull, Classes: new List<Classes> { Classes.Thief }));
				maluses.Add(new BonusMalus(BonusMalusAction.NoPromoMagic, "No Promo Sp", mod: 0, mod2: 0, binarylist: nullSpells, Classes: new List<Classes> { Classes.Fighter, Classes.Thief }));
				maluses.Add(new BonusMalus(BonusMalusAction.UnarmedAttack, "Promo\n Unarmed", mod: 99, Classes: new List<Classes> { Classes.BlackBelt }));

				if (flags.MDefMode == MDEFGrowthMode.None)
				{
					highTier.Add(new BonusMalus(BonusMalusAction.MDefGrowth, "Promo\n +3 MDef", mod: 3, mod2: 99, Classes: new List<Classes> { Classes.BlackBelt }));
				}
			}

			if (!(bool)flags.ArmorCrafter)
			{
				baseTier.Add(new BonusMalus(BonusMalusAction.WoodAdept, "Wood@A@s@h Set\n Add Evade"));
				highTier.Add(new BonusMalus(BonusMalusAction.SteelLord, "Steel@A\n Cast Fast", Classes: new List<Classes> { Classes.Fighter }));
				maluses.Add(new BonusMalus(BonusMalusAction.ArmorRemove, "-" + olditemnames[(int)Item.ProRing], equipment: new List<Item> { Item.ProRing }));
			}

			// Single Spells Bonus/Malus
			baseTier.AddRange(CreateSpellBonuses(rom, rng, flags));
			maluses.AddRange(CreateSpellMaluses(rom, rng, flags));

			if ((bool)flags.RandomizeClassCasting)
			{
				var magicBonuses = CreateMagicBonuses(rom, rng, flags);

				baseTier.AddRange(magicBonuses.Item1);
				highTier.AddRange(magicBonuses.Item2); // +2
			}

			// Add Lockpicking Bonus/Malus
			if ((bool)flags.Lockpicking && flags.LockpickingLevelRequirement < 50)
			{
				maluses.Add(new BonusMalus(BonusMalusAction.LockpickingLevel, "LateLockpik", mod: 10, Classes: new List<Classes> { Classes.Thief }));
			}

			if ((bool)flags.Lockpicking && flags.LockpickingLevelRequirement > 1)
			{
				highTier.Add(new BonusMalus(BonusMalusAction.LockpickingLevel, "EarlyLokpik", mod: -10, Classes: new List<Classes> { Classes.Thief }));
			}

			// Add XP Bonuses
			if ((bool)flags.RandomizeClassIncludeXpBonus)
			{
				highTier.Add(new BonusMalus(BonusMalusAction.BonusXp, "+50% XP", mod: 150, Classes: new List<Classes> { Classes.Fighter, Classes.BlackBelt }));
				highTier.Add(new BonusMalus(BonusMalusAction.BonusXp, "+100% XP", mod: 200, Classes: new List<Classes> { Classes.Thief, Classes.RedMage, Classes.BlackMage, Classes.WhiteMage }));

				baseTier.Add(new BonusMalus(BonusMalusAction.BonusXp, "+50% XP", mod: 150, Classes: new List<Classes> { Classes.Thief, Classes.RedMage, Classes.BlackMage, Classes.WhiteMage }));
				baseTier.Add(new BonusMalus(BonusMalusAction.BonusXp, "+50% XP", mod: 150, Classes: new List<Classes> { Classes.Thief, Classes.RedMage, Classes.BlackMage, Classes.WhiteMage }));
			}

			// Add Max MP on MP Gain Bonus
			if (flags.MpGainOnMaxGainMode == MpGainOnMaxGain.Blursed)
			{
				baseTier.Add(new BonusMalus(BonusMalusAction.MpGainOnMaxMpGain, "Max+Mp+", Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));
			}

			// Distribute the class high tier blessings, we control probabilites (should be 5% per blessing in this list)
			highTier.Shuffle(rng);

			// Pick class exclusive blessings first
			foreach (var gameclass in classesBlessingsLists.Select(x => x.Key).ToList())
			{
				var classExclusives = highTier.Where(b => b.ClassList.Count == 1 && b.ClassList.Contains(gameclass)).ToList();

				// We should avoid such a scenario, in which case blursings distribution need updating again, but just in case
				if (classExclusives.Count >= 5)
				{
					classExclusives = classExclusives.GetRange(0, 5);
				}

				classesBlessingsLists[gameclass].AddRange(classExclusives);
				highTier = highTier.Except(classExclusives).ToList();
			}

			var gameClasses = classesBlessingsLists.Select(x => x.Key).ToList();

			// Then high but not class exclusive blessings
			while(gameClasses.Any())
			{
				var currentClass = gameClasses.PickRandom(rng);

				if (classesBlessingsLists[currentClass].Count >= 5)
				{
					gameClasses.Remove(currentClass);
					continue;
				}
				else
				{
					BonusMalus currentBlessing;
					var validHighTier = highTier.Where(b => b.ClassList.Contains(currentClass)).ToList();
					if (validHighTier.Any())
					{
						currentBlessing = validHighTier.PickRandom(rng);
						highTier.Remove(currentBlessing);
					}
					else
					{
						currentBlessing = baseTier.Where(b => b.ClassList.Contains(currentClass)).ToList().PickRandom(rng);
						baseTier.Remove(currentBlessing);
					}

					classesBlessingsLists[currentClass].Add(currentBlessing);
				}
			}

			// Dump anything left in baseTier
			baseTier = baseTier.Concat(highTier).ToList();
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
			if ((flags.LockMode != LockHitMode.Vanilla) && (flags.LockMode != LockHitMode.Accuracy107))
			{
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

			SpellSlotInfo emptySlot = new();

			List<List<byte>> blackSpellList = new();
			List<List<byte>> whiteSpellList = new();


			List<byte> spellNuke = spellHelper.FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies).Where(s => s.Info.elem == SpellElement.None && s.Info.effect >= 100).Select(x => (byte)x.Id).ToList(); // Nuke
			List<byte> spellElem3 = spellHelper.FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies).Where(s => s.Info.effect >= 50 && s.Info.elem != SpellElement.None).Select(x => (byte)x.Id).ToList();
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
					spellBlursingsStrong.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Nuke Magic", spellsmod: new List<SpellSlotInfo> { spellId, spellId, emptySlot }, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));
				}
			}

			if (spellElem3.Count >= 2)
			{
				List<SpellSlotInfo> spells = new();

				while (spells.Count < 2 && spellElem3.Any())
				{
					var selectedSpell = spellElem3.SpliceRandom(rng);
					if (SpellSlotStructure.GetSpellSlots().TryFind(x => x.NameId == selectedSpell, out var spellId))
					{
						spells.Add(spellId);
					}
				}

				if (spells.Count >= 2)
				{
					spellBlursingsNormal.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Elem+ Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], emptySlot }, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));

					spellBlursingsStrong.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Elem+ Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], emptySlot }, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt }));
				}
			}

			if (spellElem2.Count >= 2)
			{
				List<SpellSlotInfo> spells = new();

				while (spells.Count < 2 && spellElem2.Any())
				{
					var selectedSpell = spellElem2.SpliceRandom(rng);
					if (SpellSlotStructure.GetSpellSlots().TryFind(x => x.NameId == selectedSpell, out var spellId))
					{
						spells.Add(spellId);
					}
				}

				if (spells.Count >= 2)
				{
					spellBlursingsNormal.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Elem Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], emptySlot }));
				}
			}

			if (spellCleaning.Count >= 2)
			{
				List<SpellSlotInfo> spells = new();

				while (spells.Count < 2 && spellCleaning.Any())
				{
					var selectedSpell = spellCleaning.SpliceRandom(rng);
					if (SpellSlotStructure.GetSpellSlots().TryFind(x => x.NameId == selectedSpell, out var spellId))
					{
						spells.Add(spellId);
					}
				}

				if (spells.Count >= 2)
				{
					spellBlursingsNormal.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Clean Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], emptySlot }));
				}
			}

			if (spellDoom.Count >= 2)
			{
				List<SpellSlotInfo> spells = new();

				while (spells.Count < 2 && spellDoom.Any())
				{
					var selectedSpell = spellDoom.SpliceRandom(rng);
					if (SpellSlotStructure.GetSpellSlots().TryFind(x => x.NameId == selectedSpell, out var spellId))
					{
						spells.Add(spellId);
					}
				}

				if (spells.Count >= 2)
				{
					spellBlursingsNormal.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Doom Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], emptySlot }, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));
				}
			}

			if (spellCur3.Any() && spellHel2.Any() && spellLife.Any())
			{
				List<SpellSlotInfo> spells = new();
				SpellSlotInfo spellId = new();
				SpellSlotInfo lifespell;

				SpellSlotStructure.GetSpellSlots().TryFind(x => x.NameId == spellLife.PickRandom(rng), out lifespell);

				if (SpellSlotStructure.GetSpellSlots().TryFind(x => x.NameId == spellCur3.PickRandom(rng), out spellId))
				{
					spells.Add(spellId);
				}

				if (SpellSlotStructure.GetSpellSlots().TryFind(x => x.NameId == spellHel2.PickRandom(rng), out spellId))
				{
					spells.Add(spellId);
				}

				if (spells.Count >= 2 || (lifespell != null && spells.Count >= 1))
				{
					spells = new() { spells.SpliceRandom(rng), lifespell ?? spells.SpliceRandom(rng) };
					spells.Shuffle(rng);

					spellBlursingsNormal.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Heal Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], emptySlot }));
				}
			}

			if (spellCur4.Any() && spellHel3.Any() && spellLife.Any())
			{
				List<SpellSlotInfo> spells = new();
				SpellSlotInfo spellId = new();
				SpellSlotInfo lifespell;

				SpellSlotStructure.GetSpellSlots().TryFind(x => x.NameId == spellLife.PickRandom(rng), out lifespell);

				if (SpellSlotStructure.GetSpellSlots().TryFind(x => x.NameId == spellCur4.PickRandom(rng), out spellId))
				{
					spells.Add(spellId);
				}

				if (SpellSlotStructure.GetSpellSlots().TryFind(x => x.NameId == spellHel3.PickRandom(rng), out spellId))
				{
					spells.Add(spellId);
				}

				if (spells.Count >= 2 || (lifespell != null && spells.Count >= 1))
				{
					spells = new() { spells.SpliceRandom(rng), lifespell ?? spells.SpliceRandom(rng) };
					spells.Shuffle(rng);

					spellBlursingsNormal.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Heal+ Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], emptySlot }, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));

					spellBlursingsStrong.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Heal+ Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], emptySlot }, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt }));
				}
			}

			if (spellRuse.Any() && spellSabr.Any())
			{
				List<SpellSlotInfo> spells = new();
				SpellSlotInfo spellId = new();

				if (SpellSlotStructure.GetSpellSlots().TryFind(x => x.NameId == spellRuse.PickRandom(rng), out spellId))
				{
					spells.Add(spellId);
				}

				if (SpellSlotStructure.GetSpellSlots().TryFind(x => x.NameId == spellSabr.PickRandom(rng), out spellId))
				{
					spells.Add(spellId);
				}

				if (spells.Count >= 2)
				{
					spellBlursingsStrong.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Self Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], emptySlot }, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt }));
				}
			}

			if (spellTmpr.Any() && spellFast.Any())
			{
				List<SpellSlotInfo> spells = new();
				SpellSlotInfo spellId = new();

				if (SpellSlotStructure.GetSpellSlots().TryFind(x => x.NameId == spellTmpr.PickRandom(rng), out spellId))
				{
					spells.Add(spellId);
				}

				if (SpellSlotStructure.GetSpellSlots().TryFind(x => x.NameId == spellFast.PickRandom(rng), out spellId))
				{
					spells.Add(spellId);
				}

				spells.Shuffle(rng);

				if (spells.Count >= 2)
				{
					spellBlursingsNormal.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Buff Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], emptySlot }, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));

					spellBlursingsStrong.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Buff Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], emptySlot }, Classes: new List<Classes> { Classes.Fighter, Classes.Thief, Classes.BlackBelt }));
				}
			}

			if (spellWarp.Any() && spellExit.Any())
			{
				List<SpellSlotInfo> spells = new();
				SpellSlotInfo spellId = new();

				if (SpellSlotStructure.GetSpellSlots().TryFind(x => x.NameId == spellWarp.PickRandom(rng), out spellId))
				{
					spells.Add(spellId);
				}

				if (SpellSlotStructure.GetSpellSlots().TryFind(x => x.NameId == spellExit.PickRandom(rng), out spellId))
				{
					spells.Add(spellId);
				}

				if (spells.Count == 2)
				{
					spellBlursingsNormal.Add(new BonusMalus(BonusMalusAction.InnateSpells, "Tele Magic", spellsmod: new List<SpellSlotInfo> { spells[0], spells[1], emptySlot }));
				}
			}

			spellBlursingsStrong.Shuffle(rng);

			return (spellBlursingsNormal, spellBlursingsStrong);
		}
	}
}
