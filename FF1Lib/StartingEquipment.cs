using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public enum StartingEquipmentSet
	{
		[Description("None")]
		None,

		[Description("Masamune")]
		Masamune,

		[Description("Katana")]
		Katana,

		[Description("HealStaff")]
		HealStaff,

		[Description("ZeusGauntlet")]
		ZeusGauntlet,

		[Description("White Shirt")]
		WhiteShirt,

		[Description("Ribbon")]
		Ribbon,

		[Description("Dragonslayer")]
		Dragonslayer,

		[Description("LegendKit")]
		LegendKit,

		[Description("R. Endgame Weapon")]
		RandomEndgameWeapon,

		[Description("R. Endgame Gear")]
		RandomEndgameGear,

		[Description("R. Aoe Item")]
		RandomAoe,

		[Description("R. Caster Item")]
		RandomCasterItem,

		[Description("Grandpas Secret Stash")]
		GrandpasSecretStash,

		[Description("One Helpful Item")]
		OneItem,

		[Description("Random Crap")]
		RandomCrap
	}


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

			switch (flags.StartingEquipment)
			{
				case StartingEquipmentSet.None:
					break;
				case StartingEquipmentSet.Masamune:
					items.Add(Item.Masamune);
					break;
				case StartingEquipmentSet.Katana:
					items.Add(Item.Katana);
					break;
				case StartingEquipmentSet.HealStaff:
					items.Add(Item.HealRod);
					break;
				case StartingEquipmentSet.ZeusGauntlet:
					items.Add(Item.ZeusGauntlets);
					break;
				case StartingEquipmentSet.WhiteShirt:
					items.Add(Item.WhiteShirt);
					break;
				case StartingEquipmentSet.Ribbon:
					items.Add(Item.Ribbon);
					break;
				case StartingEquipmentSet.Dragonslayer:
					items.Add(Item.DragonArmor);
					items.Add(Item.DragonSword);
					break;
				case StartingEquipmentSet.LegendKit:
					items.Add(Item.Xcalber);
					items.Add(Item.OpalArmor);
					items.Add(Item.OpalGauntlets);
					items.Add(Item.OpalHelm);
					items.Add(Item.OpalShield);
					break;
				case StartingEquipmentSet.RandomEndgameWeapon:
					GetRandomEndgameWeapon(items);
					break;
				case StartingEquipmentSet.RandomEndgameGear:
					GetRandomEndgameGear(items);
					break;
				case StartingEquipmentSet.RandomAoe:
					GetRandomAoe(items);
					break;
				case StartingEquipmentSet.RandomCasterItem:
					GetRandomCasterItem(items);
					break;
				case StartingEquipmentSet.OneItem:
					GetOneItem(items);
					break;
				case StartingEquipmentSet.GrandpasSecretStash:
					GetGrandpasSecretStash(items);
					break;
				case StartingEquipmentSet.RandomCrap:
					GetRandomCrap(items);
					break;
			}

			items.Shuffle(rng);

			var weapons = items.Where(i => i < Item.Cloth).ToArray();
			var armor = items.Where(i => i >= Item.Cloth).ToArray();

			Array.Resize(ref weapons, 16);
			Array.Resize(ref armor, 16);

			return (weapons, armor);
		}

		private void GetRandomEndgameWeapon(List<Item> items)
		{
			var pool = ItemLists.UberTier.Concat(ItemLists.LegendaryWeaponTier).Concat(new Item[] { Item.FlameSword, Item.IceSword, Item.DragonSword }).Concat(new Item[] { Item.Katana, Item.Katana }).ToList();

			items.Add(pool.PickRandom(rng));
		}

		private void GetRandomEndgameGear(List<Item> items)
		{
			var pool = ItemLists.UberTier.Concat(ItemLists.LegendaryWeaponTier).Concat(new Item[] { Item.FlameSword, Item.IceSword, Item.DragonSword }).Concat(new Item[] { Item.Katana, Item.Katana}).Concat(ItemLists.LegendaryArmorTier).ToList();

			items.Add(pool.PickRandom(rng));
		}

		private void GetRandomAoe(List<Item> items)
		{
			var pool = new Item[] { Item.BaneSword, Item.BlackShirt, Item.ZeusGauntlets, Item.ThorHammer, Item.MageRod }.ToList();

			items.Add(pool.PickRandom(rng));
		}

		private void GetRandomCasterItem(List<Item> items)
		{
			var pool = new Item[] { Item.BaneSword, Item.BlackShirt, Item.ZeusGauntlets, Item.ThorHammer, Item.MageRod, Item.Defense, Item.WhiteShirt, Item.HealRod, Item.PowerGauntlets }.ToList();

			items.Add(pool.PickRandom(rng));
		}

		private void GetOneItem(List<Item> items)
		{
			var pool = GetLegendaryPool();

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

			items.Add(pool.SpliceRandom(rng));
			items.Add(pool.SpliceRandom(rng));
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

			var pool = pool1
				.Concat(pool2)
				.Concat(pool2)
				.Concat(pool2);

			for (int i = 0; i < 30; i++) pool = pool.Concat(pool3);

			var weapons = pool.Where(i => i < Item.Cloth).ToList();
			var armor = pool.Where(i => i >= Item.Cloth).ToList();

			weapons.Shuffle(rng);
			armor.Shuffle(rng);

			items.AddRange(weapons.Take(16));
			items.AddRange(armor.Take(16));
		}
	}
}
