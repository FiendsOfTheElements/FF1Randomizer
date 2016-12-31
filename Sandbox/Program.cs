using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FF1Randomizer;
using RomUtilities;

namespace Sandbox
{
	class Program
	{
		static void Main(string[] args)
		{
			var bits = new BitArray(40);
			bits[39] = true;
			var bytes = new byte[5];
			bits.CopyTo(bytes, 0);

			Console.WriteLine("{0:X2}-{1:X2}-{2:X2}-{3:X2}-{4:X2}", bytes[0], bytes[1], bytes[2], bytes[3], bytes[4]);

			Console.ReadLine();
		}
	}
}
