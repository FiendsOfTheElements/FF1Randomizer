using FF1Lib.Sanity;
using MapGenerationTask = FF1Lib.Procgen.ProgenFramework.GenerationTaskType<FF1Lib.Procgen.MapResult>;

namespace FF1Lib.Procgen
{

    public partial class MapState {
	public async Task<MapResult> WipeMap(byte tile) {
	    this.OwnTilemap();

	    for (int y = 0; y < MAPSIZE; y++) {
		for (int x = 0; x < MAPSIZE; x++) {
		    this.Tilemap[y, x] = tile;
		}
	    }

	    return await this.NextStep();
	}

	public async Task<MapResult> PlaceShop() {
	    this.RenderFeature(new SCCoords(5, 5), DungeonTiles.BLACK_MAGIC_SHOP.Tiles);
	    return await this.NextStep();
	}

	public enum Direction : int {
	    Up = 0,
	    Right = 1,
	    Left = 2,
	    Down = 3
	};

	public static List<SCCoords> InnerEdge(SCCoords cp, int w, int h) {
	    HashSet<SCCoords> points = new ();
	    for (int x = 0; x < w; x++) {
		for (int y = 0; y < h; y++) {
		    // top edge
		    int px = cp.X+x;
		    int py = cp.Y;
		    px = ((px%MapState.MAPSIZE) + MapState.MAPSIZE) % MapState.MAPSIZE;
		    py = ((py%MapState.MAPSIZE) + MapState.MAPSIZE) % MapState.MAPSIZE;
		    points.Add(new SCCoords(px, py));

		    // right edge
		    px = cp.X+w-1;
		    py = cp.Y+y;
		    px = ((px%MapState.MAPSIZE) + MapState.MAPSIZE) % MapState.MAPSIZE;
		    py = ((py%MapState.MAPSIZE) + MapState.MAPSIZE) % MapState.MAPSIZE;
		    points.Add(new SCCoords(px, py));

		    // bottom edge
		    px = cp.X+x;
		    py = cp.Y+h-1;
		    px = ((px%MapState.MAPSIZE) + MapState.MAPSIZE) % MapState.MAPSIZE;
		    py = ((py%MapState.MAPSIZE) + MapState.MAPSIZE) % MapState.MAPSIZE;
		    points.Add(new SCCoords(px, py));

		    // left edge
		    px = cp.X;
		    py = cp.Y+y;
		    px = ((px%MapState.MAPSIZE) + MapState.MAPSIZE) % MapState.MAPSIZE;
		    py = ((py%MapState.MAPSIZE) + MapState.MAPSIZE) % MapState.MAPSIZE;
		    points.Add(new SCCoords(px, py));
		}
	    }
	    return new List<SCCoords>(points);
	}

	public static List<SCCoords> OuterEdge(SCCoords cp, int w, int h) {
	    HashSet<SCCoords> points = new ();
	    for (int x = 0; x < w+2; x++) {
		for (int y = 0; y < h+2; y++) {
		    // top edge
		    var px = cp.X-1+x;
		    var py = cp.Y-1;
		    px = ((px%MapState.MAPSIZE) + MapState.MAPSIZE) % MapState.MAPSIZE;
		    py = ((py%MapState.MAPSIZE) + MapState.MAPSIZE) % MapState.MAPSIZE;
		    points.Add(new SCCoords(px, py));

		    // right edge
		    px = cp.X+w;
		    py = cp.Y-1+y;
		    px = ((px%MapState.MAPSIZE) + MapState.MAPSIZE) % MapState.MAPSIZE;
		    py = ((py%MapState.MAPSIZE) + MapState.MAPSIZE) % MapState.MAPSIZE;
		    points.Add(new SCCoords(px, py));

		    // bottom edge
		    px = cp.X-1+x;
		    py = cp.Y+h;
		    px = ((px%MapState.MAPSIZE) + MapState.MAPSIZE) % MapState.MAPSIZE;
		    py = ((py%MapState.MAPSIZE) + MapState.MAPSIZE) % MapState.MAPSIZE;
		    points.Add(new SCCoords(px, py));

		    // left edge
		    px = cp.X-1;
		    py = cp.Y-1+y;
		    px = ((px%MapState.MAPSIZE) + MapState.MAPSIZE) % MapState.MAPSIZE;
		    py = ((py%MapState.MAPSIZE) + MapState.MAPSIZE) % MapState.MAPSIZE;
		    points.Add(new SCCoords(px, py));
		}
	    }
	    return new List<SCCoords>(points);
	}

	public bool CheckClear(int x, int y, int w, int h, byte clearTile) {
	    bool clear = true;
	    for (int i = x; i < x + w && clear; ++ i)
	    {
		for (int j = y; j < y + h && clear; ++j)
		{
		    if (this.Tilemap[j, i] != clearTile) {
			clear = false;
			break;
		    }
		}
	    }
	    return clear;
	}

	public List<SCCoords> Candidates(List<byte> candidateTile) {
	    List<SCCoords> pts = new();
	    for (int i = 0; i < MAPSIZE; i++) {
		for (int j = 0; j < MAPSIZE; j++) {
		    if (candidateTile.Contains(this.Tilemap[j, i])) {
			pts.Add(new SCCoords(i, j));
		    }
		}
	    }
	    return pts;
	}

	public List<SCCoords> AddBox(List<SCCoords> candidates, Direction direction, (int n, int x) major, (int n, int x) minor,
				      byte emptyTile, byte roomTile, bool endOnly) {
	    if (candidates == null || candidates.Count == 0) {
		return null;
	    }
	    candidates.Shuffle(this.rng);

	    var majN = this.rng.Between(major.n, major.x);
	    var minN = this.rng.Between(minor.n, minor.x);

	    foreach (var c in candidates) {
		int x = 0, y = 0, w = 0, h = 0;
		switch (direction) {
		    case Direction.Up:
			w = minN;
			h = -majN;
			x = c.X-(w/2);
			y = c.Y;
			break;
		    case Direction.Right:
			w = majN;
			h = minN;
			x = c.X+1;
			y = c.Y-(h/2);
			break;
		    case Direction.Down:
			w = minN;
			h = majN;
			x = c.X-(w/2);
			y = c.Y+1;
			break;
		    case Direction.Left:
			w = -majN;
			h = minN;
			x = c.X;
			y = c.Y-(h/2);
			break;
		}

		if (h < 0) { y += h; h = -h; }
		if (w < 0) { x += w; w = -w; }

		if (x < 1 || y < 1 || (x+w)>(MAPSIZE-1) || (y+h)>(MAPSIZE-1)) {
		    continue;
		}

		if (!this.CheckClear(x, y, w, h, emptyTile)) {
		    continue;
		}

		this.OwnTilemap();
		this.Tilemap.Fill((x,y), (w, h), roomTile);

		var pts = InnerEdge(new SCCoords(x, y), w, h);

		if (!endOnly) {
		    return pts;
		}

		var d1 = direction;
		switch (direction) {
		    case Direction.Up:
			if (this.Tilemap[y-1, x] != emptyTile) {
			    direction = Direction.Down;
			}
			break;
		    case Direction.Right:
			if (this.Tilemap[y, x+w] != emptyTile) {
			    direction = Direction.Left;
			}
			break;
		    case Direction.Down:
			if (this.Tilemap[y+h, x] != emptyTile) {
			    direction = Direction.Up;
			}
			break;
		    case Direction.Left:
			if (this.Tilemap[y, x-1] != emptyTile) { direction = Direction.Right; }
			break;
		}

		if (d1 != direction) {
		    //Console.WriteLine($"d2 {d1} {direction} {x} {y} {w} {h}");
		}

		List<SCCoords> end = null;
		int endcap = 2;
		switch (direction) {
		    case Direction.Up:
			end = pts.Where((p) => p.Y < y+endcap).ToList();
			break;
		    case Direction.Right:
			end = pts.Where((p) => p.X > x+(w-endcap-1)).ToList();
			break;
		    case Direction.Down:
			end = pts.Where((p) => p.Y > y+(h-endcap-1)).ToList();
			break;
		    case Direction.Left:
			end = pts.Where((p) => p.X < x+endcap).ToList();
			break;
		}
		return end;
	    }
	    return null;
	}

	public async Task<MapResult> EarthB1Style() {
	    var start = this.AddBox(new List<SCCoords> { new SCCoords(this.Entrance.X, this.Entrance.Y-3) }, Direction.Down, (5, 5), (5, 5),
				     DungeonTiles.CAVE_BLANK, DungeonTiles.CAVE_FLOOR, false);

	    var ls = new List<List<SCCoords>>();
	    ls.Add(start);
	    ls.Add(start);
	    ls.Add(start);
	    ls.Add(start);
	    ls.Add(start);
	    //ls.Add(start);

	    var ld = new List<Direction>();
	    ld.Add(Direction.Up);
	    ld.Add(Direction.Right);
	    ld.Add(Direction.Down);
	    ld.Add(Direction.Left);
	    ld.Add((Direction)this.rng.Between(0, 3));
	    //ld.Add((Direction)this.rng.Between(0, 3));

	    for (int i = 0; i < 9; i++) {
		for (int j = 0; j < ld.Count; j++) {
		    var ret = this.AddBox(ls[j], ld[j], (4, 12), (1, 2),
					   DungeonTiles.CAVE_BLANK, DungeonTiles.CAVE_FLOOR, true);
		    if (ret != null) { ls[j] = ret; }

		    Direction dir;
		    do {
			dir = (Direction)this.rng.Between(0, 3);
		    } while (dir == ld[j]);
		    ld[j] = dir;
		}
	    }

	    int treasureRooms = 0;
	    for (int i = 0; i < 12; i++) {
		for (int j = 0; j < ld.Count; j++) {
		    var ret = this.AddBox(ls[j], ld[j], (8, 12), (8, 12),
					   DungeonTiles.CAVE_BLANK, DungeonTiles.CAVE_FLOOR, false);
		    if (ret != null) {
			ls[j].Clear();
			treasureRooms++;
		    }

		    Direction dir;
		    do {
			dir = (Direction)this.rng.Between(0, 3);
		    } while (dir == ld[j]);
		    ld[j] = dir;
		}
	    }
	    //Console.WriteLine($"{treasureRooms}");

	    if (treasureRooms < 2) {
		return new MapResult(false);
	    }

	    this.roomCount = treasureRooms;

	    return await this.NextStep();
	}

	public async Task<MapResult> PlaceTreasureRooms() {
	    var tasks = new List<MapGenerationTask>();
	    tasks.Add(() => new MapState(this).PlaceTreasureRoom(this.roomCount));
	    await Task.Yield();
	    return new MapResult(tasks);
	}

	public async Task<MapResult> PlaceTreasureRoom(int numRooms) {
	    var cand = this.Candidates(new List<byte> {DungeonTiles.CAVE_FLOOR});

	    for (int j = 0; j < numRooms; j++) {
		List<SCCoords> border = null;
		for (int i = 0; i < 6 && border == null; i++) {
		    border = AddBox(cand, Direction.Down, (8, 12), (8, 12), DungeonTiles.CAVE_FLOOR, DungeonTiles.CAVE_ROOM_FLOOR, false);
		}

		if (border == null) {
		    return new MapResult(false);
		}

		int xmin = 65, xmax = -1, ymax = -1;
		foreach (var b in border) {
		    if (b.X < xmin) { xmin = b.X; }
		    if (b.X > xmax) { xmax = b.X; }
		    if (b.Y > ymax) { ymax = b.Y; }
		    this.Tilemap[b.Y, b.X] = DungeonTiles.CAVE_FLOOR;
		}

		var doorx = this.rng.Between(xmin+2, xmax-2);
		this.Tilemap[ymax-1, doorx] = DungeonTiles.CAVE_DOOR;
		this.Tilemap[ymax, doorx] = DungeonTiles.CAVE_CLOSE_DOOR;
	    }

	    return await this.NextStep();
	}

	public async Task<MapResult> PlaceExitStairs(byte tile) {
	    var c = this.dt.cave_corners.Candidates(this.Tilemap.MapBytes);
	    var doors = this.Candidates(new List<byte> {DungeonTiles.CAVE_DOOR});

	    var e = c.Where((d) => d.Dist(this.Entrance) > 16);

	    foreach (var f in doors) {
		e = e.Where((d) => d.Dist(f) > 12);
	    }

	    var g = e.ToList();

	    if (g.Count == 0) {
		return new MapResult(false);
	    }

	    var p = g.PickRandom(this.rng);

	    this.OwnTilemap();
	    this.Tilemap[p.Y, p.X] = tile;

	    return await this.NextStep();
	}


	public async Task<MapResult> PlaceHallOfGiants(List<byte> traptiles) {
	    bool debug = false;

	    var c = this.dt.cave_corners.Candidates(this.Tilemap.MapBytes);
	    var desttiles = new List<byte> {DungeonTiles.CAVE_DOOR};
	    desttiles.AddRange(this.Stairs);
	    var destpoints = this.Candidates(desttiles);

	    List<SCCoords> pathpoints = new();
	    foreach (var p in destpoints) {
		pathpoints.AddRange(this.ShortestPath(this.Entrance, p));
	    }

	    c.Shuffle(this.rng);

	    this.OwnTilemap();
	    /*foreach (var p in pathpoints) {
		this.Tilemap[p.Y, p.X] = 0x12;
		}*/

	    foreach (var corner in c) {
		bool valid = true;
		foreach (var p in pathpoints) {
		    if (corner.Dist(p) < 5) {
			valid = false;
			break;
		    }
		}
		if (!valid) continue;

		this.OwnTilemap();

		var working_list = new Queue<SCCoords>();
		HashSet<SCCoords> visited = new ();

		working_list.Enqueue(corner);

		int count = 0;
		while (count < 50 && working_list.Count > 0) {
		    var pt = working_list.Dequeue();
		    if (this.Tilemap[pt.Y, pt.X] != DungeonTiles.CAVE_FLOOR) {
			continue;
		    }

		    bool b = true;
		    foreach (var p in pathpoints) {
			if (pt.Dist(p) < 3) {
			    b = false;
			    break;
			}
		    }
		    if (!b) continue;

		    this.Tilemap[pt.Y, pt.X] = traptiles.PickRandom(this.rng);
		    if (debug) this.Tilemap[pt.Y, pt.X] = 0x13;

		    count++;

		    working_list.Enqueue(pt.SmUp);
		    working_list.Enqueue(pt.SmRight);
		    working_list.Enqueue(pt.SmDown);
		    working_list.Enqueue(pt.SmLeft);
		}

		break;
	    }
	    return await this.NextStep();
	}

	public async Task<MapResult> PlaceChests() {
	    this.OwnTilemap();

	    var cand = this.Candidates(new List<byte> {DungeonTiles.CAVE_ROOM_FLOOR});
	    var doors = this.Candidates(new List<byte> {DungeonTiles.CAVE_DOOR});

	    var traps = new List<byte>(this.Traps);

	    List<MapElement> chests = new();

	    foreach (var c in this.Chests) {
		var p = cand.SpliceRandom(this.rng);
		var me = this.Tilemap[(p.X, p.Y)];
		me.Value = c;
		chests.Add(me);
	    }

	    List<FF1Rom.Room> rooms = new();
	    foreach (var d in doors) {
		FF1Rom.Room r = new();
		r.start = this.Tilemap[(d.X, d.Y)];
	    }

	    FF1Rom.PlaceSpikeTiles(this.rng, traps, chests,
				   this.RoomFloorTiles,
				   this.RoomBattleTiles,
				   rooms);

	    return await this.NextStep();
	}

	public async Task<MapResult> PlaceTile(int x, int y, byte tile) {
	    this.OwnTilemap();
	    this.Tilemap[y, x] = tile;
	    return await this.NextStep();
	}

	public async Task<MapResult> CollectInfo() {
	    this.OwnFeatures();

	    this.Traps = new();
	    this.RoomFloorTiles = new();
	    this.RoomBattleTiles = new();
	    byte randomEncounter = 0x80;

	    FF1Rom.FindRoomTiles(this.tileSet,
				 this.RoomFloorTiles,
				 this.Traps,
				 this.RoomBattleTiles,
				 randomEncounter);

	    for (int y = 0; y < MAPSIZE; y++) {
		for (int x = 0; x < MAPSIZE; x++) {
		    byte t = this.Tilemap[y, x];

		    if (this.tileSet.TileProperties[t].TilePropFunc == (TilePropFunc.TP_SPEC_TREASURE | TilePropFunc.TP_NOMOVE)) {
			this.Chests.Add(t);
		    }
		    if ((this.tileSet.TileProperties[t].TilePropFunc & TilePropFunc.TP_TELE_MASK) != 0) {
			this.Stairs.Add(t);
		    }
		    /*if (this.tileSet.TileProperties[t].TilePropFunc == TilePropFunc.TP_SPEC_BATTLE) {
			if (this.tileSet.TileProperties[t].BattleId != randomEncounter) {
			    this.Traps.Add(t);
			}
			}*/
		}
	    }

	    return await this.NextStep();
	}

	public async Task<MapResult> SetEntrance(int x, int y) {
	    this.Entrance = new SCCoords(x, y);
	    return await this.NextStep();
	}

	public async Task<MapResult> SanityCheck() {
	    this.OwnFeatures();

	    foreach (var c in this.Chests) {
		Console.WriteLine($"chest {c:X}");
	    }

	    this.Tilemap.Flood((this.Entrance.X, this.Entrance.Y), (MapElement me) => {
		if (this.Chests.Contains(me.Value)) {
		    Console.WriteLine($"removing {me.Value:X}");
		    this.Chests.Remove(me.Value);
		}
		if (this.Stairs.Contains(me.Value)) {
		    this.Stairs.Remove(me.Value);
		}

		if ((this.tileSet.TileProperties[me.Value].TilePropFunc & TilePropFunc.TP_SPEC_DOOR) == TilePropFunc.TP_SPEC_DOOR) {
		    return true;
		}

		if ((this.tileSet.TileProperties[me.Value].TilePropFunc & TilePropFunc.TP_SPEC_CLOSEROOM) == TilePropFunc.TP_SPEC_CLOSEROOM) {
		    return true;
		}

		if ((this.tileSet.TileProperties[me.Value].TilePropFunc & TilePropFunc.TP_NOMOVE) == TilePropFunc.TP_NOMOVE) {
		    return false;
		}
		return true;
	    });

	    Console.WriteLine($"count {this.Chests.Count} {this.Stairs.Count}");

	    if (this.Chests.Count > 0 || this.Stairs.Count > 0) {
		return new MapResult(false);
	    }

	    return await this.NextStep();
	}


	public List<SCCoords> ShortestPath(SCCoords start, SCCoords end) {
	    PriorityQueue<List<SCCoords>, double> working = new();
	    HashSet<SCCoords> visited = new ();

	    working.Enqueue(new List<SCCoords> { start }, start.Dist(end));

	    try {
		while (true) {
		    var path = working.Dequeue();
		    var pos = path[path.Count-1];
		    if (pos == end) {
			return path;
		    }

		    foreach (var nextPos in new SCCoords[] { pos.SmUp, pos.SmRight, pos.SmDown, pos.SmLeft }) {
			if (visited.Contains(nextPos)) {
			    continue;
			}
			var t = this.Tilemap[nextPos.Y, nextPos.X];
			if ((this.tileSet.TileProperties[t].TilePropFunc & TilePropFunc.TP_NOMOVE) == 0 ||
			    (this.tileSet.TileProperties[t].TilePropFunc & TilePropFunc.TP_SPEC_DOOR) == TilePropFunc.TP_SPEC_DOOR ||
			    (this.tileSet.TileProperties[t].TilePropFunc & TilePropFunc.TP_SPEC_CLOSEROOM) == TilePropFunc.TP_SPEC_CLOSEROOM)
			{
			    var nextPath = new List<SCCoords>(path);
			    nextPath.Add(nextPos);
			    working.Enqueue(nextPath, nextPos.Dist(end));
			    visited.Add(nextPos);
			}
		    }
		}
	    } catch (InvalidOperationException) { }

	    return null;
	}
    }
}
