using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public unsafe struct Domain
	{
		private fixed byte Formations[8];

		public byte this[int i]
		{
			get { return Formations[i]; }
			set { Formations[i] = value; }
		}

		public byte this[byte i]
		{
			get { return Formations[i]; }
			set { Formations[i] = value; }
		}
	}

	public class DomainData : MemTable<Domain>
	{
		public DomainData(FF1Rom _rom) : base(_rom, 0x2C000, 128)
		{
		}

		public void SwapDomains(int from, int to)
		{
			var tmp = Data[to];
			Data[to] = Data[from];
			Data[from] = tmp;
		}
	}
}
