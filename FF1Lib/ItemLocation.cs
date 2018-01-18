using System;
using System.Linq;

namespace FF1Lib
{
    public struct ItemLocation
    {
        public readonly int Address;
        public readonly string Name;
        public readonly MapLocation MapLocation;
        public readonly Item Item;
        public readonly bool IsTreasure;
        public readonly bool RequiresKey;
        public readonly bool RequiresRod;
        public readonly bool RequiresOxyale;
        public readonly bool RequiresCube;
        public readonly bool RequiresBlackOrb;
        public readonly bool RequiresLute;
        public readonly bool IsUnused;
        public string SpoilerText =>
        $"{Name}{string.Join("", Enumerable.Repeat(" ", Math.Max(1, 30 - Name.Length)).ToList())}" +
        $"\t{Enum.GetName(typeof(Item), Item)}";
        public ItemLocation(int address, string name, MapLocation mapLocation, Item item,
                            bool isTreasure = true, bool requiresKey = false,
                            bool requiresRod = false, bool requiresOxyale = false,
                            bool requiresCube = false, bool requiresBlackOrb = false, 
                            bool requiresLute = false, bool isUnused = false)
        {
            Address = address;
            Name = name;
            Item = item;
            MapLocation = mapLocation;
            IsTreasure = isTreasure;
            RequiresKey = requiresKey;
            RequiresRod = requiresRod;
            RequiresOxyale = requiresOxyale;
            RequiresCube = requiresCube;
            RequiresBlackOrb = requiresBlackOrb;
            RequiresLute = requiresLute;
            IsUnused = isUnused;
        }
        public ItemLocation(ItemLocation copyFromItemLocation, Item item)
        {
            Address = copyFromItemLocation.Address;
            Name = copyFromItemLocation.Name;
            Item = item;
            MapLocation = copyFromItemLocation.MapLocation;
            IsTreasure = copyFromItemLocation.IsTreasure;
            RequiresKey = copyFromItemLocation.RequiresKey;
            RequiresRod = copyFromItemLocation.RequiresRod;
            RequiresOxyale = copyFromItemLocation.RequiresOxyale;
            RequiresCube = copyFromItemLocation.RequiresCube;
            RequiresBlackOrb = copyFromItemLocation.RequiresBlackOrb;
            RequiresLute = copyFromItemLocation.RequiresLute;
            IsUnused = copyFromItemLocation.IsUnused;
        }
        public override int GetHashCode() => Address.GetHashCode();
    }
}