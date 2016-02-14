using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace ROMUtilities
{
	public class MT19337
	{
		public const int N = 624;
		public const int M = 397;
		public const uint A = 0x9908b0df;

		private readonly uint[] _state = new uint[N];
		private uint _index;

		private readonly uint[] _register = new uint[2]; 

		public MT19337(uint seed)
		{
			_state[0] = seed;
			for (_index = 1; _index < N; _index++)
			{
				_state[_index] = 0x6C078965*(_state[_index - 1] ^ (_state[_index - 1] >> 30)) + _index;
			}

			_register[0] = 0;
			_register[1] = A;
		}

		public uint Next()
		{
			if (_index >= N)
			{
				GenerateMore();
			}

			uint next = _state[_index++];
			next ^= next >> 11;
			next ^= (next << 7) & 0x9d2c5680;
			next ^= (next << 15) & 0xefc60000;
			next ^= next >> 18;

			return next;
		}

		private void GenerateMore()
		{
			uint y;
			for (int i = 0; i < N - M; i++)
			{
				y = (_state[i] & 0x80000000) | (_state[i + 1] & 0x7FFFFFFF);
				_state[i] = _state[i + M] ^ (y >> 1) ^ _register[y & 1];
			}

			for (int i = N - M; i < N - 1; i++)
			{
				y = (_state[i] & 0x80000000) | (_state[i + 1] & 0x7FFFFFFF);
				_state[i] = _state[i + M - N] ^ (y >> 1) ^ _register[y & 1];
			}

			y = (_state[N - 1] & 0x80000000) | (_state[0] & 0x7FFFFFFF);
			_state[N - 1] = _state[M - 1] ^ (y >> 1) ^ _register[y & 1];

			_index = 0;
		}
	}
}
