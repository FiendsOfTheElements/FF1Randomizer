using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RomUtilities;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		private const Item ReplacementItem = Item.Cabin;

		public const int TreasureOffset = 0x03100;
		public const int TreasureSize = 1;
		public const int TreasurePoolCount = 256;
		public const int TreasureCount = 256;

		public const int lut_MapObjTalkJumpTblAddress = 0x390D3;
		public const string giveRewardRoutineAddress = "93DD";
		public static readonly List<int> UnusedTreasureIndices =
			Enumerable.Range(0, 1).Concat(
			Enumerable.Range(145, 4)).Concat(
			Enumerable.Range(187, 9)).Concat(
			Enumerable.Range(255, 1))
			.ToList();

		public static readonly List<int> UsedTreasureIndices = Enumerable.Range(0, 256).Except(UnusedTreasureIndices).ToList(); // This maps a compacted list back to the game's array, skipping the unused slots.

		public List<IRewardSource> ShuffleTreasures(MT19337 rng,
													IItemPlacementFlags flags,
													IncentiveData incentivesData,
													ItemShopSlot caravanItemLocation,
													Dictionary<MapLocation, List<MapChange>> mapLocationRequirements,
													Dictionary<MapLocation, Tuple<MapLocation, AccessRequirement>> mapLocationFloorRequirements,
													Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullFloorRequirements)
		{
			var vanillaNPCs = !flags.NPCItems && !flags.NPCFetchItems;
			if (!vanillaNPCs)
			{
				EnableBridgeShipCanalAnywhere();
				EnableNPCsGiveAnyItem();
				// This extends Vampire's routine to set a flag for Sarda, but it also clobers Sarda's routine
				if (!flags.EarlySarda)
				{
					Put(0x393E1, Blob.FromHex("207F90A51160"));
				}
			}

			var treasureBlob = Get(TreasureOffset, TreasureSize * TreasureCount);
			var treasurePool = UsedTreasureIndices.Select(x => (Item)treasureBlob[x])
							.Concat(ItemLists.AllNonTreasureChestItems).ToList();

			if (flags.ShardHunt)
			{
				treasurePool = treasurePool.Select(ShardHuntTreasureSelector).ToList();
				int shardsAdded = treasurePool.Count(item => item == Item.Shard);
				Debug.Assert(shardsAdded == TotalOrbsToInsert);
			}

			var placedItems =
				ItemPlacement.PlaceSaneItems(rng,
											flags,
											incentivesData,
											treasurePool,
											caravanItemLocation,
											mapLocationRequirements,
											mapLocationFloorRequirements,
											fullFloorRequirements);

			if (flags.FreeBridge)
			{
				placedItems = placedItems.Select(x => x.Item != Item.Bridge ? x : ItemPlacement.NewItemPlacement(x, ReplacementItem)).ToList();
			}
			if (flags.FreeAirship)
			{
				placedItems = placedItems.Select(x => x.Item != Item.Floater ? x : ItemPlacement.NewItemPlacement(x, ReplacementItem)).ToList();
			}

			// Output the results to the ROM
			foreach (var item in placedItems.Where(x => !x.IsUnused && x.Address < 0x80000 && (!vanillaNPCs || x is TreasureChest)))
			{
				//Debug.WriteLine(item.SpoilerText);
				item.Put(this);
			}

			MoveShipToRewardSource(placedItems.Find(reward => reward.Item == Item.Ship));
			return placedItems;
		}

		/// <summary>
		/// Start of NPC item shuffle should call the following 4 methods:
		/// 
		/// PermanentCaravan();
		/// CheckCanoeItemInsteadOfEventVar();
		/// EnableBridgeShipCanalAnywhere();
		/// EnableNPCsGiveAnyItem();
		/// 
		/// Then any item can be assigned to an NPC like this:
		/// new MapObject(ItemLocations.KingConeria, Item.Key).Put(this);
		/// </summary>
		public void EnableBridgeShipCanalAnywhere()
		{
			// Replace a long unused dialog string with text for game variables
			var gameVariableText =
				"9C91929900000000" + // SHIP
				"8A929B9C91929900" + // AIRSHIP
				"8B9B928D908E0000" + // BRIDGE
				"8C8A978A95000000" + // CANAL
				"00000000" +
				"8C8A97988E00"; // CANOE
			Put(0x2825C, Blob.FromHex(gameVariableText));

			// Add processing in control code 3 to check for variables
			var controlCode3 =
				"A5616920901D0A" +
				"695A853E" +
				"A982853F" +
				"184C9ADB";
			Put(0x7DBF8, Blob.FromHex(controlCode3));
			// See source: ~/asm/1F_DBF8_ControlCode3.asm

			// Use control code 3 instead of 2 for normal treasure
			Data[0x2B187] = 0x03;
		}

		/// <summary>
		/// Start of NPC item shuffle should call the following 4 methods:
		/// 
		/// PermanentCaravan();
		/// CheckCanoeItemInsteadOfEventVar();
		/// EnableBridgeShipCanalAnywhere();
		/// EnableNPCsGiveAnyItem();
		/// 
		/// Then any item can be assigned to an NPC like this:
		/// new MapObject(ItemLocations.KingConeria, Item.Key).Put(this);
		/// </summary>
		public void EnableNPCsGiveAnyItem()
		{
			// These first 2 are safe to call even if NPC items aren't being shuffled
			SplitOpenTreasureRoutine();
			CleanupNPCRoutines();

			// Replace Don't be greedy text with NPC item text "Here, take this"
			var replacementText =
				"91A823" + // Here
				"BF1BA4AE1A" + // , take 
				"1C3005" + // this\n
				$"0300";
			Put(0x28F35, Blob.FromHex(replacementText));

			// New routine for NPC item trades
			var itemTradeNPCRoutine =
				"A416207990B027A5106920AA8617BD0060F01B" +
				"A513F01720" + giveRewardRoutineAddress +
				"B014A517C931B004AADE0060" +
				"A416207F90A93A60" +
				"A51160";
			// Put at Nerrick routine
			Put(0x39356, Blob.FromHex(itemTradeNPCRoutine));
			// See source: ~/asm/0E_9356_StandardNPCItemTrade.asm

			// New routine for NPC items based on game event flag
			var eventFlagGiveNPCRoutine =
				"A41098F0052079909007" +
				"A4162079909003A51260" +
				"A51320" + giveRewardRoutineAddress +
				"B007A416" +
				"207F90A93A60";
			// Put at Talk_ifairship, overrunning Talk_ifearthfire and most of Talk_CubeBotBad
			Put(0x3956B, Blob.FromHex(eventFlagGiveNPCRoutine));
			// See source: ~/asm/0E_956B_StandardNPCItem.asm

			// *** Only Nerrick and Smith and required to give an item since 
			// those routines are overwritten so we set the vanilla item here 
			// so he still gives it if nothing else changes
			Data[ItemLocations.Nerrick.Address] = (byte)Item.Canal;
			Data[ItemLocations.Smith.Address] = (byte)Item.Xcalber;

			// *** Handle special cases (Bikke and Astos)
			EnableBikkeAnyItem();

			EnableAstosAnyItem();
		}

		private void EnableAstosAnyItem()
		{
			Data[ItemLocations.Astos.Address] = (byte)Item.Crystal;
			var newText =
				"571E92BFC1" + // tis I, 
				"8A37B2B6BFC1" + // Astos, 
				"5F585F3CAAC4C405" + // all along!!\n
				"92BE4EC1B7A4AE1A1C39" +// I'll take that
				"C18C9B98A097C1222705" + //  CROWN and\n 
				"56B8BE4E4FB54B1C1A0305" + // you'll pry the \n___
				"A94DB0424BA6B2AF27A72B27413BB6C400!"; // from my cold dead hands!
			Put(0x285EF, Blob.FromHex(newText));
			var newAstosRoutine =
				"AD2260F016A513F01220" + giveRewardRoutineAddress +
				"B00FA007207392A97D" +
				"20C590A93A60A51160";
			Put(0x39338, Blob.FromHex(newAstosRoutine));
			// See source: ~/asm/0E_9338_AstosAnyItem.asm
		}

		private void EnableBikkeAnyItem()
		{
			Data[ItemLocations.Bikke.Address] = (byte)Item.Ship;
			var newBikkeRoutine =
				"A03F209190B00B" +
				"20A490A97E" +
				"20C590A51160" +
				"A004207990B013A513F00F841020" + giveRewardRoutineAddress +
				"B00AA004207F90A93A60A51260";
			Put(0x392D0, Blob.FromHex(newBikkeRoutine));
			// See source: ~/asm/0E_92D0_BikkeAnyItem.asm
		}

		private void SplitOpenTreasureRoutine()
		{
			// Replace "OpenTreasureChest" routine
			var openTreasureChest =
				$"A9002003FEA645BD00B120{giveRewardRoutineAddress}" +
				"B00AA445B9006209049900628A60"; // 27 bytes
			Put(0x7DD78, Blob.FromHex(openTreasureChest));
			// See source: ~/asm/1F_DD78_OpenTreasureChestRewrite.asm

			// New "GiveReward" routine
			const string checkItem =
				"85616920C93CB013AAC90CD005" +
				"DE0060B003FE0060C931B02A902B"; // 27 bytes
			const string notItem =
				"A561C96C900920B9EC20EADD4CD6DD" +
				"C944B0092034DDB007A9E59007" +
				"2046DDB00CA9BD" +
				"65619D0061"; // 40 bytes
			const string openChest =
				"18E67DE67DA2F09004EEB960E88A60"; // 12 bytes
			var giveRewardRoutine =
				$"{checkItem}{notItem}{openChest}";
			Put(0x7DD93, Blob.FromHex(giveRewardRoutine));
			// See source: ~/asm/1F_DD78_OpenTreasureChestRewrite.asm
		}

		private void MoveShipToRewardSource(IRewardSource source)
		{
			Blob location = null;
			//if (!ItemLocations.ShipLocations.TryGetValue(source.MapLocation, out location))
			{
				location = Dock.Coneria;
			}

			Put(0x3000 + UnsramIndex.ShipX, location);
		}
	}
}
