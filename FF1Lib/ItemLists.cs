using System.Collections.Generic;
using System.Linq;
namespace FF1Lib
{
    public static class ItemLists
    {
        public static readonly IReadOnlyCollection<Item> Tier1Requirements =
            new List<Item> {
            Item.Lute, Item.Key, Item.EarthOrb, Item.FireOrb, Item.WaterOrb, Item.AirOrb, 
            Item.Canoe, Item.Floater, Item.Rod, Item.Oxyale, Item.Chime, Item.Cube
        };
        public static readonly IReadOnlyCollection<Item> Tier2Requirements =
            new List<Item> {
            Item.Tnt, Item.Herb, Item.Crystal, Item.Crown, Item.Slab, Item.Bottle, Item.Ruby
        };
        public static readonly IReadOnlyCollection<Item> NiceOptionals1 =
            new List<Item> {
            Item.Tail, Item.Adamant, Item.Masamune, Item.Xcalber, Item.Ribbon
        };
        public static readonly IReadOnlyCollection<Item> NiceOptionals2 =
            new List<Item> {
            Item.ProRing, Item.ProCape, Item.Opal, Item.DragonArmor, Item.Katana, 
            Item.WhiteShirt, Item.BlackShirt, Item.PowerGauntlets, Item.ZeusGauntlets, 
            Item.Defense
        };
        public static readonly IReadOnlyCollection<Item> NiceOptionals3 =
            new List<Item> {
            Item.HealHelm, Item.AegisShield, Item.BaneSword, Item.ThorHammer, 
            Item.MageRod, Item.WizardRod, Item.LightAxe, Item.HealRod, Item.Gold65000
        };

        public static readonly IReadOnlyCollection<Item> AllWeapons =
            new List<Item> {
            Item.WoodenNunchucks, Item.SmallKnife, Item.WoodenRod, Item.Rapier, Item.IronHammer,
            Item.ShortSword, Item.HandAxe, Item.Scimitar, Item.IronNunchucks, Item.LargeKnife, Item.IronSword,
            Item.Sabre, Item.LongSword, Item.GreatAxe, Item.Falchon, Item.SilverKnife, Item.SilverSword,
            Item.SilverHammer, Item.SilverAxe, Item.FlameSword, Item.IceSword, Item.DragonSword,
            Item.GiantSword, Item.SunSword, Item.CoralSword, Item.WereSword, Item.RuneSword, Item.PowerRod,
            Item.LightAxe, Item.HealRod, Item.MageRod, Item.Defense, Item.WizardRod, Item.Vorpal, Item.CatClaw,
            Item.ThorHammer, Item.BaneSword, Item.Katana, Item.Xcalber, Item.Masamune
        };

        public static readonly IReadOnlyCollection<Item> AllArmor =
            new List<Item> {
            Item.Cloth, Item.WoodenArmor, Item.ChainArmor, Item.IronArmor, Item.SteelArmor, Item.SilverArmor,
            Item.FlameArmor, Item.IceArmor, Item.OpalArmor, Item.DragonArmor, Item.Copper, Item.Silver, Item.Gold,
            Item.Opal, Item.WhiteShirt, Item.BlackShirt, Item.WoodenShield, Item.IronShield, Item.SilverShield,
            Item.FlameShield, Item.IceShield, Item.OpalShield, Item.AegisShield, Item.Buckler, Item.ProCape, Item.Cap,
            Item.WoodenHelm, Item.IronHelm, Item.SilverHelm, Item.OpalHelm, Item.HealHelm, Item.Ribbon, Item.Gloves,
            Item.CopperGauntlets, Item.IronGauntlets, Item.SilverGauntlets, Item.ZeusGauntlets,
            Item.PowerGauntlets, Item.OpalGauntlets, Item.ProRing
        };

        public static readonly IReadOnlyCollection<Item> AllConsumables =
            new List<Item> { Item.Tent, Item.Cabin, Item.House, Item.Heal, Item.Pure, Item.Soft };

        public static readonly IReadOnlyCollection<Item> AllQuestItems =
            new List<Item> {
            Item.Lute, Item.Crown, Item.Crystal, Item.Herb, Item.Key, Item.Tnt, Item.Adamant, Item.Slab, Item.Ruby, Item.Rod,
            Item.Floater, Item.Chime, Item.Tail, Item.Cube, Item.Bottle, Item.Oxyale, Item.Canoe
        };

        public static readonly IReadOnlyCollection<Item> AllOrbs =
            new List<Item> {
            Item.EarthOrb, Item.FireOrb, Item.WaterOrb, Item.AirOrb
        };

        public static readonly IReadOnlyCollection<Item> AllGoldTreasure =
            new List<Item> {
            Item.Gold10, Item.Gold20, Item.Gold25, Item.Gold30, Item.Gold55, Item.Gold70, Item.Gold85, Item.Gold110, Item.
            Gold135, Item.Gold155, Item.Gold160, Item.Gold180, Item.Gold240, Item.Gold255, Item.Gold260, Item.
            Gold295, Item.Gold300, Item.Gold315, Item.Gold330, Item.Gold350, Item.Gold385, Item.Gold400, Item.
            Gold450, Item.Gold500, Item.Gold530, Item.Gold575, Item.Gold620, Item.Gold680, Item.Gold750, Item.
            Gold795, Item.Gold880, Item.Gold1020, Item.Gold1250, Item.Gold1455, Item.Gold1520, Item.Gold1760, Item.
            Gold1975, Item.Gold2000, Item.Gold2750, Item.Gold3400, Item.Gold4150, Item.Gold5000, Item.Gold5450, Item.
            Gold6400, Item.Gold6720, Item.Gold7340, Item.Gold7690, Item.Gold7900, Item.Gold8135, Item.Gold9000, Item.
            Gold9300, Item.Gold9500, Item.Gold9900, Item.Gold10000, Item.Gold12350, Item.Gold13000, Item.
            Gold13450, Item.Gold14050, Item.Gold14720, Item.Gold15000, Item.Gold17490, Item.Gold18010, Item.
            Gold19990, Item.Gold20000, Item.Gold20010, Item.Gold26000, Item.Gold45000, Item.Gold65000
        };

        public static readonly Dictionary<byte, string> TextLookup = 
            new Dictionary<Item, string>
        {
            {Item.Lute, nameof(Item.Lute)},
            {Item.Crown, nameof(Item.Crown)},
            {Item.Crystal, nameof(Item.Crystal)}, 
            {Item.Herb, nameof(Item.Herb)}, 
            {Item.Key, nameof(Item.Key)}, 
            {Item.Tnt, nameof(Item.Tnt)}, 
            {Item.Adamant, nameof(Item.Adamant)}, 
            {Item.Slab, nameof(Item.Slab)}, 
            {Item.Ruby, nameof(Item.Ruby)}, 
            {Item.Rod, nameof(Item.Rod)},
            {Item.Floater, nameof(Item.Floater)}, 
            {Item.Chime, nameof(Item.Chime)}, 
            {Item.Tail, nameof(Item.Tail)}, 
            {Item.Cube, nameof(Item.Cube)}, 
            {Item.Bottle, nameof(Item.Bottle)}, 
            {Item.Oxyale, nameof(Item.Oxyale)}, 
            {Item.Canoe, nameof(Item.Canoe)}, 
            {Item.Tent, nameof(Item.Tent)}, 
            {Item.Cabin, nameof(Item.Cabin)}, 
            {Item.House, nameof(Item.House)}, 
            {Item.Heal, nameof(Item.Heal)}, 
            {Item.Pure, nameof(Item.Pure)}, 
            {Item.Soft, nameof(Item.Soft)},
            {Item.WoodenNunchucks, nameof(Item.WoodenNunchucks)}, 
            {Item.SmallKnife, nameof(Item.SmallKnife)}, 
            {Item.WoodenRod, nameof(Item.WoodenRod)}, 
            {Item.Rapier, nameof(Item.Rapier)}, 
            {Item.IronHammer, nameof(Item.IronHammer)},
            {Item.ShortSword, nameof(Item.ShortSword)}, 
            {Item.HandAxe, nameof(Item.HandAxe)}, 
            {Item.Scimitar, nameof(Item.Scimitar)}, 
            {Item.IronNunchucks, nameof(Item.IronNunchucks)}, 
            {Item.LargeKnife, nameof(Item.LargeKnife)}, 
            {Item.IronSword, nameof(Item.IronSword)},
            {Item.Sabre, nameof(Item.Sabre)}, 
            {Item.LongSword, nameof(Item.LongSword)}, 
            {Item.GreatAxe, nameof(Item.GreatAxe)}, 
            {Item.Falchon, nameof(Item.Falchon)}, 
            {Item.SilverKnife, nameof(Item.SilverKnife)}, 
            {Item.SilverSword, nameof(Item.SilverSword)},
            {Item.SilverHammer, nameof(Item.SilverHammer)}, 
            {Item.SilverAxe, nameof(Item.SilverAxe)}, 
            {Item.FlameSword, nameof(Item.FlameSword)}, 
            {Item.IceSword, nameof(Item.IceSword)}, 
            {Item.DragonSword, nameof(Item.DragonSword)},
            {Item.GiantSword, nameof(Item.GiantSword)}, 
            {Item.SunSword, nameof(Item.SunSword)}, 
            {Item.CoralSword, nameof(Item.CoralSword)}, 
            {Item.WereSword, nameof(Item.WereSword)}, 
            {Item.RuneSword, nameof(Item.RuneSword)}, 
            {Item.PowerRod, nameof(Item.PowerRod)},
            {Item.LightAxe, nameof(Item.LightAxe)}, 
            {Item.HealRod, nameof(Item.HealRod)}, 
            {Item.MageRod, nameof(Item.MageRod)}, 
            {Item.Defense, nameof(Item.Defense)}, 
            {Item.WizardRod, nameof(Item.WizardRod)}, 
            {Item.Vorpal, nameof(Item.Vorpal)}, 
            {Item.CatClaw, nameof(Item.CatClaw)},
            {Item.ThorHammer, nameof(Item.ThorHammer)}, 
            {Item.BaneSword, nameof(Item.BaneSword)}, 
            {Item.Katana, nameof(Item.Katana)}, 
            {Item.Xcalber, nameof(Item.Xcalber)}, 
            {Item.Masamune, nameof(Item.Masamune)},
            {Item.Cloth, nameof(Item.Cloth)}, 
            {Item.WoodenArmor, nameof(Item.WoodenArmor)}, 
            {Item.ChainArmor, nameof(Item.ChainArmor)}, 
            {Item.IronArmor, nameof(Item.IronArmor)}, 
            {Item.SteelArmor, nameof(Item.SteelArmor)}, 
            {Item.SilverArmor, nameof(Item.SilverArmor)},
            {Item.FlameArmor, nameof(Item.FlameArmor)}, 
            {Item.IceArmor, nameof(Item.IceArmor)}, 
            {Item.OpalArmor, nameof(Item.OpalArmor)}, 
            {Item.DragonArmor, nameof(Item.DragonArmor)}, 
            {Item.Copper, nameof(Item.Copper)}, 
            {Item.Silver, nameof(Item.Silver)}, 
            {Item.Gold, nameof(Item.Gold)},
            {Item.Opal, nameof(Item.Opal)}, 
            {Item.WhiteShirt, nameof(Item.WhiteShirt)}, 
            {Item.BlackShirt, nameof(Item.BlackShirt)}, 
            {Item.WoodenShield, nameof(Item.WoodenShield)}, 
            {Item.IronShield, nameof(Item.IronShield)}, 
            {Item.SilverShield, nameof(Item.SilverShield)}, 
            {Item.FlameShield, nameof(Item.FlameShield)}, 
            {Item.IceShield, nameof(Item.IceShield)}, 
            {Item.OpalShield, nameof(Item.OpalShield)}, 
            {Item.AegisShield, nameof(Item.AegisShield)}, 
            {Item.Buckler, nameof(Item.Buckler)}, 
            {Item.ProCape, nameof(Item.ProCape)}, 
            {Item.Cap, nameof(Item.Cap)},
            {Item.WoodenHelm, nameof(Item.WoodenHelm)}, 
            {Item.IronHelm, nameof(Item.IronHelm)}, 
            {Item.SilverHelm, nameof(Item.SilverHelm)}, 
            {Item.OpalHelm, nameof(Item.OpalHelm)}, 
            {Item.HealHelm, nameof(Item.HealHelm)}, 
            {Item.Ribbon, nameof(Item.Ribbon)}, 
            {Item.Gloves, nameof(Item.Gloves)},
            {Item.CopperGauntlets, nameof(Item.CopperGauntlets)}, 
            {Item.IronGauntlets, nameof(Item.IronGauntlets)}, 
            {Item.SilverGauntlets, nameof(Item.SilverGauntlets)}, 
            {Item.ZeusGauntlets, nameof(Item.ZeusGauntlets)},
            {Item.PowerGauntlets, nameof(Item.PowerGauntlets)}, 
            {Item.OpalGauntlets, nameof(Item.OpalGauntlets)}, 
            {Item.ProRing, nameof(Item.ProRing)},
            {Item.Gold10, nameof(Item.Gold10)}, 
            {Item.Gold20, nameof(Item.Gold20)}, 
            {Item.Gold25, nameof(Item.Gold25)}, 
            {Item.Gold30, nameof(Item.Gold30)}, 
            {Item.Gold55, nameof(Item.Gold55)}, 
            {Item.Gold70, nameof(Item.Gold70)}, 
            {Item.Gold85, nameof(Item.Gold85)}, 
            {Item.Gold110, nameof(Item.Gold110)},
            {Item.Gold135, nameof(Item.Gold135)}, 
            {Item.Gold155, nameof(Item.Gold155)}, 
            {Item.Gold160, nameof(Item.Gold160)}, 
            {Item.Gold180, nameof(Item.Gold180)}, 
            {Item.Gold240, nameof(Item.Gold240)}, 
            {Item.Gold255, nameof(Item.Gold255)}, 
            {Item.Gold260, nameof(Item.Gold260)},
            {Item.Gold295, nameof(Item.Gold295)}, 
            {Item.Gold300, nameof(Item.Gold300)}, 
            {Item.Gold315, nameof(Item.Gold315)}, 
            {Item.Gold330, nameof(Item.Gold330)}, 
            {Item.Gold350, nameof(Item.Gold350)}, 
            {Item.Gold385, nameof(Item.Gold385)}, 
            {Item.Gold400, nameof(Item.Gold400)},
            {Item.Gold450, nameof(Item.Gold450)}, 
            {Item.Gold500, nameof(Item.Gold500)}, 
            {Item.Gold530, nameof(Item.Gold530)}, 
            {Item.Gold575, nameof(Item.Gold575)}, 
            {Item.Gold620, nameof(Item.Gold620)}, 
            {Item.Gold680, nameof(Item.Gold680)}, 
            {Item.Gold750, nameof(Item.Gold750)},
            {Item.Gold795, nameof(Item.Gold795)}, 
            {Item.Gold880, nameof(Item.Gold880)}, 
            {Item.Gold1020, nameof(Item.Gold1020)}, 
            {Item.Gold1250, nameof(Item.Gold1250)}, 
            {Item.Gold1455, nameof(Item.Gold1455)}, 
            {Item.Gold1520, nameof(Item.Gold1520)}, 
            {Item.Gold1760, nameof(Item.Gold1760)},
            {Item.Gold1975, nameof(Item.Gold1975)}, 
            {Item.Gold2000, nameof(Item.Gold2000)}, 
            {Item.Gold2750, nameof(Item.Gold2750)}, 
            {Item.Gold3400, nameof(Item.Gold3400)}, 
            {Item.Gold4150, nameof(Item.Gold4150)}, 
            {Item.Gold5000, nameof(Item.Gold5000)}, 
            {Item.Gold5450, nameof(Item.Gold5450)},
            {Item.Gold6400, nameof(Item.Gold6400)}, 
            {Item.Gold6720, nameof(Item.Gold6720)}, 
            {Item.Gold7340, nameof(Item.Gold7340)}, 
            {Item.Gold7690, nameof(Item.Gold7690)}, 
            {Item.Gold7900, nameof(Item.Gold7900)}, 
            {Item.Gold8135, nameof(Item.Gold8135)}, 
            {Item.Gold9000, nameof(Item.Gold9000)},
            {Item.Gold9300, nameof(Item.Gold9300)}, 
            {Item.Gold9500, nameof(Item.Gold9500)}, 
            {Item.Gold9900, nameof(Item.Gold9900)}, 
            {Item.Gold10000, nameof(Item.Gold10000)}, 
            {Item.Gold12350, nameof(Item.Gold12350)}, 
            {Item.Gold13000, nameof(Item.Gold13000)},
            {Item.Gold13450, nameof(Item.Gold13450)}, 
            {Item.Gold14050, nameof(Item.Gold14050)}, 
            {Item.Gold14720, nameof(Item.Gold14720)}, 
            {Item.Gold15000, nameof(Item.Gold15000)}, 
            {Item.Gold17490, nameof(Item.Gold17490)}, 
            {Item.Gold18010, nameof(Item.Gold18010)},
            {Item.Gold19990, nameof(Item.Gold19990)}, 
            {Item.Gold20000, nameof(Item.Gold20000)}, 
            {Item.Gold20010, nameof(Item.Gold20010)}, 
            {Item.Gold26000, nameof(Item.Gold26000)}, 
            {Item.Gold45000, nameof(Item.Gold45000)}, 
            {Item.Gold65000, nameof(Item.Gold65000)}
        }.ToDictionary(x => (byte)x.Key, x => x.Value);
    }
}
