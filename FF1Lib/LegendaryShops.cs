using FF1Lib.Data;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public class LegendaryShops
	{
		MT19337 rng;
		Flags flags;
		FF1Rom rom;
		StandardMaps maps;
		List<MapIndex> flippedMaps;
		List<MapIndex> vflippedMaps;

		Dictionary<MapIndex, byte> MapTileSets;
		ShopData ShopData;
		//TileSet[] TileSets = new TileSet[8];
		TileSetsData TileSets;

		Dictionary<byte, List<byte>> UnusedTilesbyTileSet;
		Dictionary<string, MagicSpell> Spells;

		List<SpellInfo> SpellInfos;
		TreasureData treasureData;

		//MapIndex, X, Y, UL Tile, UR Tile, BL Tile, BR Tile, Pallette
		private List<(MapIndex, int, int, byte, byte, byte, byte, byte)> Locations = new List<(MapIndex, int, int, byte, byte, byte, byte, byte)>
		{
			(MapIndex.EarthCaveB2, 20, 34, 0x22, 0x23, 0x32, 0x33, 0xFF),
			(MapIndex.MarshCaveB2, 13, 44, 0x22, 0x23, 0x32, 0x33, 0xAA),
			(MapIndex.DwarfCave, 12, 50, 0x22, 0x23, 0x32, 0x33, 0xFF),
			(MapIndex.MirageTower1F, 23, 02, 0x22, 0x23, 0x32, 0x33, 0xAA),
			(MapIndex.IceCaveB1, 25, 24, 0x22, 0x23, 0x32, 0x33, 0xFF),
			(MapIndex.SeaShrineB4, 27, 40, 0x22, 0x23, 0x32, 0x33, 0xAA)
		};

		public LegendaryShops(MT19337 _rng, Flags _flags, StandardMaps _maps, ShopData _shopdata, TileSetsData _tilesets, FF1Rom _rom)
		{
			rng = _rng;
			flags = _flags;
			rom = _rom;
			maps = _maps;
			flippedMaps = maps.HorizontalFlippedMaps;
			vflippedMaps = maps.VerticalFlippedMaps;
			TileSets = _tilesets;

			MapTileSets = maps.MapTileSets.Select((t,i) => (i, (byte)t)).ToDictionary(t => (MapIndex)t.i, t => t.Item2);
			ShopData = _shopdata;
			SpellInfos = rom.LoadSpells().ToList();
			treasureData = new TreasureData(rom);
		}

		private void LoadUnusedTileIds()
		{
			byte[] possibleTileIds = new byte[128];
			for (byte i = 0; i < 128; i++) possibleTileIds[i] = i;

			// Remove Closed door tile, since it's not used in the map, but still needed
			possibleTileIds[0x37] = 0;

			UnusedTilesbyTileSet = Enum.GetValues<MapIndex>()
				.GroupBy(m => MapTileSets[m])
				.Select(t => (t.Key, t.Select(m => maps[m].Map
						.Select(e => e.Value))
					.SelectMany(x => x)
					.Distinct()
					.ToDictionary(x => x)))
				.Select(t => (t.Key, possibleTileIds.Where(i => !t.Item2.ContainsKey(i)).ToList()))
				.ToDictionary(t => t.Key, t => t.Item2);
		}

		public void PlaceShops()
		{
			PrepareMaps();

			Spells = rom.GetSpells().ToDictionary(s => s.Name.ToLowerInvariant());
			/*
			for (int i = 0; i < 8; i++)
			{
				TileSets[i] = new TileSet(rom, (byte)i);
				TileSets[i].LoadData();
			}*/

			LoadUnusedTileIds();

			var allocatedslots = AllocateSlots();
			var pool = Locations.Take(allocatedslots.Where(s => s > 0).Count() + 1).ToList();
			pool.Shuffle(rng);

			treasureData.LoadTable();

			CreateWeaponShop(allocatedslots[0], pool);
			CreateArmorShop(allocatedslots[1], pool);
			CreateBlackShop(allocatedslots[2], pool);
			CreateWhiteShop(allocatedslots[3], pool);
			CreateItemShop(allocatedslots[4], pool);

			//ShopData.StoreData();
			treasureData.StoreTable();

			//for (int i = 0; i < 8; i++) TileSets[i].StoreData();

			rom.PutInBank(0x1F, 0xEBBD, Blob.FromHex("03"));//Set ShopType
			rom.PutInBank(0x1F, 0xEBC7, Blob.FromHex("02"));//Set ShopType
		}

		private void PrepareMaps()
		{
			maps[MapIndex.DwarfCave].Map[51, 12] = maps[MapIndex.DwarfCave].Map[51, 13];
		}

		private void CreateWeaponShop(int slots, List<(MapIndex, int, int, byte, byte, byte, byte, byte)> pool)
		{
			if (slots <= 0) return;

			Shop shop = new Shop(6, ShopType.Weapon, MapLocation.Coneria, MapIndex.ConeriaTown, 0, string.Empty, GetWeaponShopInventory(slots));

			if (flags.ExclusiveLegendaryWeaponShop)
			{
				RemoveFromShops(shop.Entries, Item.IronNunchucks);
				RemoveFromChests(shop.Entries, Item.Gold10);
			}

			if (shop.Entries.Count > 0)
			{
				ShopData.Shops.Add(shop);
				PlaceShop(shop.Index, pool);
			}
		}

		private void CreateArmorShop(int slots, List<(MapIndex, int, int, byte, byte, byte, byte, byte)> pool)
		{
			if (slots <= 0) return;

			Shop shop = new Shop(16, ShopType.Armor, MapLocation.Coneria, MapIndex.ConeriaTown, 0, string.Empty, GetArmorShopInventory(slots));

			if (flags.ExclusiveLegendaryArmorShop)
			{
				RemoveFromShops(shop.Entries, Item.Cloth);
				RemoveFromChests(shop.Entries, Item.Gold10);
			}

			if (shop.Entries.Count > 0)
			{
				ShopData.Shops.Add(shop);
				PlaceShop(shop.Index, pool);
			}
		}

		private void CreateBlackShop(int slots, List<(MapIndex, int, int, byte, byte, byte, byte, byte)> pool)
		{
			if (slots <= 0) return;

			Shop shop = new Shop(7, ShopType.Black, MapLocation.Coneria, MapIndex.ConeriaTown, 0, string.Empty, GetBlackShopInventory(slots));

			if (flags.ExclusiveLegendaryBlackShop)
			{
				RemoveFromShops(shop.Entries, GetBadBlackSpell());
			}

			if (shop.Entries.Count > 0)
			{
				ShopData.Shops.Add(shop);
				PlaceShop(shop.Index, pool);
			}
		}

		private void CreateWhiteShop(int slots, List<(MapIndex, int, int, byte, byte, byte, byte, byte)> pool)
		{
			if (slots <= 0) return;

			Shop shop = new Shop(17, ShopType.White, MapLocation.Coneria, MapIndex.ConeriaTown, 0, string.Empty, GetWhiteShopInventory(slots));

			if (flags.ExclusiveLegendaryWhiteShop)
			{
				RemoveFromShops(shop.Entries, GetBadWhiteSpell());
			}

			if (shop.Entries.Count > 0)
			{
				ShopData.Shops.Add(shop);
				PlaceShop(shop.Index, pool);
			}
		}

		private void CreateItemShop(int slots, List<(MapIndex, int, int, byte, byte, byte, byte, byte)> pool)
		{
			if (slots <= 0) return;

			Shop shop = new Shop(66, ShopType.Item, MapLocation.Coneria, MapIndex.ConeriaTown, 0, string.Empty, GetItemShopInventory(slots));

			if (flags.ExclusiveLegendaryItemShop)
			{
				RemoveFromShops(shop.Entries, Item.Soft);
			}

			if (shop.Entries.Count > 0)
			{
				ShopData.Shops.Add(shop);
				PlaceShop(shop.Index, pool);
			}
		}

		private void PlaceShop(int index, List<(MapIndex, int, int, byte, byte, byte, byte, byte)> pool)
		{
			var locidx = rng.Between(0, pool.Count - 1);
			var loc = pool[locidx];
			pool.RemoveAt(locidx);

			var tile = CreateTile(loc.Item1, index + 1, loc.Item4, loc.Item5, loc.Item6, loc.Item7, loc.Item8);

			if (flippedMaps.Contains(loc.Item1)) loc.Item2 = Map.RowLength - loc.Item2 - 1;
			if (vflippedMaps.Contains(loc.Item1)) loc.Item3 = Map.RowLength - loc.Item3 - 1;

			maps[loc.Item1].Map[loc.Item3, loc.Item2] = tile;

			if (loc.Item1 == MapIndex.MarshCaveB2 && rng.Between(1,3) == 1)
			{
				int x = flippedMaps.Contains(MapIndex.MarshCaveB1) ? Map.RowLength - 0x28 - 1 : 0x28;
				int y = vflippedMaps.Contains(MapIndex.MarshCaveB1) ? Map.RowLength - 0x18 - 1 : 0x18;

				maps[MapIndex.MarshCaveB1].Map[y, x] = tile;
			}
		}

		private byte CreateTile(MapIndex mapindex, int ShopId, byte ul, byte ur, byte bl, byte br, byte pi)
		{
			var tileSet = TileSets[MapTileSets[mapindex]];
			var tile = UnusedTilesbyTileSet[MapTileSets[mapindex]][0];
			UnusedTilesbyTileSet[MapTileSets[mapindex]].RemoveAt(0);

			tileSet.Tiles[tile] = new TileSM(tile, tileSet.Index, (TilePalette)pi, new() { ul, ur, bl, br }, 3, (byte)ShopId);
			/*
			tileSet.TileAttributes[tile] = pi;
			tileSet.TileProperties[tile] = new TileProp(3, (byte)ShopId);
			tileSet.TopLeftTiles[tile] = ul;
			tileSet.TopRightTiles[tile] = ur;
			tileSet.BottomLeftTiles[tile] = bl;
			tileSet.BottomRightTiles[tile] = br;*/

			return tile;
		}

		private int[] AllocateSlots()
		{
			int slots = ShopData.GetAvailableSlots();

			int[] result = new int[] { 0, 0, 0, 0, 0 };

			if (flags.LegendaryBlackShop ?? false) result[2] = Allocate(ref slots, 5);//Black
			if (flags.LegendaryArmorShop ?? false) result[1] = Allocate(ref slots, 5);//Armor
			if (flags.LegendaryWhiteShop ?? false) result[3] = Allocate(ref slots, 4);//White
			if (flags.LegendaryWeaponShop ?? false) result[0] = Allocate(ref slots, 3);//Weapon
			if (flags.LegendaryItemShop ?? false) result[4] = Allocate(ref slots, 3);//Item

			return result;
		}

		private int Allocate(ref int slots, int max)
		{
			int a = Math.Min(slots - 1, max);
			slots = slots - a - 1;
			return a;
		}

		private List<Item> GetItemShopInventory(int slots)
		{
			var items = new List<Item> { Item.Cabin, Item.House, Item.Heal, Item.Pure, Item.Soft };

			if (flags.ExtConsumableSet != ExtConsumableSet.None && (flags.LegendaryShopHasExtConsumables ?? false))
			{
				items.Add(Item.WoodenNunchucks);
				items.Add(Item.SmallKnife);
				items.Add(Item.WoodenRod);
				items.Add(Item.Rapier);
			}

			List<Item> result = new List<Item>();
			if (slots > 0)
			{
				Item save = (Item)rng.Between((int)Item.Cabin, (int)Item.House);
				result.Add(save);
				items.Remove(save);
				slots--;
			}

			if (slots > 0 && flags.ExtConsumableSet != ExtConsumableSet.None && (flags.LegendaryShopHasExtConsumables ?? false))
			{
				Item ext = (Item)rng.Between((int)Item.WoodenNunchucks, (int)Item.Rapier);
				result.Add(ext);
				items.Remove(ext);
				slots--;
			}

			items.Shuffle(rng);
			result.AddRange(items.Take(slots));

			return result;
		}

		private List<Item> GetWeaponShopInventory(int slots)
		{
			var items = new List<Item> { Item.Vorpal, Item.Katana, Item.Xcalber, Item.Defense, Item.FlameSword, Item.IceSword, Item.CoralSword, Item.SunSword, Item.BaneSword, Item.CatClaw, Item.ThorHammer, Item.WizardRod, Item.MageRod, Item.HealRod, Item.LightAxe };
			// Currently Absent Rares:
			// Item.DragonSword, Item.GiantSword, Item.WereSword, Item.RuneSword, Item.PowerRod

			List<Item> result = new List<Item>();

			items.Shuffle(rng);
			result.AddRange(items.Take(slots));

			return result;
		}

		private List<Item> GetArmorShopInventory(int slots)
		{
			var items = ItemLists.LegendaryArmorTier.Distinct().Concat(ItemLists.RareArmorTier).Distinct().ToList();

			List<Item> result = new List<Item>();

			items.Shuffle(rng);
			result.AddRange(items.Take(slots));

			return result;
		}

		private List<Item> GetWhiteShopInventory(int slots)
		{
			if (flags.GenerateNewSpellbook ?? false)
			{
				return GetCraftedSpellInventory(slots, false);
			}
			else
			{
				var spells = new List<Spell> { Spell.RUSE, Spell.INVS, Spell.CUR3, Spell.LIFE, Spell.HRM3, Spell.EXIT, Spell.INV2, Spell.CUR4, Spell.HRM4, Spell.HEL3, Spell.LIF2, Spell.FADE, Spell.WALL, Spell.HEL2, Spell.XFER };
				// If status restorative effects are set up to be especially rare, lean into that and makes the spells rarer too
				if (flags.ExclusiveLegendaryWhiteShop && flags.ExclusiveLegendaryItemShop && (flags.LegendaryWhiteShop ?? false) && (flags.LegendaryItemShop ?? false)) {
					spells.Add(Spell.SOFT);
					spells.Add(Spell.PURE);
				}

				var items = spells.Where(s => Spells.ContainsKey(s.ToString().ToLowerInvariant())).Select(s => Convert.ToByte(Spells[s.ToString().ToLowerInvariant()].Index + MagicNamesIndexInItemText)).Cast<Item>().ToList();

				List<Item> result = new List<Item>();

				items.Shuffle(rng);
				result.AddRange(items.Take(slots));

				return result;
			}
		}

		private List<Item> GetBlackShopInventory(int slots)
		{
			if (flags.GenerateNewSpellbook ?? false)
			{
				return GetCraftedSpellInventory(slots, true);
			}
			else
			{
				var spells = new List<Spell> { Spell.TMPR, Spell.FIR2, Spell.LIT2, Spell.FAST, Spell.ICE2, Spell.FIR3, Spell.BANE, Spell.WARP, Spell.LIT3, Spell.QAKE, Spell.ICE3, Spell.BRAK, Spell.SABR, Spell.NUKE, Spell.ZAP, Spell.XXXX };
				// LOCK & LOK2 are included as long as their accuracy is set to high or auto-hit
				if ((flags.LockMode != LockHitMode.Vanilla) && (flags.LockMode != LockHitMode.Accuracy107)) {
					spells.Add(Spell.LOCK);
					spells.Add(Spell.LOK2);
				}

				var items = spells.Where(s => Spells.ContainsKey(s.ToString().ToLowerInvariant())).Select(s => Convert.ToByte(Spells[s.ToString().ToLowerInvariant()].Index + MagicNamesIndexInItemText)).Cast<Item>().ToList();

				List<Item> result = new List<Item>();

				items.Shuffle(rng);
				result.AddRange(items.Take(slots));

				return result;
			}
		}

		private List<Item> GetCraftedSpellInventory(int slots, bool black)
		{
			var stDamSpells = SpellInfos.Where(s => s.routine == 0x01 && s.targeting != 0x01).Where(s => s.tier >= 3).OrderBy(s => -s.tier).Take(3);
			var aoeDamSpells = SpellInfos.Where(s => s.routine == 0x01 && s.targeting == 0x01).OrderBy(s => -s.tier).Take(6);

			var stHarmSpells = SpellInfos.Where(s => s.routine == 0x02 && s.targeting != 0x01).OrderBy(s => -s.tier).Take(1);
			var aoeHarmSpells = SpellInfos.Where(s => s.routine == 0x02 && s.targeting == 0x01).OrderBy(s => -s.tier).Take(1);

			var stHealSpells = SpellInfos.Where(s => (s.routine == 0x07 || s.routine == 0x0F) && s.targeting != 0x08).OrderBy(s => -s.tier).Take(2);
			var aoeHealSpells = SpellInfos.Where(s => (s.routine == 0x07 || s.routine == 0x0F) && s.targeting == 0x08).OrderBy(s => -s.tier).Take(2);

			var WallSpells = SpellInfos.Where(s => s.routine == 0x0A && s.effect == 0xFF).Take(1);
			var FastSpells = SpellInfos.Where(s => s.routine == 0x0C).OrderBy(s => -s.tier).Take(2);
			var SabrSpells = SpellInfos.Where(s => s.routine == 0x0D).OrderBy(s => -s.tier).Take(2);
			var LockSpells = SpellInfos.Where(s => s.routine == 0x0E).OrderBy(s => -s.tier).Take(2);
			var RuseSpells = SpellInfos.Where(s => s.routine == 0x10).OrderBy(s => -s.tier).Take(2);
			var WordSpells = SpellInfos.Where(s => s.routine == 0x12 && s.effect == 0b00000001).Take(1);
			var WordSpells2 = SpellInfos.Where(s => s.routine == 0x03 && (s.effect == 0b00000010 || s.effect == 0b00000001)).Take(3);

			var spells = stDamSpells
				.Concat(aoeDamSpells)
				.Concat(aoeHarmSpells)
				.Concat(stHealSpells)
				.Concat(aoeHealSpells)
				.Concat(WallSpells)
				.Concat(FastSpells)
				.Concat(SabrSpells)
				.Concat(LockSpells)
				.Concat(RuseSpells)
				.Concat(WordSpells)
				.Concat(WordSpells2)
				.Select(s => Convert.ToByte(SpellInfos.IndexOf(s)));

			var specialSpells = Spells.Where(s => s.Key.StartsWith("lif"))
			.Concat(Spells.Where(s => s.Key.StartsWith("warp")))
			.Concat(Spells.Where(s => s.Key.StartsWith("wrp")))
			.Concat(Spells.Where(s => s.Key.StartsWith("exit")))
			.Concat(Spells.Where(s => s.Key.StartsWith("ext")))
			.Concat(Spells.Where(s => s.Key.StartsWith("xfer")))
			.Concat(Spells.Where(s => s.Key.StartsWith("xfr")))
			.Select(s => Convert.ToByte(s.Value.Index));

			// Only add Soft & Pure if they are otherwise rare/special
			IEnumerable<byte> specialSpells2 = new List<byte>();
			if (flags.ExclusiveLegendaryWhiteShop && flags.ExclusiveLegendaryItemShop && (flags.LegendaryWhiteShop ?? false) && (flags.LegendaryItemShop ?? false)) {
				specialSpells2 = Spells.Where(s => s.Key.StartsWith("soft"))
				.Concat(Spells.Where(s => s.Key.StartsWith("sft")))
				.Concat(Spells.Where(s => s.Key.StartsWith("pure")))
				.Concat(Spells.Where(s => s.Key.StartsWith("pur")))
				.Select(s => Convert.ToByte(s.Value.Index));
			}


			var items = spells
				.Concat(specialSpells)
				.Concat(specialSpells2)
				.Where(s => BlackSpell(s) ^ !black)
				.ToList();

			var spellnames = Spells.Where(s => items.Contains(s.Value.Index)).Select(s => s.Key).ToList();

			List<Item> result = new List<Item>();

			items.Shuffle(rng);
			result.AddRange(items.Take(slots).Select(i => (Item)Convert.ToByte(i + 0xB0)));

			return result;
		}

		bool BlackSpell(int id) => id % 8 > 3;

		private Item GetBadBlackSpell()
		{
			var spell = SpellInfos.Where((s, i) => BlackSpell(i) && s.routine == 0x01).OrderBy(s => s.tier).First();
			return (Item)Convert.ToByte(SpellInfos.IndexOf(spell) + 0xB0);
		}

		private Item GetBadWhiteSpell()
		{
			var spell = SpellInfos.Where((s, i) => !BlackSpell(i) && s.routine == 0x07).OrderBy(s => s.tier).First();
			return (Item)Convert.ToByte(SpellInfos.IndexOf(spell) + 0xB0);
		}

		private void RemoveFromShops(IEnumerable<Item> entries, Item replacement)
		{
			foreach (var entry in entries)
			{
				foreach (var shop in ShopData.Shops)
				{
					if (shop.Entries == null) continue;

					shop.Entries.RemoveAll(e => e == entry);
					if (shop.Entries.Count == 0) shop.Entries.Add(replacement);
				}
			}
		}

		private void RemoveFromChests(List<Item> entries, Item replacement)
		{
			foreach (var entry in entries)
			{
				for (int i = 0; i < treasureData.Data.Length; i++)
				{
					if (treasureData[i] == entry) treasureData[i] = replacement;
				}
			}
		}
	}
}
