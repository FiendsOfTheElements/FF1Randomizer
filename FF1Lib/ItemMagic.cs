using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomUtilities;
using System.Diagnostics;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public const int GearTextOffset = 0x2B9BD;
		public const int GearTextSize = 8;
		private const int MagicBitOffset = 0x3;
		private const int SpellNamesOffset = 0x2BE03;
		private const int SpellNamesSize = 5;
		private const int SpellNamesCount = 64;

		private int WeaponStart = (byte)ItemLists.AllWeapons.ElementAt(0);
		private int ArmorStart = (byte)ItemLists.AllArmor.ElementAt(0);

		public void ShuffleItemMagic(MT19337 rng, bool balancedShuffle)
		{
			var Spells = GetSpells();

			// Remove out of battle only spells (spells where the effect is 0)
			Spells.RemoveAll(spell => spell.Data[4] == 0);
			if (balancedShuffle)
			{
				// if balanced shuffle is on, remove spells which are too strong or too weak
				// remove any spell which boosts attack power that isn't self-casting
				Spells.RemoveAll(spell => spell.Data[4] == 0x0D && spell.Data[3] != 0x04);
				// remove any spell that doubles hit rate
				Spells.RemoveAll(spell => spell.Data[4] == 0x0C);
				// remove any non-elemental damage spell with >55 effectivity
				Spells.RemoveAll(spell => spell.Data[4] == 0x01 && spell.Data[1] > 55 && spell.Data[2] == 0b0000000);
				// remove any multitarget elemental damage spell with > 70 effectivity
				Spells.RemoveAll(spell => spell.Data[4] == 0x01 && spell.Data[1] > 70 && spell.Data[3] == 0x01);
				// remove any singletarget elemental damage spell with > 100 effectivity
				Spells.RemoveAll(spell => spell.Data[4] == 0x01 && spell.Data[1] > 100);
				// remove any damage spell with <20 effectivity
				Spells.RemoveAll(spell => spell.Data[4] == 0x01 && spell.Data[1] < 20);
				// remove status recovery spells which don't heal paralysis or mute
				Spells.RemoveAll(spell => spell.Data[4] == 0x08 && (spell.Data[1] & 0b01010000) == 0);
				// remove HP Max
				Spells.RemoveAll(spell => spell.Data[4] == 0x0F);
				// remove HARM spells with > 120 effectivity
				Spells.RemoveAll(spell => spell.Data[4] == 0x02 && spell.Data[1] > 120);
				// remove non-elemental power word kill
				Spells.RemoveAll(spell => spell.Data[4] == 0x12 && (spell.Data[1] & 0b00000011) != 0 && spell.Data[2] == 0b00000000);
				// remove status spells which only cast darkness, sleep, or poison
				Spells.RemoveAll(spell => spell.Data[4] == 0x03 && (spell.Data[1] & 0b11010011) == 0);
				Spells.RemoveAll(spell => spell.Data[4] == 0x12 && (spell.Data[1] & 0b11010011) == 0);
				// remove evasion up spells with effects greater than 80
				Spells.RemoveAll(spell => spell.Data[4] == 0x10 && spell.Data[1] > 80);
				// remove armor up spells with effects greater than 32
				Spells.RemoveAll(spell => spell.Data[4] == 0x09 && spell.Data[1] > 32);
				// remove spells which resist all elements
				Spells.RemoveAll(spell => spell.Data[4] == 0x0A && spell.Data[1] == 0xFF);
			}
			Spells.Shuffle(rng); // Shuffle all spells remaining, then assign to each item that can cast a spell

			foreach (var item in Spells.Zip(ItemLists.AllMagicItem, (s, i) => new { Spell = s, Item = i }))
			{
				WriteItemSpellData(item.Spell, item.Item);
			}
		}

		private void WriteItemSpellData(MagicSpell Spell, Item item)
		{
			// Set the spell an item casts
			var offset = WeaponOffset + 0x8 * Math.Min((byte)item - WeaponStart, ArmorStart - WeaponStart) + 0x4 * Math.Max(0, (byte)item - ArmorStart) + MagicBitOffset;
			Data[offset] = (byte)(Spell.Index + 1);

			// Setup the text of the item's name to include the spell name.
			offset = GearTextOffset + ((byte)item > (byte)Item.Ribbon ? 1 : 0) + GearTextSize * ((byte)item - WeaponStart);
			if (Get(offset, 1)[0] > 200)
			{
				offset++; // If the first byte is in the icon range, bump the pointer to overwrite after it.
			}
			else if (Get(offset + 6, 1)[0] <= 200)
			{
				Data[offset + 6] = 0xFF; // Erase final non-icon characters from name.
			}

			// Fix up the name of the spell so it works as part of an item name.
			var fixedSpellName = FF1Text.TextToBytes(Spell.Name.PadRight(6), false, FF1Text.Delimiter.Empty);
			Debug.Assert(fixedSpellName.Length == 6);
			Put(offset, fixedSpellName);
		}

		public void CastableItemTargeting()
		{
			// update a lut with the correct location of a routine
			Put(0x314E2, Blob.FromHex("EC9563"));

			// see 0C_94F5_BatleSubMenu_MagicDrinkItem.asm for all changes
			// This is only useful if Item Magic is shuffled since there are no single target spells on items in vanilla
			int changesOffset = 0x314F5; // offset where changes start

			Put(changesOffset, Get(changesOffset + 0x3, 0x14));
			Put(changesOffset + 0x14, Blob.FromHex("2097F8"));
			Put(changesOffset + 0x17, Get(changesOffset + 0x1D, 0x81));
			Put(changesOffset + 0xAC, Get(changesOffset + 0x9F, 0x6));
			Put(changesOffset + 0xB2, Blob.FromHex("04A0FF"));
			Put(changesOffset + 0x99, Blob.FromHex("A940209D95C9FFF0034C5D934CF59448AE7D6B"));
			Put(changesOffset + 0xB5, Blob.FromHex("68604A901020A49AAE7D6BC902D00468A9FF6068604A9008AD7A6B0980A868604A900468A0FE60AD7A6B203A9AAE7D6BC902D00468A9FF60ADAB6A29030980A86860"));
			Put(changesOffset + 0xF7, Get(changesOffset + 0x100, 0x42));
			Put(changesOffset + 0x127, Blob.FromHex("11"));
			Put(changesOffset + 0x139, Get(changesOffset + 0x14F, 0xB0));
			Put(changesOffset + 0x144, Blob.FromHex("EC"));
			Put(changesOffset + 0x14F, Blob.FromHex("7D6B"));
			Put(changesOffset + 0x155, Blob.FromHex("7D6B"));
			Put(changesOffset + 0x1CF, Blob.FromHex("D6"));
			Put(changesOffset + 0x1E9, Blob.FromHex("8D7D6BCE7D6BAD7D6B20119785808681A910209D95C9FFF0034C5D934C639600"));

			// Writes the spell name rather than the item name, not in the .asm, intruduces a small bug when using non castable items
			//Put(0x3307D, Blob.FromHex("0CAD8C6CC942B00369B04CA0B0C901D008AD8C6C38E927D00AAD8C6CC942B0031869B0"));

			PutInBank(0x0C, 0x96E7, Blob.FromHex("A82011972065C2EAEA"));
			PutInBank(0x1F, 0xC265, CreateLongJumpTableEntry(0x0F, 0x8AD0));
			PutInBank(0x0F, 0x8AD0, Blob.FromHex("85808681C0FFD008A9D68580A9968581A91060"));
		}


	}
}
