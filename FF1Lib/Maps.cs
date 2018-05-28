using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RomUtilities;

namespace FF1Lib
{
	public enum MapId : byte
	{
		Coneria = 0,
		Pravoka,
		Elfland,
		Melmond,
		CrescentLake,
		Gaia,
		Onrac,
		Lefein,
		ConeriaCastle1F,
		ElflandCastle,
		NorthwestCastle,
		CastleOfOrdeals1F,
		TempleOfFiends,
		EarthCaveB1,
		GurguVolcanoB1,
		IceCaveB1,
		Cardia,
		BahamutsRoomB1,
		Waterfall,
		DwarfCave,
		MatoyasCave,
		SardasCave,
		MarshCaveB1,
		MirageTower1F,
		ConeriaCastle2F,
		CastleOfOrdeals2F,
		CastleOfOrdeals3F,
		MarshCaveB2,
		MarshCaveB3,
		EarthCaveB2,
		EarthCaveB3,
		EarthCaveB4,
		EarthCaveB5,
		GurguVolcanoB2,
		GurguVolcanoB3,
		GurguVolcanoB4,
		GurguVolcanoB5,
		IceCaveB2,
		IceCaveB3,
		BahamutsRoomB2,
		MirageTower2F,
		MirageTower3F,
		SeaShrineB5,
		SeaShrineB4,
		SeaShrineB3,
		SeaShrineB2,
		SeaShrineB1,
		SkyPalace1F,
		SkyPalace2F,
		SkyPalace3F,
		SkyPalace4F,
		SkyPalace5F,
		TempleOfFiendsRevisited1F,
		TempleOfFiendsRevisited2F,
		TempleOfFiendsRevisited3F,
		TempleOfFiendsRevisitedEarth,
		TempleOfFiendsRevisitedFire,
		TempleOfFiendsRevisitedWater,
		TempleOfFiendsRevisitedAir,
		TempleOfFiendsRevisitedChaos,
		TitansTunnel
	}

	public enum WarMECHMode
	{
		Vanilla,
		Wandering4F,
		Aggro4F,
		BridgeOfDestiny
	}

	public partial class FF1Rom : NesRom
	{
		public const int MapPointerOffset = 0x10000;
		public const int MapPointerSize = 2;
		public const int MapCount = 61;
		public const int MapDataOffset = 0x10080;

		public const int TeleportOffset = 0x02D00;
		public const int TeleportCount = 64;

		public const int TilesetDataOffset = 0x00800;
		public const int TilesetDataSize = 2;
		public const int TilesetDataCount = 128;

		public const int MapObjJumpTableOffset = 0x390D3;
		public const int JumpTablePointerSize = 2;
		public const int MapObjOffset = 0x395D5;
		public const int MapObjGfxOffset = 0x02E00;
		public const int MapObjSize = 4;
		public const int MapObjCount = 0xD0;

		const ushort TalkFight = 0x94AA;

		private struct OrdealsRoom
		{
			public byte Entrance;
			public List<(int, int)> Teleporters;
		}

		public void ShuffleOrdeals(MT19337 rng, List<Map> maps)
		{
			// Here are all the teleporter rooms except the one you start in.
			// The last one is not normally accessible in the game.  We'll rewrite the teleporter located in that
			// room to go TO that room, and then shuffle it into one of the other locations so that you can go
			// through that room.
			var rooms = new List<OrdealsRoom>
			{
				new OrdealsRoom
				{
					Entrance = 0x4C,
					Teleporters = new List<(int, int)> { (0x0F, 0x09) }
				},
				new OrdealsRoom
				{
					Entrance = 0x4D,
					Teleporters = new List<(int, int)> { (0x08, 0x08), (0x08, 0x0A) }
				},
				new OrdealsRoom
				{
					Entrance = 0x4F,
					Teleporters = new List<(int, int)> { (0x09, 0x0F) }
				},
				new OrdealsRoom
				{
					Entrance = 0x50,
					Teleporters = new List<(int, int)> { (0x04, 0x14), (0x04, 0x16) }
				},
				new OrdealsRoom
				{
					Entrance = 0x51,
					Teleporters = new List<(int, int)> { (0x04, 0x12) }
				},
				new OrdealsRoom
				{
					Entrance = 0x52,
					Teleporters = new List<(int, int)> { (0x01, 0x01), (0x03, 0x02) }
				},
				new OrdealsRoom
				{
					Entrance = 0x53,
					Teleporters = new List<(int, int)> { (0x06, 0x08), (0x14, 0x0B), (0x14, 0x0D), (0x12, 0x10) }
				},
				new OrdealsRoom
				{
					Entrance = 0x54, // Normally inaccessible
					Teleporters = new List<(int, int)> { (0x08, 0x12) }
				}
			};

			// Shuffle the rooms, make sure the 4-pad room isn't the first room.
			do
			{
				rooms.Shuffle(rng);
			} while (rooms[0].Teleporters.Count == 4);

			// The room you start in always remains the same, but we need to adjust where it teleports you to.
			rooms.Insert(0, new OrdealsRoom
			{
				Entrance = 0x4E,
				Teleporters = new List<(int, int)> { (0x10, 0x0F) }
			});

			// First we choose a teleporter to link to the next room.
			byte exit = 0x55;
			var map = maps[(byte)MapId.CastleOfOrdeals2F];
			for (int i = 0; i < rooms.Count; i++)
			{
				int teleporter = rng.Between(0, rooms[i].Teleporters.Count - 1);
				var (x, y) = rooms[i].Teleporters[teleporter];
				rooms[i].Teleporters.RemoveAt(teleporter);

				if (i < rooms.Count - 1)
				{
					map[y, x] = rooms[i + 1].Entrance;
				}
				else
				{
					map[y, x] = exit;
				}
			}

			// Now we make all the other teleporters go to a random previous room, or the same room.
			for (int i = 0; i < rooms.Count; i++)
			{
				if (rooms[i].Teleporters.Count == 1)
				{
					var (x, y) = rooms[i].Teleporters[0];
					var backDestination = rng.Between(0, i);
					map[y, x] = rooms[backDestination].Entrance;
				}
				// Special rules for the 4-pad room: two teleporters go back, one goes to a totally random room,
				// and none of the four teleporters can be the same.
				else if (rooms[i].Teleporters.Count == 3)
				{
					rooms[i].Teleporters.Shuffle(rng);

					var (x, y) = rooms[i].Teleporters[0];
					var backDestination = rng.Between(0, i);
					map[y, x] = rooms[backDestination].Entrance;

					(x, y) = rooms[i].Teleporters[1];
					var otherBackDestination = backDestination;
					while (otherBackDestination == backDestination)
					{
						otherBackDestination = rng.Between(0, i);
					}
					map[y, x] = rooms[otherBackDestination].Entrance;

					(x, y) = rooms[i].Teleporters[2];
					var randomDestination = backDestination;
					while (randomDestination == backDestination || randomDestination == otherBackDestination || randomDestination == i + 1)
					{
						randomDestination = rng.Between(0, rooms.Count - 1);
					}
					map[y, x] = rooms[randomDestination].Entrance;
				}
			}

			// Now let's rewrite that teleporter.  The X coordinates are packed together, followed by the Y coordinates,
			// followed by the map indices.  Maybe we'll make a data structure for that someday soon.
			const byte LostTeleportIndex = 0x3C;
			Put(TeleportOffset + LostTeleportIndex, new byte[] { 0x10 });
			Put(TeleportOffset + TeleportCount + LostTeleportIndex, new byte[] { 0x12 });
		}

		public void ShuffleSkyCastle4F(MT19337 rng, List<Map> maps)
		{
			var map = maps[(byte)MapId.SkyPalace4F];

			var downTeleporter = (x: 0x03, y: 0x03);
			var upTeleporter = (x: 0x23, y: 0x23);

			var dest = GetSkyCastleFloorTile(rng, map);
			SwapTiles(map, upTeleporter, dest);
			//dest = GetSkyCastleFloorTile(rng, map);
			//SwapTiles(map, downTeleporter, dest);

			//const byte TeleportIndex3FTo4F = 0x2E;
			//Put(TeleportOffset + TeleportIndex3FTo4F, new [] { (byte)dest.x });
			//Put(TeleportOffset + TeleportCount + TeleportIndex3FTo4F, new [] { (byte)dest.y });
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

		private void SwapTiles(Map map, (int x, int y) src, (int x, int y) dest)
		{
			byte temp = map[dest.y, dest.x];
			map[dest.y, dest.x] = map[src.y, src.x];
			map[src.y, src.x] = temp;
		}

		public void EnableEarlyOrdeals()
		{
			// Remove CROWN requirement for Ordeals.
			const int OrdealsTileset = 1;
			var ordealsTilesetOffset = TilesetDataOffset + OrdealsTileset * TilesetDataCount * TilesetDataSize;
			var ordealsTilesetData = Get(ordealsTilesetOffset, TilesetDataCount * TilesetDataSize).ToUShorts();

			// The 4 masked-out bits are special flags for a tile.  We wipe the flags for the two throne teleportation tiles,
			// which normally indicate that the CROWN is required to use them.
			const ushort specialMask = 0b11111111_11100001;
			ordealsTilesetData[0x61] &= specialMask;
			ordealsTilesetData[0x62] &= specialMask;

			Put(ordealsTilesetOffset, Blob.FromUShorts(ordealsTilesetData));
		}

		public void EnableTitansTrove(List<Map> maps)
        {
			MoveNpc(MapId.TitansTunnel, 0, 4, 8, inRoom: false, stationary: true); // Move the Titan
			maps[(byte)MapId.TitansTunnel][9, 3] = 0x3F; // Block the tunnel
        }

		public void WarMECHNpc(WarMECHMode mode, MT19337 rng, List<Map> maps)
		{
			const byte UnusedTextPointer = 0xF7;
			const byte WarMECHEncounter = 0x56;
			const byte RobotGfx = 0x15;

			// Set up the map object.
			Put(MapObjOffset + (byte)ObjectId.WarMECH * MapObjSize, new [] { (byte)ObjectId.WarMECH, UnusedTextPointer, (byte)0x00, WarMECHEncounter });
			Data[MapObjGfxOffset + (byte)ObjectId.WarMECH] = RobotGfx;

			// Set the action when you talk to WarMECH.
			Put(MapObjJumpTableOffset + (byte)ObjectId.WarMECH * JumpTablePointerSize, Blob.FromUShorts(new[] { TalkFight }));

			// Change the dialogue.
			var dialogueStrings = new List<Blob>
			{
				FF1Text.TextToBytes("I. aM. WarMECH."),
				Blob.Concat(FF1Text.TextToBytes("I think you ought to know,"), new byte[] { 0x05 }, FF1Text.TextToBytes("I'm feeling very depressed.")),
				FF1Text.TextToBytes("Bite my shiny metal ass!"),
				Blob.Concat(FF1Text.TextToBytes("Put down your weapons."), new byte[] { 0x05 }, FF1Text.TextToBytes("You have 15 seconds to comply.")),
				// Blob.Concat(FF1Text.TextToBytes("I'm sorry "), new byte[] { 0x03 }, FF1Text.TextToBytes(","), new byte[] { 0x05 }, FF1Text.TextToBytes("I'm afraid I can't do that.")),
				FF1Text.TextToBytes("rEsIsTaNcE iS fUtIlE."),
				FF1Text.TextToBytes("Hasta la vista, baby."),
				Blob.Concat(FF1Text.TextToBytes("Bring back life form."), new byte[] { 0x05 }, FF1Text.TextToBytes("Priority one."), new byte[] { 0x05 }, FF1Text.TextToBytes("All other priorities rescinded."))
			};
			ushort freeTextSpacePointer = 0xB487;
			int pointerTarget = 0x20000 + freeTextSpacePointer;
			Put(pointerTarget, dialogueStrings.PickRandom(rng));
			Put(DialogueTextPointerOffset + 2 * UnusedTextPointer, Blob.FromUShorts(new [] { freeTextSpacePointer }));

			// Get rid of random WarMECH encounters.  Group 8 is now also group 7.
			var formationOffset = FormationFrequencyOffset + FormationFrequencySize * (64 + (byte)MapId.SkyPalace5F);
			var formations = Get(formationOffset, FormationFrequencySize);
			formations[6] = formations[7];
			Put(formationOffset, formations);

			if (mode == WarMECHMode.BridgeOfDestiny)
			{
				// Can't use mapNpcIndex 0, that's the Wind ORB.
				SetNpc(MapId.SkyPalace5F, 1, ObjectId.WarMECH, 0x07, 0x0E, inRoom: false, stationary: true);

				Data[0x029AB] = 0x14; // we can only change one color without messing up the Wind ORB.
			}
			else if (mode == WarMECHMode.Wandering4F || mode == WarMECHMode.Aggro4F)
			{
				var (x, y) = GetSkyCastleFloorTile(rng, maps[(byte)MapId.SkyPalace4F]);
				SetNpc(MapId.SkyPalace4F, 0, ObjectId.WarMECH, x, y, inRoom: false, stationary: false);

				// We can change all the colors here.
				Put(0x02978, Blob.FromHex("0F0F18140F0F1714"));
			}
		}

		private void MoveNpc(MapId mapId, int mapNpcIndex, int x, int y, bool inRoom, bool stationary)
		{
			int offset = MapSpriteOffset + ((byte)mapId * MapSpriteCount + mapNpcIndex) * MapSpriteSize;

			byte firstByte = (byte)x;
			firstByte |= (byte)(inRoom ? 0x80 : 0x00);
			firstByte |= (byte)(stationary ? 0x40 : 0x00);

			Data[offset + 1] = firstByte;
			Data[offset + 2] = (byte)y;
		}

		private void SetNpc(MapId mapId, int mapNpcIndex, ObjectId mapObjId, int x, int y, bool inRoom, bool stationary)
		{
			int offset = MapSpriteOffset + ((byte)mapId * MapSpriteCount + mapNpcIndex) * MapSpriteSize;

			byte firstByte = (byte)x;
			firstByte |= (byte)(inRoom ? 0x80 : 0x00);
			firstByte |= (byte)(stationary ? 0x40 : 0x00);

			Data[offset] = (byte)mapObjId;
			Data[offset + 1] = firstByte;
			Data[offset + 2] = (byte)y;
		}

		public List<Map> ReadMaps()
		{
			var pointers = Get(MapPointerOffset, MapCount * MapPointerSize).ToUShorts();

			return pointers.Select(pointer => new Map(Get(MapPointerOffset + pointer, Map.RowCount * Map.RowLength))).ToList();
		}

		public void WriteMaps(List<Map> maps)
		{
			var data = maps.Select(map => map.GetCompressedData()).ToList();

			var pointers = new ushort[MapCount];
			pointers[0] = MapDataOffset - MapPointerOffset;
			for (int i = 1; i < MapCount; i++)
			{
				pointers[i] = (ushort)(pointers[i - 1] + data[i - 1].Length);
			}

			Put(MapPointerOffset, Blob.FromUShorts(pointers));
			for (int i = 0; i < MapCount; i++)
			{
				Put(MapPointerOffset + pointers[i], data[i]);
			}
		}
	}
}
