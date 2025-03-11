global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;
global using RomUtilities;

using System.Security.Cryptography;
using System.Text.RegularExpressions;

using FF1Lib.Procgen;
using FF1Lib.Assembly;
using System.Numerics;
using System.Diagnostics;
using System.Xml.Linq;

namespace FF1Lib;

// ReSharper disable once InconsistentNaming
public partial class FF1Rom : NesRom
{
	public string RomPutsTracker = "";
	public new void Put(int index, Blob data)
	{
		//Debug.Assert(index <= 0x4000 * 0x0E + 0x9F48 - 0x8000 && (index + data.Length) > 0x4000 * 0x0E + 0x9F48 - 0x8000);
		base.Put(index, data);
		
		/*
		// This is tracking code to register all the Puts to the rom, for debugging only
		// Get the stack calling the put
		StackTrace trace = new StackTrace();
		string stackline = "";

		foreach (var frame in trace.GetFrames())
		{
			var name = frame.GetMethod().Name;
			// Might apply only to blazorizer interface, but if we're at move next, we don't need to go further
			if (name == "MoveNext")
			{
				break;
			}
			// no need to write down put/putinbank either
			else if (name == "Put" || name == "PutInBank")
			{
				continue;
			}

			var origin = frame.GetMethod().DeclaringType.Name;
			stackline += origin + "-" + name + "/";
		}

		// if only 0's are written, we're wiping data
		var sumdata = 1;
		try
		{
			// if it's not 0's, sum() will probably overflow, we skip that error then
			sumdata = data.ToInts().Sum();
		}
		catch
		{ }

		if (sumdata == 0)
		{
			stackline += "[Wipe]";
		}

		// get the bank, start offset, end offset and write to tracker
		int bank = index / 0x4000;
		int offsetstart = bank == 0x1F ? (index % 0x4000) + 0xC000 : (index % 0x4000) + 0x8000;
		int offsetend = bank == 0x1F ? (index % 0x4000) + 0xC000 + data.Length - 1 : (index % 0x4000) + 0x8000 + data.Length - 1;
		var fullstring = $"{stackline} - Bank: {bank:X2)}, Address: {offsetstart:X4} - {offsetend:X4}";
		RomPutsTracker += fullstring + "\n";
		*/
	}

	public void PutInBank(int bank, int address, Blob data)
	{
		if (bank == 0x1F)
		{
			if ((address - 0xC000) + data.Length > 0x4000)
			{
				throw new Exception("Data is too large to fit within its bank.");
			}
			int offset = (bank * 0x4000) + (address - 0xC000);
			this.Put(offset, data);
		}
		else
		{
			if ((address - 0x8000) + data.Length > 0x4000)
			{
				throw new Exception("Data is too large to fit within its bank.");
			}
			int offset = (bank * 0x4000) + (address - 0x8000);
			this.Put(offset, data);
		}
	}

	// overload for putting single byte in a bank
	public void PutInBank(int bank, int address, byte data)
	{
		if (bank == 0x1F)
		{
			if ((address - 0xC000) >= 0x4000)
			{
				throw new Exception("Address provided is after the end of this bank.");
			}
			int offset = (bank * 0x4000) + (address - 0xC000);
			Data[offset] = data;
		}
		else
		{
			if ((address - 0x8000) >= 0x4000)
			{
				throw new Exception("Address provided is after the end of this bank.");
			}
			int offset = (bank * 0x4000) + (address - 0x8000);
			Data[offset] = data;
		}
	}

	//overlaod for putting a ushort in a bank; converts the ushort to a little-endian pair of bytes
	public void PutInBank(int bank, int address, ushort usdata)
	{
		var data = Blob.FromUShorts(new ushort[] {usdata});
		if (bank == 0x1F)
		{
			if ((address - 0xC000) + data.Length > 0x4000)
			{
				throw new Exception("Data is too large to fit within its bank.");
			}
			int offset = (bank * 0x4000) + (address - 0xC000);
			this.Put(offset, data);
		}
		else
		{
			if ((address - 0x8000) + data.Length > 0x4000)
			{
				throw new Exception("Data is too large to fit within its bank.");
			}
			int offset = (bank * 0x4000) + (address - 0x8000);
			this.Put(offset, data);
		}
	}

	public Blob GetFromBank(int bank, int address, int length, bool extended = true)
	{
		int lastbank = extended ? 0x1F : 0x0F;

		if (bank == lastbank)
		{
			if ((address - 0xC000) + length > 0x4000)
			{
				throw new Exception("Data is too large to fit within one bank.");
			}
			int offset = (bank * 0x4000) + (address - 0xC000);
			return this.Get(offset, length);
		}
		else
		{
			if ((address - 0x8000) + length > 0x4000)
			{
				throw new Exception("Data is too large to fit within one bank.");
			}
			int offset = (bank * 0x4000) + (address - 0x8000);
			return this.Get(offset, length);
		}
	}

	public Blob CreateLongJumpTableEntry(byte bank, ushort addr)
	{
		List<byte> tmp = new List<byte> { 0x20, 0xC8, 0xD7 }; // JSR $D7C8, beginning of each table entry

		var addrBytes = BitConverter.GetBytes(addr); // next add the address to jump to
		tmp.Add(addrBytes[0]);
		tmp.Add(addrBytes[1]);
		tmp.Add(bank); //finally, add the bank that the routine is located in

		return tmp.ToArray();
	}

	public FF1Rom(string filename) : base(filename)
	{ }

	public FF1Rom(Stream readStream) : base(readStream)
	{ }

	private FF1Rom()
	{ }

	public static async Task<FF1Rom> CreateAsync(Stream readStream)
	{
		var rom = new FF1Rom();
		await rom.LoadAsync(readStream);

		return rom;
	}
	public void AssureSafe()
	{
		using (SHA256 hasher = SHA256.Create())
		{
			byte[] hashable = Data.ToBytes();

			//zero out overworld palette data
			for (int i = 0x380; i < 0x390; i++)
			{
				hashable[i] = 0;
			}

			//zero out mapman palette data
			for (int i = 0x390; i < 0x3BC; i++)
			{
				hashable[i] = 0;
			}

			//zero out standard map palette data
			for (int i = 0x2000; i < 0x2C00; i++)
			{
				hashable[i] = 0;
			}

			//zero out standard map object graphic lookup
			for (int i = 0x2E00; i < 0x2ED0; i++)
			{
				hashable[i] = 0;
			}

			//zero out backdrop palette data
			for (int i = 0x3200; i < 0x3260; i++)
			{
				hashable[i] = 0;
			}

			//zero out battle backdrop lookup
			for (int i = 0x3300; i < 0x3380; i++)
			{
				hashable[i] = 0;
			}

			//zero out overworld graphics
			for (int i = 0x8000; i < 0x9000; i++)
			{
				hashable[i] = 0;
			}

			//zero out character mapman graphics
			for (int i = 0x9000; i < 0xA200; i++)
			{
				hashable[i] = 0;
			}

			//zero out standard map object graphics
			for (int i = 0xA200; i < 0xC000; i++)
			{
				hashable[i] = 0;
			}

			//zero out standard map graphics
			for (int i = 0xC000; i < 0x10000; i++)
			{
				hashable[i] = 0;
			}

			//zero out character battle graphics
			for (int i = 0x25000; i < 0x26800; i++)
			{
				hashable[i] = 0;
			}

			// Battlepalettes
			for (int i = 0x30F20; i < 0x31020; i++)
			{
				hashable[i] = 0;
			}

			// lut_InBattleCharPaletteAssign (LUT for assigning palettes to in-battle char sprites)
			for (int i = 0x3203C; i < 0x32048; i++)
			{
				hashable[i] = 0;
			}

			// BattleSpritePalettes (palette for battle sprites)
			for (int i = 0x3EBA4; i < 0x3EBB5; i++)
			{
				hashable[i] = 0;
			}

			// lutClassBatSprPalette (LUT for battle sprite palettes)
			for (int i = 0x3ECA4; i < 0x3ECB0; i++)
			{
				hashable[i] = 0;
			}

			var Hash = hasher.ComputeHash(hashable);
			if (ByteArrayToString(Hash) != "0614d282abe33d5c6e9a22f6cc7b5f972d30c292d4b873ce07f703c1a14b168c")
			{
				Console.WriteLine($"Rom hash: {ByteArrayToString(Hash)}");
				throw new TournamentSafeException("File has been modified");
			}
		}
	}

	public class TournamentSafeException : Exception
	{
		public TournamentSafeException(string message)
			: base(message) { }
	}

	private static string ByteArrayToString(byte[] ba)
	{
		StringBuilder hex = new StringBuilder(ba.Length * 2);
		foreach (byte b in ba)
			hex.AppendFormat("{0:x2}", b);
		return hex.ToString();
	}
	public override bool Validate()
	{
		return Get(0, 16) == Blob.FromHex("06400e890e890e401e400e400e400b42");
	}


	//public void WriteSeedAndFlags(string seed, Flags flags, Flags flagsforrng, Flags umodifiedflags, string resourcepackhash, uint last_rng_value)
	public void WriteSeedAndFlags(string seed, Flags flags, Flags flagsforrng, Flags umodifiedflags, string resourcepackhash, uint last_rng_value)
	{

		string flagstext = Flags.EncodeFlagsText(flags);
		var rngflagstext = Flags.EncodeFlagsText(flagsforrng);
		string owseed = "none";

		if (flags.ReplacementMap != null)
		{
			owseed = flags.MapGenSeed.ToString("X8");
			rngflagstext += "_" + flags.ReplacementMap.ComputeChecksum();
		}

		rngflagstext += "_" + resourcepackhash;

		// Replace most of the old copyright string printing with a JSR to a LongJump
		Put(0x38486, Blob.FromHex("20B9FF60"));

		// DrawSeedAndFlags LongJump
		PutInBank(0x1F, 0xFFB9, CreateLongJumpTableEntry(0x0F, 0x8980));

		Blob hash;
		var hasher = SHA256.Create();
		hash = hasher.ComputeHash(Encoding.ASCII.GetBytes($"{seed}_{rngflagstext}_{FFRVersion.Sha}_{last_rng_value}"));

		var hashpart = BitConverter.ToUInt64(hash, 0);
		hash = Blob.FromHex("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
		for (int i = 8; i < 24; i += 2)
		{
			// 0xD4 through 0xDF are good symbols to use.
			hash[i] = (byte)(0xD4 + hashpart % 12);
			hashpart /= 12;
		}

		SavedHash = hash;

		Regex rgx = new Regex("[^a-zA-Z0-9]");
		// Put the new string data in a known location.
		PutInBank(0x0F, 0x8900, Blob.Concat(
			FF1Text.TextToCopyrightLine("Final Fantasy Randomizer " + FFRVersion.Version),
			FF1Text.TextToCopyrightLine((FFRVersion.Branch == "master" ? "Seed " : rgx.Replace(FFRVersion.Branch, "") + " BUILD ") + seed),
			hash));

		// Write Flagstring + Version for reference
		var urlpart = (FFRVersion.Branch == "master") ? FFRVersion.Version.Replace('.','-') : "beta-" + FFRVersion.Sha.PadRight(7).Substring(0, 7);
		PutInBank(0x1E, 0xBE00, Encoding.ASCII.GetBytes($"FFRInfo|Seed: {seed}|OW Seed: {owseed}|Res. Pack Hash: {((resourcepackhash == "00") ? "none" : resourcepackhash)}|Flags: {Flags.EncodeFlagsText(umodifiedflags)}|Version: {urlpart}"));
	}

	public string SpoilBlursings()
	{
		string blursetext = "";
		List<string> classlist = new() { "Fighter", "Thief", "Black Belt", "Red Mage", "White Mage", "Black Mage" };
		for (int i = 0; i < 6; i++)
		{
			var pointer = GetFromBank(0x1E, 0x8950 + (i * 2), 2).ToUShorts();
			var endpointer = GetFromBank(0x1E, 0x8950 + ((i+1) * 2), 2).ToUShorts();

			var temptext = FF1Text.BytesToText(GetFromBank(0x1E, pointer[0], endpointer[0] - pointer[0])).Split("\n").ToList();

			temptext.RemoveAll(x => x == "");

			for (int j = 0; j < temptext.Count; j++)
			{
				temptext[j] = temptext[j].Replace("\n", "");
				temptext[j] = temptext[j].Replace("@S", "Sword");
				temptext[j] = temptext[j].Replace("@H", "Hammer");
				temptext[j] = temptext[j].Replace("@K", "Knife");
				temptext[j] = temptext[j].Replace("@X", "Axe");
				temptext[j] = temptext[j].Replace("@F", "Staff");
				temptext[j] = temptext[j].Replace("@N", "Nunchuks");
				temptext[j] = temptext[j].Replace("@A", "Armor");
				temptext[j] = temptext[j].Replace("@s", "Shield");
				temptext[j] = temptext[j].Replace("@h", "Helmet");
				temptext[j] = temptext[j].Replace("@G", "Gauntlet");
				temptext[j] = temptext[j].Replace("@B", "Bracelet");
				temptext[j] = temptext[j].Replace("@T", "Shirt");
			}

			blursetext += classlist[i] + "\n" + "BONUS" + "\n" + String.Join("\n", temptext.ToArray()) + "\n\n";
		}

		return blursetext;
	}

	public string RomInfo()
	{
		var rawtext = GetFromBank(0x1E, 0xBE00, 0x200);
		var trimedtext = rawtext.ReplaceOutOfPlace(Blob.FromHex("00"), Blob.FromHex(""));
		string infotext = Encoding.ASCII.GetString(trimedtext).Replace('|', '\n');

		return infotext;
	}
	public string GetHash()
	{
		string hashtext = FF1Text.BytesToText(SavedHash);

		hashtext = hashtext.Replace(" ", "");
		hashtext = hashtext.Replace("@", "");

		return hashtext;
	}
}
