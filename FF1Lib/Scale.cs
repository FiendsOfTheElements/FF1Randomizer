using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomUtilities;
using static System.Math;

namespace FF1Lib
{
	public enum ProgressiveScaleMode
	{
		[Description("Disabled")]
		Disabled,
		[Description("150% @ 12 Items")]
		FiftyPercentAt12,
		[Description("150% @ 15 Items")]
		FiftyPercentAt15,
		[Description("200% @ 12 Items")]
		DoubledAt12,
		[Description("200% @ 15 Items")]
		DoubledAt15,
		[Description("+ 5% Per Item")]
		Progressive5Percent,
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
		[Description("Very low, 225")]
		veryLow,
		[Description("low, 230")]
		low,
		[Description("medium low, 235")]
		medLow,
		[Description("medium, 240")]
		medium,
		[Description("medium high, 245")]
		medHigh,
		[Description("high, 250")]
		high,
		[Description("extreme, 253")]
		extreme,
		[Description("insane, 255")]
		insane,
	}

	public partial class FF1Rom : NesRom
	{
		public static readonly List<int> Bosses = new List<int> { Enemy.Garland, Enemy.Astos, Enemy.Pirate, Enemy.WarMech,
			Enemy.Lich, Enemy.Lich2, Enemy.Kary, Enemy.Kary2, Enemy.Kraken, Enemy.Kraken2, Enemy.Tiamat, Enemy.Tiamat2, Enemy.Chaos };

		public static readonly List<int> NonBossEnemies = Enumerable.Range(0, EnemyCount).Where(x => !Bosses.Contains(x)).ToList();

		public const int PriceOffset = 0x37C00;
		public const int PriceSize = 2;
		public const int PriceCount = 240;

		public const int ThreatLevelsOffset = 0x2CC00;
		public const int ThreatLevelsSize = 64;
		public const int OverworldThreatLevelOffset = 0x7C4FE;
		public const int OceanThreatLevelOffset = 0x7C506;

		// Scale is the geometric scale factor used with RNG.  Multiplier is where we make everything cheaper
		// instead of enemies giving more gold, so we don't overflow.
		public void ScalePrices(IScaleFlags flags, string[] text, MT19337 rng, bool increaseOnly, ItemShopSlot shopItemLocation)
		{
			var scale = flags.PriceScaleFactor;
			var multiplier = flags.ExpMultiplier;
			var prices = Get(PriceOffset, PriceSize * PriceCount).ToUShorts();
			for (int i = 0; i < prices.Length; i++)
			{
				var newPrice = Scale(prices[i] / multiplier, scale, 1, rng, increaseOnly);
				prices[i] = (ushort)(flags.WrapPriceOverflow ? ((newPrice - 1) % 0xFFFF) + 1 : Min(newPrice, 0xFFFF));
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
				text[i] = prices[i].ToString() + " G";
			}

			var pointers = Get(ShopPointerOffset, ShopPointerCount * ShopPointerSize).ToUShorts();
			RepackShops(pointers);

			for (int i = (int)ShopType.Clinic; i < (int)ShopType.Inn + ShopSectionSize; i++)
			{
				if (pointers[i] != ShopNullPointer)
				{
					var priceBytes = Get(ShopPointerBase + pointers[i], 2);
					var priceValue = BitConverter.ToUInt16(priceBytes, 0);

					priceValue = (ushort)Scale(priceValue / multiplier, scale, 1, rng, increaseOnly);
					priceBytes = BitConverter.GetBytes(priceValue);
					Put(ShopPointerBase + pointers[i], priceBytes);
				}
			}
			if (flags.StartingGold)
			{
				var startingGold = BitConverter.ToUInt16(Get(StartingGoldOffset, 2), 0);

				startingGold = (ushort)Min(Scale(startingGold / multiplier, scale, 1, rng, increaseOnly), 0xFFFF);

				Put(StartingGoldOffset, BitConverter.GetBytes(startingGold));
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

			NonBossEnemies.ForEach(index => ScaleSingleEnemyStats(index, minStats, highStats, flags.WrapStatOverflow, flags.IncludeMorale, rng,
				(bool)flags.SeparateEnemyHPScaling, minHp, highHp, GetEvadeIntFromFlag(flags.EvadeCap)));
		}


		public void ScaleBossStats(MT19337 rng, Flags flags)
		{
			int minStats = (bool)flags.ClampMinimumBossStatScale ? 100 : flags.BossScaleStatsLow;
			int highStats = (bool)flags.ClampMinimumBossStatScale ? Math.Max(100, flags.BossScaleStatsHigh) : flags.BossScaleStatsHigh;
			int minHp = (bool)flags.ClampBossHPScaling ? 100 : flags.BossScaleHpLow;
			int highHp = (bool)flags.ClampBossHPScaling ? Math.Max(100, flags.BossScaleHpHigh) : flags.BossScaleHpHigh;
			Bosses.ForEach(index => ScaleSingleEnemyStats(index, minStats, highStats, flags.WrapStatOverflow, flags.IncludeMorale, rng,
				(bool)flags.SeparateBossHPScaling, minHp, highHp, GetEvadeIntFromFlag(flags.EvadeCap)));
		}

		public void ScaleSingleEnemyStats(int index, int lowPercentStats, int highPercentStats, bool wrapOverflow, bool includeMorale, MT19337 rng,
			bool separateHPScale, int lowPercentHp, int highPercentHp, int evadeClamp)
		{
			double lowDecimalStats = (double)lowPercentStats / 100.0;
			double highDecimalStats = (double)highPercentStats / 100.0;
			double lowDecimalHp = (double)lowPercentHp / 100.0;
			double highDecimalHp = (double)highPercentHp / 100.0;

			var enemy = Get(EnemyOffset + index * EnemySize, EnemySize);

			var hp = BitConverter.ToUInt16(enemy, 4);
			if(separateHPScale)
			{
				hp = (ushort)Min(RangeScale(hp, lowDecimalHp, highDecimalHp, 1.0, rng), 0x7FFF);
			} else
			{
				hp = (ushort)Min(RangeScale(hp, lowDecimalStats, highDecimalStats, 1.0, rng), 0x7FFF);
			}
			var hpBytes = BitConverter.GetBytes(hp);
			Array.Copy(hpBytes, 0, enemy, 4, 2);

			var newMorale = includeMorale ? RangeScale(enemy[6], lowDecimalStats, highDecimalStats, 0.25, rng) : enemy[6];
			var newEvade = RangeScale(enemy[8], lowDecimalStats, highDecimalStats, 1.0, rng);
			var newDefense = RangeScale(enemy[9], lowDecimalStats, highDecimalStats, 0.5, rng);
			var newHits = RangeScale(enemy[10], lowDecimalStats, highDecimalStats, 0.5, rng);
			var newHitPercent = RangeScale(enemy[11], lowDecimalStats, highDecimalStats, 1.0, rng);
			var newStrength = RangeScale(enemy[12], lowDecimalStats, highDecimalStats, 0.25, rng);
			var newCrit = RangeScale(enemy[13], lowDecimalStats, highDecimalStats, 0.5, rng);
			if (wrapOverflow)
			{
				newEvade = ((newEvade - 1) % 0xFF) + 1;
				newDefense = ((newDefense - 1) % 0xFF) + 1;
				newHits = ((newHits - 1) % 0xFF) + 1;
				newHitPercent = ((newHitPercent - 1) % 0xFF) + 1;
				newStrength = ((newStrength - 1) % 0xFF) + 1;
				newCrit = ((newCrit - 1) % 0xFF) + 1;
			}
			enemy[6] = (byte)Min(newMorale, 0xFF); // morale
			enemy[8] = (byte)Min(newEvade, evadeClamp); // evade
			enemy[9] = (byte)Min(newDefense, 0xFF); // defense
			enemy[10] = (byte)Max(Min(newHits, 0xFF), 1); // hits
			enemy[11] = (byte)Min(newHitPercent, 0xFF); // hit%
			enemy[12] = (byte)Min(newStrength, 0xFF); // strength
			enemy[13] = (byte)Min(newCrit, 0xFF); // critical%

			Put(EnemyOffset + index * EnemySize, enemy);
		}

		private int RangeScale(double value, double lowPercent, double highPercent, double adjustment, MT19337 rng)
		{
			double exponent = (rng != null) ? (double)rng.Next() / uint.MaxValue : 1.0; // A number from 0 - 1
			double logLowPercent = Log(lowPercent);
			double logDifference = Log(highPercent) - logLowPercent;
			exponent = exponent * logDifference + logLowPercent; // For example for 50-200% a number from -0.69 to 0.69, for 200-400% a number from 0.69 to 1.38
	
			double scaleValue = Exp(exponent); // A number from 0.5 to 2, or 2 to 4
			double adjustedScale = scaleValue > 1 ? (scaleValue - 1) * adjustment + 1 : 1 - ((1 - scaleValue) * adjustment); // Tightens the scale so some stats are not changed by as much. For example for strength (adjustment of 0.25) this becomes 0.875 to 1.25, 1.25 to 1.75 while for hp (adjustment of 1) this stays 0.5 to 2, 2 to 4
			return (int)(value * adjustedScale);
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

		private void ScaleEncounterRate(double overworldMultiplier, double dungeonMultiplier)
		{
			if (overworldMultiplier == 0)
			{
				PutInBank(0x1F, 0xC50E, Blob.FromHex("EAEA"));
			}
			else
			{
				byte[] threats = new byte[] { Data[OverworldThreatLevelOffset], Data[OceanThreatLevelOffset] };
				threats = threats.Select(x => (byte)Math.Ceiling(x * overworldMultiplier)).ToArray();
				Data[OverworldThreatLevelOffset] = threats[0];
				Data[OceanThreatLevelOffset] = threats[1];
			}

			if (dungeonMultiplier == 0)
			{
				PutInBank(0x1F, 0xCDCC, Blob.FromHex("1860"));
			}
			else
			{
				var threats = Get(ThreatLevelsOffset, ThreatLevelsSize).ToBytes();
				threats = threats.Select(x => (byte)Math.Ceiling(x * dungeonMultiplier)).ToArray();
				Put(ThreatLevelsOffset, threats);
			}
		}

		public void ExpGoldBoost(double bonus, double multiplier)
		{
			var enemyBlob = Get(EnemyOffset, EnemySize * EnemyCount);
			var enemies = enemyBlob.Chunk(EnemySize);

			foreach (var enemy in enemies)
			{
				var exp = BitConverter.ToUInt16(enemy, 0);
				var gold = BitConverter.ToUInt16(enemy, 2);

				exp += (ushort)(bonus / multiplier);
				gold += (ushort)(bonus / multiplier);

				var expBytes = BitConverter.GetBytes(exp);
				var goldBytes = BitConverter.GetBytes(gold);
				Array.Copy(expBytes, 0, enemy, 0, 2);
				Array.Copy(goldBytes, 0, enemy, 2, 2);
			}

			enemyBlob = Blob.Concat(enemies);

			Put(EnemyOffset, enemyBlob);

			var levelRequirementsBlob = Get(LevelRequirementsOffset, LevelRequirementsSize * LevelRequirementsCount);
			var levelRequirementsBytes = levelRequirementsBlob.Chunk(3).Select(threeBytes => new byte[] { threeBytes[0], threeBytes[1], threeBytes[2], 0 }).ToList();
			for (int i = 0; i < LevelRequirementsCount; i++)
			{
				uint levelRequirement = (uint)(BitConverter.ToUInt32(levelRequirementsBytes[i], 0) / multiplier);
				levelRequirementsBytes[i] = BitConverter.GetBytes(levelRequirement);
			}

			Put(LevelRequirementsOffset, Blob.Concat(levelRequirementsBytes.Select(bytes => (Blob)new byte[] { bytes[0], bytes[1], bytes[2] })));

			// A dirty, ugly, evil piece of code that sets the level requirement for level 2, even though that's already defined in the above table.
			byte firstLevelRequirement = Data[0x7C04B];
			firstLevelRequirement = (byte)(firstLevelRequirement / multiplier);
			Data[0x7C04B] = firstLevelRequirement;
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
	}
}
