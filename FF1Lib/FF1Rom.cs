using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RomUtilities;
using static System.Math;
using System.Collections;

namespace FF1Lib
{
	// ReSharper disable once InconsistentNaming
	public partial class FF1Rom : NesRom
	{
		public const string Version = "1.3.0";

		public const int CopyrightOffset1 = 0x384A8;
		public const int CopyrightOffset2 = 0x384BA;

		public const int RngOffset = 0x3F100;
		public const int RngSize = 256;

		public FF1Rom(string filename) : base(filename)
		{}

		public void Randomize(Blob seed, Flags flags)
		{
			var rng = new MT19337(BitConverter.ToUInt32(seed, 0));

			EasterEggs();

			if (flags.Treasures)
			{
				ShuffleTreasures(rng, flags.EarlyCanoe, flags.Ordeals);
			}

			if (flags.Shops)
			{
				ShuffleShops(rng, flags.EnemyStatusAttacks);
			}

			if (flags.MagicShops)
			{
				ShuffleMagicShops(rng);
			}

			if (flags.MagicLevels)
			{
				ShuffleMagicLevels(rng, flags.MagicPermissions);
			}

			if (flags.Rng)
			{
				ShuffleRng(rng);
			}

			if (flags.EnemyScripts)
			{
				ShuffleEnemyScripts(rng);
			}

			if (flags.EnemySkillsSpells)
			{
				ShuffleEnemySkillsSpells(rng);
			}

			if (flags.EnemyStatusAttacks)
			{
				ShuffleEnemyStatusAttacks(rng);
			}

			if (flags.Ordeals)
			{
				ShuffleOrdeals(rng);
			}

			if (flags.EarlyRod)
			{
				EnableEarlyRod();
			}

			if (flags.EarlyCanoe)
			{
				EnableEarlyCanoe();
			}

			if (flags.NoPartyShuffle)
			{
				DisablePartyShuffle();
			}

			if (flags.SpeedHacks)
			{
				EnableSpeedHacks();
			}

			if (flags.IdentifyTreasures)
			{
				EnableIdentifyTreasures();
			}

			if (flags.Dash)
			{
				EnableDash();
			}

			if (flags.BuyTen)
			{
				EnableBuyTen();
			}

			if (flags.HouseMPRestoration)
			{
				FixHouse();
			}

			if (flags.WeaponStats)
			{
				FixWeaponStats();
			}

			if (flags.PriceScaleFactor > 1)
			{
				ScalePrices(flags.PriceScaleFactor, rng);
			}

			if (flags.EnemyScaleFactor > 1)
			{
				ScaleEnemyStats(flags.EnemyScaleFactor, rng);
			}

			if (flags.ExpMultiplier > 1 || flags.ExpBonus > 0)
			{
				ExpGoldBoost(flags.ExpBonus * 10, flags.ExpMultiplier);
			}

			WriteSeedAndFlags(Version, seed.ToHex(), EncodeFlagsText(flags));
		}

		public override bool Validate()
		{
			return Get(0, 16) == Blob.FromHex("06400e890e890e401e400e400e400b42");
		}

		public void WriteSeedAndFlags(string version, string seed, string flags)
		{
			var seedBytes = FF1Text.TextToBytes($"{version}  {seed}");
			var flagBytes = FF1Text.TextToBytes($"{flags}");
			var padding = new byte[15 - flagBytes.Length];
			for (int i = 0; i < padding.Length; i++)
			{
				padding[i] = 0xFF;
			}

			Put(CopyrightOffset1, seedBytes);
			Put(CopyrightOffset2, padding + flagBytes);
		}

		public void ShuffleRng(MT19337 rng)
		{
			var rngTable = Get(RngOffset, RngSize).Chunk(1).ToList();
			rngTable.Shuffle(rng);

			Put(RngOffset, rngTable.SelectMany(blob => blob.ToBytes()).ToArray());
		}

		public void ExpGoldBoost(double bonus, double multiplier)
		{
			var enemyBlob = Get(EnemyOffset, EnemySize*EnemyCount);
			var enemies = enemyBlob.Chunk(EnemySize);

			foreach (var enemy in enemies)
			{
				var exp = BitConverter.ToUInt16(enemy, 0);
				var gold = BitConverter.ToUInt16(enemy, 2);

				exp = (ushort)Min(bonus + exp*multiplier, 0x7FFF);
				gold = (ushort)Min(bonus + gold*multiplier, 0x7FFF);

				var expBytes = BitConverter.GetBytes(exp);
				var goldBytes = BitConverter.GetBytes(gold);
				Array.Copy(expBytes, 0, enemy, 0, 2);
				Array.Copy(goldBytes, 0, enemy, 2, 2);
			}

			enemyBlob = Blob.Concat(enemies);

			Put(EnemyOffset, enemyBlob);
		}

		public static string EncodeFlagsText(Flags flags)
		{
			var bits = new BitArray(19);

			bits[0] = flags.Treasures;
			bits[1] = flags.Shops;
			bits[2] = flags.MagicShops;
			bits[3] = flags.MagicLevels;
			bits[4] = flags.MagicPermissions;
			bits[5] = flags.Rng;
			bits[6] = flags.EnemyScripts;
			bits[7] = flags.EnemySkillsSpells;
			bits[8] = flags.EnemyStatusAttacks;
			bits[9] = flags.Ordeals;

			bits[10] = flags.EarlyRod;
			bits[11] = flags.EarlyCanoe;
			bits[12] = flags.NoPartyShuffle;
			bits[13] = flags.SpeedHacks;
			bits[14] = flags.IdentifyTreasures;
			bits[15] = flags.Dash;
			bits[16] = flags.BuyTen;

			bits[17] = flags.HouseMPRestoration;
			bits[18] = flags.WeaponStats;

			var bytes = new byte[3];
			// Freaking .NET Core doesn't have BitArray.CopyTo
			for (int i = 0; i < bits.Length; i++)
			{
				bytes[i/8] |= (byte)((bits[i] ? 1 : 0) << (i%8));
			}

			var text = Convert.ToBase64String(bytes);
			text = text.TrimEnd('=');
			text = text.Replace('+', '!');
			text = text.Replace('/', '%');

			text += SliderToBase64((int)(10 * flags.PriceScaleFactor));
			text += SliderToBase64((int)(10 * flags.EnemyScaleFactor));
			text += SliderToBase64((int)(10 * flags.ExpMultiplier));
			text += SliderToBase64((int)flags.ExpBonus);

			return text;
		}

		public static Flags DecodeFlagsText(string text)
		{
			var bitString = text.Substring(0, 4);
			bitString = bitString.Replace('!', '+');
			bitString = bitString.Replace('%', '/');

			var bytes = Convert.FromBase64String(bitString);
			var bits = new BitArray(bytes);

			return new Flags
			{
				Treasures = bits[0],
				Shops = bits[1],
				MagicShops = bits[2],
				MagicLevels = bits[3],
				MagicPermissions = bits[4],
				Rng = bits[5],
				EnemyScripts = bits[6],
				EnemySkillsSpells = bits[7],
				EnemyStatusAttacks = bits[8],
				Ordeals = bits[9],

				EarlyRod = bits[10],
				EarlyCanoe = bits[11],
				NoPartyShuffle = bits[12],
				SpeedHacks = bits[13],
				IdentifyTreasures = bits[14],
				Dash = bits[15],
				BuyTen = bits[16],

				HouseMPRestoration = bits[17],
				WeaponStats = bits[18],

				PriceScaleFactor = Base64ToSlider(text[4]) / 10.0,
				EnemyScaleFactor = Base64ToSlider(text[5]) / 10.0,
				ExpMultiplier = Base64ToSlider(text[6]) / 10.0,
				ExpBonus = Base64ToSlider(text[7])
			};
		}

		private static char SliderToBase64(int value)
		{
			if (value < 0 || value > 63)
			{
				throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be between 0 and 63.");
			}
			else if (value < 10)
			{
				return (char)('0' + value);
			}
			else if (value < 36)
			{
				return (char)('A' + value - 10);
			}
			else if (value < 62)
			{
				return (char)('a' + value - 36);
			}
			else if (value == 62)
			{
				return '!';
			}
			else
			{
				return '%';
			}
		}

		private static int Base64ToSlider(char value)
		{
			if (value >= '0' && value <= '9')
			{
				return value - '0';
			}
			else if (value >= 'A' && value <= 'Z')
			{
				return value - 'A' + 10;
			}
			else if (value >= 'a' && value <= 'z')
			{
				return value - 'a' + 36;
			}
			else if (value == '!')
			{
				return 62;
			}
			else
			{
				return 63;
			}
		}
	}
}
