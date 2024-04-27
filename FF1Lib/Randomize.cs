using FF1Lib.Procgen;
namespace FF1Lib;

// ReSharper disable once InconsistentNaming
public partial class FF1Rom : NesRom
{
	public const int LevelRequirementsOffset = 0x6CC81;
	public const int LevelRequirementsSize = 3;
	public const int LevelRequirementsCount = 49;

	public const int StartingGoldOffset = 0x0301C;

	public const int GoldItemOffset = 108; // 108 items before gold chests
	public const int GoldItemCount = 68;
	public List<int> UnusedGoldItems = new List<int> { 110, 111, 112, 113, 114, 116, 120, 121, 122, 124, 125, 127, 132, 158, 165, 166, 167, 168, 170, 171, 172 };

	public ItemNames ItemsText;
	public GearPermissions ArmorPermissions;
	public GearPermissions WeaponPermissions;
	public SpellPermissions SpellPermissions;
	public GameClasses ClassData;
	public RngTables RngTables;
	public TileSetsData TileSetsData;
	public ZoneFormations ZoneFormations;
	public StandardMaps Maps;
	public NpcObjectData NpcData;
	public DialogueData Dialogues;
	//public ShipLocations ShipLocations;

	public DeepDungeon DeepDungeon;

	public OwMapExchange OverworldMapData;
	public Teleporters Teleporters;
	public Overworld Overworld;

	private SanityCheckerV2 sanityChecker = null;
	private IncentiveData incentivesData = null;

	private Blob SavedHash;
	public Settings Settings;

	public void LoadSharedDataTables()
	{
		ItemsText = new ItemNames(this);
		ArmorPermissions = new GearPermissions(0x3BFA0, (int)Item.Cloth, this);
		WeaponPermissions = new GearPermissions(0x3BF50, (int)Item.WoodenNunchucks, this);
		SpellPermissions = new SpellPermissions(this);
		ClassData = new GameClasses(WeaponPermissions, ArmorPermissions, SpellPermissions, this);
	}

	public delegate Task ProgressMessage(int step, int max, string message);

	public delegate Task ReportProgress(string message="", int addMax=0);

	int currentStep = 0;
	int maxSteps = 0;
	public ProgressMessage ProgressCallback;

	public async Task Progress(string message="", int addMax=0)
	{
	    maxSteps += addMax;
	    currentStep += 1;
	    if (ProgressCallback != null) {
		await ProgressCallback(currentStep, maxSteps, message);
	    }
	}
	public async Task Randomize(Blob seed, Flags flags, Preferences preferences)
	{
		if (flags.TournamentSafe) AssureSafe();

		// Copy flags for various use
		Flags flagsForRng = flags.ShallowCopy();
		Flags unmodifiedFlags = flags.ShallowCopy();

		// Setup Rng
		Blob resourcesPackHash = GenerateRng(flagsForRng, seed);

		// Collapse tristate flags
		flags = Flags.ConvertAllTriState(flags, rng);

		Settings = new(true);
		//Settings.GenerateFlagstring();

		await this.Progress("Beginning Randomization", 22);

		RngTables = new(this);
		TileSetsData = new(this);
		ZoneFormations = new(this);
		Overworld = new(this, flags, Settings, rng);
		Overworld.LoadMapExchange();
		//Settings.CollapseRandomSettings(rng);
		//Settings.ProcessStandardFlags();
		//FlagRules.ProcessBaseFlags(settingstest);

		//Settings.SetValue();

		var talkroutines = new TalkRoutines();


		LoadSharedDataTables();
		GlobalHacks();

		Teleporters = new Teleporters(this, Overworld.MapExchangeData);
		Maps = new StandardMaps(this, Teleporters, flags);
		NpcData = new NpcObjectData(this);
		Dialogues = new DialogueData(this);

		ClassesBalances(flags, rng);
		Bugfixes(flags);
		GlobalImprovements(flags, preferences);



		EncounterRate encounterRate = new(this);


		DynamicWindowColor(preferences.MenuColor);


		//Bugfixes(settings);
		//ClassesBalance(settings, rng);



		await this.Progress();

		DeepDungeon = new DeepDungeon(this);

		talkroutines.TransferTalkRoutines(this, flags);
		Dialogues.UpdateNPCDialogues(flags);

		NpcData.Update(Maps, flags);
		PacifistBat(Maps, talkroutines, NpcData);
		FF1Text.AddNewIcons(this, flags);

		if (flags.TournamentSafe) Put(0x3FFE3, Blob.FromHex("66696E616C2066616E74617379"));


		//var overworldMap = new OverworldMap(this, flags);

		//var owMapExchange = await OwMapExchange.FromFlags(this, overworldMap, flags, rng);
		//owMapExchange?.ExecuteStep1();

		await this.Progress();

		//Teleporters teleporters = new Teleporters(this, owMapExchange?.Data);
		//overworldMap.Teleporters = teleporters;

		//ShipLocations = owMapExchange?.ShipLocations ?? OwMapExchange.GetDefaultShipLocations(this);

		//Overworld.Update(Teleporters);

		//var maps = ReadMaps();
		var shopItemLocation = ItemLocations.CaravanItemShop1;
		var oldItemNames = ItemsText.ToList();

		DesertOfDeath.ApplyDesertModifications((bool)flags.DesertOfDeath, this, ZoneFormations, Overworld.Locations.StartingLocation, NpcData, Dialogues);

		//TileSetsData.UpdateTrapTiles(this, ZoneFormations, Settings, rng);
		TileSetsData.UpdateTrapTiles(this, ZoneFormations, flags, rng);
		RngTables.Update(flags, rng);
		//ZoneFormations.ShuffleEnemyFormations(rng, (FormationShuffleMode)Settings.GetInt("FormationShuffleMode"), Settings.GetBool("RandomizeFormationEnemizer") | Settings.GetBool("RandomizeEnemizer"));
		ZoneFormations.ShuffleEnemyFormations(rng, flags.FormationShuffleMode, flags.EnemizerEnabled);
		//ZoneFormations.UnleashWarMECH((WarMECHMode)Settings.GetInt("WarMechMode") == WarMECHMode.Unleashed || (WarMECHMode)Settings.GetInt("WarMechMode") == WarMECHMode.All);
		
		Spooky(talkroutines, NpcData, Dialogues, ZoneFormations, Maps, rng, flags);

		await this.Progress();
		LoadResourcePackMaps(flags.ResourcePack, Maps, Teleporters);


			if((bool)flags.OWDamageTiles || flags.DesertOfDeath)
			{
				EnableDamageTile();
			}
			if ((bool)flags.DamageTilesKill)
			{
				DamageTilesKill(flags.SaveGameWhenGameOver);
			}

			if ((bool)flags.ArmorResistsDamageTileDamage && !(bool)flags.ArmorCrafter)
			{
				EnableArmorDamageTileResist((bool)flags.DamageTilesKill);
			}

		// Adjustable lava damage - run if anything other than the default of 1 damage
		if ((int)flags.DamageTileLow != 1 || (int)flags.DamageTileHigh != 1)
			{
				int DamageTileAmount = rng.Between(flags.DamageTileLow, flags.DamageTileHigh);
				AdjustDamageTileDamage(DamageTileAmount, (bool)flags.DamageTilesKill, (bool)flags.ArmorResistsDamageTileDamage);
			}

		Maps.Update(ZoneFormations, rng);
		UpdateToFR(Maps, Teleporters, TileSetsData, flags, rng);
		GeneralMapHacks(flags, Overworld, Maps, ZoneFormations, TileSetsData, rng);
		TileSetsData.Update(flags, rng);

		//var restructuredMaps = new RestructuredMaps(this, maps, flags, Teleporters, rng);
		//restructuredMaps.Process();

		if ((bool)flags.RandomizeFormationEnemizer)
		{
			DoEnemizer(rng, (bool)flags.RandomizeEnemizer, (bool)flags.RandomizeFormationEnemizer, flags.EnemizerDontMakeNewScripts);
		}

		await this.Progress();

		if (preferences.ModernBattlefield)
		{
			EnableModernBattlefield();
		}

		// NPC
		if (flags.GameMode == GameModes.DeepDungeon)
		{
			await this.Progress("Generating Deep Dungeon's Floors...", 2);

			DeepDungeon.Generate(rng, Overworld.OverworldMap, Teleporters, Dialogues, Maps.GetMapList(), flags);
			DeepDungeonFloorIndicator();
			UnusedGoldItems = new List<int> { };

			await this.Progress("Generating Deep Dungeon's Floors... Done!");
		}

		MapIndex warmMechFloor = (MapIndex)DeepDungeon.WarMechFloor;

		await this.Progress();

		//must be done before spells get shuffled around otherwise we'd be changing a spell that isnt lock
		if (flags.LockMode != LockHitMode.Vanilla)
		{
			ChangeLockMode(flags.LockMode);
		}

		if ((bool)flags.AlternateFiends && !flags.SpookyFlag)
		{
			await this.Progress("Creating new Fiends", 1);
			AlternativeFiends(rng, flags);
		}

		if (flags.BuffTier1DamageSpells)
		{
			BuffTier1DamageSpells();
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
		    Weaponizer(rng, (bool)flags.WeaponizerNamesUseQualityOnly, (bool)flags.WeaponizerCommonWeaponsHavePowers, flags.ItemMagicMode == ItemMagicMode.None);
		}

		if ((bool)flags.ArmorCrafter) {
		    ArmorCrafter(rng, flags.ItemMagicMode == ItemMagicMode.None, flags.RibbonMode == RibbonMode.Split);
		}

		if (flags.ItemMagicMode != ItemMagicMode.None && flags.ItemMagicMode != ItemMagicMode.Vanilla)
		{
			ShuffleItemMagic(rng, flags);
		}

		if (flags.GuaranteedDefenseItem != GuaranteedDefenseItem.None && !(flags.ItemMagicMode == ItemMagicMode.None))
		{
			CraftDefenseItem(flags);
		}

		await this.Progress();

		if (flags.GuaranteedPowerItem != GuaranteedPowerItem.None && !(flags.ItemMagicMode == ItemMagicMode.None))
		{
			CraftPowerItem(flags);
		}

		new RibbonShuffle(this, rng, flags, ItemsText, ArmorPermissions).Work();

		if (((bool)flags.Treasures) && flags.ShardHunt)
		{
			EnableShardHunt(rng, talkroutines, Dialogues, flags.ShardCount, preferences.randomShardNames, new MT19337(funRng.Next()));
		}

		if (!flags.ShardHunt && (flags.GameMode != GameModes.DeepDungeon))
		{
			SetOrbRequirement(rng, talkroutines, Dialogues, flags.OrbsRequiredCount, flags.OrbsRequiredMode, (bool)flags.OrbsRequiredSpoilers);
		}

		if (flags.TransformFinalFormation != FinalFormation.None && !flags.SpookyFlag)
		{
			TransformFinalFormation(flags.TransformFinalFormation, flags.EvadeCap, rng);
		}

		await this.Progress();

		if ((bool)flags.IsBridgeFree && (!flags.DesertOfDeath))
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

		if ((bool)flags.IsCanalFree)
		{
			EnableFreeCanal((bool)flags.NPCItems);
		}

		await this.Progress();

		if ((bool)flags.IsCanoeFree)
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

		if ((bool)flags.FreeRod)
		{
			EnableFreeRod();
		}

		encounterRate.ScaleEncounterRate(flags);

		var shopData = new ShopData(this);
		shopData.LoadData();

		var extConsumables = new ExtConsumables(this, flags, rng, shopData);
		extConsumables.AddNormalShopEntries();

		Overworld.Update(Teleporters);

		if(flags.NoOverworld) await this.Progress("Linking NoOverworld's Map", 1);
		NoOverworld(Overworld.DecompressedMap, Maps, Teleporters, talkroutines, Dialogues, NpcData, flags, rng);

		DraculasCurse(Overworld, Teleporters, rng, flags);

		await this.Progress();

		// NPC Stuff
		ClassAsNPC(flags, talkroutines, NpcData, Dialogues, Maps, rng);


		// NOTE: logic checking for relocated chests
		// accounts for NPC locations and whether they
		// are fightable/killable, so it needs to
		// happen after anything that adds, removes or
		// relocates NPCs or changes their routines.

		await new RelocateChests(this).RandomlyRelocateChests(rng, Maps, TileSetsData, Teleporters, NpcData, flags);

		//EnterTeleData enterBackup = new EnterTeleData(this);
		//NormTeleData normBackup = new NormTeleData(this);

		//enterBackup.LoadData();
		//normBackup.LoadData();

		var maxRetries = 3;
		for (var i = 0; i < maxRetries; i++)
		{
			try
			{
				await this.Progress((bool)flags.Treasures ? "Shuffling Treasures - Retries: " + i : "Placing Treasures", 3);

				//enterBackup.StoreData();
				//normBackup.StoreData();

				//overworldMap = new OverworldMap(this, flags);
				//overworldMap.Teleporters = teleporters;
				/*
				if (((bool)flags.Entrances || (bool)flags.Floors || (bool)flags.Towns) && ((bool)flags.Treasures) && ((bool)flags.NPCItems) && flags.GameMode == GameModes.Standard)
				{
					overworldMap.ShuffleEntrancesAndFloors(rng, flags);
				}*/

				// Disable the Princess Warp back to Castle Coneria
				if ((bool)flags.Entrances || (bool)flags.Floors || (flags.GameMode == GameModes.Standard && flags.OwMapExchange != OwMapExchanges.None) || (flags.OrbsRequiredCount == 0 && !flags.ShardHunt))
					talkroutines.ReplaceChunk(newTalkRoutines.Talk_Princess1, Blob.FromHex("20CC90"), Blob.FromHex("EAEAEA"));
				/*
				if ((bool)flags.Treasures && (bool)flags.ShuffleObjectiveNPCs && (flags.GameMode != GameModes.DeepDungeon))
				{
					overworldMap.ShuffleObjectiveNPCs(rng);
				}*/

				incentivesData = new IncentiveData(rng, flags, Overworld.OverworldMap, shopItemLocation, new SanityCheckerV1());

				await this.Progress();

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

						if (flags.GuaranteedDefenseItem != GuaranteedDefenseItem.None && !(flags.ItemMagicMode == ItemMagicMode.None))
							excludeItemsFromRandomShops.Add(Item.PowerRod);

						if (flags.GuaranteedPowerItem != GuaranteedPowerItem.None && !(flags.ItemMagicMode == ItemMagicMode.None))
							excludeItemsFromRandomShops.Add(Item.PowerGauntlets);
					}

					if ((bool)flags.NoMasamune)
					{
						excludeItemsFromRandomShops.Add(Item.Masamune);
					}

					if ((bool)flags.NoXcalber)
					{
						excludeItemsFromRandomShops.Add(Item.Xcalber);
					}

					shopItemLocation = ShuffleShops(rng, (bool)flags.ImmediatePureAndSoftRequired, ((bool)flags.RandomWares), excludeItemsFromRandomShops, flags.WorldWealth, Overworld.OverworldMap.ConeriaTownEntranceItemShopIndex);

					incentivesData = new IncentiveData(rng, flags, Overworld.OverworldMap, shopItemLocation, new SanityCheckerV1());
				}

			        await this.Progress();

				if (flags.GameMode == GameModes.DeepDungeon)
				{
					sanityChecker = new SanityCheckerV2(Maps, Overworld, NpcData, Teleporters, this, shopItemLocation);
					generatedPlacement = DeepDungeon.ShuffleTreasures(flags, incentivesData, rng);
				}
				else if ((bool)flags.Treasures)
				{
					sanityChecker = new SanityCheckerV2(Maps, Overworld, NpcData, Teleporters, this, shopItemLocation);
					generatedPlacement = ShuffleTreasures(rng, flags, incentivesData, shopItemLocation, Overworld, Teleporters, sanityChecker);
				}
				else if (Overworld.MapExchange != null)
				{
					sanityChecker = new SanityCheckerV2(Maps, Overworld, NpcData, Teleporters, this, shopItemLocation);
					if (!sanityChecker.CheckSanity(ItemLocations.AllQuestItemLocations.ToList(), null, flags).Complete) throw new InsaneException("Not Completable");
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

		List<string> funMessages = new()
		{
			"Placing Out of Bound Bat",
			"Cleaning up Lich's closet",
			"Labelling pots in Sarda's Cave",
			"Partying in the Bat Party Room",
			"Buffing up NPC's path-blocking AI",
			"Debugging WarMech's software",
			"Knocking down some impertinent fools",
			"Sending Garland 2,000 years in the past",
			"Giving a snack to Titan while waiting for the main course",
			"Disguising Astos",
			"Cursing the Elf Prince",
			"Stealing Matoya's Crystal",
			"Abducting Princess Sara",
			"Placing the worst skills in Medusa's script",
			"Applying for a Bridge building permit",
			"Reticulating Splines",
			"Digging Cave Holes",
			"Bottling the Fairy",
			"Floating the Sky Castle",
			"Locking the door in Temple of Fiends",
			"Teaching Kraken Kung Fu",
			"Raising the Lich",
		};

		funMessages.AddRange(Enumerable.Repeat("Finalizing", funMessages.Count * 4).ToList());

		await this.Progress(funMessages.PickRandom(rng));

		// Change Astos routine so item isn't lost in wall of text
		if ((bool)flags.NPCItems || (bool)flags.NPCFetchItems || (bool)flags.ShuffleAstos)
			talkroutines.Replace(newTalkRoutines.Talk_Astos, Blob.FromHex("A674F005BD2060F027A57385612080B1B020A572203D96A5752020B1A476207F90207392A5611820109F201896A9F060A57060"));

		NpcData.UpdateItemPlacement(generatedPlacement);

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

		new ShopKiller(rng, flags, Maps, this).KillShops();

		shopData.LoadData();

		new LegendaryShops(rng, flags, Maps, shopData, TileSetsData, this).PlaceShops();

		if (flags.GameMode == GameModes.DeepDungeon)
		{
			shopData.Shops.Find(x => x.Type == FF1Lib.ShopType.Item && x.Entries.Contains(Item.Bottle)).Entries.Remove(Item.Bottle);
			shopData.StoreData();
		}

		// This need to be after the last modification of shopData 
		shopData.UpdateShopSlotPlacement(generatedPlacement);
		
		//has to be done before modifying itemnames and after modifying spellnames...
		extConsumables.LoadSpells();

		await this.Progress();

		if (preferences.AccessibleSpellNames)
		{
			AccessibleSpellNames(flags);
		}

		if (flags.SpellNameMadness != SpellNameMadness.None)
		{
			MixUpSpellNames(flags.SpellNameMadness, rng);
		}

		if (flags.EnableSoftInBattle)
		{
			EnableSoftInBattle();
		}

		if (flags.EnableLifeInBattle != LifeInBattleSetting.LifeInBattleOff)
		{
			EnableLifeInBattle(flags);
		}

		if (flags.TranceHasStatusElement)
		{
			TranceHasStatusElement();
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
			EnableSaveOnDeath(flags, Overworld);
		}

		Console.WriteLine("588");

		ShuffleEnemyScripts(rng, flags);

		ShuffleEnemySkillsSpells(rng, flags);

		StatusAttacks(flags, rng);

		await this.Progress();

		if ((bool)flags.UnrunnableShuffle) {
			int UnrunnablePercent = rng.Between(flags.UnrunnablesLow, flags.UnrunnablesHigh);
			// This is separate because the basic Imp formation is not otherwise included in the possible unrunnable formations
			if (UnrunnablePercent >= 100)
				CompletelyUnrunnable();
			else
				ShuffleUnrunnable(rng, flags, UnrunnablePercent);
		}

		// Always on to supply the correct changes for WaitWhenUnrunnable
		AllowStrikeFirstAndSurprise(flags.WaitWhenUnrunnable, (bool)flags.UnrunnablesStrikeFirstAndSurprise);

		if ((bool)flags.EnemyFormationsSurprise)
		{
			ShuffleSurpriseBonus(rng);
		}

		// After unrunnable shuffle and before formation shuffle. Perfect!
		WarMechMode.Process(this, flags, NpcData, ZoneFormations, Dialogues, rng, Maps, warmMechFloor);
		FightBahamut(flags, talkroutines, NpcData, ZoneFormations, Dialogues, Maps, rng);
		ShuffleAstos(flags, NpcData, Dialogues, talkroutines, rng);

		if ((bool)flags.FiendShuffle)
		{
			FiendShuffle(rng);
		}

		await this.Progress();

		if (flags.DisableMinimap)
		{
			DisableMinimap();
		}

		new TreasureStacks(this, flags).SetTreasureStacks();

		if (flags.WeaponCritRate)
		{
			DoubleWeaponCritRates();
		}

		List<int> blursesValues = Enumerable.Repeat(0, 40).ToList();

			//needs to go after item magic, moved after double weapon crit to have more control over the actual number of crit gained.
			if ((bool)flags.RandomWeaponBonus)
			{
				blursesValues = RandomWeaponBonus(rng, flags.RandomWeaponBonusLow, flags.RandomWeaponBonusHigh, (bool)flags.RandomWeaponBonusExcludeMasa, preferences.CleanBlursedEquipmentNames);
			}

			if ((bool)flags.RandomArmorBonus)
			{
				RandomArmorBonus(rng, flags.RandomArmorBonusLow, flags.RandomArmorBonusHigh, preferences.CleanBlursedEquipmentNames);
			}

		if (flags.WeaponBonuses)
		{
			IncreaseWeaponBonus(flags.WeaponTypeBonusValue);
		}

		await this.Progress();

		if (flags.ImproveTurnOrderRandomization)
		{
			ImproveTurnOrderRandomization(rng);
		}

		if ((bool)flags.IncreaseDarkPenalty)
		{
			IncreaseDarkPenalty();
		}

		if (preferences.FunEnemyNames && !flags.EnemizerEnabled)
		{
		    FunEnemyNames(preferences.TeamSteak, (bool)flags.AlternateFiends, new MT19337(funRng.Next()));
		}

		await this.Progress();

		if (ItemsText[(int)Item.Ribbon].Length > 7
		    && ItemsText[(int)Item.Ribbon][7] == ' ')
		    {
			ItemsText[(int)Item.Ribbon] = ItemsText[(int)Item.Ribbon].Remove(7);
		    }

		if (flags.ImprovedClinic && !(bool)flags.RecruitmentMode)
		{
			ImprovedClinic();
		}

		if (flags.Etherizer)
		{
			Etherizer();
			ItemsText[(int)Item.Tent] = "ETHR@p";
			ItemsText[(int)Item.Cabin] = "DRY@p ";
			ItemsText[(int)Item.House] = "XETH@p";
		}

		NPCHints(rng, NpcData, Dialogues, flags, incentivesData, sanityChecker, shopData);
		SkyWarriorSpoilerBats(rng, flags, NpcData, Dialogues);

		MonsterInABox(ZoneFormations, NpcData, Dialogues, rng, flags);

		ExpGoldBoost(flags);

		if(flags.ExpMultiplierFighter != 1.0)
		{
			ScaleAltExp(flags.ExpMultiplierFighter, FF1Class.Fighter);
		}
		if (flags.ExpMultiplierThief != 1.0)
		{
			ScaleAltExp(flags.ExpMultiplierThief, FF1Class.Thief);
		}
		if (flags.ExpMultiplierBlackBelt != 1.0)
		{
			ScaleAltExp(flags.ExpMultiplierBlackBelt, FF1Class.BlackBelt);
		}
		if (flags.ExpMultiplierRedMage != 1.0)
		{
			ScaleAltExp(flags.ExpMultiplierRedMage, FF1Class.RedMage);
		}

		await this.Progress();

		if (flags.ExpMultiplierWhiteMage != 1.0)
		{
			ScaleAltExp(flags.ExpMultiplierWhiteMage, FF1Class.WhiteMage);
		}
		if (flags.ExpMultiplierBlackMage != 1.0)
		{
			ScaleAltExp(flags.ExpMultiplierBlackMage, FF1Class.BlackMage);
		}

		ScalePrices(flags, rng, ((bool)flags.ClampMinimumPriceScale), shopItemLocation, flags.ImprovedClinic);

		extConsumables.AddExtConsumables();

		if ((bool)flags.SwolePirates)
		{
			EnableSwolePirates();
		}

		if ((bool)flags.SwoleAstos)
		{
			EnableSwoleAstos(rng);
		}

		if (flags.EnemyScaleStatsHigh != 100 || flags.EnemyScaleStatsLow != 100 || ((bool)flags.SeparateEnemyHPScaling && (flags.EnemyScaleHpLow != 100 || flags.EnemyScaleHpHigh != 100)))
		{
			ScaleEnemyStats(rng, flags);
		}

		if (flags.BossScaleStatsHigh != 100 || flags.BossScaleStatsLow != 100 || ((bool)flags.SeparateBossHPScaling && (flags.BossScaleHpLow != 100 || flags.BossScaleHpHigh != 100)))
		{
			ScaleBossStats(rng, flags);
		}

		PartyGeneration(rng, flags, preferences);
		PubReplaceClinic(rng, Maps.AttackedTown, flags);
		await this.Progress();

		if ((bool)flags.MapCanalBridge)
		{
			EnableCanalBridge();
		}

		if (flags.NonesGainXP || flags.DeadsGainXP)
		{
			XpAdmissibility((bool)flags.NonesGainXP, flags.DeadsGainXP);
		}

		SetProgressiveScaleMode(flags);

		ClassData.SetMPMax(flags);
		ClassData.SetMpGainOnMaxGain(flags, this);
		ClassData.RaiseThiefHitRate(flags);
		ClassData.BuffThiefAGI(flags);
		ClassData.EarlierHighTierMagicCharges(flags);
		ClassData.Randomize(flags, Settings, rng, oldItemNames, ItemsText, this);
		ClassData.ProcessStartWithRoutines(flags, blursesValues, this);;

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

		if (flags.ItemMagicMode == ItemMagicMode.None)
		{
			NoItemMagic(flags);
		}

		if (flags.ShopInfo)
		{
			ShopUpgrade(flags, Dialogues, preferences);
		}

		await this.Progress();

		await this.LoadResourcePack(flags.ResourcePack, Dialogues);

		RollCredits(rng);
		StatsTrackingScreen();

		if (preferences.DisableDamageTileFlicker || flags.TournamentSafe)
		{
			DisableDamageTileFlicker();
		}
		if (preferences.DisableDamageTileSFX)
		{
			DisableDamageTileSFX();
		}

		if (preferences.ThirdBattlePalette || flags.ResourcePack != null)
		{
			UseVariablePaletteForCursorAndStone();
		}

		if (preferences.PaletteSwap && !flags.EnemizerEnabled)
		{
			PaletteSwap(new MT19337(funRng.Next()));
		}

		if (preferences.TeamSteak && !(bool)flags.RandomizeEnemizer)
		{
			TeamSteak();
		}


		ChangeLute(preferences.ChangeLute, Dialogues, new MT19337(funRng.Next()));
		TitanSnack(preferences.TitanSnack, NpcData, Dialogues, new MT19337(funRng.Next()));
		HurrayDwarfFate(preferences.HurrayDwarfFate, NpcData, Dialogues, new MT19337(funRng.Next()));

		await this.Progress();

		if (preferences.Music != MusicShuffle.None)
		{
			ShuffleMusic(preferences.Music, new MT19337(funRng.Next()));
		}

		if ((bool)flags.AirBoat)
		{
			EnableAirBoat((bool)flags.IsAirshipFree, (bool)flags.IsShipFree);
		}

		if (preferences.DisableSpellCastFlash || flags.TournamentSafe)
		{
			DisableSpellCastScreenFlash();
		}

		if (preferences.LockRespondRate)
		{
			LockRespondRate(preferences.RespondRate);
		}

		if (preferences.UninterruptedMusic)
		{
			UninterruptedMusic();
		}

		if (preferences.SpriteSheet != null)
		{
		    using (var stream = new MemoryStream(Convert.FromBase64String(preferences.SpriteSheet)))
		    {
				await SetCustomPlayerSprites(stream, preferences.ThirdBattlePalette);
		    }
		}

		if ((bool)flags.IsShipFree)
		{
			Overworld.SetShipLocation(255);
		}

		//owMapExchange?.ExecuteStep2();

		// Used to be a separate Quick Minimap flag - consolidated into Speed Hacks
		if(flags.SpeedHacks || Overworld.MapExchange != null)
		{
			new QuickMiniMap(this, Overworld.DecompressedMap).EnableQuickMinimap();
		}

		if (flags.TournamentSafe || preferences.CropScreen) ActivateCropScreen();

		var expChests = new ExpChests(this, flags, rng);
		expChests.BuildExpChests();

		if (flags.Archipelago)
		{
			Overworld.SetShipLocation(255);

			Archipelago exporter = new Archipelago(this, generatedPlacement, sanityChecker, expChests, incentivesData, Overworld.Locations, seed, flags, unmodifiedFlags, preferences);
			Utilities.ArchipelagoCache = exporter.Work();
		}

		ItemsText.Write(this, UnusedGoldItems);

		if (flags.Spoilers && sanityChecker != null) new ExtSpoiler(this, sanityChecker, shopData, ItemsText, generatedPlacement, Overworld, incentivesData, WeaponPermissions, ArmorPermissions, flags).WriteSpoiler();

		OpenChestsInOrder(flags.OpenChestsInOrder && !flags.Archipelago);

		if (flags.SetRNG)
		{
			SetRNG(flags);
		}

		// Write back everything
		talkroutines.Write(this);
		NpcData.Write(talkroutines.ScriptPointers);
		Dialogues.Write();

		encounterRate.Write();

		Maps.Write();
		TileSetsData.Write();
		ZoneFormations.Write(this);
		RngTables.Write(this);
		Teleporters.Write();
		Overworld.Write();
		ArmorPermissions.Write(this);
		WeaponPermissions.Write(this);
		SpellPermissions.Write(this);
		ClassData.Write(this);

		await this.Progress();


		uint last_rng_value = rng.Next();

		//WriteSeedAndFlags(seed.ToHex(), flags, flagsForRng, unmodifiedFlags, "ressourcePackHash", last_rng_value);
		WriteSeedAndFlags(seed.ToHex(), flags, flagsForRng, unmodifiedFlags, resourcesPackHash.ToHex(), last_rng_value);
		ExtraTrackingAndInitCode(flags, preferences);

		await this.Progress("Randomization Completed");
	}
}
