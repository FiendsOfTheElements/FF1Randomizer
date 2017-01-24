using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomUtilities;

namespace FF1Randomizer
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

		public void SpeedHacks()
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
		}
	}
}
