import { updatePagination } from './pagination.js';

let isForUserBooks = document.getElementById('isForUserBooks')?.value.toLowerCase() === "true";

const bookSearchInput = document.querySelector('.bookSearchInput');
const bookSearchButton = document.getElementById('bookSearchButton');

bookSearchButton.addEventListener('click', () => fetchForBooks());
bookSearchInput.addEventListener('keypress', function (event) {
    if (event.key === 'Enter') {
        fetchForBooks(1, getLanguagesIds());
    }
});
$(function () {
    $('.languagesSelect').select2({
        placeholder: "Filter by language...",
        width: "100%",
    });

    $('.languagesSelect').on('select2:select', function () {
        fetchForBooks(1, getLanguagesIds());
    });

    const select = document.querySelector('.languagesSelect');
    const category = encodeURIComponent(new URLSearchParams(window.location.search).get('category'));
    let url;
    if (!isForUserBooks) {
        url = `/ApiBook/GetLanguagesInBookCategory?category=${category}`;
    } else {
        url = '/ApiBook/GetLanguagesInUserBooks';
    }

    fetch(url)
        .then(res => res.json())
        .then(res => {
            res.forEach(model => {
                const optionEl = document.createElement('option');
                optionEl.setAttribute('value', model.id);
                optionEl.textContent = model.name;
                optionEl.addEventListener('click', () => fetchForBooks());
                select.appendChild(optionEl);
            });
        }).catch(res => console.log(res));
});

const fetchForBooks = (page, languagesIds) => {
    //isForUserBooks = isForUserBooks.toLowerCase() === "true";

    if (!isForUserBooks) { isForUserBooks = false; }

    const category = document.getElementById('categoryName')?.value;

    if (!page) { page = 1; }

    let input = bookSearchInput.value.trim();
    if (!input) { input = ''; }

    const model = {
        Input: input,
        Page: page,
        Category: category,
        IsForUserBooks: isForUserBooks,
        LanguagesIds: languagesIds
    };

    const token = document.getElementById('RequestVerificationToken').value;

    fetch('/ApiBook/SearchBooks', {
        method: 'POST',
        headers:
        {
            'X-CSRF-TOKEN': token,
            'Content-Type': 'application/json'
        },
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

    if (books?.length > 0) {
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
    } else {
        const h2Element = document.createElement('h2');
        h2Element.textContent = 'No books found based on search!';
        h2Element.style = 'color: white';
        bookSection.appendChild(h2Element);
    }
};

function getLanguagesIds() {
    return $('.languagesSelect').select2('data').map(x => x.id);
}