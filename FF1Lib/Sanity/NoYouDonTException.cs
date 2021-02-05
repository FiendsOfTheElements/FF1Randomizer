using System;
using System.Runtime.Serialization;

namespace FF1Lib.Sanity
{
	[Serializable]
	internal class NoYouDonTException : Exception
	{
		private MapId mapId;

		public NoYouDonTException()
		{
		}

		public NoYouDonTException(MapId mapid, string message) : base(message)
		{
			this.mapId = mapid;
		}

		public NoYouDonTException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected NoYouDonTException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
