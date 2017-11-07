using System;
using FF1Lib;
using RomUtilities;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
			BmpTo2bpp.Convert("icon.bmp", "icon.chr");
	        Console.ReadKey();
        }
    }
}
