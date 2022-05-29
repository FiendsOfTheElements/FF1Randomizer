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

	[Option("-t")]
	public bool Tiles { get; } = false;

	int OnExecute(IConsole console)
	{
	    var rom = new FF1Rom(RomPath);
	    rom.LoadSharedDataTables();

	    var maps = rom.ReadMaps();

	    int start = Map;
	    int end = Map;

	    if (All) {
		start = 0;
		end = 60;
	    }

	    for (int i = start; i <= end; i++) {
		Image<Rgba32> output;
		string name;

		if (Tiles) {
		    output = rom.ExportMapTiles((FF1Lib.MapId)i, Inside);
		    name = $"dungeontiles{i}.png";
		} else {
		    output = rom.RenderMap(maps, (FF1Lib.MapId)i, Inside);
		    name = $"dungeonmap{i}.png";
		}
		output.Save(name);
		Console.WriteLine($"Wrote {name}");
	    }

	    return 0;
	}
    }

    [Command("relocatechests", Description = "Render dungeon")]
    class RelocateDungeonChests
    {
	[Argument(0, Description = "Final Fantasy Randomized ROM")]
	[FileExists]
	public string RomPath { get; }

	int OnExecute(IConsole console)
	{
	    var rom = new FF1Rom(RomPath);
	    rom.LoadSharedDataTables();

	    var maps = rom.ReadMaps();

	    rom.ShuffleAllChestLocations(maps);

	    int start = 0;
	    int end = 60;

	    for (int i = start; i <= end; i++) {
		Image<Rgba32> output;
		string name;
		output = rom.RenderMap(maps, (MapId)i, true);
		name = $"dungeonmap{i}.png";
		output.Save(name);
		Console.WriteLine($"Wrote {name}");
	    }

	    return 0;
	}
    }
}
