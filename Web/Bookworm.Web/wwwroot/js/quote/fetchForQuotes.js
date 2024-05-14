import { filterQuotes } from './filterQuotes.js';
import { updatePagination } from '../pagination.js';
import { updateQuotesTable } from './updateUserQuotesTable.js';
import { getCheckedRadioBtn, constructUrl, getSearchTextFromQuoteTypeId } from './quotesUtil.js';

export const fetchForQuotes = (page) => {
    let isForUserRecords = document.getElementById('isForUserQuotes')?.value;

    const url = constructUrl(page ? page : 1, isForUserRecords ? true : false);

    const searchText = getSearchTextFromQuoteTypeId(getCheckedRadioBtn('btnradio')?.id);

    fetch(url)
        .then(res => res.json())
        .then(res => {
            isForUserRecords === 'true' ?
                updateQuotesTable(res.quotes, res.pageNumber, searchText) :
                filterQuotes(res.quotes, searchText);

            updatePagination(res, fetchForQuotes);
        }).catch(err => console.log(err));
};