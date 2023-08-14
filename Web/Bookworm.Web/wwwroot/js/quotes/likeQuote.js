function likeQuoteFunc(quoteId, clickedIcon) {
    var xhr = new XMLHttpRequest();

    let antiForgeryForm = document.getElementById('antiForgeryForm');
    let token = antiForgeryForm.querySelector('input[name="__RequestVerificationToken"]').value;

    if (clickedIcon.classList.contains('fa-solid')) {
        xhr.open('POST', `/ApiQuote/DislikeQuote?quoteId=${quoteId}`, true);
        xhr.setRequestHeader("X-CSRF-TOKEN", token);

        xhr.onload = function () {
            if (this.status == 200) {
                let container = clickedIcon.closest('td');
                let likesCountSpan = container.querySelector('#likesCount');
                likesCountSpan.textContent = `(${this.response})`;
                clickedIcon.classList.replace('fa-solid', 'fa-regular');
            } else {
                alert('error');
            }
        }

        xhr.send();
    } else {
        xhr.open('POST', `/ApiQuote/LikeQuote?quoteId=${quoteId}`, true);
        xhr.setRequestHeader("X-CSRF-TOKEN", token);

        xhr.onload = function () {
            if (this.status == 200) {
                let container = clickedIcon.closest('td');
                let likesCountSpan = container.querySelector('#likesCount');
                likesCountSpan.textContent = `(${this.response})`;
                clickedIcon.classList.replace('fa-regular', 'fa-solid');
            } else {
                alert('error');
            }
        }

        xhr.send();
    }
}