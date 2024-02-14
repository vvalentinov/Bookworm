export function filterQuotes(quotes, searchedText) {
    const quotesContainerEl = document.querySelector('.quotesContainer');
    quotesContainerEl.innerHTML = '';

    const searchInput = document.getElementById('searchQuotesInput');
    searchInput.placeholder = searchedText;

    if (quotes.length == 0) {
        const pEl = document.createElement('p');
        pEl.textContent = 'No quotes found!';
        pEl.className = 'text-center fs-2 text-white align-middle';
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