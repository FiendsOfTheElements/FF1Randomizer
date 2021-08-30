﻿using FF1Lib.Sanity;
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
		[Description("Vanilla (Default)")]
		None,

		[Description("Melmond Start")]
		MelmondStart,

		[Description("Elfland Start")]
		ElflandStart,

		[Description("Crescent Start")]
		CrecsentStart,

		[Description("Desert of Death")]
		Desert,

		[Description("No Overworld")]
		NoOverworld,

		[Description("Chanel #125")]
		ProcGen1,

		[Description("Epic Quest")]
		ProcGen2,

		[Description("Archipelago")]
		ProcGen3,

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

		public OwMapExchangeData Data => data;

		public ShipLocations ShipLocations { get; private set; }

		public SCCoords StartingLocation => locations.StartingLocation;

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

			//load default locations first, doh
			locations.LoadData();

			if (data.StartingLocation.HasValue) locations.StartingLocation = data.StartingLocation.Value;
			if (data.AirShipLocation.HasValue) locations.AirShipLocation = data.AirShipLocation.Value;
			if (data.BridgeLocation.HasValue) locations.BridgeLocation = data.BridgeLocation.Value;
			if (data.CanalLocation.HasValue) locations.CanalLocation = data.CanalLocation.Value;

			locations.StoreData();

			foreach (var tf in data.TeleporterFixups) exit[tf.Index.Value] = tf.To;

			exit.StoreData();

			ShipLocations.SetShipLocation(255);
		}

		public void RefreshData()
		{
			//load default locations first, doh
			locations.LoadData();

			if (data.StartingLocation.HasValue) locations.StartingLocation = data.StartingLocation.Value;
			if (data.AirShipLocation.HasValue) locations.AirShipLocation = data.AirShipLocation.Value;
			if (data.BridgeLocation.HasValue) locations.BridgeLocation = data.BridgeLocation.Value;
			if (data.CanalLocation.HasValue) locations.CanalLocation = data.CanalLocation.Value;

			locations.StoreData();

			foreach (var tf in data.TeleporterFixups) exit[tf.Index.Value] = tf.To;

			exit.StoreData();

			ShipLocations = new ShipLocations(locations, data.ShipLocations);

			ShipLocations.SetShipLocation(255);
		}

		public void ExecuteStep2()
		{
		    DomainData originalDomains = new DomainData(rom);
			originalDomains.LoadTable();
			domains.LoadTable();

			foreach (var df in data.DomainFixups) domains.SwapDomains(df.From, df.To);
			foreach (var df in data.DomainUpdates) domains.Data[df.To] = originalDomains.Data[df.From];

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
				case OwMapExchanges.Desert:
					return new OwMapExchange(_rom, _overworldMap, "desert");
				case OwMapExchanges.NoOverworld:
					return new OwMapExchange(_rom, _overworldMap, "nooverworld");
				case OwMapExchanges.ProcGen1:
					return new OwMapExchange(_rom, _overworldMap, "procgen1");
				case OwMapExchanges.ProcGen2:
					return new OwMapExchange(_rom, _overworldMap, "procgen2");
				case OwMapExchanges.ProcGen3:
					return new OwMapExchange(_rom, _overworldMap, "procgen3");
			}

			throw new Exception("oops");
		}

		public static ShipLocations GetDefaultShipLocations(FF1Rom _rom)
		{
			var data = LoadJson("default");

			return new ShipLocations(new OwLocationData(_rom), data.ShipLocations);
		}

		public void SetAirshipLocation(SCCoords coords)
		{
			locations.LoadData();

			locations.AirShipLocation = coords;

			locations.StoreData();
		}
		public void SetStartingLocation(SCCoords coords)
		{
			locations.LoadData();

			locations.StartingLocation = coords;

			locations.StoreData();
		}
	}
}
