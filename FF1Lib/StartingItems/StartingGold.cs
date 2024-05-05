using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public enum StartingGold
	{
		[Description("0 GP")]
		None,
		[Description("100 GP")]
		Gp100,
		[Description("200 GP")]
		Gp200,
		[Description("400 GP (Vanilla)")]
		Gp400,
		[Description("800 GP")]
		Gp800,
		[Description("2500 GP")]
		Gp2500,
		[Description("9999 GP")]
		Gp9999,
		[Description("65,535 GP")]
		Gp65535,
		[Description("Random (0-800 GP)")]
		RandomLow,
		[Description("Random (0-65,535 GP)")]
		RandomHigh,
	}
	public class StartingGp
	{
		private ushort startingGp;
		private const int StartingGoldOffset = 0x0301C;

		public StartingGp(StartingGold startgold, MT19337 rng)
		{
			List<(StartingGold, ushort)> startingGold = new()
			{
				(StartingGold.None, 0),
				(StartingGold.Gp100, 100),
				(StartingGold.Gp200, 200),
				(StartingGold.Gp400, 400),
				(StartingGold.Gp800, 800),
				(StartingGold.Gp2500, 2500),
				(StartingGold.Gp9999, 9999),
				(StartingGold.Gp65535, 65535),
				(StartingGold.RandomLow, (ushort)rng.Between(0, 800)),
				(StartingGold.RandomHigh, (ushort)rng.Between(0, 65535)),
			};

			startingGp = startingGold[(int)startgold].Item2;
		}
		public void Write(FF1Rom rom)
		{
			rom.Put(StartingGoldOffset, BitConverter.GetBytes(startingGp));
		}
	}
}
