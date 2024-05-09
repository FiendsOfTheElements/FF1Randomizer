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

			UpdateMagicAutohitThreshold(rng, flags.MagicAutohitThreshold);

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

		public void UpdateMagicAutohitThreshold(MT19337 rng, AutohitThreshold threshold)
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
