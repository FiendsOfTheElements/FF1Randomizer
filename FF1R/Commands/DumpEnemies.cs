using FF1Lib.Helpers;
using Newtonsoft.Json;

namespace FF1R.Commands
{
	using System;
	using McMaster.Extensions.CommandLineUtils;

	using FF1Lib;
	using FFR.Common;

	[Command("dumpenemies", Description = "Dump enemy stat tables from ROM")]
	class DumpEnemyStats
	{
		[Argument(0, Description = "Final Fantasy Randomized ROM")]
		[FileExists]
		public string RomPath { get; }

		int OnExecute(IConsole console)
		{
		    var rom = new FF1Rom(RomPath);
		    rom.LoadSharedDataTables();

		    var allEnemies = rom.GetEnemies();
		    Console.WriteLine(JsonConvert.SerializeObject(allEnemies, Formatting.Indented));

		    return 0;

		}
	}
}
