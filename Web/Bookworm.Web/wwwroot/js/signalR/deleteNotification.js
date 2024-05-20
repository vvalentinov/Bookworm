document.querySelectorAll('.alert .btn-close').forEach(btn => {
    btn?.addEventListener('click', (e) => {
        const token = document.getElementById('RequestVerificationToken').value;
        const id = e.currentTarget.getAttribute('data-model-id');

        fetch(`/ApiNotification/DeleteNotification?id=${id}`, { method: 'DELETE', headers: { 'X-CSRF-TOKEN': token } })
            .then(res => {
                if (!res.ok) {
                    res.json().then((result) => { throw new Error(result); });
                }
            })
            .then(res => document.querySelector('.notificationBtn .badge').textContent = res)
            .catch(err => console.log(err));
    });
});