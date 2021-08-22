namespace FF1R.Commands
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using McMaster.Extensions.CommandLineUtils;
	using RomUtilities;
	using System.IO;
	using FFR.Common;

	[Command("procgen", Description = "Create a procedurally generated map")]
	
	class Procgen
	{
		[Argument(0, Description = "The output file name")]
		public string OutputFile { get; }

		int OnExecute(IConsole console)
		{
			var rng = new MT19337(125);
			var replacement = FF1Lib.Procgen.NewOverworld.GenerateNewOverworld(rng);
			using (BinaryWriter writer = new BinaryWriter(File.Open("map5.ffm", FileMode.Create))) {
				for (int y = 0; y < FF1Lib.Procgen.OverworldState.MAPSIZE; y++) {
					for (int x = 0; x < FF1Lib.Procgen.OverworldState.MAPSIZE; x++) {
						writer.Write(replacement.Tiles[y][x]);
					}
				}            
        	}
			return 0;
		}
	}
}
