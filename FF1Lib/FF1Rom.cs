using FF1Lib.Procgen;
using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FF1Lib.Assembly;
using System.Text.RegularExpressions;

namespace FF1Lib
{
	// ReSharper disable once InconsistentNaming
	public partial class FF1Rom : NesRom
	{
		public const int RngOffset = 0x7F100;
		public const int BattleRngOffset = 0x7FCF1;
		public const int RngSize = 256;

		public const int LevelRequirementsOffset = 0x6CC81;
		public const int LevelRequirementsSize = 3;
		public const int LevelRequirementsCount = 49;

		public const int StartingGoldOffset = 0x0301C;

		public const int GoldItemOffset = 108; // 108 items before gold chests
		public const int GoldItemCount = 68;
		public static List<int> UnusedGoldItems = new List<int> { 110, 111, 112, 113, 114, 116, 120, 121, 122, 124, 125, 127, 132, 158, 165, 166, 167, 168, 170, 171, 172 };

		public void PutInBank(int bank, int address, Blob data)
		{
			if (bank == 0x1F)
			{
				if ((address - 0xC000) + data.Length >= 0x4000)
				{
					throw new Exception("Data is too large to fit within its bank.");
				}
				int offset = (bank * 0x4000) + (address - 0xC000);
				this.Put(offset, data);
			}
			else
			{
				if ((address - 0x8000) + data.Length >= 0x4000)
				{
					throw new Exception("Data is too large to fit within its bank.");
				}
				int offset = (bank * 0x4000) + (address - 0x8000);
				this.Put(offset, data);
			}

		}
		public Blob GetFromBank(int bank, int address, int length)
		{
			if (bank == 0x1F)
			{
				if ((address - 0xC000) + length >= 0x4000)
				{
					throw new Exception("Data is too large to fit within one bank.");
				}
				int offset = (bank * 0x4000) + (address - 0xC000);
				return this.Get(offset, length);
			}
			else
			{
				if ((address - 0x8000) + length >= 0x4000)
				{
					throw new Exception("Data is too large to fit within one bank.");
				}
				int offset = (bank * 0x4000) + (address - 0x8000);
				return this.Get(offset, length);
			}
		}

		private Blob CreateLongJumpTableEntry(byte bank, ushort addr)
		{
			List<byte> tmp = new List<byte> { 0x20, 0xC8, 0xD7 }; // JSR $D7C8, beginning of each table entry

			var addrBytes = BitConverter.GetBytes(addr); // next add the address to jump to
			tmp.Add(addrBytes[0]);
			tmp.Add(addrBytes[1]);
			tmp.Add(bank); //finally, add the bank that the routine is located in

			return tmp.ToArray();
		}

		public FF1Rom(string filename) : base(filename)
		{ }

		public FF1Rom(Stream readStream) : base(readStream)
		{ }

		private FF1Rom()
		{ }

		public static async Task<FF1Rom> CreateAsync(Stream readStream)
		{
			var rom = new FF1Rom();
			await rom.LoadAsync(readStream);

			return rom;
		}

		public void Randomize(Blob seed, Flags flags, Preferences preferences)
		{
			MT19337 rng;
			using (SHA256 hasher = SHA256.Create())
			{
				Blob FlagsBlob = Encoding.UTF8.GetBytes(Flags.EncodeFlagsText(flags));
				Blob SeedAndFlags = Blob.Concat(new Blob[] { FlagsBlob, seed });
				Blob hash = hasher.ComputeHash(SeedAndFlags);
				rng = new MT19337(BitConverter.ToUInt32(hash, 0));
			}
			if (flags.TournamentSafe) AssureSafe();

			UpgradeToMMC3();
			MakeSpace();
			Bank1E();
			Bank1B();
			EasterEggs();
			DynamicWindowColor(preferences.MenuColor);
			PermanentCaravan();
			ShiftEarthOrbDown();
			CastableItemTargeting();
			FixEnemyPalettes(); // fixes a bug in the original game's programming that causes third enemy slot's palette to render incorrectly
			FixWarpBug(); // The warp bug must be fixed for magic level shuffle and spellcrafter
			ExpandNormalTeleporters();
			SeparateUnrunnables();
			var talkroutines = new TalkRoutines();
			var npcdata = new NPCdata(this);
			UpdateDialogs(npcdata, flags);

			if (flags.TournamentSafe) Put(0x3FFE3, Blob.FromHex("66696E616C2066616E74617379"));

			flags = Flags.ConvertAllTriState(flags, rng);

			TeleportShuffle teleporters = new TeleportShuffle();
			var palettes = OverworldMap.GeneratePalettes(Get(OverworldMap.MapPaletteOffset, MapCount * OverworldMap.MapPaletteSize).Chunk(OverworldMap.MapPaletteSize));
			var overworldMap = new OverworldMap(this, flags, palettes, teleporters);

			var owMapExchange = OwMapExchange.FromFlags(this, overworldMap, flags, rng);
			owMapExchange?.ExecuteStep1();

			var shipLocations = owMapExchange?.ShipLocations ?? OwMapExchange.GetDefaultShipLocations(this);

			var maps = ReadMaps();
			var shopItemLocation = ItemLocations.CaravanItemShop1;
			var oldItemNames = ReadText(ItemTextPointerOffset, ItemTextPointerBase, ItemTextPointerCount);

			if ((bool)flags.NPCItems || (bool)flags.NPCFetchItems)
			{
				NPCShuffleDialogs();
			}

			if (flags.EFGWaterfall || flags.EFGEarth1 || flags.EFGEarth2)
			{
				MapRequirements reqs;
				MapGeneratorStrategy strategy;
				MapGenerator generator = new MapGenerator();
				if (flags.EFGWaterfall)
				{
					reqs = new MapRequirements
					{
						MapId = MapId.Waterfall,
						Rom = this,
					};
					strategy = MapGeneratorStrategy.WaterfallClone;
					CompleteMap waterfall = generator.Generate(rng, strategy, reqs);

					// Should add more into the reqs so that this can be done inside the generator.
					teleporters.Waterfall.SetEntrance(waterfall.Entrance);
					overworldMap.PutOverworldTeleport(OverworldTeleportIndex.Waterfall, teleporters.Waterfall);
					maps[(int)MapId.Waterfall] = waterfall.Map;
				}

				if (flags.EFGEarth1)
				{
					reqs = new MapRequirements
					{
						MapId = MapId.EarthCaveB1,
						Rom = this,
					};

					strategy = MapGeneratorStrategy.Square;
					var earthB1 = generator.Generate(rng, strategy, reqs);

					// Should add more into the reqs so that this can be done inside the generator.
					teleporters.EarthCave1.SetEntrance(earthB1.Entrance);
					overworldMap.PutOverworldTeleport(OverworldTeleportIndex.EarthCave1, teleporters.EarthCave1);
					maps[(int)MapId.EarthCaveB1] = earthB1.Map;
				}
				if (flags.EFGEarth2)
				{
					reqs = new MapRequirements
					{
						MapId = MapId.EarthCaveB2,
						Rom = this,
					};

					strategy = MapGeneratorStrategy.Square;
					var earthB2 = generator.Generate(rng, strategy, reqs);

					// Should add more into the reqs so that this can be done inside the generator.
					teleporters.EarthCave2.SetEntrance(earthB2.Entrance);
					overworldMap.PutStandardTeleport(TeleportIndex.EarthCave2, teleporters.EarthCave2, OverworldTeleportIndex.EarthCave1);
					maps[(int)MapId.EarthCaveB2] = earthB2.Map;
				}
			}

			var flippedMaps = new List<MapId>();

			if ((bool)flags.FlipDungeons)
			{
				flippedMaps = HorizontalFlipDungeons(rng, maps, teleporters, overworldMap);
			}

			if ((bool)flags.RandomizeFormationEnemizer)
			{
				DoEnemizer(rng, (bool)flags.RandomizeEnemizer, (bool)flags.RandomizeFormationEnemizer, flags.EnemizerDontMakeNewScripts);
			}

			if (flags.DeepDungeon)
			{
				DeepDungeon(rng, overworldMap, maps, flags);
				UnusedGoldItems = new List<int> { };
			}

			if (preferences.ModernBattlefield)
			{
				EnableModernBattlefield();
			}

			if ((bool)flags.TitansTrove)
			{
				EnableTitansTrove(maps);
			}

			if ((bool)flags.LefeinShops)
			{
				EnableLefeinShops(maps);
			}

            if ((bool)flags.MelmondClinic)
            {
                EnableMelmondClinic(maps);
            }

			if ((bool)flags.RandomVampAttack)
			{
				RandomVampireAttack(maps, (bool)flags.LefeinShops, (bool)flags.RandomVampAttackIncludesConeria, rng);
			}

			if ((bool)flags.GaiaShortcut)
			{
				EnableGaiaShortcut(maps);
				if ((bool)flags.MoveGaiaItemShop)
				{
					MoveGaiaItemShop(maps, rng);
				}
			}

			if ((bool)flags.LefeinSuperStore && (flags.ShopKillMode_White == ShopKillMode.None && flags.ShopKillMode_Black == ShopKillMode.None))
			{
				EnableLefeinSuperStore(maps);
			}

			// This has to be done before we shuffle spell levels.
			if (flags.SpellBugs)
			{
				FixSpellBugs();
			}

			//must be done before spells get shuffled around otherwise we'd be changing a spell that isnt lock
			if (flags.LockMode != LockHitMode.Vanilla)
			{
				ChangeLockMode(flags.LockMode);
			}

			if (flags.EnemySpellsTargetingAllies)
			{
				FixEnemyAOESpells();
			}

			if (flags.AllSpellLevelsForKnightNinja)
			{
				KnightNinjaChargesForAllLevels();
			}

			if (flags.BuffHealingSpells)
			{
				BuffHealingSpells();
			}

			UpdateMagicAutohitThreshold(rng, flags.MagicAutohitThreshold);

			if ((bool)flags.GenerateNewSpellbook)
			{
				CraftNewSpellbook(rng, (bool)flags.SpellcrafterMixSpells, flags.LockMode, (bool)flags.MagicLevels, (bool)flags.SpellcrafterRetainPermissions);
			}

			if ((bool)flags.Weaponizer) {
			    Weaponizer(rng, (bool)flags.WeaponizerNamesUseQualityOnly, (bool)flags.WeaponizerCommonWeaponsHavePowers);
			}

			if ((bool)flags.MagisizeWeapons)
			{
				MagisizeWeapons(rng, (bool)flags.MagisizeWeaponsBalanced);
			}

			if ((bool)flags.ItemMagic)
			{
				ShuffleItemMagic(rng, (bool)flags.BalancedItemMagicShuffle);
			}

			if ((bool)flags.GuaranteedRuseItem)
			{
				CraftRuseItem();
			}

			if ((bool)flags.ShortToFR && !flags.DeepDungeon)
			{
				ShortenToFR(maps, (bool)flags.PreserveFiendRefights, (bool)flags.PreserveAllFiendRefights, (bool)flags.ExitToFR, (bool)flags.LutePlateInShortToFR, rng);
			}

			if ((bool)flags.ExitToFR && !flags.DeepDungeon)
			{
				EnableToFRExit(maps);
			}

			if (((bool)flags.Treasures) && flags.ShardHunt && !flags.FreeOrbs && !flags.DeepDungeon)
			{
				EnableShardHunt(rng, talkroutines, flags.ShardCount);
			}

			if (flags.TransformFinalFormation != FinalFormation.None && !flags.SpookyFlag)
			{
				TransformFinalFormation(flags.TransformFinalFormation, flags.EvadeCap, rng);
			}

			if ((bool)flags.EarlyOrdeals)
			{
				EnableEarlyOrdeals();
			}

			if ((bool)flags.OrdealsPillars)
			{
				ShuffleOrdeals(rng, maps);
			}

			if (flags.SkyCastle4FMazeMode == SkyCastle4FMazeMode.Maze)
			{
				DoSkyCastle4FMaze(rng, maps);
			}
			else if (flags.SkyCastle4FMazeMode == SkyCastle4FMazeMode.Teleporters)
			{
				ShuffleSkyCastle4F(rng, maps);
			}

			if ((bool)flags.EarlyKing)
			{
				EnableEarlyKing(npcdata);
			}

			if ((bool)flags.EarlySarda)
			{
				EnableEarlySarda(npcdata);
			}

			if ((bool)flags.EarlySage)
			{
				EnableEarlySage(npcdata);
			}

			if (flags.ChaosRush)
			{
				EnableChaosRush();
			}

			if ((bool)flags.FreeBridge)
			{
				EnableFreeBridge();
			}

			if ((bool)flags.IsAirshipFree)
			{
				EnableFreeAirship();
			}

			if ((bool)flags.IsShipFree)
			{
				EnableFreeShip();
			}

			if (flags.FreeOrbs)
			{
				EnableFreeOrbs();
			}

			if ((bool)flags.IsCanalFree)
			{
				EnableFreeCanal((bool)flags.NPCItems, npcdata);
			}

			if ((bool)flags.FreeCanoe)
			{
				EnableFreeCanoe();
			}

			if ((bool)flags.FreeLute)
			{
				EnableFreeLute();
			}

			if ((bool)flags.FreeTail && !(bool)flags.NoTail)
			{
				EnableFreeTail();
			}

			if ((bool)flags.MapHallOfDragons) {
			    BahamutB1Encounters(maps);
			}

			DragonsHoard(maps, (bool)flags.MapDragonsHoard);

			var shopData = new ShopData(this);
			shopData.LoadData();

			var extConsumables = new ExtConsumables(this, flags, rng, shopData);
			extConsumables.AddNormalShopEntries();

			overworldMap.ApplyMapEdits();

			var maxRetries = 8;
			for (var i = 0; i < maxRetries; i++)
			{
				try
				{
					overworldMap = new OverworldMap(this, flags, palettes, teleporters);
					if (((bool)flags.Entrances || (bool)flags.Floors || (bool)flags.Towns) && ((bool)flags.Treasures) && ((bool)flags.NPCItems) && !flags.DeepDungeon)
					{
						overworldMap.ShuffleEntrancesAndFloors(rng, flags);

						// Disable the Princess Warp back to Castle Coneria
						if ((bool)flags.Entrances || (bool)flags.Floors)
							talkroutines.ReplaceChunk(newTalkRoutines.Talk_Princess1, Blob.FromHex("20CC90"), Blob.FromHex("EAEAEA"));
					}

					if ((bool)flags.Treasures && (bool)flags.ShuffleObjectiveNPCs && !flags.DeepDungeon)
					{
						overworldMap.ShuffleObjectiveNPCs(rng);
					}


					ISanityChecker checker = new SanityCheckerV1();
					IncentiveData incentivesData = new IncentiveData(rng, flags, overworldMap, shopItemLocation, checker);

					if (((bool)flags.Shops))
					{
						var excludeItemsFromRandomShops = new List<Item>();
						if ((bool)flags.Treasures)
						{
							excludeItemsFromRandomShops = incentivesData.ForcedItemPlacements.Select(x => x.Item).Concat(incentivesData.IncentiveItems).ToList();
						}

						if (!((bool)flags.RandomWaresIncludesSpecialGear))
						{
							excludeItemsFromRandomShops.AddRange(ItemLists.SpecialGear);
							if ((bool)flags.GuaranteedRuseItem)
								excludeItemsFromRandomShops.Add(Item.PowerRod);
						}

						if ((bool)flags.NoMasamune)
						{
							excludeItemsFromRandomShops.Add(Item.Masamune);
						}

						if ((bool)flags.NoXcalber)
						{
							excludeItemsFromRandomShops.Add(Item.Xcalber);
						}

						shopItemLocation = ShuffleShops(rng, (bool)flags.ImmediatePureAndSoftRequired, ((bool)flags.RandomWares), excludeItemsFromRandomShops, flags.WorldWealth, overworldMap.ConeriaTownEntranceItemShopIndex);

						incentivesData = new IncentiveData(rng, flags, overworldMap, shopItemLocation, checker);
					}

					if ((bool)flags.Treasures && !flags.DeepDungeon)
					{
						if(flags.SanityCheckerV2) checker = new SanityCheckerV2(maps, overworldMap, npcdata, this, shopItemLocation, shipLocations);
						generatedPlacement = ShuffleTreasures(rng, flags, incentivesData, shopItemLocation, overworldMap, teleporters, checker);
					}

					break;
				}
				catch (InsaneException e)
				{
					Console.WriteLine(e.Message);
					if (maxRetries > (i + 1)) continue;
					throw new InvalidOperationException(e.Message);
				}
			}

			// Change Astos routine so item isn't lost in wall of text
			if ((bool)flags.NPCItems || (bool)flags.NPCFetchItems || (bool)flags.ShuffleAstos)
				talkroutines.Replace(newTalkRoutines.Talk_Astos, Blob.FromHex("A674F005BD2060F027A57385612080B1B020A572203D96A5752000B1A476207F90207392A5611820109F201896A9F060A57060"));

			npcdata.UpdateItemPlacement(generatedPlacement);

			if (flags.NoOverworld && (bool)!flags.Entrances && (bool)!flags.Floors && (bool)!flags.Towns)
			{
				NoOverworld(overworldMap, maps, talkroutines, npcdata, flippedMaps, flags, rng);
			}

			if ((bool)flags.AlternateFiends && !flags.SpookyFlag)
			{
				AlternativeFiends(rng);
			}
			if ((bool)flags.MagicShopLocs)
			{
				ShuffleMagicLocations(rng, (bool)flags.MagicShopLocationPairs);
			}

			if (((bool)flags.MagicShops))
			{
				ShuffleMagicShops(rng);
			}

			if (((bool)flags.MagicLevels))
			{
				ShuffleMagicLevels(rng, ((bool)flags.MagicPermissions), (bool)flags.MagicLevelsTiered, (bool)flags.MagicLevelsMixed, (bool)!flags.GenerateNewSpellbook);
			}

			new StartingInventory(rng, flags, this).SetStartingInventory();
			new StartingEquipment(rng, flags, this).SetStartingEquipment();

			new ShopKiller(rng, flags, maps, this).KillShops();

			shopData.LoadData();

			new LegendaryShops(rng, flags, maps, flippedMaps, shopData, this).PlaceShops();

			if (flags.DeepDungeon)
			{
				shopData.Shops.Find(x => x.Type == FF1Lib.ShopType.Item && x.Entries.Contains(Item.Bottle)).Entries.Remove(Item.Bottle);
				shopData.StoreData();
			}		

			//has to be done before modifying itemnames and after modifying spellnames...
			extConsumables.LoadSpells();

			if (preferences.AccessibleSpellNames)
			{
				AccessibleSpellNames(flags);
			}

			if (flags.SpellNameMadness != SpellNameMadness.None)
			{
				MixUpSpellNames(flags.SpellNameMadness, rng);
			}

			/*
			if (flags.WeaponPermissions)
			{
				ShuffleWeaponPermissions(rng);
			}

			if (flags.ArmorPermissions)
			{
				ShuffleArmorPermissions(rng);
			}
			*/

			if (flags.SaveGameWhenGameOver)
			{
				EnableSaveOnDeath(flags);
			}

			// Ordered before RNG shuffle. In the event that both flags are on, RNG shuffle depends on this.
			if (((bool)flags.FixMissingBattleRngEntry))
			{
				FixMissingBattleRngEntry();
			}

			if (((bool)flags.Rng))
			{
				ShuffleRng(rng);
			}

			if (((bool)flags.EnemyScripts))
			{
				ShuffleEnemyScripts(rng, (bool)flags.AllowUnsafePirates, (bool)!flags.BossScriptsOnly, (bool)!flags.NoBossSkillScriptShuffle, ((bool)flags.EnemySkillsSpellsTiered || (bool)flags.ScaryImps), (bool)flags.ScaryImps);
			}

			if (((bool)flags.EnemySkillsSpells))
			{
				if ((bool)flags.EnemySkillsSpellsTiered && (bool)!flags.BossSkillsOnly)
				{
					GenerateBalancedEnemyScripts(rng, (bool)flags.SwolePirates);
					ShuffleEnemySkillsSpells(rng, false, (bool)!flags.NoBossSkillScriptShuffle);
				}
				else
				{
					ShuffleEnemySkillsSpells(rng, (bool)!flags.BossSkillsOnly, (bool)!flags.NoBossSkillScriptShuffle);
				}
			}

			if (((bool)flags.EnemyStatusAttacks))
			{
				if (((bool)flags.RandomStatusAttacks))
				{
					RandomEnemyStatusAttacks(rng, (bool)flags.AllowUnsafePirates, (bool)flags.DisableStunTouch);
				}
				else
				{
					ShuffleEnemyStatusAttacks(rng, (bool)flags.AllowUnsafePirates);
				}
			}

			if (flags.Runnability == Runnability.Random)
				flags.Runnability = (Runnability)Rng.Between(rng, 0, 3);

			if (flags.Runnability == Runnability.AllRunnable)
				CompletelyRunnable();
			else if (flags.Runnability == Runnability.AllUnrunnable)
				CompletelyUnrunnable();
			else if (flags.Runnability == Runnability.Shuffle)
				ShuffleUnrunnable(rng);

			// Always on to supply the correct changes for WaitWhenUnrunnable
			AllowStrikeFirstAndSurprise(flags.WaitWhenUnrunnable, (bool)flags.UnrunnablesStrikeFirstAndSurprise);

			if (((bool)flags.EnemyFormationsSurprise))
			{
				ShuffleSurpriseBonus(rng);
			}

			// Put this before other encounter / trap tile edits.
			if ((bool)flags.AllowUnsafeMelmond)
			{
				EnableMelmondGhetto(flags.EnemizerEnabled);
			}

			// After unrunnable shuffle and before formation shuffle. Perfect!
			if (flags.WarMECHMode != WarMECHMode.Vanilla)
			{
				WarMECHNpc(flags.WarMECHMode, npcdata, rng, maps);
			}

			if (flags.WarMECHMode == WarMECHMode.Unleashed)
			{
				UnleashWarMECH();
			}

			if ((bool)flags.ClassAsNpcFiends || (bool)flags.ClassAsNpcKeyNPC)
			{
				ClassAsNPC(flags, talkroutines, npcdata, flippedMaps, rng);
			}

			if ((bool)flags.FiendShuffle)
			{
				FiendShuffle(rng);
			}

			if (flags.FormationShuffleMode != FormationShuffleMode.None && !flags.EnemizerEnabled)
			{
				ShuffleEnemyFormations(rng, flags.FormationShuffleMode);
			}

			if ((bool)flags.RemoveTrapTiles)
			{
				RemoveTrapTiles(flags.EnemizerEnabled);
			}

			if (((bool)flags.EnemyTrapTiles) && !flags.EnemizerEnabled)
			{
				ShuffleTrapTiles(rng, (bool)flags.RandomTrapFormations, (bool)flags.FightBahamut);
			}

			if ((bool)flags.ConfusedOldMen)
			{
				EnableConfusedOldMen(rng);
			}

			if (flags.NoPartyShuffle)
			{
				DisablePartyShuffle();
			}

			if (flags.SpeedHacks)
			{
				EnableSpeedHacks();
			}

			if (flags.IdentifyTreasures)
			{
				EnableIdentifyTreasures();
			}

			if (flags.Dash || flags.SpeedBoat)
			{
				EnableDash(flags.SpeedBoat);
			}

			if (flags.BuyTen)
			{
				EnableBuyQuantity();
			}

			if (flags.WaitWhenUnrunnable)
			{
				ChangeUnrunnableRunToWait();
			}

			if (flags.SpeedHacks && flags.EnableCritNumberDisplay)
			{
				EnableCritNumberDisplay();
			}

			if (flags.BattleMagicMenuWrapAround)
			{
				BattleMagicMenuWrapAround();
			}

			if (flags.NPCSwatter)
			{
				EnableNPCSwatter(npcdata);
			}

			if (flags.EasyMode)
			{
				EnableEasyMode();
			}

			new TreasureStacks(this, flags).SetTreasureStacks();
			new StartingLevels(this, flags).SetStartingLevels();

			if (flags.MaxLevelLow < 50)
			{
				SetMaxLevel(flags, rng);
			}

			if ((bool)flags.TrappedChestsEnabled)
			{
				MonsterInABox(rng, flags);

				if((bool)flags.TrappedChaos)
				{
					SetChaosForMIAB(npcdata);
				}
			}

			if (!flags.Etherizer && (flags.HouseMPRestoration || flags.HousesFillHp))
			{
				FixHouse(flags.HouseMPRestoration, flags.HousesFillHp);
			}

			if (flags.BBCritRate)
			{
				DontDoubleBBCritRates();
			}

			if (flags.WeaponCritRate)
			{
				DoubleWeaponCritRates();
			}

			//needs to go after item magic, moved after double weapon crit to have more control over the actual number of crit gained.
			if ((bool)flags.RandomWeaponBonus)
			{
				RandomWeaponBonus(rng, flags.RandomWeaponBonusLow, flags.RandomWeaponBonusHigh, (bool)flags.RandomWeaponBonusExcludeMasa);
			}

			if ((bool)flags.RandomArmorBonus)
			{
				RandomArmorBonus(rng, flags.RandomArmorBonusLow, flags.RandomArmorBonusHigh);
			}

			if (flags.WeaponBonuses)
			{
				IncreaseWeaponBonus(flags.WeaponTypeBonusValue);
			}

			if (flags.WeaponStats)
			{
				FixWeaponStats();
			}

			new ChanceToRun(this, flags).FixChanceToRun();

			if (flags.EnemyStatusAttackBug)
			{
				FixEnemyStatusAttackBug();
			}

			if (flags.BlackBeltAbsorb)
			{
				FixBBAbsorbBug();
			}

			if (flags.MDefMode != MDEFGrowthMode.None)
			{
				MDefChanges(flags.MDefMode);
			}

			if (flags.ThiefHitRate)
			{
				ThiefHitRate();
			}

			if (flags.ThiefAgilityBuff != ThiefAGI.Vanilla)
			{
			        BuffThiefAGI(flags.ThiefAgilityBuff);
			}

			if (flags.ImproveTurnOrderRandomization)
			{
				ImproveTurnOrderRandomization(rng);
			}

			if (flags.FixHitChanceCap)
			{
				FixHitChanceCap();
			}

			if (flags.EnemyElementalResistancesBug)
			{
				FixEnemyElementalResistances();
			}

			if (preferences.FunEnemyNames && !flags.EnemizerEnabled)
			{
				FunEnemyNames(preferences.TeamSteak);
			}

			var itemText = ReadText(ItemTextPointerOffset, ItemTextPointerBase, ItemTextPointerCount);
			itemText[(int)Item.Ribbon] = itemText[(int)Item.Ribbon].Remove(7);

			if (flags.Etherizer)
			{
				Etherizer();
				itemText[(int)Item.Tent] = "ETHR@p";
				itemText[(int)Item.Cabin] = "DRY@p ";
				itemText[(int)Item.House] = "XETH@p";
			}

			if (flags.ExtensiveHints_Enable)
			{
				new ExtensiveHints(rng, npcdata, flags, overworldMap, this).Generate();
			}
			else if ((bool)flags.HintsVillage || (bool)flags.HintsDungeon)
			{
				if ((bool)flags.HintsDungeon)
					SetDungeonNPC(flippedMaps, rng);

				NPCHints(rng, npcdata, flags, overworldMap);
			}

			ExpGoldBoost(flags);
			ScalePrices(flags, itemText, rng, ((bool)flags.ClampMinimumPriceScale), shopItemLocation);
			ScaleEncounterRate(flags.EncounterRate / 30.0, flags.DungeonEncounterRate / 30.0);

			WriteMaps(maps);

			WriteText(itemText, ItemTextPointerOffset, ItemTextPointerBase, ItemTextOffset, UnusedGoldItems);

			extConsumables.AddExtConsumables();

			if ((bool)flags.SwolePirates)
			{
				EnableSwolePirates();
			}

			if (flags.EnemyScaleStatsHigh != 100 || flags.EnemyScaleStatsLow != 100 || ((bool)flags.SeparateEnemyHPScaling && (flags.EnemyScaleHpLow != 100 || flags.EnemyScaleHpHigh != 100)))
			{
				ScaleEnemyStats(rng, flags);
			}

			if (flags.BossScaleStatsHigh != 100 || flags.BossScaleStatsLow != 100 || ((bool)flags.SeparateBossHPScaling && (flags.BossScaleHpLow != 100 || flags.BossScaleHpHigh != 100)))
			{
				ScaleBossStats(rng, flags);
			}

			PartyComposition(rng, flags, preferences);

			if (((bool)flags.RecruitmentMode))
			{
				PubReplaceClinic(rng, flags);
			}

			if ((bool)flags.ChangeMaxMP)
			{
				SetMPMax(flags.RedMageMaxMP, flags.WhiteMageMaxMP, flags.BlackMageMaxMP, flags.KnightMaxMP, flags.NinjaMaxMP);
			}

			if ((bool)flags.ShuffleAstos)
			{
				ShuffleAstos(flags, npcdata, talkroutines, rng);
			}

			if ((bool)flags.EnablePoolParty)
			{
				EnablePoolParty(flags, rng);
			}

			if ((bool)flags.MapCanalBridge)
			{
				EnableCanalBridge();
			}

			if (flags.NonesGainXP || flags.DeadsGainXP)
			{
				XpAdmissibility((bool)flags.NonesGainXP, flags.DeadsGainXP);
			}

			SetProgressiveScaleMode(flags);

			if ((bool)flags.RandomizeClass)
			{
				RandomizeClass(rng, flags, oldItemNames);
			}

			if ((bool)flags.EnableRandomPromotions)
			{
				EnableRandomPromotions(flags, rng);
			}

			if (flags.DisableTentSaving)
			{
				CannotSaveOnOverworld();
			}

			if (flags.DisableInnSaving)
			{
				CannotSaveAtInns();
			}

			if (flags.PacifistMode && !flags.SpookyFlag)
			{
				PacifistEnd(talkroutines, npcdata, (bool)flags.EnemyTrapTiles || flags.EnemizerEnabled);
			}

			if (flags.ShopInfo)
			{
				ShopUpgrade(flags);
			}

			Fix3DigitStats();

			if ((bool)flags.FightBahamut && !flags.SpookyFlag && !(bool)flags.RandomizeFormationEnemizer)
			{
				FightBahamut(talkroutines, npcdata, (bool)flags.NoTail, (bool)flags.SwoleBahamut, flags.EvadeCap, rng);
			}

			if (flags.SpookyFlag && !(bool)flags.RandomizeFormationEnemizer)
			{
				Spooky(talkroutines, npcdata, rng, flags);
			}

			if (flags.InventoryAutosort && !(preferences.RenounceAutosort))
			{
				EnableInventoryAutosort();
			}

			if (flags.SkyWarriorSpoilerBats != SpoilerBatHints.Vanilla) {
			    SkyWarriorSpoilerBats(rng, flags, npcdata);
			}

			ObfuscateEnemies(rng, flags);

			// We have to do "fun" stuff last because it alters the RNG state.
			// Back up Rng so that fun flags are uniform when different ones are selected
			uint funRngSeed = rng.Next();

			RollCredits(rng);

			if (preferences.DisableDamageTileFlicker || flags.TournamentSafe)
			{
				DisableDamageTileFlicker();
			}

			if (preferences.ThirdBattlePalette)
			{
				UseVariablePaletteForCursorAndStone();
			}

			if (preferences.PaletteSwap && !flags.EnemizerEnabled && flags.EnemyObfuscation == EnemyObfuscation.None)
			{
				rng = new MT19337(funRngSeed);
				PaletteSwap(rng);
			}

			if (preferences.TeamSteak && !(bool)flags.RandomizeEnemizer && flags.EnemyObfuscation == EnemyObfuscation.None)
			{
				TeamSteak();
			}

			if (preferences.ChangeLute)
			{
				rng = new MT19337(funRngSeed);
				ChangeLute(rng);
			}

			rng = new MT19337(funRngSeed);

			TitanSnack(preferences.TitanSnack, npcdata, rng);

			rng = new MT19337(funRngSeed);

			HurrayDwarfFate(preferences.HurrayDwarfFate, npcdata, rng);

			if (preferences.Music != MusicShuffle.None)
			{
				rng = new MT19337(funRngSeed);
				ShuffleMusic(preferences.Music, rng);
			}

			if (preferences.DisableSpellCastFlash || flags.TournamentSafe)
			{
				DisableSpellCastScreenFlash();
			}

			if (preferences.SpriteSheet != null) {
			    using (var stream = new MemoryStream(Convert.FromBase64String(preferences.SpriteSheet)))
			    {
				SetCustomPlayerSprites(stream, preferences.ThirdBattlePalette);
			    }
			}

			owMapExchange?.ExecuteStep2();

			npcdata.WriteNPCdata(this);
			talkroutines.WriteRoutines(this);
			talkroutines.UpdateNPCRoutines(this, npcdata);

			WriteSeedAndFlags(seed.ToHex(), Flags.EncodeFlagsText(flags));
			ExtraTrackingAndInitCode(flags);
		}

		private void EnableNPCSwatter(NPCdata npcdata)
		{
			for (int i = 0; i < npcdata.GetNPCCount(); i++)
			{
				if (npcdata.GetRoutine((ObjectId)i) == newTalkRoutines.Talk_norm)
					npcdata.SetRoutine((ObjectId)i, newTalkRoutines.Talk_kill);
			}

			// Protect Lute and Rod Plate
			npcdata.SetRoutine(ObjectId.LutePlate, newTalkRoutines.Talk_norm);
			npcdata.SetRoutine(ObjectId.RodPlate, newTalkRoutines.Talk_norm);
		}

		public void AssureSafe()
		{
			using (SHA256 hasher = SHA256.Create())
			{
				byte[] hashable = Data.ToBytes();


				//zero out mapman palette data
				for (int i = 0x390; i < 0x3BC; i++)
				{
					hashable[i] = 0;
				}

				//zero out character mapman graphics
				for (int i = 0x9000; i < 0xA200; i++)
				{
					hashable[i] = 0;
				}
				//zero out character battle graphics
				for (int i = 0x25000; i < 0x26800; i++)
				{
					hashable[i] = 0;
				}

				// lut_InBattleCharPaletteAssign (LUT for assigning palettes to in-battle char sprites)
				for (int i = 0x3203C; i < 0x32048; i++)
				{
					hashable[i] = 0;
				}

				// BattleSpritePalettes (palette for battle sprites)
				for (int i = 0x3EBA4; i < 0x3EBB5; i++)
				{
					hashable[i] = 0;
				}

				// lutClassBatSprPalette (LUT for battle sprite palettes)
				for (int i = 0x3ECA4; i < 0x3ECB0; i++)
				{
					hashable[i] = 0;
				}

				var Hash = hasher.ComputeHash(hashable);
				if (ByteArrayToString(Hash) != "7ea7f20bcb93b9d3c5f951f59b9cc3a42b347dbf0323b98d74b06bc81309d77a")
				{
					Console.WriteLine($"Rom hash: {ByteArrayToString(Hash)}");
					throw new TournamentSafeException("File has been modified");
				}
			}
		}

		public class TournamentSafeException : Exception
		{
			public TournamentSafeException(string message)
				: base(message) { }
		}

		private static string ByteArrayToString(byte[] ba)
		{
			StringBuilder hex = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba)
				hex.AppendFormat("{0:x2}", b);
			return hex.ToString();
		}

		private void ExtraTrackingAndInitCode(Flags flags)
		{
			// Expanded game init code, does several things:
			//	- Encounter table emu/hardware fix
			//	- track hard/soft resets
			//	- initialize tracking variables if no game is saved
			PutInBank(0x0F, 0x8000, Blob.FromHex("A9008D00208D012085FEA90885FF85FDA51BC901D00160A901851BA94DC5F9F008A9FF85F585F685F7182088C8B049A94DC5F918F013ADA36469018DA364ADA46469008DA464189010ADA56469018DA564ADA66469008DA664A9008DFD64A200187D00647D00657D00667D0067E8D0F149FF8DFD64189010A2A0A9009D00609D0064E8D0F7EEFB64ADFB648DFB6060"));
			Put(0x7C012, Blob.FromHex("A90F2003FE200080EAEAEAEAEAEAEAEA"));


			// Move controller handling out of bank 1F
			// This bit of code is also altered to allow a hard reset using Up+A on controller 2
			PutInBank(0x0F, 0x8200, Blob.FromHex("20108220008360"));
			PutInBank(0x0F, 0x8210, Blob.FromHex("A9018D1640A9008D1640A208AD16402903C9012620AD17402903C901261ECAD0EBA51EC988F008C948F001604C2EFE20A8FE20A8FE20A8FEA2FF9AA900851E9500CAD0FBA6004C12C0"));
			PutInBank(0x0F, 0x8300, Blob.FromHex("A5202903F002A2038611A520290CF0058A090C8511A52045212511452185214520AA2910F00EA5202910F002E623A521491085218A2920F00EA5202920F002E622A521492085218A2940F00EA5202940F002E625A521494085218A2980F00EA5202980F002E624A5214980852160"));
			PutInBank(0x1F, 0xD7C2, CreateLongJumpTableEntry(0x0F, 0x8200));

			// Battles use 2 separate and independent controller handlers for a total of 3 (because why not), so we patch these to respond to Up+A also
			PutInBank(0x0F, 0x8580, Blob.FromHex("A0018C1640888C1640A008AD16404AB0014A6EB368AD17402903C901261E88D0EAA51EC988F00BC948F004ADB368604C2EFE20A8FE20A8FE20A8FEA2FF9AA900851E9500CAD0FBA6004C12C0"));
			PutInBank(0x1F, 0xD828, CreateLongJumpTableEntry(0x0F, 0x8580));
			// PutInBank(0x0B, 0x9A06, Blob.FromHex("4C28D8")); Included in bank 1B changes
			PutInBank(0x0C, 0x97C7, Blob.FromHex("2027F22028D82029ABADB36860"));


			// Put LongJump routine 6 bytes after UpdateJoy used to be
			PutInBank(0x1F, 0xD7C8, Blob.FromHex("85E99885EA6885EB6885ECA001B1EB85EDC8B1EB85EEC8ADFC6085E8B1EB2003FEA9D748A9F548A5E9A4EA6CED0085E9A5E82003FEA5E960"));
			// LongJump entries can start at 0xD800 and must stop before 0xD850 (at which point additional space will need to be freed to make room)

			// Patches for various tracking variables follow:
			// Pedometer + chests opened
			PutInBank(0x0F, 0x8100, Blob.FromHex("18A532D027A52D2901F006A550D01DF00398D018ADA06069018DA060ADA16069008DA160ADA26069008DA260A52F8530A9FF8518A200A000BD00622904F001C8E8D0F5988DB7606060"));
			Put(0x7D023, Blob.FromHex("A90F2003FE200081"));
			// Count number of battles
			PutInBank(0x0F, 0x8400, Blob.FromHex("18ADA76069018DA7609003EEA86020A8FE60"));
			PutInBank(0x1F, 0xD800, CreateLongJumpTableEntry(0x0F, 0x8400));
			PutInBank(0x1F, 0xF28D, Blob.FromHex("2000D8"));
			// Ambushes / Strike First
			PutInBank(0x0F, 0x8420, Blob.FromHex("AD5668C90B9015C95A901F18ADAB6069018DAB609014EEAC6018900E18ADA96069018DA9609003EEAA60AC5668AE576860"));
			PutInBank(0x1F, 0xD806, CreateLongJumpTableEntry(0x0F, 0x8420));
			Put(0x313FB, Blob.FromHex("eaeaea2006D8"));
			// Runs
			PutInBank(0x0F, 0x8480, Blob.FromHex("AD5868F00E18ADAD6069018DAD609003EEAE60AD586860"));
			PutInBank(0x1F, 0xD81C, CreateLongJumpTableEntry(0x0F, 0x8480));
			Put(0x32418, Blob.FromHex("201CD8"));
			// Physical Damage
			PutInBank(0x0F, 0x84B0, Blob.FromHex("8E7D68AD8768F01DADB2606D82688DB260ADB3606D83688DB360ADB46069008DB46018901AADAF606D82688DAF60ADB0606D83688DB060ADB16069008DB160AE7D6860"));
			PutInBank(0x1F, 0xD822, CreateLongJumpTableEntry(0x0F, 0x84B0));
			Put(0x32968, Blob.FromHex("2022D8"));
			// Magic Damage
			PutInBank(0x0F, 0x8500, Blob.FromHex("AD8A6C2980F01CADB2606D58688DB260ADB3606D59688DB360ADB46069008DB460901AADAF606D58688DAF60ADB0606D59688DB060ADB16069008DB160A912A212A00160"));
			PutInBank(0x1F, 0xD83A, CreateLongJumpTableEntry(0x0F, 0x8500));
			PutInBank(0x0C, 0xB8ED, Blob.FromHex("203AD8eaeaea"));
			// Party Wipes
			PutInBank(0x0F, 0x85D0, Blob.FromHex("EEB564A9008DFD64A200187D00647D00657D00667D0067E8D0F149FF8DFD64A952854B60"));
			PutInBank(0x1F, 0xD82E, CreateLongJumpTableEntry(0x0F, 0x85D0));
			//PutInBank(0x0B, 0x9AF5, Blob.FromHex("202ED8EAEA")); included in 1B changes
			// "Nothing Here"s
			PutInBank(0x0F, 0x8600, Blob.FromHex("A54429C2D005A545F00360A900EEB66060"));
			PutInBank(0x1F, 0xD834, CreateLongJumpTableEntry(0x0F, 0x8600));
			PutInBank(0x1F, 0xCBF3, Blob.FromHex("4C34D8"));

			// Add select button handler on game start menu to change color
			PutInBank(0x0F, 0x8620, Blob.FromHex("203CC4A662A9488540ADFB60D003EEFB60A522F022EEFB60ADFB60C90D300EF007A9018DFB60D005A90F8DFB60A90085222029EBA90060A90160"));
			PutInBank(0x1F, 0xD840, CreateLongJumpTableEntry(0x0F, 0x8620));
			Put(0x3A1B5, Blob.FromHex("2040D8D0034C56A1EA"));
			// Move Most of LoadBorderPalette_Blue out of the way to do a dynamic version.
			PutInBank(0x0F, 0x8700, Blob.FromHex("988DCE038DEE03A90F8DCC03A9008DCD03A9308DCF0360"));

			// Move DrawCommandMenu out of Bank F so we can add no Escape to it
			PutInBank(0x0F, 0x8740, Blob.FromHex("A000A200B91BFA9D9E6AE8C01BD015AD916D2903F00EA9139D9E6AE8C8A9F79D9E6AE8C8E005D0052090F6A200C8C01ED0D260"));

			// Create a clone of IsOnBridge that checks the canal too.
			PutInBank(0x0F, 0x8780, Blob.FromHex("AD0860F014A512CD0960D00DA513CD0A60D006A90085451860A512CD0D60D00DA513CD0E60D006A900854518603860"));

			// BB Absorb fix.
			//PutInBank(0x0F, 0x8800, Blob.FromHex("A000B186C902F005C908F00160A018B186301BC8B1863016C8B1863011C8B186300CA026B1861869010AA0209186A01CB186301AC8B1863015C8B1863010C8B186300BA026B186186901A022918660"));

			// Copyright overhaul, see 0F_8960_DrawSeedAndFlags.asm
			PutInBank(0x0F, 0x8980, Blob.FromHex("A9238D0620A9208D0620A200BD00898D0720E8E060D0F560"));

			// Fast Battle Boxes
			PutInBank(0x0F, 0x8A00, Blob.FromHex("A940858AA922858BA91E8588A969858960"));
			PutInBank(0x0F, 0x8A20, Blob.FromHex($"A9{BattleBoxDrawInRows}8DB96820A1F420E8F4A5881869208588A58969008589A58A186920858AA58B6900858BCEB968D0DE60"));

			// Fast Battle Boxes Undraw (Similar... yet different!)
			PutInBank(0x0F, 0x8A80, Blob.FromHex("A9A0858AA923858BA97E8588A96A858960"));
			PutInBank(0x0F, 0x8AA0, Blob.FromHex($"A9{BattleBoxUndrawRows}8DB96820A1F420E8F4A58838E9208588A589E9008589A58A38E920858AA58BE900858BCEB968D0DE60"));

			// Softlock fix
			Put(0x7C956, Blob.FromHex("A90F2003FE4C008B"));
			PutInBank(0x0F, 0x8B00, Blob.FromHex("BAE030B01E8A8D1001A9F4AAA9FBA8BD0001990001CA88E010D0F4AD1001186907AA9AA52948A52A48A50D48A54848A549484C65C9"));

			// Change INT to MDEF in the Status screen
			Put(0x388F5, Blob.FromHex("968D8E8F"));
			Data[0x38DED] = 0x25;

			//Key Items + Progressive Scaling
			if (flags.ProgressiveScaleMode == ProgressiveScaleMode.OrbProgressiveSlow || flags.ProgressiveScaleMode == ProgressiveScaleMode.OrbProgressiveMedium || flags.ProgressiveScaleMode == ProgressiveScaleMode.OrbProgressiveFast || flags.ProgressiveScaleMode == ProgressiveScaleMode.OrbProgressiveVFast)
			{
				if (flags.ShardHunt)
				{
					PutInBank(0x0F, 0x9000, Blob.FromHex("AD35608DB86060"));
				}
				else
				{
					PutInBank(0x0F, 0x9000, Blob.FromHex("A200AD3160F001E8AD3260F001E8AD3360F001E8AD3460F001E88EB86060"));
				}
			}
			else
			{
				PutInBank(0x0F, 0x9000, Blob.FromHex("A200AD2160F001E8AD2260F001E8AD2560F001E8AD2A60F001E8AD2B60F001E8AD2C60F001E8AD2E60F001E8AD3060F001E8AD0060F001E8AD1260F001E8AD0460F001E8AD0860F001E8AD0C60D001E8AD2360D007AD0A622902F001E8AD2460D007AD05622902F001E8AD2660D007AD08622902F001E8AD2760D007AD09622902F001E8AD2860D007AD0B622902F001E8AD2960D007AD14622901D001E8AD2D60D007AD0E622902F001E8AD2F60D007AD13622903F001E88EB86060"));
			}
			PutInBank(0x1F, 0xCFCB, CreateLongJumpTableEntry(0x0F, 0x9100));
			//Division routine
			PutInBank(0x0F, 0x90C0, Blob.FromHex("8A48A9008513A210261026112613A513C5129004E512851326102611CAD0EDA513851268AA60"));
			// Progressive scaling also writes to 0x9100 approaching 200 bytes, begin next Put at 0x9200.

			// Replace Overworld to Floor and Floor to Floor teleport code to JSR out to 0x9200 to set X / Y AND inroom from unused high bit of X.
			PutInBank(0x1F, 0xC1E2, Blob.FromHex("A9002003FEA545293FAABD00AC8510BD20AC8511BD40AC8548AABDC0AC8549A90F2003FE200092EAEAEAEAEAEA"));
			PutInBank(0x1F, 0xC968, Blob.FromHex("A90F2003FEA645BD00B08510BD00B18511BD00B28548200092A9002003FEA548AABDC0AC8549"));
			PutInBank(0x0F, 0x9200, Blob.FromHex("A200A5100A9002A2814A38E907293F8529A5110A9002860D4A38E907293F852A60"));

			// Critical hit display for number of hits
			PutInBank(0x0F, 0x9280, FF1Text.TextToBytes("Critical hit!!", false));
			PutInBank(0x0F, 0x9290, FF1Text.TextToBytes(" Critical hits!", false));
			PutInBank(0x0F, 0x92A0, Blob.FromHex("AD6B68C901F01EA2019D3A6BA9118D3A6BA900E89D3A6BA0FFC8E8B990929D3A6BD0F6F00EA2FFA0FFC8E8B980929D3A6BD0F6A23AA06BA904201CF7EEF86A60"));

			// Enable 3 palettes in battle
			PutInBank(0x1F, 0xFDF1, CreateLongJumpTableEntry(0x0F, 0x9380));
			PutInBank(0x0F, 0x9380, Blob.FromHex("ADD16A2910F00BA020B9336D99866B88D0F7ADD16A290F8DD16A20A1F4AD0220A9028D1440A93F8D0620A9008D0620A000B9876B8D0720C8C020D0F5A93F8D0620A9008D06208D06208D062060"));
		}

		public void MakeSpace()
		{
			// 54 bytes starting at 0xC265 in bank 1F, ROM offset: 7C275. FULL
			// This removes the code for the minigame on the ship, and moves the prior code around too
			PutInBank(0x1F, 0xC244, Blob.FromHex("F003C6476020C2D7A520290FD049A524F00EA9008524A542C908F074C901F0B160EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));
			// 15 bytes starting at 0xC8A4 in bank 1F, ROM offset: 7C8B4
			// This removes the routine that give a reward for beating the minigame, no need for a reward without the minigame
			PutInBank(0x1F, 0xC8A4, Blob.FromHex("EAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));
			// 28 byte starting at 0xCFCB in bank 1F, ROM offset: 7CFE1
			// This removes the AssertNasirCRC routine, which we were skipping anyways, no point in keeping uncalled routines
			PutInBank(0x1F, 0xCFCB, Blob.FromHex("EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));

			// Used by ShufflePromotions() and AllowNone()
			PutInBank(0x0E, 0xB816, Blob.FromHex("206BC24C95EC"));
			PutInBank(0x1F, 0xC26B, CreateLongJumpTableEntry(0x0F, 0x8B40));
			PutInBank(0x0F, 0x8B40, Blob.FromHex("A562851029030A851118651165110A0A0A1869508540A5100A0A29F0186928854160"));
		}

		public override bool Validate()
		{
			return Get(0, 16) == Blob.FromHex("06400e890e890e401e400e400e400b42");
		}

		public void UpgradeToMMC3()
		{
			Header[4] = 32; // 32 pages of 16 kB
			Header[6] = 0x43; // original is 0x13 where 1 = MMC1 and 4 = MMC3

			// Expand ROM size, moving bank 0F to the end.
			Blob newData = new byte[0x80000];
			Array.Copy(Data, newData, 0x3C000);
			Array.Copy(Data, 0x3C000, newData, 0x7C000, 0x4000);
			Data = newData;

			// Update symbol info
			BA.MemoryMode = MemoryMode.MMC3;

			// Change bank swap code.
			// We put this code at SwapPRG_L, so we don't have to move any of the "long" calls to it.
			// We completely overwrite SetMMC1SwapMode, since we don't need it anymore, and partially overwrite the original SwapPRG.
			Put(0x7FE03, Blob.FromHex("8dfc6048a9068d0080680a8d018048a9078d00806869018d0180a90060"));

			// Initialize MMC3
			Put(0x7FE48, Blob.FromHex("8d00e0a9808d01a0a0008c00a08c00808c0180c88c0080c88c01808c0080c8c88c0180a9038d0080c88c0180a9048d00804ccdffa900"));
			Put(0x7FFCD, Blob.FromHex("c88c0180a9058d0080c88c01804c7cfeea"));

			// Rewrite the lone place where SwapPRG was called directly and not through SwapPRG_L.
			Data[0x7FE97] = 0x03;
		}

		public void WriteSeedAndFlags(string seed, string flags)
		{
			// Replace most of the old copyright string printing with a JSR to a LongJump
			Put(0x38486, Blob.FromHex("20B9FF60"));

			// DrawSeedAndFlags LongJump
			PutInBank(0x1F, 0xFFB9, CreateLongJumpTableEntry(0x0F, 0x8980));

			Blob hash;
			var hasher = SHA256.Create();
			hash = hasher.ComputeHash(Encoding.ASCII.GetBytes($"{seed}_{flags}_{FFRVersion.Sha}"));

			var hashpart = BitConverter.ToUInt64(hash, 0);
			hash = Blob.FromHex("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
			for (int i = 8; i < 24; i += 2)
			{
				// 0xD4 through 0xDF are good symbols to use.
				hash[i] = (byte)(0xD4 + hashpart % 12);
				hashpart /= 12;
			}

			Regex rgx = new Regex("[^a-zA-Z0-9]");
			// Put the new string data in a known location.
			PutInBank(0x0F, 0x8900, Blob.Concat(
				FF1Text.TextToCopyrightLine("Final Fantasy Randomizer " + FFRVersion.Version),
				FF1Text.TextToCopyrightLine((FFRVersion.Branch == "master" ? "Seed " : rgx.Replace(FFRVersion.Branch, "") + " BUILD ") + seed),
				hash));
		}

		public void FixMissingBattleRngEntry()
		{
			// of the 256 entries in the battle RNG table, the 98th entry (index 97) is a duplicate '00' where '95' hex / 149 int is absent.
			// you could arbitrarily choose the other '00', the 111th entry (index 110), to replace instead
			var battleRng = Get(BattleRngOffset, RngSize).Chunk(1).ToList();
			battleRng[97] = Blob.FromHex("95");

			Put(BattleRngOffset, battleRng.SelectMany(blob => blob.ToBytes()).ToArray());
		}

		public void ShuffleRng(MT19337 rng)
		{
			var rngTable = Get(RngOffset, RngSize).Chunk(1).ToList();
			rngTable.Shuffle(rng);

			Put(RngOffset, rngTable.SelectMany(blob => blob.ToBytes()).ToArray());

			var battleRng = Get(BattleRngOffset, RngSize).Chunk(1).ToList();
			battleRng.Shuffle(rng);

			Put(BattleRngOffset, battleRng.SelectMany(blob => blob.ToBytes()).ToArray());
		}

	}
}
