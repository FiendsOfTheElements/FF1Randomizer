﻿using FF1Lib.Helpers;
using FF1Lib.Sanity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class ExtSpoiler
	{
		private static HashSet<int> LegendaryShopIndices = new HashSet<int>(new int[] { 6, 16, 7, 17, 66 });

		private FF1Rom rom;
		private SanityCheckerV2 checker;
		private ShopData shopData;
		private ItemNames itemsText;
		private List<IRewardSource> itemPlacement;
		private OverworldMap overworldMap;
		private IncentiveData incentivesData;
		private GearPermissions weaponPermissions;
		private GearPermissions armorPermissions;
		private Flags flags;

		private SCLogic logic;
		private List<Weapon> weapons;
		private List<Armor> armors;
		private List<MagicSpell> magicSpells;

		public ExtSpoiler(FF1Rom _rom, SanityCheckerV2 _checker, ShopData _shopData, ItemNames _itemsText, List<IRewardSource> _itemPlacement, OverworldMap _overworldMap, IncentiveData _incentivesData, GearPermissions _weaponPermissions, GearPermissions _armorPermissions, Flags _flags)
		{
			rom = _rom;
			checker = _checker;
			shopData = _shopData;
			itemsText = _itemsText;
			itemPlacement = _itemPlacement;
			overworldMap = _overworldMap;
			incentivesData = _incentivesData;
			weaponPermissions = _weaponPermissions;
			armorPermissions = _armorPermissions;
			flags = _flags;

			logic = new SCLogic(rom, checker.Main, itemPlacement, flags, false);
			weapons = Weapon.LoadAllWeapons(rom, flags).ToList();
			armors = Armor.LoadAllArmors(rom, flags).ToList();
			magicSpells = rom.GetSpells();
		}

		public void WriteSpoiler()
		{
			WriteItemPlacementSpoiler();
			WriteWeaponSpoiler();
			WriteArmorSpoiler();
			WriteShopSpoiler();
			WriteV2Spoiler();
		}

		private void WriteWeaponSpoiler()
		{
			Utilities.WriteSpoilerLine("Weapon    Damage  Crit    Hit     Usability     Casting");
			Utilities.WriteSpoilerLine("----------------------------------------------------------------------------------------------------");

			foreach (var weapon in weapons)
			{
				var casting = "";
				if(weapon.SpellIndex != 0) casting = "casting: " + magicSpells[weapon.SpellIndex - 1].Name;

				Utilities.WriteSpoilerLine($"{weapon.Name.PadRight(8)}  +{weapon.Damage,2}      +{weapon.Crit,2}%    +{weapon.HitBonus,2}%    {rom.GenerateEquipPermission(weaponPermissions[weapon.Id]),12}  {casting}");
			}

			Utilities.WriteSpoilerLine("");
			Utilities.WriteSpoilerLine("");
		}

		private void WriteArmorSpoiler()
		{
			Utilities.WriteSpoilerLine("Armor     Type        Absorb  Weight  Usability     Casting         Resist");
			Utilities.WriteSpoilerLine("----------------------------------------------------------------------------------------------------");

			foreach (var armor in armors)
			{
				var casting = "";
				if (armor.SpellIndex != 0) casting = "casting: " + magicSpells[armor.SpellIndex - 1].Name;
				
				Utilities.WriteSpoilerLine($"{armor.Name.PadRight(8)}  {armor.Type.ToString().PadRight(10)}  +{armor.Absorb,2}     -{armor.Weight,3}%   {rom.GenerateEquipPermission(armorPermissions[armor.Id]),12}  {casting.PadRight(16)}{GetElementalResist((SpellElement)armor.ElementalResist)}");
			}

			Utilities.WriteSpoilerLine("");
			Utilities.WriteSpoilerLine("");
		}

		private string GetElementalResist(SpellElement elementalResist)
		{
			if (elementalResist == SpellElement.None) return "";
			if (elementalResist == SpellElement.All) return "All";

			var list = new List<SpellElement>();

			if (elementalResist.HasFlag(SpellElement.Earth)) list.Add(SpellElement.Earth);
			if (elementalResist.HasFlag(SpellElement.Lightning)) list.Add(SpellElement.Lightning);
			if (elementalResist.HasFlag(SpellElement.Ice)) list.Add(SpellElement.Ice);
			if (elementalResist.HasFlag(SpellElement.Fire)) list.Add(SpellElement.Fire);
			if (elementalResist.HasFlag(SpellElement.Death)) list.Add(SpellElement.Death);
			if (elementalResist.HasFlag(SpellElement.Time)) list.Add(SpellElement.Time);
			if (elementalResist.HasFlag(SpellElement.Poison)) list.Add(SpellElement.Poison);
			if (elementalResist.HasFlag(SpellElement.Status)) list.Add(SpellElement.Status);

			return string.Join(", ", list);
		}

		private void WriteItemPlacementSpoiler()
		{
			HashSet<Item> items = new HashSet<Item>(incentivesData.IncentiveItems.Concat(ItemLists.AllQuestItems).Concat(ItemLists.AllOrbs));

			var sorted = logic.RewardSources.Where(r => items.Contains(r.RewardSource.Item)).ToList();
			sorted.Sort((SCLogicRewardSource lhs, SCLogicRewardSource rhs) => lhs.RewardSource.Item.ToString().CompareTo(rhs.RewardSource.Item.ToString()));

			// Start of the item spoiler log output.
			Utilities.WriteSpoilerLine("Item        Entrance  ->  Floor  ->  Source                             Requirements");
			Utilities.WriteSpoilerLine("----------------------------------------------------------------------------------------------------");

			sorted.ForEach(source =>
			{
				var overworldLocation = source.RewardSource.MapLocation.ToString();

				if (overworldMap.OverriddenOverworldLocations != null && overworldMap.OverriddenOverworldLocations.TryGetValue(source.RewardSource.MapLocation, out var overriden))
				{
					overworldLocation = overriden.ToString();
				}

				var itemStr = source.RewardSource.Item.ToString().PadRight(12);
				var locStr = $"{overworldLocation} -> {source.RewardSource.MapLocation} -> {source.RewardSource.Name} ".PadRight(60);
				var reqs = BuildRequirements(source);

				Utilities.WriteSpoilerLine($"{itemStr}{locStr}{reqs}");
			});

			Utilities.WriteSpoilerLine("");
			Utilities.WriteSpoilerLine("");
		}

		private string BuildRequirements(SCLogicRewardSource r)
		{
			var rule = GetRule(r);

			var result = string.Join(" OR ", rule.Select(inner => "(" + string.Join(" AND ", inner) + ")"));

			return result;
		}

		private List<List<string>> GetRule(SCLogicRewardSource r)
		{
			return r.Requirements.Select(l => GetRule(l)).ToList();
		}

		private List<string> GetRule(SCRequirements l)
		{
			var list = new List<string>();

			if (l.HasFlag(SCRequirements.Lute)) list.Add(Item.Lute.ToString());
			if (l.HasFlag(SCRequirements.Crown)) list.Add(Item.Crown.ToString());
			if (l.HasFlag(SCRequirements.Key)) list.Add(Item.Key.ToString());
			if (l.HasFlag(SCRequirements.Ruby)) list.Add(Item.Ruby.ToString());
			if (l.HasFlag(SCRequirements.Rod)) list.Add(Item.Rod.ToString());
			if (l.HasFlag(SCRequirements.Chime)) list.Add(Item.Chime.ToString());
			if (l.HasFlag(SCRequirements.Cube)) list.Add(Item.Cube.ToString());
			if (l.HasFlag(SCRequirements.Oxyale)) list.Add(Item.Oxyale.ToString());
			if (l.HasFlag(SCRequirements.Tnt)) list.Add(Item.Tnt.ToString());
			if (l.HasFlag(SCRequirements.Canoe)) list.Add(Item.Canoe.ToString());
			if (l.HasFlag(SCRequirements.Floater)) list.Add(Item.Floater.ToString());
			if (l.HasFlag(SCRequirements.Bridge)) list.Add(Item.Bridge.ToString());
			if (l.HasFlag(SCRequirements.Canal)) list.Add(Item.Canal.ToString());
			if (l.HasFlag(SCRequirements.Bottle)) list.Add(Item.Bottle.ToString());
			if (l.HasFlag(SCRequirements.Crystal)) list.Add(Item.Crystal.ToString());
			if (l.HasFlag(SCRequirements.Herb)) list.Add(Item.Herb.ToString());
			if (l.HasFlag(SCRequirements.Adamant)) list.Add(Item.Adamant.ToString());
			if (l.HasFlag(SCRequirements.Slab)) list.Add(Item.Slab.ToString());
			if (l.HasFlag(SCRequirements.Ship)) list.Add(Item.Ship.ToString());

			return list;
		}

		private void WriteV2Spoiler()
		{
			Utilities.WriteSpoilerLine("V2 Spoiler");
			Utilities.WriteSpoilerLine("----------------------------------------------------------------------------------------------------");

			foreach (var d in checker.Main.Dungeons)
			{
				Utilities.WriteSpoilerLine(d.OverworldTeleport.ToString() + ":");

				var entranceArea = d.Areas.FirstOrDefault(a => a != null && a.IsRoot == true);

				if (entranceArea != null)
				{
					WriteV2SpoilerArea(entranceArea, "\t", 1);
				}
			}

			Utilities.WriteSpoilerLine("");
			Utilities.WriteSpoilerLine("");
		}

		private void WriteV2SpoilerArea(Sanity.SCArea a, string tab, int depth)
		{
			Utilities.WriteSpoilerLine(tab + a.Map.MapId.ToString());

			//if (a.Map.MapId == MapId.TempleOfFiends && depth < 15) throw new RerollException();

			foreach (var p in a.PointsOfInterest)
			{
				switch (p.Type)
				{
					case Sanity.SCPointOfInterestType.Orb:
						Utilities.WriteSpoilerLine(tab + " - " + a.Map.MapId.ToString() + " - " + p.Type.ToString() + " - " + p.BitFlagSet);
						break;
					case Sanity.SCPointOfInterestType.Shop:
						var shop = shopData.Shops.First(x => x.Index == p.ShopId - 1);
						Utilities.WriteSpoilerLine(tab + " - " + a.Map.MapId.ToString() + " - " + p.Type.ToString() + " - " + shop.Type + "." + shop.Location + "." + p.ShopId.ToString() + " - " + p.BitFlagSet);
						break;
					case Sanity.SCPointOfInterestType.Treasure:
						var item = (Item)rom.Get(0x3100 + p.TreasureId, 1)[0];
						Utilities.WriteSpoilerLine(tab + " - " + a.Map.MapId.ToString() + " - " + p.Type.ToString() + " - " + GetItemName(item) + " - " + p.BitFlagSet);
						break;
				}
			}

			foreach (var a2 in a.ChildAreas)
			{
				WriteV2SpoilerArea(a2, tab + "\t", depth + 1);
			}
		}

		private string GetItemName(Item item)
		{
			switch (item)
			{
				case Item.Ship:
				case Item.Bridge:
				case Item.Canal:
				case Item.Canoe:
					return item.ToString();
				default:
					return itemsText[(int)item];
			}
		}

		private void WriteShopSpoiler()
		{

			Dictionary<string, MagicSpell> Spells = magicSpells.ToDictionary(s => s.Name.ToLowerInvariant());
			var spls = shopData.Shops.Where(s => s.Type == FF1Lib.ShopType.Black || s.Type == FF1Lib.ShopType.White).SelectMany(s => s.Entries).Distinct().ToList();

			HashSet<int> allspells = new HashSet<int>(spls.Select(s => (int)s));

			Utilities.WriteSpoilerLine("Type      Location        Level   Item");
			Utilities.WriteSpoilerLine("----------------------------------------------------------------------------------------------------");


			foreach (var s in shopData.Shops)
			{
				if (s.Type == ShopType.Inn || s.Type == ShopType.Clinic) continue;
				foreach (var e in s.Entries)
				{
					var location = s.Location.ToString();
					if (LegendaryShopIndices.Contains(s.Index)) location = "Legendary";

					if (s.Type != ShopType.Black && s.Type != ShopType.White)
					{
						Utilities.WriteSpoilerLine($"{s.Type.ToString().PadRight(10)}{location.ToString().PadRight(16)}-       {GetItemName(e)}");
					}
					else
					{
						var spell = Spells.FirstOrDefault(s => Convert.ToByte((s.Value.Index + FF1Rom.MagicNamesIndexInItemText)) == Convert.ToByte(e));

						var name = spell.Key;
						var level = spell.Value.Index / 8 + 1;

						Utilities.WriteSpoilerLine($"{s.Type.ToString().PadRight(10)}{location.ToString().PadRight(16)}{level.ToString().PadRight(8)}{name}");
					}
				}
			}

			Utilities.WriteSpoilerLine("");
			Utilities.WriteSpoilerLine("");
		}
	}
}
