function validateSeed() {
	var seedInput = document.getElementById("Seed");
    var isValid = seedInput.value.match(/^[A-Fa-f0-9]{8}$/)
	if (isValid) {
		seedInput.parentElement.classList.remove("has-error");
	} else {
		seedInput.parentElement.classList.add("has-error");
	}
    return isValid;
}

function validateFlags() {
	var flagsInput = document.getElementById("Flags");
	var isValid = flagsInput.value.match(/^[A-Za-z0-9!%]{24}$/);
	if (isValid) {
		flagsInput.parentElement.classList.remove("has-error");
	} else {
		flagsInput.parentElement.classList.add("has-error");
	}

	return isValid;
}

function importSeedFlags() {
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
    return false;
}

var checkboxIds = [
	"Flags_Treasures",
	"Flags_incentivizeIceCave",
	"Flags_incentivizeOrdeals",
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
	"Flags_ModernBattlefield"
];

// ! and % are printable in FF, + and / are not.
var base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!%";

function setFileName() {
    var fileInput = document.getElementById("File");
    var fileLabel = document.getElementById("file-label");
    var file = fileInput.files[0];
    if (file) {
        fileLabel.innerHTML = file.name;
    }
    return true;
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
/*
$(document).ready(function () {
	setCallbacks();

	setFlagsString();
	getPercentageCallback(document.getElementById("Flags_PriceScaleFactor"), "prices-display")();
	getPercentageCallback(document.getElementById("Flags_EnemyScaleFactor"), "enemy-stats-display")();
	expGoldBoostCallback();
	forcedPartyMembersCallback();
});*/

function bitProperty(flagCharIndex, flagValue) {
    var charBit = flagValue % base64Chars.length;
    return {
        get: function() { 
            if (this.flagString.length <= flagCharIndex) return false;
            var currentFlagChar = this.flagString.charAt(flagCharIndex);
            return (base64Chars.indexOf(currentFlagChar) & charBit) > 0; 
        },
        set: function() { 
            while(this.flagString.length <= flagCharIndex)
                this.flagString += base64Chars[0];
            var toggled = (base64Chars.indexOf(this.flagString.charAt(flagCharIndex))) ^ charBit;
            var newChar = base64Chars[toggled];
            this.flagString = this.flagString.substr(0,flagCharIndex) + newChar + this.flagString.substr(flagCharIndex+1);
        }
    };
}
function sixBitProperty(flagCharIndex, multiplier) {
    return {
        get: function () {
            if (this.flagString.length <= flagCharIndex) return 0;
            return base64Chars.indexOf(this.flagString[flagCharIndex]) * multiplier;
        },
        set: function(newValue) {
            while(this.flagString.length <= flagCharIndex)
                this.flagString += base64Chars[0];
            var scaledValue = (newValue / multiplier).toFixed() % base64Chars.length;
            var newChar = base64Chars[scaledValue];
            this.flagString = this.flagString.substr(0,flagCharIndex) + newChar + this.flagString.substr(flagCharIndex+1);
        }
    }
}
var app = new Vue({
  el: '#vueScope',
  data: {
    flagString: "HPBPP0vMg%%%%CUUoUAAADDPf"// document.getElementById('Flags').value
  },
  computed: {
    flagsInput: {
        get: function () {
            return this.flagString;
        },
        set: function(newValue) {
            if (!validateFlags()) return;
            this.flagString = newValue;
        }
    },
    treasures: bitProperty(0, 1),
    npcItems: bitProperty(0, 2),
    shops: bitProperty(0, 4),
    
    earlyRod: bitProperty(1, 1),
    earlyCanoe: bitProperty(1, 2),
    earlyOrdeals: bitProperty(1, 4),
    earlyBridge: bitProperty(1, 8),
    
    magicShops: bitProperty(2, 1),
    magicLevels: bitProperty(2, 2),
    magicPermissions: bitProperty(2, 4),
    
    rng: bitProperty(3, 1),
    enemyScripts: bitProperty(3, 2),
    enemySkillsSpells: bitProperty(3, 4),
    enemyStatusAttacks: bitProperty(3, 8),
    easyMode: bitProperty(3, 32),
    
    
    priceScaleFactor: sixBitProperty(14, 0.1),
    displayPriceScale: function () { 
        var slider = this.priceScaleFactor;
        return Math.round(100 / slider) + "% - " + Math.round(slider * 100) + "%"
    },
    enemyScaleFactor: sixBitProperty(15, 0.1),
    displayEnemyScale: function () { 
        var slider = this.enemyScaleFactor;
        return Math.round(100 / slider) + "% - " + Math.round(slider * 100) + "%"
    },
    expMultiplier: sixBitProperty(16, 0.1),
    expBonus: sixBitProperty(17, 10),
    forcedPartyMembers: sixBitProperty(18, 1),
    
    music: {
        get: function() {
            var values = ["Standard", "Nonsensical", "MusicDisabled", "None"];
            if (this.flagString.length <= 20) return 0;
            return values[base64Chars.indexOf(this.flagString[20])];
        },
        set: function(newValue) {
            var values = ["Standard", "Nonsensical", "MusicDisabled", "None"];
            var newIndex = values.indexOf(newValue);
            var newChar = base64Chars[newIndex];
            this.flagString = this.flagString.substr(0,flagCharIndex) + newChar + this.flagString.substr(flagCharIndex+1);
        }
    },
    identifyTreasures: bitProperty(21, 1),
    modernBattlefield: bitProperty(21, 2),
    
    // fun
    funEnemyNames: bitProperty(22, 1),
    paletteSwap: bitProperty(22, 2),
    teamSteak: bitProperty(22, 4),
    
    // speed hacks
    speedHacks: bitProperty(23, 1),
    noPartyShuffle: bitProperty(23, 2),
    dash: bitProperty(23, 4),
    buyTen: bitProperty(23, 8),
    
    // bug fixes
    houseMPRestoration: bitProperty(24, 1),
    weaponStats: bitProperty(24, 2),
    chanceToRun: bitProperty(24, 4),
    spellBugs: bitProperty(24, 8),
    enemyStatusAttackBug: bitProperty(24, 16),
    
    // map
    mapOrdeals: bitProperty(4, 1),
    mapTitansTrove: bitProperty(4, 2),
    mapConeriaDwarves: bitProperty(4, 4),
    mapVolcanoIceRiver: bitProperty(4, 8),
    
    // incentives
    incentivizeMarsh: bitProperty(5, 1),
    incentivizeConeria: bitProperty(5, 2),
    incentivizeEarth: bitProperty(5, 4),
    incentivizeIceCave: bitProperty(5, 8),
    incentivizeOrdeals: bitProperty(5, 16),
    incentivizeCrown: bitProperty(5, 32),
    incentivizeTnt: bitProperty(6, 1),
    incentivizeRuby: bitProperty(6, 2),
    incentivizeFloater: bitProperty(6, 4),
    incentivizeTail: bitProperty(6, 8),
    incentivizeSlab: bitProperty(6, 16),
    incentivizeAdamant: bitProperty(6, 32),
    incentivizeSeaShrine: bitProperty(7, 1),
    incentivizeVolcano: bitProperty(7, 2),
    incentivizeMasamune: bitProperty(7, 4),
    incentivizeRibbon: bitProperty(7, 8),
    incentivizeRibbon2: bitProperty(7, 16),
    incentivizePowerGauntlet: bitProperty(7, 32),
    incentivizeWhiteShirt: bitProperty(8, 1),
    incentivizeBlackShirt: bitProperty(8, 2),
    incentivizeOpal: bitProperty(8, 4),
    incentivize65K: bitProperty(8, 8),
    incentivizeBad: bitProperty(8, 16),
    incentivizeKingConeria: bitProperty(8, 32),
    incentivizePrincess: bitProperty(9, 1),
    incentivizeBikke: bitProperty(9, 2),
    incentivizeAstos: bitProperty(9, 4),
    incentivizeMatoya: bitProperty(9, 8),
    incentivizeElfPrince: bitProperty(9, 16),
    incentivizeNerrick: bitProperty(9, 32),
    incentivizeSarda: bitProperty(10, 1),
    incentivizeCanoeSage: bitProperty(10, 2),
    incentivizeFairy: bitProperty(10, 4),
    incentivizeLefein: bitProperty(10, 8),
    incentivizeCubeBot: bitProperty(10, 16),
    incentivizeSmith: bitProperty(10, 32),
    incentivizeBridge: bitProperty(11, 1),
    incentivizeLute: bitProperty(11, 2),
    incentivizeShip: bitProperty(11, 4),
    incentivizeCrystal: bitProperty(11, 8),
    incentivizeHerb: bitProperty(11, 16),
    incentivizeKey: bitProperty(11, 32),
    incentivizeCanal: bitProperty(12, 1),
    incentivizeRod: bitProperty(12, 2),
    incentivizeCanoe: bitProperty(12, 4),
    incentivizeOxyale: bitProperty(12, 8),
    incentivizeChime: bitProperty(12, 16),
    incentivizeCube: bitProperty(12, 32),
    incentivizeXcalber: bitProperty(13, 1),
    incentivizeBottle: bitProperty(13, 2)
  }
});