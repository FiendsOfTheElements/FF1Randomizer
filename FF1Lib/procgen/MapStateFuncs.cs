using FF1Lib.Sanity;
using OwGenerationTask = FF1Lib.Procgen.ProgenFramework.GenerationTaskType<FF1Lib.Procgen.OwResult>;

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
		    if (this.Tilemap[j, i] == clearTile) {
			clear = false;
			break;
		    }
		}
	    }
	    return clear;
	}

	public List<SCCoords> Candidates(byte candidateTile) {
	    List<SCCoords> pts = new();
	    for (int i = 0; i < MAPSIZE; i++) {
		for (int j = 0; j < MAPSIZE; j++) {
		    if (this.Tilemap[j, i] == candidateTile) {
			pts.Add(new SCCoords(i, j));
		    }
		}
	    }
	    return pts;
	}

	public List<SCCoords> AddRoom(List<SCCoords> candidates, Direction direction, (int n, int x) major, (int n, int x) minor, bool endOnly) {
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

		if (x < 0 || y < 0 || (x+w)>MAPSIZE || (y+h)>MAPSIZE) {
		    continue;
		}


		if (!this.CheckClear(x, y, w, h, DungeonTiles.CAVE_FLOOR)) {
		    continue;
		}

		this.Tilemap.Fill((x,y), (w, h), DungeonTiles.CAVE_FLOOR);

		var pts = InnerEdge(new SCCoords(x, y), w, h);

		if (!endOnly) {
		    return pts;
		}

		List<SCCoords> end = null;
		switch (direction) {
		    case Direction.Up:
			end = pts.Where((p) => p.Y < y+(h/2)).ToList();
			break;
		    case Direction.Right:
			end = pts.Where((p) => p.X > x+(w/2)).ToList();
			break;
		    case Direction.Down:
			end = pts.Where((p) => p.Y > y+(h/2)).ToList();
			break;
		    case Direction.Left:
			end = pts.Where((p) => p.X < x+(w/2)).ToList();
			break;
		}
		return end;
	    }
	    return null;
	}

	public async Task<MapResult> EarthB1Style() {
	    this.OwnTilemap();
	    var start = this.AddRoom(new List<SCCoords> { new SCCoords(30, 30) }, Direction.Down, (9, 9), (9, 9), false);

	    var ls = new List<List<SCCoords>>();
	    ls.Add(start);
	    ls.Add(start);
	    ls.Add(start);
	    ls.Add(start);
	    ls.Add(start);
	    ls.Add(start);

	    var ld = new List<Direction>();
	    ld.Add(Direction.Up);
	    ld.Add(Direction.Right);
	    ld.Add(Direction.Down);
	    ld.Add(Direction.Left);
	    ld.Add((Direction)this.rng.Between(0, 3));
	    ld.Add((Direction)this.rng.Between(0, 3));

	    for (int i = 0; i < 12; i++) {
		for (int j = 0; j < ld.Count; j++) {
		    var ret = this.AddRoom(ls[j], ld[j], (4, 12), (2, 3), true);
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
		    var ret = this.AddRoom(ls[j], ld[j], (8, 12), (8, 12), false);
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
	    Console.WriteLine($"{treasureRooms}");

	    if (treasureRooms < 2) {
		return new MapResult(false);
	    }

	    return await this.NextStep();
	}

	public async Task<MapResult> PlaceTreasureRoom() {
	    var cand = this.Candidates(DungeonTiles.CAVE_FLOOR);
	    cand.Shuffle(this.rng);

	    this.Tilemap[cand[0].Y, cand[0].X] = 0;

	    return await this.NextStep();
	}
    }
}
