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
	var isValid = flagsInput.value.match(/^[A-Za-z0-9!-]{23}$/);
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

// - and ! are printable in FF, + and / are not.
var base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-!";

var computedPropertyArray = vueModelData;
computedPropertyArray.FlagsInput = {
        get: function () {
            return this.flagString;
        },
        set: function(newValue) {
            if (!validateFlags()) return;
            this.flagString = newValue;
        }
    };

var initalFlagString = document.getElementById('Flags').value;
var app = new Vue({
  el: '#vueScope',
  data: {
    flagString: initalFlagString
  },
  methods: {
    importSeedFlags: function () {
        var str = prompt("Paste in a seed and flags string as given to you by our lord and master, crim_bot. (SEED_FLAGS)");
        var seed;
        var flags;
    
        [seed, flags] = str.split("_", 2);
    
        this.flagString = flags;
        document.getElementById("Seed").value = seed;
    },
    preset: function(presetString) {
        this.flagString = presetString.length !== initalFlagString.length ? initalFlagString : presetString;
    },
    /* Debug methods as of 2.0, not very maintainable if incentive options change */
    getCountIncentivizedItems: function() {
        return this.IncentivizeMasamune + this.IncentivizeOpal + this.IncentivizeRibbon + 
            this.IncentivizeDefCastArmor + this.IncentivizeOtherCastArmor + 
            this.IncentivizeDefCastWeapon + this.IncentivizeOffCastWeapon + 
            this.IncentivizeFetchItems * 2 + this.IncentivizeRequiredItems * 4 + this.IncentivizeNonRequiredItems + 
            (this.IncentivizeRequiredItems && this.IncentivizeFreeNPCs) * 7 + 
            (!this.NPCFetchItems && this.IncentivizeNonRequiredItems) * 2 +
            ((!this.IncentivizeRequiredItems || this.NPCFetchItems) && IncentivizeFetchItems) * 3 +
            ((!this.IncentivizeNonRequiredItems || this.NPCFetchItems)  && IncentivizeFetchItems) * 2;
    },
    getCountIncentivizedLocations: function() {
        return this.IncentivizeIceCave + this.IncentivizeOrdeals + this.IncentivizeMarsh + this.IncentivizeEarth + 
            this.IncentivizeVolcano + this.IncentivizeSeaShrine + this.IncentivizeSkyPalace + this.IncentivizeConeria + 
            this.IncentivizeMarshKeyLocked + 7 * (this.IncentivizeFreeNPCs * this.NPCItems + this.IncentivizeFetchNPCs * this.NPCFetchItems);
    }
    /* End Debug Methods */
  },
  computed: computedPropertyArray
});
