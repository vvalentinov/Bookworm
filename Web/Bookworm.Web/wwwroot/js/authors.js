const addAuthorButton = document.getElementById('addAuthorBtn');

const getAuthorsCount = () => document.getElementById('authorsContainer').children.length - 1;

$(function () {
    if (getAuthorsCount() == 5) { addAuthorButton.disabled = true; }
})

addAuthorButton.addEventListener('click', (e) => onClickAddAuthorBtn(e.target));
function onClickAddAuthorBtn(addAuthorBtn) {
    if (!addAuthorBtn.disabled) {
        createAndAppendAuthor();
    }
}
function createAndAppendAuthor() {
    const authorsCount = getAuthorsCount();

    const inputGroupDivElement = document.createElement('div');
    inputGroupDivElement.className = 'input-group mt-4 animate__animated animate__zoomIn';

    const inputElement = document.createElement('input');
    inputElement.name = `Authors[${authorsCount}].Name`;
    inputElement.type = 'text';
    inputElement.className = 'form-control border-2 border-dark fs-5';
    inputElement.placeholder = 'Author\'s Name';

    const buttonElement = document.createElement('button');
    buttonElement.className = 'btn btn-danger border-2 border-dark';
    buttonElement.type = 'button';
    buttonElement.id = 'deleteAuthorBtn';
    buttonElement.textContent = 'Remove';
    buttonElement.addEventListener('click', (e) => onRemoveAuthorBtnClick(e));

    inputGroupDivElement.appendChild(inputElement);
    inputGroupDivElement.appendChild(buttonElement);

    authorsContainer.appendChild(inputGroupDivElement);

    if (authorsCount == 4) { addAuthorBtn.disabled = true; }
}
function onRemoveAuthorBtnClick(event) {
    event.target.parentElement.remove();
    addAuthorButton.disabled = false;

    const hiddenIdInputs = authorsContainer.querySelectorAll('input[type="hidden"]');
    hiddenIdInputs.forEach((input, index) => input.name = `Authors[${index}].Id`);
    const authorsNameInputs = authorsContainer.querySelectorAll('input[type="text"]');
    authorsNameInputs.forEach((input, index) => input.name = `Authors[${index}].Name`);
}