using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FF1Lib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FF1RandomizerOnline.Models
{
	public class RandomizeViewModel
	{
		[BindRequired]
		[RegularExpression("^[A-Fa-f0-9]{8}$")]
		public string Seed { get; set; }

		[BindRequired]
		public Flags Flags { get; set; }

		[BindRequired]
		public string FlagsInput { get; set; }

		[BindRequired]
		public IFormFile File { get; set; }
	}
}
