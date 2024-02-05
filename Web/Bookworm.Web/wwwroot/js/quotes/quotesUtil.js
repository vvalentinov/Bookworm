export function getCheckedRadioBtn(name) {
    return [...document.getElementsByName(name)].find(button => button.checked);
}

export function getQuoteTypeFromId(id) {
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
export function getQuoteStatusFromId(id) {
    switch (id) {
        case 'approvedQuotesRadio':
            return 'Approved';
        case 'unapprovedQuotesRadio':
            return 'Unapproved';
        default:
            return null;
    }
}
export function getQuoteSortCriteriaFromId(id) {
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
    } else {
        return 'Search in quotes...';
    }
}

export function constructUrlParams(page, isForUserQuotes) {
    const searchContent = searchQuotesInput.value;
    const quoteType = getQuoteTypeFromId(getCheckedRadioBtn('btnradio')?.id);
    const quoteStatus = getQuoteStatusFromId(getCheckedRadioBtn('quoteStatusRadio')?.id);
    const sortCriteria = getQuoteSortCriteriaFromId(getCheckedRadioBtn('sortBtnRadio').id);

    let urlParams = `sortCriteria=${sortCriteria}&content=${searchContent}&page=${page}&isForUserQuotes=${isForUserQuotes}`;
    if (quoteType) {
        urlParams += `&type=${quoteType}`;
    }
    if (quoteStatus) {
        urlParams += `&quoteStatus=${quoteStatus}`;
    }
    return urlParams;
}

export const constructUrl = (page, isForUserQuotes) => `/ApiQuote/GetQuotes?${constructUrlParams(page, isForUserQuotes)}`;