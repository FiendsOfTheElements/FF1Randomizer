using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class Translator
	{
		FF1Rom rom;
		string[] itemnames;

		public Translator(FF1Rom _rom)
		{
			rom = _rom;
			itemnames = rom.ReadText(FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextPointerCount);
		}

		public string TranslateItem(Item item)
		{
			switch (item)
			{
				case Item.Ship: return FF1Text.BytesToText(rom.Get(0x2B5D0, 4));
				case Item.Bridge: return FF1Text.BytesToText(rom.Get(0x2B5D0 + 16, 6));
				case Item.Canal: return FF1Text.BytesToText(rom.Get(0x2B5D0 + 24, 5));
				case Item.Canoe: return FF1Text.BytesToText(rom.Get(0x2B5D0 + 36, 5));
				default: return itemnames[(int)item].Replace(" ", "");
			}
		}

		public string TranslateFloor(MapLocation mapLocation)
		{
			Dictionary<MapLocation, string> StandardOverworldLocations =
			   new Dictionary<MapLocation, string>
			   {
				{MapLocation.Coneria,"Coneria"},
				{MapLocation.Pravoka, "Pravoka"},
				{MapLocation.Elfland, "Elfland"},
				{MapLocation.Melmond, "Melmond"},
				{MapLocation.CrescentLake, "CrescentLake"},
				{MapLocation.Gaia,"Gaia"},
				{MapLocation.Onrac,"Onrac"},
				{MapLocation.Lefein,"Lefein"},
				{MapLocation.ConeriaCastle1,"ConeriaCastle"},
				{MapLocation.ConeriaCastle2,"ConeriaCastle"},
				{MapLocation.ConeriaCastleRoom1,"ConeriaCastle"},
				{MapLocation.ConeriaCastleRoom2,"ConeriaCastle"},
				{MapLocation.ElflandCastle,"ElflandCastle"},
				{MapLocation.ElflandCastleRoom1,"ElflandCastle"},
				{MapLocation.NorthwestCastle,"NorthwestCastle"},
				{MapLocation.NorthwestCastleRoom2,"NorthwestCastle"},
				{MapLocation.CastleOrdeals1,"CastleOrdeals 1"},
				{MapLocation.CastleOrdealsMaze,"CastleOrdeals 2"},
				{MapLocation.CastleOrdealsTop,"CastleOrdeals 3"},
				{MapLocation.TempleOfFiends1,"TempleOfFiends 1"},
				{MapLocation.TempleOfFiends1Room1,"TempleOfFiends 1"},
				{MapLocation.TempleOfFiends1Room2,"TempleOfFiends 1"},
				{MapLocation.TempleOfFiends1Room3,"TempleOfFiends 1"},
				{MapLocation.TempleOfFiends1Room4,"TempleOfFiends 1"},
				{MapLocation.TempleOfFiends2,"TempleOfFiends 2"},
				{MapLocation.TempleOfFiends3,"TempleOfFiends 3"},
				{MapLocation.TempleOfFiendsChaos,"TempleOfFiends Chaos"},
				{MapLocation.TempleOfFiendsAir,"TempleOfFiends Air"},
				{MapLocation.TempleOfFiendsEarth,"TempleOfFiends Earth"},
				{MapLocation.TempleOfFiendsFire,"TempleOfFiends Fire"},
				{MapLocation.TempleOfFiendsWater,"TempleOfFiends Water"},
				{MapLocation.TempleOfFiendsPhantom,"TempleOfFiends 4"},
				{MapLocation.EarthCave1,"EarthCave 1"},
				{MapLocation.EarthCave2,"EarthCave 2"},
				{MapLocation.EarthCaveVampire,"EarthCave 3"},
				{MapLocation.EarthCave4,"EarthCave 4"},
				{MapLocation.EarthCaveLich,"EarthCave 4"},
				{MapLocation.GurguVolcano1,"Volcano 1"},
				{MapLocation.GurguVolcano2,"Volcano 2"},
				{MapLocation.GurguVolcano3,"Volcano 3"},
				{MapLocation.GurguVolcano4,"Volcano 4"},
				{MapLocation.GurguVolcano5,"Volcano 5"},
				{MapLocation.GurguVolcano6,"Volcano 6"},
				{MapLocation.GurguVolcanoKary,"Volcano Kary"},
				{MapLocation.IceCave1,"IceCave 1"},
				{MapLocation.IceCave2,"IceCave 2"},
				{MapLocation.IceCave3,"IceCave 3"},
				{MapLocation.IceCave5,"IceCave 5"},
				{MapLocation.IceCaveBackExit,"IceCave 6"},
				{MapLocation.IceCaveFloater,"IceCave 4"},
				{MapLocation.IceCavePitRoom,"IceCave 4"},
				{MapLocation.SeaShrine1, "SeaShrine 1"},
				{MapLocation.SeaShrine2, "SeaShrine Right 2"},
				{MapLocation.SeaShrine2Room2, "SeaShrine Right 2"},
				{MapLocation.SeaShrine4, "SeaShrine Left 2"},
				{MapLocation.SeaShrine5, "SeaShrine Left 3"},
				{MapLocation.SeaShrine6, "SeaShrine Left 4"},
				{MapLocation.SeaShrine7, "SeaShrine Left 5"},
				{MapLocation.SeaShrine8, "SeaShrine Left 6"},
				{MapLocation.SeaShrineKraken, "SeaShrine Kraken"},
				{MapLocation.SeaShrineMermaids, "SeaShrine Mermaids"},
				{MapLocation.Cardia1,"Cardia 1"},
				{MapLocation.Cardia2,"Cardia 2"},
				{MapLocation.BahamutCave1,"Cardia Bahamut"},
				{MapLocation.BahamutCave2,"Cardia Bahamut"},
				{MapLocation.Cardia4,"Cardia 4"},
				{MapLocation.Cardia5,"Cardia 5"},
				{MapLocation.Cardia6,"Cardia 6"},
				{MapLocation.Waterfall,"Waterfall"},
				{MapLocation.DwarfCave,"DwarfCave"},
				{MapLocation.DwarfCaveRoom3,"DwarfCave"},
				{MapLocation.MatoyasCave,"MatoyasCave"},
				{MapLocation.SardasCave,"SardasCave"},
				{MapLocation.MarshCave1,"MarshCave 1"},
				{MapLocation.MarshCave3,"MarshCave Bottom 2"},
				{MapLocation.MarshCaveBottom,"MarshCave Bottom 3"},
				{MapLocation.MarshCaveBottomRoom13,"MarshCave Bottom 3"},
				{MapLocation.MarshCaveBottomRoom14,"MarshCave Bottom 3"},
				{MapLocation.MarshCaveBottomRoom16,"MarshCave Bottom 3"},
				{MapLocation.MarshCaveTop,"MarshCave Top 2"},
				{MapLocation.MirageTower1,"MirageTower 1"},
				{MapLocation.MirageTower2,"MirageTower 2"},
				{MapLocation.MirageTower3,"MirageTower 3"},
				{MapLocation.SkyPalace1,"SkyPalace 1"},
				{MapLocation.SkyPalace2,"SkyPalace 2"},
				{MapLocation.SkyPalace3,"SkyPalace 3"},
				{MapLocation.SkyPalaceMaze,"SkyPalace 4"},
				{MapLocation.SkyPalaceTiamat,"SkyPalace Tiamat"},
				{MapLocation.TitansTunnelEast,"TitansTunnel"},
				{MapLocation.TitansTunnelWest,"TitansTunnel"},
				{MapLocation.TitansTunnelRoom,"TitansTunnel"},
			   };

			return StandardOverworldLocations.TryGetValue(mapLocation, out var text) ? text : "...";
		}

		public string TranslateNpc(string name)
		{
			return FF1Rom.NiceNpcName[name];
		}


		public string TranslateOverWorldLocation(MapLocation mapLocation)
		{
			Dictionary<MapLocation, string> StandardOverworldLocations =
			new Dictionary<MapLocation, string>
			{
				{MapLocation.Coneria,"Coneria"},
				{MapLocation.Pravoka, "Pravoka"},
				{MapLocation.Elfland, "Elfland"},
				{MapLocation.Melmond, "Melmond"},
				{MapLocation.CrescentLake, "CrescentLake"},
				{MapLocation.Gaia,"Gaia"},
				{MapLocation.Onrac,"Onrac"},
				{MapLocation.Lefein,"Lefein"},
				{MapLocation.ConeriaCastle1,"Coneria"},
				{MapLocation.ConeriaCastle2,"Coneria"},
				{MapLocation.ConeriaCastleRoom1,"Coneria"},
				{MapLocation.ConeriaCastleRoom2,"Coneria"},
				{MapLocation.ElflandCastle,"Elfland"},
				{MapLocation.ElflandCastleRoom1,"Elfland"},
				{MapLocation.NorthwestCastle,"NorthwestCastle"},
				{MapLocation.NorthwestCastleRoom2,"NorthwestCastle"},
				{MapLocation.CastleOrdeals1,"CastleOrdeals"},
				{MapLocation.CastleOrdealsMaze,"CastleOrdeals"},
				{MapLocation.CastleOrdealsTop,"CastleOrdeals"},
				{MapLocation.TempleOfFiends1,"TempleOfFiends"},
				{MapLocation.TempleOfFiends1Room1,"TempleOfFiends"},
				{MapLocation.TempleOfFiends1Room2,"TempleOfFiends"},
				{MapLocation.TempleOfFiends1Room3,"TempleOfFiends"},
				{MapLocation.TempleOfFiends1Room4,"TempleOfFiends"},
				{MapLocation.TempleOfFiends2,"TempleOfFiends"},
				{MapLocation.TempleOfFiends3,"TempleOfFiends"},
				{MapLocation.TempleOfFiendsChaos,"TempleOfFiends"},
				{MapLocation.TempleOfFiendsAir,"TempleOfFiends"},
				{MapLocation.TempleOfFiendsEarth,"TempleOfFiends"},
				{MapLocation.TempleOfFiendsFire,"TempleOfFiends"},
				{MapLocation.TempleOfFiendsWater,"TempleOfFiends"},
				{MapLocation.TempleOfFiendsPhantom,"TempleOfFiends"},
				{MapLocation.EarthCave1,"EarthCave"},
				{MapLocation.EarthCave2,"EarthCave"},
				{MapLocation.EarthCaveVampire,"EarthCave"},
				{MapLocation.EarthCave4,"EarthCave"},
				{MapLocation.EarthCaveLich,"EarthCave"},
				{MapLocation.GurguVolcano1,"Volcano"},
				{MapLocation.GurguVolcano2,"Volcano"},
				{MapLocation.GurguVolcano3,"Volcano"},
				{MapLocation.GurguVolcano4,"Volcano"},
				{MapLocation.GurguVolcano5,"Volcano"},
				{MapLocation.GurguVolcano6,"Volcano"},
				{MapLocation.GurguVolcanoKary,"Volcano"},
				{MapLocation.IceCave1,"IceCave"},
				{MapLocation.IceCave2,"IceCave"},
				{MapLocation.IceCave3,"IceCave"},
				{MapLocation.IceCave5,"IceCave"},
				{MapLocation.IceCaveBackExit,"IceCave"},
				{MapLocation.IceCaveFloater,"IceCave"},
				{MapLocation.IceCavePitRoom,"IceCave"},
				{MapLocation.SeaShrine1, "SeaShrine"},
				{MapLocation.SeaShrine2, "SeaShrine"},
				{MapLocation.SeaShrine2Room2, "SeaShrine"},
				{MapLocation.SeaShrine4, "SeaShrine"},
				{MapLocation.SeaShrine5, "SeaShrine"},
				{MapLocation.SeaShrine6, "SeaShrine"},
				{MapLocation.SeaShrine7, "SeaShrine"},
				{MapLocation.SeaShrine8, "SeaShrine"},
				{MapLocation.SeaShrineKraken, "SeaShrine"},
				{MapLocation.SeaShrineMermaids, "SeaShrine"},
				{MapLocation.Cardia1,"Cardia"},
				{MapLocation.Cardia2,"Cardia"},
				{MapLocation.BahamutCave1,"Cardia"},
				{MapLocation.BahamutCave2,"Cardia"},
				{MapLocation.Cardia4,"Cardia"},
				{MapLocation.Cardia5,"Cardia"},
				{MapLocation.Cardia6,"Cardia"},
				{MapLocation.Waterfall,"Waterfall"},
				{MapLocation.DwarfCave,"DwarfCave"},
				{MapLocation.DwarfCaveRoom3,"DwarfCave"},
				{MapLocation.MatoyasCave,"MatoyasCave"},
				{MapLocation.SardasCave,"SardasCave"},
				{MapLocation.MarshCave1,"MarshCave"},
				{MapLocation.MarshCave3,"MarshCave"},
				{MapLocation.MarshCaveBottom,"MarshCave"},
				{MapLocation.MarshCaveBottomRoom13,"MarshCave"},
				{MapLocation.MarshCaveBottomRoom14,"MarshCave"},
				{MapLocation.MarshCaveBottomRoom16,"MarshCave"},
				{MapLocation.MarshCaveTop,"MarshCave"},
				{MapLocation.MirageTower1,"MirageTower"},
				{MapLocation.MirageTower2,"MirageTower"},
				{MapLocation.MirageTower3,"MirageTower"},
				{MapLocation.SkyPalace1,"SkyPalace"},
				{MapLocation.SkyPalace2,"SkyPalace"},
				{MapLocation.SkyPalace3,"SkyPalace"},
				{MapLocation.SkyPalaceMaze,"SkyPalace"},
				{MapLocation.SkyPalaceTiamat,"SkyPalace"},
				{MapLocation.TitansTunnelEast,"TitansTunnel"},
				{MapLocation.TitansTunnelWest,"TitansTunnel"},
				{MapLocation.TitansTunnelRoom,"TitansTunnel"},
			};

			return StandardOverworldLocations.TryGetValue(mapLocation, out var text) ? text : "...";
		}
	}
}
