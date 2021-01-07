using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class GeneratedHint
	{
		public virtual HintCategoryCoverage Coverage { get; set; }

		public virtual HintPlacementStrategy PlacementStrategy { get; set; }

		public virtual ObjectId FixedNpc { get; set; }

		public virtual MapLocation MapLocation { get; set; }

		public virtual string Text { get; set; }	
	}
}
