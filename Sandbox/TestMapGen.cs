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
			flags.ProcgenWaterfallNoLoops = true;
			MapGenerator generator = new MapGenerator(flags);
			MapGeneratorStrategy strategy = MapGeneratorStrategy.SpanningTree;
			Stopwatch stopwatch = new();

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
					case ConsoleKey.H:
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
					case ConsoleKey.L:
						{
							generator._flags.ProcgenWaterfallNoLoops = !generator._flags.ProcgenWaterfallNoLoops;
							cont = true;
							Console.WriteLine("\rToggled loop avoidance " + ((bool)generator._flags.ProcgenWaterfallNoLoops? "On." : "Off."));
							break;
						}
					case ConsoleKey.A:
						{
							generator._flags.ProcgenWaterfallEntrance = ProcgenWaterfallEntrance.Anywhere;
							cont = true;
							Console.WriteLine("\rSet entrance location to Anywhere");
							break;
						}
					case ConsoleKey.F:
						{
							generator._flags.ProcgenWaterfallEntrance = ProcgenWaterfallEntrance.Furthest;
							cont = true;
							Console.WriteLine("\rSet entrance location to Furthest");
							break;
						}
					case ConsoleKey.C:
						{
							generator._flags.ProcgenWaterfallEntrance = ProcgenWaterfallEntrance.Center;
							cont = true;
							Console.WriteLine("\rSet entrance location to Center");
							break;
						}
					case ConsoleKey.M:
						{
							generator._flags.ProcgenWaterfallEntrance = ProcgenWaterfallEntrance.Mid;
							cont = true;
							Console.WriteLine("\rSet entrance location to Mid");
							break;
						}
					case ConsoleKey.B:
						{
							generator._flags.ProcgenWaterfallEntrance = ProcgenWaterfallEntrance.Branch;
							cont = true;
							Console.WriteLine("\rSet entrance location to Branch");
							break;
						}
					case ConsoleKey.Q:
						{
							generator._flags.ProcgenWaterfallEntrance = ProcgenWaterfallEntrance.Maddening;
							cont = true;
							Console.WriteLine("\rSet entrance location to Maddening");
							break;
						}
					case ConsoleKey.Spacebar:
						{
							stopwatch.Reset();
							stopwatch.Start();
							
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
				stopwatch.Stop();
				var time = stopwatch.Elapsed;
				Console.WriteLine($"Gen time: {time.Minutes * 60 + (double)time.Seconds + time.Milliseconds/1000.0} seconds");
			}
		}
	}
}
