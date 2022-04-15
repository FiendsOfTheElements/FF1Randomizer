using FF1Lib.Helpers;
using Newtonsoft.Json;

namespace FF1R.Commands
{
	using System;
	using McMaster.Extensions.CommandLineUtils;

	using FF1Lib;
	using FFR.Common;

	[Command("dump", Description = "Dump enemy, weapon, armor and magic tables from ROM")]
	class DumpStats
	{
		[Argument(0, Description = "Final Fantasy Randomized ROM")]
		[FileExists]
		public string RomPath { get; }

		int OnExecute(IConsole console)
		{
		    var rom = new FF1Rom(RomPath);
		    rom.LoadSharedDataTables();
		    var allSpells = rom.GetSpells();
		    Console.WriteLine(JsonConvert.SerializeObject(allSpells, Formatting.Indented));
		    return 0;
		}
	}
}
