// using FF1Lib.Sanity;

// namespace FF1Lib
// {
// 	public class ExtFlags
// 	{
// 		private static HashSet<int> LegendaryShopIndices = new HashSet<int>(new int[] { 6, 16, 7, 17, 66 });

// 		private FF1Rom rom;
// 		private SanityCheckerV2 checker;
// 		private ShopData shopData;
// 		private ItemNames itemsText;
// 		private List<IRewardSource> itemPlacement;
// 		private OverworldMap overworldMap;
// 		private PlacementContext incentivesData;
// 		private Overworld overworld;
// 		private GearPermissions weaponPermissions;
// 		private GearPermissions armorPermissions;
// 		private Flags flags;

// 		private SCLogic logic;
// 		private List<Weapon> weapons;
// 		private List<Armor> armors;
// 		private List<MagicSpell> magicSpells;

// 		public ExtFlags(FF1Rom _rom, SanityCheckerV2 _checker, ShopData _shopData, ItemNames _itemsText, List<IRewardSource> _itemPlacement, Overworld _overworld, PlacementContext _incentivesData, GearPermissions _weaponPermissions, GearPermissions _armorPermissions, Flags _flags)
// 		{
// 			rom = _rom;
// 			checker = _checker;
// 			shopData = _shopData;
// 			itemsText = _itemsText;
// 			itemPlacement = _itemPlacement;
// 			overworld = _overworld;
// 			overworldMap = _overworld.OverworldMap;
// 			incentivesData = _incentivesData;
// 			weaponPermissions = _weaponPermissions;
// 			armorPermissions = _armorPermissions;
// 			flags = _flags;

// 			logic = new SCLogic(rom, checker.Main, itemPlacement, overworld.Locations, flags, false);
// 			weapons = Weapon.LoadAllWeapons(rom, flags).ToList();
// 			armors = Armor.LoadAllArmors(rom, flags).ToList();
// 			magicSpells = rom.GetSpells();
// 		}

// 		public void WriteFlags()
// 		{
// 			Utilities.WriteFlagLine("Collapsed TriStates:");
// 			Utilities.WriteFlagLine(flags.Encoded);

// 			// WriteGoalFlags();
// 			// WriteDifficultyFlags();
// 			// WriteIncentiveFlags();
// 			// WriteModeFlags();
// 			// WriteMapFlags();
// 			// WriteTreasureFlags();
// 			// WriteShopFlags();
// 			// WriteEnemyFlags();
// 			// WritePartyFlags();
// 			// WriteClassFlags();
// 			// WriteEquipmentFlags();
// 			// WriteAdjustmentFlags();
// 			// WriteExperimentalFlags();
// 		}

// 		private void WriteGoalFlags()
// 		{
// 			Utilities.WriteFlagLine("Goal Flags");
// 			Utilities.WriteFlagLine("-----------------");
// 			if (!flags.ShardHunt) {
// 				Utilities.WriteFlagLine("Orbs of Light");
// 				Utilities.WriteFlagLine(
// 					"Orbs Required - " + flags.OrbsRequiredCount + " Orbs Required Mode - " + flags.OrbsRequiredMode +  " Orb Spoilers - " + flags.OrbsRequiredSpoilers
// 				);
// 			} else {
// 				Utilities.WriteFlagLine("Shard Hunt");
// 				Utilities.WriteFlagLine(
// 					"Shard Hunt - " + flags.ShardHunt + " Shard Hunt Count - " + flags.ShardCount
// 				);
// 			}

// 			Utilities.WriteFlagLine("Final Boss");
// 			if (flags.TrappedChaos == true) Utilities.WriteFlagLine("Hidden Chaos - " + flags.TrappedChaos);
// 			if (flags.SpookyFlag == true && flags.RandomizeFormationEnemizer == false) {
// 				Utilities.WriteFlagLine(
// 					"Lich's Revenge - " + flags.SpookyFlag
// 				);
// 			} else {
// 				Utilities.WriteFlagLine(
// 					"Alternate Final Boss - " + flags.TransformFinalFormation
// 				);
// 			}

// 			Utilities.WriteFlagLine("Temple of Fiends");
// 			Utilities.WriteFlagLine(
// 				"Mode - " + flags.ToFRMode + " Fiends Refights - " + flags.FiendsRefights
// 			);
// 			string tempStr = "";
// 			if (flags.ChaosRush == true) tempStr += "Unlocked ToFR - " + flags.ChaosRush + " ";
// 			if (flags.ExitToFR == true) tempStr += "ExitToFR - " + flags.ExitToFR + " ";
// 			if (flags.ChaosFloorEncounters == true) tempStr += "Chaos Floor Encounters - " + flags.ChaosFloorEncounters;
// 			Utilities.WriteFlagLine(tempStr);

// 			if (flags.Archipelago) {
// 				Utilities.WriteFlagLine("Archipelago");
// 				Utilities.WriteFlagLine(
// 				// Do I really need to output enabled?
// 					"Enable - " + flags.Archipelago + " Shards - " + flags.ArchipelagoShards + " Gold - " + flags.ArchipelagoGold + " Consumables - " + flags.ArchipelagoConsumables + " Equipment - " + flags.ArchipelagoEquipment
// 				);
// 			}

// 			Utilities.WriteFlagLine("");
// 		}

// 		private void WriteDifficultyFlags()
// 		{
// 			Utilities.WriteFlagLine("Difficulty Flags");
// 			Utilities.WriteFlagLine("-----------------");

// 			// Utilities.WriteFlagLine("Gold");
// 			Utilities.WriteFlagLine(
// 				"Price Scaling " + flags.PriceScaleFactorLow + "% - " + flags.PriceScaleFactorHigh + "%"
// 			);
// 			Utilities.WriteFlagLine("Starting Gold - " + flags.StartingGold);
// 			string tempStr = "";
// 			if (flags.ExcludeGoldFromScaling == true){
// 				tempStr = "Exclude Gold Scaling - " + flags.ExcludeGoldFromScaling;
// 				if (flags.CheapVendorItem) tempStr += " Cheap Vendor Item - " + flags.CheapVendorItem;
// 			}
// 			Utilities.WriteFlagLine(tempStr);

// 			// Utilities.WriteFlagLine("Enemies");
// 			Utilities.WriteFlagLine("Enemy Stats");
// 			Utilities.WriteFlagLine(
// 				"Enemy Stats " + flags.EnemyScaleStatsLow + "% - " + flags.EnemyScaleStatsHigh + "%"
// 			);
// 			if (flags.SeparateEnemyHPScaling == true){
// 				Utilities.WriteFlagLine(
// 					"Enemy HP " + flags.EnemyScaleHpLow + "% - " + flags.EnemyScaleHpHigh + "%"
// 				);
// 			}
// 			Utilities.WriteFlagLine("Boss Stats");
// 			Utilities.WriteFlagLine(
// 				"Boss Stats " + flags.BossScaleStatsLow + "% - " + flags.BossScaleStatsHigh + "%"
// 			);
// 			if (flags.SeparateBossHPScaling == true)
// 			Utilities.WriteFlagLine(
// 				"Boss HP " + flags.BossScaleHpLow + "% - " + flags.BossScaleHpHigh + "%"
// 			);
// 			Utilities.WriteFlagLine(
// 				"Evade Cap Value - " + flags.EvadeCap
// 			);
// 			if (flags.IncludeMorale) Utilities.WriteFlagLine("Include Morale - " + flags.IncludeMorale);

// 			double landEncounterRate  = Math.Ceiling(10.0 * (flags.EncounterRate/30.0));
// 			double seaEncounterRate  = Math.Ceiling(3.0 * (flags.EncounterRate/30.0));
// 			double dungeonEncounterRate  = Math.Ceiling(8.0 * (flags.DungeonEncounterRate/30.0));

// 			Utilities.WriteFlagLine("Encounter Rates");
// 			Utilities.WriteFlagLine(
// 				"Land Encounter Rate - " + landEncounterRate + "/256"
// 			);
// 			Utilities.WriteFlagLine(
// 				"Sea Encounter Rate - " + seaEncounterRate + "/256"
// 			);
// 			Utilities.WriteFlagLine(
// 				"Dungeon Encounter Rate - " + dungeonEncounterRate + "/256"
// 			);
// 			Utilities.WriteFlagLine("Experience");
// 			Utilities.WriteFlagLine(
// 				"Multiplier - " + flags.ExpMultiplier + " Flat Gold Gain - " + flags.ExpBonus
// 			);
// 			Utilities.WriteFlagLine(
// 				"Scaling - " + flags.ProgressiveScaleMode
// 			);
// 			Utilities.WriteFlagLine(
// 				"Nones Gain XP - " + flags.NonesGainXP + " Deads Gain XP - " + flags.DeadsGainXP
// 			);
// 			Utilities.WriteFlagLine(
// 				"XP Chests Conversion " + flags.ExpChestConversionMin + "% - " + flags.ExpChestConversionMax + "%"
// 			);
// 			if (flags.ExpChestConversionMax != 0){
// 				Utilities.WriteFlagLine(
// 					"XP Chests Reward " + flags.ExpChestMinReward + "% - " + flags.ExpChestMaxReward + "%"
// 				);
// 			}

// 			Utilities.WriteFlagLine("");
// 		}

// 		private void WriteIncentiveFlags()
// 		{
// 			Utilities.WriteFlagLine("Incentived Locations and Items Flags");
// 			Utilities.WriteFlagLine("-----------------");
// 			Utilities.WriteFlagLine("Locations");
// 			int locCount = 0;
// 			string tempStr = "";
// 			if (flags.IncentivizeFreeNPCs == true){
// 				tempStr += "Main NPCs - " + flags.IncentivizeFreeNPCs + " ";
// 				locCount = locCount + 7;
// 			}
// 			if (flags.IncentivizeFetchNPCs == true){
// 				tempStr += "Fetch Quest NPCs - " + flags.IncentivizeFetchNPCs + " ";
// 				locCount = locCount + 7;
// 			}
// 			if (flags.IncentivizeIceCave == true){
// 				tempStr += "Ice Cave - " + flags.IncentivizeIceCave + " Ice Cave Placement - " + flags.IceCaveIncentivePlacementType + " ";
// 				locCount++;
// 			}
// 			if (flags.IncentivizeOrdeals == true){
// 				tempStr += "Ordeals - " + flags.IncentivizeOrdeals + " Ordeals Placement - " + flags.OrdealsIncentivePlacementType + " ";
// 				locCount++;
// 			}
// 			if (flags.IncentivizeMarsh == true){
// 				tempStr += "Marsh Cave - " + flags.IncentivizeMarsh + " Marsh Cave Placement- " + flags.MarshIncentivePlacementType + " ";
// 				locCount++;
// 			}
// 			if (flags.IncentivizeTitansTrove == true){
// 				tempStr += "Titan's Trove - " + flags.IncentivizeTitansTrove + " Titan's Trove Placement - " + flags.TitansIncentivePlacementType + " ";
// 				locCount++;
// 			}
// 			if (flags.IncentivizeEarth == true){
// 				tempStr += "Earth Cave - " + flags.IncentivizeEarth + " Earth Cave Placement - " + flags.EarthIncentivePlacementType + " ";
// 				locCount++;
// 			}
// 			if (flags.IncentivizeVolcano == true){
// 				tempStr += "Volcano - " + flags.IncentivizeVolcano + " Volcano Placement- " + flags.VolcanoIncentivePlacementType + " ";
// 				locCount++;
// 			}
// 			if (flags.IncentivizeSeaShrine == true){
// 				tempStr += "Sea Shrine - " + flags.IncentivizeSeaShrine + " Sea Shrine Placement - " + flags.SeaShrineIncentivePlacementType + " ";
// 				locCount++;
// 			}
// 			if (flags.IncentivizeSkyPalace == true){
// 				tempStr += "Sky Palace - " + flags.IncentivizeSkyPalace + " Sky Palace Placement - " + flags.SkyPalaceIncentivePlacementType + " ";
// 				locCount++;
// 			}
// 			if (flags.IncentivizeConeria == true){
// 				tempStr += "Coneria (Locked) - " + flags.IncentivizeConeria + " Coneria (Locked) Placement - " + flags.CorneriaIncentivePlacementType + " ";
// 				locCount++;
// 			}
// 			if (flags.IncentivizeMarshKeyLocked == true){
// 				tempStr += "Marsh (Locked) - " + flags.IncentivizeMarshKeyLocked + " Marsh (Locked) Placement - " + flags.MarshLockedIncentivePlacementType + " ";
// 				locCount++;
// 			}
// 			if (flags.IncentivizeCardia == true){
// 				tempStr += "Cardia Islands - " + flags.IncentivizeCardia + " Cardia Islands Placement - " + flags.CardiaIncentivePlacementType;
// 				locCount++;
// 			}				 
// 			Utilities.WriteFlagLine(tempStr);
			
// 			Utilities.WriteFlagLine("Items");
// 			int itemCount = 0;
// 			tempStr = "";
// 			if (flags.IncentivizeMainItems == true){
// 				tempStr += "Progression Items - " + flags.IncentivizeMainItems + " ";
// 				itemCount = itemCount + 6;
// 			}
// 			if (flags.IncentivizeFetchItems == true){
// 				tempStr += "Other Quest Items - " + flags.IncentivizeFetchItems + " ";
// 				itemCount = itemCount + 8;
// 			}
// 			if (flags.IncentivizeAirship == true){
// 				tempStr += "Floater - " + flags.IncentivizeAirship + " ";
// 				itemCount++;
// 			}
// 			if (flags.IncentivizeCanoeItem == true){
// 				tempStr += "Canoe - " + flags.IncentivizeCanoeItem + " ";
// 				itemCount++;
// 			}
// 			if (flags.IncentivizeShipAndCanal == true){
// 				tempStr += "Ship & Canal - " + flags.IncentivizeShipAndCanal + " ";
// 				itemCount = itemCount + 2;
// 			}
// 			if (flags.IncentivizeBridgeItem == true){
// 				tempStr += "Bridge - " + flags.IncentivizeBridgeItem + " ";
// 				itemCount++;
// 			}
// 			if (flags.IncentivizeTail == true){
// 				tempStr += "Tail - " + flags.IncentivizeTail + " ";
// 				itemCount++;
// 			}
// 			if (flags.IncentivizeMasamune == true){
// 				tempStr += "Masamune - " + flags.IncentivizeMasamune + " ";
// 				itemCount++;
// 			}
// 			if (flags.IncentivizeXcalber == true){
// 				tempStr += "Xcalber - " + flags.IncentivizeXcalber + " ";
// 				itemCount++;
// 			}
// 			if (flags.IncentivizeKatana == true){
// 				tempStr += "Katana - " + flags.IncentivizeKatana + " ";
// 				itemCount++;
// 			}
// 			if (flags.IncentivizeVorpal == true){
// 				tempStr += "Vorpal - " + flags.IncentivizeVorpal + " ";
// 				itemCount++;
// 			}
// 			if (flags.IncentivizeDefCastWeapon == true){
// 				tempStr += "Defense Sword - " + flags.IncentivizeDefCastWeapon + " ";
// 				itemCount++;
// 			}
// 			if (flags.IncentivizeOffCastWeapon == true){
// 				tempStr += "Thor Hammer - " + flags.IncentivizeOffCastWeapon + " ";
// 				itemCount++;
// 			}
// 			if (flags.IncentivizeOpal == true){
// 				tempStr += "Opal Bracelet - " + flags.IncentivizeOpal + " ";
// 				itemCount++;
// 			}
// 			if (flags.IncentivizeOtherCastArmor == true){
// 				tempStr += "Power Gauntlet - " + flags.IncentivizeOtherCastArmor + " ";
// 				itemCount++;
// 			}
// 			if (flags.IncentivizePowerRod == true){
// 				tempStr += "Power Staff - " + flags.IncentivizePowerRod + " ";
// 				itemCount++;
// 			}
// 			if (flags.IncentivizeDefCastArmor == true){
// 				tempStr += "White Shirt - " + flags.IncentivizeDefCastArmor + " ";
// 				itemCount++;
// 			}
// 			if (flags.IncentivizeOffCastArmor == true){
// 				tempStr += "Black Shirt - " + flags.IncentivizeOffCastArmor + " ";
// 				itemCount++;
// 			}
// 			if (flags.IncentivizeRibbon == true){
// 				tempStr += "Ribbon - " + flags.IncentivizeRibbon;
// 				itemCount++;
// 			}
// 			Utilities.WriteFlagLine(tempStr);

// 			if (itemCount > locCount || flags.IncentivizeMainItems == false || flags.IncentivizeFetchItems == false || flags.IncentivizeAirship == false || flags.IncentivizeCanoeItem == false || flags.IncentivizeShipAndCanal == false || flags.IncentivizeBridgeItem == false || flags.IncentivizeTail == false){
// 				Utilities.WriteFlagLine("Loose Placement");
// 				tempStr = "Mode - " + flags.LoosePlacementMode + " ";
// 				if (flags.LaterLoose == true) tempStr += "Later Loose - " + flags.LaterLoose + " ";
// 				if (flags.IncentivizeFreeNPCs == false || flags.IncentivizeFetchNPCs == false){
// 					tempStr += "Favor NPCs - " + flags.LooseItemsNpcBalance + " ";
// 				}
// 				if (flags.LooseExcludePlacedDungeons == true) tempStr += "No Loose Items in Incentivized Dungeons - " + flags.LooseExcludePlacedDungeons;
// 				Utilities.WriteFlagLine(tempStr);
// 			}

// 			if (flags.AllowUnsafePlacement || flags.ShipCanalBeforeFloater) {
// 				Utilities.WriteFlagLine("Key Items Placement");
// 				if (flags.AllowUnsafePlacement)	Utilities.WriteFlagLine("Unsafe Placement - " + flags.AllowUnsafePlacement);
// 				if (flags.ShipCanalBeforeFloater) Utilities.WriteFlagLine("Ship & Canal Before Floater - " + flags.ShipCanalBeforeFloater);
// 			}

// 			if (flags.GameMode != GameModes.DeepDungeon && !flags.Archipelago){
// 				Utilities.WriteFlagLine("Hints");
// 				Utilities.WriteFlagLine(
// 					"Hint Givers in Towns - " + flags.HintsVillage
// 				);
// 			}

// 			Utilities.WriteFlagLine("");
// 		}

// 		private void WriteModeFlags()
// 		{
// 			Utilities.WriteFlagLine("Game Mode & Overworld Flags");
// 			Utilities.WriteFlagLine("-----------------");
// 			Utilities.WriteFlagLine("Game Mode - " + flags.GameMode);
// 			if (flags.GameMode == GameModes.NoOverworld){
// 			} else if (flags.GameMode == GameModes.Standard){
// 				if (flags.OwMapExchange == OwMapExchanges.None){
// 					Utilities.WriteFlagLine(
// 						"Overworld Map - " + flags.OwMapExchange
// 					);
// 					// Vanilla Map
// 					Utilities.WriteFlagLine("Overworld Edits");
// 					string tempStr = "";
// 					if (flags.MapOpenProgression == true) tempStr += "Early Open Progression - " + flags.MapOpenProgression + " ";
// 					if (flags.MapOpenProgression == true || flags.MapOpenProgressionExtended == true) tempStr += "Extended Open Progression - " + flags.MapOpenProgressionExtended + " ";
// 					if (flags.MapOpenProgressionDocks == true) tempStr += "Northern Docks - " + flags.MapOpenProgressionDocks + " ";
// 					if (flags.MapOpenProgression == true || flags.MapAirshipDock == true) tempStr += "Ryukahn Desert Dock - " + flags.MapAirshipDock + " ";
// 					if (flags.MapBahamutCardiaDock == true) tempStr += "Bahamut Cardia Dock - " + flags.MapBahamutCardiaDock + " ";
// 					if (flags.MapLefeinRiver == true) tempStr += "Lefein River Dock - " + flags.MapLefeinRiver + " ";
// 					if (flags.MapBridgeLefein == true) tempStr += "Bridge to Lefein - " + flags.MapBridgeLefein + " ";
// 					if (flags.MapGaiaMountainPass == true) tempStr += "Gaia Mountain Pass - " + flags.MapGaiaMountainPass + " ";
// 					if (flags.MapHighwayToOrdeals == true) tempStr += "Highway to Ordeals - " + flags.MapHighwayToOrdeals + " ";
// 					if (flags.MapRiverToMelmond == true) tempStr += "River to Melmond - " + flags.MapRiverToMelmond;
// 					Utilities.WriteFlagLine(tempStr);
// 				} else if (flags.OwMapExchange == OwMapExchanges.GenerateNewOverworld || flags.OwMapExchange == OwMapExchanges.LostWoods || flags.OwMapExchange == OwMapExchanges.Desert){
// 					Utilities.WriteFlagLine("Map Generation Parameters");
// 					Utilities.WriteFlagLine("Map Generation Seed - " + flags.MapGenSeed);

// 					if (flags.OwMapExchange == OwMapExchanges.GenerateNewOverworld){
// 						Utilities.WriteFlagLine(
// 							"Use Pregenerated Map - " + flags.OwRandomPregen + " Shuffled Access - " + flags.OwShuffledAccess + " Unsafe Start - " + flags.OwUnsafeStart
// 						);
// 					}						
// 				} else if (flags.OwMapExchange == OwMapExchanges.ImportCustomMap){
// 					// Import Custom Map
// 					Utilities.WriteFlagLine("Custom Map - " + flags.ReplacementMap);	
// 				}

// 				Utilities.WriteFlagLine("Other Map Changes");
// 				Utilities.WriteFlagLine(
// 					"Shuffle Chime Access - " + flags.ShuffleChimeAccess + " Include Towns - " + flags.ShuffleChimeIncludeTowns
// 				);

// 				if (flags.OwMapExchange != OwMapExchanges.Desert){
// 					if (flags.DamageTileHigh > 0){
// 						Utilities.WriteFlagLine(
// 							"Damage Tile Low - " + flags.DamageTileLow + " Damage Tile High - " + flags.DamageTileHigh
// 						);
// 					}
// 					if (flags.OWDamageTiles == true) Utilities.WriteFlagLine("Marsh & Desert Tiles Deal Damage - " + flags.OWDamageTiles);
// 					if (flags.DamageTilesKill == true) Utilities.WriteFlagLine("Lethal Damage Tiles - " + flags.DamageTilesKill);
// 				}
// 			} else if (flags.GameMode == GameModes.DeepDungeon){
// 				// Deep Dungeon Options
// 				Utilities.WriteFlagLine("Deep Dungeon Options");
// 				Utilities.WriteFlagLine(
// 					"Progressive Tilesets - " + flags.DDProgressiveTilesets + " Fiend Orbs - " + flags.DDFiendOrbs + " Bahamut and Tail Floors - " + flags.TailBahamutMode
// 				);
// 			}

// 			Utilities.WriteFlagLine("");
// 		}

// 		private void WriteMapFlags()
// 		{
// 			Utilities.WriteFlagLine("Maps and Routing Flags");
// 			Utilities.WriteFlagLine("-----------------");
// 			if (flags.Entrances == true || flags.Towns == true) Utilities.WriteFlagLine("Entrance Shuffle");
// 			if (flags.ChestsKeyItems == true && flags.NPCItems == true && (flags.GameMode != GameModes.DeepDungeon)) Utilities.WriteFlagLine("Entrances - " + flags.Entrances);
// 			if (flags.Entrances == true && flags.GameMode == GameModes.Standard)	Utilities.WriteFlagLine("Allow Unsafe Start - " + flags.AllowUnsafeStartArea);
// 			if (flags.Entrances == true && flags.GameMode == GameModes.Standard)	Utilities.WriteFlagLine("Include Dead Ends - " + flags.EntrancesIncludesDeadEnds);
// 			if (flags.Entrances == true) Utilities.WriteFlagLine("Floors - " + flags.Floors);
// 			if (flags.DeepCastlesPossible == true ) Utilities.WriteFlagLine("Deep ToFR - " + flags.AllowDeepCastles);
// 			if (flags.NPCItems == true && (flags.GameMode != GameModes.DeepDungeon))	Utilities.WriteFlagLine(" Towns - " + flags.Towns);
// 			if (flags.Towns == true) Utilities.WriteFlagLine("Include Coneria - " + flags.IncludeConeria);
// 			if (flags.Towns == true && flags.Entrances == true) Utilities.WriteFlagLine("Mix All Entrances Together - " + flags.EntrancesMixedWithTowns);
// 			if (flags.DeepTownsPossible == true & flags.GameMode == GameModes.Standard)	Utilities.WriteFlagLine("Deep Towns - " + flags.AllowDeepTowns);
			
// 			Utilities.WriteFlagLine("Dungeons");
// 			Utilities.WriteFlagLine("Sky Castle 4F - " + flags.SkyCastle4FMazeMode);
// 			if (flags.OrdealsPillars == true) Utilities.WriteFlagLine("Castle Ordeals Pillars - " + flags.OrdealsPillars);
// 			if (flags.TitansTrove == true) Utilities.WriteFlagLine("Titan's Trove - " + flags.TitansTrove);
// 			if (flags.MapDragonsHoard == true) Utilities.WriteFlagLine("Bahamut's Hoard - " + flags.MapDragonsHoard);
// 			if (flags.MapHallOfDragons == true) Utilities.WriteFlagLine("Hall of Dragons - " + flags.MapHallOfDragons);
// 			if (flags.MermaidPrison == true) Utilities.WriteFlagLine("Mermaid Prison - " + flags.MermaidPrison);
// 			if (flags.EFGWaterfall == true) Utilities.WriteFlagLine("Generated Waterfall Cave - " + flags.EFGWaterfall);
// 			if (flags.FlipDungeons == true) Utilities.WriteFlagLine("Flip Dungeons Horizontally - " + flags.FlipDungeons);
// 			if (flags.GameMode == GameModes.Standard) {
// 				if (flags.VerticallyFlipDungeons == true) Utilities.WriteFlagLine("Flip Dungeons Vertically - " + flags.VerticallyFlipDungeons);
// 				if (flags.ReversedFloors == true) Utilities.WriteFlagLine("Swap Stair Locations - " + flags.ReversedFloors);
// 			}
// 			if (flags.RelocateChests == true) Utilities.WriteFlagLine("Relocate Chests and Trap Tiles - " + flags.RelocateChests);
// 			if (flags.RelocateChests == true && flags.RelocateChestsTrapIndicator == true) Utilities.WriteFlagLine("Relocated Trap Tiles Are Marked - " + flags.RelocateChestsTrapIndicator);
// 			if (flags.MoveToFBats == true) Utilities.WriteFlagLine("Move Temple of Fiends Bats - " + flags.MoveToFBats);

// 			if (flags.EarlyKing == true || flags.EarlySarda == true || flags.EarlySage == true || flags.EarlyOrdeals == true){
// 				Utilities.WriteFlagLine("Progression");
// 				if (flags.EarlyKing == true) Utilities.WriteFlagLine("Early King Item - " + flags.EarlyKing);
// 				if (flags.EarlySarda == true) Utilities.WriteFlagLine("Early Sarda Item - " + flags.EarlySarda);
// 				if (flags.EarlySage == true) Utilities.WriteFlagLine("Early Sage Item - " + flags.EarlySage);
// 				if (flags.EarlyOrdeals == true) Utilities.WriteFlagLine("Early Ordeals - " + flags.EarlyOrdeals);
// 			}

// 			if (flags.AllowUnsafeMelmond == true || flags.MelmondClinic == true || flags.ConfusedOldMen == true || flags.GaiaShortcut == true
// 			 || flags.MoveGaiaItemShop == true || flags.LefeinShops == true || flags.LefeinSuperStore == true || flags.ShufflePravokaShops == true
// 			 || flags.RandomVampAttack == true || flags.RandomVampAttackIncludesConeria == true){
// 				Utilities.WriteFlagLine("Towns");
// 				if (flags.AllowUnsafeMelmond == true) Utilities.WriteFlagLine("Zozo Melmond - " + flags.AllowUnsafeMelmond);
// 				if (flags.MelmondClinic == true) Utilities.WriteFlagLine("Melmondish Hospitiality - " + flags.MelmondClinic);
// 				if (flags.ConfusedOldMen == true) Utilities.WriteFlagLine("Confused Old Men - " + flags.ConfusedOldMen);
// 				if (flags.GaiaShortcut == true) Utilities.WriteFlagLine("Add Gaia Shortcut - " + flags.GaiaShortcut);
// 				if (flags.GaiaShortcut == true && flags.MoveGaiaItemShop == true) Utilities.WriteFlagLine("Move Gaia Item Shop - " + flags.MoveGaiaItemShop);
// 				if (flags.LefeinShops == true) Utilities.WriteFlagLine("Lefeinish Hospitality - " + flags.LefeinShops);
// 				if (flags.LefeinSuperStoreEnabled == true && flags.LefeinSuperStore == true) Utilities.WriteFlagLine("Add Lefein SuperStore - " + flags.LefeinSuperStore);
// 				if (flags.ShufflePravokaShops == true) Utilities.WriteFlagLine("Shuffle Pravoka's Shops - " + flags.ShufflePravokaShops);
// 				if (flags.RandomVampAttack == true) Utilities.WriteFlagLine("Vampire Attacks Random Town - " + flags.RandomVampAttack);
// 				if (flags.RandomVampAttack == true && flags.RandomVampAttackIncludesConeria == true) Utilities.WriteFlagLine(" Include Coneria - " + flags.RandomVampAttackIncludesConeria);
// 			}
// 			Utilities.WriteFlagLine("");
// 		}

// 		private void WriteTreasureFlags()
// 		{
// 			Utilities.WriteFlagLine("Treasures and Freebies Flags");
// 			Utilities.WriteFlagLine("-----------------");
// 			if (flags.Treasures == true || flags.BetterTrapChests == true || flags.ChestsKeyItems == true || flags.ShuffleObjectiveNPCs == true ||
// 				flags.NPCItems == true || flags.NPCFetchItems == true){
// 				Utilities.WriteFlagLine("Item Shuffle");
// 				if (flags.Treasures == true) Utilities.WriteFlagLine("Treasures - " + flags.Treasures);
// 				if (flags.Treasures == true || flags.BetterTrapChests == true) Utilities.WriteFlagLine("Better Trap Treasure - " + flags.BetterTrapChests);
// 				if (flags.ChestsKeyItems == true) Utilities.WriteFlagLine("Chests Key Items - " + flags.ChestsKeyItems);
// 				if (flags.ShuffleObjectiveNPCs == true || flags.ChestsKeyItems == true) Utilities.WriteFlagLine("Objective NPCs - " + flags.ShuffleObjectiveNPCs);
// 				if (flags.NPCItems == true) Utilities.WriteFlagLine("Main NPC Items - " + flags.NPCItems);
// 				if (flags.NPCFetchItems == true) Utilities.WriteFlagLine("Fetch Quest Rewards - " + flags.NPCFetchItems);
// 				if (flags.Treasures == true) Utilities.WriteFlagLine("Randomize Treasures - " + flags.RandomizeTreasure);
// 				// if (flags.WorldWealth == true) Utilities.WriteFlagLine("Wealth - " + flags.WorldWealth);
// 				// if (flags.WorldWealth == true) Utilities.WriteFlagLine("Wealth - " + flags.DeepDungeonGenerator);
// 			}

// 			// if (){
// 			// Utilities.WriteFlagLine("Trapped Chests");
// 			// Utilities.WriteFlagLine(
// 			// 	"Formations - " + flags.TCFormations + 
// 			// 	"Trapped Chests - " + flags.TCChestCount + 
// 			// 	"Rare Treasures - " + flags.TCBetterTreasure + 
// 			// 	"Key Items - " + flags.TCKeyItems +
// 			// 	"Shards - " + flags.TCShards + 
// 			// 	"Exclude Common Treasures - " + flags.TCExcludeCommons + 
// 			// 	"Exclude Incentivized Items - " + flags.TCProtectIncentives + 
// 			// 	"Trapped Chests are Marked - " + flags.TCIndicator +
// 			// 	"Mode - " + flags.DeepDungeonGenerator
// 			// );
// 			// Utilities.WriteFlagLine(
// 			// 	"Formations - " + flags.TCFormations + 
// 			// 	"Trapped Chests - " + flags.TCChestCount + 
// 			// 	"Rare Treasures - " + flags.TCBetterTreasure + 
// 			// 	"Key Items - " + flags.TCKeyItems +
// 			// 	"Shards - " + flags.TCShards + 
// 			// 	"Exclude Common Treasures - " + flags.TCExcludeCommons + 
// 			// 	"Exclude Incentivized Items - " + flags.TCProtectIncentives + 
// 			// 	"Trapped Chests are Marked - " + flags.TCIndicator +
// 			// 	"Mode - " + flags.DeepDungeonGenerator
// 			// );
// 			// Utilities.WriteFlagLine(
// 			// 	"Formations - " + flags.TCFormations + 
// 			// 	"Trapped Chests - " + flags.TCChestCount + 
// 			// 	"Rare Treasures - " + flags.TCBetterTreasure + 
// 			// 	"Key Items - " + flags.TCKeyItems +
// 			// 	"Shards - " + flags.TCShards + 
// 			// 	"Exclude Common Treasures - " + flags.TCExcludeCommons + 
// 			// 	"Exclude Incentivized Items - " + flags.TCProtectIncentives + 
// 			// 	"Trapped Chests are Marked - " + flags.TCIndicator +
// 			// 	"Mode - " + flags.DeepDungeonGenerator
// 			// );
// 			// Utilities.WriteFlagLine(
// 			// 	"Formations - " + flags.TCFormations + 
// 			// 	"Trapped Chests - " + flags.TCChestCount + 
// 			// 	"Rare Treasures - " + flags.TCBetterTreasure + 
// 			// 	"Key Items - " + flags.TCKeyItems +
// 			// 	"Shards - " + flags.TCShards + 
// 			// 	"Exclude Common Treasures - " + flags.TCExcludeCommons + 
// 			// 	"Exclude Incentivized Items - " + flags.TCProtectIncentives + 
// 			// 	"Trapped Chests are Marked - " + flags.TCIndicator +
// 			// 	"Mode - " + flags.DeepDungeonGenerator
// 			// );
// 			// Utilities.WriteFlagLine(
// 			// 	"Formations - " + flags.TCFormations + 
// 			// 	"Trapped Chests - " + flags.TCChestCount + 
// 			// 	"Rare Treasures - " + flags.TCBetterTreasure + 
// 			// 	"Key Items - " + flags.TCKeyItems +
// 			// 	"Shards - " + flags.TCShards + 
// 			// 	"Exclude Common Treasures - " + flags.TCExcludeCommons + 
// 			// 	"Exclude Incentivized Items - " + flags.TCProtectIncentives + 
// 			// 	"Trapped Chests are Marked - " + flags.TCIndicator +
// 			// 	"Mode - " + flags.DeepDungeonGenerator
// 			// );
// 			// Utilities.WriteFlagLine(
// 			// 	"Formations - " + flags.TCFormations + 
// 			// 	"Trapped Chests - " + flags.TCChestCount + 
// 			// 	"Rare Treasures - " + flags.TCBetterTreasure + 
// 			// 	"Key Items - " + flags.TCKeyItems +
// 			// 	"Shards - " + flags.TCShards + 
// 			// 	"Exclude Common Treasures - " + flags.TCExcludeCommons + 
// 			// 	"Exclude Incentivized Items - " + flags.TCProtectIncentives + 
// 			// 	"Trapped Chests are Marked - " + flags.TCIndicator +
// 			// 	"Mode - " + flags.DeepDungeonGenerator
// 			// );
// 			// Utilities.WriteFlagLine(
// 			// 	"Formations - " + flags.TCFormations + 
// 			// 	"Trapped Chests - " + flags.TCChestCount + 
// 			// 	"Rare Treasures - " + flags.TCBetterTreasure + 
// 			// 	"Key Items - " + flags.TCKeyItems +
// 			// 	"Shards - " + flags.TCShards + 
// 			// 	"Exclude Common Treasures - " + flags.TCExcludeCommons + 
// 			// 	"Exclude Incentivized Items - " + flags.TCProtectIncentives + 
// 			// 	"Trapped Chests are Marked - " + flags.TCIndicator +
// 			// 	"Mode - " + flags.DeepDungeonGenerator
// 			// );
// 			// Utilities.WriteFlagLine(
// 			// 	"Formations - " + flags.TCFormations + 
// 			// 	"Trapped Chests - " + flags.TCChestCount + 
// 			// 	"Rare Treasures - " + flags.TCBetterTreasure + 
// 			// 	"Key Items - " + flags.TCKeyItems +
// 			// 	"Shards - " + flags.TCShards + 
// 			// 	"Exclude Common Treasures - " + flags.TCExcludeCommons + 
// 			// 	"Exclude Incentivized Items - " + flags.TCProtectIncentives + 
// 			// 	"Trapped Chests are Marked - " + flags.TCIndicator +
// 			// 	"Mode - " + flags.DeepDungeonGenerator
// 			// );
// 			// Utilities.WriteFlagLine(
// 			// 	"Formations - " + flags.TCFormations + 
// 			// 	"Trapped Chests - " + flags.TCChestCount + 
// 			// 	"Rare Treasures - " + flags.TCBetterTreasure + 
// 			// 	"Key Items - " + flags.TCKeyItems +
// 			// 	"Shards - " + flags.TCShards + 
// 			// 	"Exclude Common Treasures - " + flags.TCExcludeCommons + 
// 			// 	"Exclude Incentivized Items - " + flags.TCProtectIncentives + 
// 			// 	"Trapped Chests are Marked - " + flags.TCIndicator +
// 			// 	"Mode - " + flags.DeepDungeonGenerator
// 			// );
// 			// }

// 			// if (){
// 			// Utilities.WriteFlagLine("Freebies");
// 			// Utilities.WriteFlagLine(
// 			// 	"Free Ship - " + flags.FreeShip + " Free Airship - " + flags.FreeAirship + " Free Bridge - " + flags.FreeBridge + " Free Canal - " + flags.FreeCanal +
// 			// 	" Free Canoe - " + flags.FreeCanoe + " Free Lute - " + flags.FreeLute + " Free Tail - " + flags.FreeTail + " Free Rod - " + flags.FreeRod
// 			// );
// 			// }


// 			// if (){
// 			// Utilities.WriteFlagLine("Bans");
// 			// Utilities.WriteFlagLine(
// 			// 	"Remove Tail - " + flags.NoTail + " Remove Masamune - " + flags.NoMasamune + " Remove Xcalber - " + flags.NoXcalber + " Remove Floater - " + flags.NoFloater
// 			// );
// 			// }

// 			// if (){
// 			// Utilities.WriteFlagLine("Masamune");
// 			// Utilities.WriteFlagLine(
// 			// 	"Guaranteed Endgame Masamune - " + flags.GuaranteedMasamune + " Send Masamune Home - " + flags.SendMasamuneHome + " WarMech, Guardian of Masamune - " + flags.TCMasaGuardian
// 			// );
// 			// }

// 			Utilities.WriteFlagLine("");
// 		}

// 		private void WriteShopFlags()
// 		{
// 			Utilities.WriteFlagLine("Shops and Magic Flags");
// 			Utilities.WriteFlagLine("-----------------");
// 			Utilities.WriteFlagLine("Shops");
// 			Utilities.WriteFlagLine(
// 				"Shop Shuffle - " + flags.Shops + " Random Weapons and Armor - " + flags.RandomWares + " Include Caster & Elite Gear - " + flags.RandomWaresIncludesSpecialGear +
// 				" Magic Shops - " + flags.MagicShops + " Magic Shop Locations - " + flags.MagicShopLocs + " Paired Shuffle - " + flags.MagicShopLocationPairs + " Improved Clinics - " + flags.ImprovedClinic
// 			);
// 			Utilities.WriteFlagLine("Legendary Shops");
// 			Utilities.WriteFlagLine(
// 				"Legendary Weapon Shop - " + flags.LegendaryWeaponShop + " Exclusive - " + flags.ExclusiveLegendaryWeaponShop + 
// 				" Legendary Armor Shop - " + flags.LegendaryArmorShop + " Exclusive - " + flags.ExclusiveLegendaryArmorShop + 
// 				" Legendary Black Shop - " + flags.LegendaryBlackShop + " Exclusive - " + flags.ExclusiveLegendaryBlackShop + 
// 				" Legendary White Shop - " + flags.LegendaryWhiteShop + " Exclusive - " + flags.ExclusiveLegendaryWhiteShop + 
// 				" Legendary Item Shop - " + flags.LegendaryItemShop + " Exclusive - " + flags.ExclusiveLegendaryItemShop
// 			);
// 			Utilities.WriteFlagLine("Shop Reduction");
// 			Utilities.WriteFlagLine("Weapons Shops");
// 			Utilities.WriteFlagLine(
// 				"Mode - " + flags.ShopKillMode_Weapons +
// 				" Percent - " + flags.ShopKillFactor_Weapons +
// 				" Exclude Coneria - " + flags.ShopKillExcludeConeria_Weapons
// 			);
// 			Utilities.WriteFlagLine("Armor Shops");
// 			Utilities.WriteFlagLine(
// 				"Mode - " + flags.ShopKillMode_Armor +
// 				" Percent - " + flags.ShopKillFactor_Armor +
// 				" Exclude Coneria - " + flags.ShopKillExcludeConeria_Armor
// 			);
// 			Utilities.WriteFlagLine("Item Shops");
// 			Utilities.WriteFlagLine(
// 				"Mode - " + flags.ShopKillMode_Item +
// 				" Percent - " + flags.ShopKillFactor_Item +
// 				" Exclude Coneria - " + flags.ShopKillExcludeConeria_Item
// 			);
// 			Utilities.WriteFlagLine("Black Magic");
// 			Utilities.WriteFlagLine(
// 				"Mode - " + flags.ShopKillMode_Black +
// 				" Percent - " + flags.ShopKillFactor_Black +
// 				" Exclude Coneria - " + flags.ShopKillExcludeConeria_Black
// 			);
// 			Utilities.WriteFlagLine("White Magic");
// 			Utilities.WriteFlagLine(
// 				"Mode - " + flags.ShopKillMode_White +
// 				" Percent - " + flags.ShopKillFactor_White +
// 				" Exclude Coneria - " + flags.ShopKillExcludeConeria_White
// 			);
// 			Utilities.WriteFlagLine("Magic");
// 			Utilities.WriteFlagLine(
// 				"Shuffle Magic Levels - " + flags.MagicLevels + " Tiered Shuffle - " + flags.MagicLevelsTiered + " Mix Spellbooks - " + flags.MagicLevelsMixed + " Keep Permissions - " + flags.MagicPermissions
// 			);
// 			Utilities.WriteFlagLine("Balance");
// 			Utilities.WriteFlagLine(
// 				"Improved Defensive Spells - " + flags.BuffHealingSpells + " Improved Offensive Spells - " + flags.BuffTier1DamageSpells + " Lock Mode - " + flags.LockMode +
// 				" Power Word Threshold - " + flags.MagicAutohitThreshold + " Soft in Battle - " + flags.EnableSoftInBattle + " Life in Battle - " + flags.EnableLifeInBattle
// 			);
// 			Utilities.WriteFlagLine("Spell Crafter");
// 			Utilities.WriteFlagLine(
// 				"Generate New Spellbooks - " + flags.GenerateNewSpellbook + " Retain Old Permissions - " + flags.SpellcrafterRetainPermissions + " Mix Spellbooks - " + flags.SpellcrafterMixSpells
// 			);
// 			Utilities.WriteFlagLine("Obfuscation");
// 			Utilities.WriteFlagLine(
// 				"Spell Name Obfuscation - " + flags.SpellNameMadness
// 			);

// 			Utilities.WriteFlagLine("");
// 		}

// 		private void WriteEnemyFlags()
// 		{
// 			Utilities.WriteFlagLine("Enemies, Formations, and Bosses Flags");
// 			Utilities.WriteFlagLine("-----------------");
// 			Utilities.WriteFlagLine("Formations");
// 			Utilities.WriteFlagLine(
// 				"Rarity - " + flags.FormationShuffleMode + " Shuffle Unrunnable Encounters - " + flags.UnrunnableShuffle + " Unrunnable Count Low - " + flags.UnrunnablesLow +
// 				" Unrunnable Count High - " + flags.UnrunnablesHigh + " Unrunnable Ambush/Preemptive - " + flags.UnrunnablesStrikeFirstAndSurprise + " Shuffle Surprise Bonus - " + flags.EnemyFormationsSurprise
// 			);
// 			Utilities.WriteFlagLine("Bosses");
// 			Utilities.WriteFlagLine(
// 				"Remove Boss Scripts - " + flags.RemoveBossScripts + " Shuffle Scripts - " + flags.ShuffleScriptsBosses + " Shuffle Skills & Spells - " + flags.ShuffleSkillsSpellsBosses +
// 				" Temple of Fiends Only - " + flags.TempleOfFiendRefightsOnly + " Overworld Fiends Only - " + flags.OverworldFiendsOnly + " No Consecutive NUKE or NUCLEAR - " + flags.NoConsecutiveNukes
// 			);
// 			Utilities.WriteFlagLine("Enemies");
// 			Utilities.WriteFlagLine(
// 				"Shuffle Scripts - " + flags.ShuffleScriptsEnemies + " Script Count - " + flags.ScriptMultiplier + " Shuffle Skills & Spells - " + flags.ShuffleSkillsSpellsEnemies +
// 				" Generate Balanced Scripts - " + flags.EnemySkillsSpellsTiered + " No Empty Scripts - " + flags.NoEmptyScripts
// 			);
// 			Utilities.WriteFlagLine("Status Attacks");
// 			Utilities.WriteFlagLine(
// 				"Mode - " + flags.TouchMode + " Count - " + flags.TouchMultiplier + " Pool - " + flags.TouchPool + " Include Bosses - " + flags.TouchIncludeBosses
// 			);
// 			Utilities.WriteFlagLine("Trap Tiles");
// 			Utilities.WriteFlagLine(
// 				"Pool - " + flags.EnemyTrapTiles
// 			);
// 			Utilities.WriteFlagLine("WarMech");
// 			Utilities.WriteFlagLine(
// 				"Mode - " + flags.WarMECHMode
// 			);
// 			Utilities.WriteFlagLine("Pirates");
// 			Utilities.WriteFlagLine(
// 				"Buffed - " + flags.SwolePirates + " Unsafe Script - " + flags.AllowUnsafePirates
// 			);
// 			Utilities.WriteFlagLine("Astos");
// 			Utilities.WriteFlagLine(
// 				"Buffed - " + flags.SwoleAstos + " Shuffle Location - " + flags.ShuffleAstos + " Unsafe - " + flags.UnsafeAstos
// 			);
// 			Utilities.WriteFlagLine("Fiends");
// 			Utilities.WriteFlagLine(
// 				"Shuffle - " + flags.FiendShuffle + " Alternate - " + flags.AlternateFiends + " FF1 Bonus - " + flags.FinalFantasy1BonusFiends + " FF2 - " + flags.FinalFantasy2Fiends + " FF3 - " + flags.FinalFantasy3Fiends +
// 				" FF4 - " + flags.FinalFantasy4Fiends + " FF5 - " + flags.FinalFantasy5Fiends + " FF6 - " + flags.FinalFantasy6Fiends + " Black Orb Rising - " + flags.BlackOrbFiends
// 			);
// 			Utilities.WriteFlagLine("Bahamut");
// 			Utilities.WriteFlagLine(
// 				"Fight - " + flags.FightBahamut + " Buffed - " + flags.SwoleBahamut + " Temple of Fiends Bats Give Boss Hints - " + flags.SkyWarriorSpoilerBats + " Early Spoilers - " + flags.SpoilerBatsDontCheckOrbs
// 			);
// 			Utilities.WriteFlagLine("Enemizer");
// 			Utilities.WriteFlagLine(
// 				"Generate New Formations - " + flags.RandomizeFormationEnemizer + " Generate New Enemies - " + flags.RandomizeEnemizer
// 			);

// 			Utilities.WriteFlagLine("");
// 		}

// 		private void WritePartyFlags()
// 		{
// 			Utilities.WriteFlagLine("Party Composition Flags");
// 			Utilities.WriteFlagLine("-----------------");
// 			Utilities.WriteFlagLine("Allowed Starting Classes");
// 			Utilities.WriteFlagLine(
// 				"#1 - Forced - " + flags.FORCED1 +
// 				" Fighter - " + flags.FIGHTER1 + " Thief - " + flags.THIEF1 + " Black Belt - " + flags.BLACK_BELT1 +
// 				" Red Mage - " + flags.RED_MAGE1 + " White Mage - " + flags.WHITE_MAGE1 + " Black Mage - " + flags.BLACK_MAGE1 +
// 				" Knight - " + flags.KNIGHT1 + " Ninja - " + flags.NINJA1 + " Master - " + flags.MASTER1 +
// 				" Red Wizard - " + flags.RED_WIZ1 + " White Wizard - " + flags.WHITE_WIZ1 + " Black Wizard - " + flags.BLACK_WIZ1
// 				);
// 			Utilities.WriteFlagLine(
// 					"#2 - Forced - " + flags.FORCED2 +
// 					" Fighter - " + flags.FIGHTER2 + " Thief - " + flags.THIEF2 + " Black Belt - " + flags.BLACK_BELT2 +
// 					" Red Mage - " + flags.RED_MAGE2 + " White Mage - " + flags.WHITE_MAGE2 + " Black Mage - " + flags.BLACK_MAGE2 +
// 					" Knight - " + flags.KNIGHT2 + " Ninja - " + flags.NINJA2 + " Master - " + flags.MASTER2 +
// 					" Red Wizard - " + flags.RED_WIZ2 + " White Wizard - " + flags.WHITE_WIZ2 + " Black Wizard - " + flags.BLACK_WIZ2 +
// 					" None - " + flags.NONE_CLASS2
// 				);
// 			Utilities.WriteFlagLine(
// 					"#3 - Forced - " + flags.FORCED3 +
// 					" Fighter - " + flags.FIGHTER3 + " Thief - " + flags.THIEF3 + " Black Belt - " + flags.BLACK_BELT3 +
// 					" Red Mage - " + flags.RED_MAGE3 + " White Mage - " + flags.WHITE_MAGE3 + " Black Mage - " + flags.BLACK_MAGE3 +
// 					" Knight - " + flags.KNIGHT3 + " Ninja - " + flags.NINJA3 + " Master - " + flags.MASTER3 +
// 					" Red Wizard - " + flags.RED_WIZ3 + " White Wizard - " + flags.WHITE_WIZ3 + " Black Wizard - " + flags.BLACK_WIZ3 +
// 					" None - " + flags.NONE_CLASS3
// 				);
// 			Utilities.WriteFlagLine(
// 					"#4 - Forced - " + flags.FORCED4 +
// 					" Fighter - " + flags.FIGHTER4 + " Thief - " + flags.THIEF4 + " Black Belt - " + flags.BLACK_BELT4 +
// 					" Red Mage - " + flags.RED_MAGE4 + " White Mage - " + flags.WHITE_MAGE4 + " Black Mage - " + flags.BLACK_MAGE4 +
// 					" Knight - " + flags.KNIGHT4 + " Ninja - " + flags.NINJA4 + " Master - " + flags.MASTER4 +
// 					" Red Wizard - " + flags.RED_WIZ4 + " White Wizard - " + flags.WHITE_WIZ4 + " Black Wizard - " + flags.BLACK_WIZ4 +
// 					" None - " + flags.NONE_CLASS4
// 				);
// 			Utilities.WriteFlagLine(
// 				"Party Draft - " + flags.EnablePoolParty + " Pool Size - " + flags.PoolSize + " Safe Party Draft - " + flags.SafePoolParty
// 			);
// 			Utilities.WriteFlagLine("Draftable");
// 			string Draftable = "";
// 				if (flags.FIGHTER1 == true || flags.FIGHTER2 == true || flags.FIGHTER3 == true || flags.FIGHTER4 == true) Draftable += "Fighter - " + flags.DraftFighter + " ";
// 				if (flags.THIEF1 == true || flags.THIEF2 == true || flags.THIEF3 == true || flags.THIEF4 == true) Draftable += "Thief - " + flags.DraftThief + " ";
// 				if (flags.BLACK_BELT1 == true || flags.BLACK_BELT2 == true || flags.BLACK_BELT3 ==true || flags.BLACK_BELT4 == true) Draftable += "Black Belt - " + flags.DraftBlackBelt + " ";
// 				if (flags.RED_MAGE1 == true || flags.RED_MAGE2 == true || flags.RED_MAGE3 == true || flags.RED_MAGE4 == true) Draftable += "Red Mage - " + flags.DraftRedMage + " ";
// 				if (flags.WHITE_MAGE1 == true || flags.WHITE_MAGE2 == true || flags.WHITE_MAGE3 == true || flags.WHITE_MAGE4 == true) Draftable += "White Mage - " + flags.DraftWhiteMage + " ";
// 				if (flags.BLACK_MAGE1 == true || flags.BLACK_MAGE2 == true || flags.BLACK_MAGE3 == true || flags.BLACK_MAGE4 == true) Draftable += "Black Mage - " + flags.DraftBlackMage + " ";
// 				if (flags.KNIGHT1 == true || flags.KNIGHT2 == true || flags.KNIGHT3 == true || flags.KNIGHT4 == true) Draftable += "Knight - " + flags.DraftKnight + " ";
// 				if (flags.NINJA1 == true || flags.NINJA2 == true || flags.NINJA3 == true || flags.NINJA4 == true) Draftable += "Ninja - " + flags.DraftNinja + " ";
// 				if (flags.MASTER1 == true || flags.MASTER2 == true || flags.MASTER3 == true || flags.MASTER4 == true) Draftable += "Master - " + flags.DraftMaster + " ";
// 				if (flags.RED_WIZ1 == true || flags.RED_WIZ2 == true || flags.RED_WIZ3 == true || flags.RED_WIZ4 == true) Draftable += "Red Wizard - " + flags.DraftRedWiz + " ";
// 				if (flags.WHITE_WIZ1 == true || flags.WHITE_WIZ2 == true || flags.WHITE_WIZ3 == true || flags.WHITE_WIZ4 == true) Draftable += "White Wizard - " + flags.DraftWhiteWiz + " ";
// 				if (flags.BLACK_WIZ1 == true || flags.BLACK_WIZ2 == true || flags.BLACK_WIZ3 == true || flags.BLACK_WIZ4 == true) Draftable += "Black Wizard - " + flags.DraftBlackWiz;
// 			Utilities.WriteFlagLine(Draftable);
// 			Utilities.WriteFlagLine("Tavern Recruitment");
// 			Utilities.WriteFlagLine(
// 				"Tavern Recruitment Mode - " + flags.RecruitmentMode + " Disable Reviving at Taverns - " + flags.RecruitmentModeHireOnly + " Only Replace Nones at Taverns - " + flags.RecruitmentModeReplaceOnlyNone
// 			);
// 			Utilities.WriteFlagLine("Recruitable");
// 			Utilities.WriteFlagLine(
// 				"Fighter - " + flags.TAVERN1 + " Thief - " + flags.TAVERN2 + " Black Belt - " + flags.TAVERN3 + " Red Mage - " + flags.TAVERN4 + " White Mage - " + flags.TAVERN5 + " Black Mage - " + flags.TAVERN6
// 			);
// 			Utilities.WriteFlagLine("NPC Recruits");
// 			Utilities.WriteFlagLine(
// 				"Recruits Gated by Fiends - " + flags.ClassAsNpcFiends + " Forced Recruits - " + flags.ClassAsNpcForcedFiends + " Recruits Linked to Key NPCs - " + flags.ClassAsNpcKeyNPC + " Recruit Counts - " + flags.ClassAsNpcCount + " Allow Duplicates - " + flags.ClassAsNpcDuplicate + " Promoted  Classes - " + flags.ClassAsNpcPromotion
// 			);

// 			Utilities.WriteFlagLine("");
// 		}

// 		private void WriteClassFlags()
// 		{
// 			Utilities.WriteFlagLine("Classes Flags");
// 			Utilities.WriteFlagLine("-----------------");
// 			Utilities.WriteFlagLine("Class Balance");
// 			Utilities.WriteFlagLine(
// 				"Halve BB Crit Rate - " + flags.BBCritRate + " Double Thief & Ninja Hit% Growth - " + flags.ThiefHitRate + " Better Thief & Ninja Agility - " + flags.ThiefAgilityBuff + " MDEF Growth - " + flags.MDefMode +
// 				" Improved HARM - " + flags.WhiteMageHarmEveryone + " Thief Lockpicking - " + flags.Lockpicking + " Lockpicking Level Requirement - " + flags.LockpickingLevelRequirement + " Reduced Luck - " + flags.ReducedLuck
// 			);
// 			Utilities.WriteFlagLine("Class Randomization");
// 			Utilities.WriteFlagLine(
// 				"Mode - " + flags.RandomizeClassMode
// 			);
// 			// Blursings
// 			Utilities.WriteFlagLine(
// 				"Bonuses - " + flags.RandomizeClassMaxBonus + " Maluses - " + flags.RandomizeClassMaxMalus + " Add XP Bonuses - " + flags.RandomizeClassIncludeXpBonus +
// 				" Add Inate Magic Bonuses - " + flags.RandomizeClassCasting + " Start with Key Items - " + flags.RandomizeClassKeyItems
// 			);
// 			// Transmooglifier
// 			Utilities.WriteFlagLine(
// 				"Variance - " + flags.TransmooglifierVariance + " Guarantee Custom Party Composition - " + flags.GuaranteeCustomClassComposition + " Balance Weapons for Customs - " + flags.MooglieWeaponBalance
// 			);
// 			Utilities.WriteFlagLine("Promotion");
// 			Utilities.WriteFlagLine(
// 				"Random Classes at Class Change - " + flags.EnableRandomPromotions + " Include Base Classes - " + flags.IncludeBaseClasses + " Spoil Promotions - " + flags.RandomPromotionsSpoilers
// 			);
// 			Utilities.WriteFlagLine("Levels");
// 			Utilities.WriteFlagLine(
// 				"Starting Levels - " + flags.StartingLevel + " Max Level Low - " + flags.MaxLevelLow + " Max Level High - " + flags.MaxLevelHigh
// 			);
// 			Utilities.WriteFlagLine("Magic Charges");
// 			Utilities.WriteFlagLine(
// 				"MP Charges Restore on New Max MP - " + flags.MpGainOnMaxGainMode + " Earlier High-Tier Magic Charges - " + flags.EarlierHighTierMagic + " Max MP - " + flags.ChangeMaxMP + " Red Mage Max MP - " + flags.RedMageMaxMP + " White Mage Max MP - " + flags.WhiteMageMaxMP +
// 				" Black Mage Max MP - " + flags.BlackMageMaxMP + " Knight Max MP - " + flags.KnightMaxMP + " Ninja Max MP - " + flags.NinjaMaxMP + " Knight and Ninja Gain Charges in All Levels - " + flags.AllSpellLevelsForKnightNinja
// 			);
// 			Utilities.WriteFlagLine("Magic Adjustments");
// 			Utilities.WriteFlagLine(
// 				"Knight Level 4 Spell - " + flags.Knightlvl4 + " Pink Mage - " + flags.PinkMage + " Black Knight - " + flags.BlackKnight + " Keep Permissions - " + flags.BlackKnightKeep + " White Ninja - " + flags.WhiteNinja + " Keep Permissions - " + flags.WhiteNinjaKeep
// 			);
// 			Utilities.WriteFlagLine("Class XP Bonus");
// 			Utilities.WriteFlagLine(
// 				"Fighter - " + flags.ExpMultiplierFighter + " Thief - " + flags.ExpMultiplierThief + " Black Belt - " + flags.ExpMultiplierBlackBelt + " Red Mage - " + flags.ExpMultiplierRedMage + " White Mage - " + flags.ExpMultiplierWhiteMage + " Black Mage - " + flags.ExpMultiplierBlackMage
// 			);

// 			Utilities.WriteFlagLine("");
// 		}

// 		private void WriteEquipmentFlags()
// 		{
// 			Utilities.WriteFlagLine("Equipment and Items Flags");
// 			Utilities.WriteFlagLine("-----------------");
// 			Utilities.WriteFlagLine("Starting Equipment");
// 			Utilities.WriteFlagLine(
// 				"Masamune - " + flags.StartingEquipmentMasamune + " Katana - " + flags.StartingEquipmentKatana + " Heal Staff - " + flags.StartingEquipmentHealStaff + " Zeus Gauntlet - " + flags.StartingEquipmentZeusGauntlet +
// 				" White Shirt - " +flags.StartingEquipmentWhiteShirt + " Ribbon - " + flags.StartingEquipmentRibbon + " Dragonslayer - " + flags.StartingEquipmentDragonslayer + " Legendary Kit - " + flags.StartingEquipmentLegendKit +
// 				" Random Endgame Weapon - " + flags.StartingEquipmentRandomEndgameWeapon + " Random AoE Caster Item - " + flags.StartingEquipmentRandomAoe + " Random Caster Item - " + flags.StartingEquipmentRandomCasterItem + " One Legendary/Caster Item - " + flags.StartingEquipmentOneItem +
// 				" Random Typed Weapon - "  + flags.StartingEquipmentRandomTypeWeapon + " Grandpa's Secret Stash - " + flags.StartingEquipmentGrandpasSecretStash + " Random Commons - " + flags.StartingEquipmentRandomCrap + " Starter Pack - " + flags.StartingEquipmentStarterPack +
// 				" No Legendary Duplicates - " + flags.StartingEquipmentNoDuplicates + " Remove Items from Treasure Chests - " + flags.StartingEquipmentRemoveFromPool
// 			);
// 			Utilities.WriteFlagLine("Equipment Blursing");
// 			Utilities.WriteFlagLine(
// 				"Blursed Weapons - " + flags.RandomWeaponBonus + " Blursed Weapons Low - " + flags.RandomWeaponBonusLow + " Blursed Weapons High - " + flags.RandomWeaponBonusHigh + " Exclude Masamune - " + flags.RandomWeaponBonusExcludeMasa +
// 				" Blursed Armor - " + flags.RandomArmorBonus + " Blursed Armor Low - " + flags.RandomArmorBonusLow + " Blursed Armor High - " + flags.RandomArmorBonusHigh
// 			);
// 			Utilities.WriteFlagLine("Balance");
// 			Utilities.WriteFlagLine(
// 				"Double Weapon Crit Rate - " + flags.WeaponCritRate + " Increase Weapon Elemental & Type Bonuses - " + flags.WeaponBonuses + " Bonus - " + flags.WeaponTypeBonusValue
// 			);
// 			Utilities.WriteFlagLine("Consumable Items");
// 			Utilities.WriteFlagLine(
// 				"Starting Inventory - " + flags.StartingItemSet + " Treasure Chest Stacks - " + flags.ConsumableTreasureStackSize + " Consumable Chests - " + flags.MoreConsumableChests +
// 				" House Full HP Restoration - " + flags.HousesFillHp + " Turn Shelters into Ethers - " + flags.Etherizer
// 			);
// 			Utilities.WriteFlagLine("New Consumables");
// 			Utilities.WriteFlagLine(
// 				"Consumables - " + flags.ExtConsumableSet + " Starting Inventory - " + flags.ExtStartingItemSet + " Chests - " + flags.ExtConsumableChests +
// 				" Chest Stacks - " + flags.ExtConsumableTreasureStackSize + " Normal Shops - " + flags.NormalShopsHaveExtConsumables + " Legendary Shops - " + flags.LegendaryShopHasExtConsumables
// 			);
// 			Utilities.WriteFlagLine("Spellcasting Gear");
// 			Utilities.WriteFlagLine(
// 				"Item Magic - " + flags.ItemMagicMode + " Item Magic Pool - " + flags.ItemMagicPool + " All Weapons Cast Spells - " + flags.MagisizeWeapons +
// 				" Guaranteed Defense Item - " + flags.GuaranteedDefenseItem + " Guaranteed Offense Item - " + flags.GuaranteedPowerItem
// 			);
// 			Utilities.WriteFlagLine("Weapon Randomization");
// 			Utilities.WriteFlagLine(
// 				"Use Weaponizer - " + flags.Weaponizer + " Common Weapons Can Have Powers - " + flags.WeaponizerCommonWeaponsHavePowers + " Name for Casting & Quality Only - " + flags.WeaponizerNamesUseQualityOnly
// 			);
// 			Utilities.WriteFlagLine("Armor Randomization");
// 			Utilities.WriteFlagLine(
// 				"Ribbon - " + flags.RibbonMode + " Use Armor Crafter - " + flags.ArmorCrafter
// 			);

// 			Utilities.WriteFlagLine("");
// 		}

// 		private void WriteAdjustmentFlags()
// 		{
// 			Utilities.WriteFlagLine("Game Adjustments Flags");
// 			Utilities.WriteFlagLine("-----------------");
// 			Utilities.WriteFlagLine("Bug Fixes");
// 			Utilities.WriteFlagLine(
// 				"House MP Restoration - " + flags.HouseMPRestoration + " Weapon Stats - " + flags.WeaponStats + " Chance to Run - " + flags.ChanceToRun +
// 				" Spell Fixes - " + flags.SpellBugs + " Enemy Status Attacks - "  + flags.EnemyStatusAttackBug + " Black Belt & Master Absorb Fix - " + flags.BlackBeltAbsorb
// 			);
// 			Utilities.WriteFlagLine("RNG");
// 			Utilities.WriteFlagLine(
// 				"RNG Tables - " + flags.Rng + " Improve Turn Order Randomization - " + flags.ImproveTurnOrderRandomization + " Fix Missing Battle RNG Entry - " + flags.FixMissingBattleRngEntry + " Set RNG - " + flags.SetRNG
// 			);
// 			Utilities.WriteFlagLine("Saving");
// 			Utilities.WriteFlagLine(
// 				"Disable Shelter Saving - " + flags.DisableTentSaving + " Disable Inn Saving - " + flags.DisableInnSaving + " Save Game When Game Over - " + flags.SaveGameWhenGameOver + " Dragon Warrior Mode - " + flags.SaveGameDWMode
// 			);
// 			Utilities.WriteFlagLine("Transport");
// 			Utilities.WriteFlagLine(
// 				"AirBoat - " + flags.AirBoat
// 			);
// 			Utilities.WriteFlagLine("Conveniences");
// 			Utilities.WriteFlagLine(
// 				"No Party Shuffle - " + flags.NoPartyShuffle + " Speed Hacks - " + flags.SpeedHacks + " Faster Walking Speed - " + flags.Dash + " Faster Ship - " + flags.SpeedBoat +
// 				" Identify Treasures - " + flags.IdentifyTreasures + " Buy Quantity - " + flags.BuyTen + " Change Unnrunnable RUN to WAIT - " + flags.WaitWhenUnrunnable + " Critical Hit Count - " + flags.EnableCritNumberDisplay +
// 				" Battle Magic Menu Wrap Around - " + flags.BattleMagicMenuWrapAround + " NPC Guillotine - " + flags.NPCSwatter + " Autosort Inventory - " + flags.InventoryAutosort + " Auto Retargeting - " + flags.AutoRetargeting +
// 				" Shop Information - " + flags.ShopInfo + " Chest Information - " + flags.ChestInfo + " Magic Menu Spell Reordering - " + flags.MagicMenuSpellReordering + " All Incentive Chest Fanfare - " + flags.IncentiveChestItemsFanfare
// 			);
// 			Utilities.WriteFlagLine("Other Tweaks");
// 			Utilities.WriteFlagLine(
// 				"TRANCE is Status Element - " + flags.TranceHasStatusElement + " Increase Dark Penalty - " + flags.IncreaseDarkPenalty + " Increase Enemy Regeneration - " + flags.IncreaseRegeneration +
// 				" Disable Minimap - " + flags.DisableMinimap + " In-Battle Poison Damage - " + flags.PoisonMode + " Poison Fixed Damage - " + flags.PoisonSetDamageValue
// 			);

// 			Utilities.WriteFlagLine("");
// 		}

// 		private void WriteExperimentalFlags()
// 		{
// 			Utilities.WriteFlagLine("Experimental Beta Flags");
// 			Utilities.WriteFlagLine("-----------------");
// 			Utilities.WriteFlagLine(
// 				"Open Chests in Order - " + flags.OpenChestsInOrder + " Encounter Table Uses True RNG - " + flags.EncounterPrng + " Armor Resists Damage Tiles - " + flags.ArmorResistsDamageTileDamage +
// 				" Shuffle Lava Tiles - " + flags.ShuffleLavaTiles + " Add Damage Tiles - " + flags.AddDamageTiles + " Towns - " + flags.DamageTilesTowns + " Castles - " + flags.DamageTilesCastles +
// 				" Dungeons - " + flags.DamageTilesDungeons + " Caves - " + flags.DamageTilesCaves + " Temple of Fiends - " + flags.DamageTilesTof + " Quantity - " + flags.DamageTilesQuantity +
// 				" Generated Earth Caves - " + flags.ProcgenEarth + " INT Affects Spells - " + flags.IntAffectsSpells + " Repeated Heal Potion Use - " + flags.RepeatedHealPotionUse
// 			);
// 		}
	// }
// }
