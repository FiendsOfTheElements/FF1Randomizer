using FF1Lib.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace FF1R.Commands
{
	using System;
	using McMaster.Extensions.CommandLineUtils;

	using FF1Lib;
	using FFR.Common;
	using System.Linq;

	[Command("importmagic", Description = "Import magic tables into ROM")]
	class ImportMagicStats
	{
		[Argument(0, Description = "Final Fantasy Randomized ROM")]
		[FileExists]
		public string RomPath { get; }

		[Argument(1, Description = "Magic tables json")]
		[FileExists]
		public string MagicJson { get; }

		[Argument(2, Description = "Output filename")]
		public string OutputFilename { get; }

		int OnExecute(IConsole console)
		{
		    var rom = new FF1Rom(RomPath);
		    rom.LoadSharedDataTables();

		    List<MagicSpell> allSpells;
		    allSpells = JsonConvert.DeserializeObject<List<MagicSpell>>(File.ReadAllText(MagicJson));

			EnemyScripts enemyScripts = new(rom);
		    rom.PutSpells(allSpells, enemyScripts);
		    rom.ItemsText.Write(rom, ItemLists.UnusedGoldItems.ToList());
			enemyScripts.Write(rom);

			allSpells = rom.GetSpells();
		    Console.WriteLine(JsonConvert.SerializeObject(allSpells, Formatting.Indented));

			rom.Save(OutputFilename);

		    return 0;

		}
	}
}
