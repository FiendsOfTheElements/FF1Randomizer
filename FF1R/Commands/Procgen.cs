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

namespace FF1R.Commands
{
    [Command("procgen", Description = "Create a procedurally generated map")]
    class Procgen
    {
	[Argument(0, Description = "The seed")]
	public int Seed { get; } = 0;

	[Option("-t")]
	public string Subtype { get; } = "GenerateNewOverworld";

	[Option("-r")]
	public bool DoRender { get; }

	int OnExecute(IConsole console)
	{
	    if (Seed == 0) {
		Console.WriteLine("Missing seed");
		return 1;
	    }

	    var rng = new MT19337((uint)this.Seed);

	    var replacementMap = FF1Lib.Procgen.NewOverworld.GenerateNewOverworld(rng, Enum.Parse<OwMapExchanges>(Subtype));
	    replacementMap.Checksum = replacementMap.ComputeChecksum();
	    replacementMap.Seed = this.Seed;
	    replacementMap.FFRVersion = FF1Lib.FFRVersion.Version;

	    var fn = $"FFR_map_{replacementMap.Checksum}.json";
	    using (StreamWriter file = File.CreateText(fn)) {
		JsonSerializer serializer = new JsonSerializer();
		serializer.Formatting = Formatting.Indented;
		serializer.Serialize(file, replacementMap);
	    }
	    Console.WriteLine(fn);

	    if (this.DoRender) {
		MapRender.RenderMap(fn);
	    }
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
		IImageFormat format;
		var tiles = Image.Load(stream, out format);

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
