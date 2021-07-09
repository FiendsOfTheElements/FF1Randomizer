using System;
using System.Collections.Generic;
using System.Text;
using RomUtilities;

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

	    // Determines what kind of powers to assign.
	    const int COMMON = 0;
	    const int RARE = 1;
	    const int LEGENDARY = 2;
	    const int WHITE_CASTING = 3;
	    const int BLACK_CASTING = 4;
	    const int BUFF_CASTING = 5;
	    const int RIBBON = 6;

	    // type, class
	    var allArmors = new int[,] {
		{ ARMOR, CLOTH, COMMON }, // cloth
		{ ARMOR, LIGHT, COMMON }, // wooden armor
		{ ARMOR, MEDIUM, COMMON }, // chain armor
		{ ARMOR, HEAVY, COMMON }, // iron armor
		{ ARMOR, HEAVY, COMMON }, // steel armor
		{ ARMOR, MEDIUM, COMMON }, // silver armor
		{ ARMOR, HEAVY, RARE }, // flame armor
		{ ARMOR, HEAVY, RARE }, // ice armor
		{ ARMOR, KNIGHT, LEGENDARY }, // opal armor
		{ ARMOR, KNIGHT, LEGENDARY }, // dragon armor
		{ BRACELET, CLOTH, COMMON }, // copper bracelet
		{ BRACELET, LIGHT, COMMON }, // silver bracelet
		{ BRACELET, MEDIUM, RARE }, // gold bracelet
		{ BRACELET, HEAVY, RARE }, // opal bracelet
		{ SHIRT, CLOTH, WHITE_CASTING }, // white shirt
		{ SHIRT, CLOTH, BLACK_CASTING }, // black shirt
		{ SHIELD, LIGHT, COMMON }, // wooden shield
		{ SHIELD, HEAVY, COMMON }, // iron shield
		{ SHIELD, MEDIUM, COMMON }, // silver shield
		{ SHIELD, HEAVY, RARE }, // flame shield
		{ SHIELD, HEAVY, RARE }, // ice shield
		{ SHIELD, KNIGHT, LEGENDARY }, // opal shield
		{ SHIELD, KNIGHT, LEGENDARY }, // aegis shield
		{ SHIELD, LIGHT, COMMON }, // buckler
		{ CAPE,  CLOTH, RARE }, // procape
		{ HELM,  CLOTH, COMMON }, // cap
		{ HELM,  LIGHT, COMMON }, // wooden helm
		{ HELM,  HEAVY, COMMON }, // iron helm
		{ HELM,  MEDIUM, COMMON }, // silver helm
		{ HELM,  KNIGHT, LEGENDARY }, // opal helm
		{ HELM,  HEAVY,  WHITE_CASTING }, // heal helm
		{ HELM,  CLOTH,  RIBBON }, // ribbon
		{ GAUNTLET,  CLOTH,  COMMON }, // gloves
		{ GAUNTLET,  LIGHT,  COMMON }, // copper gauntlet
		{ GAUNTLET,  HEAVY,  COMMON }, // iron gauntlet
		{ GAUNTLET,  MEDIUM,  COMMON }, // silver gauntlet
		{ GAUNTLET,  MEDIUM,  BLACK_CASTING }, // zeus gauntlet
		{ GAUNTLET,  MEDIUM,  BUFF_CASTING }, // power gauntlet
		{ GAUNTLET,  KNIGHT,  LEGENDARY }, // opal gauntlet
		{ RING,       CLOTH,  RARE }, // proring
	    };

	    for (int i = 0; i < 40; i++) {
		var armorType = allArmors[i, 0];
		var armorClass = allArmors[i, 1];
		var armorPower = allArmors[i, 2];

		var name = "armor";
		byte weight = 0;
		byte absorb = 0;
		byte elementalResist = 0;
		byte spellIndex = 0;
		ArmorType type = ArmorType.ARMOR;
		ushort permissions = 0;

		absorb = (byte)RangeScale(armorDefenseBase[armorClass, armorType], .7, 1.4, 1.0, rng);
		weight = (byte)RangeScale(armorEvadePenaltyBase[armorClass, armorType], .7, 1.4, 1.0, rng);

		switch (armorClass) {
		    case CLOTH:
			name = "Cloth";
			permissions = 0xFFF;
			break;
		    case LIGHT:
			permissions = (ushort)(EquipPermission.Fighter|EquipPermission.Knight|
					       EquipPermission.Thief|EquipPermission.Ninja|
					       EquipPermission.BlackBelt|EquipPermission.Master|
					       EquipPermission.RedMage|EquipPermission.RedWizard);
			name = "Light";
			break;
		    case MEDIUM:
			name = "Medium";
			permissions = (ushort)(EquipPermission.Fighter|EquipPermission.Knight|
					       EquipPermission.Ninja|
					       EquipPermission.RedMage|EquipPermission.RedWizard);
			break;
		    case HEAVY:
			name = "Heavy";
			permissions = (ushort)(EquipPermission.Fighter|EquipPermission.Knight|
					       EquipPermission.Ninja);
			break;
		    case KNIGHT:
			name = "Knight";
			permissions = (ushort)EquipPermission.Knight;
			break;
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
			break;
		    case SHIRT:
			name = $"{name,-6}{Armor.IconCodes[ArmorIcon.SHIRT]}";
			type = ArmorType.ARMOR;
			break;
		    case CAPE:
			name = "ProCape ";
			type = ArmorType.SHIELD;
			break;
		    case RING:
			name = "ProRing ";
			type = ArmorType.GAUNTLET;
			break;
		}

		var armor = new Armor(i, name, ArmorIcon.NONE, weight, absorb, elementalResist, spellIndex, type);
		armor.setClassUsability(permissions);
		armor.writeArmorMemory(this);
	    }

	    for (int i = 0; i < 40; i++) {
		var a = new Armor(i, this);
		Console.WriteLine($"{i}: [] {a.Name,8} {a.Icon} weight: {a.Weight} absorb: {a.Absorb} resist: {a.ElementalResist:X} casting: {a.SpellIndex} type: {a.Type} permissions: {a.ClassUsability:X}");
		Utilities.WriteSpoilerLine($"{i}: [] {a.Name,8} {a.Icon} {a.Weight} {a.Absorb} {a.ElementalResist} {a.SpellIndex} {a.Type} {a.ClassUsability}");
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
