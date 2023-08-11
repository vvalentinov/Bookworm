// Function to create and append alert message
export function alertMessage() {
    let mainElement = document.querySelector('main[role="main"]');

    let alertMessageContainer = document.createElement('div');
    alertMessageContainer.id = 'alertMessageContainer';
    let alertMessage = document.createElement('div');

    alertMessage.className = 'alert alert-info alert-dismissible fade show my-5 animate__animated animate__pulse';
    alertMessage.role = 'alert';
    alertMessage.textContent = 'Sorry! There were no quotes found based on your search!';

    let alertMessageButton = document.createElement('button');
    alertMessageButton.type = 'button';
    alertMessageButton.className = 'btn-close';
    alertMessageButton.setAttribute('data-bs-dismiss', 'alert');
    alertMessageButton.setAttribute('aria-label', 'Close');

    alertMessage.appendChild(alertMessageButton);
    alertMessageContainer.appendChild(alertMessage);

    setTimeout(() => {
        alertMessageContainer.remove();
    }, 5000);

    mainElement.appendChild(alertMessageContainer);
}