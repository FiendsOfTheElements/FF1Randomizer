using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.Sanity
{
	public class SCEntrance
	{
		public SCMap Map { get; private set; }

		public SCTile[,] Tiles { get; private set; }

		public SCCoords Coords { get; private set; }

		public List<SCPointOfInterest> PointsOfInterest { get; set; }

		Queue<SCTileQueueEntry> todoset = new Queue<SCTileQueueEntry>(256);

		public SCEntrance(SCMap _map, SCCoords _coords)
		{
			Map = _map;
			Coords = _coords;

			InstatiateMapData();
			DoPathing();
			ProcessPointsOfInterest();
		}

		private void InstatiateMapData()
		{
			Tiles = new SCTile[Map.Tiles.GetLength(0), Map.Tiles.GetLength(1)];
			Array.Copy(Map.Tiles, Tiles, Map.Tiles.Length);

			PointsOfInterest = Map.PointsOfInterest.Select(p => p.Clone()).ToList();
		}

		public override string ToString()
		{
			return Coords.ToString();
		}

		private void DoPathing()
		{
			CheckTile(Coords.SmLeft, SCBitFlags.Done);
			CheckTile(Coords.SmRight, SCBitFlags.Done);
			CheckTile(Coords.SmUp, SCBitFlags.Done);
			CheckTile(Coords.SmDown, SCBitFlags.Done);

			while (todoset.Count > 0)
			{
				var nextTile = todoset.Dequeue();

				CheckTile(nextTile.Coords.SmLeft, nextTile.BitFlags);
				CheckTile(nextTile.Coords.SmRight, nextTile.BitFlags);
				CheckTile(nextTile.Coords.SmUp, nextTile.BitFlags);
				CheckTile(nextTile.Coords.SmDown, nextTile.BitFlags);
			}
		}

		private void CheckTile(SCCoords coords, SCBitFlags requirements)
		{
			var tile = Tiles[coords.X, coords.Y];

			requirements |= tile.Tile;

			if (!tile.Tile.IsBlocked() && Tiles[coords.X, coords.Y].MergeFlag(requirements) && !tile.Tile.IsImpassable())
			{
				todoset.Enqueue(new SCTileQueueEntry { BitFlags = requirements, Coords = coords });
			}			
		}

		private void ProcessPointsOfInterest()
		{
			foreach (var poi in PointsOfInterest)
			{
				if (poi.Type == SCPointOfInterestType.QuestNpc || poi.Type == SCPointOfInterestType.Treasure)
				{
					SetPoiAccessFromNeighbors(poi);
				}
				else
				{
					SetPoiAccess(poi);
				}
			}
		}

		private void SetPoiAccessFromNeighbors(SCPointOfInterest poi)
		{
			poi.BitFlagSet = GetPassableBitFlagSetFromTile(poi.Coords.SmLeft);
			poi.BitFlagSet.Merge(GetPassableBitFlagSetFromTile(poi.Coords.SmRight));
			poi.BitFlagSet.Merge(GetPassableBitFlagSetFromTile(poi.Coords.SmUp));
			poi.BitFlagSet.Merge(GetPassableBitFlagSetFromTile(poi.Coords.SmDown));
		}

		private SCBitFlagSet GetPassableBitFlagSetFromTile(SCCoords coords)
		{
			return Tiles[coords.X, coords.Y].GetPassableBitFlagSet();
		}

		private void SetPoiAccess(SCPointOfInterest poi)
		{
			poi.BitFlagSet = GetBitFlagSetFromTile(poi.Coords);
		}

		private SCBitFlagSet GetBitFlagSetFromTile(SCCoords coords)
		{
			return Tiles[coords.X, coords.Y].GetBitFlagSet();
		}
	}
}
