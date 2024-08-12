using System.ComponentModel;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public enum ExtConsumableSet
	{
		[Description("None")]
		None,

		[Description("FCURE, PHNIX, BLAST, SMOKE")]
		SetA,

		[Description("RFRSH, FLARE, BLACK, GUARD")]
		SetB,

		[Description("QUICK, HIGH, WIZRD, CLOAK")]
		SetC
	}

	public class ExtConsumables
	{
		FF1Rom rom;
		Flags flags;
		MT19337 rng;

		Dictionary<string, MagicSpell> Spells;

		List<SpellInfo> SpellInfos;

		ShopData ShopData;

		public ExtConsumables(ShopData _shopData, FF1Rom _rom, Flags _flags, MT19337 _rng)
		{
			rom = _rom;
			flags = _flags;
			rng = _rng;
			ShopData = _shopData;
		}

		public void LoadSpells()
		{
			SpellInfos = rom.LoadSpells().ToList();
			Spells = rom.GetSpells().ToDictionary(s => s.Name.ToLowerInvariant());
		}

		public void AddExtConsumables()
		{
			ChangeItemJumpTable();

			WriteOutOfBattleRoutines();

			WriteDrawItemBoxEndOfItemIndex();

			ChangeItemNames();
			ChangeMenuTexts();

			try
			{
				WriteDrawDrinkBoxBreakoutRoutine();
				WriteDrawDrinkBoxRoutine();
				WriteLutDrinkBoxOrder();
				WriteLutDrinkBoxEffect();
				WriteLutDrinkBoxTarget();
				WriteLutDrinkBox();
				WriteCursorPositions();
			}
			catch (InvalidOperationException)
			{
				if ((bool)this.flags.GenerateNewSpellbook)
				{
					_ = rom.Progress("Error generating extended consumables with spellcrafter enabled. Please select a different seed or disable \"Generate New Spellbooks\" and/or \"New Consumables\".");
					throw;
				}
				else
				{
					throw;
				}
			}

			WriteDrinkBoxMenuRoutine();
			ModifyBattle_PlMag_TargetOnePlayer();
			WriteCureAilmentsBreakout();

			ModifyUndoBattleCommand();
			ModifyAfterFadeIn();
			WriteBattleDoTurn();
			ModifyBattleLogicLoop();

			if (flags.ExtConsumableSet == ExtConsumableSet.SetA)
			{
				CreateLifeSpell();
				CreateSmokeSpell();
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetB)
			{
				CreateBlackPotionSpell();
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetC)
			{
				CreateHighPotionSpell();
			}

			ClearShops();
		}		

		private void ChangeItemJumpTable()
		{
			//we read a few more values for convenience, so that the size of the array is right.
			var table = rom.GetFromBank(0x0E, 0xB177, 72).ToUShorts();

			//I need the menu texts, so i cut them here
			var useitem_bad = table[0];
			table[(int)Item.Crystal] = useitem_bad;
			table[(int)Item.Herb] = useitem_bad;
			table[(int)Item.Key] = useitem_bad;
			table[(int)Item.Tnt] = useitem_bad;

			if (flags.ExtConsumableSet == ExtConsumableSet.SetB)
			{
				table[(int)Item.WoodenNunchucks] = 0x96F8;
				table[(int)Item.SmallKnife] = 0x9790;
				table[(int)Item.WoodenRod] = 0x9748;
				table[(int)Item.Rapier] = 0x9795;
			}
			else
			{
				table[(int)Item.WoodenNunchucks] = 0x96F8;
				table[(int)Item.SmallKnife] = 0x9748;
				table[(int)Item.WoodenRod] = 0x9790;
				table[(int)Item.Rapier] = 0x9795;
			}

			rom.PutInBank(0x0E, 0x96B0, Blob.FromUShorts(table));

			//exchange affected LDAs
			rom.PutInBank(0x0E, 0xB16A, Blob.FromHex("BDB096"));
			rom.PutInBank(0x0E, 0xB16F, Blob.FromHex("BDB196"));
		}

		private void WriteOutOfBattleRoutines()
		{
			if (flags.ExtConsumableSet == ExtConsumableSet.SetA)
			{
				WriteOutOfBattleFCureRoutine();
				WriteOutOfBattlePhnixRoutine();
				WriteOutOfBattleGeneric3Routine();
				WriteOutOfBattleGeneric4Routine();
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetB)
			{
				WriteOutOfBattleRefreshRoutine();
				WriteOutOfBattleFlareRoutine();
				WriteOutOfBattleBlackRoutine();
				WriteOutOfBattleGeneric4Routine();
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetC)
			{
				WriteOutOfBattleGeneric1Routine();
				WriteOutOfBattleHighRoutine();
				WriteOutOfBattleGeneric3Routine();
				WriteOutOfBattleGeneric4Routine();
			}
		}

		private void WriteOutOfBattleFCureRoutine()
		{
			rom.PutInBank(0x0E,0x96F8,Blob.FromHex("2000B4A908202BB920A0B3B028A5626A6A6A29C0AABD0161C901F01CC902F018BD0C619D0A61BD0D619D0B612000B42013B6CE3C604C1DB12026DB4C0097"));
		}

		private void WriteOutOfBattlePhnixRoutine()
		{
			rom.PutInBank(0x0E,0x9748,Blob.FromHex("2000B4A909202BB920A0B3B01FA5626A6A6A29C0AAA90185102088B3B011A9642061B52000B42013B6CE3D604C1DB12026DB4C5097"));
		}

		private void WriteOutOfBattleRefreshRoutine()
		{
			rom.PutInBank(0x0E,0x96F8,Blob.FromHex("2000B4A908202BB9203CC42025B6A5202980F022A200BD0161C901F009C902F005A9402061B58A186940AA90E92000B42013B6CE3C604C1DB1"));
		}

		private void WriteOutOfBattleBlackRoutine()
		{
			rom.PutInBank(0x0E,0x9748,Blob.FromHex("2000B4A90A202BB920A0B3B01EA5626A6A6A29C0AAA9019D0161A9009D0A619D0B612000B42013B6CE3E604C1DB12026DB4C5097"));
		}

		private void WriteOutOfBattleHighRoutine()
		{
			rom.PutInBank(0x0E,0x9748,Blob.FromHex("2000B4A909202BB920A0B3B021A5626A6A6A29C0AABD0161C901F015C902F011A9A02061B52000B42013B6CE3D604C1DB12026DB4C5097"));
		}

		private void WriteOutOfBattleFlareRoutine()
		{
			rom.PutInBank(0x0E, 0x9790, Blob.FromHex("A9094CB8B1"));
		}

		private void WriteOutOfBattleGeneric1Routine()
		{
			rom.PutInBank(0x0E, 0x96F8, Blob.FromHex("A9084CB8B1"));
		}

		private void WriteOutOfBattleGeneric2Routine()
		{
			rom.PutInBank(0x0E, 0x9748, Blob.FromHex("A9094CB8B1"));
		}

		private void WriteOutOfBattleGeneric3Routine()
		{
			rom.PutInBank(0x0E, 0x9790, Blob.FromHex("A90A4CB8B1"));
		}

		private void WriteOutOfBattleGeneric4Routine()
		{
			rom.PutInBank(0x0E, 0x9795, Blob.FromHex("A90B4CB8B1"));
		}

		private void WriteDrawDrinkBoxBreakoutRoutine()
		{
			rom.PutInBank(0x1F, 0xF921, Blob.FromHex("A91C2003FE2020A0A90C2003FE60"));
		}

		private void WriteDrawItemBoxEndOfItemIndex()
		{
			if (flags.ExtConsumableSet != ExtConsumableSet.None)
			{
				//DrawItemBox EndOfItemIndex
				rom.PutInBank(0x1F, 0xEF4D, new byte[] { 0x20 });
			}
		}

		private void WriteDrawDrinkBoxRoutine()
		{
			rom.PutInBank(0x1C,0xA020,Blob.FromHex("A005B90FA0999D6A88D0F72090F62057F7EE9E6AEE9F6AEEA06AA200A0002053A0A0182053A0A0302053A0A0482053A04C48F69848207AA0E8207AA0E8A900991A6B1868691A8DA16AA96B69008DA26A2090F6EEA06AEEA06A608A48BD30F948F034AABD2060F02EAAC8C8A90E991A6BC868991A6B8AC964B001C88AC90AB001C8C8A911991A6BC88A991A6BC8A900991A6BC868AA60A910991A6BC8A90A991A6BC86868AA60AD7468F037AD6D682D7468F024AD746849FF2D6D688D6D68AD7468C901F040AD7468C981F049A9B948A9D548A90C4C03FEA9B948A9D748A90C4C03FE686868686868A9A448A92348A56AC973F02CC974F028C975F024C976F020A90C4C03FEA9108D7468A9B948A99848A90C4C03FEA9B948A9C548A90C4C03FEA5334AB00C4AB00E4AB0104AB0124C20A1C6294C20A1E6294C20A1C62A4C20A1E62A4C20A1"));
		}

		private void WriteLutDrinkBoxOrder()
		{
			string blob = "191A";
			blob += flags.EnableSoftInBattle ? "1B" : "00";
			blob += "00";
			if (flags.ExtConsumableSet == ExtConsumableSet.SetA)
			{
				blob += "1C";
				blob += (flags.EnableLifeInBattle != LifeInBattleSetting.LifeInBattleOff) ? "1D" : "00";
				blob += "1E1F";
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetB)
			{
				blob += "1C1D1E1F";
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetC)
			{
				blob += "1C1D1E1F";
			}
			else
			{
				blob += "00000000";
			}

			rom.PutInBank(0x1F, 0xF930, Blob.FromHex(blob));
		}

		private void WriteLutDrinkBoxEffect()
		{
			if (flags.ExtConsumableSet == ExtConsumableSet.None)
			{
				var data = new byte[] { FindLowSpell("CURE", "CUR"), FindLowSpell("PURE", "PUR"), FindLowSpell("SOFT", "SFT"), 0x00, 0x00, 0x00, 0x00, 0x00 };
				rom.PutInBank(0x1F, 0xF938, data);
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetA)
			{
				var data = new byte[] { FindLowSpell("CURE", "CUR"), FindLowSpell("PURE", "PUR"), FindLowSpell("SOFT", "SFT"), 0x00, FindHighSpell("CUR4", "CUR"), 0x40, FindBlastSpell(), 0x41 };
				rom.PutInBank(0x1F, 0xF938, data);
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetB)
			{
				var data = new byte[] { FindLowSpell("CURE", "CUR"), FindLowSpell("PURE", "PUR"), FindLowSpell("SOFT", "SFT"), 0x00, FindHighSpell("HEL3", "HEL"), 0x56, 0x40, FindGuardSpell() };
				rom.PutInBank(0x1F, 0xF938, data);
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetC)
			{
				var data = new byte[] { FindLowSpell("CURE", "CUR"), FindLowSpell("PURE", "PUR"), FindLowSpell("SOFT", "SFT"), 0x00, FindHighSpell("FAST", "FST"), 0x40, FindConfSpell(), FindRuseSpell() };
				rom.PutInBank(0x1F, 0xF938, data);
			}
		}

		private void WriteLutDrinkBoxTarget()
		{
			if (flags.ExtConsumableSet == ExtConsumableSet.None)
			{
				rom.PutInBank(0x1F, 0xF940, Blob.FromHex("0101010000000000"));
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetA)
			{
				rom.PutInBank(0x1F, 0xF940, Blob.FromHex("0101010001010000"));
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetB)
			{
				rom.PutInBank(0x1F, 0xF940, Blob.FromHex("0101010000000000"));
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetC)
			{
				rom.PutInBank(0x1F, 0xF940, Blob.FromHex("0101010001010000"));
			}
		}

		private void WriteLutDrinkBox()
		{
			rom.PutInBank(0x1C, 0xA010, Blob.FromHex("000201160A"));
		}

		private void WriteCursorPositions()
		{
			rom.PutInBank(0x0C, 0x9FE7, Blob.FromHex("18A618B618C618D668A668B668C668D6"));			
		}
		private void WriteDrinkBoxMenuRoutine()
		{
			//includes Battle_PlMag_IsPlayerValid at the end
			rom.PutInBank(0x0C,0x95EC,Blob.FromHex("2021F920069C48A90120B3F668C902F052ADAB6A0A0DAA6AA8BE30F9F042BD2060F03D8E56688C5768B940F9F00AAD7A6B203A9AC902F0C8AC7A6BA90299A368AD5668999F6899A768AADE2060AC5768BE38F9ADAB6A29030980A8A9104C5D9320FE964CAF94AD6E68C908F006A001B192290360"));
		}

		private void ModifyBattle_PlMag_TargetOnePlayer()
		{
			rom.PutInBank(0x0C, 0xB43D, Blob.FromHex("205296"));
		}

		private void WriteCureAilmentsBreakout()
		{
			rom.PutInBank(0x0C,0xB9CD,Blob.FromHex("A9A048A9C548A91C4C03FE2085B860"));
		}

		private void ModifyUndoBattleCommand()
		{
			rom.PutInBank(0x0C, 0x93A5, Blob.FromHex("FE2060"));
		}

		private void ModifyAfterFadeIn()
		{
			rom.PutInBank(0x0C, 0x93AE, Blob.FromHex("A004A9008899A368C000D0F8"));
		}

		private void WriteBattleDoTurn()
		{
			rom.PutInBank(0x0C,0xA364,Blob.FromHex("9008BE9168A5884CBAA44A4A9012A688A9009DA368B99068BE9168A4884CB5B34A9005A5884CD8A34A90179848A488BEA768DE206368A8B99068BE9168A4884CC5B3A5882045A1A001B18285892920F005A5884C2FA4A5892910F005A5884C81A460A00488209993C000D0F84C3B93"));
		}

		private void ModifyBattleLogicLoop()
		{
			rom.PutInBank(0x0C, 0x9455, Blob.FromHex("20C6A3"));
		}

		private void ChangeItemNames()
		{
			var itemnames = rom.ItemsText;

			if (flags.ExtConsumableSet == ExtConsumableSet.SetA)
			{
				itemnames[(int)Item.Soft] = "SOFT@p"; // the original name contains trailing spaces
				itemnames[(int)Item.WoodenNunchucks] = "FCURE";
				itemnames[(int)Item.SmallKnife] = "PHNIX";
				itemnames[(int)Item.WoodenRod] = "BLAST";
				itemnames[(int)Item.Rapier] = "SMOKE";
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetB)
			{
				itemnames[(int)Item.Soft] = "SOFT@p"; // the original name contains trailing spaces
				itemnames[(int)Item.WoodenNunchucks] = "RFRSH";
				itemnames[(int)Item.SmallKnife] = "FLARE";
				itemnames[(int)Item.WoodenRod] = "BLACK";
				itemnames[(int)Item.Rapier] = "GUARD";
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetC)
			{
				itemnames[(int)Item.Soft] = "SOFT@p"; // the original name contains trailing spaces
				itemnames[(int)Item.WoodenNunchucks] = "QUICK";
				itemnames[(int)Item.SmallKnife] = "HIGH@p";
				itemnames[(int)Item.WoodenRod] = "WIZRD";
				itemnames[(int)Item.Rapier] = "CLOAK";
			}
			else
			{
				itemnames[(int)Item.Soft] = "SOFT@p"; // the original name contains trailing spaces
			}
		}

		private void ChangeMenuTexts()
		{

			if (flags.ExtConsumableSet == ExtConsumableSet.SetA)
			{
				rom.MenuText.MenuStrings[(int)FF1Text.MenuString.UseCrystal] = FF1Text.TextToBytes("Who needs a full cure?");
				rom.MenuText.MenuStrings[(int)FF1Text.MenuString.UseHerb] = FF1Text.TextToBytes("Who needs a new life?");
				rom.MenuText.MenuStrings[(int)FF1Text.MenuString.UseKey] = FF1Text.TextToBytes("Take a sip and blast away.");
				rom.MenuText.MenuStrings[(int)FF1Text.MenuString.UseTNT] = FF1Text.TextToBytes("Useful when you need to run.");
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetB)
			{
				rom.MenuText.MenuStrings[(int)FF1Text.MenuString.UseCrystal] = FF1Text.TextToBytes("Need a party heal?");
				rom.MenuText.MenuStrings[(int)FF1Text.MenuString.UseHerb] = FF1Text.TextToBytes("The nuclear option.");
				rom.MenuText.MenuStrings[(int)FF1Text.MenuString.UseKey] = FF1Text.TextToBytes("Who needs a little love?");
				rom.MenuText.MenuStrings[(int)FF1Text.MenuString.UseTNT] = FF1Text.TextToBytes("Not enough armor?");
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetC)
			{
				rom.MenuText.MenuStrings[(int)FF1Text.MenuString.UseCrystal] = FF1Text.TextToBytes("Too slow?");
				rom.MenuText.MenuStrings[(int)FF1Text.MenuString.UseHerb] = FF1Text.TextToBytes("Who needs to recover?");
				rom.MenuText.MenuStrings[(int)FF1Text.MenuString.UseKey] = FF1Text.TextToBytes("The original stuff.");
				rom.MenuText.MenuStrings[(int)FF1Text.MenuString.UseTNT] = FF1Text.TextToBytes("Need to hide?");
			}

		}

		private byte FindLowSpell(string name, string altname)
		{
			name = name.ToLowerInvariant();
			altname = altname.ToLowerInvariant();

			if (Spells.TryGetValue(name, out var spl1))
			{
				return (byte)(spl1.Index);
			}
			else
			{
				var spl2 = Spells.Where(s => s.Key.StartsWith(altname)).OrderBy(s => s.Key).Select(s => s.Value).First();
				return (byte)(spl2.Index);
			}
		}

		private byte FindHighSpell(string name, string altname)
		{
			name = name.ToLowerInvariant();
			altname = altname.ToLowerInvariant();

			if (Spells.TryGetValue(name, out var spl1))
			{
				return (byte)(spl1.Index);
			}
			else
			{
				var spl2 = Spells.Where(s => s.Key.StartsWith(altname)).OrderBy(s => s.Key).Select(s => s.Value).Last();
				return (byte)(spl2.Index);
			}
		}

		private byte FindBlastSpell()
		{
			var spl = SpellInfos.Where(s => s.routine == (byte)SpellRoutine.Damage && s.targeting == (byte)SpellTargeting.AllEnemies && s.elem != (byte)SpellElement.None).OrderBy(s => -s.tier).First();
			return (byte)(SpellInfos.IndexOf(spl));
		}

		private byte FindGuardSpell()
		{
			var spl = SpellInfos.Where(s => s.routine == (byte)SpellRoutine.ArmorUp && s.targeting == (byte)SpellTargeting.AllCharacters).OrderBy(s => -s.tier).First();
			return (byte)(SpellInfos.IndexOf(spl));
		}

		private byte FindConfSpell()
		{
			var spl = SpellInfos.Where(s => s.routine == (byte)SpellRoutine.InflictStatus && s.effect == (byte)SpellStatus.Confuse && s.targeting == (byte)SpellTargeting.AllEnemies).OrderBy(s => -s.tier).First();
			return (byte)(SpellInfos.IndexOf(spl));
		}

		private byte FindRuseSpell()
		{
			var spl = SpellInfos.Where(s => s.routine == (byte)SpellRoutine.Ruse && s.targeting == (byte)SpellTargeting.Self).OrderBy(s => -s.tier).First();
			return (byte)(SpellInfos.IndexOf(spl));
		}

		private void CreateLifeSpell()
		{
			SpellInfo spell = new SpellInfo
			{
				routine = (byte)SpellRoutine.CureAilment, //cure ailment
				effect = (byte)SpellStatus.Death, //death element
				targeting = (byte)SpellTargeting.OneCharacter //single target
			};

			rom.Put(MagicOffset + 0x40 * MagicSize, spell.compressData());
		}

		private void CreateSmokeSpell()
		{
			SpellInfo spell = new SpellInfo
			{
				routine = (byte)SpellRoutine.CureAilment, //cure ailment
				effect = (byte)SpellStatus.None, //no element
				targeting = (byte)SpellTargeting.OneCharacter //single target
			};

			rom.Put(MagicOffset + 0x41 * MagicSize, spell.compressData());
		}

		private void CreateHighPotionSpell()
		{
			SpellInfo spell = new SpellInfo
			{
				routine = (byte)SpellRoutine.Heal,
				effect = (byte)SpellStatus.Mute | (byte)SpellStatus.Sleep,
				targeting = (byte)SpellTargeting.OneCharacter
			};

			rom.Put(MagicOffset + 0x40 * MagicSize, spell.compressData());
		}

		private void CreateBlackPotionSpell()
		{
			SpellInfo spell = new SpellInfo
			{
				routine = (byte)SpellRoutine.InflictStatus,
				effect = (byte)SpellStatus.Death,
				elem = (byte)SpellElement.Poison,
				accuracy = 0x18,
				targeting = (byte)SpellTargeting.AllEnemies
			};

			rom.Put(MagicOffset + 0x40 * MagicSize, spell.compressData());
		}

		private void ClearShops()
		{
			if (flags.ExtConsumableSet == ExtConsumableSet.None) return;

			foreach (var shop in ShopData.Shops)
			{
				if (shop.Type == ShopType.Weapon)
				{
					for (int i = 0; i < shop.Entries.Count; i++)
					{
						if (shop.Entries[i] == Item.WoodenNunchucks ||
							shop.Entries[i] == Item.SmallKnife ||
							shop.Entries[i] == Item.WoodenRod ||
							shop.Entries[i] == Item.Rapier)
						{
							shop.Entries[i] += 4;
						}
					}
				}
			}
		}

		public void AddNormalShopEntries()
		{
			if (flags.ExtConsumableSet == ExtConsumableSet.None || !(flags.NormalShopsHaveExtConsumables ?? false)) return;

			ReplaceShopEntry(Item.Heal, Item.WoodenNunchucks);
			ReplaceShopEntry(Item.Heal, Item.SmallKnife);
			ReplaceShopEntry(Item.Pure, Item.WoodenRod);
			ReplaceShopEntry(Item.Soft, Item.Rapier);
		}

		private void ReplaceShopEntry(Item item1, Item item2)
		{
			var shop = ShopData.Shops.Where(s => s.Type == ShopType.Item && s.Entries.Contains(item1)).ToList().PickRandom(rng);
			shop.Entries.Remove(item1);
			shop.Entries.Add(item2);
		}

		public static double ExtConsumablePriceFix(Item item, double value, IScaleFlags flags)
		{
			if (flags.ExtConsumableSet == ExtConsumableSet.SetA)
			{
				if (item == Item.WoodenNunchucks) return 49152.0;
				if (item == Item.SmallKnife) return 262144.0;
				if (item == Item.WoodenRod) return 16384.0;
				if (item == Item.Rapier) return 12288.0;
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetB)
			{
				if (item == Item.WoodenNunchucks) return 49152.0;
				if (item == Item.SmallKnife) return 32768.0;
				if (item == Item.WoodenRod) return 16384.0;
				if (item == Item.Rapier) return 16384.0;
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetC)
			{
				if (item == Item.WoodenNunchucks) return 49152.0;
				if (item == Item.SmallKnife) return 32768.0;
				if (item == Item.WoodenRod) return 8192.0;
				if (item == Item.Rapier) return 12288.0;
			}

			return value;
		}

		public static void ExtConsumableStartingEquipmentFix(Item[] weapons, Flags flags)
		{
			if (flags.ExtConsumableSet != ExtConsumableSet.None)
			{
				for (int i = 0; i < weapons.Length; i++)
				{
					if (weapons[i] == Item.WoodenNunchucks ||
						weapons[i] == Item.SmallKnife ||
						weapons[i] == Item.WoodenRod ||
						weapons[i] == Item.Rapier)
					{
						weapons[i] += 4;
					}
				}
			}
		}

		public static Item ExtConsumableStartingEquipmentFix(Item item, Flags flags)
		{
			if (flags.ExtConsumableSet != ExtConsumableSet.None)
			{
				if (item == Item.WoodenNunchucks ||
						item == Item.SmallKnife ||
						item == Item.WoodenRod ||
						item == Item.Rapier)
				{
					return item + 4;
				}
			}

			return item;
		}
	}
}
