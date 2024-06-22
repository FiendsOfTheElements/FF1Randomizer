using System.ComponentModel;

namespace FF1Lib
{
	public class StartingOverworldItems
	{
		private FF1Rom rom;
		private byte ShipValue;
		private List<(int address, byte value)> addressesToSet;
		public StartingOverworldItems(FF1Rom _rom)
		{
			rom = _rom;
			addressesToSet = new();
			ShipValue = 0x01;
		}
		public void Process(List<Item> items, bool freeairboat)
		{
			if (freeairboat)
			{
				ShipValue = 0x81;
			}

			foreach (var item in items)
			{
				if (owItemAddresses.TryGetValue(item, out var result))
				{
					addressesToSet.Add((result, (item == Item.Ship) ? ShipValue : (byte)0x01));
				}
			}
		}
		private static Dictionary<Item, int> owItemAddresses = new()
		{
			{ Item.Bridge, 0x3008 },
			{ Item.Ship, 0x3000 },
			{ Item.Canal, 0x300C },
			{ Item.Canoe, 0x3012 },
			{ Item.Airship, 0x3004 },
		};
		public void Write()
		{
			foreach (var address in addressesToSet)
			{
				rom.Put(address.address, new byte[] { address.value });
			}
		}
	}
}
