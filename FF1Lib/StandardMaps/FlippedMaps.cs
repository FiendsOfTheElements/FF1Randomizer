using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class FlippedMaps
	{
		FF1Rom rom;
		StandardMaps maps;
		Flags flags;
		MT19337 rng;
		Teleporters teleporters;
		//NormTeleData tele;
		//EnterTeleData enter;

		List<MapIndex> ValidMaps_Horizontal = new List<MapIndex>
		{
			MapIndex.TempleOfFiends,
			MapIndex.EarthCaveB1, MapIndex.EarthCaveB2, MapIndex.EarthCaveB3, MapIndex.EarthCaveB4, MapIndex.EarthCaveB5,
			MapIndex.MarshCaveB1, MapIndex.MarshCaveB2, MapIndex.MarshCaveB3,
			MapIndex.IceCaveB1, MapIndex.IceCaveB2, MapIndex.IceCaveB3,
			MapIndex.MirageTower1F, MapIndex.MirageTower2F,
			MapIndex.SkyPalace1F, MapIndex.SkyPalace2F, MapIndex.SkyPalace3F,
			MapIndex.SeaShrineB1, MapIndex.SeaShrineB2, MapIndex.SeaShrineB3, MapIndex.SeaShrineB4, MapIndex.SeaShrineB5,
			MapIndex.Waterfall,
			MapIndex.GurguVolcanoB1, MapIndex.GurguVolcanoB2, MapIndex.GurguVolcanoB3, MapIndex.GurguVolcanoB4, MapIndex.GurguVolcanoB5

		};

		List<MapIndex> ValidMaps_Vertical = new List<MapIndex>
		{
			MapIndex.TempleOfFiends,
			MapIndex.EarthCaveB1, MapIndex.EarthCaveB2, MapIndex.EarthCaveB4, MapIndex.EarthCaveB3, MapIndex.EarthCaveB5,
			MapIndex.MarshCaveB1, MapIndex.MarshCaveB2, MapIndex.MarshCaveB3,
			MapIndex.IceCaveB1, MapIndex.IceCaveB2, MapIndex.IceCaveB3,
			MapIndex.MirageTower1F, MapIndex.MirageTower2F,
			MapIndex.SkyPalace1F, MapIndex.SkyPalace2F, MapIndex.SkyPalace3F,
			MapIndex.SeaShrineB1, MapIndex.SeaShrineB2, MapIndex.SeaShrineB3, MapIndex.SeaShrineB4, MapIndex.SeaShrineB5,
			MapIndex.Waterfall,
			MapIndex.GurguVolcanoB1, MapIndex.GurguVolcanoB2, MapIndex.GurguVolcanoB3, MapIndex.GurguVolcanoB4, MapIndex.GurguVolcanoB5
		};

		List<MapIndex> mapsToFlipVertically;

		public FlippedMaps(FF1Rom _rom, StandardMaps _maps, Flags _flags, Teleporters _teleporters, MT19337 _rng)
		{
			rom = _rom;
			maps = _maps;
			flags = _flags;
			rng = _rng;
			teleporters = _teleporters;
			//tele = new NormTeleData(rom);
			//enter = new EnterTeleData(rom);
		}

		public List<MapIndex> VerticalFlipStep1()
		{
			if (!(bool)flags.VerticallyFlipDungeons || flags.GameMode != GameModes.Standard)
			{
				mapsToFlipVertically = new List<MapIndex>();
				return mapsToFlipVertically;
			}

			if ((bool)flags.ProcgenEarth)
			{
				ValidMaps_Vertical.Remove(MapIndex.EarthCaveB1);
				ValidMaps_Vertical.Remove(MapIndex.EarthCaveB2);
				ValidMaps_Vertical.Remove(MapIndex.EarthCaveB3);
				ValidMaps_Vertical.Remove(MapIndex.EarthCaveB4);
				ValidMaps_Vertical.Remove(MapIndex.EarthCaveB5);
			}

			ValidMaps_Vertical.Shuffle(rng);
			mapsToFlipVertically = ValidMaps_Vertical.GetRange(0, rng.Between((int)(ValidMaps_Vertical.Count * 0.33), (int)(ValidMaps_Vertical.Count * 0.75)));
			if (flags.EFGWaterfall) mapsToFlipVertically.Remove(MapIndex.Waterfall);

			/*
			foreach (var MapIndex in mapsToFlipVertically)
			{
				var map = maps[(int)MapIndex];
				var fileName = @"d:\FFR\vfmaps\" + MapIndex.ToString() + ".ffm";
				using var stream = new FileStream(fileName, FileMode.Create);
				map.Save(stream);
				stream.Close();
			}
			*/

			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var resourcePath = assembly.GetManifestResourceNames().First(str => str.EndsWith("vfmaps.zip"));

			using var stream = assembly.GetManifestResourceStream(resourcePath);

			var archive = new ZipArchive(stream);

			foreach (var MapIndex in mapsToFlipVertically)
			{
				var map = maps[MapIndex];

				var entry = archive.GetEntry(MapIndex.ToString() + ".ffm");

				using var stream2 = entry.Open();
				map.Map.Load(stream2);
			}

			return mapsToFlipVertically;
		}

		public void VerticalFlipStep2()
		{
			//tele.LoadData();
			//enter.LoadData();

			foreach (MapIndex mapindex in mapsToFlipVertically)
			{
				var map = maps[mapindex];

				if (mapindex == MapIndex.GurguVolcanoB5)
				{
					FlipVolcanoB5(map);
				}
				else if (mapindex == MapIndex.GurguVolcanoB2)
				{
					FlipVolcanoB2(mapindex, map.Map);
				}
				else if (mapindex == MapIndex.Waterfall || mapindex == MapIndex.GurguVolcanoB3 || mapindex == MapIndex.GurguVolcanoB4)
				{
					map.Map.FlipVertical();

					FlipNormalTele(mapindex, true);
					FlipEnterTele(mapindex, true);

					FixFullRoomMap(mapindex, map.Map);

					FlipNPCs(mapindex);

					if (mapindex == MapIndex.Waterfall)
					{
						map.MapObjects[0].Offset(0, 2);
					}

					if (mapindex == MapIndex.GurguVolcanoB4)
					{
						for (int x = 0; x < 64; x++)
						{
							if (map.Map[1, x] == 48) map.Map[1, x] = 65;
						}
					}

					if (mapindex == MapIndex.GurguVolcanoB3 || mapindex == MapIndex.Waterfall)
					{
						for (int x = 0; x < 64; x++)
						{
							if (map.Map[63, x] == 7) map.Map[63, x] = 46;
						}
					}
				}
				else
				{
					map.Map.FlipVertical();

					FlipNormalTele(mapindex, true);
					FlipEnterTele(mapindex, true);

					FixRooms(mapindex, map.Map);
					FixWalls(mapindex, map.Map);

					FlipNPCs(mapindex);
				}
			}
		}

		private void FlipVolcanoB5(MapDataGroup map)
		{
			map.Map.SwapSections(0x04, 0x32, 0x0B, 0x3A, 0x04, 0x04);

			for (int x = 4; x <= 0x0D; x++) map.Map[4, x] = 48;
			

			var npc = map.MapObjects[0];
			map.MapObjects.MoveNpc(0, npc.Coords.X, npc.Coords.Y - 0x2E, npc.InRoom, npc.Stationary);
		}

		private void FlipVolcanoB2(MapIndex mapindex, Map map)
		{
			map.FlipSectionVertical(0x01, 0x20, 0x1E, 0x21);
			map.FlipSectionVertical(0x00, 0x00, 0x1F, 0x1E);

			for (int x = 0; x <= 0x1F; x++)
			{
				map[0, x] = (byte)(map[0, x] - 6);
				map[0x1E, x] = (byte)(map[0x1E, x] + 6);
				if (map[0x1F, x] == 54 || map[0x1F, x] == 59) map[0x20, x] = 58;
			}

			for (int x = 0; x <= 0x1F; x++)
				for (int y = 0x1C; y >= 0x01; y--)
				{
					if (map[y, x] == 0 && map[y - 1, x] == 3 && map[y + 1, x] != 3 || map[y, x] == 2 && map[y - 1, x] == 5 && map[y + 1, x] != 5)
					{
						map[y + 1, x] = 1;
					}
					else if (map[y, x] == 3 && map[y + 1, x - 1] == 1 && map[y + 1, x] != 3) // don't ask
					{
						map[y + 1, x] = 46;
					}
					else if (map[y, x] == 1 && map[y + 1, x] == 3)
					{
						map[y + 1, x] = 0;
					}
					else
					{
						map[y + 1, x] = map[y, x];
					}

					if (y == 1 && map[y, x] > 8) map[y, x] = 46;
				}

			SwapTele(mapindex, 0x1E, 0x20, 0x1E, 0x21);
		}

		private void FlipNormalTele(MapIndex mapindex, bool yaxis)
		{
			var teletoflip = teleporters.StandardMapTeleporters.Where(t => t.Value.Index == mapindex).ToList();

			foreach (var tele in teletoflip)
			{
				teleporters.StandardMapTeleporters[tele.Key] = new TeleportDestination(tele.Value, yaxis ? tele.Value.FlipYcoordinate() : tele.Value.FlipXcoordinate());
			}
		}

		private void FlipEnterTele(MapIndex mapindex, bool yaxis)
		{
			var teletoflip = teleporters.OverworldTeleporters.Where(t => t.Value.Index == mapindex).ToList();
			foreach (var tele in teletoflip)
			{
				teleporters.OverworldTeleporters[tele.Key] = new TeleportDestination(tele.Value, yaxis ? tele.Value.FlipYcoordinate() : tele.Value.FlipXcoordinate());
			}
		}

		private void FlipNPCs(MapIndex mapindex)
		{
			foreach (var npc in maps[mapindex].MapObjects)
			{
				npc.Flip(false, true);
				if (npc.InRoom)
				{
					npc.Offset(0, -1);
				}
			}
		}

		private void FixRooms(MapIndex mapindex, Map map)
		{
			for(int x = 0; x < 64; x++)
			{
				for(int y = 62; y >= 0; y--)
				{
					if(map[y, x] == 0 || map[y, x] == 1 || map[y, x] == 2)
					{
						var ybegin = y;
						for(;y >= 0; y--)
						{
							if ((map[y, x] == 6 || map[y, x] == 7 || map[y, x] == 8 || map[y - 1, x] == 54 || map[y - 1, x] == 59) && map[y - 1, x] != 0 && map[y - 1, x] != 1 && map[y - 1, x] != 2)
							{
								var yend = y;

								ProcessRoomColumn(mapindex, map, x, ybegin, yend);
								y--;
								break;
							}
						}
					}
				}
			}
		}

		private void ProcessRoomColumn(MapIndex mapindex, Map map, int x, int ybegin, int yend)
		{
			bool door = map[yend - 1, x] == 54;
			bool lockeddoor = map[yend - 1, x] == 59;
			byte specialtile = (door || lockeddoor) ? map[yend, x] : (byte)0;

			for (int y = yend; y <= ybegin; y++)
			{
				var t = map[y, x];

				if (t >= 6 && t <= 8) t = (byte)(t - 6);
				else if (t >= 0 && t <= 2) t = (byte)(t + 6);

				map[y - 1, x] = t;

				SwapTele(mapindex, x, y, x, y - 1);
			}

			if (specialtile > 8) map[yend - 1, x] = 1;

			if (door)
			{
				map[ybegin, x] = 54;
				map[ybegin + 1, x] = 58;
			}
			else if (lockeddoor)
			{
				map[ybegin, x] = 59;
				map[ybegin + 1, x] = 58;
			}
			else
			{
				map[ybegin, x] = 48;
			}

			if (specialtile > 8)
			{
				map[ybegin - 1, x] = specialtile;
			}
		}

		private void FixWalls(MapIndex mapindex, Map map)
		{
			for (int y = 0; y < 63; y++)
				for (int x = 0; x < 64; x++)
				{
					if (map[y, x] == 48 && map[y + 1, x] == 50) map[y, x] = 52;
					if (map[y, x] == 48 && map[y + 1, x] == 51) map[y, x] = 53;

					if (map[y, x] == 52 && map[y + 1, x] == 65) map[y, x] = 48;
					if (map[y, x] == 53 && map[y + 1, x] == 65) map[y, x] = 48;

					if (map[y, x] == 52 && map[y + 1, x] == 56) map[y, x] = 48;
					if (map[y, x] == 53 && map[y + 1, x] == 56) map[y, x] = 48;
				}
		}

		private void FixFullRoomMap(MapIndex mapindex, Map map)
		{
			for (int x = 0; x < 64; x++)
			{
				bool inroom = true;
				bool door = false;
				bool lockeddoor = false;
				byte specialtile = 0;

				for (int y = 1; y < 63; y++)
				{
					var t = map[y, x];
					var t2 = map[y + 1, x];

					if (t == 54)
					{
						door = true;
						specialtile = t2;
						inroom = true;
						map[y, x] = 1;
					}
					else if (t == 59)
					{
						lockeddoor = true;
						specialtile = t2;
						inroom = true;
						map[y, x] = 1;
					}
					else if (t == 48 && t2 >= 6 && t2 <= 8)
					{
						map[y, x] = (byte)(t2 - 6);
						inroom = true;
					}
					else if (t2 >= 0 && t2 <= 2)
					{
						map[y, x] = (byte)(t2 + 6);

						if (door)
						{
							map[y + 1, x] = 54;
							map[y + 2, x] = 58;
							door = false;

							if (specialtile > 8) map[y, x] = specialtile;

							y++;
						}
						else if (lockeddoor)
						{
							map[y + 1, x] = 59;
							map[y + 2, x] = 58;
							lockeddoor = false;

							if (specialtile > 8) map[y, x] = specialtile;

							y++;
						}
						else
						{
							map[y + 1, x] = 48;
						}

						inroom = false;
					}
					else if (inroom)
					{
						map[y, x] = t2;

						SwapTele(mapindex, x, y + 1, x, y);
					}
				}
			}
		}

		private void SwapTele(MapIndex mapindex, int x1, int y1, int x2, int y2)
		{

			var overtoswap = teleporters.OverworldTeleporters.Where(t => t.Value.Index == mapindex && t.Value.Coordinates.X == x1 && t.Value.Coordinates.Y == y1).ToList();
			overtoswap.ForEach(t => t.Value.SetTeleporter(new Coordinate((byte)x2, (byte)y2, t.Value.Coordinates.Context)));

			var teletoswap = teleporters.StandardMapTeleporters.Where(t => t.Value.Index == mapindex && t.Value.Coordinates.X == x1 && t.Value.Coordinates.Y == y1).ToList();
			teletoswap.ForEach(t => t.Value.SetTeleporter(new Coordinate((byte)x2, (byte)y2, t.Value.Coordinates.Context)));

			/*
			foreach (var tele in teletoswap)
			{
				tele.SetEntrance
				tele = new .CoordinateX = x2;


			}
			for (int i = 0; i < 64; i++)
			{
				var t = tele[i];
				if (t.Map == mapindex && t.X == x1 && t.Y == y1)
				{
					t.X = (byte)(x2 & 0x3F | t.X & 0xC0);
					t.Y = (byte)(y2 & 0x3F | t.Y & 0xC0);
					tele[i] = t;
					break;
				}
			}

			for (int i = 0; i < 32; i++)
			{
				var t = enter[i];
				if (t.Map == mapindex && t.X == x1 && t.Y == y1)
				{
					t.X = (byte)(x2 & 0x3F | t.X & 0xC0);
					t.Y = (byte)(y2 & 0x3F | t.Y & 0xC0);
					enter[i] = t;
					break;
				}
			}*/
		}

		public List<MapIndex> HorizontalFlip(MT19337 rng, StandardMaps maps, Teleporters teleporters)
		{
			// Select maps to flip
			if ((bool)flags.ProcgenEarth)
			{
				ValidMaps_Horizontal.Remove(MapIndex.EarthCaveB1);
				ValidMaps_Horizontal.Remove(MapIndex.EarthCaveB2);
				ValidMaps_Horizontal.Remove(MapIndex.EarthCaveB3);
				ValidMaps_Horizontal.Remove(MapIndex.EarthCaveB4);
				ValidMaps_Horizontal.Remove(MapIndex.EarthCaveB5);
			}

			ValidMaps_Horizontal.Shuffle(rng);
			var mapsToFlip = ValidMaps_Horizontal.GetRange(0, rng.Between((int)(ValidMaps_Horizontal.Count * 0.33), (int)(ValidMaps_Horizontal.Count * 0.75)));
			if (flags.EFGWaterfall) mapsToFlip.Remove(MapIndex.Waterfall);

			foreach (MapIndex map in mapsToFlip)
			{
				maps[map].Map.FlipHorizontal();

				// Switch room wall tiles and some other wall tiles for look
				maps[map].Map.Replace(0x00, 0xFF);
				maps[map].Map.Replace(0x02, 0x00);
				maps[map].Map.Replace(0xFF, 0x02);

				maps[map].Map.Replace(0x03, 0xFF);
				maps[map].Map.Replace(0x05, 0x03);
				maps[map].Map.Replace(0xFF, 0x05);

				maps[map].Map.Replace(0x06, 0xFF);
				maps[map].Map.Replace(0x08, 0x06);
				maps[map].Map.Replace(0xFF, 0x08);

				maps[map].Map.Replace(0x32, 0xFF);
				maps[map].Map.Replace(0x33, 0x32);
				maps[map].Map.Replace(0xFF, 0x33);

				maps[map].Map.Replace(0x34, 0xFF);
				maps[map].Map.Replace(0x35, 0x34);
				maps[map].Map.Replace(0xFF, 0x35);

				// Flip NPCs position
				for (int i = 0; i < 16; i++)
				{
					var tempNpc = maps[map].MapObjects[i];
					if (tempNpc.ObjectId != 0x00)
					{
						maps[map].MapObjects[i].Flip(true, false);
					}
				}
			}

			// Update entrance and teleporters coordinate, it has to be done manually for entrance/floor shuffles
			foreach (var maptoflip in mapsToFlip)
			{
				FlipNormalTele(maptoflip, false);
				FlipEnterTele(maptoflip, false);
			}
			/*
			if (mapsToFlip.Contains(MapIndex.EarthCaveB1)) teleporters.EarthCave1.FlipXcoordinate(); 
			if (mapsToFlip.Contains(MapIndex.EarthCaveB2)) teleporters.EarthCave2.FlipXcoordinate();
			if (mapsToFlip.Contains(MapIndex.EarthCaveB3)) teleporters.EarthCaveVampire.FlipXcoordinate(); 
			if (mapsToFlip.Contains(MapIndex.EarthCaveB4)) teleporters.EarthCave4.FlipXcoordinate();
			if (mapsToFlip.Contains(MapIndex.EarthCaveB5)) teleporters.EarthCaveLich.FlipXcoordinate();

			if (mapsToFlip.Contains(MapIndex.GurguVolcanoB1)) teleporters.GurguVolcano1.FlipXcoordinate(); 
			if (mapsToFlip.Contains(MapIndex.GurguVolcanoB2)) teleporters.GurguVolcano2.FlipXcoordinate(); 
			if (mapsToFlip.Contains(MapIndex.GurguVolcanoB3))
			{
				teleporters.GurguVolcano3.FlipXcoordinate(); 
				teleporters.GurguVolcano5.FlipXcoordinate(); 
			}
			if (mapsToFlip.Contains(MapIndex.GurguVolcanoB4))
			{
				teleporters.GurguVolcano4.FlipXcoordinate(); 
				teleporters.GurguVolcano6.FlipXcoordinate(); 
			}
			if (mapsToFlip.Contains(MapIndex.GurguVolcanoB5)) teleporters.GurguVolcanoKary.FlipXcoordinate(); 

			if (mapsToFlip.Contains(MapIndex.IceCaveB1))
			{
				teleporters.IceCave1.FlipXcoordinate();
				teleporters.IceCave6.FlipXcoordinate();
				//teleporters.IceCavePitRoom.FlipXcoordinate();*/

				/*
				var t = teleporters.OverworldTeleporters[(int)TeleportIndex.IceCave5];
				t.FlipXcoordinate();
				overworld.PutStandardTeleport(TeleportIndex.IceCave5, new TeleportDestination(MapLocation.IceCaveBackExit, MapIndex.IceCaveB1, new Coordinate(t.X, t.Y, CoordinateLocale.Standard), TeleportIndex.IceCave5), OverworldTeleportIndex.IceCave1);*/
			/*}
			if (mapsToFlip.Contains(MapIndex.IceCaveB2))
			{
				teleporters.IceCave2.FlipXcoordinate();
				teleporters.IceCavePitRoom.FlipXcoordinate();
				teleporters.IceCave8.FlipXcoordinate();
			}
			if (mapsToFlip.Contains(MapIndex.IceCaveB3))
			{
				teleporters.IceCave3.FlipXcoordinate();
				teleporters.IceCave5.FlipXcoordinate();
				teleporters.IceCave7.FlipXcoordinate();*/

				/*
				var t1 = teleporters.OverworldTeleporters[(int)TeleportIndex.IceCave5];
				t1.FlipXcoordinate();
				overworld.PutStandardTeleport(TeleportIndex.IceCave5, new TeleportDestination(MapLocation.IceCave3, MapIndex.IceCaveB3, new Coordinate(t1.X, t1.Y, CoordinateLocale.StandardInRoom), TeleportIndex.IceCave5), OverworldTeleportIndex.IceCave1);

				var t2 = teleporters.OverworldTeleporters[(int)TeleportIndex.IceCave6];
				t2.FlipXcoordinate();
				overworld.PutStandardTeleport(TeleportIndex.IceCave6, new TeleportDestination(MapLocation.IceCave3, MapIndex.IceCaveB3, new Coordinate(t2.X, t2.Y, CoordinateLocale.Standard), TeleportIndex.IceCave6), OverworldTeleportIndex.IceCave1);*/
			/*}

			if (mapsToFlip.Contains(MapIndex.MarshCaveB1)) teleporters.MarshCave1.FlipXcoordinate();
			if (mapsToFlip.Contains(MapIndex.MarshCaveB2))
			{
				teleporters.MarshCaveTop.FlipXcoordinate();
				teleporters.MarshCave3.FlipXcoordinate();
			}
			if (mapsToFlip.Contains(MapIndex.MarshCaveB3)) teleporters.MarshCaveBottom.FlipXcoordinate();

			if (mapsToFlip.Contains(MapIndex.MirageTower1F)) teleporters.MirageTower1.FlipXcoordinate();
			if (mapsToFlip.Contains(MapIndex.MirageTower2F)) teleporters.MirageTower2.FlipXcoordinate();

			if (mapsToFlip.Contains(MapIndex.SeaShrineB1)) teleporters.SeaShrineMermaids.FlipXcoordinate();
			if (mapsToFlip.Contains(MapIndex.SeaShrineB2))
			{
				teleporters.SeaShrine2.FlipXcoordinate();
				teleporters.SeaShrine6.FlipXcoordinate();
			}
			if (mapsToFlip.Contains(MapIndex.SeaShrineB3))
			{
				teleporters.SeaShrine1.FlipXcoordinate();
				teleporters.SeaShrine5.FlipXcoordinate();
				teleporters.SeaShrine7.FlipXcoordinate();
			}
			if (mapsToFlip.Contains(MapIndex.SeaShrineB4))
			{
				teleporters.SeaShrine4.FlipXcoordinate();
				teleporters.SeaShrine8.FlipXcoordinate();
			}
			if (mapsToFlip.Contains(MapIndex.SeaShrineB5)) teleporters.SeaShrineKraken.FlipXcoordinate();

			if (mapsToFlip.Contains(MapIndex.SkyPalace1F)) teleporters.SkyPalace1.FlipXcoordinate();
			if (mapsToFlip.Contains(MapIndex.SkyPalace2F)) teleporters.SkyPalace2.FlipXcoordinate();
			if (mapsToFlip.Contains(MapIndex.SkyPalace3F)) teleporters.SkyPalace3.FlipXcoordinate();

			if (mapsToFlip.Contains(MapIndex.TempleOfFiends)) teleporters.TempleOfFiends.FlipXcoordinate(); 

			if (mapsToFlip.Contains(MapIndex.Waterfall)) teleporters.Waterfall.FlipXcoordinate();
			*/
			return mapsToFlip;
		}
	}
}
