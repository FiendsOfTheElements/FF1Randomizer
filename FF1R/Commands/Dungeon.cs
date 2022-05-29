using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using RomUtilities;
using System.IO;
using FFR.Common;
using Newtonsoft.Json;
using FF1Lib;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System.Linq;
using System.Threading.Tasks;

namespace FF1R.Commands
{
    [Command("renderdungeon", Description = "Render dungeon")]
    class RenderDungeon
    {
	[Argument(0, Description = "Final Fantasy Randomized ROM")]
	[FileExists]
	public string RomPath { get; }

	[Argument(1, Description = "Map to dump")]
	public int Map { get; } = 0;

	[Option("-a")]
	public bool All { get; } = false;

	[Option("-i")]
	public bool Inside { get; } = false;

	int OnExecute(IConsole console)
	{
	    var rom = new FF1Rom(RomPath);
	    rom.LoadSharedDataTables();

	    var maps = rom.ReadMaps();

	    if (All) {
		for (int i = 0; i < 60; i++) {
		    var output = rom.RenderMap(maps, i, Inside);
		    output.Save($"dungeonmap{i}.png");
		}
	    }
	    else {
		var output = rom.RenderMap(maps, Map, Inside);
		output.Save($"dungeonmap{Map}.png");
	    }
	    return 0;
	}
    }
}
