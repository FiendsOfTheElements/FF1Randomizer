using System;
using FF1Lib;
using RomUtilities;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
	        Console.WriteLine(FF1Text.BytesToText(Blob.FromHex("aba4b3b3a8b1b6")));
			Console.ReadKey();
        }
    }
}
