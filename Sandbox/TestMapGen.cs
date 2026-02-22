using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using FF1Lib;
using FF1Lib.Procgen;
using Microsoft.VisualBasic;
using RomUtilities;

namespace Sandbox
{
	static class TestMapGen
	{
		public static void Run(FF1Rom rom)
		{
			
			var csharpRNG = RandomNumberGenerator.Create();
			MapRequirements reqs = new MapRequirements
			{
				MapIndex = MapIndex.Waterfall,
				Rom = rom,
				MapObjects = new(rom, MapIndex.Waterfall)
			};

			var flags = new Flags();
			flags.ProcgenWaterfall = ProcgenWaterfallMode.Uniform;
			flags.ProcgenWaterfallDensity = ProcgenWaterfallDensity.Normal;
			flags.ProcgenWaterfallHallwayLength = ProcgenWaterfallHallwayLength.Mid;
			MapGenerator generator = new MapGenerator(flags);
			MapGeneratorStrategy strategy = MapGeneratorStrategy.SpanningTree;

			Console.WriteLine("\n\nReady to generate\n");
			while (true)
			{
				
				// Console.WriteLine("Press a key to generate another one (X to quit)...");
				var k = Console.ReadKey();
				bool cont = false;
				switch (k.Key)
				{
					case ConsoleKey.U:
						{
							generator._flags.ProcgenWaterfall = ProcgenWaterfallMode.Uniform;
							cont = true;
							Console.WriteLine("\rSet mode to Uniform.");
							break;
						}
					case ConsoleKey.P:
						{
							generator._flags.ProcgenWaterfall = ProcgenWaterfallMode.Polar;
							cont = true;
							Console.WriteLine("\rSet mode to Polar.");
							break;
						}
					case ConsoleKey.J:
						{
							generator._flags.ProcgenWaterfall = ProcgenWaterfallMode.JitteredEven;
							cont = true;
							Console.WriteLine("\rSet mode to Jittered Even.");
							break;
						}
					case ConsoleKey.L:
						{
							generator._flags.ProcgenWaterfall = ProcgenWaterfallMode.Linear;
							cont = true;
							Console.WriteLine("\rSet mode to Hallway.");
							break;
						}
					case ConsoleKey.S:
						{
							generator._flags.ProcgenWaterfallDensity = ProcgenWaterfallDensity.Sparse;
							cont = true;
							Console.WriteLine("\rSet density to Sparse.");
							break;
						}
					case ConsoleKey.N:
						{
							generator._flags.ProcgenWaterfallDensity = ProcgenWaterfallDensity.Normal;
							cont = true;
							Console.WriteLine("\rSet density to Normal.");
							break;
						}
					case ConsoleKey.D:
						{
							generator._flags.ProcgenWaterfallDensity = ProcgenWaterfallDensity.Dense;
							cont = true;
							Console.WriteLine("\rSet density to Dense.");
							break;
						}
					case ConsoleKey.D1:
						{
							generator._flags.ProcgenWaterfallHallwayLength = ProcgenWaterfallHallwayLength.Short;
							cont = true;
							Console.WriteLine("\rSet hallway length to Short.");
							break;
						}
					case ConsoleKey.D2:
						{
							generator._flags.ProcgenWaterfallHallwayLength = ProcgenWaterfallHallwayLength.Mid;
							cont = true;
							Console.WriteLine("\rSet hallway length to Mid.");
							break;
						}
					case ConsoleKey.D3:
						{
							generator._flags.ProcgenWaterfallHallwayLength = ProcgenWaterfallHallwayLength.Long;
							cont = true;
							Console.WriteLine("\rSet hallway length to Long.");
							break;
						}
					case ConsoleKey.D4:
						{
							generator._flags.ProcgenWaterfallHallwayLength = ProcgenWaterfallHallwayLength.Absurd;
							cont = true;
							Console.WriteLine("\rSet hallway length to Absurd.");
							break;
						}
					case ConsoleKey.Spacebar:
						{
							cont = false;
							break;
						}
					default:
						{
							Console.Write("\r \r");
							cont = true;
							break;
						}
				}
				if (cont)
					continue;

				var seed = new byte[8];
				csharpRNG.GetBytes(seed);
				MT19337 rng = new MT19337(BitConverter.ToUInt32(seed, 0));
				CompleteMap waterfall = generator.Generate(rng, strategy, reqs);
				
			}
		}
	}
}
