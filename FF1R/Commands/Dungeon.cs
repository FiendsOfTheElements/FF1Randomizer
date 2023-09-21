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

		name = $"dungeon{i}-npcs.json";
		var npcdata = new FF1Rom.NPCdata(rom);
		var npcs = rom.GetNpcs((FF1Lib.MapId)i, npcdata);
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

	int OnExecute(IConsole console)
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

	    rom.Randomize(new byte[]{(byte)Seed}, flags, new Preferences());

	    int start = Map;
	    int end = Map;

	    if (All) {
		start = 0;
		end = 60;
	    }

	    var maps = rom.ReadMaps();

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

    [Command("createdungeon", Description = "Createdungeon")]
    class CreateDungeon
    {
	[Argument(0, Description = "Final Fantasy Randomized ROM")]
	[FileExists]
	public string RomPath { get; }

	[Argument(1, Description = "Map to dump")]
	public int Map { get; } = 0;

	[Option("-a")]
	public bool All { get; } = false;

	[Option("-s")]
	public int Seed { get; } = 0;

	async Task Progress(string message="", int addMax=0) {
	    await Task.Yield();
	}

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

	    var rng = new MT19337((uint)Seed);

	    var npcdata = new FF1Rom.NPCdata(rom);

	    for (int i = start; i <= end; i++) {
		Image<Rgba32> output;
		string name;

		var replacementMaps = Task.Run<List<FF1Lib.Procgen.CompleteMap>>(async () => await FF1Lib.Procgen.NewDungeon.GenerateNewDungeon(rng, rom, (FF1Lib.MapId)i,
																		maps, npcdata, this.Progress)).Result;

		foreach (var replacementMap in replacementMaps) {
		    maps[(int)replacementMap.MapId] = replacementMap.Map;

		    output = rom.RenderMap(maps, replacementMap.MapId, false);
		    name = $"dungeonmap{(int)replacementMap.MapId}-outside.png";
		    output.Save(name);
		    Console.WriteLine($"Wrote {name}");

		    output = rom.RenderMap(maps, replacementMap.MapId, true);
		    name = $"dungeonmap{(int)replacementMap.MapId}-inside.png";
		    output.Save(name);
		    Console.WriteLine($"Wrote {name}");

		    name = $"dungeonmap{(int)replacementMap.MapId}.json";
		    using (StreamWriter file = File.CreateText(name)) {
			replacementMap.SaveJson(file);
		    }
		    Console.WriteLine($"Wrote {name}");

		    using (StreamReader file = File.OpenText(name)) {
			var newmap = FF1Lib.Procgen.CompleteMap.LoadJson(file);

			name = $"dungeonmap{(int)replacementMap.MapId}-2.json";
			using (StreamWriter file2 = File.CreateText(name)) {
			    newmap.SaveJson(file2);
			}
			Console.WriteLine($"Wrote {name}");
		    }
		}
	    }

	    return 0;
	}
    }

}
