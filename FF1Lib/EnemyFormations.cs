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
					ScaleSingleEnemyStats(0x78, 140, 140, false, false, null, false, 100, 100);
					ScaleSingleEnemyStats(0x33, 120, 120, false, false, null, false, 100, 100);
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

		public void Spooky(MT19337 rng, Flags flags)
		{
			const byte WarMECHEncounter = 0x56;
			const byte bWarMECHEncounter = 0xFD;
			const byte Lich1Encounter = 0x7A;
			const byte Lich2Encounter = 0x73;
			const byte ChaosEncounter = 0x7B;
			const byte VampEncounter = 0x7C;

			const byte singleVamp = 0xFC;
			const byte singleZombie = 0x04;
			const byte singleGhost = 0x46;
			const byte singleGeist = 0x08;
			const byte singleZomBull = 0xB2;
			const byte singleZombieD = 0x4B;

			var zombieDialog = new List<byte> { 0x32, 0x33, 0x34, 0x36 };

			Dictionary<int, string> evilDialogs = new Dictionary<int, string>();

			var encounterData = Get(FormationsOffset, FormationCount * FormationSize).Chunk(FormationSize);

			// Replacement WarMech
			encounterData[bWarMECHEncounter - 0x80][GFXOffset] = 0x0E;
			encounterData[bWarMECHEncounter - 0x80][IDsOffset + 1] = 0x60;      // Chaos
			encounterData[bWarMECHEncounter - 0x80][PalettesOffset + 1] = 0x2F;
			encounterData[bWarMECHEncounter - 0x80][PaletteAsignmentOffset] = 0x73;
			encounterData[bWarMECHEncounter - 0x80][QuantityBOffset + 0] = 0x00;
			encounterData[bWarMECHEncounter - 0x80][QuantityBOffset + 1] = 0x11;

			// Single Vampire / WizVamp
			encounterData[VampEncounter][PaletteAsignmentOffset] |= 0x01;
			encounterData[VampEncounter][GFXOffset] = 0x00;          // Garland Garland Garland N/A
			encounterData[VampEncounter][IDsOffset + 0] = 0x3D;      // Garland
			encounterData[VampEncounter][IDsOffset + 1] = 0x3C;      // Chaos
			encounterData[VampEncounter][QuantityOffset + 0] = 0x11;
			encounterData[VampEncounter][QuantityOffset + 1] = 0x00;
			encounterData[VampEncounter][PalettesOffset + 0] = 0x20;
			encounterData[VampEncounter][PalettesOffset + 1] = 0x1F;
			encounterData[VampEncounter][PaletteAsignmentOffset] = 0x73;
			encounterData[VampEncounter][QuantityBOffset + 0] = 0x00;
			encounterData[VampEncounter][QuantityBOffset + 1] = 0x11;

			// Single Zombie
			encounterData[singleZombie][QuantityOffset + 0] = 0x11;
			encounterData[singleZombie][PaletteAsignmentOffset] |= 0x01;

			// Single Ghost
			encounterData[singleGhost][QuantityOffset + 0] = 0x11;
			encounterData[singleGhost][QuantityOffset + 1] = 0x00;
			encounterData[singleGhost][PaletteAsignmentOffset] |= 0x01;

			// Single Geist
			encounterData[singleGeist][QuantityOffset + 0] = 0x00;
			encounterData[singleGeist][QuantityOffset + 1] = 0x11;
			encounterData[singleGeist][PaletteAsignmentOffset] |= 0x01;

			// Single zomBull
			encounterData[singleZomBull - 0x80][QuantityBOffset + 0] = 0x11;
			encounterData[singleZomBull - 0x80][QuantityBOffset + 1] = 0x00;
			encounterData[singleZomBull - 0x80][PaletteAsignmentOffset] |= 0x02;

			// Single zombieD
			encounterData[singleZombieD][QuantityOffset + 0] = 0x11;
			encounterData[singleZombieD][PaletteAsignmentOffset] |= 0x01;

			// Phantom is Lich1
			encounterData[Lich1Encounter][TypeOffset] = 0x24;
			encounterData[Lich1Encounter][GFXOffset] = 0x03;
			encounterData[Lich1Encounter][QuantityOffset + 0] = 0x11;
			encounterData[Lich1Encounter][PalettesOffset + 0] = 0x16;

			// Phantom is Lich2
			encounterData[Lich2Encounter][TypeOffset] = 0x24;
			encounterData[Lich2Encounter][GFXOffset] = 0x03;
			encounterData[Lich2Encounter][QuantityOffset + 0] = 0x11;
			encounterData[Lich2Encounter][PalettesOffset + 0] = 0x16;

			// Lich is Chaos
			encounterData[ChaosEncounter][TypeOffset] = 0x3D;
			encounterData[ChaosEncounter][GFXOffset] = 0x01;
			encounterData[ChaosEncounter][QuantityOffset + 0] = 0x11;
			encounterData[ChaosEncounter][PalettesOffset + 0] = 0x36;
			encounterData[ChaosEncounter][PalettesOffset + 1] = 0x37;

			// Lich is Warmech
			encounterData[WarMECHEncounter][TypeOffset] = 0x3D;
			encounterData[WarMECHEncounter][GFXOffset] = 0x01;
			encounterData[WarMECHEncounter][QuantityOffset + 0] = 0x11;
			encounterData[WarMECHEncounter][PalettesOffset + 0] = 0x07;
			encounterData[WarMECHEncounter][PalettesOffset + 1] = 0x07;
			encounterData[WarMECHEncounter][QuantityBOffset + 0] = 0x11;
			encounterData[WarMECHEncounter][QuantityBOffset + 1] = 0x00;
			encounterData[WarMECHEncounter][PaletteAsignmentOffset] |= 0x02;

			Put(FormationsOffset, encounterData.SelectMany(encounterData => encounterData.ToBytes()).ToArray());

			//Update enemies names
			var enemyText = ReadText(EnemyTextPointerOffset, EnemyTextPointerBase, EnemyCount);
			enemyText[118] = "LICH?"; // WarMech > Lich?
			enemyText[119] = "PHANTOM"; // Lich1 > Phantom
			enemyText[120] = ""; // Lich2 > Phantom
			enemyText[127] = "LICH"; // Chaos > Lich
			WriteText(enemyText, EnemyTextPointerOffset, EnemyTextPointerBase, EnemyTextOffset);
			var test = Get(EnemyTextPointerOffset + 119 * 2, 2); // Lich2 point to Phantom1
			Put(EnemyTextPointerOffset + 120 * 2, test);

			// Scale Undeads
			ScaleSingleEnemyStats(0x15, 120, 120, false, false, null, false, 120, 120); // Bone
			ScaleSingleEnemyStats(0x16, 120, 120, false, false, null, false, 120, 120); // R.Bone
			ScaleSingleEnemyStats(0x24, 120, 120, false, false, null, false, 120, 120); // ZomBull
			ScaleSingleEnemyStats(0x27, 120, 120, false, false, null, false, 120, 120); // Shadow
			ScaleSingleEnemyStats(0x28, 120, 120, false, false, null, false, 120, 120); // Image
			ScaleSingleEnemyStats(0x29, 120, 120, false, false, null, false, 120, 120); // Wraith
			ScaleSingleEnemyStats(0x2A, 120, 120, false, false, null, false, 120, 120); // Ghost
			ScaleSingleEnemyStats(0x2B, 120, 120, false, false, null, false, 120, 120); // Zombie
			ScaleSingleEnemyStats(0x2C, 120, 120, false, false, null, false, 120, 120); // Ghoul
			ScaleSingleEnemyStats(0x2D, 120, 120, false, false, null, false, 120, 120); // Geist
			ScaleSingleEnemyStats(0x2E, 120, 120, false, false, null, false, 120, 120); // Specter
			ScaleSingleEnemyStats(0x33, 120, 120, false, false, null, false, 120, 120); // Phantom
			ScaleSingleEnemyStats(0x3C, 120, 120, false, false, null, false, 120, 120); // Vampire
			ScaleSingleEnemyStats(0x3D, 120, 120, false, false, null, false, 120, 120); // WzVampire
			ScaleSingleEnemyStats(0x44, 120, 120, false, false, null, false, 120, 120); // Zombie D
			ScaleSingleEnemyStats(0x4F, 120, 120, false, false, null, false, 120, 120); // Mummy
			ScaleSingleEnemyStats(0x50, 120, 120, false, false, null, false, 120, 120); // WzMummy
			ScaleSingleEnemyStats(0x77, 120, 120, false, false, null, false, 120, 120); // Lich1
			ScaleSingleEnemyStats(0x78, 120, 120, false, false, null, false, 120, 120); // Lich2
			ScaleSingleEnemyStats(0x7F, 120, 120, false, false, null, false, 120, 120); // Chaos

			// Intro
			Blob intro = FF1Text.TextToStory(new string[]
			{
				"I was flipping through the", "",
				"Book of Death when I thought", "",
				"how marvelous it would be to", "",
				"be a necromancer.", "",
				"How fantastic.", "",
				"To go beyond human.", "",
				"A living corpse. An undead.", "",
				"I am damned! I am evil!", "",
				"I MAIM AND KILL.", "",
				"I AM A BEAST OF BRUTAL WILL.", "",
				"I AM DEATH..", "", "",
				"I. AM. LICH."
			});
			System.Diagnostics.Debug.Assert(intro.Length <= 208);
			Put(0x37F20, intro);

			// Get all NPC scripts and script values to update them
			var npcScript = new List<Blob>();
			var npcScriptValue = new List<Blob>();

			for (int i = 0; i < 0xD0; i++)
			{
				npcScriptValue.Add(Get(MapObjOffset + i * 4, 4));
				npcScript.Add(Get(0x390D3 + i * 2, 2));
			}

			var validTalk = new List<Blob> { newTalk.Talk_norm, newTalk.Talk_GoBridge, newTalk.Talk_ifearthfire, newTalk.Talk_ifearthvamp, newTalk.Talk_ifevent, newTalk.Talk_ifitem, newTalk.Talk_ifkeytnt, newTalk.Talk_ifvis, newTalk.Talk_Invis, newTalk.Talk_4Orb };
			var invalidZombie = new List<ObjectId> { ObjectId.Bat, ObjectId.GaiaBroom, ObjectId.MatoyaBroom1, ObjectId.MatoyaBroom2, ObjectId.MatoyaBroom3, ObjectId.MatoyaBroom4, ObjectId.MirageRobot1, ObjectId.MirageRobot2, ObjectId.MirageRobot3, ObjectId.SkyRobot, ObjectId.LutePlate, ObjectId.RodPlate };
			var validZombie = new List<ObjectId>();

			if (flags.HintsVillage ?? false)
				invalidZombie.AddRange(new List<ObjectId> { ObjectId.ConeriaOldMan, ObjectId.PravokaOldMan, ObjectId.ElflandScholar1, ObjectId.MelmondOldMan2, ObjectId.CrescentSage11, ObjectId.OnracOldMan2, ObjectId.GaiaWitch, ObjectId.LefeinMan12 });

			if (flags.HintsDungeon ?? false)
				invalidZombie.AddRange(new List<ObjectId> { ObjectId.OnracPunk2, ObjectId.DwarfcaveDwarf4, ObjectId.CardiaDragon2, ObjectId.SkyRobot, ObjectId.Mermaid3 });

			// Change base NPCs' scripts to Talk_fight
			for (int i = 0; i < 0xD0; i++)
			{
				if (validTalk.Contains(npcScript[i]) && !(invalidZombie.Contains((ObjectId)i)))
				{
					npcScript[i] = newTalk.Talk_fight;
					if ((i >= 0x85 && i <= 0x90) || i == 0x9B)
						npcScriptValue[i][3] = singleZombieD;
					else
						npcScriptValue[i][3] = singleZombie;

					npcScriptValue[i][1] = zombieDialog.PickRandom(rng);
					validZombie.Add((ObjectId)i);
				}
			}

			// Update Garland's script
			npcScript[(int)ObjectId.Garland] = newTalk.Talk_CoOGuy;

			// Change dialogues
			evilDialogs.Add(0x32, "Braaaaain!");
			evilDialogs.Add(0x33, "Barf!");
			evilDialogs.Add(0x34, "Uaaaaaargh!");
			evilDialogs.Add(0x36, "Groaaarn!");

			evilDialogs.Add(0x04, "What the hell!?\nThat princess is crazy,\nshe tried to bite me!\n\nThat's it. Screw that.\nI'm going home.");

			evilDialogs.Add(0x02, "What is going on!? My\nguard tried to kill me!\nUgh.. this is a deep\nwound.. I don't feel so\nwell..\nGwooorrrgl!\n\nReceived #");
			npcScriptValue[(int)ObjectId.King][1] = singleZombie;

			evilDialogs.Add(0x06, "So, you are.. the..\nLIGHTarrgaar..\nWarglb..\n\nBraaaain...\n\nReceived #");
			npcScriptValue[(int)ObjectId.Princess2][1] = singleZombie;
			//evilDialogs.Add(0x09, "Okay, you got me.\nTake this.\n\n\n\nReceived #");

			evilDialogs.Add(0x0E, "At last I wake up from\nmy eternal slumber.\nCome, LIGHT WARRIORS,\nembrace the darkness,\njoin me in death..\n\nReceived #");
			npcScriptValue[(int)ObjectId.ElfPrince][1] = singleVamp;

			evilDialogs.Add(0x0C, "Yes, yes, the master\nwill be pleased. Let's\nclean this place up\nbefore he wakes.\nStarting with you!");
			npcScriptValue[(int)ObjectId.ElfDoc][1] = singleGeist;

			evilDialogs.Add(0x13, "The world is going to\nhell, but this won't\nstop me from digging\nmy canal!");
			evilDialogs.Add(0x14, "Excellent! Finally,\nnow Lich's undead army\ncan flow through the\nrest of the world!\n\nReceived #");
			npcScriptValue[(int)ObjectId.Nerrick][1] = singleVamp;

			evilDialogs.Add(0x15, "I never thought I'd\nhave to forge the\nweapon that would slay\nmy brothers. Bring me\nADAMANT, quick!");
			evilDialogs.Add(0x16, "You were too slow,\nLIGHT WARRIORS. You have\nforsaken me!\nJoin my damned soul in\nthe afterworld!\n\nReceived #");
			npcScriptValue[(int)ObjectId.Smith][1] = singleGhost;

			evilDialogs.Add(0x17, "Pfah! Everyone else can\nrot in Hell for all\nI care, I'm  perfectly\nsafe here!");
			evilDialogs.Add(0x19, "SCRIIIIIIIIIIIIIIIIIIII!\n\nReceived #");
			npcScriptValue[(int)ObjectId.Matoya][1] = singleGeist;

			evilDialogs.Add(0x1C, "Now, listen to me, a\nbasic word from\nLeifeinish is Lu..\nHack! Cough! Sorry,\nLu..lu..paaaargh!");
			npcScriptValue[(int)ObjectId.Unne][1] = singleGeist;

			evilDialogs.Add(0x1D, "Ah, humans who wish to\npay me tribute. What?\nYou miserable little\npile of secrets!\nEnough talk! Have at you!");

			evilDialogs.Add(0x1E, "I.. HUNGER!\n\nReceived #");
			npcScriptValue[(int)ObjectId.Sarda][1] = singleZomBull;

			evilDialogs.Add(0x23, "Come play with me,\nLIGHT WARRIORS.\nFor ever and ever\nand ever..\n\nReceived #");
			npcScriptValue[(int)ObjectId.Fairy][1] = singleGhost;

			evilDialogs.Add(0x27, "Exterminate.\n\n\n\n\nReceived #");
			npcScriptValue[(int)ObjectId.CubeBot][1] = bWarMECHEncounter;

			evilDialogs.Add(0x2B, "My friends..\nMy colleagues..\nNow.. I join them..\n\nReceived #");
			npcScriptValue[(int)ObjectId.CanoeSage][1] = singleZomBull;

			evilDialogs.Add(0xCD, "Luuuuu... paaaargh!\n\n\n\nReceived #");
			npcScriptValue[(int)ObjectId.Lefein][1] = singleZomBull;

			evilDialogs.Add(0xFA, "Sorry, LIGHT WARRIORS,\nbut your LICH is in\nanother castle!\n\nMwahahahaha!");
			evilDialogs.Add(0x2F, "You did well fighting\nmy Army of Darkness,\nLIGHT WARRIORS! But it\nis for naught!\nI am UNSTOPPABLE!\nThis time, YOU are\nthe SPEEDBUMP!");
			evilDialogs.Add(0x30, "HAHA! Alright, enough\nplaying around.");

			InsertDialogs(evilDialogs);

			// Update Chaos sprite
			Put(0x2F00 + 0x18, Blob.FromHex("00"));
			Put(0x2F00 + 0x19, Blob.FromHex("01"));
			Put(0x2F00 + 0x1A, Blob.FromHex("00"));
			Data[MapObjGfxOffset + 0x1A] = 0x0F;
			Data[MapObjGfxOffset + 0x19] = 0x0F;
			PutInBank(0x00, 0xA000 + ((byte)MapId.TempleOfFiendsRevisitedChaos * 0x30) + 0x18, Blob.FromHex("0F0F13300F0F1530"));

			npcScriptValue[0x19][0] = 0x2F;
			npcScriptValue[0x19][1] = WarMECHEncounter + 0x80;
			npcScriptValue[0x19][2] = 0x2F;
			npcScriptValue[0x19][3] = 0x1A;

			// Switch princess
			Data[MapSpriteOffset + ((byte)MapId.TempleOfFiends * MapSpriteCount + 1) * MapSpriteSize] = (byte)ObjectId.Princess2;
			Data[MapSpriteOffset + ((byte)MapId.ConeriaCastle2F * MapSpriteCount + 1) * MapSpriteSize] = (byte)ObjectId.None;
			Put(0x2F00 + 0x12, Blob.FromHex("01"));
			Put(0x2F00 + 0x03, Blob.FromHex("02"));

			// Let zombies roam free
			var npcMap = new List<MapId> { MapId.Cardia, MapId.BahamutsRoomB2, MapId.Coneria, MapId.ConeriaCastle1F, MapId.ConeriaCastle2F, MapId.CrescentLake, MapId.DwarfCave, MapId.Elfland, MapId.ElflandCastle, MapId.Gaia, MapId.Lefein, MapId.Melmond, MapId.Onrac, MapId.Pravoka };

			foreach (var map in npcMap)
			{
				for (var i = 0; i < 0x10; i++)
				{
					int offset = MapSpriteOffset + ((byte)map * MapSpriteCount + i) * MapSpriteSize;
					if (validZombie.Contains((ObjectId)Data[offset]))
						Data[offset + 1] &= 0b10111111;
				}
			}

			// Ulgy fix to not bug battles because of money
			var targetTalkScript = new List<Blob> { newTalk.Talk_Nerrick, newTalk.Talk_GiveItemOnFlag, newTalk.Talk_GiveItemOnItem, newTalk.Talk_TradeItems };
			for (int i = 0; i < 0x20; i++)
			{
				if (targetTalkScript.Contains(npcScript[i]) && npcScriptValue[i][3] >= 0x6C && npcScriptValue[i][3] <= 0xAF)
					npcScriptValue[i][3] = (byte)Item.Cabin;
			}

			// Reinsert updated scripts
			for (int i = 0; i < 0xD0; i++)
			{
				PutInBank(0x0E, 0x90D3 + i * 2, npcScript[i]);
				PutInBank(0x0E, 0x95D5 + i * 4, npcScriptValue[i]);
			}

			// Update Talk Script to have them fight
			PutInBank(0x0E, 0x93FE, Blob.FromHex("206095")); // Nerrick
			PutInBank(0x0E, 0x943C, Blob.FromHex("206095")); // Unne
			PutInBank(0x0E, 0x946C, Blob.FromHex("206095")); // Talk_GiveItemOnFlag
			PutInBank(0x0E, 0x949D, Blob.FromHex("206095")); // Talk_TradeItems
			PutInBank(0x0E, 0x94CA, Blob.FromHex("206095")); // Talk_GiveItemOnItem

			// Talk_Replace
			PutInBank(0x0E, 0x932E, Blob.FromHex("A41320A490A416206095A51260"));

			// Fight Script 
			PutInBank(0x0E, 0x9560, Blob.FromHex("A51185132073924C4393"));


		}
	}

}
