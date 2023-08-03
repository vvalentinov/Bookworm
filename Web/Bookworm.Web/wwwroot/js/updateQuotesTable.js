//Radio Elements
const approvedQuotesRadioEl = document.getElementById('approvedQuotesRadio');
const unpprovedQuotesRadioEl = document.getElementById('unapprovedQuotesRadio');
const movieQuotesRadioEl = document.getElementById('movieQuotesRadio');
const bookQuotesRadioEl = document.getElementById('bookQuotesRadio');

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
                xhr.open('GET', `/api/UserQuote/SearchQuote?content=${searchValue}&type=ApprovedQuotes`, true);
                break;
            case 'unapprovedQuotesRadio':
                xhr.open('GET', `/api/UserQuote/SearchQuote?content=${searchValue}&type=UnapprovedQuotes`, true);
                break;
            case 'movieQuotesRadio':
                xhr.open('GET', `/api/UserQuote/SearchQuote?content=${searchValue}&type=MovieQuotes`, true);
                break;
            case 'bookQuotesRadio':
                xhr.open('GET', `/api/UserQuote/SearchQuote?content=${searchValue}&type=BookQuotes`, true);
                break;
        }
    } else {
        xhr.open('GET', `/api/UserQuote/SearchQuote?content=${searchValue}`, true);
    }

    xhr.onload = function () {
        if (this.status == 200) {
            updateQuotesTable(JSON.parse(this.response));
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
                    searchQuotesInput.placeholder = 'Search in your approved quotes...';
                    xhr.open('GET', '/api/UserQuote/GetQuotes?type=ApprovedQuotes', true);
                    break;
                case 'unapprovedQuotesRadio':
                    searchQuotesInput.placeholder = 'Search in your unapproved quotes...';
                    xhr.open('GET', '/api/UserQuote/GetQuotes?type=UnapprovedQuotes', true);
                    break;
                case 'movieQuotesRadio':
                    searchQuotesInput.placeholder = 'Search in your movie quotes...';
                    xhr.open('GET', '/api/UserQuote/GetQuotes?type=MovieQuotes', true);
                    break;
                case 'bookQuotesRadio':
                    searchQuotesInput.placeholder = 'Search in your book quotes...';
                    xhr.open('GET', '/api/UserQuote/GetQuotes?type=BookQuotes', true);
                    break;
            }
            xhr.onload = function () {
                if (this.status == 200) {
                    updateQuotesTable(JSON.parse(this.response));
                }
            }

            xhr.send();
        });
    }
}

// Function to create and append alert message
function alertMessage() {
    let mainElement = document.querySelector('main[role="main"]');

    let alertMessageContainer = document.createElement('div');
    alertMessageContainer.id = 'alertMessageContainer';
    let alertMessage = document.createElement('div');

    alertMessage.className = 'alert alert-info alert-dismissible fade show animate__animated animate__pulse';
    alertMessage.role = 'alert';
    alertMessage.textContent = 'Sorry! There were no quotes found based on your search!';

    let alertMessageButton = document.createElement('button');
    alertMessageButton.type = 'button';
    alertMessageButton.className = 'btn-close';
    alertMessageButton.setAttribute('data-bs-dismiss', 'alert');
    alertMessageButton.setAttribute('aria-label', 'Close');

    alertMessage.appendChild(alertMessageButton);
    alertMessageContainer.appendChild(alertMessage);

    setTimeout(() => {
        alertMessageContainer.remove();
    }, 5000);

    mainElement.appendChild(alertMessageContainer);
}

// Function to update quotes table with new data
function updateQuotesTable(data) {
    let tableElement = document.getElementsByClassName('table-responsive')[0];
    if (data.length == 0) {
        tableElement.style.display = 'none';

        alertMessage();
    } else {
        tableElement.style.display = '';
        const quotes = data.map((obj) => obj.content);
        filterTableRows(quotes);
    }
}

// Function to filter the table
function filterTableRows(filterValue) {
    let tableBodyElement = document.getElementsByClassName('table-body')[0];
    var rows = tableBodyElement.getElementsByTagName("tr");
    for (var i = rows.length - 1; i >= 0; i--) {
        if (rows[i].style.display == 'none') {
            rows[i].style.display = '';
        }
    }

    for (var i = rows.length - 1; i >= 0; i--) {
        var cellValue = rows[i].getElementsByTagName("td")[0].textContent;
        if (filterValue.includes(cellValue) == false) {
            rows[i].style.display = 'none';
        }
    }
}