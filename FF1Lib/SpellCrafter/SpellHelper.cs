using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.SpellCrafter
{
	public class SpellHelper
	{
		private FF1Rom rom;

		public SpellHelper(FF1Rom _rom)
		{
			rom = _rom;
		}

		public static IEnumerable<Spell> ReadAllSpells(FF1Rom rom)
		{
			var itemnames = rom.ReadText(FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextPointerCount);
			var spellPermissions = rom.Get(FF1Rom.MagicPermissionsOffset, 8 * 12).Chunk(FF1Rom.MagicCount / 8);
			var spellMessages = rom.Get(FF1Rom.MagicTextPointersOffset, FF1Rom.MagicCount);
			var spellData = rom.Get(FF1Rom.MagicOffset, FF1Rom.MagicSize * FF1Rom.MagicCount).Chunk(FF1Rom.MagicSize);

			List<Spell> spells = new List<Spell>();
			for (int i = 0; i < FF1Rom.MagicSize; i++)
			{
				var name = itemnames[176 + i];
				var data = spellData[i];
				var msg = spellMessages[i];
				var permissions = GetPermissions(spellPermissions, i);

				spells.Add(new Spell(i, data, name, msg, permissions));
			}

			return spells;
		}

		public static void WriteAllSpells(FF1Rom rom, IEnumerable<Spell> spells)
		{
			//yeah, this is pointless, but easier to construct
			var itemnames = rom.ReadText(FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextPointerCount);
			var spellPermissions = rom.Get(FF1Rom.MagicPermissionsOffset, 8 * 12).Chunk(FF1Rom.MagicCount / 8);
			var spellMessages = rom.Get(FF1Rom.MagicTextPointersOffset, FF1Rom.MagicCount);
			var spellData = rom.Get(FF1Rom.MagicOffset, FF1Rom.MagicSize * FF1Rom.MagicCount).Chunk(FF1Rom.MagicSize);

			foreach (var s in spells)
			{
				itemnames[176 + s.Index] = s.Name;
				spellMessages[s.Index] = (byte)s.Message;
				SetPermissions(spellPermissions, s.Index, s.Permissions);
				spellData[s.Index] = s.Data;
			}

			rom.WriteText(itemnames, FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextOffset);
			rom.Put(FF1Rom.MagicPermissionsOffset, Blob.Concat(spellPermissions));
			rom.Put(FF1Rom.MagicTextPointersOffset, spellMessages);
			rom.Put(FF1Rom.MagicOffset, Blob.Concat(spellData));
		}

		private static SpellPermission GetPermissions(List<Blob> spellPermissions, int i)
		{
			SpellPermission permissions = SpellPermission.None;
			for (int j = 0; j < 12; j++)
			{
				var p = (SpellPermission)(1 << j);
				if ((spellPermissions[j][i / 8] & (0x80 >> i % 8)) == 0) permissions |= p;
			}

			return permissions;
		}

		private static void SetPermissions(List<Blob> spellPermissions, int i, SpellPermission permissions)
		{
			for (int j = 0; j < 12; j++)
			{
				var p = (SpellPermission)(1 << j);
				if (!permissions.HasFlag(p))
				{
					spellPermissions[j][i / 8] = (byte)(spellPermissions[j][i / 8] | (0x80 >> i % 8));
				}
				else
				{
					spellPermissions[j][i / 8] = (byte)(spellPermissions[j][i / 8] & (0xFF - (0x80 >> i % 8)));
				}
			}
		}

		public void SetOOBSpellEffect(OOBSpellEffects slot, int spellIndex, string rngMod = null)
		{
			rom.Put(FF1Rom.MagicOutOfBattleOffset + FF1Rom.MagicOutOfBattleSize * (byte)slot, new[] { (byte)(spellIndex + 0xB0) });

			if (rngMod == null) return;

			switch (slot)
			{
				case OOBSpellEffects.CURE:
					rom.Put(0x3AF5E, Blob.FromHex(rngMod)); //290F0910
					break;
				case OOBSpellEffects.CUR2:
					rom.Put(0x3AF66, Blob.FromHex(rngMod)); //291F0920
					break;
				case OOBSpellEffects.CUR3:
					rom.Put(0x3AF6E, Blob.FromHex(rngMod)); //293F0940
					break;
				case OOBSpellEffects.HEAL:
					rom.Put(0x3AFDE, Blob.FromHex(rngMod)); //2907186910
					break;
				case OOBSpellEffects.HEL2:
					rom.Put(0x3AFE7, Blob.FromHex(rngMod)); //290F186920
					break;
				case OOBSpellEffects.HEL3:
					rom.Put(0x3AFF0, Blob.FromHex(rngMod)); //291F186940
					break;
				default:
					break;
			}
		}
	}
}
