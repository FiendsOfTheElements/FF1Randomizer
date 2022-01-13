using FF1Lib.Helpers;
using RomUtilities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace FF1Lib
{
	public enum RibbonMode
	{
		[Description("Vanilla")]
		Vanilla,

		[Description("Shattered")]
		Split,

		[Description("Removed")]
		Removed
	}

	public class RibbonShuffle
	{
		private const int ElementalGroup1Count = 15;
		private const int ElementalGroup2Count = 10;
		private const int ElementalGroup3Count = 5;

		MT19337 rng;
		Flags flags;
		FF1Rom rom;
		ItemNames itemsText;

		HashSet<Item> rareArmors;
		List<SpellElement> allElements;

		public RibbonShuffle(FF1Rom _rom, MT19337 _rng, Flags _flags, ItemNames _itemsText)
		{
			rom = _rom;
			rng = _rng;
			flags = _flags;
			itemsText = _itemsText;

		}

		public void Work()
		{
			if (flags.RibbonMode == RibbonMode.Removed)
			{
				var armors = Armor.LoadAllArmors(rom, flags).ToList();

				var ribbon = armors.FirstOrDefault(a => a.Id == Item.Ribbon);
				ribbon.ElementalResist = 0x00;
				ribbon.writeArmorMemory(rom);

				itemsText[(int)Item.Ribbon] = "Ripoff";
			}
			else if (flags.RibbonMode == RibbonMode.Split)
			{
				var armors = Armor.LoadAllArmors(rom, flags).ToList();
				rareArmors = new HashSet<Item>(ItemLists.RareArmorTier.Concat(ItemLists.LegendaryArmorTier));

				allElements = new List<SpellElement>();

				for (int i = 0; i < ElementalGroup1Count; i++)
				{
					allElements.Add(SpellElement.Fire);
					allElements.Add(SpellElement.Lightning);
					allElements.Add(SpellElement.Ice);
					allElements.Add(SpellElement.Earth);
				}

				for (int i = 0; i < ElementalGroup2Count; i++)
				{
					allElements.Add(SpellElement.Poison);
					allElements.Add(SpellElement.Time);
				}

				for (int i = 0; i < ElementalGroup3Count; i++)
				{
					allElements.Add(SpellElement.Death);
					allElements.Add(SpellElement.Status);
				}

				foreach (var a in armors) a.ElementalResist = 0x00;

				EnsureFullProtectionForAllClasses(armors);

				foreach (var a in armors) a.writeArmorMemory(rom);
			}
		}

		private void EnsureFullProtectionForAllClasses(List<Armor> armors)
		{
			Dictionary<EquipPermission, List<Armor>> armorsPerClass = new Dictionary<EquipPermission, List<Armor>>();

			armorsPerClass.Add(EquipPermission.Knight, new List<Armor>());
			armorsPerClass.Add(EquipPermission.Ninja, new List<Armor>());
			armorsPerClass.Add(EquipPermission.Master, new List<Armor>());
			armorsPerClass.Add(EquipPermission.RedWizard, new List<Armor>());
			armorsPerClass.Add(EquipPermission.WhiteWizard, new List<Armor>());
			armorsPerClass.Add(EquipPermission.BlackWizard, new List<Armor>());

			foreach (var a in armors.Where(a => a.Absorb <= 25 && a.Spell == Spell.None))
			{
				if ((a.ClassUsability & (ushort)EquipPermission.Knight) > 0) armorsPerClass[EquipPermission.Knight].Add(a);
				if ((a.ClassUsability & (ushort)EquipPermission.Ninja) > 0) armorsPerClass[EquipPermission.Ninja].Add(a);
				if ((a.ClassUsability & (ushort)EquipPermission.Master) > 0) armorsPerClass[EquipPermission.Master].Add(a);
				if ((a.ClassUsability & (ushort)EquipPermission.RedWizard) > 0) armorsPerClass[EquipPermission.RedWizard].Add(a);
				if ((a.ClassUsability & (ushort)EquipPermission.WhiteWizard) > 0) armorsPerClass[EquipPermission.WhiteWizard].Add(a);
				if ((a.ClassUsability & (ushort)EquipPermission.BlackWizard) > 0) armorsPerClass[EquipPermission.BlackWizard].Add(a);
			}

			foreach (var cls in armorsPerClass.OrderBy(c => c.Value.Count))
			{
				EnsureFullProtectionForClass(armors, cls);
			}

			PlacePoolElements(armors);
		}

		private void PlacePoolElements(List<Armor> armors)
		{
			List<Armor> rares = armors.Where(a => rareArmors.Contains(a.Id)).ToList();

			GenerateReplacementArmor(ArmorType.ARMOR, 9, rares);
			GenerateReplacementArmor(ArmorType.ARMOR, 9, rares);
			GenerateReplacementArmor(ArmorType.SHIELD, 9, rares);
			GenerateReplacementArmor(ArmorType.HELM, 9, rares);
			GenerateReplacementArmor(ArmorType.GAUNTLET, 9, rares);

			GenerateReplacementArmor(ArmorType.ARMOR, 7, armors);
			GenerateReplacementArmor(ArmorType.ARMOR, 7, armors);
			GenerateReplacementArmor(ArmorType.SHIELD, 7, armors);
			GenerateReplacementArmor(ArmorType.HELM, 7, armors);
			GenerateReplacementArmor(ArmorType.GAUNTLET, 7, armors);

			GenerateReplacementArmor(ArmorType.ARMOR, 7, armors);
			GenerateReplacementArmor(ArmorType.ARMOR, 7, armors);
			GenerateReplacementArmor(ArmorType.SHIELD, 7, armors);
			GenerateReplacementArmor(ArmorType.HELM, 7, armors);
			GenerateReplacementArmor(ArmorType.GAUNTLET, 7, armors);

			GenerateReplacementArmor(ArmorType.ARMOR, 5, armors);
			GenerateReplacementArmor(ArmorType.ARMOR, 5, armors);
			GenerateReplacementArmor(ArmorType.SHIELD, 5, armors);
			GenerateReplacementArmor(ArmorType.SHIELD, 5, armors);
			GenerateReplacementArmor(ArmorType.HELM, 4, armors);
			GenerateReplacementArmor(ArmorType.HELM, 4, armors);
			GenerateReplacementArmor(ArmorType.GAUNTLET, 4, armors);
			GenerateReplacementArmor(ArmorType.GAUNTLET, 4, armors);

			var list = armors.Where(a => a.ElementalResist == 0x00).ToList();

			for (int i = 0; i < 300; i++)
			{
				if (allElements.Count == 0 || list.Count == 0) return;

				var a = list.PickRandom(rng);
				if (GetScore(a.ElementalResist) > 3) AddRandomElement2(a, allElements);
			}
		}

		private void GenerateReplacementArmor(ArmorType type, int score, List<Armor> armors)
		{
			var list = armors.Where(a => a.Type == type && a.ElementalResist == 0x00 && (score <= 5 || a.Absorb <= 25 && a.Spell == Spell.None)).ToList();

			if (list.Count > 0)
			{
				var a = list.PickRandom(rng);
				if (GetScore(a.ElementalResist) < score && allElements.Count > 0) AddRandomElement2(a, allElements);
				if (GetScore(a.ElementalResist) < score && allElements.Count > 0) AddRandomElement2(a, allElements);
				if (GetScore(a.ElementalResist) < score && allElements.Count > 0) AddRandomElement2(a, allElements);
				if (GetScore(a.ElementalResist) < score && allElements.Count > 0) AddRandomElement2(a, allElements);
				if (GetScore(a.ElementalResist) < score && allElements.Count > 0) AddRandomElement2(a, allElements);
			}
		}

		private void EnsureFullProtectionForClass(List<Armor> armors, KeyValuePair<EquipPermission, List<Armor>> cls)
		{
			//No armors equippable
			if (cls.Value.Count == 0) return;

			List<SpellElement> elements = new List<SpellElement> { SpellElement.Fire, SpellElement.Lightning, SpellElement.Ice, SpellElement.Earth, SpellElement.Poison, SpellElement.Time, SpellElement.Death, SpellElement.Status };

			List<Armor> rares = cls.Value.Where(a => rareArmors.Contains(a.Id)).ToList();

			var existingResists = cls.Value.Where(a => a.ElementalResist > 0).ToList();
			(var complete, var a1, var a2) = CheckForCompleteProtection(existingResists);

			if (complete) return;

			//prefer a rare or legendary armor for a1
			if (a1 == null)
			{
				if (rares.Count > 0)
				{
					a1 = rares.PickRandom(rng);
				}
				else
				{
					a1 = cls.Value.PickRandom(rng);
				}
			}

			//second armor of different type
			if (a2 == null)
			{
				var a2rares = cls.Value.Where(a => a.Type != a1.Type && rareArmors.Contains(a.Id)).ToList();
				var a2candidates = cls.Value.Where(a => a.Type != a1.Type).ToList();
				if (a2rares.Count > 0)
				{
					a2 = a2rares.PickRandom(rng);
				}
				else if (a2candidates.Count > 0)
				{
					a2 = a2candidates.PickRandom(rng);
				}
			}

			//third armor of different type
			Armor a3 = null;
			if (a2 != null)
			{
				var a3rares = cls.Value.Where(a => a.Type != a1.Type && a.Type != a2.Type && rareArmors.Contains(a.Id)).ToList();
				var a3candidates = cls.Value.Where(a => a.Type != a1.Type && a.Type != a2.Type).ToList();
				if (a3rares.Count > 0)
				{
					a3 = a3rares.PickRandom(rng);
				}
				else if (a3candidates.Count > 0)
				{
					a3 = a3candidates.PickRandom(rng);
				}
			}

			var resist = a1.ElementalResist | a2.ElementalResist | a3.ElementalResist;

			if ((resist & (byte)SpellElement.Fire) > 0) elements.Remove(SpellElement.Fire);
			if ((resist & (byte)SpellElement.Lightning) > 0) elements.Remove(SpellElement.Lightning);
			if ((resist & (byte)SpellElement.Ice) > 0) elements.Remove(SpellElement.Ice);
			if ((resist & (byte)SpellElement.Earth) > 0) elements.Remove(SpellElement.Earth);
			if ((resist & (byte)SpellElement.Poison) > 0) elements.Remove(SpellElement.Poison);
			if ((resist & (byte)SpellElement.Time) > 0) elements.Remove(SpellElement.Time);
			if ((resist & (byte)SpellElement.Death) > 0) elements.Remove(SpellElement.Death);
			if ((resist & (byte)SpellElement.Status) > 0) elements.Remove(SpellElement.Status);

			if (a1 != null && elements.Contains(SpellElement.Status)) AddElement(a1, SpellElement.Status, elements);

			if (a1 != null && elements.Count > 0 && GetScore(a1.ElementalResist) < 6) AddRandomElement(a1, elements);
			if (a1 != null && elements.Count > 0 && GetScore(a1.ElementalResist) < 6) AddRandomElement(a1, elements);
			if (a1 != null && elements.Count > 0 && GetScore(a1.ElementalResist) < 6) AddRandomElement(a1, elements);

			if (a2 != null && elements.Count > 0 && GetScore(a2.ElementalResist) < 6) AddRandomElement(a2, elements);
			if (a2 != null && elements.Count > 0 && GetScore(a2.ElementalResist) < 6) AddRandomElement(a2, elements);
			if (a2 != null && elements.Count > 0 && GetScore(a2.ElementalResist) < 6) AddRandomElement(a2, elements);

			if (a3 != null && elements.Count > 0) AddRandomElement(a3, elements);
			if (a3 != null && elements.Count > 0) AddRandomElement(a3, elements);
			if (a3 != null && elements.Count > 0) AddRandomElement(a3, elements);
			if (a3 != null && elements.Count > 0) AddRandomElement(a3, elements);

			if (a1 != null && a2 == null && elements.Count > 0)
			{
				if (elements.Contains(SpellElement.Death))
				{
					AddElement(a2, SpellElement.Death, elements);
				}
				else
				{
					AddRandomElement(a2, elements);
				}
			}

			if (a2 != null && a3 == null && elements.Count > 0)
			{
				if (elements.Contains(SpellElement.Death))
				{
					AddElement(a2, SpellElement.Death, elements);
				}
			}
		}

		private void AddElement(Armor a, SpellElement e, List<SpellElement> elements)
		{
			a.ElementalResist |= (byte)e;
			elements.Remove(e);
			allElements.Remove(e);
		}

		private void AddRandomElement2(Armor a, List<SpellElement> elements)
		{
			if ((a.Absorb > 25 || a.Spell != Spell.None) && GetScore(a.ElementalResist) >= 4) return;

			HashSet<SpellElement> elementSet = new HashSet<SpellElement> { SpellElement.Fire, SpellElement.Lightning, SpellElement.Ice, SpellElement.Earth, SpellElement.Poison, SpellElement.Time, SpellElement.Death, SpellElement.Status };

			var resist = a.ElementalResist;

			if ((resist & (byte)SpellElement.Fire) > 0) elementSet.Remove(SpellElement.Fire);
			if ((resist & (byte)SpellElement.Lightning) > 0) elementSet.Remove(SpellElement.Lightning);
			if ((resist & (byte)SpellElement.Ice) > 0) elementSet.Remove(SpellElement.Ice);
			if ((resist & (byte)SpellElement.Earth) > 0) elementSet.Remove(SpellElement.Earth);
			if ((resist & (byte)SpellElement.Poison) > 0) elementSet.Remove(SpellElement.Poison);
			if ((resist & (byte)SpellElement.Time) > 0) elementSet.Remove(SpellElement.Time);
			if ((resist & (byte)SpellElement.Death) > 0) elementSet.Remove(SpellElement.Death);
			if ((resist & (byte)SpellElement.Status) > 0) elementSet.Remove(SpellElement.Status);

			var list = elements.Where(e => elementSet.Contains(e)).ToList();

			var e = list.PickRandom(rng);
			a.ElementalResist |= (byte)e;
			elements.Remove(e);
		}

		private void AddRandomElement(Armor a, List<SpellElement> elements)
		{
			var e = elements.SpliceRandom(rng);
			a.ElementalResist |= (byte)e;
			allElements.Remove(e);
		}

		private (bool, Armor, Armor) CheckForCompleteProtection(List<Armor> armors)
		{
			int score = 0;
			Armor bestA1 = null;
			Armor bestA2 = null;

			foreach (var a1 in armors)
			{
				var s1 = GetScore(a1.ElementalResist);
				if (s1 > score && s1 >= 6)
				{
					bestA1 = a1;
					bestA2 = null;
					score = s1;
				}

				foreach (var a2 in armors.Where(a => a.Type != a1.Type))
				{
					var s2 = GetScore(a2.ElementalResist);
					var s = GetScore(a1.ElementalResist | a2.ElementalResist);
					if (s > score && s1 >= 6 && s2 >= 4)
					{
						bestA1 = a1;
						bestA2 = a2;
						score = s;
					}

					foreach(var a3 in armors.Where(a => a.Type != a1.Type && a.Type != a2.Type))
					{
						if ((a1.ElementalResist | a2.ElementalResist | a3.ElementalResist) == 0xFF) return (true, null, null);
					}
				}
			}

			return (false, bestA1, bestA2);
		}

		private int GetScore(int elementalResist)
		{
			var score = 0;
			if ((elementalResist & (int)SpellElement.Fire) > 0) score += 2;
			if ((elementalResist & (int)SpellElement.Lightning) > 0) score += 2;
			if ((elementalResist & (int)SpellElement.Ice) > 0) score += 2;
			if ((elementalResist & (int)SpellElement.Earth) > 0) score += 2;
			if ((elementalResist & (int)SpellElement.Poison) > 0) score += 2;
			if ((elementalResist & (int)SpellElement.Time) > 0) score += 2;
			if ((elementalResist & (int)SpellElement.Death) > 0) score += 3;
			if ((elementalResist & (int)SpellElement.Status) > 0) score += 3;

			return score;
		}
	}
}
