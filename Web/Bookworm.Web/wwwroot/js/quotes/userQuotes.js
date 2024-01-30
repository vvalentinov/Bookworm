import { updateQuotesTable } from './updateQuotesTable.js';

// Search Elements
const searchQuotesInput = document.getElementById('searchQuotesInput');
const searchQuotesButton = document.getElementById('searchQuotesBtn');


// Function to search for quotes
function searchQuotes() {
    let searchValue = searchQuotesInput.value;
    if (searchValue == '') {
        return;
    }

    let alertMessage = document.getElementById('alertMessageContainer');
    if (alertMessage) {
        return;
    }

    var xhr = new XMLHttpRequest();

    let checkedRadio = getCheckedRadio();

    if (checkedRadio != undefined) {
        switch (checkedRadio.id) {
            case 'approvedQuotesRadio':
                xhr.open('GET', `/ApiQuote/SearchUserQuotesByContent?content=${searchValue}&type=ApprovedQuote`, true);
                break;
            case 'unapprovedQuotesRadio':
                xhr.open('GET', `/ApiQuote/SearchUserQuotesByContent?content=${searchValue}&type=UnapprovedQuote`, true);
                break;
            case 'movieQuotesRadio':
                xhr.open('GET', `/ApiQuote/SearchUserQuotesByContent?content=${searchValue}&type=MovieQuote`, true);
                break;
            case 'bookQuotesRadio':
                xhr.open('GET', `/ApiQuote/SearchUserQuotesByContent?content=${searchValue}&type=BookQuote`, true);
                break;
            case 'generalQuotesRadio':
                xhr.open('GET', `/ApiQuote/SearchUserQuotesByContent?content=${searchValue}&type=GeneralQuote`, true);
                break;
            case 'likedQuotesRadio':
                xhr.open('GET', `/ApiQuote/SearchUserQuotesByContent?content=${searchValue}&type=LikedQuote`, true);
                break;
        }
    } else {
        xhr.open('GET', `/ApiQuote/SearchUserQuotesByContent?content=${searchValue}`, true);
    }

    xhr.onload = function () {
        if (this.status == 200) {
            updateQuotesTable(JSON.parse(this.response));
        } else {
            alert('error!');
        }
    }

    xhr.send();
}


// Add event listeners to search elements
searchQuotesButton.addEventListener('click', searchQuotes);
searchQuotesInput.addEventListener('keypress', function (event) {
    if (event.key === 'Enter') {
        searchQuotes();
    }
});


// Function to retrieve checked radio element
function getCheckedRadio() {
    const radioButtons = document.getElementsByName('btnradio');
    for (let i = 0; i < radioButtons.length; i++) {
        if (radioButtons[i].checked) {
            return radioButtons[i];
        }
    }
}

// Function to add 'change' event listener to radio elements
radioAddEventListener();
function radioAddEventListener() {
    const radioButtons = document.getElementsByName('btnradio');
    for (let i = 0; i < radioButtons.length; i++) {
        radioButtons[i].addEventListener('change', function () {
            searchQuotesInput.value = '';
            var xhr = new XMLHttpRequest();
            switch (radioButtons[i].id) {
                case 'approvedQuotesRadio':
                    searchQuotesInput.placeholder = 'Search in approved quotes...';
                    xhr.open('GET', '/ApiQuote/GetUserApprovedQuotes', true);
                    break;
                case 'unapprovedQuotesRadio':
                    searchQuotesInput.placeholder = 'Search in unapproved quotes...';
                    xhr.open('GET', '/ApiQuote/GetUserUnapprovedQuotes', true);
                    break;
                case 'movieQuotesRadio':
                    searchQuotesInput.placeholder = 'Search in movie quotes...';
                    xhr.open('GET', '/ApiQuote/GetUserQuotesByType?type=MovieQuote', true);
                    break;
                case 'bookQuotesRadio':
                    searchQuotesInput.placeholder = 'Search in book quotes...';
                    xhr.open('GET', '/ApiQuote/GetUserQuotesByType?type=BookQuote', true);
                    break;
                case 'generalQuotesRadio':
                    searchQuotesInput.placeholder = 'Search in general quotes...';
                    xhr.open('GET', '/ApiQuote/GetUserQuotesByType?type=GeneralQuote', true);
                    break;
                case 'likedQuotesRadio':
                    searchQuotesInput.placeholder = 'Search in liked quotes...';
                    xhr.open('GET', '/ApiQuote/GetUserLikedQuotes', true);
                    break;
            }
            xhr.onload = function () {
                if (this.status == 200) {
                    updateQuotesTable(JSON.parse(this.response));
                } else {
                    alert('error');
                }
            }

            xhr.send();
        });
    }
}