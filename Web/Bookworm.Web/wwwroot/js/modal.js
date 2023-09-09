const modalBtns = document.querySelectorAll('.modalBtn');
const modelIdInputs = document.querySelectorAll('.modelIdInput');

modalBtns.forEach(button => {
    button.addEventListener('click', function () {
        const modelId = this.getAttribute('data-model-id');
        modelIdInputs.forEach(input => input.value = modelId);
    });
});