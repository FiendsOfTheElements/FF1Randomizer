using System.Collections.Generic;
using FF1Lib.Sanity;

namespace FF1Lib.Procgen
{
    public class Rule {
	public Rule(byte[,] pattern, byte replacement) {
	    this.pattern = pattern;
	    this.replacement = replacement;
	}
	public byte[,] pattern;
	public byte replacement;
    }

    public class OwTileFilter {
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

	public OwTileFilter(Rule[] rules,
		     HashSet<byte> starTiles,
		     HashSet<byte> matchTiles,
		     HashSet<byte> caveTiles) {
	    this.rules = rules;
	    this.starTiles = starTiles;
	    this.matchTiles = matchTiles;
	    this.caveTiles = caveTiles;
		this.matcherFunc = 0;
	}

		public OwTileFilter(byte[][] regionTypeList, Dictionary<byte, int> regionTypeMap) {
	    this.rules = null;
	    this.starTiles = null;
	    this.matchTiles = null;
	    this.caveTiles = null;
		this.regionTypeList = regionTypeList;
		this.regionTypeMap = regionTypeMap;
		this.matcherFunc = 1;
	}

	byte CheckRule(byte[,] tilemap, Rule rule, int x, int y) {
	    for (int j = 0; j < 3; j++) {
		for (int i = 0; i < 3; i++) {
		    var ruletile = rule.pattern[j,i];
		    var checktile = tilemap[y+(j-1),x+(i-1)];
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
		
		/*if (regionType == OverworldTiles.FOREST_REGION || regionType == OverworldTiles.MOUNTAIN_REGION) {
			// Check that a forest or mountain tile has two adjacent forest
			// or mountain tiles at 90 degrees to one another, otherwise
			// turn it into plain land.
			int streak = 0;
			foreach (var t in adjacent) {
				var adjRegionType = this.regionTypeMap[tilemap[t.Y,t.X]];
				if (adjRegionType == regionType) {
					streak += 1;
				} else {
					streak = 0;
				}
				if (streak == 2) {
					return 0xFF;
				}
			}
			if (streak == 1 && tilemap[p.Y-1,p.X] == regionType) {
				return 0xFF;
			}
			return OverworldTiles.LAND;
		}*/

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

	public byte[,] ApplyFilter(byte[,] tilemap) {
		byte[,] newtilemap = new byte[OverworldState.MAPSIZE,OverworldState.MAPSIZE];

		for (int y = 0; y < OverworldState.MAPSIZE; y++) {
		    for (int x = 0; x < OverworldState.MAPSIZE; x++) {
			byte rep = tilemap[y,x];

			if (y == 0 || y == (OverworldState.MAPSIZE-1) || x == 0 || x == (OverworldState.MAPSIZE-1)) {
			    newtilemap[y,x] = rep;
			    continue;
			}

			if (matcherFunc == 0) {
				foreach (var r in rules) {
					byte check = this.CheckRule(tilemap, r, x, y);
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
			newtilemap[y,x] = rep;
		    }
		}
		tilemap = newtilemap;

	    return tilemap;
	}
    }
}
