using System;
using System.IO;
using System.Text.RegularExpressions;
using FF1Lib;
using RomUtilities;

namespace FF1R
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				DoUsage();
				return;
			}

			var filename = args[0];
			try
			{
				ParseArguments(args, out Blob seed, out Flags flags);

				DoRandomize(filename, seed, flags);
			}
			catch (ArgumentException ex)
			{
				Console.WriteLine(ex.Message);
				DoUsage();
			}
		}

		private static void ParseArguments(string[] args, out Blob seed, out Flags flags)
		{
			Flags LoadPreset(string preset)
			{
				if (preset.LastIndexOf(".json") == -1)
				{
					preset += ".json";
				}

				if (preset.IndexOfAny(new[] { '/', '\\' }) == -1)
				{
					preset = "presets/" + preset;
				}

				if (!File.Exists(preset))
				{
					throw new ArgumentException("Preset not found");
				}

				string json = File.ReadAllText(preset);
				return Flags.FromJson(json);
			}

			seed = null;
			flags = null;

			int i = 1;
			while (i < args.Length - 1)
			{
				string paramName = args[i];
				string param = args[i + 1];

				switch (paramName)
				{
					case "--import":
					case "-i":
						if (!Regex.IsMatch(param, "[0-9A-F]{8}_[0-9A-Z-a-z!-]+"))
						{
							throw new ArgumentException("Bad import string");
						}

						seed = Blob.FromHex(param.Substring(0, 8));
						flags = Flags.DecodeFlagsText(param.Substring(9));

						break;

					case "--seed":
					case "-s":
						if (!Regex.IsMatch(param, "[0-9A-F]{8}"))
						{
							throw new ArgumentException("Bad seed string");
						}

						seed = Blob.FromHex(param);

						break;

					case "--flags":
					case "-f":
						flags = Flags.DecodeFlagsText(param);

						break;

					case "--preset":
					case "-p":
						flags = LoadPreset(param);

						break;

					default:
						throw new ArgumentException($"Unrecognized parameter", paramName);
				}

				i += 2;
			}

			if (seed == null)
			{
				seed = Blob.Random(4);
			}

			if (flags == null)
			{
				flags = LoadPreset("default");
			}
		}

		private static void DoUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("    dotnet FF1R.dll <filename> <parameters>");
			Console.WriteLine("");
			Console.WriteLine("Parameters:");
			Console.WriteLine("You may provide a seed and flags together or separately, or you may specify a");
			Console.WriteLine("flags preset.  If you do not provide a seed, a random one will be generated for");
			Console.WriteLine("you.  If you do not provide flags or a preset, the default preset will be");
			Console.WriteLine("loaded.");
			Console.WriteLine("");
			Console.WriteLine("    -i, --import <crimbot_string>");
			Console.WriteLine("        A seed and flags string output by crimbot.");
			Console.WriteLine("    -s, --seed <seed>");
			Console.WriteLine("        A seed in hexadecimal format (8 characters).");
			Console.WriteLine("    -f, --flags <flags>");
			Console.WriteLine("        A set of flags, base64 encoded.");
			Console.WriteLine("    -p, --preset <preset_name>");
			Console.WriteLine("        The name of a JSON preset file from which to load flags.");
		}

		private static void DoRandomize(string filename, Blob seed, Flags flags)
		{
			var rom = new FF1Rom(filename);
			rom.Randomize(seed, flags);

			var fileRoot = filename.Substring(0, filename.LastIndexOf(".", StringComparison.InvariantCulture));
			var outputFilename = $"{fileRoot}_{seed.ToHex()}_{Flags.EncodeFlagsText(flags)}.nes";
			rom.Save(outputFilename);
			Console.WriteLine(outputFilename);
		}
	}
}
