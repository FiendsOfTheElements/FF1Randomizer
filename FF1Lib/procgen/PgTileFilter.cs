using FF1Lib.Sanity;

namespace FF1Lib.Procgen
{
	public class Rule {
	public Rule(byte[,] pattern, byte replacement) {
	    this.pattern = pattern;
	    this.replacement = replacement;
	    this.rx = 0;
	    this.ry = 0;
	}
	public Rule(byte[,] pattern, byte replacement, int rx, int ry) {
	    this.pattern = pattern;
	    this.replacement = replacement;
	    this.rx = rx;
	    this.ry = ry;
	}
	public byte[,] pattern;
	public byte replacement;
	public int rx, ry;
    }

    public class PgTileFilter {
	const byte STAR = 0x80;
	const byte MATCH = 0x81;
	const byte CAVE = 0x82;

	Rule[] rules;
	HashSet<byte> starTiles;
	HashSet<byte> matchTiles;
	HashSet<byte> caveTiles;

	int matcherFunc;

	Dictionary<byte, int> regionTypeMap;
	byte[][] regionTypeList;

	public PgTileFilter(Rule[] rules,
		     HashSet<byte> starTiles,
		     HashSet<byte> matchTiles,
		     HashSet<byte> caveTiles) {
	    this.rules = rules;
	    this.starTiles = starTiles;
	    this.matchTiles = matchTiles;
	    this.caveTiles = caveTiles;
		this.matcherFunc = 0;
	}

		public PgTileFilter(byte[][] regionTypeList, Dictionary<byte, int> regionTypeMap) {
	    this.rules = null;
	    this.starTiles = null;
	    this.matchTiles = null;
	    this.caveTiles = null;
		this.regionTypeList = regionTypeList;
		this.regionTypeMap = regionTypeMap;
		this.matcherFunc = 1;
	}

	byte CheckRule(byte[,] tilemap, Rule rule, int x, int y, int xmax, int ymax) {
	    x -= rule.rx;
	    y -= rule.ry;
	    for (int j = 0; j < 3; j++) {
		for (int i = 0; i < 3; i++) {
		    var ruletile = rule.pattern[j,i];
		    int ty = ((y+(j-1))%ymax + ymax) % ymax;
		    int tx = ((x+(i-1))%xmax + xmax) % xmax;
		    var checktile = tilemap[ty, tx];
		    if (!((checktile == ruletile) ||
			  (ruletile == STAR && starTiles.Contains(checktile)) ||
			  (ruletile == MATCH && matchTiles.Contains(checktile)) ||
			  (ruletile == CAVE && caveTiles.Contains(checktile))))
		    {
			return 0xFF;
		    }
		}
	    }
	    return rule.replacement;
	}
	byte CheckSalient(byte[,] tilemap, int x, int y) {
		// Salients are single-tile bits that stick out,
		// they don't play well with with border tiles.

		var regionType = this.regionTypeMap[tilemap[y,x]];

		var p = new SCCoords(x, y);
		var adjacent = new SCCoords[] { p.OwUp, p.OwRight, p.OwDown, p.OwLeft };

		int[] counts = new int[4] { 0, 0, 0, 0};
		int[] countedTypes = new int[4] {-1, -1, -1, -1};

    	foreach (var t in adjacent) {
			var adjRegionType = this.regionTypeMap[tilemap[t.Y,t.X]];
			for (int i = 0; i < 4; i++) {
				if (countedTypes[i] == -1) {
					countedTypes[i] = adjRegionType;
				}
				if (countedTypes[i] == adjRegionType) {
					counts[i] += 1;
					break;
				}
			}
		}

		for (int i = 0; i < 4; i++) {
			if (counts[i] >= 3 && countedTypes[i] != regionType) {
				// surrounded on 3 or 4 sides, convert it to the tile type surrounding it
				return regionTypeList[countedTypes[i]][0];
			}
		}

		return 0xFF;
	}

	public byte[,] ApplyFilter(byte[,] tilemap, bool repeat) {
	    byte[,] newtilemap = new byte[tilemap.GetLength(0),tilemap.GetLength(1)];

	    bool anyChanged;
	    do {
		anyChanged = false;

		for (int y = 0; y < tilemap.GetLength(0); y++) {
		    for (int x = 0; x < tilemap.GetLength(1); x++) {
			byte rep = tilemap[y,x];

			if (matcherFunc == 0) {
			    foreach (var r in rules) {
				byte check = this.CheckRule(tilemap, r, x, y, tilemap.GetLength(1), tilemap.GetLength(0));
				if (check != 0xFF) {
				    rep = check;
				    break;
				}
			    }
			}
			else if (matcherFunc == 1) {
			    byte check = this.CheckSalient(tilemap, x, y);
			    if (check != 0xFF) {
				rep = check;
			    }
			}
			if (rep != tilemap[y,x]) {
			    anyChanged = true;
			}
			newtilemap[y,x] = rep;
		    }
		}
		tilemap = newtilemap;
	    } while (repeat && anyChanged);

	    return tilemap;
	}

	public List<SCCoords> Candidates(byte[,] tilemap) {
	    List<SCCoords> candidates = new();

	    for (int y = 0; y < tilemap.GetLength(0); y++) {
		for (int x = 0; x < tilemap.GetLength(1); x++) {
		    foreach (var r in rules) {
			byte check = this.CheckRule(tilemap, r, x, y, tilemap.GetLength(1), tilemap.GetLength(0));
			if (check != 0xFF) {
			    candidates.Add(new SCCoords(x, y));
			    break;
			}
		    }
		}
	    }

	    return candidates;
	}
    }
}
