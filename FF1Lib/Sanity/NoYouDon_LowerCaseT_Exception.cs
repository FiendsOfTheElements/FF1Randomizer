using System.Runtime.Serialization;

namespace FF1Lib.Sanity
{
	[Serializable]
	internal class NoYouDon_LowerCaseT_Exception : Exception
	{
		private MapIndex MapIndex;

		public NoYouDon_LowerCaseT_Exception()
		{
		}

		public NoYouDon_LowerCaseT_Exception(MapIndex MapIndex, string message) : base(message)
		{
			this.MapIndex = MapIndex;
		}

		public NoYouDon_LowerCaseT_Exception(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected NoYouDon_LowerCaseT_Exception(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
