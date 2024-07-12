using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public partial class FF1Rom
	{
		private void DamageTilesHack(Flags flags, Overworld overworld)
		{
			int damageTileAmount = 1;

			// Adjustable lava damage - run if anything other than the default of 1 damage
			if ((int)flags.DamageTileLow != 1 || (int)flags.DamageTileHigh != 1)
			{
				damageTileAmount = rng.Between(flags.DamageTileLow, flags.DamageTileHigh);
				// Always write at vanilla location, it will be ignored if other flags are enabled
				PutInBank(0x1F, 0xC86C, Blob.FromHex($"{(damageTileAmount + 1):X2}"));
				PutInBank(0x1F, 0xC874, Blob.FromHex($"{damageTileAmount:X2}"));
			}

			if ((bool)flags.OWDamageTiles || flags.DesertOfDeath)
			{
				EnableDamageTile(overworld);
			}

			DamageTilesKillAndCanBeResisted(damageTileAmount, (bool)flags.ArmorResistsDamageTileDamage && !(bool)flags.ArmorCrafter, (bool)flags.DamageTilesKill, flags.SaveGameWhenGameOver);
		}

		public void EnableDamageTile(Overworld overworld)
		{
			// Allow tiles to do Walk Damage, see 1E_B000_DoWalkDamage.asm
			PutInBank(0x1E, 0xB000, Blob.FromHex("A542C901D00E20FBC7A54429E0C9E0D0034CDEC760"));
			PutInBank(0x1F, 0xC33C, Blob.FromHex("A91E2003FE4C00B0"));

			PutInBank(0x0E, 0xB267, Blob.FromHex("E0")); // Expand OWTP_SPEC_MASK to 0b1110_0000
			PutInBank(0x1F, 0xC4DA, Blob.FromHex("E0"));
			PutInBank(0x1F, 0xC6B4, Blob.FromHex("E0"));

			PutInBank(0x1F, 0xC2EC, Blob.FromHex("E1")); // Update mask for docking

			List<int> owDamageTileList = new() { 0x45, 0x55, 0x62, 0x63, 0x72, 0x73 };

			foreach (var tile in owDamageTileList)
			{
				var tileProperty = overworld.TileSet.Tiles[tile];
				tileProperty.PropertyType = (byte)(tileProperty.PropertyType | 0b1110_0000);
			}
		}

		public void DamageTilesKillAndCanBeResisted(int damageTileAmount, bool armorResistEnabled, bool damageTileKill, bool saveonDeath)
		{
			if (!armorResistEnabled && !damageTileKill)
			{
				return;
			}

			// See 1E_B100_DamageTilesKill.asm
			string saveOnDeathJump = saveonDeath ? "EAEAEA" : "4C12C0";
			string armorResists = armorResistEnabled ? "00" : "01";
			string setDeadStatus = damageTileKill ? "A9019D0161A9009D0A614C6BB1" : "EAEAEAEAEAA9019D0A614C6CB1";

			//Hack this out
			PutInBank(0x1E, 0xB100, Blob.FromHex(
				// AssignMapTileDamage
				$"A200A000BD0161C901F060" +
				// Armor Resist Check
				$"A9{armorResists}D043A5F2C903F03D" +
				$"A548C90EF013C921F00FC922F00BC923F007C924F0034C38B1A98820FDB1D01D4C6CB1C90FF00CC925F008C926F004C927D00AA98720FDB1D0034C6CB1" +
				// AssignDamage
				$"BD0B61D021BD0A61C9{(damageTileAmount+1):X2}B01A{setDeadStatus}C88A186940AAD091C004F01560" +
				// DmgSubtract
				$"BD0A6138E9{damageTileAmount:X2}9D0A61BD0B61E9009D0B614C6CB1" +
				// MapGameOver
				$"A980854BA91E8557A52D6AA9009002A902851C2000FE2082D920B3D9A9064820EFD968A88898D0F6A040" +
				// WaitLoop
				$"2000FE2018D92082D9A91E8D01202089C688D0ECA952854B20EFB1A9008D00208D0120{saveOnDeathJump}A9C048A91148A98F48A9F748A91B85574C03FE" +
				// WaitForAnyInput
				$"2028D8482000FE2089C668F0F360841185108AA8B91C61C510D005A411A90060C8982904C904D0ECA411A90160"));

			PutInBank(0x1F, 0xC861, Blob.FromHex("A91E2003FE4C00B1"));
		}
	}
}
