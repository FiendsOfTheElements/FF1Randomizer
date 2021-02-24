using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using FF1Lib;
using FF1Lib.Procgen;
using RomUtilities;

namespace Sandbox
{
	internal static class TestMapGen
	{
		public static void Run(FF1Rom rom, RandomNumberGenerator csharpRNG)
		{
			MapRequirements reqs = new()
			{
				MapId = MapId.EarthCaveB1,
				Rom = rom,
			};

			MapGenerator generator = new();
			MapGeneratorStrategy strategy = MapGeneratorStrategy.BSPTree;


			while (true)
			{
				byte[] seed = new byte[8];
				csharpRNG.GetBytes(seed);
				MT19337 rng = new(BitConverter.ToUInt32(seed, 0));
#pragma warning disable IDE0059 // Unnecessary assignment of a value
				CompleteMap waterfall = generator.Generate(rng, strategy, reqs);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
				Console.WriteLine("Press a key to generate another one (X to quit)...");
				if (Console.ReadKey().Key == ConsoleKey.X)
				{
					break;
				}
			}
		}
	}
}
