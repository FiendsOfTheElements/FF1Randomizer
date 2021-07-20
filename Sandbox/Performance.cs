using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FF1Lib;
using RomUtilities;

namespace Sandbox
{
	public class Performance
	{
		public static async Task Run()
		{
			string json;
			using (var fs = new FileStream("presets/full-npc.json", FileMode.Open, FileAccess.Read, FileShare.Read))
			using (var sr = new StreamReader(fs))
			{
				json = await sr.ReadToEndAsync();
			}

			var seeds = Enumerable.Range(0, 1000).Select(x => Blob.Random(4)).ToList();

			var flags = Flags.FromJson(json).flags;
			flags.Entrances = true;
			flags.Towns = true;
			flags.Floors = true;
			flags.MapOpenProgression = true;
			flags.MapOpenProgressionExtended = true;
			flags.AllowDeepCastles = true;
			flags.AllowDeepTowns = true;

			var preferences = new Preferences();

			var roms = new List<FF1Rom>();
			for (int i = 0; i < 1000; i++)
			{
				roms.Add(new FF1Rom("ff1.nes"));
			}

			for (int i = 0; i < 1000; i++)
			{
				roms[i].Randomize(seeds[i], flags, preferences);
			}
		}
	}
}
