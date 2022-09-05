using System;
using System.Runtime.Serialization;

namespace FF1Lib.Sanity
{
	[Serializable]
	internal class NoYouDon_LowerCaseT_Exception : Exception
	{
		private MapId mapId;

		public NoYouDon_LowerCaseT_Exception()
		{
		}

		public NoYouDon_LowerCaseT_Exception(MapId mapid, string message) : base(message)
		{
			this.mapId = mapid;
		}

		public NoYouDon_LowerCaseT_Exception(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected NoYouDon_LowerCaseT_Exception(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
