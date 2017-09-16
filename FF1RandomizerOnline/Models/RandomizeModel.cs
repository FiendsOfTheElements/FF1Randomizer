using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FF1Lib;
using Microsoft.AspNetCore.Http;

namespace FF1RandomizerOnline.Models
{
    public class RandomizeModel
    {
	    public string Seed;
	    public Flags Flags;

	    public IFormFile File;
    }
}
