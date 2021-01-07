using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public class ExtensiveHints
	{
		MT19337 rng;
		NPCdata npcData;
		Flags flags;
		OverworldMap overworldMap;
		FF1Rom rom;

		Translator translator;

		Dictionary<HintPlacementStrategy, List<ObjectId>> PlacementPools;

		int LooseCount;

		public ExtensiveHints(MT19337 _rng, NPCdata _npcData, Flags _flags, OverworldMap _overworldMap, FF1Rom _rom)
		{
			rng = _rng;
			npcData = _npcData;
			flags = _flags;
			overworldMap = _overworldMap;
			rom = _rom;

			translator = new Translator(rom);

			PlacementPools = HintNPCs.StrategyDic.Select(s => (key: s.Key, values: s.Value.SelectMany(l => HintNPCs.LocationDic[l]))).ToDictionary(s => s.key, s => s.values.ToList());
		}
		
		public void Generate()
		{
			List<GeneratedHint> hints = new List<GeneratedHint>();

			LooseCount = 0;

			hints.AddRange(GenerateLooseItemFloorHints());
			hints.AddRange(GenerateLooseItemNameHints());
			hints.AddRange(GenerateIncentiveItemNameHints());
			hints.AddRange(GenerateFloorHints());
			hints.AddRange(GenerateEquipmentFloorHints());

			var (prioritized, common, fill) = ProcessCoverage(hints);

			HashSet<ObjectId> usedIds = new HashSet<ObjectId>();
			Dictionary<ObjectId, string> hintPlacement = new Dictionary<ObjectId, string>();

			PlaceBin(rng, prioritized, usedIds, hintPlacement);
			PlaceBin(rng, common, usedIds, hintPlacement);
			PlaceBin(rng, fill, usedIds, hintPlacement);

			var availableIDs = ScavengeDialogIds(hintPlacement.Keys);

			Dictionary<int, string> dialogs = new Dictionary<int, string>();
			int i = 0;

			//If we didn't scavenge enough Ids, make a placeholder text.
			if (hintPlacement.Count > availableIDs.Count)
			{
				dialogs.Add(availableIDs[i], "I dont know what to say.\nMy master gave my TextId\nto someone else.");
				i++;
			}

			SetNpcHint(HintNPCs.LooseCountNpc, availableIDs[i], "I think there are " + LooseCount.ToString() + "\nLoose in the world.\n\nHappy Hunting!", dialogs);
			i++;

			foreach (var e in hintPlacement)
			{
				if (i < availableIDs.Count)
				{
					SetNpcHint((ObjectId)e.Key, availableIDs[i], e.Value, dialogs);
					i++;
				}
				else
				{
					SetNpcHint((ObjectId)e.Key, availableIDs[0], null, dialogs);
				}
			}

			//That one has got to be in there
			dialogs.Add(0, "Please stop bothering\nthe static scenery.\n\nThank You.");
			dialogs.Add(80, "Please stop bothering\nthe static scenery.\n\nThank You.");

			rom.InsertDialogs(dialogs);
		}

		private void SetNpcHint(ObjectId npc, int textId, string text, Dictionary<int, string> dialogs)
		{
			npcData.GetTalkArray(npc)[(int)TalkArrayPos.dialogue_1] = 0;
			npcData.GetTalkArray(npc)[(int)TalkArrayPos.dialogue_2] = (byte)textId;
			npcData.GetTalkArray(npc)[(int)TalkArrayPos.dialogue_3] = 0;
			npcData.SetRoutine(npc, newTalkRoutines.Talk_norm);
			if (text != null) dialogs.Add(textId, text);

			foreach(var npcinstance in rom.FindNpc(npc))
			{
				rom.SetNpc(npcinstance.Item1, npcinstance.Item2.Index, npc, npcinstance.Item2.Coord.x, npcinstance.Item2.Coord.y, npcinstance.Item2.InRoom, true);
			}
		}

		private (List<GeneratedHint>, List<GeneratedHint>, List<GeneratedHint>) ProcessCoverage(IEnumerable<GeneratedHint> hints)
		{
			var prioritized = new List<GeneratedHint>();
			var common = new List<GeneratedHint>();
			var fill = new List<GeneratedHint>();
			foreach (var category in Enum.GetValues<HintCategory>())
			{
				var cathints = hints.Where(h => h.Category == category).ToList();
				int cnt = cathints.Count * GetCoveragePercent(category) / 100;

				cathints.Shuffle(rng);

				if (GetCoverage(category) == HintCategoryCoverage.HintCategoryCoveragePrioritized)
				{
					prioritized.AddRange(cathints.Take(cnt));
				}
				else if (GetCoverage(category) == HintCategoryCoverage.HintCategoryCoverageFill)
				{
					fill.AddRange(cathints.Take(cnt));
				}
				else
				{
					common.AddRange(cathints.Take(cnt));
				}
			}

			return (prioritized, common, fill);
		}

		public HintCategoryCoverage GetCoverage(HintCategory category)
		{
			switch (category)
			{
				case HintCategory.LooseItemFloor: return flags.ExtensiveHints_LooseItemFloorCoverage;
				case HintCategory.LooseItemName: return flags.ExtensiveHints_LooseItemNameCoverage;
				case HintCategory.IncentiveItemName: return flags.ExtensiveHints_IncentiveItemNameCoverage;
				case HintCategory.FloorHint: return flags.ExtensiveHints_FloorHintCoverage;
				case HintCategory.EquipmentFloor: return flags.ExtensiveHints_EquipmentFloorCoverage;
				case HintCategory.EquipmentName: return flags.ExtensiveHints_EquipmentNameCoverage;
				default: return HintCategoryCoverage.HintCategoryCoverageNone;
			}
		}

		public int GetCoveragePercent(HintCategory category)
		{
			var coverage = GetCoverage(category);

			switch (coverage)
			{
				case HintCategoryCoverage.HintCategoryCoverageNone: return 0;
				case HintCategoryCoverage.HintCategoryCoverage20: return 20;
				case HintCategoryCoverage.HintCategoryCoverage40: return 40;
				case HintCategoryCoverage.HintCategoryCoverage60: return 60;
				case HintCategoryCoverage.HintCategoryCoverage80: return 80;
				case HintCategoryCoverage.HintCategoryCoverageAll: return 100;
				case HintCategoryCoverage.HintCategoryCoveragePrioritized: return 100;
				case HintCategoryCoverage.HintCategoryCoverageFill: return 100;
				default: return 0;
			}
		}

		private List<int> ScavengeDialogIds(IEnumerable<ObjectId> usedNPCs)
		{
			HashSet<ObjectId> usedNPCSet = new HashSet<ObjectId>(usedNPCs);
			List<int> usedDialogs = new List<int>();
			List<int> everythingElse = new List<int>();

			for (int i = 0; i < 208; i++)
			{
				if (usedNPCSet.Contains((ObjectId)i))
				{
					usedDialogs.Add(npcData.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1]);
					usedDialogs.Add(npcData.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2]);
					usedDialogs.Add(npcData.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3]);
				}
				else
				{
					everythingElse.Add(npcData.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1]);
					everythingElse.Add(npcData.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2]);
					everythingElse.Add(npcData.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3]);
				}
			}

			var usedDialogSet = new HashSet<int>(usedDialogs.Distinct());
			foreach (var d in everythingElse) if (usedDialogSet.Contains(d)) usedDialogSet.Remove(d);

			return usedDialogSet.ToList();
		}

		private void PlaceBin(MT19337 rng, List<GeneratedHint> hints, HashSet<ObjectId> usedIds, Dictionary<ObjectId, string> dialogs)
		{
			if (hints.Count == 0) return;

			foreach (var hint in hints)
			{
				var pool = GetNpcPool(hint, usedIds);
				if (pool.Count > 0)
				{
					var npc = pool.PickRandom(rng);
					dialogs.Add(npc, hint.Text);
					usedIds.Add(npc);
				}
			}
		}

		private List<ObjectId> GetNpcPool(GeneratedHint hint, HashSet<ObjectId> usedIds)
		{
			var strategy = GetStrategy(hint.Category);

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
		public HintPlacementStrategy GetStrategy(HintCategory category)
		{
			switch (category)
			{
				case HintCategory.LooseItemFloor: return flags.ExtensiveHints_LooseItemFloorPlacement;
				case HintCategory.LooseItemName: return flags.ExtensiveHints_LooseItemNamePlacement;
				case HintCategory.IncentiveItemName: return flags.ExtensiveHints_IncentiveItemNamePlacement;
				case HintCategory.FloorHint: return flags.ExtensiveHints_FloorHintPlacement;
				case HintCategory.EquipmentFloor: return flags.ExtensiveHints_EquipmentFloorPlacement;
				case HintCategory.EquipmentName: return flags.ExtensiveHints_EquipmentNamePlacement;
				default: return HintPlacementStrategy.Everywhere;
			}
		}

		private List<GeneratedHint> GenerateLooseItemFloorHints()
		{
			List<GeneratedHint> hints = new List<GeneratedHint>();

			var incentivePool = GetIncentivePool(flags);
			var incentiveChests = GetIncentiveChests(flags);

			foreach (var item in incentivePool)
			{
				var placement = rom.generatedPlacement.Find(x => x.Item == item);
				if (placement != null)
				{
					if (placement is TreasureChest && !incentiveChests.Contains(placement.Name))
					{
						LooseCount++;
						hints.Add(new GeneratedHint
						{
							Category = HintCategory.LooseItemFloor,
							MapLocation = placement.MapLocation,
							Text = "There is {0} Loose\nfound in\n" + translator.TranslateFloor(placement.MapLocation) + "!"
						});
					}
				}
			}

			return hints.GroupBy(h => h.Text).Select(h => new GeneratedHint { Category = h.First().Category, MapLocation = h.First().MapLocation, Text = h.First().Text.Replace("{0}", h.Count().ToString()) }).ToList();

			//return hints.Distinct().ToList();
		}

		private List<GeneratedHint> GenerateLooseItemNameHints()
		{
			List<GeneratedHint> hints = new List<GeneratedHint>();

			var incentivePool = GetIncentivePool(flags);
			var incentiveChests = GetIncentiveChests(flags);

			foreach (var item in incentivePool)
			{
				var placement = rom.generatedPlacement.Find(x => x.Item == item);
				if (placement != null)
				{
					if (placement is TreasureChest && !incentiveChests.Contains(placement.Name))
					{
						hints.Add(new GeneratedHint
						{
							Category = HintCategory.LooseItemName,
							MapLocation = placement.MapLocation,
							Text = translator.TranslateItem(placement.Item) + "\nis found in\n" + translator.TranslateOverWorldLocation(placement.MapLocation) + "!"
						});
					}
				}
			}

			return hints.Distinct().ToList();
		}

		private List<GeneratedHint> GenerateIncentiveItemNameHints()
		{
			List<GeneratedHint> hints = new List<GeneratedHint>();

			var incentivePool = GetIncentivePool(flags);
			var incentiveChests = GetIncentiveChests(flags);

			foreach (var item in incentivePool)
			{
				var placement = rom.generatedPlacement.Find(x => x.Item == item);
				if (placement != null)
				{
					if (placement is TreasureChest && incentiveChests.Contains(placement.Name))
					{
						hints.Add(new GeneratedHint
						{
							Category = HintCategory.IncentiveItemName,
							MapLocation = placement.MapLocation,
							Text = translator.TranslateItem(placement.Item) + "\nis found in\n" + translator.TranslateOverWorldLocation(placement.MapLocation) + "!"
						});
					}
					else if (placement is MapObject)
					{
						hints.Add(new GeneratedHint
						{
							Category = HintCategory.IncentiveItemName,
							MapLocation = placement.MapLocation,
							Text = translator.TranslateItem(placement.Item) + "\nis held by\n" + translator.TranslateNpc(placement.Name) + "!"
						});

					}
					else if (placement is ItemShopSlot)
					{
						hints.Add(new GeneratedHint
						{
							Category = HintCategory.IncentiveItemName,
							MapLocation = placement.MapLocation,
							Text = translator.TranslateItem(placement.Item) + "\nis for sale in\n" + translator.TranslateFloor(placement.MapLocation) + "!"
						});
					}
				}
			}

			return hints.Distinct().ToList();
		}

		private List<GeneratedHint> GenerateFloorHints()
		{
			List<GeneratedHint> hints = new List<GeneratedHint>();

			HashSet<Item> WowItems = new HashSet<Item>(ItemLists.UberTier.Concat(ItemLists.LegendaryWeaponTier).Concat(ItemLists.LegendaryArmorTier).Concat(ItemLists.AllQuestItems).Distinct());
			HashSet<Item> MehItems = new HashSet<Item>(ItemLists.RareWeaponTier.Concat(ItemLists.RareArmorTier).Distinct());
			HashSet<Item> SparkleItems = new HashSet<Item>(ItemLists.BigGoldTreasure);

			var locations = rom.generatedPlacement.Where(p => p is TreasureChest).GroupBy(p => p.MapLocation);

			foreach (var location in locations)
			{
				bool wow = location.FirstOrDefault(p => WowItems.Contains(p.Item)) != null;
				bool meh = location.FirstOrDefault(p => MehItems.Contains(p.Item)) != null;
				bool sparkle = location.FirstOrDefault(p => SparkleItems.Contains(p.Item)) != null;

				if (wow && sparkle)
				{
					hints.Add(new GeneratedHint
					{
						Category = HintCategory.FloorHint,
						MapLocation = location.Key,
						Text = translator.TranslateFloor(location.Key) + "\nis\nWow and Sparkle!"
					});
				}
				else if (wow && !sparkle)
				{
					hints.Add(new GeneratedHint
					{
						Category = HintCategory.FloorHint,
						MapLocation = location.Key,
						Text = translator.TranslateFloor(location.Key) + "\nis\nWow!"
					});
				}
				else if (meh && sparkle)
				{
					hints.Add(new GeneratedHint
					{
						Category = HintCategory.FloorHint,
						MapLocation = location.Key,
						Text = translator.TranslateFloor(location.Key) + "\nis\nMeh but Sparkle!"
					});
				}
				else if (meh && !sparkle)
				{
					hints.Add(new GeneratedHint
					{
						Category = HintCategory.FloorHint,
						MapLocation = location.Key,
						Text = translator.TranslateFloor(location.Key) + "\nis\nMeh!"
					});
				}
				else if (sparkle)
				{
					hints.Add(new GeneratedHint
					{
						Category = HintCategory.FloorHint,
						MapLocation = location.Key,
						Text = translator.TranslateFloor(location.Key) + "\nis\njust Sparkle!"
					});
				}
				else
				{
					hints.Add(new GeneratedHint
					{
						Category = HintCategory.FloorHint,
						MapLocation = location.Key,
						Text = translator.TranslateFloor(location.Key) + "\nis\na Dud!"
					});
				}
			}

			return hints.Distinct().ToList();
		}

		private List<GeneratedHint> GenerateEquipmentFloorHints()
		{
			List<Item> WowItems = ItemLists.UberTier.Concat(ItemLists.LegendaryWeaponTier).Concat(ItemLists.LegendaryArmorTier).Concat(ItemLists.RareWeaponTier).Concat(ItemLists.RareArmorTier).ToList();

			List<GeneratedHint> hints = new List<GeneratedHint>();

			foreach (var item in WowItems)
			{
				var placement = rom.generatedPlacement.Find(x => x.Item == item);
				if (placement != null)
				{
					if (placement is TreasureChest)
					{
						hints.Add(new GeneratedHint
						{
							Category = HintCategory.EquipmentFloor,
							MapLocation = placement.MapLocation,
							Text = translator.TranslateItem(placement.Item) + "\nis found in\n" + translator.TranslateFloor(placement.MapLocation) + "!"
						});
					}
				}
			}

			return hints.Distinct().ToList();
		}

		private List<Item> GetIncentivePool(Flags flags)
		{
			var incentivePool = new List<Item>();

			if (flags.IncentivizeMasamune ?? false) incentivePool.Add(Item.Masamune);
			if (flags.IncentivizeKatana ?? false) incentivePool.Add(Item.Katana);
			if (flags.IncentivizeVorpal ?? false) incentivePool.Add(Item.Vorpal);
			if (flags.IncentivizeDefCastWeapon ?? false) incentivePool.Add(Item.Defense);
			if (flags.IncentivizeOffCastWeapon ?? false) incentivePool.Add(Item.ThorHammer);
			if (flags.IncentivizeOpal ?? false) incentivePool.Add(Item.Opal);
			if (flags.IncentivizeOtherCastArmor ?? false) incentivePool.Add(Item.PowerGauntlets);
			if (flags.IncentivizeDefCastArmor ?? false) incentivePool.Add(Item.WhiteShirt);
			if (flags.IncentivizeOffCastArmor ?? false) incentivePool.Add(Item.BlackShirt);
			if (flags.IncentivizeRibbon ?? false) incentivePool.Add(Item.Ribbon);
			if (flags.IncentivizeSlab ?? false) incentivePool.Add(Item.Slab);
			if (flags.IncentivizeRuby ?? false) incentivePool.Add(Item.Ruby);
			if (flags.IncentivizeFloater ?? false) incentivePool.Add(Item.Floater);
			if (flags.IncentivizeTnt ?? false) incentivePool.Add(Item.Tnt);
			if (flags.IncentivizeCrown ?? false) incentivePool.Add(Item.Crown);
			if (flags.IncentivizeTail ?? false) incentivePool.Add(Item.Tail);
			if (flags.IncentivizeAdamant ?? false) incentivePool.Add(Item.Adamant);

			if (flags.IncentivizeBridge) incentivePool.Add(Item.Bridge);
			if (flags.IncentivizeLute ?? false) incentivePool.Add(Item.Lute);
			if (flags.IncentivizeShip ?? false) incentivePool.Add(Item.Ship);
			if (flags.IncentivizeRod ?? false) incentivePool.Add(Item.Rod);
			if (flags.IncentivizeCanoe ?? false) incentivePool.Add(Item.Canoe);
			if (flags.IncentivizeCube ?? false) incentivePool.Add(Item.Cube);
			if (flags.IncentivizeBottle ?? false) incentivePool.Add(Item.Bottle);

			if (flags.IncentivizeKey ?? false) incentivePool.Add(Item.Key);
			if (flags.IncentivizeCrystal ?? false) incentivePool.Add(Item.Crystal);
			if (flags.IncentivizeOxyale ?? false) incentivePool.Add(Item.Oxyale);
			if (flags.IncentivizeCanal ?? false) incentivePool.Add(Item.Canal);
			if (flags.IncentivizeHerb ?? false) incentivePool.Add(Item.Herb);
			if (flags.IncentivizeChime ?? false) incentivePool.Add(Item.Chime);
			if (flags.IncentivizeXcalber ?? false) incentivePool.Add(Item.Xcalber);

			return incentivePool.Concat(ItemLists.AllQuestItems).Distinct().ToList();
		}

		private List<string> GetIncentiveChests(Flags flags)
		{
			var incentivizedChests = new List<string>();

			if (flags.IncentivizeEarth ?? false) incentivizedChests.Add(ItemLocations.EarthCaveMajor.Name);
			if (flags.IncentivizeIceCave ?? false) incentivizedChests.Add(ItemLocations.IceCaveMajor.Name);
			if (flags.IncentivizeMarsh ?? false) incentivizedChests.Add(ItemLocations.MarshCaveMajor.Name);
			if (flags.IncentivizeMarshKeyLocked ?? false) incentivizedChests.Add(ItemLocations.MarshCave13.Name);
			if (flags.IncentivizeOrdeals ?? false) incentivizedChests.Add(ItemLocations.OrdealsMajor.Name);
			if (flags.IncentivizeSeaShrine ?? false) incentivizedChests.Add(ItemLocations.SeaShrineMajor.Name);
			if (flags.IncentivizeSkyPalace ?? false) incentivizedChests.Add(ItemLocations.SkyPalaceMajor.Name);
			if (flags.IncentivizeTitansTrove ?? false) incentivizedChests.Add(ItemLocations.TitansTunnel1.Name);
			if (flags.IncentivizeVolcano ?? false) incentivizedChests.Add(ItemLocations.VolcanoMajor.Name);
			if (flags.IncentivizeConeria ?? false) incentivizedChests.Add(ItemLocations.ConeriaMajor.Name);

			return incentivizedChests;
		}
	}
}
