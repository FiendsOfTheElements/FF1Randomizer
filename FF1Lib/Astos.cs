using FF1Lib.Helpers;
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
		private void Astos(NpcObjectData npcdata, DialogueData dialogues, TalkRoutines talkroutines, Flags flags, MT19337 rng)
		{
			ShuffleAstos(flags, npcdata, dialogues, talkroutines, rng);

			if ((bool)flags.SwoleAstos)
			{
				EnableSwoleAstos(rng);
			}
		}


		public void EnableSwoleAstos(MT19337 rng)
		{
			EnemyInfo newAstos = new EnemyInfo();
			newAstos.decompressData(Get(EnemyOffset + EnemySize * Enemy.Astos, EnemySize));

			newAstos.morale = 255;
			newAstos.monster_type = (byte)MonsterType.MAGE;
			newAstos.exp = 12800;
			newAstos.gp = 8000;
			newAstos.hp = 850;
			newAstos.num_hits = 2;
			newAstos.damage = 45;
			newAstos.absorb = 60;
			newAstos.mdef = 180;
			newAstos.accuracy = 42;
			newAstos.critrate = 1;
			newAstos.agility = 250;
			newAstos.elem_weakness = (byte)SpellElement.Status | (byte)SpellElement.Death;
			newAstos.elem_resist = (byte)SpellElement.None;

			if (newAstos.AIscript == 0xFF)
			{
				var i = searchForNoSpellNoAbilityEnemyScript();
				if (i == -1) { return; }
				newAstos.AIscript = (byte)i;
			}
			Put(EnemyOffset + EnemySize * Enemy.Astos, newAstos.compressData());

			var astosScript = new EnemyScriptInfo();
			astosScript.decompressData(Get(ScriptOffset + newAstos.AIscript * ScriptSize, ScriptSize));
			astosScript.spell_chance = 96;
			astosScript.skill_chance = 96;

			// use "find spell by effect" to be compatible with spell shuffle and spell crafter.
			var helper = new SpellHelper(this);
			var spells = helper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any, SpellElement.Any, SpellStatus.Mute).ToList();
			spells.AddRange(helper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any, SpellElement.Any, SpellStatus.Stun));
			spells.AddRange(helper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any, SpellElement.Any, SpellStatus.Sleep));
			spells.AddRange(helper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any, SpellElement.Any, SpellStatus.Stone));
			spells.AddRange(helper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any, SpellElement.Any, SpellStatus.Death));

			spells.Shuffle(rng);

			astosScript.spell_list = new byte[8];
			for (int i = 0; i < 8; i++)
			{
				astosScript.spell_list[i] = (byte)(spells[i % spells.Count].Id - Spell.CURE);
			}

			var skills = new List<byte> { (byte)EnemySkills.Poison_Stone, (byte)EnemySkills.Crack, (byte)EnemySkills.Trance, (byte)EnemySkills.Toxic };
			skills.Shuffle(rng);
			astosScript.skill_list = skills.ToArray();
			Put(ScriptOffset + newAstos.AIscript * ScriptSize, astosScript.compressData());
		}

		public void ShuffleAstos(Flags flags, NpcObjectData npcdata, DialogueData dialogues, TalkRoutines talkroutines, MT19337 rng)
		{
			if (!(bool)flags.ShuffleAstos)
			{
				return;
			}

			// NPC pool to swap Astos with
			List<ObjectId> npcpool = new List<ObjectId> { ObjectId.Astos, ObjectId.Bahamut, ObjectId.CanoeSage, ObjectId.CubeBot, ObjectId.ElfDoc,
			ObjectId.Fairy, ObjectId.Matoya, ObjectId.Nerrick, ObjectId.Smith,
			ObjectId.Titan, ObjectId.Unne, ObjectId.Sarda, ObjectId.ElfPrince, ObjectId.Lefein };

			if ((bool)flags.FightBahamut)
			{
				npcpool.Remove(ObjectId.Bahamut);
			}

			if ((bool)flags.UnsafeAstos)
			{
				npcpool.Add(ObjectId.King);
				npcpool.Add(ObjectId.Princess2);
			}

			// Select random npc
			ObjectId newastos = npcpool.PickRandom(rng);

			// If Astos, we're done here
			if (newastos == ObjectId.Astos) return;

			// If not get NPC talk routine, get NPC object
			var talkscript = npcdata[newastos].Script;

			// Switch astos to Talk_GiveItemOnItem;
			npcdata[ObjectId.Astos].Script = TalkScripts.Talk_GiveItemOnItem;

			// Get items name
			//var newastositem = FormattedItemName((Item)npcdata.GetTalkArray(newastos)[(int)TalkArrayPos.item_id]);
			//var nwkingitem = FormattedItemName((Item)npcdata.GetTalkArray(ObjectId.Astos)[(int)TalkArrayPos.item_id]);
			// so why don't we use the item id? it should be set by the time we get the dialogues (since we check if there's inventory space)

			// Custom dialogs for Astos NPC and the Kindly Old King
			List<(byte, string)> astosdialogs = new List<(byte, string)>
			{
				(0x00, ""),
				//(0x02, "You have ruined my plans\nto steal this " + newastositem + "!\nThe princess will see\nthrough my disguise.\nTremble before the might\nof Astos, the Dark King!"),
				(0x02, "You have ruined my plans\nto steal this #!\nThe princess will see\nthrough my disguise.\nTremble before the might\nof Astos, the Dark King!"),
				(0x00, ""),(0x00, ""),(0x00, ""),
				(0x0C, "You found the HERB?\nCurses! The Elf Prince\nmust never awaken.\nOnly then shall I,\nAstos, become\nthe King of ALL Elves!"),
				//(0x0E, "Is this a dream?.. Are\nyou, the LIGHT WARRIORS?\nHA! Thank you for waking\nme! I am actually Astos,\nKing of ALL Elves! You\nwon't take my " + newastositem + "!"),
				(0x0E, "Is this a dream?.. Are\nyou, the LIGHT WARRIORS?\nHA! Thank you for waking\nme! I am actually Astos,\nKing of ALL Elves! You\nwon't take my #!"),
				//(0x12, "My CROWN! Oh, but it\ndoesn't go with this\noutfit at all. You keep\nit. But thanks! Here,\ntake this also!\n\nReceived " + nwkingitem),
				(0x12, "My CROWN! Oh, but it\ndoesn't go with this\noutfit at all. You keep\nit. But thanks! Here,\ntake this also!\n\nReceived #"),
//				(0x14, "Oh, wonderful!\nNice work! Yes, this TNT\nis just what I need to\nblow open the vault.\nSoon more than\nthe " + newastositem + " will\nbelong to Astos,\nKing of Dark Dwarves!"),
				(0x14, "Oh, wonderful!\nNice work! Yes, this TNT\nis just what I need to\nblow open the vault.\nSoon more than\nthe # will\nbelong to Astos,\nKing of Dark Dwarves!"),
				//(0x16, "ADAMANT!! Now let me\nmake this " + newastositem + "..\nAnd now that I have\nthis, you shall take a\nbeating from Astos,\nthe Dark Blacksmith!"),
				(0x16, "ADAMANT!! Now let me\nmake this #..\nAnd now that I have\nthis, you shall take a\nbeating from Astos,\nthe Dark Blacksmith!"),
				//(0x19, "You found my CRYSTAL and\nwant my " + newastositem + "? Oh!\nI can see!! And now, you\nwill see the wrath of\nAstos, the Dark Witch!"),
				(0x19, "You found my CRYSTAL and\nwant my #? Oh!\nI can see!! And now, you\nwill see the wrath of\nAstos, the Dark Witch!"),
				(0x1C, "Finally! With this SLAB,\nI shall conquer Lefein\nand her secrets will\nbelong to Astos,\nthe Dark Scholar!"),
				(0x00, ""),
				//(0x1E, "Can't you take a hint?\nI just want to be left\nalone with my " + newastositem + "!\nI even paid a Titan to\nguard the path! Fine.\nNow you face Astos,\nKing of the Hermits!"),
				(0x1E, "Can't you take a hint?\nI just want to be left\nalone with my #!\nI even paid a Titan to\nguard the path! Fine.\nNow you face Astos,\nKing of the Hermits!"),
				(0x20, "Really, a rat TAIL?\nYou think this is what\nwould impress me?\nIf you want to prove\nyourself, face off with\nAstos, the Dark Dragon!"),
				//(0xCD, "Kupo?.. Lali ho?..\nMugu mugu?.. Fine! You\nare in the presence of\nAstos, the Dark Thief!\nI stole their " + newastositem + "\nfair and square!"),
				(0xCD, "Kupo?.. Lali ho?..\nMugu mugu?.. Fine! You\nare in the presence of\nAstos, the Dark Thief!\nI stole their #\nfair and square!"),
				(0x00, ""),
				//(0x27, "Boop Beep Boop..\nError! Malfunction!..\nI see you are not\nfooled. It is I, Astos,\nKing of the Dark Robots!\nYou shall never have\nthis " + newastositem + "!"),
				(0x27, "Boop Beep Boop..\nError! Malfunction!..\nI see you are not\nfooled. It is I, Astos,\nKing of the Dark Robots!\nYou shall never have\nthis #!"),
				//(0x06, "This " + newastositem + " has passed\nfrom Queen to Princess\nfor 2000 years. It would\nhave been mine if you\nhadn't rescued me! Now\nyou face Astos, the\nDark Queen!"),
				(0x06, "This # has passed\nfrom Queen to Princess\nfor 2000 years. It would\nhave been mine if you\nhadn't rescued me! Now\nyou face Astos, the\nDark Queen!"),
				//(0x23, "I, Astos the Dark Fairy,\nam free! The other\nfairies trapped me in\nthat BOTTLE! I'd give\nyou this " + newastositem + " in\nthanks, but I would\nrather just kill you."),
				(0x23, "I, Astos the Dark Fairy,\nam free! The other\nfairies trapped me in\nthat BOTTLE! I'd give\nyou this # in\nthanks, but I would\nrather just kill you."),
				(0x2A, "If you want pass, give\nme the RUBY..\nHa, it mine! Now, you in\ntrouble. Me am Astos,\nKing of the Titans!"),
				//(0x2B, "Curses! Do you know how\nlong it took me to\ninfiltrate these grumpy\nold men and steal\nthe " + newastositem + "?\nNow feel the wrath of\nAstos, the Dark Sage!")
				(0x2B, "Curses! Do you know how\nlong it took me to\ninfiltrate these grumpy\nold men and steal\nthe #?\nNow feel the wrath of\nAstos, the Dark Sage!")
			};

			dialogues[astosdialogs[(int)newastos].Item1] = astosdialogs[(int)newastos].Item2;
			dialogues[astosdialogs[(int)ObjectId.Astos].Item1] = astosdialogs[(int)ObjectId.Astos].Item2;

			if (talkscript == TalkScripts.Talk_Titan || talkscript == TalkScripts.Talk_ElfDocUnne)
			{
				// Skip giving item for Titan, ElfDoc or Unne
				talkroutines.ReplaceChunk(newTalkRoutines.Talk_Astos, Blob.FromHex("20109F"), Blob.FromHex("EAEAEA"));
				talkroutines.ReplaceChunk(newTalkRoutines.Talk_Astos, Blob.FromHex("A9F060"), Blob.FromHex("4C4396"));
				npcdata[newastos].Script = TalkScripts.Talk_Astos;
			}
			else if (talkscript == TalkScripts.Talk_GiveItemOnFlag)
			{
				// Check for a flag instead of an item
				talkroutines.ReplaceChunk(newTalkRoutines.Talk_Astos, Blob.FromHex("A674F005BD2060F0"), Blob.FromHex("A474F00520799090"));
				npcdata[newastos].Script = TalkScripts.Talk_Astos;
			}
			else if (talkscript == TalkScripts.Talk_Nerrick || talkscript == TalkScripts.Talk_GiveItemOnItem || talkscript == TalkScripts.Talk_TradeItems)
			{
				// Just set NPC to Astos routine
				npcdata[newastos].Script = TalkScripts.Talk_Astos;
			}
			else if (talkscript == TalkScripts.Talk_Bahamut)
			{
				// Change routine to check for Tail, give promotion and trigger the battle at the same time, see 11_8200_TalkRoutines.asm
				talkroutines.Replace(newTalkRoutines.Talk_Bahamut, Blob.FromHex("AD2D60D003A57160E67DA572203D96A5752020B1A476207F9020739220AE952018964C439660"));
			}

			// Set battle
			npcdata[newastos].Battle = 0x7D;
		}

	}
}
