using FF1Lib.procgen;
using System.Diagnostics;

namespace FF1Lib.Procgen
{
	public struct MapRequirements
	{
		public const int Width = Map.RowLength;
		public const int Height = Map.RowCount;

		public MapIndex MapIndex;
		public Tile Floor;
		public Tile InRoomFloor;
		public Tile OutOfBounds;
		public Tile Barrier;

		public IEnumerable<RoomSpec> Rooms; // Precisely fixed rooms and npcs. RENAME THIS TO REGIONS THEY CAN BE ANYTHING REALLY
		public IEnumerable<byte> Objects;   // In room items like chests and decorations. They ought to always be reachable on foot.
		public IEnumerable<byte> Portals;   // Warps, Teleports, Exits that should be considered traverseable and must be reachable.
		public IEnumerable<byte> Traps;     // Trap tiles assumed to be in rooms.
		public IEnumerable<int> FreeNPCs;   // Random extra NPCs outside of rooms. (Coords can be ignored)

		public FF1Rom Rom;
	}

	public enum MapGeneratorStrategy
	{
		Cellular,
		WaterfallClone,
		Square,
		BSPTree
	}

	public interface IMapGeneratorEngine
	{
		/// <summary>
		/// Makes a map! Returns null if generation failed for some reason.
		/// </summary>
		CompleteMap Generate(MT19337 rng, MapRequirements reqs);
	}

	public class MapGenerator
	{
		private const int MAX_MAP_ITERATIONS = 50;

		public CompleteMap Generate(MT19337 rng, MapGeneratorStrategy strategy, MapRequirements reqs)
		{
			CompleteMap map = null;

			if (reqs.MapIndex == MapIndex.Waterfall)
			{
				reqs.Floor = Tile.WaterfallRandomEncounters;
				reqs.InRoomFloor = Tile.WaterfallInside;
				reqs.FreeNPCs = Enumerable.Range(1, 10);
				reqs.Rooms = new List<RoomSpec>
				{
					/*new RoomSpec
					{
						Tiledata = new byte[,] {
							{ 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x02 },
							{ 0x03, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x05 },
							{ 0x03, 0x46, 0x46, 0x46, 0x46, 0x46, 0x46, 0x05 },
							{ 0x06, 0x07, 0x48, 0x07, 0x07, 0x07, 0x07, 0x08 },
							{ 0x30, 0x30, 0x36, 0x30, 0x30, 0x30, 0x30, 0x30 },
							{ 0x49, 0x49, 0x3A, 0x49, 0x49, 0x49, 0x49, 0x49 }},
						NPCs = new List<NPC> { new NPC { Index = 0, Coord = (5, 2), InRoom = true, Stationary = false } },
					}*/
					new RoomSpec
					{
						Tiledata = new byte[,] {
							{ 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x02 },
							{ 0x03, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x05 },
							{ 0x03, 0x46, 0x46, 0x46, 0x46, 0x46, 0x46, 0x05 },
							{ 0x06, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x08 }},
						NPCs = new List<NPC> { new NPC { ObjectId = ObjectId.CubeBot, Index = 0, Coord = (5, 2), InRoom = true, Stationary = false } },
					}
				};
				reqs.Portals = new byte[] { (byte)Tile.WarpUp };

				IMapGeneratorEngine engine = GetEngine(strategy);
				int iterations = 0;
				while (iterations < MAX_MAP_ITERATIONS && map == null)
				{
					Console.WriteLine($"Generating {reqs.MapIndex} - iteration #{iterations}");
					map = engine.Generate(rng, reqs);
					iterations++;
				}

				if (map == null)
				{
					throw new InsaneException($"Couldn't generate map using {strategy} after maximum {iterations} iterations.");
				}

				// add the reqs we used
				map.Requirements = reqs;
			}
			else if (reqs.MapIndex == MapIndex.EarthCaveB1)
			{
				reqs.Floor = Tile.EarthCaveRandomEncounters;
				reqs.InRoomFloor = Tile.EarthCaveInside;
				reqs.OutOfBounds = Tile.EarthCaveOOB;
				reqs.Barrier = Tile.EarthCaveRockA;
				reqs.FreeNPCs = new int[] { };
				reqs.Rooms = new List<RoomSpec> { };
				reqs.Portals = new byte[] { (byte)Tile.WarpUp, 0x24 };
				reqs.Objects = Enumerable.Range(0x42, 5).Select(x => (byte)x);
				reqs.Traps = Enumerable.Repeat(0x1D, 3).Select(x => (byte)x);

				IMapGeneratorEngine engine = GetEngine(strategy);
				map = engine.Generate(rng, reqs);

				// add the reqs we used
				map.Requirements = reqs;
			}
			else if (reqs.MapIndex == MapIndex.EarthCaveB2)
			{
				reqs.Floor = Tile.EarthCaveRandomEncounters;
				reqs.InRoomFloor = Tile.EarthCaveInside;
				reqs.OutOfBounds = Tile.EarthCaveOOB;
				reqs.Barrier = Tile.EarthCaveRockA;
				reqs.FreeNPCs = Enumerable.Range(0, 13);
				reqs.Rooms = new List<RoomSpec> { };
				reqs.Portals = new byte[] { (byte)Tile.WarpUp, 0x25 };
				reqs.Objects = Enumerable.Range(0x47, 6).Select(x => (byte)x);
				reqs.Traps = Enumerable.Range(0x1D, 1).Select(x => (byte)x);

				IMapGeneratorEngine engine = GetEngine(strategy);
				map = engine.Generate(rng, reqs);

				// add the reqs we used
				map.Requirements = reqs;
			}
			else
			{
				throw new ArgumentOutOfRangeException();
			}

			// Free NPC placement doesn't require the engine
			var locations = map.Map.Where(element => element.Tile == reqs.Floor).ToList();
			reqs.FreeNPCs.ToList().ForEach(npc =>
			{
				var location = locations.SpliceRandom(rng);
				reqs.Rom.MoveNpc(reqs.MapIndex, npc, location.X, location.Y, false, false);
			});

			if (Debugger.IsAttached)
			{
				Console.Write(map.AsText());
			}

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
				case MapGeneratorStrategy.Square:
					return new RectilinearGenerator();
				case MapGeneratorStrategy.BSPTree:
					return new BSPTreeEngine();
				default:
					return null;
			}
		}
	}


}
