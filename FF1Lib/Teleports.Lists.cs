using System.Collections.Generic;
using System.Linq;

namespace FF1Lib
{
	public static class TeleportShuffle
	{
		public static TeleportDestination Coneria => new TeleportDestination(MapLocation.Coneria, MapIndex.ConeriaTown, new Coordinate(16, 23, CoordinateLocale.Standard));
		public static TeleportDestination Pravoka => new TeleportDestination(MapLocation.Pravoka, MapIndex.Pravoka, new Coordinate(19, 32, CoordinateLocale.Standard));
		public static TeleportDestination Elfland => new TeleportDestination(MapLocation.Elfland, MapIndex.Elfland, new Coordinate(41, 22, CoordinateLocale.Standard));
		public static TeleportDestination Melmond => new TeleportDestination(MapLocation.Melmond, MapIndex.Melmond, new Coordinate(1, 16, CoordinateLocale.Standard));
		public static TeleportDestination CrescentLake => new TeleportDestination(MapLocation.CrescentLake, MapIndex.CrescentLake, new Coordinate(11, 23, CoordinateLocale.Standard));
		public static TeleportDestination Gaia => new TeleportDestination(MapLocation.Gaia, MapIndex.Gaia, new Coordinate(61, 61, CoordinateLocale.Standard));
		public static TeleportDestination Onrac => new TeleportDestination(MapLocation.Onrac, MapIndex.Onrac, new Coordinate(1, 12, CoordinateLocale.Standard), TeleportIndex.SeaShrine1);
		public static TeleportDestination Lefein => new TeleportDestination(MapLocation.Lefein, MapIndex.Lefein, new Coordinate(19, 23, CoordinateLocale.Standard));
		public static TeleportDestination ConeriaCastle1 => new TeleportDestination(MapLocation.ConeriaCastle1, MapIndex.ConeriaCastle1F, new Coordinate(12, 35, CoordinateLocale.Standard), ExitTeleportIndex.ExitCastleConeria);
		public static TeleportDestination ConeriaCastle2 => new TeleportDestination(MapLocation.ConeriaCastle2, MapIndex.ConeriaCastle2F, new Coordinate(12, 18, CoordinateLocale.Standard)); // Could be used if the teleporter here is turned into warp stairs
		public static TeleportDestination ElflandCastle => new TeleportDestination(MapLocation.ElflandCastle, MapIndex.ElflandCastle, new Coordinate(16, 31, CoordinateLocale.Standard));
		public static TeleportDestination NorthwestCastle => new TeleportDestination(MapLocation.NorthwestCastle, MapIndex.NorthwestCastle, new Coordinate(22, 24, CoordinateLocale.Standard));
		public static TeleportDestination CastleOrdeals1 => new TeleportDestination(MapLocation.CastleOrdeals1, MapIndex.CastleOrdeals1F, new Coordinate(12, 21, CoordinateLocale.Standard), ExitTeleportIndex.ExitCastleOrdeals);
		public static TeleportDestination CastleOrdealsMaze => new TeleportDestination(MapLocation.CastleOrdealsMaze, MapIndex.CastleOrdeals2F, new Coordinate(12, 12, CoordinateLocale.Standard), TeleportIndex.CastleOrdealsTop);
		public static TeleportDestination CastleOrdealsTop => new TeleportDestination(MapLocation.CastleOrdealsTop, MapIndex.CastleOrdeals3F, new Coordinate(22, 22, CoordinateLocale.Standard), TeleportIndex.CastleOrdealsBack);
		public static TeleportDestination TempleOfFiends => new TeleportDestination(MapLocation.TempleOfFiends1, MapIndex.TempleOfFiends, new Coordinate(20, 30, CoordinateLocale.Standard));
		public static TeleportDestination DwarfCave => new TeleportDestination(MapLocation.DwarfCave, MapIndex.DwarfCave, new Coordinate(22, 11, CoordinateLocale.Standard));
		public static TeleportDestination MatoyasCave => new TeleportDestination(MapLocation.MatoyasCave, MapIndex.MatoyasCave, new Coordinate(15, 11, CoordinateLocale.Standard));
		public static TeleportDestination SardasCave => new TeleportDestination(MapLocation.SardasCave, MapIndex.SardasCave, new Coordinate(18, 13, CoordinateLocale.Standard));
		public static TeleportDestination Cardia1 => new TeleportDestination(MapLocation.Cardia1, MapIndex.Cardia, new Coordinate(30, 18, CoordinateLocale.Standard));
		public static TeleportDestination Cardia2 => new TeleportDestination(MapLocation.Cardia2, MapIndex.Cardia, new Coordinate(12, 15, CoordinateLocale.Standard));
		public static TeleportDestination BahamutCave1 => new TeleportDestination(MapLocation.BahamutCave1, MapIndex.BahamutCaveB1, new Coordinate(2, 2, CoordinateLocale.Standard), TeleportIndex.BahamutsRoom);
		public static TeleportDestination BahamutsRoom => new TeleportDestination(MapLocation.BahamutCave2, MapIndex.BahamutCaveB2, new Coordinate(23, 55, CoordinateLocale.Standard));
		public static TeleportDestination Cardia4 => new TeleportDestination(MapLocation.Cardia4, MapIndex.Cardia, new Coordinate(19, 36, CoordinateLocale.Standard));
		public static TeleportDestination Cardia5 => new TeleportDestination(MapLocation.Cardia5, MapIndex.Cardia, new Coordinate(43, 29, CoordinateLocale.Standard));
		public static TeleportDestination Cardia6 => new TeleportDestination(MapLocation.Cardia6, MapIndex.Cardia, new Coordinate(58, 55, CoordinateLocale.Standard));
		public static TeleportDestination IceCave1 => new TeleportDestination(MapLocation.IceCave1, MapIndex.IceCaveB1, new Coordinate(7, 1, CoordinateLocale.Standard), TeleportIndex.IceCave2);
		public static TeleportDestination IceCave2 => new TeleportDestination(MapLocation.IceCave2, MapIndex.IceCaveB2, new Coordinate(30, 2, CoordinateLocale.Standard), TeleportIndex.IceCave3);
		public static TeleportDestination IceCave3 => new TeleportDestination(MapLocation.IceCave3, MapIndex.IceCaveB3, new Coordinate(3, 2, CoordinateLocale.Standard), TeleportIndex.IceCavePitRoom);
		public static TeleportDestination IceCavePitRoom => new TeleportDestination(MapLocation.IceCavePitRoom, MapIndex.IceCaveB2, new Coordinate(55, 5, CoordinateLocale.Standard), ExitTeleportIndex.ExitIceCave);
		public static TeleportDestination Waterfall => new TeleportDestination(MapLocation.Waterfall, MapIndex.Waterfall, new Coordinate(57, 56, CoordinateLocale.Standard));
		public static TeleportDestination TitansTunnelEast => new TeleportDestination(MapLocation.TitansTunnelEast, MapIndex.TitansTunnel, new Coordinate(11, 14, CoordinateLocale.Standard), ExitTeleportIndex.ExitTitanE);
		public static TeleportDestination TitansTunnelWest => new TeleportDestination(MapLocation.TitansTunnelWest, MapIndex.TitansTunnel, new Coordinate(5, 3, CoordinateLocale.Standard), ExitTeleportIndex.ExitTitanW);
		public static TeleportDestination EarthCave1 => new TeleportDestination(MapLocation.EarthCave1, MapIndex.EarthCaveB1, new Coordinate(23, 24, CoordinateLocale.Standard), TeleportIndex.EarthCave2);
		public static TeleportDestination EarthCave2 => new TeleportDestination(MapLocation.EarthCave2, MapIndex.EarthCaveB2, new Coordinate(10, 9, CoordinateLocale.Standard), TeleportIndex.EarthCaveVampire);
		public static TeleportDestination EarthCaveVampire => new TeleportDestination(MapLocation.EarthCaveVampire, MapIndex.EarthCaveB3, new Coordinate(27, 45, CoordinateLocale.Standard), TeleportIndex.EarthCave4);
		public static TeleportDestination EarthCave4 => new TeleportDestination(MapLocation.EarthCave4, MapIndex.EarthCaveB4, new Coordinate(61, 33, CoordinateLocale.Standard), TeleportIndex.EarthCaveLich);
		public static TeleportDestination EarthCaveLich => new TeleportDestination(MapLocation.EarthCaveLich, MapIndex.EarthCaveB5, new Coordinate(25, 53, CoordinateLocale.Standard), ExitTeleportIndex.ExitEarthCave);
		public static TeleportDestination GurguVolcano1 => new TeleportDestination(MapLocation.GurguVolcano1, MapIndex.GurguVolcanoB1, new Coordinate(27, 15, CoordinateLocale.Standard), TeleportIndex.GurguVolcano2);
		public static TeleportDestination GurguVolcano2 => new TeleportDestination(MapLocation.GurguVolcano2, MapIndex.GurguVolcanoB2, new Coordinate(30, 32, CoordinateLocale.Standard), TeleportIndex.GurguVolcano3);
		public static TeleportDestination GurguVolcano3 => new TeleportDestination(MapLocation.GurguVolcano3, MapIndex.GurguVolcanoB3, new Coordinate(18, 2, CoordinateLocale.Standard), TeleportIndex.GurguVolcano4);
		public static TeleportDestination GurguVolcano4 => new TeleportDestination(MapLocation.GurguVolcano4, MapIndex.GurguVolcanoB4, new Coordinate(3, 23, CoordinateLocale.Standard), TeleportIndex.GurguVolcano5);
		public static TeleportDestination GurguVolcano5 => new TeleportDestination(MapLocation.GurguVolcano5, MapIndex.GurguVolcanoB3, new Coordinate(46, 23, CoordinateLocale.Standard), TeleportIndex.GurguVolcano6);
		public static TeleportDestination GurguVolcano6 => new TeleportDestination(MapLocation.GurguVolcano6, MapIndex.GurguVolcanoB4, new Coordinate(35, 6, CoordinateLocale.Standard), TeleportIndex.GurguVolcanoKary);
		public static TeleportDestination GurguVolcanoKary => new TeleportDestination(MapLocation.GurguVolcanoKary, MapIndex.GurguVolcanoB5, new Coordinate(32, 31, CoordinateLocale.Standard), ExitTeleportIndex.ExitGurguVolcano);
		public static TeleportDestination MarshCave1 => new TeleportDestination(MapLocation.MarshCave1, MapIndex.MarshCaveB1, new Coordinate(21, 27, CoordinateLocale.Standard), new List<TeleportIndex> { TeleportIndex.MarshCaveTop, TeleportIndex.MarshCave3 });
		public static TeleportDestination MarshCaveTop => new TeleportDestination(MapLocation.MarshCaveTop, MapIndex.MarshCaveB2, new Coordinate(18, 16, CoordinateLocale.Standard));
		public static TeleportDestination MarshCave3 => new TeleportDestination(MapLocation.MarshCave3, MapIndex.MarshCaveB2, new Coordinate(34, 37, CoordinateLocale.StandardInRoom), TeleportIndex.MarshCaveBottom);
		public static TeleportDestination MarshCaveBottom => new TeleportDestination(MapLocation.MarshCaveBottom, MapIndex.MarshCaveB3, new Coordinate(5, 6, CoordinateLocale.Standard));
		public static TeleportDestination MirageTower1 => new TeleportDestination(MapLocation.MirageTower1, MapIndex.MirageTower1F, new Coordinate(17, 31, CoordinateLocale.Standard), TeleportIndex.MirageTower2);
		public static TeleportDestination MirageTower2 => new TeleportDestination(MapLocation.MirageTower2, MapIndex.MirageTower2F, new Coordinate(16, 31, CoordinateLocale.Standard), TeleportIndex.MirageTower3);
		public static TeleportDestination MirageTower3 => new TeleportDestination(MapLocation.MirageTower3, MapIndex.MirageTower3F, new Coordinate(8, 1, CoordinateLocale.Standard), TeleportIndex.SkyPalace1);
		public static TeleportDestination SeaShrineMermaids => new TeleportDestination(MapLocation.SeaShrineMermaids, MapIndex.SeaShrineB1, new Coordinate(12, 26, CoordinateLocale.Standard));
		public static TeleportDestination SeaShrine2 => new TeleportDestination(MapLocation.SeaShrine2, MapIndex.SeaShrineB2, new Coordinate(45, 8, CoordinateLocale.Standard), TeleportIndex.SeaShrineMermaids);
		public static TeleportDestination SeaShrine1 => new TeleportDestination(MapLocation.SeaShrine1, MapIndex.SeaShrineB3, new Coordinate(21, 42, CoordinateLocale.Standard), new List<TeleportIndex> { TeleportIndex.SeaShrine2, TeleportIndex.SeaShrine4 });
		public static TeleportDestination SeaShrine4 => new TeleportDestination(MapLocation.SeaShrine4, MapIndex.SeaShrineB4, new Coordinate(61, 49, CoordinateLocale.Standard), TeleportIndex.SeaShrine5);
		public static TeleportDestination SeaShrine5 => new TeleportDestination(MapLocation.SeaShrine5, MapIndex.SeaShrineB3, new Coordinate(47, 39, CoordinateLocale.Standard), TeleportIndex.SeaShrine6);
		public static TeleportDestination SeaShrine6 => new TeleportDestination(MapLocation.SeaShrine6, MapIndex.SeaShrineB2, new Coordinate(54, 41, CoordinateLocale.Standard), TeleportIndex.SeaShrine7);
		public static TeleportDestination SeaShrine7 => new TeleportDestination(MapLocation.SeaShrine7, MapIndex.SeaShrineB3, new Coordinate(48, 10, CoordinateLocale.Standard), TeleportIndex.SeaShrine8);
		public static TeleportDestination SeaShrine8 => new TeleportDestination(MapLocation.SeaShrine8, MapIndex.SeaShrineB4, new Coordinate(45, 20, CoordinateLocale.Standard), TeleportIndex.SeaShrineKraken);
		public static TeleportDestination SeaShrineKraken => new TeleportDestination(MapLocation.SeaShrineKraken, MapIndex.SeaShrineB5, new Coordinate(50, 48, CoordinateLocale.Standard), ExitTeleportIndex.ExitSeaShrine);
		public static TeleportDestination SkyPalace1 => new TeleportDestination(MapLocation.SkyPalace1, MapIndex.SkyPalace1F, new Coordinate(19, 21, CoordinateLocale.StandardInRoom), TeleportIndex.SkyPalace2); // X high bit means inroom
		public static TeleportDestination SkyPalace2 => new TeleportDestination(MapLocation.SkyPalace2, MapIndex.SkyPalace2F, new Coordinate(19, 4, CoordinateLocale.Standard), TeleportIndex.SkyPalace3);
		public static TeleportDestination SkyPalace3 => new TeleportDestination(MapLocation.SkyPalace3, MapIndex.SkyPalace3F, new Coordinate(24, 23, CoordinateLocale.Standard), TeleportIndex.SkyPalaceMaze);
		public static TeleportDestination SkyPalaceMaze => new TeleportDestination(MapLocation.SkyPalaceMaze, MapIndex.SkyPalace4F, new Coordinate(3, 3, CoordinateLocale.Standard), TeleportIndex.SkyPalaceTiamat);
		public static TeleportDestination SkyPalaceTiamat => new TeleportDestination(MapLocation.SkyPalaceTiamat, MapIndex.SkyPalace5F, new Coordinate(7, 54, CoordinateLocale.Standard), ExitTeleportIndex.ExitSkyPalace);
		public static List<MapIndex> InRoomMaps = new List<MapIndex> { MapIndex.SkyPalace1F, MapIndex.MarshCaveB3, MapIndex.CastleOrdeals2F };
		public static Dictionary<TeleportIndex, AccessRequirement> TeleportRestrictions =
			new Dictionary<TeleportIndex, AccessRequirement>
			{
				{TeleportIndex.CastleOrdealsMaze, AccessRequirement.Crown},
				{TeleportIndex.SkyPalace1, AccessRequirement.Cube},
				{TeleportIndex.SeaShrine1, AccessRequirement.Oxyale},
				{TeleportIndex.EarthCave4, AccessRequirement.Rod},
				{TeleportIndex.TempleOfFiends2, AccessRequirement.BlackOrb}, // needs to be verified
				{TeleportIndex.TempleOfFiends4, AccessRequirement.Lute} // needs to be verified
			};

		public static List<TeleportDestination> NonTownForcedTopFloors =>
		 	new List<TeleportDestination>
			{
				TitansTunnelEast, TitansTunnelWest, ConeriaCastle1, CastleOrdeals1
			};
		public static List<TeleportDestination> TownTeleports =>
		 	new List<TeleportDestination>
			{
				Coneria, Pravoka, Elfland, Melmond, CrescentLake, Gaia, Onrac, Lefein
			};
		public static List<TeleportDestination> ForcedTopFloors =>
		 	TownTeleports.Concat(NonTownForcedTopFloors).ToList();
		public static List<TeleportDestination> FreePlacementFloors =>
			 new List<TeleportDestination>
			{
				ElflandCastle, NorthwestCastle, TempleOfFiends,
				DwarfCave, MatoyasCave, SardasCave, Cardia1, Cardia2, BahamutCave1, BahamutsRoom, Cardia4, Cardia5, Cardia6,
				IceCave1, IceCave2, IceCave3, IceCavePitRoom,
				Waterfall,
				EarthCave1, EarthCave2, EarthCaveVampire, EarthCave4, EarthCaveLich,
				GurguVolcano1, GurguVolcano2, GurguVolcano3, GurguVolcano4, GurguVolcano5, GurguVolcano6, GurguVolcanoKary,
				MarshCave1, MarshCaveTop, MarshCave3, MarshCaveBottom,
				MirageTower1, MirageTower2, MirageTower3,
				SeaShrineMermaids, SeaShrine2, SeaShrine1, SeaShrine4, SeaShrine5, SeaShrine6, SeaShrine7, SeaShrine8, SeaShrineKraken,
				SkyPalace1, SkyPalace2, SkyPalace3, SkyPalaceMaze, SkyPalaceTiamat
			};
		public static List<TeleportDestination> FloorTeleports =>
		 	ForcedTopFloors.Concat(FreePlacementFloors).ToList();

		public static Dictionary<OverworldTeleportIndex, Coordinate> OverworldCoordinates =>
			new Dictionary<OverworldTeleportIndex, Coordinate>
			{
				{OverworldTeleportIndex.Coneria,new Coordinate(152, 162, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.Pravoka,new Coordinate(210, 150, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.Elfland,new Coordinate(136, 222, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.Melmond,new Coordinate(81, 160, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.CrescentLake,new Coordinate(219, 218, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.Gaia,new Coordinate(221, 28, CoordinateLocale.Overworld)}, // requires airship
				{OverworldTeleportIndex.Onrac,new Coordinate(62, 56, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.Lefein,new Coordinate(235, 99, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.ConeriaCastle1,new Coordinate(153, 159, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.ElflandCastle,new Coordinate(136, 221, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.NorthwestCastle,new Coordinate(103, 186, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.CastleOrdeals1,new Coordinate(130, 45, CoordinateLocale.Overworld)}, // requires canoe
				{OverworldTeleportIndex.TempleOfFiends1,new Coordinate(130, 123, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.EarthCave1,new Coordinate(65, 187, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.GurguVolcano1,new Coordinate(188, 205, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.IceCave1,new Coordinate(197, 183, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.Cardia1,new Coordinate(92, 48, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.Cardia2,new Coordinate(79, 49, CoordinateLocale.Overworld)}, // requires airship
				{OverworldTeleportIndex.BahamutCave1,new Coordinate(96, 51, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.Cardia4,new Coordinate(93, 58, CoordinateLocale.Overworld)}, // requires airship
				{OverworldTeleportIndex.Cardia5,new Coordinate(105, 59, CoordinateLocale.Overworld)}, // requires airship
				{OverworldTeleportIndex.Cardia6,new Coordinate(116, 66, CoordinateLocale.Overworld)}, // requires airship
				{OverworldTeleportIndex.Waterfall,new Coordinate(54, 29, CoordinateLocale.Overworld)}, // requires canoe
				{OverworldTeleportIndex.DwarfCave,new Coordinate(100, 155, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.MatoyasCave,new Coordinate(168, 117, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.SardasCave,new Coordinate(30, 190, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.MarshCave1,new Coordinate(102, 236, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.MirageTower1,new Coordinate(194, 59, CoordinateLocale.Overworld)}, // requires chime
				{OverworldTeleportIndex.TitansTunnelEast,new Coordinate(42, 174, CoordinateLocale.Overworld)},
				{OverworldTeleportIndex.TitansTunnelWest,new Coordinate(30, 175, CoordinateLocale.Overworld)}
			};
		public static Dictionary<OverworldTeleportIndex, MapLocation> OverworldMapLocations =>
			new Dictionary<OverworldTeleportIndex, MapLocation>
			{
				{OverworldTeleportIndex.Coneria,MapLocation.Coneria},
				{OverworldTeleportIndex.Pravoka,MapLocation.Pravoka},
				{OverworldTeleportIndex.Elfland,MapLocation.Elfland},
				{OverworldTeleportIndex.Melmond,MapLocation.Melmond},
				{OverworldTeleportIndex.CrescentLake,MapLocation.CrescentLake},
				{OverworldTeleportIndex.Gaia,MapLocation.Gaia}, // requires airship
				{OverworldTeleportIndex.Onrac,MapLocation.Onrac},
				{OverworldTeleportIndex.Lefein,MapLocation.Lefein},
				{OverworldTeleportIndex.ConeriaCastle1,MapLocation.ConeriaCastle1},
				{OverworldTeleportIndex.ElflandCastle,MapLocation.ElflandCastle},
				{OverworldTeleportIndex.NorthwestCastle,MapLocation.NorthwestCastle},
				{OverworldTeleportIndex.CastleOrdeals1,MapLocation.CastleOrdeals1}, // requires canoe
				{OverworldTeleportIndex.TempleOfFiends1,MapLocation.TempleOfFiends1},
				{OverworldTeleportIndex.EarthCave1,MapLocation.EarthCave1},
				{OverworldTeleportIndex.GurguVolcano1,MapLocation.GurguVolcano1},
				{OverworldTeleportIndex.IceCave1,MapLocation.IceCave1},
				{OverworldTeleportIndex.Cardia1,MapLocation.Cardia1},
				{OverworldTeleportIndex.Cardia2,MapLocation.Cardia2}, // requires airship
				{OverworldTeleportIndex.BahamutCave1,MapLocation.BahamutCave1},
				{OverworldTeleportIndex.Cardia4,MapLocation.Cardia4}, // requires airship
				{OverworldTeleportIndex.Cardia5,MapLocation.Cardia5}, // requires airship
				{OverworldTeleportIndex.Cardia6,MapLocation.Cardia6}, // requires airship
				{OverworldTeleportIndex.Waterfall,MapLocation.Waterfall}, // requires canoe
				{OverworldTeleportIndex.DwarfCave,MapLocation.DwarfCave},
				{OverworldTeleportIndex.MatoyasCave,MapLocation.MatoyasCave},
				{OverworldTeleportIndex.SardasCave,MapLocation.SardasCave},
				{OverworldTeleportIndex.MarshCave1,MapLocation.MarshCave1},
				{OverworldTeleportIndex.MirageTower1,MapLocation.MirageTower1}, // requires chime
				{OverworldTeleportIndex.TitansTunnelEast,MapLocation.TitansTunnelEast},
				{OverworldTeleportIndex.TitansTunnelWest,MapLocation.TitansTunnelWest},
			};
		public static Dictionary<TeleportIndex, MapLocation> StandardMapLocations =>
			new Dictionary<TeleportIndex, MapLocation>
			{
				{TeleportIndex.ConeriaCastle2, MapLocation.ConeriaCastle2},
				{TeleportIndex.TempleOfFiends2, MapLocation.TempleOfFiends2},
				{TeleportIndex.MarshCaveTop, MapLocation.MarshCaveTop},
				{TeleportIndex.MarshCave3, MapLocation.MarshCave3},
				{TeleportIndex.MarshCaveBottom, MapLocation.MarshCaveBottom},
				{TeleportIndex.EarthCave2, MapLocation.EarthCave2},
				{TeleportIndex.EarthCaveVampire, MapLocation.EarthCaveVampire},
				{TeleportIndex.EarthCave4, MapLocation.EarthCave4},
				{TeleportIndex.EarthCaveLich, MapLocation.EarthCaveLich},
				{TeleportIndex.GurguVolcano2, MapLocation.GurguVolcano2},
				{TeleportIndex.GurguVolcano3, MapLocation.GurguVolcano3},
				{TeleportIndex.GurguVolcano4, MapLocation.GurguVolcano4},
				{TeleportIndex.GurguVolcano5, MapLocation.GurguVolcano5},
				{TeleportIndex.GurguVolcano6, MapLocation.GurguVolcano6},
				{TeleportIndex.GurguVolcanoKary, MapLocation.GurguVolcanoKary},
				{TeleportIndex.IceCave2, MapLocation.IceCave2},
				{TeleportIndex.IceCave3, MapLocation.IceCave3},
				{TeleportIndex.IceCavePitRoom, MapLocation.IceCavePitRoom},
				{TeleportIndex.CastleOrdealsMaze, MapLocation.CastleOrdealsMaze},
				{TeleportIndex.CastleOrdealsTop, MapLocation.CastleOrdealsTop},
				{TeleportIndex.CastleOrdealsBack, MapLocation.CastleOrdeals1},
				{TeleportIndex.BahamutsRoom, MapLocation.BahamutCave2},
				{TeleportIndex.SeaShrine1, MapLocation.SeaShrine1},
				{TeleportIndex.SeaShrine2, MapLocation.SeaShrine2},
				{TeleportIndex.SeaShrineMermaids, MapLocation.SeaShrineMermaids},
				{TeleportIndex.SeaShrine4, MapLocation.SeaShrine4},
				{TeleportIndex.SeaShrine5, MapLocation.SeaShrine5},
				{TeleportIndex.SeaShrine6, MapLocation.SeaShrine6},
				{TeleportIndex.SeaShrine7, MapLocation.SeaShrine7},
				{TeleportIndex.SeaShrine8, MapLocation.SeaShrine8},
				{TeleportIndex.SeaShrineKraken, MapLocation.SeaShrineKraken},
				{TeleportIndex.MirageTower2, MapLocation.MirageTower2},
				{TeleportIndex.MirageTower3, MapLocation.MirageTower3},
				{TeleportIndex.SkyPalace1, MapLocation.SkyPalace1},
				{TeleportIndex.SkyPalace2, MapLocation.SkyPalace2},
				{TeleportIndex.SkyPalace3, MapLocation.SkyPalace3},
				{TeleportIndex.SkyPalaceMaze, MapLocation.SkyPalaceMaze},
				{TeleportIndex.SkyPalaceTiamat, MapLocation.SkyPalaceTiamat}
			};
		public static Dictionary<OverworldTeleportIndex, TeleportDestination> VanillaOverworldTeleports =>
			new Dictionary<OverworldTeleportIndex, TeleportDestination>
			{
				{OverworldTeleportIndex.Coneria,Coneria},
				{OverworldTeleportIndex.Pravoka,Pravoka},
				{OverworldTeleportIndex.Elfland,Elfland},
				{OverworldTeleportIndex.Melmond,Melmond},
				{OverworldTeleportIndex.CrescentLake,CrescentLake},
				{OverworldTeleportIndex.Gaia,Gaia},
				{OverworldTeleportIndex.Onrac,Onrac},
				{OverworldTeleportIndex.Lefein,Lefein},
				{OverworldTeleportIndex.ConeriaCastle1,ConeriaCastle1},
				{OverworldTeleportIndex.ElflandCastle,ElflandCastle},
				{OverworldTeleportIndex.NorthwestCastle,NorthwestCastle},
				{OverworldTeleportIndex.CastleOrdeals1,CastleOrdeals1},
				{OverworldTeleportIndex.TempleOfFiends1,TempleOfFiends},
				{OverworldTeleportIndex.EarthCave1,EarthCave1},
				{OverworldTeleportIndex.GurguVolcano1,GurguVolcano1},
				{OverworldTeleportIndex.IceCave1,IceCave1},
				{OverworldTeleportIndex.Cardia1,Cardia1},
				{OverworldTeleportIndex.Cardia2,Cardia2},
				{OverworldTeleportIndex.BahamutCave1,BahamutCave1},
				{OverworldTeleportIndex.Cardia4,Cardia4},
				{OverworldTeleportIndex.Cardia5,Cardia5},
				{OverworldTeleportIndex.Cardia6,Cardia6},
				{OverworldTeleportIndex.Waterfall,Waterfall},
				{OverworldTeleportIndex.DwarfCave,DwarfCave},
				{OverworldTeleportIndex.MatoyasCave,MatoyasCave},
				{OverworldTeleportIndex.SardasCave,SardasCave},
				{OverworldTeleportIndex.MarshCave1,MarshCave1},
				{OverworldTeleportIndex.MirageTower1,MirageTower1},
				{OverworldTeleportIndex.TitansTunnelEast,TitansTunnelEast},
				{OverworldTeleportIndex.TitansTunnelWest,TitansTunnelWest},
			};
		public static Dictionary<TeleportIndex, TeleportDestination> VanillaStandardTeleports =>
			new Dictionary<TeleportIndex, TeleportDestination>
			{
				{TeleportIndex.ConeriaCastle2, ConeriaCastle2},
				//{TeleportIndex.TempleOfFiends2, TempleOfFiends2},
				{TeleportIndex.MarshCaveTop, MarshCaveTop},
				{TeleportIndex.MarshCave3, MarshCave3},
				{TeleportIndex.MarshCaveBottom, MarshCaveBottom},
				{TeleportIndex.EarthCave2, EarthCave2},
				{TeleportIndex.EarthCaveVampire, EarthCaveVampire},
				{TeleportIndex.EarthCave4, EarthCave4},
				{TeleportIndex.EarthCaveLich, EarthCaveLich},
				{TeleportIndex.GurguVolcano2, GurguVolcano2},
				{TeleportIndex.GurguVolcano3, GurguVolcano3},
				{TeleportIndex.GurguVolcano4, GurguVolcano4},
				{TeleportIndex.GurguVolcano5, GurguVolcano5},
				{TeleportIndex.GurguVolcano6, GurguVolcano6},
				{TeleportIndex.GurguVolcanoKary, GurguVolcanoKary},
				{TeleportIndex.IceCave2, IceCave2},
				{TeleportIndex.IceCave3, IceCave3},
				{TeleportIndex.IceCavePitRoom, IceCavePitRoom},
				{TeleportIndex.CastleOrdealsMaze, CastleOrdealsMaze},
				{TeleportIndex.CastleOrdealsTop, CastleOrdealsTop},
				//{TeleportIndex.CastleOrdealsBack, CastleOrdealsBack},
				{TeleportIndex.BahamutsRoom, BahamutsRoom},
				{TeleportIndex.SeaShrine1, SeaShrine1},
				{TeleportIndex.SeaShrine2, SeaShrine2},
				{TeleportIndex.SeaShrineMermaids, SeaShrineMermaids},
				{TeleportIndex.SeaShrine4, SeaShrine4},
				{TeleportIndex.SeaShrine5, SeaShrine5},
				{TeleportIndex.SeaShrine6, SeaShrine6},
				{TeleportIndex.SeaShrine7, SeaShrine7},
				{TeleportIndex.SeaShrine8, SeaShrine8},
				{TeleportIndex.SeaShrineKraken, SeaShrineKraken},
				{TeleportIndex.MirageTower2, MirageTower2},
				{TeleportIndex.MirageTower3, MirageTower3},
				{TeleportIndex.SkyPalace1, SkyPalace1},
				{TeleportIndex.SkyPalace2, SkyPalace2},
				{TeleportIndex.SkyPalace3, SkyPalace3},
				{TeleportIndex.SkyPalaceMaze, SkyPalaceMaze},
				{TeleportIndex.SkyPalaceTiamat, SkyPalaceTiamat}
			};
	}
}
