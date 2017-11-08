using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class Flags
	{
		public bool Treasures { get; set; }
		public bool IncentivizeIceCave { get; set; }
		public bool IncentivizeOrdeals { get; set; }
		public bool Shops { get; set; }
		public bool MagicShops { get; set; }
		public bool MagicLevels { get; set; }
		public bool MagicPermissions { get; set; }
		public bool Rng { get; set; }
		public bool EnemyScripts { get; set; }
		public bool EnemySkillsSpells { get; set; }
		public bool EnemyStatusAttacks { get; set; }
		public bool Ordeals { get; set; }

		public bool EarlyRod { get; set; }
		public bool EarlyCanoe { get; set; }
		public bool EarlyOrdeals { get; set; }
		public bool NoPartyShuffle { get; set; }
		public bool SpeedHacks { get; set; }
		public bool IdentifyTreasures { get; set; }
		public bool Dash { get; set; }
		public bool BuyTen { get; set; }

		public bool HouseMPRestoration { get; set; }
		public bool WeaponStats { get; set; }
		public bool ChanceToRun { get; set; }
		public bool SpellBugs { get; set; }
		public bool EnemyStatusAttackBug { get; set; }

		public bool FunEnemyNames { get; set; }
		public bool PaletteSwap { get; set; }
		public bool TeamSteak { get; set; }

		public double EnemyScaleFactor { get; set; }
		public double PriceScaleFactor { get; set; }
		public double ExpMultiplier { get; set; }
		public double ExpBonus { get; set; }
	}
}
