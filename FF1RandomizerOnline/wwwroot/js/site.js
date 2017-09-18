var checkboxIds = [
	"Flags_Treasures",
	"Flags_IncentivizeIceCave",
	"Flags_IncentivizeOrdeals",
	"Flags_Shops",
	"Flags_MagicShops",
	"Flags_MagicLevels",
	"Flags_MagicPermissions",
	"Flags_Rng",
	"Flags_EnemyScripts",
	"Flags_EnemySkillsSpells",
	"Flags_EnemyStatusAttacks",
	"Flags_Ordeals",
	"Flags_EarlyRod",
	"Flags_EarlyCanoe",
	"Flags_EarlyOrdeals",
	"Flags_NoPartyShuffle",
	"Flags_SpeedHacks",
	"Flags_IdentifyTreasures",
	"Flags_Dash",
	"Flags_BuyTen",
	"Flags_HouseMPRestoration",
	"Flags_WeaponStats",
	"Flags_ChanceToRun",
	"Flags_SpellBugs",
	"Flags_EnemyStatusAttackBug"
];

var sliderIds = [
	"Flags_PriceScaleFactor",
	"Flags_EnemyScaleFactor",
	"Flags_ExpMultiplier",
	"Flags_ExpBonus"
];

function setCallbacks() {
	for (var i = 0; i < checkboxIds.length; i++) {
		var checkbox = document.getElementById(checkboxIds[i]);
		checkbox.onchange = setFlagsString;
	}

	setPercentageCallback("Flags_PriceScaleFactor", "prices-display");
	setPercentageCallback("Flags_EnemyScaleFactor", "enemy-stats-display");

	var slider;
	slider = document.getElementById("Flags_ExpMultiplier");
	slider.onchange = expGoldBoostCallback;
	slider = document.getElementById("Flags_ExpBonus");
	slider.onchange = expGoldBoostCallback;
}

function setPercentageCallback(sliderId, labelId) {
	var slider = document.getElementById(sliderId);
	slider.onchange = getPercentageCallback(slider, labelId);
}

function getPercentageCallback(slider, labelId) {
	return function() {
		var label = document.getElementById(labelId);
		label.innerHTML = Math.round((1 / slider.value) * 100) + "% - " + Math.round(slider.value * 100) + "%";
		setFlagsString();
	}
}

function expGoldBoostCallback() {
	var multiplierSlider = document.getElementById("Flags_ExpMultiplier");
	var bonusSlider = document.getElementById("Flags_ExpBonus");
	var label = document.getElementById("exp-gold-display");
	label.innerHTML = multiplierSlider.value + "x + " + bonusSlider.value;
	setFlagsString();
}

function getFlagsString() {
	var checkboxBits = 0;
	for (var i = 0; i < checkboxIds.length; i++) {
		var checkbox = document.getElementById(checkboxIds[i]);
		if (checkbox.checked) {
			checkboxBits |= 1 << i;
		}
	}

	var flagsString = "";
	// ! and % are printable in FF, + and / are not.
	var base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!%";
	var charBits;
	charBits = (checkboxBits & 0x000000FC) >>>  2;
	flagsString += base64Chars[charBits];
	charBits = (checkboxBits & 0x00000003) <<   4 | (checkboxBits & 0x0000F000) >>> 12;
	flagsString += base64Chars[charBits];
	charBits = (checkboxBits & 0x00000F00) >>>  6 | (checkboxBits & 0x00C00000) >>> 22;
	flagsString += base64Chars[charBits];
	charBits = (checkboxBits & 0x00FC0000) >>> 18;
	flagsString += base64Chars[charBits];
	charBits = (checkboxBits & 0xFC000000) >>> 26;
	flagsString += base64Chars[charBits];
	charBits = (checkboxBits & 0x03000000) >>> 20;
	flagsString += base64Chars[charBits];

	var slider;
	slider = document.getElementById("Flags_PriceScaleFactor");
	flagsString += base64Chars[slider.value * 10];
	slider = document.getElementById("Flags_EnemyScaleFactor");
	flagsString += base64Chars[slider.value * 10];
	slider = document.getElementById("Flags_ExpMultiplier");
	flagsString += base64Chars[slider.value * 10];
	slider = document.getElementById("Flags_ExpBonus");
	flagsString += base64Chars[slider.value / 10];

	return flagsString;
}

function setFlagsString() {
	document.getElementById("Flags").value = getFlagsString();
}

$(document).ready(function () {
	setFlagsString();
	setCallbacks();

	getPercentageCallback(document.getElementById("Flags_PriceScaleFactor"), "prices-display")();
	getPercentageCallback(document.getElementById("Flags_EnemyScaleFactor"), "enemy-stats-display")();
	expGoldBoostCallback();
});
