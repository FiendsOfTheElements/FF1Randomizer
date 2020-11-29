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
			// Load a list of formations from ROM
			List<Blob> formations = Get(FormationsOffset, FormationSize * FormationCount).Chunk(FormationSize);
			int unrunnableAcount = 0, unrunnableBcount = 0; // number of formations marked as unrunnable on both sides
			// First we mark all formations as runnable except for fiends/chaos (both sides) or the other boss fights (on a-side only)
			for (int i = 0; i < FormationCount; ++i)
			{
				if (i >= NormalFormationCount && i <= ChaosFormationIndex)
					continue; // skip fiends and Chaos on both sides
				if ((formations[i][UnrunnableOffset] & 0x01) == 0x01 && i < NormalFormationCount)
					unrunnableAcount++; // only add normal formations to the count for a-side
				if ((formations[i][UnrunnableOffset] & 0x02) == 0x02 && (i < NormalFormationCount || i > ChaosFormationIndex))
					unrunnableBcount++; // only add the b-sides that are not fiend/chaos fights
				if (i > ChaosFormationIndex)
					formations[i][UnrunnableOffset] &= 0xFD; // the last four fights only mark the B-side as runnable
				else
					formations[i][UnrunnableOffset] &= 0xFC; // while everything else in the normal encounter range is marked unrunnable
			}
			// Generate a shuffled list of ids for encounters
			// We include - all normal formation A-Sides except encounter 00 (imps), all normal formation B-Sides, and the four B-sides at the end
			List<int> ids = Enumerable.Range(1, NormalFormationCount - 1).Concat(Enumerable.Range(FormationCount, NormalFormationCount)).Concat(Enumerable.Range(FormationCount + ChaosFormationIndex + 1, 4)).ToList();
			ids.Shuffle(rng);
			ids = ids.Take(unrunnableAcount + unrunnableBcount).ToList(); // combine the number of unrunnables between both sides
			foreach (int id in ids)
			{
				if (id < FormationCount)
					formations[id][UnrunnableOffset] |= 0x01; // last bit is A-Side unrunnability
				else
					formations[id - FormationCount][UnrunnableOffset] |= 0x02; // and second-to-last bit is B-Side unrunnability
			}
			Put(FormationsOffset, formations.SelectMany(formation => formation.ToBytes()).ToArray()); // and put it all back in the ROM
		}

		public void CompletelyUnrunnable()
		{
			List<Blob> formations = Get(FormationsOffset, FormationSize * NormalFormationCount).Chunk(FormationSize);
			formations.ForEach(formation => formation[UnrunnableOffset] |= 0x03);
			Put(FormationsOffset, formations.SelectMany(formation => formation.ToBytes()).ToArray());

			List<Blob> lastFormations = Get(FormationsOffset + FormationSize * 0x7E, FormationSize * 2).Chunk(FormationSize);
			lastFormations.ForEach(formation => formation[UnrunnableOffset] |= 0x02);
			Put(FormationsOffset + FormationSize * 0x7E, lastFormations.SelectMany(formation => formation.ToBytes()).ToArray());
		}

		public void CompletelyRunnable()
		{
			List<Blob> formations = Get(FormationsOffset, FormationSize * NormalFormationCount).Chunk(FormationSize);
			formations.ForEach(formation => formation[UnrunnableOffset] &= 0xFC);
			Put(FormationsOffset, formations.SelectMany(formation => formation.ToBytes()).ToArray());

			List<Blob> lastFormations = Get(FormationsOffset + FormationSize * 0x7E, FormationSize * 2).Chunk(FormationSize);
			lastFormations.ForEach(formation => formation[UnrunnableOffset] &= 0xFD);
			Put(FormationsOffset + FormationSize * 0x7E, lastFormations.SelectMany(formation => formation.ToBytes()).ToArray());
		}

		private void FiendShuffle(MT19337 rng)
		{
			//Shuffle the four Fiend1 fights.
			//Specifically, shuffle what fight triggers during dialog with each of the Elemental Orbs
			int Fiend1Offset = 119;
			List<Blob> fiendFormations = Get(FormationsOffset + FormationSize*Fiend1Offset, FormationSize * 4).Chunk(FormationSize);
			fiendFormations.Shuffle(rng);
			Put(FormationsOffset + FormationSize * Fiend1Offset, fiendFormations.SelectMany(formation => formation.ToBytes()).ToArray());
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

		public void AllowStrikeFirstAndSurprise(bool UnrunnableToWait, bool AllowStrikeFirstAndSurpriseflag)
		{
			if (AllowStrikeFirstAndSurpriseflag)
			{
				PutInBank(0x0C, 0x93D4, Blob.FromHex("EAEA"));
			}
			PutInBank(0x0C, 0xA3E0, Blob.FromHex($"AD916D2903D0{(UnrunnableToWait ? "25" : "36")}ADAE6BD036")); // we dont want to be able to run if we get a first strike on an unrunnable
		}

		public void MakeWarMECHUnrunnable()
		{
			// This needs to be called after ShuffleUnrunnable, otherwise it will shuffle away this unrunnability.
			Blob warMECHFormation = Get(FormationsOffset + FormationSize * WarMECHFormationIndex, FormationSize);
			warMECHFormation[UnrunnableOffset] |= 0x01;
			Put(FormationsOffset + FormationSize * WarMECHFormationIndex, warMECHFormation);
		}

		public void TransformFinalFormation(FinalFormation formation, EvadeCapValues evadeClampFlag)
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
					ScaleSingleEnemyStats(0x78, 140, 140, false, false, null, false, 100, 100, GetEvadeIntFromFlag(evadeClampFlag));
					ScaleSingleEnemyStats(0x33, 120, 120, false, false, null, false, 100, 100, GetEvadeIntFromFlag(evadeClampFlag));
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

		public void PacifistEnd()
		{
			// Remove ToFR Fiends tiles
			var tilesets = Get(TilesetDataOffset, TilesetDataCount * TilesetDataSize * TilesetCount).Chunk(TilesetDataSize).ToList();
			tilesets.ForEach(tile =>
			{
				if (IsBossTrapTile(tile))
				{
					tile[1] = 0x80;
				}
			});
			Put(TilesetDataOffset, tilesets.SelectMany(tileset => tileset.ToBytes()).ToArray());

			// Get all NPC scripts and script values to update them
			var npcScript = GetFromBank(newTalkRoutinesBank, lut_MapObjTalkJumpTbl, 0xD0 * 2).Chunk(2);

			var Talk_Ending = Blob.FromHex("4693");

			for (int i = 0; i < 0xD0; i++)
			{
				if (npcScript[i] == newTalk.Talk_fight)
					npcScript[i] = newTalk.Talk_CoOGuy;
			}

			// Update Chaos script
			npcScript[0x1A] = Talk_Ending;

			// Reinsert updated scripts
			PutInBank(newTalkRoutinesBank, lut_MapObjTalkJumpTbl, npcScript.SelectMany(script => script.ToBytes()).ToArray());

			//Update Talk_CooGuy and change Talk_fight to load End game
			PutInBank(newTalkRoutinesBank, 0x933B, Blob.FromHex("A476207F90209690A571604C38C9"));

			//Update Astos and Bikke
			PutInBank(newTalkRoutinesBank, 0x93C0, Blob.FromHex("EAEAEA"));
			PutInBank(newTalkRoutinesBank, 0x9507, Blob.FromHex("EAEAEA"));

		}
	}

}
