using System.ComponentModel;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public const int SardaOffset = 0x393E9;
		public const int SardaSize = 7;
		public const int CanoeSageOffset = 0x39482;
		public const int CanoeSageSize = 5;
		public const int MapSpriteOffset = 0x03400;
		public const int MapSpriteSize = 3;
		public const int MapSpriteCount = 16;

		public void EnableSaveOnDeath(Flags flags, Overworld overworld)
		{
			// rewrite rando's GameOver routine to jump to a new section that will save the game data
			PutInBank(0x1B, 0x801A, Blob.FromHex("4CF58F"));

			byte coneria_x = 0x92;
			byte coneria_y = 0x9E;
			byte airship_x = 0x99;
			byte airship_y = 0xA5;
			byte ship_x = 0x98;
			byte ship_y = 0xA9;

			if (flags.GameMode == GameModes.DeepDungeon)
			{
				coneria_y = 0x9B;
			}

			if (overworld.MapExchange != null && flags.GameMode == GameModes.Standard)
			{
				coneria_x = (byte)(overworld.Locations.StartingLocation.X - 0x07);
				coneria_y = (byte)(overworld.Locations.StartingLocation.Y - 0x07);

				airship_x = overworld.Locations.StartingLocation.X;
				airship_y = overworld.Locations.StartingLocation.Y;

				ship_x = overworld.GetShipLocation((int)OverworldTeleportIndex.Coneria).X;
				ship_y = overworld.GetShipLocation((int)OverworldTeleportIndex.Coneria).Y;
			}

			// write new routine to save data at game over (the game will save when you clear the final textbox and not before), see 1B_8FF5_GameOverAndRestart.asm
			var saveondeath_standardmid = $"AD0460D02EAD0060F04FAD0160CD0164D008AD0260CD0264F03FAD016038E9078D1060AD026038E9078D1160A9048D1460D026AD056038E9078D1060AD066038E9078D1160A9018D1460AD0060F00AA9{ship_x:X2}8D0160A9{ship_y:X2}8D0260";
			var saveondeath_dwmodemid = $"AD0460F00AA9{airship_x:X2}8D0560A9{airship_y:X2}8D0660AD0060F00AA9{ship_x:X2}8D0160A9{ship_y:X2}8D0260A9{coneria_x:X2}8D1060A9{coneria_y:X2}8D11604E1E606E1D606E1C60A9018D1460EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA";
			var saveondeath_part1 = "20E38BA200BD0061C9FFF041BD0C619D0A61BD0D619D0B61BD28639D2063BD29639D2163BD2A639D2263BD2B639D2363BD2C639D2463BD2D639D2563BD2E639D2663BD2F639D2763A9009D01618A186940AAD0B1";
			var saveondeath_part2 = "A200BD00609D0064BD00619D0065BD00629D0066BD00639D0067E8D0E5A9558DFE64A9AA8DFF64A9008DFD64A200187D00647D00657D00667D0067E8D0F149FF8DFD644C1D80";

			// Since we want to spawn inside with No Overworld and not at transport, update coordinate to Coneria Castle
			if (flags.NoOverworld)
			{
				coneria_x = GetFromBank(0x0E, 0x9DC0+0x08, 1)[0];
				coneria_y = GetFromBank(0x0E, 0x9DD0+0x08, 1)[0];

				saveondeath_standardmid = "EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA";
				saveondeath_dwmodemid = $"AD0460F00AA9998D0560A9A58D0660AD0060F00AA9988D0160A9A98D0260A9{coneria_x:X2}8D1060A9{coneria_y:X2}8D11604E1E606E1D606E1C60EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA";
			}
			var saveondeath = saveondeath_part1 + (flags.SaveGameDWMode ? saveondeath_dwmodemid : saveondeath_standardmid) + saveondeath_part2;

			PutInBank(0x1B, 0x8FF5, Blob.FromHex(saveondeath));
		}
		public void DeepDungeonFloorIndicator()
		{
			// Add Current Floor indicator above orbs, see 0E_9850_DrawOrbFloor.asm
			PutInBank(0x0E, 0xB83D, Blob.FromHex("205098"));
			PutInBank(0x0E, 0x9850, Blob.FromHex("2078B8A548C93CB018C908900638E9074C7398A8B9A698855AB9AE98855B4C8398A900851020668EA000B13E855AC8B13E855BA97A855CA985855DA982855EA900855FA95A853EA900853FA902853BA904853A4C44B98C998E968C909895B2B5AFA8B5A4B1A8"));
			// Extend Orb Box
			PutInBank(0x0E, 0xBAA2, Blob.FromHex("02010809"));
		}
		public void ShuffleAstos(Flags flags, NpcObjectData npcdata, DialogueData dialogues, TalkRoutines talkroutines, MT19337 rng)
		{
			// NPC pool to swap Astos with
			List<ObjectId> npcpool = new List<ObjectId> { ObjectId.Astos, ObjectId.Bahamut, ObjectId.CanoeSage, ObjectId.CubeBot, ObjectId.ElfDoc,
			ObjectId.Fairy, ObjectId.Matoya, ObjectId.Nerrick, ObjectId.Smith,
			ObjectId.Titan, ObjectId.Unne, ObjectId.Sarda, ObjectId.ElfPrince, ObjectId.Lefein };

			if ((bool)flags.FightBahamut)
			{
				npcpool.Remove(ObjectId.Bahamut);
			}

			if ((bool)flags.UnsafeAstos)
			{
				npcpool.Add(ObjectId.King);
				npcpool.Add(ObjectId.Princess2);
			}

			// Select random npc
			ObjectId newastos = npcpool.PickRandom(rng);

			// If Astos, we're done here
			if (newastos == ObjectId.Astos) return;

			// If not get NPC talk routine, get NPC object
			var talkscript = npcdata[newastos].Script;

			// Switch astos to Talk_GiveItemOnItem;
			npcdata[ObjectId.Astos].Script = TalkScripts.Talk_GiveItemOnItem;

			// Get items name
			//var newastositem = FormattedItemName((Item)npcdata.GetTalkArray(newastos)[(int)TalkArrayPos.item_id]);
			//var nwkingitem = FormattedItemName((Item)npcdata.GetTalkArray(ObjectId.Astos)[(int)TalkArrayPos.item_id]);
			// so why don't we use the item id? it should be set by the time we get the dialogues (since we check if there's inventory space)

			// Custom dialogs for Astos NPC and the Kindly Old King
			List<(byte, string)> astosdialogs = new List<(byte, string)>
			{
				(0x00, ""),
				//(0x02, "You have ruined my plans\nto steal this " + newastositem + "!\nThe princess will see\nthrough my disguise.\nTremble before the might\nof Astos, the Dark King!"),
				(0x02, "You have ruined my plans\nto steal this #!\nThe princess will see\nthrough my disguise.\nTremble before the might\nof Astos, the Dark King!"),
				(0x00, ""),(0x00, ""),(0x00, ""),
				(0x0C, "You found the HERB?\nCurses! The Elf Prince\nmust never awaken.\nOnly then shall I,\nAstos, become\nthe King of ALL Elves!"),
				//(0x0E, "Is this a dream?.. Are\nyou, the LIGHT WARRIORS?\nHA! Thank you for waking\nme! I am actually Astos,\nKing of ALL Elves! You\nwon't take my " + newastositem + "!"),
				(0x0E, "Is this a dream?.. Are\nyou, the LIGHT WARRIORS?\nHA! Thank you for waking\nme! I am actually Astos,\nKing of ALL Elves! You\nwon't take my #!"),
				//(0x12, "My CROWN! Oh, but it\ndoesn't go with this\noutfit at all. You keep\nit. But thanks! Here,\ntake this also!\n\nReceived " + nwkingitem),
				(0x12, "My CROWN! Oh, but it\ndoesn't go with this\noutfit at all. You keep\nit. But thanks! Here,\ntake this also!\n\nReceived #"),
//				(0x14, "Oh, wonderful!\nNice work! Yes, this TNT\nis just what I need to\nblow open the vault.\nSoon more than\nthe " + newastositem + " will\nbelong to Astos,\nKing of Dark Dwarves!"),
				(0x14, "Oh, wonderful!\nNice work! Yes, this TNT\nis just what I need to\nblow open the vault.\nSoon more than\nthe # will\nbelong to Astos,\nKing of Dark Dwarves!"),
				//(0x16, "ADAMANT!! Now let me\nmake this " + newastositem + "..\nAnd now that I have\nthis, you shall take a\nbeating from Astos,\nthe Dark Blacksmith!"),
				(0x16, "ADAMANT!! Now let me\nmake this #..\nAnd now that I have\nthis, you shall take a\nbeating from Astos,\nthe Dark Blacksmith!"),
				//(0x19, "You found my CRYSTAL and\nwant my " + newastositem + "? Oh!\nI can see!! And now, you\nwill see the wrath of\nAstos, the Dark Witch!"),
				(0x19, "You found my CRYSTAL and\nwant my #? Oh!\nI can see!! And now, you\nwill see the wrath of\nAstos, the Dark Witch!"),
				(0x1C, "Finally! With this SLAB,\nI shall conquer Lefein\nand her secrets will\nbelong to Astos,\nthe Dark Scholar!"),
				(0x00, ""),
				//(0x1E, "Can't you take a hint?\nI just want to be left\nalone with my " + newastositem + "!\nI even paid a Titan to\nguard the path! Fine.\nNow you face Astos,\nKing of the Hermits!"),
				(0x1E, "Can't you take a hint?\nI just want to be left\nalone with my #!\nI even paid a Titan to\nguard the path! Fine.\nNow you face Astos,\nKing of the Hermits!"),
				(0x20, "Really, a rat TAIL?\nYou think this is what\nwould impress me?\nIf you want to prove\nyourself, face off with\nAstos, the Dark Dragon!"),
				//(0xCD, "Kupo?.. Lali ho?..\nMugu mugu?.. Fine! You\nare in the presence of\nAstos, the Dark Thief!\nI stole their " + newastositem + "\nfair and square!"),
				(0xCD, "Kupo?.. Lali ho?..\nMugu mugu?.. Fine! You\nare in the presence of\nAstos, the Dark Thief!\nI stole their #\nfair and square!"),
				(0x00, ""),
				//(0x27, "Boop Beep Boop..\nError! Malfunction!..\nI see you are not\nfooled. It is I, Astos,\nKing of the Dark Robots!\nYou shall never have\nthis " + newastositem + "!"),
				(0x27, "Boop Beep Boop..\nError! Malfunction!..\nI see you are not\nfooled. It is I, Astos,\nKing of the Dark Robots!\nYou shall never have\nthis #!"),
				//(0x06, "This " + newastositem + " has passed\nfrom Queen to Princess\nfor 2000 years. It would\nhave been mine if you\nhadn't rescued me! Now\nyou face Astos, the\nDark Queen!"),
				(0x06, "This # has passed\nfrom Queen to Princess\nfor 2000 years. It would\nhave been mine if you\nhadn't rescued me! Now\nyou face Astos, the\nDark Queen!"),
				//(0x23, "I, Astos the Dark Fairy,\nam free! The other\nfairies trapped me in\nthat BOTTLE! I'd give\nyou this " + newastositem + " in\nthanks, but I would\nrather just kill you."),
				(0x23, "I, Astos the Dark Fairy,\nam free! The other\nfairies trapped me in\nthat BOTTLE! I'd give\nyou this # in\nthanks, but I would\nrather just kill you."),
				(0x2A, "If you want pass, give\nme the RUBY..\nHa, it mine! Now, you in\ntrouble. Me am Astos,\nKing of the Titans!"),
				//(0x2B, "Curses! Do you know how\nlong it took me to\ninfiltrate these grumpy\nold men and steal\nthe " + newastositem + "?\nNow feel the wrath of\nAstos, the Dark Sage!")
				(0x2B, "Curses! Do you know how\nlong it took me to\ninfiltrate these grumpy\nold men and steal\nthe #?\nNow feel the wrath of\nAstos, the Dark Sage!")
			};

			dialogues[astosdialogs[(int)newastos].Item1] = astosdialogs[(int)newastos].Item2;
			dialogues[astosdialogs[(int)ObjectId.Astos].Item1] = astosdialogs[(int)ObjectId.Astos].Item2;

			if (talkscript == TalkScripts.Talk_Titan || talkscript == TalkScripts.Talk_ElfDocUnne)
			{
				// Skip giving item for Titan, ElfDoc or Unne
				talkroutines.ReplaceChunk(newTalkRoutines.Talk_Astos, Blob.FromHex("20109F"), Blob.FromHex("EAEAEA"));
				talkroutines.ReplaceChunk(newTalkRoutines.Talk_Astos, Blob.FromHex("A9F060"), Blob.FromHex("4C4396"));
				npcdata[newastos].Script = TalkScripts.Talk_Astos;
			}
			else if (talkscript == TalkScripts.Talk_GiveItemOnFlag)
			{
				// Check for a flag instead of an item
				talkroutines.ReplaceChunk(newTalkRoutines.Talk_Astos, Blob.FromHex("A674F005BD2060F0"), Blob.FromHex("A474F00520799090"));
				npcdata[newastos].Script = TalkScripts.Talk_Astos;
			}
			else if (talkscript == TalkScripts.Talk_Nerrick || talkscript == TalkScripts.Talk_GiveItemOnItem || talkscript == TalkScripts.Talk_TradeItems)
			{
				// Just set NPC to Astos routine
				npcdata[newastos].Script = TalkScripts.Talk_Astos;
			}
			else if (talkscript == TalkScripts.Talk_Bahamut)
			{
				// Change routine to check for Tail, give promotion and trigger the battle at the same time, see 11_8200_TalkRoutines.asm
				talkroutines.Replace(newTalkRoutines.Talk_Bahamut, Blob.FromHex("AD2D60D003A57160E67DA572203D96A5752020B1A476207F9020739220AE952018964C439660"));
			}

			// Set battle
			npcdata[newastos].Battle = 0x7D;
		}
		/// <summary>
		/// Unused method, but this would allow a non-npc shuffle king to build bridge without rescuing princess
		/// </summary>
		public void EnableFreeBridge()
		{
			// Set the default bridge_vis byte on game start to true. It's a mother beautiful bridge - and it's gonna be there.
			Data[0x3008] = 0x01;
		}

		public void EnableFreeShip()
		{
			Data[0x3000] = 1;
			Data[0x3001] = 152;
			Data[0x3002] = 169;
		}

		public void EnableFreeAirship()
		{
			Data[0x3004] = 1;
			Data[0x3005] = 153;
			Data[0x3006] = 165;
		}

		public void EnableFreeCanal(bool npcShuffleEnabled)
		{
			Data[0x300C] = 0;
		}

		public void EnableFreeCanoe()
		{
			Data[0x3012] = 0x01;
		}

		public void EnableCanalBridge()
		{
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
		public void EnableFreeLute()
		{
			Data[0x3020 + (int)Item.Lute] = 0x01;
		}

		public void EnableFreeRod()
		{
			Data[0x3020 + (int)Item.Rod] = 0x01;
		}

		public void EnableFreeTail()
		{
			Data[0x3020 + (int)Item.Tail] = 0x01;
		}
		public void EnableAirBoat(bool freeAirship, bool freeShip)
		{
			if (freeAirship)
			{
				Data[0x3020 + (int)Item.Floater] = 0x01;
			}

			if (freeAirship && freeShip)
			{
				Data[0x3000] = 0x81;
			}

			byte overworldtrack = Data[0x7C649];
			byte shiptrack = Data[0x7C62D];
			byte airshiptrack = Data[0x7C235];

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

		public void ImproveTurnOrderRandomization(MT19337 rng)
		{
			// Shuffle the initial bias so enemies are no longer always at the start initially.
			List<byte> turnOrder = new List<byte> { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x80, 0x81, 0x82, 0x83 };
			turnOrder.Shuffle(rng);
			Put(0x3215C, turnOrder.ToArray());

			// Rewrite turn order shuffle to Fisher-Yates.
			Put(0x3217A, Blob.FromHex("A90C8D8E68A900AE8E68205DAEA8AE8E68EAEAEAEAEAEA"));
		}

		public void XpAdmissibility(bool nonesGainXp, bool deadsGainXp)
		{
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

		public void CannotSaveOnOverworld()
		{
			// Hacks the game to disallow saving on the overworld with Tents, Cabins, or Houses
			Put(0x3B2F9, Blob.FromHex("1860"));
			// Change Item using text to avoid confusion
			PutInBank(0x0E, 0x87B0, FF1Text.TextToBytes("\n\nSAVING DISABLED!"));
			PutInBank(0x0E, 0x87E7, FF1Text.TextToBytes("\n\nSAVING DISABLED!"));
			PutInBank(0x0E, 0x8825, FF1Text.TextToBytes("\n\nSAVING DISABLED!"));
		}

		public void CannotSaveAtInns()
		{
			// Hacks the game so that Inns do not save the game
			Put(0x3A53D, Blob.FromHex("EAEAEA"));
			// Change Inn text to avoid confusion
			PutInBank(0x0E, 0x81BB, FF1Text.TextToBytes("Welcome\n  ..\nStay to\nheal\nyour\nwounds?"));
			PutInBank(0x0E, 0x81DC, FF1Text.TextToBytes("Don't\nforget\n.."));
			PutInBank(0x0E, 0x81FC, FF1Text.TextToBytes("Your\ngame\nhasn't\nbeen\nsaved."));
		}
		private void defineNewAI(int availableScript, byte spellChance, byte skillChance, List<byte> spells, List<byte> skills)
		{
			EnemyScriptInfo newAI = new EnemyScriptInfo();
			newAI.spell_chance = spellChance;
			newAI.skill_chance = skillChance;
			newAI.spell_list = spells.ToArray();
			newAI.skill_list = skills.ToArray();
			Put(ScriptOffset + (availableScript * ScriptSize), newAI.compressData());
		}

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

		/// <summary>
		/// Returns index of first occurrence where an enemy AI script has NONE for all spells and skills
		/// Returns -1 if none exist
		/// </summary>
		/// <returns>int</returns>
		private int searchForNoSpellNoAbilityEnemyScript()
		{
			int firstResult = -1;

			for (int i = 0; i < ScriptCount; i++)
			{
				EnemyScriptInfo enemyScriptInfo = new EnemyScriptInfo();
				enemyScriptInfo.decompressData(Get(ScriptOffset + (i * ScriptSize), ScriptSize));
				if (enemyScriptInfo.skill_list[0] == 255 &&
					enemyScriptInfo.skill_list[1] == 255 &&
					enemyScriptInfo.skill_list[2] == 255 &&
					enemyScriptInfo.skill_list[3] == 255 &&

					enemyScriptInfo.spell_list[0] == 255 &&
					enemyScriptInfo.spell_list[1] == 255 &&
					enemyScriptInfo.spell_list[2] == 255 &&
					enemyScriptInfo.spell_list[3] == 255 &&
					enemyScriptInfo.spell_list[4] == 255 &&
					enemyScriptInfo.spell_list[5] == 255 &&
					enemyScriptInfo.spell_list[6] == 255 &&
					enemyScriptInfo.spell_list[7] == 255)
				{
					firstResult = i;
					break;
				}
			}
			return firstResult;
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
				    if (decompressedMap[i*8+y][j*8+x] != OverworldMap.OceanTile) {
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

		public void DisableMinimap()
		{
			PutInBank(0x1F, 0xC1A6, Blob.FromHex("EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));
		}

		public void IncreaseDarkPenalty()
		{
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


		public void DraculasCurse(TalkRoutines talkroutines, NPCdata npcdata, MT19337 rng, Flags flags) {
		    var enemyText = ReadText(EnemyTextPointerOffset, EnemyTextPointerBase, EnemyCount);
		    enemyText[119] = "Twin D";  //  +2
		    enemyText[120] = "Twin D";  //  +2
		    enemyText[121] = "CARMILLA"; // +4
		    enemyText[122] = "CARMILLA"; // +4
		    enemyText[123] = "GrREAPER"; // +2
		    enemyText[124] = "GrREAPER"; // +2
		    enemyText[125] = "FRANKEN";  // +1
		    enemyText[126] = "FRANKEN";  // +1
		    enemyText[127] = "VLAD";     // -1

		    // Moving IMP and GrIMP gives another 10 bytes, for a total of 19 extra bytes, of which I'm using 17.
		    var enemyTextPart1 = enemyText.Take(2).ToArray();
		    var enemyTextPart2 = enemyText.Skip(2).ToArray();
		    WriteText(enemyTextPart1, EnemyTextPointerOffset, EnemyTextPointerBase, 0x2CFEC);
		    WriteText(enemyTextPart2, EnemyTextPointerOffset + 4, EnemyTextPointerBase, EnemyTextOffset);

			// Change Orbs to Dracula's relics
			PutInBank(0x0E, 0xAD78, Blob.FromHex("0F050130")); // Update Orbs palette
			// Lit Orbs
			PutInBank(0x0D, 0xB640, Blob.FromHex("0003030303020303F8FBFAFAFAFAFAFA00808080800080803FBF3F3F3F3F3F3F0302030301010000FAFAFAFBFDFDFEFF80008080808080003F3F3F3F3FBFBF3F"));
			PutInBank(0x0D, 0xB680, Blob.FromHex("0000000201030307FFFFF9F8FCF8FBF20000008080C0E0E0FFFF3F3F3F1F0F0F0F0F0F0700000000E4E0E3F0F8FFFFFFE0E0C080000000000F8F1F3F7FFFFFFF"));
			PutInBank(0x0D, 0xB6C0, Blob.FromHex("00000000070F1F1FFFFFFFF8F7E9D4D0000000000080C0C0FFFFFFFF7FBFDFDF1F0F070000000000D9EFF7F8FFFFFFFFC080000000000000DFBF7FFFFFFFFFFF"));
			PutInBank(0x0D, 0xB700, Blob.FromHex("00000000070F0F0FFFFFFFF8F7EFEFEE0000000080C0C080FFFFFF7F3F9F1F3F0F06060200000000ECF4F0F8FCFFFFFF00000000000000007FFFFFFFFFFFFFFF"));
			// Unlit Orbs
			PutInBank(0x0D, 0xB760, Blob.FromHex("0003010000061F3FFCFBFDFCF8E6DFBF0080F0F8000000C07F8FF7FB071F3F9F3F7F7F3F3F1F0000BF7F7FBFBFD0E0FFE0F0F0F0E0C000008FC7C787070103FF"));

			var tileprop = new TilePropTable(this, 0xff);
			tileprop.LoadData();

			// Coneria castle entrance goes to ToF
			tileprop[0x01] = new TileProp(0, (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.TempleOfFiends + 1);
			tileprop[0x02] = new TileProp(0, (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.TempleOfFiends + 1);

			// ToF entrance goes to Ordeals
			tileprop[0x57] = new TileProp(0, (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.CastleOrdeals1F + 1);
			tileprop[0x58] = new TileProp(0, (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.CastleOrdeals1F + 1);

			// Ordeals entrance goes to Coneria castle
			tileprop[0x38] = new TileProp(0, (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.ConeriaCastle1F + 1);
			tileprop[0x39] = new TileProp(0, (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.ConeriaCastle1F + 1);

			// Volcano entrance (evil tree) goes to Mirage
			tileprop[0x64] = new TileProp((byte)TilePropFunc.OWTP_RIVER | (byte)TilePropFunc.OWTP_OCEAN, 0);
			tileprop[0x65] = new TileProp((byte)TilePropFunc.OWTP_RIVER | (byte)TilePropFunc.OWTP_OCEAN, 0);
			tileprop[0x74] = new TileProp(0, (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.MirageTower1F + 1);
			tileprop[0x75] = new TileProp(0, (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.MirageTower1F + 1);

			// Mirage entrance (Desert mountain) goes to Volcano (still requires Chime)
			tileprop[0x1D] = new TileProp((byte)TilePropFunc.OWTP_SPEC_CHIME | (byte)TilePropFunc.OWTP_RIVER | (byte)TilePropFunc.OWTP_OCEAN,
						      (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.GurguVolcanoB1 + 1);
			tileprop[0x1E] = new TileProp((byte)TilePropFunc.OWTP_SPEC_CHIME | (byte)TilePropFunc.OWTP_RIVER | (byte)TilePropFunc.OWTP_OCEAN,
						      (byte)TilePropFunc.TP_TELE_NORM | (byte)MapIndex.GurguVolcanoB1 + 1);

			tileprop.StoreData();

			const int BATTLEBACKDROPASSIGNMENT_OFFSET =		0x3300;

			// fix up battle backdrop on tof
			Put(BATTLEBACKDROPASSIGNMENT_OFFSET + 0x01, new byte[] { 9, 9});

			// fix up battle backdrop on ordeals
			Put(BATTLEBACKDROPASSIGNMENT_OFFSET + 0x57, new byte[] { 5, 5});

			// fix up battle backdrop on mirage tiles
			Put(BATTLEBACKDROPASSIGNMENT_OFFSET + 0x74, new byte[] { 11, 11});

			// fix up battle backdrop on volcano tiles
			Put(BATTLEBACKDROPASSIGNMENT_OFFSET + 0x1D, new byte[] { 14, 14});

			var teledata = new ExitTeleData(this);
			teledata.LoadData();

			var tpsReport = new Teleporters(this, flags.ReplacementMap);

			var tofCoord = tpsReport.OverworldCoordinates[OverworldTeleportIndex.TempleOfFiends1];
			var mirageCoord = tpsReport.OverworldCoordinates[OverworldTeleportIndex.MirageTower1];
			var volcanoCoord = tpsReport.OverworldCoordinates[OverworldTeleportIndex.GurguVolcano1];
			var ordealCoord = tpsReport.OverworldCoordinates[OverworldTeleportIndex.CastleOrdeals1];

			teledata[(byte)ExitTeleportIndex.ExitCastleOrdeals] = new TeleData { X = tofCoord.X, Y = tofCoord.Y, Map = (MapIndex)0xFF }; // ordeals exit to ToF location
			teledata[(byte)ExitTeleportIndex.ExitCastleConeria] = new TeleData { X = ordealCoord.X, Y = ordealCoord.Y, Map = (MapIndex)0xFF }; // coneria exit to ordeal location
			teledata[(byte)ExitTeleportIndex.ExitGurguVolcano] = new TeleData { X = mirageCoord.X, Y = mirageCoord.Y, Map = (MapIndex)0xFF }; // volcano exit to mirage location
			teledata[(byte)ExitTeleportIndex.ExitSkyPalace] = new TeleData { X = volcanoCoord.X, Y = volcanoCoord.Y, Map = (MapIndex)0xFF }; // mirage exit to volcano location

			teledata.StoreData();
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
