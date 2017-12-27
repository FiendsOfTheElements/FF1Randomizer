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

	var sections = flagsInput.value.split('-');

	var isValid = sections.length == 2
		&& !!sections[0].match(/^[A-Za-z0-9!%\.]/)
		&& !!sections[1].match(/[A-Za-z0-9!%]{5}$/);
	if (isValid) {
		flagsInput.parentElement.classList.remove("has-error");
	} else {
		flagsInput.parentElement.classList.add("has-error");
	}

	return isValid;
}

function newSeed() {
	var seed = Math.floor((0xFFFFFFFF + 1) * Math.random());
	var seedString = seed.toString(16).toUpperCase();

	if (seedString.length < 8) {
		seedString = Array(8 - seedString.length + 1).join("0") + seedString;
	}

	document.getElementById("Seed").value = seedString;
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
	"Flags_EarlyBridge",
	"Flags_NoPartyShuffle",
	"Flags_SpeedHacks",
	"Flags_IdentifyTreasures",
	"Flags_Dash",
	"Flags_BuyTen",
	"Flags_HouseMPRestoration",
	"Flags_WeaponStats",
	"Flags_ChanceToRun",
	"Flags_SpellBugs",
	"Flags_EnemyStatusAttackBug",
	"Flags_FunEnemyNames",
	"Flags_PaletteSwap",
	"Flags_ShuffleLeader"
];

var sliderIds = [
	"Flags_PriceScaleFactor",
	"Flags_EnemyScaleFactor",
	"Flags_ExpMultiplier",
	"Flags_ExpBonus"
];

// Our Base64 Variant uses NES printable and Filename allowable characters. No + or /.
var base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!%";

function setCallbacks() {
	for (var i = 0; i < checkboxIds.length; i++) {
		var checkbox = document.getElementById(checkboxIds[i]);
		checkbox.onchange = setFlagsString;
	}
	document.getElementById("Flags_TeamSteak").onchange = setFlagsString;
	document.getElementById("Flags_Music").onchange = setFlagsString;

	setPercentageCallback("Flags_PriceScaleFactor", "prices-display");
	setPercentageCallback("Flags_EnemyScaleFactor", "enemy-stats-display");

	var slider;
	slider = document.getElementById("Flags_ExpMultiplier");
	slider.onchange = expGoldBoostCallback;
	slider = document.getElementById("Flags_ExpBonus");
	slider.onchange = expGoldBoostCallback;

	slider = document.getElementById("Flags_ForcedPartyMembers");
	slider.onchange = forcedPartyMembersCallback;

	var seed = document.getElementById("Seed");
	seed.oninput = validateSeed;

	var flags = document.getElementById("Flags");
	flags.oninput = function() {
		if (validateFlags()) {
			setFlags();

			getPercentageCallback(document.getElementById("Flags_PriceScaleFactor"), "prices-display")();
			getPercentageCallback(document.getElementById("Flags_EnemyScaleFactor"), "enemy-stats-display")();
			expGoldBoostCallback();
			forcedPartyMembersCallback();
		}
	};

	var fileInput = document.getElementById("File");
	fileInput.onchange = function() {
		var fileLabel = document.getElementById("file-label");
		var file = fileInput.files[0];
		if (file) {
			fileLabel.innerHTML = file.name;
		}
	};
}

function setPercentageCallback(sliderId, labelId) {
	var slider = document.getElementById(sliderId);
	slider.onchange = getPercentageCallback(slider, labelId);
}

function getPercentageCallback(slider, labelId) {
	return function() {
		var label = document.getElementById(labelId);
		label.innerHTML = Math.round(100 / slider.value) + "% - " + Math.round(slider.value * 100) + "%";
		setFlagsString();
	};
}

function expGoldBoostCallback() {
	var multiplierSlider = document.getElementById("Flags_ExpMultiplier");
	var bonusSlider = document.getElementById("Flags_ExpBonus");
	var label = document.getElementById("exp-gold-display");
	label.innerHTML = multiplierSlider.value + "x + " + bonusSlider.value;
	setFlagsString();
}

function forcedPartyMembersCallback() {
	var slider = document.getElementById("Flags_ForcedPartyMembers");
	var label = document.getElementById("forced-party-members-display");
	label.innerHTML = slider.value;
	setFlagsString();
}

function getFlagsString() {
	// We put all the information into an array of bools and work it into a tight base64 string later.
	var flagBools = [];
	for (var i = 0; i < checkboxIds.length; i++) {
		var checkbox = document.getElementById(checkboxIds[i]);
		flagBools.push(!!checkbox.checked);
	}

	var select = document.getElementById("Flags_TeamSteak");
	flagBools.push(select.value === "True");

	select = document.getElementById("Flags_Music");
	if (select.value === "None") {
		flagBools.push(false);
		flagBools.push(false);
	} else if (select.value === "Standard") {
		flagBools.push(true);
		flagBools.push(false);
	} else if (select.value === "Nonsensical") {
		flagBools.push(false);
		flagBools.push(true);
	} else if (select.value === "MusicDisabled") {
		flagBools.push(true);
		flagBools.push(true);
	}

	// Put the bool array into a uint8 array. We read beyond the
	// end of flagBools here but it just gives undefineds which become 0s.
	// If the order of the bits is odd it's because we match C#'s bits.CopyTo
	var flagBytes = new Uint8Array(Math.ceil((flagBools.length + 0.0) / 8));
	for (var i = 0; i < flagBytes.length; ++i) {
		flagBytes[i] = (
			((flagBools[i * 8 + 0] ? 0x01 : 0x00) << 0) |
			((flagBools[i * 8 + 1] ? 0x01 : 0x00) << 1) |
			((flagBools[i * 8 + 2] ? 0x01 : 0x00) << 2) |
			((flagBools[i * 8 + 3] ? 0x01 : 0x00) << 3) |
			((flagBools[i * 8 + 4] ? 0x01 : 0x00) << 4) |
			((flagBools[i * 8 + 5] ? 0x01 : 0x00) << 5) |
			((flagBools[i * 8 + 6] ? 0x01 : 0x00) << 6) |
			((flagBools[i * 8 + 7] ? 0x01 : 0x00) << 7)
		);
	}

	var flagsString = btoa(String.fromCharCode.apply(null, flagBytes));
	flagsString = flagsString.replace(/\+/g, "!").replace(/\//g, "%").replace(/=/g, ".");

	var sliderString = "";
	var slider;
	slider = document.getElementById("Flags_PriceScaleFactor");
	sliderString += base64Chars[slider.value * 10];
	slider = document.getElementById("Flags_EnemyScaleFactor");
	sliderString += base64Chars[slider.value * 10];
	slider = document.getElementById("Flags_ExpMultiplier");
	sliderString += base64Chars[slider.value * 10];
	slider = document.getElementById("Flags_ExpBonus");
	sliderString += base64Chars[slider.value / 10];
	slider = document.getElementById("Flags_ForcedPartyMembers");
	sliderString += base64Chars[slider.value];

	return flagsString + '-' + sliderString;
}

function setFlagsString() {
	var flags = document.getElementById("Flags");
	flags.value = getFlagsString();
	flags.parentElement.classList.remove("has-error");
}

function setFlags() {
	var flags = document.getElementById("Flags");
	var sections = flags.value.split('-');

	// Unpack the raw base64 string into an array of booleans
	var raw = atob(sections[0].replace(/%/g, "/").replace(/!/g, "+").replace(/\./g, "="));
	var bytes = new Uint8Array(new ArrayBuffer(raw.length));
	var bits = [];
	for (i = 0; i < raw.length; i++) {
		bytes[i] = raw.charCodeAt(i);
		bits.push(!!((bytes[i] >> 0) & 0x01));
		bits.push(!!((bytes[i] >> 1) & 0x01));
		bits.push(!!((bytes[i] >> 2) & 0x01));
		bits.push(!!((bytes[i] >> 3) & 0x01));
		bits.push(!!((bytes[i] >> 4) & 0x01));
		bits.push(!!((bytes[i] >> 5) & 0x01));
		bits.push(!!((bytes[i] >> 6) & 0x01));
		bits.push(!!((bytes[i] >> 7) & 0x01));
	}

	for (var i = 0; i < checkboxIds.length; i++) {
		var checkbox = document.getElementById(checkboxIds[i]);
		checkbox.checked = bits[i];
	}
	var select = document.getElementById("Flags_TeamSteak");
	if (bits[checkboxIds.length]) {
		select.value = "True";
	} else {
		select.value = "False";
	}

	select = document.getElementById("Flags_Music");
	var musicLow = bits[checkboxIds.length + 1];
	var musicHigh = bits[checkboxIds.length + 2];
	if (musicLow && !musicHigh) {
		select.value = "Standard";
	} else if (!musicLow && musicHigh) {
		select.value = "Nonsensical";
	} else if (musicLow && musicHigh) {
		select.value = "MusicDisabled";
	} else {
		select.value = "None";
	}

	var sliders = sections[1];
	var slider;
	slider = document.getElementById("Flags_PriceScaleFactor");
	slider.value = base64Chars.indexOf(sliders[0]) / 10;
	slider = document.getElementById("Flags_EnemyScaleFactor");
	slider.value = base64Chars.indexOf(sliders[1]) / 10;
	slider = document.getElementById("Flags_ExpMultiplier");
	slider.value = base64Chars.indexOf(sliders[2]) / 10;
	slider = document.getElementById("Flags_ExpBonus");
	slider.value = base64Chars.indexOf(sliders[3]) * 10;
	slider = document.getElementById("Flags_ForcedPartyMembers");
	slider.value = base64Chars.indexOf(sliders[4]);

	flags.value = getFlagsString();
}

$(document).ready(function () {
	setCallbacks();

	setFlagsString();
	getPercentageCallback(document.getElementById("Flags_PriceScaleFactor"), "prices-display")();
	getPercentageCallback(document.getElementById("Flags_EnemyScaleFactor"), "enemy-stats-display")();
	expGoldBoostCallback();
	forcedPartyMembersCallback();
});
