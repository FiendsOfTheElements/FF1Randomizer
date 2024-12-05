using System.ComponentModel;

namespace FF1Lib
{
	public enum PoisonModeOptions
	{
		[Description("Set amount")]
		Constant,
		[Description("50% of Max HP")]
		Halved1,
		[Description("25% of Max HP")]
		Halved2,
		[Description("12.5% of Max HP")]
		Halved3,
		[Description("50% of Current HP")]
		Dimishing1,
		[Description("25% of Current HP")]
		Dimishing2,
		[Description("12.5% of Current HP")]
		Dimishing3
	}
	public partial class FF1Rom : NesRom
	{
		public const int SardaOffset = 0x393E9;
		public const int SardaSize = 7;
		public const int CanoeSageOffset = 0x39482;
		public const int CanoeSageSize = 5;
		public const int MapSpriteOffset = 0x03400;
		public const int MapSpriteSize = 3;
		public const int MapSpriteCount = 16;

		

		private void MiscHacks(Flags flags, MT19337 rng)
		{
			EnableCanalBridge((bool)flags.MapCanalBridge);
		}

		public void DeepDungeonFloorIndicator()
		{
			// Add Current Floor indicator above orbs, see 0E_9850_DrawOrbFloor.asm
			PutInBank(0x0E, 0xB83D, Blob.FromHex("205098"));
			PutInBank(0x0E, 0x9850, Blob.FromHex("2078B8A548C93CB018C908900638E9074C7398A8B9A698855AB9AE98855B4C8398A900851020668EA000B13E855AC8B13E855BA97A855CA985855DA982855EA900855FA95A853EA900853FA902853BA904853A4C44B98C998E968C909895B2B5AFA8B5A4B1A8"));
			// Extend Orb Box
			PutInBank(0x0E, 0xBAA2, Blob.FromHex("02010809"));
		}

		/// <summary>
		/// Unused method, but this would allow a non-npc shuffle king to build bridge without rescuing princess
		/// </summary>
		public void EnableCanalBridge(bool enable)
		{
			if (!enable)
			{
				return;
			}

			// Inline edit to draw the isthmus or the bridge, but never the open canal anymore.
			// See 0F_8780_IsOnEitherBridge for this and the IsOnBridge replacement called from below.
			Put(0x7E3B8, Blob.FromHex("20CEE3AE0D60AC0E6020DFE3B0DFA908AE0C60F0010A"));

			/**
			 *  A slight wrinkle from normal cross page jump in that we need to preserve the status register,
			 *  since the carry bit is what's used to determine if you're on a bridge or canal, of course.
			 *
			    LDA $60FC
				PHA
				LDA #$0F
				JSR $FE03 ;SwapPRG_L
				JSR $8780
				PLA
				PHP
				JSR $FE03 ;SwapPRG_L
				PLP
				RTS
			**/
			Put(0x7C64D, Blob.FromHex("ADFC6048A90F2003FE20808768082003FE2860"));
		}
		public void EnableAirBoat(Flags flags)
		{
			if (!(bool)flags.AirBoat)
			{
				return;
			}

			byte overworldtrack = (byte)(SongTracks.Overworld+0x41);
			byte shiptrack = (byte)(SongTracks.Ship+0x41);
			byte airshiptrack = (byte)(SongTracks.Airship+0x41);

			// see 1B_A000_AirBoatRoutines.asm
			PutInBank(0x0E, 0xB25F, Blob.FromHex("EAEA38")); // disable floater raising the airship
			PutInBank(0x1F, 0xC10C, Blob.FromHex("204BE2"));
			PutInBank(0x1F, 0xC25A, Blob.FromHex("A91B85572003FE4C10A0"));
			PutInBank(0x1F, 0xC609, Blob.FromHex("EAEAEAEAEAEAEAA91B85572003FE20C2A0F0C8"));
			PutInBank(0x1F, 0xC632, Blob.FromHex("A91B85572003FE4C9FA0"));
			PutInBank(0x1F, 0xC6D7, Blob.FromHex("EAEA201CA0"));
			PutInBank(0x1F, 0xE1F6, Blob.FromHex("2089C6"));
			PutInBank(0x1F, 0xE248, Blob.FromHex("4C58E2A91B85572003FE4C25E2"));
			PutInBank(0x1F, 0xE373, Blob.FromHex("2000A0"));
			PutInBank(0x1B, 0xA000, Blob.FromHex($"AD00602901D00160AD00602980498060A542C908F0034C5FA04CB8C6BD00042908D008A9018D0460A90060BD00042904F00160AD00602901D003A90160A5271869078D0160A5281869078D0260A90485468542A9{shiptrack:X2}854BA9008D0460686860AD0460D00DA542C904F00160AD2B60D01560A527186907CD0560D0F5A528186907CD0660D0EBA90885468542A9{airshiptrack:X2}854BA9008D0460AD006009808D00604CA8E1AD0060297F8D0060A5271869078D0160A5281869078D026018A9308D0C40A9{overworldtrack:X2}854B602000A0F011AD0160C512D00AAD0260C513D003A90160A90060"));

			// super secret stuff
			PutInBank(0x0D, 0xB600, Blob.FromHex("FDF804E2D897D9477797DA07D977974777274777D104B6D897D9477797DA07D977974777274777D517B6D90777A7DA07572747D9A7DA07D95777A7D52AB6D017B6FDF803E2D897979797D546B6979797979797979797979797D54DB6070707070707070707070707D55CB6D04DB6FDF809E7C0C0D994DA27477794DB07DAB77794DB07DAB7779475B744240527D9B577D174B6DA045777A7DB045747DAA7DB045747DAA7DB04DAA5DB27DA7454457725D9A7DA045777A7DB045747DAA7DB045747DAA7DB04A7975770D074B6"));
			PutInBank(0x0D, 0x8028, Blob.FromHex("00B641B66EB6"));
		}

		public void ImproveTurnOrderRandomization(bool enable, MT19337 rng)
		{
			if (!enable)
			{
				return;
			}
			// Shuffle the initial bias so enemies are no longer always at the start initially.
			List<byte> turnOrder = new List<byte> { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x80, 0x81, 0x82, 0x83 };
			turnOrder.Shuffle(rng);
			Put(0x3215C, turnOrder.ToArray());

			// Rewrite turn order shuffle to Fisher-Yates.
			Put(0x3217A, Blob.FromHex("A90C8D8E68A900AE8E68205DAEA8AE8E68EAEAEAEAEAEA"));
		}

		public void XpAdmissibility(bool nonesGainXp, bool deadsGainXp)
		{
			if (!nonesGainXp && !deadsGainXp)
			{
				return;
			}

			// New routine to see if character can get XP LvlUp_AwardExp
			if (nonesGainXp && !deadsGainXp)
			{
				PutInBank(0x1B, 0x8710, Blob.FromHex("A000B186C9FFF010A001B1862903F006C903F00218603860AD78688588AD7968858920608820A08A1860"));
			}
			else if (!nonesGainXp && deadsGainXp)
			{
				PutInBank(0x1B, 0x8710, Blob.FromHex("A000B186C9FFF010A001B1862903F006C903F00238603860EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA1860"));
			}
			else if (nonesGainXp && deadsGainXp)
			{
				PutInBank(0x1B, 0x8710, Blob.FromHex("A000B186C9FFF010A001B1862903F006C903F00238603860AD78688588AD7968858920608820A08A1860"));
			}

			// Have LvlUp_AwardExp reroute to new routine
			PutInBank(0x1B, 0x8826, Blob.FromHex("201087B00860"));

			// New routine to count nones for DivideRewardBySurvivors
			PutInBank(0x1B, 0x8D20, Blob.FromHex("A000AD0168C9FFD001C8AD1368C9FFD001C8AD2568C9FFD001C8AD3768C9FFD001C8A20460"));

			// Have DivideRewardBySurvivors reroute to new routine to count nones
			if (nonesGainXp && !deadsGainXp)
			{
				PutInBank(0x1B, 0x8B43, Blob.FromHex("20208DEA"));
			}
			else if (!nonesGainXp && deadsGainXp)
			{
				PutInBank(0x1B, 0x8B43, Blob.FromHex("20208DA9048CB36838EDB368A8EAEAEAEA"));
			}
			else if (nonesGainXp && deadsGainXp)
			{
				PutInBank(0x1B, 0x8B43, Blob.FromHex("A204A004EAEAEAEAEAEAEAEAEAEAEAEAEA")); // NoDanMode legacy code
			}
		}

		public void ShuffleWeaponPermissions(MT19337 rng)
		{
			const int WeaponPermissionsOffset = 0x3BF50;
			ShuffleGearPermissions(rng, WeaponPermissionsOffset);
		}

		public void ShuffleArmorPermissions(MT19337 rng)
		{
			const int ArmorPermissionsOffset = 0x3BFA0;
			ShuffleGearPermissions(rng, ArmorPermissionsOffset);
		}

		public void ShuffleGearPermissions(MT19337 rng, int offset)
		{
			const int PermissionsSize = 2;
			const int PermissionsCount = 40;

			// lut_ClassEquipBit: ;  FT   TH   BB   RM   WM   BM      KN   NJ   MA   RW   WW   BW
			// .WORD               $800,$400,$200,$100,$080,$040,   $020,$010,$008,$004,$002,$001
			var mask = 0x0820; // Fighter/Knight class bit lut. Each class is a shift of this.
			var order = Enumerable.Range(0, 6).ToList();
			order.Shuffle(rng);

			var oldPermissions = Get(offset, PermissionsSize * PermissionsCount).ToUShorts();
			var newPermissions = oldPermissions.Select(item =>
			{
				UInt16 shuffled = 0x0000;
				for (int i = 0; i < 6; ++i)
				{
					// Shift the mask into each class's slot, then AND with vanilla permission.
					// Shift left to vanilla fighter, shift right into new permission.
					shuffled |= (ushort)(((item & (mask >> i)) << i) >> order[i]);
				}
				return shuffled;
			});

			Put(offset, Blob.FromUShorts(newPermissions.ToArray()));
		}
		public void EnableCardiaTreasures(MT19337 rng, Map cardia)
		{
			// Assign items to the chests.
			// Incomplete.

			// Put the chests in Cardia
			var room = Map.CreateEmptyRoom((3, 4), 1);
			room[1, 1] = 0x75;
			cardia.Put((0x2A, 0x07), room);
			cardia[0x0B, 0x2B] = (byte)Tile.Doorway;

			room[1, 1] = 0x76;
			cardia.Put((0x26, 0x1C), room);
			cardia[0x20, 0x27] = (byte)Tile.Doorway;
		}
		/*
		private void defineNewAI(int availableScript, byte spellChance, byte skillChance, List<byte> spells, List<byte> skills)
		{
			EnemyScriptInfo newAI = new EnemyScriptInfo();
			newAI.spell_chance = spellChance;
			newAI.skill_chance = skillChance;
			newAI.spell_list = spells.ToArray();
			newAI.skill_list = skills.ToArray();
			Put(ScriptOffset + (availableScript * ScriptSize), newAI.compressData());
		}*/

		private void setAIScriptToNoneForEnemiesUsing(int availableScript)
		{
			for (int i = 0; i < EnemyCount; i++)
			{
				EnemyInfo enemyInfo = new EnemyInfo();
				enemyInfo.decompressData(Get(EnemyOffset + (i * EnemySize), EnemySize));
				if (enemyInfo.AIscript == availableScript)
				{
					enemyInfo.AIscript = 0xFF;
					Put(EnemyOffset + (i * EnemySize), enemyInfo.compressData());
				}
			}
		}
		public void NoItemMagic(Flags flags)
		{
			var weapons = Weapon.LoadAllWeapons(this, flags).ToList();
			var armors = Armor.LoadAllArmors(this, flags).ToList();

			foreach (var w in weapons)
			{
				w.SpellIndex = 0;
				w.writeWeaponMemory(this);
			}

			foreach (var a in armors)
			{
				a.SpellIndex = 0;
				a.writeArmorMemory(this);
			}

			if (!(flags.Weaponizer ?? false))
			{
				ItemsText[(int)Item.BaneSword] = "Lame  @S";
				ItemsText[(int)Item.HealRod] = "Eel   @F";
				ItemsText[(int)Item.MageRod] = "Age   @F";
				ItemsText[(int)Item.WizardRod] = "Lizard@F";
				ItemsText[(int)Item.LightAxe] = "Slight@X";
			}

			if (!(flags.ArmorCrafter ?? false)) {
				ItemsText[(int)Item.HealHelm] = "Deal  @h";
				ItemsText[(int)Item.ZeusGauntlets] = "Moose @G";
			}

			//possible incentive items
			ItemsText[(int)Item.Defense] = "Dunce @S";
			ItemsText[(int)Item.ThorHammer] = "Bore  @H";
			ItemsText[(int)Item.PowerGauntlets] = "Sour  @G";
			ItemsText[(int)Item.WhiteShirt] = "Right @T";
			ItemsText[(int)Item.BlackShirt] = "Whack @T";
		}

		public byte findEmptyTile(List<List<byte>> decompressedMap) {
		    for (int i = 1; i < 31; i++) {
			for (int j = 1; j < 31; j++) {
			    bool isEmpty = true;
			    for (int x = 0; isEmpty && x < 8; x++) {
				for (int y = 0; isEmpty && y < 8; y++) {
				    if (decompressedMap[i*8+y][j*8+x] != (byte)OwTiles.OceanTile) {
					isEmpty = false;
				    }
				}
			    }
			    if (isEmpty) {
				return (byte)(i*8 + j);
			    }
			}
		    }
		    return 0;
		}

		public void DisableMinimap(bool enable)
		{
			if (!enable)
			{
				return;
			}

			PutInBank(0x1F, 0xC1A6, Blob.FromHex("EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));
		}

		public void IncreaseDarkPenalty(bool enable)
		{
			if (!enable)
			{
				return;
			}
			/* :: Original::                        :: Modified ::
			 * LDA math_hitchance                   LDA #0
			 * SEC                                  STA math_hitchance
			 * SBC #40                              STA math_critchance
			 * STA math_hitchance                   INC A
			 *
			 * 0x326A7: AD 56 68 38 E9 28 8D 56 68  0x326A7: A9 00 8D 56 68 8D 62 68 1A
			 */

			// replace asm, set hitchance and critchance to 0
			Put(0x326A7, Blob.FromHex("A9008D56688D62681A"));
		}

		public void IncreaseRegeneration(bool enable)
		{
			if (!enable)
			{
				return;
			}

			PutInBank(0x0C, 0xA26A, Blob.FromHex("39"));
		}
		
		public void SetPoisonMode(PoisonModeOptions poisonMode, int poisonValue)
		{
			//see 1C_A670_ImprovedPoison.asm
			byte mode = 0;
			byte loopcount = 3;
			short constantValue = (short)poisonValue;

			switch (poisonMode)
			{
				case PoisonModeOptions.Constant:
					mode = 0; break;
				case PoisonModeOptions.Halved1:
					mode = 1; loopcount = 1; break;
				case PoisonModeOptions.Halved2:
					mode = 1; loopcount = 2; break;
				case PoisonModeOptions.Halved3:
					mode = 1; loopcount = 3; break;
				case PoisonModeOptions.Dimishing1:
					mode = 2; loopcount = 1; break;
				case PoisonModeOptions.Dimishing2:
					mode = 2; loopcount = 2; break;
				case PoisonModeOptions.Dimishing3:
					mode = 2; loopcount = 3; break;
			}


			PutInBank(0x0C, 0xA2D7, Blob.FromHex("A9A648A96F48A91C4C03FE"));
			PutInBank(0x1C, 0xA670, Blob.FromHex("AEAD6BBD0C618D5868BD0D618D5968BD0A618D5668BD0B618D5768AC21A7F00988F02188F01288F02" +
				"BAD23A78D5868AD24A78D59684CD9A6BD0A618D5868BD0B618D5968AC22A7186E59686E586888D0F64CD9A6AD23A78D5A68AD24A78D5B68A90" +
				"1A201A00220EDA6A900A200A00120EDA6A9A248A9E948A90C4C03FE488A0AAA980AA8BD566838F956688DCF6BBD5768F957688DD06BB008A90" +
				$"08DCF6B8DD06B680AAAADCF6B9D5668ADD06B9D576860{mode:X2}{loopcount:X2}{(constantValue % 0x100):X2}{((constantValue / 0x100) % 0x100)}"));
		}


		public void DraculasCurse(Overworld overworld, Teleporters teleporters, MT19337 rng, Flags flags)
		{
			if (!flags.DraculasFlag)
			{
				return;
			}

			var enemyText = ReadEnemyText();
			enemyText[119] = "Twin D";  //  +2
			enemyText[120] = "Twin D";  //  +2
			enemyText[121] = "CARMILLA"; // +4
			enemyText[122] = "CARMILLA"; // +4
			enemyText[123] = "GrREAPER"; // +2
			enemyText[124] = "GrREAPER"; // +2
			enemyText[125] = "FRANKEN";  // +1
			enemyText[126] = "FRANKEN";  // +1
			enemyText[127] = "VLAD";     // -1


			WriteEnemyText(enemyText);

			// Change Orbs to Dracula's relics
			PutInBank(0x0E, 0xAD78, Blob.FromHex("0F050130")); // Update Orbs palette
															   // Lit Orbs
			PutInBank(0x0D, 0xB640, Blob.FromHex("0003030303020303F8FBFAFAFAFAFAFA00808080800080803FBF3F3F3F3F3F3F0302030301010000FAFAFAFBFDFDFEFF80008080808080003F3F3F3F3FBFBF3F"));
			PutInBank(0x0D, 0xB680, Blob.FromHex("0000000201030307FFFFF9F8FCF8FBF20000008080C0E0E0FFFF3F3F3F1F0F0F0F0F0F0700000000E4E0E3F0F8FFFFFFE0E0C080000000000F8F1F3F7FFFFFFF"));
			PutInBank(0x0D, 0xB6C0, Blob.FromHex("00000000070F1F1FFFFFFFF8F7E9D4D0000000000080C0C0FFFFFFFF7FBFDFDF1F0F070000000000D9EFF7F8FFFFFFFFC080000000000000DFBF7FFFFFFFFFFF"));
			PutInBank(0x0D, 0xB700, Blob.FromHex("00000000070F0F0FFFFFFFF8F7EFEFEE0000000080C0C080FFFFFF7F3F9F1F3F0F06060200000000ECF4F0F8FCFFFFFF00000000000000007FFFFFFFFFFFFFFF"));
			// Unlit Orbs
			PutInBank(0x0D, 0xB760, Blob.FromHex("0003010000061F3FFCFBFDFCF8E6DFBF0080F0F8000000C07F8FF7FB071F3F9F3F7F7F3F3F1F0000BF7F7FBFBFD0E0FFE0F0F0F0E0C000008FC7C787070103FF"));

			// Coneria castle entrance goes to ToF
			overworld.TileSet.Tiles[0x01].Properties = new TileProp(0, (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.TempleOfFiends + 1);
			overworld.TileSet.Tiles[0x02].Properties = new TileProp(0, (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.TempleOfFiends + 1);

			// ToF entrance goes to Ordeals
			overworld.TileSet.Tiles[0x57].Properties = new TileProp(0, (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.CastleOrdeals1F + 1);
			overworld.TileSet.Tiles[0x58].Properties = new TileProp(0, (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.CastleOrdeals1F + 1);

			// Ordeals entrance goes to Coneria castle
			overworld.TileSet.Tiles[0x38].Properties = new TileProp(0, (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.ConeriaCastle1F + 1);
			overworld.TileSet.Tiles[0x39].Properties = new TileProp(0, (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.ConeriaCastle1F + 1);

			// Volcano entrance (evil tree) goes to Mirage
			overworld.TileSet.Tiles[0x64].Properties = new TileProp((byte)TilePropFunc.OWTP_RIVER | (byte)TilePropFunc.OWTP_OCEAN, 0);
			overworld.TileSet.Tiles[0x64].Properties = new TileProp((byte)TilePropFunc.OWTP_RIVER | (byte)TilePropFunc.OWTP_OCEAN, 0);
			overworld.TileSet.Tiles[0x74].Properties = new TileProp(0, (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.MirageTower1F + 1);
			overworld.TileSet.Tiles[0x75].Properties = new TileProp(0, (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.MirageTower1F + 1);

			// Mirage entrance (Desert mountain) goes to Volcano (still requires Chime)
			overworld.TileSet.Tiles[0x1D].Properties = new TileProp((byte)TilePropFunc.OWTP_SPEC_CHIME | (byte)TilePropFunc.OWTP_RIVER | (byte)TilePropFunc.OWTP_OCEAN,
							  (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.GurguVolcanoB1 + 1);
			overworld.TileSet.Tiles[0x1E].Properties = new TileProp((byte)TilePropFunc.OWTP_SPEC_CHIME | (byte)TilePropFunc.OWTP_RIVER | (byte)TilePropFunc.OWTP_OCEAN,
							  (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.GurguVolcanoB1 + 1);

			const int BATTLEBACKDROPASSIGNMENT_OFFSET =		0x3300;

			// fix up battle backdrop on tof
			Put(BATTLEBACKDROPASSIGNMENT_OFFSET + 0x01, new byte[] { 9, 9});

			// fix up battle backdrop on ordeals
			Put(BATTLEBACKDROPASSIGNMENT_OFFSET + 0x57, new byte[] { 5, 5});

			// fix up battle backdrop on mirage tiles
			Put(BATTLEBACKDROPASSIGNMENT_OFFSET + 0x74, new byte[] { 11, 11});

			// fix up battle backdrop on volcano tiles
			Put(BATTLEBACKDROPASSIGNMENT_OFFSET + 0x1D, new byte[] { 14, 14});

			//var teledata = new ExitTeleData(this);
			//teledata.LoadData();

			//var tpsReport = new Teleporters(this, flags.ReplacementMap);

			var tofCoord = teleporters.OverworldCoordinates[OverworldTeleportIndex.TempleOfFiends1];
			var mirageCoord = teleporters.OverworldCoordinates[OverworldTeleportIndex.MirageTower1];
			var volcanoCoord = teleporters.OverworldCoordinates[OverworldTeleportIndex.GurguVolcano1];
			var ordealCoord = teleporters.OverworldCoordinates[OverworldTeleportIndex.CastleOrdeals1];

			teleporters.ExitTeleporters[ExitTeleportIndex.ExitCastleOrdeals] = new TeleportDestination(MapIndex.ConeriaTown, new Coordinate(tofCoord.X, tofCoord.Y, CoordinateLocale.Overworld)); // ordeals exit to ToF location
			teleporters.ExitTeleporters[ExitTeleportIndex.ExitCastleConeria] = new TeleportDestination(MapIndex.ConeriaTown, new Coordinate(ordealCoord.X, ordealCoord.Y, CoordinateLocale.Overworld)); // coneria exit to ordeal location
			teleporters.ExitTeleporters[ExitTeleportIndex.ExitGurguVolcano] = new TeleportDestination(MapIndex.ConeriaTown, new Coordinate(mirageCoord.X, mirageCoord.Y, CoordinateLocale.Overworld)); // volcano exit to mirage location
			teleporters.ExitTeleporters[ExitTeleportIndex.ExitSkyPalace] = new TeleportDestination(MapIndex.ConeriaTown, new Coordinate(volcanoCoord.X, volcanoCoord.Y, CoordinateLocale.Overworld)); // mirage exit to volcano location
		}

		public void OpenChestsInOrder(bool enabled)
		{
			if (!enabled)
			{
				return;
			}

			PutInBank(0x1F, 0xDD78, Blob.FromHex("A9112003FEBD00B6D0062000B9BD00BF2010B42015B98A60EAEAEAEAEAEA"));

			PutInBank(0x11, 0xB900, Blob.FromHex("A000A200B900622904F006B900B6D001E8C8D0F060"));

			PutInBank(0x11, 0xB915, Blob.FromHex("B00AA445B90062090499006260"));

			//Change the too full logic for GiveReward consumables to clear the chest. This is to not run into an issue with consumables blocking chest progression.
			PutInBank(0x11, 0xB432, Blob.FromHex("B045"));
		}

		public void SetRNG(Flags flags)
		{
			if (!flags.SetRNG)
			{
				return;
			}

			//see 1B_9900_SetRNG.asm for details
			//take into consideration if disable music is on:
			//had to move the battle prep function which included loading the music track
			byte musicTrack = Get(0x2D9C1, 1)[0];
			PutInBank(0x1B, 0x9A00, Blob.FromHex($"A9008DB7688DB868A9{musicTrack:X2}8D4B008DA76B60"));


			//a lot of patch bridge expansions from banks C and B to 1B
			PutInBank(0x0C, 0x97C7, Blob.FromHex("EAEAEA"));

			PutInBank(0x0B, 0x99B8, Blob.FromHex("A99848A9FF48A91B4C03FEEAEAEAEAEA"));

			PutInBank(0x0C, 0x942C, Blob.FromHex("A99948A93F48A91B4C03FEEAEA"));

			PutInBank(0x0C, 0xB197, Blob.FromHex("AAA99948A97F48A91B4C03FE8AEAEAEA2086BB859A869B200CB0"));

			PutInBank(0x0C, 0xA357, Blob.FromHex("A99948A9BF48A91B4C03FE8A28"));

			PutInBank(0x1B, 0x9900, Blob.FromHex("20009AA56A8510297FC973B010AD4103D00BA5F70A0A0A0A38E5F78510A56A0A0A0A0A18656A186510188D40038D8A68A9008D4103A99948A9C248A90B4C03FE"));

			PutInBank(0x1B, 0x9940, Blob.FromHex("AD40031869438D8A688D4003A01CA9008D8C6C998E6888D0FAA99448A93648A90C4C03FE"));

			PutInBank(0x1B, 0x9980, Blob.FromHex("8A8D896C8DCD6BAD8E68180A0A0A0A6D8E68186D8E68186D8E68186D40038D8A6818A9028DA76DA9008D8F6CA9B148A9A248A90C4C03FE"));

			PutInBank(0x1B, 0x99C0, Blob.FromHex("AD8E68180A0A0A0A6D8E68186D8E68186D8E68186D40038D8A6818AC8E68B94868290385880A0AA8B98F684A4A4A08AAA9A348A96148A90C4C03FE"));

			//rewrite SMMove_Battle to mark the trap tile flag to keep parity between runners on trap tiles.
			PutInBank(0x1F, 0xCDC3, Blob.FromHex("A545D0112071C5C5F8B013A548186940204AC59005856A8D4103A92085441860"));

			// Vanilla doesnt use the 'extended' way of knowing if a tile is a trap tile so we update those.
			// already updated
		}


	}
}
