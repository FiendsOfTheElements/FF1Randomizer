using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomUtilities;
using System.Collections;
using System.IO;

namespace FF1Lib
{
	// ReSharper disable once InconsistentNaming
	public partial class FF1Rom : NesRom
	{
		public const string Version = "1.5.5";

		public const int CopyrightOffset1 = 0x384A8;
		public const int CopyrightOffset2 = 0x384BA;

		public const int RngOffset = 0x3F100;
		public const int RngSize = 256;

		public const int LevelRequirementsOffset = 0x2D000;
		public const int LevelRequirementsSize = 3;
		public const int LevelRequirementsCount = 49;

		public const int StartingGoldOffset = 0x301C;

		public const int GoldItemOffset = 108; // 108 items before gold chests
		public const int GoldItemCount = 68;

		public FF1Rom(string filename) : base(filename)
		{}

		public FF1Rom(Stream readStream) : base(readStream)
		{}

		private FF1Rom()
		{}

		public static async Task<FF1Rom> CreateAsync(Stream readStream)
		{
			var rom = new FF1Rom();
			await rom.LoadAsync(readStream);

			return rom;
		}

		public void Randomize(Blob seed, Flags flags)
		{
			var rng = new MT19337(BitConverter.ToUInt32(seed, 0));

			EasterEggs();

			// This has to be done before we shuffle spell levels.
			if (flags.SpellBugs)
			{
				FixSpellBugs();
			}

			if (flags.Treasures)
			{
				ShuffleTreasures(rng, flags.EarlyCanoe, flags.EarlyOrdeals, flags.IncentivizeIceCave, flags.IncentivizeOrdeals);
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

			if (flags.EarlyOrdeals)
			{
				EnableEarlyOrdeals();
			}

			if (flags.EarlyRod)
			{
				EnableEarlyRod();
			}

			if (flags.EarlyCanoe)
			{
				EnableEarlyCanoe();
			}

			if (flags.EarlyBridge)
			{
				EnableEarlyBridge();
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

			if (flags.ChanceToRun)
			{
				FixChanceToRun();
			}

			if (flags.EnemyStatusAttackBug)
			{
				FixEnemyStatusAttackBug();
			}

			if (flags.FunEnemyNames)
			{
				FunEnemyNames(flags.TeamSteak);
			}

			var itemText = ReadText(ItemTextPointerOffset, ItemTextPointerBase, ItemTextPointerCount);
			itemText[99] = FF1Text.TextToBytes("Ribbon ", useDTE: false);

			ExpGoldBoost(flags.ExpBonus, flags.ExpMultiplier);
			ScalePrices(flags.PriceScaleFactor, flags.ExpMultiplier, itemText, rng);

			WriteText(itemText, ItemTextPointerOffset, ItemTextPointerBase, ItemTextOffset);

			if (flags.EnemyScaleFactor > 1)
			{
				ScaleEnemyStats(flags.EnemyScaleFactor, rng);
			}

			// We have to do "fun" stuff last because it alters the RNG state.
			if (flags.PaletteSwap)
			{
				PaletteSwap(rng);
			}

			if (flags.TeamSteak)
			{
				TeamSteak();
			}

			if (flags.Music != MusicShuffle.None)
			{
				ShuffleMusic(flags.Music, rng);
			}

			if (flags.ShuffleLeader)
			{
				ShuffleLeader(rng);
			}

			WriteSeedAndFlags(Version, seed.ToHex(), EncodeFlagsText(flags));
		}

		public override bool Validate()
		{
			return Get(0, 16) == Blob.FromHex("06400e890e890e401e400e400e400b42");
		}

		public void WriteSeedAndFlags(string version, string seed, string flags)
		{
			var seedBytes = FF1Text.TextToBytes($"{version}  {seed}", useDTE: false);
			var flagBytes = FF1Text.TextToBytes($"{flags}", useDTE: false);
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
			byte firstLevelRequirement = Data[0x3C04B];
			firstLevelRequirement = (byte)(firstLevelRequirement / multiplier);
			Data[0x3C04B] = firstLevelRequirement;
		}

		public static string EncodeFlagsText(Flags flags)
		{
			var bits = new BitArray(32);
			int i = 0;

			bits[i++] = flags.Treasures;
			bits[i++] = flags.IncentivizeIceCave;
			bits[i++] = flags.IncentivizeOrdeals;
			bits[i++] = flags.Shops;
			bits[i++] = flags.MagicShops;
			bits[i++] = flags.MagicLevels;
			bits[i++] = flags.MagicPermissions;
			bits[i++] = flags.Rng;
			bits[i++] = flags.EnemyScripts;
			bits[i++] = flags.EnemySkillsSpells;
			bits[i++] = flags.EnemyStatusAttacks;
			bits[i++] = flags.Ordeals;

			bits[i++] = flags.EarlyRod;
			bits[i++] = flags.EarlyCanoe;
			bits[i++] = flags.EarlyOrdeals;
			bits[i++] = flags.EarlyBridge;
			bits[i++] = flags.NoPartyShuffle;
			bits[i++] = flags.SpeedHacks;
			bits[i++] = flags.IdentifyTreasures;
			bits[i++] = flags.Dash;
			bits[i++] = flags.BuyTen;

			bits[i++] = flags.HouseMPRestoration;
			bits[i++] = flags.WeaponStats;
			bits[i++] = flags.ChanceToRun;
			bits[i++] = flags.SpellBugs;
			bits[i++] = flags.EnemyStatusAttackBug;

			bits[i++] = flags.FunEnemyNames;
			bits[i++] = flags.PaletteSwap;
			bits[i++] = flags.TeamSteak;
			bits[i++] = flags.ShuffleLeader;
			bits[i++] = flags.Music == MusicShuffle.Standard;
			bits[i++] = flags.Music == MusicShuffle.Nonsensical;

			System.Diagnostics.Debug.Assert(i == bits.Length, "Unused bits writing flags.");

			var bytes = new byte[4];
			bits.CopyTo(bytes, 0);

			var text = Convert.ToBase64String(bytes);
			text = text.TrimEnd('=');
			text = text.Replace('+', '!');
			text = text.Replace('/', '%');

			text += SliderToBase64((int)(flags.PriceScaleFactor * 10.0));
			text += SliderToBase64((int)(flags.EnemyScaleFactor * 10.0));
			text += SliderToBase64((int)(flags.ExpMultiplier * 10.0));
			text += SliderToBase64((int)(flags.ExpBonus / 10.0));

			return text;
		}

		public static Flags DecodeFlagsText(string text)
		{
			var bitString = text.Substring(0, 6);
			bitString = bitString.Replace('!', '+');
			bitString = bitString.Replace('%', '/');
			bitString += "==";

			var bytes = Convert.FromBase64String(bitString);
			var bits = new BitArray(bytes);
			int i = 0;

			Flags flags = new Flags();
			flags.Treasures = bits[i++];
			flags.IncentivizeIceCave = bits[i++];
			flags.IncentivizeOrdeals = bits[i++];
			flags.Shops = bits[i++];
			flags.MagicShops = bits[i++];
			flags.MagicLevels = bits[i++];
			flags.MagicPermissions = bits[i++];
			flags.Rng = bits[i++];
			flags.EnemyScripts = bits[i++];
			flags.EnemySkillsSpells = bits[i++];
			flags.EnemyStatusAttacks = bits[i++];
			flags.Ordeals = bits[i++];

			flags.EarlyRod = bits[i++];
			flags.EarlyCanoe = bits[i++];
			flags.EarlyOrdeals = bits[i++];
			flags.EarlyBridge = bits[i++];
			flags.NoPartyShuffle = bits[i++];
			flags.SpeedHacks = bits[i++];
			flags.IdentifyTreasures = bits[i++];
			flags.Dash = bits[i++];
			flags.BuyTen = bits[i++];

			flags.HouseMPRestoration = bits[i++];
			flags.WeaponStats = bits[i++];
			flags.ChanceToRun = bits[i++];
			flags.SpellBugs = bits[i++];
			flags.EnemyStatusAttackBug = bits[i++];

			flags.FunEnemyNames = bits[i++];
			flags.PaletteSwap = bits[i++];
			flags.TeamSteak = bits[i++];
			flags.ShuffleLeader = bits[i++];

			flags.Music = bits[i] ? MusicShuffle.Standard :
				bits[i+1] ? MusicShuffle.Nonsensical :
				MusicShuffle.None;
			i += 2;

			flags.PriceScaleFactor = Base64ToSlider(text[6]) / 10.0;
			flags.EnemyScaleFactor = Base64ToSlider(text[7]) / 10.0;
			flags.ExpMultiplier = Base64ToSlider(text[8]) / 10.0;
			flags.ExpBonus = Base64ToSlider(text[9]) * 10.0;

			return flags;
		}

		private static char SliderToBase64(int value)
		{
			if (value < 0 || value > 63)
			{
				throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be between 0 and 63.");
			}
			else if (value < 26)
			{
				return (char)('A' + value);
			}
			else if (value < 52)
			{
				return (char)('a' + value - 26);
			}
			else if (value < 62)
			{
				return (char)('0' + value - 52);
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
			if (value >= 'A' && value <= 'Z')
			{
				return value - 'A';
			}
			else if (value >= 'a' && value <= 'z')
			{
				return value - 'a' + 26;
			}
			else if (value >= '0' && value <= '9')
			{
				return value - '0' + 52;
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
