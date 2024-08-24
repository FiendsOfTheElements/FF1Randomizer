using FF1Lib.Sanity;

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
		private PlacementContext incentivesData;
		private Overworld overworld;
		private GearPermissions weaponPermissions;
		private GearPermissions armorPermissions;
		private Flags flags;

		private SCLogic logic;
		private List<Weapon> weapons;
		private List<Armor> armors;
		private List<MagicSpell> magicSpells;

		public ExtSpoiler(FF1Rom _rom, SanityCheckerV2 _checker, ShopData _shopData, ItemNames _itemsText, List<IRewardSource> _itemPlacement, Overworld _overworld, PlacementContext _incentivesData, GearPermissions _weaponPermissions, GearPermissions _armorPermissions, Flags _flags)
		{
			rom = _rom;
			checker = _checker;
			shopData = _shopData;
			itemsText = _itemsText;
			itemPlacement = _itemPlacement;
			overworld = _overworld;
			overworldMap = _overworld.OverworldMap;
			incentivesData = _incentivesData;
			weaponPermissions = _weaponPermissions;
			armorPermissions = _armorPermissions;
			flags = _flags;

			logic = new SCLogic(rom, checker.Main, itemPlacement, overworld.Locations, flags, false);
			weapons = Weapon.LoadAllWeapons(rom, flags).ToList();
			armors = Armor.LoadAllArmors(rom, flags).ToList();
			magicSpells = rom.GetSpells();
		}

		public void WriteSpoiler()
		{
			WriteItemPlacementSpoiler();
			WriteClassSpoiler();
			WriteWeaponSpoiler();
			WriteArmorSpoiler();
			WriteSpellSpoiler();
			WriteShopSpoiler();
			if (flags.GameMode != GameModes.DeepDungeon)
			{
				WriteV2Spoiler();
			}
		}

		private void WriteWeaponSpoiler()
		{
			Utilities.WriteSpoilerLine("Weapon    Damage  Crit    Hit     Usability           Casting");
			Utilities.WriteSpoilerLine("----------------------------------------------------------------------------------------------------");

			foreach (var weapon in weapons)
			{
				var casting = "";
				if(weapon.SpellIndex != 0) casting = "casting: " + magicSpells[weapon.SpellIndex - 1].Name;

				Utilities.WriteSpoilerLine($"{weapon.Name.PadRight(8)}  +{weapon.Damage,2}      +{weapon.Crit,2}%    +{weapon.HitBonus,2}%    {GenerateSpoilerEquipPermission(weaponPermissions[weapon.Id]),17}  {casting}");
			}

			Utilities.WriteSpoilerLine("");
			Utilities.WriteSpoilerLine("");
		}

		private void WriteArmorSpoiler()
		{
			Utilities.WriteSpoilerLine("Armor     Type        Absorb  Weight  Usability          Casting         Resist");
			Utilities.WriteSpoilerLine("----------------------------------------------------------------------------------------------------");

			foreach (var armor in armors)
			{
				var casting = "";
				if (armor.SpellIndex != 0) casting = "casting: " + magicSpells[armor.SpellIndex - 1].Name;
				
				Utilities.WriteSpoilerLine($"{armor.Name.PadRight(8)}  {armor.Type.ToString().PadRight(10)}  +{armor.Absorb,2}     -{armor.Weight,3}%   {GenerateSpoilerEquipPermission(armorPermissions[armor.Id]),17}  {casting.PadRight(16)}{GetElementalResist((SpellElement)armor.ElementalResist)}");
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

		private void WriteSpellSpoiler()
		{
			Utilities.WriteSpoilerLine("Lv   Spell  Target  Acc   Usability          Routine       OOB  Element");
			Utilities.WriteSpoilerLine("----------------------------------------------------------------------------------------------------");

			foreach (var spell in magicSpells)
			{
				Utilities.WriteSpoilerLine($"{spell.Level}.{spell.Slot}  {spell.Name.PadRight(6)}  {spell.targeting.ToString().Substring(0,4).PadRight(7)} {spell.accuracy.ToString().PadRight(3)} {GenerateSpellPermission(rom.SpellPermissions.PermissionsFor((SpellSlots)spell.Index)),17}  {spell.routine.ToString().PadRight(14)} {spell.oobSpellRoutine.ToString().PadRight(5)} {GetElementalResist((SpellElement)spell.elem)}");
			}

			Utilities.WriteSpoilerLine("");
			Utilities.WriteSpoilerLine("");
		}

		public string GenerateSpellPermission(List<Classes> usableClasses)
		{
			var description = "";
			for (int i = 0; i < 6; i++)
			{
				if (usableClasses.Contains((Classes)i))
					description += " " + rom.InfoClassAbbrev[i * 2];
				else if (usableClasses.Contains((Classes)i+6))
					description += " " + rom.InfoClassAbbrev[i * 2 + 1];
				else
					description += "   ";
			}
			return description;
		}

		public string GenerateSpoilerEquipPermission(int classUsability)
		{
			var description = "";
			for (int i = 0; i < 6; i++)
			{
				if ((classUsability & rom.InfoClassEquipPerms[i * 2]) != 0)
					description += " " + rom.InfoClassAbbrev[i * 2];
				else if ((classUsability & rom.InfoClassEquipPerms[i * 2 + 1]) != 0)
					description += " " + rom.InfoClassAbbrev[i * 2 + 1];
				else
					description += "   ";
			}
			return description;
		}

		private void WriteClassSpoiler()
		{
			Utilities.WriteSpoilerLine("Name        HP   Str  Agi  Int  Vit  Lck  Mdef Hit    Promotes to");
			Utilities.WriteSpoilerLine("----------------------------------------------------------------------------------------------------");

			for (int i = 0; i < 6; i++) {
				ClassData c = rom.ClassData[(Classes)i];
				ClassData p = rom.ClassData[(Classes)i+6];

				int VitForHealthUp = (c.VitGrowth.Where(x => x).Count() + c.VitStarting + 49 - c.VitGrowth.Where(x => x).Count() + c.VitStarting) / 2; // (Lv50 Vit + Lv 1 Vit) / 2 -> Average Vit


				Utilities.WriteSpoilerLine($"{rom.ItemsText[0xF0 + i], -12}{c.HpGrowth.Where(x=>x).Count() * 20 + c.HpStarting + 49 * (VitForHealthUp / 4), 4} {c.StrGrowth.Where(x => x).Count() + c.StrStarting + (49-c.StrGrowth.Where(x => x).Count())/4,3}  " +
					$"{c.AgiGrowth.Where(x => x).Count() + c.AgiStarting+ (49 - c.AgiGrowth.Where(x => x).Count()) / 4,3}  {c.IntGrowth.Where(x => x).Count() + c.IntStarting+ (49 - c.IntGrowth.Where(x => x).Count()) / 4,3}  {c.VitGrowth.Where(x => x).Count() + c.VitStarting+ (49 - c.VitGrowth.Where(x => x).Count()) / 4,3}  {c.LckGrowth.Where(x => x).Count() + c.LckStarting+ (49 - c.LckGrowth.Where(x => x).Count()) / 4,3}  " +
					$"{c.MDefGrowth * 49 + c.MDefStarting,3}  {c.HitGrowth * 49 + c.HitStarting,3}   {rom.ItemsText[0xF0 + i+6],-14}");
			}

			Utilities.WriteSpoilerLine("");
			Utilities.WriteSpoilerLine("");
		}

		private void WriteItemPlacementSpoiler()
		{
			HashSet<Item> items = new HashSet<Item>(incentivesData.IncentiveItems.Concat(ItemLists.AllQuestItems).Concat(ItemLists.AllOrbs));
			if (flags.GuaranteedDefenseItem != GuaranteedDefenseItem.None && !(flags.ItemMagicMode == ItemMagicMode.None)) items.Add(Item.PowerRod);
			if (flags.GuaranteedPowerItem != GuaranteedPowerItem.None && !(flags.ItemMagicMode == ItemMagicMode.None)) items.Add(Item.PowerGauntlets);

			var sorted = logic.RewardSources.Where(r => items.Contains(r.RewardSource.Item)).ToList();
			sorted.Sort((SCLogicRewardSource lhs, SCLogicRewardSource rhs) => lhs.RewardSource.Item.ToString().CompareTo(rhs.RewardSource.Item.ToString()));

			// Start of the item spoiler log output.
			Utilities.WriteSpoilerLine("Item        Entrance  ->  Floor  ->  Source                             Requirements");
			Utilities.WriteSpoilerLine("----------------------------------------------------------------------------------------------------");

			sorted.ForEach(source =>
			{

				var overworldLocation = source.RewardSource.Entrance.ToString();
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
			Utilities.WriteSpoilerLine(tab + a.Map.MapIndex.ToString());

			//if (a.Map.MapIndex == MapIndex.TempleOfFiends && depth < 15) throw new RerollException();

			foreach (var p in a.PointsOfInterest)
			{
				switch (p.Type)
				{
					case Sanity.SCPointOfInterestType.Orb:
						Utilities.WriteSpoilerLine(tab + " - " + a.Map.MapIndex.ToString() + " - " + p.Type.ToString() + " - " + p.BitFlagSet);
						break;
					case Sanity.SCPointOfInterestType.Shop:
						var shop = shopData.Shops.First(x => x.Index == p.ShopId - 1);
						Utilities.WriteSpoilerLine(tab + " - " + a.Map.MapIndex.ToString() + " - " + p.Type.ToString() + " - " + shop.Type + "." + shop.Location + "." + p.ShopId.ToString() + " - " + p.BitFlagSet);
						break;
					case Sanity.SCPointOfInterestType.Treasure:
						if (logic.RewardSources.TryFind(r => r.RewardSource.Address - 0x3100 == p.TreasureId, out var location))
						{
							Utilities.WriteSpoilerLine(tab + " - " + a.Map.MapIndex.ToString() + " - " + p.Type.ToString() + " - " + GetItemName(location.RewardSource.Item) + " - " + p.BitFlagSet);
						}
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
