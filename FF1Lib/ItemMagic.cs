using FF1Lib.Helpers;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public const int GearTextOffset = 0x2B9BD;
		public const int GearTextSize = 8;
		private const int MagicBitOffset = 0x3;
		private const int SpellNamesOffset = 0x2BE03;
		private const int SpellNamesSize = 5;
		private const int SpellNamesCount = 64;

		private int WeaponStart = (byte)ItemLists.AllWeapons.ElementAt(0);
		private int ArmorStart = (byte)ItemLists.AllArmor.ElementAt(0);

		public void ShuffleItemMagic(MT19337 rng, Flags flags)
		{
			IEnumerable<Item> magicWeapons;

			if ((flags.MagisizeWeapons ?? false) && flags.ItemMagicPool == ItemMagicPool.Balanced)
			{
				//this case needs to be done with the original magisize code, because it has a different behavior.
				MagisizeWeaponsBalanced(rng);
				magicWeapons = Weapon.LoadAllWeapons(this, flags).Where(w => w.Spell >= Spell.CURE).Select(w => w.Id);
			}
			else if (flags.MagisizeWeapons ?? false)
			{
				magicWeapons = Weapon.LoadAllWeapons(this, flags).Select(w => w.Id);
			}
			else
			{
				magicWeapons = Weapon.LoadAllWeapons(this, flags).Where(w => w.Spell >= Spell.CURE).Select(w => w.Id);
			}

			var magicArmor = Armor.LoadAllArmors(this, flags).Where(w => w.Spell >= Spell.CURE).Select(w => w.Id);
			var magicItems = magicWeapons.Concat(magicArmor).ToList();

			List<MagicSpell> Spells;

			if (flags.ItemMagicPool == ItemMagicPool.Balanced)
			{
				Spells = GetBalancedSpells(rng);
			}
			else if (flags.ItemMagicPool == ItemMagicPool.Low)
			{
				Spells = GetLowSpells(rng);
			}
			else if (flags.ItemMagicPool == ItemMagicPool.Support)
			{
				Spells = GetSupportSpells(rng);
			}
			else if (flags.ItemMagicPool == ItemMagicPool.Curated)
			{
				Spells = GetTournamentSpells(rng, (bool)flags.MagisizeWeapons);
			}
			else
			{
				Spells = GetAllSpells(rng);
			}

			if ((bool)flags.StartingEquipmentRandomAoe && flags.ItemMagicMode != ItemMagicMode.None) {
				// guarantee that at least one item has an AoE attack spell
				if (Spells.Where(spell => spell.IsAoEAttack()).Count() < 1) {
					var spellHelper = new SpellHelper(this);
					var aoe_spells = new List<MagicSpell>(spellHelper.GetAoEAttackSpells().Select(s => s.Info).ToList());
					aoe_spells.Shuffle(rng);
					Spells.Add(aoe_spells.First());
				}
			}

			foreach (var item in Spells.Zip(magicItems, (s, i) => new { Spell = s, Item = i }))
			{
				WriteItemSpellData(item.Spell, item.Item);
			}
		}

		public void MagisizeWeaponsBalanced(MT19337 rng)
		{
			var Spells = GetSpells();

			var tieredSpells = new List<List<MagicSpell>> { Spells.GetRange(0, 16), Spells.GetRange(16, 16), Spells.GetRange(32, 16), Spells.GetRange(48, 16) };

			var commonOdds = new List<int> { 0, 0, 0, 0, 1, 1, 1, 1, 2, 2 };
			var rareOdds = new List<int> { 0, 1, 1, 1, 2, 2, 2, 3, 3, 3 };
			var legendaryOdds = new List<int> { 1, 2, 2, 2, 3, 3, 3, 3, 3, 3 };

			for (int i = 0; i < 4; i++)
				tieredSpells[i].RemoveAll(spell => spell.Data[4] == 0);

			foreach (Item weapon in ItemLists.CommonWeaponTier)
			{
				var selectedTier = commonOdds.PickRandom(rng);
				while (tieredSpells[selectedTier].Count == 0)
					selectedTier = commonOdds.PickRandom(rng);

				WriteItemSpellData(tieredSpells[selectedTier].SpliceRandom(rng), weapon);
			}

			foreach (Item weapon in ItemLists.RareWeaponTier.Except(ItemLists.AllMagicItem).ToList())
			{
				var selectedTier = rareOdds.PickRandom(rng);
				while (tieredSpells[selectedTier].Count == 0)
					selectedTier = rareOdds.PickRandom(rng);

				WriteItemSpellData(tieredSpells[selectedTier].SpliceRandom(rng), weapon);
			}

			foreach (Item weapon in ItemLists.LegendaryWeaponTier)
			{
				var selectedTier = legendaryOdds.PickRandom(rng);
				while (tieredSpells[selectedTier].Count == 0)
					selectedTier = legendaryOdds.PickRandom(rng);

				WriteItemSpellData(tieredSpells[selectedTier].SpliceRandom(rng), weapon);
			}

			foreach (Item weapon in ItemLists.UberTier)
			{
				var selectedTier = Rng.Between(rng, 0, 3);
				while (tieredSpells[selectedTier].Count == 0)
					selectedTier = Rng.Between(rng, 0, 3);

				WriteItemSpellData(tieredSpells[selectedTier].SpliceRandom(rng), weapon);
			}
		}

		private List<MagicSpell> GetAllSpells(MT19337 rng)
		{
			var Spells = GetSpells();

			// Remove out of battle only spells (spells where the effect is 0)
			Spells.RemoveAll(spell => spell.Data[4] == 0);

			Spells.Shuffle(rng); // Shuffle all spells remaining, then assign to each item that can cast a spell
			return Spells;
		}

		private List<MagicSpell> GetBalancedSpells(MT19337 rng)
		{
			var Spells = GetSpells();

			// Remove out of battle only spells (spells where the effect is 0)
			Spells.RemoveAll(spell => spell.Data[4] == 0);

			// if balanced shuffle is on, remove spells which are too strong or too weak
			// remove any spell which boosts attack power that isn't self-casting
			Spells.RemoveAll(spell => spell.Data[4] == 0x0D && spell.Data[3] != 0x04);
			// remove any spell that doubles hit rate
			Spells.RemoveAll(spell => spell.Data[4] == 0x0C);
			// remove any non-elemental damage spell with >55 effectivity
			Spells.RemoveAll(spell => spell.Data[4] == 0x01 && spell.Data[1] > 55 && spell.Data[2] == 0b0000000);
			// remove any multitarget elemental damage spell with > 70 effectivity
			Spells.RemoveAll(spell => spell.Data[4] == 0x01 && spell.Data[1] > 70 && spell.Data[3] == 0x01);
			// remove any singletarget elemental damage spell with > 100 effectivity
			Spells.RemoveAll(spell => spell.Data[4] == 0x01 && spell.Data[1] > 100);
			// remove any damage spell with <20 effectivity
			Spells.RemoveAll(spell => spell.Data[4] == 0x01 && spell.Data[1] < 20);
			// remove status recovery spells which don't heal paralysis or mute
			Spells.RemoveAll(spell => spell.Data[4] == 0x08 && (spell.Data[1] & 0b01010000) == 0);
			// remove HP Max
			Spells.RemoveAll(spell => spell.Data[4] == 0x0F);
			// remove HARM spells with > 120 effectivity
			Spells.RemoveAll(spell => spell.Data[4] == 0x02 && spell.Data[1] > 120);
			// remove non-elemental power word kill
			Spells.RemoveAll(spell => spell.Data[4] == 0x12 && (spell.Data[1] & 0b00000011) != 0 && spell.Data[2] == 0b00000000);
			// remove status spells which only cast darkness, sleep, or poison
			Spells.RemoveAll(spell => spell.Data[4] == 0x03 && (spell.Data[1] & 0b11010011) == 0);
			Spells.RemoveAll(spell => spell.Data[4] == 0x12 && (spell.Data[1] & 0b11010011) == 0);
			// remove evasion up spells with effects greater than 80
			Spells.RemoveAll(spell => spell.Data[4] == 0x10 && spell.Data[1] > 80);
			// remove armor up spells with effects greater than 32
			Spells.RemoveAll(spell => spell.Data[4] == 0x09 && spell.Data[1] > 32);
			// remove spells which resist all elements
			Spells.RemoveAll(spell => spell.Data[4] == 0x0A && spell.Data[1] == 0xFF);

			Spells.Shuffle(rng); // Shuffle all spells remaining, then assign to each item that can cast a spell
			return Spells;
		}

		private List<MagicSpell> GetLowSpells(MT19337 rng)
		{
			var Spells = GetSpells();

			SpellHelper spellHelper = new SpellHelper(this);
			List<(Spell Id, MagicSpell Info)> foundSpells = GetLowSpells(spellHelper);

			var Spells2 = new List<MagicSpell>();
			foreach (var spl in foundSpells)
			{
				var idx = (int)spl.Id - 0xB0;
				Spells2.Add(Spells[idx]);
			}

			Spells2.Shuffle(rng); // Shuffle all spells remaining, then assign to each item that can cast a spell
			return Spells2;
		}

		private List<(Spell Id, MagicSpell Info)> GetLowSpells(SpellHelper spellHelper)
		{
			List<(Spell Id, MagicSpell Info)> foundSpells = new List<(Spell Id, MagicSpell Info)>();

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.ArmorUp, SpellTargeting.Self));
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.ArmorUp, SpellTargeting.OneCharacter));

			//Remove Life and Smoke
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.CureAilment, SpellTargeting.Any).Where(s => s.Info.effect != 0x00 && s.Info.effect != 0x81 && s.Info.effect != 0x01));

			//up to Ice2
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Damage, SpellTargeting.Any).Where(s => s.Info.effect <= 40));

			//Up to Hrm3
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.DamageUndead, SpellTargeting.Any).Where(s => s.Info.effect <= 60));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Fast, SpellTargeting.Self));
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Fast, SpellTargeting.OneCharacter));

			//up to Cur3, Hel2
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Heal, SpellTargeting.OneCharacter).Where(s => s.Info.effect <= 40));
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Heal, SpellTargeting.AllCharacters).Where(s => s.Info.effect <= 25));

			//Allow single target insta and multi target status
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.AllEnemies, SpellElement.Status));
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.OneEnemy));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Lock, SpellTargeting.Any));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.PowerWord, SpellTargeting.OneEnemy));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Ruse, SpellTargeting.Any).Where(s => s.Info.effect <= 50));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Sabr, SpellTargeting.Self).Where(s => s.Info.effect <= 15));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Sabr, SpellTargeting.OneCharacter));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Smoke, SpellTargeting.Any));

			return foundSpells.Concat(foundSpells).ToList();
		}

		private List<MagicSpell> GetTournamentSpells(MT19337 rng, bool AllWeaponsMagic)
		{
			var Spells = GetSpells();

			SpellHelper spellHelper = new SpellHelper(this);
			List<(Spell Id, MagicSpell Info)> foundSpells = GetTournamentSpells(spellHelper, rng, AllWeaponsMagic);

			var Spells2 = new List<MagicSpell>();
			foreach (var spl in foundSpells)
			{
				var idx = (int)spl.Id - 0xB0;
				Spells2.Add(Spells[idx]);
			}

			Spells2.Shuffle(rng); // Shuffle all spells remaining, then assign to each item that can cast a spell
			return Spells2;
		}

		private List<(Spell Id, MagicSpell Info)> GetTournamentSpells(SpellHelper spellHelper, MT19337 rng, bool AllWeaponsMagic)
		{
			List<(Spell Id, MagicSpell Info)> foundSpells = new List<(Spell Id, MagicSpell Info)>();

			//up to Ice2
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies).Where(s => s.Info.effect <= 40));
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Damage, SpellTargeting.OneEnemy).Where(s => s.Info.effect >= 40 && s.Info.effect <= 120));
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Heal, SpellTargeting.OneCharacter).Where(s => s.Info.effect >= 40 && s.Info.effect <= 64));
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Heal, SpellTargeting.AllCharacters).Where(s => s.Info.effect >= 20 && s.Info.effect <= 32));

			//double chance
			foundSpells.AddRange(foundSpells.ToList());

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.DamageUndead, SpellTargeting.AllEnemies).Where(s => s.Info.effect >= 40 && s.Info.effect <= 60));
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.DamageUndead, SpellTargeting.OneEnemy).Where(s => s.Info.effect >= 80 && s.Info.effect <= 120));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Fast, SpellTargeting.Self));
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Fast, SpellTargeting.OneCharacter));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any).Where(s => s.Info.effect == (byte)SpellStatus.Death));
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any).Where(s => s.Info.effect == (byte)SpellStatus.Stone));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Lock, SpellTargeting.Any));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.PowerWord, SpellTargeting.OneEnemy).Where(s => s.Info.effect == (byte)SpellStatus.Death));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Ruse, SpellTargeting.Any).Where(s => s.Info.effect <= 50));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Sabr, SpellTargeting.Self).Where(s => s.Info.effect <= 15));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Sabr, SpellTargeting.OneCharacter));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any, SpellElement.Any, SpellStatus.Confuse));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any, SpellElement.Any, SpellStatus.Stun));

			foundSpells.Shuffle(rng);

			// For normal situations, prevents any spell from appearing on more than two items
			var selection = foundSpells.Take(22).Distinct();
			// Use the full set with the All Weapons Cast Spells flag so that there's enough
			if (AllWeaponsMagic) {
				selection = foundSpells;
			}

			return selection.Concat(selection).ToList();
		}

		private List<MagicSpell> GetSupportSpells(MT19337 rng)
		{
			var Spells = GetSpells();

			SpellHelper spellHelper = new SpellHelper(this);
			List<(Spell Id, MagicSpell Info)> foundSpells = GetSupportSpells(spellHelper);

			var Spells2 = new List<MagicSpell>();
			foreach (var spl in foundSpells)
			{
				var idx = (int)spl.Id - 0xB0;
				Spells2.Add(Spells[idx]);
			}

			Spells2.Shuffle(rng); // Shuffle all spells remaining, then assign to each item that can cast a spell
			return Spells2;
		}

		private List<(Spell Id, MagicSpell Info)> GetSupportSpells(SpellHelper spellHelper)
		{
			List<(Spell Id, MagicSpell Info)> foundSpells = new List<(Spell Id, MagicSpell Info)>();

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.ArmorUp, SpellTargeting.Any));

			//Remove Life and Smoke
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.CureAilment, SpellTargeting.Any).Where(s => s.Info.effect != 0x00 && s.Info.effect != 0x81 && s.Info.effect != 0x01));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Fast, SpellTargeting.Any));

			//up to Cur3, Hel2
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Heal, SpellTargeting.OneCharacter).Where(s => s.Info.effect <= 40));
			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Heal, SpellTargeting.AllCharacters).Where(s => s.Info.effect <= 25));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any, SpellElement.Status));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Lock, SpellTargeting.Any));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Ruse, SpellTargeting.Any));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Sabr, SpellTargeting.Any));

			foundSpells.AddRange(spellHelper.FindSpells(SpellRoutine.Smoke, SpellTargeting.Any));

			return foundSpells.Concat(foundSpells).ToList();
		}

		private void WriteItemSpellData(MagicSpell Spell, Item item)
		{
			// Set the spell an item casts
			var offset = WeaponOffset + 0x8 * Math.Min((byte)item - WeaponStart, ArmorStart - WeaponStart) + 0x4 * Math.Max(0, (byte)item - ArmorStart) + MagicBitOffset;
			Data[offset] = (byte)(Spell.Index + 1);

			if (ItemsText[(int)item].Contains("@"))
			{
				ItemsText[(int)item] = ItemsText[(int)item].Remove(0, 6).Insert(0, Spell.Name.PadRight(6));
			}
			else
			{
				ItemsText[(int)item] = Spell.Name.PadRight(7);
			}
		}

		public void CastableItemTargeting()
		{
			// update a lut with the correct location of a routine
			Put(0x314E2, Blob.FromHex("EC9563"));

			// see 0C_94F5_BatleSubMenu_MagicDrinkItem.asm for all changes
			// This is only useful if Item Magic is shuffled since there are no single target spells on items in vanilla
			int changesOffset = 0x314F5; // offset where changes start

			Put(changesOffset, Get(changesOffset + 0x3, 0x14));
			Put(changesOffset + 0x14, Blob.FromHex("2097F8"));
			Put(changesOffset + 0x17, Get(changesOffset + 0x1D, 0x81));
			Put(changesOffset + 0xAC, Get(changesOffset + 0x9F, 0x6));
			Put(changesOffset + 0xB2, Blob.FromHex("04A0FF"));
			Put(changesOffset + 0x99, Blob.FromHex("A940209D95C9FFF0034C5D934CF59448AE7D6B"));
			Put(changesOffset + 0xB5, Blob.FromHex("68604A901020A49AAE7D6BC902D00468A9FF6068604A9008AD7A6B0980A868604A900468A0FE60AD7A6B203A9AAE7D6BC902D00468A9FF60ADAB6A29030980A86860"));
			Put(changesOffset + 0xF7, Get(changesOffset + 0x100, 0x42));
			Put(changesOffset + 0x127, Blob.FromHex("11"));
			Put(changesOffset + 0x139, Get(changesOffset + 0x14F, 0xB0));
			Put(changesOffset + 0x144, Blob.FromHex("EC"));
			Put(changesOffset + 0x14F, Blob.FromHex("7D6B"));
			Put(changesOffset + 0x155, Blob.FromHex("7D6B"));
			Put(changesOffset + 0x1CF, Blob.FromHex("D6"));
			Put(changesOffset + 0x1E9, Blob.FromHex("8D7D6BCE7D6BAD7D6B20119785808681A910209D95C9FFF0034C5D934C639600"));

			// Writes the spell name rather than the item name, not in the .asm, intruduces a small bug when using non castable items
			//Put(0x3307D, Blob.FromHex("0CAD8C6CC942B00369B04CA0B0C901D008AD8C6C38E927D00AAD8C6CC942B0031869B0"));

			PutInBank(0x0C, 0x96E7, Blob.FromHex("A82011972065C2EAEA"));
			PutInBank(0x1F, 0xC265, CreateLongJumpTableEntry(0x0F, 0x8AD0));
			PutInBank(0x0F, 0x8AD0, Blob.FromHex("85808681C0FFD008A9D68580A9968581A91060"));
		}
	}

	public partial class FF1Rom
	{
		public void CraftDefenseItem(Flags flags)
		{
			var newspell = GetSpells();

			var ruse = newspell.Select((s, i) => (s, i)).Where(x => x.s.Data[4] == 0x10 && x.s.Data[3] == 0x04).Select(s => (int?)s.i).FirstOrDefault();
			var inv = newspell.Select((s, i) => (s, i)).Where(x => x.s.Data[4] == 0x10 && x.s.Data[3] == 0x10).Select(s => (int?)s.i).FirstOrDefault();
			var inv2 = newspell.Select((s, i) => (s, i)).Where(x => x.s.Data[4] == 0x10 && x.s.Data[3] == 0x08).Select(s => (int?)s.i).FirstOrDefault();

			//no spell was found
			if (ruse == null && inv == null && inv2 == null) return;

			//backup spells
			ruse = ruse.HasValue ? ruse : (inv2.HasValue ? inv2 : inv);
			inv = inv.HasValue ? inv : (inv2.HasValue ? inv2 : ruse);
			inv2 = inv2.HasValue ? inv2 : (ruse.HasValue ? ruse : inv);

			switch (flags.GuaranteedDefenseItem)
			{
				case GuaranteedDefenseItem.RUSE:
					WriteItemSpellData(newspell[ruse.Value], Item.PowerRod);
					break;
				case GuaranteedDefenseItem.INV:
					WriteItemSpellData(newspell[inv.Value], Item.PowerRod);
					break;
				case GuaranteedDefenseItem.INV2:
					WriteItemSpellData(newspell[inv2.Value], Item.PowerRod);
					break;
			}
		}

		public void CraftPowerItem(Flags flags)
		{
			var newspell = GetSpells();

			var sabr = newspell.Select((s, i) => (s, i)).Where(x => x.s.Data[4] == 0x0D && x.s.Data[3] == 0x04).Select(s => (int?)s.i).FirstOrDefault();
			var tmpr = newspell.Select((s, i) => (s, i)).Where(x => x.s.Data[4] == 0x0D && x.s.Data[3] == 0x10).Select(s => (int?)s.i).FirstOrDefault();
			var fast = newspell.Select((s, i) => (s, i)).Where(x => x.s.Data[4] == 0x0C).Select(s => (int?)s.i).FirstOrDefault();

			//no spell was found
			if (sabr == null && tmpr == null && fast == null) return;

			//backup spells
			sabr = sabr.HasValue ? sabr : (tmpr.HasValue ? tmpr : fast);
			tmpr = tmpr.HasValue ? tmpr : (sabr.HasValue ? sabr : fast);
			fast = fast.HasValue ? fast : (sabr.HasValue ? sabr : tmpr);

			switch (flags.GuaranteedPowerItem)
			{
				case GuaranteedPowerItem.SABR:
					WriteItemSpellData(newspell[sabr.Value], Item.PowerGauntlets);
					break;
				case GuaranteedPowerItem.TMPR:
					WriteItemSpellData(newspell[tmpr.Value], Item.PowerGauntlets);
					break;
				case GuaranteedPowerItem.FAST:
					WriteItemSpellData(newspell[fast.Value], Item.PowerGauntlets);
					break;
			}
		}
	}

	public enum ItemMagicMode
	{
		Vanilla = 0,
		Randomized = 1,
		None = 2,
		Random = 3
	}

	public enum ItemMagicPool
	{
		All = 0,
		Balanced = 1,
		Low = 2,
		Support = 3,
		Curated = 4,
		Random = 5
	}

	public enum GuaranteedDefenseItem
	{
		None = 0,
		INV = 1,
		INV2 = 2,
		RUSE = 3,
		Any = 4,
		Random = 5
	}

	public enum GuaranteedPowerItem
	{
		None = 0,
		TMPR = 1,
		SABR = 2,
		FAST = 3,
		Any = 4,
		Random = 5
	}
}
