using System;
using System.Collections.Generic;
using System.Linq;

namespace FF1Lib
{
    // These aren't actual values from the game code, just placeholders
    public enum MapLocations
    {
        StartingLocation,
        ConeriaTown,
        ConeriaCastle,
        TempleOfFiends,
        MatoyasCave,
        Pravoka,
        DwarfCave,
        ElflandTown,
        ElflandCastle,
        NorthwestCastle,
        MarshCave,
        Melmond,
        EarthCave,
        TitansTunnelA,
        TitansTunnelB,
        SardasCave,
        CresentLake,
        GurguVolcano,
        IceCave,
        CastleOrdeals,
        Cardia1,
        Cardia2,
        Cardia3,
        Cardia4,
        Cardia5,
        Cardia6,
        Caravan,
        Gaia,
        Onrac,
        Waterfall,
        Lefein,
        MirageTower,
        BridgeLocation,
        ShipLocation,
        CanalLocation,
        AirshipLocation
    }
    public enum MapChanges
    {
        Bridge, // NPC Only
        Ship, // NPC Only
        Canal, // NPC Only. As a general rule we can assume the ship cannot require the canal
        TitanFed,
        Canoe,
        Airship
    }
    public class MapDetails
    {
        private static bool Bridge(IReadOnlyCollection<MapChanges> x) => x.Contains(MapChanges.Bridge);
        private static bool Ship(IReadOnlyCollection<MapChanges> x) => x.Contains(MapChanges.Ship);
        private static bool Canoe(IReadOnlyCollection<MapChanges> x) => x.Contains(MapChanges.Canoe);
        private static bool Airship(IReadOnlyCollection<MapChanges> x) => x.Contains(MapChanges.Airship);
        private static bool Canal(IReadOnlyCollection<MapChanges> x) => x.Contains(MapChanges.Canal) && Ship(x);
        private static bool NormalTitanFedOrAirship(IReadOnlyCollection<MapChanges> x) => Airship(x) || (Canal(x) && x.Contains(MapChanges.TitanFed));
        private static bool ShipAndCanoe(IReadOnlyCollection<MapChanges> x) => Ship(x) && Canoe(x);
        private static bool AirshipAndCanoe(IReadOnlyCollection<MapChanges> x) => Canoe(x) && Airship(x);
        private static bool CanalOrAirship(IReadOnlyCollection<MapChanges> x) => Canal(x) || Airship(x);
        private static bool BridgeOrShip(IReadOnlyCollection<MapChanges> x) => Bridge(x) || Ship(x);
        public static readonly IReadOnlyDictionary<MapLocations, Func<IReadOnlyCollection<MapChanges>, bool>> DefaultMapRequirements =
            new Dictionary<MapLocations, Func<IReadOnlyCollection<MapChanges>, bool>>
            {
            {MapLocations.StartingLocation, x => true},
            {MapLocations.ConeriaTown, x => true},
            {MapLocations.ConeriaCastle, x => true},
            {MapLocations.TempleOfFiends, x => true},
            {MapLocations.MatoyasCave, BridgeOrShip},
            {MapLocations.Pravoka, BridgeOrShip},
            {MapLocations.DwarfCave, Ship},
            {MapLocations.ElflandTown, Ship},
            {MapLocations.ElflandCastle, Ship},
            {MapLocations.NorthwestCastle, Ship},
            {MapLocations.MarshCave, Ship},
            {MapLocations.Melmond, CanalOrAirship},
            {MapLocations.EarthCave, CanalOrAirship},
            {MapLocations.TitansTunnelA, CanalOrAirship},
            {MapLocations.TitansTunnelB, NormalTitanFedOrAirship},
            {MapLocations.SardasCave, NormalTitanFedOrAirship},
            {MapLocations.CresentLake, x => Canal(x)}, // || ShipAndCanoe(x)}, CanoeSage was getting Canal too often
            {MapLocations.GurguVolcano, ShipAndCanoe},
            {MapLocations.IceCave, x => BridgeOrShip(x) && Canoe(x)},
            {MapLocations.CastleOrdeals, x => Airship(x) || (Canal(x) && Canoe(x))},
            {MapLocations.Cardia1, Airship},
            {MapLocations.Cardia2, Airship},
            {MapLocations.Cardia3, Airship},
            {MapLocations.Cardia4, Airship},
            {MapLocations.Cardia5, Airship},
            {MapLocations.Cardia6, Airship},
            {MapLocations.Caravan, Airship},
            {MapLocations.Gaia, Airship},
            {MapLocations.Onrac, AirshipAndCanoe},
            {MapLocations.Waterfall, AirshipAndCanoe},
            {MapLocations.Lefein, Airship},
            {MapLocations.MirageTower, Airship},

            {MapLocations.BridgeLocation, x => true},
            {MapLocations.ShipLocation, Bridge},
            {MapLocations.CanalLocation, Ship},
            {MapLocations.AirshipLocation, ShipAndCanoe}
        };

        private IReadOnlyCollection<MapChanges> requiredMapChanges;
        public IReadOnlyCollection<MapChanges> RequiredMapChanges
        {
            get
            {
                if (requiredMapChanges != null) return requiredMapChanges;

                var result = new List<MapChanges>();
                var locationsRequiringChanges = LocationsRequired.Where(x => !LocationRequirements[x](new List<MapChanges>()));
                var allMapChanges = (Enum.GetValues(typeof(MapChanges)) as MapChanges[]).ToList();
                // This is a bit brute force but it should catch all the required map changes
                for (var i = 0; i < allMapChanges.Count(); i ++) {
                    var mapChangesPossiblyRequired = allMapChanges.Except(result);
                    foreach (var location in locationsRequiringChanges)
                    {
                        foreach (var mc1 in mapChangesPossiblyRequired)
                        {
                            var except1 = mapChangesPossiblyRequired.Where(x => x != mc1).Concat(result).ToList();
                            if (LocationRequirements[location](except1)) continue;
                            result.Add(mc1);
                            break;
                        }
                    }
                }
                requiredMapChanges = result;
                return requiredMapChanges;
            }
        }

        public readonly IReadOnlyDictionary<MapLocations, Func<IReadOnlyCollection<MapChanges>, bool>> LocationRequirements;
        public readonly IReadOnlyCollection<MapLocations> LocationsRequired;
        public readonly IReadOnlyCollection<byte> ItemsRequired;
        // These switches can be used to indicate that an item is not needed if another items is available
        public bool ShipObsoletesBridge = false; 
        //public bool AirshipObsoletesTitanFed = false;

        public MapDetails(IReadOnlyCollection<byte> itemsRequired, 
                          IReadOnlyCollection<MapLocations> locationsRequired, 
                          IReadOnlyDictionary<MapLocations, Func<IReadOnlyCollection<MapChanges>, bool>> locationRequirements)
        {
            ItemsRequired = itemsRequired;
            LocationsRequired = locationsRequired;
            LocationRequirements = locationRequirements ?? DefaultMapRequirements;
        }
    }
}
