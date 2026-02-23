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
		public const int PartyShuffleOffset = 0x312E0;
		public const int PartyShuffleSize = 3;

		public void GlobalImprovements(Flags flags, StandardMaps maps, Preferences preferences)
		{
			if ((bool)flags.NoPartyShuffle)
			{
				DisablePartyShuffle();
			}

			if ((bool)flags.IdentifyTreasures)
			{
				EnableIdentifyTreasures(Dialogues);
			}

			if ((bool)flags.ChestsAppearOpened)
			{
				EnableChestsAppearOpened();
			}

			if ((bool)flags.BuyTen || (bool)flags.Archipelago)
			{
				EnableBuyQuantity((bool)flags.Archipelago);
			}

			if ((bool)flags.WaitWhenUnrunnable)
			{
				ChangeUnrunnableRunToWait();
			}

			if ((bool)flags.SpeedHacks && (bool)flags.EnableCritNumberDisplay)
			{
				EnableCritNumberDisplay();
			}

			if ((bool)flags.InventoryAutosort && !preferences.RenounceAutosort)
			{
				EnableInventoryAutosort(flags.GameMode == GameModes.NoOverworld);
			}

			if ((bool)flags.AutoRetargeting)
			{
				EnableAutoRetargeting();
			}

			if ((bool)flags.SpeedHacks)
			{
				EnableSpeedHacks(preferences);
			}

			if ((bool)flags.Dash || (bool)flags.SpeedBoat)
			{
				EnableDash((bool)flags.SpeedBoat, preferences.OptOutSpeedHackDash);
			}

			if ((bool)flags.SpeedHacks)
			{
				SpeedHacksMoveNpcs((bool)flags.ProcgenEarth, maps);
			}
			if ((bool)flags.MagicMenuSpellReordering && (bool)flags.ShopInfo)
			{
				MagicMenuSpellReordering();
			}
			if ((bool)flags.RepeatedHealPotionUse)
			{
				RepeatedHealPotionUse();
			}

		}

		public void DisablePartyShuffle()
		{
			var nops = new byte[PartyShuffleSize];
			for (int i = 0; i < nops.Length; i++)
			{
				nops[i] = Nop;
			}
			Put(PartyShuffleOffset, nops);

			nops = new byte[2];
			for (int i = 0; i < nops.Length; i++)
			{
				nops[i] = Nop;
			}
			Put(0x39A6B, nops);
			Put(0x39A74, nops);
		}

		// Dialogues class
		public void EnableIdentifyTreasures(DialogueData dialogues)
		{
			dialogues[0xF1 + 0x50] =  "Can't hold\n#";
			dialogues[0xF1] = "Can't hold\n#";
		}

		public void EnableChestsAppearOpened()
		{
			// Open chest routine
			PutInBank(0x1F, 0xCBF9, Blob.FromHex("A9112003FE4C8BB9"));
			PutInBank(0x11, 0xB98B, Blob.FromHex("A645BD00622904F003A9F2602078DDC9F1F06148A52F186907A8A533C908D004884CB4B9C904D001C898C90F9002E90FA8A514291FC910B00E0A19E2D5850EB9F2D5850F4CE1B9290F0A19E2D5850EB9F2D50904850F20A8FEAD0220A50F8D0620A50E8D0620A97E8D0720A97F8D072020A1CC6860"));

			// Map loading routine
			PutInBank(0x1F, 0xCC45, Blob.FromHex("A9112003FE4C40B9"));
			PutInBank(0x1F, 0xCC62, Blob.FromHex("A9002003FE"));
			PutInBank(0x11, 0xB940, Blob.FromHex("A000B90004291EC908D017BE0104BD00622904F00D984AAAA97E9D0005A97F9D8005C8C8D0DCA9008511A5480A0A0A26110A26118510A6110610261118651085108A651109A085114C62CC"));
		}

		public void EnableBuyQuantity(bool archipelagoenabled)
		{
			Put(0x39E00, Blob.FromHex("ad0a0385104c668eae0c03bd2060186d0a03c9649001609d206060a903203baaa9018d0a03a520290f856120909f2032aa2043a7a525d056a524d05aa520290fc561f0ed8561c900f0e7c904f02fc908f01ac901f00ace0a03d0d0ee0a03d0cbee0a03c964d0c4ce0a03d0bfad0a0318690a8d0a03c96490b2a96310f5ad0a0338e90af0021002a9018d0a03109d38a90085248525601890f6"));
			Put(0x39E99, Blob.FromHex("a90e205baaa5620a0a0a186916aabd00038d0c0320b9ecae0a03a9008d0b038d0e038d0f0318ad0b0365108d0b03ad0e0365118d0e03ad0f0369008d0f03b005caf00dd0e1a9ff8d0b038d0e038d0f03ad0f038512ad0e038511ad0b03851020429f2032aa60"));
			Put(0x39EFF, Blob.FromHex("ad1e60cd0f03f0049016b016ad1d60cd0e03f004900ab00aad1c60cd0b03b00238601860ad1c6038ed0b038d1c60ad1d60ed0e038d1d60ad1e60ed0f038d1e604cefa74c8e8e"));
			Put(0x3A494, Blob.FromHex("201b9eb0e820999e20c2a8b0e0a562d0dc20ff9e9008a910205baa4c81a420089e9008a90c205baa4c81a420239fa913205baa4c81a4eaeaea"));

			PutInBank(0x0E, 0x9F90, Blob.FromHex("A5620A0A0A186916A8BE0003BC2060AD0A038CEF6A186DEF6AE963300EA963EDEF6AF0021002A9018D0A034C009E"));

			if (archipelagoenabled)
			{
				//Replace NewCheckforSpace with patch in 0E_9F48_ItemShopCheckForSpace.asm
				PutInBank(0x0E, 0xA4B2, Blob.FromHex("20489F"));
				PutInBank(0x0E, 0x9F48, Blob.FromHex("AE0C03E016900CBD2060186D0A03C964900D60A2FFBD006209029D006218609D20601860"));
			}
		}

		public void ChangeUnrunnableRunToWait()
		{
			// See Unrunnable.asm
			// Replace DrawCommandMenu with a cross page jump to a replacement that swaps RUN for WAIT if the battle is unrunnable.
			// The last 5 bytes here are the null terminated WAIT string (stashed in some leftover space of the original subroutine)
			Put(0x7F700, Blob.FromHex("ADFC6048A90F2003FE204087682003FE4C48F6A08A929D00"));

			// Replace some useless code with a special handler for unrunnables that prints 'Nothing Happens'
			// We then update the unrunnable branch to point here instead of the generic Can't Run handler
			// See Disch's comments here: Battle_PlayerTryRun  [$A3D8 :: 0x323E8]
			Put(0x32409, Blob.FromHex("189005A94E4C07AAEAEAEAEAEAEAEA"));
			// new delta to special unrunnable message handler done in
		}
		public void EnableInventoryAutosort(bool noOverworld)
		{
			string finalChunk = "60";

			if (noOverworld)
			{
				finalChunk = "AD1260F00DA911990003E663C8A90099000360";
			}

			//see 0F_8670_SortInventory.asm
			Put(0x7EF58, Blob.FromHex("A90F2003FE20008DEAEAEA"));
			PutInBank(0x0F, 0x8D00, Blob.FromHex("8663A9009D0003A900856218A000A219BD2060F0058A990003C8E8E01CD0F1A216BD2060F0058A990003C8E8E019D0F1A21CBD2060F0058A990003C8E8E020D0F1A200BD2060F0058A990003C8E8E011D0F1" + finalChunk));
		}

		public void EnableAutoRetargeting()
		{
			//see 1C_A250_AutoRetargeting.asm
			//melee routines
			PutInBank(0x0C, 0xA4C2, Blob.FromHex("4CF492EAEAEA"));
			PutInBank(0x0C, 0x92F4, Blob.FromHex("A9A248A94F48A91C4C03FE"));
			PutInBank(0x1C, 0xA250, Blob.FromHex("BDB76BC9FFD00DA209BDB66BC9FFD003CAD0F6CA8E85688E8A6CA9A448A9C748A90C4C03FE"));

			//magic routines
			PutInBank(0x0C, 0xB3CD, Blob.FromHex("A9A148A9BF48A91C4C03FE"));
			PutInBank(0x1C, 0xA1C0, Blob.FromHex("98290309808D896C8DD16B8AC909B014BDB76BC9FFD00DA209BDB66BC9FFD003CAD0F6CAA9B348A9D748A90C4C03FE"));
		}

		public void EnableCritNumberDisplay()
		{
			// Overwrite the normal critical hit handler by calling ours instead
			PutInBank(0x0C, 0xA94B, Blob.FromHex("20D1CFEAEA"));
			PutInBank(0x1F, 0xCFD1, CreateLongJumpTableEntry(0x0F, 0x92A0));
		}

		public void ActivateCropScreen()
		{
			//PutInBank(0x0E, 0xA222, Blob.FromHex("20D0A0"));
			//PutInBank(0x0E, 0xA0D0, Blob.FromHex("2006E9A9038DD00360"));

			PutInBank(0x1F, 0xE8FD, Blob.FromHex("4CE0DD"));
			PutInBank(0x1F, 0xDDE0, Blob.FromHex("20B9EAA9038DD00360"));
		}

		private void MagicMenuSpellReordering()
		{
			//see 1C_A600_MagicMenuSpellReordering.asm
			PutInBank(0x0E, 0xAECD, Blob.FromHex("4CF5BF"));
			PutInBank(0x0E, 0xBFF5, Blob.FromHex("A9A548A9FF48A91C4C03FE"));
			PutInBank(0x1C, 0xA600, Blob.FromHex("A523F039A9008523A5622502D02FA5662A2A2A2A2A2A6562AABD0063F01FA8E8BD0063F018CA9D006398E89D0063A9008523A9AE48A99648A90E4C03FEA99148A92548A90E4C03FE"));
		}

		private void RepeatedHealPotionUse()
		{
			//see 1A_8600_RepeatedHealPotionUse.asm
			PutInBank(0x0E, 0xB321, Blob.FromHex("A98548A9FF48A91A4C03FE"));
			PutInBank(0x1A, 0x84FB, Blob.FromHex("4C4EE0"));
			PutInBank(0x1A, 0x8600, Blob.FromHex("A91E20428620B886CE3960F01B20C687B00BA9B348A90D48A90E4C03FEA9B348A92E48A90E4C03FE209787A5240525F0F7A9" +
				"0085248525A9B348A92E48A90E4C03FE8510187D0A619D0A61BD0B6169009D0B61DD0D61F01BB023A957854BA9308D0440A97F8D0540A9008D06408D0740A51060BD0" +
				"A61DD0C61B00290DDBD0C619D0A61BD0D619D0B614C5A86AD0220A9208D0620A9008D0620A00098A2038D0720C8D0FACAD0F78D0720C8C0C090F8A9FF8D0720C8D0FA" +
				"60A9008D01208537208C86A90B8539A9018538A91E853CA908853D2063E020DC864C658760A200A904853AA911853B20FD86C63BA20220FD86C63BA20420FD86C63BC" +
				"63BA206BD0A87853EBD0B87853F4C8E87128728873E8753877A1006FFFFFF7A1106FFFFFF7A1206FFFFFF7A130600FF1005FFFFFFFF1105FFFFFFFF1205FFFFFFFF13" +
				"05001002FFFFFFFF1102FFFFFFFF1202FFFFFFFF1302001000FFFFFF1100FFFFFF1200FFFFFF130000203CC420A8FEA9028D14402050D8A90885FF8D0020A91E8D012" +
				"0A9008D05208D0520A91A85574C89C6A91A855785584C36DE20A8FEA9028D1440A5FF8D0020A9008D05208D0520A54B1004A951854BA91A85572089C6E6F0A9008524" +
				"85254CC2D7A90085248525203CC4201088209787A524D02DA525D02EA5202903C561F0E18561C900F0DBC901D008A5621869014CFC87A56238E901290385622023884" +
				"CC68720398818602039883860A662BD1F888540A96885414C95EC60104880B8A97A8D0440A99B8D0540A9208D06404A8D0740857E60A9BA8D0440A9BA8D0540A9408D" +
				"0640A9008D0740A91F857E60"));
		}

	}
}
