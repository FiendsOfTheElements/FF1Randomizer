using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public class LegendaryShops
	{
		private MT19337 rng;
		private readonly Flags flags;
		private readonly FF1Rom rom;
		private readonly List<Map> maps;
		private readonly MapTileSets MapTileSets;
		private readonly ShopData ShopData;
		private readonly TileSet[] TileSets = new TileSet[8];
		private Dictionary<byte, List<byte>> UnusedTilesbyTileSet;
		private Dictionary<string, MagicSpell> Spells;
		private readonly List<SpellInfo> SpellInfos;

		//MapId, X, Y, UL Tile, UR Tile, BL Tile, BR Tile, Pallette
		private readonly List<(MapId, int, int, byte, byte, byte, byte, byte)> Locations = new()
		{
			(MapId.EarthCaveB2, 20, 34, 0x22, 0x23, 0x32, 0x33, 0xFF),
			(MapId.MarshCaveB2, 13, 44, 0x22, 0x23, 0x32, 0x33, 0xAA),
			(MapId.DwarfCave, 12, 50, 0x22, 0x23, 0x32, 0x33, 0xFF),
			(MapId.MirageTower1F, 23, 02, 0x22, 0x23, 0x32, 0x33, 0xAA),
			(MapId.IceCaveB1, 25, 24, 0x22, 0x23, 0x32, 0x33, 0xFF),
			(MapId.SeaShrineB4, 27, 40, 0x22, 0x23, 0x32, 0x33, 0xAA)
		};

		public LegendaryShops(MT19337 _rng, Flags _flags, List<Map> _maps, FF1Rom _rom)
		{
			rng = _rng;
			flags = _flags;
			rom = _rom;
			maps = _maps;

			MapTileSets = new MapTileSets(rom);
			ShopData = new ShopData(rom);
			SpellInfos = rom.LoadSpells().ToList();
		}

		private void LoadUnusedTileIds()
		{
			byte[] possibleTileIds = new byte[128];
			for (byte i = 0; i < 128; i++)
			{
				possibleTileIds[i] = i;
			}

			UnusedTilesbyTileSet = Enum.GetValues<MapId>()
				.GroupBy(m => MapTileSets[m])
				.Select(t => (t.Key, t.Select(m => maps[(int)m]
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

			Spells = rom.GetSpells().ToDictionary(s => FF1Text.BytesToText(s.Name));
			ShopData.LoadData();
			MapTileSets.LoadTable();

			for (int i = 0; i < 8; i++)
			{
				TileSets[i] = new TileSet(rom, (byte)i);
				TileSets[i].LoadData();
			}

			LoadUnusedTileIds();

			int[] allocatedslots = AllocateSlots();
			List<(MapId, int, int, byte, byte, byte, byte, byte)> pool = Locations.Take(allocatedslots.Where(s => s > 0).Count() + 1).ToList();
			pool.Shuffle(rng);

			CreateWeaponShop(allocatedslots[0], pool);
			CreateArmorShop(allocatedslots[1], pool);
			CreateBlackShop(allocatedslots[2], pool);
			CreateWhiteShop(allocatedslots[3], pool);
			CreateItemShop(allocatedslots[4], pool);

			ShopData.StoreData();

			for (int i = 0; i < 8; i++)
			{
				TileSets[i].StoreData();
			}

			rom.PutInBank(0x1F, 0xEBBD, Blob.FromHex("03"));//Set ShopType
			rom.PutInBank(0x1F, 0xEBC7, Blob.FromHex("02"));//Set ShopType
		}

		private void PrepareMaps()
		{
			maps[(int)MapId.DwarfCave][51, 12] = maps[(int)MapId.DwarfCave][51, 13];
		}

		private void CreateWeaponShop(int slots, List<(MapId, int, int, byte, byte, byte, byte, byte)> pool)
		{
			if (slots <= 0)
			{
				return;
			}

			Shop shop = new(6, ShopType.Weapon, MapLocation.Coneria, MapId.Coneria, 0, string.Empty, GetWeaponShopInventory(slots));

			if (shop.Entries.Count > 0)
			{
				ShopData.Shops.Add(shop);
				PlaceShop(shop.Index, pool);
			}
		}

		private void CreateArmorShop(int slots, List<(MapId, int, int, byte, byte, byte, byte, byte)> pool)
		{
			if (slots <= 0)
			{
				return;
			}

			Shop shop = new(16, ShopType.Armor, MapLocation.Coneria, MapId.Coneria, 0, string.Empty, GetArmorShopInventory(slots));

			if (shop.Entries.Count > 0)
			{
				ShopData.Shops.Add(shop);
				PlaceShop(shop.Index, pool);
			}
		}

		private void CreateBlackShop(int slots, List<(MapId, int, int, byte, byte, byte, byte, byte)> pool)
		{
			if (slots <= 0)
			{
				return;
			}

			Shop shop = new(7, ShopType.Black, MapLocation.Coneria, MapId.Coneria, 0, string.Empty, GetBlackShopInventory(slots));

			if (shop.Entries.Count > 0)
			{
				ShopData.Shops.Add(shop);
				PlaceShop(shop.Index, pool);
			}
		}

		private void CreateWhiteShop(int slots, List<(MapId, int, int, byte, byte, byte, byte, byte)> pool)
		{
			if (slots <= 0)
			{
				return;
			}

			Shop shop = new(17, ShopType.White, MapLocation.Coneria, MapId.Coneria, 0, string.Empty, GetWhiteShopInventory(slots));

			if (shop.Entries.Count > 0)
			{
				ShopData.Shops.Add(shop);
				PlaceShop(shop.Index, pool);
			}
		}

		private void CreateItemShop(int slots, List<(MapId, int, int, byte, byte, byte, byte, byte)> pool)
		{
			if (slots <= 0)
			{
				return;
			}

			Shop shop = new(66, ShopType.Item, MapLocation.Coneria, MapId.Coneria, 0, string.Empty, GetItemShopInventory(slots));

			if (shop.Entries.Count > 0)
			{
				ShopData.Shops.Add(shop);
				PlaceShop(shop.Index, pool);
			}
		}

		private void PlaceShop(int index, List<(MapId, int, int, byte, byte, byte, byte, byte)> pool)
		{
			var locidx = rng.Between(0, pool.Count - 1);
			(MapId, int, int, byte, byte, byte, byte, byte) loc = pool[locidx];
			pool.RemoveAt(locidx);

			byte tile = CreateTile(loc.Item1, index + 1, loc.Item4, loc.Item5, loc.Item6, loc.Item7, loc.Item8);

			maps[(int)loc.Item1][loc.Item3, loc.Item2] = tile;
		}

		private byte CreateTile(MapId mapId, int ShopId, byte ul, byte ur, byte bl, byte br, byte pi)
		{
			TileSet tileSet = TileSets[MapTileSets[mapId]];
			byte tile = UnusedTilesbyTileSet[MapTileSets[mapId]][0];
			UnusedTilesbyTileSet[MapTileSets[mapId]].RemoveAt(0);

			tileSet.TileAttributes[tile] = pi;
			tileSet.TileProperties[tile] = new TileProp(3, (byte)ShopId);
			tileSet.TopLeftTiles[tile] = ul;
			tileSet.TopRightTiles[tile] = ur;
			tileSet.BottemLeftTiles[tile] = bl;
			tileSet.BottemRightTiles[tile] = br;

			return tile;
		}

		private int[] AllocateSlots()
		{
			int slots = ShopData.StoreData();

			int[] result = new int[] { 0, 0, 0, 0, 0 };

			if (flags.LegendaryBlackShop ?? false)
			{
				result[2] = Allocate(ref slots, 5);//Black
			}

			if (flags.LegendaryArmorShop ?? false)
			{
				result[1] = Allocate(ref slots, 5);//Armor
			}

			if (flags.LegendaryWhiteShop ?? false)
			{
				result[3] = Allocate(ref slots, 4);//White
			}

			if (flags.LegendaryWeaponShop ?? false)
			{
				result[0] = Allocate(ref slots, 3);//Weapon
			}

			if (flags.LegendaryItemShop ?? false)
			{
				result[4] = Allocate(ref slots, 3);//Item
			}

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
			List<Item> items = new() { Item.Cabin, Item.House, Item.Heal, Item.Pure, Item.Soft };

			List<Item> result = new();
			if (slots > 0)
			{
				Item save = (Item)rng.Between((int)Item.Cabin, (int)Item.House);
				result.Add(save);
				items.Remove(save);
				slots--;
			}

			items.Shuffle(rng);
			result.AddRange(items.Take(slots));

			return result;
		}

		private List<Item> GetWeaponShopInventory(int slots)
		{
			List<Item> items = new() { Item.Vorpal, Item.Katana, Item.Xcalber, Item.Defense, Item.FlameSword, Item.IceSword, Item.CoralSword, Item.SunSword, Item.BaneSword, Item.CatClaw, Item.ThorHammer, Item.WizardRod, Item.MageRod, Item.HealRod, Item.LightAxe };

			List<Item> result = new();

			items.Shuffle(rng);
			result.AddRange(items.Take(slots));

			return result;
		}

		private List<Item> GetArmorShopInventory(int slots)
		{
			List<Item> items = ItemLists.LegendaryArmorTier.Concat(ItemLists.RareArmorTier).Distinct().ToList();

			List<Item> result = new();

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
				List<Spell> spells = new() { Spell.RUSE, Spell.INVS, Spell.PURE, Spell.CUR3, Spell.LIFE, Spell.HRM3, Spell.SOFT, Spell.EXIT, Spell.INV2, Spell.CUR4, Spell.HRM4, Spell.HEL3, Spell.LIF2, Spell.FADE, Spell.WALL };
				List<Item> items = spells.Where(s => Spells.ContainsKey(s.ToString())).Select(s => Convert.ToByte(Spells[s.ToString()].Index + 176)).Cast<Item>().ToList();

				List<Item> result = new();

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
				List<Spell> spells = new() { Spell.LOCK, Spell.TMPR, Spell.FIR2, Spell.LIT2, Spell.LOK2, Spell.FAST, Spell.ICE2, Spell.FIR3, Spell.BANE, Spell.WARP, Spell.LIT3, Spell.QAKE, Spell.ICE3, Spell.BRAK, Spell.SABR, Spell.NUKE, Spell.ZAP, Spell.XXXX };
				List<Item> items = spells.Where(s => Spells.ContainsKey(s.ToString())).Select(s => Convert.ToByte(Spells[s.ToString()].Index + 176)).Cast<Item>().ToList();

				List<Item> result = new();

				items.Shuffle(rng);
				result.AddRange(items.Take(slots));

				return result;
			}
		}

		private List<Item> GetCraftedSpellInventory(int slots, bool black)
		{
			IEnumerable<SpellInfo> stDamSpells = SpellInfos.Where(s => s.routine == 0x01 && s.targeting == 0x01).Where(s => s.tier >= 3).OrderBy(s => -s.tier).Take(3);
			IEnumerable<SpellInfo> aoeDamSpells = SpellInfos.Where(s => s.routine == 0x01 && s.targeting != 0x01).OrderBy(s => -s.tier).Take(6);

			IEnumerable<SpellInfo> stHarmSpells = SpellInfos.Where(s => s.routine == 0x02 && s.targeting == 0x01).OrderBy(s => -s.tier).Take(1);
			IEnumerable<SpellInfo> aoeHarmSpells = SpellInfos.Where(s => s.routine == 0x02 && s.targeting != 0x01).OrderBy(s => -s.tier).Take(1);

			IEnumerable<SpellInfo> stHealSpells = SpellInfos.Where(s => (s.routine == 0x07 || s.routine == 0x0F) && s.targeting != 0x08).OrderBy(s => -s.tier).Take(2);
			IEnumerable<SpellInfo> aoeHealSpells = SpellInfos.Where(s => (s.routine == 0x07 || s.routine == 0x0F) && s.targeting != 0x08).OrderBy(s => -s.tier).Take(2);

			IEnumerable<SpellInfo> WallSpells = SpellInfos.Where(s => s.routine == 0x0A && s.effect == 0xFF).Take(1);
			IEnumerable<SpellInfo> FastSpells = SpellInfos.Where(s => s.routine == 0x0C).OrderBy(s => -s.tier).Take(2);
			IEnumerable<SpellInfo> SabrSpells = SpellInfos.Where(s => s.routine == 0x0D).OrderBy(s => -s.tier).Take(2);
			IEnumerable<SpellInfo> LockSpells = SpellInfos.Where(s => s.routine == 0x0E).OrderBy(s => -s.tier).Take(2);
			IEnumerable<SpellInfo> RuseSpells = SpellInfos.Where(s => s.routine == 0x10).OrderBy(s => -s.tier).Take(2);
			IEnumerable<SpellInfo> WordSpells = SpellInfos.Where(s => s.routine == 0x12 && s.effect == 0b00000001).Take(1);
			IEnumerable<SpellInfo> WordSpells2 = SpellInfos.Where(s => s.routine == 0x03 && (s.effect == 0b00000010 || s.effect == 0b00000001)).Take(3);

			IEnumerable<byte> spells = stDamSpells
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


			IEnumerable<byte> specialSpells = Spells.Where(s => s.Key.StartsWith("LIF"))
			.Concat(Spells.Where(s => s.Key.StartsWith("WARP")))
			.Concat(Spells.Where(s => s.Key.StartsWith("WRP")))
			.Concat(Spells.Where(s => s.Key.StartsWith("EXIT")))
			.Concat(Spells.Where(s => s.Key.StartsWith("EXT")))
			.Concat(Spells.Where(s => s.Key.StartsWith("SOFT")))
			.Concat(Spells.Where(s => s.Key.StartsWith("SFT")))
			.Concat(Spells.Where(s => s.Key.StartsWith("PURE")))
			.Concat(Spells.Where(s => s.Key.StartsWith("PUR")))
			.Select(s => Convert.ToByte(s.Value.Index));


			List<byte> items = spells
				.Concat(specialSpells)
				.Where(s => BlackSpell(s) ^ !black)
				.ToList();

			List<string> spellnames = Spells.Where(s => items.Contains(s.Value.Index)).Select(s => s.Key).ToList();

			List<Item> result = new();

			items.Shuffle(rng);
			result.AddRange(items.Take(slots).Select(i => (Item)Convert.ToByte(i + 0xB0)));

			return result;
		}

		private bool BlackSpell(int id)
		{
			return id % 8 > 3;
		}
	}
}
