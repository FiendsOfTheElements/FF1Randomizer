using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace FF1Lib
{
    public static partial class Items
    {
        public static readonly ReadOnlyCollection<byte> Tier1Requirements =
            new List<byte> {
            Lute, Key, EarthOrb, FireOrb, WaterOrb, AirOrb, 
            Canoe, Floater, Rod, Oxyale, Chime, Cube
        }.AsReadOnly();
        public static readonly ReadOnlyCollection<byte> Tier2Requirements =
            new List<byte> {
            Tnt, Herb, Crystal, Crown, Slab, Bottle, Ruby
        }.AsReadOnly();
        public static readonly ReadOnlyCollection<byte> NiceOptionals1 =
            new List<byte> {
            Tail, Adamant, Masamune, Xcalber, Ribbon
        }.AsReadOnly();
        public static readonly ReadOnlyCollection<byte> NiceOptionals2 =
            new List<byte> {
            ProRing, ProCape, Opal, DragonArmor, Katana, WhiteShirt, BlackShirt,
            PowerGauntlets, ZeusGauntlets, Defense
        }.AsReadOnly();
        public static readonly ReadOnlyCollection<byte> NiceOptionals3 =
            new List<byte> {
            HealHelm, AegisShield, BaneSword, ThorHammer, MageRod, WizardRod, 
            LightAxe, HealRod, Gold65000
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<byte> AllWeapons =
            new List<byte> {
            WoodenNunchucks, SmallKnife, WoodenRod, Rapier, IronHammer,
            ShortSword, HandAxe, Scimitar, IronNunchucks, LargeKnife, IronSword,
            Sabre, LongSword, GreatAxe, Falchon, SilverKnife, SilverSword,
            SilverHammer, SilverAxe, FlameSword, IceSword, DragonSword,
            GiantSword, SunSword, CoralSword, WereSword, RuneSword, PowerRod,
            LightAxe, HealRod, MageRod, Defense, WizardRod, Vorpal, CatClaw,
            ThorHammer, BaneSword, Katana, Xcalber, Masamune
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<byte> AllArmor =
            new List<byte> {
            Cloth, WoodenArmor, ChainArmor, IronArmor, SteelArmor, SilverArmor,
            FlameArmor, IceArmor, OpalArmor, DragonArmor, Copper, Silver, Gold,
            Opal, WhiteShirt, BlackShirt, WoodenShield, IronShield, SilverShield,
            FlameShield, IceShield, OpalShield, AegisShield, Buckler, ProCape, Cap,
            WoodenHelm, IronHelm, SilverHelm, OpalHelm, HealHelm, Ribbon, Gloves,
            CopperGauntlets, IronGauntlets, SilverGauntlets, ZeusGauntlets,
            PowerGauntlets, OpalGauntlets, ProRing
        }.AsReadOnly();

		public static readonly ReadOnlyCollection<byte> AllConsumables =
			new List<byte> { Tent, Cabin, House, Heal, Pure, Soft }.AsReadOnly();

        public static readonly ReadOnlyCollection<byte> AllQuestItems =
            new List<byte> {
            Lute, Crown, Crystal, Herb, Key, Tnt, Adamant, Slab, Ruby, Rod,
            Floater, Chime, Tail, Cube, Bottle, Oxyale, Canoe
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<byte> AllOrbs =
            new List<byte> {
            EarthOrb, FireOrb, WaterOrb, AirOrb
        }.AsReadOnly();

		public static readonly ReadOnlyCollection<byte> AllGoldTreasure =
			new List<byte> {
            Gold10, Gold20, Gold25, Gold30, Gold55, Gold70, Gold85, Gold110, 
            Gold135, Gold155, Gold160, Gold180, Gold240, Gold255, Gold260, 
            Gold295, Gold300, Gold315, Gold330, Gold350, Gold385, Gold400, 
            Gold450, Gold500, Gold530, Gold575, Gold620, Gold680, Gold750, 
            Gold795, Gold880, Gold1020, Gold1250, Gold1455, Gold1520, Gold1760, 
            Gold1975, Gold2000, Gold2750, Gold3400, Gold4150, Gold5000, Gold5450, 
            Gold6400, Gold6720, Gold7340, Gold7690, Gold7900, Gold8135, Gold9000, 
            Gold9300, Gold9500, Gold9900, Gold10000, Gold12350, Gold13000, 
            Gold13450, Gold14050, Gold14720, Gold15000, Gold17490, Gold18010, 
            Gold19990, Gold20000, Gold20010, Gold26000, Gold45000, Gold65000
		}.AsReadOnly();

        public static readonly Dictionary<byte, string> TextLookup = 
            new Dictionary<byte, string>
        {
            {Lute, nameof(Lute)},
            {Crown, nameof(Crown)},
            {Crystal, nameof(Crystal)}, 
            {Herb, nameof(Herb)}, 
            {Key, nameof(Key)}, 
            {Tnt, nameof(Tnt)}, 
            {Adamant, nameof(Adamant)}, 
            {Slab, nameof(Slab)}, 
            {Ruby, nameof(Ruby)}, 
            {Rod, nameof(Rod)},
            {Floater, nameof(Floater)}, 
            {Chime, nameof(Chime)}, 
            {Tail, nameof(Tail)}, 
            {Cube, nameof(Cube)}, 
            {Bottle, nameof(Bottle)}, 
            {Oxyale, nameof(Oxyale)}, 
            {Canoe, nameof(Canoe)}, 
            {Tent, nameof(Tent)}, 
            {Cabin, nameof(Cabin)}, 
            {House, nameof(House)}, 
            {Heal, nameof(Heal)}, 
            {Pure, nameof(Pure)}, 
            {Soft, nameof(Soft)},
            {WoodenNunchucks, nameof(WoodenNunchucks)}, 
            {SmallKnife, nameof(SmallKnife)}, 
            {WoodenRod, nameof(WoodenRod)}, 
            {Rapier, nameof(Rapier)}, 
            {IronHammer, nameof(IronHammer)},
            {ShortSword, nameof(ShortSword)}, 
            {HandAxe, nameof(HandAxe)}, 
            {Scimitar, nameof(Scimitar)}, 
            {IronNunchucks, nameof(IronNunchucks)}, 
            {LargeKnife, nameof(LargeKnife)}, 
            {IronSword, nameof(IronSword)},
            {Sabre, nameof(Sabre)}, 
            {LongSword, nameof(LongSword)}, 
            {GreatAxe, nameof(GreatAxe)}, 
            {Falchon, nameof(Falchon)}, 
            {SilverKnife, nameof(SilverKnife)}, 
            {SilverSword, nameof(SilverSword)},
            {SilverHammer, nameof(SilverHammer)}, 
            {SilverAxe, nameof(SilverAxe)}, 
            {FlameSword, nameof(FlameSword)}, 
            {IceSword, nameof(IceSword)}, 
            {DragonSword, nameof(DragonSword)},
            {GiantSword, nameof(GiantSword)}, 
            {SunSword, nameof(SunSword)}, 
            {CoralSword, nameof(CoralSword)}, 
            {WereSword, nameof(WereSword)}, 
            {RuneSword, nameof(RuneSword)}, 
            {PowerRod, nameof(PowerRod)},
            {LightAxe, nameof(LightAxe)}, 
            {HealRod, nameof(HealRod)}, 
            {MageRod, nameof(MageRod)}, 
            {Defense, nameof(Defense)}, 
            {WizardRod, nameof(WizardRod)}, 
            {Vorpal, nameof(Vorpal)}, 
            {CatClaw, nameof(CatClaw)},
            {ThorHammer, nameof(ThorHammer)}, 
            {BaneSword, nameof(BaneSword)}, 
            {Katana, nameof(Katana)}, 
            {Xcalber, nameof(Xcalber)}, 
            {Masamune, nameof(Masamune)},
            {Cloth, nameof(Cloth)}, 
            {WoodenArmor, nameof(WoodenArmor)}, 
            {ChainArmor, nameof(ChainArmor)}, 
            {IronArmor, nameof(IronArmor)}, 
            {SteelArmor, nameof(SteelArmor)}, 
            {SilverArmor, nameof(SilverArmor)},
            {FlameArmor, nameof(FlameArmor)}, 
            {IceArmor, nameof(IceArmor)}, 
            {OpalArmor, nameof(OpalArmor)}, 
            {DragonArmor, nameof(DragonArmor)}, 
            {Copper, nameof(Copper)}, 
            {Silver, nameof(Silver)}, 
            {Gold, nameof(Gold)},
            {Opal, nameof(Opal)}, 
            {WhiteShirt, nameof(WhiteShirt)}, 
            {BlackShirt, nameof(BlackShirt)}, 
            {WoodenShield, nameof(WoodenShield)}, 
            {IronShield, nameof(IronShield)}, 
            {SilverShield, nameof(SilverShield)}, 
            {FlameShield, nameof(FlameShield)}, 
            {IceShield, nameof(IceShield)}, 
            {OpalShield, nameof(OpalShield)}, 
            {AegisShield, nameof(AegisShield)}, 
            {Buckler, nameof(Buckler)}, 
            {ProCape, nameof(ProCape)}, 
            {Cap, nameof(Cap)},
            {WoodenHelm, nameof(WoodenHelm)}, 
            {IronHelm, nameof(IronHelm)}, 
            {SilverHelm, nameof(SilverHelm)}, 
            {OpalHelm, nameof(OpalHelm)}, 
            {HealHelm, nameof(HealHelm)}, 
            {Ribbon, nameof(Ribbon)}, 
            {Gloves, nameof(Gloves)},
            {CopperGauntlets, nameof(CopperGauntlets)}, 
            {IronGauntlets, nameof(IronGauntlets)}, 
            {SilverGauntlets, nameof(SilverGauntlets)}, 
            {ZeusGauntlets, nameof(ZeusGauntlets)},
            {PowerGauntlets, nameof(PowerGauntlets)}, 
            {OpalGauntlets, nameof(OpalGauntlets)}, 
            {ProRing, nameof(ProRing)},
            {Gold10, nameof(Gold10)}, 
            {Gold20, nameof(Gold20)}, 
            {Gold25, nameof(Gold25)}, 
            {Gold30, nameof(Gold30)}, 
            {Gold55, nameof(Gold55)}, 
            {Gold70, nameof(Gold70)}, 
            {Gold85, nameof(Gold85)}, 
            {Gold110, nameof(Gold110)},
            {Gold135, nameof(Gold135)}, 
            {Gold155, nameof(Gold155)}, 
            {Gold160, nameof(Gold160)}, 
            {Gold180, nameof(Gold180)}, 
            {Gold240, nameof(Gold240)}, 
            {Gold255, nameof(Gold255)}, 
            {Gold260, nameof(Gold260)},
            {Gold295, nameof(Gold295)}, 
            {Gold300, nameof(Gold300)}, 
            {Gold315, nameof(Gold315)}, 
            {Gold330, nameof(Gold330)}, 
            {Gold350, nameof(Gold350)}, 
            {Gold385, nameof(Gold385)}, 
            {Gold400, nameof(Gold400)},
            {Gold450, nameof(Gold450)}, 
            {Gold500, nameof(Gold500)}, 
            {Gold530, nameof(Gold530)}, 
            {Gold575, nameof(Gold575)}, 
            {Gold620, nameof(Gold620)}, 
            {Gold680, nameof(Gold680)}, 
            {Gold750, nameof(Gold750)},
            {Gold795, nameof(Gold795)}, 
            {Gold880, nameof(Gold880)}, 
            {Gold1020, nameof(Gold1020)}, 
            {Gold1250, nameof(Gold1250)}, 
            {Gold1455, nameof(Gold1455)}, 
            {Gold1520, nameof(Gold1520)}, 
            {Gold1760, nameof(Gold1760)},
            {Gold1975, nameof(Gold1975)}, 
            {Gold2000, nameof(Gold2000)}, 
            {Gold2750, nameof(Gold2750)}, 
            {Gold3400, nameof(Gold3400)}, 
            {Gold4150, nameof(Gold4150)}, 
            {Gold5000, nameof(Gold5000)}, 
            {Gold5450, nameof(Gold5450)},
            {Gold6400, nameof(Gold6400)}, 
            {Gold6720, nameof(Gold6720)}, 
            {Gold7340, nameof(Gold7340)}, 
            {Gold7690, nameof(Gold7690)}, 
            {Gold7900, nameof(Gold7900)}, 
            {Gold8135, nameof(Gold8135)}, 
            {Gold9000, nameof(Gold9000)},
            {Gold9300, nameof(Gold9300)}, 
            {Gold9500, nameof(Gold9500)}, 
            {Gold9900, nameof(Gold9900)}, 
            {Gold10000, nameof(Gold10000)}, 
            {Gold12350, nameof(Gold12350)}, 
            {Gold13000, nameof(Gold13000)},
            {Gold13450, nameof(Gold13450)}, 
            {Gold14050, nameof(Gold14050)}, 
            {Gold14720, nameof(Gold14720)}, 
            {Gold15000, nameof(Gold15000)}, 
            {Gold17490, nameof(Gold17490)}, 
            {Gold18010, nameof(Gold18010)},
            {Gold19990, nameof(Gold19990)}, 
            {Gold20000, nameof(Gold20000)}, 
            {Gold20010, nameof(Gold20010)}, 
            {Gold26000, nameof(Gold26000)}, 
            {Gold45000, nameof(Gold45000)}, 
            {Gold65000, nameof(Gold65000)}
        };
    }
}
