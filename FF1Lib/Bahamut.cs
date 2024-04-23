using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public partial class FF1Rom
	{
		public void FightBahamut(Flags flags, TalkRoutines talkroutines, NpcObjectData npcdata, ZoneFormations zoneformations, DialogueData dialogues, StandardMaps maps, MT19337 rng)
		{
			if (!(bool)flags.FightBahamut || flags.SpookyFlag || (bool)flags.RandomizeFormationEnemizer)
			{
				return;
			}

			bool removeTail = (bool)flags.NoTail;
			bool swoleBahamut = (bool)flags.SwoleBahamut;
			bool deepDungeon = flags.GameMode == GameModes.DeepDungeon;
			EvadeCapValues evadeClampFlag = flags.EvadeCap;



			const byte offsetAtoB = 0x80; // diff between A side and B side

			const byte encAnkylo = 0x71; // ANKYLO FORMATION
			const byte idAnkylo = 0x4E; // ANKYLO ENEMY #
			const byte encCerbWzOgre = 0x22; // CEREBUS + WzOGRE
			const byte encTyroWyvern = 0x3D + offsetAtoB; // TYRO + WYVERN
			const byte encGasD = 0x59; // GAS DRAGON
			const byte encBlueD = 0x4E; // BLUE DRAGON

			// Turn Ankylo into Bahamut
			var encountersData = new Encounters(this);
			encountersData.formations[encAnkylo].pattern = FormationPattern.Large4;
			encountersData.formations[encAnkylo].spriteSheet = FormationSpriteSheet.WizardGarlandDragon2Golem;
			encountersData.formations[encAnkylo].gfxOffset1 = (int)FormationGFX.Sprite3;
			encountersData.formations[encAnkylo].palette1 = 0x1C;
			encountersData.formations[encAnkylo].paletteAssign1 = 0;
			encountersData.formations[encAnkylo].minmax1 = (1, 1);
			encountersData.formations[encAnkylo].unrunnableA = true;
			encountersData.Write(this);

			EnemyInfo bahamutInfo = new EnemyInfo();
			bahamutInfo.decompressData(Get(EnemyOffset + (idAnkylo * EnemySize), EnemySize));
			bahamutInfo.morale = 255; // always prevent running away, whether swole or not
			bahamutInfo.monster_type = (byte)MonsterType.DRAGON;
			bahamutInfo.exp = 3000;
			bahamutInfo.gp = 1;
			bahamutInfo.hp = 525;      // subject to additional boss HP scaling
			bahamutInfo.num_hits = 1;

			// These stats are based on the ankylo base stats increased to about 120%
			// stats will be further scaled based on boss stat scaling
			bahamutInfo.damage = 118;
			bahamutInfo.absorb = 58;
			bahamutInfo.mdef = 188;
			bahamutInfo.accuracy = 106;
			bahamutInfo.critrate = 1;
			bahamutInfo.agility = 58;
			bahamutInfo.elem_weakness = (byte)SpellElement.None;

			if (swoleBahamut)
			{
				bahamutInfo.exp = 16000; // increase exp for swole bahamut, either mode
				bahamutInfo.hp = 700; // subject to additional boss HP scaling
				bahamutInfo.elem_resist = (byte)SpellElement.Poison; // no longer susceptible to BANE or BRAK

				int availableScript = bahamutInfo.AIscript;
				if (availableScript == 0xFF)
				{
					availableScript = searchForNoSpellNoAbilityEnemyScript();
				}

				if (availableScript >= 0 && Rng.Between(rng, 0, 3) > 0) // because spells and skills shuffle is common, allow RNG to also make physical bahamut (1 in 4)
				{
					// spells and skills were shuffled in a way that a script exists with NONES for all magic and skills
					// find and borrow that script to make a bahamut AI
					// (magical bahamut mode)

					// assign any enemies using the availabe script to use true NONE script
					setAIScriptToNoneForEnemiesUsing(availableScript);

					// pick skills from this list
					List<byte> potentialSkills = new List<byte> {
						(byte)EnemySkills.Snorting,
						(byte)EnemySkills.Stinger,
						(byte)EnemySkills.Cremate,
						(byte)EnemySkills.Blizzard,
						(byte)EnemySkills.Blaze,
						(byte)EnemySkills.Inferno,
						(byte)EnemySkills.Poison_Damage,
						(byte)EnemySkills.Thunder,
						(byte)EnemySkills.Tornado,
						(byte)EnemySkills.Nuclear,
						(byte)EnemySkills.Swirl,
					};
					potentialSkills.Shuffle(rng);

					// create and assign script
					defineNewAI(availableScript,
						spellChance: 0x00,
						skillChance: 0x40,
						spells: new List<byte> { (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE,
							(byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
						skills: new List<byte> { potentialSkills[0], potentialSkills[1], potentialSkills[2], potentialSkills[3] }
						);
					bahamutInfo.AIscript = (byte)availableScript;
					bahamutInfo.mdef = 215; // swole magical bahamut has increased MDEF
				}
				else
				{
					// no script is available: spells and skills aren't shuffled or we got unlucky
					// (physical bahamut mode)
					bahamutInfo.critrate = 20;
					bahamutInfo.num_hits = 2;
					bahamutInfo.AIscript = 0xFF;
				}
			}
			Put(EnemyOffset + (idAnkylo * EnemySize), bahamutInfo.compressData());

			// Update name
			var enemyText = ReadText(EnemyTextPointerOffset, EnemyTextPointerBase, EnemyCount);
			enemyText[78] = "BAHAMUT"; // +1 byte compared to ANKYLO, is this an issue?
			WriteText(enemyText, EnemyTextPointerOffset, EnemyTextPointerBase, EnemyTextOffset);

			// Remove Ankylo from the Overworld, with appropriate substitutions
			zoneformations.ReplaceEncounter(encAnkylo, encCerbWzOgre); // handle Ankylo A side (single Ankylo)
			zoneformations.ReplaceEncounter(encAnkylo + offsetAtoB, encTyroWyvern); // handle Ankylo B side (two Ankylos)

			// Update Bahamut behavior
			String asmNoTailPromote = "EE7D00AD7200203D96AD75002020B1AC7600207F9020739220AE952018964C439660"; // 11_820 LichsRevenge ASM : ClassChange_bB
			String asmTailRequiredPromote = "AD2D60D003A57160E67DA572203D96A5752020B1A476207F9020739220AE952018964C439660"; // 11_820 LichsRevenge ASM : Talk_battleBahamut
			String asm = "";
			String bahamutDialogue = "";
			if (removeTail)
			{
				asm = asmNoTailPromote;
				bahamutDialogue = "To prove your worth..\nYou must show me your\nstrength..\n\nCome at me WARRIORS,\nor die by my claws!";
			}
			else
			{
				asm = asmTailRequiredPromote;
				bahamutDialogue = "The TAIL! Impressive..\nNow show me your true\nstrength..\n\nCome at me WARRIORS,\nor die by my claws!";
			}
			var fightBahamut = talkroutines.Add(Blob.FromHex(asm));
			npcdata[ObjectId.Bahamut].Battle = encAnkylo;
			npcdata[ObjectId.Bahamut].Script = (TalkScripts)fightBahamut;

			// Fun Minion dialogue pairings
			var minionDialogs = new List<string> {
				"For the king!;For the master!",
				"No one approaches\nthe master and lives!;Who goes there!?\nYou are not worthy!",
				"Weaklings!\nYou will die here!;NOW you DIE!",
				"PFFT!;PEW..PEW..PEW!",
				"ROOOOAAR!!;HIIISSSS!!",
				"No! You are not welcome\nhere!;Get out get out get out!",
			};
			string[] minions = minionDialogs.PickRandom(rng).Split(";");

			// Update Dragon dialogs
			Dictionary<int, string> dialogs = new Dictionary<int, string>();
			dialogs.Add(0x20, bahamutDialogue);
			dialogs.Add(0xEC, minions[0]); // (CardiaDragon11 / Left) "This is BAHAMUT's room."
			dialogs.Add(0xED, minions[1]); // (CardiaDragon12 / Right) "BAHAMUT verifies the\ntrue courage of all."
			dialogs.Add(0xE9, "Have you met BAHAMUT,\nthe Dragon King? He\nfights those with\ncourage as true\nwarriors."); // Cardia Tiny
			dialogs.Add(0xE5, "You are not afraid of\nBAHAMUT??\nYou will be!"); // Cardia Forest
			dialogs.Add(0xAB, "Many have searched\nfor BAHAMUT, but,\nnone that found him\nsurvived."); // Onrac Pre-Promo
			dialogs.Add(0xAC, "Well, well..\nI see you have\nslayed the dragon."); // Onrac Post-Promo
			dialogues.InsertDialogues(dialogs);

			// Change Bahamut Dragon NPCs (Left/Right Minions)
			if (!deepDungeon)
			{
				npcdata[ObjectId.CardiaDragon11].Battle = encGasD;
				npcdata[ObjectId.CardiaDragon11].Script = TalkScripts.Talk_fight;

				npcdata[ObjectId.CardiaDragon12].Battle = encBlueD;
				npcdata[ObjectId.CardiaDragon12].Script = TalkScripts.Talk_fight;

				maps[MapIndex.BahamutCaveB2].MapObjects.MoveNpc(1, 20, 4, true, true);
				maps[MapIndex.BahamutCaveB2].MapObjects.MoveNpc(2, 22, 4, true, true);
			}
		}
	}
}
