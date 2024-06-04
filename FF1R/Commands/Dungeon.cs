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

		var maps = new StandardMaps(rom);

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
		    output = rom.ExportMapTiles((FF1Lib.MapIndex)i, Inside);
		    name = $"dungeontiles{i}.png";
		} else {
		    output = rom.RenderMap(maps.GetMapList(), (FF1Lib.MapIndex)i, Inside);
		    name = $"dungeonmap{i}.png";
		}
		output.Save(name);
		Console.WriteLine($"Wrote {name}");

		name = $"dungeon{i}-npcs.json";
		var npcs = maps[(MapIndex)i].MapObjects.ToList();
		using (StreamWriter file = File.CreateText(name)) {
		    JsonSerializer serializer = new JsonSerializer();
		    serializer.Formatting = Formatting.Indented;
		    serializer.Serialize(file, npcs);
		}
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

	[Option("-s")]
	public int Seed { get; } = 13;

	[Argument(1, Description = "Map to dump")]
	public int Map { get; } = 0;

	[Option("-a")]
	public bool All { get; } = false;

	async Task<int> OnExecute(IConsole console)
	{
	    var rom = new FF1Rom(RomPath);

	    var flags = new Flags();

	    flags.RelocateChests = true;
	    flags.RelocateChestsTrapIndicator = true;
	    flags.IncentivizeMarsh = true;
	    flags.IncentivizeEarth = true;
	    flags.IncentivizeVolcano = true;
	    flags.IncentivizeIceCave = true;
	    flags.IncentivizeOrdeals = true;
	    flags.IncentivizeSeaShrine = true;
	    flags.IncentivizeConeria = true;
	    flags.IncentivizeMarshKeyLocked = true;
	    flags.IncentivizeSkyPalace = true;
	    flags.IncentivizeCardia = true;
	    flags.IncentivizeMarshKeyLocked = true;
	    flags.SpeedHacks = true;

	    await rom.Randomize(new byte[]{(byte)Seed}, flags, new Preferences());

	    int start = Map;
	    int end = Map;

	    if (All) {
		start = 0;
		end = 60;
	    }


	    var maps = rom.Maps.GetMapList();

	    for (int i = start; i <= end; i++) {
		Image<Rgba32> output;
		string name;
		output = rom.RenderMap(maps, (MapIndex)i, true);
		name = $"dungeonmap{i}.png";
		output.Save(name);
		Console.WriteLine($"Wrote {name}");
	    }

	    return 0;
	}
    }

    [Command("createdungeon", Description = "Createdungeon")]
    class CreateDungeon
    {
	[Argument(0, Description = "Final Fantasy 'Improved Vanilla' ROM input")]
	[FileExists]
	public string RomPath { get; }

	[Argument(1, Description = "Dungeon to create")]
	public string Map { get; }

	[Option("-s")]
	public int Seed { get; } = 0;

	[Option("-S")]
	public string SeedFile { get; } = null;

	[Option("-r")]
	public bool DoRender { get; }

	[Option("-p")]
	public int Pack { get; } = 1;

	async Task Progress(string message="", int addMax=0) {
	    await Task.Yield();
	}

	int OnExecute(IConsole console)
	{
	    if (Seed == 0 && this.SeedFile == null) {
		Console.WriteLine("Missing seed");
		return 1;
	    }

	    var rom = new FF1Rom(RomPath);
	    rom.LoadSharedDataTables();

	    var maps = new StandardMaps(rom);

	    FF1Lib.MapIndex mapid = Enum.Parse<MapIndex>(Map);

	    var rng = new MT19337((uint)Seed);

	    var numberOfMapsToGenerate = this.Pack;
	    List<int> seeds = null;
	    if (this.SeedFile != null) {
		seeds = new List<int>();
		foreach (string line in System.IO.File.ReadLines(this.SeedFile))
		{
		    seeds.Add(Int32.Parse(line));
		}
		numberOfMapsToGenerate = seeds.Count;
	    }

	    Parallel.For(0, numberOfMapsToGenerate, i =>
	    {
		int effectiveSeed;
		if (seeds == null) {
		    effectiveSeed = this.Seed + i;
		} else {
		    effectiveSeed = seeds[i];
		}
		var rng = new MT19337((uint)effectiveSeed);

		Image<Rgba32> output;
		string name;

		var mapsCopy = new StandardMaps(rom);
		var replacementMaps = Task.Run<List<FF1Lib.Procgen.CompleteMap>>(async () => await FF1Lib.Procgen.NewDungeon.GenerateNewDungeon(rng, rom, mapid,
																		mapsCopy, this.Progress)).Result;

		foreach (var replacementMap in replacementMaps) {
		    mapsCopy[replacementMap.MapIndex].Map = replacementMap.Map;
		    if (DoRender) {
			output = rom.RenderMap(mapsCopy.GetMapList(), replacementMap.MapIndex, false);
			name = $"{effectiveSeed,8:X8}-dungeonmap{(int)replacementMap.MapIndex}-outside.png";
			output.Save(name);
			Console.WriteLine($"Wrote {name}");

			output = rom.RenderMap(mapsCopy.GetMapList(), replacementMap.MapIndex, true);
			name = $"{effectiveSeed,8:X8}-dungeonmap{(int)replacementMap.MapIndex}-inside.png";
			output.Save(name);
			Console.WriteLine($"Wrote {name}");
		    }
		}

		name = $"{effectiveSeed,8:X8}.json";
		using (StreamWriter file = File.CreateText(name)) {
		    FF1Lib.Procgen.CompleteMap.SaveJson(replacementMaps, file);
		}
		Console.WriteLine($"Wrote {name}");
	    });

	    return 0;
	}
    }
}
