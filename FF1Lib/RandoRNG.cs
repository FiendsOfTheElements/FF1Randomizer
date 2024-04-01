using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace FF1Lib
{
	public partial class FF1Rom
	{
		private MT19337 rng;
		private MT19337 funRng;
		//private MT19337 asyncRng;

		private void GenerateRng(Settings settings, Blob seed)
		{
			// to review 
			if (settings.GetInt("OwMapExchange") == (int)OwMapExchanges.GenerateNewOverworld || settings.GetInt("OwMapExchange") == (int)OwMapExchanges.LostWoods)
			{
				// Procgen maps can be either
				// generated or imported.  All else
				// being equal, we want the user who
				// generated the map
				// (OwMapExchange == GenerateNewOverworld)
				// and the user who imported the map
				// (OwMapExchange == ImportCustomMap)
				// to get the same ROM, so for the
				// purposes of initializing the RNG
				// consider them all to be
				// "ImportCustomMap".
				settings.UpdateSetting("OwMapExchange", (int)OwMapExchanges.ImportCustomMap);
				/*
				flagsForRng = flags.ShallowCopy();
				flagsForRng.OwMapExchange = OwMapExchanges.ImportCustomMap;*/
			}

			Blob resourcesPackHash = new byte[1];

			using (SHA256 hasher = SHA256.Create())
			{
				// review resourcepack too
				/*
				if (flags.ResourcePack != null)
				{
					var rp = new MemoryStream(Convert.FromBase64String(flags.ResourcePack));
					if (flags.TournamentSafe || ResourcePackHasGameplayChanges(rp))
					{
						rp.Seek(0, SeekOrigin.Begin);
						resourcesPackHash = hasher.ComputeHash(rp).ToArray();
					}
				}*/

				Blob FlagsBlob = Encoding.UTF8.GetBytes(settings.GenerateFlagstring());
				Blob SeedAndFlags = Blob.Concat(new Blob[] { FlagsBlob, seed, resourcesPackHash });
				Blob hash = hasher.ComputeHash(SeedAndFlags);
				rng = new MT19337(BitConverter.ToUInt32(hash, 0));
				funRng = new MT19337(BitConverter.ToUInt32(hash, 0));
			}

			// We have to do "fun" stuff last because it alters the RNG state.
			// Back up Rng so that fun flags are uniform when different ones are selected
			//funRng = rng.Next();


		}

	}
}
