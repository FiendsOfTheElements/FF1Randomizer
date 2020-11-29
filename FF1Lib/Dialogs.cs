using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RomUtilities;

namespace FF1Lib
{
	public partial class FF1Rom
	{
		public const int dialogsOffset = 0x402A0;
		public const int dialogsPointerOffset = 0x40000;
		public const int dialogsPointerBase = 0x38000;
		public const int dialogsPointerCount = 0x150;

		public const int NpcTalkOffset = 0x390D3;
		public const int NpcTalkSize = 2;
		public const int NpcVisFlagOffset = 0x2F00;

		public const int lut_MapObjTalkJumpTbl = 0x90D3;
		public const int lut_MapObjTalkData = 0x95D5;

		public const int oldTalkRoutinesBank = 0x0E;
		public const int newTalkRoutinesBank = 0x11;

		public List<IRewardSource> generatedPlacement;
		// All original talk scripts for reference
		public static partial class originalTalk
		{
			public static readonly Blob Talk_None = Blob.FromHex("9692");
			public static readonly Blob Talk_KingConeria = Blob.FromHex("9792");
			public static readonly Blob Talk_Garland = Blob.FromHex("B192");
			public static readonly Blob Talk_Princess1 = Blob.FromHex("BE92");
			public static readonly Blob Talk_Bikke = Blob.FromHex("D092");
			public static readonly Blob Talk_ElfDoc = Blob.FromHex("0193");
			public static readonly Blob Talk_ElfPrince = Blob.FromHex("1E93");
			public static readonly Blob Talk_Astos = Blob.FromHex("3893");
			public static readonly Blob Talk_Nerrick = Blob.FromHex("5293");
			public static readonly Blob Talk_Smith = Blob.FromHex("6C93");
			public static readonly Blob Talk_Matoya = Blob.FromHex("9893");
			public static readonly Blob Talk_Unne = Blob.FromHex("BA93");
			public static readonly Blob Talk_Vampire = Blob.FromHex("D793");
			public static readonly Blob Talk_Sarda = Blob.FromHex("E493");
			public static readonly Blob Talk_Bahamut = Blob.FromHex("FB93");
			public static readonly Blob Talk_ifvis = Blob.FromHex("1B94");
			public static readonly Blob Talk_SubEng = Blob.FromHex("2894");
			public static readonly Blob Talk_CubeBot = Blob.FromHex("3894");
			public static readonly Blob Talk_Princess2 = Blob.FromHex("4894");
			public static readonly Blob Talk_Fairy = Blob.FromHex("5894");
			public static readonly Blob Talk_Titan = Blob.FromHex("6894");
			public static readonly Blob Talk_CanoeSage = Blob.FromHex("7D94");
			public static readonly Blob Talk_norm = Blob.FromHex("9294");
			public static readonly Blob Talk_Replace = Blob.FromHex("9594");
			public static readonly Blob Talk_CoOGuy = Blob.FromHex("A294");
			public static readonly Blob Talk_fight = Blob.FromHex("AA94");
			public static readonly Blob Talk_Unused = Blob.FromHex("B794");
			public static readonly Blob Talk_ifitem = Blob.FromHex("B894");
			public static readonly Blob Talk_Invis = Blob.FromHex("C594");
			public static readonly Blob Talk_ifbridge = Blob.FromHex("D794");
			public static readonly Blob Talk_ifevent = Blob.FromHex("E294");
			public static readonly Blob Talk_GoBridge = Blob.FromHex("F094");
			public static readonly Blob Talk_BlackOrb = Blob.FromHex("0295");
			public static readonly Blob Talk_4Orb = Blob.FromHex("1F95");
			public static readonly Blob Talk_ifcanoe = Blob.FromHex("3395");
			public static readonly Blob Talk_ifcanal = Blob.FromHex("3E95");
			public static readonly Blob Talk_ifkeytnt = Blob.FromHex("4995");
			public static readonly Blob Talk_ifearthvamp = Blob.FromHex("5995");
			public static readonly Blob Talk_ifairship = Blob.FromHex("6B95");
			public static readonly Blob Talk_ifearthfire = Blob.FromHex("7695");
			public static readonly Blob Talk_CubeBotBad = Blob.FromHex("8695");
			public static readonly Blob Talk_Chime = Blob.FromHex("9495");
		}
		// New talk scripts
		public static partial class newTalk
		{
			public static readonly Blob Talk_None = Blob.FromHex("9692");
			public static readonly Blob Talk_norm = Blob.FromHex("9792");
			public static readonly Blob Talk_ifvis = Blob.FromHex("9A92");
			public static readonly Blob Talk_ifitem = Blob.FromHex("A792");
			public static readonly Blob Talk_Invis = Blob.FromHex("B792");
			public static readonly Blob Talk_ifevent = Blob.FromHex("C992");
			public static readonly Blob Talk_GoBridge = Blob.FromHex("D692");
			public static readonly Blob Talk_4Orb = Blob.FromHex("E892");
			public static readonly Blob Talk_ifkeytnt = Blob.FromHex("FC92");
			public static readonly Blob Talk_ifearthvamp = Blob.FromHex("0C93");
			public static readonly Blob Talk_ifearthfire = Blob.FromHex("1E93");
			public static readonly Blob Talk_Replace = Blob.FromHex("2E93");
			public static readonly Blob Talk_CoOGuy = Blob.FromHex("3B93");
			public static readonly Blob Talk_fight = Blob.FromHex("4393");
			public static readonly Blob Talk_BlackOrb = Blob.FromHex("5393");
			public static readonly Blob Talk_Princess1 = Blob.FromHex("7093");
			public static readonly Blob Talk_SubEng = Blob.FromHex("8593");
			public static readonly Blob Talk_Titan = Blob.FromHex("9593");
			public static readonly Blob Talk_Bikke = Blob.FromHex("AA93");
			public static readonly Blob Talk_Nerrick = Blob.FromHex("E693");
			public static readonly Blob Talk_Bahamut = Blob.FromHex("0A94");
			public static readonly Blob Talk_ElfDocUnne = Blob.FromHex("2A94");
			public static readonly Blob Talk_GiveItemOnFlag = Blob.FromHex("4A94");
			public static readonly Blob Talk_TradeItems = Blob.FromHex("7894");
			public static readonly Blob Talk_GiveItemOnItem = Blob.FromHex("A994");
			public static readonly Blob Talk_Astos = Blob.FromHex("D694");
			public static readonly Blob Talk_kill = Blob.FromHex("1095");
		}
		public bool RedMageHasLife()
		{
			var itemnames = ReadText(ItemTextPointerOffset, ItemTextPointerBase, ItemTextPointerCount);
			var magicPermissions = Get(MagicPermissionsOffset, 8 * 12).Chunk(8);
			var magicArray = new List<byte> { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };

			var firstLifeIndex = itemnames.ToList().FindIndex(x => x.ToLower().Contains("lif")) - 176;
			var secondLifeIndex = itemnames.ToList().FindIndex(firstLifeIndex + 177, x => x.ToLower().Contains("lif")) - 176;

			var firstlife = firstLifeIndex >= 0 ? (((~magicPermissions[3][firstLifeIndex / 8] & magicArray[firstLifeIndex % 8]) > 0) ? true : false) : false;
			var secondlife = secondLifeIndex >= 0 ? (((~magicPermissions[3][secondLifeIndex / 8] & magicArray[secondLifeIndex % 8]) > 0) ? true : false) : false;

			return (firstlife || secondlife);
		}
		public void AddNewChars()
		{
			// New characters, frees up 5 characters at 0xCA, 0xCE, 0xE6, 0xE8, 0xE9
			// STATUS, use normal S and draw AT closer
			// Put(0x385A1, Blob.FromHex("9CCBCCCD9C"));
			// Put(0x24CC0, Blob.FromHex("00DFC66666E63636"));
			// Put(0x24CD0, Blob.FromHex("00B333333333331E"));

			// Stone ailment, use normal S and e, use Poison's "on"
			Put(0x6C360, Blob.FromHex("9CE7E5A8")); // Stone

			// White out free chars
			//Put(0x24CA0, Blob.FromHex("FFFFFFFFFFFFFFFF"));
			//Put(0x24CE0, Blob.FromHex("FFFFFFFFFFFFFFFF"));
			Put(0x24E60, Blob.FromHex("FFFFFFFFFFFFFFFF"));
			Put(0x24E80, Blob.FromHex("FFFFFFFFFFFFFFFF"));
			Put(0x24E90, Blob.FromHex("FFFFFFFFFFFFFFFF"));

			// Add plus sign at 0xC1
			Put(0x24C10, Blob.FromHex("000008083E080800"));
		}

		public void TransferDialogs()
		{
			// Get dialogs from bank A and put them in bank 10
			PutInBank(0x10, 0x8000, Get(0x28000, 0x3600));

			// Clear most of bank 10 up to 0x2B600 where monster skills and items are, this space is now free to use
			PutInBank(0x0A, 0x8000, new byte[0x3600]);

			// Get dialogs
			var dialogs = ReadText(0x40000, 0x38000, 0x150);

			// Zero out the 0x50 extra dialogs we added
			for (int i = 0x0; i < 0x50; i++)
			{
				dialogs[0x100 + i] = "";
			}

			// Move all tile dialogs to > 0x100, this frees a few spot so we don't have to write over standard dialogs
			// Sarda get B3, Astos BF, WarMech F7 - leaving DB, E6, EE, EF, F0, F2, F3, F4, F5, F6, F7, F8, F9 free (13 total)
			dialogs[0x103] = dialogs[0xB3]; // Sample
			dialogs[0xB3] = "";
			dialogs[0x10F] = dialogs[0xBF]; // Locked
			dialogs[0xBF] = "";
			dialogs[0x12B] = dialogs[0xDB]; // Here lies
			dialogs[0xDB] = "";
			dialogs[0x136] = dialogs[0xE6]; // Wash your face
			dialogs[0xE6] = "";
			dialogs[0x13E] = dialogs[0xEE]; // Tomb
			dialogs[0xEE] = "";
			dialogs[0x13F] = dialogs[0xEF]; // Welll
			dialogs[0xEF] = "";
			dialogs[0x140] = dialogs[0xF0]; // Treasure box
			dialogs[0xF0] = "";
			dialogs[0x141] = dialogs[0xF1]; // Can't hold (duplicate for key NPCs)
											//dialogs[0xF1] = "";
			dialogs[0x142] = dialogs[0xF2]; // Empty
			dialogs[0xF2] = "";
			dialogs[0x143] = dialogs[0xF3]; // Altar of Earth
			dialogs[0xF3] = "";
			dialogs[0x144] = dialogs[0xF4]; // Fire
			dialogs[0xF4] = "";
			dialogs[0x145] = dialogs[0xF5]; // Water
			dialogs[0xF5] = "";
			dialogs[0x146] = dialogs[0xF6]; // Air
			dialogs[0xF6] = "";
			dialogs[0x147] = dialogs[0xF7]; // Bottom of spring
			dialogs[0xF7] = "";
			dialogs[0x148] = dialogs[0xF8]; // Tiamat
			dialogs[0xF8] = "";
			dialogs[0x149] = dialogs[0xF9]; // See the world
			dialogs[0xF9] = "";
			dialogs[0xDB] = dialogs[0x50];
			dialogs[0x50] = dialogs[0];    // Nothing here

			// Reinsert updated dialogs with updated pointers
			int offset = dialogsOffset;
			var pointers = new ushort[dialogs.Length];
			Blob generatedText = Blob.FromHex("");

			for (int i = 0; i < dialogs.Length; i++)
			{
				var blob = FF1Text.TextToBytes(dialogs[i], useDTE: true);
				generatedText += blob;

				pointers[i] = (ushort)(offset - dialogsPointerBase);
				offset += blob.Length;
			}

			// Check if dialogs are too long
			if (pointers.Length * 2 + generatedText.Length > 0x4000)
				throw new Exception("Dialogs maximum length exceeded.");

			// Insert dialogs
			Put(dialogsPointerOffset, Blob.FromUShorts(pointers) + generatedText);
		}
		public void TransferTalkRoutines()
		{
			// Get Talk Routines from Bank E and put them in bank 11
			PutInBank(newTalkRoutinesBank, 0x902B, Get(0x3902B, 0x8EA));
			
			// Backup npc manpulation routines and showMapObject as various other routines use them
			var npcManipulationRoutines = GetFromBank(0x0E, 0x9079, 0x60);
			var hideMapObject = GetFromBank(0x0E, 0x9273, 0x30);

			// Clear saved space
			PutInBank(oldTalkRoutinesBank, 0x902B, new byte[0x8EA]);

			// LoadPrice bank switching fix
			PutInBank(0x1F, 0xECD5, Blob.FromHex("A558"));

			// Put back HideMapObject & showMapObject
			PutInBank(0x0E, 0x9079, npcManipulationRoutines);
			PutInBank(0x0E, 0x9273, hideMapObject);

			// Update bank
			Data[0x7C9F2] = newTalkRoutinesBank;
		}

		// Required for npc quest item randomizing
		// Doesn't substantially change anything if EnableNPCsGiveAnyItem isn't called
		public void CleanupNPCRoutines()
		{
			// Swap string pointer in index 2 and 3 for King, Bikke, Prince, and Lefein
			/*
			var temp = Data[ItemLocations.KingConeria.Address];
			Data[ItemLocations.KingConeria.Address] = Data[ItemLocations.KingConeria.Address - 1];
			Data[ItemLocations.KingConeria.Address - 1] = temp;
			temp = Data[ItemLocations.Bikke.Address];
			Data[ItemLocations.Bikke.Address] = Data[ItemLocations.Bikke.Address - 1];
			Data[ItemLocations.Bikke.Address - 1] = temp;
			temp = Data[ItemLocations.ElfPrince.Address];
			Data[ItemLocations.ElfPrince.Address] = Data[ItemLocations.ElfPrince.Address - 1];
			Data[ItemLocations.ElfPrince.Address - 1] = temp;
			temp = Data[ItemLocations.CanoeSage.Address - 1];
			Data[ItemLocations.CanoeSage.Address - 1] = Data[ItemLocations.CanoeSage.Address - 2];
			Data[ItemLocations.CanoeSage.Address - 2] = temp;
			temp = Data[ItemLocations.Lefein.Address];
			Data[ItemLocations.Lefein.Address] = Data[ItemLocations.Lefein.Address - 1];
			Data[ItemLocations.Lefein.Address - 1] = temp;
			*/
			// And do the same swap in the vanilla routines so those still work if needed
			Data[0x392A7] = 0x12;
			Data[0x392AA] = 0x13;
			Data[0x392FC] = 0x13;
			Data[0x392FF] = 0x12;
			Data[0x39326] = 0x12;
			Data[0x3932E] = 0x13;
			Data[0x3959C] = 0x12;
			Data[0x395A4] = 0x13;

			// When getting jump address from lut_MapObjTalkJumpTbl (starting 0x3902B), store
			// it in tmp+4 & tmp+5 (unused normally) instead of tmp+6 & tmp+7 so that tmp+6
			// will still have the mapobj_id (allowing optimizations in TalkRoutines)
			Data[0x39063] = 0x14;
			Data[0x39068] = 0x15;
			Data[0x3906A] = 0x14;
			Data[0x39070] = 0x14;
			Data[0x39075] = 0x15;
			Data[0x39077] = 0x14;
		}

		// Remove trailing spaces and return the right item for some generated text
		public string FormattedItemName(Item item, bool specialItem = true)
		{
			var itemnames = ReadText(FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, 256);
			var formatted = itemnames[(byte)item].TrimEnd(' ');

			if(specialItem)
			{ 
				switch (item)
				{
					case Item.Ship:
						formatted = FF1Text.BytesToText(Get(0x2B5D0, 8)).TrimEnd(' ');
						break;
					case Item.Bridge:
						formatted = FF1Text.BytesToText(Get(0x2B5D0 + 16, 8)).TrimEnd(' ');
						break;
					case Item.Canal:
						formatted = FF1Text.BytesToText(Get(0x2B5D0 + 24, 8)).TrimEnd(' ');
						break;
					case Item.Canoe:
						formatted = FF1Text.BytesToText(Get(0x2B5D0 + 36, 8)).TrimEnd(' ');
						break;
				}
			}
			return formatted;
		}
		// Insert a single dialog in the given position
		public void InsertDialogs(int dialogID, string dialogtext)
		{
			// Get dialogs
			var dialogs = ReadText(dialogsPointerOffset, dialogsPointerBase, dialogsPointerCount);

			// Insert the new dialog string at the right position
			dialogs[dialogID] = dialogtext;

			// Convert all dialogs to bytes
			int offset = dialogsOffset;
			var pointers = new ushort[dialogs.Length];
			Blob generatedText = Blob.FromHex("");

			for (int i = 0; i < dialogs.Length; i++)
			{
				var blob = FF1Text.TextToBytes(dialogs[i], useDTE: true);
				generatedText += blob;

				pointers[i] = (ushort)(offset - dialogsPointerBase);
				offset += blob.Length;
			}

			// Check if dialogs are too long
			if (pointers.Length * 2 + generatedText.Length > 0x4000)
				throw new Exception("Dialogs maximum length exceeded.");

			// Insert dialogs
			Put(dialogsPointerOffset, Blob.FromUShorts(pointers) + generatedText);
		}
		// Insert several dialogs at the given positions
		public void InsertDialogs(Dictionary<int, string> dialogsdict)
		{
			// Get dialogs
			var dialogs = ReadText(dialogsPointerOffset, dialogsPointerBase, dialogsPointerCount);

			// Insert at the right position each dictionary entry
			foreach (var x in dialogsdict)
				dialogs[x.Key] = x.Value;

			// Convert all dialogs to bytes
			int offset = dialogsOffset;
			var pointers = new ushort[dialogs.Length];
			Blob generatedText = Blob.FromHex("");

			for (int i = 0; i < dialogs.Length; i++)
			{
				var blob = FF1Text.TextToBytes(dialogs[i], useDTE: true);
				generatedText += blob;

				pointers[i] = (ushort)(offset - dialogsPointerBase);
				offset += blob.Length;
			}

			// Check if dialogs are too long
			if (pointers.Length * 2 + generatedText.Length > 0x4000)
				throw new Exception("Dialogs maximum length exceeded.");

			// Insert dialogs
			Put(dialogsPointerOffset, Blob.FromUShorts(pointers) + generatedText);
		}
		public void NPCShuffleDialogs()
		{
			// Update all NPC dialogs for NPC shuffle so we can show what item they're giving.
			Dictionary<int, string> NPCShuffleDialogs = new Dictionary<int, string>();

			NPCShuffleDialogs.Add(0x02, "Thank you for saving the\nPrincess. To aid your\nquest, please take this.\n\n\nReceived #");
			NPCShuffleDialogs.Add(0x06, "This heirloom has been\npassed down from Queen\nto Princess for 2000\nyears. Please take it.\n\nReceived #");
			NPCShuffleDialogs.Add(0x09, "Okay, you got me.\nTake this.\n\n\n\nReceived #");
			NPCShuffleDialogs.Add(0x0E, "Is this a dream?.. Are\nyou the LIGHT WARRIORS?\nSo, as legend says,\nI give you this.\n\nReceived #");
			NPCShuffleDialogs.Add(0x12, "HA, HA, HA! I am Astos,\nKing of the Dark Elves.\nI have the #\nand you shall give me\nthat CROWN, now!!!");
			NPCShuffleDialogs.Add(0x14, "Yes, yes indeed,\nthis TNT is just what I\nneed to finish my work.\nTake this in return!\n\nReceived #");
			NPCShuffleDialogs.Add(0x16, "ADAMANT!! Now let me\nforge this for you..\nHere, the best work\nI've ever done.\n\nReceived #");
			NPCShuffleDialogs.Add(0x19, "I'll trade my most\npowerful charm to get\nmy CRYSTAL back..\nOh! I can see!!\n\nReceived #");
			NPCShuffleDialogs.Add(0x1E, "Take this.\nIt will help you\nfight the source of the\nearth's rot.\n\nReceived #");
			NPCShuffleDialogs.Add(0x23, "That pirate trapped me\nin the BOTTLE. I will\nget what's at the bottom\nof the spring for you.\n\nReceived #");
			NPCShuffleDialogs.Add(0x27, "Take this.\n\n\n\n\nReceived #");
			NPCShuffleDialogs.Add(0x2B, "Great job vanquishing\nthe Earth FIEND.\nWith this, go and defeat\nthe other FIENDS!\n\nReceived #");
			NPCShuffleDialogs.Add(0xCD, "With this, you can\navenge the SKY WARRIORS.\n\n\n\nReceived #");

			InsertDialogs(NPCShuffleDialogs);
		}
		public void UpdateDialogs()
		{
			Dictionary<int, string> newDialogs = new Dictionary<int, string>();

			CleanupNPCRoutines();
			SplitOpenTreasureRoutine();
			TransferDialogs();
			TransferTalkRoutines();
			AddNewChars();

			// Get all NPC scripts and script values to update them
			var npcScriptValue = GetFromBank(newTalkRoutinesBank, lut_MapObjTalkData, 0xD0 * 4).Chunk(4);
			var npcScript = GetFromBank(newTalkRoutinesBank, lut_MapObjTalkJumpTbl, 0xD0 * 2).Chunk(2);

			// Insert new dialogs Talk routine, see 0E_9296_TalkRoutines.asm
			//			var newTalkRoutines = "60A51160A410209190B003A51160A51260A5106920A8B90060F003A51160A51260A012209190B008AD2160D003A51160A51260A410207990B003A51160A51260A0122091909008AD0860D003A51160A51260AD32602D33602D34602D3160F003A51160A51260AD2560F008AD2660D003A51160A51260A00C209190B008AD3160D003A51160A51260AD3160F008AD3260D003A51160A51260A416209690A41320A490A51160A416209690A51160A416207F90209690A51320C590A51160AD32602D33602D34602D3160F00CA0CA209690E67DE67DA51160A51260A416207F90209690A01220A490A93F20CC90A51160AD3060D003A51160A416209690A51260AD2960D003A51160CE2960A416209690A512E67D60A03F209190B01520A490A04020A490A04120A490A97E20C590A51160A416207990B016A513F012A4128414182093DDB00AA416207F90A51460A51060AD2660F01CA513F018A4128414182093DDB010CE2660A416207F90207392A51460A51060A00E2079909003A51360AD2D60D003A51160CE2D60207F9020AE95E67DA51260A416207990B013BE8095BD2060F00EDE2060207F90E67DA51260A51160A51060A416207990B021B98095F006A82079909019A513F015A4128414182093DDB00DA416207F90A51460A51160A51060A416207990B024BE8095BD2060F01FA513F01BA4128414182093DDB013A416BE8095DE2060207F90A51460A51160A51060A416207990B0F3BE8095F005BD2060F019A513F015A4128414182093DDB00DA416207F90A51460A51160A51060A416BE8095F006EABD2060F02AA513F026A4128414182093DDB01E8467A5108D0E03A5118D0F03A416207F90207392A97D20C590A51460A51060A000209690A51160";
			var newTalkRoutines = "60A57160A470209190B003A57160A57260A5706920A8B90060F003A57160A57260A012209190B008AD2160D003A57160A57260A470207990B003A57160A57260A0122091909008AD0860D003A57160A57260AD32602D33602D34602D3160F003A57160A57260AD2560F008AD2660D003A57160A57260A00C209190B008AD3160D003A57160A57260AD3160F008AD3260D003A57160A57260A476209690A47320A490A57160A476209690A57160A476207F90209690A57320C590A57160AD32602D33602D34602D3160F00CA0CA209690E67DE67DA57160A57260A476207F90209690A01220A490A93F20CC90A57160AD3060D003A57160A476209690A57260AD2960D003A57160CE2960A476209690A572E67D60A03F209190B01520A490A04020A490A04120A490A97E20C590A57160A476207990B016A573F012A4728474182093DDB00AA476207F90A57460A57060AD2660F01CA573F018A4728474182093DDB010CE2660A476207F90207392A57460A57060A00E2079909003A57360AD2D60D003A57160CE2D60207F9020AE95E67DA57260A476207990B013BE8095BD2060F00EDE2060207F90E67DA57260A57160A57060A476207990B021B98095F006A82079909019A573F015A4728474182093DDB00DA476207F90A57460A57160A57060A476207990B024BE8095BD2060F01FA573F01BA4728474182093DDB013A476BE8095DE2060207F90A57460A57160A57060A476207990B020BE8095F005BD2060F019A573F015A4728474182093DDB00DA476207F90A57460A57160A57060A476BE8095F006EABD2060F02AA573F026A4728474182093DDB01E8467A5108D0E03A5118D0F03A476207F90207392A97D20C590A57460A57060A000209690A57160";

			// Fix for bank change
			newTalkRoutines = newTalkRoutines.Replace("2093DD", "20109F");
			PutInBank(newTalkRoutinesBank, 0x9296, Blob.FromHex(newTalkRoutines));

			// LoadPrice fix
			PutInBank(newTalkRoutinesBank, 0x9F10, Blob.FromHex("A9118558A5734C93DD"));

			// Load NPC scripts in zero page at $70 instead of $10
			Data[0x4502F] = 0x76;
			Data[0x45049] = 0x70;
			Data[0x4504E] = 0x71;
			Data[0x45053] = 0x72;
			Data[0x45058] = 0x73;
			Data[0x4505A] = 0x76;

			// Lut for required items check, only the first 32 NPCs can give an item with this, but it should be enough for now
			PutInBank(newTalkRoutinesBank, 0x9580, Blob.FromHex("000300000004050206070308000C0D0B10000000091100000000000000000000"));

			// Update all NPC's dialogs script, default behaviours are maintained
			for (int i = 0; i < 0xD0; i++)
			{
				if (npcScript[i] == originalTalk.Talk_4Orb)
				{
					npcScript[i] = newTalk.Talk_4Orb;
				}
				else if (npcScript[i] == originalTalk.Talk_Astos)
				{
					//newDialogs.Add(0xBF, "I'm not quite dead yet.");
					npcScript[i] = newTalk.Talk_Astos;
					npcScriptValue[i] = Blob.FromHex("11181200");
					npcScriptValue[i][3] = (byte)Item.Crystal;
				}
				else if (npcScript[i] == originalTalk.Talk_Bahamut)
				{
					npcScript[i] = newTalk.Talk_Bahamut;
				}
				else if (npcScript[i] == originalTalk.Talk_Bikke)
				{
					npcScript[i] = newTalk.Talk_Bikke;
					npcScriptValue[i] = Blob.FromHex("0A080900");
					npcScriptValue[i][3] = (byte)Item.Ship;
				}
				else if (npcScript[i] == originalTalk.Talk_BlackOrb)
				{
					npcScript[i] = newTalk.Talk_BlackOrb;
				}
				else if (npcScript[i] == originalTalk.Talk_CanoeSage)
				{
					npcScript[i] = newTalk.Talk_GiveItemOnItem;
					npcScriptValue[i] = Blob.FromHex("2C2C2B00");
					npcScriptValue[i][3] = (byte)Item.Canoe;
				}
				else if (npcScript[i] == originalTalk.Talk_Chime)
				{
					npcScript[i] = newTalk.Talk_GiveItemOnFlag;
					npcScriptValue[i] = Blob.FromHex("D0CECD00");
					npcScriptValue[i][3] = (byte)Item.Chime;
				}
				else if (npcScript[i] == originalTalk.Talk_CoOGuy)
				{
					npcScript[i] = newTalk.Talk_CoOGuy;
				}
				else if (npcScript[i] == originalTalk.Talk_CubeBot)
				{
					npcScript[i] = newTalk.Talk_GiveItemOnFlag;
					npcScriptValue[i] = Blob.FromHex("00282700");
					npcScriptValue[i][3] = (byte)Item.Cube;
				}
				else if (npcScript[i] == originalTalk.Talk_ElfDoc)
				{
					npcScript[i] = newTalk.Talk_ElfDocUnne;
					npcScriptValue[i] = Blob.FromHex("0B0D0C00");
				}
				else if (npcScript[i] == originalTalk.Talk_ElfPrince)
				{
					npcScript[i] = newTalk.Talk_GiveItemOnFlag;
					npcScriptValue[i] = Blob.FromHex("100F0E00");
					npcScriptValue[i][3] = (byte)Item.Key;
				}
				else if (npcScript[i] == originalTalk.Talk_Fairy)
				{
					npcScript[i] = newTalk.Talk_GiveItemOnFlag;
					npcScriptValue[i] = Blob.FromHex("00242300");
					npcScriptValue[i][3] = (byte)Item.Oxyale;
				}
				else if (npcScript[i] == originalTalk.Talk_fight)
				{
					npcScript[i] = newTalk.Talk_fight;
				}
				else if (npcScript[i] == originalTalk.Talk_Garland)
				{
					npcScript[i] = newTalk.Talk_fight;
					npcScriptValue[i][3] = 0x7F;
				}
				else if (npcScript[i] == originalTalk.Talk_GoBridge)
				{
					npcScript[i] = newTalk.Talk_GoBridge;
				}
				else if (npcScript[i] == originalTalk.Talk_ifairship)
				{
					npcScript[i] = newTalk.Talk_ifitem;
					npcScriptValue[i][3] = 0xE4;
				}
				else if (npcScript[i] == originalTalk.Talk_ifbridge)
				{
					npcScript[i] = newTalk.Talk_ifitem;
					npcScriptValue[i][0] = (byte)Item.Bridge;
				}
				else if (npcScript[i] == originalTalk.Talk_ifcanal)
				{
					npcScript[i] = newTalk.Talk_ifitem;
					npcScriptValue[i][0] = (byte)Item.Canal;
					var a = npcScriptValue[i][1];
					npcScriptValue[i][1] = npcScriptValue[i][2];
					npcScriptValue[i][2] = a;
				}
				else if (npcScript[i] == originalTalk.Talk_ifearthvamp)
				{
					npcScript[i] = newTalk.Talk_ifearthvamp;
				}
				else if (npcScript[i] == originalTalk.Talk_ifevent)
				{
					npcScript[i] = newTalk.Talk_ifevent;
				}
				else if (npcScript[i] == originalTalk.Talk_ifitem)
				{
					npcScript[i] = newTalk.Talk_ifitem;
				}
				else if (npcScript[i] == originalTalk.Talk_ifkeytnt)
				{
					npcScript[i] = newTalk.Talk_ifkeytnt;
				}
				else if (npcScript[i] == originalTalk.Talk_ifearthfire)
				{
					npcScript[i] = newTalk.Talk_ifearthfire;
				}
				else if (npcScript[i] == originalTalk.Talk_ifvis)
				{
					npcScript[i] = newTalk.Talk_ifvis;
				}
				else if (npcScript[i] == originalTalk.Talk_Invis)
				{
					npcScript[i] = newTalk.Talk_Invis;
				}
				else if (npcScript[i] == originalTalk.Talk_KingConeria)
				{
					npcScript[i] = newTalk.Talk_GiveItemOnFlag;
					npcScriptValue[i] = Blob.FromHex("01030200");
					npcScriptValue[i][3] = (byte)Item.Bridge;
				}
				else if (npcScript[i] == originalTalk.Talk_Matoya)
				{
					npcScript[i] = newTalk.Talk_TradeItems;
					npcScriptValue[i] = Blob.FromHex("171A1900");
					npcScriptValue[i][3] = (byte)Item.Herb;
				}
				else if (npcScript[i] == originalTalk.Talk_None)
				{
					npcScript[i] = newTalk.Talk_None;
				}
				else if (npcScript[i] == originalTalk.Talk_norm)
				{
					npcScript[i] = newTalk.Talk_norm;
				}
				else if (npcScript[i] == originalTalk.Talk_Princess1)
				{
					npcScript[i] = newTalk.Talk_Princess1;
				}
				else if (npcScript[i] == originalTalk.Talk_Princess2)
				{
					npcScript[i] = newTalk.Talk_GiveItemOnFlag;
					npcScriptValue[i] = Blob.FromHex("00070600");
					npcScriptValue[i][3] = (byte)Item.Lute;
				}
				else if (npcScript[i] == originalTalk.Talk_Replace)
				{
					npcScript[i] = newTalk.Talk_Replace;
				}
				else if (npcScript[i] == originalTalk.Talk_Sarda)
				{
					npcScript[i] = newTalk.Talk_GiveItemOnFlag;
					npcScriptValue[i] = Blob.FromHex("B3181E00");
					npcScriptValue[i][3] = (byte)Item.Rod;
				}
				else if (npcScript[i] == originalTalk.Talk_SubEng)
				{
					npcScript[i] = newTalk.Talk_SubEng;
				}
				else if (npcScript[i] == originalTalk.Talk_Titan)
				{
					npcScript[i] = newTalk.Talk_Titan;
				}
				else if (npcScript[i] == originalTalk.Talk_Unne)
				{
					npcScript[i] = newTalk.Talk_ElfDocUnne;
					npcScriptValue[i] = Blob.FromHex("1B181C00");
				}
				else if (npcScript[i] == originalTalk.Talk_Unused)
				{
					npcScript[i] = newTalk.Talk_None;
				}
				else if (npcScript[i] == originalTalk.Talk_Vampire)
				{
					npcScript[i] = newTalk.Talk_fight;
					npcScriptValue[i][3] = 0x7C;
				}

				else if (npcScript[i] == originalTalk.Talk_ifcanoe)
				{
					npcScript[i] = newTalk.Talk_ifitem;
					npcScriptValue[i][0] = (byte)Item.Canoe;
				}
				else if (npcScript[i] == originalTalk.Talk_Nerrick)
				{
					npcScript[i] = newTalk.Talk_Nerrick;
					npcScriptValue[i] = Blob.FromHex("13001400");
					npcScriptValue[i][3] = (byte)Item.Canal;
				}
				else if (npcScript[i] == originalTalk.Talk_Smith)
				{
					npcScript[i] = newTalk.Talk_TradeItems;
					npcScriptValue[i] = Blob.FromHex("15181600");
					npcScriptValue[i][3] = (byte)Item.Xcalber;
				}
			}

			// Replace sky warrior dialog that got taken over by "Nothing here".
			npcScriptValue[0x3D] = Blob.FromHex("00DB4C00");

			// Chime Lefein man is moved to ID 15 to keep him with all the other NPCs
			Put(MapObjGfxOffset + 0x0F, Blob.FromHex("0E"));
			Put(0x03400 + (int)MapId.Lefein * 48 + 0, Blob.FromHex("0F"));
			npcScript[0x0F] = newTalk.Talk_GiveItemOnFlag;
			npcScriptValue[0x0F] = Blob.FromHex("D0CECD00");
			npcScriptValue[0x0F][3] = (byte)Item.Chime;

			// Insert the updated talk scripts
			PutInBank(newTalkRoutinesBank, lut_MapObjTalkData, npcScriptValue.SelectMany(data => data.ToBytes()).ToArray());
			PutInBank(newTalkRoutinesBank, lut_MapObjTalkJumpTbl, npcScript.SelectMany(script => script.ToBytes()).ToArray());

			// Dialogue for Sarda if Early sarda is off
			newDialogs.Add(0xB3, "I shall help only\nthe true LIGHT WARRIORS.\nProve yourself by\ndefeating the Vampire.");

			// Update treasure box dialog for new DrawDialogueString routine
			newDialogs.Add(0xF0 + 0x50, "In the treasure box,\nyou found..\n#");

			InsertDialogs(newDialogs);

			// SHIP, BRIDGE, CANAL, CANOE and AIRSHIP text so they can be given by NPCs
			var gameVariableText =
				"9C91929900000000" + // SHIP
				"8A929B9C91929900" + // AIRSHIP
				"8B9B928D908E0000" + // BRIDGE
				"8C8A978A95000000" + // CANAL 4x8
				"00000000" + // +4
				"8C8A97988E00"; // CANOE
			Put(0x2B5D0, Blob.FromHex(gameVariableText));

			// Update to DrawDialogueString, see 1F_DB64_DrawDialogueString.asm
			var newDrawDialogueString = "AAA91085572003FEA9808597A567C9F0D0068AA2A04C7FDB8AA2002007FCA594853EA595853FA90A85172000FEA538853AA539853B2080DCA000B13EF0BEE63ED002E63FC91A904AC97A90168D0720A53A186901293F853A291FD0DC2080DC4C9CDB38E91AAA48BDA0F08D0720204EDC68AABD50F08D0720204EDCC617D0B920A1CC2069C62000FEA90A85172080DC4C9CDBC903D04EA53E48A53F48A90A85572003FEA5616920900D0A69D0853EA9B5853F184C2DDCA9B78597A561A2002007FCA594853EA595853F209CDBA91085572003FE68853F68853E4C9CDB0000000000000000205FDC4C9CDB";
			Put(0x7DB64, Blob.FromHex(newDrawDialogueString));
		}

		public string LocationText(MapLocation location, OverworldMap overworldmap)
		{
			Dictionary<MapLocation, OverworldTeleportIndex> StandardOverworldLocations =
			new Dictionary<MapLocation, OverworldTeleportIndex>
			{
				{MapLocation.Coneria,OverworldTeleportIndex.Coneria},
				{MapLocation.Caravan,(OverworldTeleportIndex)36},
				{MapLocation.Pravoka, OverworldTeleportIndex.Pravoka},
				{MapLocation.Elfland, OverworldTeleportIndex.Elfland},
				{MapLocation.Melmond, OverworldTeleportIndex.Melmond},
				{MapLocation.CrescentLake, OverworldTeleportIndex.CrescentLake},
				{MapLocation.Gaia,OverworldTeleportIndex.Gaia},
				{MapLocation.Onrac,OverworldTeleportIndex.Onrac},
				{MapLocation.Lefein,OverworldTeleportIndex.Lefein},
				{MapLocation.ConeriaCastle1,OverworldTeleportIndex.ConeriaCastle1},
				{MapLocation.ConeriaCastle2,OverworldTeleportIndex.ConeriaCastle1},
				{MapLocation.ConeriaCastleRoom1,OverworldTeleportIndex.ConeriaCastle1},
				{MapLocation.ConeriaCastleRoom2,OverworldTeleportIndex.ConeriaCastle1},
				{MapLocation.ElflandCastle,OverworldTeleportIndex.ElflandCastle},
				{MapLocation.ElflandCastleRoom1,OverworldTeleportIndex.ElflandCastle},
				{MapLocation.NorthwestCastle,OverworldTeleportIndex.NorthwestCastle},
				{MapLocation.NorthwestCastleRoom2,OverworldTeleportIndex.NorthwestCastle},
				{MapLocation.CastleOrdeals1,OverworldTeleportIndex.CastleOrdeals1},
				{MapLocation.CastleOrdealsMaze,OverworldTeleportIndex.CastleOrdeals1},
				{MapLocation.CastleOrdealsTop,OverworldTeleportIndex.CastleOrdeals1},
				{MapLocation.TempleOfFiends1,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends1Room1,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends1Room2,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends1Room3,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends1Room4,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends2,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends3,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsChaos,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsAir,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsEarth,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsFire,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsWater,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsPhantom,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.EarthCave1,OverworldTeleportIndex.EarthCave1},
				{MapLocation.EarthCave2,OverworldTeleportIndex.EarthCave1},
				{MapLocation.EarthCaveVampire,OverworldTeleportIndex.EarthCave1},
				{MapLocation.EarthCave4,OverworldTeleportIndex.EarthCave1},
				{MapLocation.EarthCaveLich,OverworldTeleportIndex.EarthCave1},
				{MapLocation.GurguVolcano1,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcano2,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcano3,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcano4,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcano5,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcano6,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcanoKary,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.IceCave1,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCave2,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCave3,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCave5,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCaveBackExit,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCaveFloater,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCavePitRoom,OverworldTeleportIndex.IceCave1},
				{MapLocation.SeaShrine1, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine2, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine2Room2, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine4, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine5, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine6, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine7, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine8, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrineKraken, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrineMermaids, (OverworldTeleportIndex)35},
				{MapLocation.Cardia1,OverworldTeleportIndex.Cardia1},
				{MapLocation.Cardia2,OverworldTeleportIndex.Cardia2},
				{MapLocation.BahamutCave1,OverworldTeleportIndex.BahamutCave1},
				{MapLocation.BahamutCave2,OverworldTeleportIndex.BahamutCave1},
				{MapLocation.Cardia4,OverworldTeleportIndex.Cardia4},
				{MapLocation.Cardia5,OverworldTeleportIndex.Cardia5},
				{MapLocation.Cardia6,OverworldTeleportIndex.Cardia6},
				{MapLocation.Waterfall,OverworldTeleportIndex.Waterfall},
				{MapLocation.DwarfCave,OverworldTeleportIndex.DwarfCave},
				{MapLocation.DwarfCaveRoom3,OverworldTeleportIndex.DwarfCave},
				{MapLocation.MatoyasCave,OverworldTeleportIndex.MatoyasCave},
				{MapLocation.SardasCave,OverworldTeleportIndex.SardasCave},
				{MapLocation.MarshCave1,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCave3,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCaveBottom,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCaveBottomRoom13,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCaveBottomRoom14,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCaveBottomRoom16,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCaveTop,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MirageTower1,OverworldTeleportIndex.MirageTower1},
				{MapLocation.MirageTower2,OverworldTeleportIndex.MirageTower1},
				{MapLocation.MirageTower3,OverworldTeleportIndex.MirageTower1},
				{MapLocation.SkyPalace1,(OverworldTeleportIndex)37},
				{MapLocation.SkyPalace2,(OverworldTeleportIndex)37},
				{MapLocation.SkyPalace3,(OverworldTeleportIndex)37},
				{MapLocation.SkyPalaceMaze,(OverworldTeleportIndex)37},
				{MapLocation.SkyPalaceTiamat,(OverworldTeleportIndex)37},
				{MapLocation.TitansTunnelEast,OverworldTeleportIndex.TitansTunnelEast},
				{MapLocation.TitansTunnelWest,OverworldTeleportIndex.TitansTunnelWest},
				{MapLocation.TitansTunnelRoom,OverworldTeleportIndex.TitansTunnelWest},
			};

			Dictionary<OverworldTeleportIndex, string> LocationNames = new Dictionary<OverworldTeleportIndex, string>
			{
				{OverworldTeleportIndex.Coneria,"Coneria"},
				{OverworldTeleportIndex.Pravoka,"Pravoka"},
				{OverworldTeleportIndex.Elfland,"Elfland"},
				{OverworldTeleportIndex.Melmond,"Melmond"},
				{OverworldTeleportIndex.CrescentLake,"Crescent Lake"},
				{OverworldTeleportIndex.Gaia,"Gaia"},
				{OverworldTeleportIndex.Onrac,"Onrac"},
				{OverworldTeleportIndex.Lefein,"Lefein"},
				{OverworldTeleportIndex.ConeriaCastle1,"Coneria Castle"},
				{OverworldTeleportIndex.ElflandCastle,"the Castle of Efland"},
				{OverworldTeleportIndex.NorthwestCastle,"Northwest Castle"},
				{OverworldTeleportIndex.CastleOrdeals1,"the Castle of Ordeals"},
				{OverworldTeleportIndex.TempleOfFiends1,"the Temple of Fiends"},
				{OverworldTeleportIndex.EarthCave1,"the Earth Cave"},
				{OverworldTeleportIndex.GurguVolcano1,"Gurgu Volcano"},
				{OverworldTeleportIndex.IceCave1,"the Ice Cave"},
				{OverworldTeleportIndex.Cardia1,"the Northernemost Cave of Cardia"}, // To check
				{OverworldTeleportIndex.Cardia2,"the Western plains of Cardia"},
				{OverworldTeleportIndex.BahamutCave1,"Bahamut's Cave"},
				{OverworldTeleportIndex.Cardia4,"the Marshes of Cardia"},
				{OverworldTeleportIndex.Cardia5,"the Tiny island's Cave of Cardia"}, // To check
				{OverworldTeleportIndex.Cardia6,"the Eastern forest of Cardia"},
				{OverworldTeleportIndex.Waterfall,"the Waterfall"},
				{OverworldTeleportIndex.DwarfCave,"the Dwarves' Cave"},
				{OverworldTeleportIndex.MatoyasCave,"Matoya's Cave"},
				{OverworldTeleportIndex.SardasCave,"Sarda's Cave"},
				{OverworldTeleportIndex.MarshCave1,"the Marsh Cave"},
				{OverworldTeleportIndex.MirageTower1,"the Mirage Tower"},
				{OverworldTeleportIndex.TitansTunnelEast,"the Titan's tunnel"},
				{OverworldTeleportIndex.TitansTunnelWest,"the Titan's tunnel"},
				{(OverworldTeleportIndex)35,"the Sea Shrine"},
				{(OverworldTeleportIndex)36,"the Caravan"},
				{(OverworldTeleportIndex)37,"the Sky Palace"},
			};

			var targetlocation = new OverworldTeleportIndex();
			if (overworldmap.OverriddenOverworldLocations != null && overworldmap.OverriddenOverworldLocations.Where(x => x.Key == location).Any())
				targetlocation = overworldmap.OverriddenOverworldLocations.Where(x => x.Key == location).First().Value;
			else
				targetlocation = StandardOverworldLocations.Where(x => x.Key == location).First().Value;

			return LocationNames.Where(x => x.Key == targetlocation).First().Value;
		}

		public static Dictionary<string, string> NiceNpcName = new Dictionary<string, string>
		{
			{ItemLocations.Astos.Name, "the kindly old King from Northwest Castle"},
			{ItemLocations.Bikke.Name, "Bikke the Pirate"},
			{ItemLocations.CanoeSage.Name, "the Sage from Crescent Lake"},
			{ItemLocations.CubeBot.Name, "a Robot in the Waterfall"},
			{ItemLocations.ElfPrince.Name, "the Elf Prince"},
			{ItemLocations.Fairy.Name, "a Fairy in a Bottle"},
			{ItemLocations.KingConeria.Name, "the King of Coneria"},
			{ItemLocations.Lefein.Name, "a man in Lefein"},
			{ItemLocations.Matoya.Name, "Matoya the Witch"},
			{ItemLocations.Nerrick.Name, "Nerrick the Dwarf"},
			{ItemLocations.Princess.Name, "the Princess of Coneria"},
			{ItemLocations.Sarda.Name, "Sarda the Sage"},
			{ItemLocations.Smith.Name, "the Blacksmith"},
		};
		public string FormatText(string text)
		{
			var tempstring = text.Split(' ');
			var tempchars = tempstring[0].ToArray();
			tempchars[0] = char.ToUpper(tempchars[0]);
			tempstring[0] = new String(tempchars);

			string[] templines = new string[7];
			int linenumber = 0;
			templines[0] = tempstring[0];

			for (int j = 1; j < tempstring.Length; j++)
			{
				if (templines[linenumber].Length + tempstring[j].Length > 23)
				{
					templines[linenumber] += "\n";
					linenumber++;
					templines[linenumber] += tempstring[j];
				}
				else
				{ templines[linenumber] += " " + tempstring[j]; }

				if (linenumber > 5) break;
			}

			text = string.Concat(templines);

			return text;
		}
		public List<Map> SetDungeonNPC(List<Map> maps, MT19337 rng, bool randomize)
		{
			// Earth modification
			List<List<byte>> earthmod = new List<List<byte>> {
				new List<byte> { 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E },
				new List<byte> { 0x3E, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41 },
				new List<byte> { 0x3E, 0x41, 0x00, 0x01, 0x02, 0x41, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E },
				new List<byte> { 0x3E, 0x41, 0x03, 0x2E, 0x05, 0x41, 0x3E, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38 },
				new List<byte> { 0x3E, 0x41, 0x06, 0x07, 0x08, 0x41, 0x3E, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38 },
				new List<byte> { 0x3E, 0x41, 0x30, 0x36, 0x30, 0x41, 0x3E, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38 },
				new List<byte> { 0x3E, 0x41, 0x41, 0x3A, 0x41, 0x41, 0x3E, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38 },
				new List<byte> { 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x3E, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38, 0x38 },
			};

			for (int y = 0; y < earthmod.Count; y++)
			{
				for (int x = 0; x < earthmod[y].Count; x++)
				{
					maps[(byte)MapId.EarthCaveB1][y + 35, x + 2] = earthmod[y][x];
				}
			}

			// Seashrine modification
			maps[(byte)MapId.SeaShrineB4][0x2E, 0x16] = 0x36;
			maps[(byte)MapId.SeaShrineB4][0x2F, 0x16] = 0x3A;

			// Palettes changes
			PutInBank(0x00, 0xA000 + ((byte)MapId.EarthCaveB1 * 0x30) + 0x18, Blob.FromHex("000F2736000F1A36"));
			PutInBank(0x00, 0xA000 + ((byte)MapId.SeaShrineB4 * 0x30) + 0x18, Blob.FromHex("0F1527360F241536"));
			PutInBank(0x00, 0xA000 + ((byte)MapId.SeaShrineB3 * 0x30) + 0x18, Blob.FromHex("0F1527360F241536"));
			PutInBank(0x00, 0xA000 + ((byte)MapId.GurguVolcanoB3 * 0x30) + 0x18, Blob.FromHex("000F1716000F1716"));
			PutInBank(0x00, 0xA000 + ((byte)MapId.GurguVolcanoB2 * 0x30) + 0x18, Blob.FromHex("000F1716000F1716"));
			PutInBank(0x00, 0xA000 + ((byte)MapId.MarshCaveB1 * 0x30) + 0x18, Blob.FromHex("000F1C34000F1834"));
			PutInBank(0x00, 0xA000 + ((byte)MapId.MarshCaveB2 * 0x30) + 0x18, Blob.FromHex("000F1C34000F1834"));
			PutInBank(0x00, 0xA000 + ((byte)MapId.MarshCaveB3 * 0x30) + 0x18, Blob.FromHex("000F1C34000F1834"));
			PutInBank(0x00, 0xA000 + ((byte)MapId.SkyPalace4F * 0x30) + 0x18, Blob.FromHex("0F0F18140F0F1714"));


			int randomposition = randomize ? rng.Between(1, 3) : 1;

			// Dwarf hinter - Earth - Text 0x70 - 0x5B - 5e
			SetNpc(MapId.DwarfCave, 5, ObjectId.None, 0x12, 0x18, false, false);

			if (randomposition == 1)
				SetNpc(MapId.EarthCaveB5, 0x0B, (ObjectId)0x5B, 0x12, 0x18, false, false);
			else if (randomposition == 2)
				SetNpc(MapId.EarthCaveB1, 0, (ObjectId)0x5B, 0x05, 0x26, true, false);
			else
				SetNpc(MapId.EarthCaveB3, 2, (ObjectId)0x5B, 0x09, 0x09, true, false);


			// Robot hinter - Sky - Tex 0xE1 - 0xCF
			SetNpc(MapId.SkyPalace3F, 0, ObjectId.None, 0x08, 0x1C, false, true);
			randomposition = randomize ? rng.Between(1, 3) : 1;
			if (randomposition == 1)
				SetNpc(MapId.SkyPalace3F, 0, (ObjectId)0xCF, 0x1B, 0x34, true, false);
			else if (randomposition == 2)
				SetNpc(MapId.MirageTower2F, 1, (ObjectId)0xCF, 0x08, 0x1C, false, true);
			else
			{
				var (x_robot, y_robot) = GetSkyCastleFloorTile(rng, maps[(byte)MapId.SkyPalace4F]);
				SetNpc(MapId.SkyPalace4F, 1, (ObjectId)0xCF, x_robot, y_robot, inRoom: false, stationary: false);
			}

			// Dragon hinter - Gurgu - Text 0xE3 - 0x86 dragon
			SetNpc(MapId.Cardia, 1, ObjectId.None, 0x2D, 0x1A, false, false);
			randomposition = randomize ? rng.Between(1, 3) : 1;
			if (randomposition == 1)
				SetNpc(MapId.GurguVolcanoB3, 1, (ObjectId)0x86, 0x04, 0x1D, false, false);
			else if (randomposition == 2)
				SetNpc(MapId.GurguVolcanoB2, 1, (ObjectId)0x86, 0x02, 0x02, true, false);
			else
				SetNpc(MapId.GurguVolcanoB5, 1, (ObjectId)0x86, 0x37, 0x08, true, false);
			//SetNpc(MapId.GurguVolcanoB3, 1, (ObjectId)0x86, 0x01, 0x02, false, true);

			// Punk hinter - Marsh - Text 0xAF - 0x9D punk
			SetNpc(MapId.Onrac, 11, ObjectId.None, 0x2D, 0x1A, false, false);
			randomposition = randomize ? rng.Between(1, 3) : 1;
			if (randomposition == 1)
				SetNpc(MapId.MarshCaveB1, 5, ObjectId.OnracPunk2, 0x2D, 0x1A, false, false);
			else if (randomposition == 2)
				SetNpc(MapId.MarshCaveB2, 0x0E, ObjectId.OnracPunk2, 0x0E, 0x34, true, false);
			else
				SetNpc(MapId.MarshCaveB2, 0x0D, ObjectId.OnracPunk2, 0x37, 0x21, true, false);

			// Mermaid hinter - Text 0xB6 - 0xA5 mermaid
			randomposition = randomize ? rng.Between(1, 3) : 1;
			if (randomposition == 1)
			{
				SetNpc(MapId.SeaShrineB1, 2, ObjectId.None, 0x00, 0x00, true, false);
				SetNpc(MapId.SeaShrineB4, 0, (ObjectId)0xA5, 0x16, 0x2C, true, false);
			}
			else if (randomposition == 2)
			{
				SetNpc(MapId.SeaShrineB1, 2, ObjectId.None, 0x00, 0x00, true, false);
				SetNpc(MapId.SeaShrineB3, 0, (ObjectId)0xA5, 0x1A, 0x11, true, false);
			}
			else
			{
				List<ObjectId> mermaids = new List<ObjectId> { ObjectId.Mermaid1, ObjectId.Mermaid2, ObjectId.Mermaid4, ObjectId.Mermaid5, ObjectId.Mermaid6, ObjectId.Mermaid7, ObjectId.Mermaid8, ObjectId.Mermaid9, ObjectId.Mermaid10 };
				var selectedMermaidId = mermaids.PickRandom(rng);
				var selectedMermaid = FindNpc(MapId.SeaShrineB1, selectedMermaidId);
				var hintMermaid = FindNpc(MapId.SeaShrineB1, ObjectId.Mermaid3);
				SetNpc(MapId.SeaShrineB1, selectedMermaid.Index, ObjectId.Mermaid3, selectedMermaid.Coord.x, selectedMermaid.Coord.y, selectedMermaid.InRoom, selectedMermaid.Stationary);
				SetNpc(MapId.SeaShrineB1, hintMermaid.Index, selectedMermaidId, hintMermaid.Coord.x, hintMermaid.Coord.y, hintMermaid.InRoom, hintMermaid.Stationary);
			}

			return maps;
		}
		public void NPCHints(MT19337 rng, Flags flags, OverworldMap overworldmap)
		{
			// Het all game dialogs, get all item names, set dialog templates
			var itemnames = ReadText(ItemTextPointerOffset, ItemTextPointerBase, ItemTextPointerCount);
			var hintschests = new List<string>() { "The $ is #.", "The $? It's # I believe.", "Did you know that the $ is #?", "My grandpa used to say 'The $ is #'.", "Did you hear? The $ is #!", "Wanna hear a secret? The $ is #!", "I've read somewhere that the $ is #.", "I used to have the $. I lost it #!", "I've hidden the $ #, can you find it?", "Interesting! This book says the $ is #!", "Duh, everyone knows that the $ is #!", "I saw the $ while I was #." };
			var hintsnpc = new List<string>() { "& has the $.", "The $? Did you try asking &?", "The $? & will never part with it!", "& stole the $ from ME! I swear!", "& told me not to reveal he has the $.", "& is hiding something. I bet it's the $!" };
			var hintsvendormed = new List<string>() { "The $ is for sale #.", "I used to have the $. I sold it #!", "There's a fire sale for the $ #.", "I almost bought the $ for sale #." };
			var uselesshints = new List<string>() { "GET A SILK BAG FROM THE\nGRAVEYARD DUCK TO LIVE\nLONGER.", "You spoony bard!", "Press A to talk\nto NPCs!", "A crooked trader is\noffering bum deals in\nthis town.", "The game doesn't start\nuntil you say 'yes'.", "Thieves run away\nreally fast.", "No, I won't move quit\npushing me.", "Dr. Unnes instant\ntranslation services,\njust send one slab\nand 299 GP for\nprocessing.", "I am error.", "Kraken has a good chance\nto one-shot your knight.", "If NPC guillotine is on,\npress reset now or your\nemulator will crash!", "GET EQUIPPED WITH\nTED WOOLSEY.", "8 and palm trees.\nGet it?" };

			if (!RedMageHasLife())
				uselesshints.Add("Red Mages have no life!");

			// Set item pool from flags, we only give hints for randomized items
			var incentivePool = new List<Item>();

			if (flags.Treasures ?? false)
			{
				incentivePool.Add(Item.Masamune);
				incentivePool.Add(Item.Katana);
				incentivePool.Add(Item.Vorpal);
				incentivePool.Add(Item.Defense);
				incentivePool.Add(Item.ThorHammer);
				incentivePool.Add(Item.Opal);
				incentivePool.Add(Item.PowerGauntlets);
				incentivePool.Add(Item.WhiteShirt);
				incentivePool.Add(Item.BlackShirt);
				incentivePool.Add(Item.Ribbon);
				incentivePool.Add(Item.Slab);
				incentivePool.Add(Item.Ruby);
				incentivePool.Add(Item.Floater);
				incentivePool.Add(Item.Tnt);
				incentivePool.Add(Item.Crown);
				incentivePool.Add(Item.Tail);
				incentivePool.Add(Item.Adamant);
			}

			if (flags.NPCItems ?? false)
			{
				incentivePool.Add(Item.Bridge);
				incentivePool.Add(Item.Lute);
				incentivePool.Add(Item.Ship);
				incentivePool.Add(Item.Rod);
				incentivePool.Add(Item.Canoe);
				incentivePool.Add(Item.Cube);
				incentivePool.Add(Item.Bottle);
			}

			if (flags.NPCFetchItems ?? false)
			{
				incentivePool.Add(Item.Key);
				incentivePool.Add(Item.Crystal);
				incentivePool.Add(Item.Oxyale);
				incentivePool.Add(Item.Canal);
				incentivePool.Add(Item.Herb);
				incentivePool.Add(Item.Chime);
				incentivePool.Add(Item.Xcalber);
			}

			if (flags.FreeAirship ?? false) incentivePool.Remove(Item.Floater);
			if (flags.FreeBridge ?? false) incentivePool.Remove(Item.Bridge);
			if (flags.FreeCanal ?? false) incentivePool.Remove(Item.Canal);
			if (flags.FreeLute ?? false) incentivePool.Remove(Item.Lute);
			if (flags.FreeShip ?? false) incentivePool.Remove(Item.Ship);
			if ((flags.FreeTail ?? false) || (flags.NoTail ?? false)) incentivePool.Remove(Item.Tail);
			
			if (incentivePool.Count == 0)
				incentivePool.Add(Item.Cabin);

			// Select NPCs from flags
			var priorityList = new List<Item> { Item.Lute, Item.Key, Item.Rod, Item.Oxyale, Item.Chime, Item.Cube, Item.Floater, Item.Canoe, Item.Ship, Item.Bridge, Item.Canal, Item.Bottle, Item.Slab, Item.Ruby, Item.Crown, Item.Crystal, Item.Herb, Item.Tnt, Item.Tail };

			var npcSelected = new List<ObjectId>();
			var dialogueID = new List<byte>();
			if (flags.HintsVillage ?? false)
			{
				npcSelected.AddRange(new List<ObjectId> { ObjectId.ConeriaOldMan, ObjectId.PravokaOldMan, ObjectId.ElflandScholar1, ObjectId.MelmondOldMan2, ObjectId.CrescentSage11, ObjectId.OnracOldMan2, ObjectId.GaiaWitch, ObjectId.LefeinMan12 });
				dialogueID.AddRange(new List<byte> { 0x45, 0x53, 0x69, 0x82, 0xA0, 0xAA, 0xCB, 0xDC });
				MoveNpc(MapId.Lefein, 0x0C, 0x0E, 0x15, false, true);
			}

			if (flags.HintsDungeon ?? false)
			{
				npcSelected.AddRange(new List<ObjectId> { ObjectId.OnracPunk2, ObjectId.DwarfcaveDwarf4, ObjectId.CardiaDragon2, ObjectId.SkyRobot, ObjectId.Mermaid3 });
				dialogueID.AddRange(new List<byte> { 0x9D, 0x70, 0xE3, 0xE1, 0xB6 });
			}

			var incentivizedChests = new List<string>();

			if (flags.IncentivizeEarth ?? false) incentivizedChests.Add(ItemLocations.EarthCaveMajor.Name);
			if (flags.IncentivizeIceCave ?? false) incentivizedChests.Add(ItemLocations.IceCaveMajor.Name);
			if (flags.IncentivizeMarsh ?? false) incentivizedChests.Add(ItemLocations.MarshCaveMajor.Name);
			if (flags.IncentivizeMarshKeyLocked ?? false) incentivizedChests.Add(ItemLocations.MarshCave13.Name);
			if (flags.IncentivizeOrdeals ?? false) incentivizedChests.Add(ItemLocations.OrdealsMajor.Name);
			if (flags.IncentivizeSeaShrine ?? false) incentivizedChests.Add(ItemLocations.SeaShrineMajor.Name);
			if (flags.IncentivizeSkyPalace ?? false) incentivizedChests.Add(ItemLocations.SkyPalaceMajor.Name);
			if (flags.IncentivizeTitansTrove ?? false) incentivizedChests.Add(ItemLocations.TitansTunnel1.Name);
			if (flags.IncentivizeVolcano ?? false) incentivizedChests.Add(ItemLocations.VolcanoMajor.Name);
			if (flags.IncentivizeConeria ?? false) incentivizedChests.Add(ItemLocations.ConeriaMajor.Name);

			var hintedItems = new List<Item>();
			foreach (Item priorityitem in priorityList)
			{
				if (generatedPlacement.Find(x => x.Item == priorityitem) != null)
					if (generatedPlacement.Find(x => x.Item == priorityitem).GetType().Equals(typeof(TreasureChest)) && !incentivizedChests.Contains(generatedPlacement.Find(x => x.Item == priorityitem).Name))
						hintedItems.Add(priorityitem);

				if (hintedItems.Count == npcSelected.Count)
					break;
			}

			while (hintedItems.Count < npcSelected.Count)
			{
				var tempItem = incentivePool.PickRandom(rng);
				if (generatedPlacement.Find(x => x.Item == tempItem) != null)
					hintedItems.Add(tempItem);
			}

			// Declare hints string for each hinted at item
			var hintsList = new List<string>();

			// Create hint for a random item in the pool for each NPC
			var attempts = 0;
			while (++attempts < 50)
			{
				for (int i = 0; i < npcSelected.Count; i++)
				{
					var tempItem = hintedItems.First();
					string tempHint;
					string tempName;

					if (tempItem.Equals(Item.Ship)) tempName = FF1Text.BytesToText(Get(0x2B5D0, 4));
					//else if (tempRndItem.Equals(Item.Airship)) tempName = FF1Text.BytesToText((byte[])Get(0x285C+8, 7));
					else if (tempItem.Equals(Item.Bridge)) tempName = FF1Text.BytesToText(Get(0x2B5D0 + 16, 6));
					else if (tempItem.Equals(Item.Canal)) tempName = FF1Text.BytesToText(Get(0x2B5D0 + 24, 5));
					else if (tempItem.Equals(Item.Canoe)) tempName = FF1Text.BytesToText(Get(0x2B5D0 + 36, 5));
					else tempName = itemnames[(int)tempItem].Replace(" ", "");

					if (generatedPlacement.Find(x => x.Item == tempItem).GetType().Equals(typeof(TreasureChest)))
					{
						tempHint = hintschests.PickRandom(rng);
						tempHint = tempHint.Split('$')[0] + tempName + tempHint.Split('$')[1];

						tempHint = FormatText(tempHint.Split('#')[0] + "in " + LocationText(generatedPlacement.Find(x => x.Item == tempItem).MapLocation, overworldmap) + tempHint.Split('#')[1]);
						hintsList.Add(tempHint);
						hintedItems.RemoveRange(0, 1);
					}
					else if (generatedPlacement.Find(x => x.Item == tempItem).GetType().Equals(typeof(MapObject)))
					{
						tempHint = hintsnpc.PickRandom(rng);
						tempHint = tempHint.Split('$')[0] + tempName + tempHint.Split('$')[1];
						tempHint = FormatText(tempHint.Split('&')[0] + NiceNpcName[generatedPlacement.Find(x => x.Item == tempItem).Name] + tempHint.Split('&')[1]);
						hintsList.Add(tempHint);
						hintedItems.RemoveRange(0, 1);
					}
					else if (generatedPlacement.Find(x => x.Item == tempItem).GetType().Equals(typeof(ItemShopSlot)))
					{
						tempHint = hintsvendormed.PickRandom(rng);
						tempHint = tempHint.Split('$')[0] + tempName + tempHint.Split('$')[1];
						tempHint = FormatText(tempHint.Split('#')[0] + ((generatedPlacement.Find(x => x.Item == tempItem).MapLocation.Equals(MapLocation.Caravan)) ? "at " : "in ") + LocationText(generatedPlacement.Find(x => x.Item == tempItem).MapLocation, overworldmap) + tempHint.Split('#')[1]);
						hintsList.Add(tempHint);
						hintedItems.RemoveRange(0, 1);
					}
					else
						tempHint = "I am error.";
				}

				if (flags.HintsUseless != false)
				{
					uselesshints.Shuffle(rng);
					hintsList.Reverse();
					var uselessHintsCount = hintsList.Count() / 2;
					hintsList.RemoveRange(0, uselessHintsCount);

					for (int i = 0; i < uselessHintsCount; i++)
						hintsList.Add(uselesshints[i]);
				}
				//var hintsList = hints.ToList();
				hintsList.Shuffle(rng);

				Dictionary<int, string> hintDialogues = new Dictionary<int, string>();

				// Set NPCs new dialogs
				for (int i = 0; i < npcSelected.Count; i++)
				{
					PutInBank(newTalkRoutinesBank, lut_MapObjTalkJumpTbl + (byte)npcSelected[i] * NpcTalkSize, newTalk.Talk_norm);
					PutInBank(newTalkRoutinesBank, lut_MapObjTalkData + (byte)npcSelected[i] * MapObjSize, Blob.FromSBytes(new sbyte[] { 0x00, (sbyte)dialogueID[i], 0x00, 0x00 }));

					hintDialogues.Add(dialogueID[i], hintsList.First());
					hintsList.RemoveRange(0, 1);
				}

				try
				{
					InsertDialogs(hintDialogues);
				}
				catch (Exception e)
				{
					if (e == new Exception("Dialogs maximum length exceeded."))
						continue;
				}
				if(attempts > 1) Console.WriteLine($"NPC Hints generated in {attempts} attempts.");
				return;
			}
			throw new Exception("Couldn't generate hints in 50 tries.");
		}
	}
}
