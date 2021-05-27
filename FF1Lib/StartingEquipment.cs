using FF1Lib.Data;
using FF1Lib.Helpers;
using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public class StartingEquipment
	{
		MT19337 rng;
		Flags flags;
		FF1Rom rom;

		SpellHelper spellHelper;
		List<Weapon> weapons;
		List<Armor> armors;

		public StartingEquipment(MT19337 _rng, Flags _flags, FF1Rom _rom)
		{
			rng = _rng;
			flags = _flags;
			rom = _rom;

			spellHelper = new SpellHelper(rom);
			weapons = Weapon.LoadAllWeapons(rom, flags).ToList();
			armors = Armor.LoadAllArmors(rom, flags).ToList();
		}

		public void SetStartingEquipment()
		{
			var items = GetEquipment();

			ExtConsumables.ExtConsumableStartingEquipmentFix(items.Weapons, flags);

			if (flags.StartingEquipmentRemoveFromPool) ReplaceTreasures(items.Weapons.Concat(items.Armor));

			for (int i = 0; i < 16; i++)
			{
				if (items.Weapons[i] > 0) items.Weapons[i] -= Item.Soft;
				if (items.Armor[i] > 0) items.Armor[i] -= Item.Masamune;
			}

			for (int i = 0; i < 4; i++)
			{
				byte[] buffer1 = new byte[4];
				Buffer.BlockCopy(items.Weapons, 4 * i, buffer1, 0,  4);

				byte[] buffer2 = new byte[4];
				Buffer.BlockCopy(items.Armor, 4 * i, buffer2, 0,  4);

				rom.PutInBank(0x1B, 0x8520 + 0x08 * i, buffer1);
				rom.PutInBank(0x1B, 0x8524 + 0x08 * i, buffer2);
			}

			rom.PutInBank(0x1B, 0x8540, Blob.FromHex("A200A000205785A040205785A080205785A0C020578560BD2085991861E8C898290FC908D0F160"));

		}

		private (Item[] Weapons, Item[] Armor) GetEquipment()
		{
			List<Item> items = new List<Item>();

			if (flags.StartingEquipmentMasamune ?? false) items.Add(Item.Masamune);
			if (flags.StartingEquipmentKatana ?? false) items.Add(Item.Katana);
			if (flags.StartingEquipmentHealStaff ?? false) GetHealRod(items);
			if (flags.StartingEquipmentZeusGauntlet ?? false) items.Add(Item.ZeusGauntlets);
			if (flags.StartingEquipmentWhiteShirt ?? false) items.Add(Item.WhiteShirt);
			if (flags.StartingEquipmentRibbon ?? false) items.Add(Item.Ribbon);

			if ((flags.StartingEquipmentDragonslayer ?? false) && !(flags.Weaponizer ?? false))
			{
				items.Add(Item.DragonArmor);
				items.Add(Item.DragonSword);
			}

			if ((flags.StartingEquipmentLegendKit ?? false) && !(flags.Weaponizer ?? false))
			{
				items.Add(Item.Xcalber);
				items.Add(Item.OpalArmor);
				items.Add(Item.OpalGauntlets);
				items.Add(Item.OpalHelm);
				items.Add(Item.OpalShield);
			}

			if (flags.StartingEquipmentRandomEndgameWeapon ?? false) GetRandomEndgameWeapon(items);
			if (flags.StartingEquipmentRandomAoe ?? false) GetRandomAoe(items);
			if (flags.StartingEquipmentRandomCasterItem ?? false) GetRandomCasterItem(items);
			if (flags.StartingEquipmentOneItem ?? false) GetOneItem(items);
			if (flags.StartingEquipmentGrandpasSecretStash ?? false) GetGrandpasSecretStash(items);
			if (flags.StartingEquipmentRandomCrap ?? false) GetRandomCrap(items);


			var weaponsList = items.Where(i => i < Item.Cloth).Take(16).ToList();
			var armorList = items.Where(i => i >= Item.Cloth).Take(16).ToList();

			weaponsList.Shuffle(rng);
			armorList.Shuffle(rng);

			var weapons = weaponsList.ToArray();
			var armor = armorList.ToArray();

			Array.Resize(ref weapons, 16);
			Array.Resize(ref armor, 16);

			return (weapons, armor);
		}

		private void GetHealRod(List<Item> items)
		{
			if (!(flags.Weaponizer ?? false))
			{
				items.Add(Item.HealRod);
			}
			else
			{
				var spells = new HashSet<Spell>(spellHelper.FindSpells(SpellRoutine.Heal, SpellTargeting.AllCharacters).Where(s => s.Info.effect <= 32).Select(s => s.Id));
				var weapon = weapons.FirstOrDefault(w => w.Damage < 20 && spells.Contains(w.Spell));

				if (weapon != null)
				{
					items.Add(weapon.Id);
				}
				else
				{
					items.Add(Item.HealHelm);
				}
			}
		}

		private void GetRandomEndgameWeapon(List<Item> items)
		{
			var pool = ItemLists.UberTier.Concat(ItemLists.LegendaryWeaponTier).Concat(new Item[] { Item.FlameSword, Item.IceSword, Item.SunSword, Item.CoralSword }).Concat(new Item[] { Item.Katana, Item.Katana }).ToList();

			if (flags.StartingEquipmentNoDuplicates) pool.RemoveAll(i => items.Contains(i));

			items.Add(pool.PickRandom(rng));
		}

		private void GetRandomAoe(List<Item> items)
		{
			if (!(flags.Weaponizer ?? false) && !(flags.MagisizeWeapons ?? false))
			{
				var pool = new Item[] { Item.BaneSword, Item.BlackShirt, Item.ZeusGauntlets, Item.ThorHammer, Item.MageRod }.ToList();

				if (flags.StartingEquipmentNoDuplicates) pool.RemoveAll(i => items.Contains(i));

				if (pool.Count > 0) items.Add(pool.PickRandom(rng));
			}
			else
			{
				var damageAoes = spellHelper.FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies).Select(s => s.Id);
				var instaAoes = spellHelper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.AllEnemies, SpellElement.Any, SpellStatus.Death).Select(s => s.Id);
				var powerWordAoes = spellHelper.FindSpells(SpellRoutine.PowerWord, SpellTargeting.AllEnemies, SpellElement.Any, SpellStatus.Death).Select(s => s.Id);

				var spells = new HashSet<Spell>(damageAoes.Concat(instaAoes).Concat(powerWordAoes));

				var weaps = weapons.Where(w => spells.Contains(w.Spell)).Select(w => w.Id);
				var arms = armors.Where(w => spells.Contains(w.Spell)).Select(w => w.Id);

				var pool = weaps.Concat(arms).ToList();

				var names = weapons.Where(w => spells.Contains(w.Spell)).Select(w => w.Name).Concat(armors.Where(w => spells.Contains(w.Spell)).Select(a => a.Name)).ToList();

				if (pool.Count > 0) items.Add(pool.PickRandom(rng));
			}
		}

		private void GetRandomCasterItem(List<Item> items)
		{
			if (!(flags.Weaponizer ?? false) && !(flags.MagisizeWeapons ?? false))
			{
				var pool = new Item[] { Item.BaneSword, Item.BlackShirt, Item.ZeusGauntlets, Item.ThorHammer, Item.MageRod, Item.Defense, Item.WhiteShirt, Item.HealRod, Item.PowerGauntlets }.ToList();

				if (flags.StartingEquipmentNoDuplicates) pool.RemoveAll(i => items.Contains(i));

				if (pool.Count > 0) items.Add(pool.PickRandom(rng));
			}
			else
			{
				var pool = GetCasterItemPool();

				if (pool.Count > 0) items.Add(pool.PickRandom(rng));
			}
		}

		private List<Item> GetCasterItemPool()
		{
			var spells = new HashSet<Spell>(spellHelper.GetAllSpells().Where(s =>
				s.Info.routine == (byte)SpellRoutine.Damage ||
				s.Info.routine == (byte)SpellRoutine.DamageUndead ||
				s.Info.routine == (byte)SpellRoutine.Fast ||
				s.Info.routine == (byte)SpellRoutine.FullHeal ||
				s.Info.routine == (byte)SpellRoutine.Heal ||
				s.Info.routine == (byte)SpellRoutine.InflictStatus &&
					(
						s.Info.effect == (byte)SpellStatus.Death ||
						s.Info.effect == (byte)SpellStatus.Confuse ||
						s.Info.effect == (byte)SpellStatus.Stone
					) ||
				s.Info.routine == (byte)SpellRoutine.Lock ||
				s.Info.routine == (byte)SpellRoutine.PowerWord ||
				s.Info.routine == (byte)SpellRoutine.Ruse ||
				s.Info.routine == (byte)SpellRoutine.Sabr).Select(s => s.Id));

			var weaps = weapons.Where(w => spells.Contains(w.Spell)).Select(w => w.Id);
			var arms = armors.Where(w => spells.Contains(w.Spell)).Select(w => w.Id);

			var names = weapons.Where(w => spells.Contains(w.Spell)).Select(w => w.Name).Concat(armors.Where(w => spells.Contains(w.Spell)).Select(a => a.Name)).ToList();

			return weaps.Concat(arms).ToList();
		}

		private void GetOneItem(List<Item> items)
		{
			var pool = GetLegendaryPool();

			if (flags.StartingEquipmentNoDuplicates) pool.RemoveAll(i => items.Contains(i));

			items.Add(pool.PickRandom(rng));
		}

		private List<Item> GetLegendaryPool()
		{
			if (!(flags.Weaponizer ?? false) && !(flags.MagisizeWeapons ?? false))
			{
				return ItemLists.UberTier
				.Concat(ItemLists.LegendaryWeaponTier)
				.Concat(ItemLists.LegendaryArmorTier.Distinct())
				.Concat(new Item[] { Item.Katana, Item.Katana, Item.Katana })
				.Concat(new Item[] { Item.BaneSword, Item.BlackShirt, Item.ZeusGauntlets, Item.ThorHammer, Item.MageRod, Item.Defense, Item.WhiteShirt, Item.HealRod, Item.PowerGauntlets }).ToList();
			}
			else
			{
				return ItemLists.UberTier
				.Concat(ItemLists.LegendaryWeaponTier)
				.Concat(ItemLists.LegendaryArmorTier)
				.Concat(GetCasterItemPool()).Distinct()
				.Concat(new Item[] { Item.Katana, Item.Katana, Item.Katana }).ToList();
			}
		}

		private void GetGrandpasSecretStash(List<Item> items)
		{
			var pool = GetLegendaryPool();

			if (flags.StartingEquipmentNoDuplicates) pool.RemoveAll(i => items.Contains(i));
			items.Add(pool.SpliceRandom(rng));

			if (flags.StartingEquipmentNoDuplicates) pool.RemoveAll(i => items.Contains(i));
			items.Add(pool.SpliceRandom(rng));

			if (flags.StartingEquipmentNoDuplicates) pool.RemoveAll(i => items.Contains(i));
			items.Add(pool.SpliceRandom(rng));

			var pool2 = ItemLists.CommonArmorTier.Concat(ItemLists.CommonWeaponTier).ToList();

			items.Add(pool2.PickRandom(rng));
			items.Add(pool2.PickRandom(rng));
			items.Add(pool2.PickRandom(rng));
			items.Add(pool2.PickRandom(rng));
			items.Add(pool2.PickRandom(rng));
		}

		private void GetRandomCrap(List<Item> items)
		{
			var pool1 = GetLegendaryPool();
			var pool2 = ItemLists.RareWeaponTier.Concat(ItemLists.RareArmorTier).Where(i => !pool1.Contains(i)).ToList();
			var pool3 = ItemLists.CommonArmorTier.Concat(ItemLists.CommonWeaponTier).ToList();

			var hqPool = new HashSet<Item>(pool1.Concat(pool2));
			var itemSet = new HashSet<Item>(items);

			var pool = pool1
				.Concat(pool2)
				.Concat(pool2)
				.Concat(pool2);

			for (int i = 0; i < 30; i++) pool = pool.Concat(pool3);

			var weapons = pool.Where(i => i < Item.Cloth).ToList();
			var armor = pool.Where(i => i >= Item.Cloth).ToList();

			weapons.Shuffle(rng);
			armor.Shuffle(rng);

			for (int i = 0; i < 16; i++)
			{
				if (flags.StartingEquipmentNoDuplicates) weapons.RemoveAll(i => itemSet.Contains(i) && hqPool.Contains(i));

				var item = weapons.PickRandom(rng);

				items.Add(item);
				itemSet.Add(item);
			}

			for (int i = 0; i < 16; i++)
			{
				if (flags.StartingEquipmentNoDuplicates)armor.RemoveAll(i => itemSet.Contains(i) && hqPool.Contains(i));

				var item = armor.PickRandom(rng);

				items.Add(item);
				itemSet.Add(item);
			}
		}


		private void ReplaceTreasures(IEnumerable<Item> items)
		{
			var treasures = new TreasureData(rom);
			treasures.LoadTable();

			var pool1 = GetLegendaryPool();
			var pool2 = ItemLists.RareWeaponTier.Concat(ItemLists.RareArmorTier).Where(i => !pool1.Contains(i)).ToList();
			var pool3 = ItemLists.CommonArmorTier.Concat(ItemLists.CommonWeaponTier).ToList();

			var hqPool = new HashSet<Item>(pool1.Concat(pool2));
			items = items.Where(i => hqPool.Contains(i)).ToArray();

			foreach (var i in items)
			{
				var indices = treasures.Data.Select((item, idx) => (item, idx)).Where(e => e.item == i).ToList();
				if (indices.Count > 0)
				{
					var idx = indices.PickRandom(rng).idx;
					treasures[idx] = ExtConsumables.ExtConsumableStartingEquipmentFix(pool3.PickRandom(rng), flags);
				}
			}

			treasures.StoreTable();
		}
	}
}
