using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class PerlinNoise
	{
		// Based on Andrew Kensler's "Building Up Perlin Noise"
		// http://eastfarthing.com/blog/2015-04-21-noise/

		private int size;
		private int mask;
		private readonly int[] perm;
		private readonly float[] grads_x;
		private readonly float[] grads_y;

		private float f(float t)
		{
			t = Math.Abs(t);
			return t >= 1.0f ? 0.0f : 1.0f - (3.0f - 2.0f * t) * t * t;
		}
		private float surflet(float x, float y, float grad_x, float grad_y)
		{
			return f(x) * f(y) * (grad_x * x + grad_y * y);
		}
		public PerlinNoise(MT19337 rng, int size = 64)
		{
			this.size = size;
			this.mask = size - 1;
			perm = new int[size];
			grads_x = new float[size];
			grads_y = new float[size];

			for (int index = 0; index < size; ++index)
			{
				int other = (int)(rng.Next() >> 1) % (index + 1);
				if (index > other)
					perm[index] = perm[other];
				perm[other] = index;
				grads_x[index] = (float)Math.Cos(2.0f * Math.PI * index / size);
				grads_y[index] = (float)Math.Sin(2.0f * Math.PI * index / size);
			}
		}

		/// <summary>
		/// Returns a value between -1 and +1
		/// </summary>
		public float GetValue(float x, float y)
		{
			float result = 0.0f;
			int cell_x = (int)Math.Floor(x);
			int cell_y = (int)Math.Floor(y);
			for (int grid_y = cell_y; grid_y <= cell_y + 1; ++grid_y)
				for (int grid_x = cell_x; grid_x <= cell_x + 1; ++grid_x)
				{
					int hash = perm[(perm[grid_x & mask] + grid_y) & mask];
					result += surflet(x - grid_x, y - grid_y,
									   grads_x[hash], grads_y[hash]);
				}
			return result;
		}
		/// <summary>
		/// Returns true if value is above threshold between 0 and 1
		/// </summary>
		public bool GetBool(float x, float y, float threshold)
		{
			float result = this.GetValue(x, y);

			if ((result + 1f) / 2 > threshold)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
