import { filterTableRows } from './filterQuotesTable.js';

// Function to update quotes table with new data
export function updateQuotesTable(data) {
    let tableElement = document.getElementsByClassName('table-responsive')[0];
    if (data.length == 0) {
        tableElement.style.display = 'none';
    } else {
        tableElement.style.display = '';
        const quotes = data.map((obj) => obj.content);
        filterTableRows(quotes);
    }
}