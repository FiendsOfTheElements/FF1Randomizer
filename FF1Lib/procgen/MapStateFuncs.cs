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

	public static List<SCCoords> OuterEdge(SCCoords cp, int w, int h, bool corners) {
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
	    if (!corners) {
		points.Remove(new SCCoords(cp.X-1, cp.Y-1));
		points.Remove(new SCCoords(cp.X+w, cp.Y-1));
		points.Remove(new SCCoords(cp.X-1, cp.Y+h));
		points.Remove(new SCCoords(cp.X+w, cp.Y+h));
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

	public bool CheckClear(int x, int y, int w, int h, byte clearTile, List<SCCoords> coverable, int zone) {

	    if (x < zone || y < zone || (x + w) >= (MAPSIZE-zone) || (y + h) >= (MAPSIZE-zone)) return false;

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

	    int zone = 4;
	    if (x < zone || y < zone || (x + w) >= (MAPSIZE-zone) || (y + h) >= (MAPSIZE-zone)) {
		return false;
	    }

	    return true;
	}

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
		    return pts.Where((p) => p.X >= midx && p.Y <= midy).ToList();
		case Quadrants.DownRight:
		    return pts.Where((p) => p.X >= midx && p.Y >= midy).ToList();
		case Quadrants.DownLeft:
		    return pts.Where((p) => p.X <= midx && p.Y >= midy).ToList();
		case Quadrants.UpLeft:
		    return pts.Where((p) => p.X <= midx && p.Y <= midy).ToList();
		default:
		    return null;
	    }
	}

	public bool CornerBox(List<SCCoords> candidates, Quadrants q, (int n, int x) wrange, (int n, int x) hrange,
			      List<SCCoords> coverable, byte emptyTile, int minArea, out SCCoords topLeft, out int w, out int h) {
	    candidates.Shuffle(this.rng);

	    do {
		w = this.rng.Between(wrange.n, wrange.x);
		h = this.rng.Between(hrange.n, hrange.x);
	    } while ((w*h) < minArea);

	    topLeft = new SCCoords(0, 0);

	    while (w >= wrange.n && h >= hrange.n && (w*h >= minArea)) {
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

		    if (!this.CheckClear(topLeft.X, topLeft.Y, w, h, emptyTile, coverable, 4)) {
			//Console.WriteLine($"failed {topLeft} {w} {h}");
			continue;
		    }

		    return true;
		}

		int shrink = this.rng.Between(0, 1);
		if (shrink == 0 && w > wrange.n) {
		    w--;
		} else if (h > hrange.n) {
		    h--;
		} else {
		    w--;
		}
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

		if (emptyTile != 0xff && !this.CheckClear(x, y, w, h, emptyTile, null, 0)) {
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

	public async Task<MapResult> EarthB1Style(int minTreasureRooms) {
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

	    if (treasureRooms < minTreasureRooms) {
		return new MapResult(false);
	    }

	    this.roomCount = treasureRooms;

	    return await this.NextStep();
	}

	public class EarthB2Room {
	    public List<SCCoords> points;
	    public SCCoords topLeft;
	    public int w, h;
	    public bool tight;
	    public bool hallway;
	    public int gating;
	}

	public async Task<MapResult> ConnectRegions() {

	    this.OwnTilemap();

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
				foreach (var i in dirs) {
				    var nn = me.Neighbor(d).Neighbor(i);
				    var neighborReg = regions[nn.Y, nn.X];
				    if (neighborReg > thisRegion) {
					if (!hops.ContainsKey(neighborReg)) {
					    hops[neighborReg] = new List<SCCoords>();
					}
					hops[neighborReg].Add(new SCCoords(n.X, n.Y));
				    }
				}
			    }
			}
			return true;
		    }
		    return false;
		});

		foreach (var h in hops.Keys.ToList()) {
		    var p = hops[h].PickRandom(this.rng);
		    this.Tilemap[p.Y, p.X] = DungeonTiles.CAVE_FLOOR;
		}
	    }

	    return await this.NextStep();
	}

	public async Task<MapResult> EarthB2Style(int startRoomLeft, int startRoomRight, int startRoomTop, int startRoomBottom,
	    int minRooms, int maxRooms, int progressWindow, int connectWindow, int minTreasureRooms, PgFeature[] features, int featureAfter) {
	    this.OwnTilemap();

	    var startroom = new EarthB2Room();

	    this.AddBox(new List<SCCoords> { new SCCoords(this.Entrance.X-startRoomLeft, this.Entrance.Y-startRoomTop) }, Direction.Right,
			(startRoomLeft+startRoomRight+1, startRoomLeft+startRoomRight+1),
			(startRoomTop+startRoomBottom+1, startRoomTop+startRoomBottom+1),
			DungeonTiles.CAVE_BLANK, DungeonTiles.CAVE_FLOOR, false, out startroom.topLeft, out startroom.w, out startroom.h);

	    startroom.points = BoxPoints(startroom.topLeft, startroom.w, startroom.h);
	    startroom.gating = 0;

	    var rooms = new List<EarthB2Room> { startroom };
	    var regions = new byte[MAPSIZE,MAPSIZE];
	    int roomTotal = this.rng.Between(minRooms, maxRooms);

	    var tasks = new List<MapGenerationTask>();

	    tasks.Add(() => new MapState(this).EarthB2NextRoom(progressWindow, connectWindow, minTreasureRooms, features, featureAfter,
							       rooms, regions, roomTotal, 0, 0, false, 0));

	    await Task.Yield();
	    return new MapResult(tasks);
	}

	public async Task<MapResult> EarthB2NextRoom(int progressWindow, int connectWindow, int minTreasureRooms, PgFeature[] features, int featureAfter,
						     List<EarthB2Room> rooms, byte[,] regions, int roomTotal, int treasureCount, int featureCount,
						     bool restart, int gating)
	{
	    if (rooms.Count == roomTotal) {
		if (treasureCount < minTreasureRooms || featureCount < features.Length) {
		    return new MapResult(false);
		}
		return await this.NextStep();
	    }

	    var tasks = new List<MapGenerationTask>();
	    var quads = new List<Quadrants> { Quadrants.UpRight, Quadrants.DownRight, Quadrants.DownLeft, Quadrants.UpLeft };

	    List<int> roomIdx = new();

	    if (gating == rooms.Count-1) {
		roomIdx.Add(gating);
	    } else if (restart) {
		for (int i = rooms.Count-1; i >= 0 ; i--) {
		    roomIdx.Add(i);
		}
	    } else if (rooms[rooms.Count-1].hallway) {
		roomIdx.Add(rooms.Count-1);
	    } else {
		for (int i = rooms.Count-1; i >= Math.Max(rooms.Count-progressWindow, 0); i--) {
		    roomIdx.Add(i);
		}
	    }

	    var fallbackTasks = new List<MapGenerationTask>();

	    foreach (var rm in roomIdx) {
		var room = rooms[rm];

		int hallwayDraw;
		if (room.tight) {
		    hallwayDraw = 1;
		} else {
		    hallwayDraw = this.rng.Between(0, 7);
		}

		int treasureDraw = this.rng.Between(0, Math.Max(0, rooms.Count-(treasureCount*minTreasureRooms)));
		int featureDraw = this.rng.Between(0, Math.Max(0, rooms.Count-(featureCount*features.Length + featureAfter)));

		quads.Shuffle(this.rng);
		foreach (var quad in quads) {
		    bool placeTreasure = false;
		    bool placeFeature = false;
		    bool featureCandidate = false;
		    if (quad == Quadrants.UpLeft || quad == Quadrants.UpRight || room.hallway) {
			placeTreasure = (minTreasureRooms > 0 && treasureDraw > minTreasureRooms);
			featureCandidate = featureCount < features.Length && rooms.Count > featureAfter && (rm != gating) && (room.gating == gating);
			placeFeature = featureCandidate && featureDraw > features.Length;
		    }

		    if (placeFeature) {
			tasks.Add(() => new MapState(this).EarthB2AddRoom(rm, quad, progressWindow, connectWindow, minTreasureRooms, features, featureAfter,
									  rooms, regions, roomTotal, treasureCount, featureCount, gating,
									  false, true, hallwayDraw));
		    }
		    if (placeTreasure) {
			// tried to place a feature, try treasure placement instead.
			tasks.Add(() => new MapState(this).EarthB2AddRoom(rm, quad, progressWindow, connectWindow, minTreasureRooms, features, featureAfter,
									  rooms, regions, roomTotal, treasureCount, featureCount, gating,
									  true, false, hallwayDraw));
		    }
		    // tried a treasure room placement, try a regular room
		    tasks.Add(() => new MapState(this).EarthB2AddRoom(rm, quad, progressWindow, connectWindow, minTreasureRooms, features, featureAfter,
								      rooms, regions, roomTotal, treasureCount, featureCount, gating,
								      false, false, hallwayDraw));

		    if (featureCandidate && !placeFeature) {
			// we didn't try to place a feature earlier but we want to, so add it to try later
			fallbackTasks.Add(() => new MapState(this).EarthB2AddRoom(rm, quad, progressWindow, connectWindow, minTreasureRooms, features, featureAfter,
									  rooms, regions, roomTotal, treasureCount, featureCount, gating,
									  false, true, hallwayDraw));
		    }
		}
	    }
	    //Console.WriteLine($"Adding {tasks.Count} tasks");

	    tasks.AddRange(fallbackTasks);

	    return new MapResult(tasks);
	}

	public async Task<MapResult> EarthB2AddRoom(int roomIndex, Quadrants quad, int progressWindow, int connectWindow, int minTreasureRooms, PgFeature[] features, int featureAfter,
						    List<EarthB2Room> rooms, byte[,] regions, int roomTotal, int treasureCount, int featureCount,
						    int gating, bool placeTreasure, bool placeFeature, int hallwayDraw)
	{
	    //Console.WriteLine($"{rooms.Count}/{roomTotal} adding to {roomIndex} {quad} treasure {placeTreasure} feature {placeFeature}");

	    var room = rooms[roomIndex];
	    var candidates = Quadrant(room.points, quad, room.topLeft, room.w, room.h);

	    //Console.WriteLine($"{candidates.Count} {quad} {room.topLeft} {room.w} {room.h}");

	    SCCoords topLeft = new SCCoords(0, 0);
	    int w = 0;
	    int h = 0;

	    bool valid = false;
	    bool restart = false;
	    bool makeHallway = (hallwayDraw == 0 || hallwayDraw == 1);

	    if (placeFeature) {
		var thisFeature = features[featureCount];
		int featureH = thisFeature.Tiles.GetLength(0);
		int featureW = thisFeature.Tiles.GetLength(1);

		List<SCCoords> checkpoints = new();
		for (int j = 0; j < room.w; j++) {
		    checkpoints.Add(new SCCoords(room.topLeft.X-featureW/2 + j, room.topLeft.Y-featureH));
		}
		checkpoints.Shuffle(this.rng);
		foreach (var c in checkpoints) {
		    if (CheckClear(c.X, c.Y, featureW, featureH, DungeonTiles.CAVE_BLANK, null, 4)) {
			//Console.WriteLine($"trying {c}");
			topLeft = c;
			w = featureW;
			h = featureH;
			valid = true;
			break;
		    }
		}
	    } else if (placeTreasure) {
		var minRoomArea = (this.Chests.Count * 6) / minTreasureRooms;
		valid = CornerBox(candidates, quad, (4, 9), (4, 9), candidates, DungeonTiles.CAVE_BLANK, minRoomArea, out topLeft, out w, out h);
	    } else if (hallwayDraw == 0) {
		// Vertical hallway
		valid = CornerBox(candidates, quad, (1, 1), (6, 8), candidates, DungeonTiles.CAVE_BLANK, 0, out topLeft, out w, out h);
	    } else if (hallwayDraw == 1) {
		// Horizontal hallway
		valid = CornerBox(candidates, quad, (6, 8), (1, 1), candidates, DungeonTiles.CAVE_BLANK, 0, out topLeft, out w, out h);
	    } else {
		valid = CornerBox(candidates, quad, (4, 7), (4, 7), candidates, DungeonTiles.CAVE_BLANK, 0, out topLeft, out w, out h);
	    }

	    //Console.WriteLine($"val {valid} {hallwayDraw} {topLeft} {w} {h}");

	    if (!valid) {
		return new MapResult(false);
	    }

	    var boxpoints = BoxPoints(topLeft, w, h);
	    var oldRoomBoxpoints = BoxPoints(room.topLeft, room.w, room.h);
	    var clear = OuterEdge(room.topLeft, room.w, room.h, true);
	    var newRoomEdge = OuterEdge(topLeft, w, h, true);

	    // Make sure it isn't touching anything
	    foreach (var c in newRoomEdge) {
		if (this.Tilemap[c.Y, c.X] != DungeonTiles.CAVE_BLANK &&
		    !oldRoomBoxpoints.Contains(c))
		{
		    valid = false;
		    break;
		}
	    }

	    if (!valid) {
		//Console.WriteLine($"hit edge 1");
		return new MapResult(false);
	    }

	    List<SCCoords> secondEdge = OuterEdge(topLeft.SmNeighbor(Quadrants.UpLeft), w+2, h+2, false);
	    List<int> connecting = new();
	    foreach (var c in secondEdge) {
		if (this.Tilemap[c.Y, c.X] != DungeonTiles.CAVE_BLANK &&
		    !oldRoomBoxpoints.Contains(c))
		{
		    // Pass if this is adjacent to a room that we placed sufficiently earlier & in the same gating
		    var reg = regions[c.Y, c.X];
		    if (reg > 0 && (reg+connectWindow) <= rooms.Count && (rooms[reg].gating == room.gating)) {
			connecting.Add(reg);
			restart = true;
		    } else {
			//Console.WriteLine($"rejected connecting {reg} {rooms.Count} {c} {this.Tilemap[c.Y, c.X]:X}");
			valid = false;
			break;
		    }
		}
	    }

	    if (!room.tight) {
		secondEdge = OuterEdge(topLeft.SmNeighbor(Quadrants.UpLeft).SmNeighbor(Quadrants.UpLeft), w+4, h+4, false);
		foreach (var c in secondEdge) {
		    if (this.Tilemap[c.Y, c.X] != DungeonTiles.CAVE_BLANK &&
			!oldRoomBoxpoints.Contains(c) &&
			!connecting.Contains(regions[c.Y, c.X]))
		    {
			//Console.WriteLine($"second edge {regions[c.Y, c.X]} {rooms.Count} {oldRoomBoxpoints.Contains(c)} {connecting.Contains(regions[c.Y, c.X])}");
			valid = false;
		    }
		}
	    }

	    if (!valid) {
		return new MapResult(false);
	    }

	    this.OwnTilemap();

	    var fromRegions = regions;
	    regions = new byte[MAPSIZE,MAPSIZE];
	    Array.Copy(fromRegions, regions, regions.Length);

	    if (placeFeature) {
		this.RenderFeature(topLeft, features[featureCount].Tiles);
		if (features[featureCount].Entrances.ContainsKey("ExitDoor")) {
		    var pts = new List<SCCoords>();
		    var exit = features[featureCount].Entrances["ExitDoor"];
		    var pt = new SCCoords(topLeft.X + exit.X, topLeft.Y + exit.Y + 1);
		    pts.Add(pt);
		    //Console.WriteLine($"------------------ {pt} {rooms.Count}");

		    gating = rooms.Count;
		    foreach (var b in boxpoints) {
			regions[b.Y, b.X] = (byte)rooms.Count;
		    }
		    rooms = new(rooms);
		    rooms.Add(new EarthB2Room { points = pts, topLeft = topLeft, w = w, h = h, tight = true, gating = gating });
		}

		var npcPlacements = new Dictionary<string, ObjectId> {
		    {"Vampire", ObjectId.Vampire},
		    {"RodPlate", ObjectId.RodPlate},
		    {"LichOrb", ObjectId.LichOrb},
		};

		foreach (var f in npcPlacements) {
		    if (features[featureCount].Entrances.ContainsKey(f.Key)) {
			this.OwnFeatures();
			var coord = features[featureCount].Entrances[f.Key];
			var npc = this.NPCs.Where(n => n.ObjectId == f.Value).First();
			npc.Coord = (topLeft.X + coord.X, topLeft.Y + coord.Y);
			this.NPCs[npc.Index] = npc;
		    }
		}

		featureCount++;
	    } else if (placeTreasure) {
		List<SCCoords> doorCandidates = new();
		for (int x = topLeft.X+1; x < topLeft.X+w-1; x++) {
		    if (this.Tilemap[topLeft.Y+h, x] == DungeonTiles.CAVE_FLOOR) {
			doorCandidates.Add(new SCCoords(x, topLeft.Y+h-1));
		    }
		}

		if (doorCandidates.Count == 0) {
		    return new MapResult(false);
		}

		treasureCount++;
		foreach (var b in boxpoints) {
		    regions[b.Y, b.X] = (byte)rooms.Count;
		    this.Tilemap[b.Y, b.X] = DungeonTiles.CAVE_ROOM_FLOOR;
		}
		var door = doorCandidates.PickRandom(this.rng);
		this.Tilemap[door.Y, door.X] = DungeonTiles.CAVE_DOOR;
		restart = true;
	    } else {
		foreach (var b in boxpoints) {
		    regions[b.Y, b.X] = (byte)rooms.Count;
		    this.Tilemap[b.Y, b.X] = DungeonTiles.CAVE_FLOOR;
		}

		bool cornerToCorner = false;
		if ((topLeft.X+w == room.topLeft.X && topLeft.Y+h == room.topLeft.Y) ||
		    (topLeft.X+w == room.topLeft.X && topLeft.Y == room.topLeft.Y+room.h) ||
		    (topLeft.X == room.topLeft.X+room.h && topLeft.Y == room.topLeft.Y+room.h) ||
		    (topLeft.X == room.topLeft.X+room.h && topLeft.Y+h == room.topLeft.Y)
		) {
		    cornerToCorner = true;
		}

		if (!makeHallway && !room.hallway && !cornerToCorner) {
		    foreach (var c in clear) {
			if (boxpoints.Contains(c)) {
			    this.Tilemap[c.Y, c.X] = DungeonTiles.CAVE_BLANK;
			    regions[c.Y, c.X] = 0;
			    boxpoints.Remove(c);
			}
		    }
		}
		rooms = new(rooms);
		rooms.Add(new EarthB2Room { points = boxpoints, topLeft = topLeft, w = w, h = h, hallway = makeHallway, gating = room.gating });
	    }

	    var tasks = new List<MapGenerationTask>();
	    tasks.Add(() => new MapState(this).EarthB2NextRoom(progressWindow, connectWindow, minTreasureRooms, features, featureAfter,
							       rooms, regions, roomTotal, treasureCount, featureCount, restart, gating));

	    await Task.Yield();
	    return new MapResult(tasks);
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

	public async Task<MapResult> PlaceExitStairs(PgTileFilter candidateFilter, byte tile, int minEntranceRadius, int minDoorRadius) {
	    var c = candidateFilter.Candidates(this.Tilemap.MapBytes);
	    var doors = this.Candidates(new List<byte> {DungeonTiles.CAVE_DOOR});

	    var e = c.Where((d) => d.Dist(this.Entrance) > minEntranceRadius);

	    foreach (var f in doors) {
		e = e.Where((d) => d.Dist(f) > minDoorRadius);
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

	public async Task<MapResult> PlaceChests(FF1Rom rom, StandardMaps maps, List<(MapIndex,byte)> preserveChests) {
	    this.OwnTilemap();
            this.OwnFeatures();

	    List<byte> chestsToPlace = new (this.Chests);
	    foreach (var i in preserveChests) {
		chestsToPlace.Remove(i.Item2);
	    }

			//maps = new List<Map>(maps);
			//maps[(int)MapIndex] = this.Tilemap;
			maps[this.MapIndex].Map = this.Tilemap;
	    try {
		await new RelocateChests(rom).shuffleChestLocations(this.rng, maps, new TileSetsData(rom), new Teleporters(rom, null), new MapIndex[] { this.MapIndex },
						preserveChests, null, 0x80, true, false,
						chestsToPlace, this.Traps, null);
	    } catch (Exception) {
		return new MapResult(false);
	    }

	    return await this.NextStep();
	}

	public async Task<MapResult> PlaceTile(int x, int y, byte tile) {
	    this.OwnTilemap();
	    this.Tilemap[y, x] = tile;
	    return await this.NextStep();
	}

	public async Task<MapResult> PlaceEntrance(byte tile) {
	    this.OwnTilemap();
	    this.Tilemap[this.Entrance.Y, this.Entrance.X] = tile;
	    return await this.NextStep();
	}

	public async Task<MapResult> PlaceRodRoom(byte exitTile) {
	    this.OwnTilemap();

	    var candidates = this.dt.cave_edges.Candidates(this.Tilemap.MapBytes);

	    candidates.Shuffle(this.rng);

	    foreach (var c in candidates) {
		Direction d;
		if (this.Tilemap[c.Y-1, c.X] == DungeonTiles.CAVE_FLOOR) {
		    d = Direction.Down;
		} else if (this.Tilemap[c.Y, c.X+1] == DungeonTiles.CAVE_FLOOR) {
		    d = Direction.Left;
		} else if (this.Tilemap[c.Y+1, c.X] == DungeonTiles.CAVE_FLOOR) {
		    d = Direction.Up;
		} else if (this.Tilemap[c.Y, c.X-1] == DungeonTiles.CAVE_FLOOR) {
		    d = Direction.Right;
		} else {
		    continue;
		}

		SCCoords topLeft;
		int _w, _h;
		var pts = this.AddBox(new List<SCCoords> { c.SmNeighbor(d).SmNeighbor(d) },
			    d, (7, 7), (7, 7),
			    DungeonTiles.CAVE_BLANK, DungeonTiles.CAVE_ROD_FLOOR, false, out topLeft, out _w, out _h);

		if (pts == null) {
		    continue;
		}

		foreach (var p in pts) {
		    this.Tilemap[p.Y, p.X] = DungeonTiles.CAVE_ROCK;
		}

		this.Tilemap[c.Y, c.X] = DungeonTiles.CAVE_ROD_FLOOR;
		var next = c;
		for (int i = 0; i < 3; i++ ) {
		    next = next.SmNeighbor(d);
		    this.Tilemap[next.Y, next.X] = DungeonTiles.CAVE_ROD_FLOOR;
		}

		next = next.SmNeighbor(d).SmNeighbor(d).SmNeighbor(d);

		this.Tilemap[next.Y, next.X] = exitTile;

		break;
	    }
	    return await this.NextStep();
	}

	public async Task<MapResult> PlaceVampire() {
	    var candidates = this.dt.room_tops.Candidates(this.Tilemap.MapBytes);

	    candidates.Shuffle(this.rng);

	    foreach (var c in candidates) {
		this.RenderFeature(c.SmLeft, DungeonTiles.VAMPIRE_ROOM.Tiles);
		return await this.NextStep();
	    }
	    return new MapResult(false);
	}

	public async Task<MapResult> CollectInfo(FF1Rom rom, StandardMaps maps) {
	    this.OwnFeatures();

	    List<byte> spikeTiles = new();
		List<byte> doorSpikeTiles = new();
		List<byte> walkableTiles = new();
	    this.Traps = new();
	    this.RoomFloorTiles = new();
	    this.RoomBattleTiles = new();
	    byte randomEncounter = 0x80;

	    this.NPCs = maps[this.MapIndex].MapObjects.ToList().Select(o => new NPC() { Index = o.Index, ObjectId = o.ObjectId, Coord = (o.Coords.X, o.Coords.Y), InRoom = o.InRoom, Stationary = o.Stationary }).ToList();

	    RelocateChests.FindRoomTiles(rom, this.tileSet,
				 this.RoomFloorTiles,
				 walkableTiles,
				 spikeTiles,
				 doorSpikeTiles,
				 this.RoomBattleTiles,
				 randomEncounter);

	    for (int y = 0; y < MAPSIZE; y++) {
		for (int x = 0; x < MAPSIZE; x++) {
		    byte t = this.Tilemap[y, x];
		    var tp = this.tileSet.Tiles[t].Properties;

		    if (tp.TilePropFunc == (TilePropFunc.TP_SPEC_TREASURE | TilePropFunc.TP_NOMOVE)) {
			this.Chests.Add(t);
		    }
		    if ((tp.TilePropFunc & TilePropFunc.TP_TELE_MASK) != 0) {
			this.Stairs.Add(t);
		    }
		    if (spikeTiles.Contains(t)) {
			this.Traps.Add(t);
		    }
		}
	    }

	    return await this.NextStep();
	}

	public async Task<MapResult> SetEntrance(SCCoords coord)
	{
	    this.Entrance = coord;
	    return await this.NextStep();
	}

	public async Task<MapResult> AddOverworldEntrance(OverworldTeleportIndex idx, TeleportDestination td)
	{
	    this.OverworldEntrances = new Dictionary<OverworldTeleportIndex, TeleportDestination>(this.OverworldEntrances);
	    this.OverworldEntrances[idx] = td;
	    return await this.NextStep();
	}

	public async Task<MapResult> AddMapDestination(TeleportIndex idx, TeleportDestination td)
	{
	    this.MapDestinations = new Dictionary<TeleportIndex, TeleportDestination>(this.MapDestinations);
	    this.MapDestinations[idx] = td;
	    return await this.NextStep();
	}

	public async Task<MapResult> MoveBats() {
	    this.OwnFeatures();

	    var cand = this.Candidates(new List<byte> {DungeonTiles.CAVE_FLOOR});

	    for (int i = 0; i < NPCs.Count; i++) {
		var npc = NPCs[i];
		if (npc.ObjectId == ObjectId.Bat) {
		    var coord = cand.SpliceRandom(this.rng);
		    npc.Coord = (coord.X, coord.Y);
		    npc.InRoom = false;
		    NPCs[i] = npc;
		}
	    }
	    return await this.NextStep();
	}

	public async Task<MapResult> SanityCheck() {
	    this.OwnFeatures();

	    var sanityError = new List<string>();

	    this.Tilemap.Flood((this.Entrance.X, this.Entrance.Y), (MapElement me) => {
		var tileProp = this.tileSet.Tiles[me.Value].Properties;

		if (tileProp.TilePropFunc == (TilePropFunc.TP_SPEC_TREASURE | TilePropFunc.TP_NOMOVE)) {
		    if (this.Chests.Contains(me.Value)) {
			this.Chests.Remove(me.Value);
		    } else {
			sanityError.Add($"!!! failed sanity check, unexpected or duplicate chest {me.Value:X} at {me.X}, {me.Y}");
		    }
		}

		if (this.Stairs.Contains(me.Value)) {
		    this.Stairs.Remove(me.Value);
		}

		if ((tileProp.TilePropFunc & TilePropFunc.TP_SPEC_DOOR) == TilePropFunc.TP_SPEC_DOOR) {
		    return true;
		}

		if ((tileProp.TilePropFunc & TilePropFunc.TP_SPEC_CLOSEROOM) == TilePropFunc.TP_SPEC_CLOSEROOM) {
		    return true;
		}

		if ((tileProp.TilePropFunc & TilePropFunc.TP_NOMOVE) == TilePropFunc.TP_NOMOVE) {
		    return false;
		}
		return true;
	    });

	    if (this.Chests.Count > 0 || this.Stairs.Count > 0 || sanityError.Count > 0) {
		foreach (var se in sanityError) {
		    Console.WriteLine(se);
		}
		Console.WriteLine($"!!! failed sanity check, unreachable chests: {this.Chests.Count} stairs: {this.Stairs.Count}");
		return new MapResult(false);
	    }

	    Console.WriteLine($"Passed sanity check");

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
			if ((this.tileSet.Tiles[t].Properties.TilePropFunc & TilePropFunc.TP_NOMOVE) == 0 ||
			    (this.tileSet.Tiles[t].Properties.TilePropFunc & TilePropFunc.TP_SPEC_DOOR) == TilePropFunc.TP_SPEC_DOOR ||
			    (this.tileSet.Tiles[t].Properties.TilePropFunc & TilePropFunc.TP_SPEC_CLOSEROOM) == TilePropFunc.TP_SPEC_CLOSEROOM)
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
