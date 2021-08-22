using System.Collections.Generic;

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

	public OwTileFilter(Rule[] rules,
		     HashSet<byte> starTiles,
		     HashSet<byte> matchTiles,
		     HashSet<byte> caveTiles,
			 int matcherFunc = 0) {
	    this.rules = rules;
	    this.starTiles = starTiles;
	    this.matchTiles = matchTiles;
	    this.caveTiles = caveTiles;
		this.matcherFunc = matcherFunc;
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
		return 0;
	}

	public byte[,] ApplyFilter(byte[,] tilemap, bool repeat=false) {
	    bool recur = true;
	    while (recur) {
		byte[,] newtilemap = new byte[OverworldState.MAPSIZE,OverworldState.MAPSIZE];

		bool any_rule_matched = false;
		for (int y = 0; y < OverworldState.MAPSIZE; y++) {
		    for (int x = 0; x < OverworldState.MAPSIZE; x++) {
			byte rep = tilemap[y,x];

			if (y == 0 || y == (OverworldState.MAPSIZE-1) || x == 0 || x == (OverworldState.MAPSIZE-1)) {
			    newtilemap[y,x] = rep;
			    continue;
			}

			foreach (var r in rules) {
			    byte check = 0xFF;
			    switch(matcherFunc) {
				case 0:
				    check = this.CheckRule(tilemap, r, x, y);
				    break;
				case 1:
				    check = this.CheckSalient(tilemap, x, y);
				    break;
			    }
			    if (check != 0xFF) {
				rep = check;
				any_rule_matched = true;
				break;
			    }
			}
			newtilemap[y,x] = rep;
		    }
		}
		tilemap = newtilemap;
		recur = repeat && any_rule_matched;
	    }
	    return tilemap;
	}
    }
}
