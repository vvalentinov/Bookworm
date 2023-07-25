let approvedQuotesCheckbox = document.getElementById('flexRadioDefault1');
let unpprovedQuotesCheckbox = document.getElementById('flexRadioDefault2');

function approvedQuotes() {
    approvedQuotesCheckbox.addEventListener('change', function () {
        if (this.checked) {
            $.ajax({
                url: '/api/UserQuote/GetApprovedQuotes',
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    updateQuotesTable(data);
                }
            });
        }
    });
}

function unapprovedQuotes() {
    unpprovedQuotesCheckbox.addEventListener('change', function () {
        if (this.checked) {
            $.ajax({
                url: '/api/UserQuote/GetUnapprovedQuotes',
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    updateQuotesTable(data);
                }
            });
        }
    });
}


function updateQuotesTable(data) {
    let tableElement = document.getElementsByClassName('table-responsive')[0];
    if (data.length == 0) {
        // tableElement.setAttribute('display', 'none');
        tableElement.style.display = 'none';
    } else {
        tableElement.style.display = '';
        let tableBodyElement = document.getElementsByClassName('table-body')[0];

        while (tableBodyElement.lastElementChild) {
            tableBodyElement.removeChild(tableBodyElement.lastElementChild);
        }

        for (let i = 0; i < data.length; i++) {
            let tableRowElement = document.createElement('tr');

            let thElement = document.createElement('th');
            thElement.className = 'text-center align-middle';
            thElement.textContent = (i + 1).toString();

            let tdContent = document.createElement('td');
            tdContent.className = 'align-middle';
            tdContent.textContent = data[i].content;

            let tdAuthor = document.createElement('td');
            tdAuthor.className = 'text-center align-middle';
            if (data[i].authorName == null) {
                let iconElement = document.createElement('i');
                iconElement.className = 'fa-regular fa-circle-xmark fa-xl x-icon';
                tdAuthor.appendChild(iconElement);
            } else {
                tdAuthor.textContent = data[i].authorName;
            }

            let tdMovie = document.createElement('td');
            tdMovie.className = 'text-center align-middle';
            if (data[i].movieTitle == null) {
                let iconElement = document.createElement('i');
                iconElement.className = 'fa-regular fa-circle-xmark fa-xl x-icon';
                tdMovie.appendChild(iconElement);
            } else {
                tdMovie.textContent = data[i].movieTitle;
            }


            let tdBook = document.createElement('td');
            tdBook.className = 'text-center align-middle';
            if (data[i].bookTitle == null) {
                let iconElement = document.createElement('i');
                iconElement.className = 'fa-regular fa-circle-xmark fa-xl x-icon';
                tdBook.appendChild(iconElement);
            } else {
                tdBook.textContent = data[i].bookTitle;
            }

            let tdIsApproved = document.createElement('td');
            tdIsApproved.className = 'text-center align-middle';
            if (data[i].isApproved == true) {
                let iconElement = document.createElement('i');
                iconElement.className = 'fa-solid fa-thumbs-up fa-xl';
                tdIsApproved.appendChild(iconElement);
            } else {
                let iconElement = document.createElement('i');
                iconElement.className = 'fa-solid fa-thumbs-down fa-xl';
                tdIsApproved.appendChild(iconElement);
            }

            let tdEdit = document.createElement('td');
            tdEdit.className = 'text-center align-middle';
            let anchorElement = document.createElement('a');
            anchorElement.className = 'btn btn-warning';

            anchorElement.href = `Edit/${data[i].id}`;
            tdEdit.appendChild(anchorElement);
            anchorElement.textContent = 'Edit';

            tableRowElement.appendChild(thElement);
            tableRowElement.appendChild(tdContent);
            tableRowElement.appendChild(tdAuthor);
            tableRowElement.appendChild(tdMovie);
            tableRowElement.appendChild(tdBook);
            tableRowElement.appendChild(tdIsApproved);
            tableRowElement.appendChild(tdEdit);

            tableBodyElement.append(tableRowElement);
        }
    }

}