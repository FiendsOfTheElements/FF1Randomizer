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
		public enum newTalkRoutines
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
			Talk_Chaos
		}
		public class TalkRoutines
		{
			private List<Blob> _talkroutines = new List<Blob>();
			private int TalkRoutinesOffset = 0x8200;
			private int lut_MapObjTalkJumpTbl_new = 0x8000;

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
				_talkroutines.Add(Blob.FromHex("A571203D96A575200096A476207F902073922018964C439660"));
				_talkroutines.Add(Blob.FromHex("AD32602D33602D34602D3160F00CA0CA209690E67DE67DA57160A57260"));
				_talkroutines.Add(Blob.FromHex("A476207F90209690A01220A490A93F20CC90A57160"));
				_talkroutines.Add(Blob.FromHex("AD3060D003A57160A476209690A57260"));
				_talkroutines.Add(Blob.FromHex("AD2960D003A57160CE2960A476209690A572E67D60"));
				_talkroutines.Add(Blob.FromHex("A03F209190B01FA571203D96A575200096A03F20A490A04020A490A04120A4902018964C4396A476207990B012A573F00E1820109FB00AA476207F90A57260A57060")); // Talk_Bikke
				_talkroutines.Add(Blob.FromHex("AD2660F018A573F0141820109FB010CE2660A476207F90207392A57260A57060")); // Talk_Nerrick
				_talkroutines.Add(Blob.FromHex("A00E2079909003A57360AD2D60D003A57160CE2D60207F9020AE95E67DA57260"));
				_talkroutines.Add(Blob.FromHex("A476207990B012A674BD2060F00EDE2060207F90E67DA57260A57160A57060"));
				_talkroutines.Add(Blob.FromHex("A476207990B01BA474F0052079909015A573F0111820109FB00DA476207F90A57260A57160A57060"));
				_talkroutines.Add(Blob.FromHex("A476207990B01EA674BD2060F01AA573F0161820109FB012A674DE2060A476207F90A57260A57160A57060"));
				_talkroutines.Add(Blob.FromHex("A476207990B01BA674F005BD2060F015A573F0111820109FB00DA476207F90A57260A57160A57060"));
				_talkroutines.Add(Blob.FromHex("A674F005BD2060F029A5738561202096F022E67DA572203D96A575200096A476207F90207392A5611820109F2018964C4396A57060")); // Talk_Astos
				_talkroutines.Add(Blob.FromHex("A000209690A57160")); // Talk_Kill
				_talkroutines.Add(Blob.FromHex("A57520C590A57160")); // Talk_Chaos
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
			public void Replace(newTalkRoutines oldroutine, Blob newroutine)
			{
				_talkroutines[(int)oldroutine] = newroutine;
			}
			public void ReplaceChunk(newTalkRoutines oldroutine, Blob search, Blob replace)
			{
				_talkroutines[(int)oldroutine].ReplaceInPlace(search, replace);
			}
			public int GetAddress(int talkid)
			{
				var pos = 0;
				for (int i = 0; i < talkid; i++)
				{
					pos += _talkroutines[i].Length;
				}

				return TalkRoutinesOffset + pos;
			}
			public Blob GetAddressLE(int talkid)
			{
				var address = GetAddress(talkid);
				sbyte[] addressLE = new sbyte[] { (sbyte)(address % 0x100), (sbyte)(address / 0x100) };

				return Blob.FromSBytes(addressLE);
			}
			public void WriteRoutines(FF1Rom rom)
			{
				rom.PutInBank(newTalkRoutinesBank, TalkRoutinesOffset, Blob.FromSBytes(_talkroutines.SelectMany(talk => talk.ToSBytes()).ToArray()));
			}

			public void UpdateNPCRoutines(FF1Rom rom, NPCdata npcdata)
			{
				var tempblob = new byte[] { };

				for (int i = 0; i < npcdata.GetNPCCount(); i++)
				{
					tempblob = tempblob.Concat(GetAddressLE((int)npcdata.GetRoutine((ObjectId)i)).ToBytes()).ToArray();
				}

				rom.PutInBank(newTalkRoutinesBank, lut_MapObjTalkJumpTbl_new, tempblob);
			}
		}

		public class generalNPC
		{
			public byte sprite;
			public Blob oldtalkroutine;
			public newTalkRoutines talkroutine;
			public byte[] talkarray;
		}
		public class NPCdata
		{
			private List<generalNPC> _npcs = new List<generalNPC>();

			public NPCdata(FF1Rom rom)
			{
				for (int i = 0; i < 0xD0; i++)
				{
					_npcs.Add(new generalNPC { sprite = rom.Data[MapObjGfxOffset + i],
						oldtalkroutine = rom.GetFromBank(oldTalkRoutinesBank, lut_MapObjTalkJumpTbl + i * 2, 2),
						talkroutine = newTalkRoutines.Talk_None,
						talkarray = rom.GetFromBank(oldTalkRoutinesBank, lut_MapObjTalkData + 0x04 * i, 4).ToBytes().Concat(new byte[] { 0x00, 0x00 }).ToArray()
					});
				}
			}
			public void SetSprite(ObjectId targetobject, byte targetsprite)
			{
				_npcs[(int)targetobject].sprite = targetsprite;
			}
			public Blob GetOldRoutine(ObjectId targetobject)
			{
				return _npcs[(int)targetobject].oldtalkroutine;
			}
			public void SetRoutine(ObjectId targetobject, newTalkRoutines targettalkroutine)
			{
				_npcs[(int)targetobject].talkroutine = targettalkroutine;
			}
			public newTalkRoutines GetRoutine(ObjectId targetobject)
			{
				return _npcs[(int)targetobject].talkroutine;
			}
			public byte[] GetTalkArray(ObjectId targetobject)
			{
				return _npcs[(int)targetobject].talkarray;
			}
			public int GetNPCCount()
			{
				return _npcs.Count;
			}
			public void UpdateItemPlacement(List<IRewardSource> itemplacement)
			{
				if (itemplacement == null) // Return if vanilla placement
					return;

				var targetnpc = itemplacement.Where(x => x.GetType().Equals(typeof(MapObject)));

				foreach (var item in targetnpc)
				{
					switch (item.Name)
					{
						case "Astos":
							_npcs[(int)ObjectId.Astos].talkarray[(int)TalkArrayPos.item_id] = (byte)item.Item;
							break;
						case "CubeBot":
							_npcs[(int)ObjectId.CubeBot].talkarray[(int)TalkArrayPos.item_id] = (byte)item.Item;
							break;
						case "King":
							_npcs[(int)ObjectId.King].talkarray[(int)TalkArrayPos.item_id] = (byte)item.Item;
							break;
						case "Princess2":
							_npcs[(int)ObjectId.Princess2].talkarray[(int)TalkArrayPos.item_id] = (byte)item.Item;
							break;
						case "Matoya":
							_npcs[(int)ObjectId.Matoya].talkarray[(int)TalkArrayPos.item_id] = (byte)item.Item;
							break;
						case "Bikke":
							_npcs[(int)ObjectId.Bikke].talkarray[(int)TalkArrayPos.item_id] = (byte)item.Item;
							break;
						case "ElfPrince":
							_npcs[(int)ObjectId.ElfPrince].talkarray[(int)TalkArrayPos.item_id] = (byte)item.Item;
							break;
						case "Sarda":
							_npcs[(int)ObjectId.Sarda].talkarray[(int)TalkArrayPos.item_id] = (byte)item.Item;
							break;
						case "CanoeSage":
							_npcs[(int)ObjectId.CanoeSage].talkarray[(int)TalkArrayPos.item_id] = (byte)item.Item;
							break;
						case "Fairy":
							_npcs[(int)ObjectId.Fairy].talkarray[(int)TalkArrayPos.item_id] = (byte)item.Item;
							break;
						case "Lefein":
							_npcs[(int)ObjectId.Lefein].talkarray[(int)TalkArrayPos.item_id] = (byte)item.Item;
							break;
						case "Nerrick":
							_npcs[(int)ObjectId.Nerrick].talkarray[(int)TalkArrayPos.item_id] = (byte)item.Item;
							break;
						case "Smith":
							_npcs[(int)ObjectId.Smith].talkarray[(int)TalkArrayPos.item_id] = (byte)item.Item;
							break;
					}
				}
			}
			public void WriteNPCdata(FF1Rom rom)
			{
				var lut_MapObjTalkData_move = 0xBA00;
				rom.PutInBank(newTalkRoutinesBank, lut_MapObjTalkData_move, _npcs.SelectMany(data => data.talkarray).ToArray());
			}
		}
		public enum TalkArrayPos
		{
			dialogue_1 = 0,
			dialogue_2 = 1,
			dialogue_3 = 2,
			item_id = 3,
			requirement_id = 4,
			battle_id = 5
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

			// New utilities for talk routines, see 11_8200_TalkRoutines.asm
			PutInBank(newTalkRoutinesBank, 0x9600, Blob.FromHex("856A20CDD8A9008D01208D1540A002204A96A001204A9660A003204A964C0FE4C96CB015C944B00CC91C900D2034DDB0094C39962046DDB00118A9F160A000204A9660686868684CB6C88510A91148A9FE48A90648B9609648B9649648A51060CAF1E8CF02FFFF39"));

			// TalkToObject Upgraded, see 11_8200_TalkRoutines.asm
			PutInBank(newTalkRoutinesBank, 0x901B, Blob.FromHex("BD006F8576A00084150A26151865769002E6150A261569008514A9BA651585158612A900AAA8B1149570C8E8E006D0F6A612A5760AA8900DB900818516B9018185176C1600B900808516B9018085176C1600"));
			Data[0x7C9F7] = 0x1B;

			// LoadPrice fix
			PutInBank(newTalkRoutinesBank, 0x9F10, Blob.FromHex("A9118558A5734C93DD"));

			// Update bank
			Data[0x7C9F2] = newTalkRoutinesBank;
		}

		// Required for npc quest item randomizing
		// Doesn't substantially change anything if EnableNPCsGiveAnyItem isn't called
		// Deprecated, delete if no bugs comes up - 2020-12-12
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
			NPCShuffleDialogs.Add(0xF0, "Received #");

			InsertDialogs(NPCShuffleDialogs);
		}
		public void UpdateDialogs(NPCdata npcdata)
		{
			Dictionary<int, string> newDialogs = new Dictionary<int, string>();

			//CleanupNPCRoutines(); - Deprecated 2020-12-12
			SplitOpenTreasureRoutine();
			TransferDialogs();
			TransferTalkRoutines();
			AddNewChars();

			// Update all NPC's dialogs script, default behaviours are maintained
			for (int i = 0; i < 0xD0; i++)
			{
				if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_4Orb)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_4Orb);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Astos)
				{
					//newDialogs.Add(0xBF, "I'm not quite dead yet.");
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_Astos);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1] = npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2];
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2] = 0x18;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.item_id] = (byte)Item.Crystal;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.requirement_id] = (byte)Item.Crown;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.battle_id] = 0x7D;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Bahamut)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_Bahamut);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Bikke)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_Bikke);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1] = 0x0A;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.item_id] = (byte)Item.Ship;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.battle_id] = 0x7E;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_BlackOrb)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_BlackOrb);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_CanoeSage)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_GiveItemOnItem);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1] = npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3];
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3] = npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2];
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2] = npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1];
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.item_id] = (byte)Item.Canoe;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.requirement_id] = (byte)Item.EarthOrb;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Chime)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_GiveItemOnFlag);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.item_id] = (byte)Item.Chime;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.requirement_id] = (byte)ObjectId.Unne;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.battle_id] = 0x7D;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_CoOGuy)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_CoOGuy);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_CubeBot)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_GiveItemOnFlag);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1] = 0x00;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2] = 0x28;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3] = 0x27;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.item_id] = (byte)Item.Cube;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_ElfDoc)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_ElfDocUnne);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1] = 0x0B;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2] = 0x0D;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3] = 0x0C;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.requirement_id] = (byte)Item.Herb;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_ElfPrince)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_GiveItemOnFlag);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1] = 0x10;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2] = 0x0F;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3] = 0x0E;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.item_id] = (byte)Item.Key;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.requirement_id] = (byte)ObjectId.ElfDoc;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Fairy)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_GiveItemOnFlag);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1] = 0x00;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2] = 0x24;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3] = 0x23;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.item_id] = (byte)Item.Oxyale;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_fight)
				{
					if (npcdata.GetTalkArray((ObjectId)i)[3] == 0x7B)
						npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_Chaos);
					else
						npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_fight);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.battle_id] = npcdata.GetTalkArray((ObjectId)i)[3];
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Garland)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_fight);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.battle_id] = 0x7F;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_GoBridge)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_GoBridge);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_ifairship)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_ifitem);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.requirement_id] = 0xE4;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_ifbridge)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_ifitem);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.requirement_id] = (byte)Item.Bridge;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_ifcanal)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_ifitem);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.requirement_id] = (byte)Item.Canal;
					var a = npcdata.GetTalkArray((ObjectId)i)[1];
					npcdata.GetTalkArray((ObjectId)i)[1] = npcdata.GetTalkArray((ObjectId)i)[2];
					npcdata.GetTalkArray((ObjectId)i)[2] = a;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_ifearthvamp)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_ifearthvamp);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_ifevent)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_ifevent);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_ifitem)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_ifitem);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_ifkeytnt)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_ifkeytnt);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_ifearthfire)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_ifearthfire);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_ifvis)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_ifvis);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Invis)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_Invis);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_KingConeria)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_GiveItemOnFlag);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1] = 0x01;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2] = 0x03;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3] = 0x02;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.item_id] = (byte)Item.Bridge;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.requirement_id] = (byte)ObjectId.Princess1;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Matoya)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_TradeItems);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1] = 0x17;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2] = 0x1A;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3] = 0x19;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.item_id] = (byte)Item.Herb;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.requirement_id] = (byte)Item.Crystal;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_None)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_None);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_norm)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_norm);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Princess1)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_Princess1);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Princess2)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_GiveItemOnFlag);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1] = 0x00;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2] = 0x07;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3] = 0x06;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.item_id] = (byte)Item.Lute;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Replace)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_Replace);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Sarda)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_GiveItemOnFlag);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1] = 0xB3;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2] = 0x18;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3] = 0x1E;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.item_id] = (byte)Item.Rod;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.requirement_id] = (byte)ObjectId.Vampire;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_SubEng)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_SubEng);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Titan)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_Titan);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Unne)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_ElfDocUnne);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1] = 0x1B;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2] = 0x18;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3] = 0x1C;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.requirement_id] = (byte)Item.Slab;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Unused)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_None);
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Vampire)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_fight);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.battle_id] = 0x7C;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_ifcanoe)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_ifitem);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.requirement_id] = (byte)Item.Canoe;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Nerrick)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_Nerrick);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1] = 0x13;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2] = 0x00;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3] = 0x14;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.item_id] = (byte)Item.Canal;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.requirement_id] = (byte)Item.Tnt;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Smith)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_TradeItems);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1] = 0x15;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2] = 0x18;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3] = 0x16;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.item_id] = (byte)Item.Xcalber;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.requirement_id] = (byte)Item.Adamant;
				}
				else
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_None);
			}

			// Replace sky warrior dialog that got taken over by "Nothing here".
			npcdata.GetTalkArray((ObjectId)0x3D)[(int)TalkArrayPos.dialogue_2] = 0xDB;
			npcdata.GetTalkArray((ObjectId)0x3D)[(int)TalkArrayPos.dialogue_3] = 0x4C;

			// Chime Lefein man is moved to ID 15 to keep him with all the other NPCs
			Put(MapObjGfxOffset + 0x0F, Blob.FromHex("0E"));
			Put(0x03400 + (int)MapId.Lefein * 48 + 0, Blob.FromHex("0F"));
			npcdata.SetRoutine((ObjectId)0x0F, newTalkRoutines.Talk_GiveItemOnFlag);
			npcdata.GetTalkArray((ObjectId)0x0F)[(int)TalkArrayPos.dialogue_1] = 0xD0;
			npcdata.GetTalkArray((ObjectId)0x0F)[(int)TalkArrayPos.dialogue_2] = 0xCE;
			npcdata.GetTalkArray((ObjectId)0x0F)[(int)TalkArrayPos.dialogue_3] = 0xCD;
			npcdata.GetTalkArray((ObjectId)0x0F)[(int)TalkArrayPos.item_id] = (byte)Item.Chime;
			npcdata.GetTalkArray((ObjectId)0x0F)[(int)TalkArrayPos.requirement_id] = (byte)ObjectId.Unne;

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

			var floorlist = new List<(List<MapLocation>, string)> {
				(new List<MapLocation> { MapLocation.Cardia1, MapLocation.Cardia2, MapLocation.Cardia4, MapLocation.Cardia5, MapLocation.Cardia6,
					MapLocation.DwarfCave, MapLocation.DwarfCaveRoom3, MapLocation.ElflandCastle, MapLocation.ElflandCastleRoom1, MapLocation.MatoyasCave, MapLocation.NorthwestCastle,
					MapLocation.NorthwestCastleRoom2, MapLocation.TitansTunnelEast, MapLocation.TitansTunnelRoom, MapLocation.TitansTunnelWest,
					MapLocation.Waterfall },
					""),
				(new List<MapLocation> { MapLocation.Caravan },
					""),
				(new List<MapLocation> { MapLocation.ConeriaCastleRoom1, MapLocation.ConeriaCastleRoom2, MapLocation.MirageTower1,
					MapLocation.SeaShrine1, MapLocation.SkyPalace1, MapLocation.TempleOfFiends1Room1, MapLocation.TempleOfFiends1Room2,
					MapLocation.TempleOfFiends1Room3, MapLocation.TempleOfFiends1Room4, MapLocation.CastleOrdeals1, MapLocation.TempleOfFiends2,
					MapLocation.EarthCave1, MapLocation.GurguVolcano1, MapLocation.IceCave1, MapLocation.MarshCave1 },
					"on floor 1"),
				(new List<MapLocation> { MapLocation.CastleOrdealsMaze, MapLocation.MirageTower2, MapLocation.SkyPalace2,
					MapLocation.TempleOfFiends2, MapLocation.EarthCave2, MapLocation.GurguVolcano2, MapLocation.IceCave2, },
					"on floor 2"),
				(new List<MapLocation> { MapLocation.CastleOrdealsTop, MapLocation.MirageTower3, MapLocation.SkyPalace3, MapLocation.TempleOfFiends3,
					MapLocation.EarthCaveVampire, MapLocation.GurguVolcano3, MapLocation.IceCave3 },
					"on floor 3"),
				(new List<MapLocation> { MapLocation.TempleOfFiendsPhantom,  MapLocation.SkyPalaceMaze, MapLocation.EarthCave4, MapLocation.GurguVolcano4,
					MapLocation.IceCaveFloater, MapLocation.IceCavePitRoom }, "on floor 4"),
				(new List<MapLocation> { MapLocation.EarthCaveLich, MapLocation.GurguVolcano5, MapLocation.SkyPalaceTiamat, MapLocation.TempleOfFiendsEarth,
					MapLocation.IceCave5 }, "on floor 5"),
				(new List<MapLocation> { MapLocation.GurguVolcano6, MapLocation.TempleOfFiendsFire, MapLocation.IceCaveBackExit }, "on floor 6"),
				(new List<MapLocation> { MapLocation.GurguVolcanoKary, MapLocation.TempleOfFiendsWater }, "on floor 7"),
				(new List<MapLocation> { MapLocation.TempleOfFiendsAir }, "on floor 8"),
				(new List<MapLocation> { MapLocation.TempleOfFiendsChaos }, "on floor 9"),

				(new List<MapLocation> { MapLocation.MarshCaveTop }, "on floor 2, Top"),
				(new List<MapLocation> { MapLocation.MarshCave3 }, "on floor 2, Bottom"),
				(new List<MapLocation> { MapLocation.MarshCaveBottom, MapLocation.MarshCaveBottomRoom13, MapLocation.MarshCaveBottomRoom14,
					MapLocation.MarshCaveBottomRoom16 }, "on floor 3, Bottom"),

				(new List<MapLocation> { MapLocation.SeaShrine2, MapLocation.SeaShrine2Room2 }, "on floor 2, Right Side"),
				(new List<MapLocation> { MapLocation.SeaShrineMermaids }, "on floor 3, Right Side"),
				(new List<MapLocation> { MapLocation.SeaShrine7 }, "on floor 5, Left Side"),
				(new List<MapLocation> { MapLocation.SeaShrine8 }, "on floor 6, Left Side"),
				(new List<MapLocation> { MapLocation.SeaShrineKraken }, "on floor 7, Left Side"),
			};

			var parentfloor = new List<(MapLocation, MapLocation)> {
				(MapLocation.ConeriaCastleRoom1, MapLocation.ConeriaCastle1),
				(MapLocation.ConeriaCastleRoom2, MapLocation.ConeriaCastle1),
				(MapLocation.DwarfCaveRoom3, MapLocation.DwarfCave),
				(MapLocation.ElflandCastleRoom1, MapLocation.ElflandCastle),
				(MapLocation.MarshCaveBottomRoom13, MapLocation.MarshCaveBottom),
				(MapLocation.MarshCaveBottomRoom14, MapLocation.MarshCaveBottom),
				(MapLocation.MarshCaveBottomRoom16, MapLocation.MarshCaveBottom),
				(MapLocation.NorthwestCastleRoom2, MapLocation.NorthwestCastle),
				(MapLocation.SeaShrine2Room2, MapLocation.SeaShrine2),
				(MapLocation.TempleOfFiends1Room1, MapLocation.TempleOfFiends1),
				(MapLocation.TempleOfFiends1Room2, MapLocation.TempleOfFiends1),
				(MapLocation.TempleOfFiends1Room3, MapLocation.TempleOfFiends1),
				(MapLocation.TempleOfFiends1Room4, MapLocation.TempleOfFiends1),
				(MapLocation.TitansTunnelRoom, MapLocation.TitansTunnelEast),
				(MapLocation.IceCaveFloater, MapLocation.IceCavePitRoom)
			};

			var invalidlocation = new List<MapLocation> { MapLocation.ConeriaCastleRoom1, MapLocation.ConeriaCastleRoom2, MapLocation.DwarfCaveRoom3,
					MapLocation.ElflandCastleRoom1, MapLocation.MarshCaveBottomRoom13, MapLocation.MarshCaveBottomRoom14, MapLocation.MarshCaveBottomRoom16,
					MapLocation.NorthwestCastleRoom2, MapLocation.SeaShrine2Room2, MapLocation.TempleOfFiends1Room1, MapLocation.TempleOfFiends1Room2,
					MapLocation.TempleOfFiends1Room3, MapLocation.TempleOfFiends1Room4, MapLocation.TitansTunnelRoom, MapLocation.StartingLocation,
					MapLocation.AirshipLocation
			};

			var deadends = new List<MapLocation> { MapLocation.BahamutCave2, MapLocation.Cardia1, MapLocation.Cardia2, MapLocation.Cardia4, MapLocation.Cardia5,
					MapLocation.Cardia6, MapLocation.CastleOrdealsTop, MapLocation.ConeriaCastle2, MapLocation.Coneria, MapLocation.CrescentLake, MapLocation.DwarfCave,
					MapLocation.EarthCaveLich, MapLocation.Elfland, MapLocation.ElflandCastle, MapLocation.Gaia, MapLocation.GurguVolcanoKary, MapLocation.IceCaveBackExit,
					MapLocation.Lefein, MapLocation.MarshCaveBottom, MapLocation.MarshCaveTop, MapLocation.MatoyasCave, MapLocation.Melmond, MapLocation.NorthwestCastle,
					MapLocation.Pravoka, MapLocation.SardasCave, MapLocation.SeaShrineKraken, MapLocation.SeaShrineMermaids, MapLocation.StartingLocation,
					MapLocation.TempleOfFiendsChaos, MapLocation.TitansTunnelEast, MapLocation.TitansTunnelWest, MapLocation.Waterfall
			};

			var targetlocation = new OverworldTeleportIndex();
			var finalstring = "";

			// Check if first floor of Sea is flipped
			var sea1flipped = false;
			var maps = this.ReadMaps();
			if(maps[(int)MapId.SeaShrineB3][(0x02, 0x04)].Value != 0x55) sea1flipped = true;

			// Check if floor shuffle is on
			if (overworldmap.OverriddenOverworldLocations != null && overworldmap.OverriddenOverworldLocations.Where(x => x.Key == location).Any())
			{ 
				var parentlocation = parentfloor.Find(x => x.Item1 == location).Item2;
				var validlocation = location;

				// If location is a room, set it to its parent location
				if (parentlocation != MapLocation.StartingLocation)
					validlocation = parentlocation;

				// Get worldmap location
				targetlocation = overworldmap.OverriddenOverworldLocations.Where(x => x.Key == validlocation).First().Value;

				// Get all the floors from that world map location while removing the rooms
				var dungeonfloors = overworldmap.OverriddenOverworldLocations.Where(x => x.Value == targetlocation && !invalidlocation.Contains(x.Key)).ToList();

				// If there's a split, we need to compute the floor position
				if (dungeonfloors.Select(x => x.Key).ToList().Contains(MapLocation.MarshCave1) || dungeonfloors.Select(x => x.Key).ToList().Contains(MapLocation.SeaShrine1))
				{
					var floornumber = new List<int> { 0, 0, 0 };
					var splitindex = 0;
					var description = new List<List<string>>();
					var descriptionindexer = new List<int> { 0, 0 };
					var descriptionindex = -1;
					for (int i = 0; i < dungeonfloors.Count(); i++)
					{
						if (dungeonfloors[i].Key == validlocation)
						{
							floornumber[splitindex]++;
							break;
						}
						else if (deadends.Contains(dungeonfloors[i].Key))
						{
							floornumber[splitindex] = 0;
							descriptionindexer[descriptionindex]--;
							if (descriptionindexer[descriptionindex] == 0)
								descriptionindex--;
							splitindex--;
						}
						else if (dungeonfloors[i].Key == MapLocation.MarshCave1)
						{
							description.Add(new List<string> { "", "Bottom", "Top" });
							descriptionindex++;
							descriptionindexer[descriptionindex] = 2;
							floornumber[splitindex]++;
							splitindex++;
						}
						else if (dungeonfloors[i].Key == MapLocation.SeaShrine1)
						{
							if (sea1flipped)
								description.Add(new List<string> { "", "Right Side", "Left Side" });
							else
								description.Add(new List<string> { "", "Left Side", "Right Side" });
							descriptionindex++;
							descriptionindexer[descriptionindex] = 2;
							floornumber[splitindex]++;
							splitindex++;
						}
						else
							floornumber[splitindex]++;
					}

					var finalfloor = 0;
					for (int i = 0; i < 3; i++)
					{
						finalfloor += floornumber[i];
					}

					finalstring = "on floor " + finalfloor;

					for (int i = 0; i < description.Count(); i++)
					{
						if (description[i][descriptionindexer[i]] != "")
							finalstring += ", " + description[i][descriptionindexer[i]];
					}
				}
				else // If there's no split, just get that floor index
					finalstring = "on floor " + (dungeonfloors.FindIndex(x => x.Key == validlocation) + 1);
			}
			else // No E/F shuffle, use the floorlist
			{
				targetlocation = StandardOverworldLocations.Where(x => x.Key == location).First().Value;
				finalstring = floorlist.Find(x => x.Item1.Contains(location)).Item2;
			}

			if(location == MapLocation.Caravan)
				finalstring += "at " + LocationNames.Where(x => x.Key == targetlocation).First().Value;
			else
				finalstring += ((finalstring == "" || finalstring == null)? "in " : " of ") + LocationNames.Where(x => x.Key == targetlocation).First().Value;

			return finalstring;
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
		public void SetDungeonNPC(List<MapId> flippedmaps, MT19337 rng)
		{
			// Check if maps are flipped
			bool earthB5flipped = flippedmaps.Contains(MapId.EarthCaveB5);
			bool volcanoB3flipped = flippedmaps.Contains(MapId.GurguVolcanoB3);
			bool sky3Fflipped = flippedmaps.Contains(MapId.SkyPalace3F);
			bool marshB1flipped = flippedmaps.Contains(MapId.MarshCaveB1);

			// Palettes changes
			PutInBank(0x00, 0xA000 + ((byte)MapId.GurguVolcanoB3 * 0x30) + 0x18, Blob.FromHex("000F1716000F1716"));
			PutInBank(0x00, 0xA000 + ((byte)MapId.MarshCaveB1 * 0x30) + 0x18, Blob.FromHex("000F1C34000F1834"));

			// Dwarf hinter - Earth - Text 0x70 - 0x5B - 5e
			SetNpc(MapId.DwarfCave, 5, ObjectId.None, 0x12, 0x18, false, false);
			SetNpc(MapId.EarthCaveB5, 0x0B, (ObjectId)0x5B, earthB5flipped ? (0x3F - 0x12) : 0x12, 0x18, false, false);

			// Robot hinter - Sky - Tex 0xE1 - 0xCF
			SetNpc(MapId.SkyPalace3F, 0, ObjectId.None, 0x08, 0x1C, false, true);
			SetNpc(MapId.SkyPalace3F, 0, (ObjectId)0xCF, sky3Fflipped ? (0x3F - 0x1B) : 0x1B, 0x34, true, false);

			// Dragon hinter - Gurgu - Text 0xE3 - 0x86 dragon
			SetNpc(MapId.Cardia, 1, ObjectId.None, 0x2D, 0x1A, false, false);
			SetNpc(MapId.GurguVolcanoB3, 1, (ObjectId)0x86, volcanoB3flipped ? (0x3F - 0x04) : 0x04, 0x1D, false, false);

			// Punk hinter - Marsh - Text 0xAF - 0x9D punk
			SetNpc(MapId.Onrac, 11, ObjectId.None, 0x2D, 0x1A, false, false);
			SetNpc(MapId.MarshCaveB1, 5, ObjectId.OnracPunk2, marshB1flipped ? (0x3F - 0x2D) : 0x2D, 0x1A, false, false);

			// Mermaid hinter - Text 0xB6 - 0xA5 mermaid
			List<ObjectId> mermaids = new List<ObjectId> { ObjectId.Mermaid1, ObjectId.Mermaid2, ObjectId.Mermaid4, ObjectId.Mermaid5, ObjectId.Mermaid6, ObjectId.Mermaid7, ObjectId.Mermaid8, ObjectId.Mermaid9, ObjectId.Mermaid10 };
			var selectedMermaidId = mermaids.PickRandom(rng);
			var selectedMermaid = FindNpc(MapId.SeaShrineB1, selectedMermaidId);
			var hintMermaid = FindNpc(MapId.SeaShrineB1, ObjectId.Mermaid3);
			SetNpc(MapId.SeaShrineB1, selectedMermaid.Index, ObjectId.Mermaid3, selectedMermaid.Coord.x, selectedMermaid.Coord.y, selectedMermaid.InRoom, selectedMermaid.Stationary);
			SetNpc(MapId.SeaShrineB1, hintMermaid.Index, selectedMermaidId, hintMermaid.Coord.x, hintMermaid.Coord.y, hintMermaid.InRoom, hintMermaid.Stationary);
		}
		public void NPCHints(MT19337 rng, NPCdata npcdata, Flags flags, OverworldMap overworldmap)
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
			if (flags.FreeCanoe ?? false) incentivePool.Remove(Item.Canoe);
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

						tempHint = FormatText(tempHint.Split('#')[0] + LocationText(generatedPlacement.Find(x => x.Item == tempItem).MapLocation, overworldmap) + tempHint.Split('#')[1]);
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
						tempHint = FormatText(tempHint.Split('#')[0] + LocationText(generatedPlacement.Find(x => x.Item == tempItem).MapLocation, overworldmap) + tempHint.Split('#')[1]);
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
					npcdata.GetTalkArray(npcSelected[i])[(int)TalkArrayPos.dialogue_2] = dialogueID[i];
					npcdata.SetRoutine(npcSelected[i], newTalkRoutines.Talk_norm);

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
