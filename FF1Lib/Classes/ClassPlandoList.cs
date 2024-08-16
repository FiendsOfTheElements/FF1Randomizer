using FF1Lib.Helpers;
using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*Created this new list due to the old one having conditional statements that are not needed for the plando code as
any bonus or malus that is not functional will not be selectable, as well there is no need for randomization or class
based blusrings */

namespace FF1Lib
{
	public partial class GameClasses
	{
		public class BonusMalusPlando
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
			public BonusMalusPlando(BonusMalusAction action, string description, int mod = 0, int mod2 = 0, List<Item> equipment = null, List<bool> binarylist = null, List<SpellSlots> spelllist = null, List<byte> bytelist = null, SpellSlotInfo spellslotmod = null, List<SpellSlotInfo> spellsmod = null, List<Classes> Classes = null)
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
		public void GenerateListsPlando(List<BonusMalus> bonusList, List<BonusMalus> maluses, List<string> olditemnames, ItemNames itemnames, Flags flags, FF1Rom rom)
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

			//Master Bonus List

			bonusList.AddRange(new List<BonusMalus>()
			{
				new BonusMalus(BonusMalusAction.StrMod, "+10 Str.", mod: 10),
				new BonusMalus(BonusMalusAction.AgiMod, "+15 Agi.", mod: 15),
				new BonusMalus(BonusMalusAction.VitMod, "+10 Vit.", mod: 10),
				new BonusMalus(BonusMalusAction.LckMod, "+5 Luck", mod: 5),
				new BonusMalus(BonusMalusAction.HpMod, "+20 HP", mod: 20),
				new BonusMalus(BonusMalusAction.HitMod, "+10 Hit%", mod: 10),
				new BonusMalus(BonusMalusAction.MDefMod, "+10 MDef", mod: 10),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+Equip @X", equipment: equipAxes),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Equip @T", equipment: equipShirts),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Equip @s", equipment: equipShields),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Equip @G+@h", equipment: equipGauntletsHelmets),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+Thief @S", equipment: equipThiefWeapon),
				new BonusMalus(BonusMalusAction.SpcMod, "+2 Lv1 MP", mod: 2),
				new BonusMalus(BonusMalusAction.StrMod, "+20 Str.", mod: 20),
				new BonusMalus(BonusMalusAction.AgiMod, "+25 Agi.", mod: 25),
				new BonusMalus(BonusMalusAction.VitMod, "+20 Vit.", mod: 20),
				new BonusMalus(BonusMalusAction.LckMod, "+10 Luck", mod: 10),
				new BonusMalus(BonusMalusAction.HpMod, "+40 HP", mod: 40),
				new BonusMalus(BonusMalusAction.HitMod, "+20 Hit%", mod: 20),
				new BonusMalus(BonusMalusAction.MDefMod, "+20 MDef", mod: 20),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+Legendary@S", equipment: equipLegendaryWeapons),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Red Mage @A", equipment: equipRedMageArmor),
				new BonusMalus(BonusMalusAction.StartWithMp, "+1 MP LvAll"),
				new BonusMalus(BonusMalusAction.ThorMaster, "Improved\n Thor@H"),
				new BonusMalus(BonusMalusAction.Hunter, "Hurt Undead", mod: 0x18),
				new BonusMalus(BonusMalusAction.Hunter, "Hurt Dragon", mod: 0x02),
				new BonusMalus(BonusMalusAction.InnateResist, "Res. PEDTS", mod: (int)(SpellElement.Poison | SpellElement.Earth | SpellElement.Death | SpellElement.Time | SpellElement.Status)),
				//start of strong tier
				new BonusMalus(BonusMalusAction.StrMod, "+40 Str.", mod: 40),
				new BonusMalus(BonusMalusAction.AgiMod, "+50 Agi.", mod: 50),
				new BonusMalus(BonusMalusAction.VitMod, "+40 Vit.", mod: 40),
				new BonusMalus(BonusMalusAction.LckMod, "+15 Luck", mod: 15),
				new BonusMalus(BonusMalusAction.HpMod, "+80 HP", mod: 80),
				new BonusMalus(BonusMalusAction.MDefGrowth, "+2 MDef/Lv", mod: 2),
				new BonusMalus(BonusMalusAction.WeaponAdd, "+Fighter @S", equipment: equipFighterWeapon),
				new BonusMalus(BonusMalusAction.ArmorAdd, "+Fighter @A", equipment: equipFighterArmor),
				new BonusMalus(BonusMalusAction.SpcGrowth, "Improved MP", bytelist: improvedMPlist),
				new BonusMalus(BonusMalusAction.PowerRW, "Sage", mod: 1, spelllist: wmWhiteSpells.Concat(bmBlackSpells).Concat(wwWhiteSpells).Concat(bwBlackSpells).ToList()),
				new BonusMalus(BonusMalusAction.Hunter, "Hurt All", mod: 0xFF),
				new BonusMalus(BonusMalusAction.InnateResist, "Res. All", mod: 0xFF),
				//Add here the code for the resistances
				//int tier
				new BonusMalus(BonusMalusAction.IntMod, "+10 Int.", mod: 10),
				new BonusMalus(BonusMalusAction.IntMod, "+20 Int.", mod: 20),
				new BonusMalus(BonusMalusAction.IntMod, "+40 Int.", mod: 40),
				//weaponizer tier
				new BonusMalus(BonusMalusAction.CatClawMaster, "Improved\n CatClaw", equipment: new List<Item>() { Item.CatClaw }),
				new BonusMalus(BonusMalusAction.DualWieldKnife, "DualWield @K"),
				//gold tier
				new BonusMalus(BonusMalusAction.StartWithGold, "+200 GP", mod: 2),
				new BonusMalus(BonusMalusAction.StartWithGold, "+1400 GP", mod: 14),
				new BonusMalus(BonusMalusAction.StartWithGold, "+400 GP", mod: 4),
				new BonusMalus(BonusMalusAction.StartWithGold, "+2000 GP", mod: 20),







				/*maluses.Add(new BonusMalus(BonusMalusAction.IntMod, "-10 Int.", mod: -10, Classes: new List<Classes> { Classes.Thief, Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));
				maluses.Add(new BonusMalus(BonusMalusAction.IntMod, "-20 Int.", mod: -20, Classes: new List<Classes> { Classes.RedMage, Classes.WhiteMage, Classes.BlackMage }));*/
			});

		}
	}
}
