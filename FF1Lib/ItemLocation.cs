using System;
using System.Linq;

namespace FF1Lib
{
    public struct ItemLocation
    {
        public readonly int Address;
        public readonly string Name;
        public readonly MapLocations MapLocation;
        public readonly byte Item;
        public readonly bool IsTreasure;
        public readonly bool UpdatesVariable;
        public readonly bool RequiresKey;
        public readonly bool RequiresRod;
        public readonly bool RequiresOxyale;
        public readonly bool RequiresChime;
        public readonly bool RequiresCube;
        public readonly bool RequiresBlackOrb;
        public readonly bool RequiresLute;
        public readonly bool IsUnused;
        public string SpoilerText =>
        $"{Name}{string.Join("", Enumerable.Repeat(" ", Math.Max(1, 30 - Name.Length)).ToList())}" +
        $"\t{(UpdatesVariable ? $"Visibility {Item}" : Items.TextLookup[Item])}";
        public ItemLocation(int address, string name, MapLocations mapLocation, byte item,
                            bool isTreasure = true, bool updatesVariable = false, bool requiresKey = false,
                            bool requiresRod = false, bool requiresOxyale = false,
                            bool requiresChime = false, bool requiresCube = false,
                            bool requiresBlackOrb = false, bool requiresLute = false, 
                            bool isUnused = false)
        {
            Address = address;
            Name = name;
            Item = item;
            MapLocation = mapLocation;
            UpdatesVariable = updatesVariable;
            IsTreasure = isTreasure;
            RequiresKey = requiresKey;
            RequiresRod = requiresRod;
            RequiresOxyale = requiresOxyale;
            RequiresChime = requiresChime;
            RequiresCube = requiresCube;
            RequiresBlackOrb = requiresBlackOrb;
            RequiresLute = requiresLute;
            IsUnused = isUnused;
            if (IsTreasure && UpdatesVariable)
                throw new InvalidOperationException(
                    $"Invalid argument combination {nameof(isTreasure)}: {isTreasure}, " +
                    $"{nameof(updatesVariable)}: {updatesVariable}. Only NPCs can update variables.");
        }
        public ItemLocation(ItemLocation copyFromItemLocation, byte item, bool updatesVariable = false)
        {
            Address = copyFromItemLocation.Address;
            Name = copyFromItemLocation.Name;
            Item = item;
            MapLocation = copyFromItemLocation.MapLocation;
            UpdatesVariable = updatesVariable;
            IsTreasure = copyFromItemLocation.IsTreasure;
            RequiresKey = copyFromItemLocation.RequiresKey;
            RequiresRod = copyFromItemLocation.RequiresRod;
            RequiresOxyale = copyFromItemLocation.RequiresOxyale;
            RequiresChime = copyFromItemLocation.RequiresChime;
            RequiresCube = copyFromItemLocation.RequiresCube;
            RequiresBlackOrb = copyFromItemLocation.RequiresBlackOrb;
            RequiresLute = copyFromItemLocation.RequiresLute;
            IsUnused = copyFromItemLocation.IsUnused;
            if (IsTreasure && UpdatesVariable)
                throw new InvalidOperationException(
                    $"Invalid argument combination {nameof(copyFromItemLocation.IsTreasure)}: {copyFromItemLocation.IsTreasure}, " +
                    $"{nameof(updatesVariable)}: {updatesVariable}. Only NPCs can update variables.");
        }
        public override int GetHashCode() => Address.GetHashCode();
    }
}