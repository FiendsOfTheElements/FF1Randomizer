using System;
using McMaster.Extensions.CommandLineUtils;

using FFR.Common;

namespace FF1R
{
	[Command(Name = "ff1r", Description = "Final Fantasy Randomizer"),
		Subcommand("presets", typeof(Commands.Presets)),
		Subcommand("generate", typeof(Commands.Generate))]
	internal class Program
	{
		private readonly VersionInfo version = new(0, 1);

		[Option("--version", Description = "Show version")]
		public bool Version { get; }

		public static int Main(string[] args)
		{
			return CommandLineApplication.Execute<Program>(args);
		}

		private int OnExecute(CommandLineApplication app, IConsole console)
		{
			if (Version)
			{
				console.WriteLine($"{version}");
				return 0;
			}

			console.WriteLine("You must specify a subcommand.");
			app.ShowHelp();
			return 1;
		}
	}
}
