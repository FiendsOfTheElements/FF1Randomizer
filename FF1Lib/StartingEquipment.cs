using FF1Lib.Data;
using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class StartingEquipment
	{
		MT19337 rng;
		Flags flags;
		FF1Rom rom;

		public StartingEquipment(MT19337 _rng, Flags _flags, FF1Rom _rom)
		{
			rng = _rng;
			flags = _flags;
			rom = _rom;
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
			if (flags.StartingEquipmentHealStaff ?? false) items.Add(Item.HealRod);
			if (flags.StartingEquipmentZeusGauntlet ?? false) items.Add(Item.ZeusGauntlets);
			if (flags.StartingEquipmentWhiteShirt ?? false) items.Add(Item.WhiteShirt);
			if (flags.StartingEquipmentRibbon ?? false) items.Add(Item.Ribbon);

			if (flags.StartingEquipmentDragonslayer ?? false)
			{
				items.Add(Item.DragonArmor);
				items.Add(Item.DragonSword);
			}

			if (flags.StartingEquipmentLegendKit ?? false)
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

		private void GetRandomEndgameWeapon(List<Item> items)
		{
			var pool = ItemLists.UberTier.Concat(ItemLists.LegendaryWeaponTier).Concat(new Item[] { Item.FlameSword, Item.IceSword, Item.DragonSword }).Concat(new Item[] { Item.Katana, Item.Katana }).ToList();

			if (flags.StartingEquipmentNoDuplicates) pool.RemoveAll(i => items.Contains(i));

			items.Add(pool.PickRandom(rng));
		}

		private void GetRandomAoe(List<Item> items)
		{
			var pool = new Item[] { Item.BaneSword, Item.BlackShirt, Item.ZeusGauntlets, Item.ThorHammer, Item.MageRod }.ToList();

			if (flags.StartingEquipmentNoDuplicates) pool.RemoveAll(i => items.Contains(i));

			items.Add(pool.PickRandom(rng));
		}

		private void GetRandomCasterItem(List<Item> items)
		{
			var pool = new Item[] { Item.BaneSword, Item.BlackShirt, Item.ZeusGauntlets, Item.ThorHammer, Item.MageRod, Item.Defense, Item.WhiteShirt, Item.HealRod, Item.PowerGauntlets }.ToList();

			if (flags.StartingEquipmentNoDuplicates) pool.RemoveAll(i => items.Contains(i));

			items.Add(pool.PickRandom(rng));
		}

		private void GetOneItem(List<Item> items)
		{
			var pool = GetLegendaryPool();

			if (flags.StartingEquipmentNoDuplicates) pool.RemoveAll(i => items.Contains(i));

			items.Add(pool.PickRandom(rng));
		}

		private static List<Item> GetLegendaryPool()
		{
			return ItemLists.UberTier
				.Concat(ItemLists.LegendaryWeaponTier)
				.Concat(ItemLists.LegendaryArmorTier.Distinct())
				.Concat(new Item[] { Item.Katana, Item.Katana, Item.Katana })
				.Concat(new Item[] { Item.BaneSword, Item.BlackShirt, Item.ZeusGauntlets, Item.ThorHammer, Item.MageRod, Item.Defense, Item.WhiteShirt, Item.HealRod, Item.PowerGauntlets }).ToList();
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
