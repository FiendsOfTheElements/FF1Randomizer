using System.Collections.Generic;
using System.Linq;
namespace FF1Lib
{
    public static partial class ItemLocations
    {
        public static readonly IReadOnlyCollection<ItemLocation> Unused =
           new List<ItemLocation>{ Unused2, Unused3, Unused4, Unused5,
                Unused6, Unused7, Unused8, Unused9, Unused10, Unused11,
                Unused12, Unused13, Unused14, Unused15 };
        public static readonly IReadOnlyCollection<ItemLocation> Coneria =
            new List<ItemLocation> { Coneria1, Coneria2, ConeriaMajor, Coneria4, Coneria5, Coneria6 };
        public static readonly IReadOnlyCollection<ItemLocation> TempleOfFiends =
            new List<ItemLocation>{ TempleOfFiendsTopLeft1, TempleOfFiendsTopLeft2, TempleOfFiendsBottomLeft,
            TempleOfFiendsBottomRight, TempleOfFiendsTopRight1, TempleOfFiendsTopRight2};
        public static readonly IReadOnlyCollection<ItemLocation> MatoyasCave =
            new List<ItemLocation> { MatoyasCave1, MatoyasCave2, MatoyasCave3 };
        public static readonly IReadOnlyCollection<ItemLocation> Elfland =
            new List<ItemLocation> { Elfland1, Elfland2, Elfland3, Elfland4 };
        public static readonly IReadOnlyCollection<ItemLocation> MarshCave =
            new List<ItemLocation>{ MarshCave1, MarshCave2, MarshCave3, MarshCave4,
            MarshCave5, MarshCave6, MarshCave7, MarshCave8, MarshCaveMajor,
            MarshCave10, MarshCave11, MarshCave12, MarshCave13 };
        public static readonly IReadOnlyCollection<ItemLocation> NorthwestCastle =
            new List<ItemLocation> { NorthwestCastle1, NorthwestCastle2, NorthwestCastle3 };
        public static readonly IReadOnlyCollection<ItemLocation> DwarfCave =
            new List<ItemLocation>{ DwarfCave1, DwarfCave2, DwarfCave3, DwarfCave4,
            DwarfCave5, DwarfCave6, DwarfCave7, DwarfCave8, DwarfCave9,
            DwarfCave10 };
        public static readonly IReadOnlyCollection<ItemLocation> EarthCave =
            new List<ItemLocation> { EarthCave1, EarthCave2, EarthCave3, EarthCave4,
            EarthCave5, EarthCave6, EarthCave7, EarthCave8, EarthCave9,
            EarthCave10, EarthCave11, EarthCave12, EarthCave13, EarthCave14,
            EarthCave15, EarthCaveMajor, EarthCave17, EarthCave18, EarthCave19,
            EarthCave20, EarthCave21, EarthCave22, EarthCave23, EarthCave24 };
        public static readonly IReadOnlyCollection<ItemLocation> TitansTunnel =
            new List<ItemLocation> { TitansTunnel1, TitansTunnel2, TitansTunnel3, TitansTunnel4 };
        public static readonly IReadOnlyCollection<ItemLocation> GurguVolcano =
            new List<ItemLocation> { GurguVolcano1, GurguVolcano2, GurguVolcano3, GurguVolcano4,
            GurguVolcano5, GurguVolcano6, GurguVolcano7, GurguVolcano8, GurguVolcano9,
            GurguVolcano10, GurguVolcano11, GurguVolcano12, GurguVolcano13, GurguVolcano14,
            GurguVolcano15, GurguVolcano16, GurguVolcano17, GurguVolcano18, GurguVolcano19,
            GurguVolcano20, GurguVolcano21, GurguVolcano22, GurguVolcano23, GurguVolcano24,
            GurguVolcano25, GurguVolcano26, GurguVolcano27, GurguVolcano28, GurguVolcano29,
            GurguVolcano30, GurguVolcano31, GurguVolcano32, GurguVolcanoMajor };
        public static readonly IReadOnlyCollection<ItemLocation> IceCave =
            new List<ItemLocation> { IceCave1, IceCave2, IceCave3, IceCave4,
            IceCave5, IceCave6, IceCave7, IceCaveMajor, IceCave9,
            IceCave10, IceCave11, IceCave12, IceCave13, IceCave14,
            IceCave15, IceCave16 };
        public static readonly IReadOnlyCollection<ItemLocation> Ordeals =
            new List<ItemLocation> { Ordeals1, Ordeals2, Ordeals3, Ordeals4, Ordeals5,
            Ordeals6, Ordeals7, Ordeals8, OrdealsMajor };
        public static readonly IReadOnlyCollection<ItemLocation> Cardia =
            new List<ItemLocation> { Cardia1, Cardia2, Cardia3, Cardia4, Cardia5,
            Cardia6, Cardia7, Cardia8, Cardia9, Cardia10, Cardia11,
            Cardia12, Cardia13 };
        public static readonly IReadOnlyCollection<ItemLocation> SeaShrine =
            new List<ItemLocation> { SeaShrine1, SeaShrine2, SeaShrine3, SeaShrine4,
            SeaShrine5, SeaShrine6, SeaShrine7, SeaShrine8, SeaShrine9,
            SeaShrine10, SeaShrine11, SeaShrine12, SeaShrine13, SeaShrine14,
            SeaShrine15, SeaShrine16, SeaShrine17, SeaShrine18, SeaShrine19,
            SeaShrine20, SeaShrine21, SeaShrine22, SeaShrine23, SeaShrine24,
            SeaShrine25, SeaShrine26, SeaShrine27, SeaShrine28, SeaShrine29,
            SeaShrine30, SeaShrine31, SeaShrineMajor };
        public static readonly IReadOnlyCollection<ItemLocation> Waterfall =
            new List<ItemLocation> { Waterfall1, Waterfall2, Waterfall3, Waterfall4,
            Waterfall5, Waterfall6 };
        public static readonly IReadOnlyCollection<ItemLocation> MirageTower =
            new List<ItemLocation> { MirageTower1, MirageTower2, MirageTower3, MirageTower4,
            MirageTower5, MirageTower6, MirageTower7, MirageTower8, MirageTower9,
            MirageTower10, MirageTower11, MirageTower12, MirageTower13, MirageTower14,
            MirageTower15, MirageTower16, MirageTower17, MirageTower18 };
        public static readonly IReadOnlyCollection<ItemLocation> SkyPalace =
            new List<ItemLocation> { SkyPalace1, SkyPalace2, SkyPalace3, SkyPalace4,
            SkyPalace5, SkyPalace6, SkyPalace7, SkyPalace8, SkyPalace9,
            SkyPalace10, SkyPalace11, SkyPalace12, SkyPalace13, SkyPalace14,
            SkyPalace15, SkyPalace16, SkyPalace17, SkyPalace18, SkyPalace19,
            SkyPalaceMajor, SkyPalace21, SkyPalace22, SkyPalace23, SkyPalace24,
            SkyPalace25, SkyPalace26, SkyPalace27, SkyPalace28, SkyPalace29,
            SkyPalace30, SkyPalace31, SkyPalace32, SkyPalace33, SkyPalace34 }
            .AsReadOnly();
        public static readonly IReadOnlyCollection<ItemLocation> ToFR =
            new List<ItemLocation> { ToFRevisitedMajor, ToFRevisited2, ToFRevisited3,
            ToFRevisited4, ToFRevisited5, ToFRevisited6, ToFRevisited7};
        public static readonly IReadOnlyCollection<ItemLocation> AllTreasures =
        Unused.Concat(Coneria).Concat(TempleOfFiends).Concat(MatoyasCave)
              .Concat(Elfland).Concat(MarshCave).Concat(NorthwestCastle)
              .Concat(DwarfCave).Concat(EarthCave).Concat(TitansTunnel)
              .Concat(GurguVolcano).Concat(IceCave).Concat(Ordeals)
              .Concat(Cardia).Concat(SeaShrine).Concat(Waterfall)
              .Concat(MirageTower).Concat(SkyPalace).Concat(ToFR)
              .ToList();

        public static readonly IReadOnlyCollection<ItemLocation> AllNPCItemLocations =
            new List<ItemLocation> {
            KingConeria, Princess, Matoya, Bikke, ElfPrince, Astos, Nerrick, Sarda, CanoeSage, CubeBot, Fairy, Lefein
        };

        public static readonly IReadOnlyCollection<ItemLocation> AllShopItemLocations =
            new List<ItemLocation> {
            CaravanItemShop1
        };

        public static readonly IReadOnlyCollection<ItemLocation> AllNonTreasureItemLocations =
            AllNPCItemLocations.Concat(AllShopItemLocations).ToList();

        public static readonly IReadOnlyCollection<ItemLocation> AllQuestItemLocations =
            AllTreasures.Concat(AllNonTreasureItemLocations).Where(x => !x.IsUnused).ToList();

    }
}
