using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public enum ShopKillMode
	{
		[Description("None")]
		None,

		[Description("Whole Shops")]
		WholeShops,

		[Description("All Items of a Kind")]
		AllItemsOfAKind,

		[Description("Spread")]
		Spread,

		[Description("Unequal")]
		Chaos,

		[Description("Random")]
		Random
	}

	public enum ShopKillFactor
	{
		[Description("Remove 20%")]
		Kill20Percent,

		[Description("Remove 40%")]
		Kill40Percent,

		[Description("Remove 60%")]
		Kill60Percent,

		[Description("Remove 80%")]
		Kill80Percent,

		[Description("Remove All")]
		KillAll,

		[Description("Random")]
		KillRandom
	}

	public class ShopKiller
	{
		private const byte DoorReplacementTile = 29;
		private const byte SignReplacementTile = 24;
		private readonly MT19337 rng;
		private readonly Flags flags;
		private readonly FF1Rom rom;
		private readonly List<Map> maps;
		private readonly ShopData ShopData;
		private readonly HashSet<Item> QuestItems;

		public ShopKiller(MT19337 _rng, Flags _flags, List<Map> _maps, FF1Rom _rom)
		{
			rng = _rng;
			flags = _flags;
			rom = _rom;
			maps = _maps;

			ShopData = new ShopData(rom);
			QuestItems = new HashSet<Item>(ItemLists.AllQuestItems);
		}

		public void KillShops()
		{
			ShopData.LoadData();

			KillShops(ShopType.Weapon, flags.ShopKillMode_Weapons, flags.ShopKillFactor_Weapons, flags.ShopKillExcludeConeria_Weapons);
			KillShops(ShopType.Armor, flags.ShopKillMode_Armor, flags.ShopKillFactor_Armor, flags.ShopKillExcludeConeria_Armor);
			KillShops(ShopType.Item, flags.ShopKillMode_Item, flags.ShopKillFactor_Item, flags.ShopKillExcludeConeria_Item);
			KillShops(ShopType.Black, flags.ShopKillMode_Black, flags.ShopKillFactor_Black, flags.ShopKillExcludeConeria_Black);
			KillShops(ShopType.White, flags.ShopKillMode_White, flags.ShopKillFactor_White, flags.ShopKillExcludeConeria_White);

			ShopData.StoreData();
		}

		private void KillShops(ShopType shopType, ShopKillMode mode, ShopKillFactor factor, bool excludeConeria)
		{
			if (mode == ShopKillMode.Random)
			{
				mode = (ShopKillMode)Math.Max(rng.Between(-3, 4), 0);
			}

			if (factor == ShopKillFactor.KillRandom)
			{
				factor = (ShopKillFactor)rng.Between(0, 4);
			}

			switch (mode)
			{
				case ShopKillMode.WholeShops:
					KillWholeShops(shopType, factor, excludeConeria);
					break;
				case ShopKillMode.AllItemsOfAKind:
					KillAllItemsOfAKind(shopType, factor, excludeConeria);
					break;
				case ShopKillMode.Spread:
					KillSpreads(shopType, factor, excludeConeria);
					break;
				case ShopKillMode.Chaos:
					KillChaos(shopType, factor, excludeConeria);
					break;
				case ShopKillMode.None:
					break;
				case ShopKillMode.Random:
					break;
				default:
					break;
			}
		}

		private void KillWholeShops(ShopType shopType, ShopKillFactor factor, bool excludeConeria)
		{
			List<Shop> shops = ShopData.Shops.Where(s => s.Type == shopType).ToList();
			if (excludeConeria)
			{
				shops = shops.Where(s => s.Location != MapLocation.Coneria).ToList();
			}

			shops.Shuffle(rng);

			int cnt = (int)Math.Round(shops.Count * GetKillFactor(factor) / 100.0f, 0);

			List<Shop> shopsToRemove = shops.Take(cnt).ToList();

			foreach (Shop shop in shopsToRemove)
			{
				shop.Entries = shop.Entries.Where(e => QuestItems.Contains(e)).ToList();
				if (shop.Entries.Count == 0)
				{
					shop.Entries.Add(GetDefaultItem(shopType));
					RemoveShopFromMap(shop);
				}
			}
		}

		private void KillAllItemsOfAKind(ShopType shopType, ShopKillFactor factor, bool excludeConeria)
		{
			List<Shop> shops = ShopData.Shops.Where(s => s.Type == shopType).ToList();
			if (excludeConeria)
			{
				shops = shops.Where(s => s.Location != MapLocation.Coneria).ToList();
			}

			List<Item> dinstinctEntries = shops.SelectMany(s => s.Entries).Distinct().Where(e => !QuestItems.Contains(e)).ToList();

			dinstinctEntries.Shuffle(rng);

			int cnt = (int)Math.Round(dinstinctEntries.Count * GetKillFactor(factor) / 100.0f, 0);

			HashSet<Item> dinstinctToRemove = new(dinstinctEntries.Take(cnt));

			foreach (Shop shop in shops)
			{
				shop.Entries = shop.Entries.Where(e => !dinstinctToRemove.Contains(e)).ToList();

				if (shop.Entries.Count == 0)
				{
					shop.Entries.Add(GetDefaultItem(shopType));
					RemoveShopFromMap(shop);
				}
			}
		}

		private void KillSpreads(ShopType shopType, ShopKillFactor factor, bool excludeConeria)
		{
			List<Shop> shops = ShopData.Shops.Where(s => s.Type == shopType).ToList();
			if (excludeConeria)
			{
				shops = shops.Where(s => s.Location != MapLocation.Coneria).ToList();
			}

			foreach (Shop shop in shops)
			{
				List<Item> entries = shop.Entries.Where(e => !QuestItems.Contains(e)).ToList();

				entries.Shuffle(rng);

				int cnt = (int)Math.Round(shop.Entries.Count * GetKillFactor(factor) / 100.0f, 0);

				List<Item> entriesToRemove = entries.Take(cnt).ToList();

				foreach (Item e in entriesToRemove)
				{
					shop.Entries.Remove(e);
				}

				if (shop.Entries.Count == 0)
				{
					shop.Entries.Add(GetDefaultItem(shopType));
					RemoveShopFromMap(shop);
				}
			}
		}

		private void KillChaos(ShopType shopType, ShopKillFactor factor, bool excludeConeria)
		{
			List<Shop> shops = ShopData.Shops.Where(s => s.Type == shopType).ToList();
			if (excludeConeria)
			{
				shops = shops.Where(s => s.Location != MapLocation.Coneria).ToList();
			}

			List<(Shop s, Item e)> allEntries = shops.Select(s => s.Entries.Select(e => (s, e))).SelectMany(x => x).Where(e => !QuestItems.Contains(e.e)).ToList();

			allEntries.Shuffle(rng);

			int cnt = (int)Math.Round(allEntries.Count * GetKillFactor(factor) / 100.0f, 0);

			List<(Shop s, Item e)> entriesToRemove = allEntries.Take(cnt).ToList();

			foreach ((Shop s, Item e) in entriesToRemove)
			{
				s.Entries.Remove(e);

				if (s.Entries.Count == 0)
				{
					s.Entries.Add(GetDefaultItem(shopType));
					RemoveShopFromMap(s);
				}
			}
		}

		private void RemoveShopFromMap(Shop shop)
		{
			//Caravan
			if (shop.Index == 69)
			{
				return;
			}

			Map map = maps[(int)shop.MapId];

			if (map.FindFirst(shop.TileId, out int x, out int y))
			{
				map[y, x] = DoorReplacementTile;
				//map[y - 1, x] = SignReplacementTile;
			}
		}

		private int GetKillFactor(ShopKillFactor factor)
		{
			switch (factor)
			{
				case ShopKillFactor.Kill20Percent: return 20;
				case ShopKillFactor.Kill40Percent: return 40;
				case ShopKillFactor.Kill60Percent: return 60;
				case ShopKillFactor.Kill80Percent: return 80;
				case ShopKillFactor.KillAll: return 100;
				case ShopKillFactor.KillRandom:
				default: return 0;
			}
		}

		private Item GetDefaultItem(ShopType shopType)
		{
			switch (shopType)
			{
				case ShopType.Weapon: return Item.SmallKnife;
				case ShopType.Armor: return Item.Cap;
				case ShopType.Item: return Item.Soft;
				case ShopType.White: return (Item)184;//Lamp
				case ShopType.Black: return (Item)189;//Dark
				case ShopType.Clinic:
				case ShopType.Inn:
				default: return Item.None;
			}
		}
	}
}
