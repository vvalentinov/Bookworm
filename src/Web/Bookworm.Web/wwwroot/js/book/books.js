import { updatePagination } from '../pagination.js';

const bookSearchInput = document.querySelector('.bookSearchInput');
const bookSearchButton = document.getElementById('bookSearchButton');

const isForUserBooks = document.getElementById('isForUserBooks')?.value.toLowerCase() === "true";

bookSearchButton?.addEventListener('click', () => fetchForBooks());
bookSearchInput?.addEventListener('keypress', function (event) {
    if (event.key === 'Enter') {
        fetchForBooks();
    }
});
$(function () {
    $('.languagesSelect').select2({
        placeholder: "Filter by language...",
        width: "100%",
        selectionCssClass: 'selectionContainer',
        dropdownCssClass: 'select2DropdownMenu',
    });

    let preventOpen = false;

    $('.languagesSelect').on('select2:unselecting', () => preventOpen = true);

    $('.languagesSelect').on('select2:opening', function (e) {
        if (preventOpen) {
            e.preventDefault();
            preventOpen = false;
        }
    });

    $('.languagesSelect').on('select2:select', () => fetchForBooks());
    $('.languagesSelect').on('select2:unselect', () => fetchForBooks());

    let url;
    if (isForUserBooks) {
        url = '/ApiBook/GetLanguagesInUserBooks';
    } else {
        const category = encodeURIComponent(document.getElementById('category')?.value);
        url = `/ApiBook/GetLanguagesInBookCategory?category=${category}`;
    }

    fetch(url)
        .then(res => res.json())
        .then(res => populateLanguagesOptions(res))
        .catch(res => console.log(res));
});

const fetchForBooks = (page) => {
    const category = document.getElementById('category')?.value;

    if (!page) { page = 1; }

    let input = bookSearchInput.value.trim();
    if (!input) { input = ''; }

    const languagesIds = getLanguagesIds();

    const model = {
        input,
        page,
        category,
        isForUserBooks,
        languagesIds
    };

    const token = document.getElementById('RequestVerificationToken').value;

    fetch('/ApiBook/SearchBooks', {
        method: 'POST',
        headers: { 'X-CSRF-TOKEN': token, 'Content-Type': 'application/json' },
        body: JSON.stringify(model)
    })
        .then(res => res.json())
        .then(res => {
            updateBooks(res.books);
            updatePagination(res, fetchForBooks);
        }).catch(err => console.log(err.message));
};

const updateBooks = (books) => {
    const bookSection = document.querySelector('.booksSection');
    bookSection.innerHTML = '';

    if (books?.length == 0) {
        const h2Element = document.createElement('h2');
        h2Element.textContent = 'No books found based on search!';
        h2Element.style = 'color: white';
        bookSection.appendChild(h2Element);
        return;
    }

    books.forEach(book => {
        const divElement = document.createElement('div');

        const anchorElement = document.createElement('a');
        anchorElement.href = `/Book/Details/${book.id}`;

        const imageElement = document.createElement('img');
        imageElement.title = book.title;
        imageElement.src = book.imageUrl;
        imageElement.alt = book.title;
        imageElement.className = 'imageZoom';
        imageElement.height = 300;

        anchorElement.appendChild(imageElement);
        divElement.appendChild(anchorElement);
        bookSection.appendChild(divElement);
    });
};

const getLanguagesIds = () => $('.languagesSelect').select2('data').map(x => x.id);
function populateLanguagesOptions(languages) {
    const select = document.querySelector('.languagesSelect');
    languages.forEach(language => {
        const optionEl = document.createElement('option');
        optionEl.setAttribute('value', language.id);
        optionEl.textContent = language.name;
        optionEl.addEventListener('click', () => fetchForBooks());
        select.appendChild(optionEl);
    });
}