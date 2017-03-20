using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomUtilities;

namespace FF1Lib
{
	public partial class FF1Rom
	{
		public void FixHouse()
		{
			Put(0x03B2CB, Blob.FromHex("20F3ABA91E20E0B2EAEA"));
			Put(0x038816, Blob.FromHex("203B42A4AAACA6FF23A6B23223A7C0059C8A9F8EC5FFFFFFFFFFFFFF"));
		}
	}
}
