using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FF1Lib;
using RomUtilities;

namespace Sandbox
{
	class Program
	{
		static void Main(string[] args)
		{
			var rom = new FF1Rom("E:/OldHD/Emu/NES/ROMS/finalfantasy1.nes");
			var rng = MT19337.New();
			rom.ShuffleOrdeals(rng);
		}
	}
}
