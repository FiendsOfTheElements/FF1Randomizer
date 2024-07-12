namespace FF1Lib.Procgen
{
	class CellularGenerator : IMapGeneratorEngine
	{
		public int Steps = 5;

		private const byte SentinelAlive = 0xFF; // Unused tile range for generation usage
		private const byte SentinelDead = 0xFE; // Unused tile range for generation usage

		public CellularGenerator()
		{
		}

		public CompleteMap Generate(MT19337 rng, MapRequirements reqs)
		{
			int sanity = 0;
			while (true)
			{
				if (++sanity == 500)
					throw new InsaneException("Failed to generate map!");

				try
				{
					CompleteMap complete = new CompleteMap
					{
						Map = new Map(SentinelDead),
						Requirements = reqs,
					};

					{
						(int x, int y) coord = (0, 0);
						reqs.Rooms.ToList().ForEach(room =>
						{
							coord.x = rng.Between(0, MapRequirements.Width - 1 - room.Width);
							complete.Map.Put(coord, room.Tiledata);
							coord.y += room.Height + rng.Between(0, 5);

							room.NPCs.ToList().ForEach(npc =>
							{
								npc.Coord.x += coord.x;
								npc.Coord.y += coord.y;
								//reqs.Rom.SetNpc(reqs.MapIndex, npc);
								reqs.MapObjects.SetNpc(npc.Index, npc.ObjectId, npc.Coord.x, npc.Coord.y, npc.InRoom, npc.Stationary);

							});
						});
					}

					// Generate rooms first. We'll aim to have enough for all the required chests.
					Map rooms = complete.Map.Clone();
					InitializeMap(rng, rooms, 0.33);
					rooms = DoSimulationStep(rooms, 4, 2, 3);

					Console.WriteLine($"Room map has {rooms.Count(element => element.Value == SentinelAlive)} walkable tiles.");
					complete.Map = rooms;
					Console.Write(complete.AsText());

					var clone = rooms.Clone();
					foreach (var el in clone.Where(el => el.Value == SentinelDead))
					{
						var roomTile = rooms[el.Coord];

						foreach (var newTile in roomTile.Surrounding().Concat(roomTile.Left().Surrounding()))
						{
							newTile.Value = SentinelDead;
						}
					}

					Console.WriteLine($"Room map has {rooms.Count(element => element.Value == SentinelAlive)} walkable tiles.");

					complete.Map = rooms;
					Console.Write(complete.AsText());

					clone = rooms.Clone();
					foreach (var el in clone.Where(el => el.Value == SentinelDead))
					{
						var roomTile = rooms[el.Coord];
						if (el.Left().Value == SentinelAlive)
						{
							if (el.Up().Value == SentinelAlive)
							{
								roomTile.Tile = Tile.RoomBackLeft;
							}
							else if (el.Down().Value == SentinelAlive)
							{
								roomTile.Tile = Tile.RoomFrontLeft;
								roomTile.Down().Tile = Tile.InsideWall;
							}
							else
							{
								roomTile.Tile = Tile.RoomLeft;
							}
						}
						else if (el.Right().Value == SentinelAlive)
						{
							if (el.Up().Value == SentinelAlive)
							{
								roomTile.Tile = Tile.RoomBackRight;
							}
							else if (el.Down().Value == SentinelAlive)
							{
								roomTile.Tile = Tile.RoomFrontRight;
								roomTile.Down().Tile = Tile.InsideWall;
							}
							else
							{
								roomTile.Tile = Tile.RoomRight;
							}
						}
						else if (el.Up().Value == SentinelAlive)
						{
							roomTile.Tile = Tile.RoomBackCenter;
						}
						else if (el.Down().Value == SentinelAlive)
						{
							roomTile.Tile = Tile.RoomFrontCenter;
							roomTile.Down().Tile = Tile.InsideWall;
						}
						else
						{
							roomTile.Tile = Tile.RoomCenter;
						}
					}

					// Carve out a door to all the accessible room tiles.
					var roomTiles = rooms.Where(el => el.Tile == Tile.RoomCenter);
					var doorways = new List<MapElement> { };

					foreach(var innerTile in roomTiles)
					{
						var results = FloodFill(rooms, innerTile.Coord, new List<Tile> { Tile.RoomCenter, Tile.RoomFrontCenter, Tile.Door });
						if (results[Tile.Door].Any()) {
							continue;
						}

						var potentialDoorways = results[Tile.RoomFrontCenter];
						if (potentialDoorways.Any())
						{
							var entryway = potentialDoorways.SpliceRandom(rng);
							var door = entryway.Down();
							var doorway = door.Down();

							System.Diagnostics.Debug.Assert(door.Tile == Tile.InsideWall);

							if (doorway.Value != SentinelAlive)
							{
								throw new InsaneException("Doorway not available.");
							}

							door.Tile = Tile.Door;
							doorway.Tile = Tile.Doorway;
							doorways.Add(doorway);
						}

					}

					// Place chests now
					var chestLocations = roomTiles.Where(el => el.Up().Tile == Tile.RoomBackCenter).ToList();
					if (reqs.Objects.Count() > chestLocations.Count())
						throw new InsaneException("Not enough chest locations.");

					foreach(byte chest in reqs.Objects)
					{
						chestLocations.SpliceRandom(rng).Value = chest;
					}

					// Deaden to prepare for second run
					//foreach (var el in rooms.Where(el => el.Value == SentinelAlive)) { el.Value = SentinelDead; }

					// Place non-room walls now.
					/*
					for (int sanity2 = 0; sanity2 <501; ++sanity2)
					{
						if (sanity2 == 500)
							throw new InsaneException("Couldn't make floors walls.");

						var walled = rooms.Clone();
						var entrances = doorways.ToList();
						//InitializeMap(rng, walled, 0.4);
						walled = DoSimulationStep(walled, 5, 4, 4);

						entrances.ForEach(entrance =>
						{
							foreach (var tile in entrance.Surrounding().Where(el => el.Value == SentinelDead))
							{
								walled[tile.Coord].Value = SentinelAlive;
							}
						});

						complete.Map = walled;
						Console.Write(complete.AsText());
						bool success = false; // eww

						// Find a big enough segment to be the main area.
						while (walled.Any(el => el.Value == SentinelAlive))
						{
							var tile = walled.First(el => el.Value == SentinelAlive);
							var results = FloodFill(walled, tile.Coord, new List<Tile> { reqs.Floor, Tile.Doorway }, new List<Tile> { (Tile)SentinelAlive }, reqs.Floor);

							if (results[reqs.Floor].Count() < 500)
							{
								// Small section just mark as walls.
								FloodFill(walled, tile.Coord, new List<Tile> { reqs.Floor }, new List<Tile> { reqs.Floor }, (Tile)SentinelDead);
							}
							else
							{
								// This is the big section. Make sure all the doors are accessible
								success = results[Tile.Doorway].Count() == doorways.Count();
								break;
							}
						}

						if (success)
						{
							foreach (var el in walled.Where(el => el.Value == SentinelDead))
							{
								if (el.Up().Tile == reqs.Floor || el.Down().Tile == reqs.Floor || el.Left().Tile == reqs.Floor || el.Right().Tile == reqs.Floor)
								{
									el.Tile = reqs.Barrier;
								} else
								{
									el.Tile = reqs.OutOfBounds;
								}
							}
							complete.Map = walled;
							break;
						}
					}
					*/

					// All the tiles we're editing are now either SentinelAlive or SentinelDead.
					// Time to map those to real values now.
					foreach (var el in complete.Map.Where(el => el.Value == SentinelAlive)) { el.Tile = reqs.Floor; }

					Console.WriteLine($"Room map has {complete.Map.Count(element => element.Tile == reqs.Floor)} walkable tiles.");
					Console.Write(complete.AsText());

					// Pad all the Alive tiles
					var locations = complete.Map.Where(element => element.Tile == reqs.Floor).ToList();
					reqs.Portals.ToList().ForEach(portal =>
					{
						var location = locations.SpliceRandom(rng);
						complete.Map[location.Y, location.X] = portal;

						if (portal == (byte)Tile.WarpUp)
						{
							complete.Entrance = new Coordinate((byte)location.X, (byte)location.Y, CoordinateLocale.Standard);

							var finalResult = FloodFill(complete.Map, location.Coord, new List<Tile> { reqs.Floor, Tile.WarpUp, Tile.Doorway, Tile.Door, Tile.RoomCenter, Tile.RoomFrontCenter });
							if (finalResult[Tile.Door].Count() < doorways.Count())
							{
								throw new InsaneException("Can't reach all rooms.");
							}
						}
					});

					Console.Write(complete.AsText());
					return complete;
				
				} catch(InsaneException e)
				{
					Console.WriteLine(e.ToString());
				}
			}
		}

		private void InitializeMap(MT19337 rng, Map map, double initialAliveChance)
		{
			map.Where(element => element.Value == SentinelDead && (double)rng.Next() / uint.MaxValue < initialAliveChance)
				.ToList().ForEach(element => element.Value = SentinelAlive);
		}

		private Dictionary<Tile, List<MapElement>> FloodFill(Map map, (int, int) coord, IEnumerable<Tile> counts, IEnumerable<Tile> finds = null, Tile replace = Tile.FloorSafe)
		{
			var results = counts.ToDictionary(tile => tile, tile => new List<MapElement> { });
			FloodFill(results, map, coord, counts, finds, replace);
			return results;
		}

		private void FloodFill(Dictionary<Tile, List<MapElement>> results, Map map, (int, int) inCoord, IEnumerable<Tile> counts, IEnumerable<Tile> finds = null, Tile replace = Tile.FloorSafe)
		{
			map.Flood(inCoord, element =>
			{
				// Update if desired
				bool? found = finds?.Contains(element.Tile);
				if (found.HasValue && found.Value)
				{
					element.Tile = replace;
				}

				// Accumulate results and recurse if this is a counter tile.
				if (counts.Contains(element.Tile))
				{
					results[element.Tile].Add(element);
					return true;
				}

				return false;
			});
		}

		private Map DoSimulationStep(Map old, int steps, int birthLimit, int deathLimit)
		{
			for (int i = 0; i < steps; ++i)
			{
				Map map = old.Clone();
				// Loop over each row and column of the map

				foreach (var element in old)
				{
					int x = element.X;
					int y = element.Y;
					int nbs = CountAliveNeighbours(old, x, y);
					//The new value is based on our simulation rules
					//First, if a cell is alive but has too few neighbours, kill it.
					if (element.Value == SentinelAlive)
					{
						if (nbs < deathLimit)
						{
							map[y, x] = SentinelDead;
						}
						else
						{
							map[y, x] = SentinelAlive;
						}
					} //Otherwise, if the cell is dead now, check if it has the right number of neighbours to be 'born'
					else if (element.Value == SentinelDead)
					{
						if (nbs > birthLimit)
						{
							map[y, x] = SentinelAlive;
						}
						else
						{
							map[y, x] = SentinelDead;
						}
					}
					else
					{
						map[y, x] = element.Value;
					}
				}

				old = map;
			}

			return old;
		}

		//Returns the number of cells in a ring around (x,y) that are alive.
		public int CountAliveNeighbours(Map map, int x, int y)
		{
			int count = 0;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					if (i == 0 && j == 0) continue;

					if (map[(y + j + MapRequirements.Height) % MapRequirements.Height, (x + i + MapRequirements.Width) % MapRequirements.Width] == SentinelAlive)
					{
						count = count + 1;
					}
				}
			}
			return count;
		}
	}
}
