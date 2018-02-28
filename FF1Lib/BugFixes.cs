using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomUtilities;

namespace FF1Lib
{
	public partial class FF1Rom
	{
		public void FixHouse()
		{
			Put(0x03B2CB, Blob.FromHex("20F3ABA91E20E0B2EAEA"));
			Put(0x038816, Blob.FromHex("203B42A4AAACA6FF23A6B23223A7C0059C8A9F8EC5FFFFFFFFFFFFFF"));
		}

		public void FixWeaponStats()
		{
			// Move function pointer
			Put(0x031322, new byte[] { 0xF2 });
			// Move and rewrite function
			Put(0x032CE1, Blob.FromHex("B18248C8B182488AA86891808868918060A9002006ADA9012006ADA9022006ADA9034C06AD8DB3682045A1A000B182A8B93CA0ACB36899A86BADB368A0009180A20220E1ACA00AA20420E1ACA021A20620E1ACA025A20720E1ACA023A20820E1ACA022A20920E1ACA020A20A20E1ACA024A20B20E1ACA901A00B9180A021B1824A4A4A4A4A186901A00C9180A900A00D9180C89180C89180A018B1823007C8C01CD0F7A900297FF02DE9002005AC85888689A002B188A00F9180A005B188A00D9180A004B188A00E9180"));

			// Don't double BB crit
			Put(0x32DDD, new byte[] { 0xEA });

			// Increase crit rate of all weapons
			var weapons = Get(WeaponOffset, WeaponSize * WeaponCount).Chunk(WeaponSize);
			foreach (var weapon in weapons)
			{
				weapon[2] *= 2;
			}
			Put(WeaponOffset, weapons.SelectMany(weapon => weapon.ToBytes()).ToArray());

			// Change damage bonus from +4 to +10
			Put(0x326F5, Blob.FromHex("0A"));

			// Fix player elemental and category defense
			Put(0x325B0, Blob.FromHex("A9008D6D68A00E"));
			Put(0x325E8, Blob.FromHex("A9008D7668A00EA900"));
			Put(0x33618, Blob.FromHex("A900"));
			Put(0x33655, Blob.FromHex("A900"));
		}

		public void FixChanceToRun()
		{
			Put(0x323EF, new byte[] { 0x82 });
		}

		public void FixSpellBugs()
		{
			Put(0x33A4E, Blob.FromHex("F017EA")); // LOCK routine
			Put(0x3029C, Blob.FromHex("0E")); // LOK2 spell effect
			Put(0x302F9, Blob.FromHex("18")); // HEL2 effectivity

			// TMPR and SABR
			// Remove jump to PrepareEnemyMagAttack
			Put(0x334F4, Blob.FromHex("EAEAEA"));
			// Replace PrepareEnemyMagAttack with loading defender strength and hit%
			Put(0x33730, Blob.FromHex("A006B1908D7268A009B1908D8268A005B1908D846860"));
			// Replace end of PreparePlayerMagAttack with saving defender strength and hit%
			Put(0x3369E, Blob.FromHex("60A007AD85689190A009AD82689190A005AD8468919060"));
			// Call new loading code from BtlMag_LoadPlayerDefenderStats
			Put(0x33661, Blob.FromHex("2030B7EAEAEAEA"));
			// Call new saving code from BtlMag_SavePlayerDefenderStats
			Put(0x337C5, Blob.FromHex("209FB6EAEAEAEA"));
			// SABR's hit% bonus
			Put(0x30390, Blob.FromHex("0A"));
		}

		public void FixEnemyStatusAttackBug()
		{
			Put(0x32812, Blob.FromHex("DF")); // This is the craziest freaking patch ever, man.
		}

		public void FixEnmeyAOESpells()
		{
			// Remove comparison and branch on equal which skips the caster when casting aoe spells
			Put(0x33568, Blob.FromHex("EAEAEAEAEAEAEAEA"));
		}
	}
}
