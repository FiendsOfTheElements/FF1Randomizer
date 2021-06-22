function handleFileSelect(inputId) {
        const input = document.getElementById(inputId);
        const file = input.files[0];
        const reader = new FileReader();

        return new Promise((resolve, reject) => {
                reader.onload = e => {
                        const encoded = e.target.result.split(',')[1];
                        resolve(encoded);
                };
                reader.onerror = () => {
                        reader.abort();
                        reject(new DOMException("Error reading file"));
                };
                reader.readAsDataURL(file);
        });
}

function handlePresetSelect(inputId) {
        const input = document.getElementById(inputId);
        const file = input.files[0];
        const reader = new FileReader();

        return new Promise((resolve, reject) => {
                reader.onload = e => {
                        document.querySelector('#presetNameInput').value = JSON.parse(e.target.result).Name;
                        resolve(e.target.result);
                        input.value = null;
                };
                reader.onerror = () => {
                        reader.abort();
                        reject(new DOMException("Error reading file"));
                        input.value = null;
                };
                reader.readAsText(file);
        });
}

async function getPresets(name) {
    var presets = await getFFRPreferences("presets");
    if (presets === null) {
        return {};
    } else {
        return JSON.parse(presets);
    }
}

async function storePreset(name, json) {
    var presets = await getPresets(name);
    presets[name] = json;
    setFFRPreferences("presets", presets);
}

async function deleteLocalPreset(preset) {
    var presets = await getPresets(name);
    delete presets[preset];
    setFFRPreferences("presets", presets);
}

async function listLocalPresets() {
    var presets = await getPresets(name);
    return Object.keys(presets);
}

async function loadLocalPreset(preset) {
    document.querySelector('#presetNameInput').value = preset;
    var presets = await getPresets(name);
    return presets[preset];
}

async function computePreset(preset) {
        const result = await fetch('presets/' + preset + '.json');
        const overrides = await result.json();

        if (preset !== 'default') {
                const defaultResult = await fetch('presets/default.json');
                const basic = await defaultResult.json();
                overrides.Flags = Object.assign(basic.Flags, overrides.Flags);
        }
        document.querySelector('#presetNameInput').value = overrides.Name;
        return JSON.stringify(overrides);
}

async function downloadFile(filename, encoded) {
        const url = "data:application/octet-stream;base64," + encoded;
        const result = await fetch(url);
        const blob = await result.blob();

        const anchor = document.createElement('a');
        anchor.download = filename;
        anchor.href = window.URL.createObjectURL(blob);
        anchor.dispatchEvent(new MouseEvent('click'));
}

function updateHistory(seedString, flagString) {
        let href = document.location.href;
        if (href.indexOf('?') > 0) {
                href = href.substr(0, href.indexOf('?'));
        }

        history.replaceState({}, '', href + '?' + 's=' + seedString + '&' + 'f=' + flagString);
}

function copyLocation() {
        const textarea = document.createElement('textarea');
        textarea.value = location.href;
        document.body.appendChild(textarea);
        textarea.select();
        document.execCommand('copy');
        document.body.removeChild(textarea);
}

function getScreenRightEdge() {
        return window.innerWidth;
}

let newWorker;
Blazor.start({}).then(() => {
        if ('serviceWorker' in navigator) {
                navigator.serviceWorker.register('/service-worker.js').then(reg => {
                        console.debug('service worker registered');
                        reg.addEventListener('updatefound', () => {
                                console.debug('New update found');
                                newWorker = reg.installing;
                                newWorker.addEventListener('statechange', () => {
                                        if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
                                                console.debug('Showing update notification');
                                                DotNet.invokeMethod('FF1Blazorizer', 'ShowUpdateNotification');
                                        }
                                });
                        });
                });

                let refreshing;
                navigator.serviceWorker.addEventListener('controllerchange', function () {
                        if (refreshing) return;
                        window.location.reload();
                        refreshing = true;
                });
        }
})


/**
 * Call from Blazor to register the new service worker
 */
function updateServiceWorkerNow() {
        if (newWorker) {
                newWorker.postMessage({ action: 'skipWaiting' });
        } else {
                window.location.reload();
        }
}

let pwa;
window.addEventListener('beforeinstallprompt', (e) => { 
        e.preventDefault();
        pwa = e;
});

function showPWAInstall() {
        pwa?.prompt();
        pwa = null;
}

async function setFFRPreferences(keyname, prefdata) {
    var iframe = document.getElementsByTagName('iframe')[0];
    var win;
    // some browser (don't remember which one) throw exception when you try to access
    // contentWindow for the first time, it work when you do that second time
    try {
        win = iframe.contentWindow;
    } catch(e) {
        win = iframe.contentWindow;
    }
    // save obj in subdomain localStorage
    win.postMessage(JSON.stringify({key: keyname, method: "set", data: prefdata}), "*");
    return Promise.resolve();
};

window.FFRPreferencesCallbacks = {};
window.onmessage = function(e) {
    var response = JSON.parse(e.data);
    if (window.FFRPreferencesCallbacks[response.key]) {
        var resolve = window.FFRPreferencesCallbacks[response.key];
        resolve(response.data);
    }
};

async function getFFRPreferences(keyname) {
    var iframe = document.getElementById('preferences-iframe');
    var win;
    // some browser (don't remember which one) throw exception when you try to access
    // contentWindow for the first time, it work when you do that second time
    try {
        win = iframe.contentWindow;
    } catch(e) {
        win = iframe.contentWindow;
    }

    return new Promise((resolve, reject) => {
        window.FFRPreferencesCallbacks[keyname] = resolve;
        win.postMessage(JSON.stringify({key: keyname, method: "get"}), "*");
    });
};
