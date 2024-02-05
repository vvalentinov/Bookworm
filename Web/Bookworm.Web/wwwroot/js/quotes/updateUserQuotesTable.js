export function updateQuotesTable(quotes) {
    const tableElementContainer = document.getElementsByClassName('table-responsive')[0];
    tableElementContainer.children[0].remove();

    const tableElement = document.createElement('table');
    tableElement.className = 'table table-bordered table-hover';

    const tableHeadEl = document.createElement('thead');
    const tableHeadRowEl = document.createElement('tr');
    const columnNames = ['Content', 'Author', 'Movie', 'Book', 'Likes', 'Is Approved', 'Edit', 'Delete'];
    columnNames.forEach(columnName => generateColumnHeader(columnName));
    function generateColumnHeader(columnName) {
        const thEl = document.createElement('th');
        thEl.className = 'text-center align-middle';
        thEl.setAttribute('scope', 'col');
        thEl.textContent = columnName;
        tableHeadRowEl.appendChild(thEl);
    }
    tableHeadEl.appendChild(tableHeadRowEl);
    tableElement.appendChild(tableHeadEl);

    const tBodyEl = document.createElement('tbody');
    tBodyEl.className = 'table-body';
    for (var i = 0; i < quotes.length; i++) {
        const currQuote = quotes[i];
        const trEl = document.createElement('tr');

        const contentTdEl = createElement('td', 'align-middle', currQuote.content);
        trEl.appendChild(contentTdEl);
        if (currQuote.authorName) {
            const authorNameTdEl = createElement('td', 'text-center align-middle', currQuote.authorName);
            trEl.appendChild(authorNameTdEl);
        } else {
            const noAuthorNameTdEl = createElement('td');
            noAuthorNameTdEl.className = 'text-center align-middle';
            const iElement = document.createElement('i');
            iElement.className = 'fa-solid fa-minus fa-xl';
            noAuthorNameTdEl.appendChild(iElement);
            trEl.appendChild(noAuthorNameTdEl);
        }
        if (currQuote.movieTitle) {
            const movieTitleTdEl = createElement('td', 'text-center align-middle', currQuote.movieTitle);
            trEl.appendChild(movieTitleTdEl);
        } else {
            const noMovieTitleTdEl = createElement('td');
            noMovieTitleTdEl.className = 'text-center align-middle';
            const iElement = document.createElement('i');
            iElement.className = 'fa-solid fa-minus fa-xl';
            noMovieTitleTdEl.appendChild(iElement);
            trEl.appendChild(noMovieTitleTdEl);
        }
        if (currQuote.bookTitle) {
            const bookTitleTdEl = createElement('td', 'text-center align-middle', currQuote.bookTitle);
            trEl.appendChild(bookTitleTdEl);
        } else {
            const noBookTitleTdEl = createElement('td');
            noBookTitleTdEl.className = 'text-center align-middle';
            const iElement = document.createElement('i');
            iElement.className = 'fa-solid fa-minus fa-xl';
            noBookTitleTdEl.appendChild(iElement);
            trEl.appendChild(noBookTitleTdEl);
        }
        const likesTdEl = document.createElement('td');
        likesTdEl.className = 'text-center align-middle';
        likesTdEl.textContent = currQuote.likes;
        trEl.appendChild(likesTdEl);

        const statusTdEl = document.createElement('td');
        statusTdEl.className = 'text-center align-middle';
        if (currQuote.isApproved) {
            const iElement = document.createElement('i');
            iElement.className = 'fa-solid fa-circle-check fa-xl checkIcon';
            statusTdEl.appendChild(iElement);
        } else {
            const iElement = document.createElement('i');
            iElement.className = 'fa-regular fa-circle-xmark fa-xl x-icon';
            statusTdEl.appendChild(iElement);
        }
        trEl.appendChild(statusTdEl);

        const editTdEl = document.createElement('td');
        editTdEl.className = 'text-center align-middle';

        const editAnchorEl = document.createElement('a');
        editAnchorEl.href = `/Quote/Edit/${currQuote.id}`;
        editAnchorEl.className = 'btn btn-outline-warning';

        const editIconEl = document.createElement('i');
        editIconEl.className = 'fa-solid fa-pen-to-square fa-xl';
        editAnchorEl.appendChild(editIconEl);
        editTdEl.appendChild(editAnchorEl);
        trEl.appendChild(editTdEl);

        const deleteTdEl = document.createElement('td');
        deleteTdEl.className = 'text-center align-middle';

        const deleteAnchorEl = document.createElement('a');
        deleteAnchorEl.className = 'btn btn-outline-danger modalBtn';
        deleteAnchorEl.setAttribute('data-model-id', currQuote.id);
        deleteAnchorEl.setAttribute('data-bs-toggle', 'modal');
        deleteAnchorEl.setAttribute('data-bs-target', '#deleteQuoteModal');

        const deleteIEl = document.createElement('i');
        deleteIEl.className = 'fa-solid fa-trash-can fa-xl';

        deleteAnchorEl.appendChild(deleteIEl);
        deleteTdEl.appendChild(deleteAnchorEl);
        trEl.appendChild(deleteTdEl);
        tBodyEl.appendChild(trEl);
    }
    tableElement.appendChild(tBodyEl);
    tableElementContainer.appendChild(tableElement);
}

function createElement(type, className, content) {
    const element = document.createElement(type);
    element.className = className;
    element.textContent = content;

    return element;
}