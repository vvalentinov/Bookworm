import { createToast } from '../toast.js';

const addToFavoritesBtn = document.getElementById('addToFavoritesBtn');
const bookId = addToFavoritesBtn.getAttribute('asp-bookId');

addToFavoritesBtn.addEventListener('click', () => {
    var token = document.getElementById("RequestVerificationToken").value;
    fetch(`/ApiBook/AddToFavorites?id=${bookId}`, {
        method: 'POST',
        headers: { 'X-CSRF-TOKEN': token }
    })
        .then(res => {
            if (!res.ok) {
                return res.json().then(res => {
                    throw new Error(res.error);
                });
            }

            return res.json();
        })
        .then(res => createToast('Success', res, 'bg-success'))
        .catch(err => createToast('Error', err.message, 'bg-danger'));
});