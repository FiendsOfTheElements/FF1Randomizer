using System.ComponentModel;

namespace FF1Lib
{
	public class StartingOverworldItems
	{
		private FF1Rom rom;

		private List<int> addressesToSet;
		public StartingOverworldItems(FF1Rom _rom)
		{
			rom = _rom;
			addressesToSet = new();
		}
		public void Process(List<Item> items)
		{
			foreach (var item in items)
			{
				if (owItemAddresses.TryGetValue(item, out var result))
				{
					addressesToSet.Add(result);
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
				rom.Put(address, new byte[] { 0x01 });
			}
		}
	}
}
