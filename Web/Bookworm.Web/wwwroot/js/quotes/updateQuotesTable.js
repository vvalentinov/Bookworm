import { filterTableRows } from './filterQuotesTable.js';
import { alertMessage } from './quoteAlertMessage.js';

// Function to update quotes table with new data
export function updateQuotesTable(data) {
    let tableElement = document.getElementsByClassName('table-responsive')[0];
    let alertMessageEl = document.getElementById('alertMessageContainer');
    if (data.length == 0) {
        tableElement.style.display = 'none';
        if (alertMessageEl == null) {
            alertMessage();
        }
    } else {
        tableElement.style.display = '';
        if (alertMessageEl != null) {
            alertMessageEl.style.display = 'none';
        }
        const quotes = data.map((obj) => obj.content);
        filterTableRows(quotes);
    }
}