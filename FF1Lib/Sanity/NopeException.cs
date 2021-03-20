using System;
using System.Runtime.Serialization;

namespace FF1Lib.Sanity
{
	[Serializable]
	public class NopeException : Exception
	{
		public MapId mapId { get; private set; }

		public NopeException()
		{
		}

		public NopeException(MapId _mapId, string message) : base(message)
		{
			mapId = _mapId;
		}
	}
}
