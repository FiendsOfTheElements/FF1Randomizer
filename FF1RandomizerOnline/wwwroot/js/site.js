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
							mode == "Wandering4F" ? 1 :
								mode == "BridgeOfDestiny" ? 3 : 0;
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
			var incentiveItemCount = 0;
			//First, handle all of the single item toggles
			incentiveItemCount += 1 * this.IncentivizeMasamune;
			incentiveItemCount += 1 * this.IncentivizeOpal;
			incentiveItemCount += 1 * this.IncentivizeRibbon;
			incentiveItemCount += 1 * this.IncentivizeDefCastArmor;
			incentiveItemCount += 1 * this.IncentivizeOtherCastArmor;
			incentiveItemCount += 1 * this.IncentivizeDefCastWeapon;
			incentiveItemCount += 1 * this.IncentivizeOffCastWeapon;
			incentiveItemCount += 1 * this.IncentivizeTail;
			
			//Lute is an incentive is Chaos rush is off and we shuffle main NPCS
			incentiveItemCount += 1 * (!this.ChaosRush && this.NPCItems);

			//Floater is an incentive if we don't have a free airship
			incentiveItemCount += 1 * !this.FreeAirship;

			//If we shuffle Fetch Quest Items, we incentivize Key, Chime and Oxyale
			incentiveItemCount += 3 * this.NPCFetchItems;

			//If we shuffle Main NPC items, we incentivize Rod, Canoe and Cube
			incentiveItemCount += 3 * this.NPCItems;

			//If we shuffle extra unrequired fetch items, we incentivize Adamant, Crystal, Herb
			incentiveItemCount += 3 * this.IncentivizeFetchItems;

			//Now to the ugly checks: First, the logic for Crown and Slab
			incentiveItemCount += 2 * (!this.NPCFetchItems || this.IncentivizeFetchItems);

			//Bottle
			incentiveItemCount += 1 * ((!this.NPCFetchItems || this.IncentivizeFetchItems) && this.NPCItems);

			//Canal
			incentiveItemCount += 1 * (this.NPCItems && this.NPCFetchItems && this.IncentivizeShipAndCanal);

			//Ship
			incentiveItemCount += 1 * (this.IncentivizeShipAndCanal && this.NPCItems);

			//TNT
			incentiveItemCount += 1 * ((!this.NPCFetchItems && !this.NPCItems) || this.IncentivizeFetchItems);

			//Ruby
			incentiveItemCount += 1 * ((!this.EarlySage && !this.NPCItems) || this.IncentivizeFetchItems);

			return incentiveItemCount;
		},
		getCountIncentivizedLocations: function () {
			return this.IncentivizeIceCave + this.IncentivizeOrdeals + this.IncentivizeMarsh + this.IncentivizeEarth + this.IncentivizeTitansTrove + 
				this.IncentivizeVolcano + this.IncentivizeSeaShrine + this.IncentivizeSkyPalace + this.IncentivizeConeria +
				this.IncentivizeMarshKeyLocked + 7 * (this.IncentivizeFreeNPCs * this.NPCItems + this.IncentivizeFetchNPCs * this.NPCFetchItems);
		}
		/* End Debug Methods */
	},
	computed: computedPropertyArray
});
