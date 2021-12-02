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
using System.IO.Compression;
using FF1Lib.Procgen;

namespace FF1Lib
{
	public enum OwMapExchanges
	{
		[Description("Vanilla (Default)")]
		None,

    [Description("Generate New Overworld")]
		GenerateNewOverworld,

    [Description("Generate New Overworld (shuffled access)")]
		GenerateNewOverworldShuffledAccess,

    [Description("Generate New Overworld (unsafe start)")]
		GenerateNewOverworldShuffledAccessUnsafe,

    [Description("Lost Woods")]
		LostWoods,

    [Description("Desert of Death")]
		Desert,

		[Description("No Overworld")]
		NoOverworld,

    [Description("Random Pregenerated 512")]
		RandomPregenerated,

		[Description("Import Custom Map")]
		ImportCustomMap,
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

		Stream mapStream;

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

		public OwMapExchange(FF1Rom _rom, OverworldMap _overworldMap, MT19337 rng)
		{
 			rom = _rom;
			overworldMap = _overworldMap;

			exit = new ExitTeleData(rom);
			locations = new OwLocationData(rom);
			domains = new DomainData(rom);

			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var resourcePath = assembly.GetManifestResourceNames().First(str => str.EndsWith("pregenerated.zip"));

			using Stream stream = assembly.GetManifestResourceStream(resourcePath);

			var archive = new ZipArchive(stream);

			var maplist = archive.Entries.Where(e => e.Name.EndsWith(".ffm")).Select(e => e.Name.Replace(".ffm", "")).ToList();

			var map = maplist.PickRandom(rng);


			data = LoadJson(archive.GetEntry(map + ".json").Open());

			ShipLocations = new ShipLocations(locations, data.ShipLocations);

			using var stream2 = archive.GetEntry(map + ".ffm").Open();
			using var rd = new BinaryReader(stream2);
			byte[] mapData = rd.ReadBytes(65536);
			mapStream = new MemoryStream(mapData);
    }

    public OwMapExchange(FF1Rom _rom, OverworldMap _overworldMap, OwMapExchangeData replacement)
		{

			rom = _rom;
			overworldMap = _overworldMap;

			exit = new ExitTeleData(rom);
			locations = new OwLocationData(rom);
			domains = new DomainData(rom);

      data = replacement;

			ShipLocations = new ShipLocations(locations, data.ShipLocations);
		}

		public void ExecuteStep1()
		{
			if (mapStream != null)
			{
				overworldMap.SwapMap(mapStream);
			}
      else if (Data.DecompressedMapRows != null)
      {
        overworldMap.SwapMap(Data.DecompressedMapRows);
      }
			else
			{
				overworldMap.SwapMap(name + ".ffm");
			}

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

			if (data.HorizontalBridge) {
			    // Rotate bridge sprite
			    // tiles $14 - $17
			    // CHR for map tiles is bank $02 at $9C00

			    // 0 1  \ \
			    // 2 3  / /

			    var tile0 = new byte[8*8] {
				0, 0, 0, 0, 1, 1, 1, 1,
				0, 0, 1, 1, 2, 2, 2, 2,
				1, 1, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 1, 1, 1, 1, 1,
				2, 1, 1, 2, 2, 2, 2, 2,
				1, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2,
			    };

			    var tile1 = new byte[8*8] {
				2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 1, 1, 1, 1,
				2, 2, 1, 1, 2, 2, 2, 2,
				1, 1, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 1, 1, 1, 1, 1,
				1, 1, 1, 0, 0, 0, 0, 0
			    };

			    var tile2 = new byte[8*8];
			    var tile3 = new byte[8*8];

			    for (int j = 0; j < 8; j++) {
				for (int i = 0; i < 8; i++) {
				    // mirror tiles
				    tile2[j*8 + i] = tile0[j*8 + (7-i)];
				    tile3[j*8 + i] = tile1[j*8 + (7-i)];
				}
			    }

			    rom.PutInBank(0x2, 0x9C00 + 0x04*16, rom.EncodeForPPU(tile0));
			    rom.PutInBank(0x2, 0x9C00 + 0x05*16, rom.EncodeForPPU(tile2));
			    rom.PutInBank(0x2, 0x9C00 + 0x06*16, rom.EncodeForPPU(tile1));
			    rom.PutInBank(0x2, 0x9C00 + 0x07*16, rom.EncodeForPPU(tile3));
			}
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

			if (data.DomainFixups != null) foreach (var df in data.DomainFixups) domains.SwapDomains(df.From, df.To);
			if (data.DomainUpdates != null) foreach (var df in data.DomainUpdates) domains.Data[df.To] = originalDomains.Data[df.From];

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

		private OwMapExchangeData LoadJson(Stream stream)
		{
			using (StreamReader rd = new StreamReader(stream))
			{
				return JsonConvert.DeserializeObject<OwMapExchangeData>(rd.ReadToEnd());
			}
		}

		public static OwMapExchange FromFlags(FF1Rom _rom, OverworldMap _overworldMap, Flags flags, MT19337 rng)
		{
			if (!flags.SanityCheckerV2) return null;

			int seed;

			if (flags.MapGenSeed != 0)
			{
				seed = flags.MapGenSeed;
			}
			else
			{
				seed = (int)rng.Next();
			}

			MT19337 maprng = new MT19337((uint)seed);

			var mx = flags.OwMapExchange;

			switch (mx)
			{
				case OwMapExchanges.None:
					return null;
				case OwMapExchanges.Desert:
					if (flags.ReplacementMap == null)
					{
						flags.ReplacementMap = DesertOfDeath.GenerateDesert(maprng);
					}
					return new OwMapExchange(_rom, _overworldMap, flags.ReplacementMap);
				case OwMapExchanges.NoOverworld:
					return new OwMapExchange(_rom, _overworldMap, "nooverworld");
				case OwMapExchanges.RandomPregenerated:
					return new OwMapExchange(_rom, _overworldMap, rng);
				case OwMapExchanges.GenerateNewOverworld:
				case OwMapExchanges.GenerateNewOverworldShuffledAccess:
				case OwMapExchanges.GenerateNewOverworldShuffledAccessUnsafe:
				case OwMapExchanges.LostWoods:
				  if (flags.ReplacementMap == null) {
				      flags.ReplacementMap = NewOverworld.GenerateNewOverworld(maprng, mx);
				  }
				  return new OwMapExchange(_rom, _overworldMap, flags.ReplacementMap);
			    case OwMapExchanges.ImportCustomMap:
				if (flags.ReplacementMap == null) {
				    throw new Exception("No replacement map was supplied");
				}
				return new OwMapExchange(_rom, _overworldMap, flags.ReplacementMap);
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
