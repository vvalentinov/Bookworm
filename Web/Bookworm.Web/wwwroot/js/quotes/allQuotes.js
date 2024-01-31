
const movieQuotesBtn = document.querySelector('#movieQuotesRadio');
const bookQuotesBtn = document.querySelector('#bookQuotesRadio');
const generalQuotesBtn = document.querySelector('#generalQuotesRadio');
const likedQuotesBtn = document.querySelector('#likedQuotesRadio');

movieQuotesBtn.addEventListener('click', function () {
    fetch('/ApiQuote/GetAllQuotesByType?type=MovieQuote')
        .then(res => res.json())
        .then(res => filterQuotes(res, 'Search in movie quotes...'))
        .catch(err => console.log(err))
});

bookQuotesBtn.addEventListener('click', function () {
    fetch('/ApiQuote/GetAllQuotesByType?type=BookQuote')
        .then(res => res.json())
        .then(res => filterQuotes(res, 'Search in book quotes...'))
        .catch(err => console.log(err))
});

generalQuotesBtn.addEventListener('click', function () {
    fetch('/ApiQuote/GetAllQuotesByType?type=GeneralQuote')
        .then(res => res.json())
        .then(res => filterQuotes(res, 'Search in general quotes...'))
        .catch(err => console.log(err))
});

likedQuotesBtn.addEventListener('click', function () {
    fetch('/ApiQuote/GetLikedQuotes')
        .then(res => res.json())
        .then(res => filterQuotes(res, 'Search in liked quotes...'))
        .catch(err => console.log(err))
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

searchQuotesButton.addEventListener('click', searchQuotes);
searchQuotesInput.addEventListener('keypress', function (event) {
    if (event.key === 'Enter') {
        searchQuotes();
    }
});

function searchQuotes() {
    let searchValue = searchQuotesInput.value;
    if (searchValue == '') {
        return;
    }

    let checkedRadio = getCheckedRadio();

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
                // TODO
                break;
        }
    } else {
        fetch(`/ApiQuote/SearchAllQuotesByContent?content=${searchValue}`)
            .then(res => res.json())
            .then(res => filterQuotes(res, searchValue))
            .catch(err => console.log(err));
    }
}

function getCheckedRadio() {
    const radioButtons = document.getElementsByName('btnradio');
    for (let i = 0; i < radioButtons.length; i++) {
        if (radioButtons[i].checked) {
            return radioButtons[i];
        }
    }
};