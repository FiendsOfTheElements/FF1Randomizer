using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomUtilities;

namespace FF1Lib.Procgen
{
	public struct MapRequirements
	{
		public const int Width = Map.RowLength;
		public const int Height = Map.RowCount;

		public MapId MapId;
		public Tile Floor;

		public IEnumerable<RoomSpec> Rooms; // Precisely fixed rooms and npcs.
		public IEnumerable<byte> Treasures; // Random extra chests requested in random rooms.
		public IEnumerable<int> FreeNPCs;   // Random extra NPCs outside of rooms. (Coords can be ignored)

		public FF1Rom Rom;
	}

	public enum MapGeneratorStrategy
	{
		Cellular,
		WaterfallClone
	}

	public interface IMapGeneratorEngine
	{
		CompleteMap Generate(MT19337 rng, MapRequirements reqs);
	}

	public class MapGenerator
	{
		public CompleteMap Generate(MT19337 rng, MapGeneratorStrategy strategy, MapRequirements reqs)
		{
			CompleteMap map;

			if (reqs.MapId == MapId.Waterfall)
			{
				reqs.Floor = Tile.WaterfallRandomEncounters;
				reqs.Treasures = new List<byte> { };
				reqs.FreeNPCs = Enumerable.Range(1, 10);
				reqs.Rooms = new List<RoomSpec>
				{
					new RoomSpec
					{
						Tiledata = new byte[,] {
							{ 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x02 },
							{ 0x03, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x05 },
							{ 0x03, 0x46, 0x46, 0x46, 0x46, 0x46, 0x46, 0x05 },
							{ 0x06, 0x07, 0x48, 0x07, 0x07, 0x07, 0x07, 0x08 },
							{ 0x30, 0x30, 0x36, 0x30, 0x30, 0x30, 0x30, 0x30 },
							{ 0x49, 0x49, 0x3A, 0x49, 0x49, 0x49, 0x49, 0x49 }},
						NPCs = new List<NPC> { new NPC { Index = 0, Coord = (5, 2), InRoom = true, Stationary = false } },
					}
				};

				IMapGeneratorEngine engine = GetEngine(strategy);
				map = engine.Generate(rng, reqs);
			}
			else
			{
				throw new ArgumentOutOfRangeException();
			}

			// Free NPC placement doesn't require the engine
			reqs.FreeNPCs.ToList().ForEach(npc =>
			{
				for (int sanity = 0; sanity < 1000; ++sanity)
				{
					int x = rng.Between(0, MapRequirements.Width - 1);
					int y = rng.Between(0, MapRequirements.Height - 1);
					if (map.Map[y, x] == (byte)reqs.Floor)
					{
						reqs.Rom.MoveNpc(reqs.MapId, npc, x, y, false, false);
						return;
					}
				}
				throw new InsaneException("Couln't find a home for an npc.");
			});
#if DEBUG
			Console.Write(map.AsText());
#endif
			return map;
		}

		private IMapGeneratorEngine GetEngine(MapGeneratorStrategy strategy)
		{
			switch (strategy)
			{
				case MapGeneratorStrategy.Cellular:
					return new CellularGenerator();
				case MapGeneratorStrategy.WaterfallClone:
					return new WaterfallEngine();
				default:
					return null;
			}
		}
	}


}
