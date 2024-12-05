using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;
using static FF1Lib.NpcScriptsLists;

namespace FF1Lib
{
	public enum ObjectSprites
	{
		Princess = 0,
		Woman,
		OldWoman,
		Dancer,
		Orb,
		Witch,
		Prince,
		Soldier,
		Scholar,
		Mohawk,
		Boy,
		OldMan,
		Dwarf,
		Mermaid,
		Lefein,
		King,
		Broom,
		Bat,
		Garland,
		Pirate,
		Fairy,
		Robot,
		Dragon,
		Bahamut,
		ElfWoman,
		ElfMan,
		ElfPrince,
		Plate,
		Titan,
		Vampire,
	}
	public class NpcObject
	{
		public ObjectSprites Sprite { get; set; }
		public TalkScripts Script { get; set; }
		public Blob OldScript { get; set; }
		public byte Dialogue1 { get; set; }
		public byte Dialogue2 { get; set; }
		public byte Dialogue3 { get; set; }
		public byte Item { get; set; }
		public byte Requirement { get; set; }
		public byte Battle { get; set; }
		public NpcObject(ObjectSprites sprite, TalkScripts script)
		{
			Sprite = sprite;
			Script = script;
			Dialogue1 = 0x00;
			Dialogue2 = 0x00;
			Dialogue3 = 0x00;
			Item = 0x00;
			Requirement = 0x00;
			Battle = 0x00;
		}
		public NpcObject(ObjectSprites sprite, Blob oldscript, byte[] data)
		{
			Sprite = sprite;
			OldScript = oldscript;
			Script = NpcScriptsLists.NewToOldScripts[oldscript.ToUShorts()[0]];
			Dialogue1 = data[0];
			Dialogue2 = data[1];
			Dialogue3 = data[2];
			Item = data[3];
			Requirement = 0x00;
			Battle = 0x00;
		}
		public byte[] GetBytes()
		{
			return new byte[] { Dialogue1, Dialogue2, Dialogue3, Item, Requirement, Battle };
		}

		// NPCs, Plate, Orbs, Fiends
		// MapObject > NpcObject > NpcScript > Dialogue
	}

	public class NpcObjectData
	{
		private List<NpcObject> npcObjects;

		private const int NpcTalkOffset = 0x390D3;
		private const int NpcTalkSize = 2;
		private const int NpcVisFlagOffset = 0x2F00;

		private const int npcObjectQty = 0xD0;

		private const int lut_MapObjTalkJumpTbl = 0x90D3;
		private const int lut_MapObjTalkData = 0x95D5;

		private const int lut_MapObjTalkData_move = 0xBA00;

		private const int oldTalkRoutinesBank = 0x0E;
		private const int newTalkRoutinesBank = 0x11;
		private const int lut_MapObjTalkJumpTbl_new = 0x8000;

		private FF1Rom rom;

		public NpcObjectData(StandardMaps maps, Flags flags, MT19337 rng, FF1Rom _rom)
		{
			rom = _rom;
			var sprites = rom.Get(MapObjGfxOffset, npcObjectQty).ToBytes();
			var talkroutines = rom.GetFromBank(oldTalkRoutinesBank, lut_MapObjTalkJumpTbl, npcObjectQty * 2).Chunk(2);
			var talkarrays = rom.GetFromBank(oldTalkRoutinesBank, lut_MapObjTalkData, npcObjectQty * 4).Chunk(4);
			npcObjects = sprites.Select((s, i) => new NpcObject((ObjectSprites)s, talkroutines[i], talkarrays[i].ToBytes())).ToList();
			Update(maps, flags, rng);
		}
		public NpcObject this[ObjectId index]
		{
			get => npcObjects[(int)index];
		}
		public void Write(List<ushort> pointers)
		{
			rom.Put(MapObjGfxOffset, npcObjects.Select(o => (byte)o.Sprite).ToArray());
			rom.PutInBank(newTalkRoutinesBank, lut_MapObjTalkData_move, npcObjects.SelectMany(o => o.GetBytes()).ToArray());

			Blob.FromUShorts(npcObjects.Select(o => pointers[(int)o.Script]).ToArray());
			rom.PutInBank(newTalkRoutinesBank, lut_MapObjTalkJumpTbl_new, Blob.FromUShorts(npcObjects.Select(o => pointers[(int)o.Script]).ToArray()));
		}
		public void Update(StandardMaps maps, Flags flags, MT19337 rng)
		{
			UpdateScripts(flags, maps);
			EnableNPCSwatter((bool)flags.NPCSwatter);
			ShuffleObjectiveNPCs((bool)flags.ChestsKeyItems && (bool)flags.ShuffleObjectiveNPCs && (flags.GameMode != GameModes.DeepDungeon), maps, rng);
		}
		private void UpdateScripts(Flags flags, StandardMaps maps)
		{
			// Update all NPC's dialogues script, default behaviours are maintained
			// Dialogue 1 is usually initial dialogue, 2 is after item given, 3 is give item
			npcObjects[(int)ObjectId.Astos].Dialogue1 = npcObjects[(int)ObjectId.Astos].Dialogue2;
			npcObjects[(int)ObjectId.Astos].Dialogue2 = 0x18;
			npcObjects[(int)ObjectId.Astos].Item = (byte)Item.Crystal;
			npcObjects[(int)ObjectId.Astos].Requirement = (byte)Item.Crown;
			npcObjects[(int)ObjectId.Astos].Battle = 0x7D;

			npcObjects[(int)ObjectId.Bikke].Dialogue1 = 0x0A;
			npcObjects[(int)ObjectId.Bikke].Item = (byte)Item.Ship;
			npcObjects[(int)ObjectId.Bikke].Battle = 0x7E;

			npcObjects[(int)ObjectId.CanoeSage].Dialogue1 = npcObjects[(int)ObjectId.CanoeSage].Dialogue3;
			npcObjects[(int)ObjectId.CanoeSage].Dialogue3 = npcObjects[(int)ObjectId.CanoeSage].Dialogue2;
			npcObjects[(int)ObjectId.CanoeSage].Dialogue2 = npcObjects[(int)ObjectId.CanoeSage].Dialogue1;
			npcObjects[(int)ObjectId.CanoeSage].Item = (byte)Item.Canoe;
			npcObjects[(int)ObjectId.CanoeSage].Requirement = (bool)flags.EarlySage ? (byte)0x00 : (byte)Item.EarthOrb;

			npcObjects[(int)ObjectId.CubeBot].Dialogue1 = 0x00;
			npcObjects[(int)ObjectId.CubeBot].Dialogue2 = 0x28;
			npcObjects[(int)ObjectId.CubeBot].Dialogue3 = 0x27;
			npcObjects[(int)ObjectId.CubeBot].Item = (byte)Item.Cube;

			npcObjects[(int)ObjectId.ElfDoc].Dialogue1 = 0x0B;
			npcObjects[(int)ObjectId.ElfDoc].Dialogue2 = 0x0D;
			npcObjects[(int)ObjectId.ElfDoc].Dialogue3 = 0x0C;
			npcObjects[(int)ObjectId.ElfDoc].Item = 0x00;
			npcObjects[(int)ObjectId.ElfDoc].Requirement = (byte)Item.Herb;

			npcObjects[(int)ObjectId.ElfPrince].Dialogue1 = 0x10;
			npcObjects[(int)ObjectId.ElfPrince].Dialogue2 = 0x0F;
			npcObjects[(int)ObjectId.ElfPrince].Dialogue3 = 0x0E;
			npcObjects[(int)ObjectId.ElfPrince].Item = (byte)Item.Key;
			npcObjects[(int)ObjectId.ElfPrince].Requirement = (byte)ObjectId.ElfDoc;

			npcObjects[(int)ObjectId.Fairy].Dialogue1 = 0x00;
			npcObjects[(int)ObjectId.Fairy].Dialogue2 = 0x24;
			npcObjects[(int)ObjectId.Fairy].Dialogue3 = 0x23;
			npcObjects[(int)ObjectId.Fairy].Item = (byte)Item.Oxyale;

			npcObjects[(int)ObjectId.Chaos3].Script = TalkScripts.Talk_Chaos;
			npcObjects[(int)ObjectId.Chaos3].Battle = 0x7B;

			npcObjects[(int)ObjectId.LichOrb].Battle = 0x7A;
			npcObjects[(int)ObjectId.KaryOrb].Battle = 0x79;
			npcObjects[(int)ObjectId.KrakenOrb].Battle = 0x78;
			npcObjects[(int)ObjectId.TiamatOrb].Battle = 0x77;
			npcObjects[(int)ObjectId.Vampire].Battle = 0x7C;

			npcObjects[(int)ObjectId.Garland].Battle = 0x7F;

			npcObjects.Where(o => o.OldScript == OriginalTalk.Talk_ifairship).ToList().ForEach(o => o.Requirement = 0xE4);
			npcObjects.Where(o => o.OldScript == OriginalTalk.Talk_ifbridge).ToList().ForEach(o => o.Requirement = (byte)Item.Bridge);
			npcObjects.Where(o => o.OldScript == OriginalTalk.Talk_ifcanoe).ToList().ForEach(o => o.Requirement = (byte)Item.Canoe);
			foreach (var npcobject in npcObjects.Where(o => o.OldScript == OriginalTalk.Talk_ifcanal).ToList())
			{
				npcobject.Requirement = (byte)Item.Canal;
				byte tempdialogue = npcobject.Dialogue2;
				npcobject.Dialogue2 = npcobject.Dialogue3;
				npcobject.Dialogue3 = tempdialogue;
			};

			npcObjects[(int)ObjectId.King].Dialogue1 = 0x01;
			npcObjects[(int)ObjectId.King].Dialogue2 = 0x03;
			npcObjects[(int)ObjectId.King].Dialogue3 = 0x02;
			npcObjects[(int)ObjectId.King].Item = (byte)Item.Bridge;
			npcObjects[(int)ObjectId.King].Requirement = (bool)flags.EarlyKing ? (byte)0x00 : (byte)ObjectId.Princess1;

			npcObjects[(int)ObjectId.Matoya].Dialogue1 = 0x17;
			npcObjects[(int)ObjectId.Matoya].Dialogue2 = 0x1A;
			npcObjects[(int)ObjectId.Matoya].Dialogue3 = 0x19;
			npcObjects[(int)ObjectId.Matoya].Item = (byte)Item.Herb;
			npcObjects[(int)ObjectId.Matoya].Requirement = (byte)Item.Crystal;

			npcObjects[(int)ObjectId.Princess2].Dialogue1 = 0x00;
			npcObjects[(int)ObjectId.Princess2].Dialogue2 = 0x07;
			npcObjects[(int)ObjectId.Princess2].Dialogue3 = 0x06;
			npcObjects[(int)ObjectId.Princess2].Item = (byte)Item.Lute;

			npcObjects[(int)ObjectId.Sarda].Dialogue1 = 0xB3;
			npcObjects[(int)ObjectId.Sarda].Dialogue2 = 0x18;
			npcObjects[(int)ObjectId.Sarda].Dialogue3 = 0x1E;
			npcObjects[(int)ObjectId.Sarda].Item = (byte)Item.Rod;
			npcObjects[(int)ObjectId.Sarda].Requirement = (bool)flags.EarlySarda ? (byte)0x00 : (byte)ObjectId.Vampire;

			npcObjects[(int)ObjectId.Titan].Dialogue1 = 0x29;
			npcObjects[(int)ObjectId.Titan].Requirement = (byte)Item.Ruby;

			npcObjects[(int)ObjectId.Unne].Dialogue1 = 0x1B;
			npcObjects[(int)ObjectId.Unne].Dialogue2 = 0x18;
			npcObjects[(int)ObjectId.Unne].Dialogue3 = 0x1C;
			npcObjects[(int)ObjectId.Unne].Item = 0x00;
			npcObjects[(int)ObjectId.Unne].Requirement = (byte)Item.Slab;

			npcObjects[(int)ObjectId.Nerrick].Dialogue1 = 0x13;
			npcObjects[(int)ObjectId.Nerrick].Dialogue2 = 0x00;
			npcObjects[(int)ObjectId.Nerrick].Dialogue3 = 0x14;
			npcObjects[(int)ObjectId.Nerrick].Item = (!(bool)flags.NPCItems && (bool)flags.IsCanalFree) ? (byte)Item.Canal : (byte)Item.Cabin;
			npcObjects[(int)ObjectId.Nerrick].Requirement = (byte)Item.Tnt;

			npcObjects[(int)ObjectId.Smith].Dialogue1 = 0x15;
			npcObjects[(int)ObjectId.Smith].Dialogue2 = 0x18;
			npcObjects[(int)ObjectId.Smith].Dialogue3 = 0x16;
			npcObjects[(int)ObjectId.Smith].Item = (byte)Item.Xcalber;
			npcObjects[(int)ObjectId.Smith].Requirement = (byte)Item.Adamant;

			// Replace sky warrior dialogue that got taken over by "Nothing here".
			npcObjects[0x3D].Dialogue2 = 0xDB;
			npcObjects[0x3D].Dialogue3 = 0x4C;

			// Chime Lefein man is moved to ID 15 to keep him with all the other NPCs
			npcObjects[(int)ObjectId.Lefein].Sprite = ObjectSprites.Lefein;
			npcObjects[(int)ObjectId.Lefein].Script = TalkScripts.Talk_GiveItemOnFlag;
			npcObjects[(int)ObjectId.Lefein].Dialogue1 = 0xD0;
			npcObjects[(int)ObjectId.Lefein].Dialogue2 = 0xCE;
			npcObjects[(int)ObjectId.Lefein].Dialogue3 = 0xCD;
			npcObjects[(int)ObjectId.Lefein].Item = (byte)Item.Chime;
			npcObjects[(int)ObjectId.Lefein].Requirement = (byte)ObjectId.Unne;
			maps[MapIndex.Lefein].MapObjects[0].ObjectId = ObjectId.Lefein;
		}
		private void EnableNPCSwatter(bool enable)
		{
			if (!enable)
			{
				return;
			}

			foreach (var npc in npcObjects)
			{
				if (npc.Script == TalkScripts.Talk_norm)
				{
					npc.Script = TalkScripts.Talk_kill;
				}
			}
			// Protect Lute and Rod Plate
			npcObjects[(int)ObjectId.LutePlate].Script = TalkScripts.Talk_norm;
			npcObjects[(int)ObjectId.RodPlate].Script = TalkScripts.Talk_norm;
		}
		public void ShuffleObjectiveNPCs(bool enable, StandardMaps maps, MT19337 rng)
		{
			if (!enable)
			{
				return;
			}

			var objectiveNPCs = new Dictionary<ObjectId, MapLocation>
			{
				{ ObjectId.Bahamut, MapLocation.BahamutCave2 },
				{ ObjectId.Unne, MapLocation.Melmond },
				{ ObjectId.ElfDoc, MapLocation.ElflandCastle },
			};

			Dictionary<MapLocation, (int x, int y)> objectiveNPCPositions = new Dictionary<MapLocation, (int x, int y)>
			{
				{ MapLocation.BahamutCave2, (0x15, 0x03) },
				{ MapLocation.Melmond, (0x1A, 0x01) },
				{ MapLocation.ElflandCastle, (0x09, 0x05) },
			};

			Dictionary<MapLocation, MapIndex> objectiveNPCMapIndexs = new Dictionary<MapLocation, MapIndex>
			{
				{ MapLocation.BahamutCave2, MapIndex.BahamutCaveB2 },
				{ MapLocation.Melmond, MapIndex.Melmond },
				{ MapLocation.ElflandCastle, MapIndex.ElflandCastle },
			};

			var locations = objectiveNPCs.Values.ToList();
			foreach (var npc in objectiveNPCs.Keys.ToList())
			{
				var location = locations.SpliceRandom(rng);
				objectiveNPCs[npc] = location;

				var (x, y) = objectiveNPCPositions[location];
				y += (location == MapLocation.ElflandCastle && npc == ObjectId.Bahamut) ? 1 : 0;

				var inRoom = location != MapLocation.Melmond;
				var stationary = npc == ObjectId.Bahamut || (npc == ObjectId.ElfDoc && location == MapLocation.ElflandCastle);

				maps[objectiveNPCMapIndexs[location]].MapObjects.SetNpc(0, npc, x, y, inRoom, stationary);
			}
		}
		public void UpdateItemPlacement(List<IRewardSource> itemplacement)
		{
			if (itemplacement == null) // Return if vanilla placement
				return;

			var targetnpc = itemplacement.Where(x => x.GetType().Equals(typeof(NpcReward)));

			foreach (var item in targetnpc)
			{
				switch (item.Name)
				{
					case "Astos":
						npcObjects[(int)ObjectId.Astos].Item = (byte)item.Item;
						break;
					case "CubeBot":
						npcObjects[(int)ObjectId.CubeBot].Item = (byte)item.Item;
						break;
					case "King":
						npcObjects[(int)ObjectId.King].Item = (byte)item.Item;
						break;
					case "Princess2":
						npcObjects[(int)ObjectId.Princess2].Item = (byte)item.Item;
						break;
					case "Matoya":
						npcObjects[(int)ObjectId.Matoya].Item = (byte)item.Item;
						break;
					case "Bikke":
						npcObjects[(int)ObjectId.Bikke].Item = (byte)item.Item;
						break;
					case "ElfPrince":
						npcObjects[(int)ObjectId.ElfPrince].Item = (byte)item.Item;
						break;
					case "Sarda":
						npcObjects[(int)ObjectId.Sarda].Item = (byte)item.Item;
						break;
					case "CanoeSage":
						npcObjects[(int)ObjectId.CanoeSage].Item = (byte)item.Item;
						break;
					case "Fairy":
						npcObjects[(int)ObjectId.Fairy].Item = (byte)item.Item;
						break;
					case "Lefein":
						npcObjects[(int)ObjectId.Lefein].Item = (byte)item.Item;
						break;
					case "Nerrick":
						npcObjects[(int)ObjectId.Nerrick].Item = (byte)item.Item;
						break;
					case "Smith":
						npcObjects[(int)ObjectId.Smith].Item = (byte)item.Item;
						break;
				}
			}

		}
	}
}
