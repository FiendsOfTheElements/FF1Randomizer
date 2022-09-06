using FF1Lib.Sanity;

namespace FF1Lib.Procgen
{
	public class DungeonTiles {

        public const byte ROOF_TOP = 0x17;
        public const byte ROOF_MIDDLE = 0x18;
        public const byte BIG_WINDOW = 0x1C;
        public const byte SMALL_WINDOWS = 0x1D;
        public const byte BLACK_MAGIC_SIGN = 0x24;
	public const byte INN_SIGN = 0x25;
	public const byte None = 0xFF;

	public static PgFeature BLACK_MAGIC_SHOP = new PgFeature(new byte[,] {
	    {ROOF_TOP, ROOF_TOP, ROOF_TOP},
	    {ROOF_MIDDLE, BLACK_MAGIC_SIGN, ROOF_MIDDLE},
	    {BIG_WINDOW, None, BIG_WINDOW},
	    }, new Dictionary<string, SCCoords> {
	    });

	}
}
