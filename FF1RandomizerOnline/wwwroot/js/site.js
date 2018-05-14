var presets = [];
loadPresetFile("alpha-debug.json");
loadPresetFile("beginner.json");
loadPresetFile("default.json");
loadPresetFile("full-npc.json");
loadPresetFile("improved-vanilla.json");
loadPresetFile("normal-npc.json");
loadPresetFile("tournament.json");

function loadPresetFile(filename) {
	$.getJSON("/presets/" + filename, (preset) => {
		presets.push(preset);
	});
}

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
	var isValid = flagsInput.value.match(/^[A-Za-z0-9!-]{24}$/);
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
        var seed = document.getElementById("Seed").value;
        var flags = document.getElementById("Flags").value;
        var str = prompt("Press Ctrl+C to copy to clipboard or paste in a SEED_FLAGS string and click OK to save changes.", seed + "_" + flags);
    
		if (str) {
			[seed, flags] = str.split("_", 2);

			this.flagString = flags;
			document.getElementById("Seed").value = seed;
		}
    },
    preset: function(presetName) {
		let presetFlags = presets.find((preset) => preset.Name === presetName).Flags;
		for (var key in presetFlags) {
			if (this[key] !== true && this[key] !== false) {
				this[key] = presetFlags[key];
			}
			else if (this[key] && !presetFlags[key] || !this[key] && presetFlags[key]) {
				this[key] = presetFlags[key];
			}
		}
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
