using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using FF1Lib;
using RomUtilities;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
			var filename = "ff1.nes";
			var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);

			var rom = new FF1Rom(fs);
			var provider = RNGCryptoServiceProvider.Create();
			var seed = new byte[8];
			MT19337 rng;

			MapRequirements reqs = new MapRequirements
			{
				MapId = MapId.Waterfall,
				Floor = Tile.WaterfallRandomEncounters,
				Treasures = new List<byte> { 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E },
				NPCs = Enumerable.Range(0, 11),

				Rom = rom,
			};

			MapGenerator generator = new MapGenerator();
			MapGeneratorStrategy strategy = MapGeneratorStrategy.WaterfallClone;


			while (true)
			{
				provider.GetBytes(seed);
				rng = new MT19337(BitConverter.ToUInt32(seed, 0));
				CompleteMap waterfall = generator.Generate(rng, strategy, reqs);
				Console.WriteLine("Press a key...");
				Console.ReadLine();
			}

		}
	}
}
