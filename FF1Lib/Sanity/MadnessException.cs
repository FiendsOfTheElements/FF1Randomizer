using System;
using System.Runtime.Serialization;

namespace FF1Lib.Sanity
{
	[Serializable]
	internal class MadnessException : Exception
	{
		public MadnessException()
		{
		}

		public MadnessException(string message) : base(message)
		{
		}

		public MadnessException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected MadnessException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}