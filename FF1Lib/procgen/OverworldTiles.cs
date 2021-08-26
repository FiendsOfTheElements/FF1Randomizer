using System.Collections.Generic;
namespace FF1Lib.Procgen
{
    public class OverworldTiles {
        public const byte LAND = 0x00;
	public const byte CONERIA_CASTLE_ENTRANCE_W = 0x01;
	public const byte CONERIA_CASTLE_ENTRANCE_E = 0x02;
	public const byte FOREST_NW = 0x03;
	public const byte FOREST_N = 0x04;
	public const byte FOREST_NE = 0x05;
	public const byte SHORE_SE = 0x06;
	public const byte SHORE_S = 0x07;
	public const byte SHORE_SW = 0x08;
	public const byte CONERIA_CASTLE_TOP_W = 0x09;
	public const byte CONERIA_CASTLE_TOP_E = 0x0A;
	public const byte SMALL_CASTLE_TOP_W = 0x0B;
	public const byte SMALL_CASTLE_TOP_E = 0x0C;
	public const byte MIRAGE_TOP = 0x0D;
	public const byte EARTH_CAVE = 0x0E;
	public const byte DOCK_W = 0x0F;
	public const byte MOUNTAIN_NW = 0x10;
	public const byte MOUNTAIN_N = 0x11;
	public const byte MOUNTAIN_NE = 0x12;
	public const byte FOREST_W = 0x13;
	public const byte FOREST = 0x14;
	public const byte FOREST_E = 0x15;
	public const byte SHORE_E = 0x16;
	public const byte OCEAN = 0x17;
	public const byte SHORE_W = 0x18;
	public const byte CONERIA_CASTLE_MID_W = 0x19;
	public const byte CONERIA_CASTLE_MID_E = 0x1A;
	public const byte ELFLAND_CASTLE_W = 0x1B;
	public const byte ELFLAND_CASTLE_E = 0x1C;
	public const byte MIRAGE_BOTTOM = 0x1D;
	public const byte MIRAGE_SHADOW = 0x1E;
	public const byte DOCK_E = 0x1F;
	public const byte MOUNTAIN_W = 0x20;
	public const byte MOUNTAIN = 0x21;
	public const byte MOUNTAIN_E = 0x22;
	public const byte FOREST_SW = 0x23;
	public const byte FOREST_S = 0x24;
	public const byte FOREST_SE = 0x25;
	public const byte SHORE_NE = 0x26;
	public const byte SHORE_N = 0x27;
	public const byte SHORE_NW = 0x28;
	public const byte ASTOS_CASTLE_W = 0x29;
	public const byte ASTOS_CASTLE_E = 0x2A;
	public const byte ICE_CAVE = 0x2B;
	public const byte CITY_WALL_NW = 0x2C;
	public const byte CITY_WALL_N = 0x2D;
	public const byte CITY_WALL_NE = 0x2E;
	public const byte DWARF_CAVE = 0x2F;
	public const byte MOUNTAIN_SW = 0x30;
	public const byte MOUNTAIN_S = 0x31;
	public const byte MATOYAS_CAVE = 0x32;
	public const byte MOUNTAIN_SE = 0x33;
	public const byte TITAN_CAVE_E = 0x34;
	public const byte TITAN_CAVE_W = 0x35;
	public const byte CARAVAN_DESERT = 0x36;
	public const byte AIRSHIP_DESERT = 0x37;
	public const byte ORDEALS_CASTLE_W = 0x38;
	public const byte ORDEALS_CASTLE_E = 0x39;
	public const byte SARDAS_CAVE = 0x3A;
	public const byte CITY_WALL_W1 = 0x3B;
	public const byte CITY_WALL_W2 = 0x3C;
	public const byte CITY_PAVED = 0x3D;
	public const byte CITY_WALL_E2 = 0x3E;
	public const byte CITY_WALL_E1 = 0x3F;
	public const byte RIVER_NW = 0x40;
	public const byte RIVER_NE = 0x41;
	public const byte DESERT_NW = 0x42;
	public const byte DESERT_NE = 0x43;
	public const byte RIVER = 0x44;
	public const byte DESERT = 0x45;
	public const byte WATERFALL = 0x46;
	public const byte TOF_TOP_W = 0x47;
	public const byte TOF_TOP_E = 0x48;
	public const byte CONERIA = 0x49;
	public const byte PRAVOKA = 0x4A;
	public const byte CITY_WALL_W3 = 0x4B;
	public const byte ELFLAND = 0x4C;
	public const byte MELMOND = 0x4D;
	public const byte CRESCENT_LAKE = 0x4E;
	public const byte CITY_WALL_E3 = 0x4F;
	public const byte RIVER_SW = 0x50;
	public const byte RIVER_SE = 0x51;
	public const byte DESERT_SW = 0x52;
	public const byte DESERT_SE = 0x53;
	public const byte GRASS = 0x54;
	public const byte MARSH = 0x55;
	public const byte TOF_BOTTOM_W = 0x56;
	public const byte TOF_ENTRANCE_W = 0x57;
	public const byte TOF_ENTRANCE_E = 0x58;
	public const byte TOF_BOTTOM_E = 0x59;
	public const byte GAIA = 0x5A;
	public const byte CITY_WALL_W4 = 0x5B;
	public const byte CITY_WALL_SW1 = 0x5C;
	public const byte ONRAC = 0x5D;
	public const byte CITY_WALL_SE1 = 0x5E;
	public const byte CITY_WALL_E4 = 0x5F;
	public const byte GRASS_NW = 0x60;
	public const byte GRASS_NE = 0x61;
	public const byte MARSH_NW = 0x62;
	public const byte MARSH_NE = 0x63;
	public const byte VOLCANO_TOP_W = 0x64;
	public const byte VOLCANO_TOP_E = 0x65;
	public const byte CARDIA_2 = 0x66;
	public const byte CARDIA_3 = 0x67;
	public const byte CARDIA_4 = 0x68;
	public const byte CARDIA_5 = 0x69;
	public const byte CARDIA_1 = 0x6A;
	public const byte CITY_WALL_W5 = 0x6B;
	public const byte BAHAMUTS_CAVE = 0x6C;
	public const byte LEFEIN = 0x6D;
	public const byte MARSH_CAVE = 0x6E;
	public const byte CITY_WALL_E5 = 0x6F;
	public const byte GRASS_SW = 0x70;
	public const byte GRASS_SE = 0x71;
	public const byte MARSH_SW = 0x72;
	public const byte MARSH_SE = 0x73;
	public const byte VOLCANO_BASE_W = 0x74;
	public const byte VOLCANO_BASE_E = 0x75;
	public const byte LAND_NO_FIGHT = 0x76;
	public const byte DOCK_SE = 0x77;
	public const byte DOCK_S = 0x78;
	public const byte DOCK_SW = 0x79;
	public const byte DOCK_SQ = 0x7A;
	public const byte CITY_WALL_SW2 = 0x7B;
	public const byte CITY_WALL_S = 0x7C;
	public const byte CITY_WALL_GATE_W = 0x7D;
	public const byte CITY_WALL_GATE_E = 0x7E;
	public const byte CITY_WALL_SE2 = 0x7F;

    public const byte STAR = 0x80;
    public const byte _ = 0x81;
    public const byte CAVE = 0x82;



	public const int LAND_REGION = 0;
	public const int OCEAN_REGION = 1;
	public const int RIVER_REGION = 2;
    public const int MOUNTAIN_REGION = 3;
	public const int GRASS_REGION = 4;
	public const int MARSH_REGION = 5;
	public const int FOREST_REGION = 6;
	public const int DESERT_REGION = 7;
	public const int OTHER_REGION = 8;

	public byte[][] PreShoreRegionTypes = new byte[][] {
	    new byte[] { LAND, SHORE_NW, SHORE_NE, SHORE_SW, SHORE_SE },
	    new byte[] { OCEAN, SHORE_W, SHORE_N, SHORE_E, SHORE_S },
	    new byte[] { RIVER, RIVER_NW, RIVER_NE, RIVER_SW, RIVER_SE },
	    new byte[] { MOUNTAIN, MOUNTAIN_NW, MOUNTAIN_N, MOUNTAIN_NE ,
			   MOUNTAIN_W, MOUNTAIN_E,
			   MOUNTAIN_SW, MOUNTAIN_S, MOUNTAIN_SE},
	    new byte[] { GRASS, GRASS_NW, GRASS_NE, GRASS_SW, GRASS_SE },
	    new byte[] { MARSH, MARSH_NW, MARSH_NE, MARSH_SW, MARSH_SE },
	    new byte[] {FOREST, FOREST_NW, FOREST_N, FOREST_NE,
			  FOREST_W, FOREST_E,
			  FOREST_SW, FOREST_S, FOREST_SE},
	    new byte[] {DESERT, DESERT_NW, DESERT_NE, DESERT_SW, DESERT_SE}
	};
	public Dictionary<byte, int> PreShoreRegionTypeMap;

    public byte[][] TraversableRegionTypes = new byte[][] {
	    new byte[] {
            LAND, GRASS, GRASS_NW, GRASS_NE, GRASS_SW, GRASS_SE,
     MARSH, MARSH_NW, MARSH_NE, MARSH_SW, MARSH_SE,
     FOREST, FOREST_NW, FOREST_N, FOREST_NE,
     FOREST_W, FOREST_E,
     FOREST_SW, FOREST_S, FOREST_SE,
     SHORE_NW, SHORE_NE, SHORE_SW, SHORE_SE,
     DESERT, DESERT_NW, DESERT_NE, DESERT_SW, DESERT_SE,
     CITY_PAVED},
        new byte[] {OCEAN, SHORE_W, SHORE_N, SHORE_E, SHORE_S},
        new byte[] {RIVER, RIVER_NW, RIVER_NE, RIVER_SW, RIVER_SE},
	    new byte[] { MOUNTAIN, MOUNTAIN_NW, MOUNTAIN_N, MOUNTAIN_NE ,
			   MOUNTAIN_W, MOUNTAIN_E,
			   MOUNTAIN_SW, MOUNTAIN_S, MOUNTAIN_SE},
    };
	public Dictionary<byte, int> TraversableRegionTypeMap;

    public OwTileFilter expand_mountains;
    public OwTileFilter expand_oceans;
    public OwTileFilter connect_diagonals;
    public OwTileFilter remove_salients;

    public OwTileFilter apply_shores1;

    public OverworldTiles() {
        this.expand_mountains = new OwTileFilter(
            new Rule[] {
            new Rule(new byte[3,3] {
                {MOUNTAIN, _, _},
                {_,        _, _},
                {_,        _, _}
                }, MOUNTAIN),
            new Rule(new byte[3,3] {
                {_, MOUNTAIN, _},
                {_,        _, _},
                {_,        _, _},
                }, MOUNTAIN),
            new Rule(new byte[3,3] {
                {_, _, MOUNTAIN},
                {_,        _, _},
                {_,        _, _},
                }, MOUNTAIN),
            new Rule(new byte[3,3] {
                {_,        _, _},
                {MOUNTAIN, _, _},
                {_,        _, _}
                }, MOUNTAIN),
            new Rule(new byte[3,3] {
                {_,        _, _},
                {_, MOUNTAIN, _},
                {_,        _, _},
                }, MOUNTAIN),
            new Rule(new byte[3,3] {
                {_,        _, _},
                {_, _, MOUNTAIN},
                {_,        _, _},
                }, MOUNTAIN),
            new Rule(new byte[3,3] {
                {_,        _, _},
                {_,        _, _},
                {MOUNTAIN, _, _},
                }, MOUNTAIN),
            new Rule(new byte[3,3] {
                {_,        _, _},
                {_,        _, _},
                {_, MOUNTAIN, _},
                }, MOUNTAIN),
            new Rule(new byte[3,3] {
                {_,        _, _},
                {_,        _, _},
                {_, _, MOUNTAIN},
                }, MOUNTAIN),
            },
            null, new HashSet<byte>(new byte[] {LAND, OCEAN, MOUNTAIN}), null);

        this.expand_oceans = new OwTileFilter(
            new Rule[] {
            new Rule(new byte[3,3] {
                {OCEAN, _, _},
                {_,        _, _},
                {_,        _, _}
                }, OCEAN),
            new Rule(new byte[3,3] {
                {_, OCEAN, _},
                {_,        _, _},
                {_,        _, _},
                }, OCEAN),
            new Rule(new byte[3,3] {
                {_, _, OCEAN},
                {_,        _, _},
                {_,        _, _},
                }, OCEAN),
            new Rule(new byte[3,3] {
                {_,        _, _},
                {OCEAN, _, _},
                {_,        _, _}
                }, OCEAN),
            new Rule(new byte[3,3] {
                {_,        _, _},
                {_, OCEAN, _},
                {_,        _, _},
                }, OCEAN),
            new Rule(new byte[3,3] {
                {_,        _, _},
                {_, _, OCEAN},
                {_,        _, _},
                }, OCEAN),
            new Rule(new byte[3,3] {
                {_,        _, _},
                {_,        _, _},
                {OCEAN, _, _},
                }, OCEAN),
            new Rule(new byte[3,3] {
                {_,        _, _},
                {_,        _, _},
                {_, OCEAN, _},
                }, OCEAN),
            new Rule(new byte[3,3] {
                {_,        _, _},
                {_,        _, _},
                {_, _, OCEAN},
                }, OCEAN),
            },
            null, new HashSet<byte>(new byte[] {LAND, OCEAN, MOUNTAIN}), null);

        this.connect_diagonals = new OwTileFilter(
            new Rule[] {
            new Rule(new byte[3,3] {
                {STAR, RIVER, _},
                {STAR, _, RIVER},
                {STAR, STAR, STAR}},
                RIVER),
            new Rule(new byte[3,3] {
                {_, RIVER, STAR},
                {RIVER, _, STAR},
                {STAR, STAR, STAR}},
                RIVER),
            new Rule(new byte[3,3] {
                {STAR, OCEAN, _},
                {STAR, _, OCEAN},
                {STAR, STAR, STAR}},
                OCEAN),
            new Rule(new byte[3,3] {
                {_, OCEAN, STAR},
                {OCEAN, _, STAR},
                {STAR, STAR, STAR}},
                OCEAN),
            new Rule(new byte[3,3] {
                {STAR, MOUNTAIN, _},
                {STAR, _, MOUNTAIN},
                {STAR, STAR, STAR}},
                MOUNTAIN),
            new Rule(new byte[3,3] {
                {_, MOUNTAIN, STAR},
                {MOUNTAIN, _, STAR},
                {STAR, STAR, STAR}},
                MOUNTAIN),
            },
            new HashSet<byte>(new byte[] {LAND, MOUNTAIN, OCEAN, RIVER}),
            new HashSet<byte>(new byte[] {MOUNTAIN, LAND}),
            null);

        var allTiles = new HashSet<byte>();
        for (byte i = 0; i < 0x80; i++) {
            allTiles.Add(i);
        }
        var non_water_tiles = new HashSet<byte>(allTiles);
        non_water_tiles.Remove(OCEAN);
        non_water_tiles.Remove(RIVER);
        non_water_tiles.Remove(SHORE_N);
        non_water_tiles.Remove(SHORE_E);
        non_water_tiles.Remove(SHORE_S);
        non_water_tiles.Remove(SHORE_W);
        non_water_tiles.Remove(DOCK_W);
        non_water_tiles.Remove(DOCK_E);
        non_water_tiles.Remove(DOCK_SE);
        non_water_tiles.Remove(DOCK_S);
        non_water_tiles.Remove(DOCK_SW);
        non_water_tiles.Remove(DOCK_SQ);

        this.apply_shores1 = new OwTileFilter(
            new Rule[] {
            new Rule(new byte[3,3] {
    { STAR, STAR,   STAR},
    {  STAR, OCEAN, _ },
    { STAR, _,  MOUNTAIN}},
     OCEAN), 

    new Rule(new byte[3,3] {
        { STAR, STAR,    STAR },
      {_, OCEAN, STAR },
      { MOUNTAIN, _,   STAR}},
     OCEAN),

    new Rule(new byte[3,3] {
        { MOUNTAIN,  _,  STAR },
      { _, OCEAN, STAR },
      { STAR,  STAR,   STAR }},
     OCEAN),

    new Rule(new byte[3,3] {
        {STAR, _,    MOUNTAIN},
        {STAR, OCEAN,  _},
        {STAR,  STAR,   STAR}},
     OCEAN),

    new Rule(new byte[3,3] {
        {STAR,   OCEAN,   STAR},
        {OCEAN, OCEAN, _},
        {STAR,      _,  STAR}},
     SHORE_NW),

    new Rule(new byte[3,3] {
        {STAR, OCEAN,    STAR},
      {_, OCEAN, OCEAN},
      {STAR, _,   STAR}},
     SHORE_NE),

    new Rule(new byte[3,3] {
        {STAR,  _,  STAR},
      {_, OCEAN, OCEAN},
      {STAR, OCEAN,   STAR}},
     SHORE_SE),

    new Rule(new byte[3,3] {
        {STAR,     _,  STAR},
      {OCEAN, OCEAN,  _},
      {STAR,   OCEAN,  STAR}},
     SHORE_SW),
            }, allTiles, non_water_tiles, null);


        this.PreShoreRegionTypeMap = new Dictionary<byte, int>();
        for (int i = 0; i < PreShoreRegionTypes.Length; i++) {
            for (int j = 0; j < PreShoreRegionTypes[i].Length; j++) {
                this.PreShoreRegionTypeMap[(byte)PreShoreRegionTypes[i][j]] = i;
            }
        }

        this.TraversableRegionTypeMap = new Dictionary<byte, int>();
        for (int i = 0; i < TraversableRegionTypes.Length; i++) {
            for (int j = 0; j < TraversableRegionTypes[i].Length; j++) {
                this.TraversableRegionTypeMap[(byte)TraversableRegionTypes[i][j]] = i;
            }
        }

        this.remove_salients = new OwTileFilter(PreShoreRegionTypes, PreShoreRegionTypeMap);
    }



    }

}
