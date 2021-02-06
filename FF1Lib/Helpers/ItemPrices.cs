using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class ItemPrices : MemTable<ushort, Item>
	{
		public ItemPrices(FF1Rom _rom) : base(_rom, 0x37C00, 240)
		{
		}
	}
}
