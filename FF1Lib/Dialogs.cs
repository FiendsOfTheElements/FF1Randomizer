using System.ComponentModel;

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
			Talk_Chaos,
			NoOW_Floater,
			NoOW_Chime,
			NoOW_Canoe,
			NoOW_Nerrick
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
		public void AddNewChars()
		{
			const int statusCharOffset = 0x37000;
			const int tilesetOffset = 0x0C000;
			const int tilesetSize = 0x800;
			const int tilesetCount = 8;
			const int battleTilesetOffset = 0x1C000;
			const int battleTilesetSize = 0x800;
			const int battleTilesetCount = 16;
			const int shopTilesetOffset = 0x24000;

			// There's 5 unused characters available on every tileset, 0x7B to 0x7F
			//  new chars must be added to the BytesByText list in FF1Text.cs
			var newChars = new List<(byte, string)>
			{
				(0x7B, "000008083E080800FFFFFFFFFFFFFFFF"), // + sign
				(0x7C, "FFFFFF7F3DFFFFFFFFFF99C2E6C299FE"),  // Trapped chest (standard)
				(0x7D, "FFFF99C3E7C399FF0000663C183C6600")  // Trap tile
			};

			foreach (var newchar in newChars)
			{
				// Menu screen tilset
				Put(statusCharOffset + newchar.Item1 * 0x10, Blob.FromHex(newchar.Item2));
				// Shop tileset
				Put(shopTilesetOffset + newchar.Item1 * 0x10, Blob.FromHex(newchar.Item2));

				// Map tilesets
				for (int i = 0; i < tilesetCount; i++)
				{
					Put(tilesetOffset + tilesetSize * i + newchar.Item1 * 0x10, Blob.FromHex(newchar.Item2));
				}

				// Battle tilesets
				for (int i = 0; i < battleTilesetCount; i++)
				{
					Put(battleTilesetOffset + battleTilesetSize * i + newchar.Item1 * 0x10, Blob.FromHex(newchar.Item2));
				}
			}

			// Hack this one in, because chests in sky have different graphics from other chests
			var trappedChestSky = "FFFFFF7F3DFFFF7FFF6699C2E64299EE";
			Put(tilesetOffset + tilesetSize * (int)TileSets.SkyCastle + 0x7C * 0x10, Blob.FromHex(trappedChestSky));
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
			dialogs[0xDB] = dialogs[0x50]; // One of the sky warriors (ToF bat)
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
		public void TransferTalkRoutines(Flags flags)
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

			// TalkInBattle, compatible with ext items
			PutInBank(newTalkRoutinesBank, 0xB120, Blob.FromHex("856A20CDD8A9008D01208D1540A002204A96A001204A96A903CD866BD00820189668684C439660"));

			// LoadPrice fix
			PutInBank(newTalkRoutinesBank, 0x9F10, Blob.FromHex("A9118558A5734C10B4"));

			//CheckCanTake
			if (flags.ExtConsumableSet != ExtConsumableSet.None)
			{
				//C920 instead of C916
				PutInBank(newTalkRoutinesBank, 0xB180, Blob.FromHex("C9169027C920900BC9449012C96C90164CABB1AABD2060C963B0114CABB12034DDB0094CABB12046DDB00118A9F160"));
			}
			else
			{
				PutInBank(newTalkRoutinesBank, 0xB180, Blob.FromHex("C9169027C91C900BC9449012C96C90164CABB1AABD2060C963B0114CABB12034DDB0094CABB12046DDB00118A9F160"));
			}

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
			var formatted = ItemsText[(byte)item].TrimEnd(' ');

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
		public void UpdateDialogs(NPCdata npcdata, Flags flags)
		{
			Dictionary<int, string> newDialogs = new Dictionary<int, string>();

			//CleanupNPCRoutines(); - Deprecated 2020-12-12
			TransferDialogs();
			TransferTalkRoutines(flags);
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
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.item_id] = 0x00;
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
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1] = 0x29;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.requirement_id] = (byte)Item.Ruby;
				}
				else if (npcdata.GetOldRoutine((ObjectId)i) == originalTalk.Talk_Unne)
				{
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_ElfDocUnne);
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1] = 0x1B;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2] = 0x18;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3] = 0x1C;
					npcdata.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.item_id] = 0x00;
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

			// Remove reference to "Cave of Marsh" from Astos dialog, unless Crown not shuffled
			if (flags.IncentivizeFetchNPCs != false || flags.Treasures != false) {
				newDialogs.Add(0x11, "Astos double-crossed us.\nFind where they stashed\nthe CROWN. Then bring it\ndirectly back to me!");
			}

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
