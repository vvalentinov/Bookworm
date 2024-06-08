if (window.location.pathname.includes('/Notifications') == false) {
    fetch('/ApiNotification/GetUserNotificationsCount')
        .then(res => res.json())
        .then(res => {
            if (res > 0) {
                document.querySelector('.notificationBtn .badge').textContent = res;
            }
        }).catch(err => console.log(err.message));
}