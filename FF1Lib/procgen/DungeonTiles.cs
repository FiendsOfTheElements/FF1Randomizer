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
        public const byte CAVE_ROOM_LOWER_WALL = 0x30;

	public const byte CAVE_EARTH_B1_ENTRANCE = 0x18;
	public const byte CAVE_EARTH_B1_EXIT = 0x24;

	public const byte STAR = 0x80;
	public const byte _ = 0x81;
	public const byte None = 0xFF;

	public static PgFeature BLACK_MAGIC_SHOP = new PgFeature(new byte[,] {
	    {TOWN_ROOF_TOP, TOWN_ROOF_TOP, TOWN_ROOF_TOP},
	    {TOWN_ROOF_MIDDLE, TOWN_BLACK_MAGIC_SIGN, TOWN_ROOF_MIDDLE},
	    {TOWN_BIG_WINDOW, None, TOWN_BIG_WINDOW},
	    }, new Dictionary<string, SCCoords> {
	    });

	public PgTileFilter cave_rock_walls;
	public PgTileFilter cave_room_walls;
	public PgTileFilter cave_room_walls2;
	public PgTileFilter cave_corners;

	public DungeonTiles() {

	    var allTiles = new HashSet<byte>();
	    for (byte i = 0; i < 0x80; i++) {
		allTiles.Add(i);
	    }

	    var non_room_tiles = new HashSet<byte>(allTiles);
	    non_room_tiles.Remove(CAVE_ROOM_FLOOR);
	    non_room_tiles.Remove(CAVE_DOOR);
	    non_room_tiles.Remove(CAVE_ROOM_NW);
	    non_room_tiles.Remove(CAVE_ROOM_N);
	    non_room_tiles.Remove(CAVE_ROOM_NE);
	    non_room_tiles.Remove(CAVE_ROOM_E);
	    non_room_tiles.Remove(CAVE_ROOM_W);
	    non_room_tiles.Remove(CAVE_ROOM_SE);
	    non_room_tiles.Remove(CAVE_ROOM_S);
	    non_room_tiles.Remove(CAVE_ROOM_SW);

	    this.cave_rock_walls = new PgTileFilter(
		new Rule[] {
		    new Rule(new byte[3,3] {
			{STAR, CAVE_FLOOR, STAR},
			{STAR, CAVE_BLANK, STAR},
			{STAR, STAR,       STAR}},
			CAVE_ROCK),

		    new Rule(new byte[3,3] {
			{STAR, STAR, CAVE_FLOOR},
			{STAR, CAVE_BLANK, STAR},
			{STAR, STAR,       STAR}},
			CAVE_ROCK),
		    new Rule(new byte[3,3] {
			{STAR, STAR,       STAR},
			{STAR, CAVE_BLANK, CAVE_FLOOR},
			{STAR, STAR,       STAR}},
			CAVE_ROCK),
		    new Rule(new byte[3,3] {
			{STAR, STAR,       STAR},
			{STAR, CAVE_BLANK, STAR},
			{STAR, STAR,       CAVE_FLOOR}},
			CAVE_ROCK),
		    new Rule(new byte[3,3] {
			{STAR, STAR,       STAR},
			{STAR, CAVE_BLANK, STAR},
			{STAR, CAVE_FLOOR, STAR}},
			CAVE_ROCK),
		    new Rule(new byte[3,3] {
			{STAR, STAR,       STAR},
			{STAR, CAVE_BLANK, STAR},
			{CAVE_FLOOR, STAR, STAR}},
			CAVE_ROCK),
		    new Rule(new byte[3,3] {
			{STAR, STAR,             STAR},
			{CAVE_FLOOR, CAVE_BLANK, STAR},
			{STAR, STAR, STAR}},
			CAVE_ROCK),
		    new Rule(new byte[3,3] {
			{CAVE_FLOOR, STAR,             STAR},
			{STAR, CAVE_BLANK, STAR},
			{STAR, STAR, STAR}},
			CAVE_ROCK),
		}, allTiles, null, null);

	    this.cave_room_walls = new PgTileFilter(
		new Rule[] {
		    new Rule(new byte[3,3] {
			{_,                  _,            STAR},
			{_,    CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR},
			{STAR, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR}},
			CAVE_ROOM_NW),

		    new Rule(new byte[3,3] {
			{STAR,                  _,         _},
			{CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, _},
			{CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, STAR}},
			CAVE_ROOM_NE),

		    new Rule(new byte[3,3] {
			{STAR, CAVE_ROOM_FLOOR, STAR},
			{STAR, CAVE_ROOM_FLOOR, STAR},
			{STAR,               _, STAR}},
			CAVE_ROOM_LOWER_WALL),

		    new Rule(new byte[3,3] {
			{STAR,               _, STAR},
			{STAR, CAVE_ROOM_FLOOR, STAR},
			{STAR, CAVE_ROOM_FLOOR, STAR}},
			CAVE_ROOM_N),

		    new Rule(new byte[3,3] {
			{STAR, STAR, STAR},
			{_, CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR},
			{STAR, STAR, STAR}},
			CAVE_ROOM_W),

		    new Rule(new byte[3,3] {
			{STAR, STAR, STAR},
			{CAVE_ROOM_FLOOR, CAVE_ROOM_FLOOR, _},
			{STAR, STAR, STAR}},
			CAVE_ROOM_E),
		}, allTiles, non_room_tiles, null);

	    this.cave_room_walls2 = new PgTileFilter(
		new Rule[] {
		    new Rule(new byte[3,3] {
			{STAR,            STAR,      STAR},
			{STAR, CAVE_ROOM_W,      STAR},
			{STAR, CAVE_ROOM_LOWER_WALL, STAR}},
			CAVE_ROOM_SW),

		    new Rule(new byte[3,3] {
			{STAR,            STAR,      STAR},
			{STAR, CAVE_ROOM_E,          STAR},
			{STAR, CAVE_ROOM_LOWER_WALL, STAR}},
			CAVE_ROOM_SE),

		    new Rule(new byte[3,3] {
			{STAR,            STAR,      STAR},
			{STAR, CAVE_ROOM_FLOOR,      STAR},
			{STAR, CAVE_ROOM_LOWER_WALL, STAR}},
			CAVE_ROOM_S),

		    new Rule(new byte[3,3] {
			{STAR,            STAR,      STAR},
			{STAR, CAVE_ROOM_FLOOR,      STAR},
			{STAR, CAVE_DOOR, STAR}},
			CAVE_ROOM_S),
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
	}
    }
}
