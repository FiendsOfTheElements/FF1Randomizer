using FF1Lib.Sanity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public enum TeleportType
	{
		Enter,
		Exit,
		Tele
	}

	public class TeleportFixup
	{
	    public TeleportFixup(TeleportType tp, int idx, TeleData to);

		public TeleportType Type { get; set; }

		public int? Index { get; set; }

		public TeleData? From { get; set; }

		public TeleData To { get; set; }

	}
}
