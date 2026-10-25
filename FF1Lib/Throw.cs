using FF1Lib.Assembly;
using System.ComponentModel;

namespace FF1Lib
{
	public enum ThrowMode
	{
		[Description("Off")]
		Off,
		[Description("Blursing")]
		Blursing,
		[Description("Ninja only")]
		NinjaOnly,
	}

	public partial class FF1Rom
	{
		/* Throw is encoded as a Fight command ($04) with bit 7 of the effect byte set
		 * and the weapon slot in the low bits. Battle_DoPlayerTurn is already rewritten
		 * by consumables and the RNG hack, so a new command bit cannot be added there. */
		public void EnableThrow(Flags flags)
		{
			if (flags.Throw == ThrowMode.Off)
			{
				return;
			}
			if (flags.Throw == ThrowMode.Blursing && !AnyClassHasBlursing(BonusMalusAction.Throw))
			{
				return;
			}

			byte[] menu = Assembler.Assemble(new BA(0x0C, 0x6E50), "ThrowMenu", MenuSource);
			if (menu.Length == 0 || menu.Length >= 128)
			{
				throw new Exception($"Throw menu dispatcher is {menu.Length} bytes.");
			}

			string mainSource = MainSource
				.Replace("MENU_LEN", menu.Length.ToString())
				.Replace("TYPE_BONUS", Data[0x326F5].ToString());
			byte[] main = Assembler.Assemble(new BA(0x04, 0x8000), "Throw", mainSource);
			if (main.Length > 0x900)
			{
				throw new Exception($"Throw code is {main.Length} bytes and overlaps its data.");
			}

			/* Combat routines at $8000 and the menu dispatcher copied to $6E50. */
			PutInBank(0x04, 0x8000, Blob.FromHex(Convert.ToHexString(main)));
			PutInBank(0x04, 0x8900, Blob.FromHex(Convert.ToHexString(menu)));

			/* Which classes can THROW, and which are unarmed (cannot throw armor). */
			for (int i = 0; i < 12; i++)
			{
				ClassData cls = ClassData[(Classes)i];
				byte canThrow = 0;
				if (flags.Throw == ThrowMode.NinjaOnly)
				{
					if ((Classes)i == Classes.Ninja)
					{
						canThrow = 1;
					}
				}
				else if (cls.ToggleBlursings.Contains(BonusMalusAction.Throw))
				{
					canThrow = 1;
				}
				PutInBank(0x04, 0x8980 + i, canThrow);
				PutInBank(0x04, 0x898C + i, (byte)(cls.UnarmedAttack ? 1 : 0));
			}

			/* THROW is two tiles longer than RUN, so the last letter lands on the
			 * command box border. Text is tile-aligned, so move the right column
			 * one tile (8px) left, and the cursor pixels with it. */
			Data[0x7FA35] = (byte)(Data[0x7FA35] - 1);
			for (int i = 0; i < 16; i++)
			{
				byte curs = Data[0x31F75 + i];
				if ((i & 1) == 0 && i >= 8)
				{
					curs = (byte)(curs - 8);
				}
				PutInBank(0x04, 0x8998 + i, curs);
			}

			/* Right-column second-row Y is MAGIC's, so the cursor lands on THROW.
			 * DrawLabel copies this table into btlcurs_positions. Label X/Y are
			 * RUN's column after the shift and MAGIC's row. Autoretarget flag last. */
			PutInBank(0x04, 0x8998 + 11, Data[0x31F75 + 3]);
			PutInBank(0x04, 0x89A8, Data[0x7FA35]);
			PutInBank(0x04, 0x89A9, Data[0x7FA27]);
			PutInBank(0x04, 0x89AA, (byte)(flags.AutoRetargeting ? 1 : 0));

			/* Bank 04 copy of weapon stats for Throw_AttackEntry. */
			for (int i = 0; i < WeaponCount * WeaponSize; i++)
			{
				PutInBank(0x04, 0x8A00 + i, Data[WeaponOffset + i]);
			}

			/* Command-cursor path: DrawLabel, then enter the command menu at $9D0E. */
			PutInBank(0x0C, 0x9A2C, Blob.FromHex(Convert.ToHexString(PhaTrampoline(0x8000, false))));
			PutInBank(0x0C, 0x9A37, Blob.FromHex("4C0E9D"));

			/* DrawCommandBox flush goes through Throw_AddCommand so THROW is in that same flush.
			 * $F710 is the flush when RUN has been replaced with WAIT; otherwise $F718. */
			PutInBank(0x0C, 0x9CE6, Blob.FromHex(Convert.ToHexString(PhaTrampoline(0x800F, false))));
			int commandFlush = Data[0x7F710] == 0x4C ? 0xF710 : 0xF718;
			PutInBank(0x1F, commandFlush, Blob.FromHex("4CE69C"));

			/* A on the command menu, the physical-attack entry, and the walk-forward swing.
			 * Throw_MenuA accepts THROW; Throw_AttackEntry applies the thrown weapon then
			 * continues at $A4C8; a throw shows the weapon, slides it to the enemy, then
			 * every other walk continues at $9E7E. */
			PutInBank(0x0C, 0x94B8, Blob.FromHex(Convert.ToHexString(PhaTrampoline(0x8003, false))));
			PutInBank(0x0C, 0xA4BA, Blob.FromHex(Convert.ToHexString(PhaTrampoline(0x8006, true))));
			PutInBank(0x0C, 0x9E70, Blob.FromHex(Convert.ToHexString(PhaTrampoline(0x8009, true))));

			/* Choosing FIGHT. X is cleared so the effect byte is not a throw, then the
			 * stub at $ADC1 records a normal attack command. */
			PutInBank(0x0C, 0x94F0, Blob.FromHex("A2004CC1AD"));
			PutInBank(0x0C, 0xADC1, Blob.FromHex("A9044C5D93"));

			/* A roll of 200 is a guaranteed miss. That check goes through the gate at
			 * $AD2E, which lets a throw hit anyway. The injector at $AD3D replaces this
			 * hit's damage with the throw total when $6DC3 is 1. The stat loader returns
			 * at $AD2C with A=0, and the leading branch skips both so loading stats does
			 * not apply a throw. */
			PutInBank(0x0C, 0xA7EE, Blob.FromHex("202EADEA"));
			PutInBank(0x0C, 0xA857, Blob.FromHex("203DAD"));
			PutInBank(0x0C, 0xAD2C, Blob.FromHex("1029C9C8D006ADC36DD0016068684CF5A7ADC36DC901D00FADDE6D8D6068ADDF6D8D6168CEC36D8A4C7BAE"));

			/* The stat loader's return begins with LDA #$AD at $9465, so the jump
			 * replaces that instruction and lands on the clear below. Zero the throw
			 * flag and damage total, then return to bank 0C at $AD2C. $FDF7 swaps back
			 * to bank 0C and returns A from $6DC1. Callers push the address they resume at. */
			PutInBank(0x1B, 0x9465, Blob.FromHex("4C7094"));
			PutInBank(0x1B, 0x9470, Blob.FromHex("A9008DC36D8DDE6D8DDF6DA9AD48A92B48A90C4C03FE"));
			PutInBank(0x1F, 0xFDF7, Blob.FromHex("A90C2003FEADC16D60"));
		}

		bool AnyClassHasBlursing(BonusMalusAction action)
		{
			for (int i = 0; i < 12; i++)
			{
				if (ClassData[(Classes)i].ToggleBlursings.Contains(action))
				{
					return true;
				}
			}
			return false;
		}

		static byte[] PhaTrampoline(int dest, bool saveA)
		{
			byte hi = (byte)((dest - 1) >> 8);
			byte lo = (byte)((dest - 1) & 0xFF);
			if (saveA)
			{
				return new byte[] { 0x48, 0xA9, hi, 0x48, 0xA9, lo, 0x48, 0xA9, 0x04, 0x4C, 0x03, 0xFE };
			}
			return new byte[] { 0xA9, hi, 0x48, 0xA9, lo, 0x48, 0xA9, 0x04, 0x4C, 0x03, 0xFE };
		}

		const string MenuSource = """
			LDA btlcurs_y
			AND #$03
			ASL
			TAY
			LDA lut_BattleSubMenu, Y
			STA $88
			LDA lut_BattleSubMenu+1, Y
			STA $89
			JMP ($0088)
			""";

		const string MainSource = """
			JMP Throw_DrawLabel
			JMP Throw_MenuA
			JMP Throw_AttackEntry
			JMP Throw_WalkGate
			JMP Throw_AutoCrit
			JMP Throw_AddCommand

			; DrawCommandBox is about to flush with bank 0C mapped, so the label
			; string has to be copied to RAM before it is added to that buffer.
			; THROW is drawn for every character. Who can use it is checked on A.
			Throw_AddCommand:
				LDY #$00
			Throw_AddCopy:
				LDA Throw_StrThrow, Y
				STA $6DC8, Y
				INY
				CPY #$06
				BNE Throw_AddCopy
				LDA #$C8
				STA btl_msgdraw_srcptr
				LDA #$6D
				STA btl_msgdraw_srcptr+1
				LDA #$01
				STA btl_msgdraw_hdr
				LDA $89A8
				STA btl_msgdraw_x
				LDA $89A9
				STA btl_msgdraw_y
				JSR BattleDraw_AddBlockToBuffer
				LDA #$F6
				PHA
				LDA #$47
				PHA
				LDA #$0C
				JMP SwapPRG_L

			Throw_DrawLabel:
				LDY #$00
			Throw_CopyCurs:
				LDA $8998, Y
				STA btlcurs_positions, Y
				INY
				CPY #16
				BNE Throw_CopyCurs
				LDA #$9A
				PHA
				LDA #$36
				PHA
				JMP $FDF7

			Throw_MenuA:
				LDA btlcurs_x
				AND #$01
				BEQ Throw_MenuLeft
				LDA btlcurs_y
				AND #$03
				BEQ Throw_MenuRun
				CMP #$01
				BNE Throw_MenuRun
				JSR Throw_ClassHas
				BEQ Throw_MenuDeny
				JSR Throw_Select
				LDA $6DC4
				BEQ Throw_MenuCancel
				LDA $6DC0
				ORA #$80
				TAX
				LDY btlcmd_target
				LDA #$04
				STA $6DC1
				LDA #$93
				PHA
				LDA #$5C
				PHA
				JMP $FDF7
			Throw_MenuRun:
				LDA btlcmd_curchar
				ORA #$80
				TAY
				LDA #$20
				STA $6DC1
				LDA #$93
				PHA
				LDA #$5C
				PHA
				JMP $FDF7
			Throw_MenuDeny:
				LDX #<Throw_StrCant
				LDY #>Throw_StrCant
				JSR Throw_Say
				JMP Throw_MenuCancel
			Throw_MenuCancel:
				LDA #$94
				PHA
				LDA #$AE
				PHA
				JMP $FDF7
			Throw_MenuLeft:
				JSR Throw_CopyMenu
				LDA #$6E
				PHA
				LDA #$4F
				PHA
				LDA #$00
				STA $6DC1
				JMP $FDF7

			Throw_Select:
				LDA #$00
				STA $6DC4
				JSR Throw_CallItemBox
			Throw_SelectLoop:
				JSR Throw_CallItemMenu
				CMP #$02
				BEQ Throw_SelectBack
				LDA btlcurs_x
				AND #$01
				BNE Throw_SelectArmor
				LDA btlcurs_y
				AND #$03
				STA $6DC0
				CLC
				ADC #$18
				TAY
				LDA (btl_ob_charstat_ptr), Y
				AND #$7F
				BEQ Throw_SelectEmpty
				CMP #41
				BCS Throw_SelectArmor
				JSR Throw_CallEnemyMenu
				CMP #$02
				BEQ Throw_SelectRetarget
				LDA btl_battletype
				CMP #$03
				BCC Throw_SelectKeep
				LDA #$00
				STA btlcmd_target
			Throw_SelectKeep:
				LDA #$01
				STA $6DC4
				LDA #$01
				JSR Throw_CallUndraw
				RTS
			Throw_SelectRetarget:
				JSR Throw_CallClear
				JMP Throw_SelectLoop
			Throw_SelectBack:
				LDA #$01
				JSR Throw_CallUndraw
				RTS
			Throw_SelectArmor:
				JSR Throw_Cannot
				LDA #$01
				JSR Throw_CallUndraw
				RTS
			Throw_SelectEmpty:
				JSR Throw_CallNothing
				LDA #$01
				JSR Throw_CallUndraw
				RTS

			Throw_Cannot:
				LDX #<Throw_StrCannot
				LDY #>Throw_StrCannot
			Throw_Say:
				LDA #$04
				JSR DrawCombatBox_L
			Throw_SayWait:
				JSR Throw_CallInput
				BEQ Throw_SayWait
				LDA #$01
				JSR Throw_CallUndraw
				RTS

			Throw_AttackEntry:
				PLA
				AND #$03
				STA battle_attacker_index
				STX $6DC4
				JSR Throw_CallPrep
				JSR Throw_Adjust
				LDX $6DC4
				LDA $89AA
				BEQ Throw_AttackDirect
				STX battle_defender_index
				STX btl_defender
				LDA #$92
				PHA
				LDA #$F3
				PHA
				LDA #$00
				STA $6DC1
				JMP $FDF7
			Throw_AttackDirect:
				STX battle_defender_index
				STX btl_defender
				LDA #$A4
				PHA
				LDA #$C7
				PHA
				LDA #$00
				STA $6DC1
				JMP $FDF7

			Throw_WalkGate:
				PLA
				STA btl_animatingchar
				STX btlattackspr_gfx
				STY btlattackspr_wepmag
				LDA #$FE
				STA $68AB
				LDA $6DC2
				BEQ Throw_WalkNormal
				LDA #$00
				STA $6DC2
				LDA btl_animatingchar
				ASL
				ASL
				TAY
				LDA $6890, Y
				BMI Throw_WalkThrow
			Throw_WalkNormal:
				LDA #$9E
				PHA
				LDA #$7D
				PHA
				LDA #$00
				STA $6DC1
				JMP $FDF7
			Throw_WalkThrow:
				LDA btl_animatingchar
				JSR Throw_CallPrep
				JSR Throw_Remove
				LDA math_numhits
				STA $6DC4
				LDA $6DD0
				STA $6DC6
				JSR Throw_CallRestorePal
				LDA #$00
				STA btlattackspr_wepmag
				STA btlattackspr_nodraw
				STA btlattackspr_hidell
				JSR Throw_CallWalk
				LDA btl_drawflagsA
				ORA #$20
				STA btl_drawflagsA
				JSR Throw_CallPose
				LDA #6
				STA $6DCE
			Throw_HandLoop:
				LDA btl_drawflagsA
				ORA #$20
				STA btl_drawflagsA
				JSR Throw_CallSprites
				DEC $6DCE
				BNE Throw_HandLoop
				JSR Throw_SetupDest
				JSR Throw_FlyInit
				LDA #24
				STA $6DD1
			Throw_FlyLoop:
				LDY #8
			Throw_FlyInner:
				JSR Throw_FlyPixel
				BCS Throw_FlyDone
				DEY
				BNE Throw_FlyInner
				LDA btl_drawflagsA
				ORA #$20
				STA btl_drawflagsA
				JSR Throw_CallSprites
				DEC $6DD1
				BNE Throw_FlyLoop
			Throw_FlyDone:
				LDA $6DCB
				STA btlattackspr_x
				LDA $6DCC
				STA btlattackspr_y
				LDA btl_drawflagsA
				ORA #$20
				STA btl_drawflagsA
				JSR Throw_CallSprites
				JSR Throw_CallClear
				LDA #2
				STA $68AB
				JSR Throw_CallWalk
				LDA btl_animatingchar
				JSR Throw_CallNatural
				JSR Throw_AutoCrit
				LDA #$00
				STA $6DC1
				JMP $FDF7

			Throw_Adjust:
				LDA #$00
				STA $6DC2
				STA $6DC3
				LDA battle_attacker_index
				ASL
				ASL
				TAY
				LDA $6890, Y
				BMI Throw_AdjustMark
				JMP Throw_AdjustAbort
			Throw_AdjustMark:
				AND #$7F
				CMP #$04
				BCC Throw_AdjustSlot
				JMP Throw_AdjustAbort
			Throw_AdjustSlot:
				STA $6DC0
				LDY #$21
				LDA (btl_ob_charstat_ptr), Y
				STA $6DC5
				LDY #$20
				LDA (btl_ob_charstat_ptr), Y
				STA $6DC6
				LDY #$0F
				LDA (btl_ib_charstat_ptr), Y
				STA $6DC7
				LDY #$10
				LDA (btl_ib_charstat_ptr), Y
				STA $6DC8
				LDY #$11
				LDA (btl_ib_charstat_ptr), Y
				STA $6DC9
				LDY #$0C
				LDA (btl_ib_charstat_ptr), Y
				STA $6DCA
				LDY #$0E
				LDA (btl_ib_charstat_ptr), Y
				STA $6DCB
				LDY #$0D
				LDA (btl_ib_charstat_ptr), Y
				STA $6DCC
				LDA $6DC0
				CLC
				ADC #$18
				TAY
				LDA (btl_ob_charstat_ptr), Y
				PHA
				LDA battle_attacker_index
				ASL
				ASL
				TAY
				PLA
				PHA
				STA $6892, Y
				AND #$80
				BEQ Throw_AdjustNotEq
				LDA #$01
				STA $6DC3
			Throw_AdjustNotEq:
				PLA
				AND #$7F
				BNE Throw_AdjustId
				JMP Throw_AdjustAbort
			Throw_AdjustId:
				SEC
				SBC #$01
				CMP #40
				BCC Throw_AdjustIdx
				JMP Throw_AdjustAbort
			Throw_AdjustIdx:
				JSR Throw_Lookup
				LDA $6DD4
				STA $6DCF
				LDA $6DD5
				STA $6DD0
				LDA $6DD6
				STA $6DD1
				LDA $6DDA
				STA $6DD2
				LDA $6DDB
				STA $6DD3
				LDA $6DD8
				LDY #$0E
				STA (btl_ib_charstat_ptr), Y
				LDA $6DD9
				LDY #$0D
				STA (btl_ib_charstat_ptr), Y
				LDA #$00
				STA $6DDC
				STA $6DDD
				LDY #$18
				JSR Throw_Consider
				LDY #$19
				JSR Throw_Consider
				LDY #$1A
				JSR Throw_Consider
				LDY #$1B
				JSR Throw_Consider
				LDA $6DC5
				LDX $6DDC
				JSR Throw_ClampSub
				LDX $6DCF
				JSR Throw_ClampAdd
				LDY #$05
				STA (btl_ib_charstat_ptr), Y
				LDA $6DC6
				LDX $6DDD
				JSR Throw_ClampSub
				LDX $6DD0
				JSR Throw_ClampAdd
				LDY #$09
				STA (btl_ib_charstat_ptr), Y
				LDY #$0F
				LDA #$FF
				STA (btl_ib_charstat_ptr), Y
				LDY #$10
				LDA $6DD2
				STA (btl_ib_charstat_ptr), Y
				LDY #$11
				LDA $6DD3
				STA (btl_ib_charstat_ptr), Y
				LDA #$01
				STA $6DC2
			Throw_AdjustAbort:
				RTS

			; $6DC4 is the Fight hit count and $6DC6 is the thrown weapon's attack.
			; Each hit is a crit for twice that attack.
			Throw_AutoCrit:
				LDA #$FF
				STA math_hitchance
				STA math_critchance
				LDA #$01
				STA $6DC3
				LDA $6DC4
				BNE Throw_SumCount
				LDA #$01
				STA $6DC4
			Throw_SumCount:
				LDA #$00
				STA $6DDE
				STA $6DDF
				LDA btl_attacker_category
				AND btl_defender_category
				BNE Throw_SumBonus
				LDA btl_attacker_element
				AND btl_defender_elemweakness
				BEQ Throw_SumLoop
			Throw_SumBonus:
				LDA $6DC6
				CLC
				ADC #TYPE_BONUS
				BCC Throw_SumStore
				LDA #$FF
			Throw_SumStore:
				STA $6DC6
			Throw_SumLoop:
				LDA $6DC6
				ASL
				BCC Throw_SumAdd
				LDA #$FF
			Throw_SumAdd:
				CLC
				ADC $6DDE
				STA $6DDE
				LDA $6DDF
				ADC #$00
				STA $6DDF
				BCC Throw_SumNext
				LDA #$FF
				STA $6DDE
				STA $6DDF
			Throw_SumNext:
				DEC $6DC4
				BNE Throw_SumLoop
				LDA #$01
				STA math_numhits
				RTS

			Throw_Remove:
				LDA btl_animatingchar
				ASL
				ASL
				TAY
				LDA $6890, Y
				AND #$7F
				CMP #$04
				BCC Throw_RemoveHave
				RTS
			Throw_RemoveHave:
				STA $6DC0
				LDA btl_animatingchar
				ASL
				ASL
				TAY
				LDA $6892, Y
				BNE Throw_RemoveByte
				LDA $6DC0
				CLC
				ADC #$18
				TAY
				LDA (btl_ob_charstat_ptr), Y
				BEQ Throw_RemoveSkip
			Throw_RemoveByte:
				STA $6DC3
				AND #$7F
				BEQ Throw_RemoveSkip
				SEC
				SBC #$01
				CMP #40
				BCC Throw_RemoveId
			Throw_RemoveSkip:
				RTS
			Throw_RemoveId:
				JSR Throw_Lookup
				LDA $6DD4
				STA $6DCF
				LDA $6DD5
				STA $6DD0
				LDA $6DC3
				BPL Throw_RemoveSpare
				LDA #$01
				STA $6DC3
				JMP Throw_RemoveEq
			Throw_RemoveSpare:
				LDA #$00
				STA $6DC3
				LDY #$05
				LDA $6DC5
				STA (btl_ib_charstat_ptr), Y
				LDY #$09
				LDA $6DC6
				STA (btl_ib_charstat_ptr), Y
				LDY #$0F
				LDA $6DC7
				STA (btl_ib_charstat_ptr), Y
				LDY #$10
				LDA $6DC8
				STA (btl_ib_charstat_ptr), Y
				LDY #$11
				LDA $6DC9
				STA (btl_ib_charstat_ptr), Y
				LDY #$0C
				LDA $6DCA
				STA (btl_ib_charstat_ptr), Y
				LDY #$0E
				LDA $6DCB
				STA (btl_ib_charstat_ptr), Y
				LDY #$0D
				LDA $6DCC
				STA (btl_ib_charstat_ptr), Y
				JMP Throw_ClearSlot
			Throw_RemoveEq:
				LDY #$05
				LDA (btl_ib_charstat_ptr), Y
				LDX $6DCF
				JSR Throw_ClampSub
				STA $6DC5
				LDY #$05
				STA (btl_ib_charstat_ptr), Y
				LDY #$21
				LDA (btl_ob_charstat_ptr), Y
				LDX $6DCF
				JSR Throw_ClampSub
				LDY #$21
				STA (btl_ob_charstat_ptr), Y
				LDY #$09
				LDA (btl_ib_charstat_ptr), Y
				LDX $6DD0
				JSR Throw_ClampSub
				STA $6DC6
				LDY #$09
				STA (btl_ib_charstat_ptr), Y
				LDY #$20
				LDA (btl_ob_charstat_ptr), Y
				LDX $6DD0
				JSR Throw_ClampSub
				LDY #$20
				STA (btl_ob_charstat_ptr), Y
				LDY #$00
				LDA (btl_ob_charstat_ptr), Y
				CMP #12
				BCS Throw_RemovePlain
				TAX
				LDA $898C, X
				BEQ Throw_RemovePlain
				LDY #$26
				LDA (btl_ob_charstat_ptr), Y
				CLC
				ADC #$01
				ASL
				STA $6DDE
				LDA $6DC6
				LDX $6DDE
				JSR Throw_ClampAdd
				STA $6DC6
				LDY #$09
				STA (btl_ib_charstat_ptr), Y
				LDY #$20
				LDA (btl_ob_charstat_ptr), Y
				LDX $6DDE
				JSR Throw_ClampAdd
				LDY #$20
				STA (btl_ob_charstat_ptr), Y
				LDA $6DDE
				LDY #$0F
				STA (btl_ib_charstat_ptr), Y
				LDA #$AC
				LDY #$10
				STA (btl_ib_charstat_ptr), Y
				LDA #$28
				LDY #$11
				STA (btl_ib_charstat_ptr), Y
				LDA $6DC5
				LSR
				LSR
				LSR
				LSR
				LSR
				CLC
				ADC #$01
				ASL
				LDY #$0C
				STA (btl_ib_charstat_ptr), Y
				JMP Throw_ClearSlot
			Throw_RemovePlain:
				LDA #$00
				LDY #$0F
				STA (btl_ib_charstat_ptr), Y
				LDY #$10
				STA (btl_ib_charstat_ptr), Y
				LDY #$11
				STA (btl_ib_charstat_ptr), Y
				LDY #$0E
				STA (btl_ib_charstat_ptr), Y
				LDY #$0D
				STA (btl_ib_charstat_ptr), Y
				LDA $6DC5
				LSR
				LSR
				LSR
				LSR
				LSR
				CLC
				ADC #$01
				LDY #$0C
				STA (btl_ib_charstat_ptr), Y
			Throw_ClearSlot:
				LDA btl_animatingchar
			Throw_ClearChar:
				ASL
				ASL
				ASL
				ASL
				ASL
				ASL
				CLC
				ADC $6DC0
				TAX
				LDA #$00
				STA ch_weapons, X
				LDA $6DC0
				CLC
				ADC #$18
				TAY
				LDA #$00
				STA (btl_ob_charstat_ptr), Y
			Throw_RemoveDone:
				RTS

			Throw_Consider:
				LDA (btl_ob_charstat_ptr), Y
				BPL Throw_ConsiderNo
				AND #$7F
				BEQ Throw_ConsiderNo
				SEC
				SBC #$01
				CMP #40
				BCS Throw_ConsiderNo
				JSR Throw_Lookup
				LDA $6DD4
				STA $6DDC
				LDA $6DD5
				STA $6DDD
			Throw_ConsiderNo:
				RTS

			Throw_Lookup:
				STA $6DDE
				LDA #$00
				STA $89
				LDA $6DDE
				ASL
				ROL $89
				ASL
				ROL $89
				ASL
				ROL $89
				CLC
				ADC #$00
				STA $88
				LDA $89
				ADC #$8A
				STA $89
				LDY #$00
			Throw_LookupCopy:
				LDA ($88), Y
				STA $6DD4, Y
				INY
				CPY #$08
				BNE Throw_LookupCopy
				RTS

			Throw_ClampSub:
				STA $6DCE
				STX $6DDF
				SEC
				SBC $6DDF
				BCS Throw_ClampSubOk
				LDA #$00
			Throw_ClampSubOk:
				RTS

			Throw_ClampAdd:
				STA $6DCE
				TXA
				CLC
				ADC $6DCE
				BCC Throw_ClampAddOk
				LDA #$FF
			Throw_ClampAddOk:
				RTS

			Throw_ClassHas:
				LDY #$00
				LDA (btl_ob_charstat_ptr), Y
				CMP #12
				BCS Throw_ClassNo
				TAX
				LDA $8980, X
				RTS
			Throw_ClassNo:
				LDA #$00
				RTS

			Throw_CopyMenu:
				LDY #$00
			Throw_CopyMenuLoop:
				LDA $8900, Y
				STA $6E50, Y
				INY
				CPY #MENU_LEN
				BNE Throw_CopyMenuLoop
				RTS

			; Item names are printed from the item bank, and that routine swaps back to
			; battle_bank before returning. These stubs run from RAM so the return
			; lands somewhere that is still mapped, then they swap back to bank 04.
			Throw_CallItemBox:
				LDX #$00
			Throw_CopyItem:
				LDA Throw_StubItem, X
				STA $6E50, X
				INX
				CPX #8
				BNE Throw_CopyItem
				JMP $6E50
			Throw_StubItem:
				JSR DrawBattleItemBox_L
				LDA #$04
				JMP SwapPRG_L
			Throw_CallUndraw:
				PHA
				LDX #$00
			Throw_CopyUndraw:
				LDA Throw_StubUndraw, X
				STA $6E50, X
				INX
				CPX #8
				BNE Throw_CopyUndraw
				PLA
				JMP $6E50
			Throw_StubUndraw:
				JSR UndrawNBattleBlocks_L
				LDA #$04
				JMP SwapPRG_L

			; DoFrame_WithInput long-jumps to the controller reader on every frame.
			; A long jump cannot stay open across that, so call bank 0C from RAM
			; and swap back after the routine returns. A, X, and Y pass through.
			Throw_CallItemMenu:
				LDA #<MenuSelection_Item
				STA $6DDE
				LDA #>MenuSelection_Item
				STA $6DDF
				JMP Throw_Far0C
			Throw_CallEnemyMenu:
				LDA #<SelectEnemyTarget
				STA $6DDE
				LDA #>SelectEnemyTarget
				STA $6DDF
				JMP Throw_Far0C
			Throw_CallNothing:
				LDA #<DoNothingMessageBox
				STA $6DDE
				LDA #>DoNothingMessageBox
				STA $6DDF
				JMP Throw_Far0C
			Throw_CallInput:
				LDA #<DoFrame_WithInput
				STA $6DDE
				LDA #>DoFrame_WithInput
				STA $6DDF
				JMP Throw_Far0C
			Throw_CallClear:
				LDA #<BattleClearVariableSprite
				STA $6DDE
				LDA #>BattleClearVariableSprite
				STA $6DDF
				JMP Throw_Far0C
			Throw_CallWalk:
				LDA #<CharacterWalkAnimation
				STA $6DDE
				LDA #>CharacterWalkAnimation
				STA $6DDF
				JMP Throw_Far0C
			Throw_CallSprites:
				LDA #<UpdateSprites_TwoFrames
				STA $6DDE
				LDA #>UpdateSprites_TwoFrames
				STA $6DDF
				JMP Throw_Far0C
			Throw_CallPose:
				LDA #<PrepAttackSprite_AFrame
				STA $6DDE
				LDA #>PrepAttackSprite_AFrame
				STA $6DDF
				JMP Throw_Far0C
			Throw_CallNatural:
				PHA
				LDA #<SetNaturalPose
				STA $6DDE
				LDA #>SetNaturalPose
				STA $6DDF
				PLA
				JMP Throw_Far0C
			Throw_CallPrep:
				PHA
				LDA #<PrepCharStatPointers
				STA $6DDE
				LDA #>PrepCharStatPointers
				STA $6DDF
				PLA
				JMP Throw_Far0C
			Throw_SetupDest:
				LDA btl_battletype
				CMP #$03
				BCS Throw_DestFiend
				LDA battle_defender_index
				CMP #$09
				BCC Throw_DestIndex
				LDA #$00
			Throw_DestIndex:
				STA $6DCE
				LDA btl_battletype
				BEQ Throw_Dest9
				CMP #$01
				BEQ Throw_Dest4
				LDA $6DCE
				CMP #$02
				BCC Throw_Dest4
				SEC
				SBC #$02
				CMP #$06
				BCC Throw_Dest6Ok
				LDA #$00
			Throw_Dest6Ok:
				ASL
				TAY
				LDA Throw_Coords6, Y
				STA $6DCB
				INY
				LDA Throw_Coords6, Y
				STA $6DCC
				RTS
			Throw_Dest9:
				LDA $6DCE
				ASL
				TAY
				LDA Throw_Coords9, Y
				STA $6DCB
				INY
				LDA Throw_Coords9, Y
				STA $6DCC
				RTS
			Throw_Dest4:
				LDA $6DCE
				CMP #$04
				BCC Throw_Dest4Ok
				LDA #$00
			Throw_Dest4Ok:
				ASL
				TAY
				LDA Throw_Coords4, Y
				STA $6DCB
				INY
				LDA Throw_Coords4, Y
				STA $6DCC
				RTS
			Throw_DestFiend:
				LDA #$20
				STA $6DCB
				LDA #$40
				STA $6DCC
				RTS

			Throw_FlyInit:
				LDA $6DCB
				SEC
				SBC btlattackspr_x
				BCS Throw_FlyDxP
				EOR #$FF
				CLC
				ADC #$01
				LDX #$FF
				BNE Throw_FlyDxS
			Throw_FlyDxP:
				LDX #$01
			Throw_FlyDxS:
				STA $6DC9
				STX $6DC7
				LDA $6DCC
				SEC
				SBC btlattackspr_y
				BCS Throw_FlyDyP
				EOR #$FF
				CLC
				ADC #$01
				LDX #$FF
				BNE Throw_FlyDyS
			Throw_FlyDyP:
				LDX #$01
			Throw_FlyDyS:
				STA $6DCA
				STX $6DC8
				LDA #$00
				STA $6DCE
				LDA $6DC9
				CMP $6DCA
				BCC Throw_FlyYMaj
				STA $6DCD
				LSR $6DCD
				LDA #$00
				STA $6DCF
				RTS
			Throw_FlyYMaj:
				LDA $6DCA
				LSR A
				STA $6DCD
				LDA #$01
				STA $6DCF
				RTS

			Throw_FlyPixel:
				LDA btlattackspr_x
				CMP $6DCB
				BNE Throw_FlyNeed
				LDA btlattackspr_y
				CMP $6DCC
				BNE Throw_FlyNeed
				SEC
				RTS
			Throw_FlyNeed:
				LDA $6DCF
				BNE Throw_FlyStepY
				LDA btlattackspr_x
				CLC
				ADC $6DC7
				STA btlattackspr_x
				LDA $6DCD
				SEC
				SBC $6DCA
				STA $6DCD
				LDA $6DCE
				SBC #$00
				STA $6DCE
				BPL Throw_FlyMoved
				LDA btlattackspr_y
				CLC
				ADC $6DC8
				STA btlattackspr_y
				LDA $6DCD
				CLC
				ADC $6DC9
				STA $6DCD
				LDA $6DCE
				ADC #$00
				STA $6DCE
				CLC
				RTS
			Throw_FlyStepY:
				LDA btlattackspr_y
				CLC
				ADC $6DC8
				STA btlattackspr_y
				LDA $6DCD
				SEC
				SBC $6DC9
				STA $6DCD
				LDA $6DCE
				SBC #$00
				STA $6DCE
				BPL Throw_FlyMoved
				LDA btlattackspr_x
				CLC
				ADC $6DC7
				STA btlattackspr_x
				LDA $6DCD
				CLC
				ADC $6DCA
				STA $6DCD
				LDA $6DCE
				ADC #$00
				STA $6DCE
			Throw_FlyMoved:
				CLC
				RTS

			Throw_CallRestorePal:
				LDA #<ResetUsePalette
				STA $6DDE
				LDA #>ResetUsePalette
				STA $6DDF
				JSR Throw_Far0C
				LDA #<DoFrame_UpdatePalette
				STA $6DDE
				LDA #>DoFrame_UpdatePalette
				STA $6DDF
				JMP Throw_Far0C
			Throw_Far0C:
				PHA
				TXA
				PHA
				TYA
				PHA
				LDX #$00
			Throw_FarCopy:
				LDA Throw_FarStub, X
				STA $6E50, X
				INX
				CPX #21
				BNE Throw_FarCopy
				PLA
				TAY
				PLA
				TAX
				PLA
				JMP $6E50
			Throw_FarStub:
				PHA
				LDA #$0C
				JSR SwapPRG_L
				PLA
				JSR $6E62
				PHA
				LDA #$04
				JSR SwapPRG_L
				PLA
				RTS
				JMP ($6DDE)

			Throw_Coords9:
				.byte $18, $58, $18, $38, $18, $78, $38, $58, $38, $38, $38, $78, $58, $58, $58, $38, $58, $78
			Throw_Coords4:
				.byte $18, $38, $18, $68, $58, $38, $58, $68
			Throw_Coords6:
				.byte $48, $58, $48, $38, $48, $78, $68, $58, $68, $38, $68, $78

			Throw_StrThrow:
				.byte $9D, $91, $9B, $98, $A0, $00
			Throw_StrCant:
				.byte $8C, $A4, $B1, $BE, $B7, $FF, $B7, $AB, $B5, $B2, $BA, $00
			Throw_StrCannot:
				.byte $8C, $A4, $B1, $B1, $B2, $B7, $FF, $B7, $AB, $B5, $B2, $BA, $00
			""";
	}
}
