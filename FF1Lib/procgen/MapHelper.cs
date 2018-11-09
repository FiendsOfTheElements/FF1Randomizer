using System;
using System.Collections.Generic;
using System.Text;

namespace FF1Lib.Procgen
{
	static class MapHelper
	{
		private static T[,] TransposeFilter<T>(T[,] filter)
		{
			T[,] newFilter = (T[,])filter.Clone();
			for (var i = 0; i < filter.GetLength(0); i++)
			{
				for (var j = 0; j < filter.GetLength(1); j++)
				{
					newFilter[j, i] = filter[i, j];
				}
			}
			return newFilter;
		}

		//Looks for patterns of single smoothTarget tiles and massages them out.
		//Uses a more general Filter function that we set up
		//But the Equality2DArrayComparer is the big trick to get this to work
		//Returns true if changes were made
		public static bool SmoothFilter(Map map, Tile smoothTarget, Tile background)
		{
			byte bgTile = (byte)background;
			byte target = (byte)smoothTarget;

			byte[,] pattern1 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, target, bgTile },
				{ bgTile, bgTile, bgTile } };

			byte[,] replacement1 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile } };

			byte[,] pattern2 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, target },
				{ bgTile, bgTile, bgTile } };

			byte[,] replacement2 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile } };

			byte[,] pattern3 = {
				{ bgTile, bgTile, bgTile },
				{ target, bgTile, bgTile },
				{ bgTile, bgTile, bgTile } };

			byte[,] replacement3 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile } };

			byte[,] pattern4 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, target, target },
				{ bgTile, bgTile, bgTile } };

			byte[,] replacement4 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile } };

			byte[,] pattern5 = {
				{ bgTile, bgTile, bgTile },
				{ target, target, bgTile },
				{ bgTile, bgTile, bgTile } };

			byte[,] replacement5 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile } };

			byte[,] pattern6 = {
				{ bgTile, bgTile, bgTile },
				{ target, target, target },
				{ bgTile, bgTile, bgTile } };

			byte[,] replacement6 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile } };

			Dictionary<byte[,], byte[,]> filter = new Dictionary<byte[,], byte[,]>(new Equality2DArrayComparer())
			{
				{ pattern1, replacement1 },
				{ pattern2, replacement2 },
				{ pattern3, replacement3 },
				{ pattern4, replacement4 },
				{ pattern5, replacement5 },
				{ pattern6, replacement6 },
				{ TransposeFilter(pattern2), TransposeFilter(replacement2) },
				{ TransposeFilter(pattern3), TransposeFilter(replacement3) },
				{ TransposeFilter(pattern4), TransposeFilter(replacement4) },
				{ TransposeFilter(pattern5), TransposeFilter(replacement5) },
				{ TransposeFilter(pattern6), TransposeFilter(replacement6) }
			};

			var tetst = filter.ContainsKey(pattern1);
			return map.Filter(filter, (x: 3, y: 3));
		}

		//A class that allows us to actually compare two byte[,] arrays
		class Equality2DArrayComparer : IEqualityComparer<byte[,]>
		{
			public bool Equals(byte[,] x, byte[,] y)
			{
				if (x.Length != y.Length || x.GetLength(0) != y.GetLength(0) || x.GetLength(1) != y.GetLength(1))
				{
					return false;
				}
				for (var i = 0; i < x.GetLength(0); i++)
				{
					for (var j = 0; j < x.GetLength(1); j++)
					{
						if (x[i, j] != y[i, j])
							return false;
					}
				}
				return true;
			}

			public int GetHashCode(byte[,] obj)
			{
				int result = 17;
				for (int i = 0; i < obj.GetLength(0); i++)
				{
					for (int j = 0; j < obj.GetLength(1); j++)
						unchecked
						{
							result = result * 23 + obj[i, j];
						}
				}
				return result;
			}
		}

	}
}
