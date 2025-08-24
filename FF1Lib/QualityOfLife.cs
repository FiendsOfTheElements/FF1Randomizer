using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{

	public enum PoisonSFX
	{
		[Description("Vanilla")]
		Vanilla,

		[Description("Silent")]
		Silent,

		[Description("Ouch")]
		Ouch,

		[Description("Bonk")]
		Bonk,

		[Description("Oops")]
		Oops,

		[Description("Beep")]
		Beep,
	}
	public partial class FF1Rom
	{
		public void QualityOfLifeHacks(Flags flags, Preferences preferences)
		{
			if (preferences.DisableDamageTileFlicker || flags.TournamentSafe)
			{
				DisableDamageTileFlicker();
			}
			if (preferences.DisableDamageTileSFX)
			{
				DisableDamageTileSFX();
			}
			if (preferences.AltPoisonSFX != PoisonSFX.Vanilla)
			{
				SetAltPoisonSFX(flags, preferences.AltPoisonSFX);
			}
			if (preferences.DisableAirshipSFX)
			{
				DisableAirshipSFX();
			}
			if (preferences.DisableSpellCastFlash || flags.TournamentSafe)
			{
				DisableSpellCastScreenFlash();
			}

			if (preferences.LockRespondRate)
			{
				LockRespondRate(preferences.RespondRate);
			}

			if (preferences.UninterruptedMusic)
			{
				UninterruptedMusic();
			}
			if (preferences.MagicShopMenuChange)
			{
				MagicShopsMatchEquipShops();
			}
		}
		public void MagicShopsMatchEquipShops()
		{
			// QoS to make magic shop function like weapon/armor shops
	 		PutInBank(0x0E, 0xA360, Blob.FromHex("A90D205BAA2089A9B0EDA662BD00038D0C032065AA20C2A8B0DEA562D0DA20EBA49008A910205BAA4C65A34CE094A5626A6A6A29C08D0A0320DFAA20CDA4AE0A03AD0B039D00634C60A3"));

			// assembly for QoS
			// LDA #$0D                 
			// JSR $AA5B                
			// JSR $A989                
			// BCS $A357               
			// LDX $62                  
			// LDA $0300,X             
			// STA $030C                
			// JSR $AA65                
			// JSR $A8C2                
			// BCS $A358                
			// LDA $62                  
			// BNE $A358                
			// JSR $A4EB                
			// BCC $A38B                
			// LDA #$10                 
			// JSR $AA5B                
			// JMP $A365                
			// JSR $94E0                
			// LDA $62                  
			// ROR A                    
			// ROR A                    
			// ROR A                    
			// AND #$C0                 
			// STA $030A                
			// JSR $AADF                
			// JSR $A4CD                
			// LDX $030A                
			// LDA $030B                
			// STA $6300,X              
			// JMP $A360                

			PutInBank(0x0E, 0x94E0, Blob.FromHex("A917205BAA20E4A8B0034C8EA34C65A3"));

			// assembly for QoS
			// LDA #$17
  			// JSR $AA5B
			// JSR $A8E4
			// BCS $94ED
			// JMP $A38E
			// JMP $A365

		}
		public void DisableDamageTileFlicker()
		{
			Data[0x7C7E2] = 0xA9;
		}

		// Overwrites the lava SFX Assembly Code with a bunch of NOPs
		public void DisableDamageTileSFX()
		{
			Put(0x7C7E7, Blob.FromHex("EAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));
		}

		public void SetAltPoisonSFX(Flags flags, PoisonSFX poisonSFX)
		{

			string SFXCommand;
			switch (poisonSFX)
			{
				case PoisonSFX.Silent:
				{
					SFXCommand = "EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA";
					break;
				}
				case PoisonSFX.Ouch:
				{
					// one iteration of the vanilla poison sound
					SFXCommand = "A93F8D0440A9818D0540A9608D0640A9008D0740A906857E";
					break;
				}
				case PoisonSFX.Beep:
				{
					if (flags.TournamentSafe)
					{
						// Beep sound was deemed not tournament safe; might remind spectators of hospital sounds
						// Set to "Ouch" instead.
						SFXCommand = "A93F8D0440A9818D0540A9608D0640A9008D0740A906857E";
					}
					else
					{
						// similar to the Zelda 1 low-health alarm
						SFXCommand = "A9BA8D0440A9008D0540A95E8D0640A9008D0740A906857E";
					}
					break;
				}
				case PoisonSFX.Bonk:
				{
					// similar to the Dragon Warrior "bonk into wall" sound
					SFXCommand = "A9BF8D0440A9B48D0540A9F08D0640A9028D0740A909857E";
					break;
				}
				case PoisonSFX.Oops:
				{
					// similar to the Super Mario Bros. 1 stomp/swim sound
					SFXCommand = "A9BA8D0440A98C8D0540A9FF8D0640A9008D0740A90A857E";
					break;
				}
				default:
				{
					/// not likely ever to end up here, but just in case...
					SFXCommand = "EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA";
					break;
				}

			}

			// Overwrites the entire MapPoisonDamage subroutine, preserving damage assignment but changing the order
			// of operations, playing a new sound (or none) once every 4 steps instead of once every frame
			PutInBank(0x1F, 0xC7FB, Blob.FromHex($"A534F00160A000A200BD0161C903D01FA001BD0B61D007BD0A61C9029011BD0A6138E9019D0A61BD0B61E9009D0B618A186940AAD0D398F01FADA0602903D018{SFXCommand}60EAEAEAEAEAEAEAEAEAEAEAEAEA"));
		}

		public void DisableAirshipSFX()
		{
			// this is very simple, so no .asm saved for this routine
		
			// NOP out the sfx during the airship transition animations
			PutInBank(0x1F,0xE215,Blob.FromHex("60EAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));

			// Here, we just don't check to see if we're in airship at all, bypassing the main airship sfx entirely
			PutInBank(0x1F,0xC112, 0x04);
		}
		public void DisableSpellCastScreenFlash()
		{
			//just load the original battleground background in place of the flash color, it will still use the same number of frames
			Put(0x32051, Blob.FromHex("AD446D"));
		}

		public void LockRespondRate(int respondRate)
		{
			// original title screen behavior
			/*
			C9 01     CMP #RIGHT              ; did they press Right?
            D0 04     BNE @Left               ;  if not, they must've pressed Left
            A9 01     LDA #1                  ; add +1 to rate if right
            D0        BNE :+
            02        @Left:
            A9 FF     LDA #-1                 ; or -1 if left
            18    :   CLC
            65 FA     ADC respondrate         ; add/subtract 1 from respond rate
            29 07     AND #7                  ; mask to wrap it from 0<->7
            85 FA     STA respondrate
			 */

			// MODIFIED behavior
			/*
			C9 01     CMP #RIGHT              ; did they press Right?
            D0 04     BNE @Left               ;  if not, they must've pressed Left
            A9 00     LDA #0                  ; MODIFIED (adds nothing instead of 1)
            D0        BNE :+
            02        @Left:
            A9 00     LDA #0                  ; MODIFIED (adds nothing instead of -1)
            18    :   CLC
            65 FA     ADC respondrate         ; add/subtract 1 from respond rate
            29 07     AND #7                  ; mask to wrap it from 0<->7
            85 FA     STA respondrate
			 */

			Put(0x3A1FD, Blob.FromHex("C901D004A900D002A9001865FA290785FA"));

			// Override the read of the respond rate address so that it always reports a hardcoded value
			PutInBank(0x0B, 0x99C8, Blob.FromHex($"EAA0{respondRate - 1:X2}")); // respondrate is 0-based
		}

		public void UninterruptedMusic()
		{
			// Full commented Assembly code can be seen in "UninterruptedMusic-QoLFlag.asm"
			// These 3 replace existing code for processing the old Treasure Chest sound chime, jumping out to the new code
			PutInBank(0x1F, 0xD62B, Blob.FromHex("A90F2003FE2065A0EAEA"));
			PutInBank(0x1F, 0xD675, Blob.FromHex("A90F2003FE2073A04C88D6A90F2003FE4C00A0EA"));
			PutInBank(0x1F, 0xD6C4, Blob.FromHex("4C80D6"));
			// New code generating a new sound that no longer interrupts the music but otherwise works the same (plays for 27 frames)
			PutInBank(0x0F, 0xA000, Blob.FromHex("A57DC90A905CC67DC963904CC97DB017C96A903FC96F9036C974902DC978F01FC97890204C3AA0A91B857EA97F8D0440A97F8D0540A9098D0740A91C4C5FA0A9088D0740A9E14C5FA0A9C94C5FA0A98E4C5FA0A9704C5FA0A932857D4C09C08D06404C09C0A57DC901F005A97E857D60A254864B60A57DC932F014A54BC981F00AA90F855720A1D64C75A0A57C854BA900857D60"));
		}

	}
}
