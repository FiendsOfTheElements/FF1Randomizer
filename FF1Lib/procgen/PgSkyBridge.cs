using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public enum ProcgenSkyBridgeMode
	{
		Off = 0,
		Short = 2,
		Medium = 5,
		Long = 9
	}

	public class PgSkyBridge
	{
		private static byte[,] SKYBRIDGE = {
		{0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 57, 57, },
		{3, 73, 73, 73, 73, 73, 73, 10, 73, 73, 73, 73, 73, 73, 5, 57, 57, },
		{3, 73, 73, 73, 73, 73, 12, 13, 14, 73, 73, 73, 73, 73, 5, 57, 57, },
		{3, 73, 73, 73, 73, 73, 73, 11, 73, 73, 73, 73, 73, 73, 5, 57, 57, },
		{3, 73, 73, 73, 73, 73, 73, 11, 73, 73, 73, 73, 73, 73, 5, 57, 57, },
		{3, 73, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 73, 5, 57, 57, },
		{3, 73, 73, 73, 73, 73, 24, 11, 24, 73, 73, 73, 73, 73, 5, 57, 57, },
		{3, 73, 73, 73, 73, 73, 24, 11, 24, 73, 73, 73, 73, 73, 5, 57, 57, },
		{3, 73, 73, 73, 73, 73, 24, 11, 24, 73, 73, 73, 73, 73, 5, 57, 57, },
		{3, 73, 73, 73, 73, 73, 24, 11, 24, 73, 73, 73, 73, 73, 5, 57, 57, },
		{3, 73, 73, 73, 73, 73, 24, 11, 24, 73, 73, 73, 73, 73, 5, 57, 57, },
		{6, 7, 7, 7, 7, 7, 8, 11, 6, 7, 7, 7, 7, 7, 8, 57, 57, },
		{48, 48, 48, 48, 48, 48, 52, 54, 53, 48, 48, 48, 48, 48, 48, 57, 57, },
		{57, 57, 57, 57, 57, 57, 50, 58, 51, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 50, 75, 51, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 48, 75, 48, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 75, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 75, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 52, 75, 53, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 50, 69, 51, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 48, 48, 48, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		{57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, },
		};

		private static byte BRIDGE_TILE = 75;

		//writes map to console in array initializer syntax
		public static string DumpMap(Map m)
		{
			var sb = new StringBuilder();
			sb.Append('{');
			for (int i = 0; i < Map.RowCount; i++)
			{
				sb.Append('{');
				for (int j = 0; j < Map.RowLength; j++)
				{
					sb.Append(m[i, j].ToString() + ", ");
				}
				sb.Append("},\n");
			}
			sb.Append('}');
			return sb.ToString();
		}
		private static List<int> RangeHelper(int start, int end)
		{
			if (start > end)
			{
				return Enumerable.Range(end, start - end).ToList();
			}
			else
			{
				return Enumerable.Range(start, end - start).Reverse().ToList();
			}
		}
		public static void Generate(Map m, MT19337 rng, ProcgenSkyBridgeMode mode)
		{
			const int MAP_WIDTH = 64;
			const int VERTICAL_START = 16;
			const int VERTICAL_END = 52;
			const int HORIZONTAL_START = 7;

			//replace map with template
			int imax = SKYBRIDGE.GetLength(0);
			int jmax = SKYBRIDGE.GetLength(1);
			for (int i = 0; i < imax-1; i++)
			{
				for (int j = 0; j < jmax-1; j++)
				{
					m[i, j] = SKYBRIDGE[i, j];
				}
			}
			
			//generate a number of points to draw between
			List<int> vert_points = new List<int>();
			List<int> horiz_points = new List<int>();

			var heights_range = Enumerable.Range(VERTICAL_START + 1, (VERTICAL_END - VERTICAL_START) - 2).ToList();
			var key_points_range = Enumerable.Range(0, MAP_WIDTH).ToList();

			heights_range = heights_range.Where((item, index) => index % 2 == 0).ToList();
			heights_range.Shuffle(rng);
			key_points_range.Shuffle(rng);

			for (int i = 0; i < (int)mode; i++)
			{
				vert_points.Add(heights_range[i]);
				horiz_points.Add(key_points_range[i]);
			}
			vert_points.Sort();

			//end where we're supposed to
			vert_points.Add(VERTICAL_END);
			horiz_points.Add(HORIZONTAL_START);
			
			int cur_v = VERTICAL_START;
			int cur_h = HORIZONTAL_START;
			
			List<(int,int)> path = new List<(int,int)> ();

			for (int j = 0; j < horiz_points.Count; j++)
			{
				//travel horizontally to next point
				foreach (int i in RangeHelper(horiz_points[j], cur_h))
				{
					path.Add((i, cur_v));
					cur_h = i;
				}
				//travel vertically to next height
				foreach (int i in RangeHelper(cur_v, vert_points[j]).Reverse<int>())
				{
					path.Add((cur_h, i));
					cur_v = i;
				}
			}
			foreach (int i in RangeHelper(cur_h, HORIZONTAL_START+1))
			{
				path.Add((i, cur_v));
			}

			//finally, create the path on the map
			foreach (var p in path)
			{
				m[p.Item2, p.Item1] = BRIDGE_TILE;
			}

		}
	}
}
