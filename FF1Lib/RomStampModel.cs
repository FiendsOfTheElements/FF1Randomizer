using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class RomStampModel
	{
		public const String romStampLocation = "a";
		private String seed;
		private const int seedSize = 64;
		private String flag;
		private const int flagSize= 19;
		private String commitSha;
		private const int commitShaSize = 40;
		private String inputRomSha;
		private const int inputRomShaSize = 40;
		// This filler is blank space reserved for the ROM stamp.
		private String filler;
		private const int fillerSize = 37;
		private String romStamp;
		private const int romStampSize = seedSize + flagSize + commitShaSize + inputRomShaSize + fillerSize;

		public RomStampModel (String _seed, String _flags, String _commitSha, String _inputRomSha)
		{
			seed = StringPadder(_seed, seedSize);
			flag = StringPadder(_flags, flagSize);
			commitSha = StringPadder(_commitSha, commitShaSize);
			inputRomSha = StringPadder(_inputRomSha, inputRomShaSize);
			filler = StringPadder("", fillerSize);
			romStamp = BuildRomStamp();
		}
		public RomStampModel(String _romStamp)
		{
			romStamp = _romStamp;
			DeconstructRomStamp();
		}

		private String BuildRomStamp()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(seed);
			sb.Append(flag);
			sb.Append(commitSha);
			sb.Append(inputRomSha);
			sb.Append(filler);
			if (!(sb.Length == romStampSize))
			{
				throw new Exception("Rom stamp is incorrect size");
			}
			return sb.ToString();
		}

		private void DeconstructRomStamp()
		{
			int romStampIndex = 0;
			seed = romStamp.Substring(romStampIndex, seedSize).Trim();
			romStampIndex += seedSize;
			flag = romStamp.Substring(romStampIndex, flagSize).Trim();
			romStampIndex += flagSize;
			commitSha = romStamp.Substring(romStampIndex, commitShaSize).Trim();
			romStampIndex += commitShaSize;
			inputRomSha = romStamp.Substring(romStampIndex, inputRomShaSize).Trim();
			//No need to extract the filler as it is just blank space.
		}

		private String StringPadder(String _inputString, int _maxLenth)
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

		public String GetRomStamp()
		{
			return romStamp;
		}

		public String GetSeed()
		{
			return seed;
		}

		public String GetEncodedFlagString()
		{
			return flag;
		}

		public String GetCommitSha()
		{
			return commitSha;
		}

		public String GetInputRomSha()
		{
			return inputRomSha;
		}
	}
}
