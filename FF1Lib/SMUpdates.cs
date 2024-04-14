using FF1Lib.Procgen;
using RomUtilities;
using System.IO.Compression;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public partial class StandardMaps
	{
		public void Update(MT19337 rng)
		{
			if ((bool)flags.ProcgenEarth)
			{
				LoadPregenDungeon(rng, teleporters, "earthcaves.zip");

				// Here's the code to generate from scratch, but it takes too long in the browser.
				// So we get one from the pregen pack above.
				//
				// var newmaps = await NewDungeon.GenerateNewDungeon(rng, this, MapIndex.EarthCaveB1, maps, npcdata, this.Progress);
				// foreach (var newmap in newmaps) {
				//   this.ImportCustomMap(maps, teleporters, overworldMap, npcdata, newmap);
				//  }
			}

			MoveToFBats((bool)flags.MoveToFBats);
			FlipMaps(rng);

		}

		private void MoveToFBats(bool movebats)
		{
			if (!movebats)
			{
				return;
			}
			mapObjects[(int)MapIndex.TempleOfFiends].MoveNpc(2, 0x0C, 0x0D, false, false);
			mapObjects[(int)MapIndex.TempleOfFiends].MoveNpc(3, 0x1D, 0x0B, false, false);
			mapObjects[(int)MapIndex.TempleOfFiends].MoveNpc(4, 0x1A, 0x19, false, false);
			mapObjects[(int)MapIndex.TempleOfFiends].MoveNpc(5, 0x0F, 0x18, false, false);
			mapObjects[(int)MapIndex.TempleOfFiends].MoveNpc(6, 0x14, 0x0C, false, false);
		}
		public void FlipMaps(MT19337 rng)
		{
			var mapFlipper = new FlippedMaps(rom, maps, flags, teleporters, rng);
			VerticalFlippedMaps = mapFlipper.VerticalFlipStep1();

			if ((bool)flags.ReversedFloors) new ReversedFloors(rom, maps, rng, teleporters, VerticalFlippedMaps).Work();

			mapFlipper.VerticalFlipStep2();

			if ((bool)flags.FlipDungeons)
			{
				HorizontalFlippedMaps = mapFlipper.HorizontalFlip(rng, maps, teleporters);
			}
		}
	}
}
