// Documents page: opens a PDF or image in the pop-up window (Bootstrap modal).
(function () {
    const modalElement = document.getElementById('documentModal');
    if (!modalElement) return;

    const modal = new bootstrap.Modal(modalElement);
    const title = document.getElementById('documentModalTitle');
    const downloadLink = document.getElementById('documentModalDownload');
    const frame = document.getElementById('documentFrame');
    const image = document.getElementById('documentImage');

    document.querySelectorAll('.js-view-document').forEach(function (button) {
        button.addEventListener('click', function () {
            // These come from the data-... attributes on the View button
            const url = button.dataset.url;
            const mode = button.dataset.mode;          // "pdf" or "image"

            title.textContent = button.dataset.name;
            downloadLink.href = button.dataset.downloadUrl;

            if (mode === 'pdf') {
                image.classList.add('d-none');
                image.removeAttribute('src');
                frame.src = url;                        // the browser's own PDF viewer shows it
                frame.classList.remove('d-none');
            } else {
                frame.classList.add('d-none');
                frame.removeAttribute('src');
                image.src = url;
                image.alt = button.dataset.name;
                image.classList.remove('d-none');
            }

            modal.show();
        });
    });

    // Unload the file when the window closes
    modalElement.addEventListener('hidden.bs.modal', function () {
        frame.removeAttribute('src');
        image.removeAttribute('src');
    });
})();