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
			using (FileStream fs = new("presets/full-npc.json", FileMode.Open, FileAccess.Read, FileShare.Read))
			using (StreamReader sr = new(fs))
			{
				json = await sr.ReadToEndAsync();
			}

			List<Blob> seeds = Enumerable.Range(0, 1000).Select(x => Blob.Random(4)).ToList();

			Flags flags = Flags.FromJson(json);
			flags.Entrances = true;
			flags.Towns = true;
			flags.Floors = true;
			flags.MapOpenProgression = true;
			flags.MapOpenProgressionExtended = true;
			flags.AllowDeepCastles = true;
			flags.AllowDeepTowns = true;

			Preferences preferences = new();

			List<FF1Rom> roms = new();
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
