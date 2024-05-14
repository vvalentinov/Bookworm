export function getCheckedRadioBtn(name) {
    return [...document.getElementsByName(name)].find(button => button.checked);
}

export function getSearchTextFromQuoteTypeId(id) {
    if (id) {
        switch (id) {
            case 'movie-quotes':
                return 'Search in movie quotes...';
            case 'book-quotes':
                return 'Search in book quotes...';
            case 'general-quotes':
                return 'Search in general quotes...';
            case 'liked-quotes':
                return 'Search in liked quotes...';
        }
    }

    return 'Search in quotes...';
}

export const constructUrl = (page, isForUserQuotes) => {
    // Get Quote Sort Criteria
    let sortCriteria;
    switch (getCheckedRadioBtn('sortBtnRadio')?.id) {
        case 'newest-to-oldest':
            sortCriteria = 'NewestToOldest';
            break;
        case 'oldest-to-newest':
            sortCriteria = 'OldestToNewest';
            break;
        case 'likes-count-desc':
            sortCriteria = 'LikesCountDesc';
            break;
        default:
            sortCriteria = null;
    }

    let urlParams = `sortCriteria=${sortCriteria}
        &content=${searchQuotesInput.value}
        &page=${page}
        &isForUserQuotes=${isForUserQuotes}`;

    // Get Quote Type
    let quoteType;
    switch (getCheckedRadioBtn('btnradio')?.id) {
        case 'movie-quotes':
            quoteType = 'MovieQuote';
            break;
        case 'book-quotes':
            quoteType = 'BookQuote';
            break;
        case 'general-quotes':
            quoteType = 'GeneralQuote';
            break;
        case 'liked-quotes':
            quoteType = 'LikedQuote';
            break;
        default:
            quoteType = null;
    }

    if (quoteType) {
        urlParams += `&type=${quoteType}`;
    }

    // Get Quote Status
    let quoteStatus;
    switch (getCheckedRadioBtn('quoteStatusRadio')?.id) {
        case 'approvedQuotesRadio':
            quoteStatus = 'Approved';
            break;
        case 'unapprovedQuotesRadio':
            quoteStatus = 'Unapproved';
            break;
        default: quoteStatus = null;
    }

    if (quoteStatus) {
        urlParams += `&quoteStatus=${quoteStatus}`;
    }

    return `/ApiQuote/GetQuotes?${urlParams}`;
};