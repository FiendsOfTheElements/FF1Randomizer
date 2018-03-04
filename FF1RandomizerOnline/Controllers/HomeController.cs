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
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Randomize()
		{
			var vm = new RandomizeViewModel {
				File = null,
				Seed = Blob.Random(4).ToHex(),
				Flags = new Flags {
					Treasures = true,
					NPCItems = true,

					IncentivizeKingConeria = true,
					IncentivizePrincess = true,
					IncentivizeMatoya = false,
					IncentivizeBikke = true,
					IncentivizeElfPrince = false,
					IncentivizeAstos = true,
					IncentivizeNerrick = false,
					IncentivizeSmith = true,
					IncentivizeSarda = true,
					IncentivizeCanoeSage = true,
					IncentivizeCubeBot = true,
					IncentivizeFairy = false,
					IncentivizeLefein = true,
					IncentivizeMarsh = true,
					IncentivizeVolcano = true,
					IncentivizeConeria = false,
					IncentivizeEarth = true,
					IncentivizeIceCave = false,
					IncentivizeOrdeals = true,
					IncentivizeSeaShrine = true,

					IncentivizeBridge = true,
					IncentivizeLute = true,
					IncentivizeShip = true,
					IncentivizeCrown = true,
					IncentivizeCrystal = false,
					IncentivizeHerb = false,
					IncentivizeKey = true,
					IncentivizeTnt = false,
					IncentivizeCanal = true,
					IncentivizeRuby = true,
					IncentivizeRod = true,
					IncentivizeCanoe = true,
					IncentivizeFloater = true,
					IncentivizeTail = true,
					IncentivizeBottle = false,
					IncentivizeOxyale = true,
					IncentivizeSlab = true,
					IncentivizeChime = true,
					IncentivizeCube = true,
					IncentivizeAdamant = true,
					IncentivizeXcalber = false,
					IncentivizeMasamune = true,
					IncentivizeRibbon = false,
					IncentivizeRibbon2 = false,
					IncentivizePowerGauntlet = true,
					IncentivizeWhiteShirt = true,
					IncentivizeBlackShirt = false,
					IncentivizeOpal = true,
					Incentivize65K = false,
					IncentivizeBad = false,

					Shops = true,
					MagicShops = false,
					MagicLevels = true,
					MagicPermissions = false,
					Rng = true,
					EnemyScripts = true,
					EnemySkillsSpells = true,
					EnemyStatusAttacks = true,

					EarlyRod = true,
					EarlyCanoe = true,
					EarlyOrdeals = true,
					EarlyBridge = true,
					NoPartyShuffle = true,
					SpeedHacks = true,
					IdentifyTreasures = true,
					Dash = true,
					BuyTen = true,

					HouseMPRestoration = true,
					WeaponStats = true,
					ChanceToRun = true,
					SpellBugs = true,
					EnemyStatusAttackBug = true,
					EnemySpellsTargetingAllies = true,

					FunEnemyNames = true,
					PaletteSwap = true,
					TeamSteak = true,
					ModernBattlefield = true,
					Music = MusicShuffle.None,

					ForcedPartyMembers = 1,
					PriceScaleFactor = 3.0,
					EnemyScaleFactor = 1.5,
					ExpMultiplier = 3.0,
					ExpBonus = 100,
					EasyMode = false,

					Ordeals = true,
					MapConeriaDwarves = true,
					MapVolcanoIceRiver = true,
					MapTitansTrove = true
				}
			};
			vm.FlagsInput = vm.Flags.GetString();

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
