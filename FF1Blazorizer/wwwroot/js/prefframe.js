window.onmessage = function(e) {
    /*if (e.origin !== "http://example.com") {
        return;
    }*/
    var payload = JSON.parse(e.data);
    switch(payload.method) {
        case 'set':
            localStorage.setItem(payload.key, JSON.stringify(payload.data));
            break;
        case 'get':
            var parent = window.parent;
            var data = localStorage.getItem(payload.key);
            parent.postMessage(JSON.stringify({key: payload.key, data: data}), "*");
            break;
        case 'remove':
            localStorage.removeItem(payload.key);
            break;
    }
};
