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
		public const int PriceCount = 108;
		public const int MagicPriceOffset = 0x37D60;

		// Scale is the geometric scale factor used with RNG.  Multiplier is where we make everything cheaper
		// instead of enemies giving more gold, so we don't overflow.
		public void ScalePrices(double scale, double multiplier, MT19337 rng)
		{
			var prices = Get(PriceOffset, PriceSize * PriceCount).Chunk(PriceSize);
			foreach (var price in prices)
			{
				var priceValue = BitConverter.ToUInt16(price, 0);
				priceValue = (ushort)Min(Scale(priceValue / multiplier, scale, 1, rng), 0xFFFF);

				var priceBytes = BitConverter.GetBytes(priceValue);
				Array.Copy(priceBytes, 0, price, 0, 2);
			}
			Put(PriceOffset, prices.SelectMany(price => price.ToBytes()).ToArray());

			prices = Get(MagicPriceOffset, PriceSize * MagicCount).Chunk(PriceSize);
			foreach (var price in prices)
			{
				var priceValue = BitConverter.ToUInt16(price, 0);
				priceValue = (ushort)Min(Scale(priceValue / multiplier, scale, 1, rng), 0xFFFF);

				var priceBytes = BitConverter.GetBytes(priceValue);
				Array.Copy(priceBytes, 0, price, 0, 2);
			}
			Put(MagicPriceOffset, prices.SelectMany(price => price.ToBytes()).ToArray());

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
				enemy[8] = (byte)Min(Scale(enemy[8], scale, 1.0, rng), 0xFA); // evade clamped to 250
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
