using System;
using FF1Lib;
using RomUtilities;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
	        TurnOrder.TurnOrderTestVanilla();
			TurnOrder.TurnOrderTestImproved();
			TurnOrder.TurnOrderTestFisherYates();
        }
	}
}
