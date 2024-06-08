export function updateQuotesTable(quotes, pageNumber, searchedText) {
    const tableElementContainer = document.getElementsByClassName('tableContainer')[0];
    tableElementContainer.children[0]?.remove();

    const searchInput = document.getElementById('searchQuotesInput');
    searchInput.placeholder = searchedText;

    if (quotes.length == 0) {
        const pEl = document.createElement('p');
        pEl.textContent = 'No quotes found!';
        pEl.className = 'text-center fs-2 text-black align-middle';
        tableElementContainer.appendChild(pEl);
    } else {
        const tableElement = document.createElement('table');
        tableElement.className = 'table table-hover table-dark table-bordered table-responsive fs-5';

        const tableHeadEl = document.createElement('thead');
        const tableHeadRowEl = document.createElement('tr');
        const columnNames = ['#', 'Content', 'Author', 'Movie', 'Book', 'Likes', 'Approved', 'Edit', 'Delete'];
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

            const currNumber = (pageNumber - 1) * 6 + i + 1;
            const numberTdEl = createElement('td', 'text-center align-middle', `${currNumber}`);
            trEl.appendChild(numberTdEl);

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

            const deleteBtnEl = document.createElement('button');
            deleteBtnEl.className = 'btn btn-outline-danger modalBtn';
            deleteBtnEl.setAttribute('onclick', `onDeleteBtnClick(${currQuote.id})`);
            deleteBtnEl.setAttribute('data-model-id', currQuote.id);
            deleteBtnEl.setAttribute('data-bs-toggle', 'modal');
            deleteBtnEl.setAttribute('data-bs-target', '#deleteQuoteModal');

            const deleteIconEl = document.createElement('i');
            deleteIconEl.className = 'fa-solid fa-trash-can fa-xl';

            deleteBtnEl.appendChild(deleteIconEl);
            deleteTdEl.appendChild(deleteBtnEl);
            trEl.appendChild(deleteTdEl);
            tBodyEl.appendChild(trEl);
        }

        tableElement.appendChild(tBodyEl);

        tableElementContainer.appendChild(tableElement);
    }
}

function createElement(type, className, content) {
    const element = document.createElement(type);
    element.className = className;
    element.textContent = content;
    return element;
}