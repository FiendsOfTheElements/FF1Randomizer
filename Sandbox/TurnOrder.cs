using System;
using System.Collections.Generic;
using System.Text;
using RomUtilities;

namespace Sandbox
{
    public class TurnOrder
    {
		public static readonly Blob RNGTable = Blob.FromHex("AED0388AED60DB725C5927D80A4AF43408A9C396563BF155F86B31EF6D28AC41681E2AC1E58F50F53E7BB74C143912CDB2628B823CBA63853A17B82EB5BE20CB46512CCF037853970669EB7786E6EA740C21E240D45A3DC72B94D58C44FDEED24300BBFAC61D98A0D3545F5EDCA800AF93A1E16C04DEB6D73616C5C8C4E40F02ABE8339973116A0967F3FFA2DF320E1F0D90256475B3652FC9B0DA5D9FEC29CEE3F0917A5845241C47A489182DCCBD6F80F68122E90770FBDDAD35A661B4A3FEB1304B15486E4F5B139C839201C2197F1A1B71B93F4E9BBF9E870B1057F226799A05C0E0F74D7DCA529DF9BCAAFC8D7ED1A542E7D676A7848E667C23883749D9");

	    private static int rngIndex;

	    public static void TurnOrderTestVanilla()
	    {
			int[][] frequencies = new int[4][];
		    for (int i = 0; i < 4; i++)
		    {
			    frequencies[i] = new int[13];
		    }

		    for (int i = 0; i < 256; i++)
		    {
			    rngIndex = i;
			    int[] turnOrder = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 0x80, 0x81, 0x82, 0x83 };
			    for (int j = 0; j < 16; j++)
			    {
				    Swap(turnOrder, GetSwapPosition(12), GetSwapPosition(12));
			    }

			    frequencies[0][Array.IndexOf(turnOrder, 0x80)]++;
			    frequencies[1][Array.IndexOf(turnOrder, 0x81)]++;
			    frequencies[2][Array.IndexOf(turnOrder, 0x82)]++;
			    frequencies[3][Array.IndexOf(turnOrder, 0x83)]++;
		    }

			Console.WriteLine("Vanilla:");
		    for (int i = 0; i < 4; i++)
		    {
			    Console.WriteLine("Party member {0}:", i + 1);
			    for (int j = 0; j < 13; j++)
			    {
				    Console.WriteLine("    {0}: {1}", j, frequencies[i][j]);
			    }
		    }
			Console.WriteLine();
	    }

		public static void TurnOrderTestImproved()
	    {
		    int[][] frequencies = new int[4][];
		    for (int i = 0; i < 4; i++)
		    {
			    frequencies[i] = new int[13];
		    }

		    for (int i = 0; i < 256; i++)
		    {
			    rngIndex = i;
			    int[] turnOrder = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 0x80, 0x81, 0x82, 0x83 };
			    for (int j = 0; j < 64; j++)
			    {
				    Swap(turnOrder, GetSwapPosition(12), GetSwapPosition(12));
			    }

			    frequencies[0][Array.IndexOf(turnOrder, 0x80)]++;
			    frequencies[1][Array.IndexOf(turnOrder, 0x81)]++;
			    frequencies[2][Array.IndexOf(turnOrder, 0x82)]++;
			    frequencies[3][Array.IndexOf(turnOrder, 0x83)]++;
		    }

		    Console.WriteLine("Improved:");
		    for (int i = 0; i < 4; i++)
		    {
			    Console.WriteLine("Party member {0}:", i + 1);
			    for (int j = 0; j < 13; j++)
			    {
				    Console.WriteLine("    {0}: {1}", j, frequencies[i][j]);
			    }
		    }
			Console.WriteLine();
	    }

		public static void TurnOrderTestFisherYates()
	    {
		    int[][] frequencies = new int[4][];
		    for (int i = 0; i < 4; i++)
		    {
			    frequencies[i] = new int[13];
		    }

		    for (int i = 0; i < 256; i++)
		    {
			    rngIndex = i;
			    int[] turnOrder = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 0x80, 0x81, 0x82, 0x83 };
			    for (int j = 12; j >= 1; j--)
			    {
				    Swap(turnOrder, j, GetSwapPosition(j));
			    }

			    frequencies[0][Array.IndexOf(turnOrder, 0x80)]++;
			    frequencies[1][Array.IndexOf(turnOrder, 0x81)]++;
			    frequencies[2][Array.IndexOf(turnOrder, 0x82)]++;
			    frequencies[3][Array.IndexOf(turnOrder, 0x83)]++;
		    }

		    Console.WriteLine("Fisher-Yates:");
		    for (int i = 0; i < 4; i++)
		    {
			    Console.WriteLine("Party member {0}:", i + 1);
			    for (int j = 0; j < 13; j++)
			    {
				    Console.WriteLine("    {0}: {1}", j, frequencies[i][j]);
			    }
		    }
			Console.WriteLine();
	    }

	    public static int GetSwapPosition(int max)
	    {
		    if (rngIndex > 255)
		    {
			    rngIndex -= 256;
		    }

		    return RNGTable[rngIndex++] * (max + 1) / 256;
	    }

	    public static void Swap(int[] array, int first, int second)
	    {
		    int temp = array[first];
		    array[first] = array[second];
		    array[second] = temp;
	    }
    }
}
