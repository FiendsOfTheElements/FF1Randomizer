using FF1Lib.Sanity;

namespace FF1Lib.Procgen
{
    public class DungeonTiles {

        public const byte TOWN_ROOF_TOP = 0x17;
        public const byte TOWN_ROOF_MIDDLE = 0x18;
        public const byte TOWN_BIG_WINDOW = 0x1C;
        public const byte TOWN_SMALL_WINDOWS = 0x1D;
        public const byte TOWN_BLACK_MAGIC_SIGN = 0x24;
	public const byte TOWN_INN_SIGN = 0x25;

	public const byte CAVE_ROOM_NW = 0x00;
	public const byte CAVE_ROOM_N  = 0x01;
	public const byte CAVE_ROOM_NE = 0x02;
	public const byte CAVE_ROOM_W  = 0x03;
	public const byte CAVE_ROOM_E  = 0x05;
	public const byte CAVE_ROOM_SW  = 0x06;
	public const byte CAVE_ROOM_S  = 0x07;
	public const byte CAVE_ROOM_SE  = 0x08;

        public const byte CAVE_BLANK = 0x1A;
        public const byte CAVE_ROOM_FLOOR = 0x2E;
        public const byte CAVE_FLOOR = 0x41;
        public const byte CAVE_DOOR = 0x36;
        public const byte CAVE_CLOSE_DOOR = 0x3A;
        public const byte CAVE_ROCK = 0x3E;

        public const byte CAVE_SPIKES = 0x13;
        public const byte LIGHT_EARTH_ORB = 0x0E;
        public const byte EXIT_EARTH_CAVE = 0x0F;

	public const byte CAVE_ROD_FLOOR = 0x20;

        public const byte CAVE_WALL_N = 0x30;
        public const byte CAVE_WALL_W = 0x32;
        public const byte CAVE_WALL_E = 0x33;
        public const byte CAVE_WALL_NW = 0x34;
        public const byte CAVE_WALL_NE = 0x35;

	public const byte CAVE_EARTH_WARP = 0x18;
	public const byte CAVE_EXIT_TO_EARTH_B2 = 0x24;
	public const byte CAVE_EXIT_TO_EARTH_B3 = 0x25;
	public const byte CAVE_EXIT_TO_EARTH_B4 = 0x26;
	public const byte CAVE_EXIT_TO_EARTH_B5 = 0x27;

	public const byte RUBY_CHEST = 0x51;
	public const byte CAVE_BOSS_DECORATION_W = 0x10;
	public const byte CAVE_BOSS_DECORATION_E = 0x11;

	public const byte STAR = 0x80;
	public const byte _ = 0x81;
	public const byte ROOM = 0x82;
	public const byte None = 0xFF;

	public static PgFeature BLACK_MAGIC_SHOP = new PgFeature(new byte[,] {
	    {TOWN_ROOF_TOP, TOWN_ROOF_TOP, TOWN_ROOF_TOP},
	    {TOWN_ROOF_MIDDLE, TOWN_BLACK_MAGIC_SIGN, TOWN_ROOF_MIDDLE},
	    {TOWN_BIG_WINDOW, None, TOWN_BIG_WINDOW},
	    }, new Dictionary<string, SCCoords> {
	    });

	public static PgFeature SMALL_VAMPIRE_FEATURE = new PgFeature(new byte[,] {
	    {CAVE_ROOM_E, RUBY_CHEST, CAVE_ROOM_W},
	    {CAVE_BOSS_DECORATION_W, CAVE_ROOM_FLOOR, CAVE_BOSS_DECORATION_E},
	    }, new Dictionary<string, SCCoords> {
		{"Vampire", new SCCoords(1, 1)}
	    });

	public static PgFeature VAMPIRE_ROOM = new PgFeature(new byte[,] {
	    {CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR},
	    {CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, RUBY_CHEST, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR},
	    {CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_BOSS_DECORATION_W, CAVE_ROOM_FLOOR, CAVE_BOSS_DECORATION_E, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_DOOR, CAVE_ROOM_FLOOR},
	    {None, None, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_E, CAVE_ROOM_FLOOR, CAVE_ROOM_W, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, None, None},
	    {None, None, None, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_DOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, None, None, None},
	    }, new Dictionary<string, SCCoords> {
		{"Vampire", new SCCoords(5, 2)},
		{"ExitDoor", new SCCoords(9, 2)}
	    });

	public static PgFeature ROD_ROOM = new PgFeature(new byte[,] {
	    {CAVE_ROCK, CAVE_ROCK, CAVE_ROCK, CAVE_ROCK, CAVE_ROCK, CAVE_ROCK, CAVE_ROCK},
	    {CAVE_ROCK, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_ROCK},
	    {CAVE_ROCK, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_ROCK},
	    {CAVE_ROCK, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_EXIT_TO_EARTH_B4, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_ROCK},
	    {CAVE_ROCK, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_ROCK},
	    {CAVE_ROCK, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_ROD_FLOOR, CAVE_ROCK},
	    {CAVE_ROCK, CAVE_ROCK, CAVE_ROCK, CAVE_FLOOR, CAVE_ROCK, CAVE_ROCK, CAVE_ROCK},
	    {None, None, CAVE_ROCK, CAVE_FLOOR, CAVE_ROCK, None, None},
	    {None, None, CAVE_ROCK, CAVE_FLOOR, CAVE_ROCK, None, None},
	    }, new Dictionary<string, SCCoords> {
		{"RodPlate", new SCCoords(3, 3)},
	    });

	public static PgFeature LICH_ROOM = new PgFeature(new byte[,] {
	    {CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR},
	    {CAVE_ROOM_FLOOR, CAVE_SPIKES, CAVE_SPIKES, CAVE_ROOM_FLOOR, EXIT_EARTH_CAVE, CAVE_ROOM_FLOOR, CAVE_SPIKES, CAVE_SPIKES, CAVE_ROOM_FLOOR},
	    {CAVE_ROOM_FLOOR, CAVE_SPIKES, CAVE_SPIKES, CAVE_BOSS_DECORATION_W, LIGHT_EARTH_ORB, CAVE_BOSS_DECORATION_E, CAVE_SPIKES, CAVE_SPIKES, CAVE_ROOM_FLOOR},
	    {CAVE_ROOM_FLOOR, CAVE_SPIKES, CAVE_SPIKES, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_SPIKES, CAVE_SPIKES, CAVE_ROOM_FLOOR},
	    {CAVE_ROOM_FLOOR, CAVE_SPIKES, CAVE_SPIKES, CAVE_SPIKES, CAVE_ROOM_FLOOR, CAVE_SPIKES, CAVE_SPIKES, CAVE_SPIKES, CAVE_ROOM_FLOOR},
	    {CAVE_ROOM_FLOOR, CAVE_SPIKES, CAVE_SPIKES, CAVE_SPIKES, CAVE_ROOM_FLOOR, CAVE_SPIKES, CAVE_SPIKES, CAVE_SPIKES, CAVE_ROOM_FLOOR},
	    {CAVE_ROOM_FLOOR, CAVE_SPIKES, CAVE_SPIKES, CAVE_SPIKES, CAVE_ROOM_FLOOR, CAVE_SPIKES, CAVE_SPIKES, CAVE_SPIKES, CAVE_ROOM_FLOOR},
	    {CAVE_ROOM_FLOOR, CAVE_SPIKES, CAVE_SPIKES, CAVE_SPIKES, CAVE_ROOM_FLOOR, CAVE_SPIKES, CAVE_SPIKES, CAVE_SPIKES, CAVE_ROOM_FLOOR},
	    {CAVE_ROOM_FLOOR, CAVE_SPIKES, CAVE_SPIKES, CAVE_SPIKES, CAVE_ROOM_FLOOR, CAVE_SPIKES, CAVE_SPIKES, CAVE_SPIKES, CAVE_ROOM_FLOOR},
	    {CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR},
	    {CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_DOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR},
	    }, new Dictionary<string, SCCoords> {
		{"LichOrb", new SCCoords(4, 6)},
	});

	public PgTileFilter cave_rock_walls;
	public PgTileFilter cave_room_walls;
	public PgTileFilter cave_room_walls2;
	public PgTileFilter cave_corners;
	public PgTileFilter cave_wall_corners;
	public PgTileFilter earth_cave_walls;
	public PgTileFilter earth_cave_walls2;
	public PgTileFilter earth_cave_walls3;
	public PgTileFilter earth_cave_extend_walls;
	public PgTileFilter earth_cave_walls4;
	public PgTileFilter earth_cave_walls5;
	public PgTileFilter cave_edges;
	public PgTileFilter room_tops;

	public DungeonTiles() {

	    var allTiles = new HashSet<byte>();
	    for (byte i = 0; i < 0x80; i++) {
		allTiles.Add(i);
	    }

	    var room_tiles = new HashSet<byte>();
	    room_tiles.Add(CAVE_ROOM_FLOOR);
	    room_tiles.Add(CAVE_DOOR);
	    room_tiles.Add(CAVE_ROOM_NW);
	    room_tiles.Add(CAVE_ROOM_N);
	    room_tiles.Add(CAVE_ROOM_NE);
	    room_tiles.Add(CAVE_ROOM_E);
	    room_tiles.Add(CAVE_ROOM_W);
	    room_tiles.Add(CAVE_ROOM_SE);
	    room_tiles.Add(CAVE_ROOM_S);
	    room_tiles.Add(CAVE_ROOM_SW);
	    room_tiles.Add(RUBY_CHEST);
	    room_tiles.Add(CAVE_BOSS_DECORATION_W);
	    room_tiles.Add(CAVE_BOSS_DECORATION_E);
	    room_tiles.Add(CAVE_SPIKES);
	    room_tiles.Add(LIGHT_EARTH_ORB);
	    room_tiles.Add(EXIT_EARTH_CAVE);

	    var non_room_tiles = new HashSet<byte>(allTiles);
	    foreach (var t in room_tiles) {
		non_room_tiles.Remove(t);
	    }

	    var non_floor_tiles = new HashSet<byte>(allTiles);
	    non_floor_tiles.Remove(CAVE_FLOOR);

	    var floorOrRodFloor = new HashSet<byte>();
	    floorOrRodFloor.Add(CAVE_FLOOR);
	    floorOrRodFloor.Add(CAVE_ROD_FLOOR);

	    this.cave_rock_walls = new PgTileFilter(
		new Rule[] {
		    new Rule(new byte[3,3] {
			{STAR, _, STAR},
			{STAR, CAVE_BLANK, STAR},
			{STAR, STAR,       STAR}},
			CAVE_ROCK),

		    new Rule(new byte[3,3] {
			{STAR, STAR, _},
			{STAR, CAVE_BLANK, STAR},
			{STAR, STAR,       STAR}},
			CAVE_ROCK),
		    new Rule(new byte[3,3] {
			{STAR, STAR,       STAR},
			{STAR, CAVE_BLANK, _},
			{STAR, STAR,       STAR}},
			CAVE_ROCK),
		    new Rule(new byte[3,3] {
			{STAR, STAR,       STAR},
			{STAR, CAVE_BLANK, STAR},
			{STAR, STAR,       _}},
			CAVE_ROCK),
		    new Rule(new byte[3,3] {
			{STAR, STAR,       STAR},
			{STAR, CAVE_BLANK, STAR},
			{STAR, _, STAR}},
			CAVE_ROCK),
		    new Rule(new byte[3,3] {
			{STAR, STAR,       STAR},
			{STAR, CAVE_BLANK, STAR},
			{_, STAR, STAR}},
			CAVE_ROCK),
		    new Rule(new byte[3,3] {
			{STAR, STAR,             STAR},
			{_, CAVE_BLANK, STAR},
			{STAR, STAR, STAR}},
			CAVE_ROCK),
		    new Rule(new byte[3,3] {
			{_, STAR,             STAR},
			{STAR, CAVE_BLANK, STAR},
			{STAR, STAR, STAR}},
			CAVE_ROCK),
		}, allTiles, floorOrRodFloor, null);

	    this.cave_room_walls = new PgTileFilter(
		new Rule[] {
		    new Rule(new byte[3,3] {
			{_,                  _,            STAR},
			{_,    CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR},
			{STAR, CAVE_ROOM_FLOOR, ROOM}},
			CAVE_ROOM_NW),

		    new Rule(new byte[3,3] {
			{STAR,                  _,         _},
			{CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, _},
			{ROOM, CAVE_ROOM_FLOOR, STAR}},
			CAVE_ROOM_NE),

		    new Rule(new byte[3,3] {
			{STAR, ROOM, STAR},
			{STAR, CAVE_DOOR, STAR},
			{STAR,    _, STAR}},
			CAVE_DOOR),

		    new Rule(new byte[3,3] {
			{STAR, ROOM, STAR},
			{STAR, ROOM, STAR},
			{STAR,    _, STAR}},
			CAVE_WALL_N),

		    new Rule(new byte[3,3] {
			{STAR,    _, STAR},
			{STAR, ROOM, STAR},
			{STAR, ROOM, STAR}},
			CAVE_ROOM_N),

		    new Rule(new byte[3,3] {
			{STAR, STAR, STAR},
			{_, CAVE_ROOM_FLOOR, ROOM},
			{STAR, STAR, STAR}},
			CAVE_ROOM_W),

		    new Rule(new byte[3,3] {
			{STAR, STAR, STAR},
			{ROOM, CAVE_ROOM_FLOOR, _},
			{STAR, STAR, STAR}},
			CAVE_ROOM_E),
		}, allTiles, non_room_tiles, room_tiles);

	    this.cave_room_walls2 = new PgTileFilter(
		new Rule[] {
		    new Rule(new byte[3,3] {
			{STAR,            STAR,      STAR},
			{STAR, CAVE_ROOM_W,      STAR},
			{STAR, CAVE_WALL_N, STAR}},
			CAVE_ROOM_SW),

		    new Rule(new byte[3,3] {
			{STAR,            STAR,      STAR},
			{CAVE_WALL_N, CAVE_ROOM_FLOOR,  STAR},
			{STAR, CAVE_WALL_N, STAR}},
			CAVE_ROOM_SW),

		    new Rule(new byte[3,3] {
			{STAR,            STAR,      STAR},
			{STAR, CAVE_ROOM_E,          STAR},
			{STAR, CAVE_WALL_N, STAR}},
			CAVE_ROOM_SE),

		    new Rule(new byte[3,3] {
			{STAR,            STAR,      STAR},
			{STAR, CAVE_ROOM_FLOOR,      CAVE_WALL_N},
			{STAR, CAVE_WALL_N, STAR}},
			CAVE_ROOM_SE),

		    new Rule(new byte[3,3] {
			{STAR,            STAR,      STAR},
			{STAR, CAVE_ROOM_FLOOR,      CAVE_DOOR},
			{STAR, CAVE_WALL_N, STAR}},
			CAVE_ROOM_SE),

		    new Rule(new byte[3,3] {
			{STAR,            STAR,      STAR},
			{STAR, CAVE_ROOM_FLOOR,      STAR},
			{STAR, CAVE_WALL_N, STAR}},
			CAVE_ROOM_S),

		    new Rule(new byte[3,3] {
			{STAR,            STAR,      STAR},
			{STAR, CAVE_ROOM_FLOOR,      STAR},
			{STAR, CAVE_DOOR, STAR}},
			CAVE_ROOM_S),

		    new Rule(new byte[3,3] {
			{STAR,  CAVE_DOOR,  STAR},
			{STAR, CAVE_FLOOR,  STAR},
			{STAR, STAR, STAR}},
			CAVE_CLOSE_DOOR),
		}, allTiles, non_room_tiles, null);

	    this.cave_corners = new PgTileFilter(
		new Rule[] {
		    new Rule(new byte[3,3] {
			{CAVE_ROCK, CAVE_ROCK,  CAVE_ROCK},
			{CAVE_ROCK, CAVE_FLOOR, CAVE_FLOOR},
			{CAVE_ROCK, CAVE_FLOOR, CAVE_FLOOR}},
			CAVE_FLOOR),

		    new Rule(new byte[3,3] {
			{CAVE_ROCK,   CAVE_ROCK, CAVE_ROCK},
			{CAVE_FLOOR, CAVE_FLOOR, CAVE_ROCK},
			{CAVE_FLOOR, CAVE_FLOOR, CAVE_ROCK}},
			CAVE_FLOOR),

		    new Rule(new byte[3,3] {
			{CAVE_FLOOR, CAVE_FLOOR, CAVE_ROCK},
			{CAVE_FLOOR, CAVE_FLOOR, CAVE_ROCK},
			{CAVE_ROCK,   CAVE_ROCK, CAVE_ROCK}},
			CAVE_FLOOR),

		    new Rule(new byte[3,3] {
			{CAVE_ROCK, CAVE_FLOOR, CAVE_FLOOR},
			{CAVE_ROCK, CAVE_FLOOR, CAVE_FLOOR},
			{CAVE_ROCK,  CAVE_ROCK, CAVE_ROCK}},
			CAVE_FLOOR),

		}, allTiles, null, null);


	    this.cave_wall_corners = new PgTileFilter(
		new Rule[] {
		    new Rule(new byte[3,3] {
			{STAR,       CAVE_WALL_N,  STAR},
			{CAVE_WALL_W, CAVE_FLOOR, CAVE_FLOOR},
			{STAR,        CAVE_FLOOR, CAVE_FLOOR}},
			CAVE_FLOOR),

		    new Rule(new byte[3,3] {
			{STAR,      CAVE_WALL_N, STAR},
			{CAVE_FLOOR, CAVE_FLOOR, CAVE_WALL_E},
			{CAVE_FLOOR, CAVE_FLOOR, STAR}},
			CAVE_FLOOR),

		    new Rule(new byte[3,3] {
			{CAVE_FLOOR, CAVE_FLOOR, STAR},
			{CAVE_FLOOR, CAVE_FLOOR, CAVE_WALL_E},
			{STAR,   CAVE_WALL_N, STAR}},
			CAVE_FLOOR),

		    new Rule(new byte[3,3] {
			{STAR,        CAVE_FLOOR, CAVE_FLOOR},
			{CAVE_WALL_E, CAVE_FLOOR, CAVE_FLOOR},
			{STAR,  CAVE_WALL_N, STAR}},
			CAVE_FLOOR),

		}, allTiles, null, null);

	    this.room_tops = new PgTileFilter(
		new Rule[] {
		    new Rule(new byte[3,3] {
			{CAVE_ROOM_N, CAVE_ROOM_N,  CAVE_ROOM_N},
			{CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR},
			{CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR}},
			CAVE_ROOM_FLOOR),
		}, allTiles, null, null);

	    this.cave_edges = new PgTileFilter(
		new Rule[] {
		    new Rule(new byte[3,3] {
			{CAVE_FLOOR, CAVE_FLOOR,  CAVE_FLOOR},
			{CAVE_BLANK, CAVE_BLANK,  CAVE_BLANK},
			{CAVE_BLANK, CAVE_BLANK,  CAVE_BLANK}},
			CAVE_BLANK),

		    new Rule(new byte[3,3] {
			{CAVE_BLANK, CAVE_BLANK,  CAVE_FLOOR},
			{CAVE_BLANK, CAVE_BLANK,  CAVE_FLOOR},
			{CAVE_BLANK, CAVE_BLANK,  CAVE_FLOOR}},
			CAVE_BLANK),

		    new Rule(new byte[3,3] {
			{CAVE_BLANK, CAVE_BLANK,  CAVE_BLANK},
			{CAVE_BLANK, CAVE_BLANK,  CAVE_BLANK},
			{CAVE_FLOOR, CAVE_FLOOR,  CAVE_FLOOR}},
			CAVE_BLANK),

		    new Rule(new byte[3,3] {
			{CAVE_FLOOR, CAVE_BLANK,  CAVE_BLANK},
			{CAVE_FLOOR, CAVE_BLANK,  CAVE_BLANK},
			{CAVE_FLOOR, CAVE_BLANK,  CAVE_BLANK}},
			CAVE_BLANK),

		}, allTiles, null, null);

	    this.earth_cave_walls = new PgTileFilter(
		new Rule[] {
		    // NW and NE corners
		    new Rule(new byte[3,3] {
			{STAR, STAR, STAR},
			{STAR,    CAVE_BLANK, _},
			{      _, CAVE_BLANK, CAVE_FLOOR}},
			CAVE_WALL_NW),

		    new Rule(new byte[3,3] {
			{STAR, STAR, STAR},
			{_, CAVE_BLANK, STAR},
			{CAVE_FLOOR, CAVE_BLANK, _}},
			CAVE_WALL_NE),

		    // W and E sides
		    new Rule(new byte[3,3] {
			{STAR,       _, STAR},
			{_, CAVE_BLANK, CAVE_FLOOR},
			{STAR,       _, STAR}},
			CAVE_WALL_W),

		    new Rule(new byte[3,3] {
			{STAR,       _, STAR},
			{CAVE_FLOOR, CAVE_BLANK, _},
			{STAR,       _, STAR}},
			CAVE_WALL_E),

		    // Top and bottom walls
		    new Rule(new byte[3,3] {
			{STAR, STAR, STAR},
			{_, CAVE_BLANK, _},
			{STAR, CAVE_FLOOR, STAR}},
			CAVE_WALL_N),

		    new Rule(new byte[3,3] {
			{STAR, CAVE_FLOOR, STAR},
			{CAVE_BLANK, CAVE_BLANK, CAVE_BLANK},
			{STAR, STAR, STAR}},
			CAVE_WALL_N),

		    // SW and SE corners
		    new Rule(new byte[3,3] {
			{CAVE_FLOOR, CAVE_BLANK, STAR},
			{CAVE_BLANK, CAVE_BLANK, STAR},
			{STAR, STAR, STAR}},
			CAVE_WALL_N),

		    new Rule(new byte[3,3] {
			{STAR, CAVE_BLANK, CAVE_FLOOR},
			{STAR, CAVE_BLANK, CAVE_BLANK},
			{STAR, STAR, STAR}},
			CAVE_WALL_N),

		    // End caps
		    new Rule(new byte[3,3] {
			{STAR,       CAVE_FLOOR, CAVE_FLOOR},
			{CAVE_BLANK, CAVE_BLANK, CAVE_FLOOR},
			{STAR,       CAVE_FLOOR, CAVE_FLOOR}},
			CAVE_WALL_N),

		    new Rule(new byte[3,3] {
			{CAVE_FLOOR, CAVE_FLOOR, STAR},
			{CAVE_FLOOR, CAVE_BLANK, CAVE_BLANK},
			{CAVE_FLOOR, CAVE_FLOOR, STAR}},
			CAVE_WALL_N),

		}, allTiles, non_floor_tiles, null);

	    this.earth_cave_extend_walls = new PgTileFilter(
		new Rule[] {

		    new Rule(new byte[3,3] {
			{STAR, STAR, STAR},
			{STAR, CAVE_BLANK,  STAR},
			{STAR, CAVE_WALL_W, STAR}},
			CAVE_WALL_W),

		    new Rule(new byte[3,3] {
			{STAR, STAR, STAR},
			{STAR, CAVE_BLANK,  STAR},
			{STAR, CAVE_WALL_E, STAR}},
			CAVE_WALL_E),

		    new Rule(new byte[3,3] {
			{STAR, CAVE_WALL_W, STAR},
			{STAR, CAVE_BLANK,  STAR},
			{STAR, STAR, STAR}},
			CAVE_WALL_W),

		    new Rule(new byte[3,3] {
			{STAR, CAVE_WALL_E, STAR},
			{STAR, CAVE_BLANK,  STAR},
			{STAR, STAR, STAR}},
			CAVE_WALL_E),

		    new Rule(new byte[3,3] {
			{STAR, CAVE_WALL_NW, STAR},
			{STAR, CAVE_BLANK,  STAR},
			{STAR, STAR, STAR}},
			CAVE_WALL_W),

		    new Rule(new byte[3,3] {
			{STAR, CAVE_WALL_NE, STAR},
			{STAR, CAVE_BLANK,  STAR},
			{STAR, STAR, STAR}},
			CAVE_WALL_E),

		}, allTiles, null, null);

	    this.earth_cave_walls2 = new PgTileFilter(
		new Rule[] {

		    // Hop gaps with the correct side wall
		    new Rule(new byte[3,3] {
			{STAR, CAVE_WALL_W, STAR},
			{STAR, CAVE_FLOOR,  STAR},
			{CAVE_FLOOR, CAVE_BLANK, CAVE_FLOOR}},
			CAVE_WALL_W, 0, 1),

		    new Rule(new byte[3,3] {
			{CAVE_FLOOR, CAVE_BLANK, CAVE_FLOOR},
			{STAR, CAVE_FLOOR,  STAR},
			{STAR, CAVE_WALL_W, STAR}},
			CAVE_WALL_W, 0, -1),

		    new Rule(new byte[3,3] {
			{STAR, CAVE_WALL_E, STAR},
			{STAR, CAVE_FLOOR,  STAR},
			{CAVE_FLOOR, CAVE_BLANK, CAVE_FLOOR}},
			CAVE_WALL_E, 0, 1),

		    new Rule(new byte[3,3] {
			{CAVE_FLOOR, CAVE_BLANK, CAVE_FLOOR},
			{STAR, CAVE_WALL_E, STAR},
			{STAR, CAVE_FLOOR,  STAR}},
			CAVE_WALL_E, 0, -1),

		    // Lower corner fixup
		    new Rule(new byte[3,3] {
			{STAR,       CAVE_FLOOR, CAVE_FLOOR},
			{CAVE_WALL_N, CAVE_BLANK, CAVE_FLOOR},
			{STAR,       CAVE_WALL_N, STAR}},
			CAVE_WALL_W),

		    new Rule(new byte[3,3] {
			{CAVE_FLOOR, CAVE_FLOOR, STAR},
			{CAVE_FLOOR, CAVE_BLANK, CAVE_WALL_N},
			{STAR,       CAVE_WALL_N, STAR}},
			CAVE_WALL_E),

		    // lower horizonal gap fix for on E wall
		    new Rule(new byte[3,3] {
			{STAR, STAR, STAR},
			{_, CAVE_FLOOR, CAVE_WALL_E},
			{STAR, CAVE_FLOOR, CAVE_FLOOR}},
			CAVE_WALL_N, 1, 0),

		    // lower horizonal gap fix on W wall
		    new Rule(new byte[3,3] {
			{STAR, STAR, STAR},
			{CAVE_WALL_W, CAVE_FLOOR,  _},
			{CAVE_FLOOR,  CAVE_FLOOR, STAR}},
			CAVE_WALL_N, -1, 0),

		    // upper horizonal gap fix for on E wall
		    new Rule(new byte[3,3] {
			{STAR,  CAVE_FLOOR, CAVE_FLOOR},
			{_,     CAVE_FLOOR, CAVE_WALL_E},
			{STAR,       STAR,  CAVE_WALL_E}},
			CAVE_WALL_NE, 1, 0),

		    // upper horizonal gap fix on W wall
		    new Rule(new byte[3,3] {
			{CAVE_FLOOR,  CAVE_FLOOR, STAR},
			{CAVE_WALL_W, CAVE_FLOOR,  _},
			{CAVE_WALL_W, STAR, STAR}},
			CAVE_WALL_NW, -1, 0),

		}, allTiles, non_floor_tiles, null);


	    this.earth_cave_walls3 = new PgTileFilter(
		new Rule[] {

		    new Rule(new byte[3,3] {
			{STAR,          CAVE_FLOOR, STAR},
			{CAVE_WALL_N,  CAVE_WALL_E,  CAVE_FLOOR},
			{STAR, CAVE_WALL_E, STAR}},
			CAVE_WALL_NE),

		    new Rule(new byte[3,3] {
			{STAR,          CAVE_FLOOR, STAR},
			{CAVE_FLOOR,  CAVE_WALL_W,  CAVE_WALL_N},
			{STAR, CAVE_WALL_W, STAR}},
			CAVE_WALL_NW),

		    new Rule(new byte[3,3] {
			{STAR,        CAVE_FLOOR, STAR},
			{CAVE_FLOOR,  CAVE_BLANK, CAVE_WALL_N},
			{STAR,        CAVE_BLANK, STAR}},
			CAVE_WALL_NW),

		    new Rule(new byte[3,3] {
			{STAR,        CAVE_FLOOR, STAR},
			{CAVE_FLOOR,  CAVE_BLANK, CAVE_BLANK},
			{STAR,        CAVE_BLANK, STAR}},
			CAVE_WALL_NW),

		    new Rule(new byte[3,3] {
			{STAR,         CAVE_FLOOR, STAR},
			{CAVE_WALL_N,  CAVE_BLANK, CAVE_FLOOR},
			{STAR,         CAVE_BLANK, STAR}},
			CAVE_WALL_NE),

		    new Rule(new byte[3,3] {
			{STAR,         CAVE_FLOOR, STAR},
			{CAVE_BLANK,   CAVE_BLANK, CAVE_FLOOR},
			{STAR,         CAVE_BLANK, STAR}},
			CAVE_WALL_NE),

		    new Rule(new byte[3,3] {
			{STAR,         CAVE_FLOOR, STAR},
			{CAVE_BLANK,   CAVE_BLANK, CAVE_FLOOR},
			{STAR,         CAVE_BLANK, STAR}},
			CAVE_WALL_NE),


		    new Rule(new byte[3,3] {
			{STAR,                STAR, STAR},
			{CAVE_FLOOR,   CAVE_WALL_E, CAVE_WALL_N},
			{CAVE_FLOOR,   CAVE_FLOOR,  STAR}},
			CAVE_WALL_N),


		}, allTiles, null, null);

	    this.earth_cave_walls4 = new PgTileFilter(
		new Rule[] {

		    // junction
		    new Rule(new byte[3,3] {
			{STAR,       CAVE_WALL_E, STAR},
			{CAVE_FLOOR, CAVE_FLOOR, CAVE_FLOOR},
			{STAR,       CAVE_WALL_N, STAR}},
			CAVE_WALL_N, 0, -1),

		    // junction
		    new Rule(new byte[3,3] {
			{STAR,       CAVE_WALL_W, STAR},
			{CAVE_FLOOR, CAVE_FLOOR, CAVE_FLOOR},
			{STAR,       CAVE_WALL_N, STAR}},
			CAVE_WALL_N, 0, -1),

		    // junction
		    new Rule(new byte[3,3] {
			{STAR, STAR, STAR},
			{CAVE_FLOOR, CAVE_WALL_E, CAVE_WALL_NE},
			{STAR, STAR, STAR}},
			CAVE_WALL_N),

		    // junction
		    new Rule(new byte[3,3] {
			{STAR, STAR, STAR},
			{CAVE_WALL_NW, CAVE_WALL_W, CAVE_FLOOR},
			{STAR, STAR, STAR}},
			CAVE_WALL_N),

		    // junction
		    new Rule(new byte[3,3] {
			{STAR,      CAVE_WALL_N, STAR},
			{CAVE_FLOOR, CAVE_FLOOR, CAVE_FLOOR},
			{STAR,      CAVE_WALL_W, STAR}},
			CAVE_WALL_NW, 0, -1),

		    // junction
		    new Rule(new byte[3,3] {
			{STAR,       CAVE_WALL_N, STAR},
			{CAVE_FLOOR, CAVE_FLOOR, CAVE_FLOOR},
			{STAR,       CAVE_WALL_E, STAR}},
			CAVE_WALL_NE, 0, -1),

		    // doesn't look good.
		    /*new Rule(new byte[3,3] {
			{STAR, STAR, STAR},
			{CAVE_FLOOR, CAVE_WALL_E, CAVE_WALL_N},
			{STAR, CAVE_FLOOR, STAR}},
			CAVE_WALL_N),*/

		    new Rule(new byte[3,3] {
			{STAR,         STAR, STAR},
			{CAVE_FLOOR,  CAVE_BLANK,  CAVE_FLOOR},
			{STAR, STAR, STAR}},
			CAVE_WALL_E),

		    // mid wall gap fix on W wall
		    new Rule(new byte[3,3] {
			{CAVE_WALL_W, CAVE_FLOOR, STAR},
			{CAVE_WALL_W, CAVE_FLOOR, CAVE_WALL_NE},
			{CAVE_WALL_W, CAVE_FLOOR, STAR}},
			CAVE_WALL_NW, -1, 0),

		    // mid wall gap fix on W wall
		    new Rule(new byte[3,3] {
			{CAVE_WALL_W, CAVE_FLOOR, STAR},
			{CAVE_WALL_W, CAVE_FLOOR, CAVE_WALL_N},
			{CAVE_WALL_W, CAVE_FLOOR, STAR}},
			CAVE_WALL_NW, -1, 0),

		    // mid wall gap fix on E wall
		    new Rule(new byte[3,3] {
			{STAR,         CAVE_FLOOR, CAVE_WALL_E},
			{CAVE_WALL_NW, CAVE_FLOOR, CAVE_WALL_E},
			{STAR,         CAVE_FLOOR, CAVE_WALL_E}},
			CAVE_WALL_NE, 1, 0),

		    // mid wall gap fix on E wall
		    new Rule(new byte[3,3] {
			{STAR,         CAVE_FLOOR, CAVE_WALL_E},
			{CAVE_WALL_N,  CAVE_FLOOR, CAVE_WALL_E},
			{STAR,         CAVE_FLOOR, CAVE_WALL_E}},
			CAVE_WALL_NE, 1, 0),

		    // fill in absolutely any other remaining gaps.
		    new Rule(new byte[3,3] {
			{STAR, CAVE_FLOOR, STAR},
			{STAR, CAVE_BLANK, STAR},
			{STAR,       STAR, STAR}},
			CAVE_WALL_N),

		    new Rule(new byte[3,3] {
			{STAR,       STAR,       STAR},
			{STAR, CAVE_BLANK, CAVE_FLOOR},
			{STAR,       STAR,      STAR}},
			CAVE_WALL_N),

		    new Rule(new byte[3,3] {
			{STAR,       STAR, STAR},
			{STAR, CAVE_BLANK, STAR},
			{STAR, CAVE_FLOOR, STAR}},
			CAVE_WALL_N),

		    new Rule(new byte[3,3] {
			{STAR,             STAR, STAR},
			{CAVE_FLOOR, CAVE_BLANK, STAR},
			{STAR,             STAR, STAR}},
			CAVE_WALL_N),

		}, allTiles, null, null);


	    this.earth_cave_walls5 = new PgTileFilter(
		new Rule[] {

		    // mismatched vertical walls
		    new Rule(new byte[3,3] {
			{STAR, CAVE_WALL_E, STAR},
			{STAR, CAVE_WALL_W, STAR},
			{STAR, CAVE_WALL_N, STAR}},
			CAVE_WALL_E),

		    // mismatched vertical walls
		    new Rule(new byte[3,3] {
			{STAR, CAVE_WALL_W, STAR},
			{STAR, CAVE_WALL_E, STAR},
			{STAR, CAVE_WALL_N, STAR}},
			CAVE_WALL_W),

		    // N wall gap
		    new Rule(new byte[3,3] {
			{STAR,              STAR,         STAR},
			{CAVE_WALL_N, CAVE_BLANK,  CAVE_WALL_N},
			{STAR,        CAVE_WALL_E, STAR}},
			CAVE_WALL_NE),

		    // N wall gap
		    new Rule(new byte[3,3] {
			{STAR,              STAR,         STAR},
			{CAVE_WALL_N, CAVE_BLANK,  CAVE_WALL_N},
			{STAR,        CAVE_WALL_W, STAR}},
			CAVE_WALL_NW),
		}, allTiles, null, null);
	}
    }
}
