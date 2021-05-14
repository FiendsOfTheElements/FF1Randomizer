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
	}
	public enum FormationPool
	{
		[Description("Alt Formations (Random)")]
		AltFormationRng,
		[Description("Alt Formations (Distributed)")]
		AltFormationDist,
		[Description("Random Fiends")]
		Fiends,
		[Description("Phantom")]
		Phantom,
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
			PutInBank(0x11, 0x8EA0, Blob.FromHex("A561202096B039A645BD008FF02E856AA9C0203D96A56A200096A903CD866BD00820189668684C4396A97BC56AF00A1820E28E201896A2F0604C38C920E28E60AA60A911855818A5612010B0A445B9006209049900626000"));

			InsertDialogs(0x110, "Monster-in-a-box!"); // 0xC0

			// Select treasure
			bool tempflag = false;
			List<IRewardSource> usedChests = new();
			var validChests = ItemLocations.AllTreasures.ToList();
			var chestMonsterList = new byte[0x100];
			var treasureList = Get(lut_TreasureOffset, 0x100);
			int maxChests = (int)ChestsPool.Size160;
			var betterEquipmentList = ItemLists.UberTier.Concat(ItemLists.LegendaryArmorTier).Concat(ItemLists.LegendaryWeaponTier).Concat(ItemLists.RareArmorTier).Concat(ItemLists.RareWeaponTier);
			validChests.RemoveAll(x => x.IsUnused == true || (tempflag && ItemLists.AllQuestItems.Contains((Item)treasureList[x.Address - lut_TreasureOffset])) || (tempflag && treasureList[x.Address - lut_TreasureOffset] == (int)Item.Shard) || (tempflag && betterEquipmentList.Contains((Item)treasureList[x.Address - lut_TreasureOffset])) || (tempflag && GetIncentiveList(flags).Contains((Item)treasureList[x.Address - lut_TreasureOffset])));

			// Get encounters
			List<byte> encounters;
			encounters = Enumerable.Range(128, FirstBossEncounterIndex).Select(value => (byte)value).ToList();
			encounters.Add(0xFF); // IronGOL

			// Process pool first
			if (tempflag)
			{
				maxChests = Math.Min(maxChests, validChests.Count);

				for (int i = 0; i < maxChests; i++)
				{
					usedChests.Add(validChests.SpliceRandom(rng));
					chestMonsterList[(usedChests.Last().Address - lut_TreasureOffset)] = encounters.SpliceRandom(rng);
				}
			}

			// Better Treasure
			if (tempflag)
			{
				for (int i = 0; i < 0x100; i++)
				{
					if (betterEquipmentList.Contains((Item)treasureList[i]))
						chestMonsterList[i] = encounters.SpliceRandom(rng);
				}
			}

			// Key Items
			if (tempflag)
			{
				for (int i = 0; i < 0x100; i++)
				{
					if (ItemLists.AllQuestItems.Contains((Item)treasureList[i]))
						chestMonsterList[i] = encounters.SpliceRandom(rng);
				}
			}

			// Shards
			if ((bool)flags.TrappedShards)
			{
				for (int i = 0; i < 0x100; i++)
				{
					if (treasureList[i] == (byte)Item.Shard)
						chestMonsterList[i] = encounters.SpliceRandom(rng);
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

				chestList = ItemLocations.AllTreasures.ToList();
				chestList.RemoveAll(x => disallowedLocations.Contains(x.MapLocation) || x.IsUnused == true || chestMonsterList[x.Address - lut_TreasureOffset] > 0);
				chestMonsterList[chestList.SpliceRandom(rng).Address - lut_TreasureOffset] = ChaosFormationIndex;

				SetNpc(MapId.TempleOfFiendsRevisitedChaos, 0, ObjectId.None, 0, 0, true, true);
			}

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

		public byte PickEncounter()
		{
			List<byte> encounters;
			encounters = Enumerable.Range(128, FirstBossEncounterIndex).Select(value => (byte)value).ToList();
			encounters.Add(0xFF); // IronGOL


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

			return incentivePool.ToList();
		}
	}
}
