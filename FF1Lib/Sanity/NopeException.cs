namespace FF1Lib.Sanity
{
	[Serializable]
	public class NopeException : Exception
	{
		public MapIndex MapIndex { get; private set; }

		public NopeException()
		{
		}

		public NopeException(MapIndex _MapIndex, string message) : base(message)
		{
			MapIndex = _MapIndex;
		}
	}
}
