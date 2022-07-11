namespace FF1Lib
{
	public enum TeleportType
	{
		Enter,
		Exit,
		Tele
	}

	public class TeleportFixup
	{
	    public TeleportFixup() { }
	    public TeleportFixup(TeleportType tp, int idx, TeleData to) {
		this.Type = tp;
		this.Index = idx;
		this.To = to;
	    }

		public TeleportType Type { get; set; }

		public int? Index { get; set; }

		public TeleData? From { get; set; }

		public TeleData To { get; set; }

	}
}
