using System;
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
			var bytes = FF1Text.TextToBytes("Seed    01234567");
			var text = FF1Text.BytesToText(bytes);

			Console.WriteLine(text);

			Console.ReadLine();
		}
	}
}
