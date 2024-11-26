using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public enum BlackOrbFiends
	{
		None = 0,
		Lamorte,
		Kismet,
		Kalamia,
		Infernar,
		Marine,
		Angela,
		Browler,
		Whisper
	}
	public partial class ExtAltFiends
	{
		public List<AlternateFiends> BlackOrbAltFiends = new()
		{
			new AlternateFiends
			{
				Name = "LAMORTE",
				SpriteSheet = FormationSpriteSheet.KaryLich,
				FormationPattern = FormationPattern.Fiends,
				GFXOffset = FormationGFX.Sprite1,
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x60,
				Spells1 = new List<byte> { (byte)SpellByte.ICE2, (byte)SpellByte.SLP2, (byte)SpellByte.ICE2, (byte)SpellByte.SLEP, (byte)SpellByte.ICE2, (byte)SpellByte.SLP2, (byte)SpellByte.ICE2, (byte)SpellByte.SLEP, },
				SkillChance1 = 0x00,
				Skills1 = Enumerable.Repeat((byte)EnemySkills.None, 4).ToList(),
				SpellChance2 = 0x60,
				Spells2 = new List<byte> { (byte)SpellByte.NUKE, (byte)SpellByte.ICE3, (byte)SpellByte.BRAK, (byte)SpellByte.ICE3, (byte)SpellByte.BRAK, (byte)SpellByte.NUKE, (byte)SpellByte.ICE3, (byte)SpellByte.BRAK },
				SkillChance2 = 0x00,
				Skills2 = Enumerable.Repeat((byte)EnemySkills.None, 4).ToList(),
			},
			new AlternateFiends
			{
				Name = "KISMET",
				SpriteSheet = FormationSpriteSheet.KaryLich,
				FormationPattern = FormationPattern.Fiends,
				GFXOffset = FormationGFX.Sprite1,
				ElementalWeakness = SpellElement.Time,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x30,
				Spells1 = new List<byte> { (byte)SpellByte.FAST, (byte)SpellByte.SLOW, (byte)SpellByte.SLOW, (byte)SpellByte.SLOW, (byte)SpellByte.FAST, (byte)SpellByte.SLOW, (byte)SpellByte.SLOW, (byte)SpellByte.SLOW, },
				SkillChance1 = 0x00,
				Skills1 = Enumerable.Repeat((byte)EnemySkills.None, 4).ToList(),
				SpellChance2 = 0x30,
				Spells2 = new List<byte> { (byte)SpellByte.FAST, (byte)SpellByte.LOK2, (byte)SpellByte.INVS, (byte)SpellByte.LOK2, (byte)SpellByte.FAST, (byte)SpellByte.LOK2, (byte)SpellByte.INVS, (byte)SpellByte.LOK2 },
				SkillChance2 = 0x00,
				Skills2 = Enumerable.Repeat((byte)EnemySkills.None, 4).ToList(),
			},
			new AlternateFiends
			{
				Name = "KALAMIA",
				SpriteSheet = FormationSpriteSheet.KaryLich,
				FormationPattern = FormationPattern.Fiends,
				GFXOffset = FormationGFX.Sprite1,
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x30,
				Spells1 = new List<byte> { (byte)SpellByte.MUTE, (byte)SpellByte.TMPR, (byte)SpellByte.MUTE, (byte)SpellByte.TMPR, (byte)SpellByte.MUTE, (byte)SpellByte.TMPR, (byte)SpellByte.MUTE, (byte)SpellByte.TMPR, },
				SkillChance1 = 0x00,
				Skills1 = Enumerable.Repeat((byte)EnemySkills.None, 4).ToList(),
				SpellChance2 = 0x30,
				Spells2 = new List<byte> { (byte)SpellByte.MUTE, (byte)SpellByte.XFER, (byte)SpellByte.SABR, (byte)SpellByte.MUTE, (byte)SpellByte.SABR, (byte)SpellByte.MUTE, (byte)SpellByte.XFER, (byte)SpellByte.SABR },
				SkillChance2 = 0x00,
				Skills2 = Enumerable.Repeat((byte)EnemySkills.None, 4).ToList(),
			},
			new AlternateFiends
			{
				Name = "INFERNAR",
				SpriteSheet = FormationSpriteSheet.KaryLich,
				FormationPattern = FormationPattern.Fiends,
				GFXOffset = FormationGFX.Sprite1,
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = Enumerable.Repeat((byte)SpellByte.FIR2, 8).ToList(),
				SkillChance1 = 0x00,
				Skills1 = Enumerable.Repeat((byte)EnemySkills.None, 4).ToList(),
				SpellChance2 = 0x40,
				Spells2 = Enumerable.Repeat((byte)SpellByte.FIR3, 8).ToList(),
				SkillChance2 = 0x00,
				Skills2 = Enumerable.Repeat((byte)EnemySkills.None, 4).ToList(),
			},
			new AlternateFiends
			{
				Name = "MARINE",
				SpriteSheet = FormationSpriteSheet.KrakenTiamat,
				FormationPattern = FormationPattern.Fiends,
				GFXOffset = FormationGFX.Sprite1,
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.AQUATIC,
				SpellChance1 = 0x00,
				Spells1 = Enumerable.Repeat((byte)SpellByte.NONE, 8).ToList(),
				SkillChance1 = 0x20,
				Skills1 = Enumerable.Repeat((byte)EnemySkills.Stinger, 4).ToList(),
				SpellChance2 = 0x40,
				Spells2 = Enumerable.Repeat((byte)SpellByte.ICE2, 8).ToList(),
				SkillChance2 = 0x20,
				Skills2 = Enumerable.Repeat((byte)EnemySkills.Stinger, 4).ToList(),
			},
			new AlternateFiends
			{
				Name = "ANGELA",
				SpriteSheet = FormationSpriteSheet.KrakenTiamat,
				FormationPattern = FormationPattern.Fiends,
				GFXOffset = FormationGFX.Sprite1,
				ElementalWeakness = SpellElement.Death,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x00,
				Spells1 = Enumerable.Repeat((byte)SpellByte.NONE, 8).ToList(),
				SkillChance1 = 0x30,
				Skills1 = Enumerable.Repeat((byte)EnemySkills.Frost, 4).ToList(),
				SpellChance2 = 0x30,
				Spells2 = new List<byte> { (byte)SpellByte.FADE }.Concat(Enumerable.Repeat((byte)SpellByte.CUR3, 7).ToList()).ToList(),
				SkillChance2 = 0x30,
				Skills2 = Enumerable.Repeat((byte)EnemySkills.Frost, 4).ToList(),
			},
			new AlternateFiends
			{
				Name = "BROWLER",
				SpriteSheet = FormationSpriteSheet.KrakenTiamat,
				FormationPattern = FormationPattern.Fiends,
				GFXOffset = FormationGFX.Sprite1,
				ElementalWeakness = SpellElement.Earth,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x00,
				Spells1 = Enumerable.Repeat((byte)SpellByte.NONE, 8).ToList(),
				SkillChance1 = 0x30,
				Skills1 = Enumerable.Repeat((byte)EnemySkills.Dazzle, 4).ToList(),
				SpellChance2 = 0x30,
				Spells2 = new List<byte> { (byte)SpellByte.QAKE, (byte)SpellByte.FOG, (byte)SpellByte.QAKE, (byte)SpellByte.WALL, (byte)SpellByte.QAKE, (byte)SpellByte.FOG, (byte)SpellByte.QAKE, (byte)SpellByte.WALL },
				SkillChance2 = 0x30,
				Skills2 = new List<byte> { (byte)EnemySkills.Dazzle, (byte)EnemySkills.Trance, (byte)EnemySkills.Dazzle, (byte)EnemySkills.Trance },
			},
			new AlternateFiends
			{
				Name = "WHISPER",
				SpriteSheet = FormationSpriteSheet.KrakenTiamat,
				FormationPattern = FormationPattern.Fiends,
				GFXOffset = FormationGFX.Sprite1,
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x00,
				Spells1 = Enumerable.Repeat((byte)SpellByte.NONE, 8).ToList(),
				SkillChance1 = 0x20,
				Skills1 = new List<byte> { (byte)EnemySkills.Stare, (byte)EnemySkills.Swirl, (byte)EnemySkills.Stare, (byte)EnemySkills.Swirl },
				SpellChance2 = 0x40,
				Spells2 = new List<byte> { (byte)SpellByte.ZAP, (byte)SpellByte.XXXX, (byte)SpellByte.BANE, (byte)SpellByte.QAKE, (byte)SpellByte.ZAP, (byte)SpellByte.XXXX, (byte)SpellByte.BANE, (byte)SpellByte.QAKE },
				SkillChance2 = 0x20,
				Skills2 = Enumerable.Repeat((byte)EnemySkills.Swirl, 4).ToList(),
			},
		};
		public List<AlternateFiends> PickBlackOrbFiends(List<AlternateFiends> allfiends, MT19337 rng)
		{
			List<ExtAltFiend> extAltFiends = new()
			{
				new ExtAltFiend { Name = "LAMORTE", Domain = FiendDomain.Earth, Fiend = BlackOrbFiends.Lamorte, OrbPalette = Blob.FromHex("0F2A3A300F2A3A30") },
				new ExtAltFiend { Name = "KISMET", Domain = FiendDomain.Earth, Fiend = BlackOrbFiends.Kismet, OrbPalette = Blob.FromHex("0F2131300F213130") },
				new ExtAltFiend { Name = "KALAMIA", Domain = FiendDomain.Volcano, Fiend = BlackOrbFiends.Kalamia, OrbPalette = Blob.FromHex("0F1626360F162636") },
				new ExtAltFiend { Name = "INFERNAR", Domain = FiendDomain.Volcano, Fiend = BlackOrbFiends.Infernar, OrbPalette = Blob.FromHex("0F0628380F062838") },
				new ExtAltFiend { Name = "MARINE", Domain = FiendDomain.Sea, Fiend = BlackOrbFiends.Marine, OrbPalette = Blob.FromHex("0F1121310F112131") },
				new ExtAltFiend { Name = "ANGELA", Domain = FiendDomain.Sea, Fiend = BlackOrbFiends.Angela, OrbPalette = Blob.FromHex("0F0010300F001030") },
				new ExtAltFiend { Name = "BROWLER", Domain = FiendDomain.Sky, Fiend = BlackOrbFiends.Browler, OrbPalette = Blob.FromHex("0F1828380F182838") },
				new ExtAltFiend { Name = "WHISPER", Domain = FiendDomain.Sky, Fiend = BlackOrbFiends.Whisper, OrbPalette = Blob.FromHex("0F2333300F233330") },
			};

			EarthFiend = extAltFiends.Where(f => f.Domain == FiendDomain.Earth).ToList().PickRandom(rng);
			VolcanoFiend = extAltFiends.Where(f => f.Domain == FiendDomain.Volcano).ToList().PickRandom(rng);
			SeaFiend = extAltFiends.Where(f => f.Domain == FiendDomain.Sea).ToList().PickRandom(rng);
			SkyFiend = extAltFiends.Where(f => f.Domain == FiendDomain.Sky).ToList().PickRandom(rng);


			List<AlternateFiends> altFiendsPicked = new()
			{
				allfiends.Find(f => f.Name == EarthFiend.Name),
				allfiends.Find(f => f.Name == VolcanoFiend.Name),
				allfiends.Find(f => f.Name == SeaFiend.Name),
				allfiends.Find(f => f.Name == SkyFiend.Name)
			};

			return altFiendsPicked;
		}

		public void ExtendedFiendsUpdate(NpcObjectData npcdata, DialogueData dialogues, FF1Rom rom, MT19337 rng)
		{
			if (!ExtendedFiends)
			{
				return;
			}

			// Update palettes
			rom.PutInBank(0x00, 0xA000 + ((int)MapIndex.EarthCaveB5 * 0x30) + 0x18, EarthFiend.OrbPalette);
			rom.PutInBank(0x00, 0xA000 + ((int)MapIndex.GurguVolcanoB5 * 0x30) + 0x18, VolcanoFiend.OrbPalette);
			rom.PutInBank(0x00, 0xA000 + ((int)MapIndex.SeaShrineB5 * 0x30) + 0x18, SeaFiend.OrbPalette);
			rom.PutInBank(0x00, 0xA000 + ((int)MapIndex.SkyPalace5F * 0x30) + 0x18, SkyFiend.OrbPalette);

			// Pick friends/foes
			List<(BlackOrbFiends fiend, BlackOrbFiends friend, BlackOrbFiends foe)> friendfoelist = new()
			{
				(BlackOrbFiends.Lamorte, BlackOrbFiends.Angela, BlackOrbFiends.Infernar),
				(BlackOrbFiends.Kismet, BlackOrbFiends.Marine, BlackOrbFiends.Whisper),
				(BlackOrbFiends.Kalamia, BlackOrbFiends.Whisper, BlackOrbFiends.Marine),
				(BlackOrbFiends.Infernar, BlackOrbFiends.Browler, BlackOrbFiends.Lamorte),
				(BlackOrbFiends.Marine, BlackOrbFiends.Kismet, BlackOrbFiends.Kalamia),
				(BlackOrbFiends.Angela, BlackOrbFiends.Lamorte, BlackOrbFiends.Browler),
				(BlackOrbFiends.Browler, BlackOrbFiends.Infernar, BlackOrbFiends.Angela),
				(BlackOrbFiends.Whisper, BlackOrbFiends.Kalamia, BlackOrbFiends.Kismet),
			};

			List<BlackOrbFiends> selectedFiends = new() { EarthFiend.Fiend, VolcanoFiend.Fiend, SeaFiend.Fiend, SkyFiend.Fiend };

			friendfoelist = friendfoelist.Where(f => selectedFiends.Contains(f.fiend)).ToList();

			var fiendswithfriend = friendfoelist.Where(f => selectedFiends.Contains(f.friend)).ToList();
			(BlackOrbFiends fiend, BlackOrbFiends friend) fiendwithfriend = (BlackOrbFiends.None, BlackOrbFiends.None);
			if (fiendswithfriend.Any())
			{
				var friendlypick = fiendswithfriend.PickRandom(rng);
				fiendwithfriend = (friendlypick.fiend, friendlypick.friend);
				friendfoelist = friendfoelist.Where(f => f.fiend != fiendwithfriend.fiend).ToList();
			}

			var fiendswithfoe = friendfoelist.Where(f => selectedFiends.Contains(f.foe)).ToList();
			(BlackOrbFiends fiend, BlackOrbFiends foe) fiendwithfoe = (BlackOrbFiends.None, BlackOrbFiends.None);
			if (fiendswithfoe.Any())
			{
				var foepick = fiendswithfoe.PickRandom(rng);
				fiendwithfoe = (foepick.fiend, foepick.foe);
			}

			List<(BlackOrbFiends fiend, ObjectId npcid)> npcIds = new()
			{
				(BlackOrbFiends.Lamorte, ObjectId.LichOrb),
				(BlackOrbFiends.Kismet, ObjectId.LichOrb),
				(BlackOrbFiends.Kalamia, ObjectId.KaryOrb),
				(BlackOrbFiends.Infernar, ObjectId.KaryOrb),
				(BlackOrbFiends.Marine, ObjectId.KrakenOrb),
				(BlackOrbFiends.Angela, ObjectId.KrakenOrb),
				(BlackOrbFiends.Browler, ObjectId.TiamatOrb),
				(BlackOrbFiends.Whisper, ObjectId.TiamatOrb),
			};

			List<(BlackOrbFiends fiend, byte dialogueid)> dialogueIds = new()
			{
				(EarthFiend.Fiend, 0xFA),
				(VolcanoFiend.Fiend, 0xFB),
				(SeaFiend.Fiend, 0xFC),
				(SkyFiend.Fiend, 0xFD),
			};

			List<byte> extraDialogueIds = new() { 0xF8, 0xF9 };

			// Update Fiend Orbs npcs and dialogues
			foreach (var dialogue in dialogueIds)
			{
				ObjectId npcid = npcIds.Find(f => f.fiend == dialogue.fiend).npcid;

				if (fiendwithfriend.fiend == dialogue.fiend)
				{
					// Update npc
					npcdata[npcid].Script = TalkScripts.Talk_FightBranched;
					npcdata[npcid].Requirement = (byte)npcIds.Find(f => f.fiend == fiendwithfriend.friend).npcid;
					var extradialogueid = extraDialogueIds.First();
					npcdata[npcid].Dialogue1 = extradialogueid;
					extraDialogueIds.RemoveAt(0);

					// Pick dialogues
					dialogues[dialogue.dialogueid] = friendDialogues.Find(f => f.fiend == dialogue.fiend).dialogue1;
					dialogues[extradialogueid] = friendDialogues.Find(f => f.fiend == dialogue.fiend).dialogue2;
				}
				else if (fiendwithfoe.fiend == dialogue.fiend)
				{
					// Update npc
					npcdata[npcid].Script = TalkScripts.Talk_FightBranched;
					npcdata[npcid].Requirement = (byte)npcIds.Find(f => f.fiend == fiendwithfoe.foe).npcid;
					var extradialogueid = extraDialogueIds.First();
					npcdata[npcid].Dialogue1 = extraDialogueIds.First();
					extraDialogueIds.RemoveAt(0);

					// Pick dialogues
					dialogues[dialogue.dialogueid] = foeDialogues.Find(f => f.fiend == dialogue.fiend).dialogue1;
					dialogues[extradialogueid] = foeDialogues.Find(f => f.fiend == dialogue.fiend).dialogue2;
				}
				else
				{
					// Pick dialogue
					dialogues[dialogue.dialogueid] = neutralDialogues.Find(f => f.fiend == dialogue.fiend).dialogue;
				}
			}
		}

		List<(BlackOrbFiends fiend, string dialogue)> neutralDialogues = new()
		{
			(BlackOrbFiends.Lamorte, "Oh Light Warriors, stuck\nin your bubble, thinking\nyou can come here and\nsimply defeat\nthe Black Orb. Your\nenlightenment level is\npathetic. Let's rid this\nworld of your stench."),
			(BlackOrbFiends.Kismet, "Ooh, mighty LIGHT\nWarriors, ppplease don't\nhurt me... I'll, I'll let\nyou pass, jjjust spare my\nlife... TTThat's right,\nturn your backs to me.\nAllow my blades to taste\nyour blood!"),
			(BlackOrbFiends.Kalamia, "The Black Orb told me a\nworthy foe would\nchallenge me. Is that\nyou, Light Warriors? I've\nbeen training for this.\nDon't hold anything back,\nbecause I won't."),
			(BlackOrbFiends.Infernar, "I heard about your\nexploits, Light Warriors.\nClearly the Black Orb has\nchoosen me to put a stop\nto you. I won't fail this\ntest, I will prove myself\nto the Black Orb.\nI will rise."),
			(BlackOrbFiends.Marine, "Fuh fuh fuh... Well, what\na surprise, some Light\nWarriors to relieve my\nboredom. I would love to\nchat more, but it's time\nfor you to do like the\nmermaids and get\ncarbonated!"),
			(BlackOrbFiends.Angela, "You think that your aim\nis pure, but the world\nyou're trying to protect\nis filled with suffering\nand pain. The Light Orbs \nwon't fix any of it.\nYou fight for a lie, I\nfight for the Black Orb."),
			(BlackOrbFiends.Browler, "I have no wish to fight\nyou, Light Warriors. You\ncan turn back, go home,\nand enjoy the riches you\nfound along the way, and\nwe'll both be happy. So?\nWahahah! Very well, suit\nyourself."),
			(BlackOrbFiends.Whisper, "What is it? You want to\nlight your puny Orb? Eh?\nJust waltzing in like\nyou own the place!? I'm\nsure the Fiend won't\nmind! Well you made a big\nmistake, bud!! I'LL RIP\nYOUR HEADS OFF!!"),
		};

		List<(BlackOrbFiends fiend, string dialogue1, string dialogue2)> friendDialogues = new()
		{
			(BlackOrbFiends.Lamorte, "I'm glad you came here\nfirst, Light Warriors.\nANGELA wouldn't have been\nas kind as me and give\nyou some needed life\nadvice! Open your mind,\ncome out of your bubble!\nAnd die!", "ANGELA was chosen by the\nBlack Orb, because she\nhad attained the ultimate\nlevel of enLightenment.\nShe's immortal, she'll be\nback. Feeble minds like\nyours can't understand,\nlet's fix this mistake."),
			(BlackOrbFiends.Kismet, "MARINE ttthought that she\nwould get to kill the\nLLLight Warriors, but\nssseems that I won our\nbet. She will never hear\nttthe end of it. Let's\nfinish this so I cccan\ncollect my dddue.", "You dddefeated MARINE?\nHa! What a pppitiful\nattempt at dddeception.\nHey, everyone, we have\nsssome jok, jokers with\nus tttonight! Just\nst, stop! I'll kill you,\nI'll kill you all!"),
			(BlackOrbFiends.Kalamia, "I'm glad you came here\nfirst, Light Warriors.\nWHISPER has been a \ntremendous sparring\npartner, but her temper\nruns too hot for the ends\nof the Black Orb. My\nfists are your demise.", "WHISPER? Defeated? You\nare a cunning lot, Light\nWarriors. Using her\nnatural fury against her\nwas your only path to\nvictory, but this won't\nwork against me.\nHave at you!"),
			(BlackOrbFiends.Infernar, "BROWLER would says this\nis pure chance, but I\nknow that our paths were\ndestined to cross.\nMy valor had to be shown,\nand putting an end to\nyour quest will do just\nthat.", "So, this is how I acquire\nglory, by avenging\nBROWLER. The Black Orb\nhas choosen a sorrowful\npath for me, but I won't\ndoubt it. BROWLER's laugh\nlives eternal, yours is\nforever silenced."),
			(BlackOrbFiends.Marine, "Oh, delightful! KISMET\nwill be so flustered.\nI'll sure enjoy watching\nhim squirm with rage\nwhen he'll learn that\nit is I that has killed\nthe Light Warriors!\nHe's such a kid!", "I have so few pleasures\nin life, and you dare\ndeny me my KISMET? Who\nis the real, heartless\nsea demon here? Maybe\nthe Black Orb should\nhave picked you to take\ncare of his pet Chaos!"),
			(BlackOrbFiends.Angela, "LAMORTE understands the\nlink that binds us to the\nBlack Orb. Life is just\na mess of primitive\ninteractions. You might\nresist us, but you can't\ndestroy the ideals of\nthe Black Orb.", "The Black Orb unite us,\nbut I also shared my own\nlink with LAMORTE. Is\nthis... sadness?\nLight Warriors, I go to\nbattle for the Black Orb\nonly, but I will destroy\nyou for my friend."),
			(BlackOrbFiends.Browler, "Like INFERNAR you think\nthat Chaos should be\ndestroyed, but he\nchampions your side,\nLight Warriors. He stands\nfor humanity when the\nBlack Orb goes against\nit. Don't forget that!", "So you slayed INFERNAR.\nI had hoped is anguish\nwould have been calmed by\ngrowth rather than by\ndeath, but that's not how\nthe coin landed. Let's\nsee how it will land\nfor you, Light Warriors."),
			(BlackOrbFiends.Whisper, "Oh, Light Warriors! You\nwant a fight? I'm ready\nfor you! KALAMIA said\n'You're not ready, you\ngotta train more, sister'\nand I said nuh uh I'm\nready! I'm ready to beat\nyou up to a bloody pulp!!", "The Black Orb said you\nkilled KALAMIA, Light\nWarriors...\nyou...\nyou...\nyouuuUUUAAAAARRRRGH!!!"),
		};

		List<(BlackOrbFiends fiend, string dialogue1, string dialogue2)> foeDialogues = new()
		{
			(BlackOrbFiends.Lamorte, "Why did you come here to\nburden me? You should\nhave gone to INFERNAR.\nLike you he is simple\nminded, you share a\nbubble. Well, maybe it's\nbest low levels such as\nyou don't congregate.", "Maybe I misjudged you,\nLight Warriors. You saw\nhow primitive INFERNAR\nwas and did the world a\nfavor by killing him.\nDoesn't mean the world\nwouldn't be better off\nwithout you."),
			(BlackOrbFiends.Kismet, "Light Warriors! The Black\nOrb foretold your coming,\nmuch lllike it revealed\nhow WHISPER was a fraud.\nThe truth broke his mind,\nnow pure confusion reigns\ninside. MMMe? No effect!\nI'm ppperfectly sane!", "I knew WHISPER bbbecame\nweak after gazing into\nthe Black Orb, bbbut I\ndidn't expect it would be\nso bad as to lose to the\nLight Warriors! One, one\ndelusion down, time to\nend yours tttoo!"),
			(BlackOrbFiends.Kalamia, "MARINE tried to use the\nBlack Orb's power to lure\nyou to her, but the Black\nOrb knew her motives were\nselfish. You get to die\nin an honorable battle,\nLight Warriors. The Black\nOrb will be pleased.", "The Black Orb is all \npowerful, but MARINE's\ndeceit was a disgrace\nto the Fiends. Thanks to\nyou we are stronger. And\nthanks to me the Black\nOrb's design will be\nfulfilled."),
			(BlackOrbFiends.Infernar, "You remind me of LAMORTE,\nLight Warriors. You think\nso little of me that you\nbelieve you can challenge\nme. Like her, you try to\nmask your insecurities\nwith an air of vapid\nsuperiority. How pitiful.", "I smell the stench of\nLAMORTE on you, Light\nWarriors. She was a\ncorrupt mentor, the will\nof the Black Orb is now\nstronger without her.\nAnd the will of the Black\nOrb commands your death."),
			(BlackOrbFiends.Marine, "Hahaha, marvelous.\nKALAMIA will explode with\nfuror. 'Oh, the ultimate\nbattle against the Light\nWarriors you've been \npreparing for your whole\nlife? Oops! I guess\nI squashed them, sorry!'", "I never understood why\nthe Black Orb chose\nKALAMIA. It goes to show\nthat if you're born in\nmediocrity, you'll stay\nin mediocrity. Yes! Of\ncourse I'm talking about\nyou, Light Warriors."),
			(BlackOrbFiends.Angela, "Like us, Chaos serves the\nBlack Orb, but BROWLER\nnever took it seriously.\nYes, the Time Loop\nremoves meaning, but the\nBlack Orb grants it. But\nso, your fate is always\ndeath, Light Warriors.", "You defeated BROWLER,\nLight Warriors, but it is\nno surprise. He failed\nbecause he didn't devote\nhimself to the Black Orb.\nMy faith is unwavering.\nIt guides my spear,\nand pierces your hearts."),
			(BlackOrbFiends.Browler, "I am no slave to the\nBlack Orb. ANGELA's blind\ndevotion will be her\ndownfall, for she cannot\nthink for herself. Me, I\nfight for my own being!\nI am material. Wahaha!\nAnd you, Light Warriors?", "So ANGELA is no more. The\nBlack Orb lost a soldier,\nbut now we can serve it\nas Fiends rather than\nrobots. Wahaha! Thank\nyou, Light Warriors! The\npath to Chaos will\nnow prevail."),
			(BlackOrbFiends.Whisper, "I see you trying to sneak\nby, Light Warriors.\nYou're like that loser, \nKISMET. Always turning\nhis coat, Chaos is the\nsuccessor, oh no the\nBlack Orb is supreme.\nIDIOT! All of you!!", "KISMET is dead? Ugh,\nfinally. He was always\nstirring drama. We don't\nneed intrigues or deep\ndebates, we need to focus\non Fiends fundamentals!\nYeah, that's right! Like\nkilling Light Warriors!"),
		};
	}
}
