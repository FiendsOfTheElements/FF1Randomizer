using System.Collections.Generic;
using System;

namespace FF1Lib.Procgen
{
	public struct RoomSpec
	{
		public byte[,] Tiledata;
		public IEnumerable<NPC> NPCs; // NPCs to be forced into the room. (Coord is an offset from Tiledata origin)

		public int Width => Tiledata.GetLength(1);
		public int Height => Tiledata.GetLength(0);
	}

	public class Region
	{
		public int x;
		public int y;
		public int Width;
		public int Height;
		public byte[,] TileData;
		public IEnumerable<NPC> NPCs;

		public Region(int x, int y, int Width, int Height, byte FillValue)
		{
			this.x = x;
			this.y = y;
			this.Width = Width;
			this.Height = Height;
			TileData = new byte[Height, Width];
			for (var i = 0; i < Height; i++)
			{
				for (var j = 0; j < Width; j++)
				{
					TileData[i,j] = FillValue;
				}
			}
			NPCs = null;
		}

		public Region(int x, int y, int Width, int Height, Tile FillValue)
		{
			this.x = x;
			this.y = y;
			this.Width = Width;
			this.Height = Height;
			TileData = new byte[Height, Width];
			for (var i = 0; i < Height; i++)
			{
				for (var j = 0; j < Width; j++)
				{
					TileData[i, j] = (byte) FillValue;
				}
			}
			NPCs = null;
		}

		public Region(int x, int y, Region clone)
		{
			this.x = x;
			this.y = y;
			this.Width = clone.Width;
			this.Height = clone.Height;
			TileData = new byte[Height, Width];
			for (var i = 0; i < Height; i++)
			{
				for (var j = 0; j < Width; j++)
				{
					TileData[i, j] = clone.TileData[i,j];
				}
			}
			NPCs = null;
		}

		public Region(int x, int y, RoomSpec room)
		{
			this.x = x;
			this.y = y;
			this.Width = room.Tiledata.GetLength(1);
			this.Height = room.Tiledata.GetLength(0);
			TileData = room.Tiledata;
			NPCs = room.NPCs;
		}

		public bool Intersects(Region otherRegion)
		{
			var x1 = (otherRegion.x - this.x + 64) % 64 < this.Width;
			var x2 = (this.x - otherRegion.x + 64) % 64 < otherRegion.Width;
			var y1 = (otherRegion.y - this.y + 64) % 64 < this.Height;
			var y2 = (this.y - otherRegion.y + 64) % 64 < otherRegion.Height;
			return (x1 || x2) && (y1 || y2);
		}

		public bool Borders(Region otherRegion)
		{
			var xInter = ((otherRegion.x - this.x + 64) % 64 < this.Width || (this.x - otherRegion.x + 64) % 64 < otherRegion.Width);
			var yEqual = ((otherRegion.y - this.y + 64) % 64 == this.Height || (this.y - otherRegion.y + 64) % 64 == otherRegion.Height);


			var yInter = (otherRegion.y - this.y + 64) % 64 < this.Height || (this.y - otherRegion.y + 64) % 64 < otherRegion.Height;
			var xEqual = ((otherRegion.x - this.x + 64) % 64 == this.Width || (this.x - otherRegion.x + 64) % 64 == otherRegion.Width);

			return (xInter && yEqual) || (yInter && xEqual);
		}

		public bool Adjacent(Region otherRegion)
		{
			return this.Intersects(otherRegion) || this.Borders(otherRegion);
		}

		public bool IntersectsRoom(RoomSpec room, int room_x, int room_y)
		{
			var x1 = (room_x - this.x + 64) % 64 < this.Width;
			var x2 = (this.x - room_x + 64) % 64 < room.Width;
			var y1 = (room_y - this.y + 64) % 64 < this.Height;
			var y2 = (this.y - room_y + 64) % 64 < room.Height;
			return (x1 || x2) && (y1 || y2);
		}

		public List<Region> GetAdjacents(List<Region> regionList)
		{
			List<Region> outs = new List<Region>();
			foreach (Region r in regionList)
			{
				if (this.Adjacent(r))
				{
					outs.Add(r);
				}
			}

			return outs;
		}

		public void DrawRegion(CompleteMap complete)
		{
			complete.Map.Put((x, y), TileData);
		}
	}
}
