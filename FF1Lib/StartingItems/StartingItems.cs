using System.ComponentModel;

namespace FF1Lib
{
	public class StartingItems
	{
		MT19337 rng;
		Flags flags;
		FF1Rom rom;

		Dictionary<Item, int> initialItems;
		private StartingGp startingGp;
		private StartingInventory startingInventory;
		private StartingEquipment startingEquipement;
		private StartingOverworldItems startingOwItems;
		public List<Item> StartingKeyItems { get; private set; }

		public StartingItems(Dictionary<Item, int> startingList, MT19337 _rng, Flags _flags, FF1Rom _rom)
		{
			// Initialize
			rng = _rng;
			flags = _flags;
			rom = _rom;

			initialItems = startingList.ToDictionary(i => i.Key, i => i.Value);

			startingGp = new(flags.StartingGold, rng);
			startingInventory = new(rng, flags, rom);
			startingEquipement = new(rng, flags, rom);
			startingOwItems = new(rom);

			// Parse flags, temporary until native item list
			ParseFlags();

			// Distribute Items to handlers
			startingInventory.Process(initialItems, flags.ExtConsumablesEnabled);
			startingOwItems.Process(initialItems.Where(i => i.Key >= Item.Ship).Select(i => i.Key).ToList(), (bool)flags.IsAirshipFree && (bool)flags.IsShipFree);
			startingEquipement.Process(initialItems, flags.ExtConsumablesEnabled);

			StartingKeyItems = initialItems.Where(i => (i.Key >= Item.Lute && i.Key <= Item.AirOrb) || (i.Key >= Item.Ship)).Select(i => i.Key).ToList();
		}
		public void Write()
		{
			startingGp.Write(rom);
			startingOwItems.Write();
			startingInventory.Write();
			startingEquipement.Write();
		}
		public void ReplaceGear(List<Item> treasurePool)
		{
			startingEquipement.ReplaceTreasures(treasurePool, flags.StartingEquipmentNoDuplicates);
		}

		private void ParseFlags()
		{
			// Free Key Items
			List<(bool flagenabled, Item item)> freeKeyItems = new()
			{
				((bool)flags.FreeLute, Item.Lute),
				((bool)flags.FreeRod, Item.Rod),
				((bool)flags.AirBoat && (bool)flags.IsAirshipFree, Item.Floater),
				((bool)flags.FreeTail && !(bool)flags.NoTail, Item.Tail),
			};

			List<(bool flagenabled, Item item)> freeOwItems = new()
			{
				((bool)flags.IsAirshipFree, Item.Airship),
				((bool)flags.IsBridgeFree && (!flags.DesertOfDeath), Item.Bridge),
				((bool)flags.IsCanalFree, Item.Canal),
				((bool)flags.IsCanoeFree, Item.Canoe),
				((bool)flags.IsShipFree, Item.Ship),
			};

			freeKeyItems.Where(i => i.flagenabled).ToList().ForEach(i => initialItems.Add(i.item, 1));
			freeOwItems.Where(i => i.flagenabled).ToList().ForEach(i => initialItems.Add(i.item, 1));

			// Starting Inventory
			startingInventory.ReturnStartingInventory().ForEach(i => initialItems.Add(i.item, i.qty));

			// Starting Gear
			startingEquipement.GetStartingEquipment().ToList().ForEach(i => initialItems.Add(i.Key, i.Value));
		}
	}
}
