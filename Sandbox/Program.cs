using System;
using FF1Lib;
using RomUtilities;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
			var rom = new FF1MMC5Rom("ff1.nes");
			rom.UpgradeToMMC5();
			rom.Save("ff1mmc5.nes");

			Console.WriteLine("Done.");
	        Console.ReadKey();
        }
    }
}
