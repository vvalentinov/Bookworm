document.querySelector('.notificationBtn .badge').textContent = '';

const token = document.getElementById('RequestVerificationToken').value;

document.querySelectorAll('.alert .btn-close').forEach(btn => {
    btn?.addEventListener('click', (e) => {
        const id = e.currentTarget.getAttribute('data-model-id');

        fetch(`/ApiNotification/DeleteNotification?id=${id}`, { method: 'DELETE', headers: { 'X-CSRF-TOKEN': token } })
            .then(res => {
                return res.json().then(data => {
                    if (!res.ok) {
                        throw new Error(data.error);
                    }
                    return data;
                });
            }).catch(err => console.log(err));
    });
});

fetch('/ApiNotification/MarkUserNotificationsAsRead', { method: 'PUT', headers: { 'X-CSRF-TOKEN': token } })
    .then(res => {
        return res.json().then(data => {
            if (!res.ok) {
                throw new Error(data.error);
            }
            return data;
        });
    }).catch(err => console.log(err));