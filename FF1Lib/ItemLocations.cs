using System.Reflection;

namespace FF1Lib
{
	public static partial class ItemLocations
	{
		public static TreasureChest Coneria1 = new TreasureChest(0x3101, nameof(Coneria1), MapLocation.ConeriaCastleRoom1, Item.IronArmor);
		public static TreasureChest Coneria2 = new TreasureChest(0x3102, nameof(Coneria2), MapLocation.ConeriaCastleRoom1, Item.IronShield);
		public static TreasureChest ConeriaMajor = new TreasureChest(0x3103, nameof(ConeriaMajor), MapLocation.ConeriaCastleRoom1, Item.Tnt);
		public static TreasureChest Coneria4 = new TreasureChest(0x3104, nameof(Coneria4), MapLocation.ConeriaCastleRoom2, Item.IronStaff);
		public static TreasureChest Coneria5 = new TreasureChest(0x3105, nameof(Coneria5), MapLocation.ConeriaCastleRoom2, Item.Sabre);
		public static TreasureChest Coneria6 = new TreasureChest(0x3106, nameof(Coneria6), MapLocation.ConeriaCastleRoom2, Item.SilverKnife);
		public static TreasureChest ToFTopLeft1 = new TreasureChest(0x3107, nameof(ToFTopLeft1), MapLocation.TempleOfFiends1Room1, Item.Cabin);
		public static TreasureChest ToFTopLeft2 = new TreasureChest(0x3108, nameof(ToFTopLeft2), MapLocation.TempleOfFiends1Room1, Item.Heal);
		public static TreasureChest ToFBottomLeft = new TreasureChest(0x3109, nameof(ToFBottomLeft), MapLocation.TempleOfFiends1Room3, Item.Cap);
		public static TreasureChest ToFBottomRight = new TreasureChest(0x310A, nameof(ToFBottomRight), MapLocation.TempleOfFiends1Room4, Item.RuneSword);
		public static TreasureChest ToFTopRight1 = new TreasureChest(0x310B, nameof(ToFTopRight1), MapLocation.TempleOfFiends1Room2, Item.WereSword);
		public static TreasureChest ToFTopRight2 = new TreasureChest(0x310C, nameof(ToFTopRight2), MapLocation.TempleOfFiends1Room2, Item.Soft);
		public static TreasureChest Elfland1 = new TreasureChest(0x310D, nameof(Elfland1), MapLocation.ElflandCastleRoom1, Item.SilverHammer);
		public static TreasureChest Elfland2 = new TreasureChest(0x310E, nameof(Elfland2), MapLocation.ElflandCastleRoom1, Item.Gold400);
		public static TreasureChest Elfland3 = new TreasureChest(0x310F, nameof(Elfland3), MapLocation.ElflandCastleRoom1, Item.Gold330);
		public static TreasureChest Elfland4 = new TreasureChest(0x3110, nameof(Elfland4), MapLocation.ElflandCastleRoom1, Item.CopperGauntlets);
		public static TreasureChest NorthwestCastle1 = new TreasureChest(0x3111, nameof(NorthwestCastle1), MapLocation.NorthwestCastleRoom2, Item.PowerRod);
		public static TreasureChest NorthwestCastle2 = new TreasureChest(0x3112, nameof(NorthwestCastle2), MapLocation.NorthwestCastleRoom2, Item.IronGauntlets);
		public static TreasureChest NorthwestCastle3 = new TreasureChest(0x3113, nameof(NorthwestCastle3), MapLocation.NorthwestCastleRoom2, Item.Falchon);
		// Marsh Cave has 5 extra empty treasure chests
		public static TreasureChest MarshCave1 = new TreasureChest(0x3114, nameof(MarshCave1), MapLocation.MarshCaveBottom, Item.Gold295);
		public static TreasureChest MarshCave2 = new TreasureChest(0x3115, nameof(MarshCave2), MapLocation.MarshCaveBottom, Item.Copper);
		public static TreasureChest MarshCave3 = new TreasureChest(0x3116, nameof(MarshCave3), MapLocation.MarshCaveBottom, Item.House);
		public static TreasureChest MarshCave4 = new TreasureChest(0x3117, nameof(MarshCave4), MapLocation.MarshCaveBottom, Item.Gold385);
		public static TreasureChest MarshCave5 = new TreasureChest(0x3118, nameof(MarshCave5), MapLocation.MarshCaveTop, Item.Gold620);
		public static TreasureChest MarshCave6 = new TreasureChest(0x3119, nameof(MarshCave6), MapLocation.MarshCaveTop, Item.ShortSword);
		public static TreasureChest MarshCave7 = new TreasureChest(0x311A, nameof(MarshCave7), MapLocation.MarshCaveTop, Item.Gold680);
		public static TreasureChest MarshCave8 = new TreasureChest(0x311B, nameof(MarshCave8), MapLocation.MarshCaveTop, Item.LargeKnife);
		public static TreasureChest MarshCaveMajor = new TreasureChest(0x311C, nameof(MarshCaveMajor), MapLocation.MarshCaveBottom, Item.Crown);
		public static TreasureChest MarshCave10 = new TreasureChest(0x311D, nameof(MarshCave10), MapLocation.MarshCaveBottom, Item.IronArmor);
		public static TreasureChest MarshCave11 = new TreasureChest(0x311E, nameof(MarshCave11), MapLocation.MarshCaveBottomRoom13, Item.Silver);
		public static TreasureChest MarshCave12 = new TreasureChest(0x311F, nameof(MarshCave12), MapLocation.MarshCaveBottomRoom14, Item.SilverKnife);
		public static TreasureChest MarshCave13 = new TreasureChest(0x3120, nameof(MarshCave13), MapLocation.MarshCaveBottomRoom16, Item.Gold1020);
		public static TreasureChest DwarfCave1 = new TreasureChest(0x3121, nameof(DwarfCave1), MapLocation.DwarfCave, Item.Gold450);
		public static TreasureChest DwarfCave2 = new TreasureChest(0x3122, nameof(DwarfCave2), MapLocation.DwarfCave, Item.Gold575);
		public static TreasureChest DwarfCave3 = new TreasureChest(0x3123, nameof(DwarfCave3), MapLocation.DwarfCaveRoom3, Item.Cabin);
		public static TreasureChest DwarfCave4 = new TreasureChest(0x3124, nameof(DwarfCave4), MapLocation.DwarfCaveRoom3, Item.IronHelm);
		public static TreasureChest DwarfCave5 = new TreasureChest(0x3125, nameof(DwarfCave5), MapLocation.DwarfCaveRoom3, Item.WoodenHelm);
		public static TreasureChest DwarfCave6 = new TreasureChest(0x3126, nameof(DwarfCave6), MapLocation.DwarfCaveRoom3, Item.DragonSword);
		public static TreasureChest DwarfCave7 = new TreasureChest(0x3127, nameof(DwarfCave7), MapLocation.DwarfCaveRoom3, Item.SilverKnife);
		public static TreasureChest DwarfCave8 = new TreasureChest(0x3128, nameof(DwarfCave8), MapLocation.DwarfCaveRoom3, Item.SilverArmor);
		public static TreasureChest DwarfCave9 = new TreasureChest(0x3129, nameof(DwarfCave9), MapLocation.DwarfCaveRoom3, Item.Gold575);
		public static TreasureChest DwarfCave10 = new TreasureChest(0x312A, nameof(DwarfCave10), MapLocation.DwarfCaveRoom3, Item.House);
		public static TreasureChest MatoyasCave1 = new TreasureChest(0x312B, nameof(MatoyasCave1), MapLocation.MatoyasCave, Item.Heal);
		public static TreasureChest MatoyasCave2 = new TreasureChest(0x312C, nameof(MatoyasCave2), MapLocation.MatoyasCave, Item.Pure);
		public static TreasureChest MatoyasCave3 = new TreasureChest(0x312D, nameof(MatoyasCave3), MapLocation.MatoyasCave, Item.Heal);
		public static TreasureChest EarthCave1 = new TreasureChest(0x312E, nameof(EarthCave1), MapLocation.EarthCave1, Item.Gold880);
		public static TreasureChest EarthCave2 = new TreasureChest(0x312F, nameof(EarthCave2), MapLocation.EarthCave1, Item.Heal);
		public static TreasureChest EarthCave3 = new TreasureChest(0x3130, nameof(EarthCave3), MapLocation.EarthCave1, Item.Pure);
		public static TreasureChest EarthCave4 = new TreasureChest(0x3131, nameof(EarthCave4), MapLocation.EarthCave1, Item.Gold795);
		public static TreasureChest EarthCave5 = new TreasureChest(0x3132, nameof(EarthCave5), MapLocation.EarthCave1, Item.Gold1975);
		public static TreasureChest EarthCave6 = new TreasureChest(0x3133, nameof(EarthCave6), MapLocation.EarthCave2, Item.CoralSword);
		public static TreasureChest EarthCave7 = new TreasureChest(0x3134, nameof(EarthCave7), MapLocation.EarthCave2, Item.Cabin);
		public static TreasureChest EarthCave8 = new TreasureChest(0x3135, nameof(EarthCave8), MapLocation.EarthCave2, Item.Gold330);
		public static TreasureChest EarthCave9 = new TreasureChest(0x3136, nameof(EarthCave9), MapLocation.EarthCave2, Item.Gold5000);
		public static TreasureChest EarthCave10 = new TreasureChest(0x3137, nameof(EarthCave10), MapLocation.EarthCave2, Item.WoodenShield);
		public static TreasureChest EarthCave11 = new TreasureChest(0x3138, nameof(EarthCave11), MapLocation.EarthCave2, Item.Gold575);
		public static TreasureChest EarthCave12 = new TreasureChest(0x3139, nameof(EarthCave12), MapLocation.EarthCaveVampire, Item.Gold1020);
		public static TreasureChest EarthCave13 = new TreasureChest(0x313A, nameof(EarthCave13), MapLocation.EarthCaveVampire, Item.Gold3400);
		public static TreasureChest EarthCave14 = new TreasureChest(0x313B, nameof(EarthCave14), MapLocation.EarthCaveVampire, Item.Tent);
		public static TreasureChest EarthCave15 = new TreasureChest(0x313C, nameof(EarthCave15), MapLocation.EarthCaveVampire, Item.Heal);
		public static TreasureChest EarthCaveMajor = new TreasureChest(0x313D, nameof(EarthCaveMajor), MapLocation.EarthCaveVampire, Item.Ruby);
		public static TreasureChest EarthCave17 = new TreasureChest(0x313E, nameof(EarthCave17), MapLocation.EarthCave4, Item.Gold1250);
		public static TreasureChest EarthCave18 = new TreasureChest(0x313F, nameof(EarthCave18), MapLocation.EarthCave4, Item.SilverShield);
		public static TreasureChest EarthCave19 = new TreasureChest(0x3140, nameof(EarthCave19), MapLocation.EarthCave4, Item.Cabin);
		public static TreasureChest EarthCave20 = new TreasureChest(0x3141, nameof(EarthCave20), MapLocation.EarthCave4, Item.Gold5450);
		public static TreasureChest EarthCave21 = new TreasureChest(0x3142, nameof(EarthCave21), MapLocation.EarthCave4, Item.Gold1520);
		public static TreasureChest EarthCave22 = new TreasureChest(0x3143, nameof(EarthCave22), MapLocation.EarthCave4, Item.WoodenRod);
		public static TreasureChest EarthCave23 = new TreasureChest(0x3144, nameof(EarthCave23), MapLocation.EarthCave4, Item.Gold3400);
		public static TreasureChest EarthCave24 = new TreasureChest(0x3145, nameof(EarthCave24), MapLocation.EarthCave4, Item.Gold1455);
		public static TreasureChest TitansTunnel1 = new TreasureChest(0x3146, nameof(TitansTunnel1), MapLocation.TitansTunnelRoom, Item.SilverHelm);
		public static TreasureChest TitansTunnel2 = new TreasureChest(0x3147, nameof(TitansTunnel2), MapLocation.TitansTunnelRoom, Item.Gold450);
		public static TreasureChest TitansTunnel3 = new TreasureChest(0x3148, nameof(TitansTunnel3), MapLocation.TitansTunnelRoom, Item.Gold620);
		public static TreasureChest TitansTunnel4 = new TreasureChest(0x3149, nameof(TitansTunnel4), MapLocation.TitansTunnelRoom, Item.GreatAxe);
		// Volcano has 3 extra empty treasure chests
		public static TreasureChest Volcano1 = new TreasureChest(0x314A, nameof(Volcano1), MapLocation.GurguVolcano2, Item.Heal);
		public static TreasureChest Volcano2 = new TreasureChest(0x314B, nameof(Volcano2), MapLocation.GurguVolcano2, Item.Cabin);
		public static TreasureChest Volcano3 = new TreasureChest(0x314C, nameof(Volcano3), MapLocation.GurguVolcano2, Item.Gold1975);
		public static TreasureChest Volcano4 = new TreasureChest(0x314D, nameof(Volcano4), MapLocation.GurguVolcano2, Item.Pure);
		public static TreasureChest Volcano5 = new TreasureChest(0x314E, nameof(Volcano5), MapLocation.GurguVolcano2, Item.Heal);
		public static TreasureChest Volcano6 = new TreasureChest(0x314F, nameof(Volcano6), MapLocation.GurguVolcano2, Item.Gold1455);
		public static TreasureChest Volcano7 = new TreasureChest(0x3150, nameof(Volcano7), MapLocation.GurguVolcano2, Item.SilverShield);
		public static TreasureChest Volcano8 = new TreasureChest(0x3151, nameof(Volcano8), MapLocation.GurguVolcano2, Item.Gold1520);
		public static TreasureChest Volcano9 = new TreasureChest(0x3152, nameof(Volcano9), MapLocation.GurguVolcano2, Item.SilverHelm);
		public static TreasureChest Volcano10 = new TreasureChest(0x3153, nameof(Volcano10), MapLocation.GurguVolcano2, Item.SilverGauntlets);
		public static TreasureChest Volcano11 = new TreasureChest(0x3154, nameof(Volcano11), MapLocation.GurguVolcano2, Item.Gold1760);
		public static TreasureChest Volcano12 = new TreasureChest(0x3155, nameof(Volcano12), MapLocation.GurguVolcano2, Item.SilverAxe);
		public static TreasureChest Volcano13 = new TreasureChest(0x3156, nameof(Volcano13), MapLocation.GurguVolcano2, Item.Gold795);
		public static TreasureChest Volcano14 = new TreasureChest(0x3157, nameof(Volcano14), MapLocation.GurguVolcano2, Item.Gold750);
		public static TreasureChest Volcano15 = new TreasureChest(0x3158, nameof(Volcano15), MapLocation.GurguVolcano2, Item.GiantSword);
		public static TreasureChest Volcano16 = new TreasureChest(0x3159, nameof(Volcano16), MapLocation.GurguVolcano2, Item.Gold4150);
		public static TreasureChest Volcano17 = new TreasureChest(0x315A, nameof(Volcano17), MapLocation.GurguVolcano2, Item.Gold1520);
		public static TreasureChest Volcano18 = new TreasureChest(0x315B, nameof(Volcano18), MapLocation.GurguVolcano2, Item.SilverHelm);
		public static TreasureChest Volcano19 = new TreasureChest(0x315C, nameof(Volcano19), MapLocation.GurguVolcano6, Item.Soft);
		public static TreasureChest Volcano20 = new TreasureChest(0x315D, nameof(Volcano20), MapLocation.GurguVolcano6, Item.Gold2750);
		public static TreasureChest Volcano21 = new TreasureChest(0x315E, nameof(Volcano21), MapLocation.GurguVolcano6, Item.Gold1760);
		public static TreasureChest Volcano22 = new TreasureChest(0x315F, nameof(Volcano22), MapLocation.GurguVolcano6, Item.WoodenRod);
		public static TreasureChest Volcano23 = new TreasureChest(0x3160, nameof(Volcano23), MapLocation.GurguVolcano6, Item.Gold1250);
		public static TreasureChest Volcano24 = new TreasureChest(0x3161, nameof(Volcano24), MapLocation.GurguVolcano6, Item.Gold10);
		public static TreasureChest Volcano25 = new TreasureChest(0x3162, nameof(Volcano25), MapLocation.GurguVolcano6, Item.Gold155);
		public static TreasureChest Volcano26 = new TreasureChest(0x3163, nameof(Volcano26), MapLocation.GurguVolcano6, Item.House);
		public static TreasureChest Volcano27 = new TreasureChest(0x3164, nameof(Volcano27), MapLocation.GurguVolcano6, Item.Gold2000);
		public static TreasureChest Volcano28 = new TreasureChest(0x3165, nameof(Volcano28), MapLocation.GurguVolcano6, Item.IceSword);
		public static TreasureChest Volcano29 = new TreasureChest(0x3166, nameof(Volcano29), MapLocation.GurguVolcano6, Item.Gold880);
		public static TreasureChest Volcano30 = new TreasureChest(0x3167, nameof(Volcano30), MapLocation.GurguVolcano6, Item.Pure);
		public static TreasureChest Volcano31 = new TreasureChest(0x3168, nameof(Volcano31), MapLocation.GurguVolcano6, Item.FlameShield);
		public static TreasureChest Volcano32 = new TreasureChest(0x3169, nameof(Volcano32), MapLocation.GurguVolcano6, Item.Gold7340);
		public static TreasureChest VolcanoMajor = new TreasureChest(0x316A, nameof(VolcanoMajor), MapLocation.GurguVolcanoKary, Item.FlameArmor);
		public static TreasureChest IceCave1 = new TreasureChest(0x316B, nameof(IceCave1), MapLocation.IceCaveBackExit, Item.Heal);
		public static TreasureChest IceCave2 = new TreasureChest(0x316C, nameof(IceCave2), MapLocation.IceCaveBackExit, Item.Gold10000);
		public static TreasureChest IceCave3 = new TreasureChest(0x316D, nameof(IceCave3), MapLocation.IceCaveBackExit, Item.Gold9500);
		public static TreasureChest IceCave4 = new TreasureChest(0x316E, nameof(IceCave4), MapLocation.IceCaveBackExit, Item.Tent);
		public static TreasureChest IceCave5 = new TreasureChest(0x316F, nameof(IceCave5), MapLocation.IceCaveBackExit, Item.IceShield);
		public static TreasureChest IceCave6 = new TreasureChest(0x3170, nameof(IceCave6), MapLocation.IceCavePitRoom, Item.Cloth);
		public static TreasureChest IceCave7 = new TreasureChest(0x3171, nameof(IceCave7), MapLocation.IceCavePitRoom, Item.FlameSword);
		public static TreasureChest IceCaveMajor = new TreasureChest(0x3172, nameof(IceCaveMajor), MapLocation.IceCaveFloater, Item.Floater);
		public static TreasureChest IceCave9 = new TreasureChest(0x3173, nameof(IceCave9), MapLocation.IceCave5, Item.Gold7900);
		public static TreasureChest IceCave10 = new TreasureChest(0x3174, nameof(IceCave10), MapLocation.IceCave5, Item.Gold5450);
		public static TreasureChest IceCave11 = new TreasureChest(0x3175, nameof(IceCave11), MapLocation.IceCave5, Item.Gold9900);
		public static TreasureChest IceCave12 = new TreasureChest(0x3176, nameof(IceCave12), MapLocation.IceCave5, Item.Gold5000);
		public static TreasureChest IceCave13 = new TreasureChest(0x3177, nameof(IceCave13), MapLocation.IceCave5, Item.Gold180);
		public static TreasureChest IceCave14 = new TreasureChest(0x3178, nameof(IceCave14), MapLocation.IceCave5, Item.Gold12350);
		public static TreasureChest IceCave15 = new TreasureChest(0x3179, nameof(IceCave15), MapLocation.IceCave5, Item.SilverGauntlets);
		public static TreasureChest IceCave16 = new TreasureChest(0x317A, nameof(IceCave16), MapLocation.IceCave5, Item.IceArmor);
		// Ordeals has 1 extra empty treasure chest
		public static TreasureChest Ordeals1 = new TreasureChest(0x317B, nameof(Ordeals1), MapLocation.CastleOrdealsTop, Item.ZeusGauntlets);
		public static TreasureChest Ordeals2 = new TreasureChest(0x317C, nameof(Ordeals2), MapLocation.CastleOrdealsTop, Item.House);
		public static TreasureChest Ordeals3 = new TreasureChest(0x317D, nameof(Ordeals3), MapLocation.CastleOrdealsTop, Item.Gold1455);
		public static TreasureChest Ordeals4 = new TreasureChest(0x317E, nameof(Ordeals4), MapLocation.CastleOrdealsTop, Item.Gold7340);
		public static TreasureChest Ordeals5 = new TreasureChest(0x317F, nameof(Ordeals5), MapLocation.CastleOrdealsTop, Item.Gold);
		public static TreasureChest Ordeals6 = new TreasureChest(0x3180, nameof(Ordeals6), MapLocation.CastleOrdealsTop, Item.IceSword);
		public static TreasureChest Ordeals7 = new TreasureChest(0x3181, nameof(Ordeals7), MapLocation.CastleOrdealsTop, Item.IronGauntlets);
		public static TreasureChest Ordeals8 = new TreasureChest(0x3182, nameof(Ordeals8), MapLocation.CastleOrdealsTop, Item.HealRod);
		public static TreasureChest OrdealsMajor = new TreasureChest(0x3183, nameof(OrdealsMajor), MapLocation.CastleOrdealsTop, Item.Tail);
		public static TreasureChest Cardia1 = new TreasureChest(0x3184, nameof(Cardia1), MapLocation.Cardia6, Item.Gold1455);
		public static TreasureChest Cardia2 = new TreasureChest(0x3185, nameof(Cardia2), MapLocation.Cardia6, Item.Gold2000);
		public static TreasureChest Cardia3 = new TreasureChest(0x3186, nameof(Cardia3), MapLocation.Cardia6, Item.Gold2750);
		public static TreasureChest Cardia4 = new TreasureChest(0x3187, nameof(Cardia4), MapLocation.Cardia6, Item.Gold2750);
		public static TreasureChest Cardia5 = new TreasureChest(0x3188, nameof(Cardia5), MapLocation.Cardia6, Item.Gold1520);
		public static TreasureChest Cardia6 = new TreasureChest(0x3189, nameof(Cardia6), MapLocation.Cardia4, Item.Gold10);
		public static TreasureChest Cardia7 = new TreasureChest(0x318A, nameof(Cardia7), MapLocation.Cardia4, Item.Gold500);
		public static TreasureChest Cardia8 = new TreasureChest(0x318B, nameof(Cardia8), MapLocation.Cardia4, Item.House);
		public static TreasureChest Cardia9 = new TreasureChest(0x318C, nameof(Cardia9), MapLocation.Cardia2, Item.Gold575);
		public static TreasureChest Cardia10 = new TreasureChest(0x318D, nameof(Cardia10), MapLocation.Cardia2, Item.Soft);
		public static TreasureChest Cardia11 = new TreasureChest(0x318E, nameof(Cardia11), MapLocation.Cardia2, Item.Cabin);
		public static TreasureChest Cardia12 = new TreasureChest(0x318F, nameof(Cardia12), MapLocation.Cardia6, Item.Gold9500);
		public static TreasureChest Cardia13 = new TreasureChest(0x3190, nameof(Cardia13), MapLocation.Cardia6, Item.Gold160);
		public static TreasureChest Unused2 = new TreasureChest(0x3191, nameof(Unused2), 0, Item.Gold530, isUnused: true);
		public static TreasureChest Unused3 = new TreasureChest(0x3192, nameof(Unused3), 0, Item.SmallKnife, isUnused: true);
		public static TreasureChest Unused4 = new TreasureChest(0x3193, nameof(Unused4), 0, Item.Cap, isUnused: true);
		public static TreasureChest Unused5 = new TreasureChest(0x3194, nameof(Unused5), 0, Item.ZeusGauntlets, isUnused: true);
		public static TreasureChest SeaShrine1 = new TreasureChest(0x3195, nameof(SeaShrine1), MapLocation.SeaShrine8, Item.Ribbon);
		public static TreasureChest SeaShrine2 = new TreasureChest(0x3196, nameof(SeaShrine2), MapLocation.SeaShrine8, Item.Gold9900);
		public static TreasureChest SeaShrine3 = new TreasureChest(0x3197, nameof(SeaShrine3), MapLocation.SeaShrine8, Item.Gold7340);
		public static TreasureChest SeaShrine4 = new TreasureChest(0x3198, nameof(SeaShrine4), MapLocation.SeaShrine8, Item.Gold2750);
		public static TreasureChest SeaShrine5 = new TreasureChest(0x3199, nameof(SeaShrine5), MapLocation.SeaShrine8, Item.Gold7690);
		public static TreasureChest SeaShrine6 = new TreasureChest(0x319A, nameof(SeaShrine6), MapLocation.SeaShrine8, Item.Gold8135);
		public static TreasureChest SeaShrine7 = new TreasureChest(0x319B, nameof(SeaShrine7), MapLocation.SeaShrine8, Item.Gold5450);
		public static TreasureChest SeaShrine8 = new TreasureChest(0x319C, nameof(SeaShrine8), MapLocation.SeaShrine8, Item.Gold385);
		public static TreasureChest SeaShrine9 = new TreasureChest(0x319D, nameof(SeaShrine9), MapLocation.SeaShrine8, Item.PowerGauntlets);
		public static TreasureChest SeaShrine10 = new TreasureChest(0x319E, nameof(SeaShrine10), MapLocation.SeaShrine8, Item.LightAxe);
		public static TreasureChest SeaShrine11 = new TreasureChest(0x319F, nameof(SeaShrine11), MapLocation.SeaShrine1, Item.Gold9900);
		public static TreasureChest SeaShrine12 = new TreasureChest(0x31A0, nameof(SeaShrine12), MapLocation.SeaShrine1, Item.Gold2000);
		public static TreasureChest SeaShrine13 = new TreasureChest(0x31A1, nameof(SeaShrine13), MapLocation.SeaShrine7, Item.Gold450);
		public static TreasureChest SeaShrine14 = new TreasureChest(0x31A2, nameof(SeaShrine14), MapLocation.SeaShrine7, Item.Gold110);
		public static TreasureChest SeaShrine15 = new TreasureChest(0x31A3, nameof(SeaShrine15), MapLocation.SeaShrine2, Item.LightAxe);
		public static TreasureChest SeaShrine16 = new TreasureChest(0x31A4, nameof(SeaShrine16), MapLocation.SeaShrine2, Item.OpalArmor);
		public static TreasureChest SeaShrineLocked = new TreasureChest(0x31A5, nameof(SeaShrineLocked), MapLocation.SeaShrine2Room2, Item.Gold20);
		public static TreasureChest SeaShrine18 = new TreasureChest(0x31A6, nameof(SeaShrine18), MapLocation.SeaShrine2, Item.MageRod);
		public static TreasureChest SeaShrine19 = new TreasureChest(0x31A7, nameof(SeaShrine19), MapLocation.SeaShrine2, Item.Gold12350);
		public static TreasureChest SeaShrine20 = new TreasureChest(0x31A8, nameof(SeaShrine20), MapLocation.SeaShrineMermaids, Item.Gold9000);
		public static TreasureChest SeaShrine21 = new TreasureChest(0x31A9, nameof(SeaShrine21), MapLocation.SeaShrineMermaids, Item.Gold1760);
		public static TreasureChest SeaShrine22 = new TreasureChest(0x31AA, nameof(SeaShrine22), MapLocation.SeaShrineMermaids, Item.Opal);
		public static TreasureChest SeaShrine23 = new TreasureChest(0x31AB, nameof(SeaShrine23), MapLocation.SeaShrineMermaids, Item.Gold2750);
		public static TreasureChest SeaShrine24 = new TreasureChest(0x31AC, nameof(SeaShrine24), MapLocation.SeaShrineMermaids, Item.Gold10000);
		public static TreasureChest SeaShrine25 = new TreasureChest(0x31AD, nameof(SeaShrine25), MapLocation.SeaShrineMermaids, Item.Gold10);
		public static TreasureChest SeaShrine26 = new TreasureChest(0x31AE, nameof(SeaShrine26), MapLocation.SeaShrineMermaids, Item.Gold4150);
		public static TreasureChest SeaShrine27 = new TreasureChest(0x31AF, nameof(SeaShrine27), MapLocation.SeaShrineMermaids, Item.Gold5000);
		public static TreasureChest SeaShrine28 = new TreasureChest(0x31B0, nameof(SeaShrine28), MapLocation.SeaShrineMermaids, Item.Pure);
		public static TreasureChest SeaShrine29 = new TreasureChest(0x31B1, nameof(SeaShrine29), MapLocation.SeaShrineMermaids, Item.OpalShield);
		public static TreasureChest SeaShrine30 = new TreasureChest(0x31B2, nameof(SeaShrine30), MapLocation.SeaShrineMermaids, Item.OpalHelm);
		public static TreasureChest SeaShrine31 = new TreasureChest(0x31B3, nameof(SeaShrine31), MapLocation.SeaShrineMermaids, Item.OpalGauntlets);
		public static TreasureChest SeaShrineMajor = new TreasureChest(0x31B4, nameof(SeaShrineMajor), MapLocation.SeaShrineMermaids, Item.Slab);
		public static TreasureChest Waterfall1 = new TreasureChest(0x31B5, nameof(Waterfall1), MapLocation.Waterfall, Item.WizardRod);
		public static TreasureChest Waterfall2 = new TreasureChest(0x31B6, nameof(Waterfall2), MapLocation.Waterfall, Item.Ribbon);
		public static TreasureChest Waterfall3 = new TreasureChest(0x31B7, nameof(Waterfall3), MapLocation.Waterfall, Item.Gold13450);
		public static TreasureChest Waterfall4 = new TreasureChest(0x31B8, nameof(Waterfall4), MapLocation.Waterfall, Item.Gold6400);
		public static TreasureChest Waterfall5 = new TreasureChest(0x31B9, nameof(Waterfall5), MapLocation.Waterfall, Item.Gold5000);
		public static TreasureChest Waterfall6 = new TreasureChest(0x31BA, nameof(Waterfall6), MapLocation.Waterfall, Item.Defense);
		public static TreasureChest Unused6 = new TreasureChest(0x31BB, nameof(Unused6), 0, Item.Heal, isUnused: true);
		public static TreasureChest Unused7 = new TreasureChest(0x31BC, nameof(Unused7), 0, Item.Heal, isUnused: true);
		public static TreasureChest Unused8 = new TreasureChest(0x31BD, nameof(Unused8), 0, Item.Heal, isUnused: true);
		public static TreasureChest Unused9 = new TreasureChest(0x31BE, nameof(Unused9), 0, Item.Heal, isUnused: true);
		public static TreasureChest Unused10 = new TreasureChest(0x31BF, nameof(Unused10), 0, Item.Heal, isUnused: true);
		public static TreasureChest Unused11 = new TreasureChest(0x31C0, nameof(Unused11), 0, Item.Heal, isUnused: true);
		public static TreasureChest Unused12 = new TreasureChest(0x31C1, nameof(Unused12), 0, Item.Heal, isUnused: true);
		public static TreasureChest Unused13 = new TreasureChest(0x31C2, nameof(Unused13), 0, Item.Heal, isUnused: true);
		public static TreasureChest Unused14 = new TreasureChest(0x31C3, nameof(Unused14), 0, Item.Heal, isUnused: true);
		public static TreasureChest MirageTower1 = new TreasureChest(0x31C4, nameof(MirageTower1), MapLocation.MirageTower1, Item.AegisShield);
		public static TreasureChest MirageTower2 = new TreasureChest(0x31C5, nameof(MirageTower2), MapLocation.MirageTower1, Item.Gold2750);
		public static TreasureChest MirageTower3 = new TreasureChest(0x31C6, nameof(MirageTower3), MapLocation.MirageTower1, Item.Gold3400);
		public static TreasureChest MirageTower4 = new TreasureChest(0x31C7, nameof(MirageTower4), MapLocation.MirageTower1, Item.Gold18010);
		public static TreasureChest MirageTower5 = new TreasureChest(0x31C8, nameof(MirageTower5), MapLocation.MirageTower1, Item.Cabin);
		public static TreasureChest MirageTower6 = new TreasureChest(0x31C9, nameof(MirageTower6), MapLocation.MirageTower1, Item.HealHelm);
		public static TreasureChest MirageTower7 = new TreasureChest(0x31CA, nameof(MirageTower7), MapLocation.MirageTower1, Item.Gold880);
		public static TreasureChest MirageTower8 = new TreasureChest(0x31CB, nameof(MirageTower8), MapLocation.MirageTower1, Item.Vorpal);
		public static TreasureChest MirageTower9 = new TreasureChest(0x31CC, nameof(MirageTower9), MapLocation.MirageTower2, Item.House);
		public static TreasureChest MirageTower10 = new TreasureChest(0x31CD, nameof(MirageTower10), MapLocation.MirageTower2, Item.Gold7690);
		public static TreasureChest MirageTower11 = new TreasureChest(0x31CE, nameof(MirageTower11), MapLocation.MirageTower2, Item.SunSword);
		public static TreasureChest MirageTower12 = new TreasureChest(0x31CF, nameof(MirageTower12), MapLocation.MirageTower2, Item.Gold10000);
		public static TreasureChest MirageTower13 = new TreasureChest(0x31D0, nameof(MirageTower13), MapLocation.MirageTower2, Item.DragonArmor);
		public static TreasureChest MirageTower14 = new TreasureChest(0x31D1, nameof(MirageTower14), MapLocation.MirageTower2, Item.Gold8135);
		public static TreasureChest MirageTower15 = new TreasureChest(0x31D2, nameof(MirageTower15), MapLocation.MirageTower2, Item.Gold7900);
		public static TreasureChest MirageTower16 = new TreasureChest(0x31D3, nameof(MirageTower16), MapLocation.MirageTower2, Item.ThorHammer);
		public static TreasureChest MirageTower17 = new TreasureChest(0x31D4, nameof(MirageTower17), MapLocation.MirageTower2, Item.Gold12350);
		public static TreasureChest MirageTower18 = new TreasureChest(0x31D5, nameof(MirageTower18), MapLocation.MirageTower2, Item.Gold13000);
		public static TreasureChest SkyPalace1 = new TreasureChest(0x31D6, nameof(SkyPalace1), MapLocation.SkyPalace1, Item.Gold9900);
		public static TreasureChest SkyPalace2 = new TreasureChest(0x31D7, nameof(SkyPalace2), MapLocation.SkyPalace1, Item.Heal);
		public static TreasureChest SkyPalace3 = new TreasureChest(0x31D8, nameof(SkyPalace3), MapLocation.SkyPalace1, Item.Gold4150);
		public static TreasureChest SkyPalace4 = new TreasureChest(0x31D9, nameof(SkyPalace4), MapLocation.SkyPalace1, Item.Gold7900);
		public static TreasureChest SkyPalace5 = new TreasureChest(0x31DA, nameof(SkyPalace5), MapLocation.SkyPalace1, Item.Gold5000);
		public static TreasureChest SkyPalace6 = new TreasureChest(0x31DB, nameof(SkyPalace6), MapLocation.SkyPalace1, Item.ProRing);
		public static TreasureChest SkyPalace7 = new TreasureChest(0x31DC, nameof(SkyPalace7), MapLocation.SkyPalace1, Item.Gold6720);
		public static TreasureChest SkyPalace8 = new TreasureChest(0x31DD, nameof(SkyPalace8), MapLocation.SkyPalace1, Item.HealHelm);
		public static TreasureChest SkyPalace9 = new TreasureChest(0x31DE, nameof(SkyPalace9), MapLocation.SkyPalace1, Item.Gold180);
		public static TreasureChest SkyPalace10 = new TreasureChest(0x31DF, nameof(SkyPalace10), MapLocation.SkyPalace1, Item.BaneSword);
		public static TreasureChest SkyPalace11 = new TreasureChest(0x31E0, nameof(SkyPalace11), MapLocation.SkyPalace2, Item.WhiteShirt);
		public static TreasureChest SkyPalace12 = new TreasureChest(0x31E1, nameof(SkyPalace12), MapLocation.SkyPalace2, Item.BlackShirt);
		public static TreasureChest SkyPalace13 = new TreasureChest(0x31E2, nameof(SkyPalace13), MapLocation.SkyPalace2, Item.Ribbon);
		public static TreasureChest SkyPalace14 = new TreasureChest(0x31E3, nameof(SkyPalace14), MapLocation.SkyPalace2, Item.OpalGauntlets);
		public static TreasureChest SkyPalace15 = new TreasureChest(0x31E4, nameof(SkyPalace15), MapLocation.SkyPalace2, Item.OpalShield);
		public static TreasureChest SkyPalace16 = new TreasureChest(0x31E5, nameof(SkyPalace16), MapLocation.SkyPalace2, Item.SilverHelm);
		public static TreasureChest SkyPalace17 = new TreasureChest(0x31E6, nameof(SkyPalace17), MapLocation.SkyPalace2, Item.House);
		public static TreasureChest SkyPalace18 = new TreasureChest(0x31E7, nameof(SkyPalace18), MapLocation.SkyPalace2, Item.Gold880);
		public static TreasureChest SkyPalace19 = new TreasureChest(0x31E8, nameof(SkyPalace19), MapLocation.SkyPalace2, Item.Gold13000);
		public static TreasureChest SkyPalaceMajor = new TreasureChest(0x31E9, nameof(SkyPalaceMajor), MapLocation.SkyPalace2, Item.Adamant);
		public static TreasureChest SkyPalace21 = new TreasureChest(0x31EA, nameof(SkyPalace21), MapLocation.SkyPalace3, Item.Gold4150);
		public static TreasureChest SkyPalace22 = new TreasureChest(0x31EB, nameof(SkyPalace22), MapLocation.SkyPalace3, Item.Soft);
		public static TreasureChest SkyPalace23 = new TreasureChest(0x31EC, nameof(SkyPalace23), MapLocation.SkyPalace3, Item.Gold3400);
		public static TreasureChest SkyPalace24 = new TreasureChest(0x31ED, nameof(SkyPalace24), MapLocation.SkyPalace3, Item.Katana);
		public static TreasureChest SkyPalace25 = new TreasureChest(0x31EE, nameof(SkyPalace25), MapLocation.SkyPalace3, Item.ProCape);
		public static TreasureChest SkyPalace26 = new TreasureChest(0x31EF, nameof(SkyPalace26), MapLocation.SkyPalace3, Item.Cloth);
		public static TreasureChest SkyPalace27 = new TreasureChest(0x31F0, nameof(SkyPalace27), MapLocation.SkyPalace3, Item.Gold9500);
		public static TreasureChest SkyPalace28 = new TreasureChest(0x31F1, nameof(SkyPalace28), MapLocation.SkyPalace3, Item.Soft);
		public static TreasureChest SkyPalace29 = new TreasureChest(0x31F2, nameof(SkyPalace29), MapLocation.SkyPalace3, Item.Gold6400);
		public static TreasureChest SkyPalace30 = new TreasureChest(0x31F3, nameof(SkyPalace30), MapLocation.SkyPalace3, Item.Gold8135);
		public static TreasureChest SkyPalace31 = new TreasureChest(0x31F4, nameof(SkyPalace31), MapLocation.SkyPalace3, Item.Gold9000);
		public static TreasureChest SkyPalace32 = new TreasureChest(0x31F5, nameof(SkyPalace32), MapLocation.SkyPalace3, Item.Heal);
		public static TreasureChest SkyPalace33 = new TreasureChest(0x31F6, nameof(SkyPalace33), MapLocation.SkyPalace3, Item.ProRing);
		public static TreasureChest SkyPalace34 = new TreasureChest(0x31F7, nameof(SkyPalace34), MapLocation.SkyPalace3, Item.Gold5450);
		public static TreasureChest ToFRMasmune = new TreasureChest(0x31F8, nameof(ToFRMasmune), MapLocation.TempleOfFiendsAir, Item.Masamune);
		public static TreasureChest ToFRevisited2 = new TreasureChest(0x31F9, nameof(ToFRevisited2), MapLocation.TempleOfFiendsFire, Item.Gold26000);
		public static TreasureChest ToFRevisited3 = new TreasureChest(0x31FA, nameof(ToFRevisited3), MapLocation.TempleOfFiendsFire, Item.Katana);
		public static TreasureChest ToFRevisited4 = new TreasureChest(0x31FB, nameof(ToFRevisited4), MapLocation.TempleOfFiendsFire, Item.ProRing);
		public static TreasureChest ToFRevisited5 = new TreasureChest(0x31FC, nameof(ToFRevisited5), MapLocation.TempleOfFiendsFire, Item.ProCape);
		public static TreasureChest ToFRevisited6 = new TreasureChest(0x31FD, nameof(ToFRevisited6), MapLocation.TempleOfFiendsPhantom, Item.Gold45000);
		public static TreasureChest ToFRevisited7 = new TreasureChest(0x31FE, nameof(ToFRevisited7), MapLocation.TempleOfFiendsPhantom, Item.Gold65000);
		public static TreasureChest Unused15 = new TreasureChest(0x31FF, nameof(Unused15), 0, 0, isUnused: true);

		public static MapObject KingConeria = new MapObject(ObjectId.King, MapLocation.ConeriaCastle2, Item.Bridge, requiredGameEventFlag: ObjectId.Princess2, requiredSecondLocation: MapLocation.TempleOfFiends1);
		public static MapObject Princess = new MapObject(ObjectId.Princess2, MapLocation.ConeriaCastle2, Item.Lute, requiredSecondLocation: MapLocation.TempleOfFiends1);
		public static MapObject Matoya = new MapObject(ObjectId.Matoya, MapLocation.MatoyasCave, Item.Herb, AccessRequirement.Crystal, requiredItemTrade: Item.Crystal);
		public static MapObject Bikke = new MapObject(ObjectId.Bikke, MapLocation.Pravoka, Item.Ship);
		// Assumption is made that if you have access to the Elf Prince you also have access to the Elf Doc
		public static MapObject ElfPrince = new MapObject(ObjectId.ElfPrince, MapLocation.ElflandCastle, Item.Key, AccessRequirement.Herb, ObjectId.ElfDoc);
		public static MapObject Astos = new MapObject(ObjectId.Astos, MapLocation.NorthwestCastle, Item.Crystal, AccessRequirement.Crown);
		public static MapObject Sarda = new MapObject(ObjectId.Sarda, MapLocation.SardasCave, Item.Rod, requiredGameEventFlag: ObjectId.Vampire, requiredSecondLocation: MapLocation.EarthCaveVampire);
		public static MapObject CanoeSage = new MapObject(ObjectId.CanoeSage, MapLocation.CrescentLake, Item.Canoe, AccessRequirement.EarthOrb, requiredItemTrade: Item.EarthOrb);
		public static MapObject CubeBot = new MapObject(ObjectId.CubeBot, MapLocation.Waterfall, Item.Cube);
		public static MapObject Fairy = new MapObject(ObjectId.Fairy, MapLocation.Gaia, Item.Oxyale, AccessRequirement.Bottle);
		// Assumption is made that if you have the slab and access to Lefein then you also have access to Unne
		public static MapObject Lefein = new MapObject(ObjectId.Lefein, MapLocation.Lefein, Item.Chime, AccessRequirement.Slab, ObjectId.Unne, requiredSecondLocation: MapLocation.Melmond);
		public static MapObject Nerrick = new MapObject(ObjectId.Nerrick, MapLocation.DwarfCave, Item.Canal, AccessRequirement.Tnt, requiredItemTrade: Item.Tnt);
		public static MapObject Smith = new MapObject(ObjectId.Smith, MapLocation.DwarfCave, Item.Xcalber, AccessRequirement.Adamant, requiredItemTrade: Item.Adamant);

		public static ItemShopSlot CaravanItemShop1 =
			new ItemShopSlot(0x38461, nameof(CaravanItemShop1), MapLocation.Caravan, Item.Bottle, 69);

		public static StaticItemLocation LichReward =
			new StaticItemLocation(nameof(LichReward), MapLocation.EarthCaveLich, Item.EarthOrb);
		public static StaticItemLocation KaryReward =
			new StaticItemLocation(nameof(KaryReward), MapLocation.GurguVolcanoKary, Item.FireOrb);
		public static StaticItemLocation KrakenReward =
			new StaticItemLocation(nameof(KrakenReward), MapLocation.SeaShrineKraken, Item.WaterOrb);
		public static StaticItemLocation TiamatReward =
			new StaticItemLocation(nameof(TiamatReward), MapLocation.SkyPalaceTiamat, Item.AirOrb);
		public static StaticItemLocation ChaosReward =
			new StaticItemLocation(nameof(ChaosReward), MapLocation.TempleOfFiendsChaos, Item.None, AccessRequirement.BlackOrb);

		private const MapChange AirshipAndCanoe = MapChange.Airship | MapChange.Canoe;
		private const MapChange CanalAndShip = MapChange.Canal | MapChange.Ship;
		private const MapChange ShipAndCanoe = MapChange.Canoe | MapChange.Ship;
		public static Dictionary<MapLocation, IEnumerable<MapChange>> MapLocationRequirements =>
			new Dictionary<MapLocation, IEnumerable<MapChange>>
		{
			{MapLocation.StartingLocation, new List<MapChange>{ MapChange.None }},
			{MapLocation.Coneria, new List<MapChange>{ MapChange.None }},
			{MapLocation.ConeriaCastle1, new List<MapChange>{ MapChange.None }},
			{MapLocation.TempleOfFiends1, new List<MapChange>{ MapChange.None }},
			{MapLocation.MatoyasCave, new List<MapChange>{ MapChange.Bridge, MapChange.Ship, MapChange.Airship }},
			{MapLocation.Pravoka, new List<MapChange>{ MapChange.Bridge, MapChange.Ship, MapChange.Airship }},
			{MapLocation.DwarfCave, new List<MapChange>{ MapChange.Ship, MapChange.Airship }},
			{MapLocation.Elfland, new List<MapChange>{ MapChange.Ship, MapChange.Airship }},
			{MapLocation.ElflandCastle, new List<MapChange>{ MapChange.Ship, MapChange.Airship }},
			{MapLocation.NorthwestCastle, new List<MapChange>{ MapChange.Ship, MapChange.Airship }},
			{MapLocation.MarshCave1, new List<MapChange>{ MapChange.Ship, MapChange.Airship }},
			{MapLocation.Melmond, new List<MapChange>{ CanalAndShip, MapChange.Airship }},
			{MapLocation.EarthCave1, new List<MapChange>{ CanalAndShip, MapChange.Airship }},
			{MapLocation.TitansTunnelEast, new List<MapChange>{ CanalAndShip, MapChange.Airship }},
			{MapLocation.TitansTunnelWest, new List<MapChange>{ CanalAndShip | MapChange.TitanFed, MapChange.Airship }},
			{MapLocation.SardasCave, new List<MapChange>{ CanalAndShip | MapChange.TitanFed, MapChange.Airship }},
			{MapLocation.CrescentLake, new List<MapChange>{ CanalAndShip, ShipAndCanoe, MapChange.Airship }},
			{MapLocation.GurguVolcano1, new List<MapChange>{ ShipAndCanoe, MapChange.Airship }},
			{MapLocation.IceCave1, new List<MapChange>{ MapChange.Bridge | MapChange.Canoe, MapChange.Ship | MapChange.Canoe, MapChange.Airship }},
			{MapLocation.CastleOrdeals1, new List<MapChange>{ CanalAndShip | MapChange.Canoe, AirshipAndCanoe }},
			{MapLocation.Cardia1, new List<MapChange>{ MapChange.Airship }},
			{MapLocation.Cardia2, new List<MapChange>{ MapChange.Airship }},
			{MapLocation.BahamutCave1, new List<MapChange>{ MapChange.Airship }},
			{MapLocation.Cardia4, new List<MapChange>{ MapChange.Airship }},
			{MapLocation.Cardia5, new List<MapChange>{ MapChange.Airship }},
			{MapLocation.Cardia6, new List<MapChange>{ MapChange.Airship }},
			{MapLocation.Caravan, new List<MapChange>{ MapChange.Airship }},
			{MapLocation.Gaia, new List<MapChange>{ MapChange.Airship }},
			{MapLocation.Onrac, new List<MapChange>{ AirshipAndCanoe }},
			{MapLocation.Waterfall, new List<MapChange>{ AirshipAndCanoe }},
			{MapLocation.Lefein, new List<MapChange>{ MapChange.Airship }},
			{MapLocation.MirageTower1, new List<MapChange>{ MapChange.Chime | MapChange.Airship }},
			{MapLocation.AirshipLocation, new List<MapChange>{ ShipAndCanoe }}
		};
		public static Dictionary<MapLocation, Tuple<MapLocation, AccessRequirement>> MapLocationFloorRequirements =>
			new Dictionary<MapLocation, Tuple<MapLocation, AccessRequirement>>
		{
			{MapLocation.ConeriaCastleRoom1, new Tuple<MapLocation, AccessRequirement>(MapLocation.ConeriaCastle1, AccessRequirement.Key)},
			{MapLocation.ConeriaCastleRoom2, new Tuple<MapLocation, AccessRequirement>(MapLocation.ConeriaCastle1, AccessRequirement.Key)},
			{MapLocation.ConeriaCastle2, new Tuple<MapLocation, AccessRequirement>(MapLocation.ConeriaCastle1, AccessRequirement.None)},
			{MapLocation.TempleOfFiends1Room1, new Tuple<MapLocation, AccessRequirement>(MapLocation.TempleOfFiends1, AccessRequirement.None)},
			{MapLocation.TempleOfFiends1Room2, new Tuple<MapLocation, AccessRequirement>(MapLocation.TempleOfFiends1, AccessRequirement.Key)},
			{MapLocation.TempleOfFiends1Room3, new Tuple<MapLocation, AccessRequirement>(MapLocation.TempleOfFiends1, AccessRequirement.None)},
			{MapLocation.TempleOfFiends1Room4, new Tuple<MapLocation, AccessRequirement>(MapLocation.TempleOfFiends1, AccessRequirement.Key)},
			{MapLocation.DwarfCaveRoom3, new Tuple<MapLocation, AccessRequirement>(MapLocation.DwarfCave, AccessRequirement.Key)},
			{MapLocation.ElflandCastleRoom1, new Tuple<MapLocation, AccessRequirement>(MapLocation.ElflandCastle, AccessRequirement.Key)},
			{MapLocation.NorthwestCastleRoom2, new Tuple<MapLocation, AccessRequirement>(MapLocation.NorthwestCastle, AccessRequirement.Key)},
			{MapLocation.MarshCaveTop, new Tuple<MapLocation, AccessRequirement>(MapLocation.MarshCave1, AccessRequirement.None)},
			{MapLocation.MarshCave3, new Tuple<MapLocation, AccessRequirement>(MapLocation.MarshCave1, AccessRequirement.None)},
			{MapLocation.MarshCaveBottom, new Tuple<MapLocation, AccessRequirement>(MapLocation.MarshCave3, AccessRequirement.None)},
			{MapLocation.MarshCaveBottomRoom13, new Tuple<MapLocation, AccessRequirement>(MapLocation.MarshCaveBottom, AccessRequirement.Key)},
			{MapLocation.MarshCaveBottomRoom14, new Tuple<MapLocation, AccessRequirement>(MapLocation.MarshCaveBottom, AccessRequirement.Key)},
			{MapLocation.MarshCaveBottomRoom16, new Tuple<MapLocation, AccessRequirement>(MapLocation.MarshCaveBottom, AccessRequirement.Key)},
			{MapLocation.CastleOrdealsMaze, new Tuple<MapLocation, AccessRequirement>(MapLocation.CastleOrdeals1, AccessRequirement.Crown)},
			{MapLocation.CastleOrdealsTop, new Tuple<MapLocation, AccessRequirement>(MapLocation.CastleOrdealsMaze, AccessRequirement.None)},
			{MapLocation.EarthCave2, new Tuple<MapLocation, AccessRequirement>(MapLocation.EarthCave1, AccessRequirement.None)},
			{MapLocation.EarthCaveVampire, new Tuple<MapLocation, AccessRequirement>(MapLocation.EarthCave2, AccessRequirement.None)},
			{MapLocation.EarthCave4, new Tuple<MapLocation, AccessRequirement>(MapLocation.EarthCaveVampire, AccessRequirement.Rod)},
			{MapLocation.EarthCaveLich, new Tuple<MapLocation, AccessRequirement>(MapLocation.EarthCave4, AccessRequirement.None)},
			{MapLocation.GurguVolcano2, new Tuple<MapLocation, AccessRequirement>(MapLocation.GurguVolcano1, AccessRequirement.None)},
			{MapLocation.GurguVolcano3, new Tuple<MapLocation, AccessRequirement>(MapLocation.GurguVolcano2, AccessRequirement.None)},
			{MapLocation.GurguVolcano4, new Tuple<MapLocation, AccessRequirement>(MapLocation.GurguVolcano3, AccessRequirement.None)},
			{MapLocation.GurguVolcano5, new Tuple<MapLocation, AccessRequirement>(MapLocation.GurguVolcano4, AccessRequirement.None)},
			{MapLocation.GurguVolcano6, new Tuple<MapLocation, AccessRequirement>(MapLocation.GurguVolcano5, AccessRequirement.None)},
			{MapLocation.GurguVolcanoKary, new Tuple<MapLocation, AccessRequirement>(MapLocation.GurguVolcano6, AccessRequirement.None)},
			{MapLocation.IceCave2, new Tuple<MapLocation, AccessRequirement>(MapLocation.IceCave1, AccessRequirement.None)},
			{MapLocation.IceCave3, new Tuple<MapLocation, AccessRequirement>(MapLocation.IceCave2, AccessRequirement.None)},
			{MapLocation.IceCavePitRoom, new Tuple<MapLocation, AccessRequirement>(MapLocation.IceCave3, AccessRequirement.None)},
			{MapLocation.IceCave5, new Tuple<MapLocation, AccessRequirement>(MapLocation.IceCavePitRoom, AccessRequirement.None)},
			{MapLocation.IceCaveBackExit, new Tuple<MapLocation, AccessRequirement>(MapLocation.IceCave5, AccessRequirement.None)},
			{MapLocation.IceCaveFloater, new Tuple<MapLocation, AccessRequirement>(MapLocation.IceCaveBackExit, AccessRequirement.None)},
			{MapLocation.BahamutCave2, new Tuple<MapLocation, AccessRequirement>(MapLocation.BahamutCave1, AccessRequirement.None)},
			{MapLocation.MirageTower2, new Tuple<MapLocation, AccessRequirement>(MapLocation.MirageTower1, AccessRequirement.None)},
			{MapLocation.MirageTower3, new Tuple<MapLocation, AccessRequirement>(MapLocation.MirageTower2, AccessRequirement.None)},
			{MapLocation.SkyPalace1, new Tuple<MapLocation, AccessRequirement>(MapLocation.MirageTower3, AccessRequirement.Cube)},
			{MapLocation.SkyPalace2, new Tuple<MapLocation, AccessRequirement>(MapLocation.SkyPalace1, AccessRequirement.None)},
			{MapLocation.SkyPalace3, new Tuple<MapLocation, AccessRequirement>(MapLocation.SkyPalace2, AccessRequirement.None)},
			{MapLocation.SkyPalaceMaze, new Tuple<MapLocation, AccessRequirement>(MapLocation.SkyPalace3, AccessRequirement.None)},
			{MapLocation.SkyPalaceTiamat, new Tuple<MapLocation, AccessRequirement>(MapLocation.SkyPalaceMaze, AccessRequirement.None)},
			{MapLocation.SeaShrine1, new Tuple<MapLocation, AccessRequirement>(MapLocation.Onrac, AccessRequirement.Oxyale)},
			{MapLocation.SeaShrine2, new Tuple<MapLocation, AccessRequirement>(MapLocation.SeaShrine1, AccessRequirement.None)},
			{MapLocation.SeaShrine2Room2, new Tuple<MapLocation, AccessRequirement>(MapLocation.SeaShrine2, AccessRequirement.Key)},
			{MapLocation.SeaShrineMermaids, new Tuple<MapLocation, AccessRequirement>(MapLocation.SeaShrine2, AccessRequirement.None)},
			{MapLocation.SeaShrine4, new Tuple<MapLocation, AccessRequirement>(MapLocation.SeaShrine1, AccessRequirement.None)},
			{MapLocation.SeaShrine5, new Tuple<MapLocation, AccessRequirement>(MapLocation.SeaShrine4, AccessRequirement.None)},
			{MapLocation.SeaShrine6, new Tuple<MapLocation, AccessRequirement>(MapLocation.SeaShrine5, AccessRequirement.None)},
			{MapLocation.SeaShrine7, new Tuple<MapLocation, AccessRequirement>(MapLocation.SeaShrine6, AccessRequirement.None)},
			{MapLocation.SeaShrine8, new Tuple<MapLocation, AccessRequirement>(MapLocation.SeaShrine7, AccessRequirement.None)},
			{MapLocation.SeaShrineKraken, new Tuple<MapLocation, AccessRequirement>(MapLocation.SeaShrine8, AccessRequirement.None)},
			{MapLocation.TempleOfFiends2, new Tuple<MapLocation, AccessRequirement>(MapLocation.TempleOfFiends1, AccessRequirement.BlackOrb)},
			{MapLocation.TempleOfFiends3, new Tuple<MapLocation, AccessRequirement>(MapLocation.TempleOfFiends2, AccessRequirement.None)},
			{MapLocation.TempleOfFiendsPhantom, new Tuple<MapLocation, AccessRequirement>(MapLocation.TempleOfFiends3, AccessRequirement.None)},
			{MapLocation.TempleOfFiendsEarth, new Tuple<MapLocation, AccessRequirement>(MapLocation.TempleOfFiendsPhantom, AccessRequirement.Lute)},
			{MapLocation.TempleOfFiendsFire, new Tuple<MapLocation, AccessRequirement>(MapLocation.TempleOfFiendsEarth, AccessRequirement.Key)},
			{MapLocation.TempleOfFiendsWater, new Tuple<MapLocation, AccessRequirement>(MapLocation.TempleOfFiendsFire, AccessRequirement.Key)},
			{MapLocation.TempleOfFiendsAir, new Tuple<MapLocation, AccessRequirement>(MapLocation.TempleOfFiendsWater, AccessRequirement.Key)},
			{MapLocation.TempleOfFiendsChaos, new Tuple<MapLocation, AccessRequirement>(MapLocation.TempleOfFiendsAir, AccessRequirement.Key)},
			{MapLocation.TitansTunnelRoom, new Tuple<MapLocation, AccessRequirement>(MapLocation.TitansTunnelWest, AccessRequirement.None)},
		};

		public static Dictionary<MapLocation, RomUtilities.Blob> ShipLocations = new Dictionary<MapLocation, RomUtilities.Blob>
		{
			{MapLocation.ConeriaCastle1, Dock.Coneria},
			{MapLocation.ConeriaCastle2, Dock.Coneria},
			{MapLocation.Coneria, Dock.Coneria},
			{MapLocation.TempleOfFiends1, Dock.Coneria},
			{MapLocation.DwarfCave, Dock.DwarfCave},
			{MapLocation.DwarfCaveRoom3, Dock.DwarfCave},
			{MapLocation.MatoyasCave, Dock.MatoyasCave},
			{MapLocation.Pravoka, Dock.Pravoka},
			{MapLocation.IceCave1, Dock.Pravoka},
			{MapLocation.IceCave2, Dock.Pravoka},
			{MapLocation.IceCave3, Dock.Pravoka},
			{MapLocation.IceCave5, Dock.Pravoka},
			{MapLocation.IceCaveBackExit, Dock.Pravoka},
			{MapLocation.IceCaveFloater, Dock.Pravoka},
			{MapLocation.IceCavePitRoom, Dock.Pravoka},
			{MapLocation.ElflandCastle, Dock.Elfland},
			{MapLocation.ElflandCastleRoom1, Dock.Elfland},
			{MapLocation.Elfland, Dock.Elfland},
			{MapLocation.MarshCave1, Dock.Elfland},
			{MapLocation.MarshCaveTop, Dock.Elfland},
			{MapLocation.MarshCave3, Dock.Elfland},
			{MapLocation.MarshCaveBottom, Dock.Elfland},
			{MapLocation.MarshCaveBottomRoom13, Dock.Elfland},
			{MapLocation.MarshCaveBottomRoom14, Dock.Elfland},
			{MapLocation.MarshCaveBottomRoom16, Dock.Elfland},
			{MapLocation.NorthwestCastle, Dock.Elfland},
			{MapLocation.NorthwestCastleRoom2, Dock.Elfland},
			{MapLocation.CrescentLake, Dock.Elfland},
			{MapLocation.GurguVolcano1, Dock.Elfland},
			{MapLocation.GurguVolcano2, Dock.Elfland},
			{MapLocation.GurguVolcano3, Dock.Elfland},
			{MapLocation.GurguVolcano4, Dock.Elfland},
			{MapLocation.GurguVolcano5, Dock.Elfland},
			{MapLocation.GurguVolcano6, Dock.Elfland},
			{MapLocation.GurguVolcanoKary, Dock.Elfland},
			{MapLocation.Gaia, Dock.Pravoka},
			{MapLocation.Lefein, Dock.Pravoka},
			{MapLocation.MirageTower1, Dock.Pravoka},
			{MapLocation.MirageTower2, Dock.Pravoka},
			{MapLocation.MirageTower3, Dock.Pravoka},
			{MapLocation.SkyPalace1, Dock.Pravoka},
			{MapLocation.SkyPalace2, Dock.Pravoka},
			{MapLocation.SkyPalace3, Dock.Pravoka},
			{MapLocation.SkyPalaceMaze, Dock.Pravoka},
			{MapLocation.SkyPalaceTiamat, Dock.Pravoka},
			{MapLocation.CastleOrdeals1, Dock.Pravoka},
			{MapLocation.CastleOrdealsMaze, Dock.Pravoka},
			{MapLocation.CastleOrdealsTop, Dock.Pravoka},
			{MapLocation.Melmond, Dock.DwarfCave},
			{MapLocation.TitansTunnelRoom, Dock.DwarfCave},
			{MapLocation.TitansTunnelEast, Dock.DwarfCave},
			{MapLocation.TitansTunnelWest, Dock.DwarfCave},
			{MapLocation.EarthCave1, Dock.DwarfCave},
			{MapLocation.EarthCave2, Dock.DwarfCave},
			{MapLocation.EarthCaveVampire, Dock.DwarfCave},
			{MapLocation.EarthCave4, Dock.DwarfCave},
			{MapLocation.EarthCaveLich, Dock.DwarfCave},
			{MapLocation.SardasCave, Dock.DwarfCave},
		};
		public static Dictionary<OverworldTeleportIndex, MapLocation> OverworldToMapLocation = new Dictionary<OverworldTeleportIndex, MapLocation>
		{
			{OverworldTeleportIndex.Cardia1, MapLocation.Cardia1},
			{OverworldTeleportIndex.Coneria, MapLocation.Coneria},
			{OverworldTeleportIndex.Pravoka, MapLocation.Pravoka},
			{OverworldTeleportIndex.Elfland, MapLocation.Elfland},
			{OverworldTeleportIndex.Melmond, MapLocation.Melmond},
			{OverworldTeleportIndex.CrescentLake, MapLocation.CrescentLake},
			{OverworldTeleportIndex.Gaia, MapLocation.Gaia},
			{OverworldTeleportIndex.Onrac, MapLocation.Onrac},
			{OverworldTeleportIndex.Lefein, MapLocation.Lefein},
			{OverworldTeleportIndex.ConeriaCastle1, MapLocation.ConeriaCastle1},
			{OverworldTeleportIndex.ElflandCastle, MapLocation.ElflandCastle},
			{OverworldTeleportIndex.NorthwestCastle, MapLocation.NorthwestCastle},
			{OverworldTeleportIndex.CastleOrdeals1, MapLocation.CastleOrdeals1},
			{OverworldTeleportIndex.TempleOfFiends1, MapLocation.TempleOfFiends1},
			{OverworldTeleportIndex.EarthCave1, MapLocation.EarthCave1},
			{OverworldTeleportIndex.GurguVolcano1, MapLocation.GurguVolcano1},
			{OverworldTeleportIndex.IceCave1, MapLocation.IceCave1},
			{OverworldTeleportIndex.Cardia2, MapLocation.Cardia2},
			{OverworldTeleportIndex.BahamutCave1, MapLocation.BahamutCave1},
			{OverworldTeleportIndex.Waterfall, MapLocation.Waterfall},
			{OverworldTeleportIndex.DwarfCave, MapLocation.DwarfCave},
			{OverworldTeleportIndex.MatoyasCave, MapLocation.MatoyasCave},
			{OverworldTeleportIndex.SardasCave, MapLocation.SardasCave},
			{OverworldTeleportIndex.MarshCave1, MapLocation.MarshCave1},
			{OverworldTeleportIndex.MirageTower1, MapLocation.MirageTower1},
			{OverworldTeleportIndex.TitansTunnelEast, MapLocation.TitansTunnelEast},
			{OverworldTeleportIndex.TitansTunnelWest, MapLocation.TitansTunnelWest},
			{OverworldTeleportIndex.Cardia4, MapLocation.Cardia4},
			{OverworldTeleportIndex.Cardia5, MapLocation.Cardia5},
			{OverworldTeleportIndex.Cardia6, MapLocation.Cardia6},
		};

		public static Dictionary<MapLocation, List<TreasureChest>> GetTreasureDictionary()
		{
			var fields = typeof(ItemLocations).GetFields(BindingFlags.Public | BindingFlags.Static);
			return fields.Where(f => f.FieldType == typeof(TreasureChest))
				.Select(f => f.GetValue(null) as TreasureChest)
				.Where(t => t != null)
				.GroupBy(t => t.MapLocation)
				.ToDictionary(g => g.Key, g => g.ToList());
		}

		public static Dictionary<int, MapLocation> GetTreasuresMapLocation()
		{
			const int lut_TreasureOffset = 0x3100;
			var fields = typeof(ItemLocations).GetFields(BindingFlags.Public | BindingFlags.Static);
			return fields.Where(f => f.FieldType == typeof(TreasureChest))
				.Select(f => f.GetValue(null) as TreasureChest)
				.Where(t => t != null)
				.ToDictionary(g => (g.Address - lut_TreasureOffset), g => g.MapLocation);
		}

		public static Dictionary<MapLocation, MapIndex> MapLocationToMapIndex =
	new Dictionary<MapLocation, MapIndex>
	{
				{MapLocation.StartingLocation, MapIndex.ConeriaTown},
				{MapLocation.AirshipLocation, MapIndex.ConeriaTown},
				{MapLocation.Coneria, MapIndex.ConeriaTown},
				{MapLocation.Pravoka, MapIndex.Pravoka},
				{MapLocation.Elfland, MapIndex.Elfland},
				{MapLocation.Melmond, MapIndex.Melmond},
				{MapLocation.CrescentLake, MapIndex.CrescentLake},
				{MapLocation.Gaia,MapIndex.Gaia},
				{MapLocation.Onrac,MapIndex.Onrac},
				{MapLocation.Lefein,MapIndex.Lefein},
				{MapLocation.ConeriaCastle1,MapIndex.ConeriaCastle1F},
				{MapLocation.ConeriaCastle2,MapIndex.ConeriaCastle2F},
				{MapLocation.ConeriaCastleRoom1,MapIndex.ConeriaCastle1F},
				{MapLocation.ConeriaCastleRoom2,MapIndex.ConeriaCastle1F},
				{MapLocation.ElflandCastle,MapIndex.ElflandCastle},
				{MapLocation.ElflandCastleRoom1,MapIndex.ElflandCastle},
				{MapLocation.NorthwestCastle,MapIndex.NorthwestCastle},
				{MapLocation.NorthwestCastleRoom2,MapIndex.NorthwestCastle},
				{MapLocation.CastleOrdeals1,MapIndex.CastleOrdeals1F},
				{MapLocation.CastleOrdealsMaze,MapIndex.CastleOrdeals2F},
				{MapLocation.CastleOrdealsTop,MapIndex.CastleOrdeals3F},
				{MapLocation.TempleOfFiends1,MapIndex.TempleOfFiends},
				{MapLocation.TempleOfFiends1Room1,MapIndex.TempleOfFiends},
				{MapLocation.TempleOfFiends1Room2,MapIndex.TempleOfFiends},
				{MapLocation.TempleOfFiends1Room3,MapIndex.TempleOfFiends},
				{MapLocation.TempleOfFiends1Room4,MapIndex.TempleOfFiends},
				{MapLocation.TempleOfFiends2,MapIndex.TempleOfFiendsRevisited1F},
				{MapLocation.TempleOfFiends3,MapIndex.TempleOfFiendsRevisited3F},
				{MapLocation.TempleOfFiendsChaos,MapIndex.TempleOfFiendsRevisitedChaos},
				{MapLocation.TempleOfFiendsAir,MapIndex.TempleOfFiendsRevisitedAir},
				{MapLocation.TempleOfFiendsEarth,MapIndex.TempleOfFiendsRevisitedEarth},
				{MapLocation.TempleOfFiendsFire,MapIndex.TempleOfFiendsRevisitedFire},
				{MapLocation.TempleOfFiendsWater,MapIndex.TempleOfFiendsRevisitedWater},
				{MapLocation.TempleOfFiendsPhantom,MapIndex.TempleOfFiendsRevisited2F},
				{MapLocation.EarthCave1,MapIndex.EarthCaveB1},
				{MapLocation.EarthCave2,MapIndex.EarthCaveB2},
				{MapLocation.EarthCaveVampire,MapIndex.EarthCaveB3},
				{MapLocation.EarthCave4,MapIndex.EarthCaveB4},
				{MapLocation.EarthCaveLich,MapIndex.EarthCaveB5},
				{MapLocation.GurguVolcano1,MapIndex.GurguVolcanoB1},
				{MapLocation.GurguVolcano2,MapIndex.GurguVolcanoB2},
				{MapLocation.GurguVolcano3,MapIndex.GurguVolcanoB3},
				{MapLocation.GurguVolcano4,MapIndex.GurguVolcanoB4},
				{MapLocation.GurguVolcano5,MapIndex.GurguVolcanoB3},
				{MapLocation.GurguVolcano6,MapIndex.GurguVolcanoB4},
				{MapLocation.GurguVolcanoKary,MapIndex.GurguVolcanoB5},
				{MapLocation.IceCave1,MapIndex.IceCaveB1},
				{MapLocation.IceCave2,MapIndex.IceCaveB2},
				{MapLocation.IceCave3,MapIndex.IceCaveB3},
				{MapLocation.IceCave5,MapIndex.IceCaveB3},
				{MapLocation.IceCaveBackExit,MapIndex.IceCaveB1},
				{MapLocation.IceCaveFloater,MapIndex.IceCaveB2},
				{MapLocation.IceCavePitRoom,MapIndex.IceCaveB2},
				{MapLocation.SeaShrine1, MapIndex.SeaShrineB3 },
				{MapLocation.SeaShrine2, MapIndex.SeaShrineB2 },
				{MapLocation.SeaShrine2Room2, MapIndex.SeaShrineB2 },
				{MapLocation.SeaShrine4, MapIndex.SeaShrineB4 },
				{MapLocation.SeaShrine5, MapIndex.SeaShrineB3 },
				{MapLocation.SeaShrine6, MapIndex.SeaShrineB2 },
				{MapLocation.SeaShrine7, MapIndex.SeaShrineB3 },
				{MapLocation.SeaShrine8, MapIndex.SeaShrineB4 },
				{MapLocation.SeaShrineKraken, MapIndex.SeaShrineB5 },
				{MapLocation.SeaShrineMermaids, MapIndex.SeaShrineB1 },
				{MapLocation.Cardia1,MapIndex.Cardia},
				{MapLocation.Cardia2,MapIndex.Cardia},
				{MapLocation.BahamutCave1,MapIndex.BahamutCaveB1},
				{MapLocation.BahamutCave2,MapIndex.BahamutCaveB1},
				{MapLocation.Cardia4,MapIndex.Cardia},
				{MapLocation.Cardia5,MapIndex.Cardia},
				{MapLocation.Cardia6,MapIndex.Cardia},
				{MapLocation.Waterfall,MapIndex.Waterfall},
				{MapLocation.DwarfCave,MapIndex.DwarfCave},
				{MapLocation.DwarfCaveRoom3,MapIndex.DwarfCave},
				{MapLocation.MatoyasCave,MapIndex.MatoyasCave},
				{MapLocation.SardasCave,MapIndex.SardasCave},
				{MapLocation.MarshCave1,MapIndex.MarshCaveB1},
				{MapLocation.MarshCave3,MapIndex.MarshCaveB2},
				{MapLocation.MarshCaveBottom,MapIndex.MarshCaveB3},
				{MapLocation.MarshCaveBottomRoom13,MapIndex.MarshCaveB3},
				{MapLocation.MarshCaveBottomRoom14,MapIndex.MarshCaveB3},
				{MapLocation.MarshCaveBottomRoom16,MapIndex.MarshCaveB3},
				{MapLocation.MarshCaveTop,MapIndex.MarshCaveB2},
				{MapLocation.MirageTower1,MapIndex.MirageTower1F},
				{MapLocation.MirageTower2,MapIndex.MirageTower2F},
				{MapLocation.MirageTower3,MapIndex.MirageTower3F},
				{MapLocation.SkyPalace1,MapIndex.SkyPalace1F},
				{MapLocation.SkyPalace2,MapIndex.SkyPalace2F},
				{MapLocation.SkyPalace3,MapIndex.SkyPalace3F},
				{MapLocation.SkyPalaceMaze,MapIndex.SkyPalace4F},
				{MapLocation.SkyPalaceTiamat,MapIndex.SkyPalace5F},
				{MapLocation.TitansTunnelEast,MapIndex.TitansTunnel},
				{MapLocation.TitansTunnelWest,MapIndex.TitansTunnel},
				{MapLocation.TitansTunnelRoom,MapIndex.TitansTunnel},
				{MapLocation.Caravan, (MapIndex)61},
	};

		public static Dictionary<MapLocation, OverworldTeleportIndex> MapLocationToStandardOverworldLocations =
			new Dictionary<MapLocation, OverworldTeleportIndex>
			{
				{MapLocation.Coneria,OverworldTeleportIndex.Coneria},
				{MapLocation.Caravan,(OverworldTeleportIndex)36},
				{MapLocation.Pravoka, OverworldTeleportIndex.Pravoka},
				{MapLocation.Elfland, OverworldTeleportIndex.Elfland},
				{MapLocation.Melmond, OverworldTeleportIndex.Melmond},
				{MapLocation.CrescentLake, OverworldTeleportIndex.CrescentLake},
				{MapLocation.Gaia,OverworldTeleportIndex.Gaia},
				{MapLocation.Onrac,OverworldTeleportIndex.Onrac},
				{MapLocation.Lefein,OverworldTeleportIndex.Lefein},
				{MapLocation.ConeriaCastle1,OverworldTeleportIndex.ConeriaCastle1},
				{MapLocation.ConeriaCastle2,OverworldTeleportIndex.ConeriaCastle1},
				{MapLocation.ConeriaCastleRoom1,OverworldTeleportIndex.ConeriaCastle1},
				{MapLocation.ConeriaCastleRoom2,OverworldTeleportIndex.ConeriaCastle1},
				{MapLocation.ElflandCastle,OverworldTeleportIndex.ElflandCastle},
				{MapLocation.ElflandCastleRoom1,OverworldTeleportIndex.ElflandCastle},
				{MapLocation.NorthwestCastle,OverworldTeleportIndex.NorthwestCastle},
				{MapLocation.NorthwestCastleRoom2,OverworldTeleportIndex.NorthwestCastle},
				{MapLocation.CastleOrdeals1,OverworldTeleportIndex.CastleOrdeals1},
				{MapLocation.CastleOrdealsMaze,OverworldTeleportIndex.CastleOrdeals1},
				{MapLocation.CastleOrdealsTop,OverworldTeleportIndex.CastleOrdeals1},
				{MapLocation.TempleOfFiends1,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends1Room1,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends1Room2,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends1Room3,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends1Room4,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends2,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends3,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsChaos,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsAir,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsEarth,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsFire,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsWater,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsPhantom,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.EarthCave1,OverworldTeleportIndex.EarthCave1},
				{MapLocation.EarthCave2,OverworldTeleportIndex.EarthCave1},
				{MapLocation.EarthCaveVampire,OverworldTeleportIndex.EarthCave1},
				{MapLocation.EarthCave4,OverworldTeleportIndex.EarthCave1},
				{MapLocation.EarthCaveLich,OverworldTeleportIndex.EarthCave1},
				{MapLocation.GurguVolcano1,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcano2,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcano3,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcano4,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcano5,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcano6,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcanoKary,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.IceCave1,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCave2,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCave3,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCave5,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCaveBackExit,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCaveFloater,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCavePitRoom,OverworldTeleportIndex.IceCave1},
				{MapLocation.SeaShrine1, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine2, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine2Room2, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine4, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine5, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine6, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine7, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine8, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrineKraken, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrineMermaids, (OverworldTeleportIndex)35},
				{MapLocation.Cardia1,OverworldTeleportIndex.Cardia1},
				{MapLocation.Cardia2,OverworldTeleportIndex.Cardia2},
				{MapLocation.BahamutCave1,OverworldTeleportIndex.BahamutCave1},
				{MapLocation.BahamutCave2,OverworldTeleportIndex.BahamutCave1},
				{MapLocation.Cardia4,OverworldTeleportIndex.Cardia4},
				{MapLocation.Cardia5,OverworldTeleportIndex.Cardia5},
				{MapLocation.Cardia6,OverworldTeleportIndex.Cardia6},
				{MapLocation.Waterfall,OverworldTeleportIndex.Waterfall},
				{MapLocation.DwarfCave,OverworldTeleportIndex.DwarfCave},
				{MapLocation.DwarfCaveRoom3,OverworldTeleportIndex.DwarfCave},
				{MapLocation.MatoyasCave,OverworldTeleportIndex.MatoyasCave},
				{MapLocation.SardasCave,OverworldTeleportIndex.SardasCave},
				{MapLocation.MarshCave1,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCave3,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCaveBottom,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCaveBottomRoom13,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCaveBottomRoom14,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCaveBottomRoom16,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCaveTop,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MirageTower1,OverworldTeleportIndex.MirageTower1},
				{MapLocation.MirageTower2,OverworldTeleportIndex.MirageTower1},
				{MapLocation.MirageTower3,OverworldTeleportIndex.MirageTower1},
				{MapLocation.SkyPalace1,(OverworldTeleportIndex)37},
				{MapLocation.SkyPalace2,(OverworldTeleportIndex)37},
				{MapLocation.SkyPalace3,(OverworldTeleportIndex)37},
				{MapLocation.SkyPalaceMaze,(OverworldTeleportIndex)37},
				{MapLocation.SkyPalaceTiamat,(OverworldTeleportIndex)37},
				{MapLocation.TitansTunnelEast,OverworldTeleportIndex.TitansTunnelEast},
				{MapLocation.TitansTunnelWest,OverworldTeleportIndex.TitansTunnelWest},
				{MapLocation.TitansTunnelRoom,OverworldTeleportIndex.TitansTunnelWest},
			};
		public static Dictionary<MapLocation, OverworldTeleportIndex> MapLocationToOverworldLocations =
			new Dictionary<MapLocation, OverworldTeleportIndex>
			{
				{MapLocation.Coneria,OverworldTeleportIndex.Coneria},
				{MapLocation.Caravan,(OverworldTeleportIndex)36},
				{MapLocation.Pravoka, OverworldTeleportIndex.Pravoka},
				{MapLocation.Elfland, OverworldTeleportIndex.Elfland},
				{MapLocation.Melmond, OverworldTeleportIndex.Melmond},
				{MapLocation.CrescentLake, OverworldTeleportIndex.CrescentLake},
				{MapLocation.Gaia,OverworldTeleportIndex.Gaia},
				{MapLocation.Onrac,OverworldTeleportIndex.Onrac},
				{MapLocation.Lefein,OverworldTeleportIndex.Lefein},
				{MapLocation.ConeriaCastle1,OverworldTeleportIndex.ConeriaCastle1},
				{MapLocation.ConeriaCastle2,OverworldTeleportIndex.ConeriaCastle1},
				{MapLocation.ConeriaCastleRoom1,OverworldTeleportIndex.ConeriaCastle1},
				{MapLocation.ConeriaCastleRoom2,OverworldTeleportIndex.ConeriaCastle1},
				{MapLocation.ElflandCastle,OverworldTeleportIndex.ElflandCastle},
				{MapLocation.ElflandCastleRoom1,OverworldTeleportIndex.ElflandCastle},
				{MapLocation.NorthwestCastle,OverworldTeleportIndex.NorthwestCastle},
				{MapLocation.NorthwestCastleRoom2,OverworldTeleportIndex.NorthwestCastle},
				{MapLocation.CastleOrdeals1,OverworldTeleportIndex.CastleOrdeals1},
				{MapLocation.CastleOrdealsMaze,OverworldTeleportIndex.CastleOrdeals1},
				{MapLocation.CastleOrdealsTop,OverworldTeleportIndex.CastleOrdeals1},
				{MapLocation.TempleOfFiends1,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends1Room1,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends1Room2,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends1Room3,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends1Room4,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends2,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiends3,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsChaos,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsAir,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsEarth,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsFire,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsWater,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.TempleOfFiendsPhantom,OverworldTeleportIndex.TempleOfFiends1},
				{MapLocation.EarthCave1,OverworldTeleportIndex.EarthCave1},
				{MapLocation.EarthCave2,OverworldTeleportIndex.EarthCave1},
				{MapLocation.EarthCaveVampire,OverworldTeleportIndex.EarthCave1},
				{MapLocation.EarthCave4,OverworldTeleportIndex.EarthCave1},
				{MapLocation.EarthCaveLich,OverworldTeleportIndex.EarthCave1},
				{MapLocation.GurguVolcano1,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcano2,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcano3,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcano4,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcano5,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcano6,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.GurguVolcanoKary,OverworldTeleportIndex.GurguVolcano1},
				{MapLocation.IceCave1,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCave2,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCave3,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCave5,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCaveBackExit,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCaveFloater,OverworldTeleportIndex.IceCave1},
				{MapLocation.IceCavePitRoom,OverworldTeleportIndex.IceCave1},
				{MapLocation.SeaShrine1, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine2, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine2Room2, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine4, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine5, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine6, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine7, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrine8, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrineKraken, (OverworldTeleportIndex)35},
				{MapLocation.SeaShrineMermaids, (OverworldTeleportIndex)35},
				{MapLocation.Cardia1,OverworldTeleportIndex.Cardia1},
				{MapLocation.Cardia2,OverworldTeleportIndex.Cardia2},
				{MapLocation.BahamutCave1,OverworldTeleportIndex.BahamutCave1},
				{MapLocation.BahamutCave2,OverworldTeleportIndex.BahamutCave1},
				{MapLocation.Cardia4,OverworldTeleportIndex.Cardia4},
				{MapLocation.Cardia5,OverworldTeleportIndex.Cardia5},
				{MapLocation.Cardia6,OverworldTeleportIndex.Cardia6},
				{MapLocation.Waterfall,OverworldTeleportIndex.Waterfall},
				{MapLocation.DwarfCave,OverworldTeleportIndex.DwarfCave},
				{MapLocation.DwarfCaveRoom3,OverworldTeleportIndex.DwarfCave},
				{MapLocation.MatoyasCave,OverworldTeleportIndex.MatoyasCave},
				{MapLocation.SardasCave,OverworldTeleportIndex.SardasCave},
				{MapLocation.MarshCave1,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCave3,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCaveBottom,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCaveBottomRoom13,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCaveBottomRoom14,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCaveBottomRoom16,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MarshCaveTop,OverworldTeleportIndex.MarshCave1},
				{MapLocation.MirageTower1,OverworldTeleportIndex.MirageTower1},
				{MapLocation.MirageTower2,OverworldTeleportIndex.MirageTower1},
				{MapLocation.MirageTower3,OverworldTeleportIndex.MirageTower1},
				{MapLocation.SkyPalace1,OverworldTeleportIndex.MirageTower1},
				{MapLocation.SkyPalace2,OverworldTeleportIndex.MirageTower1},
				{MapLocation.SkyPalace3,OverworldTeleportIndex.MirageTower1},
				{MapLocation.SkyPalaceMaze,OverworldTeleportIndex.MirageTower1},
				{MapLocation.SkyPalaceTiamat,OverworldTeleportIndex.MirageTower1},
				{MapLocation.TitansTunnelEast,OverworldTeleportIndex.TitansTunnelEast},
				{MapLocation.TitansTunnelWest,OverworldTeleportIndex.TitansTunnelWest},
				{MapLocation.TitansTunnelRoom,OverworldTeleportIndex.TitansTunnelWest},
		};
	}
}
