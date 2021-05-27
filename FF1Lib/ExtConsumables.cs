using FF1Lib.Data;
using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public class ExtConsumables
	{
		FF1Rom rom;
		Flags flags;
		MT19337 rng;

		Dictionary<string, MagicSpell> Spells;

		List<SpellInfo> SpellInfos;

		ShopData ShopData;

		public ExtConsumables(FF1Rom _rom, Flags _flags, MT19337 _rng, ShopData _shopData)
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
			if (!flags.EnableExtConsumables) return;

			ChangeItemJumpTable();

			WriteOutOfBattleFCureRoutine();
			WriteOutOfBattlePhnixRoutine();
			WriteOutOfBattleSmokeRoutine();
			WriteOutOfBattleBlastRoutine();
			
			//DrawItemBox EndOfItemIndex
			rom.PutInBank(0x1F, 0xEF4D, new byte[] { 0x20 });
			
			ChangeItemNames();
			ChangeMenuTexts();
			
			WriteDrawDrinkBoxBreakoutRoutine();
			WriteDrawDrinkBoxRoutine();
			WriteLutDrinkBoxOrder();
			WriteLutDrinkBoxEffect();
			WriteLutDrinkBoxTarget();
			WriteLutDrinkBox();
			WriteCursorPositions();

			WriteDrinkBoxMenuRoutine();
			ModifyBattle_PlMag_TargetOnePlayer();
			WriteCureAilmentsBreakout();

			ModifyUndoBattleCommand();
			ModifyAfterFadeIn();
			WriteBattleDoTurn();
			ModifyBattleLogicLoop();

			CreateLifeSpell();
			CreateSmokeSpell();

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

			table[(int)Item.WoodenNunchucks] = 0x96F8;
			table[(int)Item.SmallKnife] = 0x9748;
			table[(int)Item.WoodenRod] = 0x9790;
			table[(int)Item.Rapier] = 0x9795;

			rom.PutInBank(0x0E, 0x96B0, Blob.FromUShorts(table));

			//exchange affected LDAs
			rom.PutInBank(0x0E, 0xB16A, Blob.FromHex("BDB096"));
			rom.PutInBank(0x0E, 0xB16F, Blob.FromHex("BDB196"));
		}

		private void WriteOutOfBattleFCureRoutine()
		{
			rom.PutInBank(0x0E, 0x96F8, Blob.FromHex("2000B4A908202BB920A0B3B028A5626A6A6A29C0AABD0161C901F01CC902F018BD0C619D0A61BD0D619D0B612000B42013B6CE3C604C1DB12026DB4C0097"));
		}

		private void WriteOutOfBattlePhnixRoutine()
		{
			rom.PutInBank(0x0E, 0x9748, Blob.FromHex("2000B4A909202BB920A0B3B01FA5626A6A6A29C0AAA90185102088B3B011A9642061B52000B42013B6CE3D604C1DB12026DB4C5097"));
		}

		private void WriteOutOfBattleSmokeRoutine()
		{
			rom.PutInBank(0x0E, 0x9790, Blob.FromHex("A90A4CB8B1"));
		}

		private void WriteOutOfBattleBlastRoutine()
		{
			rom.PutInBank(0x0E, 0x9795, Blob.FromHex("A90B4CB8B1"));
		}

		private void WriteDrawDrinkBoxBreakoutRoutine()
		{
			rom.PutInBank(0x1F, 0xF921, Blob.FromHex("A91C2003FE2020A0A90C2003FE60"));
		}

		private void WriteDrawDrinkBoxRoutine()
		{
			rom.PutInBank(0x1C,0xA020,Blob.FromHex("A005B90FA0999D6A88D0F72090F62057F7EE9E6AEE9F6AEEA06AA200A0002053A0A0182053A0A0302053A0A0482053A04C48F69848207AA0E8207AA0E8A900991A6B1868691A8DA16AA96B69008DA26A2090F6EEA06AEEA06A608A48BD30F948F034AABD2060F02EAAC8C8A90E991A6BC868991A6B8AC964B001C88AC90AB001C8C8A911991A6BC88A991A6BC8A900991A6BC868AA60A910991A6BC8A90A991A6BC86868AA60AD7468F02FAD6D682D7468F01CAD746849FF2D6D688D6D68AD74684AF039A9B948A9D548A90C4C03FEA9B948A9D848A90C4C03FE686868686868A9A448A92348A56AC973F021C974F01DC975F019C976F015A90C4C03FEA9108D7468A9B948A99848A90C4C03FEA5334AB00C4AB00E4AB0104AB0124C18A1C6294C18A1E6294C18A1C62A4C18A1E62A4C18A1"));
		}

		private void WriteLutDrinkBoxOrder()
		{
			rom.PutInBank(0x1F, 0xF930, Blob.FromHex("191A1B001C1D1F1E"));
		}

		private void WriteLutDrinkBoxEffect()
		{
			var data = new byte[] { FindLowSpell("CURE", "CUR"), FindLowSpell("PURE", "PUR"), FindLowSpell("SOFT", "SFT"), 0x00, FindHighSpell("CUR4", "CUR"), 0x40, FindBlastSpell(), 0x41 };
			rom.PutInBank(0x1F, 0xF938, data);
		}

		private void WriteLutDrinkBoxTarget()
		{
			rom.PutInBank(0x1F, 0xF940, Blob.FromHex("0101010001010000"));
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
			rom.PutInBank(0x0C,0xA357,Blob.FromHex("290385880A0AA8B98F684A4A4A9008BE9168A5884CBAA44A4A9012A688A9009DA368B99068BE9168A4884CB5B34A9005A5884CD8A34A90179848A488BEA768DE206368A8B99068BE9168A4884CC5B3A5882045A1A001B18285892920F005A5884C2FA4A5892910F005A5884C81A460A00488209993C000D0F84C3B93"));
		}

		private void ModifyBattleLogicLoop()
		{
			rom.PutInBank(0x0C, 0x9455, Blob.FromHex("20C6A3"));
		}

		private void ChangeItemNames()
		{
			var itemnames = rom.ReadText(FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextPointerCount);

			itemnames[(int)Item.Soft] = "SOFT@p"; // the original name contains trailing spaces
			itemnames[(int)Item.WoodenNunchucks] = "FCURE";
			itemnames[(int)Item.SmallKnife] = "PHNIX";
			itemnames[(int)Item.WoodenRod] = "SMOKE";
			itemnames[(int)Item.Rapier] = "BLAST";

			rom.WriteText(itemnames, FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextOffset, FF1Rom.UnusedGoldItems);
		}

		private void ChangeMenuTexts()
		{
			var pointers = rom.GetFromBank(0x0E, 0x8500, 128).ToUShorts();

			pointers[0x09] = ChangeMenuText(pointers[0x08], "Who needs a full cure?");
			pointers[0x0A] = ChangeMenuText(pointers[0x09], "Who needs a new life?");
			pointers[0x0B] = ChangeMenuText(pointers[0x0A], "Useful when you need to run.");
			ChangeMenuText(pointers[0x0B], "Take a sip and blast away.");

			rom.PutInBank(0x0E, 0x8500, Blob.FromUShorts(pointers));
		}

		private ushort ChangeMenuText(ushort p, string text)
		{
			var textblob = FF1Text.TextToBytes(text);
			rom.PutInBank(0x0E, p, textblob);

			return (ushort)(p + textblob.Length);
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
			var spl = SpellInfos.Where(s => s.routine == 0x01 && s.targeting == 0x01 && s.elem != 0).OrderBy(s => -s.tier).First();
			return (byte)(SpellInfos.IndexOf(spl));
		}

		private void CreateLifeSpell()
		{
			SpellInfo spell = new SpellInfo
			{
				routine = 0x08, //cure ailment
				effect = 0x01, //death element
				targeting = 0x10 //single target
			};

			rom.Put(MagicOffset + 0x40 * MagicSize, spell.compressData());
		}

		private void CreateSmokeSpell()
		{
			SpellInfo spell = new SpellInfo
			{
				routine = 0x08, //cure ailment
				effect = 0x00, //no element
				targeting = 0x10 //single target
			};

			rom.Put(MagicOffset + 0x41 * MagicSize, spell.compressData());
		}

		private void ClearShops()
		{
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

			ShopData.StoreData();
		}

		public void AddNormalShopEntries()
		{
			if (!flags.EnableExtConsumables || !(flags.NormalShopsHaveExtConsumables ?? false)) return;

			ReplaceShopEntry(Item.Heal, Item.WoodenNunchucks);
			ReplaceShopEntry(Item.Heal, Item.SmallKnife);
			ReplaceShopEntry(Item.Pure, Item.WoodenRod);
			ReplaceShopEntry(Item.Soft, Item.Rapier);

			ShopData.StoreData();
		}

		private void ReplaceShopEntry(Item item1, Item item2)
		{
			var shop = ShopData.Shops.Where(s => s.Type == ShopType.Item && s.Entries.Contains(item1)).ToList().PickRandom(rng);
			shop.Entries.Remove(item1);
			shop.Entries.Add(item2);
		}

		public static double ExtConsumablePriceFix(Item item, double value, IScaleFlags flags)
		{
			if (flags.EnableExtConsumables)
			{
				if (item == Item.WoodenNunchucks) return 49152.0;
				if (item == Item.SmallKnife) return 262144.0;
				if (item == Item.WoodenRod) return 16384.0;
				if (item == Item.Rapier) return 12288.0;
			}

			return value;
		}

		public static void ExtConsumableStartingEquipmentFix(Item[] weapons, Flags flags)
		{
			if (flags.EnableExtConsumables)
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
			if (item == Item.WoodenNunchucks ||
						item == Item.SmallKnife ||
						item == Item.WoodenRod ||
						item == Item.Rapier)
			{
				return item + 4;
			}

			return item;
		}
	}
}
