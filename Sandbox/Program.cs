using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FF1Lib;
using RomUtilities;

namespace Sandbox
{
	class Program
	{
		static void Main(string[] args)
		{
			var test = new BitArray(Blob.FromHex("FF"));

			Console.WriteLine(test);

			Console.ReadLine();
		}
	}
}
