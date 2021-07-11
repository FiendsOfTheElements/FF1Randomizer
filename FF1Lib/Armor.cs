using System;
using System.Collections.Generic;
using System.Text;
using RomUtilities;
using FF1Lib.Helpers;
using System.Linq;

namespace FF1Lib
{
	enum ArmorIcon : byte
	{
		NONE = 0x00,
		ARMOR = 0xDA,
		SHIELD = 0xDB,
		HELM = 0xDC,
		GAUNTLET = 0xDD,
		BRACELET = 0xDE,
		SHIRT = 0xDF
	}

	enum ArmorType : byte
	{
		ARMOR = 0x00,
		SHIELD = 0x01,
		HELM = 0x02,
		GAUNTLET = 0x03
	}

	public partial class FF1Rom : NesRom
	{
		public const int ArmorPermissionsOffset = 0x3BFA0;
		public const int ArmorTypeOffset        = 0x3BCD1;
		public const int ArmorPermissionsCount = 40;

		public void RandomArmorBonus(MT19337 rng, int min, int max)
		{
			//get base stats
			Armor currentArmor;
			for (int i = 0; i < ArmorCount; i++)
			{
				currentArmor = new Armor(i, this);
				int bonus = rng.Between(min, max);
				if (bonus != 0)
				{
					//body armor(indexes 0 - 15) +2/-2 +2/-2 weight
					//shield(indexes 16 - 24) +1/-1 +1/-1 weight
					//helm(indexes 25 - 31) +1/-1 +1/-1 weight
					//hand(indexes 32 - 39) +1/-1 +1/-1 weight
					int armorTypeAbsorbBonus = i < 16 ? 2 : 1;
					int armorTypeWeightBonus = i < 16 ? 2 : 1;

					//adjust stats
					//clamp minimums to 0 weight, and 1 absorb
					currentArmor.Weight = (byte)Math.Max(0, currentArmor.Weight - (armorTypeWeightBonus * bonus));
					currentArmor.Absorb = (byte)Math.Max(1, currentArmor.Absorb + (armorTypeAbsorbBonus * bonus));

					//change last two non icon characters to -/+bonus
					string bonusString = string.Format((bonus > 0) ? "+{0}" : "{0}", bonus.ToString());
					byte[] bonusBytes = FF1Text.TextToBytes(bonusString);

					var nameBytes = FF1Text.TextToBytes(currentArmor.Name, false);

					int iconIndex = nameBytes[6] > 200 && nameBytes[6] != 255 ? 5 : 6;
					for (int j = 0; j < bonusBytes.Length - 1; j++)
					{
						nameBytes[iconIndex - j] = bonusBytes[bonusBytes.Length - 2 - j];
					}

					currentArmor.Name = FF1Text.BytesToText(nameBytes);

					currentArmor.writeArmorMemory(this);
				}
			}
		}

		//sample function for creating new armor
		public void ExpandArmor()
		{
		    Armor platinumBracelet = new Armor(12, "Plat@B", ArmorIcon.BRACELET, 1, 42, 0, 0, ArmorType.ARMOR);
			platinumBracelet.setClassUsability((ushort)(
				EquipPermission.BlackBelt |
				EquipPermission.BlackMage |
				EquipPermission.BlackWizard |
				EquipPermission.Fighter |
				EquipPermission.Knight |
				EquipPermission.Master |
				EquipPermission.Ninja |
				EquipPermission.RedMage |
				EquipPermission.RedWizard |
				EquipPermission.Thief |
				EquipPermission.WhiteMage |
				EquipPermission.WhiteWizard));
			platinumBracelet.writeArmorMemory(this);
		}

	public void ArmorCrafter(MT19337 rng) {

	    var tierList = new List<IReadOnlyList<Item>> { ItemLists.CommonArmorTier,
							   ItemLists.RareArmorTier,
							   ItemLists.LegendaryArmorTier };

	    // armor classes, determines who can equip
	    // cloth -- everyone
	    // light -- fi/th/bb/rm
	    // medium -- fi/ni/rm
	    // heavy -- fi/ni
	    // legendary (knight, mage stuff, ribbon, 1 more?)

	    const int CLOTH = 0;
	    const int LIGHT = 1;
	    const int MEDIUM = 2;
	    const int HEAVY = 3;
	    const int KNIGHT = 4;

	    // Armor types.
	    const int ARMOR = 0;
	    const int SHIELD = 1;
	    const int HELM = 2;
	    const int GAUNTLET = 3;
	    const int BRACELET = 4;
	    const int SHIRT = 5;
	    const int CAPE = 6;
	    const int RING = 7;

	    // class x type -> defense base
	    var armorDefenseBase = new int[,] {
	    // armor, shield, helm, gauntlet, bracelet, shirt, cape, ring
		{ 4,       2,    1,        1,        4,    24,    8,    8 },  // cloth
		{ 8,       4,    3,        2,        8,    24,    8,    8 },  // light
		{ 16,      8,    6,        4,       16,    24,    8,    8 },  // medium
		{ 32,     12,    6,        6,       32,    24,    8,    8 },  // heavy
		{ 40,     16,    8,        8,       40,    24,    8,    8 },  // knight
	    };

	    // class x type -> evade base
	    var armorEvadePenaltyBase = new int[,] {
	    // armor, shield, helm, gauntlet, bracelet, shirt, cape, ring
		{ 4,       1,    1,        1,        1,     2,    2,    1 },  // cloth
		{ 10,      8,    3,        3,        1,     2,    2,    1 },  // light
		{ 20,      8,    3,        3,        1,     2,    2,    1 },  // medium
		{ 30,     10,    6,        3,        1,     2,    2,    1 },  // heavy
		{ 40,     10,    3,        3,        1,     2,    2,    1 },  // knight
	    };

	    var resists = new byte[] {
		(byte)Element.STATUS,
		(byte)Element.POISON,
		(byte)Element.TIME,
		(byte)Element.DEATH,
		(byte)Element.FIRE,
		(byte)Element.ICE,
		(byte)Element.LIGHTNING,
		(byte)Element.EARTH,
	    };
	    var resistNames = new Dictionary<int, string> {
		{ (int)Element.STATUS, "Active" }, // resist status attacks
		{ (int)Element.POISON, "Aegis" },  // resist stone/poison
		{ (int)Element.TIME, "Time" },   // resis time
		{ (int)Element.DEATH, "Prot" },   // resist death
		{ (int)Element.FIRE, "Ice" },    // resist fire
		{ (int)Element.ICE, "Flame" },   // resist ice
		{ (int)Element.LIGHTNING, "Thundr" }, // resist lightning
		{ (int)Element.EARTH, "Earth" },  // resist earth
	    };
	    var classNames = new string[][] {
		new string[] { "Cloth",  "Velvet", "Silk" },
		new string[] { "Leathr", "Copper" },
		new string[] { "Silver", "Mithrl" },
		new string[] { "Iron",   "Steel", "Gold" },
		new string[] { "Opal",   "Dragon" },
	    };

	    var spellHelper = new SpellHelper(this);
	    var allSpells = GetSpells();
	    var generatedItems = new HashSet<FF1Lib.Item>();
	    var generatedNames = new HashSet<string>();
	    for (int tier = 2; tier >= 0; tier--) {
		List<int> requireType;
		List<int> requireClasses;
		switch(tier) {
		    case 2:
			requireType = new List<int> {ARMOR, SHIELD, HELM, GAUNTLET};
			requireClasses = new List<int> {KNIGHT, KNIGHT, KNIGHT, KNIGHT};
			break;
		    case 0:
			requireType = new List<int> {BRACELET, BRACELET, BRACELET};
			requireClasses = new List<int> {CLOTH, LIGHT, MEDIUM};
			break;
		    default:
			requireType = new List<int>();
			requireClasses = new List<int>();
			break;
		}
		for (int count = 0; count < tierList[tier].Count; ) {
		    var itemId = tierList[tier][count];

		    if (generatedItems.Contains(itemId)) {
			count++;
			continue;
		    }

		    int armorType;
		    int armorClass;

		    // incentive armor that needs to be re-created:
		    //
		    // opal bracelet
		    // power bonk
		    // white shirt
		    // black shirt
		    // ribbon

		    var accessories = new List<int> {HELM, CAPE, RING};

		    armorType = rng.Between(0, 7);
		    armorClass = rng.Between(0, 3);
		    if (requireType.Count > 0) {
			armorType = requireType[0];
			armorClass = requireClasses[0];
			requireType.RemoveAt(0);
			requireClasses.RemoveAt(0);
		    } else if (itemId == Item.Opal) {
			armorType = BRACELET;
			armorClass = HEAVY;
		    } else if (itemId == Item.PowerGauntlets) {
			armorType = GAUNTLET;
		    } else if (itemId == Item.WhiteShirt) {
			armorType = SHIRT;
			armorClass = HEAVY;
		    } else if (itemId == Item.BlackShirt) {
			armorType = SHIRT;
			armorClass = HEAVY;
		    } else if (itemId == Item.Ribbon) {
			armorType = accessories[rng.Between(0, accessories.Count-1)];
			armorClass = CLOTH;
		    }

		    var name = "";
		    byte weight = 0;
		    byte absorb = 0;
		    byte elementalResist = 0;
		    byte spellIndex = 0xFF;
		    ArmorType type = ArmorType.ARMOR;
		    ushort permissions = 0;

		    absorb = (byte)RangeScale(armorDefenseBase[armorClass, armorType], .7, 1.4, 1.0, rng);
		    weight = (byte)RangeScale(armorEvadePenaltyBase[armorClass, armorType], .7, 1.4, 1.0, rng);

		    int nameClass = armorClass;
		    name = classNames[nameClass][rng.Between(0, classNames[nameClass].Length-1)];
		    switch (armorClass) {
			case CLOTH:
			    permissions = 0xFFF;
			    break;
			case LIGHT:
			    permissions = (ushort)(EquipPermission.Fighter|EquipPermission.Knight|
						   EquipPermission.Thief|EquipPermission.Ninja|
						   EquipPermission.BlackBelt|EquipPermission.Master|
						   EquipPermission.RedMage|EquipPermission.RedWizard);
			    break;
			case MEDIUM:
			    permissions = (ushort)(EquipPermission.Fighter|EquipPermission.Knight|
						   EquipPermission.Ninja|
						   EquipPermission.RedMage|EquipPermission.RedWizard);
			    break;
			case HEAVY:
			    permissions = (ushort)(EquipPermission.Fighter|EquipPermission.Knight|
						   EquipPermission.Ninja);
			    break;
			case KNIGHT:
			    permissions = (ushort)EquipPermission.Knight;
			    break;
		    }

		    var spells = new List<FF1Lib.Spell>();
		    if (itemId == Item.PowerGauntlets) {
			// cast FAST, SABR or TMPR
			spells = new List<FF1Lib.Spell>(spellHelper.FindSpells(SpellRoutine.Fast, SpellTargeting.Any).
					       Concat(spellHelper.FindSpells(SpellRoutine.Fast, SpellTargeting.Any)).
					       Select(s => s.Id));

		    } else if (itemId == Item.WhiteShirt) {
			// cast INV2, FOG2, or WALL
			spells = new List<FF1Lib.Spell>(spellHelper.FindSpells(SpellRoutine.Ruse, SpellTargeting.AllCharacters).
					       Concat(spellHelper.FindSpells(SpellRoutine.ArmorUp, SpellTargeting.AllCharacters)).
					       Concat(spellHelper.FindSpells(SpellRoutine.DefElement, SpellTargeting.Any, (SpellElement)0xFF)).
					       Select(s => s.Id));
		    } else if (itemId == Item.BlackShirt) {
			// Any AOE damage
			spells = new List<FF1Lib.Spell>(spellHelper.FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies).
					       Select(s => s.Id));
		    } else if (itemId == Item.Ribbon) {
		    } else if (tier >= 1 && (armorType == HELM || armorType == GAUNTLET
					     || armorType == SHIRT || armorType == CAPE || armorType == RING))
		    {
			var roll = rng.Between(0, 100);
			if (roll < 50) {
			    spells = new List<FF1Lib.Spell>(spellHelper.FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies).
						   Concat(spellHelper.FindSpells(SpellRoutine.DamageUndead, SpellTargeting.AllEnemies)).
						   Concat(spellHelper.FindSpells(SpellRoutine.Heal, SpellTargeting.AllCharacters)).
						   Concat(spellHelper.FindSpells(SpellRoutine.ArmorUp, SpellTargeting.AllCharacters)).
						   Concat(spellHelper.FindSpells(SpellRoutine.Lock, SpellTargeting.OneEnemy)).
						   Concat(spellHelper.FindSpells(SpellRoutine.Lock, SpellTargeting.AllEnemies)).
						   Concat(spellHelper.FindSpells(SpellRoutine.Ruse, SpellTargeting.Any)).
						   Select(s => s.Id));
			}
		    }
		    if (spells.Count > 0) {
			spellIndex = (byte)(spells.SpliceRandom(rng));
			spellIndex = (byte)(spellIndex-(byte)Spell.CURE);
			name = allSpells[spellIndex].Name;
		    }

		    var chooseResist = new List<byte>(resists);
		    if (itemId == Item.Ribbon) {
			elementalResist |= 0xFF;
			name = "Ribo";
		    } else if (tier == 2 && (armorType == ARMOR || armorType == SHIRT)) {
			elementalResist |= chooseResist.SpliceRandom(rng);
			name = resistNames[elementalResist];
			elementalResist |= chooseResist.SpliceRandom(rng);
			elementalResist |= chooseResist.SpliceRandom(rng);
		    } else if (tier == 1 && (armorType == SHIRT)) {
			elementalResist |= chooseResist.SpliceRandom(rng);
			name = resistNames[elementalResist];
			elementalResist |= chooseResist.SpliceRandom(rng);
		    } else if (tier >= 1 && armorType != BRACELET && spellIndex == 0xFF) {
			elementalResist |= chooseResist.SpliceRandom(rng);
			name = resistNames[elementalResist];
		    }

		    switch (armorType) {
			case ARMOR:
			    name = $"{name,-6}{Armor.IconCodes[ArmorIcon.ARMOR]}";
			    type = ArmorType.ARMOR;
			    break;
			case SHIELD:
			    name = $"{name,-6}{Armor.IconCodes[ArmorIcon.SHIELD]}";
			    type = ArmorType.SHIELD;
			    break;
			case HELM:
			    name = $"{name,-6}{Armor.IconCodes[ArmorIcon.HELM]}";
			    type = ArmorType.HELM;
			    break;
			case GAUNTLET:
			    name = $"{name,-6}{Armor.IconCodes[ArmorIcon.GAUNTLET]}";
			    type = ArmorType.GAUNTLET;
			    break;
			case BRACELET:
			    name = $"{name,-6}{Armor.IconCodes[ArmorIcon.BRACELET]}";
			    type = ArmorType.ARMOR;
			    permissions = 0xFFF;
			    break;
			case SHIRT:
			    type = ArmorType.ARMOR;
			    if (itemId == Item.WhiteShirt) {
				name = "White";
				permissions = (ushort)(EquipPermission.WhiteWizard);
			    } else if (itemId == Item.BlackShirt) {
				name = "Black";
				permissions = (ushort)(EquipPermission.BlackWizard);
			    } else {
				permissions = (ushort)(EquipPermission.WhiteWizard | EquipPermission.BlackWizard);
			    }
			    name = $"{name,-6}{Armor.IconCodes[ArmorIcon.SHIRT]}";
			    break;
			case CAPE:
			    name = name.Substring(0, Math.Min(4, name.Length));
			    name = $"{name,-4}Cape";
			    type = ArmorType.SHIELD;
			    permissions = 0xFFF;
			    break;
			case RING:
			    name = name.Substring(0, Math.Min(4, name.Length));
			    name = $"{name,-4}Ring";
			    type = ArmorType.GAUNTLET;
			    permissions = 0xFFF;
			    break;
		    }

		    if (generatedNames.Contains(name)) {
			continue;
		    }
		    generatedNames.Add(name);

		    var armor = new Armor(itemId-Item.Cloth, name, ArmorIcon.NONE, weight, absorb,
					  elementalResist, (byte)(spellIndex == 0xFF ? 0 : spellIndex+1), type);
		    armor.setClassUsability(permissions);
		    armor.writeArmorMemory(this);

		    generatedItems.Add(itemId);
		    count++;
		}
	    }
		for (int i = 0; i < 40; i++) {
		    var a = new Armor(i, this);
		    var casting = "";
		    if (a.SpellIndex != 0) {
			casting = allSpells[a.SpellIndex-1].Name;
		    }

		    var resistName = "";
		    for (int j = 0; j < resists.Length; j++) {
			if ((a.ElementalResist & resists[j]) != 0) {
			    resistName += " " + (Element)resists[j];
			}
		    }

		    var logLine = $"{i}: {a.Name,8} absorb: {a.Absorb} weight: {a.Weight} resist:{resistName} casting: {casting} type: {a.Type} permissions: {a.ClassUsability:X}";
		    Console.WriteLine(logLine);
		    Utilities.WriteSpoilerLine(logLine);
		}

	}
	}

	class Armor
	{
		public Item Id => (Item)(ArmorIndex + (int)Item.Cloth);
		public Spell Spell => SpellIndex == 0xFF ? 0 : (Spell)(SpellIndex - 1 + (int)Spell.CURE);


		//offset lookups
		//0 - weight
		//1 - absorb
		//2 - resist
		//3 - spell index

		public int ArmorIndex;

		public string Name;
		public ArmorIcon Icon;

		public byte Weight;
		public byte Absorb;
		public byte ElementalResist;
		public byte SpellIndex;
	        public ArmorType Type;

		//written to armor permission area
		public ushort ClassUsability;

	    public Armor(int armorIndex, string name, ArmorIcon icon, byte weight, byte absorb, byte elementalResist, byte spellIndex, ArmorType type)
		{
			ArmorIndex = armorIndex;
			Name = name;
			Icon = icon;
			Weight = weight;
			Absorb = absorb;
			ElementalResist = elementalResist;
			SpellIndex = spellIndex;
			Type = type;
		}

		public Armor(int armorIndex, FF1Rom rom)
		{

			ArmorIndex = armorIndex;
			//read stats from memory
			int armorBaseOffset = FF1Rom.ArmorOffset + (ArmorIndex * FF1Rom.ArmorSize);
			Weight = rom.Get(armorBaseOffset, 1).ToBytes()[0];
			Absorb = rom.Get(armorBaseOffset + 1, 1).ToBytes()[0];
			ElementalResist = rom.Get(armorBaseOffset + 2, 1).ToBytes()[0];
			SpellIndex = rom.Get(armorBaseOffset + 3, 1).ToBytes()[0];
			Type = (ArmorType)rom.Get(FF1Rom.ArmorTypeOffset+ArmorIndex, 1).ToBytes()[0];

			//read permissions
			int armorPermissionOffset = FF1Rom.ArmorPermissionsOffset + (ArmorIndex * FF1Rom.PermissionsSize);

			byte highByte = rom.Get(armorPermissionOffset, 1).ToBytes()[0];
			byte lowByte = rom.Get(armorPermissionOffset + 1, 1).ToBytes()[0];

			ushort assembledUShort = BitConverter.ToUInt16(new byte[2] { highByte, lowByte }, 0);
			ushort convertedClassPermissions = (ushort)(assembledUShort ^ 0xFFF);
			ClassUsability = convertedClassPermissions;

			//get name stuff
			Icon = ArmorIcon.NONE;

			var itemnames = rom.ReadText(FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextPointerCount);

			Name = itemnames[(int)Item.Cloth + ArmorIndex];

			foreach (var kv in IconCodes)
			{
				if (Name.Contains(kv.Value))
				{
					Icon = kv.Key;
					break;
				}
			}
		}

		public static IEnumerable<Armor> LoadAllArmors(FF1Rom rom, Flags flags)
		{
			for (int i = 0; i < 40; i++)
			{
				yield return new Armor(i, rom);
			}
		}

		public void setClassUsability(ushort classUsability)
		{
			ClassUsability = classUsability;
		}

		public void writeArmorMemory(FF1Rom rom)
		{
			//armor stats
			int armorBaseOffset = FF1Rom.ArmorOffset + (ArmorIndex * FF1Rom.ArmorSize);
			rom.Put(armorBaseOffset, new byte[] { Weight, Absorb, ElementalResist, SpellIndex });

			//armor permissions
			int armorPermissionOffset = FF1Rom.ArmorPermissionsOffset + (ArmorIndex * FF1Rom.PermissionsSize);
			ushort convertedClassPermissions = (ushort)(ClassUsability ^ 0xFFF);
			rom.Put(armorPermissionOffset, BitConverter.GetBytes(convertedClassPermissions));
			rom.Put(FF1Rom.ArmorTypeOffset+ArmorIndex, new byte[]{(byte)Type});

			rom.UpdateItemName((Item)((int)Item.Cloth + ArmorIndex), Name);
		}

		private ArmorIcon getArmorIconFromByte(byte icon)
		{
			ArmorIcon matchedType = ArmorIcon.NONE;
			foreach (ArmorIcon temp in (ArmorIcon[])Enum.GetValues(typeof(ArmorIcon)))
			{
				if (icon == (byte)temp)
				{
					matchedType = temp;
				}
			}

			return matchedType;
		}

		public static Dictionary<ArmorIcon, string> IconCodes = new Dictionary<ArmorIcon, string>
		{
			{ ArmorIcon.ARMOR, "@A" },
			{ ArmorIcon.SHIELD, "@s" },
			{ ArmorIcon.HELM, "@h" },
			{ ArmorIcon.GAUNTLET, "@G" },
			{ ArmorIcon.BRACELET, "@B" },
			{ ArmorIcon.SHIRT, "@T" }
		};
	}
}
