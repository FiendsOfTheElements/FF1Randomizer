using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace FF1Lib
{
	public class RomStampModel
	{
		public const int romStampBank = 0x1B;
		public const int romStampOffset = 0xA000;
		private String seed;
		private String flag;
		private String commitSha;
		private String inputRomSha;
		private String romStamp;

		public RomStampModel (String _seed, String _flags, String _commitSha, String _inputRomSha)
		{
			seed = _seed.Trim();
			flag = _flags.Trim();
			commitSha = _commitSha.Trim();
			inputRomSha = _inputRomSha.Trim();
			romStamp = BuildRomStampJson().Trim();
		}
		public RomStampModel(String _romStampHex)
		{
			DeconstructRomStamp(DecodeHex(_romStampHex));
		}

		private String DecodeHex(String _romStampHex)
		{
			try
			{
				string ascii = string.Empty;

				for (int i = 0; i < _romStampHex.Length; i += 2)
				{
					String hs = string.Empty;

					hs = _romStampHex.Substring(i, 2);
					uint decval = System.Convert.ToUInt32(hs, 16);
					char character = System.Convert.ToChar(decval);
					ascii += character;

				}

				return ascii;
			}
			catch (Exception ex) { Console.WriteLine(ex.Message); }

			return string.Empty;
		}
		private String BuildRomStampJson()
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);
			JsonWriter writer = new JsonTextWriter(sw);
			writer.Formatting = Formatting.Indented;

			writer.WriteStartObject();
			writer.WritePropertyName("s"); //Seed
			writer.WriteValue(seed);
			writer.WritePropertyName("f"); //Encoded Flag String
			writer.WriteValue(flag);
			writer.WritePropertyName("g"); // Git Commit SHA
			writer.WriteValue(commitSha);
			writer.WritePropertyName("r"); // Input ROM SHA-1
			writer.WriteValue(inputRomSha);
			writer.WriteEndObject();

			return sb.ToString();
		}

		private void DeconstructRomStamp(String _romStampJson)
		{
			romStamp = _romStampJson;
			JsonTextReader reader = new JsonTextReader(new StringReader(_romStampJson));
			while (reader.Read())
			{
				if (reader.TokenType.ToString().Equals("PropertyName"))
				{
					switch (reader.Value.ToString()){
						case "s":
							reader.Read();
							seed = reader.Value.ToString();
							break;
						case "f":
							reader.Read();
							flag = reader.Value.ToString();
							break;
						case "g":
							reader.Read();
							commitSha = reader.Value.ToString();
							break;
						case "r":
							reader.Read();
							inputRomSha = reader.Value.ToString();
							break;
					}
				}
			}
		}


		public String GetRomStampJson()
		{
			return romStamp;
		}
		public String GetRomStampHex()
		{
			byte[] bytes = Encoding.Default.GetBytes(romStamp);
			string hexString = BitConverter.ToString(bytes);
			return hexString.Replace("-", "");
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
