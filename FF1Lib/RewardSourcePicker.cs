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

			//This loop sums up all weights of the reward source.
			double sum = 0.0;
			foreach (var s in sources)
			{
				//If weight already exists for rewardsource use it
				if (weights.TryGetValue(s.Address, out var v))
				{
					sum += v;
				}
				//If it doesn't have a weight and it's a chest, give it the weight 1
				else if(s is TreasureChest)
				{
					sum += 1.0;
					weights.Add(s.Address, 1.0);
				}
				//If it's not a chest, give it a significantly higher weight. There are way more chests than nonchet rewards.
				//That way there is a reasonable chance, that some loose key items land on npcs.
				else
				{
					sum += nonchest;
					weights.Add(s.Address, nonchest);
				}
			}

			//We pick a value between 0 and sum. We can't work with the full double range, but with reasonable weight reduction, it's not going to be a problem.
			var r = rng.Next() / (double)uint.MaxValue * sum;

			//This loop finds the reward source associated with the rng number.
			//If Forward placement is active, reduce the weight of all reward sources that were elligible for an item this step.
			//Normally chest like ToF or Matoya have a chance to roll a loose item at every step, because they are accessible from the start.
			//TFC is accessible way later so is used in way less rolls.
			//By reducing the weight on chests, that already had a chance, it's more likely to select a chest or npc from a newly opened up area(hence forward placement).
			//The forward placement is deactivated for incentive items, so it doesn't skew the placement.
			sum = 0.0;
			foreach (var s in sources)
			{
				var v = weights[s.Address];
				if (forward) weights[s.Address] = v * reduction;

				sum += v;
				if (result == null && r <= sum) result = s;
			}

			//If we fail to find a reward source because of numerical problems, just pick a random one. It hasn't happend so far, but just to be sure.
			if (result == null) result = sources.PickRandom(rng);

			//The V2 builds a list of chests for each overworld entrance. GetNearRewardSources selects chests accessible from the same entrance with the same requirements.
			//So something like all accessible chests in marsh, or all keylocked chests in marsh. The function works for F/E. Otherwise V2 wouldn't work for F/E.
			//All chests found are demoted heavily. So the placement algorithm will only place another item there if it "has" to.
			//That spreads the key items out into different places(hence spread placement).
			//The spread placement remains active for incentive items. There is only one incentive location per dungeon and access requirement.
			//This has the effect of reducing the chance of a loose item in incentivized dungeons.
			//There is a small chance of two incentive locations in one dungeon with F/E. But the effect will not be noticable by any unaware observer.
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
