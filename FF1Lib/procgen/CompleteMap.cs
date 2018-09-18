using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FF1Lib.Procgen
{
	public class CompleteMap
	{
		public Map Map;
		public Coordinate Entrance;
		public MapRequirements Requirements;

		/* -- The rest of this is text map drawing. -- */

		static private Dictionary<Tile, char> asciiTileMap = new Dictionary<Tile, char>
		{
			{ Tile.RoomBackLeft, '/' },
			{ Tile.RoomBackCenter, '=' },
			{ Tile.RoomBackRight, '\\' },
			{ Tile.RoomLeft, '|' },
			{ Tile.RoomCenter, '`' },
			{ Tile.RoomRight, '|' },
			{ Tile.RoomFrontLeft, '\\' },
			{ Tile.RoomFrontCenter, '_' },
			{ Tile.RoomFrontRight, '/' },
			{ Tile.Ladder, '#' },
			{ Tile.LadderHole, 'o' },
			{ Tile.WarpUp, '<' },
			{ Tile.InsideWall, '-' },
			{ Tile.WallLeft, '+' },
			{ Tile.WallRight, '+' },
			{ Tile.HallwayLeft, '|' },
			{ Tile.HallwayRight, '|' },
			{ Tile.FloorSafe, 'f' },
			{ Tile.Impassable, 'X' },
			{ Tile.Door, 'D' },
			{ Tile.Doorway, 'd' },
			{ Tile.Lava, 'L' },
			{ Tile.EarthCaveRockA, 'X' },
			{ Tile.EarthCaveRockB, 'X' },
			{ Tile.EarthCaveRandomEncounters, ' ' },
			{ Tile.EarthCaveOOB, '0' },
			{ Tile.WaterfallInside, '_' },
			{ Tile.WaterfallRandomEncounters, ' ' },
			{ (Tile)0xFE, 'X' },
			{ (Tile)0xFF, ' ' },
		};

		public string AsText()
		{
			StringBuilder sb = new StringBuilder(MapRequirements.Height * MapRequirements.Width);

			int y, x;
			var otherCharMap = new Dictionary<int, char>();
			var seenValues = new HashSet<int>();

			// other chars to pull from when automatically mapping byte values to chars
			// just letters obviously. someone smart can come up with a better idea.
			var otherChars = new Stack<char>("ABCDFGHIJKLMNPRSTUVWYZxabcdefghijklmnopqrstuwvz");

			sb.Append($"Map {Requirements.MapId}\n");

			for (y = 0; y < MapRequirements.Height; y++)
			{
				bool seenEntrance = false;
				for (x = 0; x < MapRequirements.Width; x++)
				{
					int value = Map[y, x];
					if (asciiTileMap.ContainsKey((Tile) value))
						sb.Append(asciiTileMap[(Tile) value]);
					else if (otherCharMap.ContainsKey(value))
						sb.Append(otherCharMap[value]);
					else
					{
						char newchar = otherChars.Pop();
						otherCharMap.Add(value, newchar);
						sb.Append(newchar);
					}

					seenValues.Add(value);
					if (value == (int) Tile.WarpUp)
						seenEntrance = true;
				}

				if (seenEntrance)
					sb.Append("\t <-- Entrance this row");

				sb.Append("\n");
			}

			sb.Append("\n");

			// make a nice little table
			int index = 0;
			var legend_groups = asciiTileMap.
				Where(entry => seenValues.Contains((int) entry.Key)).
				Select(entry => String.Format("{0,-30}  {1,2}", entry.Key, entry.Value)).
				GroupBy(str => index++ / 2).ToList();
			legend_groups.ForEach(grouping =>
				sb.Append("\t").Append(String.Join("\t\t\t", grouping)).Append("\n")
			);
			sb.AppendLine();

			index = 0;
			legend_groups = otherCharMap.
				Select(entry => $"{entry.Key:X2}        {entry.Value}").
				GroupBy(str => index++ / 4).ToList();
			legend_groups.ForEach(grouping =>
				sb.Append("\t").Append(String.Join("\t\t", grouping)).Append("\n")
			);

			return sb.ToString();
		}

	}
}
