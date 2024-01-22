using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class FlippedMaps
	{
		FF1Rom rom;
		List<Map> maps;
		Flags flags;
		MT19337 rng;
		NormTeleData tele;
		EnterTeleData enter;

		List<MapId> ValidMaps_Horizontal = new List<MapId>
		{
			MapId.TempleOfFiends,
			MapId.EarthCaveB1, MapId.EarthCaveB2, MapId.EarthCaveB3, MapId.EarthCaveB4, MapId.EarthCaveB5,
			MapId.MarshCaveB1, MapId.MarshCaveB2, MapId.MarshCaveB3,
			MapId.IceCaveB1, MapId.IceCaveB2, MapId.IceCaveB3,
			MapId.MirageTower1F, MapId.MirageTower2F,
			MapId.SkyPalace1F, MapId.SkyPalace2F, MapId.SkyPalace3F,
			MapId.SeaShrineB1, MapId.SeaShrineB2, MapId.SeaShrineB3, MapId.SeaShrineB4, MapId.SeaShrineB5,
			MapId.Waterfall,
			MapId.GurguVolcanoB1, MapId.GurguVolcanoB2, MapId.GurguVolcanoB3, MapId.GurguVolcanoB4, MapId.GurguVolcanoB5

		};

		List<MapId> ValidMaps_Vertical = new List<MapId>
		{
			MapId.TempleOfFiends,
			MapId.EarthCaveB1, MapId.EarthCaveB2, MapId.EarthCaveB4, MapId.EarthCaveB3, MapId.EarthCaveB5,
			MapId.MarshCaveB1, MapId.MarshCaveB2, MapId.MarshCaveB3,
			MapId.IceCaveB1, MapId.IceCaveB2, MapId.IceCaveB3,
			MapId.MirageTower1F, MapId.MirageTower2F,
			MapId.SkyPalace1F, MapId.SkyPalace2F, MapId.SkyPalace3F,
			MapId.SeaShrineB1, MapId.SeaShrineB2, MapId.SeaShrineB3, MapId.SeaShrineB4, MapId.SeaShrineB5,
			MapId.Waterfall,
			MapId.GurguVolcanoB1, MapId.GurguVolcanoB2, MapId.GurguVolcanoB3, MapId.GurguVolcanoB4, MapId.GurguVolcanoB5
		};

		List<MapId> mapsToFlipVertically;

		public FlippedMaps(FF1Rom _rom, List<Map> _maps, Flags _flags, MT19337 _rng)
		{
			rom = _rom;
			maps = _maps;
			flags = _flags;
			rng = _rng;
			tele = new NormTeleData(rom);
			enter = new EnterTeleData(rom);
		}

		public List<MapId> VerticalFlipStep1()
		{
			if (!flags.VerticallyFlipDungeons ?? false || flags.GameMode != GameModes.Standard)
			{
				mapsToFlipVertically = new List<MapId>();
				return mapsToFlipVertically;
			}

			ValidMaps_Vertical.Shuffle(rng);
			mapsToFlipVertically = ValidMaps_Vertical.GetRange(0, rng.Between((int)(ValidMaps_Vertical.Count * 0.33), (int)(ValidMaps_Vertical.Count * 0.75)));
			if (flags.EFGWaterfall) mapsToFlipVertically.Remove(MapId.Waterfall);

			/*
			foreach (var mapId in mapsToFlipVertically)
			{
				var map = maps[(int)mapId];
				var fileName = @"d:\FFR\vfmaps\" + mapId.ToString() + ".ffm";
				using var stream = new FileStream(fileName, FileMode.Create);
				map.Save(stream);
				stream.Close();
			}
			*/

			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var resourcePath = assembly.GetManifestResourceNames().First(str => str.EndsWith("vfmaps.zip"));

			using var stream = assembly.GetManifestResourceStream(resourcePath);

			var archive = new ZipArchive(stream);

			foreach (var mapId in mapsToFlipVertically)
			{
				var map = maps[(int)mapId];

				var entry = archive.GetEntry(mapId.ToString() + ".ffm");

				using var stream2 = entry.Open();
				map.Load(stream2);
			}

			return mapsToFlipVertically;
		}

		public void VerticalFlipStep2()
		{
			tele.LoadData();
			enter.LoadData();

			foreach (MapId mapId in mapsToFlipVertically)
			{
				var map = maps[(int)mapId];

				if (mapId == MapId.GurguVolcanoB5)
				{
					FlipVolcanoB5(map);
				}
				else if (mapId == MapId.GurguVolcanoB2)
				{
					FlipVolcanoB2(mapId, map);
				}
				else if (mapId == MapId.Waterfall || mapId == MapId.GurguVolcanoB3 || mapId == MapId.GurguVolcanoB4)
				{
					map.FlipVertical();

					FlipNormalTele(mapId);
					FlipEnterTele(mapId);

					FixFullRoomMap(mapId, map);

					FlipNPCs(mapId);

					if (mapId == MapId.Waterfall)
					{
						var npc = rom.GetNpc(mapId, 0);
						npc.Coord.y = npc.Coord.y + 2;
						rom.SetNpc(mapId, npc);
					}

					if (mapId == MapId.GurguVolcanoB4)
					{
						for (int x = 0; x < 64; x++)
						{
							if (map[1, x] == 48) map[1, x] = 65;
						}
					}

					if (mapId == MapId.GurguVolcanoB3 || mapId == MapId.Waterfall)
					{
						for (int x = 0; x < 64; x++)
						{
							if (map[63, x] == 7) map[63, x] = 46;
						}
					}
				}
				else
				{
					map.FlipVertical();

					FlipNormalTele(mapId);
					FlipEnterTele(mapId);

					FixRooms(mapId, map);
					FixWalls(mapId, map);

					FlipNPCs(mapId);
				}
			}

			tele.StoreData();
			enter.StoreData();
		}

		private void FlipVolcanoB5(Map map)
		{
			map.SwapSections(0x04, 0x32, 0x0B, 0x3A, 0x04, 0x04);

			for (int x = 4; x <= 0x0D; x++) map[4, x] = 48;

			var npc = rom.GetNpc(MapId.GurguVolcanoB5, 0);
			npc.Coord.y = npc.Coord.y - 0x2E;
			rom.SetNpc(MapId.GurguVolcanoB5, npc);
		}

		private void FlipVolcanoB2(MapId mapId, Map map)
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

			SwapTele(mapId, 0x1E, 0x20, 0x1E, 0x21);
		}

		private void FlipNormalTele(MapId mapId)
		{
			for (int i = 0; i < 64; i++)
			{
				var t = tele[i];
				if (t.Map == mapId)
				{
					t.FlipYcoordinate();
					tele[i] = t;
				}
			}
		}

		private void FlipEnterTele(MapId mapId)
		{
			for (int i = 0; i < 32; i++)
			{
				var t = enter[i];
				if (t.Map == mapId)
				{
					t.FlipYcoordinate();
					enter[i] = t;
				}
			}
		}

		private void FlipNPCs(MapId mapId)
		{
			for (int i = 0; i < 16; i++)
			{
				var tempNpc = rom.GetNpc(mapId, i);
				if (tempNpc.Coord != (0, 0))
				{
					tempNpc.Coord.y = 64 - tempNpc.Coord.y - 1 + (tempNpc.InRoom ? -1 : 0);
					rom.SetNpc(mapId, tempNpc);
				}
			}
		}

		private void FixRooms(MapId mapId, Map map)
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

								ProcessRoomColumn(mapId, map, x, ybegin, yend);
								y--;
								break;
							}
						}
					}
				}
			}
		}

		private void ProcessRoomColumn(MapId mapId, Map map, int x, int ybegin, int yend)
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

				SwapTele(mapId, x, y, x, y - 1);
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

		private void FixWalls(MapId mapId, Map map)
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

		private void FixFullRoomMap(MapId mapId, Map map)
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

						SwapTele(mapId, x, y + 1, x, y);
					}
				}
			}
		}

		private void SwapTele(MapId mapId, int x1, int y1, int x2, int y2)
		{
			for (int i = 0; i < 64; i++)
			{
				var t = tele[i];
				if (t.Map == mapId && t.X == x1 && t.Y == y1)
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
				if (t.Map == mapId && t.X == x1 && t.Y == y1)
				{
					t.X = (byte)(x2 & 0x3F | t.X & 0xC0);
					t.Y = (byte)(y2 & 0x3F | t.Y & 0xC0);
					enter[i] = t;
					break;
				}
			}
		}

		public List<MapId> HorizontalFlip(MT19337 rng, List<Map> maps, TeleportShuffle teleporters, OverworldMap overworld)
		{
			// Select maps to flip
			ValidMaps_Horizontal.Shuffle(rng);
			var mapsToFlip = ValidMaps_Horizontal.GetRange(0, rng.Between((int)(ValidMaps_Horizontal.Count * 0.33), (int)(ValidMaps_Horizontal.Count * 0.75)));
			if (flags.EFGWaterfall) mapsToFlip.Remove(MapId.Waterfall);

			foreach (MapId map in mapsToFlip)
			{
				maps[(int)map].FlipHorizontal();

				// Switch room wall tiles and some other wall tiles for look
				maps[(int)map].Replace(0x00, 0xFF);
				maps[(int)map].Replace(0x02, 0x00);
				maps[(int)map].Replace(0xFF, 0x02);

				maps[(int)map].Replace(0x03, 0xFF);
				maps[(int)map].Replace(0x05, 0x03);
				maps[(int)map].Replace(0xFF, 0x05);

				maps[(int)map].Replace(0x06, 0xFF);
				maps[(int)map].Replace(0x08, 0x06);
				maps[(int)map].Replace(0xFF, 0x08);

				maps[(int)map].Replace(0x32, 0xFF);
				maps[(int)map].Replace(0x33, 0x32);
				maps[(int)map].Replace(0xFF, 0x33);

				maps[(int)map].Replace(0x34, 0xFF);
				maps[(int)map].Replace(0x35, 0x34);
				maps[(int)map].Replace(0xFF, 0x35);

				// Flip NPCs position
				for (int i = 0; i < 16; i++)
				{
					var tempNpc = rom.GetNpc(map, i);
					if (tempNpc.Coord != (0, 0))
					{
						tempNpc.Coord.x = 64 - tempNpc.Coord.x - 1;
						rom.SetNpc(map, tempNpc);
					}
				}
			}

			// Update entrance and teleporters coordinate, it has to be done manually for entrance/floor shuffles
			if (mapsToFlip.Contains(MapId.EarthCaveB1)) teleporters.EarthCave1.FlipXcoordinate(); overworld.PutOverworldTeleport(OverworldTeleportIndex.EarthCave1, teleporters.EarthCave1);
			if (mapsToFlip.Contains(MapId.EarthCaveB2)) teleporters.EarthCave2.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.EarthCave2, teleporters.EarthCave2, OverworldTeleportIndex.EarthCave1);
			if (mapsToFlip.Contains(MapId.EarthCaveB3)) teleporters.EarthCaveVampire.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.EarthCaveVampire, teleporters.EarthCaveVampire, OverworldTeleportIndex.EarthCave1);
			if (mapsToFlip.Contains(MapId.EarthCaveB4)) teleporters.EarthCave4.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.EarthCave4, teleporters.EarthCave4, OverworldTeleportIndex.EarthCave1);
			if (mapsToFlip.Contains(MapId.EarthCaveB5)) teleporters.EarthCaveLich.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.EarthCaveLich, teleporters.EarthCaveLich, OverworldTeleportIndex.EarthCave1);

			if (mapsToFlip.Contains(MapId.GurguVolcanoB1)) teleporters.GurguVolcano1.FlipXcoordinate(); overworld.PutOverworldTeleport(OverworldTeleportIndex.GurguVolcano1, teleporters.GurguVolcano1);
			if (mapsToFlip.Contains(MapId.GurguVolcanoB2)) teleporters.GurguVolcano2.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.GurguVolcano2, teleporters.GurguVolcano2, OverworldTeleportIndex.GurguVolcano1);
			if (mapsToFlip.Contains(MapId.GurguVolcanoB3))
			{
				teleporters.GurguVolcano3.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.GurguVolcano3, teleporters.GurguVolcano3, OverworldTeleportIndex.GurguVolcano1);
				teleporters.GurguVolcano5.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.GurguVolcano5, teleporters.GurguVolcano5, OverworldTeleportIndex.GurguVolcano1);
			}
			if (mapsToFlip.Contains(MapId.GurguVolcanoB4))
			{
				teleporters.GurguVolcano4.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.GurguVolcano4, teleporters.GurguVolcano4, OverworldTeleportIndex.GurguVolcano1);
				teleporters.GurguVolcano6.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.GurguVolcano6, teleporters.GurguVolcano6, OverworldTeleportIndex.GurguVolcano1);
			}
			if (mapsToFlip.Contains(MapId.GurguVolcanoB5)) teleporters.GurguVolcanoKary.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.GurguVolcanoKary, teleporters.GurguVolcanoKary, OverworldTeleportIndex.GurguVolcano1);

			if (mapsToFlip.Contains(MapId.IceCaveB1))
			{
				teleporters.IceCave1.FlipXcoordinate(); overworld.PutOverworldTeleport(OverworldTeleportIndex.IceCave1, teleporters.IceCave1);

				var t = teleporters.NormalTele[(int)TeleportIndex.IceCave5];
				t.FlipXcoordinate();
				overworld.PutStandardTeleport(TeleportIndex.IceCave5, new TeleportDestination(MapLocation.IceCaveBackExit, MapIndex.IceCaveB1, new Coordinate(t.X, t.Y, CoordinateLocale.Standard), TeleportIndex.IceCave5), OverworldTeleportIndex.IceCave1);
			}
			if (mapsToFlip.Contains(MapId.IceCaveB2))
			{
				teleporters.IceCave2.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.IceCave2, teleporters.IceCave2, OverworldTeleportIndex.IceCave1);
				teleporters.IceCavePitRoom.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.IceCavePitRoom, teleporters.IceCavePitRoom, OverworldTeleportIndex.IceCave1);

				var t = teleporters.NormalTele[(int)TeleportIndex.IceCave7];
				t.FlipXcoordinate();
				overworld.PutStandardTeleport(TeleportIndex.IceCave7, new TeleportDestination(MapLocation.IceCaveFloater, MapIndex.IceCaveB2, new Coordinate(t.X, t.Y, CoordinateLocale.StandardInRoom), TeleportIndex.IceCave7), OverworldTeleportIndex.IceCave1);
			}
			if (mapsToFlip.Contains(MapId.IceCaveB3))
			{
				teleporters.IceCave3.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.IceCave3, teleporters.IceCave3, OverworldTeleportIndex.IceCave1);

				var t1 = teleporters.NormalTele[(int)TeleportIndex.IceCave4];
				t1.FlipXcoordinate();
				overworld.PutStandardTeleport(TeleportIndex.IceCave4, new TeleportDestination(MapLocation.IceCave3, MapIndex.IceCaveB3, new Coordinate(t1.X, t1.Y, CoordinateLocale.StandardInRoom), TeleportIndex.IceCave4), OverworldTeleportIndex.IceCave1);

				var t2 = teleporters.NormalTele[(int)TeleportIndex.IceCave6];
				t2.FlipXcoordinate();
				overworld.PutStandardTeleport(TeleportIndex.IceCave6, new TeleportDestination(MapLocation.IceCave3, MapIndex.IceCaveB3, new Coordinate(t2.X, t2.Y, CoordinateLocale.Standard), TeleportIndex.IceCave6), OverworldTeleportIndex.IceCave1);
			}

			if (mapsToFlip.Contains(MapId.MarshCaveB1)) teleporters.MarshCave1.FlipXcoordinate(); overworld.PutOverworldTeleport(OverworldTeleportIndex.MarshCave1, teleporters.MarshCave1);
			if (mapsToFlip.Contains(MapId.MarshCaveB2))
			{
				teleporters.MarshCaveTop.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.MarshCaveTop, teleporters.MarshCaveTop, OverworldTeleportIndex.MarshCave1);
				teleporters.MarshCave3.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.MarshCave3, teleporters.MarshCave3, OverworldTeleportIndex.MarshCave1);
			}
			if (mapsToFlip.Contains(MapId.MarshCaveB3)) teleporters.MarshCaveBottom.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.MarshCaveBottom, teleporters.MarshCaveBottom, OverworldTeleportIndex.MarshCave1);

			if (mapsToFlip.Contains(MapId.MirageTower1F)) teleporters.MirageTower1.FlipXcoordinate(); overworld.PutOverworldTeleport(OverworldTeleportIndex.MirageTower1, teleporters.MirageTower1);
			if (mapsToFlip.Contains(MapId.MirageTower2F)) teleporters.MirageTower2.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.MirageTower2, teleporters.MirageTower2, OverworldTeleportIndex.MirageTower1);

			if (mapsToFlip.Contains(MapId.SeaShrineB1)) teleporters.SeaShrineMermaids.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrineMermaids, teleporters.SeaShrineMermaids, OverworldTeleportIndex.Onrac);
			if (mapsToFlip.Contains(MapId.SeaShrineB2))
			{
				teleporters.SeaShrine2.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrine2, teleporters.SeaShrine2, OverworldTeleportIndex.Onrac);
				teleporters.SeaShrine6.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrine6, teleporters.SeaShrine6, OverworldTeleportIndex.Onrac);
			}
			if (mapsToFlip.Contains(MapId.SeaShrineB3))
			{
				teleporters.SeaShrine1.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrine1, teleporters.SeaShrine1, OverworldTeleportIndex.Onrac);
				teleporters.SeaShrine5.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrine5, teleporters.SeaShrine5, OverworldTeleportIndex.Onrac);
				teleporters.SeaShrine7.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrine7, teleporters.SeaShrine7, OverworldTeleportIndex.Onrac);
			}
			if (mapsToFlip.Contains(MapId.SeaShrineB4))
			{
				teleporters.SeaShrine4.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrine4, teleporters.SeaShrine4, OverworldTeleportIndex.Onrac);
				teleporters.SeaShrine8.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrine8, teleporters.SeaShrine8, OverworldTeleportIndex.Onrac);
			}
			if (mapsToFlip.Contains(MapId.SeaShrineB5)) teleporters.SeaShrineKraken.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrineKraken, teleporters.SeaShrineKraken, OverworldTeleportIndex.Onrac);

			if (mapsToFlip.Contains(MapId.SkyPalace1F)) teleporters.SkyPalace1.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SkyPalace1, teleporters.SkyPalace1, OverworldTeleportIndex.MirageTower1);
			if (mapsToFlip.Contains(MapId.SkyPalace2F)) teleporters.SkyPalace2.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SkyPalace2, teleporters.SkyPalace2, OverworldTeleportIndex.MirageTower1);
			if (mapsToFlip.Contains(MapId.SkyPalace3F)) teleporters.SkyPalace3.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SkyPalace3, teleporters.SkyPalace3, OverworldTeleportIndex.MirageTower1);

			if (mapsToFlip.Contains(MapId.TempleOfFiends)) teleporters.TempleOfFiends.FlipXcoordinate(); overworld.PutOverworldTeleport(OverworldTeleportIndex.TempleOfFiends1, teleporters.TempleOfFiends);

			if (mapsToFlip.Contains(MapId.Waterfall)) teleporters.Waterfall.FlipXcoordinate(); overworld.PutOverworldTeleport(OverworldTeleportIndex.Waterfall, teleporters.Waterfall);

			return mapsToFlip;
		}
	}
}
