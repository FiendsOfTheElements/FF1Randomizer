using System.Runtime.Serialization;

namespace FF1Lib.Sanity
{
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

	}
}
