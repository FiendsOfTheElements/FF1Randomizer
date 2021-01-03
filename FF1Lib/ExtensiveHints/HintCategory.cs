using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public enum HintCategory
	{
		LooseItemFloor, //Loose in MarshTop
		LooseItemName, //Lute in Earth
		IncentiveItemName, //Lute in Earth
		FloorHint, //Earth1 is a Dud
		EquipmentFloor, //Legendary Weapon in Earth2
		EquipmentName //Masamune in Sky
	}

	public enum HintCategoryOrder
	{
		[Description("0 - First")]
		HintCategoryOrder00 = 0,
		[Description("1")]
		HintCategoryOrder01,
		[Description("2")]
		HintCategoryOrder02,
		[Description("3")]
		HintCategoryOrder03,
		[Description("4")]
		HintCategoryOrder04,
		[Description("5")]
		HintCategoryOrder05,
		[Description("6")]
		HintCategoryOrder06,
		[Description("7")]
		HintCategoryOrder07,
		[Description("8")]
		HintCategoryOrder08,
		[Description("9")]
		HintCategoryOrder09,
		[Description("10")]
		HintCategoryOrder10,
		[Description("11")]
		HintCategoryOrder11,
		[Description("12")]
		HintCategoryOrder12,
		[Description("13")]
		HintCategoryOrder13,
		[Description("14")]
		HintCategoryOrder14,
		[Description("15 - Last")]
		HintCategoryOrder15,
	}

	public enum HintCategoryCoverage
	{
		[Description("0 - None")]
		HintCategoryCoverage00 = 0,
		[Description("1")]
		HintCategoryCoverage01,
		[Description("2")]
		HintCategoryCoverage02,
		[Description("3")]
		HintCategoryCoverage03,
		[Description("4")]
		HintCategoryCoverage04,
		[Description("5")]
		HintCategoryCoverage05,
		[Description("6")]
		HintCategoryCoverage06,
		[Description("7")]
		HintCategoryCoverage07,
		[Description("8")]
		HintCategoryCoverage08,
		[Description("9")]
		HintCategoryCoverage09,
		[Description("10 - All")]
		HintCategoryCoverage10
	}
}
