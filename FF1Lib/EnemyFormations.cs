using System.Collections.Generic;
using System.Linq;
using RomUtilities;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public enum FinalFormation
		{
			WarMECHsAndFriends,
			KaryAndTiamat,
			TheFundead,
			TimeLoop,
		};

		private const int FormationsOffset = 0x2C400;
		private const int FormationSize = 16;

		private const int FormationCount = 128;           // Total formations
		private const int NormalFormationCount = 115;     // Number of formations before all the boss encounters.
		private const int BossFormationCount = 13;        // Lichx2, Karyx2, Krakenx2, Tiamatx2, Chaos, Vampire, Astos, Pirates, Garland
		private const int VanillaUnrunnableCount = 13;    // 13 of the Vanilla normal formations are unrunnable.
		private const int ChaosFormationIndex = 123;      // Index of Chaos battle that preceeds The End
		private const int WarMECHFormationIndex = 86;

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
			// We skip formation 1 to avoid unrunnable trolly IMPs at the start of the game.
			List<int> ids = Enumerable.Range(1, NormalFormationCount - 1).ToList();
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

		public void MakeWarMECHUnrunnable()
		{
			// This needs to be called after ShuffleUnrunnable, otherwise it will shuffle away this unrunnability.
			Blob warMECHFormation = Get(FormationsOffset + FormationSize * WarMECHFormationIndex, FormationSize);
			warMECHFormation[UnrunnableOffset] |= 0x01;
			Put(FormationsOffset + FormationSize * WarMECHFormationIndex, warMECHFormation);
		}

		public void TransformFinalFormation(FinalFormation formation)
		{
			Blob finalBattle = Get(FormationsOffset + ChaosFormationIndex * FormationSize, FormationSize);

			switch (formation)
			{
				case FinalFormation.WarMECHsAndFriends:
					finalBattle[TypeOffset] = 0x2C;         // Big/Small Enemy mix, and the Astos/Madpony/Badman/WarMECH patterns
					finalBattle[GFXOffset] = 0x03;          // WarMECH Badman N/A N/A
					finalBattle[IDsOffset + 0] = 0x76;      // WarMECH (battle stats, etc)
					finalBattle[IDsOffset + 1] = 0x70;      // EvilMan
					finalBattle[QuantityOffset + 0] = 0x22;
					finalBattle[QuantityOffset + 1] = 0x66;
					finalBattle[PalettesOffset + 0] = 0x2F;
					finalBattle[PalettesOffset + 1] = 0x17;
					finalBattle[PaletteAsignmentOffset] = 0x41; // Palette Assignment in top nibble, 1 in bottom for unrunnable.
					break;
				case FinalFormation.KaryAndTiamat:
					finalBattle[TypeOffset] = 0x2B;         // Big/Small Enemy mix, and the Dragon2 pattern
					finalBattle[GFXOffset] = 0x05;          // Dragon Dragon N/A N/A
					finalBattle[IDsOffset + 0] = 0x7A;      // Kary2
					finalBattle[IDsOffset + 1] = 0x7E;      // Tiamat2
					finalBattle[QuantityOffset + 0] = 0x11;
					finalBattle[QuantityOffset + 1] = 0x11;
					finalBattle[PalettesOffset + 0] = 0x08;
					finalBattle[PalettesOffset + 1] = 0x0A;
					finalBattle[PaletteAsignmentOffset] = 0x41; // Palette Assignment in top nibble, 1 in bottom for unrunnable.
					break;
				case FinalFormation.TheFundead:
					finalBattle[TypeOffset] = 0x24;         // Eye pattern
					finalBattle[GFXOffset] = 0x0B;          // Eye / Geist
					finalBattle[IDsOffset + 0] = 0x78;      // Lich2
					finalBattle[IDsOffset + 1] = 0x33;      // Phantom
					finalBattle[QuantityOffset + 0] = 0x22;
					finalBattle[QuantityOffset + 1] = 0x44;
					finalBattle[PalettesOffset + 0] = 0x03;
					finalBattle[PalettesOffset + 1] = 0x17;
					finalBattle[PaletteAsignmentOffset] = 0x41; // Palette Assignment in top nibble, 1 in bottom for unrunnable.

					// Scale up the Fundead enemies if we end up with them. They're too weak otherwise.
					ScaleSingleEnemyStats(0x78, 1.4, false, false, null, false);
					ScaleSingleEnemyStats(0x33, 1.2, false, false, null, false);
					break;
				case FinalFormation.TimeLoop:
					finalBattle[TypeOffset] = 0x0B;         // 9Small + Garland pattern
					finalBattle[GFXOffset] = 0x2A;          // Garland Garland Garland N/A
					finalBattle[IDsOffset + 0] = 0x69;      // Garland
					finalBattle[IDsOffset + 1] = 0x7F;      // Chaos
					finalBattle[IDsOffset + 2] = 0x69;      // Garland
					finalBattle[QuantityOffset + 0] = 0x08;
					finalBattle[QuantityOffset + 1] = 0x11;
					finalBattle[QuantityOffset + 2] = 0x88;
					finalBattle[PalettesOffset + 0] = 0x00;
					finalBattle[PalettesOffset + 1] = 0x00;
					finalBattle[PaletteAsignmentOffset] = 0x01; // Palette Assignment in top nibble, 1 in bottom for unrunnable.
					break;
			}

			Put(FormationsOffset + ChaosFormationIndex * FormationSize, finalBattle);
		}
	}

}
