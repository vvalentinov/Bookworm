function getCheckedButton() {
    const radioButtons = document.getElementsByName('star-rating');
    for (let i = 0; i < radioButtons.length; i++) {
        if (radioButtons[i].checked) {
            return radioButtons[i];
        }
    }
};

var rateButton = document.getElementById('rateBtn');
rateButton.addEventListener('click', function () {
    const checkedInput = getCheckedButton();
    if (!checkedInput) {
        return;
    }

    const checkedInputValue = checkedInput.value;

    const bookId = this.getAttribute('data-model-id');
    var token = document.getElementById("RequestVerificationToken").value;

    console.log(token);

    const model = {
        BookId: bookId,
        Value: parseInt(checkedInputValue)
    };

    fetch('/ApiRating/Post', {
        method: 'POST',
        headers: {
            'content-type': 'application/json',
            'X-CSRF-TOKEN': token
        },
        body: JSON.stringify(model)
    })
        .then(res => {
            if (!res.ok) {
                throw new Error(res.error);
            }

            return res.json();
        })
        .then(res => {
            const bookAvgRating = document.querySelector('.bookAvgRating');
            const bookRatingsCount = document.querySelector('.bookRatingsCount');
            bookAvgRating.textContent = res.averageVote.toFixed(1);
            bookRatingsCount.textContent = res.votesCount;
        })
        .catch(err => console.log(err));
});