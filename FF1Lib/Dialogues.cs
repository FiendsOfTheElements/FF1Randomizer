using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public enum TalkScripts
	{
		Talk_None = 0,
		Talk_norm = 1,
		Talk_ifvis = 2,
		Talk_ifitem = 3,
		Talk_Invis,
		Talk_ifevent,
		Talk_GoBridge,
		Talk_4Orb,
		Talk_ifkeytnt,
		Talk_ifearthvamp,
		Talk_ifearthfire,
		Talk_Replace,
		Talk_CoOGuy,
		Talk_fight,
		Talk_BlackOrb,
		Talk_Princess1,
		Talk_SubEng,
		Talk_Titan,
		Talk_Bikke,
		Talk_Nerrick,
		Talk_Bahamut,
		Talk_ElfDocUnne,
		Talk_GiveItemOnFlag,
		Talk_TradeItems,
		Talk_GiveItemOnItem,
		Talk_Astos,
		Talk_kill,
		Talk_Chaos,
		NoOW_Floater,
		NoOW_Chime,
		NoOW_Canoe,
		NoOW_Nerrick,
		Spooky_Unne,
		Spooky_GiveOnFlag,
		Spooky_GiveOnItem,
		Spooky_Bahamut,
		Spooky_Lich,
		Talk_FightBranched

	}
	public class TalkRoutines
	{
		private List<Blob> _talkroutines = new List<Blob>();
		private const int TalkRoutinesOffset = 0x8200;
		private const int oldTalkRoutinesBank = 0x0E;
		private const int newTalkRoutinesBank = 0x11;
		public List<ushort> ScriptPointers { get; private set; }
		public TalkRoutines()
		{
			// See 11_8200_TalkRoutines.asm
			_talkroutines.Add(Blob.FromHex("60"));
			_talkroutines.Add(Blob.FromHex("A57160"));
			_talkroutines.Add(Blob.FromHex("A470209190B003A57160A57260"));
			_talkroutines.Add(Blob.FromHex("A5706920A8B90060F003A57160A57260"));
			_talkroutines.Add(Blob.FromHex("A012209190B008AD2160D003A57160A57260"));
			_talkroutines.Add(Blob.FromHex("A470207990B003A57160A57260"));
			_talkroutines.Add(Blob.FromHex("A0122091909008AD0860D003A57160A57260"));
			_talkroutines.Add(Blob.FromHex("AD32602D33602D34602D3160F003A57160A57260"));
			_talkroutines.Add(Blob.FromHex("AD2560F008AD2660D003A57160A57260"));
			_talkroutines.Add(Blob.FromHex("A00C209190B008AD3160D003A57160A57260"));
			_talkroutines.Add(Blob.FromHex("AD3160F008AD3260D003A57160A57260"));
			_talkroutines.Add(Blob.FromHex("A476209690A47320A490A57160"));
			_talkroutines.Add(Blob.FromHex("A476209690A57160"));
			_talkroutines.Add(Blob.FromHex("A571203D96A5752020B1A476207F902073922018964C439660"));
			_talkroutines.Add(Blob.FromHex("AD32602D33602D34602D3160F00CA0CA209690E67DE67DA57160A57260"));
			_talkroutines.Add(Blob.FromHex("A476207F90209690A01220A490A93F20CC90A57160"));
			_talkroutines.Add(Blob.FromHex("AD3060D003A57160A476209690A57260"));
			_talkroutines.Add(Blob.FromHex("AD2960D003A57160CE2960A476209690A572E67D60"));
			_talkroutines.Add(Blob.FromHex("A03F209190B01FA571203D96A5752020B1A03F20A490A04020A490A04120A4902018964C4396A476207990B012A573F00E1820109FB00AA476207F90A57260A57060")); // Talk_Bikke
			_talkroutines.Add(Blob.FromHex("AD2660F018A573F0141820109FB010CE2660A476207F90207392A57260A57060")); // Talk_Nerrick
			_talkroutines.Add(Blob.FromHex("A00E2079909003A57360AD2D60D003A57160CE2D60207F9020AE95E67DA57260"));
			_talkroutines.Add(Blob.FromHex("A476207990B012A674BD2060F00EDE2060207F90E67DA57260A57160A57060"));
			_talkroutines.Add(Blob.FromHex("A476207990B01BA474F0052079909015A573F0111820109FB00DA476207F90A57260A57160A57060"));
			_talkroutines.Add(Blob.FromHex("A476207990B01EA674BD2060F01AA573F0161820109FB012A674DE2060A476207F90A57260A57160A57060"));
			_talkroutines.Add(Blob.FromHex("A476207990B01BA674F005BD2060F015A573F0111820109FB00DA476207F90A57260A57160A57060"));
			_talkroutines.Add(Blob.FromHex("A674F005BD2060F029A57385612080B1F022E67DA572203D96A5752020B1A476207F90207392A5611820109F2018964C4396A57060")); // Talk_Astos
			_talkroutines.Add(Blob.FromHex("A000209690A57160")); // Talk_Kill
			_talkroutines.Add(Blob.FromHex("A57520C590A57160")); // Talk_Chaos

			// NoOverworld Routines
			_talkroutines.Add(Blob.FromHex("AD2B60D003A57160A476207392A57260"));
			_talkroutines.Add(Blob.FromHex("AD2C60D003A57160A476207392A57260"));
			_talkroutines.Add(Blob.FromHex("AD1260D003A57160A476207392A57260"));
			_talkroutines.Add(Blob.FromHex("AD2660F018A573F0141820109FB010CE2660A476207F90207392A57260A57060")); // NoOW_Nerrick

			// Lich's Revenge Routines
			_talkroutines.Add(Blob.FromHex("A674F005BD2060F01AE67DA572203D96A5752020B1A476207F902073922018964C4396A57060")); // Battle Unne
			_talkroutines.Add(Blob.FromHex("A474F0052079909029A57385612080B1B022E67DA572203D96A5752020B1A476207F90207392A5611820109F2018964C4396A57060")); // Battle Give On Flag
			_talkroutines.Add(Blob.FromHex("A674F005BD2060F029A57385612080B1B022E67DA572203D96A5752020B1A476207F90207392A5611820109F2018964C4396A57060")); // Battle Give On Item
			_talkroutines.Add(Blob.FromHex("AD2D60D003A57160E67DA572203D96A5752020B1A476207F9020739220AE952018964C439660")); // Battle Bahamut
			_talkroutines.Add(Blob.FromHex("A572203D96A5752020B1A476207F90207392A47320A4902018964C4396")); // Battle Lich

			// AltFiend Routine - TalkFightBranched
			_talkroutines.Add(Blob.FromHex("A474209190B004A570D002A571203D96A5752020B1A476207F902073922018964C439660")); // Check npc, talk, battle, then vanish

		}
		public Blob this[int talkid]
		{
			get => _talkroutines[talkid];
			set => _talkroutines[talkid] = value;
		}
		public int Add(Blob newroutine)
		{
			_talkroutines.Add(newroutine);
			return _talkroutines.Count - 1;
		}
		public void Replace(TalkScripts oldroutine, Blob newroutine)
		{
			_talkroutines[(int)oldroutine] = newroutine;
		}
		public void ReplaceChunk(TalkScripts oldroutine, Blob search, Blob replace)
		{
			_talkroutines[(int)oldroutine].ReplaceInPlace(search, replace);
		}
		private void ComputePointers()
		{
			ScriptPointers = new();
			var pos = 0;
			for (int i = 0; i < _talkroutines.Count; i++)
			{
				ScriptPointers.Add((ushort)(TalkRoutinesOffset + pos));
				pos += _talkroutines[i].Length;
			}
		}
		public void TransferTalkRoutines(FF1Rom rom, Flags flags)
		{
			// Get Talk Routines from Bank E and put them in bank 11
			rom.PutInBank(newTalkRoutinesBank, 0x902B, rom.Get(0x3902B, 0x8EA));

			// Backup npc manpulation routines and showMapObject as various other routines use them
			var npcManipulationRoutines = rom.GetFromBank(0x0E, 0x9079, 0x60);
			var hideMapObject = rom.GetFromBank(0x0E, 0x9273, 0x30);

			// Clear saved space
			rom.PutInBank(oldTalkRoutinesBank, 0x902B, new byte[0x8EA]);

			// LoadPrice bank switching fix
			rom.PutInBank(0x1F, 0xECD5, Blob.FromHex("A558"));

			// Put back HideMapObject & showMapObject
			rom.PutInBank(0x0E, 0x9079, npcManipulationRoutines);
			rom.PutInBank(0x0E, 0x9273, hideMapObject);

			// New utilities for talk routines, see 11_8200_TalkRoutines.asm
			rom.PutInBank(newTalkRoutinesBank, 0x9600, Blob.FromHex("856A20CDD8A9008D01208D1540A002204A96A001204A9660A003204A964C0FE4C96CB015C944B00CC91C900D2034DDB0094C39962046DDB00118A9F160A000204A9660686868684CB6C88510A91148A9FE48A90648B9609648B9649648A51060CAF1E8CF02FFFF39"));

			// TalkToObject Upgraded, see 11_8200_TalkRoutines.asm
			rom.PutInBank(newTalkRoutinesBank, 0x901B, Blob.FromHex("BD006F8576A00084150A26151865769002E6150A261569008514A9BA651585158612A900AAA8B1149570C8E8E006D0F6A612A5760AA8900DB900818516B9018185176C1600B900808516B9018085176C1600"));
			rom[0x7C9F7] = 0x1B;

			// TalkInBattle, compatible with ext items
			rom.PutInBank(newTalkRoutinesBank, 0xB120, Blob.FromHex("856A20CDD8A9008D01208D1540A002204A96A001204A96A903CD866BD00820189668684C439660"));

			// LoadPrice fix
			rom.PutInBank(newTalkRoutinesBank, 0x9F10, Blob.FromHex("A9118558A5734C10B4"));

			//CheckCanTake
			if (flags.ExtConsumableSet != ExtConsumableSet.None)
			{
				//C920 instead of C916
				rom.PutInBank(newTalkRoutinesBank, 0xB180, Blob.FromHex("C9169027C920900BC9449012C96C90164CABB1AABD2060C963B0114CABB12034DDB0094CABB12046DDB00118A9F160"));
			}
			else
			{
				rom.PutInBank(newTalkRoutinesBank, 0xB180, Blob.FromHex("C9169027C91C900BC9449012C96C90164CABB1AABD2060C963B0114CABB12034DDB0094CABB12046DDB00118A9F160"));
			}

			// Update bank
			rom[0x7C9F2] = newTalkRoutinesBank;
		}
		public void Write(FF1Rom rom)
		{
			ComputePointers();
			rom.PutInBank(newTalkRoutinesBank, TalkRoutinesOffset, _talkroutines.SelectMany(talk => talk.ToBytes()).ToArray());
		}
		public void Update(Flags flags)
		{
			// Disable the Princess Warp back to Castle Coneria
			if ((bool)flags.Entrances || (bool)flags.Floors || (flags.GameMode == GameModes.Standard && flags.OwMapExchange != OwMapExchanges.None) || (flags.OrbsRequiredCount == 0 && !flags.ShardHunt))
			{ 
				this.ReplaceChunk(TalkScripts.Talk_Princess1, Blob.FromHex("20CC90"), Blob.FromHex("EAEAEA"));
			}

			// Change Astos routine so item isn't lost in wall of text
			if ((bool)flags.NPCItems || (bool)flags.NPCFetchItems || (bool)flags.ShuffleAstos)
			{ 
				this.Replace(TalkScripts.Talk_Astos, Blob.FromHex("A674F005BD2060F027A57385612080B1B020A572203D96A5752020B1A476207F90207392A5611820109F201896A9F060A57060"));
			}

		}
	}
	public class DialogueData
	{
		private const int dialogsOffset = 0x402A0;
		private const int dialogsPointerOffset = 0x40000;
		private const int dialogsPointerBase = 0x38000;
		private const int dialogsPointerCount = 0x150;

		private FF1Rom rom;
		private List<string> dialogues;

		public DialogueData(FF1Rom _rom)
		{
			rom = _rom;
			LoadDialogues();
		}
		public string this[int index]
		{
			get => dialogues[index];
			set => dialogues[index] = value;
		}
		private void LoadDialogues()
		{
			// Get dialogs
			dialogues = rom.ReadText(0x28000, 0x20000, 0x100).ToList();

			// Add 0x50 extra dialogues
			dialogues.AddRange(Enumerable.Repeat("", 0x50));

			// Zero out the 0x50 extra dialogs we added
			/*for (int i = 0x0; i < 0x50; i++)
			{
				dialogues[0x100 + i] = "";
			}*/

			// Move all tile dialogs to > 0x100, this frees a few spot so we don't have to write over standard dialogs
			// Sarda get B3, Astos BF, WarMech F7 - leaving DB, E6, EE, EF, F0, F2, F3, F4, F5, F6, F7, F8, F9 free (13 total)
			dialogues[0x103] = dialogues[0xB3]; // Sample
			dialogues[0xB3] = "";
			dialogues[0x10F] = dialogues[0xBF]; // Locked
			dialogues[0xBF] = "";
			dialogues[0x12B] = dialogues[0xDB]; // Here lies
			dialogues[0xDB] = "";
			dialogues[0x136] = dialogues[0xE6]; // Wash your face
			dialogues[0xE6] = "";
			dialogues[0x13E] = dialogues[0xEE]; // Tomb
			dialogues[0xEE] = "";
			dialogues[0x13F] = dialogues[0xEF]; // Welll
			dialogues[0xEF] = "";
			dialogues[0x140] = dialogues[0xF0]; // Treasure box
			dialogues[0xF0] = "";
			dialogues[0x141] = dialogues[0xF1]; // Can't hold (duplicate for key NPCs)
												//dialogs[0xF1] = "";
			dialogues[0x142] = dialogues[0xF2]; // Empty
			dialogues[0xF2] = "";
			dialogues[0x143] = dialogues[0xF3]; // Altar of Earth
			dialogues[0xF3] = "";
			dialogues[0x144] = dialogues[0xF4]; // Fire
			dialogues[0xF4] = "";
			dialogues[0x145] = dialogues[0xF5]; // Water
			dialogues[0xF5] = "";
			dialogues[0x146] = dialogues[0xF6]; // Air
			dialogues[0xF6] = "";
			dialogues[0x147] = dialogues[0xF7]; // Bottom of spring
			dialogues[0xF7] = "";
			dialogues[0x148] = dialogues[0xF8]; // Tiamat
			dialogues[0xF8] = "";
			dialogues[0x149] = dialogues[0xF9]; // See the world
			dialogues[0xF9] = "";
			dialogues[0xDB] = dialogues[0x50]; // One of the sky warriors (ToF bat)
			dialogues[0x50] = dialogues[0];    // Nothing here
		}
		public void TransferDialogues()
		{
			// Get dialogs from bank A and put them in bank 10
			//rom.PutInBank(0x10, 0x8000, rom.Get(0x28000, 0x3600));

			// Clear most of bank 10 up to 0x2B600 where monster skills and items are, this space is now free to use
			rom.PutInBank(0x0A, 0x8000, new byte[0x3600]);

			// Get dialogs
			//dialogues = rom.ReadText(0x40000, 0x38000, 0x150).ToList();

			// SHIP, BRIDGE, CANAL, CANOE and AIRSHIP text so they can be given by NPCs
			var gameVariableText =
				"9C91929900000000" + // SHIP
				"8A929B9C91929900" + // AIRSHIP
				"8B9B928D908E0000" + // BRIDGE
				"8C8A978A95000000" + // CANAL 4x8
				"00000000" + // +4
				"8C8A97988E00"; // CANOE
			rom.Put(0x2B5D0, Blob.FromHex(gameVariableText));

			// Update to DrawDialogueString, see 1F_DB64_DrawDialogueString.asm
			var newDrawDialogueString = "AAA91085572003FEA9808597A567C9F0D0068AA2A04C7FDB8AA2002007FCA594853EA595853FA90A85172000FEA538853AA539853B2080DCA000B13EF0BEE63ED002E63FC91A904AC97A90168D0720A53A186901293F853A291FD0DC2080DC4C9CDB38E91AAA48BDA0F08D0720204EDC68AABD50F08D0720204EDCC617D0B920A1CC2069C62000FEA90A85172080DC4C9CDBC903D04EA53E48A53F48A90A85572003FEA5616920900D0A69D0853EA9B5853F184C2DDCA9B78597A561A2002007FCA594853EA595853F209CDBA91085572003FE68853F68853E4C9CDB0000000000000000205FDC4C9CDB";
			rom.Put(0x7DB64, Blob.FromHex(newDrawDialogueString));
		}
		public void InsertDialogues(Dictionary<int, string> dialogsdict)
		{
			// Insert at the right position each dictionary entry
			foreach (var d in dialogsdict)
			{
				dialogues[d.Key] = d.Value;
			}
		}
		public void UpdateNPCDialogues(Flags flags)
		{
			// Update treasure box dialog for new DrawDialogueString routine
			dialogues[0xF0 + 0x50] = "In the treasure box,\nyou found..\n#";

			// Remove reference to "Cave of Marsh" from Astos dialog, unless Crown/Crystal not shuffled
			// We do this before returning from Lich's Revenge since that flag modify all other dialogues, but not astos'
			if (flags.IncentivizeFetchNPCs != false || flags.ChestsKeyItems != false)
			{
				dialogues[0x11] = "Astos double-crossed us.\nFind where they stashed\nthe CROWN. Then bring it\ndirectly back to me!";
				dialogues[0x12] = "HA, HA, HA! I am Astos,\nKing of the Dark Elves.\nI have the #\nand you shall give me\nthat CROWN, now!!!";
			}

			dialogues[0xF0] = "Received #";

			if (flags.SpookyFlag)
			{
				return;
			}

			// Dialogue for Sarda if Early sarda is off
			dialogues[0xB3]  = "I shall help only\nthe true LIGHT WARRIORS.\nProve yourself by\ndefeating the Vampire.";

			if (!(bool)flags.NPCItems && !(bool)flags.NPCFetchItems)
			{
				return;
			}

			// Update all NPC dialogs for NPC shuffle so we can show what item they're giving.
			dialogues[0x02] = (bool)flags.EarlyKing ? "To aid you on your\nquest, please take this.\n\n\n\nReceived #" : "Thank you for saving the\nPrincess. To aid your\nquest, please take this.\n\n\nReceived #";
			dialogues[0x06] = "This heirloom has been\npassed down from Queen\nto Princess for 2000\nyears. Please take it.\n\nReceived #";
			dialogues[0x09] = "Okay, you got me.\nTake this.\n\n\n\nReceived #";
			dialogues[0x0E] = "Is this a dream?.. Are\nyou the LIGHT WARRIORS?\nSo, as legend says,\nI give you this.\n\nReceived #";
			dialogues[0x14] = "Yes, yes indeed,\nthis TNT is just what I\nneed to finish my work.\nTake this in return!\n\nReceived #";
			dialogues[0x16] = "ADAMANT!! Now let me\nforge this for you..\nHere, the best work\nI've ever done.\n\nReceived #";
			dialogues[0x19] = "I'll trade my most\npowerful charm to get\nmy CRYSTAL back..\nOh! I can see!!\n\nReceived #";
			dialogues[0x1E] = "Take this.\nIt will help you\nfight the source of the\nearth's rot.\n\nReceived #";
			dialogues[0x23] = "That pirate trapped me\nin the BOTTLE. I will\nget what's at the bottom\nof the spring for you.\n\nReceived #";
			dialogues[0x27] = "Take this.\n\n\n\n\nReceived #";
			dialogues[0x2B] = (bool)flags.EarlySage ? "The FIENDS are waking.\nTake this and go defeat\nthem!\n\n\nReceived #" : "Great job vanquishing\nthe Earth FIEND.\nWith this, go and defeat\nthe other FIENDS!\n\nReceived #";
			dialogues[0xCD] = "With this, you can\navenge the SKY WARRIORS.\n\n\n\nReceived #";
		}
		public void Write()
		{
			// Reinsert updated dialogs with updated pointers
			int offset = dialogsOffset;
			var pointers = new ushort[dialogues.Count];
			Blob generatedText = Blob.FromHex("");

			for (int i = 0; i < dialogues.Count; i++)
			{
				var blob = FF1Text.TextToBytes(dialogues[i], useDTE: true);
				generatedText += blob;

				pointers[i] = (ushort)(offset - dialogsPointerBase);
				offset += blob.Length;
			}

			// Check if dialogs are too long
			if (pointers.Length * 2 + generatedText.Length > 0x4000)
				throw new Exception("Dialogs maximum length exceeded.");

			// Insert dialogs
			rom.Put(dialogsPointerOffset, Blob.FromUShorts(pointers) + generatedText);
		}
	}
}
