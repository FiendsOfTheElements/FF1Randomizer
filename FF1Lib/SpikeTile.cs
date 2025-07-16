using RomUtilities;
using System.ComponentModel;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public enum TrapTileMode
	{
		[Description("Vanilla")]
		Vanilla,
		[Description("Shuffle")]
		Shuffle,
		[Description("A-Side Formations")]
		ASideFormations,
		[Description("B-Side Formations")]
		BSideFormations,
		[Description("A-Side & B-Side")]
		Random,
		[Description("Local Formations")]
		LocalFormations,
		[Description("Remove Trap Tiles")]
		Remove,
		[Description("Curated")]
		Curated,
		[Description("Overpowered")]
		Overpowered
	}

	public enum TrapTileOffsets : int
	{
		NWCastleMummy = 217,
		NWCastleImage = 218,
		OrdealsMudGol = 219,
		OrdealsNightmare = 220,
		OrdealsZombieD = 221,
		HallOfGiants1 = 283,
		HallOfGiants2 = 284,
		EarthElemental = 285,
		WzOgreEarth = 286,
		EarthSphinx = 289,
		VolcanoFireElemental = 290,
		VolcanoGreyWorm = 291,
		VolcanoAgama = 303,
		VolcanoRedD = 320,
		IceUndeadPack = 448,
		IceFrWolfPack = 449,
		FrGiantWolfPack = 450,
		IceMagePack = 451,
		IceFrostDPack = 452,
		IceEyeTile = 453,
		WaterfallMummyPack = 456,
		WizardMarshMajor = 537,
		WizardMarshKey = 538,
		CobraMarshKey = 539,
		BlueD = 540,
		GargoyleToF = 672,
		SeaPartyPack = 673,
		Sharknado = 674,
		SeaWaterElemental = 675,
		SeaMummy = 676,
		GargoyleToF2 = 724,
		Phantom = 982
	}


	public partial class TileSM
	{
		private const int FirstBossEncounterIndex = 0x73;
		private const int LastBossEncounterIndex = 0x7F;

		public bool IsBattleTile => PropertyType == 0x0A;
		public bool IsRandomBattleTile => IsBattleTile && ((PropertyValue & 0x80) != 0x00);
		public bool IsNonBossTrapTile => IsBattleTile && PropertyValue > 0 && PropertyValue < FirstBossEncounterIndex;
		public bool IsNonBossTrapTileEx => IsBattleTile && ((PropertyValue > 0 && PropertyValue < FirstBossEncounterIndex) || PropertyValue > LastBossEncounterIndex);
		public bool IsBossTrapTile => IsBattleTile && PropertyValue <= LastBossEncounterIndex && PropertyValue >= FirstBossEncounterIndex;

	}
	public partial class TileSetsData
	{
		public static readonly Dictionary<TrapTileOffsets, List<MapIndex>> TrapTileMapIndex = new Dictionary<TrapTileOffsets, List<MapIndex>>
		{
			{TrapTileOffsets.NWCastleMummy, new List<MapIndex>{MapIndex.NorthwestCastle} },
			{TrapTileOffsets.NWCastleImage, new List<MapIndex>{MapIndex.NorthwestCastle} },
			{TrapTileOffsets.OrdealsMudGol, new List<MapIndex>{MapIndex.CastleOrdeals2F, MapIndex.CastleOrdeals3F} },
			{TrapTileOffsets.OrdealsNightmare, new List<MapIndex>{MapIndex.CastleOrdeals2F, MapIndex.CastleOrdeals3F} },
			{TrapTileOffsets.OrdealsZombieD, new List<MapIndex>{MapIndex.CastleOrdeals2F, MapIndex.CastleOrdeals3F} },
			{TrapTileOffsets.HallOfGiants1, new List<MapIndex>{MapIndex.EarthCaveB1, MapIndex.EarthCaveB2, MapIndex.EarthCaveB3, MapIndex.EarthCaveB4, MapIndex.EarthCaveB5} },
			{TrapTileOffsets.HallOfGiants2, new List<MapIndex>{MapIndex.EarthCaveB1, MapIndex.EarthCaveB2, MapIndex.EarthCaveB3, MapIndex.EarthCaveB4, MapIndex.EarthCaveB5}},
			{TrapTileOffsets.EarthElemental, new List<MapIndex>{MapIndex.EarthCaveB1, MapIndex.EarthCaveB2, MapIndex.EarthCaveB3, MapIndex.EarthCaveB4, MapIndex.EarthCaveB5} },
			{TrapTileOffsets.WzOgreEarth, new List<MapIndex>{MapIndex.EarthCaveB1, MapIndex.EarthCaveB2, MapIndex.EarthCaveB3, MapIndex.EarthCaveB4, MapIndex.EarthCaveB5} },
			{TrapTileOffsets.EarthSphinx, new List<MapIndex>{MapIndex.EarthCaveB1, MapIndex.EarthCaveB2, MapIndex.EarthCaveB3, MapIndex.EarthCaveB4, MapIndex.EarthCaveB5} },
			{TrapTileOffsets.VolcanoFireElemental, new List<MapIndex>{MapIndex.GurguVolcanoB1, MapIndex.GurguVolcanoB2, MapIndex.GurguVolcanoB3, MapIndex.GurguVolcanoB4, MapIndex.GurguVolcanoB5} },
			{TrapTileOffsets.VolcanoGreyWorm, new List<MapIndex>{MapIndex.GurguVolcanoB1, MapIndex.GurguVolcanoB2, MapIndex.GurguVolcanoB3, MapIndex.GurguVolcanoB4, MapIndex.GurguVolcanoB5} },
			{TrapTileOffsets.VolcanoAgama, new List<MapIndex>{MapIndex.GurguVolcanoB4, MapIndex.GurguVolcanoB5} },
			{TrapTileOffsets.VolcanoRedD, new List<MapIndex>{MapIndex.GurguVolcanoB4, MapIndex.GurguVolcanoB5} },
			{TrapTileOffsets.IceUndeadPack, new List<MapIndex>{MapIndex.IceCaveB1, MapIndex.IceCaveB2, MapIndex.IceCaveB3} },
			{TrapTileOffsets.IceFrWolfPack, new List<MapIndex>{MapIndex.IceCaveB1, MapIndex.IceCaveB2, MapIndex.IceCaveB3} },
			{TrapTileOffsets.FrGiantWolfPack, new List<MapIndex>{MapIndex.IceCaveB1, MapIndex.IceCaveB2, MapIndex.IceCaveB3} },
			{TrapTileOffsets.IceMagePack, new List<MapIndex>{MapIndex.IceCaveB1, MapIndex.IceCaveB2, MapIndex.IceCaveB3} },
			{TrapTileOffsets.IceFrostDPack, new List<MapIndex>{MapIndex.IceCaveB1, MapIndex.IceCaveB2, MapIndex.IceCaveB3} },
			{TrapTileOffsets.IceEyeTile, new List<MapIndex>{MapIndex.IceCaveB1, MapIndex.IceCaveB2, MapIndex.IceCaveB3} },
			{TrapTileOffsets.WaterfallMummyPack, new List<MapIndex>{MapIndex.Waterfall} },
			{TrapTileOffsets.WizardMarshMajor, new List<MapIndex>{MapIndex.MarshCaveB1, MapIndex.MarshCaveB2, MapIndex.MarshCaveB3 } },
			{TrapTileOffsets.WizardMarshKey, new List<MapIndex>{MapIndex.MarshCaveB1, MapIndex.MarshCaveB2, MapIndex.MarshCaveB3 } },
			{TrapTileOffsets.CobraMarshKey, new List<MapIndex>{MapIndex.MarshCaveB1, MapIndex.MarshCaveB2, MapIndex.MarshCaveB3 } },
			{TrapTileOffsets.BlueD, new List<MapIndex>{MapIndex.MirageTower1F, MapIndex.MirageTower2F, MapIndex.MirageTower3F, MapIndex.SkyPalace1F, MapIndex.SkyPalace2F, MapIndex.SkyPalace3F, MapIndex.SkyPalace4F, MapIndex.SkyPalace5F } },
			{TrapTileOffsets.GargoyleToF, new List<MapIndex>{MapIndex.TempleOfFiends}},
			{TrapTileOffsets.SeaPartyPack, new List<MapIndex>{MapIndex.SeaShrineB2, MapIndex.SeaShrineB3, MapIndex.SeaShrineB4, MapIndex.SeaShrineB5 } },
			{TrapTileOffsets.Sharknado, new List<MapIndex>{MapIndex.SeaShrineB2, MapIndex.SeaShrineB3, MapIndex.SeaShrineB4, MapIndex.SeaShrineB5 }},
			{TrapTileOffsets.SeaWaterElemental, new List<MapIndex>{MapIndex.SeaShrineB4, MapIndex.SeaShrineB5 } },
			{TrapTileOffsets.SeaMummy, new List<MapIndex>{MapIndex.SeaShrineB4, MapIndex.SeaShrineB5 } },
			{TrapTileOffsets.GargoyleToF2,  new List<MapIndex>{MapIndex.TempleOfFiends} },
			{TrapTileOffsets.Phantom,  new List<MapIndex>{MapIndex.TempleOfFiendsRevisited1F, MapIndex.TempleOfFiendsRevisited2F, MapIndex.TempleOfFiendsRevisited3F, MapIndex.TempleOfFiendsRevisitedEarth, MapIndex.TempleOfFiendsRevisitedFire, MapIndex.TempleOfFiendsRevisitedWater, MapIndex.TempleOfFiendsRevisitedAir}}
		};
		public void UpdateTrapTiles(FF1Rom rom, ZoneFormations zoneformations, Flags flags, MT19337 rng)
		{
			UpdateTrapTilesCode(rom);

			TrapTileMode mode = flags.EnemyTrapTiles;
			if (mode == TrapTileMode.Vanilla)
			{
				return;
			}
			else if (mode == TrapTileMode.Remove)
			{
				RemoveTrapTiles(true);
			}
			else
			{
				ShuffleTrapTiles(zoneformations, rng, mode, (bool)flags.FightBahamut);
			}

		}
		public void UpdateTrapTilesCode(FF1Rom rom)
		{
			// This is magic BNE code that enables formation 1 trap tiles but we have to change
			// all the 0x0A 0x80 into 0x0A 0x00 and use 0x00 for random encounters instead of 0x80.
			rom.Put(0x7CDC5, Blob.FromHex("D0"));

			foreach (var tileset in tileSets)
			{
				tileset.Tiles.ForEach(tile =>
				{
					if (tile.IsRandomBattleTile)
					{
						tile.PropertyValue = 0x00;
					}
				});
			}
		}

		public void RemoveTrapTiles(bool extendedtraptiles)
		{
			// This must be called before shuffle trap tiles since it uses the vanilla format for random encounters
			//var tilesets = Get(TilesetDataOffset, TilesetDataCount * TilesetDataSize * TilesetCount).Chunk(TilesetDataSize).ToList();
			foreach (var tileset in tileSets)
			{
				tileset.Tiles.ForEach(tile =>
				{
					if (extendedtraptiles ? tile.IsNonBossTrapTileEx : tile.IsNonBossTrapTile)
					{
						tile.PropertyValue = (byte)(extendedtraptiles ? 0x00 : 0x80);
					}
				});
			}
		}

		public void ShuffleTrapTiles(ZoneFormations zoneformations, MT19337 rng, TrapTileMode mode, bool fightBahamut)
		{ 
			if (mode == TrapTileMode.LocalFormations)
			{
				//code from local formations MIAB
				// Formations List for Vanilla Spikes & Local Formations
				List<byte> castleEncounters = new() { 0x84, 0x8C, 0x8D, 0x92 };
				List<byte> caveEncounters = new() { 0x9D, 0x9C, 0x95, 0x97 };
				List<byte> cardiaEncounters = new() { 0xAA, 0xB0, 0xCB, 0xCE, 0xD9 };

				List<List<byte>> encountersGroup = zoneformations.GetBytes();

				encountersGroup[(int)MapIndex.ConeriaCastle1F] = castleEncounters;
				encountersGroup[(int)MapIndex.ElflandCastle] = castleEncounters;
				encountersGroup[(int)MapIndex.NorthwestCastle] = castleEncounters;
				encountersGroup[(int)MapIndex.CastleOrdeals1F] = castleEncounters;

				encountersGroup[(int)MapIndex.Cardia] = cardiaEncounters;
				encountersGroup[(int)MapIndex.BahamutCaveB1] = cardiaEncounters;
				encountersGroup[(int)MapIndex.BahamutCaveB2] = cardiaEncounters;

				encountersGroup[(int)MapIndex.DwarfCave] = caveEncounters;
				encountersGroup[(int)MapIndex.SardasCave] = caveEncounters;
				encountersGroup[(int)MapIndex.MatoyasCave] = caveEncounters;

				//use index to lookup tileset data to MapIndex
				//Blob tile;
				MapIndex pickedMap;
				foreach (var tileset in tileSets)
				{
					foreach (var tile in tileset.Tiles)
					{
						TrapTileOffsets traptileindex = (TrapTileOffsets)(tile.Index + 128 * tileset.Index);
						if (tile.IsNonBossTrapTile && TrapTileMapIndex.ContainsKey(traptileindex))
						{
							pickedMap = TrapTileMapIndex[traptileindex].PickRandom(rng);
							tile.PropertyValue = encountersGroup[(int)pickedMap].SpliceRandom(rng);
						}
					}
				}
			}
			else
			{
				List<byte> encounters;
				if (mode == TrapTileMode.Shuffle)
				{
					var traps = tileSets.SelectMany(t => t.Tiles).Where(t => t.IsNonBossTrapTile).ToList();
					encounters = traps.Select(trap => trap.PropertyValue).ToList();
				}
				else if (mode == TrapTileMode.ASideFormations)
				{
					//all random
					encounters = new(FormationLists.ASideEncounters);
					if (fightBahamut)
					{
						encounters = encounters.Except(FormationLists.BahamutEncounter).ToList();
					}
				}
				else if (mode == TrapTileMode.BSideFormations)
				{
					encounters = new(FormationLists.BSideEncounters);
					if (fightBahamut)
					{
						encounters = encounters.Except(FormationLists.BahamutEncounter).ToList();
					}
				}
				else if (mode == TrapTileMode.Random)
				{
					//all random
					encounters = new(FormationLists.AllRandomEncounters);
					if (fightBahamut)
					{
						encounters = encounters.Except(FormationLists.BahamutEncounter).ToList();
					}
				}
				else if (mode == TrapTileMode.Curated)
				{
					//balanced/curated mode
					//this mode is really just in here so tournament organizers know that it's possible
					encounters = new(FormationLists.AllRandomEncounters);

					//remove the god grinds
					encounters.Remove(0x69);        //1 eye tile.
					encounters.Remove(0x69 + 0x80); //3 eye tile.
					encounters.Remove(0x56 + 0x80); //2 fighter.
					encounters.Remove(0x3C);        //1 sandworm
					encounters.Remove(0x3C + 0x80); //1-2 sandworm

					//remove the too hard/annoying encounters
					encounters.Remove(0x21 + 0x80); //2-4 Earth
					encounters.Remove(0x27 + 0x80); //3-4 Fire
					encounters.Remove(0x38);        //1-2 rankylo
					encounters.Remove(0x38 + 0x80); //4 rankylo
					encounters.Remove(0x4E + 0x80); //3 blue d
					encounters.Remove(0x3B + 0x80); //3-4 chimera
					encounters.Remove(0x4D + 0x80); //5-9 badmen
					encounters.Remove(0x49 + 0x80); //3-6 waters
					encounters.Remove(0x51 + 0x80); //3-6 airs
					encounters.Remove(0x57 + 0x80); //3-4 worm

					//remove the worst enemies in the game
					encounters.Remove(0x6A); //2-5 rgoyles
					encounters.Remove(0x6A + 0x80); //3-7 rgoyles

					if (fightBahamut)
					{
						encounters.Remove(0x80 + 0x71); // ANKYLO (used for Bahamut)
						encounters.Remove(0x71); // ANKYLO (used for Bahamut)
					}
				}
				else
				{
					//UNbalanced/curated mode
					//this mode is really just in here so we have the option to make spike tiles overpowered
					encounters = new(FormationLists.AllRandomEncounters);
					encounters.Add(0x56);
					/*
					//keep the god grinds
					encounters.Remove(0x69);        //1 eye tile.
					encounters.Remove(0x69 + 0x80); //3 eye tile.
					encounters.Remove(0x56 + 0x80); //2 fighter.
					encounters.Remove(0x3C);        //1 sandworm
					encounters.Remove(0x3C + 0x80); //1-2 sandworm
					*/

					//remove the too hard/annoying encounters
					encounters.Remove(0x21 + 0x80); //2-4 Earth
					encounters.Remove(0x27 + 0x80); //3-4 Fire
					encounters.Remove(0x38);        //1-2 rankylo
					encounters.Remove(0x38 + 0x80); //4 rankylo
													//encounters.Remove(0x4E + 0x80); //3 blue d
													//encounters.Remove(0x3B + 0x80); //3-4 chimera
					encounters.Remove(0x4D + 0x80); //5-9 badmen
													//encounters.Remove(0x49 + 0x80); //3-6 waters
					encounters.Remove(0x51 + 0x80); //3-6 airs
													//encounters.Remove(0x57 + 0x80); //3-4 worm

					//remove the worst enemies in the game
					encounters.Remove(0x6A); //2-5 rgoyles
					encounters.Remove(0x6A + 0x80); //3-7 rgoyles

					//foreach (var tileset in tileSets)
					//{
					//	tileset.Tiles.ForEach(tile =>
							//if (fightBahamut)
							//{
						encounters.Remove(0x80 + 0x71); // ANKYLO (used for Bahamut)
						encounters.Remove(0x71); // ANKYLO (used for Bahamut)
					//}
					//these can be not on the list regardless of whether fight Bahamut is on

					//remove most of the encounters
					for (int i = 0; i < 0x17; i++) {  //stop before Pedes 0x17
						encounters.Remove((byte)i);
						encounters.Remove((byte)(0x80 + i));
					}
					encounters.Remove(0x80 + 0x17);
					for (int i = 0x18; i < 0x1E; i++) //stop before Giants 0x1E
					{
						encounters.Remove((byte)i);
						encounters.Remove((byte)(0x80 + i));
					}
					encounters.Remove(0x80 + 0x1E);
					for (int i = 0x1F; i < 0x24; i++) //stop before R.Hydra 0x24 and Ochos 0x25 and R.Giants 0x26
					{
						encounters.Remove((byte)i);
						encounters.Remove((byte)(0x80 + i));
					}
					encounters.Remove(0x80 + 0x25);
					encounters.Remove(0x27); //1-2 fires
					encounters.Remove(0x28); // grey Worm
					encounters.Remove(0x80 + 0x28);
					//skip 0x29 agama and 0x2A Red D
					encounters.Remove(0x2B);
					encounters.Remove(0x2C);
					encounters.Remove(0x80 + 0x2B);
					encounters.Remove(0x80 + 0x2C);
					//skip FrWolfs 0x2D
					encounters.Remove(0x2E); //FrWolfs + FrGiants
					encounters.Remove(0x80 + 0x2E); //FrWolfs + FrGiants
													//skip Mages 0x2F
					for (int i = 0x30; i < 0x3B; i++) //stop before Chimeras 0x3B, sandworm, and both steaks 0x3E
					{
						encounters.Remove((byte)i);
						encounters.Remove((byte)(0x80 + i));
					}
					encounters.Remove(0x80 + 0x3E);
					encounters.Remove(0x3F);//mud gols
					encounters.Remove(0x08 + 0x3F);//mud gols
					encounters.Remove(0x40);//grmedusas
					encounters.Remove(0x80 + 0x40);//grmedusas
												   //NOACHO skip 0x41
					for (int i = 0x42; i < 0x45; i++) //stop before GrShark+WizSahag 0x45
					{
						encounters.Remove((byte)i);
					}
					encounters.Remove(0x80 + 0x45);
					encounters.Remove(0x46);//Phantom
					encounters.Remove(0x80 + 0x46);
					encounters.Remove(0x47);//Naga Water
											//skip bigeye grshark 0x48
					encounters.Remove(0x49);//1-3 waters
											//skip wizMumies 0x4A and Zombie Ds 0x4B
					encounters.Remove(0x4A);//mummies, wiz mumies, conctrice
					encounters.Remove(0x80 + 0x4A);
					for (int i = 0x4C; i < 0x4E; i++) //stop before Blue D 0x4E
					{
						encounters.Remove((byte)i);
					}
					encounters.Remove(0x80 + 0x4C);
					encounters.Remove(0x4F);//nitemares
					encounters.Remove(0x80 + 0x4F);//nitemares
												   //skip slimes 0x50
					encounters.Remove(0x51);//2-4 air
					encounters.Remove(0x80 + 0x51);
					encounters.Remove(0x52);//Gr Naga + air
					encounters.Remove(0x80 + 0x52);
					//skip wz vamps
					encounters.Remove(0x54);//Nitemares + evilman
					encounters.Remove(0x80 + 0x54);
					//skip Jimera, Warmech, Worms, RockGol, Gas D,
					encounters.Remove(0x80 + 0x55);
					for (int i = 0x5A; i < 0x69; i++) //stop before Eye 0x69
					{
						encounters.Remove((byte)i);
						encounters.Remove((byte)(0x80 + i));
					}
					for (int i = 0x6A; i < 0x73; i++) //stop before Lich 0x73
					{
						encounters.Remove((byte)i);
						encounters.Remove((byte)(0x80 + i));
					}
					encounters.Remove(0x80 + 0x7E);//R + Wiz Sahags
				}

				foreach (var tileset in tileSets)
				{
					tileset.Tiles.ForEach(tile =>
					{
						if (tile.IsNonBossTrapTileEx)
						{
							tile.PropertyValue = encounters.SpliceRandom(rng);
						}
					});
				}
			}
		}
	}
}
