function onDeleteCommentBtnClick(e) {
    const modelId = e.getAttribute('data-model-id');
    const commentIdInput = document.querySelector("input[name='deleteCommentId']");
    commentIdInput.value = modelId;
}

function onEditCommentBtnClick(e) {
    const article = e.parentElement.parentElement.parentElement;
    tinymce.get("editCommentTextarea").setContent(article.children[1].innerHTML);

    const modelId = e.getAttribute('data-model-id');
    const commentIdInput = document.querySelector("input[name='editCommentId']");
    commentIdInput.value = modelId;
};

function getSortedCommentsByDateAsc() {
    fetch('/ApiComment/GetSortedComments?criteria=CreatedOnAsc')
        .then(res => res.json())
        .then(res => updateComments(res))
        .catch(err => console.log(err));
}

function getSortedCommentsByDateDesc() {
    fetch('/ApiComment/GetSortedComments?criteria=CreatedOnDesc')
        .then(res => res.json())
        .then(res => updateComments(res))
        .catch(err => console.log(err));
}

function getSortedCommentsByNetWorthDesc() {
    fetch('/ApiComment/GetSortedComments?criteria=NetWorthDesc')
        .then(res => res.json())
        .then(res => updateComments(res))
        .catch(err => console.log(err));
}

function updateComments(model) {
    const comments = model.comments;

    const commentsContainerEl = document.querySelector('.commentsContainer');
    commentsContainerEl.innerHTML = '';

    for (var i = 0; i < comments.length; i++) {
        const currComment = comments[i];

        const articleEl = document.createElement('article');
        articleEl.className = 'card';

        const divCardHeaderEl = document.createElement('div');
        divCardHeaderEl.className = 'card-header';

        const authorSpanEl = document.createElement('span');
        authorSpanEl.textContent = `Posted by - ${currComment.userUserName}`;

        divCardHeaderEl.appendChild(authorSpanEl);

        if (model.isUserSignedIn) {
            const divCommentActionsContainerEl = document.createElement('div');
            divCommentActionsContainerEl.className = 'commentActionsContainer';

            const divArrowsContainerEl = document.createElement('div');
            divArrowsContainerEl.className = 'arrowsContainer';

            if (!currComment.isCommentOwner) {
                const onUpArrowIconEl = document.createElement('i');
                onUpArrowIconEl.onclick = () => onUpArrowClick(onUpArrowIconEl);
                onUpArrowIconEl.setAttribute('data-model-id', currComment.id);
                onUpArrowIconEl.className = `fas fa-circle-up hover ${currComment.userVoteValue == 1 ? "greenUpArrow" : ""}`;

                const commentNetWorthSpanEl = document.createElement('span');
                commentNetWorthSpanEl.textContent = `${currComment.netWorth}`;

                const onDownArrowIconEl = document.createElement('i');
                onDownArrowIconEl.onclick = () => onDownArrowClick(onDownArrowIconEl);
                onDownArrowIconEl.setAttribute('data-model-id', currComment.id);
                onDownArrowIconEl.className = `fas fa-circle-down hover ${currComment.userVoteValue == -1 ? "redDownArrow" : ""}`;

                divArrowsContainerEl.appendChild(onUpArrowIconEl);
                divArrowsContainerEl.appendChild(commentNetWorthSpanEl);
                divArrowsContainerEl.appendChild(onDownArrowIconEl);
            } else {
                const onUpArrowIconEl = document.createElement('i');
                onUpArrowIconEl.className = 'fas fa-circle-up';

                const commentNetWorthSpanEl = document.createElement('span');
                commentNetWorthSpanEl.textContent = `${currComment.netWorth}`;

                const onDownArrowIconEl = document.createElement('i');
                onDownArrowIconEl.className = 'fas fa-circle-down';

                divArrowsContainerEl.appendChild(onUpArrowIconEl);
                divArrowsContainerEl.appendChild(commentNetWorthSpanEl);
                divArrowsContainerEl.appendChild(onDownArrowIconEl);
            }

            divCommentActionsContainerEl.appendChild(divArrowsContainerEl);

            if (currComment.isCommentOwner || model.isUserAdmin) {
                const deleteCommentBtn = document.createElement('button');
                deleteCommentBtn.className = 'btn btn-lg btn-danger modalBtn';
                deleteCommentBtn.setAttribute("onclick", "onDeleteCommentBtnClick(this)");
                deleteCommentBtn.setAttribute('data-model-id', currComment.id);
                deleteCommentBtn.setAttribute('data-bs-toggle', 'modal');
                deleteCommentBtn.setAttribute('data-bs-target', '#deleteModal');
                deleteCommentBtn.textContent = 'Delete';

                const editCommentBtn = document.createElement('button');
                editCommentBtn.className = 'btn btn-lg btn-warning modalBtn';
                editCommentBtn.setAttribute("onclick", "onEditCommentBtnClick(this)");
                editCommentBtn.setAttribute('data-model-id', currComment.id);
                editCommentBtn.setAttribute('data-bs-toggle', 'modal');
                editCommentBtn.setAttribute('data-bs-target', '#editModal');
                editCommentBtn.textContent = 'Edit';

                divCommentActionsContainerEl.appendChild(deleteCommentBtn);
                divCommentActionsContainerEl.appendChild(editCommentBtn);
            }

            divCardHeaderEl.appendChild(divCommentActionsContainerEl);
        }

        const divCardBodyEl = document.createElement('div');
        divCardBodyEl.className = 'card-body';
        divCardBodyEl.innerHTML = `${currComment.sanitizedContent}`;

        articleEl.appendChild(divCardHeaderEl);
        articleEl.appendChild(divCardBodyEl);

        commentsContainerEl.appendChild(articleEl);
    }
}