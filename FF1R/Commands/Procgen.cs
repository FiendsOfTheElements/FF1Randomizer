namespace FF1R.Commands
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using McMaster.Extensions.CommandLineUtils;

	using FFR.Common;

	[Command("procgen", Description = "Create a procedurally generated map")]
	
	class Procgen
	{
		[Argument(0, Description = "The output file name")]
		public string OutputFile { get; }

		int OnExecute(IConsole console)
		{
			console.Error.WriteLine($"procgen subcommand {OutputFile}");
			return 1;
		}
	}
}
