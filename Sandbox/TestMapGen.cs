using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using FF1Lib;
using FF1Lib.Procgen;
using RomUtilities;

namespace Sandbox
{
	static class TestMapGen
	{
		public static void Run(FF1Rom rom, RandomNumberGenerator csharpRNG)
		{
			MapRequirements reqs = new MapRequirements
			{
				MapId = MapId.EarthCaveB1,
				Rom = rom,
			};

			MapGenerator generator = new MapGenerator();
			MapGeneratorStrategy strategy = MapGeneratorStrategy.BSPTree;


			while (true)
			{
				var seed = new byte[8];
				csharpRNG.GetBytes(seed);
				MT19337 rng = new MT19337(BitConverter.ToUInt32(seed, 0));
				CompleteMap waterfall = generator.Generate(rng, strategy, reqs);
				Console.WriteLine("Press a key to generate another one (X to quit)...");
				if (Console.ReadKey().Key == ConsoleKey.X)
					break;
			}
		}
	}
}
