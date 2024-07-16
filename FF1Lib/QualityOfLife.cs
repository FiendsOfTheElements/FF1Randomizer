using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
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
			PutInBank(0x0B, 0x99C9, Blob.FromHex($"EAA0{respondRate - 1:X2}")); // respondrate is 0-based
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
