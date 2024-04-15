using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public enum TileSets
	{
		Town = 0,
		Castle,
		EarthTitanVolcano,
		MatoyaDwarfCardiaIceWaterfall,
		MarshMirage,
		ToFSeaShrine,
		SkyCastle,
		ToFR
	}
	public partial class TileSetsData
	{
		public void Update(Flags flags, MT19337 rng)
		{
			EnableEarlyOrdeals((bool)flags.EarlyOrdeals);

		}
		private void EnableEarlyOrdeals(bool earlyordeals)
		{
			if (!earlyordeals)
			{
				return;
			}

			// The 4 masked-out bits are special flags for a tile.  We wipe the flags for the two throne teleportation tiles,
			// which normally indicate that the CROWN is required to use them.
			const byte specialMask = 0b1110_0001;
			tileSets[(int)TileSets.Castle].Tiles[0x61].PropertyType &= specialMask;
		}




	}
}
