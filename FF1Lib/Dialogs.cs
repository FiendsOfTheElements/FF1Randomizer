using System;
using System.Collections.Generic;
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
			public static readonly Blob Talk_Nerrick = Blob.FromHex("E593");
			public static readonly Blob Talk_Bahamut = Blob.FromHex("0594");
			public static readonly Blob Talk_ElfDocUnne = Blob.FromHex("2594");
			public static readonly Blob Talk_GiveItemOnFlag = Blob.FromHex("4594");
			public static readonly Blob Talk_TradeItems = Blob.FromHex("7394");
			public static readonly Blob Talk_GiveItemOnItem = Blob.FromHex("A394");
			public static readonly Blob Talk_Astos = Blob.FromHex("CF94");
			public static readonly Blob Talk_kill = Blob.FromHex("0995");
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

		// Required for npc quest item randomizing
		// Doesn't substantially change anything if EnableNPCsGiveAnyItem isn't called
		public void CleanupNPCRoutines()
		{
			// Swap string pointer in index 2 and 3 for King, Bikke, Prince, and Lefein
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
		public void FormatItems()
		{
			// Format items name so they look nicer in text boxes
			var itemNames = ReadText(FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, 256);
			Blob itemNamesBlob = Blob.FromHex("");

			for (int i = 1; i < 18; i++)
			{
				byte[] byteitemname = new byte[8];
				itemNames[i] = itemNames[i].TrimEnd(' ');
				Array.Copy(FF1Text.TextToBytes(itemNames[i], false), byteitemname, itemNames[i].Length);
				itemNamesBlob = Blob.Concat(itemNamesBlob, byteitemname);
			}

			for (int i = 18; i < 22; i++)
			{
				byte[] byteitemname = new byte[2];
				itemNames[i] = " ";
				Array.Copy(FF1Text.TextToBytes(itemNames[i], false), byteitemname, itemNames[i].Length);
				itemNamesBlob = Blob.Concat(itemNamesBlob, byteitemname);
			}
			for (int i = 22; i < 23; i++)
			{
				byte[] byteitemname = new byte[8];
				itemNames[i] = itemNames[i].TrimEnd(' ') + " ";
				Array.Copy(FF1Text.TextToBytes(itemNames[i], false), byteitemname, itemNames[i].Length);
				itemNamesBlob = Blob.Concat(itemNamesBlob, byteitemname);
			}

			for (int i = 23; i < 25; i++)
			{
				byte[] byteitemname = new byte[8];
				itemNames[i] = itemNames[i].TrimEnd(' ');
				Array.Copy(FF1Text.TextToBytes(itemNames[i], false), byteitemname, itemNames[i].Length);
				itemNamesBlob = Blob.Concat(itemNamesBlob, byteitemname);
			}
			for (int i = 25; i < 27; i++)
			{
				byte[] byteitemname = new byte[6];
				itemNames[i] = itemNames[i].TrimEnd(' ');
				Array.Copy(FF1Text.TextToBytes(itemNames[i], false), byteitemname, itemNames[i].Length);
				itemNamesBlob = Blob.Concat(itemNamesBlob, byteitemname);
			}
			for (int i = 27; i < 99; i++)
			{
				byte[] byteitemname = new byte[8];
				itemNames[i] = itemNames[i].TrimEnd(' ');
				Array.Copy(FF1Text.TextToBytes(itemNames[i], false), byteitemname, itemNames[i].Length);
				itemNamesBlob = Blob.Concat(itemNamesBlob, byteitemname);
			}
			for (int i = 99; i < 100; i++)
			{
				byte[] byteitemname = new byte[9];
				itemNames[i] = itemNames[i].TrimEnd(' ');
				Array.Copy(FF1Text.TextToBytes(itemNames[i], false), byteitemname, itemNames[i].Length);
				itemNamesBlob = Blob.Concat(itemNamesBlob, byteitemname);
			}
			for (int i = 100; i < 108; i++)
			{
				byte[] byteitemname = new byte[8];
				itemNames[i] = itemNames[i].TrimEnd(' ');
				Array.Copy(FF1Text.TextToBytes(itemNames[i], false), byteitemname, itemNames[i].Length);
				itemNamesBlob = Blob.Concat(itemNamesBlob, byteitemname);
			}
			Put(ItemTextOffset + 1, itemNamesBlob);
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

			NPCShuffleDialogs.Add(0x02, "Thank you for saving the\nPrincess. To aid your\nquest, please take this.\n\n\nReceived #.");
			NPCShuffleDialogs.Add(0x06, "This heirloom has been\npassed down from Queen\nto Princess for 2000\nyears. Please take it.\n\nReceived #.");
			NPCShuffleDialogs.Add(0x09, "Okay, you got me.\nTake my #.\n\n\n\nReceived #.");
			NPCShuffleDialogs.Add(0x0E, "Is this a dream?.. Are\nyou the LIGHT WARRIORS?\nSo, as legend says,\nI give you this.\n\nReceived #.");
			NPCShuffleDialogs.Add(0x12, "HA, HA, HA! I am Astos,\nKing of the Dark Elves.\nI have the #,\nand you shall give me\nthat CROWN, now!!!");
			NPCShuffleDialogs.Add(0x14, "Yes, yes indeed,\nthis TNT is just what I\nneed to finish my work.\nTake this in return!\n\nReceived #.");
			NPCShuffleDialogs.Add(0x16, "ADAMANT!! Now let me\nforge this for you..\nHere, the best work\nI've ever done.\n\nReceived #.");
			NPCShuffleDialogs.Add(0x19, "I'll trade my most\npowerful charm to get\nmy CRYSTAL back..\nOh! I can see!!\n\nReceived #.");
			NPCShuffleDialogs.Add(0x1E, "Take this.\nIt will help you\nfight the source of the\nearth's rot.\n\nReceived #.");
			NPCShuffleDialogs.Add(0x23, "That pirate trapped me\nin the BOTTLE. I will\nget what's at the bottom\nof the spring for you.\n\nReceived #.");
			NPCShuffleDialogs.Add(0x27, "Take this.\n\n\n\n\nReceived the #.");
			NPCShuffleDialogs.Add(0x2B, "Great job vanquishing\nthe Earth FIEND.\nWith this, go and defeat\nthe other FIENDS!\n\nReceived #.");
			NPCShuffleDialogs.Add(0xCD, "With this, you can\navenge the SKY WARRIORS.\n\n\n\nReceived #.");

			InsertDialogs(NPCShuffleDialogs);
		}
		public void UpdateDialogs()
		{
			Dictionary<int, string> newDialogs = new Dictionary<int, string>();

			FormatItems();
			CleanupNPCRoutines();
			SplitOpenTreasureRoutine();
			TransferDialogs();

			// Get all NPC scripts and script values to update them
			var npcScript = new List<Blob>();
			var npcScriptValue = new List<Blob>();

			for (int i = 0; i < 0xD0; i++)
			{
				npcScriptValue.Add(Get(MapObjOffset + i * 4, 4));
				npcScript.Add(Get(0x390D3 + i * 2, 2));
			}

			// Insert new dialogs Talk routine, see 0E_9296_TalkRoutines.asm
			var newTalkRoutines = "60A51160A410209190B003A51160A51260A5106920A8B90060F003A51160A51260A012209190B008AD2160D003A51160A51260A410207990B003A51160A51260A0122091909008AD0860D003A51160A51260AD32602D33602D34602D3160F003A51160A51260AD2560F008AD2660D003A51160A51260A00C209190B008AD3160D003A51160A51260AD3160F008AD3260D003A51160A51260A416209690A41320A490A51160A416209690A51160A416207F90209690A51320C590A51160AD32602D33602D34602D3160F00CA0CA209690E67DE67DA51160A51260A416207F90209690A01220A490A93F20CC90A51160AD3060D003A51160A416209690A51260AD2960D003A51160CE2960A416209690A512E67D60A03F209190B01520A490A04020A490A04120A490A97E20C590A51160A416207990B015A513F011A41284132093DDB00AA416207F90A51360A51060AD2660F018A513F014A41284132093DDB00DCE2660A416207392A51360A51060A00E2079909003A51360AD2D60D003A51160CE2D60207F9020AE95E67DA51260A416207990B013BE8095BD2060F00EDE2060207F90E67DA51260A51160A51060A416207990B021B98095F006A82079909019A513F015A4128414182093DDB00DA416207F90A51460A51160A51060A416207990B023BE8095BD2060F01EA513F01AA41284142093DDB013A416BE8095DE2060207F90A51460A51160A51060A416207990B0F3BE8095F005BD2060F018A513F014A41284142093DDB00DA416207F90A51460A51160A51060A416BE8095F006EABD2060F02AA513F026A4128414182093DDB01E8467A5108D0E03A5118D0F03A416207F90207392A97D20C590A51460A51060A000209690A51160";
			PutInBank(0x0E, 0x9296, Blob.FromHex(newTalkRoutines));

			// Lut for required items check, only the first 32 NPCs can give an item with this, but it should be enough for now
			PutInBank(0x0E, 0x9580, Blob.FromHex("000300000004050206070308000C0D0B10000000091100000000000000000000"));

			// Update all NPC's dialogs script, default behaviours are maintained
			for (int i = 0; i < 0xD0; i++)
			{
				if (npcScript[i] == originalTalk.Talk_4Orb)
				{
					npcScript[i] = newTalk.Talk_4Orb;
					npcScriptValue[0x3D] = Blob.FromHex("00DB4C00"); // replace sky warrior dialog that got taken over by nothing here.
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
					newDialogs.Add(0xB3, "I shall help only\nthe true LIGHT WARRIORS.\nProve yourself by\ndefeating the Vampire.");
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

			// Chime Lefein man is moved to ID 15 to keep him with all the other NPCs
			Put(MapObjGfxOffset + 0x0F, Blob.FromHex("0E"));
			Put(0x03400 + (int)MapId.Lefein * 48 + 0, Blob.FromHex("0F"));
			npcScript[0x0F] = newTalk.Talk_GiveItemOnFlag;
			npcScriptValue[0x0F] = Blob.FromHex("D0CECD00");
			npcScriptValue[0x0F][3] = (byte)Item.Chime;

			// Insert the updated talk scripts
			for (int i = 0; i < 0xD0; i++)
			{
				PutInBank(0x0E, 0x90D3 + i * 2, npcScript[i]);
				PutInBank(0x0E, 0x95D5 + i * 4, npcScriptValue[i]);
			}



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
	}
}
