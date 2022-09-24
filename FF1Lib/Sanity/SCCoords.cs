using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Direction = FF1Lib.Direction;
using Quadrants = FF1Lib.Procgen.MapState.Quadrants;

namespace FF1Lib.Sanity
{
	public struct SCCoords
	{
		public byte X { get; set; }

		public byte Y { get; set; }

		public SCCoords(int x, int y)
		{
			X = (byte)x;
			Y = (byte)y;
		}

		public override string ToString()
		{
			return "(" + X.ToString() + ", " + Y.ToString() + ")";
		}

	    [JsonIgnore()]
		public SCCoords SmLeft => new SCCoords((0x3F + X) & 0x3F, Y);

	    [JsonIgnore()]
		public SCCoords SmRight => new SCCoords((X + 1) & 0x3F, Y);

	    [JsonIgnore()]
		public SCCoords SmUp => new SCCoords(X, (0x3F + Y) & 0x3F);

	    [JsonIgnore()]
		public SCCoords SmDown => new SCCoords(X, (Y + 1) & 0x3F);

	    [JsonIgnore()]
		public SCCoords OwLeft => new SCCoords((0xFF + X) & 0xFF, Y);

	    [JsonIgnore()]
		public SCCoords OwRight => new SCCoords((X + 1) & 0xFF, Y);

	    [JsonIgnore()]
		public SCCoords OwUp => new SCCoords(X, (0xFF + Y) & 0xFF);

	    [JsonIgnore()]
		public SCCoords OwDown => new SCCoords(X, (Y + 1) & 0xFF);

	    [JsonIgnore()]
		public SCCoords SmClamp => new SCCoords(X & 0x3F, Y & 0x3F);

	    public SCCoords SmNeighbor(Direction d) {
		switch (d) {
		    case Direction.Up:
			return SmUp;
		    case Direction.Right:
			return SmRight;
		    case Direction.Down:
			return SmDown;
		    case Direction.Left:
			return SmLeft;
		    default:
			return this;
		}
	    }

	    public SCCoords SmNeighbor(Quadrants q) {
		switch (q) {
		    case Quadrants.UpRight:
			return SmUp.SmRight;
		    case Quadrants.DownRight:
			return SmDown.SmRight;
		    case Quadrants.DownLeft:
			return SmDown.SmLeft;
		    case Quadrants.UpLeft:
			return SmUp.SmLeft;
		    default:
			return this;
		}
	    }

		public static bool operator ==(SCCoords a, SCCoords b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator !=(SCCoords a, SCCoords b)
		{
			return a.X != b.X || a.Y != b.Y;
		}

		static SCCoordsEqualityComparer cec = new SCCoordsEqualityComparer();

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

	    public double Dist(SCCoords d) {
		return Math.Sqrt((d.X-this.X)*(d.X-this.X) + (d.Y-this.Y)*(d.Y-this.Y));
	    }
	}

	public class SCCoordsEqualityComparer : IEqualityComparer<SCCoords>
	{
		public bool Equals(SCCoords x, SCCoords y)
		{
			return x.X == y.X && x.Y == y.Y;
		}

		public int GetHashCode([DisallowNull] SCCoords obj)
		{
			return obj.X & 0x0F + (obj.Y & 0x0f) * 16 + (obj.X & 0xF0) * 256 + (obj.Y & 0xF0) * 4096;
		}
	}
}
