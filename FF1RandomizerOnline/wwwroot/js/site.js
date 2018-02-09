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
/*
$(document).ready(function () {
	setCallbacks();

	setFlagsString();
	getPercentageCallback(document.getElementById("Flags_PriceScaleFactor"), "prices-display")();
	getPercentageCallback(document.getElementById("Flags_EnemyScaleFactor"), "enemy-stats-display")();
	expGoldBoostCallback();
	forcedPartyMembersCallback();
});*/

function bitProperty(flagChunk, flagIndex) {
    var charBit = Math.pow(2, flagIndex);
    return {
        get: function() { return (this.flagChunks[flagChunk] & charBit) > 0; },
        set: function() { 
            Vue.set(this.flagChunks, flagChunk, this.flagChunks[flagChunk] ^ charBit); 
        }
    };
}
function chunk (arr, len) {

  var chunks = [],
      i = 0,
      n = arr.length;

  while (i < n) {
    chunks.push(arr.slice(i, i += len));
  }

  return chunks;
}
var FLAG_BITS_PER_CHAR = 6;
var FLAG_CHARS_PER_CHUNK = 8;
var FLAG_BITS_PER_CHUNK = FLAG_BITS_PER_CHAR * FLAG_CHARS_PER_CHUNK;
var index = 0;
var app = new Vue({
  el: '#vueScope',
  data: {
    flagChunks: [0]
  },
  computed: {
    flagsInput: {
        get: function () {
            var flagsString = "";
            for(var i = 0; i < this.flagChunks.length; i++)
            {
                var flagChunk = this.flagChunks[i];
                var charMask = 0x3F;
                for(var j = 0; j < FLAG_CHARS_PER_CHUNK; j++)
                {
                    var charValue = (flagChunk & charMask) >>> (j * FLAG_BITS_PER_CHAR);
                    flagsString += base64Chars[charValue];
                    charMask = charMask << FLAG_BITS_PER_CHAR;
                }
            }
            return flagsString;
        },
        set: function(newValue) {
            var newFlagChunks = chunk(newValue.split(''), FLAG_CHARS_PER_CHUNK);
            while(newFlagChunks[newFlagChunks.length - 1]
                    .filter(function(x){return x==="A"}).length === 
                    newFlagChunks[newFlagChunks.length - 1].length) 
                    newFlagChunks.pop();
            var newChunks = [];
            for(var i = 0; i < newFlagChunks.length; i++)
            {
                var flagChunk = newFlagChunks[i];
                newChunks[i] = 0;
                for(var j = 0; j < flagChunk.length; j ++)
                {
                    newChunks[i] += base64Chars.indexOf(flagChunk[j]) << (FLAG_BITS_PER_CHAR * j);
                }
            }
            this.flagChunks = newChunks;
        }
    },
    treasures: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    npcItems: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    shops: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    magicShops: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    magicLevels: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    magicPermissions: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    rng: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    enemyScripts: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    enemySkillsSpells: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    enemyStatusAttacks: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    
    earlyRod: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    earlyCanoe: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    earlyOrdeals: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    earlyBridge: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    noPartyShuffle: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    speedHacks: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    identifyTreasures: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    dash: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    buyTen: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    houseMPRestoration: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    weaponStats: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    chanceToRun: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    spellBugs: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    enemyStatusAttackBug: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    funEnemyNames: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    paletteSwap: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    modernBattleField: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    
    mapOrdeals: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    mapTitansTrove: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    mapConeriaDwarves: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    mapVolcanoIceRiver: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    
    incentivizeMarsh: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeConeria: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeEarth: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeIceCave: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeOrdeals: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeCrown: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeTnt: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeRuby: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeFloater: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeTail: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeSlab: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeAdamant: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeSeaShrine: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeVolcano: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeMasamune: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeRibbon: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeRibbon2: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizePowerGauntlet: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeWhiteShirt: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeBlackShirt: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeOpal: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivize65K: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeBad: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeKingConeria: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizePrincess: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeBikke: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeAstos: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeMatoya: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeElfPrince: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeNerrick: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeSarda: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeCanoeSage: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeFairy: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeLefein: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeCubeBot: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeSmith: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeBridge: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeLute: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeShip: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeCrystal: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeHerb: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeKey: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeCanal: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeRod: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeCanoe: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeOxyale: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeChime: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeCube: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeXcalber: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++),
    incentivizeBottle: bitProperty(Math.floor(index / FLAG_BITS_PER_CHUNK), index++)
  }
});