﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class RomStampModel
	{
		private String seed;
		private const int seedSize = 64;
		private String flag;
		private const int flagSize= 19;
		private String commitSha;
		private const int commitShaSize = 40;
		private String inputRomSha;
		private const int inputRomShaSize = 40;
		private String romStamp;
		private const int romStampSize = seedSize + flagSize + commitShaSize + inputRomShaSize;

		public RomStampModel (String _seed, String _flags, String _commitSha, String _inputRomSha)
		{
			seed = stringPadder(_seed, seedSize);
			flag = stringPadder(_flags, flagSize);
			commitSha = stringPadder(_commitSha, commitShaSize);
			inputRomSha = stringPadder(_inputRomSha, inputRomShaSize);
			romStamp = buildRomStamp();
		}
		public RomStampModel(String _romStamp)
		{
			romStamp = _romStamp;
			deconstructRomStamp();
		}

		private String buildRomStamp()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(seed);
			sb.Append(flag);
			sb.Append(commitSha);
			sb.Append(inputRomSha);
			if (!(sb.Length == romStampSize))
			{
				throw new Exception("Rom stamp is incorrect size");
			}
			return sb.ToString();
		}

		private void deconstructRomStamp()
		{
			int romStampIndex = 0;
			seed = romStamp.Substring(romStampIndex, seedSize);
			romStampIndex += seedSize;
			flag = romStamp.Substring(romStampIndex, flagSize);
			romStampIndex += flagSize;
			commitSha = romStamp.Substring(romStampIndex, commitShaSize);
			romStampIndex += commitShaSize;
			inputRomSha = romStamp.Substring(romStampIndex);
		}

		private String stringPadder(String _inputString, int _maxLenth)
		{
			if (_inputString.Length <= _maxLenth)
			{
				return _inputString.PadLeft(_maxLenth, ' ');
			}
			else
			{
				throw new Exception("rom stamp part has an invalid size");
			}
		}

		public String getRomStamp()
		{
			return romStamp;
		}

		public String getSeed()
		{
			return seed;
		}

		public String getEncodedFlagString()
		{
			return flag;
		}

		public String getCommitSha()
		{
			return commitSha;
		}

		public String getInputRomSha()
		{
			return inputRomSha;
		}
	}
}
