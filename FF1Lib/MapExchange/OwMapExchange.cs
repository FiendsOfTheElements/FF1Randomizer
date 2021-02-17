using FF1Lib.Sanity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FF1Lib
{
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

			LoadJson();

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

		private void LoadJson()
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly();			
			var resourcePath = assembly.GetManifestResourceNames().First(str => str.EndsWith(name + ".json"));

			using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
			using (StreamReader rd = new StreamReader(stream))
			{
				data = JsonConvert.DeserializeObject<OwMapExchangeData>(rd.ReadToEnd());
			}
		}
	}
}
