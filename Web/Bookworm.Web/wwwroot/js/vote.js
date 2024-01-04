const upArrow = document.querySelector('.fas.fa-circle-up');
const downArrow = document.querySelector('.fas.fa-circle-down');

upArrow.addEventListener('click', function () {
    const commentId = this.getAttribute('data-model-id');

    var token = document.getElementById("RequestVerificationToken").value;

    const input = {
        CommentId: parseInt(commentId),
        IsUpVote: true
    };

    fetch('/ApiVote/Post', {
        method: 'POST',
        headers: {
            'content-type': 'application/json',
            'X-CSRF-TOKEN': token
        },
        body: JSON.stringify(input)
    })
        .then(response => response.json())
        .then(data => {
            const spanElement = document.querySelector('.arrowsContainer').children[1];
            spanElement.textContent = data;
        })
        .catch(error => console.log(error));
});

downArrow.addEventListener('click', function () {
    const commentId = this.getAttribute('data-model-id');

    var token = document.getElementById("RequestVerificationToken").value;

    const input = {
        CommentId: parseInt(commentId),
        IsUpVote: false
    };

    fetch('/ApiVote/Post', {
        method: 'POST',
        headers: {
            'content-type': 'application/json',
            'X-CSRF-TOKEN': token
        },
        body: JSON.stringify(input)
    })
        .then(response => response.json())
        .then(data => {
            const spanElement = document.querySelector('.arrowsContainer').children[1];
            spanElement.textContent = data;
        })
        .catch(error => console.log(error));
});