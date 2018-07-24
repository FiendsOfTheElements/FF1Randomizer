using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomUtilities;
using System.Collections;
using System.IO;
using System.Security.Cryptography;

namespace FF1Lib
{
	// ReSharper disable once InconsistentNaming
	public partial class FF1Rom : NesRom
	{
		public const string Version = "2.4.0";

		public const int RngOffset = 0x7F100;
		public const int RngSize = 256;

		public const int LevelRequirementsOffset = 0x2D000;
		public const int LevelRequirementsSize = 3;
		public const int LevelRequirementsCount = 49;

		public const int StartingGoldOffset = 0x0301C;

		public const int GoldItemOffset = 108; // 108 items before gold chests
		public const int GoldItemCount = 68;
		public static readonly List<int> UnusedGoldItems = new List<int> { 110, 111, 112, 113, 114, 116, 120, 121, 122, 124, 125, 127, 132, 158, 165, 166, 167, 168, 170, 171, 172 };

		public void PutInBank(int bank, int address, Blob data)
		{
			if (bank == 0x1F)
			{
				if ((address - 0xC000) + data.Length >= 0x4000)
				{
					throw new Exception("Data is too large to fit within its bank.");
				}
				int offset = (bank * 0x4000) + (address - 0xC000);
				this.Put(offset, data);
			}
			else
			{
				if ((address - 0x8000) + data.Length >= 0x4000)
				{
					throw new Exception("Data is too large to fit within its bank.");
				}
				int offset = (bank * 0x4000) + (address - 0x8000);
				this.Put(offset, data);
			}

		}
		private Blob CreateLongJumpTableEntry(byte bank, ushort addr)
		{
			List<byte> tmp = new List<byte> { 0x20, 0xC8, 0xD7 }; // JSR $D7C8, beginning of each table entry

			var addrBytes = BitConverter.GetBytes(addr); // next add the address to jump to
			tmp.Add(addrBytes[0]);
			tmp.Add(addrBytes[1]);
			tmp.Add(bank); //finally, add the bank that the routine is located in

			return tmp.ToArray();
		}

		public FF1Rom(string filename) : base(filename)
		{ }

		public FF1Rom(Stream readStream) : base(readStream)
		{ }

		private FF1Rom()
		{ }

		public static async Task<FF1Rom> CreateAsync(Stream readStream)
		{
			var rom = new FF1Rom();
			await rom.LoadAsync(readStream);

			return rom;
		}

		public void Randomize(Blob seed, Flags flags)
		{
			var rng = new MT19337(BitConverter.ToUInt32(seed, 0));

			UpgradeToMMC3();
			MakeSpaceIn1F();
			EasterEggs();
			DynamicWindowColor();
			PermanentCaravan();
			ShiftEarthOrbDown();

			var palettes = OverworldMap.GeneratePalettes(Get(OverworldMap.MapPaletteOffset, MapCount * OverworldMap.MapPaletteSize).Chunk(OverworldMap.MapPaletteSize));
			var overworldMap = new OverworldMap(this, flags, palettes);
			var maps = ReadMaps();
			var shopItemLocation = ItemLocations.CaravanItemShop1;

			if (flags.ModernBattlefield)
			{
				EnableModernBattlefield();
			}

			if (flags.TitansTrove)
			{
				EnableTitansTrove(maps);
			}

			// This has to be done before we shuffle spell levels.
			if (flags.SpellBugs)
			{
				FixSpellBugs();
			}

			if (flags.EnemySpellsTargetingAllies)
			{
				FixEnemyAOESpells();
			}

			if (flags.ItemMagic)
			{
				ShuffleItemMagic(rng);
			}

			if (flags.ShortToFR)
			{
				ShortenToFR(maps, flags.PreserveFiendRefights, rng);
			}

			if (flags.Treasures && flags.ShardHunt && !flags.ChaosRush)
			{
				EnableShardHunt(rng, flags.ExtraShards ? rng.Between(24, 30) : 16, flags.NPCItems);
			}

			if (flags.TransformFinalFormation)
			{
				TransformFinalFormation((FinalFormation)rng.Between(0, Enum.GetValues(typeof(FinalFormation)).Length - 1));
			}

			var maxRetries = 500;
			for (var i = 0; i < maxRetries; i++)
			{
				try
				{
					overworldMap = new OverworldMap(this, flags, palettes);
					if ((flags.Entrances || flags.Floors || flags.Towns) && flags.Treasures && flags.NPCItems)
					{
						overworldMap.ShuffleEntrancesAndFloors(rng, flags);
					}

					var incentivesData = new IncentiveData(rng, flags, overworldMap.MapLocationRequirements, overworldMap.FloorLocationRequirements, overworldMap.FullLocationRequirements);

					if (flags.Shops)
					{
						var excludeItemsFromRandomShops = flags.Treasures
							? incentivesData.ForcedItemPlacements.Select(x => x.Item).Concat(incentivesData.IncentiveItems).ToList()
							: new List<Item>();
						shopItemLocation = ShuffleShops(rng, flags.EnemyStatusAttacks, flags.RandomWares, excludeItemsFromRandomShops);
					}

					if (flags.Treasures)
					{
						ShuffleTreasures(rng, flags, incentivesData, shopItemLocation, overworldMap);
					}
					break;
				}
				catch (InsaneException)
				{
					Console.WriteLine("Insane seed. Retrying");
					if (maxRetries > (i + 1)) continue;
					throw new InvalidOperationException("Failed Sanity Check too many times");
				}
			}

			if (flags.MagicShops)
			{
				ShuffleMagicShops(rng);
			}

			if (flags.MagicLevels)
			{
				FixWarpBug(); // The warp bug only needs to be fixed if the magic levels are being shuffled
				ShuffleMagicLevels(rng, flags.MagicPermissions);
			}

			if (flags.Rng)
			{
				ShuffleRng(rng);
			}

			if (flags.EnemyScripts)
			{
				ShuffleEnemyScripts(rng, flags.AllowUnsafePirates);
			}

			if (flags.EnemySkillsSpells)
			{
				ShuffleEnemySkillsSpells(rng);
			}

			if (flags.EnemyStatusAttacks)
			{
				ShuffleEnemyStatusAttacks(rng, flags.AllowUnsafePirates);
			}

			if (flags.EnemyFormationsUnrunnable)
			{
				ShuffleUnrunnable(rng);
			}

			if (flags.EnemyFormationsSurprise)
			{
				ShuffleSurpriseBonus(rng);
			}

			if (flags.EnemyFormationsFrequency)
			{
				ShuffleEnemyFormations(rng);
			}

			if (flags.EnemyTrapTiles)
			{
				ShuffleTrapTiles(rng, flags.RandomTrapFormations);
			}

			if (flags.OrdealsPillars)
			{
				ShuffleOrdeals(rng, maps);
			}

			if (flags.SkyCastle4FTeleporters)
			{
				ShuffleSkyCastle4F(rng, maps);
			}

			if (flags.ConfusedOldMen)
			{
				EnableConfusedOldMen(rng);
			}

			if (flags.CrownlessOrdeals)
			{
				EnableEarlyOrdeals();
			}

			if (flags.ChaosRush)
			{
				EnableChaosRush();
			}

			if (flags.EarlySarda && !flags.NPCItems)
			{
				EnableEarlySarda();
			}

			if (flags.EarlySage && !flags.NPCItems)
			{
				EnableEarlySage();
			}

			if (flags.FreeBridge)
			{
				EnableFreeBridge();
			}

			if (flags.FreeAirship)
			{
				EnableFreeAirship();
			}

			if (flags.FreeOrbs)
			{
				EnableFreeOrbs();
			}

			if (flags.NoPartyShuffle)
			{
				DisablePartyShuffle();
			}

			if (flags.SpeedHacks)
			{
				EnableSpeedHacks();
			}

			if (flags.IdentifyTreasures)
			{
				EnableIdentifyTreasures();
			}

			if (flags.Dash)
			{
				EnableDash();
			}

			if (flags.BuyTen)
			{
				EnableBuyTen();
			}

			if (flags.WaitWhenUnrunnable)
			{
				ChangeUnrunnableRunToWait();
			}

			if (flags.EnableCritNumberDisplay)
			{
				EnableCritNumberDisplay();
			}

			if (flags.EasyMode)
			{
				EnableEasyMode();
			}

			if (flags.HouseMPRestoration)
			{
				FixHouse();
			}

			if (flags.WeaponStats)
			{
				FixWeaponStats();
			}

			if (flags.ChanceToRun)
			{
				FixChanceToRun();
			}

			if (flags.EnemyStatusAttackBug)
			{
				FixEnemyStatusAttackBug();
			}

			if (flags.BlackBeltAbsorb)
			{
				FixBBAbsorbBug();
			}

			if (flags.ImproveTurnOrderRandomization)
			{
				ImproveTurnOrderRandomization(rng);
			}

			if (flags.EnemyElementalResistancesBug)
			{
				FixEnemyElementalResistances();
			}

			if (flags.FunEnemyNames)
			{
				FunEnemyNames(flags.TeamSteak);
			}

			var itemText = ReadText(ItemTextPointerOffset, ItemTextPointerBase, ItemTextPointerCount);
			// var dialogueText = ReadText(DialogueTextPointerOffset, DialogueTextPointerBase, DialogueTextPointerCount);
			FixVanillaRibbon(itemText);
			ExpGoldBoost(flags.ExpBonus, flags.ExpMultiplier);
			ScalePrices(flags, itemText, rng);
			ScaleEncounterRate(flags.EncounterRate / 30.0, flags.DungeonEncounterRate / 30.0);


			if (flags.WarMECHMode != WarMECHMode.Vanilla)
			{
				WarMECHNpc(flags.WarMECHMode, rng, maps);
			}

			overworldMap.ApplyMapEdits();
			WriteMaps(maps);

			WriteText(itemText, ItemTextPointerOffset, ItemTextPointerBase, ItemTextOffset, UnusedGoldItems);
			// WriteText(dialogueText, DialogueTextPointerOffset, DialogueTextPointerBase, DialogueTextOffset);

			if (flags.EnemyScaleFactor > 1)
			{
				ScaleEnemyStats(flags.EnemyScaleFactor, flags.WrapStatOverflow, rng);
			}

			if (flags.BossScaleFactor > 1)
			{
				ScaleBossStats(flags.BossScaleFactor, flags.WrapStatOverflow, rng);
			}

			if (flags.ForcedPartyMembers > 0)
			{
				PartyRandomize(rng, flags.ForcedPartyMembers);
			}

			if (flags.MapCanalBridge)
			{
				EnableCanalBridge();
			}

			SetProgressiveScaleMode(flags.ProgressiveScaleMode);

			// We have to do "fun" stuff last because it alters the RNG state.
			RollCredits(rng);

			if (flags.PaletteSwap)
			{
				PaletteSwap(rng);
			}

			if (flags.TeamSteak)
			{
				TeamSteak();
			}

			if (flags.Music != MusicShuffle.None)
			{
				ShuffleMusic(flags.Music, rng);
			}

			WriteSeedAndFlags(Version, seed.ToHex(), Flags.EncodeFlagsText(flags));
			ExtraTrackingAndInitCode();
		}

		private void ExtraTrackingAndInitCode()
		{
			// Expanded game init code, does several things:
			//	- Encounter table emu/hardware fix
			//	- track hard/soft resets
			//	- initialize tracking variables if no game is saved
			PutInBank(0x0F, 0x8000, Blob.FromHex("A9008D00208D012085FEA90885FF85FDA51BC901D00160A901851BA94DC5F9F008A9FF85F585F685F7182088C8B049A94DC5F918F013ADA36469018DA364ADA46469008DA464189010ADA56469018DA564ADA66469008DA664A9008DFD64A200187D00647D00657D00667D0067E8D0F149FF8DFD64189010A2A0A9009D00609D0064E8D0F7EEFB64ADFB648DFB6060"));
			Put(0x7C012, Blob.FromHex("A90F2003FE200080EAEAEAEAEAEAEAEA"));
			Data[0x30FB] = 0x01;


			// Move controller handling out of bank 1F
			// This bit of code is also altered to allow a hard reset using Up+A on controller 2
			PutInBank(0x0F, 0x8200, Blob.FromHex("20108220008360"));
			PutInBank(0x0F, 0x8210, Blob.FromHex("A9018D1640A9008D1640A208AD16402903C9012620AD17402903C901261ECAD0EBA51EC988F0016020A8FE20A8FE20A8FEA2FF9AA900851E9500CAD0FBA6004C12C0"));
			PutInBank(0x0F, 0x8300, Blob.FromHex("A5202903F002A2038611A520290CF0058A090C8511A52045212511452185214520AA2910F00EA5202910F002E623A521491085218A2920F00EA5202920F002E622A521492085218A2940F00EA5202940F002E625A521494085218A2980F00EA5202980F002E624A5214980852160"));
			PutInBank(0x1F, 0xD7C2, CreateLongJumpTableEntry(0x0F, 0x8200));

			// Battles use 2 separate and independent controller handlers for a total of 3 (because why not), so we patch these to respond to Up+A also
			PutInBank(0x0F, 0x8580, Blob.FromHex("A0018C1640888C1640A008AD16404AB0014A6EB368AD17402903C901261E88D0EAA51EC988F004ADB3686020A8FE20A8FE20A8FEA2FF9AA900851E9500CAD0FBA6004C12C0"));
			PutInBank(0x1F, 0xD828, CreateLongJumpTableEntry(0x0F, 0x8580));
			PutInBank(0x0B, 0x9A06, Blob.FromHex("4C28D8"));
			PutInBank(0x0C, 0x97C7, Blob.FromHex("2027F22028D82029ABADB36860"));


			// Put LongJump routine 6 bytes after UpdateJoy used to be
			PutInBank(0x1F, 0xD7C8, Blob.FromHex("85E99885EA6885EB6885ECA001B1EB85EDC8B1EB85EEC8ADFC6085E8B1EB2003FEA9D748A9F548A5E9A4EA6CED0085E9A5E82003FEA5E960"));
			// LongJump entries can start at 0xD800 and must stop before 0xD850 (at which point additional space will need to be freed to make room)

			// Patches for various tracking variables follow:
			// Pedometer + chests opened
			PutInBank(0x0F, 0x8100, Blob.FromHex("18A532D027A52D2901F006A550D01DF00398D018ADA06069018DA060ADA16069008DA160ADA26069008DA260A52F8530A9FF8518A200A000BD00622904F001C8E8D0F5988DB7606060"));
			Put(0x7D023, Blob.FromHex("A90F2003FE200081"));
			// Count number of battles
			PutInBank(0x0F, 0x8400, Blob.FromHex("18ADA76069018DA7609003EEA86020A8FE60"));
			PutInBank(0x1F, 0xD800, CreateLongJumpTableEntry(0x0F, 0x8400));
			PutInBank(0x1F, 0xF28D, Blob.FromHex("2000D8"));
			// Ambushes / Strike First
			PutInBank(0x0F, 0x8420, Blob.FromHex("AD5668C90B9015C95A901F18ADAB6069018DAB609014EEAC6018900E18ADA96069018DA9609003EEAA60AC5668AE576860"));
			PutInBank(0x1F, 0xD806, CreateLongJumpTableEntry(0x0F, 0x8420));
			Put(0x313FB, Blob.FromHex("eaeaea2006D8"));
			// Runs
			PutInBank(0x0F, 0x8480, Blob.FromHex("AD5868F00E18ADAD6069018DAD609003EEAE60AD586860"));
			PutInBank(0x1F, 0xD81C, CreateLongJumpTableEntry(0x0F, 0x8480));
			Put(0x32418, Blob.FromHex("201CD8"));
			// Physical Damage
			PutInBank(0x0F, 0x84B0, Blob.FromHex("8E7D68AD8768F01DADB2606D82688DB260ADB3606D83688DB360ADB46069008DB46018901AADAF606D82688DAF60ADB0606D83688DB060ADB16069008DB160AE7D6860"));
			PutInBank(0x1F, 0xD822, CreateLongJumpTableEntry(0x0F, 0x84B0));
			Put(0x32968, Blob.FromHex("2022D8"));
			// Magic Damage
			PutInBank(0x0F, 0x8500, Blob.FromHex("AD8A6C2980F01CADB2606D58688DB260ADB3606D59688DB360ADB46069008DB460901AADAF606D58688DAF60ADB0606D59688DB060ADB16069008DB160A912A212A00160"));
			PutInBank(0x1F, 0xD83A, CreateLongJumpTableEntry(0x0F, 0x8500));
			PutInBank(0x0C, 0xB8ED, Blob.FromHex("203AD8eaeaea"));
			// Party Wipes
			PutInBank(0x0F, 0x85D0, Blob.FromHex("EEB564A9008DFD64A200187D00647D00657D00667D0067E8D0F149FF8DFD64A952854B60"));
			PutInBank(0x1F, 0xD82E, CreateLongJumpTableEntry(0x0F, 0x85D0));
			PutInBank(0x0B, 0x9AF5, Blob.FromHex("202ED8EAEA"));
			// "Nothing Here"s
			PutInBank(0x0F, 0x8600, Blob.FromHex("A54429C2D005A545F00360A900EEB66060"));
			PutInBank(0x1F, 0xD834, CreateLongJumpTableEntry(0x0F, 0x8600));
			PutInBank(0x1F, 0xCBF3, Blob.FromHex("4C34D8"));

			// Add select button handler on game start menu to change color
			PutInBank(0x0F, 0x8620, Blob.FromHex("203CC4A662A9488540ADFB60D003EEFB60A522F022EEFB60ADFB60C90D300EF007A9018DFB60D005A90F8DFB60A90085222029EBA90060A90160"));
			PutInBank(0x1F, 0xD840, CreateLongJumpTableEntry(0x0F, 0x8620));
			Put(0x3A1B5, Blob.FromHex("2040D8D0034C56A1EA"));
			// Move Most of LoadBorderPalette_Blue out of the way to do a dynamic version.
			PutInBank(0x0F, 0x8700, Blob.FromHex("988DCE038DEE03A90F8DCC03A9008DCD03A9308DCF0360"));

			// Move DrawCommandMenu out of Bank F so we can add no Escape to it
			PutInBank(0x0F, 0x8740, Blob.FromHex("A000A200B91BFA9D9E6AE8C01BD015AD916D2901F00EA9139D9E6AE8C8A9F79D9E6AE8C8E005D0052090F6A200C8C01ED0D260"));

			// Create a clone of IsOnBridge that checks the canal too.
			PutInBank(0x0F, 0x8780, Blob.FromHex("AD0860F014A512CD0960D00DA513CD0A60D006A90085451860A512CD0D60D00DA513CD0E60D006A900854518603860"));

			// BB Absorb fix.
			PutInBank(0x0F, 0x8800, Blob.FromHex("A000B186C902F005C908F00160A018B186301BC8B1863016C8B1863011C8B186300CA026B1861869010AA0209186A01CB186301AC8B1863015C8B1863010C8B186300BA026B186186901A022918660"));

			// Copyright overhaul, see 0F_8960_DrawSeedAndFlags.asm
			PutInBank(0x0F, 0x8980, Blob.FromHex("A9238D0620A9208D0620A200BD00898D0720E8E080D0F560"));

			// Fast Battle Boxes
			PutInBank(0x0F, 0x8A00, Blob.FromHex("A940858AA922858BA91E8588A969858960"));
			PutInBank(0x0F, 0x8A20, Blob.FromHex($"A9{BattleBoxDrawInRows}8DB96820A1F420E8F4A5881869208588A58969008589A58A186920858AA58B6900858BCEB968D0DE60"));

			// Fast Battle Boxes Undraw (Similar... yet different!)
			PutInBank(0x0F, 0x8A80, Blob.FromHex("A9A0858AA923858BA97E8588A96A858960"));
			PutInBank(0x0F, 0x8AA0, Blob.FromHex($"A9{BattleBoxUndrawRows}8DB96820A1F420E8F4A58838E9208588A589E9008589A58A38E920858AA58BE900858BCEB968D0DE60"));

			// Softlock fix
			Put(0x7C956, Blob.FromHex("A90F2003FE4C008B"));
			PutInBank(0x0F, 0x8B00, Blob.FromHex("BAE030B01E8A8D1001A9F4AAA9FBA8BD0001990001CA88E010D0F4AD1001186907AA9AA52948A52A48A50D48A54848A549484C65C9"));

			// Change INT to MDEF in the Status screen
			Put(0x388F5, Blob.FromHex("968D8E8F"));
			Data[0x38DED] = 0x25;

			//Key Items + Progressive Scaling
			PutInBank(0x0F, 0x9000, Blob.FromHex("A200AD2160F001E8AD2260F001E8AD2560F001E8AD2A60F001E8AD2B60F001E8AD2C60F001E8AD2E60F001E8AD3060F001E8AD0060F001E8AD1260F001E8AD0460F001E8AD0860F001E8AD0C60D001E8AD2360D007AD0A622902F001E8AD2460D007AD05622902F001E8AD2660D007AD08622902F001E8AD2760D007AD09622902F001E8AD2860D007AD0B622902F001E8AD2960D007AD14622901D001E8AD2D60D007AD0E622902F001E8AD2F60D007AD13622903F001E88EB86060"));
			PutInBank(0x1F, 0xCFCB, CreateLongJumpTableEntry(0x0F, 0x9100));
			//Division routine
			PutInBank(0x0F, 0x90C0, Blob.FromHex("8A48A9008513A210261026112613A513C5129004E512851326102611CAD0EDA513851268AA60"));
			// Progressive scaling also writes to 0x9100 approaching 200 bytes, begin next Put at 0x9200.

			// Replace Overworld to Floor and Floor to Floor teleport code to JSR out to 0x9200 to set X / Y AND inroom from unused high bit of X.
			PutInBank(0x1F, 0xC1E2, Blob.FromHex("A9002003FEA545293FAABD00AC8510BD20AC8511BD40AC8548AABDC0AC8549A90F2003FE200092EAEAEAEAEAEA"));
			PutInBank(0x1F, 0xC968, Blob.FromHex("A9002003FEA645BD00AD8510BD40AD8511BD80AD8548AABDC0AC8549A90F2003FE200092EAEA"));
			PutInBank(0x0F, 0x9200, Blob.FromHex("A200A5100A9002A2814A38E907293F8529A5110A9002860D4A38E907293F852A60"));

			// Critical hit display for number of hits
			PutInBank(0x0F, 0x9280, FF1Text.TextToBytes("critical hits!", false));
			PutInBank(0x0F, 0x9295, Blob.FromHex("AD6B68C901F02EA2019D3A6BA9118D3A6BE8A9009D3A6BE8A9BB9D3A6BE8A9FF9D3A6BE8A000B980929D3A6BE8C8C00FD0F44CDC92A90F8D3A6BA92C8D3B6BA9008D3C6BEEF86AA23AA06BA904201CF7EEF86A60"));
		}

		public void MakeSpaceIn1F()
		{
			// 54 bytes starting at 0xC265 in bank 1F, ROM offset: 7C275
			// This removes the code for the minigame on the ship, and moves the prior code around too
			PutInBank(0x1F, 0xC244, Blob.FromHex("F003C6476020C2D7A520290FD049A524F00EA9008524A542C908F074C901F0B160EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));
			// 15 bytes starting at 0xC8A4 in bank 1F, ROM offset: 7C8B4
			// This removes the routine that give a reward for beating the minigame, no need for a reward without the minigame 
			PutInBank(0x1F, 0xC8A4, Blob.FromHex("EAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));
			// 28 byte starting at 0xCFCB in bank 1F, ROM offset: 7CFDB
			// This removes the AssertNasirCRC routine, which we were skipping anyways, no point in keeping uncalled routines
			PutInBank(0x1F, 0xCFCB, Blob.FromHex("EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));
		}

		public override bool Validate()
		{
			return Get(0, 16) == Blob.FromHex("06400e890e890e401e400e400e400b42");
		}

		public void UpgradeToMMC3()
		{
			Header[4] = 32; // 32 pages of 16 kB
			Header[6] = 0x43; // original is 0x13 where 1 = MMC1 and 4 = MMC3

			// Expand ROM size, moving bank 0F to the end.
			Blob newData = new byte[0x80000];
			Array.Copy(Data, newData, 0x3C000);
			Array.Copy(Data, 0x3C000, newData, 0x7C000, 0x4000);
			Data = newData;

			// Change bank swap code.
			// We put this code at SwapPRG_L, so we don't have to move any of the "long" calls to it.
			// We completely overwrite SetMMC1SwapMode, since we don't need it anymore, and partially overwrite the original SwapPRG.
			Put(0x7FE03, Blob.FromHex("8dfc6048a9068d0080680a8d018048a9078d00806869018d0180a90060"));

			// Initialize MMC3
			Put(0x7FE48, Blob.FromHex("8d00e0a9808d01a0a0008c00a08c00808c0180c88c0080c88c01808c0080c8c88c0180a9038d0080c88c0180a9048d00804ccdffa900"));
			Put(0x7FFCD, Blob.FromHex("c88c0180a9058d0080c88c01804c7cfeea"));

			// Rewrite the lone place where SwapPRG was called directly and not through SwapPRG_L.
			Data[0x7FE97] = 0x03;
		}

		public void WriteSeedAndFlags(string version, string seed, string flags)
		{
			// Replace most of the old copyright string printing with a JSR to a LongJump
			Put(0x38486, Blob.FromHex("20FCFE60"));

			// DrawSeedAndFlags LongJump
			PutInBank(0x1F, 0xFEFC, CreateLongJumpTableEntry(0x0F, 0x8980));

			var sha = File.Exists("version.txt") ? File.ReadAllText("version.txt").Trim() : "development";
			Blob hash;
			if (sha == "development")
			{
				hash = FF1Text.TextToCopyrightLine("DEVELOPMENT VERSION");
			}
			else
			{
				var hasher = SHA256.Create();
				hash = hasher.ComputeHash(Encoding.ASCII.GetBytes($"{seed}_{flags}_{sha}"));

				var hashpart = BitConverter.ToUInt64(hash, 0);
				hash = Blob.FromHex("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
				for (int i = 13; i < 19; i++)
				{
					// 0xD4 through 0xDF are good symbols to use.
					hash[i] = (byte)(0xD4 + hashpart % 12);
					hashpart /= 12;
				}
			}

			// Put the new string data in a known location.
			PutInBank(0x0F, 0x8900, Blob.Concat(
				FF1Text.TextToCopyrightLine("Final Fantasy Randomizer " + version),
				FF1Text.TextToCopyrightLine("Seed  " + seed),
				FF1Text.TextToCopyrightLine(flags),
				hash));
		}

		public void ShuffleRng(MT19337 rng)
		{
			var rngTable = Get(RngOffset, RngSize).Chunk(1).ToList();
			rngTable.Shuffle(rng);

			Put(RngOffset, rngTable.SelectMany(blob => blob.ToBytes()).ToArray());
		}

	}
}
