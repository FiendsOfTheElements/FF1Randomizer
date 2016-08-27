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
			var rom = new FF1Rom("E:/temp/FF1Randomizer/FF1.nes");

			var rng = MT19337.New();
			for (int i = 0; i < 100; i++)
			{
				rom.ShuffleTreasures(rng);
				var treasures = rom.GetTreasures();
				Console.WriteLine($"{treasures.IndexOf((byte)QuestItems.Floater):000} {treasures.IndexOf((byte)QuestItems.Ruby):000} {treasures.IndexOf((byte)QuestItems.Slab):000} ");
			}

			Console.ReadLine();
		}
	}

	static class RomExtensions
	{
		public static Blob GetTreasures(this FF1Rom rom)
		{
			return rom.Get(FF1Rom.TreasureOffset, FF1Rom.TreasureSize*FF1Rom.TreasureCount);
		}
	}
}
