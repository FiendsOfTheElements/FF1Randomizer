namespace FF1Lib
{
	public class EnemizerVariant
	{
		public string Name { get; set; }
		public byte MonsterType { get; set; }
		public byte ElemResist { get; set; }
		public byte ElemWeakness { get; set; }
		public (int min, int max)? CritRateRange { get; set; }
		public bool IsLargeElemental { get; set; }
		public bool IsSmallElemental { get; set; }
		public bool IsDragon { get; set; }
		public bool SpecialAbsorbChance { get; set; }
		public bool SpecialGolemLogic { get; set; }
		public bool SpecialFlanLogic { get; set; }

		public EnemizerVariant(string name, byte monsterType, byte elemResist, byte elemWeakness)
		{
			Name = name;
			MonsterType = monsterType;
			ElemResist = elemResist;
			ElemWeakness = elemWeakness;
		}
	}
}
