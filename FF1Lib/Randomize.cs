using FF1Lib.Procgen;
using RomUtilities;
namespace FF1Lib;

// ReSharper disable once InconsistentNaming
public partial class FF1Rom : NesRom
{
	public const int LevelRequirementsOffset = 0x6CC81;
	public const int LevelRequirementsSize = 3;
	public const int LevelRequirementsCount = 49;

	public const int GoldItemOffset = 108; // 108 items before gold chests
	public const int GoldItemCount = 68;
	//public List<int> UnusedGoldItems = new List<int> { 110, 111, 112, 113, 114, 116, 120, 121, 122, 124, 125, 127, 132, 158, 165, 166, 167, 168, 170, 171, 172 };

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
	public TalkRoutines TalkRoutines;
	public StartingItems StartingItems;
	public EncounterRate EncounterRates;
	//public ShipLocations ShipLocations;

	public DeepDungeon DeepDungeon;

	public OwMapExchange OverworldMapData;
	public Teleporters Teleporters;
	public Overworld Overworld;

	private SanityCheckerV2 sanityChecker = null;
	private PlacementContext PlacementContext = null;

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
		// Confirm ROM integrity
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

		// Load Initial Data
		RngTables = new(this);
		TileSetsData = new(this);
		ZoneFormations = new(this);
		Overworld = new(this, flags, ZoneFormations, Settings, rng);
		Overworld.LoadMapExchange();
		TalkRoutines = new();

		//Settings.CollapseRandomSettings(rng);
		//Settings.ProcessStandardFlags();
		//FlagRules.ProcessBaseFlags(settingstest);

		//Settings.SetValue();

		// We should apply Global Hacks after loading everything, but some data classes assume data has been moved already, so we call it in the middle -- To fix
		GlobalHacks();

		// Finis Loading Initial Data
		ItemsText = new ItemNames(this);
		ArmorPermissions = new GearPermissions(0x3BFA0, (int)Item.Cloth, this);
		WeaponPermissions = new GearPermissions(0x3BF50, (int)Item.WoodenNunchucks, this);
		SpellPermissions = new SpellPermissions(this);
		ClassData = new GameClasses(WeaponPermissions, ArmorPermissions, SpellPermissions, this);
		Teleporters = new Teleporters(this, Overworld.MapExchangeData);
		Maps = new StandardMaps(this, Teleporters, flags);
		NpcData = new NpcObjectData(Maps, flags, this);
		Dialogues = new DialogueData(this);
		EncounterRates = new(this);

		await this.Progress();

		// Apply general fixes and hacks
		Bugfixes(flags);
		GlobalImprovements(flags, Maps, preferences);
		MiscHacks(flags, rng);

		// Ressources Packs
		LoadResourcePackMaps(flags.ResourcePack, Maps, Teleporters);

		await this.Progress();

		// Game Modes
		DeepDungeon = new DeepDungeon(this);
		DesertOfDeath.ApplyDesertModifications((bool)flags.DesertOfDeath, this, ZoneFormations, Overworld.Locations.StartingLocation, NpcData, Dialogues);
		Spooky(TalkRoutines, NpcData, Dialogues, ZoneFormations, Maps, rng, flags);
		BlackOrbMode(TalkRoutines, Dialogues, flags, preferences, rng, new MT19337(funRng.Next()));
		if (flags.NoOverworld) await this.Progress("Linking NoOverworld's Map", 1);
		NoOverworld(Overworld.DecompressedMap, Maps, Teleporters, TileSetsData, TalkRoutines, Dialogues, NpcData, flags, rng);
		DraculasCurse(Overworld, Teleporters, rng, flags);

		FF1Text.AddNewIcons(this, flags);

		await this.Progress();

		var oldItemNames = ItemsText.ToList();

		// Maps
		Overworld.Update(Teleporters);
		GeneralMapHacks(flags, Overworld, Maps, ZoneFormations, TileSetsData, rng);
		Maps.Update(ZoneFormations, rng);
		UpdateToFR(Maps, Teleporters, TileSetsData, flags, rng);

		// Tile Sets 
		TileSetsData.Update(flags, rng);
		TileSetsData.UpdateTrapTiles(this, ZoneFormations, flags, rng);
		DamageTilesHack(flags, Overworld);

		await this.Progress();

		// NPC
		if (flags.GameMode == GameModes.DeepDungeon)
		{
			await this.Progress("Generating Deep Dungeon's Floors...", 2);

			DeepDungeon.Generate(rng, Overworld.OverworldMap, Teleporters, Dialogues, Maps.GetMapList(), flags);
			DeepDungeonFloorIndicator();

			await this.Progress("Generating Deep Dungeon's Floors... Done!");
		}

		MapIndex warmMechFloor = (MapIndex)DeepDungeon.WarMechFloor;

		await this.Progress();



		await this.Progress();

		EncounterRates.ScaleEncounterRate(flags);

		var shopData = new ShopData(this);
		shopData.LoadData();

		var extConsumables = new ExtConsumables(this, flags, rng, shopData);
		extConsumables.AddNormalShopEntries();



		await this.Progress();

		// NPC Stuff
		TalkRoutines.TransferTalkRoutines(this, flags);
		Dialogues.UpdateNPCDialogues(flags);
		PacifistBat(Maps, TalkRoutines, NpcData);
		TalkRoutines.Update(flags);
		ClassAsNPC(flags, TalkRoutines, NpcData, Dialogues, Maps, rng);

		// NOTE: logic checking for relocated chests
		// accounts for NPC locations and whether they
		// are fightable/killable, so it needs to
		// happen after anything that adds, removes or
		// relocates NPCs or changes their routines.
		await new RelocateChests(this).RandomlyRelocateChests(rng, Maps, TileSetsData, Teleporters, NpcData, flags);


		// Spells
		//must be done before spells get shuffled around otherwise we'd be changing a spell that isnt lock
		SpellBalanceHacks(flags, rng);

		if ((bool)flags.GenerateNewSpellbook)
		{
			CraftNewSpellbook(rng, (bool)flags.SpellcrafterMixSpells, flags.LockMode, (bool)flags.MagicLevels, (bool)flags.SpellcrafterRetainPermissions);
		}

		if (flags.TranceHasStatusElement)
		{
			TranceHasStatusElement();
		}

		// Create items
		if ((bool)flags.Weaponizer)
		{
			Weaponizer(rng, (bool)flags.WeaponizerNamesUseQualityOnly, (bool)flags.WeaponizerCommonWeaponsHavePowers, flags.ItemMagicMode == ItemMagicMode.None);
		}

		if ((bool)flags.ArmorCrafter)
		{
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


		if (flags.WeaponCritRate)
		{
			DoubleWeaponCritRates();
		}
		if (flags.ItemMagicMode == ItemMagicMode.None)
		{
			NoItemMagic(flags);
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

		if (flags.Etherizer)
		{
			Etherizer();
			ItemsText[(int)Item.Tent] = "ETHR@p";
			ItemsText[(int)Item.Cabin] = "DRY@p ";
			ItemsText[(int)Item.House] = "XETH@p";
		}

		// Starting Inventory
		StartingItems = new StartingItems(new() { }, rng, flags, this);


		// Shop stuff


		// Placement Context
		var priceList = Get(0x37C00, 0x200).ToUShorts().Select(x => (int)x).ToList(); // Temprorary until we extract price
		PlacementContext = new PlacementContext(StartingItems, new() { }, priceList, rng, flags);

		// ShopSlot should be managed by shop class, right? Maybe, probably, we'll know for sure with Shop Class
		PlacementContext.ShopSlot = ShuffleShops(rng, (bool)flags.Shops, (bool)flags.ImmediatePureAndSoftRequired, ((bool)flags.RandomWares), PlacementContext.ExcludedItemsFromShops, flags.WorldWealth, Overworld.OverworldMap.ConeriaTownEntranceItemShopIndex);

		// Sanity + Actual Placement
		sanityChecker = new SanityCheckerV2(Maps, Overworld, NpcData, Teleporters, TileSetsData, this, PlacementContext.ShopSlot);
		if (!sanityChecker.CheckSanity(ItemLocations.AllQuestItemLocations.ToList(), null, flags, true).Complete) throw new InsaneException("Not Completable");

		await this.Progress((bool)flags.Treasures ? "Shuffling Treasures" : "Placing Treasures", 1);

		ItemPlacement itemPlacement = ItemPlacement.Create(this, flags, PlacementContext, PlacementContext.ShopSlot, Overworld, sanityChecker);
		itemPlacement.PlaceItems(rng);

		NpcData.UpdateItemPlacement(itemPlacement.PlacedItems);

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

		new ShopKiller(rng, flags, Maps, this).KillShops();

		shopData.LoadData();

		new LegendaryShops(rng, flags, Maps, shopData, TileSetsData, this).PlaceShops();

		if (flags.GameMode == GameModes.DeepDungeon)
		{
			shopData.Shops.Find(x => x.Type == FF1Lib.ShopType.Item && x.Entries.Contains(Item.Bottle)).Entries.Remove(Item.Bottle);
			shopData.StoreData();
		}

		// This need to be after the last modification of shopData 
		shopData.UpdateShopSlotPlacement(itemPlacement.PlacedItems);
		
		//has to be done before modifying itemnames and after modifying spellnames...
		extConsumables.LoadSpells();

		await this.Progress();

		// Should be in spell class
		if (preferences.AccessibleSpellNames)
		{
			AccessibleSpellNames(flags);
		}

		if (flags.SpellNameMadness != SpellNameMadness.None)
		{
			MixUpSpellNames(flags.SpellNameMadness, rng);
		}

		await this.Progress();

		// Encounters
		ZoneFormations.ShuffleEnemyFormations(rng, flags.FormationShuffleMode, flags.EnemizerEnabled);
		ZoneFormations.UnleashWarMECH(flags.WarMECHMode == WarMECHMode.Unleashed || flags.WarMECHMode == WarMECHMode.All);

		// Enemies
		ShuffleEnemyScripts(rng, flags);
		ShuffleEnemySkillsSpells(rng, flags);
		StatusAttacks(flags, rng);
		ShuffleUnrunnable(rng, flags);

		// Always on to supply the correct changes for WaitWhenUnrunnable
		AllowStrikeFirstAndSurprise(flags.WaitWhenUnrunnable, (bool)flags.UnrunnablesStrikeFirstAndSurprise);
		ShuffleSurpriseBonus((bool)flags.EnemyFormationsSurprise, rng);

		// After unrunnable shuffle and before formation shuffle. Perfect!
		WarMechMode.Process(this, flags, NpcData, ZoneFormations, Dialogues, rng, Maps, warmMechFloor);
		FightBahamut(flags, TalkRoutines, NpcData, ZoneFormations, Dialogues, Maps, rng);
		Astos(NpcData, Dialogues, TalkRoutines, flags, rng);
		EnableSwolePirates((bool)flags.SwolePirates);

		if ((bool)flags.AlternateFiends && !flags.SpookyFlag) await this.Progress("Creating new Fiends", 1);
		AlternativeFiends(rng, flags);
		TransformFinalFormation(flags, rng);
		DoEnemizer(flags, rng);

		ScaleEnemyStats(rng, flags);
		ScaleBossStats(rng, flags);

		if ((bool)flags.FiendShuffle)
		{
			FiendShuffle(rng);
		}


		await this.Progress();


		// Misc
		RngTables.Update(flags, rng);
		DisableMinimap(flags.DisableMinimap);
		ImproveTurnOrderRandomization(flags.ImproveTurnOrderRandomization, rng);
		SavingHacks(Overworld, flags);
		ImprovedClinic(flags.ImprovedClinic && !(bool)flags.RecruitmentMode);
		IncreaseDarkPenalty((bool)flags.IncreaseDarkPenalty);
		new QuickMiniMap(this, Overworld.DecompressedMap).EnableQuickMinimap(flags.SpeedHacks || Overworld.MapExchange != null);
		ShopUpgrade(flags, Dialogues, preferences);
		EnableAirBoat(flags);
		OpenChestsInOrder(flags.OpenChestsInOrder && !flags.Archipelago);
		SetRNG(flags);

		new TreasureStacks(this, flags).SetTreasureStacks();

		await this.Progress();

		NPCHints(rng, NpcData, Maps, Dialogues, flags, PlacementContext, sanityChecker, shopData);
		SkyWarriorSpoilerBats(rng, flags, NpcData, Dialogues);

		MonsterInABox(itemPlacement, ZoneFormations, TileSetsData, NpcData, Dialogues, rng, flags);

		ExpGoldBoost(flags);

		await this.Progress();

		ScalePrices(flags, rng, ((bool)flags.ClampMinimumPriceScale), PlacementContext.ShopSlot, flags.ImprovedClinic);

		extConsumables.AddExtConsumables();

		// Party
		PartyGeneration(rng, flags, preferences);
		PubReplaceClinic(rng, Maps.AttackedTown, flags);

		await this.Progress();

		// Experience
		ScaleAllAltExp(flags);
		XpAdmissibility((bool)flags.NonesGainXP, flags.DeadsGainXP);
		SetProgressiveScaleMode(flags);
		var expChests = new ExpChests(itemPlacement.PlacedItems, this, flags, rng);
		expChests.BuildExpChests();

		// Classes Features
		ClassesBalances(flags, rng);
		ClassData.SetMPMax(flags);
		ClassData.SetMpGainOnMaxGain(flags, this);
		ClassData.RaiseThiefHitRate(flags);
		ClassData.BuffThiefAGI(flags);
		ClassData.EarlierHighTierMagicCharges(flags);
		ClassData.Randomize(flags, Settings, rng, oldItemNames, ItemsText, this);
		ClassData.ProcessStartWithRoutines(flags, blursesValues, this);;
		EnableRandomPromotions(flags, rng);

		await this.Progress();

		await this.LoadResourcePack(flags.ResourcePack, Dialogues);

		StatsTrackingHacks(flags, preferences);
		RollCredits(rng);
		if ((bool)flags.IsShipFree) Overworld.SetShipLocation(255);
		if (flags.TournamentSafe || preferences.CropScreen) ActivateCropScreen();

		// Quality of Life Stuff
		QualityOfLifeHacks(flags, preferences);
		UseVariablePaletteForCursorAndStone(preferences.ThirdBattlePalette || flags.ResourcePack != null);
		EnableModernBattlefield(preferences.ModernBattlefield);
		DynamicWindowColor(preferences.MenuColor);

		// Fun Stuff
		ChangeLute(preferences.ChangeLute, Dialogues, new MT19337(funRng.Next()));
		TitanSnack(preferences.TitanSnack, NpcData, Dialogues, new MT19337(funRng.Next()));
		HurrayDwarfFate(preferences.HurrayDwarfFate, NpcData, Dialogues, new MT19337(funRng.Next()));
		PaletteSwap(preferences.PaletteSwap && !flags.EnemizerEnabled, new MT19337(funRng.Next()));
		TeamSteak(preferences.TeamSteak && !(bool)flags.RandomizeEnemizer);
		FunEnemyNames(flags, preferences, new MT19337(funRng.Next()));
		ShuffleMusic(preferences.Music, new MT19337(funRng.Next()));

		await this.Progress();

		// Custom Sprites
		if (preferences.SpriteSheet != null)
		{
		    using (var stream = new MemoryStream(Convert.FromBase64String(preferences.SpriteSheet)))
		    {
				await SetCustomPlayerSprites(stream, preferences.ThirdBattlePalette);
		    }
		}

		// Archipelago
		if (flags.Archipelago)
		{
			Overworld.SetShipLocation(255);

			Archipelago exporter = new Archipelago(this, itemPlacement.PlacedItems, sanityChecker, expChests, PlacementContext, Overworld.Locations, seed, flags, unmodifiedFlags, preferences);
			Utilities.ArchipelagoCache = exporter.Work();
		}

		// Spoilers
		if (flags.Spoilers && sanityChecker != null) new ExtSpoiler(this, sanityChecker, shopData, ItemsText, itemPlacement.PlacedItems, Overworld, PlacementContext, WeaponPermissions, ArmorPermissions, flags).WriteSpoiler();

		// Write back everything
		ItemsText.Write(this, flags.GameMode == GameModes.DeepDungeon ? new List<Item>() : ItemLists.UnusedGoldItems.ToList());
		TalkRoutines.Write(this);
		NpcData.Write(TalkRoutines.ScriptPointers);
		Dialogues.Write();

		EncounterRates.Write();
		itemPlacement.Write();

		Maps.Write();
		TileSetsData.Write();
		ZoneFormations.Write(this);
		StartingItems.Write();
		RngTables.Write(this);
		Teleporters.Write();
		Overworld.Write();
		ArmorPermissions.Write(this);
		WeaponPermissions.Write(this);
		SpellPermissions.Write(this);
		ClassData.Write(this);

		await this.Progress();

		uint last_rng_value = rng.Next();

		// Final ROM writes
		//WriteSeedAndFlags(seed.ToHex(), flags, flagsForRng, unmodifiedFlags, "ressourcePackHash", last_rng_value);
		WriteSeedAndFlags(seed.ToHex(), flags, flagsForRng, unmodifiedFlags, resourcesPackHash.ToHex(), last_rng_value);
		if (flags.TournamentSafe) Put(0x3FFE3, Blob.FromHex("66696E616C2066616E74617379"));

		await this.Progress("Randomization Completed");
	}
}
