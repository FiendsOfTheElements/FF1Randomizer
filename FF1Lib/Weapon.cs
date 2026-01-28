using FF1Lib.Helpers;

namespace FF1Lib
{
	public enum WeaponIcon : byte
	{
		NONE = 0x00,
		SWORD = 0xD4,
		HAMMER = 0xD5,
		KNIFE = 0xD6,
		AXE = 0xD7,
		STAFF = 0xD8,
		CHUCK = 0xD9,
	}

	public enum WeaponSprite : byte
	{
		NONE = 0x00,
		SHORTSWORD = 0x80,
		SCIMITAR = 0x84,
		FALCHION = 0x88,
		LONGSWORD = 0x8C,
		RAPIER = 0x90,
		HAMMER = 0x94,
		KNIFE = 0x98,
		AXE = 0x9C,
		STAFF = 0xA0,
		IRONSTAFF = 0xA4,
		CHUCK = 0xA8
	}

	[Flags]
	public enum MonsterType : byte
	{
		NONE = 0x00,
		MAGICAL = 0x01,
		DRAGON = 0x02,
		GIANT = 0x04,
		UNDEAD = 0x08,
		WERE = 0x10,
		AQUATIC = 0x20,
		MAGE = 0x40,
		REGENERATIVE = 0x80
	}

	[Flags]
	public enum EquipPermission : ushort
	{
		Fighter = 0x800,
		Thief = 0x400,
		BlackBelt = 0x200,
		RedMage = 0x100,
		WhiteMage = 0x080,
		BlackMage = 0x040,
		Knight = 0x020,
		Ninja = 0x010,
		Master = 0x008,
		RedWizard = 0x004,
		WhiteWizard = 0x002,
		BlackWizard = 0x001,
		None = 0x000
	}

	// lut_ClassEquipBit: ;  FT   TH   BB   RM   WM   BM      KN   NJ   MA   RW   WW   BW
	// .WORD               $800,$400,$200,$100,$080,$040,   $020,$010,$008,$004,$002,$001

	public partial class FF1Rom : NesRom
	{
		public const int WeaponPermissionsOffset = 0x3BF50;
		public const int PermissionsSize = 2;
		public const int PermissionsCount = 40;

		//offset lookups
		//0 - hit
		//1 - dmg
		//2 - crit
		//3 - weapon spell index
		//4 - elemental weaknesses
		//5 - type weaknesses
		//6 - weapon type sprite
		//7 - weapon sprite palette color

		public void RandomWeaponBonus(Flags flags, bool cleanNames, List<int> blurseValues, MT19337 rng)
		{
			bool enable = (bool)flags.RandomWeaponBonus;
			int min = flags.RandomWeaponBonusLow;
			int max = flags.RandomWeaponBonusHigh;
			bool excludeMasa = (bool)flags.RandomWeaponBonusExcludeMasa;

			if (!enable)
			{
				return;
			}

			//get base stats
			Weapon currentWeapon;

			for (int i = 0; i < WeaponCount; i++)
			{
				if (i != 39 || !excludeMasa)
				{
					currentWeapon = new Weapon(i, this);
					int bonus = rng.Between(min, max);
					blurseValues[i] = bonus;
					if (bonus != 0)
					{
						//adjust stats
						//clamp to 1 dmg min, 0 hit min, 50 hit maximum
						currentWeapon.HitBonus = (byte)Math.Max(0, (int)(currentWeapon.HitBonus + (3 * bonus)));
						currentWeapon.HitBonus = (byte)Math.Min(50, (int)(currentWeapon.HitBonus));
						currentWeapon.Damage = (byte)Math.Max(1, (int)currentWeapon.Damage + (2 * bonus));
						currentWeapon.Crit = (byte)Math.Max(1, (int)currentWeapon.Crit + (3 * bonus));

						// Shortens names to make more sense when there are only 4 letters available
						// Most of these are only used with Weaponizer, but some are vanilla
						if (cleanNames)
						{
							// I didn't use a switch+case here because of the mix of name lengths being checked, but I'm sure this could be more elegant and anyone can feel free to clean it up
							if (currentWeapon.Name[0..5] == "Shock") { currentWeapon.Name = "Shok" + currentWeapon.Name.Substring(4); }
							else if (currentWeapon.Name[0..5] == "Water")  { currentWeapon.Name = "Watr" + currentWeapon.Name.Substring(4); }
							else if (currentWeapon.Name[0..6] == "IceHot") { currentWeapon.Name = "IcHt" + currentWeapon.Name.Substring(4); }
							else if (currentWeapon.Name[0..5] == "Storm")  { currentWeapon.Name = "Strm" + currentWeapon.Name.Substring(4); }
							else if (currentWeapon.Name[0..6] == "Splash") { currentWeapon.Name = "Aqua" + currentWeapon.Name.Substring(4); }
							else if (currentWeapon.Name[0..6] == "Banish") { currentWeapon.Name = "Holy" + currentWeapon.Name.Substring(4); }
							else if (currentWeapon.Name[0..5] == "Smite")  { currentWeapon.Name = "Holy" + currentWeapon.Name.Substring(4); }
							else if (currentWeapon.Name[0..6] == "Divine") { currentWeapon.Name = "Bles" + currentWeapon.Name.Substring(4); }
							else if (currentWeapon.Name[0..5] == "Weird")  { currentWeapon.Name = "Odd " + currentWeapon.Name.Substring(4); }
							else if (currentWeapon.Name[0..5] == "Prism")  { currentWeapon.Name = "Prsm" + currentWeapon.Name.Substring(4); }

							else if (currentWeapon.Name[0..5] == "Sharp")  { currentWeapon.Name = "Shrp" + currentWeapon.Name.Substring(4); }
							else if (currentWeapon.Name[0..6] == "Copper") { currentWeapon.Name = "Copr" + currentWeapon.Name.Substring(4); }
							else if (currentWeapon.Name[0..5] == "Shiny")  { currentWeapon.Name = "Shny" + currentWeapon.Name.Substring(4); }
							else if (currentWeapon.Name[0..6] == "Wicked") { currentWeapon.Name = "Wckd" + currentWeapon.Name.Substring(4); }
							else if (currentWeapon.Name[0..5] == "Steel")  { currentWeapon.Name = "Stl " + currentWeapon.Name.Substring(4); }
							else if (currentWeapon.Name[0..5] == "Heavy")  { currentWeapon.Name = "Hvy " + currentWeapon.Name.Substring(4); }
							else if (currentWeapon.Name[0..6] == "Silver") { currentWeapon.Name = "Slvr" + currentWeapon.Name.Substring(4); }

							else if (currentWeapon.Name[0..6] == "Vorpal") { currentWeapon.Name = "Vorpl" + currentWeapon.Name.Substring(5); }
						}

						//change last two non icon characters to -/+bonus
						string bonusString = string.Format((bonus > 0) ? "+{0}" : "-{0}", Math.Abs(bonus).ToString());
						byte[] bonusBytes = FF1Text.TextToBytes(bonusString);

						var nameBytes = FF1Text.TextToBytes(currentWeapon.Name, false);

						int iconIndex = nameBytes[6] > 200 && nameBytes[6] != 255 ? 5 : 6;
						for (int j = 0; j < bonusBytes.Length - 1; j++)
						{
							nameBytes[iconIndex - j] = bonusBytes[bonusBytes.Length - 2 - j];
						}

						currentWeapon.Name = FF1Text.BytesToText(nameBytes);

						currentWeapon.writeWeaponMemory(this);
					}
				}
			}

			return;
		}

		//sample function for creating new weapons
		public void ExpandWeapon()
		{
			Weapon flameChucks = new Weapon(0, "Flame@N", WeaponIcon.CHUCK, 20, 26, 10, 0, (byte)SpellElement.Fire, 0, WeaponSprite.CHUCK, 0x25);
			WeaponPermissions[flameChucks.Id] = (ushort)(EquipPermission.BlackBelt | EquipPermission.Master | EquipPermission.Ninja);
			flameChucks.writeWeaponMemory(this);
		}

		public void Weaponizer(Flags flags, MT19337 rng) {

			bool enableWeaponizer = (bool)flags.Weaponizer;
			bool useQualityNamesOnly = (bool)flags.WeaponizerNamesUseQualityOnly;
			bool commonWeaponsHavePowers = (bool)flags.WeaponizerCommonWeaponsHavePowers;
			bool noItemMagic = flags.ItemMagicMode == ItemMagicMode.None;

			if (!enableWeaponizer)
			{
				return;
			}

			var tierList = new List<IReadOnlyList<Item>> { ItemLists.CommonWeaponTier, ItemLists.RareWeaponTier,
							  ItemLists.LegendaryWeaponTier, ItemLists.UberTier};
		    var damageBases = new int[]      { 10, 18, 26, 36, 52 };
		    var critPercentBases = new int[] {  5, 10, 20, 30, 45 };
		    var hitPercentBases = new int[]  {  5, 10, 20, 40, 60 };

		    var weaponTypeAdjust = new int[,] {
			// damage, crit%, hit%
			{  0,  0,  0 }, // sword
			{  1,  0, -1 }, // axe
			{ -1,  0,  1 }, // knife
			{ -1,  1,  0 }, // chucks
			{  0, -1,  0 }, // hammer
			{  0,  0, -1 }, // staff
		    };

		    var powers = new int[] {
			// Chroma/XCal effect should stay first so it properly gets assigned to Xcalbr
			(int)MonsterType.MAGICAL<<8|(int)MonsterType.DRAGON<<8|(int)MonsterType.GIANT<<8|(int)MonsterType.UNDEAD<<8
				|(int)MonsterType.WERE<<8|(int)MonsterType.AQUATIC<<8|(int)MonsterType.MAGE<<8|(int)MonsterType.REGENERATIVE<<8 |
				(int)SpellElement.Poison|(int)SpellElement.Fire|(int)SpellElement.Ice|(int)SpellElement.Lightning|
				(int)SpellElement.Earth|(int)SpellElement.Death|(int)SpellElement.Time|(int)SpellElement.Status,
			
			(int)SpellElement.Poison, // Sort of a Dud; only slays vs Tiamat-1 and RedD
			(int)SpellElement.Earth|(int)SpellElement.Death|(int)SpellElement.Time|(int)SpellElement.Status|
				(int)MonsterType.WERE<<8|(int)MonsterType.REGENERATIVE<<8, // Most of these attributes aren't used in vanilla but can matter with e.g. Enemizer; Regen covers 9 vanilla enemies, notably WarMech
			
			(int)SpellElement.Fire | ((int)MonsterType.UNDEAD<<8) | ((int)MonsterType.WERE<<8), //Undead only adds Lich-2 to the Fire list
			
			(int)SpellElement.Ice,
			
			(int)SpellElement.Lightning,
			(int)SpellElement.Lightning | (int)MonsterType.AQUATIC<<8, // Almost the same as just Lightning; adds Kraken-2 and Wizard
			
			(int)(MonsterType.MAGICAL|MonsterType.MAGE)<<8,
			
			(int)MonsterType.DRAGON<<8,
			
			(int)MonsterType.GIANT<<8,
			
			(int)MonsterType.UNDEAD<<8,
			
			(int)MonsterType.AQUATIC<<8,
			
			(int)SpellElement.Fire | (int)SpellElement.Ice,

			(int)SpellElement.Poison | (int)SpellElement.Fire | (int)SpellElement.Ice | (int)SpellElement.Lightning,
			
			(int)MonsterType.AQUATIC<<8 | (int)MonsterType.MAGE<<8 | (int)MonsterType.REGENERATIVE<<8, // Covers all Fiends and WarMech
		    };

		    var powerNames = new string[][] {
			new string[] { "Chroma", "Prism" },
			
			new string[] { "Poison" },
			new string[] { "Odd", "Weird" },
			
			new string[] { "Flame", "Burn", "Blaze", "Hot", "Heat" },
			
			new string[] { "Icy", "Freeze", "Frost", "Cold", "Frozen" },
			
			new string[] { "Shock", "Bolt" },
			new string[] { "Storm" },
			
			new string[] { "Rune", "Ritual" },
		
			new string[] { "Dragon", "Dino" },
			
			new string[] { "Giant", "Imp", "Ogre" },
			
			new string[] { "Holy", "Smite", "Banish", "Divine", "Blessd", "Sun" },

			new string[] { "Coral", "Aqua", "Water", "Splash" },
			
			new string[] { "IceHot" },
			
			new string[] { "Elmntl" },
			
			new string[] { "Boss" },
		    };

			// Weighs some effects to be more common than others, reducing how often both the strongest and weakest are seen
			var powerWeighting = new int[] {
				0,			// Chroma
				1,			// Poison
				2,			// Weird
				3,  3,  3,	// Flame
				4,  4,		// Icy
				5,			// Shock
				6,			// Storm (Very similar to Shock)
				7,  7,		// Rune
				8,  8,  8,	// Dragon
				9,  9,		// Giant
				10, 10, 10,	// Holy
				11, 11,	11,	// Aqua
				12, 12,		// IceHot
				13,			// Elmntl
				14,			// Boss
			};

			var weaponIcons = new WeaponIcon[] {
			WeaponIcon.SWORD,
			WeaponIcon.AXE,
			WeaponIcon.KNIFE,
			WeaponIcon.CHUCK,
			WeaponIcon.HAMMER,
			WeaponIcon.STAFF
		    };

		    var unpromotedPermissions = new Dictionary<WeaponIcon, EquipPermission>
		    {
			{ WeaponIcon.SWORD,  EquipPermission.Fighter|EquipPermission.Thief|EquipPermission.RedMage },
			{ WeaponIcon.AXE,    EquipPermission.Fighter },
			{ WeaponIcon.KNIFE,  EquipPermission.Fighter|EquipPermission.Thief|EquipPermission.RedMage|EquipPermission.BlackMage },
			{ WeaponIcon.CHUCK,  EquipPermission.Thief|EquipPermission.BlackBelt },
			{ WeaponIcon.HAMMER, EquipPermission.Fighter|EquipPermission.WhiteMage },
			{ WeaponIcon.STAFF,  EquipPermission.RedMage|EquipPermission.BlackMage|EquipPermission.WhiteMage }
		    };

		    var promotedPermissions = new Dictionary<WeaponIcon, EquipPermission>
		    {
			{ WeaponIcon.SWORD,  EquipPermission.Knight|EquipPermission.Ninja|EquipPermission.RedWizard },
			{ WeaponIcon.AXE,    EquipPermission.Knight|EquipPermission.Ninja },
			{ WeaponIcon.KNIFE,  EquipPermission.Knight|EquipPermission.Ninja|EquipPermission.RedWizard|EquipPermission.BlackWizard },
			{ WeaponIcon.CHUCK,  EquipPermission.Ninja|EquipPermission.Master },
			{ WeaponIcon.HAMMER, EquipPermission.Knight|EquipPermission.WhiteWizard|EquipPermission.Ninja },
			{ WeaponIcon.STAFF,  EquipPermission.RedWizard|EquipPermission.WhiteWizard|EquipPermission.BlackWizard|EquipPermission.Ninja },
		    };

		    var weaponSprites = new WeaponSprite[][] {
			new WeaponSprite[] { WeaponSprite.SHORTSWORD, WeaponSprite.SCIMITAR, WeaponSprite.FALCHION,
					     WeaponSprite.LONGSWORD, WeaponSprite.RAPIER },
			new WeaponSprite[] { WeaponSprite.AXE },
			new WeaponSprite[] { WeaponSprite.KNIFE },
			new WeaponSprite[] { WeaponSprite.CHUCK },
			new WeaponSprite[] { WeaponSprite.HAMMER, WeaponSprite.IRONSTAFF },
			new WeaponSprite[] { WeaponSprite.STAFF, WeaponSprite.IRONSTAFF },
		    };

		    var qualityLevels = new int[] {
			18,
			26,
			32,
			40,
			50,
			60
		    };
		    var gearQuality = new string[][] {
			new string[] { "Wooden", "Small", "Short" }, // <= 18
			new string[] { "Copper", "Long"},            // <= 26
			new string[] { "Iron",   "Heavy" },          // <= 32
			new string[] { "Steel",  "Great" },          // <= 40
			new string[] { "Silver", "Shiny" },          // <= 50
			new string[] { "Mithrl", "Sharp" },          // <= 60
			new string[] { "Opal", "Diamnd", "Wicked" }  // > 60
		    };

		    var preferredSprites = new Dictionary<string, WeaponSprite> {
			{"Short @S", WeaponSprite.RAPIER},
			{"Small @S", WeaponSprite.RAPIER},
			{"Long  @S", WeaponSprite.LONGSWORD},
			{"Katana ", WeaponSprite.SCIMITAR},
		    };
			/// IMPORTANT:
			/// The following needs to include any spells whose names may have been changed
			/// for accessibility. Look for an IMPORTANT comment in Magic.cs
		    var preferredColors = new Dictionary<string, byte> {
			{"Wooden", 0x28},
			{"Copper", 0x27},
			{"Bronze", 0x27},
			{"Iron", 0x20},
			{"Steel", 0x20},
			{"Silver", 0x2C},
			{"Mithrl", 0x2C},
			{"Ice", 0x22},
			{"Freeze", 0x22},
			{"ICE ", 0x22},
			{"ICE2", 0x22},
			{"ICE3", 0x22},
			{"Flame", 0x26},
			{"Burn", 0x26},
			{"FIRE", 0x26},
			{"FIR2", 0x26},
			{"FIR3", 0x26},
			{"Shock", 0x28},
			{"Bolt", 0x28},
			{"LIT ", 0x28},
			{"LIT2", 0x28},
			{"LIT3", 0x28},
			{"THUN", 0x28},
			{"THN2", 0x28},
			{"THN3", 0x28},
			{"Dragon", 0x2A},
		    };

		    var spellHelper = new SpellHelper(this);
		    var Spells = GetSpells();

		    var defenseSwordSpells = new List<FF1Lib.Spell>(spellHelper.FindSpells(SpellRoutine.Ruse, SpellTargeting.Any).
								Concat(spellHelper.FindSpells(SpellRoutine.ArmorUp, SpellTargeting.AllCharacters)).
								Concat(spellHelper.FindSpells(SpellRoutine.DefElement, SpellTargeting.Any).Where(s => s.Info.status == SpellStatus.Any)).
								Select(s => s.Id));
		    var thorHammerSpells =  new List<FF1Lib.Spell>(spellHelper.FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies).
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

		    var weaponNames = new List<string>();
		    var weaponGfx = new List<int>();

		    for (int tier = 4; tier > 0; tier--) {
			var requireTypes = new List<int> { 0, 1, 2, 3, 5 };
			for (int count = 0; count < tierList[tier-1].Count; ) {
			    var weaponItemId = tierList[tier-1][count];
			    int weaponIndex = (int)weaponItemId - (int)Item.WoodenNunchucks;
			    string name;
			    WeaponIcon icon;
			    byte hitBonus;
			    byte damage;
			    byte crit;
			    byte spellIndex = 0xFF;
			    byte elementalWeakness = 0;
			    byte typeWeakeness = 0;
			    WeaponSprite weaponTypeSprite = WeaponSprite.NONE;
			    byte weaponSpritePaletteColor = 0;

			    int weaponType;
			    switch (weaponItemId) {
				case Item.Masamune:
				case Item.Xcalber:
				    // 0 (sword) or 1 (axe)
				    weaponType = rng.Between(0, 1);
				    break;
				case Item.Katana:
				case Item.Vorpal:
				case Item.Defense:
				    weaponType = 0;
				    break;
				case Item.ThorHammer:
				    weaponType = 4;
				    break;
				default:
				    if (requireTypes.Count > 0) {
					weaponType = requireTypes[0];
				    } else {
					weaponType = rng.Between(0, 5);
				    }
				    break;
			    }

			    icon = weaponIcons[weaponType];

			    int damageTier = tier + weaponTypeAdjust[weaponType, 0];
			    int critTier =   tier + weaponTypeAdjust[weaponType, 1];
			    int hitpctTier = tier + weaponTypeAdjust[weaponType, 2];

			    if (weaponItemId == Item.Katana || weaponItemId == Item.Vorpal) {
				critTier += 1;
			    }

			    if (weaponItemId == Item.Masamune) {
				critTier -= 1;
			    }

			    if (weaponItemId == Item.Vorpal) {
				damageTier -= 1;
			    }

			    if (weaponItemId == Item.ThorHammer) {
				damageTier += 1;
			    }

			    damageTier = Math.Min(Math.Max(damageTier, 0), 4);
			    critTier   = Math.Min(Math.Max(critTier,   0), 4);
			    hitpctTier = Math.Min(Math.Max(hitpctTier, 0), 4);

			    damage = (byte)RangeScale(damageBases[damageTier], .7, 1.4, 1.0, rng);
			    crit = (byte)RangeScale(critPercentBases[critTier], .7, 1.4, 1.0, rng);
			    hitBonus = (byte)RangeScale(hitPercentBases[hitpctTier], .7, 1.4, 1.0, rng);

			    // need to clamp this to prevent overflow
			    hitBonus = Math.Min(hitBonus, (byte)50);

			    double ddamage = damage;
			    double dcrit = crit;
			    double dhitBonus = hitBonus;

			    // "score" is a rough approximation of DPS
			    // used to assign name based on gear
			    // quality and set the base price.
			    double score = ((ddamage*1.5) + ((ddamage*2.0) * (dcrit / 200.0))) * (1+Math.Floor((dhitBonus+4)/32));

			    int specialPower = -1;
			    var spells = new List<FF1Lib.Spell>();
			    if (weaponItemId == Item.Defense) {
				spells = defenseSwordSpells;
			    } else if (weaponItemId == Item.ThorHammer) {
				spells = thorHammerSpells;
			    } else if (weaponItemId == Item.Xcalber) {
				 // Give xcal the same type weakness
				 // bonus (all of them) as vanilla
				 // xcal because based on player
				 // feedback, that's what they expect.
				specialPower = 0;
			    } else {
				int spellChance = rng.Between(1, 100);
				if ((commonWeaponsHavePowers || tier == 2)
				    && ((weaponType < 4 && spellChance <= Math.Min(10*tier, 20))
					|| (weaponType >= 4 && spellChance <= Math.Min(30*tier, 60))))
				{
				    spells = otherSpells;
				}
			    }
			    if (spells.Count > 0 && !noItemMagic) {
				spellIndex = (byte)(spells[rng.Between(0, spells.Count-1)]-Spell.CURE);
			    }

			    if (spellIndex == 0xFF && specialPower == -1) {
				int powerChance = rng.Between(1, 100);
				if (tier == 1 && commonWeaponsHavePowers && powerChance <= 20) {
				    specialPower = powerWeighting[rng.Between(0, powerWeighting.Length - 1)];
				} else if (tier > 1) {
				    specialPower = powerWeighting[rng.Between(0, powerWeighting.Length - 1)];
				}
			    }

			    if (specialPower != -1) {
				elementalWeakness = (byte)(powers[specialPower] & 0xFF);
				typeWeakeness = (byte)((powers[specialPower]>>8) & 0xFF);
			    }

			    string nameWithIcon;
			    if (weaponItemId == Item.Masamune) {
				name = nameWithIcon = (weaponType == 0 ? "Masmune" : "Maxmune");
			    } else if (weaponItemId == Item.Xcalber) {
				name = nameWithIcon = (weaponType == 0 ? "Xcalber" : "Axcalbr");
			    } else if (weaponItemId == Item.Katana) {
				name = nameWithIcon = "Katana ";
			    } else if (weaponItemId == Item.Vorpal) {
				name = nameWithIcon = "Vorpal ";
			    } else if (weaponItemId == Item.Defense && !noItemMagic) {
				name = nameWithIcon =  "Defense";
			    } else if (weaponItemId == Item.ThorHammer && !noItemMagic) {
				name = nameWithIcon = "Thor  @H";
				elementalWeakness = (byte)(SpellElement.Ice|SpellElement.Fire|SpellElement.Lightning);
			    } else {
				if (spellIndex != 0xFF) {
				    name = Spells[spellIndex].Name;
				} else if (specialPower != -1 && !useQualityNamesOnly) {
				    var powername = rng.Between(0, powerNames[specialPower].Length-1);
				    name = powerNames[specialPower][powername];
				} else {
				    string[] gear = gearQuality[gearQuality.Length-1];
				    for (int i = 0; i < qualityLevels.Length; i++) {
					if (score <= qualityLevels[i]) {
					    gear = gearQuality[i];
					    break;
					}
				    }
				    var gearname = rng.Between(0, gear.Length-1);
				    name = gear[gearname];
				}
				nameWithIcon = $"{name,-6}{Weapon.IconCodes[icon]}";
			    }

			    if (weaponNames.Contains(nameWithIcon)) {
				continue;
			    }

			    if (nameWithIcon == "Wooden@K") {
				// Wooden stake slays vampires
				typeWeakeness = (byte)MonsterType.UNDEAD;
			    }

			    if (spellIndex != 0xFF) {
				var spellInfo = Spells[spellIndex];
				// Weapons casting elemental magic also
				// get elemental bonus
				if ((spellInfo.elem & SpellElement.Ice) != 0) {
				    elementalWeakness = (byte)SpellElement.Ice;
				}
				if ((spellInfo.elem & SpellElement.Fire) != 0) {
				    elementalWeakness = (byte)SpellElement.Fire;
				}
				if ((spellInfo.elem & SpellElement.Lightning) != 0) {
				    elementalWeakness = (byte)SpellElement.Lightning;
				}
				if ((spellInfo.elem & SpellElement.Poison) != 0) {
				    elementalWeakness = (byte)SpellElement.Poison;
				}
				if (spellInfo.routine == SpellRoutine.DamageUndead) {
				    typeWeakeness = (byte)MonsterType.UNDEAD;
				}
			    }

			    if (preferredColors.ContainsKey(name)) {
				weaponSpritePaletteColor = preferredColors[name];
			    }
			    if (preferredSprites.ContainsKey(nameWithIcon)) {
				weaponTypeSprite = preferredSprites[nameWithIcon];
			    }

			    var tries = 10;
			    while(tries > 0) {
				if (weaponTypeSprite == WeaponSprite.NONE) {
				    weaponTypeSprite = weaponSprites[weaponType][rng.Between(0, weaponSprites[weaponType].Length-1)];
				}
				if (weaponSpritePaletteColor == 0) {
				    weaponSpritePaletteColor = (byte)rng.Between(0x20, 0x2C);
				}
				if (!weaponGfx.Contains((weaponSpritePaletteColor<<8) | (int)weaponTypeSprite)) {
				    break;
				}
				weaponTypeSprite = WeaponSprite.NONE;
				weaponSpritePaletteColor = 0;
				tries--;
			    }

			    if (tries == 0) {
				continue;
			    }

			    double goldvalue = score;
			    if (specialPower != -1) {
				goldvalue += 15;
			    }
			    if (spellIndex != 0xFF) {
				goldvalue += 30;
			    }
			    goldvalue = Math.Max(goldvalue, 9);
			    switch (tier) {
				case 1:
				    goldvalue *= (goldvalue-8);
				    break;
				case 2:
				    goldvalue *= (goldvalue + 25);
				    break;
				case 3:
				    goldvalue *= (goldvalue + 45);
				    break;
				case 4:
				    goldvalue *= (goldvalue + 45);
				    break;
			    }
			    goldvalue = Math.Ceiling(goldvalue);
			    goldvalue = Math.Min(Math.Max(goldvalue, 1), 65535);

			    EquipPermission permissions;
			    if (weaponItemId == Item.Masamune) {
				permissions = (EquipPermission)0xFFF;
			    } else if (weaponItemId == Item.Xcalber) {
				permissions = EquipPermission.Knight;
			    } else if (weaponItemId == Item.Katana) {
				permissions = EquipPermission.Ninja;
			    } else if (weaponItemId == Item.ThorHammer) {
				permissions = EquipPermission.WhiteWizard;
			    } else {
				permissions = promotedPermissions[icon];
				switch (tier) {
				    case 1:
					permissions |= unpromotedPermissions[icon];
					break;
				    case 2:
					if (rng.Between(1, 100) < 50) {
					    permissions |= unpromotedPermissions[icon];
					}
					break;
				    default:
					// promoted only
					break;
				}
			    }

			    var casting = "";
			    if (spellIndex != 0xFF) {
				casting = "casting: " + Spells[spellIndex].Name;
			    }

			   var newWeapon = new Weapon(weaponIndex, nameWithIcon, icon, hitBonus, damage, crit,
						       (byte)(spellIndex == 0xFF ? 0 : spellIndex+1), elementalWeakness,
						       typeWeakeness, weaponTypeSprite, weaponSpritePaletteColor);
				WeaponPermissions[newWeapon.Id] = (ushort)permissions;
			    newWeapon.writeWeaponMemory(this);

			    Put(PriceOffset + (PriceSize*(int)weaponItemId), Blob.FromUShorts(new ushort[] {(ushort)goldvalue}));

			    weaponGfx.Add((weaponSpritePaletteColor<<8) | (int)weaponTypeSprite);
			    weaponNames.Add(nameWithIcon);
			    if (requireTypes.Count > 0) {
				requireTypes.RemoveAt(0);
			    }
			    count++;
			}
		    }
		}

	}

	public class Weapon
	{
		public Item Id => (Item)(WeaponIndex + (int)Item.WoodenNunchucks);
	        public Spell Spell => SpellIndex == 0xFF ? 0 : (Spell)(SpellIndex - 1 + (int)Spell.CURE);

		//index
		public int WeaponIndex;

		//writen to separate area
		public WeaponIcon Icon;
		public string Name;

		//written to weapon data area
		public byte HitBonus;
		public byte Damage;
		public byte Crit;
		public byte SpellIndex;
		public byte ElementalWeakness;
		public byte TypeWeakness;
		public WeaponSprite WeaponTypeSprite;
		public byte WeaponSpritePaletteColor;

		public Weapon(int weaponIndex, FF1Rom rom)
		{

			WeaponIndex = weaponIndex;
			//read stats from memory
			int weaponBaseOffset = FF1Rom.WeaponOffset + (WeaponIndex * FF1Rom.WeaponSize);
			HitBonus = rom.Get(weaponBaseOffset, 1).ToBytes()[0];
			Damage = rom.Get(weaponBaseOffset + 1, 1).ToBytes()[0];
			Crit = rom.Get(weaponBaseOffset + 2, 1).ToBytes()[0];
			SpellIndex = rom.Get(weaponBaseOffset + 3, 1).ToBytes()[0];
			ElementalWeakness = rom.Get(weaponBaseOffset + 4, 1).ToBytes()[0];
			TypeWeakness = rom.Get(weaponBaseOffset + 5, 1).ToBytes()[0];
			byte weaponSpriteTypeHolder = rom.Get(weaponBaseOffset + 6, 1).ToBytes()[0];
			WeaponTypeSprite = getWeaponSpriteFromByte(weaponSpriteTypeHolder);
			WeaponSpritePaletteColor = rom.Get(weaponBaseOffset + 7, 1).ToBytes()[0];

			//get name stuff
			Icon = WeaponIcon.NONE;

			Name = rom.ItemsText[(int)Item.WoodenNunchucks + WeaponIndex];

			foreach (var kv in IconCodes)
			{
				if (Name.Contains(kv.Value))
				{
					Icon = kv.Key;
					break;
				}
			}
		}

		//pre-load values, could be used to create new weapons in the future
		public Weapon(int weaponIndex, string name, WeaponIcon icon, byte hitBonus, byte damage, byte crit, byte spellIndex, byte elementalWeakness, byte typeWeakeness, WeaponSprite weaponTypeSprite, byte weaponSpritePaletteColor)
		{
			WeaponIndex = weaponIndex;
			Name = name;
			Icon = icon;
			HitBonus = hitBonus;
			Damage = damage;
			Crit = crit;
			SpellIndex = spellIndex;
			ElementalWeakness = elementalWeakness;
			TypeWeakness = typeWeakeness;
			WeaponTypeSprite = weaponTypeSprite;
			WeaponSpritePaletteColor = weaponSpritePaletteColor;
		}

		public static IEnumerable<Weapon> LoadAllWeapons(FF1Rom rom, Flags flags)
		{
			int i = flags != null && flags.ExtConsumableSet != ExtConsumableSet.None ? 4 : 0;
			for (; i < 40; i++)
			{
				yield return new Weapon(i, rom);
			}
		}

		public void writeWeaponMemory(FF1Rom rom)
		{
			//weapon stats
			int weaponBaseOffset = FF1Rom.WeaponOffset + (WeaponIndex * FF1Rom.WeaponSize);
			rom.Put(weaponBaseOffset, new byte[] { HitBonus, Damage, Crit, SpellIndex, ElementalWeakness, TypeWeakness, (byte)WeaponTypeSprite, WeaponSpritePaletteColor });

			rom.ItemsText[(int)Item.WoodenNunchucks + WeaponIndex] = Name;
		}

		private WeaponIcon getWeaponIconFromByte(byte icon)
		{
			WeaponIcon matchedType = WeaponIcon.NONE;
			foreach (WeaponIcon temp in (WeaponIcon[])Enum.GetValues(typeof(WeaponIcon)))
			{
				if (icon == (byte)temp)
				{
					matchedType = temp;
				}
			}

			return matchedType;
		}

		private WeaponSprite getWeaponSpriteFromByte(byte sprite)
		{
			WeaponSprite matchedType = WeaponSprite.NONE;
			foreach (WeaponSprite temp in (WeaponSprite[])Enum.GetValues(typeof(WeaponSprite)))
			{
				if (sprite == (byte)temp)
				{
					matchedType = temp;
				}
			}

			return matchedType;
		}

		public static Dictionary<WeaponIcon, string> IconCodes = new Dictionary<WeaponIcon, string>
		{
			{ WeaponIcon.SWORD, "@S" },
			{ WeaponIcon.HAMMER, "@H" },
			{ WeaponIcon.KNIFE, "@K" },
			{ WeaponIcon.AXE, "@X" },
			{ WeaponIcon.STAFF, "@F" },
			{ WeaponIcon.CHUCK, "@N" }
		};
	}
}
