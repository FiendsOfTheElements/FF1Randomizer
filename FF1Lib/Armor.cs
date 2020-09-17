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

					int iconIndex = currentArmor.NameBytes[6] > 200 && currentArmor.NameBytes[6] != 255 ? 5 : 6;
					for (int j = 0; j < bonusBytes.Length - 1; j++)
					{
						currentArmor.NameBytes[iconIndex - j] = bonusBytes[bonusBytes.Length - 2 - j];
					}

					currentArmor.writeArmorMemory(this);
				}
			}
		}

		//sample function for creating new armor
		public void ExpandArmor()
		{
			Armor platinumBracelet = new Armor(12, FF1Text.TextToBytes("Plat"), ArmorIcon.BRACELET, 1, 42, 0, 0);
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

		//offset lookups
		//0 - weight
		//1 - absorb
		//2 - resist
		//3 - spell index

		public int ArmorIndex;

		public byte[] NameBytes;
		public ArmorIcon Icon;

		public byte Weight;
		public byte Absorb;
		public byte ElementalResist;
		public byte SpellIndex;

		//written to armor permission area
		public ushort ClassUsability;

		public Armor(int armorIndex, byte[] nameBytes, ArmorIcon icon, byte weight, byte absorb, byte elementalResist, byte spellIndex)
		{
			ArmorIndex = armorIndex;
			NameBytes = nameBytes;
			Icon = icon;
			if (icon != ArmorIcon.NONE)
			{
				NameBytes[6] = (byte)icon;
			}
			Weight = weight;
			Absorb = absorb;
			ElementalResist = elementalResist;
			SpellIndex = spellIndex;
		}

		public Armor(int armorIndex, NesRom rom)
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
			int armorBaseNameOffset = FF1Rom.GearTextOffset + ((ArmorIndex + 40) * FF1Rom.GearTextSize) + (ArmorIndex > 31 ? 1 : 0);
			//TODO figure out if treasure hunt is enabled, if so add 6 to this offset
			byte currentValue;
			NameBytes = rom.Get(armorBaseNameOffset, 8).ToBytes();

			//find icon
			for (int i = 0; i < 8; i++)
			{
				currentValue = NameBytes[i];
				//icon range > 200
				if (currentValue > 200)
				{
					//check for icon
					Icon = getArmorIconFromByte(currentValue);
				}
			}
		}

		public void setClassUsability(ushort classUsability)
		{
			ClassUsability = classUsability;
		}

		public void writeArmorMemory(NesRom rom)
		{
			//armor stats
			int armorBaseOffset = FF1Rom.ArmorOffset + (ArmorIndex * FF1Rom.ArmorSize);
			rom.Put(armorBaseOffset, new byte[] { Weight, Absorb, ElementalResist, SpellIndex });

			//armor permissions
			int armorPermissionOffset = FF1Rom.ArmorPermissionsOffset + (ArmorIndex * FF1Rom.PermissionsSize);
			ushort convertedClassPermissions = (ushort)(ClassUsability ^ 0xFFF);
			rom.Put(armorPermissionOffset, BitConverter.GetBytes(convertedClassPermissions));

			//armor name
			int armorBaseNameOffset = FF1Rom.GearTextOffset + ((ArmorIndex + 40) * FF1Rom.GearTextSize) + (ArmorIndex > 31 ? 1 : 0);
			rom.Put(armorBaseNameOffset, NameBytes);
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

	}
}
