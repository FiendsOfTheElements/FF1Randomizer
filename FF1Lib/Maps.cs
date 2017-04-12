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

			// Here are all the teleporter rooms except the one you start in.
			var rooms = new List<OrdealsRoom>
		    {
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

		    do
		    {
			    rooms.Shuffle(rng);
		    } while (
				rooms[0].Teleporters.Count == 4 ||
			    rooms[5].Teleporters.Count == 4 ||
			    rooms[6].Teleporters.Count == 4);

			// The room you start in always remains the same, but we need to adjust where it teleports you to.
			rooms.Insert(0, new OrdealsRoom
		    {
			    Entrance = 0x4E,
			    Teleporters = new List<(int, int)> { (0x10, 0x0F) }
		    });

		    byte exit = 0x55;
		    const int OrdealsMapIndex = 25;
		    var map = maps[OrdealsMapIndex];
		    for (int i = 0; i < rooms.Count; i++)
		    {
			    int teleporter = rng.Between(0, rooms[i].Teleporters.Count - 1);
				var (x, y) = rooms[i].Teleporters[teleporter];
				rooms[i].Teleporters.RemoveAt(teleporter);

			    if (i < rooms.Count - 1)
			    {
				    map[y, x] = rooms[i + 1].Entrance;
			    }
			    else
			    {
				    map[y, x] = exit;
			    }
		    }

		    for (int i = 0; i < rooms.Count; i++)
		    {
			    if (rooms[i].Teleporters.Count == 1)
			    {
				    var (x, y) = rooms[i].Teleporters[0];
				    var backDestination = rng.Between(0, i);
					map[y, x] = rooms[backDestination].Entrance;
			    }
				else if (rooms[i].Teleporters.Count == 3)
			    {
				    rooms[i].Teleporters.Shuffle(rng);

					var (x, y) = rooms[i].Teleporters[0];
				    var backDestination = rng.Between(0, i);
					map[y, x] = rooms[backDestination].Entrance;

				    (x, y) = rooms[i].Teleporters[1];
				    var otherBackDestination = backDestination;
				    while (otherBackDestination == backDestination)
				    {
					    otherBackDestination = rng.Between(0, i);
					}
				    map[y, x] = rooms[otherBackDestination].Entrance;

				    (x, y) = rooms[i].Teleporters[2];
				    var forwardDestination = rng.Between(i + 1, rooms.Count - 1);
				    map[y, x] = rooms[forwardDestination].Entrance;
			    }
			}

			WriteMaps(maps);
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
