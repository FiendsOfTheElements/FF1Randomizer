using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomUtilities;
using static System.Math;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public const int PriceOffset = 0x37C00;
		public const int PriceSize = 2;
		public const int PriceCount = 240;

		// Scale is the geometric scale factor used with RNG.  Multiplier is where we make everything cheaper
		// instead of enemies giving more gold, so we don't overflow.
		public void ScalePrices(IScaleFlags flags, Blob[] text, MT19337 rng)
		{
			var scale = flags.PriceScaleFactor;
			var multiplier = flags.ExpMultiplier;
            var prices = Get(PriceOffset, PriceSize * PriceCount).ToUShorts();
			for (int i = 0; i < prices.Length; i++)
			{
				var newPrice = Scale(prices[i] / multiplier, scale, 1, rng);
				prices[i] = (ushort) (flags.WrapPriceOverflow ? ((newPrice - 1) % 0xFFFF) + 1 : Min(newPrice, 0xFFFF));
            }
            var questItemPrice = prices[(int)Item.Bottle];
            for (var i = 0; i < (int)Item.Tent; i++)
            {
                prices[i] = questItemPrice;
            }
			prices[(int)Item.WhiteShirt] = (ushort)(questItemPrice / 2);
			prices[(int)Item.BlackShirt] = (ushort)(questItemPrice / 2);
			prices[(int)Item.Ribbon] = questItemPrice;
            // Crystal can block Ship in early game where 50000 G would be too expensive
            prices[(int)Item.Crystal] = (ushort)(prices[(int)Item.Crystal] / 8);

			Put(PriceOffset, Blob.FromUShorts(prices));

			for (int i = GoldItemOffset; i < GoldItemOffset + GoldItemCount; i++)
			{
				text[i] = FF1Text.TextToBytes(prices[i].ToString() + " G");
			}

			var pointers = Get(ShopPointerOffset, ShopPointerCount * ShopPointerSize).ToUShorts();
			RepackShops(pointers);

			for (int i = (int)ShopType.Clinic; i < (int)ShopType.Inn + ShopSectionSize; i++)
			{
				if (pointers[i] != ShopNullPointer)
				{
					var priceBytes = Get(ShopPointerBase + pointers[i], 2);
					var priceValue = BitConverter.ToUInt16(priceBytes, 0);

					priceValue = (ushort)Scale(priceValue / multiplier, scale, 1, rng);
					priceBytes = BitConverter.GetBytes(priceValue);
					Put(ShopPointerBase + pointers[i], priceBytes);
				}
			}
			if (flags.StartingGold)
			{
				var startingGold = BitConverter.ToUInt16(Get(StartingGoldOffset, 2), 0);

				startingGold = (ushort)Min(Scale(startingGold / multiplier, scale, 1, rng), 0xFFFF);

				Put(StartingGoldOffset, BitConverter.GetBytes(startingGold));
			}
			
		}

		public void ScaleEnemyStats(double scale, bool wrapOverflow, MT19337 rng)
		{
			Enumerable.Range(0, EnemyCount).ToList().ForEach(index => ScaleSingleEnemyStats(index, scale, wrapOverflow, rng));
		}

		public void ScaleSingleEnemyStats(int index, double scale, bool wrapOverflow = false, MT19337 rng = null)
		{
			var enemy = Get(EnemyOffset + index * EnemySize, EnemySize);

			var hp = BitConverter.ToUInt16(enemy, 4);
			hp = (ushort)Min(Scale(hp, scale, 1.0, rng), 0x7FFF);
			var hpBytes = BitConverter.GetBytes(hp);
			Array.Copy(hpBytes, 0, enemy, 4, 2);

			var newMorale = Scale(enemy[6], scale, 0.25, rng);
			var newEvade = Scale(enemy[8], scale, 1.0, rng);
			var newDefense = Scale(enemy[9], scale, 0.5, rng);
			var newHits = Scale(enemy[10], scale, 0.5, rng);
			var newHitPercent = Scale(enemy[11], scale, 1.0, rng);
			var newStrength = Scale(enemy[12], scale, 0.25, rng);
			var newCrit = Scale(enemy[13], scale, 0.5, rng);
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
			enemy[8] = (byte)Min(newEvade, 0xF0); // evade clamped to 240
			enemy[9] = (byte)Min(newDefense, 0xFF); // defense
			enemy[10] = (byte)Max(Min(newHits, 0xFF), 1); // hits
			enemy[11] = (byte)Min(newHitPercent, 0xFF); // hit%
			enemy[12] = (byte)Min(newStrength, 0xFF); // strength
			enemy[13] = (byte)Min(newCrit, 0xFF); // critical%

			Put(EnemyOffset + index * EnemySize, enemy);
		}

		private int Scale(double value, double scale, double adjustment, MT19337 rng = null)
		{
			double exponent = rng == null ? 1.0 : (double)rng.Next() / uint.MaxValue * 2.0 - 1.0;
			double adjustedScale = 1.0 + adjustment * (scale - 1.0);

			return (int)Round(Pow(adjustedScale, exponent) * value, MidpointRounding.AwayFromZero);
		}

	}
}
