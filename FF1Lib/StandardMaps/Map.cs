using System.Collections;
using FF1Lib.Sanity;

namespace FF1Lib
{
	public class Map : IEnumerable<MapElement>
	{
		public const int RowLength = 64;
		public const int RowCount = 64;

		private byte[,] _map;

		public byte this[int y, int x]
		{
			get => _map[(RowCount + y) % RowCount, (RowLength + x) % RowLength];
			set => _map[(RowCount + y) % RowCount, (RowLength + x) % RowLength] = value;
		}

		// The coordinate version of the accessor has the params in the normal order.
		public MapElement this[(int x, int y) coord]
		{
			get => new MapElement(this, coord.x, coord.y, this[coord.y, coord.x]);
			set => this[coord.y, coord.x] = value.Value;
		}

		public Map(byte[] data)
		{
			_map = new byte[RowCount, RowLength];

			int dataOffset = 0;
			int x = 0, y = 0;
			while (y < RowCount)
			{
				byte tile = data[dataOffset++];
				if ((tile & 0x80) != 0)
				{
					tile &= 0x7F;
					int count = data[dataOffset++];
					if (count == 0)
					{
						count = 256;
					}
					for (int j = 0; j < count; j++)
					{
						_map[y, x++] = tile;
						if (x >= RowLength)
						{
							x = 0;
							y++;
						}
					}
				}
				else
				{
					_map[y, x++] = tile;
					if (x >= RowLength)
					{
						x = 0;
						y++;
					}
				}
			}
		}

		public Map(byte fill)
		{
			_map = new byte[RowCount, RowLength];
			for (int y = 0; y < RowCount; ++y)
			{
				for (int x = 0; x < RowLength; ++x)
				{
					_map[y, x] = fill;
				}
			}
		}

		public byte[,] MapBytes {
		    get { return this._map; }
		    set { this._map = value; }
		}

		public void SwapSections(int xs1, int ys1, int xs2, int ys2, int xt, int yt)
		{
			for (int x1 = xs1; x1 <= xs2; x1++)
				for (int y1 = ys1; y1 <= ys2; y1++)
				{
					var x2 = x1 - xs1 + xt;
					var y2 = y1 - ys1 + yt;

					var t = _map[y1, x1];
					_map[y1, x1] = _map[y2, x2];
					_map[y2, x2] = t;
				}
		}
		public void SwapTiles((int x, int y) src, (int x, int y) dest)
		{
			byte temp = _map[dest.y, dest.x];
			_map[dest.y, dest.x] = _map[src.y, src.x];
			_map[src.y, src.x] = temp;
		}
		public void FlipSectionVertical(int xs1, int ys1, int xs2, int ys2)
		{
			for (int x1 = xs1; x1 <= xs2; x1++)
				for (int y1 = ys1; y1 <= ys2; y1++)
				{
					var x2 = x1;
					var y2 = ys2 - (y1 - ys1);

					if (y2 > y1)
					{
						var t = this[y1, x1];
						this[y1, x1] = this[y2, x2];
						this[y2, x2] = t;
					}
				}
		}

		public Map Clone()
		{
			Map map = new Map(0);
			for (int y = 0; y < RowCount; ++y)
			{
				for (int x = 0; x < RowLength; ++x)
				{
					map[y, x] = _map[y, x];
				}
			}
			return map;
		}
		public void CopyFrom(Map map)
		{
			for (int y = 0; y < RowCount; ++y)
			{
				for (int x = 0; x < RowLength; ++x)
				{
					_map[y, x] = map[y, x];
				}
			}
		}
		public void Save(Stream stream)
		{
			var buffer = new byte[4096];
			Buffer.BlockCopy(_map, 0, buffer, 0, buffer.Length);
			stream.Write(buffer);
		}

		public void Load(Stream stream)
		{
			var buffer = new byte[4096];
			int readResult = stream.Read(buffer, 0, buffer.Length);
			Buffer.BlockCopy(buffer, 0, _map, 0, buffer.Length);
		}

		public void Put((int x, int y) coord, Blob[] blobs)
		{
			for (int i = 0; i < blobs.Length; ++i)
			{
				for (int j = 0; j < blobs[i].Length; ++j)
				{
					this[coord.y + i, coord.x + j] = blobs[i][j];
				}
			}
		}

		public void Put((int x, int y) coord, byte[,] rows)
		{
			for (int i = 0; i < rows.GetLength(0); ++i)
			{
				for (int j = 0; j < rows.GetLength(1); ++j)
				{
					this[coord.y + i, coord.x + j] = rows[i, j];
				}
			}
		}

		public void Fill((int x, int y) coord, (int w, int h) size, Tile fill)
		{
			Fill(coord, size, (byte)fill);
		}

		public void FlipHorizontal()
		{
			Map map = this.Clone();
			for (int y = 0; y < RowCount; ++y)
			{
				for (int x = 0; x < RowLength; ++x)
				{
					this[y, x] = map[y, (RowLength - x - 1)];
				}
			}
		}
		public void FlipVertical()
		{
			Map map = this.Clone();
			for (int y = 0; y < RowCount; ++y)
			{
				for (int x = 0; x < RowLength; ++x)
				{
					this[y, x] = map[(RowLength - y - 1), x];
				}
			}
		}
		public void Replace(byte originaltile, byte newtile)
		{
			for (int y = 0; y < RowCount; ++y)
			{
				for (int x = 0; x < RowLength; ++x)
				{
					if (this[y, x] == originaltile)
						this[y, x] = newtile;
				}
			}
		}
		public void Fill((int x, int y) coord, (int w, int h) size, byte fill)
		{
		    int x = coord.x;
		    int y = coord.y;
		    int w = size.w;
		    int h = size.h;
		    if (h < 0) { y += h; h = -h; }
		    if (w < 0) { x += w; w = -w; }

			for (int i = x; i < x + w; ++ i)
			{
				for (int j = y; j < y + h; ++j)
				{
					this[j, i] = fill;
				}
			}
		}

		public void Flood((int x, int y) coord, Func<MapElement, bool> action)
		{
			List<(int x, int y)> coords = new List<(int x, int y)> { coord };
			for (int i = 0; i < coords.Count(); ++i)
			{
				(int x, int y) = coords[i];

				// Recurse if our callback returns true.
				if (action(new MapElement(this, x, y, _map[y, x])))
				{
					if (!coords.Contains(((RowLength + x - 1) % RowLength, y))) { coords.Add(((RowLength + x - 1) % RowLength, y)); }
					if (!coords.Contains(((RowLength + x + 1) % RowLength, y))) { coords.Add(((RowLength + x + 1) % RowLength, y)); }
					if (!coords.Contains((x, (RowCount + y - 1) % RowCount))) { coords.Add((x, (RowCount + y - 1) % RowCount)); }
					if (!coords.Contains((x, (RowCount + y + 1) % RowCount))) { coords.Add((x, (RowCount + y + 1) % RowCount)); }
				}
			}
		}

		public byte[,] GetSection((int x, int y) start, (int x, int y) size)
		{
			byte[,] section = new byte[size.x, size.y];
			for (int x = 0; x < size.x; x++)
			{
				for (int y = 0; y < size.y; y++)
				{
					section[x, y] = this[start.y + y, start.x + x];
				}
			}
			return section;
		}



		public bool Filter(Dictionary<byte[,], byte[,]> filter, (int x, int y) filterSize)
		{
			var rVal = false;
			for (int x = 0; x < Map.RowLength - filterSize.x; x++)
			{
				for (int y = 0; y < Map.RowCount - filterSize.y; y++)
				{
					byte[,] section = GetSection((x, y), filterSize);
					if (filter.ContainsKey(section))
					{
						Put((x, y), filter[section]);
						rVal = true;
					}
				}
			}
			return rVal;
		}

		public static byte[,] CreateEmptyRoom((int w, int h) dimensions, int doorX)
		{
			if (dimensions.w < 3 || dimensions.h < 3)
				throw new ArgumentOutOfRangeException();


			byte[,] room = new byte[dimensions.h, dimensions.w];
			for (int y = 1; y < dimensions.h - 2; ++y)
			{
				for (int x = 1; x < dimensions.w - 1; ++x)
				{
					room[y, x] = (byte)Tile.RoomCenter;
				}
			}
			for (int x = 1; x < dimensions.w - 1; ++x)
			{
				room[0, x] = (byte)Tile.RoomBackCenter;
				room[dimensions.h - 2, x] = (byte)Tile.RoomFrontCenter;
				room[dimensions.h - 1, x] = (byte)Tile.InsideWall;
			}
			for (int y = 1; y < dimensions.h - 1; ++y)
			{
				room[y, 0] = (byte)Tile.RoomLeft;
				room[y, dimensions.w - 1] = (byte)Tile.RoomRight;
			}
			room[0, 0] = (byte)Tile.RoomBackLeft;
			room[0, dimensions.w - 1] = (byte)Tile.RoomBackRight;
			room[dimensions.h - 2, 0] = (byte)Tile.RoomFrontLeft;
			room[dimensions.h - 2, dimensions.w - 1] = (byte)Tile.RoomFrontRight;
			room[dimensions.h - 1, 0] = (byte)Tile.InsideWall;
			room[dimensions.h - 1, dimensions.w - 1] = (byte)Tile.InsideWall;

			room[dimensions.h - 1, doorX] = (byte)Tile.Door;

			return room;
		}

		public byte[] GetCompressedData()
		{
			var compressedData = new List<byte>();

			var data = new byte[_map.Length];
			Buffer.BlockCopy(_map, 0, data, 0, _map.Length);

			int dataOffset = 0;
			while (dataOffset < data.Length)
			{
				var tile = data[dataOffset++];
				if (dataOffset >= data.Length || data[dataOffset] != tile)
				{
					compressedData.Add(tile);
					continue;
				}

				byte count = 1;
				while (dataOffset < data.Length && count != 0 && data[dataOffset] == tile)
				{
					count++;
					dataOffset++;
				}

				tile |= 0x80;
				compressedData.Add(tile);
				compressedData.Add(count);
			}

			compressedData.Add(0xFF);

			return compressedData.ToArray();
		}

		//Return a random map element
		public MapElement GetRandomElement(MT19337 rng)
		{
			var newX = rng.Between(0, Map.RowLength - 1);
			var newY = rng.Between(0, Map.RowCount - 1);
			return new MapElement(this, newX, newY, _map[newY, newX]);
		}

		//Return a random element such that its value equals target
		public MapElement GetRandomElement(MT19337 rng, byte target)
		{
			List<MapElement> correspondingTiles = new();

			for (int y = 0; y < RowCount; ++y)
			{
				for (int x = 0; x < RowLength; ++x)
				{
					if (this[y, x] == target)
					{
						correspondingTiles.Add(this[(x,y)]);
					};
				}
			}

			if (correspondingTiles.Any())
			{
				return correspondingTiles.PickRandom(rng);
			}
			else
			{
				return null;
			}
		}

		//Return a random element such that its value equals target's byte value
		public MapElement GetRandomElement(MT19337 rng, Tile target)
		{
			byte tempTarget = (byte)target;
			return GetRandomElement(rng, tempTarget);
		}

		public bool FindFirst(byte tile, out int x, out int y)
		{
			for (y = 0; y < RowCount; y++)
			{
				for (x = 0; x < RowLength; x++)
				{
					if (_map[y, x] == tile) return true;
				}
			}

			x = 0;
			y = 0;

			return false;
		}

		/// <summary>
		/// Enumerates the MapElements in this Map. Writing to a MapElement's Value will set the value in this Map.
		/// </summary>
		public IEnumerator<MapElement> GetEnumerator()
		{
			for (int y = 0; y < RowCount; y++)
			{
				for (int x = 0; x < RowLength; x++)
				{
					yield return new MapElement(this, x, y, _map[y, x]);
				}
			}
		}

		/// <summary>
		/// Enumerates the MapElements in this Map. Writing to a MapElement's Value will set the value in this Map.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public enum Direction : int
	{
		Up = 0,
		Down,
		Left,
		Right,
	};

	/// <summary>
	/// Represents an element of an instantiated Map. Setting Value of a MapElement will set the value in the Map.
	/// </summary>
	public class MapElement
	{
		public readonly Map Map;
		public readonly int X, Y;

		public byte Value
		{
			get { return Map[Y, X]; }
			set { Map[Y, X] = value; }
		}

		public Tile Tile
		{
			get { return (Tile)Map[Y, X]; }
			set { Map[Y, X] = (byte)value; }
		}

		public MapElement(Map map, int x, int y, byte value)
		{
			Map = map;
			X = x;
			Y = y;
		}

		public (int x, int y) Coord => (X, Y);

		public MapElement Neighbor(Direction direction)
		{
			int x = direction == Direction.Left ? ((Map.RowLength + X - 1) % Map.RowLength)
				: direction == Direction.Right ? ((X + 1) % Map.RowLength)
				: X;
			int y = direction == Direction.Up ? ((Map.RowCount + Y - 1) % Map.RowCount)
				: direction == Direction.Down ? ((Y + 1) % Map.RowCount)
				: Y;
			return new MapElement(Map, x, y, Map[y, x]);
		}

		public MapElement Up() { return Neighbor(Direction.Up); }
		public MapElement Down() { return Neighbor(Direction.Down); }
		public MapElement Left() { return Neighbor(Direction.Left); }
		public MapElement Right() { return Neighbor(Direction.Right); }

		/// <summary>
		/// Enumerates the MapElements in this Map. Writing to a MapElement's Value will set the value in this Map.
		/// </summary>
		public IEnumerable<MapElement> Surrounding()
		{
			var up = Up();
			var down = Down();

			return new List<MapElement> { up.Left(), up, up.Right(), Left(), Right(), down.Left(), down, down.Right() };
		}

	    public override bool Equals(Object obj) {
		if ((obj == null) || ! this.GetType().Equals(obj.GetType()))
		{
		    return false;
		}
		var b = (MapElement)obj;
		return (this.Map == b.Map && this.X == b.X && this.Y == b.Y);
	    }

	    public override int GetHashCode()
	    {
		return (X << 2) ^ Y;
	    }
	}
}
