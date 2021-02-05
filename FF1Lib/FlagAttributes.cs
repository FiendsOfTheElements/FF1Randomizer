using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	[AttributeUsage(AttributeTargets.Property)]
	public class IntegerFlagAttribute : Attribute
	{
		public int Min { get; set; }
		public int Max { get; set; }
		public int Step { get; set; }

		public IntegerFlagAttribute(int min, int max, int step = 1)
		{
			Min = min;
			Max = max;
			Step = step;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class DoubleFlagAttribute : Attribute
	{
		public double Min { get; set; }
		public double Max { get; set; }
		public double Step { get; set; }

		public DoubleFlagAttribute(double min, double max, double step = 1)
		{
			Min = min;
			Max = max;
			Step = step;
		}
	}
}
