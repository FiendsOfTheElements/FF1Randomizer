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

	int OnExecute(IConsole console)
	{
	    var rom = new FF1Rom(RomPath);
	    rom.LoadSharedDataTables();

	    var output = rom.ExportMapGraphics(Map);
	    output.Save("dungeontiles.png");
	    return 0;
	}
    }
}
