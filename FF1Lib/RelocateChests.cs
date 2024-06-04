using FF1Lib.Sanity;
using RomUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static FF1Lib.FF1Rom;
using static FF1Lib.NpcScriptsLists;
using static FF1Lib.RelocateChests;

namespace FF1Lib
{
	public partial class RelocateChests
	{
		private const int TILESETPATTERNTABLE_OFFSET = 0xC000;
		private FF1Rom rom;

		public RelocateChests(FF1Rom _rom)
		{
			rom = _rom;
		}

		public class Room
		{
			public MapIndex MapIndex;
			public int RoomIndex;
			public MapElement start;
			public List<MapElement> floor = new();
			public List<MapElement> walkable = new();
			public List<MapElement> doors = new();
			public List<MapElement> chests = new();
			public List<MapElement> teleIn = new();
			public List<MapElement> teleOut = new();
			public List<MapElement> npcs = new();
			public List<MapElement> killablenpcs = new();
			public bool hasBattles = false;
			public Room() { }
			public Room(Room copyfrom)
			{
				MapIndex = copyfrom.MapIndex;
				RoomIndex = copyfrom.RoomIndex;
				start = copyfrom.start;
				floor = new List<MapElement>(copyfrom.floor);
				walkable = new List<MapElement>(copyfrom.walkable);
				doors = new List<MapElement>(copyfrom.doors);
				chests = new List<MapElement>(copyfrom.chests);
				teleIn = new List<MapElement>(copyfrom.teleIn);
				teleOut = new List<MapElement>(copyfrom.teleOut);
				npcs = new List<MapElement>(copyfrom.npcs);
				killablenpcs = new List<MapElement>(copyfrom.killablenpcs);
				hasBattles = copyfrom.hasBattles;
			}
			public void Replace(Room copyfrom)
			{
				MapIndex = copyfrom.MapIndex;
				start = copyfrom.start;
				RoomIndex = copyfrom.RoomIndex;
				floor.Clear();
				floor.AddRange(copyfrom.floor);

				walkable.Clear();
				walkable.AddRange(copyfrom.walkable);

				doors.Clear();
				doors.AddRange(copyfrom.doors);

				chests.Clear();
				chests.AddRange(copyfrom.chests);

				teleIn.Clear();
				teleIn.AddRange(copyfrom.teleIn);

				teleOut.Clear();
				teleOut.AddRange(copyfrom.teleOut);

				npcs.Clear();
				npcs.AddRange(copyfrom.npcs);

				killablenpcs.Clear();
				killablenpcs.AddRange(copyfrom.killablenpcs);

				hasBattles = copyfrom.hasBattles;
			}
		}
		public static void FindRoomTiles(FF1Rom rom, TileSet tileset,
						List<byte> floorTiles,
						List<byte> walkableTiles,
						List<byte> spikeTiles,
						List<byte> doorSpikeTiles,
						List<byte> battleTiles,
						byte randomEncounter)
		{
			bool debug = false;

			List<byte> blankPPUTiles = new();

			for (byte i = 0; i < 128; i++)
			{
				// Go through the pattern table, and find the CHR which
				// is entirely color 1.

				var chr = rom.Get(TILESETPATTERNTABLE_OFFSET + (tileset.Index << 11) + (i * 16), 16);
				var dec = DecodePPU(chr);

				bool blank = true;
				// Check that entire tile is color 1
				for (int j = 0; j < dec.Length; j++)
				{
					if (dec[j] != 1)
					{
						blank = false;
						break;
					}
				}
				if (blank)
				{
					blankPPUTiles.Add(i);
				}
			}

			// Add trap indicator tile
			blankPPUTiles.Add(0x7D);

			for (byte i = 0; i < 128; i++)
			{
				if ((tileset.Tiles[i].Properties.TilePropFunc & TilePropFunc.TP_NOMOVE) != 0)
				{
					continue;
				}
				// Allowed to walk onto this tile

				if (tileset.Tiles[i].Palette != TilePalette.RoomPalette1 && tileset.Tiles[i].Palette != TilePalette.RoomPalette2)
				{
					continue;
				}
				// This tile has the "room" palette

				if (tileset.Tiles[i].Properties.TilePropFunc == TilePropFunc.TP_SPEC_BATTLE)
				{
					if (tileset.Tiles[i].Properties.BattleId != randomEncounter)
					{
						// This is a spike tile
						if (!blankPPUTiles.Contains(tileset.Tiles[i].BottomLeftTile) && !blankPPUTiles.Contains(tileset.Tiles[i].BottomRightTile))
						{
							doorSpikeTiles.Add(i);
						}
						else
						{
							spikeTiles.Add(i);
						}
						
						if (debug) Console.WriteLine($"spike tile {i:X} BattleId {tileset.Tiles[i].Properties.BattleId:X}");
					}
					else
					{
						// This is random battle tile
						battleTiles.Add(i);
						if (debug) Console.WriteLine($"battle tile {i:X}");
					}
					continue;
				}

				if (!blankPPUTiles.Contains(tileset.Tiles[i].TopLeftTile) ||
					!blankPPUTiles.Contains(tileset.Tiles[i].TopRightTile) ||
					!blankPPUTiles.Contains(tileset.Tiles[i].BottomLeftTile) ||
					!blankPPUTiles.Contains(tileset.Tiles[i].BottomRightTile))
				{
					walkableTiles.Add(i);
					continue;
				}
				// The visual tile is "blank" (flips between
				// floor color (black) and ceiling color
				// (generally white/off white).

				// Found a plain, no-encounter floor tile
				floorTiles.Add(i);
				if (debug) Console.WriteLine($"floor tile {i:X}");
			}

		}

		public static void PlaceSpikeTiles(MT19337 rng,
						   List<byte> spikePool,
						   List<byte> doorSpikePool,
						   List<MapElement> placedChests,
						   List<byte> floorTiles,
						   List<byte> battleTiles,
						   List<Room> roomsToSanityCheck)
		{
			bool debug = false;

			var directions = new Direction[] { Direction.Down, Direction.Down, Direction.Down,
			Direction.Right, Direction.Left, Direction.Up };

			int tries = spikePool.Count * 4;
			while (spikePool.Count > 0 && tries > 0)
			{
				var pc = placedChests.PickRandom(rng);
				var dir = directions.PickRandom(rng);
				var me = pc.Neighbor(dir);
				if (floorTiles.Contains(me.Value) || battleTiles.Contains(me.Value))
				{
					me.Value = spikePool[spikePool.Count - 1];
					if (debug) me.Value = 0xD;
					spikePool.RemoveAt(spikePool.Count - 1);
				}
				var roll = rng.Between(1, 20);
				if (roll == 1 && spikePool.Count > 0 && roomsToSanityCheck.Count > 0)
				{
					var rm = roomsToSanityCheck.SpliceRandom(rng);
					rm.start.Value = spikePool[spikePool.Count - 1];
					if (debug) rm.start.Value = 0xD;
					spikePool.RemoveAt(spikePool.Count - 1);
				}
				tries--;
			}

			foreach (var spike in doorSpikePool)
			{
				var rm = roomsToSanityCheck.SpliceRandom(rng);
				var me = rm.start;
				me.Value = spike;
			}
		}

		public async Task shuffleChestLocations(MT19337 rng, StandardMaps maps, TileSetsData tilesets, Teleporters teleporters, MapIndex[] ids, List<(MapIndex, byte)> preserveChests,
							NpcObjectData npcdata, byte randomEncounter, bool spreadPlacement, bool markSpikeTiles,
							List<byte> chestPool, List<byte> spikePool, List<byte> doorSpikePool)
		{
			// For a tileset, I need to determine:
			//
			// * doors and locked doors
			// * floor tiles with the move bit that are empty.

			bool debug = false;

			if (debug) Console.WriteLine($"\nTiles for {ids[0]}");

			bool keepLinkedChests = false;

			var tileset = tilesets[(int)maps[ids[0]].MapTileSet];

			// Go through the all the map tiles and find ones
			// with certain properties.
			List<byte> doorTiles = new() { 0x36, 0x37, 0x3B };
			byte doorNonSpikeTile = 0x07;
			List<byte> floorTiles = new();
			List<byte> walkableTiles = new();
			List<byte> spikeTiles = new();
			List<byte> doorSpikeTiles = new();
			List<byte> battleTiles = new();
				
			FindRoomTiles(rom, tileset, floorTiles, walkableTiles, spikeTiles, doorSpikeTiles, battleTiles, randomEncounter);

			byte vanillaTeleporters = 0x41;
			var smTeleporters = teleporters.StandardMapTeleporters.Where(t => (int)t.Key < vanillaTeleporters).Select(t => t.Value).ToList();

			if (chestPool == null) chestPool = new();
			if (spikePool == null) spikePool = new();
			if (doorSpikePool == null) doorSpikePool = new();

			// To relocate chests in a dungeon (a group of maps)
			//
			// * Find the all the chest tiles and spike tiles
			// * Wipe all the tiles with the floor tile in the tileset
			// * Find all the doors
			// * For each door, flood fill search for floor tiles, other doors, and teleports & record what we found
			// * Also record the positions of NPCs

			List<Room> rooms = new();

			foreach (var mapindex in ids)
			{
				if (debug) Console.WriteLine($"\nFinding rooms for map {mapindex}");

				var map = maps[mapindex].Map;

				List<SCCoords> startCoords = new();
				for (int y = 0; y < 64; y++)
				{
					for (int x = 0; x < 64; x++)
					{
						var tf = tileset.Tiles[map[y, x]].Properties;
						bool wipe = false;
						bool setDoor = false;

						if (tf.TilePropFunc == (TilePropFunc.TP_SPEC_TREASURE | TilePropFunc.TP_NOMOVE))
						{
							bool skip = false;
							foreach (var pc in preserveChests)
							{
								if (mapindex == pc.Item1 && pc.Item2 == map[y, x])
								{
									skip = true;
								}
							}
							if (!skip)
							{
								if (keepLinkedChests || !chestPool.Contains(map[y, x]))
								{
									chestPool.Add(map[y, x]);
								}
								if (debug) Console.WriteLine($"add {map[y, x]:X} to chest pool");
								wipe = true;
							}
						}
						if (spikeTiles.Contains(map[y, x]))
						{
							spikePool.Add(map[y, x]);
							if (debug) Console.WriteLine($"add {map[y, x]:X} to spike pool");
							wipe = true;
						}
						if (doorSpikeTiles.Contains(map[y, x]))
						{
							doorSpikePool.Add(map[y, x]);
							if (debug) Console.WriteLine($"add {map[y, x]:X} to spike pool");
							setDoor = true;
						}
						if (doorTiles.Contains(map[y, x]))
						{
							startCoords.Add(new SCCoords(x, y - 1));
							if (debug) Console.WriteLine($"Found door {map[y, x]:X} at {x},{y}");
						}

						if (wipe)
						{
							map[y, x] = 0xff;
						}

						if (setDoor)
						{
							map[y, x] = doorNonSpikeTile;
						}
					}
				}

				foreach (var t in smTeleporters)
				{
					if (t.Index == mapindex &&
					(floorTiles.Contains(map[t.Coordinates.Y, t.Coordinates.X]) || battleTiles.Contains(map[t.Coordinates.Y, t.Coordinates.X])))
					{
						startCoords.Add(new SCCoords(t.Coordinates.X, t.Coordinates.Y));
					}
				}

				List<(int x, int y)> searched = new();
				int roomCount = 0;
				foreach (var st in startCoords)
				{
					var room = new Room();
					room.MapIndex = mapindex;
					room.RoomIndex = roomCount;
					roomCount++;
					room.start = map[(st.X, st.Y)];
					bool newRoom = true;
					if (debug) Console.WriteLine($"Searching from {st}");

					var logit = false;

					map.Flood((st.X, st.Y), (MapElement me) => {

						if (logit)
						{
							if (debug) Console.WriteLine($"Search {me.Coord} {me.Value:X}");
						}

						bool hasNpc = false;
						bool hasKillableNpc = false;
						for (int i = 0; i < 16; i++)
						{
							var npc = maps[mapindex].MapObjects[i];
							if ((npc.Coords.X, npc.Coords.Y) == me.Coord)
							{
								hasNpc = true;
								if (npcdata != null)
								{
									hasKillableNpc = (npcdata[npc.ObjectId].Script == TalkScripts.Talk_fight ||
											  npcdata[npc.ObjectId].Script == TalkScripts.Talk_kill);
								}
								room.npcs.Add(me);
								if (hasKillableNpc)
								{
									room.killablenpcs.Add(me);
								}
							}
						}

						if (me.Value == 0xff)
						{
							// This square previously contained
							// a chest or spike tile
							if (!hasNpc)
							{
								// not occupied by NPC
								room.floor.Add(me);
								return true;
							}
							// Is occupied by an NPC, but if killable, we search past it.
							return hasKillableNpc;
						}

						if (tileset.Tiles[me.Value].Properties.TilePropFunc == (TilePropFunc.TP_SPEC_TREASURE | TilePropFunc.TP_NOMOVE))
						{
							// A preserved chest (vanilla
							// incentive location) needs to be
							// included in logic checking so
							// we don't accept a placement
							// that puts another chest
							// blocking it.
							room.chests.Add(me);
							return false;
						}

						if (doorTiles.Contains(me.Value))
						{
							room.doors.Add(me);
							if (searched.Contains(me.Coord))
							{
								if (debug) Console.WriteLine($"Saw {me.Coord} already");
								newRoom = false;
							}
							else
							{
								if (debug) Console.WriteLine($"{me.Coord} is new door");
								searched.Add(me.Coord);
							}
							return false;
						}

						if (walkableTiles.Contains(me.Value))
						{
							room.walkable.Add(me);
						}

						bool teleporterTarget = false;
						foreach (var t in smTeleporters)
						{
							if (t.Index == mapindex && me.Coord == (t.Coordinates.X, t.Coordinates.Y))
							{
								if (debug) Console.WriteLine($"Found teleport in {me.Coord}");
								room.teleIn.Add(me);
								teleporterTarget = true;
							}
						}

						if (tileset.Tiles[me.Value].Properties.TilePropFunc == TilePropFunc.TP_SPEC_BATTLE &&
							tileset.Tiles[me.Value].Properties.BattleId == randomEncounter)
						{
							room.hasBattles = true;
						}

						if (!hasNpc && !teleporterTarget && (floorTiles.Contains(me.Value) || battleTiles.Contains(me.Value) || spikeTiles.Contains(me.Value)) &&
							(me.Coord != (st.X, st.Y)))
						{
							room.floor.Add(me);
						}

						if ((tileset.Tiles[me.Value].Properties.TilePropFunc & TilePropFunc.TP_TELE_MASK) != 0)
						{
							if (debug) Console.WriteLine($"Found teleport out {me.Coord}");
							room.teleOut.Add(me);
							return false;
						}

						return (!hasNpc || hasKillableNpc) &&
							(tileset.Tiles[me.Value].Properties.TilePropFunc & TilePropFunc.TP_NOMOVE) == 0;
					});
					if (newRoom)
					{
						if (debug) Console.WriteLine($"Added new room");
						rooms.Add(room);
					}

					foreach (var me in room.floor)
					{
						if (me.Value == 0xff)
						{
							if ((room.hasBattles && battleTiles.Count > 0) || floorTiles.Count == 0)
							{
								me.Value = battleTiles[0];
							}
							else
							{
								me.Value = floorTiles[0];
							}
						}
					}
				}

				for (int y = 0; y < 64; y++)
				{
					for (int x = 0; x < 64; x++)
					{
						if (map[y, x] == 0xff)
						{
							if (floorTiles.Count > 0)
							{
								map[y, x] = floorTiles[0];
							}
							else
							{
								map[y, x] = battleTiles[0];
							}
						}
					}
				}
			}

			// make a copy of the rooms
			List<Room> workingrooms = new(rooms.Count);
			ChestPicker chestPicker = new();

			foreach (var r in rooms)
			{
				workingrooms.Add(new Room());
				chestPicker.AddRoom(r);
			}

			bool needRetry = true;
			List<(Room, MapElement)> placedChests = new();
			List<Room> roomsToSanityCheck = new();
			List<(Room, MapElement)> allCandidates = new();

			int attempts;
			await rom.Progress($"Relocating chests for group of floors containing {ids[0]}", 1);
			for (attempts = 0; needRetry && attempts < 800; attempts++)
			{
				if (attempts % 10 == 0)
				{
					await rom.Progress("", 1);
					System.GC.Collect(System.GC.MaxGeneration);
				}

				// Make a copy of the rooms
				placedChests.Clear();
				roomsToSanityCheck.Clear();
				needRetry = false;

				for (int i = 0; i < rooms.Count; i++)
				{
					// We do this goofy replacement thing
					// instead of just allocating new objects
					// because we need this inner loop to be
					// as close to constant memory usage as
					// possible, otherwise it'll fall over as
					// the number of iterations gets large and
					// it starts grabbing more and more
					// memory.
					workingrooms[i].Replace(rooms[i]);
				}

				allCandidates.Clear();
				foreach (var r in workingrooms)
				{
					foreach (var f in r.floor)
					{
						//f.Value = 0xD;
						allCandidates.Add((r, f));
					}
				}

				if (debug) Console.WriteLine($"rooms {rooms.Count}");

				List<Room> pendingRooms = new(workingrooms);

				var chestCount = chestPool.Count;
				foreach (var c in chestPool)
				{

					(Room, MapElement) me;
					if (spreadPlacement)
					{
						me = chestPicker.GetSpreadCandidate(pendingRooms, workingrooms, chestCount, rng);
					}
					else
					{
						// full random
						me = chestPicker.GetPooledCandidate(chestCount, rng);
					}
					me.Item2.Value = c;
					me.Item1.chests.Add(me.Item2);
					placedChests.Add((me.Item1, me.Item2));
					chestCount--;
					if (!roomsToSanityCheck.Contains(me.Item1))
					{
						roomsToSanityCheck.Add(me.Item1);
					}
				}

				if (debug) Console.WriteLine($"did chest placement");
				/*
				foreach (var r in roomsToSanityCheck)
				{
					r.start.Map.Flood((r.start.X, r.start.Y), (MapElement me) => {
						if (r.doors.Remove(me) || r.chests.Remove(me) || r.teleOut.Remove(me) || r.npcs.Remove(me))
						{
							// found something, don't traverse
							if (!r.killablenpcs.Contains(me))
							{
								return false;
							}
						}
						r.teleIn.Remove(me);
						return (tileset.Tiles[me.Value].Properties.TilePropFunc & TilePropFunc.TP_NOMOVE) == 0;
					});
					if (r.doors.Count > 0 || r.chests.Count > 0 || r.teleOut.Count > 0 || r.npcs.Count > 0)
					{
						if (debug) Console.WriteLine($"Room at {r.MapIndex} {r.start.X}, {r.start.Y} failed sanity check: {r.doors.Count} {r.chests.Count} {r.teleOut.Count} {r.npcs.Count}");
						//needRetry = true;
						break;
					}
				}

				if (needRetry)
				{
					// Clear failed chest attempt
					foreach (var ch in placedChests)
					{
						if ((ch.Item1.hasBattles && battleTiles.Count > 0) || floorTiles.Count == 0)
						{
							ch.Item2.Value = battleTiles[0];
						}
						else
						{
							ch.Item2.Value = floorTiles[0];
						}
					}
				}*/
			}

			if (!needRetry)
			{
				Console.WriteLine($"{ids[0]} success after {attempts} attempts");
			}

			if (needRetry)
			{
				throw new Exception($"{ids[0]} Couldn't place chests after {attempts} attempts");
			}

			// Finally, add spike tiles.
			if (debug) Console.WriteLine($"spikes {placedChests.Count} {spikePool.Count}");

			if (placedChests.Count == 0)
			{
				return;
			}

			/*if (spikePool.Count > placedChests.Count)  {
			Console.WriteLine($"WARNING spikePool.Count > placedChests.Count something is wrong");
			return;
			}*/

			if (markSpikeTiles)
			{
				var ts = tilesets[(int)maps[ids[0]].MapTileSet];
				foreach (var sp in spikePool)
				{
					tileset.Tiles[sp].TopRightTile = 0x7D;
				}

				foreach (var sp in doorSpikePool)
				{
					tileset.Tiles[sp].TopRightTile = 0x7D;
				}
			}

			PlaceSpikeTiles(rng, spikePool, doorSpikePool,
					placedChests.Select((pc) => pc.Item2).ToList(),
					floorTiles,
					battleTiles, roomsToSanityCheck);

			//
			// * Place each chest tile randomly on a room floor tiles
			// * Place the spike tile randomly on a room floor tile adjacent to chest tiles
			// ** Weighting 40% below chest, 20% left/right/top
			// * Finally, sanity check each room that we can reach all chests, doors, teleports and NPCs in the room
		}
		public async Task RandomlyRelocateChests(MT19337 rng, StandardMaps maps, TileSetsData tilesets, Teleporters teleporters,  NpcObjectData npcdata, Flags flags)
		{
			if (!(bool)flags.RelocateChests || flags.GameMode == GameModes.DeepDungeon)
			{
				return;
			}

			// Groups of maps that make up a shuffle pool
			// They need to all use the same tileset.
			List<MapIndex[]> dungeons = new() {
			new MapIndex[] { MapIndex.ConeriaCastle1F, MapIndex.ConeriaCastle2F, MapIndex.ElflandCastle, MapIndex.NorthwestCastle },

			new MapIndex[] { MapIndex.MarshCaveB1, MapIndex.MarshCaveB2, MapIndex.MarshCaveB3 }, // Marsh
			new MapIndex[] { MapIndex.IceCaveB1, MapIndex.IceCaveB2, MapIndex.IceCaveB3 }, // Ice Cave
			new MapIndex[] { MapIndex.CastleOrdeals1F, MapIndex.CastleOrdeals2F, MapIndex.CastleOrdeals3F }, // Ordeals
			new MapIndex[] { MapIndex.MirageTower1F, MapIndex.MirageTower2F, MapIndex.MirageTower3F }, // Mirage
			new MapIndex[] { MapIndex.TempleOfFiendsRevisited1F,
				MapIndex.TempleOfFiendsRevisited2F,
				MapIndex.TempleOfFiendsRevisited3F,
				MapIndex.TempleOfFiendsRevisitedEarth,
				MapIndex.TempleOfFiendsRevisitedFire,
				MapIndex.TempleOfFiendsRevisitedWater,
				MapIndex.TempleOfFiendsRevisitedAir }, // ToFR

			// new MapIndex[] { MapIndex.TitansTunnel }, // Titan
		    };

			List<MapIndex[]> spreadPlacementDungeons = new() {
			new MapIndex[] { MapIndex.Waterfall, MapIndex.DwarfCave, MapIndex.MatoyasCave, MapIndex.SardasCave },
			new MapIndex[] { MapIndex.Cardia, MapIndex.BahamutCaveB1, MapIndex.BahamutCaveB2 }, // Cardia
			new MapIndex[] { MapIndex.SkyPalace1F, MapIndex.SkyPalace2F, MapIndex.SkyPalace3F, MapIndex.SkyPalace4F, MapIndex.SkyPalace5F }, // Sky Castle
			new MapIndex[] { MapIndex.TempleOfFiends }, // ToF
		    };

			List<(MapIndex, byte)> preserveChests = new();

			if ((bool)flags.IncentivizeMarsh && flags.MarshIncentivePlacementType == IncentivePlacementType.Vanilla)
			{
				preserveChests.Add((MapIndex.MarshCaveB3, 0x49));
			}

			bool addearth = true;
			if ((bool)flags.IncentivizeEarth)
			{
				if (flags.EarthIncentivePlacementType == IncentivePlacementTypeEarth.Vanilla)
				{
					preserveChests.Add((MapIndex.EarthCaveB3, 0x51));
				}

				if (flags.EarthIncentivePlacementType == IncentivePlacementTypeEarth.RandomPreRod ||
					flags.EarthIncentivePlacementType == IncentivePlacementTypeEarth.RandomPostRod)
				{
					// Split shuffle before/after gating
					dungeons.Add(new MapIndex[] { MapIndex.EarthCaveB1, MapIndex.EarthCaveB2, MapIndex.EarthCaveB3 });
					dungeons.Add(new MapIndex[] { MapIndex.EarthCaveB4, MapIndex.EarthCaveB5 });
					addearth = false;
				}
			}
			if (addearth)
			{
				dungeons.Add(new MapIndex[] { MapIndex.EarthCaveB1, MapIndex.EarthCaveB2, MapIndex.EarthCaveB3, MapIndex.EarthCaveB4, MapIndex.EarthCaveB5 });
			}



			bool addvolcano = true;
			if ((bool)flags.IncentivizeVolcano && flags.VolcanoIncentivePlacementType == IncentivePlacementTypeVolcano.Vanilla)
			{
				preserveChests.Add((MapIndex.GurguVolcanoB5, 0x7E));
			}
			else if (flags.VolcanoIncentivePlacementType == IncentivePlacementTypeVolcano.RandomShallow ||
					 flags.VolcanoIncentivePlacementType == IncentivePlacementTypeVolcano.RandomDeep)
			{
				// Split shuffle before/after gating
				dungeons.Add(new MapIndex[] { MapIndex.GurguVolcanoB1, MapIndex.GurguVolcanoB2 });
				dungeons.Add(new MapIndex[] { MapIndex.GurguVolcanoB3, MapIndex.GurguVolcanoB4, MapIndex.GurguVolcanoB5 });
				addvolcano = false;
			}
			if (addvolcano)
			{
				dungeons.Add(new MapIndex[] { MapIndex.GurguVolcanoB1, MapIndex.GurguVolcanoB2, MapIndex.GurguVolcanoB3, MapIndex.GurguVolcanoB4, MapIndex.GurguVolcanoB5 });
			}


			if ((bool)flags.IncentivizeIceCave && flags.IceCaveIncentivePlacementType == IncentivePlacementType.Vanilla)
			{
				preserveChests.Add((MapIndex.IceCaveB2, 0x5F));
			}

			if ((bool)flags.IncentivizeOrdeals && flags.OrdealsIncentivePlacementType == IncentivePlacementType.Vanilla)
			{
				preserveChests.Add((MapIndex.CastleOrdeals3F, 0x78));
			}

			bool addsea = true;
			if ((bool)flags.IncentivizeSeaShrine)
			{
				if (flags.SeaShrineIncentivePlacementType == IncentivePlacementTypeSea.Vanilla)
				{
					preserveChests.Add((MapIndex.SeaShrineB1, 0x7B));
				}

				if (flags.SeaShrineIncentivePlacementType == IncentivePlacementTypeSea.RandomUnlocked ||
					flags.SeaShrineIncentivePlacementType == IncentivePlacementTypeSea.RandomLocked)
				{
					if ((bool)flags.MermaidPrison)
					{
						// Split shuffle before/after gating
						dungeons.Add(new MapIndex[] { MapIndex.SeaShrineB2, MapIndex.SeaShrineB3, MapIndex.SeaShrineB4, MapIndex.SeaShrineB5 });
						dungeons.Add(new MapIndex[] { MapIndex.SeaShrineB1 }); //Mermaid Floor
						preserveChests.Add((MapIndex.SeaShrineB2, 0x6C));
						addsea = false;
					}
					else
					{
						preserveChests.Add((MapIndex.SeaShrineB2, 0x6C)); // TFC
					}
				}
			}
			if (addsea)
			{
				dungeons.Add(new MapIndex[] { MapIndex.SeaShrineB1, MapIndex.SeaShrineB2, MapIndex.SeaShrineB3, MapIndex.SeaShrineB4, MapIndex.SeaShrineB5 });
			}

			if ((bool)flags.IncentivizeConeria)
			{
				if (flags.CorneriaIncentivePlacementType == IncentivePlacementType.Vanilla)
				{
					preserveChests.Add((MapIndex.ConeriaCastle1F, 0x65));
				}
				if (flags.CorneriaIncentivePlacementType == IncentivePlacementType.RandomAtLocation)
				{
					preserveChests.Add((MapIndex.ConeriaCastle1F, 0x63));
					preserveChests.Add((MapIndex.ConeriaCastle1F, 0x64));
					preserveChests.Add((MapIndex.ConeriaCastle1F, 0x65));
					preserveChests.Add((MapIndex.ConeriaCastle1F, 0x66));
					preserveChests.Add((MapIndex.ConeriaCastle1F, 0x67));
					preserveChests.Add((MapIndex.ConeriaCastle1F, 0x68));
				}
			}

			if ((bool)flags.IncentivizeMarshKeyLocked)
			{
				if (flags.MarshLockedIncentivePlacementType == IncentivePlacementType.Vanilla)
				{
					preserveChests.Add((MapIndex.MarshCaveB3, 0x4D));
				}
				if (flags.MarshLockedIncentivePlacementType == IncentivePlacementType.RandomAtLocation)
				{
					preserveChests.Add((MapIndex.MarshCaveB3, 0x4B));
					preserveChests.Add((MapIndex.MarshCaveB3, 0x4C));
					preserveChests.Add((MapIndex.MarshCaveB3, 0x4D));
				}
			}

			if ((bool)flags.IncentivizeSkyPalace && flags.SkyPalaceIncentivePlacementType == IncentivePlacementTypeSky.Vanilla)
			{
				preserveChests.Add((MapIndex.SkyPalace2F, 0x5F));
			}

			if ((bool)flags.IncentivizeCardia && flags.CardiaIncentivePlacementType == IncentivePlacementType.Vanilla)
			{
				preserveChests.Add((MapIndex.Cardia, 0x6B));
			}

			foreach (MapIndex[] b in dungeons)
			{
				await shuffleChestLocations(rng, maps, tilesets, teleporters, b, preserveChests, npcdata,
							  0x00,
								false, flags.RelocateChestsTrapIndicator,
								null, null, null);
			}

			foreach (MapIndex[] b in spreadPlacementDungeons)
			{
				await shuffleChestLocations(rng, maps, tilesets, teleporters, b, preserveChests, npcdata,
							  0x00,
								true, flags.RelocateChestsTrapIndicator,
								null, null, null);
			}
		}
	}
}
