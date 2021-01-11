using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public enum StartingItemCount
	{
		[Description("0")]
		None,

		[Description("1")]
		One,

		[Description("2")]
		Two,

		[Description("5")]
		Five,

		[Description("10")]
		Ten,

		[Description("25")]
		TwentyFive,

		[Description("50")]
		Fifty,

		[Description("99")]
		NinetyNine,

		[Description("0 - 10")]
		Random0To10,

		[Description("0 - 25")]
		Random0To25,

		[Description("0 - 99")]
		Random0To99,

		[Description("10 - 50")]
		Random10To50,

		[Description("10 - 99")]
		Random10To99
	}

	public class StartingInventory
	{
		MT19337 rng;
		Flags flags;
		FF1Rom rom;

		StartingItems ItemData;

		public StartingInventory(MT19337 _rng, Flags _flags, FF1Rom _rom)
		{
			rng = _rng;
			flags = _flags;
			rom = _rom;

			ItemData = new StartingItems(rom);
		}

		public void SetStartingInventory()
		{
			ItemData.LoadTable();

			ItemData[Item.Tent] = GetItemCount(flags.StartingInventory_Tent);
			ItemData[Item.Cabin] = GetItemCount(flags.StartingInventory_Cabin);
			ItemData[Item.House] = GetItemCount(flags.StartingInventory_House);
			ItemData[Item.Heal] = GetItemCount(flags.StartingInventory_Heal);
			ItemData[Item.Pure] = GetItemCount(flags.StartingInventory_Pure);
			ItemData[Item.Soft] = GetItemCount(flags.StartingInventory_Soft);

			ItemData.StoreTable();
		}

		private byte GetItemCount(StartingItemCount count)
		{
			switch (count)
			{
				case StartingItemCount.None: return 0;
				case StartingItemCount.One: return 1;
				case StartingItemCount.Two: return 2;
				case StartingItemCount.Five: return 5;
				case StartingItemCount.Ten: return 10;
				case StartingItemCount.TwentyFive: return 25;
				case StartingItemCount.Fifty: return 50;
				case StartingItemCount.NinetyNine: return 99;
				case StartingItemCount.Random0To10: return (byte)rng.Between(0, 10);
				case StartingItemCount.Random0To25: return (byte)rng.Between(0, 25);
				case StartingItemCount.Random0To99: return (byte)rng.Between(0, 99);
				case StartingItemCount.Random10To50: return (byte)rng.Between(10, 50);
				case StartingItemCount.Random10To99: return (byte)rng.Between(10, 99);
				default: return 0;
			}
		}
	}
}
