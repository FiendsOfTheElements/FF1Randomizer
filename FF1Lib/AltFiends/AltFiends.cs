using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public partial class FF1Rom
	{
		public struct AlternateFiends
		{
			public string Name;
			public List<byte> Spells1;
			public List<byte> Skills1;
			public byte SpellChance1;
			public byte SkillChance1;
			public List<byte> Spells2;
			public List<byte> Skills2;
			public byte SpellChance2;
			public byte SkillChance2;
			public MonsterType MonsterType;
			public SpellElement ElementalWeakness;
			public FormationSpriteSheet SpriteSheet;
			public FormationPattern FormationPattern;
			public int Palette1;
			public int Palette2;
			public FormationGFX GFXOffset;

		}
		public void AlternativeFiends(ExtAltFiends extAltFiends, EnemyScripts enemyScripts, MT19337 rng, Flags flags)
		{
			if (!(bool)flags.AlternateFiends || flags.SpookyFlag)
			{
				return;
			}

			const int FiendsIndex = 0x77;
			const int FiendsScriptIndex = 0x22;
			var fiendsFormationOrder = new List<int> { 0x7A, 0x73, 0x79, 0x74, 0x78, 0x75, 0x77, 0x76 };

			//Oslodo's note; if you are going to test new altfiends, you will need to have at least 7 in the list or the code will hang
			//Barber's note: all the Alt Fiend lists have been broken out to separate files.

			var alternateFiendsList = new List<AlternateFiends>
			{


			};
			if ((bool)flags.FinalFantasy2Fiends /*&& (bool)!flags.HardcoreAltFiends*/)

			{
				alternateFiendsList.AddRange(FF2AltFiendslist);
			}

			if ((bool)flags.FinalFantasy3Fiends /*&& (bool)!flags.HardcoreAltFiends*/)

			{
				alternateFiendsList.AddRange(FF3AltFiendslist);
			}

			if ((bool)flags.FinalFantasy4Fiends /*&& (bool)!flags.HardcoreAltFiends*/)

			{
				alternateFiendsList.AddRange(FF4AltFiendsList);
			}

			if ((bool)flags.FinalFantasy5Fiends /*&& (bool)!flags.HardcoreAltFiends*/)

			{
				alternateFiendsList.AddRange(FF5AltFiendsList);
			}

			if ((bool)flags.FinalFantasy6Fiends /*&& (bool)!flags.HardcoreAltFiends*/)

			{
				alternateFiendsList.AddRange(FF6AltFiendsList);
			}

			if ((bool)flags.FinalFantasy1BonusFiends /*&& (bool)!flags.HardcoreAltFiends*/)
			{
				alternateFiendsList.AddRange(FF1BonusFiendsList);
			}

			if ((bool)flags.WinnerCircleFiends /*&& (bool)!flags.HardcoreAltFiends*/)
			{
				alternateFiendsList.AddRange(WinnerCircleFiendsList);
			}

				if ((bool)flags.BlackOrbFiends)
			{
				alternateFiendsList.AddRange(extAltFiends.BlackOrbAltFiends);
			}

			if ((bool)!flags.FinalFantasy2Fiends && (bool)!flags.FinalFantasy3Fiends && (bool)!flags.FinalFantasy4Fiends && (bool)!flags.FinalFantasy5Fiends && (bool)!flags.FinalFantasy6Fiends && (bool)!flags.FinalFantasy1BonusFiends && (bool)!flags.WinnerCircleFiends && (bool)!flags.BlackOrbFiends)
			{
				alternateFiendsList.AddRange(FF1MasterFiendList);
			}

			var encountersData = new Encounters(this);

			EnemyInfo[] fiends = new EnemyInfo[8];
			EnemyScriptInfo[] fiendsScript = enemyScripts.GetList().Where(s => s.index >= FiendsScriptIndex && s.index <= (FiendsScriptIndex + 7)).ToArray();

			for (int i = 0; i < 8; i++)
			{
				fiends[i] = new EnemyInfo();
				fiends[i].decompressData(Get(EnemyOffset + (FiendsIndex + i) * EnemySize, EnemySize));
			}

			// Do Graphics
			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
		
						
			if (extAltFiends.ExtendedFiends)
			{
				// These alt fiends all fit together, so they shouldn't trigger a too large graphics error
				alternateFiendsList = extAltFiends.PickBlackOrbFiends(alternateFiendsList, rng);

				var resourcePath1 = assembly.GetManifestResourceNames().First(str => str.EndsWith(alternateFiendsList[0].Name + ".png"));
				var resourcePath2 = assembly.GetManifestResourceNames().First(str => str.EndsWith(alternateFiendsList[1].Name + ".png"));
				using (Stream stream1 = assembly.GetManifestResourceStream(resourcePath1))
				{
					using (Stream stream2 = assembly.GetManifestResourceStream(resourcePath2))
					{
						if (!SetLichKaryGraphics(stream1, stream2))
						{
							throw new Exception("Extended Alt Fiends LichKary graphics error.");
						}
					}
				}
				resourcePath1 = assembly.GetManifestResourceNames().First(str => str.EndsWith(alternateFiendsList[2].Name + ".png"));
				resourcePath2 = assembly.GetManifestResourceNames().First(str => str.EndsWith(alternateFiendsList[3].Name + ".png"));
				using (Stream stream1 = assembly.GetManifestResourceStream(resourcePath1))
				{
					using (Stream stream2 = assembly.GetManifestResourceStream(resourcePath2))
					{
						if (!SetKrakenTiamatGraphics(stream1, stream2))
						{
							throw new Exception("Extended Alt Fiends KrakenTiamat graphics error.");
						}
					}
				}
			}
			else
			{
				while (true)
				{
					// Shuffle alternate
					alternateFiendsList.Shuffle(rng);

					while (alternateFiendsList.Count >= 4)
					{
						var resourcePath1 = assembly.GetManifestResourceNames().First(str => str.EndsWith("bosses." + alternateFiendsList[0].Name + ".png"));
						var resourcePath2 = assembly.GetManifestResourceNames().First(str => str.EndsWith("bosses." + alternateFiendsList[1].Name + ".png"));
						// Console.WriteLine($"Trying to import {alternateFiendsList[0].Name} and {alternateFiendsList[1].Name}.");
						using (Stream stream1 = assembly.GetManifestResourceStream(resourcePath1))
						{
							using (Stream stream2 = assembly.GetManifestResourceStream(resourcePath2))
							{
								//if (await SetLichKaryGraphics(stream1, stream2)) {
								if (SetLichKaryGraphics(stream1, stream2))
								{
									break;
								}
								// The graphics didn't fit, throw out the first element and try the next pair
								alternateFiendsList.RemoveAt(0);
							}
						}
					}
					if (alternateFiendsList.Count < 4)
					{
						// Couldn't find a pair where the graphics fit, reshuffle
						continue;
					}

					while (alternateFiendsList.Count >= 4)
					{
						var resourcePath1 = assembly.GetManifestResourceNames().First(str => str.EndsWith("bosses." + alternateFiendsList[2].Name + ".png"));
						var resourcePath2 = assembly.GetManifestResourceNames().First(str => str.EndsWith("bosses." + alternateFiendsList[3].Name + ".png"));
						// Console.WriteLine($"Trying to import {alternateFiendsList[2].Name} and {alternateFiendsList[3].Name}.");
						using (Stream stream1 = assembly.GetManifestResourceStream(resourcePath1))
						{
							using (Stream stream2 = assembly.GetManifestResourceStream(resourcePath2))
							{
								//if (await SetKrakenTiamatGraphics(stream1, stream2)) {
								if (SetKrakenTiamatGraphics(stream1, stream2))
								{
									break;
								}
								alternateFiendsList.RemoveAt(2);
							}
						}
					}
					if (alternateFiendsList.Count < 4)
					{
						continue;
					}
					break;
				}
			}

			// Replace the 4 fiends and their 2nd version at the same time
			for (int i = 0; i < 4; i++)
			{
				var scriptIndex = FiendsScriptIndex + (i * 2);

				fiends[(i * 2)].monster_type = (byte)alternateFiendsList[i].MonsterType;
				fiends[(i * 2) + 1].monster_type = (byte)alternateFiendsList[i].MonsterType;
				fiends[(i * 2)].elem_weakness = (byte)alternateFiendsList[i].ElementalWeakness;
				fiends[(i * 2) + 1].elem_weakness = 0x00;
				fiends[(i * 2)].elem_resist = (byte)(fiends[(i * 2)].elem_resist & ~(byte)alternateFiendsList[i].ElementalWeakness);
				fiends[(i * 2) + 1].elem_resist = (byte)(fiends[(i * 2) + 1].elem_resist & ~(byte)alternateFiendsList[i].ElementalWeakness);

				if (enemyScripts[scriptIndex].skill_chance == 0x00 || alternateFiendsList[i].SkillChance1 == 0x00)
					enemyScripts[scriptIndex].skill_chance = alternateFiendsList[i].SkillChance1;

				if (enemyScripts[scriptIndex + 1].skill_chance == 0x00 || alternateFiendsList[i].SkillChance2 == 0x00)
					enemyScripts[scriptIndex + 1].skill_chance = alternateFiendsList[i].SkillChance2;

				enemyScripts[scriptIndex].skill_list = alternateFiendsList[i].Skills1.ToArray();
				enemyScripts[scriptIndex + 1].skill_list = alternateFiendsList[i].Skills2.ToArray();

				if (enemyScripts[scriptIndex].spell_chance == 0x00 || alternateFiendsList[i].SpellChance1 == 0x00)
					enemyScripts[scriptIndex].spell_chance = alternateFiendsList[i].SpellChance1;

				if (enemyScripts[scriptIndex + 1].spell_chance == 0x00 || alternateFiendsList[i].SpellChance2 == 0x00)
					enemyScripts[scriptIndex + 1].spell_chance = alternateFiendsList[i].SpellChance2;

				enemyScripts.ImportVanillaSpellList(scriptIndex, alternateFiendsList[i].Spells1);
				enemyScripts.ImportVanillaSpellList(scriptIndex + 1, alternateFiendsList[i].Spells2);
			}

			encountersData.Write(this);

			for (int i = 0; i < 8; i++)
			{
				Put(EnemyOffset + (FiendsIndex + i) * EnemySize, fiends[i].compressData());
			}

			
			//var enemyText = ReadEnemyText();
			//Console.WriteLine("Final alternate fiends list in order:");
			for (int i = 0; i < 4; i++)
			{
				//Console.WriteLine(alternateFiendsList[i].Name);
				EnemyText[119 + (i * 2)] = alternateFiendsList[i].Name;
				EnemyText[120 + (i * 2)] = alternateFiendsList[i].Name;
			}

			//WriteEnemyText(enemyText);
		}
	}
}
