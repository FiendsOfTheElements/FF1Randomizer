using System;
using FF1Lib;
using RomUtilities;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
			var rom = new FF1MapperRom("ff1.nes");
			rom.UpgradeToMMC3();
			rom.Save("ff1mmc3.nes");

			Console.WriteLine("Done.");
	        Console.ReadKey();
        }
    }
}
