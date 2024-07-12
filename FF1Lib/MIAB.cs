using System.ComponentModel;

namespace FF1Lib
{
	public enum ChestsPool
	{
		[Description("None")]
		None = 0,
		[Description("20")]
		Size20 = 20,
		[Description("40")]
		Size40 = 40,
		[Description("80")]
		Size80 = 80,
		[Description("160")]
		Size160 = 160,
		[Description("240")]
		Size240 = 240,
		[Description("Random")]
		Random = 255,
	}
	public enum FormationPool
	{
		[Description("Allocated Formations")]
		AltFormationDist,
		[Description("Random Formations")]
		AltFormationRng,
		[Description("Local Formations")]
		LocalFormations,
		[Description("Vanilla Spikes")]
		VanillaSpikes,
		[Description("Random Fiends")]
		Fiends,
		[Description("Phantom")]
		Phantom,
		[Description("Random")]
		Random,
	}
	public enum TCOptions
	{
		[Description("Never")]
		None,
		[Description("Random")]
		Pooled,
		[Description("Always")]
		All,
	}

	public partial class FF1Rom : NesRom
	{
		public void MonsterInABox(ItemPlacement itemPlacement, ZoneFormations zoneformations, TileSetsData tileSetsData, NpcObjectData npcdata, DialogueData dialogues, MT19337 rng, Flags flags)
		{
			if (!(bool)flags.TrappedChestsEnabled)
			{
				return;
			}

			const int lut_TreasureOffset = 0x3100;
			//const int BANK_SMINFO = 0x00;
			//const int lut_TileSMsetProp = 0x8800; // BANK_SMINFO - page                        - 0x100 bytes x 8  (2 bytes per)

			// Replace OpenTreasureChest routine, see 11_8EC0_CheckTrap.asm
			PutInBank(0x1F, 0xDD78, Blob.FromHex("A9002003FEA645BD00B18561A9112003FE20A08E8A60"));

			// Check for trapped monster routine, see 11_8EC0_CheckTrap.asm
			PutInBank(0x11,0x8EA0,Blob.FromHex("A5612080B1B045A645BD008FF03A856AA56148A9008561A9C0203D96A56A20808EA903CD866BD0096820189668684C4396688561A97BC56AF00B20EE8E201896A2F08645604C38C920EE8E60AA6018A5612010B4A445B90062090499006260"));

			// InTalkBattleNoRun to trigger fight
			PutInBank(0x11, 0x8E80, Blob.FromHex("856A8D410320CDD8A9008D01208D1540A002204A96A001204A9660"));

			dialogues[0x110] = "Monster-in-a-box!"; // 0xC0

			List<IRewardSource> validChests = new();
			var chestMonsterList = new byte[0x100];

			List<int> treasureList = new();

			for (int i = 0; i < 0x100; i++)
			{
				var index = itemPlacement.PlacedItems.FindIndex(r => (r.Address - 0x3100) == i);
				if (index > -1)
				{
					treasureList.Add((int)itemPlacement.PlacedItems[index].Item);
				}
				else
				{
					treasureList.Add((int)Item.Cabin);
				}
			}

			// Select treasures			
			var betterEquipmentList = ItemLists.UberTier.Concat(ItemLists.LegendaryArmorTier).Concat(ItemLists.LegendaryWeaponTier).Concat(ItemLists.RareArmorTier).Concat(ItemLists.RareWeaponTier);

			var RangeKeyItems = ItemLocations.AllTreasures.Where(x => !x.IsUnused && ItemLists.AllQuestItems.Contains((Item)treasureList[x.Address - lut_TreasureOffset]));
			var RangeShards = ItemLocations.AllTreasures.Where(x => !x.IsUnused && treasureList[x.Address - lut_TreasureOffset] == (int)Item.Shard);
			var RangeBetterTreasure = ItemLocations.AllTreasures.Where(x => !x.IsUnused && betterEquipmentList.Contains((Item)treasureList[x.Address - lut_TreasureOffset]));
			var RangeRandom = ItemLocations.AllTreasures.Where(x => !x.IsUnused && !RangeKeyItems.Contains(x) && !RangeShards.Contains(x) && !RangeBetterTreasure.Contains(x));
			var RangeMasamune = ItemLocations.AllTreasures.Where(x => !x.IsUnused && treasureList[x.Address - lut_TreasureOffset] == (int)Item.Masamune);

			if (flags.TCKeyItems == TCOptions.Pooled)
			{
				validChests.AddRange(RangeKeyItems);
			}

			if (flags.TCShards == TCOptions.Pooled)
			{
				validChests.AddRange(RangeShards);
			}

			if (flags.TCBetterTreasure == TCOptions.Pooled)
			{
				validChests.AddRange(RangeBetterTreasure);
			}

			if (flags.TCExcludeCommons == false)
			{
				validChests.AddRange(RangeRandom);
			}

			if ((bool)flags.TCMasaGuardian == true)
			{
				validChests.RemoveAll(x => treasureList[x.Address - lut_TreasureOffset] == (int)Item.Masamune);
			}

			if (flags.TCProtectIncentives == true)
			{
				validChests.RemoveAll(x => GetIncentiveList(flags).Contains((Item)treasureList[x.Address - lut_TreasureOffset]));
			}

			int maxChests = 0;
			int guaranteedChests = ((flags.TCKeyItems == TCOptions.All) ? RangeKeyItems.Count() : 0) + ((flags.TCShards == TCOptions.All) ? RangeShards.Count() : 0) + ((flags.TCBetterTreasure == TCOptions.All) ? RangeBetterTreasure.Count() : 0) + (((bool)flags.TCMasaGuardian) ? RangeMasamune.Count() : 0) + (((bool)flags.TrappedChaos) ? 1 : 0);

			if (flags.TCChestCount == 13)
			{
				maxChests = Rng.Between(rng, 20, (240 - guaranteedChests));
			}
			else if (flags.TCChestCount > 0)
			{
				maxChests = Math.Max(0, (flags.TCChestCount * 20) - guaranteedChests);
			}
			else
			{
				maxChests = 0;
			}

			// Get encounters
			const byte spookyZomBull = 0xB2;
			const byte spookyZombieD = 0xCB;
			const byte fightBahamut = 0xF1;

			List<byte> altEncountersList = new(FormationLists.BSideEncounters);
			altEncountersList.Add(0xFF); // IronGol

			if ((bool)flags.SpookyFlag)
			{
				altEncountersList.Remove(spookyZomBull);
				altEncountersList.Remove(spookyZombieD);
			}

			if ((bool)flags.FightBahamut)
			{
				altEncountersList.Remove(fightBahamut);
			}

			List<byte> encounters = new();
			List<List<byte>> encountersGroup = new();

			// Formations List for Vanilla Spikes & Local Formations
			List<byte> castleEncounters = new() { 0x84, 0x8C, 0x8D, 0x92 };
			List<byte> caveEncounters = new() { 0x9D, 0x9C, 0x95, 0x97 };
			List<byte> cardiaEncounters = new() { 0xAA, 0xB0, 0xCB, 0xCE, 0xD9 };

			// Formations List for Vanilla Spikes
			List<byte> tofEncounters = new() { 0x10 };
			List<byte> nwcastleEncounters = new() { 0x18, 0x1D };
			List<byte> marshEncounters = new() { 0x1C, 0x15 };
			List<byte> earthEncounters = new() { 0x21, 0x1E, 0x1F, 0x6E, 0x6F };
			List<byte> volcEncounters = new() { 0x27, 0x28, 0x29, 0x2A };
			List<byte> iceEncounters = new() { 0x2C, 0x2D, 0x2F, 0x30, 0x69, };
			List<byte> ordealsEncounters = new() { 0x3F, 0x4F, 0x4B };
			List<byte> waterfallEncounters = new() { 0x4A };
			List<byte> seaEncounters = new() { 0x44, 0x45, 0x49, 0x4A };
			List<byte> mirageEncounters = new() { 0x4E };
			List<byte> tofrEncounters = new() { 0x46 };

			if (flags.TCFormations == FormationPool.Random)
			{
				flags.TCFormations = (FormationPool)Rng.Between(rng, 0, 4);
			}

			switch (flags.TCFormations)
			{
				case FormationPool.AltFormationDist:
					List<List<byte>> baseEncounterList = new();
					for (int i = 0; i < 3; i++)
					{
						baseEncounterList.Add(new List<byte>(altEncountersList));
						baseEncounterList.Last().Shuffle(rng);
					}
					encounters = baseEncounterList.SelectMany(x => x).ToList();
					break;
				case FormationPool.AltFormationRng:
					encounters = altEncountersList;
					break;
				case FormationPool.LocalFormations:
					encountersGroup = zoneformations.GetBytes();

					encountersGroup[(int)MapIndex.ConeriaCastle1F] = castleEncounters;
					encountersGroup[(int)MapIndex.ElflandCastle] = castleEncounters;
					encountersGroup[(int)MapIndex.NorthwestCastle] = castleEncounters;
					encountersGroup[(int)MapIndex.CastleOrdeals1F] = castleEncounters;

					encountersGroup[(int)MapIndex.Cardia] = cardiaEncounters;
					encountersGroup[(int)MapIndex.BahamutCaveB1] = cardiaEncounters;
					encountersGroup[(int)MapIndex.BahamutCaveB2] = cardiaEncounters;

					encountersGroup[(int)MapIndex.DwarfCave] = caveEncounters;
					encountersGroup[(int)MapIndex.SardasCave] = caveEncounters;
					encountersGroup[(int)MapIndex.MatoyasCave] = caveEncounters;

					break;
				case FormationPool.VanillaSpikes:
					encountersGroup = new() {
						castleEncounters,
						castleEncounters,
						castleEncounters,
						castleEncounters,
						castleEncounters,
						castleEncounters,
						castleEncounters,
						castleEncounters,
						castleEncounters,
						castleEncounters,
						nwcastleEncounters,
						ordealsEncounters,
						tofEncounters,
						earthEncounters,
						volcEncounters,
						iceEncounters,
						cardiaEncounters,
						cardiaEncounters,
						waterfallEncounters,
						caveEncounters,
						caveEncounters,
						caveEncounters,
						marshEncounters,
						mirageEncounters,
						castleEncounters,
						ordealsEncounters,
						ordealsEncounters,
						marshEncounters,
						marshEncounters,
						earthEncounters,
						earthEncounters,
						earthEncounters,
						earthEncounters,
						volcEncounters,
						volcEncounters,
						volcEncounters,
						volcEncounters,
						iceEncounters,
						iceEncounters,
						cardiaEncounters,
						mirageEncounters,
						mirageEncounters,
						seaEncounters,
						seaEncounters,
						seaEncounters,
						seaEncounters,
						seaEncounters,
						mirageEncounters,
						mirageEncounters,
						mirageEncounters,
						mirageEncounters,
						mirageEncounters,
						tofrEncounters,
						tofrEncounters,
						tofrEncounters,
						tofrEncounters,
						tofrEncounters,
						tofrEncounters,
						tofrEncounters,
						tofrEncounters,
						caveEncounters,
					};
					break;
				case FormationPool.Fiends:
					encounters = Enumerable.Range(0x77, 4).Select(value => (byte)value).ToList();
					break;
				case FormationPool.Phantom:
					encounters = new List<byte> { 0x46 };
					break;
			}

			int altFormationPosition = 0;
			var chestsMapLocations = ItemLocations.GetTreasuresMapLocation().ToDictionary(x => x.Key, x => ItemLocations.MapLocationToMapIndex[x.Value]);

			byte GetEncounter(int i)
			{
				if (flags.TCFormations == FormationPool.LocalFormations || flags.TCFormations == FormationPool.VanillaSpikes)
				{
					return encountersGroup[(int)chestsMapLocations[i]].PickRandom(rng);
				}
				else if (flags.TCFormations == FormationPool.AltFormationDist)
				{
					return encounters[altFormationPosition++];
				}
				else
				{
					return encounters.PickRandom(rng);
				}
			}

			// Process pool first
			if (maxChests > 0)
			{
				maxChests = Math.Min(maxChests, validChests.Count);

				for (int i = 0; i < maxChests; i++)
				{
					var selectedChest = validChests.SpliceRandom(rng).Address - lut_TreasureOffset;
					chestMonsterList[selectedChest] = GetEncounter(selectedChest);
				}
			}

			// Better Treasure
			if (flags.TCBetterTreasure == TCOptions.All)
			{
				for (int i = 0; i < 0x100; i++)
				{
					if (betterEquipmentList.Contains((Item)treasureList[i]))
						chestMonsterList[i] = GetEncounter(i);
				}
			}

			// Key Items
			if (flags.TCKeyItems == TCOptions.All)
			{
				for (int i = 0; i < 0x100; i++)
				{
					if (ItemLists.AllQuestItems.Contains((Item)treasureList[i]))
						chestMonsterList[i] = GetEncounter(i);
				}
			}

			// Shards
			if (flags.TCShards == TCOptions.All)
			{
				for (int i = 0; i < 0x100; i++)
				{
					if (treasureList[i] == (byte)Item.Shard)
						chestMonsterList[i] = GetEncounter(i);
				}
			}

			// Masamune
			if ((bool)flags.TCMasaGuardian)
			{
				for (int i = 0; i < 0x100; i++)
				{
					if (treasureList[i] == (byte)Item.Masamune)
						chestMonsterList[i] = (byte)WarMECHFormationIndex;
				}
			}

			// Chaos
			if ((bool)flags.TrappedChaos)
			{
				List<MapLocation> disallowedLocations = new() { MapLocation.TempleOfFiends1Room1, MapLocation.TempleOfFiends1Room2, MapLocation.TempleOfFiends2, MapLocation.TempleOfFiends3, MapLocation.TempleOfFiendsAir, MapLocation.TempleOfFiendsChaos, MapLocation.TempleOfFiendsEarth, MapLocation.TempleOfFiendsFire, MapLocation.TempleOfFiendsPhantom, MapLocation.TempleOfFiendsWater, MapLocation.MatoyasCave, MapLocation.DwarfCave };

				validChests.RemoveAll(x => disallowedLocations.Contains(x.MapLocation));

				// Replace a random chest if there's no valid location left for Chaos
				if (!validChests.Any())
				{
					validChests = RangeRandom.ToList();
					validChests.RemoveAll(x => disallowedLocations.Contains(x.MapLocation) || ((bool)flags.TCMasaGuardian == true && treasureList[x.Address - lut_TreasureOffset] == (int)Item.Masamune) || ((bool)flags.TCProtectIncentives == true && GetIncentiveList(flags).Contains((Item)treasureList[x.Address - lut_TreasureOffset])));
				}

				var chaosChest = validChests.SpliceRandom(rng);
				itemPlacement.PlacedItems.RemoveAll(c => c.Address == chaosChest.Address);

				// Change Chaos item for none so the chest always open and stays local for AP
				itemPlacement.PlacedItems.Add(new TreasureChest(chaosChest, Item.None));

				chestMonsterList[chaosChest.Address - lut_TreasureOffset] = ChaosFormationIndex;

				SetChaosForMIAB(npcdata, dialogues);
			}


			//Spoilers
			/*
			int count = 0;
			for (int i = 0; i < 0x100; i++)
			{
				if (chestMonsterList[i] > 0)
				{
					count++;
					Console.WriteLine(count + ". " + ItemLocations.AllTreasures.Where(x => (x.Address - lut_TreasureOffset) == i).First().Name + " -> " + Enum.GetName((Item)treasureList[i]));
				}
			}
			*/

			// Mark trapped chests
			if ((bool)flags.TCIndicator)
			{ 
				for (int i = 0; i < 8; i++)
				{
					for (int j = 0; j < 0x80; j++)
					{


						var tempTileProperties = tileSetsData[i].Tiles[j].Properties;
						//var tempTileProperties = GetFromBank(BANK_SMINFO, lut_TileSMsetProp + (i * 0x100) + (j * 2), 2);
						if ((tempTileProperties.Byte1 & (byte)TilePropFunc.TP_SPEC_TREASURE) > 0 && (tempTileProperties.Byte1 & (byte)TilePropFunc.TP_NOMOVE) > 0)
						{
							if (chestMonsterList[(int)tempTileProperties.Byte2] > 0)
							{
								tileSetsData[i].Tiles[j].TileGraphic = new List<byte> { 0x2A, 0x7C, 0x3A, 0x3B };
								//TileSM temptile = new((byte)j, i, this);
								//temptile.TileGraphic = new List<byte> { 0x2A, 0x7C, 0x3A, 0x3B };
								//temptile.Write(this);
							}
						}
					}
				}
			}

			// Insert trapped chest list
			PutInBank(0x11, 0x8F00, chestMonsterList);
		}
		public void SetChaosForMIAB(NpcObjectData npcdata, DialogueData dialogues)
		{
			npcdata[ObjectId.Chaos3].Script = TalkScripts.Talk_4Orb;
			npcdata[ObjectId.Chaos3].Dialogue1 = 0x30;
			npcdata[ObjectId.Chaos3].Dialogue2 = 0x30;
			npcdata[ObjectId.Chaos3].Dialogue3 = 0x30;

			npcdata[ObjectId.Chaos1].Sprite = (ObjectSprites)0xF4;
			npcdata[ObjectId.Chaos2].Sprite = (ObjectSprites)0xF4;
			npcdata[ObjectId.Chaos3].Sprite = (ObjectSprites)0xF4;
			PutInBank(0x00, 0xA000 + ((byte)MapIndex.TempleOfFiendsRevisitedChaos * 0x30) + 0x18, Blob.FromHex("000F1636000F1636"));

			Dictionary<int, string> newgarlanddialogue = new Dictionary<int, string>();

			newgarlanddialogue.Add(0x2E, "That's right, it's me,\nBurtReynoldz. Didn't\nexpect to see me, right?");
			newgarlanddialogue.Add(0x2F, "Many moons ago I managed\nto run away from Chaos.\nAnd lo and behold,\nI BECAME Chaos. I took\nhis place.");
			newgarlanddialogue.Add(0x30, "Okay, I won't fight you.\nSome say you can find\nthe other Chaos hidden\nsomewhere in a chest.\nGood luck!");

			dialogues.InsertDialogues(newgarlanddialogue);
		}
		public static List<Item> GetIncentiveList(Flags flags)
		{
			var incentivePool = new List<Item>();

			if (flags.IncentivizeMasamune ?? false) incentivePool.Add(Item.Masamune);
			if (flags.IncentivizeKatana ?? false) incentivePool.Add(Item.Katana);
			if (flags.IncentivizeVorpal ?? false) incentivePool.Add(Item.Vorpal);
			if (flags.IncentivizeDefCastWeapon ?? false) incentivePool.Add(Item.Defense);
			if (flags.IncentivizeOffCastWeapon ?? false) incentivePool.Add(Item.ThorHammer);
			if (flags.IncentivizeOpal ?? false) incentivePool.Add(Item.Opal);
			if (flags.IncentivizeOtherCastArmor ?? false) incentivePool.Add(Item.PowerGauntlets);
			if (flags.IncentivizePowerRod ?? false) incentivePool.Add(Item.PowerRod);
			if (flags.IncentivizeDefCastArmor ?? false) incentivePool.Add(Item.WhiteShirt);
			if (flags.IncentivizeOffCastArmor ?? false) incentivePool.Add(Item.BlackShirt);
			if (flags.IncentivizeRibbon ?? false) incentivePool.Add(Item.Ribbon);
			if (flags.IncentivizeXcalber ?? false) incentivePool.Add(Item.Xcalber);

			if (flags.IncentivizeSlab ?? false) incentivePool.Add(Item.Slab);
			if (flags.IncentivizeRuby ?? false) incentivePool.Add(Item.Ruby);
			if (flags.IncentivizeFloater ?? false) incentivePool.Add(Item.Floater);
			if (flags.IncentivizeTnt ?? false) incentivePool.Add(Item.Tnt);
			if (flags.IncentivizeCrown ?? false) incentivePool.Add(Item.Crown);
			if (flags.IncentivizeTail ?? false) incentivePool.Add(Item.Tail);
			if (flags.IncentivizeAdamant ?? false) incentivePool.Add(Item.Adamant);
			if (flags.IncentivizeCrystal ?? false) incentivePool.Add(Item.Crystal);
			if (flags.IncentivizeBottle ?? false) incentivePool.Add(Item.Bottle);
			if (flags.IncentivizeHerb ?? false) incentivePool.Add(Item.Herb);

			if (flags.IncentivizeKey ?? false) incentivePool.Add(Item.Key);
			if (flags.IncentivizeOxyale ?? false) incentivePool.Add(Item.Oxyale);
			if (flags.IncentivizeLute ?? false) incentivePool.Add(Item.Lute);
			if (flags.IncentivizeRod ?? false) incentivePool.Add(Item.Rod);
			if (flags.IncentivizeCube ?? false) incentivePool.Add(Item.Cube);
			if (flags.IncentivizeChime ?? false) incentivePool.Add(Item.Chime);

			if (flags.IncentivizeBridge ?? false) incentivePool.Add(Item.Bridge);
			if (flags.IncentivizeShip ?? false) incentivePool.Add(Item.Ship);
			if (flags.IncentivizeCanoe ?? false) incentivePool.Add(Item.Canoe);
			if (flags.IncentivizeCanal ?? false) incentivePool.Add(Item.Canal);

			return incentivePool.ToList();
		}
	}
}
