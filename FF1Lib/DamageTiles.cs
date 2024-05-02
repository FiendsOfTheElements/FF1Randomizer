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
			// Adjustable lava damage - run if anything other than the default of 1 damage
			if ((int)flags.DamageTileLow != 1 || (int)flags.DamageTileHigh != 1)
			{
				int DamageTileAmount = rng.Between(flags.DamageTileLow, flags.DamageTileHigh);
				AdjustDamageTileDamage(DamageTileAmount, (bool)flags.DamageTilesKill, (bool)flags.ArmorResistsDamageTileDamage);
			}

			if ((bool)flags.OWDamageTiles || flags.DesertOfDeath)
			{
				EnableDamageTile(overworld);
			}
			if ((bool)flags.DamageTilesKill)
			{
				DamageTilesKill(flags.SaveGameWhenGameOver);
			}

			if ((bool)flags.ArmorResistsDamageTileDamage && !(bool)flags.ArmorCrafter)
			{
				EnableArmorDamageTileResist((bool)flags.DamageTilesKill);
			}
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

		public void DamageTilesKill(bool saveonDeath)
		{
			// See 1E_B100_DamageTilesKill.asm
			string saveOnDeathJump = "4C12C0";
			if (saveonDeath)
			{
				saveOnDeathJump = "EAEAEA";
			}

			PutInBank(0x1E, 0xB100, Blob.FromHex($"A200A000BD0161C901F060A901D043A5F2C903903DA548C90EF013C921F00FC922F00BC923F007C924F0034C38B1A98820FDB1D01D4C6CB1C90FF00CC925F008C926F004C927F000A98720FDB1F0034C6CB1BD0B61D021BD0A61C902B01AA9019D0161A9009D0A614C6BB1C88A186940AAD091C004F01560BD0A6138E9019D0A61BD0B61E9009D0B614C6CB1A980854BA91E8557A52D6AA9009002A902851C2000FE2082D920B3D9A9064820EFD968A88898D0F6A0402000FE2018D92082D9A91E8D01202089C688D0ECA952854B20EFB1A9008D00208D0120{saveOnDeathJump}A9C048A91148A98F48A9F748A91B85574C03FE2028D8482000FE2089C668F0F360841185108AA8B91C61C510D005A411A90060C8982904C904D0ECA411A90160"));
			//Original code below - above edit is to prevent a bug with adjustable lava damage (spaces added to line up cleanly with the new code for comparison)
			//PutInBank(0x1E, 0xB100, Blob.FromHex($"A200A000BD0161C901F026BD0B61D00DBD0A61C902B006A9019D0161    C8          BD0A6138E9019D0A61BD0B61E9009D0B614C32B1C88A186940AAD0CBC004F00160A980854BA91E8557A52D6AA9009002A902851C2000FE2082D920B3D9A9064820EFD968A88898D0F6A0402000FE2018D92082D9A91E8D01202089C688D0ECA952854B20A1B1A9008D00208D0120{saveOnDeathJump}A9C048A91148A98F48A9F748A91B85574C03FE2028D8482000FE2089C668F0F360"));
			PutInBank(0x1F, 0xC861, Blob.FromHex("A91E2003FE4C00B1"));
		}

		public void AdjustDamageTileDamage(int DamageTileAmount, bool isDamageTilesKillOn, bool IsArmorResistsDamageTileDamageOn)
		{
			// Overwrites a bit of the newly created DamageTilesKill assembly code to account for the adjustable damage
			if (isDamageTilesKillOn)
			{
				PutInBank(0x1E, 0xB155, Blob.FromHex($"{DamageTileAmount + 1:X2}"));
				PutInBank(0x1E, 0xB16A, Blob.FromHex($"{DamageTileAmount:X2}"));
			}
			else if (IsArmorResistsDamageTileDamageOn)
			{
				PutInBank(0x1E, 0xB14C, Blob.FromHex($"{DamageTileAmount + 1:X2}"));
				PutInBank(0x1E, 0xB15C, Blob.FromHex($"{DamageTileAmount:X2}"));
			}
			// No Lethal Damage Tiles Flag, overwrite normal rom code instead
			else
			{
				Data[0x7C86C] = (byte)(DamageTileAmount + 1); //"HP Less than" check so it doesn't kill
				Data[0x7C874] = (byte)(DamageTileAmount);
			}
		}

		public void EnableArmorDamageTileResist(bool isDamageTilesKillOn)
		{
			// See 1E_B100_DamageTilesArmorResist.asm
			if (isDamageTilesKillOn)
			{
				// DamageTilesKill already has the necessary code inline, so just enable it
				PutInBank(0x1E, 0xB10C, Blob.FromHex($"00"));
			}
			else
			{
				PutInBank(0x1F, 0xC861, Blob.FromHex("A91E2003FE4C00B1"));
				PutInBank(0x1E, 0xB100, Blob.FromHex($"A200A901D043A5F2C903903DA548C90EF013C921F00FC922F00BC923F007C924F0034C2FB1A9882076B1D01D4C6EB1C90FF00CC925F008C926F004C927F000A9872076B1F0034C6EB1BD0B61D00FBD0A61C902B008A9019D0A614C6EB1BD0A6138E9019D0A61BD0B61E9009D0B618A186940AAD08D6085108AA8B91C61C510D003A90060C8982904C904D0EEA90160"));
				PutInBank(0x1E, 0xB103, Blob.FromHex($"00"));
			}
		}
	}
}
