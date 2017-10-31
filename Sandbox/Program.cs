using System;
using FF1Lib;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
	        Console.WriteLine(FF1Text.BytesToText(new byte[] { 0x99, 0x98, 0x92, 0x9C, 0x98, 0x97 }));
	        Console.ReadKey();
        }
    }
}
