using FF1Lib.Sanity;
using MapGenerationTask = FF1Lib.Procgen.ProgenFramework.GenerationTaskType<FF1Lib.Procgen.MapResult>;
using Direction = FF1Lib.Direction;

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

	public static List<SCCoords> BoxPoints(SCCoords cp, int w, int h) {
	    List<SCCoords> coords = new List<SCCoords>();
	    for (int i = cp.X; i < cp.X + w; ++ i) {
		for (int j = cp.Y; j < cp.Y + h; ++j) {
		    coords.Add(new SCCoords(i, j));
		}
	    }
	    return coords;
	}

	public bool CheckClear(int x, int y, int w, int h, byte clearTile, List<SCCoords> coverable) {
	    bool clear = true;
	    for (int i = x; i < x + w && clear; ++ i) {
		for (int j = y; j < y + h && clear; ++j) {
		    if (coverable != null && coverable.Contains(new SCCoords(i, j))) {
			continue;
		    }
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

	SCCoords Adj(SCCoords p, Direction d) {
	    switch (d) {
		case Direction.Up:
		    return p.SmUp;
		case Direction.Right:
		    return p.SmRight;
		case Direction.Down:
		    return p.SmDown;
		case Direction.Left:
		    return p.SmLeft;
		default:
		    return p;
	    }
	}

	public bool MakeBox(SCCoords c, Direction direction, (int n, int x) major, (int n, int x) minor,
				      out int x, out int y, out int w, out int h)
	{
	    var majN = this.rng.Between(major.n, major.x);
	    var minN = this.rng.Between(minor.n, minor.x);

	    x = 0;
	    y = 0;
	    w = 0;
	    h = 0;
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
		return false;
	    }
	    return true;
	}

	/*public bool CornerBox(SCCoords c, (int n, int x) major, (int n, int x) minor,
			      out int x, out int y, out int w, out int h)
	{
	    var majN = this.rng.Between(major.n, major.x);
	    var minN = this.rng.Between(minor.n, minor.x);

	}*/

	public List<SCCoords> EndCap(List<SCCoords> pts, Direction direction, SCCoords topLeft, int w, int h) {
	    int endcap = 2;
	    switch (direction) {
		case Direction.Up:
		    return pts.Where((p) => p.Y < topLeft.Y+endcap).ToList();
		case Direction.Right:
		    return pts.Where((p) => p.X > topLeft.X+(w-endcap-1)).ToList();
		case Direction.Down:
		    return pts.Where((p) => p.Y > topLeft.Y+(h-endcap-1)).ToList();
		case Direction.Left:
		    return pts.Where((p) => p.X < topLeft.X+endcap).ToList();
		default:
		    return null;
	    }
	}

	public enum Quadrants : int {
	    UpRight = 0,
	    DownRight = 1,
	    DownLeft = 2,
	    UpLeft = 3
	}

	public List<SCCoords> Quadrant(List<SCCoords> pts, Quadrants q, SCCoords topLeft, int w, int h) {
	    int midx = topLeft.X + (w/2);
	    int midy = topLeft.Y + (h/2);

	    switch (q) {
		case Quadrants.UpRight:
		    return pts.Where((p) => p.X >= midx && p.Y < midy).ToList();
		case Quadrants.DownRight:
		    return pts.Where((p) => p.X >= midx && p.Y >= midy).ToList();
		case Quadrants.DownLeft:
		    return pts.Where((p) => p.X < midx && p.Y >= midy).ToList();
		case Quadrants.UpLeft:
		    return pts.Where((p) => p.X >= midx && p.Y < midy).ToList();
		default:
		    return null;
	    }
	}

	public bool CornerBox(List<SCCoords> candidates, Quadrants q, (int n, int x) wrange, (int n, int x) hrange,
					List<SCCoords> coverable, byte emptyTile, out SCCoords topLeft, out int w, out int h) {
	    candidates.Shuffle(this.rng);

	    w = this.rng.Between(wrange.n, wrange.x);
	    h = this.rng.Between(hrange.n, hrange.x);

	    topLeft = new SCCoords(0, 0);

	    foreach (var c in candidates) {
		switch (q) {
		case Quadrants.UpRight:
		    if (h > c.Y) { continue; }
		    topLeft = new SCCoords(c.X, c.Y-h);
		    break;
		case Quadrants.DownRight:
		    topLeft = new SCCoords(c.X, c.Y);
		    break;
		case Quadrants.DownLeft:
		    if (w > c.X) { continue; }
		    topLeft = new SCCoords(c.X-w, c.Y);
		    break;
		case Quadrants.UpLeft:
		    if (w > c.X || h > c.Y) { continue; }
		    topLeft = new SCCoords(c.X-w, c.Y-h);
		    break;
		default:
		    continue;
		}

		if (topLeft.X == 0 || topLeft.Y == 0 || (topLeft.X + w) >= MAPSIZE || (topLeft.Y + h) >= MAPSIZE) continue;

		if (!this.CheckClear(topLeft.X, topLeft.Y, w, h, emptyTile, coverable)) {
		    continue;
		}

		return true;
	    }
	    return false;
	}

	public List<SCCoords> AddBox(List<SCCoords> candidates, Direction direction, (int n, int x) major, (int n, int x) minor,
				     byte emptyTile, byte roomTile, bool endOnly, out SCCoords topLeft, out int _w, out int _h) {
	    topLeft = new SCCoords(0, 0);
	    _w = 0;
	    _h = 0;
	    if (candidates == null || candidates.Count == 0) {
		return null;
	    }
	    candidates.Shuffle(this.rng);

	    var majN = this.rng.Between(major.n, major.x);
	    var minN = this.rng.Between(minor.n, minor.x);

	    foreach (var c in candidates) {
		int x, y, w, h;
		if (!this.MakeBox(c, direction, (majN, majN), (minN, minN), out x, out y, out w, out h)) {
		    continue;
		}

		if (emptyTile != 0xff && !this.CheckClear(x, y, w, h, emptyTile, null)) {
		    continue;
		}

		this.OwnTilemap();
		this.Tilemap.Fill((x,y), (w, h), roomTile);

		topLeft = new SCCoords(x, y);
		_w = w;
		_h = h;
		var pts = InnerEdge(topLeft, w, h);

		if (!endOnly) {
		    return pts;
		}

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

		return this.EndCap(pts, direction, topLeft, w, h);
	    }
	    return null;
	}

	public async Task<MapResult> EarthB1Style() {
	    SCCoords _topLeft;
	    int _w, _h;
	    var start = this.AddBox(new List<SCCoords> { new SCCoords(this.Entrance.X, this.Entrance.Y-3) }, Direction.Down, (5, 5), (5, 5),
				    DungeonTiles.CAVE_BLANK, DungeonTiles.CAVE_FLOOR, false, out _topLeft, out _w, out _h);

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
					  DungeonTiles.CAVE_BLANK, DungeonTiles.CAVE_FLOOR, true,
					  out _topLeft, out _w, out _h);
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
					  DungeonTiles.CAVE_BLANK, DungeonTiles.CAVE_FLOOR, false,
					  out _topLeft, out _w, out _h);
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

	class EarthB2Room {
	    public List<SCCoords> points;
	    public SCCoords topLeft;
	    public int w, h;
	}

	public void ConnectRegions() {

	    var regions = new byte[MAPSIZE,MAPSIZE];

	    var regionList = new List<SCCoords>();

	    regionList.Add(new SCCoords(0, 0));

	    for (int y = 0; y < MAPSIZE; y++) {
		for (int x = 0; x < MAPSIZE; x++) {
		    if (regions[y,x] > 0) continue;
		    if (this.Tilemap[y,x] != DungeonTiles.CAVE_FLOOR) continue;

		    regionList.Add(new SCCoords(x, y));
		    int currentRegion = regionList.Count-1;
		    this.Tilemap.Flood((x, y), (MapElement me) => {
			if (me.Value == DungeonTiles.CAVE_FLOOR) {
			    regions[me.Y, me.X] = (byte)currentRegion;
			    return true;
			}
			return false;
		    });
		}
	    }

	    var dirs = new Direction[] { Direction.Up, Direction.Right, Direction.Down, Direction.Left };

	    foreach (var r in regionList) {
		var thisRegion = regions[r.Y, r.X];
		if (thisRegion == 0) continue;

		Dictionary<int, List<SCCoords>> hops = new();

		this.Tilemap.Flood((r.X, r.Y), (MapElement me) => {
		    if (regions[me.Y, me.X] == thisRegion) {
			foreach (var d in dirs) {
			    var n = me.Neighbor(d);
			    if (n.Value == DungeonTiles.CAVE_BLANK) {
				var nn = me.Neighbor(d).Neighbor(d);
				var neighborReg = regions[nn.Y, nn.X];
				if (neighborReg > thisRegion) {
				    if (!hops.ContainsKey(neighborReg)) {
					hops[neighborReg] = new List<SCCoords>();
				    }
				    hops[neighborReg].Add(new SCCoords(n.X, n.Y));
				}
			    }
			}
			return true;
		    }
		    return false;
		});

		Console.WriteLine($"{hops.Keys}");
		foreach (var h in hops.Keys.ToList()) {
		    var p = hops[h].PickRandom(this.rng);
		    this.Tilemap[p.Y, p.X] = DungeonTiles.CAVE_FLOOR;
		}
	    }
	}

	public async Task<MapResult> EarthB2Style() {
	    this.OwnTilemap();

	    var startroom = new EarthB2Room();

	    this.AddBox(new List<SCCoords> { new SCCoords(this.Entrance.X, this.Entrance.Y-3) }, Direction.Down, (5, 5), (5, 5),
			DungeonTiles.CAVE_BLANK, DungeonTiles.CAVE_FLOOR, false, out startroom.topLeft, out startroom.w, out startroom.h);

	    startroom.points = BoxPoints(startroom.topLeft, startroom.w, startroom.h);

	    var rooms = new List<EarthB2Room> { startroom };

	    int roomTotal = this.rng.Between(35, 40);
	    int i;
	    for (i = 0; rooms.Count < roomTotal && i < 200; i++) {
		int roomIndex = this.rng.Between(Math.Max(rooms.Count-6, 0), rooms.Count-1);
		//int roomIndex = this.rng.Between(0, rooms.Count-1);
		var room = rooms[roomIndex];
		var quad = (Quadrants)this.rng.Between(0, 3);

		var candidates = Quadrant(room.points, quad, room.topLeft, room.w, room.h);

		//Console.WriteLine($"{candidates.Count} {quad} {room.topLeft} {room.w} {room.h}");

		SCCoords topLeft;
		int w, h;

		bool valid;

		valid = CornerBox(candidates, quad, (4, 6), (4, 6), candidates, DungeonTiles.CAVE_BLANK, out topLeft, out w, out h);

		if (!valid) {
		    continue;
		}

		var boxpoints = BoxPoints(topLeft, w, h);
		var oldRoomBoxpoints = BoxPoints(room.topLeft, room.w, room.h);
		var clear = OuterEdge(room.topLeft, room.w, room.h);
		var newRoomEdge = OuterEdge(topLeft, w, h);

		foreach (var c in newRoomEdge) {
		    if (this.Tilemap[c.Y, c.X] != DungeonTiles.CAVE_BLANK && !oldRoomBoxpoints.Contains(c)) {
			valid = false;
			break;
		    }
		}

		if (!valid) {
		    continue;
		}

		this.Tilemap.Fill((topLeft.X, topLeft.Y), (w, h), DungeonTiles.CAVE_FLOOR);

		foreach (var c in clear) {
		    if (boxpoints.Contains(c)) {
			this.Tilemap[c.Y, c.X] = DungeonTiles.CAVE_BLANK;
			boxpoints.Remove(c);
		    }
		}

		rooms.Add(new EarthB2Room { points = boxpoints, topLeft = topLeft, w = w, h = h });
	    }

	    Console.WriteLine($"{rooms.Count} {i}");

	    this.ConnectRegions();

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

	    SCCoords _topLeft;
	    int _w, _h;

	    for (int j = 0; j < numRooms; j++) {
		List<SCCoords> border = null;
		for (int i = 0; i < 6 && border == null; i++) {
		    border = AddBox(cand, Direction.Down, (8, 12), (8, 12), DungeonTiles.CAVE_FLOOR, DungeonTiles.CAVE_ROOM_FLOOR, false,
		    out _topLeft, out _w, out _h);
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


	public List<SCCoords> ShortestPath(SCCoords start, SCCoords end, List<byte> nonWalkable=null) {
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
			    //Console.WriteLine($"visited already {nextPos}");
			    continue;
			}
			visited.Add(nextPos);
			var t = this.Tilemap[nextPos.Y, nextPos.X];
			if (nonWalkable != null && nonWalkable.Contains(t)) { continue; }
			if ((this.tileSet.TileProperties[t].TilePropFunc & TilePropFunc.TP_NOMOVE) == 0 ||
			    (this.tileSet.TileProperties[t].TilePropFunc & TilePropFunc.TP_SPEC_DOOR) == TilePropFunc.TP_SPEC_DOOR ||
			    (this.tileSet.TileProperties[t].TilePropFunc & TilePropFunc.TP_SPEC_CLOSEROOM) == TilePropFunc.TP_SPEC_CLOSEROOM)
			{
			    var nextPath = new List<SCCoords>(path);
			    nextPath.Add(nextPos);
			    working.Enqueue(nextPath, nextPos.Dist(end));
			    //Console.WriteLine($"enqueuing on {nextPos}");
			}
		    }
		}
	    } catch (InvalidOperationException) { }

	    return null;
	}
    }
}
