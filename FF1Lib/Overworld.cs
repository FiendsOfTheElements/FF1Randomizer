using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class Overworld
	{
		private OverworldMap overworldMap;
		private OwMapExchange owMapExchange;
		private OwMapExchangeData owMapExchangeData;
		private OwLocationData locations;
		private ShipLocations shipLocations;

		private Flags flags;
		private FF1Rom rom;
		private MT19337 rng;

		public List<List<byte>> DecompressedMap { get => overworldMap.MapBytes; }
		public OwMapExchangeData MapExchangeData { get => owMapExchange.Data; }
		public OwMapExchange MapExchange { get => owMapExchange; }
		public OverworldMap OverworldMap { get => overworldMap; }
		public Overworld(FF1Rom _rom, Flags _flags, Settings _settings, MT19337 _rng)
		{
			flags = _flags;
			rom = _rom;
			rng = _rng;

			locations = new OwLocationData(rom);
			overworldMap = new OverworldMap(rom, flags);
			//locations = new OwLocationData(_rom);
			shipLocations = OwMapExchange.GetDefaultShipLocations(_rom);
			//_owMapExchange = await OwMapExchange.FromFlags(this, _overworldMap, flags, rng);


		}
		public async void LoadMapExchange()
		{
			owMapExchange = await OwMapExchange.FromFlags(rom, overworldMap, flags, rng);
			owMapExchange?.ExecuteStep1();
			shipLocations = owMapExchange?.ShipLocations ?? OwMapExchange.GetDefaultShipLocations(rom);
		}
		public void Update(Settings settings)
		{
			shipLocations.UpdateDocks(settings);
		}
		public void Update(Teleporters teleporters)
		{
			shipLocations.UpdateDocks(flags);

			overworldMap.ApplyMapEdits();

			if ((bool)flags.ShuffleChimeAccess)
			{
				overworldMap.ShuffleChime(rng, (bool)flags.ShuffleChimeIncludeTowns);
			}

			// we just want to upadte palette here, so this should be a teleporter thing
			overworldMap.ShuffleEntrancesAndFloors(rng, teleporters, flags);
		}

	}
}
