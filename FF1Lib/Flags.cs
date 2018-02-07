namespace FF1Lib
{
	public interface ITreasureShuffleFlags
	{
		bool IncentivizeOrdeals { get; }
		bool IncentivizeIceCave { get; }
		bool IncentivizeVolcano { get; }
		bool IncentivizeConeria { get; }
		bool IncentivizeMarsh { get; }
		bool IncentivizeEarth { get; }
		bool IncentivizeSeaShrine { get; }
		bool EarlyRod { get; }
		bool NPCItems { get; }
		bool EarlyCanoe { get; }
		bool EarlyOrdeals { get; }
		bool EarlyBridge { get; }
		bool TitansTrove { get; }
		bool AllowIceShip { get; }
	}
	public class Flags : ITreasureShuffleFlags
	{
		public bool Treasures { get; set; }
		public bool NPCItems { get; set; }
		public bool IncentivizeMarsh { get; set; }
		public bool IncentivizeVolcano { get; set; }
		public bool IncentivizeConeria { get; set; }
		public bool IncentivizeEarth { get; set; }
		public bool IncentivizeIceCave { get; set; }
		public bool IncentivizeOrdeals { get; set; }
		public bool IncentivizeSeaShrine { get; set; }
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
		public bool EarlyBridge { get; set; }
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
		public bool ModernBattlefield { get; set; }
		public MusicShuffle Music { get; set; }

		public int ForcedPartyMembers { get; set; }
		public double EnemyScaleFactor { get; set; }
		public double PriceScaleFactor { get; set; }
		public double ExpMultiplier { get; set; }
		public int ExpBonus { get; set; }

		public bool MapConeriaDwarves { get; set; }
		public bool MapVolcanoIceRiver { get; set; }
		public bool AllowIceShip => false;
		public bool TitansTrove => false;
	}
}
