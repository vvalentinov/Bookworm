var connectionNotification = new signalR.HubConnectionBuilder().withUrl('/hubs/notification').build();

connectionNotification.on('notification', (message) => {
    createToast(message);
    increaseNotificationsCount();
});

function createToast(message) {
    const toastContainerDiv = document.createElement('div');
    toastContainerDiv.className = 'toast-container position-fixed top-0 end-0 p-3 fs-5';

    const toastDivEl = document.createElement('div');
    toastDivEl.id = 'liveToast';
    toastDivEl.className = 'toast';
    toastDivEl.role = 'alert';
    toastDivEl.setAttribute('aria-live', 'assertive');
    toastDivEl.setAttribute('aria-atomic', 'true');

    const toastHeaderEl = document.createElement('div');
    toastHeaderEl.className = 'toast-header bg-info';

    const toastTitle = document.createElement('strong');
    toastTitle.className = 'me-auto';
    toastTitle.textContent = 'Notification';

    const toastBtn = document.createElement('button');
    toastBtn.type = 'button';
    toastBtn.className = 'btn-close';
    toastBtn.setAttribute('data-bs-dismiss', 'toast');
    toastBtn.setAttribute('aria-label', 'Close');

    toastHeaderEl.appendChild(toastTitle);
    toastHeaderEl.appendChild(toastBtn);

    const toastBodyEl = document.createElement('div');
    toastBodyEl.className = 'toast-body';
    toastBodyEl.textContent = `${message}`;

    toastDivEl.appendChild(toastHeaderEl);
    toastDivEl.appendChild(toastBodyEl);

    toastContainerDiv.appendChild(toastDivEl);

    document.body.appendChild(toastContainerDiv);

    const toastLiveExample = document.getElementById('liveToast');
    const toastBootstrap = bootstrap.Toast.getOrCreateInstance(toastLiveExample);
    toastBootstrap.show();
}

function increaseNotificationsCount() {
    const spanEl = document.querySelector('.notificationBtn .badge');
    if (spanEl.textContent == '') {
        spanEl.textContent = 1;
    } else {
        let currCount = parseInt(spanEl.textContent);
        if (isNaN(currCount) == false) {
            spanEl.textContent = ++currCount;
        }
    }
}

function fulfilled() { }
function rejected() { }

connectionNotification.start().then(fulfilled, rejected);

document.querySelector('.notificationBtn')?.addEventListener('click', (e) => {
    e.currentTarget.querySelector('.badge').textContent = '';
});

fetch('/ApiNotification/GetUserNotificationsCount')
    .then(res => res.json())
    .then(res => {
        if (res > 0) {
            document.querySelector('.notificationBtn .badge').textContent = res;
        }
    }).catch(err => console.log(err.message));