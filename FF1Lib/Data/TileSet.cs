using System.Runtime.InteropServices;

namespace FF1Lib
{
	[Flags]
	public enum TilePropFunc : byte
	{
		TP_SPEC_DOOR = 0b00000010,
		TP_SPEC_LOCKED = 0b00000100,
		TP_SPEC_CLOSEROOM = 0b00000110,
		TP_SPEC_TREASURE = 0b00001000,
		TP_SPEC_BATTLE = 0b00001010,
		TP_SPEC_DAMAGE = 0b00001100,
		TP_SPEC_CROWN = 0b00001110,
		TP_SPEC_CUBE = 0b00010000,
		TP_SPEC_4ORBS = 0b00010010,
		TP_SPEC_USEROD = 0b00010100,
		TP_SPEC_USELUTE = 0b00010110,

		TP_SPEC_EARTHORB = 0b00011000,
		TP_SPEC_FIREORB = 0b00011010,
		TP_SPEC_WATERORB = 0b00011100,
		TP_SPEC_AIRORB = 0b00011110,

		TP_TELE_EXIT = 0b11000000,
		TP_TELE_NORM = 0b10000000,
		TP_TELE_WARP = 0b01000000,

		OWTP_NORMAL = 0b00000001,
		OWTP_RIVER = 0b00000010,
		OWTP_OCEAN = 0b00000100,
		OWTP_DOCKAIRSHIP = 0b00001000,
		OWTP_FOREST = 0b00010000,
		OWTP_DOCKSHIP = 0b00100000,
		OWTP_SPEC_CHIME = 0b01000000,
		OWTP_SPEC_CARAVAN = 0b10000000,
		OWTP_SPEC_FLOATER = 0b11000000,
		OWTP_SPEC_MASK = 0b11000000,
		OWTP_EXT_MASK = 0b11100000,

		TP_NOMOVE = 0b00000001,
		TP_SPEC_MASK = 0b00011110,
		TP_TELE_MASK = 0b11000000,
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TileProp
	{
		public byte Byte1;
		public byte Byte2;

		public TilePropFunc TilePropFunc
		{
			get { return (TilePropFunc)Byte1; }
			set { Byte1 = (byte)value; }
		}

		public byte ShopId
		{
			get { return Byte2; }
			set { Byte2 = value; }
		}

		public byte BattleId
		{
			get { return Byte2; }
			set { Byte2 = value; }
		}

		public TileProp(byte p1, byte p2)
		{
			Byte1 = p1;
			Byte2 = p2;
		}
	}

	public class TileSet
	{
		public const byte OverworldIndex = 0xFF;

		public MemTable<TileProp> TileProperties;
		public MemTable<byte> TileAttributes;
		public MemTable<byte> TopLeftTiles;
		public MemTable<byte> TopRightTiles;
		public MemTable<byte> BottomLeftTiles;
		public MemTable<byte> BottomRightTiles;
	        public byte Index;
	        public FF1Rom Rom;

		public TileSet(FF1Rom _rom, byte idx)
		{
		        this.Index = idx;
			this.Rom = _rom;
			if (idx == OverworldIndex)
			{
				TileProperties = new MemTable<TileProp>(_rom, 0x0000, 128);

				TopLeftTiles = new MemTable<byte>(_rom, 0x0100, 128);
				TopRightTiles = new MemTable<byte>(_rom, 0x0180, 128);
				BottomLeftTiles = new MemTable<byte>(_rom, 0x0200, 128);
				BottomRightTiles = new MemTable<byte>(_rom, 0x0280, 128);

				TileAttributes = new MemTable<byte>(_rom, 0x0300, 128);
			}
			else
			{
				TileProperties = new MemTable<TileProp>(_rom, 0x800 + 0x100 * idx, 128);
				TileAttributes = new MemTable<byte>(_rom, 0x400 + 0x80 * idx, 128);

				TopLeftTiles = new MemTable<byte>(_rom, 0x1000 + 0x200 * idx, 128);
				TopRightTiles = new MemTable<byte>(_rom, 0x1080 + 0x200 * idx, 128);
				BottomLeftTiles = new MemTable<byte>(_rom, 0x1100 + 0x200 * idx, 128);
				BottomRightTiles = new MemTable<byte>(_rom, 0x1180 + 0x200 * idx, 128);
			}
		}

		public void LoadData()
		{
			TileProperties.LoadTable();
			TileAttributes.LoadTable();
			TopLeftTiles.LoadTable();
			TopRightTiles.LoadTable();
			BottomLeftTiles.LoadTable();
			BottomRightTiles.LoadTable();
		}

		public void StoreData()
		{
			TileProperties.StoreTable();
			TileAttributes.StoreTable();
			TopLeftTiles.StoreTable();
			TopRightTiles.StoreTable();
			BottomLeftTiles.StoreTable();
			BottomRightTiles.StoreTable();
		}
	}
}
