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

						int iconIndex = currentWeapon.NameBytes[6] > 200 && currentWeapon.NameBytes[6] != 255 ? 5 : 6;
						for (int j = 0; j < bonusBytes.Length - 1; j++)
						{
							currentWeapon.NameBytes[iconIndex - j] = bonusBytes[bonusBytes.Length - 2 - j];
						}

						currentWeapon.writeWeaponMemory(this);
					}
				}
			}
		}

		//sample function for creating new weapons
		public void ExpandWeapon()
		{
			Weapon flameChucks = new Weapon(0, FF1Text.TextToBytes("Flame"), WeaponIcon.CHUCK, 20, 26, 10, 0, (byte)Element.FIRE, 0, WeaponSprite.CHUCK, 0x25);
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
	}

	public class Weapon
	{
		//index
		public int WeaponIndex;

		//writen to separate area
		public WeaponIcon Icon;
		public byte[] NameBytes;

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

		public Weapon(int weaponIndex, NesRom rom)
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
			//hold over from armor, doesnt really need this check
			int weaponBaseNameOffset = FF1Rom.GearTextOffset + (WeaponIndex * FF1Rom.GearTextSize) + (weaponIndex > (byte)Item.Ribbon ? 1 : 0);
			//TODO figure out if treasure hunt is enabled, if so add 6 to this offset
			byte currentValue;
			NameBytes = rom.Get(weaponBaseNameOffset, 8).ToBytes();

			//find icon
			for (int i = 0; i < 8; i++)
			{
				currentValue = NameBytes[i];
				//icon range > 200
				if (currentValue > 200)
				{
					//check for icon
					Icon = getWeaponIconFromByte(currentValue);
				}
			}
		}

		//pre-load values, could be used to create new weapons in the future
		public Weapon(int weaponIndex, byte[] nameBytes, WeaponIcon icon, byte hitBonus, byte damage, byte crit, byte spellIndex, byte elementalWeakness, byte typeWeakeness, WeaponSprite weaponTypeSprite, byte weaponSpritePaletteColor)
		{
			WeaponIndex = weaponIndex;
			NameBytes = nameBytes;
			Icon = icon;
			if (icon != WeaponIcon.NONE)
			{
				NameBytes[6] = (byte)icon;
			}
			HitBonus = hitBonus;
			Damage = damage;
			Crit = crit;
			SpellIndex = spellIndex;
			ElementalWeakness = elementalWeakness;
			TypeWeakness = typeWeakeness;
			WeaponTypeSprite = weaponTypeSprite;
			WeaponSpritePaletteColor = weaponSpritePaletteColor;
		}

		public void setClassUsability(ushort classUsability)
		{
			ClassUsability = classUsability;
		}

		public void writeWeaponMemory(NesRom rom)
		{
			//weapon stats
			int weaponBaseOffset = FF1Rom.WeaponOffset + (WeaponIndex * FF1Rom.WeaponSize);
			rom.Put(weaponBaseOffset, new byte[] { HitBonus, Damage, Crit, SpellIndex, ElementalWeakness, TypeWeakness, (byte)WeaponTypeSprite, WeaponSpritePaletteColor });

			//weapon usability
			int weaponUsabilityOffset = FF1Rom.WeaponPermissionsOffset + (WeaponIndex * FF1Rom.PermissionsSize);
			ushort convertedClassPermissions = (ushort)(ClassUsability ^ 0xFFF);
			rom.Put(weaponUsabilityOffset, BitConverter.GetBytes(convertedClassPermissions));

			//weapon name
			int weaponBaseNameOffset = FF1Rom.GearTextOffset + (WeaponIndex * FF1Rom.GearTextSize);
			rom.Put(weaponBaseNameOffset, NameBytes);
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
	}

}
