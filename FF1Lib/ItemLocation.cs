using System;
using System.Linq;

namespace FF1Lib
{
    [Flags]
    public enum AccessRequirement
    {
        None = 0x00,
        Key = 0x01,
        Rod = 0x02,
        Oxyale = 0x04,
        Cube = 0x08,
        BlackOrb = 0x10,
        Lute = 0x20,
        All = 0xFF
    }
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
                            AccessRequirement accessRequirement = AccessRequirement.None,
                            bool isTreasure = true, bool isUnused = false)
        {
            Address = address;
            Name = name;
            Item = item;
            MapLocation = mapLocation;
            IsTreasure = isTreasure;
            RequiresKey = accessRequirement.HasFlag(AccessRequirement.Key);
            RequiresRod = accessRequirement.HasFlag(AccessRequirement.Rod);
            RequiresOxyale = accessRequirement.HasFlag(AccessRequirement.Oxyale);
            RequiresCube = accessRequirement.HasFlag(AccessRequirement.Cube);
            RequiresBlackOrb = accessRequirement.HasFlag(AccessRequirement.BlackOrb);
            RequiresLute = accessRequirement.HasFlag(AccessRequirement.Lute);
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