using System.ComponentModel;

namespace FF1Lib
{
	public static partial class FlagRules
	{
		public static FlagRule BugFixes { get; set; } = new FlagRule()
		{
			Conditions = new() { new() { new FlagCondition() { Name = "Bugfixes", Type = SettingType.Toggle, Value = 1 } } },
			Actions = new List<FlagAction>()
				{
					new FlagAction() { Setting = "FixHouseMP", Action = FlagActions.Enable },
					new FlagAction() { Setting = "FixHouseHP", Action = FlagActions.Enable },
					new FlagAction() { Setting = "FixWeaponStats", Action = FlagActions.Enable },
					new FlagAction() { Setting = "FixSpellBugs", Action = FlagActions.Enable },
					new FlagAction() { Setting = "FixEnemyStatusAttack", Action = FlagActions.Enable },
					new FlagAction() { Setting = "FixBBAbsorbBug", Action = FlagActions.Enable },
					new FlagAction() { Setting = "ChanceToRun", Action = FlagActions.SetValue, Value = (int)ChanceToRunMode.Fixed },
				}
		};
	}
	public partial class FF1Rom
	{


		private void Bugfixes(Settings settings)
		{
			bool fixhousemp = settings.GetBool("FixHouseMP");
			bool fixhousehp = settings.GetBool("FixHouseHP");
			bool ether = settings.GetBool("Etherizer");

			if (!ether)
			{
				FixHouse(fixhousehp, fixhousemp);
			}

			if (settings.GetBool("FixWeaponStats"))
			{
				FixWeaponStats();
			}

			if (settings.GetBool("FixSpellBugs"))
			{
				FixSpellBugs();
				FixEnemyAOESpells();
				FixEnemyElementalResistances();
			}

			if (settings.GetBool("FixEnemyStatusAttack"))
			{
				FixEnemyStatusAttackBug();
			}

			if (settings.GetBool("FixBBAbsorbBug"))
			{
				FixBBAbsorbBug();
			}
			
			FixWarpBug(); // The warp bug must be fixed for magic level shuffle and spellcrafter
			FixEnemyPalettes(); // fixes a bug in the original game's programming that causes third enemy slot's palette to render incorrectly
			Fix3DigitStats();
		}
		public void FixHouse(bool MPfix, bool HPfix)
		{
			if (!MPfix && !HPfix)
			{
				return;
			}

			Put(0x03B2BE, Blob.FromHex("A52D4AB018CE3860A978203FB5EAEAEAA91E20E0B2900320F3AB4C1DB1A91F4CB8B2"));

			if (MPfix)
			{
				Put(0x03B2CE, Blob.FromHex("20F3ABA91E20E0B2EAEA"));
				Put(0x038816, Blob.FromHex("203B42A4AAACA6FF23A6B23223A7C0059C8A9F8EC5FFFFFFFFFFFFFF"));
			}

			if (HPfix)
			{
				Put(0x03b2c8, Blob.FromHex("20D2AB2000B4"));
			}
		}

		public void FixWeaponStats()
		{
			// Move function pointer
			Put(0x031322, new byte[] { 0xF2 });
			// Move and rewrite function
			Put(0x032CE1, Blob.FromHex("B18248C8B182488AA86891808868918060A9002006ADA9012006ADA9022006ADA9034C06AD8DB3682045A1A000B182A8B93CA0ACB36899A86BADB368A0009180A20220E1ACA00AA20420E1ACA021A20620E1ACA025A20720E1ACA023A20820E1ACA022A20920E1ACA020A20A20E1ACA024A20B20E1ACA901A00B9180A021B1824A4A4A4A4A186901A00C9180A900A00D9180C89180C89180A018B1823007C8C01CD0F7A900297FF02DE9002005AC85888689A002B188A00F9180A005B188A00D9180A004B188A00E9180"));

			// Fix player elemental and category defense
			Put(0x325B0, Blob.FromHex("A9008D6D68A00E"));
			Put(0x325E8, Blob.FromHex("A9008D7668A00EA900"));
			Put(0x33618, Blob.FromHex("A900"));
			Put(0x33655, Blob.FromHex("A900"));
		}
		public void FixWarpBug()
		{
			Put(0x3AEF3, Blob.FromHex("187D0063")); // Allows last slot in a spell level to be used outside of battle
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

			TameExitAndWarpBoss();

			// Cure 4 fix, see 0E_AF7C_Cure4.asm
			Put(0x3AF80, Blob.FromHex("36"));
			Put(0x3AF8F, Blob.FromHex("2AC902F026A564C900F0062061B54CACAFBD0D619D0B61BD0C619D0A612000B4A665DE00632013B64C97AE2026DB4C7CAF0000000000000000000000002000B4A92B202BB9A9008D64004C7CAF"));
			Put(0x3AF13, Blob.FromHex("CC")); // update address for Cure4 routine

			// Better Slow battle message
			Put(0x6C11F, FF1Text.TextToBytes("Attack rate down", false, FF1Text.Delimiter.Null));
		}

		public void FixEnemyStatusAttackBug()
		{
			Put(0x32812, Blob.FromHex("DF")); // This is the craziest freaking patch ever, man.
		}

		public void FixBBAbsorbBug()
		{
			PutInBank(0x1B, 0x874A + 0x5, Blob.FromHex("A000B186C902F005C908F00160A018B186301BC8B1863016C8B1863011C8B186300CA026B1861869010AA0209186A01CB186301AC8B1863015C8B1863010C8B186300BA026B186186901A022918660"));
			//PutInBank(0x0B, 0x9966, Blob.FromHex("2046D860"));
			//PutInBank(0x1F, 0xD846, CreateLongJumpTableEntry(0x0F, 0x8800));


			// Fixes documented in 1B_BBUnarmedLUTs.asm

			// Overwrite BB check with Unarmed LUT check
			PutInBank(0x1B, 0x874A, Blob.FromHex("A90085EDA9B285EEA000B1862003B0"));

			// Overwrite UnadjustBBEquipStats
			PutInBank(0x1F, 0xEEBF, Blob.FromHex("A91B2003FE2040B098F0"));
			PutInBank(0x1F, 0xEEDD, Blob.FromHex("A91B2003FE2040B098F0"));

			PutInBank(0x1B, 0xB040, Blob.FromHex("A90085EDA9B285EEBD00612003B0A8A90C4C03FE"));
		}

		public void FixEnemyElementalResistances()
		{
			// make XFER and other elemental resistance changing spells affect enemies
			// Replace second copy of low bye for hp with elemental resistance
			Put(0x32FE1, Blob.FromHex("13"));
			// Switch to reading elemental resistance from ROM to RAM and make room for the extra byte
			Put(0x3370A, Blob.FromHex("A012B1908D7768A009B1908D7E68B1928D7F68C8B1908D8568C8B1908D82684CFABB00000000"));
			// add JSR to new routine for the extra room
			Put(0x3378C, Blob.FromHex("20B5B6"));
			// move 3 byes from previous subroutine and save elemental resistance of the enemy
			Put(0x336B5, Blob.FromHex("C89190AD7768A01291906000000000000000")); // extra room at the end for new code
		}

		public void FixEnemyAOESpells()
		{
			// Remove comparison and branch on equal which skips the caster when casting aoe spells
			Put(0x33568, Blob.FromHex("EAEAEAEAEAEAEAEA"));
		}

		public void TameExitAndWarpBoss()
		{
			// See 0E_9C54_ExitBoss.asm for this
			PutInBank(0x0E, 0x9C54, Blob.FromHex("205DB6AD2400D012AD2500F0F32084AD6868A9008D25004C97AE2084ADA9008D2400AE6500DE006360"));

			Put(0x3B0D6, Blob.FromHex("EAEAEAEAEA20549C")); // Warp
			Put(0x38A77, Blob.FromHex("A02FB3315EAE36B1A8FFA9AFB2B2B50599B8B6ABFF8BFFB7B2FFA4A5B2B5B7")); // Text
			Put(0x38B5A, Blob.FromHex("A0A4B5B3FFA5A4A6AEFFB2B1A805A9AFB2B2B5C099B8B6ABFF8BFFB7B2FFA4A5B2B5B700FFFFFFFFFFFFFF"));

			Put(0x3B0F7, Blob.FromHex("EAEAEAEAEA20549C")); // Exit
			Put(0x38AB3, Blob.FromHex("95B237C5FF972E5D4B26B7C5FF9EB61A1C3005B6B3A84E1B2EA8BB5BC40599B8B6ABFF8BFF2820A53521")); // Text
			Put(0x38BAA, Blob.FromHex("95B2B6B7C5FF97B2FFBAA4BCFFB2B8B7C5059EB6A8FFB7ABACB605B6B3A8AFAFFFB7B2FFA8BBACB7C4FF99B8B6ABFF8BFFB7B2FFA4A5B2B5B7"));
		}
		public void FixEnemyPalettes()
		{
			Data[0x2E382] = 0xEA; // remove an extraneous LSR A when drawing monsters in a Large-Small mixed formation, so that the enemy in the third monster slot in such formations uses the correct palette
		}

	    public void Fix3DigitStats()
		{
			// Fix character stat rendering so basic stats are
			// rendered properly for values over 99
			// See 0E_8DE4_FixPrintCharStat.asm
			PutInBank(0x0E, 0x8DE4, Blob.FromHex("A910D010A911D00CA925D008A913D004A914D000186567AABD00618510A90085114C708EEAEAEAEAEAEAEAEA"));
	    }
	}
}
