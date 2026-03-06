using System.ComponentModel;
using static System.Math;
using FF1Lib.Helpers;
using RomUtilities;
using System.Reflection;
using System.Runtime.CompilerServices;

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
		
		public enum SleepMode
	{
		[Description("Fixed Value")]
		Fixed = 0,
		[Description("Wake On Hit")]
		WakeOnHit,
		// [Description("HP Range")]
		// HPRange,
	}
	public class EncounterRate
	{
		private const int ThreatLevelsOffset = 0x2CC01; // +1 because first entry is for overworld [unused]
		private const int ThreatLevelsSize = 63; // -1 because of overworld offset

		private const int overworldThreatLevelBank = 0x0F;
		//private const int OverworldThreatLevelOffset = 0x7C4FE;
		//private const int OceanThreatLevelOffset = 0x7C506;
		private const int OverworldThreatLevelOffset = 0xC4FE;
		private const int OceanThreatLevelOffset = 0xC506;

		private const int newOverworldThreatLevelBank = 0x1F;
		private const int newOverworldThreatLevelOffset = 0x7C4FE;
		private const int newOceanThreatLevelOffset = 0x7C506;

		private FF1Rom rom;
		private byte[] dungeonEncounterRate;
		private byte overworldEncounterRate;
		private byte oceanEncounterRate;
		public EncounterRate(FF1Rom _rom)
		{
			rom = _rom;
			dungeonEncounterRate = _rom.Get(ThreatLevelsOffset, ThreatLevelsSize).ToBytes();
			overworldEncounterRate = _rom.GetFromBank(overworldThreatLevelBank, OverworldThreatLevelOffset, 1, false)[0];
			oceanEncounterRate = _rom.GetFromBank(overworldThreatLevelBank, OceanThreatLevelOffset, 1, false)[0];
		}
		public void UpdateRate(MapIndex map, int rate)
		{
			dungeonEncounterRate[(int)map] = (byte)rate;
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
				overworldEncounterRate = 0;
				oceanEncounterRate = 0;
				rom.PutInBank(0x1F, 0xC50E, Blob.FromHex("EAEA"));
			}
			else
			{
				overworldEncounterRate = (byte)Math.Ceiling(overworldEncounterRate * overworldMultiplier);
				oceanEncounterRate = (byte)Math.Ceiling(oceanEncounterRate * overworldMultiplier);
			}

			dungeonEncounterRate = dungeonEncounterRate.Select(x => (byte)Math.Ceiling(x * dungeonMultiplier)).ToArray();
		}

		public List<byte> Get()
		{
			List<byte> encounterRates = new() {overworldEncounterRate, oceanEncounterRate};
			encounterRates.AddRange(dungeonEncounterRate);
			return encounterRates;
		}
		public void Write()
		{
			rom.PutInBank(newOverworldThreatLevelBank, OverworldThreatLevelOffset, new byte[] { overworldEncounterRate });
			rom.PutInBank(newOverworldThreatLevelBank, OceanThreatLevelOffset, new byte[] { oceanEncounterRate });
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
		public void ScalePrices(ShopData shopData, IScaleFlags flags, MT19337 rng, bool increaseOnly, ItemShopSlot shopItemLocation, bool FreeClinic = false)
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

			/*
			var pointers = Get(ShopPointerOffset, ShopPointerCount * ShopPointerSize).ToUShorts();
			RepackShops(pointers);*/

			foreach (var shop in shopData.Shops.Where(s => s.Type == ShopType.Inn || s.Type == ShopType.Clinic).ToList())
			{
				var priceValue = (ushort)RangeScale(shop.Price / multiplier, scaleLow, scaleHigh, 1, rng);
				if (FreeClinic && shop.Type == ShopType.Clinic) priceValue = 0;
				shop.Price = priceValue;
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
		public void ScaleSleep(MT19337 rng, Flags flags)
		{
			// Fixes the index used for Enemy HP for Sleep
			PutInBank(0x0C,0xB1D5,Blob.FromHex("02"));
			// Fixes math buffer bank for subtraction
			PutInBank(0x0C,0xB1EA,Blob.FromHex("00"));
			// Fixes RandAX extra increment; otherwise using 256 results in an overflow of the high value making the range equal to the low value
			PutInBank(0x0C,0xAE60,Blob.FromHex("EAEAEAEA"));
			// fix the enemy sleep buffer
			PutInBank(0x0C,0xB1EE,Blob.FromHex("F009A9DF2090B1A90FD002A9024C8EB2EAEAEA"));
			// fix player sleep subroutine
			PutInBank(0x0C,0xA451,Blob.FromHex("EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAAD5668"));

			if (flags.SleepModeDropDown == SleepMode.Fixed)
			{
				if (flags.PlayerSleepScaleLow == 0 && flags.PlayerSleepScaleHigh == 80 && flags.EnemySleepScaleLow == 0 && flags.EnemySleepScaleHigh == 80)
				{
					return;
				}

				string minPlayerSleep = (bool)flags.ClampMinimumStatScale ? 0.ToString("X") : flags.PlayerSleepScaleLow.ToString("X");
				Console.WriteLine(minPlayerSleep);
				string highPlayerSleep = (bool)flags.ClampMinimumStatScale ? Math.Max(255, flags.PlayerSleepScaleHigh).ToString("X") : flags.PlayerSleepScaleHigh.ToString("X");
				Console.WriteLine(highPlayerSleep);
				string minEnemySleep = (bool)flags.ClampMinimumStatScale ? 0.ToString("X") : flags.EnemySleepScaleLow.ToString("X");
				Console.WriteLine(minEnemySleep);
				string highEnemySleep = (bool)flags.ClampMinimumStatScale ? Math.Max(255, flags.EnemySleepScaleHigh).ToString("X") : flags.EnemySleepScaleHigh.ToString("X");
				Console.WriteLine(highEnemySleep);

				PutInBank(0x0C,0xA445,Blob.FromHex(minPlayerSleep));
				PutInBank(0x0C,0xA447,Blob.FromHex(highPlayerSleep));
				PutInBank(0x0C,0xB1E2,Blob.FromHex(minEnemySleep));
				PutInBank(0x0C,0xB1E4,Blob.FromHex(highEnemySleep));
			}
			if (flags.SleepModeDropDown == SleepMode.WakeOnHit)
			{
				// Remove random player wake up
				PutInBank(0x0C,0xA3B1,Blob.FromHex("EAEAEAEAEAEAEAEAEA"));
				// Remove random enemy wake up
				PutInBank(0x0C,0xB1BC,Blob.FromHex("EAEAEAEA"));

				if (flags.StartOfHits == true)
				{
					// Replace Do Physical Attack Function
					PutInBank(0x0C,0xA67B,Blob.FromHex("AD6068D003EE6068EE6A68AD6268CD5E68900BA905AE586820DDAEEE6B68A916A216A005207BAEA9DF2D89688D8968AD8768F065AD7368F060AD5668CD5E689058A9648D6468AD7B682D6E68EAEAEAEAEAEAF005A9008D6468A907AE7A68200AAFEAEAEAEAEAEAAD6468D003EE6468A900A2C8205DAE8D6668C9C8F01CAD6468CD66689014AD896849FF2D73682088A9AD89680D73688D8968CE5A68F0034CDDA7AD6A68C90290268D0B6BA9118D0A6BA9008D0C6B8D0F6BA90F8D0D6BA92B8D0E6BA901A20AA06B2018F2EEF86AAD82680D8368D011A90F8D2A6BA9408D2B6BA9008D2C6BF020A9118D2A6BAD82688D2B6BAD83688D2C6BA90F8D2D6BA92E8D2E6BA9008D2F6BA903A22AA06B2018F2EEF86AAD6B68F008A52C209DA020E6A0A913A213A01620ACAEEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAAD7C68D013A9018D8968AD8768F004A920D002A93F209DA04CD7A0"));
				}
				if (flags.AfterHits == true)
				{
					// Replace Do Physical Attack Function
					PutInBank(0x0C,0xA67B,Blob.FromHex("A900AAA00220A2A0A902A003A20020A2A0205AA6A9A88D5668AD6C688D5868AD72688D6268AD88682908F009AD566838E9288D5668AD89682908F009AD56681869288D5668AD6D682D76688D5C68AD6E682D77688D5E68AD5C680D5E68F026A900A22820DDAEAC5668AE576820AAA48C56688E5768AD586818691E8D58689005A9FF8D5868AD89682930F016AD58684A4A186D58688D58689036A9FF8D58684C4BA7A900AE6F6820DDAEAC5668AE576820AAA48C56688E5768A900AE7868200AAFEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAAD5668D003EE5668AD6268CD56689006AD56688D6268AD7168AE70682009AE8D5A68AD5A68D003EE5A68AD8168A200202FABAD8768D024AE8068D003EEEE6AAE8068A000AD846820709EAD89682903D00320FABBA9008DEE6AF017A90120B8BE202AF2AD89682903D008AD8568290320FCA0AD89682903F005A9214C07AAA9008D6B688D6A688D82688D8368AD58688DAD6B205AA68D6068A2C8205DAE8D5E68AD5E68C9C8D0034CC4A8AEAD6BA900205DAE186DAD6B8D58689005A9FF8D5868AD5668CD5E68902BAD58688D6068A905AE7968200AAFEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAAD6068D003EE6068EE6A68AD6268CD5E68900BA905AE586820DDAEEE6B68A916A216A005207BAEAD8768F065AD7368F060AD5668CD5E689058A9648D6468AD7B682D6E68EAEAEAEAEAEAF005A9008D6468A907AE7A68200AAFEAEAEAEAEAEAAD6468D003EE6468A900A2C8205DAE8D6668C9C8F01CAD6468CD66689014AD896849FF2D73682088A9AD89680D73688D8968CE5A68F0034CDDA7AD6A68C90290268D0B6BA9118D0A6BA9008D0C6B8D0F6BA90F8D0D6BA92B8D0E6BA901A20AA06B2018F2EEF86AAD82680D8368D011A90F8D2A6BA9408D2B6BA9008D2C6BF028A9DF2D89688D8968A9118D2A6BAD82688D2B6BAD83688D2C6BA90F8D2D6BA92E8D2E6BA9008D2F6BA903A22AA06B2018F2EEF86AAD6B68F008A52C209DA020E6A0A913A213A01620ACAEEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAAD7C68D013A9018D8968AD8768F004A920D002A93F209DA04CD7A0"));
				}
			}
			// if (mode == SleepMode.HPRange)
			// {}
		}

		public void ScaleEnemyStats(MT19337 rng, Flags flags)
		{
			if ((flags.EnemyScaleStatsHigh == 100 && flags.EnemyScaleStatsLow == 100 && !(bool)flags.SeparateEnemyHPScaling) ||
				(flags.EnemyScaleStatsHigh == 100 && flags.EnemyScaleStatsLow == 100 && (bool)flags.SeparateEnemyHPScaling && flags.EnemyScaleHpLow == 100 && flags.EnemyScaleHpHigh == 100))
			{
				return;
			}

			int minStats = (bool)flags.ClampMinimumStatScale ? 100 : flags.EnemyScaleStatsLow;
			int highStats = (bool)flags.ClampMinimumStatScale ? Math.Max(100, flags.EnemyScaleStatsHigh) : flags.EnemyScaleStatsHigh;
			int minHp = (bool)flags.ClampEnemyHpScaling ? 100 : flags.EnemyScaleHpLow;
			int highHp = (bool)flags.ClampEnemyHpScaling ? Math.Max(100, flags.EnemyScaleHpHigh) : flags.EnemyScaleHpHigh;

			NonBossEnemies.ForEach(index => ScaleSingleEnemyStats(index, minStats, highStats, flags.IncludeMorale, rng,
				(bool)flags.SeparateEnemyHPScaling, minHp, highHp, GetEvadeIntFromFlag(flags.EvadeCap)));
		}


		public void ScaleBossStats(MT19337 rng, Flags flags)
		{
			if ((flags.BossScaleStatsHigh == 100 && flags.BossScaleStatsLow == 100 && !(bool)flags.SeparateBossHPScaling) ||
				(flags.BossScaleStatsHigh == 100 && flags.BossScaleStatsLow == 100 && (bool)flags.SeparateBossHPScaling && flags.BossScaleHpLow == 100 && flags.BossScaleHpHigh == 100))
			{
				return;
			}


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

		private void ScaleAllAltExp(Flags flags)
		{
			if (flags.ExpMultiplierFighter != 1.0)
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
			if (flags.ExpMultiplierWhiteMage != 1.0)
			{
				ScaleAltExp(flags.ExpMultiplierWhiteMage, FF1Class.WhiteMage);
			}
			if (flags.ExpMultiplierBlackMage != 1.0)
			{
				ScaleAltExp(flags.ExpMultiplierBlackMage, FF1Class.BlackMage);
			}
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

		public void EnableSwolePirates(bool enable)
		{
			if (!enable)
			{
				return;
			}

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
