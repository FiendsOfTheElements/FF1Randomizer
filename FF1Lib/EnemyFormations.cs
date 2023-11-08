using System.ComponentModel;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public enum FinalFormation
		{
			[Description("None")]
			None,
			[Description("Random")]
			Random,
			[Description("WarMech and Friends")]
			WarMECHsAndFriends,
			[Description("Double Dragons")]
			KaryAndTiamat,
			[Description("The Fundead")]
			TheFundead,
			[Description("Whack-a-Garland")]
			TimeLoop,
			[Description("The Sahagin Wagon")]
			KrakensAndSahags,
			[Description("Snake Pit")]
			SnakePit,
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

		public void ShuffleUnrunnable(MT19337 rng, Flags flags, int unrunnablePercent)
		{
			// Load a list of formations from ROM
			List<Blob> formations = Get(FormationsOffset, FormationSize * FormationCount).Chunk(FormationSize);
			// First we mark all formations as runnable except for fiends/chaos (both sides) or the other boss fights (on a-side only)
			for (int i = 0; i < FormationCount; ++i)
			{
				if (i >= NormalFormationCount && i <= ChaosFormationIndex)
					continue; // skip fiends and Chaos on both sides
				if (i > ChaosFormationIndex)
					formations[i][UnrunnableOffset] &= 0xFD; // the last four fights only mark the B-side as runnable
				else
					formations[i][UnrunnableOffset] &= 0xFC; // while everything else in the normal encounter range is marked unrunnable
			}
			// Generate a shuffled list of ids for encounters
			// We include - all normal formation A-Sides except encounter 00 (imps), all normal formation B-Sides, and the four B-sides at the end
			List<int> ids = Enumerable.Range(1, NormalFormationCount - 1).Concat(Enumerable.Range(FormationCount, NormalFormationCount)).Concat(Enumerable.Range(FormationCount + ChaosFormationIndex + 1, 4)).ToList();
			ids.Shuffle(rng);

			// Take unrunnable percentage from flags and convert into number of encounters that are unrunnable
			// The +1 to NormalFormationCount is needed to account for the extra B-side formation spots
			int unrunnableTotal = (int)Math.Round((unrunnablePercent * (NormalFormationCount + 1) / 100.0 * 2));

			ids = ids.Take(unrunnableTotal).ToList();
			foreach (int id in ids)
			{
				if (id < FormationCount)
					formations[id][UnrunnableOffset] |= 0x01; // last bit is A-Side unrunnability
				else
					formations[id - FormationCount][UnrunnableOffset] |= 0x02; // and second-to-last bit is B-Side unrunnability
			}
			Put(FormationsOffset, formations.SelectMany(formation => formation.ToBytes()).ToArray()); // and put it all back in the ROM
		}

		// TODO Should probably be merged into the above, and just include the Imp Formation if the percentage is 99.8+
		public void CompletelyUnrunnable()
		{
			List<Blob> formations = Get(FormationsOffset, FormationSize * NormalFormationCount).Chunk(FormationSize);
			formations.ForEach(formation => formation[UnrunnableOffset] |= 0x03);
			Put(FormationsOffset, formations.SelectMany(formation => formation.ToBytes()).ToArray());

			List<Blob> lastFormations = Get(FormationsOffset + FormationSize * 0x7E, FormationSize * 2).Chunk(FormationSize);
			lastFormations.ForEach(formation => formation[UnrunnableOffset] |= 0x02);
			Put(FormationsOffset + FormationSize * 0x7E, lastFormations.SelectMany(formation => formation.ToBytes()).ToArray());
		}

		// Should be obsolete, leaving here just in case
		/*public void CompletelyRunnable()
		{
			List<Blob> formations = Get(FormationsOffset, FormationSize * NormalFormationCount).Chunk(FormationSize);
			formations.ForEach(formation => formation[UnrunnableOffset] &= 0xFC);
			Put(FormationsOffset, formations.SelectMany(formation => formation.ToBytes()).ToArray());

			List<Blob> lastFormations = Get(FormationsOffset + FormationSize * 0x7E, FormationSize * 2).Chunk(FormationSize);
			lastFormations.ForEach(formation => formation[UnrunnableOffset] &= 0xFD);
			Put(FormationsOffset + FormationSize * 0x7E, lastFormations.SelectMany(formation => formation.ToBytes()).ToArray());
		}*/

		private void FiendShuffle(MT19337 rng)
		{
			//Shuffle the four Fiend1 fights.
			//Specifically, shuffle what fight triggers during dialog with each of the Elemental Orbs
			int Fiend1Offset = 119;
			List<Blob> fiendFormations = Get(FormationsOffset + FormationSize * Fiend1Offset, FormationSize * 4).Chunk(FormationSize);
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

		public void TransformFinalFormation(FinalFormation formation, EvadeCapValues evadeClampFlag, MT19337 rng)
		{
			if (formation == FinalFormation.None) // This shouldnt be possible, but we are still checking anyways to be safe.
			{
				return;
			}

			Blob finalBattle = Get(FormationsOffset + ChaosFormationIndex * FormationSize, FormationSize);
			if (formation == FinalFormation.Random)
			{
				formation = (FinalFormation)rng.Between(2, Enum.GetValues(typeof(FinalFormation)).Length - 1); // First two are None and Random
				System.Diagnostics.Debug.Assert(formation != FinalFormation.None);
				System.Diagnostics.Debug.Assert(formation != FinalFormation.Random);
			}

			switch (formation)
			{
				case FinalFormation.WarMECHsAndFriends:
					finalBattle[TypeOffset] = 0x2C;         // Big/Small Enemy mix, and the Astos/Madpony/Badman/WarMECH patterns
					finalBattle[GFXOffset] = 0x03;          // WarMECH Badman N/A N/A
					finalBattle[IDsOffset + 0] = Enemy.WarMech;
					finalBattle[IDsOffset + 1] = Enemy.Evilman;
					finalBattle[QuantityOffset + 0] = 0x22;
					finalBattle[QuantityOffset + 1] = 0x66;
					finalBattle[PalettesOffset + 0] = 0x2F;
					finalBattle[PalettesOffset + 1] = 0x17;
					finalBattle[PaletteAsignmentOffset] = 0x41; // Palette Assignment in top nibble, 1 in bottom for unrunnable.
					break;
				case FinalFormation.KaryAndTiamat:
					finalBattle[TypeOffset] = 0x2B;         // Big/Small Enemy mix, and the Dragon2 pattern
					finalBattle[GFXOffset] = 0x05;          // Dragon Dragon N/A N/A
					finalBattle[IDsOffset + 0] = Enemy.Kary2;
					finalBattle[IDsOffset + 1] = Enemy.Tiamat2;
					finalBattle[QuantityOffset + 0] = 0x11;
					finalBattle[QuantityOffset + 1] = 0x11;
					finalBattle[PalettesOffset + 0] = 0x08;
					finalBattle[PalettesOffset + 1] = 0x0A;
					finalBattle[PaletteAsignmentOffset] = 0x41; // Palette Assignment in top nibble, 1 in bottom for unrunnable.
					break;
				case FinalFormation.TheFundead:
					finalBattle[TypeOffset] = 0x24;         // Eye pattern
					finalBattle[GFXOffset] = 0x0B;          // Eye / Geist
					finalBattle[IDsOffset + 0] = Enemy.Lich2;
					finalBattle[IDsOffset + 1] = Enemy.Phantom;
					finalBattle[QuantityOffset + 0] = 0x22;
					finalBattle[QuantityOffset + 1] = 0x44;
					finalBattle[PalettesOffset + 0] = 0x03;
					finalBattle[PalettesOffset + 1] = 0x17;
					finalBattle[PaletteAsignmentOffset] = 0x41; // Palette Assignment in top nibble, 1 in bottom for unrunnable.

					// Scale up the Fundead enemies if we end up with them. They're too weak otherwise.
					ScaleSingleEnemyStats(Enemy.Lich2, 140, 140, false, null, false, 100, 100, GetEvadeIntFromFlag(evadeClampFlag));
					ScaleSingleEnemyStats(Enemy.Phantom, 120, 120, false, null, false, 100, 100, GetEvadeIntFromFlag(evadeClampFlag));
					break;
				case FinalFormation.TimeLoop:
					finalBattle[TypeOffset] = 0x0B;         // 9Small + Garland pattern
					finalBattle[GFXOffset] = 0x2A;          // Garland Garland Garland N/A
					finalBattle[IDsOffset + 0] = Enemy.Garland;
					finalBattle[IDsOffset + 1] = Enemy.Chaos;
					finalBattle[IDsOffset + 2] = Enemy.Garland;
					finalBattle[QuantityOffset + 0] = 0x08;
					finalBattle[QuantityOffset + 1] = 0x11;
					finalBattle[QuantityOffset + 2] = 0x88;
					finalBattle[PalettesOffset + 0] = 0x00;
					finalBattle[PalettesOffset + 1] = 0x00;
					finalBattle[PaletteAsignmentOffset] = 0x01; // Palette Assignment in top nibble, 1 in bottom for unrunnable.
					break;
				case FinalFormation.KrakensAndSahags:
					finalBattle[TypeOffset] = 0x21;         // 2/4 + Sahag Pattern
					finalBattle[GFXOffset] = 0x0F;          // BigEye BigEye Sahag Sahag
					finalBattle[IDsOffset + 0] = Enemy.Kraken;
					finalBattle[IDsOffset + 1] = Enemy.Kraken2;
					finalBattle[IDsOffset + 2] = Enemy.RSahag;
					finalBattle[IDsOffset + 3] = Enemy.WzSahag;
					finalBattle[QuantityOffset + 0] = 0x11;
					finalBattle[QuantityOffset + 1] = 0x11;
					finalBattle[QuantityOffset + 2] = 0x24;
					finalBattle[QuantityOffset + 3] = 0x44;
					finalBattle[PalettesOffset + 0] = 0x08;
					finalBattle[PalettesOffset + 1] = 0x0B;
					finalBattle[PaletteAsignmentOffset] = 0x51; // Palette Assignment in top nibble, 1 in bottom for unrunnable.
					BoostEnemyMorale(Enemy.RSahag);
					BoostEnemyMorale(Enemy.WzSahag);
					break;
				case FinalFormation.SnakePit:
					finalBattle[TypeOffset] = 0x03;         // 9 Small + Asp pattern
					finalBattle[GFXOffset] = 0x00;          // Asp Asp Asp Asp
					finalBattle[IDsOffset + 0] = Enemy.Asp;
					finalBattle[IDsOffset + 1] = Enemy.Tiamat;
					finalBattle[IDsOffset + 2] = Enemy.Tiamat2;
					finalBattle[IDsOffset + 3] = Enemy.Cobra;
					finalBattle[QuantityOffset + 0] = 0x16;
					finalBattle[QuantityOffset + 1] = 0x11;
					finalBattle[QuantityOffset + 2] = 0x11;
					finalBattle[QuantityOffset + 3] = 0x66;
					finalBattle[PalettesOffset + 0] = 0x3C;
					finalBattle[PaletteAsignmentOffset] = 0x01; // Palette Assignment in top nibble, 1 in bottom for unrunnable.
					BoostEnemyMorale(Enemy.Asp);
					BoostEnemyMorale(Enemy.Cobra);
					break;
			}

			Put(FormationsOffset + ChaosFormationIndex * FormationSize, finalBattle);
		}

		public void PacifistEnd(TalkRoutines talkroutines, NPCdata npcdata, bool extendedtraptiles)
		{
			// Remove ToFR Fiends tiles
			var tilesets = Get(TilesetDataOffset, TilesetDataCount * TilesetDataSize * TilesetCount).Chunk(TilesetDataSize).ToList();
			tilesets.ForEach(tile =>
			{
				if (IsBossTrapTile(tile))
				{
					tile[1] = (byte)(extendedtraptiles ? 0x00 : 0x80);
				}
			});
			Put(TilesetDataOffset, tilesets.SelectMany(tileset => tileset.ToBytes()).ToArray());

			// Update Chaos script
			var Talk_Ending = talkroutines.Add(Blob.FromHex("4C38C9"));
			npcdata.SetRoutine((ObjectId)0x1A, (newTalkRoutines)Talk_Ending);

			//Update Fiends, Garland, Vampire, Astos and Bikke
			var battleJump = Blob.FromHex("2020B1");
			var mapreload = Blob.FromHex("201896");
			talkroutines.ReplaceChunk(newTalkRoutines.Talk_fight, battleJump, Blob.FromHex("EAEAEA"));
			talkroutines.ReplaceChunk(newTalkRoutines.Talk_fight, mapreload, Blob.FromHex("EAEAEA"));
			talkroutines.ReplaceChunk(newTalkRoutines.Talk_Bikke, battleJump, Blob.FromHex("EAEAEA"));
			talkroutines.ReplaceChunk(newTalkRoutines.Talk_Bikke, mapreload, Blob.FromHex("EAEAEA"));
			talkroutines.ReplaceChunk(newTalkRoutines.Talk_Astos, battleJump, Blob.FromHex("EAEAEA"));
			talkroutines.ReplaceChunk(newTalkRoutines.Talk_Astos, mapreload, Blob.FromHex("EAEAEA"));
		}
		public void PacifistBat(TalkRoutines talkroutines, NPCdata npcdata)
		{
			// Add Script
			var Talk_Ending = talkroutines.Add(Blob.FromHex("4C38C9"));
			npcdata.SetRoutine(ObjectId.MatoyaBroom4, (newTalkRoutines)Talk_Ending);
			var broom4 = GetNpc(MapId.MatoyasCave, 4);
			var bat3 = GetNpc(MapId.MarshCaveB1, 3);
			SetNpc(MapId.MatoyasCave, 4, ObjectId.MatoyaBroom3, broom4.Coord.x, broom4.Coord.y, true, false);
			SetNpc(MapId.MarshCaveB1, 3, ObjectId.MatoyaBroom4, bat3.Coord.x, bat3.Coord.y, false, false);
			Data[MapObjGfxOffset + (int)ObjectId.MatoyaBroom4] = 0x11;
		}

		public enum FormationPattern
		{
			Small9 = 0,
			Large4 = 1,
			Mixed = 2,
			Fiends = 3,
			Chaos = 4
		}

		public enum FormationSpriteSheet
		{
			ImpWolfIguanaGiant = 0,
			SahagPirateSharkBigEye = 1,
			BoneCreepHyenaOgre = 2,
			AspLobsterBullTroll = 3,
			ImageGeistWormEye = 4,
			MedusaCatmanPedeTiger = 5,
			VampGargoyleEarthDragon1 = 6,
			SlimeSpiderManticorAnkylo = 7,
			MummyCoctricWyvernTyro = 8,
			CaribeGatorOchoHydra = 9,
			SentryWaterNagaChimera = 10,
			WizardGarlandDragon2Golem = 11,
			BadmanAstosMadponyWarmech = 12,
			KaryLich = 13,
			KrakenTiamat = 14,
			Chaos = 15
		}
		public enum FormationGFX
		{
			Sprite1 = 0,
			Sprite2 = 2,
			Sprite3 = 1,
			Sprite4 = 3
		}
		public class Encounters
		{
			public List<FormationData> formations = new List<FormationData>();

			public class FormationData
			{
				public FormationPattern pattern { get; set; }
				public FormationSpriteSheet spriteSheet { get; set; }
				public int palette1 { get; set; }
				public int palette2 { get; set; }
				public int enemy1 { get; set; }
				public int enemy2 { get; set; }
				public int enemy3 { get; set; }
				public int enemy4 { get; set; }
				public int gfxOffset1 { get; set; }
				public int gfxOffset2 { get; set; }
				public int gfxOffset3 { get; set; }
				public int gfxOffset4 { get; set; }
				public (int, int) minmax1 { get; set; }
				public (int, int) minmax2 { get; set; }
				public (int, int) minmax3 { get; set; }
				public (int, int) minmax4 { get; set; }
				public int paletteAssign1 { get; set; }
				public int paletteAssign2 { get; set; }
				public int paletteAssign3 { get; set; }
				public int paletteAssign4 { get; set; }
				public bool unrunnableA { get; set; }
				public bool unrunnableB { get; set; }
				public (int, int) minmaxB1 { get; set; }
				public (int, int) minmaxB2 { get; set; }
				public int supriseFactor { get; set; }


				public FormationData(byte[] formationdata)
				{
					LoadData(formationdata);
				}

				public void LoadData(byte[] formationdata)
				{
					pattern = (FormationPattern)(formationdata[TypeOffset] / 0x10);
					spriteSheet = (FormationSpriteSheet)(formationdata[TypeOffset] & 0x0F);
					gfxOffset1 = formationdata[GFXOffset] & 0x03;
					gfxOffset2 = (formationdata[GFXOffset] / 0x04) & 0x03;
					gfxOffset3 = (formationdata[GFXOffset] / 0x10) & 0x03;
					gfxOffset4 = (formationdata[GFXOffset] / 0x40) & 0x03;
					enemy1 = formationdata[IDsOffset + 0];
					enemy2 = formationdata[IDsOffset + 1];
					enemy3 = formationdata[IDsOffset + 2];
					enemy4 = formationdata[IDsOffset + 3];
					minmax1 = (formationdata[QuantityOffset + 0] / 0x10, formationdata[QuantityOffset + 0] & 0x0F);
					minmax2 = (formationdata[QuantityOffset + 1] / 0x10, formationdata[QuantityOffset + 1] & 0x0F);
					minmax3 = (formationdata[QuantityOffset + 2] / 0x10, formationdata[QuantityOffset + 2] & 0x0F);
					minmax4 = (formationdata[QuantityOffset + 3] / 0x10, formationdata[QuantityOffset + 3] & 0x0F);
					palette1 = formationdata[PalettesOffset + 0];
					palette2 = formationdata[PalettesOffset + 1];
					paletteAssign1 = ((formationdata[PaletteAsignmentOffset] & 0x80) > 0) ? 1 : 0;
					paletteAssign2 = ((formationdata[PaletteAsignmentOffset] & 0x40) > 0) ? 1 : 0;
					paletteAssign3 = ((formationdata[PaletteAsignmentOffset] & 0x20) > 0) ? 1 : 0;
					paletteAssign4 = ((formationdata[PaletteAsignmentOffset] & 0x10) > 0) ? 1 : 0;
					unrunnableA = (formationdata[PaletteAsignmentOffset] & 0x01) == 0 ? false : true;
					unrunnableB = (formationdata[PaletteAsignmentOffset] & 0x02) == 0 ? false : true;
					minmaxB1 = (formationdata[QuantityBOffset + 0] / 0x10, formationdata[QuantityBOffset + 0] & 0x0F);
					minmaxB2 = (formationdata[QuantityBOffset + 1] / 0x10, formationdata[QuantityBOffset + 1] & 0x0F);
					supriseFactor = formationdata[0x0C];
				}

				public Blob OutputBlob()
				{
					var formationdata = new byte[0x10];

					formationdata[TypeOffset] = (byte)((int)pattern * 0x10 + (int)spriteSheet);
					formationdata[GFXOffset] = (byte)(gfxOffset1 + gfxOffset2 * 0x04 + gfxOffset3 * 0x10 + gfxOffset4 * 0x40);

					formationdata[IDsOffset + 0] = (byte)enemy1;
					formationdata[IDsOffset + 1] = (byte)enemy2;
					formationdata[IDsOffset + 2] = (byte)enemy3;
					formationdata[IDsOffset + 3] = (byte)enemy4;

					formationdata[QuantityOffset + 0] = (byte)(minmax1.Item1 * 0x10 + minmax1.Item2);
					formationdata[QuantityOffset + 1] = (byte)(minmax2.Item1 * 0x10 + minmax2.Item2);
					formationdata[QuantityOffset + 2] = (byte)(minmax3.Item1 * 0x10 + minmax3.Item2);
					formationdata[QuantityOffset + 3] = (byte)(minmax4.Item1 * 0x10 + minmax4.Item2);

					formationdata[PalettesOffset + 0] = (byte)palette1;
					formationdata[PalettesOffset + 1] = (byte)palette2;

					formationdata[PaletteAsignmentOffset] = (byte)(paletteAssign1 * 0x80 + paletteAssign2 * 0x40 + paletteAssign3 * 0x20 + paletteAssign4 * 0x10
						+ (unrunnableB ? 0x02 : 0x00) + (unrunnableA ? 0x01 : 0x00));

					formationdata[QuantityBOffset + 0] = (byte)(minmaxB1.Item1 * 0x10 + minmaxB1.Item2);
					formationdata[QuantityBOffset + 1] = (byte)(minmaxB2.Item1 * 0x10 + minmaxB2.Item2);

					formationdata[0x0C] = (byte)supriseFactor;

					return formationdata;
				}
			}

			public Encounters(FF1Rom rom)
			{
				var encounterData = rom.Get(FormationsOffset, FormationCount * FormationSize).Chunk(FormationSize);

				foreach (var formation in encounterData)
				{
					formations.Add(new FormationData(formation));
				}
			}

			public void Write(FF1Rom rom)
			{
				rom.Put(FormationsOffset, formations.SelectMany(encounterData => encounterData.OutputBlob().ToBytes()).ToArray());
			}
		}
	}

}
