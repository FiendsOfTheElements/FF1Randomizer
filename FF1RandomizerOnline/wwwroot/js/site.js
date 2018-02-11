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
	var isValid = flagsInput.value.match(/^[A-Za-z0-9!%]{21}$/);
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

function setFileName() {
    var fileInput = document.getElementById("File");
    var fileLabel = document.getElementById("file-label");
    var file = fileInput.files[0];
    if (file) {
        fileLabel.innerHTML = file.name;
    }
    return true;
}

// ! and % are printable in FF, + and / are not.
var base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!%";

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
function dropdownProperty(flagCharIndex, dropdownValues) {
    return {
        get: function() {
            if (this.flagString.length <= flagCharIndex) return dropdownValues[0];
            return dropdownValues[base64Chars.indexOf(this.flagString[flagCharIndex]) % dropdownValues.length];
        },
        set: function(newValue) {
            while(this.flagString.length <= flagCharIndex)
                this.flagString += base64Chars[0];
            var startingIndex = base64Chars.indexOf(this.flagString[flagCharIndex]);
            startingIndex = startingIndex - (startingIndex % dropdownValues.length);
            var newIndex = dropdownValues.indexOf(newValue);
            var newChar = base64Chars[newIndex + startingIndex];
            this.flagString = this.flagString.substr(0,flagCharIndex) + newChar + this.flagString.substr(flagCharIndex+1);
        }
    }
}
var app = new Vue({
  el: '#vueScope',
  data: {
    flagString: "HPBPPSf%%7%%!A%fUUUNB"// document.getElementById('Flags').value
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
    
    // map
    mapOrdeals: bitProperty(4, 1),
    mapTitansTrove: bitProperty(4, 2),
    mapConeriaDwarves: bitProperty(4, 4),
    mapVolcanoIceRiver: bitProperty(4, 8),
    
    priceScaleFactor: sixBitProperty(16, 0.1),
    enemyScaleFactor: sixBitProperty(17, 0.1),
    expMultiplier: sixBitProperty(18, 0.1),
    expBonus: sixBitProperty(19, 10),
    forcedPartyMembers: sixBitProperty(20, 1),
    
    // conveniences
    speedHacks: bitProperty(14, 1),
    noPartyShuffle: bitProperty(14, 2),
    dash: bitProperty(14, 4),
    buyTen: bitProperty(14, 8),
    identifyTreasures: bitProperty(14, 16),
    modernBattlefield: bitProperty(14, 32),
    
    // bug fixes
    houseMPRestoration: bitProperty(15, 1),
    weaponStats: bitProperty(15, 2),
    chanceToRun: bitProperty(15, 4),
    spellBugs: bitProperty(15, 8),
    enemyStatusAttackBug: bitProperty(15, 16),
    
    // incentives
    incentivizeMarsh: bitProperty(5, 1),
    incentivizeEarth: bitProperty(5, 2),
    incentivizeVolcano: bitProperty(5, 4),
    incentivizeIceCave: bitProperty(5, 8),
    incentivizeOrdeals: bitProperty(5, 16),
    incentivizeSeaShrine: bitProperty(5, 32),
    
    incentivizeCrown: bitProperty(6, 1),
    incentivizeRuby: bitProperty(6, 2),
    incentivizeTnt: bitProperty(6, 4),
    incentivizeFloater: bitProperty(6, 8),
    incentivizeTail: bitProperty(6, 16),
    incentivizeSlab: bitProperty(6, 32),
    
    incentivizeKingConeria: bitProperty(7, 1),
    incentivizePrincess: bitProperty(7, 2),
    incentivizeBikke: bitProperty(7, 4),
    incentivizeAstos: bitProperty(7, 8),
    incentivizeMatoya: bitProperty(7, 16),
    incentivizeElfPrince: bitProperty(7, 32),
    
    incentivizeNerrick: bitProperty(8, 1),
    incentivizeSarda: bitProperty(8, 2),
    incentivizeCanoeSage: bitProperty(8, 4),
    incentivizeFairy: bitProperty(8, 8),
    incentivizeLefein: bitProperty(8, 16),
    incentivizeCubeBot: bitProperty(8, 32),
    
    incentivizeSmith: bitProperty(9, 1),
    //incentivizeCaravan: bitProperty(9, 2),
    incentivizeConeria: bitProperty(9, 4),
    
    incentivizeBridge: bitProperty(10, 1),
    incentivizeLute: bitProperty(10, 2),
    incentivizeShip: bitProperty(10, 4),
    incentivizeCrystal: bitProperty(10, 8),
    incentivizeHerb: bitProperty(10, 16),
    incentivizeKey: bitProperty(10, 32),
    
    incentivizeCanal: bitProperty(11, 1),
    incentivizeRod: bitProperty(11, 2),
    incentivizeCanoe: bitProperty(11, 4),
    incentivizeOxyale: bitProperty(11, 8),
    incentivizeChime: bitProperty(11, 16),
    incentivizeCube: bitProperty(11, 32),
    
    incentivizeXcalber: bitProperty(12, 1),
    incentivizeBottle: bitProperty(12, 2),
    incentivizeAdamant: bitProperty(12, 4),
    incentivizeMasamune: bitProperty(12, 8),
    incentivizeRibbon: bitProperty(12, 16),
    incentivizeRibbon2: bitProperty(12, 32),
    
    incentivizePowerGauntlet: bitProperty(13, 1),
    incentivizeWhiteShirt: bitProperty(13, 2),
    incentivizeBlackShirt: bitProperty(13, 4),
    incentivizeOpal: bitProperty(13, 8),
    incentivize65K: bitProperty(13, 16),
    incentivizeBad: bitProperty(13, 32)
  }
});