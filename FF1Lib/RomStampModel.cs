using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class RomStampModel
	{
		private byte[] seed = new byte[64];
		private byte[] flags = new byte[19];
		private byte[] commitSha = new byte[40];
		private byte[] inputRomSha = new byte[40];

		public RomStampModel (String _seed, String _flags, String _commitSha, String _inputRomSha)
		{
			seed = byteArrayFormatter(_seed, seed.Length);
			flags = byteArrayFormatter(_flags, flags.Length);
			commitSha = byteArrayFormatter(_commitSha, commitSha.Length);
			inputRomSha = byteArrayFormatter(_inputRomSha, inputRomSha.Length);
		}

		private byte[] byteArrayFormatter(String _inputString, int _maxLenth)
		{
			return Encoding.ASCII.GetBytes(_inputString.PadLeft(_maxLenth, ' '));
		}
	}
}
