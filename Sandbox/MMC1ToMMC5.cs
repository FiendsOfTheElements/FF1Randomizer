using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FF1Lib;
using RomUtilities;
// ReSharper disable InconsistentNaming

namespace Sandbox
{
    public class FF1MMC5Rom : FF1Rom
    {
	    public FF1MMC5Rom(string filename) : base(filename)
	    {}

		public void UpgradeToMMC5()
		{
			// See https://www.romhacking.net/forum/index.php?topic=24989.0

			Header[4] = 32; // 32 pages of 16 kB
			Header[6] = 0x53; // original is 0x13, where 1 = MMC1 and 5 = MMC5

			Blob newData = new byte[0x80000];
			Array.Copy(Data, newData, 0x3C000);
			Array.Copy(Data, 0x3C000, newData, 0x7C000, 0x4000);
			Data = newData;

			// Initialize MMC5
			Put(0x7FE48, Blob.FromHex("a9008d15508d10508d04528d30518d01518d27518d0052a9ff8d1751a9018d00518d03510a8d02518d04510a8d1351a9448d0551a900"));

			// Change bank swap code
			Put(0x7FE1A, Blob.FromHex("0a09808d1551a90060"));
		}
    }
}
