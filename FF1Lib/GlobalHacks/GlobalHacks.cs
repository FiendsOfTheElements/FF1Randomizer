using FF1Lib.Assembly;
using System.ComponentModel;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public const int Nop = 0xEA;

		public const int CaravanFairyCheck = 0x7C4E5;
		public const int CaravanFairyCheckSize = 7;


		public void GlobalHacks()
		{
			// These are always applied since they're either required for randomization or are backend changes for other flags
			UpgradeToMMC3();
			MakeSpace();
			Bank1E();
			Bank1B();
			EasterEggs();
			PermanentCaravan();
			ShiftEarthOrbDown();
			CastableItemTargeting();
			UnifySpellSystem();
			ExpandNormalTeleporters();
			SeparateUnrunnables();
			DrawCanoeUnderBridge();
		}
		public void UpgradeToMMC3()
		{
			Header[4] = 32; // 32 pages of 16 kB
			Header[6] = 0x43; // original is 0x13 where 1 = MMC1 and 4 = MMC3

			// Expand ROM size, moving bank 0F to the end.
			Blob newData = new byte[0x80000];
			Array.Copy(Data, newData, 0x3C000);
			Array.Copy(Data, 0x3C000, newData, 0x7C000, 0x4000);
			Data = newData;

			// Update symbol info
			BA.MemoryMode = MemoryMode.MMC3;

			// Change bank swap code.
			// We put this code at SwapPRG_L, so we don't have to move any of the "long" calls to it.
			// We completely overwrite SetMMC1SwapMode, since we don't need it anymore, and partially overwrite the original SwapPRG.
			Put(0x7FE03, Blob.FromHex("8dfc6048a9068d0080680a8d018048a9078d00806869018d0180a90060"));

			// Initialize MMC3
			Put(0x7FE48, Blob.FromHex("8d00e0a9808d01a0a0008c00a08c00808c0180c88c0080c88c01808c0080c8c88c0180a9038d0080c88c0180a9048d00804ccdffa900"));
			Put(0x7FFCD, Blob.FromHex("c88c0180a9058d0080c88c01804c7cfeea"));

			// Rewrite the lone place where SwapPRG was called directly and not through SwapPRG_L.
			Data[0x7FE97] = 0x03;
		}

		public void MakeSpace()
		{
			// 54 bytes starting at 0xC265 in bank 1F, ROM offset: 7C275. FULL
			// This removes the code for the minigame on the ship, and moves the prior code around too
			PutInBank(0x1F, 0xC244, Blob.FromHex("F003C6476020C2D7A520290FD049A524F00EA9008524A542C908F074C901F0B160EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));
			// 15 bytes starting at 0xC8A4 in bank 1F, ROM offset: 7C8B4
			// This removes the routine that give a reward for beating the minigame, no need for a reward without the minigame
			PutInBank(0x1F, 0xC8A4, Blob.FromHex("EAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));
			// 28 byte starting at 0xCFCB in bank 1F, ROM offset: 7CFE1
			// This removes the AssertNasirCRC routine, which we were skipping anyways, no point in keeping uncalled routines
			PutInBank(0x1F, 0xCFCB, Blob.FromHex("EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));

			// Used by ShufflePromotions() and AllowNone()
			PutInBank(0x0E, 0xB816, Blob.FromHex("206BC24C95EC"));
			PutInBank(0x1F, 0xC26B, CreateLongJumpTableEntry(0x0F, 0x8B40));
			PutInBank(0x0F, 0x8B40, Blob.FromHex("A562851029030A851118651165110A0A0A1869508540A5100A0A29F0186928854160"));
		}

		public void ShiftEarthOrbDown()
		{
			// The orb rewarding code is inefficient enough there was room to add in giving you a shard as well.
			// When not playing shard hunt this value is unused, but we always run this code because we always
			// shuffle the earth orb orbs down 4 location to make the code simpler throughout FFR.
			// Getting free shards for killing fiends is an interesting semi-incentive and adds an edge to this game.
			// See OF_CE12_ShardRewards.asm
			Put(0x7CE12, Blob.FromHex("A202A000F010A202A001D00AA204A002D004A204A003B93160D00FA901993160188A6D35608D3560E66C1860"));
			Put(0x7CDB9, Blob.FromHex("12CE18CE1ECE24CE")); // Orb handling jump table.

			// Now anyplace that refers to orb_earth in the assembly outside of the above code
			// is going to need updating to the new address. Earth Orb is pretty popular actually.
			List<int> earthOrbPtrsLowBytes = new List<int> {
				0x39483, // Canoe Sage when not using Early Canoe
				0x3950C, // Talk_BlackOrb
				0x39529, // Talk_4Orb (bats in ToF)
				0x39561, // Talk_ifearthvamp
				0x39577, // Talk_ifearthfire
				0x3B8A0, // DrawOrbBox in the main menu
				0x7CE04, // SMMove_4Orbs
			};
			earthOrbPtrsLowBytes.ForEach(address =>
			{
				// It's entirely possible some future mods might touch these addresses so
				// let's put a litle guard here.
				System.Diagnostics.Debug.Assert(Data[address] == 0x35);
				Data[address] = 0x31;
			});

			// Fix for four NPCs checking for the Earth Orb in the wrong position (1 in Dwarf Cave, 3 in Melmond)
			Data[MapObjOffset + 0x5D * MapObjSize] = 0x11;
			Data[MapObjOffset + 0x6B * MapObjSize] = 0x11;
			Data[MapObjOffset + 0x70 * MapObjSize] = 0x11;
			Data[MapObjOffset + 0x74 * MapObjSize] = 0x11;

			Data[0x7EF45] = 0x11; // Skip over orbs and shards when printing the item menu
		}
		// Required for npc quest item randomizing
		public void PermanentCaravan()
		{
			Put(CaravanFairyCheck, Enumerable.Repeat((byte)Nop, CaravanFairyCheckSize).ToArray());
		}
		public void EasterEggs()
		{
			Put(0x2ADDE, Blob.FromHex("91251A682CFF8EB1B74DB32505FFBE9296991E2F1AB6A4A9A8BE05FFFFFFFFFFFF9B929900"));
		}
		public void DrawCanoeUnderBridge()
		{
			// Draw canoe under bridge if bridge is placed over a river, see 1F_E26A_DrawCanoeUnderBridge.asm
			PutInBank(0x1F, 0xE231, Blob.FromHex("F037"));
			PutInBank(0x1F, 0xE26A, Blob.FromHex("20A6E3A4422081E2A442C004F0032073E34C8CE3"));
		}
		public void SeparateUnrunnables()
		{
			// See SetRunnability.asm
			// replace a segment of code in PrepareEnemyFormation with a JSR to a new routine in dummied ROM space in bank 0B
			Put(0x2E141, Blob.FromHex("200C9B"));
			// move the rest of PrepareEnemyFormation up pad the excised code with NOPs
			Put(0x2E144, Get(0x2E160, 0x1E));
			PutInBank(0x0B, 0xA162, Enumerable.Repeat((byte)0xEA, 0x1C).ToArray());
			// write the new routine
			Put(0x2DB0C, Blob.FromHex("AD6A001023AD926D8D8A6DAD936D8D8B6DA2008E886D8E896D8E8C6D8E8D6DAD916D29FE8D916D60AD916D29FD8D916D60"));
			// change checks for unrunnability in bank 0C to check last two bits instead of last bit
			Put(0x313D3, Blob.FromHex("03")); // changes AND #$01 to AND #$03 when checking start of battle for unrunnability
											  // the second change is done in AllowStrikeFirstAndSurprise, which checks the unrunnability in battle
											  // alter the default formation data to set unrunnability of a formation to both sides if the unrunnable flag is set
			var formData = Get(FormationDataOffset, FormationDataSize * FormationCount).Chunk(FormationDataSize);
			for (int i = 0; i < NormalFormationCount; ++i)
			{
				if ((formData[i][UnrunnableOffset] & 0x01) != 0)
					formData[i][UnrunnableOffset] |= 0x02;
			}
			formData[126][UnrunnableOffset] |= 0x02; // set unrunnability for WzSahag/R.Sahag fight
			formData[127][UnrunnableOffset] |= 0x02; // set unrunnability for IronGol fight

			Put(FormationsOffset, formData.SelectMany(formation => formation.ToBytes()).ToArray());
		}
		public void BattleMagicMenuWrapAround()
		{
			// Allow wrapping up or down in the battle magic menu, see 0C_9C9E_MenuSelection_Magic.asm
			PutInBank(0x0C, 0x9C9E, Blob.FromHex("ADB36829F0C980F057C940F045C920F005C910F01160ADAB6A2903C903D00320D29CEEAB6A60ADAB6A2903D00320D29CCEAB6A60EEF86AADF86A29018DF86AA901200FF2201BF260"));

			// Zero out empty space
			var emptySpace = new byte[0x0A];
			PutInBank(0X0C, 0x9CE6, emptySpace);
		}
		public void UnifySpellSystem()
		{
			// ConvertOBStatsToIB
			PutInBank(0x0B, 0x9A41, Blob.FromHex("EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));

			// ConvertIBStatsToOB
			PutInBank(0x0B, 0x9A88, Blob.FromHex("EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));

			// Magic Shop
			PutInBank(0x0E, 0xAB25, Blob.FromHex("12"));
			PutInBank(0x0E, 0xAB5E, Blob.FromHex("12"));

			// Magic Menu
			PutInBank(0x0E, 0xAEF2, Blob.FromHex("00"));

			// Draw Complex Strings
			PutInBank(0x1F, 0xDF7C, Blob.FromHex("00"));
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
		public void ExpandNormalTeleporters()
		{
			// Code for extension is included in ExtraTrackingAndInitCode() in FF1Rom.cs
			//  see 0F_9200_TeleportXYInroom.asm
			const int BANK_TELEPORTINFO = 0x00;
			const int BANK_EXTTELEPORTINFO = 0x0F;

			const int lut_NormTele_X = 0xAD00;
			const int lut_NormTele_Y = 0xAD40;
			const int lut_NormTele_Map = 0xAD80;
			const int NormTele_qty = 0x40;

			const int lut_NormTele_X_ext = 0xB000;
			const int lut_NormTele_Y_ext = 0xB100;
			const int lut_NormTele_Map_ext = 0xB200;
			//const int NormTele_ext_qty = 0x100;

			var NormTele_X = GetFromBank(BANK_TELEPORTINFO, lut_NormTele_X, NormTele_qty);
			var NormTele_Y = GetFromBank(BANK_TELEPORTINFO, lut_NormTele_Y, NormTele_qty);
			var NormTele_Map = GetFromBank(BANK_TELEPORTINFO, lut_NormTele_Map, NormTele_qty);

			PutInBank(BANK_EXTTELEPORTINFO, lut_NormTele_X_ext, NormTele_X);
			PutInBank(BANK_EXTTELEPORTINFO, lut_NormTele_Y_ext, NormTele_Y);
			PutInBank(BANK_EXTTELEPORTINFO, lut_NormTele_Map_ext, NormTele_Map);
		}
	}
}
