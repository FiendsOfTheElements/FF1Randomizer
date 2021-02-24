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
			for (int i = 0; i < Height; i++)
			{
				for (int j = 0; j < Width; j++)
				{
					TileData[i, j] = FillValue;
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
			for (int i = 0; i < Height; i++)
			{
				for (int j = 0; j < Width; j++)
				{
					TileData[i, j] = (byte)FillValue;
				}
			}
			NPCs = null;
		}

		public Region(int x, int y, Region clone)
		{
			this.x = x;
			this.y = y;
			Width = clone.Width;
			Height = clone.Height;
			TileData = new byte[Height, Width];
			for (int i = 0; i < Height; i++)
			{
				for (int j = 0; j < Width; j++)
				{
					TileData[i, j] = clone.TileData[i, j];
				}
			}
			NPCs = null;
		}

		public Region(int x, int y, RoomSpec room)
		{
			this.x = x;
			this.y = y;
			Width = room.Tiledata.GetLength(1);
			Height = room.Tiledata.GetLength(0);
			TileData = room.Tiledata;
			NPCs = room.NPCs;
		}

		public bool Intersects(Region otherRegion)
		{
			bool x1 = (otherRegion.x - x + 64) % 64 < Width;
			bool x2 = (x - otherRegion.x + 64) % 64 < otherRegion.Width;
			bool y1 = (otherRegion.y - y + 64) % 64 < Height;
			bool y2 = (y - otherRegion.y + 64) % 64 < otherRegion.Height;
			return (x1 || x2) && (y1 || y2);
		}

		public bool Borders(Region otherRegion)
		{
			bool xInter = (otherRegion.x - x + 64) % 64 < Width || (x - otherRegion.x + 64) % 64 < otherRegion.Width;
			bool yEqual = (otherRegion.y - y + 64) % 64 == Height || (y - otherRegion.y + 64) % 64 == otherRegion.Height;


			bool yInter = (otherRegion.y - y + 64) % 64 < Height || (y - otherRegion.y + 64) % 64 < otherRegion.Height;
			bool xEqual = (otherRegion.x - x + 64) % 64 == Width || (x - otherRegion.x + 64) % 64 == otherRegion.Width;

			return (xInter && yEqual) || (yInter && xEqual);
		}

		public bool Adjacent(Region otherRegion)
		{
			return Intersects(otherRegion) || Borders(otherRegion);
		}

		public bool IntersectsRoom(RoomSpec room, int room_x, int room_y)
		{
			bool x1 = (room_x - x + 64) % 64 < Width;
			bool x2 = (x - room_x + 64) % 64 < room.Width;
			bool y1 = (room_y - y + 64) % 64 < Height;
			bool y2 = (y - room_y + 64) % 64 < room.Height;
			return (x1 || x2) && (y1 || y2);
		}

		public List<Region> GetAdjacents(List<Region> regionList)
		{
			List<Region> outs = new();
			foreach (Region r in regionList)
			{
				if (Adjacent(r))
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
