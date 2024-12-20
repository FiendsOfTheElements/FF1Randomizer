﻿using RomUtilities;
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
		public void Spooky(TalkRoutines talkroutines, NpcObjectData npcdata, DialogueData dialogues, ZoneFormations zoneformations, StandardMaps maps, MT19337 rng, Flags flags)
		{
			
			if(!flags.SpookyFlag || (bool)flags.RandomizeFormationEnemizer)
			{
				return;
			}

			const byte FiendsEncounter = 0x77;

			byte encLich1 = 0x7A;
			const byte encLich2 = 0x73;

			const byte bossVamp = 0xFC;
			const byte bossZombie = 0x04;
			const byte bossGhost = 0x46;
			const byte bossGeist = 0x0F;
			const byte bossZomBull = 0xB2;
			const byte bossZombieD = 0xCB;
			const byte bossDracolich = 0x58;
			const byte bossSentinel = 0xFD;
			byte bossLichMech = 0x7A;

			const byte encVampire = 0x7C;
			const byte encChaos = 0x7B;
			const byte encZombieGhoul = 0x04;
			const byte encGhoulGeist = 0x08;
			const byte encSpecGeist = 0x0F;
			const byte encPhantomGhost = 0x46;
			const byte encAstos = 0x7D;
			const byte encIronGol = 0x58;
			const byte encZombullTroll = 0x32;
			const byte encZombieD = 0x4B;
			const byte encWarMech = 0x56;

			var zombieDialog = new List<byte> { 0x32, 0x33, 0x34 };
			if (!flags.NoOverworld)
			{
				zombieDialog.Add(0x36);
			}

			Dictionary<int, string> evilDialogs = new Dictionary<int, string>();

			var encountersData = new Encounters(this);

			for (int i = 0; i < 4; i++)
			{
				if (encountersData.formations[FiendsEncounter + i].enemy1 == 0x77)
					encLich1 = (byte)(FiendsEncounter + i);
			}

			bossLichMech = encLich1;

			// Phantom is Lich, and put Lich1 as Lich2 b-side
			encountersData.formations[encLich2].pattern = FormationPattern.Mixed;
			encountersData.formations[encLich2].spriteSheet = FormationSpriteSheet.ImageGeistWormEye;
			encountersData.formations[encLich2].enemy2 = 0x77;
			encountersData.formations[encLich2].gfxOffset1 = (int)FormationGFX.Sprite4;
			encountersData.formations[encLich2].gfxOffset2 = (int)FormationGFX.Sprite4;
			encountersData.formations[encLich2].palette1 = 0x16;
			encountersData.formations[encLich2].paletteAssign1 = 0;
			encountersData.formations[encLich2].paletteAssign2 = 0;
			encountersData.formations[encLich2].minmax1 = (1, 1);
			encountersData.formations[encLich2].minmax2 = (0, 0);
			encountersData.formations[encLich2].minmaxB1 = (0, 0);
			encountersData.formations[encLich2].minmaxB2 = (1, 1);
			encountersData.formations[encLich2].unrunnableB = true;

			// Add WzVamp to Vampire encounter
			encountersData.formations[encVampire].enemy1 = 0x3D;
			encountersData.formations[encVampire].enemy2 = 0x3C;
			encountersData.formations[encVampire].minmax1 = (1, 1);
			encountersData.formations[encVampire].minmax2 = (0, 0);
			encountersData.formations[encVampire].palette1 = 0x20;
			encountersData.formations[encVampire].palette2 = 0x1F;
			encountersData.formations[encVampire].paletteAssign1 = 0;
			encountersData.formations[encVampire].paletteAssign2 = 1;
			encountersData.formations[encVampire].minmaxB1 = (0, 0);
			encountersData.formations[encVampire].minmaxB2 = (1, 1);
			encountersData.formations[encVampire].unrunnableA = true;
			encountersData.formations[encVampire].unrunnableB = true;

			// Add Sentinel boss (w WarMech sprite) to Astos encounter
			encountersData.formations[encAstos].enemy2 = 0x60;
			encountersData.formations[encAstos].pattern = FormationPattern.Mixed;
			encountersData.formations[encAstos].gfxOffset2 = (int)FormationGFX.Sprite4;
			encountersData.formations[encAstos].palette2 = 0x2F;
			encountersData.formations[encAstos].paletteAssign2 = 1;
			encountersData.formations[encAstos].minmaxB1 = (0, 0);
			encountersData.formations[encAstos].minmaxB2 = (1, 1);
			encountersData.formations[encAstos].unrunnableB = true;

			// Create new zombie encounters to make space, make Geist/Zombie bosses
			encountersData.formations[encZombieGhoul].minmax1 = (1, 1);
			encountersData.formations[encZombieGhoul].minmax2 = (0, 0);
			encountersData.formations[encZombieGhoul].unrunnableA = true;

			encountersData.formations[encGhoulGeist].minmax1 = (0, 2);
			encountersData.formations[encGhoulGeist].minmax2 = (0, 0);
			encountersData.formations[encGhoulGeist].minmax3 = (1, 3);
			encountersData.formations[encGhoulGeist].enemy3 = 0x2B;
			encountersData.formations[encGhoulGeist].gfxOffset3 = (int)FormationGFX.Sprite2;
			encountersData.formations[encGhoulGeist].paletteAssign3 = 1;
			encountersData.formations[encGhoulGeist].minmaxB1 = (0, 3);
			encountersData.formations[encGhoulGeist].minmaxB2 = (1, 4);

			encountersData.formations[encSpecGeist].minmax1 = (0, 0);
			encountersData.formations[encSpecGeist].minmax2 = (1, 1);
			encountersData.formations[encSpecGeist].unrunnableA = true;

			// Replace Phantom with Ghost boss
			encountersData.formations[encPhantomGhost].minmax1 = (1, 1);
			encountersData.formations[encPhantomGhost].minmax2 = (0, 0);
			encountersData.formations[encPhantomGhost].unrunnableA = true;

			// Modify zomBull encounter for zomBull boss
			encountersData.formations[encZombullTroll].minmax1 = (1, 4);
			encountersData.formations[encZombullTroll].minmax2 = (0, 0);
			encountersData.formations[encZombullTroll].minmaxB1 = (1, 1);
			encountersData.formations[encZombullTroll].minmaxB2 = (0, 0);
			encountersData.formations[encZombullTroll].unrunnableB = true;

			// Modify zombieD encounter for zombieD boss
			encountersData.formations[encZombieD].minmax1 = (2, 4);
			encountersData.formations[encZombieD].minmaxB1 = (1, 1);
			encountersData.formations[encZombieD].unrunnableB = true;

			// Modify ironGol encounter for Dracolich (Phantom) boss
			encountersData.formations[encIronGol].enemy2 = Enemy.Phantom;
			encountersData.formations[encIronGol].gfxOffset2 = (int)FormationGFX.Sprite3;
			encountersData.formations[encIronGol].palette2 = 0x16;
			encountersData.formations[encIronGol].paletteAssign2 = 1;
			encountersData.formations[encIronGol].minmax1 = (0, 0);
			encountersData.formations[encIronGol].minmax2 = (1, 1);
			encountersData.formations[encIronGol].unrunnableA = true;

			// Modify Lich1 encounter for Lich? (WarMech) boss
			encountersData.formations[encLich1].pattern = FormationPattern.Fiends;
			encountersData.formations[encLich1].spriteSheet = FormationSpriteSheet.KaryLich;
			encountersData.formations[encLich1].gfxOffset1 = (int)FormationGFX.Sprite3;
			encountersData.formations[encLich1].enemy1 = Enemy.WarMech;
			encountersData.formations[encLich1].minmax1 = (1, 1);
			encountersData.formations[encLich1].palette1 = 0x07;
			encountersData.formations[encLich1].palette2 = 0x07;
			encountersData.formations[encLich1].unrunnableA = true;

			// Lich is Chaos
			encountersData.formations[encChaos].pattern = FormationPattern.Fiends;
			encountersData.formations[encChaos].spriteSheet = FormationSpriteSheet.KaryLich;
			encountersData.formations[encChaos].gfxOffset1 = 0x01;
			encountersData.formations[encChaos].gfxOffset2 = 0x00;
			encountersData.formations[encChaos].palette1 = 0x36;
			encountersData.formations[encChaos].palette2 = 0x37;

			encountersData.Write(this);

			// Update Phantom trap tile
			if (Data[0x0FAD] == encPhantomGhost)
				Data[0x0FAD] = bossDracolich;

			// Switch WarMechEncounter B formation to not get it in Sky
			zoneformations.ReplaceEncounter(bossZombie, encGhoulGeist);
			zoneformations.ReplaceEncounter(bossGeist, encGhoulGeist + 0x80);
			zoneformations.ReplaceEncounter(bossZombieD, encZombieD);
			zoneformations.ReplaceEncounter(bossZomBull, encZombullTroll);
			zoneformations.ReplaceEncounter(bossGhost, bossDracolich);
			zoneformations.ReplaceEncounter(encWarMech, bossLichMech);
			zoneformations.ReplaceEncounter(bossDracolich, encIronGol + 0x80);

			// Make Chaos and WarMech Undead, Phantom a Dragon
			var statsEnemies = Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);
			statsEnemies[0x7F][0x10] |= 0x08; // Chaos
			statsEnemies[0x76][0x10] |= 0x08; // WarMech
			statsEnemies[0x33][0x10] |= 0x02; // Phantom
			Put(EnemyOffset, statsEnemies.SelectMany(enemy => enemy.ToBytes()).ToArray());

			//Update enemies names

			var enemyText = ReadEnemyText();

			enemyText[0x33] = "DRACLICH"; //Phantom > to DrakLich?
			enemyText[118] = "LICH?"; // WarMech > Lich?
			enemyText[119] = "PHANTOM"; // Lich1 > Phantom
			enemyText[120] = "PHANTOM"; // Lich2 > Phantom
			enemyText[127] = "LICH"; // Chaos > Lich

			WriteEnemyText(enemyText);

			// Scale Undeads
			int evadeCap = GetEvadeIntFromFlag(flags.EvadeCap);
			ScaleSingleEnemyStats(0x15, 125, 125, false, null, false, 125, 125, evadeCap); // Bone
			ScaleSingleEnemyStats(0x16, 125, 125, false, null, false, 125, 125, evadeCap); // R.Bone
			ScaleSingleEnemyStats(0x24, 125, 125, false, null, false, 125, 125, evadeCap); // ZomBull
			ScaleSingleEnemyStats(0x27, 125, 125, false, null, false, 125, 125, evadeCap); // Shadow
			ScaleSingleEnemyStats(0x28, 125, 125, false, null, false, 125, 125, evadeCap); // Image
			ScaleSingleEnemyStats(0x29, 125, 125, false, null, false, 125, 125, evadeCap); // Wraith
			ScaleSingleEnemyStats(0x2A, 125, 125, false, null, false, 125, 125, evadeCap); // Ghost
			ScaleSingleEnemyStats(0x2B, 125, 125, false, null, false, 125, 125, evadeCap); // Zombie
			ScaleSingleEnemyStats(0x2C, 125, 125, false, null, false, 125, 125, evadeCap); // Ghoul
			ScaleSingleEnemyStats(0x2D, 125, 125, false, null, false, 125, 125, evadeCap); // Geist
			ScaleSingleEnemyStats(0x2E, 125, 125, false, null, false, 125, 125, evadeCap); // Specter
			ScaleSingleEnemyStats(0x33, 125, 125, false, null, false, 125, 125, evadeCap); // Phantom
			ScaleSingleEnemyStats(0x3C, 125, 125, false, null, false, 125, 125, evadeCap); // Vampire
			ScaleSingleEnemyStats(0x3D, 125, 125, false, null, false, 125, 125, evadeCap); // WzVampire
			ScaleSingleEnemyStats(0x44, 125, 125, false, null, false, 125, 125, evadeCap); // Zombie D
			ScaleSingleEnemyStats(0x4F, 125, 125, false, null, false, 125, 125, evadeCap); // Mummy
			ScaleSingleEnemyStats(0x50, 125, 125, false, null, false, 125, 125, evadeCap); // WzMummy
			ScaleSingleEnemyStats(0x77, 120, 120, false, null, false, 120, 120, evadeCap); // Lich1
			ScaleSingleEnemyStats(0x78, 120, 120, false, null, false, 120, 120, evadeCap); // Lich2
			ScaleSingleEnemyStats(0x7F, 110, 110, false, null, false, 110, 110, evadeCap); // Chaos

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
				"I MAIM AND KILL", "",
				"I AM A BEAST OF BRUTAL WILL", "",
				"I AM DEATH..", "", "",
				"I. AM. LICH."
			});

			Console.WriteLine(intro.Length);
			System.Diagnostics.Debug.Assert(intro.Length <= 208);
			Put(0x37F20, intro);

			var validTalk = new List<TalkScripts> { TalkScripts.Talk_norm, TalkScripts.Talk_GoBridge, TalkScripts.Talk_ifearthfire, TalkScripts.Talk_ifearthvamp, TalkScripts.Talk_ifevent, TalkScripts.Talk_ifitem, TalkScripts.Talk_ifkeytnt, TalkScripts.Talk_ifvis, TalkScripts.Talk_Invis, TalkScripts.Talk_4Orb, TalkScripts.Talk_kill };
			var invalidZombie = new List<ObjectId> { ObjectId.Bat, ObjectId.GaiaBroom, ObjectId.MatoyaBroom1, ObjectId.MatoyaBroom2, ObjectId.MatoyaBroom3, ObjectId.MatoyaBroom4, ObjectId.MirageRobot1, ObjectId.MirageRobot2, ObjectId.MirageRobot3, ObjectId.SkyRobot, ObjectId.LutePlate, ObjectId.RodPlate, ObjectId.SkyWarrior1, ObjectId.SkyWarrior2, ObjectId.SkyWarrior3, ObjectId.SkyWarrior4, ObjectId.SkyWarrior5, (ObjectId)0x18, (ObjectId)0x19, (ObjectId)0x1A };
			var validZombie = new List<ObjectId>();

			if ((bool)flags.HintsVillage)
			{
				invalidZombie.AddRange(new List<ObjectId> { ObjectId.ConeriaOldMan, ObjectId.PravokaOldMan, ObjectId.ElflandScholar1, ObjectId.MelmondOldMan2, ObjectId.CrescentSage11, ObjectId.OnracOldMan2, ObjectId.GaiaWitch, ObjectId.LefeinMan12 });
			}

			// Change base NPCs' scripts to Talk_fight
			for (int i = 0; i < 0xD0; i++)
			{
				if (validTalk.Contains(npcdata[(ObjectId)i].Script) && !(invalidZombie.Contains((ObjectId)i)))
				{
					npcdata[(ObjectId)i].Script = TalkScripts.Talk_fight;
					if ((i >= 0x85 && i <= 0x90) || i == 0x9B)
					{
						npcdata[(ObjectId)i].Battle = bossZombieD;
					}
					else
					{
						npcdata[(ObjectId)i].Battle = bossZombie;
					}

					npcdata[(ObjectId)i].Dialogue2 = zombieDialog.PickRandom(rng);
					validZombie.Add((ObjectId)i);
				}
			}

			// New routines to fight and give item
			//var battleUnne = talkroutines.Add(Blob.FromHex("A674F005BD2060F01AE67DA572203D96A5752020B1A476207F902073922018964C4396A57060"));
			//var battleGiveOnFlag = talkroutines.Add(Blob.FromHex("A474F0052079909029A57385612080B1B022E67DA572203D96A5752020B1A476207F90207392A5611820109F2018964C4396A57060"));
			//var battleGiveOnItem = talkroutines.Add(Blob.FromHex("A674F005BD2060F029A57385612080B1B022E67DA572203D96A5752020B1A476207F90207392A5611820109F2018964C4396A57060"));
			//var battleBahamut = talkroutines.Add(Blob.FromHex("AD2D60D003A57160E67DA572203D96A5752020B1A476207F9020739220AE952018964C439660"));
			talkroutines.ReplaceChunk(TalkScripts.Talk_Bikke, Blob.FromHex("A57260A57060"), Blob.FromHex("207392A57260"));

			//var lichReplace = talkroutines.Add(Blob.FromHex("A572203D96A5752020B1A476207F90207392A47320A4902018964C4396"));

			// Update Garland's script
			npcdata[ObjectId.Garland].Script = TalkScripts.Talk_CoOGuy;

			// Change dialogues
			evilDialogs.Add(0x32, "Braaaaain!");
			evilDialogs.Add(0x33, "Barf!");
			evilDialogs.Add(0x34, "Uaaaaaargh!");
			if (!flags.NoOverworld)
			{
				evilDialogs.Add(0x36, "Groaaarn!");
			}
			

			evilDialogs.Add(0x04, "What the hell!?\nThat princess is crazy,\nshe tried to bite me!\n\nThat's it. Screw that.\nI'm going home.");

			evilDialogs.Add(0x02, "What is going on!? My\nguard tried to kill me!\nUgh.. this is a deep\nwound.. I don't feel so\nwell..\nGwooorrrgl!\n\nReceived #");

			npcdata[ObjectId.King].Battle = bossZombie;
			npcdata[ObjectId.King].Script = TalkScripts.Spooky_GiveOnFlag;

			evilDialogs.Add(0x06, "So, you are.. the..\nLIGHTarrgaar..\nWarglb..\n\nBraaaain..\n\nReceived #");
			npcdata[ObjectId.Princess2].Battle = bossZombie;
			npcdata[ObjectId.Princess2].Script = TalkScripts.Spooky_GiveOnItem;

			evilDialogs.Add(0x08, "Aaaaarrr! The LIGHT\nWARRIORS have been\ncursed too!\n\nGet 'em, boys!");
			evilDialogs.Add(0x09, "Okay then, guess I'll go\nto the pub, have a nice\ncold pint, and wait for\nall this to blow over.\n\nReceived #");

			evilDialogs.Add(0x0E, "At last I wake up from\nmy eternal slumber.\nCome, LIGHT WARRIORS,\nembrace the darkness,\njoin me in death..\n\nReceived #");
			npcdata[ObjectId.ElfPrince].Battle = bossVamp;
			npcdata[ObjectId.ElfPrince].Script = TalkScripts.Spooky_GiveOnFlag;

			evilDialogs.Add(0x0C, "Yes, yes, the master\nwill be pleased. Let's\nclean this place up\nbefore he wakes.\nStarting with you!");
			npcdata[ObjectId.ElfDoc].Battle = bossGeist;
			npcdata[ObjectId.ElfDoc].Script = TalkScripts.Spooky_Unne;

			if (npcdata[ObjectId.Astos].Script != TalkScripts.Talk_Astos)
			{
				evilDialogs.Add(0x12, "Did you ever dance with\nthe devil in the pale\nmoonlight?\n\nReceived #");
				npcdata[ObjectId.Astos].Battle = bossVamp;
				npcdata[ObjectId.Astos].Script = TalkScripts.Spooky_GiveOnItem;
			}

			evilDialogs.Add(0x13, "The world is going to\nhell, but this won't\nstop me from digging\nmy canal!");
			evilDialogs.Add(0x14, "Excellent! Finally,\nnow Lich's undead army\ncan flow through the\nrest of the world!\n\nReceived #");
			npcdata[ObjectId.Nerrick].Battle = bossVamp;
			npcdata[ObjectId.Nerrick].Script = TalkScripts.Spooky_GiveOnItem;

			evilDialogs.Add(0x15, "I never thought I'd\nhave to forge the\nweapon that would slay\nmy brothers. Bring me\nADAMANT, quick!");
			evilDialogs.Add(0x16, "You were too slow,\nLIGHT WARRIORS. You have\nforsaken me!\nJoin my damned soul in\nthe afterworld!\n\nReceived #");
			npcdata[ObjectId.Smith].Battle = bossGhost;
			npcdata[ObjectId.Smith].Script = TalkScripts.Spooky_GiveOnItem;

			evilDialogs.Add(0x17, "Pfah! Everyone else can\nrot in Hell for all\nI care, I'm  perfectly\nsafe here!");
			evilDialogs.Add(0x19, "SCREEEEEEEEEEEEEEEEEEEEE!\n\nReceived #");
			npcdata[ObjectId.Matoya].Battle = bossGeist;
			npcdata[ObjectId.Matoya].Script = TalkScripts.Spooky_GiveOnItem;

			evilDialogs.Add(0x1C, "Now, listen to me, a\nbasic word from\nLeifeinish is Lu..\nHack! Cough! Sorry,\nLu..lu..paaaargh!");
			npcdata[ObjectId.Unne].Battle = bossGeist;
			npcdata[ObjectId.Unne].Script = TalkScripts.Spooky_Unne;

			evilDialogs.Add(0x1D, "Ah, humans who wish to\npay me tribute. What?\nYou miserable little\npile of secrets!\nEnough talk! Have at you!");

			evilDialogs.Add(0x1E, "I.. HUNGER!\n\nReceived #");
			npcdata[ObjectId.Sarda].Battle = bossZomBull;
			npcdata[ObjectId.Sarda].Script = TalkScripts.Spooky_GiveOnFlag;

			evilDialogs.Add(0x20, "The TAIL! Impressive..\nYes, yes, you are indeed\nworthy..\n\nWorthy of dying by my\nown claws!");
			npcdata[ObjectId.Bahamut].Battle = bossDracolich;
			npcdata[ObjectId.Bahamut].Item = 0x1F;
			npcdata[ObjectId.Bahamut].Script = TalkScripts.Spooky_Bahamut;

			evilDialogs.Add(0x23, "Come play with me,\nLIGHT WARRIORS.\nFor ever and ever\nand ever..\n\nReceived #");
			npcdata[ObjectId.Fairy].Battle = bossGhost;
			npcdata[ObjectId.Fairy].Script = TalkScripts.Spooky_GiveOnItem;

			evilDialogs.Add(0x27, "Exterminate.\n\n\n\n\nReceived #");
			npcdata[ObjectId.CubeBot].Battle = bossSentinel;
			npcdata[ObjectId.CubeBot].Script = TalkScripts.Spooky_GiveOnItem;

			evilDialogs.Add(0x2B, "My friends..\nMy colleagues..\nNow.. I join them..\n\nReceived #");
			npcdata[ObjectId.CanoeSage].Battle = bossZomBull;
			npcdata[ObjectId.CanoeSage].Script = TalkScripts.Spooky_GiveOnItem;

			evilDialogs.Add(0xCD, "Luuuuu.. paaaargh!\n\n\n\nReceived #");
			npcdata[ObjectId.Lefein].Battle = bossZomBull;
			npcdata[ObjectId.Lefein].Script = TalkScripts.Spooky_GiveOnFlag;

			evilDialogs.Add(0xFA, "Sorry, LIGHT WARRIORS,\nbut your LICH is in\nanother castle!\n\nMwahahahaha!");

			if (!(bool)flags.TrappedChaos)
			{
				// Add new Chaos dialogues
				evilDialogs.Add(0x2F, "You did well fighting\nmy Army of Darkness,\nLIGHT WARRIORS! But it\nis for naught!\nI am UNSTOPPABLE!\nThis time, YOU are\nthe SPEEDBUMP!");
				evilDialogs.Add(0x30, "HAHA! Alright, enough\nplaying around.");

				// Update Chaos NPC
				Put(0x2F00 + 0x18, Blob.FromHex("00"));
				Put(0x2F00 + 0x19, Blob.FromHex("01"));
				Put(0x2F00 + 0x1A, Blob.FromHex("00"));

				// Update Chaos' Sprite
				npcdata[ObjectId.Chaos2].Sprite = ObjectSprites.King;
				npcdata[ObjectId.Chaos3].Sprite = ObjectSprites.King;

				// Update Chaos' Palette
				PutInBank(0x00, 0xA000 + ((byte)MapIndex.TempleOfFiendsRevisitedChaos * 0x30) + 0x18, Blob.FromHex("0F0F13300F0F1530"));

				// Add Lich? fight
				npcdata[ObjectId.Chaos2].Dialogue1 = 0x2F;
				npcdata[ObjectId.Chaos2].Dialogue3 = 0x2F;
				npcdata[ObjectId.Chaos2].Item = 0x1A;
				npcdata[ObjectId.Chaos2].Battle = bossLichMech;
				npcdata[ObjectId.Chaos2].Script = TalkScripts.Spooky_Lich;
			}

			dialogues.InsertDialogues(evilDialogs);

			for (int i = 0; i < 4; i++)
			{
				if (npcdata[(ObjectId)(0x1B + i)].Battle == encLich1)
					npcdata[(ObjectId)(0x1B + i)].Battle = encLich2 + 0x80;
			}
			npcdata[ObjectId.WarMECH].Battle = bossLichMech;

			// Switch princess
			maps[MapIndex.TempleOfFiends].MapObjects[1].ObjectId = ObjectId.Princess2;
			maps[MapIndex.ConeriaCastle2F].MapObjects[1].ObjectId = ObjectId.None;
			if (npcdata[ObjectId.King].Requirement != 00)
			{
				npcdata[ObjectId.King].Requirement = (int)ObjectId.Princess2;
			}
			
			Put(0x2F00 + 0x12, Blob.FromHex("01"));
			Put(0x2F00 + 0x03, Blob.FromHex("02"));

			// Change NPC's color
			Data[0x2000 + ((byte)MapIndex.ConeriaTown * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapIndex.ConeriaTown * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapIndex.ConeriaCastle1F * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapIndex.ConeriaCastle1F * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapIndex.ConeriaCastle2F * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapIndex.ConeriaCastle2F * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapIndex.Pravoka * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapIndex.Pravoka * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapIndex.Elfland * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapIndex.Elfland * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapIndex.ElflandCastle * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapIndex.ElflandCastle * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapIndex.DwarfCave * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapIndex.DwarfCave * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapIndex.Melmond * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapIndex.Melmond * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapIndex.CrescentLake * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapIndex.CrescentLake * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapIndex.Gaia * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapIndex.Gaia * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapIndex.Onrac * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapIndex.Onrac * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapIndex.SeaShrineB1 * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapIndex.SeaShrineB1 * 0x30) + 0x18 + 0x07] = 0x3A;

			Data[0x2000 + ((byte)MapIndex.Lefein * 0x30) + 0x18 + 0x03] = 0x3A;
			Data[0x2000 + ((byte)MapIndex.Lefein * 0x30) + 0x18 + 0x07] = 0x3A;

			// Let zombies roam free
			var npcMap = new List<MapIndex> { MapIndex.Cardia, MapIndex.BahamutCaveB2, MapIndex.ConeriaTown, MapIndex.ConeriaCastle1F, MapIndex.ConeriaCastle2F, MapIndex.CrescentLake, MapIndex.DwarfCave, MapIndex.Elfland, MapIndex.ElflandCastle, MapIndex.Gaia, MapIndex.Lefein, MapIndex.Melmond, MapIndex.Onrac, MapIndex.Pravoka };

			foreach (var map in npcMap)
			{
				for (var i = 0; i < 0x10; i++)
				{
					if (validZombie.Contains(maps[map].MapObjects[i].ObjectId))
					{
						maps[map].MapObjects[i].Stationary = false;
					}
				}
			}
		}
	}
}
