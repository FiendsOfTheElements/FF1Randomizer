using System.ComponentModel;

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
		Curated
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

	public partial class FF1Rom : NesRom
	{
		public static readonly Dictionary<TrapTileOffsets, List<MapId>> TrapTileMapId = new Dictionary<TrapTileOffsets, List<MapId>>
		{
			{TrapTileOffsets.NWCastleMummy, new List<MapId>{MapId.NorthwestCastle} },
			{TrapTileOffsets.NWCastleImage, new List<MapId>{MapId.NorthwestCastle} },
			{TrapTileOffsets.OrdealsMudGol, new List<MapId>{MapId.CastleOfOrdeals2F, MapId.CastleOfOrdeals3F} },
			{TrapTileOffsets.OrdealsNightmare, new List<MapId>{MapId.CastleOfOrdeals2F, MapId.CastleOfOrdeals3F} },
			{TrapTileOffsets.OrdealsZombieD, new List<MapId>{MapId.CastleOfOrdeals2F, MapId.CastleOfOrdeals3F} },
			{TrapTileOffsets.HallOfGiants1, new List<MapId>{MapId.EarthCaveB1, MapId.EarthCaveB2, MapId.EarthCaveB3, MapId.EarthCaveB4, MapId.EarthCaveB5} },
			{TrapTileOffsets.HallOfGiants2, new List<MapId>{MapId.EarthCaveB1, MapId.EarthCaveB2, MapId.EarthCaveB3, MapId.EarthCaveB4, MapId.EarthCaveB5}},
			{TrapTileOffsets.EarthElemental, new List<MapId>{MapId.EarthCaveB1, MapId.EarthCaveB2, MapId.EarthCaveB3, MapId.EarthCaveB4, MapId.EarthCaveB5} },
			{TrapTileOffsets.WzOgreEarth, new List<MapId>{MapId.EarthCaveB1, MapId.EarthCaveB2, MapId.EarthCaveB3, MapId.EarthCaveB4, MapId.EarthCaveB5} },
			{TrapTileOffsets.EarthSphinx, new List<MapId>{MapId.EarthCaveB1, MapId.EarthCaveB2, MapId.EarthCaveB3, MapId.EarthCaveB4, MapId.EarthCaveB5} },
			{TrapTileOffsets.VolcanoFireElemental, new List<MapId>{MapId.GurguVolcanoB1, MapId.GurguVolcanoB2, MapId.GurguVolcanoB3, MapId.GurguVolcanoB4, MapId.GurguVolcanoB5} },
			{TrapTileOffsets.VolcanoGreyWorm, new List<MapId>{MapId.GurguVolcanoB1, MapId.GurguVolcanoB2, MapId.GurguVolcanoB3, MapId.GurguVolcanoB4, MapId.GurguVolcanoB5} },
			{TrapTileOffsets.VolcanoAgama, new List<MapId>{MapId.GurguVolcanoB4, MapId.GurguVolcanoB5} },
			{TrapTileOffsets.VolcanoRedD, new List<MapId>{MapId.GurguVolcanoB4, MapId.GurguVolcanoB5} },
			{TrapTileOffsets.IceUndeadPack, new List<MapId>{MapId.IceCaveB1, MapId.IceCaveB2, MapId.IceCaveB3} },
			{TrapTileOffsets.IceFrWolfPack, new List<MapId>{MapId.IceCaveB1, MapId.IceCaveB2, MapId.IceCaveB3} },
			{TrapTileOffsets.FrGiantWolfPack, new List<MapId>{MapId.IceCaveB1, MapId.IceCaveB2, MapId.IceCaveB3} },
			{TrapTileOffsets.IceMagePack, new List<MapId>{MapId.IceCaveB1, MapId.IceCaveB2, MapId.IceCaveB3} },
			{TrapTileOffsets.IceFrostDPack, new List<MapId>{MapId.IceCaveB1, MapId.IceCaveB2, MapId.IceCaveB3} },
			{TrapTileOffsets.IceEyeTile, new List<MapId>{MapId.IceCaveB1, MapId.IceCaveB2, MapId.IceCaveB3} },
			{TrapTileOffsets.WaterfallMummyPack, new List<MapId>{MapId.Waterfall} },
			{TrapTileOffsets.WizardMarshMajor, new List<MapId>{MapId.MarshCaveB1, MapId.MarshCaveB2, MapId.MarshCaveB3 } },
			{TrapTileOffsets.WizardMarshKey, new List<MapId>{MapId.MarshCaveB1, MapId.MarshCaveB2, MapId.MarshCaveB3 } },
			{TrapTileOffsets.CobraMarshKey, new List<MapId>{MapId.MarshCaveB1, MapId.MarshCaveB2, MapId.MarshCaveB3 } },
			{TrapTileOffsets.BlueD, new List<MapId>{MapId.MirageTower1F, MapId.MirageTower2F, MapId.MirageTower3F, MapId.SkyPalace1F, MapId.SkyPalace2F, MapId.SkyPalace3F, MapId.SkyPalace4F, MapId.SkyPalace5F } },
			{TrapTileOffsets.GargoyleToF, new List<MapId>{MapId.TempleOfFiends}},
			{TrapTileOffsets.SeaPartyPack, new List<MapId>{MapId.SeaShrineB2, MapId.SeaShrineB3, MapId.SeaShrineB4, MapId.SeaShrineB5 } },
			{TrapTileOffsets.Sharknado, new List<MapId>{MapId.SeaShrineB2, MapId.SeaShrineB3, MapId.SeaShrineB4, MapId.SeaShrineB5 }},
			{TrapTileOffsets.SeaWaterElemental, new List<MapId>{MapId.SeaShrineB4, MapId.SeaShrineB5 } },
			{TrapTileOffsets.SeaMummy, new List<MapId>{MapId.SeaShrineB4, MapId.SeaShrineB5 } },
			{TrapTileOffsets.GargoyleToF2,  new List<MapId>{MapId.TempleOfFiends} },
			{TrapTileOffsets.Phantom,  new List<MapId>{MapId.TempleOfFiendsRevisited1F, MapId.TempleOfFiendsRevisited2F, MapId.TempleOfFiendsRevisited3F, MapId.TempleOfFiendsRevisitedEarth, MapId.TempleOfFiendsRevisitedFire, MapId.TempleOfFiendsRevisitedWater, MapId.TempleOfFiendsRevisitedAir}}
		};

		bool IsBattleTile(Blob tuple) => tuple[0] == 0x0A;
		bool IsRandomBattleTile(Blob tuple) => IsBattleTile(tuple) && (tuple[1] & 0x80) != 0x00;
		bool IsNonBossTrapTile(Blob tuple) => IsBattleTile(tuple) && tuple[1] > 0 && tuple[1] < FirstBossEncounterIndex;
		bool IsNonBossTrapTileEx(Blob tuple) => IsBattleTile(tuple) && ((tuple[1] > 0 && tuple[1] < FirstBossEncounterIndex) || tuple[1] > LastBossEncounterIndex);
		bool IsBossTrapTile(Blob tuple) => IsBattleTile(tuple) && tuple[1] <= LastBossEncounterIndex && tuple[1] >= FirstBossEncounterIndex;

		public void RemoveTrapTiles(bool extendedtraptiles)
		{
			// This must be called before shuffle trap tiles since it uses the vanilla format for random encounters
			var tilesets = Get(TilesetDataOffset, TilesetDataCount * TilesetDataSize * TilesetCount).Chunk(TilesetDataSize).ToList();
			tilesets.ForEach(tile =>
			{

				if (extendedtraptiles ? IsNonBossTrapTileEx(tile) : IsNonBossTrapTile(tile))
				{
					tile[1] = (byte)(extendedtraptiles ? 0x00 : 0x80);
				}
			});
			Put(TilesetDataOffset, tilesets.SelectMany(tileset => tileset.ToBytes()).ToArray());
		}

		public void ShuffleTrapTiles(MT19337 rng, TrapTileMode mode, bool fightBahamut)
		{
			// This is magic BNE code that enables formation 1 trap tiles but we have to change
			// all the 0x0A 0x80 into 0x0A 0x00 and use 0x00 for random encounters instead of 0x80.
			Data[0x7CDC5] = 0xD0;
			var tilesets = Get(TilesetDataOffset, TilesetDataCount * TilesetDataSize * TilesetCount).Chunk(TilesetDataSize).ToList();

			if(mode == TrapTileMode.LocalFormations)
			{
				//code from local formations MIAB
				// Formations List for Vanilla Spikes & Local Formations
				List<byte> castleEncounters = new() { 0x84, 0x8C, 0x8D, 0x92 };
				List<byte> caveEncounters = new() { 0x9D, 0x9C, 0x95, 0x97 };
				List<byte> cardiaEncounters = new() { 0xAA, 0xB0, 0xCB, 0xCE, 0xD9 };

				List<List<byte>> encountersGroup = new();
				encountersGroup = Get(ZoneFormationsOffset + (8 * 0x40), 8 * 0x40).Chunk(0x08).Select(x => x.ToBytes().Select(y => (byte)(y)).ToList()).ToList();

				encountersGroup[(int)MapId.ConeriaCastle1F] = castleEncounters;
				encountersGroup[(int)MapId.ElflandCastle] = castleEncounters;
				encountersGroup[(int)MapId.NorthwestCastle] = castleEncounters;
				encountersGroup[(int)MapId.CastleOfOrdeals1F] = castleEncounters;

				encountersGroup[(int)MapId.Cardia] = cardiaEncounters;
				encountersGroup[(int)MapId.BahamutsRoomB1] = cardiaEncounters;
				encountersGroup[(int)MapId.BahamutsRoomB2] = cardiaEncounters;

				encountersGroup[(int)MapId.DwarfCave] = caveEncounters;
				encountersGroup[(int)MapId.SardasCave] = caveEncounters;
				encountersGroup[(int)MapId.MatoyasCave] = caveEncounters;

				//use index to lookup tileset data to mapid
				Blob tile;
				MapId pickedMap;
				for (int i = 0; i < tilesets.Count; i++)
                {
					tile = tilesets[i];
					if (IsNonBossTrapTile(tile) && TrapTileMapId.ContainsKey((TrapTileOffsets)i))
					{
						pickedMap = TrapTileMapId[(TrapTileOffsets)i].PickRandom(rng);
						tile[1] = encountersGroup[(int)pickedMap].SpliceRandom(rng);
					}
					else if (IsRandomBattleTile(tile))
					{
						tile[1] = 0x00;
					}
				}
			}
			else
			{
				List<byte> encounters;
				if (mode == TrapTileMode.Shuffle)
				{
					var traps = tilesets.Where(IsNonBossTrapTile).ToList();
					encounters = traps.Select(trap => trap[1]).ToList();
				}
				else if (mode == TrapTileMode.ASideFormations)
				{
					//all random
					encounters = Enumerable.Range(0, FirstBossEncounterIndex).Select(value => (byte)value).ToList();
					encounters.Remove(WarMECHFormationIndex);
					if (fightBahamut)
					{
						encounters.Remove(0x71); // ANKYLO (used for Bahamut)
					}
				}
				else if (mode == TrapTileMode.BSideFormations)
				{
					encounters = Enumerable.Range(128, FirstBossEncounterIndex).Select(value => (byte)value).ToList();
					encounters.Add(0xFF); // IronGOL
					if (fightBahamut)
					{
						encounters.Remove(0x80 + 0x71); // ANKYLO (used for Bahamut)
					}
				}
				else if (mode == TrapTileMode.Random)
				{
					//all random
					encounters = Enumerable.Range(0, FirstBossEncounterIndex).Select(value => (byte)value).ToList();
					encounters.Concat(Enumerable.Range(128, FirstBossEncounterIndex).Select(value => (byte)value).ToList());
					encounters.Remove(WarMECHFormationIndex);
					encounters.Add(0xFF); // IronGOL
					if (fightBahamut)
					{
						encounters.Remove(0x80 + 0x71); // ANKYLO (used for Bahamut)
						encounters.Remove(0x71); // ANKYLO (used for Bahamut)
					}
				}
				else
				{
					//balanced/curated mode
					//this mode is really just in here so tournament organizers know that it's possible
					encounters = Enumerable.Range(0, FirstBossEncounterIndex).Select(value => (byte)value).ToList();
					encounters.Concat(Enumerable.Range(128, FirstBossEncounterIndex).Select(value => (byte)value).ToList());
					encounters.Remove(WarMECHFormationIndex);

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

				tilesets.ForEach(tile =>
				{
					if (IsNonBossTrapTile(tile))
					{
						tile[1] = encounters.SpliceRandom(rng);
					}
					else if (IsRandomBattleTile(tile))
					{
						tile[1] = 0x00;
					}
				});
			}

			Put(TilesetDataOffset, tilesets.SelectMany(tileset => tileset.ToBytes()).ToArray());
		}
	}
}
