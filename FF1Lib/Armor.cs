using FF1Lib.Helpers;

namespace FF1Lib
{
	enum ArmorIcon : byte
	{
		NONE = 0x00,
		ARMOR = 0xDA,
		SHIELD = 0xDB,
		HELM = 0xDC,
		GAUNTLET = 0xDD,
		BRACELET = 0xDE,
		SHIRT = 0xDF
	}

	enum ArmorType : byte
	{
		ARMOR = 0x00,
		SHIELD = 0x01,
		HELM = 0x02,
		GAUNTLET = 0x03
	}

	public partial class FF1Rom : NesRom
	{
		public const int ArmorPermissionsOffset = 0x3BFA0;
		public const int ArmorTypeOffset        = 0x3BCD1;
		public const int ArmorPermissionsCount = 40;

		public void RandomArmorBonus(Flags flags, bool cleanNames, MT19337 rng)
		{
			bool enable = (bool)flags.RandomArmorBonus;
			int min = flags.RandomArmorBonusLow;
			int max = flags.RandomArmorBonusHigh;

			if (!enable)
			{
				return;
			}

			//get base stats
			Armor currentArmor;
			for (int i = 0; i < ArmorCount; i++)
			{
				currentArmor = new Armor(i, this);
				int bonus = rng.Between(min, max);
				if (bonus != 0)
				{
					//body armor(indexes 0 - 15) +2/-2 +2/-2 weight
					//shield(indexes 16 - 24) +1/-1 +1/-1 weight
					//helm(indexes 25 - 31) +1/-1 +1/-1 weight
					//hand(indexes 32 - 39) +1/-1 +1/-1 weight
					int armorTypeAbsorbBonus = i < 16 ? 2 : 1;
					int armorTypeWeightBonus = i < 16 ? 2 : 1;

					//adjust stats
					//clamp minimums to 0 weight, and 1 absorb
					currentArmor.Weight = (byte)Math.Max(0, currentArmor.Weight - (armorTypeWeightBonus * bonus));
					currentArmor.Absorb = (byte)Math.Max(1, currentArmor.Absorb + (armorTypeAbsorbBonus * bonus));

					//change last two non icon characters to -/+bonus
					string bonusString = string.Format((bonus > 0) ? "+{0}" : "-{0}", Math.Abs(bonus).ToString());
					byte[] bonusBytes = FF1Text.TextToBytes(bonusString);

					// Adjusts blursed armor names to be more understandable
					if (cleanNames) {
						if (currentArmor.Name[0..6] == "Copper") { currentArmor.Name = "Copr" + currentArmor.Name.Substring(4); }
						if (currentArmor.Name[0..5] == "Steel")  { currentArmor.Name = "Stl " + currentArmor.Name.Substring(4); }
						if (currentArmor.Name[0..6] == "Silver") { currentArmor.Name = "Slvr" + currentArmor.Name.Substring(4); }
						if (currentArmor.Name[0..6] == "Burlap") { currentArmor.Name = "Brlp" + currentArmor.Name.Substring(4); }
						if (currentArmor.Name[0..6] == "Leathr") { currentArmor.Name = "Lthr" + currentArmor.Name.Substring(4); }
						if (currentArmor.Name[0..6] == "Emerld") { currentArmor.Name = "Emrl" + currentArmor.Name.Substring(4); }

						if (currentArmor.Name[0..5] == "Earth")  { currentArmor.Name = "Erth" + currentArmor.Name.Substring(4); }
						if (currentArmor.Name[0..6] == "Active") { currentArmor.Name = "Actv" + currentArmor.Name.Substring(4); }
						if (currentArmor.Name[0..5] == "Power")  { currentArmor.Name = "Powr" + currentArmor.Name.Substring(4); }

						if (currentArmor.Name[0..6] == "Ribbon") {
							// Don't adjust vanilla Ribbon with no icon; only Ribbons with icons or C/R from Armor Crafter
							if (!currentArmor.Name.EndsWith(" ")) { currentArmor.Name = "Ribn" + currentArmor.Name.Substring(4); }
						}

						// No-icon "cape" and "ring" equipment needs to keep the "C" and "R" so it's clear where it's equipped
						if (currentArmor.Name.EndsWith("C"))
						{
							currentArmor.Name = currentArmor.Name.Substring(0, 4) + "C  ";
						}
						else if (currentArmor.Name.EndsWith("R"))
						{
							currentArmor.Name = currentArmor.Name.Substring(0, 4) + "R  ";
						}
					}


					var nameBytes = FF1Text.TextToBytes(currentArmor.Name, false);

					int iconIndex = nameBytes[6] > 200 && nameBytes[6] != 255 ? 5 : 6;
					for (int j = 0; j < bonusBytes.Length - 1; j++)
					{
						nameBytes[iconIndex - j] = bonusBytes[bonusBytes.Length - 2 - j];
					}

					currentArmor.Name = FF1Text.BytesToText(nameBytes);
					currentArmor.writeArmorMemory(this);
				}
			}
		}

		//sample function for creating new armor
		public void ExpandArmor()
		{
		    Armor platinumBracelet = new Armor(12, "Plat@B", ArmorIcon.BRACELET, 1, 42, 0, 0, ArmorType.ARMOR);
			ArmorPermissions[platinumBracelet.Id] = (ushort)(
				EquipPermission.BlackBelt |
				EquipPermission.BlackMage |
				EquipPermission.BlackWizard |
				EquipPermission.Fighter |
				EquipPermission.Knight |
				EquipPermission.Master |
				EquipPermission.Ninja |
				EquipPermission.RedMage |
				EquipPermission.RedWizard |
				EquipPermission.Thief |
				EquipPermission.WhiteMage |
				EquipPermission.WhiteWizard);
			platinumBracelet.writeArmorMemory(this);
		}

		public void ArmorCrafter(Flags flags, MT19337 rng) {

			bool enableArmocrafter = (bool)flags.ArmorCrafter;
			bool noItemMagic = flags.ItemMagicMode == ItemMagicMode.None;
			bool noResists = flags.RibbonMode == RibbonMode.Split;

			if (!enableArmocrafter)
			{
				return;
			}

			var commonArmor = new List<Item>(ItemLists.CommonArmorTier);
		    var rareArmor = new List<Item>(ItemLists.RareArmorTier);
		    var legendaryArmor = new List<Item>(ItemLists.LegendaryArmorTier);

		    commonArmor.Shuffle(rng);
		    rareArmor.Shuffle(rng);
		    legendaryArmor.Shuffle(rng);

		    var tierList = new List<List<Item>> { commonArmor, rareArmor, legendaryArmor };

		    // armor classes, determines who can equip
		    // cloth -- everyone
		    // light -- fi/th/bb/rm
		    // medium -- fi/ni/rm
		    // heavy -- fi/ni
		    // legendary (knight, mage stuff, ribbon, 1 more?)

		    const int CLOTH = 0;
		    const int LIGHT = 1;
		    const int MEDIUM = 2;
		    const int HEAVY = 3;
		    const int KNIGHT = 4;

		    // Armor types.
		    const int ARMOR = 0;
		    const int SHIELD = 1;
		    const int HELM = 2;
		    const int GAUNTLET = 3;
		    const int BRACELET = 4;
		    const int SHIRT = 5;
		    const int CAPE = 6;
		    const int RING = 7;

		    // class x type -> defense base
		    var armorDefenseBase = new int[,] {
			// armor, shield, helm, gauntlet, bracelet, shirt, cape, ring
			{  4,       2,    1,        1,        4,     6,    2,    2 },  // cloth
			{ 10,       4,    3,        2,        8,    12,    4,    4 },  // light
			{ 20,       8,    6,        4,       16,    16,    6,    6 },  // medium
			{ 30,      12,    6,        6,       24,    24,    8,    8 },  // heavy
			{ 42,      16,    8,        8,       32,    24,   10,   10 },  // knight
		    };

		    // class x type -> evade penalty base
		    var armorEvadePenaltyBase = new int[,] {
			// armor, shield, helm, gauntlet, bracelet, shirt, cape, ring
			{ 4,       1,    1,        1,        1,     2,    2,    1 },  // cloth
			{ 8,       8,    3,        3,        1,     2,    2,    1 },  // light
			{ 16,      8,    3,        3,        1,     2,    2,    1 },  // medium
			{ 30,     10,    6,        3,        1,     2,    2,    1 },  // heavy
			{ 20,     10,    3,        3,        1,     2,    2,    1 },  // knight
		    };

		    var resists = new byte[] {
			(byte)SpellElement.Status,
			(byte)SpellElement.Poison,
			(byte)SpellElement.Time,
			(byte)SpellElement.Death,
			(byte)SpellElement.Fire,
			(byte)SpellElement.Ice,
			(byte)SpellElement.Lightning,
			(byte)SpellElement.Earth,
		    };
		    var resistNames = new Dictionary<int, string> {
			{ (int)SpellElement.Status, "Active" },    // resist status attacks
			{ (int)SpellElement.Poison, "Aegis" },     // resist stone/poison
			{ (int)SpellElement.Time, "Time" },        // resis time
			{ (int)SpellElement.Death, "Protec" },     // resist death
			{ (int)SpellElement.Fire, "Ice" },         // resist fire
			{ (int)SpellElement.Ice, "Flame" },        // resist ice
			{ (int)SpellElement.Lightning, "Ohm" },    // resist lightning
			{ (int)SpellElement.Earth, "Earth" },      // resist earth
		    };
		    var classNames = new string[][] {
			new string[] { "Velvet", "Silk",   "Burlap" },
			new string[] { "Leathr", "Copper", "Bronze" },
			new string[] { "Silver", "Chain",  "Mithrl"  },
			new string[] { "Iron",   "Steel",  "Gold"   },
			new string[] { "Opal",   "Dragon", "Diamnd" },
		    };
		    var ringNames = new string[][] {
			new string[] { "Brass",  "Tin" },
			new string[] { "Copper", "Bronze" },
			new string[] { "Silver", "Mithrl"  },
			new string[] { "Gold",   "Emerld" },
			new string[] { "Opal",   "Diamnd" },
		    };

		    var spellHelper = new SpellHelper(this);
		    // Someone should really combine these
		    var allSpells = GetSpells();

		    // cast FAST, SABR or TMPR
		    var powerGauntletSpells = new List<FF1Lib.Spell>(spellHelper.FindSpells(SpellRoutine.Fast, SpellTargeting.Any).
								     Concat(spellHelper.FindSpells(SpellRoutine.Sabr, SpellTargeting.Any)).
								     Select(s => s.Id));
		    // cast INV2, FOG2, or WALL
		    var whiteShirtSpells = new List<FF1Lib.Spell>(spellHelper.FindSpells(SpellRoutine.Ruse, SpellTargeting.AllCharacters).

								Concat(spellHelper.FindSpells(SpellRoutine.ArmorUp, SpellTargeting.AllCharacters)).

								Concat(spellHelper.FindSpells(SpellRoutine.DefElement, SpellTargeting.Any).Where(s => s.Info.status == SpellStatus.Any)).

								Select(s => s.Id));
		    // Any elemental AOE damage, excludes NUKE/FADE
		    var blackShirtSpells = new List<FF1Lib.Spell>(spellHelper.FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies).
								Where(s => s.Info.elem != (byte)SpellElement.None).
								Select(s => s.Id));
		    var otherSpells = new List<FF1Lib.Spell>(spellHelper.FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies).
							     Where(s => s.Info.elem != (byte)SpellElement.None).
							     Concat(spellHelper.FindSpells(SpellRoutine.DamageUndead, SpellTargeting.AllEnemies)).
							     Concat(spellHelper.FindSpells(SpellRoutine.Heal, SpellTargeting.AllCharacters)).
							     Concat(spellHelper.FindSpells(SpellRoutine.ArmorUp, SpellTargeting.AllCharacters)).
							     Concat(spellHelper.FindSpells(SpellRoutine.Lock, SpellTargeting.OneEnemy)).
							     Concat(spellHelper.FindSpells(SpellRoutine.Lock, SpellTargeting.AllEnemies)).
							     Concat(spellHelper.FindSpells(SpellRoutine.Ruse, SpellTargeting.Any)).
							     Select(s => s.Id));

		    var generatedItems = new HashSet<FF1Lib.Item>();
		    var generatedNames = new HashSet<string>();
		    for (int tier = 2; tier >= 0; tier--) {
			List<int> requireType;
			List<int> requireClasses;
			switch(tier) {
			    case 0:
				requireType = new List<int>    {BRACELET, BRACELET, BRACELET,  ARMOR, ARMOR};
				requireClasses = new List<int> {   LIGHT,   MEDIUM,    HEAVY, MEDIUM, HEAVY};
				break;
			    case 1:
				requireType = new List<int>    {ARMOR};
				requireClasses = new List<int> {HEAVY};
				break;
			    case 2:
				requireType = new List<int>    { ARMOR, SHIELD,   HELM, GAUNTLET};
				requireClasses = new List<int> {KNIGHT, KNIGHT, KNIGHT,   KNIGHT};
				break;
			    default:
				requireType = new List<int>();
				requireClasses = new List<int>();
				break;
			}
			for (int count = 0; count < tierList[tier].Count; ) {
			    var itemId = tierList[tier][count];

			    if (generatedItems.Contains(itemId)) {
				count++;
				continue;
			    }

			    int armorType;
			    int armorClass;

			    // incentive armor that needs to be re-created:
			    //
			    // opal bracelet
			    // power bonk
			    // white shirt
			    // black shirt
			    // ribbon

			    var accessories = new List<int> {HELM, CAPE, RING};

			    armorType = rng.Between(0, 7);
			    armorClass = rng.Between(0, 3);
			    switch(itemId) {
				case Item.Opal:
				    armorType = BRACELET;
				    armorClass = KNIGHT;
				    break;
				case Item.PowerGauntlets:
				    armorType = GAUNTLET;
				    break;
				case Item.WhiteShirt:
				    armorType = SHIRT;
				    armorClass = HEAVY;
				    break;
				case Item.BlackShirt:
				    armorType = SHIRT;
				    armorClass = HEAVY;
				    break;
				case Item.Ribbon:
				    armorType = accessories[rng.Between(0, accessories.Count-1)];
				    armorClass = CLOTH;
				    break;
				default:
				    if (requireType.Count > 0) {
					armorType = requireType[0];
					armorClass = requireClasses[0];
					requireType.RemoveAt(0);
					requireClasses.RemoveAt(0);
				    } else if (armorType == BRACELET) {
					// Don't roll any extra bracelets
					while (armorType == BRACELET) {
					    armorType = rng.Between(0, 7);
					}
				    }
				    break;
			    }

			    var name = "";
			    byte weight = 0;
			    byte absorb = 0;
			    byte elementalResist = 0;
			    byte spellIndex = 0xFF;
			    ArmorType type = ArmorType.ARMOR;
			    ushort permissions = 0;

			    absorb = (byte)RangeScale(armorDefenseBase[armorClass, armorType], .7, 1.4, 1.0, rng);
			    weight = (byte)RangeScale(armorEvadePenaltyBase[armorClass, armorType], .7, 1.4, 1.0, rng);

			    string[] nameClass;
			    if (armorType == RING || armorType == BRACELET || armorType == SHIELD) {
				nameClass = ringNames[armorClass];
			    } else {
				nameClass = classNames[armorClass];
			    }
			    name = nameClass[rng.Between(0, nameClass.Length-1)];

			    switch (armorClass) {
				case CLOTH:
				    permissions = 0xFFF;
				    break;
				case LIGHT:
				    permissions = (ushort)(EquipPermission.Fighter|EquipPermission.Knight|
							   EquipPermission.Thief|EquipPermission.Ninja|
							   EquipPermission.BlackBelt|EquipPermission.Master|
							   EquipPermission.RedMage|EquipPermission.RedWizard);
				    break;
				case MEDIUM:
				    permissions = (ushort)(EquipPermission.Fighter|EquipPermission.Knight|
							   EquipPermission.Ninja|
							   EquipPermission.RedMage|EquipPermission.RedWizard);
				    break;
				case HEAVY:
				    permissions = (ushort)(EquipPermission.Fighter|EquipPermission.Knight|
							   EquipPermission.Ninja);
				    break;
				case KNIGHT:
				    permissions = (ushort)EquipPermission.Knight;
				    break;
			    }

			    var spells = new List<FF1Lib.Spell>();
			    if (itemId == Item.PowerGauntlets) {
				spells = powerGauntletSpells;
			    } else if (itemId == Item.WhiteShirt) {
				spells = whiteShirtSpells;
			    } else if (itemId == Item.BlackShirt) {
				spells = blackShirtSpells;
			    } else if (itemId == Item.Ribbon) {
			    } else if (tier >= 1 &&
				       (armorType == HELM || armorType == GAUNTLET ||
					armorType == SHIRT || armorType == CAPE ||
					armorType == RING))
			    {
				var roll = rng.Between(0, 100);
				if (roll < 50) {
				    spells = otherSpells;
				}
			    }
			    if (spells.Count > 0 && !noItemMagic) {
				spellIndex = (byte)(spells[rng.Between(0, spells.Count-1)]-Spell.CURE);
				name = allSpells[spellIndex].Name;
			    }

			    var chooseResist = new List<byte>(resists);
				if (!noResists)
				{
					if (itemId == Item.Ribbon)
					{
						elementalResist |= 0xFF;
						name = "Ribbon";
						chooseResist.Clear();
					}
					else if (tier == 2 && (armorType == ARMOR || armorType == SHIRT))
					{
						elementalResist |= chooseResist.SpliceRandom(rng);
						name = resistNames[elementalResist];
						elementalResist |= chooseResist.SpliceRandom(rng);
						elementalResist |= chooseResist.SpliceRandom(rng);
					}
					else if (tier == 1 && (armorType == SHIRT))
					{
						elementalResist |= chooseResist.SpliceRandom(rng);
						name = resistNames[elementalResist];
						elementalResist |= chooseResist.SpliceRandom(rng);
					}
					else if (tier >= 1 && (armorType != BRACELET) && spellIndex == 0xFF)
					{
						elementalResist |= chooseResist.SpliceRandom(rng);
						name = resistNames[elementalResist];
					}
				}

			    switch (armorType) {
				case ARMOR:
				    name = $"{name,-6}{Armor.IconCodes[ArmorIcon.ARMOR]}";
				    type = ArmorType.ARMOR;
				    break;
				case SHIELD:
				    name = $"{name,-6}{Armor.IconCodes[ArmorIcon.SHIELD]}";
				    type = ArmorType.SHIELD;
				    if (armorClass <= HEAVY) {
					permissions = (ushort)(EquipPermission.Fighter|EquipPermission.Knight|EquipPermission.Ninja);
				    }
				    break;
				case HELM:
				    name = $"{name,-6}{Armor.IconCodes[ArmorIcon.HELM]}";
				    type = ArmorType.HELM;
				    break;
				case GAUNTLET:
				    name = $"{name,-6}{Armor.IconCodes[ArmorIcon.GAUNTLET]}";
				    type = ArmorType.GAUNTLET;
				    break;
				case BRACELET:
				    name = $"{name,-6}{Armor.IconCodes[ArmorIcon.BRACELET]}";
				    type = ArmorType.ARMOR;
				    permissions = 0xFFF;
				    break;
				case SHIRT:
				    type = ArmorType.ARMOR;
				    if (itemId == Item.WhiteShirt) {
					name = "White";
					permissions = (ushort)(EquipPermission.WhiteWizard);
				    } else if (itemId == Item.BlackShirt) {
					name = "Black";
					permissions = (ushort)(EquipPermission.BlackWizard);
				    } else {
					permissions = (ushort)(EquipPermission.WhiteWizard | EquipPermission.BlackWizard | EquipPermission.RedWizard);
				    }
				    name = $"{name,-6}{Armor.IconCodes[ArmorIcon.SHIRT]}";
				    break;
				case CAPE:
				    name = $"{name,-6}C";
				    type = ArmorType.SHIELD;
				    permissions = 0xFFF & ~((int)EquipPermission.BlackBelt|(int)EquipPermission.Master);
				    break;
				case RING:
				    name = $"{name,-6}R";
				    type = ArmorType.GAUNTLET;
				    permissions = 0xFFF;
				    break;
			    }

			    if (generatedNames.Contains(name)) {
				continue;
			    }
			    generatedNames.Add(name);

			    int resistCount = resists.Length - chooseResist.Count;
			    double score = Math.Max(absorb - (weight*.20), 1) + (Math.Min(resistCount, 4)*45);
			    double goldvalue = score;
			    if (spellIndex != 0xFF) {
				goldvalue += 30;
			    }
			    if (armorType == BRACELET) {
				goldvalue *= 4;
			    }
			    switch (tier) {
				case 0:
				    goldvalue *= (goldvalue + 5) * 2;
				    break;
				case 1:
				    goldvalue *= (goldvalue + 25);
				    break;
				case 2:
				    goldvalue *= (goldvalue + 40);
				    break;
			    }

			    goldvalue = Math.Ceiling(goldvalue);
			    goldvalue = Math.Min(Math.Max(goldvalue, 1), 65535);

			    var casting = "";
			    if (spellIndex != 0xFF) {
				casting = "casting: " + allSpells[spellIndex].Name;
			    }

			    var resistName = "";
			    for (int j = 0; j < resists.Length; j++) {
				if ((elementalResist & resists[j]) != 0) {
				    resistName += " " + (SpellElement)resists[j];
				}
			    }
			    if (resistName != "") {
				resistName = "resist:" + resistName;
			    }

			    var armor = new Armor(itemId-Item.Cloth, name, ArmorIcon.NONE, weight, absorb,
						  elementalResist, (byte)(spellIndex == 0xFF ? 0 : spellIndex+1), type);
				ArmorPermissions[armor.Id] = permissions;
			    armor.writeArmorMemory(this);
			    Put(PriceOffset + (PriceSize*(int)itemId), Blob.FromUShorts(new ushort[] {(ushort)goldvalue}));

			    generatedItems.Add(itemId);
			    count++;
			}
		    }
		}
	}

	class Armor
	{
		public Item Id => (Item)(ArmorIndex + (int)Item.Cloth);
		public Spell Spell => SpellIndex == 0xFF ? 0 : (Spell)(SpellIndex - 1 + (int)Spell.CURE);


		//offset lookups
		//0 - weight
		//1 - absorb
		//2 - resist
		//3 - spell index

		public int ArmorIndex;

		public string Name;
		public ArmorIcon Icon;

		public byte Weight;
		public byte Absorb;
		public byte ElementalResist;
		public byte SpellIndex;
        public ArmorType Type;

	    public Armor(int armorIndex, string name, ArmorIcon icon, byte weight, byte absorb, byte elementalResist, byte spellIndex, ArmorType type)
		{
			ArmorIndex = armorIndex;
			Name = name;
			Icon = icon;
			Weight = weight;
			Absorb = absorb;
			ElementalResist = elementalResist;
			SpellIndex = spellIndex;
			Type = type;
		}

		public Armor(int armorIndex, FF1Rom rom)
		{

			ArmorIndex = armorIndex;
			//read stats from memory
			int armorBaseOffset = FF1Rom.ArmorOffset + (ArmorIndex * FF1Rom.ArmorSize);
			Weight = rom.Get(armorBaseOffset, 1).ToBytes()[0];
			Absorb = rom.Get(armorBaseOffset + 1, 1).ToBytes()[0];
			ElementalResist = rom.Get(armorBaseOffset + 2, 1).ToBytes()[0];
			SpellIndex = rom.Get(armorBaseOffset + 3, 1).ToBytes()[0];
			Type = (ArmorType)rom.Get(FF1Rom.ArmorTypeOffset+ArmorIndex, 1).ToBytes()[0];

			//get name stuff
			Icon = ArmorIcon.NONE;

			Name = rom.ItemsText[(int)Item.Cloth + ArmorIndex];

			foreach (var kv in IconCodes)
			{
				if (Name.Contains(kv.Value))
				{
					Icon = kv.Key;
					break;
				}
			}
		}

		public static IEnumerable<Armor> LoadAllArmors(FF1Rom rom, Flags flags)
		{
			for (int i = 0; i < 40; i++)
			{
				yield return new Armor(i, rom);
			}
		}

		public void writeArmorMemory(FF1Rom rom)
		{
			//armor stats
			int armorBaseOffset = FF1Rom.ArmorOffset + (ArmorIndex * FF1Rom.ArmorSize);
			rom.Put(armorBaseOffset, new byte[] { Weight, Absorb, ElementalResist, SpellIndex });

			rom.Put(FF1Rom.ArmorTypeOffset + ArmorIndex, new byte[] { (byte)Type });

			rom.ItemsText[(int)Item.Cloth + ArmorIndex] = Name;
		}

		private ArmorIcon getArmorIconFromByte(byte icon)
		{
			ArmorIcon matchedType = ArmorIcon.NONE;
			foreach (ArmorIcon temp in (ArmorIcon[])Enum.GetValues(typeof(ArmorIcon)))
			{
				if (icon == (byte)temp)
				{
					matchedType = temp;
				}
			}

			return matchedType;
		}

		public static Dictionary<ArmorIcon, string> IconCodes = new Dictionary<ArmorIcon, string>
		{
			{ ArmorIcon.ARMOR, "@A" },
			{ ArmorIcon.SHIELD, "@s" },
			{ ArmorIcon.HELM, "@h" },
			{ ArmorIcon.GAUNTLET, "@G" },
			{ ArmorIcon.BRACELET, "@B" },
			{ ArmorIcon.SHIRT, "@T" }
		};
	}
}
