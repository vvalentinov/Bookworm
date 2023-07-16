let authorsCount = 1;
function AddAuthor() {
    if (authorsCount == 10) {
        let addAuthorBtn = document.getElementById('addAuthorBtn');
        addAuthorBtn.disabled = true;

        let alertMessageContainer = document.getElementById('alert-message');

        let alertMessage = document.createElement('div');
        alertMessage.className = 'alert alert-info alert-dismissible fade show animate_animated animate__bounceIn';
        alertMessage.role = 'alert';
        alertMessage.textContent = 'Sorry! You can only add 10 authors!';

        let alertMessageButton = document.createElement('button');
        alertMessageButton.type = 'button';
        alertMessageButton.className = 'btn-close';
        alertMessageButton.setAttribute('data-bs-dismiss', 'alert');
        alertMessageButton.setAttribute('aria-label', 'Close');

        alertMessage.appendChild(alertMessageButton);
        alertMessageContainer.appendChild(alertMessage);
        return;
    }
    let authorsContainer = document.getElementById('authorsContainer');

    let child = document.createElement('div');
    child.className = "input-group mt-4 animate__animated animate__zoomIn";

    let inputElement = document.createElement('input');
    inputElement.type = "text";
    inputElement.className = "form-control border border-2";
    inputElement.placeholder = "Author's Name";

    let buttonElement = document.createElement('button');
    buttonElement.className = "btn btn-outline-danger";
    buttonElement.type = "button";
    buttonElement.id = "deleteAuthorBtn";
    buttonElement.textContent = "Delete";
    buttonElement.addEventListener('click', function handleClick(event) {
        event.target.parentElement.remove();
    });

    child.appendChild(inputElement);
    child.appendChild(buttonElement);

    authorsContainer.appendChild(child);
    authorsCount++;
}

function RemoveAuthor() {
    let buttonElement = document.getElementById('deleteAuthorBtn');
    buttonElement.parentElement.remove();
}