function validateSeed() {
	var seedInput = document.getElementById("Seed");
	if (seedInput.value.match(/^[A-Fa-f0-9]{8}$/)) {
		seedInput.parentElement.classList.remove("has-error");
	} else {
		seedInput.parentElement.classList.add("has-error");
	}
}

function validateFlags() {
	var flagsInput = document.getElementById("Flags");
	var isValid = flagsInput.value.match(/^[A-Za-z0-9!%]{10}$/);
	if (isValid) {
		flagsInput.parentElement.classList.remove("has-error");
	} else {
		flagsInput.parentElement.classList.add("has-error");
	}

	return isValid;
}

function newSeed() {
	var seed = Math.floor((0xFFFFFFFF + 1) * Math.random());
	document.getElementById("Seed").value = seed.toString(16).toUpperCase();
}

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

// ! and % are printable in FF, + and / are not.
var base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!%";

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

	var seed = document.getElementById("Seed");
	seed.oninput = validateSeed;

	var flags = document.getElementById("Flags");
	flags.oninput = function() {
		if (validateFlags()) {
			setFlags();
		}
	}
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
	var charBits;
	charBits = (checkboxBits & 0x000000FC) >>>  2;
	flagsString += base64Chars[charBits];
	charBits = (checkboxBits & 0x00000003) <<   4 | (checkboxBits & 0x0000F000) >>> 12;
	flagsString += base64Chars[charBits];
	charBits = (checkboxBits & 0x00000F00) >>>  6 | (checkboxBits & 0x00C00000) >>> 22;
	flagsString += base64Chars[charBits];
	charBits = (checkboxBits & 0x003F0000) >>> 16;
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
	var flags = document.getElementById("Flags");
	flags.value = getFlagsString();
	flags.parentElement.classList.remove("has-error");
}

function setFlags() {
	var flags = document.getElementById("Flags");
	var flagsString = flags.value;
	var checkboxBits = 0, charBits;
	charBits = base64Chars.indexOf(flagsString[0]);
	checkboxBits |= charBits << 2;
	charBits = base64Chars.indexOf(flagsString[1]);
	checkboxBits |= (charBits & 0x30) >>> 4;
	checkboxBits |= (charBits & 0x0F) << 12;
	charBits = base64Chars.indexOf(flagsString[2]);
	checkboxBits |= (charBits & 0x3C) << 6;
	checkboxBits |= (charBits & 0x03) << 22;
	charBits = base64Chars.indexOf(flagsString[3]);
	checkboxBits |= charBits << 16;
	charBits = base64Chars.indexOf(flagsString[4]);
	checkboxBits |= charBits << 26;
	charBits = base64Chars.indexOf(flagsString[5]);
	checkboxBits |= (charBits & 0x30) << 20;

	for (var i = 0; i < checkboxIds.length; i++) {
		var checkbox = document.getElementById(checkboxIds[i]);
		checkbox.checked = (checkboxBits & (1 << i)) !== 0;
	}

	var slider;
	slider = document.getElementById("Flags_PriceScaleFactor");
	slider.value = base64Chars.indexOf(flagsString[6]) / 10;
	slider = document.getElementById("Flags_EnemyScaleFactor");
	slider.value = base64Chars.indexOf(flagsString[7]) / 10;
	slider = document.getElementById("Flags_ExpMultiplier");
	slider.value = base64Chars.indexOf(flagsString[8]) / 10;
	slider = document.getElementById("Flags_ExpBonus");
	slider.value = base64Chars.indexOf(flagsString[9]) * 10;

	flags.value = getFlagsString();
}

$(document).ready(function () {
	newSeed();
	setFlagsString();
	setCallbacks();

	getPercentageCallback(document.getElementById("Flags_PriceScaleFactor"), "prices-display")();
	getPercentageCallback(document.getElementById("Flags_EnemyScaleFactor"), "enemy-stats-display")();
	expGoldBoostCallback();
});
