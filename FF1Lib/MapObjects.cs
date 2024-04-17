using FF1Lib.Sanity;

namespace FF1Lib
{
	public class MapObject
	{
		public ObjectId ObjectId;
		public int Index;
		public SCCoords Coords;
		public bool InRoom;
		public bool Stationary;

		public MapObject(int _index, byte[] _data)
		{
			ObjectId = (ObjectId)_data[0];
			Index = _index;
			InRoom = (_data[1] & 0x80) > 0;
			Stationary = (_data[1] & 0x40) > 0;
			Coords = new SCCoords(_data[1], _data[2]).SmClamp;
		}
		public MapObject(int _index, ObjectId mapObjId, int x, int y, bool inRoom, bool stationary)
		{
			ObjectId = mapObjId;
			Index = _index;
			InRoom = inRoom;
			Stationary = stationary;
			Coords = new SCCoords(x, y).SmClamp;
		}
		// This is bad, but it's probably needed to keep compatibility with map import, we'll see if we can remove NPC struct easily
		public void CopyFrom(NPC newobject)
		{
			ObjectId = newobject.ObjectId;
			Index = newobject.Index;
			InRoom = newobject.InRoom;
			Stationary = newobject.Stationary;
			Coords = new SCCoords(newobject.Coord.x, newobject.Coord.y).SmClamp;
		}
		public void Flip(bool flipx, bool flipy)
		{
			var oldcoords = Coords;
			Coords = new SCCoords(flipx ? (64 - oldcoords.X - 1) : oldcoords.X, flipy ? (64 - oldcoords.Y - 1) : oldcoords.Y).SmClamp;
		}
		public void Offset(int xoffset, int yoffset)
		{
			var oldcoords = Coords;
			Coords = new SCCoords(oldcoords.X + xoffset, oldcoords.Y + yoffset).SmClamp;
		}
		public byte[] GetBytes()
		{
			return new byte[] { (byte)ObjectId, (byte)(Coords.X | (InRoom ? 0x80 : 0x00) | (Stationary ? 0x40 : 0x00)), Coords.Y };
		}
		//public FF1Rom.generalNPC General;
	}
	public class MapObjects
	{
		private const int MapObjectsOffset = 0x03400;
		private const int MapObjectsSize = 3;
		private const int MapObjectsCount = 16;

		private MapIndex mapIndex;
		private List<MapObject> mapObjects;
		public MapObjects(FF1Rom rom, MapIndex map)
		{
			mapIndex = map;
			mapObjects = rom.Get(MapObjectsOffset + ((int)mapIndex * MapObjectsCount * MapObjectsSize), MapObjectsCount * MapObjectsSize).Chunk(MapObjectsSize).Select((o, i) => new MapObject(i, o)).ToList();
		}
		public void SetNpc(int mapNpcIndex, ObjectId mapObjId, int x, int y, bool inRoom, bool stationary)
		{
			mapObjects[mapNpcIndex] = new(mapNpcIndex, mapObjId, x, y, inRoom, stationary);
		}
		public void SetNpc(int mapNpcIndex, MapObject newmapobject)
		{
			mapObjects[mapNpcIndex] = newmapobject;
		}
		public void MoveNpc(int mapNpcIndex, int x, int y, bool inRoom, bool stationary)
		{
			mapObjects[mapNpcIndex].InRoom = inRoom;
			mapObjects[mapNpcIndex].Stationary = stationary;
			mapObjects[mapNpcIndex].Coords = new SCCoords(x, y).SmClamp;
		}
		public MapObject this[int index]
		{
			get => mapObjects[index];
		}
		/*public IEnumerable<MapObject> GetEnumerator()
		{
			foreach (MapObject mapobject in mapObjects)
			{
				yield return mapobject;
			}
		}*/
		public List<MapObject> ToList()
		{
			return mapObjects;
		}
		public IEnumerator<MapObject> GetEnumerator()
		{
			return mapObjects.GetEnumerator();
		}
		public MapObject FindNpc(ObjectId mapObjId)
		{
			var validobjects = mapObjects.Where(o => o.ObjectId == mapObjId);

			if (validobjects.Any())
			{
				return validobjects.First();
			}
			else
			{
				return null;
			}
		}
		public void Write(FF1Rom rom)
		{
			rom.Put(MapObjectsOffset + ((int)mapIndex * MapObjectsCount * MapObjectsSize), mapObjects.SelectMany(o => o.GetBytes()).ToArray());
		}
	}

}
