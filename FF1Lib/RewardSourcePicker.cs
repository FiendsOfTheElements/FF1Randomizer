using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class RewardSourcePicker
	{
		Dictionary<int, double> weights = new Dictionary<int, double>();

		double reduction;
		double nonchest;

		ISanityChecker checker;

		public RewardSourcePicker(double _reduction, double _nonchest, ISanityChecker _checker)
		{
			reduction = _reduction;
			nonchest = _nonchest;
			checker = _checker;
		}

		public IRewardSource Pick(List<IRewardSource> sources, bool forward, bool spread, MT19337 rng)
		{
			if (!sources.Any()) return null;

			IRewardSource result = null;

			double sum = 0.0;
			foreach (var s in sources)
			{
				if (weights.TryGetValue(s.Address, out var v))
				{
					sum += v;
				}
				else if(s is TreasureChest)
				{
					sum += 1.0;
					weights.Add(s.Address, 1.0);
				}
				else
				{
					sum += nonchest;
					weights.Add(s.Address, nonchest);
				}
			}

			var r = rng.Next() / (double)uint.MaxValue * sum;

			sum = 0.0;
			foreach (var s in sources)
			{
				var v = weights[s.Address];
				if (forward) weights[s.Address] = v * reduction;

				sum += v;
				if (result == null && r <= sum) result = s;
			}

			if (result == null) result = sources.PickRandom(rng);

			if (spread)
			{
				foreach (var s in checker.GetNearRewardSources(sources, result))
				{
					if (weights.TryGetValue(s.Address, out var v))
					{
						weights[s.Address] = v * reduction * reduction;
					}
				}
			}

			return result;
		}
	}
}
