using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FF1RandomizerOnline.Models;
using FF1Lib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using RomUtilities;

namespace FF1RandomizerOnline.Controllers
{
    public class HomeController : Controller
    {
	    public IActionResult Index()
	    {
		    return View();
	    }

	    [HttpGet]
	    public IActionResult Randomize()
	    {
		    return View(new RandomizeViewModel());
	    }

	    [HttpPost]
		[ValidateAntiForgeryToken]
	    public IActionResult Randomize(RandomizeViewModel viewModel)
	    {
		    if (viewModel.File.Length < 256 * 1024 || viewModel.File.Length > (256 + 8) * 1024)
		    {
			    return BadRequest("Unexpected file length, FF1 ROM should be close to 256 kB.");
		    }

		    var rom = new FF1Rom(viewModel.File.OpenReadStream());
		    if (!rom.Validate())
		    {
			    return BadRequest("File does not appear to be a valid FF1 NES ROM.");
		    }

			rom.Randomize(Blob.FromHex(viewModel.Seed), viewModel.Flags);

		    var filename = viewModel.File.FileName;
		    var extensionIndex = filename.LastIndexOf('.');
		    var newFilename = extensionIndex == -1 ? filename : filename.Substring(0, extensionIndex);
		    newFilename = $"{newFilename}_{viewModel.Seed}_{FF1Rom.EncodeFlagsText(viewModel.Flags)}.nes";

		    Response.StatusCode = 200;
			Response.ContentLength = rom.TotalLength;
		    Response.ContentType = "application/octet-stream";
			Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{newFilename}\"");
			
			rom.Save(Response.Body);
			Response.Body.Close();

			return new EmptyResult();
	    }

		public IActionResult Error()
        {
            return View();
        }
    }
}
