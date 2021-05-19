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

	public partial class FF1Rom : NesRom
	{
		public const int ArmorPermissionsOffset = 0x3BFB0;
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
			Armor platinumBracelet = new Armor(12, "Plat@B", ArmorIcon.BRACELET, 1, 42, 0, 0);
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

		//written to armor permission area
		public ushort ClassUsability;

		public Armor(int armorIndex, string name, ArmorIcon icon, byte weight, byte absorb, byte elementalResist, byte spellIndex)
		{
			ArmorIndex = armorIndex;
			Name = name;
			Icon = icon;
			Weight = weight;
			Absorb = absorb;
			ElementalResist = elementalResist;
			SpellIndex = spellIndex;
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
