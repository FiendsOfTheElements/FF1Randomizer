using System.ComponentModel;
using static System.Math;
using FF1Lib.Helpers;
using RomUtilities;

namespace FF1Lib
{
	public enum ProgressiveScaleMode
	{
		[Description("Disabled")]
		Disabled,
		[Description("+50% @ 12 Items")]
		FiftyPercentAt12,
		[Description("+50% @ 15 Items")]
		FiftyPercentAt15,
		[Description("Doubled @ 12 Items")]
		DoubledAt12,
		[Description("Doubled @ 15 Items")]
		DoubledAt15,
		[Description("+5% Per Item")]
		Progressive5Percent,
		[Description("+7.7% Per Item")]
		Progressive7Percent,
		[Description("+10% Per Item")]
		Progressive10Percent,
		[Description("+20% Per Item")]
		Progressive20Percent,
		[Description("+12.5% Per Orb or 8 Shards")]
		OrbProgressiveSlow,
		[Description("+25% Per Orb or 8 Shards")]
		OrbProgressiveMedium,
		[Description("+50% Per Orb or 8 Shards")]
		OrbProgressiveFast,
		[Description("+100% Per Orb or 8 Shards")]
		OrbProgressiveVFast,
	}

	public enum EvadeCapValues
	{
		[Description("Very Low: 225")]
		veryLow,
		[Description("Low: 230")]
		low,
		[Description("Medium-low: 235")]
		medLow,
		[Description("Medium: 240")]
		medium,
		[Description("Medium-high: 245")]
		medHigh,
		[Description("High: 250")]
		high,
		[Description("Extreme: 253")]
		extreme,
		[Description("Uncapped: 255")]
		insane,
	}


	public class EncounterRate
	{
		private const int ThreatLevelsOffset = 0x2CC01; // +1 because first entry is for overworld [unused]
		private const int ThreatLevelsSize = 63; // -1 because of overworld offset
		private const int OverworldThreatLevelOffset = 0x7C4FE;
		private const int OceanThreatLevelOffset = 0x7C506;

		private FF1Rom rom;
		private byte[] dungeonEncounterRate;
		private byte overworldEncounterRate;
		private byte oceanEncounterRate;
		public EncounterRate(FF1Rom _rom)
		{
			rom = _rom;
			dungeonEncounterRate = _rom.Get(ThreatLevelsOffset, ThreatLevelsSize).ToBytes();
			overworldEncounterRate = _rom[OverworldThreatLevelOffset];
			oceanEncounterRate = _rom[OceanThreatLevelOffset];
		}
		public void ScaleEncounterRate(Flags flags)
		{
			var overworldMultiplier = flags.EncounterRate / 30.0;
			var dungeonMultiplier = flags.DungeonEncounterRate / 30.0;

			if ((bool)flags.MapHallOfDragons)
			{
				dungeonEncounterRate[(int)MapIndex.BahamutCaveB1] = 0x18;
			}

			// threat level reference for comparison: 08 = most dungeon floors; 18 = sky bridge; 09 = ToFR earth; 0A = ToFR fire; 0B = ToFR water; 0C = ToFR air; 01 = ToFR chaos
			if ((bool)flags.ChaosFloorEncounters)
			{
				dungeonEncounterRate[(int)MapIndex.TempleOfFiendsRevisitedChaos] = 0x0D;
			}

			if (overworldMultiplier == 0)
			{
				rom.PutInBank(0x1F, 0xC50E, Blob.FromHex("EAEA"));
			}
			else
			{
				overworldEncounterRate = (byte)Math.Ceiling(overworldEncounterRate * overworldMultiplier);
				oceanEncounterRate = (byte)Math.Ceiling(oceanEncounterRate * overworldMultiplier);
			}

			if (dungeonMultiplier == 0)
			{
				rom.PutInBank(0x1F, 0xCDCC, Blob.FromHex("1860"));
			}
			else
			{
				dungeonEncounterRate = dungeonEncounterRate.Select(x => (byte)Math.Ceiling(x * dungeonMultiplier)).ToArray();
			}
		}
		public void Write()
		{
			rom[OverworldThreatLevelOffset] = overworldEncounterRate;
			rom[OceanThreatLevelOffset] = oceanEncounterRate;
			rom.Put(ThreatLevelsOffset, dungeonEncounterRate);
		}

	}
	public partial class FF1Rom : NesRom
	{
		public static readonly List<int> Bosses = new List<int> { Enemy.Garland, Enemy.Astos, Enemy.Pirate, Enemy.WarMech,
			Enemy.Lich, Enemy.Lich2, Enemy.Kary, Enemy.Kary2, Enemy.Kraken, Enemy.Kraken2, Enemy.Tiamat, Enemy.Tiamat2, Enemy.Chaos };

		public static readonly List<int> NonBossEnemies = Enumerable.Range(0, EnemyCount).Where(x => !Bosses.Contains(x)).ToList();

		public const int PriceOffset = 0x37C00;
		public const int PriceSize = 2;
		public const int PriceCount = 240;

		public const int LevelRequirementsOffsetThief = 0x6D535;
		public const int LevelRequirementsOffsetBlackBelt = 0x6D5C8;
		public const int LevelRequirementsOffsetRedMage = 0x6D65B;
		public const int LevelRequirementsOffsetWhiteMage = 0x6D6EE;
		public const int LevelRequirementsOffsetBlackMage = 0x6D781;

		// Scale is the geometric scale factor used with RNG.  Multiplier is where we make everything cheaper
		// instead of enemies giving more gold, so we don't overflow.
		public void ScalePrices(IScaleFlags flags, MT19337 rng, bool increaseOnly, ItemShopSlot shopItemLocation, bool FreeClinic = false)
		{
			IEnumerable<Item> tmpExcludedItems = Array.Empty<Item>() ;
			if (flags.ExcludeGoldFromScaling ?? false) tmpExcludedItems = tmpExcludedItems.Concat(ItemLists.AllGoldTreasure);
			if ((flags.ExcludeGoldFromScaling ?? false) && flags.CheapVendorItem) tmpExcludedItems = tmpExcludedItems.Concat(ItemLists.AllQuestItems);

			HashSet<Item> excludedItems = new HashSet<Item>(tmpExcludedItems);
			HashSet<Item> questItems = new HashSet<Item>(ItemLists.AllQuestItems);

			int rawScaleLow = increaseOnly ? 100 : flags.PriceScaleFactorLow;
			int rawScaleHigh = increaseOnly ? Math.Max(100, flags.PriceScaleFactorHigh) : flags.PriceScaleFactorHigh;

			double scaleLow = (double)rawScaleLow / 100.0;
			double scaleHigh = (double)rawScaleHigh / 100.0;

			var multiplier = flags.ExcludeGoldFromScaling ?? false ? 1.0 : flags.ExpMultiplier;
			var prices = Get(PriceOffset, PriceSize * PriceCount).ToUShorts();
			for (int i = 0; i < prices.Length; i++)
			{
				if (excludedItems.Contains((Item)i))
				{
					var price = (int)prices[i];

					if (flags.CheapVendorItem && questItems.Contains((Item)i)) price = 20000;

					var newPrice = price + rng.Between(-price / 10, price / 10);
					prices[i] = (ushort)Math.Min(Math.Max(newPrice, 1), 65535);
				}
				else
				{
					var price = ExtConsumables.ExtConsumablePriceFix((Item)i, prices[i], flags);
					var newPrice = RangeScaleWithZero(price / multiplier, scaleLow, scaleHigh, 1e-5 * multiplier, 1, rng);
					prices[i] = (ushort)Min(newPrice, 0xFFFF);
				}
			}
			var questItemPrice = prices[(int)Item.Bottle];
			// If we don't do this before checking for the item shop location factor, Ribbons and Shirts will end up being really cheap
			// This realistically doesn't matter without Shop Wares shuffle on because nobody wants to sell Ribbons/Shirts, but if it is...
			prices[(int)Item.WhiteShirt] = (ushort)(questItemPrice / 2);
			prices[(int)Item.BlackShirt] = (ushort)(questItemPrice / 2);
			prices[(int)Item.Ribbon] = questItemPrice;
			var itemShopFactor = new Dictionary<MapLocation, int>() {
				{ MapLocation.Coneria, 8 },
				{ MapLocation.Pravoka, 2 }
			};
			if (itemShopFactor.TryGetValue(shopItemLocation.MapLocation, out int divisor))
			{
				questItemPrice = (ushort)(prices[(int)Item.Bottle] / divisor);
			}
			for (var i = 0; i < (int)Item.Tent; i++)
			{
				prices[i] = questItemPrice;
			}
			Put(PriceOffset, Blob.FromUShorts(prices));

			for (int i = GoldItemOffset; i < GoldItemOffset + GoldItemCount; i++)
			{
				ItemsText[i] = prices[i].ToString() + " G";
			}

			var pointers = Get(ShopPointerOffset, ShopPointerCount * ShopPointerSize).ToUShorts();
			RepackShops(pointers);

			for (int i = (int)ShopType.Clinic; i < (int)ShopType.Inn + ShopSectionSize; i++)
			{
				if (pointers[i] != ShopNullPointer)
				{
					var priceBytes = Get(ShopPointerBase + pointers[i], 2);
					var priceValue = BitConverter.ToUInt16(priceBytes, 0);

					priceValue = (ushort)RangeScale(priceValue / multiplier, scaleLow, scaleHigh, 1, rng);
					if (FreeClinic && i < (int)ShopType.Clinic + ShopSectionSize) priceValue = 0;
					priceBytes = BitConverter.GetBytes(priceValue);
					Put(ShopPointerBase + pointers[i], priceBytes);
				}
			}
		}

		public int GetEvadeIntFromFlag(EvadeCapValues evadeCapFlag)
		{
			int evadeCap;
			switch (evadeCapFlag)
			{
				case EvadeCapValues.veryLow:
					evadeCap = 0xE1;
					break;
				case EvadeCapValues.low:
					evadeCap = 0xE6;
					break;
				case EvadeCapValues.medLow:
					evadeCap = 0xEB;
					break;
				case EvadeCapValues.medium:
					evadeCap = 0xF0;
					break;
				case EvadeCapValues.medHigh:
					evadeCap = 0xF5;
					break;
				case EvadeCapValues.high:
					evadeCap = 0xFA;
					break;
				case EvadeCapValues.extreme:
					evadeCap = 0xFC;
					break;
				case EvadeCapValues.insane:
					evadeCap = 0xFF;
					break;
				default:
					evadeCap = 0xF0;
					break;
			}

			return evadeCap;

		}

		public void ScaleEnemyStats(MT19337 rng, Flags flags)
		{
			int minStats = (bool)flags.ClampMinimumStatScale ? 100 : flags.EnemyScaleStatsLow;
			int highStats = (bool)flags.ClampMinimumStatScale ? Math.Max(100, flags.EnemyScaleStatsHigh) : flags.EnemyScaleStatsHigh;
			int minHp = (bool)flags.ClampEnemyHpScaling ? 100 : flags.EnemyScaleHpLow;
			int highHp = (bool)flags.ClampEnemyHpScaling ? Math.Max(100, flags.EnemyScaleHpHigh) : flags.EnemyScaleHpHigh;

			NonBossEnemies.ForEach(index => ScaleSingleEnemyStats(index, minStats, highStats, flags.IncludeMorale, rng,
				(bool)flags.SeparateEnemyHPScaling, minHp, highHp, GetEvadeIntFromFlag(flags.EvadeCap)));
		}


		public void ScaleBossStats(MT19337 rng, Flags flags)
		{
			int minStats = (bool)flags.ClampMinimumBossStatScale ? 100 : flags.BossScaleStatsLow;
			int highStats = (bool)flags.ClampMinimumBossStatScale ? Math.Max(100, flags.BossScaleStatsHigh) : flags.BossScaleStatsHigh;
			int minHp = (bool)flags.ClampBossHPScaling ? 100 : flags.BossScaleHpLow;
			int highHp = (bool)flags.ClampBossHPScaling ? Math.Max(100, flags.BossScaleHpHigh) : flags.BossScaleHpHigh;
			Bosses.ForEach(index => ScaleSingleEnemyStats(index, minStats, highStats, flags.IncludeMorale, rng,
				(bool)flags.SeparateBossHPScaling, minHp, highHp, GetEvadeIntFromFlag(flags.EvadeCap)));
			if ((bool)flags.FightBahamut) {
			    ScaleSingleEnemyStats(Enemy.Ankylo, minStats, highStats, flags.IncludeMorale, rng,
						  (bool)flags.SeparateBossHPScaling, minHp, highHp, GetEvadeIntFromFlag(flags.EvadeCap));
			}
		}

		public abstract class EnemyStat
		{
		    public const int HP = 4;
		    public const int Morale = 6;
		    public const int Scripts = 7;
		    public const int Evade = 8;
		    public const int Defense = 9;
		    public const int Hits = 10;
		    public const int HitPercent = 11;
		    public const int Strength = 12;
		    public const int CriticalPercent = 13;
		}

		public void ScaleSingleEnemyStats(int index, int lowPercentStats, int highPercentStats, bool includeMorale, MT19337 rng,
			bool separateHPScale, int lowPercentHp, int highPercentHp, int evadeClamp)
		{
			double lowDecimalStats = (double)lowPercentStats / 100.0;
			double highDecimalStats = (double)highPercentStats / 100.0;
			double lowDecimalHp = (double)lowPercentHp / 100.0;
			double highDecimalHp = (double)highPercentHp / 100.0;

			var enemy = Get(EnemyOffset + index * EnemySize, EnemySize);

			var hp = BitConverter.ToUInt16(enemy, EnemyStat.HP);
			if(separateHPScale)
			{
				hp = (ushort)Min(RangeScale(hp, lowDecimalHp, highDecimalHp, 1.0, rng), 0x7FFF);
			} else
			{
				hp = (ushort)Min(RangeScale(hp, lowDecimalStats, highDecimalStats, 1.0, rng), 0x7FFF);
			}
			var hpBytes = BitConverter.GetBytes(hp);
			Array.Copy(hpBytes, 0, enemy, EnemyStat.HP, 2);

			var newMorale = includeMorale ? RangeScale(enemy[EnemyStat.Morale], lowDecimalStats, highDecimalStats, 0.25, rng) : enemy[6];
			var newEvade = RangeScale(enemy[EnemyStat.Evade], lowDecimalStats, highDecimalStats, 1.0, rng);
			var newDefense = RangeScale(enemy[EnemyStat.Defense], lowDecimalStats, highDecimalStats, 0.5, rng);
			var newHits = RangeScale(enemy[EnemyStat.Hits], lowDecimalStats, highDecimalStats, 0.5, rng);
			var newHitPercent = RangeScale(enemy[EnemyStat.HitPercent], lowDecimalStats, highDecimalStats, 1.0, rng);
			var newStrength = RangeScale(enemy[EnemyStat.Strength], lowDecimalStats, highDecimalStats, 0.25, rng);
			var newCrit = RangeScale(enemy[EnemyStat.CriticalPercent], lowDecimalStats, highDecimalStats, 0.5, rng);

			enemy[EnemyStat.Morale] = (byte)Min(newMorale, 0xFF); // morale
			enemy[EnemyStat.Evade] = (byte)Min(newEvade, evadeClamp); // evade
			enemy[EnemyStat.Defense] = (byte)Min(newDefense, 0xFF); // defense
			enemy[EnemyStat.Hits] = (byte)Max(Min(newHits, 0xFF), 1); // hits
			enemy[EnemyStat.HitPercent] = (byte)Min(newHitPercent, 0xFF); // hit%
			enemy[EnemyStat.Strength] = (byte)Min(newStrength, 0xFF); // strength
			enemy[EnemyStat.CriticalPercent] = (byte)Min(newCrit, 0xFF); // critical%

			Put(EnemyOffset + index * EnemySize, enemy);
		}

		private int RangeScaleWithZero(double value, double lowPercent, double highPercent, double lowScalelowPercent, double adjustment, MT19337 rng)
		{
			var internalLowPercent = lowPercent;
			var lowScaleThreshold = 0.0;

			if (lowPercent == 0 && highPercent == 0) return 0;

			if (lowPercent == 0 && highPercent >= 0.2)
			{
				internalLowPercent = 0.1;
				lowScaleThreshold = 0.14;
			}
			else if (lowPercent == 0 && highPercent == 0.1)
			{
				var ret = RangeScale(value, lowScalelowPercent, highPercent, adjustment, rng);
				return ret > 3 ? ret : 0;
			}

			double exponent = (rng != null) ? (double)rng.Next() / uint.MaxValue : 1.0; // A number from 0 - 1
			double logLowPercent = Log(internalLowPercent);
			double logDifference = Log(highPercent) - logLowPercent;
			exponent = exponent * logDifference + logLowPercent; // For example for 50-200% a number from -0.69 to 0.69, for 200-400% a number from 0.69 to 1.38

			double scaleValue = Exp(exponent); // A number from 0.5 to 2, or 2 to 4

			if (lowScaleThreshold > 0 && scaleValue < lowScaleThreshold)
			{
				var ret = RangeScale(value, lowScalelowPercent, lowScaleThreshold, adjustment, rng);
				return ret > 3 ? ret : 0;
			}

			double adjustedScale = scaleValue > 1 ? (scaleValue - 1) * adjustment + 1 : 1 - ((1 - scaleValue) * adjustment); // Tightens the scale so some stats are not changed by as much. For example for strength (adjustment of 0.25) this becomes 0.875 to 1.25, 1.25 to 1.75 while for hp (adjustment of 1) this stays 0.5 to 2, 2 to 4
			return (int)Round(value * adjustedScale);
		}

		public static int RangeScale(double value, double lowPercent, double highPercent, double adjustment, MT19337 rng)
		{
			double exponent = (rng != null) ? (double)rng.Next() / uint.MaxValue : 1.0; // A number from 0 - 1
			double logLowPercent = Log(lowPercent);
			double logDifference = Log(highPercent) - logLowPercent;
			exponent = exponent * logDifference + logLowPercent; // For example for 50-200% a number from -0.69 to 0.69, for 200-400% a number from 0.69 to 1.38

			double scaleValue = Exp(exponent); // A number from 0.5 to 2, or 2 to 4
			double adjustedScale = scaleValue > 1 ? (scaleValue - 1) * adjustment + 1 : 1 - ((1 - scaleValue) * adjustment); // Tightens the scale so some stats are not changed by as much. For example for strength (adjustment of 0.25) this becomes 0.875 to 1.25, 1.25 to 1.75 while for hp (adjustment of 1) this stays 0.5 to 2, 2 to 4
			return (int)Round(value * adjustedScale);
		}
		// Previous RangeScale(), delete if no bugs come up with new RangeScale() - 2020-10-27
		private int oldRangeScale(double value, double lowPercent, double highPercent, double adjustment, MT19337 rng)
		{
			double range = highPercent - lowPercent;
			double randomRangeScale = rng == null ? range : range * ((double)rng.Next() / uint.MaxValue);
			double actualScale = lowPercent + randomRangeScale;
			double adjustedScale = actualScale > 1 ? (actualScale - 1) * adjustment + 1 : 1 - ((1 - actualScale) * adjustment);
			return (int)(adjustedScale * value);
		}

		private int Scale(double value, double scale, double adjustment, MT19337 rng, bool increaseOnly)
		{
			double exponent = 1.0;
			if (rng != null)
			{
				exponent = (double)rng.Next() / uint.MaxValue;
				exponent = increaseOnly ? exponent : exponent * 2.0 - 1.0;
			}
			double adjustedScale = 1.0 + adjustment * (scale - 1.0);
			return (int)Round(Pow(adjustedScale, exponent) * value);
		}

		public void SetProgressiveScaleMode(Flags flags)
		{
			byte ScaleFactor = 1;   // Bonus given by progressive scaling in 1/n form (ScaleFactor = 5 means bonus is + 1/5 per item)
			byte Threshold = 0;     // Number of key items required for bonus.  Set this to 0 for progressive mode (every key item increases bonus)
			byte ShardMultiplier = 1;  // Shards are worth 1/8 Orbs
			ProgressiveScaleMode mode = flags.ProgressiveScaleMode;
			if (flags.ShardHunt)  ShardMultiplier = 8;
			switch (mode)
			{
				case ProgressiveScaleMode.Disabled:
					return;
				case ProgressiveScaleMode.DoubledAt12:
					Threshold = 12;
					break;
				case ProgressiveScaleMode.DoubledAt15:
					Threshold = 15;
					break;
				case ProgressiveScaleMode.FiftyPercentAt12:
					Threshold = 12;
					ScaleFactor = 2;
					break;
				case ProgressiveScaleMode.FiftyPercentAt15:
					Threshold = 15;
					ScaleFactor = 2;
					break;
				case ProgressiveScaleMode.Progressive5Percent:
					ScaleFactor = 20;
					break;
				case ProgressiveScaleMode.Progressive7Percent:
					ScaleFactor = 13;
					break;
				case ProgressiveScaleMode.Progressive10Percent:
					ScaleFactor = 10;
					break;
				case ProgressiveScaleMode.Progressive20Percent:
					ScaleFactor = 5;
					break;
				case ProgressiveScaleMode.OrbProgressiveSlow:
					ScaleFactor = (byte) (8 * ShardMultiplier); // +12.5 per orb
					break;
				case ProgressiveScaleMode.OrbProgressiveMedium:
					ScaleFactor = (byte)(4 * ShardMultiplier); // +25 per orb
					break;
				case ProgressiveScaleMode.OrbProgressiveFast:
					ScaleFactor = (byte)(2 * ShardMultiplier); // +50 per orb
					break;
				case ProgressiveScaleMode.OrbProgressiveVFast:
					ScaleFactor = ShardMultiplier; // +100 per orb
					break;
			}

			//Progressive/Threshold scaling
			string HexBlob = $"200090ADB860D009A91C8580A960858160A9{ScaleFactor:X2}8516A9{Threshold:X2}8514F00EADB860C51490E6A9018515189005ADB8608515AD78688510AD79688511A516851220C090A515AAAD786865108D7868AD796865118D7968C9A7900AA90F8D7868A9A78D7968CAD0DF18AD76688510AD77688511A516851220C090A515AAAD766865108D7668AD776865118D7768C9A7900AA90F8D7668A9A78D7768CAD0DFAD76688588AD77688589A91C8580A960858160";
			PutInBank(0x0F, 0x9100, Blob.FromHex(HexBlob));
			//Inject into end-of-battle code
			PutInBank(0x1B, 0x806D, Blob.FromHex("20CBCFEAEAEAEAEA"));
		}
		public void ExpGoldBoost(Flags flags)
		{
			var enemyBlob = Get(EnemyOffset, EnemySize * EnemyCount);
			var enemies = enemyBlob.Chunk(EnemySize);

			foreach (var enemy in enemies)
			{
				var exp = BitConverter.ToUInt16(enemy, 0);
				var gold = BitConverter.ToUInt16(enemy, 2);

				exp += (ushort)flags.ExpBonus;

				if (!(flags.ExcludeGoldFromScaling ?? false))
				{
					gold += (ushort)flags.ExpBonus;
				}

				var expBytes = BitConverter.GetBytes(exp);
				var goldBytes = BitConverter.GetBytes(gold);

				Array.Copy(goldBytes, 0, enemy, 2, 2);
				Array.Copy(expBytes, 0, enemy, 0, 2);
			}

			enemyBlob = Blob.Concat(enemies);

			Put(EnemyOffset, enemyBlob);

			var levelRequirementsBlob = Get(LevelRequirementsOffset, LevelRequirementsSize * LevelRequirementsCount);
			var levelRequirementsBytes = levelRequirementsBlob.Chunk(3).Select(threeBytes => new byte[] { threeBytes[0], threeBytes[1], threeBytes[2], 0 }).ToList();
			for (int i = 0; i < LevelRequirementsCount; i++)
			{
				uint levelRequirement = (uint)(Math.Max(BitConverter.ToUInt32(levelRequirementsBytes[i], 0) / flags.ExpMultiplier, 1));
				levelRequirementsBytes[i] = BitConverter.GetBytes(levelRequirement);
			}

			Put(LevelRequirementsOffset, Blob.Concat(levelRequirementsBytes.Select(bytes => (Blob)new byte[] { bytes[0], bytes[1], bytes[2] })));
			Put(LevelRequirementsOffsetThief, Blob.Concat(levelRequirementsBytes.Select(bytes => (Blob)new byte[] { bytes[0], bytes[1], bytes[2] })));
			Put(LevelRequirementsOffsetBlackBelt, Blob.Concat(levelRequirementsBytes.Select(bytes => (Blob)new byte[] { bytes[0], bytes[1], bytes[2] })));
			Put(LevelRequirementsOffsetRedMage, Blob.Concat(levelRequirementsBytes.Select(bytes => (Blob)new byte[] { bytes[0], bytes[1], bytes[2] })));
			Put(LevelRequirementsOffsetWhiteMage, Blob.Concat(levelRequirementsBytes.Select(bytes => (Blob)new byte[] { bytes[0], bytes[1], bytes[2] })));
			Put(LevelRequirementsOffsetBlackMage, Blob.Concat(levelRequirementsBytes.Select(bytes => (Blob)new byte[] { bytes[0], bytes[1], bytes[2] })));

			// A dirty, ugly, evil piece of code that sets the level requirement for level 2, even though that's already defined in the above table.
			byte firstLevelRequirement = Data[0x7C04B];
			firstLevelRequirement = (byte)(firstLevelRequirement / flags.ExpMultiplier);
			Data[0x7C04B] = firstLevelRequirement;
		}

		public void ScaleAltExp(double scale, FF1Class characterClass)
		{
			int offset = LevelRequirementsOffset;

			switch(characterClass)
			{
				case FF1Class.Fighter:
					offset = LevelRequirementsOffset;
					break;
				case FF1Class.Thief:
					offset = LevelRequirementsOffsetThief;
					break;
				case FF1Class.BlackBelt:
					offset = LevelRequirementsOffsetBlackBelt;
					break;
				case FF1Class.RedMage:
					offset = LevelRequirementsOffsetRedMage;
					break;
				case FF1Class.WhiteMage:
					offset = LevelRequirementsOffsetWhiteMage;
					break;
				case FF1Class.BlackMage:
					offset = LevelRequirementsOffsetBlackMage;
					break;
			}

			var altLevelRequirementsBlob = Get(offset, LevelRequirementsSize * LevelRequirementsCount);
			var altLevelRequirementsBytes = altLevelRequirementsBlob.Chunk(3).Select(threeBytes => new byte[] { threeBytes[0], threeBytes[1], threeBytes[2], 0 }).ToList();
			for (int i = 0; i < LevelRequirementsCount; i++)
			{
				uint levelRequirement = (uint)(Math.Max(BitConverter.ToUInt32(altLevelRequirementsBytes[i], 0) / scale, 1));
				altLevelRequirementsBytes[i] = BitConverter.GetBytes(levelRequirement);
			}

			Put(offset, Blob.Concat(altLevelRequirementsBytes.Select(bytes => (Blob)new byte[] { bytes[0], bytes[1], bytes[2] })));
		}

		public void EnableSwolePirates()
		{
			EnemyInfo newPirate = new EnemyInfo();
			newPirate.decompressData(Get(EnemyOffset + EnemySize * Enemy.Pirate, EnemySize));
			newPirate.exp = 800;
			newPirate.gp = 800;
			newPirate.hp = 160;
			newPirate.num_hits = 2;
			newPirate.damage = 32;
			newPirate.absorb = 32;
			newPirate.mdef = 30;
			newPirate.accuracy = 35;
			newPirate.critrate = 5;
			newPirate.agility = 24;
			Put(EnemyOffset + EnemySize * Enemy.Pirate, newPirate.compressData());
		}

		public void EnableSwoleAstos(MT19337 rng)
		{
			EnemyInfo newAstos = new EnemyInfo();
			newAstos.decompressData(Get(EnemyOffset + EnemySize * Enemy.Astos, EnemySize));

			newAstos.morale = 255;
			newAstos.monster_type = (byte)MonsterType.MAGE;
			newAstos.exp = 12800;
			newAstos.gp = 8000;
			newAstos.hp = 850;
			newAstos.num_hits = 2;
			newAstos.damage = 45;
			newAstos.absorb = 60;
			newAstos.mdef = 180;
			newAstos.accuracy = 42;
			newAstos.critrate = 1;
			newAstos.agility = 250;
			newAstos.elem_weakness = (byte)SpellElement.Status | (byte)SpellElement.Death;
			newAstos.elem_resist = (byte)SpellElement.None;

			if (newAstos.AIscript == 0xFF) {
			    var i = searchForNoSpellNoAbilityEnemyScript();
			    if (i == -1) { return; }
			    newAstos.AIscript = (byte)i;
			}
			Put(EnemyOffset + EnemySize * Enemy.Astos, newAstos.compressData());

			var astosScript = new EnemyScriptInfo();
			astosScript.decompressData(Get(ScriptOffset + newAstos.AIscript * ScriptSize, ScriptSize));
			astosScript.spell_chance = 96;
			astosScript.skill_chance = 96;

			// use "find spell by effect" to be compatible with spell shuffle and spell crafter.
			var helper = new SpellHelper(this);
			var spells = helper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any, SpellElement.Any, SpellStatus.Mute).ToList();
			spells.AddRange(helper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any, SpellElement.Any, SpellStatus.Stun));
			spells.AddRange(helper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any, SpellElement.Any, SpellStatus.Sleep));
			spells.AddRange(helper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any, SpellElement.Any, SpellStatus.Stone));
			spells.AddRange(helper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any, SpellElement.Any, SpellStatus.Death));

			spells.Shuffle(rng);

			astosScript.spell_list = new byte[8];
			for (int i = 0; i < 8; i++) {
			    astosScript.spell_list[i] = (byte)(spells[i % spells.Count].Id - Spell.CURE);
			}

			var skills = new List<byte> { (byte)EnemySkills.Poison_Stone, (byte)EnemySkills.Crack, (byte)EnemySkills.Trance, (byte)EnemySkills.Toxic };
			skills.Shuffle(rng);
			astosScript.skill_list = skills.ToArray();
			Put(ScriptOffset + newAstos.AIscript * ScriptSize, astosScript.compressData());
		}

		public void BoostEnemyMorale(byte index)
		{
			EnemyInfo enemy = new EnemyInfo();
			enemy.decompressData(Get(EnemyOffset + EnemySize * index, EnemySize));
			enemy.morale = 255;
			Put(EnemyOffset + EnemySize * index, enemy.compressData());
		}

		public List<Blob> GetAllEnemyStats() {
		    return Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);
		}
	}
}
