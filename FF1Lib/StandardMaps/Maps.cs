using FF1Lib.Procgen;
using FF1Lib.Sanity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO.Compression;

namespace FF1Lib
{
	public enum Tile
	{
		RoomBackLeft = 0x00,
		RoomBackCenter = 0x01,
		RoomBackRight = 0x02,
		RoomLeft = 0x03,
		RoomCenter = 0x04,
		RoomRight = 0x05,
		RoomFrontLeft = 0x06,
		RoomFrontCenter = 0x07,
		RoomFrontRight = 0x08,
		Ladder = 0x09,
		LadderHole = 0x0A,
		WarpUp = 0x18,
		EarthCaveInside = 0x2E,
		InsideWall = 0x30,
		FloorSafe = 0x31,
		HallwayLeft = 0x32,
		HallwayRight = 0x33,
		WallLeft = 0x34,
		WallRight = 0x35,
		Impassable = 0x3D,
		Door = 0x36,
		EarthCaveOOB = 0x38,
		Doorway = 0x3A,
		DoorLocked = 0x3B,
		Lava = 0x39,
		EarthCaveRockA = 0x3E,
		EarthCaveRockB = 0x3F,
		MarshCaveOOB = 0x3F,
		EarthCaveRandomEncounters = 0x41,
		WaterfallInside = 0x46,
		WaterfallSpikeTile = 0x48,
		WaterfallRandomEncounters = 0x49,
		PortalWarp = 0x40,
		ToFRNoEncounter = 0x31,
		ToFREncounter = 0x5C,
		// Begin Town Tiles
		TownGrass = 0x00,
		TownGrassShadow = 0x01,
		TownGrassShadowDiagonal = 0x02,
		TownHorizontalWallLeft = 0x03,
		TownHorizontalWallCenter = 0x04,
		TownHorizontalWallRight = 0x05,
		TownHorizontalWallOffsetLeft = 0x06,
		TownHorizontalWallOffsetCenter = 0x07,
		TownHorizontalWallOffsetRight = 0x08,
		TownHorizontalWallOffsetRightShadow = 0x09,
		TownVerticalWallRight = 0x0A,
		TownVerticalWallLeftShadowTopCorner = 0x0B,
		TownVerticalWallLeftShadow = 0x0C,
		TownVerticalWallLeftBottomCorner = 0x0D,
		TownTree = 0x0E,
		TownBushes = 0x0F,
		TownStonePath = 0x10,
		TownStonePathRoundedTL = 0x11,
		TownStonePathRoundedTR = 0x12,
		TownStonePathRoundedBL = 0x13,
		TownStonePathRoundedBR = 0x14,
		TownStonePathShadowCorner = 0x15,
		TownStonePathShadowEdge = 0x16,
		TownBuildingRoofTopLeftEdge = 0x17,
		TownBuildingRoof = 0x18,
		TownBuildingRoofTopRightEdge = 0x19, // looks identical to 0x17, had a cross in Japanese version
		TownSignClinic = 0x1A,
		TownBuildingRoofPitched = 0x1B,
		TownBuildingWindow = 0x1C,
		TownBuildingWindowDouble = 0x1D,
		TownDoor1 = 0x1E, // weapon shop, coneria
		TownDoor2 = 0x1F, // weapon shop, pravoka
		TownSignItem = 0x20,
		TownSignWeapon = 0x21,
		TownSignArmor = 0x22,
		TownSignWhiteMagic = 0x23,
		TownSignBlackMagic = 0x24,
		TownSignInn = 0x25,
		TownDoor3 = 0x26, // weapon shop, elfland
		TownWaterway = 0x27,
		TownWaterwayShadow = 0x28,
		TownWaterwayShadowDiagonal = 0x29,
		TownHorizonalBridge = 0x2A,
		TownVerticalBridge = 0x2B,
		TownBoardwalk = 0x2C, // unused?
		TownDoor4 = 0x2D, // weapon shop, melmond
		TownFountain = 0x2E,
		TownTomb1 = 0x2F, // ordinary tomb
		TownFence = 0x30,
		TownWell = 0x31,
		TownSand = 0x32,
		TownFlowers = 0x33,
		TownPalmTrees = 0x34, // unused?
		TownSignPub = 0x34, // used in tavern recruitment
		TownBoardwalk2 = 0x35, // unused? // may be identical to 0x2C, probably intended to go in front of doors
		TownDoor5 = 0x36, // weapon shop, crescent lake
		TownDoorOpen = 0x37,
		TownStonePath2 = 0x38, // looks identical to 0x10, used in front of doors
		TownGrass2 = 0x39, // looks identical to 0x00, used in front of doors
		TownVerticalBridge2 = 0x3A, // looks identical to 0x2B, used in front of doors
		TownWaterway2 = 0x3B, // looks identical to 0x27, intended for in front of doors?
		TownSand2 = 0x3C, // looks identical to 0x32, used in front of doors
		TownFlowers2 = 0x3D, // looks identical to 0x33, used in front of doors
		TownDoor6 = 0x3E, // weapon shop, gaia
		TownDoor7 = 0x3F, // weapon shop, empty #1
		TownDoor8 = 0x40, // weapon shop, empty #2
		TownDoor9 = 0x41, // weapon shop, empty #3
		TownDoor10 = 0x42, // armor shop, empty #1
		TownDoor11 = 0x43, // armor shop, coneria
		TownDoor12 = 0x44, // armor shop, pravoka
		TownDoor13 = 0x45, // armor shop, elfland
		TownDoor14 = 0x46, // armor shop, melmond
		TownGrassWarp = 0x47,
		TownSubmarineWarp = 0x48,
		TownDoor15 = 0x49, // armor shop, crescent lake
		TownDoor16 = 0x4A, // armor shop, gaia
		TownDoor17 = 0x4B, // armor shop, empty #2
		TownDoor18 = 0x4C, // armor shop, empty #3
		TownDoor19 = 0x4D, // armor shop, empty #4
		TownDoor20 = 0x4E, // armor shop, empty #5
		TownDoor21 = 0x4F, // white magic shop, L1 coneria
		TownDoor22 = 0x50, // white magic shop, L2 pravoka
		TownDoor23 = 0x51, // white magic shop, L3 elfland
		TownDoor24 = 0x52, // white magic shop, L5 melmond
		TownDoor25 = 0x53, // white magic shop, L6 crescent lake
		TownDoor26 = 0x54, // white magic shop, L4 elfland
		TownDoor27 = 0x55, // white magic shop, L7 gaia
		TownDoor28 = 0x56, // white magic shop, L8 gaia
		TownDoor29 = 0x57, // white magic shop, L7 onrac
		TownDoor30 = 0x58, // white magic shop, L8 lefein
		TownDoor31 = 0x59, // black magic shop, L1 coneria
		TownDoor32 = 0x5A, // black magic shop, L2 pravoka
		TownDoor33 = 0x5B, // black magic shop, L3 elfland
		TownDoor34 = 0x5C, // black magic shop, L5 melmond
		TownDoor35 = 0x5D, // black magic shop, L6 crescent lake
		TownDoor36 = 0x5E, // black magic shop, L4 elfland
		TownDoor37 = 0x5F, // black magic shop, L7 gaia
		TownDoor38 = 0x60, // black magic shop, L8 gaia
		TownDoor39 = 0x61, // black magic shop, L7 onrac
		TownDoor40 = 0x62, // black magic shop, L8 lefein
		TownDoor41 = 0x63, // clinic, coneria
		TownDoor42 = 0x64, // clinic, elfland
		TownDoor43 = 0x65, // clinic, crescent lake
		TownDoor44 = 0x66, // clinic, gaia
		TownDoor45 = 0x67, // clinic, onrac
		TownDoor46 = 0x68, // clinic, pravoka
		TownDoor47 = 0x69, // clinic, unused #1
		TownDoor48 = 0x6A, // clinic, unused #2
		TownDoor49 = 0x6B, // clinic, unused #3
		TownDoor50 = 0x6C, // clinic, unused #4
		TownDoor51 = 0x6D, // inn, coneria
		TownDoor52 = 0x6E, // inn, pravoka
		TownDoor53 = 0x6F, // inn, elfland
		TownDoor54 = 0x70, // inn, melmond
		TownDoor55 = 0x71, // inn, crescent lake
		TownDoor56 = 0x72, // inn, gaia
		TownDoor57 = 0x73, // inn, onrac
		TownDoor58 = 0x74, // inn, unused #1
		TownDoor59 = 0x75, // inn, unused #2
		TownDoor60 = 0x76, // inn, unused #3
		TownDoor61 = 0x77, // item shop, coneria
		TownDoor62 = 0x78, // item shop, pravoka
		TownDoor63 = 0x79, // item shop, elfland
		TownDoor64 = 0x7A, // item shop, crescent lake
		TownDoor65 = 0x7B, // item shop, gaia
		TownDoor66 = 0x7C, // item shop, onrac
		TownDoor67 = 0x7D, // item shop, empty #1
		TownDoor68 = 0x7E, // item shop, empty #2
		TownTomb2 = 0x7F, // easter egg tomb

		// Cardia tiles
		CardiaFloor = 0x04,
		CardiaCandles = 0x0D,
		CardiaChest1 = 0x68,
		CardiaChest2 = 0x69,
		CardiaChest3 = 0x6A,
		CardiaChest4 = 0x6B,
		CardiaChest5 = 0x6C,
		CardiaChest6 = 0x6D,
		CardiaChest7 = 0x6E,
		CardiaChest8 = 0x6F,
		CardiaChest9 = 0x70,
		CardiaChest10 = 0x71,
		CardiaChest11 = 0x72,
		CardiaChest12 = 0x73,
		CardiaChest13 = 0x74,
		CardiaEncounters = 0x49,
	}

	public enum SkyCastle4FMazeMode
	{
		Vanilla,
		Teleporters,
		Maze
	}

	[JsonObject(MemberSerialization.OptIn)]
	public struct NPC
	{
	    [JsonProperty]
	    [JsonConverter(typeof(StringEnumConverter))]
		public ObjectId ObjectId;

	    [JsonProperty]
		public int Index;

	    [JsonProperty]
		public (int x, int y) Coord;

	    [JsonProperty]
		public bool InRoom;

	    [JsonProperty]
		public bool Stationary;
	}

	public partial class FF1Rom : NesRom
	{
		public const int TeleportOffset = 0x3F000;
		public const int TeleportCount = 256;

		public const int TilesetDataOffset = 0x00800;
		public const int TilesetDataSize = 2;
		public const int TilesetDataCount = 128;
		public const int TilesetCount = 8;

		public const int MapObjJumpTableOffset = 0x390D3;
		public const int JumpTablePointerSize = 2;
		public const int MapObjOffset = 0x395D5;
		public const int MapObjGfxOffset = 0x02E00;
		public const int MapObjSize = 4;
		public const int MapObjCount = 0xD0;

		const ushort TalkFight = 0x94AA;
		private void GeneralMapHacks(Flags flags, Overworld overworld, StandardMaps maps, ZoneFormations formations, TileSetsData tilesets, MT19337 rng)
		{
			EnableMelmondGhetto((bool)flags.AllowUnsafeMelmond, (bool)flags.EnemizerEnabled, overworld, formations, tilesets);
		}
		private void EnableMelmondGhetto(bool melmonghetto, bool enemizerOn, Overworld overworld, ZoneFormations formations, TileSetsData tilesets)
		{
			if (!melmonghetto)
			{
				return;
			}

			// Set town desert tile to random encounters.
			// If enabled, trap tile shuffle will change that second byte to 0x00 afterward.
			tilesets[(int)TileSets.Town].Tiles[0x32].PropertyType = 0x0A;
			tilesets[(int)TileSets.Town].Tiles[0x32].PropertyValue = 0x80;

			// Give Melmond Desert backdrop
			overworld.BattleBackdrops[0x4D] = Backdrop.Desert;

			if (!enemizerOn) // if enemizer formation shuffle is on, it will have assigned battles to Melmond already
			{
				formations[64 + (int)MapIndex.Melmond] = new ZoneFormation() { Index = 64 + (int)MapIndex.Melmond, Formations = new() { 0x0F, 0x0F, 0x8F, 0x2C, 0xAC, 0xAC, 0x7E, 0x7C } };
				//Put(0x2C218, Blob.FromHex("0F0F8F2CACAC7E7C"));
			}
		}

		private (int x, int y) GetSkyCastleFloorTile(MT19337 rng, Map map)
		{
			int x, y;
			do
			{
				x = rng.Between(0, 63);
				y = rng.Between(0, 63);

			} while (map[y, x] != 0x4B);

			return (x, y);
		}
		public void SetNpc(MapIndex MapIndex, NPC npc)
		{
		    SetNpc(MapIndex, npc.Index, npc.ObjectId, npc.Coord.x, npc.Coord.y, npc.InRoom, npc.Stationary);
		}

		public void MoveNpc(MapIndex MapIndex, int mapNpcIndex, int x, int y, bool inRoom, bool stationary)
		{
			int offset = MapSpriteOffset + ((byte)MapIndex * MapSpriteCount + mapNpcIndex) * MapSpriteSize;

			byte firstByte = (byte)x;
			firstByte |= (byte)(inRoom ? 0x80 : 0x00);
			firstByte |= (byte)(stationary ? 0x40 : 0x00);

			Data[offset + 1] = firstByte;
			Data[offset + 2] = (byte)y;
		}

		public void SetNpc(MapIndex MapIndex, int mapNpcIndex, ObjectId mapObjId, int x, int y, bool inRoom, bool stationary)
		{
			int offset = MapSpriteOffset + ((byte)MapIndex * MapSpriteCount + mapNpcIndex) * MapSpriteSize;

			byte firstByte = (byte)x;
			firstByte |= (byte)(inRoom ? 0x80 : 0x00);
			firstByte |= (byte)(stationary ? 0x40 : 0x00);

			Data[offset] = (byte)mapObjId;
			Data[offset + 1] = firstByte;
			Data[offset + 2] = (byte)y;
		}

		public NPC FindNpc(MapIndex MapIndex, ObjectId mapObjId)
		{
			var tempNPC = new NPC();

			for (int i = 0; i < MapSpriteCount; i++)
			{
				int offset = MapSpriteOffset + ((byte)MapIndex * MapSpriteCount + i) * MapSpriteSize;

				if (Data[offset] == (byte)mapObjId)
				{
					tempNPC.ObjectId = (ObjectId)Data[offset];
					tempNPC.Index = i;
					tempNPC.Coord = (Data[offset + 1] & 0x3F, Data[offset + 2]);
					tempNPC.InRoom = (Data[offset + 1] & 0x80) > 0;
					tempNPC.Stationary = (Data[offset + 1] & 0x40) > 0;
					break;
				}
			}
			return tempNPC;
		}

		public IEnumerable<(MapIndex, NPC)> FindNpc(ObjectId mapObjId)
		{
			var tempNPC = new NPC();

			//not good, but quick
			foreach (var mid in Enum.GetValues<MapIndex>())
			{
				for (int i = 0; i < MapSpriteCount; i++)
				{
					int offset = MapSpriteOffset + ((byte)mid * MapSpriteCount + i) * MapSpriteSize;

					if (Data[offset] == (byte)mapObjId)
					{
					        tempNPC.ObjectId = mapObjId;
						tempNPC.Index = i;
						tempNPC.Coord = (Data[offset + 1] & 0x3F, Data[offset + 2]);
						tempNPC.InRoom = (Data[offset + 1] & 0x80) > 0;
						tempNPC.Stationary = (Data[offset + 1] & 0x40) > 0;

						yield return (mid, tempNPC);
					}
				}
			}
		}


		public NPC GetNpc(MapIndex MapIndex, int position)
		{
			var tempNPC = new NPC();

			int offset = MapSpriteOffset + ((byte)MapIndex * MapSpriteCount + position) * MapSpriteSize;

			tempNPC.ObjectId = (ObjectId)Data[offset];
			tempNPC.Index = position;
			tempNPC.Coord = (Data[offset + 1] & 0x3F, Data[offset + 2]);
			tempNPC.InRoom = (Data[offset + 1] & 0x80) > 0;
			tempNPC.Stationary = (Data[offset + 1] & 0x40) > 0;

			return tempNPC;
		}
	}
}
