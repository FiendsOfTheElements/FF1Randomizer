﻿using System;
using System.Collections.Generic;
using System.Linq;

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
		public readonly byte X;
		public readonly byte Y;
		public Coordinate(byte x, byte y, CoordinateLocale context)
		{
			X = x;
			Y = y;

			if (context > CoordinateLocale.Overworld)
			{
				Y |= 0x80;

				if (context == CoordinateLocale.StandardInRoom)
				{
					X |= 0x80;
				}
			}

			_identityValue = (short)(x * 256 + y);
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

	public struct TeleportDestination
	{
		public readonly MapLocation Destination;
		public readonly MapIndex Index;
		public byte CoordinateX { get; private set; }
		public byte CoordinateY { get; private set; }
		public readonly IEnumerable<TeleportIndex> Teleports;
		public readonly ExitTeleportIndex Exit;
		public string SpoilerText =>
		$"{Enum.GetName(typeof(MapLocation), Destination)}" +
		$"{string.Join("", Enumerable.Repeat(" ", Math.Max(1, 30 - Enum.GetName(typeof(MapLocation), Destination).Length)).ToList())}";
		public bool OwnsPalette => !LocationHelper.UnpalettableLocations.Contains(Destination);
		public TeleportDestination(MapLocation destination, MapIndex index, Coordinate coordinates, IEnumerable<TeleportIndex> teleports = null, ExitTeleportIndex exits = ExitTeleportIndex.None)
		{
			Destination = destination;
			Index = index;
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
		public void SetEntrance(Coordinate coordinate)
		{
			CoordinateX = coordinate.X;
			CoordinateY = coordinate.Y;
		}

		public void FlipXcoordinate()
		{
			CoordinateX = (byte)(64 - CoordinateX - 1);
		}
	}
}
