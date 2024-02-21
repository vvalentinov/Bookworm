document.getElementById('addAuthorBtn').addEventListener('click', function () {
    var form = document.getElementById("uploadBookForm");

    let authorsContainer = document.getElementById('authorsContainer');
    let authorsCount = authorsContainer.childElementCount - 1;

    //Create and append author
    let authorContainer = document.createElement('div');

    let inputGroupEl = document.createElement('div');
    inputGroupEl.className = 'input-group mt-4 animate__animated animate__zoomIn';

    let inputElement = document.createElement('input');
    inputElement.setAttribute('data-val', 'true');
    inputElement.setAttribute('data-val-length-max', '50');
    inputElement.setAttribute('data-val-length-min', '2');
    inputElement.setAttribute('data-val-length', 'Author name must be between 2 and 50 characters!');
    inputElement.setAttribute('data-val-required', 'Author\'s name is required.');
    inputElement.name = `Authors[${authorsCount}].Name`;
    inputElement.type = 'text';
    inputElement.className = 'form-control border-2 border-dark fs-5';
    inputElement.placeholder = 'Author\'s Name';

    let spanElement = document.createElement('span');
    spanElement.setAttribute('data-valmsg-for', `Authors[${authorsCount}].Name`);
    spanElement.setAttribute('data-valmsg-replace', 'true');
    spanElement.className = 'small text-danger field-validation-valid';

    let buttonElement = document.createElement('button');
    buttonElement.className = 'btn btn-danger border-2 border-dark';
    buttonElement.type = 'button';
    buttonElement.id = 'deleteAuthorBtn';
    buttonElement.textContent = 'Remove';
    buttonElement.addEventListener('click', function handleClick(event) {
        event.target.parentElement.parentElement.remove();
        if (authorsCount < 5) {
            addAuthorBtn.disabled = false;
        }
    });

    inputGroupEl.appendChild(inputElement);
    inputGroupEl.appendChild(buttonElement);

    authorContainer.appendChild(inputGroupEl);
    authorContainer.appendChild(spanElement);

    authorsContainer.appendChild(authorContainer);

    $(form).removeData("validator").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse(form);

    if (authorsCount == 4) {
        addAuthorBtn.disabled = true;
        return;
    }
});

function removeAuthor(e) {
    let authorsContainer = document.getElementById('authorsContainer');
    let authorsCount = authorsContainer.childElementCount - 2;
    e.target.parentElement.remove();

    const hiddenIdInputs = authorsContainer.querySelectorAll('input[type="hidden"]');
    hiddenIdInputs.forEach((input, index) => input.name = `Authors[${index}].Id`);
    const authorsNameInputs = authorsContainer.querySelectorAll('input[type="text"]');
    authorsNameInputs.forEach((input, index) => input.name = `Authors[${index}].Name`);

    if (authorsCount < 5) {
        const addAuthorBtn = document.getElementById('addAuthorBtn');
        addAuthorBtn.disabled = false;
    }
}