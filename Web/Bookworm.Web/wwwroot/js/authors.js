function AddAuthor() {
    let authorsContainer = document.getElementById('authorsContainer');
    let addAuthorBtn = document.getElementById('addAuthorBtn');
    let authorsCount = [...authorsContainer.children].slice(2).length;

    //Create and append author
    let authorContainer = document.createElement('div');
    authorContainer.className = "input-group mt-4 animate__animated animate__zoomIn";

    let inputElement = document.createElement('input');
    inputElement.type = "text";
    inputElement.className = "form-control border border-2";
    inputElement.placeholder = "Author's Name";

    let buttonElement = document.createElement('button');
    buttonElement.className = "btn btn-outline-danger";
    buttonElement.type = "button";
    buttonElement.id = "deleteAuthorBtn";
    buttonElement.textContent = "Remove";
    buttonElement.addEventListener('click', function handleClick(event) {
        event.target.parentElement.remove();
        if (authorsCount < 5) {
            addAuthorBtn.disabled = false;
        }
    });

    authorContainer.appendChild(inputElement);
    authorContainer.appendChild(buttonElement);

    authorsContainer.appendChild(authorContainer);
    if (authorsCount == 4) {
        addAuthorBtn.disabled = true;

        let alertEl = document.getElementsByClassName('alert')[0];
        if (alertEl == undefined) {
            alert();
        }
        
        return;
    }
}


// Create and append alert message
function alert() {
    let alertMessageContainer = document.getElementById('alert-message');
    let alertMessage = document.createElement('div');

    alertMessage.className = 'alert alert-info alert-dismissible fade show animate__animated animate__pulse';
    alertMessage.role = 'alert';
    alertMessage.textContent = 'You can only add 5 authors!';

    let alertMessageButton = document.createElement('button');
    alertMessageButton.type = 'button';
    alertMessageButton.className = 'btn-close';
    alertMessageButton.setAttribute('data-bs-dismiss', 'alert');
    alertMessageButton.setAttribute('aria-label', 'Close');

    alertMessage.appendChild(alertMessageButton);
    alertMessageContainer.appendChild(alertMessage);

    setTimeout(() => {
        alertMessage.remove();
    }, 5000);
}