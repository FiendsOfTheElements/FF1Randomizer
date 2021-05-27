using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
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
		[Description("Random Fiends")]
		Fiends,
		[Description("Phantom")]
		Phantom,
		[Description("Random")]
		Random,
	}
	public enum TCOptions
	{
		[Description("None")]
		None,
		[Description("All")]
		All,
		[Description("Pooled")]
		Pooled,
	}
	public enum TCRngOptions
	{
		[Description("None")]
		None,
		[Description("Pooled")]
		Pooled,
	}

	public partial class FF1Rom : NesRom
	{
		public void MonsterInABox(MT19337 rng, Flags flags)
		{
			const int lut_TreasureOffset = 0x3100;
			const int BANK_SMINFO = 0x00;
			const int lut_TileSMsetProp = 0x8800; // BANK_SMINFO - page                        - 0x100 bytes x 8  (2 bytes per)

			// Replace OpenTreasureChest routine, see 11_8EC0_CheckTrap.asm
			PutInBank(0x1F, 0xDD78, Blob.FromHex("A9002003FEA645BD00B18561A9112003FE20A08E8A60"));

			// Check for trapped monster routine, see 11_8EC0_CheckTrap.asm
			PutInBank(0x11,0x8EA0,Blob.FromHex("A5612080B1B045A645BD008FF03A856AA56148A9008561A9C0203D96A56A200096A903CD866BD0096820189668684C4396688561A97BC56AF00B20EE8E201896A2F08645604C38C920EE8E60AA6018A5612010B0A445B90062090499006260"));

			InsertDialogs(0x110, "Monster-in-a-box!"); // 0xC0

			List<IRewardSource> validChests = new();
			var chestMonsterList = new byte[0x100];
			var treasureList = Get(lut_TreasureOffset, 0x100);

			// Select treasures			
			var betterEquipmentList = ItemLists.UberTier.Concat(ItemLists.LegendaryArmorTier).Concat(ItemLists.LegendaryWeaponTier).Concat(ItemLists.RareArmorTier).Concat(ItemLists.RareWeaponTier);

			var RangeKeyItems = ItemLocations.AllTreasures.Where(x => !x.IsUnused && ItemLists.AllQuestItems.Contains((Item)treasureList[x.Address - lut_TreasureOffset]));
			var RangeShards = ItemLocations.AllTreasures.Where(x => !x.IsUnused && treasureList[x.Address - lut_TreasureOffset] == (int)Item.Shard);
			var RangeBetterTreasure = ItemLocations.AllTreasures.Where(x => !x.IsUnused && betterEquipmentList.Contains((Item)treasureList[x.Address - lut_TreasureOffset]));
			var RangeRandom = ItemLocations.AllTreasures.Where(x => !x.IsUnused && (!RangeKeyItems.Contains(x) || !RangeShards.Contains(x) || (flags.TCBetterTreasure == TCOptions.None && RangeBetterTreasure.Contains(x))));

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

			if (flags.TCRandom == TCRngOptions.Pooled)
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

			int maxChests = (flags.TCPoolSize == ChestsPool.Random) ? Rng.Between(rng, 20, 240) : (int)flags.TCPoolSize;

			// Get encounters
			const byte spookyZomBull = 0xB2;
			const byte spookyZombieD = 0xCB;
			const byte fightBahamut = 0xF1;

			List<byte> altEncountersList = Enumerable.Range(128, FirstBossEncounterIndex).Select(value => (byte)value).ToList();
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
				case FormationPool.Fiends:
					encounters = Enumerable.Range(0x77, 4).Select(value => (byte)value).ToList();
					break;
				case FormationPool.Phantom:
					encounters = new List<byte> { 0x46 };
					break;
			}

			int altFormationPosition = 0;

			byte GetEncounter() => (flags.TCFormations == FormationPool.AltFormationDist) ? encounters[altFormationPosition++] : encounters.PickRandom(rng);

			// Process pool first
			if (flags.TCPoolSize != ChestsPool.None)
			{
				maxChests = Math.Min(maxChests, validChests.Count);

				for (int i = 0; i < maxChests; i++)
				{
					chestMonsterList[(validChests.SpliceRandom(rng).Address - lut_TreasureOffset)] = GetEncounter();
				}
			}

			// Better Treasure
			if (flags.TCBetterTreasure == TCOptions.All)
			{
				for (int i = 0; i < 0x100; i++)
				{
					if (betterEquipmentList.Contains((Item)treasureList[i]))
						chestMonsterList[i] = GetEncounter();
				}
			}

			// Key Items
			if (flags.TCKeyItems == TCOptions.All)
			{
				for (int i = 0; i < 0x100; i++)
				{
					if (ItemLists.AllQuestItems.Contains((Item)treasureList[i]))
						chestMonsterList[i] = GetEncounter();
				}
			}

			// Shards
			if (flags.TCShards == TCOptions.All)
			{
				for (int i = 0; i < 0x100; i++)
				{
					if (treasureList[i] == (byte)Item.Shard)
						chestMonsterList[i] = GetEncounter();
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

				chestMonsterList[validChests.SpliceRandom(rng).Address - lut_TreasureOffset] = ChaosFormationIndex;
			}


			// Spoilers
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
						var tempTileProperties = GetFromBank(BANK_SMINFO, lut_TileSMsetProp + (i * 0x100) + (j * 2), 2);
						if ((tempTileProperties[0] & (byte)TilePropFunc.TP_SPEC_TREASURE) > 0 && (tempTileProperties[0] & (byte)TilePropFunc.TP_NOMOVE) > 0)
						{
							if (chestMonsterList[(int)tempTileProperties[1]] > 0)
							{
								TileSM temptile = new((byte)j, i, this);
								temptile.TileGraphic = new List<byte> { 0x2A, 0x7C, 0x3A, 0x3B };
								temptile.Write(this);
							}
						}
					}
				}
			}

			// Insert trapped chest list
			PutInBank(0x11, 0x8F00, chestMonsterList);
		}
		public void SetChaosForMIAB(NPCdata npcdata)
		{
			npcdata.SetRoutine((ObjectId)0x1A, newTalkRoutines.Talk_4Orb);
			npcdata.GetTalkArray((ObjectId)0x1A)[(int)TalkArrayPos.dialogue_1] = 0x30;
			npcdata.GetTalkArray((ObjectId)0x1A)[(int)TalkArrayPos.dialogue_2] = 0x30;
			npcdata.GetTalkArray((ObjectId)0x1A)[(int)TalkArrayPos.dialogue_3] = 0x30;
			Data[MapObjGfxOffset + 0x18] = 0xF4;
			Data[MapObjGfxOffset + 0x19] = 0xF4;
			Data[MapObjGfxOffset + 0x1A] = 0xF4;
			PutInBank(0x00, 0xA000 + ((byte)MapId.TempleOfFiendsRevisitedChaos * 0x30) + 0x18, Blob.FromHex("000F1636000F1636"));

			Dictionary<int, string> newgarlanddialogue = new Dictionary<int, string>();

			newgarlanddialogue.Add(0x2E, "That's right, it's me,\nBurtReynoldz. Didn't\nexpect to see me, right?");
			newgarlanddialogue.Add(0x2F, "Many moons ago I managed\nto run away from Chaos.\nAnd lo and behold,\nI BECAME Chaos. I took\nhis place.");
			newgarlanddialogue.Add(0x30, "Okay, I won't fight you.\nSome say you can find\nthe other Chaos hidden\nsomewhere in a chest.\nGood luck!");

			InsertDialogs(newgarlanddialogue);
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

			if (flags.IncentivizeBridge) incentivePool.Add(Item.Bridge);
			if (flags.IncentivizeShip ?? false) incentivePool.Add(Item.Ship);
			if (flags.IncentivizeCanoe ?? false) incentivePool.Add(Item.Canoe);
			if (flags.IncentivizeCanal ?? false) incentivePool.Add(Item.Canal);

			return incentivePool.ToList();
		}
	}
}
