using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using RomUtilities;
using System.IO;
using FFR.Common;
using Newtonsoft.Json;
using FF1Lib;
using FF1Lib.Procgen;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System.Linq;
using System.Threading.Tasks;

namespace FF1R.Commands
{
    [Command("procgen", Description = "Create a procedurally generated map")]
    class Procgen
    {
	[Option("-s")]
	public int Seed { get; } = 0;

	[Option("-t")]
	public string Subtype { get; } = "GenerateNewOverworld";

	[Option("-r")]
	public bool DoRender { get; }

	[Option("-y")]
	public bool Retry { get; }

	[Option("-p")]
	public int Pack { get; } = 1;
	[Option("-a")]
	public bool SuffleAccess { get; }
	[Option("-u")]
	public bool UnsafeStart { get; }
	[Option("-S")]
	public string SeedFile { get; } = null;

	async Task Progress(string message="", int addMax=0) {
	    await Task.Yield();
	}

	int OnExecute(IConsole console)
	{
	    if (Seed == 0 && this.SeedFile == null) {
		Console.WriteLine("Missing seed");
		return 1;
	    }

	    OwMapExchanges subtype = Enum.Parse<OwMapExchanges>(Subtype);

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
		OwMapExchangeData replacementMap = null;
		int effectiveSeed;
		if (seeds == null) {
		    effectiveSeed = this.Seed + i;
		} else {
		    effectiveSeed = seeds[i];
		}
		var rng = new MT19337((uint)effectiveSeed);
		do {
		    try {
			replacementMap = Task.Run<OwMapExchangeData>(async () => await FF1Lib.Procgen.NewOverworld.GenerateNewOverworld(rng, subtype, SuffleAccess, UnsafeStart, this.Progress)).Result;
		    } catch (System.AggregateException ae) {
			ae.Handle((x) =>
			{
			    if (x is FailedToGenerate) {
				if (!this.Retry) {
				    Console.WriteLine($"Failed to generate seed {effectiveSeed}");
				    return false;
				}
				effectiveSeed = (int)rng.Next() & 0x7FFFFFFF;
				rng = new MT19337((uint)effectiveSeed);
				return true;
			    }
			    return false;
			});
		    }
		} while (replacementMap == null);

		replacementMap.Checksum = replacementMap.ComputeChecksum();
		replacementMap.Seed = effectiveSeed;
		replacementMap.FFRVersion = FF1Lib.FFRVersion.Version;

		string fn;
		if (numberOfMapsToGenerate == 1) {
		    fn = $"FFR_map_{replacementMap.Checksum}.json";
		} else {
		    fn = $"{replacementMap.Seed,8:X8}.json";
		}
		using (StreamWriter file = File.CreateText(fn)) {
		    JsonSerializer serializer = new JsonSerializer();
		    if (this.Pack == 1) {
			serializer.Formatting = Formatting.Indented;
		    }
		    serializer.Serialize(file, replacementMap);
		}
		Console.WriteLine(fn);

		if (this.DoRender) {
		    MapRender.RenderMap(fn);
		}
	    });

	    return 0;
	}
    }

    [Command("render", Description = "Render map")]
    class MapRender
    {
	[Argument(0, Description = "The map")]
	public string Mapfile { get; }

	public static void RenderMap(string mapfile) {
	    var assembly = System.Reflection.Assembly.GetExecutingAssembly();
	    var resourcePath = assembly.GetManifestResourceNames().First(str => str.EndsWith("maptiles.png"));

	    using (Stream stream = assembly.GetManifestResourceStream(resourcePath)) {
		//IImageFormat format;
		var tiles = Image.Load(stream);

		using (StreamReader file = new StreamReader(mapfile)) {
		    JsonSerializer serializer = new JsonSerializer();

		    var map = serializer.Deserialize<OwMapExchangeData>(new JsonTextReader(file));
		    var rows = new List<List<byte>>();
		    foreach (var c in map.DecompressedMapRows) {
			rows.Add(new List<byte>(Convert.FromBase64String(c)));
		    }
		    var output = new Image<Rgba32>(16 * 256, 16 * 256);

		    for (int y = 0; y < 256; y++) {
			for (int x = 0; x < 256; x++) {
			    var t = rows[y][x];
			    var tile_row = t/16;
			    var tile_col = t%16;
			    var src = tiles.Clone(d => d.Crop(new Rectangle(tile_col*16, tile_row*16, 16, 16)));
			    output.Mutate(d => d.DrawImage(src, new Point(x*16, y*16), 1));
			}
		    }
		    var fn = mapfile.Replace(".json", ".png");
		    output.Save(fn);
		    Console.WriteLine(fn);
		}
	    }
	}

	int OnExecute(IConsole console)
	{
	    RenderMap(this.Mapfile);
	    return 0;
	}
    }
}
