import { fetchForQuotes } from './fetchForQuotes.js';

const quoteTypeButtons = [
    document.getElementById('movie-quotes'),
    document.getElementById('book-quotes'),
    document.getElementById('general-quotes'),
    document.getElementById('liked-quotes')
];

quoteTypeButtons.forEach(button => {
    if (button) {
        button.addEventListener('click', () => fetchForQuotes());
    }
})

const sortRadioButtons = [
    document.getElementById('newest-to-oldest'),
    document.getElementById('oldest-to-newest'),
    document.getElementById('likes-count-desc')
];

sortRadioButtons.forEach(button => {
    if (button) {
        button.addEventListener('click', () => fetchForQuotes());
    }
});

const searchQuotesInput = document.getElementById('searchQuotesInput');
const searchQuotesButton = document.getElementById('searchQuotesBtn');

if (searchQuotesButton) {
    searchQuotesButton.addEventListener('click', fetchForQuotes);
}

if (searchQuotesInput) {
    searchQuotesInput.addEventListener('keypress', function (event) {
        if (event.key === 'Enter') {
            fetchForQuotes();
        }
    });
}