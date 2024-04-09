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
		public const string BattleBoxDrawInFrames = "06"; // Half normal (Must divide 12)
		public const string BattleBoxDrawInRows = "02";

		public const string BattleBoxUndrawFrames = "04"; // 2/3 normal (Must  divide 12)
		public const string BattleBoxUndrawRows = "03";
		public void EnableSpeedHacks(Preferences preferences)
		{
			if (!preferences.OptOutSpeedHackWipes)
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
			}

			// Battle
			Data[0x31ECE] = 0x60; // Double character animation speed
			Data[0x2DFB4] = 0x04; // Quadruple run speed

			Data[0x33D4B] = 0x04; // Explosion effect count (big enemies), default 6
			Data[0x33CCD] = 0x04; // Explosion effect count (small enemies), default 8
			Data[0x33DAA] = 0x04; // Explosion effect count (mixed enemies), default 15

			var drawinframes = preferences.OptOutSpeedHackMessages ? "0C" : BattleBoxDrawInFrames;
			var undrawframes = preferences.OptOutSpeedHackMessages ? "06" : BattleBoxUndrawFrames;

			// Draw and Undraw Battle Boxes faster
			Put(0x2DA12, Blob.FromHex("3020181008040201")); // More practical Respond Rates
			Put(0x7F4AA, Blob.FromHex($"ADFC6048A90F2003FE20008AA9{drawinframes}8517A90F2003FE20208A2085F4C617D0F1682003FE60"));
			Put(0x7F4FF, Blob.FromHex($"ADFC6048A90F2003FE20808AA9{undrawframes}8517A90F2003FE20A08A2085F4C617D0F1682003FE60"));

			// Gain multiple levels at once.
			//Put(0x2DD82, Blob.FromHex("20789f20579f48a5802907c907f008a58029f0690785806820019c4ce89b")); old
			PutInBank(0x1B, 0x8850, Blob.FromHex("4C4888"));
			// Skip stat up messages
			PutInBank(0x1B, 0x89F0, Blob.FromHex("4CE38B"));

			var responserate = preferences.OptOutSpeedHackMessages ? (byte)0x04 : (byte)0x07;
			if (preferences.LockRespondRate)
			{
				responserate = (byte)(preferences.RespondRate - 1);
			}

			// Default Response Rate 8 (0-based)
			Data[0x384CB] = responserate; // Initialize respondrate to 7
			Put(0x3A153, Blob.FromHex("4CF0BF")); // Replace reset respond rate with a JMP to...
			Put(0x3BFF0, Blob.FromHex($"A9{responserate.ToString("00")}85FA60")); // Set respondrate to 7

			// Faster Lineup Modifications
			var animationOffsets = new List<int> { 0x39AA0, 0x39AB4, 0x39B10, 0x39B17, 0x39B20, 0x39B27 };
			animationOffsets.ForEach(addr => Data[addr] = 0x04);
		}
		// Move this one to npc class
		public void SpeedHacksMoveNpcs(bool moveEarthBats)
		{
			// Move NPCs out of the way.
			MoveNpc(MapIndex.ConeriaTown, 0, 0x11, 0x02, inRoom: false, stationary: true); // North Coneria Soldier
			MoveNpc(MapIndex.ConeriaTown, 4, 0x12, 0x14, inRoom: false, stationary: true); // South Coneria Gal
			MoveNpc(MapIndex.ConeriaTown, 7, 0x1E, 0x0B, inRoom: false, stationary: true); // East Coneria Guy
			MoveNpc(MapIndex.Elfland, 0, 0x24, 0x19, inRoom: false, stationary: true); // Efland Entrance Elf
			MoveNpc(MapIndex.Onrac, 13, 0x29, 0x1B, inRoom: false, stationary: true); // Onrac Guy
			MoveNpc(MapIndex.Lefein, 3, 0x21, 0x07, inRoom: false, stationary: true); // Lefein Guy
																				   //MoveNpc(MapIndex.Waterfall, 1, 0x0C, 0x34, inRoom: false, stationary: false); // OoB Bat!
			if (moveEarthBats)
			{
				MoveNpc(MapIndex.EarthCaveB3, 10, 0x32, 0x0C, inRoom: false, stationary: false); // Earth Cave Bat B3
				MoveNpc(MapIndex.EarthCaveB3, 7, 0x31, 0x1A, inRoom: true, stationary: false); // Earth Cave Bat B3
				MoveNpc(MapIndex.EarthCaveB3, 8, 0x1D, 0x0E, inRoom: true, stationary: false); // Earth Cave Bat B3

				MoveNpc(MapIndex.EarthCaveB3, 2, 0x0B, 0x0A, inRoom: true, stationary: false); // Earth Cave Bat B3
				MoveNpc(MapIndex.EarthCaveB3, 3, 0x0A, 0x0B, inRoom: true, stationary: false); // Earth Cave Bat B3
				MoveNpc(MapIndex.EarthCaveB3, 4, 0x09, 0x0A, inRoom: true, stationary: false); // Earth Cave Bat B3

				MoveNpc(MapIndex.EarthCaveB3, 9, 0x09, 0x25, inRoom: false, stationary: false); // Earth Cave Bat B3

				MoveNpc(MapIndex.EarthCaveB5, 1, 0x22, 0x34, inRoom: false, stationary: false); // Earth Cave Bat B5
			}

			MoveNpc(MapIndex.ConeriaCastle1F, 5, 0x07, 0x0F, inRoom: false, stationary: true); // Coneria Ghost Lady

			MoveNpc(MapIndex.Pravoka, 4, 0x1F, 0x05, inRoom: false, stationary: true); // Pravoka Old Man
			MoveNpc(MapIndex.Pravoka, 5, 0x08, 0x0E, inRoom: false, stationary: true); // Pravoka Woman
		}

		public void EnableDash(bool speedboat, bool slowMapMove)
		{
			if (slowMapMove)
			{
				//same as dash, but BVS instead of BVC
				PutInBank(0x1F, 0xD077, Blob.FromHex("A5424A69004A69000A242070014A853460"));
			}
			else if (speedboat)
			{
				// walking, canoe are speed 2
				// ship, airship are speed 4
				//
				// See asm/1F_D077_Speedboat.asm
				//
				PutInBank(0x1F, 0xD077, Blob.FromHex(
						  "A5424AB0014AA902B0010A242050014A853460"));
			}
			else
			{
				// walking, canoe, and boat are speed 2
				// airship is speed 4
				//
				// disassembly
				//
				// D077   A5 42      LDA $42     ; load vehicle
				// D079   4A         LSR A       ; shift right, if on foot (A=$01), this sets Zero, oVerflow and Carry, and set A=$00
				// D07A   69 00      ADC #$00    ; add zero, if Carry is set, this sets A=$01 and clears Z, V, and C
				// D07C   4A         LSR A       ; shift right again, if on foot (A=$01), this sets Zero and Carry, and sets A=$00
				// D07D   69 00      ADC #$00    ; add zero, if Carry is set, this sets A=$01 and clears Z, V, and C
				// D07F   0A         ASL A       ; shift left, this turns A=$01 to A=$02
				// D080   24 20      BIT $20     ; check joystick state: set Z if bit 5 is not set, V if bit 6 is set, N if bit 7 is set
				// D082   50 01      BVC $D085   ; branch if V is clear (I guess that means bit 6 is B button)
				// D084   4A         LSR A       ; V was set, which means B was pressed, so shift right (this cuts the speed in half)
				// D085   85 34      STA $34     ; store movement speed; walking/canoe/ship is 2 and airship is 4
				// D087   60         RTS         ; return
				// D088   34                     ; leftover garbage
				// D089   60                     ; leftover garbage

				PutInBank(0x1F, 0xD077, Blob.FromHex("A5424A69004A69000A242050014A853460"));
			}
		}

	}
}
