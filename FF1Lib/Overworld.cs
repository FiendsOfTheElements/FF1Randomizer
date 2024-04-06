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
		private OverworldMap _overworldMap;
		private OwMapExchange _owMapExchange;
		private OwMapExchangeData _owMapExchangeData;
		private OwLocationData _locations;
		private ShipLocations _shipLocations;

		private Flags _flags;
		private FF1Rom _rom;

		public List<List<byte>> DecompressedMap { get => _overworldMap.MapBytes; }
		public Overworld(FF1Rom rom, Flags flags, Settings settings)
		{
			_flags = flags;
			_rom = rom;

			_locations = new OwLocationData(rom);
			_overworldMap = new OverworldMap(rom, flags);
			_locations = new OwLocationData(_rom);
			_shipLocations = OwMapExchange.GetDefaultShipLocations(_rom);
			//_owMapExchange = await OwMapExchange.FromFlags(this, _overworldMap, flags, rng);


		}
		public async void LoadMapExchange(MT19337 rng)
		{
			_owMapExchange = await OwMapExchange.FromFlags(_rom, _overworldMap, _flags, rng);
			_owMapExchange?.ExecuteStep1();
			_shipLocations = _owMapExchange?.ShipLocations ?? OwMapExchange.GetDefaultShipLocations(_rom);
		}
		public void Update(Settings settings)
		{
			_shipLocations.UpdateDocks(settings);
		}


	}
}
