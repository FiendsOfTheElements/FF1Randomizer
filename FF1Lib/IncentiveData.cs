using System.Collections.Generic;
using System.Linq;

namespace FF1Lib
{
	public class IncentiveData
	{
		public IncentiveData(IIncentiveFlags flags)
		{
			var incentivePool = ItemLists.AllQuestItems.ToList();
			incentivePool.Add(Item.Xcalber);
			incentivePool.Add(Item.Masamune);
			incentivePool.Add(Item.Ribbon);
			IncentiveItems = incentivePool;
			var incentiveLocationPool = ItemLocations.AllNPCItemLocations.ToList();
			if (flags.IncentivizeVolcano)
			{
				incentiveLocationPool.Add(ItemLocations.VolcanoMajor);
			}
			if (flags.IncentivizeEarth)
			{
				incentiveLocationPool.Add(ItemLocations.EarthCaveMajor);
			}
			if (flags.IncentivizeMarsh)
			{
				incentiveLocationPool.Add(ItemLocations.MarshCaveMajor);
			}
			if (flags.IncentivizeSeaShrine)
			{
				incentiveLocationPool.Add(ItemLocations.SeaShrineLocked);
			}
			if (flags.IncentivizeConeria)
			{
				incentiveLocationPool.Add(ItemLocations.ConeriaMajor);
			}
			if (flags.IncentivizeIceCave)
			{
				incentiveLocationPool.Add(ItemLocations.IceCaveMajor);
			}
			if (flags.IncentivizeOrdeals)
			{
				incentiveLocationPool.Add(ItemLocations.OrdealsMajor);
			}
			IncentiveLocations = incentiveLocationPool;
		}

		public IEnumerable<IRewardSource> IncentiveLocations { get; }
		public IEnumerable<Item> IncentiveItems { get; }

	}
}
