namespace FF1R.Commands
{
	using System;
	using System.Text;
	using McMaster.Extensions.CommandLineUtils;
	using RomUtilities;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Schema;

	using FF1Lib;
	using FFR.Common;

	[Command("getdigest", Description = "Retrieve the randomizer digest for a given ROM")]
	class GetDigest
	{
		[Argument(0, Description = "Randomizer Generated ROM")]
		[FileExists]
		public string RomPath { get; }


		int OnExecute(IConsole console)
		{

			int startingOffset = 0xA000;
			int bufferSize = 0x01;
			int bank = 0x1b;
			int length=0;
			int offset;
			FF1Rom rom = new FF1Rom(RomPath);
			offset = startingOffset;

			while (!(Encoding.UTF8.GetString(rom.GetFromBank(bank, offset,bufferSize)).Equals("}")))
			{
				offset += bufferSize;
				length++;
			}
			FF1Lib.FF1Rom.RandomizerDigest digest = JsonConvert.DeserializeObject<FF1Lib.FF1Rom.RandomizerDigest>(Encoding.ASCII.GetString(rom.GetFromBank(bank, startingOffset, length + 1)));

			Console.WriteLine("Seed: " + digest.seed);
			Console.WriteLine("Flags: " + digest.encodedFlagString);
			Console.WriteLine("Commit: " + digest.commitSha);
			Console.WriteLine("ROM Hash: " + digest.inputRomSha1);
			return 0;
		}
	}
}
