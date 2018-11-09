using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FF1RandomizerOnline.Models;
using FF1Lib;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using RomUtilities;

namespace FF1RandomizerOnline.Controllers
{
	public class HomeController : Controller
	{
		private readonly IHostingEnvironment _environment;

		public HomeController(IHostingEnvironment environment)
		{
			_environment = environment;
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			base.OnActionExecuting(context);

			var betaString = _environment.IsDevelopment() ? " Beta" : "";
			ViewData["Title"] = "FF1 Randomizer Online " + FF1Rom.Version + betaString;
			ViewData["DebugOnlyPreset"] = _environment.IsDevelopment() ? "<a v-on:click.prevent=\"preset('DEBUG')\">DEBUG</a>," : "";

			// Make this alpha only, maybe?
			ViewData["BuildMeta"] = _environment.IsDevelopment() ?
				@"<h4>
					<b>WELCOME TO FFR DEVELOPMENT BUILDS - Good luck! You'll Need it!</b>
					<ul>
						<li style=""color: red; font-weight:bold"">DO NOT USE THIS SITE FOR LEAGUE RACES. USE THE PRODUCTION SITE HERE: <a href=""http://finalfantasyrandomizer.com"">http://finalfantasyrandomizer.com</a>.</li>
						<li>FFR development websites can be updated literally anytime with no agenda or stability guarantees.</li>
						<li>Features may appear and disappear as different developers work and refine the flags for them.</li>
					</ul>
				</h4>" : "";

		}

		public IActionResult Index()
		{
			return View();
		}
		
		public IActionResult WhatsNew()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Randomize()
		{
			var vm = new RandomizeViewModel {
				File = null,
				Seed = Blob.Random(4).ToHex(),
				Flags = Flags.FromJson(System.IO.File.ReadAllText(Path.Combine(Startup.GetPresetsDirectory(), "default.json")))
			};
			vm.FlagsInput = Flags.EncodeFlagsText(vm.Flags);

			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Randomize(RandomizeViewModel viewModel)
		{
			// Easier to just early return here and not have to verify viewModel.File != null repeatedly.
			if (!ModelState.IsValid)
			{
				return View(viewModel);
			}

			if (viewModel.File.Length < 256 * 1024 || viewModel.File.Length > (256 + 8) * 1024)
			{
				ModelState.AddModelError("File", "Unexpected file length, FF1 ROM should be close to 256 kB.");
			}

			var rom = await FF1Rom.CreateAsync(viewModel.File.OpenReadStream());
			if (!rom.Validate())
			{
				ModelState.AddModelError("File", "File does not appear to be a valid FF1 NES ROM.");
			}

			if (!ModelState.IsValid)
			{
				return View(viewModel);
			}

			rom.Randomize(Blob.FromHex(viewModel.Seed), viewModel.Flags);

			var filename = viewModel.File.FileName;
			var pathIndex = filename.LastIndexOfAny(new[] { '\\', '/' });
			filename = pathIndex == -1 ? filename : filename.Substring(pathIndex + 1);

			var extensionIndex = filename.LastIndexOf('.');
			var newFilename = extensionIndex == -1 ? filename : filename.Substring(0, extensionIndex);
			newFilename = $"{newFilename}_{viewModel.Seed}_{Flags.EncodeFlagsText(viewModel.Flags)}.nes";

			Response.StatusCode = 200;
			Response.ContentLength = rom.TotalLength;
			Response.ContentType = "application/octet-stream";
			Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{newFilename}\"");

			await rom.SaveAsync(Response.Body);
			Response.Body.Close();

			return new EmptyResult();
		}

		public IActionResult Error()
		{
			return View();
		}
	}
}
