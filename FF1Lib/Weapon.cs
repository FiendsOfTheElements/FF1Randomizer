using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RomUtilities;
using FF1Lib;

namespace FF1Lib
{
	public enum WeaponIcon : byte
	{
		NONE = 0x00,
		SWORD = 0xD4,
		HAMMER = 0xD5,
		KNIFE = 0xD6,
		AXE = 0xD7,
		STAFF = 0xD8,
		CHUCK = 0xD9,
	}

	public enum WeaponSprite : byte
	{
		NONE = 0x00,
		SHORTSWORD = 0x80,
		SCIMITAR = 0x84,
		FALCHION = 0x88,
		LONGSWORD = 0x8C,
		RAPIER = 0x90,
		HAMMER = 0x94,
		KNIFE = 0x98,
		AXE = 0x9C,
		STAFF = 0xA0,
		IRONSTAFF = 0xA4,
		CHUCK = 0xA8
	}

	public enum Element : byte
	{
		NONE = 0x00,
		STATUS = 0x01,
		POISON = 0x02,
		TIME = 0x04,
		DEATH = 0x08,
		FIRE = 0x10,
		ICE = 0x20,
		LIGHTNING = 0x40,
		EARTH = 0x80
	}

	public enum MonsterType : byte
	{
		NONE = 0x00,
		MAGICAL = 0x01,
		DRAGON = 0x02,
		GIANT = 0x04,
		UNDEAD = 0x08,
		WERE = 0x10,
		AQUATIC = 0x20,
		MAGE = 0x40,
		REGENERATIVE = 0x80
	}

	public enum EquipPermission : ushort
	{
		Fighter = 0x800,
		Thief = 0x400,
		BlackBelt = 0x200,
		RedMage = 0x100,
		WhiteMage = 0x080,
		BlackMage = 0x040,
		Knight = 0x020,
		Ninja = 0x010,
		Master = 0x008,
		RedWizard = 0x004,
		WhiteWizard = 0x002,
		BlackWizard = 0x001
	}

	// lut_ClassEquipBit: ;  FT   TH   BB   RM   WM   BM      KN   NJ   MA   RW   WW   BW
	// .WORD               $800,$400,$200,$100,$080,$040,   $020,$010,$008,$004,$002,$001

	public partial class FF1Rom : NesRom
	{
		public const int WeaponPermissionsOffset = 0x3BF50;
		public const int PermissionsSize = 2;
		public const int PermissionsCount = 40;

		//offset lookups
		//0 - hit
		//1 - dmg
		//2 - crit
		//3 - weapon spell index
		//4 - elemental weaknesses
		//5 - type weaknesses
		//6 - weapon type sprite
		//7 - weapon sprite palette color

		public void RandomWeaponBonus(MT19337 rng, int min, int max, bool excludeMasa)
		{
			//get base stats
			Weapon currentWeapon;
			for (int i = 0; i < WeaponCount; i++)
			{
				if (i != 39 || !excludeMasa)
				{
					currentWeapon = new Weapon(i, this);
					int bonus = rng.Between(min, max);
					if (bonus != 0)
					{
						//adjust stats
						//clamp to 1 dmg min, 0 hit min, 50 hit maximum
						currentWeapon.HitBonus = (byte)Math.Max(0, (int)(currentWeapon.HitBonus + (3 * bonus)));
						currentWeapon.HitBonus = (byte)Math.Min(50, (int)(currentWeapon.HitBonus));
						currentWeapon.Damage = (byte)Math.Max(1, (int)currentWeapon.Damage + (2 * bonus));
						currentWeapon.Crit = (byte)Math.Max(1, (int)currentWeapon.Crit + (3 * bonus));

						//change last two non icon characters to -/+bonus
						string bonusString = string.Format((bonus > 0) ? "+{0}" : "{0}", bonus.ToString());
						byte[] bonusBytes = FF1Text.TextToBytes(bonusString);

						var nameBytes = FF1Text.TextToBytes(currentWeapon.Name, false);

						int iconIndex = nameBytes[6] > 200 && nameBytes[6] != 255 ? 5 : 6;
						for (int j = 0; j < bonusBytes.Length - 1; j++)
						{
							nameBytes[iconIndex - j] = bonusBytes[bonusBytes.Length - 2 - j];
						}

						currentWeapon.Name = FF1Text.BytesToText(nameBytes);

						currentWeapon.writeWeaponMemory(this);
					}
				}
			}
		}

		//sample function for creating new weapons
		public void ExpandWeapon()
		{
			Weapon flameChucks = new Weapon(0, "Flame@N", WeaponIcon.CHUCK, 20, 26, 10, 0, (byte)Element.FIRE, 0, WeaponSprite.CHUCK, 0x25);
			flameChucks.setClassUsability((ushort)(EquipPermission.BlackBelt | EquipPermission.Master | EquipPermission.Ninja));
			flameChucks.writeWeaponMemory(this);
		}

		public void MagisizeWeapons(MT19337 rng, bool balanced)
		{
			var Spells = GetSpells();

			if (!balanced)
			{
				Spells.RemoveAll(spell => spell.Data[4] == 0);
				foreach (Item weapon in ItemLists.AllWeapons.Except(ItemLists.AllMagicItem).ToList())
					WriteItemSpellData(Spells.SpliceRandom(rng), weapon);
			}
			else
			{
				var tieredSpells = new List<List<MagicSpell>> { Spells.GetRange(0, 16), Spells.GetRange(16, 16), Spells.GetRange(32, 16), Spells.GetRange(48, 16) };

				var commonOdds = new List<int> { 0, 0, 0, 0, 1, 1, 1, 1, 2, 2 };
				var rareOdds = new List<int> { 0, 1, 1, 1, 2, 2, 2, 3, 3, 3 };
				var legendaryOdds = new List<int> { 1, 2, 2, 2, 3, 3, 3, 3, 3, 3 };

				for (int i = 0; i < 4; i++)
					tieredSpells[i].RemoveAll(spell => spell.Data[4] == 0);

				foreach (Item weapon in ItemLists.CommonWeaponTier)
				{
					var selectedTier = commonOdds.PickRandom(rng);
					while (tieredSpells[selectedTier].Count == 0)
						selectedTier = commonOdds.PickRandom(rng);

					WriteItemSpellData(tieredSpells[selectedTier].SpliceRandom(rng), weapon);
				}

				foreach (Item weapon in ItemLists.RareWeaponTier.Except(ItemLists.AllMagicItem).ToList())
				{
					var selectedTier = rareOdds.PickRandom(rng);
					while (tieredSpells[selectedTier].Count == 0)
						selectedTier = rareOdds.PickRandom(rng);

					WriteItemSpellData(tieredSpells[selectedTier].SpliceRandom(rng), weapon);
				}

				foreach (Item weapon in ItemLists.LegendaryWeaponTier)
				{
					var selectedTier = legendaryOdds.PickRandom(rng);
					while (tieredSpells[selectedTier].Count == 0)
						selectedTier = legendaryOdds.PickRandom(rng);

					WriteItemSpellData(tieredSpells[selectedTier].SpliceRandom(rng), weapon);
				}

				foreach (Item weapon in ItemLists.UberTier)
				{
					var selectedTier = Rng.Between(rng, 0, 3);
					while (tieredSpells[selectedTier].Count == 0)
						selectedTier = Rng.Between(rng, 0, 3);

					WriteItemSpellData(tieredSpells[selectedTier].SpliceRandom(rng), weapon);
				}
			}
		}

		public void Weaponizer(MT19337 rng, bool useQualityNamesOnly, bool commonWeaponsHavePowers) {
		    var tierList = new List<IReadOnlyList<Item>> { ItemLists.CommonWeaponTier, ItemLists.RareWeaponTier,
							  ItemLists.LegendaryWeaponTier, ItemLists.UberTier};
		    var damageBases = new int[]      { 10, 18, 26, 36, 52 };
		    var critPercentBases = new int[] {  5, 10, 20, 30, 45 };
		    var hitPercentBases = new int[]  {  5, 10, 20, 40, 60 };

		    var weaponTypeAdjust = new int[,] {
			// damage, crit%, hit%
			{  0,  0,  0 }, // sword
			{  1,  0, -1 }, // axe
			{ -1,  0,  1 }, // knife
			{ -1,  1,  0 }, // chucks
			{  0, -1,  0 }, // hammer
			{  0,  0, -1 }, // staff
		    };

		    var powers = new int[] {
			(int)Element.POISON,
			(int)Element.FIRE | ((int)MonsterType.UNDEAD<<8) | ((int)MonsterType.REGENERATIVE<<8),
			(int)Element.ICE,
			(int)Element.LIGHTNING,
			(int)(MonsterType.MAGICAL|MonsterType.MAGE)<<8,
			(int)MonsterType.DRAGON<<8,
			(int)MonsterType.GIANT<<8,
			(int)MonsterType.UNDEAD<<8,
			//(int)MonsterType.WERE<<8,
			(int)MonsterType.AQUATIC<<8,
			//(int)MonsterType.MAGE<<8,
			(int)Element.FIRE | (int)Element.ICE,
			(int)MonsterType.MAGICAL<<8|(int)MonsterType.DRAGON<<8|(int)MonsterType.GIANT<<8|(int)MonsterType.UNDEAD<<8
			    |(int)MonsterType.WERE<<8|(int)MonsterType.AQUATIC<<8|(int)MonsterType.MAGE<<8,
			(int)Element.POISON | (int)Element.FIRE | (int)Element.ICE | (int)Element.LIGHTNING,
		    };

		    var powerNames = new string[][] {
			new string[] { "Poison" },
			new string[] { "Flame", "Burn" },
			new string[] { "Ice", "Freeze" },
			new string[] { "Shock", "Bolt" },
			new string[] { "Rune", "Ritual" },
			new string[] { "Dragon" },
			new string[] { "Giant", "Imp", "Troll" },
			new string[] { "Holy", "Smite", "Banish", "Slayer" },
			//new string[] { "Were" },
			new string[] { "Coral", "Aqua", "Water", "Splash" },
			//new string[] { "Mage" },
			new string[] { "IceHot" },
			new string[] { "Chroma" },
			new string[] { "Elmntl" }
		    };

		    var weaponIcons = new WeaponIcon[] {
			WeaponIcon.SWORD,
			WeaponIcon.AXE,
			WeaponIcon.KNIFE,
			WeaponIcon.CHUCK,
			WeaponIcon.HAMMER,
			WeaponIcon.STAFF
		    };

		    var unpromotedPermissions = new Dictionary<WeaponIcon, EquipPermission>
		    {
			{ WeaponIcon.SWORD,  EquipPermission.Fighter|EquipPermission.Thief|EquipPermission.RedMage },
			{ WeaponIcon.AXE,    EquipPermission.Fighter },
			{ WeaponIcon.KNIFE,  EquipPermission.Fighter|EquipPermission.Thief|EquipPermission.RedMage|EquipPermission.BlackMage },
			{ WeaponIcon.CHUCK,  EquipPermission.Thief|EquipPermission.BlackBelt },
			{ WeaponIcon.HAMMER, EquipPermission.Fighter|EquipPermission.WhiteMage },
			{ WeaponIcon.STAFF,  EquipPermission.RedMage|EquipPermission.BlackMage|EquipPermission.WhiteMage }
		    };

		    var promotedPermissions = new Dictionary<WeaponIcon, EquipPermission>
		    {
			{ WeaponIcon.SWORD,  EquipPermission.Knight|EquipPermission.Ninja|EquipPermission.RedWizard },
			{ WeaponIcon.AXE,    EquipPermission.Knight|EquipPermission.Ninja },
			{ WeaponIcon.KNIFE,  EquipPermission.Knight|EquipPermission.Ninja|EquipPermission.RedWizard|EquipPermission.BlackWizard },
			{ WeaponIcon.CHUCK,  EquipPermission.Ninja|EquipPermission.Master },
			{ WeaponIcon.HAMMER, EquipPermission.Knight|EquipPermission.WhiteWizard|EquipPermission.Ninja },
			{ WeaponIcon.STAFF,  EquipPermission.RedWizard|EquipPermission.BlackWizard|EquipPermission.Ninja },
		    };

		    var weaponSprites = new WeaponSprite[][] {
			new WeaponSprite[] { WeaponSprite.SHORTSWORD, WeaponSprite.SCIMITAR, WeaponSprite.FALCHION,
					     WeaponSprite.LONGSWORD, WeaponSprite.RAPIER },
			new WeaponSprite[] { WeaponSprite.AXE },
			new WeaponSprite[] { WeaponSprite.KNIFE },
			new WeaponSprite[] { WeaponSprite.CHUCK },
			new WeaponSprite[] { WeaponSprite.HAMMER, WeaponSprite.IRONSTAFF },
			new WeaponSprite[] { WeaponSprite.STAFF, WeaponSprite.IRONSTAFF },
		    };

		    var qualityLevels = new int[] {
			18,
			26,
			32,
			40,
			50,
			60
		    };
		    var gearQuality = new string[][] {
			new string[] { "Wooden", "Small", "Short" }, // <= 18
			new string[] { "Copper", "Long"},            // <= 26
			new string[] { "Iron",   "Heavy" },          // <= 32
			new string[] { "Steel",  "Great" },          // <= 40
			new string[] { "Silver", "Shiny" },          // <= 50
			new string[] { "Mithrl", "Sharp" },          // <= 60
			new string[] { "Opal", "Diamnd", "Wicked" }  // > 60
		    };

		    var preferredSprites = new Dictionary<string, WeaponSprite> {
			{"Short @S", WeaponSprite.RAPIER},
			{"Small @S", WeaponSprite.RAPIER},
			{"Long  @S", WeaponSprite.LONGSWORD},
			{"Katana ", WeaponSprite.SCIMITAR},
		    };
		    var preferredColors = new Dictionary<string, byte> {
			{"Wooden", 0x28},
			{"Copper", 0x27},
			{"Bronze", 0x27},
			{"Iron", 0x20},
			{"Steel", 0x20},
			{"Silver", 0x2C},
			{"Mithrl", 0x2C},
			{"Ice", 0x22},
			{"Freeze", 0x22},
			{"ICE ", 0x22},
			{"ICE2", 0x22},
			{"ICE3", 0x22},
			{"Flame", 0x26},
			{"Burn", 0x26},
			{"FIRE", 0x26},
			{"FIR2", 0x26},
			{"FIR3", 0x26},
			{"Shock", 0x28},
			{"Bolt", 0x28},
			{"LIT ", 0x28},
			{"LIT2", 0x28},
			{"LIT3", 0x28},
			{"Dragon", 0x2A},
		    };

		    var Spells = GetSpells();
		    var defenseSwordSpells = new string[] { "RUSE", "INV2", "FOG2", "WALL" };
		    var thorHammerSpells = new string[] { "NUKE", "FADE", "ICE3", "LIT3", "FIR3", "ICE2", "LIT2", "FIR2" };
		    var thorHammerBins = new int[]      {     1,      2,     12,     24,     34,     56,     80,    100  };

		    var weaponNames = new List<string>();
		    var weaponGfx = new List<int>();

		    for (int tier = 4; tier > 0; tier--) {
			var requireTypes = new List<int> { 0, 1, 2, 3, 5 };
			for (int count = 0; count < tierList[tier-1].Count; ) {
			    var weaponItemId = tierList[tier-1][count];
			    int weaponIndex = (int)weaponItemId - (int)Item.WoodenNunchucks;
			    string name;
			    WeaponIcon icon;
			    byte hitBonus;
			    byte damage;
			    byte crit;
			    byte spellIndex = 0xFF;
			    byte elementalWeakness = 0;
			    byte typeWeakeness = 0;
			    WeaponSprite weaponTypeSprite = WeaponSprite.NONE;
			    byte weaponSpritePaletteColor = 0;

			    int weaponType;
			    switch (weaponItemId) {
				case Item.Masamune:
				case Item.Xcalber:
				    // 0 (sword) or 1 (axe)
				    weaponType = rng.Between(0, 1);
				    break;
				case Item.Katana:
				case Item.Vorpal:
				case Item.Defense:
				    weaponType = 0;
				    break;
				case Item.ThorHammer:
				    weaponType = 4;
				    break;
				default:
				    if (requireTypes.Count > 0) {
					weaponType = requireTypes[0];
				    } else {
					weaponType = rng.Between(0, 5);
				    }
				    break;
			    }

			    icon = weaponIcons[weaponType];

			    int damageTier = tier + weaponTypeAdjust[weaponType, 0];
			    int critTier =   tier + weaponTypeAdjust[weaponType, 1];
			    int hitpctTier = tier + weaponTypeAdjust[weaponType, 2];

			    if (weaponItemId == Item.Katana || weaponItemId == Item.Vorpal) {
				critTier += 1;
			    }

			    if (weaponItemId == Item.Masamune) {
				critTier -= 1;
			    }

			    if (weaponItemId == Item.Vorpal) {
				damageTier -= 1;
			    }

			    if (weaponItemId == Item.ThorHammer) {
				damageTier += 1;
			    }

			    damageTier = Math.Min(Math.Max(damageTier, 0), 4);
			    critTier   = Math.Min(Math.Max(critTier,   0), 4);
			    hitpctTier = Math.Min(Math.Max(hitpctTier, 0), 4);

			    damage = (byte)RangeScale(damageBases[damageTier], .7, 1.4, 1.0, rng);
			    crit = (byte)RangeScale(critPercentBases[critTier], .7, 1.4, 1.0, rng);
			    hitBonus = (byte)RangeScale(hitPercentBases[hitpctTier], .7, 1.4, 1.0, rng);

			    // need to clamp this to prevent overflow
			    hitBonus = Math.Min(hitBonus, (byte)50);

			    double ddamage = damage;
			    double dcrit = crit;
			    double dhitBonus = hitBonus;

			    // "score" is a rough approximation of DPS
			    // used to assign name based on gear
			    // quality and set the base price.
			    double score = ((ddamage*1.5) + ((ddamage*2.0) * (dcrit / 200.0))) * (1+Math.Floor((dhitBonus+4)/32));

			    if (weaponItemId == Item.Defense) {
				var defMagic = defenseSwordSpells[rng.Between(0, defenseSwordSpells.Length-1)];
				for (int i = 0; i < Spells.Count; i++) {
				    if (Spells[i].Name == defMagic) {
					spellIndex = (byte)i;
					break;
				    }
				}
			    } else if (weaponItemId == Item.ThorHammer) {
				var pick = rng.Between(1, 100);
				string thorMagic = "";
				for (int i = 0; i < thorHammerBins.Length; i++) {
				    if (pick <= thorHammerBins[i]) {
					thorMagic = thorHammerSpells[i];
					break;
				    }
				}
				for (int i = 0; i < Spells.Count; i++) {
				    if (Spells[i].Name == thorMagic) {
					spellIndex = (byte)i;
					break;
				    }
				}
			    } else {
				int spellChance = rng.Between(1, 100);
				if ((commonWeaponsHavePowers || tier >= 2)
				    && ((weaponType < 4 && spellChance <= Math.Min(10*tier, 20))
					|| (weaponType >= 4 && spellChance <= Math.Min(30*tier, 60))))
				{
				    var spelllevelLow = Math.Min(1+tier, 5);           // 2, 3, 4, 5
				    var spelllevelHigh = Math.Min(spelllevelLow+3, 8); // 5, 6, 7, 8
				    do {
					spellIndex = (byte)rng.Between(spelllevelLow*8, spelllevelHigh*8-1);
				    } while(Spells[spellIndex].Data[4] == 0); // must be combat castable
				}
			    }

			    int specialPower = -1;
			    if (spellIndex == 0xFF) {
				int powerChance = rng.Between(1, 100);
				if (tier == 1 && commonWeaponsHavePowers && powerChance <= 20) {
				    specialPower = rng.Between(0, powers.Length-1);
				} else if (tier > 1) {
				    specialPower = rng.Between(0, powers.Length-1);
				}
			    }

			    if (specialPower != -1) {
				elementalWeakness = (byte)(powers[specialPower] & 0xFF);
				typeWeakeness = (byte)((powers[specialPower]>>8) & 0xFF);
			    }

			    string nameWithIcon;
			    if (weaponItemId == Item.Masamune) {
				name = nameWithIcon = (weaponType == 0 ? "Masmune" : "Maxmune");
			    } else if (weaponItemId == Item.Xcalber) {
				name = nameWithIcon = (weaponType == 0 ? "Xcalber" : "Axcalbr");
			    } else if (weaponItemId == Item.Katana) {
				name = nameWithIcon = "Katana ";
			    } else if (weaponItemId == Item.Vorpal) {
				name = nameWithIcon = "Vorpal ";
			    } else if (weaponItemId == Item.Defense) {
				name = nameWithIcon = "Defense";
			    } else if (weaponItemId == Item.ThorHammer) {
				name = nameWithIcon = "Thor  @H";
				elementalWeakness = (byte)(Element.ICE|Element.FIRE|Element.LIGHTNING);
			    } else {
				if (spellIndex != 0xFF) {
				    name = Spells[spellIndex].Name;
				} else if (specialPower != -1 && !useQualityNamesOnly) {
				    var powername = rng.Between(0, powerNames[specialPower].Length-1);
				    name = powerNames[specialPower][powername];
				} else {
				    string[] gear = gearQuality[gearQuality.Length-1];
				    for (int i = 0; i < qualityLevels.Length; i++) {
					if (score <= qualityLevels[i]) {
					    gear = gearQuality[i];
					    break;
					}
				    }
				    var gearname = rng.Between(0, gear.Length-1);
				    name = gear[gearname];
				}
				nameWithIcon = $"{name,-6}{Weapon.IconCodes[icon]}";
			    }

			    if (weaponNames.Contains(nameWithIcon)) {
				continue;
			    }

			    if (nameWithIcon == "Wooden@K") {
				// Wooden stake slays vampires
				typeWeakeness = (byte)MonsterType.UNDEAD;
			    }

			    // Weapons casting elemental magic also
			    // get elemental bonus
			    if (name.StartsWith("ICE")) {
				elementalWeakness = (byte)Element.ICE;
			    }
			    if (name.StartsWith("FIR")) {
				elementalWeakness = (byte)Element.FIRE;
			    }
			    if (name.StartsWith("LIT")) {
				elementalWeakness = (byte)Element.LIGHTNING;
			    }
			    if (name == "BANE") {
				elementalWeakness = (byte)Element.POISON;
			    }
			    if (name == "HARM" || name.StartsWith("HRM")) {
				typeWeakeness = (byte)MonsterType.UNDEAD;
			    }

			    if (preferredColors.ContainsKey(name)) {
				weaponSpritePaletteColor = preferredColors[name];
			    }
			    if (preferredSprites.ContainsKey(nameWithIcon)) {
				weaponTypeSprite = preferredSprites[nameWithIcon];
			    }

			    var tries = 10;
			    while(tries > 0) {
				if (weaponTypeSprite == WeaponSprite.NONE) {
				    weaponTypeSprite = weaponSprites[weaponType][rng.Between(0, weaponSprites[weaponType].Length-1)];
				}
				if (weaponSpritePaletteColor == 0) {
				    weaponSpritePaletteColor = (byte)rng.Between(0x20, 0x2C);
				}
				if (!weaponGfx.Contains((weaponSpritePaletteColor<<8) | (int)weaponTypeSprite)) {
				    break;
				}
				weaponTypeSprite = WeaponSprite.NONE;
				weaponSpritePaletteColor = 0;
				tries--;
			    }

			    if (tries == 0) {
				continue;
			    }

			    double goldvalue = score;
			    if (specialPower != -1) {
				goldvalue += 15;
			    }
			    if (spellIndex != 0xFF) {
				goldvalue += 25;
			    }
			    switch (tier) {
				case 1:
				    goldvalue *= (goldvalue/4);
				    break;
				case 2:
				    goldvalue *= goldvalue;
				    break;
				case 3:
				    goldvalue *= goldvalue*1.5;
				    break;
				case 4:
				    goldvalue *= goldvalue*2;
				    break;
			    }

			    goldvalue = Math.Min(goldvalue, 65535);

			    EquipPermission permissions;
			    if (weaponItemId == Item.Masamune) {
				permissions = (EquipPermission)0xFFF;
			    } else if (weaponItemId == Item.Xcalber) {
				permissions = EquipPermission.Knight;
			    } else if (weaponItemId == Item.Katana) {
				permissions = EquipPermission.Ninja;
			    } else if (weaponItemId == Item.ThorHammer) {
				permissions = EquipPermission.WhiteWizard;
			    } else {
				permissions = promotedPermissions[icon];
				switch (tier) {
				    case 1:
					permissions |= unpromotedPermissions[icon];
					break;
				    case 2:
					if (rng.Between(1, 100) < 50) {
					    permissions |= unpromotedPermissions[icon];
					}
					break;
				    default:
					// promoted only
					break;
				}
			    }

			    Utilities.WriteSpoilerLine($"{weaponIndex}: [{tier}]  {nameWithIcon,8}  +{damage,2} {crit,2}% {hitBonus,2}% {goldvalue,5}g ({score}) {permissions} gfx {weaponSpritePaletteColor:X} {weaponTypeSprite}");

			    var newWeapon = new Weapon(weaponIndex, nameWithIcon, icon, hitBonus, damage, crit,
						       (byte)(spellIndex == 0xFF ? 0 : spellIndex+1), elementalWeakness,
						       typeWeakeness, weaponTypeSprite, weaponSpritePaletteColor);
			    newWeapon.setClassUsability((ushort)permissions);
			    newWeapon.writeWeaponMemory(this);

			    Put(PriceOffset + (PriceSize*(int)weaponItemId), Blob.FromUShorts(new ushort[] {(ushort)goldvalue}));

			    weaponGfx.Add((weaponSpritePaletteColor<<8) | (int)weaponTypeSprite);
			    weaponNames.Add(nameWithIcon);
			    if (requireTypes.Count > 0) {
				requireTypes.RemoveAt(0);
			    }
			    count++;
			}
		    }
		    Utilities.WriteSpoilerLine("\n");
		}

	}

	public class Weapon
	{
		public Item Id => (Item)(WeaponIndex + (int)Item.WoodenNunchucks);
		public Spell Spell => SpellIndex == 0xFF ? 0 : (Spell)(SpellIndex - 1 + (int)Spell.CURE);

		//index
		public int WeaponIndex;

		//writen to separate area
		public WeaponIcon Icon;
		public string Name;

		//written to weapon data area
		public byte HitBonus;
		public byte Damage;
		public byte Crit;
		public byte SpellIndex;
		public byte ElementalWeakness;
		public byte TypeWeakness;
		public WeaponSprite WeaponTypeSprite;
		public byte WeaponSpritePaletteColor;

		//written to class permission area
		ushort ClassUsability;

		public Weapon(int weaponIndex, FF1Rom rom)
		{

			WeaponIndex = weaponIndex;
			//read stats from memory
			int weaponBaseOffset = FF1Rom.WeaponOffset + (WeaponIndex * FF1Rom.WeaponSize);
			HitBonus = rom.Get(weaponBaseOffset, 1).ToBytes()[0];
			Damage = rom.Get(weaponBaseOffset + 1, 1).ToBytes()[0];
			Crit = rom.Get(weaponBaseOffset + 2, 1).ToBytes()[0];
			SpellIndex = rom.Get(weaponBaseOffset + 3, 1).ToBytes()[0];
			ElementalWeakness = rom.Get(weaponBaseOffset + 4, 1).ToBytes()[0];
			TypeWeakness = rom.Get(weaponBaseOffset + 5, 1).ToBytes()[0];
			byte weaponSpriteTypeHolder = rom.Get(weaponBaseOffset + 6, 1).ToBytes()[0];
			WeaponTypeSprite = getWeaponSpriteFromByte(weaponSpriteTypeHolder);
			WeaponSpritePaletteColor = rom.Get(weaponBaseOffset + 7, 1).ToBytes()[0];

			//read permissions
			int weaponUsabilityOffset = FF1Rom.WeaponPermissionsOffset + (WeaponIndex * FF1Rom.PermissionsSize);

			byte highByte = rom.Get(weaponUsabilityOffset, 1).ToBytes()[0];
			byte lowByte = rom.Get(weaponUsabilityOffset + 1, 1).ToBytes()[0];

			ushort assembledUShort = BitConverter.ToUInt16(new byte[2] { highByte, lowByte }, 0);
			ushort convertedClassPermissions = (ushort)(assembledUShort ^ 0xFFF);
			ClassUsability = convertedClassPermissions;

			//get name stuff
			Icon = WeaponIcon.NONE;

			var itemnames = rom.ReadText(FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextPointerCount);

			Name = itemnames[(int)Item.WoodenNunchucks + WeaponIndex];

			foreach (var kv in IconCodes)
			{
				if (Name.Contains(kv.Value))
				{
					Icon = kv.Key;
					break;
				}
			}
		}

		//pre-load values, could be used to create new weapons in the future
		public Weapon(int weaponIndex, string name, WeaponIcon icon, byte hitBonus, byte damage, byte crit, byte spellIndex, byte elementalWeakness, byte typeWeakeness, WeaponSprite weaponTypeSprite, byte weaponSpritePaletteColor)
		{
			WeaponIndex = weaponIndex;
			Name = name;
			Icon = icon;
			HitBonus = hitBonus;
			Damage = damage;
			Crit = crit;
			SpellIndex = spellIndex;
			ElementalWeakness = elementalWeakness;
			TypeWeakness = typeWeakeness;
			WeaponTypeSprite = weaponTypeSprite;
			WeaponSpritePaletteColor = weaponSpritePaletteColor;
		}

		public static IEnumerable<Weapon> LoadAllWeapons(FF1Rom rom, Flags flags)
		{
			int i = flags.EnableExtConsumables ? 4 : 0;
			for (; i < 40; i++)
			{
				yield return new Weapon(i, rom);
			}
		}

		public void setClassUsability(ushort classUsability)
		{
			ClassUsability = classUsability;
		}

		public void writeWeaponMemory(FF1Rom rom)
		{
			//weapon stats
			int weaponBaseOffset = FF1Rom.WeaponOffset + (WeaponIndex * FF1Rom.WeaponSize);
			rom.Put(weaponBaseOffset, new byte[] { HitBonus, Damage, Crit, SpellIndex, ElementalWeakness, TypeWeakness, (byte)WeaponTypeSprite, WeaponSpritePaletteColor });

			//weapon usability
			int weaponUsabilityOffset = FF1Rom.WeaponPermissionsOffset + (WeaponIndex * FF1Rom.PermissionsSize);
			ushort convertedClassPermissions = (ushort)(ClassUsability ^ 0xFFF);
			rom.Put(weaponUsabilityOffset, BitConverter.GetBytes(convertedClassPermissions));

			rom.UpdateItemName((Item)((int)Item.WoodenNunchucks + WeaponIndex), Name);
		}

		private WeaponIcon getWeaponIconFromByte(byte icon)
		{
			WeaponIcon matchedType = WeaponIcon.NONE;
			foreach (WeaponIcon temp in (WeaponIcon[])Enum.GetValues(typeof(WeaponIcon)))
			{
				if (icon == (byte)temp)
				{
					matchedType = temp;
				}
			}

			return matchedType;
		}

		private WeaponSprite getWeaponSpriteFromByte(byte sprite)
		{
			WeaponSprite matchedType = WeaponSprite.NONE;
			foreach (WeaponSprite temp in (WeaponSprite[])Enum.GetValues(typeof(WeaponSprite)))
			{
				if (sprite == (byte)temp)
				{
					matchedType = temp;
				}
			}

			return matchedType;
		}

		public static Dictionary<WeaponIcon, string> IconCodes = new Dictionary<WeaponIcon, string>
		{
			{ WeaponIcon.SWORD, "@S" },
			{ WeaponIcon.HAMMER, "@H" },
			{ WeaponIcon.KNIFE, "@K" },
			{ WeaponIcon.AXE, "@X" },
			{ WeaponIcon.STAFF, "@F" },
			{ WeaponIcon.CHUCK, "@N" }
		};
	}
}
