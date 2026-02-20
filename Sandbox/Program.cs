global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Threading.Tasks;
global using FF1Lib;
global using RomUtilities;

using Sandbox;

var filename = "FF1.NES";
var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);

var rom = new FF1Rom(fs);

//await Performance.Run();

TestMapGen.Run(rom);
