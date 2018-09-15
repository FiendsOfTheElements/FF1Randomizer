using System.Collections.Generic;

namespace FF1Lib.Procgen
{
	public struct RoomSpec
	{
		public byte[,] Tiledata;
		public IEnumerable<NPC> NPCs; // NPCs to be forced into the room. (Coord is an offset from Tiledata origin)
	}
}