
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
            const sortRadioButton = getCheckedRadio('sortBtnRadio');
            const sortCriteria = getQuoteSortCriteriaFromId(sortRadioButton.id);
            const quoteType = getQuoteTypeFromId(button.id);
            const content = searchQuotesInput.value;

            if (quoteType == 'LikedQuote') {
                fetch(`/ApiQuote/GetLikedQuotes?sortCriteria=${sortCriteria}&content=${content}`)
                    .then(res => res.json())
                    .then(res => filterQuotes(res, `Search in liked quotes...`))
                    .catch(err => console.log(err));
            } else {
                fetch(`/ApiQuote/GetQuotes?type=${quoteType}&sortCriteria=${sortCriteria}&content=${content}`)
                    .then(res => res.json())
                    .then(res => filterQuotes(res, getSearchTextFromQuoteType(quoteType)))
                    .catch(err => console.log(err));
            }
        })
    }
})

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

            if (quoteType == 'LikedQuote') {
                fetch(`/ApiQuote/GetLikedQuotes?&sortCriteria=${sortCriteria}&content=${content}`)
                    .then(res => res.json())
                    .then(res => filterQuotes(res, 'Search in liked quotes...'))
                    .catch(err => console.log(err));
            } else {
                let url = `/ApiQuote/GetQuotes?sortCriteria=${sortCriteria}&content=${content}`;
                let searchText = 'Search quotes...';
                if (quoteType) {
                    url = `/ApiQuote/GetQuotes?type=${quoteType}&sortCriteria=${sortCriteria}&content=${content}`;
                    searchText = getSearchTextFromQuoteType(quoteType);
                }
                fetch(url)
                    .then(res => res.json())
                    .then(res => filterQuotes(res, searchText))
                    .catch(err => console.log(err));
            }
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
    let searchValue = searchQuotesInput.value;
    if (searchValue == '') {
        return;
    }

    let checkedRadio = getCheckedRadio('btnradio');

    if (checkedRadio != undefined) {
        switch (checkedRadio.id) {
            case 'movieQuotesRadio':
                fetch(`/ApiQuote/SearchAllQuotesByContent?content=${searchValue}&type=MovieQuote`)
                    .then(res => res.json())
                    .then(res => filterQuotes(res, searchValue))
                    .catch(err => console.log(err));
                break;
            case 'bookQuotesRadio':
                fetch(`/ApiQuote/SearchAllQuotesByContent?content=${searchValue}&type=BookQuote`)
                    .then(res => res.json())
                    .then(res => filterQuotes(res, searchValue))
                    .catch(err => console.log(err));
                break;
            case 'generalQuotesRadio':
                fetch(`/ApiQuote/SearchAllQuotesByContent?content=${searchValue}&type=GeneralQuote`)
                    .then(res => res.json())
                    .then(res => filterQuotes(res, searchValue))
                    .catch(err => console.log(err));
                break;
            case 'likedQuotesRadio':
                fetch(`/ApiQuote/SearchLikedQuotesByContent?content=${searchValue}`)
                    .then(res => res.json())
                    .then(res => filterQuotes(res, searchValue))
                    .catch(err => console.log(err));
                break;
        }
    } else {
        fetch(`/ApiQuote/SearchAllQuotesByContent?content=${searchValue}`)
            .then(res => res.json())
            .then(res => filterQuotes(res, searchValue))
            .catch(err => console.log(err));
    }
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
function getSearchTextFromQuoteType(type) {
    switch (type) {
        case 'MovieQuote':
            return 'Search in movie quotes...';
        case 'BookQuote':
            return 'Search in book quotes...';
        case 'GeneralQuote':
            return 'Search in general quotes...';
    }
}