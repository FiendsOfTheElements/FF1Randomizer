using System.Diagnostics.CodeAnalysis;

namespace FF1Lib
{
	public class QuickMiniMap
	{
		FF1Rom rom;
		List<List<byte>> overworldMap;

		byte[] lut_MinimapTileset;
		List<Blob> chr_MinimapDecor;
		List<Blob> chr_MinimapTitle;
		Blob chr_MinimapSprite;
		byte[] lut_MinimapDecorCHRDest_lo;
		byte[] lut_MinimapDecorCHRDest_hi;
		byte[] lut_MinimapTitleCHRDest_lo;
		byte[] lut_MinimapTitleCHRDest_hi;
		byte[] lut_MinimapNT;

		byte[][] rawCHRData;
		List<(int x, int y)> entrances;

		List<byte[]> CHRData;
		byte[] NTData;
		byte[] ATData;
		byte[] EntranceData;

		byte[] lut_MinimapBGPal;

		public QuickMiniMap(FF1Rom _rom, List<List<byte>> decompressedMap)
		{
			rom = _rom;
			overworldMap = decompressedMap;
		}

		public void EnableQuickMinimap(bool enable, MusicTracks music)
		{
			if (!enable)
			{
				return;
			}

			LoadData();

			BuildCHR();
			BuildNT();
			FitDecor();

			BuildEntranceData();

			StoreData();

			music.SkipMinimap = true;
			StoreRoutine((byte)music.Tracks[SongTracks.Minimap]);
		}

		private void LoadData()
		{
			lut_MinimapTileset = rom.GetFromBank(0x09, 0xBF80, 128);

			chr_MinimapDecor = rom.GetFromBank(0x09, 0xB400, 768).Chunk(16);
			chr_MinimapTitle = rom.GetFromBank(0x09, 0xB700, 640).Chunk(16);
			chr_MinimapSprite = rom.GetFromBank(0x09, 0xBF00, 32);

			lut_MinimapDecorCHRDest_lo = rom.GetFromBank(0x09, 0xBCE9, 48);
			lut_MinimapDecorCHRDest_hi = rom.GetFromBank(0x09, 0xBD19, 48);

			lut_MinimapTitleCHRDest_lo = rom.GetFromBank(0x09, 0xBB8A, 40);
			lut_MinimapTitleCHRDest_hi = rom.GetFromBank(0x09, 0xBBB2, 40);

			lut_MinimapNT = rom.GetFromBank(0x09, 0xB000, 960);

			lut_MinimapBGPal = rom.GetFromBank(0x09, 0xBF20, 16);

			ATData = rom.GetFromBank(0x09, 0xB3C0, 64);
		}

		private void StoreData()
		{
			rom.PutInBank(0x19, 0x8000, Blob.Concat(CHRData.Select(d => (Blob)d)));
			rom.PutInBank(0x09, 0xB000, NTData);
			rom.PutInBank(0x09, 0xB3C0, ATData);
			rom.PutInBank(0x09, 0xB400, EntranceData);
			rom.PutInBank(0x09, 0xB500, chr_MinimapSprite);
			rom.PutInBank(0x09, 0xB520, lut_MinimapBGPal);
		}

		private void StoreRoutine(byte minimaptrack)
		{
			rom.PutInBank(0x09,0xBC00,Blob.FromHex($"A9098557A9068D0080A9328D0180A9008D01208D1540A9{minimaptrack:X2}854BA90885FF2034BFAE80B4CA8A48BD40B48541BC00B42030BE68AAE000D0ECAD0220A9188D0620A9008D0620A200BD00B58D0720E8E02090F5A9008510A9808511A900A210205AE9A9008510A9B08511A920A204205AE9A20FBD20B59DC003CA10F7ADC0038DD003A90F8DD1038DD2032000FEA9028D14402050D8A5FF8D0020A90A8D0120A9008D0520A9E88D05202049BDA5240525F0F760"));
		}

		private void BuildCHR()
		{
			var decompressedMap = overworldMap;

			entrances = new List<(int x, int y)>();

			rawCHRData = new byte[256][];
			for (int i = 0; i < 256; i++) rawCHRData[i] = new byte[16];

			for (int x = 0; x < 256; x += 2)
				for (int y = 0; y < 256; y += 2)
				{
					var mmValue = GetReducedMMValue(decompressedMap, x, y);
					var mmPixelValue = mmValue & 0x03;
					var mmEntrance = (mmValue & 0x04) > 0;

					if (mmEntrance) entrances.Add((x, y));

					PushMMPixelValue(rawCHRData, x / 2, y / 2, mmPixelValue);
				}


			CHRData = rawCHRData.Distinct(new CHRComparer()).ToList();
		}

		private void BuildNT()
		{
			var chrDic = CHRData.Select((k, i) => (k, i)).ToDictionary(x => x.k, x => x.i, new CHRComparer());

			NTData = new byte[30 * 32];
			ClearNT(chrDic);

			for (int i = 0; i < 256; i++)
			{
				var chr = rawCHRData[i];
				var tile = (byte)chrDic[chr];
				var ntindex = GetNTIndex(i);

				NTData[ntindex] = tile;
			}
		}

		private void ClearNT(Dictionary<byte[], int> chrDic)
		{
			byte emptyCHR;
			if (chrDic.TryGetValue(new byte[16], out var idx))
			{
				emptyCHR = (byte)idx;
			}
			else if (chrDic.Count < 256)
			{
				//We did not encounter an empty CHR, but there is space for one
				//so we create an empty CHR

				CHRData.Add(new byte[16]);
				chrDic.Add(new byte[16], CHRData.Count - 1);
				emptyCHR = (byte)(CHRData.Count - 1);
			}
			else
			{
				//We don't have space for an empty CHR, so there will also be no decor
				//We change the attribute table and set all tiles outside the map to the second palette
				//Then we set the second palette to all blue

				emptyCHR = 0;

				ATData = new byte[] { 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55,
									  0x55, 0x55, 0x05, 0x05, 0x05, 0x05, 0x55, 0x55,
									  0x55, 0x55, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55,
									  0x55, 0x55, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55,
									  0x55, 0x55, 0x00, 0x00, 0x00, 0x00, 0x55, 0x55,
									  0x55, 0x55, 0x50, 0x50, 0x50, 0x50, 0x55, 0x55,
									  0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55,
									  0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55 };

				lut_MinimapBGPal[4] = 0x02;
				lut_MinimapBGPal[5] = 0x02;
				lut_MinimapBGPal[6] = 0x02;
				lut_MinimapBGPal[7] = 0x02;
			}

			for (int i = 0; i < NTData.Length; i++) NTData[i] = emptyCHR;
		}

		private void BuildEntranceData()
		{
			EntranceData = new byte[129];

			EntranceData[128] = (byte)Math.Min(entrances.Count, 63);

			for (int i = 0; i < EntranceData[128]; i++)
			{
				EntranceData[i] = (byte)entrances[i].x;
				EntranceData[i + 64] = (byte)entrances[i].y;
			}
		}

		private void FitDecor()
		{
			var freeCHR = 256 - CHRData.Count;
			if (freeCHR >= 48) FitDragons();

			freeCHR = 256 - CHRData.Count;
			if (freeCHR >= 40) FitTitle();
		}

		private void FitDragons()
		{
			int baseCHRIndex = CHRData.Count;

			CHRData.AddRange(chr_MinimapDecor.Select(d => (byte[])d));

			for(int i = 0; i < 48; i++)
			{
				var oldCHRAddress = lut_MinimapDecorCHRDest_hi[i] * 256 + lut_MinimapDecorCHRDest_lo[i];
				var oldTileId = oldCHRAddress / 16;

				CopyDecorNT(oldTileId, (byte)(baseCHRIndex + i));
			}
		}

		private void FitTitle()
		{
			int baseCHRIndex = CHRData.Count;

			CHRData.AddRange(chr_MinimapTitle.Select(d => (byte[])d));

			for (int i = 0; i < 40; i++)
			{
				var oldCHRAddress = lut_MinimapTitleCHRDest_hi[i] * 256 + lut_MinimapTitleCHRDest_lo[i];
				var oldTileId = oldCHRAddress / 16;

				CopyDecorNT(oldTileId, (byte)(baseCHRIndex + i));
			}
		}

		private void CopyDecorNT(int oldTileId, byte newTileId)
		{
			for (int i = 0; i < 960; i++)
			{
				if (lut_MinimapNT[i] == oldTileId) NTData[i] = newTileId;
			}
		}
		
		private int GetNTIndex(int MMIndex)
		{
			var x = MMIndex % 16;
			var y = MMIndex / 16;

			var ntx = 8 + x;
			var nty = 6 + y;

			return 32 * nty + ntx;
		}

		private void PushMMPixelValue(byte[][] rawCHRData, int x, int y, int mmPixelValue)
		{
			int tx = x / 8;
			int ty = y / 8;

			int tindex = tx + 16 * ty;

			int ox = x % 8;
			int oy = y % 8;

			var lowByte = rawCHRData[tindex][oy];
			var highByte = rawCHRData[tindex][oy + 8];

			lowByte = SetBit(lowByte, ox, (mmPixelValue & 0x01) > 0);
			highByte = SetBit(highByte, ox, (mmPixelValue & 0x02) > 0);

			rawCHRData[tindex][oy] = lowByte;
			rawCHRData[tindex][oy + 8] = highByte;
		}

		private byte SetBit(byte val, int ox, bool bit)
		{			
			return bit ? (byte)(val | (0x80 >> ox)) : val;
		}

		private byte GetReducedMMValue(List<List<byte>> decompressedMap, int x, int y)
		{
			var v1 = GetMMValue(decompressedMap, x, y);
			var v2 = GetMMValue(decompressedMap, x + 1, y);
			var v3 = GetMMValue(decompressedMap, x, y + 1);
			var v4 = GetMMValue(decompressedMap, x + 1, y + 1);

			return (byte)(v1 | v2 | v3 | v4);
		}

		private byte GetMMValue(List<List<byte>> decompressedMap, int x, int y)
		{
			var tile = decompressedMap[y][x];
			return lut_MinimapTileset[tile];
		}

		private class CHRComparer : IEqualityComparer<byte[]>
		{
			public bool Equals(byte[] x, byte[] y)
			{
				for (int i = 0; i < 16; i++)
				{
					if (x[i] != y[i]) return false;
				}

				return true;
			}

			public int GetHashCode([DisallowNull] byte[] obj)
			{
				int result = 0;

				for (int i = 0; i < 4; i++)
				{
					result ^= BitConverter.ToInt32(obj, i * 4);
				}

				return result;
			}
		}
	}
}
