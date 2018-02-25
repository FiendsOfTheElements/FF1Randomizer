using System;
using FF1Lib;
using RomUtilities;

namespace FF1R
{
	class Program
	{
		static void Main(string[] args)
		{
			var filename = args[0];
			var argsParts = args[1].Split(new[] { '_' });
			var seedText = argsParts[0];
			var flagsText = argsParts[1];

			var rom = new FF1Rom(filename);
			rom.Randomize(Blob.FromHex(seedText), Flags.DecodeFlagsText(flagsText));

			var fileRoot = filename.Substring(0, filename.LastIndexOf(".", StringComparison.InvariantCulture));
			var outputFilename = $"{fileRoot}_{seedText}_{flagsText}.nes";
			rom.Save(outputFilename);
		}
	}
}