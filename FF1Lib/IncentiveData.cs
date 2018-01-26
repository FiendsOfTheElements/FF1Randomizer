using System.Collections.Generic;
using System.Linq;

namespace FF1Lib
{
    public class IncentiveData
    {
        public IncentiveData(ITreasureShuffleFlags flags)
        {
            var incentivePool = ItemLists.AllQuestItems.ToList();
            incentivePool.Add(Item.Xcalber);
            incentivePool.Add(Item.Masamune);
            incentivePool.Add(Item.Ribbon);
            incentivePool.Remove(Item.Ship);
            IncentiveItems = incentivePool;
            var incentiveLocationPool = ItemLocations.AllNPCItemLocations.ToList();
            if (flags.IncentivizeIceCave)
            {
                incentiveLocationPool.Add(ItemLocations.IceCaveMajor);
            }
            if (flags.IncentivizeOrdeals)
            {
                if (!flags.EarlyOrdeals)
                {
                    incentiveLocationPool.Add(ItemLocations.OrdealsMajor);
                }
                else
                {
                    incentiveLocationPool.Add(
                        new TreasureChest(ItemLocations.OrdealsMajor,
                                          Item.Tail,
                                          ItemLocations.OrdealsMajor.AccessRequirement & ~AccessRequirement.Crown));
                }
            }
            if (flags.EarlyRod && !flags.ForceVanillaNPCs)
            {
                incentiveLocationPool =
                    incentiveLocationPool
                        .Select(x => x.Address == ItemLocations.Sarda.Address
                                ? new MapObject(ObjectId.Sarda, MapLocation.SardasCave, Item.Rod)
                                : x).ToList();
            }
            IncentiveLocations = incentiveLocationPool;
        }

        public IEnumerable<IRewardSource> IncentiveLocations { get; }
        public IEnumerable<Item> IncentiveItems { get; }

    }
}
