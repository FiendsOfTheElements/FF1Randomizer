using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FF1Lib;
using RomUtilities;
// ReSharper disable InconsistentNaming

namespace Sandbox
{
    public class FF1MapperRom : FF1Rom
    {
	    public FF1MapperRom(string filename) : base(filename)
	    {}

		public void UpgradeToMMC5()
		{
			// See https://www.romhacking.net/forum/index.php?topic=24989.0

			Header[4] = 32; // 32 pages of 16 kB
			Header[6] = 0x53; // original is 0x13, where 1 = MMC1 and 5 = MMC5

			// Expand ROM size, moving bank 0F to the end
			Blob newData = new byte[0x80000];
			Array.Copy(Data, newData, 0x3C000);
			Array.Copy(Data, 0x3C000, newData, 0x7C000, 0x4000);
			Data = newData;

			// Initialize MMC5
			Put(0x7FE48, Blob.FromHex("a9008d15508d10508d04528d30518d01518d27518d0052a9ff8d1751a9018d00518d03510a8d02518d04510a8d1351a9448d0551a900"));

			// Change bank swap code
			Put(0x7FE1A, Blob.FromHex("0a09808d1551a90060"));
		}

		public new void UpgradeToMMC3()
		{
			Header[4] = 32; // 32 pages of 16 kB
			Header[6] = 0x43; // original is 0x13 where 1 = MMC1 and 4 = MMC3

			// Expand ROM size, moving bank 0F to the end.
			Blob newData = new byte[0x80000];
			Array.Copy(Data, newData, 0x3C000);
			Array.Copy(Data, 0x3C000, newData, 0x7C000, 0x4000);
			Data = newData;

			// Change bank swap code.
			// We put this code at SwapPRG_L, so we don't have to move any of the "long" calls to it.
			// We completely overwrite SetMMC1SwapMode, since we don't need it anymore, and partially overwrite the original SwapPRG.
			Put(0x7FE03, Blob.FromHex("48a9068d0080680a8d018048a9078d00806869018d0180a90060"));

			// Initialize MMC3
			Put(0x7FE48, Blob.FromHex("8d00e0a9808d01a0a0008c00a08c00808c0180c88c0080c88c01808c0080c8c88c0180a9038d0080c88c0180a9048d00804c1dfea900"));
			Put(0x7FE1D, Blob.FromHex("c88c0180a9058d0080c88c01804c7cfe"));

			// Rewrite the lone place where SwapPRG was called directly and not through SwapPRG_L.
			Data[0x7FE97] = 0x03;
		}
	}
}
