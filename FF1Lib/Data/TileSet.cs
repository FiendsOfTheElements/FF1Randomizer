using System.Runtime.InteropServices;
using static FF1Lib.FF1Rom;

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
	public partial class TileSM
	{
		private byte _attribute;
		private byte _TSAul;
		private byte _TSAur;
		private byte _TSAdl;
		private byte _TSAdr;
		private byte _property1;
		private byte _property2;
		private int _tileSetOrigin;
		private byte _tileSetID;

		const int BANK_SMINFO = 0x00;
		const int lut_TileSMsetAttr = 0x8400; // BANK_SMINFO - must be on $400 byte bound  - 0x80 x8
		const int lut_TileSMsetProp = 0x8800; // BANK_SMINFO - page                        - 0x100 bytes x 8  (2 bytes per)
		const int lut_TileSMsetTSA = 0x9000;  // BANK_SMINFO - page                        - 0x80 bytes x4 x8 => ul, ur, dl, dr
											  //const int lut_SMPalettes = 0xA000;    // BANK_SMINFO - $1000 byte bound            - 0x30 bytes x8?

		public TilePalette Palette
		{
			get { return (TilePalette)_attribute; }
			set { _attribute = (byte)value; }
		}
		public List<byte> TileGraphic
		{
			get { return new List<byte> { _TSAul, _TSAur, _TSAdl, _TSAdr }; }
			set
			{
				_TSAul = value[0];
				_TSAur = value[1];
				_TSAdl = value[2];
				_TSAdr = value[3];
			}
		}
		public byte PropertyType
		{
			get { return _property1; }
			set
			{
				_property1 = value;
			}
		}
		public byte PropertyValue
		{
			get { return _property2; }
			set
			{
				_property2 = value;
			}
		}
		public byte Index
		{
			get { return _tileSetID; }
			set
			{
				_tileSetID = value;
			}
		}
		public int TileSet
		{
			get { return _tileSetOrigin; }
			set
			{
				_tileSetOrigin = value;
			}
		}
		public TileSM(byte id, int tileset, TilePalette palette, List<byte> tilegraphics, byte property1, byte property2)
		{
			_tileSetID = id;
			_tileSetOrigin = tileset;
			_attribute = (byte)palette;
			_property1 = property1;
			_property2 = property2;
			_TSAul = tilegraphics[0];
			_TSAur = tilegraphics[1];
			_TSAdl = tilegraphics[2];
			_TSAdr = tilegraphics[3];
		}
		public TileSM(byte id, int tileset, FF1Rom rom)
		{
			Read(id, tileset, rom);
		}
		public TileProp RawProperties()
		{
			return new TileProp { Byte1 = _property1, Byte2 = _property2 };
		}
		public void Write(FF1Rom rom)
		{
			rom.PutInBank(BANK_SMINFO, lut_TileSMsetAttr + (_tileSetOrigin * 0x80) + _tileSetID, new byte[] { _attribute });
			rom.PutInBank(BANK_SMINFO, lut_TileSMsetProp + (_tileSetOrigin * 0x100) + (_tileSetID * 2), new byte[] { _property1, _property2 });
			rom.PutInBank(BANK_SMINFO, lut_TileSMsetTSA + (_tileSetOrigin * 0x200) + _tileSetID, new byte[] { _TSAul });
			rom.PutInBank(BANK_SMINFO, lut_TileSMsetTSA + (_tileSetOrigin * 0x200) + 0x80 + _tileSetID, new byte[] { _TSAur });
			rom.PutInBank(BANK_SMINFO, lut_TileSMsetTSA + (_tileSetOrigin * 0x200) + 0x100 + _tileSetID, new byte[] { _TSAdl });
			rom.PutInBank(BANK_SMINFO, lut_TileSMsetTSA + (_tileSetOrigin * 0x200) + 0x180 + _tileSetID, new byte[] { _TSAdr });
		}
		public void Read(byte id, int tileset, FF1Rom rom)
		{
			_tileSetID = id;
			_tileSetOrigin = tileset;
			_attribute = rom.GetFromBank(BANK_SMINFO, lut_TileSMsetAttr + (_tileSetOrigin * 0x80) + _tileSetID, 1)[0];
			_property1 = rom.GetFromBank(BANK_SMINFO, lut_TileSMsetProp + (_tileSetOrigin * 0x100) + (_tileSetID * 2), 2)[0];
			_property2 = rom.GetFromBank(BANK_SMINFO, lut_TileSMsetProp + (_tileSetOrigin * 0x100) + (_tileSetID * 2), 2)[1];
			_TSAul = rom.GetFromBank(BANK_SMINFO, lut_TileSMsetTSA + (_tileSetOrigin * 0x200) + _tileSetID, 1)[0];
			_TSAur = rom.GetFromBank(BANK_SMINFO, lut_TileSMsetTSA + (_tileSetOrigin * 0x200) + 0x80 + _tileSetID, 1)[0];
			_TSAdl = rom.GetFromBank(BANK_SMINFO, lut_TileSMsetTSA + (_tileSetOrigin * 0x200) + 0x100 + _tileSetID, 1)[0];
			_TSAdr = rom.GetFromBank(BANK_SMINFO, lut_TileSMsetTSA + (_tileSetOrigin * 0x200) + 0x180 + _tileSetID, 1)[0];
		}
	}

	public class TileSet
	{
		public List<TileSM> Tiles { get; set; }
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
			Tiles = new();
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

			for (int i = 0; i < 128; i++)
			{
				Tiles.Add(new TileSM((byte)i, idx, (TilePalette)TileAttributes[i], new List<byte> { TopLeftTiles[i], TopRightTiles[i], BottomLeftTiles[i], BottomRightTiles[i] }, TileProperties[i].Byte1, TileProperties[i].Byte2));
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
			foreach (var tile in Tiles)
			{
				TileProperties[tile.Index] = tile.RawProperties();
				TileAttributes[tile.Index] = (byte)tile.Palette;
				TopLeftTiles[tile.Index] = tile.TileGraphic[0];
				TopRightTiles[tile.Index] = tile.TileGraphic[1];
				BottomLeftTiles[tile.Index] = tile.TileGraphic[2];
				BottomRightTiles[tile.Index] = tile.TileGraphic[3];
			}

			TileProperties.StoreTable();
			TileAttributes.StoreTable();
			TopLeftTiles.StoreTable();
			TopRightTiles.StoreTable();
			BottomLeftTiles.StoreTable();
			BottomRightTiles.StoreTable();
		}
	}

	public partial class TileSetsData
	{
		private List<TileSet> tileSets;

		public TileSetsData(FF1Rom rom)
		{
			tileSets = new();

			for (int i = 0; i < 8; i++)
			{
				tileSets.Add(new TileSet(rom, (byte)i));
			}
		}
		public TileSet this[int idx]
		{
			get
			{
				return tileSets[idx];
			}
			set
			{
				tileSets[idx] = value;

			}
		}
		public void Write()
		{
			tileSets.ForEach(t => t.StoreData());
		}
	}
}
