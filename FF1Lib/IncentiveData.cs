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
            incentivePool.Remove(Item.Bridge);
            incentivePool.Remove(Item.Ship);
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
            if (!flags.ForceVanillaNPCs)
            {
                if (flags.EarlyCanoe)
                {
                    incentiveLocationPool =
                            incentiveLocationPool
                                .Select(x => x.Address == ItemLocations.CanoeSage.Address
                                        ? new MapObject(ObjectId.CanoeSage, MapLocation.CresentLake, Item.Canoe)
                                        : x).ToList();
                }
                if (flags.EarlyRod)
                {
                    incentiveLocationPool =
                        incentiveLocationPool
                            .Select(x => x.Address == ItemLocations.Sarda.Address
                                    ? new MapObject(ObjectId.Sarda, MapLocation.SardasCave, Item.Rod)
                                    : x).ToList();
                }
            }
            IncentiveLocations = incentiveLocationPool;
        }

        public IEnumerable<IRewardSource> IncentiveLocations { get; }
        public IEnumerable<Item> IncentiveItems { get; }

    }
}
