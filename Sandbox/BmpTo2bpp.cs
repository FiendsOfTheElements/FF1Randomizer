using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sandbox
{
	// This is DUMB, it is not meant to be generalizeable at all, it is meant to get a thing done.
	class BmpTo2bpp
    {
	    public static void Convert(string inputFilename, string outputFilename)
	    {
		    byte[,] pixels;

			using (var inStream = new FileStream(inputFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				byte[] buffer = new byte[4];

				inStream.Seek(0x0A, SeekOrigin.Begin);
				inStream.Read(buffer, 0, 4);
				var pixelOffset = BitConverter.ToInt32(buffer, 0);

				inStream.Seek(0x12, SeekOrigin.Begin);
				inStream.Read(buffer, 0, 4);
				var width = BitConverter.ToInt32(buffer, 0);
				inStream.Read(buffer, 0, 4);
				var height = BitConverter.ToInt32(buffer, 0);

				byte[] pixelBytes = new byte[width*height/2];
				inStream.Seek(pixelOffset, SeekOrigin.Begin);
				inStream.Read(pixelBytes, 0, pixelBytes.Length);

				pixels = new byte[width, height];
				int x = width - 1, y = height - 1;
				for (int i = 0; i < pixelBytes.Length; i++)
				{
					var high = (byte)((pixelBytes[i] & 0xF0) >> 4);
					var low = (byte)(pixelBytes[i] & 0x0F);

					pixels[x--, y] = Decode(high); // x--???
					pixels[x--, y] = Decode(low);
					if (x < 0)
					{
						x = width - 1;
						y--;
					}
				}
			}

			using (var outStream = new FileStream(outputFilename, FileMode.Create, FileAccess.Write, FileShare.None))
		    {
			    var buffer = new byte[16];

				for (int y = 0; y < 6; y++)
			    {
				    for (int x = 5; x >= 0; x--) // ???
				    {
					    var lowBits = new BitArray(64);
					    var highBits = new BitArray(64);
					    for (int j = 0; j < 8; j++)
					    {
						    for (int i = 0; i < 8; i++)
						    {
							    var pixel = pixels[8 * x + i, 8 * y + j];
							    lowBits[8 * j + i] = (pixel & 0x01) != 0;
							    highBits[8 * j + i] = (pixel & 0x02) != 0;
						    }
						}

						lowBits.CopyTo(buffer, 0);
						highBits.CopyTo(buffer, 8);

						outStream.Write(buffer, 0, 16);
				    }
			    }
		    }
		}

	    private static byte Decode(byte coded)
	    {
		    if (coded == 0)
			    return 0;
		    if (coded < 6)
			    return 3;
		    if (coded == 6)
			    return 2;
		    return 1;
	    }
	}
}
