using FF1Lib.Sanity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;
using RomUtilities;

namespace FF1Lib
{
	public enum OwMapExchanges
	{
		[Description("None(Default)")]
		None,

		[Description("Melmond Start")]
		MelmondStart,

		[Description("Elfland Start")]
		ElflandStart,

		[Description("Crescent Start")]
		CrecsentStart,

		[Description("No Overworld")]
		NoOverworld,

		[Description("Random")]
		Random
	}

	public class OwMapExchange
	{
		OwMapExchangeData data;
		FF1Rom rom;
		OverworldMap overworldMap;
		string name;

		ExitTeleData exit;
		OwLocationData locations;
		DomainData domains;

		public ShipLocations ShipLocations { get; private set; }

		public OwMapExchange(FF1Rom _rom, OverworldMap _overworldMap, string _name)
		{
			rom = _rom;
			overworldMap = _overworldMap;
			name = _name;

			exit = new ExitTeleData(rom);
			locations = new OwLocationData(rom);
			domains = new DomainData(rom);

			data = LoadJson(name);

			ShipLocations = new ShipLocations(locations, data.ShipLocations);
		}

		public void ExecuteStep1()
		{
			overworldMap.SwapMap(name + ".ffm");

			if (data.StartingLocation.HasValue) locations.StartingLocation = data.StartingLocation.Value;
			if (data.AirShipLocation.HasValue) locations.AirShipLocation = data.AirShipLocation.Value;

			locations.StoreData();

			foreach (var tf in data.TeleporterFixups) exit[tf.Index.Value] = tf.To;

			exit.StoreData();

			ShipLocations.SetShipLocation(255);
		}

		public void ExecuteStep2()
		{
			domains.LoadTable();

			foreach (var df in data.DomainFixups) domains.SwapDomains(df.From, df.To);

			domains.StoreTable();
			locations.StoreData();
		}

		private static OwMapExchangeData LoadJson(string _name)
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var resourcePath = assembly.GetManifestResourceNames().First(str => str.EndsWith(_name + ".json"));

			using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
			using (StreamReader rd = new StreamReader(stream))
			{
				return JsonConvert.DeserializeObject<OwMapExchangeData>(rd.ReadToEnd());
			}
		}

		public static OwMapExchange FromFlags(FF1Rom _rom, OverworldMap _overworldMap, Flags flags, MT19337 rng)
		{
			if (!flags.SanityCheckerV2) return null;

			var mx = flags.OwMapExchange;
			if (mx == OwMapExchanges.Random) mx = (OwMapExchanges)rng.Between(0, 3);

			switch (mx)
			{
				case OwMapExchanges.None:
					return null;
				case OwMapExchanges.MelmondStart:
					return new OwMapExchange(_rom, _overworldMap, "melmond_start");
				case OwMapExchanges.ElflandStart:
					return new OwMapExchange(_rom, _overworldMap, "elfland_start");
				case OwMapExchanges.CrecsentStart:
					return new OwMapExchange(_rom, _overworldMap, "crescent_start");
				case OwMapExchanges.NoOverworld:
					if ((bool)!flags.Entrances && (bool)!flags.Floors && (bool)!flags.Towns)
					{
						return new OwMapExchange(_rom, _overworldMap, "nooverworld");
					}
					else
						return null;
					
			}

			throw new Exception("oops");
		}

		public static ShipLocations GetDefaultShipLocations(FF1Rom _rom)
		{
			var data = LoadJson("default");

			return new ShipLocations(new OwLocationData(_rom), data.ShipLocations);
		}
	}
}
