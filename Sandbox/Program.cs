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

			rom = new FF1Rom(fs);
			csharpRNG = RNGCryptoServiceProvider.Create();
	        TestMapGen.Run(rom, csharpRNG);
        }
	}
}
