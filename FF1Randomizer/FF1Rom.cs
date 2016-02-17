using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ROMUtilities;

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

		public FF1Rom(string filename) : base(filename)
		{}

		public override bool Validate()
		{
			return Get(0, 16) == Blob.FromHex("06400e890e890e401e400e400e400b42");
		}

		public void ShuffleTreasures(MT19337 rng)
		{
			DirectedGraph<byte> graph;
			do
			{
				var treasureBlob = Get(TreasureOffset, TreasureSize*TreasureCount);

				var usedIndices = Enumerable.Range(0, TreasureCount).Except(TreasureConditions.NotUsed).ToList();
				var usedTreasures = usedIndices.Select(i => treasureBlob[i]).ToList();

				usedTreasures.Shuffle(rng);

				for (int i = 0; i < usedIndices.Count; i++)
				{
					treasureBlob[usedIndices[i]] = usedTreasures[i];
				}

				Put(TreasureOffset, treasureBlob);

				// This is wrong!  The RUBY blocks the CANOE, which blocks the AIRSHIP.
				// The northern hemisphere is therefore blocked by both the RUBY and the FLOATER.
				// The RUBY must be found by Earth Cave B3 or earlier.
				// Also, RUBY blocks treasure in the Titan's Cave, Gurgu Volcano, Ice Cave, and Castle of Ordeals, in addition to Earth Cave B4.

				var blockages = new List<Tuple<byte, int, List<int>>>
				{
					Tuple.Create((byte)QuestItems.Crown,   Array.IndexOf(treasureBlob, (byte)QuestItems.Crown),   TreasureConditions.CrownBlocked),
					Tuple.Create((byte)QuestItems.Tnt,     Array.IndexOf(treasureBlob, (byte)QuestItems.Tnt),     TreasureConditions.TntBlocked),
					Tuple.Create((byte)QuestItems.Ruby,    Array.IndexOf(treasureBlob, (byte)QuestItems.Ruby),    TreasureConditions.RubyBlocked),
					Tuple.Create((byte)QuestItems.Floater, Array.IndexOf(treasureBlob, (byte)QuestItems.Floater), TreasureConditions.FloaterBlocked),
					Tuple.Create((byte)QuestItems.Slab,    Array.IndexOf(treasureBlob, (byte)QuestItems.Slab),    TreasureConditions.SlabBlocked)
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
			} while (graph.HasCycles());
		}

		public void ExpGoldBoost(double factor)
		{
			var enemyBlob = Get(EnemyOffset, EnemySize*EnemyCount);
			var enemies = enemyBlob.Chunk(EnemySize);

			foreach (var enemy in enemies)
			{
				var exp = BitConverter.ToUInt16(enemy, 0);
				var gold = BitConverter.ToUInt16(enemy, 2);

				exp = (ushort)(exp*factor);
				gold = (ushort)(gold*factor);

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
