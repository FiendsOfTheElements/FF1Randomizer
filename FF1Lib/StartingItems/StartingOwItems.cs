using System.ComponentModel;

namespace FF1Lib
{
	public class StartingOverworldItems
	{
		private FF1Rom rom;
		private List<(int address, byte value)> addressesToSet;
		public StartingOverworldItems(FF1Rom _rom)
		{
			rom = _rom;
			addressesToSet = new();
		}
		public void Process(List<Item> items, bool freeairboat)
		{
			if (freeairboat)
			{
				owItemAddresses[Item.Ship] = (owItemAddresses[Item.Ship].address, 0x81);
			}

			foreach (var item in items)
			{
				if (owItemAddresses.TryGetValue(item, out var result))
				{
					addressesToSet.Add(result);
				}
			}
		}
		private Dictionary<Item, (int address, byte value)> owItemAddresses = new()
		{
			{ Item.Bridge, (0x3008, 0x01) },
			{ Item.Ship, (0x3000, 0x01) },
			{ Item.Canal, (0x300C, 0x00) },
			{ Item.Canoe, (0x3012, 0x01) },
			{ Item.Airship, (0x3004, 0x01) },
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
