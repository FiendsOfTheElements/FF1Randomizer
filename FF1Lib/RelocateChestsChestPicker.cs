using FF1Lib.Sanity;

namespace FF1Lib
{
	public partial class RelocateChests
	{
		public class TileCandidate
		{
			public MapElement Tile;
			public MapIndex MapIndex;
			public int RoomIndex;
			public SCCoords Coords;
			public Room Room;
			public List<TileCandidate> Neighbours;
			public bool Walkable;
			public bool Placeable;
			public bool Placed;
			public TileCandidate(MapElement _tile, Room _room)
			{
				Tile = _tile;
				Room = _room;
				MapIndex = _room.MapIndex;
				RoomIndex = _room.RoomIndex;
				Coords = new SCCoords(_tile.X, _tile.Y);

				Neighbours = new();

				Placed = false;
				Walkable = false;
				Placeable = false;
			}
			public TileCandidate(MapElement _tile, Room _room, bool _placed, bool _walkable, bool _placeable)
			{
				Tile = _tile;
				Room = _room;
				MapIndex = _room.MapIndex;
				RoomIndex = _room.RoomIndex;
				Coords = new SCCoords(_tile.X, _tile.Y);

				Neighbours = new();

				Placed = _placed;
				Walkable = _walkable;
				Placeable = _placeable;
			}
			public bool IsBlocking()
			{
				if (Neighbours.Where(n => n.Placed).Any())
				{
					return true;
				}
				else
				{
					bool[,] walkArray = new bool[3, 3];

					walkArray[1, 1] = false;
					bool ulcorner = false;
					bool urcorner = false;
					bool llcorner = false;
					bool lrcorner = false;

					TileCandidate foundNeighbor;
					foreach (var neighbor in Neighbours)
					{
						walkArray[1 + neighbor.Coords.X - Coords.X, 1 + neighbor.Coords.Y - Coords.Y] = neighbor.Walkable;
						if (!ulcorner && neighbor.Neighbours.TryFind(n => n.Coords.X == Coords.X - 1 && n.Coords.Y == Coords.Y - 1, out foundNeighbor))
						{
							walkArray[0, 0] = foundNeighbor.Walkable;
							ulcorner = true;
						}

						if (!urcorner && neighbor.Neighbours.TryFind(n => n.Coords.X == Coords.X + 1 && n.Coords.Y == Coords.Y - 1, out foundNeighbor))
						{
							walkArray[2, 0] = foundNeighbor.Walkable;
							urcorner = true;
						}

						if (!llcorner && neighbor.Neighbours.TryFind(n => n.Coords.X == Coords.X - 1 && n.Coords.Y == Coords.Y + 1, out foundNeighbor))
						{
							walkArray[0, 2] = foundNeighbor.Walkable;
							llcorner = true;
						}
						if (!lrcorner && neighbor.Neighbours.TryFind(n => n.Coords.X == Coords.X + 1 && n.Coords.Y == Coords.Y + 1, out foundNeighbor))
						{
							walkArray[2, 2] = foundNeighbor.Walkable;
							lrcorner = true;
						}
					}

					if (walkArray[1, 0] && walkArray[1, 2] &&
						!((walkArray[0, 0] && walkArray[0, 1] && walkArray[0, 2]) || (walkArray[2, 0] && walkArray[2, 1] && walkArray[2, 2])))
					{
						return true;
					}

					if (walkArray[0, 1] && walkArray[2, 1] &&
						!((walkArray[0, 0] && walkArray[1, 0] && walkArray[2, 0]) || (walkArray[0, 2] && walkArray[1, 2] && walkArray[2, 2])))
					{
						return true;
					}

					if (walkArray[1, 0] && walkArray[0, 1] && !walkArray[0, 0])
					{
						return true;
					}

					if (walkArray[1, 0] && walkArray[2, 1] && !walkArray[2, 0])
					{
						return true;
					}

					if (walkArray[1, 2] && walkArray[2, 1] && !walkArray[2, 2])
					{
						return true;
					}

					if (walkArray[1, 2] && walkArray[0, 1] && !walkArray[0, 2])
					{
						return true;
					}

					return false;
				}

			}
		}
		public class ChestPicker
		{
			public List<TileCandidate> Candidates;
			public ChestPicker()
			{
				Candidates = new();
			}
			private void CrawlNeighbours(TileCandidate originaltile, TileCandidate candidate, List<TileCandidate> crawledCandidates)
			{
				crawledCandidates.Add(candidate);

				foreach (var neighbour in candidate.Neighbours.Where(n => !crawledCandidates.Contains(n)))
				{
					if (neighbour != originaltile && neighbour.Walkable)
					{
						CrawlNeighbours(originaltile, neighbour, crawledCandidates);
					}
					else
					{
						crawledCandidates.Add(neighbour);
					}
				}
			}
			public void CrawlCorridor(TileCandidate currentTile, List<TileCandidate> singleaccessTiles)
			{
				singleaccessTiles.Add(currentTile);

				var walkableNeighbours = currentTile.Neighbours.Except(singleaccessTiles).Where(t => t.Walkable).ToList();
				if (walkableNeighbours.Count == 1)
				{
					CrawlCorridor(walkableNeighbours.First(), singleaccessTiles);
				}
			}
			public (Room, MapElement) GetSpreadCandidate(List<Room> pendingRooms, List<Room> workingRooms, int chestCount, MT19337 rng)
			{
				TileCandidate pickedCandidate;

				int sanityCount = 0;
				do
				{
					if (pendingRooms.Count > 0)
					{
						// Make sure every room gets a chest
						pickedCandidate = GetCandidate(pendingRooms.SpliceRandom(rng), chestCount, rng);
					}
					else
					{
						// Every room has a chest so allocate the remaining chests.
						pickedCandidate = GetCandidate(workingRooms.PickRandom(rng), chestCount, rng);
					}
					// If we drew a room with no available floor spaces
					sanityCount++;
				} while (pickedCandidate == null && sanityCount < 100);

				if (pickedCandidate != null)
				{
					pickedCandidate.Placed = true;
					pickedCandidate.Placeable = false;
					pickedCandidate.Walkable = false;
				}
				else
				{
					// if we get here something went horribly wrong (placement logic failed or there's more chests than available tiles for some reason)
					throw new Exception("Chests placement error: no valid location found");
				}
				return (pickedCandidate.Room, pickedCandidate.Tile);
			}
			public (Room, MapElement) GetPooledCandidate(int chestCount, MT19337 rng)
			{
				var pickedCandidate = GetCandidate(null, chestCount, rng);

				if (pickedCandidate != null)
				{
					pickedCandidate.Placed = true;
					pickedCandidate.Placeable = false;
					pickedCandidate.Walkable = false;
				}
				else
				{
					// if we get here something went horribly wrong (placement logic failed or there's more chests than available tiles for some reason)
					throw new Exception("Chests placement error: no valid location found");
				}

				return (pickedCandidate.Room, pickedCandidate.Tile);
			}

			public TileCandidate GetCandidate(Room room, int chestCount, MT19337 rng)
			{
				var toSeek = Candidates.Where(p => !p.Placed && p.Placeable && ((room != null) ? p.RoomIndex == room.RoomIndex : true)).ToList();
				List<TileCandidate> potentialUnreachable = new();
				List<TileCandidate> corridorTiles = new();

				int seekLoop = room != null ? 10 : 100;
				int loop = 0;

				TileCandidate candidate = null;
				bool candidateFound = false;

				while (loop < seekLoop)
				{
					loop++;

					// If there's no placeable tile left, bail
					if (!toSeek.Where(n => n.Placeable).Any())
					{
						return null;
					}

					// Pick Random tile
					candidate = toSeek.PickRandom(rng);

					List<TileCandidate> reachedCandidates = new();

					// Get candidate's room tiles
					potentialUnreachable = toSeek.Where(p => p.Placeable && p.Room.MapIndex == candidate.MapIndex && p.Room.RoomIndex == candidate.RoomIndex).ToList();

					// Remove isolated candidates outright and pick a new one
					if (candidate.Neighbours.Count(n => n.Walkable) == 0 ||
						candidate.Neighbours.Where(n => n.Placed && n.Neighbours.Count(n => n.Walkable) == 1).Any())
					{
						candidate.Placeable = false;
						continue;
					}

					// Check if you can go around the tile, skip crawling the map if so
					if (candidate.IsBlocking())
					{
						CrawlNeighbours(candidate, Candidates.Find(c => c.Tile == candidate.Room.start), reachedCandidates);
					}
					else
					{
						// We assume that all tiles are still reachable if we can go around the picked tile
						reachedCandidates = Candidates.Where(p => (p.Placed || p.Placeable) && p.MapIndex == candidate.MapIndex && p.RoomIndex == candidate.RoomIndex).ToList();
					}

					var totalPlacedCount = Candidates.Where(c => c.RoomIndex == candidate.RoomIndex && c.MapIndex == candidate.MapIndex).Count(c => c.Placed);
					var reachedPlacedCount = reachedCandidates.Count(c => c.Placed);
					var validTiles = Candidates.Where(c => (c.RoomIndex == candidate.RoomIndex && c.MapIndex == candidate.MapIndex) || c.MapIndex != candidate.MapIndex).Count(c => c.Placeable);
					var reachedTiles = reachedCandidates.Count(c => c.Placeable);

					// If we couldn't reach all placed tiles, this is a bad tile, mark it and remove it from the pool
					if (totalPlacedCount != reachedPlacedCount)
					{
						candidate.Placeable = false;
						toSeek.Remove(candidate);
					}
					else
					{
						// 1 tile wide corridors are basically a single location, so we find the corridor and remove it from contention once placed
						var walkablesNeighbours = candidate.Neighbours.Intersect(reachedCandidates).Where(n => n.Walkable).ToList();
						corridorTiles = new();
						int corridorTilesCount = 0;

						if (walkablesNeighbours.Count == 1)
						{
							CrawlCorridor(walkablesNeighbours.First(), corridorTiles);
							corridorTilesCount = corridorTiles.Where(n => n.Placeable).Count();
						}

						// can we still place everything once this tile is used?
						if ((validTiles + reachedTiles - corridorTilesCount) >= chestCount)
						{
							candidateFound = true;
							// get unreachable locations
							potentialUnreachable = potentialUnreachable.Except(reachedCandidates.Where(c => c.Placeable)).ToList();
							break;
						}
					}
				}

				if (candidateFound)
				{
					potentialUnreachable.ForEach(c => c.Placeable = false);
					potentialUnreachable.ForEach(c => c.Walkable = false);
					corridorTiles.ForEach(c => c.Placeable = false);
					return candidate;
				}
				else
				{
					return null;
				}
			}
			public void AddRoom(Room room)
			{
				List<TileCandidate> candidates = new();

				candidates.Add(new TileCandidate(room.start, room, false, true, false));

				var poiElement = room.chests.Concat(room.npcs.Except(room.killablenpcs)).Concat(room.teleOut).Concat(room.doors).ToList();

				foreach (var element in poiElement)
				{
					candidates.Add(new TileCandidate(element, room, true, false, false));
				}

				poiElement.Add(room.start);

				foreach (var element in room.teleIn.Except(poiElement))
				{
					candidates.Add(new TileCandidate(element, room, true, true, false));
				}

				poiElement.AddRange(room.teleIn);

				foreach (var element in room.killablenpcs.Except(poiElement))
				{
					candidates.Add(new TileCandidate(element, room, false, true, false));
				}

				poiElement.AddRange(room.killablenpcs);

				foreach (var element in room.walkable.Except(poiElement))
				{
					candidates.Add(new TileCandidate(element, room, false, true, false));
				}

				foreach (var element in room.floor.Except(poiElement))
				{
					candidates.Add(new TileCandidate(element, room, false, true, true));
				}

				foreach (var candidate in candidates)
				{
					CrawlRoom(candidate, candidates);
				}

				Candidates.AddRange(candidates);
			}
			private void CrawlRoom(TileCandidate currentTile, List<TileCandidate> candidates)
			{
				List<(int x, int y)> offsets = new() { (0, 1), (0, -1), (1, 0), (-1, 0) };
				foreach (var offset in offsets)
				{
					if (candidates.TryFind(c => c.Coords.X == currentTile.Coords.X + offset.x && c.Coords.Y == currentTile.Coords.Y + offset.y && c.MapIndex == currentTile.MapIndex, out var foundneighbour))
					{
						currentTile.Neighbours.Add(foundneighbour);
					}
				}
			}
		}
	}
}
