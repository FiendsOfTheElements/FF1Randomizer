function handleFileSelect(inputId) {

	var input = document.getElementById(inputId);
	var file = input.files[0];
	var reader = new FileReader();

	return new Promise((resolve, reject) => {

		reader.onload = e => {
			let encoded = e.target.result.split(',')[1];
			resolve(encoded);
		};
		reader.onerror = () => {
			reader.abort();
			reject(new DOMException("Error reading file"));
		};

		reader.readAsDataURL(file);

	});

}

function downloadROM(filename, encoded) {
	var url = "data:application/octet-stream;base64," + encoded;
	fetch(url)
		.then(result => result.blob())
		.then(blob => {

			var anchor = document.createElement('a');

			anchor.download = filename;
			anchor.href = window.URL.createObjectURL(blob);
			anchor.dispatchEvent(new MouseEvent('click'));

		});
}
