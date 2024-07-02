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
	public enum LockHitMode
	{
		[Description("64 (Vanilla)")]
		Vanilla = 0,
		[Description("107 Accuracy")]
		Accuracy107,
		[Description("162 Accuracy")]
		Accuracy162,
		[Description("Auto-hit")]
		AutoHit
	}

	public enum AutohitThreshold
	{
		[Description("300")]
		Vanilla = 0,
		[Description("600")]
		Autohit600,
		[Description("900")]
		Autohit900,
		[Description("1200")]
		Autohit1200,
		[Description("Unlimited")]
		Autohit65535,
		[Description("300 or 600")]
		Autohit300to600,
		[Description("300, 600, or 900")]
		Autohit300to900,
		[Description("300, 600, 900, or 1200")]
		Autohit300to1200,
		[Description("Any of the above")]
		Any,
	}

	public enum LifeInBattleSetting
	{
		[Description("LIFE1 & LIFE2")]
		LifeInBattleAll,
		[Description("LIFE1 Only")]
		LifeInBattleLife1Only,
		[Description("Off (Vanilla)")]
		LifeInBattleOff
	}
	public partial class FF1Rom
	{
		public void SpellBalanceHacks(Flags flags, MT19337 rng)
		{
			// Most of this should be moved into a spell class
			if (flags.LockMode != LockHitMode.Vanilla)
			{
				ChangeLockMode(flags.LockMode);
			}

			if (flags.BuffTier1DamageSpells)
			{
				BuffTier1DamageSpells();
			}

			if (flags.BuffHealingSpells)
			{
				BuffHealingSpells();
			}

			if (flags.IntAffectsSpells)
			{
				IntAffectsSpells();
			}

			UpdateMagicAutohitThreshold(rng, flags.MagicAutohitThreshold, flags.IntAffectsSpells);

			if (flags.EnableSoftInBattle)
			{
				EnableSoftInBattle();
			}

			if (flags.EnableLifeInBattle != LifeInBattleSetting.LifeInBattleOff)
			{
				EnableLifeInBattle(flags);
			}
		}

		public void BuffTier1DamageSpells()
		{
			Put(MagicOffset + MagicSize * 4 + 1, new byte[] { 50 }); // replace FIRE effectivity (same as Tier-3s)
			Put(MagicOffset + MagicSize * 7 + 1, new byte[] { 60 }); // replace LIT effectivity
			Put(MagicOffset + MagicSize * 12 + 1, new byte[] { 70 }); // replace ICE effectivity
			Put(MagicOffset + MagicSize * 1 + 1, new byte[] { 60 }); // replace HARM effectivity
			Put(MagicOffset + MagicSize * 1 + 3, new byte[] { 2 }); // make HARM single-target
		}

		public void BuffHealingSpells()
		{
			// improves CURE and HEAL spells both in and out of battle
			// this is also done by Spellcrafter
			// CURE
			Put(MagicOffset + 1, new byte[] { 0x20 }); // replace CURE effectivity with 32 (was 16)
			Put(0x3AF5F, Blob.FromHex("1F0920")); // changing the oob code for CURE to reflect new values
												  // CUR2
			Put(MagicOffset + MagicSize * 16 + 1, new byte[] { 0x40 }); // replace CUR2 effectivity with 64 (was 32)
			Put(0x3AF67, Blob.FromHex("3F0940")); // changing the oob code for CUR2 to reflect the above effect
												  // CUR3
			Put(MagicOffset + MagicSize * 32 + 1, new byte[] { 0x80 }); // replace CUR3 effectivity with 128 (was 64)
			Put(0x3AF6F, Blob.FromHex("7F0980")); // changing the oob code for CUR3 to reflect the above effect
												  // HEAL
			Put(MagicOffset + MagicSize * 19 + 1, new byte[] { 0x10 }); // replace HEAL effectivity with 16 (was 12)
			Put(0x3AFDF, Blob.FromHex("0F")); // changing the oob code for HEAL to reflect the above effect
											  // HEL2
			Put(MagicOffset + MagicSize * 35 + 1, new byte[] { 0x20 }); // replace HEL2 effectivity with 32 (was 24)
			Put(0x3AFE8, Blob.FromHex("1F")); // changing the oob code for HEL2 to reflect the above effect
											  // HEL3
			Put(MagicOffset + MagicSize * 51 + 1, new byte[] { 0x40 }); // replace HEL3 effectivity with 64 (was 48)
			Put(0x3AFF1, Blob.FromHex("3F")); // changing the oob code for HEL3 to reflect the above effect
											  // LAMP
			Put(MagicOffset + MagicSize * 8 + 1, new byte[] { 0x18 }); // LAMP heals paralysis as well as darkness
																	   // AMUT
			Put(MagicOffset + MagicSize * 27 + 1, new byte[] { 0x50 }); // AMUT heals paralysis as well as silence
		}
		public void IntAffectsSpells()
		{
			//see 1C_A290_IntAffectsMagic.asm
			//jumptable_MagicEffect overrides
			Put(0x33809, Blob.FromHex("00BA"));
			Put(0x3380B, Blob.FromHex("00BA"));
			Put(0x3380D, Blob.FromHex("00BA"));
			Put(0x3380F, Blob.FromHex("00BA"));
			Put(0x33811, Blob.FromHex("00BA"));
			Put(0x33813, Blob.FromHex("00BA"));
			Put(0x33815, Blob.FromHex("00BA"));
			Put(0x33823, Blob.FromHex("00BA"));
			Put(0x33829, Blob.FromHex("00BA"));
			Put(0x3382B, Blob.FromHex("00BA"));

			//jump target in bank 0B
			Put(0x33A00, Blob.FromHex("A9A248A98F48A91C4C03FE"));

			//new bank 1C routines
			Put(0x72290, Blob.FromHex("AD6E680AAABDA3A28596BDA4A28597186C960090A2C9A2EBA208A31CA330A349A349A30000000000000000000000008DA3" +
				"00000000A1A3C3A3A9018D5C68203AA4AD896C2980F008202BA4A9002085A4A9B848A99E48A90C4C03FE203AA4AD896C2980F008202BA4A9002085A4" +
				"A9B948A90748A90C4C03FE20F2A3A9B948A93148A9B848A92C48A90C4C03FE20F2A3A9B948A96048A9B848A92C48A90C4C03FE20F2A3A9B948A97B48" +
				"A9B848A92C48A90C4C03FEA90C4C03FEA9018D5C68AD6D682901D0EFAD74688596AD896C2980F00E202BA48A6D74689002A9FF8D7468AAA9002045A4" +
				"1865969002A9FFAAA9122085A4A9B948A9B948A90C4C03FE20F2A3A9BA48A94848A9B848A92C48A90C4C03FE203AA4AD896C2980F008202BA4A90020" +
				"85A4A9BA48A99648A90C4C03FEA90C4C03FEAD78682D7768D0F3A92C8D5A68A9018D5B68AD896C2980F00B202BA48A0AAAA9022085A4A9BA48A9E848" +
				"A90C4C03FE203AA4AD78682D7768F00BA9008D56688D57684C26A4AD896C2980F008202BA4A9002085A4AD78682D7668F007A900A2282085A4600040" +
				"80C0AD896C2903AABD27A4AABD1261AA60A9948D5668A9008D5768608DAF68E88EB0688A38EDAF688DB6682027F2AEB6682063A48A186DAF68608DB3" +
				"688EB468A208A9008DB5684EB3689004186DB4686A6EB568CAD0F0AAADB568608DCF6B488A489848ADCF6B0AA88A18795668995668A9007957689957" +
				"689008A9FF99566899576868A868AA6860"));
		}

		public void ChangeLockMode(LockHitMode lockHitMode)
		{
			if (lockHitMode == LockHitMode.Accuracy107)
			{
				Put(MagicOffset + (MagicSize * 6), new byte[] { 107 });
				Put(MagicOffset + (MagicSize * 23), new byte[] { 107 });
			}
			else if (lockHitMode == LockHitMode.Accuracy162)
			{
				Put(MagicOffset + (MagicSize * 6), new byte[] { 162 });
				Put(MagicOffset + (MagicSize * 23), new byte[] { 162 });
			}
			else if (lockHitMode == LockHitMode.AutoHit)
			{
				PutInBank(0x0C, 0xBA46, Blob.FromHex("2029B9AD856838ED7468B002A9008D85682085B860EAEAEAEAEAEAEAEAEAEAEAEAEA"));
			}
		}

		public void UpdateMagicAutohitThreshold(MT19337 rng, AutohitThreshold threshold, bool intAffectsSpells)
		{
			short limit = 300;
			switch (threshold)
			{
				case AutohitThreshold.Vanilla: limit = 300; break;
				case AutohitThreshold.Autohit600: limit = 600; break;
				case AutohitThreshold.Autohit900: limit = 900; break;
				case AutohitThreshold.Autohit1200: limit = 1200; break;
				case AutohitThreshold.Autohit65535: limit = short.MaxValue; break;
				case AutohitThreshold.Autohit300to600: limit = (short)(rng.Between(1, 2) * 300); break;
				case AutohitThreshold.Autohit300to900: limit = (short)(rng.Between(1, 3) * 300); break;
				case AutohitThreshold.Autohit300to1200: limit = (short)(rng.Between(1, 4) * 300); break;
				case AutohitThreshold.Any:
					{
						short[] any = { 300, 600, 900, 1200, short.MaxValue };
						limit = any.PickRandom(rng);
						break;
					}
			}

			// Set the low and high bytes of the limit which are then loaded and compared to the targets hp.
			Data[0x33AE0] = (byte)(limit & 0x00ff);
			Data[0x33AE5] = (byte)((limit >> 8) & 0x00ff);

			if (intAffectsSpells)
			{
				if (limit == short.MaxValue)
				{
					limit = short.MaxValue - 512; //prevent overflow when INT is added
				}
				Data[0x723CC] = (byte)(limit & 0x00ff);
				Data[0x723D1] = (byte)((limit >> 8) & 0x00ff);
			}
		}

		public void EnableSoftInBattle()
		{
			var spellInfos = LoadSpells().ToList();
			var spells = GetSpells().ToDictionary(s => s.Name.ToLowerInvariant());


			foreach (var spl in spells.Where(s => s.Key.StartsWith("soft") || s.Key.StartsWith("sft")).Select(s => s.Value))
			{
				SpellInfo spell = new SpellInfo
				{
					routine = 0x08, //cure ailment
					effect = 0x02, //earth element
					targeting = 0x10, //single target
					accuracy = 00,
					elem = 0,
					gfx = 184,
					palette = 40
				};

				Put(MagicOffset + spl.Index * MagicSize, spell.compressData());
			}
		}

		public void EnableLifeInBattle(Flags flags)
		{
			var spellInfos = LoadSpells().ToList();
			var spells = GetSpells().ToDictionary(s => s.Name.ToLowerInvariant());


			foreach (var spl in spells.Where(s => s.Key.StartsWith("life") || s.Key.StartsWith("lif")).Select(s => s.Value))
			{
				if ((spl.oobSpellRoutine == OOBSpellRoutine.LIFE && flags.EnableLifeInBattle == LifeInBattleSetting.LifeInBattleLife1Only) || (flags.EnableLifeInBattle == LifeInBattleSetting.LifeInBattleAll))
				{

					SpellInfo spell = new SpellInfo
					{
						routine = 0x08, //cure ailment
						effect = spl.oobSpellRoutine == OOBSpellRoutine.LIF2 ? (byte)0x81 : (byte)0x01, //death element
						targeting = 0x10, //single target
						accuracy = 00,
						elem = 0,
						gfx = 224,
						palette = spl.oobSpellRoutine == OOBSpellRoutine.LIF2 ? (byte)44 : (byte)43,
					};

					Put(MagicOffset + spl.Index * MagicSize, spell.compressData());
				}
			}
		}
	}
}
