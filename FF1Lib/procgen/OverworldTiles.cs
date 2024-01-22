using FF1Lib.Sanity;

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
	public const byte CARDIA_4 = 0x67;
	public const byte CARDIA_5 = 0x68;
	public const byte CARDIA_6 = 0x69;
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
	public const byte None = 0xFF;

	public const int LAND_REGION = 0;
	public const int OCEAN_REGION = 1;
	public const int RIVER_REGION = 2;
	public const int MOUNTAIN_REGION = 3;
	public const int GRASS_REGION = 4;
	public const int MARSH_REGION = 5;
	public const int FOREST_REGION = 6;
	public const int DESERT_REGION = 7;
	public const int OTHER_REGION = 8;

	public const int DOCK_REGION = 4;
	public const int ENTRANCES_REGION = 5;
	public const int BRIDGE_REGION = 6;

	public const int MainOceanRegionId = 0;

	public byte[][] BiomeRegionTypes = new byte[][] {
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
	public Dictionary<byte, int> BiomeRegionTypeMap;

	public static byte[][] TraversableRegionTypes = new byte[][] {
	    new byte[] {
		LAND, GRASS, GRASS_NW, GRASS_NE, GRASS_SW, GRASS_SE,
		MARSH, MARSH_NW, MARSH_NE, MARSH_SW, MARSH_SE,
		FOREST, FOREST_NW, FOREST_N, FOREST_NE,
		FOREST_W, FOREST_E,
		FOREST_SW, FOREST_S, FOREST_SE,
		SHORE_NW, SHORE_NE, SHORE_SW, SHORE_SE,
		DESERT, DESERT_NW, DESERT_NE, DESERT_SW, DESERT_SE,
		CITY_PAVED, AIRSHIP_DESERT},
	    new byte[] {OCEAN, SHORE_W, SHORE_N, SHORE_E, SHORE_S},
	    new byte[] {RIVER, RIVER_NW, RIVER_NE, RIVER_SW, RIVER_SE},
	    new byte[] { MOUNTAIN, MOUNTAIN_NW, MOUNTAIN_N, MOUNTAIN_NE ,
			 MOUNTAIN_W, MOUNTAIN_E,
			 MOUNTAIN_SW, MOUNTAIN_S, MOUNTAIN_SE},
	    new byte[] { DOCK_W, DOCK_E, DOCK_SE, DOCK_S, DOCK_SW, DOCK_SQ }
	};
	public Dictionary<byte, int> TraversableRegionTypeMap;

	public static List<byte> Entrances = new List<byte>{
	    CONERIA_CASTLE_ENTRANCE_W,
	    CONERIA_CASTLE_ENTRANCE_E,
	    EARTH_CAVE,
	    ELFLAND_CASTLE_W, ELFLAND_CASTLE_E,
	    MIRAGE_BOTTOM,
	    ASTOS_CASTLE_W, ASTOS_CASTLE_E,
	    ICE_CAVE, DWARF_CAVE, MATOYAS_CAVE,
	    TITAN_CAVE_E, TITAN_CAVE_W,
	    CARAVAN_DESERT, ORDEALS_CASTLE_W, ORDEALS_CASTLE_E,
	    SARDAS_CAVE, WATERFALL, CONERIA,
	    PRAVOKA, ELFLAND, MELMOND, CRESCENT_LAKE,
	    TOF_ENTRANCE_W, TOF_ENTRANCE_E,
	    GAIA, ONRAC, VOLCANO_TOP_W, VOLCANO_TOP_E,
	    CARDIA_2, CARDIA_4, CARDIA_5, CARDIA_6, CARDIA_1,
	    BAHAMUTS_CAVE, LEFEIN, MARSH_CAVE
	};

	public PgTileFilter expand_mountains;
	public PgTileFilter expand_oceans;
	public PgTileFilter connect_diagonals;
	public PgTileFilter remove_salients;

	public PgTileFilter apply_shores1;
	public PgTileFilter apply_shores2;
	public PgTileFilter apply_shores3;
	public PgTileFilter apply_shores4;
	public PgTileFilter apply_shores5;
	public PgTileFilter apply_shores6;
	public PgTileFilter apply_shores7;

	public PgTileFilter prune_forests;
	public PgTileFilter polish_mountains1;
	public PgTileFilter polish_mountains2;

	public PgTileFilter mountain_borders;
	public PgTileFilter river_borders;
	public PgTileFilter desert_borders;
	public PgTileFilter marsh_borders;
	public PgTileFilter grass_borders;
	public PgTileFilter forest_borders;

	public static PgFeature CONERIA_CITY_CASTLE = new PgFeature(new byte[,] {
	    {None, None, None,                 None,                None,  None, None, None},
	    {None, None, None, CONERIA_CASTLE_TOP_W, CONERIA_CASTLE_TOP_E, None, None, None},
	    {None, None, CITY_WALL_NW, CONERIA_CASTLE_MID_W, CONERIA_CASTLE_MID_E, CITY_WALL_NE, None, None},
	    {None, CITY_WALL_W1, CITY_WALL_W2, CONERIA_CASTLE_ENTRANCE_W, CONERIA_CASTLE_ENTRANCE_E, CITY_WALL_E2, CITY_WALL_E1, None},
	    {None, CITY_WALL_W3, CONERIA, CITY_PAVED, CITY_PAVED, CONERIA, CITY_WALL_E3, None},
	    {None, CITY_WALL_W4, CONERIA, CITY_PAVED, CITY_PAVED, CONERIA, CITY_WALL_E4, None},
	    {None, CITY_WALL_W5, CONERIA, CITY_PAVED, CITY_PAVED, CONERIA, CITY_WALL_E5, None},
	    {None, CITY_WALL_SW2, CITY_WALL_GATE_W, CITY_PAVED, CITY_PAVED, CITY_WALL_GATE_E, CITY_WALL_SE2, None},
	    {None, None, LAND,                 LAND,                LAND,  LAND, None, None},
	    }, new Dictionary<string, SCCoords> {
		{ "Coneria", new SCCoords(2, 4) },
		{ "ConeriaCastle1", new SCCoords(3, 3) },
		{ "StartingLocation", new SCCoords(3, 5) }
	    });

	public static PgFeature CONERIA_CITY = new PgFeature(new byte[,] {
	    {None, None, None,                 None,                None,  None, None, None},
	    {None, None, CITY_WALL_NW, CITY_WALL_N, CITY_WALL_N, CITY_WALL_NE, None, None},
	    {None, CITY_WALL_W1, CITY_WALL_W2, CITY_PAVED, CITY_PAVED,   CITY_WALL_E2, CITY_WALL_E1, None},
	    {None, CITY_WALL_W3, CONERIA, CITY_PAVED, CITY_PAVED, CONERIA, CITY_WALL_E3, None},
	    {None, CITY_WALL_W4, CONERIA, CITY_PAVED, CITY_PAVED, CONERIA, CITY_WALL_E4, None},
	    {None, CITY_WALL_W5, CONERIA, CITY_PAVED, CITY_PAVED, CONERIA, CITY_WALL_E5, None},
	    {None, CITY_WALL_SW2, CITY_WALL_GATE_W, CITY_PAVED, CITY_PAVED, CITY_WALL_GATE_E, CITY_WALL_SE2, None},
	    {None, None, LAND,                 LAND,                LAND,  LAND, None, None},
	    }, new Dictionary<string, SCCoords> {
		{ "Coneria", new SCCoords(2, 4) },
		{ "StartingLocation", new SCCoords(3, 5) }
	    });

	public static PgFeature CONERIA_CASTLE = new PgFeature(new byte[,] {
	    {None, None, None,                 None,                None,  None, None, None},
	    {None, None, None, CONERIA_CASTLE_TOP_W, CONERIA_CASTLE_TOP_E, None, None, None},
	    {None, None, CITY_WALL_NW, CONERIA_CASTLE_MID_W, CONERIA_CASTLE_MID_E, CITY_WALL_NE, None, None},
	    {None, CITY_WALL_W1, CITY_WALL_W2, CONERIA_CASTLE_ENTRANCE_W, CONERIA_CASTLE_ENTRANCE_E, CITY_WALL_E2, CITY_WALL_E1, None},
	    {None, CITY_WALL_W3, CITY_PAVED, CITY_PAVED, CITY_PAVED, CITY_PAVED, CITY_WALL_E3, None},
	    {None, CITY_WALL_SW1, CITY_WALL_GATE_W, CITY_PAVED, CITY_PAVED, CITY_WALL_GATE_E, CITY_WALL_SE1, None},
	    {None, None, LAND,                 LAND,                LAND,  LAND, None, None},
	    }, new Dictionary<string, SCCoords> {
		{ "ConeriaCastle1", new SCCoords(3, 3) }
	    });

	public static PgFeature TEMPLE_OF_FIENDS = new PgFeature(new byte[,] {
	    {None, None,      None,      None, None, None},
	    {None, None, TOF_TOP_W, TOF_TOP_E, None, None},
	    {None, TOF_BOTTOM_W, TOF_ENTRANCE_W, TOF_ENTRANCE_E, TOF_BOTTOM_E, None},
	    {None, None,      LAND,      LAND, None, None},
	}, new Dictionary<string, SCCoords> {
		{ "TempleOfFiends1", new SCCoords(2, 2) },
	    });

	public static PgFeature PRAVOKA_CITY = new PgFeature(new byte[,] {
	    {None, None, None, None, None, None, None},
	    {None, None, CITY_WALL_NW, CITY_WALL_N, CITY_WALL_NE, None, None},
	    {None, CITY_WALL_W1, CITY_WALL_W2, PRAVOKA, CITY_WALL_E2, CITY_WALL_E1, None},
	    {None, CITY_WALL_W3, PRAVOKA, CITY_PAVED, PRAVOKA, CITY_WALL_E3, None},
	    {None, CITY_WALL_SW1, CITY_WALL_GATE_W, CITY_PAVED, CITY_WALL_GATE_E, CITY_WALL_SE1, None},
	    {None,          LAND,             DOCK_S,       DOCK_S,             DOCK_S,          LAND, None},
	    {OCEAN,       OCEAN,              OCEAN,        OCEAN,              OCEAN,           OCEAN, OCEAN},
	    {OCEAN,       OCEAN,              OCEAN,        OCEAN,              OCEAN,           OCEAN, OCEAN}
	    }, new Dictionary<string, SCCoords> {
		{ "Pravoka", new SCCoords(2, 3) },
		{ "Ship", new SCCoords(3, 6) },
	    });

	public static PgFeature PRAVOKA_CITY_MOAT = new PgFeature(new byte[,] {
	    {None, None,  None,         None,        None,         None, None, None, None, None, None},
	    {None, OCEAN, OCEAN,       OCEAN,              OCEAN,        OCEAN,              OCEAN,           OCEAN, OCEAN, OCEAN, None},
	    {None, OCEAN, None, None,         None,        None,         None, None, None, OCEAN, None},
	    {None, OCEAN, None, None, CITY_WALL_NW, CITY_WALL_N, CITY_WALL_NE, None, None, OCEAN, None},
	    {None, OCEAN, None, CITY_WALL_W1, CITY_WALL_W2, PRAVOKA, CITY_WALL_E2, CITY_WALL_E1, None, OCEAN, None},
	    {None, OCEAN, None, CITY_WALL_W3, PRAVOKA, CITY_PAVED, PRAVOKA, CITY_WALL_E3, None, OCEAN, None},
	    {None, OCEAN, None, CITY_WALL_SW1, CITY_WALL_GATE_W, CITY_PAVED, CITY_WALL_GATE_E, CITY_WALL_SE1, None, OCEAN, None},
	    {None, OCEAN, None,          LAND,             DOCK_S,       DOCK_S,             DOCK_S,          LAND, None, OCEAN, None},
	    {OCEAN, OCEAN, OCEAN,       OCEAN,              OCEAN,        OCEAN,              OCEAN,           OCEAN, OCEAN, OCEAN, OCEAN},
	    {OCEAN, OCEAN, OCEAN,       OCEAN,              OCEAN,        OCEAN,              OCEAN,           OCEAN, OCEAN, OCEAN, OCEAN}
	    }, new Dictionary<string, SCCoords> {
		{ "Pravoka", new SCCoords(5, 4) },
		{ "Ship", new SCCoords(4, 8) },
		{ "Bridge", new SCCoords(5, 1) },
	    });

	public static PgFeature ELFLAND_CASTLE = new PgFeature(new byte[,] {
	    	    {None,               None,               None, None},
		    {None, SMALL_CASTLE_TOP_W, SMALL_CASTLE_TOP_E, None},
		    {None, ELFLAND_CASTLE_W, ELFLAND_CASTLE_E, None},
	    	    {None,               None,               None, None},
	}, new Dictionary<string, SCCoords> {
	    { "ElflandCastle", new SCCoords(2, 2) }
	    });

	public static PgFeature ASTOS_CASTLE = new PgFeature(new byte[,] {
	    {None, SMALL_CASTLE_TOP_W, SMALL_CASTLE_TOP_E, None},
	    {None, ASTOS_CASTLE_W, ASTOS_CASTLE_E, None},
	    {None, None, None, None},
	}, new Dictionary<string, SCCoords> {
		{ "NorthwestCastle", new SCCoords(1, 1) },
	    });

	public static PgFeature ORDEALS_CASTLE = new PgFeature(new byte[,] {
	    {None, SMALL_CASTLE_TOP_W, SMALL_CASTLE_TOP_E, None},
	    {None, ORDEALS_CASTLE_W, ORDEALS_CASTLE_E, None},
	    {None, None, None, None},
	}, new Dictionary<string, SCCoords> {
		{ "CastleOrdeals1", new SCCoords(1, 1) }
	    });

	public static PgFeature ELFLAND_TOWN = new PgFeature(new byte[,] {
	    {None,    None, None, None,    None, None},
	    {None, ELFLAND, LAND, LAND, ELFLAND, None},
	    {None, ELFLAND, LAND, LAND, ELFLAND, None},
	    {None,    None, None, None,    None, None},
	    },
	    new Dictionary<string, SCCoords> {
		{ "Elfland", new SCCoords(1, 1) },
	    });

	public static PgFeature ELFLAND_TOWN_CASTLE = new PgFeature(new byte[,] {
	    {None, LAND, LAND, LAND, LAND, None},
	    {LAND, LAND, SMALL_CASTLE_TOP_W, SMALL_CASTLE_TOP_E, LAND, LAND},
	    {LAND, ELFLAND, ELFLAND_CASTLE_W, ELFLAND_CASTLE_E, ELFLAND, LAND},
	    {LAND, ELFLAND, LAND, LAND, ELFLAND, LAND},
	    {LAND, LAND, LAND, LAND, LAND, LAND},
	}, new Dictionary<string, SCCoords> {
	    { "Elfland", new SCCoords(1, 3) },
	    { "ElflandCastle", new SCCoords(2, 2) }
	    });

	public static PgFeature MELMOND_TOWN = new PgFeature(new byte[,] {
	    {None, MELMOND, None,    None},
	    {None, MELMOND, MELMOND, None},
	    {None, None, None,    None},
	}, new Dictionary<string, SCCoords> {
		{ "Melmond", new SCCoords(1, 1) },
	    });

	public static PgFeature ONRAC_TOWN = new PgFeature(new byte[,] {
	    {None, None, None},
	    {None, ONRAC, ONRAC},
	    {None, ONRAC, ONRAC},
	    {None, None, None},
	}, new Dictionary<string, SCCoords> {
		{ "Onrac", new SCCoords(1, 1) },
	    });

	public static PgFeature LEFEIN_CITY = new PgFeature(new byte[,] {
	    {None,          None,             None,       None,             None,          None, None},
	    {None, None, CITY_WALL_NW, CITY_WALL_N, CITY_WALL_NE, None, None},
	    {None, CITY_WALL_W1, CITY_WALL_W2, LEFEIN, CITY_WALL_E2, CITY_WALL_E1, None},
	    {None, CITY_WALL_W3, LEFEIN, LEFEIN, LEFEIN, CITY_WALL_E3, None},
	    {None, CITY_WALL_SW1, CITY_WALL_GATE_W, CITY_PAVED, CITY_WALL_GATE_E, CITY_WALL_SE1, None},
	    {None,          None,             None,       None,             None,          None, None},
	}, new Dictionary<string, SCCoords> {
		{ "Lefein", new SCCoords(3, 3) },
	    });

	public static PgFeature CRESCENT_LAKE_CITY = new PgFeature(new byte[,] {
	    {None, None, None,         None,        None,         None, None},
	    {None, None, CITY_WALL_NW, CITY_WALL_N, CITY_WALL_NE, None, None},
	    {None, CITY_WALL_W1, CITY_WALL_W2, CITY_PAVED, CITY_WALL_E2, CITY_WALL_E1, None},
	    {None, CITY_WALL_W3, CITY_PAVED, CRESCENT_LAKE, CRESCENT_LAKE, CITY_WALL_E3, None},
	    {None, CITY_WALL_W4, CRESCENT_LAKE, CITY_PAVED, CRESCENT_LAKE, CITY_WALL_E4, None},
	    {None, CITY_WALL_W5, CRESCENT_LAKE, CITY_PAVED, CITY_PAVED, CITY_WALL_E5, None},
	    {None, CITY_WALL_SW2, CITY_WALL_GATE_W, CITY_PAVED, CITY_WALL_GATE_E, CITY_WALL_SE2, None},
	    {None,          None,             LAND,       LAND,             LAND, None, None},
	}, new Dictionary<string, SCCoords> {
		{ "CrescentLake", new SCCoords(2, 5) },
	    });

	public static PgFeature GAIA_TOWN = new PgFeature(new byte[,] {
	    {None, None, None, None, None},
	    {None, None, GAIA, GAIA, None},
	    {None, GAIA, GAIA, None, None},
	    {None, None, None, None, None},
	    }, new Dictionary<string, SCCoords> {
		{ "Gaia", new SCCoords(2, 2) },
	    });

	public static PgFeature MIRAGE_TOWER = new PgFeature(new byte[,] {
	    {None, None, None, None},
	    {None, MIRAGE_TOP, DESERT, None, },
	    {None, MIRAGE_BOTTOM, MIRAGE_SHADOW, None, },
	    {None, None, None, None},
	}, new Dictionary<string, SCCoords> {
		{ "MirageTower1", new SCCoords(1, 2) },
	    });

	public static PgFeature VOLCANO = new PgFeature(new byte[,] {
	    {MOUNTAIN,  RIVER, RIVER,                   RIVER, RIVER, MOUNTAIN},
	    {RIVER, RIVER, LAND,                     LAND, RIVER, RIVER},
	    {RIVER, LAND,  VOLCANO_TOP_W,   VOLCANO_TOP_E, LAND,  RIVER},
	    {RIVER, LAND,  VOLCANO_BASE_W, VOLCANO_BASE_E, LAND,  RIVER},
	    {RIVER, RIVER, LAND,                     LAND, RIVER, RIVER},
	    {MOUNTAIN,  RIVER, RIVER,                   RIVER, RIVER, MOUNTAIN},
	}, new Dictionary<string, SCCoords> {
		{ "GurguVolcano1", new SCCoords(3, 2) },
	    });

	public static PgFeature DRY_VOLCANO = new PgFeature(new byte[,] {
	    {None,   LAND,              LAND, None},
	    {LAND, VOLCANO_TOP_W,   VOLCANO_TOP_E, LAND},
	    {LAND, VOLCANO_BASE_W, VOLCANO_BASE_E, LAND},
	    {None,   LAND,               LAND, None},
	}, new Dictionary<string, SCCoords> {
		{ "GurguVolcano1", new SCCoords(3, 2) },
	    });

	public static PgFeature OASIS = new PgFeature(new byte[,] {
	    {DESERT_SE, DESERT_NW,      DESERT_NE, FOREST, FOREST},
	    {DESERT_NW, CARAVAN_DESERT, DESERT_SE, FOREST, FOREST},
	    {DESERT_SW, DESERT_SE,      FOREST, FOREST, FOREST},
	    {DESERT_NE,    FOREST,      FOREST, FOREST,   None},
	    {     None,    FOREST,      FOREST,   None,   None}
	}, new Dictionary<string, SCCoords> {
	    });

	public static PgFeature OASIS2 = new PgFeature(new byte[,] {
	    {     None, DESERT_NW,      DESERT_NE, FOREST, FOREST},
	    {DESERT_NW, CARAVAN_DESERT, DESERT_SE, FOREST, FOREST},
	    {DESERT_SW, DESERT_SE,      FOREST, FOREST, FOREST},
	    {     None,    FOREST,      FOREST, FOREST,   None},
	    {     None,    FOREST,      FOREST,   None,   None}
	}, new Dictionary<string, SCCoords> {
	    });

	public static PgFeature N_DOCK_STRUCTURE = new PgFeature(new byte[,] {
	    {None, DOCK_E,  OCEAN, None},
	    {None, DOCK_E,  OCEAN, None},
	    {None, DOCK_E,  OCEAN, None},
	    {None,   None,   None, None},
	    }, new Dictionary<string, SCCoords> {
		{ "Ship", new SCCoords(0, 0) },
	    });

	public static PgFeature S_DOCK_STRUCTURE = new PgFeature(new byte[,] {
	    {  None, None,   None,   None,     None},
	    {  None, DOCK_SE, DOCK_S, DOCK_SW, None},
	    {  None, OCEAN,   OCEAN,  OCEAN,   None},
	}, new Dictionary<string, SCCoords> {
		{ "Ship", new SCCoords(0, 0) },
	    });

	public static PgFeature W_DOCK_STRUCTURE = new PgFeature(new byte[,] {
	    {   None,   None,   None,  None},
	    {DOCK_S, DOCK_S,  DOCK_SW, None},
	    {OCEAN,   OCEAN,  DOCK_W,  None},
	    {   None,   None,   None,  None},
	}, new Dictionary<string, SCCoords> {
		{ "Ship", new SCCoords(0, 0) },
	    });

	public static PgFeature E_DOCK_STRUCTURE = new PgFeature(new byte[,] {
	    {   None, None,   None,   None},
	    {   None, DOCK_SE, DOCK_S, DOCK_S},
	    {   None, DOCK_E,   OCEAN,  OCEAN},
	    {   None,   None,   None,  None},
	    }, new Dictionary<string, SCCoords>  {
		{ "Ship", new SCCoords(0, 0) },
	    });

	public static PgFeature E_CANAL_STRUCTURE = new PgFeature(new byte[,] {
	    {None, None,     None,   None,  None,  None,  None},
	    {None, DOCK_SE, DOCK_S, DOCK_SW, None, None,    None},
	    {None, DOCK_E,   OCEAN,  OCEAN, OCEAN, OCEAN, OCEAN},
	    {None, None,     None,   None,  None,  None,  None},
	    {None, None,     None,   None,  None,  None,  None},
	    }, new Dictionary<string, SCCoords> {
		{"Canal", new SCCoords(5, 2)}
	    });

	public static PgFeature W_CANAL_STRUCTURE = new PgFeature(new byte[,] {
	    {None,     None,   None,  None,  None,  None, None},
	    {None, None,    None, DOCK_S, DOCK_S,  DOCK_SW, None},
	    {OCEAN,   OCEAN,  OCEAN, OCEAN, OCEAN, DOCK_W, None},
	    {None,     None,   None,  None,  None,  None, None},
	    {None,     None,   None,  None,  None,  None, None},
	    }, new Dictionary<string, SCCoords> {
		{"Canal", new SCCoords(1, 2)}
	    });

	public static PgFeature MOUNTAIN_CAVE_FEATURE = new PgFeature(new byte[,] {
	    {None, None, None, None, None},
	    {None, None,  None, None, None},
	    {None, GRASS, GRASS, GRASS, None},
	    {None, GRASS, GRASS, GRASS, None},
	    }, new Dictionary<string, SCCoords> { });

	public static PgFeature FOREST_MOUNTAIN_CAVE_FEATURE = new PgFeature(new byte[,] {
	    {None, None, None, None, None},
	    {None, None,  None, None, None},
	    {None, FOREST, FOREST, FOREST, None},
	    {None, FOREST, FOREST, FOREST, None},
	    }, new Dictionary<string, SCCoords> { });

	public static PgFeature AIRSHIP_FEATURE = new PgFeature(new byte[,] {
	    {None,        None,      None,         None,           None,         None,        None,        None, None},
	    {None, MOUNTAIN, MOUNTAIN, MOUNTAIN, MOUNTAIN, MOUNTAIN, MOUNTAIN, MOUNTAIN, None},
	    {None, MOUNTAIN,  MOUNTAIN,   MOUNTAIN, MOUNTAIN, MOUNTAIN, MOUNTAIN, MOUNTAIN, None},
	    {None, MOUNTAIN, MOUNTAIN,  DESERT_NW,  AIRSHIP_DESERT, DESERT_NE, MOUNTAIN, MOUNTAIN, None},
	    {None, MOUNTAIN, MOUNTAIN,  DESERT_SW,  AIRSHIP_DESERT, DESERT_SE, MOUNTAIN, MOUNTAIN, None},
	    {None, MOUNTAIN, MOUNTAIN,  MOUNTAIN,  AIRSHIP_DESERT, MOUNTAIN, MOUNTAIN, MOUNTAIN, None},
	    {None, LAND, MOUNTAIN,  MOUNTAIN,  AIRSHIP_DESERT, MOUNTAIN, MOUNTAIN, LAND, None},
	    {None,     None,     None,      None,            None,     None,     None,     None, None},
	    }, new Dictionary<string, SCCoords> {
		{"Airship", new SCCoords(4, 3)}
	    });

	public static PgFeature EARTH_CAVE_FEATURE = new PgFeature(EARTH_CAVE, "EarthCave1", true);
	public static PgFeature DWARF_CAVE_FEATURE = new PgFeature(DWARF_CAVE, "DwarfCave", true);
	public static PgFeature TITANS_TUNNEL_WEST = new PgFeature(TITAN_CAVE_W, "TitansTunnelWest", true);
	public static PgFeature TITANS_TUNNEL_EAST = new PgFeature(TITAN_CAVE_E, "TitansTunnelEast", true);
	public static PgFeature MATOYAS_CAVE_FEATURE = new PgFeature(MATOYAS_CAVE, "MatoyasCave", true);
	public static PgFeature SARDAS_CAVE_FEATURE = new PgFeature(SARDAS_CAVE, "SardasCave", true);
	public static PgFeature ICE_CAVE_FEATURE = new PgFeature(ICE_CAVE, "IceCave1", true);

	public static PgFeature MARSH_CAVE_FEATURE = new PgFeature(MARSH_CAVE, "MarshCave1", false);
	public static PgFeature BAHAMUTS_CAVE_FEATURE = new PgFeature(BAHAMUTS_CAVE, "BahamutCave1", false);
	public static PgFeature CARDIA_1_FEATURE = new PgFeature(CARDIA_1, "Cardia1", false);
	public static PgFeature CARDIA_2_FEATURE = new PgFeature(CARDIA_2, "Cardia2", false);
	public static PgFeature CARDIA_4_FEATURE = new PgFeature(CARDIA_4, "Cardia4", false);
	public static PgFeature CARDIA_5_FEATURE = new PgFeature(CARDIA_5, "Cardia5", false);
	public static PgFeature CARDIA_6_FEATURE = new PgFeature(CARDIA_6, "Cardia6", false);

	public static PgFeature WATERFALL_FEATURE = new PgFeature(WATERFALL, "Waterfall", false);

	public HashSet<byte> airship_landable;

    public OverworldTiles() {
        this.expand_mountains = new PgTileFilter(
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

        this.expand_oceans = new PgTileFilter(
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

        this.connect_diagonals = new PgTileFilter(
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
        non_water_tiles.Remove(WATERFALL);
	non_water_tiles.Remove(MOUNTAIN);

        var non_shore_tiles = new HashSet<byte>(allTiles);
        non_shore_tiles.Remove(OCEAN);
        non_shore_tiles.Remove(RIVER);
        non_shore_tiles.Remove(SHORE_NW);
        non_shore_tiles.Remove(SHORE_NE);
        non_shore_tiles.Remove(SHORE_SW);
        non_shore_tiles.Remove(SHORE_SE);
        non_shore_tiles.Remove(DOCK_W);
        non_shore_tiles.Remove(DOCK_E);
        non_shore_tiles.Remove(DOCK_SE);
        non_shore_tiles.Remove(DOCK_S);
        non_shore_tiles.Remove(DOCK_SW);
        non_shore_tiles.Remove(DOCK_SQ);

        var non_desert_tiles = new HashSet<byte>(allTiles);
	non_desert_tiles.Remove(DESERT);
	non_desert_tiles.Remove(DESERT_NW);
	non_desert_tiles.Remove(DESERT_NE);
	non_desert_tiles.Remove(DESERT_SW);
	non_desert_tiles.Remove(DESERT_SE);
	non_desert_tiles.Remove(MIRAGE_TOP);
	non_desert_tiles.Remove(MIRAGE_BOTTOM);
	non_desert_tiles.Remove(MIRAGE_SHADOW);

        var non_marsh_tiles = new HashSet<byte>(allTiles);
	non_marsh_tiles.Remove(MARSH);
	non_marsh_tiles.Remove(MARSH_NW);
	non_marsh_tiles.Remove(MARSH_NE);
	non_marsh_tiles.Remove(MARSH_SW);
	non_marsh_tiles.Remove(MARSH_SE);

        var non_grass_tiles = new HashSet<byte>(allTiles);
	non_grass_tiles.Remove(GRASS);
	non_grass_tiles.Remove(GRASS_NW);
	non_grass_tiles.Remove(GRASS_NE);
	non_grass_tiles.Remove(GRASS_SW);
	non_grass_tiles.Remove(GRASS_SE);

        var non_forest_tiles = new HashSet<byte>(allTiles);
	non_forest_tiles.Remove(FOREST);
	non_forest_tiles.Remove(FOREST_N);
	non_forest_tiles.Remove(FOREST_E);
	non_forest_tiles.Remove(FOREST_S);
	non_forest_tiles.Remove(FOREST_W);
	non_forest_tiles.Remove(FOREST_NW);
	non_forest_tiles.Remove(FOREST_NE);
	non_forest_tiles.Remove(FOREST_SW);
	non_forest_tiles.Remove(FOREST_SE);

	var mountain_tiles = new HashSet<byte>();
	mountain_tiles.Add(MOUNTAIN);
	mountain_tiles.Add(MOUNTAIN_N);
	mountain_tiles.Add(MOUNTAIN_E);
	mountain_tiles.Add(MOUNTAIN_S);
	mountain_tiles.Add(MOUNTAIN_W);
	mountain_tiles.Add(MOUNTAIN_NW);
	mountain_tiles.Add(MOUNTAIN_NE);
	mountain_tiles.Add(MOUNTAIN_SW);
	mountain_tiles.Add(MOUNTAIN_SE);
	mountain_tiles.Add(EARTH_CAVE);
	mountain_tiles.Add(ICE_CAVE);
	mountain_tiles.Add(DWARF_CAVE);
	mountain_tiles.Add(MATOYAS_CAVE);
	mountain_tiles.Add(TITAN_CAVE_E);
	mountain_tiles.Add(TITAN_CAVE_W);
	mountain_tiles.Add(SARDAS_CAVE);

        var non_mountain_tiles = new HashSet<byte>(allTiles);
	foreach (var m in mountain_tiles) {
	    non_mountain_tiles.Remove(m);
	}

        var pit_caves = new HashSet<byte>();
	pit_caves.Add(MARSH_CAVE);
	pit_caves.Add(BAHAMUTS_CAVE);
	pit_caves.Add(CARDIA_1);
	pit_caves.Add(CARDIA_2);
	pit_caves.Add(CARDIA_4);
	pit_caves.Add(CARDIA_5);
	pit_caves.Add(CARDIA_6);

        this.airship_landable = new HashSet<byte>();
	airship_landable.Add(LAND);
	airship_landable.Add(GRASS);
	airship_landable.Add(GRASS_NW);
	airship_landable.Add(GRASS_NE);
	airship_landable.Add(GRASS_SW);
	airship_landable.Add(GRASS_SE);
	airship_landable.Add(LAND_NO_FIGHT);

        this.apply_shores1 = new PgTileFilter(
            new Rule[] {
		new Rule(new byte[3,3] {
		    { STAR,   STAR,     STAR},
		    {  STAR, OCEAN,       _ },
		    { STAR,      _, MOUNTAIN}},
		    OCEAN),

		new Rule(new byte[3,3] {
		    { STAR,      STAR, STAR},
		    {_,         OCEAN, STAR},
		    { MOUNTAIN,     _, STAR}},
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

	this.apply_shores2 = new PgTileFilter(
	    new Rule[] {
		new Rule(new byte[3,3] {
		    {STAR, STAR,   STAR},
		    {SHORE_NE, SHORE_SW, STAR},
		    {STAR, STAR, STAR}},
		    OCEAN),

		new Rule(new byte[3,3] {
		    {STAR, STAR,   STAR},
		    {STAR, SHORE_NE, SHORE_SW},
		    {STAR, STAR, STAR}},
		    OCEAN),

		new Rule(new byte[3,3] {
		    {STAR, STAR,   SHORE_SW},
		    {STAR, SHORE_NE, STAR},
		    {STAR, STAR, STAR}},
		    OCEAN),

		new Rule(new byte[3,3] {
		    {STAR, STAR,     STAR},
		    {STAR, SHORE_SW, STAR},
		    {SHORE_NE, STAR, STAR}},
		    OCEAN),

		new Rule(new byte[3,3] {
		    {STAR, SHORE_SW, STAR},
		    {STAR, SHORE_NE, STAR},
		    {STAR, STAR, STAR}},
		    OCEAN),

		new Rule(new byte[3,3] {
		    {STAR, STAR,     STAR},
		    {STAR, SHORE_SW, STAR},
		    {STAR, SHORE_NE, STAR}},
		    OCEAN),


		new Rule(new byte[3,3] {
		    {STAR, STAR,   STAR},
		    {SHORE_SE, SHORE_NW, STAR},
		    {STAR, STAR, STAR}},
		    OCEAN),

		new Rule(new byte[3,3] {
		    {STAR, STAR,   STAR},
		    {STAR, SHORE_SE, SHORE_NW},
		    {STAR, STAR, STAR}},
		    OCEAN),

		new Rule(new byte[3,3] {
		    {SHORE_SE, STAR,   STAR},
		    {STAR, SHORE_NW, STAR},
		    {STAR, STAR, STAR}},
		    OCEAN),

		new Rule(new byte[3,3] {
		    {STAR, STAR,   STAR},
		    {STAR, SHORE_SE, STAR},
		    {STAR, STAR, SHORE_NW}},
		    OCEAN),

		new Rule(new byte[3,3] {
		    {STAR, SHORE_SE,   STAR},
		    {STAR, SHORE_NW, STAR},
		    {STAR, STAR, STAR}},
		    OCEAN),

		new Rule(new byte[3,3] {
		    {STAR, STAR,   STAR},
		    {STAR, SHORE_SE, STAR},
		    {STAR, SHORE_NW, STAR}},
		    OCEAN),

		new Rule(new byte[3,3] {
		    {STAR, STAR,   STAR},
		    {STAR, _,  OCEAN},
		    {STAR, OCEAN, STAR}},
		    SHORE_SE),

		new Rule(new byte[3,3] {
		    {STAR, STAR,    STAR},
		    {OCEAN, _, STAR},
		    {STAR, OCEAN,  STAR}},
		    SHORE_SW),

		new Rule(new byte[3,3] {
		    {STAR, OCEAN,  STAR},
		    {OCEAN, _, STAR},
		    {STAR, STAR,    STAR}},
		    SHORE_NW),

		new Rule(new byte[3,3] {
		    {STAR, OCEAN,  STAR},
		    {STAR, _,   OCEAN},
		    {STAR, STAR,    STAR}},
		    SHORE_NE)
	    }, allTiles, non_water_tiles, null);

	this.apply_shores3 = new PgTileFilter(
	    new Rule[] {
		new Rule(new byte[3,3] {
		    {STAR, OCEAN,   STAR},
		    {OCEAN,    _,      _},
		    {STAR,  STAR,   STAR}},
		    SHORE_NW),
		new Rule(new byte[3,3] {
		    {STAR, OCEAN,   STAR},
		    {OCEAN,    _,   STAR},
		    {STAR,     _,   STAR}},
		    SHORE_NW),

		new Rule(new byte[3,3] {
		    {STAR, OCEAN,   STAR},
		    {   _,     _,   OCEAN},
		    {STAR,  STAR,   STAR}},
		    SHORE_NE),
		new Rule(new byte[3,3] {
		    {STAR, OCEAN,   STAR},
		    {STAR,     _,   OCEAN},
		    {STAR,     _,   STAR}},
		    SHORE_NE),

		new Rule(new byte[3,3] {
		    {STAR,  STAR,   STAR},
		    {   _,     _,   OCEAN},
		    {STAR, OCEAN,   STAR}},
		    SHORE_SE),
		new Rule(new byte[3,3] {
		    {STAR,     _,   STAR},
		    {STAR,     _,   OCEAN},
		    {STAR, OCEAN,   STAR}},
		    SHORE_SE),

		new Rule(new byte[3,3] {
		    {STAR,   STAR,   STAR},
		    {OCEAN,     _,      _},
		    {STAR,  OCEAN,   STAR}},
		    SHORE_SW),
		new Rule(new byte[3,3] {
		    {STAR,      _,   STAR},
		    {OCEAN,     _,   STAR},
		    {STAR,  OCEAN,   STAR}},
		    SHORE_SW),
	    }, allTiles, airship_landable, null);


	this.apply_shores4 = new PgTileFilter(
	    new Rule[] {
		new Rule(new byte[3,3] {
		    {STAR, STAR,   STAR},
		    {STAR, OCEAN, _},
		    {STAR, STAR,   STAR}},
		    SHORE_W),
		new Rule(new byte[3,3] {
		    {STAR,  STAR,   STAR},
		    {_, OCEAN, STAR},
		    {STAR,  STAR,   STAR}},
		    SHORE_E),
		new Rule(new byte[3,3] {
		    {STAR, STAR,   STAR},
		    {STAR, OCEAN, STAR},
		    {STAR, _,  STAR}},
		    SHORE_N),
		new Rule(new byte[3,3] {
		    {STAR, _,   STAR},
		    {STAR, OCEAN,  STAR},
		    {STAR, STAR,    STAR,}},
		    SHORE_S),
	    }, allTiles, non_shore_tiles, null);

	this.apply_shores5 = new PgTileFilter(
	    new Rule[] {
		new Rule(new byte[3,3] {
		    {STAR, STAR,   STAR},
		    {STAR, OCEAN, STAR},
		    {STAR, DOCK_S,  STAR}},
		    SHORE_N),
		new Rule(new byte[3,3] {
		    {STAR, STAR,   STAR},
		    {STAR, OCEAN, STAR},
		    {STAR, DOCK_SE,  STAR}},
		    SHORE_N),
		new Rule(new byte[3,3] {
		    {STAR, STAR,   STAR},
		    {STAR, OCEAN, DOCK_E},
		    {STAR, STAR,   STAR}},
		    SHORE_W),
		new Rule(new byte[3,3] {
		    {STAR,  STAR,   STAR},
		    {DOCK_W, OCEAN, STAR},
		    {STAR,  STAR,   STAR}},
		    SHORE_E),
	    }, allTiles, non_shore_tiles, null);

	this.apply_shores6 = new PgTileFilter(
	    new Rule[] {
		new Rule(new byte[3,3] {
		    {STAR,     STAR, STAR},
		    {STAR, SHORE_SE, STAR},
		    {STAR,     STAR, SHORE_NW}},
		    SHORE_S),
		new Rule(new byte[3,3] {
		    {STAR,         STAR, STAR},
		    {STAR,     SHORE_SW, STAR},
		    {SHORE_NE,     STAR, STAR}},
		    SHORE_S),
	    }, allTiles, non_shore_tiles, null);

	this.apply_shores7 = new PgTileFilter(
	    new Rule[] {
		new Rule(new byte[3,3] {
		    {LAND,     STAR, STAR},
		    {STAR, SHORE_NW, STAR},
		    {STAR,     STAR, STAR}},
		    SHORE_N),
		new Rule(new byte[3,3] {
		    {STAR,         STAR, STAR},
		    {STAR,     SHORE_SW, STAR},
		    {LAND,     STAR, STAR}},
		    SHORE_S),
		new Rule(new byte[3,3] {
		    {STAR,     STAR, LAND},
		    {STAR, SHORE_NE, STAR},
		    {STAR,     STAR, STAR}},
		    SHORE_N),
		new Rule(new byte[3,3] {
		    {STAR,         STAR, STAR},
		    {STAR,     SHORE_SE, STAR},
		    {STAR,     STAR, LAND}},
		    SHORE_S),
	    }, allTiles, non_shore_tiles, null);

	this.mountain_borders = new PgTileFilter(
	    new Rule[] {
		new Rule(new byte[3,3] {
		    {STAR, STAR,       STAR},
		    {STAR, MOUNTAIN,  _},
		    {STAR,       _, STAR}},
		    MOUNTAIN_SE),
		new Rule(new byte[3,3] {
		    {STAR, STAR,       STAR},
		    {_, MOUNTAIN,  STAR},
		    {STAR,       _, STAR}},
		    MOUNTAIN_SW),
		new Rule(new byte[3,3] {
		    {STAR, _,       STAR},
		    {_, MOUNTAIN,  STAR},
		    {STAR,       STAR, STAR}},
		    MOUNTAIN_NW),
		new Rule(new byte[3,3] {
		    {STAR, _,       STAR},
		    {STAR, MOUNTAIN,  _},
		    {STAR,       STAR, STAR}},
		    MOUNTAIN_NE),
		new Rule(new byte[3,3] {
		    {STAR, STAR,       STAR},
		    {STAR, MOUNTAIN,  _},
		    {STAR,       STAR, STAR}},
		    MOUNTAIN_E),
		new Rule(new byte[3,3] {
		    {STAR, STAR,       STAR},
		    {_, MOUNTAIN,  STAR},
		    {STAR,       STAR, STAR}},
		    MOUNTAIN_W),
		new Rule(new byte[3,3] {
		    {STAR, _,       STAR},
		    {STAR, MOUNTAIN,  STAR},
		    {STAR,       STAR, STAR}},
		    MOUNTAIN_N),
		new Rule(new byte[3,3] {
		    {STAR, STAR,       STAR},
		    {STAR, MOUNTAIN,  STAR},
		    {STAR,       _, STAR}},
		    MOUNTAIN_S),
	    }, allTiles, non_mountain_tiles, null);

	this.river_borders = new PgTileFilter(
	    new Rule[] {
		new Rule(new byte[3,3] {
		    {STAR, _,   STAR},
		    {_, RIVER, RIVER},
		    {STAR, RIVER,   STAR}},
		    RIVER_NW),
		new Rule(new byte[3,3] {
		    {STAR, _,   STAR},
		    {RIVER, RIVER, _},
		    {STAR, RIVER,   STAR}},
		    RIVER_NE),
		new Rule(new byte[3,3] {
		    {STAR,  RIVER,   STAR},
		    {RIVER, RIVER, _},
		    {STAR, _,   STAR}},
		    RIVER_SE),
		new Rule(new byte[3,3] {
		    {STAR,  RIVER,   STAR},
		    {_,  RIVER, RIVER},
		    {STAR, _,   STAR}},
		    RIVER_SW),
	    }, allTiles, non_water_tiles, null);

	this.desert_borders = new PgTileFilter(
	    new Rule[] {
		new Rule(new byte[3,3] {
		    {   STAR, STAR, STAR},
		    {STAR, DESERT, DESERT},
		    {STAR, DESERT, CAVE}},
		    DESERT_SE),
		new Rule(new byte[3,3] {
		    {STAR,    STAR, STAR},
		    {DESERT, DESERT, DESERT},
		    {STAR,     CAVE, STAR}},
		    DESERT_SW),
		new Rule(new byte[3,3] {
		    {STAR,    DESERT, STAR},
		    {STAR,    DESERT, CAVE},
		    {STAR,    DESERT, STAR}},
		    DESERT_NE),
		new Rule(new byte[3,3] {
		    {STAR,    DESERT, STAR},
		    {CAVE,    DESERT, STAR},
		    {STAR,    DESERT, STAR}},
		    DESERT_SW),
		new Rule(new byte[3,3] {
		    {STAR,    DESERT, STAR},
		    {CAVE,    DESERT, STAR},
		    {STAR,    DESERT, STAR}},
		    DESERT_NW),
		new Rule(new byte[3,3] {
		    {STAR,    CAVE,    STAR},
		    {DESERT, DESERT, DESERT},
		    {STAR,    STAR, STAR}},
		    DESERT_NE),
		new Rule(new byte[3,3] {
		    {CAVE,    DESERT, STAR},
		    {DESERT, DESERT, STAR},
		    {STAR,    STAR, STAR}},
		    DESERT_NW),
		new Rule(new byte[3,3] {
		    {STAR,   _,  STAR},
		    {_, DESERT,  _},
		    {STAR, DESERT,  STAR}},
		    LAND),
		new Rule(new byte[3,3] {
		    {STAR,  _,   STAR},
		    {_, DESERT, DESERT},
		    {STAR,  _,   STAR}},
		    LAND),
		new Rule(new byte[3,3] {
		    {STAR, DESERT, STAR},
		    {_, DESERT, _},
		    {STAR,  _,  STAR}},
		    LAND),
		new Rule(new byte[3,3] {
		    {STAR,   _,   STAR},
		    {DESERT, DESERT, _},
		    {STAR,   _,   STAR}},
		    LAND),
		new Rule(new byte[3,3] {
		    {STAR, _,   STAR},
		    {_, DESERT, DESERT},
		    {STAR, DESERT,   STAR}},
		    DESERT_NW),
		new Rule(new byte[3,3] {
		    {STAR, _,   STAR},
		    {DESERT, DESERT, _},
		    {STAR, DESERT,   STAR}},
		    DESERT_NE),
		new Rule(new byte[3,3] {
		    {STAR,  DESERT,   STAR},
		    {DESERT, DESERT, _},
		    {STAR, _,   STAR}},
		    DESERT_SE),
		new Rule(new byte[3,3] {
		    {STAR,  DESERT,   STAR},
		    {_,  DESERT, DESERT},
		    {STAR, _,   STAR}},
		    DESERT_SW),
	    }, allTiles, non_desert_tiles, pit_caves);

	this.marsh_borders = new PgTileFilter(
	    new Rule[] {
		new Rule(new byte[3,3] {
		    {STAR, _,   STAR},
		    {_, MARSH, MARSH},
		    {STAR, MARSH,   STAR}},
		    MARSH_NW),
		new Rule(new byte[3,3] {
		    {STAR, _,   STAR},
		    {MARSH, MARSH, _},
		    {STAR, MARSH,   STAR}},
		    MARSH_NE),
		new Rule(new byte[3,3] {
		    {STAR,  MARSH,   STAR},
		    {MARSH, MARSH, _},
		    {STAR, _,   STAR}},
		    MARSH_SE),
		new Rule(new byte[3,3] {
		    {STAR,  MARSH,   STAR},
		    {_,  MARSH, MARSH},
		    {STAR, _,   STAR}},
		    MARSH_SW),
	    }, allTiles, non_marsh_tiles, null);

	this.grass_borders = new PgTileFilter(
	    new Rule[] {
		new Rule(new byte[3,3] {
		    {STAR,   _,  STAR},
		    {_, GRASS,  _},
		    {STAR, GRASS,  STAR}},
		    LAND),
		new Rule(new byte[3,3] {
		    {STAR,  _,   STAR},
		    {_, GRASS, GRASS},
		    {STAR,  _,   STAR}},
		    LAND),
		new Rule(new byte[3,3] {
		    {STAR, GRASS, STAR},
		    {_, GRASS, _},
		    {STAR,  _,  STAR}},
		    LAND),
		new Rule(new byte[3,3] {
		    {STAR,   _,   STAR},
		    {GRASS, GRASS, _},
		    {STAR,   _,   STAR}},
		    LAND),
		new Rule(new byte[3,3] {
		    {STAR, _,   STAR},
		    {_, GRASS, GRASS},
		    {STAR, GRASS,   STAR}},
		    GRASS_NW),
		new Rule(new byte[3,3] {
		    {STAR, _,   STAR},
		    {GRASS, GRASS, _},
		    {STAR, GRASS,   STAR}},
		    GRASS_NE),
		new Rule(new byte[3,3] {
		    {STAR,  GRASS,   STAR},
		    {GRASS, GRASS, _},
		    {STAR, _,   STAR}},
		    GRASS_SE),
		new Rule(new byte[3,3] {
		    {STAR,  GRASS,   STAR},
		    {_,  GRASS, GRASS},
		    {STAR, _,   STAR}},
		    GRASS_SW),
	    }, allTiles, non_grass_tiles, null);

	this.forest_borders = new PgTileFilter(
	    new Rule[] {
		new Rule(new byte[3,3] {
		    {STAR, STAR,       STAR},
		    {STAR, FOREST,  _},
		    {STAR,       _, STAR}},
		    FOREST_SE),
		new Rule(new byte[3,3] {
		    {STAR, STAR,       STAR},
		    {_, FOREST,  STAR},
		    {STAR,       _, STAR}},
		    FOREST_SW),
		new Rule(new byte[3,3] {
		    {STAR, _,       STAR},
		    {_, FOREST,  STAR},
		    {STAR,       STAR, STAR}},
		    FOREST_NW),
		new Rule(new byte[3,3] {
		    {STAR, _,       STAR},
		    {STAR, FOREST,  _},
		    {STAR,       STAR, STAR}},
		    FOREST_NE),
		new Rule(new byte[3,3] {
		    {STAR, STAR,       STAR},
		    {STAR, FOREST,  _},
		    {STAR,       STAR, STAR}},
		    FOREST_E),
		new Rule(new byte[3,3] {
		    {STAR, STAR,       STAR},
		    {_, FOREST,  STAR},
		    {STAR,       STAR, STAR}},
		    FOREST_W),
		new Rule(new byte[3,3] {
		    {STAR, _,       STAR},
		    {STAR, FOREST,  STAR},
		    {STAR,       STAR, STAR}},
		    FOREST_N),
		new Rule(new byte[3,3] {
		    {STAR, STAR,       STAR},
		    {STAR, FOREST,  STAR},
		    {STAR,       _, STAR}},
		    FOREST_S),
	    }, allTiles, non_forest_tiles, null);

	this.prune_forests = new PgTileFilter(
	    new Rule[] {
		new Rule(new byte[3,3] {
		    {STAR, STAR,     STAR},
		    {STAR, FOREST, FOREST},
		    {STAR, FOREST,  STAR}},
		    FOREST),
		new Rule(new byte[3,3] {
		    {STAR,   STAR,    STAR},
		    {FOREST, FOREST,  STAR},
		    {STAR,   FOREST,  STAR}},
		    FOREST),
		new Rule(new byte[3,3] {
		    {STAR,   FOREST, STAR},
		    {FOREST, FOREST, STAR},
		    {STAR,    STAR,  STAR}},
		    FOREST),
		new Rule(new byte[3,3] {
		    {STAR, FOREST,   STAR},
		    {STAR, FOREST, FOREST},
		    {STAR,   STAR,  STAR}},
		    FOREST),

		new Rule(new byte[3,3] {
		    {STAR,   STAR,  STAR},
		    {STAR, FOREST,  STAR},
		    {STAR,   STAR,  STAR}},
		    LAND),

	    }, allTiles, null, null);

	this.polish_mountains1 = new PgTileFilter(
	    new Rule[] {
		new Rule(new byte[3,3] {
		    {STAR, STAR,     STAR},
		    {STAR, MOUNTAIN, _},
		    {STAR,        _,  STAR}},
		    MOUNTAIN),
		new Rule(new byte[3,3] {
		    {STAR,   STAR,    STAR},
		    {_,      MOUNTAIN,  STAR},
		    {STAR,          _,  STAR}},
		    MOUNTAIN),
		new Rule(new byte[3,3] {
		    {STAR,          _, STAR},
		    {_,      MOUNTAIN, STAR},
		    {STAR,    STAR,  STAR}},
		    MOUNTAIN),
		new Rule(new byte[3,3] {
		    {STAR,        _,   STAR},
		    {STAR, MOUNTAIN,      _},
		    {STAR,     STAR,  STAR}},
		    MOUNTAIN),

		new Rule(new byte[3,3] {
		    {STAR,   STAR,   STAR},
		    {OCEAN, MOUNTAIN, STAR},
		    {STAR,   STAR,   STAR}},
		     OCEAN),

		new Rule(new byte[3,3] {
		    {STAR,   OCEAN,   STAR},
		    {STAR, MOUNTAIN, STAR},
		    {STAR,   STAR,   STAR}},
		     OCEAN),

		new Rule(new byte[3,3] {
		    {STAR,   STAR,   STAR},
		    {STAR, MOUNTAIN, OCEAN},
		    {STAR,   STAR,   STAR}},
		     OCEAN),

		new Rule(new byte[3,3] {
		    {STAR,   STAR,   STAR},
		    {STAR, MOUNTAIN, STAR},
		    {STAR,   OCEAN,   STAR}},
		     OCEAN),
	    }, allTiles, mountain_tiles, null);

	this.polish_mountains2 = new PgTileFilter(
	    new Rule[] {
		new Rule(new byte[3,3] {
		    {STAR, STAR,     STAR},
		    {STAR, MOUNTAIN, _},
		    {STAR,        _,  STAR}},
		    MOUNTAIN),
		new Rule(new byte[3,3] {
		    {STAR,   STAR,    STAR},
		    {_,      MOUNTAIN,  STAR},
		    {STAR,          _,  STAR}},
		    MOUNTAIN),
		new Rule(new byte[3,3] {
		    {STAR,          _, STAR},
		    {_,      MOUNTAIN, STAR},
		    {STAR,    STAR,  STAR}},
		    MOUNTAIN),
		new Rule(new byte[3,3] {
		    {STAR,        _,   STAR},
		    {STAR, MOUNTAIN,      _},
		    {STAR,     STAR,  STAR}},
		    MOUNTAIN),

		new Rule(new byte[3,3] {
		    {STAR,   STAR,   STAR},
		    {STAR, MOUNTAIN, STAR},
		    {STAR,   STAR,   STAR}},
		    LAND),

	    }, allTiles, mountain_tiles, null);


        this.BiomeRegionTypeMap = new Dictionary<byte, int>();
        for (int i = 0; i < BiomeRegionTypes.Length; i++) {
            for (int j = 0; j < BiomeRegionTypes[i].Length; j++) {
                this.BiomeRegionTypeMap[(byte)BiomeRegionTypes[i][j]] = i;
            }
        }

        this.TraversableRegionTypeMap = new Dictionary<byte, int>();
        for (int i = 0; i < TraversableRegionTypes.Length; i++) {
            for (int j = 0; j < TraversableRegionTypes[i].Length; j++) {
                this.TraversableRegionTypeMap[(byte)TraversableRegionTypes[i][j]] = i;
            }
        }

        this.remove_salients = new PgTileFilter(BiomeRegionTypes, BiomeRegionTypeMap);
    }



    }

}
