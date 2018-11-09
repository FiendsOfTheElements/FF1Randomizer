namespace FF1R.Commands
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using McMaster.Extensions.CommandLineUtils;

	using FFR.Common;

	[Command("presets", Description = "Manage presets"),
		Subcommand("list", typeof(List)),
		Subcommand("add", typeof(Add)),
		Subcommand("remove", typeof(Remove))]
	class Presets
	{
		int OnExecute(IConsole console)
		{
			console.Error.WriteLine("You must specify an action.");
			return 1;
		}

		[Command("ls", Description = "List all presets")]
		class List
		{
			int OnExecute(IConsole console)
			{
				foreach(var name in FFR.Common.Presets.List())
					console.WriteLine(name);
				return 0;
			}
		}

		[Command("add", Description = "Add a new preset")]
		class Add
		{
			[Required(ErrorMessage = "You must specify the preset's name")]
			[Argument(0, Description = "The name to give the new preset")]
			public string Name { get; }

			[Required(ErrorMessage = "You must specify the preset's flag string")]
			[Argument(1, Description = "The flag string to give the new preset")]
			public string FlagString { get; }

			int OnExecute(IConsole console)
			{
				FFR.Common.Presets.Add(Name, FlagString);
				return 0;
			}
		}

		[Command("rm", Description = "Remove a preset")]
		class Remove
		{
			[Required(ErrorMessage = "You must specify the preset's name")]
			[Argument(0, Description = "The name to give the new preset")]
			public string Name { get; }

			int OnExecute(IConsole console)
			{
				try
				{
					FFR.Common.Presets.Remove(Name);
					return 0;
				}
				catch(System.IO.FileNotFoundException)
				{
					console.Error.WriteLine($"Preset not found: '{Name}'");
					return 1;
				}
			}
		}
	}
}
