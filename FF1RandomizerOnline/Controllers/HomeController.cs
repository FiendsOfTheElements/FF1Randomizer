using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FF1RandomizerOnline.Models;
using Microsoft.AspNetCore.Mvc;

namespace FF1RandomizerOnline.Controllers
{
    public class HomeController : Controller
    {
	    public IActionResult Index()
	    {
		    return View();
	    }

		public IActionResult Randomize()
	    {
		    return View(new RandomizeModel());
	    }

		public IActionResult Error()
        {
            return View();
        }
    }
}
