namespace FF1Lib
{
	public enum spellDataBytes
	{
		Accuracy,
		Effect,
		Element,
		Target,
		Routine,
		GFX,
		Palette,
		Null
	}
	public partial class FF1Rom : NesRom
	{
		public enum shopInfoWordsIndex
		{
			wpAtk = 0,
			wpHit,
			wpCrt,
			arDef,
			arEva,
			targetAllEnemies,
			targetSingleEnemy,
			targetCaster,
			targetAllAllies,
			targetOneAlly,
			elementNone,
			elementNoneShort,
			elementStatus,
			elementStatusShort,
			elementPoison,
			elementPoisonShort,
			elementTime,
			elementTimeShort,
			elementDeath,
			elementDeathShort,
			elementFire,
			elementFireShort,
			elementIce,
			elementIceShort,
			elementLit,
			elementLitShort,
			elementEarth,
			elementEarthShort,
			statusDead,
			statusDeadShort,
			statusStone,
			statusStoneShort,
			statusPoison,
			statusPoisonShort,
			statusBlind,
			statusBlindShort,
			statusStun,
			statusStunShort,
			statusSleep,
			statusSleepShort,
			statusMute,
			statusMuteShort,
			statusConfuse,
			statusConfuseShort,
			routineNull,
			routineDamage,
			routineDmgUndead,
			routineStats,
			routineSlow,
			routineFear,
			routineCure,
			routineHealStats,
			routineDefense,
			routineResistElement,
			routineFast,
			routineRaiseAttack,
			routineReduceEvade,
			routineFullCure,
			routineRaiseEvade,
			routineVoidResist,
			routinePowerWord,
			routineHealPoison,
			routineRevive,
			routineFullRevive,
			routineWarp,
			routineHealStone,
			routineTeleport,
			hurtSpecial,
			hurtDragon,
			hurtGiant,
			hurtUndead,
			hurtWere,
			hurtWater,
			hurtMage,
			hurtRegen
		}

		public static List<string> shopInfoWordsList = new List<string> {
			" ATK +", " HIT +", " CRT +", " DEF +", " EVA -", "All Enemies", "Single Enemy", "Caster",
			"All Allies", "One Ally", "None", "None", "Status", "Stat", "Poison", "Pois", "Time", "Time", "Death", "Deat", "Fire", "Fire",
			"Ice", "Ice", "Lightng", "Lit.", "Earth", "Eart", "Dead", "Dead", "Petrified", "Ptr.", "Poisoned", "Pois", "Blind", "Blnd",
			"Paralyzed", "Para", "Asleep", "Slep", "Silenced", "Mute", "Confused", "Conf", "Null", "Damage", "Dmg Undead", "Inflict Stat",
			"Halve Hits", "Reduce Moral", "Recover HP", "Heal Stat", "Raise Def.", "Resist Elem.", "Double Hits", "Raise Attack", "Reduce Evade",
			"Full Recover", "Raise Evade", "Void Resist.", "PW Status", "Heal Poison", "Revive", "Full Revive", "Go one floor\n back",
			"Heal Stoned", "Teleport out\n of dungeons", "Magical", "Dragon", "Giant", "Undead", "Were", "Water", "Mage", "Regen"
		};
		public void ShopUpgrade(Flags flags, DialogueData dialogues, Preferences preferences)
		{
			if (!flags.ShopInfo)
			{
				return;
			}

			// Modify DrawShopPartySprites to use new DrawOBSprite routines, see 0E_9500_ShopUpgrade.asm
			PutInBank(0x0E, 0xAA04, Blob.FromHex("205795"));
			PutInBank(0x0E, 0xAA0D, Blob.FromHex("205795"));
			PutInBank(0x0E, 0xAA16, Blob.FromHex("205795"));
			PutInBank(0x0E, 0xAA23, Blob.FromHex("4C5795"));

			// Extra routines for Shops, plus equip and magic info, see 0E_9500_ShopUpgrade.asm
			// Insert new routines in Common Shop Loop
			PutInBank(0x0E, 0xA931, Blob.FromHex("200095"));
			PutInBank(0x0E, 0x9500, Blob.FromHex("A564C928F038A566C904B035C902B017A20020D495A24020D495A28020D495A2C020D4954C4195A200208B95A240208B95A280208B95A2C0208B954C41952047952027A74C2696A9008DD66A8DDA6A8DDE6A8DE26A6060AA4A8510BD0061A8B9A4EC8511BD0161F011C901F0E9C903F004A9038511A9144C8395A5104A4A4AAABDD66A18651085104C24EC8A8515BD00610AAABD00AD8510BD01AD8511A662BD000338E9B0851229078513A5124A4A4AA8B1108514A613BD38AC2514F005A9004CC595A90E8510A5154A4A4A4AAAA5109DD66A608A8515BD00610AAABDB9BC8512BDBABC8513A662BD000338C944B01638E91C0AAABD50BF25128510BD51BF251305104C1896E9440AAABDA0BF25128510BDA1BF25130510C9019005A9004CC595A90E4CC595A522F033A564C928F02DA662BD00038514205E962027A7A520C561F0F7A9008522204D964C46E1A9018538A9128539A90E853CA90A853D60204D96A90E85572063E0A53E8512A53F8513A514380AAAB00DBD0093853EBD0193853F4C8E96BD0094853EBD0194853FA9118557A90E85582036DEA512853EA513853FA900852260"));

			if (flags.ChestInfo && flags.IdentifyTreasures && !preferences.RenounceChestInfo)
			{
				// Shorten TreasureChest Dialog
				dialogues[320] = "You found.. #";
				dialogues[321] = "Can't hold.. #";

				PutInBank(0x1F, 0xD536, Blob.FromHex("68482064DB4CD0DD"));
				PutInBank(0x1F, 0xDDD0, Blob.FromHex("A9B648A9FF48A9114C03FE8A20DA8760"));
				PutInBank(0x11, 0xB700, Blob.FromHex("A911855768C9F1D004A905D003ADFB602010B8A545D00160A561C91CB00160C96C900160482089C6A9008561A639E88AC91E9002E91E8539A9068D0080A91C8D0180680AAAB00DBD0093853EBD0193853F4C5EB7BD0094853EBD0194853FA9068D0080A9228D0180A200A003B13EC8C902F05AC914F00B9D006BE8C900D0ED4C05B8A53E48A53F489848B13E0AA8B00DB9009A853EB9019A853F4CA7B7B9009B853EB9019B853FA000B13EC8C900F0079D006BE84CA9B7A9068D0080A9228D018068A8C868853F68853E4C6CB7A53E48A53F489848B13E0AA8A9068D0080A9158D0180B010B90097853EB9019738E920853F4CA7B7B90098853EB9019838E920853F4CA7B7A900853EA96B853F4C8ADBA23F8E0620A20E8E06208D0720A23F8E0620A2008E06208E06208E062020A1CC60"));
				if (preferences.RenounceCantHoldRed) PutInBank(0x11, 0xB707, Blob.FromHex("EAEAEAEAEAEA"));

				if (flags.ExtConsumableSet != ExtConsumableSet.None) PutInBank(0x11, 0xB71B, Blob.FromHex("20"));
			}

			// Patch in the equip menu loop to add gear info
			PutInBank(0x0E, 0xBB8F, Blob.FromHex("4CE090EA"));
			// Patch in the magic menu loop to add spell info
			if (!flags.MagicMenuSpellReordering)
			{
				//MagicMenuSpellReordering overwrites this jump
				PutInBank(0x0E, 0xAECD, Blob.FromHex("4C2691EA"));
			}
			// the UpgradedEquipMenu and UpgradedMagicMenu code that the above patches jump to
			PutInBank(0x0E, 0x90E0, Blob.FromHex("A525D007A522D0044C93BB60A662BD0003F030297FA466C018D005691A4C029169428514203CC4205E9620F9BCA520C561F0F7A9008D0120853720F3BD2083B720DAEC4C93BBA525D007A522D0044CD1AE60A9018537A5664A6A6A0562AA0A2900187D00631869AF8514205E962080B72025B6A9008D01208537857F20029CA56248206DBA688562A90720EFB8A9292059B92080B74CD1AE"));

			// Modify DrawComplexString, this sets control code 14-19 to use a new words table in bank 11
			//  could be used to move some stuff in items name table and make some space
			//  see 1F_DEBC_DrawComplexString.asm
			PutInBank(0x1F, 0xDEBC, Blob.FromHex("C910B005A2204C83DEC914B07B")); // Change branching to enable CC14
			PutInBank(0x1F, 0xDF44, Blob.FromHex("4CCEDF")); // Jump to routine because we're too far, put in unused char weapons CC
			PutInBank(0x1F, 0xDFCE, Blob.FromHex("A91185572003FE4CA099")); // Routine, put in unused char weapons routine

			// Routine to load the right word from the new words table
			PutInBank(0x11, 0x99A0, Blob.FromHex("A91185578558B13EE63ED002E63F203EE00AAAB00BBD009A853EBD019A4CC899BD009B853EBD019B853F2045DEA90E85584C4EE0"));

			const int weaponOffset = 0x1C; // $28 entries
			const int armorOffset = 0x44; // $28 entries
			const int spellOffset = 0xB0; // $40 entries

			var weaponsData = new List<Weapon>();
			var armorsData = new List<Armor>();

			for (int i = 0; i < WeaponCount; i++)
				weaponsData.Add(new Weapon(i, this));

			for (int i = 0; i < ArmorCount; i++)
				armorsData.Add(new Armor(i, this));

			var spellsData = GetSpells();

			// 12 char per row, 5 rows
			var descriptionsList = new List<string>();

			for (int i = 0; i < weaponOffset; i++)
				descriptionsList.Add("");

			// Insert the new words table
			int offsetWordsPointers = 0x9A00;
			int offsetWords = 0x9B00;
			var pointersWords = new ushort[shopInfoWordsList.Count()];
			Blob generatedWords = Blob.FromHex("");

			for (int i = 0; i < shopInfoWordsList.Count(); i++)
			{
				var blob = FF1Text.TextToBytes(shopInfoWordsList[i], useDTE: true);

				generatedWords += blob;

				pointersWords[i] = (ushort)(offsetWords);
				offsetWords += blob.Length;
			}

			PutInBank(0x11, 0x9B00, generatedWords);
			PutInBank(0x11, offsetWordsPointers, Blob.FromUShorts(pointersWords));

			// Build the info boxes
			for (int i = weaponOffset; i < armorOffset; i++)
				descriptionsList.Add("\n" + GenerateWeaponDescription(i - weaponOffset, preferences.ShopInfoIcons));

			for (int i = armorOffset; i < (armorOffset + 0x28); i++)
				descriptionsList.Add("\n" + GenerateArmorDescription(i - armorOffset, preferences.ShopInfoIcons));

			for (int i = (armorOffset + 0x28); i < spellOffset; i++)
				descriptionsList.Add("");

			for (int i = spellOffset; i < spellOffset + 0x40; i++)
				descriptionsList.Add("" + GenerateSpellDescription(i, spellsData[i - spellOffset].Data, preferences.ShopInfoIcons));

			// Convert all dialogues to bytes
			int offset = 0xA000;
			var pointers = new ushort[descriptionsList.Count()];
			Blob generatedText = Blob.FromHex("");

			for (int i = 0; i < descriptionsList.Count(); i++)
			{
				var blob = new byte[] { 0x02, (byte)i } + FF1Text.TextToBytesInfo(descriptionsList[i], useDTE: true);
				if (blob.Length <= 3)
					blob = new byte[0];

				generatedText += blob;

				pointers[i] = (ushort)(offset);
				offset += blob.Length;
			}

			// Check if dialogs are too long
			if (generatedText.Length > 0x1400)
				throw new Exception("ShopInfo text size maximum exceeded.");

			// Insert dialogs
			PutInBank(0x11, 0xA000, generatedText);
			PutInBank(0x0E, 0x9300, Blob.FromUShorts(pointers));

		}

		public ushort[] InfoClassEquipPerms = new ushort[] {
		    (ushort)EquipPermission.Fighter,
		    (ushort)EquipPermission.Knight,
		    (ushort)EquipPermission.Thief,
		    (ushort)EquipPermission.Ninja,
		    (ushort)EquipPermission.BlackBelt,
		    (ushort)EquipPermission.Master,
		    (ushort)EquipPermission.RedMage,
		    (ushort)EquipPermission.RedWizard,
		    (ushort)EquipPermission.WhiteMage,
		    (ushort)EquipPermission.WhiteWizard,
		    (ushort)EquipPermission.BlackMage,
		    (ushort)EquipPermission.BlackWizard,
		};
		public string[] InfoClassAbbrev = new string[] {
		    "Fi",
		    "Kn",
		    "Th",
		    "Ni",
		    "Bb",
		    "Ma",
		    "Rm",
		    "Rw",
		    "Wm",
		    "Ww",
		    "Bm",
		    "Bw"
		};

		public string GenerateEquipPermission(int classUsability) {
		    var description = "";
		    for (int i = 0; i < 6; i++) {
			if ((classUsability & InfoClassEquipPerms[i*2]) != 0)  {
			    description += " " + InfoClassAbbrev[i*2];
			} else if ((classUsability & InfoClassEquipPerms[i*2+1]) != 0) {
			    description += " " + InfoClassAbbrev[i*2+1];
			}
		    }
			description = description.Trim();
			if (description.Length > 12)
			{
				description = description.Replace(" ", "");
			}
			if (description == "FiThBbRmWmBm") {
			description =  "All classes";
		    }
		    return description;
		}

		public string GenerateWeaponDescription(int weaponid, bool iconsEnabled)
		{
			const int spellOffset = 0xB0; // $40 entries

			List<(int, string, string)> element = new();

			if (iconsEnabled)
			{
				element = new List<(int, string, string)> {
					(0x01, "€s", "€s"),
					(0x02, "€p", "€p"),
					(0x04, "€T", "€T"),
					(0x08, "€d", "€d"),
					(0x10, "€f", "€f"),
					(0x20, "€i", "€i"),
					(0x40, "€t", "€t"),
					(0x80, "€e", "€e"),
				};
			}
			else
			{
				element = new List<(int, string, string)> {
					(0x01, "¤" + ((int)shopInfoWordsIndex.elementStatus).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementStatusShort).ToString("X2")),
					(0x02, "¤" + ((int)shopInfoWordsIndex.elementPoison).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementPoisonShort).ToString("X2")),
					(0x04, "¤" + ((int)shopInfoWordsIndex.elementTime).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementTimeShort).ToString("X2")),
					(0x08, "¤" + ((int)shopInfoWordsIndex.elementDeath).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementDeathShort).ToString("X2")),
					(0x10, "¤" + ((int)shopInfoWordsIndex.elementFire).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementFireShort).ToString("X2")),
					(0x20, "¤" + ((int)shopInfoWordsIndex.elementIce).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementIceShort).ToString("X2")),
					(0x40, "¤" + ((int)shopInfoWordsIndex.elementLit).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementLitShort).ToString("X2")),
					(0x80, "¤" + ((int)shopInfoWordsIndex.elementEarth).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementEarthShort).ToString("X2"))
				};
			}

			List<(int, string)> hurt = new() {
			    (0x00, "¤" + ((int)shopInfoWordsIndex.elementNone).ToString("X2")),
			    (0x01, "¤" + ((int)shopInfoWordsIndex.hurtSpecial).ToString("X2")),
			    (0x02, "¤" + ((int)shopInfoWordsIndex.hurtDragon).ToString("X2")),
			    (0x04, "¤" + ((int)shopInfoWordsIndex.hurtGiant).ToString("X2")),
			    (0x08, "¤" + ((int)shopInfoWordsIndex.hurtUndead).ToString("X2")),
			    (0x10, "¤" + ((int)shopInfoWordsIndex.hurtWere).ToString("X2")),
			    (0x20, "¤" + ((int)shopInfoWordsIndex.hurtWater).ToString("X2")),
			    (0x40, "¤" + ((int)shopInfoWordsIndex.hurtMage).ToString("X2")),
			    (0x80, "¤" + ((int)shopInfoWordsIndex.hurtRegen).ToString("X2")) };

			var weapondata = new Weapon(weaponid, this);

			var description = "¤" + ((int)shopInfoWordsIndex.wpAtk).ToString("X2") + weapondata.Damage + "\n¤" + ((int)shopInfoWordsIndex.wpHit).ToString("X2") + weapondata.HitBonus + "\n¤" + ((int)shopInfoWordsIndex.wpCrt).ToString("X2") + weapondata.Crit;

			var activeElement = element.Where(x => (x.Item1 & weapondata.ElementalWeakness) > 0).ToList();
			var activeHurt = hurt.Where(x => (x.Item1 & weapondata.TypeWeakness) > 0).ToList();

			bool showElement = (weapondata.SpellIndex == 0x00) || (activeHurt.Count == 0);

			description += "\n";
			description += GenerateEquipPermission(WeaponPermissions[weapondata.Id]);

			if (activeHurt.Count > 0)
			{
				description += (activeHurt.Count < 8 ? "\nHurt " + activeHurt.First().Item2 : "\nHurt All");
			}

			if (activeElement.Any() && showElement == true)
			{
				if (iconsEnabled)
				{
					description += (activeElement.Count > 4 ? "\nEle " : "\nElement ") + string.Join("", activeElement.Select(x => x.Item2));
				}
				else
				{
					description += (activeElement.Count < 8 ? "\n" + activeElement.First().Item3 + " Element" : "\nAll Elements");
				}
			}

			if (weapondata.SpellIndex != 0x00)
				description += "\n" + "Cast $" + ((int)weapondata.SpellIndex + spellOffset - 1).ToString("X2");

			return description;
		}
		public string GenerateArmorDescription(int armorid, bool iconsEnabled)
		{
			const int spellOffset = 0xB0; // $40 entries

			List<(int, string, string)> element = new();

			if (iconsEnabled)
			{
				element = new List<(int, string, string)> {
					(0x01, "€s", "€s"),
					(0x02, "€p", "€p"),
					(0x04, "€T", "€T"),
					(0x08, "€d", "€d"),
					(0x10, "€f", "€f"),
					(0x20, "€i", "€i"),
					(0x40, "€t", "€t"),
					(0x80, "€e", "€e"),
				};
			}
			else
			{
				element = new List<(int, string, string)> {
					(0x01, "¤" + ((int)shopInfoWordsIndex.elementStatus).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementStatusShort).ToString("X2")),
					(0x02, "¤" + ((int)shopInfoWordsIndex.elementPoison).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementPoisonShort).ToString("X2")),
					(0x04, "¤" + ((int)shopInfoWordsIndex.elementTime).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementTimeShort).ToString("X2")),
					(0x08, "¤" + ((int)shopInfoWordsIndex.elementDeath).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementDeathShort).ToString("X2")),
					(0x10, "¤" + ((int)shopInfoWordsIndex.elementFire).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementFireShort).ToString("X2")),
					(0x20, "¤" + ((int)shopInfoWordsIndex.elementIce).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementIceShort).ToString("X2")),
					(0x40, "¤" + ((int)shopInfoWordsIndex.elementLit).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementLitShort).ToString("X2")),
					(0x80, "¤" + ((int)shopInfoWordsIndex.elementEarth).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementEarthShort).ToString("X2"))
				};
			}

			var shortDelimiter = new List<string> { "\n ", ", ", "\n ", ", ", "\n ", ", " };

			var armordata = new Armor(armorid, this);

			var description = "¤" + ((int)shopInfoWordsIndex.arDef).ToString("X2") + armordata.Absorb + "\n¤" + ((int)shopInfoWordsIndex.arEva).ToString("X2") + armordata.Weight;

			description += "\n";
			description += GenerateEquipPermission(ArmorPermissions[armordata.Id]);

			var activeElement = element.Where(x => (x.Item1 & armordata.ElementalResist) > 0).ToList();

			if (activeElement.Any())
			{
				if (iconsEnabled)
				{
					description += "\nResistance\n " + string.Join("", activeElement.Select(x => x.Item2));
				}
				else
				{
					if (activeElement.Count == 1)
					{
						description += "\nResistance\n " + activeElement[0].Item2;
					}
					else if (activeElement.Count <= 3)
					{
						description += "\nResist " + activeElement[0].Item3;

						for (int i = 1; i < activeElement.Count; i++)
						{
							description += shortDelimiter[i - 1] + activeElement[i].Item3;
						}
					}
					else if (activeElement.Count <= 7)
					{
						description += "\nResist " + activeElement[0].Item3 + "\n " + activeElement[1].Item3 + " and " + (activeElement.Count - 2) + "+";
					}
					else
					{
						description += "\nResist all";
					}
				}
			}

			if (armordata.SpellIndex != 0x00)
				description += "\n" + "Cast $" + ((int)armordata.SpellIndex + spellOffset - 1).ToString("X2");

			return description;
		}
		public string GenerateSpellDescription(int spellid, Blob spelldata, bool iconsEnabled)
		{
			List<(int, string)> target = new()  {
			    (0x01, "¤" + ((int)shopInfoWordsIndex.targetAllEnemies).ToString("X2")),
			    (0x02, "¤" + ((int)shopInfoWordsIndex.targetSingleEnemy).ToString("X2")),
			    (0x04, "¤" + ((int)shopInfoWordsIndex.targetCaster).ToString("X2")),
			    (0x08, "¤" + ((int)shopInfoWordsIndex.targetAllAllies).ToString("X2")),
			    (0x10, "¤" + ((int)shopInfoWordsIndex.targetOneAlly).ToString("X2"))
			};

			List<(int, string, string)> element = new();

			if (iconsEnabled)
			{
				element = new List<(int, string, string)> {
					(0x01, "€s", "€s"),
					(0x02, "€p", "€p"),
					(0x04, "€T", "€T"),
					(0x08, "€d", "€d"),
					(0x10, "€f", "€f"),
					(0x20, "€i", "€i"),
					(0x40, "€t", "€t"),
					(0x80, "€e", "€e"),
				};
			}
			else
			{
				element = new List<(int, string, string)> {
					(0x01, "¤" + ((int)shopInfoWordsIndex.elementStatus).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementStatusShort).ToString("X2")),
					(0x02, "¤" + ((int)shopInfoWordsIndex.elementPoison).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementPoisonShort).ToString("X2")),
					(0x04, "¤" + ((int)shopInfoWordsIndex.elementTime).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementTimeShort).ToString("X2")),
					(0x08, "¤" + ((int)shopInfoWordsIndex.elementDeath).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementDeathShort).ToString("X2")),
					(0x10, "¤" + ((int)shopInfoWordsIndex.elementFire).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementFireShort).ToString("X2")),
					(0x20, "¤" + ((int)shopInfoWordsIndex.elementIce).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementIceShort).ToString("X2")),
					(0x40, "¤" + ((int)shopInfoWordsIndex.elementLit).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementLitShort).ToString("X2")),
					(0x80, "¤" + ((int)shopInfoWordsIndex.elementEarth).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.elementEarthShort).ToString("X2"))
				};
			}

			List<(int, string, string)> status = new();

			if (iconsEnabled)
			{
				status = new List<(int, string, string)> {
					(0x01, "§d", "§d"),
					(0x02, "§s", "§s"),
					(0x04, "§p", "§p"),
					(0x08, "§b", "§b"),
					(0x10, "§P", "§P"),
					(0x20, "§Z", "§Z"),
					(0x40, "§M", "§M"),
					(0x80, "§C", "§C")
				};
			}
			else
			{
				status = new List<(int, string, string)> {
					(0x01, "¤" + ((int)shopInfoWordsIndex.statusDead).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusDeadShort).ToString("X2")),
					(0x02, "¤" + ((int)shopInfoWordsIndex.statusStone).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusStoneShort).ToString("X2")),
					(0x04, "¤" + ((int)shopInfoWordsIndex.statusPoison).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusPoisonShort).ToString("X2")),
					(0x08, "¤" + ((int)shopInfoWordsIndex.statusBlind).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusBlindShort).ToString("X2")),
					(0x10, "¤" + ((int)shopInfoWordsIndex.statusStun).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusStunShort).ToString("X2")),
					(0x20, "¤" + ((int)shopInfoWordsIndex.statusSleep).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusSleepShort).ToString("X2")),
					(0x40, "¤" + ((int)shopInfoWordsIndex.statusMute).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusMuteShort).ToString("X2")),
					(0x80, "¤" + ((int)shopInfoWordsIndex.statusConfuse).ToString("X2"), "¤" + ((int)shopInfoWordsIndex.statusConfuseShort).ToString("X2"))
				};
			}

			List<(int, string)> routine = new() {
			    (0x00, "¤" + ((int)shopInfoWordsIndex.routineNull).ToString("X2")),
			    (0x01, "¤" + ((int)shopInfoWordsIndex.routineDamage).ToString("X2")),
			    (0x02, "¤" + ((int)shopInfoWordsIndex.routineDmgUndead).ToString("X2")),
			    (0x03, "¤" + ((int)shopInfoWordsIndex.routineStats).ToString("X2")),
			    (0x04, "¤" + ((int)shopInfoWordsIndex.routineSlow).ToString("X2")),
			    (0x05, "¤" + ((int)shopInfoWordsIndex.routineFear).ToString("X2")),
			    (0x06, "¤" + ((int)shopInfoWordsIndex.routineCure).ToString("X2")),
			    (0x07, "¤" + ((int)shopInfoWordsIndex.routineCure).ToString("X2")),
			    (0x08, "¤" + ((int)shopInfoWordsIndex.routineHealStats).ToString("X2")),
			    (0x09, "¤" + ((int)shopInfoWordsIndex.routineDefense).ToString("X2")),
			    (0x0A, "¤" + ((int)shopInfoWordsIndex.routineResistElement).ToString("X2")),
			    (0x0C, "¤" + ((int)shopInfoWordsIndex.routineFast).ToString("X2")),
			    (0x0D, "¤" + ((int)shopInfoWordsIndex.routineRaiseAttack).ToString("X2")),
			    (0x0E, "¤" + ((int)shopInfoWordsIndex.routineReduceEvade).ToString("X2")),
			    (0x0F, "¤" + ((int)shopInfoWordsIndex.routineFullCure).ToString("X2")),
			    (0x10, "¤" + ((int)shopInfoWordsIndex.routineRaiseEvade).ToString("X2")),
			    (0x11, "¤" + ((int)shopInfoWordsIndex.routineVoidResist).ToString("X2")),
			    (0x12, "¤" + ((int)shopInfoWordsIndex.routinePowerWord).ToString("X2"))
			};
			List<(int, string)> oobroutine = new() {
			    (0x00, "¤" + ((int)shopInfoWordsIndex.routineCure).ToString("X2")),
			    (0x01, "¤" + ((int)shopInfoWordsIndex.routineCure).ToString("X2")),
			    (0x02, "¤" + ((int)shopInfoWordsIndex.routineCure).ToString("X2")),
			    (0x03, "¤" + ((int)shopInfoWordsIndex.routineFullCure).ToString("X2")),
			    (0x04, "¤" + ((int)shopInfoWordsIndex.routineCure).ToString("X2")),
			    (0x05, "¤" + ((int)shopInfoWordsIndex.routineCure).ToString("X2")),
			    (0x06, "¤" + ((int)shopInfoWordsIndex.routineCure).ToString("X2")),
			    (0x07, "¤" + ((int)shopInfoWordsIndex.routineHealPoison).ToString("X2")),
			    (0x08, "¤" + ((int)shopInfoWordsIndex.routineRevive).ToString("X2")),
			    (0x09, "¤" + ((int)shopInfoWordsIndex.routineFullRevive).ToString("X2")),
			    (0x0A, "¤" + ((int)shopInfoWordsIndex.routineWarp).ToString("X2")),
			    (0x0B, "¤" + ((int)shopInfoWordsIndex.routineHealStone).ToString("X2")),
			    (0x0C, "¤" + ((int)shopInfoWordsIndex.routineTeleport).ToString("X2"))
			};
			var shortDelimiter = new List<string> { "\n ", ", ", "\n ", ", ", "\n ", ", " };
			var oobSpells = new List<int>();

			for (int i = 0; i < oobroutine.Count; i++)
				oobSpells.Add(Get(MagicOutOfBattleOffset + MagicOutOfBattleSize * i, 1)[0]);

			var routineDesc = "";

			switch ((int)spelldata[(int)spellDataBytes.Routine])
			{
				case 0:
					routineDesc = oobroutine.Find(x => x.Item1 == oobSpells.FindIndex(x => x == spellid)).Item2;
					break;
				case int n when (n >= 0x01 && n <= 0x02):
					routineDesc = routine.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Routine]).Item2 + "\n " + spelldata[(int)spellDataBytes.Effect] * 2 + "-" + spelldata[(int)spellDataBytes.Effect] * 4 + " DMG\nResist  " + spelldata[(int)spellDataBytes.Effect] + "\nWeak    " + spelldata[(int)spellDataBytes.Effect] * 1.5 * 4;
					break;
				case int n when (n == 0x03 || n == 0x08 || n == 0x12):
					var activeStatus = status.Where(x => (x.Item1 & spelldata[(int)spellDataBytes.Effect]) > 0).ToList();
					var statusString = "";

					if (activeStatus.Any())
					{
						if (iconsEnabled)
						{
							statusString = "\n " + string.Join("", activeStatus.Select(x => x.Item2));
						}
						else
						{
							if (activeStatus.Count <= 3)
							{
								statusString = string.Join(string.Empty, activeStatus.SelectMany(x => "\n " + x.Item2));
							}
							else if (activeStatus.Count <= 6)
							{
								for (int i = 0; i < activeStatus.Count; i++)
								{
									statusString += shortDelimiter[i] + activeStatus[i].Item3;
								}
							}
							else if (activeStatus.Count == 7)
							{
								statusString = "\n All, except\n " + status.Find(x => (x.Item1 & spelldata[(int)spellDataBytes.Effect]) == 0).Item2;
							}
							else
							{
								statusString = "\n All";
							}
						}
					}
					else
					{
						statusString = "\n None";
					}

					routineDesc = routine.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Routine]).Item2 + statusString;
					break;
				case int n when (n == 0x04 || n == 0x0C || n == 0x0F || n == 0x11):
					routineDesc = routine.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Routine]).Item2;
					break;
				case int n when (n == 0x05 || n == 0x0E):
					routineDesc = routine.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Routine]).Item2 + "\n -" + spelldata[(int)spellDataBytes.Effect] + " pts";
					break;
				case int n when (n >= 0x06 && n <= 0x07):
					routineDesc = routine.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Routine]).Item2 + "\n " + spelldata[(int)spellDataBytes.Effect] + "-" + spelldata[(int)spellDataBytes.Effect] * 2 + " HP";
					break;
				case int n when (n == 0x09 || n == 0x10):
					routineDesc = routine.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Routine]).Item2 + "\n +" + spelldata[(int)spellDataBytes.Effect] + " pts";
					break;
				case int n when (n == 0x0A):
					var activeElement = element.Where(x => (x.Item1 & spelldata[(int)spellDataBytes.Effect]) > 0).ToList();
					var elementString = "";

					if (activeElement.Any())
					{
						if (iconsEnabled)
						{
							elementString = "\n " + string.Join("", activeElement.Select(x => x.Item2));
						}
						else
						{
							if (activeElement.Count <= 3)
							{
								elementString = string.Join(string.Empty, activeElement.SelectMany(x => "\n " + x.Item2));
							}
							else if (activeElement.Count <= 6)
							{
								for (int i = 0; i < activeElement.Count; i++)
								{
									elementString += shortDelimiter[i] + activeElement[i].Item3;
								}
							}
							else if (activeElement.Count == 7)
							{
								elementString = "\n All, except\n " + element.Find(x => (x.Item1 & spelldata[(int)spellDataBytes.Effect]) == 0).Item2;
							}
							else
							{
								elementString = "\n All";
							}
						}
					}
					else
					{
						elementString = "\n None";
					}

					routineDesc = routine.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Routine]).Item2 + elementString;
					break;
				case int n when (n == 0x0D):
					routineDesc = routine.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Routine]).Item2 + "\n +" + spelldata[(int)spellDataBytes.Effect] + " ATK\n +" + spelldata[(int)spellDataBytes.Accuracy] + " HIT";
					break;
			}

			var spellElement = element.Where(x => (x.Item1 & spelldata[(int)spellDataBytes.Element]) > 0).ToList();
			var spellElementString = "";

			if (spellElement.Any())
			{
				if (iconsEnabled)
				{
					if (spellElement.Count < 8)
					{
						spellElementString = " " + string.Join("", spellElement.Select(x => x.Item2));
					}
					else
					{
						spellElementString = string.Join("", spellElement.Select(x => x.Item2));
					}
				}
				else
				{
					spellElementString = (spelldata[(int)spellDataBytes.Element] == 0x40 ? " " : "  ") + element.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Element]).Item2;
				}
			}

			var spellstring = spellElementString + "\n" + target.Find(x => x.Item1 == spelldata[(int)spellDataBytes.Target]).Item2 + "\n\n" + routineDesc;
			return spellstring;
		}
	}
}
