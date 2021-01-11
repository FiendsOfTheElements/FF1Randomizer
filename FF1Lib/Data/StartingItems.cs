using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class StartingItems : MemTable<byte, Item>
	{
		public StartingItems(FF1Rom _rom) : base(_rom, 0x3020, 28)
		{
		}
	}
}
