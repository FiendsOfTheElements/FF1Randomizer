using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ROMUtilities;

namespace FF1Randomizer
{
	public class FF1Rom : NesRom
	{
		public const int TreasureOffset = 0x3100;
		public const int TreasureSize = 1;
		public const int TreasureCount = 256;

		public FF1Rom(string filename) : base(filename)
		{}

		public override bool Validate()
		{
			return Get(0, 16) == Blob.FromHex("06400e890e890e401e400e400e400b42");
		}
		
		public void ShuffleTreasures(MT19337 rng)
		{
			var treasureBlob = Get(TreasureOffset, TreasureSize*TreasureCount);
			var treasures = treasureBlob.ToBytes().ToList();

			treasures.Shuffle(rng);

			// Need to insert sanity checks here

			treasureBlob = treasures.ToArray();
			Put(TreasureOffset, treasureBlob);
		}
	}
}
