function previewImages(event) {
    var files = event.target.files;
    var previewContainer = document.getElementById('imagePreview');
    previewContainer.innerHTML = '';

    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        var reader = new FileReader();

        reader.onload = function (e) {
            var imgElement = document.createElement('img');
            imgElement.src = e.target.result;
            imgElement.style.width = '150px';
            imgElement.style.height = '150px';
            imgElement.style.margin = '10px';
            previewContainer.appendChild(imgElement);
        };

        reader.readAsDataURL(file);
    }
}