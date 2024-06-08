import { createToast } from '../toast.js';

var connectionNotification = new signalR.HubConnectionBuilder()
    .withUrl('/hubs/notification')
    .build();

connectionNotification.on('notify', (message) => {
    createToast('Notification', message, 'bg-info');

    // Increase notifications count
    const spanEl = document.querySelector('.notificationBtn .badge');
    if (spanEl.textContent == '') {
        spanEl.textContent = 1;
    } else {
        let currCount = parseInt(spanEl.textContent);
        if (isNaN(currCount) == false) {
            spanEl.textContent = ++currCount;
        }
    }
});

function fulfilled() { }
function rejected() { }

connectionNotification.start().then(fulfilled, rejected);
