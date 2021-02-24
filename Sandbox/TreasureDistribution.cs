using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using FF1Lib;

namespace Sandbox
{
	public static class TreasureDistribution
	{
		public static void Test()
		{
			Flags flags = new()
			{
				Treasures = true,
				IncentivizeIceCave = false,
				IncentivizeOrdeals = false,
				Shops = false,
				MagicShops = false,
				MagicLevels = false,
				MagicPermissions = false,
				Rng = false,
				EnemyScripts = false,
				EnemySkillsSpells = false,
				EnemyStatusAttacks = false,
				OrdealsPillars = false,

				EarlySarda = true,
				EarlySage = true,
				EarlyOrdeals = true,
				NoPartyShuffle = false,
				SpeedHacks = false,
				IdentifyTreasures = false,
				Dash = false,
				BuyTen = false,

				HouseMPRestoration = false,
				WeaponStats = false,
				ChanceToRun = false,
				SpellBugs = false,
				EnemyStatusAttackBug = false,

				PriceScaleFactorLow = 100,
				PriceScaleFactorHigh = 100,
				ExpMultiplier = 1.0,
				ExpBonus = 0
			};

			Preferences preferences = new()
			{
				FunEnemyNames = false,
				PaletteSwap = false,
				TeamSteak = false,
				ModernBattlefield = false,
				Music = MusicShuffle.None
			};

			string filename = "ff1.nes";
			MemoryStream memoryStream = new();
			using (FileStream fs = new(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				fs.CopyTo(memoryStream);
			}

			memoryStream.Seek(0, SeekOrigin.Begin);
			FF1Rom rom = new(memoryStream);
			for (int i = 0; i < 10000; i++)
			{
				RandomNumberGenerator rng = RandomNumberGenerator.Create();
				byte[] seed = new byte[8];
				rng.GetBytes(seed);

				rom.Randomize(seed, flags, preferences);

				if (i % 1000 == 0)
				{
					Console.WriteLine($"Generated {i} seeds...");
				}
			}

			Console.WriteLine("Test complete.");
		}
	}
}
