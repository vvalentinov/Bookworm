function onUpArrowClick(upArrow) {
    const arrowContainer = upArrow.parentNode;
    const spanEl = arrowContainer.children[1];
    const downArrow = arrowContainer.children[2];

    downArrow.style.color = 'white';
    upArrow.style.color = 'green';

    const commentId = upArrow.getAttribute('data-model-id');

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
        .then(data => spanEl.textContent = data)
        .catch(error => console.log(error));
};

function onDownArrowClick(downArrow) {
    const arrowContainer = downArrow.parentNode;
    const spanEl = arrowContainer.children[1];
    const upArrow = arrowContainer.children[0];

    downArrow.style.color = 'red';
    upArrow.style.color = 'white';

    const commentId = downArrow.getAttribute('data-model-id');

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
        .then(data => spanEl.textContent = data)
        .catch(error => console.log(error));
};