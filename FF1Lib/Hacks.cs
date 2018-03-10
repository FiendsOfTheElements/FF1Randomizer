using System;
using System.Linq;
using RomUtilities;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public const int Nop = 0xEA;
		public const int SardaOffset = 0x393E9;
		public const int SardaSize = 7;
		public const int CanoeSageOffset = 0x39482;
		public const int CanoeSageSize = 5;
		public const int PartyShuffleOffset = 0x312E0;
		public const int PartyShuffleSize = 3;
		public const int MapSpriteOffset = 0x03400;
		public const int MapSpriteSize = 3;
		public const int MapSpriteCount = 16;

		public const int CaravanFairyCheck = 0x7C4E5;
		public const int CaravanFairyCheckSize = 7;

		// Required for npc quest item randomizing
		public void PermanentCaravan()
		{
			Put(CaravanFairyCheck, Enumerable.Repeat((byte)Nop, CaravanFairyCheckSize).ToArray());
		}
		// Required for npc quest item randomizing 
		// Doesn't substantially change anything if EnableNPCsGiveAnyItem isn't called
		public void CleanupNPCRoutines()
		{
			// Have ElfDoc set his own flag instead of the prince's so that 
			// the prince can still set his own flag after giving a shuffled item
			Data[0x39302] = (byte)ObjectId.ElfDoc;
			Data[0x3931F] = (byte)ObjectId.ElfDoc;

			// Convert Talk_ifcanoe into Talk_ifairship
			Data[0x39534] = UnsramIndex.AirshipVis;
			// Point Talk_ifairship person to old Talk_ifcanoe routine
			Data[0x391B5] = 0x33;
			Data[0x391B6] = 0x95;

			// Then we move Talk_earthfire to Talk_norm to clear space for 
			// new item gift routine without overwriting Talk_chime
			Data[0x391D3] = 0x92;
			Data[0x391D4] = 0x94;

			// Swap string pointer in index 2 and 3 for King, Bikke, Prince, and Lefein
			var temp = Data[ItemLocations.KingConeria.Address];
			Data[ItemLocations.KingConeria.Address] = Data[ItemLocations.KingConeria.Address - 1];
			Data[ItemLocations.KingConeria.Address - 1] = temp;
			temp = Data[ItemLocations.Bikke.Address];
			Data[ItemLocations.Bikke.Address] = Data[ItemLocations.Bikke.Address - 1];
			Data[ItemLocations.Bikke.Address - 1] = temp;
			temp = Data[ItemLocations.ElfPrince.Address];
			Data[ItemLocations.ElfPrince.Address] = Data[ItemLocations.ElfPrince.Address - 1];
			Data[ItemLocations.ElfPrince.Address - 1] = temp;
			temp = Data[ItemLocations.CanoeSage.Address - 1];
			Data[ItemLocations.CanoeSage.Address - 1] = Data[ItemLocations.CanoeSage.Address - 2];
			Data[ItemLocations.CanoeSage.Address - 2] = temp;
			temp = Data[ItemLocations.Lefein.Address];
			Data[ItemLocations.Lefein.Address] = Data[ItemLocations.Lefein.Address - 1];
			Data[ItemLocations.Lefein.Address - 1] = temp;

			// And do the same swap in the vanilla routines so those still work if needed
			Data[0x392A7] = 0x12;
			Data[0x392AA] = 0x13;
			Data[0x392FC] = 0x13;
			Data[0x392FF] = 0x12;
			Data[0x39326] = 0x12;
			Data[0x3932E] = 0x13;
			Data[0x3959C] = 0x12;
			Data[0x395A4] = 0x13;

			// When getting jump address from lut_MapObjTalkJumpTbl (starting 0x3902B), store
			// it in tmp+4 & tmp+5 (unused normally) instead of tmp+6 & tmp+7 so that tmp+6
			// will still have the mapobj_id (allowing optimizations in TalkRoutines)
			Data[0x39063] = 0x14;
			Data[0x39068] = 0x15;
			Data[0x3906A] = 0x14;
			Data[0x39070] = 0x14;
			Data[0x39075] = 0x15;
			Data[0x39077] = 0x14;
		}

		public void EnableEarlyRod()
		{
			var nops = new byte[SardaSize];
			for (int i = 0; i < nops.Length; i++)
			{
				nops[i] = Nop;
			}

			Put(SardaOffset, nops);
		}

		public void EnableEarlyCanoe()
		{
			var nops = new byte[CanoeSageSize];
			for (int i = 0; i < nops.Length; i++)
			{
				nops[i] = Nop;
			}

			Put(CanoeSageOffset, nops);
		}

		public void PartyRoulette()
		{
			// First, disable the 'B' button to prevent going back after roulette spin
			Data[0x39C92] = 0xF7; // F7 here is really a relative jump value of -9
			Data[0x39C9B] = 0xF7;
			Data[0x39CA4] = 0xF7;
			Data[0x39CBD] = 0xEA;
			Data[0x39CBE] = 0xEA;

			// Then skip the check for directionaly input and just constantly cycle class selection 0 through 5
			Put(0x39D25, Enumerable.Repeat((byte)0xEA, 14).ToArray());
		}

		public void PartyRandomize(MT19337 rng, int numberForced)
		{
			// Always randomize all 4 default members (but don't force if not needed)
			Data[0x3A0AE] = (byte)rng.Between(0, 5);
			Data[0x3A0BE] = (byte)rng.Between(0, 5);
			Data[0x3A0CE] = (byte)rng.Between(0, 5);
			Data[0x3A0DE] = (byte)rng.Between(0, 5);

			if (numberForced <= 0)
				return;

			Data[0x39D35] = 0xE0;
			Data[0x39D36] = (byte)(numberForced * 0x10);
			Put(0x39D37, Blob.FromHex("30DFFE0003BD0003E906D0039D0003A9018537"));
			/* Starting at 0x39D35 (which is just after LDX char_index)
				* CPX ____(numberForced * 0x10)____
				* BMI @MainLoop
				* INC ptygen_class, X
				* LDA ptygen_class, X
				* SBC #$06
				* BNE :+
				*   STA ptygen_class, X
				* : LDA #$01
				*   STA menustall
				*/
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

		public void EnableSpeedHacks()
		{
			// Screen wipe
			Data[0x7D6EE] = 0x08; // These two values must evenly divide 224 (0xE0), default are 2 and 4
			Data[0x7D6F5] = 0x10;
			Data[0x7D713] = 0x0A; // These two values must evenly divide 220 (0xDC), default are 2 and 4
			Data[0x7D71A] = 0x14; // Don't ask me why they aren't the same, it's the number of scanlines to stop the loop at

			// Dialogue boxes
			Data[0x7D620] = 0x0B; // These two values must evenly divide 88 (0x58), the size of the dialogue box
			Data[0x7D699] = 0x0B;

			// Battle entry
			Data[0x7D90A] = 0x11; // This can be just about anything, default is 0x41, sfx lasts for 0x20

			// All kinds of palette cycling
			Data[0x7D955] = 0x00; // This value is ANDed with a counter, 0x03 is default, 0x01 is double speed, 0x00 is quadruple

			// Battle
			Data[0x31ECE] = 0x60; // Double character animation speed
			Data[0x2DFB4] = 0x04; // Quadruple run speed

			Data[0x33D4B] = 0x04; // Explosion effect count (big enemies), default 6
			Data[0x33CCD] = 0x04; // Explosion effect count (small enemies), default 8
			Data[0x33DAA] = 0x04; // Explosion effect count (mixed enemies), default 15

			// Gain multiple levels at once.  Also supresses stat increase messages as a side effect
			Put(0x2DD82, Blob.FromHex("20789f20579f48a5802907c907f008a58029f0690785806820019c4ce89b"));

			// Default Response Rate 8 (0-based)
			Data[0x384CB] = 0x07; // Initialize respondrate to 7
			Put(0x3A153, Blob.FromHex("4CF0BF")); // Replace reset respond rate with a JMP to...
			Put(0x3BFF0, Blob.FromHex("A90785FA60")); // Set respondrate to 7

			// Move NPCs out of the way.
			MoveNpc(0, 0, 0x11, 0x02, inRoom: false, stationary: true); // North Coneria Soldier
			MoveNpc(0, 4, 0x12, 0x14, inRoom: false, stationary: true); // South Coneria Gal
			MoveNpc(0, 7, 0x1E, 0x0B, inRoom: false, stationary: true); // East Coneria Guy
			MoveNpc(2, 0, 0x27, 0x18, inRoom: false, stationary: true); // Efland Entrance Elf
			MoveNpc(6, 13, 0x29, 0x1B, inRoom: false, stationary: true); // Onrac Guy
			MoveNpc(18, 1, 0x0C, 0x34, inRoom: false, stationary: false); // OoB Bat!
			MoveNpc(30, 10, 0x09, 0x0B, inRoom: true, stationary: false); // Earth Cave Bat B3
			MoveNpc(30, 7, 0x0B, 0x0B, inRoom: true, stationary: false); // Earth Cave Bat B3
			MoveNpc(30, 8, 0x0A, 0x0C, inRoom: true, stationary: false); // Earth Cave Bat B3
			MoveNpc(30, 9, 0x09, 0x25, inRoom: false, stationary: false); // Earth Cave Bat B3
			MoveNpc(32, 1, 0x22, 0x34, inRoom: false, stationary: false); // Earth Cave Bat B5
		}

		private void MoveNpc(int map, int npc, int x, int y, bool inRoom, bool stationary)
		{
			int offset = MapSpriteOffset + (map * MapSpriteCount + npc) * MapSpriteSize;

			byte firstByte = (byte)x;
			firstByte |= (byte)(inRoom ? 0x80 : 0x00);
			firstByte |= (byte)(stationary ? 0x40 : 0x00);

			Data[offset + 1] = firstByte;
			Data[offset + 2] = (byte)y;
		}

		public void EnableIdentifyTreasures()
		{
			Put(0x2B192, Blob.FromHex("C1010200000000"));
		}

		public void EnableDash()
		{
			Put(0x7D077, Blob.FromHex("4A252DD002A54224205002A9044A6900853460"));
		}

		public void EnableBuyTen()
		{
			Put(0x380FF, Blob.FromHex("100001110001120001130000"));
			Put(0x38248, Blob.FromHex("8BB8BC018BB8BCFF8180018EBB5B00"));
			Put(0x3A8E4, Blob.FromHex("A903203BAAA9122026AAA90485634C07A9A903203BAAA91F2026AAA90385634C07A9EA"));
			Put(0x3A32C, Blob.FromHex("71A471A4"));
			Put(0x3A45A, Blob.FromHex("2066A420EADD20EFA74CB9A3A202BD0D039510CA10F860A909D002A925205BAAA5664A900920B1A8B0EC0662900520F5A8B0E3A562F0054A90DCA909856AA90D205BAA2057A8B0D82076AAA66AF017861318A200A003BD0D0375109D0D03E888D0F4C613D0EB2068AA20C2A8B0ADA562D0A9208CAA9005A9104C77A4AE0C03BD206038656AC9649005A90C4C77A49D206020F3A4A9134C77A4A200A00338BD1C60FD0D039D1C60E888D0F34CEFA7"));
			Put(0x3AA65, Blob.FromHex("2076AAA90E205BAA2066A4208E8E4C32AAA662BD00038D0C0320B9ECA202B5109D0D03CA10F860A202BD0D03DD1C60D004CA10F51860"));
			Put(0x3A390, Blob.FromHex("208CAA"));
			Put(0x3A3E0, Blob.FromHex("208CAA"));
			Put(0x3A39D, Blob.FromHex("20F3A4"));
			Put(0x3A404, Blob.FromHex("20F3A4"));
			Put(0x3AACB, Blob.FromHex("18A202B5106A95109D0D03CA10F5"));
		}

		private void EnableEasyMode()
		{
			var newRng = Get(RngOffset, RngSize).ToBytes()
				.Select(x => (byte)Math.Min(240, x * 5))
				.ToArray();
			Put(RngOffset, newRng);
			var enemies = Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);
			foreach (var enemy in enemies)
			{
				var hp = BitConverter.ToUInt16(enemy, 4);
				hp = (ushort)(hp * 0.1);
				var hpBytes = BitConverter.GetBytes(hp);
				Array.Copy(hpBytes, 0, enemy, 4, 2);
			}

			Put(EnemyOffset, enemies.SelectMany(enemy => enemy.ToBytes()).ToArray());
		}

		public void EasterEggs()
		{
			Put(0x2ADDE, Blob.FromHex("91251A682CC18EB1B74DB32505C1BE9296991E2F1AB6A4A9A8BE05C1C1C1C1C1C19B929900"));
		}

		public void EnableEarlyBridge()
		{
			// Pass all bridge_vis checks. It's a mother beautiful bridge - and it's gonna be there.
			Blob setBridgeVis = Blob.FromHex("A901EA");
			Put(0x392A1, setBridgeVis);
			Put(0x394D7, setBridgeVis);
			Put(0x7C64D, setBridgeVis);
			Put(0x7E3A6, setBridgeVis);
		}

		public void ChangeUnrunnableRunToWait()
		{
			// See Unrunnable.asm
			// Replace DrawCommandMenu with a cross page jump to a replacement that swaps RUN for WAIT if the battle is unrunnable.
			// The last 5 bytes here are the null terminated WAIT string (stashed in some leftover space of the original subroutine)
			Put(0x7F700, Blob.FromHex("ADFC6048A90F2003FE204087682003FE4C48F6A08A929D00"));

			// Replace some useless code with a special handler for unrunnables that prints a different message.
			// We then update the unrunnable branch to point here instead of the generic Can't Run handler
			// See Disch's comments here: Battle_PlayerTryRun  [$A3D8 :: 0x323E8]
			Put(0x32409, Blob.FromHex("189005A9064C07AAEAEAEAEAEAEAEA"));
			Data[0x323EB] = 0x20; // new delta to special unrunnable message handler

			// The above code uses battle message $06 which is the unused Sight Recovered string
			// Let's overwrite that string with something more appropriate for the WAIT command
			Put(0x2CC71, FF1Text.TextToBytes("Waiting", false));
		}
	}
}
