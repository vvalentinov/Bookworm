
//Filter Quotes By Type Buttons
const quoteTypeButtons = [
    document.getElementById('movie-quotes'),
    document.getElementById('book-quotes'),
    document.getElementById('general-quotes'),
    document.getElementById('liked-quotes')
];

quoteTypeButtons.forEach(button => {
    if (button) {
        button.addEventListener('click', function () {
            const searchText = getSearchTextFromQuoteTypeId(button.id);
            const url = constructUrl(1);
            fetchForQuotes(url, searchText);
        })
    }
})

function updatePagination(model, searchText) {
    const navigation = document.getElementById('quotesPagination');
    navigation.children[0]?.remove();

    if (model.pagesCount > 1) {


        const ulElement = document.createElement('ul');
        ulElement.className = 'pagination justify-content-center';

        const previousLiEl = document.createElement('li');
        let previousLiElClass;
        if (!model.hasPreviousPage) {
            previousLiElClass = 'page-item disabled';
        } else {
            previousLiElClass = 'page-item';
            previousLiEl.style.cursor = 'pointer';
        }
        previousLiEl.className = previousLiElClass;
        const previousLiAnchorEl = document.createElement('a');
        previousLiAnchorEl.className = 'page-link';
        previousLiAnchorEl.textContent = 'Previous';
        if (model.hasPreviousPage) {
            previousLiAnchorEl.addEventListener('click', () => fetchForQuotes(constructUrl(model.previousPageNumber), searchText));
        }
        previousLiEl.appendChild(previousLiAnchorEl);
        ulElement.appendChild(previousLiEl);

        for (var i = model.pageNumber - 4; i < model.pageNumber; i++) {
            if (i > 0) {
                let page = i;
                const liElement = document.createElement('li');
                liElement.className = 'page-item';
                liElement.style.cursor = 'pointer';

                const anchorEl = document.createElement('a');
                anchorEl.className = 'page-link';
                anchorEl.textContent = `${i}`;
                anchorEl.addEventListener('click', function () {
                    fetchForQuotes(constructUrl(page), searchText);
                });

                liElement.appendChild(anchorEl);
                ulElement.appendChild(liElement);
            }
        }

        const currPageLiEl = document.createElement('li');
        currPageLiEl.className = 'page-item active';
        currPageLiEl.setAttribute('aria-current', 'page');

        const currPageSpanEl = document.createElement('span');
        currPageSpanEl.className = 'page-link';
        currPageSpanEl.textContent = `${model.pageNumber}`;

        const currPageInnerSpanEl = document.createElement('span');
        currPageInnerSpanEl.className = 'sr-only';
        currPageInnerSpanEl.textContent = '(current)';

        currPageSpanEl.appendChild(currPageInnerSpanEl);
        currPageLiEl.appendChild(currPageSpanEl);
        ulElement.appendChild(currPageLiEl);

        for (var i = model.pageNumber + 1; i <= model.pageNumber + 4; i++) {
            if (i <= model.pagesCount) {
                let page = i;
                const liElement = document.createElement('li');
                liElement.className = 'page-item';
                liElement.style.cursor = 'pointer';

                const anchorElement = document.createElement('a');
                anchorElement.className = 'page-link';
                anchorElement.textContent = `${i}`;
                anchorElement.addEventListener('click', () => fetchForQuotes(constructUrl(page), searchText));

                liElement.appendChild(anchorElement);
                ulElement.appendChild(liElement);
            }
        }

        const nextPageLiEl = document.createElement('li');
        let nextPageLiClassName;
        if (model.hasNextPage == false) {
            nextPageLiClassName = 'page-item disabled';
        } else {
            nextPageLiClassName = 'page-item';
            nextPageLiEl.style.cursor = 'pointer';
        }
        nextPageLiEl.className = nextPageLiClassName;

        const nextPageAnchorEl = document.createElement('a');
        nextPageAnchorEl.className = 'page-link';
        nextPageAnchorEl.textContent = 'Next';
        if (model.hasNextPage) {
            nextPageAnchorEl.addEventListener('click', () => fetchForQuotes(constructUrl(model.nextPageNumber), searchText));
        }
        nextPageLiEl.appendChild(nextPageAnchorEl);
        ulElement.appendChild(nextPageLiEl);
        navigation.appendChild(ulElement);
    }
}

const fetchForQuotes = (url, searchText) => {
    fetch(url)
        .then(res => res.json())
        .then(res => {
            filterQuotes(res.quotes, searchText);
            updatePagination(res, searchText);
        }).catch(err => console.log(err));
};

const sortRadioButtons = [
    document.getElementById('newest-to-oldest'),
    document.getElementById('oldest-to-newest'),
    document.getElementById('likes-count-desc')
];

sortRadioButtons.forEach(button => {
    if (button) {
        button.addEventListener('click', function () {
            const checkedQuoteTypeRadio = getCheckedRadio('btnradio');
            const quoteType = getQuoteTypeFromId(checkedQuoteTypeRadio?.id);
            const content = searchQuotesInput.value;
            const sortCriteria = getQuoteSortCriteriaFromId(button.id);

            let url = `/ApiQuote/GetQuotes?sortCriteria=${sortCriteria}&content=${content}`;
            let searchText = 'Search quotes...';

            if (quoteType) {
                if (quoteType == 'LikedQuote') {
                    searchText = 'Search in liked quotes...';
                    url = `/ApiQuote/GetLikedQuotes?&sortCriteria=${sortCriteria}&content=${content}`;
                } else {
                    url = `/ApiQuote/GetQuotes?type=${quoteType}&sortCriteria=${sortCriteria}&content=${content}`;
                    searchText = getSearchTextFromQuoteType(quoteType);
                }
            }
            fetch(url)
                .then(res => res.json())
                .then(res => filterQuotes(res, searchText))
                .catch(err => console.log(err));
        });
    }
});
function filterQuotes(quotes, searchedText) {
    const quotesContainerEl = document.querySelector('.quotesContainer');
    quotesContainerEl.innerHTML = '';

    const searchInput = document.getElementById('searchQuotesInput');
    searchInput.placeholder = searchedText;

    if (quotes.length == 0) {
        const pEl = document.createElement('p');
        pEl.textContent = 'No quotes found!';
        pEl.className = 'text-center fs-3';
        quotesContainerEl.appendChild(pEl);
    } else {
        quotes.forEach(quote => {
            const cardDivEl = document.createElement('div');
            cardDivEl.className = 'card';

            const cardHeaderEl = document.createElement('div');
            cardHeaderEl.className = 'card-header';

            const spanEl = document.createElement('span');
            const iconEl = document.createElement('i');
            iconEl.className = `${quote.isLikedByUser ? 'fa-solid' : 'fa-regular'} fa-thumbs-up ${quote.isUserQuoteCreator == false ? 'likeQuote' : ''}`;
            iconEl.setAttribute('onclick', `likeQuote(this, ${quote.id}, ${quote.isUserQuoteCreator})`);
            spanEl.appendChild(iconEl);
            const innerSpanEl = document.createElement('span');
            innerSpanEl.textContent = `(${quote.likes})`;
            spanEl.appendChild(innerSpanEl);

            cardHeaderEl.textContent = 'Quote';
            cardHeaderEl.appendChild(spanEl);

            const cardBodyEl = document.createElement('div');
            cardBodyEl.className = 'card-body';
            const blockquoteEl = document.createElement('blockquote');
            blockquoteEl.className = 'blockquote';
            const paragraphEl = document.createElement('p');
            paragraphEl.textContent = `${quote.content}`;
            blockquoteEl.appendChild(paragraphEl);
            if (quote.authorName) {
                const authorFooterEl = document.createElement('footer');
                authorFooterEl.className = 'blockquote-footer text-end';
                authorFooterEl.textContent = `${quote.authorName}`;
                blockquoteEl.appendChild(authorFooterEl);
            }
            if (quote.bookTitle) {
                const bookFooterEl = document.createElement('footer');
                bookFooterEl.className = 'blockquote-footer text-end';
                bookFooterEl.textContent = `From the book: ${quote.bookTitle}`;
                blockquoteEl.appendChild(bookFooterEl);
            }
            if (quote.movieTitle) {
                const movieFooterEl = document.createElement('footer');
                movieFooterEl.className = 'blockquote-footer text-end';
                movieFooterEl.textContent = `From the movie: ${quote.movieTitle}`;
                blockquoteEl.appendChild(movieFooterEl);
            }
            cardBodyEl.appendChild(blockquoteEl);
            cardDivEl.appendChild(cardHeaderEl);
            cardDivEl.appendChild(cardBodyEl);
            quotesContainerEl.appendChild(cardDivEl);
        });
    }
}

const searchQuotesInput = document.getElementById('searchQuotesInput');
const searchQuotesButton = document.getElementById('searchQuotesBtn');

if (searchQuotesButton) {
    searchQuotesButton.addEventListener('click', searchQuotes);
}

if (searchQuotesInput) {
    searchQuotesInput.addEventListener('keypress', function (event) {
        if (event.key === 'Enter') {
            searchQuotes();
        }
    });
}
function searchQuotes() {
    const searchValue = searchQuotesInput.value;

    const checkedQuoteTypeRadio = getCheckedRadio('btnradio');
    const quoteType = getQuoteTypeFromId(checkedQuoteTypeRadio?.id);

    const sortCriteriaButton = getCheckedRadio('sortBtnRadio');
    const sortCriteria = getQuoteSortCriteriaFromId(sortCriteriaButton.id);

    let url = `/ApiQuote/GetQuotes?sortCriteria=${sortCriteria}&content=${searchValue}`;

    let searchText = 'Search quotes...';

    if (quoteType) {
        if (quoteType == 'LikedQuote') {
            url = `/ApiQuote/GetLikedQuotes?sortCriteria=${sortCriteria}&content=${searchValue}`;
            searchText = 'Search in liked quotes...';
        } else {
            url = `/ApiQuote/GetQuotes?type=${quoteType}&sortCriteria=${sortCriteria}&content=${searchValue}`;
            searchText = getSearchTextFromQuoteTypeId(checkedQuoteTypeRadio.id);
        }
    }

    fetch(url)
        .then(res => res.json())
        .then(res => filterQuotes(res, searchText))
        .catch(err => console.log(err.message));
}

// Helper functions
const getCheckedRadio = (name) => [...document.getElementsByName(name)].find(button => button.checked);
function getQuoteTypeFromId(id) {
    switch (id) {
        case 'movie-quotes':
            return 'MovieQuote';
        case 'book-quotes':
            return 'BookQuote';
        case 'general-quotes':
            return 'GeneralQuote';
        case 'liked-quotes':
            return 'LikedQuote';
        default:
            return null;
    }
}
function getQuoteSortCriteriaFromId(id) {
    switch (id) {
        case 'newest-to-oldest':
            return 'NewestToOldest';
        case 'oldest-to-newest':
            return 'OldestToNewest';
        case 'likes-count-desc':
            return 'LikesCountDesc';
        default:
            return null;
    }
}
function getSearchTextFromQuoteTypeId(id) {
    switch (id) {
        case 'movie-quotes':
            return 'Search in movie quotes...';
        case 'book-quotes':
            return 'Search in book quotes...';
        case 'general-quotes':
            return 'Search in general quotes...';
        default:
            return 'Search in liked quotes...';
    }
}

function constructUrlParams(page) {
    const searchContent = searchQuotesInput.value;
    const quoteType = getQuoteTypeFromId(getCheckedRadio('btnradio').id);
    const sortCriteria = getQuoteSortCriteriaFromId(getCheckedRadio('sortBtnRadio').id);

    return `type=${quoteType}&sortCriteria=${sortCriteria}&content=${searchContent}&page=${page}`;
}

const constructUrl = (page) => `/ApiQuote/GetQuotes?${constructUrlParams(page)}`;