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
		public void ScalePrices(double scale, double multiplier, Blob[] text, MT19337 rng)
		{
            var prices = Get(PriceOffset, PriceSize * PriceCount).ToUShorts();
			for (int i = 0; i < prices.Length; i++)
			{
				prices[i] = (ushort)Min(Scale(prices[i] / multiplier, scale, 1, rng), 0xFFFF);
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

			var startingGold = BitConverter.ToUInt16(Get(StartingGoldOffset, 2), 0);

			startingGold = (ushort)Min(Scale(startingGold / multiplier, scale, 1, rng), 0xFFFF);

			Put(StartingGoldOffset, BitConverter.GetBytes(startingGold));
		}

		public void ScaleEnemyStats(double scale, MT19337 rng)
		{
			var enemies = Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);
			foreach (var enemy in enemies)
			{
				var hp = BitConverter.ToUInt16(enemy, 4);
				hp = (ushort)Min(Scale(hp, scale, 1.0, rng), 0x7FFF);
				var hpBytes = BitConverter.GetBytes(hp);
				Array.Copy(hpBytes, 0, enemy, 4, 2);

				enemy[6] = (byte)Min(Scale(enemy[6], scale, 0.25, rng), 0xFF); // morale
				enemy[8] = (byte)Min(Scale(enemy[8], scale, 1.0, rng), 0xF0); // evade clamped to 240
				enemy[9] = (byte)Min(Scale(enemy[9], scale, 0.5, rng), 0xFF); // defense
				enemy[10] = (byte)Max(Min(Scale(enemy[10], scale, 0.5, rng), 0xFF), 1); // hits
				enemy[11] = (byte)Min(Scale(enemy[11], scale, 1.0, rng), 0xFF); // hit%
				enemy[12] = (byte)Min(Scale(enemy[12], scale, 0.25, rng), 0xFF); // strength
				enemy[13] = (byte)Min(Scale(enemy[13], scale, 0.5, rng), 0xFF); // critical%
			}

			Put(EnemyOffset, enemies.SelectMany(enemy => enemy.ToBytes()).ToArray());
		}

		private int Scale(double value, double scale, double adjustment, MT19337 rng)
		{
			var exponent = (double)rng.Next() / uint.MaxValue * 2.0 - 1.0;
			var adjustedScale = 1.0 + adjustment * (scale - 1.0);

			return (int)Round(Pow(adjustedScale, exponent) * value, MidpointRounding.AwayFromZero);
		}
	}
}
