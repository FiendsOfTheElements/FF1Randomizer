using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace FF1Lib
{
	public enum CoordinateLocale
	{
		Overworld,
		Standard,
		StandardInRoom,
	}
	public struct Coordinate
	{
		private readonly short _identityValue;
		//public readonly byte X { get => Context > CoordinateLocale.Overworld ? (byte)(x & 0x3F) : x; }
		//public readonly byte Y { get => Context > CoordinateLocale.Overworld ? (byte)(y & 0x3F) : y; }
		public readonly byte X;
		public readonly byte Y;
		public readonly CoordinateLocale Context;
		public readonly byte RawX { get => Context == CoordinateLocale.StandardInRoom ? (byte)(X | 0x80) : X; }
		public readonly byte RawY { get => Context > CoordinateLocale.Overworld ? (byte)(Y | 0x80) : Y; }
		//public CoordinateLocale Context { get => ((X & 0x80) > 0) ? CoordinateLocale.StandardInRoom : (((Y & 0x80) > 0) ? CoordinateLocale.Overworld : CoordinateLocale.Standard); }
		public Coordinate(byte _x, byte _y, CoordinateLocale context)
		{
			X = _x;
			Y = _y;

			Context = context;

			if (context > CoordinateLocale.Overworld)
			{
				Y &= 0x3F;
				X &= 0x3F;
			}

			_identityValue = (short)(X * 256 + Y);
		}
		public Coordinate(byte _x, byte _y)
		{
			X = _x;
			Y = _y;

			if ((_x & 0x80) > 1)
			{
				Context = CoordinateLocale.StandardInRoom;
			}
			else
			{
				Context = CoordinateLocale.Standard;
			}

			if (Context > CoordinateLocale.Overworld)
			{
				Y &= 0x3F;
				X &= 0x3F;
			}

			_identityValue = (short)(X * 256 + Y);
		}
	}
	public struct LocationRequirement
	{
		public readonly IEnumerable<MapChange> MapChanges;
		public readonly Tuple<MapLocation, AccessRequirement> TeleportLocation;
		public LocationRequirement(IEnumerable<MapChange> mapChanges)
		{
			MapChanges = mapChanges.ToList();
			TeleportLocation = null;
		}
		public LocationRequirement(Tuple<MapLocation, AccessRequirement> teleportLocation)
		{
			MapChanges = null;
			TeleportLocation = teleportLocation;
		}
	}

	public abstract class LocationHelper
	{
		public static readonly IReadOnlyCollection<MapLocation> UnpalettableLocations = new List<MapLocation>
		{
			MapLocation.GurguVolcano3, MapLocation.GurguVolcano4, MapLocation.IceCave2, MapLocation.IceCave3,
			MapLocation.IceCaveBackExit, MapLocation.Cardia1, MapLocation.Cardia2,
			MapLocation.Cardia4, MapLocation.Cardia5, MapLocation.MarshCaveTop, MapLocation.SeaShrine4,
			MapLocation.SeaShrine5, MapLocation.SeaShrine6, MapLocation.SeaShrine7, MapLocation.TempleOfFiends1Room1,
			MapLocation.TempleOfFiends1Room2, MapLocation.TempleOfFiends1Room3, MapLocation.TempleOfFiends1Room4,
			MapLocation.TempleOfFiendsPhantom, MapLocation.TitansTunnelWest
		};
	};

	[JsonObject(MemberSerialization.OptIn)]
	public struct TeleportDestination
	{
	    [JsonProperty(Order=1)]
	    [JsonConverter(typeof(StringEnumConverter))]
		public readonly MapLocation Destination;

	    [JsonProperty(Order=2)]
	    [JsonConverter(typeof(StringEnumConverter))]
		public MapIndex Index;

	    [JsonProperty(Order=3)]
		public byte CoordinateX { get; private set; }

	    [JsonProperty(Order=4)]
		public byte CoordinateY { get; private set; }

	    [JsonProperty(Order=5, ItemConverterType = typeof(StringEnumConverter))]
		public readonly IEnumerable<TeleportIndex> Teleports;

	    [JsonProperty(Order=6)]
	    [JsonConverter(typeof(StringEnumConverter))]
		public readonly ExitTeleportIndex Exit;

		public Coordinate Coordinates { get; set; }
		public string SpoilerText =>
		$"{Enum.GetName(typeof(MapLocation), Destination)}" +
		$"{string.Join("", Enumerable.Repeat(" ", Math.Max(1, 30 - Enum.GetName(typeof(MapLocation), Destination).Length)).ToList())}";
		public bool OwnsPalette => !LocationHelper.UnpalettableLocations.Contains(Destination);
		public TeleportDestination(MapLocation destination, MapIndex index, Coordinate coordinates, IEnumerable<TeleportIndex> teleports = null, ExitTeleportIndex exits = ExitTeleportIndex.None)
		{
		    // destination is a logical map area, this is
		    // distinct from mapindex because some dungeons
		    // (marsh, volcano, sea) have multiple distinct
		    // areas on the same map

		    // mapindex is the actual map that gets loaded

		    // coordinates is where the player appears

		    // teleports is a list of _other_ teleports in this
		    // map location (essentially, other places the
		    // player can access once they've arrived here)

		    // exit teleports are special because they take
		    // you to a particular location on the overworld
		    // map (mainly used for titan's tunnel)

			Destination = destination;
			Index = index;
			Coordinates = coordinates;
			CoordinateX = coordinates.X;
			CoordinateY = coordinates.Y;
			Teleports = teleports?.ToList() ?? new List<TeleportIndex>();
			Exit = exits;
		}
		public TeleportDestination(MapLocation destination, MapIndex index, Coordinate coordinates, TeleportIndex teleport)
			: this(destination, index, coordinates, new List<TeleportIndex> { teleport })
		{
		}
		public TeleportDestination(MapLocation destination, MapIndex index, Coordinate coordinates, ExitTeleportIndex exit)
			: this(destination, index, coordinates, exits: exit)
		{
		}
		public TeleportDestination(TeleData teledata, CoordinateLocale context)
		{
			Destination = MapLocation.None;
			Index = teledata.Map;
			Coordinates = new();
			CoordinateX = 0;
			CoordinateY = 0;
			Teleports = null;
			Exit = new();
			SetTeleporter(new Coordinate(teledata.X, teledata.Y, context));
		}
		public TeleportDestination(MapIndex index, Coordinate newcoord)
		{
			Destination = MapLocation.None;
			Index = index;
			Coordinates = newcoord;
			CoordinateX = Coordinates.X;
			CoordinateY = Coordinates.Y;
			Teleports = null;
			Exit = new();
		}
		public TeleportDestination(TeleportDestination newdest)
		{
			Destination = newdest.Destination;
			Index = newdest.Index;
			Coordinates = new Coordinate(newdest.Coordinates.X, newdest.Coordinates.Y, newdest.Coordinates.Context);
			CoordinateX = Coordinates.X;
			CoordinateY = Coordinates.Y;
			Teleports = newdest.Teleports?.ToList();
			Exit = newdest.Exit;
		}
		public TeleportDestination(TeleportDestination newdest, Coordinate newcoords)
		{
			Destination = newdest.Destination;
			Index = newdest.Index;
			Coordinates = new Coordinate(newcoords.X, newcoords.Y, newcoords.Context);
			CoordinateX = Coordinates.X;
			CoordinateY = Coordinates.Y;
			Teleports = newdest.Teleports?.ToList();
			Exit = newdest.Exit;
		}
		public void SetEntrance(Coordinate coordinate)
		{
			Coordinates = coordinate;
		}
		public void SetTeleporter(Coordinate coordinate)
		{
			int mask = (coordinate.Context == CoordinateLocale.Overworld) ? 0xFF : 0x3F;
			Coordinates = new Coordinate((byte)(coordinate.X & mask), (byte)(coordinate.Y & mask), coordinate.Context);
			CoordinateX = Coordinates.X;
			CoordinateY = Coordinates.Y;
		}
		public void SetTeleporter(MapIndex index, Coordinate coordinate)
		{
			SetTeleporter(coordinate);
			Index = index;
		}
		public void SetTeleporter(TeleData data)
		{
			SetTeleporter(new Coordinate(data.X, data.Y, CoordinateLocale.Overworld));
		}
		public Coordinate FlipXcoordinate()
		{
			return new Coordinate((byte)(64 - Coordinates.X - 1), Coordinates.Y, Coordinates.Context);
		}

		public Coordinate FlipYcoordinate()
		{
			return new Coordinate(Coordinates.X, (byte)(64 - Coordinates.Y - 1), Coordinates.Context);
		}
	}
}
