using System.IO;
using System.Security.Cryptography;
using FF1Lib;

namespace Sandbox
{
    class Program
    {
	    private static FF1Rom rom;
	    private static RandomNumberGenerator csharpRNG;

        static void Main(string[] args)
        {
			var filename = "ff1.nes";
			var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			var rom = new FF1Rom(fs);


			rom = new FF1Rom(fs);
			csharpRNG = RNGCryptoServiceProvider.Create();

	        // asm
	        //rom.UpgradeToMMC3();
	        //rom.DemoPatchAssembler();

			// mapgen
	        //TestMapGen.Run(rom, csharpRNG);
        }
	}
}
