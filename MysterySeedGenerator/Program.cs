using System;
using System.IO;
using FF1Lib;
using Newtonsoft.Json;

namespace MysterySeedGenerator
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 1 || args.Length > 2)
			{
				throw new Exception("Please ensure you provide the file name and (optionally) a seed");
			}

			var seed = new Random().Next();
			if (args.Length == 2)
			{
				int.TryParse(args[1], out seed);
			}
			var jsonString = File.ReadAllText(args[0]);
			var weights = JsonConvert.DeserializeObject<Weights>(jsonString);
			var mysterySeed = MysterySeedGeneratorLib.GenerateMysterySeed(weights, seed);
			Console.WriteLine(Flags.EncodeFlagsText(mysterySeed.Flags));
			Console.WriteLine($"Loose Item Count: {mysterySeed.LooseCount}");
			Console.WriteLine($"Incentive Items are: {string.Join(",", mysterySeed.Incentives)}");
			Console.WriteLine($"Incentive Locations are: {string.Join(",", mysterySeed.IncentiveLocations)}");
		}
	}
}
