import { filterQuotes } from './filterQuotes.js';
import { updatePagination } from '../pagination.js';
import { updateQuotesTable } from './updateUserQuotesTable.js';
import { getCheckedRadioBtn, constructUrl, getSearchTextFromQuoteTypeId } from './quotesUtil.js';


export const fetchForQuotes = (page) => {
    const isForUserRecords = document.getElementById('isForUserQuotes')?.value;

    let url;
    if (page) {
        url = constructUrl(page, isForUserRecords);
    } else {
        url = constructUrl(1, isForUserRecords);
    }

    const checkedQuoteTypeBtnId = getCheckedRadioBtn('btnradio')?.id;
    const searchText = getSearchTextFromQuoteTypeId(checkedQuoteTypeBtnId);

    fetch(url)
        .then(res => res.json())
        .then(res => {
            if (isForUserRecords === "true") {
                updateQuotesTable(res.quotes);
            } else {
                filterQuotes(res.quotes, searchText);
            }
            updatePagination(res);
        }).catch(err => console.log(err));
};