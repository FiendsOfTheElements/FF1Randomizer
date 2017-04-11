using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RomUtilities;

namespace FF1Lib
{
    public partial class FF1Rom : NesRom
    {
	    public const int MapPointerOffset = 0x10000;
	    public const int MapPointerSize = 2;
	    public const int MapCount = 61;
	    public const int MapDataOffset = 0x10080;

	    private struct OrdealsRoom
	    {
		    public byte Entrance;
		    public List<(int, int)> Teleporters;
	    }

		public void ShuffleOrdeals(MT19337 rng)
	    {
		    var maps = ReadMaps();

		    var rooms = new List<OrdealsRoom>
		    {
			    new OrdealsRoom
				{
				    Entrance = 0x4E,
				    Teleporters = new List<(int, int)> { (0x10, 0x0F) }
			    },
			    new OrdealsRoom
				{
				    Entrance = 0x4C,
				    Teleporters = new List<(int, int)> { (0x0F, 0x09) }
			    },
			    new OrdealsRoom
				{
				    Entrance = 0x4D,
				    Teleporters = new List<(int, int)> { (0x08, 0x08), (0x08, 0x0A) }
				},
			    new OrdealsRoom
				{
				    Entrance = 0x4F,
				    Teleporters = new List<(int, int)> { (0x09, 0x0F) }
				},
			    new OrdealsRoom
				{
				    Entrance = 0x50,
				    Teleporters = new List<(int, int)> { (0x04, 0x14), (0x04, 0x16) }
				},
			    new OrdealsRoom
				{
				    Entrance = 0x51,
				    Teleporters = new List<(int, int)> { (0x04, 0x12) }
				},
			    new OrdealsRoom
				{
				    Entrance = 0x52,
				    Teleporters = new List<(int, int)> { (0x01, 0x01), (0x03, 0x02) }
				},
			    new OrdealsRoom
				{
				    Entrance = 0x53,
				    Teleporters = new List<(int, int)> { (0x06, 0x08), (0x14, 0x0B), (0x14, 0x0D), (0x12, 0x10) }
				}
		    };
			
			//byte exit = 0x55;
	    }

	    public List<Map> ReadMaps()
	    {
		    var pointers = Get(MapPointerOffset, MapCount * MapPointerSize).ToUShorts();

			return pointers.Select(pointer => new Map(Get(MapPointerOffset + pointer, Map.RowCount * Map.RowLength))).ToList();
		}

	    public void WriteMaps(List<Map> maps)
	    {
		    var data = maps.Select(map => map.GetCompressedData()).ToList();

		    var pointers = new ushort[MapCount];
		    pointers[0] = MapDataOffset - MapPointerOffset;
		    for (int i = 1; i < MapCount; i++)
		    {
			    pointers[i] = (ushort)(pointers[i - 1] + data[i - 1].Length);
		    }

			Put(MapPointerOffset, Blob.FromUShorts(pointers));
		    for (int i = 0; i < MapCount; i++)
		    {
			    Put(MapPointerOffset + pointers[i], data[i]);
		    }
	    }
	}
}
