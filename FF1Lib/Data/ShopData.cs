namespace FF1Lib
{
	public enum ShopType
	{
		Weapon = 0,
		Armor = 10,
		White = 20,
		Black = 30,
		Clinic = 40,
		Inn = 50,
		Item = 60
	}

	public class Shop
	{
		public int Index { get; private set; }

		public ShopType Type { get; private set; }

		public MapLocation Location { get; private set; }

		public MapId MapId { get; private set; }

		public byte TileId { get; private set; }

		public string Name { get; private set; }

		public List<Item> Entries { get; set; }

		public ushort Price { get; set; }

		public Shop(int _index, ShopType _type, MapLocation _location, MapId mapId, byte tileId, string name, List<Item> _items)
		{
			Index = _index;
			Type = _type;
			Location = _location;
			MapId = mapId;
			TileId = tileId;
			Name = name;

			Entries = _items;
		}

		public Shop(int _index, ShopType _type, MapLocation _location, MapId mapId, byte tileId, string name, ushort price)
		{
			Index = _index;
			Type = _type;
			Location = _location;
			MapId = mapId;
			TileId = tileId;
			Name = name;

			Price = price;
		}

		public Shop CloneEmpty()
		{
			return new Shop(Index, Type, Location, MapId, TileId, Name, new List<Item>());
		}
	}

	public class ShopData
	{
		public const int ShopPointerOffset = 0x38302; // 0x38300 technically, but the first one is unused.
		public const int ShopPointerBase = 0x30000;
		public const int ShopPointerSize = 2;
		public const int ShopPointerCount = 70;
		public const int ShopSectionSize = 10;
		public const ushort ShopNullPointer = 0x838E;

		private Dictionary<int, Shop> ShopPrototypes = new Dictionary<int, Shop>
		{
			{ 0, new Shop(0, ShopType.Weapon, MapLocation.Coneria, MapId.Coneria ,30, "Coneria Weapon", null) },
			{ 1, new Shop(1, ShopType.Weapon, MapLocation.Pravoka, MapId.Pravoka ,31, "Pravoka Weapon", null) },
			{ 2, new Shop(2, ShopType.Weapon, MapLocation.Elfland, MapId.Elfland ,38, "Elfland Weapon", null) },
			{ 3, new Shop(3, ShopType.Weapon, MapLocation.Melmond, MapId.Melmond ,45, "Melmond Weapon", null) },
			{ 4, new Shop(4, ShopType.Weapon, MapLocation.CrescentLake, MapId.CrescentLake ,54, "CrescentLake Weapon", null) },
			{ 5, new Shop(5, ShopType.Weapon, MapLocation.Gaia, MapId.Gaia ,62, "Gaia Weapon", null) },
			{ 10, new Shop(10, ShopType.Armor, MapLocation.Coneria, MapId.Coneria ,67, "Coneria Armor", null) },
			{ 11, new Shop(11, ShopType.Armor, MapLocation.Pravoka, MapId.Pravoka ,68, "Pravoka Armor", null) },
			{ 12, new Shop(12, ShopType.Armor, MapLocation.Elfland, MapId.Elfland ,69, "Elfland Armor", null) },
			{ 13, new Shop(13, ShopType.Armor, MapLocation.Melmond, MapId.Melmond ,70, "Melmond Armor", null) },
			{ 14, new Shop(14, ShopType.Armor, MapLocation.CrescentLake, MapId.CrescentLake ,73, "CrescentLake Armor", null) },
			{ 15, new Shop(15, ShopType.Armor, MapLocation.Gaia, MapId.Gaia ,74, "Gaia Armor", null) },
			{ 20, new Shop(20, ShopType.White, MapLocation.Coneria, MapId.Coneria ,79, "Coneria White", null) },
			{ 21, new Shop(21, ShopType.White, MapLocation.Pravoka, MapId.Pravoka ,80, "Pravoka White", null) },
			{ 22, new Shop(22, ShopType.White, MapLocation.Elfland, MapId.Elfland ,81, "Elfland White L3", null) },
			{ 23, new Shop(23, ShopType.White, MapLocation.Melmond, MapId.Melmond ,82, "Melmond White", null) },
			{ 24, new Shop(24, ShopType.White, MapLocation.CrescentLake, MapId.CrescentLake ,83, "CrescentLake White", null) },
			{ 25, new Shop(25, ShopType.White, MapLocation.Elfland, MapId.Elfland ,84, "Elfland White L4", null) },
			{ 26, new Shop(26, ShopType.White, MapLocation.Gaia, MapId.Gaia ,85, "Gaia White L7", null) },
			{ 27, new Shop(27, ShopType.White, MapLocation.Gaia, MapId.Gaia ,86, "Gaia White L8", null) },
			{ 28, new Shop(28, ShopType.White, MapLocation.Onrac, MapId.Onrac ,87, "Onrac White", null) },
			{ 29, new Shop(29, ShopType.White, MapLocation.Lefein, MapId.Lefein ,88, "Lefein White", null) },
			{ 30, new Shop(30, ShopType.Black, MapLocation.Coneria, MapId.Coneria ,89, "Coneria Black", null) },
			{ 31, new Shop(31, ShopType.Black, MapLocation.Pravoka, MapId.Pravoka ,90, "Pravoka Black", null) },
			{ 32, new Shop(32, ShopType.Black, MapLocation.Elfland, MapId.Elfland ,91, "Elfland Black L3", null) },
			{ 33, new Shop(33, ShopType.Black, MapLocation.Melmond, MapId.Melmond ,92, "Melmond Black", null) },
			{ 34, new Shop(34, ShopType.Black, MapLocation.CrescentLake, MapId.CrescentLake ,93, "CrescentLake Black", null) },
			{ 35, new Shop(35, ShopType.Black, MapLocation.Elfland, MapId.Elfland ,94, "Elfland Black L4", null) },
			{ 36, new Shop(36, ShopType.Black, MapLocation.Gaia, MapId.Gaia ,95, "Gaia Black L7", null) },
			{ 37, new Shop(37, ShopType.Black, MapLocation.Gaia, MapId.Gaia ,96, "Gaia Black L8", null) },
			{ 38, new Shop(38, ShopType.Black, MapLocation.Onrac, MapId.Onrac ,97, "Onrac Black", null) },
			{ 39, new Shop(39, ShopType.Black, MapLocation.Lefein, MapId.Lefein ,98, "Lefein Black", null) },
			{ 40, new Shop(40, ShopType.Clinic, MapLocation.Coneria, MapId.Coneria ,99, "Coneria Clinic", null) },
			{ 41, new Shop(41, ShopType.Clinic, MapLocation.Elfland, MapId.Elfland ,100, "Elfland Clinic", null) },
			{ 42, new Shop(42, ShopType.Clinic, MapLocation.CrescentLake, MapId.CrescentLake ,101, "CrescentLake Clinic", null) },
			{ 43, new Shop(43, ShopType.Clinic, MapLocation.Gaia, MapId.Gaia ,102, "Gaia Clinic", null) },
			{ 44, new Shop(44, ShopType.Clinic, MapLocation.Onrac, MapId.Onrac ,103, "Onrac Clinic", null) },
			{ 45, new Shop(45, ShopType.Clinic, MapLocation.Pravoka, MapId.Pravoka ,104, "Pravoka Clinic", null) },
			{ 50, new Shop(50, ShopType.Inn, MapLocation.Coneria, MapId.Coneria ,109, "Coneria Inn", null) },
			{ 51, new Shop(51, ShopType.Inn, MapLocation.Pravoka, MapId.Pravoka ,110, "Pravoka Inn", null) },
			{ 52, new Shop(52, ShopType.Inn, MapLocation.Elfland, MapId.Elfland ,111, "Elfland Inn", null) },
			{ 53, new Shop(53, ShopType.Inn, MapLocation.Melmond, MapId.Melmond ,112, "Melmond Inn", null) },
			{ 54, new Shop(54, ShopType.Inn, MapLocation.CrescentLake, MapId.CrescentLake ,113, "CrescentLake Inn", null) },
			{ 55, new Shop(55, ShopType.Inn, MapLocation.Gaia, MapId.Gaia ,114, "Gaia Inn", null) },
			{ 56, new Shop(56, ShopType.Inn, MapLocation.Onrac, MapId.Onrac ,115, "Onrac Inn", null) },
			{ 60, new Shop(60, ShopType.Item, MapLocation.Coneria, MapId.Coneria ,119, "Coneria Item", null) },
			{ 61, new Shop(61, ShopType.Item, MapLocation.Pravoka, MapId.Pravoka ,120, "Pravoka Item", null) },
			{ 62, new Shop(62, ShopType.Item, MapLocation.Elfland, MapId.Elfland ,121, "Elfland Item", null) },
			{ 63, new Shop(63, ShopType.Item, MapLocation.CrescentLake, MapId.CrescentLake ,122, "CrescentLake Item", null) },
			{ 64, new Shop(64, ShopType.Item, MapLocation.Gaia, MapId.Gaia ,123, "Gaia Item", null) },
			{ 65, new Shop(65, ShopType.Item, MapLocation.Onrac, MapId.Onrac ,124, "Onrac Item", null) },
			{ 69, new Shop(69, ShopType.Item, MapLocation.Caravan, MapId.Coneria ,0, "Caravan Item", null) },
		};

		public List<Shop> Shops { get; private set; }

		FF1Rom rom;
		MemTable<ushort> Index;

		public ShopData(FF1Rom _rom)
		{
			rom = _rom;
			Index = new MemTable<ushort>(rom, ShopPointerOffset, ShopPointerCount);
		}

		public void LoadData()
		{
			Index.LoadTable();

			Shops = new List<Shop>();
			foreach (var shopType in Enum.GetValues<ShopType>())
			{
				Shops.AddRange(GetShops(shopType));
			}
		}

		public int StoreData()
		{
			var shopdic = Shops.ToDictionary(s => s.Index);
			var shopdata = new List<byte>();

			for (int i = 0; i < 70; i++)
			{
				if (shopdic.TryGetValue(i, out var shop))
				{
					Index.Data[i] = (ushort)(ShopNullPointer + 1 + shopdata.Count);

					if (shop.Type == ShopType.Clinic || shop.Type == ShopType.Inn)
					{
						shopdata.Add((byte)(shop.Price & 0xFF));
						shopdata.Add((byte)((shop.Price >> 8) & 0xFF));
						shopdata.Add(0);
					}
					else
					{
						shopdata.AddRange(shop.Entries.Cast<byte>());
						shopdata.Add(0);
					}
				}
				else
				{
					Index.Data[i] = ShopNullPointer;
				}

				Index.StoreTable();
				rom.Put(ShopPointerBase + ShopNullPointer + 1, shopdata.Cast<byte>().ToArray());
			}

			return 241 - shopdata.Count();
		}

		private List<Shop> GetShops(ShopType shopType)
		{
			var shops = new List<Shop>();
			for (int i = 0; i < ShopSectionSize; i++)
			{
				if (Index.Data[(int)shopType + i] != ShopNullPointer)
				{
					var prototype = ShopPrototypes[(int)shopType + i];

					if (shopType == ShopType.Clinic || shopType == ShopType.Inn)
					{
						ushort price = rom.Get(ShopPointerBase + Index.Data[(int)shopType + i], 2).ToUShorts()[0];
						shops.Add(new Shop(prototype.Index, prototype.Type, prototype.Location, prototype.MapId, prototype.TileId, prototype.Name, price));
					}
					else
					{
						var entries = new List<byte>();
						var shopEntries = rom.Get(ShopPointerBase + Index.Data[(int)shopType + i], 5);

						for (int j = 0; j < 5 && shopEntries[j] != 0; j++)
						{
							entries.Add(shopEntries[j]);
						}

						shops.Add(new Shop(prototype.Index, prototype.Type, prototype.Location, prototype.MapId, prototype.TileId, prototype.Name, entries.Where(e => e != 0).Cast<Item>().ToList()));
					}
				}
			}

			return shops;
		}

		public int GetShopEntryPointer(Shop shop, int index)
		{
			return ShopPointerBase + Index.Data[shop.Index] + index;
		}

		public void SetShops(IEnumerable<Shop> newShops)
		{
			foreach (var shop in newShops) SetShop(shop);
		}

		public void SetShop(Shop shop)
		{
			Shops.First(s => s.Index == shop.Index).Entries = shop.Entries;
		}
		public ItemShopSlot UpdateShopSlotAddress(ItemShopSlot itemShop)
		{
			var targetShop = Shops.Find(s => s.Index == itemShop.ShopIndex);
			var itemSlot = targetShop.Entries.FindIndex(e => e == itemShop.Item);

			// If the slot was removed by ShopKiller, remove slot from placement
			if (itemSlot < 0 || (itemShop.Item == Item.Soft && targetShop.Entries.Count == 1))
			{
				return null;
			}

			return new(GetShopEntryPointer(targetShop, itemSlot), itemShop.Name, itemShop.MapLocation, itemShop.Item, (byte)targetShop.Index);
		}
		public void UpdateShopSlotPlacement(List<IRewardSource> placement)
		{
			if (placement == null)
			{
				return;
			}

			var placedshops = placement.Where(s => s.GetType() == typeof(ItemShopSlot));
			if (!placedshops.Any())
			{
				return;
			}

			ItemShopSlot placedShopSlot = (ItemShopSlot)placedshops.First();

			ItemShopSlot newShopSlot = UpdateShopSlotAddress(placedShopSlot);

			placement.Remove(placedShopSlot);
			if (newShopSlot != null)
			{
				placement.Add(newShopSlot);
			}
		}
	}
}
