function likeQuote(icon, quoteId, isUserQuoteCreator) {
    if (isUserQuoteCreator == false) {
        const tokenInput = document.getElementById('RequestVerificationToken');
        const token = tokenInput.value;

        if (icon.classList.contains('fa-solid')) {
            fetch(`/ApiQuote/UnlikeQuote?quoteId=${quoteId}`,
                {
                    method: 'DELETE',
                    headers: {'X-CSRF-TOKEN': token}
                })
                .then(res => res.json())
                .then(res => {
                    icon.classList.replace('fa-solid', 'fa-regular');
                    const iconParentEl = icon.parentElement;
                    const span = iconParentEl.children[1];
                    span.textContent = `(${res})`;
                })
                .catch(err => console.log(err.message));
        } else {
            fetch(`/ApiQuote/LikeQuote?quoteId=${quoteId}`,
                {
                    method: 'POST',
                    headers: {'X-CSRF-TOKEN': token}
                })
                .then(res => res.json())
                .then(res => {
                    icon.classList.replace('fa-regular', 'fa-solid');
                    const iconParentEl = icon.parentElement;
                    const span = iconParentEl.children[1];
                    span.textContent = `(${res})`;
                })
                .catch(err => console.log(err.message));
        }
    }
}