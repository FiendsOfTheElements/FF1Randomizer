namespace FF1Lib.Procgen
{
	class WaterfallEngine : IMapGeneratorEngine
	{
		public CompleteMap Generate(MT19337 rng, MapRequirements reqs)
		{
			//Constants! Get yer constants here!
			int iteration_count = 15;

			//((List<RoomSpec>)reqs.Rooms)[0].Tiledata = ((List<RoomSpec>)reqs.Rooms)[0].Tiledata;

			CompleteMap complete = new CompleteMap
			{
				Map = new Map((byte)Tile.WaterfallInside)
			};
			//(57,56)
			var startLoc = (x: 0x39, y: 0x38);

			var startingX = rng.Between(-3, 0) + startLoc.x;
			var startingY = rng.Between(-4, -1) + startLoc.y;

			var startRegion = new Region(startingX, startingY, 4, 5, Tile.WaterfallRandomEncounters);

			List<Region> regionList = new List<Region>();
			regionList.Add(startRegion);

			List<Region> endingRegions = new List<Region>();
			for (var i = 0; i < iteration_count; i++)
			{
				var startPoint = regionList[rng.Between(0, regionList.Count - 1)];
				var newRegions = RegionChain(rng, startPoint, regionList, 30 - i);
				regionList.AddRange(newRegions);
				if (newRegions.Count > 0)
				{
					endingRegions.Add(newRegions[newRegions.Count - 1]);
				}
			}

			var foundRoomPlace = false;
			var room = ((List<RoomSpec>)reqs.Rooms)[0];
			var room_x = -1;
			var room_y = -1;

			while (!foundRoomPlace && endingRegions.Count > 0)
			{
				var borderRegion = endingRegions.PickRandom(rng);

				var base_x = (borderRegion.x - (room.Width - 1) + 64) % 64;
				room_y = (borderRegion.y - room.Height + 64) % 64;
				var x_offset = 1;
				List<int> valid_offsets = new List<int>();

				while (x_offset < room.Width)
				{
					room_x = (base_x + x_offset) % 64;
					var validRoomPlace = true;
					foreach (Region r in regionList)
					{
						var testVal = validRoomPlace && !r.IntersectsRoom(room, room_x, room_y);
						if (!testVal && validRoomPlace)
						{
							//Console.WriteLine(room_x);
						}
						validRoomPlace = testVal;
					}
					if (validRoomPlace)
					{
						valid_offsets.Add(x_offset);
					}
					x_offset++;
				}

				if (valid_offsets.Count != 0)
				{
					foundRoomPlace = true;
					room_x = (base_x + valid_offsets[rng.Between(0, valid_offsets.Count - 1)]);
				}

				endingRegions.Remove(borderRegion);
			}

			if (!foundRoomPlace)
			{
				List<int> idxs = Enumerable.Range(0, regionList.Count).ToList();
				while (!foundRoomPlace && idxs.Count > 0)
				{
					int regionIdx = idxs.PickRandom(rng);

					var borderRegion = regionList[regionIdx];
					var base_x = (borderRegion.x - (room.Width - 1) + 64) % 64;
					room_y = (borderRegion.y - (room.Height - 1) + 64) % 64;
					var x_offset = 1;
					List<int> valid_offsets = new List<int>();

					while (x_offset < room.Width)
					{
						room_x = (base_x + x_offset) % 64;
						var validRoomPlace = true;
						foreach (Region r in regionList)
						{
							var testVal = validRoomPlace && !r.IntersectsRoom(room, room_x, room_y);
							if (!testVal && validRoomPlace)
							{
								//Console.WriteLine(room_x);
							}
							validRoomPlace = testVal;
						}
						if (validRoomPlace)
						{
							valid_offsets.Add(x_offset);
						}
						x_offset++;
					}

					if (valid_offsets.Count != 0)
					{
						foundRoomPlace = true;
						room_x = (base_x + valid_offsets[rng.Between(0, valid_offsets.Count - 1)]);
					}

					idxs.Remove(regionIdx);
				}
			}

			if (!foundRoomPlace)
			{
				Console.WriteLine("No room found :o");
				return null;
			}

			//Draw every room in regionList to complete
			foreach (Region r in regionList)
			{
				r.DrawRegion(complete);
			}

			Region waterfallRoom = new Region(room_x, room_y, room);
			waterfallRoom.DrawRegion(complete);


			int doorYPos = (room_y + room.Height) % 64;

			List<int> possibleDoors = new List<int>();
			for (var i = 0; i < room.Width; i++)
			{
				if (complete.Map[((room_x + i) % 64, doorYPos)].Tile == Tile.WaterfallRandomEncounters)
				{
					if (!(i == 0 || i == 7))
					{
						possibleDoors.Add(i);
					}
				}
				if (complete.Map[((room_x + i) % 64, doorYPos)].Tile == Tile.WaterfallInside)
				{
					if (complete.Map[((room_x + i + 63) % 64, doorYPos)].Tile == Tile.WaterfallRandomEncounters)
					{
						complete.Map[((room_x + i) % 64, doorYPos)].Tile = Tile.RoomLeft;
					}
					else if (complete.Map[((room_x + i + 1) % 64, doorYPos)].Tile == Tile.WaterfallRandomEncounters)
					{
						complete.Map[((room_x + i) % 64, doorYPos)].Tile = Tile.RoomRight;
					}
				}
			}

			if (possibleDoors.Count == 0)
			{
				return null;
			}

			//Then we do the cleanup
			complete = CleanUp(complete);
			if (complete == null) return null;

			//Now, place the door (so it doesn't get in the way of the cleanup)
			int doorPos = (possibleDoors[rng.Between(0, possibleDoors.Count - 1)] + room_x)%64;

			complete.Map[(doorPos, doorYPos)].Tile = Tile.Door;
			complete.Map[(doorPos, doorYPos)].Neighbor(Direction.Down).Tile = Tile.Doorway;
			complete.Map[(doorPos, doorYPos)].Neighbor(Direction.Up).Tile = Tile.WaterfallSpikeTile;

			//NPC management
			reqs.Rooms.First().NPCs.ToList().ForEach(npc =>
			{
				npc.Coord.x = (npc.Coord.x + room_x)%64;
				npc.Coord.y = (npc.Coord.y + room_y) % 64;
				reqs.Rom.SetNpc(reqs.MapIndex, npc);
			});


			//Stuff to do at the end~
			complete.Map[(startLoc.x, startLoc.y)].Tile = Tile.WarpUp;
			complete.Entrance = new Coordinate((byte)startLoc.x, (byte)startLoc.y, CoordinateLocale.Standard);

			return complete;
		}

		public CompleteMap OneGapToWalkable(CompleteMap complete)
		{
			//We're gonna deal with the 1x1 gaps by just filling them in
			//We thought about rocks. But now just making bigger spaces feels... simpler
			foreach (MapElement cell in complete.Map)
			{
				if (cell.Tile == Tile.WaterfallInside && cell.Neighbor(Direction.Up).Tile == Tile.WaterfallRandomEncounters && cell.Neighbor(Direction.Down).Tile == Tile.WaterfallRandomEncounters)
				{
					cell.Tile = Tile.WaterfallRandomEncounters;
				}
				if (cell.Tile == Tile.WaterfallInside && cell.Neighbor(Direction.Left).Tile == Tile.WaterfallRandomEncounters && cell.Neighbor(Direction.Right).Tile == Tile.WaterfallRandomEncounters)
				{
					cell.Tile = Tile.WaterfallRandomEncounters;
				}
			}
			//Two passes to catch literal corner cases.
			foreach (MapElement cell in complete.Map)
			{
				if (cell.Tile == Tile.WaterfallInside && cell.Neighbor(Direction.Up).Tile == Tile.WaterfallRandomEncounters && cell.Neighbor(Direction.Down).Tile == Tile.WaterfallRandomEncounters)
				{
					cell.Tile = Tile.WaterfallRandomEncounters;
				}
				if (cell.Tile == Tile.WaterfallInside && cell.Neighbor(Direction.Left).Tile == Tile.WaterfallRandomEncounters && cell.Neighbor(Direction.Right).Tile == Tile.WaterfallRandomEncounters)
				{
					cell.Tile = Tile.WaterfallRandomEncounters;
				}
			}

			return complete;
		}

		public CompleteMap CleanUp(CompleteMap complete)
		{
			complete = OneGapToWalkable(complete);

			//Okay, time to do something hacky to handle the one-off-wall nonsense
			foreach (MapElement cell in complete.Map)
			{
				if (cell.Tile == Tile.WaterfallRandomEncounters && cell.Neighbor(Direction.Up).Tile == Tile.WaterfallInside && cell.Neighbor(Direction.Down).Tile == Tile.WaterfallInside)
				{
					MapElement updated = cell.Neighbor(Direction.Up);
					updated.Tile = Tile.WaterfallRandomEncounters;
					if (updated.Neighbor(Direction.Right).Tile == Tile.WaterfallInside)
					{
						updated.Neighbor(Direction.Right).Tile = Tile.WaterfallRandomEncounters;
					}
					else
					{
						if (updated.Neighbor(Direction.Left).Tile != Tile.WaterfallInside && updated.Neighbor(Direction.Left).Tile != Tile.WaterfallRandomEncounters) return null;
						updated.Neighbor(Direction.Left).Tile = Tile.WaterfallRandomEncounters;
					}
				}
			}

			Dictionary<(int x, int y), Tile> replaceDict = new Dictionary<(int x, int y), Tile>();

			// Now, add the top wall bits

			foreach (MapElement cell in complete.Map)
			{
				//if (cell.Tile == Tile.Impassable || cell.Tile == Tile.WaterfallRandomEncounters)
				if (cell.Tile == Tile.WaterfallRandomEncounters)
				{
					Tile upperCell = cell.Neighbor(Direction.Up).Tile;
					bool replaceCell = (upperCell == Tile.WaterfallInside);
					replaceCell = replaceCell || upperCell == Tile.RoomFrontLeft;
					replaceCell = replaceCell || upperCell == Tile.RoomFrontRight;
					replaceCell = replaceCell || upperCell == Tile.RoomFrontCenter;
					replaceCell = replaceCell || upperCell == Tile.RoomBackLeft;
					replaceCell = replaceCell || upperCell == Tile.RoomBackRight;
					replaceCell = replaceCell || upperCell == Tile.RoomBackCenter;
					if (replaceCell)
					{
						replaceDict[cell.Coord] = Tile.InsideWall;
					}

				}

				//Now, add the 0x01 - 0x08 parts to form the borders
				if (cell.Tile == Tile.WaterfallInside)
				{
					if (cell.Neighbor(Direction.Left).Tile == Tile.WaterfallRandomEncounters)
					{
						cell.Tile = Tile.RoomLeft;
					}
					if (cell.Neighbor(Direction.Right).Tile == Tile.WaterfallRandomEncounters)
					{
						cell.Tile = Tile.RoomRight;
					}
					if (cell.Neighbor(Direction.Up).Tile == Tile.WaterfallRandomEncounters)
					{
						if (cell.Neighbor(Direction.Left).Tile == Tile.WaterfallRandomEncounters)
						{
							cell.Tile = Tile.RoomBackLeft;
						}
						else if (cell.Neighbor(Direction.Right).Tile == Tile.WaterfallRandomEncounters)
						{
							cell.Tile = Tile.RoomBackRight;
						}
						else
						{
							cell.Tile = Tile.RoomBackCenter;
						}
					}
					if (cell.Neighbor(Direction.Down).Tile == Tile.WaterfallRandomEncounters)
					{
						if (cell.Neighbor(Direction.Left).Tile == Tile.WaterfallRandomEncounters)
						{
							cell.Tile = Tile.RoomFrontLeft;
						}
						else if (cell.Neighbor(Direction.Right).Tile == Tile.WaterfallRandomEncounters)
						{
							cell.Tile = Tile.RoomFrontRight;
						}
						else
						{
							cell.Tile = Tile.RoomFrontCenter;
						}
					}

					//Highjacking this to include some room-bottom cleanup; this prevents a runaway robot!
					if (cell.Neighbor(Direction.Up).Tile == Tile.RoomFrontCenter || cell.Neighbor(Direction.Up).Tile == Tile.RoomFrontLeft || cell.Neighbor(Direction.Up).Tile == Tile.RoomFrontRight)
					{
						cell.Tile = Tile.RoomBackCenter;
					}
				}

				if (cell.Tile == Tile.RoomLeft && cell.Neighbor(Direction.Up).Tile == Tile.RoomFrontCenter)
				{
					cell.Tile = Tile.RoomBackLeft;
				}

				if (cell.Tile == Tile.RoomLeft && cell.Neighbor(Direction.Up).Tile == Tile.RoomFrontLeft)
				{
					cell.Tile = Tile.RoomBackLeft;
				}

				if (cell.Tile == Tile.RoomRight && cell.Neighbor(Direction.Up).Tile == Tile.RoomFrontCenter)
				{
					cell.Tile = Tile.RoomBackRight;
				}

				if (cell.Tile == Tile.RoomRight && cell.Neighbor(Direction.Up).Tile == Tile.RoomFrontLeft)
				{
					cell.Tile = Tile.RoomBackRight;
				}
			}

			foreach (KeyValuePair<(int x, int y), Tile> entry in replaceDict)
			{
				complete.Map[(entry.Key.x, entry.Key.y)].Tile = entry.Value;
			}

			//Finally we'll need to to clean up the room sometimes

			/*foreach (MapElement cell in complete.Map)
			{
				if (cell.Tile == Tile.RoomFrontCenter && cell.Neighbor(Direction.Down).Tile == Tile.RoomFrontCenter)
				{
					cell.Tile = Tile.WaterfallInside;
				}
				if (cell.Tile == Tile.RoomBackCenter && cell.Neighbor(Direction.Up).Tile == Tile.RoomBackCenter)
				{
					cell.Tile = Tile.WaterfallInside;
				}
				if (cell.Tile == Tile.RoomLeft && cell.Neighbor(Direction.Left).Tile == Tile.RoomLeft)
				{
					cell.Tile = Tile.WaterfallInside;
				}
				if (cell.Tile == Tile.RoomRight && cell.Neighbor(Direction.Right).Tile == Tile.RoomRight)
				{
					cell.Tile = Tile.WaterfallInside;
				}
			}*/

			return complete;
		}

		public List<Region> RegionChain(MT19337 rng, Region startCell, List<Region> curList, int iterations)
		{
			List<Region> outList = new List<Region>();

			var curCell = startCell;

			for (var i = 0; i < iterations; i++)
			{
				Region nextCell = CreateAdjacent(rng, curCell);
				if (nextCell.GetAdjacents(outList).Count + nextCell.GetAdjacents(curList).Count == 1)
				{
					outList.Add(nextCell);
					curCell = nextCell;
				}
			}

			return outList;
		}

		public Region CreateAdjacent(MT19337 rng, Region cell)
		{
			var dx = rng.Between(-4, 3);
			var dy = rng.Between(-4, 4);
			var manhat = Math.Abs(dx) + Math.Abs(dy);

			while (manhat < 2 || manhat > 7)
			{
				dx = rng.Between(-4, 3);
				dy = rng.Between(-4, 4);
				manhat = Math.Abs(dx) + Math.Abs(dy);
			}

			return new Region((cell.x + dx + 64) % 64, (cell.y + dy + 64) % 64, cell);
		}
	}
}
