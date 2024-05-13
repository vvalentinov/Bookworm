import { fetchForQuotes } from './fetchForQuotes.js';

const quoteTypeButtons = [
    document.getElementById('movie-quotes'),
    document.getElementById('book-quotes'),
    document.getElementById('general-quotes'),
    document.getElementById('liked-quotes')
];

const sortRadioButtons = [
    document.getElementById('newest-to-oldest'),
    document.getElementById('oldest-to-newest'),
    document.getElementById('likes-count-desc')
];

const quoteStatusButtons = [
    document.getElementById('approvedQuotesRadio'),
    document.getElementById('unapprovedQuotesRadio')
];

quoteTypeButtons.forEach(btn => btn.addEventListener('click', () => fetchForQuotes()));
sortRadioButtons.forEach(btn => btn.addEventListener('click', () => fetchForQuotes()));
quoteStatusButtons.forEach(btn => btn?.addEventListener('click', () => fetchForQuotes()));

const searchQuotesInput = document.getElementById('searchQuotesInput');
const searchQuotesButton = document.getElementById('searchQuotesBtn');

if (searchQuotesButton) {
    searchQuotesButton.addEventListener('click', () => fetchForQuotes());
}

if (searchQuotesInput) {
    searchQuotesInput.addEventListener('keypress', function (event) {
        if (event.key === 'Enter') {
            fetchForQuotes();
        }
    });
}