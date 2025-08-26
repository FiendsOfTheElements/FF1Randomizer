using System.Numerics;
using System.Reflection;
using Newtonsoft.Json;
using System.IO.Compression;
using static FF1Lib.FF1Rom;
using FF1Lib.Sanity;
using System.ComponentModel;

namespace FF1Lib
{
	public partial class Flags : IIncentiveFlags, IScaleFlags, IVictoryConditionFlags, IFloorShuffleFlags, IItemPlacementFlags
	{
		public bool ExtConsumablesEnabled => ExtConsumableSet != ExtConsumableSet.None;
		public bool IncentivizeRibbon2 => false;
		public bool Incentivize65K => false;
		public bool IncentivizeBad => false;
		public bool OrbsRequiredEnabled => !ShardHunt && (GameMode != GameModes.DeepDungeon);
		public bool OrbsRequiredOptionsEnabled => OrbsRequiredEnabled && (OrbsRequiredCount != 4 && OrbsRequiredCount != 0);
		public bool PoisonConstantValueEnabled => PoisonMode == PoisonModeOptions.Constant;
		public LoosePlacementMode LoosePlacementMode
		{
			get
			{
				if (LooseItemsSpreadPlacement == false) return LoosePlacementMode.Vanilla;
				if (LooseItemsSpreadPlacement == true && LooseItemsForwardPlacement == false) return LoosePlacementMode.Spread;
				return LoosePlacementMode.Forward;
			}
			set
			{
				switch (value)
				{
					case LoosePlacementMode.Vanilla:
						LooseItemsSpreadPlacement = false;
						LooseItemsForwardPlacement = false;
						break;
					case LoosePlacementMode.Spread:
						LooseItemsSpreadPlacement = true;
						LooseItemsForwardPlacement = false;
						break;
					case LoosePlacementMode.Forward:
						LooseItemsSpreadPlacement = true;
						LooseItemsForwardPlacement = true;
						break;
				}
			}
		}
		public bool IncentivizeFreeNPCsEnabled => (Treasures ?? true) && (NPCItems ?? true);
		public bool IncentivizeFetchNPCsEnabled => (Treasures ?? true) && (NPCFetchItems ?? true);
		public bool? MapCanalBridge => ((NPCItems) | (NPCFetchItems) | MapOpenProgression | MapOpenProgressionExtended) & (!DesertOfDeath);
		public bool DisableOWMapModifications => GameMode == GameModes.Standard && OwMapExchange != OwMapExchanges.None;
		public bool LefeinSuperStoreEnabled => (ShopKillMode_White.Equals(ShopKillMode.None) && ShopKillMode_Black.Equals(ShopKillMode.None));
		public bool SpoilerBatsDontCheckOrbsEnabled => !SkyWarriorSpoilerBats.Equals(SpoilerBatHints.Vanilla);
		//public string Encoded => EncodeFlagsText(this);
		//public event PropertyChangedEventHandler PropertyChanged;

		// The philosophy governing item incentivizations works something like this:
		// 1. If the item is NOT being shuffled to another location it cannot be incentivized. (Duh)
		// 2. If the item is required to unlock any location OR is given to a not-shuffled NPC who gives
		//    such an item in return it is considered a MAIN item. (e.g. CROWN/SLAB with vanilla fetch quests)
		// 3. If the item is given to an NPC who themselves is shuffled it's considered a FETCH item.
		// 4. The vehicles now have their own incentivization flags apart from other progression items.

		// Ruby is required if Sarda is Required for the ROD
		public bool? RequiredRuby => !EarlySage & !NPCItems & !IsAirshipFree;
		public bool? UselessRuby => IsAirshipFree & !TitansTrove;
		public bool? IncentivizeRuby => (RequiredRuby & IncentivizeMainItems) | (!RequiredRuby & IncentivizeFetchItems & !UselessRuby);

		// If Canoe and Fetch Quests are unshuffled and there is no free canal or airship then TNT is required
		public bool? RequiredTnt => !NPCFetchItems & !NPCItems & !(IsCanalFree | IsAirshipFree);
		// If Fetch Items are vanilla and the player has a free Canal, do not incentivize TNT even if Other Quest Items are in the pool since there would be absolutely nothing to gain from TNT
		public bool? UselessTnt => !NPCFetchItems & (IsCanalFree | (IsAirshipFree & !MapOpenProgression));
		public bool? IncentivizeTnt => (RequiredTnt & IncentivizeMainItems) | (!RequiredTnt & IncentivizeFetchItems & !UselessTnt);

		public bool? IncentivizeCrown => (!(NPCFetchItems ?? false) && (IncentivizeMainItems ?? false)) || ((NPCFetchItems ?? false) && (IncentivizeFetchItems ?? false));
		public bool? IncentivizeSlab => (!(NPCFetchItems ?? false) && (IncentivizeMainItems ?? false)) || ((NPCFetchItems ?? false) && (IncentivizeFetchItems ?? false));
		public bool? IncentivizeBottle => (!(NPCFetchItems ?? false) && (IncentivizeMainItems ?? false)) || ((NPCFetchItems ?? false) && (IncentivizeFetchItems ?? false));
		public bool NoOverworld => GameMode == GameModes.NoOverworld;
		public bool DesertOfDeath => (GameMode == GameModes.Standard & OwMapExchange == OwMapExchanges.Desert);
		public bool VanillaPlacement => (bool)Treasures || (bool)ChestsKeyItems || (bool)NPCFetchItems || (bool)NPCItems;
		public bool? IsShipFree => FreeShip | NoOverworld;
		public bool? IsCanoeFree => FreeCanoe | DesertOfDeath;
		public bool? IsAirshipFree => FreeAirship & !NoOverworld & !DesertOfDeath;
		public bool? IsBridgeFree => FreeBridge | NoOverworld | DesertOfDeath;
		public bool? IsCanalFree => (FreeCanal & !NoOverworld) | DesertOfDeath;
		public bool? IsFloaterRemoved => ((NoFloater|IsAirshipFree) & !NoOverworld) | DesertOfDeath;
		public bool? IncentivizeBridge => NPCItems & IncentivizeBridgeItem & !IsBridgeFree & !NoOverworld;
		public bool? IncentivizeCanoe => NPCItems & IncentivizeCanoeItem & !FreeCanoe & !DesertOfDeath;
		public bool? IncentivizeLute => NPCItems & !FreeLute & IncentivizeMainItems;
		public bool? IncentivizeShip => NPCItems & IncentivizeShipAndCanal & !IsShipFree & !NoOverworld;
		public bool? IncentivizeRod => NPCItems & !FreeRod & IncentivizeMainItems ;
		public bool? IncentivizeCube => NPCItems & IncentivizeMainItems;
		public bool? IncentivizeFloater => !IsAirshipFree & !IsFloaterRemoved & IncentivizeAirship;
		public bool? IncentivizePromotion => !FreeTail & !NoTail & IncentivizeTail;

		public bool? IncentivizeCanal => NPCFetchItems & IncentivizeShipAndCanal & !IsCanalFree & !NoOverworld;
		public bool? IncentivizeCrystal => NPCFetchItems & IncentivizeFetchItems;
		public bool? IncentivizeHerb => NPCFetchItems & IncentivizeFetchItems;
		public bool? IncentivizeKey => NPCFetchItems & IncentivizeMainItems;
		public bool? IncentivizeChime => NPCFetchItems & IncentivizeMainItems;
		public bool? IncentivizeOxyale => NPCFetchItems & IncentivizeMainItems;

		public bool? IncentivizeAdamant => IncentivizeFetchItems;

		public int IncentivizedItemCountMin => 0
			+ ((IncentivizePromotion ?? false) ? 1 : 0)
			+ ((IncentivizeMasamune & !NoMasamune ?? false) ? 1 : 0)
			+ ((IncentivizeKatana ?? false) ? 1 : 0)
			+ ((IncentivizeVorpal ?? false) ? 1 : 0)
			+ ((IncentivizeOpal ?? false) ? 1 : 0)
			+ ((IncentivizeRibbon ?? false) ? 1 : 0)
			+ ((IncentivizeDefCastArmor ?? false) ? 1 : 0)
			+ ((IncentivizeOffCastArmor ?? false) ? 1 : 0)
			+ ((IncentivizeOtherCastArmor ?? false) ? 1 : 0)
			+ ((IncentivizePowerRod ?? false) ? 1 : 0)
			+ ((IncentivizeDefCastWeapon ?? false) ? 1 : 0)
			+ ((IncentivizeOffCastWeapon ?? false) ? 1 : 0)
			+ (IncentivizeOtherCastWeapon ? 1 : 0)
			+ ((IncentivizeAdamant ?? false) ? 1 : 0)
			+ ((IncentivizeRuby ?? false) ? 1 : 0)
			+ ((IncentivizeCrown ?? false) ? 1 : 0)
			+ ((IncentivizeTnt ?? false) ? 1 : 0)
			+ ((IncentivizeSlab ?? false) ? 1 : 0)
			+ ((IncentivizeBottle ?? false) ? 1 : 0)
			+ ((IncentivizeFloater ?? false) ? 1 : 0)
			+ ((IncentivizeBridge ?? false) ? 1 : 0)
			+ ((IncentivizeLute ?? false) ? 1 : 0)
			+ ((IncentivizeShip ?? false) ? 1 : 0)
			+ ((IncentivizeRod ?? false) ? 1 : 0)
			+ ((IncentivizeCanoe ?? false) ? 1 : 0)
			+ ((IncentivizeCube ?? false) ? 1 : 0)
			+ ((IncentivizeCrystal ?? false) ? 1 : 0)
			+ ((IncentivizeHerb ?? false) ? 1 : 0)
			+ ((IncentivizeKey ?? false) ? 1 : 0)
			+ ((IncentivizeCanal ?? false) ? 1 : 0)
			+ ((IncentivizeChime ?? false) ? 1 : 0)
			+ ((IncentivizeOxyale ?? false) ? 1 : 0)
			+ ((IncentivizeXcalber ?? false) ? 1 : 0);

		public int IncentivizedItemCountMax => 0
			+ ((IncentivizePromotion ?? true) ? 1 : 0)
			+ ((IncentivizeMasamune & !NoMasamune ?? true) ? 1 : 0)
			+ ((IncentivizeKatana ?? true) ? 1 : 0)
			+ ((IncentivizeVorpal ?? true) ? 1 : 0)
			+ ((IncentivizeOpal ?? true) ? 1 : 0)
			+ ((IncentivizeRibbon ?? true) ? 1 : 0)
			+ ((IncentivizeDefCastArmor ?? true) ? 1 : 0)
			+ ((IncentivizeOffCastArmor ?? true) ? 1 : 0)
			+ ((IncentivizeOtherCastArmor ?? true) ? 1 : 0)
			+ ((IncentivizePowerRod ?? true) ? 1 : 0)
			+ ((IncentivizeDefCastWeapon ?? true) ? 1 : 0)
			+ ((IncentivizeOffCastWeapon ?? true) ? 1 : 0)
			+ (IncentivizeOtherCastWeapon ? 1 : 0)
			+ ((IncentivizeAdamant ?? true) ? 1 : 0)
			+ ((IncentivizeRuby ?? true) ? 1 : 0)
			+ ((IncentivizeCrown ?? true) ? 1 : 0)
			+ ((IncentivizeTnt ?? true) ? 1 : 0)
			+ ((IncentivizeSlab ?? true) ? 1 : 0)
			+ ((IncentivizeBottle ?? true) ? 1 : 0)
			+ ((IncentivizeFloater ?? true) ? 1 : 0)
			+ ((IncentivizeBridge ?? true) ? 1 : 0)
			+ ((IncentivizeLute ?? true) ? 1 : 0)
			+ ((IncentivizeShip ?? true) ? 1 : 0)
			+ ((IncentivizeRod ?? true) ? 1 : 0)
			+ ((IncentivizeCanoe ?? true) ? 1 : 0)
			+ ((IncentivizeCube ?? true) ? 1 : 0)
			+ ((IncentivizeCrystal ?? true) ? 1 : 0)
			+ ((IncentivizeHerb ?? true) ? 1 : 0)
			+ ((IncentivizeKey ?? true) ? 1 : 0)
			+ ((IncentivizeCanal ?? true) ? 1 : 0)
			+ ((IncentivizeChime ?? true) ? 1 : 0)
			+ ((IncentivizeOxyale ?? true) ? 1 : 0)
			+ ((IncentivizeXcalber ?? true) ? 1 : 0);

		public string IncentivizedItems => ""
			+ ((IncentivizeAdamant != null) ? (IncentivizeAdamant ?? false ? "Adamant " : "") : ("Adamant? "))
			+ ((IncentivizeBridge != null) ? (IncentivizeBridge ?? false ? "Bridge " : "") : ("Bridge? "))
			+ ((IncentivizeBottle != null) ? (IncentivizeBottle ?? false ? "Bottle " : "") : ("Bottle? "))
			+ ((IncentivizeCanal != null) ? (IncentivizeCanal ?? false ? "Canal " : "") : ("Canal? "))
			+ ((IncentivizeCanoe != null) ? (IncentivizeCanoe ?? false ? "Canoe " : "") : ("Canoe? "))
			+ ((IncentivizeChime != null) ? (IncentivizeChime ?? false ? "Chime " : "") : ("Chime? "))
			+ (IncentivizeBad ? "Cloth " : "")
			+ ((IncentivizeCrown != null) ? (IncentivizeCrown ?? false ? "Crown " : "") : ("Crown? "))
			+ ((IncentivizeCrystal != null) ? (IncentivizeCrystal ?? false ? "Crystal " : "") : ("Crystal? "))
			+ ((IncentivizeCube != null) ? (IncentivizeCube ?? false ? "Cube " : "") : ("Cube? "))
			+ ((IncentivizeFloater != null) ? (IncentivizeFloater ?? false ? "Floater " : "") : ("Floater? "))
			+ ((IncentivizeHerb != null) ? (IncentivizeHerb ?? false ? "Herb " : "") : ("Herb? "))
			+ ((IncentivizeKey != null) ? (IncentivizeKey ?? false ? "Key " : "") : ("Key? "))
			+ ((IncentivizeLute != null) ? (IncentivizeLute ?? false ? "Lute " : "") : ("Lute? "))
			+ ((IncentivizeOxyale != null) ? (IncentivizeOxyale ?? false ? "Oxyale " : "") : ("Oxyale? "))
			+ ((IncentivizeRod != null) ? (IncentivizeRod ?? false ? "Rod " : "") : ("Rod? "))
			+ ((IncentivizeRuby != null) ? (IncentivizeRuby ?? false ? "Ruby " : "") : ("Ruby? "))
			+ ((IncentivizeShip != null) ? (IncentivizeShip ?? false ? "Ship " : "") : ("Ship? "))
			+ ((IncentivizeSlab != null) ? (IncentivizeSlab ?? false ? "Slab " : "") : ("Slab? "))
			+ ((IncentivizePromotion != null) ? (IncentivizePromotion ?? false ? "Tail " : "") : ("Tail? "))
			+ ((IncentivizeTnt != null) ? (IncentivizeTnt ?? false ? "Tnt " : "") : ("Tnt? "))
			+ (((IncentivizeMasamune & !NoMasamune) != null) ? ((IncentivizeMasamune & !NoMasamune) ?? false ? "Masmune\U0001F5E1 " : "") : ("Masmune?\U0001F5E1 "))
			+ ((IncentivizeKatana != null) ? (IncentivizeKatana ?? false ? "Katana\U0001F5E1 " : "") : ("Katana?\U0001F5E1 "))
			+ ((IncentivizeVorpal != null) ? (IncentivizeVorpal ?? false ? "Vorpal\U0001F5E1 " : "") : ("Vorpal?\U0001F5E1 "))
			+ ((IncentivizeXcalber != null) ? (IncentivizeXcalber ?? false ? "XCalber\U0001F5E1 " : "") : ("XCalber?\U0001F5E1 "))
			+ ((IncentivizeDefCastWeapon != null) ? (IncentivizeDefCastWeapon ?? false ? "Defense\U0001F5E1 " : "") : ("Defense?\U0001F5E1 "))
			+ (IncentivizeOtherCastWeapon ? "Mage\U0001F9D9 " : "")
			+ ((IncentivizeOffCastWeapon != null) ? (IncentivizeOffCastWeapon ?? false ? "Thor\U0001F528 " : "") : ("Thor?\U0001F528 "))
			+ ((IncentivizeOpal != null) ? (IncentivizeOpal ?? false ? "Opal\U0001F48D " : "") : ("Opal?\U0001F48D "))
			+ ((IncentivizeOtherCastArmor != null) ? (IncentivizeOtherCastArmor ?? false ? "Power\U0001F94A " : "") : ("Power?\U0001F94A "))
			+ ((IncentivizePowerRod != null) ? (IncentivizePowerRod ?? false ? "PowerRod " : "") : ("PowerRod? "))
			+ ((IncentivizeOffCastArmor != null) ? (IncentivizeOffCastArmor ?? false ? "Black\U0001F9E5 " : "") : ("Black?\U0001F9E5 "))
			+ ((IncentivizeDefCastArmor != null) ? (IncentivizeDefCastArmor ?? false ? "White\U0001F455 " : "") : ("White?\U0001F455 "))
			+ ((IncentivizeRibbon != null) ? (IncentivizeRibbon ?? false ? "Ribbon\U0001F380 " : "") : ("Ribbon?\U0001F380 "))
			+ (IncentivizeRibbon2 ? "Ribbon\U0001F380 " : "")
			+ (Incentivize65K ? "65000G " : "");

		public bool IncentivizeKingConeria => (NPCItems ?? false) && (IncentivizeFreeNPCs ?? false);
		public bool IncentivizePrincess => (NPCItems ?? false) && (IncentivizeFreeNPCs ?? false);
		public bool IncentivizeBikke => (NPCItems ?? false) && (IncentivizeFreeNPCs ?? false);
		public bool IncentivizeSarda => (NPCItems ?? false) && (IncentivizeFreeNPCs ?? false);
		public bool IncentivizeCanoeSage => (NPCItems ?? false) && (IncentivizeFreeNPCs ?? false);
		public bool IncentivizeCaravan => (NPCItems ?? false) && (IncentivizeFreeNPCs ?? false);
		public bool IncentivizeCubeBot => (NPCItems ?? false) && (IncentivizeFreeNPCs ?? false);

		public bool IncentivizeFairy => (NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false);
		public bool IncentivizeAstos => (NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false);
		public bool IncentivizeMatoya => (NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false);
		public bool IncentivizeElfPrince => (NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false);
		public bool IncentivizeNerrick => (NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false) && !NoOverworld;
		public bool IncentivizeLefein => (NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false);
		public bool IncentivizeSmith => (NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false);

		public int IncentivizedLocationCountMin => 0
			+ ((NPCItems ?? false) && (IncentivizeFreeNPCs ?? false) ? 7 : 0)
			+ ((NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false) ? (!NoOverworld ? 7 : 6) : 0)
			+ ((IncentivizeMarsh ?? false) ? 1 : 0)
			+ ((IncentivizeEarth ?? false) ? 1 : 0)
			+ ((IncentivizeVolcano ?? false) ? 1 : 0)
			+ ((IncentivizeIceCave ?? false) ? 1 : 0)
			+ ((IncentivizeOrdeals ?? false) ? 1 : 0)
			+ ((IncentivizeSeaShrine ?? false) ? 1 : 0)
			+ ((IncentivizeConeria ?? false) ? 1 : 0)
			+ ((IncentivizeMarshKeyLocked ?? false) ? 1 : 0)
			+ ((IncentivizeTitansTrove ?? false) ? 1 : 0)
			+ ((IncentivizeSkyPalace ?? false) ? 1 : 0)
			+ ((IncentivizeCardia ?? false) ? 1 : 0);


		public int IncentivizedLocationCountMax => 0
			+ ((NPCItems ?? true) && (IncentivizeFreeNPCs ?? true) ? 7 : 0)
			+ ((NPCFetchItems ?? true) && (IncentivizeFetchNPCs ?? true) ? (!NoOverworld ? 7 : 6) : 0)
			+ ((IncentivizeMarsh ?? true) ? 1 : 0)
			+ ((IncentivizeEarth ?? true) ? 1 : 0)
			+ ((IncentivizeVolcano ?? true) ? 1 : 0)
			+ ((IncentivizeIceCave ?? true) ? 1 : 0)
			+ ((IncentivizeOrdeals ?? true) ? 1 : 0)
			+ ((IncentivizeSeaShrine ?? true) ? 1 : 0)
			+ ((IncentivizeConeria ?? true) ? 1 : 0)
			+ ((IncentivizeMarshKeyLocked ?? true) ? 1 : 0)
			+ ((IncentivizeTitansTrove ?? true) ? 1 : 0)
			+ ((IncentivizeSkyPalace ?? true) ? 1 : 0)
			+ ((IncentivizeCardia ?? true) ? 1 : 0);

		public int TrappedChestsFloor => 0
			+ ((ShardHunt && TCShards == TCOptions.All) ? 32 : 0)
			+ ((TCKeyItems == TCOptions.All) ? 20 : 0)
		    + ((TCBetterTreasure == TCOptions.All) ? 46 : 0)
		    + ((TCMasaGuardian == true && TCBetterTreasure != TCOptions.All) ? 1 : 0)
			+ ((TrappedChaos == true) ? 1 : 0);

		public bool? ImmediatePureAndSoftRequired => (TouchMode != TouchMode.Standard) | Entrances | MapOpenProgression | RandomizeFormationEnemizer | RandomizeEnemizer;
		public bool? DeepCastlesPossible => Entrances & Floors;
		public bool? DeepTownsPossible => Towns & Entrances & Floors & EntrancesMixedWithTowns;
		public bool EnemizerEnabled => (bool)RandomizeFormationEnemizer | (bool)RandomizeEnemizer;
		public bool EnemizerDontMakeNewScripts => (bool)ShuffleSkillsSpellsEnemies & !((bool)EnemySkillsSpellsTiered);

		public bool? TrappedChestsEnabled => (bool)TrappedChaos | (bool)TCMasaGuardian | (TCBetterTreasure == TCOptions.All | TCKeyItems == TCOptions.All | TCShards == TCOptions.All) | ((TCBetterTreasure == TCOptions.Pooled | TCKeyItems == TCOptions.Pooled | TCShards == TCOptions.Pooled) & TCChestCount > 0) | (TCChestCount > 0);

		public bool IsAnythingLoose => (IncentivizedItemCountMax > IncentivizedLocationCountMin) || IncentivizeMainItems != true || IncentivizeFetchItems != true || (IncentivizeAirship != true && FreeAirship != true && NoFloater != true) || (IncentivizeCanoeItem != true && FreeCanoe != true) || (IncentivizeShipAndCanal != true && (FreeShip != true || FreeCanal != true)) || (IncentivizeBridgeItem != true && FreeBridge != true) || (IncentivizeTail != true && FreeTail != true && NoTail != true);

	}
}
