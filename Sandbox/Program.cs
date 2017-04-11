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
			var rom = new FF1Rom("E:/OldHD/Emu/NES/ROMS/finalfantasy1test.nes");
			var maps = rom.ReadMaps();
			rom.WriteMaps(maps);
			rom.Save("E:/OldHD/Emu/NES/ROMS/finalfantasy1test2.nes");
		}
	}
}
