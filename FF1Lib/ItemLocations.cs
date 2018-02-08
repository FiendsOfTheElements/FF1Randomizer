
using System.Collections.Generic;

namespace FF1Lib
{
	public static partial class ItemLocations
	{
		public static TreasureChest Coneria1 = new TreasureChest(0x3101, nameof(Coneria1), MapLocation.ConeriaCastle, Item.IronArmor, AccessRequirement.Key);
		public static TreasureChest Coneria2 = new TreasureChest(0x3102, nameof(Coneria2), MapLocation.ConeriaCastle, Item.IronShield, AccessRequirement.Key);
		public static TreasureChest ConeriaMajor = new TreasureChest(0x3103, nameof(ConeriaMajor), MapLocation.ConeriaCastle, Item.Tnt, AccessRequirement.Key);
		public static TreasureChest Coneria4 = new TreasureChest(0x3104, nameof(Coneria4), MapLocation.ConeriaCastle, Item.IronSword, AccessRequirement.Key);
		public static TreasureChest Coneria5 = new TreasureChest(0x3105, nameof(Coneria5), MapLocation.ConeriaCastle, Item.Sabre, AccessRequirement.Key);
		public static TreasureChest Coneria6 = new TreasureChest(0x3106, nameof(Coneria6), MapLocation.ConeriaCastle, Item.SilverKnife, AccessRequirement.Key);
		public static TreasureChest ToFTopLeft1 = new TreasureChest(0x3107, nameof(ToFTopLeft1), MapLocation.TempleOfFiends, Item.Cabin);
		public static TreasureChest ToFTopLeft2 = new TreasureChest(0x3108, nameof(ToFTopLeft2), MapLocation.TempleOfFiends, Item.Heal);
		public static TreasureChest ToFBottomLeft = new TreasureChest(0x3109, nameof(ToFBottomLeft), MapLocation.TempleOfFiends, Item.Cap);
		public static TreasureChest ToFBottomRight = new TreasureChest(0x310A, nameof(ToFBottomRight), MapLocation.TempleOfFiends, Item.RuneSword, AccessRequirement.Key);
		public static TreasureChest ToFTopRight1 = new TreasureChest(0x310B, nameof(ToFTopRight1), MapLocation.TempleOfFiends, Item.WereSword, AccessRequirement.Key);
		public static TreasureChest ToFTopRight2 = new TreasureChest(0x310C, nameof(ToFTopRight2), MapLocation.TempleOfFiends, Item.Soft, AccessRequirement.Key);
		public static TreasureChest Elfland1 = new TreasureChest(0x310D, nameof(Elfland1), MapLocation.ElflandCastle, Item.SilverHammer, AccessRequirement.Key);
		public static TreasureChest Elfland2 = new TreasureChest(0x310E, nameof(Elfland2), MapLocation.ElflandCastle, Item.Gold400, AccessRequirement.Key);
		public static TreasureChest Elfland3 = new TreasureChest(0x310F, nameof(Elfland3), MapLocation.ElflandCastle, Item.Gold330, AccessRequirement.Key);
		public static TreasureChest Elfland4 = new TreasureChest(0x3110, nameof(Elfland4), MapLocation.ElflandCastle, Item.CopperGauntlets, AccessRequirement.Key);
		public static TreasureChest NorthwestCastle1 = new TreasureChest(0x3111, nameof(NorthwestCastle1), MapLocation.NorthwestCastle, Item.PowerRod, AccessRequirement.Key);
		public static TreasureChest NorthwestCastle2 = new TreasureChest(0x3112, nameof(NorthwestCastle2), MapLocation.NorthwestCastle, Item.IronGauntlets, AccessRequirement.Key);
		public static TreasureChest NorthwestCastle3 = new TreasureChest(0x3113, nameof(NorthwestCastle3), MapLocation.NorthwestCastle, Item.Falchon, AccessRequirement.Key);
		// Marsh Cave has 5 extra empty treasure chests
		public static TreasureChest MarshCave1 = new TreasureChest(0x3114, nameof(MarshCave1), MapLocation.MarshCave, Item.Gold295);
		public static TreasureChest MarshCave2 = new TreasureChest(0x3115, nameof(MarshCave2), MapLocation.MarshCave, Item.Copper);
		public static TreasureChest MarshCave3 = new TreasureChest(0x3116, nameof(MarshCave3), MapLocation.MarshCave, Item.House);
		public static TreasureChest MarshCave4 = new TreasureChest(0x3117, nameof(MarshCave4), MapLocation.MarshCave, Item.Gold385);
		public static TreasureChest MarshCave5 = new TreasureChest(0x3118, nameof(MarshCave5), MapLocation.MarshCave, Item.Gold620);
		public static TreasureChest MarshCave6 = new TreasureChest(0x3119, nameof(MarshCave6), MapLocation.MarshCave, Item.ShortSword);
		public static TreasureChest MarshCave7 = new TreasureChest(0x311A, nameof(MarshCave7), MapLocation.MarshCave, Item.Gold680);
		public static TreasureChest MarshCave8 = new TreasureChest(0x311B, nameof(MarshCave8), MapLocation.MarshCave, Item.LargeKnife);
		public static TreasureChest MarshCaveMajor = new TreasureChest(0x311C, nameof(MarshCaveMajor), MapLocation.MarshCave, Item.Crown);
		public static TreasureChest MarshCave10 = new TreasureChest(0x311D, nameof(MarshCave10), MapLocation.MarshCave, Item.IronArmor);
		public static TreasureChest MarshCave11 = new TreasureChest(0x311E, nameof(MarshCave11), MapLocation.MarshCave, Item.Silver, AccessRequirement.Key);
		public static TreasureChest MarshCave12 = new TreasureChest(0x311F, nameof(MarshCave12), MapLocation.MarshCave, Item.SilverKnife, AccessRequirement.Key);
		public static TreasureChest MarshCave13 = new TreasureChest(0x3120, nameof(MarshCave13), MapLocation.MarshCave, Item.Gold1020, AccessRequirement.Key);
		public static TreasureChest DwarfCave1 = new TreasureChest(0x3121, nameof(DwarfCave1), MapLocation.DwarfCave, Item.Gold450);
		public static TreasureChest DwarfCave2 = new TreasureChest(0x3122, nameof(DwarfCave2), MapLocation.DwarfCave, Item.Gold575);
		public static TreasureChest DwarfCave3 = new TreasureChest(0x3123, nameof(DwarfCave3), MapLocation.DwarfCave, Item.Cabin, AccessRequirement.Key);
		public static TreasureChest DwarfCave4 = new TreasureChest(0x3124, nameof(DwarfCave4), MapLocation.DwarfCave, Item.IronHelm, AccessRequirement.Key);
		public static TreasureChest DwarfCave5 = new TreasureChest(0x3125, nameof(DwarfCave5), MapLocation.DwarfCave, Item.WoodenHelm, AccessRequirement.Key);
		public static TreasureChest DwarfCave6 = new TreasureChest(0x3126, nameof(DwarfCave6), MapLocation.DwarfCave, Item.DragonSword, AccessRequirement.Key);
		public static TreasureChest DwarfCave7 = new TreasureChest(0x3127, nameof(DwarfCave7), MapLocation.DwarfCave, Item.SilverKnife, AccessRequirement.Key);
		public static TreasureChest DwarfCave8 = new TreasureChest(0x3128, nameof(DwarfCave8), MapLocation.DwarfCave, Item.SilverArmor, AccessRequirement.Key);
		public static TreasureChest DwarfCave9 = new TreasureChest(0x3129, nameof(DwarfCave9), MapLocation.DwarfCave, Item.Gold575, AccessRequirement.Key);
		public static TreasureChest DwarfCave10 = new TreasureChest(0x312A, nameof(DwarfCave10), MapLocation.DwarfCave, Item.House, AccessRequirement.Key);
		public static TreasureChest MatoyasCave1 = new TreasureChest(0x312B, nameof(MatoyasCave1), MapLocation.MatoyasCave, Item.Heal);
		public static TreasureChest MatoyasCave2 = new TreasureChest(0x312C, nameof(MatoyasCave2), MapLocation.MatoyasCave, Item.Pure);
		public static TreasureChest MatoyasCave3 = new TreasureChest(0x312D, nameof(MatoyasCave3), MapLocation.MatoyasCave, Item.Heal);
		public static TreasureChest EarthCave1 = new TreasureChest(0x312E, nameof(EarthCave1), MapLocation.EarthCave, Item.Gold880);
		public static TreasureChest EarthCave2 = new TreasureChest(0x312F, nameof(EarthCave2), MapLocation.EarthCave, Item.Heal);
		public static TreasureChest EarthCave3 = new TreasureChest(0x3130, nameof(EarthCave3), MapLocation.EarthCave, Item.Pure);
		public static TreasureChest EarthCave4 = new TreasureChest(0x3131, nameof(EarthCave4), MapLocation.EarthCave, Item.Gold795);
		public static TreasureChest EarthCave5 = new TreasureChest(0x3132, nameof(EarthCave5), MapLocation.EarthCave, Item.Gold1975);
		public static TreasureChest EarthCave6 = new TreasureChest(0x3133, nameof(EarthCave6), MapLocation.EarthCave, Item.CoralSword);
		public static TreasureChest EarthCave7 = new TreasureChest(0x3134, nameof(EarthCave7), MapLocation.EarthCave, Item.Cabin);
		public static TreasureChest EarthCave8 = new TreasureChest(0x3135, nameof(EarthCave8), MapLocation.EarthCave, Item.Gold330);
		public static TreasureChest EarthCave9 = new TreasureChest(0x3136, nameof(EarthCave9), MapLocation.EarthCave, Item.Gold5000);
		public static TreasureChest EarthCave10 = new TreasureChest(0x3137, nameof(EarthCave10), MapLocation.EarthCave, Item.WoodenShield);
		public static TreasureChest EarthCave11 = new TreasureChest(0x3138, nameof(EarthCave11), MapLocation.EarthCave, Item.Gold575);
		public static TreasureChest EarthCave12 = new TreasureChest(0x3139, nameof(EarthCave12), MapLocation.EarthCave, Item.Gold1020);
		public static TreasureChest EarthCave13 = new TreasureChest(0x313A, nameof(EarthCave13), MapLocation.EarthCave, Item.Gold3400);
		public static TreasureChest EarthCave14 = new TreasureChest(0x313B, nameof(EarthCave14), MapLocation.EarthCave, Item.Tent);
		public static TreasureChest EarthCave15 = new TreasureChest(0x313C, nameof(EarthCave15), MapLocation.EarthCave, Item.Heal);
		public static TreasureChest EarthCaveMajor = new TreasureChest(0x313D, nameof(EarthCaveMajor), MapLocation.EarthCave, Item.Ruby);
		public static TreasureChest EarthCave17 = new TreasureChest(0x313E, nameof(EarthCave17), MapLocation.EarthCave, Item.Gold1250, AccessRequirement.Rod);
		public static TreasureChest EarthCave18 = new TreasureChest(0x313F, nameof(EarthCave18), MapLocation.EarthCave, Item.SilverShield, AccessRequirement.Rod);
		public static TreasureChest EarthCave19 = new TreasureChest(0x3140, nameof(EarthCave19), MapLocation.EarthCave, Item.Cabin, AccessRequirement.Rod);
		public static TreasureChest EarthCave20 = new TreasureChest(0x3141, nameof(EarthCave20), MapLocation.EarthCave, Item.Gold5450, AccessRequirement.Rod);
		public static TreasureChest EarthCave21 = new TreasureChest(0x3142, nameof(EarthCave21), MapLocation.EarthCave, Item.Gold1520, AccessRequirement.Rod);
		public static TreasureChest EarthCave22 = new TreasureChest(0x3143, nameof(EarthCave22), MapLocation.EarthCave, Item.WoodenRod, AccessRequirement.Rod);
		public static TreasureChest EarthCave23 = new TreasureChest(0x3144, nameof(EarthCave23), MapLocation.EarthCave, Item.Gold3400, AccessRequirement.Rod);
		public static TreasureChest EarthCave24 = new TreasureChest(0x3145, nameof(EarthCave24), MapLocation.EarthCave, Item.Gold1455, AccessRequirement.Rod);
		public static TreasureChest TitansTunnel1 = new TreasureChest(0x3146, nameof(TitansTunnel1), MapLocation.TitansTunnelB, Item.SilverHelm);
		public static TreasureChest TitansTunnel2 = new TreasureChest(0x3147, nameof(TitansTunnel2), MapLocation.TitansTunnelB, Item.Gold450);
		public static TreasureChest TitansTunnel3 = new TreasureChest(0x3148, nameof(TitansTunnel3), MapLocation.TitansTunnelB, Item.Gold620);
		public static TreasureChest TitansTunnel4 = new TreasureChest(0x3149, nameof(TitansTunnel4), MapLocation.TitansTunnelB, Item.GreatAxe);
		// Volcano has 3 extra empty treasure chests
		public static TreasureChest Volcano1 = new TreasureChest(0x314A, nameof(Volcano1), MapLocation.GurguVolcano, Item.Heal);
		public static TreasureChest Volcano2 = new TreasureChest(0x314B, nameof(Volcano2), MapLocation.GurguVolcano, Item.Cabin);
		public static TreasureChest Volcano3 = new TreasureChest(0x314C, nameof(Volcano3), MapLocation.GurguVolcano, Item.Gold1975);
		public static TreasureChest Volcano4 = new TreasureChest(0x314D, nameof(Volcano4), MapLocation.GurguVolcano, Item.Pure);
		public static TreasureChest Volcano5 = new TreasureChest(0x314E, nameof(Volcano5), MapLocation.GurguVolcano, Item.Heal);
		public static TreasureChest Volcano6 = new TreasureChest(0x314F, nameof(Volcano6), MapLocation.GurguVolcano, Item.Gold1455);
		public static TreasureChest Volcano7 = new TreasureChest(0x3150, nameof(Volcano7), MapLocation.GurguVolcano, Item.SilverShield);
		public static TreasureChest Volcano8 = new TreasureChest(0x3151, nameof(Volcano8), MapLocation.GurguVolcano, Item.Gold1520);
		public static TreasureChest Volcano9 = new TreasureChest(0x3152, nameof(Volcano9), MapLocation.GurguVolcano, Item.SilverHelm);
		public static TreasureChest Volcano10 = new TreasureChest(0x3153, nameof(Volcano10), MapLocation.GurguVolcano, Item.SilverGauntlets);
		public static TreasureChest Volcano11 = new TreasureChest(0x3154, nameof(Volcano11), MapLocation.GurguVolcano, Item.Gold1760);
		public static TreasureChest Volcano12 = new TreasureChest(0x3155, nameof(Volcano12), MapLocation.GurguVolcano, Item.SilverAxe);
		public static TreasureChest Volcano13 = new TreasureChest(0x3156, nameof(Volcano13), MapLocation.GurguVolcano, Item.Gold795);
		public static TreasureChest Volcano14 = new TreasureChest(0x3157, nameof(Volcano14), MapLocation.GurguVolcano, Item.Gold750);
		public static TreasureChest Volcano15 = new TreasureChest(0x3158, nameof(Volcano15), MapLocation.GurguVolcano, Item.GiantSword);
		public static TreasureChest Volcano16 = new TreasureChest(0x3159, nameof(Volcano16), MapLocation.GurguVolcano, Item.Gold4150);
		public static TreasureChest Volcano17 = new TreasureChest(0x315A, nameof(Volcano17), MapLocation.GurguVolcano, Item.Gold1520);
		public static TreasureChest Volcano18 = new TreasureChest(0x315B, nameof(Volcano18), MapLocation.GurguVolcano, Item.SilverHelm);
		public static TreasureChest Volcano19 = new TreasureChest(0x315C, nameof(Volcano19), MapLocation.GurguVolcano, Item.Soft);
		public static TreasureChest Volcano20 = new TreasureChest(0x315D, nameof(Volcano20), MapLocation.GurguVolcano, Item.Gold2750);
		public static TreasureChest Volcano21 = new TreasureChest(0x315E, nameof(Volcano21), MapLocation.GurguVolcano, Item.Gold1760);
		public static TreasureChest Volcano22 = new TreasureChest(0x315F, nameof(Volcano22), MapLocation.GurguVolcano, Item.WoodenRod);
		public static TreasureChest Volcano23 = new TreasureChest(0x3160, nameof(Volcano23), MapLocation.GurguVolcano, Item.Gold1250);
		public static TreasureChest Volcano24 = new TreasureChest(0x3161, nameof(Volcano24), MapLocation.GurguVolcano, Item.Gold10);
		public static TreasureChest Volcano25 = new TreasureChest(0x3162, nameof(Volcano25), MapLocation.GurguVolcano, Item.Gold155);
		public static TreasureChest Volcano26 = new TreasureChest(0x3163, nameof(Volcano26), MapLocation.GurguVolcano, Item.House);
		public static TreasureChest Volcano27 = new TreasureChest(0x3164, nameof(Volcano27), MapLocation.GurguVolcano, Item.Gold2000);
		public static TreasureChest Volcano28 = new TreasureChest(0x3165, nameof(Volcano28), MapLocation.GurguVolcano, Item.IceSword);
		public static TreasureChest Volcano29 = new TreasureChest(0x3166, nameof(Volcano29), MapLocation.GurguVolcano, Item.Gold880);
		public static TreasureChest Volcano30 = new TreasureChest(0x3167, nameof(Volcano30), MapLocation.GurguVolcano, Item.Pure);
		public static TreasureChest Volcano31 = new TreasureChest(0x3168, nameof(Volcano31), MapLocation.GurguVolcano, Item.FlameShield);
		public static TreasureChest Volcano32 = new TreasureChest(0x3169, nameof(Volcano32), MapLocation.GurguVolcano, Item.Gold7340);
		public static TreasureChest VolcanoMajor = new TreasureChest(0x316A, nameof(VolcanoMajor), MapLocation.GurguVolcano, Item.FlameArmor);
		public static TreasureChest IceCave1 = new TreasureChest(0x316B, nameof(IceCave1), MapLocation.IceCave, Item.Heal);
		public static TreasureChest IceCave2 = new TreasureChest(0x316C, nameof(IceCave2), MapLocation.IceCave, Item.Gold10000);
		public static TreasureChest IceCave3 = new TreasureChest(0x316D, nameof(IceCave3), MapLocation.IceCave, Item.Gold9500);
		public static TreasureChest IceCave4 = new TreasureChest(0x316E, nameof(IceCave4), MapLocation.IceCave, Item.Tent);
		public static TreasureChest IceCave5 = new TreasureChest(0x316F, nameof(IceCave5), MapLocation.IceCave, Item.IceShield);
		public static TreasureChest IceCave6 = new TreasureChest(0x3170, nameof(IceCave6), MapLocation.IceCave, Item.Cloth);
		public static TreasureChest IceCave7 = new TreasureChest(0x3171, nameof(IceCave7), MapLocation.IceCave, Item.FlameSword);
		public static TreasureChest IceCaveMajor = new TreasureChest(0x3172, nameof(IceCaveMajor), MapLocation.IceCave, Item.Floater);
		public static TreasureChest IceCave9 = new TreasureChest(0x3173, nameof(IceCave9), MapLocation.IceCave, Item.Gold7900);
		public static TreasureChest IceCave10 = new TreasureChest(0x3174, nameof(IceCave10), MapLocation.IceCave, Item.Gold5450);
		public static TreasureChest IceCave11 = new TreasureChest(0x3175, nameof(IceCave11), MapLocation.IceCave, Item.Gold9900);
		public static TreasureChest IceCave12 = new TreasureChest(0x3176, nameof(IceCave12), MapLocation.IceCave, Item.Gold5000);
		public static TreasureChest IceCave13 = new TreasureChest(0x3177, nameof(IceCave13), MapLocation.IceCave, Item.Gold180);
		public static TreasureChest IceCave14 = new TreasureChest(0x3178, nameof(IceCave14), MapLocation.IceCave, Item.Gold12350);
		public static TreasureChest IceCave15 = new TreasureChest(0x3179, nameof(IceCave15), MapLocation.IceCave, Item.SilverGauntlets);
		public static TreasureChest IceCave16 = new TreasureChest(0x317A, nameof(IceCave16), MapLocation.IceCave, Item.IceArmor);
		// Ordeals has 1 extra empty treasure chest
		public static TreasureChest Ordeals1 = new TreasureChest(0x317B, nameof(Ordeals1), MapLocation.CastleOrdeals, Item.ZeusGauntlets, AccessRequirement.Crown);
		public static TreasureChest Ordeals2 = new TreasureChest(0x317C, nameof(Ordeals2), MapLocation.CastleOrdeals, Item.House, AccessRequirement.Crown);
		public static TreasureChest Ordeals3 = new TreasureChest(0x317D, nameof(Ordeals3), MapLocation.CastleOrdeals, Item.Gold1455, AccessRequirement.Crown);
		public static TreasureChest Ordeals4 = new TreasureChest(0x317E, nameof(Ordeals4), MapLocation.CastleOrdeals, Item.Gold7340, AccessRequirement.Crown);
		public static TreasureChest Ordeals5 = new TreasureChest(0x317F, nameof(Ordeals5), MapLocation.CastleOrdeals, Item.Gold, AccessRequirement.Crown);
		public static TreasureChest Ordeals6 = new TreasureChest(0x3180, nameof(Ordeals6), MapLocation.CastleOrdeals, Item.IceSword, AccessRequirement.Crown);
		public static TreasureChest Ordeals7 = new TreasureChest(0x3181, nameof(Ordeals7), MapLocation.CastleOrdeals, Item.IronGauntlets, AccessRequirement.Crown);
		public static TreasureChest Ordeals8 = new TreasureChest(0x3182, nameof(Ordeals8), MapLocation.CastleOrdeals, Item.HealRod, AccessRequirement.Crown);
		public static TreasureChest OrdealsMajor = new TreasureChest(0x3183, nameof(OrdealsMajor), MapLocation.CastleOrdeals, Item.Tail, AccessRequirement.Crown);
		public static TreasureChest Cardia1 = new TreasureChest(0x3184, nameof(Cardia1), MapLocation.Cardia1, Item.Gold1455);
		public static TreasureChest Cardia2 = new TreasureChest(0x3185, nameof(Cardia2), MapLocation.Cardia1, Item.Gold2000);
		public static TreasureChest Cardia3 = new TreasureChest(0x3186, nameof(Cardia3), MapLocation.Cardia1, Item.Gold2750);
		public static TreasureChest Cardia4 = new TreasureChest(0x3187, nameof(Cardia4), MapLocation.Cardia1, Item.Gold2750);
		public static TreasureChest Cardia5 = new TreasureChest(0x3188, nameof(Cardia5), MapLocation.Cardia1, Item.Gold1520);
		public static TreasureChest Cardia6 = new TreasureChest(0x3189, nameof(Cardia6), MapLocation.Cardia1, Item.Gold10);
		public static TreasureChest Cardia7 = new TreasureChest(0x318A, nameof(Cardia7), MapLocation.Cardia1, Item.Gold500);
		public static TreasureChest Cardia8 = new TreasureChest(0x318B, nameof(Cardia8), MapLocation.Cardia1, Item.House);
		public static TreasureChest Cardia9 = new TreasureChest(0x318C, nameof(Cardia9), MapLocation.Cardia1, Item.Gold575);
		public static TreasureChest Cardia10 = new TreasureChest(0x318D, nameof(Cardia10), MapLocation.Cardia1, Item.Soft);
		public static TreasureChest Cardia11 = new TreasureChest(0x318E, nameof(Cardia11), MapLocation.Cardia1, Item.Cabin);
		public static TreasureChest Cardia12 = new TreasureChest(0x318F, nameof(Cardia12), MapLocation.Cardia1, Item.Gold9500);
		public static TreasureChest Cardia13 = new TreasureChest(0x3190, nameof(Cardia13), MapLocation.Cardia1, Item.Gold160);
		public static TreasureChest Unused2 = new TreasureChest(0x3191, nameof(Unused2), 0, Item.Gold530, isUnused: true);
		public static TreasureChest Unused3 = new TreasureChest(0x3192, nameof(Unused3), 0, Item.SmallKnife, isUnused: true);
		public static TreasureChest Unused4 = new TreasureChest(0x3193, nameof(Unused4), 0, Item.Cap, isUnused: true);
		public static TreasureChest Unused5 = new TreasureChest(0x3194, nameof(Unused5), 0, Item.ZeusGauntlets, isUnused: true);
		public static TreasureChest SeaShrine1 = new TreasureChest(0x3195, nameof(SeaShrine1), MapLocation.Onrac, Item.Ribbon, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine2 = new TreasureChest(0x3196, nameof(SeaShrine2), MapLocation.Onrac, Item.Gold9900, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine3 = new TreasureChest(0x3197, nameof(SeaShrine3), MapLocation.Onrac, Item.Gold7340, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine4 = new TreasureChest(0x3198, nameof(SeaShrine4), MapLocation.Onrac, Item.Gold2750, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine5 = new TreasureChest(0x3199, nameof(SeaShrine5), MapLocation.Onrac, Item.Gold7690, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine6 = new TreasureChest(0x319A, nameof(SeaShrine6), MapLocation.Onrac, Item.Gold8135, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine7 = new TreasureChest(0x319B, nameof(SeaShrine7), MapLocation.Onrac, Item.Gold5450, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine8 = new TreasureChest(0x319C, nameof(SeaShrine8), MapLocation.Onrac, Item.Gold385, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine9 = new TreasureChest(0x319D, nameof(SeaShrine9), MapLocation.Onrac, Item.PowerGauntlets, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine10 = new TreasureChest(0x319E, nameof(SeaShrine10), MapLocation.Onrac, Item.LightAxe, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine11 = new TreasureChest(0x319F, nameof(SeaShrine11), MapLocation.Onrac, Item.Gold9900, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine12 = new TreasureChest(0x31A0, nameof(SeaShrine12), MapLocation.Onrac, Item.Gold2000, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine13 = new TreasureChest(0x31A1, nameof(SeaShrine13), MapLocation.Onrac, Item.Gold450, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine14 = new TreasureChest(0x31A2, nameof(SeaShrine14), MapLocation.Onrac, Item.Gold110, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine15 = new TreasureChest(0x31A3, nameof(SeaShrine15), MapLocation.Onrac, Item.LightAxe, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine16 = new TreasureChest(0x31A4, nameof(SeaShrine16), MapLocation.Onrac, Item.OpalArmor, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrineLocked = new TreasureChest(0x31A5, nameof(SeaShrineLocked), MapLocation.Onrac, Item.Gold20, AccessRequirement.Key | AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine18 = new TreasureChest(0x31A6, nameof(SeaShrine18), MapLocation.Onrac, Item.MageRod, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine19 = new TreasureChest(0x31A7, nameof(SeaShrine19), MapLocation.Onrac, Item.Gold12350, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine20 = new TreasureChest(0x31A8, nameof(SeaShrine20), MapLocation.Onrac, Item.Gold9000, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine21 = new TreasureChest(0x31A9, nameof(SeaShrine21), MapLocation.Onrac, Item.Gold1760, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine22 = new TreasureChest(0x31AA, nameof(SeaShrine22), MapLocation.Onrac, Item.Opal, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine23 = new TreasureChest(0x31AB, nameof(SeaShrine23), MapLocation.Onrac, Item.Gold2750, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine24 = new TreasureChest(0x31AC, nameof(SeaShrine24), MapLocation.Onrac, Item.Gold10000, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine25 = new TreasureChest(0x31AD, nameof(SeaShrine25), MapLocation.Onrac, Item.Gold10, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine26 = new TreasureChest(0x31AE, nameof(SeaShrine26), MapLocation.Onrac, Item.Gold4150, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine27 = new TreasureChest(0x31AF, nameof(SeaShrine27), MapLocation.Onrac, Item.Gold5000, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine28 = new TreasureChest(0x31B0, nameof(SeaShrine28), MapLocation.Onrac, Item.Pure, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine29 = new TreasureChest(0x31B1, nameof(SeaShrine29), MapLocation.Onrac, Item.OpalShield, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine30 = new TreasureChest(0x31B2, nameof(SeaShrine30), MapLocation.Onrac, Item.OpalHelm, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrine31 = new TreasureChest(0x31B3, nameof(SeaShrine31), MapLocation.Onrac, Item.OpalGauntlets, AccessRequirement.Oxyale);
		public static TreasureChest SeaShrineMajor = new TreasureChest(0x31B4, nameof(SeaShrineMajor), MapLocation.Onrac, Item.Slab, AccessRequirement.Oxyale);
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
		public static TreasureChest MirageTower1 = new TreasureChest(0x31C4, nameof(MirageTower1), MapLocation.MirageTower, Item.AegisShield);
		public static TreasureChest MirageTower2 = new TreasureChest(0x31C5, nameof(MirageTower2), MapLocation.MirageTower, Item.Gold2750);
		public static TreasureChest MirageTower3 = new TreasureChest(0x31C6, nameof(MirageTower3), MapLocation.MirageTower, Item.Gold3400);
		public static TreasureChest MirageTower4 = new TreasureChest(0x31C7, nameof(MirageTower4), MapLocation.MirageTower, Item.Gold18010);
		public static TreasureChest MirageTower5 = new TreasureChest(0x31C8, nameof(MirageTower5), MapLocation.MirageTower, Item.Cabin);
		public static TreasureChest MirageTower6 = new TreasureChest(0x31C9, nameof(MirageTower6), MapLocation.MirageTower, Item.HealHelm);
		public static TreasureChest MirageTower7 = new TreasureChest(0x31CA, nameof(MirageTower7), MapLocation.MirageTower, Item.Gold880);
		public static TreasureChest MirageTower8 = new TreasureChest(0x31CB, nameof(MirageTower8), MapLocation.MirageTower, Item.Vorpal);
		public static TreasureChest MirageTower9 = new TreasureChest(0x31CC, nameof(MirageTower9), MapLocation.MirageTower, Item.House);
		public static TreasureChest MirageTower10 = new TreasureChest(0x31CD, nameof(MirageTower10), MapLocation.MirageTower, Item.Gold7690);
		public static TreasureChest MirageTower11 = new TreasureChest(0x31CE, nameof(MirageTower11), MapLocation.MirageTower, Item.SunSword);
		public static TreasureChest MirageTower12 = new TreasureChest(0x31CF, nameof(MirageTower12), MapLocation.MirageTower, Item.Gold10000);
		public static TreasureChest MirageTower13 = new TreasureChest(0x31D0, nameof(MirageTower13), MapLocation.MirageTower, Item.DragonArmor);
		public static TreasureChest MirageTower14 = new TreasureChest(0x31D1, nameof(MirageTower14), MapLocation.MirageTower, Item.Gold8135);
		public static TreasureChest MirageTower15 = new TreasureChest(0x31D2, nameof(MirageTower15), MapLocation.MirageTower, Item.Gold7900);
		public static TreasureChest MirageTower16 = new TreasureChest(0x31D3, nameof(MirageTower16), MapLocation.MirageTower, Item.ThorHammer);
		public static TreasureChest MirageTower17 = new TreasureChest(0x31D4, nameof(MirageTower17), MapLocation.MirageTower, Item.Gold12350);
		public static TreasureChest MirageTower18 = new TreasureChest(0x31D5, nameof(MirageTower18), MapLocation.MirageTower, Item.Gold13000);
		public static TreasureChest SkyPalace1 = new TreasureChest(0x31D6, nameof(SkyPalace1), MapLocation.MirageTower, Item.Gold9900, AccessRequirement.Cube);
		public static TreasureChest SkyPalace2 = new TreasureChest(0x31D7, nameof(SkyPalace2), MapLocation.MirageTower, Item.Heal, AccessRequirement.Cube);
		public static TreasureChest SkyPalace3 = new TreasureChest(0x31D8, nameof(SkyPalace3), MapLocation.MirageTower, Item.Gold4150, AccessRequirement.Cube);
		public static TreasureChest SkyPalace4 = new TreasureChest(0x31D9, nameof(SkyPalace4), MapLocation.MirageTower, Item.Gold7900, AccessRequirement.Cube);
		public static TreasureChest SkyPalace5 = new TreasureChest(0x31DA, nameof(SkyPalace5), MapLocation.MirageTower, Item.Gold5000, AccessRequirement.Cube);
		public static TreasureChest SkyPalace6 = new TreasureChest(0x31DB, nameof(SkyPalace6), MapLocation.MirageTower, Item.ProRing, AccessRequirement.Cube);
		public static TreasureChest SkyPalace7 = new TreasureChest(0x31DC, nameof(SkyPalace7), MapLocation.MirageTower, Item.Gold6720, AccessRequirement.Cube);
		public static TreasureChest SkyPalace8 = new TreasureChest(0x31DD, nameof(SkyPalace8), MapLocation.MirageTower, Item.HealHelm, AccessRequirement.Cube);
		public static TreasureChest SkyPalace9 = new TreasureChest(0x31DE, nameof(SkyPalace9), MapLocation.MirageTower, Item.Gold180, AccessRequirement.Cube);
		public static TreasureChest SkyPalace10 = new TreasureChest(0x31DF, nameof(SkyPalace10), MapLocation.MirageTower, Item.BaneSword, AccessRequirement.Cube);
		public static TreasureChest SkyPalace11 = new TreasureChest(0x31E0, nameof(SkyPalace11), MapLocation.MirageTower, Item.WhiteShirt, AccessRequirement.Cube);
		public static TreasureChest SkyPalace12 = new TreasureChest(0x31E1, nameof(SkyPalace12), MapLocation.MirageTower, Item.BlackShirt, AccessRequirement.Cube);
		public static TreasureChest SkyPalace13 = new TreasureChest(0x31E2, nameof(SkyPalace13), MapLocation.MirageTower, Item.Ribbon, AccessRequirement.Cube);
		public static TreasureChest SkyPalace14 = new TreasureChest(0x31E3, nameof(SkyPalace14), MapLocation.MirageTower, Item.OpalGauntlets, AccessRequirement.Cube);
		public static TreasureChest SkyPalace15 = new TreasureChest(0x31E4, nameof(SkyPalace15), MapLocation.MirageTower, Item.OpalShield, AccessRequirement.Cube);
		public static TreasureChest SkyPalace16 = new TreasureChest(0x31E5, nameof(SkyPalace16), MapLocation.MirageTower, Item.SilverHelm, AccessRequirement.Cube);
		public static TreasureChest SkyPalace17 = new TreasureChest(0x31E6, nameof(SkyPalace17), MapLocation.MirageTower, Item.House, AccessRequirement.Cube);
		public static TreasureChest SkyPalace18 = new TreasureChest(0x31E7, nameof(SkyPalace18), MapLocation.MirageTower, Item.Gold880, AccessRequirement.Cube);
		public static TreasureChest SkyPalace19 = new TreasureChest(0x31E8, nameof(SkyPalace19), MapLocation.MirageTower, Item.Gold13000, AccessRequirement.Cube);
		public static TreasureChest SkyPalaceMajor = new TreasureChest(0x31E9, nameof(SkyPalaceMajor), MapLocation.MirageTower, Item.Adamant, AccessRequirement.Cube);
		public static TreasureChest SkyPalace21 = new TreasureChest(0x31EA, nameof(SkyPalace21), MapLocation.MirageTower, Item.Gold4150, AccessRequirement.Cube);
		public static TreasureChest SkyPalace22 = new TreasureChest(0x31EB, nameof(SkyPalace22), MapLocation.MirageTower, Item.Soft, AccessRequirement.Cube);
		public static TreasureChest SkyPalace23 = new TreasureChest(0x31EC, nameof(SkyPalace23), MapLocation.MirageTower, Item.Gold3400, AccessRequirement.Cube);
		public static TreasureChest SkyPalace24 = new TreasureChest(0x31ED, nameof(SkyPalace24), MapLocation.MirageTower, Item.Katana, AccessRequirement.Cube);
		public static TreasureChest SkyPalace25 = new TreasureChest(0x31EE, nameof(SkyPalace25), MapLocation.MirageTower, Item.ProCape, AccessRequirement.Cube);
		public static TreasureChest SkyPalace26 = new TreasureChest(0x31EF, nameof(SkyPalace26), MapLocation.MirageTower, Item.Cloth, AccessRequirement.Cube);
		public static TreasureChest SkyPalace27 = new TreasureChest(0x31F0, nameof(SkyPalace27), MapLocation.MirageTower, Item.Gold9500, AccessRequirement.Cube);
		public static TreasureChest SkyPalace28 = new TreasureChest(0x31F1, nameof(SkyPalace28), MapLocation.MirageTower, Item.Soft, AccessRequirement.Cube);
		public static TreasureChest SkyPalace29 = new TreasureChest(0x31F2, nameof(SkyPalace29), MapLocation.MirageTower, Item.Gold6400, AccessRequirement.Cube);
		public static TreasureChest SkyPalace30 = new TreasureChest(0x31F3, nameof(SkyPalace30), MapLocation.MirageTower, Item.Gold8135, AccessRequirement.Cube);
		public static TreasureChest SkyPalace31 = new TreasureChest(0x31F4, nameof(SkyPalace31), MapLocation.MirageTower, Item.Gold9000, AccessRequirement.Cube);
		public static TreasureChest SkyPalace32 = new TreasureChest(0x31F5, nameof(SkyPalace32), MapLocation.MirageTower, Item.Heal, AccessRequirement.Cube);
		public static TreasureChest SkyPalace33 = new TreasureChest(0x31F6, nameof(SkyPalace33), MapLocation.MirageTower, Item.ProRing, AccessRequirement.Cube);
		public static TreasureChest SkyPalace34 = new TreasureChest(0x31F7, nameof(SkyPalace34), MapLocation.MirageTower, Item.Gold5450, AccessRequirement.Cube);
		public static TreasureChest ToFRMasmune = new TreasureChest(0x31F8, nameof(ToFRMasmune), MapLocation.TempleOfFiends, Item.Masamune, AccessRequirement.Key | AccessRequirement.BlackOrb | AccessRequirement.Lute);
		public static TreasureChest ToFRevisited2 = new TreasureChest(0x31F9, nameof(ToFRevisited2), MapLocation.TempleOfFiends, Item.Gold26000, AccessRequirement.Key | AccessRequirement.BlackOrb | AccessRequirement.Lute);
		public static TreasureChest ToFRevisited3 = new TreasureChest(0x31FA, nameof(ToFRevisited3), MapLocation.TempleOfFiends, Item.Katana, AccessRequirement.Key | AccessRequirement.BlackOrb | AccessRequirement.Lute);
		public static TreasureChest ToFRevisited4 = new TreasureChest(0x31FB, nameof(ToFRevisited4), MapLocation.TempleOfFiends, Item.ProRing, AccessRequirement.Key | AccessRequirement.BlackOrb | AccessRequirement.Lute);
		public static TreasureChest ToFRevisited5 = new TreasureChest(0x31FC, nameof(ToFRevisited5), MapLocation.TempleOfFiends, Item.ProCape, AccessRequirement.Key | AccessRequirement.BlackOrb | AccessRequirement.Lute);
		public static TreasureChest ToFRevisited6 = new TreasureChest(0x31FD, nameof(ToFRevisited6), MapLocation.TempleOfFiends, Item.Gold45000, AccessRequirement.BlackOrb);
		public static TreasureChest ToFRevisited7 = new TreasureChest(0x31FE, nameof(ToFRevisited7), MapLocation.TempleOfFiends, Item.Gold65000, AccessRequirement.BlackOrb);
		public static TreasureChest Unused15 = new TreasureChest(0x31FF, nameof(Unused15), 0, 0, isUnused: true);

		public static MapObject KingConeria = new MapObject(ObjectId.King, MapLocation.ConeriaCastle, Item.Bridge);
		public static MapObject Princess = new MapObject(ObjectId.Princess2, MapLocation.ConeriaCastle, Item.Lute, requiredSecondLocation: MapLocation.TempleOfFiends);
		public static MapObject Matoya = new MapObject(ObjectId.Matoya, MapLocation.MatoyasCave, Item.Herb, AccessRequirement.Crystal, requiredItemTrade: Item.Crystal);
		public static MapObject Bikke = new MapObject(ObjectId.Bikke, MapLocation.Pravoka, Item.Ship, useVanillaRoutineAddress: true);
		// Assumption is made that if you have access to the Elf Prince you also have access to the Elf Doc
		public static MapObject ElfPrince = new MapObject(ObjectId.ElfPrince, MapLocation.ElflandCastle, Item.Key, AccessRequirement.Herb, ObjectId.ElfDoc);
		public static MapObject Astos = new MapObject(ObjectId.Astos, MapLocation.NorthwestCastle, Item.Crystal, AccessRequirement.Crown, useVanillaRoutineAddress: true);
		public static MapObject Sarda = new MapObject(ObjectId.Sarda, MapLocation.SardasCave, Item.Rod, requiredGameEventFlag: ObjectId.Vampire, requiredSecondLocation: MapLocation.EarthCave);
		public static MapObject CanoeSage = new MapObject(ObjectId.CanoeSage, MapLocation.CresentLake, Item.Canoe, requiredItemTrade: Item.EarthOrb);
		public static MapObject CubeBot = new MapObject(ObjectId.CubeBot, MapLocation.Waterfall, Item.Cube);
		public static MapObject Fairy = new MapObject(ObjectId.Fairy, MapLocation.Gaia, Item.Oxyale, AccessRequirement.Bottle);
		// Assumption is made that if you have the slab and access to Lefein then you also have access to Unne
		public static MapObject Lefein = new MapObject(ObjectId.Lefein, MapLocation.Lefein, Item.Chime, AccessRequirement.Slab, ObjectId.Unne, requiredSecondLocation: MapLocation.Melmond);
		public static MapObject Nerrick = new MapObject(ObjectId.Nerrick, MapLocation.DwarfCave, Item.Canal, AccessRequirement.Tnt, requiredItemTrade: Item.Tnt);
		public static MapObject Smith = new MapObject(ObjectId.Smith, MapLocation.DwarfCave, Item.Xcalber, AccessRequirement.Adamant, requiredItemTrade: Item.Adamant);

		public static ItemShopSlot CaravanItemShop1 =
			new ItemShopSlot(0x38461, nameof(CaravanItemShop1), MapLocation.Caravan, Item.Bottle);

		public static StaticItemLocation LichReward =
			new StaticItemLocation(nameof(LichReward), MapLocation.EarthCave, Item.EarthOrb, AccessRequirement.Rod);
		public static StaticItemLocation KaryReward =
			new StaticItemLocation(nameof(KaryReward), MapLocation.GurguVolcano, Item.FireOrb);
		public static StaticItemLocation KrakenReward =
			new StaticItemLocation(nameof(KrakenReward), MapLocation.Onrac, Item.WaterOrb, AccessRequirement.Oxyale);
		public static StaticItemLocation TiamatReward =
			new StaticItemLocation(nameof(TiamatReward), MapLocation.MirageTower, Item.AirOrb, AccessRequirement.Cube);
		public static StaticItemLocation ChaosReward =
			new StaticItemLocation(nameof(ChaosReward), MapLocation.TempleOfFiends, Item.None, AccessRequirement.Key | AccessRequirement.BlackOrb | AccessRequirement.Lute);

		private const MapChange AirshipAndCanoe = MapChange.Airship | MapChange.Canoe;
		private const MapChange CanalAndShip = MapChange.Canal | MapChange.Ship;
		private const MapChange ShipAndCanoe = MapChange.Canoe | MapChange.Ship;
		public static Dictionary<MapLocation, List<MapChange>> MapLocationRequirements =
			new Dictionary<MapLocation, List<MapChange>>
		{
			{MapLocation.StartingLocation, new List<MapChange>{ MapChange.None }},
			{MapLocation.ConeriaTown, new List<MapChange>{ MapChange.None }},
			{MapLocation.ConeriaCastle, new List<MapChange>{ MapChange.None }},
			{MapLocation.TempleOfFiends, new List<MapChange>{ MapChange.None }},
			{MapLocation.MatoyasCave, new List<MapChange>{ MapChange.Bridge, MapChange.Ship, MapChange.Airship }},
			{MapLocation.Pravoka, new List<MapChange>{ MapChange.Bridge, MapChange.Ship, MapChange.Airship }},
			{MapLocation.DwarfCave, new List<MapChange>{ MapChange.Ship, MapChange.Airship }},
			{MapLocation.ElflandTown, new List<MapChange>{ MapChange.Ship, MapChange.Airship }},
			{MapLocation.ElflandCastle, new List<MapChange>{ MapChange.Ship, MapChange.Airship }},
			{MapLocation.NorthwestCastle, new List<MapChange>{ MapChange.Ship, MapChange.Airship }},
			{MapLocation.MarshCave, new List<MapChange>{ MapChange.Ship, MapChange.Airship }},
			{MapLocation.Melmond, new List<MapChange>{ CanalAndShip, MapChange.Airship }},
			{MapLocation.EarthCave, new List<MapChange>{ CanalAndShip, MapChange.Airship }},
			{MapLocation.TitansTunnelA, new List<MapChange>{ CanalAndShip, MapChange.Airship }},
			{MapLocation.TitansTunnelB, new List<MapChange>{ CanalAndShip | MapChange.TitanFed, MapChange.Airship }},
			{MapLocation.SardasCave, new List<MapChange>{ CanalAndShip | MapChange.TitanFed, MapChange.Airship }},
			{MapLocation.CresentLake, new List<MapChange>{ CanalAndShip, ShipAndCanoe, MapChange.Airship }},
			{MapLocation.GurguVolcano, new List<MapChange>{ ShipAndCanoe, MapChange.Airship }},
			{MapLocation.IceCave, new List<MapChange>{ MapChange.Bridge | MapChange.Canoe, MapChange.Ship | MapChange.Canoe, MapChange.Airship }},
			{MapLocation.CastleOrdeals, new List<MapChange>{ CanalAndShip | MapChange.Canoe, AirshipAndCanoe }},
			{MapLocation.Cardia1, new List<MapChange>{ MapChange.Airship }},
			{MapLocation.Cardia2, new List<MapChange>{ MapChange.Airship }},
			{MapLocation.Cardia3, new List<MapChange>{ MapChange.Airship }},
			{MapLocation.Cardia4, new List<MapChange>{ MapChange.Airship }},
			{MapLocation.Cardia5, new List<MapChange>{ MapChange.Airship }},
			{MapLocation.Cardia6, new List<MapChange>{ MapChange.Airship }},
      // Since caravan item can be placed in any item shop later in the logic, it could end up at Onrac
      {MapLocation.Caravan, new List<MapChange>{ AirshipAndCanoe }},
			{MapLocation.Gaia, new List<MapChange>{ MapChange.Airship }},
			{MapLocation.Onrac, new List<MapChange>{ AirshipAndCanoe }},
			{MapLocation.Waterfall, new List<MapChange>{ AirshipAndCanoe }},
			{MapLocation.Lefein, new List<MapChange>{ MapChange.Airship }},
			{MapLocation.MirageTower, new List<MapChange>{ MapChange.Chime | MapChange.Airship }},
			{MapLocation.BridgeLocation, new List<MapChange>{ MapChange.None }},
			{MapLocation.ShipLocation, new List<MapChange>{ MapChange.Bridge }},
			{MapLocation.CanalLocation, new List<MapChange>{ MapChange.Ship }},
			{MapLocation.AirshipLocation, new List<MapChange>{ ShipAndCanoe }}
		};
	}
}