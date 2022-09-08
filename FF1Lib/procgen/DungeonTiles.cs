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

        public const byte CAVE_BLANK = 0x1A;
        public const byte CAVE_ROOM_FLOOR = 0x2E;
        public const byte CAVE_FLOOR = 0x41;
        public const byte CAVE_DOOR = 0x36;

	public const byte None = 0xFF;

	public static PgFeature BLACK_MAGIC_SHOP = new PgFeature(new byte[,] {
	    {TOWN_ROOF_TOP, TOWN_ROOF_TOP, TOWN_ROOF_TOP},
	    {TOWN_ROOF_MIDDLE, TOWN_BLACK_MAGIC_SIGN, TOWN_ROOF_MIDDLE},
	    {TOWN_BIG_WINDOW, None, TOWN_BIG_WINDOW},
	    }, new Dictionary<string, SCCoords> {
	    });

	}
}
