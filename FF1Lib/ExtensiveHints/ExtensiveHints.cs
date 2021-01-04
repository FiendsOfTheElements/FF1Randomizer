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

		public ExtensiveHints(MT19337 _rng, NPCdata _npcData, Flags _flags, OverworldMap _overworldMap, FF1Rom _rom)
		{
			rng = _rng;
			npcData = _npcData;
			flags = _flags;
			overworldMap = _overworldMap;
			rom = _rom;

			translator = new Translator(rom);
		}		

		public void Generate()
		{	
			var bins = new List<string>[]
			{
				GenerateLooseItemFloorHints(),
				GenerateLooseItemNameHints(),
				GenerateIncentiveItemNameHints(),
				GenerateFloorHints(),
				GenerateEquipmentFloorHints(),
				GenerateEquipmentNameHints(),
			};

			var bincoverages = new int[]
			{
				(int)flags.ExtensiveHints_LooseItemFloorCoverage,
				(int)flags.ExtensiveHints_LooseItemNameCoverage,
				(int)flags.ExtensiveHints_IncentiveItemNameCoverage,
				(int)flags.ExtensiveHints_FloorHintCoverage,
				(int)flags.ExtensiveHints_EquipmentFloorCoverage,
				(int)flags.ExtensiveHints_EquipmentNameCoverage
			};

			var binorder = new (HintCategory, int)[]
			{
				(HintCategory.LooseItemFloor, (int)flags.ExtensiveHints_LooseItemFloorOrder),
				(HintCategory.LooseItemName, (int)flags.ExtensiveHints_LooseItemNameOrder),
				(HintCategory.IncentiveItemName, (int)flags.ExtensiveHints_IncentiveItemNameOrder),
				(HintCategory.FloorHint, (int)flags.ExtensiveHints_FloorHintOrder),
				(HintCategory.EquipmentFloor, (int)flags.ExtensiveHints_EquipmentFloorOrder),
				(HintCategory.EquipmentName, (int)flags.ExtensiveHints_EquipmentNameOrder)
			};

			binorder = binorder.OrderBy(x => x.Item2 * 100 + (int)x.Item1).ToArray();

			HashSet<ObjectId> usedIds = new HashSet<ObjectId>();
			Dictionary<ObjectId, string> hintPlacement = new Dictionary<ObjectId, string>();

			foreach (var b in binorder)
			{
				var c = b.Item1;
				PlaceCategory(rng, bins[(int)c], bincoverages[(int)c], flags.ExtensiveHints_BinMatrix[(int)c], usedIds, hintPlacement);
			}

			var availableIDs = ScavengeDialogIds(hintPlacement.Keys);

			Dictionary<int, string> dialogs = new Dictionary<int, string>();
			int i = 0;

			//If we didn't scavenge enough Ids, make a placeholder text.
			if (hintPlacement.Count > availableIDs.Count)
			{
				dialogs.Add(availableIDs[0], "I dont know what to say.\nMy master gave my TextId\nto someone else.");
				i++;
			}

			foreach (var e in hintPlacement)
			{
				if (i < availableIDs.Count)
				{
					npcData.GetTalkArray((ObjectId)e.Key)[(int)TalkArrayPos.dialogue_1] = 0;
					npcData.GetTalkArray((ObjectId)e.Key)[(int)TalkArrayPos.dialogue_2] = (byte)availableIDs[i];
					npcData.GetTalkArray((ObjectId)e.Key)[(int)TalkArrayPos.dialogue_3] = 0;
					npcData.SetRoutine((ObjectId)e.Key, newTalkRoutines.Talk_norm);
					dialogs.Add(availableIDs[i], e.Value);
					i++;
				}
				else
				{
					npcData.GetTalkArray((ObjectId)e.Key)[(int)TalkArrayPos.dialogue_1] = 0;
					npcData.GetTalkArray((ObjectId)e.Key)[(int)TalkArrayPos.dialogue_2] = (byte)availableIDs[0];
					npcData.GetTalkArray((ObjectId)e.Key)[(int)TalkArrayPos.dialogue_3] = 0;
					npcData.SetRoutine((ObjectId)e.Key, newTalkRoutines.Talk_norm);
				}
			}

			//That one has got to be in there
			dialogs.Add(0, "Please stop bothering\nthe static scenery.\n\nThank You.");
			dialogs.Add(80, "Please stop bothering\nthe static scenery.\n\nThank You.");

			rom.InsertDialogs(dialogs);
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

		private void PlaceCategory(MT19337 rng, List<string> hints, int coverage, bool[] matrix, HashSet<ObjectId> usedIds, Dictionary<ObjectId, string> dialogs)
		{
			if (hints.Count == 0) return;

			var pool = matrix.Select((v, i) => (l: (HintLocation)i, v: v)).Where(e => e.v).Select(e => HintNPCs.LocationDic[e.l]).SelectMany(x => x).Where(x => !usedIds.Contains(x)).ToList();

			if (pool.Count == 0) return;

			pool.Shuffle(rng);
			hints.Shuffle(rng);

			int cnt = hints.Count * coverage / 10;

			int j = 0;
			for (int i = 0; i < cnt; i++)
			{
				while (j < pool.Count && usedIds.Contains(pool[j])) j++;
				if (j >= pool.Count) break;

				dialogs.Add(pool[j], hints[i]);
				usedIds.Add(pool[j]);
			}
		}

		private List<string> GenerateLooseItemFloorHints()
		{
			List<string> hints = new List<string>();

			var incentivePool = GetIncentivePool(flags);
			var incentiveChests = GetIncentiveChests(flags);

			foreach (var item in incentivePool)
			{
				var placement = rom.generatedPlacement.Find(x => x.Item == item);
				if (placement != null)
				{
					if (placement is TreasureChest && !incentiveChests.Contains(placement.Name))
					{
						hints.Add("Loose\nis found in\n" + translator.TranslateFloor(placement.MapLocation));
					}
				}
			}

			return hints.Distinct().ToList();
		}

		private List<string> GenerateLooseItemNameHints()
		{
			List<string> hints = new List<string>();

			var incentivePool = GetIncentivePool(flags);
			var incentiveChests = GetIncentiveChests(flags);

			foreach (var item in incentivePool)
			{
				var placement = rom.generatedPlacement.Find(x => x.Item == item);
				if (placement != null)
				{
					if (placement is TreasureChest && !incentiveChests.Contains(placement.Name))
					{
						hints.Add(translator.TranslateItem(placement.Item) + "\nis found in\n" + translator.TranslateOverWorldLocation(placement.MapLocation));
					}
				}
			}

			return hints.Distinct().ToList();
		}

		private List<string> GenerateIncentiveItemNameHints()
		{
			List<string> hints = new List<string>();

			var incentivePool = GetIncentivePool(flags);
			var incentiveChests = GetIncentiveChests(flags);

			foreach (var item in incentivePool)
			{
				var placement = rom.generatedPlacement.Find(x => x.Item == item);
				if (placement != null)
				{
					if (placement is TreasureChest && incentiveChests.Contains(placement.Name))
					{
						hints.Add(translator.TranslateItem(placement.Item) + "\nis found in\n" + translator.TranslateOverWorldLocation(placement.MapLocation));
					}
					else if (placement is MapObject)
					{
						hints.Add(translator.TranslateItem(placement.Item) + "\nis held by\n" + translator.TranslateNpc(placement.Name));

					}
					else if (placement is ItemShopSlot)
					{
						hints.Add(translator.TranslateItem(placement.Item) + "\nis for sale in\n" + translator.TranslateFloor(placement.MapLocation));
					}
				}
			}

			return hints.Distinct().ToList();
		}

		private List<string> GenerateFloorHints()
		{
			List<string> hints = new List<string>();

			HashSet<Item> WowItems = new HashSet<Item>(ItemLists.UberTier.Concat(ItemLists.LegendaryWeaponTier).Concat(ItemLists.LegendaryArmorTier).Concat(ItemLists.AllQuestItems).Distinct());
			HashSet<Item> MehItems = new HashSet<Item>(ItemLists.RareWeaponTier.Concat(ItemLists.RareArmorTier).Distinct());
			HashSet<Item> SparkleItems = new HashSet<Item>(ItemLists.BigGoldTreasure);

			var locations = rom.generatedPlacement.Where(p => p is TreasureChest).GroupBy(p => p.MapLocation);

			foreach (var location in locations)
			{
				bool dud = true;
				if (location.FirstOrDefault(p => WowItems.Contains(p.Item)) != null)
				{
					hints.Add(translator.TranslateFloor(location.Key) + "\nis\nWow");
					dud = false;
				}
				else if (location.FirstOrDefault(p => MehItems.Contains(p.Item)) != null)
				{
					hints.Add(translator.TranslateFloor(location.Key) + "\nis\nMeh");
					dud = false;
				}				

				if (location.FirstOrDefault(p => SparkleItems.Contains(p.Item)) != null)
				{
					hints.Add(translator.TranslateFloor(location.Key) + "\nis\nSparkle");
					dud = false;
				}

				if(dud)
				{
					hints.Add(translator.TranslateFloor(location.Key) + "\nis\na Dud");
				}
			}

			return hints.Distinct().ToList();
		}

		private List<string> GenerateEquipmentFloorHints()
		{
			List<Item> WowItems = ItemLists.UberTier.Concat(ItemLists.LegendaryWeaponTier).Concat(ItemLists.LegendaryArmorTier).ToList();

			List<string> hints = new List<string>();

			foreach (var item in WowItems)
			{
				var placement = rom.generatedPlacement.Find(x => x.Item == item);
				if (placement != null)
				{
					if (placement is TreasureChest)
					{
						if (ItemLists.UberTier.Contains(placement.Item) || ItemLists.LegendaryWeaponTier.Contains(placement.Item))
						{
							hints.Add("L. Weapon\nis found in\n" + translator.TranslateFloor(placement.MapLocation));
						}
						else
						{
							hints.Add("L. Armor\nis found in\n" + translator.TranslateFloor(placement.MapLocation));
						}
					}
				}
			}

			return hints.Distinct().ToList();
		}
		private List<string> GenerateEquipmentNameHints()
		{
			List<string> hints = new List<string>();

			List<Item> WowItems = ItemLists.UberTier.Concat(ItemLists.LegendaryWeaponTier).Concat(ItemLists.LegendaryArmorTier).ToList();

			foreach (var item in WowItems)
			{
				var placement = rom.generatedPlacement.Find(x => x.Item == item);
				if (placement != null)
				{
					if (placement is TreasureChest)
					{
						hints.Add(translator.TranslateItem(placement.Item) + "\nis found in\n" + translator.TranslateOverWorldLocation(placement.MapLocation));
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
