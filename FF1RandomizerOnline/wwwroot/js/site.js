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
	var isValid = flagsInput.value.match(/^[A-Za-z0-9!%]{11}$/);
	if (isValid) {
		flagsInput.parentElement.classList.remove("has-error");
	} else {
		flagsInput.parentElement.classList.add("has-error");
	}

	return isValid;
}

function importSeedFlags(e) {
	e.preventDefault();

	var str = prompt("Paste in a seed and flags string as given to you by our lord and master, crim_bot. (SEED_FLAGS)");
	var seed;
	var flags;

	[seed, flags] = str.split("_", 2);

	document.getElementById("Flags").value = flags;
	document.getElementById("Seed").value = seed;

	onFlagsChanged();
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

// ! and % are printable in FF, + and / are not.
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
	flags.oninput = onFlagsChanged;

	var fileInput = document.getElementById("File");
	fileInput.onchange = function() {
		var fileLabel = document.getElementById("file-label");
		var file = fileInput.files[0];
		if (file) {
			fileLabel.innerHTML = file.name;
		}
	};
}

function onFlagsChanged() {
	if (validateFlags()) {
		setFlags();

		getPercentageCallback(document.getElementById("Flags_PriceScaleFactor"), "prices-display")();
		getPercentageCallback(document.getElementById("Flags_EnemyScaleFactor"), "enemy-stats-display")();
		expGoldBoostCallback();
		forcedPartyMembersCallback();
	}
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
	var checkboxBits = 0;
	for (var i = 0; i < checkboxIds.length; i++) {
		var checkbox = document.getElementById(checkboxIds[i]);
		if (checkbox.checked) {
			checkboxBits |= 1 << i;
		}
	}
	var select = document.getElementById("Flags_TeamSteak");
	if (select.value === "True") {
		checkboxBits |= 1 << checkboxIds.length;
	}

	select = document.getElementById("Flags_Music");
	if (select.value === "Standard") {
		checkboxBits |= 1 << (checkboxIds.length + 1);
	} else if (select.value === "Nonsensical") {
		checkboxBits |= 1 << (checkboxIds.length + 2);
	} else if (select.value === "MusicDisabled") {
		checkboxBits |= 1 << (checkboxIds.length + 1);
		checkboxBits |= 1 << (checkboxIds.length + 2);		
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
	slider = document.getElementById("Flags_ForcedPartyMembers");
	flagsString += base64Chars[slider.value];

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
	var select = document.getElementById("Flags_TeamSteak");
	if ((checkboxBits & (1 << checkboxIds.length)) !== 0) {
		select.value = "True";
	} else {
		select.value = "False";
	}

	select = document.getElementById("Flags_Music");
	var musicLow = (checkboxBits & (1 << (checkboxIds.length + 1))) !== 0;
	var musicHigh = (checkboxBits & (1 << (checkboxIds.length + 2))) !== 0;
	if (musicLow && !musicHigh) {
		select.value = "Standard";
	} else if (!musicLow && musicHigh) {
		select.value = "Nonsensical";
	} else if (musicLow && musicHigh) {
		select.value = "MusicDisabled";
	} else {
		select.value = "None";
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
	slider = document.getElementById("Flags_ForcedPartyMembers");
	slider.value = base64Chars.indexOf(flagsString[10]);

	flags.value = getFlagsString();
}

$(document).ready(function () {
	document
		.getElementById('cmd-import')
		.addEventListener('click', importSeedFlags);

	setCallbacks();

	setFlagsString();
	getPercentageCallback(document.getElementById("Flags_PriceScaleFactor"), "prices-display")();
	getPercentageCallback(document.getElementById("Flags_EnemyScaleFactor"), "enemy-stats-display")();
	expGoldBoostCallback();
	forcedPartyMembersCallback();
});
