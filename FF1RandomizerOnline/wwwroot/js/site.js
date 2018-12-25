var presets = [];
loadPresetFile("default.json");
loadPresetFile("debug.json");
loadPresetFile("beginner.json");
loadPresetFile("full-npc.json");
loadPresetFile("improved-vanilla.json");
loadPresetFile("swiss.json");
loadPresetFile("playoff.json");

function loadPresetFile(filename) {
	$.getJSON("/presets/" + filename, (preset) => {
		presets.push(preset);
	});
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
	set: function (newValue) {
		this.flagString = newValue;
		this.flagError = !newValue.match(/^[A-Za-z0-9!-]{32,}$/);
	}
};
computedPropertyArray.SeedInput = {
	get: function () {
		return this.seedString;
	},
	set: function (newValue) {
		this.seedString = newValue;
		this.seedError = !newValue.match(/^[A-Fa-f0-9]{8}$/)
	}
};

var funKey = 'fun_flags';
var defaultFun = {
	ModernBattlefield: false,
	FunEnemyNames: false,
	PaletteSwap: false,
	TeamSteak: false,
	MusicShuffle: '0',
};

var app = new Vue({
	el: '#vueScope',
	data: {
		flagString: document.getElementById('Flags').value,
		flagError: false,
		seedString: '',
		seedError: false,
		fun: defaultFun,
		funKeyMessage: '',
		queryString: new URLSearchParams(document.location.search),
	},
	mounted: function () {
		this.$nextTick(function () {
			var obj = JSON.parse(localStorage.getItem(funKey));
			if (obj) {
				this.fun = obj;
				this.funKeyMessage = 'Your saved Fun % flags have been automatically restored.';
			}

			this.seedString = this.queryString.get('s');
			if (!this.seedString) {
				this.newSeed();
			}

			if (this.queryString.has('f')) {
				this.flagString = this.queryString.get('f');
			}
		});
	},
	watch: {
		flagString: function () {
			this.updateHistory();
		},
		seedString: function () {
			this.updateHistory();
		},
	},
	methods: {
		saveFun: function () {
			localStorage.setItem(funKey, JSON.stringify(this.fun));
			this.funKeyMessage = 'Preferences Saved.';
		},
		clearFun: function () {
			localStorage.removeItem(funKey);
			this.funKeyMessage = 'Preferences Cleared.';
		},
		updateHistory: function () {
			var href = document.location.href;
			if (href.indexOf('?') > 0) {
				href = href.substr(0, href.indexOf('?'));
			}

			this.queryString.set('s', this.seedString);
			this.queryString.set('f', this.flagString);
			history.replaceState({}, '', href + '?' + this.queryString.toString())
		},
		newSeed: function () {
			var seed = Math.floor((0xFFFFFFFF + 1) * Math.random());
			this.seedString = seed.toString(16).toUpperCase().padStart(8, '0');
		},
		importSeedFlags: function () {
			var str = prompt("Press Ctrl+C to copy to clipboard or paste in a SEED_FLAGS string and click OK to save changes.", this.seedString + "_" + this.flagString);
			if (str) {
				var [seed, flags] = str.split("_", 2);

				this.SeedInput = seed;
				this.FlagsInput = flags;
			}
		},
		preset: function (presetName) {
			let presetFlags = presets.find((preset) => preset.Name === presetName).Flags;
			for (var key in presetFlags) {
				if (key == "WarMECHMode") {
					var mode = presetFlags[key];
					this.WarMECHMode =
						mode == "Vanilla" ? 0 :
							mode == "Patrolling" ? 1 :
								mode == "Required" ? 3 : 0;
				}
				else if (this[key] !== true && this[key] !== false) {
					this[key] = presetFlags[key];
				}
				else if (this[key] && !presetFlags[key] || !this[key] && presetFlags[key]) {
					this[key] = presetFlags[key];
				}
			}
		},
		/* Debug methods as of 2.0, not very maintainable if incentive options change */
		getCountIncentivizedItems: function () {
			return this.IncentivizeMasamune + this.IncentivizeOpal + this.IncentivizeRibbon +
				this.IncentivizeDefCastArmor + this.IncentivizeOtherCastArmor +
				this.IncentivizeDefCastWeapon + this.IncentivizeOffCastWeapon +
				this.IncentivizeFetchItems * 2 + this.IncentivizeRequiredItems * 4 + this.IncentivizeNonRequiredItems +
				(this.IncentivizeRequiredItems && this.IncentivizeFreeNPCs) * 7 +
				(!this.NPCFetchItems && this.IncentivizeNonRequiredItems) * 2 +
				((!this.IncentivizeRequiredItems || this.NPCFetchItems) && IncentivizeFetchItems) * 3 +
				((!this.IncentivizeNonRequiredItems || this.NPCFetchItems) && IncentivizeFetchItems) * 2;
		},
		getCountIncentivizedLocations: function () {
			return this.IncentivizeIceCave + this.IncentivizeOrdeals + this.IncentivizeMarsh + this.IncentivizeEarth +
				this.IncentivizeVolcano + this.IncentivizeSeaShrine + this.IncentivizeSkyPalace + this.IncentivizeConeria +
				this.IncentivizeMarshKeyLocked + 7 * (this.IncentivizeFreeNPCs * this.NPCItems + this.IncentivizeFetchNPCs * this.NPCFetchItems);
		}
		/* End Debug Methods */
	},
	computed: computedPropertyArray
});
