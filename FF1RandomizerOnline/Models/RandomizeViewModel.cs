using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FF1Lib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FF1RandomizerOnline.Models
{
    public class RandomizeViewModel
    {
		public string Seed { get; set; }
	    public Flags Flags { get; set; }

	    public IFormFile File { get; set; }
    }
}
