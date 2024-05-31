const addAuthorButton = document.getElementById('addAuthorBtn');

const getAuthorsCount = () => document.getElementsByClassName('authorContainer').length;
$(function () { if (getAuthorsCount() == 5) { addAuthorButton.disabled = true; } })

addAuthorButton.addEventListener('click', (e) => createAndAppendAuthor(e.target));

function createAndAppendAuthor(button) {
    if (!button.disabled) {
        const authorsCount = getAuthorsCount();

        const authorsContainer = document.getElementById('authorsContainer');

        const authorContainer = document.createElement('div');
        authorContainer.className = 'authorContainer';

        const inputGroupDivElement = document.createElement('div');
        inputGroupDivElement.className = 'input-group mt-4 animate__animated animate__zoomIn';

        const inputElement = document.createElement('input');
        inputElement.type = 'text';
        inputElement.name = `Authors[${authorsCount}].Name`;
        inputElement.className = 'form-control border-2 border-dark fs-5';
        inputElement.placeholder = 'Author\'s Name';
        inputElement.setAttribute('data-val', 'true');
        inputElement.setAttribute('data-val-length', 'Author name must be between 2 and 50 characters!');
        inputElement.setAttribute('data-val-length-min', '2');
        inputElement.setAttribute('data-val-length-max', '50');
        inputElement.setAttribute('data-val-required', 'Author name is required!');

        const buttonElement = document.createElement('button');
        buttonElement.className = 'btn btn-danger border-2 border-dark';
        buttonElement.type = 'button';
        buttonElement.id = 'deleteAuthorBtn';
        buttonElement.textContent = 'Remove';
        buttonElement.addEventListener('click', (e) => onRemoveAuthorBtnClick(e));

        inputGroupDivElement.appendChild(inputElement);
        inputGroupDivElement.appendChild(buttonElement);

        const spanEl = document.createElement('span');
        spanEl.className = 'small text-danger field-validation-valid';
        spanEl.setAttribute('data-valmsg-for', `Authors[${authorsCount}].Name`);
        spanEl.setAttribute('data-valmsg-replace', 'true');

        authorContainer.appendChild(inputGroupDivElement);
        authorContainer.appendChild(spanEl);

        authorsContainer.appendChild(authorContainer);

        if (authorsCount == 4) { addAuthorBtn.disabled = true; }

        reinitializeValidation();
    }
}
function onRemoveAuthorBtnClick(event) {
    event.target.parentElement.parentElement.remove();
    addAuthorButton.disabled = false;

    const authorsNameInputs = authorsContainer.querySelectorAll('input[type="text"]');
    authorsNameInputs.forEach((input, index) => {
        input.name = `Authors[${index}].Name`;
        input.setAttribute('aria-describedby', `Authors[${index}].Name-error`);
    });

    const spanElements = authorsContainer.querySelectorAll('.small');
    spanElements.forEach((span, index) => {
        span.setAttribute('data-valmsg-for', `Authors[${index}].Name`);
        if (span.firstChild) {
            span.firstChild.id = `Authors[${index}].Name-error`;
        }
    });
}

function reinitializeValidation() {
    const form = $('#uploadBookForm');
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);
}