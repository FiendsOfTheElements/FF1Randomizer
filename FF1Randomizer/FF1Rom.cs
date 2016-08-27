using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RomUtilities;
using static System.Math;

namespace FF1Randomizer
{
	public class FF1Rom : NesRom
	{
		public const int TreasureOffset = 0x3100;
		public const int TreasureSize = 1;
		public const int TreasureCount = 256;

		public const int EnemyOffset = 0x30520;
		public const int EnemySize = 20;
		public const int EnemyCount = 128;

		public const int ShopPointerOffset = 0x38302; // 0x38300 technically, but the first one is unused.
		public const int ShopPointerBase = 0x30000;
		public const int ShopPointerSize = 2;
		public const int ShopPointerCount = 70;
		public const int ShopSectionSize = 10;
		public const ushort ShopNullPointer = 0x838E;

		public const int MagicOffset = 0x301E0;
		public const int MagicSize = 8;
		public const int MagicCount = 64;
		public const int MagicNamesOffset = 0x2BE03;
		public const int MagicNameSize = 5;
		public const int MagicTextPointersOffset = 0x304C0;
		public const int MagicPermissionsOffset = 0x3AD18;
		public const int MagicPermissionsSize = 8;
		public const int MagicPermissionsCount = 12;
		public const int MagicOutOfBattleOffset = 0x3AEFA;
		public const int MagicOutOfBattleSize = 7;
		public const int MagicOutOfBattleCount = 13;

		public const int AiOffset = 0x31020;
		public const int AiSize = 16;
		public const int AiCount = 44;

		public const int WeaponOffset = 0x30000;
		public const int WeaponSize = 8;
		public const int WeaponCount = 40;

		public const int ArmorOffset = 0x30140;
		public const int ArmorSize = 4;
		public const int ArmorCount = 40;

		public FF1Rom(string filename) : base(filename)
		{}

		public override bool Validate()
		{
			return Get(0, 16) == Blob.FromHex("06400e890e890e401e400e400e400b42");
		}

		public void ShuffleTreasures(MT19337 rng)
		{
			DirectedGraph<byte> graph = new DirectedGraph<byte>();
			var treasureBlob = Get(TreasureOffset, TreasureSize * TreasureCount);
			var usedIndices = Enumerable.Range(0, TreasureCount).Except(TreasureConditions.NotUsed).ToList();
			var usedTreasures = usedIndices.Select(i => treasureBlob[i]).ToList();
			bool tofrQuestItem;
			do
			{
				usedTreasures.Shuffle(rng);

				for (int i = 0; i < usedIndices.Count; i++)
				{
					treasureBlob[usedIndices[i]] = usedTreasures[i];
				}

				// ToFR is only exitable using WARP or EXIT, so we don't want these items showing up there.
				// Especially not the TAIL, as that would make class change impossible.  And the CROWN being
				// here could block a LOT of valuable loot if you don't have a WW or BW.
				tofrQuestItem =
					TreasureConditions.ToFR.Contains(treasureBlob.IndexOf((byte)QuestItems.Crown)) ||
					TreasureConditions.ToFR.Contains(treasureBlob.IndexOf((byte)QuestItems.Tail)) ||
					TreasureConditions.ToFR.Contains(treasureBlob.IndexOf((byte)QuestItems.Adamant));
				if (tofrQuestItem)
				{
					continue;
				}

				var blockages = new List<Tuple<byte, int, List<int>>>
				{
					Tuple.Create((byte)QuestItems.Crown,   treasureBlob.IndexOf((byte)QuestItems.Crown),   TreasureConditions.CrownBlocked),
					Tuple.Create((byte)QuestItems.Tnt,     treasureBlob.IndexOf((byte)QuestItems.Tnt),     TreasureConditions.TntBlocked),
					Tuple.Create((byte)QuestItems.Ruby,    treasureBlob.IndexOf((byte)QuestItems.Ruby),    TreasureConditions.RubyBlocked),
					Tuple.Create((byte)QuestItems.Floater, treasureBlob.IndexOf((byte)QuestItems.Floater), TreasureConditions.FloaterBlocked),
					Tuple.Create((byte)QuestItems.Slab,    treasureBlob.IndexOf((byte)QuestItems.Slab),    TreasureConditions.SlabBlocked)
				};

				graph = new DirectedGraph<byte>();
				foreach (var blockage in blockages)
				{
					graph.AddNode(blockage.Item1);
				}

				foreach (var blocker in blockages)
				{
					foreach (var blockee in blockages)
					{
						if (blocker.Item3.Contains(blockee.Item2))
						{
							graph.AddEdge(blockee.Item1, blocker.Item1);
						}
					}
				}
			} while (tofrQuestItem || graph.HasCycles());

			Put(TreasureOffset, treasureBlob);
		}

		private enum ShopType
		{
			Weapon = 0,
			Armor = 10,
			White = 20,
			Black = 30,
			Clinic = 40,
			Inn = 50,
			Item = 60
		}

		public void ShuffleShops(MT19337 rng)
		{
			var pointers = Get(ShopPointerOffset, ShopPointerCount*ShopPointerSize).ToUShorts();

			RepackShops(pointers);

			ShuffleShopType(ShopType.Weapon, pointers, rng);
			ShuffleShopType(ShopType.Armor, pointers, rng);
			ShuffleShopType(ShopType.Item, pointers, rng);

			Put(ShopPointerOffset, Blob.FromUShorts(pointers));
		}

		public void ShuffleMagicShops(MT19337 rng)
		{
			var pointers = Get(ShopPointerOffset, ShopPointerCount * ShopPointerSize).ToUShorts();

			RepackShops(pointers);

			ShuffleShopType(ShopType.White, pointers, rng);
			ShuffleShopType(ShopType.Black, pointers, rng);

			Put(ShopPointerOffset, Blob.FromUShorts(pointers));
		}

		private void RepackShops(ushort[] pointers)
		{
			var allEntries = new List<byte>();
			var pointer = pointers[0];
			for (int shopType = 0; shopType < (int)ShopType.Item + ShopSectionSize; shopType += ShopSectionSize)
			{
				var sectionShops = GetShops((ShopType)shopType, pointers);
				allEntries.AddRange(sectionShops.SelectMany(list =>
				{
					if (list.Any())
					{
						list.Add(0);
					}

					return list;
				}));

				for (int i = 0; i < ShopSectionSize; i++)
				{
					if (pointers[shopType + i] != ShopNullPointer)
					{
						pointers[shopType + i] = pointer;
						pointer += (ushort)sectionShops[i].Count;
					}
				}
			}

			Put(ShopPointerBase + pointers[0], allEntries.ToArray());
		}

		private void ShuffleShopType(ShopType shopType, ushort[] pointers, MT19337 rng)
		{
			var shops = GetShops(shopType, pointers);

			var allEntries = shops.SelectMany(list => list).ToList();
			allEntries.Shuffle(rng);

			var newShops = new List<byte>[ShopSectionSize];
			var entry = 0;
			for (int i = 0; i < ShopSectionSize; i++)
			{
				newShops[i] = new List<byte>();
				if (pointers[(int)shopType + i] != ShopNullPointer)
				{
					newShops[i].Add(allEntries[entry++]);
				}
			}

			while (entry < allEntries.Count)
			{
				var tryShop = newShops[rng.Between(0, ShopSectionSize - 1)];
				if (tryShop.Count > 0 && tryShop.Count < 5 && !tryShop.Contains(allEntries[entry]))
				{
					tryShop.Add(allEntries[entry++]);
				}
			}

			foreach (var newShop in newShops)
			{
				newShop.Add(0);
			}

			var pointer = pointers[(int)shopType];
			for (int i = 0; i < ShopSectionSize; i++)
			{
				if (newShops[i].Count > 1)
				{
					Put(ShopPointerBase + pointer, newShops[i].ToArray());

					pointers[(int)shopType + i] = pointer;
					pointer += (ushort)newShops[i].Count;
				}
			}
		}

		private List<byte>[] GetShops(ShopType shopType, ushort[] pointers)
		{
			var shops = new List<byte>[ShopSectionSize];
			for (int i = 0; i < ShopSectionSize; i++)
			{
				shops[i] = new List<byte>();
				if (pointers[(int)shopType + i] != ShopNullPointer)
				{
					if (shopType == ShopType.Clinic || shopType == ShopType.Inn)
					{
						shops[i].AddRange(Get(ShopPointerBase + pointers[(int)shopType + i], 2).ToBytes());
					}
					else
					{
						var shopEntries = Get(ShopPointerBase + pointers[(int)shopType + i], 5);
						for (int j = 0; j < 5 && shopEntries[j] != 0; j++)
						{
							shops[i].Add(shopEntries[j]);
						}
					}
				}
			}

			return shops;
		}

		private struct MagicSpell
		{
			public byte Index;
			public Blob Data;
			public Blob Name;
			public byte TextPointer;
		}

		private readonly List<byte> _outOfBattleSpells = new List<byte> { 0, 16, 32, 48, 19, 51, 35, 24, 33, 56, 38, 40, 41 };

		public void ShuffleMagicLevels(MT19337 rng, bool keepPermissions)
		{
			var spells = Get(MagicOffset, MagicSize*MagicCount).Chunk(MagicSize);
			var names = Get(MagicNamesOffset, MagicNameSize*MagicCount).Chunk(MagicNameSize);
			var pointers = Get(MagicTextPointersOffset, MagicCount);

			// First we have to un-interleave white and black spells.
			var whiteSpells = new List<MagicSpell>();
			var blackSpells = new List<MagicSpell>();

			for (int i = 0; i < MagicCount; i++)
			{
				if ((i/4)%2 == 0)
				{
					whiteSpells.Add(new MagicSpell
					{
						Index = (byte)i,
						Data = spells[i],
						Name = names[i],
						TextPointer = pointers[i]
					});
				}
				else
				{
					blackSpells.Add(new MagicSpell
					{
						Index = (byte)i,
						Data = spells[i],
						Name = names[i],
						TextPointer = pointers[i]
					});
				}
			}

			whiteSpells.Shuffle(rng);
			blackSpells.Shuffle(rng);

			// Now we re-interleave the spells.
			var shuffledSpells = new List<MagicSpell>();
			for (int i = 0; i < MagicCount; i++)
			{
				var sourceIndex = 4*(i/8) + i%4;
				if ((i/4)%2 == 0)
				{
					shuffledSpells.Add(whiteSpells[sourceIndex]);
				}
				else
				{
					shuffledSpells.Add(blackSpells[sourceIndex]);
				}
			}

			Put(MagicOffset, shuffledSpells.Select(spell => spell.Data).Aggregate((seed, next) => seed + next));
			Put(MagicNamesOffset, shuffledSpells.Select(spell => spell.Name).Aggregate((seed, next) => seed + next));
			Put(MagicTextPointersOffset, shuffledSpells.Select(spell => spell.TextPointer).ToArray());

			if (keepPermissions)
			{
				// Shuffle the permissions the same way the spells were shuffled.
				for (int c = 0; c < MagicPermissionsCount; c++)
				{
					var oldPermissions = Get(MagicPermissionsOffset + c*MagicPermissionsSize, MagicPermissionsSize);

					var newPermissions = new byte[MagicPermissionsSize];
					for (int i = 0; i < 8; i++)
					{
						for (int j = 0; j < 8; j++)
						{
							var oldIndex = shuffledSpells[8*i + j].Index;
							var oldPermission = (oldPermissions[oldIndex/8] & (0x80 >> oldIndex%8)) >> (7 - oldIndex%8);
							newPermissions[i] |= (byte)(oldPermission << (7 - j));
						}
					}

					Put(MagicPermissionsOffset + c*MagicPermissionsSize, newPermissions);
				}
			}

			// Map old indices to new indices.
			var newIndices = new byte[MagicCount];
			for (byte i = 0; i < MagicCount; i++)
			{
				newIndices[shuffledSpells[i].Index] = i;
			}

			// Fix enemy spell pointers to point to where the spells are now.
			var ais = Get(AiOffset, AiSize*AiCount).Chunk(AiSize);
			foreach (var ai in ais)
			{
				// Bytes 2-9 are magic spells.
				for (int i = 2; i < 10; i++)
				{
					if (ai[i] != 0xFF)
					{
						ai[i] = newIndices[ai[i]];
					}
				}
			}
			Put(AiOffset, ais.SelectMany(ai => ai.ToBytes()).ToArray());

			// Fix weapon and armor spell pointers to point to where the spells are now.
			var weapons = Get(WeaponOffset, WeaponSize*WeaponCount).Chunk(WeaponSize);
			foreach (var weapon in weapons)
			{
				if (weapon[3] != 0x00)
				{
					weapon[3] = (byte)(newIndices[weapon[3] - 1] + 1);
				}
			}
			Put(WeaponOffset, weapons.SelectMany(weapon => weapon.ToBytes()).ToArray());

			var armors = Get(ArmorOffset, ArmorSize*ArmorCount).Chunk(ArmorSize);
			foreach (var armor in armors)
			{
				if (armor[3] != 0x00)
				{
					armor[3] = (byte)(newIndices[armor[3] - 1] + 1);
				}
			}
			Put(ArmorOffset, armors.SelectMany(armor => armor.ToBytes()).ToArray());

			// Fix the crazy out of battle spell system.
			var outOfBattleSpellOffset = MagicOutOfBattleOffset;
			for (int i = 0; i < MagicOutOfBattleCount; i++)
			{
				var oldSpellIndex = _outOfBattleSpells[i];
				var newSpellIndex = newIndices[oldSpellIndex];

				Put(outOfBattleSpellOffset, new [] { (byte)(newSpellIndex + 0xB0) });

				outOfBattleSpellOffset += MagicOutOfBattleSize;
			}
		}

		public void ScaleEnemyStats(double scale, MT19337 rng)
		{
			var enemies = Get(EnemyOffset, EnemySize*EnemyCount).Chunk(EnemySize);
			foreach (var enemy in enemies)
			{
				var hp = BitConverter.ToUInt16(enemy, 4);
				hp = (ushort)Min(Scale(hp, scale, rng), 0x7FFF);
				var hpBytes = BitConverter.GetBytes(hp);
				Array.Copy(hpBytes, 0, enemy, 4, 2);

				// enemy[6] = (byte)Math.Min(Scale(enemy[6], scale, rng), 0xFF); // fear (disabled because it's too silly)
				enemy[8] = (byte)Min(Scale(enemy[8], scale, rng), 0xFF); // evade
				enemy[9] = (byte)Min(Scale(enemy[9], scale, rng), 0xFF); // defense
				enemy[10] = (byte)Max(Min(Scale(enemy[10], scale, rng), 0xFF), 1); // hits
				enemy[11] = (byte)Min(Scale(enemy[11], scale, rng), 0xFF); // hit%
				enemy[12] = (byte)Min(Scale(enemy[12], scale, rng), 0xFF); // strength
				enemy[13] = (byte)Min(Scale(enemy[13], scale, rng), 0xFF); // critical%
			}

			Put(EnemyOffset, enemies.SelectMany(enemy => enemy.ToBytes()).ToArray());
		}

		private int Scale(int value, double scale, MT19337 rng)
		{
			var exponent = (double)rng.Next()/uint.MaxValue*2.0 - 1.0;
			return (int)Round(Pow(scale, exponent)*value, MidpointRounding.AwayFromZero);
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
	}
}
