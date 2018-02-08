using System.Collections.Generic;
using System.Linq;

namespace FF1Lib
{
	public class IncentiveData
	{
		public IncentiveData(IIncentiveFlags flags)
		{
			var incentivePool = new List<Item>();
			if (flags.IncentivizeBridge)
			{
				incentivePool.Add(Item.Bridge);
			}
			if (flags.IncentivizeShip)
			{
				incentivePool.Add(Item.Ship);
			}
			if (flags.IncentivizeCanal)
			{
				incentivePool.Add(Item.Canal);
			}
			if (flags.IncentivizeLute)
			{
				incentivePool.Add(Item.Lute);
			}
			if (flags.IncentivizeCrown)
			{
				incentivePool.Add(Item.Crown);
			}
			if (flags.IncentivizeCrystal)
			{
				incentivePool.Add(Item.Crystal);
			}
			if (flags.IncentivizeHerb)
			{
				incentivePool.Add(Item.Herb);
			}
			if (flags.IncentivizeKey)
			{
				incentivePool.Add(Item.Key);
			}
			if (flags.IncentivizeTnt)
			{
				incentivePool.Add(Item.Tnt);
			}
			if (flags.IncentivizeAdamant)
			{
				incentivePool.Add(Item.Adamant);
			}
			if (flags.IncentivizeSlab)
			{
				incentivePool.Add(Item.Slab);
			}
			if (flags.IncentivizeRuby)
			{
				incentivePool.Add(Item.Ruby);
			}
			if (flags.IncentivizeRod)
			{
				incentivePool.Add(Item.Rod);
			}
			if (flags.IncentivizeFloater)
			{
				incentivePool.Add(Item.Floater);
			}
			if (flags.IncentivizeChime)
			{
				incentivePool.Add(Item.Chime);
			}
			if (flags.IncentivizeTail)
			{
				incentivePool.Add(Item.Tail);
			}
			if (flags.IncentivizeCube)
			{
				incentivePool.Add(Item.Cube);
			}
			if (flags.IncentivizeBottle)
			{
				incentivePool.Add(Item.Bottle);
			}
			if (flags.IncentivizeOxyale)
			{
				incentivePool.Add(Item.Oxyale);
			}
			if (flags.IncentivizeCanoe)
			{
				incentivePool.Add(Item.Canoe);
			}

			if (flags.IncentivizeXcalber)
			{
				incentivePool.Add(Item.Xcalber);
			}
			if (flags.IncentivizeMasamune)
			{
				incentivePool.Add(Item.Masamune);
			}
			if (flags.IncentivizeRibbon)
			{
				incentivePool.Add(Item.Ribbon);
			}
			if (flags.IncentivizeRibbon2)
			{
				incentivePool.Add(Item.Ribbon);
			}
			if (flags.IncentivizePowerGauntlet)
			{
				incentivePool.Add(Item.PowerGauntlets);
			}
			if (flags.IncentivizeWhiteShirt)
			{
				incentivePool.Add(Item.WhiteShirt);
			}
			if (flags.IncentivizeBlackShirt)
			{
				incentivePool.Add(Item.BlackShirt);
			}
			if (flags.IncentivizeOpal)
			{
				incentivePool.Add(Item.Opal);
			}
			if (flags.Incentivize65K)
			{
				incentivePool.Add(Item.Gold65000);
			}
			if (flags.IncentivizeBad)
			{
				incentivePool.Add(Item.Cloth);
			}
			IncentiveItems = incentivePool;

			var incentiveLocationPool = new List<IRewardSource>();
			if (flags.IncentivizeKingConeria)
			{
				incentiveLocationPool.Add(ItemLocations.KingConeria);
			}
			if (flags.IncentivizePrincess)
			{
				incentiveLocationPool.Add(ItemLocations.Princess);
			}
			if (flags.IncentivizeMatoya)
			{
				incentiveLocationPool.Add(ItemLocations.Matoya);
			}
			if (flags.IncentivizeBikke)
			{
				incentiveLocationPool.Add(ItemLocations.Bikke);
			}
			if (flags.IncentivizeElfPrince)
			{
				incentiveLocationPool.Add(ItemLocations.ElfPrince);
			}
			if (flags.IncentivizeAstos)
			{
				incentiveLocationPool.Add(ItemLocations.Astos);
			}
			if (flags.IncentivizeNerrick)
			{
				incentiveLocationPool.Add(ItemLocations.Nerrick);
			}
			if (flags.IncentivizeSmith)
			{
				incentiveLocationPool.Add(ItemLocations.Smith);
			}
			if (flags.IncentivizeSarda)
			{
				incentiveLocationPool.Add(ItemLocations.Sarda);
			}
			if (flags.IncentivizeCanoeSage)
			{
				incentiveLocationPool.Add(ItemLocations.CanoeSage);
			}
			if (flags.IncentivizeCubeBot)
			{
				incentiveLocationPool.Add(ItemLocations.CubeBot);
			}
			if (flags.IncentivizeFairy)
			{
				incentiveLocationPool.Add(ItemLocations.Fairy);
			}
			if (flags.IncentivizeLefein)
			{
				incentiveLocationPool.Add(ItemLocations.Lefein);
			}
			if (flags.IncentivizeVolcano)
			{
				incentiveLocationPool.Add(ItemLocations.VolcanoMajor);
			}
			if (flags.IncentivizeEarth)
			{
				incentiveLocationPool.Add(ItemLocations.EarthCaveMajor);
			}
			if (flags.IncentivizeMarsh)
			{
				incentiveLocationPool.Add(ItemLocations.MarshCaveMajor);
			}
			if (flags.IncentivizeSeaShrine)
			{
				incentiveLocationPool.Add(ItemLocations.SeaShrineLocked);
			}
			if (flags.IncentivizeConeria)
			{
				incentiveLocationPool.Add(ItemLocations.ConeriaMajor);
			}
			if (flags.IncentivizeIceCave)
			{
				incentiveLocationPool.Add(ItemLocations.IceCaveMajor);
			}
			if (flags.IncentivizeOrdeals)
			{
				incentiveLocationPool.Add(ItemLocations.OrdealsMajor);
			}
			IncentiveLocations = incentiveLocationPool;
		}

		public IEnumerable<IRewardSource> IncentiveLocations { get; }
		public IEnumerable<Item> IncentiveItems { get; }

	}
}
