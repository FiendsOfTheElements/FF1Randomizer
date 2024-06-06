using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FF1Lib.Procgen
{
    [JsonObject(MemberSerialization.OptIn)]
	public class CompleteMap
	{
	    [JsonProperty(Order=1)]
	    //[JsonConverter(typeof(StringEnumConverter))]
	        public string MapId;

		public MapIndex MapIndex;

		public Map Map;

	    [JsonProperty(Order = 2)]
	        public List<string> DecompressedMapRows { get; set; }

		public MapRequirements Requirements;

		public Coordinate Entrance;

	    [JsonProperty(Order = 4)]
	        public Dictionary<OverworldTeleportIndex, TeleportDestination> OverworldEntrances;

	    [JsonProperty(Order = 5)]
	        public Dictionary<TeleportIndex, TeleportDestination> MapDestinations;

	    [JsonProperty(Order = 5)]
	        public List<NPC> NPCs;

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

			sb.Append($"Map {Requirements.MapIndex}\n");

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

	    public static List<CompleteMap> LoadJson(Stream stream) {
		using (StreamReader rd = new StreamReader(stream))
		{
		    return LoadJson(rd);
		}
	    }

	    public static List<CompleteMap> LoadJson(StreamReader rd) {
			var objs = JsonConvert.DeserializeObject<List<CompleteMap>>(rd.ReadToEnd());

		    foreach (CompleteMap obj in objs)
			{
				obj.MapIndex = (MapIndex)Enum.Parse(typeof(MapIndex), obj.MapId);

				obj.Map = new Map(0);

				for (int y = 0; y < 64; y++)
				{
					byte[] row = Convert.FromBase64String(obj.DecompressedMapRows[y]);
					for (int x = 0; x < 64; x++) {
					obj.Map[y, x] = row[x];
					}
				}
				obj.DecompressedMapRows = null;

				foreach (var destination in obj.MapDestinations)
				{
					var oldDestination = obj.MapDestinations[destination.Key];
					obj.MapDestinations[destination.Key] = new TeleportDestination(oldDestination, new Coordinate(oldDestination.CoordinateX, oldDestination.CoordinateY));
				}

				foreach (var entrance in obj.OverworldEntrances)
				{
					var oldEntrance = obj.OverworldEntrances[entrance.Key];
					obj.OverworldEntrances[entrance.Key] = new TeleportDestination(oldEntrance, new Coordinate(oldEntrance.CoordinateX, oldEntrance.CoordinateY));
				}

			}

			return objs;
	    }

	    public void SaveJson(StreamWriter stream) {
		JsonSerializer serializer = new JsonSerializer();
		serializer.Formatting = Formatting.Indented;

		this.DecompressedMapRows = new List<string>();

		for (int y = 0; y < 64; y++)
		{
		    byte[] row = new byte[64];
		    for (int x = 0; x < 64; x++) {
			row[x] = this.Map[y, x];
		    }
		    this.DecompressedMapRows.Add(Convert.ToBase64String(row));
		}

		serializer.Serialize(stream, this);
	    }

	    public static void SaveJson(List<CompleteMap> maps, StreamWriter stream) {
		stream.Write("[\n");
		for (int i = 0; i < maps.Count; i++) {
		    maps[i].SaveJson(stream);
		    if (i+1 < maps.Count) {
			stream.Write(",\n");
		    }
		}
		stream.Write("]\n");
	    }
	}
}
