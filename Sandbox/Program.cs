using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FF1Randomizer;

namespace Sandbox
{
	class Program
	{
		static void Main(string[] args)
		{
			var rom = new FF1Rom("E:/temp/FF1Randomizer/FF1.nes");

			var shopPointerBlob = rom.Get(0x38300, 142);
			var shopPointers = Enumerable.Range(0, 71).Select(i => shopPointerBlob.SubBlob(2*i, 2)).ToList();

			var shopints = shopPointers.Select(p => BitConverter.ToUInt16(p, 0)).OrderBy(i => i).ToList();

			shopints.ForEach(i => Console.WriteLine(i));

			Console.ReadLine();
		}
	}
}
