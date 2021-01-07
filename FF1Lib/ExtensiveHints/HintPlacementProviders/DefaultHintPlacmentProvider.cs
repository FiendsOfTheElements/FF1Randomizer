using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public enum HintLocation
	{
		ConeriaCastle,
		Coneria,
		Pravoka,
		ElflandCastle,
		Elfland,
		Dwarfcave,
		Matoya,
		Melmond,
		CrescentLake,
		Cardia,
		Onrac,
		Mermaid,
		Gaia,
		Lefein
	}

	public class DefaultHintPlacmentProvider : BaseHintPlacementProvider
	{
		Dictionary<HintPlacementStrategy, List<ObjectId>> PlacementPools;

		public DefaultHintPlacmentProvider(MT19337 _rng, NPCdata _npcData, Flags _flags, OverworldMap _overworldMap, FF1Rom _rom) : base (_rng, _npcData, _flags, _overworldMap, _rom)
		{
			PlacementPools = StrategyDic.Select(s => (key: s.Key, values: s.Value.SelectMany(l => LocationDic[l]))).ToDictionary(s => s.key, s => s.values.ToList());
		}

		public override IEnumerable<HintPlacementStrategy> SupportedStrategies => new HintPlacementStrategy[]
		{
			HintPlacementStrategy.InnerSea,
			HintPlacementStrategy.ConeriaToCrescent,
			HintPlacementStrategy.ElflandToCrescent,
			HintPlacementStrategy.ElflandPlus,
			HintPlacementStrategy.MelmondOnrac,
			HintPlacementStrategy.MelmondMermaids,
			HintPlacementStrategy.MelmondPlus,
			HintPlacementStrategy.FloaterRequired,
			HintPlacementStrategy.Everywhere,
			HintPlacementStrategy.Tiered,
			HintPlacementStrategy.InnerSeaTownsAndDwarfCave
		};

		public override List<ObjectId> GetNpcPool(GeneratedHint hint, HashSet<ObjectId> usedIds)
		{
			var strategy = hint.PlacementStrategy;

			if (strategy == HintPlacementStrategy.Tiered)
			{
				var location = ItemLocations.MapLocationToStandardOverworldLocations[hint.MapLocation];

				switch (location)
				{
					case OverworldTeleportIndex.Coneria:
					case OverworldTeleportIndex.ConeriaCastle1:
					case OverworldTeleportIndex.TempleOfFiends1:
					case OverworldTeleportIndex.Pravoka:
					case OverworldTeleportIndex.Elfland:
					case OverworldTeleportIndex.ElflandCastle:
					case OverworldTeleportIndex.NorthwestCastle:
					case OverworldTeleportIndex.MarshCave1:
					case OverworldTeleportIndex.MatoyasCave:
					case OverworldTeleportIndex.DwarfCave:
						strategy = HintPlacementStrategy.InnerSea;
						break;
					case OverworldTeleportIndex.Melmond:
					case OverworldTeleportIndex.EarthCave1:
					case OverworldTeleportIndex.TitansTunnelEast:
					case OverworldTeleportIndex.TitansTunnelWest:
					case OverworldTeleportIndex.SardasCave:
					case OverworldTeleportIndex.CrescentLake:
					case OverworldTeleportIndex.GurguVolcano1:
						strategy = HintPlacementStrategy.ElflandToCrescent;
						break;
					case OverworldTeleportIndex.IceCave1:
					case OverworldTeleportIndex.Onrac:
					case OverworldTeleportIndex.Waterfall:
					case OverworldTeleportIndex.CastleOrdeals1:
					case OverworldTeleportIndex.MirageTower1:
						strategy = HintPlacementStrategy.MelmondOnrac;
						break;
					case OverworldTeleportIndex.Gaia:
					case OverworldTeleportIndex.Lefein:
					case OverworldTeleportIndex.Cardia1:
					case OverworldTeleportIndex.BahamutCave1:
					case OverworldTeleportIndex.Cardia2:
					case OverworldTeleportIndex.Cardia4:
					case OverworldTeleportIndex.Cardia5:
					case OverworldTeleportIndex.Cardia6:
					case OverworldTeleportIndex.Unused1:
					case OverworldTeleportIndex.Unused2:
						strategy = HintPlacementStrategy.MelmondMermaids;
						break;
					case (OverworldTeleportIndex)35:
						strategy = HintPlacementStrategy.MelmondOnrac;
						break;
					case (OverworldTeleportIndex)36:
						strategy = HintPlacementStrategy.MelmondOnrac;
						break;
					case (OverworldTeleportIndex)37:
						strategy = HintPlacementStrategy.MelmondMermaids;
						break;
					default:
						strategy = HintPlacementStrategy.Everywhere;
						break;
				}

				return PlacementPools[strategy].Where(x => !usedIds.Contains(x)).ToList();
			}
			else
			{
				return PlacementPools[strategy].Where(x => !usedIds.Contains(x)).ToList();
			}
		}

		public static IReadOnlyCollection<ObjectId> ConeriaCastle = new ObjectId[]
		{
			ObjectId.ConeriaCastle1FGuard1,
			ObjectId.ConeriaCastle1FWoman1,
			ObjectId.ConeriaCastle1FScholar,
			ObjectId.ConeriaCastle1FGuard2,
			ObjectId.ConeriaCastle1FWoman2,
			ObjectId.ConeriaCastle1FRoyal1,
			ObjectId.ConeriaCastle1FRoyal2,
			ObjectId.ConeriaCastle1FOldMan1,
			ObjectId.ConeriaCastle1FOldMan2,
			ObjectId.ConeriaCastle2FGuard1,
			ObjectId.ConeriaCastle2FGuard2,
			};

		public static IReadOnlyCollection<ObjectId> Coneria = new ObjectId[]
		{
			ObjectId.ConeriaGuard1,
			ObjectId.ConeriaGuard2,
			ObjectId.ConeriaDancer,
			ObjectId.ConeriaWoman1,
			ObjectId.ConeriaOldWoman,
			ObjectId.ConeriaWoman2,
			ObjectId.ConeriaMan,
			ObjectId.ConeriaOldMan,
		};

		public static IReadOnlyCollection<ObjectId> Pravoka = new ObjectId[]
		{
			ObjectId.PravokaMan1,
			ObjectId.PravokaOldMan,
			ObjectId.PravokaWoman,
			ObjectId.PravokaPunk,
			ObjectId.PravokaMan2,
		};

		public static IReadOnlyCollection<ObjectId> ElflandCastle = new ObjectId[]
		{
			ObjectId.ElflandCastleElf1,
			ObjectId.ElflandCastleElf2,
			ObjectId.ElflandCastleElf3,
			ObjectId.ElflandCastleScholar1,
			ObjectId.ElflandCastleElf4,
			ObjectId.ElflandCastleElf5,
		};

		public static IReadOnlyCollection<ObjectId> Elfland = new ObjectId[]
		{
			ObjectId.ElflandElf1,
			ObjectId.ElflandElf2,
			ObjectId.ElflandElf3,
			ObjectId.ElflandElf4,
			ObjectId.ElflandElf5,
			ObjectId.ElflandElf6,
			ObjectId.ElflandScholar1,
			ObjectId.ElflandScholar2,
		};

		public static IReadOnlyCollection<ObjectId> Dwarfcave = new ObjectId[]
		{
			ObjectId.DwarfcaveDwarf1,
			ObjectId.DwarfcaveDwarf2,
			ObjectId.DwarfcaveDwarf3,
			ObjectId.DwarfcaveDwarf4,
			ObjectId.DwarfcaveDwarf5,
			ObjectId.DwarfcaveDwarf6,
			ObjectId.DwarfcaveDwarf7,
			ObjectId.DwarfcaveDwarf8,
			ObjectId.DwarfcaveDwarf9,
			ObjectId.DwarfcaveDwarf10,
		};

		public static IReadOnlyCollection<ObjectId> Matoya = new ObjectId[]
		{
			ObjectId.MatoyaBroom1,
			ObjectId.MatoyaBroom2,
			ObjectId.MatoyaBroom3,
			ObjectId.MatoyaBroom4,
		};

		public static IReadOnlyCollection<ObjectId> Melmond = new ObjectId[]
		{
			ObjectId.MelmondMan1,
			ObjectId.MelmondMan2,
			ObjectId.MelmondMan3,
			ObjectId.MelmondOldMan1,
			ObjectId.MelmondMan4,
			ObjectId.MelmondMan5,
			ObjectId.MelmondOldMan2,
			ObjectId.MelmondMan6,
			ObjectId.MelmondMan7,
			ObjectId.MelmondMan8,
			ObjectId.MelmondWoman1,
			ObjectId.MelmondWoman2,
			ObjectId.MelmondDwarf,
		};

		public static IReadOnlyCollection<ObjectId> CrescentLake = new ObjectId[]
		{
			ObjectId.CrescentSage1,
			ObjectId.CrescentSage2,
			ObjectId.CrescentSage3,
			ObjectId.CrescentSage4,
			ObjectId.CrescentSage5,
			ObjectId.CrescentSage6,
			ObjectId.CrescentSage7,
			ObjectId.CrescentSage8,
			ObjectId.CrescentSage9,
			ObjectId.CrescentSage10,
			ObjectId.CrescentMan,
			ObjectId.CrescentSage11,
			ObjectId.CrescentWoman,
		};

		public static IReadOnlyCollection<ObjectId> Cardia = new ObjectId[]
		{
			ObjectId.CardiaDragon1,
			ObjectId.CardiaDragon2,
			ObjectId.CardiaDragon3,
			ObjectId.CardiaDragon4,
			//ObjectId.CardiaDragon5, does not exist on map???
			ObjectId.CardiaDragon6,
			ObjectId.CardiaDragon7,
			ObjectId.CardiaDragon8,
			ObjectId.CardiaDragon9,
			ObjectId.CardiaDragon10,
			ObjectId.CardiaDragon11,
			ObjectId.CardiaDragon12,
		};

		public static IReadOnlyCollection<ObjectId> Onrac = new ObjectId[]
		{
			ObjectId.OnracWoman1,
			ObjectId.OnracScholar,
			ObjectId.OnracGuard,
			ObjectId.OnracWitch,
			ObjectId.OnracDancer,
			ObjectId.OnracPunk1,
			ObjectId.OnracOldMan1,
			ObjectId.OnracOldMan2,
			ObjectId.OnracDragon,
			ObjectId.OnracPirate,
			ObjectId.OnracPunk2,
			ObjectId.OnracWoman2,
			ObjectId.OnracMan1,
			ObjectId.OnracMan2,
		};

		public static IReadOnlyCollection<ObjectId> Mermaid = new ObjectId[]
		{
			ObjectId.Mermaid1,
			ObjectId.Mermaid2,
			ObjectId.Mermaid3,
			ObjectId.Mermaid4,
			ObjectId.Mermaid5,
			ObjectId.Mermaid6,
			ObjectId.Mermaid7,
			ObjectId.Mermaid8,
			ObjectId.Mermaid9,
			ObjectId.Mermaid10,
		};

		public static IReadOnlyCollection<ObjectId> Gaia = new ObjectId[]
		{
			ObjectId.GaiaMan1,
			ObjectId.GaiaScholar1,
			ObjectId.GaiaScholar2,
			ObjectId.GaiaWoman,
			ObjectId.GaiaDancer,
			ObjectId.GaiaScholar3,
			ObjectId.GaiaMan2,
			ObjectId.GaiaMan3,
			ObjectId.GaiaPirate,
			ObjectId.GaiaMan4,
			ObjectId.GaiaBroom,
			ObjectId.GaiaWitch,
			ObjectId.GaiaOldWoman,
		};

		public static IReadOnlyCollection<ObjectId> Lefein = new ObjectId[]
		{
			ObjectId.LefeinMan1,
			ObjectId.LefeinMan2,
			ObjectId.LefeinMan3,
			ObjectId.LefeinMan4,
			ObjectId.LefeinMan5,
			ObjectId.LefeinMan6,
			ObjectId.LefeinMan7,
			ObjectId.LefeinMan8,
			ObjectId.LefeinMan9,
			ObjectId.LefeinMan10,
			ObjectId.LefeinMan11,
			ObjectId.LefeinMan12,
			ObjectId.LefeinMan13,
		};

		public static IReadOnlyDictionary<HintLocation, IReadOnlyCollection<ObjectId>> LocationDic = new Dictionary<HintLocation, IReadOnlyCollection<ObjectId>>
		{
			{ HintLocation.ConeriaCastle, ConeriaCastle },
			{ HintLocation.Coneria, Coneria },
			{ HintLocation.Pravoka, Pravoka },
			{ HintLocation.ElflandCastle, ElflandCastle },
			{ HintLocation.Elfland, Elfland },
			{ HintLocation.Dwarfcave, Dwarfcave },
			{ HintLocation.Matoya, Matoya },
			{ HintLocation.Melmond, Melmond },
			{ HintLocation.CrescentLake, CrescentLake },
			{ HintLocation.Cardia, Cardia },
			{ HintLocation.Onrac, Onrac },
			{ HintLocation.Mermaid, Mermaid },
			{ HintLocation.Gaia, Gaia },
			{ HintLocation.Lefein, Lefein },
		};

		public static Dictionary<HintPlacementStrategy, HintLocation[]> StrategyDic = new Dictionary<HintPlacementStrategy, HintLocation[]>
		{
			{
				HintPlacementStrategy.InnerSea, new HintLocation[]
				{
					HintLocation.Coneria,
					HintLocation.ConeriaCastle,
					HintLocation.Pravoka,
					HintLocation.Elfland,
					HintLocation.ElflandCastle,
					HintLocation.Matoya,
					HintLocation.Dwarfcave
				}
			},
			{
				HintPlacementStrategy.ConeriaToCrescent, new HintLocation[]
				{
					HintLocation.Coneria,
					HintLocation.ConeriaCastle,
					HintLocation.Pravoka,
					HintLocation.Elfland,
					HintLocation.ElflandCastle,
					HintLocation.Matoya,
					HintLocation.Dwarfcave,
					HintLocation.Melmond,
					HintLocation.CrescentLake
				}
			},
			{
				HintPlacementStrategy.ElflandToCrescent, new HintLocation[]
				{
					HintLocation.Elfland,
					HintLocation.ElflandCastle,
					HintLocation.Matoya,
					HintLocation.Dwarfcave,
					HintLocation.Melmond,
					HintLocation.CrescentLake
				}
			},
			{
				HintPlacementStrategy.ElflandPlus, new HintLocation[]
				{
					HintLocation.Elfland,
					HintLocation.ElflandCastle,
					HintLocation.Matoya,
					HintLocation.Dwarfcave,
					HintLocation.Melmond,
					HintLocation.CrescentLake,
					HintLocation.Onrac,
					HintLocation.Mermaid,
					HintLocation.Cardia,
					HintLocation.Lefein,
					HintLocation.Gaia
				}
			},
			{
				HintPlacementStrategy.MelmondOnrac, new HintLocation[]
				{
					HintLocation.Melmond,
					HintLocation.CrescentLake,
					HintLocation.Onrac,
				}
			},
			{
				HintPlacementStrategy.MelmondMermaids, new HintLocation[]
				{
					HintLocation.Melmond,
					HintLocation.CrescentLake,
					HintLocation.Onrac,
					HintLocation.Mermaid
				}
			},
			{
				HintPlacementStrategy.MelmondPlus, new HintLocation[]
				{
					HintLocation.Melmond,
					HintLocation.CrescentLake,
					HintLocation.Onrac,
					HintLocation.Mermaid,
					HintLocation.Cardia,
					HintLocation.Lefein,
					HintLocation.Gaia
				}
			},
			{
				HintPlacementStrategy.FloaterRequired, new HintLocation[]
				{
					HintLocation.Cardia,
					HintLocation.Lefein,
					HintLocation.Gaia
				}
			},
			{
				HintPlacementStrategy.Everywhere, new HintLocation[]
				{
					HintLocation.Coneria,
					HintLocation.ConeriaCastle,
					HintLocation.Pravoka,
					HintLocation.Elfland,
					HintLocation.ElflandCastle,
					HintLocation.Matoya,
					HintLocation.Dwarfcave,
					HintLocation.Melmond,
					HintLocation.CrescentLake,
					HintLocation.Onrac,
					HintLocation.Mermaid,
					HintLocation.Cardia,
					HintLocation.Lefein,
					HintLocation.Gaia
				}
			},
			{
				HintPlacementStrategy.InnerSeaTownsAndDwarfCave, new HintLocation[]
				{
					HintLocation.Coneria,
					HintLocation.Pravoka,
					HintLocation.Elfland,
					HintLocation.Dwarfcave
				}
			},
		};
	}
}
