using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomUtilities;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
        public const int Nop = 0xEA;
        public const int KingRoutineStartOffset = 0x39297;
        public const int SardaObjectVisibilityCheck = 0x393EA; // Normally checks vampire is visible, but could check any other object ID instead at this location
		public const int SardaOffset = 0x393E9;
        public const int SardaSize = 7;
        public const int CanoeMovementCheck = 0x3C5ED;
        public const int CanoeSageGiveCanoe = 0x39488;
		public const int CanoeSageOffset = 0x39482;
		public const int CanoeSageSize = 5;
		public const int PartyShuffleOffset = 0x312E0;
		public const int PartyShuffleSize = 3;
		public const int MapSpriteOffset = 0x3400;
		public const int MapSpriteSize = 3;
		public const int MapSpriteCount = 16;

        public const int CaravanFairyCheck = 0x3C4E5;
        public const int NerrickOffset = 0x39297 + 198; // Nerrick can give an item in exchange for Tnt instead of the canal
        //public const int BridgeSceneCheck = 0x3C2B7; // putting new byte[]{0xA9, 0x01, Nop} here will also skip the bridge scene, without the king building early

        // Required for npc quest item randomizing
        public void PermanentCaravan()
        {
            Put(CaravanFairyCheck, new byte[] { Nop, Nop, Nop, Nop, Nop, Nop, Nop });
        }
        // Required for npc quest item randomizing so that the canoe can be in a chest
        public void CheckCanoeItemInsteadOfEventVar()
        {
            Put(CanoeMovementCheck, new byte[] { Items.Canoe + Variables.ItemsBaseForNPC });
            Put(CanoeSageGiveCanoe, new byte[] { Items.Canoe + Variables.ItemsBaseForNPC });
        }

        // Required for npc quest item randomizing
        private void MirrorNPCItemChecksFromTargetItem()
        {
            const int TalkRoutineBase = 0x39297;
            Put(TalkRoutineBase + 11, Get(TalkRoutineBase + 21, 1)); // King
            Put(TalkRoutineBase + 76, Get(TalkRoutineBase + 81, 1)); // Bikke
            Put(TalkRoutineBase + 117, Get(TalkRoutineBase + 125, 1)); // Elf Doc Take
            Put(TalkRoutineBase + 146, Get(TalkRoutineBase + 154, 1)); // Prince
            Put(TalkRoutineBase + 188, Get(TalkRoutineBase + 196, 1)); // Nerrick Take
            Put(TalkRoutineBase + 224, Get(TalkRoutineBase + 247, 1)); // Smith Take
            Put(TalkRoutineBase + 258, Get(TalkRoutineBase + 281, 1)); // Matoya
            Put(TalkRoutineBase + 266, Get(TalkRoutineBase + 284, 1)); // Matoya Take
            Put(TalkRoutineBase + 302, Get(TalkRoutineBase + 310, 1)); // Unne Take
            Put(TalkRoutineBase + 334, Get(TalkRoutineBase + 346, 1)); // Sarda
            Put(TalkRoutineBase + 367, Get(TalkRoutineBase + 375, 1)); // Bahamut Take
            Put(TalkRoutineBase + 418, Get(TalkRoutineBase + 426, 1)); // Robot
            Put(TalkRoutineBase + 434, Get(TalkRoutineBase + 442, 1)); // Princess
            Put(TalkRoutineBase + 450, Get(TalkRoutineBase + 458, 1)); // Fairy
            Put(TalkRoutineBase + 466, Get(TalkRoutineBase + 474, 1)); // Titan Take
            Put(TalkRoutineBase + 487, Get(TalkRoutineBase + 497, 1)); // Canoe Sage
            Put(TalkRoutineBase + 776, Get(TalkRoutineBase + 784, 1)); // Lefein
        }
        // Required for npc quest item randomizing
        public void NormalizeNerrick()
        {
            Put(NerrickOffset, new byte[] { Nop, Nop, 238 }); // Normally Nerrick sets a value to 0 instead we increment like the others
        }
        // Required for npc quest item randomizing along with NormalizeNerrick
        public void FixCanal(IReadOnlyCollection<ItemLocation> newItemLocations)
        {
            var canalVisibilityAddress = newItemLocations.Single(x => x.UpdatesVariable && Get(x.Address, 1).ToBytes()[0] == Variables.CanalVis).Address;
            Put(canalVisibilityAddress - 1, new byte[] { 206 }); // Wherever we placed the canal, make sure to decrement the value instead of increment
            // And, flip the check for already having given the item BNE = 208, BEQ = 240
            var normals = new[] { 
                ItemLocations.KingConeria.Address, ItemLocations.ElfPrince.Address, ItemLocations.CubeBot.Address,
                ItemLocations.Princess.Address, ItemLocations.Fairy.Address, ItemLocations.Lefein.Address
            };
            if (normals.Contains(canalVisibilityAddress)) {
                Put(canalVisibilityAddress - 6, new byte[] { 208 });
                return;
            }
            if (canalVisibilityAddress == ItemLocations.Matoya.Address)
                Put(canalVisibilityAddress - 21, new byte[] { 208 });
            if (canalVisibilityAddress == ItemLocations.CanoeSage.Address)
                Put(canalVisibilityAddress - 8, new byte[] { 240 });
            if (canalVisibilityAddress == ItemLocations.Bikke.Address)
                Put(canalVisibilityAddress - 3, new byte[] { 240 });
            if (canalVisibilityAddress == ItemLocations.Sarda.Address)
                Put(canalVisibilityAddress - 10, new byte[] { 240 });
        }

        // Not strictly required for npc quest item randomizing, but makes adamant much less sucky
        public void AdamantMasamune()
        {
            Put(ItemLocations.SmithWeapon.Address, new byte[] { 40 });
        }
        // Not required for npc quest item randomizing, but it's nice to have
        // The king will no longer require the princess rescued to build the bridge (or give an item)
        // And, more importantly the cutscene on bridge will be skipped
        public void FastBridge()
        {
            Put(KingRoutineStartOffset + 5, new byte[] { 0xA9, 0x80, 0x8D, Variables.BridgeScene }); // 169, 128, 141
        }

		public void EnableEarlyRod()
		{
			var nops = new byte[SardaSize];
			for (int i = 0; i < nops.Length; i++)
			{
				nops[i] = Nop;
			}

			Put(SardaOffset, nops);
            // Alternatively, this will check if the titan was fed instead of the vampire dead (which makes more sense)
            //Put(SardaObjectVisibilityCheck, new byte[] { ObjectIds.Titan });
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
			Data[0x3D6EE] = 0x08; // These two values must evenly divide 224 (0xE0), default are 2 and 4
			Data[0x3D6F5] = 0x10;
			Data[0x3D713] = 0x0A; // These two values must evenly divide 220 (0xDC), default are 2 and 4
			Data[0x3D71A] = 0x14; // Don't ask me why they aren't the same, it's the number of scanlines to stop the loop at

			// Dialogue boxes
			Data[0x3D620] = 0x0B; // These two values must evenly divide 88 (0x58), the size of the dialogue box
			Data[0x3D699] = 0x0B;

			// Battle entry
			Data[0x3D90A] = 0x11; // This can be just about anything, default is 0x41, sfx lasts for 0x20

			// All kinds of palette cycling
			Data[0x3D955] = 0x00; // This value is ANDed with a counter, 0x03 is default, 0x01 is double speed, 0x00 is quadruple

			// Battle
			Data[0x31ECE] = 0x60; // Double character animation speed
			Data[0x2DFB4] = 0x04; // Quadruple run speed

			Data[0x33D4B] = 0x04; // Explosion effect count (big enemies), default 6
			Data[0x33CCD] = 0x04; // Explosion effect count (small enemies), default 8
			Data[0x33DAA] = 0x04; // Explosion effect count (mixed enemies), default 15

			// Default Response Rate 8 (0-based)
			Data[0x384CB] = 0x07; // Initialize respondrate to 7
			Put(0x3A153, Blob.FromHex("4CF0BF")); // Replace reset respond rate with a JMP to...
			Put(0x3BFF0, Blob.FromHex("A90785FA60")); // Set respondrate to 7

			// Move NPCs out of the way.
			MoveNpc( 0,  0, 0x11, 0x02, inRoom: false, stationary:  true); // North Coneria Soldier
			MoveNpc( 0,  4, 0x12, 0x14, inRoom: false, stationary:  true); // South Coneria Gal
			MoveNpc( 0,  7, 0x1E, 0x0B, inRoom: false, stationary:  true); // East Coneria Guy
			MoveNpc( 6, 13, 0x29, 0x1B, inRoom: false, stationary:  true); // Onrac Guy
			MoveNpc(18,  1, 0x0C, 0x34, inRoom: false, stationary: false); // OoB Bat!
			MoveNpc(30, 10, 0x09, 0x0B, inRoom:  true, stationary: false); // Earth Cave Bat B3
			MoveNpc(30,  7, 0x0B, 0x0B, inRoom: false, stationary: false); // Earth Cave Bat B3
			MoveNpc(30,  8, 0x0A, 0x0C, inRoom: false, stationary: false); // Earth Cave Bat B3
			MoveNpc(30,  9, 0x09, 0x25, inRoom: false, stationary: false); // Earth Cave Bat B3
			MoveNpc(32,  1, 0x22, 0x34, inRoom: false, stationary: false); // Earth Cave Bat B5
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
			Put(0x03D077, Blob.FromHex("4A252DD002A54224205002A9044A6900853460"));
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

		public void EasterEggs()
		{
			Put(0x2ADDE, Blob.FromHex("91251A682CC18EB1B74DB32505C1BE9296991E2F1AB6A4A9A8BE05C1C1C1C1C1C19B929900"));
		}
	}
}
