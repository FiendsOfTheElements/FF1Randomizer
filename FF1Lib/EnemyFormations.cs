using System.Collections.Generic;
using System.Linq;
using RomUtilities;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		private const int FormationsOffset = 0x2C400;
		private const int FormationSize = 16;

		private const int FormationCount = 128;            // Total formations
		private const int NormalFormationCount = 115;      // Number of formations before all the boss encounters.
		private const int BossFormationCount = 13;         // Lichx2, Karyx2, Krakenx2, Tiamatx2, Chaos, Vampire, Astos, Pirates, Garland
		private const int VanillaUnrunnableCount = 13;     // 13 of the Vanilla normal formations are unrunnable.

		// Formation Data Offsets ripped straight from Disch's variables.inc
		private const byte TypeOffset = 0x00;             // battle type (high 4 bits)
		private const byte PatternTableOffset = 0x00;     // pattern table (low 4 bits)
		private const byte GFXOffset = 0x01;              // graphic assignment (2 bits per enemy)
		private const byte IDsOffset = 0x02;              // enemy IDs (4 bytes)
		private const byte QuantityOffset = 0x06;         // enemy quantities(4 bytes)
		private const byte PalettesOffset = 0x0A;         // palettes for this battle (2 bytes)
		private const byte SurpriseOffset = 0x0C;         // surprise rate
		private const byte PaletteAsignmentOffset = 0x0D; // enemy palette assign (in high 4 bits)
		private const byte UnrunnableOffset = 0x0D;       // no run flag (in low bit)
		private const byte QuantityBOffset = 0x0E;        // enemy quantities for B formation (2 bytes)

		public void ShuffleUnrunnable(MT19337 rng)
		{
			// First indiscriminately mark all normal formations as runnable.
			List<Blob> formations = Get(FormationsOffset, FormationSize * NormalFormationCount).Chunk(FormationSize);
			formations.ForEach(formation => formation[UnrunnableOffset] &= 0xFE);

			// Generate a shuffled list of ids in the normal formation range and update formations with that.
			List<int> ids = Enumerable.Range(0, NormalFormationCount).ToList();
			ids.Shuffle(rng);
			ids = ids.Take(VanillaUnrunnableCount).ToList();
			ids.ForEach(id => formations[id][UnrunnableOffset] |= 0x01);

			Put(FormationsOffset, formations.SelectMany(formation => formation.ToBytes()).ToArray());
		}

		public void ShuffleSurpriseBonus(MT19337 rng)
		{
			// Just like the vanilla game this doesn't care if a high surprise enemy is unrunnable
			// and therefore incapable of surprise or first strike. It just shuffles indiscriminately.
			List<Blob> formations = Get(FormationsOffset, FormationSize * NormalFormationCount).Chunk(FormationSize);
			List<byte> chances = formations.Select(formation => formation[SurpriseOffset]).ToList();
			chances.Shuffle(rng);

			formations = formations.Zip(chances, (formation, chance) => { formation[SurpriseOffset] = chance; return formation; }).ToList();
			Put(FormationsOffset, formations.SelectMany(formation => formation.ToBytes()).ToArray());
		}
	}

}
