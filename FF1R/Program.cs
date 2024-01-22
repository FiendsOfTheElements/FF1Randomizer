namespace FF1R
{
	using System;
	using McMaster.Extensions.CommandLineUtils;

	using FFR.Common;

	[Command(Name = "ff1r", Description = "Final Fantasy Randomizer"),
	Subcommand("presets", typeof(Commands.Presets)),
	 Subcommand("generate", typeof(Commands.Generate)),
	 Subcommand("procgen", typeof(Commands.Procgen)),
	 Subcommand("render", typeof(Commands.MapRender)),
	 Subcommand("dumpmagic", typeof(Commands.DumpMagicStats)),
	 Subcommand("dumpenemies", typeof(Commands.DumpEnemyStats)),
	 Subcommand("importmagic", typeof(Commands.ImportMagicStats)),
	 Subcommand("renderdungeon", typeof(Commands.RenderDungeon)),
	 Subcommand("relocatechests", typeof(Commands.RelocateDungeonChests)),
	 Subcommand("createdungeon", typeof(Commands.CreateDungeon))]

	class Program
	{
		readonly VersionInfo version = new VersionInfo(0, 1);

		[Option("--version", Description = "Show version")]
		public bool Version { get; }

		public static int Main(string[] args)
			=> CommandLineApplication.Execute<Program>(args);

		int OnExecute(CommandLineApplication app, IConsole console)
		{
			if (Version) {
				console.WriteLine($"{version}");
				return 0;
			}

			console.WriteLine("You must specify a subcommand.");
			app.ShowHelp();
			return 1;
		}
	}
}
