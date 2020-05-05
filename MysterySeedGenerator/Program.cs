using System;
using System.IO;
using Newtonsoft.Json;
using Sandbox;

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
			Console.WriteLine(MysterySeedGeneratorLib.GenerateMysterySeed(weights, seed));
		}
	}
}
